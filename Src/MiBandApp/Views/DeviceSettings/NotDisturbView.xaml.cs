/*using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace MiBandApp.Views.DeviceSettings
{
    public sealed partial class NotDisturbView : UserControl
    {
        public NotDisturbView()
        {
            this.InitializeComponent();
        }
    }
}*/


// Type: MiBandApp.Views.DeviceSettings.NotDisturbView
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// 

using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Globalization;
using Windows.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Markup;

#nullable disable
namespace MiBandApp.Views.DeviceSettings
{
    public sealed partial class NotDisturbView : UserControl
    {
       

        public NotDisturbView()
        {
            this.InitializeComponent();
            this.StartTimePicker.ClockIdentifier = this.ClockFormat;
            this.EndTimePicker.ClockIdentifier = this.StartTimePicker.ClockIdentifier;
        }

        public string ClockFormat
        {
            get
            {
                return !CultureInfo.CurrentUICulture.DateTimeFormat.ShortTimePattern.Contains("H")
                    ? ClockIdentifiers.TwelveHour : ClockIdentifiers.TwentyFourHour;
            }
        }

        private void OnSaveButtonTapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            // TODO : move it to Model
        }

        private void OnConfigureTapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            // TODO : move it to Model
        }
    }
}
