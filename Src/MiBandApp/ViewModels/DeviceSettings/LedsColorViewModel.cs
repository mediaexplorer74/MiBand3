
// Type: MiBandApp.ViewModels.DeviceSettings.LedsColorViewModel
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// 

using MiBand.SDK.Configuration;
using MiBand.SDK.Core;
using MiBandApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

#nullable disable
namespace MiBandApp.ViewModels.DeviceSettings
{
  public class LedsColorViewModel : DeviceSettingViewModelBase
  {
    private readonly BandController _bandController;
    private readonly MiBandApp.Storage.Settings.Settings _settings;
    private bool _isColorChangingNotInProgress = true;
    private int _colorThemeIndex = -1;
    private bool _changingColorThemeIndex;

    public LedsColorViewModel(BandController bandController, MiBandApp.Storage.Settings.Settings settings)
    {
      this._bandController = bandController;
      this._settings = settings;
    }

    public IEnumerable<BandColorTheme> ColorThemes => BandColorTheme.AllColors.Values;

    public bool IsColorChangingNotInProgress
    {
      get => this._isColorChangingNotInProgress;
      set
      {
        if (value == this._isColorChangingNotInProgress)
          return;
        this._isColorChangingNotInProgress = value;
        this.NotifyOfPropertyChange(nameof (IsColorChangingNotInProgress));
      }
    }

    public int ColorThemeIndex
    {
      get => this._colorThemeIndex;
      set
      {
        if (value == this._colorThemeIndex)
          return;
        this._changingColorThemeIndex = true;
        try
        {
          this._colorThemeIndex = value;
          this.NotifyOfPropertyChange(nameof (ColorThemeIndex));
        }
        finally
        {
          this._changingColorThemeIndex = false;
        }
      }
    }

    public override async Task Load()
    {
      BandColorTheme colorTheme = BandColorTheme.FromName(this._settings.BandColorThemeName);
      if (colorTheme.Name == null)
        return;
      this.ColorThemeIndex = this.ColorThemes.ToList<BandColorTheme>().FindIndex((Predicate<BandColorTheme>) (t => t.Name == colorTheme.Name));
    }

    public async void OnColorThemeChanged(SelectionChangedEventArgs args)
    {
      if (this._changingColorThemeIndex)
        return;
      MessageDialog errorDialog = (MessageDialog) null;
      if (!this._bandController.DeviceInfo.Value.Capabilities.HasFlag((Enum) Capability.LedColors))
        return;
      try
      {
        this.IsColorChangingNotInProgress = false;
        BandColorTheme selectedTheme = (BandColorTheme) args.AddedItems[0];
        await this._bandController.MiBand.SetColorTheme(selectedTheme, true).ConfigureAwait(true);
        this._settings.BandColorThemeName = selectedTheme.Name;
        selectedTheme = new BandColorTheme();
      }
      catch (Exception ex)
      {
        errorDialog = new MessageDialog("Some error happened during operation. " + ex.Message, "Oops...");
      }
      finally
      {
        this.IsColorChangingNotInProgress = true;
      }
      if (errorDialog == null)
        return;
      IUICommand iuiCommand = await errorDialog.ShowAsync();
    }
  }
}
