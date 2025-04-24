
// Type: MiBand.SDK.Data.RawMinuteActivityData
// Assembly: MiBand.SDK.PCL, Version=0.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 9D9F8E0D-06B7-4D03-9C78-2BA2B5638699
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBand.SDK.PCL.dll

#nullable disable
namespace MiBand.SDK.Data
{
  public struct RawMinuteActivityData
  {
    public int Activity { get; set; }

    public int Mode { get; set; }

    public int Steps { get; set; }

    public int HeartRate { get; set; }

    public override string ToString()
    {
      return string.Join(" ", (object) this.Mode, (object) this.Activity, (object) this.Steps, (object) this.HeartRate);
    }
  }
}
