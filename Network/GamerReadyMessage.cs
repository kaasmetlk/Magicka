// Decompiled with JetBrains decompiler
// Type: Magicka.Network.GamerReadyMessage
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using System.IO;

#nullable disable
namespace Magicka.Network;

internal struct GamerReadyMessage : ISendable
{
  public byte Id;
  public bool Ready;

  public PacketType PacketType => PacketType.GamerReady;

  public void Write(BinaryWriter iWriter)
  {
    iWriter.Write(this.Id);
    iWriter.Write(this.Ready);
  }

  public void Read(BinaryReader iReader)
  {
    this.Id = iReader.ReadByte();
    this.Ready = iReader.ReadBoolean();
  }
}
