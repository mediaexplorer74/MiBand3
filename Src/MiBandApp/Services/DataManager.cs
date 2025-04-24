
// Type: MiBandApp.Services.DataManager
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// 

using MetroLog;
using MiBand.SDK.Configuration;
using MiBand.SDK.Data;
using MiBandApp.Data;
using MiBandApp.Data.Activities;
using MiBandApp.Data.Raw;
using MiBandApp.Storage.DataBases;
using MiBandApp.Storage.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

#nullable disable
namespace MiBandApp.Services
{
  public class DataManager
  {
    private readonly BandController _bandController;
    private readonly DaySummaryDataBase _daySummaryDataBase;
    private readonly RawActivityDataProcessor _rawActivityDataProcessor;
    private readonly ActivitiesDataBase _activitiesDataBase;
    private readonly RawActivityDataBase _rawActivityDataBase;
    private readonly MiBandApp.Storage.Settings.Settings _settings;
    private readonly ILogger _log;

    public DataManager(
      ILogManager logManager,
      BandController bandController,
      DaySummaryDataBase daySummaryDataBase,
      ActivitiesDataBase activitiesDataBase,
      RawActivityDataBase rawActivityDataBase,
      MiBandApp.Storage.Settings.Settings settings)
    {
      this._log = logManager.GetLogger<DataManager>();
      this._bandController = bandController;
      this._daySummaryDataBase = daySummaryDataBase;
      this._activitiesDataBase = activitiesDataBase;
      this._rawActivityDataBase = rawActivityDataBase;
      this._settings = settings;
      this._rawActivityDataProcessor = new RawActivityDataProcessor(logManager);
      this._activitiesDataBase.Init();
      this._rawActivityDataBase.Init();
      this._daySummaryDataBase.Init();
    }

    public void UpdateRealtimeSteps(RealtimeStepsData realtimeStepsData)
    {
      this._daySummaryDataBase.UpdateRealtimeSteps(realtimeStepsData);
    }

    public void AddRawActivityData(
      List<RawMinuteActivityDataSeries> rawActivityDataSeries)
    {
      try
      {
        List<ActivityMinuteData> processedMinuteData = this._rawActivityDataProcessor.GetProcessedMinuteData((IEnumerable<RawMinuteActivityDataSeries>) rawActivityDataSeries);
        if (processedMinuteData.Count == 0)
          return;
        this._rawActivityDataBase.AddOrReplace(processedMinuteData.Select<ActivityMinuteData, DbActivityMinuteData>((Func<ActivityMinuteData, DbActivityMinuteData>) (t => new DbActivityMinuteData(t))));
        this._daySummaryDataBase.UpdateFromActivityMinutes(processedMinuteData);
        List<ActivityMinuteData> extendedIntervalData = this.TryGetExtendedIntervalData(processedMinuteData);
        DataAnalyzer dataAnalyzer = new DataAnalyzer(this.GetUserParameters());
        IEnumerable<IUserActivity> activitiesDayBefore = this.GetActivitiesDayBefore(extendedIntervalData[0].Timestamp);
        List<ActivityMinuteData> dataList = extendedIntervalData;
        IEnumerable<IUserActivity> previousDayActivities = activitiesDayBefore;
        List<IUserActivity> list1 = dataAnalyzer.GetActivities(dataList, previousDayActivities).ToList<IUserActivity>();
        foreach (IUserActivity userActivity in list1)
          ;
        if (list1.Count == 0)
          return;
        List<IDbUserActivity> list2 = list1.Select<IUserActivity, IDbUserActivity>((Func<IUserActivity, IDbUserActivity>) (t => t.ToDbUserActivity())).ToList<IDbUserActivity>();
        this._activitiesDataBase.AddOrReplaceActivities((IEnumerable<IDbUserActivity>) list2);
        this._daySummaryDataBase.UpdateFromActivities(list2);
      }
      catch (Exception ex)
      {
        this._log.Error("Error during adding raw data " + (object) ex, (Exception) null);
        throw;
      }
    }

    public void AddCloudActivityData(List<DaySummary> days)
    {
      this._daySummaryDataBase.UpdateFromCloud(days);
    }

    public void AddHeartRateMeasurement(HeartRateMeasurement hr)
    {
      this.AddHeartRateMeasurements((IEnumerable<HeartRateMeasurement>) new List<HeartRateMeasurement>()
      {
        hr
      });
    }

    public void AddHeartRateMeasurements(
      IEnumerable<HeartRateMeasurement> heartRateMeasurements)
    {
      this._activitiesDataBase.AddActivities((IEnumerable<IDbUserActivity>) heartRateMeasurements.Select(hr => new
      {
        hr = hr,
        timestamp = hr.Timestamp.DateTime
      }).Select(_param1 => new DbHeartRateMeasureActivity(new HeartRateMeasureActivity()
      {
        Begin = _param1.timestamp,
        End = _param1.timestamp,
        HeartRate = (int) _param1.hr.HeartRateValue
      })));
    }

    private IEnumerable<IUserActivity> GetActivitiesDayBefore(DateTime timestamp)
    {
      return (IEnumerable<IUserActivity>) this._activitiesDataBase.GetActivities((Expression<Func<IDbUserActivity, bool>>) (t => t.End > timestamp.AddDays(-1.0) && t.End < timestamp));
    }

    private List<ActivityMinuteData> TryGetExtendedIntervalData(List<ActivityMinuteData> data)
    {
      List<IDbUserActivity> list1 = this._activitiesDataBase.GetActivities((Expression<Func<IDbUserActivity, bool>>) (t => (data[0].Timestamp - t.End).TotalMinutes < 15.0)).OrderBy<IDbUserActivity, DateTime>((Func<IDbUserActivity, DateTime>) (t => t.Begin)).ToList<IDbUserActivity>();
      if (list1.Count != 0 && (list1[list1.Count - 1] is WalkingActivity || list1[list1.Count - 1] is RunningActivity))
      {
        List<DbActivityMinuteData> list2 = this._rawActivityDataBase.GetInInterval(list1.OrderBy<IDbUserActivity, DateTime>((Func<IDbUserActivity, DateTime>) (t => t.Begin)).First<IDbUserActivity>().Begin, data.Last<ActivityMinuteData>().Timestamp).OrderBy<DbActivityMinuteData, DateTime>((Func<DbActivityMinuteData, DateTime>) (t => t.Timestamp)).ToList<DbActivityMinuteData>();
        if (list2.Count - 1 == (int) (list2.Last<DbActivityMinuteData>().Timestamp - list2.First<DbActivityMinuteData>().Timestamp).TotalMinutes)
          return list2.Cast<ActivityMinuteData>().ToList<ActivityMinuteData>();
        this._log.Warn("Extended data contained gaps. Returned original.", (Exception) null);
      }
      return data;
    }

    private UserParameters GetUserParameters()
    {
      UserInfo savedUserInfo = this._settings.GetSavedUserInfo();
      return new UserParameters()
      {
        HeightCm = savedUserInfo.HeightCm,
        IsMale = savedUserInfo.IsMale,
        WeightKg = savedUserInfo.WeightKg
      };
    }
  }
}
