using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ClassesCollection;
using Windows.ApplicationModel.Background;
using Windows.UI.Notifications;
using Windows.UI.Notifications.Management;

namespace BackgroundTaskRuntime
{
    public sealed class NotificationChanged : IBackgroundTask
    {
        private BackgroundTaskDeferral _deferral;
        private CustomMiBand _customMiBand;
        private UserNotificationListener _userNotifListener;
        private NotificationRequest _singleNotification;
        private NotificationBinding _toastBinding;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            _deferral = taskInstance.GetDeferral();
            _customMiBand = null;

            try
            {
                Debug.WriteLine($"MESSAGE: BackgroundTask: {typeof(NotificationChanged).FullName} running!");

                _userNotifListener = UserNotificationListener.Current;
                _customMiBand = new CustomMiBand();

                if (_userNotifListener.GetAccessStatus() == UserNotificationListenerAccessStatus.Allowed)
                {
                    // Get all notifications
                    IReadOnlyList<UserNotification> userNotifications = await _userNotifListener.GetNotificationsAsync(NotificationKinds.Toast);

                    // Get the last notification from the list
                    var userNotification = userNotifications.LastOrDefault();

                    if (userNotification != null && userNotification.AppInfo != null)
                    {
                        // If the notification is older than 5 seconds, ignore it
                        TimeSpan validityTime = DateTime.Now - userNotification.CreationTime.DateTime;
                        if (validityTime.TotalSeconds < 5)
                        {
                            // Add the app to the notification list if it's new
                            if (_customMiBand.NotificationResult.Requests.All(x => x.Id != userNotification.AppInfo.PackageFamilyName))
                            {
                                _customMiBand.NotificationResult.Add(userNotification.AppInfo);
                            }

                            // Get the notification request info
                            _singleNotification = _customMiBand.NotificationResult.Requests.FirstOrDefault(x => x.Id == userNotification.AppInfo.PackageFamilyName);
                            if (_singleNotification != null)
                            {
                                // Check if notifications for the app are active
                                if (_singleNotification.IsOn)
                                {
                                    // Get the title and text of the notification
                                    string title = "";
                                    string text = "";

                                    _toastBinding = userNotification.Notification.Visual.GetBinding(KnownNotificationBindings.ToastGeneric);
                                    if (_toastBinding != null)
                                    {
                                        if (_toastBinding.GetTextElements().Count > 0)
                                        {
                                            title = _toastBinding.GetTextElements().FirstOrDefault()?.Text;
                                            text = string.Join("\n", _toastBinding.GetTextElements().Skip(1).Select(x => x.Text));
                                        }
                                    }

                                    // Send the notification to the band
                                    await _customMiBand.SetNewAlert(_singleNotification, title, text);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR: {ex.Message}");
            }
            finally
            {
                _customMiBand?.Dispose();
                _customMiBand = null;
                _userNotifListener = null;
                _singleNotification = null;
                _toastBinding = null;

                _deferral.Complete();
            }
        }
    }
}

