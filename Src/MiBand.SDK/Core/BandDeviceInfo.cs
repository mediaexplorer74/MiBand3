// Decompiled with JetBrains decompiler
// Type: MiBand.SDK.Core.BandDeviceInfo
// Assembly: MiBand.SDK, Version=0.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 152FA565-3249-4054-A361-01D9C0AFA7F1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBand.SDK.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#nullable disable
namespace MiBand.SDK.Core
{
  public class BandDeviceInfo
  {
    public Version FirmwareVersion { get; private set; }

    public Version Firmware2Version { get; private set; }

    public Version ProfileVersion { get; private set; }

    public byte Feature { get; private set; }

    public byte Appearance { get; private set; }

    public byte Hardware { get; private set; }

    public Capability Capabilities { get; private set; }

    public bool HasFirmware2Version { get; private set; }

    public MiBandModel Model { get; private set; }

    public int AlarmsCount { get; private set; }

    public static BandDeviceInfo FromMiBand1Bytes(byte[] bytes)
    {
      BandDeviceInfo bandDeviceInfo = new BandDeviceInfo()
      {
        FirmwareVersion = new Version(string.Join<byte>(".", ((IEnumerable<byte>) bytes).Skip<byte>(12).Take<byte>(4).Reverse<byte>())),
        Feature = bytes[4],
        Appearance = bytes[5],
        Hardware = bytes[6],
        ProfileVersion = new Version(string.Join<byte>(".", ((IEnumerable<byte>) bytes).Skip<byte>(8).Take<byte>(4).Reverse<byte>())),
        Model = MiBandModel.MiBand1,
        Capabilities = Capability.SmartAlarm | Capability.LedColors | Capability.Reboot | Capability.NoAuthToVibrate,
        AlarmsCount = 3
      };
      if (bandDeviceInfo.Feature == (byte) 5 && bandDeviceInfo.Appearance == (byte) 0 || bandDeviceInfo.Hardware == (byte) 3)
      {
        bandDeviceInfo.Model = MiBandModel.MiBand1A;
        bandDeviceInfo.Capabilities &= ~Capability.LedColors;
      }
      bandDeviceInfo.HasFirmware2Version = bytes.Length == 20;
      if (bandDeviceInfo.HasFirmware2Version)
      {
        bandDeviceInfo.Firmware2Version = new Version(string.Join<byte>(".", ((IEnumerable<byte>) bytes).Skip<byte>(16).Take<byte>(4).Reverse<byte>()));
        bandDeviceInfo.Model = MiBandModel.MiBand1S;
        bandDeviceInfo.Capabilities &= ~Capability.LedColors;
        bandDeviceInfo.Capabilities |= Capability.HeartRate;
      }
      return bandDeviceInfo;
    }

    public static BandDeviceInfo FromMiBand2Bytes(byte[] softwareVersion)
    {
      string str = Encoding.UTF8.GetString(softwareVersion, 0, softwareVersion.Length);
      BandDeviceInfo bandDeviceInfo = new BandDeviceInfo()
      {
        FirmwareVersion = new Version(str.Substring(1)),
        Model = MiBandModel.MiBand2,
        Capabilities = Capability.HeartRate | Capability.FlipDisplayOnWristRotate | Capability.WristLiftHighlight | Capability.ActivityReminder | Capability.DisplayItems | Capability.NotDisturbMode | Capability.DateDisplay | Capability.GoalReachedNotificationConfigurable,
        AlarmsCount = 10
      };
      if (bandDeviceInfo.FirmwareVersion >= new Version(1, 0, 0, 53))
        bandDeviceInfo.Capabilities |= Capability.AlertNotifyIcon;
      return bandDeviceInfo;
    }
  }
}
