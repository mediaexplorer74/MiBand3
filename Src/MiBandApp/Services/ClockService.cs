
// Type: MiBandApp.Services.ClockService
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// 

using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.ApplicationModel;
using Windows.UI.Xaml;

#nullable disable
namespace MiBandApp.Services
{
  public class ClockService
  {
    private readonly object _subscriptionLock = new object();
    private EventHandler<ClockTickEventArgs> _minuteEventHandler;
    private Timer _minuteTimer;

    public event EventHandler<ClockTickEventArgs> MinuteTick
    {
      add
      {
        lock (this._subscriptionLock)
        {
          this._minuteEventHandler += value;
          this.EnableMinuteTicks();
        }
      }
      remove
      {
        lock (this._subscriptionLock)
        {
          this._minuteEventHandler -= value;
          if (this._minuteEventHandler != null)
            return;
          this.DisableMinuteTicks();
        }
      }
    }

        private void EnableMinuteTicks()
        {
            if (this._minuteTimer != null)
                return;
            this.InitTimer();
            Application current1 = Application.Current;
            current1.Suspending += this.AppOnSuspending; // Use event subscription syntax
            Application current2 = Application.Current;
            current2.Resuming += this.AppOnResuming; // Use event subscription syntax
        }

        private void DisableMinuteTicks()
        {
            if (this._minuteTimer == null)
                return;
            this._minuteTimer.Dispose();
            this._minuteTimer = (Timer)null;
            Application.Current.Suspending -= this.AppOnSuspending; // Use event unsubscription syntax
            Application.Current.Resuming -= this.AppOnResuming; // Use event unsubscription syntax
        }

    private void InitTimer()
    {
      this._minuteTimer = new Timer(new TimerCallback(this.OnMinuteTick), (object) null, TimeSpan.FromSeconds((double) (60 - DateTime.Now.Second)), TimeSpan.FromMinutes(1.0));
    }

    private void OnMinuteTick(object arg)
    {
      EventHandler<ClockTickEventArgs> minuteEventHandler = this._minuteEventHandler;
      if (minuteEventHandler == null)
        return;
      minuteEventHandler((object) this, new ClockTickEventArgs(DateTime.Now, ClockTickType.Normal));
    }

    private void AppOnSuspending(object sender, SuspendingEventArgs suspendingEventArgs)
    {
      this._minuteTimer.Change(-1, -1);
      this._minuteTimer.Dispose();
    }

    private void AppOnResuming(object sender, object e)
    {
      this.InitTimer();
      EventHandler<ClockTickEventArgs> minuteEventHandler = this._minuteEventHandler;
      if (minuteEventHandler == null)
        return;
      minuteEventHandler((object) this, new ClockTickEventArgs(DateTime.Now, ClockTickType.Delayed));
    }
  }
}
