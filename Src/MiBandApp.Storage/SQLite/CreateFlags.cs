
// Type: SQLite.CreateFlags
// Assembly: MiBandApp.Storage, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 12BDBED5-65C6-40FC-A182-B057969144FE
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.Storage.dll

using System;

#nullable disable
namespace SQLite
{
  [Flags]
  public enum CreateFlags
  {
    None = 0,
    ImplicitPK = 1,
    ImplicitIndex = 2,
    AllImplicit = ImplicitIndex | ImplicitPK, // 0x00000003
    AutoIncPK = 4,
  }
}
