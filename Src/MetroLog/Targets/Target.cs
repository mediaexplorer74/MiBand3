﻿// Decompiled with JetBrains decompiler
// Type: MetroLog.Targets.Target
// Assembly: MetroLog, Version=0.0.0.0, Culture=neutral, PublicKeyToken=ba4ace74c3b410f3
// MVID: 4C1C3979-5A8D-451B-A09E-C87C946DE5D1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MetroLog.dll

using MetroLog.Layouts;
using System.Threading.Tasks;

#nullable disable
namespace MetroLog.Targets
{
  public abstract class Target
  {
    protected Layout Layout { get; private set; }

    protected Target(Layout layout) => this.Layout = layout;

    internal async Task<LogWriteOperation> WriteAsync(LogWriteContext context, LogEventInfo entry)
    {
      return await this.WriteAsyncCore(context, entry).ConfigureAwait(false);
    }

    protected abstract Task<LogWriteOperation> WriteAsyncCore(
      LogWriteContext context,
      LogEventInfo entry);
  }
}
