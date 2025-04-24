
// Type: MiBandApp.Storage.Settings.AlarmsSettings
// Assembly: MiBandApp.Storage, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 12BDBED5-65C6-40FC-A182-B057969144FE
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.Storage.dll

using MiBand.SDK.Configuration;
using System;
using System.Collections.Generic;
using Windows.Storage;

#nullable disable
namespace MiBandApp.Storage.Settings
{
  public sealed class AlarmsSettings
  {
    private const int AlarmKeyNumBase = 0;
    private const string AlarmKeyBase = "Alarm";
    private const int AlarmMaxCount = 10;

    public IEnumerable<Alarm> GetAllSaved(bool disableExpired = true)
    {
      ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
      for (int i = 0; i < 10; ++i)
      {
        string key = "Alarm" + (object) i;
        if (((IDictionary<string, object>) localSettings.Values).ContainsKey(key))
        {
          Alarm alarm = this.FromCompositeValue((ApplicationDataCompositeValue) ((IDictionary<string, object>) localSettings.Values)[key]);
          if (disableExpired && this.IsAlarmExpired(alarm))
          {
            alarm.IsEnabled = false;
            this.SaveAlarm(alarm, i);
          }
          yield return alarm;
        }
      }
    }

    public void SaveAll(IList<Alarm> alarms)
    {
      for (int index = 0; index < 10; ++index)
      {
        if (index < alarms.Count)
          this.SaveAlarm(alarms[index], index);
        else
          this.RemoveAlarm(index);
      }
      this.LastSaveTime = DateTime.Now;
    }

    public DateTime LastSaveTime
    {
      get
      {
        return DateTime.FromBinary((long) ((IDictionary<string, object>) ApplicationData.Current.LocalSettings.Values)["AlarmLastSaveTime"]);
      }
      private set
      {
        ((IDictionary<string, object>) ApplicationData.Current.LocalSettings.Values)["AlarmLastSaveTime"] = (object) value.ToBinary();
      }
    }

    private void SaveAlarm(Alarm alarm, int index)
    {
      ((IDictionary<string, object>) ApplicationData.Current.LocalSettings.Values)["Alarm" + (object) index] = (object) this.ToCompositeValue(alarm);
    }

    private void RemoveAlarm(int index)
    {
      ((IDictionary<string, object>) ApplicationData.Current.LocalSettings.Values).Remove("Alarm" + (object) index);
    }

    public bool IsAlarmExpired(Alarm alarm)
    {
      if (alarm.Days != DaysOfWeek.None || !alarm.IsEnabled)
        return false;
      DateTime lastSaveTime = this.LastSaveTime;
      DateTime dateTime = lastSaveTime.Date + alarm.Time;
      if (dateTime < lastSaveTime)
        dateTime = dateTime.AddDays(1.0);
      return DateTime.Now > dateTime;
    }

    private ApplicationDataCompositeValue ToCompositeValue(Alarm alarm)
    {
      ApplicationDataCompositeValue compositeValue = new ApplicationDataCompositeValue();
      ((IDictionary<string, object>) compositeValue)["IsEnabled"] = (object) alarm.IsEnabled;
      ((IDictionary<string, object>) compositeValue)["IsSmart"] = (object) alarm.IsSmart;
      ((IDictionary<string, object>) compositeValue)["Days"] = (object) (int) alarm.Days;
      ((IDictionary<string, object>) compositeValue)["Time"] = (object) alarm.Time.Ticks;
      return compositeValue;
    }

    private Alarm FromCompositeValue(ApplicationDataCompositeValue compositeValue)
    {
      Alarm alarm = new Alarm();
      if (((IDictionary<string, object>) compositeValue).ContainsKey("IsEnabled"))
        alarm.IsEnabled = (bool) ((IDictionary<string, object>) compositeValue)["IsEnabled"];
      if (((IDictionary<string, object>) compositeValue).ContainsKey("IsSmart"))
        alarm.IsSmart = (bool) ((IDictionary<string, object>) compositeValue)["IsSmart"];
      if (((IDictionary<string, object>) compositeValue).ContainsKey("Days"))
        alarm.Days = (DaysOfWeek) ((IDictionary<string, object>) compositeValue)["Days"];
      if (((IDictionary<string, object>) compositeValue).ContainsKey("Time"))
        alarm.Time = new TimeSpan((long) ((IDictionary<string, object>) compositeValue)["Time"]);
      return alarm;
    }
  }
}
