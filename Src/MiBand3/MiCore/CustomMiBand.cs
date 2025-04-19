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
using Windows.Storage;
using Windows.Storage.Streams;

namespace MiBand3
{
    public class CustomMiBand : IDisposable, INotifyPropertyChanged
    {
        public enum AuthorizationStatus
        {
            Failed = 0,
            Success = 1
        }

        public string DeviceId { get; set; }
        public bool Authorized { get; set; }
        public List<CustomMiBandResult> DisplayItems { get; set; }
        public BatteryResult BatteryResult { get; set; }
        public StepResult StepResult { get; set; }
        public HeartResult HeartResult { get; set; }
        public NotificationResult NotificationResult { get; set; }

        private BluetoothLEDevice Device;
        private GattDeviceServicesResult GattServices;
        private GattCharacteristic GattCharacteristic;
        private string DeviceSelector = BluetoothLEDevice.GetDeviceSelector();
        private ApplicationDataContainer LocalSettings = ApplicationData.Current.LocalSettings;
        private bool disposedValue = false;
        private bool _updatemode = false;

        public delegate void AuthorizationCompletedEventHandler(object sender, AuthorizationStatus e);
        public event PropertyChangedEventHandler PropertyChanged;
        public event AuthorizationCompletedEventHandler InitializationCompleted;

        private EventWaitHandle WaitEvent = new EventWaitHandle(false, EventResetMode.ManualReset);

        public CustomMiBand()
        {
            DeviceId = "";
            DisplayItems = new List<CustomMiBandResult>();

            if (LocalSettings.Values["DeviceId"] != null)
            {
                DeviceId = LocalSettings.Values["DeviceId"].ToString();
                BatteryResult = new BatteryResult();
                StepResult = new StepResult();
                HeartResult = new HeartResult();
                NotificationResult = new NotificationResult();

                BatteryResult.Initialize();
                DisplayItems.Add(new CustomMiBandResult(CustomMiBandResult.BandOperation.Battery, BatteryResult.Percentage.ToString("N0"), Helpers.TimeSpanToText(BatteryResult.LastChargingDate)));

                StepResult.Initialize();
                DisplayItems.Add(new CustomMiBandResult(CustomMiBandResult.BandOperation.Steps, StepResult.TotalSteps.ToString(), StepResult.StepsReachedInPercent));
                DisplayItems.Add(new CustomMiBandResult(CustomMiBandResult.BandOperation.Distance, StepResult.TotalDistance.ToString(), ""));
                DisplayItems.Add(new CustomMiBandResult(CustomMiBandResult.BandOperation.Calories, StepResult.TotalCals.ToString(), ""));

                NotificationResult.Initialize();
                DisplayItems.Add(new CustomMiBandResult(CustomMiBandResult.BandOperation.Notifications, NotificationResult.Requests.Count.ToString(), "Active"));
            }
        }

        public async Task<bool> ConnectWithAuth()
        {
            try
            {
                Device = await BluetoothLEDevice.FromIdAsync(DeviceId);
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
                Device = await BluetoothLEDevice.FromIdAsync(DeviceId);
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

                // DEBUG
                if (GattCharacteristic == null)
                    return true;//default;

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

                return WaitEvent.WaitOne();
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
            //DEBUG
            if (service == null)
                return null;

            GattCharacteristicsResult characteristicsResult = 
                await service.GetCharacteristicsAsync(BluetoothCacheMode.Uncached);
            
            return characteristicsResult.Characteristics.FirstOrDefault(x => x.Uuid == uuid);
        }

        public GattDeviceService GetService(Guid uuid)
        {
            //DEBUG
            Debug.WriteLine($"GetService: {uuid}");

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

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        internal async Task SetAlertLevel(int v)
        {
            //TODO
        }

        public async Task UpdateOperations()
        {
            //TODO
        }

        internal async Task setWearLocation()
        {
            //
        }

        internal async Task setTimeFormatDisplay()
        {
            throw new NotImplementedException();
        }

        internal async Task setUserInfo(string v1, DateTime date, string v2, int v3, int v4)
        {
            throw new NotImplementedException();
        }

        internal void setRotateWristToSwitchInfo()
        {
            throw new NotImplementedException();
        }

        internal void setGoalNotification()
        {
            throw new NotImplementedException();
        }

        internal void setDoNotDisturb()
        {
            throw new NotImplementedException();
        }

        internal async Task setDisplayItems()
        {
            throw new NotImplementedException();
        }

        internal async Task setDateDisplay()
        {
            throw new NotImplementedException();
        }

        internal async Task setActivateDisplayOnLiftWrist()
        {
            throw new NotImplementedException();
        }
    }
}
