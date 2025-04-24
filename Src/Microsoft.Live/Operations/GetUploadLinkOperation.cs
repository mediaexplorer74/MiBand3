
// Type: Microsoft.Live.Operations.GetUploadLinkOperation
// Assembly: Microsoft.Live, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 84D9FE74-951C-4BB4-80F8-EDD369EAC36D
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\Microsoft.Live.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Live.Operations
{
  internal class GetUploadLinkOperation : ApiOperation
  {
    private const string UploadLocationKey = "upload_location";
    private const string FilePathPrefix = "/file.";

    public GetUploadLinkOperation(
      LiveConnectClient client,
      Uri url,
      string fileName,
      OverwriteOption option,
      SynchronizationContextWrapper syncContext)
      : base(client, url, ApiMethod.Get, (string) null, syncContext)
    {
      this.FileName = fileName;
      this.OverwriteOption = option;
    }

    public string FileName { get; private set; }

    public OverwriteOption OverwriteOption { get; private set; }

    private bool IsFilePath
    {
      get
      {
        string lowerInvariant = this.Url.AbsoluteUri.Substring(this.LiveClient.ApiEndpoint.Length).ToLowerInvariant();
        return lowerInvariant.StartsWith("/file.") || lowerInvariant.StartsWith("/file.".Substring(1));
      }
    }

    public Task<LiveOperationResult> ExecuteAsync()
    {
      TaskCompletionSource<LiveOperationResult> tcs = new TaskCompletionSource<LiveOperationResult>();
      GetUploadLinkOperation uploadLinkOperation = this;
      uploadLinkOperation.OperationCompletedCallback = uploadLinkOperation.OperationCompletedCallback + (Action<LiveOperationResult>) (result =>
      {
        if (result.IsCancelled)
          tcs.TrySetCanceled();
        else if (result.Error != null)
          tcs.TrySetException((Exception) result.Error);
        else
          tcs.TrySetResult(result);
      });
      this.Execute();
      return tcs.Task;
    }

    protected override void OnWebResponseReceived(WebResponse response)
    {
      LiveOperationResult operationResultFrom = this.CreateOperationResultFrom(response);
      if (operationResultFrom.Error != null)
      {
        this.OnOperationCompleted(operationResultFrom);
      }
      else
      {
        string uploadLocation = (string) null;
        if (operationResultFrom.Result != null && operationResultFrom.Result.ContainsKey("upload_location"))
          uploadLocation = operationResultFrom.Result["upload_location"] as string;
        LiveOperationResult opResult;
        if (string.IsNullOrEmpty(uploadLocation))
        {
          opResult = new LiveOperationResult((Exception) new LiveConnectException("client_error", ResourceHelper.GetString("NoUploadLinkFound")), false);
        }
        else
        {
          try
          {
            opResult = new LiveOperationResult((IDictionary<string, object>) null, this.ConstructUploadUri(uploadLocation).OriginalString);
          }
          catch (LiveConnectException ex)
          {
            opResult = new LiveOperationResult((Exception) ex, false);
          }
        }
        this.OnOperationCompleted(opResult);
      }
    }

    private Uri ConstructUploadUri(string uploadLocation)
    {
      Uri uri;
      try
      {
        string str = this.Url.Query;
        if (!string.IsNullOrWhiteSpace(str))
        {
          int num = str.IndexOf('?');
          if (num != -1)
            str = str.Substring(num + 1);
          uploadLocation = uploadLocation.Contains("?") ? uploadLocation + "&" + str : uploadLocation + "?" + str;
        }
        uri = new Uri(uploadLocation, UriKind.Absolute);
      }
      catch (FormatException ex)
      {
        throw new LiveConnectException("client_error", ResourceHelper.GetString("NoUploadLinkFound"), (Exception) ex);
      }
      string query = uri.Query;
      StringBuilder sb = new StringBuilder(uri.GetComponents(UriComponents.SchemeAndServer | UriComponents.KeepDelimiter | UriComponents.Path, UriFormat.SafeUnescaped));
      if (!this.IsFilePath)
        sb.AppendUrlPath(Uri.EscapeDataString(this.FileName));
      bool flag = !string.IsNullOrEmpty(query);
      if (flag)
        sb.Append(query);
      if (!this.IsFilePath)
      {
        sb.Append(flag ? '&' : '?');
        sb.AppendQueryParam("overwrite", QueryParameters.GetOverwriteValue(this.OverwriteOption));
      }
      try
      {
        return new Uri(sb.ToString(), UriKind.Absolute);
      }
      catch (FormatException ex)
      {
        throw new LiveConnectException("client_error", string.Format((IFormatProvider) CultureInfo.CurrentUICulture, ResourceHelper.GetString("FileNameInvalid"), (object) "fileName"), (Exception) ex);
      }
    }
  }
}
