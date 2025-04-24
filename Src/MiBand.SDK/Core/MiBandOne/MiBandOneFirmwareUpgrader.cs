
// Type: MiBand.SDK.Core.MiBandOne.MiBandOneFirmwareUpgrader
// Assembly: MiBand.SDK, Version=0.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 152FA565-3249-4054-A361-01D9C0AFA7F1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBand.SDK.dll

using MiBand.SDK.FirmwareUpgrade;
using MiBand.SDK.Tools;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

#nullable disable
namespace MiBand.SDK.Core.MiBandOne
{
  internal class MiBandOneFirmwareUpgrader
  {
    private readonly MiBand.SDK.Core.MiBandOne.MiBandOne _bluetoothDevice;
    private readonly ILog _log;
    private readonly List<Tuple<Firmware, Version>> _firmwaresToUpgrade = new List<Tuple<Firmware, Version>>();
    private int _firmwaresUpdated;
    private IFirmwareUpgradeProgress _firmwareUpgradeProgress;

    public MiBandOneFirmwareUpgrader(MiBand.SDK.Core.MiBandOne.MiBandOne bluetoothDevice, ILog log)
    {
      this._bluetoothDevice = bluetoothDevice;
      this._log = log;
    }

    public async Task<bool> StartUpgrade(
      Firmware firmware,
      IFirmwareUpgradeProgress firmwareUpgradeProgress)
    {
      this._firmwareUpgradeProgress = firmwareUpgradeProgress;
      await this.PrepareFirmwares(firmware);
      while (this._firmwaresToUpgrade.Count > this._firmwaresUpdated)
      {
        Tuple<Firmware, Version> tuple = this._firmwaresToUpgrade[this._firmwaresUpdated];
        if (!await new MiBandOneFirmwareUploader(this._bluetoothDevice, this._log).UpdateFirmware(tuple.Item1, tuple.Item2, this._firmwareUpgradeProgress).ConfigureAwait(false))
          return false;
        ++this._firmwaresUpdated;
        this._firmwareUpgradeProgress.ReportFirmwareCount(this._firmwaresToUpgrade.Count, this._firmwaresUpdated);
      }
      await this.RebootBand().ConfigureAwait(false);
      return true;
    }

    private async Task PrepareFirmwares(Firmware firmware)
    {
      this._firmwaresToUpgrade.Clear();
      this._firmwaresUpdated = 0;
      this._log.Debug("Preparing firmwares");
      BandDeviceInfo bandDeviceInfo = await this._bluetoothDevice.GetBandDeviceInfo(true).ConfigureAwait(false);
      if (firmware.AdditionalFirmware != null && firmware.AdditionalFirmware.Version >= bandDeviceInfo.Firmware2Version)
      {
        this._log.Debug("Additional firmware found with version " + (object) firmware.AdditionalFirmware.Version);
        this._firmwaresToUpgrade.Add(new Tuple<Firmware, Version>(firmware.AdditionalFirmware, bandDeviceInfo.Firmware2Version));
      }
      if (firmware.Version >= bandDeviceInfo.FirmwareVersion)
      {
        this._log.Debug("Firmware found with version " + (object) firmware.Version);
        this._firmwaresToUpgrade.Add(new Tuple<Firmware, Version>(firmware, bandDeviceInfo.FirmwareVersion));
      }
      this._log.Debug("Updating firmwares: " + (object) this._firmwaresToUpgrade.Count);
      this._firmwareUpgradeProgress.ReportFirmwareCount(this._firmwaresToUpgrade.Count, this._firmwaresUpdated);
    }

    private async Task RebootBand()
    {
      this._firmwareUpgradeProgress.ReportState(FirmwareUpgradeState.Rebooting);
      ConfiguredTaskAwaitable configuredTaskAwaitable = this._bluetoothDevice.Reboot().ConfigureAwait(true);
      await configuredTaskAwaitable;
      configuredTaskAwaitable = Task.Delay(TimeSpan.FromSeconds(10.0)).ConfigureAwait(true);
      await configuredTaskAwaitable;
    }
  }
}
