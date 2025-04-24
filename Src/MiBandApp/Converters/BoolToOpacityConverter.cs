
// Type: MiBandApp.Converters.BoolToOpacityConverter
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// 

using System;
using System.Globalization;
using Windows.UI.Xaml.Data;

#nullable disable
namespace MiBandApp.Converters
{
  public class BoolToOpacityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, string language)
    {
      bool flag = (bool) value;
      double num1 = double.Parse((string) parameter, (IFormatProvider) CultureInfo.InvariantCulture);
      if (num1 < 0.0)
        flag = !flag;
      int num2;
      double num3 = (double) (num2 = (int) Math.Abs(num1)) - (double) num2;
      return (object) (flag ? (double) num2 : num3);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
      throw new NotImplementedException();
    }
  }
}
