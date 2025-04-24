
// Type: MiBandApp.Controls.OpenMenuFlyoutAction
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// 

using Microsoft.Xaml.Interactivity;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;

#nullable disable
namespace MiBandApp.Controls
{
  public class OpenMenuFlyoutAction : DependencyObject, IAction
  {
    public object Execute(object sender, object parameter)
    {
      FrameworkElement frameworkElement = sender as FrameworkElement;
      FlyoutBase.GetAttachedFlyout(frameworkElement).ShowAt(frameworkElement);
      return (object) null;
    }
  }
}
