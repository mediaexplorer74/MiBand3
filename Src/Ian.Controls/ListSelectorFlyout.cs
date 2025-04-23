// Decompiled with JetBrains decompiler
// Type: Ian.Controls.ListSelectorFlyout
// Assembly: Ian.Controls, Version=0.8.2.0, Culture=neutral, PublicKeyToken=null
// MVID: C384A7D9-D254-451C-A544-CD6C2993240A
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\Ian.Controls.dll

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

#nullable disable
namespace Ian.Controls
{
  [Windows.UI.Xaml.Markup.ContentProperty(Name = "Content")]
  public class ListSelectorFlyout : PickerFlyoutBase
  {
    public static DependencyProperty ContentProperty = 
            DependencyProperty.Register(nameof (Content), typeof (object), 
                typeof (ListPickerFlyout), new PropertyMetadata((object) null));

    public static DependencyProperty ConfirmationButtonsVisibleProperty = 
            DependencyProperty.Register(nameof (ConfirmationButtonsVisible), 
                typeof (bool), typeof (ListPickerFlyout), new PropertyMetadata((object) false));

    public ListSelectorFlyout() => ((FlyoutBase) this).Placement = FlyoutPlacementMode.Full;

    public event EventHandler Confirmed;

    public object Content
    {
      get
      {
        return (object) (UIElement) ((DependencyObject) this).GetValue(ListSelectorFlyout.ContentProperty);
      }
      set => ((DependencyObject) this).SetValue(ListSelectorFlyout.ContentProperty, value);
    }

    public bool ConfirmationButtonsVisible
    {
      get
      {
        return (bool) ((DependencyObject) this).GetValue(ListSelectorFlyout.ConfirmationButtonsVisibleProperty);
      }
      set
      {
        ((DependencyObject) this).SetValue(ListSelectorFlyout.ConfirmationButtonsVisibleProperty, (object) value);
      }
    }

    protected virtual Control CreatePresenter()
    {
      FlyoutPresenter presenter = new FlyoutPresenter();
      presenter.Content = this.Content;
      presenter.IsTabStop = true;
      presenter.TabNavigation = ((KeyboardNavigationMode) 1);
      presenter.Height = Window.Current.Bounds.Height;
      ScrollViewer.SetVerticalScrollBarVisibility((DependencyObject) presenter, ScrollBarVisibility.Disabled);
      ScrollViewer.SetVerticalScrollMode((DependencyObject) presenter, ScrollMode.Disabled);
      return (Control) presenter;
    }

    protected virtual bool ShouldShowConfirmationButtons() => this.ConfirmationButtonsVisible;

    protected virtual void OnConfirmed()
    {
      EventHandler confirmed = this.Confirmed;
      if (confirmed == null)
        return;
      confirmed((object) this, EventArgs.Empty);
    }
  }
}
