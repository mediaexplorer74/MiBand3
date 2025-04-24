
// Type: MetroLog.Targets.StreamingFileTarget
// Assembly: MetroLog, Version=0.0.0.0, Culture=neutral, PublicKeyToken=ba4ace74c3b410f3
// MVID: 4C1C3979-5A8D-451B-A09E-C87C946DE5D1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MetroLog.dll

using MetroLog.Layouts;
using System.IO;
using System.Threading.Tasks;

#nullable disable
namespace MetroLog.Targets
{
  public class StreamingFileTarget : FileTarget
  {
    public StreamingFileTarget()
      : this((Layout) new SingleLineLayout())
    {
    }

    public StreamingFileTarget(Layout layout)
      : base(layout)
    {
      this.FileNamingParameters.IncludeLevel = false;
      this.FileNamingParameters.IncludeLogger = false;
      this.FileNamingParameters.IncludeSequence = false;
      this.FileNamingParameters.IncludeSession = false;
      this.FileNamingParameters.IncludeTimestamp = FileTimestampMode.Date;
      this.FileNamingParameters.CreationMode = FileCreationMode.AppendIfExisting;
    }

    protected override Task WriteTextToFileCore(StreamWriter file, string contents)
    {
      return file.WriteLineAsync(contents);
    }
  }
}
