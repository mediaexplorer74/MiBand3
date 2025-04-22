// Decompiled with JetBrains decompiler
// Type: MiBandApp.Tools.ColorExtensions
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.exe

using System;
using Windows.UI;

#nullable disable
namespace MiBandApp.Tools
{
  public static class ColorExtensions
  {
    public static Color Darken(this Color colour, float amount)
    {
      return colour.Lerp(Colors.Black, amount);
    }

    public static Color Lerp(this Color colour, Color to, float amount)
    {
      double r1 = (double) colour.R;
      float g1 = (float) colour.G;
      float b1 = (float) colour.B;
      float r2 = (float) to.R;
      float g2 = (float) to.G;
      float b2 = (float) to.B;
      double end = (double) r2;
      double amount1 = (double) amount;
      return Color.FromArgb(byte.MaxValue, (byte) ((float) r1).Lerp((float) end, (float) amount1), (byte) g1.Lerp(g2, amount), (byte) b1.Lerp(b2, amount));
    }

    private static float Lerp(this float start, float end, float amount)
    {
      float num = (end - start) * amount;
      return start + num;
    }

    public static Color DarkenHsl(this Color color, double amount)
    {
      double h;
      double s;
      double l1;
      ColorExtensions.RGB2HSL(color, out h, out s, out l1);
      double l2 = l1 * (1.0 - amount);
      return ColorExtensions.HSL2RGB(h, s, l2);
    }

    public static Color HSL2RGB(double h, double sl, double l)
    {
      double num1 = l;
      double num2 = l;
      double num3 = l;
      double num4 = l <= 0.5 ? l * (1.0 + sl) : l + sl - l * sl;
      if (num4 > 0.0)
      {
        double num5 = l + l - num4;
        double num6 = (num4 - num5) / num4;
        h *= 6.0;
        int num7 = (int) h;
        double num8 = h - (double) num7;
        double num9 = num4 * num6 * num8;
        double num10 = num5 + num9;
        double num11 = num4 - num9;
        switch (num7)
        {
          case 0:
            num1 = num4;
            num2 = num10;
            num3 = num5;
            break;
          case 1:
            num1 = num11;
            num2 = num4;
            num3 = num5;
            break;
          case 2:
            num1 = num5;
            num2 = num4;
            num3 = num10;
            break;
          case 3:
            num1 = num5;
            num2 = num11;
            num3 = num4;
            break;
          case 4:
            num1 = num10;
            num2 = num5;
            num3 = num4;
            break;
          case 5:
            num1 = num4;
            num2 = num5;
            num3 = num11;
            break;
        }
      }
      Color color;
      color.A = byte.MaxValue;
      color.R = Convert.ToByte(num1 * (double) byte.MaxValue);
      color.G = Convert.ToByte(num2 * (double) byte.MaxValue);
      color.B = Convert.ToByte(num3 * (double) byte.MaxValue);
      return color;
    }

    public static void RGB2HSL(Color rgb, out double h, out double s, out double l)
    {
      double val1 = (double) rgb.R / (double) byte.MaxValue;
      double val2_1 = (double) rgb.G / (double) byte.MaxValue;
      double val2_2 = (double) rgb.B / (double) byte.MaxValue;
      h = 0.0;
      s = 0.0;
      l = 0.0;
      double num1 = Math.Max(Math.Max(val1, val2_1), val2_2);
      double num2 = Math.Min(Math.Min(val1, val2_1), val2_2);
      l = (num2 + num1) / 2.0;
      if (l <= 0.0)
        return;
      double num3 = num1 - num2;
      s = num3;
      if (s <= 0.0)
        return;
      s /= l <= 0.5 ? num1 + num2 : 2.0 - num1 - num2;
      double num4 = (num1 - val1) / num3;
      double num5 = (num1 - val2_1) / num3;
      double num6 = (num1 - val2_2) / num3;
      h = val1 != num1 ? (val2_1 != num1 ? (val1 == num2 ? 3.0 + num5 : 5.0 - num4) : (val2_2 == num2 ? 1.0 + num4 : 3.0 - num6)) : (val2_1 == num2 ? 5.0 + num6 : 1.0 - num5);
      h /= 6.0;
    }
  }
}
