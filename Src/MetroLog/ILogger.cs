// Decompiled with JetBrains decompiler
// Type: MetroLog.ILogger
// Assembly: MetroLog, Version=0.0.0.0, Culture=neutral, PublicKeyToken=ba4ace74c3b410f3
// MVID: 4C1C3979-5A8D-451B-A09E-C87C946DE5D1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MetroLog.dll

using System;

#nullable disable
namespace MetroLog
{
  public interface ILogger
  {
    string Name { get; }

    bool IsTraceEnabled { get; }

    bool IsDebugEnabled { get; }

    bool IsInfoEnabled { get; }

    bool IsWarnEnabled { get; }

    bool IsErrorEnabled { get; }

    bool IsFatalEnabled { get; }

    void Trace(string message, Exception ex = null);

    void Trace(string message, params object[] ps);

    void Debug(string message, Exception ex = null);

    void Debug(string message, params object[] ps);

    void Info(string message, Exception ex = null);

    void Info(string message, params object[] ps);

    void Warn(string message, Exception ex = null);

    void Warn(string message, params object[] ps);

    void Error(string message, Exception ex = null);

    void Error(string message, params object[] ps);

    void Fatal(string message, Exception ex = null);

    void Fatal(string message, params object[] ps);

    void Log(LogLevel logLevel, string message, Exception ex);

    void Log(LogLevel logLevel, string message, params object[] ps);

    bool IsEnabled(LogLevel level);
  }
}
