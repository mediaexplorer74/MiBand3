// Decompiled with JetBrains decompiler
// Type: MiBandApp.Tools.SystemInfo
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.exe

using System;
using System.Reflection;

#nullable disable
namespace MiBandApp.Tools
{
  public static class SystemInfo
  {
    public static string SystemVersion { get; }

    static SystemInfo()
    {
      Type type1 = Type.GetType("Windows.System.Profile.AnalyticsInfo, Windows, ContentType=WindowsRuntime");
      Type type2 = Type.GetType("Windows.System.Profile.AnalyticsVersionInfo, Windows, ContentType=WindowsRuntime");
      if (type1 == null || type2 == null)
      {
        SystemInfo.SystemVersion = "Windows Phone 8.1";
      }
      else
      {
        object obj = type1.GetRuntimeProperty("VersionInfo").GetValue((object) null);
        long result;
        if (!long.TryParse(type2.GetRuntimeProperty("DeviceFamilyVersion").GetValue(obj).ToString(), out result))
          SystemInfo.SystemVersion = "Windows 10 Mobile";
        else
          SystemInfo.SystemVersion = "Windows 10 Mobile " + (object) new Version((int) (ushort) (result >> 48), (int) (ushort) (result >> 32), (int) (ushort) (result >> 16), (int) (ushort) result);
      }
    }
  }
}
