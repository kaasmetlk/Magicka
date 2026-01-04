// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Statistics.StatisticsManager
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities;
using Magicka.GameLogic.GameStates;
using Magicka.Levels;
using Magicka.Levels.Campaign;
using Magicka.Levels.Versus;
using Magicka.Network;
using SteamWrapper;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Statistics;

public class StatisticsManager
{
  private static StatisticsManager mSingelton;
  private static volatile object mSingeltonLock = new object();
  private MultiKillCounter[] mPlayerMultiKillCounter;
  private List<List<LeaderBoardData>> mLeaderboards;
  private List<ulong> mSteamLeaderboards;
  private StatisticsManager.ScoreValues[] mScores;
  private int mTotalScore;
  private float mTotalPlayerDamage;

  public static StatisticsManager Instance
  {
    get
    {
      if (StatisticsManager.mSingelton == null)
      {
        lock (StatisticsManager.mSingeltonLock)
        {
          if (StatisticsManager.mSingelton == null)
            StatisticsManager.mSingelton = new StatisticsManager();
        }
      }
      return StatisticsManager.mSingelton;
    }
  }

  private StatisticsManager()
  {
    this.mScores = new StatisticsManager.ScoreValues[4];
    this.mPlayerMultiKillCounter = new MultiKillCounter[4];
    for (int index = 0; index < 4; ++index)
      this.mPlayerMultiKillCounter[index] = new MultiKillCounter(100);
    int length = LevelManager.Instance.Challenges.Length;
    this.mLeaderboards = new List<List<LeaderBoardData>>(length);
    for (int index = 0; index < length; ++index)
      this.mLeaderboards.Add(new List<LeaderBoardData>(8));
    this.mSteamLeaderboards = new List<ulong>(length);
    LevelNode[] challenges = LevelManager.Instance.Challenges;
    for (int index = 0; index < length; ++index)
      SteamUserStats.FindLeaderboard(challenges[index].FileName, new Action<LeaderboardFindResult>(this.OnlineFindResult));
  }

  public void AddLocalEntry(int iChallengeIndex, LeaderBoardData iData)
  {
    this.mLeaderboards[iChallengeIndex].Add(iData);
    this.mLeaderboards[iChallengeIndex].Sort((IComparer<LeaderBoardData>) LeaderBoardData.ScoreBeforeDataComparer);
    if (this.mLeaderboards[iChallengeIndex].Count <= 8)
      return;
    this.mLeaderboards[iChallengeIndex].RemoveAt(8);
  }

  public List<LeaderBoardData> Leaderboard(int iChallengeIndex)
  {
    return this.mLeaderboards[iChallengeIndex];
  }

  public void AddOnlineEntry(int iChallengeIndex, LeaderBoardData iData)
  {
    if (NetworkManager.Instance.State == NetworkState.Client)
      return;
    if (this.mSteamLeaderboards.Count > iChallengeIndex)
      SteamUserStats.UploadLeaderboardScore(this.mSteamLeaderboards[iChallengeIndex], LeaderboardUploadScoreMethod.KeepBest, iData.Score, new int[1]
      {
        iData.Data1
      }, new Action<LeaderboardScoreUploaded>(this.OnlineScoreUploaded));
    if (NetworkManager.Instance.State != NetworkState.Server)
      return;
    NetworkManager.Instance.Interface.SendMessage<LeaderboardMessage>(ref new LeaderboardMessage()
    {
      SteamLeaderboard = this.mSteamLeaderboards[iChallengeIndex],
      ScoreMethod = LeaderboardUploadScoreMethod.KeepBest,
      Score = iData.Score,
      Data = iData.Data1
    });
  }

  public List<ulong> SteamLeaderboards => this.mSteamLeaderboards;

  private void OnlineFindResult(LeaderboardFindResult iResult)
  {
    if (iResult.mLeaderboardFound)
      this.mSteamLeaderboards.Add(iResult.mSteamLeaderboard);
    else
      this.mSteamLeaderboards.Add(0UL);
  }

  internal void OnlineScoreUploaded(LeaderboardScoreUploaded iUploaded)
  {
  }

  public void SurvivalReset()
  {
    this.mTotalScore = 0;
    this.mTotalPlayerDamage = 0.0f;
    for (int index = 0; index < this.mScores.Length; ++index)
      this.mScores[index] = new StatisticsManager.ScoreValues();
  }

  private void InternalDamageEvent(
    PlayState iPlayState,
    IDamageable iAttacker,
    IDamageable iTarget,
    double iTimeStamp,
    Damage iDamage,
    DamageResult iResult,
    int iMultiplier)
  {
    if (iResult == DamageResult.None | (iResult & DamageResult.Deflected) == DamageResult.Deflected)
      return;
    if (iAttacker is NonPlayerCharacter)
    {
      NonPlayerCharacter nonPlayerCharacter = iAttacker as NonPlayerCharacter;
      if (nonPlayerCharacter.IsCharmed)
        iAttacker = nonPlayerCharacter.CharmOwner as IDamageable;
      else if (nonPlayerCharacter.IsSummoned)
        iAttacker = (IDamageable) nonPlayerCharacter.Master;
    }
    Avatar avatar1 = iAttacker as Avatar;
    Avatar avatar2 = iTarget as Avatar;
    IRuleset ruleSet = iPlayState.Level.CurrentScene.RuleSet;
    if (avatar1 != null)
    {
      Magicka.GameLogic.Player player = avatar1.Player;
      if (player.Gamer == null)
        return;
      if ((iResult & DamageResult.Healed) == DamageResult.Healed)
      {
        this.mScores[player.ID].HealingDone += (ulong) -(double) iDamage.Amount;
        player.Gamer.HealingDone += (ulong) -(double) iDamage.Amount;
      }
      if (iTarget is Character)
      {
        if ((iResult & DamageResult.Damaged) == DamageResult.Damaged)
        {
          this.mTotalPlayerDamage += iDamage.Amount * iDamage.Magnitude;
          this.mScores[player.ID].DamageDone += (ulong) ((double) iDamage.Amount * (double) iDamage.Magnitude);
          player.Gamer.DamageDone += (ulong) ((double) iDamage.Amount * (double) iDamage.Magnitude);
        }
        if ((iResult & DamageResult.Killed) != DamageResult.None)
        {
          bool flag = (iResult & DamageResult.OverKilled) == DamageResult.OverKilled;
          if (iTarget is NonPlayerCharacter nonPlayerCharacter)
          {
            if (nonPlayerCharacter.Undying & !flag & nonPlayerCharacter.ScoreValue == 0)
              return;
            if (ruleSet is SurvivalRuleset survivalRuleset)
            {
              int num = nonPlayerCharacter.ScoreValue;
              if ((avatar1.Faction & (iTarget as Character).Faction) != Factions.NONE)
                num = -nonPlayerCharacter.ScoreValue;
              survivalRuleset.AddScore(nonPlayerCharacter.DisplayName, num * iMultiplier);
            }
            this.mTotalScore += nonPlayerCharacter.ScoreValue * iMultiplier;
            this.mTotalPlayerDamage += iDamage.Amount * iDamage.Magnitude;
            this.mPlayerMultiKillCounter[player.ID].Add(iTimeStamp);
          }
        }
      }
    }
    if (avatar2 == null)
      return;
    Magicka.GameLogic.Player player1 = avatar2.Player;
    if (player1 == null || player1.Gamer == null)
      return;
    if ((double) iDamage.Amount * (double) iDamage.Magnitude < 0.0)
    {
      this.mScores[player1.ID].HealingReceived += (ulong) -(double) iDamage.Amount;
      player1.Gamer.HealingReceived += (ulong) -(double) iDamage.Amount;
    }
    if ((iResult & DamageResult.Damaged) != DamageResult.Damaged || (double) iDamage.Amount * (double) iDamage.Magnitude <= 0.0)
      return;
    this.mScores[player1.ID].DamageReceived += (ulong) ((double) iDamage.Amount * (double) iDamage.Magnitude);
    player1.Gamer.DamageReceived += (ulong) ((double) iDamage.Amount * (double) iDamage.Magnitude);
  }

  public void AddKillEvent(PlayState iPlayState, Entity iTarget, Entity iAttacker)
  {
    VersusRuleset ruleSet1 = iPlayState.Level.CurrentScene.RuleSet as VersusRuleset;
    SurvivalRuleset ruleSet2 = iPlayState.Level.CurrentScene.RuleSet as SurvivalRuleset;
    NonPlayerCharacter nonPlayerCharacter = iAttacker as NonPlayerCharacter;
    NonPlayerCharacter iTarget1 = iTarget as NonPlayerCharacter;
    Avatar tarAvatar = iTarget as Avatar;
    Avatar atkAvatar = iAttacker as Avatar;
    if (nonPlayerCharacter != null)
    {
      if (nonPlayerCharacter.IsCharmed)
        atkAvatar = nonPlayerCharacter.CharmOwner as Avatar;
      else if (nonPlayerCharacter.IsSummoned)
        atkAvatar = nonPlayerCharacter.Master as Avatar;
    }
    if (tarAvatar != null)
    {
      Magicka.GameLogic.Player player = tarAvatar.Player;
      ++this.mScores[player.ID].Deaths;
      if (player.Gamer != null)
        ++player.Gamer.Deaths;
    }
    if (ruleSet1 != null)
    {
      if (atkAvatar != null && tarAvatar != null && (atkAvatar.Faction & tarAvatar.Faction) == Factions.NONE)
      {
        Magicka.GameLogic.Player player = atkAvatar.Player;
        ++this.mScores[player.ID].Kills;
        if (player.Gamer != null)
          ++player.Gamer.Kills;
        ruleSet1.OnPlayerKill(atkAvatar, tarAvatar);
      }
    }
    else if (ruleSet2 != null && atkAvatar != null && iTarget1 != null)
    {
      Magicka.GameLogic.Player player = atkAvatar.Player;
      ++this.mScores[player.ID].Kills;
      if (player.Gamer != null)
        ++player.Gamer.Kills;
      int num = iTarget1.ScoreValue;
      int scoreMultiplier = ruleSet2.ScoreMultiplier;
      if ((atkAvatar.Faction & iTarget1.Faction) != Factions.NONE)
        num = -iTarget1.ScoreValue;
      ruleSet2.AddScore(iTarget1.DisplayName, num * scoreMultiplier);
    }
    if (atkAvatar == null || iTarget1 == null)
      return;
    Profile.Instance.AddMythosKill(iPlayState, (Character) iTarget1);
  }

  public void AddDamageEvent(
    PlayState iPlayState,
    IDamageable iAttacker,
    IDamageable iTarget,
    double iTimeStamp,
    Damage iDamage,
    DamageResult iResult)
  {
    if (NetworkManager.Instance.State == NetworkState.Client)
      return;
    int iMultiplier = 1;
    if (iPlayState.Level.CurrentScene.RuleSet is SurvivalRuleset)
      iMultiplier = (iPlayState.Level.CurrentScene.RuleSet as SurvivalRuleset).ScoreMultiplier;
    this.InternalDamageEvent(iPlayState, iAttacker, iTarget, iTimeStamp, iDamage, iResult, iMultiplier);
    if (NetworkManager.Instance.State != NetworkState.Server)
      return;
    StatisticsMessage iMessage;
    iMessage.AttackerHandle = iAttacker != null ? iAttacker.Handle : ushort.MaxValue;
    iMessage.TargetHandle = iTarget.Handle;
    iMessage.TimeStamp = iTimeStamp;
    iMessage.Damage = iDamage;
    iMessage.Result = iResult;
    iMessage.Multiplier = (byte) iMultiplier;
    NetworkManager.Instance.Interface.SendMessage<StatisticsMessage>(ref iMessage);
  }

  internal void NetworkUpdate(ref StatisticsMessage iMsg)
  {
    PlayState iPlayState = (PlayState) null;
    IDamageable iAttacker = (IDamageable) null;
    IDamageable iTarget = (IDamageable) null;
    if (iMsg.AttackerHandle != ushort.MaxValue)
    {
      Entity fromHandle = Entity.GetFromHandle((int) iMsg.AttackerHandle);
      iAttacker = fromHandle as IDamageable;
      iPlayState = fromHandle.PlayState;
    }
    if (iMsg.TargetHandle != ushort.MaxValue)
    {
      Entity fromHandle = Entity.GetFromHandle((int) iMsg.TargetHandle);
      iTarget = fromHandle as IDamageable;
      iPlayState = fromHandle.PlayState;
    }
    if (iPlayState == null)
      return;
    this.InternalDamageEvent(iPlayState, iAttacker, iTarget, iMsg.TimeStamp, iMsg.Damage, iMsg.Result, (int) iMsg.Multiplier);
  }

  public int SurvivalTotalScore => this.mTotalScore;

  public float SurvivalTotalDamage => this.mTotalPlayerDamage;

  public void UpdatePlayerMultiKillCounter(float iDeltaTime)
  {
    for (int index = 0; index < this.mPlayerMultiKillCounter.Length; ++index)
      this.mPlayerMultiKillCounter[index].Update(Game.Instance.Players[index], iDeltaTime);
  }

  public void ClearPlayerMultiKillCounter()
  {
    for (int index = 0; index < this.mPlayerMultiKillCounter.Length; ++index)
      this.mPlayerMultiKillCounter[index].Clear();
  }

  public StatisticsManager.ScoreValues GetScore(int iPlayer) => this.mScores[iPlayer];

  public struct ScoreValues
  {
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
  }
}
