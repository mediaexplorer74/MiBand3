
// Type: Microsoft.Live.LiveConnectClient
// Assembly: Microsoft.Live, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 84D9FE74-951C-4BB4-80F8-EDD369EAC36D
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\Microsoft.Live.dll

using Microsoft.Live.Operations;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.Storage.Streams;

#nullable disable
namespace Microsoft.Live
{
  public sealed class LiveConnectClient
  {
    private const string DefaultApiEndpoint = "https://apis.live.net/v5.0";
    private SynchronizationContextWrapper syncContext;
    internal static readonly string LiveSDKDownloadGroup = "LiveSDKDownloadGroup_C8B9A6E4-1030-4BE1-ADB6-1FB89AFDCED3";
    internal static readonly string LiveSDKUploadGroup = "LiveSDKUploadGroup_C8B9A6E4-1030-4BE1-ADB6-1FB89AFDCED3";

    public LiveConnectClient(LiveConnectSession session)
    {
      this.Session = session != null ? session : throw new ArgumentNullException(nameof (session));
      this.syncContext = SynchronizationContextWrapper.Current;
      this.ApiEndpoint = "https://apis.live.net/v5.0";
    }

    public LiveConnectSession Session { get; internal set; }

    internal string ApiEndpoint { get; set; }

    internal Uri GetResourceUri(string path, ApiMethod method)
    {
      try
      {
        if (path.StartsWith("https://", StringComparison.OrdinalIgnoreCase) && !path.StartsWith(this.ApiEndpoint, StringComparison.OrdinalIgnoreCase) || path.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
          return new Uri(path, UriKind.Absolute);
        StringBuilder sb = !path.StartsWith(this.ApiEndpoint, StringComparison.OrdinalIgnoreCase) ? new StringBuilder(this.ApiEndpoint).AppendUrlPath(path) : new StringBuilder(path);
        Uri uri = new Uri(sb.ToString(), UriKind.Absolute);
        sb.Append(string.IsNullOrEmpty(uri.Query) ? "?" : "&");
        if (method != ApiMethod.Download)
        {
          sb.AppendQueryParam("suppress_response_codes", "true");
          sb.Append("&").AppendQueryParam("suppress_redirects", "true");
        }
        return new Uri(sb.ToString(), UriKind.Absolute);
      }
      catch (FormatException ex)
      {
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, ResourceHelper.GetString("UrlInvalid"), (object) path), nameof (path));
      }
    }

    private static bool IsAbsolutePath(string path)
    {
      return path.StartsWith("https://", StringComparison.OrdinalIgnoreCase) || path.StartsWith("http://", StringComparison.OrdinalIgnoreCase);
    }

    private ApiOperation GetApiOperation(string path, ApiMethod method, string body)
    {
      if (path == null)
        throw new ArgumentNullException(nameof (path));
      if (string.IsNullOrWhiteSpace(path))
        throw new ArgumentException(nameof (path));
      Uri url = !LiveConnectClient.IsAbsolutePath(path) ? this.GetResourceUri(path, method) : throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, ResourceHelper.GetString("RelativeUrlRequired"), (object) nameof (path)), nameof (path));
      if (this.Session == null)
        throw new LiveConnectException("client_error", ResourceHelper.GetString("UserNotLoggedIn"));
      ApiOperation apiOperation = (ApiOperation) null;
      switch (method)
      {
        case ApiMethod.Get:
        case ApiMethod.Delete:
          apiOperation = new ApiOperation(this, url, method, (string) null, (SynchronizationContextWrapper) null);
          break;
        case ApiMethod.Post:
        case ApiMethod.Put:
        case ApiMethod.Move:
        case ApiMethod.Copy:
          if (body == null)
            throw new ArgumentNullException(nameof (body));
          if (string.IsNullOrWhiteSpace(body))
            throw new ArgumentException(nameof (body));
          apiOperation = (ApiOperation) new ApiWriteOperation(this, url, method, body, (SynchronizationContextWrapper) null);
          break;
      }
      return apiOperation;
    }

    public static async Task<IEnumerable<LiveDownloadOperation>> GetCurrentBackgroundDownloadsAsync()
    {
      LinkedList<LiveDownloadOperation> pendingOperations = new LinkedList<LiveDownloadOperation>();
      IReadOnlyList<DownloadOperation> activeDownloads = await (IAsyncOperation<IReadOnlyList<DownloadOperation>>) BackgroundDownloader.GetCurrentDownloadsAsync(LiveConnectClient.LiveSDKDownloadGroup);
      foreach (DownloadOperation downloadOperation in (IEnumerable<DownloadOperation>) activeDownloads)
        pendingOperations.AddLast(new LiveDownloadOperation(downloadOperation));
      return (IEnumerable<LiveDownloadOperation>) pendingOperations;
    }

    public static async Task<IEnumerable<LiveUploadOperation>> GetCurrentBackgroundUploadsAsync()
    {
      LinkedList<LiveUploadOperation> pendingOperations = new LinkedList<LiveUploadOperation>();
      IReadOnlyList<UploadOperation> activeUploads = await (IAsyncOperation<IReadOnlyList<UploadOperation>>) BackgroundUploader.GetCurrentUploadsAsync(LiveConnectClient.LiveSDKUploadGroup);
      foreach (UploadOperation uploadOperation in (IEnumerable<UploadOperation>) activeUploads)
        pendingOperations.AddLast(new LiveUploadOperation(uploadOperation));
      return (IEnumerable<LiveUploadOperation>) pendingOperations;
    }

    public Task<LiveDownloadOperation> CreateBackgroundDownloadAsync(
      string path,
      IStorageFile outputFile = null)
    {
      return path != null ? new CreateBackgroundDownloadOperation(this, this.GetResourceUri(path, ApiMethod.Download), outputFile).ExecuteAsync() : throw new ArgumentNullException(nameof (path));
    }

    public Task<LiveDownloadOperationResult> BackgroundDownloadAsync(
      string path,
      IStorageFile outputFile)
    {
      return this.BackgroundDownloadAsync(path, outputFile, new CancellationToken(false), (IProgress<LiveOperationProgress>) null);
    }

    public Task<LiveDownloadOperationResult> BackgroundDownloadAsync(
      string path,
      IStorageFile outputFile,
      CancellationToken ct,
      IProgress<LiveOperationProgress> progress)
    {
      if (string.IsNullOrEmpty(path))
        throw new ArgumentException(nameof (path), string.Format((IFormatProvider) CultureInfo.CurrentUICulture, ResourceHelper.GetString("UrlInvalid"), (object) nameof (path)));
      if (outputFile == null)
        throw new ArgumentNullException(nameof (outputFile), string.Format((IFormatProvider) CultureInfo.CurrentUICulture, ResourceHelper.GetString("InvalidNullParameter"), (object) nameof (outputFile)));
      return this.InternalDownloadAsync(path, outputFile, ct, progress);
    }

    public Task<LiveDownloadOperationResult> BackgroundDownloadAsync(string path)
    {
      return this.BackgroundDownloadAsync(path, new CancellationToken(false), (IProgress<LiveOperationProgress>) null);
    }

    public Task<LiveDownloadOperationResult> BackgroundDownloadAsync(
      string path,
      CancellationToken ct,
      IProgress<LiveOperationProgress> progress)
    {
      if (path == null)
        throw new ArgumentNullException(nameof (path));
      if (string.IsNullOrWhiteSpace(path))
        throw new ArgumentException(nameof (path), string.Format((IFormatProvider) CultureInfo.CurrentUICulture, ResourceHelper.GetString("UrlInvalid"), (object) nameof (path)));
      return this.InternalDownloadAsync(path, (IStorageFile) null, ct, progress);
    }

    public Task<LiveOperationResult> BackgroundUploadAsync(
      string path,
      string fileName,
      IStorageFile inputFile,
      OverwriteOption option)
    {
      return this.BackgroundUploadAsync(path, fileName, inputFile, option, new CancellationToken(false), (IProgress<LiveOperationProgress>) null);
    }

    public Task<LiveOperationResult> BackgroundUploadAsync(
      string path,
      string fileName,
      IStorageFile inputFile,
      OverwriteOption option,
      CancellationToken ct,
      IProgress<LiveOperationProgress> progress)
    {
      if (string.IsNullOrEmpty(path))
        throw new ArgumentException(nameof (path), string.Format((IFormatProvider) CultureInfo.CurrentUICulture, ResourceHelper.GetString("UrlInvalid"), (object) nameof (path)));
      if (string.IsNullOrEmpty(fileName))
        throw new ArgumentException(nameof (fileName), string.Format((IFormatProvider) CultureInfo.CurrentUICulture, ResourceHelper.GetString("InvalidNullOrEmptyParameter"), (object) nameof (fileName)));
      if (inputFile == null)
        throw new ArgumentNullException(nameof (inputFile), string.Format((IFormatProvider) CultureInfo.CurrentUICulture, ResourceHelper.GetString("InvalidNullParameter"), (object) nameof (inputFile)));
      return this.ExecuteApiOperation((ApiOperation) new TailoredUploadOperation(this, this.GetResourceUri(path, ApiMethod.Upload), fileName, inputFile, option, progress, (SynchronizationContextWrapper) null), ct);
    }

    public Task<LiveOperationResult> BackgroundUploadAsync(
      string path,
      string fileName,
      IInputStream inputStream,
      OverwriteOption option)
    {
      return this.BackgroundUploadAsync(path, fileName, inputStream, option, new CancellationToken(false), (IProgress<LiveOperationProgress>) null);
    }

    public Task<LiveOperationResult> BackgroundUploadAsync(
      string path,
      string fileName,
      IInputStream inputStream,
      OverwriteOption option,
      CancellationToken ct,
      IProgress<LiveOperationProgress> progress)
    {
      if (string.IsNullOrEmpty(path))
        throw new ArgumentException(nameof (path), string.Format((IFormatProvider) CultureInfo.CurrentUICulture, ResourceHelper.GetString("UrlInvalid"), (object) nameof (path)));
      if (string.IsNullOrEmpty(fileName))
        throw new ArgumentException(nameof (fileName), string.Format((IFormatProvider) CultureInfo.CurrentUICulture, ResourceHelper.GetString("InvalidNullOrEmptyParameter"), (object) nameof (fileName)));
      if (inputStream == null)
        throw new ArgumentNullException(nameof (inputStream), string.Format((IFormatProvider) CultureInfo.CurrentUICulture, ResourceHelper.GetString("InvalidNullParameter"), (object) nameof (inputStream)));
      return this.ExecuteApiOperation((ApiOperation) new TailoredUploadOperation(this, this.GetResourceUri(path, ApiMethod.Upload), fileName, inputStream, option, progress, (SynchronizationContextWrapper) null), ct);
    }

    public Task<LiveUploadOperation> CreateBackgroundUploadAsync(
      string path,
      string fileName,
      IStorageFile inputFile,
      OverwriteOption option)
    {
      if (string.IsNullOrEmpty(path))
        throw new ArgumentException(nameof (path), string.Format((IFormatProvider) CultureInfo.CurrentUICulture, ResourceHelper.GetString("UrlInvalid"), (object) nameof (path)));
      if (string.IsNullOrEmpty(fileName))
        throw new ArgumentException(nameof (fileName), string.Format((IFormatProvider) CultureInfo.CurrentUICulture, ResourceHelper.GetString("InvalidNullOrEmptyParameter"), (object) nameof (fileName)));
      if (inputFile == null)
        throw new ArgumentNullException(nameof (inputFile), string.Format((IFormatProvider) CultureInfo.CurrentUICulture, ResourceHelper.GetString("InvalidNullParameter"), (object) nameof (inputFile)));
      return new CreateBackgroundUploadOperation(this, this.GetResourceUri(path, ApiMethod.Upload), fileName, inputFile, option).ExecuteAsync();
    }

    public Task<LiveUploadOperation> CreateBackgroundUploadAsync(
      string path,
      string fileName,
      IInputStream inputStream,
      OverwriteOption option)
    {
      if (string.IsNullOrEmpty(path))
        throw new ArgumentException(nameof (path), string.Format((IFormatProvider) CultureInfo.CurrentUICulture, ResourceHelper.GetString("UrlInvalid"), (object) nameof (path)));
      if (string.IsNullOrEmpty(fileName))
        throw new ArgumentException(nameof (fileName), string.Format((IFormatProvider) CultureInfo.CurrentUICulture, ResourceHelper.GetString("InvalidNullOrEmptyParameter"), (object) nameof (fileName)));
      if (inputStream == null)
        throw new ArgumentNullException(nameof (inputStream), string.Format((IFormatProvider) CultureInfo.CurrentUICulture, ResourceHelper.GetString("InvalidNullParameter"), (object) nameof (inputStream)));
      return new CreateBackgroundUploadOperation(this, this.GetResourceUri(path, ApiMethod.Upload), fileName, inputStream, option).ExecuteAsync();
    }

    internal Task<LiveDownloadOperationResult> InternalDownloadAsync(
      string path,
      IStorageFile outputFile,
      CancellationToken ct,
      IProgress<LiveOperationProgress> progress)
    {
      if (this.Session == null)
        throw new LiveConnectException("client_error", ResourceHelper.GetString("UserNotLoggedIn"));
      TaskCompletionSource<LiveDownloadOperationResult> tcs = new TaskCompletionSource<LiveDownloadOperationResult>();
      TailoredDownloadOperation downloadOperation = new TailoredDownloadOperation(this, this.GetResourceUri(path, ApiMethod.Download), outputFile, progress, (SynchronizationContextWrapper) null);
      downloadOperation.OperationCompletedCallback = (Action<LiveDownloadOperationResult>) (result =>
      {
        if (result.IsCancelled)
          tcs.TrySetCanceled();
        else if (result.Error != null)
          tcs.TrySetException((Exception) result.Error);
        else
          tcs.TrySetResult(result);
      });
      ct.Register((Action) new Action(((Operation) downloadOperation).Cancel));
      downloadOperation.Execute();
      return tcs.Task;
    }

    public Task<LiveOperationResult> GetAsync(string path)
    {
      return this.Api(path, ApiMethod.Get, (string) null, new CancellationToken(false));
    }

    public Task<LiveOperationResult> GetAsync(string path, CancellationToken ct)
    {
      return this.Api(path, ApiMethod.Get, (string) null, ct);
    }

    public Task<LiveOperationResult> DeleteAsync(string path)
    {
      return this.Api(path, ApiMethod.Delete, (string) null, new CancellationToken(false));
    }

    public Task<LiveOperationResult> DeleteAsync(string path, CancellationToken ct)
    {
      return this.Api(path, ApiMethod.Delete, (string) null, ct);
    }

    public Task<LiveOperationResult> PostAsync(string path, string body)
    {
      return this.Api(path, ApiMethod.Post, body, new CancellationToken(false));
    }

    public Task<LiveOperationResult> PostAsync(string path, IDictionary<string, object> body)
    {
      return this.Api(path, ApiMethod.Post, ApiOperation.SerializePostBody(body), new CancellationToken(false));
    }

    public Task<LiveOperationResult> PostAsync(string path, string body, CancellationToken ct)
    {
      return this.Api(path, ApiMethod.Post, body, ct);
    }

    public Task<LiveOperationResult> PostAsync(
      string path,
      IDictionary<string, object> body,
      CancellationToken ct)
    {
      return this.Api(path, ApiMethod.Post, ApiOperation.SerializePostBody(body), ct);
    }

    public Task<LiveOperationResult> PutAsync(string path, string body)
    {
      return this.Api(path, ApiMethod.Put, body, new CancellationToken(false));
    }

    public Task<LiveOperationResult> PutAsync(string path, IDictionary<string, object> body)
    {
      return this.Api(path, ApiMethod.Put, ApiOperation.SerializePostBody(body), new CancellationToken(false));
    }

    public Task<LiveOperationResult> PutAsync(string path, string body, CancellationToken ct)
    {
      return this.Api(path, ApiMethod.Put, body, ct);
    }

    public Task<LiveOperationResult> PutAsync(
      string path,
      IDictionary<string, object> body,
      CancellationToken ct)
    {
      return this.Api(path, ApiMethod.Put, ApiOperation.SerializePostBody(body), ct);
    }

    public Task<LiveOperationResult> MoveAsync(string path, string destination)
    {
      return this.MoveOrCopy(path, destination, ApiMethod.Move, new CancellationToken(false));
    }

    public Task<LiveOperationResult> MoveAsync(
      string path,
      string destination,
      CancellationToken ct)
    {
      return this.MoveOrCopy(path, destination, ApiMethod.Move, ct);
    }

    public Task<LiveOperationResult> CopyAsync(string path, string destination)
    {
      return this.MoveOrCopy(path, destination, ApiMethod.Copy, new CancellationToken(false));
    }

    public Task<LiveOperationResult> CopyAsync(
      string path,
      string destination,
      CancellationToken ct)
    {
      return this.MoveOrCopy(path, destination, ApiMethod.Copy, ct);
    }

    private Task<LiveOperationResult> MoveOrCopy(
      string path,
      string destination,
      ApiMethod method,
      CancellationToken ct)
    {
      if (destination == null)
        throw new ArgumentNullException(nameof (destination));
      string body = !string.IsNullOrWhiteSpace(destination) ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{{ \"destination\" : \"{0}\" }}", (object) destination) : throw new ArgumentException(nameof (destination));
      return this.Api(path, method, body, ct);
    }

    private Task<LiveOperationResult> Api(
      string path,
      ApiMethod method,
      string body,
      CancellationToken ct)
    {
      return this.ExecuteApiOperation(this.GetApiOperation(path, method, body), ct);
    }

    private Task<LiveOperationResult> ExecuteApiOperation(ApiOperation op, CancellationToken ct)
    {
      if (this.Session == null)
        throw new LiveConnectException("client_error", ResourceHelper.GetString("UserNotLoggedIn"));
      TaskCompletionSource<LiveOperationResult> tcs = new TaskCompletionSource<LiveOperationResult>();
      op.OperationCompletedCallback = (Action<LiveOperationResult>) (opResult =>
      {
        if (opResult.IsCancelled)
          tcs.TrySetCanceled();
        else if (opResult.Error != null)
          tcs.TrySetException((Exception) opResult.Error);
        else
          tcs.TrySetResult(opResult);
      });
      ct.Register((Action) new Action(((Operation) op).Cancel));
      op.Execute();
      return tcs.Task;
    }
  }
}
