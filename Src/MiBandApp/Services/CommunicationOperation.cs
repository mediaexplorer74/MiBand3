
// Type: MiBandApp.Services.CommunicationOperation
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// 

using System;

#nullable disable
namespace MiBandApp.Services
{
  [Flags]
  public enum CommunicationOperation
  {
    None = 0,
    Refreshing = 1,
    Connecting = 2,
    UpdatingSteps = 4,
    UpdatingBattery = 8,
    Syncing = 16, // 0x00000010
    MeasuringHeartRate = 32, // 0x00000020
  }
}
