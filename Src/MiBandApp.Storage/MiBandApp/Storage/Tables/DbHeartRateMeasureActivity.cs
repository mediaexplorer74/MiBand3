// Decompiled with JetBrains decompiler
// Type: MiBandApp.Storage.Tables.DbHeartRateMeasureActivity
// Assembly: MiBandApp.Storage, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 12BDBED5-65C6-40FC-A182-B057969144FE
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.Storage.dll

using MiBandApp.Data.Activities;
using MiBandApp.Storage.Utils;
using SQLite;

#nullable disable
namespace MiBandApp.Storage.Tables
{
  [Table("HeartRateMeasureActivity")]
  public class DbHeartRateMeasureActivity : HeartRateMeasureActivity, IDbUserActivity, IUserActivity
  {
    [PrimaryKey]
    [AutoIncrement]
    public int Id { get; set; }

    public DbHeartRateMeasureActivity()
    {
    }

    public DbHeartRateMeasureActivity(HeartRateMeasureActivity heartRateMeasureActivity)
    {
      this.CopyFromBase<HeartRateMeasureActivity, DbHeartRateMeasureActivity>(heartRateMeasureActivity);
    }
  }
}
