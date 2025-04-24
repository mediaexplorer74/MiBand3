
// Type: MiBand.SDK.Core.MiBandTwo.MiBandTwo
// Assembly: MiBand.SDK, Version=0.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 152FA565-3249-4054-A361-01D9C0AFA7F1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBand.SDK.dll

using MiBand.SDK.Bluetooth;
using MiBand.SDK.Configuration;
using MiBand.SDK.Data;
using MiBand.SDK.FirmwareUpgrade;
using MiBand.SDK.HeartRate;
using MiBand.SDK.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

#nullable disable
namespace MiBand.SDK.Core.MiBandTwo
{
  internal class MiBandTwo : MiBandBase
  {
    private readonly ILog _log;

    public MiBandTwo(GattDeviceService miliService, ILog log)
      : base(miliService)
    {
      this._log = log;
    }

    public override string Id => this.DefaultGattService.DeviceId;

    public override async Task<RealtimeStepsData> GetRealtimeSteps()
    {
      byte[] numArray = await this.ReadCharacteristic(this.GetCharacteristic(CharacteristicGuid.RealtimeSteps)).ConfigureAwait(false);
      if (numArray == null || numArray.Length < 5)
        return (RealtimeStepsData) null;
      int num1 = (int) numArray[0];
      RealtimeStepsData realtimeSteps = new RealtimeStepsData();
      realtimeSteps.TotalSteps = (int) numArray[1] | (int) numArray[2] << 8 | (int) numArray[3] << 16 | (int) numArray[4] << 24;
      int index = 5;
      if ((num1 & 1) == 1)
      {
        realtimeSteps.Running = (int) numArray[index] | (int) numArray[index + 1] << 8 | (int) numArray[index + 2] << 16 | (int) numArray[index + 3] << 24;
        index += 4;
      }
      if ((num1 >> 1 & 1) == 1)
      {
        realtimeSteps.Walking = (int) numArray[index] | (int) numArray[index + 1] << 8 | (int) numArray[index + 2] << 16 | (int) numArray[index + 3] << 24;
        index += 4;
      }
      if ((num1 >> 2 & 1) == 1)
      {
        realtimeSteps.TotalDistance = (int) numArray[index] | (int) numArray[index + 1] << 8 | (int) numArray[index + 2] << 16 | (int) numArray[index + 3] << 24;
        index += 4;
      }
      if ((num1 >> 3 & 1) == 1)
      {
        realtimeSteps.TotalCalories = (int) numArray[index] | (int) numArray[index + 1] << 8 | (int) numArray[index + 2] << 16 | (int) numArray[index + 3] << 24;
        int num2 = index + 4;
      }
      return realtimeSteps;
    }

    public override async Task<HeartRateMeasurement> GetHeartRate()
    {
      HeartRateMeasurement heartRate = await new HeartRateLoader(this._log, (BluetoothDeviceBase) this).GetHeartRate().ConfigureAwait(false);
      if (heartRate == null)
        return heartRate;
      this._log.Debug("HR: " + (object) heartRate);
      return heartRate.HeartRateValue != (ushort) 0 ? heartRate : (HeartRateMeasurement) null;
    }

    public override async Task<BatteryInfo> GetBatteryInfo(bool cached = false)
    {
      byte[] bytes = await this.ReadCharacteristic(this.GetCharacteristic(CharacteristicGuid.Battery), cached ? (BluetoothCacheMode) 0 : (BluetoothCacheMode) 1).ConfigureAwait(false);
      if (bytes == null || bytes.Length < 2)
        return (BatteryInfo) null;
      BatteryInfo batteryInfo = new BatteryInfo();
      batteryInfo.ChargedPercent = (int) bytes[1];
      int num1 = (int) bytes[0];
      bool flag1 = (num1 & 1) == 1;
      bool flag2 = (num1 >> 1 & 1) == 1;
      bool flag3 = (num1 >> 2 & 1) == 1;
      int num2 = (num1 >> 3 & 1) == 1 ? 1 : 0;
      int index = 2;
      if (flag1)
      {
        index = 3;
        batteryInfo.IsCharging = bytes[2] > (byte) 0;
      }
      if (flag2)
      {
        DateTimeOffset dateTimeOffset = MiBand2Tools.DateTimeOffsetFromBytes(bytes, ref index);
        batteryInfo.LastFullCharged = dateTimeOffset;
      }
      if (flag3)
      {
        DateTimeOffset dateTimeOffset = MiBand2Tools.DateTimeOffsetFromBytes(bytes, ref index);
        batteryInfo.LastCharged = dateTimeOffset;
      }
      if (num2 != 0)
        batteryInfo.LastChargeLevel = (int) bytes[index];
      return batteryInfo;
    }

    public override async Task<BandDeviceInfo> GetBandDeviceInfo(bool cached = false)
    {
      return BandDeviceInfo.FromMiBand2Bytes(await this.ReadCharacteristic(this.GetCharacteristic(GattCharacteristicUuids.SoftwareRevisionString, this.GetSecondaryService(GattServiceUuids.DeviceInformation)), cached ? (BluetoothCacheMode) 0 : (BluetoothCacheMode) 1).ConfigureAwait(false));
    }

    public override Task SetDateTime(DateTimeOffset time)
    {
      byte[] bytes = MiBand2Tools.DateTimeOffsetToBytes(time, 2, true);
      return this.WriteCharacteristic(this.GetCharacteristic(CharacteristicGuid.Time), bytes);
    }

    public override Task SetUserInfo(UserInfo userInfo, bool withPairing)
    {
      int year = userInfo.Birthday.Year;
      int month = userInfo.Birthday.Month;
      int day = userInfo.Birthday.Day;
      int heightCm = userInfo.HeightCm;
      int num = userInfo.WeightKg * 200;
      long userId = userInfo.UserId;
      byte[] data = new byte[16]
      {
        (byte) 79,
        (byte) 0,
        (byte) 0,
        (byte) (year & (int) byte.MaxValue),
        (byte) (year >> 8),
        (byte) month,
        (byte) day,
        userInfo.IsMale ? (byte) 0 : (byte) 1,
        (byte) (heightCm & (int) byte.MaxValue),
        (byte) (heightCm >> 8 & (int) byte.MaxValue),
        (byte) (num & (int) byte.MaxValue),
        (byte) (num >> 8 & (int) byte.MaxValue),
        (byte) ((ulong) userId & (ulong) byte.MaxValue),
        (byte) ((ulong) (userId >> 8) & (ulong) byte.MaxValue),
        (byte) ((ulong) (userId >> 16) & (ulong) byte.MaxValue),
        (byte) ((ulong) (userId >> 24) & (ulong) byte.MaxValue)
      };
      return this.WriteCharacteristic(this.GetCharacteristic(CharacteristicGuid.UserInfo), data);
    }

    public override Task SetStepsGoal(int stepsGoal)
    {
      byte[] data = new byte[7]
      {
        (byte) 16,
        (byte) 0,
        (byte) 0,
        (byte) (stepsGoal & (int) byte.MaxValue),
        (byte) (stepsGoal >> 8 & (int) byte.MaxValue),
        (byte) (stepsGoal >> 16 & (int) byte.MaxValue),
        (byte) (stepsGoal >> 24 & (int) byte.MaxValue)
      };
      return this.WriteCharacteristic(this.GetCharacteristic(CharacteristicGuid.UserInfo), data);
    }

    public override async Task SetAlarm(int alarmNumber, Alarm alarm)
    {
      byte num = (byte) alarm.Days;
      if (alarm.Days == DaysOfWeek.None)
        num = (byte) 128;
      byte[] values = new byte[5]
      {
        (byte) 2,
        (byte) (alarmNumber | (alarm.IsSmart ? 64 : 0) | (alarm.IsEnabled ? 128 : 0)),
        (byte) 0,
        (byte) 0,
        (byte) 0
      };
      byte[] numArray1 = values;
      TimeSpan timeSpan = alarm.Time;
      int hours = (int) (byte) timeSpan.Hours;
      numArray1[2] = (byte) hours;
      byte[] numArray2 = values;
      timeSpan = alarm.Time;
      int minutes = (int) (byte) timeSpan.Minutes;
      numArray2[3] = (byte) minutes;
      values[4] = num;
      this._log.Debug(string.Format("Setting alarm {0} with cmd {1}", (object) alarmNumber, (object) string.Join<byte>(":", (IEnumerable<byte>) values)));
      GattCharacteristic characteristic = this.GetCharacteristic(CharacteristicGuid.Settings);
      byte[] data = values;
      timeSpan = new TimeSpan();
      TimeSpan waitTimeSpan = timeSpan;
      NotifyResponse notifyResponse = await this.WriteCharacteristicGetResponse(characteristic, data, waitTimeSpan).ConfigureAwait(false);
    }

    public override Task SetColorTheme(BandColorTheme colorTheme, bool flashLeds)
    {
      throw new NotSupportedException("This version of Mi Band doesn't support different color schemes.");
    }

    public override Task SetWearLocation(BandWearLocation wearLocation)
    {
      int num1 = wearLocation == BandWearLocation.Neck ? 1 : 2;
      int num2 = 0;
      if (wearLocation == BandWearLocation.RightHand)
        num2 = 1;
      byte[] data = new byte[4]
      {
        (byte) 32,
        (byte) 0,
        (byte) 0,
        (byte) (num1 | num2 << 7)
      };
      return this.WriteCharacteristic(this.GetCharacteristic(CharacteristicGuid.UserInfo), data);
    }

    public override async Task SetHeartRateDuringSleep(bool enabled)
    {
      byte[] data = new byte[3]
      {
        (byte) 21,
        (byte) 0,
        enabled ? (byte) 1 : (byte) 0
      };
      await this.WriteCharacteristic(this.GetCharacteristic(GattCharacteristicUuids.HeartRateControlPoint, this.GetSecondaryService(GattServiceUuids.HeartRate)), data).ConfigureAwait(false);
    }

    public override async Task<SynchronizationDataPackage> GetActivityData(DateTimeOffset startTime)
    {
      return await new MiBandTwoActivityDataLoader(this, this._log).Sync(startTime).ConfigureAwait(false);
    }

    public override Task SubmitActivityDataFragmentReceived(
      DateTimeOffset fragmentStart,
      int fragmentSizeMinutes)
    {
      return (Task) Task.FromResult<int>(0);
    }

    public override async Task Locate()
    {
      await this.Vibrate(Vibration.Light).ConfigureAwait(false);
    }

    public override async Task Reboot()
    {
      throw new NotImplementedException(
          "No programmatical reboot is available for Mi Band 2. You need to press sensor button for 10 seconds.");
    }

    public override async Task Vibrate(Vibration vibration)
    {
      try
      {
        GattDeviceService secondaryService = this.GetSecondaryService(GattServiceUuids.ImmediateAlert);
        if (secondaryService == null)
          return;
        await this.WriteCharacteristic(secondaryService.GetCharacteristics(
            GattCharacteristicUuids.AlertLevel)[0], new byte[1]
        {
          (byte) vibration
        }, true).ConfigureAwait(false);
      }
      catch (Exception ex)
      {
        this._log.Info("Exception when trying to locate Mi Band 2: " + (object) ex);
      }
    }

    public override async Task<BindingResult> Bind(UserInfo userInfo)
    {
      GattDeviceService secondaryService = this.GetSecondaryService(ServiceUuid.Auth);
      NotifyResponse notifyResponse = await this.WriteCharacteristicGetResponse(this.GetCharacteristic(CharacteristicGuid.Auth, secondaryService), ((IEnumerable<byte>) new byte[2]
      {
        (byte) 1,
        (byte) 0
      }).Concat<byte>((IEnumerable<byte>) CryptographyTools.ComputeMD5(userInfo.UserId.ToString())).ToArray<byte>(), TimeSpan.FromSeconds(30.0)).ConfigureAwait(false);
      if (notifyResponse == null || !notifyResponse.IsSuccessCommand(1))
      {
        this._log.Info("Error when binding. Response: " + (object) notifyResponse);
        return BindingResult.Fail;
      }
      int num = (int) await this.Authenticate(userInfo).ConfigureAwait(false);
      return BindingResult.Success;
    }

    public override async Task<AuthenticationResult> Authenticate(UserInfo userInfo)
    {
      GattDeviceService secondaryService = this.GetSecondaryService(ServiceUuid.Auth);
      GattCharacteristic authChar = this.GetCharacteristic(CharacteristicGuid.Auth, secondaryService);
      byte[] source = new byte[2]{ (byte) 2, (byte) 0 };
      NotifyResponse notifyResponse1 = await this.WriteCharacteristicGetResponse(authChar, ((IEnumerable<byte>) source).ToArray<byte>()).ConfigureAwait(false);
      if (notifyResponse1 == null)
        return AuthenticationResult.None;
      if (notifyResponse1.IsSuccessCommand(2))
      {
        byte[] payload = notifyResponse1.Payload;
        if ((payload != null ? (payload.Length != 16 ? 1 : 0) : 1) == 0)
        {
          byte[] md5 = CryptographyTools.ComputeMD5(userInfo.UserId.ToString());
          byte[] arraySafe = CryptographicEngine.Encrypt(SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesEcb).CreateSymmetricKey(md5.AsBuffer()), notifyResponse1.Payload.AsBuffer(), (IBuffer) null).ToArraySafe();
          byte[] first = new byte[2]{ (byte) 3, (byte) 0 };
          NotifyResponse notifyResponse2 = await this.WriteCharacteristicGetResponse(authChar, ((IEnumerable<byte>) first).Concat<byte>((IEnumerable<byte>) arraySafe).ToArray<byte>()).ConfigureAwait(false);
          if (notifyResponse2 == null)
            return AuthenticationResult.None;
          if (notifyResponse2.IsSuccessCommand(3))
            return AuthenticationResult.Success;
          this._log.Info("Error when sending encrypted key during auth. Responce: " + (object) notifyResponse2);
          return AuthenticationResult.Fail;
        }
      }
      this._log.Info("Error when getting key for auth. Responce: " + (object) notifyResponse1);
      return AuthenticationResult.Fail;
    }

    public override Task<bool> UpgradeFirmware(
      Firmware firmware,
      IFirmwareUpgradeProgress firmwareUpgradeProgress)
    {
      return new MiBandTwoFirmwareUploader(this, this._log).UpdateFirmware(firmware, FirmwareType.Firmware, firmwareUpgradeProgress);
    }

    public override async Task SetMetricUnitsSystem(bool isMetric)
    {
      byte[] data = new byte[4]
      {
        (byte) 6,
        (byte) 3,
        (byte) 0,
        isMetric ? (byte) 0 : (byte) 1
      };
      NotifyResponse notifyResponse = await this.WriteCharacteristicGetResponse(this.GetCharacteristic(CharacteristicGuid.Settings), data, payloadOffset: 2).ConfigureAwait(false);
    }

    public override async Task Set24HTimeFormat(bool is24HourFormat)
    {
      byte[] data = new byte[4]
      {
        (byte) 6,
        (byte) 2,
        (byte) 0,
        is24HourFormat ? (byte) 1 : (byte) 0
      };
      NotifyResponse notifyResponse = await this.WriteCharacteristicGetResponse(this.GetCharacteristic(CharacteristicGuid.Settings), data, payloadOffset: 2).ConfigureAwait(false);
    }

    public override async Task SetHighlightOnWristLift(bool enabled)
    {
      byte[] data = new byte[4]
      {
        (byte) 6,
        (byte) 5,
        (byte) 0,
        enabled ? (byte) 1 : (byte) 0
      };
      NotifyResponse notifyResponse = await this.WriteCharacteristicGetResponse(this.GetCharacteristic(CharacteristicGuid.Settings), data, payloadOffset: 2).ConfigureAwait(false);
    }

    public override async Task SetFlipDisplayOnWristRotate(bool enabled)
    {
      byte[] data = new byte[4]
      {
        (byte) 6,
        (byte) 13,
        (byte) 0,
        enabled ? (byte) 1 : (byte) 0
      };
      NotifyResponse notifyResponse = await this.WriteCharacteristicGetResponse(this.GetCharacteristic(CharacteristicGuid.Settings), data, payloadOffset: 2).ConfigureAwait(false);
    }

    public override async Task SetDisplayItems(DisplayItem displayItems)
    {
      byte[] data = new byte[4]
      {
        (byte) 6,
        (byte) 4,
        (byte) 0,
        (byte) displayItems
      };
      NotifyResponse notifyResponse = await this.WriteCharacteristicGetResponse(this.GetCharacteristic(CharacteristicGuid.Settings), data, payloadOffset: 2).ConfigureAwait(false);
    }

    public override async Task SetActivityReminder(ActivityReminderConfig config)
    {
      byte[] data = new byte[12]
      {
        (byte) 8,
        config.IsEnabled.ToByte(),
        (byte) 60,
        (byte) 0,
        config.StartTime.Hours.ToByte(),
        config.StartTime.Minutes.ToByte(),
        config.EndTime.Hours.ToByte(),
        config.EndTime.Minutes.ToByte(),
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0
      };
      NotifyResponse notifyResponse = await this.WriteCharacteristicGetResponse(this.GetCharacteristic(CharacteristicGuid.Settings), data).ConfigureAwait(false);
    }

    public override async Task SetNotDisturbConfig(NotDisturbConfig config)
    {
      List<byte> byteList = new List<byte>();
      byteList.Add((byte) 9);
      byte num1 = (config.AllowHighlightOnWristLift ? 0 : 128).ToByte();
      byte num2 = 2;
      if (config.IsEnabled)
      {
        num2 = (byte) 3;
        if (!config.IsSmart)
        {
          num2 = (byte) 1;
          byteList.Add(config.StartTime.Hours.ToByte());
          byteList.Add(config.StartTime.Minutes.ToByte());
          byteList.Add(config.EndTime.Hours.ToByte());
          byteList.Add(config.EndTime.Minutes.ToByte());
        }
      }
      byteList.Insert(1, ((int) num2 | (int) num1).ToByte());
      NotifyResponse notifyResponse = await this.WriteCharacteristicGetResponse(this.GetCharacteristic(CharacteristicGuid.Settings), byteList.ToArray()).ConfigureAwait(false);
    }

    public override async Task SetDateDisplayMode(DateDisplayMode dateDisplayMode)
    {
      byte[] data = new byte[4]
      {
        (byte) 6,
        (byte) 10,
        (byte) 0,
        (byte) dateDisplayMode
      };
      NotifyResponse notifyResponse = await this.WriteCharacteristicGetResponse(this.GetCharacteristic(CharacteristicGuid.Settings), data, payloadOffset: 2).ConfigureAwait(false);
    }

    public override async Task SetGoalReachedNotification(bool enabled)
    {
      byte[] data = new byte[4]
      {
        (byte) 6,
        (byte) 6,
        (byte) 0,
        enabled ? (byte) 1 : (byte) 0
      };
      NotifyResponse notifyResponse = await this.WriteCharacteristicGetResponse(this.GetCharacteristic(CharacteristicGuid.Settings), data, payloadOffset: 2).ConfigureAwait(false);
    }

    public override async Task AlertNotifyIcon(AlertThirdPartyIcon alertThirdPartyIcon)
    {
      try
      {
        GattDeviceService secondaryService = this.GetSecondaryService(GattServiceUuids.AlertNotification);
        if (secondaryService == null)
          return;
        await this.WriteCharacteristic(secondaryService.GetCharacteristics(GattCharacteristicUuids.NewAlert)[0], new byte[3]
        {
          (byte) 250,
          (byte) 1,
          (byte) alertThirdPartyIcon
        }).ConfigureAwait(false);
      }
      catch (Exception ex)
      {
        this._log.Info(string.Format("Exception when {0}: ", (object) nameof (AlertNotifyIcon)) + (object) ex);
      }
    }

    public override async Task AlertNotifyIcon(MiBand.SDK.Core.AlertNotifyIcon alertNotifyIcon, string message)
    {
      try
      {
        GattDeviceService secondaryService = this.GetSecondaryService(GattServiceUuids.AlertNotification);
        if (secondaryService == null)
          return;
        GattCharacteristic characteristic = secondaryService.GetCharacteristics(GattCharacteristicUuids.NewAlert)[0];
        byte[] sourceArray = new byte[0];
        if (!string.IsNullOrEmpty(message))
          sourceArray = Encoding.UTF8.GetBytes(message);
        byte[] numArray = new byte[Math.Min(20, sourceArray.Length + 2)];
        if (sourceArray.Length != 0)
          Array.Copy((Array) sourceArray, 0, (Array) numArray, 2, numArray.Length - 2);
        numArray[0] = (byte) alertNotifyIcon;
        numArray[1] = (byte) 1;
        await this.WriteCharacteristic(characteristic, numArray).ConfigureAwait(false);
      }
      catch (Exception ex)
      {
        this._log.Info(string.Format("Exception when {0}: ", (object) nameof (AlertNotifyIcon)) + (object) ex);
      }
    }

    public override async Task StartCallNotification()
    {
      try
      {
        GattDeviceService secondaryService = this.GetSecondaryService(GattServiceUuids.AlertNotification);
        if (secondaryService == null)
          return;
        await this.WriteCharacteristic(secondaryService.GetCharacteristics(GattCharacteristicUuids.NewAlert)[0], new byte[2]
        {
          (byte) 3,
          (byte) 1
        }).ConfigureAwait(false);
      }
      catch (Exception ex)
      {
        this._log.Info(string.Format("Exception when {0}: ", (object) nameof (StartCallNotification)) + (object) ex);
      }
    }

    public override async Task StopCallNotification()
    {
      try
      {
        GattDeviceService secondaryService = this.GetSecondaryService(GattServiceUuids.AlertNotification);
        if (secondaryService == null)
          return;
        await this.WriteCharacteristic(secondaryService.GetCharacteristics(GattCharacteristicUuids.NewAlert)[0], new byte[2]
        {
          (byte) 3,
          (byte) 0
        }).ConfigureAwait(false);
      }
      catch (Exception ex)
      {
        this._log.Info(string.Format("Exception when {0}: ", (object) nameof (StopCallNotification)) + (object) ex);
      }
    }
  }
}
