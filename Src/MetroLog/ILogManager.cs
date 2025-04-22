// Decompiled with JetBrains decompiler
// Type: MetroLog.ILogManager
// Assembly: MetroLog, Version=0.0.0.0, Culture=neutral, PublicKeyToken=ba4ace74c3b410f3
// MVID: 4C1C3979-5A8D-451B-A09E-C87C946DE5D1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MetroLog.dll

using System;
using System.IO;
using System.Threading.Tasks;

#nullable disable
namespace MetroLog
{
  public interface ILogManager
  {
    LoggingConfiguration DefaultConfiguration { get; }

    ILogger GetLogger<T>(LoggingConfiguration config = null);

    ILogger GetLogger(Type type, LoggingConfiguration config = null);

    ILogger GetLogger(string name, LoggingConfiguration config = null);

    event EventHandler<LoggerEventArgs> LoggerCreated;

    Task<Stream> GetCompressedLogs();
  }
}
