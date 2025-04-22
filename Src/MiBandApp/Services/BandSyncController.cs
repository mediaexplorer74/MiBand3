// Decompiled with JetBrains decompiler
// Type: MiBandApp.Services.BandSyncController
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.exe

using MetroLog;
using MiBand.SDK.Core;
using MiBand.SDK.Data;
using MiBandApp.Tools;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Storage;

#nullable disable
namespace MiBandApp.Services
{
  public class BandSyncController
  {
    private const string LastSyncTimeKey = "LastSyncTime";
    private const int SyncIntervalMin = 5;
    private readonly BandController _bandController;
    private readonly DataManager _dataManager;
    private readonly DiagnosticsService _diagnosticsService;
    private readonly ILogger _log;
    private readonly MonitorableSource<RealtimeStepsData> _realtimeSteps = new MonitorableSource<RealtimeStepsData>();
    private readonly MonitorableSource<HeartRateMeasurement> _latestHeartRate = new MonitorableSource<HeartRateMeasurement>();
    private readonly MonitorableSource<MiBand.SDK.Core.BatteryInfo> _batteryInfo = new MonitorableSource<MiBand.SDK.Core.BatteryInfo>();
    private readonly MonitorableSource<BandSyncState> _syncState = new MonitorableSource<BandSyncState>();

    public BandSyncController(
      BandController bandController,
      DataManager dataManager,
      ILogManager logManager,
      DiagnosticsService diagnosticsService)
    {
      this._bandController = bandController;
      this._dataManager = dataManager;
      this._log = logManager.GetLogger<BandSyncController>();
      this._diagnosticsService = diagnosticsService;
    }

    public Monitorable<RealtimeStepsData> RealtimeSteps => this._realtimeSteps.Monitorable;

    public Monitorable<HeartRateMeasurement> LatestHeartRate => this._latestHeartRate.Monitorable;

    public Monitorable<MiBand.SDK.Core.BatteryInfo> BatteryInfo => this._batteryInfo.Monitorable;

    public Monitorable<BandSyncState> SyncState => this._syncState.Monitorable;

    public DateTimeOffset LastSyncTime
    {
      get
      {
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        if (!((IDictionary<string, object>) localSettings.Values).ContainsKey(nameof (LastSyncTime)))
          this.LastSyncTime = new DateTimeOffset(new DateTime(2016, 7, 1));
        return new DateTimeOffset(DateTime.SpecifyKind(DateTime.FromBinary((long) ((IDictionary<string, object>) localSettings.Values)[nameof (LastSyncTime)]), DateTimeKind.Utc).ToLocalTime());
      }
      set
      {
        ((IDictionary<string, object>) ApplicationData.Current.LocalSettings.Values)[nameof (LastSyncTime)] = (object) value.ToUniversalTime().DateTime.ToBinary();
      }
    }

    public void Refresh()
    {
      if (!this._bandController.TryStartOperation(CommunicationOperation.Refreshing))
        return;
      Task.Run((Func<Task>) (async () =>
      {
        try
        {
          switch (await this._bandController.Connect().ConfigureAwait(false))
          {
            case BandController.ConnectResult.Fail:
              break;
            case BandController.ConnectResult.BindingStarted:
              this._syncState.Value = BandSyncState.Binding;
              break;
            default:
              await this.UpdateSteps().ConfigureAwait(false);
              await this.UpdateBatteryInfo().ConfigureAwait(false);
              this.Sync().NoAwait();
              break;
          }
        }
        finally
        {
          this._bandController.StopOperation(CommunicationOperation.Refreshing);
        }
      })).ConfigureAwait(false).NoAwait();
    }

    public async Task MeasureHeartRate()
    {
      if (!this._bandController.TryStartOperation(CommunicationOperation.MeasuringHeartRate))
        return;
      try
      {
        if (this._bandController.Status != MiBandStatus.Connected)
          throw new InvalidOperationException("Can't measure heart rate when device not connected");
        HeartRateMeasurement hr = await this._bandController.MiBand.GetHeartRate().ConfigureAwait(false);
        if (hr == null)
          return;
        this._dataManager.AddHeartRateMeasurement(hr);
        this._latestHeartRate.Value = hr;
      }
      finally
      {
        this._bandController.StopOperation(CommunicationOperation.MeasuringHeartRate);
      }
    }

    private async Task UpdateSteps()
    {
      if (!this._bandController.TryStartOperation(CommunicationOperation.UpdatingSteps))
        return;
      try
      {
        this._realtimeSteps.Value = await this._bandController.MiBand.GetRealtimeSteps().ConfigureAwait(false);
        this._dataManager.UpdateRealtimeSteps(this.RealtimeSteps.Value);
      }
      catch (Exception ex)
      {
      }
      finally
      {
        this._bandController.StopOperation(CommunicationOperation.UpdatingSteps);
      }
    }

    private async Task UpdateBatteryInfo()
    {
      if (this.BatteryInfo.Value != null && this.TooEarlyToSync())
        return;
      if (!this._bandController.TryStartOperation(CommunicationOperation.UpdatingBattery))
        return;
      try
      {
        this._batteryInfo.Value = await this._bandController.MiBand.GetBatteryInfo().ConfigureAwait(false);
      }
      catch (Exception ex)
      {
      }
      finally
      {
        this._bandController.StopOperation(CommunicationOperation.UpdatingBattery);
      }
    }

    private async Task Sync()
    {
      if (!this._bandController.TryStartOperation(CommunicationOperation.Syncing))
        return;
      try
      {
        if (this._bandController.BindingState.Value != BindingState.Binded || this.SyncState.Value == BandSyncState.InProgress || this.SyncState.Value == BandSyncState.Success && this.TooEarlyToSync())
          return;
        this._syncState.Value = BandSyncState.InProgress;
        ConfiguredTaskAwaitable configuredTaskAwaitable = this._bandController.MiBand.SetDateTime((DateTimeOffset) DateTime.Now).ConfigureAwait(false);
        await configuredTaskAwaitable;
        int tryCnt = 0;
        int maxTryCnt = 3;
        SynchronizationDataPackage result;
        do
        {
          DateTimeOffset startTime = this.LastSyncTime;
          this._log.Info(string.Format("Starting synchronization attempt from ${0}", (object) startTime), (Exception) null);
          DateTimeOffset dateTimeOffset1 = startTime;
          DateTimeOffset now = DateTimeOffset.Now;
          DateTimeOffset dateTimeOffset2 = now.AddDays(-7.0);
          if (dateTimeOffset1 < dateTimeOffset2)
          {
            maxTryCnt = 4;
            if (tryCnt == 2)
            {
              now = DateTimeOffset.Now;
              startTime = now.AddDays(-7.0);
            }
            if (tryCnt == 3)
            {
              now = DateTimeOffset.Now;
              startTime = now.AddDays(-5.0);
            }
            this._log.Info(string.Format("Adjusted sync time to ${0}", (object) startTime), (Exception) null);
          }
          result = await this._bandController.MiBand.GetActivityData(startTime).ConfigureAwait(false);
          if (result == null)
            this._log.Warn("Synchronization attempt failed", (Exception) null);
          else
            goto label_13;
        }
        while (++tryCnt < maxTryCnt);
        goto label_17;
label_13:
        this._dataManager.AddRawActivityData(result.ActivitySeries);
        this._dataManager.AddHeartRateMeasurements((IEnumerable<HeartRateMeasurement>) result.HeartRateMeasurements);
        this.LastSyncTime = (DateTimeOffset) DateTime.Now;
        this._syncState.Value = BandSyncState.Success;
        this._log.Info("Submitting data from " + (object) result.TimeBegin + " to " + (object) result.TimeEnd + " " + (object) result.TotalMinutes + " minutes", (Exception) null);
        configuredTaskAwaitable = this._bandController.MiBand.SubmitActivityDataFragmentReceived(result.TimeEnd, 0).ConfigureAwait(false);
        await configuredTaskAwaitable;
        this._diagnosticsService.SaveData(result.ActivitySeries, this._bandController.DeviceInfo.Value);
        this._log.Info("Synchronization completed", (Exception) null);
        return;
      }
      catch (Exception ex)
      {
      }
      finally
      {
        this._bandController.StopOperation(CommunicationOperation.Syncing);
      }
label_17:
      this._log.Error("Synchronization failed", (Exception) null);
      this._syncState.Value = BandSyncState.Failed;
    }

    private bool TooEarlyToSync()
    {
      return ((DateTimeOffset) DateTime.Now - this.LastSyncTime).TotalMinutes < 5.0;
    }
  }
}
