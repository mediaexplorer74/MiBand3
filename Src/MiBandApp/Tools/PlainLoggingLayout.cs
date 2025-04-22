// Decompiled with JetBrains decompiler
// Type: MiBandApp.Tools.PlainLoggingLayout
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.exe

using MetroLog;
using MetroLog.Layouts;

#nullable disable
namespace MiBandApp.Tools
{
  public class PlainLoggingLayout : Layout
  {
    public override string GetFormattedString(LogWriteContext context, LogEventInfo info)
    {
      return info.Message;
    }
  }
}
