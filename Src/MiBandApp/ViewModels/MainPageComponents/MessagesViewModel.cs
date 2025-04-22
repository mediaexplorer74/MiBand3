// Decompiled with JetBrains decompiler
// Type: MiBandApp.ViewModels.MainPageComponents.MessagesViewModel
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.exe

using Caliburn.Micro;
using MiBand.SDK.Core;
using MiBandApp.Services;
using MiBandApp.Tools;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace MiBandApp.ViewModels.MainPageComponents
{
  public class MessagesViewModel : PropertyChangedBase
  {
    private readonly BandController _bandController;
    private readonly INavigationService _navigationService;
    private readonly HashSet<MessagesViewModel.Message> _messages = new HashSet<MessagesViewModel.Message>();

    public MessagesViewModel(BandController bandController, INavigationService navigationService)
    {
      this._bandController = bandController;
      this._navigationService = navigationService;
      this._bandController.StatusChanged += new EventHandler(this.BandControllerOnStatusChanged);
      this._bandController.BindingState.Updated += new EventHandler<MonitorableUpdatedEventArgs<BindingState>>(this.BindingStateOnUpdated);
    }

    public bool ShowUnreachableError
    {
      get => this.TopPriorityMessage == MessagesViewModel.Message.Unreachable;
    }

    public bool ShowUnbindedScreen
    {
      get => this.TopPriorityMessage == MessagesViewModel.Message.NotBinded;
    }

    public bool ShowUnpairedToPhone
    {
      get => this.TopPriorityMessage == MessagesViewModel.Message.NotPaired;
    }

    public bool ShowMoreThanOnePaired
    {
      get => this.TopPriorityMessage == MessagesViewModel.Message.MoreThanOnePaired;
    }

    public bool IsMessageDisplayed
    {
      get
      {
        lock (this._messages)
          return this._messages.Count != 0;
      }
    }

    public string GoToBluetoothSettingsLink
    {
      get => !WindowsVersion.IsWindows10 ? "ms-settings-bluetooth:" : "ms-settings:bluetooth";
    }

    public void Bind()
    {
      ((System.Action) (() => this._navigationService.UriFor<PairingPageViewModel>().Navigate())).OnUIThread();
    }

    private MessagesViewModel.Message TopPriorityMessage
    {
      get
      {
        lock (this._messages)
          return this._messages.Count == 0 ? MessagesViewModel.Message.None : this._messages.OrderByDescending<MessagesViewModel.Message, int>((Func<MessagesViewModel.Message, int>) (t => (int) t)).FirstOrDefault<MessagesViewModel.Message>();
      }
    }

    private void BindingStateOnUpdated(object sender, MonitorableUpdatedEventArgs<BindingState> e)
    {
      lock (this._messages)
      {
        if (e.UpdatedValue == BindingState.Unbinded || e.UpdatedValue == BindingState.New)
          this._messages.Add(MessagesViewModel.Message.NotBinded);
        else
          this._messages.Remove(MessagesViewModel.Message.NotBinded);
      }
      this.Refresh();
    }

    private void BandControllerOnStatusChanged(object sender, EventArgs e)
    {
      lock (this._messages)
      {
        MiBandStatus status = this._bandController.Status;
        if (status == MiBandStatus.NotPairedToPhone)
          this._messages.Add(MessagesViewModel.Message.NotPaired);
        else
          this._messages.Remove(MessagesViewModel.Message.NotPaired);
        if (status == MiBandStatus.Unreachable || status == MiBandStatus.Error)
          this._messages.Add(MessagesViewModel.Message.Unreachable);
        else
          this._messages.Remove(MessagesViewModel.Message.Unreachable);
        if (status == MiBandStatus.MoreThanOnePaired)
          this._messages.Add(MessagesViewModel.Message.MoreThanOnePaired);
        else
          this._messages.Remove(MessagesViewModel.Message.MoreThanOnePaired);
      }
      this.Refresh();
    }

    private enum Message
    {
      None,
      NotBinded,
      Unreachable,
      MoreThanOnePaired,
      NotPaired,
    }
  }
}
