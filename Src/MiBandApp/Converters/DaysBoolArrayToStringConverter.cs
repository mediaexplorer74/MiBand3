
// Type: MiBandApp.Converters.DaysBoolArrayToStringConverter
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// 

using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Data;

#nullable disable
namespace MiBandApp.Converters
{
  public class DaysBoolArrayToStringConverter : IValueConverter
  {
    private readonly ResourceLoader _resourceLoader = new ResourceLoader();

    public object Convert(object value, Type targetType, object parameter, string language)
    {
      IReadOnlyList<bool> source1 = (IReadOnlyList<bool>) value;
      if (source1.All<bool>((Func<bool, bool>) (t => t)))
        return (object) this._resourceLoader.GetString("Days_EveryDay");
      if (source1.Take<bool>(5).All<bool>((Func<bool, bool>) (t => t)) && !source1.Skip<bool>(5).Any<bool>((Func<bool, bool>) (t => t)))
        return (object) this._resourceLoader.GetString("Days_Weekdays");
      if (!source1.Take<bool>(5).Any<bool>((Func<bool, bool>) (t => t)) && source1.Skip<bool>(5).All<bool>((Func<bool, bool>) (t => t)))
        return (object) this._resourceLoader.GetString("Days_Weekends");
      List<int> source2 = new List<int>(6);
      for (int index = 0; index < source1.Count; ++index)
      {
        if (source1[index])
          source2.Add(index);
      }
      if (source2.Count == 0)
        return (object) string.Empty;
      return source2.Count == 1 ? (object) this.GetDayString(source2[0], "dddd") : (object) string.Join(" ", source2.Select<int, string>((Func<int, string>) (t => this.GetDayString(t))));
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
      throw new NotImplementedException();
    }

    private string GetDayString(int dayNum, string format = "ddd")
    {
      return new DateTime(2015, 8, 24).AddDays((double) dayNum).ToString(format);
    }
  }
}
