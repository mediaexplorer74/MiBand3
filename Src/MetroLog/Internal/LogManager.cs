
// Type: MetroLog.Internal.LogManager
// Assembly: MetroLog, Version=0.0.0.0, Culture=neutral, PublicKeyToken=ba4ace74c3b410f3
// MVID: 4C1C3979-5A8D-451B-A09E-C87C946DE5D1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MetroLog.dll

using MetroLog.Targets;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;

#nullable disable
namespace MetroLog.Internal
{
  internal class LogManager : IWinRTLogManager, ILogManager
  {
    private readonly Dictionary<string, Logger> _loggers;
    private readonly object _loggersLock = new object();
    internal const string DateTimeFormat = "o";

    public async Task<IStorageFile> GetCompressedLogFile()
    {
      Stream stream = await this.GetCompressedLogs();
      if (stream == null)
        return (IStorageFile) null;
      StorageFile file = await ApplicationData.Current.TemporaryFolder.CreateFileAsync(string.Format("Log - {0}.zip", (object) DateTime.UtcNow.ToString("yyyy-MM-dd HHmmss", (IFormatProvider) CultureInfo.InvariantCulture)), (CreationCollisionOption) 1);
      using (Stream ras = ((IOutputStream) await file.OpenAsync((FileAccessMode) 1)).AsStreamForWrite())
        await stream.CopyToAsync(ras);
      stream.Dispose();
      return (IStorageFile) file;
    }

        public Task ShareLogFile(string title, string description)
        {
            var tcs = new TaskCompletionSource<object>();
            var dtm = DataTransferManager.GetForCurrentView();

            TypedEventHandler<DataTransferManager, DataRequestedEventArgs> handler = (sender, args) =>
            {
                var request = args.Request;
                request.Data.Properties.Title = title;
                request.Data.Properties.Description = description;
                request.Data.SetText("Log file sharing is not implemented yet."); // Placeholder for actual log file sharing logic
                tcs.SetResult(null);
            };

            dtm.DataRequested += handler;

            try
            {
                DataTransferManager.ShowShareUI();
            }
            finally
            {
                dtm.DataRequested -= handler;
            }

            return tcs.Task;
        }

    public LoggingConfiguration DefaultConfiguration { get; private set; }

    public event EventHandler<LoggerEventArgs> LoggerCreated;

    public Task<Stream> GetCompressedLogs()
    {
      FileTargetBase fileTargetBase = this.DefaultConfiguration.GetTargets().OfType<FileTargetBase>().FirstOrDefault<FileTargetBase>();
      return fileTargetBase != null ? fileTargetBase.GetCompressedLogs() : Task.FromResult<Stream>((Stream) null);
    }

    public LogManager(LoggingConfiguration configuration)
    {
      if (configuration == null)
        throw new ArgumentNullException(nameof (configuration));
      this._loggers = new Dictionary<string, Logger>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.DefaultConfiguration = configuration;
    }

    public ILogger GetLogger<T>(LoggingConfiguration config = null)
    {
      return this.GetLogger(typeof (T), config);
    }

    public ILogger GetLogger(Type type, LoggingConfiguration config = null)
    {
      if (type == null)
        throw new ArgumentNullException(nameof (type));
      return this.GetLogger(type.Name, config);
    }

    public ILogger GetLogger(string name, LoggingConfiguration config = null)
    {
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentNullException(nameof (name));
      lock (this._loggersLock)
      {
        if (!this._loggers.ContainsKey(name))
        {
          Logger logger = new Logger(name, config ?? this.DefaultConfiguration);
          InternalLogger.Current.Info("Created Logger '{0}'", (object) name);
          this.OnLoggerCreatedSafe(new LoggerEventArgs((ILogger) logger));
          this._loggers[name] = logger;
        }
        return (ILogger) this._loggers[name];
      }
    }

    private void OnLoggerCreatedSafe(LoggerEventArgs args)
    {
      try
      {
        this.OnLoggerCreated(args);
      }
      catch (Exception ex)
      {
        InternalLogger.Current.Error("Failed to handle OnLoggerCreated event.", ex);
      }
    }

    private void OnLoggerCreated(LoggerEventArgs args)
    {
      EventHandler<LoggerEventArgs> loggerCreated = this.LoggerCreated;
      if (loggerCreated == null)
        return;
      loggerCreated((object) this, args);
    }

    internal static DateTimeOffset GetDateTime() => DateTimeOffset.UtcNow;
  }
}
