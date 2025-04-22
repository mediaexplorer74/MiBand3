// Decompiled with JetBrains decompiler
// Type: Microsoft.Live.WebRequestFactory
// Assembly: Microsoft.Live, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 84D9FE74-951C-4BB4-80F8-EDD369EAC36D
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\Microsoft.Live.dll

using System;
using System.Net;

#nullable disable
namespace Microsoft.Live
{
  internal class WebRequestFactory : IWebRequestFactory
  {
    private static IWebRequestFactory requestFactory;

    private WebRequestFactory()
    {
    }

    public static IWebRequestFactory Current
    {
      get
      {
        return WebRequestFactory.requestFactory ?? (WebRequestFactory.requestFactory = (IWebRequestFactory) new WebRequestFactory());
      }
      set => WebRequestFactory.requestFactory = value;
    }

    public WebRequest CreateWebRequest(Uri url, string method)
    {
      HttpWebRequest webRequest = WebRequest.Create((Uri) url) as HttpWebRequest;
      webRequest.Method = method;
      webRequest.Accept = "*/*";
      return (WebRequest) webRequest;
    }
  }
}
