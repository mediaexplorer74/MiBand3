// Decompiled with JetBrains decompiler
// Type: MiBand.SDK.Configuration.DaysOfWeek
// Assembly: MiBand.SDK, Version=0.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 152FA565-3249-4054-A361-01D9C0AFA7F1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBand.SDK.dll

using System;

#nullable disable
namespace MiBand.SDK.Configuration
{
  [Flags]
  public enum DaysOfWeek
  {
    None = 0,
    Monday = 1,
    Tuesday = 2,
    Wednesday = 4,
    Thursday = 8,
    Friday = 16, // 0x00000010
    Saturday = 32, // 0x00000020
    Sunday = 64, // 0x00000040
    Weekdays = Friday | Thursday | Wednesday | Tuesday | Monday, // 0x0000001F
    Weekends = Sunday | Saturday, // 0x00000060
    All = Weekends | Weekdays, // 0x0000007F
  }
}
