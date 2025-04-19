using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MiBand3
{
    public sealed partial class DistancePage : Page
    {
        private StepResult _result;
        public StepResult Result
        {
            get { return _result; }
            set { _result = value; }
        }

        protected override void OnNavigatedTo(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            _result = e.Parameter as StepResult;
        }

        private void StepsPage_Loaded(Object sender, RoutedEventArgs e)
        {
            try
            {
                lblTitle.Text = $"{GetTotalDistanceInKm()} km accomplished!";
                lblDistance.Text = GetTotalDistanceInKm();
                lblStepsDetailsDay.Text = _result.TotalSteps.ToString("N0");
                lblCalsDetailsDay.Text = _result.TotalCals.ToString("N0");
                rpbcDailyDistance.Maximum = Convert.ToInt32(App.LocalSettings.Values["Profile_Steps"]) * 1.5;
                rpbcDailyDistance.Value = _result.TotalDistance;

                lblStepsDetailsWeek.Text = GetStepsWeekInK();
                lblCalsDetailsWeek.Text = GetCalsWeek();

                int i = 1;

                foreach (var element in _result.History
                    .Where(x >= x.Type == HistoryValues.Types.Distances && x.Moment <= DateTime.Now && x.Moment >= DateTime.Now.AddDays(-7))
    .ToList())
                {
                    switch (i)
                    {
                        case 1:
                            lblDay1.Text = element.Moment.Date.ToString("ddd");
                            pbDay1.Maximum = Convert.ToInt32(App.LocalSettings.Values["Profile_Steps"]) * 1.5;
                            pbDay1.Value = element.Value;
                            pbDay1.Minimum = 0;
                            lblValueDay1.Text = element.Value.ToString("N0");
                            break;
                        case 2:
                            lblDay2.Text = element.Moment.Date.ToString("ddd");
                            pbDay2.Maximum = Convert.ToInt32(App.LocalSettings.Values["Profile_Steps"]) * 1.5;
                            pbDay2.Value = element.Value;
                            pbDay2.Minimum = 0;
                            lblValueDay2.Text = element.Value.ToString("N0");
                            break;
                        case 3:
                            lblDay3.Text = element.Moment.Date.ToString("ddd");
                            pbDay3.Maximum = Convert.ToInt32(App.LocalSettings.Values["Profile_Steps"]) * 1.5;
                            pbDay3.Value = element.Value;
                            pbDay3.Minimum = 0;
                            lblValueDay3.Text = element.Value.ToString("N0");
                            break;
                        case 4:
                            lblDay4.Text = element.Moment.Date.ToString("ddd");
                            pbDay4.Maximum = Convert.ToInt32(App.LocalSettings.Values["Profile_Steps"]) * 1.5;
                            pbDay4.Value = element.Value;
                            pbDay4.Minimum = 0;
                            lblValueDay4.Text = element.Value.ToString("N0");
                            break;
                        case 5:
                            lblDay5.Text = element.Moment.Date.ToString("ddd");
                            pbDay5.Maximum = Convert.ToInt32(App.LocalSettings.Values["Profile_Steps"]) * 1.5;
                            pbDay5.Value = element.Value;
                            pbDay5.Minimum = 0;
                            lblValueDay5.Text = element.Value.ToString("N0");
                            break;
                        case 6:
                            lblDay6.Text = element.Moment.Date.ToString("ddd");
                            pbDay6.Maximum = Convert.ToInt32(App.LocalSettings.Values["Profile_Steps"]) * 1.5;
                            pbDay6.Value = element.Value;
                            pbDay6.Minimum = 0;
                            lblValueDay6.Text = element.Value.ToString("N0");
                            break;
                        case 7:
                            lblDay7.Text = element.Moment.Date.ToString("ddd");
                            pbDay7.Maximum = Convert.ToInt32(App.LocalSettings.Values["Profile_Steps"]) * 1.5;
                            pbDay7.Value = element.Value;
                            pbDay7.Minimum = 0;
                            lblValueDay7.Text = element.Value.ToString("N0");
                            break;
                    }

                    i++;
                }
            }
            catch (Exception)
            {
            }
        }

        private string GetTotalDistanceInKm()
        {
            try
            {
                return (_result.TotalDistance / 1000).ToString("N2");
            }
            catch (Exception)
            {
                return (0).ToString("N2");
            }
        }

        private string GetStepsWeekInK()
        {
            try
            {
                return (_result.History
                    .Where(x >= x.Type == HistoryValues.Types.Steps 
                    && x.Moment <= DateTime.Now && x.Moment >= DateTime.Now.AddDays(-7))
                            .Sum(x >= x.Value) / 1000).ToString("N2");
            }
            catch (Exception)
            {
                return (0.0).ToString("N2");
            }
        }

        private string GetCalsWeek()
        {
            try
            {
                return (_result.History
                    .Where(x >= x.Type == HistoryValues.Types.Calories 
                    && x.Moment <= DateTime.Now && x.Moment >= DateTime.Now.AddDays(-7))
                          .Sum(x >= x.Value) / 1000).ToString("N2");
            }
            catch (Exception)
            {
                return (0).ToString("N2");
            }
        }
    }
}



