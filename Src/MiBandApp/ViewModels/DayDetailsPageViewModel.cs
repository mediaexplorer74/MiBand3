
// Type: MiBandApp.ViewModels.DayDetailsPageViewModel
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// 

using MiBandApp.Storage.DataBases;
using MiBandApp.Storage.Tables;
using MiBandApp.Tools;
using MiBandApp.ViewModels.Activities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

#nullable disable
namespace MiBandApp.ViewModels
{
  public class DayDetailsPageViewModel : PageBaseViewModel
  {
    private readonly ActivitiesDataBase _activitiesDataBase;
    private readonly ActivitiesListViewModel _activitiesListViewModel;
    private DateTime _date;
    private int _steps;
    private int _stepsGoal;
    private int _sleepMinutes;
    private int _sleepGoal;
    private int _calories;
    private double _distance;
    private int _deepSleepMinutes;

    public DayDetailsPageViewModel(ActivitiesDataBase activitiesDataBase)
    {
      this._activitiesDataBase = activitiesDataBase;
      this._activitiesListViewModel = new ActivitiesListViewModel((Func<List<IDbUserActivity>>) (() => this._activitiesDataBase.GetActivitiesInDay(this._date)), 7);
    }

    public DaySummary Parameter { get; set; }

    public DateTime Date
    {
      get => this._date;
      set
      {
        if (this._date == value)
          return;
        this._date = value;
        this.NotifyOfPropertyChange(nameof (Date));
      }
    }

    public int Steps
    {
      get => this._steps;
      set
      {
        if (this._steps == value)
          return;
        this._steps = value;
        this.NotifyOfPropertyChange(nameof (Steps));
      }
    }

    public int StepsGoal
    {
      get => this._stepsGoal;
      set
      {
        if (this._stepsGoal == value)
          return;
        this._stepsGoal = value;
        this.NotifyOfPropertyChange(nameof (StepsGoal));
      }
    }

    public int SleepMinutes
    {
      get => this._sleepMinutes;
      set
      {
        if (this._sleepMinutes == value)
          return;
        this._sleepMinutes = value;
        this.NotifyOfPropertyChange(nameof (SleepMinutes));
      }
    }

    public int DeepSleepMinutes
    {
      get => this._deepSleepMinutes;
      set
      {
        if (value == this._deepSleepMinutes)
          return;
        this._deepSleepMinutes = value;
        this.NotifyOfPropertyChange(nameof (DeepSleepMinutes));
      }
    }

    public int SleepGoal
    {
      get => this._sleepGoal;
      set
      {
        if (this._sleepGoal == value)
          return;
        this._sleepGoal = value;
        this.NotifyOfPropertyChange(nameof (SleepGoal));
      }
    }

    public int Calories
    {
      get => this._calories;
      private set
      {
        if (value == this._calories)
          return;
        this._calories = value;
        this.NotifyOfPropertyChange(nameof (Calories));
      }
    }

    public double Distance
    {
      get => this._distance;
      private set
      {
        if (value == this._distance)
          return;
        this._distance = value;
        this.NotifyOfPropertyChange(nameof (Distance));
      }
    }

    public ActivitiesListViewModel ActivitiesListViewModel => this._activitiesListViewModel;

    protected override async Task OnActivate()
    {
      this.Date = this.Parameter.Date;
      DaySummary parameter = this.Parameter;
      this.Steps = parameter.Steps;
      this.StepsGoal = parameter.StepsGoal;
      this.SleepGoal = parameter.SleepGoalMinutes;
      this.SleepMinutes = parameter.SleepMinutes;
      this.DeepSleepMinutes = parameter.DeepSleepMinutes;
      this.Calories = parameter.Calories;
      this.Distance = parameter.Distance;
      this._activitiesListViewModel.Refresh();
    }
  }
}
