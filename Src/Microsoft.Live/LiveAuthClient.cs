// Decompiled with JetBrains decompiler
// Type: Microsoft.Live.LiveAuthClient
// Assembly: Microsoft.Live, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 84D9FE74-951C-4BB4-80F8-EDD369EAC36D
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\Microsoft.Live.dll

using Microsoft.Live.Operations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;

#nullable disable
namespace Microsoft.Live
{
  public sealed class LiveAuthClient : INotifyPropertyChanged
  {
    private const string AuthorizeUrlTemplate = "{0}{1}?client_id={2}&redirect_uri={3}&scope={4}&response_type={5}&locale={6}&display={7}{8}";
    private const string SignInOfferName = "wl.signin";
    internal static readonly string DefaultRedirectPath = "/oauth20_desktop.srf";
    internal static readonly string DefaultConsentEndpoint = "https://login.live.com";
    internal static readonly string AuthorizeEndpoint = "/oauth20_authorize.srf";
    internal static readonly string TokenEndpoint = "/oauth20_token.srf";
    internal static readonly string LogoutUrl = "https://login.live.com/oauth20_logout.srf";
    private static readonly char[] ScopeSeparators = new char[2]
    {
      ' ',
      ','
    };
    private string clientId;
    private string redirectUri;
    private LiveConnectSession session;
    private List<string> scopes;
    private int asyncInProgress;
    private SynchronizationContextWrapper syncContext;
    private List<string> currentScopes;
    private ThemeType? theme;

    public LiveConnectSession Session
    {
      get => this.session;
      internal set
      {
        this.session = value;
        this.NotifyPropertyChanged(nameof (Session));
      }
    }

    internal ResponseType ResponseType { get; set; }

    internal DisplayType Display { get; set; }

    internal string ConsentEndpoint { get; private set; }

    internal IAuthClient AuthClient { get; set; }

    internal string RedirectUrl => this.redirectUri;

    public event PropertyChangedEventHandler PropertyChanged;

    internal string BuildLoginUrl(string scopes, bool silent)
    {
      return string.Format("{0}{1}?client_id={2}&redirect_uri={3}&scope={4}&response_type={5}&locale={6}&display={7}{8}", (object) this.ConsentEndpoint, (object) LiveAuthClient.AuthorizeEndpoint, (object) HttpUtility.UrlEncode(this.clientId), (object) HttpUtility.UrlEncode(this.redirectUri), (object) HttpUtility.UrlEncode(scopes), (object) HttpUtility.UrlEncode(this.ResponseType.ToString().ToLowerInvariant()), (object) HttpUtility.UrlEncode(Platform.GetCurrentUICultureString()), silent ? (object) "none" : (object) HttpUtility.UrlEncode(this.Display.ToString().ToLowerInvariant()), silent ? (object) string.Empty : (object) ("&theme=" + HttpUtility.UrlEncode(this.Theme.ToString().ToLowerInvariant())));
    }

    internal static string BuildScopeString(IEnumerable<string> scopes)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (scopes != null)
      {
        foreach (string scope in scopes)
          stringBuilder.Append(scope).Append(LiveAuthClient.ScopeSeparators[0]);
      }
      return stringBuilder.ToString().TrimEnd(LiveAuthClient.ScopeSeparators);
    }

    internal static DateTimeOffset CalculateExpiration(string expiresIn)
    {
      DateTimeOffset expiration = DateTimeOffset.UtcNow;
      long result;
      if (long.TryParse(expiresIn, out result))
        expiration = expiration.AddSeconds((double) result);
      return expiration;
    }

    internal static IEnumerable<string> ParseScopeString(string scopesString)
    {
      return (IEnumerable<string>) new List<string>((IEnumerable<string>) scopesString.Split(LiveAuthClient.ScopeSeparators, StringSplitOptions.RemoveEmptyEntries));
    }

    private static LiveLoginResult GetErrorResult(IDictionary<string, object> result)
    {
      string errorCode = result["error"] as string;
      if (errorCode.Equals("access_denied", StringComparison.Ordinal))
        return new LiveLoginResult(LiveConnectSessionStatus.NotConnected, (LiveConnectSession) null);
      if (errorCode.Equals("unknown_user", StringComparison.Ordinal))
        return new LiveLoginResult(LiveConnectSessionStatus.Unknown, (LiveConnectSession) null);
      string empty = string.Empty;
      if (result.ContainsKey("error_description"))
        empty = result["error_description"] as string;
      return new LiveLoginResult((Exception) new LiveAuthException(errorCode, empty));
    }

    private void InitializeMembers(string clientId, string redirectUri)
    {
      this.clientId = clientId;
      this.ConsentEndpoint = LiveAuthClient.DefaultConsentEndpoint;
      if (!string.IsNullOrEmpty(redirectUri))
        this.redirectUri = redirectUri;
      else
        this.redirectUri = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}", (object) new Uri(this.ConsentEndpoint).GetComponents(UriComponents.SchemeAndServer, UriFormat.SafeUnescaped), (object) LiveAuthClient.DefaultRedirectPath);
      this.ResponseType = Platform.GetResponseType();
      this.Display = Platform.GetDisplayType();
    }

    private void NotifyPropertyChanged(string propertyName)
    {
      PropertyChangedEventHandler handler = this.PropertyChanged;
      if (handler == null)
        return;
      this.syncContext.Post((Action) (() => handler((object) this, new PropertyChangedEventArgs(propertyName))));
    }

    private static IDictionary<string, object> ParseQueryString(string query)
    {
      Dictionary<string, object> queryString = new Dictionary<string, object>();
      if (!string.IsNullOrEmpty(query))
      {
        query = query.TrimStart('?', '#');
        if (!string.IsNullOrEmpty(query))
        {
          string str1 = query;
          char[] chArray1 = new char[1]{ '&' };
          foreach (string str2 in str1.Split(chArray1))
          {
            char[] chArray2 = new char[1]{ '=' };
            string[] strArray = str2.Split(chArray2);
            if (strArray.Length == 2)
              queryString.Add(strArray[0], (object) strArray[1]);
          }
        }
      }
      return (IDictionary<string, object>) queryString;
    }

    private LiveLoginResult ParseResponseFragment(string fragment)
    {
      LiveLoginResult responseFragment = (LiveLoginResult) null;
      IDictionary<string, object> queryString = LiveAuthClient.ParseQueryString(fragment);
      if (queryString.ContainsKey("access_token"))
        responseFragment = new LiveLoginResult(LiveConnectSessionStatus.Connected, LiveAuthClient.CreateSession(this, queryString));
      else if (queryString.ContainsKey("error"))
        responseFragment = LiveAuthClient.GetErrorResult(queryString);
      return responseFragment;
    }

    private void PrepareForAsync()
    {
      if (this.asyncInProgress > 0)
        throw new LiveAuthException("client_error", ResourceHelper.GetString("AsyncOperationInProgress"));
      Interlocked.Increment(ref this.asyncInProgress);
      this.syncContext = SynchronizationContextWrapper.Current;
    }

    private void ProcessAuthResponse(string responseData, Action<LiveLoginResult> callback)
    {
      if (string.IsNullOrEmpty(responseData))
      {
        callback(new LiveLoginResult(LiveConnectSessionStatus.Unknown, (LiveConnectSession) null));
      }
      else
      {
        Uri uri;
        try
        {
          uri = new Uri(responseData, UriKind.Absolute);
        }
        catch (FormatException ex)
        {
          callback(new LiveLoginResult((Exception) new LiveAuthException("server_error", ResourceHelper.GetString("ServerError"))));
          return;
        }
        if (!string.IsNullOrEmpty(uri.Fragment))
        {
          callback(this.ParseResponseFragment(uri.Fragment));
        }
        else
        {
          if (!string.IsNullOrEmpty(uri.Query))
          {
            IDictionary<string, object> queryString = LiveAuthClient.ParseQueryString(uri.Query);
            if (queryString.ContainsKey("code"))
            {
              string verificationCode = queryString["code"] as string;
              if (!string.IsNullOrEmpty(verificationCode))
              {
                new RefreshTokenOperation(this, this.clientId, verificationCode, this.redirectUri, this.syncContext)
                {
                  OperationCompletedCallback = callback
                }.Execute();
                return;
              }
            }
            else if (queryString.ContainsKey("error"))
            {
              callback(LiveAuthClient.GetErrorResult(queryString));
              return;
            }
          }
          callback(new LiveLoginResult((Exception) new LiveAuthException("server_error", ResourceHelper.GetString("ServerError"))));
        }
      }
    }

    public LiveAuthClient()
      : this((string) null)
    {
    }

    public LiveAuthClient(string redirectUri)
    {
      if (!string.IsNullOrEmpty(redirectUri) && !LiveAuthClient.IsValidRedirectDomain(redirectUri))
        throw new ArgumentException(redirectUri, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, ResourceHelper.GetString("UrlInvalid"), (object) nameof (redirectUri)));
      this.AuthClient = (IAuthClient) new TailoredAuthClient(this);
      if (string.IsNullOrEmpty(redirectUri))
        redirectUri = "ms-app";
      this.InitializeMembers(string.Empty, redirectUri);
    }

    public ThemeType Theme
    {
      get
      {
        if (!this.theme.HasValue)
          this.theme = new ThemeType?(Platform.GetThemeType());
        return this.theme.Value;
      }
      set => this.theme = new ThemeType?(value);
    }

    public bool CanLogout => this.AuthClient.CanSignOut;

    public Task<LiveLoginResult> InitializeAsync()
    {
      return this.InitializeAsync((IEnumerable<string>) new List<string>());
    }

    public Task<LiveLoginResult> InitializeAsync(IEnumerable<string> scopes)
    {
      return scopes != null ? this.InitializeAsync(((IEnumerable<string>) scopes).ToArray<string>()) : this.InitializeAsync((string[]) null);
    }

    public Task<LiveLoginResult> InitializeAsync(params string[] scopes)
    {
      return scopes != null ? this.ExecuteAuthTaskAsync((IEnumerable<string>) scopes, true) : throw new ArgumentNullException(nameof (scopes));
    }

    public Task<LiveLoginResult> LoginAsync(IEnumerable<string> scopes)
    {
      return scopes != null ? this.LoginAsync(((IEnumerable<string>) scopes).ToArray<string>()) : this.LoginAsync((string[]) null);
    }

    public Task<LiveLoginResult> LoginAsync(params string[] scopes)
    {
      return scopes != null || this.scopes != null ? this.ExecuteAuthTaskAsync((IEnumerable<string>) scopes, false) : throw new ArgumentNullException(nameof (scopes));
    }

    public void Logout()
    {
      if (!this.CanLogout)
        throw new LiveConnectException("client_error", ResourceHelper.GetString("CantLogout"));
      this.AuthClient.CloseSession();
    }

    internal static LiveConnectSession CreateSession(
      LiveAuthClient client,
      IDictionary<string, object> result)
    {
      LiveConnectSession session = new LiveConnectSession(client);
      if (result.ContainsKey("access_token"))
      {
        session.AccessToken = result["access_token"] as string;
        if (result.ContainsKey("authentication_token"))
          session.AuthenticationToken = result["authentication_token"] as string;
      }
      return session;
    }

    internal async Task<LiveLoginResult> GetLoginStatusAsync()
    {
      LiveLoginResult result = await this.AuthClient.AuthenticateAsync(LiveAuthClient.BuildScopeString((IEnumerable<string>) this.currentScopes), true);
      if (result.Status == LiveConnectSessionStatus.NotConnected && this.currentScopes.Count > 1)
      {
        this.currentScopes = new List<string>((IEnumerable<string>) new string[1]
        {
          "wl.signin"
        });
        result = await this.AuthClient.AuthenticateAsync(LiveAuthClient.BuildScopeString((IEnumerable<string>) this.currentScopes), true);
      }
      if (result.Status == LiveConnectSessionStatus.Unknown)
        this.Session = (LiveConnectSession) null;
      else if (result.Session != null && !LiveAuthClient.AreSessionsSame(this.Session, result.Session))
        this.Session = result.Session;
      return result;
    }

    internal bool RefreshToken(Action<LiveLoginResult> completionCallback)
    {
      this.TryRefreshToken(completionCallback);
      return true;
    }

    private async Task<LiveLoginResult> ExecuteAuthTaskAsync(
      IEnumerable<string> scopes,
      bool silent)
    {
      if (scopes != null)
        this.scopes = new List<string>((IEnumerable<string>) scopes);
      this.EnsureSignInScope();
      this.PrepareForAsync();
      LiveLoginResult result = await this.AuthClient.AuthenticateAsync(LiveAuthClient.BuildScopeString((IEnumerable<string>) this.scopes), silent);
      if (result.Session != null && !LiveAuthClient.AreSessionsSame(this.Session, result.Session))
      {
        this.MergeScopes();
        this.Session = result.Session;
      }
      Interlocked.Decrement(ref this.asyncInProgress);
      return result.Error == null ? result : throw result.Error;
    }

    private static bool AreSessionsSame(LiveConnectSession session1, LiveConnectSession session2)
    {
      if (session1 == null || session2 == null)
        return session1 == session2;
      return session1.AccessToken == session2.AccessToken && session1.AuthenticationToken == session2.AuthenticationToken;
    }

    private static bool IsValidRedirectDomain(string redirectDomain)
    {
      if (!redirectDomain.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
      {
        if (!redirectDomain.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
          return false;
      }
      try
      {
        return new Uri(redirectDomain, UriKind.Absolute).IsWellFormedOriginalString();
      }
      catch (FormatException ex)
      {
        return false;
      }
    }

    private void EnsureSignInScope()
    {
      if (!string.IsNullOrEmpty(this.scopes.Find((Predicate<string>) (s => string.CompareOrdinal(s, "wl.signin") == 0))))
        return;
      this.scopes.Insert(0, "wl.signin");
    }

    private string GetAppPackageSid()
    {
      return ((Uri) WebAuthenticationBroker.GetCurrentApplicationCallbackUri()).AbsoluteUri;
    }

    private void MergeScopes()
    {
      if (this.currentScopes == null)
      {
        this.currentScopes = new List<string>((IEnumerable<string>) this.scopes);
      }
      else
      {
        using (List<string>.Enumerator enumerator = this.scopes.GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            string current = enumerator.Current;
            if (!this.currentScopes.Contains(current))
              this.currentScopes.Add(current);
          }
        }
      }
    }

    private async void TryRefreshToken(Action<LiveLoginResult> completionCallback)
    {
      LiveLoginResult result = await this.AuthClient.AuthenticateAsync(LiveAuthClient.BuildScopeString((IEnumerable<string>) this.currentScopes), true);
      if (result.Status == LiveConnectSessionStatus.NotConnected && this.currentScopes.Count > 1)
      {
        this.currentScopes = new List<string>((IEnumerable<string>) new string[1]
        {
          "wl.signin"
        });
        result = await this.AuthClient.AuthenticateAsync(LiveAuthClient.BuildScopeString((IEnumerable<string>) this.currentScopes), true);
      }
      if (result.Status == LiveConnectSessionStatus.Unknown)
        this.Session = (LiveConnectSession) null;
      else if (result.Session != null && !LiveAuthClient.AreSessionsSame(this.Session, result.Session))
        this.Session = result.Session;
      completionCallback(result);
    }

    internal static class StorageConstants
    {
      public const string RefreshToken = "Microsoft.Live.LiveAuthClient.RefreshToken";
      public const string SigningOut = "Microsoft.Live.LiveAuthClient.SigningOut";
    }
  }
}
