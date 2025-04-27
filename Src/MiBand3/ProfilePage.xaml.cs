using MiCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace MiBand3
{

    public sealed partial class ProfilePage : Page
    {
        public ProfilePage()
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
                cboSteps.SelectedItem = steps.FirstOrDefault(x =>  x.Value == App.LocalSettings.Values["Profile_Steps"].ToString());
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

        private void Profile_Loaded(object sender, RoutedEventArgs e)
        {
        }
        private void cboGender_SelectionChanged(Object sender, SelectionChangedEventArgs e)
        {
            if (cboGender.SelectedItem != null)
            {
                App.LocalSettings.Values["Profile_Gender"] = cboGender.SelectedItem;
            }
        }

        private void cboHeight_SelectionChanged(Object sender, SelectionChangedEventArgs e)
        {
            if (cboHeight.SelectedItem != null)
            {
                App.LocalSettings.Values["Profile_Height"] = ((CustomComboBoxItem)cboHeight.SelectedItem).Value;
            }
        }

        private void cboSteps_SelectionChanged(Object sender, SelectionChangedEventArgs e)
        {
            if (cboSteps.SelectedItem != null)
            {
                App.LocalSettings.Values["Profile_Steps"] = ((CustomComboBoxItem)cboSteps.SelectedItem).Value;
            }
        }

        private void cboWeight_SelectionChanged(Object sender, SelectionChangedEventArgs e)
        {
            if (cboWeight.SelectedItem != null)
            {
                App.LocalSettings.Values["Profile_Weight"] = ((CustomComboBoxItem)cboWeight.SelectedItem).Value;
            }
        }

        private void dtpDateOfBirth_DateChanged(Object sender, DatePickerValueChangedEventArgs e)
        {
            App.LocalSettings.Values["Profile_DateOfBirth"] = dtpDateOfBirth.Date.ToString();
        }

        private void dtpSleep_TimeChanged(Object sender, TimePickerValueChangedEventArgs e)
        {
            App.LocalSettings.Values["Profile_Sleep"] = dtpSleep.Time.ToString();
        }

        private void txtAlias_TextChanged(Object sender, TextChangedEventArgs e)
        {
            App.LocalSettings.Values["Profile_Alias"] = txtAlias.Text;
        }

        private async void btnSave_Click(Object sender, RoutedEventArgs e)
        {
            pbProcessing.Visibility = Visibility.Visible;
            btnSave.IsEnabled = false;

            try
            {
                await App.CustomMiBand.setUserInfo(
                    App.LocalSettings.Values["Profile_Alias"] as String,
                    dtpDateOfBirth.Date.Date,
                    App.LocalSettings.Values["Profile_Gender"] as String,
                    Convert.ToInt32(App.LocalSettings.Values["Profile_Height"]),
                    Convert.ToInt32(App.LocalSettings.Values["Profile_Weight"]));

                if (Frame.CanGoBack)
                {
                    Frame.GoBack();
                }
            }
            catch (Exception ex)
            {
                Helpers.DebugWriter(typeof(ProfilePage), ex.Message);
            }
            finally
            {
                pbProcessing.Visibility = Visibility.Collapsed;
                btnSave.IsEnabled = true;
            }
        }

                      
    }
}



