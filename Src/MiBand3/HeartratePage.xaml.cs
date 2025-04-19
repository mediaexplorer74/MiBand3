using System;
using System.ComponentModel;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


namespace MiBand3
{
    public sealed partial class HeartratePage : Page
    {
        private HeartResult _result;
        public HeartResult Result
        {
            get { return _result; }
            set
            {
                if (_result != null)
                {
                    _result.PropertyChanged -= Result_PropertyChanged;
                }

                _result = value;

                if (_result != null)
                {
                    _result.PropertyChanged += Result_PropertyChanged;
                }
            }
        }

        protected override void OnNavigatedTo(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            Result = e.Parameter as HeartResult;
        }

        private void Result_PropertyChanged(Object sender, PropertyChangedEventArgs e)
        {
            try
            {
                Debug.WriteLine($"Received: Heart-Rate in bpm");

                lblStepsDetails.Text = _result.HeartRate.ToString("N0");
                lblBefore.Text = Helpers.TimeSpanToText(_result.LastCheckDate);
            }
            catch (Exception)
            {
            }
            finally
            {
                prMeasurement.IsActive = False;
                btnMeasurement.IsEnabled = true;
            }
        }

        private void HeartratePage_Loaded(Object sender, RoutedEventArgs e)
        {
            lblBefore.Text = Helpers.TimeSpanToText(_result.LastCheckDate);
            lblStepsDetails.Text = _result.HeartRate.ToString("N0");
        }

        private async void btnMeasurement_Click(Object sender, RoutedEventArgs e)
        {
            prMeasurement.IsActive = true;
            btnMeasurement.IsEnabled = false;

            try
            {
                Debug.WriteLine($"Request: Heart-Rate in bpm");

                await App.CustomMiBand.SetAlertLevel(3);
                await _result.GetHeartRateMeasurement(
                    await App.CustomMiBand.GetCharacteristic(App.CustomMiBand.GetService(HeartResult.SERVICE), HeartResult.HEARTRATE_MEASUREMENT),
                    await App.CustomMiBand.GetCharacteristic(App.CustomMiBand.GetService(HeartResult.SERVICE), HeartResult.HEARTRATE_CONTROL_POINT));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR: {ex.Message}");
                btnMeasurement.IsEnabled = true;
                prMeasurement.IsActive = false;
            }
        }
    }
}



