
// Type: MiBandApp.Services.DiagnosticsService
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// 

using MetroLog;
using MetroLog.Layouts;
using MetroLog.Targets;
using MiBand.SDK.Core;
using MiBand.SDK.Data;
using MiBandApp.Tools;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

#nullable disable
namespace MiBandApp.Services
{
  public class DiagnosticsService
  {
    private readonly MiBandApp.Storage.Settings.Settings _settings;
    private readonly ILogger _log;
    private readonly ILogger _dumpLogger;
    private readonly ILogManager _dumpLogManager;

    public DiagnosticsService(ILogManager logManager, MiBandApp.Storage.Settings.Settings settings)
    {
      this._settings = settings;
      this._log = logManager.GetLogger<DiagnosticsService>();
      LoggingConfiguration config = new LoggingConfiguration()
      {
        IsEnabled = true
      };
      LoggingConfiguration loggingConfiguration = config;
      SnapshotFileTarget snapshotFileTarget = new SnapshotFileTarget((Layout) new PlainLoggingLayout(), "MetroLogDumps");
      snapshotFileTarget.KeepLogFilesOpenForWrite = false;
      snapshotFileTarget.RetainDays = 7;
      snapshotFileTarget.FileNamingParameters.CreationMode = FileCreationMode.AppendIfExisting;
      snapshotFileTarget.FileNamingParameters.IncludeLevel = true;
      snapshotFileTarget.FileNamingParameters.IncludeLogger = true;
      snapshotFileTarget.FileNamingParameters.IncludeSequence = true;
      snapshotFileTarget.FileNamingParameters.IncludeSession = false;
      snapshotFileTarget.FileNamingParameters.IncludeTimestamp = FileTimestampMode.DateTime;
      loggingConfiguration.AddTarget(LogLevel.Trace, LogLevel.Fatal, (Target) snapshotFileTarget);
      this._dumpLogManager = LogManagerFactory.CreateLogManager(config);
      this._dumpLogger = this._dumpLogManager.GetLogger<DiagnosticsService>();
    }

    public void SaveData(
      List<RawMinuteActivityDataSeries> rawActivityDataSeriesList,
      BandDeviceInfo deviceInfo)
    {
      try
      {
        foreach (RawMinuteActivityDataSeries activityDataSeries in rawActivityDataSeriesList)
        {
          if (activityDataSeries.Data.Count != 0)
          {
            StringBuilder stringBuilder1 = new StringBuilder();
            stringBuilder1.AppendLine(string.Format("RA_{0}_{1}_{2}.txt", (object) deviceInfo.Model, (object) deviceInfo.FirmwareVersion, (object) this._settings.GetUserId()));
            stringBuilder1.AppendLine(string.Format("StartTime: {0}", (object) activityDataSeries.StartTime.DateTime.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
            StringBuilder stringBuilder2 = stringBuilder1;
            object[] objArray = new object[1];
            DateTime dateTime = activityDataSeries.StartTime.DateTime;
            dateTime = dateTime.AddMinutes((double) activityDataSeries.Data.Count);
            objArray[0] = (object) dateTime.ToString((IFormatProvider) CultureInfo.InvariantCulture);
            string str = string.Format("EndTime: {0}", objArray);
            stringBuilder2.AppendLine(str);
            stringBuilder1.AppendLine(string.Format("LengthMin: {0}", (object) activityDataSeries.Data.Count));
            stringBuilder1.Append(string.Join(Environment.NewLine, activityDataSeries.Data.Select<RawMinuteActivityData, string>((Func<RawMinuteActivityData, string>) (t => string.Join("\t", (object) t.Mode, (object) t.Activity, (object) t.Steps, (object) t.HeartRate)))));
            this._dumpLogger.Info(stringBuilder1.ToString(), (Exception) null);
          }
        }
      }
      catch (Exception ex)
      {
        this._log.Warn(ex.Message, (Exception) null);
      }
    }

    public async Task<IStorageFile> GetDiagnosticFile()
    {
      return await ((IWinRTLogManager) this._dumpLogManager).GetCompressedLogFile().ConfigureAwait(false);
    }
  }
}
