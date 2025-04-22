// Decompiled with JetBrains decompiler
// Type: IanSavchenko.Controls.IanSavchenko_Controls_XamlTypeInfo.XamlMetaDataProvider
// Assembly: IanSavchenko.Controls, Version=0.8.2.0, Culture=neutral, PublicKeyToken=null
// MVID: C384A7D9-D254-451C-A544-CD6C2993240A
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\IanSavchenko.Controls.dll

using System;
using System.CodeDom.Compiler;
using Windows.UI.Xaml.Markup;

#nullable disable
namespace IanSavchenko.Controls.IanSavchenko_Controls_XamlTypeInfo
{
  [GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", "4.0.0.0")]
  public sealed class XamlMetaDataProvider : IXamlMetadataProvider
  {
    private XamlTypeInfoProvider _provider;

    public IXamlType GetXamlType(Type type)
    {
      if (this._provider == null)
        this._provider = new XamlTypeInfoProvider();
      return this._provider.GetXamlTypeByType(type);
    }

    public IXamlType GetXamlType(string fullName)
    {
      if (this._provider == null)
        this._provider = new XamlTypeInfoProvider();
      return this._provider.GetXamlTypeByName(fullName);
    }

    public XmlnsDefinition[] GetXmlnsDefinitions() => new XmlnsDefinition[0];
  }
}
