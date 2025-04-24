
// Type: Microsoft.Live.ServerResponseReader
// Assembly: Microsoft.Live, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 84D9FE74-951C-4BB4-80F8-EDD369EAC36D
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\Microsoft.Live.dll

using Microsoft.Live.Serialization;
using System;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.Live
{
  internal class ServerResponseReader
  {
    private static ServerResponseReader instance;

    private ServerResponseReader()
    {
    }

    public static ServerResponseReader Instance
    {
      get
      {
        return ServerResponseReader.instance ?? (ServerResponseReader.instance = new ServerResponseReader());
      }
    }

    public void Read(string response, IServerResponseReaderObserver observer)
    {
      using (JsonReader jsonReader = new JsonReader(response))
      {
        IDictionary<string, object> result;
        try
        {
          result = jsonReader.ReadValue() as IDictionary<string, object>;
        }
        catch (FormatException ex)
        {
          observer.OnInvalidJsonResponse(ex);
          return;
        }
        if (result.ContainsKey("error"))
        {
          IDictionary<string, object> dictionary = result["error"] as IDictionary<string, object>;
          string code = dictionary["code"] as string;
          string message = dictionary["message"] as string;
          observer.OnErrorResponse(code, message);
        }
        else
          observer.OnSuccessResponse(result, response);
      }
    }
  }
}
