﻿// Decompiled with JetBrains decompiler
// Type: MetroLog.InternalLogger
// Assembly: MetroLog, Version=0.0.0.0, Culture=neutral, PublicKeyToken=ba4ace74c3b410f3
// MVID: 4C1C3979-5A8D-451B-A09E-C87C946DE5D1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MetroLog.dll

using MetroLog.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace MetroLog
{
  public class InternalLogger : ILogger
  {
    private static readonly InternalLogger current = new InternalLogger();

    private InternalLogger()
    {
    }

    public static ILogger Current => (ILogger) InternalLogger.current;

    public string Name => "(Internal)";

    public bool IsTraceEnabled => true;

    public bool IsDebugEnabled => true;

    public bool IsInfoEnabled => true;

    public bool IsWarnEnabled => true;

    public bool IsErrorEnabled => true;

    public bool IsFatalEnabled => true;

    public void Trace(string message, Exception ex = null) => this.Log(LogLevel.Trace, message, ex);

    public void Trace(string message, params object[] ps) => this.Log(LogLevel.Trace, message, ps);

    public void Debug(string message, Exception ex = null) => this.Log(LogLevel.Trace, message, ex);

    public void Debug(string message, params object[] ps) => this.Log(LogLevel.Trace, message, ps);

    public void Info(string message, Exception ex = null) => this.Log(LogLevel.Trace, message, ex);

    public void Info(string message, params object[] ps) => this.Log(LogLevel.Trace, message, ps);

    public void Warn(string message, Exception ex = null) => this.Log(LogLevel.Trace, message, ex);

    public void Warn(string message, params object[] ps) => this.Log(LogLevel.Trace, message, ps);

    public void Error(string message, Exception ex = null) => this.Log(LogLevel.Trace, message, ex);

    public void Error(string message, params object[] ps) => this.Log(LogLevel.Trace, message, ps);

    public void Fatal(string message, Exception ex = null) => this.Log(LogLevel.Trace, message, ex);

    public void Fatal(string message, params object[] ps) => this.Log(LogLevel.Trace, message, ps);

    public void Log(LogLevel logLevel, string message, Exception ex)
    {
      long nextSequenceId = LogEventInfo.GetNextSequenceId();
      string str = LogManager.GetDateTime().ToString("o");
      string upper = logLevel.ToString().ToUpper();
      int currentManagedThreadId = Environment.CurrentManagedThreadId;
      string message1;
      if (ex == null)
        message1 = string.Format("{0}|{1}|{2}|{3}|{4}", (object) nextSequenceId, (object) str, (object) upper, (object) currentManagedThreadId, (object) message);
      else
        message1 = string.Format("{0}|{1}|{2}|{3}|{4} --> {5}", (object) nextSequenceId, (object) str, (object) upper, (object) currentManagedThreadId, (object) message, (object) ex);
      System.Diagnostics.Debug.WriteLine(message1);
    }

    public void Log(LogLevel logLevel, string message, params object[] ps)
    {
      if (((IEnumerable<object>) ps).Any<object>())
        this.Log(logLevel, string.Format(message, ps), (Exception) null);
      else
        this.Log(logLevel, message, (Exception) null);
    }

    public bool IsEnabled(LogLevel level) => true;
  }
}
