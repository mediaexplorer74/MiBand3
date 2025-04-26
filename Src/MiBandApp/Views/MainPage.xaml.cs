

// Type: MiBandApp.Views.MainPage
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// 

using Caliburn.Micro;
using MiBandApp.Views.Tabs;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.UI.Core;
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

            Windows.UI.Core.SystemNavigationManager
               .GetForCurrentView().AppViewBackButtonVisibility
               = AppViewBackButtonVisibility.Visible;

            Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested += (s, a) =>
            {
                if (Frame.CanGoBack)
                {
                    Frame.GoBack();
                    a.Handled = true;
                }
            };


            this.NavigationCacheMode = NavigationCacheMode.Required;//1
        }

        public void ShowHeartRate(object viewModel)
        {
            //if (this.TabsIconsListView.Items.Count == 3)
            //    return;
            ListViewItem listViewItem = new ListViewItem();
            BitmapIcon bitmapIcon = new BitmapIcon();
            //TODO
            //bitmapIcon.Style = 
            //    ((Style)((IDictionary<object, object>)(this.TabsIconsListView).Resources)[(object)"HeartBitmapIconStyle"]);
            listViewItem.Content = ((object)bitmapIcon);
            //TODO
            //this.TabsIconsListView.Items.Add(listViewItem);
            HeartRateTabView heartRateTabView = new HeartRateTabView();
            ViewModelBinder.Bind(viewModel, (DependencyObject)heartRateTabView, (object)null);
            PivotItem pivotItem = new PivotItem();
            pivotItem.Content = heartRateTabView;
            //TODO
            //this.WalkSleepPivot.Items.Add((object)pivotItem);
        }

        public void HideHeartRate()
        {
            //TODO
            //if (this.TabsIconsListView.Items.Count == 2)
            //    return;
            //this.TabsIconsListView.Items.RemoveAt(2);
            //this.WalkSleepPivot.Items.RemoveAt(2);
        }

      
    }
}

