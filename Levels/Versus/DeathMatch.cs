// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Versus.DeathMatch
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.Levels.Triggers;
using Magicka.Misc;
using Magicka.Network;
using Magicka.WebTools.Paradox.Telemetry;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using System;
using System.Collections.Generic;
using System.Xml;

#nullable disable
namespace Magicka.Levels.Versus;

internal class DeathMatch : VersusRuleset
{
  private short[] mTeamScores = new short[2];
  private short[] mScores = new short[4];
  private float[] mRespawnTimers = new float[4];
  private float mTimeLimitTimer;
  private float mLuggageTimer;
  private float mTimeLimitNetworkUpdate;
  private float mTimeLimitTarget;
  private float mTimeLimit;
  private int mScoreLimit;
  private float mRespawnTime;
  private float mLuggageTime;
  private bool mTeams;
  private bool mNetworkInitialized;
  private DeathMatch.Settings mSettings;

  public DeathMatch(GameScene iScene, XmlNode iNode, DeathMatch.Settings iSettings)
    : base(iScene, iNode)
  {
    this.mSettings = iSettings;
  }

  public override void OnPlayerKill(Avatar iAttacker, Avatar iTarget)
  {
    if (NetworkManager.Instance.State == NetworkState.Client || this.mScene.PlayState.IsGameEnded)
      return;
    if (iAttacker == iTarget)
      --this.mScores[iAttacker.Player.ID];
    else
      ++this.mScores[iAttacker.Player.ID];
    if ((iAttacker.Player.Team & Factions.TEAM_RED) == Factions.TEAM_RED)
    {
      if (iTarget != null && (iTarget.Player.Team & Factions.TEAM_RED) == Factions.TEAM_RED)
        --this.mTeamScores[0];
      else
        ++this.mTeamScores[0];
      this.mScoreUIs[0].SetScore((int) this.mTeamScores[0]);
      if ((int) this.mTeamScores[0] >= this.mScoreLimit)
        this.EndGame();
    }
    else if ((iAttacker.Player.Team & Factions.TEAM_BLUE) == Factions.TEAM_BLUE)
    {
      if (iTarget != null && (iTarget.Player.Team & Factions.TEAM_BLUE) == Factions.TEAM_BLUE)
        --this.mTeamScores[1];
      else
        ++this.mTeamScores[1];
      this.mScoreUIs[1].SetScore((int) this.mTeamScores[1]);
      if ((int) this.mTeamScores[1] >= this.mScoreLimit)
        this.EndGame();
    }
    else
    {
      int index = this.mIDToScoreUILookUp[iAttacker.Player.ID];
      if (index != -1)
        this.mScoreUIs[index].SetScore((int) this.mScores[iAttacker.Player.ID]);
      if ((int) this.mScores[iAttacker.Player.ID] >= this.mScoreLimit)
        this.EndGame();
    }
    this.NetworkScore(iAttacker.Player.ID);
  }

  public override void OnEndGame()
  {
    Magicka.GameLogic.Player player = ControlManager.Instance.MenuController.Player;
    if ((player.Team & Factions.TEAM_RED) == Factions.TEAM_RED)
    {
      if ((int) this.mTeamScores[0] < this.mScoreLimit)
        return;
      Singleton<PlayerSegmentManager>.Instance.NotifyWonAgainstHuman();
    }
    else if ((player.Team & Factions.TEAM_BLUE) == Factions.TEAM_BLUE)
    {
      if ((int) this.mTeamScores[1] < this.mScoreLimit)
        return;
      Singleton<PlayerSegmentManager>.Instance.NotifyWonAgainstHuman();
    }
    else
    {
      if ((int) this.mScores[player.ID] < this.mScoreLimit)
        return;
      Singleton<PlayerSegmentManager>.Instance.NotifyWonAgainstHuman();
    }
  }

  public override void OnPlayerDeath(Magicka.GameLogic.Player iPlayer)
  {
    if (this.mScene.PlayState.IsGameEnded && (double) this.mRespawnTimers[iPlayer.ID] <= 0.0 || (double) this.mRespawnTimers[iPlayer.ID] > 0.0)
      return;
    this.mRespawnTimers[iPlayer.ID] = this.mRespawnTime;
  }

  public override void Initialize()
  {
    base.Initialize();
    this.mTimeLimit = (float) this.mSettings.TimeLimit * 60f;
    this.mTimeLimitTimer = this.mTimeLimit;
    this.mTimeLimitTarget = this.mTimeLimit;
    this.mScoreLimit = this.mSettings.ScoreLimit;
    this.mRespawnTime = 5f;
    this.mLuggageTime = (float) this.mSettings.LuggageInterval;
    this.mLuggageTimer = this.mLuggageTime;
    this.mTeams = this.mSettings.TeamsEnabled;
    if (this.mTeams)
    {
      this.mScoreUIs.Add(new VersusRuleset.Score(true));
      this.mScoreUIs.Add(new VersusRuleset.Score(false));
      for (int key = 0; key < this.mPlayers.Length; ++key)
      {
        if (this.mPlayers[key].Playing)
        {
          Texture2D portrait = this.mPlayers[key].Gamer.Avatar.Portrait;
          this.mPlayers[key].Avatar.Faction &= ~Factions.FRIENDLY;
          if ((this.mPlayers[key].Team & Factions.TEAM_RED) == Factions.TEAM_RED)
          {
            this.mIDToScoreUILookUp[key] = 0;
            this.mScoreUIs[0].AddPlayer(this.mPlayers[key].GamerTag, this.mPlayers[key].ID, portrait, Defines.PLAYERCOLORS[(int) this.mPlayers[key].Color]);
          }
          else
          {
            this.mIDToScoreUILookUp[key] = 1;
            this.mScoreUIs[1].AddPlayer(this.mPlayers[key].GamerTag, this.mPlayers[key].ID, portrait, Defines.PLAYERCOLORS[(int) this.mPlayers[key].Color]);
          }
        }
      }
    }
    else
    {
      int key = 0;
      int num = 0;
      for (; key < this.mPlayers.Length; ++key)
      {
        if (this.mPlayers[key].Playing)
        {
          this.mPlayers[key].Avatar.Faction &= ~Factions.FRIENDLY;
          Texture2D portrait = this.mPlayers[key].Gamer.Avatar.Portrait;
          VersusRuleset.Score score = new VersusRuleset.Score(num % 2 == 0);
          score.AddPlayer(this.mPlayers[key].GamerTag, this.mPlayers[key].ID, portrait, Defines.PLAYERCOLORS[(int) this.mPlayers[key].Color]);
          this.mIDToScoreUILookUp[key] = this.mScoreUIs.Count;
          this.mScoreUIs.Add(score);
          ++num;
        }
      }
    }
    for (int index = 0; index < this.mScores.Length; ++index)
      this.mScores[index] = (short) 0;
    for (int index = 0; index < this.mTeamScores.Length; ++index)
      this.mTeamScores[index] = (short) 0;
    for (int index = 0; index < this.mRespawnTimers.Length; ++index)
      this.mRespawnTimers[index] = 0.0f;
    this.mNetworkInitialized = false;
  }

  public override void DeInitialize()
  {
    if (this.mGameClockCue == null || this.mGameClockCue.IsStopping)
      return;
    this.mGameClockCue.Stop(AudioStopOptions.AsAuthored);
  }

  public override void Update(float iDeltaTime, DataChannel iDataChannel)
  {
    if (!this.mNetworkInitialized)
    {
      if (NetworkManager.Instance.State != NetworkState.Client)
      {
        this.mTemporarySpawns.Clear();
        this.mTemporarySpawns.AddRange((IEnumerable<int>) this.mAreas_All);
        int num = VersusRuleset.RANDOM.Next(this.mTemporarySpawns.Count);
        for (int iID = 0; iID < this.mScores.Length; ++iID)
        {
          if (this.mPlayers[iID].Playing)
          {
            int index = num % this.mTemporarySpawns.Count;
            this.SetupPlayer(iID, this.mTemporarySpawns[index]);
            this.mTemporarySpawns.RemoveAt(index);
          }
        }
      }
      this.mNetworkInitialized = true;
    }
    if (iDataChannel == DataChannel.None)
      return;
    for (int iID = 0; iID < this.mRespawnTimers.Length; ++iID)
    {
      float num = Math.Max(this.mRespawnTimers[iID] - iDeltaTime, 0.0f);
      if ((double) this.mRespawnTimers[iID] > 0.0 && (double) num <= 0.0 && this.mPlayers[iID].Playing)
        this.SetupPlayer(iID, this.GetTeamArea(this.mPlayers[iID].Team));
      this.mRespawnTimers[iID] = num;
    }
    for (int index1 = 0; index1 < this.mRespawnTimers.Length; ++index1)
    {
      int index2;
      if (this.mIDToScoreUILookUp.TryGetValue(index1, out index2) && index2 != -1)
        this.mScoreUIs[index2].SetTimer(index1, (int) this.mRespawnTimers[index1]);
    }
    if ((double) this.mLuggageTime > 0.0)
    {
      if ((double) this.mLuggageTimer <= 0.0)
      {
        TriggerArea triggerArea = this.mScene.PlayState.Level.CurrentScene.GetTriggerArea(TriggerArea.ANYID);
        int num = VersusRuleset.RANDOM.Next(0, 3);
        if (triggerArea.GetCount(this.mItemTemplate.ID) == 0 && triggerArea.GetCount(this.mMagickTemplate.ID) == 0)
          this.SpawnLuggage(num != 0);
        this.mLuggageTimer = this.mLuggageTime;
      }
      this.mLuggageTimer -= iDeltaTime;
    }
    if ((double) this.mTimeLimit > 0.0)
    {
      if (!this.mScene.PlayState.IsGameEnded)
      {
        this.mTimeLimitTimer -= iDeltaTime;
        if ((double) this.mTimeLimitTimer <= 0.0)
        {
          this.EndGame();
          this.mTimeLimitTimer = 0.0f;
        }
      }
      if (NetworkManager.Instance.State == NetworkState.Server)
      {
        if ((double) this.mTimeLimitNetworkUpdate > 1.0)
        {
          NetworkManager.Instance.Interface.SendMessage<RulesetMessage>(ref new RulesetMessage()
          {
            Type = this.RulesetType,
            Byte01 = (byte) 0,
            Float01 = this.mTimeLimitTimer
          });
          this.mTimeLimitNetworkUpdate = 0.0f;
        }
        this.mTimeLimitNetworkUpdate += iDeltaTime;
      }
    }
    VersusRuleset.RenderData renderData = this.mRenderData[(int) iDataChannel];
    renderData.DrawTime = (double) this.mTimeLimit > 0.0;
    renderData.TimeLimit = this.mTimeLimit;
    renderData.Time = this.mTimeLimitTimer;
    if ((double) this.mTimeLimitTimer <= 10.0)
    {
      if (this.mGameClockCue == null && (double) this.mTimeLimitTimer > 0.0)
        this.mGameClockCue = AudioManager.Instance.PlayCue(Banks.Additional, VersusRuleset.SOUND_GAME_CLOCK);
      renderData.SetTimeText((int) this.mTimeLimitTimer);
    }
    else
      renderData.SetTimeText(0);
    base.Update(iDeltaTime, iDataChannel);
  }

  public override void LocalUpdate(float iDeltaTime, DataChannel iDataChannel)
  {
    if (iDataChannel == DataChannel.None)
      return;
    for (int iID = 0; iID < this.mRespawnTimers.Length; ++iID)
    {
      this.mRespawnTimers[iID] = Math.Max(this.mRespawnTimers[iID] - iDeltaTime, 0.0f);
      for (int index = 0; index < this.mScoreUIs.Count; ++index)
        this.mScoreUIs[index].SetTimer(iID, (int) this.mRespawnTimers[iID]);
    }
    VersusRuleset.RenderData renderData = this.mRenderData[(int) iDataChannel];
    this.mTimeLimitTimer += (this.mTimeLimitTarget - this.mTimeLimitTimer) * iDeltaTime;
    renderData.DrawTime = (double) this.mTimeLimit > 0.0;
    renderData.TimeLimit = this.mTimeLimit;
    renderData.Time = this.mTimeLimitTimer;
    if ((double) this.mTimeLimitTimer <= 10.0)
    {
      if (this.mGameClockCue == null && (double) this.mTimeLimitTimer > 0.0)
        this.mGameClockCue = AudioManager.Instance.PlayCue(Banks.Additional, VersusRuleset.SOUND_GAME_CLOCK);
      renderData.SetTimeText((int) this.mTimeLimitTimer);
    }
    else
      renderData.SetTimeText(0);
    base.LocalUpdate(iDeltaTime, iDataChannel);
  }

  protected unsafe void NetworkScore(int iID)
  {
    if (NetworkManager.Instance.State != NetworkState.Server)
      return;
    RulesetMessage iMessage = new RulesetMessage();
    iMessage.Type = this.RulesetType;
    iMessage.Byte01 = (byte) 4;
    iMessage.Byte02 = (byte) iID;
    iMessage.Scores[0] = this.mScores[0];
    iMessage.Scores[1] = this.mScores[1];
    iMessage.Scores[2] = this.mScores[2];
    iMessage.Scores[3] = this.mScores[3];
    iMessage.Scores[4] = this.mTeamScores[0];
    iMessage.Scores[5] = this.mTeamScores[1];
    iMessage.NrOfShortItems = (byte) 6;
    NetworkManager.Instance.Interface.SendMessage<RulesetMessage>(ref iMessage);
  }

  public override unsafe void NetworkUpdate(ref RulesetMessage iMsg)
  {
    if (iMsg.Byte01 == (byte) 0)
    {
      float num = NetworkManager.Instance.Interface.GetLatency(0) * 0.5f;
      this.mTimeLimitTarget = iMsg.Float01 - num;
    }
    else if (iMsg.Byte01 == (byte) 4)
    {
      fixed (short* numPtr = iMsg.Scores)
      {
        this.mScores[0] = numPtr[0];
        this.mScores[1] = numPtr[1];
        this.mScores[2] = numPtr[2];
        this.mScores[3] = numPtr[3];
        this.mTeamScores[0] = numPtr[4];
        this.mTeamScores[1] = numPtr[5];
      }
      if ((this.mPlayers[(int) iMsg.Byte02].Team & Factions.TEAM_RED) == Factions.TEAM_RED)
      {
        this.mScoreUIs[0].SetScore((int) this.mTeamScores[0]);
        if ((int) this.mTeamScores[0] < this.mScoreLimit)
          return;
        this.EndGame();
      }
      else if ((this.mPlayers[(int) iMsg.Byte02].Team & Factions.TEAM_BLUE) == Factions.TEAM_BLUE)
      {
        this.mScoreUIs[1].SetScore((int) this.mScores[1]);
        if ((int) this.mTeamScores[1] < this.mScoreLimit)
          return;
        this.EndGame();
      }
      else
      {
        int index = this.mIDToScoreUILookUp[(int) iMsg.Byte02];
        if (index != -1)
          this.mScoreUIs[index].SetScore((int) this.mScores[(int) iMsg.Byte02]);
        if ((int) this.mScores[(int) iMsg.Byte02] < this.mScoreLimit)
          return;
        this.EndGame();
      }
    }
    else
      base.NetworkUpdate(ref iMsg);
  }

  public override Rulesets RulesetType => Rulesets.DeathMatch;

  public override bool DropMagicks => true;

  public override bool CanRevive(Magicka.GameLogic.Player iReviver, Magicka.GameLogic.Player iRevivee)
  {
    return this.mTeams && (iReviver.Team & iRevivee.Team) != Factions.NONE;
  }

  internal override short[] GetScores() => this.mScores;

  internal override short[] GetTeamScores() => this.mTeamScores;

  internal override bool Teams => this.mTeams;

  internal new class Settings : VersusRuleset.Settings
  {
    private DropDownBox<int> mTimeLimit;
    private DropDownBox<int> mScoreLimit;
    private DropDownBox<bool> mTeams;
    private DropDownBox<int> mLuggageInterval;

    public Settings()
    {
      this.mTimeLimit = this.AddOption<int>(this.LOC_TIME_LIMIT, this.LOC_TT_TIME, new int[5]
      {
        0,
        5,
        10,
        30,
        50
      }, new int?[5]
      {
        new int?(this.LOC_UNLIMITED),
        new int?(),
        new int?(),
        new int?(),
        new int?()
      });
      this.mTimeLimit.SelectedIndex = 1;
      this.mScoreLimit = this.AddOption<int>(this.LOC_SCORE_LIMIT, this.LOC_TT_SCORE, new int[5]
      {
        int.MaxValue,
        5,
        10,
        20,
        50
      }, new int?[5]
      {
        new int?(this.LOC_UNLIMITED),
        new int?(),
        new int?(),
        new int?(),
        new int?()
      });
      this.mScoreLimit.SelectedIndex = 2;
      this.mTeams = this.AddOption<bool>(this.LOC_TEAMS, this.LOC_TT_TEAMS, new bool[2]
      {
        false,
        true
      }, new int?[2]
      {
        new int?(this.LOC_NO),
        new int?(this.LOC_YES)
      });
      this.mTeams.SelectedIndex = 0;
      this.mLuggageInterval = this.AddOption<int>(this.LOC_LUGGAGE_INTERVAL, this.LOC_TT_LUGGAGE, new int[4]
      {
        15,
        30,
        90,
        0
      }, new int?[4]
      {
        new int?(this.LOC_HIGH),
        new int?(this.LOC_MEDIUM),
        new int?(this.LOC_LOW),
        new int?(this.LOC_OFF)
      });
      this.mLuggageInterval.SelectedIndex = 1;
    }

    public int TimeLimit => this.mTimeLimit.SelectedValue;

    public int ScoreLimit => this.mScoreLimit.SelectedValue;

    public override bool TeamsEnabled => this.mTeams.SelectedValue;

    public int LuggageInterval => this.mLuggageInterval.SelectedValue;
  }
}
