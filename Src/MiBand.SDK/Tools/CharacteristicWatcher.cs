// Decompiled with JetBrains decompiler
// Type: MiBand.SDK.Tools.CharacteristicWatcher
// Assembly: MiBand.SDK, Version=0.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 152FA565-3249-4054-A361-01D9C0AFA7F1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBand.SDK.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

#nullable disable
namespace MiBand.SDK.Tools
{
  internal class CharacteristicWatcher
  {
    private readonly GattCharacteristic _characteristic;
    private readonly int _pollingIntervalMs;
    private readonly Timer _timer;
    private byte[] _latestValue;
    private volatile bool _isEnabled;

    public CharacteristicWatcher(GattCharacteristic characteristic, int pollingIntervalMs = 1000)
    {
      this._characteristic = characteristic;
      this._pollingIntervalMs = pollingIntervalMs;
      this._timer = new Timer(new TimerCallback(this.OnTimerTick), (object) null, -1, -1);
    }

    public event EventHandler<CharacteristicWatcher.ValueChangedEventArgs> ValueChanged = (_param1, _param2) => { };

    public async void Enable(byte[] initialValue = null)
    {
      this._isEnabled = true;
      if (initialValue == null && this._latestValue == null)
        initialValue = await this.ReadValue().ConfigureAwait(false);
      this._latestValue = initialValue;
      this._timer.Change(this._pollingIntervalMs, -1);
    }

    public void Disable()
    {
      this._isEnabled = false;
      this._timer.Change(-1, -1);
    }

    private async void OnTimerTick(object state)
    {
      if (!this._isEnabled)
        return;
      try
      {
        byte[] first = await this.ReadValue().ConfigureAwait(false);
        bool flag = CharacteristicWatcher.NullRespectingSequenceEqual<byte>((IEnumerable<byte>) first, (IEnumerable<byte>) this._latestValue);
        try
        {
          if (!flag)
            this.ValueChanged((object) this, new CharacteristicWatcher.ValueChangedEventArgs(this._characteristic, first));
          this._latestValue = first;
        }
        finally
        {
          if (this._isEnabled)
            this._timer.Change(this._pollingIntervalMs, -1);
        }
      }
      catch
      {
      }
    }

    private async Task<byte[]> ReadValue()
    {
      try
      {
        GattReadResult gattReadResult = await this._characteristic.ReadValueAsync(
            BluetoothCacheMode.Uncached);
        return gattReadResult.Status != GattCommunicationStatus.AccessDenied 
                    ? gattReadResult.Value.ToArraySafe() 
                    : (byte[]) null;
      }
      catch
      {
        return (byte[]) null;
      }
    }

    private static bool NullRespectingSequenceEqual<T>(IEnumerable<T> first, IEnumerable<T> second)
    {
      if (first == null && second == null)
        return true;
      return first != null && second != null && first.SequenceEqual<T>(second);
    }

    public class ValueChangedEventArgs : EventArgs
    {
      public ValueChangedEventArgs(GattCharacteristic characteristic, byte[] value)
      {
        this.Characteristic = characteristic;
        this.Value = value;
      }

      public GattCharacteristic Characteristic { get; set; }

      public byte[] Value { get; set; }
    }
  }
}
