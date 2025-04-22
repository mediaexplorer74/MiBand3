// Decompiled with JetBrains decompiler
// Type: MiBandApp.Data.Activities.ActivityMode
// Assembly: MiBandApp.Data, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6F72CE9F-BCE5-4931-8D08-920048DF42FA
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.Data.dll

#nullable disable
namespace MiBandApp.Data.Activities
{
  public enum ActivityMode
  {
    Active = 0,
    Walking = 1,
    Running = 2,
    NonWear = 3,
    LightSleep = 4,
    DeepSleep = 5,
    Charging = 6,
    InBed = 7,
    NotWearing = 125, // 0x0000007D
    NoData = 126, // 0x0000007E
  }
}
