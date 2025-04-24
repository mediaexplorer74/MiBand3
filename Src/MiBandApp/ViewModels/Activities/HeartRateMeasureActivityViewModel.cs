
// Type: MiBandApp.ViewModels.Activities.HeartRateMeasureActivityViewModel
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// 

using MiBandApp.Storage.Tables;

#nullable disable
namespace MiBandApp.ViewModels.Activities
{
  public class HeartRateMeasureActivityViewModel : IActivityViewModel
  {
    public HeartRateMeasureActivityViewModel(
      DbHeartRateMeasureActivity heartRateMeasureActivity)
    {
      this.Activity = heartRateMeasureActivity;
    }

    IDbUserActivity IActivityViewModel.Activity => (IDbUserActivity) this.Activity;

    public DbHeartRateMeasureActivity Activity { get; set; }
  }
}
