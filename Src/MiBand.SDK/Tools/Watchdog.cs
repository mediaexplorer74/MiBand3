
// Type: MiBand.SDK.Tools.Watchdog
// Assembly: MiBand.SDK, Version=0.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 152FA565-3249-4054-A361-01D9C0AFA7F1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBand.SDK.dll

using System;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace MiBand.SDK.Tools
{
  internal class Watchdog
  {
    private readonly TimeSpan _interval;
    private readonly Timer _watchDogTimer;
    private bool _isRunning;
    private TaskCompletionSource<bool> _taskCompletionSource;

    public Watchdog(TimeSpan interval)
    {
      this._interval = interval;
      this._watchDogTimer = new Timer(new TimerCallback(this.OnWatchdogTick), (object) null, -1, -1);
    }

    public event EventHandler Elapsed = (_param1, _param2) => { };

    public bool HasElapsed { get; private set; }

    public Task Task
    {
      get
      {
        if (this._taskCompletionSource == null)
          this._taskCompletionSource = new TaskCompletionSource<bool>();
        return (Task) this._taskCompletionSource.Task;
      }
    }

    public void Enable()
    {
      lock (this._watchDogTimer)
      {
        if (this._isRunning)
          return;
        this._isRunning = true;
        this.Reset();
      }
    }

    public void Reset()
    {
      lock (this._watchDogTimer)
      {
        if (!this._isRunning)
          return;
        this._watchDogTimer.Change(this._interval, Timeout.InfiniteTimeSpan);
      }
    }

    public void Disable()
    {
      lock (this._watchDogTimer)
      {
        this._isRunning = false;
        this._watchDogTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
      }
    }

    private void OnWatchdogTick(object state)
    {
      this.HasElapsed = true;
      this.Elapsed((object) this, EventArgs.Empty);
      this._taskCompletionSource?.TrySetResult(true);
    }
  }
}
