// Decompiled with JetBrains decompiler
// Type: MiBandApp.ViewModels.Activities.WalkingActivityViewModel
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.exe

using MiBandApp.Storage.Tables;

#nullable disable
namespace MiBandApp.ViewModels.Activities
{
  public class WalkingActivityViewModel : IActivityViewModel
  {
    private readonly DbWalkingActivity _walkingActivity;
    private readonly bool _isSmall;

    public WalkingActivityViewModel(DbWalkingActivity walkingActivity, bool isSmall)
    {
      this._walkingActivity = walkingActivity;
      this._isSmall = isSmall;
    }

    public DbWalkingActivity Activity => this._walkingActivity;

    IDbUserActivity IActivityViewModel.Activity => (IDbUserActivity) this.Activity;

    public int DurationMin => this.Activity.GetDurationMin();

    public bool IsSmallWalk => this._isSmall;

    public bool ShowOneTimestamp => this.Activity.Begin == this.Activity.End;

    public bool ShowDetails => !this.ShowOneTimestamp && !this.IsSmallWalk;
  }
}
