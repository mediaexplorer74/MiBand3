using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.UI.Notifications.Management;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using MiNotificationWatcher;
using MiCore;


namespace MiBand3
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void btnSync_Click(Object sender, RoutedEventArgs e)
        {
            pgWorking.Visibility = Visibility.Visible;
            btnSync.IsEnabled = false;

            try
            {
                if (App.LocalSettings.Values["DeviceId"] != null)
                {
                    await App.CustomMiBand.UpdateOperations();
                }

                CustomMasterDetailsView.ItemsSource = null;
                CustomMasterDetailsView.ItemsSource = App.CustomMiBand.DisplayItems.OrderBy(x => (int)x.Operation);
                CustomMasterDetailsView.ItemsSource = App.CustomMiBand.DisplayItems.OrderBy(x => (int)x.Operation);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR: {ex.Message}");
                pgWorking.Visibility = Visibility.Collapsed;
                btnSync.IsEnabled = true;
            }
            finally
            {
                pgWorking.Visibility = Visibility.Collapsed;
                btnSync.IsEnabled = true;
            }
        }

        private void btnDevice_Click(Object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(DevicePage));
        }

        private async Task<bool> RegisterTaskForNotifications()
        {
            try
            {
                if (BackgroundTaskRegistration.AllTasks.Values.FirstOrDefault(x => x.Name == "UserNotificationChanged") == null)
                {
                    var status = await BackgroundExecutionManager.RequestAccessAsync();
                    if (status != BackgroundAccessStatus.AlwaysAllowed && status !=
                        BackgroundAccessStatus.AllowedSubjectToSystemPolicy)
                    {
                        return false;
                    }

                    BackgroundTaskBuilder builder = new BackgroundTaskBuilder()
                    {
                        Name = "UserNotificationChanged",
                        TaskEntryPoint = "MiNotificationWatcher.NotificationChanged"//typeof(MiNotificationWatcher.NotificationChanged).FullName
                    };

                    builder.SetTrigger(new UserNotificationChangedTrigger(
                                 Windows.UI.Notifications.NotificationKinds.Toast));
                    builder.Register();

                    return true;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task<bool> RegisterTaskForSync()
        {
            try
            {
                uint periodTime = 15;

                if (App.LocalSettings.Values["PeriodicSync"] != null)
                {
                    if (App.LocalSettings.Values["PeriodicSync"].ToString() == "Off")
                    {
                        var task = BackgroundTaskRegistration.AllTasks.Values.FirstOrDefault(
                            x => x.Name == "PeriodicalSync");
                        task?.Unregister(true);
                        return false;
                    }
                    else
                    {
                        periodTime = Convert.ToUInt32(App.LocalSettings.Values["PeriodicSync"]);
                    }
                }

                if (BackgroundTaskRegistration.AllTasks.Values.FirstOrDefault(
                    x => x.Name == "PeriodicalSync") == null)
                {
                    var status = await BackgroundExecutionManager.RequestAccessAsync();
                    if (status != BackgroundAccessStatus.AlwaysAllowed && status != BackgroundAccessStatus.AllowedSubjectToSystemPolicy)
                    {
                        return false;
                    }

                    var builder = new BackgroundTaskBuilder
                    {
                        Name = "PeriodicalSync",
                        TaskEntryPoint = "MiNotificationWatcher.PeriodicalSync"//typeof(MiNotificationWatcher.PeriodicalSync).FullName
                    };

                    builder.SetTrigger(new TimeTrigger(periodTime, false));
                    builder.Register();

                    return true;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async void MainPage_Loaded(Object sender, RoutedEventArgs e)
        {
            try
            {
                CustomMasterDetailsView.ItemsSource = null;
                CustomMasterDetailsView.ItemsSource = App.CustomMiBand.DisplayItems.OrderBy(
                    x => x.Operation);

                if (await RegisterTaskForNotifications())
                {
                    Debug.WriteLine($"BackgroundTask for Notifications running!");
                }
                else
                {
                    Debug.WriteLine($"BackgroundTask for Notifications NOT running!");
                }

                if (await RegisterTaskForSync())
                {
                    Debug.WriteLine($"BackgroundTask for Sync running!");
                }
                else
                {
                    Debug.WriteLine($"BackgroundTask for Sync NOT running!");
                }

                btnSync_Click(sender, e);

                await GetNotificationsListenerAccess();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR: {ex.Message}");
            }
        }

        private async Task<bool> GetNotificationsListenerAccess()
        {
            try
            {
                var listener = UserNotificationListener.Current;
                var accessStatus = await listener.RequestAccessAsync();

                return accessStatus == UserNotificationListenerAccessStatus.Allowed;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void Frame_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (args.NewValue is CustomMiBandResult result)
            {
                switch (result.Operation)
                {
                    case (int)CustomMiBandResult.BandOperation.Battery :
                        ((Frame)sender).Navigate(typeof(BatteryPage), App.CustomMiBand.BatteryResult);
                        break;
                    case (int)CustomMiBandResult.BandOperation.Notifications :
                        ((Frame)sender).Navigate(typeof(NotificationPage), App.CustomMiBand.NotificationResult);
                        break;
                    case (int)CustomMiBandResult.BandOperation.Calories :
                        ((Frame)sender).Navigate(typeof(StepsPage), App.CustomMiBand.StepResult);
                        break;
                    case (int)CustomMiBandResult.BandOperation.Steps :
                        ((Frame)sender).Navigate(typeof(StepsPage), App.CustomMiBand.StepResult);
                        break;
                    case (int)CustomMiBandResult.BandOperation.Distance:
                        ((Frame)sender).Navigate(typeof(DistancePage), App.CustomMiBand.StepResult);
                        break;
                    case (int)CustomMiBandResult.BandOperation.Heartrate:
                        ((Frame)sender).Navigate(typeof(HeartratePage), App.CustomMiBand.HeartResult);
                        break;
                }
            }
        }

        private void btnSetting_Click(Object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SettingsPage));
        }

        private void btnProfile_Click(Object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Profile));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.SourcePageType == typeof(WelcomePage))
            {
                btnSync_Click(null, null);
            }
        }
    }
}




