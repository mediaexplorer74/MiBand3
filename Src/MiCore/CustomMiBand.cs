using MiCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace MiCore
{
    public class CustomMiBand : IDisposable, INotifyPropertyChanged
    {
        public enum AuthorizationStatus
        {
            Failed = 0,
            Success = 1
        }

        private string _DeviceId;
        public string DeviceId
        {
            get => _DeviceId;
            set => _DeviceId = value;
        }

        private bool _Authorized;
        public bool Authorized
        {
            get => _Authorized;
            set => _Authorized = value;
        }

        private List<CustomMiBandResult> _DisplayItems;
        public List<CustomMiBandResult> DisplayItems
        {
            get => _DisplayItems;
            set => _DisplayItems = value;
        }

        private BatteryResult _BatteryResult;
        public BatteryResult BatteryResult
        {
            get => _BatteryResult;
            set => _BatteryResult = value;
        }

        private StepResult _StepResult;
        public StepResult StepResult
        {
            get => _StepResult;
            set => _StepResult = value;
        }

        private HeartResult _HeartResult;
        public HeartResult HeartResult
        {
            get => _HeartResult;
            set
            {
                if (_HeartResult != null)
                {
                    _HeartResult.PropertyChanged -= HeartResult_PropertyChanged;
                }
                _HeartResult = value;
                if (_HeartResult != null)
                {
                    _HeartResult.PropertyChanged += HeartResult_PropertyChanged;
                }
            }
        }

        private NotificationResult _NotificationResult;
        public NotificationResult NotificationResult
        {
            get => _NotificationResult;
            set => _NotificationResult = value;
        }

        private BluetoothLEDevice Device;
        private GattDeviceServicesResult GattServices;
        private GattCharacteristic GattCharacteristic;
        private string DeviceSelector = BluetoothLEDevice.GetDeviceSelector();
        private Windows.Storage.ApplicationDataContainer LocalSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        private bool disposedValue;
        private bool _updatemode = false;

        public delegate void AuthorizationCompletedEventHandler(object sender, AuthorizationStatus e);
        public event PropertyChangedEventHandler PropertyChanged;
        public event AuthorizationCompletedEventHandler InitializationCompleted;

        private EventWaitHandle WaitEvent = new EventWaitHandle(false, EventResetMode.ManualReset);

        public CustomMiBand()
        {
            _DeviceId = "";
            _DisplayItems = new List<CustomMiBandResult>();

            if (LocalSettings.Values["DeviceId"] != null)
            {
                _DeviceId = LocalSettings.Values["DeviceId"].ToString();
                _BatteryResult = new BatteryResult();
                _StepResult = new StepResult();
                _HeartResult = new HeartResult();
                _NotificationResult = new NotificationResult();

                _BatteryResult.Initialize();
                _DisplayItems.Add(new CustomMiBandResult(CustomMiBandResult.BandOperation.Battery, _BatteryResult.Percentage.ToString("N0"), Helpers.TimeSpanToText(_BatteryResult.LastChargingDate)));

                _StepResult.Initialize();
                _DisplayItems.Add(new CustomMiBandResult(CustomMiBandResult.BandOperation.Steps, _StepResult.TotalSteps.ToString(), _StepResult.StepsReachedInPercent));
                _DisplayItems.Add(new CustomMiBandResult(CustomMiBandResult.BandOperation.Distance, _StepResult.TotalDistance.ToString(), ""));
                _DisplayItems.Add(new CustomMiBandResult(CustomMiBandResult.BandOperation.Calories, _StepResult.TotalCals.ToString(), ""));

                NotificationResult.Initialize();
                _DisplayItems.Add(new CustomMiBandResult(CustomMiBandResult.BandOperation.Notifications, _NotificationResult.Requests.Count.ToString(), "Active"));
            }
        }

        public async Task<bool> ConnectWithAuth()
        {
            try
            {
                Device = await BluetoothLEDevice.FromIdAsync(_DeviceId);
                if (Device != null)
                {
                    if (Device.ConnectionStatus == BluetoothConnectionStatus.Disconnected)
                    {
                        GattServices = await Device.GetGattServicesAsync(BluetoothCacheMode.Uncached);
                        if (GattServices.Status == GattCommunicationStatus.Success)
                        {
                            return await AuthenticateAppOnDevice();
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        GattServices = await Device.GetGattServicesAsync(BluetoothCacheMode.Uncached);
                        return GattServices.Status == GattCommunicationStatus.Success;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR: {ex.Message}");
                return false;
            }
        }

        private async Task<bool> Connect()
        {
            try
            {
                Device = await BluetoothLEDevice.FromIdAsync(_DeviceId);
                if (Device != null)
                {
                    GattServices = await Device.GetGattServicesAsync(BluetoothCacheMode.Uncached);
                    return GattServices.Status == GattCommunicationStatus.Success;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR: {ex.Message}");
                return false;
            }
        }

        private void Disconnect()
        {
            GattCharacteristic?.Service?.Dispose();
            GattCharacteristic = null;
            GattServices = null;
            Device?.Dispose();
            Device = null;
        }

        public async Task<bool> AuthenticateAppOnDevice()
        {
            try
            {
                await Connect();

                Debug.WriteLine($"Request Handler for ValueChanged");
                GattCharacteristic = await GetCharacteristic(GetService(CustomBluetoothProfile.Authentication.Service), CustomBluetoothProfile.Authentication.AuthCharacteristic);

                if (await GattCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify) == GattCommunicationStatus.Success)
                {
                    Debug.WriteLine($"Requested Handler added");
                }
                else
                {
                    Debug.WriteLine($"Requested Handler not permitted");
                }

                if (Convert.ToBoolean(LocalSettings.Values["IsAuthenticationNeeded"]))
                {
                    var sendKey = new List<byte>
                {
                    CustomBluetoothProfile.Authentication.AuthSendKey,
                    CustomBluetoothProfile.Authentication.AuthByte
                };
                    sendKey.AddRange(GetSecretKey());

                    if (await GattCharacteristic.WriteValueAsync(sendKey.ToArray().AsBuffer()) == GattCommunicationStatus.Success)
                    {
                        Debug.WriteLine($"Authentication Level 1 successfully reached");
                    }
                    else
                    {
                        Debug.WriteLine($"Authentication Level 1 FAILED");
                        return false;
                    }
                }
                else
                {
                    Debug.WriteLine($"Authentication Level 1 successfully reached");

                    if (await GattCharacteristic.WriteValueAsync(RequestAuthNumber().AsBuffer()) == GattCommunicationStatus.Success)
                    {
                        Debug.WriteLine($"Authentication Level 2 successfully reached");
                    }
                    else
                    {
                        Debug.WriteLine($"Authentication Level 2 FAILED");
                        return false;
                    }
                }

                return true;//WaitEvent.WaitOne();
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                WaitEvent.Reset();
                Disconnect();
            }
        }

        private byte[] RequestAuthNumber()
        {
            return new byte[]
            {
            CustomBluetoothProfile.Authentication.AuthRequestRandomAuthNumber,
            CustomBluetoothProfile.Authentication.AuthByte
            };
        }

        private byte[] GetSecretKey()
        {
            return new byte[]
            {
            48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 64, 65, 66, 67, 68, 69
            };
        }

        private byte[] SendEncryptedRandomKey(byte[] responseValue)
        {
            var relevantResponsePart = responseValue.Skip(3).ToArray();
            var randomKey = new List<byte>
        {
            CustomBluetoothProfile.Authentication.AuthSendEncryptedAuthNumber,
            CustomBluetoothProfile.Authentication.AuthByte
        };
            randomKey.AddRange(EncryptToAES(relevantResponsePart));
            return randomKey.ToArray();
        }

        private byte[] EncryptToAES(byte[] toBeEncryptedBytes)
        {
            try
            {
                var aesKey = Convert.ToBase64String(GetSecretKey());
                var key = Convert.FromBase64String(aesKey).AsBuffer();
                var provider = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesCbc);
                var ckey = provider.CreateSymmetricKey(key);
                var bufferEncrypt = CryptographicEngine.Encrypt(ckey, toBeEncryptedBytes.AsBuffer(), null);
                return bufferEncrypt.ToArray();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<GattCharacteristic> GetCharacteristic(GattDeviceService service, Guid uuid)
        {
            if (service == null)
                return null;

            var characteristicsResult = await service.GetCharacteristicsAsync(BluetoothCacheMode.Uncached);
            return characteristicsResult.Characteristics.FirstOrDefault(x => x.Uuid == uuid);
        }

        public GattDeviceService GetService(Guid uuid)
        {
            if (GattServices == null)
            {
                return null;
            }
            return GattServices.Services.FirstOrDefault(x => x.Uuid == uuid);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    GattCharacteristic = null;
                    GattServices = null;
                    Device?.Dispose();
                    Device = null;
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public async Task<bool> setTimeFormatDisplay()
        {
            byte[] _bytes = null;
            try
            {
                Debug.WriteLine("BAND: SetTimeFormatDisplay");

                if (await Connect())
                {
                    var doChar = await GetCharacteristic(GetService(CustomBluetoothProfile.Basic.Service), CustomBluetoothProfile.Basic.ConfigurationCharacteristic);

                    if (Convert.ToBoolean(LocalSettings.Values["Is12hEnabled"]))
                    {
                        _bytes = CustomBluetoothProfile.Basic.DateFormatTime12Hours;
                    }
                    else
                    {
                        _bytes = CustomBluetoothProfile.Basic.DateFormatTime24Hours;
                    }

                    if (await doChar.WriteValueAsync(_bytes.AsBuffer()) == GattCommunicationStatus.Success)
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                _bytes = null;
            }
        }

        public async Task<bool> setGoalNotification()
        {
            byte[] _bytes = null;
            try
            {
                Debug.WriteLine("BAND: SetGoalNotification");

                if (await Connect())
                {
                    var doChar = await GetCharacteristic(GetService(CustomBluetoothProfile.Basic.Service), CustomBluetoothProfile.Basic.ConfigurationCharacteristic);

                    if (Convert.ToBoolean(LocalSettings.Values["IsGoalNotificationEnabled"]))
                    {
                        _bytes = CustomBluetoothProfile.Basic.CommandEnableGoalNotification;
                    }
                    else
                    {
                        _bytes = CustomBluetoothProfile.Basic.CommandDisableGoalNotification;
                    }

                    if (await doChar.WriteValueAsync(_bytes.AsBuffer()) == GattCommunicationStatus.Success)
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                _bytes = null;
            }
        }


        public async Task<bool> setRotateWristToSwitchInfo()
        {
            byte[] _bytes = null;
            try
            {
                Debug.WriteLine("BAND: SetRotateWristToSwitchInfo");

                if (await Connect())
                {
                    var doChar = await GetCharacteristic(GetService(CustomBluetoothProfile.Basic.Service), CustomBluetoothProfile.Basic.ConfigurationCharacteristic);

                    if (Convert.ToBoolean(LocalSettings.Values["IsRotateWristToSwitchInfoEnabled"]))
                    {
                        _bytes = CustomBluetoothProfile.Basic.CommandEnableRotateWristToSwitchInfo;
                    }
                    else
                    {
                        _bytes = CustomBluetoothProfile.Basic.CommandDisableRotateWristToSwitchInfo;
                    }

                    if (await doChar.WriteValueAsync(_bytes.AsBuffer()) == GattCommunicationStatus.Success)
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                _bytes = null;
            }
        }


        public async Task<bool> setDoNotDisturb()
        {
            byte[] _bytes = null;
            try
            {
                Debug.WriteLine("BAND: SetDoNotDisturb");

                if (await Connect())
                {
                    var doChar = await GetCharacteristic(GetService(CustomBluetoothProfile.Basic.Service), CustomBluetoothProfile.Basic.ConfigurationCharacteristic);

                    if (Convert.ToBoolean(LocalSettings.Values["IsDndEnabled"]))
                    {
                        _bytes = CustomBluetoothProfile.Basic.CommandDoNotDisturbAutomatic;
                    }
                    else
                    {
                        _bytes = CustomBluetoothProfile.Basic.CommandDoNotDisturbOff;
                    }

                    if (await doChar.WriteValueAsync(_bytes.AsBuffer()) == GattCommunicationStatus.Success)
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                _bytes = null;
            }
        }


        public async Task<bool> setDateDisplay()
        {
            byte[] _bytes = null;
            try
            {
                Debug.WriteLine("BAND: SetDateDisplay");

                if (await Connect())
                {
                    var doChar = await GetCharacteristic(GetService(CustomBluetoothProfile.Basic.Service), CustomBluetoothProfile.Basic.ConfigurationCharacteristic);

                    if (Convert.ToBoolean(LocalSettings.Values["IsDateEnabled"]))
                    {
                        _bytes = CustomBluetoothProfile.Basic.DateFormatDateTime;
                    }
                    else
                    {
                        _bytes = CustomBluetoothProfile.Basic.DateFormatTime;
                    }

                    if (await doChar.WriteValueAsync(_bytes.AsBuffer()) == GattCommunicationStatus.Success)
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                _bytes = null;
            }
        }


        public async Task<bool> SetAlertLevel(byte level)
        {
            try
            {
                if (await ConnectWithAuth())
                {
                    var bytes = new byte[] { level };
                    var doChar = await GetCharacteristic(GetService(CustomBluetoothProfile.AlertLevel.Service), CustomBluetoothProfile.AlertLevel.AlertLevelCharacteristic);
                    await doChar.WriteValueAsync(bytes.AsBuffer());
                }
            }
            catch (Exception)
            {
                // Handle exception if needed
            }
            finally
            {
                Disconnect();
            }

            return true;
        }


        public async Task<bool> setWearLocation()
        {
            byte[] _bytes = null;
            try
            {
                Debug.WriteLine("BAND: SetWearLocation");

                if (await Connect())
                {
                    var doChar = await GetCharacteristic(GetService(CustomBluetoothProfile.Basic.Service), CustomBluetoothProfile.Basic.UserSettingsCharacteristic);

                    if (Convert.ToBoolean(LocalSettings.Values["IsWearLocationRightEnabled"]))
                    {
                        _bytes = CustomBluetoothProfile.Basic.WearLocationRightWrist;
                    }
                    else
                    {
                        _bytes = CustomBluetoothProfile.Basic.WearLocationLeftWrist;
                    }

                    if (await doChar.WriteValueAsync(_bytes.AsBuffer()) == GattCommunicationStatus.Success)
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                _bytes = null;
            }
        }

        public async Task<bool> SetActivateDisplayOnLiftWrist()
        {
            byte[] _bytes = null;
            try
            {
                Debug.WriteLine("BAND: SetActivateDisplayOnLiftWrist");

                if (await Connect())
                {
                    var doChar = await GetCharacteristic(GetService(CustomBluetoothProfile.Basic.Service), CustomBluetoothProfile.Basic.ConfigurationCharacteristic);

                    if (Convert.ToBoolean(LocalSettings.Values["IsDisplayOnLiftWristEnabled"]))
                    {
                        _bytes = CustomBluetoothProfile.Basic.CommandEnableDisplayOnLiftWrist;
                    }
                    else
                    {
                        _bytes = CustomBluetoothProfile.Basic.CommandDisableDisplayOnLiftWrist;
                    }

                    if (await doChar.WriteValueAsync(_bytes.AsBuffer()) == GattCommunicationStatus.Success)
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                _bytes = null;
            }
        }


        public async Task<bool> UpdateOperations()
        {
            try
            {
                if (await ConnectWithAuth())
                {
                    _DisplayItems.Clear();

                    GattCharacteristic stepsCharacteristic = await GetCharacteristic(
                        GetService(StepResult.SERVICE), 
                        StepResult.REALTIME_STEPS);

                    if (await _StepResult.GetSteps(stepsCharacteristic) == GattCommunicationStatus.Success)
                    {
                        _DisplayItems.Add(new CustomMiBandResult(CustomMiBandResult.BandOperation.Steps, 
                            _StepResult.TotalSteps.ToString(), _StepResult.StepsReachedInPercent));
                        _DisplayItems.Add(new CustomMiBandResult(CustomMiBandResult.BandOperation.Distance, 
                            _StepResult.TotalDistance.ToString(), ""));
                        _DisplayItems.Add(new CustomMiBandResult(CustomMiBandResult.BandOperation.Calories, 
                            _StepResult.TotalCals.ToString(), ""));
                    }

                    GattCharacteristic batteryCharacteristic = await GetCharacteristic(
                        GetService(BatteryResult.SERVICE), 
                        BatteryResult.BATTERY_INFO);

                    if (await _BatteryResult.GetBatteryInfo(batteryCharacteristic) == GattCommunicationStatus.Success)
                    {
                        _DisplayItems.Add(new CustomMiBandResult(CustomMiBandResult.BandOperation.Battery, _BatteryResult.Percentage.ToString("N0"), Helpers.TimeSpanToText(_BatteryResult.LastChargingDate)));
                    }

                    // Uncomment if HeartRate functionality is implemented
                    // _DisplayItems.Add(new CustomMiBandResult(CustomMiBandResult.BandOperation.Heartrate, _HeartResult.HeartRate.ToString("N0"), HeartResult.Title));

                    NotificationResult.Initialize();
                    _DisplayItems.Add(new CustomMiBandResult(CustomMiBandResult.BandOperation.Notifications, _NotificationResult.Requests.Count.ToString(), "Active"));

                    OnPropertyChanged(nameof(DisplayItems));

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                Disconnect();
            }
        }

        private bool IsOperationEnabled(CustomMiBandResult.BandOperation operation)
        {
            var settingKey = $"Setting_{(int)operation}";
            if (LocalSettings.Values.ContainsKey(settingKey))
            {
                return Convert.ToBoolean(LocalSettings.Values[settingKey]);
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> setDisplayItems()
        {
            List<byte> _bytes = new List<byte>();
            try
            {
                Debug.WriteLine("BAND: SetDisplayItems");

                if (await Connect())
                {
                    var doChar = await GetCharacteristic(GetService(CustomBluetoothProfile.Basic.Service), CustomBluetoothProfile.Basic.ConfigurationCharacteristic);

                    _bytes.AddRange(CustomBluetoothProfile.Basic.CommandChangeScreens);

                    if (Convert.ToBoolean(LocalSettings.Values[$"Setting_{(int)CustomMiBandResult.BandOperation.Steps}"]))
                    {
                        _bytes[CustomBluetoothProfile.Basic.ScreenChangeByte] |= CustomBluetoothProfile.Basic.DisplayItemBitSteps;
                    }
                    if (Convert.ToBoolean(LocalSettings.Values[$"Setting_{(int)CustomMiBandResult.BandOperation.Distance}"]))
                    {
                        _bytes[CustomBluetoothProfile.Basic.ScreenChangeByte] |= CustomBluetoothProfile.Basic.DisplayItemBitDistance;
                    }
                    if (Convert.ToBoolean(LocalSettings.Values[$"Setting_{(int)CustomMiBandResult.BandOperation.Calories}"]))
                    {
                        _bytes[CustomBluetoothProfile.Basic.ScreenChangeByte] |= CustomBluetoothProfile.Basic.DisplayItemBitCalories;
                    }
                    if (Convert.ToBoolean(LocalSettings.Values[$"Setting_{(int)CustomMiBandResult.BandOperation.Heartrate}"]))
                    {
                        _bytes[CustomBluetoothProfile.Basic.ScreenChangeByte] |= CustomBluetoothProfile.Basic.DisplayItemBitHeartRate;
                    }
                    if (Convert.ToBoolean(LocalSettings.Values[$"Setting_{(int)CustomMiBandResult.BandOperation.Battery}"]))
                    {
                        _bytes[CustomBluetoothProfile.Basic.ScreenChangeByte] |= CustomBluetoothProfile.Basic.DisplayItemBitBattery;
                    }

                    if (await doChar.WriteValueAsync(_bytes.ToArray().AsBuffer()) == GattCommunicationStatus.Success)
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                _bytes = null;
            }
        }


        public async Task<bool> setUserInfo(string alias, DateTime dateOfBirth, string gender, int height, int weight)
        {
            List<byte> _bytes = new List<byte>();
            try
            {
                Debug.WriteLine("BAND: SetUserInfo");

                if (await Connect())
                {
                    int userId = 4711; // Originally an account number
                    int genderValue = gender == "Male" ? 0 : 1;

                    var doChar = await GetCharacteristic(GetService(CustomBluetoothProfile.Basic.Service), CustomBluetoothProfile.Basic.UserSettingsCharacteristic);

                    _bytes.Add(CustomBluetoothProfile.Basic.CommandSetUserInfo);
                    _bytes.Add(0);
                    _bytes.Add(0);
                    _bytes.Add((byte)(dateOfBirth.Year & 255)); // Year (low byte)
                    _bytes.Add((byte)((dateOfBirth.Year >> 8) & 255)); // Year (high byte)
                    _bytes.Add((byte)dateOfBirth.Month); // Month
                    _bytes.Add((byte)dateOfBirth.Day); // Day
                    _bytes.Add((byte)genderValue); // Gender (2 = Others, 0 = Male, 1 = Female)
                    _bytes.Add((byte)(height & 255)); // Height (low byte)
                    _bytes.Add((byte)((height >> 8) & 255)); // Height (high byte)
                    _bytes.Add((byte)((weight * 200) & 255)); // Weight (low byte)
                    _bytes.Add((byte)(((weight * 200) >> 8) & 255)); // Weight (high byte)
                    _bytes.Add((byte)(userId & 255)); // User ID (low byte)
                    _bytes.Add((byte)((userId >> 8) & 255)); // User ID (2nd byte)
                    _bytes.Add((byte)((userId >> 16) & 255)); // User ID (3rd byte)
                    _bytes.Add((byte)((userId >> 24) & 255)); // User ID (high byte)

                    if (await doChar.WriteValueAsync(_bytes.ToArray().AsBuffer()) == GattCommunicationStatus.Success)
                    {
                        // Success
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                _bytes = null;
            }
        }



        public async Task<ClockResult> GetClockStatus()
        {
            try
            {
                var doChar = await GetCharacteristic(GetService(CustomBluetoothProfile.Basic.Service), CustomBluetoothProfile.Basic.TimeCharacteristic);
                var resultRead = await doChar.ReadValueAsync(BluetoothCacheMode.Uncached);

                if (resultRead.Status == GattCommunicationStatus.Success)
                {
                    var resultBytes = resultRead.Value.ToArray();

                    // 1 = 07 = 201 + 07 = 2017 (year)
                    // 2 = month
                    // 3 = day
                    // 4 = hour
                    // 5 = minutes
                    // 6 = seconds
                    var clockDate = new DateTime(
                        year: 2010 + resultBytes[1],
                        month: resultBytes[2],
                        day: resultBytes[3],
                        hour: resultBytes[4],
                        minute: resultBytes[5],
                        second: resultBytes[6]
                    );

                    return new ClockResult(clockDate);
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> GetDeviceName()
        {
            var doChar = await GetCharacteristic(GetService(CustomBluetoothProfile.Generic.Service), CustomBluetoothProfile.Generic.DeviceNameCharacteristic);
            var resultRead = await doChar.ReadValueAsync();

            if (resultRead.Status == GattCommunicationStatus.Success)
            {
                LocalSettings.Values["DeviceName"] = Encoding.UTF8.GetString(resultRead.Value.ToArray());
            }

            return true;
        }


        public async Task<bool> GetSoftwareRevision()
        {
            var doChar = await GetCharacteristic(GetService(CustomBluetoothProfile.Information.Service), CustomBluetoothProfile.Information.SoftwareCharacteristic);
            var resultRead = await doChar.ReadValueAsync();

            if (resultRead.Status == GattCommunicationStatus.Success)
            {
                LocalSettings.Values["SoftwareRevision"] = Encoding.UTF8.GetString(resultRead.Value.ToArray());
            }

            return true;
        }



        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void HeartResult_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                var heartRateItem = _DisplayItems.FirstOrDefault(
                    x => x.Operation == (int)CustomMiBandResult.BandOperation.Heartrate);
                if (heartRateItem != null)
                {
                    heartRateItem.Value = ((HeartResult)sender).HeartRate.ToString("N0");
                    heartRateItem.OnPropertyChanged("HeartRate");
                    heartRateItem.OnPropertyChanged("Title");
                }
            }
            catch (Exception)
            {
            }
        }//HeartResult_PropertyChanged


    }
}
