
// Type: SQLite.NotNullConstraintViolationException
// Assembly: MiBandApp.Storage, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 12BDBED5-65C6-40FC-A182-B057969144FE
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.Storage.dll

using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace SQLite
{
  public class NotNullConstraintViolationException : SQLiteException
  {
    public IEnumerable<TableMapping.Column> Columns { get; protected set; }

    protected NotNullConstraintViolationException(SQLite3.Result r, string message)
      : this(r, message, (TableMapping) null, (object) null)
    {
    }

    protected NotNullConstraintViolationException(
      SQLite3.Result r,
      string message,
      TableMapping mapping,
      object obj)
      : base(r, message)
    {
      if (mapping == null || obj == null)
        return;
      this.Columns = ((IEnumerable<TableMapping.Column>) mapping.Columns).Where<TableMapping.Column>((Func<TableMapping.Column, bool>) (c => !c.IsNullable && c.GetValue(obj) == null));
    }

    public static NotNullConstraintViolationException New(SQLite3.Result r, string message)
    {
      return new NotNullConstraintViolationException(r, message);
    }

    public static NotNullConstraintViolationException New(
      SQLite3.Result r,
      string message,
      TableMapping mapping,
      object obj)
    {
      return new NotNullConstraintViolationException(r, message, mapping, obj);
    }

    public static NotNullConstraintViolationException New(
      SQLiteException exception,
      TableMapping mapping,
      object obj)
    {
      return new NotNullConstraintViolationException(exception.Result, exception.Message, mapping, obj);
    }
  }
}
