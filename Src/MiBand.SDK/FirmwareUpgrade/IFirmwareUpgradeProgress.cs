
// Type: MiBand.SDK.FirmwareUpgrade.IFirmwareUpgradeProgress
// Assembly: MiBand.SDK, Version=0.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 152FA565-3249-4054-A361-01D9C0AFA7F1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBand.SDK.dll

#nullable disable
namespace MiBand.SDK.FirmwareUpgrade
{
  public interface IFirmwareUpgradeProgress
  {
    void ReportUpload(int percents);

    void ReportState(FirmwareUpgradeState firmwareUpgradeState);

    void ReportError(FirmwareUpgradeError firmwareUpgradeError);

    void ReportFirmwareCount(int count, int currentIndex);
  }
}
