
// Type: MiBand.SDK.Core.MiBandTwo.MiBandTwoDataLoader
// Assembly: MiBand.SDK, Version=0.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 152FA565-3249-4054-A361-01D9C0AFA7F1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBand.SDK.dll

using MiBand.SDK.Bluetooth;
using MiBand.SDK.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Foundation;

#nullable disable
namespace MiBand.SDK.Core.MiBandTwo
{
  internal class MiBandTwoDataLoader
  {
    private readonly MiBand.SDK.Core.MiBandTwo.MiBandTwo _device;
    private readonly ILog _log;
    private readonly GattCharacteristic _dataSyncChar;
    private readonly GattCharacteristic _dataSyncControlChar;
    private List<byte[]> _receiveBuffer;

    public MiBandTwoDataLoader(MiBand.SDK.Core.MiBandTwo.MiBandTwo device, ILog log)
    {
      this._device = device;
      this._log = log;
      this._dataSyncChar = this._device.GetCharacteristic(CharacteristicGuid.DataSync);
      this._dataSyncControlChar = this._device.GetCharacteristic(CharacteristicGuid.DataSyncControl);
    }

    public async Task<MiBandTwoDataSyncFragment> GetData(
      DateTimeOffset searchTime,
      MiBandTwoDataLoaderKind kind)
    {
      this._log.Debug(string.Format("{0} from time {1} for kind {2}", (object) nameof (GetData), (object) searchTime, (object) kind));
      try
      {
        byte[] bytes = MiBand2Tools.DateTimeOffsetToBytes(searchTime);
        NotifyResponse notifyResponse = await this._device.WriteCharacteristicGetResponse(this._dataSyncControlChar, ((IEnumerable<byte>) new byte[2]
        {
          (byte) 1,
          (byte) kind
        }).Concat<byte>((IEnumerable<byte>) bytes).ToArray<byte>()).ConfigureAwait(false);
        if (notifyResponse == null || !notifyResponse.IsSuccessCommand(1))
        {
          this._log.Warning(string.Format("Not successfull command when getting data header for {0}.", (object) kind));
          return (MiBandTwoDataSyncFragment) null;
        }
        byte[] payload = notifyResponse.Payload;
        int itemsCount = (int) payload[0] | (int) payload[1] << 8 | (int) payload[2] << 16 | (int) payload[3] << 24;
        MiBandTwoDataSyncFragment fragment = new MiBandTwoDataSyncFragment(MiBand2Tools.DateTimeOffsetFromBytes(payload, 4), itemsCount);
        this._log.Debug(string.Format("Fragment info. Items: {0}, start time: {1}", (object) fragment.ItemsCount, (object) fragment.StartTime));
        if (fragment.ItemsCount == 0)
          return fragment;
        try
        {
          await this.StartDataSync().ConfigureAwait(false);
        }
        finally
        {
          await this.FinishDataSync().ConfigureAwait(false);
        }
        lock (this._receiveBuffer)
          fragment.RawDataLines.AddRange((IEnumerable<byte[]>) this._receiveBuffer);
        return fragment;
      }
      catch (Exception ex)
      {
        this._log.Warning("Exception while loading data: " + (object) ex);
        return (MiBandTwoDataSyncFragment) null;
      }
    }

        private async Task StartDataSync()
        {
            this._log.Debug("Starting data sync.");
            this._receiveBuffer = new List<byte[]>();

            // Subscribe to the ValueChanged event using a lambda expression
            this._dataSyncChar.ValueChanged += (sender, args) => DataSyncCharOnValueChanged(sender, args);

            await this._device.EnableNotifications(this._dataSyncChar).ConfigureAwait(false);
            NotifyResponse notifyResponse = await this._device.WriteCharacteristicGetResponse(this._dataSyncControlChar, new byte[1]
            {
                (byte) 2
            }, TimeSpan.FromSeconds(20.0)).ConfigureAwait(false);

            if (notifyResponse == null)
            {
                this._log.Error("Sync waiting timeout or incorrect response.");
                throw new TimeoutException("Synchronization timed out");
            }
            if (!notifyResponse.IsSuccessCommand(2))
                throw new Exception("Sync fragment command returned fail result.");
        }

        private async Task FinishDataSync()
        {
            this._log.Debug(string.Format("Finished data sync. Buffer size = {0}", (object)this._receiveBuffer.Count));
            await this._device.DisableNotifications(this._dataSyncChar).ConfigureAwait(false);

            // Unsubscribe from the ValueChanged event
            this._dataSyncChar.ValueChanged -= (sender, args) => DataSyncCharOnValueChanged(sender, args);

            NotifyResponse notifyResponse = await this._device.WriteCharacteristicGetResponse(this._dataSyncControlChar, new byte[1]
            {
                (byte) 3
            }).ConfigureAwait(false);
        }

    private void DataSyncCharOnValueChanged(
      GattCharacteristic sender,
      GattValueChangedEventArgs args)
    {
      lock (this._receiveBuffer)
        this._receiveBuffer.Add(args.CharacteristicValue.ToArraySafe());
    }
  }
}
