// Decompiled with JetBrains decompiler
// Type: Microsoft.Live.SynchronizationContextWrapper
// Assembly: Microsoft.Live, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 84D9FE74-951C-4BB4-80F8-EDD369EAC36D
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\Microsoft.Live.dll

using System;
using System.Runtime.InteropServices;
using Windows.UI.Core;
using Windows.UI.Xaml;

#nullable disable
namespace Microsoft.Live
{
  internal class SynchronizationContextWrapper
  {
    private readonly CoreDispatcher syncContext;

    public SynchronizationContextWrapper(CoreDispatcher syncContext)
    {
      this.syncContext = syncContext;
    }

    public static SynchronizationContextWrapper Current
    {
      get
      {
        try
        {
          if (Window.Current != null)
          {
            if (Window.Current.Dispatcher != null)
              return new SynchronizationContextWrapper(Window.Current.Dispatcher);
          }
        }
        catch (COMException ex)
        {
        }
        return new SynchronizationContextWrapper((CoreDispatcher) null);
      }
    }

    public async void Post(Action callback)
    {
      DispatchedHandler dispatchedHandler = (DispatchedHandler) null;
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      SynchronizationContextWrapper.\u003C\u003Ec__DisplayClass2 cDisplayClass2 = new SynchronizationContextWrapper.\u003C\u003Ec__DisplayClass2();
      // ISSUE: reference to a compiler-generated field
      cDisplayClass2.callback = callback;
      if (this.syncContext != null)
      {
        CoreDispatcher syncContext = this.syncContext;
        if (dispatchedHandler == null)
        {
          // ISSUE: method pointer
          dispatchedHandler = new DispatchedHandler((object) cDisplayClass2, __methodptr(\u003CPost\u003Eb__0));
        }
        DispatchedHandler dispatchedHandler1 = dispatchedHandler;
        await syncContext.RunAsync((CoreDispatcherPriority) 0, dispatchedHandler1);
      }
      else
      {
        // ISSUE: reference to a compiler-generated field
        cDisplayClass2.callback();
      }
    }
  }
}
