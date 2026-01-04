// Decompiled with JetBrains decompiler
// Type: Magicka.Network.LeaderboardMessage
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using SteamWrapper;
using System.IO;

#nullable disable
namespace Magicka.Network;

internal struct LeaderboardMessage : ISendable
{
  public ulong SteamLeaderboard;
  public LeaderboardUploadScoreMethod ScoreMethod;
  public int Score;
  public int Data;

  public PacketType PacketType => PacketType.LeaderboardEntry;

  public void Write(BinaryWriter iWriter)
  {
    iWriter.Write(this.SteamLeaderboard);
    iWriter.Write((byte) this.ScoreMethod);
    iWriter.Write(this.Score);
    iWriter.Write(this.Data);
  }

  public void Read(BinaryReader iReader)
  {
    this.SteamLeaderboard = iReader.ReadUInt64();
    this.ScoreMethod = (LeaderboardUploadScoreMethod) iReader.ReadByte();
    this.Score = iReader.ReadInt32();
    this.Data = iReader.ReadInt32();
  }
}
