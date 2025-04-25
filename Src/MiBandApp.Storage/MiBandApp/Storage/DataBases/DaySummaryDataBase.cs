
// Type: MiBandApp.Storage.DataBases.DaySummaryDataBase
// Assembly: MiBandApp.Storage, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 12BDBED5-65C6-40FC-A182-B057969144FE
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.Storage.dll

using MetroLog;
using MiBand.SDK.Data;
using MiBandApp.Data.Activities;
using MiBandApp.Storage.Tables;
using SQLite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using Windows.Storage;

#nullable disable
namespace MiBandApp.Storage.DataBases
{
  public class DaySummaryDataBase
  {
    private readonly MiBandApp.Storage.Settings.Settings _settings;
    private readonly ActivitiesDataBase _activitiesDataBase;
    private readonly RawActivityDataBase _rawActivityDataBase;
    private const string DatabaseFileName = "DaySummary.sqlite";
    private readonly string _dataBaseFilePath = Path.Combine(Path.Combine(ApplicationData.Current.LocalFolder.Path, "DaySummary.sqlite"));
    private readonly object _writeLock = new object();
    private readonly ILogger _log;

    public DaySummaryDataBase(
      MiBandApp.Storage.Settings.Settings settings,
      ILogManager logManager,
      ActivitiesDataBase activitiesDataBase,
      RawActivityDataBase rawActivityDataBase)
    {
      this._settings = settings;
      this._log = logManager.GetLogger<DaySummaryDataBase>();
      this._activitiesDataBase = activitiesDataBase;
      this._rawActivityDataBase = rawActivityDataBase;
    }

    public void Init()
    {
      try
      {
        using (SQLiteConnection connection = this.CreateConnection())
          connection.CreateTable<DaySummary>();
        this.UpdateDeepSleep();
      }
      catch (Exception ex)
      {
        this._log.Error(ex.ToString(), (Exception) null);
      }
    }

    public IReadOnlyList<DaySummary> GetAllDays()
    {
      using (SQLiteConnection connection = this.CreateConnection())
        return (IReadOnlyList<DaySummary>) connection.Table<DaySummary>().ToList<DaySummary>();
    }

    public void UpdateRealtimeSteps(RealtimeStepsData realtimeStepsData)
    {
      lock (this._writeLock)
      {
        DateTime now = DateTime.Now;
        if (now.Hour == 23 && now.Minute >= 59 || now.Hour == 0 && now.Minute <= 1)
          return;
        using (SQLiteConnection connection = this.CreateConnection())
        {
          DaySummary orAddDay = this.GetOrAddDay(DateTime.Now.Date, connection);
          if (realtimeStepsData == null)
              return;
          if (orAddDay.Steps >= realtimeStepsData.TotalSteps)
            return;

          orAddDay.Steps = realtimeStepsData.TotalSteps;
          orAddDay.StepsGoal = this._settings.GetSavedGoalInfo().StepsGoal;
          connection.Update((object) orAddDay);
        }
      }
    }

    public void UpdateFromActivityMinutes(List<ActivityMinuteData> activityMinuteDataList)
    {
      lock (this._writeLock)
      {
        foreach (IGrouping<DateTime, ActivityMinuteData> grouping in activityMinuteDataList.GroupBy<ActivityMinuteData, DateTime>((Func<ActivityMinuteData, DateTime>) (t => t.Timestamp.Date)))
        {
          DaySummary orAddDay = this.GetOrAddDay(grouping.Key);
          List<DbActivityMinuteData> list = this._rawActivityDataBase.GetInDay(grouping.Key).ToList<DbActivityMinuteData>();
          bool flag = false;
          int num = list.Sum<DbActivityMinuteData>((Func<DbActivityMinuteData, int>) (t => t.Steps));
          if (num > orAddDay.Steps)
          {
            orAddDay.Steps = num;
            orAddDay.StepsGoal = this._settings.GetSavedGoalInfo().StepsGoal;
            flag = true;
          }
          int totalMinutes1 = (int) (list.Min<DbActivityMinuteData, DateTime>((Func<DbActivityMinuteData, DateTime>) (t => t.Timestamp)) - grouping.Key).TotalMinutes;
          if (orAddDay.DataBeginMinute < totalMinutes1)
          {
            orAddDay.DataBeginMinute = totalMinutes1;
            flag = true;
          }
          int totalMinutes2 = (int) (list.Max<DbActivityMinuteData, DateTime>((Func<DbActivityMinuteData, DateTime>) (t => t.Timestamp)) - grouping.Key).TotalMinutes;
          if (orAddDay.DataEndMinute < totalMinutes2)
          {
            orAddDay.DataEndMinute = totalMinutes2;
            flag = true;
          }
          if (flag)
          {
            using (SQLiteConnection connection = this.CreateConnection())
              connection.Update((object) orAddDay);
          }
        }
      }
    }

    public void UpdateFromActivities(List<IDbUserActivity> activities)
    {
      lock (this._writeLock)
      {
        foreach (IGrouping<DateTime, DbSleepingActivity> grouping in activities.OfType<DbSleepingActivity>().GroupBy<DbSleepingActivity, DateTime>((Func<DbSleepingActivity, DateTime>) (t => t.End.Date)))
        {
          DaySummary orAddDay = this.GetOrAddDay(grouping.Key);
          List<DbSleepingActivity> list = this._activitiesDataBase.GetActivitiesInDay(grouping.Key).OfType<DbSleepingActivity>().ToList<DbSleepingActivity>();
          bool flag = false;
          int num1 = list.Sum<DbSleepingActivity>((Func<DbSleepingActivity, int>) (t => t.TotalSleepMinutes));
          int num2 = list.Sum<DbSleepingActivity>((Func<DbSleepingActivity, int>) (t => t.DeepSleepMin));
          if (num1 > orAddDay.SleepMinutes)
          {
            orAddDay.SleepMinutes = num1;
            orAddDay.SleepGoalMinutes = this._settings.GetSavedGoalInfo().SleepGoalMinutes;
            orAddDay.DeepSleepMinutes = num2;
            flag = true;
          }
          if (flag)
          {
            using (SQLiteConnection connection = this.CreateConnection())
              connection.Update((object) orAddDay);
          }
        }
        foreach (IGrouping<DateTime, IStepsActivity> grouping in activities.OfType<IStepsActivity>().GroupBy<IStepsActivity, DateTime>((Func<IStepsActivity, DateTime>) (t => t.End.Date)))
        {
          DaySummary orAddDay = this.GetOrAddDay(grouping.Key);
          List<IStepsActivity> list = this._activitiesDataBase.GetActivitiesInDay(grouping.Key).OfType<IStepsActivity>().ToList<IStepsActivity>();
          bool flag = false;
          int num3 = list.Sum<IStepsActivity>((Func<IStepsActivity, int>) (t => t.Calories));
          if (num3 > orAddDay.Calories)
          {
            orAddDay.Calories = num3;
            flag = true;
          }
          double num4 = list.Sum<IStepsActivity>((Func<IStepsActivity, double>) (t => t.Distance));
          if (num4 > orAddDay.Distance)
          {
            orAddDay.Distance = num4;
            flag = true;
          }
          if (flag)
          {
            using (SQLiteConnection connection = this.CreateConnection())
              connection.Update((object) orAddDay);
          }
        }
        foreach (IGrouping<DateTime, IDbUserActivity> grouping in activities.GroupBy<IDbUserActivity, DateTime>((Func<IDbUserActivity, DateTime>) (t => t.Begin.Date)))
          this.RecalculateDay(grouping.Key);
      }
    }

    public void UpdateFromCloud(List<DaySummary> cloudDays)
    {
      lock (this._writeLock)
      {
        List<DaySummary> objects1 = new List<DaySummary>();
        List<DaySummary> objects2 = new List<DaySummary>();
        IReadOnlyList<DaySummary> allDays = this.GetAllDays();
        foreach (DaySummary cloudDay1 in cloudDays)
        {
          DaySummary cloudDay = cloudDay1;
          DaySummary daySummary = allDays.FirstOrDefault<DaySummary>((Func<DaySummary, bool>) (t => t.Date == cloudDay.Date.Date));
          if (daySummary == null)
          {
            this._log.Debug(string.Format("Cloud sync: created new day for {0}", (object) cloudDay.Date), (Exception) null);
            if (cloudDay.DataBeginMinute == 0 && cloudDay.DataEndMinute == 0)
            {
              cloudDay.DataBeginMinute = 0;
              cloudDay.DataEndMinute = 1439;
            }
            objects2.Add(cloudDay);
          }
          else if (daySummary.DataBeginMinute == 0 && daySummary.DataEndMinute == 0 && cloudDay.DataBeginMinute == 0 && cloudDay.DataEndMinute == 0)
          {
            this._log.Debug(string.Format("Cloud sync: updating existing unitialized day {0}", (object) cloudDay.Date), (Exception) null);
            if (daySummary.Steps < cloudDay.Steps)
            {
              daySummary.Steps = cloudDay.Steps;
              daySummary.StepsGoal = cloudDay.StepsGoal;
            }
            if (daySummary.SleepMinutes < cloudDay.SleepMinutes)
            {
              daySummary.SleepGoalMinutes = cloudDay.SleepGoalMinutes;
              daySummary.SleepMinutes = cloudDay.SleepMinutes;
            }
            if (daySummary.DeepSleepMinutes < cloudDay.DeepSleepMinutes)
              daySummary.DeepSleepMinutes = cloudDay.DeepSleepMinutes;
            if (daySummary.Calories < cloudDay.Calories)
              daySummary.Calories = cloudDay.Calories;
            if (daySummary.Distance < cloudDay.Distance)
              daySummary.Distance = cloudDay.Distance;
            if (cloudDay.WasRunning)
              daySummary.WasRunning = cloudDay.WasRunning;
            daySummary.DataBeginMinute = 0;
            daySummary.DataEndMinute = 1439;
            objects1.Add(daySummary);
          }
          else if (cloudDay.DataEndMinute == 0 && cloudDay.DataBeginMinute == 0)
            this._log.Debug(string.Format("Cloud sync: skipping day {0} because not having interval info", (object) cloudDay.Date), (Exception) null);
          else if (daySummary.DataBeginMinute > cloudDay.DataEndMinute)
          {
            this._log.Debug(string.Format("Cloud sync: merging earlier data from day {0}", (object) cloudDay.Date), (Exception) null);
            daySummary.DataBeginMinute = cloudDay.DataBeginMinute;
            daySummary.DeepSleepMinutes += cloudDay.DeepSleepMinutes;
            daySummary.Calories += cloudDay.Calories;
            daySummary.Distance += cloudDay.Distance;
            daySummary.SleepMinutes += cloudDay.SleepMinutes;
            daySummary.Steps += cloudDay.Steps;
            daySummary.WasRunning |= cloudDay.WasRunning;
            objects1.Add(daySummary);
          }
        }
        using (SQLiteConnection connection = this.CreateConnection())
        {
          connection.InsertAll((IEnumerable) objects2);
          connection.UpdateAll((IEnumerable) objects1);
        }
      }
    }

    public List<DaySummary> GetDays(DateTime timeBegin, DateTime timeEnd)
    {
      lock (this._writeLock)
        return this.GetDaysByPredicate((Expression<Func<DaySummary, bool>>) (t => t.Date >= timeBegin && t.Date <= timeEnd)).ToList<DaySummary>();
    }

    public int GetCount(Expression<Func<DaySummary, bool>> predicate)
    {
      lock (this._writeLock)
      {
        using (SQLiteConnection connection = this.CreateConnection())
          return connection.Table<DaySummary>().Where<DaySummary>(predicate.Compile()).Count<DaySummary>();
      }
    }

    public void RecalculateDay(DateTime date)
    {
      lock (this._writeLock)
      {
        DaySummary orAddDay = this.GetOrAddDay(date);
        orAddDay.WasRunning = this._activitiesDataBase.GetActivitiesInDay(date).Any<IDbUserActivity>((Func<IDbUserActivity, bool>) (t => t is DbRunningActivity));
        using (SQLiteConnection connection = this.CreateConnection())
          connection.Update((object) orAddDay);
      }
    }

    private List<DaySummary> GetDaysByPredicate(Expression<Func<DaySummary, bool>> predicate)
    {
      using (SQLiteConnection connection = this.CreateConnection())
        return this.GetDaysByPredicate(predicate, connection);
    }

    private List<DaySummary> GetDaysByPredicate(
      Expression<Func<DaySummary, bool>> predicate,
      SQLiteConnection connection)
    {
      return connection.Table<DaySummary>().Where<DaySummary>(predicate.Compile()).ToList<DaySummary>();
    }

    private SQLiteConnection CreateConnection()
    {
      return new SQLiteConnection(this._dataBaseFilePath, true);
    }

    private DaySummary GetOrAddDay(DateTime date)
    {
      using (SQLiteConnection connection = this.CreateConnection())
        return this.GetOrAddDay(date, connection);
    }

    private DaySummary GetOrAddDay(DateTime date, SQLiteConnection connection)
    {
      date = date.Date;
      DaySummary orAddDay1 = connection.Table<DaySummary>().FirstOrDefault<DaySummary>((Func<DaySummary, bool>) (t => t.Date == date));
      if (orAddDay1 != null)
        return orAddDay1;
      DaySummary orAddDay2 = new DaySummary(date);
      connection.Insert((object) orAddDay2);
      return orAddDay2;
    }

    private void UpdateDeepSleep()
    {
      lock (this._writeLock)
      {
        this._activitiesDataBase.Init();
        List<DbSleepingActivity> allSleepActivities = this._activitiesDataBase.GetAllSleepActivities();
        IReadOnlyList<DaySummary> allDays = this.GetAllDays();
        DbSleepingActivity firstDeepSleep = allSleepActivities.FirstOrDefault<DbSleepingActivity>((Func<DbSleepingActivity, bool>) (t => t.DeepSleepMin != 0));
        if (firstDeepSleep == null)
          return;
        DaySummary daySummary1 = allDays.FirstOrDefault<DaySummary>((Func<DaySummary, bool>) (t => t.Date == firstDeepSleep.End.Date));
        if (daySummary1 == null || daySummary1.DeepSleepMinutes != 0)
          return;
        List<DaySummary> objects = new List<DaySummary>();
        foreach (IGrouping<DateTime, DbSleepingActivity> grouping in allSleepActivities.GroupBy<DbSleepingActivity, DateTime>((Func<DbSleepingActivity, DateTime>) (t => t.End.Date)))
        {
          IGrouping<DateTime, DbSleepingActivity> daySleep = grouping;
          DaySummary daySummary2 = allDays.FirstOrDefault<DaySummary>((Func<DaySummary, bool>) (t => t.Date == daySleep.Key));
          if (daySummary2 != null)
          {
            int num = daySleep.Sum<DbSleepingActivity>((Func<DbSleepingActivity, int>) (t => t.DeepSleepMin));
            if (num > daySummary2.DeepSleepMinutes)
            {
              daySummary2.DeepSleepMinutes = num;
              objects.Add(daySummary2);
            }
          }
        }
        using (SQLiteConnection connection = this.CreateConnection())
          connection.UpdateAll((IEnumerable) objects);
      }
    }
  }
}
