// Decompiled with JetBrains decompiler
// Type: MetroLog.LoggingConfiguration
// Assembly: MetroLog, Version=0.0.0.0, Culture=neutral, PublicKeyToken=ba4ace74c3b410f3
// MVID: 4C1C3979-5A8D-451B-A09E-C87C946DE5D1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MetroLog.dll

using MetroLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace MetroLog
{
  public class LoggingConfiguration
  {
    private readonly List<TargetBinding> bindings;
    private readonly object bindingsLock = new object();
    private bool frozen;

    public bool IsEnabled { get; set; }

    public LoggingConfiguration()
    {
      this.IsEnabled = true;
      this.bindings = new List<TargetBinding>();
    }

    public void AddTarget(LogLevel level, Target target) => this.AddTarget(level, level, target);

    public void AddTarget(LogLevel min, LogLevel max, Target target)
    {
      if (this.frozen)
        throw new InvalidOperationException("Cannot modify config after initialization");
      lock (this.bindingsLock)
        this.bindings.Add(new TargetBinding(min, max, target));
    }

    internal IEnumerable<Target> GetTargets()
    {
      lock (this.bindingsLock)
      {
        List<Target> targets = new List<Target>();
        foreach (TargetBinding binding in this.bindings)
          targets.Add(binding.Target);
        return (IEnumerable<Target>) targets;
      }
    }

    internal IEnumerable<Target> GetTargets(LogLevel level)
    {
      lock (this.bindingsLock)
        return (IEnumerable<Target>) this.bindings.Where<TargetBinding>((Func<TargetBinding, bool>) (v => v.SupportsLevel(level))).Select<TargetBinding, Target>((Func<TargetBinding, Target>) (binding => binding.Target)).ToList<Target>();
    }

    internal void Freeze() => this.frozen = true;
  }
}
