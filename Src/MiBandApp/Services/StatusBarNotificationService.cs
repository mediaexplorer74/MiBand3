// Decompiled with JetBrains decompiler
// Type: MiBandApp.Services.StatusBarNotificationService
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.exe

using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Threading;
using Windows.UI.ViewManagement;

#nullable disable
namespace MiBandApp.Services
{
  public class StatusBarNotificationService
  {
    private readonly Queue<StatusBarMessage> _messageQueue = new Queue<StatusBarMessage>();
    private readonly object _progressItemLock = new object();
    private StatusBarItem _displayedItem;
    private Timer _messageTimer;
    private StatusBarProgressItem _currentProgressItem;

    public T Show<T>(T statusBarItem) where T : StatusBarItem
    {
      if (statusBarItem is StatusBarMessage statusBarMessage)
      {
        lock (this._messageQueue)
        {
          this._messageQueue.Enqueue(statusBarMessage);
          this.PopMessage();
        }
      }
      if (statusBarItem is StatusBarProgressItem statusBarProgressItem)
      {
        lock (this._messageQueue)
        {
          this._currentProgressItem = statusBarProgressItem;
          this._currentProgressItem.OnHide += new EventHandler(this.ProgressItemOnOnHide);
          this._currentProgressItem.ProgressUpdated += new EventHandler(this.ProgressItemOnProgressUpdated);
          this.PopMessage();
        }
      }
      return statusBarItem;
    }

    private void ProgressItemOnProgressUpdated(object sender, EventArgs eventArgs)
    {
      if (sender != this._currentProgressItem || this._displayedItem != this._currentProgressItem)
        return;
      ((System.Action) (async () =>
      {
        StatusBarProgressIndicator progressIndicator = StatusBar.GetForCurrentView().ProgressIndicator;
        progressIndicator.put_ProgressValue(this._currentProgressItem.Progress);
        await progressIndicator.ShowAsync();
      })).OnUIThread();
    }

    private void ProgressItemOnOnHide(object sender, EventArgs eventArgs)
    {
      lock (this._messageQueue)
      {
        if (sender == this._currentProgressItem)
        {
          this._currentProgressItem = (StatusBarProgressItem) null;
          if (sender == this._displayedItem)
            this.HideStatusBar();
        }
      }
      StatusBarProgressItem statusBarProgressItem = (StatusBarProgressItem) sender;
      statusBarProgressItem.OnHide -= new EventHandler(this.ProgressItemOnOnHide);
      statusBarProgressItem.ProgressUpdated -= new EventHandler(this.ProgressItemOnProgressUpdated);
    }

    private void PopMessage()
    {
      lock (this._messageQueue)
      {
        if (this._messageQueue.Count <= 0)
        {
          this.HideMessage();
        }
        else
        {
          StatusBarMessage statusBarMessage = this._messageQueue.Peek();
          if (statusBarMessage == this._displayedItem)
            return;
          this.DisplayMessage(statusBarMessage);
          this._messageTimer = new Timer((TimerCallback) (state =>
          {
            this._messageQueue.Dequeue();
            this.PopMessage();
          }), (object) null, statusBarMessage.DisplayingTimeSpan, TimeSpan.FromMilliseconds(-1.0));
        }
      }
    }

    private void DisplayMessage(StatusBarMessage item)
    {
      this._displayedItem = (StatusBarItem) item;
      ((System.Action) (async () =>
      {
        StatusBarProgressIndicator progressIndicator = StatusBar.GetForCurrentView().ProgressIndicator;
        progressIndicator.put_ProgressValue(new double?(0.0));
        progressIndicator.put_Text(item.Text);
        await progressIndicator.ShowAsync();
      })).OnUIThread();
    }

    private async void HideMessage()
    {
      if (this._currentProgressItem != null)
        this.ShowCurrentProgressItem();
      else
        this.HideStatusBar();
    }

    private void HideStatusBar()
    {
      lock (this._progressItemLock)
      {
        this._displayedItem = (StatusBarItem) null;
        ((System.Action) (async () => await StatusBar.GetForCurrentView().ProgressIndicator.HideAsync())).OnUIThread();
      }
    }

    private async void ShowCurrentProgressItem()
    {
      await ((System.Action) (async () =>
      {
        StatusBarProgressItem currentProgressItem;
        lock (this._messageQueue)
        {
          if (this._currentProgressItem == null || this._currentProgressItem.IsHidden)
            return;
          currentProgressItem = this._currentProgressItem;
          this._displayedItem = (StatusBarItem) this._currentProgressItem;
        }
        StatusBarProgressIndicator progressIndicator = StatusBar.GetForCurrentView().ProgressIndicator;
        progressIndicator.put_ProgressValue(currentProgressItem.Progress);
        progressIndicator.put_Text(currentProgressItem.Text);
        await progressIndicator.ShowAsync();
      })).OnUIThreadAsync().ConfigureAwait(true);
    }
  }
}
