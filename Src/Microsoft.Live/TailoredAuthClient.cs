
// Type: Microsoft.Live.TailoredAuthClient
// Assembly: Microsoft.Live, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 84D9FE74-951C-4BB4-80F8-EDD369EAC36D
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\Microsoft.Live.dll

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Security.Authentication.OnlineId;

#nullable disable
namespace Microsoft.Live
{
  internal class TailoredAuthClient : IAuthClient
  {
    public const string Win8ReturnUriScheme = "ms-app";
    private const int UserNotFoundLoginExceptionHResult = -2147023579;
    private const int ConsentNotGrantedExceptionHResult = -2138701812;
    private const int InvalidClientExceptionHResult = -2138701821;
    private const int InvalidAuthTargetExceptionHResult = -2138701823;
    private readonly LiveAuthClient authClient;
    private readonly OnlineIdAuthenticator authenticator;

    public TailoredAuthClient(LiveAuthClient authClient)
    {
      this.authClient = authClient;
      this.authenticator = new OnlineIdAuthenticator();
    }

    public bool CanSignOut => this.authenticator.CanSignOut;

    public async Task<LiveLoginResult> AuthenticateAsync(string scopes, bool silent)
    {
      Exception error = (Exception) null;
      string accessToken = (string) null;
      string authenticationToken = (string) null;
      LiveLoginResult result = (LiveLoginResult) null;
      try
      {
        accessToken = await this.GetAccessToken(scopes, silent);
        LiveConnectSession session = new LiveConnectSession(this.authClient);
        session.AccessToken = accessToken;
        if (!string.IsNullOrEmpty(this.authClient.RedirectUrl) && !this.authClient.RedirectUrl.Equals("ms-app", StringComparison.OrdinalIgnoreCase))
        {
          authenticationToken = await this.GetAuthenticationToken(this.authClient.RedirectUrl, silent);
          session.AuthenticationToken = authenticationToken;
        }
        result = new LiveLoginResult(LiveConnectSessionStatus.Connected, session);
      }
      catch (TaskCanceledException ex)
      {
        result = new LiveLoginResult(LiveConnectSessionStatus.NotConnected, (LiveConnectSession) null);
      }
      catch (Exception ex)
      {
        switch (ex.HResult)
        {
          case -2147023579:
            result = new LiveLoginResult(LiveConnectSessionStatus.Unknown, (LiveConnectSession) null);
            break;
          case -2138701823:
          case -2138701821:
            error = (Exception) new LiveAuthException("invalid_request", ResourceHelper.GetString("InvalidAuthClient"), ex);
            break;
          case -2138701812:
            result = new LiveLoginResult(LiveConnectSessionStatus.NotConnected, (LiveConnectSession) null);
            break;
          default:
            error = (Exception) new LiveAuthException("server_error", ResourceHelper.GetString("ServerError"), ex);
            break;
        }
      }
      if (result == null)
        result = new LiveLoginResult(error);
      return result;
    }

    public LiveConnectSession LoadSession(LiveAuthClient authClient) => (LiveConnectSession) null;

    public void SaveSession(LiveConnectSession session)
    {
    }

    public async void CloseSession() => await (IAsyncAction) this.authenticator.SignOutUserAsync();

    private async Task<string> GetAccessToken(string scopes, bool silent)
    {
      string ticket = string.Empty;
      CredentialPromptType promptType = silent ? (CredentialPromptType) 2 : (CredentialPromptType) 0;
      List<OnlineIdServiceTicketRequest> ticketRequests = new List<OnlineIdServiceTicketRequest>();
      ticketRequests.Add(new OnlineIdServiceTicketRequest(scopes, "DELEGATION"));
      UserIdentity identity = await (IAsyncOperation<UserIdentity>) this.authenticator.AuthenticateUserAsync((IEnumerable<OnlineIdServiceTicketRequest>) ticketRequests, promptType);
      if (identity.Tickets != null && ((IReadOnlyCollection<OnlineIdServiceTicket>) identity.Tickets).Count > 0)
        ticket = ((IReadOnlyList<OnlineIdServiceTicket>) identity.Tickets)[0].Value;
      return ticket;
    }

    private async Task<string> GetAuthenticationToken(string redirectDomain, bool silent)
    {
      string ticket = string.Empty;
      Uri redirectUri = new Uri(redirectDomain, UriKind.Absolute);
      List<OnlineIdServiceTicketRequest> ticketRequests = new List<OnlineIdServiceTicketRequest>();
      ticketRequests.Add(new OnlineIdServiceTicketRequest(redirectUri.DnsSafeHost, "JWT"));
      CredentialPromptType promptType = silent ? (CredentialPromptType) 2 : (CredentialPromptType) 0;
      UserIdentity identity = await (IAsyncOperation<UserIdentity>) this.authenticator.AuthenticateUserAsync((IEnumerable<OnlineIdServiceTicketRequest>) ticketRequests, promptType);
      if (identity.Tickets != null && ((IReadOnlyCollection<OnlineIdServiceTicket>) identity.Tickets).Count > 0)
        ticket = ((IReadOnlyList<OnlineIdServiceTicket>) identity.Tickets)[0].Value;
      return ticket;
    }
  }
}
