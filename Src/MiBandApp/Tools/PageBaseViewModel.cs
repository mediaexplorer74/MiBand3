// Decompiled with JetBrains decompiler
// Type: MiBandApp.Tools.PageBaseViewModel
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.exe

using Caliburn.Micro;
using Microsoft.HockeyApp;
using System;
using System.Threading.Tasks;

#nullable disable
namespace MiBandApp.Tools
{
  public class PageBaseViewModel : PropertyChangedEx, IActivate, IDeactivate
  {
    public bool IsActive { get; set; }

    protected virtual Task OnActivate() => Task.Delay(0);

    protected virtual Task OnDeactivate(bool close = true) => Task.Delay(0);

    public async void Activate()
    {
      this.IsActive = true;
      await this.OnActivate().ConfigureAwait(true);
      HockeyClient.Current.TrackPageView(this.GetType().Name.Replace("ViewModel", string.Empty));
    }

    public async void Deactivate(bool close)
    {
      this.IsActive = false;
      await this.OnDeactivate(close).ConfigureAwait(true);
    }

    public event EventHandler<ActivationEventArgs> Activated;

    public event EventHandler<DeactivationEventArgs> AttemptingDeactivation;

    public event EventHandler<DeactivationEventArgs> Deactivated;
  }
}
