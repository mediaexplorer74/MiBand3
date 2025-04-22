// Decompiled with JetBrains decompiler
// Type: MiBandApp.ViewModels.UserInfoPageViewModel
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.exe

using Caliburn.Micro;
using MiBand.SDK.Configuration;
using MiBandApp.Services;
using MiBandApp.Tools;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Phone.UI.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

#nullable disable
namespace MiBandApp.ViewModels
{
  public class UserInfoPageViewModel : PageBaseViewModel
  {
    private readonly INavigationService _navigationService;
    private readonly ResourceLoader _resourceLoader;
    private readonly MiBandApp.Storage.Settings.Settings _settings;
    private readonly BandController _bandController;
    private readonly StatusBarNotificationService _statusBarNotificationService;
    private int _genderIndex;
    private int _goalSteps;
    private UserInfo _info;
    private bool _isExiting;
    private bool _isPageEnabled = true;
    private int _goalSleepMinutes;

    public UserInfoPageViewModel(
      INavigationService navigationService,
      StatusBarNotificationService statusBarNotificationService,
      MiBandApp.Storage.Settings.Settings settings,
      BandController bandController)
    {
      this._navigationService = navigationService;
      this._statusBarNotificationService = statusBarNotificationService;
      this._settings = settings;
      this._bandController = bandController;
      this._resourceLoader = new ResourceLoader();
    }

    public bool IsInitiallyActivated { get; set; }

    public UserInfo Info
    {
      get => this._info;
      set
      {
        if (this._info == value)
          return;
        this._info = value;
        this.NotifyOfPropertyChange(nameof (Info));
      }
    }

    public int GoalSteps
    {
      get => this._goalSteps;
      set
      {
        if (this._goalSteps == value)
          return;
        this._goalSteps = value;
        this.NotifyOfPropertyChange(nameof (GoalSteps));
      }
    }

    public int GoalSleepMinutes
    {
      get => this._goalSleepMinutes;
      set
      {
        if (this._goalSleepMinutes == value)
          return;
        this._goalSleepMinutes = value;
        this.NotifyOfPropertyChange(nameof (GoalSleepMinutes));
      }
    }

    public int GenderIndex
    {
      get => this._genderIndex;
      set
      {
        if (this._genderIndex == value)
          return;
        this._genderIndex = value;
        this.NotifyOfPropertyChange(nameof (GenderIndex));
      }
    }

    public DateTimeOffset MaxBirthdayTime => new DateTimeOffset(new DateTime(2010, 1, 1));

    public DateTimeOffset MinBirthdayTime => new DateTimeOffset(new DateTime(1920, 1, 1));

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

    public int Height
    {
      get => this._info.HeightCm;
      set
      {
        this._info.HeightCm = value;
        this.NotifyOfPropertyChange(nameof (Height));
      }
    }

    public int Weight
    {
      get => this._info.WeightKg;
      set
      {
        this._info.WeightKg = value;
        this.NotifyOfPropertyChange(nameof (Weight));
      }
    }

    public async void OnHeaderDoubleTapped()
    {
      string str = this._settings.GetUserId().ToString();
      ContentDialog contentDialog = new ContentDialog();
      contentDialog.put_Title((object) "User ID");
      contentDialog.put_IsPrimaryButtonEnabled(true);
      contentDialog.put_PrimaryButtonText("Ok");
      TextBox textBox = new TextBox();
      textBox.put_Text(str);
      textBox.put_IsReadOnly(true);
      ((ContentControl) contentDialog).put_Content((object) textBox);
      ContentDialogResult contentDialogResult = await contentDialog.ShowAsync();
    }

    protected override async Task OnActivate()
    {
      this._navigationService.BackPressed += new EventHandler<BackPressedEventArgs>(this.NavigationServiceOnBackPressed);
      this.Info = this._settings.GetSavedUserInfo();
      GoalInfo savedGoalInfo = this._settings.GetSavedGoalInfo();
      this.GoalSteps = savedGoalInfo.StepsGoal;
      this.GoalSleepMinutes = savedGoalInfo.SleepGoalMinutes;
      this.GenderIndex = this.Info.IsMale ? 0 : 1;
      if (!this.IsInitiallyActivated)
        return;
      IUICommand iuiCommand = await new MessageDialog(this._resourceLoader.GetString("UserInfoPageWelcomeMessage"), this._resourceLoader.GetString("UserInfoPageWelcomeMessageTitle")).ShowAsyncSafe();
    }

    private async void NavigationServiceOnBackPressed(
      object sender,
      BackPressedEventArgs backPressedEventArgs)
    {
      backPressedEventArgs.put_Handled(true);
      if (this._isExiting)
        return;
      this._isExiting = true;
      await this.SaveData().ConfigureAwait(true);
      this._navigationService.GoBack();
    }

    private async Task SaveData()
    {
      this._info.IsMale = this.GenderIndex == 0;
      GoalInfo goalInfo = new GoalInfo()
      {
        SleepGoalMinutes = this._goalSleepMinutes,
        StepsGoal = this._goalSteps
      };
      if (this._settings.GetSavedUserInfo().Equals(this._info) && this._settings.GetSavedGoalInfo().Equals(goalInfo))
        return;
      this.IsPageEnabled = false;
      StatusBarProgressItem statusBarMessage = this._statusBarNotificationService.Show<StatusBarProgressItem>(new StatusBarProgressItem(this._resourceLoader.GetString("StatusSavingOnDevice"), new double?()));
      try
      {
        if (this._bandController.BindingState.Value == BindingState.Binded)
        {
          await this._bandController.MiBand.SetUserInfo(this._info, false).ConfigureAwait(true);
          ConfiguredTaskAwaitable configuredTaskAwaitable = this._bandController.MiBand.SetDateTime((DateTimeOffset) DateTime.Now).ConfigureAwait(true);
          await configuredTaskAwaitable;
          configuredTaskAwaitable = this._bandController.MiBand.SetStepsGoal(goalInfo.StepsGoal).ConfigureAwait(true);
          await configuredTaskAwaitable;
        }
        this._settings.SaveUserInfo(this._info);
        this._settings.SaveGoalInfo(goalInfo);
      }
      catch (Exception ex)
      {
        this._statusBarNotificationService.Show<StatusBarMessage>(new StatusBarMessage(this._resourceLoader.GetString("StatusFailedToSaveTryAgainLater"), TimeSpan.FromSeconds(3.0)));
      }
      statusBarMessage.Hide();
      this.IsPageEnabled = true;
    }

    protected override async Task OnDeactivate(bool close)
    {
      this._navigationService.BackPressed -= new EventHandler<BackPressedEventArgs>(this.NavigationServiceOnBackPressed);
    }
  }
}
