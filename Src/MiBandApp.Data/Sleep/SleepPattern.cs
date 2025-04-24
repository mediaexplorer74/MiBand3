
// Type: MiBandApp.Data.Sleep.SleepPattern
// Assembly: MiBandApp.Data, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6F72CE9F-BCE5-4931-8D08-920048DF42FA
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.Data.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#nullable disable
namespace MiBandApp.Data.Sleep
{
  public class SleepPattern
  {
    public SleepPattern(IReadOnlyList<SleepPhase> phases)
    {
      this.Phases = (IReadOnlyList<SleepPhase>) Enumerable.ToList<SleepPhase>((IEnumerable<SleepPhase>) phases);
    }

    public IReadOnlyList<SleepPhase> Phases { get; private set; }

    public static SleepPattern FromString(string patternString)
    {
      string[] strArray = patternString.Split(';');
      string str = strArray[0];
      List<SleepPhase> phases = new List<SleepPhase>();
      SleepPhaseType sleepPhaseType = SleepPhaseType.Deep;
      for (int index = 1; index < strArray.Length; ++index)
      {
        if (!string.IsNullOrEmpty(strArray[index]))
        {
          SleepPhase sleepPhase = new SleepPhase();
          switch (strArray[index][0])
          {
            case 'A':
              sleepPhaseType = SleepPhaseType.Awake;
              break;
            case 'D':
              sleepPhaseType = SleepPhaseType.Deep;
              break;
            case 'L':
              sleepPhaseType = SleepPhaseType.Light;
              break;
            case 'R':
              sleepPhaseType = SleepPhaseType.REM;
              break;
          }
          sleepPhase.Type = sleepPhaseType;
          sleepPhase.Length = int.Parse(strArray[index].Substring(1));
          sleepPhase.BeginTime = Enumerable.Sum<SleepPhase>((IEnumerable<SleepPhase>) phases, (Func<SleepPhase, int>) (t => t.Length));
          phases.Add(sleepPhase);
        }
      }
      return new SleepPattern((IReadOnlyList<SleepPhase>) phases);
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("ver0;");
      foreach (SleepPhase phase in (IEnumerable<SleepPhase>) this.Phases)
      {
        switch (phase.Type)
        {
          case SleepPhaseType.Deep:
            stringBuilder.Append("D");
            break;
          case SleepPhaseType.Light:
            stringBuilder.Append("L");
            break;
          case SleepPhaseType.REM:
            stringBuilder.Append("R");
            break;
          case SleepPhaseType.Awake:
            stringBuilder.Append("A");
            break;
          default:
            throw new ArgumentOutOfRangeException();
        }
        stringBuilder.Append(phase.Length);
        stringBuilder.Append(";");
      }
      return stringBuilder.ToString();
    }

    public List<SleepPhaseType> GetPhasesByMinute()
    {
      List<SleepPhaseType> phasesByMinute = new List<SleepPhaseType>();
      foreach (SleepPhase phase in (IEnumerable<SleepPhase>) this.Phases)
      {
        int type = (int) phase.Type;
        phasesByMinute.AddRange(Enumerable.Select<int, SleepPhaseType>(Enumerable.Repeat<int>(type, phase.Length), (Func<int, SleepPhaseType>) (t => (SleepPhaseType) t)));
      }
      return phasesByMinute;
    }
  }
}
