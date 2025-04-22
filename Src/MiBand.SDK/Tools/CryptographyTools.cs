// Decompiled with JetBrains decompiler
// Type: MiBand.SDK.Tools.CryptographyTools
// Assembly: MiBand.SDK, Version=0.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 152FA565-3249-4054-A361-01D9C0AFA7F1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBand.SDK.dll

using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;

#nullable disable
namespace MiBand.SDK.Tools
{
  internal static class CryptographyTools
  {
    public static byte[] ComputeMD5(string str)
    {
      byte[] md5;
      CryptographicBuffer.CopyToByteArray(HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5).HashData(CryptographicBuffer.ConvertStringToBinary(str, (BinaryStringEncoding) 0)), ref md5);
      return md5;
    }
  }
}
