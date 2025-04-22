// Decompiled with JetBrains decompiler
// Type: Microsoft.Live.Platform
// Assembly: Microsoft.Live, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 84D9FE74-951C-4BB4-80F8-EDD369EAC36D
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\Microsoft.Live.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using Windows.ApplicationModel.Resources.Core;

#nullable disable
namespace Microsoft.Live
{
  internal static class Platform
  {
    private const string OSVersion = "8";

    public static DisplayType GetDisplayType() => DisplayType.WinDesktop;

    public static ThemeType GetThemeType() => ThemeType.Win8;

    public static ResponseType GetResponseType() => ResponseType.Token;

    public static string GetCurrentUICultureString()
    {
      return ((IReadOnlyList<string>) ResourceContext.GetForCurrentView().Languages)[0];
    }

    public static void ReportProgress(object handler, LiveOperationProgress progress)
    {
      if (!(handler is IProgress<LiveOperationProgress> progress1))
        return;
      progress1.Report(progress);
    }

    public static string GetLibraryHeaderValue()
    {
      string[] strArray = typeof (LiveAuthClient).AssemblyQualifiedName.Split(',')[2].Split('=')[1].Split('.');
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Windows/XAML{0}_{1}.{2}", (object) "8", (object) strArray[0], (object) strArray[1]);
    }
  }
}
