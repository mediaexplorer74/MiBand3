
// Type: MetroLog.Targets.SnapshotFileTarget
// Assembly: MetroLog, Version=0.0.0.0, Culture=neutral, PublicKeyToken=ba4ace74c3b410f3
// MVID: 4C1C3979-5A8D-451B-A09E-C87C946DE5D1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MetroLog.dll

using MetroLog.Layouts;
using System.IO;
using System.Threading.Tasks;

#nullable disable
namespace MetroLog.Targets
{
  public class SnapshotFileTarget : FileTarget
  {
    public SnapshotFileTarget()
      : this((Layout) new FileSnapshotLayout())
    {
    }

    public SnapshotFileTarget(Layout layout)
      : this(layout, "MetroLogs")
    {
    }

    public SnapshotFileTarget(string folderName)
      : this((Layout) new FileSnapshotLayout(), folderName)
    {
    }

    public SnapshotFileTarget(Layout layout, string folderName)
      : base(layout, folderName)
    {
      this.FileNamingParameters.IncludeLevel = true;
      this.FileNamingParameters.IncludeLogger = true;
      this.FileNamingParameters.IncludeSession = false;
      this.FileNamingParameters.IncludeSequence = true;
      this.FileNamingParameters.IncludeTimestamp = FileTimestampMode.DateTime;
      this.FileNamingParameters.CreationMode = FileCreationMode.ReplaceIfExisting;
    }

    protected override async Task WriteTextToFileCore(StreamWriter stream, string contents)
    {
      await stream.WriteAsync(contents);
    }
  }
}
