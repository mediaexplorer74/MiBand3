// Decompiled with JetBrains decompiler
// Type: MiBandApp.Converters.BoolToBoolConverter
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.exe

using System;
using Windows.UI.Xaml.Data;

#nullable disable
namespace MiBandApp.Converters
{
  public class BoolToBoolConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, string language)
    {
      bool flag = (bool) value;
      return (object) (bool) (bool.Parse((string) parameter) ? (flag ? 1 : 0) : (!flag ? 1 : 0));
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
      throw new NotImplementedException();
    }
  }
}
