
// Type: MiBandApp.ViewModels.DeviceSettings.BatteryViewModel
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// 

using MiBand.SDK.Core;
using MiBandApp.Services;
using System.Threading.Tasks;

#nullable disable
namespace MiBandApp.ViewModels.DeviceSettings
{
  public class BatteryViewModel : DeviceSettingViewModelBase
  {
    private readonly BandController _bandController;
    private BatteryInfo _batteryInfo;

    public BatteryViewModel(BandController bandController) => this._bandController = bandController;

    public BatteryInfo BatteryInfo
    {
      get => this._batteryInfo;
      set
      {
        if (this._batteryInfo == value)
          return;
        this._batteryInfo = value;
        this.NotifyOfPropertyChange(nameof (BatteryInfo));
      }
    }

    public override async Task Load()
    {
      this.BatteryInfo = await this._bandController.MiBand.GetBatteryInfo(true).ConfigureAwait(false);
    }
  }
}
