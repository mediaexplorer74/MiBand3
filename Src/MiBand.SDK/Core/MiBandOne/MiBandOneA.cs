
// Type: MiBand.SDK.Core.MiBandOne.MiBandOneA
// Assembly: MiBand.SDK, Version=0.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 152FA565-3249-4054-A361-01D9C0AFA7F1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBand.SDK.dll

using MiBand.SDK.Configuration;
using MiBand.SDK.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

#nullable disable
namespace MiBand.SDK.Core.MiBandOne
{
  internal class MiBandOneA(GattDeviceService gattService, ILog log) : MiBand.SDK.Core.MiBandOne.MiBandOne(gattService, log)
  {
    public override Task SetColorTheme(BandColorTheme colorTheme, bool flashLeds)
    {
      throw new NotImplementedException("This version of Mi Band doesn't support color changing.");
    }

    public override async Task SetUserInfo(UserInfo userInfo, bool withPairing)
    {
      BandDeviceInfo bandDeviceInfo = await this.GetBandDeviceInfo(true).ConfigureAwait(false);
      byte[] numArray = new byte[20];
      byte[] littleEndian = BytesUtils.LongToLittleEndian(userInfo.UserId);
      for (int index = 0; index < 4; ++index)
        numArray[index] = littleEndian[index];
      numArray[4] = userInfo.IsMale ? (byte) 1 : (byte) 0;
      numArray[5] = (byte) (DateTime.Now.Year - userInfo.Birthday.Year & (int) byte.MaxValue);
      numArray[6] = (byte) (userInfo.HeightCm & (int) byte.MaxValue);
      numArray[7] = (byte) (userInfo.WeightKg & (int) byte.MaxValue);
      numArray[8] = withPairing ? (byte) 1 : (byte) 0;
      numArray[9] = bandDeviceInfo.Feature;
      numArray[10] = bandDeviceInfo.Appearance;
      string str = userInfo.UserId.ToString("D8");
      for (int index = 0; index < 8; ++index)
        numArray[index + 11] = Convert.ToByte(str[index]);
      ulong bluetoothAddress = this.DefaultGattService.Device.BluetoothAddress;
      byte num = (byte) (((int) (byte) BytesUtils.GetCRC8(((IEnumerable<byte>) numArray).Take<byte>(19).ToArray<byte>()) ^ (int) bluetoothAddress) & (int) byte.MaxValue);
      numArray[19] = num;
      await this.WriteCharacteristic(this.GetCharacteristic(CharacteristicGuid.UserInfo), numArray).ConfigureAwait(false);
    }

    public override async Task<BindingResult> Bind(UserInfo userInfo)
    {
      BindingResult result = BindingResult.None;
      if (userInfo != (UserInfo) null)
        await this.SetUserInfo(userInfo, true).ConfigureAwait(false);
      CancellationTokenSource timeoutTaskCts = new CancellationTokenSource(25000);
      Task task = Task.Run((Func<Task>) (async () =>
      {
        while (!timeoutTaskCts.Token.IsCancellationRequested)
        {
          byte[] notify = await this.ReadCharacteristic(this.GetCharacteristic(CharacteristicGuid.Notify)).ConfigureAwait(false);
          if (notify != null && notify[0] != (byte) 9)
          {
            if (notify[0] == (byte) 19)
            {
              await Task.Delay(1000).ConfigureAwait(false);
            }
            else
            {
              if (notify[0] == (byte) 10)
              {
                result = BindingResult.Success;
                return;
              }
              notify = (byte[]) null;
            }
          }
          else
            break;
        }
        result = BindingResult.Fail;
      }), timeoutTaskCts.Token);
      try
      {
        await task.ConfigureAwait(false);
      }
      catch (TaskCanceledException ex)
      {
        result = BindingResult.Fail;
      }
      return result;
    }
  }
}
