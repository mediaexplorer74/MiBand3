// Decompiled with JetBrains decompiler
// Type: MiBandApp.Data.Vector2
// Assembly: MiBandApp.Data, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6F72CE9F-BCE5-4931-8D08-920048DF42FA
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.Data.dll

using System;

#nullable disable
namespace MiBandApp.Data
{
  public struct Vector2
  {
    public double X { get; set; }

    public double Y { get; set; }

    public double Distance(Vector2 start)
    {
      return Math.Sqrt(Math.Pow(this.X - start.X, 2.0) + Math.Pow(this.Y - start.Y, 2.0));
    }

    public bool Equals(Vector2 other) => this.X.Equals(other.X) && this.Y.Equals(other.Y);

    public override bool Equals(object obj)
    {
      return obj != null && obj is Vector2 other && this.Equals(other);
    }

    public override int GetHashCode()
    {
      double num1 = this.X;
      int num2 = num1.GetHashCode() * 397;
      num1 = this.Y;
      int hashCode = num1.GetHashCode();
      return num2 ^ hashCode;
    }

    public static bool operator ==(Vector2 left, Vector2 right) => left.Equals(right);

    public static bool operator !=(Vector2 left, Vector2 right) => !left.Equals(right);
  }
}
