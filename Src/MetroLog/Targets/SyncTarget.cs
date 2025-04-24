
// Type: MetroLog.Targets.SyncTarget
// Assembly: MetroLog, Version=0.0.0.0, Culture=neutral, PublicKeyToken=ba4ace74c3b410f3
// MVID: 4C1C3979-5A8D-451B-A09E-C87C946DE5D1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MetroLog.dll

using MetroLog.Layouts;
using System;
using System.Threading.Tasks;

#nullable disable
namespace MetroLog.Targets
{
  public abstract class SyncTarget(Layout layout) : Target(layout)
  {
    protected override sealed Task<LogWriteOperation> WriteAsyncCore(
      LogWriteContext context,
      LogEventInfo entry)
    {
      try
      {
        this.Write(context, entry);
        return Task.FromResult<LogWriteOperation>(new LogWriteOperation((Target) this, entry, true));
      }
      catch (Exception ex)
      {
        InternalLogger.Current.Error(string.Format("Failed to write to target '{0}'.", (object) this), ex);
        return Task.FromResult<LogWriteOperation>(new LogWriteOperation((Target) this, entry, false));
      }
    }

    protected abstract void Write(LogWriteContext context, LogEventInfo entry);
  }
}
