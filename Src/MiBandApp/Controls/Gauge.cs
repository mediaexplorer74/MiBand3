
// Type: MiBandApp.Controls.Gauge
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// 

using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;

#nullable disable
namespace MiBandApp.Controls
{
  [TemplatePart(Name = "PART_Scale", Type = typeof (Path))]
  [TemplatePart(Name = "PART_Trail", Type = typeof (Path))]
  public class Gauge : Control
  {
    private const string ScalePartName = "PART_Scale";
    private const string TrailPartName = "PART_Trail";
    private const double Degrees2Radians = 0.017453292519943295;
    public static readonly DependencyProperty ContentProperty = 
            DependencyProperty.Register(nameof (Content), typeof (UIElement), typeof (Gauge), new PropertyMetadata((object) null));
    public static readonly DependencyProperty MinimumProperty = 
            DependencyProperty.Register(nameof (Minimum), typeof (double), typeof (Gauge), new PropertyMetadata((object) 0.0));
    public static readonly DependencyProperty MaximumProperty = 
            DependencyProperty.Register(nameof (Maximum), typeof (double), typeof (Gauge), new PropertyMetadata((object) 100.0));
    public static readonly DependencyProperty ScaleWidthProperty = 
            DependencyProperty.Register(nameof (ScaleWidth), typeof (double), typeof (Gauge), new PropertyMetadata((object) 26.0));
    public static readonly DependencyProperty ValueProperty = 
            DependencyProperty.Register(nameof (Value), typeof (double), typeof (Gauge), new PropertyMetadata((object) 0.0, new PropertyChangedCallback(Gauge.OnValueChanged)));
    public static readonly DependencyProperty NeedleBrushProperty = 
            DependencyProperty.Register(nameof (NeedleBrush), typeof (Brush), typeof (Gauge), new PropertyMetadata((object) new SolidColorBrush(Colors.Red)));
    public static readonly DependencyProperty ScaleBrushProperty = 
            DependencyProperty.Register(nameof (ScaleBrush), typeof (Brush), typeof (Gauge), new PropertyMetadata((object) new SolidColorBrush(Colors.DarkGray)));
    public static readonly DependencyProperty TickBrushProperty = 
            DependencyProperty.Register(nameof (TickBrush), typeof (Brush), typeof (Gauge), new PropertyMetadata((object) new SolidColorBrush(Colors.White)));
    public static readonly DependencyProperty TrailBrushProperty = 
            DependencyProperty.Register(nameof (TrailBrush), typeof (Brush), typeof (Gauge), new PropertyMetadata((object) new SolidColorBrush(Colors.Orange)));
    public static readonly DependencyProperty ScaleTickBrushProperty = 
            DependencyProperty.Register(nameof (ScaleTickBrush), typeof (Brush), typeof (Gauge), new PropertyMetadata((object) new SolidColorBrush(Colors.Black)));
    public static readonly DependencyProperty IsUndefinedProperty = 
            DependencyProperty.Register(nameof (IsUndefined), typeof (bool), typeof (Gauge), new PropertyMetadata((object) false, new PropertyChangedCallback(Gauge.OnUndefinedChanged)));
    protected static readonly DependencyProperty ValueAngleProperty = 
            DependencyProperty.Register(nameof (ValueAngle), typeof (double), typeof (Gauge), new PropertyMetadata((object) null));

    public Gauge() => this.DefaultStyleKey = typeof (Gauge);

    public UIElement Content
    {
      get => (UIElement) ((DependencyObject) this).GetValue(Gauge.ContentProperty);
      set => ((DependencyObject) this).SetValue(Gauge.ContentProperty, (object) value);
    }

    public double Minimum
    {
      get => (double) ((DependencyObject) this).GetValue(Gauge.MinimumProperty);
      set => ((DependencyObject) this).SetValue(Gauge.MinimumProperty, (object) value);
    }

    public double Maximum
    {
      get => (double) ((DependencyObject) this).GetValue(Gauge.MaximumProperty);
      set => ((DependencyObject) this).SetValue(Gauge.MaximumProperty, (object) value);
    }

    public double ScaleWidth
    {
      get => (double) ((DependencyObject) this).GetValue(Gauge.ScaleWidthProperty);
      set => ((DependencyObject) this).SetValue(Gauge.ScaleWidthProperty, (object) value);
    }

    public double Value
    {
      get => (double) ((DependencyObject) this).GetValue(Gauge.ValueProperty);
      set => ((DependencyObject) this).SetValue(Gauge.ValueProperty, (object) value);
    }

    public Brush NeedleBrush
    {
      get => (Brush) ((DependencyObject) this).GetValue(Gauge.NeedleBrushProperty);
      set => ((DependencyObject) this).SetValue(Gauge.NeedleBrushProperty, (object) value);
    }

    public Brush TrailBrush
    {
      get => (Brush) ((DependencyObject) this).GetValue(Gauge.TrailBrushProperty);
      set => ((DependencyObject) this).SetValue(Gauge.TrailBrushProperty, (object) value);
    }

    public Brush ScaleBrush
    {
      get => (Brush) ((DependencyObject) this).GetValue(Gauge.ScaleBrushProperty);
      set => ((DependencyObject) this).SetValue(Gauge.ScaleBrushProperty, (object) value);
    }

    public Brush ScaleTickBrush
    {
      get => (Brush) ((DependencyObject) this).GetValue(Gauge.ScaleTickBrushProperty);
      set => ((DependencyObject) this).SetValue(Gauge.ScaleTickBrushProperty, (object) value);
    }

    public Brush TickBrush
    {
      get => (Brush) ((DependencyObject) this).GetValue(Gauge.TickBrushProperty);
      set => ((DependencyObject) this).SetValue(Gauge.TickBrushProperty, (object) value);
    }

    public bool IsUndefined
    {
      get => (bool) ((DependencyObject) this).GetValue(Gauge.IsUndefinedProperty);
      set => ((DependencyObject) this).SetValue(Gauge.IsUndefinedProperty, (object) value);
    }

    protected double ValueAngle
    {
      get => (double) ((DependencyObject) this).GetValue(Gauge.ValueAngleProperty);
      set => ((DependencyObject) this).SetValue(Gauge.ValueAngleProperty, (object) value);
    }

    protected override void OnApplyTemplate()
    {
      if (this.GetTemplateChild("PART_Scale") is Path templateChild)
      {
        PathGeometry pathGeometry = new PathGeometry();
        PathFigure pathFigure = new PathFigure();
        pathFigure.IsClosed = false;
        double num = 77.0 - this.ScaleWidth / 2.0;
        pathFigure.StartPoint = this.ScalePoint(0.0, num);
        ArcSegment arcSegment = new ArcSegment();
        arcSegment.SweepDirection = SweepDirection.Clockwise;
        arcSegment.IsLargeArc = true;
        arcSegment.Size = new Size(num, num);
        arcSegment.Point = this.ScalePoint(359.99, num);
        ((ICollection<PathSegment>) pathFigure.Segments).Add((PathSegment) arcSegment);
        ((ICollection<PathFigure>) pathGeometry.Figures).Add(pathFigure);
        templateChild.Data = (Geometry) pathGeometry;
      }
      Gauge.OnValueChanged((DependencyObject) this);
      Gauge.OnUndefinedChanged((DependencyObject) this);
      this.OnApplyTemplate();
    }

    private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      Gauge.OnValueChanged(d);
    }

    private static void OnUndefinedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      Gauge.OnUndefinedChanged(d);
    }

    private static void OnUndefinedChanged(DependencyObject d)
    {
      Gauge gauge = (Gauge) d;
      double middleOfScale = 77.0 - gauge.ScaleWidth / 2.0;
      if (!(gauge.GetTemplateChild("PART_Trail") is Path templateChild))
        return;
      if (!gauge.IsUndefined)
      {
        ((UIElement) templateChild).RenderTransform = (Transform) null;
        Gauge.OnValueChanged(d);
      }
      else
      {
        templateChild.RenderTransformOrigin = new Point(0.5, 0.5);
        templateChild.Visibility = Visibility.Visible;
        EllipseGeometry ellipseGeometry = new EllipseGeometry();
        ellipseGeometry.Center = gauge.ScalePoint(0.0, middleOfScale);
        ellipseGeometry.RadiusX = 0.0;
        ellipseGeometry.RadiusY = 0.0;
        templateChild.Data = (Geometry) ellipseGeometry;

        RotateTransform rotateTransform = new RotateTransform();
        templateChild.RenderTransform = (Transform)rotateTransform;
        Storyboard storyboard = new Storyboard();
        DoubleAnimationUsingKeyFrames animationUsingKeyFrames = new DoubleAnimationUsingKeyFrames();

        EasingDoubleKeyFrame easingDoubleKeyFrame1 = new EasingDoubleKeyFrame();
        easingDoubleKeyFrame1.KeyTime = ((KeyTime) TimeSpan.FromSeconds(1.5));
        easingDoubleKeyFrame1.Value = (360.0);
        QuadraticEase quadraticEase = new QuadraticEase();
        quadraticEase.EasingMode = EasingMode.EaseOut;

        easingDoubleKeyFrame1.EasingFunction = (EasingFunctionBase) quadraticEase;
        EasingDoubleKeyFrame easingDoubleKeyFrame2 = easingDoubleKeyFrame1;
        animationUsingKeyFrames.KeyFrames.Add((DoubleKeyFrame) easingDoubleKeyFrame2);
        animationUsingKeyFrames.RepeatBehavior = RepeatBehavior.Forever;
        Storyboard.SetTarget((Timeline) animationUsingKeyFrames, (DependencyObject) rotateTransform);
        Storyboard.SetTargetProperty((Timeline) animationUsingKeyFrames, "Angle");
        ((ICollection<Timeline>) storyboard.Children).Add((Timeline) animationUsingKeyFrames);
        storyboard.Begin();
      }
    }

    private static void OnValueChanged(DependencyObject d)
    {
      Gauge gauge = (Gauge) d;
      if (double.IsNaN(gauge.Value) || gauge.IsUndefined)
        return;
      double num = 77.0 - gauge.ScaleWidth / 2.0;
      gauge.ValueAngle = gauge.ValueToAngle(gauge.Value);
      if (!(gauge.GetTemplateChild("PART_Trail") is Path templateChild))
        return;
      if (gauge.ValueAngle <= 0.0)
      {
        templateChild.Visibility = Visibility.Collapsed;
      }
      else
      {
        templateChild.Visibility = Visibility.Visible;
        templateChild.RenderTransform = (Transform) null;
        PathGeometry pathGeometry = new PathGeometry();
        PathFigure pathFigure = new PathFigure();
        pathFigure.IsClosed = false;
        pathFigure.StartPoint = gauge.ScalePoint(0.0, num);
        ArcSegment arcSegment = new ArcSegment();
        arcSegment.SweepDirection = ((SweepDirection) 1);
        arcSegment.IsLargeArc = (gauge.ValueAngle > 180.0);
        arcSegment.Size = (new Size(num, num));
        arcSegment.Point = (gauge.ScalePoint(gauge.ValueAngle, num));
        ((ICollection<PathSegment>) pathFigure.Segments).Add((PathSegment) arcSegment);
        ((ICollection<PathFigure>) pathGeometry.Figures).Add(pathFigure);
        EllipseGeometry ellipseGeometry1 = new EllipseGeometry();
        ellipseGeometry1.Center = gauge.ScalePoint(0.0, num);
        ellipseGeometry1.RadiusX = 0.0;
        ellipseGeometry1.RadiusY = 0.0;
        EllipseGeometry ellipseGeometry2 = new EllipseGeometry();
        ellipseGeometry2.Center = gauge.ScalePoint(gauge.ValueAngle, num);
        ellipseGeometry2.RadiusX = 0.0;
        ellipseGeometry2.RadiusY = 0.0;
        GeometryGroup geometryGroup = new GeometryGroup();
        ((ICollection<Geometry>) geometryGroup.Children).Add((Geometry) pathGeometry);
        ((ICollection<Geometry>) geometryGroup.Children).Add((Geometry) ellipseGeometry1);
        ((ICollection<Geometry>) geometryGroup.Children).Add((Geometry) ellipseGeometry2);
        templateChild.Data = (Geometry) geometryGroup;
      }
    }

    private Point ScalePoint(double angle, double middleOfScale)
    {
      return new Point(100.0 + Math.Sin(0.017453292519943295 * angle) 
          * middleOfScale, 100.0 - Math.Cos(0.017453292519943295 * angle) * middleOfScale);
    }

    private double ValueToAngle(double value)
    {
      double angle1 = 0.0;
      double angle2 = 359.99;
      if (value < this.Minimum)
        return angle1;
      if (value > this.Maximum)
        return angle2;
      double num = angle2 - angle1;
      return (value - this.Minimum) / (this.Maximum - this.Minimum) * num + angle1;
    }
  }
}
