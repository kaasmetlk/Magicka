// Decompiled with JetBrains decompiler
// Type: Magicka.Gamers.Gamer
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Achievements;
using Magicka.GameLogic;
using Magicka.GameLogic.GameStates;
using System;
using System.IO;

#nullable disable
namespace Magicka.Gamers;

public class Gamer
{
  private static Random sRandom = new Random();
  public static readonly Gamer INVALID_GAMER = new Gamer("");
  protected string mGamerTag;
  protected byte mColor;
  protected Profile.PlayableAvatar mAvatar;
  public uint Kills;
  public uint Deaths;
  public uint OverKilled;
  public uint OverKills;
  public ulong HealingDone;
  public ulong HealingReceived;
  public ulong DamageDone;
  public ulong DamageReceived;
  public uint Suicides;
  public uint TeamKills;
  public uint TeamKilled;
  protected uint mVersusWins;
  protected uint mVersusWinStreak;
  protected uint mVersusDefeats;

  public virtual Profile.PlayableAvatar Avatar
  {
    get => this.mAvatar;
    set => this.mAvatar = value;
  }

  public uint VersusWins
  {
    get => this.mVersusWins;
    set => this.mVersusWins = value;
  }

  public uint VersusDefeats
  {
    get => this.mVersusDefeats;
    set => this.mVersusDefeats = value;
  }

  public void IncrementVersusWinStreak(PlayState iPlayState)
  {
    ++this.mVersusWinStreak;
    if (this.mVersusWinStreak < 20U)
      return;
    AchievementsManager.Instance.AwardAchievement(iPlayState, "nothingbutaman");
  }

  public void ResetVersusWinStreak() => this.mVersusWinStreak = 0U;

  public string GamerTag
  {
    get => this.mGamerTag;
    set => this.mGamerTag = value;
  }

  public virtual byte Color
  {
    get => this.mColor;
    set => this.mColor = value;
  }

  private Gamer()
  {
  }

  public Gamer(string iGamerTag)
  {
    if (iGamerTag.Length > 15)
      iGamerTag = iGamerTag.Substring(0, 15);
    this.mGamerTag = iGamerTag;
    this.mAvatar = Profile.Instance.DefaultAvatar;
    this.mColor = (byte) Gamer.sRandom.Next(Defines.PLAYERCOLORS_UNLOCKED);
  }

  public bool InUse
  {
    get
    {
      Player[] players = Game.Instance.Players;
      for (int index = 0; index < players.Length; ++index)
      {
        if (players[index].Playing & players[index].Gamer == this)
          return true;
      }
      return false;
    }
  }

  public static Gamer Read(BinaryReader iReader)
  {
    Gamer gamer = new Gamer();
    gamer.mGamerTag = iReader.ReadString();
    if (gamer.mGamerTag.Length > 15)
      gamer.mGamerTag = gamer.mGamerTag.Substring(0, 15);
    gamer.mColor = iReader.ReadByte();
    string iName = iReader.ReadString();
    gamer.mAvatar = Profile.Instance.GetAvatar(iName);
    gamer.Kills = iReader.ReadUInt32();
    gamer.Deaths = iReader.ReadUInt32();
    gamer.OverKilled = iReader.ReadUInt32();
    gamer.OverKills = iReader.ReadUInt32();
    gamer.HealingDone = iReader.ReadUInt64();
    gamer.HealingReceived = iReader.ReadUInt64();
    gamer.DamageDone = iReader.ReadUInt64();
    gamer.DamageReceived = iReader.ReadUInt64();
    gamer.Suicides = iReader.ReadUInt32();
    gamer.TeamKills = iReader.ReadUInt32();
    gamer.TeamKilled = iReader.ReadUInt32();
    gamer.mVersusWins = iReader.ReadUInt32();
    gamer.mVersusWinStreak = iReader.ReadUInt32();
    gamer.mVersusDefeats = iReader.ReadUInt32();
    return gamer;
  }

  public void Write(BinaryWriter iWriter)
  {
    iWriter.Write(this.mGamerTag);
    iWriter.Write(this.mColor);
    iWriter.Write(this.mAvatar.Name);
    iWriter.Write(this.Kills);
    iWriter.Write(this.Deaths);
    iWriter.Write(this.OverKilled);
    iWriter.Write(this.OverKills);
    iWriter.Write(this.HealingDone);
    iWriter.Write(this.HealingReceived);
    iWriter.Write(this.DamageDone);
    iWriter.Write(this.DamageReceived);
    iWriter.Write(this.Suicides);
    iWriter.Write(this.TeamKills);
    iWriter.Write(this.TeamKilled);
    iWriter.Write(this.mVersusWins);
    iWriter.Write(this.mVersusWinStreak);
    iWriter.Write(this.mVersusDefeats);
  }
}
