// Decompiled with JetBrains decompiler
// Type: MiBandApp.ViewModels.Activities.ActivitiesListViewModel
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.exe

using Caliburn.Micro;
using MiBandApp.Storage.Tables;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

#nullable disable
namespace MiBandApp.ViewModels.Activities
{
  public class ActivitiesListViewModel : PropertyChangedBase
  {
    private readonly Func<List<IDbUserActivity>> _activitiesFunc;
    private readonly int _numberOfActivitiesToShowInShortMode;
    private readonly bool _orderByDescending;
    private readonly bool _alwaysShowLatest;
    private readonly ObservableCollection<IActivityViewModel> _activityItems = new ObservableCollection<IActivityViewModel>();
    private bool _showShortActivityItems;
    private bool _activityItemsCanBeShortened;

    public ActivitiesListViewModel(
      Func<List<IDbUserActivity>> activitiesFunc,
      int numberOfActivitiesToShowInShortMode = 2147483647,
      bool orderByDescending = false,
      bool alwaysShowLatest = false)
    {
      this._activitiesFunc = activitiesFunc;
      this._orderByDescending = orderByDescending;
      this._alwaysShowLatest = alwaysShowLatest;
      this._numberOfActivitiesToShowInShortMode = numberOfActivitiesToShowInShortMode;
      this.Refresh();
    }

    public ObservableCollection<IActivityViewModel> ActivityItems => this._activityItems;

    public bool ShowShortActivityItems
    {
      get => this._showShortActivityItems;
      set
      {
        this._showShortActivityItems = value;
        this.Refresh();
      }
    }

    public bool ActivityItemsCanBeShortened
    {
      get => this._activityItemsCanBeShortened;
      set
      {
        if (this._activityItemsCanBeShortened == value)
          return;
        this._activityItemsCanBeShortened = value;
        this.NotifyOfPropertyChange(nameof (ActivityItemsCanBeShortened));
      }
    }

    public bool ActivityItemsEmpty => this._activityItems.Count == 0;

    public override void Refresh()
    {
      List<IDbUserActivity> toShow = this._activitiesFunc().OrderByDescending<IDbUserActivity, DateTime>((Func<IDbUserActivity, DateTime>) (t => t.Begin)).ToList<IDbUserActivity>();
      int smallActivityThresholdMinutes = 0;
      if (toShow.Count > this._numberOfActivitiesToShowInShortMode)
        smallActivityThresholdMinutes = toShow.Select<IDbUserActivity, int>((Func<IDbUserActivity, int>) (t => t.GetDurationMin())).OrderByDescending<int, int>((Func<int, int>) (t => t)).ToList<int>()[this._numberOfActivitiesToShowInShortMode];
      if (smallActivityThresholdMinutes != 0 || toShow.Count == 0)
      {
        HashSet<IDbUserActivity> source = new HashSet<IDbUserActivity>(toShow.Where<IDbUserActivity>((Func<IDbUserActivity, bool>) (t => t.GetDurationMin() >= smallActivityThresholdMinutes)));
        if (this._alwaysShowLatest && toShow.Count != 0)
          source.Add(toShow[0]);
        this.ActivityItemsCanBeShortened = source.Count != toShow.Count;
        if (!this.ShowShortActivityItems)
          toShow = source.ToList<IDbUserActivity>();
      }
      List<IActivityViewModel> toRemove = this._activityItems.ToList<IActivityViewModel>();
      ((System.Action) (() =>
      {
        foreach (IDbUserActivity dbUserActivity in toShow)
        {
          IDbUserActivity activity = dbUserActivity;
          IActivityViewModel activityViewModel = this._activityItems.FirstOrDefault<IActivityViewModel>((Func<IActivityViewModel, bool>) (i => i.Activity.Id == activity.Id && i.Activity.GetType() == activity.GetType()));
          if (activityViewModel != null)
          {
            toRemove.Remove(activityViewModel);
          }
          else
          {
            int index = this._activityItems.Count - 1;
            while (index >= 0 && this._activityItems[index].Activity.Begin > activity.Begin != this._orderByDescending)
              --index;
            this._activityItems.Insert(index + 1, this.CreateActivityViewModel(activity, activity.GetDurationMin() < smallActivityThresholdMinutes));
          }
        }
        foreach (IActivityViewModel activityViewModel in toRemove)
          this._activityItems.Remove(activityViewModel);
        this.NotifyOfPropertyChange<bool>((Expression<Func<bool>>) (() => this.ActivityItemsEmpty));
      })).OnUIThreadAsync();
    }

    public IActivityViewModel CreateActivityViewModel(IDbUserActivity activity, bool showSmall)
    {
      switch (activity)
      {
        case DbWalkingActivity _:
          return (IActivityViewModel) new WalkingActivityViewModel((DbWalkingActivity) activity, showSmall);
        case DbSleepingActivity _:
          return (IActivityViewModel) new SleepingActivityViewModel((DbSleepingActivity) activity);
        case DbHeartRateMeasureActivity _:
          return (IActivityViewModel) new HeartRateMeasureActivityViewModel((DbHeartRateMeasureActivity) activity);
        case DbRunningActivity _:
          return (IActivityViewModel) new RunningActivityViewModel((DbRunningActivity) activity, showSmall);
        default:
          throw new NotImplementedException();
      }
    }
  }
}
