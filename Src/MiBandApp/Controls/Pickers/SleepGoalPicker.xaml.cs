

// Type: MiBandApp.Controls.Pickers.SleepGoalPicker
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// 

using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

namespace MiBandApp.Controls.Pickers
{
    public sealed partial class SleepGoalPicker : UserControl
    {
        public SleepGoalPicker()
        {
            this.InitializeComponent();
            InitializeSelectors();
        }

        private void InitializeSelectors()
        {
            // Populate hours (0 to 23)
            var hours = new List<int>();
            for (int i = 0; i < 24; i++)
            {
                hours.Add(i);
            }
            //TODO
            //HourListSelector.ItemsSource = hours;

            // Populate minutes (0 to 59)
            var minutes = new List<int>();
            for (int i = 0; i < 60; i += 5) // Increment by 5 for simplicity
            {
                minutes.Add(i);
            }
            //TODO
            //MinutesListSelector.ItemsSource = minutes;
        }
    }
}


