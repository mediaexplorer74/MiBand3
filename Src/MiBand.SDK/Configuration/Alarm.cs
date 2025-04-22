// Decompiled with JetBrains decompiler
// Type: MiBand.SDK.Configuration.Alarm
// Assembly: MiBand.SDK, Version=0.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 152FA565-3249-4054-A361-01D9C0AFA7F1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBand.SDK.dll

using System;

#nullable disable
namespace MiBand.SDK.Configuration
{
  public sealed class Alarm
  {
    public TimeSpan Time { get; set; } = new TimeSpan(7, 0, 0);

    public DaysOfWeek Days { get; set; }

    public bool IsEnabled { get; set; }

    public bool IsSmart { get; set; }

    public override int GetHashCode()
    {
      return 7 * this.Time.GetHashCode() + 13 * ((int) this.Days).GetHashCode() + 23 * this.IsEnabled.GetHashCode() + 3 * this.IsSmart.GetHashCode();
    }

    protected bool Equals(Alarm other)
    {
      return this.Days == other.Days && this.Time.Equals(other.Time) && this.IsEnabled == other.IsEnabled && this.IsSmart == other.IsSmart;
    }

    public override bool Equals(object obj)
    {
      if (obj == null)
        return false;
      if ((object) this == obj)
        return true;
      return obj.GetType() == this.GetType() && this.Equals((Alarm) obj);
    }

    public static bool operator ==(Alarm left, Alarm right)
    {
      return object.Equals((object) left, (object) right);
    }

    public static bool operator !=(Alarm left, Alarm right)
    {
      return !object.Equals((object) left, (object) right);
    }
  }
}
