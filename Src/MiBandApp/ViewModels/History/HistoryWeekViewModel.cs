// Decompiled with JetBrains decompiler
// Type: MiBandApp.ViewModels.History.HistoryWeekViewModel
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.exe

using Caliburn.Micro;
using System;
using System.Collections.Generic;

#nullable disable
namespace MiBandApp.ViewModels.History
{
  public class HistoryWeekViewModel : PropertyChangedBase
  {
    private readonly DateTime _dayWeekStart;
    private readonly List<KeyValuePair<DateTime, int>> _stepsGoal;
    private readonly List<KeyValuePair<DateTime, int>> _sleepGoal;

    public HistoryWeekViewModel(
      DateTime dayWeekStart,
      List<DaySummaryViewModel> days,
      int stepsGoal,
      int sleepGoal)
    {
      this._dayWeekStart = dayWeekStart;
      this.Days = days;
      this._stepsGoal = new List<KeyValuePair<DateTime, int>>()
      {
        new KeyValuePair<DateTime, int>(this._dayWeekStart, stepsGoal),
        new KeyValuePair<DateTime, int>(this._dayWeekStart.AddDays(6.0), stepsGoal)
      };
      this._sleepGoal = new List<KeyValuePair<DateTime, int>>()
      {
        new KeyValuePair<DateTime, int>(this._dayWeekStart, sleepGoal),
        new KeyValuePair<DateTime, int>(this._dayWeekStart.AddDays(6.0), sleepGoal)
      };
    }

    public List<DaySummaryViewModel> Days { get; }

    public IEnumerable<KeyValuePair<DateTime, int>> StepsGoal
    {
      get => (IEnumerable<KeyValuePair<DateTime, int>>) this._stepsGoal;
    }

    public IEnumerable<KeyValuePair<DateTime, int>> SleepGoal
    {
      get => (IEnumerable<KeyValuePair<DateTime, int>>) this._sleepGoal;
    }

    public DateTime WeekStartDate => this._dayWeekStart;

    public DateTime WeekEndDate => this._dayWeekStart.AddDays(6.0);
  }
}
