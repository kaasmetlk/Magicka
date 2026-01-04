// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.GameStates.InGameMenus.InGameMenuVersusStatistics
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.GameLogic.GameStates.Menu.Main;
using Magicka.GameLogic.Statistics;
using Magicka.GameLogic.UI;
using Magicka.Gamers;
using Magicka.Graphics;
using Magicka.Levels;
using Magicka.Levels.Versus;
using Magicka.Localization;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.GameStates.InGameMenus;

internal class InGameMenuVersusStatistics : InGameMenu
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
  private static InGameMenuVersusStatistics sSingelton;
  private static volatile object sSingeltonLock = new object();
  private static readonly int LOC_VS_KING = "#versus_king".GetHashCodeCustom();
  private static readonly int LOC_VS_DM = "#versus_dm".GetHashCodeCustom();
  private static readonly int LOC_VS_BRAWL = "#versus_brawl".GetHashCodeCustom();
  private static readonly int LOC_VS_PYRITE = "#versus_pyrite".GetHashCodeCustom();
  private static readonly int LOC_VS_TOURNEY = "#versus_tourney".GetHashCodeCustom();
  private static readonly int LOC_RETRY = "#add_menu_retry".GetHashCodeCustom();
  private static readonly int LOC_READY = "#menu_charslct_02".GetHashCodeCustom();
  private static readonly int LOC_DISCONNECT = "#network_10".GetHashCodeCustom();
  private static readonly int LOC_QUIT = "#add_menu_quit".GetHashCodeCustom();
  private static readonly int LOC_LOBBY = "#menu_lobby".GetHashCodeCustom();
  private static readonly int LOC_DRAW = "#menu_stat_draw".GetHashCodeCustom();
  private BitmapFont mItemFont = FontManager.Instance.GetFont(MagickaFont.Maiandra18);
  private Texture2D mPagesTexture;
  private Text mModeText;
  private Text mVictorText;
  private Text mLoserTeamText;
  private int mVictorPlayerIndex;
  private int mVictorPlayerColor;
  private List<KeyValuePair<int, int>> mSortingOrder;
  private List<KeyValuePair<int, int>> mLoserTeamOrder;
  private Text[] mPlayerTexts;
  private Text[] mKillsTexts;
  private Text[] mDeathsTexts;
  private Magicka.GameLogic.Player[] mPlayers;
  private VersusRuleset mRuleset;
  private bool mRedWin;
  private bool mBlueWin;

  public static InGameMenuVersusStatistics Instance
  {
    get
    {
      if (InGameMenuVersusStatistics.sSingelton == null)
      {
        lock (InGameMenuVersusStatistics.sSingeltonLock)
        {
          if (InGameMenuVersusStatistics.sSingelton == null)
            InGameMenuVersusStatistics.sSingelton = new InGameMenuVersusStatistics();
        }
      }
      return InGameMenuVersusStatistics.sSingelton;
    }
  }

  private InGameMenuVersusStatistics()
  {
    this.mBackgroundSize = new Vector2(600f, 600f);
    this.mRedWin = false;
    this.mBlueWin = false;
    this.mSortingOrder = new List<KeyValuePair<int, int>>(4);
    this.mLoserTeamOrder = new List<KeyValuePair<int, int>>(4);
    BitmapFont font1 = FontManager.Instance.GetFont(MagickaFont.MenuOption);
    this.mModeText = new Text(200, font1, TextAlign.Center, false);
    this.mModeText.SetText("");
    this.mVictorText = new Text(200, font1, TextAlign.Center, false);
    this.mVictorText.SetText("");
    this.mLoserTeamText = new Text(200, font1, TextAlign.Center, false);
    this.mLoserTeamText.SetText("");
    lock (Magicka.Game.Instance.GraphicsDevice)
      this.mPagesTexture = Magicka.Game.Instance.Content.Load<Texture2D>("UI/Menu/Pages");
    BitmapFont font2 = FontManager.Instance.GetFont(MagickaFont.Maiandra18);
    this.mKillsTexts = new Text[4];
    this.mDeathsTexts = new Text[4];
    this.mPlayerTexts = new Text[4];
    for (int index = 0; index < 4; ++index)
    {
      this.mKillsTexts[index] = new Text(200, font2, TextAlign.Left, false);
      this.mKillsTexts[index].SetText("");
      this.mDeathsTexts[index] = new Text(200, font2, TextAlign.Left, false);
      this.mDeathsTexts[index].SetText("");
      this.mPlayerTexts[index] = new Text(200, font2, TextAlign.Left, false);
      this.mPlayerTexts[index].SetText("");
    }
    this.AddMenuTextItem(InGameMenuVersusStatistics.LOC_RETRY, this.mItemFont, TextAlign.Center);
    this.AddMenuTextItem(InGameMenuVersusStatistics.LOC_DISCONNECT, this.mItemFont, TextAlign.Center);
    this.AddMenuTextItem(InGameMenuVersusStatistics.LOC_QUIT, this.mItemFont, TextAlign.Center);
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
    Vector2 iPosition = new Vector2();
    iPosition.X = InGameMenu.sScreenSize.X * 0.5f;
    iPosition.Y = (float) ((double) InGameMenu.sScreenSize.Y * 0.5 - 200.0);
    InGameMenu.sEffect.Color = new Vector4(1f);
    this.mModeText.Draw(InGameMenu.sEffect, iPosition.X, iPosition.Y);
    iPosition.Y += (float) this.mModeText.Font.LineHeight * 2f;
    if (this.mRuleset.Teams)
    {
      if (this.mRedWin)
      {
        Matrix iTransform = new Matrix();
        iTransform.M44 = 1f;
        iTransform.M11 = iBackgroundSize.X * InGameMenu.sScale;
        iTransform.M22 = (float) this.mVictorText.Font.LineHeight;
        iTransform.M41 = iPosition.X - iBackgroundSize.X * 0.5f * InGameMenu.sScale;
        iTransform.M42 = iPosition.Y;
        InGameMenu.sEffect.Color = new Vector4(Defines.PLAYERCOLORS[0], 0.8f);
        this.DrawStrip(ref iTransform);
        InGameMenu.sEffect.Color = new Vector4(1f);
        this.mVictorText.Draw(InGameMenu.sEffect, iPosition.X, iPosition.Y);
        iPosition.Y += (float) this.mVictorText.Font.LineHeight;
        this.DrawSortedPlayerScores(ref iPosition, this.mSortingOrder);
        iPosition.Y += (float) this.mVictorText.Font.LineHeight;
        iTransform.M42 = iPosition.Y;
        InGameMenu.sEffect.Color = new Vector4(Defines.PLAYERCOLORS[3], 0.8f);
        this.DrawStrip(ref iTransform);
        InGameMenu.sEffect.Color = new Vector4(1f);
        this.mLoserTeamText.Draw(InGameMenu.sEffect, iPosition.X, iPosition.Y);
        iPosition.Y += (float) this.mLoserTeamText.Font.LineHeight;
        this.DrawSortedPlayerScores(ref iPosition, this.mLoserTeamOrder);
      }
      else if (this.mBlueWin)
      {
        Matrix iTransform = new Matrix();
        iTransform.M44 = 1f;
        iTransform.M11 = iBackgroundSize.X * InGameMenu.sScale;
        iTransform.M22 = (float) this.mVictorText.Font.LineHeight;
        iTransform.M41 = iPosition.X - iBackgroundSize.X * 0.5f * InGameMenu.sScale;
        iTransform.M42 = iPosition.Y;
        InGameMenu.sEffect.Color = new Vector4(Defines.PLAYERCOLORS[3], 0.8f);
        this.DrawStrip(ref iTransform);
        InGameMenu.sEffect.Color = new Vector4(1f);
        this.mVictorText.Draw(InGameMenu.sEffect, iPosition.X, iPosition.Y);
        iPosition.Y += (float) this.mVictorText.Font.LineHeight;
        this.DrawSortedPlayerScores(ref iPosition, this.mSortingOrder);
        iPosition.Y += (float) this.mVictorText.Font.LineHeight;
        iTransform.M42 = iPosition.Y;
        InGameMenu.sEffect.Color = new Vector4(Defines.PLAYERCOLORS[0], 0.8f);
        this.DrawStrip(ref iTransform);
        InGameMenu.sEffect.Color = new Vector4(1f);
        this.mLoserTeamText.Draw(InGameMenu.sEffect, iPosition.X, iPosition.Y);
        iPosition.Y += (float) this.mLoserTeamText.Font.LineHeight;
        this.DrawSortedPlayerScores(ref iPosition, this.mLoserTeamOrder);
      }
      else
      {
        Matrix iTransform = new Matrix();
        iTransform.M44 = 1f;
        iTransform.M11 = iBackgroundSize.X * InGameMenu.sScale;
        iTransform.M22 = (float) this.mVictorText.Font.LineHeight;
        iTransform.M41 = iPosition.X - iBackgroundSize.X * 0.5f * InGameMenu.sScale;
        iTransform.M42 = iPosition.Y;
        if (this.mVictorPlayerIndex != -1)
        {
          InGameMenu.sEffect.Color = new Vector4(1f, 1f, 1f, 0.8f);
          this.DrawStrip(ref iTransform);
          InGameMenu.sEffect.Color = new Vector4(1f);
        }
        this.mVictorText.Draw(InGameMenu.sEffect, iPosition.X, iPosition.Y);
        iPosition.Y += (float) this.mVictorText.Font.LineHeight;
        this.DrawSortedPlayerScores(ref iPosition, this.mSortingOrder);
      }
    }
    else
    {
      Matrix iTransform = new Matrix();
      iTransform.M44 = 1f;
      iTransform.M11 = iBackgroundSize.X * InGameMenu.sScale;
      iTransform.M22 = (float) this.mVictorText.Font.LineHeight;
      iTransform.M41 = iPosition.X - iBackgroundSize.X * 0.5f * InGameMenu.sScale;
      iTransform.M42 = iPosition.Y;
      if (this.mVictorPlayerIndex != -1)
      {
        InGameMenu.sEffect.Color = new Vector4(Defines.PLAYERCOLORS[this.mVictorPlayerColor], 0.8f);
        this.DrawStrip(ref iTransform);
        InGameMenu.sEffect.Color = this.mVictorPlayerColor != 10 ? new Vector4(1f) : new Vector4(0.0f, 0.0f, 0.0f, 1f);
      }
      this.mVictorText.Draw(InGameMenu.sEffect, iPosition.X, iPosition.Y);
      InGameMenu.sEffect.Color = new Vector4(1f);
      iPosition.Y += (float) this.mVictorText.Font.LineHeight;
      this.DrawSortedPlayerScores(ref iPosition, this.mSortingOrder);
    }
    iPosition.Y = (float) ((double) InGameMenu.sScreenSize.Y * 0.5 + 300.0 - (double) this.mMenuItems.Count * 32.0);
    NetworkState state = NetworkManager.Instance.State;
    for (int index = 0; index < this.mMenuItems.Count; ++index)
    {
      if ((state != NetworkState.Client || index != 0) && (state != NetworkState.Offline || index != 1))
      {
        MenuItem mMenuItem = this.mMenuItems[index];
        mMenuItem.Position = iPosition;
        mMenuItem.Draw(InGameMenu.sEffect);
        iPosition.Y += mMenuItem.BottomRight.Y - mMenuItem.TopLeft.Y;
      }
    }
  }

  private void DrawStrip(ref Matrix iTransform)
  {
    InGameMenu.sEffect.TextureOffset = new Vector2(0.0f, 0.0f);
    InGameMenu.sEffect.TextureScale = new Vector2(1f, 1f);
    InGameMenu.sEffect.Texture = (Texture) InGameMenu.sBackgroundTexture;
    InGameMenu.sEffect.TextureEnabled = true;
    InGameMenu.sEffect.Transform = iTransform;
    InGameMenu.sEffect.GraphicsDevice.Vertices[0].SetSource(InGameMenu.sBackground, 0, 8);
    InGameMenu.sEffect.GraphicsDevice.VertexDeclaration = InGameMenu.sBackgroundDeclaration;
    InGameMenu.sEffect.CommitChanges();
    InGameMenu.sEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
  }

  private void DrawSortedPlayerScores(
    ref Vector2 iPosition,
    List<KeyValuePair<int, int>> iSortedList)
  {
    for (int index1 = 0; index1 < iSortedList.Count; ++index1)
    {
      int index2 = iSortedList[index1].Value;
      this.mPlayerTexts[index2].Draw(InGameMenu.sEffect, iPosition.X - 180f, iPosition.Y);
      this.mKillsTexts[index2].Draw(InGameMenu.sEffect, iPosition.X + 10f, iPosition.Y);
      this.mDeathsTexts[index2].Draw(InGameMenu.sEffect, iPosition.X + 110f, iPosition.Y);
      this.DrawGraphics(this.mPagesTexture, new Rectangle(1024 /*0x0400*/, 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/), new Vector4(iPosition.X + 40f, iPosition.Y, 32f * InGameMenu.sScale, 32f * InGameMenu.sScale));
      this.DrawGraphics(this.mPagesTexture, new Rectangle(1024 /*0x0400*/, 0, 64 /*0x40*/, 64 /*0x40*/), new Vector4(iPosition.X + 140f, iPosition.Y, 32f * InGameMenu.sScale, 32f * InGameMenu.sScale));
      iPosition.Y += (float) this.mPlayerTexts[index2].Font.LineHeight;
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
    string str = InGameMenuVersusStatistics.OPTION_STRINGS[this.mSelectedItem];
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
    if (NetworkManager.Instance.State == NetworkState.Client)
    {
      this.mMenuItems[0].Enabled = false;
      this.mMenuItems[1].Enabled = true;
      this.mMenuItems[2].Enabled = true;
      (this.mMenuItems[2] as MenuTextItem).SetText(InGameMenuVersusStatistics.LOC_READY);
    }
    else if (NetworkManager.Instance.State == NetworkState.Server)
    {
      this.mMenuItems[0].Enabled = false;
      this.mMenuItems[1].Enabled = true;
      this.mMenuItems[2].Enabled = false;
      (this.mMenuItems[2] as MenuTextItem).SetText(InGameMenuVersusStatistics.LOC_LOBBY);
    }
    else
    {
      this.mMenuItems[0].Enabled = true;
      this.mMenuItems[1].Enabled = false;
      this.mMenuItems[2].Enabled = true;
      (this.mMenuItems[2] as MenuTextItem).SetText(InGameMenuVersusStatistics.LOC_QUIT);
    }
    this.mRedWin = false;
    this.mBlueWin = false;
    this.mPlayers = Magicka.Game.Instance.Players;
    this.mRuleset = InGameMenu.sPlayState.Level.CurrentScene.RuleSet as VersusRuleset;
    short[] scores = this.mRuleset.GetScores();
    short[] teamScores = this.mRuleset.GetTeamScores();
    this.mSortingOrder.Clear();
    this.mLoserTeamOrder.Clear();
    this.mVictorPlayerColor = 10;
    if (this.mRuleset.Teams)
    {
      for (int index = 0; index < this.mPlayers.Length; ++index)
      {
        if (this.mPlayers[index].Playing)
          this.mPlayerTexts[index].SetText(this.mPlayers[index].GamerTag);
      }
      for (int iPlayer = 0; iPlayer < scores.Length && iPlayer < this.mKillsTexts.Length; ++iPlayer)
      {
        if (this.mPlayers[iPlayer].Playing)
        {
          this.mPlayerTexts[iPlayer].SetText(this.mPlayers[iPlayer].GamerTag);
          StatisticsManager.ScoreValues score = StatisticsManager.Instance.GetScore(iPlayer);
          this.mKillsTexts[iPlayer].SetText(score.Kills.ToString());
          this.mDeathsTexts[iPlayer].SetText(score.Deaths.ToString());
        }
      }
      if ((int) teamScores[0] > (int) teamScores[1])
      {
        this.mRedWin = true;
        this.mVictorText.SetText(LanguageManager.Instance.GetString(Defines.LOC_GAME_VS_WINNER).Replace("#player;", LanguageManager.Instance.GetString(VersusRuleset.LOC_TEAMRED)));
        this.mLoserTeamText.SetText(LanguageManager.Instance.GetString(VersusRuleset.LOC_TEAMBLUE));
        for (int iPlayer = 0; iPlayer < this.mPlayers.Length; ++iPlayer)
        {
          if (this.mPlayers[iPlayer].Playing)
          {
            if ((this.mPlayers[iPlayer].Team & Factions.TEAM_RED) == Factions.TEAM_RED)
            {
              this.mSortingOrder.Add(new KeyValuePair<int, int>((int) StatisticsManager.Instance.GetScore(iPlayer).Kills, iPlayer));
              if (this.mPlayers[iPlayer].Gamer != null && !(this.mPlayers[iPlayer].Gamer is NetworkGamer))
              {
                ++this.mPlayers[iPlayer].Gamer.VersusWins;
                this.mPlayers[iPlayer].Gamer.IncrementVersusWinStreak(InGameMenu.sPlayState);
              }
            }
            else if ((this.mPlayers[iPlayer].Team & Factions.TEAM_BLUE) == Factions.TEAM_BLUE)
            {
              this.mLoserTeamOrder.Add(new KeyValuePair<int, int>((int) StatisticsManager.Instance.GetScore(iPlayer).Kills, iPlayer));
              if (this.mPlayers[iPlayer].Gamer != null && !(this.mPlayers[iPlayer].Gamer is NetworkGamer))
              {
                ++this.mPlayers[iPlayer].Gamer.VersusDefeats;
                this.mPlayers[iPlayer].Gamer.ResetVersusWinStreak();
              }
            }
          }
        }
        this.mSortingOrder.Sort((Comparison<KeyValuePair<int, int>>) ((kvp1, kvp2) =>
        {
          if (kvp1.Key > kvp2.Key)
            return -1;
          return kvp1.Key < kvp2.Key ? 1 : 0;
        }));
        this.mLoserTeamOrder.Sort((Comparison<KeyValuePair<int, int>>) ((kvp1, kvp2) =>
        {
          if (kvp1.Key > kvp2.Key)
            return -1;
          return kvp1.Key < kvp2.Key ? 1 : 0;
        }));
      }
      else if ((int) teamScores[0] < (int) teamScores[1])
      {
        this.mBlueWin = true;
        this.mVictorText.SetText(LanguageManager.Instance.GetString(Defines.LOC_GAME_VS_WINNER).Replace("#player;", LanguageManager.Instance.GetString(VersusRuleset.LOC_TEAMBLUE)));
        this.mLoserTeamText.SetText(LanguageManager.Instance.GetString(VersusRuleset.LOC_TEAMRED));
        for (int iPlayer = 0; iPlayer < this.mPlayers.Length; ++iPlayer)
        {
          if (this.mPlayers[iPlayer].Playing)
          {
            if ((this.mPlayers[iPlayer].Team & Factions.TEAM_BLUE) == Factions.TEAM_BLUE)
            {
              this.mSortingOrder.Add(new KeyValuePair<int, int>((int) StatisticsManager.Instance.GetScore(iPlayer).Kills, iPlayer));
              if (this.mPlayers[iPlayer].Gamer != null && !(this.mPlayers[iPlayer].Gamer is NetworkGamer))
              {
                ++this.mPlayers[iPlayer].Gamer.VersusWins;
                this.mPlayers[iPlayer].Gamer.IncrementVersusWinStreak(InGameMenu.sPlayState);
              }
            }
            else if ((this.mPlayers[iPlayer].Team & Factions.TEAM_RED) == Factions.TEAM_RED)
            {
              this.mLoserTeamOrder.Add(new KeyValuePair<int, int>((int) StatisticsManager.Instance.GetScore(iPlayer).Kills, iPlayer));
              if (this.mPlayers[iPlayer].Gamer != null && !(this.mPlayers[iPlayer].Gamer is NetworkGamer))
              {
                ++this.mPlayers[iPlayer].Gamer.VersusDefeats;
                this.mPlayers[iPlayer].Gamer.ResetVersusWinStreak();
              }
            }
          }
        }
        this.mSortingOrder.Sort((Comparison<KeyValuePair<int, int>>) ((kvp1, kvp2) =>
        {
          if (kvp1.Key > kvp2.Key)
            return -1;
          return kvp1.Key < kvp2.Key ? 1 : 0;
        }));
        this.mLoserTeamOrder.Sort((Comparison<KeyValuePair<int, int>>) ((kvp1, kvp2) =>
        {
          if (kvp1.Key > kvp2.Key)
            return -1;
          return kvp1.Key < kvp2.Key ? 1 : 0;
        }));
      }
      else
      {
        this.mVictorText.SetText(LanguageManager.Instance.GetString(InGameMenuVersusStatistics.LOC_DRAW));
        for (int iPlayer = 0; iPlayer < this.mPlayers.Length; ++iPlayer)
        {
          if (this.mPlayers[iPlayer].Playing)
            this.mSortingOrder.Add(new KeyValuePair<int, int>((int) StatisticsManager.Instance.GetScore(iPlayer).Kills, iPlayer));
        }
        this.mSortingOrder.Sort((Comparison<KeyValuePair<int, int>>) ((kvp1, kvp2) =>
        {
          if (kvp1.Key > kvp2.Key)
            return -1;
          return kvp1.Key < kvp2.Key ? 1 : 0;
        }));
      }
    }
    else
    {
      for (int iPlayer = 0; iPlayer < scores.Length && iPlayer < this.mKillsTexts.Length; ++iPlayer)
      {
        if (this.mPlayers[iPlayer].Playing)
        {
          this.mPlayerTexts[iPlayer].SetText(this.mPlayers[iPlayer].GamerTag);
          StatisticsManager.ScoreValues score = StatisticsManager.Instance.GetScore(iPlayer);
          this.mKillsTexts[iPlayer].SetText(score.Kills.ToString());
          this.mDeathsTexts[iPlayer].SetText(score.Deaths.ToString());
          this.mSortingOrder.Add(new KeyValuePair<int, int>((int) score.Kills, iPlayer));
        }
      }
      this.mSortingOrder.Sort((Comparison<KeyValuePair<int, int>>) ((kvp1, kvp2) =>
      {
        if (kvp1.Key > kvp2.Key)
          return -1;
        return kvp1.Key < kvp2.Key ? 1 : 0;
      }));
      int minValue = int.MinValue;
      this.mVictorPlayerIndex = -1;
      bool flag = false;
      for (int index = 0; index < scores.Length; ++index)
      {
        if (this.mPlayers[index].Playing)
        {
          if ((int) scores[index] > minValue)
          {
            minValue = (int) scores[index];
            this.mVictorPlayerIndex = index;
            flag = false;
          }
          else if (this.mVictorPlayerIndex != -1 && (int) scores[index] == minValue)
            flag = true;
        }
      }
      if (flag)
      {
        this.mVictorText.SetText(LanguageManager.Instance.GetString(InGameMenuVersusStatistics.LOC_DRAW));
        for (int index = 0; index < scores.Length; ++index)
        {
          if (this.mPlayers[index].Playing && this.mPlayers[index].Gamer != null && !(this.mPlayers[index].Gamer is NetworkGamer))
          {
            this.mPlayers[index].Gamer.ResetVersusWinStreak();
            ++this.mPlayers[index].Gamer.VersusDefeats;
          }
        }
      }
      else
      {
        this.mVictorText.SetText(LanguageManager.Instance.GetString(Defines.LOC_GAME_VS_WINNER).Replace("#player;", this.mPlayers[this.mVictorPlayerIndex].GamerTag));
        this.mVictorPlayerColor = (int) this.mPlayers[this.mVictorPlayerIndex].Color;
        for (int index = 0; index < scores.Length; ++index)
        {
          if (this.mPlayers[index].Playing && this.mPlayers[index].Gamer != null && !(this.mPlayers[index].Gamer is NetworkGamer))
          {
            if (index == this.mVictorPlayerIndex)
            {
              ++this.mPlayers[index].Gamer.VersusWins;
              this.mPlayers[index].Gamer.IncrementVersusWinStreak(InGameMenu.sPlayState);
            }
            else
            {
              this.mPlayers[index].Gamer.ResetVersusWinStreak();
              ++this.mPlayers[index].Gamer.VersusDefeats;
            }
          }
        }
      }
    }
    if (this.mRuleset is DeathMatch)
      this.mModeText.SetText(LanguageManager.Instance.GetString(InGameMenuVersusStatistics.LOC_VS_DM));
    else if (this.mRuleset is Brawl)
      this.mModeText.SetText(LanguageManager.Instance.GetString(InGameMenuVersusStatistics.LOC_VS_BRAWL));
    else if (this.mRuleset is Krietor)
      this.mModeText.SetText(LanguageManager.Instance.GetString(InGameMenuVersusStatistics.LOC_VS_TOURNEY));
    else if (this.mRuleset is King)
    {
      this.mModeText.SetText(LanguageManager.Instance.GetString(InGameMenuVersusStatistics.LOC_VS_KING));
    }
    else
    {
      if (!(this.mRuleset is Pyrite))
        return;
      this.mModeText.SetText(LanguageManager.Instance.GetString(InGameMenuVersusStatistics.LOC_VS_PYRITE));
    }
  }

  protected override void OnExit()
  {
  }
}
