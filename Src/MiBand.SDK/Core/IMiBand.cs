
// Type: MiBand.SDK.Core.IMiBand
// Assembly: MiBand.SDK, Version=0.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 152FA565-3249-4054-A361-01D9C0AFA7F1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBand.SDK.dll

using MiBand.SDK.Bluetooth;
using MiBand.SDK.Configuration;
using MiBand.SDK.Data;
using MiBand.SDK.FirmwareUpgrade;
using System;
using System.Threading.Tasks;

#nullable disable
namespace MiBand.SDK.Core
{
  public interface IMiBand : IBluetoothDevice, IDisposable
  {
    Task<RealtimeStepsData> GetRealtimeSteps();

    Task<HeartRateMeasurement> GetHeartRate();

    Task<BatteryInfo> GetBatteryInfo(bool cached = false);

    Task<BandDeviceInfo> GetBandDeviceInfo(bool cached = false);

    Task SetDateTime(DateTimeOffset time);

    Task SetUserInfo(UserInfo userInfo, bool withPairing);

    Task SetStepsGoal(int stepsGoal);

    Task SetAlarm(int alarmNumber, Alarm alarm);

    Task SetColorTheme(BandColorTheme colorTheme, bool flashLeds);

    Task SetWearLocation(BandWearLocation wearLocation);

    Task SetHeartRateDuringSleep(bool enabled);

    Task<SynchronizationDataPackage> GetActivityData(DateTimeOffset startTime);

    Task SubmitActivityDataFragmentReceived(DateTimeOffset fragmentStart, int fragmentSizeMinutes);

    Task Locate();

    Task Reboot();

    Task Vibrate(Vibration vibration);

    Task AlertNotifyIcon(AlertThirdPartyIcon alertThirdPartyIcon);

    Task AlertNotifyIcon(MiBand.SDK.Core.AlertNotifyIcon alertNotifyIcon, string message = null);

    Task StartCallNotification();

    Task StopCallNotification();

    Task<BindingResult> Bind(UserInfo userInfo);

    Task<AuthenticationResult> Authenticate(UserInfo userInfo);

    Task<bool> UpgradeFirmware(Firmware firmware, IFirmwareUpgradeProgress firmwareUpgradeProgress);

    Task SetMetricUnitsSystem(bool isMetric);

    Task Set24HTimeFormat(bool is24HourFormat);

    Task SetHighlightOnWristLift(bool enabled);

    Task SetFlipDisplayOnWristRotate(bool enabled);

    Task SetDisplayItems(DisplayItem displayItems);

    Task SetActivityReminder(ActivityReminderConfig activityReminderConfig);

    Task SetNotDisturbConfig(NotDisturbConfig config);

    Task SetDateDisplayMode(DateDisplayMode dateDisplayMode);

    Task SetGoalReachedNotification(bool enabled);
  }
}
