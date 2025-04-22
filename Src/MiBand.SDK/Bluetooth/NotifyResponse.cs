// Decompiled with JetBrains decompiler
// Type: MiBand.SDK.Bluetooth.NotifyResponse
// Assembly: MiBand.SDK, Version=0.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 152FA565-3249-4054-A361-01D9C0AFA7F1
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBand.SDK.dll

using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace MiBand.SDK.Bluetooth
{
  internal class NotifyResponse
  {
    public NotifyResponse(byte[] rawBytes, int payloadOffset = 0)
    {
      this.RawBytes = rawBytes;
      if (rawBytes.Length < 3 + payloadOffset)
        return;
      this.Flag = this.RawBytes[0];
      this.Command = this.RawBytes[1];
      this.Code = this.RawBytes[2 + payloadOffset];
      int count = this.RawBytes.Length - payloadOffset - 3;
      if (count == 0)
        return;
      this.Payload = ((IEnumerable<byte>) this.RawBytes).Skip<byte>(3 + payloadOffset).Take<byte>(count).ToArray<byte>();
    }

    public byte Flag { get; }

    public byte Command { get; }

    public byte Code { get; }

    public bool IsSuccess => this.Code == (byte) 1;

    public byte[] Payload { get; }

    public byte[] RawBytes { get; }

    public bool IsSuccessCommand(int command) => command == (int) this.Command && this.IsSuccess;

    public bool IsFailCommand(int command) => command == (int) this.Command && !this.IsSuccess;

    public override string ToString()
    {
      string str = "(null)";
      if (this.RawBytes != null)
        str = string.Join<byte>(" ", (IEnumerable<byte>) this.RawBytes);
      return string.Format("Flag: {0}; Command: {1}; Code: {2}; IsSuccess: {3}; RawBytes: {4}", (object) this.Flag, (object) this.Command, (object) this.Code, (object) this.IsSuccess, (object) str);
    }
  }
}
