// Decompiled with JetBrains decompiler
// Type: MiBandApp.ViewModels.SettingsPageViewModel
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.exe

using MiBand.SDK.Core;
using MiBandApp.Services;
using MiBandApp.Tools;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.System;
using Windows.UI.Popups;

#nullable disable
namespace MiBandApp.ViewModels
{
  public class SettingsPageViewModel : PageBaseViewModel
  {
    private readonly MiBandApp.Storage.Settings.Settings _settings;
    private readonly LicensingService _licensingService;
    private readonly BandSyncReminderService _bandSyncReminderService;
    private readonly BandController _bandController;
    private readonly ResourceLoader _stringLoader;
    private int _tabIndex;

    public SettingsPageViewModel(
      MiBandApp.Storage.Settings.Settings settings,
      LicensingService licensingService,
      BandSyncReminderService bandSyncReminderService,
      BandController bandController,
      CloudViewModel cloudViewModel)
    {
      this._settings = settings;
      this._licensingService = licensingService;
      this._bandSyncReminderService = bandSyncReminderService;
      this._bandController = bandController;
      this.CloudViewModel = cloudViewModel;
      this._stringLoader = new ResourceLoader();
    }

    public bool ShowSubscription { get; set; }

    public bool SyncReminderEnabled
    {
      get => this._settings.SyncReminderEnabled;
      set
      {
        this._settings.SyncReminderEnabled = value;
        if (value)
          this._bandSyncReminderService.Schedule();
        else
          this._bandSyncReminderService.Cancel();
      }
    }

    public bool IsLiveTileEnabled
    {
      get => this._settings.IsLiveTileEnabled;
      set => this._settings.IsLiveTileEnabled = value;
    }

    protected override async Task OnActivate()
    {
      if (!this.ShowSubscription)
        return;
      this.TabIndex = 1;
    }

    protected override async Task OnDeactivate(bool close)
    {
    }

    public CloudViewModel CloudViewModel { get; }

    public int TabIndex
    {
      get => this._tabIndex;
      set
      {
        if (this._tabIndex == value)
          return;
        this._tabIndex = value;
        this.NotifyOfPropertyChange(nameof (TabIndex));
      }
    }

    public bool IsProLifetimePurchased => this._licensingService.IsProLifetimeActive;

    public bool IsPro1YearPurchased => this._licensingService.IsPro1YearActive;

    public int DistanceUnitsIndex
    {
      get => (int) this._settings.DistanceUnits;
      set => this._settings.DistanceUnits = (MiBandApp.Storage.Settings.Settings.DistanceUnit) value;
    }

    public async void AuthorizeNotify()
    {
      if (this._bandController.Status == MiBandStatus.Connected)
        this._bandController.MiBand.Dispose();
      LauncherOptions launcherOptions = new LauncherOptions();
      int num = await Launcher.LaunchUriAsync(new Uri(string.Format("bindmiband-client:authorize?UserId={0}", (object) this._settings.GetUserId())), launcherOptions) ? 1 : 0;
    }

    public async void PurchaseLifetime()
    {
      MessageDialog messageDialog = (MessageDialog) null;
      try
      {
        await this._licensingService.PurchaseLifetime().ConfigureAwait(true);
        this.NotifyOfPropertyChange<bool>((Expression<Func<bool>>) (() => this.IsProLifetimePurchased));
      }
      catch (Exception ex)
      {
        messageDialog = new MessageDialog(this._stringLoader.GetString("SettingsPageErrorOnPurchaseMessage"), this._stringLoader.GetString("MessageOopsHeader"));
      }
      if (messageDialog == null)
        return;
      IUICommand iuiCommand = await messageDialog.ShowAsync();
    }

    public async void Purchase1Year()
    {
      MessageDialog messageDialog = (MessageDialog) null;
      try
      {
        await this._licensingService.Purchase1Year().ConfigureAwait(true);
        this.NotifyOfPropertyChange<bool>((Expression<Func<bool>>) (() => this.IsPro1YearPurchased));
      }
      catch (Exception ex)
      {
        messageDialog = new MessageDialog(this._stringLoader.GetString("SettingsPageErrorOnPurchaseMessage"), this._stringLoader.GetString("MessageOopsHeader"));
      }
      if (messageDialog == null)
        return;
      IUICommand iuiCommand = await messageDialog.ShowAsync();
    }
  }
}
