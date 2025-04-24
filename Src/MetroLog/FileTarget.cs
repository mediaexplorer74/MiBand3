
// Type: MetroLog.FileTarget
// Assembly: MetroLog, Version=0.0.0.0, Culture=neutral, PublicKeyToken=ba4ace74c3b410f3
// MVID: 4C1C3979-5A8D-451B-A09E-C87C946DE5D1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MetroLog.dll

using MetroLog.Layouts;
using MetroLog.Targets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Storage;

#nullable disable
namespace MetroLog
{
  public abstract class FileTarget(Layout layout, string logFolderName = "MetroLogs") : 
    FileTargetBase(layout, logFolderName)
  {
    private StorageFolder logFolder;

    public async Task<StorageFolder> EnsureInitializedAsync()
    {
      if (this.logFolder == null)
      {
        StorageFolder localFolder = ApplicationData.Current.LocalFolder;
        FileTarget fileTarget = this;
        StorageFolder logFolder = fileTarget.logFolder;
        string logFolderName = this.LogFolderName;
        StorageFolder folderAsync = await localFolder.CreateFolderAsync(logFolderName, (CreationCollisionOption) 3);
        fileTarget.logFolder = folderAsync;
        fileTarget = (FileTarget) null;
      }
      return this.logFolder;
    }

    protected override async Task<Stream> GetCompressedLogsInternal()
    {
      StorageFolder storageFolder = await this.EnsureInitializedAsync();
      MemoryStream ms = new MemoryStream();
      await ZipFile.CreateFromDirectory((IStorageFolder) this.logFolder, (Stream) ms);
      ms.Position = 0L;
      return (Stream) ms;
    }

    protected override Task EnsureInitialized() => (Task) this.EnsureInitializedAsync();

    protected override sealed async Task DoCleanup(Regex pattern, DateTime threshold)
    {
      List<StorageFile> toDelete = (await this.logFolder.GetFilesAsync()).Where<StorageFile>((Func<StorageFile, bool>) (file => pattern.Match(file.Name).Success && file.DateCreated <= (DateTimeOffset) threshold)).ToList<StorageFile>();
      Regex zipPattern = new Regex("^Log(.*).zip$");
      List<StorageFile> storageFileList = toDelete;
      IReadOnlyList<StorageFile> filesAsync = await ApplicationData.Current.TemporaryFolder.GetFilesAsync();
      Func<StorageFile, bool> predicate = (Func<StorageFile, bool>) (file => zipPattern.Match(file.Name).Success);
      storageFileList.AddRange(filesAsync.Where<StorageFile>(predicate));
      storageFileList = (List<StorageFile>) null;
      foreach (StorageFile file in toDelete)
      {
        try
        {
          await file.DeleteAsync();
        }
        catch (Exception ex)
        {
          InternalLogger.Current.Warn(string.Format("Failed to delete '{0}'.", (object) file.Path), ex);
        }
      }
    }

    protected override sealed async Task<LogWriteOperation> DoWriteAsync(
      StreamWriter streamWriter,
      string contents,
      LogEventInfo entry)
    {
      await this.WriteTextToFileCore(streamWriter, contents).ConfigureAwait(false);
      return new LogWriteOperation((Target) this, entry, true);
    }

    protected abstract Task WriteTextToFileCore(StreamWriter stream, string contents);

    protected override async Task<Stream> GetWritableStreamForFile(string fileName)
    {
      Stream writableStreamForFile = await ((IStorageFile) await this.logFolder.CreateFileAsync(fileName, this.FileNamingParameters.CreationMode == FileCreationMode.AppendIfExisting ? (CreationCollisionOption) 3 : (CreationCollisionOption) 1)).OpenStreamForWriteAsync();
      if (this.FileNamingParameters.CreationMode == FileCreationMode.AppendIfExisting)
        writableStreamForFile.Seek(0L, SeekOrigin.End);
      return writableStreamForFile;
    }
  }
}
