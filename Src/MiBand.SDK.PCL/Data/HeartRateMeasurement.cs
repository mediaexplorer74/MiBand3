
// Type: MiBand.SDK.Data.HeartRateMeasurement
// Assembly: MiBand.SDK.PCL, Version=0.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 9D9F8E0D-06B7-4D03-9C78-2BA2B5638699
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBand.SDK.PCL.dll

using System;

#nullable disable
namespace MiBand.SDK.Data
{
  public class HeartRateMeasurement
  {
    public ushort HeartRateValue { get; set; }

    public DateTimeOffset Timestamp { get; set; }

    public override string ToString()
    {
      return this.HeartRateValue.ToString() + " bpm @ " + (object) this.Timestamp;
    }
  }
}
