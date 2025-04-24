
// Type: MiBand.SDK.Bluetooth.IBluetoothDevice
// Assembly: MiBand.SDK, Version=0.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 152FA565-3249-4054-A361-01D9C0AFA7F1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBand.SDK.dll

using System;

#nullable disable
namespace MiBand.SDK.Bluetooth
{
  public interface IBluetoothDevice : IDisposable
  {
    event EventHandler ConnectionStatusChanged;

    ConnectionStatus ConnectionStatus { get; }

    string DeviceId { get; }
  }
}
