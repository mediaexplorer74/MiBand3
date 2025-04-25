
// Type: MiBandApp.ViewModels.CloudViewModel
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// 

using Caliburn.Micro;
using MiBandApp.Services;
using System;
using System.Linq.Expressions;
using Windows.ApplicationModel.Resources;
using Windows.UI.Popups;

#nullable disable
namespace MiBandApp.ViewModels
{
  public class CloudViewModel : PropertyChangedBase
  {
    private readonly MiBandApp.Storage.Settings.Settings _settings;
    private readonly OneDriveSyncService _oneDriveSyncService;
    private readonly OneDriveSessionService _oneDriveSessionService;
    private readonly LicensingService _licensingService;
    private readonly ResourceLoader _stringsLoader;
    private bool _showOneDriveLoginButton;
    private bool _showOneDriveProgressRing;
    private bool _showOneDriveListItem;

    public CloudViewModel(
      MiBandApp.Storage.Settings.Settings settings,
      OneDriveSyncService oneDriveSyncService,
      OneDriveSessionService oneDriveSessionService,
      LicensingService licensingService)
    {
      this._stringsLoader = new ResourceLoader();
      this._settings = settings;
      this._oneDriveSyncService = oneDriveSyncService;
      this._oneDriveSyncService.StateChanged
                += new EventHandler(this.OneDriveSyncServiceOnStateChanged);

      this._oneDriveSessionService = oneDriveSessionService;
      this._licensingService = licensingService;

      this._oneDriveSessionService.StatusChanged 
                += new EventHandler(this.OneDriveSessionServiceOnStatusChanged);

      this._oneDriveSessionService.Init().ConfigureAwait(false);
      this.UpdateVisibilityOneDrive();
    }

    public bool ShowOneDriveLoginButton
    {
      get => this._showOneDriveLoginButton;
      set
      {
        if (this._showOneDriveLoginButton == value)
          return;
        this._showOneDriveLoginButton = value;
        this.NotifyOfPropertyChange(nameof (ShowOneDriveLoginButton));
      }
    }

    public bool ShowOneDriveProgressRing
    {
      get => this._showOneDriveProgressRing;
      set
      {
        if (this._showOneDriveProgressRing == value)
          return;
        this._showOneDriveProgressRing = value;
        this.NotifyOfPropertyChange(nameof (ShowOneDriveProgressRing));
      }
    }

    public bool ShowOneDriveListItem
    {
      get => this._showOneDriveListItem;
      set
      {
        if (this._showOneDriveListItem == value)
          return;
        this._showOneDriveListItem = value;
        this.NotifyOfPropertyChange(nameof (ShowOneDriveListItem));
      }
    }

    public bool OneDriveManipulationEnabled
    {
      get
      {
        return this._oneDriveSyncService.State == OneDriveSyncService.SyncState.Error 
                    || this._oneDriveSyncService.State == OneDriveSyncService.SyncState.NotSynced 
                    || this._oneDriveSyncService.State == OneDriveSyncService.SyncState.Synced;
      }
    }

    public string OneDriveSyncStateText
    {
      get
      {
        DateTime lastSyncTime = this._oneDriveSyncService.LastSyncTime;
        switch (this._oneDriveSyncService.State)
        {
          case OneDriveSyncService.SyncState.NotSynced:
            return lastSyncTime == DateTime.MinValue 
                            ? this._stringsLoader.GetString("CloudPageOneDriveNotSynchronized") 
                            : this.GetSyncedAtString(lastSyncTime);
          case OneDriveSyncService.SyncState.Synced:
            return this.GetSyncedAtString(lastSyncTime);
          case OneDriveSyncService.SyncState.Connecting:
            return this._stringsLoader.GetString("CloudPageOneDriveConnecting");
          case OneDriveSyncService.SyncState.Importing:
            return this._stringsLoader.GetString("CloudPageOneDriveImporting");
          case OneDriveSyncService.SyncState.Exporting:
            return this._stringsLoader.GetString("CloudPageOneDriveExporting");
          case OneDriveSyncService.SyncState.Error:
            return this._stringsLoader.GetString("CloudPageOneDriveFailed");
          default:
            return this._oneDriveSyncService.State.ToString();
        }
      }
    }

    public bool HasNoConnection
    {
      get
      {
        return this._oneDriveSessionService.Status == OneDriveSessionService.OneDriveStatus.NoConnection;
      }
    }

    public void DisableOneDrive()
    {
      this._settings.OneDriveSyncEnabled = false;
      this.UpdateVisibilityOneDrive();
    }

        public async void LoginOneDrive()
        {
            if (!this._licensingService.IsPro)
            {
                MessageDialog messageDialog = new MessageDialog(
                    this._stringsLoader.GetString("OneDriveOnlyInProMessage"),
                    this._stringsLoader.GetString("MessageInformationHeader")
                );

                messageDialog.Commands.Add(new UICommand(
                    this._stringsLoader.GetString("MessageAnswerOk"),
                    command => { /* No-op or additional logic here */ }
                ));

                messageDialog.CancelCommandIndex = 0;
                await messageDialog.ShowAsync();
            }
            else
            {
                await this._oneDriveSessionService.ShowLogInUi().ConfigureAwait(true);
                if (this._oneDriveSessionService.Status != OneDriveSessionService.OneDriveStatus.Authorized)
                    return;

                this._settings.OneDriveSyncEnabled = true;
                this.UpdateVisibilityOneDrive();
            }
        }

    public void OneDriveSyncNow() => this._oneDriveSyncService.Sync().ConfigureAwait(false);

    private void OneDriveSessionServiceOnStatusChanged(object sender, EventArgs eventArgs)
    {
      this.UpdateVisibilityOneDrive();
    }

    private void OneDriveSyncServiceOnStateChanged(object sender, EventArgs eventArgs)
    {
      this.NotifyOfPropertyChange<string>((Expression<Func<string>>) (() => this.OneDriveSyncStateText));
      this.NotifyOfPropertyChange<bool>((Expression<Func<bool>>) (() => this.OneDriveManipulationEnabled));
      this.NotifyOfPropertyChange<bool>((Expression<Func<bool>>) (() => this.HasNoConnection));
    }

    private string GetSyncedAtString(DateTime syncTime)
    {
      string str1 = syncTime.ToString("t");
      string str2 = syncTime.ToString("M");
      return this._stringsLoader.GetString("CloudPageOneDriveSyncedAt") 
                + " " + str1 + (syncTime.Date == DateTime.Today ? "" : ", " + str2);
    }

    private void UpdateVisibilityOneDrive()
    {
      this.ShowOneDriveProgressRing = this._oneDriveSessionService.Status == OneDriveSessionService.OneDriveStatus.Connecting;
      this.ShowOneDriveLoginButton = !this.ShowOneDriveProgressRing && (!this._settings.OneDriveSyncEnabled || this._oneDriveSessionService.Status == OneDriveSessionService.OneDriveStatus.NeedsLogin || !this._licensingService.IsPro);
      this.ShowOneDriveListItem = !this.ShowOneDriveLoginButton && !this.ShowOneDriveProgressRing;
      this.NotifyOfPropertyChange<bool>((Expression<Func<bool>>) (() => this.HasNoConnection));
    }
  }
}
