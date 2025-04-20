using MiCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MiBand3
{
    public sealed partial class DevicePage : Page
    {
        private DeviceInformation mDevice;
        private DeviceInformationCollection mDevices;

        public DevicePage()
        {
            InitializeComponent();
        }

        private async void DevicePage_Loaded(Object sender, RoutedEventArgs e)
        {
            String mSelector = BluetoothLEDevice.GetDeviceSelectorFromPairingState(true);
            mDevices = await DeviceInformation.FindAllAsync(mSelector);

            App.LocalSettings.Values["DeviceId"] = null;
            App.LocalSettings.Values["IsAuthenticationNeeded"] = true;

            // Identification sequence
            foreach (var device in mDevices)
            {
                var deviceName = device.Name.ToUpper();
                if 
                ( 
                 deviceName == "MI BAND 2"
                 ||
                 deviceName == "MI BAND 3"
                 ||
                 deviceName == "MI BAND 4"
                 ||
                 deviceName == "MI BAND 5"
                )
                {
                    mDevice = device;
                    App.LocalSettings.Values["DeviceId"] = mDevice.Id;
                    break;
                }
            }

            // Authentication sequence
            if (mDevice != null)
            {
                lblTitle.Text = "Device found";
                lblSubTitle.Text = "Trying to authenticate on Band...";
                lblDescription.Text = "Tap on Band-Button to accept requested authentication!";

                App.CustomMiBand = new CustomMiBand();
                if (await App.CustomMiBand.AuthenticateAppOnDevice())
                {
                    Frame.Navigate(typeof(MainPage));
                }
                else
                {
                    pbProcess.IsIndeterminate = true;
                    lblTitle.Text = "Authentication failed!";
                    lblSubTitle.Text = "try again later...";
                    lblDescription.Text = "Make sure you tapped on Band-Button, do not ignore requested authentication!";
                }
            }
            else
            {
                lblTitle.Text = "Band not found";
                lblSubTitle.Text = "Make sure you already have paired your Band with your device...";
                lblDescription.Text = "Make sure that Bluetooth connection is enabled!";
            }
        }
    }
}



