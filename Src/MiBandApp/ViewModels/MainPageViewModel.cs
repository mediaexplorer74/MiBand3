
// Type: MiBandApp.ViewModels.MainPageViewModel
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// 

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
using System.Diagnostics;
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

      //StatusBar.GetForCurrentView().BackgroundOpacity = 1.0;

      this._bandController.StatusChanged 
                += new EventHandler(this.BandControllerOnStatusChanged);

      this._bandController.BindingState.Updated 
                += new EventHandler<MonitorableUpdatedEventArgs<BindingState>>(
                    this.BindingStateOnUpdated);

      this._bandController.CommunicationOperation.Updated 
                += new EventHandler<MonitorableUpdatedEventArgs<CommunicationOperation>>(
                    this.CommunicationOperationOnUpdated);
     
      this._bandController.DeviceInfo.Updated += 
         (EventHandler<MonitorableUpdatedEventArgs<BandDeviceInfo>>) ((sender, args) =>
      {
        if (!args.UpdatedValue.Capabilities.HasFlag((Enum) Capability.HeartRate))
          return;

        ((System.Action) (() => this._mainPageView.ShowHeartRate(this.HeartRateTabViewModel)))
          .OnUIThread();
      });

      this._bandSyncController.SyncState.Updated
                += new EventHandler<MonitorableUpdatedEventArgs<BandSyncState>>(
                    this.BandControllerOnSyncStateChanged);

      this._bandSyncController.BatteryInfo.Updated
                += (EventHandler<MonitorableUpdatedEventArgs<BatteryInfo>>) 
                ((sender, args) =>
      {
        this.NotifyOfPropertyChange<double>(() => this.BatteryValue);
        this.NotifyOfPropertyChange<string>(() => this.BatteryPercentString);
      });

      this._bandSyncController.LatestHeartRate.Updated +=
          ((sender, args) => ((System.Action) (() 
          => this.ActivitiesListViewModel.Refresh())).OnUIThread());
      this.ActivitiesListViewModel = new ActivitiesListViewModel(
          new Func<List<IDbUserActivity>>(this.GetActivityItems), 5, true, true);
      
      //TODO
      //this._navigationService.BackPressed
      //                   += new EventHandler<EventArgs>(this.NavigationServiceOnBackPressed);
    }

    public WalkTabViewModel WalkTabViewModel { get; }

    public SleepTabViewModel SleepTabViewModel { get; }

    public HeartRateTabViewModel HeartRateTabViewModel { get; }

    public ActivitiesListViewModel ActivitiesListViewModel { get; }

    public MessagesViewModel MessagesViewModel { get; }

    public bool IsCommunicating
    {
        get
        {
            return this._bandController.CommunicationOperation.Value != 0;
        }
    }

    public BandAnimation BandAnimationType
    {
      get
      {
        if (this.IsCommunicating)
          return BandAnimation.Communicating;

        return this._bandSyncController.BatteryInfo.Value != null 
                    && this._bandSyncController.BatteryInfo.Value.IsCharging 
                    ? BandAnimation.Charging
                    : BandAnimation.None;
      }
    }

    public bool DevicePageHasWarning => false;

        public bool ShowConnectivityControlDots
        {
            get
            {
                return this.IsCommunicating || this.IsBinded;
            }
        }

        public double BatteryValue
    {
      get
      {
        BatteryInfo batteryInfo = this._bandSyncController.BatteryInfo.Value;
        return batteryInfo == null 
                    ? 0.0 
                    : (double) batteryInfo.ChargedPercent / 100.0;
      }
    }

    public string BatteryPercentString
    {
      get
      {
        BatteryInfo batteryInfo = this._bandSyncController.BatteryInfo.Value;

        return batteryInfo == null || !this.IsBinded 
                    ? "" 
                    : batteryInfo.ChargedPercent.ToString() + "%";
      }
    }

    public bool IsRefreshing
    {
      get
      {
        return this._bandController.CommunicationOperation.Value.HasFlag(
            (Enum) CommunicationOperation.Refreshing) || this._isRefreshing;
      }
    }

        public bool IsPro
        {
            get
            {
                return this._licensingService.IsPro;
            }
        }

        public bool IsBinded
        {
            get
            {
                bool result = true;

                try
                {
                    // RnD / DEBUG / Test
                    result = true;
                      //this._bandController.BindingState.Value == BindingState.Binded;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("[ex] bandController.BindingState.Value error : " +
                        ex.Message);
                }

                return result;
            }
        }

        public UserInfo UserInfo
        {
            get
            {
                return this._settings.GetSavedUserInfo();
            }
        }

        public BandSyncState SyncState
        {
            get
            {
                return this._bandSyncController.SyncState.Value;
            }
        }

        public async void RefreshAll()
    {
      this._isRefreshing = true;
      this.NotifyOfPropertyChange<bool>( () => this.IsRefreshing);
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
        this.NotifyOfPropertyChange<bool> (() => this.IsRefreshing);
      }
    }

    // TODO: Replace all occurrences of UriFor<...>() with For<...>()
    // as per the updated API


    public async void OpenHistory()
    {
        if (!this._licensingService.IsPro)
        {
            MessageDialog dialog = new MessageDialog(
                this._stringsLoader.GetString("MainPageHistoryOnlyInProMessage"),
                this._stringsLoader.GetString("MessageInformationHeader")
            );

            dialog.Commands.Add(new UICommand(
                this._stringsLoader.GetString("MessageAnswerOk"),
                command => HandleOkCommand()
            ));

            dialog.Commands.Add(new UICommand(
                this._stringsLoader.GetString("MainPageBuyProAnswer"),
                command => HandleBuyProCommand()
            ));

            dialog.CancelCommandIndex = 0;
            await dialog.ShowAsync();
        }
        else
        {
            StatusBarNotificationService notificationService = 
                IoC.Get<StatusBarNotificationService>();
            StatusBarProgressItem pageLoadingMessage = new StatusBarProgressItem(
                this._stringsLoader.GetString("HistoryPageLoadingDataStatus"),
                new double?()
            );

            notificationService.Show<StatusBarProgressItem>(pageLoadingMessage);

            await Task.Delay(4500).ContinueWith(t => 
            pageLoadingMessage.Hide()).ConfigureAwait(false);
            await Task.Delay(100).ConfigureAwait(true);

              
            // Plan A - async
            var task = await Task.Delay(100).ContinueWith<Task>(
            (t => ((System.Action)(()
            =>
            this._navigationService.UriFor<HistoryPageViewModel>().Navigate()))
            .OnUIThreadAsync())).ConfigureAwait(false);

            // Plan B - not async
            //this._navigationService.UriFor<HistoryPageViewModel>().Navigate();
          }
        }

    public void GoToSubscriptionPage()
    {
        this._navigationService.UriFor<SettingsPageViewModel>()
            .WithParam<bool>(t => t.ShowSubscription, true).Navigate();
    }

    public void ManageDevice()
    {
        this._navigationService.UriFor<DevicePageViewModel>().Navigate();
    }

    public void OpenUserInfo()
    {
        this._navigationService.UriFor<UserInfoPageViewModel>().Navigate();
    }

    public void OpenAlarms()
    {
        //this._navigationService.UriFor<AlarmsPageViewModel>().Navigate();
       this._navigationService.For<AlarmsPageViewModel>().Navigate();
    }

    public void OpenAboutPage()
    {
        this._navigationService.UriFor<AboutPageViewModel>().Navigate();
    }

    public void OpenSettingsPage()
    {
        this._navigationService.UriFor<SettingsPageViewModel>().Navigate();
    }

    public async void ShowSyncHelp()
    {
        IUICommand iuiCommand = await new MessageDialog(
            this._stringsLoader.GetString("MainPageSynchronizationErrorTip"),
            this._stringsLoader.GetString("MessageOopsHeader")).ShowAsync();
    }

    protected override async Task OnActivate()
    {
        //if (await this.TryShowWhatsNew().ConfigureAwait(true))
        //  return;
        if (await this.NeedToUpdateUserInfo().ConfigureAwait(true))
        return;
        this.NotifyOfPropertyChange<bool>(() => this.IsPro);
        this.RefreshAll();
        this.Refresh();
    }

 

    private async Task<bool> TryShowWhatsNew()
    {
        if (!this._updatesHistoryService.HasNotShowedUpdates)
            return false;

        Task task = default;

        try
        {
            // Plan A - async
            task = await Task.Delay(100).ContinueWith<Task>(
            (t => ((System.Action)(()
            =>
            this._navigationService.UriFor<WhatsNewPageViewModel>().Navigate()))
            .OnUIThreadAsync())).ConfigureAwait(false);

            // Plan B - not async
            //this._navigationService.UriFor<WhatsNewPageViewModel>().Navigate();
        }
        catch (Exception ex)
        {
            Debug.WriteLine("[ex] MainPageViewModel bug: " + ex.Message);
        }

        return true;
    }

    private async Task<bool> NeedToUpdateUserInfo()
    {
        this._settings.CreateDefaultUserInfo();
        if (!this._settings.UserInfoNeedsUpdate)
            return false;
        this._settings.UserInfoNeedsUpdate = false;
        ConfiguredTaskAwaitable configuredTaskAwaitable = default;

        try
        {
            configuredTaskAwaitable 
                = await Task.Delay(1500).ContinueWith<ConfiguredTaskAwaitable>(
                (task => ((System.Action)(() =>
                    this._navigationService.UriFor<UserInfoPageViewModel>()
                    .WithParam<bool>((t => t.IsInitiallyActivated), true).Navigate()))
                    .OnUIThreadAsync()
                    .ConfigureAwait(false)))
                        .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Debug.WriteLine("[ex] MainPageViewModel - ConfiguredTask... bug: "
                + ex.Message);
            return default;
        }

        return true;
    }

private void BandControllerOnStatusChanged(object sender, EventArgs eventArgs)
{
    this.NotifyOfPropertyChange<bool>(() => this.DevicePageHasWarning);
}

private void BindingStateOnUpdated(
    object sender,
    MonitorableUpdatedEventArgs<BindingState> updatedEventArgs)
{
    this.NotifyOfPropertyChange<string>(() => this.BatteryPercentString);
    this.NotifyOfPropertyChange<bool>(() => this.IsBinded);
}

    private void BandControllerOnSyncStateChanged(object sender, EventArgs eventArgs)
    {
        this.NotifyOfPropertyChange<BandSyncState>(() => this.SyncState);
        if (this._bandSyncController.SyncState.Value == BandSyncState.Binding)
        {
            ((System.Action)(() => 
            this._navigationService.UriFor<PairingPageViewModel>().WithParam<bool>
            ((t => t.AlreadyStarted), true).Navigate())).OnUIThread(); 
            // Updated from UriFor to For
        }
        else
        {
            if (this._bandSyncController.SyncState.Value != BandSyncState.Success)
                return;
            ((System.Action)(() => this.ActivitiesListViewModel.Refresh())).OnUIThread();
        }
    }

    private void CommunicationOperationOnUpdated(
      object sender,
      MonitorableUpdatedEventArgs<CommunicationOperation> updatedEventArgs)
    {
      this.NotifyOfPropertyChangeAsync<bool>((() => this.IsRefreshing));
      this.NotifyOfPropertyChangeAsync<bool>( (() => this.IsCommunicating));
      this.NotifyOfPropertyChangeAsync<bool>((() => this.ShowConnectivityControlDots));
      this.NotifyOfPropertyChangeAsync<BandAnimation>((() => this.BandAnimationType));
    }

    private List<IDbUserActivity> GetActivityItems()
    {
      List<IDbUserActivity> list = 
                this._activitiesDataBase.GetActivitiesInDay(DateTime.Now.Date)
                .ToList<IDbUserActivity>();

      return this._licensingService.IsPro ? list : list.Where<IDbUserActivity>(
          (t => !(t is SleepingActivity))).ToList<IDbUserActivity>();
    }

    private void NavigationServiceOnBackPressed(
      object sender,
      EventArgs backPressedEventArgs)
    {
      if (this._navigationService.CanGoBack)
        return;
      Application.Current.Exit();
    }

    public void AttachView(object view, object context = null)
    {
      this._mainPageView = (MainPage) view;
    }

    public object GetView(object context = null)
    {
        //TODO
        return default;
    }

    // TODO: Ensure all identifiers are properly named and declared.

    private void HandleOkCommand()
    {
        // Logic for handling OK command
    }

    private void HandleBuyProCommand()
    {
        // Logic for handling Buy Pro command
    }

    public event EventHandler<ViewAttachedEventArgs> ViewAttached;
  }
}
