
// Type: MiBandApp.ViewModels.DeviceSettings.FlipDisplayOnWristRotateViewModel
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
  public class FlipDisplayOnWristRotateViewModel : DeviceSettingViewModelBase
  {
    private readonly StatusBarNotificationService _statusBarNotificationService;
    private readonly MiBandApp.Storage.Settings.Settings _settings;
    private readonly BandController _bandController;
    private readonly ResourceLoader _resourceLoader = new ResourceLoader();
    private bool _isEnabled;

    public FlipDisplayOnWristRotateViewModel(
      StatusBarNotificationService statusBarNotificationService,
      MiBandApp.Storage.Settings.Settings settings,
      BandController bandController)
    {
      this._statusBarNotificationService = statusBarNotificationService;
      this._settings = settings;
      this._bandController = bandController;
    }

    public bool IsEnabled
    {
      get => this._isEnabled;
      set
      {
        if (value == this._isEnabled)
          return;
        this._isEnabled = value;
        this.NotifyOfPropertyChange(nameof (IsEnabled));
        this.SaveSetting();
      }
    }

    public override async Task Load()
    {
      this.IsEnabled = this._settings.FlipDisplayOnWristRotateEnabled;
    }

    private async void SaveSetting()
    {
      try
      {
        if (this._settings.FlipDisplayOnWristRotateEnabled == this._isEnabled)
          return;
        await this._bandController.MiBand.SetFlipDisplayOnWristRotate(this._isEnabled).ConfigureAwait(true);
        this._settings.FlipDisplayOnWristRotateEnabled = this._isEnabled;
      }
      catch
      {
        this._statusBarNotificationService.Show<StatusBarMessage>(new StatusBarMessage(this._resourceLoader.GetString("StatusFailedToSaveTryAgainLater"), TimeSpan.FromSeconds(2.0)));
      }
    }
  }
}
