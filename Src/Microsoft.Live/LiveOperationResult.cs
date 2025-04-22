// Decompiled with JetBrains decompiler
// Type: Microsoft.Live.LiveOperationResult
// Assembly: Microsoft.Live, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 84D9FE74-951C-4BB4-80F8-EDD369EAC36D
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\Microsoft.Live.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.Live
{
  public class LiveOperationResult
  {
    internal LiveOperationResult(IDictionary<string, object> result, string rawResult)
    {
      this.Result = result ?? (IDictionary<string, object>) new DynamicDictionary();
      this.RawResult = rawResult;
    }

    internal LiveOperationResult(Exception error, bool cancelled)
    {
      this.Error = error;
      this.IsCancelled = cancelled;
    }

    public IDictionary<string, object> Result { get; private set; }

    public string RawResult { get; private set; }

    internal Exception Error { get; private set; }

    internal bool IsCancelled { get; private set; }

    internal static LiveOperationResult FromResponse(string response)
    {
      LiveOperationResult.Creator observer = new LiveOperationResult.Creator();
      ServerResponseReader.Instance.Read(response, (IServerResponseReaderObserver) observer);
      return observer.Result;
    }

    private class Creator : IServerResponseReaderObserver
    {
      private LiveOperationResult result;

      public void OnSuccessResponse(IDictionary<string, object> result, string rawResult)
      {
        this.Result = new LiveOperationResult(result, rawResult);
      }

      public void OnErrorResponse(string code, string message)
      {
        this.Result = new LiveOperationResult((Exception) new LiveConnectException(code, message), false);
      }

      public void OnInvalidJsonResponse(FormatException exception)
      {
        this.Result = new LiveOperationResult((Exception) exception, false);
      }

      public LiveOperationResult Result
      {
        get => this.result;
        private set => this.result = value;
      }
    }
  }
}
