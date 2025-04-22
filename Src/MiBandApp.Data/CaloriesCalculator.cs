// Decompiled with JetBrains decompiler
// Type: MiBandApp.Data.CaloriesCalculator
// Assembly: MiBandApp.Data, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6F72CE9F-BCE5-4931-8D08-920048DF42FA
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.Data.dll

#nullable disable
namespace MiBandApp.Data
{
  public class CaloriesCalculator
  {
    private readonly UserParameters _userParameters;

    public CaloriesCalculator(UserParameters userParameters)
    {
      this._userParameters = userParameters;
    }

    public int GetCalories(double distanceKm, int lengthMin, bool isRunning)
    {
      double num = isRunning ? 0.2 : 0.1;
      return (int) ((double) (5 * this._userParameters.WeightKg) * (distanceKm * 1000.0 / (double) lengthMin * num + 3.5) * (double) lengthMin);
    }
  }
}
