// Decompiled with JetBrains decompiler
// Type: MiBandApp.Tools.BandLogger
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.exe

using MetroLog;
using MiBand.SDK.Tools;
using System;

#nullable disable
namespace MiBandApp.Tools
{
  internal class BandLogger : ILog
  {
    private readonly ILogger _logger;

    public BandLogger(ILogManager logManager) => this._logger = logManager.GetLogger<BandLogger>();

    public void Debug(string message, string source = null)
    {
      this._logger.Debug(message, (Exception) null);
    }

    public void Info(string message, string source = null)
    {
      this._logger.Info(message, (Exception) null);
    }

    public void Warning(string message, string source = null)
    {
      this._logger.Warn(message, (Exception) null);
    }

    public void Error(string message, string source = null)
    {
      this._logger.Error(message, (Exception) null);
    }
  }
}
