using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace MiCore
{
    public class GetAppLogo : IValueConverter
    {
        public object Convert(Object value, Type targetType, Object parameter, String language)
        {
            try
            {
                // Retrieve the app logo asynchronously
                var task = Task.Run(async () =>
                {
                    return await Helpers.GetAppLogoById(value.ToString());
                });

                task.Wait();

                var bitmap = new BitmapImage();
                bitmap.SetSource(task.Result);

                return bitmap;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public object ConvertBack(Object value, Type targetType, Object parameter, String language)
        {
            throw new NotImplementedException();
        }
    }
}



