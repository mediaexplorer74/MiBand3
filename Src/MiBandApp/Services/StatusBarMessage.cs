// Decompiled with JetBrains decompiler
// Type: MiBandApp.Services.StatusBarMessage
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.exe

using System;

#nullable disable
namespace MiBandApp.Services
{
  public class StatusBarMessage : StatusBarItem
  {
    public StatusBarMessage(string text, TimeSpan displayingTimeSpan)
      : base(text)
    {
      this.DisplayingTimeSpan = displayingTimeSpan;
    }

    public TimeSpan DisplayingTimeSpan { get; private set; }
  }
}
