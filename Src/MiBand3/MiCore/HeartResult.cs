using MiBand3;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage;
using Windows.Storage.Streams;

namespace MiBand3
{
    public class HeartResult : IDisposable, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public double HeartRate { get; set; }
        public DateTime LastCheckDate { get; set; }
        public string Title { get; set; }

        public static readonly Guid SERVICE = new Guid("0000180D-0000-1000-8000-00805F9B34FB".ToUpper());
        public static readonly Guid HEARTRATE_MEASUREMENT = new Guid("00002A37-0000-1000-8000-00805F9B34FB".ToUpper());
        public static readonly Guid HEARTRATE_CONTROL_POINT = new Guid("00002A39-0000-1000-8000-00805F9B34FB".ToUpper());

        private readonly byte[] COMMAND_START_HEART_RATE_MEASUREMENT = { 21, 2, 1 };
        private const byte COMMAND_SET_PERIODIC_HR_MEASUREMENT_INTERVAL = 14;

        private ApplicationDataContainer LocalSettings = ApplicationData.Current.LocalSettings;

        public HeartResult()
        {
        }

        public bool Initialize()
        {
            try
            {
                if (LocalSettings.Values["HeartResult"] != null)
                {
                    var data = (HeartResult)Helpers.FromXml(LocalSettings.Values["HeartResult"].ToString(), typeof(HeartResult));

                    HeartRate = data.HeartRate;
                    LastCheckDate = data.LastCheckDate;
                    Title = data.Title;

                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private byte GetHeartRateMeasurementInterval()
        {
            return 15;
        }

        public async Task<GattCommunicationStatus> SetHeartrateMeasurementInterval(GattCharacteristic characteristic)
        {
            try
            {
                var result = await characteristic.WriteValueWithResultAsync(new byte[] { COMMAND_SET_PERIODIC_HR_MEASUREMENT_INTERVAL, GetHeartRateMeasurementInterval() }.AsBuffer());
                return result.Status;
            }
            catch (Exception)
            {
                return GattCommunicationStatus.ProtocolError;
            }
        }

        public async Task<GattCommunicationStatus> GetHeartRateMeasurement(GattCharacteristic notifyCharacteristic, GattCharacteristic characteristic)
        {
            try
            {
                if (await notifyCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify) == GattCommunicationStatus.Success)
                {
                    notifyCharacteristic.ValueChanged += ValueChanged;
                    return await characteristic.WriteValueAsync(COMMAND_START_HEART_RATE_MEASUREMENT.AsBuffer());
                }

                return GattCommunicationStatus.ProtocolError;
            }
            catch (Exception)
            {
                return GattCommunicationStatus.ProtocolError;
            }
        }

        private void ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            if (sender.Uuid == HEARTRATE_MEASUREMENT)
            {
                GetHeartRate(args.CharacteristicValue.ToArray());
            }
        }

        private async void GetHeartRate(byte[] heartResult)
        {
            try
            {
                if (heartResult.Length >= 2)
                {
                    HeartRate = heartResult[1];
                }

                LastCheckDate = DateTime.Now;
                Title = $"Last checked at: {LastCheckDate:dddd, dd.MM.yyyy HH:mm}";

                LocalSettings.Values["HeartResult"] = Helpers.ToXml(this, typeof(HeartResult));

                await CoreApplication.Views.First().Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HeartRate)));
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR: {ex.Message}");
            }
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // Dispose managed resources here if needed
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}

