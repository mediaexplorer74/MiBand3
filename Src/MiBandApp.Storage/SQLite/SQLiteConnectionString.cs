// Decompiled with JetBrains decompiler
// Type: SQLite.SQLiteConnectionString
// Assembly: MiBandApp.Storage, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 12BDBED5-65C6-40FC-A182-B057969144FE
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.Storage.dll

using System.IO;
using Windows.Storage;

#nullable disable
namespace SQLite
{
  internal class SQLiteConnectionString
  {
    private static readonly string MetroStyleDataPath = ApplicationData.Current.LocalFolder.Path;

    public string ConnectionString { get; private set; }

    public string DatabasePath { get; private set; }

    public bool StoreDateTimeAsTicks { get; private set; }

    public SQLiteConnectionString(string databasePath, bool storeDateTimeAsTicks)
    {
      this.ConnectionString = databasePath;
      this.StoreDateTimeAsTicks = storeDateTimeAsTicks;
      this.DatabasePath = Path.Combine(SQLiteConnectionString.MetroStyleDataPath, databasePath);
    }
  }
}
