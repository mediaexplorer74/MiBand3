
// Type: MetroLog.Internal.AsyncLock
// Assembly: MetroLog, Version=0.0.0.0, Culture=neutral, PublicKeyToken=ba4ace74c3b410f3
// MVID: 4C1C3979-5A8D-451B-A09E-C87C946DE5D1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MetroLog.dll

using System;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace MetroLog.Internal
{
  internal class AsyncLock
  {
    private readonly SemaphoreSlim m_semaphore;
    private readonly Task<AsyncLock.Releaser> m_releaser;

    public AsyncLock()
    {
      this.m_semaphore = new SemaphoreSlim(1);
      this.m_releaser = Task.FromResult<AsyncLock.Releaser>(new AsyncLock.Releaser(this));
    }

    public Task<AsyncLock.Releaser> LockAsync()
    {
      Task task = this.m_semaphore.WaitAsync();
      return !task.IsCompleted ? task.ContinueWith<AsyncLock.Releaser>((Func<Task, object, AsyncLock.Releaser>) ((_, state) => new AsyncLock.Releaser((AsyncLock) state)), (object) this, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default) : this.m_releaser;
    }

    public struct Releaser : IDisposable
    {
      private readonly AsyncLock m_toRelease;

      internal Releaser(AsyncLock toRelease) => this.m_toRelease = toRelease;

      public void Dispose() => this.m_toRelease?.m_semaphore.Release();
    }
  }
}
