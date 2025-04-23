// Decompiled with JetBrains decompiler
// Type: Microsoft.Live.Operations.ApiOperation
// Assembly: Microsoft.Live, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 84D9FE74-951C-4BB4-80F8-EDD369EAC36D
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\Microsoft.Live.dll

using Microsoft.Live.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

#nullable disable
namespace Microsoft.Live.Operations
{
  internal class ApiOperation : WebOperation
  {
    internal const string ContentTypeJson = "application/json;charset=UTF-8";
    internal const string AuthorizationHeader = "Authorization";
    internal const string LibraryHeader = "X-HTTP-Live-Library";
    internal const string ApiError = "error";
    internal const string ApiErrorCode = "code";
    internal const string ApiErrorMessage = "message";
    internal const string ApiClientErrorCode = "client_error";
    internal const string ApiServerErrorCode = "server_error";
    internal const string MoveRequestBodyTemplate = "{{ \"destination\" : \"{0}\" }}";
    private bool refreshed;

    public ApiOperation(
      LiveConnectClient client,
      Uri url,
      ApiMethod method,
      string body,
      SynchronizationContextWrapper syncContext)
      : base(url, body, syncContext)
    {
      this.Method = method;
      this.LiveClient = client;
    }

    public LiveConnectClient LiveClient { get; private set; }

    public ApiMethod Method { get; private set; }

    public Action<LiveOperationResult> OperationCompletedCallback { get; set; }

    internal static string SerializePostBody(IDictionary<string, object> body)
    {
      if (body == null)
        throw new ArgumentNullException(nameof (body));
      StringBuilder sb = new StringBuilder();
      using (StringWriter writer = new StringWriter((StringBuilder) sb))
      {
        new JsonWriter((TextWriter) writer).WriteValue((object) body);
        writer.Flush();
      }
      return sb.ToString();
    }

    internal static LiveOperationResult CreateOperationResultFrom(
      string responseBody,
      ApiMethod method)
    {
      if (!string.IsNullOrEmpty(responseBody))
        return LiveOperationResult.FromResponse(responseBody);
      return method != ApiMethod.Delete ? new LiveOperationResult((Exception) new LiveConnectException("client_error", ResourceHelper.GetString("NoResponseData")), false) : new LiveOperationResult((IDictionary<string, object>) null, responseBody);
    }

    protected LiveOperationResult CreateOperationResultFrom(WebResponse response)
    {
      bool flag = response == null;
      try
      {
        Stream responseStream = !flag ? response.GetResponseStream() : (Stream) null;
        if (flag || responseStream == null)
          return new LiveOperationResult((Exception) new LiveConnectException("client_error", ResourceHelper.GetString("ConnectionError")), false);
        using (StreamReader streamReader = new StreamReader(responseStream))
          return ApiOperation.CreateOperationResultFrom(streamReader.ReadToEnd(), this.Method);
      }
      finally
      {
        if (!flag)
          ((IDisposable) response).Dispose();
      }
    }

    protected override void OnExecute()
    {
      if (!this.PrepareRequest())
        return;
      try
      {
        this.Request.BeginGetResponse(
            new AsyncCallback(this.OnGetResponseCompleted), null);
      }
      catch (WebException ex)
      {
        if (ex.Status == WebExceptionStatus.RequestCanceled)
          this.OnCancel();
        else
          this.OnWebResponseReceived(ex.Response);
      }
    }

    protected override void OnCancel()
    {
      this.OnOperationCompleted(new LiveOperationResult((Exception) null, true));
    }

    protected void OnOperationCompleted(LiveOperationResult opResult)
    {
      Action<LiveOperationResult> completedCallback = this.OperationCompletedCallback;
      if (completedCallback == null)
        return;
      completedCallback(opResult);
    }

    protected override void OnWebResponseReceived(WebResponse response)
    {
      this.OnOperationCompleted(this.CreateOperationResultFrom(response));
    }

    protected bool PrepareRequest()
    {
      if (this.RefreshTokenIfNeeded())
        return false;
      string method;
      switch (this.Method)
      {
        case ApiMethod.Upload:
          method = "PUT";
          break;
        case ApiMethod.Download:
          method = "GET";
          break;
        default:
          method = this.Method.ToString().ToUpperInvariant();
          break;
      }
      this.Request = WebRequestFactory.Current.CreateWebRequest(this.Url, method);
      if (this.LiveClient.Session != null)
        this.Request.Headers["Authorization"] = "bearer " + this.LiveClient.Session.AccessToken;
      this.Request.Headers["X-HTTP-Live-Library"] = Platform.GetLibraryHeaderValue();
      if (!string.IsNullOrEmpty(this.Body))
        this.Request.ContentType = "application/json;charset=UTF-8";
      return true;
    }

    protected bool RefreshTokenIfNeeded()
    {
      bool flag = false;
      LiveAuthClient authClient = this.LiveClient.Session?.AuthClient;
      if (!this.refreshed && authClient != null)
      {
        this.refreshed = true;
        flag = authClient.RefreshToken(new Action<LiveLoginResult>(this.OnRefreshTokenOperationCompleted));
      }
      return flag;
    }

    private void OnRefreshTokenOperationCompleted(LiveLoginResult result)
    {
      switch (result.Status)
      {
        case LiveConnectSessionStatus.Unknown:
          this.LiveClient.Session = (LiveConnectSession) null;
          this.OnOperationCompleted(new LiveOperationResult((Exception) new LiveConnectException("client_error", ResourceHelper.GetString("UserNotLoggedIn")), false));
          return;
        case LiveConnectSessionStatus.Connected:
          this.LiveClient.Session = result.Session;
          break;
      }
      this.InternalExecute();
    }
  }
}
