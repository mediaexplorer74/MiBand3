
// Type: MiBandApp.ViewModels.DeviceSettings.ActivityReminderViewModel
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// 

using MiBand.SDK.Configuration;
using MiBandApp.Services;
using MiBandApp.ViewModels.Dialogs;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;

#nullable disable
namespace MiBandApp.ViewModels.DeviceSettings
{
  public class ActivityReminderViewModel : DeviceSettingViewModelBase
  {
    private readonly MiBandApp.Storage.Settings.Settings _settings;
    private readonly BandController _bandController;
    private readonly StatusBarNotificationService _statusBarNotificationService;
    private readonly ResourceLoader _resourceLoader = new ResourceLoader();
    private bool _isActivityReminderEnabled;

    public ActivityReminderViewModel(
      MiBandApp.Storage.Settings.Settings settings,
      BandController bandController,
      StatusBarNotificationService statusBarNotificationService)
    {
      this._settings = settings;
      this._bandController = bandController;
      this._statusBarNotificationService = statusBarNotificationService;
    }

    public bool IsActivityReminderEnabled
    {
      get => this._isActivityReminderEnabled;
      set
      {
        if (this._isActivityReminderEnabled == value)
          return;
        this._isActivityReminderEnabled = value;
        this.NotifyOfPropertyChange(nameof (IsActivityReminderEnabled));
        this.SaveActivityReminderSettings();
      }
    }

    public ActivityReminderDialogViewModel ActivityReminderDialog { get; } = new ActivityReminderDialogViewModel();

    public override async Task Load() => this.LoadActivityReminderSettings();

    private void LoadActivityReminderSettings()
    {
      this.ActivityReminderDialog.Initialize(this._settings.ActivityReminderStart, this._settings.ActivityReminderEnd);
      this.IsActivityReminderEnabled = this._settings.ActivityReminderEnabled;
      this.ActivityReminderDialog.Saved -= new EventHandler(this.ActivityReminderDialogOnSaved);
      this.ActivityReminderDialog.Saved += new EventHandler(this.ActivityReminderDialogOnSaved);
    }

    private void ActivityReminderDialogOnSaved(object sender, EventArgs eventArgs)
    {
      this.SaveActivityReminderSettings();
    }

    private async void SaveActivityReminderSettings()
    {
      try
      {
        if (this._settings.ActivityReminderEnabled == this.IsActivityReminderEnabled && this._settings.ActivityReminderStart == this.ActivityReminderDialog.SavedStartTime && this._settings.ActivityReminderEnd == this.ActivityReminderDialog.SavedEndTime)
          return;
        await this._bandController.MiBand.SetActivityReminder(new ActivityReminderConfig()
        {
          StartTime = this.ActivityReminderDialog.SavedStartTime,
          EndTime = this.ActivityReminderDialog.EndTime,
          IsEnabled = this.IsActivityReminderEnabled
        }).ConfigureAwait(true);
        this._settings.ActivityReminderEnabled = this.IsActivityReminderEnabled;
        this._settings.ActivityReminderStart = this.ActivityReminderDialog.SavedStartTime;
        this._settings.ActivityReminderEnd = this.ActivityReminderDialog.SavedEndTime;
      }
      catch
      {
        this._statusBarNotificationService.Show<StatusBarMessage>(new StatusBarMessage(this._resourceLoader.GetString("StatusFailedToSaveTryAgainLater"), TimeSpan.FromSeconds(2.0)));
      }
    }
  }
}
