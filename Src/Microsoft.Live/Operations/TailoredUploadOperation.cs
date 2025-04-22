// Decompiled with JetBrains decompiler
// Type: Microsoft.Live.Operations.TailoredUploadOperation
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
  internal class TailoredUploadOperation : ApiOperation
  {
    private const uint MaxUploadResponseLength = 10240;
    private UploadOperation uploadOp;
    private CancellationTokenSource uploadOpCts;
    private bool isAttach;

    public TailoredUploadOperation(
      LiveConnectClient client,
      Uri url,
      string fileName,
      IStorageFile inputFile,
      OverwriteOption option,
      IProgress<LiveOperationProgress> progress,
      SynchronizationContextWrapper syncContext)
      : this(client, url, fileName, option, progress, syncContext)
    {
      this.InputFile = inputFile;
    }

    public TailoredUploadOperation(
      LiveConnectClient client,
      Uri url,
      string fileName,
      IInputStream inputStream,
      OverwriteOption option,
      IProgress<LiveOperationProgress> progress,
      SynchronizationContextWrapper syncContext)
      : this(client, url, fileName, option, progress, syncContext)
    {
      this.InputStream = inputStream;
    }

    internal TailoredUploadOperation(
      LiveConnectClient client,
      Uri url,
      string fileName,
      OverwriteOption option,
      IProgress<LiveOperationProgress> progress,
      SynchronizationContextWrapper syncContext)
      : base(client, url, ApiMethod.Upload, (string) null, syncContext)
    {
      this.FileName = fileName;
      this.Progress = progress;
      this.OverwriteOption = option;
    }

    internal TailoredUploadOperation()
      : base((LiveConnectClient) null, (Uri) null, ApiMethod.Upload, (string) null, (SynchronizationContextWrapper) null)
    {
    }

    public string FileName { get; private set; }

    public OverwriteOption OverwriteOption { get; private set; }

    public IStorageFile InputFile { get; private set; }

    public IInputStream InputStream { get; private set; }

    public IProgress<LiveOperationProgress> Progress { get; private set; }

    public async void Attach(UploadOperation uploadOp)
    {
      this.uploadOp = uploadOp;
      this.isAttach = true;
      this.uploadOpCts = new CancellationTokenSource();
      System.Progress<UploadOperation> progressHandler = new System.Progress<UploadOperation>((Action<UploadOperation>) new Action<UploadOperation>(this.OnUploadProgress));
      try
      {
        this.uploadOp = await this.uploadOp.AttachAsync().AsTask<UploadOperation, UploadOperation>(this.uploadOpCts.Token, (IProgress<UploadOperation>) progressHandler);
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
      if (this.uploadOp != null)
        this.uploadOpCts.Cancel();
      else
        this.OnCancel();
    }

    protected override void OnExecute()
    {
      GetUploadLinkOperation uploadLinkOperation = new GetUploadLinkOperation(this.LiveClient, this.Url, this.FileName, this.OverwriteOption, (SynchronizationContextWrapper) null);
      uploadLinkOperation.OperationCompletedCallback = new Action<LiveOperationResult>(this.OnGetUploadLinkCompleted);
      uploadLinkOperation.Execute();
    }

    private async void OnGetUploadLinkCompleted(LiveOperationResult result)
    {
      if (this.Status == OperationStatus.Cancelled)
      {
        // ISSUE: reference to a compiler-generated method
        this.\u003C\u003En__FabricatedMethodf();
      }
      else if (result.Error != null || result.IsCancelled)
      {
        this.OnOperationCompleted(result);
      }
      else
      {
        Uri uploadUrl = new Uri(result.RawResult, UriKind.Absolute);
        BackgroundUploader uploader = new BackgroundUploader();
        uploader.put_Group(LiveConnectClient.LiveSDKUploadGroup);
        if (this.LiveClient.Session != null)
          uploader.SetRequestHeader("Authorization", "bearer " + this.LiveClient.Session.AccessToken);
        uploader.SetRequestHeader("X-HTTP-Live-Library", Platform.GetLibraryHeaderValue());
        uploader.put_Method("PUT");
        this.uploadOpCts = new CancellationTokenSource();
        Exception webError = (Exception) null;
        LiveOperationResult opResult = (LiveOperationResult) null;
        try
        {
          if (this.InputStream != null)
            this.uploadOp = await uploader.CreateUploadFromStreamAsync((Uri) uploadUrl, this.InputStream);
          else
            this.uploadOp = uploader.CreateUpload((Uri) uploadUrl, this.InputFile);
          System.Progress<UploadOperation> progressHandler = new System.Progress<UploadOperation>((Action<UploadOperation>) new Action<UploadOperation>(this.OnUploadProgress));
          this.uploadOp = await this.uploadOp.StartAsync().AsTask<UploadOperation, UploadOperation>(this.uploadOpCts.Token, (IProgress<UploadOperation>) progressHandler);
        }
        catch (TaskCanceledException ex)
        {
          opResult = new LiveOperationResult((Exception) ex, true);
        }
        catch (Exception ex)
        {
          webError = ex;
        }
        if (opResult == null)
        {
          try
          {
            IInputStream responseStream = this.uploadOp == null ? (IInputStream) null : this.uploadOp.GetResultStreamAt(0UL);
            if (responseStream == null)
            {
              opResult = new LiveOperationResult((Exception) new LiveConnectException("client_error", ResourceHelper.GetString("ConnectionError")), false);
            }
            else
            {
              DataReader reader = new DataReader(responseStream);
              uint length = await (IAsyncOperation<uint>) reader.LoadAsync(10240U);
              opResult = ApiOperation.CreateOperationResultFrom(reader.ReadString(length), ApiMethod.Upload);
              if (webError != null)
              {
                if (opResult.Error != null)
                {
                  if (!(opResult.Error is LiveConnectException))
                    opResult = new LiveOperationResult(webError, false);
                }
              }
            }
          }
          catch (COMException ex)
          {
            opResult = new LiveOperationResult((Exception) ex, false);
          }
          catch (FileNotFoundException ex)
          {
            opResult = new LiveOperationResult((Exception) ex, false);
          }
        }
        this.OnOperationCompleted(opResult);
      }
    }

    private void OnUploadProgress(UploadOperation uploadOp)
    {
      if (uploadOp.Progress.Status == 7 || uploadOp.Progress.Status == 6 || this.isAttach || this.Progress == null)
        return;
      this.Progress.Report(new LiveOperationProgress((long) uploadOp.Progress.BytesSent, (long) uploadOp.Progress.TotalBytesToSend));
    }
  }
}
