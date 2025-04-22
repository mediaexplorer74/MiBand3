// Decompiled with JetBrains decompiler
// Type: MiBand.SDK.Tools.MiBand2Tools
// Assembly: MiBand.SDK, Version=0.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 152FA565-3249-4054-A361-01D9C0AFA7F1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBand.SDK.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace MiBand.SDK.Tools
{
  internal static class MiBand2Tools
  {
    public static byte[] DateTimeOffsetToBytes(
      DateTimeOffset time,
      int insertZerosBeforeTz = 0,
      bool insertDayOfWeek = false)
    {
      int year = time.Year;
      int month = time.Month;
      int day = time.Day;
      int hour = time.Hour;
      int minute = time.Minute;
      int second = time.Second;
      byte num1 = (byte) (time.DayOfWeek + 1);
      sbyte num2 = (sbyte) (time.Offset.TotalMinutes / 15.0);
      List<byte> byteList = new List<byte>()
      {
        (byte) (year & (int) byte.MaxValue),
        (byte) (year >> 8),
        (byte) month,
        (byte) day,
        (byte) hour,
        (byte) minute,
        (byte) second
      };
      if (insertDayOfWeek)
        byteList.Add(num1);
      while (insertZerosBeforeTz-- > 0)
        byteList.Add((byte) 0);
      byteList.Add((byte) num2);
      return byteList.ToArray();
    }

    public static DateTimeOffset DateTimeOffsetFromBytes(byte[] bytes, int index)
    {
      return MiBand2Tools.DateTimeOffsetFromBytes(bytes, ref index);
    }

    public static DateTimeOffset DateTimeOffsetFromBytes(byte[] bytes, ref int index)
    {
      int year = (int) bytes[index] | (int) bytes[index + 1] << 8;
      index += 2;
      byte month = bytes[index++];
      byte day = bytes[index++];
      byte hour = bytes[index++];
      byte minute = bytes[index++];
      byte second = bytes[index++];
      TimeSpan offset = MiBand2Tools.OffsetFromTimeZoneCode((sbyte) bytes[index++]);
      return new DateTimeOffset(year, (int) month, (int) day, (int) hour, (int) minute, (int) second, offset);
    }

    public static DateTimeOffset UnixTimestampToDateTimeOffset(
      double unixTimestamp,
      sbyte timeZoneCode)
    {
      TimeSpan timeSpan = MiBand2Tools.OffsetFromTimeZoneCode(timeZoneCode);
      return new DateTimeOffset(1970, 1, 1, 0, 0, 0, 0, timeSpan).AddSeconds(unixTimestamp).Add(timeSpan);
    }

    private static TimeSpan OffsetFromTimeZoneCode(sbyte timeZoneCode)
    {
      int num1;
      if (timeZoneCode == sbyte.MinValue)
      {
        num1 = 480;
      }
      else
      {
        int num2 = timeZoneCode < (sbyte) 0 ? -1 : 1;
        int num3 = (int) Math.Abs(timeZoneCode);
        num1 = num2 * (num3 % 4 * 15 + num3 / 4 * 60);
      }
      return TimeSpan.FromMinutes((double) num1);
    }
  }
}
