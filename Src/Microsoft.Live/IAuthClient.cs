// Decompiled with JetBrains decompiler
// Type: Microsoft.Live.IAuthClient
// Assembly: Microsoft.Live, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 84D9FE74-951C-4BB4-80F8-EDD369EAC36D
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\Microsoft.Live.dll

using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Live
{
  internal interface IAuthClient
  {
    bool CanSignOut { get; }

    Task<LiveLoginResult> AuthenticateAsync(string scopes, bool silent);

    LiveConnectSession LoadSession(LiveAuthClient authClient);

    void SaveSession(LiveConnectSession session);

    void CloseSession();
  }
}
