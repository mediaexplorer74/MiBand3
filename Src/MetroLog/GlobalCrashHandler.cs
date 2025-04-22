// Decompiled with JetBrains decompiler
// Type: MetroLog.GlobalCrashHandler
// Assembly: MetroLog, Version=0.0.0.0, Culture=neutral, PublicKeyToken=ba4ace74c3b410f3
// MVID: 4C1C3979-5A8D-451B-A09E-C87C946DE5D1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MetroLog.dll

using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;

#nullable disable
namespace MetroLog
{
  public static class GlobalCrashHandler
  {
    public static void Configure()
    {
      Application current = Application.Current;
      WindowsRuntimeMarshal.AddEventHandler<UnhandledExceptionEventHandler>(new Func<UnhandledExceptionEventHandler, EventRegistrationToken>(current.add_UnhandledException), new Action<EventRegistrationToken>(current.remove_UnhandledException), new UnhandledExceptionEventHandler(GlobalCrashHandler.App_UnhandledException));
    }

    private static async void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
      WindowsRuntimeMarshal.RemoveEventHandler<UnhandledExceptionEventHandler>(new Action<EventRegistrationToken>(Application.Current.remove_UnhandledException), new UnhandledExceptionEventHandler(GlobalCrashHandler.App_UnhandledException));
      e.put_Handled(true);
      LogWriteOperation[] logWriteOperationArray = await ((ILoggerAsync) LogManagerFactory.DefaultLogManager.GetLogger<Application>()).FatalAsync("The application crashed: " + e.Message, (object) e);
      await LazyFlushManager.FlushAllAsync(new LogWriteContext());
      Application.Current.Exit();
    }
  }
}
