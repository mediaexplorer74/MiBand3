// Decompiled with JetBrains decompiler
// Type: MiBandApp.Storage.Tables.DaySummary
// Assembly: MiBandApp.Storage, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 12BDBED5-65C6-40FC-A182-B057969144FE
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.Storage.dll

using Newtonsoft.Json;
using SQLite;
using System;

#nullable disable
namespace MiBandApp.Storage.Tables
{
  public class DaySummary
  {
    public DaySummary()
    {
    }

    public DaySummary(DateTime date)
      : this()
    {
      this.Date = date;
    }

    [PrimaryKey]
    [AutoIncrement]
    [JsonIgnore]
    public int Id { get; set; }

    [Indexed(Unique = true, Name = "Date")]
    public DateTime Date { get; set; }

    public int Steps { get; set; }

    public int Calories { get; set; }

    public double Distance { get; set; }

    public int StepsGoal { get; set; }

    public int SleepMinutes { get; set; }

    public int DeepSleepMinutes { get; set; }

    public int SleepGoalMinutes { get; set; }

    public bool WasRunning { get; set; }

    public int DataBeginMinute { get; set; }

    public int DataEndMinute { get; set; }
  }
}
