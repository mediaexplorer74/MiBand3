
// Type: MiBandApp.Services.StatusBarItem
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// 

#nullable disable
namespace MiBandApp.Services
{
  public abstract class StatusBarItem
  {
    internal StatusBarItem(string text) => this.Text = text;

    public string Text { get; set; }
  }
}
