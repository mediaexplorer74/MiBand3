
// Type: MiBandApp.Storage.Settings.Settings
// Assembly: MiBandApp.Storage, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 12BDBED5-65C6-40FC-A182-B057969144FE
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.Storage.dll

using MiBand.SDK.Configuration;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Resources;
using Windows.Storage;

#nullable disable
namespace MiBandApp.Storage.Settings
{
  public class Settings
  {
    private const string LiveTileEnabledKey = "LiveTileEnabled";
    private const string HistoryIsChartViewKey = "HistoryIsChartView";
    private const string UserIdKey = "UserId";
    private const string IsMaleKey = "UserIsMale";
    private const string BirthdayTicksKey = "UserBirthday";
    private const string HeightKey = "UserHeight";
    private const string WeightKey = "UserWeight";
    private const string UserNameKey = "UserName";
    private const string DistanceUnitsKey = "DistanceUnits";
    private const string OneDriveSyncEnabledKey = "OneDriveSyncEnabled";
    private const string StepsGoalKey = "UserGoal";
    private const string SleepGoalKey = "UserGoalSleepMin";
    private const string HeartRateDuringSleepEnabledKey = "HeartRateDuringSleepEnabled";
    private const string HighlightOnWristLiftEnabledKey = "HighlightOnWristLiftEnabled";
    private const string DisplayItemsConfigKey = "DisplayItemsConfig";
    private const string ActivityReminderEnabledKey = "ActivityReminderEnabled";
    private const string ActivityReminderStartKey = "ActivityReminderStart";
    private const string ActivityReminderEndKey = "ActivityReminderEnd";
    private const string BandWearLocationKey = "BandWearLocation";
    private const string BandColorThemeKey = "BandColorTheme";
    private const string DisplayDateEnabledKey = "DisplayDateEnabled";
    private const string GoalReachedNotificationEnabledKey = "GoalReachedNotificationEnabled";
    private const string NotDisturbEnabledKey = "NotDisturbEnabled";
    private const string NotDisturbSmartKey = "NotDisturbSmart";
    private const string NotDisturbAllowHighlightOnWristLiftKey = "NotDisturbAllowHighlightOnWristLift";
    private const string NotDisturbStartTimeKey = "NotDisturbStartTime";
    private const string NotDisturbEndTimeKey = "NotDisturbEndTime";
    private const string FlipDisplayOnWristRotateEnabledKey = "FlipDisplayOnWristRotateEnabled";
    private readonly ApplicationDataContainer _localSettings;
    private bool? _showImperialUnits;

    public Settings()
    {
      this._localSettings = ApplicationData.Current.LocalSettings;
      this.CreateDefaults();
      this.SetDefaultGoal();
    }

    public AlarmsSettings Alarms { get; } = new AlarmsSettings();

    public bool UserInfoNeedsUpdate { get; set; }

    public long GetUserId()
    {
      return (long) ((IDictionary<string, object>) this._localSettings.Values)["UserId"];
    }

    public bool IsLiveTileEnabled
    {
      get => (bool) ((IDictionary<string, object>) this._localSettings.Values)["LiveTileEnabled"];
      set
      {
        ((IDictionary<string, object>) this._localSettings.Values)["LiveTileEnabled"] = (object) value;
      }
    }

    public bool HistoryIsChartView
    {
      get
      {
        return (bool) ((IDictionary<string, object>) this._localSettings.Values)[nameof (HistoryIsChartView)];
      }
      set
      {
        ((IDictionary<string, object>) this._localSettings.Values)[nameof (HistoryIsChartView)] = (object) value;
      }
    }

    public MiBandApp.Storage.Settings.Settings.DistanceUnit DistanceUnits
    {
      get
      {
        return (MiBandApp.Storage.Settings.Settings.DistanceUnit) ((IDictionary<string, object>) this._localSettings.Values)[nameof (DistanceUnits)];
      }
      set
      {
        ((IDictionary<string, object>) this._localSettings.Values)[nameof (DistanceUnits)] = (object) (int) value;
      }
    }

    public bool OneDriveSyncEnabled
    {
      get
      {
        return (bool) ((IDictionary<string, object>) this._localSettings.Values)[nameof (OneDriveSyncEnabled)];
      }
      set
      {
        ((IDictionary<string, object>) this._localSettings.Values)[nameof (OneDriveSyncEnabled)] = (object) value;
      }
    }

    public bool HeartRateDuringSleepEnabled
    {
      get
      {
        return (bool) ((IDictionary<string, object>) this._localSettings.Values)[nameof (HeartRateDuringSleepEnabled)];
      }
      set
      {
        ((IDictionary<string, object>) this._localSettings.Values)[nameof (HeartRateDuringSleepEnabled)] = (object) value;
      }
    }

    public bool GoalReachedNotificationEnabled
    {
      get
      {
        return (bool) ((IDictionary<string, object>) this._localSettings.Values)[nameof (GoalReachedNotificationEnabled)];
      }
      set
      {
        ((IDictionary<string, object>) this._localSettings.Values)[nameof (GoalReachedNotificationEnabled)] = (object) value;
      }
    }

    public bool HighlightOnWristLiftEnabled
    {
      get
      {
        return (bool) ((IDictionary<string, object>) this._localSettings.Values)[nameof (HighlightOnWristLiftEnabled)];
      }
      set
      {
        ((IDictionary<string, object>) this._localSettings.Values)[nameof (HighlightOnWristLiftEnabled)] = (object) value;
      }
    }

    public BandWearLocation BandWearLocation
    {
      get
      {
        return (BandWearLocation) ((IDictionary<string, object>) this._localSettings.Values)[nameof (BandWearLocation)];
      }
      set
      {
        ((IDictionary<string, object>) this._localSettings.Values)[nameof (BandWearLocation)] = (object) (int) value;
      }
    }

    public string BandColorThemeName
    {
      get => (string) ((IDictionary<string, object>) this._localSettings.Values)["BandColorTheme"];
      set
      {
        ((IDictionary<string, object>) this._localSettings.Values)["BandColorTheme"] = (object) value;
      }
    }

    public Version AppVersion
    {
      get
      {
        PackageVersion version = Package.Current.Id.Version;
        return new Version((int) version.Major, (int) version.Minor, (int) version.Build, (int) version.Revision);
      }
    }

    public DisplayItem DisplayItemsConfig
    {
      get
      {
        return (DisplayItem) (int) ((IDictionary<string, object>) this._localSettings.Values)[nameof (DisplayItemsConfig)];
      }
      set
      {
        ((IDictionary<string, object>) this._localSettings.Values)[nameof (DisplayItemsConfig)] = (object) (int) value;
      }
    }

    public bool ShowImperialUnits
    {
      get
      {
        if (!this._showImperialUnits.HasValue)
          this._showImperialUnits = new bool?(string.Compare(new ResourceLoader().GetString("UserInfoPageImperialSignShown"), "yes", StringComparison.OrdinalIgnoreCase) == 0);
        return this._showImperialUnits.Value;
      }
    }

    public bool ActivityReminderEnabled
    {
      get
      {
        return (bool) ((IDictionary<string, object>) this._localSettings.Values)[nameof (ActivityReminderEnabled)];
      }
      set
      {
        ((IDictionary<string, object>) this._localSettings.Values)[nameof (ActivityReminderEnabled)] = (object) value;
      }
    }

    public TimeSpan ActivityReminderStart
    {
      get
      {
        return TimeSpan.FromMinutes((double) (int) ((IDictionary<string, object>) this._localSettings.Values)[nameof (ActivityReminderStart)]);
      }
      set
      {
        ((IDictionary<string, object>) this._localSettings.Values)[nameof (ActivityReminderStart)] = (object) (int) value.TotalMinutes;
      }
    }

    public TimeSpan ActivityReminderEnd
    {
      get
      {
        return TimeSpan.FromMinutes((double) (int) ((IDictionary<string, object>) this._localSettings.Values)[nameof (ActivityReminderEnd)]);
      }
      set
      {
        ((IDictionary<string, object>) this._localSettings.Values)[nameof (ActivityReminderEnd)] = (object) (int) value.TotalMinutes;
      }
    }

    public bool DisplayDateEnabled
    {
      get
      {
        return (bool) ((IDictionary<string, object>) this._localSettings.Values)[nameof (DisplayDateEnabled)];
      }
      set
      {
        ((IDictionary<string, object>) this._localSettings.Values)[nameof (DisplayDateEnabled)] = (object) value;
      }
    }

    public bool NotDisturbEnabled
    {
      get
      {
        return (bool) ((IDictionary<string, object>) this._localSettings.Values)[nameof (NotDisturbEnabled)];
      }
      set
      {
        ((IDictionary<string, object>) this._localSettings.Values)[nameof (NotDisturbEnabled)] = (object) value;
      }
    }

    public bool NotDisturbSmart
    {
      get
      {
        return (bool) ((IDictionary<string, object>) this._localSettings.Values)[nameof (NotDisturbSmart)];
      }
      set
      {
        ((IDictionary<string, object>) this._localSettings.Values)[nameof (NotDisturbSmart)] = (object) value;
      }
    }

    public bool NotDisturbAllowHighlightOnWristLift
    {
      get
      {
        return (bool) ((IDictionary<string, object>) this._localSettings.Values)[nameof (NotDisturbAllowHighlightOnWristLift)];
      }
      set
      {
        ((IDictionary<string, object>) this._localSettings.Values)[nameof (NotDisturbAllowHighlightOnWristLift)] = (object) value;
      }
    }

    public TimeSpan NotDisturbStartTime
    {
      get
      {
        return TimeSpan.FromMinutes((double) (int) ((IDictionary<string, object>) this._localSettings.Values)[nameof (NotDisturbStartTime)]);
      }
      set
      {
        ((IDictionary<string, object>) this._localSettings.Values)[nameof (NotDisturbStartTime)] = (object) (int) value.TotalMinutes;
      }
    }

    public TimeSpan NotDisturbEndTime
    {
      get
      {
        return TimeSpan.FromMinutes((double) (int) ((IDictionary<string, object>) this._localSettings.Values)[nameof (NotDisturbEndTime)]);
      }
      set
      {
        ((IDictionary<string, object>) this._localSettings.Values)[nameof (NotDisturbEndTime)] = (object) (int) value.TotalMinutes;
      }
    }

    public bool FlipDisplayOnWristRotateEnabled
    {
      get
      {
        return (bool) ((IDictionary<string, object>) this._localSettings.Values)[nameof (FlipDisplayOnWristRotateEnabled)];
      }
      set
      {
        ((IDictionary<string, object>) this._localSettings.Values)[nameof (FlipDisplayOnWristRotateEnabled)] = (object) value;
      }
    }

    public bool SyncReminderEnabled
    {
      get
      {
        return (bool) ((IDictionary<string, object>) this._localSettings.Values)[nameof (SyncReminderEnabled)];
      }
      set
      {
        ((IDictionary<string, object>) this._localSettings.Values)[nameof (SyncReminderEnabled)] = (object) value;
      }
    }

    public UserInfo GetSavedUserInfo()
    {
      this.CreateDefaultUserInfo();
      return new UserInfo()
      {
        UserId = this.GetUserId(),
        IsMale = (bool) ((IDictionary<string, object>) this._localSettings.Values)["UserIsMale"],
        Birthday = new DateTimeOffset((long) ((IDictionary<string, object>) this._localSettings.Values)["UserBirthday"], TimeSpan.Zero),
        HeightCm = (int) ((IDictionary<string, object>) this._localSettings.Values)["UserHeight"],
        WeightKg = (int) ((IDictionary<string, object>) this._localSettings.Values)["UserWeight"],
        Name = (string) ((IDictionary<string, object>) this._localSettings.Values)["UserName"]
      };
    }

    public void SaveUserInfo(UserInfo userInfo)
    {
      ((IDictionary<string, object>) this._localSettings.Values)["UserIsMale"] = (object) userInfo.IsMale;
      ((IDictionary<string, object>) this._localSettings.Values)["UserBirthday"] = (object) userInfo.Birthday.Ticks;
      ((IDictionary<string, object>) this._localSettings.Values)["UserHeight"] = (object) userInfo.HeightCm;
      ((IDictionary<string, object>) this._localSettings.Values)["UserWeight"] = (object) userInfo.WeightKg;
      ((IDictionary<string, object>) this._localSettings.Values)["UserName"] = (object) userInfo.Name;
    }

    public GoalInfo GetSavedGoalInfo()
    {
      return new GoalInfo()
      {
        SleepGoalMinutes = (int) ((IDictionary<string, object>) this._localSettings.Values)["UserGoalSleepMin"],
        StepsGoal = (int) ((IDictionary<string, object>) this._localSettings.Values)["UserGoal"]
      };
    }

    public void SaveGoalInfo(GoalInfo goalInfo)
    {
      ((IDictionary<string, object>) this._localSettings.Values)["UserGoal"] = (object) goalInfo.StepsGoal;
      ((IDictionary<string, object>) this._localSettings.Values)["UserGoalSleepMin"] = (object) goalInfo.SleepGoalMinutes;
    }

    public void CreateDefaultUserInfo()
    {
      bool flag = false;
      if (!((IDictionary<string, object>) this._localSettings.Values).ContainsKey("UserBirthday"))
      {
        ((IDictionary<string, object>) this._localSettings.Values).Add("UserBirthday", (object) new DateTimeOffset(1990, 1, 1, 0, 0, 0, TimeSpan.Zero).Ticks);
        flag = true;
      }
      if (!((IDictionary<string, object>) this._localSettings.Values).ContainsKey("UserHeight"))
      {
        ((IDictionary<string, object>) this._localSettings.Values).Add("UserHeight", (object) 180);
        flag = true;
      }
      if (!((IDictionary<string, object>) this._localSettings.Values).ContainsKey("UserWeight"))
      {
        ((IDictionary<string, object>) this._localSettings.Values).Add("UserWeight", (object) 70);
        flag = true;
      }
      if (!((IDictionary<string, object>) this._localSettings.Values).ContainsKey("UserIsMale"))
      {
        ((IDictionary<string, object>) this._localSettings.Values).Add("UserIsMale", (object) true);
        flag = true;
      }
      if (!((IDictionary<string, object>) this._localSettings.Values).ContainsKey("UserName"))
      {
        ((IDictionary<string, object>) this._localSettings.Values).Add("UserName", (object) "");
        flag = true;
      }
      if (!flag)
        return;
      this.UserInfoNeedsUpdate = true;
    }

    private void CreateDefaults()
    {
      if (!((IDictionary<string, object>) this._localSettings.Values).ContainsKey("LiveTileEnabled"))
        ((IDictionary<string, object>) this._localSettings.Values).Add("LiveTileEnabled", (object) false);
      if (!((IDictionary<string, object>) this._localSettings.Values).ContainsKey("UserId"))
        ((IDictionary<string, object>) this._localSettings.Values).Add("UserId", (object) MiBandApp.Storage.Settings.Settings.GetRandom());
      if (!((IDictionary<string, object>) this._localSettings.Values).ContainsKey("HistoryIsChartView"))
        ((IDictionary<string, object>) this._localSettings.Values).Add("HistoryIsChartView", (object) true);
      if (!((IDictionary<string, object>) this._localSettings.Values).ContainsKey("DistanceUnits"))
        ((IDictionary<string, object>) this._localSettings.Values).Add("DistanceUnits", (object) 0);
      if (!((IDictionary<string, object>) this._localSettings.Values).ContainsKey("OneDriveSyncEnabled"))
        this.OneDriveSyncEnabled = false;
      if (!((IDictionary<string, object>) this._localSettings.Values).ContainsKey("HeartRateDuringSleepEnabled"))
        this.HeartRateDuringSleepEnabled = false;
      if (!((IDictionary<string, object>) this._localSettings.Values).ContainsKey("HighlightOnWristLiftEnabled"))
        this.HighlightOnWristLiftEnabled = false;
      if (!((IDictionary<string, object>) this._localSettings.Values).ContainsKey("DisplayItemsConfig"))
        this.DisplayItemsConfig = DisplayItem.Steps | DisplayItem.Distance | DisplayItem.Calories | DisplayItem.HeartRate | DisplayItem.Battery;
      if (!((IDictionary<string, object>) this._localSettings.Values).ContainsKey("DisplayDateEnabled"))
        this.DisplayDateEnabled = false;
      if (!((IDictionary<string, object>) this._localSettings.Values).ContainsKey("ActivityReminderEnabled"))
        this.ActivityReminderEnabled = false;
      if (!((IDictionary<string, object>) this._localSettings.Values).ContainsKey("ActivityReminderStart"))
        this.ActivityReminderStart = TimeSpan.FromHours(9.0);
      if (!((IDictionary<string, object>) this._localSettings.Values).ContainsKey("ActivityReminderEnd"))
        this.ActivityReminderEnd = TimeSpan.FromHours(19.0);
      if (!((IDictionary<string, object>) this._localSettings.Values).ContainsKey("BandWearLocation"))
        this.BandWearLocation = BandWearLocation.LeftHand;
      if (!((IDictionary<string, object>) this._localSettings.Values).ContainsKey("BandColorTheme"))
        ((IDictionary<string, object>) this._localSettings.Values)["BandColorTheme"] = (object) "";
      if (!((IDictionary<string, object>) this._localSettings.Values).ContainsKey("GoalReachedNotificationEnabled"))
        this.GoalReachedNotificationEnabled = true;
      if (!((IDictionary<string, object>) this._localSettings.Values).ContainsKey("NotDisturbEnabled"))
        this.NotDisturbEnabled = false;
      if (!((IDictionary<string, object>) this._localSettings.Values).ContainsKey("NotDisturbSmart"))
        this.NotDisturbSmart = true;
      if (!((IDictionary<string, object>) this._localSettings.Values).ContainsKey("NotDisturbAllowHighlightOnWristLift"))
        this.NotDisturbAllowHighlightOnWristLift = false;
      if (!((IDictionary<string, object>) this._localSettings.Values).ContainsKey("NotDisturbStartTime"))
        this.NotDisturbStartTime = TimeSpan.FromHours(22.0);
      if (!((IDictionary<string, object>) this._localSettings.Values).ContainsKey("NotDisturbEndTime"))
        this.NotDisturbEndTime = TimeSpan.FromHours(7.0);
      if (!((IDictionary<string, object>) this._localSettings.Values).ContainsKey("FlipDisplayOnWristRotateEnabled"))
        this.FlipDisplayOnWristRotateEnabled = false;
      if (((IDictionary<string, object>) this._localSettings.Values).ContainsKey("SyncReminderEnabled"))
        return;
      this.SyncReminderEnabled = true;
    }

    private void SetDefaultGoal()
    {
      if (!((IDictionary<string, object>) this._localSettings.Values).ContainsKey("UserGoal"))
        ((IDictionary<string, object>) this._localSettings.Values).Add("UserGoal", (object) 10000);
      if (((IDictionary<string, object>) this._localSettings.Values).ContainsKey("UserGoalSleepMin"))
        return;
      ((IDictionary<string, object>) this._localSettings.Values).Add("UserGoalSleepMin", (object) 480);
    }

    private static long GetRandom() => (long) new Random().Next(1000000000, int.MaxValue);

    public enum DistanceUnit
    {
      Km,
      Mi,
    }
  }
}
