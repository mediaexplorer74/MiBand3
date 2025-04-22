// Decompiled with JetBrains decompiler
// Type: MiBandApp.ViewModels.WhatsNewPageViewModel
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.exe

using Caliburn.Micro;
using MiBandApp.Services;
using MiBandApp.Tools;
using System;
using System.Collections.Generic;
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
      this._updates = updatesHistoryService.GetNotShowedUpdates().OrderBy<UpdateHistoryItem, int>((Func<UpdateHistoryItem, int>) (t => t.Id)).ToList<UpdateHistoryItem>();
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
