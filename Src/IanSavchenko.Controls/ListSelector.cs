// Decompiled with JetBrains decompiler
// Type: IanSavchenko.Controls.ListSelector
// Assembly: IanSavchenko.Controls, Version=0.8.2.0, Culture=neutral, PublicKeyToken=null
// MVID: C384A7D9-D254-451C-A544-CD6C2993240A
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\IanSavchenko.Controls.dll

using IanSavchenko.Controls.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;

#nullable disable
namespace IanSavchenko.Controls
{
  [TemplatePart(Name = "PART_ItemsControl", Type = typeof (ItemsControl))]
  [TemplatePart(Name = "PART_ScrollViewer", Type = typeof (ScrollViewer))]
  [TemplatePart(Name = "PART_InactiveStateItem", Type = typeof (ListSelectorItem))]
  public sealed class ListSelector : Control
  {
    private const string ItemsControlPartName = "PART_ItemsControl";
    private const string ScrollViewerPartName = "PART_ScrollViewer";
    private const string InactiveStateItemPartName = "PART_InactiveStateItem";
    private const int ShowHideAnimationDurationMs = 200;
    private static ListSelector ActiveSelector = (ListSelector) null;
    public static readonly DependencyProperty ItemHeightProperty = DependencyProperty.Register(nameof (ItemHeight), typeof (double), typeof (ListSelector), new PropertyMetadata((object) 130.0, new PropertyChangedCallback(ListSelector.OnItemHeightChanged)));
    public static readonly DependencyProperty ItemWidthProperty = DependencyProperty.Register(nameof (ItemWidth), typeof (double), typeof (ListSelector), new PropertyMetadata((object) 130.0, new PropertyChangedCallback(ListSelector.OnItemWidthChanged)));
    public static readonly DependencyProperty ItemMarginProperty = DependencyProperty.Register(nameof (ItemMargin), typeof (Thickness), typeof (ListSelector), new PropertyMetadata((object) new Thickness()));
    public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(nameof (ItemsSource), typeof (IList), typeof (ListSelector), new PropertyMetadata((object) null, new PropertyChangedCallback(ListSelector.OnItemsSourceChanged)));
    public static DependencyProperty ItemTemplateProperty = DependencyProperty.Register(nameof (ItemTemplate), typeof (DataTemplate), typeof (ListSelector), new PropertyMetadata((object) null, new PropertyChangedCallback(ListSelector.OnItemTemplateChangedCallback)));
    public static DependencyProperty SelectedIndexProperty = DependencyProperty.Register(nameof (SelectedIndex), typeof (int), typeof (ListSelector), new PropertyMetadata((object) -1, new PropertyChangedCallback(ListSelector.OnSelectedIndexChanged)));
    public static DependencyProperty IsActiveProperty = DependencyProperty.Register(nameof (IsActive), typeof (bool), typeof (ListSelector), new PropertyMetadata((object) false, new PropertyChangedCallback(ListSelector.OnIsActiveChanged)));
    private readonly ScheduleInvoker _scheduleInvoker;
    private readonly List<ListSelectorItem> _items = new List<ListSelectorItem>();
    private ItemsControl _itemsControlPart;
    private ScrollViewer _scrollViewerPart;
    private ListSelectorItem _inactiveStateItemPart;
    private Thickness _itemsControlMargin;
    private int _highlightIndex = -1;
    private double _latestVerticalScrollOffset;
    private volatile bool _snappingPerformed;
    private Storyboard _hideItemsStoryboard;
    private DoubleAnimation _hideItemsAnimation;
    private Storyboard _showItemsStoryboard;
    private DoubleAnimation _showItemsAnimation;
    private double _prevChangeViewCallOffset = -1.0;
    private double _prevChangeViewCallCurrentOffset = -1.0;
    private bool _active;

    public ListSelector()
    {
      this.put_DefaultStyleKey((object) typeof (ListSelector));
      WindowsRuntimeMarshal.AddEventHandler<SizeChangedEventHandler>(new Func<SizeChangedEventHandler, EventRegistrationToken>(((FrameworkElement) this).add_SizeChanged), new Action<EventRegistrationToken>(((FrameworkElement) this).remove_SizeChanged), new SizeChangedEventHandler(this.OnSizeChanged));
      WindowsRuntimeMarshal.AddEventHandler<RoutedEventHandler>(new Func<RoutedEventHandler, EventRegistrationToken>(((FrameworkElement) this).add_Loaded), new Action<EventRegistrationToken>(((FrameworkElement) this).remove_Loaded), new RoutedEventHandler(this.OnLoaded));
      this._scheduleInvoker = new ScheduleInvoker(((DependencyObject) this).Dispatcher);
    }

    public double ItemWidth
    {
      get => (double) ((DependencyObject) this).GetValue(ListSelector.ItemWidthProperty);
      set => ((DependencyObject) this).SetValue(ListSelector.ItemWidthProperty, (object) value);
    }

    public double ItemHeight
    {
      get => (double) ((DependencyObject) this).GetValue(ListSelector.ItemHeightProperty);
      set => ((DependencyObject) this).SetValue(ListSelector.ItemHeightProperty, (object) value);
    }

    public Thickness ItemMargin
    {
      get => (Thickness) ((DependencyObject) this).GetValue(ListSelector.ItemMarginProperty);
      set => ((DependencyObject) this).SetValue(ListSelector.ItemMarginProperty, (object) value);
    }

    public IList ItemsSource
    {
      get => (IList) ((DependencyObject) this).GetValue(ListSelector.ItemsSourceProperty);
      set => ((DependencyObject) this).SetValue(ListSelector.ItemsSourceProperty, (object) value);
    }

    public DataTemplate ItemTemplate
    {
      get => (DataTemplate) ((DependencyObject) this).GetValue(ListSelector.ItemTemplateProperty);
      set => ((DependencyObject) this).SetValue(ListSelector.ItemTemplateProperty, (object) value);
    }

    public int SelectedIndex
    {
      get => (int) ((DependencyObject) this).GetValue(ListSelector.SelectedIndexProperty);
      set
      {
        if (this.SelectedIndex == value)
          return;
        ((DependencyObject) this).SetValue(ListSelector.SelectedIndexProperty, (object) value);
      }
    }

    public bool IsActive
    {
      get => (bool) ((DependencyObject) this).GetValue(ListSelector.IsActiveProperty);
      set => ((DependencyObject) this).SetValue(ListSelector.IsActiveProperty, (object) value);
    }

    private double ScrollOffsetPerItem
    {
      get
      {
        return this._items.Count == 0 ? 0.0 : ((FrameworkElement) this._itemsControlPart).ActualHeight / (double) this._items.Count;
      }
    }

    protected virtual void OnApplyTemplate()
    {
      this._itemsControlPart = this.GetTemplateChild("PART_ItemsControl") as ItemsControl;
      this._scrollViewerPart = this.GetTemplateChild("PART_ScrollViewer") as ScrollViewer;
      this._inactiveStateItemPart = this.GetTemplateChild("PART_InactiveStateItem") as ListSelectorItem;
      ScrollViewer scrollViewerPart1 = this._scrollViewerPart;
      WindowsRuntimeMarshal.AddEventHandler<EventHandler<ScrollViewerViewChangedEventArgs>>(new Func<EventHandler<ScrollViewerViewChangedEventArgs>, EventRegistrationToken>(scrollViewerPart1.add_ViewChanged), new Action<EventRegistrationToken>(scrollViewerPart1.remove_ViewChanged), new EventHandler<ScrollViewerViewChangedEventArgs>(this.ScrollViewerPartOnViewChanged));
      ScrollViewer scrollViewerPart2 = this._scrollViewerPart;
      WindowsRuntimeMarshal.AddEventHandler<EventHandler<ScrollViewerViewChangingEventArgs>>(new Func<EventHandler<ScrollViewerViewChangingEventArgs>, EventRegistrationToken>(scrollViewerPart2.add_ViewChanging), new Action<EventRegistrationToken>(scrollViewerPart2.remove_ViewChanging), new EventHandler<ScrollViewerViewChangingEventArgs>(this.ScrollViewerPartOnViewChanging));
      ((UIElement) this._scrollViewerPart).put_IsTapEnabled(true);
      ScrollViewer scrollViewerPart3 = this._scrollViewerPart;
      WindowsRuntimeMarshal.AddEventHandler<TappedEventHandler>(new Func<TappedEventHandler, EventRegistrationToken>(((UIElement) scrollViewerPart3).add_Tapped), new Action<EventRegistrationToken>(((UIElement) scrollViewerPart3).remove_Tapped), new TappedEventHandler(this.ScrollViewerPartOnTapped));
      this.UpdateItemsControlItems();
      this.UpdateGeometricalParams();
      this.CreateAnimations();
      ((FrameworkElement) this).OnApplyTemplate();
    }

    private static void OnItemHeightChanged(
      DependencyObject d,
      DependencyPropertyChangedEventArgs e)
    {
    }

    private static void OnItemWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
    }

    private static void OnItemTemplateChangedCallback(
      DependencyObject dependencyObject,
      DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
    {
    }

    private static void OnIsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      int num = (bool) e.NewValue ? 1 : 0;
      ListSelector listSelector = (ListSelector) d;
      if (num != 0)
      {
        listSelector.SetActive(true);
        if (ListSelector.ActiveSelector == listSelector)
          return;
        ListSelector activeSelector = ListSelector.ActiveSelector;
        ListSelector.ActiveSelector = listSelector;
        if (activeSelector == null)
          return;
        activeSelector.IsActive = false;
      }
      else
      {
        listSelector.SetActive(false);
        if (ListSelector.ActiveSelector != listSelector)
          return;
        ListSelector.ActiveSelector = (ListSelector) null;
      }
    }

    private static async void OnSelectedIndexChanged(
      DependencyObject dependencyObject,
      DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
    {
      await ((ListSelector) dependencyObject).SelectItem((int) dependencyPropertyChangedEventArgs.NewValue).ConfigureAwait(true);
    }

    private static async void OnItemsSourceChanged(
      DependencyObject d,
      DependencyPropertyChangedEventArgs e)
    {
      ListSelector listSelector = (ListSelector) d;
      listSelector.UpdateItemsControlItems();
      await listSelector.SelectItem(listSelector.SelectedIndex).ConfigureAwait(true);
    }

    private async void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
    {
      await this.SelectItem(this.SelectedIndex).ConfigureAwait(true);
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
    {
      this.UpdateGeometricalParams();
      this.UpdateItemsOpacity();
    }

    private void UpdateGeometricalParams()
    {
      if (this._itemsControlPart == null)
        return;
      double actualHeight = ((FrameworkElement) this).ActualHeight;
      this._itemsControlMargin = new Thickness(0.0, actualHeight / 2.0 - this.ItemMargin.Top - this.ItemHeight / 2.0, 0.0, actualHeight / 2.0 - this.ItemMargin.Bottom - this.ItemHeight / 2.0);
      ((FrameworkElement) this._itemsControlPart).put_Margin(this._itemsControlMargin);
    }

    private int GetItemIndexForScrollOffset(double scrollOffset)
    {
      double top = this._itemsControlMargin.Top;
      double num = ((FrameworkElement) this).ActualHeight / 2.0;
      return (int) ((scrollOffset + num - top) / this.ScrollOffsetPerItem);
    }

    private async void UpdateItemsControlItems()
    {
      if (this._itemsControlPart == null)
        return;
      this._items.Clear();
      if (this.ItemsSource != null)
      {
        int num = 0;
        foreach (object obj in (IEnumerable) this.ItemsSource)
        {
          ListSelectorItem listSelectorItem1 = new ListSelectorItem();
          ((FrameworkElement) listSelectorItem1).put_Width(this.ItemWidth);
          ((FrameworkElement) listSelectorItem1).put_Height(this.ItemHeight);
          ((FrameworkElement) listSelectorItem1).put_Margin(this.ItemMargin);
          listSelectorItem1.ItemContent = obj;
          listSelectorItem1.ItemIndex = num++;
          ListSelectorItem listSelectorItem2 = listSelectorItem1;
          listSelectorItem2.ItemTemplate = this.ItemTemplate;
          this._items.Add(listSelectorItem2);
        }
      }
      this.UpdateItemsOpacity();
      this._itemsControlPart.put_ItemsSource((object) null);
      this._itemsControlPart.put_ItemsSource((object) this._items);
      if (this._items.Count <= 0 || this.SelectedIndex != -1)
        return;
      await this.SelectItem(0).ConfigureAwait(true);
    }

    private void CreateAnimations()
    {
      this._showItemsStoryboard = new Storyboard();
      this._hideItemsStoryboard = new Storyboard();
      this._hideItemsAnimation = new DoubleAnimation();
      this._hideItemsAnimation.put_To(new double?(0.0));
      this._showItemsAnimation = new DoubleAnimation();
      this._showItemsAnimation.put_To(new double?(1.0));
      ((ICollection<Timeline>) this._showItemsStoryboard.Children).Add((Timeline) this._showItemsAnimation);
      ((ICollection<Timeline>) this._hideItemsStoryboard.Children).Add((Timeline) this._hideItemsAnimation);
      Storyboard.SetTarget((Timeline) this._showItemsStoryboard, (DependencyObject) this._scrollViewerPart);
      Storyboard.SetTarget((Timeline) this._hideItemsStoryboard, (DependencyObject) this._scrollViewerPart);
      Storyboard.SetTargetProperty((Timeline) this._showItemsStoryboard, "Opacity");
      Storyboard.SetTargetProperty((Timeline) this._hideItemsStoryboard, "Opacity");
      Storyboard showItemsStoryboard = this._showItemsStoryboard;
      WindowsRuntimeMarshal.AddEventHandler<EventHandler<object>>(new Func<EventHandler<object>, EventRegistrationToken>(((Timeline) showItemsStoryboard).add_Completed), new Action<EventRegistrationToken>(((Timeline) showItemsStoryboard).remove_Completed), new EventHandler<object>(this.ShowItemsStoryboardOnCompleted));
    }

    private async void ScrollViewerPartOnTapped(
      object sender,
      TappedRoutedEventArgs tappedRoutedEventArgs)
    {
      if (this._snappingPerformed)
        return;
      if (!this.IsActive)
      {
        this.IsActive = true;
      }
      else
      {
        double scrollOffset = tappedRoutedEventArgs.GetPosition((UIElement) this._itemsControlPart).Y - this.ItemHeight / 2.0;
        if (scrollOffset < 0.0)
          return;
        int indexForScrollOffset = this.GetItemIndexForScrollOffset(scrollOffset);
        if (indexForScrollOffset >= this._items.Count)
          return;
        if (indexForScrollOffset == this.SelectedIndex)
        {
          if (!this.IsActive)
            return;
          this.IsActive = false;
        }
        else
        {
          this.CancelSnappingCheck();
          await this.SelectItem(indexForScrollOffset).ConfigureAwait(true);
        }
      }
    }

    private void ScrollViewerPartOnViewChanging(
      object sender,
      ScrollViewerViewChangingEventArgs scrollViewerViewChangingEventArgs)
    {
      this._latestVerticalScrollOffset = scrollViewerViewChangingEventArgs.NextView.VerticalOffset;
      this.UpdateItemsOpacity();
      if (this._snappingPerformed)
      {
        this.RescheduleSnappingCheck();
      }
      else
      {
        if (!this._active && !scrollViewerViewChangingEventArgs.IsInertial)
        {
          ((UIElement) this._inactiveStateItemPart).put_Visibility((Visibility) 1);
          this.IsActive = true;
        }
        this.RemoveHighlight();
        this.CancelSnappingCheck();
      }
    }

    private async void ScrollViewerPartOnViewChanged(
      object sender,
      ScrollViewerViewChangedEventArgs e)
    {
      if (e.IsIntermediate)
        return;
      if (this._snappingPerformed)
        this.FinishSnapping();
      await this.SelectItem(this.GetItemIndexForScrollOffset(this._latestVerticalScrollOffset)).ConfigureAwait(true);
    }

    private void UpdateItemsOpacity()
    {
      int num1 = (int) (((FrameworkElement) this).ActualHeight / this.ScrollOffsetPerItem);
      int indexForScrollOffset = this.GetItemIndexForScrollOffset(this._latestVerticalScrollOffset);
      int num2 = indexForScrollOffset - num1;
      int num3 = indexForScrollOffset + num1;
      for (int index = 0; index < this._items.Count; ++index)
        ((UIElement) this._items[index]).put_Opacity(index < num2 || index > num3 ? 0.0 : 1.0);
    }

    private async Task SelectItem(int index)
    {
      if (this._itemsControlPart == null)
        return;
      await Task.Delay(1).ConfigureAwait(true);
      this.HighlightItem(index);
      this.SnapScrollToItem(index);
      this.SelectedIndex = index;
      this.SetActive(this.IsActive);
    }

    private void HighlightItem(int index)
    {
      if (this._highlightIndex == index)
        return;
      this.RemoveHighlight();
      if (index >= this._items.Count || index < -1)
        return;
      this._highlightIndex = index;
      if (this._highlightIndex == -1)
        return;
      this._items[this._highlightIndex].IsSelected = true;
      this.UpdateInactiveStateItem();
    }

    private void RemoveHighlight()
    {
      if (this._highlightIndex == -1 || this._highlightIndex >= this._items.Count)
        return;
      this._items[this._highlightIndex].IsSelected = false;
      this._highlightIndex = -1;
    }

    private void UpdateInactiveStateItem()
    {
      this._inactiveStateItemPart.ItemTemplate = (DataTemplate) null;
      this._inactiveStateItemPart.ItemContent = this._items[this._highlightIndex].ItemContent;
      this._inactiveStateItemPart.ItemTemplate = this.ItemTemplate;
    }

    private void SnapScrollToItem(int index)
    {
      if (index == -1)
        return;
      double newOffset = Math.Round((double) index * this.ScrollOffsetPerItem, MidpointRounding.ToEven);
      if (newOffset == Math.Round(this._latestVerticalScrollOffset, MidpointRounding.ToEven))
      {
        this.FinishSnapping();
        if (this._active)
          return;
        this.SetActive(this._active);
      }
      else
      {
        this.StartSnapping();
        this.ScrollToOffset(newOffset, this._latestVerticalScrollOffset);
        this.RescheduleSnappingCheck();
      }
    }

    private void RescheduleSnappingCheck()
    {
      this._scheduleInvoker.Schedule(TimeSpan.FromMilliseconds(300.0), new Action(this.CheckSnappingFinished));
    }

    private void CancelSnappingCheck() => this._scheduleInvoker.Stop();

    private async void CheckSnappingFinished()
    {
      if (!this._snappingPerformed)
        return;
      await this.SelectItem(this.GetItemIndexForScrollOffset(this._latestVerticalScrollOffset)).ConfigureAwait(true);
    }

    private void StartSnapping()
    {
      this._snappingPerformed = true;
      ((UIElement) this._scrollViewerPart).put_IsHitTestVisible(false);
    }

    private void FinishSnapping()
    {
      this._snappingPerformed = false;
      ((UIElement) this._scrollViewerPart).put_IsHitTestVisible(true);
    }

    private void ScrollToOffset(double newOffset, double currentOffset)
    {
      if (this._prevChangeViewCallOffset == newOffset && currentOffset == this._prevChangeViewCallCurrentOffset)
      {
        this._scrollViewerPart.ScrollToVerticalOffset(newOffset);
      }
      else
      {
        this._scrollViewerPart.ChangeView(new double?(0.0), new double?(newOffset), new float?());
        this._prevChangeViewCallOffset = newOffset;
        this._prevChangeViewCallCurrentOffset = currentOffset;
      }
    }

    private void SetActive(bool active)
    {
      this._active = active;
      if (this._highlightIndex == -1 || this._snappingPerformed)
        return;
      if (active)
        this.ShowScrollViewer();
      else
        this.HideScrollViewer();
      if (!this._active)
        ((UIElement) this._inactiveStateItemPart).put_Visibility((Visibility) 0);
      this.UpdateInactiveStateItem();
    }

    private void ShowScrollViewer()
    {
      double opacity = ((UIElement) this._scrollViewerPart).Opacity;
      this._hideItemsStoryboard.Stop();
      lock (this._showItemsStoryboard)
      {
        if (this._showItemsStoryboard.GetCurrentState() != 2)
          return;
        this._showItemsAnimation.put_From(new double?(opacity));
        ((Timeline) this._showItemsAnimation).put_Duration(new Duration(TimeSpan.FromMilliseconds(200.0 * Math.Abs(1.0 - opacity))));
        this._showItemsStoryboard.Begin();
      }
    }

    private void HideScrollViewer()
    {
      double opacity = ((UIElement) this._scrollViewerPart).Opacity;
      this._showItemsStoryboard.Stop();
      lock (this._hideItemsStoryboard)
      {
        if (this._hideItemsStoryboard.GetCurrentState() != 2)
          return;
        this._hideItemsAnimation.put_From(new double?(opacity));
        ((Timeline) this._hideItemsAnimation).put_Duration(new Duration(TimeSpan.FromMilliseconds(200.0 * Math.Abs(opacity))));
        this._hideItemsStoryboard.Begin();
      }
    }

    private void ShowItemsStoryboardOnCompleted(object sender, object o)
    {
      if (!this.IsActive)
        return;
      ((UIElement) this._inactiveStateItemPart).put_Visibility((Visibility) 1);
    }
  }
}
