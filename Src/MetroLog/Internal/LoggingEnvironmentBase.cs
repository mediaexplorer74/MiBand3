
// Type: MetroLog.Internal.LoggingEnvironmentBase
// Assembly: MetroLog, Version=0.0.0.0, Culture=neutral, PublicKeyToken=ba4ace74c3b410f3
// MVID: 4C1C3979-5A8D-451B-A09E-C87C946DE5D1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MetroLog.dll

using System;
using System.Diagnostics;
using System.Reflection;

#nullable disable
namespace MetroLog.Internal
{
  public abstract class LoggingEnvironmentBase : ILoggingEnvironment
  {
    public Guid SessionId { get; private set; }

    public string FxProfile { get; private set; }

    public bool IsDebugging { get; private set; }

    public Version MetroLogVersion { get; private set; }

    protected LoggingEnvironmentBase(string fxProfile)
    {
      this.SessionId = Guid.NewGuid();
      this.FxProfile = fxProfile;
      this.IsDebugging = Debugger.IsAttached;
      this.MetroLogVersion = typeof (ILogger).GetTypeInfo().Assembly.GetName().Version;
    }

    public string ToJson() => SimpleJson.SerializeObject((object) this);
  }
}
