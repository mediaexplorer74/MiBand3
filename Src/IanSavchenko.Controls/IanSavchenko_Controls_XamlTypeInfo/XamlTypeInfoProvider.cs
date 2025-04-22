// Decompiled with JetBrains decompiler
// Type: IanSavchenko.Controls.IanSavchenko_Controls_XamlTypeInfo.XamlTypeInfoProvider
// Assembly: IanSavchenko.Controls, Version=0.8.2.0, Culture=neutral, PublicKeyToken=null
// MVID: C384A7D9-D254-451C-A544-CD6C2993240A
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\IanSavchenko.Controls.dll

using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

#nullable disable
namespace IanSavchenko.Controls.IanSavchenko_Controls_XamlTypeInfo
{
  [GeneratedCode("Microsoft.Windows.UI.Xaml.Build.Tasks", "4.0.0.0")]
  [DebuggerNonUserCode]
  internal class XamlTypeInfoProvider
  {
    private Dictionary<string, IXamlType> _xamlTypeCacheByName = new Dictionary<string, IXamlType>();
    private Dictionary<Type, IXamlType> _xamlTypeCacheByType = new Dictionary<Type, IXamlType>();
    private Dictionary<string, IXamlMember> _xamlMembers = new Dictionary<string, IXamlMember>();
    private string[] _typeNameTable;
    private Type[] _typeTable;

    public IXamlType GetXamlTypeByType(Type type)
    {
      IXamlType xamlType;
      if (this._xamlTypeCacheByType.TryGetValue(type, out xamlType))
        return xamlType;
      int typeIndex = this.LookupTypeIndexByType(type);
      if (typeIndex != -1)
        xamlType = this.CreateXamlType(typeIndex);
      if (xamlType != null)
      {
        this._xamlTypeCacheByName.Add(xamlType.FullName, xamlType);
        this._xamlTypeCacheByType.Add(xamlType.UnderlyingType, xamlType);
      }
      return xamlType;
    }

    public IXamlType GetXamlTypeByName(string typeName)
    {
      if (string.IsNullOrEmpty(typeName))
        return (IXamlType) null;
      IXamlType xamlType;
      if (this._xamlTypeCacheByName.TryGetValue(typeName, out xamlType))
        return xamlType;
      int typeIndex = this.LookupTypeIndexByName(typeName);
      if (typeIndex != -1)
        xamlType = this.CreateXamlType(typeIndex);
      if (xamlType != null)
      {
        this._xamlTypeCacheByName.Add(xamlType.FullName, xamlType);
        this._xamlTypeCacheByType.Add(xamlType.UnderlyingType, xamlType);
      }
      return xamlType;
    }

    public IXamlMember GetMemberByLongName(string longMemberName)
    {
      if (string.IsNullOrEmpty(longMemberName))
        return (IXamlMember) null;
      IXamlMember memberByLongName;
      if (this._xamlMembers.TryGetValue(longMemberName, out memberByLongName))
        return memberByLongName;
      IXamlMember xamlMember = this.CreateXamlMember(longMemberName);
      if (xamlMember != null)
        this._xamlMembers.Add(longMemberName, xamlMember);
      return xamlMember;
    }

    private void InitTypeTables()
    {
      this._typeNameTable = new string[10];
      this._typeNameTable[0] = "IanSavchenko.Controls.ListSelector";
      this._typeNameTable[1] = "Windows.UI.Xaml.Controls.Control";
      this._typeNameTable[2] = "Windows.UI.Xaml.DataTemplate";
      this._typeNameTable[3] = "Windows.UI.Xaml.Thickness";
      this._typeNameTable[4] = "Double";
      this._typeNameTable[5] = "System.Collections.IList";
      this._typeNameTable[6] = "Int32";
      this._typeNameTable[7] = "Boolean";
      this._typeNameTable[8] = "IanSavchenko.Controls.ListSelectorItem";
      this._typeNameTable[9] = "Object";
      this._typeTable = new Type[10];
      this._typeTable[0] = typeof (ListSelector);
      this._typeTable[1] = typeof (Control);
      this._typeTable[2] = typeof (DataTemplate);
      this._typeTable[3] = typeof (Thickness);
      this._typeTable[4] = typeof (double);
      this._typeTable[5] = typeof (IList);
      this._typeTable[6] = typeof (int);
      this._typeTable[7] = typeof (bool);
      this._typeTable[8] = typeof (ListSelectorItem);
      this._typeTable[9] = typeof (object);
    }

    private int LookupTypeIndexByName(string typeName)
    {
      if (this._typeNameTable == null)
        this.InitTypeTables();
      for (int index = 0; index < this._typeNameTable.Length; ++index)
      {
        if (string.CompareOrdinal(this._typeNameTable[index], typeName) == 0)
          return index;
      }
      return -1;
    }

    private int LookupTypeIndexByType(Type type)
    {
      if (this._typeTable == null)
        this.InitTypeTables();
      for (int index = 0; index < this._typeTable.Length; ++index)
      {
        if (type == this._typeTable[index])
          return index;
      }
      return -1;
    }

    private object Activate_0_ListSelector() => (object) new ListSelector();

    private object Activate_8_ListSelectorItem() => (object) new ListSelectorItem();

    private IXamlType CreateXamlType(int typeIndex)
    {
      XamlSystemBaseType xamlType = (XamlSystemBaseType) null;
      string fullName = this._typeNameTable[typeIndex];
      Type type = this._typeTable[typeIndex];
      switch (typeIndex)
      {
        case 0:
          XamlUserType xamlUserType1 = new XamlUserType(this, fullName, type, this.GetXamlTypeByName("Windows.UI.Xaml.Controls.Control"));
          xamlUserType1.Activator = new Activator(this.Activate_0_ListSelector);
          xamlUserType1.AddMemberName("ItemTemplate");
          xamlUserType1.AddMemberName("ItemMargin");
          xamlUserType1.AddMemberName("ItemWidth");
          xamlUserType1.AddMemberName("ItemHeight");
          xamlUserType1.AddMemberName("ItemsSource");
          xamlUserType1.AddMemberName("SelectedIndex");
          xamlUserType1.AddMemberName("IsActive");
          xamlUserType1.SetIsLocalType();
          xamlType = (XamlSystemBaseType) xamlUserType1;
          break;
        case 1:
          xamlType = new XamlSystemBaseType(fullName, type);
          break;
        case 2:
          xamlType = new XamlSystemBaseType(fullName, type);
          break;
        case 3:
          xamlType = new XamlSystemBaseType(fullName, type);
          break;
        case 4:
          xamlType = new XamlSystemBaseType(fullName, type);
          break;
        case 5:
          XamlUserType xamlUserType2 = new XamlUserType(this, fullName, type, (IXamlType) null);
          xamlUserType2.SetIsReturnTypeStub();
          xamlType = (XamlSystemBaseType) xamlUserType2;
          break;
        case 6:
          xamlType = new XamlSystemBaseType(fullName, type);
          break;
        case 7:
          xamlType = new XamlSystemBaseType(fullName, type);
          break;
        case 8:
          XamlUserType xamlUserType3 = new XamlUserType(this, fullName, type, this.GetXamlTypeByName("Windows.UI.Xaml.Controls.Control"));
          xamlUserType3.Activator = new Activator(this.Activate_8_ListSelectorItem);
          xamlUserType3.AddMemberName("IsSelected");
          xamlUserType3.AddMemberName("ItemTemplate");
          xamlUserType3.AddMemberName("ItemContent");
          xamlUserType3.AddMemberName("ItemIndex");
          xamlUserType3.SetIsLocalType();
          xamlType = (XamlSystemBaseType) xamlUserType3;
          break;
        case 9:
          xamlType = new XamlSystemBaseType(fullName, type);
          break;
      }
      return (IXamlType) xamlType;
    }

    private object get_0_ListSelector_ItemTemplate(object instance)
    {
      return (object) ((ListSelector) instance).ItemTemplate;
    }

    private void set_0_ListSelector_ItemTemplate(object instance, object Value)
    {
      ((ListSelector) instance).ItemTemplate = (DataTemplate) Value;
    }

    private object get_1_ListSelector_ItemMargin(object instance)
    {
      return (object) ((ListSelector) instance).ItemMargin;
    }

    private void set_1_ListSelector_ItemMargin(object instance, object Value)
    {
      ((ListSelector) instance).ItemMargin = (Thickness) Value;
    }

    private object get_2_ListSelector_ItemWidth(object instance)
    {
      return (object) ((ListSelector) instance).ItemWidth;
    }

    private void set_2_ListSelector_ItemWidth(object instance, object Value)
    {
      ((ListSelector) instance).ItemWidth = (double) Value;
    }

    private object get_3_ListSelector_ItemHeight(object instance)
    {
      return (object) ((ListSelector) instance).ItemHeight;
    }

    private void set_3_ListSelector_ItemHeight(object instance, object Value)
    {
      ((ListSelector) instance).ItemHeight = (double) Value;
    }

    private object get_4_ListSelector_ItemsSource(object instance)
    {
      return (object) ((ListSelector) instance).ItemsSource;
    }

    private void set_4_ListSelector_ItemsSource(object instance, object Value)
    {
      ((ListSelector) instance).ItemsSource = (IList) Value;
    }

    private object get_5_ListSelector_SelectedIndex(object instance)
    {
      return (object) ((ListSelector) instance).SelectedIndex;
    }

    private void set_5_ListSelector_SelectedIndex(object instance, object Value)
    {
      ((ListSelector) instance).SelectedIndex = (int) Value;
    }

    private object get_6_ListSelector_IsActive(object instance)
    {
      return (object) ((ListSelector) instance).IsActive;
    }

    private void set_6_ListSelector_IsActive(object instance, object Value)
    {
      ((ListSelector) instance).IsActive = (bool) Value;
    }

    private object get_7_ListSelectorItem_IsSelected(object instance)
    {
      return (object) ((ListSelectorItem) instance).IsSelected;
    }

    private void set_7_ListSelectorItem_IsSelected(object instance, object Value)
    {
      ((ListSelectorItem) instance).IsSelected = (bool) Value;
    }

    private object get_8_ListSelectorItem_ItemTemplate(object instance)
    {
      return (object) ((ListSelectorItem) instance).ItemTemplate;
    }

    private void set_8_ListSelectorItem_ItemTemplate(object instance, object Value)
    {
      ((ListSelectorItem) instance).ItemTemplate = (DataTemplate) Value;
    }

    private object get_9_ListSelectorItem_ItemContent(object instance)
    {
      return ((ListSelectorItem) instance).ItemContent;
    }

    private void set_9_ListSelectorItem_ItemContent(object instance, object Value)
    {
      ((ListSelectorItem) instance).ItemContent = Value;
    }

    private object get_10_ListSelectorItem_ItemIndex(object instance)
    {
      return (object) ((ListSelectorItem) instance).ItemIndex;
    }

    private void set_10_ListSelectorItem_ItemIndex(object instance, object Value)
    {
      ((ListSelectorItem) instance).ItemIndex = (int) Value;
    }

    private IXamlMember CreateXamlMember(string longMemberName)
    {
      XamlMember xamlMember = (XamlMember) null;
      switch (longMemberName)
      {
        case "IanSavchenko.Controls.ListSelector.IsActive":
          XamlUserType xamlTypeByName1 = (XamlUserType) this.GetXamlTypeByName("IanSavchenko.Controls.ListSelector");
          xamlMember = new XamlMember(this, "IsActive", "Boolean");
          xamlMember.SetIsDependencyProperty();
          xamlMember.Getter = new Getter(this.get_6_ListSelector_IsActive);
          xamlMember.Setter = new Setter(this.set_6_ListSelector_IsActive);
          break;
        case "IanSavchenko.Controls.ListSelector.ItemHeight":
          XamlUserType xamlTypeByName2 = (XamlUserType) this.GetXamlTypeByName("IanSavchenko.Controls.ListSelector");
          xamlMember = new XamlMember(this, "ItemHeight", "Double");
          xamlMember.SetIsDependencyProperty();
          xamlMember.Getter = new Getter(this.get_3_ListSelector_ItemHeight);
          xamlMember.Setter = new Setter(this.set_3_ListSelector_ItemHeight);
          break;
        case "IanSavchenko.Controls.ListSelector.ItemMargin":
          XamlUserType xamlTypeByName3 = (XamlUserType) this.GetXamlTypeByName("IanSavchenko.Controls.ListSelector");
          xamlMember = new XamlMember(this, "ItemMargin", "Windows.UI.Xaml.Thickness");
          xamlMember.SetIsDependencyProperty();
          xamlMember.Getter = new Getter(this.get_1_ListSelector_ItemMargin);
          xamlMember.Setter = new Setter(this.set_1_ListSelector_ItemMargin);
          break;
        case "IanSavchenko.Controls.ListSelector.ItemTemplate":
          XamlUserType xamlTypeByName4 = (XamlUserType) this.GetXamlTypeByName("IanSavchenko.Controls.ListSelector");
          xamlMember = new XamlMember(this, "ItemTemplate", "Windows.UI.Xaml.DataTemplate");
          xamlMember.SetIsDependencyProperty();
          xamlMember.Getter = new Getter(this.get_0_ListSelector_ItemTemplate);
          xamlMember.Setter = new Setter(this.set_0_ListSelector_ItemTemplate);
          break;
        case "IanSavchenko.Controls.ListSelector.ItemWidth":
          XamlUserType xamlTypeByName5 = (XamlUserType) this.GetXamlTypeByName("IanSavchenko.Controls.ListSelector");
          xamlMember = new XamlMember(this, "ItemWidth", "Double");
          xamlMember.SetIsDependencyProperty();
          xamlMember.Getter = new Getter(this.get_2_ListSelector_ItemWidth);
          xamlMember.Setter = new Setter(this.set_2_ListSelector_ItemWidth);
          break;
        case "IanSavchenko.Controls.ListSelector.ItemsSource":
          XamlUserType xamlTypeByName6 = (XamlUserType) this.GetXamlTypeByName("IanSavchenko.Controls.ListSelector");
          xamlMember = new XamlMember(this, "ItemsSource", "System.Collections.IList");
          xamlMember.SetIsDependencyProperty();
          xamlMember.Getter = new Getter(this.get_4_ListSelector_ItemsSource);
          xamlMember.Setter = new Setter(this.set_4_ListSelector_ItemsSource);
          break;
        case "IanSavchenko.Controls.ListSelector.SelectedIndex":
          XamlUserType xamlTypeByName7 = (XamlUserType) this.GetXamlTypeByName("IanSavchenko.Controls.ListSelector");
          xamlMember = new XamlMember(this, "SelectedIndex", "Int32");
          xamlMember.SetIsDependencyProperty();
          xamlMember.Getter = new Getter(this.get_5_ListSelector_SelectedIndex);
          xamlMember.Setter = new Setter(this.set_5_ListSelector_SelectedIndex);
          break;
        case "IanSavchenko.Controls.ListSelectorItem.IsSelected":
          XamlUserType xamlTypeByName8 = (XamlUserType) this.GetXamlTypeByName("IanSavchenko.Controls.ListSelectorItem");
          xamlMember = new XamlMember(this, "IsSelected", "Boolean");
          xamlMember.SetIsDependencyProperty();
          xamlMember.Getter = new Getter(this.get_7_ListSelectorItem_IsSelected);
          xamlMember.Setter = new Setter(this.set_7_ListSelectorItem_IsSelected);
          break;
        case "IanSavchenko.Controls.ListSelectorItem.ItemContent":
          XamlUserType xamlTypeByName9 = (XamlUserType) this.GetXamlTypeByName("IanSavchenko.Controls.ListSelectorItem");
          xamlMember = new XamlMember(this, "ItemContent", "Object");
          xamlMember.SetIsDependencyProperty();
          xamlMember.Getter = new Getter(this.get_9_ListSelectorItem_ItemContent);
          xamlMember.Setter = new Setter(this.set_9_ListSelectorItem_ItemContent);
          break;
        case "IanSavchenko.Controls.ListSelectorItem.ItemIndex":
          XamlUserType xamlTypeByName10 = (XamlUserType) this.GetXamlTypeByName("IanSavchenko.Controls.ListSelectorItem");
          xamlMember = new XamlMember(this, "ItemIndex", "Int32");
          xamlMember.Getter = new Getter(this.get_10_ListSelectorItem_ItemIndex);
          xamlMember.Setter = new Setter(this.set_10_ListSelectorItem_ItemIndex);
          break;
        case "IanSavchenko.Controls.ListSelectorItem.ItemTemplate":
          XamlUserType xamlTypeByName11 = (XamlUserType) this.GetXamlTypeByName("IanSavchenko.Controls.ListSelectorItem");
          xamlMember = new XamlMember(this, "ItemTemplate", "Windows.UI.Xaml.DataTemplate");
          xamlMember.SetIsDependencyProperty();
          xamlMember.Getter = new Getter(this.get_8_ListSelectorItem_ItemTemplate);
          xamlMember.Setter = new Setter(this.set_8_ListSelectorItem_ItemTemplate);
          break;
      }
      return (IXamlMember) xamlMember;
    }
  }
}
