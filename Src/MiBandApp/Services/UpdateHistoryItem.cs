// Decompiled with JetBrains decompiler
// Type: MiBandApp.Services.UpdateHistoryItem
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.exe

#nullable disable
namespace MiBandApp.Services
{
  public class UpdateHistoryItem
  {
    public UpdateHistoryItem(int id, string version, string description, string message)
    {
      this.Version = version;
      this.Message = message;
      this.Description = description;
      this.Id = id;
    }

    public string Message { get; private set; }

    public string Description { get; private set; }

    public string Version { get; private set; }

    public int Id { get; private set; }
  }
}
