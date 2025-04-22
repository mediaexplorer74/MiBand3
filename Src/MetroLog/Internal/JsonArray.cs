// Decompiled with JetBrains decompiler
// Type: MetroLog.Internal.JsonArray
// Assembly: MetroLog, Version=0.0.0.0, Culture=neutral, PublicKeyToken=ba4ace74c3b410f3
// MVID: 4C1C3979-5A8D-451B-A09E-C87C946DE5D1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MetroLog.dll

using System.Collections.Generic;
using System.ComponentModel;

#nullable disable
namespace MetroLog.Internal
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal class JsonArray : List<object>
  {
    public JsonArray()
    {
    }

    public JsonArray(int capacity)
      : base(capacity)
    {
    }

    public override string ToString() => SimpleJson.SerializeObject((object) this) ?? string.Empty;
  }
}
