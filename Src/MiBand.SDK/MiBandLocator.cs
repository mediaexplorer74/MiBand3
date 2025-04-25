
// Type: MiBand.SDK.MiBandLocator
// Assembly: MiBand.SDK, Version=0.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 152FA565-3249-4054-A361-01D9C0AFA7F1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBand.SDK.dll

using MiBand.SDK.Bluetooth;
using MiBand.SDK.Core;
using MiBand.SDK.Core.MiBandOne;
using MiBand.SDK.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;

#nullable disable
namespace MiBand.SDK
{
  public class MiBandLocator
  {
    private readonly ILog _log;

    public MiBandLocator(ILog log = null) => this._log = log ?? (ILog) new LogStub();

    public async Task<IEnumerable<IMiBandInfo>> FindMiBands()
    {
      this._log.Debug("Trying to find all devices");
      DeviceInformationCollection source =
                await DeviceInformation.FindAllAsync(
                    GattDeviceService.GetDeviceSelectorFromUuid(ServiceUuid.Mili))
                .AsTask<DeviceInformationCollection>().ConfigureAwait(false);
      this._log.Debug(string.Format("Found {0} devices", 
          (object) ((IReadOnlyCollection<DeviceInformation>) source).Count));
      return (IEnumerable<IMiBandInfo>) ((IEnumerable<DeviceInformation>) source)
                .Select<DeviceInformation, MiBandInfo>( t => new MiBandInfo(t.Name, t.Id) );
    }

    public async Task<IMiBand> CreateMiBand(IMiBandInfo miBandInfo)
    {
      return (IMiBand) await this.FindDeviceInternal(miBandInfo.DeviceId)
                .ConfigureAwait(false);
    }

    private async Task<MiBandBase> FindDeviceInternal(string knownDeviceId)
    {
      GattDeviceService gattService = await this.FindBandGattService(knownDeviceId)
                .ConfigureAwait(false);

      if (gattService == null)
        return (MiBandBase) null;
      return 
          gattService.GetCharacteristics(
              MiBand.SDK.Core.MiBandOne.CharacteristicGuid.DeviceInfo ).Count == 1
          ? await this.CreateMiBand1(gattService).ConfigureAwait(false)
          : this.CreateMiBand2(gattService);
    }

    private MiBandBase CreateMiBand2(GattDeviceService gattService)
    {
      this._log.Debug("Creating Mi Band 2.");
      return (MiBandBase) new MiBand.SDK.Core.MiBandTwo.MiBandTwo(gattService, this._log);
    }

    private async Task<MiBandBase> CreateMiBand1(GattDeviceService gattService)
    {
      byte[] arraySafe = (await gattService.GetCharacteristics(
          MiBand.SDK.Core.MiBandOne.CharacteristicGuid.DeviceInfo)[0]
          .ReadValueAsync((BluetoothCacheMode) 1)).Value.ToArraySafe();

      if (arraySafe == null)
        return (MiBandBase) null;

      BandDeviceInfo bandDeviceInfo = BandDeviceInfo.FromMiBand1Bytes(arraySafe);
      this._log.Debug("Creating Mi Band 1. Firmware: " + (object) bandDeviceInfo.FirmwareVersion);
      switch (bandDeviceInfo.FirmwareVersion.Major)
      {
        case 1:
          return (MiBandBase) new MiBand.SDK.Core.MiBandOne.MiBandOne(gattService, this._log);
        case 4:
          return (MiBandBase) new MiBandOneS(gattService, this._log);
        case 5:
          return (MiBandBase) new MiBandOneA(gattService, this._log);
        default:
          throw new MiBandNotSupportedException();
      }
    }

    private async Task<GattDeviceService> FindBandGattService(string knownDeviceId)
    {
      this._log.Debug(string.Format("Looking for known device id: {0}",  knownDeviceId));
      GattDeviceService bandGattService = null;

      try
      {
        bandGattService = await this.GetGattDeviceServiceForDeviceId(knownDeviceId)
                    .ConfigureAwait(false);
      }
      catch (Exception ex)
      {
        Debug.WriteLine(ex.Message);
      }

      if (bandGattService != null)
      {
        this._log.Debug("Gatt device found");
        return bandGattService;
      }
      this._log.Warning(string.Format("Gatt device not found by id: {0}",
          knownDeviceId));
      return null;
    }

    private async Task<GattDeviceService> GetGattDeviceServiceForDeviceId(string deviceId)
    {
      GattDeviceService serviceForDeviceId = null;
      try
      {
        //return
        serviceForDeviceId  = await GattDeviceService.FromIdAsync(deviceId)
                    .AsTask<GattDeviceService>().ConfigureAwait(false);
      }
      catch (Exception ex)
      {
        this._log.Info(string.Format("Not found gatt service for device id {0} with {1}",
            deviceId, ex));
        return await Task.FromResult<GattDeviceService>(null);
      }
       
      //GattDeviceService serviceForDeviceId;
      return serviceForDeviceId;
      //return await Task.FromResult<GattDeviceService>((GattDeviceService)null);
    }
  }
}
