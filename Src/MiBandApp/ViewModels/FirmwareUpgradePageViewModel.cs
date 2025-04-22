// Decompiled with JetBrains decompiler
// Type: MiBandApp.ViewModels.FirmwareUpgradePageViewModel
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.exe

using Caliburn.Micro;
using MetroLog;
using MiBand.SDK.FirmwareUpgrade;
using MiBandApp.Services;
using MiBandApp.Tools;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Phone.UI.Input;
using Windows.System.Display;

#nullable disable
namespace MiBandApp.ViewModels
{
  public class FirmwareUpgradePageViewModel : PageBaseViewModel, IFirmwareUpgradeProgress
  {
    private readonly BandController _bandController;
    private readonly INavigationService _navigationService;
    private readonly ResourceLoader _resourceLoader = new ResourceLoader();
    private readonly DisplayRequest _displayRequest = new DisplayRequest();
    private readonly ILogger _log;
    private bool _hasUpgradeStarted;
    private string _actionTitle;
    private string _actionDetails;
    private int _progressPercent;
    private bool _isProgressIndeterminate;
    private bool _isExitButtonVisible;
    private int _firmwaresToUpdate;
    private int _firmwareUpdatedIndex;

    public FirmwareUpgradePageViewModel(
      BandController bandController,
      INavigationService navigationService,
      ILogManager logManager)
    {
      this._bandController = bandController;
      this._navigationService = navigationService;
      this._log = logManager.GetLogger<FirmwareUpgradePageViewModel>();
    }

    public Firmware Parameter { get; set; }

    public bool HasUpgradeStarted
    {
      get => this._hasUpgradeStarted;
      set
      {
        if (this._hasUpgradeStarted == value)
          return;
        this._hasUpgradeStarted = value;
        this.NotifyOfPropertyChange(nameof (HasUpgradeStarted));
      }
    }

    public string ActionTitle
    {
      get => this._actionTitle;
      set
      {
        if (this._actionTitle == value)
          return;
        this._actionTitle = value;
        this.NotifyOfPropertyChange(nameof (ActionTitle));
      }
    }

    public string ActionDetails
    {
      get => this._actionDetails;
      set
      {
        if (this._actionDetails == value)
          return;
        this._actionDetails = value;
        this.NotifyOfPropertyChange(nameof (ActionDetails));
      }
    }

    public int ProgressPercent
    {
      get => this._progressPercent;
      set
      {
        if (this._progressPercent == value)
          return;
        this._progressPercent = value;
        this.NotifyOfPropertyChange(nameof (ProgressPercent));
      }
    }

    public bool IsProgressIndeterminate
    {
      get => this._isProgressIndeterminate;
      set
      {
        if (this._isProgressIndeterminate == value)
          return;
        this._isProgressIndeterminate = value;
        this.NotifyOfPropertyChange(nameof (IsProgressIndeterminate));
      }
    }

    public bool IsExitButtonVisible
    {
      get => this._isExitButtonVisible;
      set
      {
        if (this._isExitButtonVisible == value)
          return;
        this._isExitButtonVisible = value;
        this.NotifyOfPropertyChange(nameof (IsExitButtonVisible));
      }
    }

    public async void StartUpgrade()
    {
      this.HasUpgradeStarted = true;
      this.IsExitButtonVisible = false;
      try
      {
        if (!await this._bandController.MiBand.UpgradeFirmware(this.Parameter, (IFirmwareUpgradeProgress) this).ConfigureAwait(true))
          return;
        this.FinishSuccessfullUpgrade();
      }
      catch
      {
        this._navigationService.GoBack();
      }
    }

    private void FinishSuccessfullUpgrade()
    {
      while (this._navigationService.BackStack.Count > 1)
        this._navigationService.BackStack.RemoveAt(this._navigationService.BackStack.Count - 1);
      this.ActionTitle = this._resourceLoader.GetString("FirmwareUpgradePage_ActionComplete");
      this.ActionDetails = this._resourceLoader.GetString("FirmwareUpgradePage_ActionCompleteDetails");
      this.IsProgressIndeterminate = false;
      this.IsExitButtonVisible = true;
    }

    public void Exit() => this._navigationService.GoBack();

    protected override Task OnActivate()
    {
      this._navigationService.BackPressed += new EventHandler<BackPressedEventArgs>(this.NavigationServiceOnBackPressed);
      this._displayRequest.RequestActive();
      return base.OnActivate();
    }

    protected override Task OnDeactivate(bool close = true)
    {
      this._navigationService.BackPressed -= new EventHandler<BackPressedEventArgs>(this.NavigationServiceOnBackPressed);
      this._displayRequest.RequestRelease();
      return base.OnDeactivate(close);
    }

    private void NavigationServiceOnBackPressed(
      object sender,
      BackPressedEventArgs backPressedEventArgs)
    {
      if (this.IsExitButtonVisible || !this.HasUpgradeStarted)
        return;
      backPressedEventArgs.put_Handled(true);
    }

    public void ReportUpload(int percents) => this.ProgressPercent = percents;

    public void ReportState(FirmwareUpgradeState firmwareUpgradeState)
    {
      this._log.Debug("Firmware upgrade state: " + (object) firmwareUpgradeState, (Exception) null);
      switch (firmwareUpgradeState)
      {
        case FirmwareUpgradeState.Initializing:
          this.IsProgressIndeterminate = true;
          this.ActionTitle = this._resourceLoader.GetString("FirmwareUpgradePage_ActionInitializing");
          this.ActionDetails = this._resourceLoader.GetString("FirmwareUpgradePage_ActionInitializingDetails");
          this.TryAppendCountInformationToActionTitle();
          break;
        case FirmwareUpgradeState.TransmittingData:
          this.IsProgressIndeterminate = false;
          this.ActionTitle = this._resourceLoader.GetString("FirmwareUpgradePage_ActionTransmittingData");
          this.ActionDetails = this._resourceLoader.GetString("FirmwareUpgradePage_ActionTransmittingDataDetails");
          this.TryAppendCountInformationToActionTitle();
          break;
        case FirmwareUpgradeState.Validating:
          this.ActionTitle = this._resourceLoader.GetString("FirmwareUpgradePage_ActionValidation");
          this.ActionDetails = this._resourceLoader.GetString("FirmwareUpgradePage_ActionValidationDetails");
          this.IsProgressIndeterminate = true;
          this.TryAppendCountInformationToActionTitle();
          break;
        case FirmwareUpgradeState.Complete:
          break;
        case FirmwareUpgradeState.Failed:
          this.ActionTitle = this._resourceLoader.GetString("FirmwareUpgradePage_ActionFailed");
          this.IsProgressIndeterminate = false;
          this.IsExitButtonVisible = true;
          this.ProgressPercent = 0;
          break;
        case FirmwareUpgradeState.Rebooting:
          this.ActionTitle = this._resourceLoader.GetString("FirmwareUpgradePage_ActionSuccess");
          this.ActionDetails = this._resourceLoader.GetString("FirmwareUpgradePage_ActionSuccessDetails");
          this.IsProgressIndeterminate = true;
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof (firmwareUpgradeState), (object) firmwareUpgradeState, (string) null);
      }
    }

    public void ReportError(FirmwareUpgradeError firmwareUpgradeError)
    {
      switch (firmwareUpgradeError)
      {
        case FirmwareUpgradeError.TimedOut:
          this.ActionDetails = this._resourceLoader.GetString("FirmwareUpgradePage_FailTimedOut");
          break;
        case FirmwareUpgradeError.InitialCheckFailed:
          this.ActionDetails = this._resourceLoader.GetString("FirmwareUpgradePage_FailInitialCheck");
          break;
        case FirmwareUpgradeError.PostCheckFailed:
          this.ActionDetails = this._resourceLoader.GetString("FirmwareUpgradePage_FailPostCheck");
          break;
        case FirmwareUpgradeError.UnknownError:
          this.ActionDetails = this._resourceLoader.GetString("FirmwareUpgradePage_FailUnknown");
          break;
        case FirmwareUpgradeError.CommunicationError:
          this.ActionDetails = this._resourceLoader.GetString("FirmwareUpgradePage_FailCommunication");
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof (firmwareUpgradeError), (object) firmwareUpgradeError, (string) null);
      }
    }

    public void ReportFirmwareCount(int count, int currentIndex)
    {
      this._firmwaresToUpdate = count;
      this._firmwareUpdatedIndex = currentIndex;
    }

    private void TryAppendCountInformationToActionTitle()
    {
      if (this._firmwaresToUpdate <= 1)
        return;
      this.ActionTitle = this.ActionTitle + " (" + (object) (this._firmwareUpdatedIndex + 1) + "/" + (object) this._firmwaresToUpdate + ")";
    }
  }
}
