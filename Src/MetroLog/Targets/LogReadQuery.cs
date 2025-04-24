
// Type: MetroLog.Targets.LogReadQuery
// Assembly: MetroLog, Version=0.0.0.0, Culture=neutral, PublicKeyToken=ba4ace74c3b410f3
// MVID: 4C1C3979-5A8D-451B-A09E-C87C946DE5D1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MetroLog.dll

using System;

#nullable disable
namespace MetroLog.Targets
{
  public class LogReadQuery
  {
    public bool IsTraceEnabled { get; set; }

    public bool IsDebugEnabled { get; set; }

    public bool IsInfoEnabled { get; set; }

    public bool IsWarnEnabled { get; set; }

    public bool IsErrorEnabled { get; set; }

    public bool IsFatalEnabled { get; set; }

    public int Top { get; set; }

    public DateTime FromDateTimeUtc { get; set; }

    public LogReadQuery()
    {
      this.IsTraceEnabled = false;
      this.IsDebugEnabled = false;
      this.IsInfoEnabled = true;
      this.IsWarnEnabled = true;
      this.IsErrorEnabled = true;
      this.IsFatalEnabled = true;
      this.Top = 1000;
      this.FromDateTimeUtc = DateTime.UtcNow.AddDays(-7.0);
    }

    public void SetLevels(LogLevel from, LogLevel to)
    {
      this.IsTraceEnabled = LogLevel.Trace >= from && LogLevel.Trace <= to;
      this.IsDebugEnabled = LogLevel.Debug >= from && LogLevel.Debug <= to;
      this.IsInfoEnabled = LogLevel.Info >= from && LogLevel.Info <= to;
      this.IsWarnEnabled = LogLevel.Warn >= from && LogLevel.Warn <= to;
      this.IsErrorEnabled = LogLevel.Error >= from && LogLevel.Error <= to;
      this.IsFatalEnabled = LogLevel.Fatal >= from && LogLevel.Fatal <= to;
    }
  }
}
