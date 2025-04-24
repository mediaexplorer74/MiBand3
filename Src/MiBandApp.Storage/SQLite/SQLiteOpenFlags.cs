
// Type: SQLite.SQLiteOpenFlags
// Assembly: MiBandApp.Storage, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 12BDBED5-65C6-40FC-A182-B057969144FE
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.Storage.dll

using System;

#nullable disable
namespace SQLite
{
  [Flags]
  public enum SQLiteOpenFlags
  {
    ReadOnly = 1,
    ReadWrite = 2,
    Create = 4,
    NoMutex = 32768, // 0x00008000
    FullMutex = 65536, // 0x00010000
    SharedCache = 131072, // 0x00020000
    PrivateCache = 262144, // 0x00040000
    ProtectionComplete = 1048576, // 0x00100000
    ProtectionCompleteUnlessOpen = 2097152, // 0x00200000
    ProtectionCompleteUntilFirstUserAuthentication = ProtectionCompleteUnlessOpen | ProtectionComplete, // 0x00300000
    ProtectionNone = 4194304, // 0x00400000
  }
}
