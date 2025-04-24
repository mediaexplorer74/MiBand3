
// Type: SQLite.SQLiteException
// Assembly: MiBandApp.Storage, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 12BDBED5-65C6-40FC-A182-B057969144FE
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.Storage.dll

using System;

#nullable disable
namespace SQLite
{
  public class SQLiteException : Exception
  {
    public SQLite3.Result Result { get; private set; }

    protected SQLiteException(SQLite3.Result r, string message)
      : base(message)
    {
      this.Result = r;
    }

    public static SQLiteException New(SQLite3.Result r, string message)
    {
      return new SQLiteException(r, message);
    }
  }
}
