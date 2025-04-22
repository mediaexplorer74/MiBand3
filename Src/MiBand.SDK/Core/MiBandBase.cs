// Decompiled with JetBrains decompiler
// Type: MiBand.SDK.Core.MiBandBase
// Assembly: MiBand.SDK, Version=0.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 152FA565-3249-4054-A361-01D9C0AFA7F1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBand.SDK.dll

using MiBand.SDK.Bluetooth;
using MiBand.SDK.Configuration;
using MiBand.SDK.Data;
using MiBand.SDK.FirmwareUpgrade;
using System;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

#nullable disable
namespace MiBand.SDK.Core
{
  internal abstract class MiBandBase(GattDeviceService defaultGattService) : 
    BluetoothDeviceBase(defaultGattService),
    IMiBand,
    IBluetoothDevice,
    IDisposable
  {
    public abstract string Id { get; }

    public abstract Task<RealtimeStepsData> GetRealtimeSteps();

    public abstract Task<HeartRateMeasurement> GetHeartRate();

    public abstract Task<BatteryInfo> GetBatteryInfo(bool cached = false);

    public abstract Task<BandDeviceInfo> GetBandDeviceInfo(bool cached = false);

    public abstract Task SetDateTime(DateTimeOffset time);

    public abstract Task SetUserInfo(UserInfo userInfo, bool withPairing);

    public abstract Task SetStepsGoal(int stepsGoal);

    public abstract Task SetAlarm(int alarmNumber, Alarm alarm);

    public abstract Task SetColorTheme(BandColorTheme colorTheme, bool flashLeds);

    public abstract Task SetWearLocation(BandWearLocation wearLocation);

    public abstract Task SetHeartRateDuringSleep(bool enabled);

    public abstract Task<SynchronizationDataPackage> GetActivityData(DateTimeOffset startTime);

    public abstract Task SubmitActivityDataFragmentReceived(
      DateTimeOffset fragmentStart,
      int fragmentSizeMinutes);

    public abstract Task Locate();

    public abstract Task Reboot();

    public abstract Task Vibrate(Vibration vibration);

    public abstract Task AlertNotifyIcon(AlertThirdPartyIcon alertThirdPartyIcon);

    public abstract Task AlertNotifyIcon(MiBand.SDK.Core.AlertNotifyIcon alertNotifyIcon, string message);

    public abstract Task StartCallNotification();

    public abstract Task StopCallNotification();

    public abstract Task<BindingResult> Bind(UserInfo userInfo);

    public abstract Task<AuthenticationResult> Authenticate(UserInfo userInfo);

    public abstract Task<bool> UpgradeFirmware(
      Firmware firmware,
      IFirmwareUpgradeProgress firmwareUpgradeProgress);

    public abstract Task SetMetricUnitsSystem(bool isMetric);

    public abstract Task Set24HTimeFormat(bool is24HourFormat);

    public abstract Task SetHighlightOnWristLift(bool enabled);

    public abstract Task SetFlipDisplayOnWristRotate(bool enabled);

    public abstract Task SetDisplayItems(DisplayItem displayItems);

    public abstract Task SetActivityReminder(ActivityReminderConfig activityReminderConfig);

    public abstract Task SetNotDisturbConfig(NotDisturbConfig config);

    public abstract Task SetDateDisplayMode(DateDisplayMode dateDisplayMode);

    public abstract Task SetGoalReachedNotification(bool enabled);
  }
}
