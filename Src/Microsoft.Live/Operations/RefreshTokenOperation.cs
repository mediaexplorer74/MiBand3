// Decompiled with JetBrains decompiler
// Type: Microsoft.Live.Operations.RefreshTokenOperation
// Assembly: Microsoft.Live, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 84D9FE74-951C-4BB4-80F8-EDD369EAC36D
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\Microsoft.Live.dll

using Microsoft.Live.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Live.Operations
{
  internal class RefreshTokenOperation : WebOperation
  {
    private const string RefreshTokenPostBodyTemplate = "client_id={0}&refresh_token={1}&scope={2}&grant_type=refresh_token";
    private const string AuthCodePostBodyTemplate = "client_id={0}&code={1}&redirect_uri={2}&grant_type=authorization_code";
    private const string ContentTypeFormEncoded = "application/x-www-form-urlencoded";

    public RefreshTokenOperation(
      LiveAuthClient authClient,
      string clientId,
      string refreshToken,
      IEnumerable<string> scopes,
      SynchronizationContextWrapper syncContext)
      : base(new Uri(authClient.ConsentEndpoint + LiveAuthClient.TokenEndpoint), (string) null, syncContext)
    {
      this.AuthClient = authClient;
      this.Body = string.Format("client_id={0}&refresh_token={1}&scope={2}&grant_type=refresh_token", (object) HttpUtility.UrlEncode(clientId), (object) refreshToken, (object) HttpUtility.UrlEncode(LiveAuthClient.BuildScopeString(scopes)));
    }

    public RefreshTokenOperation(
      LiveAuthClient authClient,
      string clientId,
      string verificationCode,
      string redirectUri,
      SynchronizationContextWrapper syncContext)
      : base(new Uri(authClient.ConsentEndpoint + LiveAuthClient.TokenEndpoint), (string) null, syncContext)
    {
      this.AuthClient = authClient;
      this.Body = string.Format("client_id={0}&code={1}&redirect_uri={2}&grant_type=authorization_code", (object) HttpUtility.UrlEncode(clientId), (object) HttpUtility.UrlEncode(verificationCode), (object) HttpUtility.UrlEncode(redirectUri));
    }

    public LiveAuthClient AuthClient { get; private set; }

    public Action<LiveLoginResult> OperationCompletedCallback { get; set; }

    public override void Cancel()
    {
    }

    public Task<LiveLoginResult> ExecuteAsync()
    {
      TaskCompletionSource<LiveLoginResult> tcs = new TaskCompletionSource<LiveLoginResult>();
      this.OperationCompletedCallback = (Action<LiveLoginResult>) (result =>
      {
        if (result.Error != null)
          tcs.TrySetException((Exception) result.Error);
        else
          tcs.TrySetResult(result);
      });
      this.Execute();
      return tcs.Task;
    }

    protected override void OnCancel()
    {
    }

    protected override void OnExecute()
    {
      this.Request = WebRequestFactory.Current.CreateWebRequest(this.Url, "POST");
      this.Request.ContentType = "application/x-www-form-urlencoded";

      this.Request.BeginGetRequestStream(
          new AsyncCallback(this.OnGetRequestStreamCompleted), null);
    }

    protected void OnOperationCompleted(LiveLoginResult opResult)
    {
      Action<LiveLoginResult> completedCallback = this.OperationCompletedCallback;
      if (completedCallback == null)
        return;
      completedCallback(opResult);
    }

    protected override void OnWebResponseReceived(WebResponse response)
    {
      bool flag = response == null;
      LiveLoginResult opResult;
      try
      {
        Stream responseStream = !flag ? response.GetResponseStream() : (Stream) null;
        opResult = flag || responseStream == null ? new LiveLoginResult((Exception) new LiveAuthException("client_error", ResourceHelper.GetString("ConnectionError"))) : this.GenerateLoginResultFrom(responseStream);
      }
      finally
      {
        if (!flag)
          response.Dispose();
      }
      this.OnOperationCompleted(opResult);
    }

    private LiveLoginResult GenerateLoginResultFrom(Stream responseStream)
    {
      IDictionary<string, object> result;
      try
      {
        using (StreamReader streamReader = new StreamReader(responseStream))
          result = new JsonReader(streamReader.ReadToEnd()).ReadValue() as IDictionary<string, object>;
      }
      catch (FormatException ex)
      {
        return new LiveLoginResult((Exception) new LiveAuthException("server_error", ex.Message));
      }
      if (result == null)
        return new LiveLoginResult((Exception) new LiveAuthException("server_error", ResourceHelper.GetString("ServerError")));
      if (!result.ContainsKey("error"))
        return new LiveLoginResult(LiveConnectSessionStatus.Connected, LiveAuthClient.CreateSession(this.AuthClient, result));
      string errorCode = result["error"] as string;
      if (errorCode.Equals("invalid_grant", StringComparison.Ordinal))
        return new LiveLoginResult(LiveConnectSessionStatus.NotConnected, (LiveConnectSession) null);
      string empty = string.Empty;
      if (result.ContainsKey("error_description"))
        empty = result["error_description"] as string;
      return new LiveLoginResult((Exception) new LiveAuthException(errorCode, empty));
    }
  }
}
