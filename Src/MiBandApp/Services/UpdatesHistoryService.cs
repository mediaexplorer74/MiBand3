
// Type: MiBandApp.Services.UpdatesHistoryService
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// 

using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Resources;
using Windows.Storage;

#nullable disable
namespace MiBandApp.Services
{
  public class UpdatesHistoryService
  {
    private const string SettingsShowedIdKey = "UpdatesHistoryShowedId";
    private readonly Dictionary<int, UpdateHistoryItem> _updates = new Dictionary<int, UpdateHistoryItem>();
    private readonly ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;
    private int _latestUpdateId;

    public UpdatesHistoryService()
    {
      this.PopulateHistory();
      if (this._localSettings.Values.ContainsKey("UpdatesHistoryShowedId"))
        return;
      this.LastShowedId = this._latestUpdateId - 1;
    }

    public bool HasNotShowedUpdates
    {
        get
        {
            return this._latestUpdateId > this.LastShowedId;
        }
    }

    private int LastShowedId
    {
      get
      {
        return (int) this._localSettings.Values["UpdatesHistoryShowedId"];
      }
      set
      {
        ((IDictionary<string, object>) this._localSettings.Values)["UpdatesHistoryShowedId"] = value;
      }
    }

    public List<UpdateHistoryItem> GetNotShowedUpdates()
    {
      return this._updates.Where<KeyValuePair<int, UpdateHistoryItem>>(
          (t => t.Key > this.LastShowedId)).Select<KeyValuePair<int, UpdateHistoryItem>,
          UpdateHistoryItem>((t => t.Value)).ToList<UpdateHistoryItem>();
    }

    public void MarkAsSeen()
    {
       this._localSettings.Values["UpdatesHistoryShowedId"]  =  this._latestUpdateId;
    }

    public List<UpdateHistoryItem> AllUpdates => this._updates.Values.ToList<UpdateHistoryItem>();

    private void PopulateHistory()
    {
      ResourceLoader viewIndependentUse = 
                ResourceLoader.GetForViewIndependentUse(
                    "Resources"//"UpdatesHistoryResources"
                    );
      int num = 0;
      while (true)
      {
        string version = viewIndependentUse.GetString("U" + (object) num + "Version");
        if (!string.IsNullOrEmpty(version))
        {
          string description = viewIndependentUse.GetString("U" + (object) num + "Description");
          string message = viewIndependentUse.GetString("U" + (object) num + "Message");
          this._updates.Add(num, new UpdateHistoryItem(num, version, description, message));
          ++num;
        }
        else
          break;
      }
      this._latestUpdateId = this._updates.Keys.Max();
    }
  }
}
