
// Type: MiBandApp.ViewModels.Activities.IActivityViewModel
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// 

using MiBandApp.Storage.Tables;

#nullable disable
namespace MiBandApp.ViewModels.Activities
{
  public interface IActivityViewModel
  {
    IDbUserActivity Activity { get; }
  }
}
