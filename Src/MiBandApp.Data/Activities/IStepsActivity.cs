
// Type: MiBandApp.Data.Activities.IStepsActivity
// Assembly: MiBandApp.Data, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6F72CE9F-BCE5-4931-8D08-920048DF42FA
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.Data.dll

#nullable disable
namespace MiBandApp.Data.Activities
{
  public interface IStepsActivity : IUserActivity
  {
    int Steps { get; }

    int Calories { get; }

    double Distance { get; }
  }
}
