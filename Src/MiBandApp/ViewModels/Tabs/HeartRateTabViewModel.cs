
// Type: MiBandApp.ViewModels.Tabs.HeartRateTabViewModel
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// 

using Caliburn.Micro;
using MetroLog;
using MiBand.SDK.Data;
using MiBandApp.Services;
using MiBandApp.Storage.DataBases;
using MiBandApp.Storage.Tables;
using MiBandApp.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

#nullable disable
namespace MiBandApp.ViewModels.Tabs
{
  public class HeartRateTabViewModel : PropertyChangedBase
  {
    private readonly BandSyncController _bandSyncController;
    private readonly RawActivityDataBase _rawActivityDataBase;
    private readonly ActivitiesDataBase _activitiesDataBase;
    private readonly ILogger _log;
    private int _heartRate;
    private bool _isMeasuring;
    private DateTimeOffset _lastUpdateTime;

    public HeartRateTabViewModel(
      BandSyncController bandSyncController,
      ILogManager logManager,
      RawActivityDataBase rawActivityDataBase,
      ActivitiesDataBase activitiesDataBase)
    {
      this._bandSyncController = bandSyncController;
      this._log = logManager.GetLogger<HeartRateTabViewModel>();
      this._rawActivityDataBase = rawActivityDataBase;
      this._activitiesDataBase = activitiesDataBase;
      this._bandSyncController.LatestHeartRate.Updated += (EventHandler<MonitorableUpdatedEventArgs<HeartRateMeasurement>>) ((sender, args) =>
      {
        this.HeartRate = (int) args.UpdatedValue.HeartRateValue;
        this.LastUpdatedTimestamp = args.UpdatedValue.Timestamp;
      });
      this._bandSyncController.SyncState.Updated += new EventHandler<MonitorableUpdatedEventArgs<BandSyncState>>(this.SyncStateOnUpdated);
      this.LoadLatestHeartRateFromDB();
    }

    public int HeartRate
    {
      get => this._heartRate;
      set
      {
        if (this._heartRate == value)
          return;
        this._heartRate = value;
        this.NotifyOfPropertyChange(nameof (HeartRate));
      }
    }

    public DateTimeOffset LastUpdatedTimestamp
    {
      get => this._lastUpdateTime;
      set
      {
        if (this._lastUpdateTime == value)
          return;
        this._lastUpdateTime = value;
        this.NotifyOfPropertyChange<DateTimeOffset>((Expression<Func<DateTimeOffset>>) (() => this.LastUpdatedTimestamp));
        this.NotifyOfPropertyChange<bool>((Expression<Func<bool>>) (() => this.HeartRateIsValid));
        this.NotifyOfPropertyChange<int>((Expression<Func<int>>) (() => this.GaugeValue));
      }
    }

    public bool IsMeasuring
    {
      get => this._isMeasuring;
      set
      {
        if (this._isMeasuring == value)
          return;
        this._isMeasuring = value;
        this.NotifyOfPropertyChange(nameof (IsMeasuring));
      }
    }

    public bool HeartRateIsValid
    {
      get => ((DateTimeOffset) DateTime.Now - this.LastUpdatedTimestamp).TotalDays < 1.0;
    }

    public int GaugeValue => !this.HeartRateIsValid ? 0 : 100;

    public async void MeasureHeartRate()
    {
      if (this.IsMeasuring)
        return;
      try
      {
        this.IsMeasuring = true;
        await this._bandSyncController.MeasureHeartRate().ConfigureAwait(true);
      }
      catch (Exception ex)
      {
        this._log.Error(ex.ToString(), (Exception) null);
      }
      finally
      {
        this.IsMeasuring = false;
      }
    }

    private void LoadLatestHeartRateFromDB()
    {
      List<HeartRateMeasurement> source = new List<HeartRateMeasurement>();
      DbActivityMinuteData activityMinuteData = this._rawActivityDataBase.GetInInterval(DateTime.Now.AddDays(-1.0), DateTime.Now).Where<DbActivityMinuteData>((Func<DbActivityMinuteData, bool>) (t => t.HeartRate != 0)).OrderByDescending<DbActivityMinuteData, DateTime>((Func<DbActivityMinuteData, DateTime>) (t => t.Timestamp)).FirstOrDefault<DbActivityMinuteData>();
      if (activityMinuteData != null)
        source.Add(new HeartRateMeasurement()
        {
          HeartRateValue = (ushort) activityMinuteData.HeartRate,
          Timestamp = (DateTimeOffset) activityMinuteData.Timestamp
        });
      DbHeartRateMeasureActivity rateMeasureActivity = this._activitiesDataBase.GetActivities((Expression<Func<IDbUserActivity, bool>>) (t => t.End >= DateTime.Now.AddDays(-1.0))).OfType<DbHeartRateMeasureActivity>().OrderByDescending<DbHeartRateMeasureActivity, DateTime>((Func<DbHeartRateMeasureActivity, DateTime>) (t => t.End)).FirstOrDefault<DbHeartRateMeasureActivity>();
      if (rateMeasureActivity != null)
        source.Add(new HeartRateMeasurement()
        {
          HeartRateValue = (ushort) rateMeasureActivity.HeartRate,
          Timestamp = (DateTimeOffset) rateMeasureActivity.End
        });
      if (source.Count == 0)
        return;
      HeartRateMeasurement heartRateMeasurement = source.OrderByDescending<HeartRateMeasurement, DateTimeOffset>((Func<HeartRateMeasurement, DateTimeOffset>) (t => t.Timestamp)).FirstOrDefault<HeartRateMeasurement>();
      if (!(heartRateMeasurement.Timestamp > this.LastUpdatedTimestamp))
        return;
      this.HeartRate = (int) heartRateMeasurement.HeartRateValue;
      this.LastUpdatedTimestamp = heartRateMeasurement.Timestamp;
    }

    private void SyncStateOnUpdated(
      object sender,
      MonitorableUpdatedEventArgs<BandSyncState> updatedEventArgs)
    {
      if (updatedEventArgs.UpdatedValue != BandSyncState.Success)
        return;
      this.LoadLatestHeartRateFromDB();
    }
  }
}
