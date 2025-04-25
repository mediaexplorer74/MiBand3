
// Type: MiBandApp.Services.LicensingService
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// 

using MetroLog;
using MiBandApp.Tools;
//using Microsoft.HockeyApp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Store;
using Windows.Storage;

#nullable disable
namespace MiBandApp.Services
{
  public class LicensingService
  {
    private const int DaysOfDataValidity = 50;
    private const string LifetimeRefreshTimeKey = "ProLifetimeRefreshTime";
    private const string OneYearRefreshTimeKey = "Pro1YearRefreshTime";
    private const string LifetimeStatusKey = "ProLifetimeStatus";
    private const string OneYearStatusKey = "Pro1YearStatus";
    private const string ProVersionLifetimeProductName = "BindMiBandProForever";
    private const string ProVersion1YearProductName = "BindMiBandPRO1Year";
    private readonly Lazy<LicenseInformation> _licenseInformationLazy;
    private readonly ApplicationDataContainer _localSettings;
    private readonly ILogger _logger;

    public LicensingService(ILogManager logManager)
    {
      this._logger = logManager.GetLogger<LicensingService>();
      this._localSettings = ApplicationData.Current.LocalSettings;
      this._licenseInformationLazy = new Lazy<LicenseInformation>( 
          () => CurrentApp.LicenseInformation);
      Task.Run((Action) (() => this.UpdateLicensesStatus()));
    }

    public bool IsPro => this.IsProLifetimeActive || this.IsPro1YearActive;

    

    public bool IsProLifetimeActive
    {
      get
      {
        try
        {
          return (DateTime.Now - this.LifeTimeCheckTime).TotalDays < 50.0 
                && this.LifeTimeCheckTime < DateTime.Now 
                && this._localSettings.Values.GetValueOrDefault<bool>("ProLifetimeStatus") 
                || this._licenseInformationLazy.Value
                                       .ProductLicenses["BindMiBandProForever"].IsActive;
        }
        catch
        {
           return true;//false;
        }
      }
    }

    public bool IsPro1YearActive
    {
      get
      {
        try
        {
          return (DateTime.Now - this.OneYearCheckTime).TotalDays < 50.0 && this.OneYearCheckTime < DateTime.Now && ((IDictionary<string, object>) this._localSettings.Values).GetValueOrDefault<bool>("Pro1YearStatus") || this._licenseInformationLazy.Value.ProductLicenses["BindMiBandPRO1Year"].IsActive;
        }
        catch
        {
          return true;//false;
        }
      }
    }

    private DateTime LifeTimeCheckTime
    {
      get
      {
        return new DateTime(this._localSettings.Values.GetValueOrDefault<long>("ProLifetimeRefreshTime"));
      }
      set
      {
        this._localSettings.Values["ProLifetimeRefreshTime"] = (object) value.Ticks;
      }
    }

    private DateTime OneYearCheckTime
    {
      get
      {
        return new DateTime(this._localSettings.Values.GetValueOrDefault<long>("Pro1YearRefreshTime"));
      }
      set
      {
        this._localSettings.Values["Pro1YearRefreshTime"] = (object) value.Ticks;
      }
    }

    private bool LifeTimeSavedStatus
    {
      get
      {
        return this._localSettings.Values.GetValueOrDefault<bool>("ProLifetimeStatus");
      }
      set
      {
        this._localSettings.Values["ProLifetimeStatus"] = (object) value;
        this.LifeTimeCheckTime = DateTime.Now;
      }
    }

    private bool OneYearSavedStatus
    {
      get
      {
        return this._localSettings.Values.GetValueOrDefault<bool>("Pro1YearStatus");
      }
      set
      {
        this._localSettings.Values["Pro1YearStatus"] = (object) value;
        this.OneYearCheckTime = DateTime.Now;
      }
    }

    public async Task PurchaseLifetime()
    {
      ProductPurchaseStatus productPurchaseStatus = await this.PurchaseItem("BindMiBandProForever").ConfigureAwait(true);
      if (productPurchaseStatus != null && productPurchaseStatus != ProductPurchaseStatus.AlreadyPurchased)
        return;
      this.LifeTimeSavedStatus = true;
    }

    public async Task Purchase1Year()
    {
      ProductPurchaseStatus productPurchaseStatus = await this.PurchaseItem("BindMiBandPRO1Year").ConfigureAwait(true);
      if (productPurchaseStatus != null && productPurchaseStatus != ProductPurchaseStatus.AlreadyPurchased)
        return;
      this.OneYearSavedStatus = true;
    }

    private async Task<ProductPurchaseStatus> PurchaseItem(string itemId)
    {
      if (this._licenseInformationLazy.Value.ProductLicenses[itemId].IsActive)
        return (ProductPurchaseStatus) 1;

            //HockeyClient.Current.TrackEvent("RequestProductPurchaseAsync" + itemId);
            PurchaseResults purchaseResults = default;

            try
            {
                purchaseResults = default;//await CurrentApp.RequestProductPurchaseAsync(itemId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[ex] RequestProductPurchaseAsync error: " + ex.Message);
            }

      this.UpdateLicensesStatus();

      //HockeyClient.Current.TrackEvent("RequestProductPurchaseAsync" + itemId + (object) purchaseResults.Status);
      return purchaseResults.Status;
    }

    private void UpdateLicensesStatus()
    {
      try
      {
        //if (this._licenseInformationLazy.Value.ProductLicenses["BindMiBandPRO1Year"].IsActive)
        //  this.OneYearSavedStatus = true;
        //if (!this._licenseInformationLazy.Value.ProductLicenses["BindMiBandProForever"].IsActive)
        //  return;
        this.LifeTimeSavedStatus = true;
      }
      catch (Exception ex)
      {
        this._logger.Warn("Failed to check license status: " + (object) ex, (Exception) null);
      }
    }
  }
}
