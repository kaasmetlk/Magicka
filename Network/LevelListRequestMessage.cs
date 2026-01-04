// Decompiled with JetBrains decompiler
// Type: Magicka.Network.LevelListRequestMessage
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.GameStates;
using System.IO;

#nullable disable
namespace Magicka.Network;

internal struct LevelListRequestMessage : ISendable
{
  public GameType GameType;

  public PacketType PacketType => PacketType.LevelList | PacketType.Request;

  public void Write(BinaryWriter iWriter) => iWriter.Write((byte) this.GameType);

  public void Read(BinaryReader iReader) => this.GameType = (GameType) iReader.ReadByte();
}
