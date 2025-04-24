
// Type: MiBandApp.Services.BandSyncReminderService
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// 

using MetroLog;
using MiBandApp.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

#nullable disable
namespace MiBandApp.Services
{
  public class BandSyncReminderService
  {
    private const string NotificationId = "Syncrhonize";
    private readonly MiBandApp.Storage.Settings.Settings _settings;
    private readonly BandSyncController _bandSyncController;
    private readonly ResourceLoader _resourceLoader = new ResourceLoader();
    private readonly ILogger _logger;

    public BandSyncReminderService(
      MiBandApp.Storage.Settings.Settings settings,
      BandSyncController bandSyncController,
      ILogManager logManager)
    {
      this._logger = logManager.GetLogger<BandSyncReminderService>();
      this._settings = settings;
      this._bandSyncController = bandSyncController;
      this._bandSyncController.SyncState.Updated += new EventHandler<MonitorableUpdatedEventArgs<BandSyncState>>(this.SyncStateOnUpdated);
      Task.Delay(5000).ContinueWith((Action<Task>) (t => this.CleanToastsHistory()));
    }

    public void Schedule()
    {
      this._logger.Debug(nameof (Schedule), (Exception) null);
      this.Cancel();
      this.ScheduleInDays(5);
      this.ScheduleInDays(6);
      this.ScheduleInDays(7);
    }

    public void Cancel()
    {
      this._logger.Debug(nameof (Cancel), (Exception) null);
      ToastNotifier toastNotifier = ToastNotificationManager.CreateToastNotifier();
      foreach (ScheduledToastNotification toastNotification in toastNotifier.GetScheduledToastNotifications().Where<ScheduledToastNotification>((Func<ScheduledToastNotification, bool>) (t => t.Id.StartsWith("Syncrhonize"))))
      {
        toastNotifier.RemoveFromSchedule(toastNotification);
        this._logger.Debug(string.Format("Removed scheduled notification {0}", (object) toastNotification.Id), (Exception) null);
      }
    }

    private void SyncStateOnUpdated(
      object sender,
      MonitorableUpdatedEventArgs<BandSyncState> monitorableUpdatedEventArgs)
    {
      if (!this._settings.SyncReminderEnabled || monitorableUpdatedEventArgs.UpdatedValue != BandSyncState.Success)
        return;
      this.Schedule();
    }

    private void ScheduleInDays(int days)
    {
      this._logger.Debug(string.Format("Scheduling notification in {0} days", (object) days), (Exception) null);
      DateTimeOffset dateTimeOffset = this._bandSyncController.LastSyncTime.AddDays((double) days);
      if (dateTimeOffset < DateTimeOffset.Now)
        return;
      this._logger.Debug(string.Format("Scheduling notification at {0}", (object) dateTimeOffset), (Exception) null);
      XmlDocument templateContent = ToastNotificationManager.GetTemplateContent((ToastTemplateType) 5);
    
      templateContent.GetElementsByTagName("text")[0].InnerText =
                this._resourceLoader.GetString("Notification_SynchronizeBand_Title");
      templateContent.GetElementsByTagName("text")[1].InnerText =
                 this._resourceLoader.GetString("Notification_SynchronizeBand_Subtitle");

      ScheduledToastNotification toastNotification1 = new ScheduledToastNotification(templateContent, dateTimeOffset);
      toastNotification1.Id = "Syncrhonize" + (object) days;
      ScheduledToastNotification toastNotification2 = toastNotification1;
      ToastNotificationManager.CreateToastNotifier().AddToSchedule(toastNotification2);
    }

    private void CleanToastsHistory()
    {
      try
      {
        this._logger.Debug(string.Format("Trying to {0}", (object) nameof (CleanToastsHistory)), (Exception) null);
        ToastNotificationManager.History.Clear();
        this._logger.Debug(string.Format("Completed {0}", (object) nameof (CleanToastsHistory)), (Exception) null);
        foreach (ScheduledToastNotification toastNotification in ToastNotificationManager.CreateToastNotifier().GetScheduledToastNotifications().ToList<ScheduledToastNotification>().Where<ScheduledToastNotification>((Func<ScheduledToastNotification, bool>) (t => t.Id.StartsWith("Syncrhonize"))))
          this._logger.Debug(string.Format("Have scheduled notification {0} at {1}", (object) toastNotification.Id, (object) toastNotification.DeliveryTime), (Exception) null);
      }
      catch (Exception ex)
      {
        this._logger.Error(string.Format("Exception when doing {0}", (object) nameof (CleanToastsHistory)), ex);
      }
    }
  }
}
