using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;

namespace MiBand3
{
    public class CustomBand
    {
        public string DeviceId { get; set; }
        public bool IsAuthNeeded { get; set; }

        private BluetoothLEDevice _device;
        private List<CustomGattService> _gattServices;

        public CustomBand()
        {
            // ToDo: Load from Settings
            DeviceId = null;
            IsAuthNeeded = true;
            _gattServices = new List<CustomGattService>();
        }

        public async Task<bool> GetDeviceByNameAsync()
        {
            DeviceInformation deviceInformation = null;
            string deviceSelector = null;
            try
            {
                DeviceId = null;
                deviceSelector = BluetoothLEDevice.GetDeviceSelectorFromPairingState(true);

                foreach (var deviceInfo in await DeviceInformation.FindAllAsync(deviceSelector))
                {
                    if (deviceInfo.Name.ToUpper() == "MI BAND 2")
                    {
                        DeviceId = deviceInfo.Id;
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
                deviceInformation = null;
                deviceSelector = null;
            }
        }

        public async Task<bool> Connect()
        {
            GattDeviceServicesResult gattDeviceServicesResult = null;
            CustomGattService customGattService = null;
            try
            {
                _device = await BluetoothLEDevice.FromIdAsync(DeviceId);
                if (_device != null)
                {
                    gattDeviceServicesResult = await _device.GetGattServicesAsync(BluetoothCacheMode.Uncached);
                    if (gattDeviceServicesResult.Status == GattCommunicationStatus.Success)
                    {
                        foreach (var service in gattDeviceServicesResult.Services)
                        {
                            customGattService = new CustomGattService(service);
                            if (await customGattService.Initialize())
                            {
                                _gattServices.Add(customGattService);
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                customGattService = null;
                gattDeviceServicesResult = null;
            }
        }

        public async Task<bool> AuthorizeOnDeviceAsync()
        {
            try
            {
                if (await Connect())
                {
                    var characteristic = _gattServices
                        .FirstOrDefault(x => x.GattService.Uuid == CustomBluetoothProfile.Authentication.Service)
                        ?.GattCharacteristics
                        .FirstOrDefault(x => x.GattCharacteristic.Uuid == CustomBluetoothProfile.Authentication.AuthCharacteristic);

                    if (characteristic != null)
                    {
                        var result = await characteristic.WriteValueAsync(new byte[] { 100, 20 });
                        return true;
                    }

                    return false;
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

        private void Disconnect()
        {
            _gattServices = null;
            _device?.Dispose();
            _device = null;
        }
    }

    public class CustomGattService
    {
        public GattDeviceService GattService { get; set; }
        public List<CustomGattCharacteristic> GattCharacteristics { get; set; }

        public CustomGattService()
        {
            GattCharacteristics = new List<CustomGattCharacteristic>();
        }

        public CustomGattService(GattDeviceService gattService)
        {
            GattService = gattService;
            GattCharacteristics = new List<CustomGattCharacteristic>();
        }

        public async Task<bool> Initialize()
        {
            GattCharacteristicsResult gattCharacteristicsResult = null;
            CustomGattCharacteristic gattCharacteristic = null;
            try
            {
                gattCharacteristicsResult = await GattService.GetCharacteristicsAsync();
                if (gattCharacteristicsResult.Status == GattCommunicationStatus.Success)
                {
                    foreach (var characteristic in gattCharacteristicsResult.Characteristics)
                    {
                        gattCharacteristic = new CustomGattCharacteristic(characteristic);
                        await gattCharacteristic.Initialize();
                        GattCharacteristics.Add(gattCharacteristic);
                    }
                }
                else
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                gattCharacteristicsResult = null;
                gattCharacteristic = null;
            }
        }
    }

    public class CustomGattCharacteristic : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public GattCharacteristic GattCharacteristic { get; set; }
        public GattCharacteristicProperties Properties { get; set; }
        public byte[] Value { get; set; }

        public CustomGattCharacteristic()
        {
        }

        public CustomGattCharacteristic(GattCharacteristic gattCharacteristic)
        {
            GattCharacteristic = gattCharacteristic;
            Properties = gattCharacteristic.CharacteristicProperties;
        }

        public async Task<bool> Initialize()
        {
            try
            {
                if (Properties.HasFlag(GattCharacteristicProperties.Notify))
                {
                    await GattCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
                        GattClientCharacteristicConfigurationDescriptorValue.Notify);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> WriteValueAsync(byte[] writeBytes)
        {
            try
            {
                var status = await GattCharacteristic.WriteValueAsync(writeBytes.AsBuffer(), GattWriteOption.WriteWithoutResponse);
                return status == GattCommunicationStatus.Success;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"WriteValue: {ex.Message}");
                return false;
            }
        }

        public async Task<byte[]> ReadValueAsync()
        {
            try
            {
                var result = await GattCharacteristic.ReadValueAsync(BluetoothCacheMode.Uncached);
                if (result.Status == GattCommunicationStatus.Success)
                {
                    return result.Value.ToArray();
                }

                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ReadValue: {ex.Message}");
                return null;
            }
        }

        private void GattCharacteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            try
            {
                OnValueChanged(nameof(Value));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ValueChanged: {ex.Message}");
            }
        }

        private void OnValueChanged(string propertyName)
        {
            try
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"OnValueChanged: {ex.Message}");
            }
        }
    }
}
