using MiBand3;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel;
using Windows.Storage;

namespace MiBand3
{
    public class NotificationResult
    {
        public List<NotificationRequest> Requests { get; set; }

        private ApplicationDataContainer LocalSettings = ApplicationData.Current.LocalSettings;

        public NotificationResult()
        {
            Requests = new List<NotificationRequest>();
        }

        public bool Initialize()
        {
            try
            {
                if (LocalSettings.Values["NotificationResult"] != null)
                {
                    var data = (NotificationResult)Helpers.FromXml(LocalSettings.Values["NotificationResult"].ToString(), typeof(NotificationResult));
                    Requests = data.Requests;
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void Add(AppInfo appInfo)
        {
            try
            {
                Requests.Add(new NotificationRequest(appInfo));
                LocalSettings.Values["NotificationResult"] = Helpers.ToXml(this, typeof(NotificationResult));
            }
            catch (Exception)
            {
            }
        }
    }
}
