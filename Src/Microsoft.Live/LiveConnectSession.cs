
// Type: Microsoft.Live.LiveConnectSession
// Assembly: Microsoft.Live, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 84D9FE74-951C-4BB4-80F8-EDD369EAC36D
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\Microsoft.Live.dll

using System;

#nullable disable
namespace Microsoft.Live
{
  public class LiveConnectSession
  {
    private static readonly TimeSpan ExpirationTimeBufferInSec = new TimeSpan(0, 0, 60);

    internal LiveConnectSession(LiveAuthClient authClient) => this.AuthClient = authClient;

    internal LiveConnectSession()
    {
    }

    public string AccessToken { get; internal set; }

    public string AuthenticationToken { get; internal set; }

    internal LiveAuthClient AuthClient { get; set; }
  }
}
