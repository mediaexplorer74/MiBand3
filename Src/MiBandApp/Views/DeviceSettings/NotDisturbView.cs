// Decompiled with JetBrains decompiler
// Type: MiBandApp.Views.DeviceSettings.NotDisturbView
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
namespace MiBandApp.Views.DeviceSettings
{
  public sealed class NotDisturbView : UserControl, IComponentConnector
  {
    [GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
    private ToggleSwitch IsEnabledToggleSwitch;
    [GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
    private StackPanel DetailedSettingsDialog;
    [GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
    private CheckBox AutomaticScheduleCheckBox;
    [GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
    private TimePicker StartTimePicker;
    [GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
    private TimePicker EndTimePicker;
    [GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
    private bool _contentLoaded;

    public NotDisturbView()
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

    [GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("ms-appx:///Views/DeviceSettings/NotDisturbView.xaml"), (ComponentResourceLocation) 0);
      this.IsEnabledToggleSwitch = (ToggleSwitch) ((FrameworkElement) this).FindName("IsEnabledToggleSwitch");
      this.DetailedSettingsDialog = (StackPanel) ((FrameworkElement) this).FindName("DetailedSettingsDialog");
      this.AutomaticScheduleCheckBox = (CheckBox) ((FrameworkElement) this).FindName("AutomaticScheduleCheckBox");
      this.StartTimePicker = (TimePicker) ((FrameworkElement) this).FindName("StartTimePicker");
      this.EndTimePicker = (TimePicker) ((FrameworkElement) this).FindName("EndTimePicker");
    }

    [GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
    [DebuggerNonUserCode]
    public void Connect(int connectionId, object target) => this._contentLoaded = true;
  }
}
