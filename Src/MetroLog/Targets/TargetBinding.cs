
// Type: MetroLog.Targets.TargetBinding
// Assembly: MetroLog, Version=0.0.0.0, Culture=neutral, PublicKeyToken=ba4ace74c3b410f3
// MVID: 4C1C3979-5A8D-451B-A09E-C87C946DE5D1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MetroLog.dll

using System.Diagnostics;

#nullable disable
namespace MetroLog.Targets
{
  [DebuggerDisplay("Name = {Target.GetType().Name}, Min = {MinLevel}, Max = {MaxLevel}")]
  internal class TargetBinding
  {
    private LogLevel MinLevel { get; }

    private LogLevel MaxLevel { get; }

    internal Target Target { get; }

    internal TargetBinding(LogLevel min, LogLevel max, Target target)
    {
      this.MinLevel = min;
      this.MaxLevel = max;
      this.Target = target;
    }

    internal bool SupportsLevel(LogLevel level) => level >= this.MinLevel && level <= this.MaxLevel;
  }
}
