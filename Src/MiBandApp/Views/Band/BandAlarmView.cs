// Decompiled with JetBrains decompiler
// Type: MiBandApp.Views.Band.BandAlarmView
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.exe

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
  public sealed class BandAlarmView : UserControl, IComponentConnector
  {
    [GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
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
    private ToggleButton Su;
    [GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
    private bool _contentLoaded;

    public BandAlarmView()
    {
      this.InitializeComponent();
      this.AlarmTimePicker.put_ClockIdentifier(this.ClockFormat);
    }

    public string ClockFormat
    {
      get
      {
        return !CultureInfo.CurrentUICulture.DateTimeFormat.ShortTimePattern.Contains("H") ? ClockIdentifiers.TwelveHour : ClockIdentifiers.TwentyFourHour;
      }
    }

    [GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("ms-appx:///Views/Band/BandAlarmView.xaml"), (ComponentResourceLocation) 0);
      this.DataTemplateRoot = (Border) ((FrameworkElement) this).FindName("DataTemplateRoot");
      this.AlarmTimePicker = (TimePicker) ((FrameworkElement) this).FindName("AlarmTimePicker");
      this.StateSwitch = (ToggleSwitch) ((FrameworkElement) this).FindName("StateSwitch");
      this.IsSmartCheckBox = (CheckBox) ((FrameworkElement) this).FindName("IsSmartCheckBox");
      this.RepeatCheckBox = (CheckBox) ((FrameworkElement) this).FindName("RepeatCheckBox");
      this.Mo = (ToggleButton) ((FrameworkElement) this).FindName("Mo");
      this.Tu = (ToggleButton) ((FrameworkElement) this).FindName("Tu");
      this.We = (ToggleButton) ((FrameworkElement) this).FindName("We");
      this.Th = (ToggleButton) ((FrameworkElement) this).FindName("Th");
      this.Fr = (ToggleButton) ((FrameworkElement) this).FindName("Fr");
      this.Sa = (ToggleButton) ((FrameworkElement) this).FindName("Sa");
      this.Su = (ToggleButton) ((FrameworkElement) this).FindName("Su");
    }

    [GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
    [DebuggerNonUserCode]
    public void Connect(int connectionId, object target) => this._contentLoaded = true;
  }
}
