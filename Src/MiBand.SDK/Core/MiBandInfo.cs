// Decompiled with JetBrains decompiler
// Type: MiBand.SDK.Core.MiBandInfo
// Assembly: MiBand.SDK, Version=0.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 152FA565-3249-4054-A361-01D9C0AFA7F1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBand.SDK.dll

#nullable disable
namespace MiBand.SDK.Core
{
  internal class MiBandInfo : IMiBandInfo
  {
    public MiBandInfo(string name, string deviceId)
    {
      this.Name = name;
      this.DeviceId = deviceId;
    }

    public string Name { get; private set; }

    public string DeviceId { get; private set; }

    public long MacAddress { get; private set; }
  }
}
