// Decompiled with JetBrains decompiler
// Type: MiBand.SDK.Core.MiBandTwo.MiBandTwoActivityDataLoader
// Assembly: MiBand.SDK, Version=0.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 152FA565-3249-4054-A361-01D9C0AFA7F1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBand.SDK.dll

using MiBand.SDK.Data;
using MiBand.SDK.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

#nullable disable
namespace MiBand.SDK.Core.MiBandTwo
{
  internal class MiBandTwoActivityDataLoader
  {
    private readonly ILog _log;
    private readonly MiBandTwoDataLoader _dataLoader;

    public MiBandTwoActivityDataLoader(MiBand.SDK.Core.MiBandTwo.MiBandTwo device, ILog log)
    {
      this._log = log;
      this._dataLoader = new MiBandTwoDataLoader(device, log);
    }

    public async Task<SynchronizationDataPackage> Sync(DateTimeOffset searchTime)
    {
      this._log.Debug(string.Format("Searching for activity data from time {0}", (object) searchTime));
      try
      {
        SynchronizationDataPackage resultPackage = new SynchronizationDataPackage();
        bool activitesResult = await this.SyncActivities(searchTime, resultPackage);
        int num = await this.SyncHeartRate(searchTime, resultPackage) ? 1 : 0;
        return !(activitesResult & num != 0) ? (SynchronizationDataPackage) null : resultPackage;
      }
      catch (Exception ex)
      {
        this._log.Warning("Exception while loading activity data: " + (object) ex);
        return (SynchronizationDataPackage) null;
      }
    }

    private async Task<bool> SyncActivities(
      DateTimeOffset searchTime,
      SynchronizationDataPackage resultPackage)
    {
      while (true)
      {
        MiBandTwoDataSyncFragment fragment;
        MiBandTwoDataSyncFragment dataSyncFragment = fragment;
        fragment = await this._dataLoader.GetData(searchTime, MiBandTwoDataLoaderKind.Activity);
        if (fragment != null)
        {
          if (fragment.ItemsCount != 0 && !resultPackage.ActivitySeries.Any<RawMinuteActivityDataSeries>((Func<RawMinuteActivityDataSeries, bool>) (t => t.StartTime == fragment.StartTime)))
          {
            RawMinuteActivityDataSeries activityDataSeries = this.PostProcessActivitiesFragment(fragment);
            if (activityDataSeries.Data.Count == fragment.ItemsCount)
            {
              resultPackage.ActivitySeries.Add(activityDataSeries);
              searchTime = fragment.StartTime.AddMinutes((double) fragment.ItemsCount);
            }
            else
              goto label_7;
          }
          else
            goto label_5;
        }
        else
          break;
      }
      return false;
label_5:
      return true;
label_7:
      throw new InvalidDataException("Number of received minutes is not the same as in data header");
    }

    private async Task<bool> SyncHeartRate(
      DateTimeOffset searchTime,
      SynchronizationDataPackage resultPackage)
    {
      MiBandTwoDataSyncFragment data = await this._dataLoader.GetData(searchTime, MiBandTwoDataLoaderKind.HeartRate);
      if (data == null)
        return false;
      if (data.ItemsCount == 0)
        return true;
      List<HeartRateMeasurement> collection = this.PostProcessHeartRateFragment(data);
      if (collection.Count != data.ItemsCount)
        throw new InvalidDataException("Number of received heart rate measurements is not the same as in data header");
      resultPackage.HeartRateMeasurements.AddRange((IEnumerable<HeartRateMeasurement>) collection);
      return true;
    }

    private RawMinuteActivityDataSeries PostProcessActivitiesFragment(
      MiBandTwoDataSyncFragment fragment)
    {
      RawMinuteActivityDataSeries activityDataSeries = new RawMinuteActivityDataSeries();
      activityDataSeries.StartTime = fragment.StartTime;
      for (int index1 = 0; index1 < fragment.RawDataLines.Count; ++index1)
      {
        byte[] rawDataLine = fragment.RawDataLines[index1];
        if ((int) rawDataLine[0] != index1 % 256)
          throw new InvalidDataException("Data in buffer is in wrong order.");
        for (int index2 = 1; index2 < rawDataLine.Length; index2 += 4)
        {
          byte num1 = rawDataLine[index2];
          byte num2 = rawDataLine[index2 + 1];
          byte num3 = rawDataLine[index2 + 2];
          byte num4 = rawDataLine[index2 + 3];
          if (num1 == (byte) 10 && num2 == (byte) 20)
            num1 = num2 = (byte) 0;
          activityDataSeries.Data.Add(new RawMinuteActivityData()
          {
            Mode = (int) num1,
            Activity = (int) num2,
            Steps = (int) num3,
            HeartRate = (int) num4
          });
        }
      }
      return activityDataSeries;
    }

    private List<HeartRateMeasurement> PostProcessHeartRateFragment(
      MiBandTwoDataSyncFragment fragment)
    {
      List<HeartRateMeasurement> heartRateMeasurementList = new List<HeartRateMeasurement>();
      for (int index1 = 0; index1 < fragment.RawDataLines.Count; ++index1)
      {
        byte[] rawDataLine = fragment.RawDataLines[index1];
        if ((int) rawDataLine[0] != index1 % 256)
          throw new InvalidDataException("Data in buffer is in wrong order.");
        for (int index2 = 1; index2 < rawDataLine.Length; index2 += 6)
        {
          int unixTimestamp = (int) rawDataLine[index2] + ((int) rawDataLine[index2 + 1] << 8) + ((int) rawDataLine[index2 + 2] << 16) + ((int) rawDataLine[index2 + 3] << 24);
          heartRateMeasurementList.Add(new HeartRateMeasurement()
          {
            HeartRateValue = (ushort) rawDataLine[index2 + 5],
            Timestamp = MiBand2Tools.UnixTimestampToDateTimeOffset((double) unixTimestamp, (sbyte) rawDataLine[index2 + 4])
          });
        }
      }
      return heartRateMeasurementList;
    }
  }
}
