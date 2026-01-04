// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.GameStates.InGameMenus.InGameMenuSurvivalStatistics
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

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
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using System;
using System.IO;

#nullable disable
namespace Magicka.GameLogic.GameStates.InGameMenus;

internal class InGameMenuSurvivalStatistics : InGameMenu
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
  private static InGameMenuSurvivalStatistics sSingelton;
  private static volatile object sSingeltonLock = new object();
  private static readonly int LOC_RETRY = "#add_menu_retry".GetHashCodeCustom();
  private static readonly int LOC_READY = "#menu_charslct_02".GetHashCodeCustom();
  private static readonly int LOC_DISCONNECT = "#network_10".GetHashCodeCustom();
  private static readonly int LOC_QUIT = "#add_menu_quit".GetHashCodeCustom();
  private static readonly int LOC_LOBBY = "#menu_lobby".GetHashCodeCustom();
  private static readonly int LOC_SCORE = "#lb_score".GetHashCodeCustom();
  private Texture2D mPagesTexture;
  private Text mScore;
  private Text[] mPlayers;
  private Text[] mKills;
  private Text[] mDeaths;
  private Text mScoreTag;
  private int mScoreCount;
  private int mTargetScore;
  private float mDelay;

  public static InGameMenuSurvivalStatistics Instance
  {
    get
    {
      if (InGameMenuSurvivalStatistics.sSingelton == null)
      {
        lock (InGameMenuSurvivalStatistics.sSingeltonLock)
        {
          if (InGameMenuSurvivalStatistics.sSingelton == null)
            InGameMenuSurvivalStatistics.sSingelton = new InGameMenuSurvivalStatistics();
        }
      }
      return InGameMenuSurvivalStatistics.sSingelton;
    }
  }

  private InGameMenuSurvivalStatistics()
  {
    this.mBackgroundSize = new Vector2(600f, 600f);
    BitmapFont font1 = FontManager.Instance.GetFont(MagickaFont.Maiandra18);
    BitmapFont font2 = FontManager.Instance.GetFont(MagickaFont.MenuTitle);
    this.AddMenuTextItem(InGameMenuSurvivalStatistics.LOC_RETRY, font1, TextAlign.Center);
    this.AddMenuTextItem(InGameMenuSurvivalStatistics.LOC_DISCONNECT, font1, TextAlign.Center);
    this.AddMenuTextItem(InGameMenuSurvivalStatistics.LOC_QUIT, font1, TextAlign.Center);
    this.mScoreTag = new Text(64 /*0x40*/, font1, TextAlign.Center, false);
    this.mScore = new Text(64 /*0x40*/, font2, TextAlign.Center, false);
    this.mPlayers = new Text[4];
    this.mKills = new Text[4];
    this.mDeaths = new Text[4];
    for (int index = 0; index < 4; ++index)
    {
      this.mPlayers[index] = new Text(64 /*0x40*/, font1, TextAlign.Left, false);
      this.mKills[index] = new Text(64 /*0x40*/, font1, TextAlign.Center, false);
      this.mDeaths[index] = new Text(64 /*0x40*/, font1, TextAlign.Center, false);
    }
    lock (Magicka.Game.Instance.GraphicsDevice)
      this.mPagesTexture = Magicka.Game.Instance.Content.Load<Texture2D>("UI/Menu/Pages");
    this.LanguageChanged();
  }

  public override void LanguageChanged()
  {
    base.LanguageChanged();
    this.mScoreTag.SetText(LanguageManager.Instance.GetString(InGameMenu.sPlayState.Level.DisplayName));
  }

  protected override string IGetHighlightedButtonName()
  {
    string str = InGameMenuSurvivalStatistics.OPTION_STRINGS[this.mSelectedItem];
    if (NetworkManager.Instance.State != NetworkState.Client)
    {
      if (this.mSelectedItem == 1)
        str = "disconnect";
      else if (this.mSelectedItem == 2)
        str = "ready";
    }
    return str;
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

  protected override void OnEnter()
  {
    if (InGameMenu.sPlayState.Level.CurrentScene.RuleSet is SurvivalRuleset)
    {
      Player[] players = Magicka.Game.Instance.Players;
      for (int iPlayer = 0; iPlayer < 4; ++iPlayer)
      {
        if (players[iPlayer].Playing)
        {
          StatisticsManager.ScoreValues score = StatisticsManager.Instance.GetScore(iPlayer);
          this.mPlayers[iPlayer].SetText(players[iPlayer].GamerTag);
          this.mKills[iPlayer].SetText(score.Kills.ToString());
          this.mDeaths[iPlayer].SetText(score.Deaths.ToString());
        }
        else
        {
          this.mPlayers[iPlayer].SetText("");
          this.mKills[iPlayer].SetText("");
          this.mDeaths[iPlayer].SetText("");
        }
      }
      this.LanguageChanged();
      this.mTargetScore = StatisticsManager.Instance.SurvivalTotalScore;
      this.mScoreCount = 0;
      this.mScore.SetText(this.mScoreCount.ToString());
      this.mDelay = 0.0f;
      this.AddHighScore();
    }
    if (NetworkManager.Instance.State == NetworkState.Client)
    {
      this.mMenuItems[0].Enabled = false;
      this.mMenuItems[1].Enabled = true;
      this.mMenuItems[2].Enabled = true;
      (this.mMenuItems[2] as MenuTextItem).SetText(InGameMenuSurvivalStatistics.LOC_READY);
    }
    else if (NetworkManager.Instance.State == NetworkState.Server)
    {
      this.mMenuItems[0].Enabled = false;
      this.mMenuItems[1].Enabled = true;
      this.mMenuItems[2].Enabled = false;
      (this.mMenuItems[2] as MenuTextItem).SetText(InGameMenuSurvivalStatistics.LOC_LOBBY);
    }
    else
    {
      this.mMenuItems[0].Enabled = true;
      this.mMenuItems[1].Enabled = false;
      this.mMenuItems[2].Enabled = true;
      (this.mMenuItems[2] as MenuTextItem).SetText(InGameMenuSurvivalStatistics.LOC_QUIT);
    }
  }

  private void AddHighScore()
  {
    if (!(InGameMenu.sPlayState.Level.CurrentScene.RuleSet is SurvivalRuleset ruleSet))
      return;
    LeaderBoardData data = new LeaderBoardData();
    data.Score = StatisticsManager.Instance.SurvivalTotalScore;
    data.Data1 = (int) (byte) ruleSet.WaveIndex;
    if (data.Score == 0)
      return;
    Player[] players = Magicka.Game.Instance.Players;
    string str = "";
    int index = 0;
    int num = 0;
    for (; index < players.Length; ++index)
    {
      if (players[index].Playing)
      {
        str += players[index].GamerTag;
        ++num;
        if (num < Magicka.Game.Instance.PlayerCount)
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
    this.mDelay += iDeltaTime;
    if (this.mScoreCount < this.mTargetScore)
    {
      this.mScoreCount = (int) ((double) this.mTargetScore * ((double) this.mDelay / 1.0));
      if (this.mScoreCount > this.mTargetScore)
        this.mScoreCount = this.mTargetScore;
      this.mScore.SetText(this.mScoreCount.ToString());
    }
    else if (this.mScoreCount > this.mTargetScore)
    {
      this.mScoreCount = this.mTargetScore;
      this.mScore.SetText(this.mScoreCount.ToString());
    }
    Vector4 vector4_1 = new Vector4();
    vector4_1.X = vector4_1.Y = vector4_1.Z = 1f;
    vector4_1.W = this.mAlpha;
    Vector4 vector4_2 = new Vector4();
    vector4_2.X = vector4_2.Y = vector4_2.Z = 0.0f;
    vector4_2.W = this.mAlpha;
    Vector4 vector4_3 = new Vector4();
    vector4_3.X = vector4_3.Y = vector4_3.Z = 0.4f;
    vector4_3.W = this.mAlpha;
    for (int index = 0; index < this.mMenuItems.Count; ++index)
    {
      MenuItem mMenuItem = this.mMenuItems[index];
      mMenuItem.Color = vector4_1;
      mMenuItem.ColorSelected = vector4_2;
      mMenuItem.ColorDisabled = vector4_3;
      mMenuItem.Selected = mMenuItem.Enabled & this.mSelectedItem == index;
      if (mMenuItem.Selected)
      {
        InGameMenu.sEffect.Transform = new Matrix()
        {
          M44 = 1f,
          M11 = 200f * InGameMenu.sScale,
          M22 = mMenuItem.BottomRight.Y - mMenuItem.TopLeft.Y,
          M41 = mMenuItem.Position.X - 100f * InGameMenu.sScale,
          M42 = mMenuItem.TopLeft.Y
        };
        Vector4 vector4_4 = new Vector4();
        vector4_4.X = vector4_4.Y = vector4_4.Z = 1f;
        vector4_4.W = 0.8f * this.mAlpha;
        InGameMenu.sEffect.Color = vector4_4;
        InGameMenu.sEffect.GraphicsDevice.Vertices[0].SetSource(InGameMenu.sBackground, 0, 8);
        InGameMenu.sEffect.GraphicsDevice.VertexDeclaration = InGameMenu.sBackgroundDeclaration;
        InGameMenu.sEffect.CommitChanges();
        InGameMenu.sEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
      }
    }
    Vector2 vector2 = new Vector2();
    vector2.Y = (float) ((double) InGameMenu.sScreenSize.Y * 0.5 - 220.0 * (double) InGameMenu.sScale);
    vector2.X = InGameMenu.sScreenSize.X * 0.5f;
    InGameMenu.sEffect.Color = Vector4.One;
    this.mScoreTag.Draw(InGameMenu.sEffect, vector2.X, vector2.Y, InGameMenu.sScale);
    vector2.Y += 20f * InGameMenu.sScale;
    this.mScore.Draw(InGameMenu.sEffect, vector2.X, vector2.Y, InGameMenu.sScale);
    vector2.X = (float) ((double) InGameMenu.sScreenSize.X * 0.5 + 74.0 * (double) InGameMenu.sScale);
    vector2.Y += 80f * InGameMenu.sScale;
    this.DrawGraphics(this.mPagesTexture, new Rectangle(1024 /*0x0400*/, 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/), new Vector4(vector2.X, vector2.Y, 32f * InGameMenu.sScale, 32f * InGameMenu.sScale));
    vector2.X = (float) ((double) InGameMenu.sScreenSize.X * 0.5 + 174.0 * (double) InGameMenu.sScale);
    this.DrawGraphics(this.mPagesTexture, new Rectangle(1024 /*0x0400*/, 0, 64 /*0x40*/, 64 /*0x40*/), new Vector4(vector2.X, vector2.Y, 32f * InGameMenu.sScale, 32f * InGameMenu.sScale));
    vector2.X = (float) ((double) InGameMenu.sScreenSize.X * 0.5 - 210.0 * (double) InGameMenu.sScale);
    vector2.Y += 48f * InGameMenu.sScale;
    for (int index = 0; index < 4; ++index)
    {
      this.mPlayers[index].Draw(InGameMenu.sEffect, vector2.X, vector2.Y);
      this.mKills[index].Draw(InGameMenu.sEffect, vector2.X + 300f * InGameMenu.sScale, vector2.Y);
      this.mDeaths[index].Draw(InGameMenu.sEffect, vector2.X + 400f * InGameMenu.sScale, vector2.Y);
      vector2.Y += 32f * InGameMenu.sScale;
    }
    vector2.X = InGameMenu.sScreenSize.X * 0.5f;
    vector2.Y = (float) ((double) InGameMenu.sScreenSize.Y * 0.5 + 210.0 - 64.0);
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

  protected override void OnExit()
  {
  }
}
