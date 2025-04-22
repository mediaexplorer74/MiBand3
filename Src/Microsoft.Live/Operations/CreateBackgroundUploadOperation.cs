// Decompiled with JetBrains decompiler
// Type: Microsoft.Live.Operations.CreateBackgroundUploadOperation
// Assembly: Microsoft.Live, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 84D9FE74-951C-4BB4-80F8-EDD369EAC36D
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\Microsoft.Live.dll

using System;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.Storage.Streams;

#nullable disable
namespace Microsoft.Live.Operations
{
  internal class CreateBackgroundUploadOperation : ApiOperation
  {
    private TaskCompletionSource<LiveUploadOperation> taskCompletionSource;

    public CreateBackgroundUploadOperation(
      LiveConnectClient client,
      Uri url,
      string fileName,
      IStorageFile inputFile,
      OverwriteOption option)
      : base(client, url, ApiMethod.Upload, (string) null, (SynchronizationContextWrapper) null)
    {
      this.InputFile = inputFile;
      this.FileName = fileName;
      this.OverwriteOption = option;
    }

    public CreateBackgroundUploadOperation(
      LiveConnectClient client,
      Uri url,
      string fileName,
      IInputStream inputStream,
      OverwriteOption option)
      : base(client, url, ApiMethod.Upload, (string) null, (SynchronizationContextWrapper) null)
    {
      this.InputStream = inputStream;
      this.FileName = fileName;
      this.OverwriteOption = option;
    }

    public string FileName { get; private set; }

    public OverwriteOption OverwriteOption { get; private set; }

    public IStorageFile InputFile { get; private set; }

    public IInputStream InputStream { get; private set; }

    public Task<LiveUploadOperation> ExecuteAsync()
    {
      this.taskCompletionSource = new TaskCompletionSource<LiveUploadOperation>();
      this.Execute();
      return this.taskCompletionSource.Task;
    }

    protected override void OnExecute()
    {
      GetUploadLinkOperation uploadLinkOperation = new GetUploadLinkOperation(this.LiveClient, this.Url, this.FileName, this.OverwriteOption, (SynchronizationContextWrapper) null);
      uploadLinkOperation.OperationCompletedCallback = new Action<LiveOperationResult>(this.OnGetUploadLinkCompleted);
      uploadLinkOperation.Execute();
    }

    private async void OnGetUploadLinkCompleted(LiveOperationResult result)
    {
      if (result.Error != null)
      {
        this.taskCompletionSource.SetException((Exception) result.Error);
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
        UploadOperation uploadOperation;
        if (this.InputStream != null)
          uploadOperation = await uploader.CreateUploadFromStreamAsync((Uri) uploadUrl, this.InputStream);
        else
          uploadOperation = uploader.CreateUpload((Uri) uploadUrl, this.InputFile);
        this.taskCompletionSource.SetResult(new LiveUploadOperation(uploadOperation));
      }
    }
  }
}
