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
    public sealed partial class ActivityReminderView : UserControl
    {
        public ActivityReminderView()
        {
            this.InitializeComponent();
        }
    }
}*/
// Decompiled with JetBrains decompiler
// Type: MiBandApp.Views.DeviceSettings.ActivityReminderView
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.exe

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
        /*[GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
        private ToggleSwitch ActivityReminderToggleSwitch;
        [GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
        private StackPanel ActivityReminderConfigureContentDialog;
        [GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
        private TimePicker StartTimePicker;
        [GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
        private TimePicker EndTimePicker;*/
 
        public ActivityReminderView()
        {
            this.InitializeComponent();
            this.StartTimePicker.put_ClockIdentifier(this.ClockFormat);
            this.EndTimePicker.put_ClockIdentifier(this.StartTimePicker.ClockIdentifier);
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
            ((ActivityReminderDialogViewModel)((FrameworkElement)this.ActivityReminderConfigureContentDialog).DataContext).IsVisible = true;
        }

    
    }
}

