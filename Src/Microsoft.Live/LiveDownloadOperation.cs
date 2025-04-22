// Decompiled with JetBrains decompiler
// Type: Microsoft.Live.LiveDownloadOperation
// Assembly: Microsoft.Live, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 84D9FE74-951C-4BB4-80F8-EDD369EAC36D
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\Microsoft.Live.dll

using Microsoft.Live.Operations;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage.Streams;

#nullable disable
namespace Microsoft.Live
{
  public class LiveDownloadOperation
  {
    private const uint MaxDownloadResponseLength = 10240;
    private readonly DownloadOperation downloadOperation;

    internal LiveDownloadOperation(DownloadOperation downloadOperation)
    {
      this.downloadOperation = downloadOperation;
    }

    public Guid Guid => this.downloadOperation.Guid;

    public Task<LiveDownloadOperationResult> StartAsync()
    {
      return this.ExecuteAsync(true, CancellationToken.None, (IProgress<LiveOperationProgress>) null);
    }

    public Task<LiveDownloadOperationResult> StartAsync(
      CancellationToken cancellationToken,
      IProgress<LiveOperationProgress> progressHandler = null)
    {
      return this.ExecuteAsync(true, cancellationToken, progressHandler);
    }

    public Task<LiveDownloadOperationResult> AttachAsync()
    {
      return this.ExecuteAsync(false, CancellationToken.None, (IProgress<LiveOperationProgress>) null);
    }

    public Task<LiveDownloadOperationResult> AttachAsync(
      CancellationToken cancellationToken,
      IProgress<LiveOperationProgress> progressHandler = null)
    {
      return this.ExecuteAsync(false, cancellationToken, progressHandler);
    }

    private async Task<LiveDownloadOperationResult> ExecuteAsync(
      bool start,
      CancellationToken cancellationToken,
      IProgress<LiveOperationProgress> progressHandler)
    {
      LiveDownloadOperationResult result = (LiveDownloadOperationResult) null;
      Exception error = (Exception) null;
      LiveDownloadOperationResult downloadOperationResult;
      try
      {
        Progress<DownloadOperation> progressHandlerWrapper = new Progress<DownloadOperation>((Action<DownloadOperation>) (t => progressHandler?.Report(new LiveOperationProgress((long) t.Progress.BytesReceived, (long) t.Progress.TotalBytesToReceive))));
        IAsyncOperationWithProgress<DownloadOperation, DownloadOperation> asyncOperation = start ? this.downloadOperation.StartAsync() : this.downloadOperation.AttachAsync();
        DownloadOperation downloadOperation = await asyncOperation.AsTask<DownloadOperation, DownloadOperation>(cancellationToken, (IProgress<DownloadOperation>) progressHandlerWrapper);
        result = this.downloadOperation.ResultFile != null ? new LiveDownloadOperationResult(this.downloadOperation.ResultFile) : new LiveDownloadOperationResult(this.downloadOperation.GetResultStreamAt(0UL));
        downloadOperationResult = result;
        goto label_6;
      }
      catch (TaskCanceledException ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        error = ex;
      }
      error = await this.ProcessDownloadErrorResponse(error);
      throw error;
label_6:
      return downloadOperationResult;
    }

    private async Task<Exception> ProcessDownloadErrorResponse(Exception exception)
    {
      Exception error;
      try
      {
        IInputStream responseStream = this.downloadOperation.GetResultStreamAt(0UL);
        if (responseStream == null)
        {
          error = (Exception) new LiveConnectException("server_error", ResourceHelper.GetString("ConnectionError"));
        }
        else
        {
          DataReader reader = new DataReader(responseStream);
          uint length = await (IAsyncOperation<uint>) reader.LoadAsync(10240U);
          error = ApiOperation.CreateOperationResultFrom(reader.ReadString(length), ApiMethod.Download).Error;
          if (error is FormatException)
            error = exception;
        }
      }
      catch (COMException ex)
      {
        error = (Exception) ex;
      }
      catch (FileNotFoundException ex)
      {
        error = (Exception) ex;
      }
      return error;
    }
  }
}
