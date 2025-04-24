
// Type: MiBandApp.Data.HeartRateProcessor
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
  public class HeartRateProcessor
  {
    public static string GetHeartRateString(int[] heartRates)
    {
      return string.Join(";", Enumerable.Select<int, string>((IEnumerable<int>) heartRates, (Func<int, string>) (t => t == 0 ? string.Empty : t.ToString())));
    }

    public static int[] GetHeartRatesFromString(string heartRateString)
    {
      if (heartRateString == null)
        return new int[0];
      return Enumerable.ToArray<int>(Enumerable.Select<string, int>((IEnumerable<string>) heartRateString.Split(';'), (Func<string, int>) (t => !(t == string.Empty) ? int.Parse(t) : 0)));
    }

    public int[] NormalizeHeartRate(List<ActivityMinuteData> dataList, int start, int end)
    {
      int[] array = Enumerable.ToArray<int>(Enumerable.Select<ActivityMinuteData, int>(Enumerable.Take<ActivityMinuteData>(Enumerable.Skip<ActivityMinuteData>((IEnumerable<ActivityMinuteData>) dataList, start), end - start), (Func<ActivityMinuteData, int>) (t => t.HeartRate)));
      for (int index = 0; index < array.Length; ++index)
      {
        int num = array[index];
        if (num == 0 || num > 250)
          array[index] = 0;
      }
      for (int i = 0; i < array.Length; i++)
      {
        int hr = array[i];
        if (hr != 0 && !this.IsOkHrValue(hr))
        {
          int lastIndex = Array.FindLastIndex<int>(array, i, new Predicate<int>(this.IsOkHrValue));
          int index = Array.FindIndex<int>(array, i, new Predicate<int>(this.IsOkHrValue));
          if (lastIndex == -1 && index == -1)
          {
            array[i] = 0;
          }
          else
          {
            List<KeyValuePair<int, int>> keyValuePairList = new List<KeyValuePair<int, int>>();
            if (lastIndex != -1)
              keyValuePairList.Add(new KeyValuePair<int, int>(lastIndex, array[lastIndex]));
            if (index != -1)
              keyValuePairList.Add(new KeyValuePair<int, int>(index, array[index]));
            keyValuePairList.Sort((Comparison<KeyValuePair<int, int>>) ((a, b) => Math.Abs(a.Key - i) - Math.Abs(b.Key - i)));
            foreach (int num1 in Enumerable.Select<KeyValuePair<int, int>, int>((IEnumerable<KeyValuePair<int, int>>) keyValuePairList, (Func<KeyValuePair<int, int>, int>) (t => t.Value)))
            {
              double num2 = (double) hr / (double) num1;
              double num3 = num2 - Math.Truncate(num2);
              if (num3 > 0.8 || num3 < 0.2)
                array[i] = (int) ((double) hr / Math.Round(num2));
            }
          }
        }
      }
      for (int index = 0; index < array.Length; ++index)
      {
        if (!this.IsOkHrValue(array[index]))
          array[index] = 0;
      }
      return array;
    }

    private bool IsOkHrValue(int hr) => hr != 0 && hr < 100;
  }
}
