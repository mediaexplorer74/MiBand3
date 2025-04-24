
// Type: MiBand.SDK.FirmwareUpgrade.Firmware
// Assembly: MiBand.SDK, Version=0.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 152FA565-3249-4054-A361-01D9C0AFA7F1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBand.SDK.dll

using MiBand.SDK.Core;
using MiBand.SDK.Tools;
using System;

#nullable disable
namespace MiBand.SDK.FirmwareUpgrade
{
  public class Firmware
  {
    public Version Version { get; private set; }

    public byte[] Data { get; private set; }

    public Firmware AdditionalFirmware { get; private set; }

    public bool IsValid { get; private set; }

    public bool Seq { get; private set; }

    public byte? FirmwareId { get; private set; }

    public static string GetRecommendedFileName(BandDeviceInfo deviceInfo)
    {
      switch (deviceInfo.Model)
      {
        case MiBandModel.MiBand1:
          return "Mili.fw";
        case MiBandModel.MiBand1A:
          return "Mili_1a.fw";
        case MiBandModel.MiBand1S:
          return "Mili_hr.fw";
        case MiBandModel.MiBand2:
          return deviceInfo.FirmwareVersion < new Version(1, 0, 0, 53) ? "Mili_pro_53.fw" : "Mili_pro.fw";
        default:
          throw new NotImplementedException("Unknown Mi Band version.");
      }
    }

    public static Firmware CreateForBand(BandDeviceInfo deviceInfo, byte[] fileBytes)
    {
      switch (deviceInfo.Model)
      {
        case MiBandModel.MiBand1:
          return Firmware.Initialize1and1aFormat(fileBytes);
        case MiBandModel.MiBand1A:
          return Firmware.Initialize1and1aFormat(fileBytes);
        case MiBandModel.MiBand1S:
          return Firmware.Initialize1sFormat(fileBytes);
        case MiBandModel.MiBand2:
          return Firmware.Initialize2Format(fileBytes, deviceInfo);
        default:
          throw new NotImplementedException("Unknown Mi Band version.");
      }
    }

    public bool IsUpgradeAvailable(BandDeviceInfo bandDeviceInfo)
    {
      bool flag = this.Version > bandDeviceInfo.FirmwareVersion;
      if (this.AdditionalFirmware == null)
        return flag;
      return flag || this.AdditionalFirmware.Version > bandDeviceInfo.Firmware2Version;
    }

    private static Firmware Initialize1and1aFormat(byte[] fileBytes)
    {
      return new Firmware()
      {
        Version = new Version((int) fileBytes[1059], (int) fileBytes[1058], (int) fileBytes[1057], (int) fileBytes[1056]),
        Data = fileBytes,
        IsValid = true
      };
    }

    private static Firmware Initialize1sFormat(byte[] fileBytes)
    {
      Firmware firmware = new Firmware()
      {
        AdditionalFirmware = new Firmware(),
        Version = new Version((int) fileBytes[8], (int) fileBytes[9], (int) fileBytes[10], (int) fileBytes[11])
      };
      firmware.AdditionalFirmware.Version = new Version((int) fileBytes[22], (int) fileBytes[23], (int) fileBytes[24], (int) fileBytes[25]);
      int sourceIndex1 = BytesUtils.BigEndianToInt(fileBytes, 12);
      int length1 = BytesUtils.BigEndianToInt(fileBytes, 16);
      int sourceIndex2 = BytesUtils.BigEndianToInt(fileBytes, 26);
      int length2 = BytesUtils.BigEndianToInt(fileBytes, 30);
      firmware.Data = new byte[length1];
      firmware.AdditionalFirmware.Data = new byte[length2];
      Array.Copy((Array) fileBytes, sourceIndex1, (Array) firmware.Data, 0, length1);
      Array.Copy((Array) fileBytes, sourceIndex2, (Array) firmware.AdditionalFirmware.Data, 0, length2);
      firmware.IsValid = length1 + sourceIndex1 <= fileBytes.Length && length1 > 0;
      firmware.AdditionalFirmware.IsValid = length2 + sourceIndex2 <= fileBytes.Length && length2 > 0;
      firmware.Seq = fileBytes[7] == (byte) 1;
      firmware.FirmwareId = new byte?((byte) 0);
      firmware.AdditionalFirmware.FirmwareId = new byte?((byte) 1);
      return firmware;
    }

    private static Firmware Initialize2Format(byte[] fileBytes, BandDeviceInfo deviceInfo)
    {
      return new Firmware()
      {
        Version = !(deviceInfo.FirmwareVersion < new Version(1, 0, 0, 35)) ? new Version(1, 0, 1, 53) : new Version(1, 0, 0, 53),
        Data = fileBytes,
        IsValid = true
      };
    }
  }
}
