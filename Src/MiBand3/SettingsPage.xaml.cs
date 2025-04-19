using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MiBand3
{
    public sealed partial class SettingsPage : Page
    {
        private bool bolLoading = true;

        public SettingsPage()
        {
            InitializeComponent();

            var displayItemsString = new List<string> { "Clock" };

            if (App.LocalSettings.Values[$"Setting_{(int)CustomMiBandResult.BandOperation.Steps}"] != null &&
                Convert.ToBoolean(App.LocalSettings.Values[$"Setting_{(int)CustomMiBandResult.BandOperation.Steps}"]))
            {
                displayItemsString.Add("Steps");
            }

            if (App.LocalSettings.Values[$"Setting_{(int)CustomMiBandResult.BandOperation.Distance}"] != null &&
                Convert.ToBoolean(App.LocalSettings.Values[$"Setting_{(int)CustomMiBandResult.BandOperation.Distance}"]))
            {
                displayItemsString.Add("Distance");
            }

            if (App.LocalSettings.Values[$"Setting_{(int)CustomMiBandResult.BandOperation.Calories}"] != null &&
                Convert.ToBoolean(App.LocalSettings.Values[$"Setting_{(int)CustomMiBandResult.BandOperation.Calories}"]))
            {
                displayItemsString.Add("Calories");
            }

            if (App.LocalSettings.Values[$"Setting_{(int)CustomMiBandResult.BandOperation.Heartrate}"] != null &&
                Convert.ToBoolean(App.LocalSettings.Values[$"Setting_{(int)CustomMiBandResult.BandOperation.Heartrate}"]))
            {
                displayItemsString.Add("Heart-Rate");
            }

            if (App.LocalSettings.Values[$"Setting_{(int)CustomMiBandResult.BandOperation.Battery}"] != null &&
                Convert.ToBoolean(App.LocalSettings.Values[$"Setting_{(int)CustomMiBandResult.BandOperation.Battery}"]))
            {
                displayItemsString.Add("Battery");
            }

            lblDisplayItems.Text = string.Join(", ", displayItemsString);

            // Check if Windows Version supports UserNotificationListener
            if (ApiInformation.IsTypePresent("Windows.UI.Notifications.Management.UserNotificationListener"))
            {
                App.LocalSettings.Values[$"Setting_{(int)CustomMiBandResult.BandOperation.Notifications}_IsEnabled"] = true;
            }
            else
            {
                App.LocalSettings.Values[$"Setting_{(int)CustomMiBandResult.BandOperation.Notifications}_IsEnabled"] = false;
            }

            tsDisplay.IsOn = Convert.ToBoolean(App.LocalSettings.Values["IsDisplayOnLiftWristEnabled"] ?? false);
            tsTimeformat.IsOn = Convert.ToBoolean(App.LocalSettings.Values["Is12hEnabled"] ?? false);
            tsDate.IsOn = Convert.ToBoolean(App.LocalSettings.Values["IsDateEnabled"] ?? false);
            tsGoal.IsOn = Convert.ToBoolean(App.LocalSettings.Values["IsGoalNotificationEnabled"] ?? false);
            tsRotate.IsOn = Convert.ToBoolean(App.LocalSettings.Values["IsRotateWristToSwitchInfoEnabled"] ?? false);

            if (App.LocalSettings.Values["IsWearLocationRightEnabled"] != null)
            {
                rbRightHand.IsChecked = Convert.ToBoolean(App.LocalSettings.Values["IsWearLocationRightEnabled"]);
                rbLeftHand.IsChecked = !rbRightHand.IsChecked;
            }
            else
            {
                rbRightHand.IsChecked = true;
            }

            tsDnD.IsOn = Convert.ToBoolean(App.LocalSettings.Values["IsDndEnabled"] ?? false);

            if (App.LocalSettings.Values["PeriodicSync"] != null)
            {
                chkPeriodicSync.SelectedValue = App.LocalSettings.Values["PeriodicSync"];
            }

            bolLoading = false;
        }

        private void SettingsPage_Loaded(object sender, RoutedEventArgs e)
        {
        }
        private async void tsDisplay_Toggled(object sender, RoutedEventArgs e)
        {
            if (!bolLoading)
            {
                pbProcessing.Visibility = Visibility.Visible;
                var toggleSwitch = sender as ToggleSwitch;
                toggleSwitch.IsEnabled = false;

                try
                {
                    App.LocalSettings.Values["IsDisplayOnLiftWristEnabled"] = toggleSwitch.IsOn;
                    await App.CustomMiBand.setActivateDisplayOnLiftWrist();
                }
                catch (Exception ex)
                {
                    Helpers.DebugWriter(typeof(SettingsPage), ex.Message);
                }
                finally
                {
                    pbProcessing.Visibility = Visibility.Collapsed;
                    toggleSwitch.IsEnabled = true;
                }
            }
        }

        private async void tsTimeformat_Toggled(object sender, RoutedEventArgs e)
        {
            if (!bolLoading)
            {
                pbProcessing.Visibility = Visibility.Visible;
                var toggleSwitch = sender as ToggleSwitch;
                toggleSwitch.IsEnabled = false;

                try
                {
                    App.LocalSettings.Values["Is12hEnabled"] = toggleSwitch.IsOn;
                    await App.CustomMiBand.setTimeFormatDisplay();
                }
                catch (Exception ex)
                {
                    Helpers.DebugWriter(typeof(SettingsPage), ex.Message);
                }
                finally
                {
                    pbProcessing.Visibility = Visibility.Collapsed;
                    toggleSwitch.IsEnabled = true;
                }
            }
        }

        private async void tsDate_Toggled(object sender, RoutedEventArgs e)
        {
            if (!bolLoading)
            {
                pbProcessing.Visibility = Visibility.Visible;
                var toggleSwitch = sender as ToggleSwitch;
                toggleSwitch.IsEnabled = false;

                try
                {
                    App.LocalSettings.Values["IsDateEnabled"] = toggleSwitch.IsOn;
                    await App.CustomMiBand.setDateDisplay();
                }
                catch (Exception ex)
                {
                    Helpers.DebugWriter(typeof(SettingsPage), ex.Message);
                }
                finally
                {
                    pbProcessing.Visibility = Visibility.Collapsed;
                    toggleSwitch.IsEnabled = true;
                }
            }
        }

        private void tsGoal_Toggled(object sender, RoutedEventArgs e)
        {
            if (!bolLoading)
            {
                pbProcessing.Visibility = Visibility.Visible;
                var toggleSwitch = sender as ToggleSwitch;
                toggleSwitch.IsEnabled = false;

                try
                {
                    App.LocalSettings.Values["IsGoalNotificationEnabled"] = toggleSwitch.IsOn;
                    App.CustomMiBand.setGoalNotification();
                }
                catch (Exception ex)
                {
                    Helpers.DebugWriter(typeof(SettingsPage), ex.Message);
                }
                finally
                {
                    pbProcessing.Visibility = Visibility.Collapsed;
                    toggleSwitch.IsEnabled = true;
                }
            }
        }

        private void hlbMenu_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(DisplayitemsPage));
        }

        private void chkPeriodicSync_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (chkPeriodicSync.SelectedIndex > -1)
            {
                App.LocalSettings.Values["PeriodicSync"] = chkPeriodicSync.SelectedItem.ToString();
            }
            else
            {
                App.LocalSettings.Values["PeriodicSync"] = "Off";
            }

            var task = BackgroundTaskRegistration.AllTasks.Values.FirstOrDefault(x => x.Name == "PeriodicalSync");
            task?.Unregister(true);
        }

        private void tsDnD_Toggled(object sender, RoutedEventArgs e)
        {
            if (!bolLoading)
            {
                pbProcessing.Visibility = Visibility.Visible;
                var toggleSwitch = sender as ToggleSwitch;
                toggleSwitch.IsEnabled = false;

                try
                {
                    App.LocalSettings.Values["IsDndEnabled"] = toggleSwitch.IsOn;
                    App.CustomMiBand.setDoNotDisturb();
                }
                catch (Exception ex)
                {
                    Helpers.DebugWriter(typeof(SettingsPage), ex.Message);
                }
                finally
                {
                    pbProcessing.Visibility = Visibility.Collapsed;
                    toggleSwitch.IsEnabled = true;
                }
            }
        }

        private void tsRotate_Toggled(object sender, RoutedEventArgs e)
        {
            if (!bolLoading)
            {
                pbProcessing.Visibility = Visibility.Visible;
                var toggleSwitch = sender as ToggleSwitch;
                toggleSwitch.IsEnabled = false;

                try
                {
                    App.LocalSettings.Values["IsRotateWristToSwitchInfoEnabled"] = toggleSwitch.IsOn;
                    App.CustomMiBand.setRotateWristToSwitchInfo();
                }
                catch (Exception ex)
                {
                    Helpers.DebugWriter(typeof(SettingsPage), ex.Message);
                }
                finally
                {
                    pbProcessing.Visibility = Visibility.Collapsed;
                    toggleSwitch.IsEnabled = true;
                }
            }
        }

        private async void rbRightHand_Click(object sender, RoutedEventArgs e)
        {
            if (!bolLoading)
            {
                pbProcessing.Visibility = Visibility.Visible;
                var radioButton = sender as RadioButton;
                radioButton.IsEnabled = false;

                try
                {
                    App.LocalSettings.Values["IsWearLocationRightEnabled"] = rbRightHand.IsChecked;
                    await App.CustomMiBand.setWearLocation();
                }
                catch (Exception ex)
                {
                    Helpers.DebugWriter(typeof(SettingsPage), ex.Message);
                }
                finally
                {
                    pbProcessing.Visibility = Visibility.Collapsed;
                    radioButton.IsEnabled = true;
                }
            }
        }

        private async void rbLeftHand_Click(object sender, RoutedEventArgs e)
        {
            if (!bolLoading)
            {
                pbProcessing.Visibility = Visibility.Visible;
                var radioButton = sender as RadioButton;
                radioButton.IsEnabled = false;

                try
                {
                    App.LocalSettings.Values["IsWearLocationRightEnabled"] = rbRightHand.IsChecked;
                    await App.CustomMiBand.setWearLocation();
                }
                catch (Exception ex)
                {
                    Helpers.DebugWriter(typeof(SettingsPage), ex.Message);
                }
                finally
                {
                    pbProcessing.Visibility = Visibility.Collapsed;
                    radioButton.IsEnabled = true;
                }
            }
        }
      
    }
}



