
// Type: Microsoft.Live.Operations.TailoredDownloadOperation
// Assembly: Microsoft.Live, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 84D9FE74-951C-4BB4-80F8-EDD369EAC36D
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\Microsoft.Live.dll

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.Storage.Streams;

#nullable disable
namespace Microsoft.Live.Operations
{
  internal class TailoredDownloadOperation : ApiOperation
  {
    private const uint MaxDownloadResponseLength = 10240;
    private DownloadOperation downloadOp;
    private CancellationTokenSource cts;
    private bool isAttach;

    public TailoredDownloadOperation(
      LiveConnectClient client,
      Uri url,
      IStorageFile outputFile,
      IProgress<LiveOperationProgress> progress,
      SynchronizationContextWrapper syncContext)
      : base(client, url, ApiMethod.Download, (string) null, syncContext)
    {
      this.OutputFile = outputFile;
      this.Progress = progress;
    }

    internal TailoredDownloadOperation()
      : base((LiveConnectClient) null, (Uri) null, ApiMethod.Download, (string) null, (SynchronizationContextWrapper) null)
    {
    }

    public IStorageFile OutputFile { get; private set; }

    public IProgress<LiveOperationProgress> Progress { get; private set; }

    public Action<LiveDownloadOperationResult> OperationCompletedCallback { get; set; }

    public async void Attach(DownloadOperation downloadOp)
    {
      this.downloadOp = downloadOp;
      this.isAttach = true;
      this.cts = new CancellationTokenSource();
      System.Progress<DownloadOperation> progressHandler = new System.Progress<DownloadOperation>((Action<DownloadOperation>) new Action<DownloadOperation>(this.OnDownloadProgress));
      try
      {
        this.downloadOp = await this.downloadOp.AttachAsync().AsTask<DownloadOperation, DownloadOperation>(this.cts.Token, (IProgress<DownloadOperation>) progressHandler);
      }
      catch
      {
      }
    }

    public override void Cancel()
    {
      if (this.Status == OperationStatus.Cancelled || this.Status == OperationStatus.Completed)
        return;
      this.Status = OperationStatus.Cancelled;
      if (this.downloadOp != null)
        this.cts.Cancel();
      else
        this.OnCancel();
    }

    protected override void OnCancel()
    {
      this.OnOperationCompleted(new LiveDownloadOperationResult((Exception) null, true));
    }

    protected override async void OnExecute()
    {
      BackgroundDownloader downloader;
      if (this.Url.OriginalString.StartsWith(this.LiveClient.ApiEndpoint, StringComparison.OrdinalIgnoreCase))
      {
        if (this.RefreshTokenIfNeeded())
          return;
        downloader = new BackgroundDownloader();
        if (this.LiveClient.Session != null)
          downloader.SetRequestHeader("Authorization", "bearer " + this.LiveClient.Session.AccessToken);
        downloader.SetRequestHeader("X-HTTP-Live-Library", Platform.GetLibraryHeaderValue());
      }
      else
        downloader = new BackgroundDownloader();
      downloader.Group = LiveConnectClient.LiveSDKDownloadGroup;
      this.cts = new CancellationTokenSource();
      this.downloadOp = downloader.CreateDownload((Uri) this.Url, this.OutputFile);
      System.Progress<DownloadOperation> progressHandler = new System.Progress<DownloadOperation>((Action<DownloadOperation>) new Action<DownloadOperation>(this.OnDownloadProgress));
      LiveDownloadOperationResult result = (LiveDownloadOperationResult) null;
      Exception webError = (Exception) null;
      try
      {
        this.downloadOp = await this.downloadOp.StartAsync().AsTask<DownloadOperation, DownloadOperation>(this.cts.Token, (IProgress<DownloadOperation>) progressHandler);
        result = this.OutputFile != null ? new LiveDownloadOperationResult(this.OutputFile) : new LiveDownloadOperationResult(this.downloadOp.GetResultStreamAt(0UL));
      }
      catch (TaskCanceledException ex)
      {
        result = new LiveDownloadOperationResult((Exception) null, true);
      }
      catch (Exception ex)
      {
        webError = ex;
      }
      if (webError != null)
        result = await this.ProcessDownloadErrorResponse(webError);
      this.OnOperationCompleted(result);
    }

    protected void OnOperationCompleted(LiveDownloadOperationResult result)
    {
      Action<LiveDownloadOperationResult> completedCallback = this.OperationCompletedCallback;
      if (completedCallback == null)
        return;
      completedCallback(result);
    }

    private void OnDownloadProgress(DownloadOperation downloadOp)
    {
      if (downloadOp.Progress.Status == BackgroundTransferStatus.Error 
                || downloadOp.Progress.Status == BackgroundTransferStatus.Canceled 
                || this.isAttach || this.Progress == null)
        return;

      this.Progress.Report(new LiveOperationProgress((long) downloadOp.Progress.BytesReceived, 
          (long) downloadOp.Progress.TotalBytesToReceive));
    }

    private async Task<LiveDownloadOperationResult> ProcessDownloadErrorResponse(Exception exception)
    {
      LiveDownloadOperationResult opResult;
      try
      {
        IInputStream responseStream = this.downloadOp.GetResultStreamAt(0UL);
        if (responseStream == null)
        {
          opResult = new LiveDownloadOperationResult((Exception) new LiveConnectException("server_error", ResourceHelper.GetString("ConnectionError")), false);
        }
        else
        {
          DataReader reader = new DataReader(responseStream);
          uint length = await (IAsyncOperation<uint>) reader.LoadAsync(10240U);
          Exception error = ApiOperation.CreateOperationResultFrom(reader.ReadString(length), this.Method).Error;
          if (error is FormatException)
            error = exception;
          opResult = new LiveDownloadOperationResult(error, false);
        }
      }
      catch (COMException ex)
      {
        opResult = new LiveDownloadOperationResult((Exception) ex, false);
      }
      catch (FileNotFoundException ex)
      {
        opResult = new LiveDownloadOperationResult((Exception) ex, false);
      }
      return opResult;
    }
  }
}
