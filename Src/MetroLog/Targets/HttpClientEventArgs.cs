// Decompiled with JetBrains decompiler
// Type: MetroLog.Targets.HttpClientEventArgs
// Assembly: MetroLog, Version=0.0.0.0, Culture=neutral, PublicKeyToken=ba4ace74c3b410f3
// MVID: 4C1C3979-5A8D-451B-A09E-C87C946DE5D1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MetroLog.dll

using System;
using System.Net.Http;

#nullable disable
namespace MetroLog.Targets
{
  public class HttpClientEventArgs : EventArgs
  {
    public HttpClient Client { get; private set; }

    public HttpClientEventArgs(HttpClient client)
    {
    }
  }
}
