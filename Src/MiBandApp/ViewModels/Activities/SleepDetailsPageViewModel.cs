
// Type: MiBandApp.ViewModels.Activities.SleepDetailsPageViewModel
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// 

using Caliburn.Micro;
using MiBandApp.Data;
using MiBandApp.Data.Sleep;
using MiBandApp.Storage.DataBases;
using MiBandApp.Storage.Tables;
using MiBandApp.Tools;
using MiBandApp.Views.Activities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.UI.Popups;

#nullable disable
namespace MiBandApp.ViewModels.Activities
{
  public class SleepDetailsPageViewModel : PageBaseViewModel, IViewAware
  {
    private readonly RawActivityDataBase _rawActivityDataBase;
    private readonly ActivitiesDataBase _activitiesDataBase;
    private List<SleepPhaseType> _sleepMinutes;
    private SleepPattern _sleepPattern;
    private SleepDetailsPage _sleepDetailsPage;

    public SleepDetailsPageViewModel(
      RawActivityDataBase rawActivityDataBase,
      ActivitiesDataBase activitiesDataBase)
    {
      this._rawActivityDataBase = rawActivityDataBase;
      this._activitiesDataBase = activitiesDataBase;
    }

    public event EventHandler<ViewAttachedEventArgs> ViewAttached;

    public DbSleepingActivity Parameter { get; set; }

    public List<SleepDetailsPageViewModel.SleepPhaseMinute> Awakenings { get; set; }

    public List<SleepDetailsPageViewModel.SleepPhaseMinute> DeepSleep { get; set; }

    public List<SleepDetailsPageViewModel.SleepPhaseMinute> LightSleep { get; set; }

    public List<SleepDetailsPageViewModel.SleepPhaseMinute> HeartRate { get; set; }

    public DbSleepingActivity Activity => this.Parameter;

    public int LightSleepMin { get; private set; }

    public bool HasHeartRate { get; set; }

    public int HeartRateMin { get; set; }

    public int HeartRateMax { get; set; }

    public int HeartRateAverage { get; set; }

    protected override async Task OnActivate()
    {
      Task.Run(new Func<Task>(this.PrepareData)).ConfigureAwait(false).NoAwait();
    }

    private async Task PrepareData()
    {
      this._sleepPattern = SleepPattern.FromString(this.Activity.SleepPatternString);
      this._sleepMinutes = this._sleepPattern.GetPhasesByMinute();
      int[] heartRatesFromString = HeartRateProcessor.GetHeartRatesFromString(this.Activity.HeartRateString);
      this.Awakenings = new List<SleepDetailsPageViewModel.SleepPhaseMinute>();
      this.DeepSleep = new List<SleepDetailsPageViewModel.SleepPhaseMinute>();
      this.LightSleep = new List<SleepDetailsPageViewModel.SleepPhaseMinute>();
      int i = 0;
      this.HeartRate = ((IEnumerable<int>) heartRatesFromString).Select<int, SleepDetailsPageViewModel.SleepPhaseMinute>((Func<int, SleepDetailsPageViewModel.SleepPhaseMinute>) (t => new SleepDetailsPageViewModel.SleepPhaseMinute(this.Activity.Begin.AddMinutes((double) i++), t, (double) i))).Where<SleepDetailsPageViewModel.SleepPhaseMinute>((Func<SleepDetailsPageViewModel.SleepPhaseMinute, bool>) (t => t.Value != 0)).ToList<SleepDetailsPageViewModel.SleepPhaseMinute>();
      if (this.HeartRate.Any<SleepDetailsPageViewModel.SleepPhaseMinute>())
      {
        this.HasHeartRate = true;
        this.SmootheHeartRateMovingAverage();
        double coeff = 1.0;
        while (this.HeartRate.Count > 100)
        {
          this.SimplifyHeartRateLine(coeff);
          coeff *= 1.5;
        }
        this.HeartRateMin = this.HeartRate.Min<SleepDetailsPageViewModel.SleepPhaseMinute>((Func<SleepDetailsPageViewModel.SleepPhaseMinute, int>) (t => t.Value));
        this.HeartRateMax = this.HeartRate.Max<SleepDetailsPageViewModel.SleepPhaseMinute>((Func<SleepDetailsPageViewModel.SleepPhaseMinute, int>) (t => t.Value));
        this.HeartRateAverage = (int) this.HeartRate.Average<SleepDetailsPageViewModel.SleepPhaseMinute>((Func<SleepDetailsPageViewModel.SleepPhaseMinute, double>) (t => (double) t.Value));
        this.NotifyOfPropertyChange("HasHeartRate");
        this.NotifyOfPropertyChange("HeartRateMin");
        this.NotifyOfPropertyChange("HeartRateMax");
        this.NotifyOfPropertyChange("HeartRateAverage");
      }
      foreach (SleepPhase phase in (IEnumerable<SleepPhase>) this._sleepPattern.Phases)
      {
        SleepPhase sleepPhase = phase;
        List<SleepDetailsPageViewModel.SleepPhaseMinute> awakenings = this.Awakenings;
        int num;
        List<SleepDetailsPageViewModel.SleepPhaseMinute> sleepPhaseMinuteList1;
        switch (sleepPhase.Type)
        {
          case SleepPhaseType.Deep:
            num = 1;
            sleepPhaseMinuteList1 = this.DeepSleep;
            break;
          case SleepPhaseType.Light:
          case SleepPhaseType.REM:
            num = 2;
            sleepPhaseMinuteList1 = this.LightSleep;
            this.LightSleepMin += sleepPhase.Length;
            break;
          case SleepPhaseType.Awake:
            num = 2;
            sleepPhaseMinuteList1 = this.Awakenings;
            break;
          default:
            throw new ArgumentOutOfRangeException();
        }
        DateTime beginSleep;
        if (sleepPhaseMinuteList1.RemoveAll((Predicate<SleepDetailsPageViewModel.SleepPhaseMinute>) (t => t.Minute >= (double) sleepPhase.BeginTime)) == 0)
        {
          List<SleepDetailsPageViewModel.SleepPhaseMinute> sleepPhaseMinuteList2 = sleepPhaseMinuteList1;
          beginSleep = this.Parameter.BeginSleep;
          SleepDetailsPageViewModel.SleepPhaseMinute sleepPhaseMinute = new SleepDetailsPageViewModel.SleepPhaseMinute(beginSleep.AddMinutes((double) sleepPhase.BeginTime - 0.1), 0, (double) sleepPhase.BeginTime - 0.1);
          sleepPhaseMinuteList2.Add(sleepPhaseMinute);
        }
        List<SleepDetailsPageViewModel.SleepPhaseMinute> sleepPhaseMinuteList3 = sleepPhaseMinuteList1;
        beginSleep = this.Parameter.BeginSleep;
        SleepDetailsPageViewModel.SleepPhaseMinute sleepPhaseMinute1 = new SleepDetailsPageViewModel.SleepPhaseMinute(beginSleep.AddMinutes((double) sleepPhase.BeginTime), num, (double) sleepPhase.BeginTime);
        sleepPhaseMinuteList3.Add(sleepPhaseMinute1);
        List<SleepDetailsPageViewModel.SleepPhaseMinute> sleepPhaseMinuteList4 = sleepPhaseMinuteList1;
        beginSleep = this.Parameter.BeginSleep;
        SleepDetailsPageViewModel.SleepPhaseMinute sleepPhaseMinute2 = new SleepDetailsPageViewModel.SleepPhaseMinute(beginSleep.AddMinutes((double) sleepPhase.EndTime), num, (double) sleepPhase.EndTime);
        sleepPhaseMinuteList4.Add(sleepPhaseMinute2);
        List<SleepDetailsPageViewModel.SleepPhaseMinute> sleepPhaseMinuteList5 = sleepPhaseMinuteList1;
        beginSleep = this.Parameter.BeginSleep;
        SleepDetailsPageViewModel.SleepPhaseMinute sleepPhaseMinute3 = new SleepDetailsPageViewModel.SleepPhaseMinute(beginSleep.AddMinutes((double) sleepPhase.EndTime + 0.1), 0, (double) sleepPhase.EndTime + 0.1);
        sleepPhaseMinuteList5.Add(sleepPhaseMinute3);
      }
      ((System.Action) (() =>
      {
        this.NotifyOfPropertyChange<int>((Expression<Func<int>>) (() => this.LightSleepMin));
        this._sleepDetailsPage.SetLightSleepSeriesItemsSource((IEnumerable) this.LightSleep);
        this._sleepDetailsPage.SetDeepSleepSeriesItemsSource((IEnumerable) this.DeepSleep);
        this._sleepDetailsPage.SetAwakeningsSeriesItemsSource((IEnumerable) this.Awakenings);
        if (!this.HasHeartRate)
          return;
        this._sleepDetailsPage.SetHeartRateSeriesItemSource((IEnumerable) this.HeartRate, this.HeartRateMin - 25, this.HeartRateMax + 25);
      })).OnUIThread();
    }

    private void SimplifyHeartRateLine(double coeff)
    {
      this.HeartRate = new RDP().DouglasPeucker((IList<Vector2>) this.HeartRate.Select<SleepDetailsPageViewModel.SleepPhaseMinute, Vector2>((Func<SleepDetailsPageViewModel.SleepPhaseMinute, Vector2>) (t => new Vector2()
      {
        X = t.Minute,
        Y = (double) t.Value
      })).ToArray<Vector2>(), coeff).Select<Vector2, SleepDetailsPageViewModel.SleepPhaseMinute>((Func<Vector2, SleepDetailsPageViewModel.SleepPhaseMinute>) (t => new SleepDetailsPageViewModel.SleepPhaseMinute(this.Activity.Begin.AddMinutes((double) (int) t.X), (int) t.Y, t.X))).Where<SleepDetailsPageViewModel.SleepPhaseMinute>((Func<SleepDetailsPageViewModel.SleepPhaseMinute, bool>) (t => t.Value != 0)).ToList<SleepDetailsPageViewModel.SleepPhaseMinute>();
    }

    private void SmootheHeartRateMovingAverage()
    {
      int[] numArray = new int[this.HeartRate.Count];
      for (int index = 1; index < this.HeartRate.Count - 1; ++index)
      {
        double num1 = (double) this.HeartRate[index].Value;
        double minute1 = this.HeartRate[index].Minute;
        double num2 = (double) this.HeartRate[index - 1].Value;
        double minute2 = this.HeartRate[index - 1].Minute;
        double num3 = (double) this.HeartRate[index + 1].Value;
        double minute3 = this.HeartRate[index + 1].Minute;
        double num4 = 0.5;
        double num5 = (1.0 - num4) * (minute1 - minute2) / (minute3 - minute2);
        double num6 = (1.0 - num4) * (minute3 - minute1) / (minute3 - minute2);
        double num7 = num4;
        double num8 = num1 * num7 + num2 * num5 + num3 * num6;
        numArray[index] = (int) num8;
      }
      for (int index = 1; index < this.HeartRate.Count - 1; ++index)
        this.HeartRate[index].Value = numArray[index];
    }

    private void MovingMedian()
    {
      int[] numArray = new int[this.HeartRate.Count];
      for (int index1 = 0; index1 < this.HeartRate.Count; ++index1)
      {
        List<int> intList = new List<int>();
        intList.Add(this.HeartRate[index1].Value);
        for (int index2 = index1 - 1; index2 > 0; --index2)
        {
          SleepDetailsPageViewModel.SleepPhaseMinute sleepPhaseMinute = this.HeartRate[index2];
          if (Math.Abs(this.HeartRate[index1].Minute - sleepPhaseMinute.Minute) <= 2.0)
            intList.Add(sleepPhaseMinute.Value);
          else
            break;
        }
        for (int index3 = index1 + 1; index3 < this.HeartRate.Count; ++index3)
        {
          SleepDetailsPageViewModel.SleepPhaseMinute sleepPhaseMinute = this.HeartRate[index3];
          if (Math.Abs(this.HeartRate[index1].Minute - sleepPhaseMinute.Minute) <= 2.0)
            intList.Add(sleepPhaseMinute.Value);
          else
            break;
        }
        intList.Sort();
        numArray[index1] = intList[intList.Count / 2];
      }
      for (int index = 1; index < this.HeartRate.Count - 1; ++index)
        this.HeartRate[index].Value = numArray[index];
    }

    public async void ShowWarningMessage()
    {
      ResourceLoader resourceLoader = new ResourceLoader();
      IUICommand iuiCommand = await new MessageDialog(resourceLoader.GetString("SleepDetailsPage_InfoMessage"), resourceLoader.GetString("MessageWarningHeader")).ShowAsync().AsTask<IUICommand>().ConfigureAwait(true);
    }

    public void AttachView(object view, object context = null)
    {
      this._sleepDetailsPage = (SleepDetailsPage) view;
    }

    public object GetView(object context = null) => throw new NotImplementedException();

    public class SleepPhaseMinute
    {
      public SleepPhaseMinute(DateTime time, int value, double minute)
      {
        this.Time = time;
        this.Value = value;
        this.Minute = minute;
      }

      public DateTime Time { get; set; }

      public int Value { get; set; }

      public double Minute { get; set; }
    }
  }
}
