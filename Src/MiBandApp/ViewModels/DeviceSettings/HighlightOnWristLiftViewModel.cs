// Decompiled with JetBrains decompiler
// Type: MiBandApp.ViewModels.DeviceSettings.HighlightOnWristLiftViewModel
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
  public class HighlightOnWristLiftViewModel : DeviceSettingViewModelBase
  {
    private readonly StatusBarNotificationService _statusBarNotificationService;
    private readonly MiBandApp.Storage.Settings.Settings _settings;
    private readonly BandController _bandController;
    private readonly ResourceLoader _resourceLoader = new ResourceLoader();
    private bool _isHighlightOnWristLiftEnabled;

    public HighlightOnWristLiftViewModel(
      StatusBarNotificationService statusBarNotificationService,
      MiBandApp.Storage.Settings.Settings settings,
      BandController bandController)
    {
      this._statusBarNotificationService = statusBarNotificationService;
      this._settings = settings;
      this._bandController = bandController;
    }

    public bool IsHighlightOnWristLiftEnabled
    {
      get => this._isHighlightOnWristLiftEnabled;
      set
      {
        if (value == this._isHighlightOnWristLiftEnabled)
          return;
        this._isHighlightOnWristLiftEnabled = value;
        this.NotifyOfPropertyChange(nameof (IsHighlightOnWristLiftEnabled));
        this.SaveHighlightOnWristLift();
      }
    }

    public override async Task Load()
    {
      this.IsHighlightOnWristLiftEnabled = this._settings.HighlightOnWristLiftEnabled;
    }

    private async void SaveHighlightOnWristLift()
    {
      try
      {
        if (this._settings.HighlightOnWristLiftEnabled == this._isHighlightOnWristLiftEnabled)
          return;
        await this._bandController.MiBand.SetHighlightOnWristLift(this._isHighlightOnWristLiftEnabled).ConfigureAwait(true);
        this._settings.HighlightOnWristLiftEnabled = this._isHighlightOnWristLiftEnabled;
      }
      catch
      {
        this._statusBarNotificationService.Show<StatusBarMessage>(new StatusBarMessage(this._resourceLoader.GetString("StatusFailedToSaveTryAgainLater"), TimeSpan.FromSeconds(2.0)));
      }
    }
  }
}
