
// Type: MiBandApp.ViewModels.Tabs.SleepTabViewModel
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// 

using Caliburn.Micro;
using MiBand.SDK.Configuration;
using MiBandApp.Data.Activities;
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
  public class SleepTabViewModel : PropertyChangedBase
  {
    private readonly LicensingService _licensingService;
    private readonly INavigationService _navigationService;
    private readonly MiBandApp.Storage.Settings.Settings _settings;
    private readonly BandSyncController _bandSyncController;
    private readonly ActivitiesDataBase _activitiesDataBase;

    public SleepTabViewModel(
      LicensingService licensingService,
      INavigationService navigationService,
      MiBandApp.Storage.Settings.Settings settings,
      BandSyncController bandSyncController,
      ActivitiesDataBase activitiesDataBase)
    {
      this._licensingService = licensingService;
      this._navigationService = navigationService;
      this._settings = settings;
      this._bandSyncController = bandSyncController;
      this._activitiesDataBase = activitiesDataBase;
      this._bandSyncController.SyncState.Updated += new EventHandler<MonitorableUpdatedEventArgs<BandSyncState>>(this.SyncStateOnUpdated);
    }

    public bool IsPro => this._licensingService.IsPro;

    public GoalInfo GoalInfo => this._settings.GetSavedGoalInfo();

    public string SyncState => this._bandSyncController.SyncState.Value.ToString();

    public int TotalSleepTimeMin { get; set; }

    public int TotalDeepSleepMin { get; set; }

    public int TotalTimeInBed { get; set; }

    public void GoToSubscriptionPage()
    {
      this._navigationService.UriFor<SettingsPageViewModel>().WithParam<bool>((Expression<Func<SettingsPageViewModel, bool>>) (t => t.ShowSubscription), true).Navigate();
    }

    public override void Refresh()
    {
      DateTime today = DateTime.Now.Date;
      List<SleepingActivity> list = this._activitiesDataBase.GetActivities((Expression<Func<IDbUserActivity, bool>>) (t => t.End.Date == today)).OfType<SleepingActivity>().ToList<SleepingActivity>();
      this.TotalTimeInBed = list.Sum<SleepingActivity>((Func<SleepingActivity, int>) (t => t.GetDurationMin()));
      this.TotalDeepSleepMin = list.Sum<SleepingActivity>((Func<SleepingActivity, int>) (t => t.DeepSleepMin));
      this.TotalSleepTimeMin = list.Sum<SleepingActivity>((Func<SleepingActivity, int>) (t => t.TotalSleepMinutes));
      base.Refresh();
    }

    private void SyncStateOnUpdated(
      object sender,
      MonitorableUpdatedEventArgs<BandSyncState> updatedEventArgs)
    {
      this.NotifyOfPropertyChange<string>((Expression<Func<string>>) (() => this.SyncState));
      if (updatedEventArgs.UpdatedValue != BandSyncState.Success)
        return;
      this.Refresh();
    }
  }
}
