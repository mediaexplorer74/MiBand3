// Decompiled with JetBrains decompiler
// Type: Microsoft.Live.DynamicDictionary
// Assembly: Microsoft.Live, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 84D9FE74-951C-4BB4-80F8-EDD369EAC36D
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\Microsoft.Live.dll

using System.Collections;
using System.Collections.Generic;
using System.Dynamic;

#nullable disable
namespace Microsoft.Live
{
  internal sealed class DynamicDictionary : 
    DynamicObject,
    IDictionary<string, object>,
    ICollection<KeyValuePair<string, object>>,
    IEnumerable<KeyValuePair<string, object>>,
    IEnumerable
  {
    private readonly IDictionary<string, object> impl;

    public DynamicDictionary()
    {
      this.impl = (IDictionary<string, object>) new Dictionary<string, object>();
    }

    public ICollection<string> Keys => this.impl.Keys;

    public override bool TryGetMember(GetMemberBinder binder, out object result)
    {
      if (!this.impl.TryGetValue(binder.Name, out result))
        result = (object) null;
      return true;
    }

    public override bool TrySetMember(SetMemberBinder binder, object value)
    {
      this.impl[binder.Name] = value;
      return true;
    }

    public void Add(string key, object value) => this.impl.Add(key, value);

    public bool ContainsKey(string key) => this.impl.ContainsKey(key);

    public bool Remove(string key) => this.impl.Remove(key);

    public bool TryGetValue(string key, out object value) => this.impl.TryGetValue(key, out value);

    public ICollection<object> Values => this.impl.Values;

    public object this[string key]
    {
      get => this.impl[key];
      set => this.impl[key] = value;
    }

    public void Add(KeyValuePair<string, object> item) => this.impl.Add(item);

    public void Clear() => this.impl.Clear();

    public bool Contains(KeyValuePair<string, object> item) => this.impl.Contains(item);

    public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
    {
      this.impl.CopyTo(array, arrayIndex);
    }

    public int Count => this.impl.Count;

    public bool IsReadOnly => this.impl.IsReadOnly;

    public bool Remove(KeyValuePair<string, object> item)
    {
      return ((ICollection<KeyValuePair<string, object>>) this.impl).Remove(item);
    }

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => this.impl.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.impl.GetEnumerator();
  }
}
