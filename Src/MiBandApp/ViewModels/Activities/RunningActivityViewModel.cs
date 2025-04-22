// Decompiled with JetBrains decompiler
// Type: MiBandApp.ViewModels.Activities.RunningActivityViewModel
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.exe

using MiBandApp.Storage.Tables;

#nullable disable
namespace MiBandApp.ViewModels.Activities
{
  public class RunningActivityViewModel : IActivityViewModel
  {
    private readonly DbRunningActivity _runningActivity;
    private readonly bool _isSmall;

    public RunningActivityViewModel(DbRunningActivity runningActivity, bool isSmall)
    {
      this._runningActivity = runningActivity;
      this._isSmall = isSmall;
    }

    public DbRunningActivity Activity => this._runningActivity;

    IDbUserActivity IActivityViewModel.Activity => (IDbUserActivity) this.Activity;

    public int DurationMin => this.Activity.GetDurationMin();

    public bool IsSmallRun => this._isSmall;
  }
}
