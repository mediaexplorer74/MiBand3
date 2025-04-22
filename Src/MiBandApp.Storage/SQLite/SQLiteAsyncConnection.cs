// Decompiled with JetBrains decompiler
// Type: SQLite.SQLiteAsyncConnection
// Assembly: MiBandApp.Storage, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 12BDBED5-65C6-40FC-A182-B057969144FE
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.Storage.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

#nullable disable
namespace SQLite
{
  public class SQLiteAsyncConnection
  {
    private SQLiteConnectionString _connectionString;
    private SQLiteOpenFlags _openFlags;

    public SQLiteAsyncConnection(string databasePath, bool storeDateTimeAsTicks = false)
      : this(databasePath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create, storeDateTimeAsTicks)
    {
    }

    public SQLiteAsyncConnection(
      string databasePath,
      SQLiteOpenFlags openFlags,
      bool storeDateTimeAsTicks = false)
    {
      this._openFlags = openFlags;
      this._connectionString = new SQLiteConnectionString(databasePath, storeDateTimeAsTicks);
    }

    private SQLiteConnectionWithLock GetConnection()
    {
      return SQLiteConnectionPool.Shared.GetConnection(this._connectionString, this._openFlags);
    }

    public Task<CreateTablesResult> CreateTableAsync<T>() where T : new()
    {
      return this.CreateTablesAsync(typeof (T));
    }

    public Task<CreateTablesResult> CreateTablesAsync<T, T2>()
      where T : new()
      where T2 : new()
    {
      return this.CreateTablesAsync(typeof (T), typeof (T2));
    }

    public Task<CreateTablesResult> CreateTablesAsync<T, T2, T3>()
      where T : new()
      where T2 : new()
      where T3 : new()
    {
      return this.CreateTablesAsync(typeof (T), typeof (T2), typeof (T3));
    }

    public Task<CreateTablesResult> CreateTablesAsync<T, T2, T3, T4>()
      where T : new()
      where T2 : new()
      where T3 : new()
      where T4 : new()
    {
      return this.CreateTablesAsync(typeof (T), typeof (T2), typeof (T3), typeof (T4));
    }

    public Task<CreateTablesResult> CreateTablesAsync<T, T2, T3, T4, T5>()
      where T : new()
      where T2 : new()
      where T3 : new()
      where T4 : new()
      where T5 : new()
    {
      return this.CreateTablesAsync(typeof (T), typeof (T2), typeof (T3), typeof (T4), typeof (T5));
    }

    public Task<CreateTablesResult> CreateTablesAsync(params Type[] types)
    {
      return Task.Factory.StartNew<CreateTablesResult>((Func<CreateTablesResult>) (() =>
      {
        CreateTablesResult tablesAsync = new CreateTablesResult();
        SQLiteConnectionWithLock connection = this.GetConnection();
        using (connection.Lock())
        {
          foreach (Type type in types)
          {
            int table = connection.CreateTable(type);
            tablesAsync.Results[type] = table;
          }
        }
        return tablesAsync;
      }));
    }

    public Task<int> DropTableAsync<T>() where T : new()
    {
      return Task.Factory.StartNew<int>((Func<int>) (() =>
      {
        SQLiteConnectionWithLock connection = this.GetConnection();
        using (connection.Lock())
          return connection.DropTable<T>();
      }));
    }

    public Task<int> InsertAsync(object item)
    {
      return Task.Factory.StartNew<int>((Func<int>) (() =>
      {
        SQLiteConnectionWithLock connection = this.GetConnection();
        using (connection.Lock())
          return connection.Insert(item);
      }));
    }

    public Task<int> UpdateAsync(object item)
    {
      return Task.Factory.StartNew<int>((Func<int>) (() =>
      {
        SQLiteConnectionWithLock connection = this.GetConnection();
        using (connection.Lock())
          return connection.Update(item);
      }));
    }

    public Task<int> DeleteAsync(object item)
    {
      return Task.Factory.StartNew<int>((Func<int>) (() =>
      {
        SQLiteConnectionWithLock connection = this.GetConnection();
        using (connection.Lock())
          return connection.Delete(item);
      }));
    }

    public Task<T> GetAsync<T>(object pk) where T : new()
    {
      return Task.Factory.StartNew<T>((Func<T>) (() =>
      {
        SQLiteConnectionWithLock connection = this.GetConnection();
        using (connection.Lock())
          return connection.Get<T>(pk);
      }));
    }

    public Task<T> FindAsync<T>(object pk) where T : new()
    {
      return Task.Factory.StartNew<T>((Func<T>) (() =>
      {
        SQLiteConnectionWithLock connection = this.GetConnection();
        using (connection.Lock())
          return connection.Find<T>(pk);
      }));
    }

    public Task<T> GetAsync<T>(Expression<Func<T, bool>> predicate) where T : new()
    {
      return Task.Factory.StartNew<T>((Func<T>) (() =>
      {
        SQLiteConnectionWithLock connection = this.GetConnection();
        using (connection.Lock())
          return connection.Get<T>(predicate);
      }));
    }

    public Task<T> FindAsync<T>(Expression<Func<T, bool>> predicate) where T : new()
    {
      return Task.Factory.StartNew<T>((Func<T>) (() =>
      {
        SQLiteConnectionWithLock connection = this.GetConnection();
        using (connection.Lock())
          return connection.Find<T>(predicate);
      }));
    }

    public Task<int> ExecuteAsync(string query, params object[] args)
    {
      return Task<int>.Factory.StartNew((Func<int>) (() =>
      {
        SQLiteConnectionWithLock connection = this.GetConnection();
        using (connection.Lock())
          return connection.Execute(query, args);
      }));
    }

    public Task<int> InsertAllAsync(IEnumerable items)
    {
      return Task.Factory.StartNew<int>((Func<int>) (() =>
      {
        SQLiteConnectionWithLock connection = this.GetConnection();
        using (connection.Lock())
          return connection.InsertAll(items);
      }));
    }

    public Task<int> UpdateAllAsync(IEnumerable items)
    {
      return Task.Factory.StartNew<int>((Func<int>) (() =>
      {
        SQLiteConnectionWithLock connection = this.GetConnection();
        using (connection.Lock())
          return connection.UpdateAll(items);
      }));
    }

    [Obsolete("Will cause a deadlock if any call in action ends up in a different thread. Use RunInTransactionAsync(Action<SQLiteConnection>) instead.")]
    public Task RunInTransactionAsync(Action<SQLiteAsyncConnection> action)
    {
      return Task.Factory.StartNew((Action) (() =>
      {
        SQLiteConnectionWithLock connection = this.GetConnection();
        using (connection.Lock())
        {
          connection.BeginTransaction();
          try
          {
            action(this);
            connection.Commit();
          }
          catch (Exception ex)
          {
            connection.Rollback();
            throw;
          }
        }
      }));
    }

    public Task RunInTransactionAsync(Action<SQLiteConnection> action)
    {
      return Task.Factory.StartNew((Action) (() =>
      {
        SQLiteConnectionWithLock connection = this.GetConnection();
        using (connection.Lock())
        {
          connection.BeginTransaction();
          try
          {
            action((SQLiteConnection) connection);
            connection.Commit();
          }
          catch (Exception ex)
          {
            connection.Rollback();
            throw;
          }
        }
      }));
    }

    public AsyncTableQuery<T> Table<T>() where T : new()
    {
      return new AsyncTableQuery<T>(this.GetConnection().Table<T>());
    }

    public Task<T> ExecuteScalarAsync<T>(string sql, params object[] args)
    {
      return Task<T>.Factory.StartNew((Func<T>) (() =>
      {
        SQLiteConnectionWithLock connection = this.GetConnection();
        using (connection.Lock())
          return connection.CreateCommand(sql, args).ExecuteScalar<T>();
      }));
    }

    public Task<List<T>> QueryAsync<T>(string sql, params object[] args) where T : new()
    {
      return Task<List<T>>.Factory.StartNew((Func<List<T>>) (() =>
      {
        SQLiteConnectionWithLock connection = this.GetConnection();
        using (connection.Lock())
          return connection.Query<T>(sql, args);
      }));
    }
  }
}
