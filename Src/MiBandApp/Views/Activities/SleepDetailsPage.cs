// Decompiled with JetBrains decompiler
// Type: MiBandApp.Views.Activities.SleepDetailsPage
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.exe

using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Navigation;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;

#nullable disable
namespace MiBandApp.Views.Activities
{
  public sealed class SleepDetailsPage : Page, IComponentConnector
  {
    [GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
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
    private LineSeries HeartRateSeries;
    [GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
    private bool _contentLoaded;

    public SleepDetailsPage()
    {
      this.InitializeComponent();
      this.VerticalAxis = (LinearAxis) this.PhasesChart.ActualAxes[1];
      this.LightSleepSeries = (AreaSeries) this.PhasesChart.Series[0];
      this.DeepSleepSeries = (AreaSeries) this.PhasesChart.Series[1];
      this.AwakeningsSleepSeries = (AreaSeries) this.PhasesChart.Series[2];
      this.HeartRateSeries = (LineSeries) this.PhasesChart.Series[3];
      this.VerticalAxis.Interval = new double?(1.0);
      this.VerticalAxis.Maximum = new double?(2.0);
    }

    protected virtual void OnNavigatedTo(NavigationEventArgs e)
    {
    }

    public void SetLightSleepSeriesItemsSource(IEnumerable itemsSource)
    {
      this.LightSleepSeries.ItemsSource = itemsSource;
    }

    public void SetDeepSleepSeriesItemsSource(IEnumerable itemsSource)
    {
      this.DeepSleepSeries.ItemsSource = itemsSource;
    }

    public void SetAwakeningsSeriesItemsSource(IEnumerable itemsSource)
    {
      this.AwakeningsSleepSeries.ItemsSource = itemsSource;
    }

    public void SetHeartRateSeriesItemSource(IEnumerable itemsSource, int min, int max)
    {
      ((UIElement) this.HeartRateSeries).put_Visibility((Visibility) 0);
      this.HeartRateSeries.ItemsSource = itemsSource;
      ((NumericAxis) this.HeartRateSeries.DependentRangeAxis).Minimum = new double?((double) min);
      ((NumericAxis) this.HeartRateSeries.DependentRangeAxis).Maximum = new double?((double) max);
    }

    [GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("ms-appx:///Views/Activities/SleepDetailsPage.xaml"), (ComponentResourceLocation) 0);
      this.PhasesChart = (Chart) ((FrameworkElement) this).FindName("PhasesChart");
      this.VerticalAxis = (LinearAxis) ((FrameworkElement) this).FindName("VerticalAxis");
      this.LightSleepSeries = (AreaSeries) ((FrameworkElement) this).FindName("LightSleepSeries");
      this.DeepSleepSeries = (AreaSeries) ((FrameworkElement) this).FindName("DeepSleepSeries");
      this.AwakeningsSleepSeries = (AreaSeries) ((FrameworkElement) this).FindName("AwakeningsSleepSeries");
      this.HeartRateSeries = (LineSeries) ((FrameworkElement) this).FindName("HeartRateSeries");
    }

    [GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
    [DebuggerNonUserCode]
    public void Connect(int connectionId, object target) => this._contentLoaded = true;
  }
}
