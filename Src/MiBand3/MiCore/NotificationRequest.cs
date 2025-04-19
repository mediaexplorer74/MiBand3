using System;
using Windows.ApplicationModel;
using Windows.UI.Notifications;

namespace MiBand3
{
    public class NotificationRequest
    {
        public enum DisplayMethods
        {
            AppIconOnly = 0,
            MessageTitle = 1,
            AppDisplayName = 2
        }

        public string DisplayName { get; set; }
        public string Id { get; set; }
        public bool IsOn { get; set; }
        public int DisplayMethod { get; set; }
        public string DisplayMethodAsText { get; set; }

        public NotificationRequest()
        {
        }

        public NotificationRequest(AppInfo appInfo)
        {
            DisplayName = appInfo.DisplayInfo.DisplayName;
            Id = appInfo.PackageFamilyName;
            IsOn = true;
            DisplayMethod = (int)DisplayMethods.AppDisplayName;

            GetMethodText();
        }

        private void GetMethodText()
        {
            switch ((DisplayMethods)DisplayMethod)
            {
                case DisplayMethods.AppDisplayName:
                    DisplayMethodAsText = "Display App-Name";
                    break;
                case DisplayMethods.AppIconOnly:
                    DisplayMethodAsText = "Display App-Icon";
                    break;
                case DisplayMethods.MessageTitle:
                    DisplayMethodAsText = "Display Message";
                    break;
            }
        }
    }
}


