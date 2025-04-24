
// Type: MetroLog.Targets.JsonPostWrapper
// Assembly: MetroLog, Version=0.0.0.0, Culture=neutral, PublicKeyToken=ba4ace74c3b410f3
// MVID: 4C1C3979-5A8D-451B-A09E-C87C946DE5D1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MetroLog.dll

using MetroLog.Internal;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace MetroLog.Targets
{
  public class JsonPostWrapper
  {
    public ILoggingEnvironment Environment { get; set; }

    public LogEventInfo[] Events { get; set; }

    internal JsonPostWrapper(ILoggingEnvironment environment, IEnumerable<LogEventInfo> events)
    {
      this.Environment = environment;
      this.Events = events.ToArray<LogEventInfo>();
    }

    internal string ToJson() => SimpleJson.SerializeObject((object) this);
  }
}
