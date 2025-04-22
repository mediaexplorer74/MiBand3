// Decompiled with JetBrains decompiler
// Type: MiBandApp.ViewModels.History.DaySummaryViewModel
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.exe

using Caliburn.Micro;
using MiBandApp.Storage.Tables;
using System;

#nullable disable
namespace MiBandApp.ViewModels.History
{
  public class DaySummaryViewModel : PropertyChangedBase
  {
    public DaySummaryViewModel(DaySummary daySummary) => this.DaySummary = daySummary;

    public DaySummary DaySummary { get; }

    public DateTime Date => this.DaySummary.Date;

    public int SleepMinutes => this.DaySummary.SleepMinutes - this.DaySummary.DeepSleepMinutes;

    public int DeepSleepMinutes => this.DaySummary.DeepSleepMinutes;
  }
}
