
// Type: MiBandApp.Converters.BoolToBoolConverter
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// 

using System;
using System.Diagnostics;
using Windows.UI.Xaml.Data;

#nullable disable
namespace MiBandApp.Converters
{
    public class BoolToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool flag = (bool)value;
            bool param = default;

            try
            {
                param = bool.Parse((string)parameter);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[ex] BoolToBool error : " + ex.Message);
            }
            return param ? flag : !flag;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
