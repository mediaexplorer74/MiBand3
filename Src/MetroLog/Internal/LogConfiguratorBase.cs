
// Type: MetroLog.Internal.LogConfiguratorBase
// Assembly: MetroLog, Version=0.0.0.0, Culture=neutral, PublicKeyToken=ba4ace74c3b410f3
// MVID: 4C1C3979-5A8D-451B-A09E-C87C946DE5D1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MetroLog.dll

using MetroLog.Targets;

#nullable disable
namespace MetroLog.Internal
{
  public class LogConfiguratorBase : ILogConfigurator
  {
    public virtual LoggingConfiguration CreateDefaultSettings()
    {
      LoggingConfiguration defaultSettings = new LoggingConfiguration();
      defaultSettings.AddTarget(LogLevel.Trace, LogLevel.Fatal, (Target) new DebugTarget());
      return defaultSettings;
    }

    public virtual void OnLogManagerCreated(ILogManager manager)
    {
    }
  }
}
