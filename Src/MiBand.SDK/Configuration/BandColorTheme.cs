// Decompiled with JetBrains decompiler
// Type: MiBand.SDK.Configuration.BandColorTheme
// Assembly: MiBand.SDK, Version=0.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 152FA565-3249-4054-A361-01D9C0AFA7F1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBand.SDK.dll

using System.Collections.Generic;
using Windows.UI.Xaml.Media;

#nullable disable
namespace MiBand.SDK.Configuration
{
  public struct BandColorTheme
  {
    private static readonly Dictionary<string, BandColorTheme> Colors = new Dictionary<string, BandColorTheme>();
    public const string RedThemeName = "Red";
    public const string BlueThemeName = "Blue";
    public const string GreenThemeName = "Green";
    public const string OrangeThemeName = "Orange";
    public const string WhiteThemeName = "White";
    public const string YellowThemeName = "Yellow";
    public const string PurpleThemeName = "Purple";
    public const string AquaThemeName = "Aqua";
    public const string PinkThemeName = "Pink";

    static BandColorTheme()
    {
      BandColorTheme.AddThemeToCollection(new BandColorTheme()
      {
        R = (byte) 6,
        G = (byte) 0,
        B = (byte) 0,
        DisplayColor = new SolidColorBrush(Windows.UI.Colors.OrangeRed),
        Name = "Red"
      });
      BandColorTheme.AddThemeToCollection(new BandColorTheme()
      {
        R = (byte) 0,
        G = (byte) 0,
        B = (byte) 6,
        DisplayColor = new SolidColorBrush(Windows.UI.Colors.Blue),
        Name = "Blue"
      });
      BandColorTheme.AddThemeToCollection(new BandColorTheme()
      {
        R = (byte) 0,
        G = (byte) 6,
        B = (byte) 0,
        DisplayColor = new SolidColorBrush(Windows.UI.Colors.LimeGreen),
        Name = "Green"
      });
      BandColorTheme.AddThemeToCollection(new BandColorTheme()
      {
        R = (byte) 6,
        G = (byte) 3,
        B = (byte) 0,
        DisplayColor = new SolidColorBrush(Windows.UI.Colors.Orange),
        Name = "Orange"
      });
      BandColorTheme.AddThemeToCollection(new BandColorTheme()
      {
        R = (byte) 6,
        G = (byte) 6,
        B = (byte) 6,
        DisplayColor = new SolidColorBrush(Windows.UI.Colors.White),
        Name = "White"
      });
      BandColorTheme.AddThemeToCollection(new BandColorTheme()
      {
        R = (byte) 6,
        G = (byte) 6,
        B = (byte) 0,
        DisplayColor = new SolidColorBrush(Windows.UI.Colors.Yellow),
        Name = "Yellow"
      });
      BandColorTheme.AddThemeToCollection(new BandColorTheme()
      {
        R = (byte) 0,
        G = (byte) 6,
        B = (byte) 6,
        DisplayColor = new SolidColorBrush(Windows.UI.Colors.Aqua),
        Name = "Aqua"
      });
      BandColorTheme.AddThemeToCollection(new BandColorTheme()
      {
        R = (byte) 3,
        G = (byte) 0,
        B = (byte) 3,
        DisplayColor = new SolidColorBrush(Windows.UI.Colors.Purple),
        Name = "Purple"
      });
      BandColorTheme.AddThemeToCollection(new BandColorTheme()
      {
        R = (byte) 6,
        G = (byte) 0,
        B = (byte) 3,
        DisplayColor = new SolidColorBrush(Windows.UI.Colors.DeepPink),
        Name = "Pink"
      });
    }

    public byte R { get; set; }

    public byte G { get; set; }

    public byte B { get; set; }

    public SolidColorBrush DisplayColor { get; set; }

    public string Name { get; set; }

    public static IReadOnlyDictionary<string, BandColorTheme> AllColors
    {
      get => (IReadOnlyDictionary<string, BandColorTheme>) BandColorTheme.Colors;
    }

    public static BandColorTheme FromName(string name)
    {
      return BandColorTheme.Colors.ContainsKey(name) ? BandColorTheme.Colors[name] : new BandColorTheme();
    }

    private static void AddThemeToCollection(BandColorTheme theme)
    {
      BandColorTheme.Colors[theme.Name] = theme;
    }
  }
}
