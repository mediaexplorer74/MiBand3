using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI.Core;

namespace MiCore
{
    public class CustomMiBandResult : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public enum BandOperation
        {
            Steps = 0,
            Heartrate = 1,
            Calories = 2,
            Sleep = 3,
            Distance = 4,
            Battery = 5,
            Notifications = 7,
            ClockAndDate = 9
        }

        public int Operation { get; set; }
        public string Value { get; set; }
        public string Unit { get; set; }
        public string PictureUrl { get; set; }
        public string Title { get; set; }
        public NotificationResult NotificationResult { get; set; }

        private ApplicationDataContainer LocalSettings = ApplicationData.Current.LocalSettings;

        public CustomMiBandResult()
        {
        }

        public CustomMiBandResult(BandOperation operation, string value, string title)
        {
            Operation = (int)operation;
            SetPictureOfOperation();
            SetUnitOfOperation();

            Value = value;
            Title = title;
        }

        public CustomMiBandResult(BandOperation operation)
        {
            Operation = (int)operation;
            SetPictureOfOperation();
            SetUnitOfOperation();
        }

        public void SetValue(string value)
        {
            Value = value;
            OnPropertyChanged(nameof(Value));
        }

        public void SetTitle(string title)
        {
            Title = title;
            OnPropertyChanged(nameof(Title));
        }

        private void SetPictureOfOperation()
        {
            var operation = (BandOperation)Operation;
            try
            {
                switch (operation)
                {
                    case BandOperation.Battery:
                        PictureUrl = "ms-appx:///Assets/Symbols/akku.png";
                        break;
                    case BandOperation.Calories:
                        PictureUrl = "ms-appx:///Assets/Symbols/cals.png";
                        break;
                    case BandOperation.Distance:
                        PictureUrl = "ms-appx:///Assets/Symbols/distance.png";
                        break;
                    case BandOperation.Heartrate:
                        PictureUrl = "ms-appx:///Assets/Symbols/rate.png";
                        break;
                    case BandOperation.Sleep:
                        PictureUrl = "ms-appx:///Assets/Symbols/sleep.png";
                        break;
                    case BandOperation.Steps:
                        PictureUrl = "ms-appx:///Assets/Symbols/steps.png";
                        break;
                    case BandOperation.Notifications:
                        PictureUrl = "ms-appx:///Assets/Symbols/message.png";
                        break;
                }
            }
            catch (Exception)
            {
            }
        }

        private void SetUnitOfOperation()
        {
            var operation = (BandOperation)Operation;
            try
            {
                switch (operation)
                {
                    case BandOperation.Battery:
                        Unit = "%";
                        break;
                    case BandOperation.Calories:
                        Unit = "cals";
                        break;
                    case BandOperation.Distance:
                        Unit = "meter";
                        break;
                    case BandOperation.Heartrate:
                        Unit = "bpm";
                        break;
                    case BandOperation.Sleep:
                        Unit = "hours";
                        break;
                    case BandOperation.Steps:
                        Unit = "steps";
                        break;
                    case BandOperation.Notifications:
                        Unit = "active";
                        break;
                    default:
                        Unit = "";
                        break;
                }
            }
            catch (Exception)
            {
            }
        }

        public async void OnPropertyChanged(string propertyName)
        {
            await CoreApplication.Views.First().Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            });
        }
    }
}

