// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.GameStates.InGameMenus.InGameMenuTimedObjectiveStatistics
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Achievements;
using Magicka.DRM;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.GameLogic.GameStates.Menu.Main;
using Magicka.GameLogic.Statistics;
using Magicka.GameLogic.UI;
using Magicka.Graphics;
using Magicka.Levels;
using Magicka.Levels.Campaign;
using Magicka.Localization;
using Magicka.Network;
using Microsoft.Xna.Framework;
using PolygonHead;
using System;
using System.Collections.Generic;
using System.IO;

#nullable disable
namespace Magicka.GameLogic.GameStates.InGameMenus;

internal class InGameMenuTimedObjectiveStatistics : InGameMenu
{
  private const string OPTION_RETRY = "retry";
  private const string OPTION_QUIT = "quit";
  private const string OPTION_DISCONNECT = "disconnect";
  private const string OPTION_LOBBY = "lobby";
  private const string OPTION_READY = "ready";
  private static readonly string[] OPTION_STRINGS = new string[3]
  {
    "retry",
    "quit",
    "lobby"
  };
  private static InGameMenuTimedObjectiveStatistics sSingelton;
  private static volatile object sSingeltonLock = new object();
  private static readonly int LOC_RETRY = "#add_menu_retry".GetHashCodeCustom();
  private static readonly int LOC_READY = "#menu_charslct_02".GetHashCodeCustom();
  private static readonly int LOC_DISCONNECT = "#network_10".GetHashCodeCustom();
  private static readonly int LOC_QUIT = "#add_menu_quit".GetHashCodeCustom();
  private static readonly int LOC_LOBBY = "#menu_lobby".GetHashCodeCustom();
  private static readonly int LOC_TOTAL_SCORE = "#challenge_total_score".GetHashCodeCustom();
  private static readonly int LOC_MISSION_COMPLETE = "#menu_mission_complete".GetHashCodeCustom();
  private static readonly int LOC_MISSION_FAILED = "#menu_mission_failed".GetHashCodeCustom();
  private static readonly int LOC_MISSION_TIME = "#menu_mission_time".GetHashCodeCustom();
  private static readonly int LOC_TIME_BONUS = "#menu_mission_time".GetHashCodeCustom();
  private static readonly int LOC_TSCORE = "#challenge_total_score".GetHashCodeCustom();
  private List<Text> mObjectiveNames = new List<Text>();
  private List<Text> mObjectiveValues = new List<Text>();
  private List<Text> mObjectiveScores = new List<Text>();
  private float mFontHeight;
  private Text mMissionComplete;
  private Text mMissionTimeText;
  private Text mTimeText;
  private Text mTotalScoreText;
  private Text mScoreText;
  private Text mMissionTimeBonusText;
  private Text mBonusText;
  private BitmapFont mItemFont = FontManager.Instance.GetFont(MagickaFont.Maiandra18);

  public static InGameMenuTimedObjectiveStatistics Instance
  {
    get
    {
      if (InGameMenuTimedObjectiveStatistics.sSingelton == null)
      {
        lock (InGameMenuTimedObjectiveStatistics.sSingeltonLock)
        {
          if (InGameMenuTimedObjectiveStatistics.sSingelton == null)
            InGameMenuTimedObjectiveStatistics.sSingelton = new InGameMenuTimedObjectiveStatistics();
        }
      }
      return InGameMenuTimedObjectiveStatistics.sSingelton;
    }
  }

  private InGameMenuTimedObjectiveStatistics()
  {
    this.mBackgroundSize = new Vector2(600f, 600f);
    this.AddMenuTextItem(InGameMenuTimedObjectiveStatistics.LOC_RETRY, this.mItemFont, TextAlign.Center);
    this.AddMenuTextItem(InGameMenuTimedObjectiveStatistics.LOC_DISCONNECT, this.mItemFont, TextAlign.Center);
    this.AddMenuTextItem(InGameMenuTimedObjectiveStatistics.LOC_QUIT, this.mItemFont, TextAlign.Center);
    this.mMissionComplete = new Text(50, FontManager.Instance.GetFont(MagickaFont.MenuOption), TextAlign.Center, false);
    this.mTotalScoreText = new Text(50, FontManager.Instance.GetFont(MagickaFont.Maiandra18), TextAlign.Center, false);
    this.mScoreText = new Text(50, FontManager.Instance.GetFont(MagickaFont.MenuOption), TextAlign.Center, false);
    this.mMissionTimeText = new Text(50, FontManager.Instance.GetFont(MagickaFont.Maiandra18), TextAlign.Center, false);
    this.mTimeText = new Text(50, FontManager.Instance.GetFont(MagickaFont.MenuOption), TextAlign.Center, false);
    this.mMissionTimeBonusText = new Text(50, FontManager.Instance.GetFont(MagickaFont.Maiandra18), TextAlign.Left, false);
    this.mBonusText = new Text(50, FontManager.Instance.GetFont(MagickaFont.Maiandra18), TextAlign.Right, false);
    this.mFontHeight = (float) this.mTimeText.Font.LineHeight;
  }

  protected override void IUpdate(DataChannel iDataChannel, float iDeltaTime)
  {
    base.IUpdate(iDataChannel, iDeltaTime);
    if (!(NetworkManager.Instance.Interface is NetworkServer networkServer) || !networkServer.AllClientsReady)
      return;
    this.mMenuItems[0].Enabled = true;
    this.mMenuItems[2].Enabled = true;
  }

  protected override void IDraw(float iDeltaTime, ref Vector2 iBackgroundSize)
  {
    base.IDraw(iDeltaTime, ref iBackgroundSize);
    Vector4 color = InGameMenu.sEffect.Color;
    InGameMenu.sEffect.Color = Vector4.One;
    Vector2 vector2;
    vector2.X = InGameMenu.sScreenSize.X * 0.5f;
    vector2.Y = (float) ((double) InGameMenu.sScreenSize.Y * 0.5 - (double) this.mBackgroundSize.Y * 0.5 + (double) this.mMissionComplete.Font.LineHeight * 2.0);
    this.mMissionComplete.Draw(InGameMenu.sEffect, vector2.X, vector2.Y);
    vector2.Y += (float) this.mMissionComplete.Font.LineHeight;
    this.mTotalScoreText.Draw(InGameMenu.sEffect, vector2.X, vector2.Y);
    vector2.Y += (float) this.mTotalScoreText.Font.LineHeight;
    this.mScoreText.Draw(InGameMenu.sEffect, vector2.X, vector2.Y);
    vector2.Y += (float) this.mScoreText.Font.LineHeight;
    this.mMissionTimeText.Draw(InGameMenu.sEffect, vector2.X, vector2.Y);
    vector2.Y += (float) this.mMissionTimeText.Font.LineHeight;
    this.mTimeText.Draw(InGameMenu.sEffect, vector2.X, vector2.Y);
    vector2.Y += 1.5f * (float) this.mTimeText.Font.LineHeight;
    for (int index = 0; index < this.mObjectiveNames.Count; ++index)
    {
      this.mObjectiveNames[index].Draw(InGameMenu.sEffect, (float) ((double) vector2.X - (double) this.mBackgroundSize.X * 0.5 + 48.0), vector2.Y);
      this.mObjectiveValues[index].Draw(InGameMenu.sEffect, vector2.X + 128f, vector2.Y);
      this.mObjectiveScores[index].Draw(InGameMenu.sEffect, (float) ((double) vector2.X + (double) this.mBackgroundSize.X * 0.5 - 48.0), vector2.Y);
      vector2.Y += (float) this.mObjectiveNames[index].Font.LineHeight;
    }
    vector2.Y += (float) this.mMissionTimeText.Font.LineHeight;
    this.mMissionTimeBonusText.Draw(InGameMenu.sEffect, (float) ((double) vector2.X - (double) this.mBackgroundSize.X * 0.5 + 48.0), vector2.Y);
    this.mBonusText.Draw(InGameMenu.sEffect, (float) ((double) vector2.X + (double) this.mBackgroundSize.X * 0.5 - 48.0), vector2.Y);
    vector2.Y += (float) this.mBonusText.Font.LineHeight * 2f;
    InGameMenu.sEffect.Color = color;
    NetworkState state = NetworkManager.Instance.State;
    for (int index = 0; index < this.mMenuItems.Count; ++index)
    {
      if ((state != NetworkState.Client || index != 0) && (state != NetworkState.Offline || index != 1))
      {
        MenuItem mMenuItem = this.mMenuItems[index];
        mMenuItem.Position = vector2;
        mMenuItem.Draw(InGameMenu.sEffect);
        vector2.Y += mMenuItem.BottomRight.Y - mMenuItem.TopLeft.Y;
      }
    }
  }

  protected override void IControllerSelect(Controller iSender)
  {
    if (NetworkManager.Instance.State != NetworkState.Client)
    {
      NetworkServer networkServer = NetworkManager.Instance.Interface as NetworkServer;
      if (this.mSelectedItem == 0 && (networkServer == null || networkServer.AllClientsReady))
      {
        InGameMenu.sPlayState.Restart((object) this, RestartType.StartOfLevel);
        networkServer?.SetAllClientsBusy();
      }
      else if (this.mSelectedItem == 1)
      {
        GameEndMessage iMsg;
        iMsg.Condition = EndGameCondition.ChallengeExit;
        iMsg.Argument = 1;
        iMsg.DelayTime = 0.0f;
        iMsg.Phony = false;
        if (networkServer != null)
        {
          NetworkManager.Instance.EndSession();
          while (!(Tome.Instance.CurrentMenu is SubMenuMain))
            Tome.Instance.PopMenuInstant();
        }
        InGameMenu.sPlayState.Endgame(ref iMsg);
      }
      else
      {
        if (this.mSelectedItem != 2 || networkServer != null && !networkServer.AllClientsReady)
          return;
        GameEndMessage gameEndMessage;
        gameEndMessage.Condition = EndGameCondition.ChallengeExit;
        gameEndMessage.Argument = 1;
        gameEndMessage.DelayTime = 0.0f;
        gameEndMessage.Phony = false;
        InGameMenu.sPlayState.Endgame(ref gameEndMessage);
        if (networkServer == null)
          return;
        networkServer.SetAllClientsBusy();
        networkServer.SendMessage<GameEndMessage>(ref gameEndMessage);
      }
    }
    else if (this.mSelectedItem == 1)
    {
      GameEndMessage iMsg;
      iMsg.Condition = EndGameCondition.ChallengeExit;
      iMsg.Argument = 1;
      iMsg.DelayTime = 0.0f;
      iMsg.Phony = false;
      NetworkManager.Instance.EndSession();
      while (!(Tome.Instance.CurrentMenu is SubMenuMain))
        Tome.Instance.PopMenuInstant();
      InGameMenu.sPlayState.Endgame(ref iMsg);
    }
    else
    {
      if (this.mSelectedItem != 2)
        return;
      GameEndLoadMessage iMessage = new GameEndLoadMessage();
      NetworkManager.Instance.Interface.SendMessage<GameEndLoadMessage>(ref iMessage, 0);
      this.mMenuItems[1].Enabled = false;
    }
  }

  protected override void IControllerBack(Controller iSender)
  {
  }

  protected override string IGetHighlightedButtonName()
  {
    string str = InGameMenuTimedObjectiveStatistics.OPTION_STRINGS[this.mSelectedItem];
    if (NetworkManager.Instance.State != NetworkState.Client)
    {
      if (this.mSelectedItem == 1)
        str = "disconnect";
      else if (this.mSelectedItem == 2)
        str = "ready";
    }
    return str;
  }

  protected override void OnEnter()
  {
    TimedObjectiveRuleset ruleSet = InGameMenu.sPlayState.Level.CurrentScene.RuleSet as TimedObjectiveRuleset;
    float timerValue = InGameMenu.sPlayState.Level.GetTimerValue(ruleSet.TimerID);
    TimeSpan timeSpan = TimeSpan.FromSeconds((double) timerValue);
    if ((double) timerValue >= 60.0 && (double) timerValue < 3600.0)
      this.mTimeText.SetText($"0:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}");
    else if ((double) timerValue >= 3600.0)
      this.mTimeText.SetText($"{timeSpan.Hours:0}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}");
    else
      this.mTimeText.SetText($"0:00:{timeSpan.Seconds:00}");
    if (ruleSet.TimeSuccess)
      this.mMissionComplete.SetText(LanguageManager.Instance.GetString(InGameMenuTimedObjectiveStatistics.LOC_MISSION_COMPLETE));
    else
      this.mMissionComplete.SetText(LanguageManager.Instance.GetString(InGameMenuTimedObjectiveStatistics.LOC_MISSION_FAILED));
    this.mMissionTimeText.SetText(LanguageManager.Instance.GetString(InGameMenuTimedObjectiveStatistics.LOC_MISSION_TIME));
    List<TimedObjectiveRuleset.Objective> objectives = ruleSet.Objectives;
    this.mObjectiveNames.Clear();
    this.mObjectiveScores.Clear();
    this.mObjectiveValues.Clear();
    bool flag = true;
    int num1 = 0;
    BitmapFont font = FontManager.Instance.GetFont(MagickaFont.Maiandra18);
    for (int index = 0; index < objectives.Count; ++index)
    {
      string iText = LanguageManager.Instance.GetString(objectives[index].NameID);
      int counterValue = InGameMenu.sPlayState.Level.GetCounterValue(objectives[index].CounterID);
      int num2 = counterValue * objectives[index].Score;
      num1 += num2;
      Text text1 = new Text(100, font, TextAlign.Left, false);
      text1.SetText(iText);
      this.mObjectiveNames.Add(text1);
      Text text2 = new Text(100, font, TextAlign.Right, false);
      if (objectives[index].MaxValue > 0)
        text2.SetText($"{counterValue}/{objectives[index].MaxValue}");
      else
        text2.SetText("");
      this.mObjectiveValues.Add(text2);
      Text text3 = new Text(100, font, TextAlign.Right, false);
      text3.SetText(num2.ToString());
      this.mObjectiveScores.Add(text3);
      if (counterValue < objectives[index].MaxValue)
        flag = false;
    }
    if (flag)
      AchievementsManager.Instance.AwardAchievement(InGameMenu.sPlayState, "goodcompany");
    int num3 = (int) ((double) num1 * (double) ruleSet.GetBonusMultiplier);
    int num4 = num1 + num3;
    this.mScoreText.SetText(num4.ToString());
    this.mTotalScoreText.SetText(LanguageManager.Instance.GetString(InGameMenuTimedObjectiveStatistics.LOC_TSCORE));
    this.mMissionTimeBonusText.SetText(LanguageManager.Instance.GetString(InGameMenuTimedObjectiveStatistics.LOC_TIME_BONUS));
    this.mBonusText.SetText(num3.ToString());
    LeaderBoardData data = new LeaderBoardData();
    data.Score = num4;
    FloatIntConverter floatIntConverter = new FloatIntConverter(timerValue);
    data.Data1 = floatIntConverter.Int;
    Player[] players = Magicka.Game.Instance.Players;
    string str = "";
    int index1 = 0;
    int num5 = 0;
    for (; index1 < players.Length; ++index1)
    {
      if (players[index1].Playing)
      {
        str += players[index1].GamerTag;
        ++num5;
        if (num5 < Magicka.Game.Instance.PlayerCount)
          str += ", ";
        else
          break;
      }
    }
    data.Name = str;
    string lvl = InGameMenu.sPlayState.Level.Name;
    Magicka.Game.Instance.AddLoadTask((Action) (() =>
    {
      LevelNode[] challenges = LevelManager.Instance.Challenges;
      for (int iChallengeIndex = 0; iChallengeIndex < challenges.Length; ++iChallengeIndex)
      {
        if (lvl.Equals(Path.GetFileNameWithoutExtension(challenges[iChallengeIndex].FileName), StringComparison.InvariantCultureIgnoreCase))
        {
          StatisticsManager.Instance.AddLocalEntry(iChallengeIndex, data);
          if (HackHelper.LicenseStatus != HackHelper.Status.Valid || HackHelper.CheckLicense(challenges[iChallengeIndex]) != HackHelper.License.Yes)
            break;
          StatisticsManager.Instance.AddOnlineEntry(iChallengeIndex, data);
          break;
        }
      }
    }));
    if (NetworkManager.Instance.State == NetworkState.Client)
    {
      this.mMenuItems[0].Enabled = false;
      this.mMenuItems[1].Enabled = true;
      this.mMenuItems[2].Enabled = true;
      (this.mMenuItems[2] as MenuTextItem).SetText(InGameMenuTimedObjectiveStatistics.LOC_READY);
    }
    else if (NetworkManager.Instance.State == NetworkState.Server)
    {
      this.mMenuItems[0].Enabled = false;
      this.mMenuItems[1].Enabled = true;
      this.mMenuItems[2].Enabled = false;
      (this.mMenuItems[2] as MenuTextItem).SetText(InGameMenuTimedObjectiveStatistics.LOC_LOBBY);
    }
    else
    {
      this.mMenuItems[0].Enabled = true;
      this.mMenuItems[1].Enabled = false;
      this.mMenuItems[2].Enabled = true;
      (this.mMenuItems[2] as MenuTextItem).SetText(InGameMenuTimedObjectiveStatistics.LOC_QUIT);
    }
  }

  protected override void OnExit()
  {
  }
}
