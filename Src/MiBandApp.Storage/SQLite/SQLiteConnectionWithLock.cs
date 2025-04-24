
// Type: SQLite.SQLiteConnectionWithLock
// Assembly: MiBandApp.Storage, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 12BDBED5-65C6-40FC-A182-B057969144FE
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.Storage.dll

using System;
using System.Threading;

#nullable disable
namespace SQLite
{
  internal class SQLiteConnectionWithLock(
    SQLiteConnectionString connectionString,
    SQLiteOpenFlags openFlags) : SQLiteConnection(connectionString.DatabasePath, openFlags, connectionString.StoreDateTimeAsTicks)
  {
    private readonly object _lockPoint = new object();

    public IDisposable Lock()
    {
      return (IDisposable) new SQLiteConnectionWithLock.LockWrapper(this._lockPoint);
    }

    private class LockWrapper : IDisposable
    {
      private object _lockPoint;

      public LockWrapper(object lockPoint)
      {
        this._lockPoint = lockPoint;
        Monitor.Enter(this._lockPoint);
      }

      public void Dispose() => Monitor.Exit(this._lockPoint);
    }
  }
}
