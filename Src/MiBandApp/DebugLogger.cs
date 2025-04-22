// Decompiled with JetBrains decompiler
// Type: MiBandApp.DebugLogger
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.exe

using Caliburn.Micro;
using System;

#nullable disable
namespace MiBandApp
{
  internal class DebugLogger : ILog
  {
    private readonly Type _type;

    public DebugLogger(Type type) => this._type = type;

    private string CreateLogMessage(string format, params object[] args)
    {
      return string.Format("[{0}] {1}", (object) DateTime.Now.ToString("o"), (object) string.Format(format, args));
    }

    public void Error(Exception exception)
    {
    }

    public void Info(string format, params object[] args)
    {
    }

    public void Warn(string format, params object[] args)
    {
    }
  }
}
