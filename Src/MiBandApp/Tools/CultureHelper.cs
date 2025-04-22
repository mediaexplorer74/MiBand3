// Decompiled with JetBrains decompiler
// Type: MiBandApp.Tools.CultureHelper
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.exe

using MiBand.SDK.Configuration;
using System;
using System.Globalization;

#nullable disable
namespace MiBandApp.Tools
{
  public static class CultureHelper
  {
    public static DateDisplayMode GetDateDisplayMode()
    {
      CultureInfo regionalFormatCulture = App.RegionalFormatCulture;
      if (regionalFormatCulture == null || !CultureHelper.CultureHasNameOrDerivedFrom(regionalFormatCulture, "ZH"))
        return DateDisplayMode.English;
      return CultureHelper.CultureHasNameOrDerivedFrom(regionalFormatCulture, "zh-Hant") || CultureHelper.CultureHasNameOrDerivedFrom(regionalFormatCulture, "zh-CHT") ? DateDisplayMode.TraditionalChinese : DateDisplayMode.SimplifiedChinese;
    }

    public static bool CultureHasNameOrDerivedFrom(CultureInfo cultureInfo, string name)
    {
      if (string.IsNullOrEmpty(cultureInfo?.Name))
        return false;
      return cultureInfo.Name.Equals(name, StringComparison.OrdinalIgnoreCase) || CultureHelper.CultureHasNameOrDerivedFrom(cultureInfo.Parent, name);
    }
  }
}
