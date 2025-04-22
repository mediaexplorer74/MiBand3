// Decompiled with JetBrains decompiler
// Type: MiBand.SDK.Core.MiBandOne.MiBandOneFirmwareUploader
// Assembly: MiBand.SDK, Version=0.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 152FA565-3249-4054-A361-01D9C0AFA7F1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBand.SDK.dll

using MiBand.SDK.FirmwareUpgrade;
using MiBand.SDK.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

#nullable disable
namespace MiBand.SDK.Core.MiBandOne
{
  internal class MiBandOneFirmwareUploader
  {
    private readonly MiBand.SDK.Core.MiBandOne.MiBandOne _bluetoothDevice;
    private readonly ILog _log;
    private readonly TaskCompletionSource<bool> _taskCompletionSource = new TaskCompletionSource<bool>();
    private readonly Watchdog _watchdog = new Watchdog(TimeSpan.FromSeconds(30.0));
    private readonly CharacteristicWatcher _notifyCharacteristicWatcher;
    private readonly GattCharacteristic _controlPointCharacteristic;
    private readonly GattCharacteristic _firmwareDataCharacteristic;
    private IFirmwareUpgradeProgress _firmwareUpgradeProgress;
    private Version _oldFirmwareVersion;
    private Firmware _firmware;
    private bool _dataTransmitted;

    public MiBandOneFirmwareUploader(MiBand.SDK.Core.MiBandOne.MiBandOne bluetoothDevice, ILog log)
    {
      this._bluetoothDevice = bluetoothDevice;
      this._log = log;
      this._notifyCharacteristicWatcher = new CharacteristicWatcher(this._bluetoothDevice.GetCharacteristic(CharacteristicGuid.Notify));
      this._notifyCharacteristicWatcher.ValueChanged += (EventHandler<CharacteristicWatcher.ValueChangedEventArgs>) ((sender, args) => this.OnNotification(args.Value));
      this._controlPointCharacteristic = this._bluetoothDevice.GetCharacteristic(CharacteristicGuid.ControlPoint);
      this._firmwareDataCharacteristic = this._bluetoothDevice.GetCharacteristic(CharacteristicGuid.FirmwareData);
    }

    public async Task<bool> UpdateFirmware(
      Firmware newFirmware,
      Version oldFirmwareVersion,
      IFirmwareUpgradeProgress firmwareUpgradeProgress)
    {
      this._log.Info("Starting firmware upgrade from " + (object) oldFirmwareVersion + " to " + (object) newFirmware.Version);
      this._firmware = newFirmware;
      this._oldFirmwareVersion = oldFirmwareVersion;
      this._firmwareUpgradeProgress = firmwareUpgradeProgress;
      this._firmwareUpgradeProgress.ReportState(FirmwareUpgradeState.Initializing);
      try
      {
        this._watchdog.Enable();
        this._notifyCharacteristicWatcher.Enable(new byte[1]);
        await this.SendFirmwareInfo().ConfigureAwait(false);
        Task task = await Task.WhenAny((Task) this._taskCompletionSource.Task, this._watchdog.Task).ConfigureAwait(false);
        this._watchdog.Disable();
        if (this._watchdog.HasElapsed)
        {
          this._log.Error("LatestFirmware upgrade task timed out");
          this._firmwareUpgradeProgress.ReportError(FirmwareUpgradeError.TimedOut);
        }
        bool flag = !this._watchdog.HasElapsed && this._taskCompletionSource.Task.Result;
        this._firmwareUpgradeProgress.ReportState(flag ? FirmwareUpgradeState.Complete : FirmwareUpgradeState.Failed);
        return flag;
      }
      catch (Exception ex)
      {
        this._log.Error(ex.ToString());
        this._firmwareUpgradeProgress.ReportState(FirmwareUpgradeState.Failed);
        return false;
      }
      finally
      {
        this._watchdog.Disable();
        this._notifyCharacteristicWatcher.Disable();
      }
    }

    private async void OnNotification(byte[] buffer)
    {
      if (this._watchdog.HasElapsed)
        return;
      this._watchdog.Reset();
      if (buffer == null || buffer.Length != 1)
      {
        Debugger.Break();
      }
      else
      {
        this._log.Debug("Notification: " + (object) buffer[0]);
        try
        {
          switch (buffer[0])
          {
            case 1:
              this._firmwareUpgradeProgress.ReportError(FirmwareUpgradeError.PostCheckFailed);
              this._taskCompletionSource.TrySetResult(false);
              this._notifyCharacteristicWatcher.Disable();
              break;
            case 2:
              this._log.Info("LatestFirmware upgraded successfully");
              this._taskCompletionSource.TrySetResult(true);
              this._notifyCharacteristicWatcher.Disable();
              break;
            case 11:
              this._log.Error("LatestFirmware check failed");
              this._firmwareUpgradeProgress.ReportError(FirmwareUpgradeError.InitialCheckFailed);
              this._taskCompletionSource.TrySetResult(false);
              break;
            case 12:
              if (this._dataTransmitted)
                break;
              this._notifyCharacteristicWatcher.Disable();
              if (await this.SendFirmwareData().ConfigureAwait(false))
              {
                this._dataTransmitted = true;
                this._firmwareUpgradeProgress.ReportState(FirmwareUpgradeState.Validating);
                this._notifyCharacteristicWatcher.Enable();
                break;
              }
              this._taskCompletionSource.TrySetResult(false);
              break;
            default:
              this._log.Warning("Unknown notification code when updating firmware " + (object) buffer[0]);
              break;
          }
        }
        catch (Exception ex)
        {
          this._log.Error(ex.ToString());
        }
      }
    }

    private async Task<bool> SendFirmwareData()
    {
      this._firmwareUpgradeProgress.ReportState(FirmwareUpgradeState.TransmittingData);
      byte[] packetBytes = new byte[20];
      int sentBytesCount = 0;
      int sentPacketsCount = 0;
      try
      {
        int prevReportedValue = -1;
        int leftToSend;
        while ((leftToSend = this._firmware.Data.Length - sentBytesCount) > 0)
        {
          if (this._watchdog.HasElapsed)
            return false;
          int currentPacketLength = leftToSend >= 20 ? 20 : leftToSend;
          if (packetBytes.Length != currentPacketLength)
            packetBytes = new byte[currentPacketLength];
          Array.Copy((Array) this._firmware.Data, sentBytesCount, (Array) packetBytes, 0, currentPacketLength);
          GattCommunicationStatus communicationStatus1 = await this._firmwareDataCharacteristic.WriteValueAsync(packetBytes.AsBuffer(), (GattWriteOption) 1).AsTask<GattCommunicationStatus>().ConfigureAwait(false);
          sentBytesCount += currentPacketLength;
          ++sentPacketsCount;
          if (sentPacketsCount > 0 && sentPacketsCount % 50 == 0 || sentBytesCount >= this._firmware.Data.Length)
          {
            GattCommunicationStatus communicationStatus2 = await this._controlPointCharacteristic.WriteValueAsync(new byte[1]
            {
              (byte) 11
            }.AsBuffer()).AsTask<GattCommunicationStatus>().ConfigureAwait(false);
            this._watchdog.Reset();
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
        return false;
      }
      return true;
    }

    private async Task SendFirmwareInfo()
    {
      int num1 = this._firmware.Version.ToInt();
      int num2 = this._oldFirmwareVersion.ToInt();
      byte[] macAddressOctets = BytesUtils.GetMacAddressOctets(this._bluetoothDevice.DefaultGattService.Device.BluetoothAddress);
      int num3 = ((int) macAddressOctets[4] << 8 | (int) macAddressOctets[5]) ^ BytesUtils.GetCRC16(this._firmware.Data);
      int length = this._firmware.Data.Length;
      List<byte> byteList1 = new List<byte>()
      {
        (byte) 7,
        (byte) num2,
        (byte) (num2 >> 8),
        (byte) (num2 >> 16),
        (byte) (num2 >> 24),
        (byte) num1,
        (byte) (num1 >> 8),
        (byte) (num1 >> 16),
        (byte) (num1 >> 24),
        (byte) length,
        (byte) (length >> 8),
        (byte) num3,
        (byte) (num3 >> 8)
      };
      byte? firmwareId = this._firmware.FirmwareId;
      if (firmwareId.HasValue)
      {
        List<byte> byteList2 = byteList1;
        firmwareId = this._firmware.FirmwareId;
        int num4 = (int) firmwareId.Value;
        byteList2.Add((byte) num4);
      }
      try
      {
        GattCommunicationStatus communicationStatus = await this._controlPointCharacteristic.WriteValueAsync(byteList1.ToArray().AsBuffer()).AsTask<GattCommunicationStatus>().ConfigureAwait(false);
      }
      catch (Exception ex)
      {
        this._firmwareUpgradeProgress.ReportError(FirmwareUpgradeError.CommunicationError);
        throw;
      }
    }
  }
}
