
// Type: MiBandApp.Converters.StringFormatConverter
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// 

using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Data;

#nullable disable
namespace MiBandApp.Converters
{
  public sealed class StringFormatConverter : IValueConverter
  {
    private readonly ResourceLoader _resourceLoader;

    public StringFormatConverter() => this._resourceLoader = new ResourceLoader();

    public object Convert(object value, Type targetType, object parameter, string language)
    {
      if (value == null)
        return (object) null;
      if (parameter == null)
        return value;
      string format = ((string) parameter).Replace("nl", Environment.NewLine).Replace("sp", " ");
      if (format.Contains("steps"))
        format = format.Replace("steps", this._resourceLoader.GetString("UserInfoPageSteps"));
      else if (format.Contains("cm"))
        format = format.Replace("cm", this._resourceLoader.GetString("UserInfoPageCentimeters"));
      else if (format.Contains("kg"))
        format = format.Replace("kg", this._resourceLoader.GetString("UserInfoPageKilogram"));
      else if (format.Contains("bpm"))
        format = format.Replace("bpm", this._resourceLoader.GetString("Units_Bpm"));
      return (object) string.Format(format, value);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
      throw new NotImplementedException();
    }
  }
}
