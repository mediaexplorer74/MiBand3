// Decompiled with JetBrains decompiler
// Type: MiBandApp.Controls.Band
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.exe

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

#nullable disable
namespace MiBandApp.Controls
{
  [TemplatePart(Name = "PART_EllipseLeft", Type = typeof (Ellipse))]
  [TemplatePart(Name = "PART_EllipseCenter", Type = typeof (Ellipse))]
  [TemplatePart(Name = "PART_EllipseRight", Type = typeof (Ellipse))]
  public class Band : Control
  {
    private const string EllipseLeftPartName = "PART_EllipseLeft";
    private const string EllipseCenterPartName = "PART_EllipseCenter";
    private const string EllipseRightPartName = "PART_EllipseRight";
    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof (Value), typeof (double), typeof (Band), new PropertyMetadata((object) 0.0, new PropertyChangedCallback(Band.OnValueChanged)));
    public static readonly DependencyProperty AnimationProperty = DependencyProperty.Register(nameof (Animation), typeof (BandAnimation), typeof (Band), new PropertyMetadata((object) BandAnimation.None, new PropertyChangedCallback(Band.OnAnimationChanged)));
    public static readonly DependencyProperty DotsVisibilityProperty = DependencyProperty.Register(nameof (DotsVisibility), typeof (Visibility), typeof (Band), new PropertyMetadata((object) (Visibility) 0));
    private readonly List<Ellipse> _ellipses = new List<Ellipse>();
    private int _animationLightenedEllipse;
    private DispatcherTimer _animationTimer;
    private BandAnimation _timerMode;

    public Band() => this.put_DefaultStyleKey((object) typeof (Band));

    public double Value
    {
      get => (double) ((DependencyObject) this).GetValue(Band.ValueProperty);
      set => ((DependencyObject) this).SetValue(Band.ValueProperty, (object) value);
    }

    public BandAnimation Animation
    {
      get => (BandAnimation) ((DependencyObject) this).GetValue(Band.AnimationProperty);
      set => ((DependencyObject) this).SetValue(Band.AnimationProperty, (object) value);
    }

    public Visibility DotsVisibility
    {
      get => (Visibility) ((DependencyObject) this).GetValue(Band.DotsVisibilityProperty);
      set => ((DependencyObject) this).SetValue(Band.DotsVisibilityProperty, (object) value);
    }

    protected virtual void OnApplyTemplate()
    {
      this._ellipses.Add(this.GetTemplateChild("PART_EllipseLeft") as Ellipse);
      this._ellipses.Add(this.GetTemplateChild("PART_EllipseCenter") as Ellipse);
      this._ellipses.Add(this.GetTemplateChild("PART_EllipseRight") as Ellipse);
      Band.OnValueChanged((DependencyObject) this);
      Band.OnAnimationChanged((DependencyObject) this);
      ((FrameworkElement) this).OnApplyTemplate();
    }

    private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      Band.OnValueChanged(d);
    }

    private static void OnAnimationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      Band.OnAnimationChanged(d);
    }

    private static void OnValueChanged(DependencyObject d) => ((Band) d).ValueChanged();

    private static void OnAnimationChanged(DependencyObject d) => ((Band) d).AnimationChanged();

    private void ValueChanged()
    {
      if (double.IsNaN(this.Value) || this.Animation != BandAnimation.None || this._ellipses.Count == 0)
        return;
      int num1 = 0;
      double num2 = this.Value;
      if (num2 >= 0.15 && num2 < 0.5)
        num1 = 1;
      if (num2 >= 0.5 && num2 < 0.85)
        num1 = 2;
      if (num2 >= 0.85)
        num1 = 3;
      for (int index = 0; index < this._ellipses.Count; ++index)
      {
        if (index < num1)
          this.LightEllipse(index);
        else
          this.UnlightEllipse(index);
      }
    }

    private void AnimationChanged()
    {
      if (this.Animation == BandAnimation.None)
      {
        if (this._animationTimer != null)
          this._animationTimer.Stop();
        this._animationLightenedEllipse = 0;
        this._animationTimer = (DispatcherTimer) null;
        this._timerMode = BandAnimation.None;
        this.ValueChanged();
      }
      else
      {
        if (this._ellipses.Count == 0 || this._animationTimer != null && this._timerMode == this.Animation)
          return;
        this._animationTimer?.Stop();
        DispatcherTimer dispatcherTimer1 = new DispatcherTimer();
        dispatcherTimer1.put_Interval(TimeSpan.FromMilliseconds(this.Animation == BandAnimation.Communicating ? 75.0 : 500.0));
        DispatcherTimer dispatcherTimer2 = dispatcherTimer1;
        WindowsRuntimeMarshal.AddEventHandler<EventHandler<object>>(new Func<EventHandler<object>, EventRegistrationToken>(dispatcherTimer2.add_Tick), new Action<EventRegistrationToken>(dispatcherTimer2.remove_Tick), new EventHandler<object>(this.OnAnimationTimerTick));
        dispatcherTimer1.Start();
        this._animationTimer = dispatcherTimer1;
        this._timerMode = this.Animation;
      }
    }

    private void OnAnimationTimerTick(object sender, object o)
    {
      if (this.Animation == BandAnimation.None)
        return;
      if (this._animationLightenedEllipse != -1 && this._animationLightenedEllipse < this._ellipses.Count)
        this.UnlightEllipse(this._animationLightenedEllipse);
      ++this._animationLightenedEllipse;
      if (this._animationLightenedEllipse < this._ellipses.Count)
        this.LightEllipse(this._animationLightenedEllipse);
      if (this._animationLightenedEllipse <= this._ellipses.Count + (this.Animation == BandAnimation.Communicating ? 2 : 0))
        return;
      this._animationLightenedEllipse = -1;
    }

    private void LightEllipse(int index)
    {
      ((Shape) this._ellipses[index]).put_Fill(this.Foreground);
    }

    private void UnlightEllipse(int index)
    {
      ((Shape) this._ellipses[index]).put_Fill((Brush) new SolidColorBrush(Colors.Transparent));
    }
  }
}
