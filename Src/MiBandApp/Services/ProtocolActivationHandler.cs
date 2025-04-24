
// Type: MiBandApp.Services.ProtocolActivationHandler
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// 

using Caliburn.Micro;
using MiBandApp.ViewModels;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.Storage;

#nullable disable
namespace MiBandApp.Services
{
  public class ProtocolActivationHandler
  {
    private readonly DiagnosticsService _diagnosticsService;
    private readonly EmailComposer _emailComposer;
    private readonly INavigationService _navigationService;

    public ProtocolActivationHandler(
      DiagnosticsService diagnosticsService,
      EmailComposer emailComposer,
      INavigationService navigationService)
    {
      this._diagnosticsService = diagnosticsService;
      this._emailComposer = emailComposer;
      this._navigationService = navigationService;
    }

    public async Task HandleProtocolActivation(ProtocolActivatedEventArgs args)
    {
      if (!(args.Uri.Scheme.ToLowerInvariant() == "bindmiband"))
        return;
      switch (args.Uri.LocalPath.ToLowerInvariant())
      {
        case "diagnose":
          await this.HandleDiagnostics(args).ConfigureAwait(false);
          break;
        case "authorize-client":
          this.HandleAuthorizeClient(args);
          break;
      }
    }

    private void HandleAuthorizeClient(ProtocolActivatedEventArgs args)
    {
      this._navigationService.NavigateToViewModel<SettingsPageViewModel>();
    }

    private async Task HandleDiagnostics(ProtocolActivatedEventArgs args)
    {
      IStorageFile diagnosticFile = await this._diagnosticsService.GetDiagnosticFile();
      if (diagnosticFile == null)
        return;
      this._emailComposer.ComposeEmail(diagnosticFile, "BMB Diagnostics");
    }
  }
}
