// Decompiled with JetBrains decompiler
// Type: MetroLog.Layouts.SingleLineLayout
// Assembly: MetroLog, Version=0.0.0.0, Culture=neutral, PublicKeyToken=ba4ace74c3b410f3
// MVID: 4C1C3979-5A8D-451B-A09E-C87C946DE5D1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MetroLog.dll

using System;
using System.Text;

#nullable disable
namespace MetroLog.Layouts
{
  public class SingleLineLayout : Layout
  {
    public override string GetFormattedString(LogWriteContext context, LogEventInfo info)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(info.SequenceID);
      stringBuilder.Append("|");
      stringBuilder.Append(info.TimeStamp.ToString("o"));
      stringBuilder.Append("|");
      stringBuilder.Append(info.Level.ToString().ToUpper());
      stringBuilder.Append("|");
      stringBuilder.Append(Environment.CurrentManagedThreadId);
      stringBuilder.Append("|");
      stringBuilder.Append(info.Logger);
      stringBuilder.Append("|");
      stringBuilder.Append(info.Message);
      if (info.Exception != null)
      {
        stringBuilder.Append(" --> ");
        stringBuilder.Append((object) info.Exception);
      }
      return stringBuilder.ToString();
    }
  }
}
