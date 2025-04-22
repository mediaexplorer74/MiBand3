// Decompiled with JetBrains decompiler
// Type: MetroLog.LogManagerFactory
// Assembly: MetroLog, Version=0.0.0.0, Culture=neutral, PublicKeyToken=ba4ace74c3b410f3
// MVID: 4C1C3979-5A8D-451B-A09E-C87C946DE5D1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MetroLog.dll

using MetroLog.Internal;
using System;
using System.Threading;

#nullable disable
namespace MetroLog
{
  public static class LogManagerFactory
  {
    private static readonly ILogConfigurator Configurator = (ILogConfigurator) new LogConfigurator();
    private static LoggingConfiguration defaultConfig = LogManagerFactory.Configurator.CreateDefaultSettings();
    private static readonly Lazy<ILogManager> LazyLogManager = new Lazy<ILogManager>((Func<ILogManager>) (() => LogManagerFactory.CreateLogManager()), LazyThreadSafetyMode.ExecutionAndPublication);

    public static LoggingConfiguration CreateLibraryDefaultSettings()
    {
      return LogManagerFactory.Configurator.CreateDefaultSettings();
    }

    public static ILogManager CreateLogManager(LoggingConfiguration config = null)
    {
      LoggingConfiguration configuration = config ?? LogManagerFactory.DefaultConfiguration;
      configuration.Freeze();
      LogManager manager = new LogManager(configuration);
      LogManagerFactory.Configurator.OnLogManagerCreated((ILogManager) manager);
      return (ILogManager) manager;
    }

    public static LoggingConfiguration DefaultConfiguration
    {
      get => LogManagerFactory.defaultConfig;
      set
      {
        if (value == null)
          throw new ArgumentNullException(nameof (value));
        if (LogManagerFactory.LazyLogManager.IsValueCreated)
          throw new InvalidOperationException("Must set DefaultConfiguration before any calls to DefaultLogManager");
        LogManagerFactory.defaultConfig = value;
      }
    }

    public static ILogManager DefaultLogManager => LogManagerFactory.LazyLogManager.Value;
  }
}
