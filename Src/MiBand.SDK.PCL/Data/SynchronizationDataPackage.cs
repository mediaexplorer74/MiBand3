
// Type: MiBand.SDK.Data.SynchronizationDataPackage
// Assembly: MiBand.SDK.PCL, Version=0.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 9D9F8E0D-06B7-4D03-9C78-2BA2B5638699
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBand.SDK.PCL.dll

using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace MiBand.SDK.Data
{
  public class SynchronizationDataPackage
  {
    public DateTimeOffset TimeBegin
    {
      get
      {
        return this.ActivitySeries.Count != 0 
                    ? Enumerable.First<RawMinuteActivityDataSeries>(
                        (IEnumerable<RawMinuteActivityDataSeries>) Enumerable.OrderBy<RawMinuteActivityDataSeries, DateTimeOffset>((IEnumerable<RawMinuteActivityDataSeries>) this.ActivitySeries, (Func<RawMinuteActivityDataSeries, DateTimeOffset>) (t => t.StartTime))).StartTime : new DateTimeOffset();
      }
    }

    public DateTimeOffset TimeEnd => this.TimeBegin.AddMinutes((double) this.TotalMinutes);

    public int TotalMinutes
    {
      get
      {
        return Enumerable.Sum<RawMinuteActivityDataSeries>((IEnumerable<RawMinuteActivityDataSeries>) this.ActivitySeries, (Func<RawMinuteActivityDataSeries, int>) (t => t.Data.Count));
      }
    }

      
      public List<RawMinuteActivityDataSeries> ActivitySeries { get; } 
           = new List<RawMinuteActivityDataSeries>();

      public List<HeartRateMeasurement> HeartRateMeasurements { get; set; } 
            = new List<HeartRateMeasurement>();
  }
}
