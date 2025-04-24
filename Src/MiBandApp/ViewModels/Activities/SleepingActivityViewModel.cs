
// Type: MiBandApp.ViewModels.Activities.SleepingActivityViewModel
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// 

using Caliburn.Micro;
using MiBandApp.Data.Activities;
using MiBandApp.Storage.Tables;

#nullable disable
namespace MiBandApp.ViewModels.Activities
{
  public class SleepingActivityViewModel : IActivityViewModel
  {
    public SleepingActivityViewModel(DbSleepingActivity activity) => this.Activity = activity;

    public DbSleepingActivity Activity { get; set; }

    IDbUserActivity IActivityViewModel.Activity => (IDbUserActivity) this.Activity;

    public int DurationMin => this.Activity.GetDurationMin();

    public void OnTapped()
    {
      IoC.Get<INavigationService>().NavigateToViewModel<SleepDetailsPageViewModel>((object) this.Activity);
    }
  }
}
