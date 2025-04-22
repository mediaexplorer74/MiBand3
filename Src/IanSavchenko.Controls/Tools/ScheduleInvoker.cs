// Decompiled with JetBrains decompiler
// Type: IanSavchenko.Controls.Tools.ScheduleInvoker
// Assembly: IanSavchenko.Controls, Version=0.8.2.0, Culture=neutral, PublicKeyToken=null
// MVID: C384A7D9-D254-451C-A544-CD6C2993240A
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\IanSavchenko.Controls.dll

using System;
using System.Threading;
using Windows.UI.Core;

#nullable disable
namespace IanSavchenko.Controls.Tools
{
  internal class ScheduleInvoker
  {
    private readonly CoreDispatcher _dispatcher;
    private readonly Timer _timer;
    private Action _action;

    public ScheduleInvoker(CoreDispatcher dispatcher = null)
    {
      this._dispatcher = dispatcher;
      this._timer = new Timer(new System.Threading.TimerCallback(this.TimerCallback), (object) null, -1, -1);
    }

    public void Schedule(TimeSpan timeSpan, Action action)
    {
      this.Stop();
      this._action = action;
      this._timer.Change(timeSpan, Timeout.InfiniteTimeSpan);
    }

    public void Stop() => this._timer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);

    private void TimerCallback(object state)
    {
      if (this._dispatcher != null)
      {
        // ISSUE: method pointer
        this._dispatcher.RunAsync((CoreDispatcherPriority) 0, new DispatchedHandler((object) this, __methodptr(\u003CTimerCallback\u003Eb__6_0))).AsTask().ConfigureAwait(false);
      }
      else
        this._action();
    }
  }
}
