
// Type: MiBand.SDK.Core.MiBandTwo.MiBandTwoFirmwareUploader
// Assembly: MiBand.SDK, Version=0.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 152FA565-3249-4054-A361-01D9C0AFA7F1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBand.SDK.dll

using MiBand.SDK.Bluetooth;
using MiBand.SDK.FirmwareUpgrade;
using MiBand.SDK.Tools;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

#nullable disable
namespace MiBand.SDK.Core.MiBandTwo
{
  internal class MiBandTwoFirmwareUploader
  {
    private readonly MiBand.SDK.Core.MiBandTwo.MiBandTwo _bluetoothDevice;
    private readonly ILog _log;
    private readonly GattCharacteristic _controlPointCharacteristic;
    private readonly GattCharacteristic _packetCharacteristic;
    private IFirmwareUpgradeProgress _firmwareUpgradeProgress;
    private Firmware _firmware;

    public MiBandTwoFirmwareUploader(MiBand.SDK.Core.MiBandTwo.MiBandTwo bluetoothDevice, ILog log)
    {
      this._bluetoothDevice = bluetoothDevice;
      this._log = log;
      GattDeviceService secondaryService = this._bluetoothDevice.GetSecondaryService(ServiceUuid.DeviceFirmwareUpdate);
      this._controlPointCharacteristic = this._bluetoothDevice.GetCharacteristic(CharacteristicGuid.FirmwareUpdateControlPoint, secondaryService);
      this._packetCharacteristic = this._bluetoothDevice.GetCharacteristic(CharacteristicGuid.FirmwareUpdatePacket, secondaryService);
    }

    public async Task<bool> UpdateFirmware(
      Firmware newFirmware,
      FirmwareType firmwareType,
      IFirmwareUpgradeProgress firmwareUpgradeProgress)
    {
      this._log.Info("Starting firmware upgrade to " + (object) newFirmware.Version);
      this._firmware = newFirmware;
      this._firmwareUpgradeProgress = firmwareUpgradeProgress;
      this._firmwareUpgradeProgress.ReportState(FirmwareUpgradeState.Initializing);
      try
      {
        await this.SendInitCommand(newFirmware.Data.Length, firmwareType).ConfigureAwait(false);
        TaskCompletionSource<bool> transmissionCompleteTcs = new TaskCompletionSource<bool>();
        Task receiveFirmwareCommandTask = this.SendReceiveFirmware(transmissionCompleteTcs);
        ConfiguredTaskAwaitable configuredTaskAwaitable = Task.Delay(1000).ConfigureAwait(false);
        await configuredTaskAwaitable;
        int hash = await this.SendFirmwareData(transmissionCompleteTcs).ConfigureAwait(false);
        configuredTaskAwaitable = receiveFirmwareCommandTask.ConfigureAwait(false);
        await configuredTaskAwaitable;
        configuredTaskAwaitable = this.ValidateFirmware(hash).ConfigureAwait(false);
        await configuredTaskAwaitable;
        configuredTaskAwaitable = this.SendRebootCommand(firmwareType).ConfigureAwait(false);
        await configuredTaskAwaitable;
        this._firmwareUpgradeProgress.ReportState(FirmwareUpgradeState.Complete);
        return true;
      }
      catch (Exception ex)
      {
        this._log.Error(ex.ToString());
        this._firmwareUpgradeProgress.ReportState(FirmwareUpgradeState.Failed);
        return false;
      }
    }

    private async Task SendInitCommand(int firmwareSize, FirmwareType firmwareType)
    {
      byte[] data = new byte[firmwareType == FirmwareType.Firmware ? 4 : 5];
      data[0] = (byte) 1;
      data[1] = (byte) firmwareSize;
      data[2] = (byte) (firmwareSize >> 8);
      data[3] = (byte) (firmwareSize >> 16);
      if (firmwareType != FirmwareType.Firmware)
        data[4] = (byte) firmwareType;
      NotifyResponse response = await this._bluetoothDevice.WriteCharacteristicGetResponse(this._controlPointCharacteristic, data, TimeSpan.FromSeconds(10.0));
      if (response == null || !response.IsSuccessCommand(1))
      {
        this._firmwareUpgradeProgress.ReportError(FirmwareUpgradeError.InitialCheckFailed);
        throw new InvalidOperationException(string.Format("{0} failed with code: {1}", (object) nameof (SendInitCommand), (object) response));
      }
    }

    private async Task SendReceiveFirmware(TaskCompletionSource<bool> transmissionCompleteTcs)
    {
      NotifyResponse response = await this._bluetoothDevice.WriteCharacteristicGetResponse(this._controlPointCharacteristic, new byte[1]
      {
        (byte) 3
      }, (Task) transmissionCompleteTcs.Task);
      if (response == null || !response.IsSuccessCommand(3))
      {
        this._firmwareUpgradeProgress.ReportError(FirmwareUpgradeError.PostCheckFailed);
        throw new InvalidOperationException(string.Format("{0} failed with code: {1}", (object) nameof (SendReceiveFirmware), (object) response));
      }
    }

    private async Task SendRebootCommand(FirmwareType firmwareType)
    {
      if (firmwareType != FirmwareType.Firmware)
        return;
      this._firmwareUpgradeProgress.ReportState(FirmwareUpgradeState.Rebooting);
      NotifyResponse response = await this._bluetoothDevice.WriteCharacteristicGetResponse(this._controlPointCharacteristic, new byte[1]
      {
        (byte) 5
      }, TimeSpan.FromSeconds(5.0));
      if (response == null || !response.IsSuccessCommand(5))
      {
        this._firmwareUpgradeProgress.ReportError(FirmwareUpgradeError.CommunicationError);
        throw new InvalidOperationException(string.Format("{0} failed with code: {1}", (object) nameof (SendRebootCommand), (object) response));
      }
    }

    private async Task<int> SendFirmwareData(TaskCompletionSource<bool> transmissionCompleteTcs)
    {
      this._firmwareUpgradeProgress.ReportState(FirmwareUpgradeState.TransmittingData);
      byte[] packetBytes = new byte[20];
      int sentBytesCount = 0;
      int sentPacketsCount = 0;
      int hash = (int) ushort.MaxValue;
      try
      {
        int prevReportedValue = -1;
        int leftToSend;
        while ((leftToSend = this._firmware.Data.Length - sentBytesCount) > 0)
        {
          int currentPacketLength = leftToSend >= 20 ? 20 : leftToSend;
          if (packetBytes.Length != currentPacketLength)
            packetBytes = new byte[currentPacketLength];
          Array.Copy((Array) this._firmware.Data, sentBytesCount, (Array) packetBytes, 0, currentPacketLength);
          hash = MiBandTwoFirmwareUploader.UpdateHashByPacket(hash, packetBytes);
          GattCommunicationStatus communicationStatus1 = await this._packetCharacteristic.WriteValueAsync(packetBytes.AsBuffer(), (GattWriteOption) 1).AsTask<GattCommunicationStatus>().ConfigureAwait(false);
          sentBytesCount += currentPacketLength;
          ++sentPacketsCount;
          if (sentPacketsCount > 0 && sentPacketsCount % 100 == 0 || sentBytesCount >= this._firmware.Data.Length)
          {
            GattCommunicationStatus communicationStatus2 = await this._controlPointCharacteristic.WriteValueAsync(new byte[1].AsBuffer()).AsTask<GattCommunicationStatus>().ConfigureAwait(false);
          }
          int percents = 100 * sentBytesCount / this._firmware.Data.Length;
          if (percents != prevReportedValue)
          {
            prevReportedValue = percents;
            this._firmwareUpgradeProgress.ReportUpload(percents);
          }
        }
      }
      catch (Exception ex)
      {
        this._log.Error("Exception while loading firmware: " + (object) ex);
        this._firmwareUpgradeProgress.ReportError(FirmwareUpgradeError.CommunicationError);
        throw;
      }
      Task.Delay(5000).ContinueWith<bool>((Func<Task, bool>)
          (t => transmissionCompleteTcs.TrySetResult(true)));
      return hash;
    }

    private async Task ValidateFirmware(int hash)
    {
      this._firmwareUpgradeProgress.ReportState(FirmwareUpgradeState.Validating);
      NotifyResponse response = await this._bluetoothDevice.WriteCharacteristicGetResponse(this._controlPointCharacteristic, new byte[3]
      {
        (byte) 4,
        (byte) hash,
        (byte) (hash >> 8)
      });
      if (response == null || !response.IsSuccessCommand(4))
      {
        this._firmwareUpgradeProgress.ReportError(FirmwareUpgradeError.PostCheckFailed);
        throw new InvalidOperationException(string.Format("{0} failed with code: {1}", (object) nameof (ValidateFirmware), (object) response));
      }
    }

    private static int UpdateHashByPacket(int hash, byte[] packetBytes)
    {
      for (int index = 0; index < packetBytes.Length; ++index)
      {
        int num1 = (hash >> 8 | hash << 8) & (int) ushort.MaxValue ^ (int) packetBytes[index] & (int) byte.MaxValue;
        int num2 = num1 ^ (num1 & (int) byte.MaxValue) >> 4;
        int num3 = num2 ^ num2 << 12 & (int) ushort.MaxValue;
        hash = num3 ^ (num3 & (int) byte.MaxValue) << 5 & (int) ushort.MaxValue;
      }
      return hash;
    }
  }
}
