
// Type: MiBandApp.ViewModels.DeviceSettings.NotDisturbViewModel
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// 

using MetroLog;
using MiBand.SDK.Configuration;
using MiBandApp.Services;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;

#nullable disable
namespace MiBandApp.ViewModels.DeviceSettings
{
  public class NotDisturbViewModel : DeviceSettingViewModelBase
  {
    private readonly BandController _bandController;
    private readonly StatusBarNotificationService _statusBarNotificationService;
    private readonly MiBandApp.Storage.Settings.Settings _settings;
    private readonly ResourceLoader _resourceLoader = new ResourceLoader();
    private bool _isEnabled;
    private bool _isSmart;
    private bool _allowHighlightOnWristLift;
    private TimeSpan _startTime;
    private TimeSpan _endTime;
    private bool _detailedSettingsVisible;
    private readonly ILogger _log;

    public NotDisturbViewModel(
      BandController bandController,
      StatusBarNotificationService statusBarNotificationService,
      MiBandApp.Storage.Settings.Settings settings,
      ILogManager logManager)
    {
      this._bandController = bandController;
      this._statusBarNotificationService = statusBarNotificationService;
      this._settings = settings;
      this._log = logManager.GetLogger<NotDisturbViewModel>();
    }

    public bool IsEnabled
    {
      get => this._isEnabled;
      set
      {
        if (this._isEnabled == value)
          return;
        this._isEnabled = value;
        this.NotifyOfPropertyChange(nameof (IsEnabled));
        this.SaveSettings();
      }
    }

    public bool IsSmart
    {
      get => this._isSmart;
      set
      {
        if (value == this._isSmart)
          return;
        this._isSmart = value;
        this.NotifyOfPropertyChange(nameof (IsSmart));
      }
    }

    public bool AllowHighlightOnWristLift
    {
      get => this._allowHighlightOnWristLift;
      set
      {
        if (value == this._allowHighlightOnWristLift)
          return;
        this._allowHighlightOnWristLift = value;
        this.NotifyOfPropertyChange(nameof (AllowHighlightOnWristLift));
      }
    }

    public TimeSpan EndTime
    {
      get => this._endTime;
      set
      {
        if (value.Equals(this._endTime))
          return;
        this._endTime = value;
        this.NotifyOfPropertyChange(nameof (EndTime));
      }
    }

    public TimeSpan StartTime
    {
      get => this._startTime;
      set
      {
        if (value.Equals(this._startTime))
          return;
        this._startTime = value;
        this.NotifyOfPropertyChange(nameof (StartTime));
      }
    }

    public bool DetailedSettingsVisible
    {
      get => this._detailedSettingsVisible;
      set
      {
        if (value == this._detailedSettingsVisible)
          return;
        this._detailedSettingsVisible = value;
        this.NotifyOfPropertyChange(nameof (DetailedSettingsVisible));
      }
    }

    public void OnConfigureTapped() => this.DetailedSettingsVisible = true;

    public void OnSaveButtonTapped()
    {
      this.DetailedSettingsVisible = false;
      this.SaveSettings();
    }

    public override async Task Load()
    {
      this.IsSmart = this._settings.NotDisturbSmart;
      this.AllowHighlightOnWristLift = this._settings.NotDisturbAllowHighlightOnWristLift;
      this.StartTime = this._settings.NotDisturbStartTime;
      this.EndTime = this._settings.NotDisturbEndTime;
      this.IsEnabled = this._settings.NotDisturbEnabled;
    }

    private async void SaveSettings()
    {
      try
      {
        if (this._settings.NotDisturbEnabled == this.IsEnabled && this._settings.NotDisturbSmart == this.IsSmart && this._settings.NotDisturbAllowHighlightOnWristLift == this.AllowHighlightOnWristLift && this._settings.NotDisturbStartTime == this.StartTime && this._settings.NotDisturbEndTime == this.EndTime)
          return;
        NotDisturbConfig config = new NotDisturbConfig();
        config.IsEnabled = this.IsEnabled;
        config.IsSmart = this.IsSmart;
        config.AllowHighlightOnWristLift = this.AllowHighlightOnWristLift;
        config.StartTime = this.StartTime;
        config.EndTime = this.EndTime;
        await this._bandController.MiBand.SetNotDisturbConfig(config).ConfigureAwait(true);
        this._settings.NotDisturbEnabled = config.IsEnabled;
        this._settings.NotDisturbAllowHighlightOnWristLift = config.AllowHighlightOnWristLift;
        this._settings.NotDisturbEndTime = config.EndTime;
        this._settings.NotDisturbStartTime = config.StartTime;
        this._settings.NotDisturbSmart = config.IsSmart;
        config = (NotDisturbConfig) null;
      }
      catch (Exception ex)
      {
        this._statusBarNotificationService.Show<StatusBarMessage>(new StatusBarMessage(this._resourceLoader.GetString("StatusFailedToSaveTryAgainLater"), TimeSpan.FromSeconds(2.0)));
        this._log.Warn(string.Format("Couldn't {0} in {1}: {2}", (object) nameof (SaveSettings), (object) nameof (NotDisturbViewModel), (object) ex), (Exception) null);
      }
    }
  }
}
