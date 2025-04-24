
// Type: Microsoft.Live.Operations.WebOperation
// Assembly: Microsoft.Live, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 84D9FE74-951C-4BB4-80F8-EDD369EAC36D
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\Microsoft.Live.dll

using System;
using System.IO;
using System.Net;
using System.Text;

#nullable disable
namespace Microsoft.Live.Operations
{
  internal abstract class WebOperation : Operation
  {
    protected WebOperation(Uri url, string body, SynchronizationContextWrapper syncContext)
      : base(syncContext)
    {
      this.Url = url;
      this.Body = body;
    }

    public string Body { get; internal set; }

    public WebRequest Request { get; internal set; }

    public Uri Url { get; internal set; }

    public override void Cancel()
    {
      if (this.Status == OperationStatus.Cancelled || this.Status == OperationStatus.Completed)
        return;
      this.Status = OperationStatus.Cancelled;
      if (this.Request != null)
        this.Request.Abort();
      else
        this.OnCancel();
    }

    protected virtual void OnGetRequestStreamCompleted(IAsyncResult ar)
    {
      if (!ar.IsCompleted)
        return;
      try
      {
        using (Stream requestStream = this.Request.EndGetRequestStream((IAsyncResult) ar))
        {
          if (!string.IsNullOrEmpty(this.Body))
          {
            byte[] bytes = Encoding.UTF8.GetBytes(this.Body);
            requestStream.Write(bytes, 0, bytes.Length);
          }
        }
        this.Request.BeginGetResponse((AsyncCallback) new AsyncCallback(this.OnGetResponseCompleted), (object) null);
      }
      catch (WebException ex)
      {
        if (ex.Status == WebExceptionStatus.RequestCanceled)
          this.OnCancel();
        else
          this.OnWebResponseReceived(ex.Response);
      }
      catch (IOException ex)
      {
        this.OnWebResponseReceived((WebResponse) null);
      }
    }

    protected void OnGetResponseCompleted(IAsyncResult ar)
    {
      if (!ar.IsCompleted)
        return;
      try
      {
        this.OnWebResponseReceived(this.Request.EndGetResponse((IAsyncResult) ar));
      }
      catch (WebException ex)
      {
        if (ex.Status == WebExceptionStatus.RequestCanceled)
        {
          this.OnCancel();
        }
        else
        {
          using (ex.Response)
            this.OnWebResponseReceived(ex.Response);
        }
      }
    }

    protected abstract void OnWebResponseReceived(WebResponse response);
  }
}
