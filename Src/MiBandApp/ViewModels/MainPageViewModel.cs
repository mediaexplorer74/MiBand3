// Decompiled with JetBrains decompiler
// Type: MiBandApp.ViewModels.MainPageViewModel
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.exe

using Caliburn.Micro;
using MiBand.SDK.Configuration;
using MiBand.SDK.Core;
using MiBand.SDK.Data;
using MiBandApp.Controls;
using MiBandApp.Data.Activities;
using MiBandApp.Services;
using MiBandApp.Storage.DataBases;
using MiBandApp.Storage.Tables;
using MiBandApp.Tools;
using MiBandApp.ViewModels.Activities;
using MiBandApp.ViewModels.MainPageComponents;
using MiBandApp.ViewModels.Tabs;
using MiBandApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Phone.UI.Input;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

#nullable disable
namespace MiBandApp.ViewModels
{
  public class MainPageViewModel : PageBaseViewModel, IViewAware
  {
    private readonly INavigationService _navigationService;
    private readonly MiBandApp.Storage.Settings.Settings _settings;
    private readonly UpdatesHistoryService _updatesHistoryService;
    private readonly BandSyncController _bandSyncController;
    private readonly BandSyncReminderService _bandSyncReminderService;
    private readonly LicensingService _licensingService;
    private readonly ActivitiesDataBase _activitiesDataBase;
    private readonly ResourceLoader _stringsLoader;
    private readonly OneDriveSyncService _oneDriveSyncService;
    private readonly BandController _bandController;
    private bool _isRefreshing;
    private MainPage _mainPageView;

    public MainPageViewModel(
      INavigationService navigationService,
      MiBandApp.Storage.Settings.Settings settings,
      UpdatesHistoryService updatesHistoryService,
      BandSyncController bandSyncController,
      BandSyncReminderService bandSyncReminderService,
      LicensingService licensingService,
      WalkTabViewModel walkTabViewModel,
      SleepTabViewModel sleepTabViewModel,
      ActivitiesDataBase activitiesDataBase,
      HeartRateTabViewModel heartRateTabViewModel,
      MessagesViewModel messagesViewModel,
      OneDriveSyncService oneDriveSyncService,
      BandController bandController)
    {
      this._navigationService = navigationService;
      this._settings = settings;
      this._updatesHistoryService = updatesHistoryService;
      this._bandController = bandController;
      this._licensingService = licensingService;
      this.WalkTabViewModel = walkTabViewModel;
      this.SleepTabViewModel = sleepTabViewModel;
      this._activitiesDataBase = activitiesDataBase;
      this.HeartRateTabViewModel = heartRateTabViewModel;
      this.MessagesViewModel = messagesViewModel;
      this._oneDriveSyncService = oneDriveSyncService;
      this._bandController = bandController;
      this._bandSyncController = bandSyncController;
      this._bandSyncReminderService = bandSyncReminderService;
      this._stringsLoader = new ResourceLoader();
      StatusBar.GetForCurrentView().put_BackgroundOpacity(1.0);
      this._bandController.StatusChanged += new EventHandler(this.BandControllerOnStatusChanged);
      this._bandController.BindingState.Updated += new EventHandler<MonitorableUpdatedEventArgs<BindingState>>(this.BindingStateOnUpdated);
      this._bandController.CommunicationOperation.Updated += new EventHandler<MonitorableUpdatedEventArgs<CommunicationOperation>>(this.CommunicationOperationOnUpdated);
      this._bandController.DeviceInfo.Updated += (EventHandler<MonitorableUpdatedEventArgs<BandDeviceInfo>>) ((sender, args) =>
      {
        if (!args.UpdatedValue.Capabilities.HasFlag((Enum) Capability.HeartRate))
          return;
        ((System.Action) (() => this._mainPageView.ShowHeartRate((object) this.HeartRateTabViewModel))).OnUIThread();
      });
      this._bandSyncController.SyncState.Updated += new EventHandler<MonitorableUpdatedEventArgs<BandSyncState>>(this.BandControllerOnSyncStateChanged);
      this._bandSyncController.BatteryInfo.Updated += (EventHandler<MonitorableUpdatedEventArgs<BatteryInfo>>) ((sender, args) =>
      {
        this.NotifyOfPropertyChange<double>((Expression<Func<double>>) (() => this.BatteryValue));
        this.NotifyOfPropertyChange<string>((Expression<Func<string>>) (() => this.BatteryPercentString));
      });
      this._bandSyncController.LatestHeartRate.Updated += (EventHandler<MonitorableUpdatedEventArgs<HeartRateMeasurement>>) ((sender, args) => ((System.Action) (() => this.ActivitiesListViewModel.Refresh())).OnUIThread());
      this.ActivitiesListViewModel = new ActivitiesListViewModel(new Func<List<IDbUserActivity>>(this.GetActivityItems), 5, true, true);
      this._navigationService.BackPressed += new EventHandler<BackPressedEventArgs>(this.NavigationServiceOnBackPressed);
    }

    public WalkTabViewModel WalkTabViewModel { get; }

    public SleepTabViewModel SleepTabViewModel { get; }

    public HeartRateTabViewModel HeartRateTabViewModel { get; }

    public ActivitiesListViewModel ActivitiesListViewModel { get; }

    public MessagesViewModel MessagesViewModel { get; }

    public bool IsCommunicating => this._bandController.CommunicationOperation.Value != 0;

    public BandAnimation BandAnimationType
    {
      get
      {
        if (this.IsCommunicating)
          return BandAnimation.Communicating;
        return this._bandSyncController.BatteryInfo.Value != null && this._bandSyncController.BatteryInfo.Value.IsCharging ? BandAnimation.Charging : BandAnimation.None;
      }
    }

    public bool DevicePageHasWarning => false;

    public bool ShowConnectivityControlDots => this.IsCommunicating || this.IsBinded;

    public double BatteryValue
    {
      get
      {
        BatteryInfo batteryInfo = this._bandSyncController.BatteryInfo.Value;
        return batteryInfo == null ? 0.0 : (double) batteryInfo.ChargedPercent / 100.0;
      }
    }

    public string BatteryPercentString
    {
      get
      {
        BatteryInfo batteryInfo = this._bandSyncController.BatteryInfo.Value;
        return batteryInfo == null || !this.IsBinded ? "" : batteryInfo.ChargedPercent.ToString() + "%";
      }
    }

    public bool IsRefreshing
    {
      get
      {
        return this._bandController.CommunicationOperation.Value.HasFlag((Enum) CommunicationOperation.Refreshing) || this._isRefreshing;
      }
    }

    public bool IsPro => this._licensingService.IsPro;

    public bool IsBinded => this._bandController.BindingState.Value == BindingState.Binded;

    public UserInfo UserInfo => this._settings.GetSavedUserInfo();

    public BandSyncState SyncState => this._bandSyncController.SyncState.Value;

    public async void RefreshAll()
    {
      this._isRefreshing = true;
      this.NotifyOfPropertyChange<bool>((Expression<Func<bool>>) (() => this.IsRefreshing));
      try
      {
        await Task.Run((System.Action) (() =>
        {
          this._bandSyncController.Refresh();
          this.WalkTabViewModel.Refresh();
          this.SleepTabViewModel.Refresh();
          this.ActivitiesListViewModel.Refresh();
          this.MessagesViewModel.Refresh();
        }));
      }
      finally
      {
        this._isRefreshing = false;
        this.NotifyOfPropertyChange<bool>((Expression<Func<bool>>) (() => this.IsRefreshing));
      }
    }

    public async void OpenHistory()
    {
      if (!this._licensingService.IsPro)
      {
        MessageDialog dialog = new MessageDialog(this._stringsLoader.GetString("MainPageHistoryOnlyInProMessage"), this._stringsLoader.GetString("MessageInformationHeader"));
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        // ISSUE: method pointer
        dialog.Commands.Add((IUICommand) new UICommand(this._stringsLoader.GetString("MessageAnswerOk"), MainPageViewModel.\u003C\u003Ec.\u003C\u003E9__51_0 ?? (MainPageViewModel.\u003C\u003Ec.\u003C\u003E9__51_0 = new UICommandInvokedHandler((object) MainPageViewModel.\u003C\u003Ec.\u003C\u003E9, __methodptr(\u003COpenHistory\u003Eb__51_0)))));
        // ISSUE: method pointer
        dialog.Commands.Add((IUICommand) new UICommand(this._stringsLoader.GetString("MainPageBuyProAnswer"), new UICommandInvokedHandler((object) this, __methodptr(\u003COpenHistory\u003Eb__51_1))));
        dialog.put_CancelCommandIndex(0U);
        IUICommand iuiCommand = await dialog.ShowAsyncSafe();
      }
      else
      {
        StatusBarNotificationService notificationService = IoC.Get<StatusBarNotificationService>();
        StatusBarProgressItem pageLoadingMessage = new StatusBarProgressItem(this._stringsLoader.GetString("HistoryPageLoadingDataStatus"), new double?());
        StatusBarProgressItem statusBarItem = pageLoadingMessage;
        notificationService.Show<StatusBarProgressItem>(statusBarItem);
        Task.Delay(4500).ContinueWith((Action<Task>) (t => pageLoadingMessage.Hide())).ConfigureAwait(false);
        await Task.Delay(100).ConfigureAwait(true);
        this._navigationService.UriFor<HistoryPageViewModel>().Navigate();
      }
    }

    public void GoToSubscriptionPage()
    {
      this._navigationService.UriFor<SettingsPageViewModel>().WithParam<bool>((Expression<Func<SettingsPageViewModel, bool>>) (t => t.ShowSubscription), true).Navigate();
    }

    public void ManageDevice() => this._navigationService.UriFor<DevicePageViewModel>().Navigate();

    public void OpenUserInfo()
    {
      this._navigationService.UriFor<UserInfoPageViewModel>().Navigate();
    }

    public void OpenAlarms() => this._navigationService.UriFor<AlarmsPageViewModel>().Navigate();

    public void OpenAboutPage() => this._navigationService.UriFor<AboutPageViewModel>().Navigate();

    public void OpenSettingsPage()
    {
      this._navigationService.UriFor<SettingsPageViewModel>().Navigate();
    }

    public async void ShowSyncHelp()
    {
      IUICommand iuiCommand = await new MessageDialog(this._stringsLoader.GetString("MainPageSynchronizationErrorTip"), this._stringsLoader.GetString("MessageOopsHeader")).ShowAsync();
    }

    protected override async Task OnActivate()
    {
      if (await this.TryShowWhatsNew().ConfigureAwait(true))
        return;
      if (await this.NeedToUpdateUserInfo().ConfigureAwait(true))
        return;
      this.NotifyOfPropertyChange<bool>((Expression<Func<bool>>) (() => this.IsPro));
      this.RefreshAll();
      this.Refresh();
    }

    private async Task<bool> TryShowWhatsNew()
    {
      if (!this._updatesHistoryService.HasNotShowedUpdates)
        return false;
      Task task = await Task.Delay(100).ContinueWith<Task>((Func<Task, Task>) (t => ((System.Action) (() => this._navigationService.UriFor<WhatsNewPageViewModel>().Navigate())).OnUIThreadAsync())).ConfigureAwait(false);
      return true;
    }

    private async Task<bool> NeedToUpdateUserInfo()
    {
      this._settings.CreateDefaultUserInfo();
      if (!this._settings.UserInfoNeedsUpdate)
        return false;
      this._settings.UserInfoNeedsUpdate = false;
      ConfiguredTaskAwaitable configuredTaskAwaitable = await Task.Delay(1500).ContinueWith<ConfiguredTaskAwaitable>((Func<Task, ConfiguredTaskAwaitable>) (task => ((System.Action) (() => this._navigationService.UriFor<UserInfoPageViewModel>().WithParam<bool>((Expression<Func<UserInfoPageViewModel, bool>>) (t => t.IsInitiallyActivated), true).Navigate())).OnUIThreadAsync().ConfigureAwait(false))).ConfigureAwait(false);
      return true;
    }

    private void BandControllerOnStatusChanged(object sender, EventArgs eventArgs)
    {
      this.NotifyOfPropertyChange<bool>((Expression<Func<bool>>) (() => this.DevicePageHasWarning));
    }

    private void BindingStateOnUpdated(
      object sender,
      MonitorableUpdatedEventArgs<BindingState> updatedEventArgs)
    {
      this.NotifyOfPropertyChange<string>((Expression<Func<string>>) (() => this.BatteryPercentString));
      this.NotifyOfPropertyChange<bool>((Expression<Func<bool>>) (() => this.IsBinded));
    }

    private void BandControllerOnSyncStateChanged(object sender, EventArgs eventArgs)
    {
      this.NotifyOfPropertyChange<BandSyncState>((Expression<Func<BandSyncState>>) (() => this.SyncState));
      if (this._bandSyncController.SyncState.Value == BandSyncState.Binding)
      {
        ((System.Action) (() => this._navigationService.UriFor<PairingPageViewModel>().WithParam<bool>((Expression<Func<PairingPageViewModel, bool>>) (t => t.AlreadyStarted), true).Navigate())).OnUIThread();
      }
      else
      {
        if (this._bandSyncController.SyncState.Value != BandSyncState.Success)
          return;
        ((System.Action) (() => this.ActivitiesListViewModel.Refresh())).OnUIThread();
      }
    }

    private void CommunicationOperationOnUpdated(
      object sender,
      MonitorableUpdatedEventArgs<CommunicationOperation> updatedEventArgs)
    {
      this.NotifyOfPropertyChangeAsync<bool>((Expression<Func<bool>>) (() => this.IsRefreshing));
      this.NotifyOfPropertyChangeAsync<bool>((Expression<Func<bool>>) (() => this.IsCommunicating));
      this.NotifyOfPropertyChangeAsync<bool>((Expression<Func<bool>>) (() => this.ShowConnectivityControlDots));
      this.NotifyOfPropertyChangeAsync<BandAnimation>((Expression<Func<BandAnimation>>) (() => this.BandAnimationType));
    }

    private List<IDbUserActivity> GetActivityItems()
    {
      List<IDbUserActivity> list = this._activitiesDataBase.GetActivitiesInDay(DateTime.Now.Date).ToList<IDbUserActivity>();
      return this._licensingService.IsPro ? list : list.Where<IDbUserActivity>((Func<IDbUserActivity, bool>) (t => !(t is SleepingActivity))).ToList<IDbUserActivity>();
    }

    private void NavigationServiceOnBackPressed(
      object sender,
      BackPressedEventArgs backPressedEventArgs)
    {
      if (this._navigationService.CanGoBack)
        return;
      Application.Current.Exit();
    }

    public void AttachView(object view, object context = null)
    {
      this._mainPageView = (MainPage) view;
    }

    public object GetView(object context = null) => throw new NotImplementedException();

    public event EventHandler<ViewAttachedEventArgs> ViewAttached;
  }
}
