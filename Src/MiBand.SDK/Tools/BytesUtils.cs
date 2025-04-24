
// Type: MiBand.SDK.Tools.BytesUtils
// Assembly: MiBand.SDK, Version=0.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 152FA565-3249-4054-A361-01D9C0AFA7F1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBand.SDK.dll

#nullable disable
namespace MiBand.SDK.Tools
{
  internal static class BytesUtils
  {
    public static int GetCRC8(byte[] data)
    {
      int crC8 = 0;
      foreach (byte num in data)
      {
        crC8 ^= (int) num & (int) byte.MaxValue;
        for (int index = 0; index < 8; ++index)
        {
          if ((crC8 & 1) == 1)
            crC8 = crC8 >> 1 ^ 140;
          else
            crC8 >>= 1;
        }
      }
      return crC8;
    }

    public static int GetCRC16(byte[] data)
    {
      int num1 = (int) ushort.MaxValue;
      for (int index = 0; index < data.Length; ++index)
      {
        int num2 = (num1 >> 8 | num1 << 8) & (int) ushort.MaxValue ^ (int) data[index] & (int) byte.MaxValue;
        int num3 = num2 ^ (num2 & (int) byte.MaxValue) >> 4;
        int num4 = num3 ^ num3 << 12 & (int) ushort.MaxValue;
        num1 = num4 ^ (num4 & (int) byte.MaxValue) << 5 & (int) ushort.MaxValue;
      }
      return num1 & (int) ushort.MaxValue;
    }

    public static byte[] LongToLittleEndian(long data)
    {
      return new byte[4]
      {
        (byte) data,
        (byte) ((ulong) (data >>> 8) & (ulong) byte.MaxValue),
        (byte) ((ulong) (data >>> 16) & (ulong) byte.MaxValue),
        (byte) ((ulong) (data >>> 24) & (ulong) byte.MaxValue)
      };
    }

    public static long LittleEndianToLong(byte[] data, int index = 0)
    {
      return (long) ((int) data[index] + ((int) data[index + 1] << 8) + ((int) data[index + 2] << 16) + ((int) data[index + 3] << 24));
    }

    public static int BigEndianToInt(byte[] data, int index = 0)
    {
      return ((int) data[index] << 24) + ((int) data[index + 1] << 16) + ((int) data[index + 2] << 8) + (int) data[index + 3];
    }

    public static byte[] GetMacAddressOctets(ulong macAddress)
    {
      return new byte[6]
      {
        (byte) (macAddress >> 40 & (ulong) byte.MaxValue),
        (byte) (macAddress >> 32 & (ulong) byte.MaxValue),
        (byte) (macAddress >> 24 & (ulong) byte.MaxValue),
        (byte) (macAddress >> 16 & (ulong) byte.MaxValue),
        (byte) (macAddress >> 8 & (ulong) byte.MaxValue),
        (byte) (macAddress & (ulong) byte.MaxValue)
      };
    }

    public static byte ToByte(this int value) => (byte) value;

    public static byte ToByte(this bool value, byte trueValue = 1, byte falseValue = 0)
    {
      return !value ? falseValue : trueValue;
    }
  }
}
