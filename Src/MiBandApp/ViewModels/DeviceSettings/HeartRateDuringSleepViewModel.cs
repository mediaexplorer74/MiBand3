// Decompiled with JetBrains decompiler
// Type: MiBandApp.ViewModels.DeviceSettings.HeartRateDuringSleepViewModel
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.exe

using MiBandApp.Services;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;

#nullable disable
namespace MiBandApp.ViewModels.DeviceSettings
{
  public class HeartRateDuringSleepViewModel : DeviceSettingViewModelBase
  {
    private readonly BandController _bandController;
    private readonly MiBandApp.Storage.Settings.Settings _settings;
    private readonly StatusBarNotificationService _statusBarNotificationService;
    private readonly ResourceLoader _resourceLoader = new ResourceLoader();
    private bool _isHeartRateDuringSleepEnabled;

    public HeartRateDuringSleepViewModel(
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
      this.IsHeartRateDuringSleepEnabled = this._settings.HeartRateDuringSleepEnabled;
    }

    public bool IsHeartRateDuringSleepEnabled
    {
      get => this._isHeartRateDuringSleepEnabled;
      set
      {
        if (value == this._isHeartRateDuringSleepEnabled)
          return;
        this._isHeartRateDuringSleepEnabled = value;
        this.NotifyOfPropertyChange(nameof (IsHeartRateDuringSleepEnabled));
        this.SaveHeartRateDuringSleep();
      }
    }

    private async void SaveHeartRateDuringSleep()
    {
      try
      {
        if (this._settings.HeartRateDuringSleepEnabled == this._isHeartRateDuringSleepEnabled)
          return;
        await this._bandController.MiBand.SetHeartRateDuringSleep(this._isHeartRateDuringSleepEnabled).ConfigureAwait(true);
        this._settings.HeartRateDuringSleepEnabled = this._isHeartRateDuringSleepEnabled;
      }
      catch
      {
        this._statusBarNotificationService.Show<StatusBarMessage>(new StatusBarMessage(this._resourceLoader.GetString("StatusFailedToSaveTryAgainLater"), TimeSpan.FromSeconds(2.0)));
      }
    }
  }
}
