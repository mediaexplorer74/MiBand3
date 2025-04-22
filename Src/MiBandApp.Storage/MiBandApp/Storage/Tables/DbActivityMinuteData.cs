// Decompiled with JetBrains decompiler
// Type: MiBandApp.Storage.Tables.DbActivityMinuteData
// Assembly: MiBandApp.Storage, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 12BDBED5-65C6-40FC-A182-B057969144FE
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.Storage.dll

using MiBandApp.Data.Activities;
using MiBandApp.Storage.Utils;
using SQLite;
using System;

#nullable disable
namespace MiBandApp.Storage.Tables
{
  [Table("ActivityMinuteData")]
  public class DbActivityMinuteData : ActivityMinuteData
  {
    public const string TableName = "ActivityMinuteData";

    [Indexed(Unique = true, Name = "Timestamp")]
    public override DateTime Timestamp { get; set; }

    [PrimaryKey]
    [AutoIncrement]
    public int Id { get; set; }

    public DbActivityMinuteData()
    {
    }

    public DbActivityMinuteData(ActivityMinuteData activityMinuteData)
    {
      this.CopyFromBase<ActivityMinuteData, DbActivityMinuteData>(activityMinuteData);
    }
  }
}
