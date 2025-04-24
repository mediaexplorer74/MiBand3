
// Type: MiBand.SDK.Core.BatteryInfo
// Assembly: MiBand.SDK, Version=0.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 152FA565-3249-4054-A361-01D9C0AFA7F1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBand.SDK.dll

using System;

#nullable disable
namespace MiBand.SDK.Core
{
  public class BatteryInfo
  {
    public int ChargedPercent { get; set; }

    public DateTimeOffset LastCharged { get; set; }

    public DateTimeOffset LastFullCharged { get; set; }

    public int LastChargeLevel { get; set; }

    public int ChargesCount { get; set; }

    public bool IsCharging { get; set; }
  }
}
