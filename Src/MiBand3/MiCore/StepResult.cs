using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage;

namespace MiBand3
{
    public class StepResult : IDisposable
    {
        private int _totalSteps;
        public int TotalSteps
        {
            get { return _totalSteps; }
            set { _totalSteps = value; }
        }

        private List<HistoryValues> _history;
        public List<HistoryValues> History
        {
            get { return _history; }
            set { _history = value; }
        }

        private string _stepsReachedInPercent;
        public string StepsReachedInPercent
        {
            get { return _stepsReachedInPercent; }
            set { _stepsReachedInPercent = value; }
        }

        private double _totalDistance;
        public double TotalDistance
        {
            get { return _totalDistance; }
            set { _totalDistance = value; }
        }

        private double _totalCals;
        public double TotalCals
        {
            get { return _totalCals; }
            set { _totalCals = value; }
        }

        public static readonly Guid REALTIME_STEPS = new Guid("00000007-0000-3512-2118-0009af100700".ToUpper());
        public static readonly Guid SERVICE = new Guid("0000fee0-0000-1000-8000-00805f9b34fb".ToUpper());

        private ApplicationDataContainer LocalSettings = ApplicationData.Current.LocalSettings;

        public StepResult()
        {
        }

        public bool Initialize()
        {
            StepResult data = null;
            try
            {
                if (LocalSettings.Values["StepResult"] != null)
                {
                    data = (StepResult)Helpers.FromXml(LocalSettings.Values["StepResult"].ToString(), typeof(StepResult));

                    _totalSteps = data.TotalSteps;
                    _totalDistance = data.TotalDistance;
                    _totalCals = data.TotalCals;
                    _history = data.History;
                    _stepsReachedInPercent = data.StepsReachedInPercent;

                    return true;
                }
                else
                {
                    _history = new List<HistoryValues>();
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                data = null;
            }
        }

        private void AddToHistory(HistoryValues.Types type, double value)
        {
            HistoryValues history = null;
            try
            {
                history = _history.FirstOrDefault(x => x.Type == (int)type && x.Moment.Date == DateTime.Now.Date);
                if (history != null)
                {
                    history.Value = value;
                }
                else
                {
                    _history.Add(new HistoryValues(type, DateTime.Now, value));
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                history = null;
            }
        }

        public async Task<GattCommunicationStatus> GetSteps(GattCharacteristic mCharacteristic)
        {
            try
            {
                var result = await mCharacteristic.ReadValueAsync(BluetoothCacheMode.Uncached);
                if (result.Status == GattCommunicationStatus.Success)
                {
                    Debug.WriteLine("READ: STEPS, DISTANCE, CALORIES");

                    var resultBytes = result.Value.ToArray();

                    // Daily total steps
                    _totalSteps = (resultBytes[1] & 255) | ((resultBytes[2] & 255) << 8);
                    AddToHistory(HistoryValues.Types.Steps, _totalSteps);

                    _stepsReachedInPercent = $"{(_totalSteps * 100 / (double)LocalSettings.Values["Profile_Steps"]):N0}% Complete";

                    // Daily total distance
                    _totalDistance = (resultBytes[5] & 255) | ((resultBytes[6] & 255) << 8) | (resultBytes[7] & 255) | ((resultBytes[8] & 255) << 24);
                    AddToHistory(HistoryValues.Types.Distances, _totalDistance);

                    // Get calories
                    _totalCals = (resultBytes[9] & 255) | ((resultBytes[10] & 255) << 8) | (resultBytes[11] & 255) | ((resultBytes[12] & 255) << 24);
                    AddToHistory(HistoryValues.Types.Calories, _totalCals);

                    LocalSettings.Values["StepResult"] = Helpers.ToXml(this, typeof(StepResult));

                    return result.Status;
                }
                else
                {
                    return result.Status;
                }
            }
            catch (Exception)
            {
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

    public class HistoryValues
    {
        public enum Types
        {
            Steps = 0,
            Distances = 1,
            Calories = 2
        }

        private int _type;
        public int Type
        {
            get { return _type; }
            set { _type = value; }
        }

        private DateTime _moment;
        public DateTime Moment
        {
            get { return _moment; }
            set { _moment = value; }
        }

        private double _value;
        public double Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public HistoryValues()
        {
        }

        public HistoryValues(Types type, DateTime moment, double value)
        {
            _type = (int)type;
            _moment = moment;
            _value = value;
        }
    }
}
