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
  public sealed class ActivityReminderView : UserControl, IComponentConnector
  {
    [GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
    private ToggleSwitch ActivityReminderToggleSwitch;
    [GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
    private StackPanel ActivityReminderConfigureContentDialog;
    [GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
    private TimePicker StartTimePicker;
    [GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
    private TimePicker EndTimePicker;
    [GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
    private bool _contentLoaded;

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
      ((ActivityReminderDialogViewModel) ((FrameworkElement) this.ActivityReminderConfigureContentDialog).DataContext).IsVisible = true;
    }

    [GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("ms-appx:///Views/DeviceSettings/ActivityReminderView.xaml"), (ComponentResourceLocation) 0);
      this.ActivityReminderToggleSwitch = (ToggleSwitch) ((FrameworkElement) this).FindName("ActivityReminderToggleSwitch");
      this.ActivityReminderConfigureContentDialog = (StackPanel) ((FrameworkElement) this).FindName("ActivityReminderConfigureContentDialog");
      this.StartTimePicker = (TimePicker) ((FrameworkElement) this).FindName("StartTimePicker");
      this.EndTimePicker = (TimePicker) ((FrameworkElement) this).FindName("EndTimePicker");
    }

    [GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
    [DebuggerNonUserCode]
    public void Connect(int connectionId, object target)
    {
      if (connectionId == 1)
      {
        UIElement uiElement = (UIElement) target;
        WindowsRuntimeMarshal.AddEventHandler<TappedEventHandler>(new Func<TappedEventHandler, EventRegistrationToken>(uiElement.add_Tapped), new Action<EventRegistrationToken>(uiElement.remove_Tapped), new TappedEventHandler(this.OnAcitivityReminderConfigureTapped));
      }
      this._contentLoaded = true;
    }
  }
}
