// Decompiled with JetBrains decompiler
// Type: Magicka.Network.ForceSyncPlayerStatusesMessage
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using System.IO;

#nullable disable
namespace Magicka.Network;

internal struct ForceSyncPlayerStatusesMessage : ISendable
{
  public short numPlayers;
  public EntityUpdateMessage[] playerUpdateMessages;

  public PacketType PacketType => PacketType.ForceSyncPlayersMessage;

  public void Write(BinaryWriter iWriter)
  {
    iWriter.Write(this.numPlayers);
    for (int index = 0; index < (int) this.numPlayers; ++index)
      this.playerUpdateMessages[index].Write(iWriter);
  }

  public void Read(BinaryReader iReader)
  {
    this.numPlayers = iReader.ReadInt16();
    this.playerUpdateMessages = new EntityUpdateMessage[(int) this.numPlayers];
    for (int index = 0; index < (int) this.numPlayers; ++index)
      this.playerUpdateMessages[index].Read(iReader);
  }
}
