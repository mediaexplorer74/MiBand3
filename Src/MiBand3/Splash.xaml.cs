using MiCore;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MiBand3
{
    public sealed partial class Splash : Page
    {
        public Splash()
        {
            InitializeComponent();
        }

        private async void Splash_Loaded(Object sender, RoutedEventArgs e)
        {
            App.CustomMiBand = new CustomMiBand();

            //DEBUG: substitute it by "(1==0)" for temp. MainPage access (without any real device)
            if (App.LocalSettings.Values["DeviceId"] == null)
            {
                Frame.Navigate(typeof(DevicePage));
            }
            else
            {
                await App.CustomMiBand.AuthenticateAppOnDevice();
                Frame.Navigate(typeof(MainPage));
            }
        }
    }
}


