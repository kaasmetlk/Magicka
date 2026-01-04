// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Versus.Krietor
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.Abilities.SpecialAbilities;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.GameLogic.Spells;
using Magicka.Misc;
using Magicka.Network;
using Magicka.WebTools.Paradox.Telemetry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

#nullable disable
namespace Magicka.Levels.Versus;

internal class Krietor : VersusRuleset
{
  private const float RESET_TIME = 2f;
  internal static readonly int LOC_UNLOCKED = "#magick_unlocked".GetHashCodeCustom();
  private short[] mTeamScores = new short[2];
  private short[] mScores = new short[4];
  private float mTimeLimitNetworkUpdate;
  private float mTimeLimitTarget;
  private float mTimeLimitTimer;
  private float mTimeLimit;
  private float mResetTimer;
  private bool mTeams;
  private bool mSuddenDeath;
  private List<KeyValuePair<float, MagickType>> mUnlockMagicks = new List<KeyValuePair<float, MagickType>>();
  private List<KeyValuePair<float, MagickType>> mTempUnlockMagicks;
  private Krietor.Settings mSettings;

  public Krietor(GameScene iScene, XmlNode iNode, Krietor.Settings iSettings)
    : base(iScene, iNode)
  {
    this.mSettings = iSettings;
    List<int> intList1 = new List<int>();
    List<int> intList2 = new List<int>();
    List<int> intList3 = new List<int>();
    for (int i1 = 0; i1 < iNode.ChildNodes.Count; ++i1)
    {
      XmlNode childNode1 = iNode.ChildNodes[i1];
      if (childNode1.Name.Equals("Areas", StringComparison.InvariantCultureIgnoreCase))
      {
        for (int i2 = 0; i2 < childNode1.ChildNodes.Count; ++i2)
        {
          XmlNode childNode2 = childNode1.ChildNodes[i2];
          if (childNode2.Name.Equals("area", StringComparison.InvariantCultureIgnoreCase))
          {
            bool flag1 = false;
            for (int i3 = 0; i3 < childNode2.Attributes.Count; ++i3)
            {
              XmlAttribute attribute = childNode2.Attributes[i3];
              if (attribute.Name.Equals("tourney", StringComparison.InvariantCultureIgnoreCase) && attribute.Value.Equals("true", StringComparison.InvariantCultureIgnoreCase))
              {
                flag1 = true;
                break;
              }
            }
            if (flag1)
            {
              bool flag2 = false;
              for (int i4 = 0; i4 < childNode2.Attributes.Count; ++i4)
              {
                XmlAttribute attribute = childNode2.Attributes[i4];
                if (attribute.Name.Equals("team", StringComparison.InvariantCultureIgnoreCase))
                {
                  if (attribute.Value.Equals("A", StringComparison.InvariantCultureIgnoreCase) || attribute.Value.Equals("red", StringComparison.InvariantCultureIgnoreCase))
                  {
                    flag2 = true;
                    intList2.Add(childNode2.InnerText.ToLowerInvariant().GetHashCodeCustom());
                    break;
                  }
                  if (attribute.Value.Equals("B", StringComparison.InvariantCultureIgnoreCase) || attribute.Value.Equals("blue", StringComparison.InvariantCultureIgnoreCase))
                  {
                    flag2 = true;
                    intList3.Add(childNode2.InnerText.ToLowerInvariant().GetHashCodeCustom());
                    break;
                  }
                }
              }
              if (!flag2)
                intList1.Add(childNode2.InnerText.ToLowerInvariant().GetHashCodeCustom());
            }
          }
        }
      }
      else if (childNode1.Name.Equals("Tournament", StringComparison.InvariantCultureIgnoreCase))
      {
        for (int i5 = 0; i5 < childNode1.ChildNodes.Count; ++i5)
        {
          XmlNode childNode3 = childNode1.ChildNodes[i5];
          if (childNode3.Name.Equals("magick", StringComparison.InvariantCultureIgnoreCase))
          {
            float key = 1f;
            MagickType magickType = (MagickType) Enum.Parse(typeof (MagickType), childNode3.InnerText, true);
            for (int i6 = 0; i6 < childNode3.Attributes.Count; ++i6)
            {
              XmlAttribute attribute = childNode3.Attributes[i6];
              if (attribute.Name.Equals("unlock", StringComparison.InvariantCultureIgnoreCase))
              {
                if (attribute.Value.Equals("start", StringComparison.InvariantCultureIgnoreCase))
                {
                  key = 0.0f;
                  break;
                }
                float result;
                if (float.TryParse(attribute.Value, NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out result))
                {
                  key = result;
                  break;
                }
                break;
              }
            }
            if ((double) key < 1.0)
              this.mUnlockMagicks.Add(new KeyValuePair<float, MagickType>(key, magickType));
          }
        }
      }
    }
    this.mUnlockMagicks.Sort((Comparison<KeyValuePair<float, MagickType>>) ((kvp1, kvp2) =>
    {
      if ((double) kvp1.Key > (double) kvp2.Key)
        return 1;
      return (double) kvp1.Key < (double) kvp2.Key ? -1 : 0;
    }));
    this.mAreas_All = intList1.ToArray();
    this.mAreas_TeamA = intList2.ToArray();
    this.mAreas_TeamB = intList3.ToArray();
    this.mTemporarySpawns = new List<int>(this.mAreas_All.Length);
  }

  public override void OnPlayerKill(Avatar atkAvatar, Avatar tarAvatar)
  {
  }

  public override void OnPlayerDeath(Magicka.GameLogic.Player iPlayer)
  {
    if (NetworkManager.Instance.State == NetworkState.Client || this.mScene.PlayState.IsGameEnded || (double) this.mResetTimer > 0.0)
      return;
    if ((iPlayer.Team & Factions.TEAM_RED) == Factions.TEAM_RED)
    {
      bool flag = true;
      for (int index = 0; index < this.mPlayers.Length; ++index)
      {
        if (this.mPlayers[index].Playing && (this.mPlayers[index].Team & Factions.TEAM_RED) == Factions.TEAM_RED && this.mPlayers[index].Avatar != null && !this.mPlayers[index].Avatar.Dead)
          flag = false;
      }
      if (!flag)
        return;
      ++this.mScores[1];
      ++this.mTeamScores[1];
      this.NetworkScore(1);
      this.mScoreUIs[1].SetScore((int) this.mTeamScores[1]);
      if (this.mSuddenDeath)
        this.EndGame();
      else
        this.mResetTimer = 2f;
    }
    else if ((iPlayer.Team & Factions.TEAM_BLUE) == Factions.TEAM_BLUE)
    {
      bool flag = true;
      for (int index = 0; index < this.mPlayers.Length; ++index)
      {
        if (this.mPlayers[index].Playing && (this.mPlayers[index].Team & Factions.TEAM_BLUE) == Factions.TEAM_BLUE && this.mPlayers[index].Avatar != null && !this.mPlayers[index].Avatar.Dead)
          flag = false;
      }
      if (!flag)
        return;
      ++this.mScores[0];
      ++this.mTeamScores[0];
      this.NetworkScore(0);
      this.mScoreUIs[0].SetScore((int) this.mTeamScores[0]);
      if (this.mSuddenDeath)
        this.EndGame();
      else
        this.mResetTimer = 2f;
    }
    else
    {
      int num = 0;
      int index1 = -1;
      for (int index2 = 0; index2 < this.mPlayers.Length; ++index2)
      {
        if (this.mPlayers[index2].Playing && index2 != iPlayer.ID && this.mPlayers[index2].Avatar != null && !this.mPlayers[index2].Avatar.Dead)
        {
          ++num;
          index1 = index2;
        }
      }
      if (num > 1)
        return;
      if (this.mSuddenDeath)
      {
        if (index1 > -1)
        {
          ++this.mScores[index1];
          int index3 = this.mIDToScoreUILookUp[index1];
          if (index3 != -1)
            this.mScoreUIs[index3].SetScore((int) this.mScores[index1]);
        }
        this.EndGame();
      }
      else
      {
        if (index1 > -1)
        {
          ++this.mScores[index1];
          int index4 = this.mIDToScoreUILookUp[index1];
          if (index4 != -1)
            this.mScoreUIs[index4].SetScore((int) this.mScores[index1]);
        }
        this.mResetTimer = 2f;
      }
      this.NetworkScore(index1);
    }
  }

  public override void OnEndGame()
  {
    Magicka.GameLogic.Player player = ControlManager.Instance.MenuController.Player;
    if ((player.Team & Factions.TEAM_RED) == Factions.TEAM_RED)
    {
      if ((int) this.mTeamScores[0] <= (int) this.mTeamScores[1])
        return;
      Singleton<PlayerSegmentManager>.Instance.NotifyWonAgainstHuman();
    }
    else if ((player.Team & Factions.TEAM_BLUE) == Factions.TEAM_BLUE)
    {
      if ((int) this.mTeamScores[1] <= (int) this.mTeamScores[0])
        return;
      Singleton<PlayerSegmentManager>.Instance.NotifyWonAgainstHuman();
    }
    else
    {
      if (!this.HaveBestScore(player.ID))
        return;
      Singleton<PlayerSegmentManager>.Instance.NotifyWonAgainstHuman();
    }
  }

  private bool HaveBestScore(int iPlayerID)
  {
    int index1 = 0;
    for (int index2 = 1; index2 < this.mScores.Length; ++index2)
    {
      if ((int) this.mScores[index2] > (int) this.mScores[index1])
        index1 = index2;
    }
    return index1 == iPlayerID;
  }

  private void Reset()
  {
    foreach (Liquid liquid in this.mScene.Level.CurrentScene.Liquids)
    {
      if (liquid.AutoFreeze)
        liquid.FreezeAll(2f);
      else
        liquid.FreezeAll(-2f);
    }
    if (NetworkManager.Instance.State != NetworkState.Client)
    {
      if (this.mSuddenDeath && !this.mTeams)
      {
        int num1 = int.MinValue;
        for (int index = 0; index < this.mScores.Length; ++index)
        {
          if ((int) this.mScores[index] > num1)
            num1 = (int) this.mScores[index];
        }
        this.mTemporarySpawns.Clear();
        this.mTemporarySpawns.AddRange((IEnumerable<int>) this.mAreas_All);
        int num2 = VersusRuleset.RANDOM.Next(this.mTemporarySpawns.Count);
        for (int iID = 0; iID < this.mScores.Length; ++iID)
        {
          if (this.mPlayers[iID].Playing)
          {
            if ((int) this.mScores[iID] == num1)
            {
              this.SetupPlayer(iID, this.mTemporarySpawns[num2 % this.mTemporarySpawns.Count]);
              this.mTemporarySpawns.RemoveAt(num2 % this.mTemporarySpawns.Count);
            }
            else if (this.mPlayers[iID].Avatar != null && !this.mPlayers[iID].Avatar.Dead)
              this.mPlayers[iID].Avatar.Terminate(true, false, true);
          }
        }
      }
      else if (this.mTeams)
      {
        this.mTemporarySpawns.Clear();
        this.mTemporarySpawns.AddRange((IEnumerable<int>) this.mAreas_TeamA);
        int num3 = VersusRuleset.RANDOM.Next(this.mTemporarySpawns.Count);
        for (int iID = 0; iID < this.mPlayers.Length; ++iID)
        {
          if (this.mPlayers[iID].Playing && (this.mPlayers[iID].Team & Factions.TEAM_RED) == Factions.TEAM_RED)
          {
            this.SetupPlayer(iID, this.mTemporarySpawns[num3 % this.mTemporarySpawns.Count]);
            this.mTemporarySpawns.RemoveAt(num3 % this.mTemporarySpawns.Count);
          }
        }
        this.mTemporarySpawns.Clear();
        this.mTemporarySpawns.AddRange((IEnumerable<int>) this.mAreas_TeamB);
        int num4 = VersusRuleset.RANDOM.Next(this.mTemporarySpawns.Count);
        for (int iID = 0; iID < this.mPlayers.Length; ++iID)
        {
          if (this.mPlayers[iID].Playing && (this.mPlayers[iID].Team & Factions.TEAM_BLUE) == Factions.TEAM_BLUE)
          {
            this.SetupPlayer(iID, this.mTemporarySpawns[num4 % this.mTemporarySpawns.Count]);
            this.mTemporarySpawns.RemoveAt(num4 % this.mTemporarySpawns.Count);
          }
        }
      }
      else
      {
        this.mTemporarySpawns.Clear();
        this.mTemporarySpawns.AddRange((IEnumerable<int>) this.mAreas_All);
        int num = VersusRuleset.RANDOM.Next(this.mTemporarySpawns.Count);
        for (int iID = 0; iID < this.mPlayers.Length; ++iID)
        {
          if (this.mPlayers[iID].Playing)
          {
            this.SetupPlayer(iID, this.mTemporarySpawns[num % this.mTemporarySpawns.Count]);
            this.mTemporarySpawns.RemoveAt(num % this.mTemporarySpawns.Count);
          }
        }
      }
    }
    Nullify.Instance.NullifyArea(this.mScene.PlayState, new Vector3(), true);
    this.CountDown(4f);
  }

  private void UnlockMagick(MagickType iMagick)
  {
    for (int index = 0; index < this.mPlayers.Length; ++index)
    {
      if (this.mPlayers[index].Playing)
        SpellManager.Instance.UnlockMagick(this.mPlayers[index], iMagick);
    }
  }

  public override void Initialize()
  {
    base.Initialize();
    this.mSuddenDeath = false;
    this.mTempUnlockMagicks = new List<KeyValuePair<float, MagickType>>((IEnumerable<KeyValuePair<float, MagickType>>) this.mUnlockMagicks);
    this.mTimeLimit = (float) this.mSettings.TimeLimit * 60f;
    this.mTimeLimitTimer = this.mTimeLimit;
    this.mTimeLimitTarget = this.mTimeLimit;
    this.mTeams = this.mSettings.TeamsEnabled;
    this.mResetTimer = 0.0f;
    for (int index = 0; index < this.mScores.Length; ++index)
      this.mScores[index] = (short) 0;
    for (int index = 0; index < this.mTeamScores.Length; ++index)
      this.mTeamScores[index] = (short) 0;
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
    for (int index = 0; index < this.mTempUnlockMagicks.Count; ++index)
    {
      if ((double) this.mTempUnlockMagicks[index].Key <= 0.0)
      {
        this.UnlockMagick(this.mTempUnlockMagicks[index].Value);
        this.mTempUnlockMagicks.RemoveAt(index--);
      }
    }
    if (this.mGameClockCue == null || this.mGameClockCue.IsStopping)
      return;
    this.mGameClockCue.Stop(AudioStopOptions.AsAuthored);
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
    if ((double) this.mTimeLimit > 0.0)
    {
      if (!this.mScene.PlayState.IsGameEnded && (double) this.mResetTimer <= 0.0 && (double) this.mCountDownTimer <= 0.0)
      {
        this.mTimeLimitTimer -= iDeltaTime;
        if ((double) this.mTimeLimitTimer <= 0.0)
        {
          int num1 = 0;
          int num2 = int.MinValue;
          for (int index = 0; index < this.mScores.Length; ++index)
          {
            if ((int) this.mScores[index] > num2)
            {
              num2 = (int) this.mScores[index];
              num1 = index;
            }
          }
          bool flag = false;
          for (int index = 0; index < this.mScores.Length; ++index)
          {
            if (index != num1 && (int) this.mScores[index] == num2)
            {
              flag = true;
              break;
            }
          }
          if (!flag)
          {
            this.EndGame();
            this.mTimeLimitTimer = 0.0f;
          }
          else
          {
            this.mSuddenDeath = true;
            this.mTimeLimit = 0.0f;
            this.mTimeLimitTimer = this.mTimeLimit;
            if (this.mGameClockCue != null && !this.mGameClockCue.IsStopping)
              this.mGameClockCue.Stop(AudioStopOptions.AsAuthored);
            this.Reset();
            if (NetworkManager.Instance.State == NetworkState.Server)
              NetworkManager.Instance.Interface.SendMessage<RulesetMessage>(ref new RulesetMessage()
              {
                Type = this.RulesetType,
                Byte01 = (byte) 2,
                Float01 = 0.0f
              });
          }
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
    if ((double) this.mResetTimer > 0.0)
    {
      this.mResetTimer -= iDeltaTime;
      if ((double) this.mResetTimer <= 0.0)
      {
        this.mResetTimer = 0.0f;
        this.Reset();
      }
    }
    VersusRuleset.RenderData renderData = this.mRenderData[(int) iDataChannel];
    for (int index = 0; index < this.mTempUnlockMagicks.Count; ++index)
    {
      if ((double) this.mTimeLimit - (double) this.mTimeLimitTimer > (double) this.mTempUnlockMagicks[index].Key * (double) this.mTimeLimit)
      {
        this.UnlockMagick(this.mTempUnlockMagicks[index].Value);
        this.mTempUnlockMagicks.RemoveAt(index--);
      }
    }
    if (this.mTempUnlockMagicks.Count > 0)
    {
      renderData.SetUnlockMagick(this.mTempUnlockMagicks[0].Value);
      float iTime = (float) ((double) this.mTempUnlockMagicks[0].Key * (double) this.mTimeLimit - ((double) this.mTimeLimit - (double) this.mTimeLimitTimer));
      if ((double) iTime < 10.0)
        renderData.SetUnlockMagickTime(iTime);
      else
        renderData.SetUnlockMagickTime(0.0f);
    }
    else
      renderData.SetUnlockMagick(MagickType.None);
    renderData.DrawTime = (double) this.mTimeLimit > 0.0;
    renderData.TimeLimit = this.mTimeLimit;
    renderData.Time = this.mTimeLimitTimer;
    renderData.SuddenDeath(this.mSuddenDeath);
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
    VersusRuleset.RenderData renderData = this.mRenderData[(int) iDataChannel];
    if ((double) this.mResetTimer > 0.0)
    {
      this.mResetTimer -= iDeltaTime;
      if ((double) this.mResetTimer <= 0.0)
      {
        this.mResetTimer = 0.0f;
        this.Reset();
      }
    }
    this.mTimeLimitTimer += (this.mTimeLimitTarget - this.mTimeLimitTimer) * iDeltaTime;
    for (int index = 0; index < this.mTempUnlockMagicks.Count; ++index)
    {
      if ((double) this.mTimeLimit - (double) this.mTimeLimitTimer > (double) this.mTempUnlockMagicks[index].Key * (double) this.mTimeLimit)
      {
        this.UnlockMagick(this.mTempUnlockMagicks[index].Value);
        this.mTempUnlockMagicks.RemoveAt(index--);
      }
    }
    if (this.mTempUnlockMagicks.Count > 0)
    {
      renderData.SetUnlockMagick(this.mTempUnlockMagicks[0].Value);
      float iTime = (float) ((double) this.mTempUnlockMagicks[0].Key * (double) this.mTimeLimit - ((double) this.mTimeLimit - (double) this.mTimeLimitTimer));
      if ((double) iTime < 10.0)
        renderData.SetUnlockMagickTime(iTime);
    }
    renderData.DrawTime = (double) this.mTimeLimit > 0.0;
    renderData.TimeLimit = this.mTimeLimit;
    renderData.Time = this.mTimeLimitTimer;
    renderData.SuddenDeath(this.mSuddenDeath);
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
      if (this.mTeams)
      {
        this.mScoreUIs[0].SetScore((int) this.mTeamScores[0]);
        this.mScoreUIs[1].SetScore((int) this.mTeamScores[1]);
        if (this.mSuddenDeath)
          return;
        this.mResetTimer = 2f - NetworkManager.Instance.Interface.GetLatency(0) * 0.5f;
      }
      else
      {
        int index = this.mIDToScoreUILookUp[(int) iMsg.Byte02];
        if (index != -1)
          this.mScoreUIs[index].SetScore((int) this.mScores[(int) iMsg.Byte02]);
        if (this.mSuddenDeath)
          return;
        this.mResetTimer = 2f - NetworkManager.Instance.Interface.GetLatency(0) * 0.5f;
      }
    }
    else if (iMsg.Byte01 == (byte) 2)
    {
      this.mSuddenDeath = true;
      this.mTimeLimit = iMsg.Float01;
      float num = NetworkManager.Instance.Interface.GetLatency(0) * 0.5f;
      this.mTimeLimitTimer = iMsg.Float01;
      this.mTimeLimitTarget = iMsg.Float01 - num;
      if (this.mGameClockCue != null && !this.mGameClockCue.IsStopping)
        this.mGameClockCue.Stop(AudioStopOptions.AsAuthored);
      this.Reset();
    }
    else
      base.NetworkUpdate(ref iMsg);
  }

  public override bool DropMagicks => false;

  public override Rulesets RulesetType => Rulesets.Kreitor;

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
    private DropDownBox<bool> mTeams;

    public Settings()
    {
      this.mTimeLimit = this.AddOption<int>(this.LOC_TIME_LIMIT, this.LOC_TT_TIME, new int[2]
      {
        5,
        10
      }, new int?[2]{ new int?(), new int?() });
      this.mTimeLimit.SelectedIndex = 1;
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
    }

    public int TimeLimit => this.mTimeLimit.SelectedValue;

    public override bool TeamsEnabled => this.mTeams.SelectedValue;
  }
}
