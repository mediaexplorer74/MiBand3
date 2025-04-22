// Decompiled with JetBrains decompiler
// Type: Microsoft.Live.LiveAuthException
// Assembly: Microsoft.Live, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 84D9FE74-951C-4BB4-80F8-EDD369EAC36D
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\Microsoft.Live.dll

using System;

#nullable disable
namespace Microsoft.Live
{
  public class LiveAuthException : Exception
  {
    public LiveAuthException()
    {
    }

    public LiveAuthException(string errorCode, string message)
      : base(message)
    {
      this.ErrorCode = errorCode;
    }

    public LiveAuthException(string errorCode, string message, Exception innerException)
      : base(message, innerException)
    {
      this.ErrorCode = errorCode;
    }

    internal LiveAuthException(string errorCode, string message, string requestState)
      : base(message)
    {
      this.ErrorCode = errorCode;
      this.State = requestState;
    }

    public string ErrorCode { get; private set; }

    public string State { get; internal set; }

    public override string ToString() => this.ErrorCode + ": " + base.ToString();
  }
}
