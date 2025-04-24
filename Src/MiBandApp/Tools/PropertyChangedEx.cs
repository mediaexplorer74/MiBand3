
// Type: MiBandApp.Tools.PropertyChangedEx
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// 

using Caliburn.Micro;
using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

#nullable disable
namespace MiBandApp.Tools
{
  public class PropertyChangedEx : PropertyChangedBase
  {
    public virtual async void NotifyOfPropertyChangeAsync([CallerMemberName] string propertyName = null)
    {
      if (!this.IsNotifying)
        return;
      await ((System.Action) (() => this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName)))).OnUIThreadAsync();
    }

    public void NotifyOfPropertyChangeAsync<TProperty>(Expression<Func<TProperty>> property)
    {
      this.NotifyOfPropertyChangeAsync(property.GetMemberInfo().Name);
    }
  }
}
