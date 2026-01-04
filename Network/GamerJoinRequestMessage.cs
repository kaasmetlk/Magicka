// Decompiled with JetBrains decompiler
// Type: Magicka.Network.GamerJoinRequestMessage
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using SteamWrapper;
using System.IO;

#nullable disable
namespace Magicka.Network;

internal struct GamerJoinRequestMessage : ISendable
{
  public string Gamer;
  public string AvatarThumb;
  public string AvatarPortrait;
  public string AvatarType;
  public sbyte Id;
  public byte Color;
  public SteamID SteamID;

  public PacketType PacketType => PacketType.GamerJoin | PacketType.Request;

  public void Write(BinaryWriter iWriter)
  {
    iWriter.Write(this.Gamer);
    iWriter.Write(this.AvatarThumb);
    iWriter.Write(this.AvatarPortrait);
    iWriter.Write(this.AvatarType);
    iWriter.Write(this.Id);
    iWriter.Write(this.Color);
    iWriter.Write(this.SteamID.AsUInt64);
  }

  public void Read(BinaryReader iReader)
  {
    this.Gamer = iReader.ReadString();
    this.AvatarThumb = iReader.ReadString();
    this.AvatarPortrait = iReader.ReadString();
    this.AvatarType = iReader.ReadString();
    this.Id = iReader.ReadSByte();
    this.Color = iReader.ReadByte();
    this.SteamID = new SteamID(iReader.ReadUInt64());
  }
}
