// Decompiled with JetBrains decompiler
// Type: MiBandApp.Tools.MessageDialogExtensions
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.exe

using System;
using System.Threading.Tasks;
using Windows.UI.Popups;

#nullable disable
namespace MiBandApp.Tools
{
  public static class MessageDialogExtensions
  {
    private static bool _showing;

    public static async Task<IUICommand> ShowAsyncSafe(this MessageDialog dialog)
    {
      if (MessageDialogExtensions._showing)
      {
        await Task.Delay(100);
        return (IUICommand) null;
      }
      try
      {
        MessageDialogExtensions._showing = true;
        return await dialog.ShowAsync().AsTask<IUICommand>().ConfigureAwait(true);
      }
      finally
      {
        MessageDialogExtensions._showing = false;
      }
    }
  }
}
