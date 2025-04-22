// Decompiled with JetBrains decompiler
// Type: MiBandApp.Views.DeviceSettings.DisplayView
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.exe

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
  public sealed class DisplayView : UserControl, IComponentConnector
  {
    private bool _dialogShown;
    [GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
    private UserControl Root;
    [GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
    private ContentDialog DisplayItemsContentDialog;
    [GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
    private bool _contentLoaded;

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

    [GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("ms-appx:///Views/DeviceSettings/DisplayView.xaml"), (ComponentResourceLocation) 0);
      this.Root = (UserControl) ((FrameworkElement) this).FindName("Root");
      this.DisplayItemsContentDialog = (ContentDialog) ((FrameworkElement) this).FindName("DisplayItemsContentDialog");
    }

    [GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
    [DebuggerNonUserCode]
    public void Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          UIElement uiElement1 = (UIElement) target;
          WindowsRuntimeMarshal.AddEventHandler<TappedEventHandler>(new Func<TappedEventHandler, EventRegistrationToken>(uiElement1.add_Tapped), new Action<EventRegistrationToken>(uiElement1.remove_Tapped), new TappedEventHandler(this.OnDisplayItemsTapped));
          break;
        case 2:
          UIElement uiElement2 = (UIElement) target;
          WindowsRuntimeMarshal.AddEventHandler<TappedEventHandler>(new Func<TappedEventHandler, EventRegistrationToken>(uiElement2.add_Tapped), new Action<EventRegistrationToken>(uiElement2.remove_Tapped), new TappedEventHandler(this.OnDisplayItemsTapped));
          break;
      }
      this._contentLoaded = true;
    }
  }
}
