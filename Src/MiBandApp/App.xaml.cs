// Decompiled with JetBrains decompiler
// Type: MiBandApp.App
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.exe

using Caliburn.Micro;
using MetroLog;
using MetroLog.Targets;
using MiBand.SDK;
using MiBand.SDK.Core;
using MiBandApp.Services;
using MiBandApp.Storage.DataBases;
using MiBandApp.Tools;
using MiBandApp.ViewModels;
using MiBandApp.ViewModels.Activities;
using MiBandApp.ViewModels.DeviceSettings;
using MiBandApp.ViewModels.MainPageComponents;
using MiBandApp.ViewModels.Tabs;
using MiBandApp.Views;
//using Microsoft.HockeyApp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Globalization.DateTimeFormatting;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

#nullable disable
namespace MiBandApp
{
    public sealed partial class App : CaliburnApplication
    {
        private WinRTContainer _container;
        private bool _disconnectedOnSuspend;
        private ILogger _logger;

        // constructor
        public App()
        {
            //HockeyClient.Current.Configure("9fed355a8cdf46aa9a592344589bbf96").RegisterCustomUnhandledExceptionLogic((Func<UnhandledExceptionEventArgs, bool>)(args =>
            //{
            //    this.LogUnhandledException(args.Exception);
            //    return true;
            //})).RegisterCustomUnobserveredTaskExceptionLogic((Func<UnobservedTaskExceptionEventArgs, bool>)(args =>
            //{
            //    this.LogUnhandledException((Exception)args.Exception);
            //    return true;
            //}));
            this.SetupLogging();
            this.InitializeComponent();
            this.SetupTimeFormat();
        }

        public static CultureInfo RegionalFormatCulture { get; set; }

        // Configure
        protected override void Configure()
        {
            this._container = new WinRTContainer();
            this._container.RegisterWinRTServices();
            BandLogger bandLogger = new BandLogger(LogManagerFactory.DefaultLogManager);
            this._container.RegisterInstance(typeof(ILogManager), "ILogManager", (object)LogManagerFactory.DefaultLogManager);
            this._container.RegisterInstance(typeof(IWinRTLogManager), "IWinRTLogManager", (object)LogManagerFactory.DefaultLogManager);
            this._container.RegisterInstance(typeof(MiBandLocator), (string)null, (object)new MiBandLocator((MiBand.SDK.Tools.ILog)bandLogger));
            this._container.RegisterSingleton(typeof(MiBandApp.Storage.Settings.Settings), (string)null, typeof(MiBandApp.Storage.Settings.Settings));
            this._container.RegisterSingleton(typeof(UpdatesHistoryService), (string)null, typeof(UpdatesHistoryService));
            this._container.RegisterSingleton(typeof(DataManager), (string)null, typeof(DataManager));
            this._container.RegisterSingleton(typeof(ActivitiesDataBase), (string)null, typeof(ActivitiesDataBase));
            this._container.RegisterSingleton(typeof(RawActivityDataBase), (string)null, typeof(RawActivityDataBase));
            this._container.RegisterSingleton(typeof(DaySummaryDataBase), (string)null, typeof(DaySummaryDataBase));
            this._container.RegisterSingleton(typeof(BandController), (string)null, typeof(BandController));
            this._container.RegisterSingleton(typeof(BandSyncReminderService), (string)null, typeof(BandSyncReminderService));
            this._container.RegisterSingleton(typeof(BandSyncController), (string)null, typeof(BandSyncController));
            this._container.RegisterSingleton(typeof(StatusBarNotificationService), (string)null, typeof(StatusBarNotificationService));
            this._container.RegisterSingleton(typeof(LicensingService), (string)null, typeof(LicensingService));
            this._container.RegisterSingleton(typeof(OneDriveSessionService), (string)null, typeof(OneDriveSessionService));
            this._container.RegisterSingleton(typeof(OneDriveSyncService), (string)null, typeof(OneDriveSyncService));
            this._container.RegisterSingleton(typeof(DiagnosticsService), (string)null, typeof(DiagnosticsService));
            this._container.RegisterSingleton(typeof(EmailComposer), (string)null, typeof(EmailComposer));
            this._container.RegisterSingleton(typeof(ProtocolActivationHandler), (string)null, typeof(ProtocolActivationHandler));
            this._container.RegisterSingleton(typeof(ClockService), (string)null, typeof(ClockService));
            this._container.RegisterSingleton(typeof(WalkTabViewModel), (string)null, typeof(WalkTabViewModel));
            this._container.RegisterSingleton(typeof(MainPageViewModel), (string)null, typeof(MainPageViewModel));
            this._container.RegisterSingleton(typeof(SleepTabViewModel), (string)null, typeof(SleepTabViewModel));
            this._container.RegisterSingleton(typeof(HeartRateTabViewModel), (string)null, typeof(HeartRateTabViewModel));
            this._container.RegisterSingleton(typeof(MessagesViewModel), (string)null, typeof(MessagesViewModel));
            this._container.PerRequest<DevicePageViewModel>().PerRequest<UserInfoPageViewModel>().PerRequest<AlarmsPageViewModel>().PerRequest<AboutPageViewModel>().PerRequest<PairingPageViewModel>().PerRequest<WhatsNewPageViewModel>().PerRequest<SettingsPageViewModel>().PerRequest<HistoryPageViewModel>().PerRequest<SleepDetailsPageViewModel>().PerRequest<DayDetailsPageViewModel>().PerRequest<FirmwareUpgradePageViewModel>();
            this._container.PerRequest<CloudViewModel>().PerRequest<BatteryViewModel>().PerRequest<FirmwareViewModel>().PerRequest<WearingLocationViewModel>().PerRequest<LedsColorViewModel>().PerRequest<HeartRateDuringSleepViewModel>().PerRequest<HighlightOnWristLiftViewModel>().PerRequest<DisplayViewModel>().PerRequest<ActivityReminderViewModel>().PerRequest<GoalReachedNotificationViewModel>().PerRequest<NotDisturbViewModel>().PerRequest<FlipDisplayOnWristRotateViewModel>();
            this.PrepareViewFirst();
        }


        // GetInstance
        protected override object GetInstance(Type service, string key)
        {
            return this._container.GetInstance(service, key) ?? throw new Exception("Could not locate any instances.");
        }

        // GetAllInstances
        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return this._container.GetAllInstances(service);
        }

        // BuildUp
        protected override void BuildUp(object instance)
        {
            this._container.BuildUp(instance);
        }


        // PrepareViewFirst
        protected override void PrepareViewFirst(Frame rootFrame)
        {
            ((IDictionary<object, object>)Application.Current.Resources)[(object)"AppSleepColor"] = (object)((Color)((IDictionary<object, object>)Application.Current.Resources)[(object)"PhoneForegroundColor"]).DarkenHsl(0.0);
            ((IDictionary<object, object>)Application.Current.Resources)[(object)"AppSleepBrush"] = (object)new SolidColorBrush((Color)((IDictionary<object, object>)Application.Current.Resources)[(object)"AppSleepColor"]);
            ResourceDictionary resources = Application.Current.Resources;
            SolidColorBrush solidColorBrush = new SolidColorBrush((Color)((IDictionary<object, object>)Application.Current.Resources)[(object)"AppSleepColor"]);
            solidColorBrush.Opacity = 0.7;
            ((IDictionary<object, object>)resources)[(object)"AppLightSleepBrush"] = (object)solidColorBrush;
            ((IDictionary<object, object>)Application.Current.Resources)[(object)"AppDeepSleepBrush"] = (object)new SolidColorBrush((Color)((IDictionary<object, object>)Application.Current.Resources)[(object)"AppSleepColor"]);
            this._container.RegisterNavigationService(rootFrame);
        }


        // OnActivated
        protected override async void OnActivated(IActivatedEventArgs args)
        {
            if (args.PreviousExecutionState != 1 && args.PreviousExecutionState != 2)
                this.DisplayRootView<MainPage>();
            if (args.Kind != 4)
                return;
            this._container.GetInstance<ILogManager>().GetLogger<App>().Debug(string.Format("Protocol activation in {0}", (object)nameof(OnActivated)), (Exception)null);
            ProtocolActivatedEventArgs args1 = args as ProtocolActivatedEventArgs;
            await this._container.GetInstance<ProtocolActivationHandler>().HandleProtocolActivation(args1).ConfigureAwait(false);
        }


        // OnLaunched
        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            if (args.PreviousExecutionState == 1 || args.PreviousExecutionState == 2)
                return;
            int num = await HockeyClient.Current.SendCrashesAsync(true).ConfigureAwait(true) ? 1 : 0;
            this.DisplayRootView<MainPage>();
            //HockeyClient.Current.TrackEvent(nameof(OnLaunched));
        }


        // OnSuspending
        protected override async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            this._logger.Debug("OnSuspending launched", (Exception)null);
            SuspendingDeferral deferral = e.SuspendingOperation.GetDeferral();
            try
            {
                BandController instance = this._container.GetInstance<BandController>();
                if (instance.Status != MiBandStatus.Connected)
                    return;
                this._logger.Debug("Disposing Mi Band...", (Exception)null);
                instance.MiBand.Dispose();
                this._disconnectedOnSuspend = true;
            }
            catch (Exception ex)
            {
                this._logger.Warn(string.Format("Exception in {0}", (object)nameof(OnSuspending)), ex);
            }
            finally
            {
                deferral.Complete();
            }
        }

        // OnResuming
        protected override void OnResuming(object sender, object e)
        {
            this._logger.Debug("OnResuming launched", (Exception)null);
            BandSyncController instance = this._container.GetInstance<BandSyncController>();
            if (!this._disconnectedOnSuspend)
                return;
            this._logger.Debug("Reconnecting after suspend", (Exception)null);
            instance.Refresh();
            this._disconnectedOnSuspend = false;
        }

        // LogUnhandledException
        private void LogUnhandledException(Exception ex)
        {
            this._container.GetInstance<ILogManager>().GetLogger<App>().Error("CRASH: " + (object)ex, (Exception)null);
        }

        // SetupTimeFormat
        private void SetupTimeFormat()
        {
            try
            {
                App.RegionalFormatCulture = new CultureInfo(new DateTimeFormatter("longdate", (IEnumerable<string>)new string[1]
                {
          "US"
                }).ResolvedLanguage);
                DateTimeFormatInfo dateTimeFormat = App.RegionalFormatCulture.DateTimeFormat;
                CultureInfo.CurrentUICulture.DateTimeFormat = dateTimeFormat;
                CultureInfo.CurrentCulture.DateTimeFormat = dateTimeFormat;
            }
            catch (Exception ex)
            {
            }
        }

        // SetupLogging
        private void SetupLogging()
        {
            LoggingConfiguration loggingConfiguration = new LoggingConfiguration();
            StreamingFileTarget streamingFileTarget = new StreamingFileTarget();
            streamingFileTarget.RetainDays = 7;
            loggingConfiguration.AddTarget(LogLevel.Trace, LogLevel.Fatal, (Target)streamingFileTarget);
            loggingConfiguration.IsEnabled = true;
            LogManagerFactory.DefaultConfiguration = loggingConfiguration;
            this._logger = LogManagerFactory.DefaultLogManager.GetLogger<App>();
        }//SetupLogging


    }//App

}//MiBandApp

