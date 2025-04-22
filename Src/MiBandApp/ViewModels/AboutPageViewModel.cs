// Decompiled with JetBrains decompiler
// Type: MiBandApp.ViewModels.AboutPageViewModel
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.exe

using MetroLog;
using MiBandApp.Services;
using MiBandApp.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.UI.Popups;

#nullable disable
namespace MiBandApp.ViewModels
{
  public class AboutPageViewModel : PageBaseViewModel
  {
    private readonly DiagnosticsService _diagnosticsService;
    private readonly UpdatesHistoryService _updatesHistoryService;
    private readonly MiBandApp.Storage.Settings.Settings _settings;
    private readonly ILogManager _logManager;
    private readonly EmailComposer _emailComposer;
    private readonly ResourceLoader _resourceLoader = new ResourceLoader();

    public AboutPageViewModel(
      DiagnosticsService diagnosticsService,
      UpdatesHistoryService updatesHistoryService,
      MiBandApp.Storage.Settings.Settings settings,
      ILogManager logManager,
      EmailComposer emailComposer)
    {
      this._diagnosticsService = diagnosticsService;
      this._updatesHistoryService = updatesHistoryService;
      this._settings = settings;
      this._logManager = logManager;
      this._emailComposer = emailComposer;
    }

    public string Version => this._settings.AppVersion.ToString();

    public async void SendEmail()
    {
      MessageDialog dialog = new MessageDialog(this._resourceLoader.GetString("AboutPage_DiagnosticPackageMessage_Body"), this._resourceLoader.GetString("AboutPage_DiagnosticPackageMessage_Header"));
      // ISSUE: method pointer
      dialog.Commands.Add((IUICommand) new UICommand(this._resourceLoader.GetString("AboutPage_DiagnosticPackageMessage_Yes"), new UICommandInvokedHandler((object) this, __methodptr(\u003CSendEmail\u003Eb__9_0))));
      // ISSUE: method pointer
      dialog.Commands.Add((IUICommand) new UICommand(this._resourceLoader.GetString("AboutPage_DiagnosticPackageMessage_No"), new UICommandInvokedHandler((object) this, __methodptr(\u003CSendEmail\u003Eb__9_1))));
      dialog.put_DefaultCommandIndex(1U);
      IUICommand iuiCommand = await dialog.ShowAsyncSafe();
    }

    public List<UpdateHistoryItem> Updates
    {
      get
      {
        return this._updatesHistoryService.AllUpdates.OrderByDescending<UpdateHistoryItem, int>((Func<UpdateHistoryItem, int>) (t => t.Id)).ToList<UpdateHistoryItem>();
      }
    }

    public async void SendExtraDiagnostics()
    {
      IStorageFile diagnosticFile = await this._diagnosticsService.GetDiagnosticFile();
      if (diagnosticFile == null)
        return;
      this._emailComposer.ComposeEmail(diagnosticFile, "BMB Diagnostics");
    }
  }
}
