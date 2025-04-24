
// Type: MiBand.SDK.Tools.IBufferExtensions
// Assembly: MiBand.SDK, Version=0.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 152FA565-3249-4054-A361-01D9C0AFA7F1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBand.SDK.dll

using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage.Streams;

#nullable disable
namespace MiBand.SDK.Tools
{
  internal static class IBufferExtensions
  {
    public static byte[] ToArraySafe(this IBuffer buffer)
    {
      return buffer != null && buffer.Length > 0U ? buffer.ToArray() : (byte[]) null;
    }
  }
}
