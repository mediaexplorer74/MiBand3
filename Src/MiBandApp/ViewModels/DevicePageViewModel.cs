
// Type: MiBandApp.ViewModels.DevicePageViewModel
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// 

using Caliburn.Micro;
using MiBand.SDK.Core;
using MiBandApp.Services;
using MiBandApp.Tools;
using MiBandApp.ViewModels.DeviceSettings;
using System;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.UI.Popups;

#nullable disable
namespace MiBandApp.ViewModels
{
  public class DevicePageViewModel : PageBaseViewModel
  {
    private readonly FirmwareViewModel _firmwareViewModel;
    private readonly BatteryViewModel _batteryViewModel;
    private readonly WearingLocationViewModel _wearingLocationViewModel;
    private readonly LedsColorViewModel _ledsColorViewModel;
    private readonly HeartRateDuringSleepViewModel _heartRateDuringSleepViewModel;
    private readonly HighlightOnWristLiftViewModel _highlightOnWristLiftViewModel;
    private readonly DisplayViewModel _displayViewModel;
    private readonly ActivityReminderViewModel _activityReminderViewModel;
    private readonly GoalReachedNotificationViewModel _goalReachedNotificationViewModel;
    private readonly NotDisturbViewModel _notDisturbViewModel;
    private readonly FlipDisplayOnWristRotateViewModel _flipDisplayOnWristRotateViewModel;
    private readonly INavigationService _navigationService;
    private readonly StatusBarNotificationService _statusBarNotificationService;
    private readonly BandSyncReminderService _bandSyncReminderService;
    private readonly BandController _bandController;
    private readonly ResourceLoader _stringsLoader;
    private bool _isPageEnabled;
    private bool _locateWarningShown;

    public DevicePageViewModel(
      FirmwareViewModel firmwareViewModel,
      BatteryViewModel batteryViewModel,
      WearingLocationViewModel wearingLocationViewModel,
      LedsColorViewModel ledsColorViewModel,
      HeartRateDuringSleepViewModel heartRateDuringSleepViewModel,
      HighlightOnWristLiftViewModel highlightOnWristLiftViewModel,
      DisplayViewModel displayViewModel,
      ActivityReminderViewModel activityReminderViewModel,
      GoalReachedNotificationViewModel goalReachedNotificationViewModel,
      NotDisturbViewModel notDisturbViewModel,
      FlipDisplayOnWristRotateViewModel flipDisplayOnWristRotateViewModel,
      INavigationService navigationService,
      StatusBarNotificationService statusBarNotificationService,
      BandSyncReminderService bandSyncReminderService,
      BandController bandController)
    {
      this._firmwareViewModel = firmwareViewModel;
      this._batteryViewModel = batteryViewModel;
      this._wearingLocationViewModel = wearingLocationViewModel;
      this._ledsColorViewModel = ledsColorViewModel;
      this._heartRateDuringSleepViewModel = heartRateDuringSleepViewModel;
      this._highlightOnWristLiftViewModel = highlightOnWristLiftViewModel;
      this._displayViewModel = displayViewModel;
      this._activityReminderViewModel = activityReminderViewModel;
      this._goalReachedNotificationViewModel = goalReachedNotificationViewModel;
      this._notDisturbViewModel = notDisturbViewModel;
      this._flipDisplayOnWristRotateViewModel = flipDisplayOnWristRotateViewModel;
      this._navigationService = navigationService;
      this._statusBarNotificationService = statusBarNotificationService;
      this._bandSyncReminderService = bandSyncReminderService;
      this._bandController = bandController;
      this._stringsLoader = new ResourceLoader();
    }

    public ObservableCollection<DeviceSettingViewModelBase> SettingItems { get; } 
            = new ObservableCollection<DeviceSettingViewModelBase>();

    public bool IsPageEnabled
    {
      get => this._isPageEnabled;
      set
      {
        if (this._isPageEnabled == value)
          return;
        this._isPageEnabled = value;
        this.NotifyOfPropertyChange(nameof (IsPageEnabled));
      }
    }

    public string DeviceName
    {
      get
      {
        if (this._bandController.DeviceInfo.Value != null)
        {
            switch (this._bandController.DeviceInfo.Value.Model)
            {
                case MiBandModel.MiBand1:
                    return "Mi Band 1.0";
                case MiBandModel.MiBand1A:
                    return "Mi Band 1A";
                case MiBandModel.MiBand1S:
                    return "Mi Band 1S";
                case MiBandModel.MiBand2:
                    return "Mi Band 2";
                default:
                    return "";
            }
        }

        // Temp / Debug
        return "Mi Band 3";
      }
    }

    public async void LocateBand()
    {
      if (!this._locateWarningShown)
      {
        IUICommand iuiCommand = await new MessageDialog(
            this._stringsLoader.GetString("DevicePageLocateWarning"), 
            this._stringsLoader.GetString("MessageInformationHeader")).ShowAsync();
        this._locateWarningShown = true;
      }

      this._statusBarNotificationService.Show<StatusBarMessage>(
          new StatusBarMessage(this._stringsLoader.GetString("DevicePageLocateDeviceMessage1"), 
          TimeSpan.FromSeconds(3.0)));

      this._statusBarNotificationService.Show<StatusBarMessage>(
          new StatusBarMessage(this._stringsLoader.GetString("DevicePageLocateDeviceMessage2"), 
          TimeSpan.FromSeconds(1.0)));
      bool error = false;
      try
      {
        await this._bandController.MiBand.Locate().ConfigureAwait(true);
      }
      catch (Exception ex)
      {
        error = true;
      }
      if (!error)
        return;
      this._statusBarNotificationService.Show<StatusBarMessage>(
          new StatusBarMessage(this._stringsLoader.GetString("MessageUndefinedError"), 
          TimeSpan.FromSeconds(1.0)));
    }

    public async void TryUnbind()
    {
        MessageDialog errorDialog = new MessageDialog(
          this._stringsLoader.GetString("DevicePageUnbindConfirmationMessage"),
          this._stringsLoader.GetString("MessageConfirmationHeader"));

        MessageDialog messageDialog = new MessageDialog(
          this._stringsLoader.GetString("DevicePageUnbindConfirmationMessage"),
          this._stringsLoader.GetString("MessageConfirmationHeader"));
    
            messageDialog.Commands.Add(new UICommand(
                this._stringsLoader.GetString("DevicePageUnbindYesAnswer"),
                command => this.DoUnbind()));

            messageDialog.Commands.Add(new UICommand(
                this._stringsLoader.GetString("DevicePageUnbindNoAnswer"),
                command => { /* No action needed for "No" */ }));

           
            messageDialog.Commands.Add(new UICommand(
                this._stringsLoader.GetString("DevicePage_RebootYesAnswer"),
                command => this.DoRebootDevice()));

            messageDialog.Commands.Add(new UICommand(
                this._stringsLoader.GetString("DevicePage_RebootNoAnswer"),
                command => { /* No action needed for "No" */ }));

            

            messageDialog.Commands.Add(new UICommand(
                this._stringsLoader.GetString("DevicePageUnbindNoAnswer"),
                command => { /* No action needed for "No" */ }));

           

            messageDialog.DefaultCommandIndex = 0;
            messageDialog.CancelCommandIndex = 1;

            // Fix for CS1002: ; expected
            // Ensure that all statements are properly terminated with a semicolon.

            IUICommand iuiCommand = await messageDialog.ShowAsync();

            // Fix for CS1513: } expected
            // Ensure that all blocks are properly closed with a closing brace.

            if (errorDialog != null)
            {
                await errorDialog.ShowAsyncSafe();
                this._navigationService.GoBack();
            }
           
            messageDialog.Commands.Add(new UICommand(
                this._stringsLoader.GetString("DevicePageUnbindYesAnswer"),
                command => this.DoUnbind()));

            messageDialog.Commands.Add(new UICommand(
                this._stringsLoader.GetString("DevicePageUnbindNoAnswer"),
                command => { /* No action needed for "No" */ }));

          
            messageDialog.Commands.Add(new UICommand(
                this._stringsLoader.GetString("DevicePage_RebootYesAnswer"),
                command => this.DoRebootDevice()));

            messageDialog.Commands.Add(new UICommand(
                this._stringsLoader.GetString("DevicePage_RebootNoAnswer"),
                command => { /* No action needed for "No" */ }));

          
            messageDialog.Commands.Add(new UICommand(
                this._stringsLoader.GetString("DevicePageUnbindNoAnswer"),
                command => { /* No action needed for "No" */ }));

           
            messageDialog.DefaultCommandIndex = 0;
            messageDialog.CancelCommandIndex = 1;

          
            IUICommand iuiCommand1 = await messageDialog.ShowAsync();

           

            if (errorDialog != null)
            {
                await errorDialog.ShowAsyncSafe();
                this._navigationService.GoBack();
            }

          messageDialog.CancelCommandIndex = (1U);
          messageDialog.DefaultCommandIndex = (1U);
          IUICommand iuiCommand2 = await messageDialog.ShowAsync();
        }

        public async void TryReboot()
        {
            if 
            (
                !this._bandController.DeviceInfo.Value.Capabilities.HasFlag((Enum)Capability.Reboot) 
                && this._bandController.DeviceInfo.Value.Model == MiBandModel.MiBand2
            )
            {
                IUICommand iuiCommand1 = await new MessageDialog(
                    this._stringsLoader.GetString("DevicePage_MiBand2RebootMessage"),
                    this._stringsLoader.GetString("MessageInformationHeader")).ShowAsync();
            }
            else
            {
                MessageDialog messageDialog = new MessageDialog(
                    this._stringsLoader.GetString("DevicePage_RebootConfirmationMessage"),
                    this._stringsLoader.GetString("MessageConfirmationHeader"));

                // Replace problematic method pointer with a lambda expression
                messageDialog.Commands.Add(new UICommand(
                    this._stringsLoader.GetString("DevicePage_RebootYesAnswer"),
                    command => this.DoRebootDevice()));

                // Replace problematic method pointer with a lambda expression
                messageDialog.Commands.Add(new UICommand(
                    this._stringsLoader.GetString("DevicePage_RebootNoAnswer"),
                    command => { /* No action needed for "No" */ }));

                messageDialog.CancelCommandIndex = 1;
                messageDialog.DefaultCommandIndex = 0;

                IUICommand iuiCommand2 = await messageDialog.ShowAsync();
            }
        }

    // OnActivate
    protected override async Task OnActivate()
    {
      this.SettingItems.Clear();
      this.SettingItems.Add((DeviceSettingViewModelBase) this._batteryViewModel);
      this.SettingItems.Add((DeviceSettingViewModelBase) this._firmwareViewModel);
      this.SettingItems.Add((DeviceSettingViewModelBase) this._wearingLocationViewModel);

      // RnD / Test
      if (this._bandController.DeviceInfo.Value != null)
      {
            if (this._bandController.DeviceInfo.Value.Capabilities.HasFlag((Enum)Capability.LedColors))
                this.SettingItems.Add((DeviceSettingViewModelBase)this._ledsColorViewModel);

            if (this._bandController.DeviceInfo.Value.Capabilities.HasFlag((Enum)Capability.DisplayItems))
                this.SettingItems.Add((DeviceSettingViewModelBase)this._displayViewModel);

            if (this._bandController.DeviceInfo.Value.Capabilities.HasFlag((Enum)Capability.ActivityReminder))
                this.SettingItems.Add((DeviceSettingViewModelBase)this._activityReminderViewModel);

            if (this._bandController.DeviceInfo.Value.Capabilities.HasFlag((Enum)Capability.NotDisturbMode))
                this.SettingItems.Add((DeviceSettingViewModelBase)this._notDisturbViewModel);

            if (this._bandController.DeviceInfo.Value.Capabilities.HasFlag((Enum)Capability.WristLiftHighlight))
                this.SettingItems.Add((DeviceSettingViewModelBase)this._highlightOnWristLiftViewModel);

            if (this._bandController.DeviceInfo.Value.Capabilities.HasFlag((Enum)Capability.FlipDisplayOnWristRotate))
                this.SettingItems.Add((DeviceSettingViewModelBase)this._flipDisplayOnWristRotateViewModel);

            if (this._bandController.DeviceInfo.Value.Capabilities.HasFlag((Enum)Capability.GoalReachedNotificationConfigurable))
                this.SettingItems.Add((DeviceSettingViewModelBase)this._goalReachedNotificationViewModel);

            if (this._bandController.DeviceInfo.Value.Capabilities.HasFlag((Enum)Capability.HeartRate))
                this.SettingItems.Add((DeviceSettingViewModelBase)this._heartRateDuringSleepViewModel);
      }

      await this.LoadAllData().ConfigureAwait(true);
    }//OnActivate


    protected override async Task OnDeactivate(bool close)
    {
      foreach (DeviceSettingViewModelBase settingItem in this.SettingItems)
        await settingItem.Save();
      this.IsPageEnabled = false;
    }

    private async Task LoadAllData()
    {
      StatusBarProgressItem statusBar = 
                this._statusBarNotificationService.Show<StatusBarProgressItem>
                (new StatusBarProgressItem(this._stringsLoader.GetString("StatusLoadingData"),
                new double?()));

      //MessageDialog errorDialog = (MessageDialog) null;
      //try
      //{
        this.IsPageEnabled = false;
        MessageDialog errorDialog1 = (MessageDialog)null;

        foreach (DeviceSettingViewModelBase settingItem in this.SettingItems)
        {                    
            try
            {
                await settingItem.Load();
            }
            catch (Exception ex1)
            {
                errorDialog1 = 
                    new MessageDialog(this._stringsLoader.GetString("DevicePageLoadingDataFailedMessage")
                    + " " + ex1.Message, this._stringsLoader.GetString("MessageOopsHeader"));
                
            }
       }
      //}
      //catch (Exception ex)
      //{
      //  errorDialog = new MessageDialog(this._stringsLoader.GetString("DevicePageLoadingDataFailedMessage") + " " + ex.Message, this._stringsLoader.GetString("MessageOopsHeader"));
      //}
      //finally
      //{
        this.IsPageEnabled = true;
      //}
      
      statusBar.Hide();
      
      //if (errorDialog1 == null)//(errorDialog == null)
      //   return;


        //IUICommand iuiCommand = await errorDialog1.ShowAsyncSafe(); //IUICommand iuiCommand = await errorDialog.ShowAsyncSafe();
        //this._navigationService.GoBack();
    }

    private async void DoUnbind()
    {
      this._bandController.Unbind();
      this._bandController.Reset();
      this._bandSyncReminderService.Cancel();

      IUICommand iuiCommand = await new MessageDialog(
          this._stringsLoader.GetString("DevicePageOnUnbindMessage"), 
          this._stringsLoader.GetString("MessageInformationHeader")).ShowAsync();
      this._navigationService.GoBack();
    }

    private async void DoRebootDevice()
    {
      StatusBarProgressItem progressItem = 
                this._statusBarNotificationService.Show<StatusBarProgressItem>(
                    new StatusBarProgressItem(this._stringsLoader.GetString("DevicePage_RebootingMessage"),
                    new double?()));
      this.IsPageEnabled = false;
      try
      {
        ConfiguredTaskAwaitable configuredTaskAwaitable
                    = this._bandController.MiBand.Reboot().ConfigureAwait(true);

        await configuredTaskAwaitable;
        this._bandController.Reset();
        configuredTaskAwaitable = Task.Delay(5000).ConfigureAwait(true);
        await configuredTaskAwaitable;
      }
      catch
      {
      }
      finally
      {
        progressItem.Hide();
        this._navigationService.GoBack();
      }
    }
  }
}
