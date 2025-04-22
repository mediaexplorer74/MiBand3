﻿// Decompiled with JetBrains decompiler
// Type: MetroLog.LazyFlushManager
// Assembly: MetroLog, Version=0.0.0.0, Culture=neutral, PublicKeyToken=ba4ace74c3b410f3
// MVID: 4C1C3979-5A8D-451B-A09E-C87C946DE5D1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MetroLog.dll

using MetroLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.System.Threading;
using Windows.UI.Xaml;

#nullable disable
namespace MetroLog
{
  public class LazyFlushManager
  {
    private object _lock = new object();

    private ILogManager Owner { get; set; }

    private List<ILazyFlushable> Clients { get; set; }

    private ThreadPoolTimer Timer { get; set; }

    private static Dictionary<ILogManager, LazyFlushManager> Owners { get; set; }

    private LazyFlushManager(ILogManager owner)
    {
      this.Owner = owner;
      this.Owner.LoggerCreated += new EventHandler<LoggerEventArgs>(this.Owner_LoggerCreated);
      this.Clients = new List<ILazyFlushable>();
      // ISSUE: method pointer
      this.Timer = ThreadPoolTimer.CreatePeriodicTimer(new TimerElapsedHandler((object) this, __methodptr(\u003C\u002Ector\u003Eb__17_0)), TimeSpan.FromMinutes(2.0));
    }

    private void Owner_LoggerCreated(object sender, LoggerEventArgs e)
    {
      lock (this._lock)
      {
        foreach (Target target in ((ILoggerQuery) e.Logger).GetTargets())
        {
          if (target is ILazyFlushable && !this.Clients.Contains((ILazyFlushable) target))
            this.Clients.Add((ILazyFlushable) target);
        }
      }
    }

    static LazyFlushManager()
    {
      LazyFlushManager.Owners = new Dictionary<ILogManager, LazyFlushManager>();
      if (LoggingEnvironment.XamlApplicationState != XamlApplicationState.Available)
        return;
      Application current = Application.Current;
      WindowsRuntimeMarshal.AddEventHandler<SuspendingEventHandler>(new Func<SuspendingEventHandler, EventRegistrationToken>(current.add_Suspending), new Action<EventRegistrationToken>(current.remove_Suspending), new SuspendingEventHandler(LazyFlushManager.Current_Suspending));
    }

    private static async void Current_Suspending(object sender, SuspendingEventArgs e)
    {
      await LazyFlushManager.FlushAllAsync(new LogWriteContext());
    }

    public static async Task FlushAllAsync(LogWriteContext context)
    {
      List<Task> taskList = new List<Task>();
      foreach (LazyFlushManager lazyFlushManager in LazyFlushManager.Owners.Values)
        taskList.Add(lazyFlushManager.LazyFlushAsync(context));
      await Task.WhenAll((IEnumerable<Task>) taskList);
    }

    private async Task LazyFlushAsync(LogWriteContext context)
    {
      List<ILazyFlushable> source = (List<ILazyFlushable>) null;
      lock (this._lock)
        source = new List<ILazyFlushable>((IEnumerable<ILazyFlushable>) this.Clients);
      if (!source.Any<ILazyFlushable>())
        return;
      await Task.WhenAll((IEnumerable<Task>) source.Select<ILazyFlushable, Task>((Func<ILazyFlushable, Task>) (client => client.LazyFlushAsync(context))).ToList<Task>());
    }

    internal static void Initialize(ILogManager manager)
    {
      LazyFlushManager.Owners[manager] = new LazyFlushManager(manager);
    }
  }
}
