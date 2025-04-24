
// Type: MiBandApp.Controls.Pickers.HeightPicker
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// 

using Caliburn.Micro;
using Ian.Controls;
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
    public sealed partial class HeightPicker : UserControl
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
                //this.OptionsListSelector.SelectedIndex = this.Value - 100;
                string str = this.CreateCmString(this.Value);
                if (this._settings.ShowImperialUnits)
                    str = str + " (" + this.CreateImperialString(this.Value) + ")";
                this.MainButton.Content = str;
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
            //this.OptionsListSelector.ItemsSource = (IList)list;
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
            //this.Value = this.OptionsListSelector.SelectedIndex + 100;
            //this.OptionsFlyout.Hide();
        }

        private void OptionsFlyout_OnOpened(object sender, object e)
        {
            //this.OptionsListSelector.IsActive = true;
        }

        private void OptionsFlyout_OnClosed(object sender, object e)
        {
        }

        private void OptionsFlyout_OnOpening(object sender, object e) => this.UpdateSelectedValue();

       
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

