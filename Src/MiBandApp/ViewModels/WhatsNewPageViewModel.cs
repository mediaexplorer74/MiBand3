
// Type: MiBandApp.ViewModels.WhatsNewPageViewModel
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// 

using Caliburn.Micro;
using MiBandApp.Services;
using MiBandApp.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

#nullable disable
namespace MiBandApp.ViewModels
{
  public class WhatsNewPageViewModel : PageBaseViewModel
  {
    private readonly UpdatesHistoryService _updatesHistoryService;
    private readonly INavigationService _navigationService;
    private readonly List<UpdateHistoryItem> _updates;

    public WhatsNewPageViewModel(
      UpdatesHistoryService updatesHistoryService,
      INavigationService navigationService)
    {
      this._updatesHistoryService = updatesHistoryService;
      this._navigationService = navigationService;

        try
        {
            List<UpdateHistoryItem> historyitems = updatesHistoryService.GetNotShowedUpdates();
            IOrderedEnumerable<UpdateHistoryItem> b = historyitems.OrderBy<UpdateHistoryItem, int>(t => t.Id);
            this._updates = b.ToList<UpdateHistoryItem>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine("[ex] WhatsNewPage error: " + ex.Message);
        }
      if (!this._updates.Any<UpdateHistoryItem>())
        return;
      this.LastMessage = this._updates.Last<UpdateHistoryItem>().Message;
    }

    public List<UpdateHistoryItem> Updates => this._updates;

    public string LastMessage { get; }

    public void Skip() => this._navigationService.GoBack();

    protected override async Task OnDeactivate(bool close = true)
    {
      this._updatesHistoryService.MarkAsSeen();
    }
  }
}
