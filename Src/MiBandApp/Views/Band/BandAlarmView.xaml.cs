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

namespace MiBandApp.Views.Band
{
    public sealed partial class BandAlarmView : UserControl
    {
        public BandAlarmView()
        {
            this.InitializeComponent();
        }
    }
}*/

// Type: MiBandApp.Views.Band.BandAlarmView
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
namespace MiBandApp.Views.Band
{
    public sealed partial class BandAlarmView : UserControl
    {
        /*[GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
        private Border DataTemplateRoot;
        [GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
        private TimePicker AlarmTimePicker;
        [GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
        private ToggleSwitch StateSwitch;
        [GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
        private CheckBox IsSmartCheckBox;
        [GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
        private CheckBox RepeatCheckBox;
        [GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
        private ToggleButton Mo;
        [GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
        private ToggleButton Tu;
        [GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
        private ToggleButton We;
        [GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
        private ToggleButton Th;
        [GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
        private ToggleButton Fr;
        [GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
        private ToggleButton Sa;
        [GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
        private ToggleButton Su;*/
      
        public BandAlarmView()
        {
            this.InitializeComponent();
            this.AlarmTimePicker.ClockIdentifier = this.ClockFormat;
        }

        public string ClockFormat
        {
            get
            {
                return !CultureInfo.CurrentUICulture.DateTimeFormat.ShortTimePattern.Contains("H") 
                    ? ClockIdentifiers.TwelveHour
                    : ClockIdentifiers.TwentyFourHour;
            }
        }
      
    }
}

