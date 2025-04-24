
// Type: MiBand.SDK.Core.Capability
// Assembly: MiBand.SDK, Version=0.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 152FA565-3249-4054-A361-01D9C0AFA7F1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBand.SDK.dll

using System;

#nullable disable
namespace MiBand.SDK.Core
{
  [Flags]
  public enum Capability
  {
    None = 0,
    HeartRate = 1,
    SmartAlarm = 2,
    AlertNotifyIcon = 4,
    LedColors = 8,
    Reboot = 16, // 0x00000010
    FlipDisplayOnWristRotate = 32, // 0x00000020
    WristLiftHighlight = 64, // 0x00000040
    ActivityReminder = 128, // 0x00000080
    DisplayItems = 256, // 0x00000100
    NoAuthToVibrate = 512, // 0x00000200
    NotDisturbMode = 1024, // 0x00000400
    DateDisplay = 2048, // 0x00000800
    GoalReachedNotificationConfigurable = 4096, // 0x00001000
  }
}
