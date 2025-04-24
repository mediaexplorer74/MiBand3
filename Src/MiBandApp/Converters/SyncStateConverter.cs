
// Type: MiBandApp.Converters.SyncStateConverter
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// 

using MiBandApp.Services;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Resources;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

#nullable disable
namespace MiBandApp.Converters
{
  public class SyncStateConverter : IValueConverter
  {
    private readonly ResourceLoader _resourceLoader = new ResourceLoader();

    public object Convert(object value, Type targetType, object parameter, string language)
    {
      string paramString = (string) null;
      if (parameter != null)
        paramString = (string) parameter;
      BandSyncState syncState = (BandSyncState) value;
      if (targetType == typeof (Brush))
        return (object) this.GetBrushForState(syncState, paramString);
      if (targetType == typeof (string))
        return (object) this.GetStringForState(syncState);
      if (targetType == typeof (double))
        return (object) this.GetDoubleForState(syncState);
      if (targetType == typeof (bool))
        return (object) this.GetBoolForState(syncState, paramString);
      if (targetType == typeof (Visibility))
        return (object) this.GetVisibilityForState(syncState, paramString);
      throw new NotImplementedException();
    }

    private Visibility GetVisibilityForState(BandSyncState syncState, string paramString)
    {
      return syncState.ToString().Equals(paramString, StringComparison.OrdinalIgnoreCase) ? (Visibility) 0 : (Visibility) 1;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
      throw new NotImplementedException();
    }

    private string GetStringForState(BandSyncState syncState)
    {
      switch (syncState)
      {
        case BandSyncState.None:
          return this._resourceLoader.GetString("SyncStateNotSynchronized");
        case BandSyncState.InProgress:
          return this._resourceLoader.GetString("SyncStateSynchronizing");
        case BandSyncState.Success:
          return this._resourceLoader.GetString("SyncStateSynchronized");
        case BandSyncState.Failed:
          return this._resourceLoader.GetString("SyncStateSynchronizationError");
        case BandSyncState.Binding:
          return "";
        default:
          throw new ArgumentOutOfRangeException(nameof (syncState));
      }
    }

    private Brush GetBrushForState(BandSyncState syncState, string param)
    {
      switch (syncState)
      {
        case BandSyncState.None:
        case BandSyncState.Success:
        case BandSyncState.Binding:
          return param == "textblock" ? (Brush) ((IDictionary<object, object>) Application.Current.Resources)[(object) "AppForegroundDisabledBrush"] : (Brush) ((IDictionary<object, object>) Application.Current.Resources)[(object) "CommandBarBackgroundThemeBrush"];
        case BandSyncState.InProgress:
          return (Brush) ((IDictionary<object, object>) Application.Current.Resources)[(object) "AppAccentBrush"];
        case BandSyncState.Failed:
          return (Brush) new SolidColorBrush(Colors.Red);
        default:
          throw new ArgumentOutOfRangeException(nameof (syncState));
      }
    }

    private double GetDoubleForState(BandSyncState syncState)
    {
      switch (syncState)
      {
        case BandSyncState.None:
        case BandSyncState.InProgress:
        case BandSyncState.Success:
        case BandSyncState.Binding:
          return 0.0;
        case BandSyncState.Failed:
          return 100.0;
        default:
          throw new ArgumentOutOfRangeException(nameof (syncState));
      }
    }

    private bool GetBoolForState(BandSyncState syncState, string paramString)
    {
      return syncState.ToString().Equals(paramString, StringComparison.OrdinalIgnoreCase);
    }
  }
}
