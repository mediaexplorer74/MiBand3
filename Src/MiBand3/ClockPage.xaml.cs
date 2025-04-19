using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


namespace MiBand3
{
    public sealed partial class ClockPage : Page
    {
        public ClockPage()
        {
            InitializeComponent();

            lblNow.Text = $"{DateTime.Now:dddd, MM/dd/yyyy HH:mm}";
            lblThisDevice.Text = $"This Device: {DateTime.Now:dddd, MM/dd/yyyy HH:mm}";
            lblLastSync.Text = $"Sync date: {DateTime.Now:dddd, MM/dd/yyyy HH:mm}";
            lblOnBand.Text = $"On Band: {DateTime.Now:dddd, MM/dd/yyyy HH:mm}";
        }

        private void ClockPage_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void btnSync_Click(Object sender, RoutedEventArgs e)
        {
            // Add synchronization logic here
        }

       
    }
}


