﻿
// Type: MetroLog.Targets.BufferedTarget
// Assembly: MetroLog, Version=0.0.0.0, Culture=neutral, PublicKeyToken=ba4ace74c3b410f3
// MVID: 4C1C3979-5A8D-451B-A09E-C87C946DE5D1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MetroLog.dll

using MetroLog.Layouts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#nullable disable
namespace MetroLog.Targets
{
  public abstract class BufferedTarget : AsyncTarget, ILazyFlushable
  {
    private readonly object _lock = new object();

    private List<LogEventInfo> Buffer { get; set; }

    private int Threshold { get; set; }

    public BufferedTarget(Layout layout, int threshold)
      : base(layout)
    {
      this.Threshold = threshold >= 1 ? threshold : throw new ArgumentOutOfRangeException(nameof (threshold));
      this.Buffer = new List<LogEventInfo>();
    }

    protected override sealed Task<LogWriteOperation> WriteAsyncCore(
      LogWriteContext context,
      LogEventInfo entry)
    {
      try
      {
        List<LogEventInfo> toFlush = (List<LogEventInfo>) null;
        lock (this._lock)
        {
          this.Buffer.Add(entry);
          if (this.Buffer.Count >= this.Threshold)
          {
            toFlush = new List<LogEventInfo>((IEnumerable<LogEventInfo>) this.Buffer);
            this.Buffer = new List<LogEventInfo>();
          }
        }
        return toFlush != null ? this.FlushAsync(context, (IEnumerable<LogEventInfo>) toFlush) : Task.FromResult<LogWriteOperation>(new LogWriteOperation((Target) this, entry, true));
      }
      catch (Exception ex)
      {
        InternalLogger.Current.Error(string.Format("Failed to write to target '{0}'.", (object) this), ex);
        return Task.FromResult<LogWriteOperation>(new LogWriteOperation((Target) this, entry, false));
      }
    }

    private async Task<LogWriteOperation> FlushAsync(
      LogWriteContext context,
      IEnumerable<LogEventInfo> toFlush)
    {
      try
      {
        await this.DoFlushAsync(context, toFlush);
        return new LogWriteOperation((Target) this, toFlush, true);
      }
      catch (Exception ex)
      {
        InternalLogger.Current.Error(string.Format("Failed to flush for target '{0}'.", (object) this), ex);
        return new LogWriteOperation((Target) this, toFlush, false);
      }
    }

    protected abstract Task DoFlushAsync(LogWriteContext context, IEnumerable<LogEventInfo> toFlush);

    async Task ILazyFlushable.LazyFlushAsync(LogWriteContext context)
    {
      List<LogEventInfo> logEventInfoList = (List<LogEventInfo>) null;
      lock (this._lock)
      {
        logEventInfoList = new List<LogEventInfo>((IEnumerable<LogEventInfo>) this.Buffer);
        this.Buffer = new List<LogEventInfo>();
      }
      if (!logEventInfoList.Any<LogEventInfo>())
        return;
      await this.DoFlushAsync(context, (IEnumerable<LogEventInfo>) logEventInfoList);
    }
  }
}
