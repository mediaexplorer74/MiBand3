
// Type: MiBandApp.Data.Raw.RawActivityDataProcessor
// Assembly: MiBandApp.Data, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6F72CE9F-BCE5-4931-8D08-920048DF42FA
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.Data.dll

using MetroLog;
using MiBand.SDK.Data;
using MiBandApp.Data.Activities;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace MiBandApp.Data.Raw
{
  public class RawActivityDataProcessor
  {
    private readonly ILogger _log;

    public RawActivityDataProcessor(ILogManager logManager)
    {
      this._log = logManager.GetLogger<RawActivityDataProcessor>();
    }

    public List<ActivityMinuteData> GetProcessedMinuteData(
      IEnumerable<RawMinuteActivityDataSeries> series)
    {
      List<ActivityMinuteData> minuteData = this.GetMinuteData(series);
      if (minuteData.Count == 0)
        return new List<ActivityMinuteData>();
      this.MergeDuplicatingByTime(minuteData);
      this.FillSmallGaps(minuteData);
      this.DetectNotWearing(minuteData);
      return minuteData;
    }

    private List<ActivityMinuteData> GetMinuteData(RawMinuteActivityDataSeries rawActivityDataSeries)
    {
      List<ActivityMinuteData> minuteData = new List<ActivityMinuteData>();
      int num = 0;
      using (List<RawMinuteActivityData>.Enumerator enumerator = rawActivityDataSeries.Data.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          RawMinuteActivityData current = enumerator.Current;
          minuteData.Add(new ActivityMinuteData(current, rawActivityDataSeries.StartTime.AddMinutes((double) num++)));
        }
      }
      return minuteData;
    }

    private List<ActivityMinuteData> GetMinuteData(
      IEnumerable<RawMinuteActivityDataSeries> seriesList)
    {
      seriesList = (IEnumerable<RawMinuteActivityDataSeries>) Enumerable.OrderBy<RawMinuteActivityDataSeries, DateTimeOffset>(seriesList, (Func<RawMinuteActivityDataSeries, DateTimeOffset>) (t => t.StartTime));
      List<ActivityMinuteData> minuteData = new List<ActivityMinuteData>();
      foreach (RawMinuteActivityDataSeries series in seriesList)
      {
        ActivityMinuteData activityMinuteData = Enumerable.LastOrDefault<ActivityMinuteData>((IEnumerable<ActivityMinuteData>) minuteData);
        if (activityMinuteData != null)
        {
          int totalMinutes = (int) (series.StartTime - (DateTimeOffset) activityMinuteData.Timestamp).TotalMinutes;
          int num = 1;
          while (totalMinutes-- > 1)
            minuteData.Add(new ActivityMinuteData((DateTimeOffset) activityMinuteData.Timestamp.AddMinutes((double) num++)));
        }
        minuteData.AddRange((IEnumerable<ActivityMinuteData>) this.GetMinuteData(series));
      }
      return minuteData;
    }

    private void FillSmallGaps(List<ActivityMinuteData> activityData)
    {
      for (int index = 1; index < activityData.Count - 1; ++index)
      {
        if (activityData[index].Mode == ActivityMode.NoData && activityData[index - 1].Mode != ActivityMode.NoData && activityData[index + 1].Mode != ActivityMode.NoData)
        {
          activityData[index].Mode = activityData[index + 1].Mode;
          activityData[index].Activity = (activityData[index - 1].Activity + activityData[index + 1].Activity) / 2;
        }
      }
    }

    private void DetectNotWearing(List<ActivityMinuteData> activityData)
    {
      for (int index = 0; index < activityData.Count - 60; ++index)
      {
        ActivityMinuteData activityMinuteData1 = activityData[index];
        bool flag = activityMinuteData1.Timestamp.Hour > 22 || activityMinuteData1.Timestamp.Hour < 7;
        int num = flag ? 300 : 60;
        int notWearingActivity = flag ? 2 : 0;
        if (activityData.Count >= index + num && !Enumerable.Any<ActivityMinuteData>(Enumerable.Take<ActivityMinuteData>(Enumerable.Skip<ActivityMinuteData>((IEnumerable<ActivityMinuteData>) activityData, index), num), (Func<ActivityMinuteData, bool>) (t =>
        {
          if (t.Activity > notWearingActivity)
            return true;
          return t.HeartRate != 0 && t.HeartRate != (int) byte.MaxValue;
        })))
        {
          if (activityData[index].Mode != ActivityMode.NotWearing)
            this._log.Warn("Not wearing detected at " + (object) activityData[index].Timestamp, (Exception) null);
          foreach (ActivityMinuteData activityMinuteData2 in Enumerable.Take<ActivityMinuteData>(Enumerable.Skip<ActivityMinuteData>((IEnumerable<ActivityMinuteData>) activityData, index), num))
            activityMinuteData2.Mode = ActivityMode.NotWearing;
        }
      }
    }

    private void MergeDuplicatingByTime(List<ActivityMinuteData> dataToProcess)
    {
      dataToProcess.Sort((Comparison<ActivityMinuteData>) ((a, b) => (int) (a.Timestamp - b.Timestamp).TotalMinutes));
      int index = 0;
      while (index < dataToProcess.Count - 1)
      {
        ActivityMinuteData activityMinuteData1 = dataToProcess[index];
        ActivityMinuteData activityMinuteData2 = dataToProcess[index + 1];
        if (activityMinuteData1.Timestamp == activityMinuteData2.Timestamp)
        {
          activityMinuteData1.Activity += activityMinuteData2.Activity;
          activityMinuteData1.Steps += activityMinuteData2.Steps;
          if (activityMinuteData2.Mode > activityMinuteData1.Mode)
          {
            activityMinuteData1.Mode = activityMinuteData2.Mode;
            activityMinuteData1.Runs = activityMinuteData2.Runs;
          }
          dataToProcess.RemoveAt(index + 1);
        }
        else
          ++index;
      }
    }
  }
}
