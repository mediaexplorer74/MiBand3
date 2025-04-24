
// Type: MiBandApp.Converters.DistanceKmToFormattedStringConverter
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// 

using Caliburn.Micro;
using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Data;

#nullable disable
namespace MiBandApp.Converters
{
  public class DistanceKmToFormattedStringConverter : IValueConverter
  {
    private readonly ResourceLoader _resourceLoader = new ResourceLoader();

    public object Convert(object value, Type targetType, object parameter, string language)
    {
      MiBandApp.Storage.Settings.Settings settings = IoC.Get<MiBandApp.Storage.Settings.Settings>();
      double num = (double) value;
      if (settings.DistanceUnits == MiBandApp.Storage.Settings.Settings.DistanceUnit.Km)
        return (object) (num.ToString("F2") + " " + this._resourceLoader.GetString("MainPageKilometers"));
      if (settings.DistanceUnits == MiBandApp.Storage.Settings.Settings.DistanceUnit.Mi)
        return (object) ((num * 0.621).ToString("F2") + " " + this._resourceLoader.GetString("MainPageMiles"));
      throw new NotImplementedException();
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
      throw new NotImplementedException();
    }
  }
}
