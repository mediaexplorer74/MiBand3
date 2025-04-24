
// Type: MiBandApp.Tools.MonitorableSource`1
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// 

using System;
using System.Collections.Generic;

#nullable disable
namespace MiBandApp.Tools
{
  public class MonitorableSource<T>
  {
    private T _value;

    public MonitorableSource() => this.Monitorable = new MiBandApp.Tools.Monitorable<T>(this);

    public MiBandApp.Tools.Monitorable<T> Monitorable { get; }

    public T Value
    {
      get => this._value;
      set
      {
        T x = this._value;
        this._value = value;
        if (EqualityComparer<T>.Default.Equals(x, value))
          return;
        this.Updated((object) this, new MonitorableUpdatedEventArgs<T>(value));
      }
    }

    public event EventHandler<MonitorableUpdatedEventArgs<T>> Updated = (_param1, _param2) => { };

    public static explicit operator MonitorableSource<T>(T value)
    {
      return new MonitorableSource<T>() { _value = value };
    }
  }
}
