
// Type: SQLite.SQLiteCommand
// Assembly: MiBandApp.Storage, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 12BDBED5-65C6-40FC-A182-B057969144FE
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.Storage.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#nullable disable
namespace SQLite
{
  public class SQLiteCommand
  {
    private SQLiteConnection _conn;
    private List<SQLiteCommand.Binding> _bindings;
    internal static IntPtr NegativePointer = new IntPtr(-1);

    public string CommandText { get; set; }

    internal SQLiteCommand(SQLiteConnection conn)
    {
      this._conn = conn;
      this._bindings = new List<SQLiteCommand.Binding>();
      this.CommandText = "";
    }

    public int ExecuteNonQuery()
    {
      int num = this._conn.Trace ? 1 : 0;
      IntPtr stmt = this.Prepare();
      SQLite3.Result r = SQLite3.Step(stmt);
      this.Finalize(stmt);
      switch (r)
      {
        case SQLite3.Result.Error:
          string errmsg = SQLite3.GetErrmsg(this._conn.Handle);
          throw SQLiteException.New(r, errmsg);
        case SQLite3.Result.Constraint:
          if (SQLite3.ExtendedErrCode(this._conn.Handle) == SQLite3.ExtendedResult.ConstraintNotNull)
            throw NotNullConstraintViolationException.New(r, SQLite3.GetErrmsg(this._conn.Handle));
          break;
        case SQLite3.Result.Done:
          return SQLite3.Changes(this._conn.Handle);
      }
      throw SQLiteException.New(r, r.ToString());
    }

    public IEnumerable<T> ExecuteDeferredQuery<T>()
    {
      return this.ExecuteDeferredQuery<T>(this._conn.GetMapping(typeof (T)));
    }

    public List<T> ExecuteQuery<T>()
    {
      return this.ExecuteDeferredQuery<T>(this._conn.GetMapping(typeof (T))).ToList<T>();
    }

    public List<T> ExecuteQuery<T>(TableMapping map)
    {
      return this.ExecuteDeferredQuery<T>(map).ToList<T>();
    }

    protected virtual void OnInstanceCreated(object obj)
    {
    }

    public IEnumerable<T> ExecuteDeferredQuery<T>(TableMapping map)
    {
      int num1 = this._conn.Trace ? 1 : 0;
      IntPtr stmt = this.Prepare();
      try
      {
        TableMapping.Column[] cols = new TableMapping.Column[SQLite3.ColumnCount(stmt)];
        for (int index = 0; index < cols.Length; ++index)
        {
          string columnName = SQLite3.ColumnName16(stmt, index);
          cols[index] = map.FindColumn(columnName);
        }
        while (SQLite3.Step(stmt) == SQLite3.Result.Row)
        {
          object instance = Activator.CreateInstance(map.MappedType);
          for (int index = 0; index < cols.Length; ++index)
          {
            if (cols[index] != null)
            {
              SQLite3.ColType type = SQLite3.ColumnType(stmt, index);
              object val = this.ReadCol(stmt, index, type, cols[index].ColumnType);
              cols[index].SetValue(instance, val);
            }
          }
          this.OnInstanceCreated(instance);
          yield return (T) instance;
        }
        cols = (TableMapping.Column[]) null;
      }
      finally
      {
        int num2 = (int) SQLite3.Finalize(stmt);
      }
    }

    public T ExecuteScalar<T>()
    {
      int num = this._conn.Trace ? 1 : 0;
      T obj = default (T);
      IntPtr stmt = this.Prepare();
      try
      {
        SQLite3.Result r = SQLite3.Step(stmt);
        switch (r)
        {
          case SQLite3.Result.Row:
            SQLite3.ColType type = SQLite3.ColumnType(stmt, 0);
            obj = (T) this.ReadCol(stmt, 0, type, typeof (T));
            break;
          case SQLite3.Result.Done:
            break;
          default:
            throw SQLiteException.New(r, SQLite3.GetErrmsg(this._conn.Handle));
        }
      }
      finally
      {
        this.Finalize(stmt);
      }
      return obj;
    }

    public void Bind(string name, object val)
    {
      this._bindings.Add(new SQLiteCommand.Binding()
      {
        Name = name,
        Value = val
      });
    }

    public void Bind(object val) => this.Bind((string) null, val);

    public override string ToString()
    {
      string[] strArray = new string[1 + this._bindings.Count];
      strArray[0] = this.CommandText;
      int index = 1;
      foreach (SQLiteCommand.Binding binding in this._bindings)
      {
        strArray[index] = string.Format("  {0}: {1}", (object) (index - 1), binding.Value);
        ++index;
      }
      return string.Join(Environment.NewLine, strArray);
    }

    private IntPtr Prepare()
    {
      IntPtr stmt = SQLite3.Prepare2(this._conn.Handle, this.CommandText);
      this.BindAll(stmt);
      return stmt;
    }

    private void Finalize(IntPtr stmt)
    {
      int num = (int) SQLite3.Finalize(stmt);
    }

    private void BindAll(IntPtr stmt)
    {
      int num = 1;
      foreach (SQLiteCommand.Binding binding in this._bindings)
      {
        binding.Index = binding.Name == null ? num++ : SQLite3.BindParameterIndex(stmt, binding.Name);
        SQLiteCommand.BindParameter(stmt, binding.Index, binding.Value, this._conn.StoreDateTimeAsTicks);
      }
    }

    internal static void BindParameter(
      IntPtr stmt,
      int index,
      object value,
      bool storeDateTimeAsTicks)
    {
      switch (value)
      {
        case null:
          SQLite3.BindNull(stmt, index);
          break;
        case int val:
          SQLite3.BindInt(stmt, index, val);
          break;
        case string _:
          SQLite3.BindText(stmt, index, (string) value, -1, SQLiteCommand.NegativePointer);
          break;
        case byte _:
        case ushort _:
        case sbyte _:
        case short _:
          SQLite3.BindInt(stmt, index, Convert.ToInt32(value));
          break;
        case bool flag:
          SQLite3.BindInt(stmt, index, flag ? 1 : 0);
          break;
        case uint _:
        case long _:
          SQLite3.BindInt64(stmt, index, Convert.ToInt64(value));
          break;
        case float _:
        case double _:
        case Decimal _:
          SQLite3.BindDouble(stmt, index, Convert.ToDouble(value));
          break;
        case TimeSpan timeSpan:
          SQLite3.BindInt64(stmt, index, timeSpan.Ticks);
          break;
        case DateTime _:
          if (storeDateTimeAsTicks)
          {
            SQLite3.BindInt64(stmt, index, ((DateTime) value).Ticks);
            break;
          }
          SQLite3.BindText(stmt, index, ((DateTime) value).ToString("yyyy-MM-dd HH:mm:ss"), -1, SQLiteCommand.NegativePointer);
          break;
        case DateTimeOffset dateTimeOffset:
          SQLite3.BindInt64(stmt, index, dateTimeOffset.UtcTicks);
          break;
        default:
          if (value.GetType().GetTypeInfo().IsEnum)
          {
            SQLite3.BindInt(stmt, index, Convert.ToInt32(value));
            break;
          }
          switch (value)
          {
            case byte[] _:
              SQLite3.BindBlob(stmt, index, (byte[]) value, ((byte[]) value).Length, SQLiteCommand.NegativePointer);
              return;
            case Guid guid:
              SQLite3.BindText(stmt, index, guid.ToString(), 72, SQLiteCommand.NegativePointer);
              return;
            default:
              throw new NotSupportedException("Cannot store type: " + (object) value.GetType());
          }
      }
    }

    private object ReadCol(IntPtr stmt, int index, SQLite3.ColType type, Type clrType)
    {
      if (type == SQLite3.ColType.Null)
        return (object) null;
      if (clrType == typeof (string))
        return (object) SQLite3.ColumnString(stmt, index);
      if (clrType == typeof (int))
        return (object) SQLite3.ColumnInt(stmt, index);
      if (clrType == typeof (bool))
        return (object) (SQLite3.ColumnInt(stmt, index) == 1);
      if (clrType == typeof (double))
        return (object) SQLite3.ColumnDouble(stmt, index);
      if (clrType == typeof (float))
        return (object) (float) SQLite3.ColumnDouble(stmt, index);
      if (clrType == typeof (TimeSpan))
        return (object) new TimeSpan(SQLite3.ColumnInt64(stmt, index));
      if (clrType == typeof (DateTime))
        return this._conn.StoreDateTimeAsTicks ? (object) new DateTime(SQLite3.ColumnInt64(stmt, index)) : (object) DateTime.Parse(SQLite3.ColumnString(stmt, index));
      if (clrType == typeof (DateTimeOffset))
        return (object) new DateTimeOffset(SQLite3.ColumnInt64(stmt, index), TimeSpan.Zero);
      if (clrType.GetTypeInfo().IsEnum)
        return (object) SQLite3.ColumnInt(stmt, index);
      if (clrType == typeof (long))
        return (object) SQLite3.ColumnInt64(stmt, index);
      if (clrType == typeof (uint))
        return (object) (uint) SQLite3.ColumnInt64(stmt, index);
      if (clrType == typeof (Decimal))
        return (object) (Decimal) SQLite3.ColumnDouble(stmt, index);
      if (clrType == typeof (byte))
        return (object) (byte) SQLite3.ColumnInt(stmt, index);
      if (clrType == typeof (ushort))
        return (object) (ushort) SQLite3.ColumnInt(stmt, index);
      if (clrType == typeof (short))
        return (object) (short) SQLite3.ColumnInt(stmt, index);
      if (clrType == typeof (sbyte))
        return (object) (sbyte) SQLite3.ColumnInt(stmt, index);
      if (clrType == typeof (byte[]))
        return (object) SQLite3.ColumnByteArray(stmt, index);
      if (clrType == typeof (Guid))
        return (object) new Guid(SQLite3.ColumnString(stmt, index));
      throw new NotSupportedException("Don't know how to read " + (object) clrType);
    }

    private class Binding
    {
      public string Name { get; set; }

      public object Value { get; set; }

      public int Index { get; set; }
    }
  }
}
