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
    public sealed partial class HeightPicker : UserControl
    {
        public HeightPicker()
        {
            this.InitializeComponent();
        }
    }
}*/
// Decompiled with JetBrains decompiler
// Type: MiBandApp.Controls.Pickers.HeightPicker
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.exe

using Caliburn.Micro;
using IanSavchenko.Controls;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Markup;

#nullable disable
namespace MiBandApp.Controls.Pickers
{
    public sealed partial class : UserControl
    {
        private const int MaxValue = 220;
        private const int MinValue = 100;
        public DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(int), typeof(HeightPicker), new PropertyMetadata((object)175, new PropertyChangedCallback(HeightPicker.ValuePropertyChangedCallback)));
        private ResourceLoader _resourceLoader = new ResourceLoader();
        private MiBandApp.Storage.Settings.Settings _settings;
       

        public HeightPicker()
        {
            this.InitializeComponent();
            this._settings = IoC.Get<MiBandApp.Storage.Settings.Settings>();
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
            ((HeightPicker)dependencyObject).UpdateSelectedValue();
        }

        private void UpdateSelectedValue()
        {
            if (this.Value < 100)
                this.Value = 100;
            else if (this.Value > 220)
            {
                this.Value = 220;
            }
            else
            {
                this.OptionsListSelector.SelectedIndex = this.Value - 100;
                string str = this.CreateCmString(this.Value);
                if (this._settings.ShowImperialUnits)
                    str = str + " (" + this.CreateImperialString(this.Value) + ")";
                ((ContentControl)this.MainButton).put_Content((object)str);
            }
        }

        private void InitializeListSelector()
        {
            List<HeightPicker.HeightOption> list = Enumerable.Range(100, 121).Select<int, HeightPicker.HeightOption>((Func<int, HeightPicker.HeightOption>)(t => new HeightPicker.HeightOption()
            {
                HeightCm = t,
                CmString = this.CreateCmString(t)
            })).ToList<HeightPicker.HeightOption>();
            if (this._settings.ShowImperialUnits)
            {
                string str = (string)null;
                foreach (HeightPicker.HeightOption heightOption in list)
                {
                    heightOption.ImperialString = this.CreateImperialString(heightOption.HeightCm);
                    if (str == heightOption.ImperialString)
                        heightOption.ImperialString = "";
                    else
                        str = heightOption.ImperialString;
                }
            }
            this.OptionsListSelector.ItemsSource = (IList)list;
        }

        private string CreateCmString(int heightCm)
        {
            return heightCm.ToString() + " " + this._resourceLoader.GetString("UserInfoPageCentimeters");
        }

        private string CreateImperialString(int heightCm)
        {
            double num;
            return ((int)(num = (double)heightCm / 2.54) / 12).ToString() + "' " + (object)((int)num % 12) + "\"";
        }

        private void ListSelectorFlyout_OnConfirmed(object sender, EventArgs e)
        {
            this.Value = this.OptionsListSelector.SelectedIndex + 100;
            ((FlyoutBase)this.OptionsFlyout).Hide();
        }

        private void OptionsFlyout_OnOpened(object sender, object e)
        {
            this.OptionsListSelector.IsActive = true;
        }

        private void OptionsFlyout_OnClosed(object sender, object e)
        {
        }

        private void OptionsFlyout_OnOpening(object sender, object e) => this.UpdateSelectedValue();

        [GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
        [DebuggerNonUserCode]
        public void InitializeComponent()
        {
            if (this._contentLoaded)
                return;
            this._contentLoaded = true;
            Application.LoadComponent((object)this, new Uri("ms-appx:///Controls/Pickers/HeightPicker.xaml"), (ComponentResourceLocation)0);
            this.Root = (UserControl)((FrameworkElement)this).FindName("Root");
            this.MainButton = (Button)((FrameworkElement)this).FindName("MainButton");
            this.OptionsFlyout = (ListSelectorFlyout)((FrameworkElement)this).FindName("OptionsFlyout");
            this.OptionsListSelector = (ListSelector)((FrameworkElement)this).FindName("OptionsListSelector");
        }

        [GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
        [DebuggerNonUserCode]
        public void Connect(int connectionId, object target)
        {
            if (connectionId == 1)
            {
                FlyoutBase flyoutBase1 = (FlyoutBase)target;
                WindowsRuntimeMarshal.AddEventHandler<EventHandler<object>>(new Func<EventHandler<object>, EventRegistrationToken>(flyoutBase1.add_Closed), new Action<EventRegistrationToken>(flyoutBase1.remove_Closed), new EventHandler<object>(this.OptionsFlyout_OnClosed));
                FlyoutBase flyoutBase2 = (FlyoutBase)target;
                WindowsRuntimeMarshal.AddEventHandler<EventHandler<object>>(new Func<EventHandler<object>, EventRegistrationToken>(flyoutBase2.add_Opening), new Action<EventRegistrationToken>(flyoutBase2.remove_Opening), new EventHandler<object>(this.OptionsFlyout_OnOpening));
                ((ListSelectorFlyout)target).Confirmed += new EventHandler(this.ListSelectorFlyout_OnConfirmed);
                FlyoutBase flyoutBase3 = (FlyoutBase)target;
                WindowsRuntimeMarshal.AddEventHandler<EventHandler<object>>(new Func<EventHandler<object>, EventRegistrationToken>(flyoutBase3.add_Opened), new Action<EventRegistrationToken>(flyoutBase3.remove_Opened), new EventHandler<object>(this.OptionsFlyout_OnOpened));
            }
            this._contentLoaded = true;
        }

        private class HeightOption
        {
            public int HeightCm { get; set; }

            public string CmString { get; set; }

            public string ImperialString { get; set; }

            public Visibility ImperialLabelVisibility
            {
                get => this.ImperialString != null ? (Visibility)0 : (Visibility)1;
            }
        }
    }
}

