// Decompiled with JetBrains decompiler
// Type: MiBandApp.Data.Activities.WalkingActivity
// Assembly: MiBandApp.Data, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6F72CE9F-BCE5-4931-8D08-920048DF42FA
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.Data.dll

using System;
using System.Text;

#nullable disable
namespace MiBandApp.Data.Activities
{
  public class WalkingActivity : IUserActivity, IStepsActivity
  {
    public DateTime Begin { get; set; }

    public DateTime End { get; set; }

    public int Steps { get; set; }

    public int Calories { get; set; }

    public double Distance { get; set; }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("Walk begin: " + (object) this.Begin);
      stringBuilder.AppendLine("Walk end: " + (object) this.End);
      stringBuilder.AppendLine("Steps: " + (object) this.Steps);
      return stringBuilder.ToString();
    }
  }
}
