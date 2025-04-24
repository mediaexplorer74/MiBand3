
// Type: MetroLog.LogEventInfo
// Assembly: MetroLog, Version=0.0.0.0, Culture=neutral, PublicKeyToken=ba4ace74c3b410f3
// MVID: 4C1C3979-5A8D-451B-A09E-C87C946DE5D1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MetroLog.dll

using MetroLog.Internal;
using System;
using System.Threading;

#nullable disable
namespace MetroLog
{
  public class LogEventInfo
  {
    private ExceptionWrapper _exceptionWrapper;
    private static long globalSequenceId;

    public long SequenceID { get; set; }

    public LogLevel Level { get; set; }

    public string Logger { get; set; }

    public string Message { get; set; }

    public DateTimeOffset TimeStamp { get; set; }

    [JsonIgnore]
    public Exception Exception { get; set; }

    public LogEventInfo(LogLevel level, string logger, string message, Exception ex)
    {
      this.Level = level;
      this.Logger = logger;
      this.Message = message;
      this.Exception = ex;
      this.TimeStamp = LogManager.GetDateTime();
      this.SequenceID = LogEventInfo.GetNextSequenceId();
    }

    internal static long GetNextSequenceId()
    {
      return Interlocked.Increment(ref LogEventInfo.globalSequenceId);
    }

    public string ToJson() => SimpleJson.SerializeObject((object) this);

    public ExceptionWrapper ExceptionWrapper
    {
      get
      {
        if (this._exceptionWrapper == null && this.Exception != null)
          this._exceptionWrapper = new ExceptionWrapper(this.Exception);
        return this._exceptionWrapper;
      }
      set => this._exceptionWrapper = value;
    }
  }
}
