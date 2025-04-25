
// Type: MiBandApp.ViewModels.PairingPageViewModel
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// 

using Caliburn.Micro;
using MetroLog;
using MiBand.SDK.Configuration;
using MiBand.SDK.Core;
using MiBandApp.Services;
using MiBandApp.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Phone.UI.Input;
using Windows.UI.Popups;

#nullable disable
namespace MiBandApp.ViewModels
{
  public class PairingPageViewModel : PageBaseViewModel
  {
    private readonly INavigationService _navigationService;
    private readonly BandController _bandController;
    private readonly MiBandApp.Storage.Settings.Settings _settings;
    private readonly StatusBarNotificationService _statusBarNotificationService;
    private readonly ResourceLoader _stringsLoader;
    private readonly ILogger _log;
    private bool _hasPaired;
    private bool _hasErrorDisplayed;
    private StatusBarProgressItem _currentStatusBarProgressItem;

    public PairingPageViewModel(
      INavigationService navigationService,
      BandController bandController,
      MiBandApp.Storage.Settings.Settings settings,
      StatusBarNotificationService statusBarNotificationService,
      ILogManager logManager)
    {
      this._navigationService = navigationService;
      this._bandController = bandController;
      this._settings = settings;
      this._statusBarNotificationService = statusBarNotificationService;
      this._log = logManager.GetLogger<PairingPageViewModel>();
      this._stringsLoader = new ResourceLoader();
    }

    public bool HasPaired
    {
      get => this._hasPaired;
      set
      {
        if (value == this._hasPaired)
          return;
        this._hasPaired = value;
        this.NotifyOfPropertyChange(nameof (HasPaired));
      }
    }

    public bool HasErrorDisplayed
    {
      get => this._hasErrorDisplayed;
      set
      {
        if (value == this._hasErrorDisplayed)
          return;
        this._hasErrorDisplayed = value;
        this.NotifyOfPropertyChange(nameof (HasErrorDisplayed));
      }
    }

    public bool AlreadyStarted { get; set; }

    protected override async Task OnActivate()
    {
      //this._navigationService.BackPressed += new EventHandler<EventArgs>(this.NavigationServiceOnBackPressed);
      this.Pair(this.AlreadyStarted);
    }

    protected override async Task OnDeactivate(bool close)
    {
      //this._navigationService.BackPressed -= new EventHandler<EventArgs>(this.NavigationServiceOnBackPressed);
      this._currentStatusBarProgressItem?.Hide();
    }

    private async void Pair(bool alreadyStarted)
    {
      this._log.Info("Starting binding", (Exception) null);
      MessageDialog errorDialog = (MessageDialog) null;
      this._currentStatusBarProgressItem 
                = this._statusBarNotificationService.Show<StatusBarProgressItem>(new StatusBarProgressItem(this._stringsLoader.GetString("PairingPageBindingMessage"), new double?()));
      do
      {
        try
        {
          BindingResult result = await this._bandController.MiBand.Bind(alreadyStarted
              ? (UserInfo) null
              : this._settings.GetSavedUserInfo()).ConfigureAwait(true);

          switch (result)
          {
            case BindingResult.Success:
              goto label_12;
            case BindingResult.Fail:
              this.HasErrorDisplayed = true;
              if (await this.FailPairingTryAgain().ConfigureAwait(true))
              {
                this.HasErrorDisplayed = false;
                await Task.Delay(5000).ConfigureAwait(true);
                continue;
              }
              goto label_12;
          }
        }
        catch (Exception ex)
        {
          this._log.Error(ex.ToString(), (Exception) null);
          errorDialog = new MessageDialog(this._stringsLoader.GetString("PairingPageInitializeErrorMessage"), this._stringsLoader.GetString("MessageOopsHeader"));
        }
      }
      while (errorDialog == null);
      this.HasErrorDisplayed = true;
      IUICommand iuiCommand = await errorDialog.ShowAsyncSafe();
      this.HasErrorDisplayed = false;
      this._navigationService.GoBack();
      return;
label_12:
      await this.CompletePairing();
    }

    private async Task<bool> FailPairingTryAgain()
    {
      MessageDialog dialog = new MessageDialog(this._stringsLoader.GetString("PairingPageBindingFailedTryAgainMessage"), this._stringsLoader.GetString("MessageOopsHeader"));
      IList<IUICommand> commands1 = dialog.Commands;
      UICommand uiCommand1 = new UICommand(this._stringsLoader.GetString("PairingPageTryAgainOption"));
      uiCommand1.Id = (object) 1;
      commands1.Add((IUICommand) uiCommand1);
      IList<IUICommand> commands2 = dialog.Commands;
      UICommand uiCommand2 = new UICommand(this._stringsLoader.GetString("PairingPageIgnoreOption"));
      uiCommand2.Id = (object) 2;
      commands2.Add((IUICommand) uiCommand2);
      dialog.CancelCommandIndex = 1U;

      IUICommand iuiCommand = await dialog.ShowAsyncSafe();
      return iuiCommand != null && (int) iuiCommand.Id == 1;
    }

    private async Task CompletePairing()
    {
      this.HasPaired = true;
      await this.InitDevice().ConfigureAwait(false);
      this._bandController.Bind();
      await Task.Delay(1000).ConfigureAwait(false);
      ((System.Action) (() =>
      {
        this._currentStatusBarProgressItem.Hide();
        this._navigationService.GoBack();
      })).OnUIThread();
    }

    private async Task InitDevice()
    {
      this._currentStatusBarProgressItem = this._statusBarNotificationService.Show<StatusBarProgressItem>(new StatusBarProgressItem(this._stringsLoader.GetString("PairingPageInitializingDeviceMessage"), new double?()));
      try
      {
        await this._bandController.MiBand.SetUserInfo(this._settings.GetSavedUserInfo(), false).ConfigureAwait(true);
        await this._bandController.MiBand.SetStepsGoal(this._settings.GetSavedGoalInfo().StepsGoal).ConfigureAwait(true);
        List<Alarm> alarms = this._settings.Alarms.GetAllSaved().ToList<Alarm>();
        for (int i = 0; i < this._bandController.DeviceInfo.Value.AlarmsCount; ++i)
        {
          if (i < alarms.Count)
            await this._bandController.MiBand.SetAlarm(i, alarms[i]).ConfigureAwait(true);
          else
            await this._bandController.MiBand.SetAlarm(i, new Alarm()
            {
              IsEnabled = false
            }).ConfigureAwait(true);
        }
        ConfiguredTaskAwaitable configuredTaskAwaitable;
        if (this._bandController.DeviceInfo.Value.Capabilities.HasFlag((Enum) Capability.HeartRate))
        {
          configuredTaskAwaitable = this._bandController.MiBand.SetHeartRateDuringSleep(this._settings.HeartRateDuringSleepEnabled).ConfigureAwait(true);
          await configuredTaskAwaitable;
        }
        if (this._bandController.DeviceInfo.Value.Capabilities.HasFlag((Enum) Capability.ActivityReminder))
        {
          configuredTaskAwaitable = this._bandController.MiBand.SetActivityReminder(new ActivityReminderConfig()
          {
            IsEnabled = this._settings.ActivityReminderEnabled,
            StartTime = this._settings.ActivityReminderStart,
            EndTime = this._settings.ActivityReminderEnd
          }).ConfigureAwait(true);
          await configuredTaskAwaitable;
        }
        if (this._bandController.DeviceInfo.Value.Capabilities.HasFlag((Enum) Capability.DisplayItems))
        {
          configuredTaskAwaitable = this._bandController.MiBand.SetDisplayItems(this._settings.DisplayItemsConfig).ConfigureAwait(true);
          await configuredTaskAwaitable;
        }
        if (this._bandController.DeviceInfo.Value.Capabilities.HasFlag((Enum) Capability.WristLiftHighlight))
        {
          configuredTaskAwaitable = this._bandController.MiBand.SetHighlightOnWristLift(this._settings.HighlightOnWristLiftEnabled).ConfigureAwait(true);
          await configuredTaskAwaitable;
        }
        if (this._bandController.DeviceInfo.Value.Capabilities.HasFlag((Enum) Capability.FlipDisplayOnWristRotate))
        {
          configuredTaskAwaitable = this._bandController.MiBand.SetFlipDisplayOnWristRotate(this._settings.FlipDisplayOnWristRotateEnabled).ConfigureAwait(true);
          await configuredTaskAwaitable;
        }
        if (this._bandController.DeviceInfo.Value.Capabilities.HasFlag((Enum) Capability.DateDisplay))
        {
          configuredTaskAwaitable = this._bandController.MiBand.SetDateDisplayMode(this._settings.DisplayDateEnabled ? CultureHelper.GetDateDisplayMode() : DateDisplayMode.None).ConfigureAwait(true);
          await configuredTaskAwaitable;
        }
        if (this._bandController.DeviceInfo.Value.Capabilities.HasFlag((Enum) Capability.GoalReachedNotificationConfigurable))
        {
          configuredTaskAwaitable = this._bandController.MiBand.SetGoalReachedNotification(this._settings.GoalReachedNotificationEnabled).ConfigureAwait(true);
          await configuredTaskAwaitable;
        }
        if (this._bandController.DeviceInfo.Value.Capabilities.HasFlag((Enum) Capability.NotDisturbMode))
        {
          configuredTaskAwaitable = this._bandController.MiBand.SetNotDisturbConfig(new NotDisturbConfig()
          {
            IsEnabled = this._settings.NotDisturbEnabled,
            IsSmart = this._settings.NotDisturbSmart,
            AllowHighlightOnWristLift = this._settings.NotDisturbAllowHighlightOnWristLift,
            StartTime = this._settings.NotDisturbStartTime,
            EndTime = this._settings.NotDisturbEndTime
          }).ConfigureAwait(true);
          await configuredTaskAwaitable;
        }
        alarms = (List<Alarm>) null;
      }
      catch (Exception ex)
      {
        this._log.Error("Exception while initializing device: " + (object) ex, (Exception) null);
      }
    }

    private void NavigationServiceOnBackPressed(
      object sender,
      EventArgs backPressedEventArgs)
    {
      //backPressedEventArgs.Handled = true;
    }
  }
}
