
// Type: MiBandApp.Data.SleepAnalyzer
// Assembly: MiBandApp.Data, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6F72CE9F-BCE5-4931-8D08-920048DF42FA
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.Data.dll

using MiBandApp.Data.Activities;
using MiBandApp.Data.Sleep;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace MiBandApp.Data
{
  public class SleepAnalyzer
  {
    private bool _detectedForMiBand2;
    public const int LowActivityThreshold = 6;
    public const int HighActivityThreshold = 15;
    private const int NotInBedActivityThreshold = 30;
    public const int MinsOfHighActivityAsAwake = 5;
    private const int MinsOfLowActivityAsSleep = 3;
    private const int MinsOfNotInBedActivity = 15;

    public IEnumerable<SleepingActivity> GetSleepInfo(
      List<ActivityMinuteData> dataList,
      IEnumerable<SleepingActivity> prevDaySleepingActivities)
    {
      List<SleepingActivity> previousSleeps = Enumerable.ToList<SleepingActivity>(prevDaySleepingActivities);
      int skipNumber = 0;
      while (true)
      {
        int startSleepIndex = this.FindStartSleepIndex(dataList, skipNumber);
        if (startSleepIndex != -1)
        {
          int num1 = skipNumber + this.FindStartOnBedIndex(Enumerable.ToList<ActivityMinuteData>(Enumerable.Skip<ActivityMinuteData>((IEnumerable<ActivityMinuteData>) dataList, skipNumber)), startSleepIndex - skipNumber);
          int correctedStartSleepIndex = this.FindCorrectedStartSleepIndex(dataList, num1, startSleepIndex);
          int stopInBedIndex = this.FindStopInBedIndex(dataList, correctedStartSleepIndex);
          int stopSleepIndex = this.FindStopSleepIndex(dataList, correctedStartSleepIndex, stopInBedIndex);
          List<ActivityMinuteData> sleepDataList = Enumerable.ToList<ActivityMinuteData>(Enumerable.Skip<ActivityMinuteData>(Enumerable.Take<ActivityMinuteData>((IEnumerable<ActivityMinuteData>) dataList, stopSleepIndex), correctedStartSleepIndex));
          int timeAsleep = Enumerable.Count<ActivityMinuteData>((IEnumerable<ActivityMinuteData>) sleepDataList, (Func<ActivityMinuteData, bool>) (t => t.Activity < 6));
          int num2 = Enumerable.Count<ActivityMinuteData>((IEnumerable<ActivityMinuteData>) sleepDataList, (Func<ActivityMinuteData, bool>) (t => t.Activity >= 15));
          int miBand2LongestNonWearing = SleepAnalyzer.CalcMiBand2LongestNonWearing(sleepDataList);
          int num3 = timeAsleep;
          if (num2 > num3 || timeAsleep < 10 || Enumerable.Any<ActivityMinuteData>((IEnumerable<ActivityMinuteData>) sleepDataList, (Func<ActivityMinuteData, bool>) (t =>
          {
            if (t.Mode == ActivityMode.NotWearing || t.Mode == ActivityMode.Charging)
              return true;
            return this._detectedForMiBand2 && (double) miBand2LongestNonWearing > 0.3 * (double) timeAsleep;
          })))
          {
            skipNumber = stopInBedIndex + 1;
          }
          else
          {
            SleepPattern sleepPattern = new SleepPatternAnalyzer(sleepDataList, Enumerable.Where<SleepingActivity>((IEnumerable<SleepingActivity>) previousSleeps, (Func<SleepingActivity, bool>) (t => t.End > sleepDataList[0].Timestamp.AddHours(-1.0)))).GetSleepPattern();
            int[] heartRates = new HeartRateProcessor().NormalizeHeartRate(dataList, num1, stopInBedIndex);
            SleepingActivity activity = new SleepingActivity()
            {
              Awakenings = Enumerable.Count<SleepPhase>((IEnumerable<SleepPhase>) sleepPattern.Phases, (Func<SleepPhase, bool>) (t => t.Type == SleepPhaseType.Awake)),
              End = dataList[stopInBedIndex].Timestamp,
              EndSleep = dataList[stopSleepIndex].Timestamp,
              Begin = dataList[num1].Timestamp,
              BeginSleep = dataList[correctedStartSleepIndex].Timestamp,
              HeartRateString = HeartRateProcessor.GetHeartRateString(heartRates)
            };
            SleepAnalyzer.ApplyPatternToSleep(activity, sleepPattern);
            previousSleeps.Add(activity);
            yield return activity;
            skipNumber = stopInBedIndex;
          }
        }
        else
          break;
      }
    }

    public static void ApplyPatternToSleep(SleepingActivity activity, SleepPattern pattern)
    {
      int num = Enumerable.Sum<SleepPhase>(Enumerable.Where<SleepPhase>((IEnumerable<SleepPhase>) pattern.Phases, (Func<SleepPhase, bool>) (t => t.Type == SleepPhaseType.Deep)), (Func<SleepPhase, int>) (t => t.Length));
      activity.DeepSleepMin = num;
      activity.SleepPatternString = pattern.ToString();
    }

    private int FindStartSleepIndex(List<ActivityMinuteData> dataList, int skipNumber)
    {
      for (int index1 = skipNumber; index1 < dataList.Count; ++index1)
      {
        if (dataList[index1].Mode == ActivityMode.LightSleep || dataList[index1].Mode == ActivityMode.DeepSleep)
          return index1;
        if (dataList[index1].Mode == (ActivityMode) 11)
        {
          this._detectedForMiBand2 = true;
          return index1;
        }
        if (dataList.Count - index1 < 60)
          return -1;
        int num1 = 0;
        int num2 = 0;
        int startSleepIndex = -1;
        int num3 = 0;
        for (int index2 = index1; index2 < index1 + 60; ++index2)
        {
          if (dataList[index2].Activity < 6)
          {
            ++num1;
            if (startSleepIndex == -1)
              startSleepIndex = index2;
          }
          else if (startSleepIndex != -1 && dataList[index2].Activity < 15)
            ++num2;
          if (dataList[index2].Activity == 0)
            ++num3;
        }
        if ((double) num1 >= 54.0 && (double) num2 <= 3.0 && (double) num3 <= 57.0)
          return startSleepIndex;
      }
      return -1;
    }

    private int FindStartOnBedIndex(List<ActivityMinuteData> dataList, int startSleepIndex)
    {
      ActivityMinuteData activityMinuteData = Enumerable.LastOrDefault<ActivityMinuteData>(Enumerable.Take<ActivityMinuteData>((IEnumerable<ActivityMinuteData>) dataList, startSleepIndex), (Func<ActivityMinuteData, bool>) (t => t.Steps > 0));
      return activityMinuteData == null ? 0 : dataList.IndexOf(activityMinuteData);
    }

    private int FindCorrectedStartSleepIndex(
      List<ActivityMinuteData> dataList,
      int startOnBedIndex,
      int startSleepIndex)
    {
      List<ActivityMinuteData> truncatedList = Enumerable.ToList<ActivityMinuteData>(Enumerable.Reverse<ActivityMinuteData>(Enumerable.Skip<ActivityMinuteData>(Enumerable.Take<ActivityMinuteData>((IEnumerable<ActivityMinuteData>) dataList, startSleepIndex), startOnBedIndex)));
      ActivityMinuteData activityMinuteData = Enumerable.FirstOrDefault<ActivityMinuteData>((IEnumerable<ActivityMinuteData>) truncatedList, (Func<ActivityMinuteData, bool>) (t => truncatedList.GetAverageActivity(truncatedList.IndexOf(t), 3) > 6.0));
      return activityMinuteData == null ? startSleepIndex : dataList.IndexOf(activityMinuteData) + 1;
    }

    private int FindStopInBedIndex(List<ActivityMinuteData> dataList, int startSleepIndex)
    {
      for (int begin = startSleepIndex; begin < dataList.Count; ++begin)
      {
        if (dataList.GetAverageActivity(begin, 15) > 30.0)
        {
          do
          { }
          while (begin < dataList.Count && dataList[begin++].Steps <= 0);
          return begin - 1;
        }
      }
      return dataList.Count - 1;
    }

    private int FindStopSleepIndex(
      List<ActivityMinuteData> dataList,
      int startSleepIndex,
      int stopInBedIndex)
    {
      for (int begin = stopInBedIndex - 5; begin > startSleepIndex; --begin)
      {
        if (dataList.GetAverageActivity(begin, 5) < 15.0)
          return begin;
      }
      return stopInBedIndex - 1;
    }

    private static int CalcMiBand2LongestNonWearing(List<ActivityMinuteData> sleepDataList)
    {
      int num1 = 0;
      int num2 = 0;
      using (List<ActivityMinuteData>.Enumerator enumerator = sleepDataList.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          if (enumerator.Current.Mode == ActivityMode.NonWear)
          {
            ++num2;
          }
          else
          {
            if (num2 > num1)
              num1 = num2;
            num2 = 0;
          }
        }
      }
      return num1;
    }
  }
}
