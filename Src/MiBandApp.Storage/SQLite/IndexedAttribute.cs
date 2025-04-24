
// Type: SQLite.IndexedAttribute
// Assembly: MiBandApp.Storage, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 12BDBED5-65C6-40FC-A182-B057969144FE
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.Storage.dll

using System;

#nullable disable
namespace SQLite
{
  [AttributeUsage(AttributeTargets.Property)]
  public class IndexedAttribute : Attribute
  {
    public string Name { get; set; }

    public int Order { get; set; }

    public virtual bool Unique { get; set; }

    public IndexedAttribute()
    {
    }

    public IndexedAttribute(string name, int order)
    {
      this.Name = name;
      this.Order = order;
    }
  }
}
