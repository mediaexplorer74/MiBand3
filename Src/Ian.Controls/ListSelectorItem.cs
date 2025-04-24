
// Type: Ian.Controls.ListSelectorItem
// Assembly: Ian.Controls, Version=0.8.2.0, Culture=neutral, PublicKeyToken=null
// MVID: C384A7D9-D254-451C-A544-CD6C2993240A
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\Ian.Controls.dll

using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

#nullable disable
namespace Ian.Controls
{
  public sealed class ListSelectorItem : Control
  {
    public static DependencyProperty ItemContentProperty = 
            DependencyProperty.Register(nameof (ItemContent), typeof (object),
                typeof (ListSelectorItem), new PropertyMetadata((object) null));
    public static DependencyProperty ItemTemplateProperty = 
            DependencyProperty.Register(nameof (ItemTemplate), typeof (DataTemplate),
                typeof (ListSelectorItem), new PropertyMetadata((object) null));
    public static DependencyProperty IsSelectedProperty = 
            DependencyProperty.Register(nameof (IsSelected), typeof (bool), typeof (ListSelectorItem), 
                new PropertyMetadata((object) false, 
                    new PropertyChangedCallback(ListSelectorItem.IsSelectedPropertyChanged)));

        public ListSelectorItem()
        {
            this.DefaultStyleKey = typeof(ListSelectorItem);
            ((FrameworkElement)this).Loaded += (sender, args) => this.UpdateStates(true);
        }

    public DataTemplate ItemTemplate
    {
      get
      {
        return (DataTemplate) ((DependencyObject) this).GetValue(ListSelectorItem.ItemTemplateProperty);
      }
      set
      {
        ((DependencyObject) this).SetValue(ListSelectorItem.ItemTemplateProperty, (object) value);
      }
    }

    public object ItemContent
    {
      get => ((DependencyObject) this).GetValue(ListSelectorItem.ItemContentProperty);
      set => ((DependencyObject) this).SetValue(ListSelectorItem.ItemContentProperty, value);
    }

    public bool IsSelected
    {
      get => (bool) ((DependencyObject) this).GetValue(ListSelectorItem.IsSelectedProperty);
      set
      {
        ((DependencyObject) this).SetValue(ListSelectorItem.IsSelectedProperty, (object) value);
      }
    }

    public int ItemIndex { get; set; }

    private static void IsSelectedPropertyChanged(
      DependencyObject dependencyObject,
      DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
    {
      ((ListSelectorItem) dependencyObject).UpdateStates(true);
    }

    private void UpdateStates(bool useTransitions)
    {
      if (this.IsSelected)
        VisualStateManager.GoToState((Control) this, "Selected", useTransitions);
      else
        VisualStateManager.GoToState((Control) this, "Normal", useTransitions);
    }
  }
}
