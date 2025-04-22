// Decompiled with JetBrains decompiler
// Type: MiBandApp.ViewModels.Tabs.WalkTabViewModel
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.exe

using MiBand.SDK.Configuration;
using MiBand.SDK.Data;
using MiBandApp.Data.Activities;
using MiBandApp.Services;
using MiBandApp.Storage.DataBases;
using MiBandApp.Storage.Tables;
using MiBandApp.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Windows.ApplicationModel.Resources;

#nullable disable
namespace MiBandApp.ViewModels.Tabs
{
  public class WalkTabViewModel : PropertyChangedEx
  {
    private readonly BandSyncController _bandSyncController;
    private readonly BandController _bandController;
    private readonly MiBandApp.Storage.Settings.Settings _settings;
    private readonly ActivitiesDataBase _activitiesDataBase;
    private readonly ResourceLoader _resourceLoader = new ResourceLoader();
    private bool _isRealtimeStepsLoaded;
    private int _realtimeSteps;

    public WalkTabViewModel(
      BandController bandController,
      BandSyncController bandSyncController,
      MiBandApp.Storage.Settings.Settings settings,
      ActivitiesDataBase activitiesDataBase)
    {
      this._bandController = bandController;
      this._bandSyncController = bandSyncController;
      this._settings = settings;
      this._activitiesDataBase = activitiesDataBase;
      this.Activate();
    }

    public GoalInfo GoalInfo => this._settings.GetSavedGoalInfo();

    public bool IsRealtimeStepsLoaded
    {
      get => this._isRealtimeStepsLoaded;
      set
      {
        if (this._isRealtimeStepsLoaded == value)
          return;
        this._isRealtimeStepsLoaded = value;
        this.NotifyOfPropertyChange(nameof (IsRealtimeStepsLoaded));
      }
    }

    public bool IsRefreshingSteps
    {
      get
      {
        return this._bandController.CommunicationOperation.Value == CommunicationOperation.UpdatingSteps || this.RealtimeSteps == -1;
      }
    }

    public int RealtimeSteps
    {
      get => this._realtimeSteps;
      set
      {
        if (this._realtimeSteps == value)
          return;
        this._realtimeSteps = value;
        this.NotifyOfPropertyChange(nameof (RealtimeSteps));
        this.IsRealtimeStepsLoaded = value != -1;
      }
    }

    public int TotalWalkingTime { get; set; }

    public string DailyDistance { get; set; }

    public string DailyEnergy { get; set; }

    public string DistanceUnits
    {
      get
      {
        if (this._settings.DistanceUnits == MiBandApp.Storage.Settings.Settings.DistanceUnit.Km)
          return this._resourceLoader.GetString("MainPageKilometers");
        if (this._settings.DistanceUnits == MiBandApp.Storage.Settings.Settings.DistanceUnit.Mi)
          return this._resourceLoader.GetString("MainPageMiles");
        throw new NotImplementedException();
      }
    }

    public string EnergyUnits => this._resourceLoader.GetString("Units_Kcal");

    public override void Refresh()
    {
      DateTime today = DateTime.Now.Date;
      List<IStepsActivity> list = this._activitiesDataBase.GetActivities((Expression<Func<IDbUserActivity, bool>>) (t => t.End.Date == today)).OfType<IStepsActivity>().ToList<IStepsActivity>();
      this.DailyEnergy = this.GetKCaloriesString((IEnumerable<IStepsActivity>) list);
      this.DailyDistance = this.GetDistanceString((IEnumerable<IStepsActivity>) list);
      this.TotalWalkingTime = list.Sum<IStepsActivity>((Func<IStepsActivity, int>) (t => t.GetDurationMin()));
      base.Refresh();
    }

    private void Activate()
    {
      this._bandController.CommunicationOperation.Updated += new EventHandler<MonitorableUpdatedEventArgs<CommunicationOperation>>(this.CommunicationOperationOnUpdated);
      this._bandSyncController.RealtimeSteps.Updated += new EventHandler<MonitorableUpdatedEventArgs<RealtimeStepsData>>(this.RealtimeStepsOnUpdated);
      this._bandSyncController.SyncState.Updated += new EventHandler<MonitorableUpdatedEventArgs<BandSyncState>>(this.SyncStateOnUpdated);
      RealtimeStepsData realtimeStepsData = this._bandSyncController.RealtimeSteps.Value;
      this.RealtimeSteps = realtimeStepsData != null ? realtimeStepsData.TotalSteps : -1;
      this.Refresh();
    }

    private void SyncStateOnUpdated(
      object sender,
      MonitorableUpdatedEventArgs<BandSyncState> updatedEventArgs)
    {
      if (updatedEventArgs.UpdatedValue != BandSyncState.Success)
        return;
      this.Refresh();
    }

    private void CommunicationOperationOnUpdated(
      object sender,
      MonitorableUpdatedEventArgs<CommunicationOperation> updatedEventArgs)
    {
      this.NotifyOfPropertyChangeAsync<bool>((Expression<Func<bool>>) (() => this.IsRefreshingSteps));
    }

    private void RealtimeStepsOnUpdated(
      object sender,
      MonitorableUpdatedEventArgs<RealtimeStepsData> updatedEventArgs)
    {
      this.RealtimeSteps = updatedEventArgs.UpdatedValue.TotalSteps;
    }

    private string GetDistanceString(IEnumerable<IStepsActivity> activities)
    {
      double num = activities.Sum<IStepsActivity>((Func<IStepsActivity, double>) (t => t.Distance));
      if (this._settings.DistanceUnits == MiBandApp.Storage.Settings.Settings.DistanceUnit.Km)
        return num.ToString("F2");
      if (this._settings.DistanceUnits == MiBandApp.Storage.Settings.Settings.DistanceUnit.Mi)
        return (num * 0.621).ToString("F2");
      throw new NotImplementedException();
    }

    private string GetKCaloriesString(IEnumerable<IStepsActivity> activities)
    {
      return (activities.Sum<IStepsActivity>((Func<IStepsActivity, int>) (t => t.Calories)) / 1000).ToString("F0");
    }
  }
}
