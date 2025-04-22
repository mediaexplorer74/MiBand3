// Decompiled with JetBrains decompiler
// Type: MiBandApp.Services.SerializableDataBase
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.exe

using MiBandApp.Storage.Tables;
using System;
using System.Collections.Generic;

#nullable disable
namespace MiBandApp.Services
{
  public class SerializableDataBase
  {
    public SerializableDataBase()
    {
      this.Version = "1.1";
      this.TimeStamp = DateTime.Now;
    }

    public string Version { get; set; }

    public DateTime TimeStamp { get; set; }

    public List<DaySummary> Days { get; set; }
  }
}
