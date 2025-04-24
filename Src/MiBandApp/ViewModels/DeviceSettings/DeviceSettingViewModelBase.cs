
// Type: MiBandApp.ViewModels.DeviceSettings.DeviceSettingViewModelBase
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// 

using MiBandApp.Tools;
using System.Threading.Tasks;

#nullable disable
namespace MiBandApp.ViewModels.DeviceSettings
{
  public abstract class DeviceSettingViewModelBase : PropertyChangedEx
  {
    public virtual async Task Load()
    {
    }

    public virtual async Task Save()
    {
    }
  }
}
