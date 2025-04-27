using MiCore;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Navigation;

namespace MiBand3
{
    public sealed partial class BatteryPage1 : Page
    {
        private BatteryResult _result;
        public BatteryResult Result
        {
            get { return _result; }
            set { _result = value; }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _result = e.Parameter as BatteryResult;
        }

        private void BatteryPage_Loaded(Object sender, RoutedEventArgs e)
        {
            if (_result != null)
            {
                lblPercentage.Text = $"{_result.Percentage} %";
                lblBefore.Text = "";
                lblChargingDate.Text = $"Last charging date: {_result.LastChargingDate:dd.MM.yyyy HH:mm}";
                lblEstimated.Text = GetEstimatedTime();
                pgPercentage.Value = _result.Percentage;
            }
        }

        private string GetEstimatedTime()
        {
            // 20 days according to the manufacturer (battery life) = 28800 minutes
            Double estimatedMinutes = 28800 / 100 * _result.Percentage;
            TimeSpan estimatedTimeSpan = DateTime.Now.AddMinutes(estimatedMinutes) - DateTime.Now;

            return $"Estimated time remaining: {estimatedTimeSpan.Days} days {estimatedTimeSpan.Hours} hours {estimatedTimeSpan.Minutes} minutes";
        }

        private void sliderPowerSaving_ValueChanged(Object sender, RangeBaseValueChangedEventArgs e)
        {
            if (lblDisplaySliderValue != null)
            {
                lblDisplaySliderValue.Text = $"{sliderPowerSaving.Value}%";
            }
        }

        private void chkSlider_Checked(Object sender, RoutedEventArgs e)
        {
            sliderPowerSaving.IsEnabled = chkSlider.IsChecked == true;
        }

        private void chkSlider_Unchecked(Object sender, RoutedEventArgs e)
        {
            sliderPowerSaving.IsEnabled = chkSlider.IsChecked == true;
        }
    }
}


