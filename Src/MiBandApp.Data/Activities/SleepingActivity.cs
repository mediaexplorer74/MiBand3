// Decompiled with JetBrains decompiler
// Type: MiBandApp.Data.Activities.SleepingActivity
// Assembly: MiBandApp.Data, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6F72CE9F-BCE5-4931-8D08-920048DF42FA
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.Data.dll

using System;
using System.Text;

#nullable disable
namespace MiBandApp.Data.Activities
{
  public class SleepingActivity : IUserActivity
  {
    public DateTime BeginSleep { get; set; }

    public DateTime EndSleep { get; set; }

    public DateTime Begin { get; set; }

    public DateTime End { get; set; }

    public int Awakenings { get; set; }

    public string SleepPatternString { get; set; }

    public int DeepSleepMin { get; set; }

    public string HeartRateString { get; set; }

    public virtual int TotalSleepMinutes => (int) (this.EndSleep - this.BeginSleep).TotalMinutes;

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("Begin in bed: " + (object) this.Begin);
      stringBuilder.AppendLine("Begin sleep: " + (object) this.BeginSleep);
      stringBuilder.AppendLine("Stop sleep: " + (object) this.EndSleep);
      stringBuilder.AppendLine("Stop in bed: " + (object) this.End);
      stringBuilder.AppendLine("Awakenings: " + (object) this.Awakenings);
      stringBuilder.AppendLine("Deep sleep: " + (object) this.DeepSleepMin);
      stringBuilder.AppendLine("Pattern: " + this.SleepPatternString);
      return stringBuilder.ToString();
    }
  }
}
