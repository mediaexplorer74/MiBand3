/*using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace MiBandApp.Controls.Pickers
{
    public sealed partial class SleepGoalPicker : UserControl
    {
        public SleepGoalPicker()
        {
            this.InitializeComponent();
        }
    }
}
*/
// Decompiled with JetBrains decompiler
// Type: MiBandApp.Controls.Pickers.SleepGoalPicker
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.exe

using IanSavchenko.Controls;
using MiBandApp.Converters;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Markup;

#nullable disable
namespace MiBandApp.Controls.Pickers
{
    public sealed partial class SleepGoalPicker : UserControl
    {
        private const int MaxValue = 600;
        private const int MinValue = 180;
        private const int SmallStep = 15;
        private const int LargeStep = 60;
        public DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(int), typeof(SleepGoalPicker), new PropertyMetadata((object)480, new PropertyChangedCallback(SleepGoalPicker.ValuePropertyChangedCallback)));
        private readonly ResourceLoader _resourceLoader = new ResourceLoader();
        private readonly MinutesToStringConverter _minutesToStringConverter = new MinutesToStringConverter();
       /*[GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
        private UserControl Root;
        [GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
        private Button MainButton;
        [GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
        private ListSelectorFlyout OptionsFlyout;
        [GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
        private ListSelector HourListSelector;
        [GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
        private ListSelector MinutesListSelector;
        [GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
        private bool _contentLoaded;*/

        public SleepGoalPicker()
        {
            this.InitializeComponent();
            this.InitializeListSelector();
            this.UpdateSelectedValue();
        }

        public int Value
        {
            get => (int)((DependencyObject)this).GetValue(this.ValueProperty);
            set => ((DependencyObject)this).SetValue(this.ValueProperty, (object)value);
        }

        private static void ValuePropertyChangedCallback(
          DependencyObject dependencyObject,
          DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            ((SleepGoalPicker)dependencyObject).UpdateSelectedValue();
        }

        private void UpdateSelectedValue()
        {
            if (this.Value < 180)
                this.Value = 180;
            else if (this.Value > 600)
            {
                this.Value = 600;
            }
            else
            {
                int num1 = (this.Value - 180) / 60;
                int num2 = (this.Value - num1 * 60 - 180) / 15;
                this.HourListSelector.SelectedIndex = num1;
                this.MinutesListSelector.SelectedIndex = num2;
                ((ContentControl)this.MainButton).put_Content((object)this.CreateClosedStateString(this.Value));
            }
        }

        private void InitializeListSelector()
        {
            List<string> stringList1 = new List<string>();
            for (int index = 180; index <= 600; index += 60)
                stringList1.Add((index / 60).ToString() + this._resourceLoader.GetString("Duration_Hour_Short"));
            this.HourListSelector.ItemsSource = (IList)stringList1;
            List<string> stringList2 = new List<string>();
            for (int index = 0; index < 60; index += 15)
                stringList2.Add(index.ToString() + this._resourceLoader.GetString("Duration_Minute_Short"));
            this.MinutesListSelector.ItemsSource = (IList)stringList2;
        }

        private string CreateClosedStateString(int minutes)
        {
            return (string)this._minutesToStringConverter.Convert((object)minutes, typeof(string), (object)null, (string)null);
        }

        private void ListSelectorFlyout_OnConfirmed(object sender, EventArgs e)
        {
            this.Value = this.HourListSelector.SelectedIndex * 60 + this.MinutesListSelector.SelectedIndex * 15 + 180;
            ((FlyoutBase)this.OptionsFlyout).Hide();
        }

        private void OptionsFlyout_OnOpened(object sender, object e)
        {
            this.HourListSelector.IsActive = true;
        }

        private void OptionsFlyout_OnClosed(object sender, object e)
        {
        }

        private void OptionsFlyout_OnOpening(object sender, object e) => this.UpdateSelectedValue();

        /*[GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
        [DebuggerNonUserCode]
        public void InitializeComponent()
        {
            if (this._contentLoaded)
                return;
            this._contentLoaded = true;
            Application.LoadComponent((object)this, new Uri("ms-appx:///Controls/Pickers/SleepGoalPicker.xaml"), (ComponentResourceLocation)0);
            this.Root = (UserControl)((FrameworkElement)this).FindName("Root");
            this.MainButton = (Button)((FrameworkElement)this).FindName("MainButton");
            this.OptionsFlyout = (ListSelectorFlyout)((FrameworkElement)this).FindName("OptionsFlyout");
            this.HourListSelector = (ListSelector)((FrameworkElement)this).FindName("HourListSelector");
            this.MinutesListSelector = (ListSelector)((FrameworkElement)this).FindName("MinutesListSelector");
        }

        [GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
        [DebuggerNonUserCode]
        public void Connect(int connectionId, object target)
        {
            if (connectionId == 1)
            {
                FlyoutBase flyoutBase1 = (FlyoutBase)target;
                WindowsRuntimeMarshal.AddEventHandler<EventHandler<object>>(new Func<EventHandler<object>, EventRegistrationToken>(flyoutBase1.add_Closed), new Action<EventRegistrationToken>(flyoutBase1.remove_Closed), new EventHandler<object>(this.OptionsFlyout_OnClosed));
                ((ListSelectorFlyout)target).Confirmed += new EventHandler(this.ListSelectorFlyout_OnConfirmed);
                FlyoutBase flyoutBase2 = (FlyoutBase)target;
                WindowsRuntimeMarshal.AddEventHandler<EventHandler<object>>(new Func<EventHandler<object>, EventRegistrationToken>(flyoutBase2.add_Opened), new Action<EventRegistrationToken>(flyoutBase2.remove_Opened), new EventHandler<object>(this.OptionsFlyout_OnOpened));
                FlyoutBase flyoutBase3 = (FlyoutBase)target;
                WindowsRuntimeMarshal.AddEventHandler<EventHandler<object>>(new Func<EventHandler<object>, EventRegistrationToken>(flyoutBase3.add_Opening), new Action<EventRegistrationToken>(flyoutBase3.remove_Opening), new EventHandler<object>(this.OptionsFlyout_OnOpening));
            }
            this._contentLoaded = true;
        }*/
    }
}

