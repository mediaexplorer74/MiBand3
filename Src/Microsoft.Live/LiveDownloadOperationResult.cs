// Decompiled with JetBrains decompiler
// Type: Microsoft.Live.LiveDownloadOperationResult
// Assembly: Microsoft.Live, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 84D9FE74-951C-4BB4-80F8-EDD369EAC36D
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\Microsoft.Live.dll

using System;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;

#nullable disable
namespace Microsoft.Live
{
  public class LiveDownloadOperationResult
  {
    private const uint BufferSize = 102400;

    internal LiveDownloadOperationResult(IStorageFile file) => this.File = file;

    internal LiveDownloadOperationResult(IInputStream stream) => this.Stream = stream;

    internal LiveDownloadOperationResult(Exception error, bool cancelled)
    {
      this.Error = error;
      this.IsCancelled = cancelled;
    }

    public IStorageFile File { get; private set; }

    public IInputStream Stream { get; private set; }

    internal Exception Error { get; private set; }

    internal bool IsCancelled { get; private set; }

    public async Task<IRandomAccessStream> GetRandomAccessStreamAsync()
    {
      IRandomAccessStream ras = (IRandomAccessStream) null;
      if (this.File != null)
        ras = await this.File.OpenAsync((FileAccessMode) 0);
      else if (this.Stream != null)
      {
        ras = (IRandomAccessStream) new InMemoryRandomAccessStream();
        DataWriter dw = new DataWriter(ras.GetOutputStreamAt(0UL));
        using (DataReader dr = new DataReader(this.Stream))
        {
          uint bytesRead = 0;
          do
          {
            bytesRead = await (IAsyncOperation<uint>) dr.LoadAsync(102400U);
            if (bytesRead > 0U)
            {
              byte[] numArray = new byte[(IntPtr) bytesRead];
              dr.ReadBytes(numArray);
              dw.WriteBytes(numArray);
            }
          }
          while (bytesRead > 0U);
        }
        int num = (int) await (IAsyncOperation<uint>) dw.StoreAsync();
      }
      return ras;
    }
  }
}
