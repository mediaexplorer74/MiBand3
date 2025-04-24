
// Type: MetroLog.LogWriteOperation
// Assembly: MetroLog, Version=0.0.0.0, Culture=neutral, PublicKeyToken=ba4ace74c3b410f3
// MVID: 4C1C3979-5A8D-451B-A09E-C87C946DE5D1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MetroLog.dll

using MetroLog.Targets;
using System.Collections.Generic;

#nullable disable
namespace MetroLog
{
  public struct LogWriteOperation
  {
    private readonly List<LogEventInfo> entries;

    public LogWriteOperation(Target target, LogEventInfo entry, bool success)
    {
      Target target1 = target;
      List<LogEventInfo> entries = new List<LogEventInfo>();
      entries.Add(entry);
      int num = success ? 1 : 0;
      this = new LogWriteOperation(target1, (IEnumerable<LogEventInfo>) entries, num != 0);
    }

    public LogWriteOperation(Target target, IEnumerable<LogEventInfo> entries, bool success)
    {
      this.Target = target;
      this.entries = new List<LogEventInfo>(entries);
      this.Success = success;
    }

    public Target Target { get; }

    public IEnumerable<LogEventInfo> GetEntries()
    {
      return (IEnumerable<LogEventInfo>) new List<LogEventInfo>((IEnumerable<LogEventInfo>) this.entries);
    }

    public bool Success { get; }
  }
}
