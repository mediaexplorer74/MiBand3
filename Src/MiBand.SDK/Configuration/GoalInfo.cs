// Decompiled with JetBrains decompiler
// Type: MiBand.SDK.Configuration.GoalInfo
// Assembly: MiBand.SDK, Version=0.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 152FA565-3249-4054-A361-01D9C0AFA7F1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBand.SDK.dll

#nullable disable
namespace MiBand.SDK.Configuration
{
  public class GoalInfo
  {
    public int StepsGoal { get; set; }

    public int SleepGoalMinutes { get; set; }

    public override int GetHashCode() => 17 * this.StepsGoal + 19 * this.SleepGoalMinutes;

    public override bool Equals(object obj)
    {
      if (obj == null)
        return false;
      GoalInfo goalInfo = obj as GoalInfo;
      return !(goalInfo == (GoalInfo) null) && this.Equals(goalInfo);
    }

    public bool Equals(GoalInfo goalInfo)
    {
      return (object) goalInfo != null && this.StepsGoal == goalInfo.StepsGoal && this.SleepGoalMinutes == goalInfo.SleepGoalMinutes;
    }

    public static bool operator ==(GoalInfo a, GoalInfo b)
    {
      if ((object) a == (object) b)
        return true;
      return (object) a != null && (object) b != null && a.Equals(b);
    }

    public static bool operator !=(GoalInfo a, GoalInfo b) => !(a == b);
  }
}
