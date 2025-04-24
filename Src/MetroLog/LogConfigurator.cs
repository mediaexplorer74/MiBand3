
// Type: MetroLog.LogConfigurator
// Assembly: MetroLog, Version=0.0.0.0, Culture=neutral, PublicKeyToken=ba4ace74c3b410f3
// MVID: 4C1C3979-5A8D-451B-A09E-C87C946DE5D1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MetroLog.dll

using MetroLog.Internal;
using MetroLog.Targets;

#nullable disable
namespace MetroLog
{
  internal sealed class LogConfigurator : LogConfiguratorBase
  {
    public override LoggingConfiguration CreateDefaultSettings()
    {
      LoggingConfiguration defaultSettings = base.CreateDefaultSettings();
      defaultSettings.AddTarget(LogLevel.Error, LogLevel.Fatal, (Target) new StreamingFileTarget());
      return defaultSettings;
    }

    public override void OnLogManagerCreated(ILogManager manager)
    {
      LazyFlushManager.Initialize(manager);
    }
  }
}
