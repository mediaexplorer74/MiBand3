// Decompiled with JetBrains decompiler
// Type: MiBandApp.Converters.CaloriesToFormattedStringConverter
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.exe

using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Data;

#nullable disable
namespace MiBandApp.Converters
{
  public class CaloriesToFormattedStringConverter : IValueConverter
  {
    private readonly ResourceLoader _resourceLoader = new ResourceLoader();

    public object Convert(object value, Type targetType, object parameter, string language)
    {
      int num = (int) value;
      return num < 1000 ? (object) (num.ToString() + " " + this._resourceLoader.GetString("MainPageCalories")) : (object) (((double) num / 1000.0).ToString("F0") + " " + this._resourceLoader.GetString("Units_Kcal"));
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
      throw new NotImplementedException();
    }
  }
}
