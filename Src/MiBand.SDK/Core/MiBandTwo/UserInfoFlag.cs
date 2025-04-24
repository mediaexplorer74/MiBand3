
// Type: MiBand.SDK.Core.MiBandTwo.UserInfoFlag
// Assembly: MiBand.SDK, Version=0.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 152FA565-3249-4054-A361-01D9C0AFA7F1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBand.SDK.dll

using System;

#nullable disable
namespace MiBand.SDK.Core.MiBandTwo
{
  [Flags]
  internal enum UserInfoFlag : sbyte
  {
    WriteOperation = 0,
    Birth = 1,
    Gender = 2,
    Height = 4,
    Weight = 8,
    Goal = 16, // 0x10
    Location = 32, // 0x20
    Uid = 64, // 0x40
    Base = Uid | Weight | Height | Gender | Birth, // 0x4F
    All = Base | Location | Goal, // 0x7F
  }
}
