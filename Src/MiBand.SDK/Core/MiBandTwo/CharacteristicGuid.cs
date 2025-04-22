// Decompiled with JetBrains decompiler
// Type: MiBand.SDK.Core.MiBandTwo.CharacteristicGuid
// Assembly: MiBand.SDK, Version=0.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 152FA565-3249-4054-A361-01D9C0AFA7F1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBand.SDK.dll

using System;

#nullable disable
namespace MiBand.SDK.Core.MiBandTwo
{
  internal static class CharacteristicGuid
  {
    public static readonly Guid Time = new Guid("00002a2b-0000-1000-8000-00805f9b34fb");
    public static readonly Guid Settings = new Guid("00000003-0000-3512-2118-0009af100700");
    public static readonly Guid DataSyncControl = new Guid("00000004-0000-3512-2118-0009af100700");
    public static readonly Guid DataSync = new Guid("00000005-0000-3512-2118-0009af100700");
    public static readonly Guid Battery = new Guid("00000006-0000-3512-2118-0009af100700");
    public static readonly Guid RealtimeSteps = new Guid("00000007-0000-3512-2118-0009af100700");
    public static readonly Guid UserInfo = new Guid("00000008-0000-3512-2118-0009af100700");
    public static readonly Guid Auth = new Guid("00000009-0000-3512-2118-0009af100700");
    public static readonly Guid FirmwareUpdateControlPoint = new Guid("00001531-0000-3512-2118-0009af100700");
    public static readonly Guid FirmwareUpdatePacket = new Guid("00001532-0000-3512-2118-0009af100700");
  }
}
