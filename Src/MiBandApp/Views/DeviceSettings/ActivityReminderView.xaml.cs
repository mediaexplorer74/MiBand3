
// Type: MiBandApp.Views.DeviceSettings.ActivityReminderView
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// 

using MiBandApp.ViewModels.Dialogs;
using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;

#nullable disable
namespace MiBandApp.Views.DeviceSettings
{
    public sealed partial class ActivityReminderView : UserControl
    {
        
        public ActivityReminderView()
        {
            this.InitializeComponent();
            //TODO
            //this.StartTimePicker.ClockIdentifier = this.ClockFormat;
            //this.EndTimePicker.ClockIdentifier = this.StartTimePicker.ClockIdentifier;
        }

        public string ClockFormat
        {
            get
            {
                return !CultureInfo.CurrentUICulture.DateTimeFormat.ShortTimePattern.Contains("H") ? ClockIdentifiers.TwelveHour : ClockIdentifiers.TwentyFourHour;
            }
        }

        private async void OnAcitivityReminderConfigureTapped(object sender, TappedRoutedEventArgs e)
        {
            //TODO
            //this.ActivityReminderConfigureContentDialog.DataContext.IsVisible = true;
        }

    
    }
}

