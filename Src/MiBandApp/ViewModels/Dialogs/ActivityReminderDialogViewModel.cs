
// Type: MiBandApp.ViewModels.Dialogs.ActivityReminderDialogViewModel
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// 

using Caliburn.Micro;
using System;

#nullable disable
namespace MiBandApp.ViewModels.Dialogs
{
  public class ActivityReminderDialogViewModel : PropertyChangedBase
  {
    private TimeSpan _savedStartTime;
    private TimeSpan _savedEndTime;
    private TimeSpan _startTime;
    private TimeSpan _endTime;
    private bool _isVisible;

    public event EventHandler Saved;

    public bool IsSaveButtonEnabled => this.EndTime > this.StartTime;

    public bool IsVisible
    {
      get => this._isVisible;
      set
      {
        if (value == this._isVisible)
          return;
        this._isVisible = value;
        this.NotifyOfPropertyChange(nameof (IsVisible));
      }
    }

    public TimeSpan StartTime
    {
      get => this._startTime;
      set
      {
        if (value.Equals(this._startTime))
          return;
        this._startTime = value;
        this.NotifyOfPropertyChange(nameof (StartTime));
        this.NotifyOfPropertyChange("IsSaveButtonEnabled");
      }
    }

    public TimeSpan EndTime
    {
      get => this._endTime;
      set
      {
        if (value.Equals(this._endTime))
          return;
        this._endTime = value;
        this.NotifyOfPropertyChange(nameof (EndTime));
        this.NotifyOfPropertyChange("IsSaveButtonEnabled");
      }
    }

    public TimeSpan SavedStartTime => this._savedStartTime;

    public TimeSpan SavedEndTime => this._savedEndTime;

    public void Initialize(TimeSpan startTime, TimeSpan endTime)
    {
      this._savedStartTime = startTime;
      this._savedEndTime = endTime;
      this.StartTime = this._savedStartTime;
      this.EndTime = this._savedEndTime;
    }

    public void OnSaveButtonTapped()
    {
      this._savedStartTime = this.StartTime;
      this._savedEndTime = this.EndTime;
      this.IsVisible = false;
      EventHandler saved = this.Saved;
      if (saved == null)
        return;
      saved((object) this, EventArgs.Empty);
    }
  }
}
