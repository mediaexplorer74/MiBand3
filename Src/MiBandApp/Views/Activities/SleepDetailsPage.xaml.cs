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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MiBandApp.Views.Activities
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SleepDetailsPage : Page
    {
        public SleepDetailsPage()
        {
            this.InitializeComponent();
        }
    }
}*/

// Type: MiBandApp.Views.Activities.SleepDetailsPage
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// 

using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Navigation;
//using WinRTXamlToolkit.Controls.DataVisualization.Charting;

#nullable disable
namespace MiBandApp.Views.Activities
{
    public sealed partial class SleepDetailsPage : Page
    {
        /*[GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
        private Chart PhasesChart;
        [GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
        private LinearAxis VerticalAxis;
        [GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
        private AreaSeries LightSleepSeries;
        [GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
        private AreaSeries DeepSleepSeries;
        [GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
        private AreaSeries AwakeningsSleepSeries;
        [GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
        private LineSeries HeartRateSeries;*/
    

        public SleepDetailsPage()
        {
            this.InitializeComponent();
            //TODO
            /*this.VerticalAxis = (LinearAxis)this.PhasesChart.ActualAxes[1];
            this.LightSleepSeries = (AreaSeries)this.PhasesChart.Series[0];
            this.DeepSleepSeries = (AreaSeries)this.PhasesChart.Series[1];
            this.AwakeningsSleepSeries = (AreaSeries)this.PhasesChart.Series[2];
            this.HeartRateSeries = (LineSeries)this.PhasesChart.Series[3];
            this.VerticalAxis.Interval = new double?(1.0);
            this.VerticalAxis.Maximum = new double?(2.0);*/
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        public void SetLightSleepSeriesItemsSource(IEnumerable itemsSource)
        {
            //TODO
            //this.LightSleepSeries.ItemsSource = itemsSource;
        }

        public void SetDeepSleepSeriesItemsSource(IEnumerable itemsSource)
        {
            //TODO
            //this.DeepSleepSeries.ItemsSource = itemsSource;
        }

        public void SetAwakeningsSeriesItemsSource(IEnumerable itemsSource)
        {
            //TODO
            //this.AwakeningsSleepSeries.ItemsSource = itemsSource;
        }

        public void SetHeartRateSeriesItemSource(IEnumerable itemsSource, int min, int max)
        {
            //TODO
            //this.HeartRateSeries.Visibility = Visibility.Visible;
            //this.HeartRateSeries.ItemsSource = itemsSource;
            //this.HeartRateSeries.DependentRangeAxis.Minimum = new double?((double)min);
            //this.HeartRateSeries.DependentRangeAxis.Maximum = new double?((double)max);
        }

            
    }
}

