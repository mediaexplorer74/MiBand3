// Decompiled with JetBrains decompiler
// Type: SQLite.BaseTableQuery
// Assembly: MiBandApp.Storage, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 12BDBED5-65C6-40FC-A182-B057969144FE
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.Storage.dll

#nullable disable
namespace SQLite
{
  public abstract class BaseTableQuery
  {
    protected class Ordering
    {
      public string ColumnName { get; set; }

      public bool Ascending { get; set; }
    }
  }
}
