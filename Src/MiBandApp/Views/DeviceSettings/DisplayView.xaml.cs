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
    public sealed partial class DisplayView : UserControl
    {
        public DisplayView()
        {
            this.InitializeComponent();
        }
    }
}*/

// Type: MiBandApp.Views.DeviceSettings.DisplayView
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// 

using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;

#nullable disable
namespace MiBandApp.Views.DeviceSettings
{
    public sealed partial class DisplayView : UserControl
    {
        private bool _dialogShown;
        
        public DisplayView() => this.InitializeComponent();

        private async void OnDisplayItemsTapped(object sender, TappedRoutedEventArgs e)
        {
            lock (this.DisplayItemsContentDialog)
            {
                if (this._dialogShown)
                    return;
                this._dialogShown = true;
            }
            try
            {
                ContentDialogResult contentDialogResult = await this.DisplayItemsContentDialog.ShowAsync();
            }
            finally
            {
                this._dialogShown = false;
            }
        }

        private void SaveDisplayItems(ContentDialog sender, ContentDialogClosedEventArgs args)
        {
            // Move it to Model
        }
    }
}

