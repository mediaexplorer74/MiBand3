
// Type: SQLite.TableMapping
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
  public class TableMapping
  {
    private TableMapping.Column _autoPk;
    private TableMapping.Column[] _insertColumns;
    private TableMapping.Column[] _insertOrReplaceColumns;
    private PreparedSqlLiteInsertCommand _insertCommand;
    private string _insertCommandExtra;

    public Type MappedType { get; private set; }

    public string TableName { get; private set; }

    public TableMapping.Column[] Columns { get; private set; }

    public TableMapping.Column PK { get; private set; }

    public string GetByPrimaryKeySql { get; private set; }

    public TableMapping(Type type, CreateFlags createFlags = CreateFlags.None)
    {
      this.MappedType = type;
      TableAttribute customAttribute = (TableAttribute) type.GetTypeInfo().GetCustomAttribute(typeof (TableAttribute), true);
      this.TableName = customAttribute != null ? customAttribute.Name : this.MappedType.Name;
      IEnumerable<PropertyInfo> propertyInfos = this.MappedType.GetRuntimeProperties().Where<PropertyInfo>((Func<PropertyInfo, bool>) (p =>
      {
        if (p.GetMethod != null && p.GetMethod.IsPublic || p.SetMethod != null && p.SetMethod.IsPublic || p.GetMethod != null && p.GetMethod.IsStatic)
          return true;
        return p.SetMethod != null && p.SetMethod.IsStatic;
      }));
      List<TableMapping.Column> columnList = new List<TableMapping.Column>();
      foreach (PropertyInfo propertyInfo in propertyInfos)
      {
        bool flag = propertyInfo.GetCustomAttributes(typeof (IgnoreAttribute), true).Count<Attribute>() > 0;
        if (propertyInfo.CanWrite && !flag)
          columnList.Add(new TableMapping.Column(propertyInfo, createFlags));
      }
      this.Columns = columnList.ToArray();
      foreach (TableMapping.Column column in this.Columns)
      {
        if (column.IsAutoInc && column.IsPK)
          this._autoPk = column;
        if (column.IsPK)
          this.PK = column;
      }
      this.HasAutoIncPK = this._autoPk != null;
      if (this.PK != null)
        this.GetByPrimaryKeySql = string.Format("select * from \"{0}\" where \"{1}\" = ?", (object) this.TableName, (object) this.PK.Name);
      else
        this.GetByPrimaryKeySql = string.Format("select * from \"{0}\" limit 1", (object) this.TableName);
    }

    public bool HasAutoIncPK { get; private set; }

    public void SetAutoIncPK(object obj, long id)
    {
      if (this._autoPk == null)
        return;
      this._autoPk.SetValue(obj, Convert.ChangeType((object) id, this._autoPk.ColumnType, (IFormatProvider) null));
    }

    public TableMapping.Column[] InsertColumns
    {
      get
      {
        if (this._insertColumns == null)
          this._insertColumns = ((IEnumerable<TableMapping.Column>) this.Columns).Where<TableMapping.Column>((Func<TableMapping.Column, bool>) (c => !c.IsAutoInc)).ToArray<TableMapping.Column>();
        return this._insertColumns;
      }
    }

    public TableMapping.Column[] InsertOrReplaceColumns
    {
      get
      {
        if (this._insertOrReplaceColumns == null)
          this._insertOrReplaceColumns = ((IEnumerable<TableMapping.Column>) this.Columns).ToArray<TableMapping.Column>();
        return this._insertOrReplaceColumns;
      }
    }

    public TableMapping.Column FindColumnWithPropertyName(string propertyName)
    {
      return ((IEnumerable<TableMapping.Column>) this.Columns).FirstOrDefault<TableMapping.Column>((Func<TableMapping.Column, bool>) (c => c.PropertyName == propertyName));
    }

    public TableMapping.Column FindColumn(string columnName)
    {
      return ((IEnumerable<TableMapping.Column>) this.Columns).FirstOrDefault<TableMapping.Column>((Func<TableMapping.Column, bool>) (c => c.Name == columnName));
    }

    public PreparedSqlLiteInsertCommand GetInsertCommand(SQLiteConnection conn, string extra)
    {
      if (this._insertCommand == null)
      {
        this._insertCommand = this.CreateInsertCommand(conn, extra);
        this._insertCommandExtra = extra;
      }
      else if (this._insertCommandExtra != extra)
      {
        this._insertCommand.Dispose();
        this._insertCommand = this.CreateInsertCommand(conn, extra);
        this._insertCommandExtra = extra;
      }
      return this._insertCommand;
    }

    private PreparedSqlLiteInsertCommand CreateInsertCommand(SQLiteConnection conn, string extra)
    {
      TableMapping.Column[] source = this.InsertColumns;
      string str;
      if (!((IEnumerable<TableMapping.Column>) source).Any<TableMapping.Column>() && ((IEnumerable<TableMapping.Column>) this.Columns).Count<TableMapping.Column>() == 1 && this.Columns[0].IsAutoInc)
      {
        str = string.Format("insert {1} into \"{0}\" default values", (object) this.TableName, (object) extra);
      }
      else
      {
        if (string.Compare(extra, "OR REPLACE", StringComparison.OrdinalIgnoreCase) == 0)
          source = this.InsertOrReplaceColumns;
        str = string.Format("insert {3} into \"{0}\"({1}) values ({2})", (object) this.TableName, (object) string.Join(",", ((IEnumerable<TableMapping.Column>) source).Select<TableMapping.Column, string>((Func<TableMapping.Column, string>) (c => "\"" + c.Name + "\"")).ToArray<string>()), (object) string.Join(",", ((IEnumerable<TableMapping.Column>) source).Select<TableMapping.Column, string>((Func<TableMapping.Column, string>) (c => "?")).ToArray<string>()), (object) extra);
      }
      return new PreparedSqlLiteInsertCommand(conn)
      {
        CommandText = str
      };
    }

    protected internal void Dispose()
    {
      if (this._insertCommand == null)
        return;
      this._insertCommand.Dispose();
      this._insertCommand = (PreparedSqlLiteInsertCommand) null;
    }

    public class Column
    {
      private PropertyInfo _prop;

      public string Name { get; private set; }

      public string PropertyName => this._prop.Name;

      public Type ColumnType { get; private set; }

      public string Collation { get; private set; }

      public bool IsAutoInc { get; private set; }

      public bool IsAutoGuid { get; private set; }

      public bool IsPK { get; private set; }

      public IEnumerable<IndexedAttribute> Indices { get; set; }

      public bool IsNullable { get; private set; }

      public int? MaxStringLength { get; private set; }

      public Column(PropertyInfo prop, CreateFlags createFlags = CreateFlags.None)
      {
        ColumnAttribute columnAttribute = (ColumnAttribute) prop.GetCustomAttributes(typeof (ColumnAttribute), true).FirstOrDefault<Attribute>();
        this._prop = prop;
        this.Name = columnAttribute == null ? prop.Name : columnAttribute.Name;
        this.ColumnType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
        this.Collation = Orm.Collation((MemberInfo) prop);
        this.IsPK = Orm.IsPK((MemberInfo) prop) || (createFlags & CreateFlags.ImplicitPK) == CreateFlags.ImplicitPK && string.Compare(prop.Name, "Id", StringComparison.OrdinalIgnoreCase) == 0;
        bool flag = Orm.IsAutoInc((MemberInfo) prop) || this.IsPK && (createFlags & CreateFlags.AutoIncPK) == CreateFlags.AutoIncPK;
        this.IsAutoGuid = flag && this.ColumnType == typeof (Guid);
        this.IsAutoInc = flag && !this.IsAutoGuid;
        this.Indices = Orm.GetIndices((MemberInfo) prop);
        if (!this.Indices.Any<IndexedAttribute>() && !this.IsPK && (createFlags & CreateFlags.ImplicitIndex) == CreateFlags.ImplicitIndex && this.Name.EndsWith("Id", StringComparison.OrdinalIgnoreCase))
          this.Indices = (IEnumerable<IndexedAttribute>) new IndexedAttribute[1]
          {
            new IndexedAttribute()
          };
        this.IsNullable = !this.IsPK && !Orm.IsMarkedNotNull((MemberInfo) prop);
        this.MaxStringLength = Orm.MaxStringLength(prop);
      }

      public void SetValue(object obj, object val)
      {
        this._prop.SetValue(obj, val, (object[]) null);
      }

      public object GetValue(object obj) => this._prop.GetValue(obj, (object[]) null);
    }
  }
}
