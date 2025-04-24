
// Type: MiBandApp.Data.WalkAnalyzer
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
  public class WalkAnalyzer
  {
    private readonly List<ActivityMinuteData> _dataList;
    private readonly DistanceCalculator _distanceCalculator;
    private readonly CaloriesCalculator _caloriesCalculator;

    public WalkAnalyzer(UserParameters userParameters, List<ActivityMinuteData> dataList)
    {
      this._dataList = dataList;
      this._distanceCalculator = new DistanceCalculator(userParameters);
      this._caloriesCalculator = new CaloriesCalculator(userParameters);
    }

    public IEnumerable<WalkingActivity> GetWalkingActivities()
    {
      int analyzedCount = 0;
      while (true)
      {
        int walkStartIndex = this.GetWalkStartIndex(analyzedCount);
        if (walkStartIndex != -1)
        {
          int walkEndIndex = this.GetWalkEndIndex(walkStartIndex);
          List<ActivityMinuteData> list = Enumerable.ToList<ActivityMinuteData>((IEnumerable<ActivityMinuteData>) this._dataList.GetRange(walkStartIndex, walkEndIndex - walkStartIndex + 1));
          double distanceKm = this._distanceCalculator.GetDistanceKm(list);
          int calories = this._caloriesCalculator.GetCalories(distanceKm, walkEndIndex - walkStartIndex + 1, false);
          yield return new WalkingActivity()
          {
            Begin = this._dataList[walkStartIndex].Timestamp,
            End = this._dataList[walkEndIndex].Timestamp,
            Steps = Enumerable.Sum<ActivityMinuteData>((IEnumerable<ActivityMinuteData>) list, (Func<ActivityMinuteData, int>) (t => t.Steps)),
            Distance = distanceKm,
            Calories = calories
          };
          analyzedCount = walkEndIndex + 1;
        }
        else
          break;
      }
    }

    private int GetWalkStartIndex(int analyzedCount)
    {
      return this._dataList.FindIndex(analyzedCount, (Predicate<ActivityMinuteData>) (t => t.Mode == ActivityMode.Walking));
    }

    private int GetWalkEndIndex(int walkStartIndex)
    {
      int num = 0;
      int index;
      for (index = walkStartIndex; index < this._dataList.Count && num <= 1; ++index)
      {
        switch (this._dataList[index].Mode)
        {
          case ActivityMode.Walking:
          case ActivityMode.Running:
            num = 0;
            break;
          default:
            if (this._dataList[index].Steps <= 0)
            {
              ++num;
              break;
            }
            goto case ActivityMode.Walking;
        }
      }
      return index - num - 1;
    }
  }
}
