// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Versus.Brawl
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
using System.Xml;

#nullable disable
namespace Magicka.Levels.Versus;

internal class Brawl : VersusRuleset
{
  private short[] mTeamRespawns = new short[2];
  private short[] mRespawns = new short[4];
  private float[] mRespawnTimers = new float[4];
  private float mLuggageTimer;
  private float mRespawnTime;
  private float mLuggageTime;
  private bool mTeams;
  private float mTimeLimitTimer;
  private float mTimeLimitNetworkUpdate;
  private float mTimeLimitTarget;
  private float mTimeLimit;
  private int mMaxRespawns;
  private Brawl.Settings mSettings;

  public Brawl(GameScene iScene, XmlNode iNode, Brawl.Settings iSettings)
    : base(iScene, iNode)
  {
    this.mSettings = iSettings;
  }

  public override void OnPlayerKill(Avatar atkAvatar, Avatar tarAvatar)
  {
  }

  public override void OnPlayerDeath(Magicka.GameLogic.Player iPlayer)
  {
    if (NetworkManager.Instance.State == NetworkState.Client || this.mScene.PlayState.IsGameEnded)
      return;
    if ((iPlayer.Team & Factions.TEAM_RED) == Factions.TEAM_RED)
    {
      --this.mTeamRespawns[0];
      --this.mRespawns[iPlayer.ID];
      this.mScoreUIs[0].SetScore((int) this.mTeamRespawns[0]);
      this.NetworkScore(iPlayer.ID);
      if (this.mTeamRespawns[0] >= (short) 0)
      {
        this.mRespawnTimers[iPlayer.ID] = this.mRespawnTime;
      }
      else
      {
        bool flag = true;
        for (int index = 0; index < this.mPlayers.Length; ++index)
        {
          if (this.mPlayers[index].Playing && (this.mPlayers[index].Team & Factions.TEAM_RED) == Factions.TEAM_RED)
          {
            if (this.mPlayers[index].Avatar != null && !this.mPlayers[index].Avatar.Dead)
              flag = false;
            else if ((double) this.mRespawnTimers[this.mPlayers[index].ID] > 0.0)
              flag = false;
          }
        }
        if (!flag)
          return;
        this.EndGame();
      }
    }
    else if ((iPlayer.Team & Factions.TEAM_BLUE) == Factions.TEAM_BLUE)
    {
      --this.mTeamRespawns[1];
      --this.mRespawns[iPlayer.ID];
      this.mScoreUIs[1].SetScore((int) this.mTeamRespawns[1]);
      this.NetworkScore(iPlayer.ID);
      if (this.mTeamRespawns[1] >= (short) 0)
      {
        this.mRespawnTimers[iPlayer.ID] = this.mRespawnTime;
      }
      else
      {
        bool flag = true;
        for (int index = 0; index < this.mPlayers.Length; ++index)
        {
          if (this.mPlayers[index].Playing && (this.mPlayers[index].Team & Factions.TEAM_BLUE) == Factions.TEAM_BLUE)
          {
            if (this.mPlayers[index].Avatar != null && !this.mPlayers[index].Avatar.Dead)
              flag = false;
            else if ((double) this.mRespawnTimers[this.mPlayers[index].ID] > 0.0)
              flag = false;
          }
        }
        if (!flag)
          return;
        this.EndGame();
      }
    }
    else
    {
      --this.mRespawns[iPlayer.ID];
      int index1 = this.mIDToScoreUILookUp[iPlayer.ID];
      if (index1 != -1)
        this.mScoreUIs[index1].SetScore((int) this.mRespawns[iPlayer.ID]);
      this.NetworkScore(iPlayer.ID);
      if (this.mRespawns[iPlayer.ID] >= (short) 0)
      {
        this.mRespawnTimers[iPlayer.ID] = this.mRespawnTime;
      }
      else
      {
        int num = 0;
        for (int index2 = 0; index2 < this.mPlayers.Length; ++index2)
        {
          if (this.mPlayers[index2].Playing)
          {
            if (this.mPlayers[index2].Avatar != null && !this.mPlayers[index2].Avatar.Dead)
              ++num;
            else if ((double) this.mRespawnTimers[this.mPlayers[index2].ID] > 0.0)
              ++num;
          }
        }
        if (num > 1)
          return;
        this.EndGame();
      }
    }
  }

  public override void OnEndGame()
  {
    Magicka.GameLogic.Player player = ControlManager.Instance.MenuController.Player;
    if ((player.Team & Factions.TEAM_RED) == Factions.TEAM_RED)
    {
      if (this.IsTeamDead(Factions.TEAM_RED))
        return;
      Singleton<PlayerSegmentManager>.Instance.NotifyWonAgainstHuman();
    }
    else if ((player.Team & Factions.TEAM_BLUE) == Factions.TEAM_BLUE)
    {
      if (this.IsTeamDead(Factions.TEAM_BLUE))
        return;
      Singleton<PlayerSegmentManager>.Instance.NotifyWonAgainstHuman();
    }
    else
    {
      if ((player.Avatar == null || player.Avatar.Dead) && (double) this.mRespawnTimers[player.ID] <= 0.0)
        return;
      Singleton<PlayerSegmentManager>.Instance.NotifyWonAgainstHuman();
    }
  }

  private bool IsTeamDead(Factions iFaction)
  {
    bool flag = true;
    for (int index = 0; index < this.mPlayers.Length; ++index)
    {
      if (this.mPlayers[index].Playing && (this.mPlayers[index].Team & iFaction) == iFaction)
      {
        if (this.mPlayers[index].Avatar != null && !this.mPlayers[index].Avatar.Dead)
          flag = false;
        else if ((double) this.mRespawnTimers[this.mPlayers[index].ID] > 0.0)
          flag = false;
      }
    }
    return flag;
  }

  public override void Initialize()
  {
    base.Initialize();
    this.mMaxRespawns = this.mSettings.Lives;
    this.mLuggageTimer = (float) this.mSettings.LuggageInterval;
    this.mLuggageTime = this.mLuggageTimer;
    this.mRespawnTime = 5f;
    this.mTeams = this.mSettings.TeamsEnabled;
    this.mTimeLimit = (float) this.mSettings.TimeLimit * 60f;
    this.mTimeLimitTimer = this.mTimeLimit;
    this.mTimeLimitTarget = this.mTimeLimit;
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
    for (int index = 0; index < this.mRespawns.Length; ++index)
      this.mRespawns[index] = (short) (byte) this.mMaxRespawns;
    for (int index = 0; index < this.mTeamRespawns.Length; ++index)
      this.mTeamRespawns[index] = (short) (byte) this.mMaxRespawns;
    for (int index = 0; index < this.mScoreUIs.Count; ++index)
    {
      this.mScoreUIs[index].HideNegativeScore = true;
      this.mScoreUIs[index].SetScore(this.mMaxRespawns);
    }
  }

  public override void DeInitialize()
  {
    if (this.mGameClockCue == null || this.mGameClockCue.IsStopping)
      return;
    this.mGameClockCue.Stop(AudioStopOptions.AsAuthored);
  }

  public override void Update(float iDeltaTime, DataChannel iDataChannel)
  {
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
          this.mTimeLimitTimer = this.mTimeLimit;
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

  public override Rulesets RulesetType => Rulesets.Brawl;

  public override bool DropMagicks => true;

  public override bool CanRevive(Magicka.GameLogic.Player iReviver, Magicka.GameLogic.Player iRevivee)
  {
    if (!this.mTeams || (iReviver.Team & iRevivee.Team) == Factions.NONE)
      return false;
    return (iReviver.Team & Factions.TEAM_RED) == Factions.TEAM_RED ? this.mRespawns[0] > (short) 0 : this.mRespawns[1] > (short) 0;
  }

  protected unsafe void NetworkScore(int iID)
  {
    if (NetworkManager.Instance.State != NetworkState.Server)
      return;
    RulesetMessage iMessage = new RulesetMessage();
    iMessage.Type = this.RulesetType;
    iMessage.Byte01 = (byte) 4;
    iMessage.Byte02 = (byte) iID;
    iMessage.Scores[0] = this.mRespawns[0];
    iMessage.Scores[1] = this.mRespawns[1];
    iMessage.Scores[2] = this.mRespawns[2];
    iMessage.Scores[3] = this.mRespawns[3];
    iMessage.Scores[4] = this.mTeamRespawns[0];
    iMessage.Scores[5] = this.mTeamRespawns[1];
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
        this.mRespawns[0] = numPtr[0];
        this.mRespawns[1] = numPtr[1];
        this.mRespawns[2] = numPtr[2];
        this.mRespawns[3] = numPtr[3];
        this.mTeamRespawns[0] = numPtr[4];
        this.mTeamRespawns[1] = numPtr[5];
      }
      if ((this.mPlayers[(int) iMsg.Byte02].Team & Factions.TEAM_RED) == Factions.TEAM_RED)
      {
        if (this.mTeamRespawns[0] <= (short) 0)
          return;
        this.mRespawnTimers[(int) iMsg.Byte02] = this.mRespawnTime;
        this.mScoreUIs[0].SetScore((int) this.mTeamRespawns[0]);
      }
      else if ((this.mPlayers[(int) iMsg.Byte02].Team & Factions.TEAM_BLUE) == Factions.TEAM_BLUE)
      {
        if (this.mTeamRespawns[1] <= (short) 0)
          return;
        this.mRespawnTimers[(int) iMsg.Byte02] = this.mRespawnTime;
        this.mScoreUIs[1].SetScore((int) this.mTeamRespawns[1]);
      }
      else
      {
        if (this.mRespawns[(int) iMsg.Byte02] >= (short) 0)
          this.mRespawnTimers[(int) iMsg.Byte02] = this.mRespawnTime;
        int index = this.mIDToScoreUILookUp[(int) iMsg.Byte02];
        if (index == -1)
          return;
        this.mScoreUIs[index].SetScore((int) this.mRespawns[(int) iMsg.Byte02]);
      }
    }
    else
      base.NetworkUpdate(ref iMsg);
  }

  internal override short[] GetScores() => this.mRespawns;

  internal override short[] GetTeamScores() => this.mTeamRespawns;

  internal override bool Teams => this.mTeams;

  internal new class Settings : VersusRuleset.Settings
  {
    private DropDownBox<int> mTimeLimit;
    private DropDownBox<int> mLives;
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
      this.mLives = this.AddOption<int>(this.LOC_LIVES, this.LOC_TT_LIVES, new int[5]
      {
        1,
        5,
        10,
        20,
        50
      }, (int?[]) null);
      this.mLives.SelectedIndex = 1;
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

    public int Lives => this.mLives.SelectedValue;

    public override bool TeamsEnabled => this.mTeams.SelectedValue;

    public int LuggageInterval => this.mLuggageInterval.SelectedValue;
  }
}
