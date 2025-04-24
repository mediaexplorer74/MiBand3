
// Type: MiBandApp.Storage.Tables.IUserActivityExtensions
// Assembly: MiBandApp.Storage, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 12BDBED5-65C6-40FC-A182-B057969144FE
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.Storage.dll

using MiBandApp.Data.Activities;
using System;

#nullable disable
namespace MiBandApp.Storage.Tables
{
  public static class IUserActivityExtensions
  {
    public static IDbUserActivity ToDbUserActivity(this IUserActivity userActivity)
    {
      Type type = userActivity.GetType();
      if (type == typeof (WalkingActivity))
        return (IDbUserActivity) new DbWalkingActivity((WalkingActivity) userActivity);
      if (type == typeof (SleepingActivity))
        return (IDbUserActivity) new DbSleepingActivity((SleepingActivity) userActivity);
      if (type == typeof (HeartRateMeasureActivity))
        return (IDbUserActivity) new DbHeartRateMeasureActivity((HeartRateMeasureActivity) userActivity);
      if (type == typeof (RunningActivity))
        return (IDbUserActivity) new DbRunningActivity((RunningActivity) userActivity);
      throw new NotImplementedException();
    }
  }
}
