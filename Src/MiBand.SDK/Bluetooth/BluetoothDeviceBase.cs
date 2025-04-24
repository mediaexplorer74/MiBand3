
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
  internal abstract class BluetoothDeviceBase : IBluetoothDevice//, IDisposable
  {
    private ConnectionStatus _connectionStatus;
    private readonly List<GattCharacteristic> _notificationEnabled = new List<GattCharacteristic>();
    private readonly Dictionary<Guid, GattDeviceService> _secondaryServices = new Dictionary<Guid, GattDeviceService>();

    protected BluetoothDeviceBase(GattDeviceService defaultGattService)
    {
      this.DefaultGattService = defaultGattService;
    }

    //public event EventHandler<EventArgs> ConnectionStatusChanged;
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
      BluetoothCacheMode mode = BluetoothCacheMode.Uncached)
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

        // Fix for CS0518, CS1001, CS1002, CS1056, CS0246, CS0103

        // The issue seems to be related to the decompiled code and the use of a compiler-generated class `c__DisplayClass20_0`.
        // This class is not defined in the provided code, and the decompiler has introduced invalid syntax.
        // To fix this, we need to replace the problematic code with a proper implementation.

        internal async Task<NotifyResponse> WriteCharacteristicGetResponse(
            GattCharacteristic characteristic,
            byte[] data,
            Task delayTask,
            int payloadOffset = 0)
        {
            if (delayTask == null)
                throw new ArgumentNullException(nameof(delayTask));

            var notifyReceivedTcs = new TaskCompletionSource<NotifyResponse>();
            TypedEventHandler<GattCharacteristic, GattValueChangedEventArgs> valueChangedDelegate 
                = (sender, args) =>
            {
                try
                {
                    if (args.CharacteristicValue == null)
                    {
                        notifyReceivedTcs.TrySetResult(null);
                    }
                    else
                    {
                        NotifyResponse result = new NotifyResponse(args.CharacteristicValue.ToArraySafe(), payloadOffset);
                        notifyReceivedTcs.TrySetResult(result);
                    }
                }
                catch
                {
                    notifyReceivedTcs.TrySetResult(null);
                }
            };

            characteristic.ValueChanged += valueChangedDelegate;

            NotifyResponse notifyResponse = null;
            try
            {
                await EnableNotifications(characteristic).ConfigureAwait(false);
                await WriteCharacteristic(characteristic, data, characteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.WriteWithoutResponse)).ConfigureAwait(false);

                if (await Task.WhenAny(notifyReceivedTcs.Task, delayTask).ConfigureAwait(false) == notifyReceivedTcs.Task)
                {
                    notifyResponse = notifyReceivedTcs.Task.Result;
                }
            }
            finally
            {
                await DisableNotifications(characteristic).ConfigureAwait(false);
                characteristic.ValueChanged -= valueChangedDelegate;
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
      if (readResult.Status == GattCommunicationStatus.Unreachable)
      {
        this.ConnectionStatus = ConnectionStatus.Unreachable;
        throw new DeviceUnreachableException();
      }
      this.ConnectionStatus = ConnectionStatus.Ok;
    }

    private void CheckWriteResult(GattCommunicationStatus communicationStatus)
    {
      if (communicationStatus == GattCommunicationStatus.Unreachable)
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
