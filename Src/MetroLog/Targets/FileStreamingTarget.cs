
// Type: MetroLog.Targets.FileStreamingTarget
// Assembly: MetroLog, Version=0.0.0.0, Culture=neutral, PublicKeyToken=ba4ace74c3b410f3
// MVID: 4C1C3979-5A8D-451B-A09E-C87C946DE5D1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MetroLog.dll

using MetroLog.Layouts;
using System;

#nullable disable
namespace MetroLog.Targets
{
  [Obsolete("Use StreamingFileTarget")]
  public class FileStreamingTarget : StreamingFileTarget
  {
    public FileStreamingTarget()
    {
    }

    public FileStreamingTarget(Layout layout)
      : base(layout)
    {
    }
  }
}
