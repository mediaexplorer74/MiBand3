// Decompiled with JetBrains decompiler
// Type: MiBand.SDK.Configuration.DisplayItem
// Assembly: MiBand.SDK, Version=0.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 152FA565-3249-4054-A361-01D9C0AFA7F1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBand.SDK.dll

using System;

#nullable disable
namespace MiBand.SDK.Configuration
{
  [Flags]
  public enum DisplayItem : byte
  {
    None = 0,
    Steps = 1,
    Distance = 2,
    Calories = 4,
    HeartRate = 8,
    Battery = 16, // 0x10
  }
}
