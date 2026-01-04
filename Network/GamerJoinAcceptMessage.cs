// Decompiled with JetBrains decompiler
// Type: Magicka.Network.GamerJoinAcceptMessage
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using System.IO;

#nullable disable
namespace Magicka.Network;

internal struct GamerJoinAcceptMessage : ISendable
{
  public string Gamer;
  public sbyte Id;

  public PacketType PacketType => PacketType.GamerJoin;

  public void Write(BinaryWriter iWriter)
  {
    iWriter.Write(this.Gamer);
    iWriter.Write(this.Id);
  }

  public void Read(BinaryReader iReader)
  {
    this.Gamer = iReader.ReadString();
    this.Id = iReader.ReadSByte();
  }
}
