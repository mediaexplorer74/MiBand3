
// Type: Microsoft.Live.Operations.CreateBackgroundDownloadOperation
// Assembly: Microsoft.Live, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 84D9FE74-951C-4BB4-80F8-EDD369EAC36D
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\Microsoft.Live.dll

using System;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;

#nullable disable
namespace Microsoft.Live.Operations
{
  internal class CreateBackgroundDownloadOperation : ApiOperation
  {
    private DownloadOperation downloadOp;
    private TaskCompletionSource<LiveDownloadOperation> taskCompletionSource;

    public CreateBackgroundDownloadOperation(
      LiveConnectClient client,
      Uri url,
      IStorageFile outputFile)
      : base(client, url, ApiMethod.Download, (string) null, (SynchronizationContextWrapper) null)
    {
      this.OutputFile = outputFile;
    }

    public IStorageFile OutputFile { get; private set; }

    public Task<LiveDownloadOperation> ExecuteAsync()
    {
      this.taskCompletionSource = new TaskCompletionSource<LiveDownloadOperation>();
      this.Execute();
      return this.taskCompletionSource.Task;
    }

    protected override async void OnExecute()
    {
      LiveLoginResult loginResult = await this.LiveClient.Session.AuthClient.GetLoginStatusAsync();
      if (loginResult.Error != null)
      {
        this.taskCompletionSource.SetException((Exception) loginResult.Error);
      }
      else
      {
        BackgroundDownloader backgroundDownloader = new BackgroundDownloader();
        backgroundDownloader.SetRequestHeader("Authorization", "bearer " + loginResult.Session.AccessToken);
        backgroundDownloader.SetRequestHeader("X-HTTP-Live-Library", Platform.GetLibraryHeaderValue());
        backgroundDownloader.Group = LiveConnectClient.LiveSDKDownloadGroup;
        this.downloadOp = backgroundDownloader.CreateDownload((Uri) this.Url, this.OutputFile);
        this.taskCompletionSource.SetResult(new LiveDownloadOperation(this.downloadOp));
      }
    }
  }
}
