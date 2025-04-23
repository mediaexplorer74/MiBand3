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
                return new SynchronizationContextWrapper((CoreDispatcher)null);
            }
        }

        public async void Post(Action callback)
        {
            if (this.syncContext != null)
            {
                await this.syncContext.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    callback?.Invoke();
                });
            }
            else
            {
                callback?.Invoke();
            }
        }
    }
}
