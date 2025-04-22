// Decompiled with JetBrains decompiler
// Type: MiBand.SDK.Core.MiBandOne.MiBandOne
// Assembly: MiBand.SDK, Version=0.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 152FA565-3249-4054-A361-01D9C0AFA7F1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBand.SDK.dll

using MiBand.SDK.Configuration;
using MiBand.SDK.Data;
using MiBand.SDK.FirmwareUpgrade;
using MiBand.SDK.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

#nullable disable
namespace MiBand.SDK.Core.MiBandOne
{
  internal class MiBandOne : MiBandBase
  {
    public MiBandOne(GattDeviceService gattService, ILog log)
      : base(gattService)
    {
      this.Log = log;
    }

    protected ILog Log { get; set; }

    public override string Id => this.DefaultGattService.DeviceId;

    public override async Task<RealtimeStepsData> GetRealtimeSteps()
    {
      byte[] numArray = await this.ReadCharacteristic(this.GetCharacteristic(CharacteristicGuid.RealtimeSteps)).ConfigureAwait(false);
      int num = 0;
      switch (numArray.Length)
      {
        case 2:
          num = (int) numArray[0] + ((int) numArray[1] << 8);
          break;
        case 3:
          num = (int) numArray[0] + ((int) numArray[1] << 8) + ((int) numArray[2] << 16);
          break;
        case 4:
          num = (int) numArray[0] + ((int) numArray[1] << 8) + ((int) numArray[2] << 16) + ((int) numArray[3] << 24);
          break;
      }
      return new RealtimeStepsData() { TotalSteps = num };
    }

    public override Task<HeartRateMeasurement> GetHeartRate()
    {
      throw new NotSupportedException("This version of Mi Band can't measure heart rate.");
    }

    public override async Task<BatteryInfo> GetBatteryInfo(bool cached)
    {
      byte[] numArray = await this.ReadCharacteristic(this.GetCharacteristic(CharacteristicGuid.Battery), cached ? (BluetoothCacheMode) 0 : (BluetoothCacheMode) 1).ConfigureAwait(false);
      return new BatteryInfo()
      {
        ChargedPercent = (int) numArray[0],
        LastCharged = (DateTimeOffset) new DateTime((int) numArray[1] + 2000, (int) numArray[2] + 1, (int) numArray[3], (int) numArray[4], (int) numArray[5], (int) numArray[6]),
        ChargesCount = (int) numArray[7] + ((int) numArray[8] << 8),
        IsCharging = numArray[9] == (byte) 2
      };
    }

    public override async Task<BandDeviceInfo> GetBandDeviceInfo(bool cached = false)
    {
      return BandDeviceInfo.FromMiBand1Bytes(await this.ReadCharacteristic(this.GetCharacteristic(CharacteristicGuid.DeviceInfo), cached ? (BluetoothCacheMode) 0 : (BluetoothCacheMode) 1).ConfigureAwait(false));
    }

    public override async Task SetDateTime(DateTimeOffset time)
    {
      byte[] data = new byte[6]
      {
        (byte) (time.Year - 2000 & (int) byte.MaxValue),
        (byte) (time.Month - 1),
        (byte) time.Day,
        (byte) time.Hour,
        (byte) time.Minute,
        (byte) time.Second
      };
      await this.WriteCharacteristic(this.GetCharacteristic(CharacteristicGuid.DateTime), data).ConfigureAwait(false);
    }

    public override async Task SetUserInfo(UserInfo userInfo, bool withPairing)
    {
      byte[] numArray = new byte[20];
      byte[] littleEndian = BytesUtils.LongToLittleEndian(userInfo.UserId);
      for (int index = 0; index < 4; ++index)
        numArray[index] = littleEndian[index];
      numArray[4] = userInfo.IsMale ? (byte) 1 : (byte) 0;
      numArray[5] = (byte) (DateTime.Now.Year - userInfo.Birthday.Year & (int) byte.MaxValue);
      numArray[6] = (byte) (userInfo.HeightCm & (int) byte.MaxValue);
      numArray[7] = (byte) (userInfo.WeightKg & (int) byte.MaxValue);
      numArray[8] = withPairing ? (byte) 1 : (byte) 0;
      string str = userInfo.UserId.ToString("D10");
      for (int index = 0; index < str.Length; ++index)
        numArray[index + 9] = Convert.ToByte(str[index]);
      ulong bluetoothAddress = this.DefaultGattService.Device.BluetoothAddress;
      byte num = (byte) (((int) (byte) BytesUtils.GetCRC8(((IEnumerable<byte>) numArray).Take<byte>(19).ToArray<byte>()) ^ (int) bluetoothAddress) & (int) byte.MaxValue);
      numArray[19] = num;
      await this.WriteCharacteristic(this.GetCharacteristic(CharacteristicGuid.UserInfo), numArray).ConfigureAwait(false);
    }

    public override async Task SetStepsGoal(int stepsGoal)
    {
      byte[] data = new byte[4]
      {
        (byte) 5,
        (byte) 0,
        (byte) (stepsGoal & (int) byte.MaxValue),
        (byte) (stepsGoal >> 8 & (int) byte.MaxValue)
      };
      await this.WriteCharacteristic(this.GetCharacteristic(CharacteristicGuid.ControlPoint), data).ConfigureAwait(false);
    }

    public override async Task SetAlarm(int alarmNumber, Alarm alarm)
    {
      int num1 = alarm.IsSmart ? 30 : 0;
      byte num2 = (byte) alarm.Days;
      if (alarm.Days == DaysOfWeek.None)
        num2 = (byte) 128;
      DateTime dateTime = DateTime.Now.Date + alarm.Time;
      if (dateTime < DateTime.Now)
        dateTime = dateTime.AddDays(1.0);
      byte[] data = new byte[11]
      {
        (byte) 4,
        (byte) alarmNumber,
        alarm.IsEnabled ? (byte) 1 : (byte) 0,
        (byte) (dateTime.Year - 2000 & (int) byte.MaxValue),
        (byte) (dateTime.Month - 1),
        (byte) dateTime.Day,
        (byte) dateTime.Hour,
        (byte) dateTime.Minute,
        (byte) 0,
        (byte) num1,
        num2
      };
      await this.WriteCharacteristic(this.GetCharacteristic(CharacteristicGuid.ControlPoint), data).ConfigureAwait(false);
    }

    public override async Task SetColorTheme(BandColorTheme colorTheme, bool flashLeds)
    {
      byte[] data = new byte[5]
      {
        (byte) 14,
        colorTheme.R,
        colorTheme.G,
        colorTheme.B,
        flashLeds ? (byte) 1 : (byte) 0
      };
      await this.WriteCharacteristic(this.GetCharacteristic(CharacteristicGuid.ControlPoint), data).ConfigureAwait(false);
    }

    public override async Task SetWearLocation(BandWearLocation wearLocation)
    {
      byte[] data = new byte[2]{ (byte) 15, (byte) 0 };
      byte num;
      switch (wearLocation)
      {
        case BandWearLocation.RightHand:
          num = (byte) 1;
          break;
        case BandWearLocation.Neck:
          num = (byte) 2;
          break;
        default:
          num = (byte) 0;
          break;
      }
      data[1] = num;
      await this.WriteCharacteristic(this.GetCharacteristic(CharacteristicGuid.ControlPoint), data).ConfigureAwait(false);
    }

    public override Task SetHeartRateDuringSleep(bool enabled)
    {
      throw new NotSupportedException("Heart rate during sleep can't be set for Mi Band 1.");
    }

    public override async Task<SynchronizationDataPackage> GetActivityData(DateTimeOffset startTime)
    {
      return await new MiBandOneActivityDataLoader(this, this.Log).LoadData().ConfigureAwait(false);
    }

    public override async Task SubmitActivityDataFragmentReceived(
      DateTimeOffset fragmentStart,
      int fragmentSizeMinutes)
    {
      DateTimeOffset dateTimeOffset = fragmentStart;
      byte[] data = new byte[9]
      {
        (byte) 10,
        (byte) (dateTimeOffset.Year - 2000 & (int) byte.MaxValue),
        (byte) (dateTimeOffset.Month - 1),
        (byte) dateTimeOffset.Day,
        (byte) dateTimeOffset.Hour,
        (byte) dateTimeOffset.Minute,
        (byte) dateTimeOffset.Second,
        (byte) (fragmentSizeMinutes & (int) byte.MaxValue),
        (byte) (fragmentSizeMinutes >> 8 & (int) byte.MaxValue)
      };
      await this.WriteCharacteristic(this.GetCharacteristic(CharacteristicGuid.ControlPoint), data).ConfigureAwait(false);
    }

    public override async Task Locate()
    {
      await this.Vibrate(Vibration.CallHigh).ConfigureAwait(false);
    }

    public override async Task Reboot()
    {
      await this.WriteCharacteristic(this.GetCharacteristic(CharacteristicGuid.ControlPoint), new byte[1]
      {
        (byte) 12
      }).ConfigureAwait(false);
    }

    public override async Task Vibrate(Vibration vibration)
    {
      try
      {
        GattDeviceService secondaryService = this.GetSecondaryService(GattServiceUuids.ImmediateAlert);
        if (secondaryService == null)
          return;
        await this.WriteCharacteristic(secondaryService.GetCharacteristics(GattCharacteristicUuids.AlertLevel).First<GattCharacteristic>(), new byte[1]
        {
          (byte) vibration
        }, true).ConfigureAwait(false);
      }
      catch (Exception ex)
      {
      }
    }

    public override async Task<BindingResult> Bind(UserInfo userInfo)
    {
      BindingResult result = BindingResult.None;
      if (userInfo != (UserInfo) null)
        await this.SetUserInfo(userInfo, true).ConfigureAwait(false);
      CancellationTokenSource timeoutTaskCts = new CancellationTokenSource(25000);
      Task task = Task.Run((Func<Task>) (async () =>
      {
        while (!timeoutTaskCts.Token.IsCancellationRequested)
        {
          byte[] numArray = await this.ReadCharacteristic(this.GetCharacteristic(CharacteristicGuid.Notify)).ConfigureAwait(false);
          if (numArray != null)
          {
            if (numArray[0] == (byte) 21)
            {
              result = BindingResult.Success;
              return;
            }
            if (numArray[0] == (byte) 9)
              break;
          }
          if (numArray == null || numArray[0] == (byte) 19)
            await Task.Delay(1000).ConfigureAwait(false);
        }
        result = BindingResult.Fail;
      }), timeoutTaskCts.Token);
      try
      {
        await task.ConfigureAwait(false);
      }
      catch (TaskCanceledException ex)
      {
        result = BindingResult.Fail;
      }
      return result;
    }

    public override async Task<AuthenticationResult> Authenticate(UserInfo userInfo)
    {
      await this.SetUserInfo(userInfo, false).ConfigureAwait(false);
      byte? nullable1 = (await this.ReadCharacteristic(this.GetCharacteristic(CharacteristicGuid.Notify)).ConfigureAwait(false))?[0];
      int? nullable2 = nullable1.HasValue ? new int?((int) nullable1.GetValueOrDefault()) : new int?();
      int num = 19;
      if ((nullable2.GetValueOrDefault() == num ? (nullable2.HasValue ? 1 : 0) : 0) == 0)
        return AuthenticationResult.Success;
      this.Log.Warning("Initiating binding");
      return AuthenticationResult.BindingStarted;
    }

    public override Task<bool> UpgradeFirmware(
      Firmware firmware,
      IFirmwareUpgradeProgress firmwareUpgradeProgress)
    {
      return new MiBandOneFirmwareUpgrader(this, this.Log).StartUpgrade(firmware, firmwareUpgradeProgress);
    }

    public override Task SetMetricUnitsSystem(bool isMetric) => (Task) Task.FromResult<int>(0);

    public override Task Set24HTimeFormat(bool is24HourFormat) => (Task) Task.FromResult<int>(0);

    public override Task SetHighlightOnWristLift(bool enabled)
    {
      throw new NotSupportedException("SetHighlightOnWristLift is not supported by " + this.GetType().Name);
    }

    public override Task SetFlipDisplayOnWristRotate(bool enabled)
    {
      throw new NotSupportedException("SetFlipDisplayOnWristRotate is not supported by " + this.GetType().Name);
    }

    public override Task SetDisplayItems(DisplayItem displayItems)
    {
      throw new NotSupportedException("SetDisplayItems is not supported by " + this.GetType().Name);
    }

    public override Task SetActivityReminder(ActivityReminderConfig activityReminderConfig)
    {
      throw new NotSupportedException("SetActivityReminder is not support by " + this.GetType().Name);
    }

    public override Task SetNotDisturbConfig(NotDisturbConfig config)
    {
      throw new NotSupportedException("SetNotDisturbConfig is not supported by " + this.GetType().Name);
    }

    public override Task SetDateDisplayMode(DateDisplayMode dateDisplayMode)
    {
      throw new NotSupportedException("SetDateDisplayMode is not supported by " + this.GetType().Name);
    }

    public override Task SetGoalReachedNotification(bool enabled)
    {
      throw new NotSupportedException("SetGoalReachedNotification is not supported by " + this.GetType().Name);
    }

    public override Task AlertNotifyIcon(AlertThirdPartyIcon alertThirdPartyIcon)
    {
      throw new NotSupportedException("AlertNotifyIcon is not supported by " + this.GetType().Name);
    }

    public override Task AlertNotifyIcon(MiBand.SDK.Core.AlertNotifyIcon alertNotifyIcon, string message)
    {
      throw new NotSupportedException("AlertNotifyIcon is not supported by " + this.GetType().Name);
    }

    public override Task StartCallNotification() => this.Vibrate(Vibration.CallHigh);

    public override Task StopCallNotification() => this.Vibrate(Vibration.None);
  }
}
