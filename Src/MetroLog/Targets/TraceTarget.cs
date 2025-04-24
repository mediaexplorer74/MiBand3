
// Type: MetroLog.Targets.TraceTarget
// Assembly: MetroLog, Version=0.0.0.0, Culture=neutral, PublicKeyToken=ba4ace74c3b410f3
// MVID: 4C1C3979-5A8D-451B-A09E-C87C946DE5D1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MetroLog.dll

using MetroLog.Layouts;
using System.Diagnostics;

#nullable disable
namespace MetroLog.Targets
{
  public class TraceTarget(Layout layout) : SyncTarget(layout)
  {
    public TraceTarget()
      : this((Layout) new SingleLineLayout())
    {
    }

    protected override void Write(LogWriteContext context, LogEventInfo entry)
    {
      Debug.WriteLine(this.Layout.GetFormattedString(context, entry));
    }
  }
}
