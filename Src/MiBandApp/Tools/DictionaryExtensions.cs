// Decompiled with JetBrains decompiler
// Type: MiBandApp.Tools.DictionaryExtensions
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.exe

using System.Collections.Generic;

#nullable disable
namespace MiBandApp.Tools
{
  public static class DictionaryExtensions
  {
    public static TReturnValue GetValueOrDefault<TReturnValue>(
      this IDictionary<string, object> @this,
      string key)
    {
      return @this.ContainsKey(key) ? (TReturnValue) @this[key] : default (TReturnValue);
    }
  }
}
