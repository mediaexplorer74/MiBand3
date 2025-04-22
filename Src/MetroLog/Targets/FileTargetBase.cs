// Decompiled with JetBrains decompiler
// Type: MetroLog.Targets.FileTargetBase
// Assembly: MetroLog, Version=0.0.0.0, Culture=neutral, PublicKeyToken=ba4ace74c3b410f3
// MVID: 4C1C3979-5A8D-451B-A09E-C87C946DE5D1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MetroLog.dll

using MetroLog.Internal;
using MetroLog.Layouts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

#nullable disable
namespace MetroLog.Targets
{
  public abstract class FileTargetBase : AsyncTarget
  {
    protected readonly string LogFolderName;
    public const string DefaultLogFolderName = "MetroLogs";
    private readonly AsyncLock _lock = new AsyncLock();
    private readonly Dictionary<string, StreamWriter> openStreamWriters = new Dictionary<string, StreamWriter>();

    public FileNamingParameters FileNamingParameters { get; private set; }

    public int RetainDays { get; set; }

    public bool KeepLogFilesOpenForWrite { get; set; }

    protected DateTime NextCleanupUtc { get; set; }

    protected FileTargetBase(Layout layout, string logFolderName = "MetroLogs")
      : base(layout)
    {
      this.FileNamingParameters = new FileNamingParameters();
      this.RetainDays = 30;
      this.KeepLogFilesOpenForWrite = true;
      this.LogFolderName = logFolderName;
    }

    protected abstract Task EnsureInitialized();

    protected abstract Task DoCleanup(Regex pattern, DateTime threshold);

    protected abstract Task<Stream> GetCompressedLogsInternal();

    internal async Task<Stream> GetCompressedLogs()
    {
      Stream compressedLogs;
      using (await this._lock.LockAsync().ConfigureAwait(false))
      {
        this.CloseAllOpenStreamsInternal();
        compressedLogs = await this.GetCompressedLogsInternal().ConfigureAwait(false);
      }
      return compressedLogs;
    }

    internal async Task ForceCleanupAsync()
    {
      DateTime threshold = DateTime.UtcNow.AddDays((double) -this.RetainDays);
      await this.DoCleanup(this.FileNamingParameters.GetRegex(), threshold);
    }

    private async Task CheckCleanupAsync()
    {
      DateTime utcNow = DateTime.UtcNow;
      if (utcNow < this.NextCleanupUtc)
        return;
      if (this.RetainDays < 1)
        return;
      try
      {
        DateTime threshold = utcNow.AddDays((double) -this.RetainDays);
        await this.DoCleanup(this.FileNamingParameters.GetRegex(), threshold);
      }
      finally
      {
        this.NextCleanupUtc = DateTime.UtcNow.AddHours(1.0);
      }
    }

    protected override sealed async Task<LogWriteOperation> WriteAsyncCore(
      LogWriteContext context,
      LogEventInfo entry)
    {
      string contents = this.Layout.GetFormattedString(context, entry);
      LogWriteOperation logWriteOperation1;
      using (await this._lock.LockAsync().ConfigureAwait(false))
      {
        ConfiguredTaskAwaitable configuredTaskAwaitable = this.EnsureInitialized().ConfigureAwait(false);
        await configuredTaskAwaitable;
        configuredTaskAwaitable = this.CheckCleanupAsync().ConfigureAwait(false);
        await configuredTaskAwaitable;
        StreamWriter sw = await this.GetOrCreateStreamWriterForFile(this.FileNamingParameters.GetFilename(context, entry)).ConfigureAwait(false);
        LogWriteOperation logWriteOperation2 = await this.DoWriteAsync(sw, contents, entry);
        if (!this.KeepLogFilesOpenForWrite)
          sw.Dispose();
        logWriteOperation1 = logWriteOperation2;
      }
      return logWriteOperation1;
    }

    protected abstract Task<LogWriteOperation> DoWriteAsync(
      StreamWriter fileName,
      string contents,
      LogEventInfo entry);

    private async Task<StreamWriter> GetOrCreateStreamWriterForFile(string fileName)
    {
      StreamWriter streamWriterForFile = (StreamWriter) null;
      if (this.KeepLogFilesOpenForWrite && !this.openStreamWriters.TryGetValue(fileName, out streamWriterForFile))
      {
        streamWriterForFile = new StreamWriter(await this.GetWritableStreamForFile(fileName).ConfigureAwait(false))
        {
          AutoFlush = true
        };
        this.openStreamWriters.Add(fileName, streamWriterForFile);
      }
      else if (streamWriterForFile == null)
        streamWriterForFile = new StreamWriter(await this.GetWritableStreamForFile(fileName).ConfigureAwait(false))
        {
          AutoFlush = true
        };
      return streamWriterForFile;
    }

    protected abstract Task<Stream> GetWritableStreamForFile(string fileName);

    private void CloseAllOpenStreamsInternal()
    {
      foreach (StreamWriter streamWriter in this.openStreamWriters.Values)
      {
        streamWriter.Flush();
        streamWriter.Dispose();
      }
      this.openStreamWriters.Clear();
    }

    public async Task CloseAllOpenFiles()
    {
      using (await this._lock.LockAsync().ConfigureAwait(false))
        this.CloseAllOpenStreamsInternal();
    }
  }
}
