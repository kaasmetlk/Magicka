// Decompiled with JetBrains decompiler
// Type: Magicka.Network.PingRequestMessage
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using System.IO;

#nullable disable
namespace Magicka.Network;

internal struct PingRequestMessage : ISendable
{
  public int Payload;

  public PacketType PacketType => PacketType.Request;

  public void Write(BinaryWriter iWriter) => iWriter.Write(this.Payload);

  public void Read(BinaryReader iReader) => this.Payload = iReader.ReadInt32();
}
