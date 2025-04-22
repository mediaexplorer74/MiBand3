// Decompiled with JetBrains decompiler
// Type: MiBandApp.Data.Sleep.SleepPhase
// Assembly: MiBandApp.Data, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6F72CE9F-BCE5-4931-8D08-920048DF42FA
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.Data.dll

#nullable disable
namespace MiBandApp.Data.Sleep
{
  public struct SleepPhase
  {
    public SleepPhaseType Type { get; set; }

    public int Length { get; set; }

    public int BeginTime { get; set; }

    public int EndTime => this.BeginTime + this.Length;
  }
}
