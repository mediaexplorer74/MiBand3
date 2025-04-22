// Decompiled with JetBrains decompiler
// Type: MiBandApp.Storage.DataBases.RawActivityDataBase
// Assembly: MiBandApp.Storage, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 12BDBED5-65C6-40FC-A182-B057969144FE
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.Storage.dll

using MiBandApp.Storage.Tables;
using SQLite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Storage;

#nullable disable
namespace MiBandApp.Storage.DataBases
{
  public class RawActivityDataBase
  {
    private const string DatabaseFileName = "RawActivity.sqlite";
    private readonly string _dataBaseFilePath = Path.Combine(Path.Combine(ApplicationData.Current.LocalFolder.Path, "RawActivity.sqlite"));

    public void Init()
    {
      try
      {
        using (SQLiteConnection sqLiteConnection = new SQLiteConnection(this._dataBaseFilePath, true))
          sqLiteConnection.CreateTable<DbActivityMinuteData>();
      }
      catch
      {
      }
    }

    public void AddOrReplace(IEnumerable<DbActivityMinuteData> activityMinutes)
    {
      List<DbActivityMinuteData> list = activityMinutes.ToList<DbActivityMinuteData>();
      DateTime begin = list.Select<DbActivityMinuteData, DateTime>((Func<DbActivityMinuteData, DateTime>) (t => t.Timestamp)).Min<DateTime>();
      DateTime end = list.Select<DbActivityMinuteData, DateTime>((Func<DbActivityMinuteData, DateTime>) (t => t.Timestamp)).Max<DateTime>();
      using (SQLiteConnection connection = new SQLiteConnection(this._dataBaseFilePath, true))
      {
        connection.BeginTransaction();
        try
        {
          this.DeleteInInterval(connection, begin, end);
          connection.InsertAll((IEnumerable) list);
          connection.Commit();
        }
        catch
        {
          connection.Rollback();
          throw;
        }
      }
    }

    public List<DbActivityMinuteData> GetInInterval(DateTime begin, DateTime end)
    {
      using (SQLiteConnection conn = new SQLiteConnection(this._dataBaseFilePath, true))
      {
        SQLiteCommand sqLiteCommand = new SQLiteCommand(conn);
        sqLiteCommand.CommandText = "SELECT * FROM ActivityMinuteData WHERE Timestamp >= @beginTime AND Timestamp <= @endTime";
        sqLiteCommand.Bind("@beginTime", (object) begin);
        sqLiteCommand.Bind("@endTime", (object) end);
        return sqLiteCommand.ExecuteQuery<DbActivityMinuteData>().ToList<DbActivityMinuteData>();
      }
    }

    public List<DbActivityMinuteData> GetInDay(DateTime day)
    {
      using (SQLiteConnection conn = new SQLiteConnection(this._dataBaseFilePath, true))
      {
        SQLiteCommand sqLiteCommand = new SQLiteCommand(conn);
        sqLiteCommand.CommandText = "SELECT * FROM ActivityMinuteData WHERE Timestamp >= @beginTime AND Timestamp < @endTime";
        sqLiteCommand.Bind("@beginTime", (object) day.Date);
        sqLiteCommand.Bind("@endTime", (object) day.Date.AddDays(1.0));
        return sqLiteCommand.ExecuteQuery<DbActivityMinuteData>().ToList<DbActivityMinuteData>();
      }
    }

    private void DeleteInInterval(SQLiteConnection connection, DateTime begin, DateTime end)
    {
      SQLiteCommand sqLiteCommand = new SQLiteCommand(connection);
      sqLiteCommand.CommandText = "DELETE FROM ActivityMinuteData WHERE Timestamp >= @beginTime AND Timestamp <= @endTime";
      sqLiteCommand.Bind("@beginTime", (object) begin);
      sqLiteCommand.Bind("@endTime", (object) end);
      sqLiteCommand.ExecuteNonQuery();
    }
  }
}
