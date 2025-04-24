
// Type: MetroLog.ExceptionWrapper
// Assembly: MetroLog, Version=0.0.0.0, Culture=neutral, PublicKeyToken=ba4ace74c3b410f3
// MVID: 4C1C3979-5A8D-451B-A09E-C87C946DE5D1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MetroLog.dll

using MetroLog.Internal;
using System;

#nullable disable
namespace MetroLog
{
  public class ExceptionWrapper
  {
    public string TypeName { get; set; }

    public string AsString { get; set; }

    public int Hresult { get; set; }

    public ExceptionWrapper()
    {
    }

    internal ExceptionWrapper(Exception ex)
    {
      this.TypeName = ex.GetType().AssemblyQualifiedName;
      this.AsString = ex.ToString();
      this.Hresult = ex.HResult;
    }

    public string ToJson() => SimpleJson.SerializeObject((object) this);
  }
}
