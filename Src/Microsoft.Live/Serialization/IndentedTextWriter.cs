
// Type: Microsoft.Live.Serialization.IndentedTextWriter
// Assembly: Microsoft.Live, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 84D9FE74-951C-4BB4-80F8-EDD369EAC36D
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\Microsoft.Live.dll

using System;
using System.Globalization;
using System.IO;
using System.Text;

#nullable disable
namespace Microsoft.Live.Serialization
{
  internal sealed class IndentedTextWriter : TextWriter
  {
    private readonly TextWriter writer;
    private readonly bool minimized;
    private int indentLevel;
    private bool tabsPending;
    private string tabString;

    public IndentedTextWriter(TextWriter writer, bool minimized)
      : base((IFormatProvider) CultureInfo.InvariantCulture)
    {
      this.writer = writer;
      this.minimized = minimized;
      this.NewLine = minimized ? "" : "\r";
      this.tabString = "   ";
      this.indentLevel = 0;
      this.tabsPending = false;
    }

    public override Encoding Encoding => this.writer.Encoding;

    public override string NewLine
    {
      get => this.writer.NewLine;
      set => this.writer.NewLine = value;
    }

    public int Indent
    {
      get => this.indentLevel;
      set
      {
        if (value < 0)
          value = 0;
        this.indentLevel = value;
      }
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
        this.writer.Dispose();
      base.Dispose(disposing);
    }

    public override void Flush() => this.writer.Flush();

    public override void Write(string s)
    {
      this.OutputTabs();
      this.writer.Write(s);
    }

    public override void Write(bool value)
    {
      this.OutputTabs();
      this.writer.Write(value);
    }

    public override void Write(char value)
    {
      this.OutputTabs();
      this.writer.Write(value);
    }

    public override void Write(char[] buffer)
    {
      this.OutputTabs();
      this.writer.Write(buffer);
    }

    public override void Write(char[] buffer, int index, int count)
    {
      this.OutputTabs();
      this.writer.Write(buffer, index, count);
    }

    public override void Write(double value)
    {
      this.OutputTabs();
      this.writer.Write(value);
    }

    public override void Write(float value)
    {
      this.OutputTabs();
      this.writer.Write(value);
    }

    public override void Write(int value)
    {
      this.OutputTabs();
      this.writer.Write(value);
    }

    public override void Write(long value)
    {
      this.OutputTabs();
      this.writer.Write(value);
    }

    public override void Write(object value)
    {
      this.OutputTabs();
      this.writer.Write(value);
    }

    public override void Write(string format, params object[] arg)
    {
      this.OutputTabs();
      this.writer.Write(format, arg);
    }

    public override void Write(uint value)
    {
      this.OutputTabs();
      this.writer.Write(value);
    }

    public void WriteLineNoTabs(string s) => this.writer.WriteLine(s);

    public override void WriteLine(string s)
    {
      this.OutputTabs();
      this.writer.WriteLine(s);
      this.tabsPending = true;
    }

    public override void WriteLine()
    {
      this.OutputTabs();
      this.writer.WriteLine();
      this.tabsPending = true;
    }

    public override void WriteLine(bool value)
    {
      this.OutputTabs();
      this.writer.WriteLine(value);
      this.tabsPending = true;
    }

    public override void WriteLine(char value)
    {
      this.OutputTabs();
      this.writer.WriteLine(value);
      this.tabsPending = true;
    }

    public override void WriteLine(char[] buffer)
    {
      this.OutputTabs();
      this.writer.WriteLine(buffer);
      this.tabsPending = true;
    }

    public override void WriteLine(char[] buffer, int index, int count)
    {
      this.OutputTabs();
      this.writer.WriteLine(buffer, index, count);
      this.tabsPending = true;
    }

    public override void WriteLine(double value)
    {
      this.OutputTabs();
      this.writer.WriteLine(value);
      this.tabsPending = true;
    }

    public override void WriteLine(float value)
    {
      this.OutputTabs();
      this.writer.WriteLine(value);
      this.tabsPending = true;
    }

    public override void WriteLine(int value)
    {
      this.OutputTabs();
      this.writer.WriteLine(value);
      this.tabsPending = true;
    }

    public override void WriteLine(long value)
    {
      this.OutputTabs();
      this.writer.WriteLine(value);
      this.tabsPending = true;
    }

    public override void WriteLine(object value)
    {
      this.OutputTabs();
      this.writer.WriteLine(value);
      this.tabsPending = true;
    }

    public override void WriteLine(string format, params object[] arg)
    {
      this.OutputTabs();
      this.writer.WriteLine(format, arg);
      this.tabsPending = true;
    }

    public override void WriteLine(uint value)
    {
      this.OutputTabs();
      this.writer.WriteLine(value);
      this.tabsPending = true;
    }

    public void WriteTrimmed(string text)
    {
      if (!this.minimized)
        this.Write(text);
      else
        this.Write(text.Trim());
    }

    private void OutputTabs()
    {
      if (!this.tabsPending)
        return;
      if (!this.minimized)
      {
        for (int index = 0; index < this.indentLevel; ++index)
          this.writer.Write(this.tabString);
      }
      this.tabsPending = false;
    }
  }
}
