
// Type: MiBand.SDK.Core.MiBandTwo.MiBandTwoDataSyncFragment
// Assembly: MiBand.SDK, Version=0.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 152FA565-3249-4054-A361-01D9C0AFA7F1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBand.SDK.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace MiBand.SDK.Core.MiBandTwo
{
  internal class MiBandTwoDataSyncFragment
  {
    public MiBandTwoDataSyncFragment(DateTimeOffset startTime, int itemsCount)
    {
      this.StartTime = startTime;
      this.ItemsCount = itemsCount;
    }

    public DateTimeOffset StartTime { get; }

    public int ItemsCount { get; }

    public List<byte[]> RawDataLines { get; private set; } = new List<byte[]>();
  }
}
