// Decompiled with JetBrains decompiler
// Type: MiBand.SDK.Tools.ILog
// Assembly: MiBand.SDK, Version=0.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 152FA565-3249-4054-A361-01D9C0AFA7F1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBand.SDK.dll

#nullable disable
namespace MiBand.SDK.Tools
{
  public interface ILog
  {
    void Debug(string message, string source = null);

    void Info(string message, string source = null);

    void Warning(string message, string source = null);

    void Error(string message, string source = null);
  }
}
