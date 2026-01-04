// Decompiled with JetBrains decompiler
// Type: Magicka.Network.ConnectReplyMessage
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using SteamWrapper;
using System.IO;

#nullable disable
namespace Magicka.Network;

internal struct ConnectReplyMessage : ISendable
{
  public SteamID ServerID;
  public bool VACSecure;
  public string ServerName;

  public PacketType PacketType => PacketType.Connect;

  public void Write(BinaryWriter iWriter)
  {
    iWriter.Write(this.ServerID.AsUInt64);
    iWriter.Write(this.VACSecure);
    iWriter.Write(this.ServerName);
  }

  public void Read(BinaryReader iReader)
  {
    this.ServerID = new SteamID(iReader.ReadUInt64());
    this.VACSecure = iReader.ReadBoolean();
    this.ServerName = iReader.ReadString();
  }
}
