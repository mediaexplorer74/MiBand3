
// Type: MiBandApp.Services.OneDriveSessionService
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// 

using MetroLog;
using Microsoft.Live;
using System;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

#nullable disable
namespace MiBandApp.Services
{
  public class OneDriveSessionService
  {
    private readonly string[] _oneDriveScopes = new string[2]
    {
      "wl.signin",
      "wl.skydrive_update"
    };
    private readonly ILogger _logger;
    private OneDriveSessionService.OneDriveStatus _status;

    public OneDriveSessionService(ILogManager logManager)
    {
      this._logger = logManager.GetLogger<OneDriveSessionService>();
    }

    public OneDriveSessionService.OneDriveStatus Status
    {
      get => this._status;
      set
      {
        if (this._status == value)
          return;
        this._status = value;
        this.StatusChanged((object) this, EventArgs.Empty);
      }
    }

    public LiveConnectSession Session { get; private set; }

    public event EventHandler StatusChanged = (sender, args) => { };

    public async Task Init()
    {
      if (this.Status == OneDriveSessionService.OneDriveStatus.Connecting || this.Status == OneDriveSessionService.OneDriveStatus.Authorized)
        return;
      this.Status = OneDriveSessionService.OneDriveStatus.Connecting;
      try
      {
        LiveLoginResult liveLoginResult = await new LiveAuthClient().InitializeAsync(this._oneDriveScopes).ConfigureAwait(false);
        if (liveLoginResult.Status == LiveConnectSessionStatus.Connected)
        {
          this.Status = OneDriveSessionService.OneDriveStatus.Authorized;
          this.Session = liveLoginResult.Session;
          return;
        }
        this.Status = OneDriveSessionService.OneDriveStatus.NeedsLogin;
        return;
      }
      catch (Exception ex)
      {
        this._logger.Error("Excpetion when silently logging in to OneDrive: " + (object) ex, (Exception) null);
      }
      this.Status = NetworkInterface.GetIsNetworkAvailable() ? OneDriveSessionService.OneDriveStatus.NeedsLogin : OneDriveSessionService.OneDriveStatus.NoConnection;
    }

    public async Task ShowLogInUi()
    {
      try
      {
        this.Status = OneDriveSessionService.OneDriveStatus.Connecting;
        LiveLoginResult liveLoginResult = await new LiveAuthClient().LoginAsync(this._oneDriveScopes).ConfigureAwait(false);
        this.Status = liveLoginResult.Status == LiveConnectSessionStatus.Connected ? OneDriveSessionService.OneDriveStatus.Authorized : OneDriveSessionService.OneDriveStatus.NeedsLogin;
        this.Session = liveLoginResult.Session;
      }
      catch (Exception ex)
      {
        this._logger.Error("Exception when showing log in UI: " + (object) ex, (Exception) null);
        this.Status = OneDriveSessionService.OneDriveStatus.NoConnection;
      }
    }

    public enum OneDriveStatus
    {
      None,
      Connecting,
      NeedsLogin,
      NoConnection,
      Authorized,
    }
  }
}
