// Decompiled with JetBrains decompiler
// Type: Microsoft.Live.Operations.Operation
// Assembly: Microsoft.Live, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 84D9FE74-951C-4BB4-80F8-EDD369EAC36D
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\Microsoft.Live.dll

#nullable disable
namespace Microsoft.Live.Operations
{
  internal abstract class Operation
  {
    protected Operation(SynchronizationContextWrapper syncContext)
    {
      this.Status = OperationStatus.NotStarted;
      this.Dispatcher = syncContext ?? SynchronizationContextWrapper.Current;
    }

    public SynchronizationContextWrapper Dispatcher { get; private set; }

    public OperationStatus Status { get; internal set; }

    public bool IsCancelled => this.Status == OperationStatus.Cancelled;

    public void Execute()
    {
      if (this.IsCancelled)
        return;
      this.Status = OperationStatus.Started;
      this.InternalExecute();
    }

    public virtual void Cancel()
    {
      if (this.Status == OperationStatus.Cancelled || this.Status == OperationStatus.Completed)
        return;
      this.Status = OperationStatus.Cancelled;
    }

    protected abstract void OnExecute();

    protected abstract void OnCancel();

    protected void InternalExecute()
    {
      if (this.IsCancelled)
        this.OnCancel();
      else
        this.OnExecute();
    }
  }
}
