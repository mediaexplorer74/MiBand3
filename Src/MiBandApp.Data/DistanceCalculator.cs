// Decompiled with JetBrains decompiler
// Type: MiBandApp.Data.DistanceCalculator
// Assembly: MiBandApp.Data, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6F72CE9F-BCE5-4931-8D08-920048DF42FA
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.Data.dll

using MiBandApp.Data.Activities;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace MiBandApp.Data
{
  public class DistanceCalculator
  {
    private readonly UserParameters _userParameters;
    private readonly Func<double, double> _stepLengthCmFunc;

    public DistanceCalculator(UserParameters userParameters)
    {
      this._userParameters = userParameters;
      this._stepLengthCmFunc = this._userParameters.IsMale ? new Func<double, double>(this.GetStepLengthCmMale) : new Func<double, double>(this.GetStepLengthCmFemale);
    }

    public double GetDistanceKm(List<ActivityMinuteData> activityMinuteData)
    {
      double num = Enumerable.Sum<ActivityMinuteData>((IEnumerable<ActivityMinuteData>) activityMinuteData, (Func<ActivityMinuteData, double>) (t => this.GetDistanceCmForMinute(t))) / 100.0 / 1000.0;
      return Math.Round(!this._userParameters.IsMale ? num * this.GetStepCorrectionFemale() : num * this.GetStepCorrectionMale(), 2, MidpointRounding.AwayFromZero);
    }

    private double GetDistanceCmForMinute(ActivityMinuteData activityMinuteData)
    {
      double num = (double) activityMinuteData.Steps / 60.0;
      if (num < 1.5)
        num = 1.5;
      return this._stepLengthCmFunc(num) * (double) activityMinuteData.Steps;
    }

    private double GetStepLengthCmMale(double stepsPerSecond) => 36.0 * stepsPerSecond + 10.2;

    private double GetStepLengthCmFemale(double stepsPerSecond) => 34.0 * stepsPerSecond + 7.1;

    private double GetStepCorrectionMale()
    {
      double num = (double) this._userParameters.HeightCm * 0.3937;
      if (num > 78.0)
        return 1.08;
      if (this._userParameters.HeightCm < 58)
        return 0.92;
      double x = num;
      return 1.0 + (0.003842775535 * Math.Pow(x, 3.0) - 0.7839262092 * Math.Pow(x, 2.0) + 53.65153019 * x - 1231.720859) / 100.0;
    }

    private double GetStepCorrectionFemale()
    {
      double num = (double) this._userParameters.HeightCm * 0.3937;
      if (num > 74.0)
        return 1.08;
      if (this._userParameters.HeightCm < 54)
        return 0.92;
      double x = num;
      return 1.0 + (0.003842775601 * Math.Pow(x, 3.0) - 0.7378129155 * Math.Pow(x, 2.0) + 47.56457455 * x - 1029.411637) / 100.0;
    }
  }
}
