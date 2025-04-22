// Decompiled with JetBrains decompiler
// Type: MiBandApp.Services.BandController
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.exe

using MetroLog;
using MiBand.SDK;
using MiBand.SDK.Bluetooth;
using MiBand.SDK.Core;
using MiBandApp.Tools;
using Microsoft.HockeyApp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

#nullable disable
namespace MiBandApp.Services
{
  public class BandController
  {
    private const string LastBindedDeviceIdKey = "LastSyncedDeviceId";
    private readonly MiBandLocator _miBandLocator;
    private readonly MiBandApp.Storage.Settings.Settings _settings;
    private readonly ILogger _log;
    private readonly object _operationLocker = new object();
    private readonly MonitorableSource<MiBandApp.Services.BindingState> _bindingState = new MonitorableSource<MiBandApp.Services.BindingState>();
    private readonly MonitorableSource<BandDeviceInfo> _deviceInfo = new MonitorableSource<BandDeviceInfo>();
    private readonly MonitorableSource<MiBandApp.Services.CommunicationOperation> _communicationOperation = new MonitorableSource<MiBandApp.Services.CommunicationOperation>();
    private IMiBand _miBand;
    private MiBandStatus _status;

    public BandController(MiBandLocator miBandLocator, MiBandApp.Storage.Settings.Settings settings, ILogManager logManager)
    {
      this._miBandLocator = miBandLocator;
      this._settings = settings;
      this._log = logManager.GetLogger<BandController>();
    }

    public event EventHandler StatusChanged;

    public IMiBand MiBand
    {
      get
      {
        if (this._miBand == null)
          throw new MiBandNotPairedException();
        return this._miBand.ConnectionStatus != ConnectionStatus.Error && this._miBand.ConnectionStatus != ConnectionStatus.Unreachable ? this._miBand : throw new MiBandInFaultStateException();
      }
      private set
      {
        if (this._miBand == value)
          return;
        if (this._miBand != null)
          this._miBand.ConnectionStatusChanged -= new EventHandler(this.OnMiBandConnectionStatusChanged);
        this._miBand = value;
        if (this._miBand == null)
          return;
        this._miBand.ConnectionStatusChanged += new EventHandler(this.OnMiBandConnectionStatusChanged);
      }
    }

    public MiBandStatus Status
    {
      get => this._status;
      set
      {
        if (this._status == value)
          return;
        this._status = value;
        this.UpdateBindingState();
        EventHandler statusChanged = this.StatusChanged;
        if (statusChanged == null)
          return;
        statusChanged((object) this, EventArgs.Empty);
      }
    }

    public Monitorable<MiBandApp.Services.BindingState> BindingState
    {
      get => this._bindingState.Monitorable;
    }

    public Monitorable<BandDeviceInfo> DeviceInfo => this._deviceInfo.Monitorable;

    public Monitorable<MiBandApp.Services.CommunicationOperation> CommunicationOperation
    {
      get => this._communicationOperation.Monitorable;
    }

    public bool FirmwareUpdateRecommended
    {
      get
      {
        if (this.DeviceInfo == null)
          return false;
        string str = this.DeviceInfo.Value.FirmwareVersion.ToString();
        return str == "5.15.7.2" || str == "5.15.11.19" || str == "4.15.11.19";
      }
    }

    private string LastBindedDeviceId
    {
      get
      {
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        if (!((IDictionary<string, object>) localSettings.Values).ContainsKey("LastSyncedDeviceId"))
          ((IDictionary<string, object>) localSettings.Values).Add("LastSyncedDeviceId", (object) "new");
        return (string) ((IDictionary<string, object>) localSettings.Values)["LastSyncedDeviceId"];
      }
      set
      {
        ((IDictionary<string, object>) ApplicationData.Current.LocalSettings.Values)["LastSyncedDeviceId"] = (object) value;
        this.UpdateBindingState();
      }
    }

    public bool TryStartOperation(MiBandApp.Services.CommunicationOperation requestedOperation)
    {
      lock (this._operationLocker)
      {
        if (this.CommunicationOperation.Value.HasFlag((Enum) requestedOperation))
          return false;
        this._communicationOperation.Value |= requestedOperation;
        return true;
      }
    }

    public void StopOperation(MiBandApp.Services.CommunicationOperation toStopOperation)
    {
      lock (this._operationLocker)
        this._communicationOperation.Value &= ~toStopOperation;
    }

    public async Task<BandController.ConnectResult> Connect()
    {
      if (!this.TryStartOperation(MiBandApp.Services.CommunicationOperation.Connecting))
        return BandController.ConnectResult.Fail;
      try
      {
        if (this.Status == MiBandStatus.Connected)
          return BandController.ConnectResult.Success;
        string deviceId = this.LastBindedDeviceId;
        if (deviceId == "new" || deviceId == "unbinded")
          deviceId = (string) null;
        List<IMiBandInfo> list = (await this._miBandLocator.FindMiBands().ConfigureAwait(true)).ToList<IMiBandInfo>();
        if (!list.Any<IMiBandInfo>())
        {
          this.Status = MiBandStatus.NotPairedToPhone;
          return BandController.ConnectResult.Fail;
        }
        if (list.Count > 1)
        {
          this.Status = MiBandStatus.MoreThanOnePaired;
          return BandController.ConnectResult.Fail;
        }
        IMiBandInfo miBandInfo = list.FirstOrDefault<IMiBandInfo>((Func<IMiBandInfo, bool>) (t => t.DeviceId == deviceId)) ?? list.First<IMiBandInfo>();
        this._miBand?.Dispose();
        this.MiBand = await this._miBandLocator.CreateMiBand(miBandInfo).ConfigureAwait(false);
        if (this._miBand == null)
        {
          this.Status = MiBandStatus.NotPairedToPhone;
          return BandController.ConnectResult.Fail;
        }
        this.Status = MiBandStatus.PairedToPhone;
        this._deviceInfo.Value = await this.MiBand.GetBandDeviceInfo().ConfigureAwait(false);
        if (this.BindingState.Value == MiBandApp.Services.BindingState.Unbinded || this.BindingState.Value == MiBandApp.Services.BindingState.New)
          return BandController.ConnectResult.Fail;
        HockeyClient.Current.TrackEvent("MiBandModel." + (object) this.DeviceInfo.Value.Model);
        switch (await this.MiBand.Authenticate(this._settings.GetSavedUserInfo()).ConfigureAwait(false))
        {
          case AuthenticationResult.None:
            Debugger.Break();
            break;
          case AuthenticationResult.Fail:
            this.LastBindedDeviceId = "unbinded";
            break;
          case AuthenticationResult.BindingStarted:
            this._log.Warn("Initiating binding", (Exception) null);
            return BandController.ConnectResult.BindingStarted;
          case AuthenticationResult.Success:
            this.LastBindedDeviceId = this.MiBand.DeviceId;
            break;
          default:
            throw new ArgumentOutOfRangeException();
        }
        return this.BindingState.Value == MiBandApp.Services.BindingState.Binded ? BandController.ConnectResult.Success : BandController.ConnectResult.Fail;
      }
      catch (CommunicationException ex)
      {
        return BandController.ConnectResult.Fail;
      }
      catch (MiBandException ex)
      {
        return BandController.ConnectResult.Fail;
      }
      catch (Exception ex)
      {
        return BandController.ConnectResult.Fail;
      }
      finally
      {
        this.StopOperation(MiBandApp.Services.CommunicationOperation.Connecting);
      }
    }

    public void Bind()
    {
      if (this.Status != MiBandStatus.Connected)
        return;
      this.LastBindedDeviceId = this._miBand.DeviceId;
    }

    public void Unbind() => this.LastBindedDeviceId = "unbinded";

    public void Reset() => this.Status = MiBandStatus.Unknown;

    private void OnMiBandConnectionStatusChanged(object sender, EventArgs eventArgs)
    {
      switch (this._miBand.ConnectionStatus)
      {
        case ConnectionStatus.None:
          break;
        case ConnectionStatus.Unreachable:
          this.Status = MiBandStatus.Unreachable;
          break;
        case ConnectionStatus.Ok:
          this.Status = MiBandStatus.Connected;
          break;
        case ConnectionStatus.Error:
          this.Status = MiBandStatus.Error;
          break;
        case ConnectionStatus.Disposed:
          this.Status = MiBandStatus.Unknown;
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    private void UpdateBindingState() => this._bindingState.Value = this.GetBindingState();

    private MiBandApp.Services.BindingState GetBindingState()
    {
      if (this.Status != MiBandStatus.Connected)
        return MiBandApp.Services.BindingState.None;
      if (this.LastBindedDeviceId == "new")
        return MiBandApp.Services.BindingState.New;
      return !(this.LastBindedDeviceId == "unbinded") ? MiBandApp.Services.BindingState.Binded : MiBandApp.Services.BindingState.Unbinded;
    }

    public enum ConnectResult
    {
      Fail,
      Success,
      BindingStarted,
    }
  }
}
