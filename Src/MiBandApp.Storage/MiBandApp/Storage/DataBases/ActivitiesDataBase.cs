
// Type: MiBandApp.Storage.DataBases.ActivitiesDataBase
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
using System.Linq.Expressions;
using Windows.Storage;

#nullable disable
namespace MiBandApp.Storage.DataBases
{
  public class ActivitiesDataBase
  {
    private const string DatabaseFileName = "Activities.sqlite";
    private readonly string _dataBaseFilePath = Path.Combine(Path.Combine(ApplicationData.Current.LocalFolder.Path, "Activities.sqlite"));

    public void Init()
    {
      try
      {
        using (SQLiteConnection sqLiteConnection = new SQLiteConnection(this._dataBaseFilePath, true))
        {
          sqLiteConnection.CreateTable<DbSleepingActivity>();
          sqLiteConnection.CreateTable<DbWalkingActivity>();
          sqLiteConnection.CreateTable<DbRunningActivity>();
          sqLiteConnection.CreateTable<DbHeartRateMeasureActivity>();
        }
      }
      catch
      {
      }
    }

    public void AddOrReplaceActivities(IEnumerable<IDbUserActivity> activities)
    {
      List<IDbUserActivity> list = activities.ToList<IDbUserActivity>();
      DateTime begin = list.Select<IDbUserActivity, DateTime>((Func<IDbUserActivity, DateTime>) (t => t.Begin)).Min<DateTime>();
      List<Type> types = list.Select<IDbUserActivity, Type>((Func<IDbUserActivity, Type>) (t => t.GetType())).Distinct<Type>().ToList<Type>();
      List<IDbUserActivity> activities1 = this.GetActivities((Expression<Func<IDbUserActivity, bool>>) (t => t.Begin >= begin && types.Contains(t.GetType())));
      using (SQLiteConnection sqLiteConnection = new SQLiteConnection(this._dataBaseFilePath, true))
      {
        sqLiteConnection.BeginTransaction();
        try
        {
          foreach (IDbUserActivity objectToDelete in activities1)
            sqLiteConnection.Delete((object) objectToDelete);
          sqLiteConnection.InsertAll((IEnumerable) list);
          sqLiteConnection.Commit();
        }
        catch
        {
          sqLiteConnection.Rollback();
          throw;
        }
      }
    }

    public void AddActivities(IEnumerable<IDbUserActivity> activities)
    {
      List<IDbUserActivity> list = activities.ToList<IDbUserActivity>();
      using (SQLiteConnection sqLiteConnection = new SQLiteConnection(this._dataBaseFilePath, true))
      {
        sqLiteConnection.BeginTransaction();
        try
        {
          sqLiteConnection.InsertAll((IEnumerable) list);
          sqLiteConnection.Commit();
        }
        catch
        {
          sqLiteConnection.Rollback();
          throw;
        }
      }
    }

    public List<IDbUserActivity> GetActivities(Expression<Func<IDbUserActivity, bool>> predicate)
    {
      List<IDbUserActivity> source = new List<IDbUserActivity>();
      using (SQLiteConnection sqLiteConnection = new SQLiteConnection(this._dataBaseFilePath, true))
      {
        source.AddRange(((IEnumerable<IDbUserActivity>) sqLiteConnection.Table<DbSleepingActivity>()).Where<IDbUserActivity>(predicate.Compile()));
        source.AddRange(((IEnumerable<IDbUserActivity>) sqLiteConnection.Table<DbWalkingActivity>()).Where<IDbUserActivity>(predicate.Compile()));
        source.AddRange(((IEnumerable<IDbUserActivity>) sqLiteConnection.Table<DbRunningActivity>()).Where<IDbUserActivity>(predicate.Compile()));
        source.AddRange(((IEnumerable<IDbUserActivity>) sqLiteConnection.Table<DbHeartRateMeasureActivity>()).Where<IDbUserActivity>(predicate.Compile()));
      }
      return source.OrderBy<IDbUserActivity, DateTime>((Func<IDbUserActivity, DateTime>) (t => t.Begin)).ToList<IDbUserActivity>();
    }

    public List<IDbUserActivity> GetActivitiesInDay(DateTime day)
    {
      day = day.Date;
      List<IDbUserActivity> source = new List<IDbUserActivity>();
      using (SQLiteConnection sqLiteConnection = new SQLiteConnection(this._dataBaseFilePath, true))
      {
        source.AddRange(((IEnumerable<IDbUserActivity>) sqLiteConnection.Table<DbSleepingActivity>()).Where<IDbUserActivity>(this.Compile((Expression<Func<IDbUserActivity, bool>>) (t => t.End.Date == day))));
        source.AddRange(((IEnumerable<IDbUserActivity>) sqLiteConnection.Table<DbWalkingActivity>()).Where<IDbUserActivity>(this.Compile((Expression<Func<IDbUserActivity, bool>>) (t => t.Begin.Date == day))));
        source.AddRange(((IEnumerable<IDbUserActivity>) sqLiteConnection.Table<DbRunningActivity>()).Where<IDbUserActivity>(this.Compile((Expression<Func<IDbUserActivity, bool>>) (t => t.Begin.Date == day))));
        source.AddRange(((IEnumerable<IDbUserActivity>) sqLiteConnection.Table<DbHeartRateMeasureActivity>()).Where<IDbUserActivity>(this.Compile((Expression<Func<IDbUserActivity, bool>>) (t => t.Begin.Date == day))));
      }
      return source.OrderBy<IDbUserActivity, DateTime>((Func<IDbUserActivity, DateTime>) (t => t.Begin)).ToList<IDbUserActivity>();
    }

    public List<DbSleepingActivity> GetAllSleepActivities()
    {
      List<DbSleepingActivity> source = new List<DbSleepingActivity>();
      using (SQLiteConnection sqLiteConnection = new SQLiteConnection(this._dataBaseFilePath, true))
        source.AddRange((IEnumerable<DbSleepingActivity>) sqLiteConnection.Table<DbSleepingActivity>());
      return source.OrderBy<DbSleepingActivity, DateTime>((Func<DbSleepingActivity, DateTime>) (t => t.Begin)).ToList<DbSleepingActivity>();
    }

    private Func<IDbUserActivity, bool> Compile(Expression<Func<IDbUserActivity, bool>> predicate)
    {
      return predicate.Compile();
    }
  }
}
