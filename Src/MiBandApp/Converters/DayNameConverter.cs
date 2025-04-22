// Decompiled with JetBrains decompiler
// Type: MiBandApp.Converters.DayNameConverter
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.exe

using System;
using Windows.UI.Xaml.Data;

#nullable disable
namespace MiBandApp.Converters
{
  public class DayNameConverter : IValueConverter
  {
    private readonly StringFormatConverter _formatConverter;

    public DayNameConverter() => this._formatConverter = new StringFormatConverter();

    public object Convert(object value, Type targetType, object parameter, string language)
    {
      return value == null ? (object) null : this._formatConverter.Convert((object) new DateTime(2015, 8, 24).AddDays((double) int.Parse((string) value)), targetType, parameter, language);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
      throw new NotImplementedException();
    }
  }
}
