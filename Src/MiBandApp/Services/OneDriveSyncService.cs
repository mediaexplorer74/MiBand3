
// Type: MiBandApp.Services.OneDriveSyncService
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// 

using MetroLog;
using MiBandApp.Storage.DataBases;
using MiBandApp.Storage.Tables;
using MiBandApp.Tools;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Live;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

#nullable disable
namespace MiBandApp.Services
{
  public class OneDriveSyncService
  {
    private const string FolderName = "Bind Mi Band";
    private const string ActivityDbFileName = "Activity.db";
    private const string LastSyncTimeKey = "OneDriveLastSyncTime";
    private readonly string _logSourceName;
    private readonly OneDriveSessionService _oneDriveSessionService;
    private readonly BandSyncController _bandSyncController;
    private readonly MiBandApp.Storage.Settings.Settings _settings;
    private readonly DataManager _dataManager;
    private readonly DaySummaryDataBase _daySummaryDataBase;
    private readonly LicensingService _licensingService;
    private readonly ILogger _log;
    private OneDriveSyncService.SyncState _state;

    public OneDriveSyncService(
      OneDriveSessionService oneDriveSessionService,
      BandSyncController bandSyncController,
      MiBandApp.Storage.Settings.Settings settings,
      ILogManager logManager,
      DataManager dataManager,
      DaySummaryDataBase daySummaryDataBase,
      LicensingService licensingService)
    {
      this._oneDriveSessionService = oneDriveSessionService;
      this._bandSyncController = bandSyncController;
      this._settings = settings;
      this._log = logManager.GetLogger<OneDriveSyncService>();
      this._dataManager = dataManager;
      this._daySummaryDataBase = daySummaryDataBase;
      this._licensingService = licensingService;
      this._bandSyncController.SyncState.Updated += new EventHandler<MonitorableUpdatedEventArgs<BandSyncState>>(this.BandControllerOnSyncStateChanged);
      this._logSourceName = typeof (OneDriveSyncService).Name;
    }

    public bool IsEnabled => this._settings.OneDriveSyncEnabled;

    public OneDriveSyncService.SyncState State
    {
      get => this._state;
      set
      {
        if (this._state == value)
          return;
        this._state = value;
        if (this._state == OneDriveSyncService.SyncState.Synced)
          this.LastSyncTime = DateTime.Now;
        this.StateChanged((object) this, EventArgs.Empty);
      }
    }

    public DateTime LastSyncTime
    {
      get
      {
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        if (!((IDictionary<string, object>) localSettings.Values).ContainsKey("OneDriveLastSyncTime"))
          this.LastSyncTime = DateTime.MinValue;
        return DateTime.FromBinary((long) ((IDictionary<string, object>) localSettings.Values)["OneDriveLastSyncTime"]);
      }
      set
      {
        ((IDictionary<string, object>) ApplicationData.Current.LocalSettings.Values)["OneDriveLastSyncTime"] = (object) value.ToBinary();
      }
    }

    public event EventHandler StateChanged = (sender, args) => { };

    public async Task Sync()
    {
      if (!await this.CheckRunConditions().ConfigureAwait(false))
        return;
      this.State = OneDriveSyncService.SyncState.Connecting;
      LiveConnectClient client = new LiveConnectClient(this._oneDriveSessionService.Session);
      string oneDriveFolderId = await this.GetOneDriveFolderId(client).ConfigureAwait(false);
      if (oneDriveFolderId == null)
      {
        this.State = OneDriveSyncService.SyncState.Error;
      }
      else
      {
        this.State = OneDriveSyncService.SyncState.Importing;
        if (!await this.ImportAllData(client, oneDriveFolderId).ConfigureAwait(false))
        {
          this.State = OneDriveSyncService.SyncState.Error;
        }
        else
        {
          this.State = OneDriveSyncService.SyncState.Exporting;
          this.State = await this.ExportAllData(client, oneDriveFolderId).ConfigureAwait(false) ? OneDriveSyncService.SyncState.Synced : OneDriveSyncService.SyncState.Error;
        }
      }
    }

    private async Task<bool> CheckRunConditions()
    {
      if (!this.IsEnabled || this._bandSyncController.SyncState.Value != BandSyncState.Success || !this._licensingService.IsPro)
        return false;
      await this._oneDriveSessionService.Init().ConfigureAwait(false);
      if (this._oneDriveSessionService.Status != OneDriveSessionService.OneDriveStatus.Authorized)
        return false;
      this._log.Info("OneDrive sync started", (object) this._logSourceName);
      return true;
    }

        private async Task<string> GetOneDriveFolderId(LiveConnectClient liveConnectClient)
        {
            try
            {
                var result = (await liveConnectClient.GetAsync("me/skydrive/files").ConfigureAwait(false)).Result;

                // Ensure proper casting and avoid dynamic binding issues
                if (result is IDictionary<string, object> resultDict && resultDict.TryGetValue("data", out var dataObj) && dataObj is IEnumerable<object> data)
                {
                    foreach (var item in data)
                    {
                        if (item is IDictionary<string, object> itemDict &&
                            itemDict.TryGetValue("name", out var nameObj) &&
                            string.Equals(nameObj as string, FolderName, StringComparison.OrdinalIgnoreCase))
                        {
                            if (itemDict.TryGetValue("id", out var idObj))
                            {
                                return idObj as string;
                            }
                        }
                    }
                }

                // Create folder if it doesn't exist
                var folderCreationResult = await liveConnectClient.PostAsync("me/skydrive", new Dictionary<string, object>
                {
                    { "name", FolderName }
                }).ConfigureAwait(false);

                if (folderCreationResult.Result is IDictionary<string, object> creationResultDict &&
                    creationResultDict.TryGetValue("id", out var createdFolderId))
                {
                    return createdFolderId as string;
                }
            }
            catch (Exception ex)
            {
                _log.Error($"Couldn't create folder in OneDrive with name: {FolderName}. Exception: {ex}", _logSourceName);
            }

            return null;
        }

    private async Task<bool> ImportAllData(LiveConnectClient liveConnectClient, string folderId)
    {
      try
      {
        List<object> objectList = (await liveConnectClient.GetAsync(folderId + "/files").ConfigureAwait(false)).Result["data"] as List<object>;
        string str = (string) null;
        foreach (object obj in objectList)
        {
          IDictionary<string, object> dictionary = obj as IDictionary<string, object>;
          if (string.Compare(dictionary["name"].ToString(), "Activity.db", StringComparison.OrdinalIgnoreCase) == 0)
          {
            str = dictionary["id"].ToString();
            break;
          }
        }
        if (str == null)
        {
          this._log.Warn("File not found for import: Activity.db", (object) this._logSourceName);
          return true;
        }
        Stream responseStream = (await WebRequest.Create((string) (await liveConnectClient.GetAsync(str + "/content")).Result["location"]).GetResponseAsync().ConfigureAwait(false)).GetResponseStream();
        Encoding encoding = Encoding.GetEncoding("utf-8");
        DateTime lastDay = new DateTime();
        using (StreamReader readStream = new StreamReader(responseStream, encoding))
        {
          SerializableDataBase serializableDataBase = JsonConvert.DeserializeObject<SerializableDataBase>(await readStream.ReadToEndAsync().ConfigureAwait(false));
          this._dataManager.AddCloudActivityData(serializableDataBase.Days);
          if (serializableDataBase.Days.Count != 0)
            lastDay = serializableDataBase.Days.OrderByDescending<DaySummary, DateTime>((Func<DaySummary, DateTime>) (t => t.Date)).First<DaySummary>().Date;
        }
        this._log.Info("Importing successfull. Last day in OneDrive: " + (object) lastDay, (object) this._logSourceName);
        return true;
      }
      catch (Exception ex)
      {
        this._log.Error("Exception while importing: " + ex.ToString(), (object) this._logSourceName);
        return false;
      }
    }

    private async Task<bool> ExportAllData(LiveConnectClient client, string folderId)
    {
      try
      {
        this._log.Info("Export started", (object) this._logSourceName);
        string body = JsonConvert.SerializeObject((object) new SerializableDataBase()
        {
          Days = this._daySummaryDataBase.GetAllDays().ToList<DaySummary>()
        });
        this._log.Info("Exporting " + (object) body.Length + " symbols", (object) this._logSourceName);
        LiveOperationResult liveOperationResult = await client.PutAsync(folderId + "/files/Activity.db", body).ConfigureAwait(false);
        this._log.Info("Export successfull", (object) this._logSourceName);
        return true;
      }
      catch (Exception ex)
      {
        this._log.Error("Exception while exporting: " + (object) ex, (object) this._logSourceName);
        return false;
      }
    }

    private async void BandControllerOnSyncStateChanged(object sender, EventArgs eventArgs)
    {
      await this.Sync().ConfigureAwait(false);
    }

    public enum SyncState
    {
      NotSynced,
      Synced,
      Connecting,
      Importing,
      Exporting,
      Error,
    }
  }
}
