// Decompiled with JetBrains decompiler
// Type: MiBand.SDK.Bluetooth.BluetoothDeviceBase
// Assembly: MiBand.SDK, Version=0.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 152FA565-3249-4054-A361-01D9C0AFA7F1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBand.SDK.dll

using MiBand.SDK.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Foundation;

#nullable disable
namespace MiBand.SDK.Bluetooth
{
  internal abstract class BluetoothDeviceBase : IBluetoothDevice, IDisposable
  {
    private ConnectionStatus _connectionStatus;
    private readonly List<GattCharacteristic> _notificationEnabled = new List<GattCharacteristic>();
    private readonly Dictionary<Guid, GattDeviceService> _secondaryServices = new Dictionary<Guid, GattDeviceService>();

    protected BluetoothDeviceBase(GattDeviceService defaultGattService)
    {
      this.DefaultGattService = defaultGattService;
    }

    public event EventHandler ConnectionStatusChanged;

    public ConnectionStatus ConnectionStatus
    {
      get => this._connectionStatus;
      protected set
      {
        if (value == this._connectionStatus)
          return;
        this._connectionStatus = value;
        EventHandler connectionStatusChanged = this.ConnectionStatusChanged;
        if (connectionStatusChanged == null)
          return;
        connectionStatusChanged((object) this, EventArgs.Empty);
      }
    }

    public string DeviceId => this.DefaultGattService.DeviceId;

    public GattDeviceService DefaultGattService { get; protected set; }

    public virtual void Dispose()
    {
      this.DefaultGattService?.Dispose();
      this.DefaultGattService = (GattDeviceService) null;
      foreach (GattDeviceService gattDeviceService in this._secondaryServices.Values)
        gattDeviceService?.Dispose();
      this._secondaryServices.Clear();
      GC.SuppressFinalize((object) this);
      GC.Collect();
      this.ConnectionStatus = ConnectionStatus.Disposed;
    }

    protected internal GattDeviceService GetSecondaryService(Guid serviceGuid)
    {
      if (this._secondaryServices.ContainsKey(serviceGuid))
        return this._secondaryServices[serviceGuid];
      GattDeviceService gattService = this.DefaultGattService.Device.GetGattService(serviceGuid);
      this._secondaryServices[serviceGuid] = gattService;
      return gattService;
    }

    protected internal async Task<byte[]> ReadCharacteristic(
      GattCharacteristic characteristic,
      BluetoothCacheMode mode = 1)
    {
      this.CheckConnectionStatus();
      GattReadResult read;
      try
      {
        read = await characteristic.ReadValueAsync(mode).AsTask<GattReadResult>().ConfigureAwait(false);
      }
      catch
      {
        this.ConnectionStatus = ConnectionStatus.Error;
        throw;
      }
      this.CheckReadResult(read);
      return read.Value.ToArraySafe();
    }

    protected internal async Task WriteCharacteristic(
      GattCharacteristic characteristic,
      byte[] data,
      bool withoutResponse = false)
    {
      this.CheckConnectionStatus();
      GattCommunicationStatus write;
      try
      {
        write = await characteristic.WriteValueAsync(data.AsBuffer(), withoutResponse ? (GattWriteOption) 1 : (GattWriteOption) 0).AsTask<GattCommunicationStatus>().ConfigureAwait(false);
      }
      catch
      {
        this.ConnectionStatus = ConnectionStatus.Error;
        throw;
      }
      this.CheckWriteResult(write);
    }

    internal async Task<NotifyResponse> WriteCharacteristicGetResponse(
      GattCharacteristic characteristic,
      byte[] data,
      Task delayTask,
      int payloadOffset = 0)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      BluetoothDeviceBase.\u003C\u003Ec__DisplayClass20_0 cDisplayClass200 = new BluetoothDeviceBase.\u003C\u003Ec__DisplayClass20_0();
      // ISSUE: reference to a compiler-generated field
      cDisplayClass200.\u003C\u003E4__this = this;
      // ISSUE: reference to a compiler-generated field
      cDisplayClass200.payloadOffset = payloadOffset;
      if (delayTask == null)
        throw new ArgumentNullException(string.Format("Argument {0} shouldn't be null", (object) nameof (delayTask)));
      // ISSUE: reference to a compiler-generated field
      cDisplayClass200.notifyReceivedTcs = new TaskCompletionSource<NotifyResponse>((object) null);
      // ISSUE: method pointer
      TypedEventHandler<GattCharacteristic, GattValueChangedEventArgs> valueChangedDelegate = new TypedEventHandler<GattCharacteristic, GattValueChangedEventArgs>((object) cDisplayClass200, __methodptr(\u003CWriteCharacteristicGetResponse\u003Eb__0));
      GattCharacteristic gattCharacteristic = characteristic;
      WindowsRuntimeMarshal.AddEventHandler<TypedEventHandler<GattCharacteristic, GattValueChangedEventArgs>>(new Func<TypedEventHandler<GattCharacteristic, GattValueChangedEventArgs>, EventRegistrationToken>(gattCharacteristic.add_ValueChanged), new Action<EventRegistrationToken>(gattCharacteristic.remove_ValueChanged), valueChangedDelegate);
      NotifyResponse notifyResponse = (NotifyResponse) null;
      try
      {
        await this.EnableNotifications(characteristic).ConfigureAwait(false);
        await this.WriteCharacteristic(characteristic, data, ((Enum) (object) characteristic.CharacteristicProperties).HasFlag((Enum) (object) (GattCharacteristicProperties) 4)).ConfigureAwait(false);
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        if (await Task.WhenAny((Task) cDisplayClass200.notifyReceivedTcs.Task, delayTask).ConfigureAwait(false) == cDisplayClass200.notifyReceivedTcs.Task)
        {
          // ISSUE: reference to a compiler-generated field
          notifyResponse = cDisplayClass200.notifyReceivedTcs.Task.Result;
        }
      }
      finally
      {
        await this.DisableNotifications(characteristic).ConfigureAwait(false);
        WindowsRuntimeMarshal.RemoveEventHandler<TypedEventHandler<GattCharacteristic, GattValueChangedEventArgs>>(new Action<EventRegistrationToken>(characteristic.remove_ValueChanged), valueChangedDelegate);
      }
      return notifyResponse;
    }

    internal Task<NotifyResponse> WriteCharacteristicGetResponse(
      GattCharacteristic characteristic,
      byte[] data,
      TimeSpan waitTimeSpan = default (TimeSpan),
      int payloadOffset = 0)
    {
      if (waitTimeSpan == new TimeSpan())
        waitTimeSpan = TimeSpan.FromSeconds(3.0);
      return this.WriteCharacteristicGetResponse(characteristic, data, Task.Delay(waitTimeSpan), payloadOffset);
    }

    protected internal async Task EnableNotifications(GattCharacteristic characteristic)
    {
      if (this._notificationEnabled.Contains(characteristic))
        return;
      if (!((Enum) (object) characteristic.CharacteristicProperties).HasFlag((Enum) (object) (GattCharacteristicProperties) 16))
      {
        Debugger.Break();
      }
      else
      {
        this._notificationEnabled.Add(characteristic);
        GattCommunicationStatus communicationStatus = await characteristic.WriteClientCharacteristicConfigurationDescriptorAsync((GattClientCharacteristicConfigurationDescriptorValue) 1).AsTask<GattCommunicationStatus>().ConfigureAwait(false);
      }
    }

    protected internal async Task DisableNotifications(GattCharacteristic characteristic)
    {
      if (!this._notificationEnabled.Contains(characteristic))
        return;
      if (!((Enum) (object) characteristic.CharacteristicProperties).HasFlag((Enum) (object) (GattCharacteristicProperties) 16))
      {
        Debugger.Break();
      }
      else
      {
        this._notificationEnabled.Remove(characteristic);
        GattCommunicationStatus communicationStatus = await characteristic.WriteClientCharacteristicConfigurationDescriptorAsync((GattClientCharacteristicConfigurationDescriptorValue) 0).AsTask<GattCommunicationStatus>().ConfigureAwait(false);
      }
    }

    protected internal GattCharacteristic GetCharacteristic(
      Guid characteristicGuid,
      GattDeviceService gattDeviceService = null)
    {
      if (gattDeviceService == null)
        gattDeviceService = this.DefaultGattService;
      try
      {
        return gattDeviceService.GetCharacteristics(characteristicGuid)[0];
      }
      catch (Exception ex)
      {
        this.ConnectionStatus = ConnectionStatus.Error;
        throw;
      }
    }

    private void OnNotifyResponse(
      TaskCompletionSource<NotifyResponse> notifyResponceTcs,
      GattValueChangedEventArgs args,
      int payloadOffset)
    {
      try
      {
        if (args.CharacteristicValue == null)
        {
          notifyResponceTcs.TrySetResult((NotifyResponse) null);
        }
        else
        {
          NotifyResponse result = new NotifyResponse(args.CharacteristicValue.ToArraySafe(), payloadOffset);
          notifyResponceTcs.TrySetResult(result);
        }
      }
      catch
      {
        notifyResponceTcs.TrySetResult((NotifyResponse) null);
      }
    }

    private void CheckReadResult(GattReadResult readResult)
    {
      if (readResult.Status == 1)
      {
        this.ConnectionStatus = ConnectionStatus.Unreachable;
        throw new DeviceUnreachableException();
      }
      this.ConnectionStatus = ConnectionStatus.Ok;
    }

    private void CheckWriteResult(GattCommunicationStatus communicationStatus)
    {
      if (communicationStatus == 1)
      {
        this.ConnectionStatus = ConnectionStatus.Unreachable;
        throw new DeviceUnreachableException();
      }
      this.ConnectionStatus = ConnectionStatus.Ok;
    }

    private void CheckConnectionStatus()
    {
      if (this.ConnectionStatus != ConnectionStatus.Ok && this.ConnectionStatus != ConnectionStatus.None)
        throw new DeviceNotConnectedException();
    }
  }
}
