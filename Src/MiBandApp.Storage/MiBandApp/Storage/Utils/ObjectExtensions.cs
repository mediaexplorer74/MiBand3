
// Type: MiBandApp.Storage.Utils.ObjectExtensions
// Assembly: MiBandApp.Storage, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 12BDBED5-65C6-40FC-A182-B057969144FE
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.Storage.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#nullable disable
namespace MiBandApp.Storage.Utils
{
  public static class ObjectExtensions
  {
    private static readonly Dictionary<Type, List<PropertyInfo>> WriteableProperties = new Dictionary<Type, List<PropertyInfo>>();

    public static void CopyFromBase<TBase, TDerived>(this TDerived derivedObject, TBase baseObject) where TDerived : TBase
    {
      foreach (PropertyInfo propertyInfo in ObjectExtensions.GetWriteablePropertiesOfType(typeof (TBase)))
        propertyInfo.SetValue((object) derivedObject, propertyInfo.GetValue((object) baseObject, (object[]) null), (object[]) null);
    }

    private static List<PropertyInfo> GetWriteablePropertiesOfType(Type type)
    {
      if (!ObjectExtensions.WriteableProperties.ContainsKey(type))
        ObjectExtensions.WriteableProperties[type] = type.GetRuntimeProperties().Where<PropertyInfo>((Func<PropertyInfo, bool>) (t => t.CanWrite)).ToList<PropertyInfo>();
      return ObjectExtensions.WriteableProperties[type];
    }
  }
}
