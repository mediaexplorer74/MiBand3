// Decompiled with JetBrains decompiler
// Type: MiBand.SDK.HeartRate.HeartRateLoader
// Assembly: MiBand.SDK, Version=0.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 152FA565-3249-4054-A361-01D9C0AFA7F1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBand.SDK.dll

using MiBand.SDK.Bluetooth;
using MiBand.SDK.Data;
using MiBand.SDK.Tools;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Foundation;
using Windows.Storage.Streams;

#nullable disable
namespace MiBand.SDK.HeartRate
{
  internal class HeartRateLoader
  {
    private readonly GattDeviceService _hrService;
    private readonly ILog _log;
    private readonly GattCharacteristic _hrCharacteristics;
    private readonly GattCharacteristic _hrControlCharacteristic;
    private volatile TaskCompletionSource<HeartRateMeasurement> _gettingDataTaskCompletionSource;

    public HeartRateLoader(ILog log, BluetoothDeviceBase bluetoothDevice)
    {
      this._log = log;
      this._hrService = bluetoothDevice.GetSecondaryService(GattServiceUuids.HeartRate);
      if (this._hrService == null)
      {
        this._log.Debug("Heart rate service not found in device");
      }
      else
      {
        this._hrCharacteristics = this._hrService.GetCharacteristics(GattCharacteristicUuids.HeartRateMeasurement)[0];
        this._hrControlCharacteristic = this._hrService.GetCharacteristics(GattCharacteristicUuids.HeartRateControlPoint)[0];
      }
    }

    public async Task<HeartRateMeasurement> GetHeartRate()
    {
      if (this._hrService == null)
        return (HeartRateMeasurement) null;
      object obj = (object) null;
      int num = 0;
      HeartRateMeasurement heartRate = default;
      try
      {
        this._gettingDataTaskCompletionSource = new TaskCompletionSource<HeartRateMeasurement>();
        ConfiguredTaskAwaitable configuredTaskAwaitable = 
                    this.ConfigureServiceForNotificationsAsync().ConfigureAwait(false);

        await configuredTaskAwaitable;
        configuredTaskAwaitable = this.SendMeasurementStartCommand().ConfigureAwait(false);
        await configuredTaskAwaitable;
        Task<HeartRateMeasurement> hrTask = this._gettingDataTaskCompletionSource.Task;
        if (await Task.WhenAny((Task) hrTask, Task.Delay(TimeSpan.FromSeconds(35.0))).ConfigureAwait(false) == hrTask)
        {
          configuredTaskAwaitable = this.SendMeasurementStopCommand().ConfigureAwait(false);
          await configuredTaskAwaitable;
          heartRate = hrTask.Result;
        }
        else
        {
          this._log.Info("Heart rate measurement timed out.");
          heartRate = (HeartRateMeasurement) null;
        }
        num = 1;
      }
      catch (Exception ex)
      {
        obj = ex;
      }
      await this.UnsubscribeFromNotifies().ConfigureAwait(false);
      object obj1 = obj;
      if (obj1 != null)
      {
        if (!(obj1 is Exception source))
         throw (Exception)obj1;
        
        ExceptionDispatchInfo.Capture(source).Throw();
      }
      if (num == 1)
        return heartRate;
      obj = (object) null;
      heartRate = (HeartRateMeasurement) default;
      HeartRateMeasurement heartRate1 = heartRate;
      return heartRate1;
    }

    private async Task SendMeasurementStartCommand()
    {
      GattCommunicationStatus communicationStatus = await this._hrControlCharacteristic.WriteValueAsync(new byte[3]
      {
        (byte) 21,
        (byte) 2,
        (byte) 1
      }.AsBuffer());
    }

    private async Task SendMeasurementStopCommand()
    {
      GattCommunicationStatus communicationStatus = await this._hrControlCharacteristic.WriteValueAsync(new byte[3]
      {
        (byte) 21,
        (byte) 2,
        (byte) 0
      }.AsBuffer());
    }

        private async Task ConfigureServiceForNotificationsAsync()
        {
            try
            {
                GattCharacteristic hrCharacteristics = this._hrCharacteristics;

                // Use lambda expressions to add and remove event handlers
                hrCharacteristics.ValueChanged += (sender, args) => HeartRateOnValueChanged(sender, args);

                GattCommunicationStatus communicationStatus = await this._hrCharacteristics
                    .WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
            }
            catch (Exception ex)
            {
                this._log.Warning("Exception when trying to configure service for notifications on: " + ex);
            }
        }

        private async Task UnsubscribeFromNotifies()
        {
            try
            {
                GattCommunicationStatus communicationStatus = await this._hrCharacteristics
                    .WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.None);

                // Corrected the variable name to use the existing field '_hrCharacteristics'
                this._hrCharacteristics.ValueChanged -= HeartRateOnValueChanged;
            }
            catch (Exception ex)
            {
                this._log.Warning("Exception when trying to configure service for notifications off: " + ex);
            }
        }

    private void HeartRateOnValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
    {
      try
      {
        byte[] data = new byte[(int) args.CharacteristicValue.Length];
        DataReader.FromBuffer(args.CharacteristicValue).ReadBytes(data);
        HeartRateMeasurement result = this.ProcessData(data);
        result.Timestamp = (DateTimeOffset) args.Timestamp.DateTime;
        this._gettingDataTaskCompletionSource?.SetResult(result);
      }
      catch (Exception ex)
      {
        this._log.Error("Error when processing HeartRate value change: " + (object) ex);
      }
    }

    private HeartRateMeasurement ProcessData(byte[] data)
    {
      this._log.Debug("HR data: " + string.Join<byte>(" ", (IEnumerable<byte>) data));
      byte index1 = 0;
      int num1 = ((uint) data[(int) index1] & 1U) > 0U ? 1 : 0;
      byte index2 = (byte) ((uint) index1 + 1U);
      ushort num2 = num1 == 0 ? (ushort) data[(int) index2] : (ushort) (((uint) data[(int) index2 + 1] << 8) + (uint) data[(int) index2]);
      return new HeartRateMeasurement()
      {
        HeartRateValue = num2
      };
    }
  }
}
