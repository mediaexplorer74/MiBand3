// Decompiled with JetBrains decompiler
// Type: Microsoft.Live.Serialization.JsonReader
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
  internal sealed class JsonReader : IDisposable
  {
    private const int MaxUserJsonDepth = 10;
    private readonly TextReader reader;
    private readonly int maxObjectDepth;
    private int currentDepth;
    private bool isDisposed;

    public JsonReader(string text)
      : this(text, true)
    {
    }

    public JsonReader(string text, bool trusted)
    {
      this.reader = (TextReader) new StringReader(text);
      this.maxObjectDepth = trusted ? 0 : 10;
    }

    public void Dispose()
    {
      if (this.isDisposed)
        return;
      if (this.reader != null)
        this.reader.Dispose();
      this.isDisposed = true;
    }

    public object ReadValue()
    {
      if (this.isDisposed)
        throw new ObjectDisposedException("Object was disposed.");
      ++this.currentDepth;
      if (this.maxObjectDepth != 0 && this.currentDepth > this.maxObjectDepth)
        throw new FormatException("JSON object is too deep.");
      object obj = (object) null;
      bool flag = false;
      char c = this.PeekNextSignificantCharacter();
      switch (c)
      {
        case '"':
        case '\'':
          obj = (object) this.ReadString();
          break;
        case '[':
          obj = (object) this.ReadArray();
          break;
        case '{':
          obj = (object) this.ReadObject();
          break;
        default:
          if (!char.IsDigit(c))
          {
            switch (c)
            {
              case '-':
              case '.':
                break;
              case 'f':
              case 't':
                obj = (object) this.ReadBoolean();
                goto label_13;
              case 'n':
                this.ReadNull();
                flag = true;
                goto label_13;
              default:
                goto label_13;
            }
          }
          obj = this.ReadNumber();
          break;
      }
label_13:
      if (obj == null && !flag)
        throw new FormatException("Invalid JSON text.");
      --this.currentDepth;
      return obj;
    }

    private char GetNextSignificantCharacter()
    {
      char c;
      do
      {
        c = this.ReadCharFromReader();
      }
      while (c != char.MinValue && char.IsWhiteSpace(c));
      return c;
    }

    private string GetCharacters(int count)
    {
      string empty = string.Empty;
      for (int index = 0; index < count; ++index)
      {
        char ch = this.ReadCharFromReader();
        if (ch == char.MinValue)
          return (string) null;
        empty += (string) (object) ch;
      }
      return empty;
    }

    private char PeekNextSignificantCharacter()
    {
      char c;
      for (c = this.PeekCharFromReader(); c != char.MinValue && char.IsWhiteSpace(c); c = this.PeekCharFromReader())
      {
        int num = (int) this.ReadCharFromReader();
      }
      return c;
    }

    private IList ReadArray()
    {
      IList list = (IList) new List<object>();
      int num1 = (int) this.ReadCharFromReader();
      while (true)
      {
        char ch = this.PeekNextSignificantCharacter();
        switch (ch)
        {
          case char.MinValue:
            goto label_2;
          case ']':
            goto label_3;
          default:
            if (list.Count != 0)
            {
              if (ch == ',')
              {
                int num2 = (int) this.ReadCharFromReader();
              }
              else
                goto label_6;
            }
            object obj = this.ReadValue();
            list.Add(obj);
            continue;
        }
      }
label_2:
      throw new FormatException("Unterminated array literal.");
label_3:
      int num3 = (int) this.ReadCharFromReader();
      return list;
label_6:
      throw new FormatException("Invalid array literal.");
    }

    private bool ReadBoolean()
    {
      string str = this.ReadName(false);
      if (str != null)
      {
        if (str.Equals("true", StringComparison.Ordinal))
          return true;
        if (str.Equals("false", StringComparison.Ordinal))
          return false;
      }
      throw new FormatException("Invalid boolean literal.");
    }

    private char PeekCharFromReader()
    {
      int num = this.reader.Peek();
      return num != -1 ? (char) num : throw new FormatException("Unexpected end of string.");
    }

    private char ReadCharFromReader()
    {
      int num = this.reader.Read();
      return num != -1 ? (char) num : throw new FormatException("Unexpected end of string.");
    }

    private string ReadName(bool allowQuotes)
    {
      switch (this.PeekNextSignificantCharacter())
      {
        case '"':
        case '\'':
          return allowQuotes ? this.ReadString() : (string) null;
        default:
          StringBuilder stringBuilder = new StringBuilder();
          while (true)
          {
            char c = this.PeekCharFromReader();
            if (c == '_' || char.IsLetterOrDigit(c))
            {
              int num = (int) this.ReadCharFromReader();
              stringBuilder.Append(c);
            }
            else
              break;
          }
          return stringBuilder.ToString();
      }
    }

    private void ReadNull()
    {
      string str = this.ReadName(false);
      if (str == null || !str.Equals("null", StringComparison.Ordinal))
        throw new FormatException("Invalid null literal.");
    }

    private object ReadNumber()
    {
      char ch = this.ReadCharFromReader();
      bool flag = ch == '.';
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(ch);
      while (true)
      {
        char c = this.PeekNextSignificantCharacter();
        if (!char.IsDigit(c))
        {
          if (c == '.' || c == '+' || c == '-' || char.ToLowerInvariant(c) == 'e')
            flag = true;
          else
            break;
        }
        int num = (int) this.ReadCharFromReader();
        stringBuilder.Append(c);
      }
      string s = stringBuilder.ToString();
      if (flag)
      {
        float result;
        if (float.TryParse(s, NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result))
          return (object) result;
      }
      else
      {
        int result1;
        if (int.TryParse(s, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result1))
          return (object) result1;
        long result2;
        if (long.TryParse(s, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result2))
          return (object) result2;
      }
      throw new FormatException("Invalid numeric literal.");
    }

    private IDictionary<string, object> ReadObject()
    {
      IDictionary<string, object> dictionary = (IDictionary<string, object>) new DynamicDictionary();
      int num1 = (int) this.ReadCharFromReader();
      while (true)
      {
        char ch = this.PeekNextSignificantCharacter();
        switch (ch)
        {
          case char.MinValue:
            goto label_2;
          case '}':
            goto label_3;
          default:
            if (dictionary.Count != 0)
            {
              if (ch == ',')
              {
                int num2 = (int) this.ReadCharFromReader();
              }
              else
                goto label_6;
            }
            string key = this.ReadName(true);
            if (this.PeekNextSignificantCharacter() == ':')
            {
              int num3 = (int) this.ReadCharFromReader();
              object obj = this.ReadValue();
              dictionary[key] = obj;
              continue;
            }
            goto label_9;
        }
      }
label_2:
      throw new FormatException("Unterminated object literal.");
label_3:
      int num4 = (int) this.ReadCharFromReader();
      return dictionary;
label_6:
      throw new FormatException("Invalid object literal.");
label_9:
      throw new FormatException("Unexpected name/value pair syntax in object literal.");
    }

    private string ReadString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      char ch1 = this.ReadCharFromReader();
      bool flag = false;
      while (true)
      {
        char ch2 = this.ReadCharFromReader();
        if (ch2 != char.MinValue)
        {
          if (flag)
          {
            switch (ch2)
            {
              case 'b':
                ch2 = '\b';
                break;
              case 'f':
                ch2 = '\f';
                break;
              case 'n':
                ch2 = '\n';
                break;
              case 'r':
                ch2 = '\r';
                break;
              case 't':
                ch2 = '\t';
                break;
              case 'u':
                string characters = this.GetCharacters(4);
                if (characters != null)
                {
                  ch2 = (char) int.Parse(characters, NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture);
                  break;
                }
                goto label_6;
            }
            stringBuilder.Append(ch2);
            flag = false;
          }
          else if (ch2 == '\\')
            flag = true;
          else if ((int) ch2 != (int) ch1)
            stringBuilder.Append(ch2);
          else
            goto label_17;
        }
        else
          break;
      }
      throw new FormatException("Unterminated string literal.");
label_6:
      throw new FormatException("Unterminated string literal.");
label_17:
      return stringBuilder.ToString();
    }
  }
}
