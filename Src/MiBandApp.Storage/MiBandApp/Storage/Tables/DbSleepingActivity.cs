// Decompiled with JetBrains decompiler
// Type: MiBandApp.Storage.Tables.DbSleepingActivity
// Assembly: MiBandApp.Storage, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 12BDBED5-65C6-40FC-A182-B057969144FE
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.Storage.dll

using MiBandApp.Data.Activities;
using MiBandApp.Storage.Utils;
using SQLite;

#nullable disable
namespace MiBandApp.Storage.Tables
{
  [Table("SleepingActivity")]
  public class DbSleepingActivity : SleepingActivity, IDbUserActivity, IUserActivity
  {
    public DbSleepingActivity()
    {
    }

    public DbSleepingActivity(SleepingActivity sleepingActivity)
    {
      this.CopyFromBase<SleepingActivity, DbSleepingActivity>(sleepingActivity);
    }

    [PrimaryKey]
    [AutoIncrement]
    public int Id { get; set; }

    [Ignore]
    public override int TotalSleepMinutes => base.TotalSleepMinutes;
  }
}
