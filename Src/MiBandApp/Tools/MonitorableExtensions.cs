// Decompiled with JetBrains decompiler
// Type: MiBandApp.Tools.MonitorableExtensions
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.exe

using System;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace MiBandApp.Tools
{
  public static class MonitorableExtensions
  {
    public static Task<T> WaitForValue<T>(
      this Monitorable<T> monitorable,
      T value,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TaskCompletionSource<T> tcs = new TaskCompletionSource<T>();
      EventHandler<MonitorableUpdatedEventArgs<T>> updatedDelegate = (EventHandler<MonitorableUpdatedEventArgs<T>>) ((sender, args) =>
      {
        if (!args.UpdatedValue.Equals((object) (T) value))
          return;
        tcs.TrySetResult(value);
      });
      cancellationToken.Register((Action) (() => tcs.TrySetCanceled()));
      monitorable.Updated += updatedDelegate;
      if (monitorable.Value.Equals((object) value))
        tcs.TrySetResult(value);
      tcs.Task.ContinueWith((Action<Task<T>>) (t => monitorable.Updated -= updatedDelegate)).NoAwait();
      return tcs.Task;
    }
  }
}
