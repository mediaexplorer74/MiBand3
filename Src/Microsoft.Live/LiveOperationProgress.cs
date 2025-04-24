
// Type: Microsoft.Live.LiveOperationProgress
// Assembly: Microsoft.Live, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 84D9FE74-951C-4BB4-80F8-EDD369EAC36D
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\Microsoft.Live.dll

#nullable disable
namespace Microsoft.Live
{
  public class LiveOperationProgress
  {
    internal LiveOperationProgress(long bytesTransferred, long totalBytes)
    {
      this.BytesTransferred = bytesTransferred;
      this.TotalBytes = totalBytes;
    }

    public long BytesTransferred { get; private set; }

    public long TotalBytes { get; private set; }

    public double ProgressPercentage
    {
      get
      {
        return this.TotalBytes != 0L ? (double) this.BytesTransferred / (double) this.TotalBytes * 100.0 : 0.0;
      }
    }
  }
}
