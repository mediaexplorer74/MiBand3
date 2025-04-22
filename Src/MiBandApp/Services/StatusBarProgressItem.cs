// Decompiled with JetBrains decompiler
// Type: MiBandApp.Services.StatusBarProgressItem
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.exe

using System;

#nullable disable
namespace MiBandApp.Services
{
  public class StatusBarProgressItem : StatusBarItem
  {
    public StatusBarProgressItem(string text, double? progress)
      : base(text)
    {
      this.Progress = progress;
    }

    public event EventHandler OnHide = (_param1, _param2) => { };

    public event EventHandler ProgressUpdated = (_param1, _param2) => { };

    public bool IsHidden { get; private set; }

    public double? Progress { get; private set; }

    public void UpdateProgress(double? progress)
    {
      this.Progress = progress;
      this.ProgressUpdated((object) this, EventArgs.Empty);
    }

    public void Hide()
    {
      this.IsHidden = true;
      this.OnHide((object) this, EventArgs.Empty);
    }
  }
}
