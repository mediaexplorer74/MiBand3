// Decompiled with JetBrains decompiler
// Type: Microsoft.Live.LiveUploadOperation
// Assembly: Microsoft.Live, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 84D9FE74-951C-4BB4-80F8-EDD369EAC36D
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\Microsoft.Live.dll

using Microsoft.Live.Operations;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage.Streams;

#nullable disable
namespace Microsoft.Live
{
  public class LiveUploadOperation
  {
    private const uint MaxUploadResponseLength = 10240;
    private readonly UploadOperation uploadOperation;

    internal LiveUploadOperation(UploadOperation uploadOperation)
    {
      this.uploadOperation = uploadOperation;
    }

    public Guid Guid => this.uploadOperation.Guid;

    public Task<LiveOperationResult> StartAsync()
    {
      return this.ExecuteAsync(true, CancellationToken.None, (IProgress<LiveOperationProgress>) null);
    }

    public Task<LiveOperationResult> StartAsync(
      CancellationToken cancellationToken,
      IProgress<LiveOperationProgress> progressHandler)
    {
      return this.ExecuteAsync(true, cancellationToken, progressHandler);
    }

    public Task<LiveOperationResult> AttachAsync()
    {
      return this.ExecuteAsync(false, CancellationToken.None, (IProgress<LiveOperationProgress>) null);
    }

    public Task<LiveOperationResult> AttachAsync(
      CancellationToken cancellationToken,
      IProgress<LiveOperationProgress> progressHandler)
    {
      return this.ExecuteAsync(false, cancellationToken, progressHandler);
    }

    private async Task<LiveOperationResult> ExecuteAsync(
      bool start,
      CancellationToken cancellationToken,
      IProgress<LiveOperationProgress> progressHandler)
    {
      Exception error = (Exception) null;
      try
      {
        Progress<UploadOperation> progressHandlerWrapper = new Progress<UploadOperation>((Action<UploadOperation>) (t => progressHandler?.Report(new LiveOperationProgress((long) t.Progress.BytesSent, (long) t.Progress.TotalBytesToSend))));
        IAsyncOperationWithProgress<UploadOperation, UploadOperation> asyncOperation = start ? this.uploadOperation.StartAsync() : this.uploadOperation.AttachAsync();
        UploadOperation uploadOperation = await asyncOperation.AsTask<UploadOperation, UploadOperation>(cancellationToken, (IProgress<UploadOperation>) progressHandlerWrapper);
      }
      catch (TaskCanceledException ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        error = ex;
      }
      DataReader reader = new DataReader(this.uploadOperation.GetResultStreamAt(0UL) ?? throw new LiveConnectException("client_error", ResourceHelper.GetString("ConnectionError")));
      uint length = await (IAsyncOperation<uint>) reader.LoadAsync(10240U);
      LiveOperationResult opResult = ApiOperation.CreateOperationResultFrom(reader.ReadString(length), ApiMethod.Upload);
      if (opResult.Error != null)
      {
        if (opResult.Error is LiveConnectException)
          throw opResult.Error;
        if (error != null)
          throw error;
      }
      return opResult;
    }
  }
}
