
// Type: MiBandApp.ViewModels.AlarmsPageViewModel
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// 

using Caliburn.Micro;
using MiBand.SDK.Configuration;
using MiBand.SDK.Core;
using MiBandApp.Services;
using MiBandApp.Tools;
using MiBandApp.ViewModels.Band;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Phone.UI.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

#nullable disable
namespace MiBandApp.ViewModels
{
  public class AlarmsPageViewModel : PageBaseViewModel
  {
    private readonly StatusBarNotificationService _statusBarNotificationService;
    private readonly BandController _bandController;
    private readonly ClockService _clockService;
    private readonly MiBandApp.Storage.Settings.Settings _settings;
    private readonly INavigationService _navigationService;
    private readonly ResourceLoader _resourceLoader = new ResourceLoader();
    private int _selectionIndex = -1;
    private bool _isPageEnabled = true;
    private bool _isExitingPage;
    private bool _alarmsChanged;

    public AlarmsPageViewModel(
      INavigationService navigationService,
      StatusBarNotificationService statusBarNotificationService,
      BandController bandController,
      ClockService clockService,
      MiBandApp.Storage.Settings.Settings settings)
    {
      this._statusBarNotificationService = statusBarNotificationService;
      this._bandController = bandController;
      this._clockService = clockService;
      this._settings = settings;
      this._navigationService = navigationService;
      this.Alarms = new ObservableCollection<BandAlarmViewModel>(this._settings.Alarms.GetAllSaved().Select<Alarm, BandAlarmViewModel>((Func<Alarm, BandAlarmViewModel>) (t => new BandAlarmViewModel(t, this))));
    }

    public ObservableCollection<BandAlarmViewModel> Alarms { get; set; }

    public bool AlarmsEmpty => this.Alarms.Count == 0;

    public int SelectionIndex
    {
      get => this._selectionIndex;
      set
      {
        if (this._selectionIndex == value)
          return;
        this._selectionIndex = value;
        this.NotifyOfPropertyChange(nameof (SelectionIndex));
      }
    }

    public bool IsSmartSupported
    {
      get
      {
        return this._bandController.DeviceInfo.Value.Capabilities.HasFlag((Enum) Capability.SmartAlarm);
      }
    }

    public bool IsPageEnabled
    {
      get => this._isPageEnabled;
      set
      {
        if (value == this._isPageEnabled)
          return;
        this._isPageEnabled = value;
        this.NotifyOfPropertyChange(nameof (IsPageEnabled));
      }
    }

    public int MaxAlarmCount => this._bandController.DeviceInfo.Value.AlarmsCount;

    public void OnSelectedChanged(SelectionChangedEventArgs args)
    {
      foreach (BandAlarmViewModel addedItem in (IEnumerable<object>) args.AddedItems)
        addedItem.IsUnfolded = true;
      foreach (BandAlarmViewModel removedItem in (IEnumerable<object>) args.RemovedItems)
        removedItem.IsUnfolded = false;
    }

    public void OnAlarmTapped(object sender)
    {
      if (sender == null || !((BandAlarmViewModel) sender).IsUnfolded)
        return;
      this.FoldAllAlarms();
    }

    public bool CanAddAlarm => this.Alarms.Count < this.MaxAlarmCount;

    public void AddAlarm()
    {
      this.Alarms.Add(new BandAlarmViewModel(new Alarm()
      {
        Days = DaysOfWeek.None,
        IsEnabled = false,
        Time = TimeSpan.FromHours(7.0)
      }, this));
      this.NotifyOfPropertyChange("CanAddAlarm");
      this.NotifyOfPropertyChange("AlarmsEmpty");
      this._alarmsChanged = true;
    }

    public void RemoveAlarm(BandAlarmViewModel alarmViewModel)
    {
      this.Alarms.Remove(alarmViewModel);
      this.NotifyOfPropertyChange("CanAddAlarm");
      this.NotifyOfPropertyChange("AlarmsEmpty");
      this._alarmsChanged = true;
    }

    public async void AlarmsNotWorking()
    {
      IUICommand iuiCommand = await new MessageDialog(this._resourceLoader.GetString("AlarmsPage_NotWorkingTip"), this._resourceLoader.GetString("MessageInformationHeader")).ShowAsync();
    }

    private void CheckOneTimeAlarms()
    {
      foreach (BandAlarmViewModel bandAlarmViewModel in this.Alarms.Where<BandAlarmViewModel>((Func<BandAlarmViewModel, bool>) (t => !t.Repeat && t.IsEnabled && !t.HasChanged)))
      {
        if (this._settings.Alarms.IsAlarmExpired(bandAlarmViewModel.GetAlarm()))
        {
          bandAlarmViewModel.DisableExpired();
          this._alarmsChanged = true;
        }
      }
    }

    private async void NavigationServiceOnBackPressed(
      object sender,
      EventArgs backPressedEventArgs)
    {
      if (this.Alarms.Any<BandAlarmViewModel>((Func<BandAlarmViewModel, bool>) (t => t.IsUnfolded)))
      {
        this.FoldAllAlarms();
        //backPressedEventArgs.Handled = true;
      }
      else
      {
        //backPressedEventArgs.Handled = true;
        if (this._isExitingPage)
          return;
        this._isExitingPage = true;
        if (this.Alarms.Any<BandAlarmViewModel>((Func<BandAlarmViewModel, bool>) (t => t.HasChanged))
                    || this._alarmsChanged)
          await this.SaveAll().ConfigureAwait(true);
        this._navigationService.GoBack();
        //this._navigationService.BackPressed -= new EventHandler<EventArgs>(this.NavigationServiceOnBackPressed);
      }
    }

    private void FoldAllAlarms()
    {
      foreach (BandAlarmViewModel alarm in (Collection<BandAlarmViewModel>) this.Alarms)
        alarm.IsUnfolded = false;
      this.SelectionIndex = -1;
    }

    private async Task SaveAll()
    {
      this.IsPageEnabled = false;
      StatusBarProgressItem statusBarMessage = this._statusBarNotificationService.Show<StatusBarProgressItem>(
          new StatusBarProgressItem(this._resourceLoader.GetString("StatusSavingOnDevice"), new double?()));
      try
      {
        ConfiguredTaskAwaitable configuredTaskAwaitable;
        if (this._bandController.DeviceInfo.Value.Model == MiBandModel.MiBand1)
        {
          configuredTaskAwaitable = this._bandController.MiBand.SetUserInfo(
              this._settings.GetSavedUserInfo(), false).ConfigureAwait(true);
          await configuredTaskAwaitable;
        }
        List<Alarm> savedAlarms = this._settings.Alarms.GetAllSaved(false).ToList<Alarm>();
        int maxAlarmCount = this._bandController.DeviceInfo.Value.AlarmsCount;
        for (int i = 0; i < savedAlarms.Count && i < maxAlarmCount; ++i)
        {
          if (i < this.Alarms.Count)
          {
            Alarm alarm1 = savedAlarms[i];
            Alarm alarm2 = this.Alarms[i].GetAlarm();
            if (alarm2 != alarm1 || this.Alarms[i].HasChanged)
            {
              configuredTaskAwaitable = this._bandController.MiBand.SetAlarm(i, alarm2).ConfigureAwait(true);
              await configuredTaskAwaitable;
            }
          }
          else
            await this._bandController.MiBand.SetAlarm(i, new Alarm()
            {
              IsEnabled = false
            });
        }
        for (int i = savedAlarms.Count; i < this.Alarms.Count; ++i)
        {
          configuredTaskAwaitable = this._bandController.MiBand.SetAlarm(i, 
              this.Alarms[i].GetAlarm()).ConfigureAwait(true);
          await configuredTaskAwaitable;
        }
        this._settings.Alarms.SaveAll((IList<Alarm>) this.Alarms.Select<BandAlarmViewModel, Alarm>(
            (Func<BandAlarmViewModel, Alarm>) (t => t.GetAlarm())).ToList<Alarm>());
        savedAlarms = (List<Alarm>) null;
      }
      catch (Exception ex)
      {
        this._statusBarNotificationService.Show<StatusBarMessage>(new StatusBarMessage(this._resourceLoader.GetString("StatusFailedToSaveTryAgainLater"), TimeSpan.FromSeconds(3.0)));
      }
      statusBarMessage.Hide();
      this.IsPageEnabled = true;
    }

    protected override async Task OnActivate()
    {
      //this._navigationService.BackPressed += new EventHandler<EventArgs>(this.NavigationServiceOnBackPressed);
      this._clockService.MinuteTick += new EventHandler<ClockTickEventArgs>(this.ClockServiceOnMinuteTick);
    }

    protected override async Task OnDeactivate(bool close)
    {
      //this._navigationService.BackPressed -= new EventHandler<EventArgs>(this.NavigationServiceOnBackPressed);
      this._clockService.MinuteTick -= new EventHandler<ClockTickEventArgs>(this.ClockServiceOnMinuteTick);
    }

    private void ClockServiceOnMinuteTick(object sender, ClockTickEventArgs clockTickEventArgs)
    {
      this.CheckOneTimeAlarms();
    }
  }
}
