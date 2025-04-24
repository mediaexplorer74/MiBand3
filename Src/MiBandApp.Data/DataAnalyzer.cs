
// Type: MiBandApp.Data.DataAnalyzer
// Assembly: MiBandApp.Data, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6F72CE9F-BCE5-4931-8D08-920048DF42FA
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.Data.dll

using MiBandApp.Data.Activities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace MiBandApp.Data
{
  public class DataAnalyzer
  {
    private readonly UserParameters _userParameters;
    private readonly SleepAnalyzer _sleepAnalyzer = new SleepAnalyzer();

    public DataAnalyzer(UserParameters userParameters) => this._userParameters = userParameters;

    public IEnumerable<IUserActivity> GetActivities(
      List<ActivityMinuteData> dataList,
      IEnumerable<IUserActivity> previousDayActivities = null)
    {
      if (previousDayActivities == null)
        previousDayActivities = (IEnumerable<IUserActivity>) new List<IUserActivity>();
      List<IUserActivity> userActivityList = new List<IUserActivity>();
      List<SleepingActivity> list = Enumerable.ToList<SleepingActivity>(this._sleepAnalyzer.GetSleepInfo(dataList, Enumerable.OfType<SleepingActivity>((IEnumerable) previousDayActivities)));
      userActivityList.AddRange((IEnumerable<IUserActivity>) list);
      foreach (List<ActivityMinuteData> fragmentsWithoutActivity in this.GetFragmentsWithoutActivities(dataList, Enumerable.ToList<IUserActivity>((IEnumerable<IUserActivity>) userActivityList)))
      {
        IEnumerable<RunningActivity> runningActivities = new RunningAnalyzer(this._userParameters, fragmentsWithoutActivity).GetRunningActivities();
        userActivityList.AddRange((IEnumerable<IUserActivity>) runningActivities);
      }
      foreach (List<ActivityMinuteData> fragmentsWithoutActivity in this.GetFragmentsWithoutActivities(dataList, Enumerable.ToList<IUserActivity>((IEnumerable<IUserActivity>) userActivityList)))
      {
        IEnumerable<WalkingActivity> walkingActivities = new WalkAnalyzer(this._userParameters, fragmentsWithoutActivity).GetWalkingActivities();
        userActivityList.AddRange((IEnumerable<IUserActivity>) walkingActivities);
      }
      return (IEnumerable<IUserActivity>) Enumerable.OrderBy<IUserActivity, DateTime>((IEnumerable<IUserActivity>) userActivityList, (Func<IUserActivity, DateTime>) (t => t.Begin));
    }

    private IEnumerable<List<ActivityMinuteData>> GetFragmentsWithoutActivities(
      List<ActivityMinuteData> data,
      List<IUserActivity> userActivities)
    {
      int fragmentStartIndex = 0;
      int fragmentEndIndex = 0;
      int activityIndex = 0;
      if (userActivities.Count == 0)
      {
        yield return data;
      }
      else
      {
        userActivities = Enumerable.ToList<IUserActivity>((IEnumerable<IUserActivity>) Enumerable.OrderBy<IUserActivity, DateTime>((IEnumerable<IUserActivity>) userActivities, (Func<IUserActivity, DateTime>) (t => t.Begin)));
        if (userActivities[activityIndex].Begin <= data[fragmentStartIndex].Timestamp && userActivities[activityIndex].End >= data[fragmentStartIndex].Timestamp)
        {
          fragmentStartIndex = data.FindIndex(fragmentEndIndex, (Predicate<ActivityMinuteData>) (t => t.Timestamp > userActivities[activityIndex].End));
          ++activityIndex;
        }
        while (fragmentStartIndex < data.Count && fragmentStartIndex != -1)
        {
          if (userActivities.Count == activityIndex)
          {
            fragmentEndIndex = data.Count;
          }
          else
          {
            fragmentEndIndex = data.FindIndex(fragmentStartIndex, (Predicate<ActivityMinuteData>) (t => t.Timestamp >= userActivities[activityIndex].Begin));
            if (fragmentEndIndex == -1)
              fragmentEndIndex = data.Count;
          }
          int count = fragmentEndIndex - fragmentStartIndex;
          if (count != 0)
            yield return data.GetRange(fragmentStartIndex, count);
          if (fragmentEndIndex == data.Count || userActivities.Count == activityIndex)
            break;
          fragmentStartIndex = data.FindIndex(fragmentEndIndex, (Predicate<ActivityMinuteData>) (t => t.Timestamp > userActivities[activityIndex].End));
          ++activityIndex;
        }
      }
    }
  }
}
