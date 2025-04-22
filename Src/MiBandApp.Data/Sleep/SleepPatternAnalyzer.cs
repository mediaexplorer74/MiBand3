// Decompiled with JetBrains decompiler
// Type: MiBandApp.Data.Sleep.SleepPatternAnalyzer
// Assembly: MiBandApp.Data, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6F72CE9F-BCE5-4931-8D08-920048DF42FA
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.Data.dll

using MiBandApp.Data.Activities;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace MiBandApp.Data.Sleep
{
  public class SleepPatternAnalyzer
  {
    private const int DeepSleepActivityThreshold = 3;
    private readonly List<ActivityMinuteData> _sleepDataList;
    private readonly SortedDictionary<int, SleepPhase> _phases = new SortedDictionary<int, SleepPhase>();
    private List<int> _cycleEndIndexes;
    private int _previousSleepEffectiveCycles;

    public SleepPatternAnalyzer(
      List<ActivityMinuteData> dataList,
      IEnumerable<SleepingActivity> prevHourSleepingActivities)
    {
      this._sleepDataList = Enumerable.ToList<ActivityMinuteData>((IEnumerable<ActivityMinuteData>) dataList);
      this.CalculatePreviousDayEffectiveCycles(prevHourSleepingActivities);
    }

    public SleepPattern GetSleepPattern()
    {
      this._phases.Clear();
      this.MarkAwakenings();
      this.MarkDeepSleepPhases();
      this.MarkLightSleepPhases();
      this.FindCyclesAndRemPhases();
      this.FilterDeepSleepPhases();
      this.FilterShortLightSleepPhases();
      this.FilterShortDeepSleepPhases();
      SleepPattern sleepPattern = new SleepPattern((IReadOnlyList<SleepPhase>) Enumerable.ToList<SleepPhase>((IEnumerable<SleepPhase>) this._phases.Values));
      Enumerable.Aggregate<SleepPhase>((IEnumerable<SleepPhase>) sleepPattern.Phases, (Func<SleepPhase, SleepPhase, SleepPhase>) ((a, b) => b));
      return sleepPattern;
    }

    private void CalculatePreviousDayEffectiveCycles(
      IEnumerable<SleepingActivity> prevHourSleepingActivities)
    {
      List<SleepingActivity> list = Enumerable.ToList<SleepingActivity>(prevHourSleepingActivities);
      if (list.Count == 0)
        return;
      DateTime currentSleepStartTime = this._sleepDataList[0].Timestamp;
      double num = Enumerable.Max<SleepingActivity>((IEnumerable<SleepingActivity>) list, (Func<SleepingActivity, double>) (t => (double) t.TotalSleepMinutes - (currentSleepStartTime - t.End).TotalMinutes));
      if (num < 0.0)
        return;
      this._previousSleepEffectiveCycles = (int) Math.Round(num / 90.0);
    }

    private void FilterShortDeepSleepPhases()
    {
      List<SleepPhase> list = Enumerable.ToList<SleepPhase>(Enumerable.Where<SleepPhase>((IEnumerable<SleepPhase>) this._phases.Values, (Func<SleepPhase, bool>) (t => t.Length <= 10 && t.Type == SleepPhaseType.Deep)));
      using (List<SleepPhase>.Enumerator enumerator = list.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          SleepPhase current = enumerator.Current with
          {
            Type = SleepPhaseType.Light
          };
          this._phases[current.BeginTime] = current;
        }
      }
      if (list.Count <= 0)
        return;
      this.MergePhases();
    }

    private void FilterShortLightSleepPhases()
    {
      List<SleepPhase> list = Enumerable.ToList<SleepPhase>(Enumerable.Where<SleepPhase>((IEnumerable<SleepPhase>) this._phases.Values, (Func<SleepPhase, bool>) (t => t.Length <= 2 && t.Type == SleepPhaseType.Light)));
      using (List<SleepPhase>.Enumerator enumerator = list.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          SleepPhase current = enumerator.Current with
          {
            Type = SleepPhaseType.Deep
          };
          this._phases[current.BeginTime] = current;
        }
      }
      if (list.Count <= 0)
        return;
      this.MergePhases();
    }

    private void MergePhases()
    {
      List<SleepPhase> list = Enumerable.ToList<SleepPhase>((IEnumerable<SleepPhase>) this._phases.Values);
      List<int> intList1 = new List<int>();
      for (int index = 0; index < this._phases.Count - 1; ++index)
      {
        SleepPhase sleepPhase1 = list[index];
        while (index + 1 < this._phases.Count)
        {
          SleepPhase sleepPhase2 = list[index + 1];
          if (sleepPhase2.Type == sleepPhase1.Type)
          {
            ++index;
            ref SleepPhase local = ref sleepPhase1;
            int length1 = local.Length;
            sleepPhase2 = list[index];
            int length2 = sleepPhase2.Length;
            local.Length = length1 + length2;
            List<int> intList2 = intList1;
            sleepPhase2 = list[index];
            int beginTime = sleepPhase2.BeginTime;
            intList2.Add(beginTime);
          }
          else
            break;
        }
        this._phases[sleepPhase1.BeginTime] = sleepPhase1;
      }
      using (List<int>.Enumerator enumerator = intList1.GetEnumerator())
      {
        while (enumerator.MoveNext())
          this._phases.Remove(enumerator.Current);
      }
    }

    private void FilterDeepSleepPhases()
    {
      this._phases[0] = this._phases[0] with
      {
        Type = SleepPhaseType.Light
      };
      for (int cycleIndex = this._previousSleepEffectiveCycles; cycleIndex < this._cycleEndIndexes.Count; cycleIndex++)
      {
        int expectedDeepSleepInCycle = SleepPatternAnalyzer.GetExpectedDeepSleepInCycle(cycleIndex);
        int cycleStartTime = 0;
        if (cycleIndex > 0)
          cycleStartTime = this._phases[this._cycleEndIndexes[cycleIndex - 1]].EndTime + 1;
        foreach (SleepPhase sleepPhase1 in Enumerable.Select<KeyValuePair<int, SleepPhase>, SleepPhase>((IEnumerable<KeyValuePair<int, SleepPhase>>) Enumerable.ToList<KeyValuePair<int, SleepPhase>>((IEnumerable<KeyValuePair<int, SleepPhase>>) Enumerable.OrderBy<KeyValuePair<int, SleepPhase>, int>(Enumerable.Where<KeyValuePair<int, SleepPhase>>((IEnumerable<KeyValuePair<int, SleepPhase>>) this._phases, (Func<KeyValuePair<int, SleepPhase>, bool>) (t => t.Value.Type == SleepPhaseType.Deep && t.Value.BeginTime < this._cycleEndIndexes[cycleIndex] && t.Value.EndTime > cycleStartTime)), (Func<KeyValuePair<int, SleepPhase>, int>) (t => Math.Abs(expectedDeepSleepInCycle - t.Value.Length)))), (Func<KeyValuePair<int, SleepPhase>, SleepPhase>) (t => t.Value)))
        {
          if (1.2 * (double) expectedDeepSleepInCycle - (double) sleepPhase1.Length >= 0.0)
          {
            expectedDeepSleepInCycle -= sleepPhase1.Length;
          }
          else
          {
            SleepPhase sleepPhase2 = sleepPhase1 with
            {
              Type = SleepPhaseType.Light
            };
            this._phases[sleepPhase2.BeginTime] = sleepPhase2;
          }
        }
      }
      this.MergePhases();
    }

    private void MarkAwakenings()
    {
      foreach (SleepPhase awakening in this.FindAwakenings())
        this._phases.Add(awakening.BeginTime, awakening);
    }

    private void MarkDeepSleepPhases()
    {
      foreach (SleepPhase deepSleepPhase in this.GetDeepSleepPhases())
        this._phases.Add(deepSleepPhase.BeginTime, deepSleepPhase);
    }

    private void MarkLightSleepPhases()
    {
      foreach (SleepPhase sleepPhase in Enumerable.Select<List<ActivityMinuteData>, SleepPhase>(this.GetFragmentsWithoutPhases(Enumerable.ToList<SleepPhase>((IEnumerable<SleepPhase>) this._phases.Values)), (Func<List<ActivityMinuteData>, SleepPhase>) (activityMinuteData => new SleepPhase()
      {
        Type = SleepPhaseType.Light,
        Length = activityMinuteData.Count,
        BeginTime = this._sleepDataList.IndexOf(activityMinuteData[0])
      })))
        this._phases.Add(sleepPhase.BeginTime, sleepPhase);
    }

    private void FindCyclesAndRemPhases()
    {
      this._cycleEndIndexes = this.GetSleepCyclesRemIndex(this._previousSleepEffectiveCycles, 0).Item;
      using (List<int>.Enumerator enumerator = this._cycleEndIndexes.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          int current = enumerator.Current;
          SleepPhase phase = this._phases[current];
          if (phase.Type == SleepPhaseType.Deep)
          {
            phase.Type = SleepPhaseType.REM;
            this._phases[current] = phase;
          }
        }
      }
    }

    private IEnumerable<SleepPhase> FindAwakenings()
    {
      int minutesAwake = 3;
      int activityAwake = 30;
      SleepPhase awakening = new SleepPhase();
      for (int i = 0; i < this._sleepDataList.Count - minutesAwake; ++i)
      {
        if (this._sleepDataList[i].Activity > activityAwake && this._sleepDataList.GetAverageActivity(i, minutesAwake) > (double) activityAwake)
        {
          if (awakening.Equals((object) new SleepPhase()))
          {
            awakening = new SleepPhase();
            awakening.BeginTime = i;
            awakening.Length = 0;
            awakening.Type = SleepPhaseType.Awake;
          }
          ++awakening.Length;
        }
        else if (!awakening.Equals((object) new SleepPhase()))
        {
          yield return awakening;
          awakening = new SleepPhase();
        }
      }
    }

    private IEnumerable<SleepPhase> GetDeepSleepPhases()
    {
      int i = 0;
      while (i < this._sleepDataList.Count - 5)
      {
        if (Enumerable.All<ActivityMinuteData>((IEnumerable<ActivityMinuteData>) this._sleepDataList.GetRange(i, 5), (Func<ActivityMinuteData, bool>) (t => t.Activity <= 3)))
        {
          SleepPhase deepSleepPhase = new SleepPhase();
          deepSleepPhase.Type = SleepPhaseType.Deep;
          deepSleepPhase.BeginTime = i;
          deepSleepPhase.Length = 5;
          ++i;
          while (i + 5 < this._sleepDataList.Count && Enumerable.All<ActivityMinuteData>((IEnumerable<ActivityMinuteData>) this._sleepDataList.GetRange(i, 5), (Func<ActivityMinuteData, bool>) (t => t.Activity <= 3)))
          {
            ++i;
            ++deepSleepPhase.Length;
          }
          yield return deepSleepPhase;
          i += 5;
        }
        else
          ++i;
      }
    }

    private IEnumerable<List<ActivityMinuteData>> GetFragmentsWithoutPhases(List<SleepPhase> phases)
    {
      List<ActivityMinuteData> data = this._sleepDataList;
      int fragmentStartIndex = 0;
      int phaseIndex = 0;
      if (phases.Count == 0)
      {
        yield return data;
      }
      else
      {
        for (phases = Enumerable.ToList<SleepPhase>((IEnumerable<SleepPhase>) Enumerable.OrderBy<SleepPhase, int>((IEnumerable<SleepPhase>) phases, (Func<SleepPhase, int>) (t => t.BeginTime))); phaseIndex <= phases.Count; ++phaseIndex)
        {
          if (phaseIndex >= phases.Count)
          {
            int count = data.Count - fragmentStartIndex;
            if (count == 0)
              break;
            yield return data.GetRange(fragmentStartIndex, count);
            break;
          }
          SleepPhase nextPhase = phases[phaseIndex];
          int count1 = nextPhase.BeginTime - fragmentStartIndex;
          if (count1 != 0)
            yield return data.GetRange(fragmentStartIndex, count1);
          fragmentStartIndex = nextPhase.BeginTime + nextPhase.Length;
        }
      }
    }

    private SleepPatternAnalyzer.WithProbability<List<int>> GetSleepCyclesRemIndex(
      int cycleNumber,
      int cycleStartTime)
    {
      int expectedCycleLen = this.GetExpectedCycleLen(cycleNumber);
      int expectedRemLen = this.GetExpectedRemLen(cycleNumber);
      List<KeyValuePair<int, SleepPhase>> list = Enumerable.ToList<KeyValuePair<int, SleepPhase>>(Enumerable.SkipWhile<KeyValuePair<int, SleepPhase>>((IEnumerable<KeyValuePair<int, SleepPhase>>) this._phases, (Func<KeyValuePair<int, SleepPhase>, bool>) (t => t.Key < cycleStartTime)));
      if (list.Count == 0)
        return new SleepPatternAnalyzer.WithProbability<List<int>>(new List<int>())
        {
          Probability = 1.0
        };
      List<SleepPatternAnalyzer.WithProbability<int>> withProbabilityList = new List<SleepPatternAnalyzer.WithProbability<int>>();
      using (List<KeyValuePair<int, SleepPhase>>.Enumerator enumerator = list.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          KeyValuePair<int, SleepPhase> current = enumerator.Current;
          SleepPhase sleepPhase = current.Value;
          int num1 = sleepPhase.EndTime - cycleStartTime;
          if ((double) num1 <= 1.25 * (double) expectedCycleLen)
          {
            sleepPhase = current.Value;
            if (sleepPhase.Type == SleepPhaseType.Awake)
            {
              withProbabilityList.Add(new SleepPatternAnalyzer.WithProbability<int>(current.Key)
              {
                Probability = 1.0
              });
              break;
            }
            sleepPhase = current.Value;
            if (sleepPhase.Type == SleepPhaseType.Deep && (double) num1 >= 0.75 * (double) expectedCycleLen)
            {
              double num2 = this.GetProbability((double) Math.Abs(num1 - expectedCycleLen), 0.5 / (0.75 * (double) expectedCycleLen));
              int num3 = expectedRemLen;
              sleepPhase = current.Value;
              int length = sleepPhase.Length;
              double probability = this.GetProbability((double) Math.Abs(num3 - length), 0.01);
              if (cycleNumber == 0)
                num2 = 1.0;
              double num4 = num2;
              double num5 = probability * num4;
              withProbabilityList.Add(new SleepPatternAnalyzer.WithProbability<int>(current.Key)
              {
                Probability = num5
              });
            }
          }
          else
            break;
        }
      }
      if (withProbabilityList.Count == 0)
      {
        KeyValuePair<int, SleepPhase> keyValuePair = Enumerable.First<KeyValuePair<int, SleepPhase>>((IEnumerable<KeyValuePair<int, SleepPhase>>) Enumerable.OrderBy<KeyValuePair<int, SleepPhase>, int>((IEnumerable<KeyValuePair<int, SleepPhase>>) list, (Func<KeyValuePair<int, SleepPhase>, int>) (t => Math.Abs(t.Value.EndTime - cycleStartTime - expectedCycleLen))));
        withProbabilityList.Add(new SleepPatternAnalyzer.WithProbability<int>(keyValuePair.Key)
        {
          Probability = 1.0
        });
      }
      return Enumerable.First<SleepPatternAnalyzer.WithProbability<List<int>>>((IEnumerable<SleepPatternAnalyzer.WithProbability<List<int>>>) Enumerable.OrderByDescending<SleepPatternAnalyzer.WithProbability<List<int>>, double>((IEnumerable<SleepPatternAnalyzer.WithProbability<List<int>>>) Enumerable.ToList<SleepPatternAnalyzer.WithProbability<List<int>>>(Enumerable.Select(Enumerable.Select((IEnumerable<SleepPatternAnalyzer.WithProbability<int>>) withProbabilityList, t => new
      {
        Candidate = t,
        NextCycles = this.GetSleepCyclesRemIndex(cycleNumber + 1, this._phases[t.Item].EndTime + 1)
      }), t =>
      {
        List<int> intList = new List<int>();
        intList.Add(t.Candidate.Item);
        return new SleepPatternAnalyzer.WithProbability<List<int>>(Enumerable.ToList<int>(Enumerable.Concat<int>((IEnumerable<int>) intList, (IEnumerable<int>) t.NextCycles.Item)))
        {
          Probability = t.NextCycles.Probability * t.Candidate.Probability
        };
      })), (Func<SleepPatternAnalyzer.WithProbability<List<int>>, double>) (t => t.Probability)));
    }

    private double GetProbability(double delta, double k) => 1.0 / (k * delta + 1.0);

    private int GetExpectedRemLen(int cycleNumber)
    {
      switch (cycleNumber)
      {
        case 0:
          return 5;
        case 1:
          return 10;
        case 2:
          return 20;
        default:
          return 30;
      }
    }

    private int GetExpectedCycleLen(int cycleNumber)
    {
      return cycleNumber == 0 || cycleNumber == 1 ? 110 : 105;
    }

    private static int GetExpectedDeepSleepInCycle(int cycleIndex)
    {
      if (cycleIndex == 0 || cycleIndex == 1)
        return 70;
      return cycleIndex == 2 ? 60 : Math.Max(50 - cycleIndex * 10, 0);
    }

    public class WithProbability<T>
    {
      public WithProbability(T item) => this.Item = item;

      public T Item { get; set; }

      public double Probability { get; set; }
    }
  }
}
