// Decompiled with JetBrains decompiler
// Type: MiBandApp.ViewModels.DeviceSettings.FirmwareViewModel
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.exe

using Caliburn.Micro;
using MiBand.SDK.FirmwareUpgrade;
using MiBandApp.Services;
using MiBandApp.Tools;
using System;
using System.Linq.Expressions;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.Streams;

#nullable disable
namespace MiBandApp.ViewModels.DeviceSettings
{
  public class FirmwareViewModel : DeviceSettingViewModelBase
  {
    private readonly BandController _bandController;
    private readonly INavigationService _navigationService;
    private Firmware _firmware;
    private string _firmwareVersion;

    public FirmwareViewModel(BandController bandController, INavigationService navigationService)
    {
      this._bandController = bandController;
      this._navigationService = navigationService;
    }

    public bool IsFirmwareNeedingUpgrade => this._bandController.FirmwareUpdateRecommended;

    public bool IsFirmwareUpgradeAvailable
    {
      get
      {
        Firmware firmware = this._firmware;
        return firmware != null && firmware.IsUpgradeAvailable(this._bandController.DeviceInfo.Value);
      }
    }

    public bool IsFirmwareUpgradeEnabled
    {
      get => this._bandController.CommunicationOperation.Value == CommunicationOperation.None;
    }

    public string NewerFirmwareVersion => this._firmware?.Version?.ToString();

    public bool IsFirmwareUpToDate => !this.IsFirmwareUpgradeAvailable;

    public string FirmwareVersion
    {
      get => this._firmwareVersion;
      set
      {
        if (value == this._firmwareVersion)
          return;
        this._firmwareVersion = value;
        this.NotifyOfPropertyChange(nameof (FirmwareVersion));
      }
    }

    public override async Task Load()
    {
      this.FirmwareVersion = this._bandController.DeviceInfo.Value.FirmwareVersion.ToString();
      string recommendedFileName = Firmware.GetRecommendedFileName(this._bandController.DeviceInfo.Value);
      if (recommendedFileName != null)
        this._firmware = Firmware.CreateForBand(this._bandController.DeviceInfo.Value, await FirmwareViewModel.ReadFirmwareFile(recommendedFileName).ConfigureAwait(true));
      this.NotifyOfPropertyChange("IsFirmwareUpgradeAvailable");
      this.NotifyOfPropertyChange("IsFirmwareNeedingUpgrade");
      this.NotifyOfPropertyChange("IsFirmwareUpgradeEnabled");
      this.NotifyOfPropertyChange("IsFirmwareUpToDate");
      this.NotifyOfPropertyChange("NewerFirmwareVersion");
      this._bandController.CommunicationOperation.Updated += new EventHandler<MonitorableUpdatedEventArgs<CommunicationOperation>>(this.CommunicationOperationOnUpdated);
    }

    public override async Task Save()
    {
      this._bandController.CommunicationOperation.Updated -= new EventHandler<MonitorableUpdatedEventArgs<CommunicationOperation>>(this.CommunicationOperationOnUpdated);
    }

    public void UpgradeFirmware()
    {
      this._navigationService.NavigateToViewModel<FirmwareUpgradePageViewModel>((object) this._firmware);
    }

    private static async Task<byte[]> ReadFirmwareFile(string fileName)
    {
      return (await FileIO.ReadBufferAsync((IStorageFile) await Package.Current.InstalledLocation.GetFileAsync("Firmware\\Files\\" + fileName).AsTask<StorageFile>().ConfigureAwait(false)).AsTask<IBuffer>().ConfigureAwait(false)).ToArray();
    }

    private void CommunicationOperationOnUpdated(
      object sender,
      MonitorableUpdatedEventArgs<CommunicationOperation> updatedEventArgs)
    {
      this.NotifyOfPropertyChangeAsync<bool>((Expression<Func<bool>>) (() => this.IsFirmwareUpgradeEnabled));
    }
  }
}
