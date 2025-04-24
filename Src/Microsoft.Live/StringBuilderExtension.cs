
// Type: Microsoft.Live.StringBuilderExtension
// Assembly: Microsoft.Live, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 84D9FE74-951C-4BB4-80F8-EDD369EAC36D
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\Microsoft.Live.dll

using System.Text;

#nullable disable
namespace Microsoft.Live
{
  internal static class StringBuilderExtension
  {
    private const char ForwardSlash = '/';

    public static StringBuilder AppendUrlPath(this StringBuilder sb, string path)
    {
      if (path == null)
        return sb;
      if (sb.Length == 0)
        return sb.Append(path.TrimEnd('/'));
      if (sb[sb.Length - 1] != '/')
        sb.Append('/');
      return sb.Append(path.Trim('/'));
    }

    public static StringBuilder AppendQueryParam(this StringBuilder sb, string key, string value)
    {
      return sb.Append(key).Append('=').Append(value);
    }
  }
}
