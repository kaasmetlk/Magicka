// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.GameStates.Menu.Main.SubMenuLeaderboards
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.Statistics;
using Magicka.GameLogic.UI;
using Magicka.Graphics;
using Magicka.Levels;
using Magicka.Levels.Campaign;
using Magicka.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PolygonHead;
using SteamWrapper;
using System;
using System.Collections.Generic;
using System.Text;

#nullable disable
namespace Magicka.GameLogic.GameStates.Menu.Main;

internal class SubMenuLeaderboards : SubMenu
{
  private const int VISIBLE_RANKS = 8;
  private static SubMenuLeaderboards mSingelton;
  private static volatile object mSingeltonLock = new object();
  private VertexBuffer mVertexBuffer;
  private VertexDeclaration mVertexDeclaration;
  private static readonly int LOC_LEADERBOARD = "#menu_lb_01".GetHashCodeCustom();
  private static readonly int LOC_ONLINE = "#network_02".GetHashCodeCustom();
  private static readonly int LOC_LOCAL = "#network_31".GetHashCodeCustom();
  public static readonly int LOC_DEBUG = "#location_debug".GetHashCodeCustom();
  public static readonly int LOC_SWAMP = "#challenge_swamp".GetHashCodeCustom();
  public static readonly int LOC_GLADE = "#challenge_glade".GetHashCodeCustom();
  public static readonly int LOC_CAVERN = "#challenge_cavern".GetHashCodeCustom();
  public static readonly int LOC_ARENA = "#challenge_arena".GetHashCodeCustom();
  public static readonly int LOC_TITLE_TIME = "#lb_time".GetHashCodeCustom();
  private static readonly int LOC_RANK = "#lb_rank".GetHashCodeCustom();
  private static readonly int LOC_SCORE = "#lb_score".GetHashCodeCustom();
  private static readonly int LOC_NAME = "#lb_name".GetHashCodeCustom();
  private static readonly int LOC_TIME = "#lb_time".GetHashCodeCustom();
  private static readonly int LOC_WAVES = "#lb_wave".GetHashCodeCustom();
  private SubMenuLeaderboards.Selections mSelection = SubMenuLeaderboards.Selections.Level;
  private int mChallenge;
  private bool mShowLocalBoards;
  private List<ulong> mSteamLeaderboards;
  private static readonly Vector2 TITLE_POSITION = new Vector2(512f, 192f);
  private static readonly Vector2 CHALLENGE_POSITION = new Vector2(144f, SubMenuLeaderboards.TITLE_POSITION.Y + 48f);
  private static readonly Vector2 CHALLENGE_SIZE = new Vector2(768f, 256f);
  private static readonly Vector2 RANKS_POSITION = new Vector2(SubMenuLeaderboards.CHALLENGE_POSITION.X, SubMenuLeaderboards.CHALLENGE_POSITION.Y + SubMenuLeaderboards.CHALLENGE_SIZE.Y);
  private static readonly Vector2 NAMES_POSITION = new Vector2(SubMenuLeaderboards.RANKS_POSITION.X + 128f, SubMenuLeaderboards.RANKS_POSITION.Y);
  private static readonly Vector2 WAVES_POSITION = new Vector2((float) ((double) SubMenuLeaderboards.RANKS_POSITION.X + 128.0 + 256.0 + 32.0 + 32.0), SubMenuLeaderboards.RANKS_POSITION.Y);
  private static readonly Vector2 SCORES_POSITION = new Vector2(896f, SubMenuLeaderboards.RANKS_POSITION.Y);
  private static readonly Vector2 ONLINE_POSITION = new Vector2(848f, 960f);
  private StringBuilder mRankStringBuilder;
  private StringBuilder mNameStringBuilder;
  private StringBuilder mScoreStringBuilder;
  private StringBuilder mDataStringBuilder;
  private PolygonHead.Text mRanksHeader;
  private PolygonHead.Text mNamesHeader;
  private PolygonHead.Text mScoresHeader;
  private PolygonHead.Text mWavesHeader;
  private PolygonHead.Text mRankColumn;
  private PolygonHead.Text mNameColumn;
  private PolygonHead.Text mScoreColumn;
  private PolygonHead.Text mWavesColumn;
  private float mHeadLineHeight;
  private float mLineHeight;
  private int mCurrentTopRank;
  private int mVisibleRanks;
  private MenuImageItem mListRightArrow;
  private MenuImageItem mListLeftArrow;
  private PolygonHead.Text mListTop;
  private PolygonHead.Text mListBottom;
  private MenuImageItem mLevelRightArrow;
  private MenuImageItem mLevelLeftArrow;
  private ContentManager mCurrentContent;
  private string mCurrentTexturePath;
  private string mTexturePath = "Levels/Challenges/#1;";
  private Texture2D mCurrentTexture;
  private float mTextureAlpha;

  public static SubMenuLeaderboards Instance
  {
    get
    {
      if (SubMenuLeaderboards.mSingelton == null)
      {
        lock (SubMenuLeaderboards.mSingeltonLock)
        {
          if (SubMenuLeaderboards.mSingelton == null)
            SubMenuLeaderboards.mSingelton = new SubMenuLeaderboards();
        }
      }
      return SubMenuLeaderboards.mSingelton;
    }
  }

  public SubMenuLeaderboards()
  {
    this.mSteamLeaderboards = StatisticsManager.Instance.SteamLeaderboards;
    lock (Magicka.Game.Instance.GraphicsDevice)
    {
      this.mVertexBuffer = new VertexBuffer(Magicka.Game.Instance.GraphicsDevice, Defines.QUAD_TEX_VERTS_C.Length * VertexPositionTexture.SizeInBytes, BufferUsage.WriteOnly);
      this.mVertexBuffer.SetData<VertexPositionTexture>(Defines.QUAD_TEX_VERTS_C);
      this.mVertexDeclaration = RenderManager.Instance.CreateVertexDeclaration(VertexPositionTexture.VertexElements);
    }
    this.mRankStringBuilder = new StringBuilder(512 /*0x0200*/);
    this.mNameStringBuilder = new StringBuilder(512 /*0x0200*/);
    this.mScoreStringBuilder = new StringBuilder(512 /*0x0200*/);
    this.mDataStringBuilder = new StringBuilder(512 /*0x0200*/);
    this.mTextureAlpha = 0.0f;
    this.mCurrentTopRank = 0;
    this.mSelectedPosition = 0;
    this.mShowLocalBoards = true;
    BitmapFont font1 = FontManager.Instance.GetFont(MagickaFont.MenuOption);
    this.mHeadLineHeight = (float) font1.LineHeight;
    this.mRanksHeader = new PolygonHead.Text(64 /*0x40*/, font1, TextAlign.Left, false);
    this.mNamesHeader = new PolygonHead.Text(64 /*0x40*/, font1, TextAlign.Left, false);
    this.mScoresHeader = new PolygonHead.Text(64 /*0x40*/, font1, TextAlign.Right, false);
    this.mWavesHeader = new PolygonHead.Text(64 /*0x40*/, font1, TextAlign.Left, false);
    this.mRanksHeader.SetText(LanguageManager.Instance.GetString(SubMenuLeaderboards.LOC_RANK));
    this.mNamesHeader.SetText(LanguageManager.Instance.GetString(SubMenuLeaderboards.LOC_NAME));
    this.mWavesHeader.SetText(LanguageManager.Instance.GetString(SubMenuLeaderboards.LOC_WAVES));
    this.mScoresHeader.SetText(LanguageManager.Instance.GetString(SubMenuLeaderboards.LOC_SCORE));
    BitmapFont font2 = FontManager.Instance.GetFont(MagickaFont.MenuOption);
    this.mLineHeight = (float) font2.LineHeight;
    this.mRankColumn = new PolygonHead.Text(1024 /*0x0400*/, font2, TextAlign.Left, false);
    this.mNameColumn = new PolygonHead.Text(1024 /*0x0400*/, font2, TextAlign.Left, false);
    this.mScoreColumn = new PolygonHead.Text(1024 /*0x0400*/, font2, TextAlign.Right, false);
    this.mWavesColumn = new PolygonHead.Text(1024 /*0x0400*/, font2, TextAlign.Left, false);
    this.mMenuItems = new List<MenuItem>();
    this.mMenuTitle = new PolygonHead.Text(30, FontManager.Instance.GetFont(MagickaFont.MenuTitle), TextAlign.Center, false);
    this.mMenuTitle.SetText(LanguageManager.Instance.GetString(SubMenu.LOC_LEADERBOARDS));
    VertexPositionTexture[] data = new VertexPositionTexture[Defines.QUAD_TEX_VERTS_C.Length];
    Defines.QUAD_TEX_VERTS_C.CopyTo((Array) data, 0);
    data[0].TextureCoordinate.X = 1344f / (float) SubMenu.sPagesTexture.Width;
    data[0].TextureCoordinate.Y = 160f / (float) SubMenu.sPagesTexture.Height;
    data[1].TextureCoordinate.X = 1280f / (float) SubMenu.sPagesTexture.Width;
    data[1].TextureCoordinate.Y = 160f / (float) SubMenu.sPagesTexture.Height;
    data[2].TextureCoordinate.X = 1280f / (float) SubMenu.sPagesTexture.Width;
    data[2].TextureCoordinate.Y = 96f / (float) SubMenu.sPagesTexture.Height;
    data[3].TextureCoordinate.X = 1344f / (float) SubMenu.sPagesTexture.Width;
    data[3].TextureCoordinate.Y = 96f / (float) SubMenu.sPagesTexture.Height;
    VertexBuffer iVertices;
    VertexDeclaration iDeclaration;
    lock (Magicka.Game.Instance.GraphicsDevice)
    {
      iVertices = new VertexBuffer(Magicka.Game.Instance.GraphicsDevice, data.Length * VertexPositionTexture.SizeInBytes, BufferUsage.WriteOnly);
      iVertices.SetData<VertexPositionTexture>(data);
      iDeclaration = new VertexDeclaration(Magicka.Game.Instance.GraphicsDevice, VertexPositionTexture.VertexElements);
    }
    this.mMenuItems.Add((MenuItem) new MenuTextItem(SubMenuLeaderboards.LOC_DEBUG, SubMenuLeaderboards.TITLE_POSITION, FontManager.Instance.GetFont(MagickaFont.MenuOption), TextAlign.Center));
    this.mMenuItems.Add((MenuItem) new MenuTextItem(SubMenuLeaderboards.LOC_LOCAL, SubMenuLeaderboards.ONLINE_POSITION, FontManager.Instance.GetFont(MagickaFont.MenuOption), TextAlign.Center));
    this.mListRightArrow = new MenuImageItem(new Vector2(this.mPosition.X + 128f, 896f), SubMenu.sPagesTexture, iVertices, iDeclaration, -1.57079637f, 0, VertexPositionTexture.SizeInBytes, 64f, 64f);
    this.mListLeftArrow = new MenuImageItem(new Vector2(this.mPosition.X - 128f, 896f), SubMenu.sPagesTexture, iVertices, iDeclaration, 1.57079637f, 0, VertexPositionTexture.SizeInBytes, 64f, 64f);
    this.mListTop = new PolygonHead.Text(10, FontManager.Instance.GetFont(MagickaFont.MenuOption), TextAlign.Left, false);
    this.mListTop.SetText("1");
    this.mListTop.DefaultColor = MenuItem.COLOR;
    this.mListBottom = new PolygonHead.Text(10, FontManager.Instance.GetFont(MagickaFont.MenuOption), TextAlign.Right, false);
    this.mListBottom.SetText(8.ToString());
    this.mListBottom.DefaultColor = MenuItem.COLOR;
    this.mLevelRightArrow = new MenuImageItem(SubMenuLeaderboards.CHALLENGE_POSITION + new Vector2(SubMenuLeaderboards.CHALLENGE_SIZE.X + 16f, SubMenuLeaderboards.CHALLENGE_SIZE.Y * 0.5f), SubMenu.sPagesTexture, iVertices, iDeclaration, -1.57079637f, 0, VertexPositionTexture.SizeInBytes, 64f, 64f);
    this.mLevelLeftArrow = new MenuImageItem(SubMenuLeaderboards.CHALLENGE_POSITION + new Vector2(-16f, SubMenuLeaderboards.CHALLENGE_SIZE.Y * 0.5f), SubMenu.sPagesTexture, iVertices, iDeclaration, 1.57079637f, 0, VertexPositionTexture.SizeInBytes, 64f, 64f);
    this.mMenuItems.Add((MenuItem) new MenuImageTextItem(SubMenu.BACK_POSITION, SubMenu.sPagesTexture, SubMenu.BACK_UVOFFSET, SubMenu.BACK_UVSCALE, 0, new Vector2(), SubMenu.BACK_TEXT_ALIGN, FontManager.Instance.GetFont(SubMenu.BACK_FONT), SubMenu.BACK_SIZE));
  }

  public override void LanguageChanged()
  {
    base.LanguageChanged();
    this.mRanksHeader.SetText(LanguageManager.Instance.GetString(SubMenuLeaderboards.LOC_RANK));
    this.mNamesHeader.SetText(LanguageManager.Instance.GetString(SubMenuLeaderboards.LOC_NAME));
    if (LevelManager.Instance.Challenges[this.mChallenge].FileName.Equals("ch_vietnam", StringComparison.OrdinalIgnoreCase))
      this.mWavesHeader.SetText(LanguageManager.Instance.GetString(SubMenuLeaderboards.LOC_TITLE_TIME));
    else
      this.mWavesHeader.SetText(LanguageManager.Instance.GetString(SubMenuLeaderboards.LOC_WAVES));
    this.mScoresHeader.SetText(LanguageManager.Instance.GetString(SubMenuLeaderboards.LOC_SCORE));
    this.mMenuTitle.SetText(LanguageManager.Instance.GetString(SubMenu.LOC_LEADERBOARDS));
    this.UpdateScores();
    this.ChangeLeaderboard();
  }

  public override void Draw(Viewport iLeftSide, Viewport iRightSide)
  {
    this.mEffect.GraphicsDevice.Viewport = iRightSide;
    this.mEffect.Begin();
    this.mEffect.CurrentTechnique.Passes[0].Begin();
    this.mEffect.VertexColorEnabled = false;
    this.mEffect.Color = this.mSelection == SubMenuLeaderboards.Selections.List ? MenuItem.COLOR_SELECTED : MenuItem.COLOR;
    this.mRanksHeader.Draw(this.mEffect, SubMenuLeaderboards.RANKS_POSITION.X, SubMenuLeaderboards.RANKS_POSITION.Y);
    this.mNamesHeader.Draw(this.mEffect, SubMenuLeaderboards.NAMES_POSITION.X, SubMenuLeaderboards.NAMES_POSITION.Y);
    this.mWavesHeader.Draw(this.mEffect, SubMenuLeaderboards.WAVES_POSITION.X, SubMenuLeaderboards.WAVES_POSITION.Y);
    this.mScoresHeader.Draw(this.mEffect, SubMenuLeaderboards.SCORES_POSITION.X, SubMenuLeaderboards.SCORES_POSITION.Y);
    this.mRankColumn.Draw(this.mEffect, SubMenuLeaderboards.RANKS_POSITION.X + 8f, SubMenuLeaderboards.RANKS_POSITION.Y + this.mHeadLineHeight);
    this.mNameColumn.Draw(this.mEffect, SubMenuLeaderboards.NAMES_POSITION.X, SubMenuLeaderboards.NAMES_POSITION.Y + this.mHeadLineHeight);
    this.mWavesColumn.Draw(this.mEffect, SubMenuLeaderboards.WAVES_POSITION.X, SubMenuLeaderboards.WAVES_POSITION.Y + this.mHeadLineHeight);
    this.mScoreColumn.Draw(this.mEffect, SubMenuLeaderboards.SCORES_POSITION.X, SubMenuLeaderboards.SCORES_POSITION.Y + this.mHeadLineHeight);
    this.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertexBuffer, 0, VertexPositionTexture.SizeInBytes);
    this.mEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
    this.mEffect.VertexColorEnabled = true;
    this.mEffect.TextureEnabled = false;
    this.mEffect.Color = Vector4.One;
    if (!this.mShowLocalBoards)
    {
      this.mListRightArrow.Draw(this.mEffect, 64f);
      this.mListLeftArrow.Draw(this.mEffect, 64f);
      this.mEffect.VertexColorEnabled = false;
      this.mEffect.Color = this.mSelection == SubMenuLeaderboards.Selections.List ? MenuItem.COLOR_SELECTED : MenuItem.COLOR;
      Vector2 position = this.mListRightArrow.Position;
      position.Y -= (float) this.mListBottom.Font.LineHeight * 0.5f;
      position.X -= 32f;
      this.mListBottom.Draw(this.mEffect, position.X, position.Y);
      position = this.mListLeftArrow.Position;
      position.Y -= (float) this.mListBottom.Font.LineHeight * 0.5f;
      position.X += 32f;
      this.mListTop.Draw(this.mEffect, position.X, position.Y);
      this.mEffect.Color = Vector4.One;
    }
    this.mLevelRightArrow.Draw(this.mEffect, 64f);
    this.mLevelLeftArrow.Draw(this.mEffect, 64f);
    this.mEffect.VertexColorEnabled = false;
    this.mEffect.Color = new Vector4(0.0f, 0.0f, 0.0f, 0.8f);
    this.mEffect.CommitChanges();
    this.mMenuTitle.Draw(this.mEffect, this.mPosition.X, 96f);
    foreach (MenuItem mMenuItem in this.mMenuItems)
      mMenuItem.Draw(this.mEffect);
    if (this.mCurrentTexture != null && !this.mCurrentTexture.IsDisposed)
    {
      lock (this.mCurrentTexture)
      {
        this.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertexBuffer, 0, VertexPositionTexture.SizeInBytes);
        this.mEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
        Matrix identity = Matrix.Identity with
        {
          M11 = SubMenuLeaderboards.CHALLENGE_SIZE.X,
          M22 = SubMenuLeaderboards.CHALLENGE_SIZE.Y,
          M41 = SubMenuLeaderboards.CHALLENGE_POSITION.X + SubMenuLeaderboards.CHALLENGE_SIZE.X * 0.5f,
          M42 = SubMenuLeaderboards.CHALLENGE_POSITION.Y + SubMenuLeaderboards.CHALLENGE_SIZE.Y * 0.5f
        };
        this.mEffect.Color = new Vector4(1f, 1f, 1f, this.mTextureAlpha);
        this.mEffect.Transform = identity;
        this.mEffect.VertexColorEnabled = false;
        this.mEffect.Texture = (Texture) this.mCurrentTexture;
        this.mEffect.TextureEnabled = true;
        this.mEffect.TextureOffset = Vector2.Zero;
        this.mEffect.TextureScale = Vector2.One;
        this.mEffect.Saturation = 1f;
        this.mEffect.CommitChanges();
        this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
      }
    }
    this.mEffect.GraphicsDevice.Vertices[0].SetSource((VertexBuffer) null, 0, 0);
    this.mEffect.CurrentTechnique.Passes[0].End();
    this.mEffect.End();
  }

  public override void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    if (this.mCurrentTexture == null || this.mCurrentTexture.IsDisposed)
      return;
    this.mTextureAlpha = Math.Min(this.mTextureAlpha + iDeltaTime * 4f, 1f);
  }

  public override void ControllerMouseAction(
    Controller iSender,
    Point iScreenSize,
    MouseState iState,
    MouseState iOldState)
  {
    if (iState.LeftButton != ButtonState.Released || iOldState.LeftButton != ButtonState.Pressed)
      return;
    Vector2 oHitPosition;
    bool oRightPageHit;
    if (Tome.MousePickTome(iScreenSize, iState.X, iState.Y, out oHitPosition, out oRightPageHit) && oRightPageHit)
    {
      this.mSelectedPosition = -1;
      this.mMenuItems[0].Selected = false;
      this.mMenuItems[1].Selected = false;
      this.mMenuItems[2].Selected = false;
      if (!this.mShowLocalBoards && this.mListRightArrow.InsideBounds(ref oHitPosition))
      {
        int num = Math.Max(0, this.mCurrentTopRank + 8);
        if (this.mShowLocalBoards)
        {
          if (StatisticsManager.Instance.Leaderboard(this.mChallenge).Count > num)
          {
            this.mCurrentTopRank = num;
            this.UpdateScores();
          }
        }
        else if (this.mSteamLeaderboards.Count > this.mChallenge && SteamUserStats.GetLeaderboardEntryCount(this.mSteamLeaderboards[this.mChallenge]) > num)
        {
          this.mCurrentTopRank = num;
          this.UpdateScores();
        }
      }
      else if (!this.mShowLocalBoards && this.mListLeftArrow.InsideBounds(ref oHitPosition) && this.mCurrentTopRank > 0)
      {
        int num = Math.Max(0, this.mCurrentTopRank - 8);
        if (this.mShowLocalBoards)
        {
          if (StatisticsManager.Instance.Leaderboard(this.mChallenge).Count > num)
          {
            this.mCurrentTopRank = num;
            this.UpdateScores();
          }
        }
        else if (this.mSteamLeaderboards.Count > this.mChallenge && SteamUserStats.GetLeaderboardEntryCount(this.mSteamLeaderboards[this.mChallenge]) > num)
        {
          this.mCurrentTopRank = num;
          this.UpdateScores();
        }
      }
      else if (this.mLevelRightArrow.InsideBounds(ref oHitPosition))
      {
        this.mSelectedPosition = 0;
        this.mCurrentTopRank = 0;
        int num = this.mChallenge - 1;
        if (num < 0)
          num = LevelManager.Instance.Challenges.Length - 1;
        this.mChallenge = num;
        this.ChangeLeaderboard();
        this.mMenuItems[0].Selected = false;
      }
      else if (this.mLevelLeftArrow.InsideBounds(ref oHitPosition))
      {
        this.mSelectedPosition = 0;
        this.mCurrentTopRank = 0;
        int num = this.mChallenge + 1;
        if (num >= LevelManager.Instance.Challenges.Length)
          num = 0;
        this.mChallenge = num;
        this.ChangeLeaderboard();
        this.mMenuItems[0].Selected = false;
      }
      else if (this.mMenuItems[this.mMenuItems.Count - 2].InsideBounds(ref oHitPosition))
      {
        this.mSelectedPosition = 1;
        this.mShowLocalBoards = !this.mShowLocalBoards;
        if (this.mShowLocalBoards)
          (this.mMenuItems[1] as MenuTextItem).SetText(LanguageManager.Instance.GetString(SubMenuLeaderboards.LOC_LOCAL));
        else
          (this.mMenuItems[1] as MenuTextItem).SetText(LanguageManager.Instance.GetString(SubMenuLeaderboards.LOC_ONLINE));
        this.mCurrentTopRank = 0;
        this.ChangeLeaderboard();
        this.mMenuItems[1].Selected = true;
      }
      else if (this.mMenuItems[this.mMenuItems.Count - 1].InsideBounds(ref oHitPosition))
        Tome.Instance.PopMenu();
    }
    this.mSelection = SubMenuLeaderboards.Selections.None;
  }

  public override void ControllerMouseMove(
    Controller iSender,
    Point iScreenSize,
    MouseState iState,
    MouseState iOldState)
  {
    if (this.mMenuItems == null || this.mMenuItems.Count == 0)
      return;
    Vector2 oHitPosition;
    bool oRightPageHit;
    if (Tome.MousePickTome(iScreenSize, iState.X, iState.Y, out oHitPosition, out oRightPageHit))
    {
      if (!oRightPageHit)
        return;
      bool flag = false;
      for (int index1 = 0; index1 < this.mMenuItems.Count; ++index1)
      {
        MenuItem mMenuItem = this.mMenuItems[index1];
        if (mMenuItem.Enabled && mMenuItem.InsideBounds(ref oHitPosition))
        {
          if (this.mSelectedPosition != index1)
            AudioManager.Instance.PlayCue(Banks.UI, "ui_menu_mouseover".GetHashCodeCustom());
          this.mKeyboardSelection = false;
          this.mSelectedPosition = index1;
          for (int index2 = 0; index2 < this.mMenuItems.Count; ++index2)
            this.mMenuItems[index2].Selected = index2 == index1;
          flag = index1 != 0;
          break;
        }
      }
      if (flag)
        return;
      for (int index = 0; index < this.mMenuItems.Count; ++index)
        this.mMenuItems[index].Selected = false;
      this.mSelectedPosition = -1;
    }
    else
    {
      if (!(!this.mKeyboardSelection & this.mMenuItems != null))
        return;
      for (int index = 0; index < this.mMenuItems.Count; ++index)
        this.mMenuItems[index].Selected = false;
    }
  }

  public override void ControllerLeft(Controller iSender)
  {
    this.mMenuItems[0].Selected = false;
    this.mMenuItems[1].Selected = false;
    this.mMenuItems[2].Selected = false;
    switch (this.mSelection)
    {
      case SubMenuLeaderboards.Selections.LocalOnline:
        this.mShowLocalBoards = !this.mShowLocalBoards;
        if (this.mShowLocalBoards)
          (this.mMenuItems[1] as MenuTextItem).SetText(LanguageManager.Instance.GetString(SubMenuLeaderboards.LOC_LOCAL));
        else
          (this.mMenuItems[1] as MenuTextItem).SetText(LanguageManager.Instance.GetString(SubMenuLeaderboards.LOC_ONLINE));
        this.mMenuItems[1].Selected = true;
        this.mCurrentTopRank = 0;
        this.ChangeLeaderboard();
        break;
      case SubMenuLeaderboards.Selections.List:
        int num1 = Math.Max(0, this.mCurrentTopRank - 8);
        if (this.mShowLocalBoards)
        {
          if (StatisticsManager.Instance.Leaderboard(this.mChallenge).Count <= num1)
            break;
          this.mCurrentTopRank = num1;
          this.UpdateScores();
          break;
        }
        if (this.mSteamLeaderboards.Count <= this.mChallenge || SteamUserStats.GetLeaderboardEntryCount(this.mSteamLeaderboards[this.mChallenge]) <= num1)
          break;
        this.mCurrentTopRank = num1;
        this.UpdateScores();
        break;
      case SubMenuLeaderboards.Selections.Level:
        this.mCurrentTopRank = 0;
        int num2 = this.mChallenge + 1;
        if (num2 >= LevelManager.Instance.Challenges.Length)
          num2 = 0;
        this.mChallenge = num2;
        this.ChangeLeaderboard();
        this.mMenuItems[0].Selected = !(iSender is KeyboardMouseController);
        break;
      default:
        this.mMenuItems[2].Selected = true;
        break;
    }
  }

  public override void ControllerRight(Controller iSender)
  {
    this.mMenuItems[0].Selected = false;
    this.mMenuItems[1].Selected = false;
    this.mMenuItems[2].Selected = false;
    switch (this.mSelection)
    {
      case SubMenuLeaderboards.Selections.LocalOnline:
        this.mShowLocalBoards = !this.mShowLocalBoards;
        if (this.mShowLocalBoards)
          (this.mMenuItems[1] as MenuTextItem).SetText(LanguageManager.Instance.GetString(SubMenuLeaderboards.LOC_LOCAL));
        else
          (this.mMenuItems[1] as MenuTextItem).SetText(LanguageManager.Instance.GetString(SubMenuLeaderboards.LOC_ONLINE));
        this.mCurrentTopRank = 0;
        this.ChangeLeaderboard();
        this.mMenuItems[1].Selected = true;
        break;
      case SubMenuLeaderboards.Selections.List:
        int num1 = Math.Max(0, this.mCurrentTopRank + 8);
        if (this.mShowLocalBoards)
        {
          if (StatisticsManager.Instance.Leaderboard(this.mChallenge).Count <= num1)
            break;
          this.mCurrentTopRank = num1;
          this.UpdateScores();
          break;
        }
        if (this.mSteamLeaderboards.Count <= this.mChallenge || SteamUserStats.GetLeaderboardEntryCount(this.mSteamLeaderboards[this.mChallenge]) <= num1)
          break;
        this.mCurrentTopRank = num1;
        this.UpdateScores();
        break;
      case SubMenuLeaderboards.Selections.Level:
        this.mCurrentTopRank = 0;
        int num2 = this.mChallenge - 1;
        if (num2 < 0)
          num2 = LevelManager.Instance.Challenges.Length - 1;
        this.mChallenge = num2;
        this.ChangeLeaderboard();
        this.mMenuItems[0].Selected = !(iSender is KeyboardMouseController);
        break;
      default:
        this.mMenuItems[2].Selected = true;
        break;
    }
  }

  public override void ControllerUp(Controller iSender)
  {
    this.mMenuItems[0].Selected = false;
    this.mMenuItems[1].Selected = false;
    this.mMenuItems[2].Selected = false;
    switch (this.mSelection)
    {
      case SubMenuLeaderboards.Selections.LocalOnline:
        if (!this.mShowLocalBoards)
        {
          this.mSelection = SubMenuLeaderboards.Selections.List;
          break;
        }
        this.mSelection = SubMenuLeaderboards.Selections.Level;
        this.mMenuItems[0].Selected = true;
        break;
      case SubMenuLeaderboards.Selections.List:
        this.mSelection = SubMenuLeaderboards.Selections.Level;
        this.mMenuItems[0].Selected = true;
        break;
      case SubMenuLeaderboards.Selections.Level:
        this.mSelection = SubMenuLeaderboards.Selections.Back;
        this.mMenuItems[2].Selected = true;
        break;
      default:
        this.mSelection = SubMenuLeaderboards.Selections.LocalOnline;
        this.mMenuItems[1].Selected = true;
        break;
    }
  }

  public override void ControllerDown(Controller iSender)
  {
    this.mMenuItems[0].Selected = false;
    this.mMenuItems[1].Selected = false;
    this.mMenuItems[2].Selected = false;
    switch (this.mSelection)
    {
      case SubMenuLeaderboards.Selections.LocalOnline:
        this.mSelection = SubMenuLeaderboards.Selections.Back;
        this.mMenuItems[2].Selected = true;
        break;
      case SubMenuLeaderboards.Selections.List:
        this.mSelection = SubMenuLeaderboards.Selections.LocalOnline;
        this.mMenuItems[1].Selected = true;
        break;
      case SubMenuLeaderboards.Selections.Level:
        if (!this.mShowLocalBoards)
        {
          this.mSelection = SubMenuLeaderboards.Selections.List;
          break;
        }
        this.mSelection = SubMenuLeaderboards.Selections.LocalOnline;
        this.mMenuItems[1].Selected = true;
        break;
      default:
        this.mSelection = SubMenuLeaderboards.Selections.Level;
        this.mMenuItems[0].Selected = true;
        break;
    }
  }

  public override void ControllerA(Controller iSender)
  {
    switch (this.mSelection)
    {
      case SubMenuLeaderboards.Selections.Back:
        Tome.Instance.PopMenu();
        break;
      default:
        this.ControllerRight(iSender);
        break;
    }
  }

  private void LoadTexture()
  {
    if (this.mCurrentTexture != null)
    {
      lock (this.mCurrentTexture)
      {
        if (this.mCurrentContent == null)
          this.mCurrentContent = new ContentManager(Magicka.Game.Instance.Content.ServiceProvider, Magicka.Game.Instance.Content.RootDirectory);
        else
          this.mCurrentContent.Unload();
        this.mCurrentTexture = this.mCurrentContent.Load<Texture2D>(this.mCurrentTexturePath);
      }
    }
    else
    {
      if (this.mCurrentContent == null)
        this.mCurrentContent = new ContentManager(Magicka.Game.Instance.Content.ServiceProvider, Magicka.Game.Instance.Content.RootDirectory);
      else
        this.mCurrentContent.Unload();
      this.mCurrentTexture = this.mCurrentContent.Load<Texture2D>(this.mCurrentTexturePath);
    }
  }

  private void ChangeLeaderboard()
  {
    string currentTexturePath = this.mCurrentTexturePath;
    LevelNode challenge = LevelManager.Instance.Challenges[this.mChallenge];
    (this.mMenuItems[0] as MenuTextItem).SetText(LanguageManager.Instance.GetString(challenge.Name.GetHashCodeCustom()));
    this.mCurrentTexturePath = this.mTexturePath.Replace("#1;", challenge.LoadingImage);
    if (LevelManager.Instance.Challenges[this.mChallenge].FileName.Equals("ch_vietnam", StringComparison.OrdinalIgnoreCase))
      this.mWavesHeader.SetText(LanguageManager.Instance.GetString(SubMenuLeaderboards.LOC_TITLE_TIME));
    else
      this.mWavesHeader.SetText(LanguageManager.Instance.GetString(SubMenuLeaderboards.LOC_WAVES));
    if (currentTexturePath == null || !currentTexturePath.Equals(this.mCurrentTexturePath, StringComparison.InvariantCultureIgnoreCase))
    {
      this.mTextureAlpha = 0.0f;
      Magicka.Game.Instance.AddLoadTask(new Action(this.LoadTexture));
    }
    this.mVisibleRanks = 0;
    this.mRankColumn.Clear();
    this.mNameColumn.Clear();
    this.mWavesColumn.Clear();
    this.mScoreColumn.Clear();
    this.UpdateScores();
  }

  private void OnlineFindResult(LeaderboardFindResult iResult)
  {
  }

  private void OnlineScoresDownloaded(LeaderboardScoresDownloaded iDownloaded)
  {
    bool flag1 = false;
    if (LevelManager.Instance.Challenges[this.mChallenge].RulesetType == Rulesets.TimedObjective)
      flag1 = true;
    this.mRankStringBuilder.Remove(0, this.mRankStringBuilder.Length);
    this.mNameStringBuilder.Remove(0, this.mNameStringBuilder.Length);
    this.mScoreStringBuilder.Remove(0, this.mScoreStringBuilder.Length);
    this.mDataStringBuilder.Remove(0, this.mDataStringBuilder.Length);
    this.mVisibleRanks = iDownloaded.mEntryCount;
    for (int index = 0; index < iDownloaded.mEntryCount; ++index)
    {
      int[] pDetails = new int[1];
      LeaderboardEntry pLeaderboardEntry;
      if (SteamUserStats.GetDownloadedLeaderboardEntry(iDownloaded.mSteamLeaderboardEntries, index, out pLeaderboardEntry, pDetails))
      {
        this.mRankStringBuilder.Append(pLeaderboardEntry.GlobalRank);
        float num = (float) ((double) SubMenuLeaderboards.WAVES_POSITION.X - (double) SubMenuLeaderboards.NAMES_POSITION.X + 12.0);
        bool flag2 = false;
        string iText;
        for (iText = SteamFriends.GetFriendPersonaName(pLeaderboardEntry.SteamIDUser); (double) this.mNameColumn.Font.MeasureText(iText, true).X > (double) num; iText = iText.Remove(iText.Length - 1, 1))
          flag2 = true;
        if (flag2)
        {
          this.mNameStringBuilder.Append(iText.Remove(iText.Length - 1, 1));
          this.mNameStringBuilder.Append("...");
        }
        else
          this.mNameStringBuilder.Append(iText);
        this.mScoreStringBuilder.Append(pLeaderboardEntry.Score);
        if (flag1)
        {
          FloatIntConverter floatIntConverter = new FloatIntConverter(pDetails[pLeaderboardEntry.Details - 1]);
          TimeSpan timeSpan = TimeSpan.FromSeconds((double) floatIntConverter.Float);
          if ((double) floatIntConverter.Float >= 60.0 && (double) floatIntConverter.Float < 3600.0)
            this.mDataStringBuilder.Append($"0:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}");
          else if ((double) floatIntConverter.Float >= 3600.0)
            this.mDataStringBuilder.Append($"{timeSpan.Hours:0}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}");
          else
            this.mDataStringBuilder.Append($"0:00:{timeSpan.Seconds:00}");
        }
        else
          this.mDataStringBuilder.Append(pDetails[pLeaderboardEntry.Details - 1]);
        this.mRankStringBuilder.Append("\n");
        this.mNameStringBuilder.Append("\n");
        this.mScoreStringBuilder.Append("\n");
        this.mDataStringBuilder.Append("\n");
      }
    }
    this.mRankColumn.SetText(this.mRankStringBuilder.ToString());
    this.mNameColumn.SetText(this.mNameStringBuilder.ToString());
    this.mScoreColumn.SetText(this.mScoreStringBuilder.ToString());
    this.mWavesColumn.SetText(this.mDataStringBuilder.ToString());
  }

  private void UpdateScores()
  {
    if (!this.mShowLocalBoards)
    {
      if (this.mSteamLeaderboards.Count <= this.mChallenge)
        return;
      this.mRankColumn.SetText("");
      this.mNameColumn.SetText("");
      this.mWavesColumn.SetText("");
      this.mScoreColumn.SetText("");
      SteamUserStats.DownloadLeaderboardEntries(this.mSteamLeaderboards[this.mChallenge], LeaderboardDataRequest.Global, this.mCurrentTopRank + 1, this.mCurrentTopRank + 8, new Action<LeaderboardScoresDownloaded>(this.OnlineScoresDownloaded));
      this.mListBottom.Clear();
      this.mListBottom.Append(this.mCurrentTopRank + 8);
      this.mListTop.Clear();
      this.mListTop.Append(this.mCurrentTopRank + 1);
    }
    else
    {
      List<LeaderBoardData> leaderBoardDataList = StatisticsManager.Instance.Leaderboard(this.mChallenge);
      if (this.mCurrentTopRank >= leaderBoardDataList.Count && leaderBoardDataList.Count > 0)
      {
        this.mCurrentTopRank -= 8;
      }
      else
      {
        bool flag1 = false;
        if (LevelManager.Instance.Challenges[this.mChallenge].RulesetType == Rulesets.TimedObjective)
          flag1 = true;
        int num1 = Math.Min(this.mCurrentTopRank, leaderBoardDataList.Count);
        int num2 = Math.Min(8, leaderBoardDataList.Count - num1);
        this.mRankStringBuilder.Remove(0, this.mRankStringBuilder.Length);
        this.mNameStringBuilder.Remove(0, this.mNameStringBuilder.Length);
        this.mScoreStringBuilder.Remove(0, this.mScoreStringBuilder.Length);
        this.mDataStringBuilder.Remove(0, this.mDataStringBuilder.Length);
        float num3 = SubMenuLeaderboards.WAVES_POSITION.X - SubMenuLeaderboards.NAMES_POSITION.X;
        this.mVisibleRanks = Math.Min(this.mCurrentTopRank + num2, leaderBoardDataList.Count);
        int num4 = 0;
        int index = num1;
        while (index < this.mVisibleRanks)
        {
          this.mRankStringBuilder.Append(num4 + 1);
          bool flag2 = false;
          string iText;
          for (iText = leaderBoardDataList[index].Name; (double) this.mNameColumn.Font.MeasureText(iText, true).X > (double) num3; iText = iText.Remove(iText.Length - 1, 1))
            flag2 = true;
          if (flag2)
          {
            this.mNameStringBuilder.Append(iText.Remove(iText.Length - 1, 1));
            this.mNameStringBuilder.Append("...");
          }
          else
            this.mNameStringBuilder.Append(iText);
          this.mScoreStringBuilder.Append(leaderBoardDataList[index].Score);
          if (flag1)
          {
            FloatIntConverter floatIntConverter = new FloatIntConverter(leaderBoardDataList[index].Data1);
            TimeSpan timeSpan = TimeSpan.FromSeconds((double) floatIntConverter.Float);
            if ((double) floatIntConverter.Float >= 60.0 && (double) floatIntConverter.Float < 3600.0)
              this.mDataStringBuilder.Append($"0:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}");
            else if ((double) floatIntConverter.Float >= 3600.0)
              this.mDataStringBuilder.Append($"{timeSpan.Hours:0}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}");
            else
              this.mDataStringBuilder.Append($"0:00:{timeSpan.Seconds:00}");
          }
          else
            this.mDataStringBuilder.Append(leaderBoardDataList[index].Data1);
          this.mRankStringBuilder.Append("\n");
          this.mNameStringBuilder.Append("\n");
          this.mScoreStringBuilder.Append("\n");
          this.mDataStringBuilder.Append("\n");
          ++index;
          ++num4;
        }
        this.mRankColumn.SetText(this.mRankStringBuilder.ToString());
        this.mNameColumn.SetText(this.mNameStringBuilder.ToString());
        this.mWavesColumn.SetText(this.mDataStringBuilder.ToString());
        this.mScoreColumn.SetText(this.mScoreStringBuilder.ToString());
      }
    }
  }

  public override void OnEnter()
  {
    GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Bottom, GamePadMenuHelp.LOC_SELECT);
    GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Right, GamePadMenuHelp.LOC_BACK);
    GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Left);
    GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Top);
    base.OnEnter();
    for (int index = 0; index < this.mMenuItems.Count; ++index)
      this.mMenuItems[index].Selected = false;
    this.mSelection = SubMenuLeaderboards.Selections.Level;
    this.mChallenge = 0;
    this.ChangeLeaderboard();
  }

  private enum Selections
  {
    None,
    Back,
    LocalOnline,
    List,
    Level,
  }
}
