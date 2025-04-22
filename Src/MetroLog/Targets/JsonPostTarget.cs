// Decompiled with JetBrains decompiler
// Type: MetroLog.Targets.JsonPostTarget
// Assembly: MetroLog, Version=0.0.0.0, Culture=neutral, PublicKeyToken=ba4ace74c3b410f3
// MVID: 4C1C3979-5A8D-451B-A09E-C87C946DE5D1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MetroLog.dll

using MetroLog.Layouts;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

#nullable disable
namespace MetroLog.Targets
{
  public class JsonPostTarget : BufferedTarget
  {
    public Uri Url { get; private set; }

    public event EventHandler<HttpClientEventArgs> BeforePost;

    public JsonPostTarget(int threshold, Uri uri)
      : this((Layout) new NullLayout(), threshold, uri)
    {
    }

    public JsonPostTarget(Layout layout, int threshold, Uri url)
      : base(layout, threshold)
    {
      this.Url = url;
    }

    protected override async Task DoFlushAsync(
      LogWriteContext context,
      IEnumerable<LogEventInfo> toFlush)
    {
      string json = new JsonPostWrapper((ILoggingEnvironment) new LoggingEnvironment(), toFlush).ToJson();
      HttpClient client = new HttpClient();
      StringContent content = new StringContent(json);
      content.Headers.ContentType.MediaType = "text/json";
      this.OnBeforePost(new HttpClientEventArgs(client));
      HttpResponseMessage httpResponseMessage = await client.PostAsync(this.Url, (HttpContent) content);
    }

    protected virtual void OnBeforePost(HttpClientEventArgs args)
    {
      EventHandler<HttpClientEventArgs> beforePost = this.BeforePost;
      if (beforePost == null)
        return;
      beforePost((object) this, args);
    }
  }
}
