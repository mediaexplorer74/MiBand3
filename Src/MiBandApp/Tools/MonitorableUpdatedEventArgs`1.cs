
// Type: MiBandApp.Tools.MonitorableUpdatedEventArgs`1
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// 

using System;

#nullable disable
namespace MiBandApp.Tools
{
  public class MonitorableUpdatedEventArgs<T> : EventArgs
  {
    public MonitorableUpdatedEventArgs(T updatedValue) => this.UpdatedValue = updatedValue;

    public T UpdatedValue { get; }
  }
}
