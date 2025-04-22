// Decompiled with JetBrains decompiler
// Type: Microsoft.Live.Operations.ApiWriteOperation
// Assembly: Microsoft.Live, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 84D9FE74-951C-4BB4-80F8-EDD369EAC36D
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\Microsoft.Live.dll

using System;
using System.Net;

#nullable disable
namespace Microsoft.Live.Operations
{
  internal class ApiWriteOperation(
    LiveConnectClient client,
    Uri url,
    ApiMethod method,
    string body,
    SynchronizationContextWrapper syncContext) : ApiOperation(client, url, method, body, syncContext)
  {
    protected override void OnExecute()
    {
      if (!this.PrepareRequest())
        return;
      try
      {
        this.Request.BeginGetRequestStream((AsyncCallback) new AsyncCallback(((WebOperation) this).OnGetRequestStreamCompleted), (object) null);
      }
      catch (WebException ex)
      {
        if (ex.Status == WebExceptionStatus.RequestCanceled)
          this.OnCancel();
        else
          this.OnWebResponseReceived(ex.Response);
      }
    }
  }
}
