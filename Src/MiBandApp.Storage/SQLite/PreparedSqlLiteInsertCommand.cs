
// Type: SQLite.PreparedSqlLiteInsertCommand
// Assembly: MiBandApp.Storage, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 12BDBED5-65C6-40FC-A182-B057969144FE
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.Storage.dll

using System;

#nullable disable
namespace SQLite
{
  public class PreparedSqlLiteInsertCommand : IDisposable
  {
    internal static readonly IntPtr NullStatement;

    public bool Initialized { get; set; }

    protected SQLiteConnection Connection { get; set; }

    public string CommandText { get; set; }

    protected IntPtr Statement { get; set; }

    internal PreparedSqlLiteInsertCommand(SQLiteConnection conn) => this.Connection = conn;

    public int ExecuteNonQuery(object[] source)
    {
      int num1 = this.Connection.Trace ? 1 : 0;
      if (!this.Initialized)
      {
        this.Statement = this.Prepare();
        this.Initialized = true;
      }
      if (source != null)
      {
        for (int index = 0; index < source.Length; ++index)
          SQLiteCommand.BindParameter(this.Statement, index + 1, source[index], this.Connection.StoreDateTimeAsTicks);
      }
      SQLite3.Result r = SQLite3.Step(this.Statement);
      switch (r)
      {
        case SQLite3.Result.Error:
          string errmsg = SQLite3.GetErrmsg(this.Connection.Handle);
          int num2 = (int) SQLite3.Reset(this.Statement);
          throw SQLiteException.New(r, errmsg);
        case SQLite3.Result.Constraint:
          if (SQLite3.ExtendedErrCode(this.Connection.Handle) == SQLite3.ExtendedResult.ConstraintNotNull)
          {
            int num3 = (int) SQLite3.Reset(this.Statement);
            throw NotNullConstraintViolationException.New(r, SQLite3.GetErrmsg(this.Connection.Handle));
          }
          break;
        case SQLite3.Result.Done:
          int num4 = SQLite3.Changes(this.Connection.Handle);
          int num5 = (int) SQLite3.Reset(this.Statement);
          return num4;
      }
      int num6 = (int) SQLite3.Reset(this.Statement);
      throw SQLiteException.New(r, r.ToString());
    }

    protected virtual IntPtr Prepare()
    {
      return SQLite3.Prepare2(this.Connection.Handle, this.CommandText);
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    private void Dispose(bool disposing)
    {
      if (!(this.Statement != PreparedSqlLiteInsertCommand.NullStatement))
        return;
      try
      {
        int num = (int) SQLite3.Finalize(this.Statement);
      }
      finally
      {
        this.Statement = PreparedSqlLiteInsertCommand.NullStatement;
        this.Connection = (SQLiteConnection) null;
      }
    }

    ~PreparedSqlLiteInsertCommand() => this.Dispose(false);
  }
}
