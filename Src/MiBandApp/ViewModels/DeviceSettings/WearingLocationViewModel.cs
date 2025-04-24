
// Type: MiBandApp.ViewModels.DeviceSettings.WearingLocationViewModel
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// 

using MiBand.SDK.Configuration;
using MiBandApp.Services;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;

#nullable disable
namespace MiBandApp.ViewModels.DeviceSettings
{
  public class WearingLocationViewModel : DeviceSettingViewModelBase
  {
    private readonly MiBandApp.Storage.Settings.Settings _settings;
    private readonly BandController _bandController;
    private readonly StatusBarNotificationService _statusBarNotificationService;
    private readonly ResourceLoader _stringsLoader = new ResourceLoader();
    private BandWearLocation _wearLocation;

    public WearingLocationViewModel(
      MiBandApp.Storage.Settings.Settings settings,
      BandController bandController,
      StatusBarNotificationService statusBarNotificationService)
    {
      this._settings = settings;
      this._bandController = bandController;
      this._statusBarNotificationService = statusBarNotificationService;
    }

    public BandWearLocation WearLocation
    {
      get => this._wearLocation;
      set
      {
        if (value == BandWearLocation.None)
          return;
        this._wearLocation = value;
        this.NotifyOfPropertyChange(nameof (WearLocation));
        this.SaveWearLocation();
      }
    }

    public override async Task Load()
    {
      this.WearLocation = this._settings.BandWearLocation;
      await Task.Delay(200).ConfigureAwait(true);
    }

    private async void SaveWearLocation()
    {
      try
      {
        if (this._settings.BandWearLocation == this.WearLocation)
          return;
        await this._bandController.MiBand.SetWearLocation(this.WearLocation).ConfigureAwait(true);
        this._settings.BandWearLocation = this.WearLocation;
      }
      catch
      {
        this._statusBarNotificationService.Show<StatusBarMessage>(new StatusBarMessage(this._stringsLoader.GetString("StatusFailedToSaveTryAgainLater"), TimeSpan.FromSeconds(2.0)));
      }
    }
  }
}
