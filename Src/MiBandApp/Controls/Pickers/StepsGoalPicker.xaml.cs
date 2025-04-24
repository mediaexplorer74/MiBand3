
// Type: MiBandApp.Controls.Pickers.StepsGoalPicker
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// 

using Ian.Controls;
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
    public sealed partial class StepsGoalPicker : UserControl
    {
        private const int MaxValue = 40000;
        private const int MinValue = 2000;
        private const int ValueStep = 1000;

        public DependencyProperty ValueProperty = DependencyProperty.Register(
            nameof(Value), typeof(int), typeof(StepsGoalPicker), 
            new PropertyMetadata((object)10000, 
                new PropertyChangedCallback(StepsGoalPicker.ValuePropertyChangedCallback)));

        private readonly ResourceLoader _resourceLoader = new ResourceLoader();
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

        public StepsGoalPicker()
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
            ((StepsGoalPicker)dependencyObject).UpdateSelectedValue();
        }

        private void UpdateSelectedValue()
        {
            if (this.Value < 2000)
                this.Value = 2000;
            else if (this.Value > 40000)
            {
                this.Value = 40000;
            }
            else
            {
                //TODO
                //this.OptionsListSelector.SelectedIndex = (this.Value - 2000) / 1000;
                this.MainButton.Content = this.CreateClosedStateString(this.Value);
            }
        }

        private void InitializeListSelector()
        {
            List<StepsGoalPicker.GoalOption> goalOptionList = new List<StepsGoalPicker.GoalOption>();
            for (int index = 2000; index <= 40000; index += 1000)
                goalOptionList.Add(new StepsGoalPicker.GoalOption()
                {
                    Steps = index,
                    StepsString = this._resourceLoader.GetString("UserInfoPageSteps")
                });
            //TODO
            //this.OptionsListSelector.ItemsSource = (IList)goalOptionList;
        }

        private string CreateClosedStateString(int steps)
        {
            return steps.ToString() + " " + this._resourceLoader.GetString("UserInfoPageSteps");
        }

        private void ListSelectorFlyout_OnConfirmed(object sender, EventArgs e)
        {
            //TODO
            //this.Value = this.OptionsListSelector.SelectedIndex * 1000 + 2000;
            //this.OptionsFlyout.Hide();
        }

        private void OptionsFlyout_OnOpened(object sender, object e)
        {
            //TODO
            //this.OptionsListSelector.IsActive = true;
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
            Application.LoadComponent((object)this, new Uri("ms-appx:///Controls/Pickers/StepsGoalPicker.xaml"), (ComponentResourceLocation)0);
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
                WindowsRuntimeMarshal.AddEventHandler<EventHandler<object>>(new Func<EventHandler<object>, EventRegistrationToken>(flyoutBase1.add_Opening), new Action<EventRegistrationToken>(flyoutBase1.remove_Opening), new EventHandler<object>(this.OptionsFlyout_OnOpening));
                FlyoutBase flyoutBase2 = (FlyoutBase)target;
                WindowsRuntimeMarshal.AddEventHandler<EventHandler<object>>(new Func<EventHandler<object>, EventRegistrationToken>(flyoutBase2.add_Closed), new Action<EventRegistrationToken>(flyoutBase2.remove_Closed), new EventHandler<object>(this.OptionsFlyout_OnClosed));
                ((ListSelectorFlyout)target).Confirmed += new EventHandler(this.ListSelectorFlyout_OnConfirmed);
                FlyoutBase flyoutBase3 = (FlyoutBase)target;
                WindowsRuntimeMarshal.AddEventHandler<EventHandler<object>>(new Func<EventHandler<object>, EventRegistrationToken>(flyoutBase3.add_Opened), new Action<EventRegistrationToken>(flyoutBase3.remove_Opened), new EventHandler<object>(this.OptionsFlyout_OnOpened));
            }
            this._contentLoaded = true;
        }*/

        private class GoalOption
        {
            public int Steps { get; set; }

            public string StepsString { get; set; }
        }
    }
}

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
    public sealed partial class StepsGoalPicker : UserControl
    {
        public StepsGoalPicker()
        {
            this.InitializeComponent();
        }
    }
}
*/