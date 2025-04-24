
// Type: MiBandApp.Tools.WindowsVersion
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// 

using System;

#nullable disable
namespace MiBandApp.Tools
{
  public static class WindowsVersion
  {
    private static bool _isWindows10;
    private static bool _initialized;

    public static bool IsWindows10
    {
      get
      {
        WindowsVersion.TryInitialize();
        return WindowsVersion._isWindows10;
      }
    }

    private static void TryInitialize()
    {
      if (WindowsVersion._initialized)
        return;
      WindowsVersion._initialized = true;
      Type type1 = Type.GetType("Windows.System.Profile.AnalyticsInfo, Windows, ContentType=WindowsRuntime");
      Type type2 = Type.GetType("Windows.System.Profile.AnalyticsVersionInfo, Windows, ContentType=WindowsRuntime");
      if (type1 == null || type2 == null)
        WindowsVersion._isWindows10 = false;
      else
        WindowsVersion._isWindows10 = true;
    }
  }
}
