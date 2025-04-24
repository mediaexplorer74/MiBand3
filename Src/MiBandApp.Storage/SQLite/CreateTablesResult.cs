
// Type: SQLite.CreateTablesResult
// Assembly: MiBandApp.Storage, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 12BDBED5-65C6-40FC-A182-B057969144FE
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.Storage.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace SQLite
{
  public class CreateTablesResult
  {
    public Dictionary<Type, int> Results { get; private set; }

    internal CreateTablesResult() => this.Results = new Dictionary<Type, int>();
  }
}
