// Decompiled with JetBrains decompiler
// Type: MiBandApp.ViewModels.Band.BandAlarmViewModel
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.exe

using Caliburn.Micro;
using MiBand.SDK.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Windows.ApplicationModel.Resources;

#nullable disable
namespace MiBandApp.ViewModels.Band
{
  public class BandAlarmViewModel : PropertyChangedBase
  {
    private readonly Alarm _originalAlarm;
    private readonly AlarmsPageViewModel _parentViewModel;
    private readonly ObservableCollection<bool> _days = new ObservableCollection<bool>();
    private readonly ResourceLoader _resourceLoader = new ResourceLoader();
    private bool _isUnfolded;
    private bool _isEnabled;

    public BandAlarmViewModel(Alarm originalAlarm, AlarmsPageViewModel parentViewModel)
    {
      this._originalAlarm = originalAlarm;
      this._parentViewModel = parentViewModel;
      int days = (int) originalAlarm.Days;
      for (int index = 0; index < 7; ++index)
        this._days.Add((days & 1 << index) != 0);
      this._isEnabled = originalAlarm.IsEnabled;
      this.IsSmart = originalAlarm.IsSmart && this._parentViewModel.IsSmartSupported;
      this.Time = originalAlarm.Time;
      this.Repeat = this.Days.Any<bool>((Func<bool, bool>) (t => t));
      this.Days.CollectionChanged += (NotifyCollectionChangedEventHandler) ((sender, args) => this.NotifyOfPropertyChange(nameof (Days)));
    }

    public ObservableCollection<bool> Days => this._days;

    public TimeSpan Time { get; set; }

    public bool Repeat { get; set; }

    public string Description
    {
      get
      {
        return !this.Repeat ? this._resourceLoader.GetString("Alarm_Description_Once") : this.GetDaysString((IReadOnlyList<bool>) this._days);
      }
    }

    public bool IsUnfolded
    {
      get => this._isUnfolded;
      set
      {
        this._isUnfolded = value;
        if (!this._isUnfolded && !this.Days.Any<bool>((Func<bool, bool>) (t => t)))
        {
          this.Repeat = false;
          this.NotifyOfPropertyChange("Repeat");
        }
        this.NotifyOfPropertyChange(nameof (IsUnfolded));
        this.NotifyOfPropertyChange("Description");
      }
    }

    public bool HasChanged => this.GetAlarm() != this._originalAlarm;

    public bool IsSmartSupported => this._parentViewModel.IsSmartSupported;

    public Alarm GetAlarm()
    {
      int num = 0;
      if (this.Repeat)
      {
        for (int index = 0; index < 7; ++index)
        {
          if (this.Days[index])
            num |= 1 << index;
        }
      }
      return new Alarm()
      {
        Days = (DaysOfWeek) num,
        Time = this.Time,
        IsEnabled = this.IsEnabled,
        IsSmart = this.IsSmart
      };
    }

    public bool IsEnabled
    {
      get => this._isEnabled;
      set
      {
        if (value == this._isEnabled)
          return;
        this._isEnabled = value;
        this.NotifyOfPropertyChange(nameof (IsEnabled));
      }
    }

    public bool IsSmart { get; set; }

    public void RemoveAlarm() => this._parentViewModel.RemoveAlarm(this);

    public void DisableExpired()
    {
      this.IsEnabled = false;
      this._originalAlarm.IsEnabled = false;
    }

    private string GetDaysString(IReadOnlyList<bool> days)
    {
      if (days.All<bool>((Func<bool, bool>) (t => t)))
        return this._resourceLoader.GetString("Days_EveryDay");
      if (days.Take<bool>(5).All<bool>((Func<bool, bool>) (t => t)) && !days.Skip<bool>(5).Any<bool>((Func<bool, bool>) (t => t)))
        return this._resourceLoader.GetString("Days_Weekdays");
      if (!days.Take<bool>(5).Any<bool>((Func<bool, bool>) (t => t)) && days.Skip<bool>(5).All<bool>((Func<bool, bool>) (t => t)))
        return this._resourceLoader.GetString("Days_Weekends");
      List<int> source = new List<int>(6);
      for (int index = 0; index < days.Count; ++index)
      {
        if (days[index])
          source.Add(index);
      }
      if (source.Count == 0)
        return string.Empty;
      return source.Count == 1 ? this.GetDayString(source[0], "dddd") : string.Join(" ", source.Select<int, string>((Func<int, string>) (t => this.GetDayString(t))));
    }

    private string GetDayString(int dayNum, string format = "ddd")
    {
      return new DateTime(2015, 8, 24).AddDays((double) dayNum).ToString(format);
    }
  }
}
