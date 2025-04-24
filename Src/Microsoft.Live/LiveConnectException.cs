
// Type: Microsoft.Live.LiveConnectException
// Assembly: Microsoft.Live, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 84D9FE74-951C-4BB4-80F8-EDD369EAC36D
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\Microsoft.Live.dll

using System;

#nullable disable
namespace Microsoft.Live
{
  public class LiveConnectException : Exception
  {
    public LiveConnectException()
    {
    }

    public LiveConnectException(string errorCode, string message)
      : base(message)
    {
      this.ErrorCode = errorCode;
    }

    public LiveConnectException(string errorCode, string message, Exception innerException)
      : base(message, innerException)
    {
      this.ErrorCode = errorCode;
    }

    public string ErrorCode { get; private set; }

    public override string ToString() => this.ErrorCode + ": " + base.ToString();
  }
}
