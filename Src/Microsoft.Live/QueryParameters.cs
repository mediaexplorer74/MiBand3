// Decompiled with JetBrains decompiler
// Type: Microsoft.Live.QueryParameters
// Assembly: Microsoft.Live, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 84D9FE74-951C-4BB4-80F8-EDD369EAC36D
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\Microsoft.Live.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Live
{
  internal static class QueryParameters
  {
    public const string Method = "method";
    public const string Overwrite = "overwrite";
    public const string SuppressResponseCodes = "suppress_response_codes";
    public const string SuppressRedirects = "suppress_redirects";
    private static readonly Dictionary<OverwriteOption, string> uploadOptionToOverwriteValue = new Dictionary<OverwriteOption, string>();

    static QueryParameters()
    {
      QueryParameters.uploadOptionToOverwriteValue[OverwriteOption.Rename] = "choosenewname";
      QueryParameters.uploadOptionToOverwriteValue[OverwriteOption.Overwrite] = "true";
      QueryParameters.uploadOptionToOverwriteValue[OverwriteOption.DoNotOverwrite] = "false";
    }

    public static string GetOverwriteValue(OverwriteOption option)
    {
      return QueryParameters.uploadOptionToOverwriteValue[option];
    }
  }
}
