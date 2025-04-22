// Decompiled with JetBrains decompiler
// Type: MiBandApp.Data.Activities.ActivityDataListExtensions
// Assembly: MiBandApp.Data, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6F72CE9F-BCE5-4931-8D08-920048DF42FA
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.Data.dll

using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace MiBandApp.Data.Activities
{
  public static class ActivityDataListExtensions
  {
    public static double GetAverageActivity(
      this List<ActivityMinuteData> dataList,
      int begin,
      int length)
    {
      return Enumerable.Average<ActivityMinuteData>(Enumerable.Take<ActivityMinuteData>(Enumerable.Skip<ActivityMinuteData>((IEnumerable<ActivityMinuteData>) dataList, begin), length), (Func<ActivityMinuteData, int>) (t => t.Activity));
    }
  }
}
