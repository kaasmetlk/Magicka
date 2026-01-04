// Decompiled with JetBrains decompiler
// Type: Magicka.Network.ClientConnectedMessage
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using SteamWrapper;
using System.IO;

#nullable disable
namespace Magicka.Network;

internal struct ClientConnectedMessage : ISendable
{
  public SteamID ID;

  public PacketType PacketType => PacketType.ClientConnected;

  public void Write(BinaryWriter iWriter) => iWriter.Write(this.ID.AsUInt64);

  public void Read(BinaryReader iReader) => this.ID = new SteamID(iReader.ReadUInt64());
}
