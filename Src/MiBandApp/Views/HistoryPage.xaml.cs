
// Type: MiBandApp.Views.HistoryPage
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// 

using MiBandApp.ViewModels.History;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Navigation;
//using WinRTXamlToolkit.Controls.DataVisualization.Charting;

#nullable disable
namespace MiBandApp.Views
{
    public sealed partial class HistoryPage : Page
    {
        private HistoryWeekViewModel _week;
        private bool _isSleep;
     

        public HistoryPage()
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



            this.NavigationCacheMode = NavigationCacheMode.Required;
            //TODO
            //this.SleepChart.Axes[0].Minimum = new double?(0.0);
            //this.WalkChart.Axes[0].Minimum = new double?(0.0);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        public void SetItemSource(HistoryWeekViewModel week)
        {
            this._week = week;
            this.UpdateCharts();
        }

        public void ChangeViewMode(bool isSleep)
        {
            this._isSleep = isSleep;
            if (this._isSleep)
            {
                //this.WalkChart.Width = 0.0;
                //this.SleepChart.Width = double.NaN;
                //this.DaysListView.ItemTemplate = 
                //    this.DownFlipViewItem.Resources[(object)"SleepItemDataTemplate"];
            }
            else
            {
                //this.SleepChart.Width = (0.0);
                //this.WalkChart.Width = (double.NaN);
                //this.DaysListView.ItemTemplate = this.DownFlipViewItem.Resources["WalkItemDataTemplate"];
            }
            this.UpdateCharts();
        }

        private void UpdateCharts()
        {
            if (this._week == null)
                return;
            List<DaySummaryViewModel> days = this._week.Days;
            //SeriesDefinition seriesDefinition1 = this.SleepChart.Series[0].SeriesDefinitions[0];
            //SeriesDefinition seriesDefinition2 = this.SleepChart.Series[0].SeriesDefinitions[1];
            //ColumnSeries columnSeries = this.WalkChart.Series[0];
            //if (this._isSleep && seriesDefinition1.ItemsSource != days)
            //{
            //    seriesDefinition1.ItemsSource = (IEnumerable)days;
            //    seriesDefinition2.ItemsSource = (IEnumerable)days;
            //}
            //if (this._isSleep || columnSeries.ItemsSource == days)
            //    return;
            //columnSeries.ItemsSource = (IEnumerable)this._week.Days;
        }

    
    }
}

