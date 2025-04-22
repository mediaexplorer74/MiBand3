// Decompiled with JetBrains decompiler
// Type: Microsoft.Live.LoginResult
// Assembly: Microsoft.Live, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 84D9FE74-951C-4BB4-80F8-EDD369EAC36D
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\Microsoft.Live.dll

#nullable disable
namespace Microsoft.Live
{
  internal class LoginResult
  {
    public LiveConnectSessionStatus Status { get; internal set; }

    public LiveConnectSession Session { get; internal set; }

    public string ErrorCode { get; internal set; }

    public string ErrorDescription { get; internal set; }
  }
}
