
// Type: MiBandApp.ViewModels.HistoryPageViewModel
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// 

using Caliburn.Micro;
using MiBand.SDK.Configuration;
using MiBandApp.Storage.DataBases;
using MiBandApp.Storage.Tables;
using MiBandApp.Tools;
using MiBandApp.ViewModels.History;
using MiBandApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml.Navigation;

#nullable disable
namespace MiBandApp.ViewModels
{
  public class HistoryPageViewModel : PageBaseViewModel, IViewAware
  {
    private readonly DaySummaryDataBase _daySummaryDataBase;
    private readonly MiBandApp.Storage.Settings.Settings _settings;
    private readonly INavigationService _navigationService;
    private readonly List<HistoryWeekViewModel> _weeks = new List<HistoryWeekViewModel>();
    private HistoryPage _view;
    private bool _realGoBack;
    private int _weekIndex;
    private int _maxWeekIndex;
    private int _viewModeIndex;
    private bool _isDisplayingSleep;
    private DateTime _currentWeekStartDate;

    public HistoryPageViewModel(
      MiBandApp.Storage.Settings.Settings settings,
      INavigationService navigationService,
      DaySummaryDataBase daySummaryDataBase)
    {
      this._settings = settings;
      this._navigationService = navigationService;
      this._daySummaryDataBase = daySummaryDataBase;
    }

    public HistoryWeekViewModel Week => this.GetWeek(this._weekIndex);

    public int ViewModeIndex
    {
      get => this._viewModeIndex;
      set
      {
        if (this._viewModeIndex == value)
          return;
        this._viewModeIndex = value;
        this.NotifyOfPropertyChange(nameof (ViewModeIndex));
        this.NotifyOfPropertyChange<bool>((Expression<Func<bool>>) (() => this.IsInListView));
      }
    }

    public bool IsInListView
    {
      get => this.ViewModeIndex == 1;
      set => this.ViewModeIndex = value ? 1 : 0;
    }

    public bool CanGoLeft
    {
      get
      {
        if (this._weekIndex < this._maxWeekIndex)
          return true;
        DateTime visibleWeekStart = this.GetWeekStartDayByIndex(this._weekIndex);
        return this._daySummaryDataBase.GetCount((Expression<Func<DaySummary, bool>>) (t => t.Date < visibleWeekStart)) > 0;
      }
    }

    public bool CanGoRight => this._weekIndex != 0;

    public bool IsDisplayingSleep
    {
      get => this._isDisplayingSleep;
      set
      {
        if (this._isDisplayingSleep == value)
          return;
        this._isDisplayingSleep = value;
        this.NotifyOfPropertyChange(nameof (IsDisplayingSleep));
        this._view.ChangeViewMode(this._isDisplayingSleep);
      }
    }

    public async void GoLeft()
    {
      ++this._weekIndex;
      if (this._maxWeekIndex < this._weekIndex)
        this._maxWeekIndex = this._weekIndex;
      this.NotifyOfPropertyChange<bool>((Expression<Func<bool>>) (() => this.CanGoRight));
      this.NotifyOfPropertyChange<bool>((Expression<Func<bool>>) (() => this.CanGoLeft));
      HistoryWeekViewModel historyWeekViewModel = await Task.Run<HistoryWeekViewModel>((Func<HistoryWeekViewModel>) (() => this.GetWeek(this._weekIndex))).ConfigureAwait(true);
      this.NotifyOfPropertyChange<HistoryWeekViewModel>((Expression<Func<HistoryWeekViewModel>>) (() => this.Week));
      this._view.SetItemSource(this.Week);
    }

    public async void GoRight()
    {
      --this._weekIndex;
      if (this._weekIndex < 0)
        this._weekIndex = 0;
      this.NotifyOfPropertyChange<bool>((Expression<Func<bool>>) (() => this.CanGoRight));
      this.NotifyOfPropertyChange<bool>((Expression<Func<bool>>) (() => this.CanGoLeft));
      this.NotifyOfPropertyChange<HistoryWeekViewModel>((Expression<Func<HistoryWeekViewModel>>) (() => this.Week));
      this._view.SetItemSource(this.Week);
    }

    public void AttachView(object view, object context = null)
    {
      this._view = (HistoryPage) view;
      this._view.SetItemSource(this.Week);
    }

    public object GetView(object context = null) => throw new NotImplementedException();

    public event EventHandler<ViewAttachedEventArgs> ViewAttached;

    public void GoToDay(DaySummaryViewModel day)
    {
      if (day == null)
        return;
      this._navigationService.NavigateToViewModel<DayDetailsPageViewModel>((object) day.DaySummary);
    }

    protected override async Task OnActivate()
    {
      //this._navigationService.BackPressed += new EventHandler<EventArgs>(this.NavigationServiceOnBackPressed);
    }

    protected override async Task OnDeactivate(bool close = true)
    {
      //this._navigationService.BackPressed -= new EventHandler<EventArgs>(this.NavigationServiceOnBackPressed);
      if (!this._realGoBack)
        return;
      this._view.NavigationCacheMode = NavigationCacheMode.Disabled;
    }

    private void NavigationServiceOnBackPressed(
      object sender,
      EventArgs backPressedEventArgs)
    {
      this._realGoBack = true;
    }

    private HistoryWeekViewModel GetWeek(int weekIndex)
    {
      while (this._weeks.Count <= weekIndex)
      {
        if (!this.TryAddWeek(this.GetWeekStartDayByIndex(this._weeks.Count)))
          return (HistoryWeekViewModel) null;
      }
      return this._weeks[weekIndex];
    }

    private DateTime GetFirstWeekDayContainingDay(DateTime day)
    {
      while (day.DayOfWeek != DayOfWeek.Monday)
        day = day.Subtract(TimeSpan.FromDays(1.0));
      return day;
    }

    private DateTime GetWeekStartDayByIndex(int index)
    {
      if (this._currentWeekStartDate == new DateTime())
        this._currentWeekStartDate = this.GetFirstWeekDayContainingDay(DateTime.Now.Date);
      return this._currentWeekStartDate.Subtract(TimeSpan.FromDays((double) (7 * index)));
    }

    private bool TryAddWeek(DateTime weekStartDate)
    {
      GoalInfo savedGoalInfo = this._settings.GetSavedGoalInfo();
      Dictionary<DateTime, DaySummaryViewModel> dictionary = 
                this._daySummaryDataBase.GetDays(weekStartDate, weekStartDate.AddDays(6.0))
                .Select<DaySummary, DaySummaryViewModel>((Func<DaySummary, DaySummaryViewModel>)
                (t => new DaySummaryViewModel(t))).ToDictionary<DaySummaryViewModel, DateTime>(
                    (Func<DaySummaryViewModel, DateTime>) (t => t.DaySummary.Date));

      List<DaySummaryViewModel> days = new List<DaySummaryViewModel>(7);
      DateTime dateTime = weekStartDate;
      int num = 0;
      while (num < 7)
      {
        if (dictionary.ContainsKey(dateTime))
          days.Add(dictionary[dateTime]);
        else
          days.Add(new DaySummaryViewModel(new DaySummary(dateTime)));
        ++num;
        dateTime = dateTime.AddDays(1.0);
      }
      this._weeks.Add(new HistoryWeekViewModel(weekStartDate, days, savedGoalInfo.StepsGoal, 
          savedGoalInfo.SleepGoalMinutes));
      return true;
    }
  }
}
