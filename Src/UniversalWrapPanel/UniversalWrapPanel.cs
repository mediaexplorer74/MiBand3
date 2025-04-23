// Decompiled with JetBrains decompiler
// Type: Gregstoll.UniversalWrapPanel
// Assembly: UniversalWrapPanel, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 60235FB9-3146-43DC-9650-8467765D36E9
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\UniversalWrapPanel.dll

using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

#nullable disable
namespace Gregstoll
{
  public class UniversalWrapPanel : Panel
  {
    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(nameof (Orientation), (Type) typeof (Orientation), (Type) typeof (UniversalWrapPanel), (PropertyMetadata) null);

    public Orientation Orientation
    {
      get
      {
        return (Orientation) ((DependencyObject) this).GetValue(UniversalWrapPanel.OrientationProperty);
      }
      set
      {
        ((DependencyObject) this).SetValue(UniversalWrapPanel.OrientationProperty, (object) value);
      }
    }

    public UniversalWrapPanel() => this.Orientation = (Orientation) 0;

    protected virtual Size MeasureOverride(Size availableSize)
    {
      Point point = new Point(0.0, 0.0);
      if (this.Orientation == Orientation.Horizontal)
      {
        double val1 = 0.0;
        double num = 0.0;
        for (int index = 0; index < ((ICollection<UIElement>) this.Children).Count; ++index)
        {
          UIElement child = ((IList<UIElement>) this.Children)[index];
          child.Measure((Size) availableSize);
          if (point.X + ((Size) child.DesiredSize).Width > availableSize.Width)
          {
            point.X = 0.0;
            point.Y += val1;
            val1 = 0.0;
          }
          point.X += ((Size) child.DesiredSize).Width;
          val1 = Math.Max(val1, ((Size) child.DesiredSize).Height);
          num = Math.Max(num, point.X);
        }
        return new Size(num, point.Y + val1);
      }
      double val1_1 = 0.0;
      double num1 = 0.0;
      for (int index = 0; index < ((ICollection<UIElement>) this.Children).Count; ++index)
      {
        UIElement child = ((IList<UIElement>) this.Children)[index];
        child.Measure((Size) availableSize);
        if (point.Y + ((Size) child.DesiredSize).Height > availableSize.Height)
        {
          point.Y = 0.0;
          point.X += val1_1;
          val1_1 = 0.0;
        }
        point.Y += ((Size) child.DesiredSize).Height;
        val1_1 = Math.Max(val1_1, ((Size) child.DesiredSize).Width);
        num1 = Math.Max(num1, point.Y);
      }
      return new Size(point.X + val1_1, num1);
    }

    protected virtual Size ArrangeOverride(Size finalSize)
    {
      Point point1 = new Point(0.0, 0.0);
      int num1 = 0;
      if (this.Orientation == Orientation.Horizontal)
      {
        double num2 = 0.0;
        foreach (UIElement child in (IEnumerable<UIElement>) this.Children)
        {
          child.Arrange((Rect) new Rect(point1, new Point(point1.X + ((Size) child.DesiredSize).Width, point1.Y + ((Size) child.DesiredSize).Height)));
          if (((Size) child.DesiredSize).Height > num2)
            num2 = ((Size) child.DesiredSize).Height;
          point1.X += ((Size) child.DesiredSize).Width;
          if (num1 + 1 < ((ICollection<UIElement>) this.Children).Count && point1.X + ((Size) ((IList<UIElement>) this.Children)[num1 + 1].DesiredSize).Width > finalSize.Width)
          {
            point1.X = 0.0;
            point1.Y += num2;
            num2 = 0.0;
          }
          ++num1;
        }
      }
      else
      {
        double num3 = 0.0;
        foreach (UIElement child in (IEnumerable<UIElement>) this.Children)
        {
          child.Arrange( new Rect( point1, 
              new Point(point1.X + child.DesiredSize.Width, 
              point1.Y + child.DesiredSize.Height) ) );

          if (child.DesiredSize.Width > num3)
            num3 = child.DesiredSize.Width;
          point1.Y += child.DesiredSize.Height;
          if (num1 + 1 < this.Children.Count 
             && point1.Y + this.Children[num1 + 1].DesiredSize.Height > finalSize.Height)
          {
            point1.Y = 0.0;
            point1.X += num3;
            num3 = 0.0;
          }
          ++num1;
        }
      }
      return this.ArrangeOverride(finalSize);
    }
  }
}
