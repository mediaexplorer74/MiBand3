
// Type: MiBand.SDK.Core.MiBandOne.MiBandOneActivityDataLoader
// Assembly: MiBand.SDK, Version=0.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 152FA565-3249-4054-A361-01D9C0AFA7F1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBand.SDK.dll

using MiBand.SDK.Data;
using MiBand.SDK.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Foundation;

#nullable disable
namespace MiBand.SDK.Core.MiBandOne
{
  internal class MiBandOneActivityDataLoader 
    {
    private static volatile bool _dataLoading;
    private const int ReadingTimeoutSec = 5;
    private readonly MiBand.SDK.Core.MiBandOne.MiBandOne _bluetoothDevice;
    private readonly ILog _log;
    private readonly int _bytesPerMinute;
    private readonly Watchdog _watchdog = new Watchdog(TimeSpan.FromSeconds(5.0));
    private readonly TaskCompletionSource<bool> _gettingDataTaskCompletionSource;
    private MemoryStream _dataMemoryStream;
    private int _currentFragmentDataExpected;
    private GattCharacteristic _activityCharacteristic;
    private GattCharacteristic _controlPointCharacteristic;
    private bool _hasNumberLineFirstByte;
    private int _totalMinutes;

    public MiBandOneActivityDataLoader(MiBand.SDK.Core.MiBandOne.MiBandOne bluetoothDevice, ILog log)
    {
      this._bluetoothDevice = bluetoothDevice;
      this._log = log;
      this._gettingDataTaskCompletionSource = new TaskCompletionSource<bool>();
      this._bytesPerMinute = 3;
      if (!(bluetoothDevice is MiBandOneS))
        return;
      this._bytesPerMinute = 4;
    }

    public async Task<SynchronizationDataPackage> LoadData()
    {
      MiBandOneActivityDataLoader._dataLoading = !MiBandOneActivityDataLoader._dataLoading 
                ? true 
                : throw new InvalidOperationException(
                    "Cannot load data when previous operation not finished");

      object obj = (object) null;
      int num = 0;
      SynchronizationDataPackage synchronizationDataPackage = new SynchronizationDataPackage();//default;
      try
      {
        if (!await this.InitReceiver().ConfigureAwait(false))
        {
          this._log.Error("Receiver not initialized");
          synchronizationDataPackage = null;
        }
        else if (!await this.StartReceivingData().ConfigureAwait(false))
        {
          this._log.Error("Receiving data not started");
          synchronizationDataPackage = (SynchronizationDataPackage) null;
        }
        else
        {
          this._watchdog.Enable();
          Task task = await Task.WhenAny(this._watchdog.Task, (Task) this._gettingDataTaskCompletionSource.Task).ConfigureAwait(false);
          this._watchdog.Disable();
          if (this._watchdog.HasElapsed || task != this._gettingDataTaskCompletionSource.Task)
          {
            this._log.Warning("Payload loading timeout exceeded. Was expected of fragment size: " + (object) this._currentFragmentDataExpected);
            synchronizationDataPackage = (SynchronizationDataPackage) null;
          }
          else if (!this._gettingDataTaskCompletionSource.Task.Result || this._dataMemoryStream == null)
          {
            this._log.Error("Getting activity data failed");
            synchronizationDataPackage = (SynchronizationDataPackage) null;
          }
          else
            synchronizationDataPackage = this.CompleteResultPackage();
        }
        num = 1;
      }
      catch (Exception ex)
      {
        obj = ex;
      }
      MiBandOneActivityDataLoader._dataLoading = false;
      await this.DisposeReceiver().ConfigureAwait(false);
      object obj1 = obj;
      if (obj1 != null)
      {
        if (!(obj1 is Exception source))
          throw new InvalidOperationException("An unexpected object type was encountered. " 
              + obj1.ToString());
        ExceptionDispatchInfo.Capture(source).Throw();
      }
      if (num == 1)
        return synchronizationDataPackage;
      obj = (object) null;
      synchronizationDataPackage = (SynchronizationDataPackage) null;
      SynchronizationDataPackage synchronizationDataPackage1 = synchronizationDataPackage;
      return synchronizationDataPackage1;
    }

        private async Task<bool> InitReceiver()
        {
            try
            {
                this._activityCharacteristic = this._bluetoothDevice.GetCharacteristic(CharacteristicGuid.Activity);
                if (this._activityCharacteristic == null)
                    return false;

                // Subscribe to the _activityCharacteristic ValueChanged event
                this._activityCharacteristic.ValueChanged += ActivityCharOnValueChanged;

                GattCommunicationStatus communicationStatus = await this._activityCharacteristic
                    .WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);

                BandDeviceInfo bandDeviceInfo = await this._bluetoothDevice.GetBandDeviceInfo(true);
                this._hasNumberLineFirstByte = bandDeviceInfo.ProfileVersion >= new Version("2.0.7");
                if (this._hasNumberLineFirstByte)
                    this._log.Info(string.Format("Lines have first number as a number line. Profile version: {0}", bandDeviceInfo.ProfileVersion));
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

    private async Task<bool> StartReceivingData()
    {
      this._dataMemoryStream = (MemoryStream) null;
      try
      {
        this._controlPointCharacteristic = this._bluetoothDevice.GetCharacteristic(CharacteristicGuid.ControlPoint);
        GattCommunicationStatus communicationStatus = await this._controlPointCharacteristic.WriteValueAsync(new byte[1]
        {
          (byte) 6
        }.AsBuffer());
      }
      catch (Exception ex)
      {
        return false;
      }
      return true;
    }

    private void ActivityCharOnValueChanged(
      GattCharacteristic sender,
      GattValueChangedEventArgs args)
    {
      this._watchdog.Reset();
      byte[] arraySafe = args.CharacteristicValue.ToArraySafe();
      if (arraySafe == null)
        return;
      this.ProcessDataPiece(arraySafe);
    }

    private void ProcessDataPiece(byte[] data)
    {
      try
      {
        if (this._dataMemoryStream == null)
        {
          if (data.Length != 11)
          {
            this._log.Warning("First packet has wrong size", this.GetType().Name);
            this._gettingDataTaskCompletionSource.TrySetResult(false);
            return;
          }
          this._totalMinutes = (int) data[7] + ((int) data[8] << 8);
          this._dataMemoryStream = new MemoryStream(1000 + this._totalMinutes * this._bytesPerMinute);
        }
        if (this._currentFragmentDataExpected == 0 && data.Length == 11)
        {
          this._dataMemoryStream.Write(data, 0, data.Length);
          this._currentFragmentDataExpected = ((int) data[9] + ((int) data[10] << 8)) * this._bytesPerMinute;
          if (this._currentFragmentDataExpected != 0)
            return;
          this._gettingDataTaskCompletionSource.TrySetResult(true);
        }
        else
        {
          int offset = 0;
          int count = data.Length;
          if (this._hasNumberLineFirstByte)
          {
            offset = 1;
            count = data.Length - 1;
          }
          this._dataMemoryStream.Write(data, offset, count);
          this._currentFragmentDataExpected -= count;
        }
      }
      catch (Exception ex)
      {
        this._log.Error("Exception happened while handling data: " + ex.Message);
        this._gettingDataTaskCompletionSource?.TrySetResult(false);
      }
    }

        private async Task DisposeReceiver()
        {
            try
            {
                GattCommunicationStatus communicationStatus = await this._activityCharacteristic
                    .WriteClientCharacteristicConfigurationDescriptorAsync(
                    GattClientCharacteristicConfigurationDescriptorValue.None);
            }
            catch (Exception ex)
            {
                // Handle exception if needed
            }
            finally
            {
                // Unsubscribe from the ValueChanged event using the -= operator
                this._activityCharacteristic.ValueChanged -= ActivityCharOnValueChanged;
            }
        }

    private SynchronizationDataPackage CompleteResultPackage()
    {
      try
      {
        List<MiBandOneActivityDataLoader.DataSyncFragment> list = this.GetFragmentsFromBytes().ToList<MiBandOneActivityDataLoader.DataSyncFragment>();
       SynchronizationDataPackage synchronizationDataPackage = new SynchronizationDataPackage();

        IEnumerable<RawMinuteActivityDataSeries> r = this.ProcessAllFragments(list);
        synchronizationDataPackage.ActivitySeries.AddRange(r);
        if (synchronizationDataPackage.TotalMinutes != this._totalMinutes)
          throw new Exception(string.Format(
                  "Number of received minute data ({0}) is not the same as declared in first header {1}",
              (object) synchronizationDataPackage.TotalMinutes, (object) this._totalMinutes));
        return synchronizationDataPackage;
      }
      catch (Exception ex)
      {
        this._log.Error("Error while anylyzing activity fragments " + (object) ex);
        return (SynchronizationDataPackage) null;
      }
    }

    private IEnumerable<MiBandOneActivityDataLoader.DataSyncFragment> GetFragmentsFromBytes()
    {
      this._dataMemoryStream.Seek(0L, SeekOrigin.Begin);
      int fragmentLen = 0;
      do
      {
        MiBandOneActivityDataLoader.DataSyncFragment fragment = this.GetFragment();
        fragmentLen = fragment.TotalMinutes;
        yield return fragment;
      }
      while (fragmentLen != 0);
    }

    private IEnumerable<RawMinuteActivityDataSeries> ProcessAllFragments(
      List<MiBandOneActivityDataLoader.DataSyncFragment> activityDataFragments)
    {
      foreach (MiBandOneActivityDataLoader.DataSyncFragment activityDataFragment in activityDataFragments)
      {
        RawMinuteActivityDataSeries activityDataSeries = new RawMinuteActivityDataSeries();
        activityDataSeries.StartTime = (DateTimeOffset) activityDataFragment.TimeStampStart;
        byte[] dataArray = activityDataFragment.DataArray;
        for (int index = 0; index < activityDataFragment.TotalMinutes; ++index)
        {
          RawMinuteActivityData minuteActivityData = new RawMinuteActivityData()
          {
            Mode = (int) dataArray[index * this._bytesPerMinute],
            Activity = (int) dataArray[index * this._bytesPerMinute + 1],
            Steps = (int) dataArray[index * this._bytesPerMinute + 2]
          };
          
          if (this._bytesPerMinute == 4)
            minuteActivityData.HeartRate = (int) dataArray[index * this._bytesPerMinute + 3];

          activityDataSeries.Data.Add(minuteActivityData);
        }
        yield return activityDataSeries;
      }
    }

    private MiBandOneActivityDataLoader.DataSyncFragment GetFragment()
    {
      byte[] numArray = new byte[11];
      if (this._dataMemoryStream.Read(numArray, 0, numArray.Length) != numArray.Length)
        throw new InvalidOperationException("Couldn't completely read data header from memory stream.");
      this._log.Debug("Fragment header: " + string.Join<byte>(" ", (IEnumerable<byte>) numArray));
      int num = (int) numArray[0];

      MiBandOneActivityDataLoader.DataSyncFragment fragment = new MiBandOneActivityDataLoader.DataSyncFragment(new DateTime(2000 + (int) numArray[1], (int) numArray[2] + 1, (int) numArray[3], (int) numArray[4], (int) numArray[5], (int) numArray[6]), (int) numArray[9] + ((int) numArray[10] << 8), this._bytesPerMinute);
      if (fragment.TotalMinutes != 0 && this._dataMemoryStream.Read(fragment.DataArray, 0, fragment.DataArray.Length) != fragment.DataArray.Length)
        throw new InvalidOperationException("Couldn't completely read data from memory stream.");
     
      return fragment;
    }

    private class DataSyncFragment
    {
      public DataSyncFragment(DateTime timeStampStart, int totalMinutes, int bytePerMinute)
      {
        this.TimeStampStart = timeStampStart;
        this.TotalMinutes = totalMinutes;
        this.DataArray = new byte[totalMinutes * bytePerMinute];
      }

      public byte[] DataArray { get; private set; }

      public DateTime TimeStampStart { get; private set; }

      public int TotalMinutes { get; private set; }
    }
  }
}
