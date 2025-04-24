
// Type: Microsoft.Live.Serialization.JsonWriter
// Assembly: Microsoft.Live, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 84D9FE74-951C-4BB4-80F8-EDD369EAC36D
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\Microsoft.Live.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

#nullable disable
namespace Microsoft.Live.Serialization
{
  internal class JsonWriter
  {
    private static readonly string[] QuotedCharacterMap;
    private static readonly bool[] QuotedCharacters;
    private static readonly bool[] QuotedCharactersWithoutControlCharacters;
    private static readonly DateTime Epoch;
    private readonly IndentedTextWriter writer;
    private readonly Stack<JsonWriter.Scope> scopes;
    private readonly bool[] quotedCharacters;

    static JsonWriter()
    {
      string[] strArray = new string[256];
      bool[] flagArray1 = new bool[256];
      bool[] flagArray2 = new bool[256];
      for (int index = 0; index < 256; ++index)
        strArray[index] = "\\u" + index.ToString("x", (IFormatProvider) CultureInfo.InvariantCulture).PadLeft(4, '0');
      strArray[34] = "\\\"";
      strArray[92] = "\\\\";
      for (int index = 0; index < 256; ++index)
      {
        flagArray1[index] = index < 32 || index == 34 || index == 92;
        flagArray2[index] = index == 34 || index == 92;
      }
      JsonWriter.QuotedCharacterMap = strArray;
      JsonWriter.QuotedCharacters = flagArray1;
      JsonWriter.QuotedCharactersWithoutControlCharacters = flagArray2;
      JsonWriter.Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    }

    public JsonWriter(TextWriter writer)
      : this(writer, true)
    {
    }

    public JsonWriter(TextWriter writer, bool minimized)
      : this(writer, minimized, true)
    {
    }

    public JsonWriter(TextWriter writer, bool minimized, bool escapeControlCharacters)
    {
      this.writer = new IndentedTextWriter(writer, minimized);
      this.scopes = new Stack<JsonWriter.Scope>();
      this.quotedCharacters = escapeControlCharacters ? JsonWriter.QuotedCharacters : JsonWriter.QuotedCharactersWithoutControlCharacters;
    }

    public void WriteValue(object o)
    {
      switch (o)
      {
        case null:
          this.WriteCore("null", false);
          break;
        case bool flag:
          this.WriteValue(flag);
          break;
        case int num1:
          this.WriteValue(num1);
          break;
        case long num2:
          this.WriteValue(num2);
          break;
        case uint num3:
          this.WriteValue(num3);
          break;
        case ulong num4:
          this.WriteValue(num4);
          break;
        case byte num5:
          this.WriteValue(num5);
          break;
        case sbyte num6:
          this.WriteValue(num6);
          break;
        case short num7:
          this.WriteValue(num7);
          break;
        case ushort num8:
          this.WriteValue(num8);
          break;
        case Enum _:
          this.WriteValue((Enum) o);
          break;
        case float num9:
          this.WriteValue(num9);
          break;
        case DateTime dateTime:
          this.WriteValue(dateTime);
          break;
        case string _:
          this.WriteValue((string) o);
          break;
        case Guid guid:
          this.WriteValue(guid);
          break;
        case IJsonSerializable _:
          this.WriteValue((IJsonSerializable) o);
          break;
        case IDictionary<string, object> _:
          this.WriteValue((IDictionary<string, object>) o);
          break;
        case ICollection _:
          this.WriteValue((ICollection) o);
          break;
        default:
          throw new InvalidOperationException("unsupported type");
      }
    }

    protected virtual void WriteProperty(string name, object value)
    {
      this.WriteName(name);
      this.WriteValue(value);
    }

    protected virtual string NormalizeName(string name) => name;

    protected void WriteName(string name)
    {
      if (string.IsNullOrEmpty(name))
        throw new ArgumentNullException(nameof (name));
      JsonWriter.Scope scope = this.scopes.Count != 0 ? this.scopes.Peek() : throw new InvalidOperationException("No active scope to write into.");
      if (scope.Type != JsonWriter.ScopeType.Object)
        throw new InvalidOperationException("Names can only be written into Object scopes.");
      if (scope.ObjectCount != 0)
      {
        this.writer.WriteTrimmed(", ");
        this.writer.WriteLine();
      }
      ++scope.ObjectCount;
      this.WriteCore(this.NormalizeName(name), true);
      this.writer.WriteTrimmed(": ");
    }

    protected virtual void WriteValue(byte value)
    {
      this.WriteCore(value.ToString((IFormatProvider) CultureInfo.InvariantCulture), false);
    }

    protected virtual void WriteValue(sbyte value)
    {
      this.WriteCore(value.ToString((IFormatProvider) CultureInfo.InvariantCulture), false);
    }

    protected virtual void WriteValue(short value)
    {
      this.WriteCore(value.ToString((IFormatProvider) CultureInfo.InvariantCulture), false);
    }

    protected virtual void WriteValue(ushort value)
    {
      this.WriteCore(value.ToString((IFormatProvider) CultureInfo.InvariantCulture), false);
    }

    protected virtual void WriteValue(Enum value) => this.WriteCore(value.ToString(), true);

    protected virtual void WriteValue(bool value)
    {
      this.WriteCore(value ? "true" : "false", false);
    }

    protected virtual void WriteValue(int value)
    {
      this.WriteCore(value.ToString((IFormatProvider) CultureInfo.InvariantCulture), false);
    }

    protected virtual void WriteValue(uint value)
    {
      this.WriteCore(value.ToString((IFormatProvider) CultureInfo.InvariantCulture), false);
    }

    protected virtual void WriteValue(long value)
    {
      this.WriteCore(value.ToString((IFormatProvider) CultureInfo.InvariantCulture), false);
    }

    protected virtual void WriteValue(ulong value)
    {
      this.WriteCore(value.ToString((IFormatProvider) CultureInfo.InvariantCulture), false);
    }

    protected virtual void WriteValue(float value)
    {
      this.WriteCore(value.ToString((IFormatProvider) CultureInfo.InvariantCulture), false);
    }

    protected virtual void WriteValue(DateTime value)
    {
      this.WriteCore(Convert.ToInt64(value.ToUniversalTime().Subtract(JsonWriter.Epoch).TotalMilliseconds).ToString((IFormatProvider) CultureInfo.InvariantCulture), true);
    }

    protected virtual void WriteValue(string s)
    {
      if (s == null)
        this.WriteCore("null", false);
      else
        this.WriteCore(this.EscapeString(s), true);
    }

    protected virtual void WriteValue(Guid value) => this.WriteCore(value.ToString(), true);

    protected virtual void WriteValue(IJsonSerializable obj) => this.WriteCore(obj.ToJson(), false);

    protected virtual void WriteValue(ICollection items)
    {
      if (items == null || items.Count == 0)
      {
        this.WriteCore("[]", false);
      }
      else
      {
        this.StartArrayScope();
        foreach (object o in (IEnumerable) items)
          this.WriteValue(o);
        this.EndScope();
      }
    }

    protected virtual void WriteValue(IDictionary<string, object> record)
    {
      if (record == null || record.Count == 0)
      {
        this.WriteCore("{}", false);
      }
      else
      {
        this.StartObjectScope();
        foreach (string key in (IEnumerable<string>) record.Keys)
        {
          if (string.IsNullOrEmpty(key))
            throw new ArgumentException("Key of unsupported type contained in the dictionary.");
          this.WriteName(key);
          this.WriteValue(record[key]);
        }
        this.EndScope();
      }
    }

    protected void WriteCore(string text, bool quotes)
    {
      if (this.scopes.Count != 0)
      {
        JsonWriter.Scope scope = this.scopes.Peek();
        if (scope.Type == JsonWriter.ScopeType.Array)
        {
          if (scope.ObjectCount != 0)
          {
            this.writer.WriteTrimmed(", ");
            this.writer.WriteLine();
          }
          ++scope.ObjectCount;
        }
      }
      if (quotes)
        this.writer.Write('"');
      this.writer.Write(text);
      if (!quotes)
        return;
      this.writer.Write('"');
    }

    private string EscapeString(string s)
    {
      if (string.IsNullOrEmpty(s))
        return string.Empty;
      StringBuilder stringBuilder = (StringBuilder) null;
      int startIndex = 0;
      int count = 0;
      for (int index1 = 0; index1 < s.Length; ++index1)
      {
        char index2 = s[index1];
        if (index2 > 'ÿ' || this.quotedCharacters[(int) index2])
        {
          if (stringBuilder == null)
            stringBuilder = new StringBuilder(s.Length + 24);
          if (count > 0)
            stringBuilder.Append(s, startIndex, count);
          startIndex = index1 + 1;
          count = 0;
          if (index2 > 'ÿ')
          {
            if (index2 == '\u2028' || index2 == '\u2029')
              stringBuilder.Append("\\u" + ((int) index2).ToString("x", (IFormatProvider) CultureInfo.InvariantCulture).PadLeft(4, '0'));
            else
              stringBuilder.Append(index2);
          }
          else
            stringBuilder.Append(JsonWriter.QuotedCharacterMap[(int) index2]);
        }
        else
          ++count;
      }
      if (stringBuilder == null)
        return s;
      if (count > 0)
        stringBuilder.Append(s, startIndex, count);
      return stringBuilder.ToString();
    }

    private void EndScope()
    {
      if (this.scopes.Count == 0)
        throw new InvalidOperationException("No active scope to end.");
      this.writer.WriteLine();
      --this.writer.Indent;
      if (this.scopes.Pop().Type == JsonWriter.ScopeType.Array)
        this.writer.Write("]");
      else
        this.writer.Write("}");
    }

    private void StartArrayScope() => this.StartScope(JsonWriter.ScopeType.Array);

    private void StartObjectScope() => this.StartScope(JsonWriter.ScopeType.Object);

    private void StartScope(JsonWriter.ScopeType type)
    {
      if (this.scopes.Count != 0)
      {
        JsonWriter.Scope scope = this.scopes.Peek();
        if (scope.Type == JsonWriter.ScopeType.Array && scope.ObjectCount != 0)
          this.writer.WriteTrimmed(", ");
        ++scope.ObjectCount;
      }
      this.scopes.Push(new JsonWriter.Scope(type));
      if (type == JsonWriter.ScopeType.Array)
        this.writer.Write("[");
      else
        this.writer.Write("{");
      ++this.writer.Indent;
      this.writer.WriteLine();
    }

    private enum ScopeType
    {
      Array,
      Object,
    }

    private sealed class Scope
    {
      public readonly JsonWriter.ScopeType Type;
      public int ObjectCount;

      public Scope(JsonWriter.ScopeType type) => this.Type = type;
    }
  }
}
