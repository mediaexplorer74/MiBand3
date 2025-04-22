// Decompiled with JetBrains decompiler
// Type: MiBandApp.Data.RDP
// Assembly: MiBandApp.Data, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6F72CE9F-BCE5-4931-8D08-920048DF42FA
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.Data.dll

using System;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace MiBandApp.Data
{
  public class RDP
  {
    public BitArray DouglasPeucker(
      IList<Vector2> points,
      int startIndex,
      int lastIndex,
      double epsilon)
    {
      Stack<KeyValuePair<int, int>> keyValuePairStack = new Stack<KeyValuePair<int, int>>();
      keyValuePairStack.Push(new KeyValuePair<int, int>(startIndex, lastIndex));
      int num1 = startIndex;
      BitArray bitArray = new BitArray(lastIndex - startIndex + 1, true);
      while (keyValuePairStack.Count > 0)
      {
        startIndex = keyValuePairStack.Peek().Key;
        lastIndex = keyValuePairStack.Peek().Value;
        keyValuePairStack.Pop();
        double num2 = 0.0;
        int key = startIndex;
        for (int index = key + 1; index < lastIndex; ++index)
        {
          if (bitArray[index - num1])
          {
            double num3 = this.PointLineDistance(points[index], points[startIndex], points[lastIndex]);
            if (num3 > num2)
            {
              key = index;
              num2 = num3;
            }
          }
        }
        if (num2 > epsilon)
        {
          keyValuePairStack.Push(new KeyValuePair<int, int>(startIndex, key));
          keyValuePairStack.Push(new KeyValuePair<int, int>(key, lastIndex));
        }
        else
        {
          for (int index = startIndex + 1; index < lastIndex; ++index)
            bitArray[index - num1] = false;
        }
      }
      return bitArray;
    }

    public IList<Vector2> DouglasPeucker(IList<Vector2> points, double epsilon)
    {
      BitArray bitArray = this.DouglasPeucker(points, 0, points.Count - 1, epsilon);
      List<Vector2> vector2List = new List<Vector2>();
      int index = 0;
      for (int count = points.Count; index < count; ++index)
      {
        if (bitArray[index])
          vector2List.Add(points[index]);
      }
      return (IList<Vector2>) vector2List;
    }

    public double PointLineDistance(Vector2 point, Vector2 start, Vector2 end)
    {
      return start == end ? point.Distance(start) : Math.Abs((end.X - start.X) * (start.Y - point.Y) - (start.X - point.X) * (end.Y - start.Y)) / Math.Sqrt((end.X - start.X) * (end.X - start.X) + (end.Y - start.Y) * (end.Y - start.Y));
    }
  }
}
