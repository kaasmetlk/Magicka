// Decompiled with JetBrains decompiler
// Type: Magicka.Network.GameInfoMessage
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.GameStates;
using Magicka.Levels.Campaign;
using System;
using System.IO;

#nullable disable
namespace Magicka.Network;

internal struct GameInfoMessage : ISendable
{
  public int Latency;
  public string GameName;
  public byte NrOfPlayers;
  public GameType GameType;
  public int Level;

  public PacketType PacketType => PacketType.GameInfo;

  public void Write(BinaryWriter iWriter)
  {
    iWriter.Write(this.GameName);
    iWriter.Write(this.NrOfPlayers);
    iWriter.Write((byte) this.GameType);
    int index = this.Level;
    if (index == -1)
      index = 0;
    byte[] combinedHash;
    switch (this.GameType)
    {
      case GameType.Campaign:
        combinedHash = LevelManager.Instance.VanillaCampaign[index].GetCombinedHash();
        break;
      case GameType.Challenge:
        combinedHash = LevelManager.Instance.Challenges[index].GetCombinedHash();
        break;
      case GameType.Versus:
        combinedHash = LevelManager.Instance.Versus[index].GetCombinedHash();
        break;
      case GameType.Mythos:
        combinedHash = LevelManager.Instance.MythosCampaign[index].GetCombinedHash();
        break;
      case GameType.StoryChallange:
        combinedHash = LevelManager.Instance.StoryChallanges[index].GetCombinedHash();
        break;
      default:
        throw new Exception("Invalid GameType!");
    }
    iWriter.Write(combinedHash.Length);
    iWriter.Write(combinedHash);
  }

  public void Read(BinaryReader iReader)
  {
    this.GameName = iReader.ReadString();
    this.NrOfPlayers = iReader.ReadByte();
    this.GameType = (GameType) iReader.ReadByte();
    LevelManager.Instance.GetLevel(this.GameType, iReader.ReadBytes(iReader.ReadInt32()), out this.Level);
  }
}
