
// Type: MiBandApp.Tools.Monitorable`1
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// 

using System;

#nullable disable
namespace MiBandApp.Tools
{
  public class Monitorable<T>
  {
    private readonly MonitorableSource<T> _source;

    public Monitorable(MonitorableSource<T> source) => this._source = source;

    public T Value => this._source.Value;

    public event EventHandler<MonitorableUpdatedEventArgs<T>> Updated
    {
      add => this._source.Updated += value;
      remove => this._source.Updated -= value;
    }
  }
}
