// Decompiled with JetBrains decompiler
// Type: Magicka.Network.ConnectionClosedMessage
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using System.IO;

#nullable disable
namespace Magicka.Network;

internal struct ConnectionClosedMessage : ISendable
{
  public ConnectionClosedMessage.CReason Reason;

  public PacketType PacketType => PacketType.ConnectionClosed;

  public void Write(BinaryWriter iWriter) => iWriter.Write((byte) this.Reason);

  public void Read(BinaryReader iReader)
  {
    this.Reason = (ConnectionClosedMessage.CReason) iReader.ReadByte();
  }

  public enum CReason : byte
  {
    Kicked,
    Left,
    LostConnection,
    Unknown,
  }
}
