
// Type: MiBandApp.ViewModels.DeviceSettings.DisplayViewModel
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// 

using MetroLog;
using MiBand.SDK.Configuration;
using MiBand.SDK.Core;
using MiBandApp.Services;
using MiBandApp.Tools;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;

#nullable disable
namespace MiBandApp.ViewModels.DeviceSettings
{
  public class DisplayViewModel : DeviceSettingViewModelBase
  {
    private readonly MiBandApp.Storage.Settings.Settings _settings;
    private readonly BandController _bandController;
    private readonly StatusBarNotificationService _statusBarNotificationService;
    private readonly ObservableCollection<bool> _displayItems = new ObservableCollection<bool>();
    private readonly ResourceLoader _resourceLoader = new ResourceLoader();
    private readonly ILogger _log;
    private bool _isDateShown;

    public DisplayViewModel(
      MiBandApp.Storage.Settings.Settings settings,
      BandController bandController,
      StatusBarNotificationService statusBarNotificationService,
      ILogManager logManager)
    {
      this._settings = settings;
      this._bandController = bandController;
      this._statusBarNotificationService = statusBarNotificationService;
      this._log = logManager.GetLogger<DisplayViewModel>();
      for (int index = 0; index < 5; ++index)
        this._displayItems.Add(false);
    }

    public ObservableCollection<bool> DisplayItems => this._displayItems;

    public bool IsDateAvailable
    {
      get
      {
        return this._bandController.DeviceInfo.Value.Capabilities.HasFlag((Enum) Capability.DateDisplay);
      }
    }

    public bool IsDateShown
    {
      get => this._isDateShown;
      set
      {
        this._isDateShown = value;
        this.NotifyOfPropertyChange(nameof (IsDateShown));
      }
    }

    public override async Task Load()
    {
      this.LoadDisplayItemsState();
      this.UpdateRegionalSettings();
    }

    private async void UpdateRegionalSettings()
    {
      try
      {
        await this._bandController.MiBand.Set24HTimeFormat(CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern.Contains("H")).ConfigureAwait(true);
        await this._bandController.MiBand.SetMetricUnitsSystem(this._settings.DistanceUnits == MiBandApp.Storage.Settings.Settings.DistanceUnit.Km).ConfigureAwait(true);
      }
      catch (Exception ex)
      {
        this._log.Warn(string.Format("Exception while doing {0}: {1}", (object) nameof (UpdateRegionalSettings), (object) ex), (Exception) null);
      }
    }

    public async void SaveDisplayItems()
    {
      try
      {
        DisplayItem newConfig = DisplayItem.None;
        for (int index = 0; index < 5; ++index)
          newConfig |= this.DisplayItems[index] ? (DisplayItem) (1 << index) : DisplayItem.None;
        if (this._settings.DisplayItemsConfig == newConfig && this._settings.DisplayDateEnabled == this.IsDateShown)
          return;
        await this._bandController.MiBand.SetDisplayItems(newConfig).ConfigureAwait(true);
        await this._bandController.MiBand.SetDateDisplayMode(this.GetDateDisplayMode());
        this._settings.DisplayItemsConfig = newConfig;
        this._settings.DisplayDateEnabled = this.IsDateShown;
      }
      catch
      {
        this._statusBarNotificationService.Show<StatusBarMessage>(new StatusBarMessage(this._resourceLoader.GetString("StatusFailedToSaveTryAgainLater"), TimeSpan.FromSeconds(2.0)));
      }
    }

    private DateDisplayMode GetDateDisplayMode()
    {
      return !this.IsDateShown ? DateDisplayMode.None : CultureHelper.GetDateDisplayMode();
    }

    private void LoadDisplayItemsState()
    {
      this.IsDateShown = this._settings.DisplayDateEnabled;
      int displayItemsConfig = (int) this._settings.DisplayItemsConfig;
      for (int index = 0; index < this._displayItems.Count; ++index)
        this._displayItems[index] = (displayItemsConfig >> index & 1) == 1;
    }
  }
}
