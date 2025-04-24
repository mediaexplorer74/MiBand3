
// Type: MiBandApp.Data.Activities.ActivityMinuteData
// Assembly: MiBandApp.Data, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6F72CE9F-BCE5-4931-8D08-920048DF42FA
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.Data.dll

using MiBand.SDK.Data;
using System;

#nullable disable
namespace MiBandApp.Data.Activities
{
  public class ActivityMinuteData
  {
    public ActivityMinuteData(RawMinuteActivityData rawMinuteActivityData, DateTimeOffset timestamp)
      : this(timestamp)
    {
      this.Activity = rawMinuteActivityData.Activity;
      this.Steps = rawMinuteActivityData.Steps;
      this.HeartRate = rawMinuteActivityData.HeartRate == (int) byte.MaxValue ? 0 : rawMinuteActivityData.HeartRate;
      this.Runs = rawMinuteActivityData.Mode >> 4;
      this.Mode = (ActivityMode) (rawMinuteActivityData.Mode & 15);
      if (this.Mode != ActivityMode.DeepSleep && this.Mode != ActivityMode.LightSleep || this.Activity <= 15)
        return;
      this.Mode = ActivityMode.Active;
    }

    public ActivityMinuteData(DateTimeOffset timestamp)
    {
      this.Timestamp = timestamp.DateTime.AddSeconds((double) -timestamp.Second);
      this.Mode = ActivityMode.NoData;
    }

    public ActivityMinuteData()
      : this((DateTimeOffset) DateTime.Now)
    {
    }

    public virtual DateTime Timestamp { get; set; }

    public ActivityMode Mode { get; set; }

    public int Activity { get; set; }

    public int Steps { get; set; }

    public int Runs { get; set; }

    public int HeartRate { get; set; }
  }
}
