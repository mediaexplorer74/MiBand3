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
    public sealed partial class WeightPicker : UserControl
    {
        public WeightPicker()
        {
            this.InitializeComponent();
        }
    }
}*/
// Decompiled with JetBrains decompiler
// Type: MiBandApp.Controls.Pickers.WeightPicker
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
    public sealed partial class WeightPicker : UserControl
    {
        private const int MaxValue = 150;
        private const int MinValue = 30;
        public DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(int), 
                typeof(WeightPicker), new PropertyMetadata((object)75, 
                    new PropertyChangedCallback(WeightPicker.ValuePropertyChangedCallback)));

        private ResourceLoader _resourceLoader = new ResourceLoader();
        private MiBandApp.Storage.Settings.Settings _settings;
        /*[GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
        private UserControl Root;
        [GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
        private Button MainButton;
        [GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
        private ListSelectorFlyout OptionsFlyout;
        [GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
        private ListSelector OptionsListSelector;
        [GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", " 4.0.0.0")]
        private bool _contentLoaded;*/

        public WeightPicker()
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
            ((WeightPicker)dependencyObject).UpdateSelectedValue();
        }

        private void UpdateSelectedValue()
        {
            if (this.Value < 30)
                this.Value = 30;
            else if (this.Value > 150)
            {
                this.Value = 150;
            }
            else
            {
                this.OptionsListSelector.SelectedIndex = this.Value - 30;
                string str = this.CreateKgString(this.Value);
                if (this._settings.ShowImperialUnits)
                    str = str + " (" + this.CreateImperialString(this.Value) + ")";
                ((ContentControl)this.MainButton).put_Content((object)str);
            }
        }

        private void InitializeListSelector()
        {
            List<WeightPicker.WeightOption> list = Enumerable.Range(30, 121).Select<int, WeightPicker.WeightOption>((Func<int, WeightPicker.WeightOption>)(t => new WeightPicker.WeightOption()
            {
                WeightKg = t,
                KgString = this.CreateKgString(t)
            })).ToList<WeightPicker.WeightOption>();
            if (this._settings.ShowImperialUnits)
            {
                string str = (string)null;
                foreach (WeightPicker.WeightOption weightOption in list)
                {
                    weightOption.ImperialString = this.CreateImperialString(weightOption.WeightKg);
                    if (str == weightOption.ImperialString)
                        weightOption.ImperialString = "";
                    else
                        str = weightOption.ImperialString;
                }
            }
            this.OptionsListSelector.ItemsSource = (IList)list;
        }

        private string CreateKgString(int weightKg)
        {
            return weightKg.ToString() + " " + this._resourceLoader.GetString("UserInfoPageKilogram");
        }

        private string CreateImperialString(int weightKg)
        {
            return ((int)((double)weightKg * 2.204)).ToString() + " " + this._resourceLoader.GetString("UserInfoPagePounds");
        }

        private void ListSelectorFlyout_OnConfirmed(object sender, EventArgs e)
        {
            this.Value = this.OptionsListSelector.SelectedIndex + 30;
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

       

        private class WeightOption
        {
            public int WeightKg { get; set; }

            public string KgString { get; set; }

            public string ImperialString { get; set; }

            public Visibility ImperialLabelVisibility
            {
                get => this.ImperialString != null ? (Visibility)0 : (Visibility)1;
            }
        }
    }
}

