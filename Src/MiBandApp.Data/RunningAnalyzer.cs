
// Type: MiBandApp.Data.RunningAnalyzer
// Assembly: MiBandApp.Data, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6F72CE9F-BCE5-4931-8D08-920048DF42FA
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.Data.dll

using MiBandApp.Data.Activities;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace MiBandApp.Data
{
  public class RunningAnalyzer
  {
    private const int MinRunningDetectedMinutes = 3;
    private readonly List<ActivityMinuteData> _dataList;
    private readonly DistanceCalculator _distanceCalculator;
    private readonly CaloriesCalculator _caloriesCalculator;

    public RunningAnalyzer(UserParameters userParameters, List<ActivityMinuteData> dataList)
    {
      this._dataList = dataList;
      this._distanceCalculator = new DistanceCalculator(userParameters);
      this._caloriesCalculator = new CaloriesCalculator(userParameters);
    }

    public IEnumerable<RunningActivity> GetRunningActivities()
    {
      int analyzedCount = 0;
      while (true)
      {
        int runningStartIndex = this.GetRunningStartIndex(analyzedCount);
        if (runningStartIndex != -1)
        {
          int runningEndIndex = this.GetRunningEndIndex(runningStartIndex);
          List<ActivityMinuteData> list = Enumerable.ToList<ActivityMinuteData>((IEnumerable<ActivityMinuteData>) this._dataList.GetRange(runningStartIndex, runningEndIndex - runningStartIndex + 1));
          double distanceKm = this._distanceCalculator.GetDistanceKm(list);
          int calories = this._caloriesCalculator.GetCalories(distanceKm, runningEndIndex - runningStartIndex + 1, true);
          RunningActivity runningActivity = new RunningActivity()
          {
            Begin = this._dataList[runningStartIndex].Timestamp,
            End = this._dataList[runningEndIndex].Timestamp,
            Steps = Enumerable.Sum<ActivityMinuteData>((IEnumerable<ActivityMinuteData>) list, (Func<ActivityMinuteData, int>) (t => t.Steps)),
            Distance = distanceKm,
            Calories = calories
          };
          if (runningEndIndex - runningStartIndex >= 3)
            yield return runningActivity;
          analyzedCount = runningEndIndex + 1;
        }
        else
          break;
      }
    }

    private int GetRunningStartIndex(int analyzedCount)
    {
      return this._dataList.FindIndex(analyzedCount, (Predicate<ActivityMinuteData>) (t => t.Mode == ActivityMode.Running));
    }

    private int GetRunningEndIndex(int runStartIndex)
    {
      int num = 0;
      int index;
      for (index = runStartIndex; index < this._dataList.Count && num <= 1; ++index)
      {
        if (this._dataList[index].Mode == ActivityMode.Running)
          num = 0;
        else
          ++num;
      }
      return index - num - 1;
    }
  }
}
