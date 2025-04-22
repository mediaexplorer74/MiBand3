// Decompiled with JetBrains decompiler
// Type: MiBandApp.Data.Activities.IUserActivityExtensions
// Assembly: MiBandApp.Data, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6F72CE9F-BCE5-4931-8D08-920048DF42FA
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.Data.dll

#nullable disable
namespace MiBandApp.Data.Activities
{
  public static class IUserActivityExtensions
  {
    public static int GetDurationMin(this IUserActivity activity)
    {
      return (int) (activity.End - activity.Begin).TotalMinutes + 1;
    }
  }
}
