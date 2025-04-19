using MiCore;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


namespace MiBand3
{
    public sealed partial class NotificationPage : Page
    {
        private bool bolLoading = true;

        private NotificationResult _result;
        public NotificationResult Result
        {
            get { return _result; }
            set { _result = value; }
        }

        protected override void OnNavigatedTo(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            Result = e.Parameter as NotificationResult;
        }

        private void ToggleSwitch_Toggled(Object sender, RoutedEventArgs e)
        {
            if (!bolLoading)
            {
                var toggleSwitch = sender as ToggleSwitch;
                if (toggleSwitch != null)
                {
                    var notificationRequest = toggleSwitch.DataContext as NotificationRequest;
                    if (notificationRequest != null)
                    {
                        notificationRequest.IsOn = toggleSwitch.IsOn;
                    }
                }
            }
        }

        private void NotificationPage_Loaded(Object sender, RoutedEventArgs e)
        {
            lvApps.ItemsSource = null;
            lvApps.ItemsSource = _result.Requests;
            bolLoading = false;
        }
    }
}


