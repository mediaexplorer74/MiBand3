
// Type: MiBand.SDK.Core.MiBandOne.MiBandOneS
// Assembly: MiBand.SDK, Version=0.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 152FA565-3249-4054-A361-01D9C0AFA7F1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBand.SDK.dll

using MiBand.SDK.Bluetooth;
using MiBand.SDK.Data;
using MiBand.SDK.HeartRate;
using MiBand.SDK.Tools;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

#nullable disable
namespace MiBand.SDK.Core.MiBandOne
{
  internal class MiBandOneS(GattDeviceService gattService, ILog log) : MiBandOneA(gattService, log)
  {
    public override async Task<HeartRateMeasurement> GetHeartRate()
    {
      HeartRateMeasurement heartRate = await new HeartRateLoader(this.Log, (BluetoothDeviceBase) this).GetHeartRate().ConfigureAwait(false);
      if (heartRate == null)
        return heartRate;
      this.Log.Debug("HR: " + (object) heartRate);
      return heartRate.HeartRateValue != (ushort) 0 ? heartRate : (HeartRateMeasurement) null;
    }

    public override async Task SetHeartRateDuringSleep(bool enabled)
    {
      byte[] source = new byte[3]
      {
        (byte) 21,
        (byte) 0,
        enabled ? (byte) 1 : (byte) 0
      };
      GattCommunicationStatus communicationStatus = await this.GetCharacteristic(GattCharacteristicUuids.HeartRateControlPoint, this.GetSecondaryService(GattServiceUuids.HeartRate)).WriteValueAsync(source.AsBuffer()).AsTask<GattCommunicationStatus>().ConfigureAwait(false);
    }
  }
}
