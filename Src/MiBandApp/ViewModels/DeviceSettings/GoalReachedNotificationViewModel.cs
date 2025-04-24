
// Type: MiBandApp.ViewModels.DeviceSettings.GoalReachedNotificationViewModel
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// 

using MiBandApp.Services;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;

#nullable disable
namespace MiBandApp.ViewModels.DeviceSettings
{
  public class GoalReachedNotificationViewModel : DeviceSettingViewModelBase
  {
    private readonly BandController _bandController;
    private readonly MiBandApp.Storage.Settings.Settings _settings;
    private readonly StatusBarNotificationService _statusBarNotificationService;
    private readonly ResourceLoader _resourceLoader = new ResourceLoader();
    private bool _isGoalReachedNotificationEnabled;

    public GoalReachedNotificationViewModel(
      BandController bandController,
      MiBandApp.Storage.Settings.Settings settings,
      StatusBarNotificationService statusBarNotificationService)
    {
      this._bandController = bandController;
      this._settings = settings;
      this._statusBarNotificationService = statusBarNotificationService;
    }

    public override async Task Load()
    {
      this.IsGoalReachedNotificationEnabled = this._settings.GoalReachedNotificationEnabled;
    }

    public bool IsGoalReachedNotificationEnabled
    {
      get => this._isGoalReachedNotificationEnabled;
      set
      {
        if (value == this._isGoalReachedNotificationEnabled)
          return;
        this._isGoalReachedNotificationEnabled = value;
        this.NotifyOfPropertyChange(nameof (IsGoalReachedNotificationEnabled));
        this.SaveSetting();
      }
    }

    private async void SaveSetting()
    {
      try
      {
        if (this._settings.GoalReachedNotificationEnabled == this._isGoalReachedNotificationEnabled)
          return;
        await this._bandController.MiBand.SetGoalReachedNotification(this._isGoalReachedNotificationEnabled).ConfigureAwait(true);
        this._settings.GoalReachedNotificationEnabled = this._isGoalReachedNotificationEnabled;
      }
      catch
      {
        this._statusBarNotificationService.Show<StatusBarMessage>(new StatusBarMessage(this._resourceLoader.GetString("StatusFailedToSaveTryAgainLater"), TimeSpan.FromSeconds(2.0)));
      }
    }
  }
}
