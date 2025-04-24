
// Type: MiBandApp.Converters.MinutesToStringConverter
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// 

using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Data;

#nullable disable
namespace MiBandApp.Converters
{
  public class MinutesToStringConverter : IValueConverter
  {
    private readonly ResourceLoader _stringsLoader;

    public MinutesToStringConverter() => this._stringsLoader = new ResourceLoader();

    public object Convert(object value, Type targetType, object parameter, string language)
    {
      int result1 = 0;
      if (value is double num1)
        result1 = (int) num1;
      else if (!(value is int) && value is string && !int.TryParse((string) value, out result1))
      {
        double result;
        if (!double.TryParse((string) value, out result))
          throw new InvalidOperationException();
        result1 = (int) result;
      }
      int num2 = result1 >= 0 ? result1 : 0;
      int num3 = num2 / 60;
      int num4 = num2 % 60;
      bool flag = false;
      if (parameter != null)
      {
        string str = (string) parameter;
        if (str.Equals("short", StringComparison.OrdinalIgnoreCase))
          flag = true;
        if (str.Equals("HoursOnly", StringComparison.OrdinalIgnoreCase))
          return (object) num3;
        if (str.Equals("MinutesOnly", StringComparison.OrdinalIgnoreCase))
          return (object) num4;
        if (str.Equals("HoursText", StringComparison.OrdinalIgnoreCase))
          return (object) this._stringsLoader.GetString("Duration_Hour_Short");
        if (str.Equals("MinutesText", StringComparison.OrdinalIgnoreCase))
          return (object) this._stringsLoader.GetString("Duration_Minute_Short");
      }
      if (flag && num3 == 0)
        return (object) (num4.ToString() + this._stringsLoader.GetString("Duration_Minute"));
      return (object) (num3.ToString() + this._stringsLoader.GetString("Duration_Hour_Short") + " " + (object) num4 + this._stringsLoader.GetString("Duration_Minute_Short"));
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
      throw new NotImplementedException();
    }
  }
}
