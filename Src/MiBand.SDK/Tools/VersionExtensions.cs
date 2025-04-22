// Decompiled with JetBrains decompiler
// Type: MiBand.SDK.Tools.VersionExtensions
// Assembly: MiBand.SDK, Version=0.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 152FA565-3249-4054-A361-01D9C0AFA7F1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBand.SDK.dll

using System;

#nullable disable
namespace MiBand.SDK.Tools
{
  internal static class VersionExtensions
  {
    public static int ToInt(this Version version)
    {
      return (version.Major << 24) + (version.Minor << 16) + (version.Build << 8) + version.Revision;
    }
  }
}
