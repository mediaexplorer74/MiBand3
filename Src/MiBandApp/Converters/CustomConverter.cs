// Decompiled with JetBrains decompiler
// Type: MiBandApp.Converters.CustomConverter
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.exe

using System;
using System.Collections.Generic;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

#nullable disable
namespace MiBandApp.Converters
{
  public class CustomConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, string language)
    {
      switch ((string) parameter)
      {
        case "DeviceWarningToColor":
          return this.ConvertDeviceWarningToColor(value);
        case "HistoryPageNavigationButtons":
          return (object) this.ConvertIsDisplayingSleepToColor(value);
        default:
          return (object) null;
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
      throw new NotImplementedException();
    }

    private object ConvertDeviceWarningToColor(object value)
    {
      return (bool) value ? (object) new SolidColorBrush(Colors.Red) : (object) (SolidColorBrush) ((IDictionary<object, object>) Application.Current.Resources)[(object) "AppForegroundDisabledBrush"];
    }

    private Brush ConvertIsDisplayingSleepToColor(object value)
    {
      return (bool) value ? (Brush) ((IDictionary<object, object>) Application.Current.Resources)[(object) "AppSleepBrush"] : (Brush) ((IDictionary<object, object>) Application.Current.Resources)[(object) "AppAccentBrush"];
    }
  }
}
