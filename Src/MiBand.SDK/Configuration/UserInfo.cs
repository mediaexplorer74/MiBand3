// Decompiled with JetBrains decompiler
// Type: MiBand.SDK.Configuration.UserInfo
// Assembly: MiBand.SDK, Version=0.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 152FA565-3249-4054-A361-01D9C0AFA7F1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBand.SDK.dll

using System;

#nullable disable
namespace MiBand.SDK.Configuration
{
  public class UserInfo
  {
    public long UserId { get; set; }

    public bool IsMale { get; set; }

    public DateTimeOffset Birthday { get; set; }

    public int HeightCm { get; set; }

    public int WeightKg { get; set; }

    public string Name { get; set; }

    public override int GetHashCode()
    {
      return 3 * this.UserId.GetHashCode() + 5 * this.IsMale.GetHashCode() + 7 * this.Birthday.GetHashCode() + 13 * this.HeightCm.GetHashCode() + 17 * this.WeightKg.GetHashCode() + 19 * this.Name.GetHashCode() + int.MaxValue;
    }

    public override bool Equals(object obj)
    {
      if (obj == null)
        return false;
      UserInfo userInfo = obj as UserInfo;
      return !(userInfo == (UserInfo) null) && this.Equals(userInfo);
    }

    public bool Equals(UserInfo userInfo)
    {
      return (object) userInfo != null && this.HeightCm == userInfo.HeightCm && this.WeightKg == userInfo.WeightKg && this.Birthday == userInfo.Birthday && this.UserId == userInfo.UserId && this.IsMale == userInfo.IsMale && this.Name == userInfo.Name;
    }

    public static bool operator ==(UserInfo a, UserInfo b)
    {
      if ((object) a == (object) b)
        return true;
      return (object) a != null && (object) b != null && a.Equals(b);
    }

    public static bool operator !=(UserInfo a, UserInfo b) => !(a == b);
  }
}
