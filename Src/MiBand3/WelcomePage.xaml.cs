using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace MiBand3
{
    public sealed partial class WelcomePage : Page
    {
        private bool _readyToSave = false;

        public WelcomePage()
        {
            InitializeComponent();

            // Initialize height options
            var heights = new List<CustomComboBoxItem>();
            for (int i = 80; i <= 240; i++)
            {
                heights.Add(new CustomComboBoxItem($"{i} cm", i.ToString()));
            }
            cboHeight.ItemsSource = heights;
            cboHeight.DisplayMemberPath = "Text";

            // Initialize weight options
            var weights = new List<CustomComboBoxItem>();
            for (int i = 30; i <= 170; i++)
            {
                weights.Add(new CustomComboBoxItem($"{i} kg", i.ToString()));
            }
            cboWeight.ItemsSource = weights;
            cboWeight.DisplayMemberPath = "Text";

            // Initialize steps options
            var steps = new List<CustomComboBoxItem>();
            for (int i = 2000; i <= 50000; i += 1000)
            {
                steps.Add(new CustomComboBoxItem($"{i} steps", i.ToString()));
            }
            cboSteps.ItemsSource = steps;
            cboSteps.DisplayMemberPath = "Text";

            // Load saved settings
            if (App.LocalSettings.Values["Profile_Alias"] != null)
            {
                txtAlias.Text = App.LocalSettings.Values["Profile_Alias"].ToString();
            }

            if (App.LocalSettings.Values["Profile_Gender"] != null)
            {
                cboGender.SelectedValue = App.LocalSettings.Values["Profile_Gender"];
            }

            if (App.LocalSettings.Values["Profile_Height"] != null)
            {
                cboHeight.SelectedItem = heights.FirstOrDefault(x => x.Value == App.LocalSettings.Values["Profile_Height"].ToString());
            }

            if (App.LocalSettings.Values["Profile_Weight"] != null)
            {
                cboWeight.SelectedItem = weights.FirstOrDefault(x => x.Value == App.LocalSettings.Values["Profile_Weight"].ToString());
            }

            if (App.LocalSettings.Values["Profile_Steps"] != null)
            {
                cboSteps.SelectedItem = steps.FirstOrDefault(x => x.Value == App.LocalSettings.Values["Profile_Steps"].ToString());
            }

            if (App.LocalSettings.Values["Profile_DateOfBirth"] != null)
            {
                dtpDateOfBirth.Date = DateTime.Parse(App.LocalSettings.Values["Profile_DateOfBirth"].ToString());
            }

            if (App.LocalSettings.Values["Profile_Sleep"] != null)
            {
                dtpSleep.Time = TimeSpan.Parse(App.LocalSettings.Values["Profile_Sleep"].ToString());
            }
        }

        private void cboGender_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboGender.SelectedItem != null)
            {
                App.LocalSettings.Values["Profile_Gender"] = cboGender.SelectedItem;
            }
        }

        private void cboHeight_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboHeight.SelectedItem != null)
            {
                App.LocalSettings.Values["Profile_Height"] = ((CustomComboBoxItem)cboHeight.SelectedItem).Value;
            }
        }

        private void cboSteps_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboSteps.SelectedItem != null)
            {
                App.LocalSettings.Values["Profile_Steps"] = ((CustomComboBoxItem)cboSteps.SelectedItem).Value;
            }
        }

        private void cboWeight_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboWeight.SelectedItem != null)
            {
                App.LocalSettings.Values["Profile_Weight"] = ((CustomComboBoxItem)cboWeight.SelectedItem).Value;
            }
        }

        private void dtpDateOfBirth_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            App.LocalSettings.Values["Profile_DateOfBirth"] = dtpDateOfBirth.Date.ToString();
        }

        private void dtpSleep_TimeChanged(object sender, TimePickerValueChangedEventArgs e)
        {
            App.LocalSettings.Values["Profile_Sleep"] = dtpSleep.Time.ToString();
        }

        private void txtAlias_TextChanged(object sender, TextChangedEventArgs e)
        {
            App.LocalSettings.Values["Profile_Alias"] = txtAlias.Text;
        }

        private void btnAddDevice_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(DevicePage));
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            btnSave.IsEnabled = false;

            try
            {
                if (_readyToSave)
                {
                    App.LocalSettings.Values["Setting_8"] = false;
                    App.LocalSettings.Values[$"Setting_{(int)CustomMiBandResult.BandOperation.Battery}"] = true;
                    App.LocalSettings.Values[$"Setting_{(int)CustomMiBandResult.BandOperation.Distance}"] = true;
                    App.LocalSettings.Values[$"Setting_{(int)CustomMiBandResult.BandOperation.Steps}"] = true;
                    App.LocalSettings.Values[$"Setting_{(int)CustomMiBandResult.BandOperation.Calories}"] = true;
                    App.LocalSettings.Values[$"Setting_{(int)CustomMiBandResult.BandOperation.Heartrate}"] = true;

                    App.CustomMiBand = new CustomMiBand();
                    var result = await App.CustomMiBand.ConnectWithAuth();
                    if (result)
                    {
                        await App.CustomMiBand.UpdateOperations();
                    }
                    else
                    {
                        // Device not reachable
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                btnSave.IsEnabled = true;
            }
        }
    }
}



