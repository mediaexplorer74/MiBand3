
// Type: Microsoft.Live.LiveLoginResult
// Assembly: Microsoft.Live, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 84D9FE74-951C-4BB4-80F8-EDD369EAC36D
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\Microsoft.Live.dll

using System;

#nullable disable
namespace Microsoft.Live
{
  public class LiveLoginResult
  {
    internal LiveLoginResult(LiveConnectSessionStatus status, LiveConnectSession session)
    {
      this.Status = status;
      this.Session = session;
    }

    internal LiveLoginResult(Exception error) => this.Error = error;

    public LiveConnectSession Session { get; private set; }

    public LiveConnectSessionStatus Status { get; private set; }

    public string State { get; internal set; }

    internal Exception Error { get; private set; }
  }
}
