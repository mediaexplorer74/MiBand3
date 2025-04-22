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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MiBandApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }
    }
}*/
// Decompiled with JetBrains decompiler
// Type: MiBandApp.Views.MainPage
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.exe

using Caliburn.Micro;
using MiBandApp.Views.Tabs;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Navigation;

#nullable disable
namespace MiBandApp.Views
{
    public sealed partial class MainPage : Page
    {
       
        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;//1
        }

        public void ShowHeartRate(object viewModel)
        {
            if (((ICollection<object>)((ItemsControl)this.TabsIconsListView).Items).Count == 3)
                return;
            ListViewItem listViewItem = new ListViewItem();
            BitmapIcon bitmapIcon = new BitmapIcon();
            ((FrameworkElement)bitmapIcon).put_Style((Style)((IDictionary<object, object>)((FrameworkElement)this.TabsIconsListView).Resources)[(object)"HeartBitmapIconStyle"]);
            ((ContentControl)listViewItem).put_Content((object)bitmapIcon);
            ((ICollection<object>)((ItemsControl)this.TabsIconsListView).Items).Add((object)listViewItem);
            HeartRateTabView heartRateTabView = new HeartRateTabView();
            ViewModelBinder.Bind(viewModel, (DependencyObject)heartRateTabView, (object)null);
            PivotItem pivotItem = new PivotItem();
            ((ContentControl)pivotItem).put_Content((object)heartRateTabView);
            ((ICollection<object>)((ItemsControl)this.WalkSleepPivot).Items).Add((object)pivotItem);
        }

        public void HideHeartRate()
        {
            if (((ICollection<object>)((ItemsControl)this.TabsIconsListView).Items).Count == 2)
                return;
            ((IList<object>)((ItemsControl)this.TabsIconsListView).Items).RemoveAt(2);
            ((IList<object>)((ItemsControl)this.WalkSleepPivot).Items).RemoveAt(2);
        }

      
    }
}

