using System;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage;

namespace MiCore
{
    public class BatteryResult : IDisposable
    {
        private double _percentage;
        public double Percentage
        {
            get { return _percentage; }
            set { _percentage = value; }
        }

        private bool _charging;
        public bool Charging
        {
            get { return _charging; }
            set { _charging = value; }
        }

        private int _cycles;
        public int Cycles
        {
            get { return _cycles; }
            set { _cycles = value; }
        }

        private DateTime _lastChargingDate;
        public DateTime LastChargingDate
        {
            get { return _lastChargingDate; }
            set { _lastChargingDate = value; }
        }

        public static readonly Guid SERVICE = new Guid("0000fee0-0000-1000-8000-00805f9b34fb".ToUpper());
        public static readonly Guid BATTERY_INFO = new Guid("00000006-0000-3512-2118-0009af100700".ToUpper());

        private const int DEVICE_BATTERY_NORMAL = 0;
        private const int DEVICE_BATTERY_CHARGING = 1;

        private ApplicationDataContainer LocalSettings = ApplicationData.Current.LocalSettings;

        public BatteryResult() { }

        public bool Initialize()
        {
            BatteryResult _data = null;
            try
            {
                if (LocalSettings.Values["BatteryResult"] != null)
                {
                    _data = (BatteryResult)Helpers.FromXml(LocalSettings.Values["BatteryResult"].ToString(), typeof(BatteryResult));

                    _percentage = _data.Percentage;
                    _charging = _data.Charging;
                    _cycles = _data.Cycles;
                    _lastChargingDate = _data.LastChargingDate;

                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                _data = null;
            }
        }

        public async Task<GattCommunicationStatus> GetBatteryInfo(GattCharacteristic mCharacteristic)
        {
            try
            {
                GattReadResult batteryResult = default;

                try
                {
                    if (mCharacteristic != null)
                      batteryResult = await mCharacteristic.ReadValueAsync(BluetoothCacheMode.Uncached);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("[ex] Get batteryResult error: " + ex.Message);
                }

                if (batteryResult == null)
                    return default;

                if (batteryResult.Status == GattCommunicationStatus.Success)
                {
                    Debug.WriteLine("READ: BATTERY INFO");

                    var batteryResultBytes = batteryResult.Value.ToArray();

                    // resultBytes[1] = % charged
                    // resultBytes[2] = state: normal or charging
                    // mData[12] = last charged year + 201
                    // mData[13] = last charged month
                    // mData[14] = last charged day
                    // mData[15] = last charged hour
                    // mData[16] = last charged minute
                    // mData[17] = last charged second
                    // mData[18] = cycles

                    _charging = batteryResultBytes[2] == DEVICE_BATTERY_CHARGING;

                    _percentage = batteryResultBytes[1];
                    _cycles = batteryResultBytes[18];
                    _lastChargingDate = DateTime.Parse(
                        $"{batteryResultBytes[13]}.{batteryResultBytes[14]}.201{batteryResultBytes[12]} {batteryResultBytes[15]}:{batteryResultBytes[16]}:{batteryResultBytes[17]}");

                    LocalSettings.Values["BatteryResult"] = Helpers.ToXml(this, typeof(BatteryResult));

                    return batteryResult.Status;
                }
                else
                {
                    return batteryResult.Status;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR: {ex.Message}");
                return GattCommunicationStatus.ProtocolError;
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
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}