using MiCore;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MiBand3
{
    public sealed partial class DisplayitemsPage : Page
    {
        public DisplayitemsPage()
        {
            InitializeComponent();

            if (App.LocalSettings.Values[$"Setting_{(int)CustomMiBandResult.BandOperation.Battery}"] != null)
                tsBattery.IsOn = Convert.ToBoolean(App.LocalSettings.Values[$"Setting_{(int)CustomMiBandResult.BandOperation.Battery}"]);

            if (App.LocalSettings.Values[$"Setting_{(int)CustomMiBandResult.BandOperation.Distance}"] != null)
                tsDistance.IsOn = Convert.ToBoolean(App.LocalSettings.Values[$"Setting_{(int)CustomMiBandResult.BandOperation.Distance}"]);

            if (App.LocalSettings.Values[$"Setting_{(int)CustomMiBandResult.BandOperation.Steps}"] != null)
                tsSteps.IsOn = Convert.ToBoolean(App.LocalSettings.Values[$"Setting_{(int)CustomMiBandResult.BandOperation.Steps}"]);

            if (App.LocalSettings.Values[$"Setting_{(int)CustomMiBandResult.BandOperation.Calories}"] != null)
                tsCalories.IsOn = Convert.ToBoolean(App.LocalSettings.Values[$"Setting_{(int)CustomMiBandResult.BandOperation.Calories}"]);

            if (App.LocalSettings.Values[$"Setting_{(int)CustomMiBandResult.BandOperation.Heartrate}"] != null)
                tsHeartrate.IsOn = Convert.ToBoolean(App.LocalSettings.Values[$"Setting_{(int)CustomMiBandResult.BandOperation.Heartrate}"]);
        }

        private void DisplayItemsPage_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            pbProcessing.Visibility = Visibility.Visible;
            btnSave.IsEnabled = false;

            try
            {
                App.LocalSettings.Values[$"Setting_{(int)CustomMiBandResult.BandOperation.Battery}"] = tsBattery.IsOn;
                App.LocalSettings.Values[$"Setting_{(int)CustomMiBandResult.BandOperation.Distance}"] = tsDistance.IsOn;
                App.LocalSettings.Values[$"Setting_{(int)CustomMiBandResult.BandOperation.Steps}"] = tsSteps.IsOn;
                App.LocalSettings.Values[$"Setting_{(int)CustomMiBandResult.BandOperation.Calories}"] = tsCalories.IsOn;
                App.LocalSettings.Values[$"Setting_{(int)CustomMiBandResult.BandOperation.Heartrate}"] = tsHeartrate.IsOn;

                await App.CustomMiBand.setDisplayItems();

                if (Frame.CanGoBack)
                {
                    Frame.GoBack();
                }
            }
            catch (Exception ex)
            {
                Helpers.DebugWriter(typeof(DisplayitemsPage), ex.Message);
            }
            finally
            {
                pbProcessing.Visibility = Visibility.Collapsed;
                btnSave.IsEnabled = true;
            }
        }

       
    }
}



