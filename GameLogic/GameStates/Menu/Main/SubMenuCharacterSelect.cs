// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.GameStates.Menu.Main.SubMenuCharacterSelect
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Achievements;
using Magicka.DRM;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.UI;
using Magicka.Gamers;
using Magicka.Graphics;
using Magicka.Levels;
using Magicka.Levels.Campaign;
using Magicka.Levels.Packs;
using Magicka.Levels.Versus;
using Magicka.Localization;
using Magicka.Misc;
using Magicka.Network;
using Magicka.Storage;
using Magicka.WebTools.Paradox;
using Magicka.WebTools.Paradox.Telemetry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PolygonHead;
using PolygonHead.Effects;
using SteamWrapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

#nullable disable
namespace Magicka.GameLogic.GameStates.Menu.Main;

internal class SubMenuCharacterSelect : SubMenu
{
  private const float OFFSETX = 1008f;
  private const float OFFSETY = 224f;
  private const float SIZE = 128f;
  private const float MARGIN = 16f;
  private const float MAX_GAMERTAG_WIDTH = 190f;
  private const float LOCKED_ITEM_SATURATION = 0.75f;
  private const int MAXVISIBLEGAMERS = 4;
  private const int SELECTION_PLAYER0 = 0;
  private const int SELECTION_PLAYER1 = 1;
  private const int SELECTION_PLAYER2 = 2;
  private const int SELECTION_PLAYER3 = 3;
  private const int SELECTION_START = 4;
  private const int SELECTION_PACKS = 5;
  private const int SELECTION_LEVEL = 6;
  private const int SELECTION_CHAT = -1;
  private const int SELECTION_GAME_MODE = 7;
  private const int SELECTION_VS_SETTINGS = 8;
  private const int SELECTION_CHAPTER = 9;
  private const int MAX_PACK_ROWS = 7;
  private const float PACK_THUMB_SPACING = 64f;
  private const int MAX_VISIBLE_PACKS = 5;
  private const float PACK_POS_X = 64f;
  private const float PACK_POS_Y = 408f;
  private const int MAX_VISIBLE_LEVELS = 6;
  private const float LEVEL_SPACING = 140f;
  private const int LEVEL_POS_Y = 64 /*0x40*/;
  private const int LEVEL_POS_X = 64 /*0x40*/;
  private const float LEVEL_LOWEST_POSY_Y = 904f;
  private const float AVATAR_WIDTH = 96f;
  private const float COLOR_SPACING = 34f;
  private const int COLUMN_WIDTH = 448;
  private const int LEVEL_NAME_TARGET_LINE_WIDTH = 560;
  private const int LEVEL_DESC_TARGET_LINE_WIDTH = 940;
  private const int LEVEL_DESC_TARGET_LINE_WIDTH_CHAPTER = 560;
  private const float SLOT_HEIGHT = 112f;
  private const float SLOT_SPACING = 25f;
  private const float THUMB_SLOT_SIZE = 96f;
  private const float SLOT_POS_X = 544f;
  private const float PLAYER_SLOT_POS_Y = 89f;
  private const float SPECTATOR_SLOT_POS_Y = 593f;
  private const float GAMER_SPACING = 31f;
  private const float CHAT_POS_X = 544f;
  private const float CHAT_POS_Y = 710f;
  private const int CHAT_SIZE_X = 448;
  private const int CHAT_SIZE_Y = 185;
  private const float READY_OFFSET_X = 48f;
  private const float READY_WIDTH = 128f;
  private const float READY_HEIGHT = 80f;
  private const float CONTROLLER_ICON_WIDTH = 128f;
  private const float CONTROLLER_ICON_HEIGHT = 64f;
  private const float CONTROLLER_ICON_OFFSET_X = -108.8f;
  private const float CONTROLLER_ICON_OFFSET_Y = -54.4f;
  private const int MAX_VISIBLE_SETTINGS = 5;
  protected static readonly string[] dungeonLevelFileNames = new string[2]
  {
    "ch_dungeons_ch1",
    "ch_dungeons_ch2"
  };
  protected static readonly int LOC_LOADING = "#network_23".GetHashCodeCustom();
  protected static readonly int LOC_SELECT_CHARACTER = "#menu_charslct_01".GetHashCodeCustom();
  protected static readonly int LOC_READY = "#menu_charslct_02".GetHashCodeCustom();
  protected static readonly int LOC_CHANGE_COLOR = "#menu_charslct_03".GetHashCodeCustom();
  protected static readonly int LOC_JOIN = "#add_menu_join".GetHashCodeCustom();
  protected static readonly int LOC_NEW = "#add_menu_prof_new".GetHashCodeCustom();
  protected static readonly int LOC_LEVEL = "#menu_vs_13".GetHashCodeCustom();
  protected static readonly int LOC_GAMEMODE = "#opt_vs_mode".GetHashCodeCustom();
  protected static readonly int LOC_PACKS = "#menu_enabledpacks".GetHashCodeCustom();
  protected static readonly int LOC_MAGICKPACKS = "#menu_tome_02".GetHashCodeCustom();
  protected static readonly int LOC_ITEMPACKS = "#menu_tome_03".GetHashCodeCustom();
  protected static readonly int LOC_LATENCY = "#menu_latency".GetHashCodeCustom();
  protected new static readonly int LOC_ENTER_NAME = "#add_menu_prof_name".GetHashCodeCustom();
  protected static readonly int LOC_COUNTDOWN = "#menu_countdown".GetHashCodeCustom();
  protected static readonly int LOC_COUNTDOWN_ABORTED = "#menu_countdown_abort".GetHashCodeCustom();
  protected static readonly int LOC_TT_CHANGE_LEVEL = "#tooltip_lobby_level".GetHashCodeCustom();
  protected static readonly int LOC_TT_CUSTOMIZE = "#tooltip_lobby_customize".GetHashCodeCustom();
  protected static readonly int LOC_TT_GAME_MODE = "#tooltip_vs_mode".GetHashCodeCustom();
  protected static readonly int LOC_KICK = "#network_06".GetHashCodeCustom();
  protected static readonly int LOC_KICKED = "#add_menu_not_kicked".GetHashCodeCustom();
  protected static readonly int LOC_CHATMESSAGE = "#menu_chat_message".GetHashCodeCustom();
  protected static readonly string sServerChangingLevelText = LanguageManager.Instance.GetString(SubMenuOnline.LOC_NO_LEVEL_SELECTED);
  private static SubMenuCharacterSelect sSingelton;
  private static volatile object sSingeltonLock = new object();
  private static Vector4 MENU_COLOR_LIGHT_GRAY = new Vector4(1f, 1f, 1f, 1f);
  private SubMenuCharacterSelect.State mCurrentState;
  private float mOptionsAlpha = 1f;
  private float mLevelSelectAlpha;
  private float mPackSelectAlpha;
  private bool HasSelectedLevel = true;
  private GameInfoMessage mGameSettings;
  private bool mCustomLevel;
  private Texture2D mCustomLevelOverlay;
  private static readonly Vector4 LEVEL_DESCR_COLOR_NONCAMP = new Vector4(0.0f, 0.0f, 0.0f, 1f);
  private Texture2D mLevelLockedOverlay;
  private Texture2D mLevelUnusedOverlay;
  private Texture2D mRobeLockedOverlay;
  private Texture2D mRobeUnusedOverlay;
  private Texture2D mRobeFreeOverlay;
  private Texture2D mRobeFreeAndLockedOverlay;
  private Texture2D mRobeFreeAndUnusedOverlay;
  private Texture2D mRobeNewOverlay;
  private Texture2D mLevelFreeOverlay;
  private Texture2D mLevelFreeAndLockedOverlay;
  private Texture2D mLevelFreeAndUnusedOverlay;
  private Texture2D mLevelNewOverlay;
  private List<Text> mGamerItems;
  private MenuImageTextItem mBackButton;
  private MenuTextButtonItem mStartButton;
  private MenuImageTextItem mGenericStar;
  protected static Texture2D sSelectLevelButtonFrame;
  protected static Texture2D sStarTexture;
  private MenuTextButtonItem mSelectLevelButton;
  private BitmapFont mFont;
  private BitmapFont mGamerFont;
  private VertexDeclaration mVertexDeclaration;
  private VertexBuffer mVertexBuffer;
  private IndexBuffer mIndexBuffer;
  private ContextMenu mGamerDropDownMenu;
  private ContextMenu mAdminDropDownMenu;
  private MenuScrollBar mSpecialScrollBar;
  private Controller mCurrentController;
  private Text mChapterName;
  private Text mChapterDescription;
  private Texture2D mMapTexture;
  private Texture2D mMapMaskTexture;
  private Rectangle mMapRect;
  private Text mItemsText;
  private Text mMagicksText;
  private Text mPacksText;
  private int mSelectedPack = -1;
  private ControllerDirection mSelectedPackScroll;
  private int mPackScrollValue;
  private Texture2D mMagicksTexture;
  private static Texture2D[] mControllerTextures;
  public static readonly Vector2 SELECT_LEVEL_BUTTON_POS = new Vector2(129f, 560f);
  private ContentManager mLevelContent;
  private Text mLoadingText;
  private bool mValidatingLevels;
  private bool mLoadingLevels;
  private Controller mNameInputController;
  private TextInputMessageBox mNameInputBox;
  private VertexBuffer mAvatarVertices;
  private VertexDeclaration mAvatarVertexDeclaration;
  private Texture2D mCustomTexture;
  private bool mDefaultAvatars;
  private MenuScrollBar[] mGamerScrollBars;
  private Text mOpenText;
  private Text mClosedText;
  private SubMenuCharacterSelect.PlayerState[] mPlayerSlots;
  private MenuTextButtonItem mPackCloseButton;
  private DropDownBox<Rulesets> mGameModeBox;
  private Text mGameModeTitle;
  private VersusRuleset.Settings mVersusSettings;
  private Text[] mVersusSettingsTitles;
  private MenuScrollBar mSettingsScrollbar;
  private OptionsMessageBox mCheckPointRUSure;
  private int mLastCountDownNr = -1;
  private float mCountDown;
  private int mLevelToSet;
  private string mLevelNameToFocusWhenLevelComplete = "";
  private static List<SubMenuCharacterSelect.LevelRep> mLevelRepresentations = (List<SubMenuCharacterSelect.LevelRep>) null;
  private static List<SubMenuCharacterSelect.RobeRep> mRobeRepresentations = (List<SubMenuCharacterSelect.RobeRep>) null;

  public static SubMenuCharacterSelect Instance
  {
    get
    {
      if (SubMenuCharacterSelect.sSingelton == null)
      {
        lock (SubMenuCharacterSelect.sSingeltonLock)
        {
          if (SubMenuCharacterSelect.sSingelton == null)
            SubMenuCharacterSelect.sSingelton = new SubMenuCharacterSelect();
        }
      }
      return SubMenuCharacterSelect.sSingelton;
    }
  }

  public SubMenuCharacterSelect()
  {
    LanguageManager instance = LanguageManager.Instance;
    this.mFont = FontManager.Instance.GetFont(MagickaFont.MenuOption);
    this.mPlayerSlots = new SubMenuCharacterSelect.PlayerState[4];
    GraphicsDevice graphicsDevice = Magicka.Game.Instance.GraphicsDevice;
    lock (graphicsDevice)
    {
      this.mCustomTexture = Magicka.Game.Instance.Content.Load<Texture2D>("UI/menu/customAvatar");
      this.mMagicksTexture = Magicka.Game.Instance.Content.Load<Texture2D>("UI/Menu/Magicks");
      this.mMapTexture = Magicka.Game.Instance.Content.Load<Texture2D>("UI/Menu/CampaignMap");
      this.mMapMaskTexture = Magicka.Game.Instance.Content.Load<Texture2D>("UI/Menu/MapMask");
      SubMenuCharacterSelect.mControllerTextures = new Texture2D[5];
      SubMenuCharacterSelect.mControllerTextures[0] = Magicka.Game.Instance.Content.Load<Texture2D>("UI/Menu/ControllerIcons/keyboard");
      SubMenuCharacterSelect.mControllerTextures[1] = Magicka.Game.Instance.Content.Load<Texture2D>("UI/Menu/ControllerIcons/gp1");
      SubMenuCharacterSelect.mControllerTextures[2] = Magicka.Game.Instance.Content.Load<Texture2D>("UI/Menu/ControllerIcons/gp2");
      SubMenuCharacterSelect.mControllerTextures[3] = Magicka.Game.Instance.Content.Load<Texture2D>("UI/Menu/ControllerIcons/gp3");
      SubMenuCharacterSelect.mControllerTextures[4] = Magicka.Game.Instance.Content.Load<Texture2D>("UI/Menu/ControllerIcons/gp4");
    }
    this.mMapRect = new Rectangle();
    this.mMapRect.Width = this.mMapRect.Height = 448;
    this.mMapRect.X = 64 /*0x40*/;
    this.mMapRect.Y = 224 /*0xE0*/;
    this.mValidatingLevels = false;
    Vector4[] vector4Array = new Vector4[64 /*0x40*/];
    Vector2 iMargin = new Vector2();
    iMargin.X = iMargin.Y = 16f;
    Vector2 iSize1 = new Vector2();
    iSize1.X = 448f;
    iSize1.Y = 112f;
    Vector2 iSize2 = new Vector2();
    iSize2.X = iSize2.Y = 96f;
    Vector2 iUVOffset = new Vector2();
    iUVOffset.X = 832f / (float) SubMenu.sPagesTexture.Width;
    iUVOffset.Y = 128f / (float) SubMenu.sPagesTexture.Height;
    Vector2 iUVSize = new Vector2();
    iUVSize.X = 128f / (float) SubMenu.sPagesTexture.Width;
    iUVSize.Y = 128f / (float) SubMenu.sPagesTexture.Height;
    Vector2 iUVMargin = new Vector2();
    iUVMargin.X = 16f / (float) SubMenu.sPagesTexture.Width;
    iUVMargin.Y = 16f / (float) SubMenu.sPagesTexture.Height;
    int vertices1 = SubMenuCharacterSelect.CreateVertices(vector4Array, 0, ref iSize1, ref iMargin, ref iUVOffset, ref iUVSize, ref iUVMargin);
    int vertices2 = SubMenuCharacterSelect.CreateVertices(vector4Array, vertices1, ref iSize2, ref iMargin, ref iUVOffset, ref iUVSize, ref iUVMargin);
    iSize2.X = 320f;
    iSize2.Y = 144f;
    int vertices3 = SubMenuCharacterSelect.CreateVertices(vector4Array, vertices2, ref iSize2, ref iMargin, ref iUVOffset, ref iUVSize, ref iUVMargin);
    Vector2 vector2_1 = new Vector2(64f / (float) SubMenu.sPagesTexture.Width, 64f / (float) SubMenu.sPagesTexture.Height);
    Vector2 vector2_2 = new Vector2(1408f / (float) SubMenu.sPagesTexture.Width, 32f / (float) SubMenu.sPagesTexture.Height);
    vector4Array[vertices3].X = -0.5f;
    vector4Array[vertices3].Y = -0.5f;
    vector4Array[vertices3].Z = vector2_2.X;
    vector4Array[vertices3].W = vector2_2.Y;
    int index1 = vertices3 + 1;
    vector4Array[index1].X = 0.5f;
    vector4Array[index1].Y = -0.5f;
    vector4Array[index1].Z = vector2_2.X + vector2_1.X;
    vector4Array[index1].W = vector2_2.Y;
    int index2 = index1 + 1;
    vector4Array[index2].X = 0.5f;
    vector4Array[index2].Y = 0.5f;
    vector4Array[index2].Z = vector2_2.X + vector2_1.X;
    vector4Array[index2].W = vector2_2.Y + vector2_1.Y;
    int index3 = index2 + 1;
    vector4Array[index3].X = -0.5f;
    vector4Array[index3].Y = 0.5f;
    vector4Array[index3].Z = vector2_2.X;
    vector4Array[index3].W = vector2_2.Y + vector2_1.Y;
    int index4 = index3 + 1;
    vector2_2 = new Vector2(896f / (float) SubMenu.sPagesTexture.Width, 320f / (float) SubMenu.sPagesTexture.Height);
    vector4Array[index4].X = -0.5f;
    vector4Array[index4].Y = -0.5f;
    vector4Array[index4].Z = vector2_2.X;
    vector4Array[index4].W = vector2_2.Y;
    int index5 = index4 + 1;
    vector4Array[index5].X = 0.5f;
    vector4Array[index5].Y = -0.5f;
    vector4Array[index5].Z = vector2_2.X + vector2_1.X;
    vector4Array[index5].W = vector2_2.Y;
    int index6 = index5 + 1;
    vector4Array[index6].X = 0.5f;
    vector4Array[index6].Y = 0.5f;
    vector4Array[index6].Z = vector2_2.X + vector2_1.X;
    vector4Array[index6].W = vector2_2.Y + vector2_1.Y;
    int index7 = index6 + 1;
    vector4Array[index7].X = -0.5f;
    vector4Array[index7].Y = 0.5f;
    vector4Array[index7].Z = vector2_2.X;
    vector4Array[index7].W = vector2_2.Y + vector2_1.Y;
    int index8 = index7 + 1;
    vector4Array[index8].X = -0.5f;
    vector4Array[index8].Y = -0.5f;
    vector4Array[index8].Z = 0.0f;
    vector4Array[index8].W = 0.0f;
    int index9 = index8 + 1;
    vector4Array[index9].X = 0.5f;
    vector4Array[index9].Y = -0.5f;
    vector4Array[index9].Z = 1f;
    vector4Array[index9].W = 0.0f;
    int index10 = index9 + 1;
    vector4Array[index10].X = 0.5f;
    vector4Array[index10].Y = 0.5f;
    vector4Array[index10].Z = 1f;
    vector4Array[index10].W = 1f;
    int index11 = index10 + 1;
    vector4Array[index11].X = -0.5f;
    vector4Array[index11].Y = 0.5f;
    vector4Array[index11].Z = 0.0f;
    vector4Array[index11].W = 1f;
    int num = index11 + 1;
    lock (graphicsDevice)
    {
      this.mVertexDeclaration = RenderManager.Instance.CreateVertexDeclaration(new VertexElement[2]
      {
        new VertexElement((short) 0, (short) 0, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.Position, (byte) 0),
        new VertexElement((short) 0, (short) 8, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, (byte) 0)
      });
      this.mVertexBuffer = new VertexBuffer(graphicsDevice, vector4Array.Length * 4 * 4, BufferUsage.WriteOnly);
      this.mVertexBuffer.SetData<Vector4>(vector4Array);
      this.mIndexBuffer = new IndexBuffer(graphicsDevice, TextBox.INDICES.Length * 2, BufferUsage.WriteOnly, IndexElementSize.SixteenBits);
      this.mIndexBuffer.SetData<ushort>(TextBox.INDICES);
    }
    this.mOpenText = new Text(48 /*0x30*/, this.mFont, TextAlign.Left, false);
    this.mOpenText.SetText(instance.GetString(SubMenuCharacterSelect.LOC_JOIN));
    this.mClosedText = new Text(48 /*0x30*/, this.mFont, TextAlign.Left, false);
    this.mClosedText.SetText(instance.GetString(0));
    this.ResetLevelTexts();
    this.mLoadingText = new Text(64 /*0x40*/, FontManager.Instance.GetFont(MagickaFont.MenuTitle), TextAlign.Center, false);
    this.mLoadingText.SetText(LanguageManager.Instance.GetString(SubMenuCharacterSelect.LOC_LOADING));
    this.mGamerDropDownMenu = new ContextMenu(this.mFont, TextAlign.Right, new int?(200));
    this.mAdminDropDownMenu = new ContextMenu(this.mFont, TextAlign.Right, new int?(200));
    this.mAdminDropDownMenu.AddOption(SubMenuCharacterSelect.LOC_KICK);
    this.mGameModeBox = new DropDownBox<Rulesets>(this.mFont, new Rulesets[3]
    {
      Rulesets.DeathMatch,
      Rulesets.Brawl,
      Rulesets.Kreitor
    }, new int?[3]
    {
      new int?("#versus_dm".GetHashCodeCustom()),
      new int?("#versus_brawl".GetHashCodeCustom()),
      new int?("#versus_tourney".GetHashCodeCustom())
    }, 250);
    this.mGameModeBox.Position = new Vector2(512f - this.mGameModeBox.Size.X, 89f - (float) this.mFont.LineHeight);
    this.mGameModeBox.ValueChanged += new Action<DropDownBox, Rulesets>(this.mGameModeBox_ValueChanged);
    this.mGameModeTitle = new Text(48 /*0x30*/, this.mFont, TextAlign.Left, false);
    this.mGameModeTitle.SetText(instance.GetString(SubMenuCharacterSelect.LOC_GAMEMODE));
    this.mSettingsScrollbar = new MenuScrollBar(new Vector2(), 170f, 0);
    this.mGameModeBox_ValueChanged((DropDownBox) this.mGameModeBox, this.mGameModeBox.SelectedValue);
    this.mPacksText = new Text(64 /*0x40*/, this.mFont, TextAlign.Left, false);
    this.mItemsText = new Text(64 /*0x40*/, this.mFont, TextAlign.Left, false);
    this.mMagicksText = new Text(64 /*0x40*/, this.mFont, TextAlign.Left, false);
    this.mPacksText.SetText(LanguageManager.Instance.GetString(SubMenuCharacterSelect.LOC_PACKS));
    this.mItemsText.SetText(LanguageManager.Instance.GetString(SubMenuCharacterSelect.LOC_ITEMPACKS));
    this.mMagicksText.SetText(LanguageManager.Instance.GetString(SubMenuCharacterSelect.LOC_MAGICKPACKS));
    this.mPosition.X = 128f;
    this.mPosition.Y = (float) Tome.PAGERIGHTSHEET.Height - 128f;
    this.mBackButton = new MenuImageTextItem(SubMenu.BACK_POSITION, SubMenu.sPagesTexture, SubMenu.BACK_UVOFFSET, SubMenu.BACK_UVSCALE, 0, new Vector2(), SubMenu.BACK_TEXT_ALIGN, FontManager.Instance.GetFont(SubMenu.BACK_FONT), SubMenu.BACK_SIZE);
    this.mPosition.X = (float) Tome.PAGERIGHTSHEET.Width - 128f;
    this.mPosition.Y = (float) Tome.PAGERIGHTSHEET.Height - 128f;
    this.mStartButton = new MenuTextButtonItem(this.mPosition, SubMenu.sPagesTexture, MenuTextButtonItem.DEFAULT_UV_OFFSET, MenuTextButtonItem.DEFAULT_SIZE, SubMenu.LOC_START, this.mFont, 200f, TextAlign.Right);
    this.mMenuTitle = new Text(48 /*0x30*/, FontManager.Instance.GetFont(MagickaFont.MenuTitle), TextAlign.Center, false);
    this.mGamerItems = new List<Text>();
    this.mGamerFont = FontManager.Instance.GetFont(MagickaFont.MenuDefault);
    this.mGamerScrollBars = new MenuScrollBar[6];
    Vector2 iPosition = new Vector2();
    iPosition.X = 948f;
    iPosition.Y = 145f;
    Vector2 vector2_3 = new Vector2(-384f, 224f);
    Vector4 dialogueColorDefault = Defines.DIALOGUE_COLOR_DEFAULT;
    dialogueColorDefault.X *= 1.333f;
    dialogueColorDefault.Y *= 1.333f;
    dialogueColorDefault.Z *= 1.333f;
    int iMaxValue = Math.Max(0, this.mGamerItems.Count - 4);
    for (int index12 = 0; index12 < this.mGamerScrollBars.Length; ++index12)
    {
      this.mGamerScrollBars[index12] = new MenuScrollBar(iPosition, 190f, iMaxValue);
      this.mGamerScrollBars[index12].TextureOffset = vector2_3;
      this.mGamerScrollBars[index12].Scale = 0.75f;
      this.mGamerScrollBars[index12].Color = dialogueColorDefault;
      iPosition.Y += 144f;
    }
    this.UpdateGamers();
    for (int index13 = 0; index13 < this.mPlayerSlots.Length; ++index13)
    {
      this.mPlayerSlots[index13].State = SubMenuCharacterSelect.GamerState.Open;
      this.mPlayerSlots[index13].Team = 0;
      this.mPlayerSlots[index13].Name = new Text(32 /*0x20*/, this.mFont, TextAlign.Left, false);
      this.mPlayerSlots[index13].LatencyText = new Text(32 /*0x20*/, FontManager.Instance.GetFont(MagickaFont.Maiandra16), TextAlign.Left, true);
      this.mPlayerSlots[index13].ControllerType = -1;
    }
    lock (Magicka.Game.Instance.GraphicsDevice)
    {
      this.mCustomLevelOverlay = Magicka.Game.Instance.Content.Load<Texture2D>("UI/Menu/CustomLevel");
      this.mLevelLockedOverlay = Magicka.Game.Instance.Content.Load<Texture2D>("UI/DLCFeedback/level_locked");
      this.mLevelUnusedOverlay = Magicka.Game.Instance.Content.Load<Texture2D>("UI/DLCFeedback/level_unused");
      this.mLevelFreeOverlay = Magicka.Game.Instance.Content.Load<Texture2D>("UI/DLCFeedback/level_free");
      this.mLevelFreeAndLockedOverlay = Magicka.Game.Instance.Content.Load<Texture2D>("UI/DLCFeedback/level_locked_free");
      this.mLevelFreeAndUnusedOverlay = Magicka.Game.Instance.Content.Load<Texture2D>("UI/DLCFeedback/level_unused_free");
      this.mLevelNewOverlay = Magicka.Game.Instance.Content.Load<Texture2D>("UI/DLCFeedback/level_new");
      this.mRobeLockedOverlay = Magicka.Game.Instance.Content.Load<Texture2D>("UI/DLCFeedback/robe_locked");
      this.mRobeUnusedOverlay = Magicka.Game.Instance.Content.Load<Texture2D>("UI/DLCFeedback/robe_unused");
      this.mRobeFreeOverlay = Magicka.Game.Instance.Content.Load<Texture2D>("UI/DLCFeedback/robe_free");
      this.mRobeFreeAndLockedOverlay = Magicka.Game.Instance.Content.Load<Texture2D>("UI/DLCFeedback/robe_locked_free");
      this.mRobeFreeAndUnusedOverlay = Magicka.Game.Instance.Content.Load<Texture2D>("UI/DLCFeedback/robe_unused_free");
      this.mRobeNewOverlay = Magicka.Game.Instance.Content.Load<Texture2D>("UI/DLCFeedback/robe_new");
      if (SubMenuCharacterSelect.sSelectLevelButtonFrame == null)
        SubMenuCharacterSelect.sSelectLevelButtonFrame = Magicka.Game.Instance.Content.Load<Texture2D>("UI/DLCFeedback/select_level_frame");
      if (SubMenuCharacterSelect.sStarTexture == null)
        SubMenuCharacterSelect.sStarTexture = Magicka.Game.Instance.Content.Load<Texture2D>("UI/DLCFeedback/unused_generic");
    }
    this.mSelectLevelButton = new MenuTextButtonItem(SubMenuCharacterSelect.SELECT_LEVEL_BUTTON_POS, SubMenu.sPagesTexture, MenuTextButtonItem.DEFAULT_UV_OFFSET, MenuTextButtonItem.DEFAULT_SIZE, SubMenuCharacterSelect.LOC_TT_CHANGE_LEVEL, this.mFont, 320f, TextAlign.Left);
    this.mGenericStar = new MenuImageTextItem(Vector2.Zero, SubMenuCharacterSelect.sStarTexture, Vector2.Zero, Vector2.One, 0, Vector2.Zero, TextAlign.Center, this.mFont, new Vector2((float) SubMenuCharacterSelect.sStarTexture.Width, (float) SubMenuCharacterSelect.sStarTexture.Height));
    this.mPackCloseButton = new MenuTextButtonItem(new Vector2(), SubMenu.sPagesTexture, MenuTextButtonItem.DEFAULT_UV_OFFSET, MenuTextButtonItem.DEFAULT_SIZE, SubMenu.LOC_OK, this.mFont, 200f, TextAlign.Right);
    this.mSpecialScrollBar = new MenuScrollBar(new Vector2(), 100f, 0);
    SubMenuCharacterSelect.VertexPositionTextureTexture[] positionTextureTextureArray = new SubMenuCharacterSelect.VertexPositionTextureTexture[20];
    positionTextureTextureArray[0].Position.X = -0.5f;
    positionTextureTextureArray[0].Position.Y = -0.5f;
    positionTextureTextureArray[0].TexCoord0.X = 0.0f;
    positionTextureTextureArray[0].TexCoord0.Y = 0.0f;
    positionTextureTextureArray[0].TexCoord1.X = 0.0f;
    positionTextureTextureArray[0].TexCoord1.Y = 0.5f;
    positionTextureTextureArray[1].Position.X = 0.5f;
    positionTextureTextureArray[1].Position.Y = -0.5f;
    positionTextureTextureArray[1].TexCoord0.X = 1f;
    positionTextureTextureArray[1].TexCoord0.Y = 0.0f;
    positionTextureTextureArray[1].TexCoord1.X = 1f;
    positionTextureTextureArray[1].TexCoord1.Y = 0.5f;
    positionTextureTextureArray[2].Position.X = 0.5f;
    positionTextureTextureArray[2].Position.Y = 0.5f;
    positionTextureTextureArray[2].TexCoord0.X = 1f;
    positionTextureTextureArray[2].TexCoord0.Y = 0.5f;
    positionTextureTextureArray[2].TexCoord1.X = 1f;
    positionTextureTextureArray[2].TexCoord1.Y = 1f;
    positionTextureTextureArray[3].Position.X = -0.5f;
    positionTextureTextureArray[3].Position.Y = 0.5f;
    positionTextureTextureArray[3].TexCoord0.X = 0.0f;
    positionTextureTextureArray[3].TexCoord0.Y = 0.5f;
    positionTextureTextureArray[3].TexCoord1.X = 0.0f;
    positionTextureTextureArray[3].TexCoord1.Y = 1f;
    SubMenuCharacterSelect.CreateVertices(positionTextureTextureArray, 4, ref iSize1, ref iMargin, ref new Vector2()
    {
      X = 912f / (float) SubMenu.sPagesTexture.Width,
      Y = 512f / (float) SubMenu.sPagesTexture.Height
    }, ref new Vector2()
    {
      X = 912f / (float) SubMenu.sPagesTexture.Width,
      Y = 640f / (float) SubMenu.sPagesTexture.Height
    }, ref iUVSize, ref iUVMargin);
    lock (Magicka.Game.Instance.GraphicsDevice)
    {
      this.mAvatarVertices = new VertexBuffer(Magicka.Game.Instance.GraphicsDevice, positionTextureTextureArray.Length * 24, BufferUsage.WriteOnly);
      this.mAvatarVertices.SetData<SubMenuCharacterSelect.VertexPositionTextureTexture>(positionTextureTextureArray);
    }
    this.mAvatarVertexDeclaration = RenderManager.Instance.CreateVertexDeclaration(SubMenuCharacterSelect.VertexPositionTextureTexture.VertexElements);
    this.mNameInputBox = new TextInputMessageBox(SubMenuCharacterSelect.LOC_ENTER_NAME, 15);
    this.mLevelContent = new ContentManager(Magicka.Game.Instance.Content.ServiceProvider, Magicka.Game.Instance.Content.RootDirectory);
    SteamAPI.DlcInstalled += new Action<DlcInstalled>(this.UpdateAvailableLevels);
    SteamAPI.DlcInstalled += new Action<DlcInstalled>(this.UpdateAvailableAvatars);
    SteamAPI.DlcInstalled += new Action<DlcInstalled>(this.UpdateAvailableDefaultAvatars);
    PackMan.Instance.PackEnabledChanged += new Action<object, bool>(this.Instance_PackEnabledChanged);
    this.mCheckPointRUSure = new OptionsMessageBox("#chapter_notification".GetHashCodeCustom(), new int[2]
    {
      Defines.LOC_GEN_YES,
      Defines.LOC_GEN_NO
    });
    this.mCheckPointRUSure.Select += new Action<OptionsMessageBox, int>(this.DeleteCheckpointCallback);
  }

  private void ResetLevelTexts()
  {
    if (this.mChapterName == null)
      this.mChapterName = new Text(64 /*0x40*/, FontManager.Instance.GetFont(MagickaFont.MenuTitle), TextAlign.Center, false);
    int iLength = 150;
    TextAlign iAlign = TextAlign.Center;
    if (this.mGameSettings.GameType != GameType.Campaign && this.mGameSettings.GameType != GameType.Mythos)
    {
      iLength = 250;
      iAlign = TextAlign.Left;
    }
    this.mChapterDescription = new Text(iLength, FontManager.Instance.GetFont(MagickaFont.MenuTitle), iAlign, false);
  }

  private static bool AvatarAllowedInGameMode(GameType gameType, Profile.PlayableAvatar target)
  {
    switch (gameType)
    {
      case GameType.Campaign:
      case GameType.Mythos:
        return target.AllowCampaign;
      case GameType.Challenge:
      case GameType.StoryChallange:
        return target.AllowChallenge;
      case GameType.Versus:
        return target.AllowPVP;
      default:
        return false;
    }
  }

  public void UpdateAvailableAvatars(DlcInstalled dlcInstalled)
  {
    LevelNode level = LevelManager.Instance.GetLevel(this.mGameSettings.GameType, this.mGameSettings.Level);
    if (level != null && level.AllowedAvatars != null && level.AllowedAvatars.Count > 0)
    {
      this.UpdateAvailableDefaultAvatars(dlcInstalled);
    }
    else
    {
      this.mDefaultAvatars = false;
      if (SubMenuCharacterSelect.mRobeRepresentations == null)
        SubMenuCharacterSelect.mRobeRepresentations = new List<SubMenuCharacterSelect.RobeRep>();
      else
        SubMenuCharacterSelect.mRobeRepresentations.Clear();
      lock (SubMenuCharacterSelect.mRobeRepresentations)
      {
        IList<Profile.PlayableAvatar> values = Profile.Instance.Avatars.Values;
        for (int index = 0; index < values.Count; ++index)
        {
          Profile.PlayableAvatar playableAvatar = values[index];
          string name = playableAvatar.Name;
          if (string.Compare(name, "wizardcul") != 0 || AchievementsManager.Instance.HasAchievement("fhtagnoncemore"))
          {
            HackHelper.License license = HackHelper.CheckLicense(playableAvatar);
            SubMenuCharacterSelect.RobeRep robeRep = new SubMenuCharacterSelect.RobeRep();
            robeRep.Name = name;
            robeRep.OriginalIndex = index;
            robeRep.IsCustom = license == HackHelper.License.Custom;
            bool flag = SubMenuCharacterSelect.AvatarAllowedInGameMode(this.mGameSettings.GameType, playableAvatar);
            if (flag && (!robeRep.IsCustom || this.AllowCustom))
            {
              if (license == HackHelper.License.No)
                robeRep.IsLocked = true;
              uint appID = 0;
              robeRep.DisplayName = playableAvatar.DisplayName;
              robeRep.Description = playableAvatar.Description;
              robeRep.HashSum = playableAvatar.HashSum;
              if (string.Compare(name, "wizard") == 0)
              {
                robeRep.IsLocked = false;
                robeRep.IsUsed = true;
                robeRep.IsFree = false;
                robeRep.IsNew = false;
              }
              else if (flag && license != HackHelper.License.Custom)
              {
                if (license != HackHelper.License.Custom)
                {
                  if (DLC_StatusHelper.ValidateRobeLocked(license, playableAvatar, out appID))
                  {
                    robeRep.IsLocked = true;
                    robeRep.IsUsed = true;
                  }
                  else
                  {
                    robeRep.IsLocked = false;
                    robeRep.IsUsed = !DLC_StatusHelper.Instance.Item_IsUnused("robe", name, appID, false);
                  }
                  robeRep.BelongsToAppID = appID;
                }
                robeRep.IsNew = DLC_StatusHelper.Instance.AppID_IsNew(appID, name);
                robeRep.IsFree = DLC_StatusHelper.Instance.IsFreeDLC(appID, name);
                if (robeRep.IsLocked && string.Compare(name, "wizardbat") == 0)
                  continue;
              }
              SubMenuCharacterSelect.mRobeRepresentations.Add(robeRep);
            }
          }
        }
      }
      DLC_StatusHelper.Instance.SaveLocalData();
      for (int index = 0; index < Magicka.Game.Instance.Players.Length; ++index)
      {
        Magicka.GameLogic.Player player = Magicka.Game.Instance.Players[index];
        if (player.Gamer != null)
          this.VerifyAvatar(ref player);
      }
      this.SortRobeRepList();
    }
  }

  private void SteamOverlayActivated(GameOverlayActivated gameOverlayActivated)
  {
    if (gameOverlayActivated.mActive != (byte) 0)
      return;
    this.UpdateAvailableLevels(new DlcInstalled());
    this.UpdateAvailableAvatars(new DlcInstalled());
  }

  private void UpdateAvailableDefaultAvatars(DlcInstalled dlcInstalled)
  {
    LevelNode level = LevelManager.Instance.GetLevel(this.mGameSettings.GameType, this.mGameSettings.Level);
    this.mDefaultAvatars = true;
    if (SubMenuCharacterSelect.mRobeRepresentations == null)
      SubMenuCharacterSelect.mRobeRepresentations = new List<SubMenuCharacterSelect.RobeRep>();
    else
      SubMenuCharacterSelect.mRobeRepresentations.Clear();
    lock (SubMenuCharacterSelect.mRobeRepresentations)
    {
      IList<Profile.PlayableAvatar> values = Profile.Instance.Avatars.Values;
      foreach (string allowedAvatar in level.AllowedAvatars)
      {
        for (int index = 0; index < values.Count; ++index)
        {
          Profile.PlayableAvatar playableAvatar = values[index];
          if (allowedAvatar == playableAvatar.Name)
          {
            SubMenuCharacterSelect.RobeRep robeRep = new SubMenuCharacterSelect.RobeRep();
            HackHelper.License license = HackHelper.CheckLicense(playableAvatar);
            if (license == HackHelper.License.Yes || license == HackHelper.License.Custom && this.AllowCustom)
            {
              robeRep.OriginalIndex = index;
              robeRep.IsCustom = license != HackHelper.License.Yes;
            }
            else if (license == HackHelper.License.No)
              robeRep.IsLocked = true;
            else
              continue;
            uint appID = 0;
            string str = robeRep.Name = playableAvatar.Name;
            robeRep.DisplayName = playableAvatar.DisplayName;
            robeRep.Description = playableAvatar.Description;
            robeRep.HashSum = playableAvatar.HashSum;
            if (string.Compare(str, "wizard") == 0)
            {
              robeRep.IsLocked = false;
              robeRep.IsUsed = true;
              robeRep.IsFree = false;
              robeRep.IsNew = false;
            }
            else
            {
              if (license != HackHelper.License.Custom)
              {
                if (DLC_StatusHelper.ValidateRobeLocked(license, playableAvatar, out appID))
                {
                  robeRep.IsLocked = true;
                  robeRep.IsUsed = true;
                }
                else
                {
                  robeRep.IsLocked = false;
                  robeRep.IsUsed = !DLC_StatusHelper.Instance.Item_IsUnused("robe", str, appID, false);
                }
              }
              robeRep.BelongsToAppID = appID;
              robeRep.IsNew = DLC_StatusHelper.Instance.AppID_IsNew(appID, str);
              robeRep.IsFree = DLC_StatusHelper.Instance.IsFreeDLC(appID, str);
            }
            SubMenuCharacterSelect.mRobeRepresentations.Add(robeRep);
          }
        }
      }
    }
    DLC_StatusHelper.Instance.SaveLocalData();
    this.DefaultAvatars();
  }

  public bool NeedToUpdateDefaultAvatarsUponClientLeaving()
  {
    return this.mGameSettings.GameType == GameType.StoryChallange && this.HasSelectedLevel && LevelManager.Instance.GetLevel(GameType.StoryChallange, this.mGameSettings.Level).FileName.Equals("ch_osotc");
  }

  public void DefaultAvatars()
  {
    if (NetworkManager.Instance.State == NetworkState.Client)
      return;
    Magicka.GameLogic.Player[] players = Magicka.Game.Instance.Players;
    bool[] flagArray = new bool[4];
    flagArray[0] = flagArray[1] = flagArray[2] = flagArray[3] = false;
label_21:
    for (int index1 = 0; index1 < SubMenuCharacterSelect.mRobeRepresentations.Count; ++index1)
    {
      Profile.PlayableAvatar iAvatar = Profile.Instance.Avatars.Values[SubMenuCharacterSelect.mRobeRepresentations[index1].OriginalIndex];
      bool flag = false;
      for (int index2 = 0; index2 < players.Length; ++index2)
      {
        Magicka.GameLogic.Player player = players[index2];
        if (player.Playing && !flagArray[index2] && player.Gamer.Avatar == iAvatar)
        {
          flag = true;
          flagArray[index2] = true;
          break;
        }
      }
      if (!flag)
      {
        for (int index3 = 0; index3 < players.Length; ++index3)
        {
          Magicka.GameLogic.Player iPlayer = players[index3];
          if (iPlayer.Playing && !flagArray[index3])
          {
            switch (HackHelper.CheckLicense(iAvatar))
            {
              case HackHelper.License.Yes:
                if (!iPlayer.Gamer.Avatar.Name.Equals(iAvatar.Name))
                  this.mPlayerSlots[index3].ConsecutiveColorChanges = -1;
                flagArray[index3] = true;
                iPlayer.Gamer.Avatar = iAvatar;
                this.mPlayerSlots[index3].Custom = SubMenuCharacterSelect.mRobeRepresentations[index3].IsCustom;
                if (NetworkManager.Instance.State == NetworkState.Server)
                {
                  GamerChangedMessage iMessage = new GamerChangedMessage(iPlayer);
                  NetworkManager.Instance.Interface.SendMessage<GamerChangedMessage>(ref iMessage);
                }
                ToolTipMan.Instance.Kill(iPlayer, false);
                this.mPlayerSlots[index3].SelectedItem = -1;
                goto label_21;
              case HackHelper.License.Custom:
                if (NetworkManager.Instance.State == NetworkState.Offline || !NetworkManager.Instance.Interface.IsVACSecure)
                  goto case HackHelper.License.Yes;
                continue;
              default:
                continue;
            }
          }
        }
      }
    }
  }

  private void DefaultAvatar(Controller iSender)
  {
    int index = this.NumActivePlayer() - 1;
    bool flag = false;
    Profile.PlayableAvatar iAvatar = Profile.Instance.DefaultAvatar;
    if (index >= SubMenuCharacterSelect.mRobeRepresentations.Count)
    {
      LevelNode level = LevelManager.Instance.GetLevel(this.mGameSettings.GameType, this.mGameSettings.Level);
      if (NetworkManager.Instance.State == NetworkState.Client && level.AllowedAvatars != null && level.AllowedAvatars.Count > 0)
      {
        iAvatar = Profile.Instance.Avatars[level.AllowedAvatars[index]];
        flag = true;
      }
    }
    else
      iAvatar = Profile.Instance.Avatars.Values[SubMenuCharacterSelect.mRobeRepresentations[index].OriginalIndex];
    HackHelper.License license = HackHelper.CheckLicense(iAvatar);
    if (!flag)
    {
      switch (license)
      {
        case HackHelper.License.Yes:
          break;
        case HackHelper.License.Custom:
          if (NetworkManager.Instance.State != NetworkState.Offline && NetworkManager.Instance.Interface.IsVACSecure)
            return;
          break;
        default:
          return;
      }
    }
    if (!iSender.Player.Gamer.Avatar.Name.Equals(iAvatar.Name))
      this.mPlayerSlots[index].ConsecutiveColorChanges = -1;
    iSender.Player.Gamer.Avatar = iAvatar;
    this.mPlayerSlots[index].Custom = !flag && SubMenuCharacterSelect.mRobeRepresentations[index].IsCustom;
    if (NetworkManager.Instance.State != NetworkState.Offline)
    {
      GamerChangedMessage iMessage = new GamerChangedMessage(iSender.Player);
      NetworkManager.Instance.Interface.SendMessage<GamerChangedMessage>(ref iMessage);
    }
    ToolTipMan.Instance.Kill(iSender.Player, false);
    this.mPlayerSlots[index].SelectedItem = -1;
  }

  private int NumActivePlayer()
  {
    int num = 0;
    Magicka.GameLogic.Player[] players = Magicka.Game.Instance.Players;
    for (int index = 0; index < players.Length; ++index)
    {
      if (players[index].Playing && this.mPlayerSlots[index].AvatarSelected)
        ++num;
    }
    return num;
  }

  private void UpdateAvailableLevels(DlcInstalled obj)
  {
    LevelNode[] levelNodeArray;
    switch (this.mGameSettings.GameType)
    {
      case GameType.Campaign:
        levelNodeArray = (LevelNode[]) LevelManager.Instance.VanillaCampaign;
        break;
      case GameType.Challenge:
        levelNodeArray = LevelManager.Instance.Challenges;
        break;
      case GameType.Campaign | GameType.Challenge:
        return;
      case GameType.Versus:
        levelNodeArray = LevelManager.Instance.Versus;
        break;
      case GameType.Mythos:
        levelNodeArray = (LevelNode[]) LevelManager.Instance.MythosCampaign;
        break;
      case GameType.StoryChallange:
        levelNodeArray = LevelManager.Instance.StoryChallanges;
        break;
      default:
        return;
    }
    bool flag1 = NetworkManager.Instance.State != NetworkState.Client;
    if ((this.mGameSettings.GameType == GameType.Mythos || this.mGameSettings.GameType == GameType.Campaign) && flag1)
    {
      SubMenuCharacterSelect.mLevelRepresentations.Clear();
      SaveData currentSaveData = SubMenuCampaignSelect_SaveSlotSelect.Instance.CurrentSaveData;
      if (currentSaveData != null)
      {
        int val2 = (int) currentSaveData.MaxAllowedLevel + 1;
        for (int index = 0; index < Math.Min(levelNodeArray.Length, val2); ++index)
        {
          SubMenuCharacterSelect.LevelRep levelRep = new SubMenuCharacterSelect.LevelRep();
          levelRep.OriginalIndex = index;
          levelRep.IsCustom = false;
          levelRep.PreviewImage = (Texture2D) null;
          uint num = 0;
          levelRep.Name = levelNodeArray[index].Name;
          levelRep.FileName = levelNodeArray[index].FileName;
          levelRep.IsLocked = false;
          levelRep.BelongsToAppID = num;
          levelRep.IsNew = false;
          levelRep.IsUsed = false;
          levelRep.IsFree = false;
          levelRep.HashSum = levelNodeArray[index].HashSum;
          SubMenuCharacterSelect.mLevelRepresentations.Add(levelRep);
        }
      }
    }
    else
    {
      SubMenuCharacterSelect.LevelRep levelRep = (SubMenuCharacterSelect.LevelRep) null;
      for (int index = 0; index < levelNodeArray.Length; ++index)
      {
        bool flag2 = false;
        foreach (SubMenuCharacterSelect.LevelRep levelRepresentation in SubMenuCharacterSelect.mLevelRepresentations)
        {
          if (levelRepresentation.OriginalIndex == index)
          {
            levelRep = levelRepresentation;
            flag2 = true;
            break;
          }
        }
        if (flag2)
        {
          HackHelper.License license = HackHelper.CheckLicense(levelNodeArray[index]);
          levelRep.IsCustom = license == HackHelper.License.Custom;
          uint appID = 0;
          string name = levelNodeArray[index].Name;
          levelRep.IsLocked = DLC_StatusHelper.ValidateLevelLocked(license, levelNodeArray[index], out appID);
          levelRep.BelongsToAppID = appID;
          levelRep.IsNew = DLC_StatusHelper.Instance.AppID_IsNew(appID, levelNodeArray[index].FileName);
          levelRep.IsUsed = !DLC_StatusHelper.Instance.Item_IsUnused("level", name, appID, false);
          levelRep.IsFree = DLC_StatusHelper.Instance.IsFreeDLC(appID, levelNodeArray[index].FileName);
        }
      }
    }
    DLC_StatusHelper.Instance.SaveLocalData();
    this.UpdateLevelDescriptions();
    if (this.mGameSettings.GameType == GameType.Challenge || this.mGameSettings.GameType == GameType.Versus)
      this.SortLevelRepList();
    this.mSpecialScrollBar.Value = 0;
  }

  private void SortLevelRepList()
  {
    if (SubMenuCharacterSelect.mLevelRepresentations == null || SubMenuCharacterSelect.mLevelRepresentations.Count == 0)
      return;
    lock (SubMenuCharacterSelect.mLevelRepresentations)
      SubMenuCharacterSelect.mLevelRepresentations.Sort(new Comparison<SubMenuCharacterSelect.LevelRep>(SubMenuCharacterSelect.SelectableObjectRep.CompareRep));
  }

  private void SortRobeRepList()
  {
    if (SubMenuCharacterSelect.mRobeRepresentations == null || SubMenuCharacterSelect.mRobeRepresentations.Count == 0)
      return;
    lock (SubMenuCharacterSelect.mRobeRepresentations)
      SubMenuCharacterSelect.mRobeRepresentations.Sort(new Comparison<SubMenuCharacterSelect.RobeRep>(SubMenuCharacterSelect.SelectableObjectRep.CompareRep));
  }

  private int GetLevelOriginalIndex(int sortedIndex)
  {
    return SubMenuCharacterSelect.mLevelRepresentations[sortedIndex].OriginalIndex;
  }

  private int GetLevelSortedIndex(int originalIndex)
  {
    if (SubMenuCharacterSelect.mLevelRepresentations == null)
      return -1;
    for (int index = 0; index < SubMenuCharacterSelect.mLevelRepresentations.Count; ++index)
    {
      if (SubMenuCharacterSelect.mLevelRepresentations[index].OriginalIndex == originalIndex)
        return index;
    }
    return -1;
  }

  private int GetRobeOriginalIndex(int sortedIndex)
  {
    return SubMenuCharacterSelect.mRobeRepresentations[sortedIndex].OriginalIndex;
  }

  private int GetRobeSortedIndex(int originalIndex)
  {
    for (int index = 0; index < SubMenuCharacterSelect.mRobeRepresentations.Count; ++index)
    {
      if (SubMenuCharacterSelect.mRobeRepresentations[index].OriginalIndex == originalIndex)
        return index;
    }
    return -1;
  }

  private void UpdateLevelDescriptions()
  {
    LevelNode[] levelNodeArray;
    switch (this.mGameSettings.GameType)
    {
      case GameType.Campaign:
        levelNodeArray = (LevelNode[]) LevelManager.Instance.VanillaCampaign;
        break;
      case GameType.Challenge:
        levelNodeArray = LevelManager.Instance.Challenges;
        break;
      case GameType.Campaign | GameType.Challenge:
        return;
      case GameType.Versus:
        levelNodeArray = LevelManager.Instance.Versus;
        break;
      case GameType.Mythos:
        levelNodeArray = (LevelNode[]) LevelManager.Instance.MythosCampaign;
        break;
      case GameType.StoryChallange:
        levelNodeArray = LevelManager.Instance.StoryChallanges;
        break;
      default:
        return;
    }
    LanguageManager instance = LanguageManager.Instance;
    if (SubMenuCharacterSelect.mLevelRepresentations != null)
    {
      for (int index = 0; index < SubMenuCharacterSelect.mLevelRepresentations.Count; ++index)
      {
        if (SubMenuCharacterSelect.mLevelRepresentations[index].Title != null)
          SubMenuCharacterSelect.mLevelRepresentations[index].Title.Dispose();
        if (SubMenuCharacterSelect.mLevelRepresentations[index].Descr != null)
          SubMenuCharacterSelect.mLevelRepresentations[index].Descr.Dispose();
        SubMenuCharacterSelect.mLevelRepresentations[index].Title = new Text(64 /*0x40*/, this.mFont, TextAlign.Center, false);
        SubMenuCharacterSelect.mLevelRepresentations[index].Title.SetText(instance.GetString(SubMenuCharacterSelect.mLevelRepresentations[index].Name.GetHashCodeCustom()));
      }
    }
    if (this.mGameSettings.GameType != GameType.Mythos && this.mGameSettings.GameType != GameType.Campaign)
      return;
    BitmapFont font = FontManager.Instance.GetFont(MagickaFont.MenuTitle);
    if (SubMenuCharacterSelect.mLevelRepresentations == null)
      return;
    for (int index = 0; index < SubMenuCharacterSelect.mLevelRepresentations.Count; ++index)
    {
      SubMenuCharacterSelect.mLevelRepresentations[index].Descr = new Text(64 /*0x40*/, this.mFont, TextAlign.Center, false);
      string iText1 = instance.GetString(levelNodeArray[SubMenuCharacterSelect.mLevelRepresentations[index].OriginalIndex].Description);
      string iText2 = font.Wrap(iText1, 539, true);
      SubMenuCharacterSelect.mLevelRepresentations[index].Descr.SetText(iText2);
    }
  }

  private void Instance_PackEnabledChanged(object arg1, bool arg2)
  {
    if (NetworkManager.Instance.State != NetworkState.Server)
      return;
    PackOptionsMessage iMessage = new PackOptionsMessage();
    NetworkManager.Instance.Interface.SendMessage<PackOptionsMessage>(ref iMessage);
  }

  private void LoadLevelPreviews()
  {
    lock (this.mLevelContent)
      this.mLevelContent.Unload();
    LevelNode[] levelNodeArray = (LevelNode[]) null;
    string str = (string) null;
    bool flag1 = false;
    switch (this.mGameSettings.GameType)
    {
      case GameType.Campaign:
      case GameType.Mythos:
        flag1 = true;
        break;
      case GameType.Challenge:
        levelNodeArray = LevelManager.Instance.Challenges;
        str = "Levels/Challenges/";
        break;
      case GameType.Campaign | GameType.Challenge:
        return;
      case GameType.Versus:
        levelNodeArray = LevelManager.Instance.Versus;
        str = "Levels/Versus/";
        break;
      case GameType.StoryChallange:
        levelNodeArray = LevelManager.Instance.StoryChallanges;
        str = "Levels/Challenges/";
        break;
      default:
        return;
    }
    SubMenuCharacterSelect.mLevelRepresentations = new List<SubMenuCharacterSelect.LevelRep>();
    if (!string.IsNullOrEmpty(str))
    {
      for (int index = 0; index < levelNodeArray.Length; ++index)
      {
        if (HackHelper.CheckLicense(levelNodeArray[index]) != HackHelper.License.Custom || this.AllowCustom)
        {
          string name = levelNodeArray[index].Name;
          if (!string.IsNullOrEmpty(name) && (!name.Equals("#challenge_vietnam", StringComparison.InvariantCultureIgnoreCase) || this.mGameSettings.GameType != GameType.Challenge))
          {
            SubMenuCharacterSelect.LevelRep levelRep = new SubMenuCharacterSelect.LevelRep();
            levelRep.OriginalIndex = index;
            levelRep.Name = name;
            levelRep.FileName = levelNodeArray[index].FileName;
            levelRep.HashSum = levelNodeArray[index].HashSum;
            if (!flag1)
            {
              try
              {
                levelRep.PreviewImage = this.mLevelContent.Load<Texture2D>(str + levelNodeArray[index].LoadingImage);
              }
              catch (ContentLoadException ex)
              {
                levelRep.PreviewImage = (Texture2D) null;
              }
            }
            else
              levelRep.PreviewImage = (Texture2D) null;
            SubMenuCharacterSelect.mLevelRepresentations.Add(levelRep);
          }
        }
      }
    }
    this.UpdateAvailableLevels(new DlcInstalled());
    this.mLoadingLevels = false;
    if (levelNodeArray == null)
      return;
    if (NetworkManager.Instance.State == NetworkState.Client && this.HasSelectedLevel && this.mGameSettings.Level > -1)
    {
      this.SortLevelRepList();
      this.mGameSettings.Level = this.GetLevelSortedIndex(this.mGameSettings.Level);
      this.HasSelectedLevel = this.mGameSettings.Level > -1;
    }
    else
    {
      if (string.IsNullOrEmpty(this.mLevelNameToFocusWhenLevelComplete))
        return;
      bool flag2 = false;
      int iLevel = 0;
      foreach (LevelNode levelNode in levelNodeArray)
      {
        if (levelNode.FileName.Equals(this.mLevelNameToFocusWhenLevelComplete, StringComparison.InvariantCultureIgnoreCase))
        {
          flag2 = true;
          this.mSelectedPack = this.mGameSettings.Level = iLevel;
          this.HasSelectedLevel = true;
          break;
        }
        ++iLevel;
      }
      if (!flag2)
      {
        iLevel = -1;
        this.HasSelectedLevel = false;
      }
      this.mGameSettings.Level = iLevel;
      this.OnLevelChange((Controller) null, this.mGameSettings.GameType, iLevel);
      this.ChangeState((Controller) null, SubMenuCharacterSelect.State.Normal);
      this.mLevelNameToFocusWhenLevelComplete = (string) null;
    }
  }

  private void mGameModeBox_ValueChanged(DropDownBox iSender, Rulesets iValue)
  {
    if (this.mVersusSettings != null)
      this.mVersusSettings.Changed -= new VersusRuleset.Settings.SettingChanged(this.mVersusSettings_Changed);
    if (this.mVersusSettings != null)
    {
      VersusRuleset.Settings.OptionsMessage oMessage;
      this.mVersusSettings.GetMessage(out oMessage);
      switch (oMessage.Ruleset)
      {
        case Rulesets.DeathMatch:
          GlobalSettings.Instance.VSSettings.DeathMatch = oMessage;
          break;
        case Rulesets.Brawl:
          GlobalSettings.Instance.VSSettings.Brawl = oMessage;
          break;
        case Rulesets.Pyrite:
          GlobalSettings.Instance.VSSettings.PyriteSnitch = oMessage;
          break;
        case Rulesets.Kreitor:
          GlobalSettings.Instance.VSSettings.Kreitor = oMessage;
          break;
        case Rulesets.King:
          GlobalSettings.Instance.VSSettings.KingOfTheHill = oMessage;
          break;
      }
    }
    switch (iValue)
    {
      case Rulesets.DeathMatch:
        VersusRuleset.Settings.ApplyMessage(ref this.mVersusSettings, ref GlobalSettings.Instance.VSSettings.DeathMatch);
        break;
      case Rulesets.Brawl:
        VersusRuleset.Settings.ApplyMessage(ref this.mVersusSettings, ref GlobalSettings.Instance.VSSettings.Brawl);
        break;
      case Rulesets.Pyrite:
        VersusRuleset.Settings.ApplyMessage(ref this.mVersusSettings, ref GlobalSettings.Instance.VSSettings.PyriteSnitch);
        break;
      case Rulesets.Kreitor:
        VersusRuleset.Settings.ApplyMessage(ref this.mVersusSettings, ref GlobalSettings.Instance.VSSettings.Kreitor);
        break;
      case Rulesets.King:
        VersusRuleset.Settings.ApplyMessage(ref this.mVersusSettings, ref GlobalSettings.Instance.VSSettings.KingOfTheHill);
        break;
      default:
        VersusRuleset.Settings.ApplyMessage(ref this.mVersusSettings, ref GlobalSettings.Instance.VSSettings.DeathMatch);
        break;
    }
    this.mSettingsScrollbar.SetMaxValue(this.mVersusSettings.MenuItems.Count - 5);
    LanguageManager instance = LanguageManager.Instance;
    this.mVersusSettingsTitles = new Text[this.mVersusSettings.MenuTitles.Count];
    for (int index = 0; index < this.mVersusSettings.MenuTitles.Count; ++index)
    {
      this.mVersusSettingsTitles[index] = new Text(48 /*0x30*/, this.mVersusSettings.MenuItems[index].Font, TextAlign.Left, false);
      this.mVersusSettingsTitles[index].SetText(instance.GetString(this.mVersusSettings.MenuTitles[index]));
    }
    this.mVersusSettings.Changed += new VersusRuleset.Settings.SettingChanged(this.mVersusSettings_Changed);
    NetworkState state = NetworkManager.Instance.State;
    if (state == NetworkState.Server)
    {
      VersusRuleset.Settings.OptionsMessage oMessage;
      this.mVersusSettings.GetMessage(out oMessage);
      NetworkManager.Instance.Interface.SendMessage<VersusRuleset.Settings.OptionsMessage>(ref oMessage);
    }
    if (state != NetworkState.Client)
    {
      bool flag = this.mVersusSettings.TeamsEnabled && this.mGameSettings.GameType == GameType.Versus;
      int num1 = 0;
      int num2 = 0;
      Magicka.GameLogic.Player[] players = Magicka.Game.Instance.Players;
      for (int index = 0; index < players.Length; ++index)
      {
        if (players[index].Playing)
        {
          if (flag)
          {
            if (players[index].Team == Factions.NONE)
            {
              if (num1 <= num2)
              {
                players[index].Team = Factions.TEAM_RED;
                ++num1;
              }
              else
              {
                players[index].Team = Factions.TEAM_BLUE;
                ++num2;
              }
              if (state == NetworkState.Server)
                NetworkManager.Instance.Interface.SendMessage<MenuSelectMessage>(ref new MenuSelectMessage()
                {
                  Option = 2,
                  Param0I = index,
                  Param1I = (int) players[index].Team
                });
            }
            else if ((players[index].Team & Factions.TEAM_RED) != Factions.NONE)
              ++num1;
            else
              ++num2;
          }
          else
          {
            players[index].Team = Factions.NONE;
            if (state == NetworkState.Server)
              NetworkManager.Instance.Interface.SendMessage<MenuSelectMessage>(ref new MenuSelectMessage()
              {
                Option = 2,
                Param0I = index,
                Param1I = (int) players[index].Team
              });
          }
        }
      }
    }
    this.UpdateGamerDropDownMenu();
  }

  private void mVersusSettings_Changed(int iOption, int iNewSelection)
  {
    if (NetworkManager.Instance.State == NetworkState.Server)
      NetworkManager.Instance.Interface.SendMessage<MenuSelectMessage>(ref new MenuSelectMessage()
      {
        IntendedMenu = MenuSelectMessage.MenuType.CharacterSelect,
        Option = 0,
        Param0I = iOption,
        Param1I = iNewSelection
      });
    this.UpdateGamerDropDownMenu();
    Magicka.GameLogic.Player[] players = Magicka.Game.Instance.Players;
    if (NetworkManager.Instance.State != NetworkState.Client && this.mGameSettings.GameType == GameType.Versus && this.mVersusSettings.TeamsEnabled)
    {
      int index1 = 0;
      for (int index2 = 3; index1 <= index2; --index2)
      {
        while (index1 < index2 && !players[index1].Playing)
          ++index1;
        if (players[index1].Playing && players[index1].Team == Factions.NONE)
        {
          players[index1].Team = Factions.TEAM_RED;
          if (NetworkManager.Instance.State == NetworkState.Server)
            NetworkManager.Instance.Interface.SendMessage<MenuSelectMessage>(ref new MenuSelectMessage()
            {
              IntendedMenu = MenuSelectMessage.MenuType.CharacterSelect,
              Option = 2,
              Param0I = index1,
              Param1I = 4096 /*0x1000*/
            });
        }
        while (index1 < index2 && !players[index2].Playing)
          --index2;
        if (players[index2].Playing && players[index2].Team == Factions.NONE)
        {
          players[index2].Team = Factions.TEAM_BLUE;
          if (NetworkManager.Instance.State == NetworkState.Server)
            NetworkManager.Instance.Interface.SendMessage<MenuSelectMessage>(ref new MenuSelectMessage()
            {
              IntendedMenu = MenuSelectMessage.MenuType.CharacterSelect,
              Option = 2,
              Param0I = index2,
              Param1I = 8192 /*0x2000*/
            });
        }
        ++index1;
      }
    }
    else
    {
      if (this.mVersusSettings.TeamsEnabled && this.mGameSettings.GameType == GameType.Versus)
        return;
      for (int index = 0; index < players.Length; ++index)
        players[index].Team = Factions.NONE;
    }
  }

  private void UpdateGamerDropDownMenu()
  {
    if (this.mGameSettings.GameType == GameType.Versus && this.mVersusSettings.TeamsEnabled)
    {
      if (this.mGamerDropDownMenu.Count > 0)
        return;
      this.mGamerDropDownMenu.AddOption("#menu_change_team".GetHashCodeCustom());
    }
    else
    {
      if (this.mGamerDropDownMenu.Count <= 0)
        return;
      this.mGamerDropDownMenu.RemoveAt(0);
    }
  }

  public override void LanguageChanged()
  {
    base.LanguageChanged();
    this.UpdateLevelDescriptions();
    LanguageManager instance = LanguageManager.Instance;
    this.mPacksText.SetText(instance.GetString(SubMenuCharacterSelect.LOC_PACKS));
    this.mItemsText.SetText(instance.GetString(SubMenuCharacterSelect.LOC_ITEMPACKS));
    this.mMagicksText.SetText(instance.GetString(SubMenuCharacterSelect.LOC_MAGICKPACKS));
    this.mOpenText.SetText(instance.GetString(SubMenuCharacterSelect.LOC_JOIN));
    this.mSelectLevelButton.SetText(instance.GetString(SubMenuCharacterSelect.LOC_TT_CHANGE_LEVEL));
    this.mBackButton.LanguageChanged();
    this.mStartButton.LanguageChanged();
    this.mPackCloseButton.LanguageChanged();
    this.mLoadingText.SetText(LanguageManager.Instance.GetString(SubMenuCharacterSelect.LOC_LOADING));
    this.mGameModeBox.LanguageChanged();
    this.mGameModeTitle.SetText(instance.GetString(SubMenuCharacterSelect.LOC_GAMEMODE));
    for (int index = 0; index < this.mVersusSettings.MenuItems.Count; ++index)
    {
      this.mVersusSettings.MenuItems[index].LanguageChanged();
      this.mVersusSettingsTitles[index].SetText(instance.GetString(this.mVersusSettings.MenuTitles[index]));
    }
  }

  public void GameChanged() => this.SetReady(false);

  private void UpdateGamers()
  {
    BitmapFont font = FontManager.Instance.GetFont(MagickaFont.MenuDefault);
    lock (Magicka.Game.Instance.GraphicsDevice)
    {
      this.mGamerItems.Clear();
      for (int index = 0; index < Profile.Instance.Gamers.Count; ++index)
      {
        Text text = new Text(32 /*0x20*/, font, TextAlign.Center, false);
        text.SetText(Profile.Instance.Gamers.Keys[index]);
        this.mGamerItems.Add(text);
      }
      Text text1 = new Text(32 /*0x20*/, font, TextAlign.Center, false);
      text1.SetText(LanguageManager.Instance.GetString(SubMenuCharacterSelect.LOC_NEW));
      this.mGamerItems.Add(text1);
      for (int index = 0; index < this.mGamerScrollBars.Length; ++index)
        this.mGamerScrollBars[index].SetMaxValue(this.mGamerItems.Count - 4);
    }
  }

  public override void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    NetworkState state = NetworkManager.Instance.State;
    if (this.mCurrentState == SubMenuCharacterSelect.State.Normal || this.mCurrentState == SubMenuCharacterSelect.State.CountDown)
    {
      this.mOptionsAlpha = Math.Min(this.mOptionsAlpha + iDeltaTime * 4f, 1f);
      this.mLevelSelectAlpha = Math.Max(this.mLevelSelectAlpha - iDeltaTime * 4f, 0.0f);
      this.mPackSelectAlpha = Math.Max(this.mPackSelectAlpha - iDeltaTime * 4f, 0.0f);
    }
    else
    {
      this.mOptionsAlpha = Math.Max(this.mOptionsAlpha - iDeltaTime * 4f, 0.0f);
      if (this.mCurrentState == SubMenuCharacterSelect.State.ChangingLevel)
      {
        this.mLevelSelectAlpha = Math.Min(this.mLevelSelectAlpha + iDeltaTime * 4f, 1f);
        this.mPackSelectAlpha = Math.Max(this.mPackSelectAlpha - iDeltaTime * 4f, 0.0f);
      }
      else
      {
        this.mLevelSelectAlpha = Math.Max(this.mLevelSelectAlpha - iDeltaTime * 4f, 0.0f);
        this.mPackSelectAlpha = Math.Min(this.mPackSelectAlpha + iDeltaTime * 4f, 1f);
      }
    }
    if (state == NetworkState.Server)
    {
      bool transitionActive = RenderManager.Instance.IsTransitionActive;
      (NetworkManager.Instance.Interface as NetworkServer).Playing = (double) this.mCountDown > 0.0 || transitionActive;
    }
    if (this.mLastCountDownNr >= 0)
    {
      this.mCountDown -= iDeltaTime;
      int num = (int) Math.Ceiling((double) this.mCountDown);
      if (num != this.mLastCountDownNr)
      {
        NetworkChat.Instance.AddMessage(LanguageManager.Instance.GetString(SubMenuCharacterSelect.LOC_COUNTDOWN).Replace("#1;", num.ToString()));
        if (num == 0 && NetworkManager.Instance.State != NetworkState.Offline)
          this.StartLevel();
        this.mLastCountDownNr = num;
      }
    }
    bool flag = this.mCurrentState != SubMenuCharacterSelect.State.CountDown && this.mGameSettings.GameType == GameType.Versus && NetworkManager.Instance.State != NetworkState.Client;
    this.mGameModeBox.Enabled = flag;
    for (int index = 0; index < this.mVersusSettings.MenuItems.Count; ++index)
      this.mVersusSettings.MenuItems[index].Enabled = flag;
    this.mGamerDropDownMenu.Update(iDeltaTime);
    this.mAdminDropDownMenu.Update(iDeltaTime);
    this.mGameModeBox.Update(iDeltaTime);
    for (int index = 0; index < this.mVersusSettings.MenuItems.Count; ++index)
      this.mVersusSettings.MenuItems[index].Update(iDeltaTime);
    Magicka.GameLogic.Player[] players = Magicka.Game.Instance.Players;
    this.mStartButton.Enabled = this.HasSelectedLevel && !this.mValidatingLevels && !this.mLoadingLevels && state != NetworkState.Client;
    if (state == NetworkState.Offline)
      return;
    for (int index = 0; index < players.Length; ++index)
    {
      NetworkGamer gamer = players[index].Gamer as NetworkGamer;
      if (players[index].Playing && gamer != null)
        this.mPlayerSlots[index].SetLatency(NetworkManager.Instance.Interface.GetLatencyMS(gamer.ClientID));
      else
        this.mPlayerSlots[index].SetLatency(0);
    }
    NetworkChat.Instance.Update(iDeltaTime);
  }

  private bool HitPackList(ref Vector2 iMousePos, out int oIndex)
  {
    oIndex = -1;
    if ((double) iMousePos.X < 128.0 || (double) iMousePos.X > 448.0)
      return false;
    int num1 = this.mSpecialScrollBar.Value;
    ItemPack[] itemPacks = PackMan.Instance.ItemPacks;
    MagickPack[] magickPacks = PackMan.Instance.MagickPacks;
    int num2 = Math.Max(0, 1 - num1);
    float num3 = this.mSpecialScrollBar.Position.Y - 224f;
    if (num1 == 0)
      num3 += 64f;
    for (int index1 = 5 * Math.Max(num1 - 1, 0); num2 < 7 && index1 < itemPacks.Length; ++num2)
    {
      float num4 = 128f;
      for (int index2 = 0; index1 < itemPacks.Length && index2 < 5; ++index2)
      {
        if ((double) iMousePos.X >= (double) num4 && (double) iMousePos.X <= (double) num4 + 64.0 && (double) iMousePos.Y >= (double) num3 && (double) iMousePos.Y <= (double) num3 + 64.0)
        {
          oIndex = index1;
          return true;
        }
        num4 += 64f;
        ++index1;
      }
      num3 += 64f;
    }
    int num5 = itemPacks.Length / 5;
    if (itemPacks.Length % 5 != 0)
      ++num5;
    if (num2 < 7 && num5 + 1 - num1 >= 0)
    {
      num3 += 64f;
      ++num2;
    }
    for (int index3 = Math.Max(0, num1 - 2 - num5) * 5; num2 < 7 && index3 < magickPacks.Length; ++num2)
    {
      float num6 = 128f;
      for (int index4 = 0; index3 < magickPacks.Length && index4 < 5; ++index4)
      {
        if ((double) iMousePos.X >= (double) num6 && (double) iMousePos.X <= (double) num6 + 64.0 && (double) iMousePos.Y >= (double) num3 && (double) iMousePos.Y <= (double) num3 + 64.0)
        {
          oIndex = index3 + itemPacks.Length;
          return true;
        }
        num6 += 64f;
        ++index3;
      }
      num3 += 64f;
    }
    return false;
  }

  private bool HitPackOverview(
    ref Vector2 iMousePos,
    out int oIndex,
    out ControllerDirection oScrollDirection)
  {
    oIndex = -1;
    oScrollDirection = ControllerDirection.Center;
    float num1 = 408f;
    if ((double) iMousePos.Y >= (double) num1 && (double) iMousePos.Y <= (double) num1 + 64.0)
    {
      float num2 = 288f;
      IPack[] allPacks = PackMan.Instance.AllPacks;
      int num3 = 0;
      for (int index = 0; index < allPacks.Length; ++index)
      {
        if (allPacks[index].Enabled)
          ++num3;
      }
      int num4 = this.mPackScrollValue;
      if (num3 > 5)
      {
        float num5 = num2 - 224f;
        if ((double) iMousePos.X >= (double) num5 && (double) iMousePos.X <= (double) num5 + 64.0)
        {
          oScrollDirection = ControllerDirection.Left;
          return true;
        }
        float num6 = num2 + 160f;
        if ((double) iMousePos.X >= (double) num6 && (double) iMousePos.X <= (double) num6 + 64.0)
        {
          oScrollDirection = ControllerDirection.Right;
          return true;
        }
      }
      else
        num4 = 0;
      int index1 = num4;
      float num7 = num2 - 160f;
      for (int index2 = 0; index2 < 5; ++index2)
      {
        while (!allPacks[index1].Enabled)
        {
          index1 = (index1 + 1) % allPacks.Length;
          if (index1 == num4)
            return false;
        }
        if ((double) iMousePos.X >= (double) num7 && (double) iMousePos.X <= (double) num7 + 64.0)
        {
          oIndex = index1;
          return true;
        }
        index1 = (index1 + 1) % allPacks.Length;
        num7 += 64f;
      }
    }
    else
    {
      Vector2 vector2 = this.mPacksText.Font.MeasureText(this.mPacksText.Characters, true);
      if ((double) iMousePos.X >= 128.0 && (double) iMousePos.X <= 128.0 + (double) vector2.X && (double) iMousePos.Y >= 368.0 && (double) iMousePos.Y <= 408.0)
        return true;
    }
    return false;
  }

  private bool HitGamer(ref Vector2 iMousePos, out int oIndex)
  {
    if ((double) iMousePos.X >= 528.0 && (double) iMousePos.X <= 1008.0)
    {
      float num1 = 89f;
      float num2 = 201f;
      for (int index = 0; index < 4; ++index)
      {
        if ((double) iMousePos.Y >= (double) num1 && (double) iMousePos.Y <= (double) num2)
        {
          oIndex = index;
          return true;
        }
        num1 += 137f;
        num2 += 137f;
      }
    }
    oIndex = -1;
    return false;
  }

  private bool HitAvatar(int iID, ref Vector2 iMousePos, out int oIndex)
  {
    int count = SubMenuCharacterSelect.mRobeRepresentations.Count;
    float num1 = (float) (76.5 + 137.0 * (double) iID);
    float num2 = (float) ((double) num1 + 112.0 + 25.0);
    if ((double) iMousePos.Y >= (double) num1 && (double) iMousePos.Y <= (double) num2)
    {
      bool flag = count > 4;
      float num3 = 576f;
      float num4 = num3 + 96f;
      for (int index = 0; index < Math.Min(count, 4); ++index)
      {
        if ((double) iMousePos.X >= (double) num3 && (double) iMousePos.X <= (double) num4)
        {
          int num5 = 0;
          if (flag)
            num5 = this.mPlayerSlots[iID].ScrollValue;
          oIndex = (index + num5) % count;
          return true;
        }
        num3 += 96f;
        num4 += 96f;
      }
    }
    oIndex = -1;
    return false;
  }

  private bool HitColor(int iID, ref Vector2 iMousePos, out int oIndex)
  {
    Vector2 vector2 = new Vector2();
    vector2.X = (float) (936.0 - (double) (Defines.PLAYERCOLORS.Length / 2) * 34.0);
    vector2.Y = (float) (89.0 + (double) iID * 137.0 + 56.0 - 34.0);
    for (int index = 0; index < Defines.PLAYERCOLORS.Length; ++index)
    {
      if ((double) iMousePos.X >= (double) vector2.X && (double) iMousePos.X <= (double) vector2.X + 34.0 && (double) iMousePos.Y >= (double) vector2.Y && (double) iMousePos.Y <= (double) vector2.Y + 34.0)
      {
        oIndex = index;
        return true;
      }
      if (index == Defines.PLAYERCOLORS.Length / 2)
      {
        vector2.X -= 34f * (float) (Defines.PLAYERCOLORS.Length / 2);
        vector2.Y += 34f;
      }
      else
        vector2.X += 34f;
    }
    oIndex = -1;
    return false;
  }

  public override void Draw(Viewport iLeftSide, Viewport iRightSide)
  {
    this.mEffect.GraphicsDevice.Viewport = iRightSide;
    float mOptionsAlpha = this.mOptionsAlpha;
    Vector2 iPos1 = new Vector2();
    iPos1.X = 544f;
    iPos1.Y = 710f;
    NetworkChat.Instance.Draw(ref iPos1);
    Matrix iTransform = new Matrix();
    iTransform.M11 = iTransform.M22 = iTransform.M44 = 1f;
    Vector4 vector4_1 = Vector4.One;
    bool flag1 = NetworkManager.Instance.State == NetworkState.Server || NetworkManager.Instance.State == NetworkState.Offline;
    Point iPos2 = new Point(64 /*0x40*/, 480);
    this.mEffect.Begin();
    this.mEffect.CurrentTechnique.Passes[0].Begin();
    if (this.mLoadingLevels || SubMenuCharacterSelect.mLevelRepresentations == null || SubMenuCharacterSelect.mLevelRepresentations.Count == 0)
    {
      this.mSelectLevelButton.Enabled = this.mSelectLevelButton.Selected = false;
      this.mSelectLevelButton.SetText(LanguageManager.Instance.GetString(SubMenuCharacterSelect.LOC_LOADING));
      this.DrawLevel(-1, false, false, ref iPos2, 448, 150, 1f);
    }
    else
    {
      this.mSelectLevelButton.Enabled = true;
      this.mSelectLevelButton.SetText(LanguageManager.Instance.GetString(SubMenuCharacterSelect.LOC_TT_CHANGE_LEVEL));
      float levelSelectAlpha = this.mLevelSelectAlpha;
      if ((double) levelSelectAlpha > 0.0)
      {
        Point iPos3 = new Point();
        iPos3.X = 64 /*0x40*/;
        iPos3.Y = 64 /*0x40*/;
        this.mSpecialScrollBar.Position = new Vector2((float) iPos3.X + 416f, (float) iPos3.Y + 420f);
        vector4_1.W *= levelSelectAlpha;
        this.mSpecialScrollBar.Color = vector4_1;
        this.mSpecialScrollBar.Draw(this.mEffect);
        int num = this.mSpecialScrollBar.Value;
        for (int index = num; index < Math.Min(num + 6, SubMenuCharacterSelect.mLevelRepresentations.Count); ++index)
        {
          iPos3.Y += 38;
          bool iSelected = index == this.mSelectedPack;
          if (this.GameType != GameType.Mythos && this.GameType != GameType.Campaign)
            this.DrawLevel(index, iSelected, SubMenuCharacterSelect.mLevelRepresentations[index].IsCustom, ref iPos3, 384, 100, levelSelectAlpha);
          vector4_1 = iSelected ? MenuItem.COLOR_SELECTED : MenuItem.COLOR;
          vector4_1.W *= levelSelectAlpha;
          this.mEffect.Color = vector4_1;
          if (SubMenuCharacterSelect.mLevelRepresentations[index].Title != null)
            SubMenuCharacterSelect.mLevelRepresentations[index].Title.Draw(this.mEffect, (float) iPos3.X + 192f, (float) iPos3.Y - 32f, 0.8f);
          if (this.GameType == GameType.Mythos || this.GameType == GameType.Campaign)
          {
            SubMenuCharacterSelect.mLevelRepresentations[index].Descr.Draw(this.mEffect, (float) iPos3.X + 192f, (float) iPos3.Y, 0.7f);
            this.mEffect.Saturation = iSelected ? 1.3f : 0.7f;
            Vector4 iColor = Vector4.One * (iSelected ? 1.5f : 1f);
            iColor.W *= levelSelectAlpha;
            this.mEffect.CommitChanges();
            this.DrawGraphics(SubMenu.sPagesTexture, new Rectangle(448, 976, 286, 48 /*0x30*/), new Rectangle(iPos3.X, iPos3.Y - 36, 116, 32 /*0x20*/), iColor);
            this.DrawGraphics(SubMenu.sPagesTexture, new Rectangle(770, 976, 286, 48 /*0x30*/), new Rectangle(iPos3.X + 268, iPos3.Y - 36, 116, 32 /*0x20*/), iColor);
            this.mEffect.Saturation = 1f;
          }
          iPos3.Y += 102;
        }
      }
    }
    float mPackSelectAlpha = this.mPackSelectAlpha;
    if ((double) mPackSelectAlpha > 0.0)
    {
      this.mSpecialScrollBar.Position = new Vector2(480f, (float) (96.0 + (double) this.mSpecialScrollBar.Height * 0.5));
      vector4_1.W *= mPackSelectAlpha;
      this.mSpecialScrollBar.Color = vector4_1;
      this.mSpecialScrollBar.Draw(this.mEffect);
      this.DrawPacksList(mPackSelectAlpha);
      this.mPackCloseButton.Selected = this.mSelectedPosition == 5 && this.mSelectedPack < 0;
      this.mPackCloseButton.Position = new Vector2(512f, (float) ((double) this.mSpecialScrollBar.Position.Y + (double) this.mSpecialScrollBar.Height * 0.5 + 32.0));
      this.mPackCloseButton.Draw(this.mEffect, 1f, mPackSelectAlpha);
    }
    if (this.mGameSettings.GameType != GameType.Campaign && this.mGameSettings.GameType != GameType.Mythos)
    {
      if (!this.HasSelectedLevel && this.mCurrentState != SubMenuCharacterSelect.State.ChangingLevel && this.mCurrentState != SubMenuCharacterSelect.State.ChangingPacks)
      {
        if (flag1)
        {
          this.DrawLevel(-1, true, false, ref iPos2, 448, 150, 1f);
        }
        else
        {
          Vector4 color = this.mEffect.Color;
          this.mEffect.Color = SubMenuCharacterSelect.LEVEL_DESCR_COLOR_NONCAMP;
          iPos1.X = (float) iPos2.X + 224f;
          iPos1.Y = (float) iPos2.Y + 35f;
          this.mChapterName.SetText(SubMenuCharacterSelect.sServerChangingLevelText);
          this.mChapterName.Draw(this.mEffect, iPos1.X, iPos1.Y, 0.8f);
          this.mEffect.Color = color;
        }
      }
      else if (this.HasSelectedLevel && this.mCurrentState != SubMenuCharacterSelect.State.ChangingLevel && this.mCurrentState != SubMenuCharacterSelect.State.ChangingPacks)
      {
        int levelSortedIndex = this.GetLevelSortedIndex(this.mGameSettings.Level);
        if (levelSortedIndex == -1)
          this.HasSelectedLevel = false;
        else
          this.DrawLevel(levelSortedIndex, this.mSelectedPosition == 6, this.mCustomLevel, ref iPos2, 448, 150, mOptionsAlpha);
      }
      if (this.HasSelectedLevel && this.mCurrentState != SubMenuCharacterSelect.State.ChangingLevel)
      {
        Vector4 color = this.mEffect.Color;
        this.mEffect.Color = SubMenuCharacterSelect.LEVEL_DESCR_COLOR_NONCAMP;
        Vector2 vector2 = this.mChapterName.Font.MeasureText(this.mChapterName.Characters, true);
        iPos1.X = (float) iPos2.X + 224f;
        if ((int) vector2.X >= 545)
          iPos1.X += 10f;
        iPos1.Y = (float) iPos2.Y + 155f;
        this.mChapterName.Draw(this.mEffect, iPos1.X, iPos1.Y, 0.8f);
        iPos1.X = (float) iPos2.X + 30f;
        iPos1.Y += vector2.Y - 7f;
        this.mChapterDescription.Draw(this.mEffect, iPos1.X, iPos1.Y, 0.45f);
        this.mEffect.Color = color;
      }
    }
    else
    {
      bool flag2 = this.mSelectedPosition == 9;
      if (this.mCurrentState != SubMenuCharacterSelect.State.ChangingLevel)
      {
        Vector4 vector4_2 = flag2 ? MenuItem.COLOR_SELECTED : MenuItem.COLOR;
        vector4_2.W = (1f - this.mLevelSelectAlpha) * vector4_2.W;
        this.mEffect.Color = vector4_2;
        iPos1.X = 288f;
        iPos1.Y = 96f;
        this.mEffect.Saturation = flag2 ? 1.3f : 1f;
        this.mChapterName.Draw(this.mEffect, iPos1.X, iPos1.Y);
        iPos1.Y += (float) this.mChapterName.Font.LineHeight;
        this.mChapterDescription.Draw(this.mEffect, iPos1.X, iPos1.Y, 0.8f);
        this.mEffect.Saturation = 1f;
      }
      this.mEffect.Color = MenuItem.COLOR;
      if (this.HasSelectedLevel && this.mGameSettings.Level > -1)
      {
        CampaignNode level = LevelManager.Instance.GetLevel(this.mGameSettings.GameType, this.mGameSettings.Level) as CampaignNode;
        if (level.Cutscene != null)
        {
          Vector2 oPosition;
          float oZoom;
          level.Cutscene.GetCamera(float.MaxValue, out oPosition, out oZoom);
          Rectangle iScrRect = new Rectangle();
          iScrRect.Width = iScrRect.Height = (int) (1024.0 / (double) oZoom);
          iScrRect.X = (int) ((double) oPosition.X - 512.0 / (double) oZoom);
          iScrRect.Y = (int) ((double) oPosition.Y - 512.0 / (double) oZoom);
          float num = 1.3f;
          bool flag3 = this.mSelectedPosition == 9;
          Vector4 iColor = !flag3 ? new Vector4(1f, 1f, 1f, 1f - this.mLevelSelectAlpha) : new Vector4(num, num, num, Math.Min(num, 1f - this.mLevelSelectAlpha));
          this.mEffect.Saturation = flag3 ? 1.3f : 1f;
          this.mEffect.GraphicsDevice.RenderState.AlphaBlendEnable = false;
          this.mEffect.GraphicsDevice.RenderState.SeparateAlphaBlendEnabled = false;
          this.mEffect.GraphicsDevice.RenderState.ColorWriteChannels = ColorWriteChannels.Alpha;
          this.DrawGraphics(this.mMapMaskTexture, new Rectangle(0, 0, this.mMapMaskTexture.Width, this.mMapMaskTexture.Height), this.mMapRect, iColor);
          this.mEffect.GraphicsDevice.RenderState.ColorWriteChannels = ColorWriteChannels.All;
          this.mEffect.GraphicsDevice.RenderState.AlphaBlendEnable = true;
          this.mEffect.GraphicsDevice.RenderState.DestinationBlend = Blend.InverseDestinationAlpha;
          this.mEffect.GraphicsDevice.RenderState.SourceBlend = Blend.DestinationAlpha;
          this.mEffect.GraphicsDevice.RenderState.SeparateAlphaBlendEnabled = true;
          this.mEffect.GraphicsDevice.RenderState.AlphaDestinationBlend = Blend.Zero;
          this.mEffect.GraphicsDevice.RenderState.AlphaSourceBlend = Blend.One;
          if ((double) this.mLevelSelectAlpha == 0.0)
            this.DrawGraphics(this.mMapTexture, iScrRect, this.mMapRect, iColor);
          else
            this.DrawGraphics(this.mMapTexture, iScrRect, this.mMapRect);
          this.mEffect.GraphicsDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
          this.mEffect.GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha;
          this.mEffect.GraphicsDevice.RenderState.AlphaDestinationBlend = Blend.One;
          this.mEffect.GraphicsDevice.RenderState.AlphaSourceBlend = Blend.Zero;
          this.mEffect.Color = Vector4.One;
          this.mEffect.Saturation = 1f;
        }
      }
    }
    int selectedPosition = this.mSelectedPosition;
    this.mEffect.TextureOffset = Vector2.Zero;
    this.mEffect.TextureScale = Vector2.One;
    float num1 = 544f;
    float num2 = 89f;
    iPos1.X = num1;
    iPos1.Y = num2;
    vector4_1 = Vector4.One;
    Magicka.GameLogic.Player[] players = Magicka.Game.Instance.Players;
    this.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mAvatarVertices, 0, 24);
    this.mEffect.GraphicsDevice.Indices = this.mIndexBuffer;
    this.mEffect.GraphicsDevice.VertexDeclaration = this.mAvatarVertexDeclaration;
    for (int iIndex = 0; iIndex < 4; ++iIndex)
    {
      this.DrawSlotBackground(iIndex, ref iPos1, this.mPlayerSlots[iIndex].State == SubMenuCharacterSelect.GamerState.Locked, players[iIndex].Playing, iIndex == this.mSelectedPosition);
      iPos1.Y += 137f;
    }
    this.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertexBuffer, 0, 16 /*0x10*/);
    this.mEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
    iPos1.Y = 89f;
    for (int index = 0; index < 4; ++index)
    {
      Gamer gamer = players[index].Gamer;
      if (players[index].Playing && gamer != null)
      {
        if (gamer == Gamer.INVALID_GAMER)
        {
          this.mEffect.TextureOffset = Vector2.Zero;
          this.mEffect.TextureScale = Vector2.One;
          this.mEffect.Saturation = 1f;
          this.mEffect.Color = Vector4.One;
          iTransform.M11 = 1f;
          iTransform.M22 = 1f;
          iTransform.M41 = iPos1.X + 112f;
          iTransform.M42 = iPos1.Y - 16f;
          this.mEffect.Transform = iTransform;
          this.mEffect.CommitChanges();
          this.mEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 32 /*0x20*/, 0, 16 /*0x10*/, 0, 18);
        }
        else if ((NetworkManager.Instance.State == NetworkState.Client || gamer is NetworkGamer) && (this.mPlayerSlots[index].State == SubMenuCharacterSelect.GamerState.Open || this.mPlayerSlots[index].State == SubMenuCharacterSelect.GamerState.Ready))
        {
          vector4_1.X = vector4_1.Y = vector4_1.Z = 1f;
          vector4_1.W = 1f;
          float num3 = this.mPlayerSlots[index].State == SubMenuCharacterSelect.GamerState.Ready || gamer is NetworkGamer && NetworkManager.Instance.State == NetworkState.Client && (gamer as NetworkGamer).ClientID == NetworkManager.Instance.Interface.ServerID ? 1f : 0.0f;
          if (this.mPlayerSlots[index].SelectedItem == 0)
          {
            num3 += 0.5f;
            vector4_1.X += 0.5f;
            vector4_1.Y += 0.5f;
            vector4_1.Z += 0.5f;
          }
          this.mEffect.Saturation = num3;
          this.mEffect.Color = vector4_1;
          this.mEffect.TextureOffset = new Vector2(912f / (float) SubMenu.sPagesTexture.Width, 416f / (float) SubMenu.sPagesTexture.Height);
          this.mEffect.TextureScale = new Vector2(128f / (float) SubMenu.sPagesTexture.Width, 80f / (float) SubMenu.sPagesTexture.Height);
          iTransform.M11 = 128f;
          iTransform.M22 = 80f;
          iTransform.M41 = (float) ((double) iPos1.X + 448.0 - 64.0 + 48.0);
          iTransform.M42 = (float) ((double) iPos1.Y + 112.0 - 40.0);
          this.mEffect.Transform = iTransform;
          this.mEffect.CommitChanges();
          this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 56, 2);
        }
      }
      iPos1.Y += 137f;
    }
    this.mEffect.TextureOffset = Vector2.Zero;
    this.mEffect.TextureScale = Vector2.One;
    iPos1.Y = 593f;
    iPos1.Y = 764f;
    for (int index = 0; index < 2; ++index)
      iPos1.Y += 112f;
    this.mEffect.Saturation = 1f;
    iPos1.X = 544f;
    iPos1.Y = num2;
    for (int iID = 0; iID < 4; ++iID)
    {
      Gamer gamer = players[iID].Gamer;
      if (players[iID].Playing && gamer != null)
      {
        if (gamer == Gamer.INVALID_GAMER)
        {
          MenuScrollBar mGamerScrollBar = this.mGamerScrollBars[iID];
          Vector2 vector2 = new Vector2();
          mGamerScrollBar.Draw(this.mEffect);
          vector2.X = iPos1.X + 256f;
          vector2.Y = (float) ((double) iPos1.Y + 56.0 - 62.0);
          int num4 = mGamerScrollBar.Value;
          for (int index = num4; index < Math.Min(num4 + 4, this.mGamerItems.Count); ++index)
          {
            if (index < Profile.Instance.Gamers.Count && Profile.Instance.Gamers.Values[index].InUse)
              this.mEffect.Color = MenuItem.COLOR_DISABLED;
            else if (index == this.mPlayerSlots[iID].SelectedItem)
            {
              this.mEffect.Color = MenuItem.COLOR_SELECTED;
            }
            else
            {
              vector4_1 = Defines.DIALOGUE_COLOR_DEFAULT;
              vector4_1.X *= 1.333f;
              vector4_1.Y *= 1.333f;
              vector4_1.Z *= 1.333f;
              this.mEffect.Color = vector4_1;
            }
            float x = this.mGamerItems[index].Font.MeasureText(this.mGamerItems[index].Characters, true).X;
            iTransform.M11 = Math.Min(0.9f, 260f / x);
            iTransform.M22 = 0.9f;
            iTransform.M41 = vector2.X;
            iTransform.M42 = vector2.Y;
            iTransform.M44 = 1f;
            this.mGamerItems[index].Draw(this.mEffect, ref iTransform);
            vector2.Y += 31f;
          }
        }
        else
        {
          switch (gamer)
          {
            case null:
            case NetworkGamer _:
              if (this.mPlayerSlots[iID].State == SubMenuCharacterSelect.GamerState.CustomizingAvatar)
              {
                this.DrawAvatars(iID, ref iPos1);
                this.mEffect.OverlayTextureEnabled = false;
                break;
              }
              if (this.mPlayerSlots[iID].State == SubMenuCharacterSelect.GamerState.CustomizingColor)
              {
                Vector2 iPos4 = new Vector2();
                iPos4.X = (float) ((double) iPos1.X + 224.0 - 144.0);
                iPos4.Y = iPos1.Y + 56f;
                int index1 = this.mPlayerSlots[iID].SelectedItem;
                if (index1 < 0 || index1 >= Defines.PLAYERCOLORS.Length)
                  index1 = (int) players[iID].Color;
                this.DrawAvatar(gamer.Avatar.Thumb, this.mPlayerSlots[iID].Custom, Defines.PLAYERCOLORS[index1], ref iPos4, 0.5f);
                this.mEffect.OverlayTextureEnabled = false;
                Vector2 vector2 = new Vector2();
                vector2.X = -64f / (float) SubMenu.sPagesTexture.Width;
                iTransform.M41 = (float) ((double) iPos1.X + 448.0 - 56.0 - ((double) (Defines.PLAYERCOLORS.Length / 2) - 0.5) * 34.0);
                iTransform.M42 = (float) ((double) iPos1.Y + 56.0 - 17.0);
                for (int index2 = 0; index2 < Defines.PLAYERCOLORS.Length; ++index2)
                {
                  iTransform.M11 = index2 != index1 ? (iTransform.M22 = 38f) : (iTransform.M22 = 50f);
                  this.mEffect.Texture = (Texture) SubMenu.sPagesTexture;
                  this.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertexBuffer, 0, 16 /*0x10*/);
                  this.mEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
                  ref Vector4 local1 = ref vector4_1;
                  ref Vector4 local2 = ref vector4_1;
                  float num5;
                  vector4_1.Z = num5 = index2 == index1 ? 1.5f : 1f;
                  double num6;
                  float num7 = (float) (num6 = (double) num5);
                  local2.Y = (float) num6;
                  double num8 = (double) num7;
                  local1.X = (float) num8;
                  vector4_1.W = 1f;
                  this.mEffect.Color = vector4_1;
                  this.mEffect.Transform = iTransform;
                  this.mEffect.TextureOffset = new Vector2();
                  this.mEffect.CommitChanges();
                  this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 48 /*0x30*/, 2);
                  this.mEffect.Color = new Vector4(Defines.PLAYERCOLORS[index2], 1f);
                  this.mEffect.TextureOffset = vector2;
                  this.mEffect.CommitChanges();
                  this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 48 /*0x30*/, 2);
                  if (index2 == Defines.PLAYERCOLORS.Length / 2)
                  {
                    iTransform.M41 -= 34f * (float) index2;
                    iTransform.M42 += 34f;
                  }
                  else
                    iTransform.M41 += 34f;
                }
                this.mEffect.TextureOffset = new Vector2();
                break;
              }
              Vector2 iPos5 = new Vector2();
              iPos5.X = (float) ((double) iPos1.X + 224.0 - 144.0);
              iPos5.Y = iPos1.Y + 56f;
              Vector2 vector2_1 = this.mPlayerSlots[iID].Name.Font.MeasureText(this.mPlayerSlots[iID].Name.Characters, true);
              iTransform.M11 = Math.Min(1f, 288f / vector2_1.X);
              iTransform.M22 = 1f;
              iTransform.M41 = iPos1.X + 144f;
              iTransform.M42 = iPos1.Y + 16f;
              iTransform.M44 = 1f;
              this.mEffect.Color = Defines.DIALOGUE_COLOR_DEFAULT * 1.2f;
              this.mPlayerSlots[iID].Name.Draw(this.mEffect, ref iTransform);
              if (players[iID].Gamer is NetworkGamer)
              {
                this.mEffect.Color = Vector4.One;
                this.mEffect.VertexColorEnabled = true;
                this.mPlayerSlots[iID].LatencyText.Draw(this.mEffect, iPos1.X + 144f, (float) ((double) iPos1.Y + 112.0 - 16.0) - (float) this.mPlayerSlots[iID].LatencyText.Font.LineHeight);
                this.mEffect.VertexColorEnabled = false;
              }
              this.DrawAvatar(gamer.Avatar.Thumb, this.mPlayerSlots[iID].Custom, Defines.PLAYERCOLORS[(int) players[iID].Color], ref iPos5, 0.5f);
              this.mEffect.OverlayTextureEnabled = false;
              break;
            default:
              int controllerType = this.mPlayerSlots[iID].ControllerType;
              if (controllerType >= 0)
              {
                Rectangle iScrRect = new Rectangle(0, 0, 128 /*0x80*/, 64 /*0x40*/);
                Rectangle iDestRect = new Rectangle(iDestRect.X = (int) ((double) iPos1.X + 448.0 - 108.80000305175781), iDestRect.Y = (int) ((double) iPos1.Y + 56.0 - 32.0 - 54.400001525878906), iScrRect.Width, iScrRect.Height);
                this.DrawGraphics(SubMenuCharacterSelect.mControllerTextures[controllerType], iScrRect, iDestRect, new Vector4(1f, 1f, 1f, 1f));
                goto case null;
              }
              goto case null;
          }
        }
      }
      else if (this.mPlayerSlots[iID].State != SubMenuCharacterSelect.GamerState.Locked)
      {
        this.mEffect.Color = this.mSelectedPosition == iID ? MenuItem.COLOR_SELECTED : MenuItem.COLOR;
        this.mOpenText.Draw(this.mEffect, iPos1.X + 112f, iPos1.Y + 16f);
      }
      else
      {
        this.mEffect.Color = MenuItem.COLOR_DISABLED;
        this.mClosedText.Draw(this.mEffect, iPos1.X + 112f, iPos1.Y + 16f);
      }
      iPos1.Y += 137f;
    }
    if (this.mGameSettings.GameType != GameType.Campaign && this.mGameSettings.GameType != GameType.StoryChallange && this.mGameSettings.GameType != GameType.Mythos && !(this.mVersusSettings is Krietor.Settings))
      this.DrawPacksOverview(mOptionsAlpha);
    this.mStartButton.Position = new Vector2(992f, SubMenu.BACK_POSITION.Y + 40f);
    if (!this.mStartButton.Enabled)
    {
      this.mStartButton.Selected = false;
      Vector4 color = this.mEffect.Color;
      this.mEffect.Color = new Vector4(0.75f, 0.75f, 0.75f, 1f);
      this.mStartButton.Draw(this.mEffect);
      this.mEffect.Color = color;
    }
    else
    {
      this.mStartButton.Selected = this.mSelectedPosition == 4;
      this.mStartButton.Draw(this.mEffect);
    }
    this.mBackButton.Draw(this.mEffect);
    if (this.mGameSettings.GameType == GameType.Versus)
      this.DrawVSSettings(mOptionsAlpha);
    else if (this.mGameSettings.GameType == GameType.Challenge || this.mGameSettings.GameType == GameType.StoryChallange)
    {
      this.mEffect.Saturation = 1f;
      Rectangle iScrRect = new Rectangle();
      iScrRect.X = 800;
      iScrRect.Y = 1250;
      iScrRect.Width = 400;
      iScrRect.Height = 250;
      Rectangle iDestRect = new Rectangle()
      {
        Width = iScrRect.Width,
        Height = iScrRect.Height
      };
      iDestRect.X = 64 /*0x40*/ + (448 - iDestRect.Width) / 2;
      iDestRect.Y = 96 /*0x60*/;
      this.DrawGraphics(this.mMagicksTexture, iScrRect, iDestRect, new Vector4(1f, 1f, 1f, mOptionsAlpha));
    }
    this.mGamerDropDownMenu.Draw(this.mEffect);
    this.mAdminDropDownMenu.Draw(this.mEffect);
    this.mEffect.CurrentTechnique.Passes[0].End();
    this.mEffect.End();
  }

  private void DrawLevel(
    int idx,
    bool iSelected,
    bool iCustom,
    ref Point iPos,
    int iMaxWidth,
    int iMaxHeight,
    float iAlpha)
  {
    if (this.mValidatingLevels)
    {
      Vector4 color = MenuItem.COLOR;
      color.W *= iAlpha;
      this.mEffect.Color = color;
      this.mLoadingText.Draw(this.mEffect, (float) iPos.X + (float) iMaxWidth * 0.5f, (float) iPos.Y + (float) (iMaxHeight - this.mLoadingText.Font.LineHeight) * 0.5f);
    }
    else if (idx == -1)
    {
      this.mSelectLevelButton.Draw(this.mEffect, this.mSelectLevelButton.Scale, 1f);
    }
    else
    {
      Texture2D iTexture = (Texture2D) null;
      if (SubMenuCharacterSelect.mLevelRepresentations != null)
        iTexture = SubMenuCharacterSelect.mLevelRepresentations[idx].PreviewImage;
      if (iTexture == null || iTexture.IsDisposed)
      {
        Vector4 color = MenuItem.COLOR;
        color.W *= iAlpha;
        this.mEffect.Color = color;
        this.mLoadingText.Draw(this.mEffect, (float) iPos.X + (float) iMaxWidth * 0.5f, (float) iPos.Y + (float) (iMaxHeight - this.mLoadingText.Font.LineHeight) * 0.5f);
      }
      else
      {
        Vector4 iColor = new Vector4();
        iColor.W = iAlpha;
        Rectangle iScrRect = new Rectangle(0, 0, iTexture.Width, iTexture.Height);
        float num1;
        if (iSelected)
        {
          num1 = 1.3f;
          iColor.X = iColor.Y = iColor.Z = 1.5f;
        }
        else
        {
          num1 = 1f;
          iColor.X = iColor.Y = iColor.Z = 1f;
        }
        float num2 = Math.Min((float) iMaxWidth / (float) iScrRect.Width, (float) iMaxHeight / (float) iScrRect.Height);
        Rectangle iDestRect = new Rectangle(iPos.X, iPos.Y, (int) ((double) iScrRect.Width * (double) num2), (int) ((double) iScrRect.Height * (double) num2));
        iDestRect.X += (iMaxWidth - iDestRect.Width) / 2;
        this.mEffect.Saturation = num1;
        bool flag1 = this.Level_CheckIfLocked(idx);
        bool flag2 = !flag1 && !this.Level_CheckIfUsed(idx) && !iCustom;
        bool flag3 = this.Level_CheckIfFree(idx);
        bool flag4 = this.Level_CheckIfNew(idx);
        if (iTexture == null || iTexture.IsDisposed)
        {
          if (this.mLoadingLevels)
          {
            Thread.Sleep(0);
          }
          else
          {
            this.mLoadingLevels = true;
            Magicka.Game.Instance.AddLoadTask(new Action(this.LoadLevelPreviews));
          }
        }
        else
        {
          this.DrawGraphics(iTexture, iScrRect, iDestRect, flag1 ? new Vector4(0.75f, 0.75f, 0.75f, 1f) : iColor);
          if (iCustom && !flag1)
          {
            iScrRect.Width = this.mCustomLevelOverlay.Width;
            iScrRect.Height = this.mCustomLevelOverlay.Height;
            Math.Min((float) iMaxWidth / (float) iScrRect.Width, (float) iMaxHeight / (float) iScrRect.Height);
            this.DrawGraphics(this.mCustomLevelOverlay, iScrRect, iDestRect, iColor);
          }
          if (flag3)
          {
            if (flag1)
              this.DrawGraphics(this.mLevelFreeAndLockedOverlay, iScrRect, iDestRect, iColor);
            else if (flag2)
              this.DrawGraphics(this.mLevelFreeAndUnusedOverlay, iScrRect, iDestRect, iColor);
            else
              this.DrawGraphics(this.mLevelFreeOverlay, iScrRect, iDestRect, iColor);
          }
          else if (flag1)
            this.DrawGraphics(this.mLevelLockedOverlay, iScrRect, iDestRect, iColor);
          else if (flag2)
            this.DrawGraphics(this.mLevelUnusedOverlay, iScrRect, iDestRect, iColor);
          if (flag4)
            this.DrawGraphics(this.mLevelNewOverlay, iScrRect, iDestRect, iColor);
          this.mEffect.Saturation = 1f;
        }
      }
    }
  }

  private void DrawVSSettings(float iAlpha)
  {
    Vector4 vector4_1 = new Vector4();
    this.mGameModeBox.Selected = this.mSelectedPosition == 7;
    Vector2 position = this.mGameModeBox.Position;
    position.Y += 16f;
    position.X += this.mGameModeBox.Size.X - 16f;
    position.Y += this.mGameModeBox.Size.Y + 85f;
    vector4_1.X = vector4_1.Y = vector4_1.Z = 1f;
    vector4_1.W = iAlpha;
    this.mSettingsScrollbar.Color = vector4_1;
    this.mSettingsScrollbar.Scale = 0.75f;
    this.mSettingsScrollbar.Height = 244f;
    this.mSettingsScrollbar.Position = position;
    if (this.mSettingsScrollbar.MaxValue > 0)
      this.mSettingsScrollbar.Draw(this.mEffect);
    Vector4 vector4_2 = this.mGameModeBox.Enabled ? (!this.mGameModeBox.Selected || this.mGameModeBox.IsDown ? this.mGameModeBox.Color : this.mGameModeBox.ColorSelected) : this.mGameModeBox.ColorDisabled;
    vector4_2.W *= iAlpha;
    this.mEffect.Color = vector4_2;
    this.mGameModeTitle.Draw(this.mEffect, 64f, this.mGameModeBox.Position.Y);
    int num1 = this.mSelectedPosition - 8;
    if (num1 >= 0 && num1 < this.mVersusSettings.MenuItems.Count)
    {
      while (this.mSettingsScrollbar.Value > num1)
        --this.mSettingsScrollbar.Value;
      while (this.mSettingsScrollbar.Value < this.mSettingsScrollbar.MaxValue && this.mSettingsScrollbar.Value + 5 - 1 < num1)
        ++this.mSettingsScrollbar.Value;
    }
    position.X = (float) ((double) this.mGameModeBox.Position.X + (double) this.mGameModeBox.Size.X - 180.0 - 40.0);
    position.Y = (float) ((double) this.mGameModeBox.Position.Y + (double) this.mGameModeBox.Size.Y + 16.0 + (double) (Math.Min(5, this.mVersusSettings.MenuItems.Count) - 1) * 34.0);
    int num2 = this.mSettingsScrollbar.Value;
    for (int index = Math.Min(num2 + 5, this.mVersusSettings.MenuItems.Count) - 1; index >= num2; --index)
    {
      DropDownBox menuItem = this.mVersusSettings.MenuItems[index];
      menuItem.Selected = this.mSelectedPosition == 8 + index;
      Vector4 vector4_3 = menuItem.Enabled ? (!(menuItem.Selected & !menuItem.IsDown) ? menuItem.Color : menuItem.ColorSelected) : menuItem.ColorDisabled;
      vector4_3.W *= iAlpha;
      this.mEffect.Color = vector4_3;
      this.mVersusSettingsTitles[index].Draw(this.mEffect, 96f, position.Y, 0.9f);
      menuItem.Scale = 0.9f;
      menuItem.Position = position;
      menuItem.Draw(this.mEffect, menuItem.Scale, iAlpha);
      position.Y -= 34f;
    }
    this.mGameModeBox.Draw(this.mEffect, this.mGameModeBox.Scale, iAlpha);
  }

  private void DrawPacksList(float iAlpha)
  {
    ItemPack[] itemPacks = PackMan.Instance.ItemPacks;
    MagickPack[] magickPacks = PackMan.Instance.MagickPacks;
    Texture2D thumbnails = PackMan.Instance.Thumbnails;
    Vector2 vector2_1 = new Vector2();
    Matrix matrix = new Matrix();
    matrix.M44 = 1f;
    matrix.M11 = 64f;
    matrix.M22 = 64f;
    matrix.M42 = this.mSpecialScrollBar.Position.Y - 192f;
    Vector2 vector2_2 = new Vector2(((float) thumbnails.Width - 64f) / (float) thumbnails.Width, ((float) thumbnails.Height - 64f) / (float) thumbnails.Height);
    int num1 = 0;
    int num2 = this.mSpecialScrollBar.Value;
    bool flag = this.mSelectedPosition == 5;
    Vector4 color;
    if (num2 == 0)
    {
      color = MenuItem.COLOR;
      color.W *= iAlpha;
      this.mEffect.Color = color;
      vector2_1.X = vector2_1.Y = 1f;
      this.mEffect.TextureScale = vector2_1;
      this.mEffect.TextureOffset = new Vector2();
      this.mItemsText.Draw(this.mEffect, 96f, matrix.M42 - 14f);
      matrix.M42 += 64f;
      ++num1;
    }
    color.W = iAlpha;
    this.mEffect.Texture = (Texture) thumbnails;
    this.mEffect.TextureEnabled = true;
    this.mEffect.VertexColorEnabled = false;
    vector2_1.X = 64f / (float) thumbnails.Width;
    vector2_1.Y = 64f / (float) thumbnails.Height;
    this.mEffect.TextureScale = vector2_1;
    this.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertexBuffer, 0, 16 /*0x10*/);
    this.mEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
    for (int index1 = 5 * Math.Max(num2 - 1, 0); num1 < 7 && index1 < itemPacks.Length; ++num1)
    {
      matrix.M41 = 160f;
      for (int index2 = 0; index1 < itemPacks.Length && index2 < 5; ++index2)
      {
        ItemPack itemPack = itemPacks[index1];
        if (itemPack.License == HackHelper.License.Yes)
          this.mEffect.TextureOffset = itemPack.ThumbOffset;
        else
          this.mEffect.TextureOffset = new Vector2();
        if (itemPack.Enabled)
          this.mEffect.Saturation = 1f;
        else
          this.mEffect.Saturation = 0.0f;
        color.X = !flag || this.mSelectedPack != index1 ? (color.Y = color.Z = 1f) : (color.Y = color.Z = 1.5f);
        this.mEffect.Color = color;
        this.mEffect.Transform = matrix;
        this.mEffect.CommitChanges();
        this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 56, 2);
        if (!itemPack.IsUsed && itemPack.License == HackHelper.License.Yes && itemPack.Enabled)
        {
          this.mEffect.TextureOffset = vector2_2;
          this.mEffect.CommitChanges();
          this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 56, 2);
        }
        matrix.M41 += 64f;
        ++index1;
      }
      matrix.M42 += 64f;
    }
    int num3 = itemPacks.Length / 5;
    if (itemPacks.Length % 5 != 0)
      ++num3;
    if (num1 < 7 && num3 + 1 - num2 >= 0)
    {
      color = MenuItem.COLOR;
      color.W *= iAlpha;
      this.mEffect.Color = color;
      vector2_1.X = vector2_1.Y = 1f;
      this.mEffect.TextureScale = vector2_1;
      this.mEffect.TextureOffset = new Vector2();
      this.mMagicksText.Draw(this.mEffect, 96f, matrix.M42 - 14f);
      matrix.M42 += 64f;
      ++num1;
    }
    color.W = iAlpha;
    this.mEffect.Texture = (Texture) thumbnails;
    this.mEffect.TextureEnabled = true;
    this.mEffect.VertexColorEnabled = false;
    vector2_1.X = 64f / (float) thumbnails.Width;
    vector2_1.Y = 64f / (float) thumbnails.Height;
    this.mEffect.TextureScale = vector2_1;
    this.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertexBuffer, 0, 16 /*0x10*/);
    this.mEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
    for (int index3 = 5 * Math.Max(num2 - 2 - num3, 0); num1 < 7 && index3 < magickPacks.Length; ++num1)
    {
      matrix.M41 = 160f;
      for (int index4 = 0; index3 < magickPacks.Length && index4 < 5; ++index4)
      {
        MagickPack magickPack = magickPacks[index3];
        if (magickPack.License == HackHelper.License.Yes)
          this.mEffect.TextureOffset = magickPack.ThumbOffset;
        else
          this.mEffect.TextureOffset = new Vector2();
        if (magickPack.Enabled)
          this.mEffect.Saturation = 1f;
        else
          this.mEffect.Saturation = 0.0f;
        color.X = !flag || this.mSelectedPack != index3 + itemPacks.Length ? (color.Y = color.Z = 1f) : (color.Y = color.Z = 1.5f);
        this.mEffect.Color = color;
        this.mEffect.Transform = matrix;
        this.mEffect.CommitChanges();
        this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 56, 2);
        if (!magickPack.IsUsed && magickPack.License == HackHelper.License.Yes && magickPack.Enabled)
        {
          this.mEffect.TextureOffset = vector2_2;
          this.mEffect.CommitChanges();
          this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 56, 2);
        }
        matrix.M41 += 64f;
        ++index3;
      }
      matrix.M42 += 64f;
    }
    this.mEffect.TextureScale = Vector2.One;
    this.mEffect.TextureOffset = Vector2.Zero;
  }

  private void DrawPacksOverview(float iAlpha)
  {
    bool flag1 = this.mSelectedPosition == 5;
    bool flag2 = DLC_StatusHelper.HasAnyUnusedItemPacks() || DLC_StatusHelper.HasAnyUnusedMagicPacks();
    Vector4 vector4 = flag1 ? MenuItem.COLOR_SELECTED : MenuItem.COLOR;
    vector4.W *= iAlpha;
    this.mEffect.Color = vector4;
    this.mPacksText.Draw(this.mEffect, 128f, 368f);
    if (flag2 && this.mCurrentState == SubMenuCharacterSelect.State.Normal && !this.mValidatingLevels)
    {
      this.mGenericStar.Position = new Vector2(128f + (this.mFont.MeasureText(LanguageManager.Instance.GetString(SubMenuCharacterSelect.LOC_PACKS), false).X + 15f), 368f);
      this.mGenericStar.Draw(this.mEffect);
    }
    Texture2D thumbnails = PackMan.Instance.Thumbnails;
    Vector2 vector2 = new Vector2();
    Matrix matrix = new Matrix();
    matrix.M44 = 1f;
    this.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertexBuffer, 0, 16 /*0x10*/);
    this.mEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
    float num1 = 288f;
    IPack[] allPacks = PackMan.Instance.AllPacks;
    int val1 = 0;
    for (int index = 0; index < allPacks.Length; ++index)
    {
      if (allPacks[index].Enabled)
        ++val1;
    }
    bool flag3 = val1 > 5;
    vector4.W = iAlpha;
    this.mEffect.Texture = (Texture) SubMenu.sPagesTexture;
    vector2.X = 64f / (float) SubMenu.sPagesTexture.Width;
    vector2.Y = 64f / (float) SubMenu.sPagesTexture.Height;
    this.mEffect.TextureScale = vector2;
    this.mEffect.TextureOffset = new Vector2()
    {
      X = 1280f / (float) SubMenu.sPagesTexture.Width,
      Y = 96f / (float) SubMenu.sPagesTexture.Height
    };
    if (flag3)
    {
      matrix.M42 = 440f;
      matrix.M11 = 0.0f;
      matrix.M22 = 0.0f;
      matrix.M41 = num1 - 192f;
      matrix.M12 = -64f;
      matrix.M21 = 64f;
      this.mEffect.Transform = matrix;
      if (flag1 && this.mSelectedPackScroll == ControllerDirection.Left)
      {
        vector4.X = vector4.Y = vector4.Z = 1.5f;
        this.mEffect.Saturation = 1.5f;
      }
      else
      {
        vector4.X = vector4.Y = vector4.Z = 1f;
        this.mEffect.Saturation = 1f;
      }
      this.mEffect.Color = vector4;
      this.mEffect.CommitChanges();
      this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 56, 2);
      matrix.M41 = num1 + 192f;
      matrix.M12 = 64f;
      matrix.M21 = -64f;
      this.mEffect.Transform = matrix;
      if (flag1 && this.mSelectedPackScroll == ControllerDirection.Right)
      {
        vector4.X = vector4.Y = vector4.Z = 1.5f;
        this.mEffect.Saturation = 1.5f;
      }
      else
      {
        vector4.X = vector4.Y = vector4.Z = 1f;
        this.mEffect.Saturation = 1f;
      }
      this.mEffect.Color = vector4;
      this.mEffect.CommitChanges();
      this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 56, 2);
    }
    matrix.M41 = num1 - 128f;
    matrix.M42 = 440f;
    matrix.M11 = 64f;
    matrix.M12 = 0.0f;
    matrix.M22 = 64f;
    matrix.M21 = 0.0f;
    this.mEffect.Texture = (Texture) thumbnails;
    vector2.X = 64f / (float) thumbnails.Width;
    vector2.Y = 64f / (float) thumbnails.Height;
    this.mEffect.TextureScale = vector2;
    this.mEffect.Saturation = 1f;
    if (!flag3)
      this.mPackScrollValue = 0;
    int mPackScrollValue = this.mPackScrollValue;
    int index1 = mPackScrollValue;
    int num2 = Math.Min(val1, 5);
    for (int index2 = 0; index2 < num2; ++index2)
    {
      while (!allPacks[index1].Enabled)
      {
        index1 = (index1 + 1) % allPacks.Length;
        if (index1 == mPackScrollValue)
          goto label_22;
      }
      vector4.W = 1f * iAlpha;
      vector4.X = !flag1 || this.mSelectedPack != index1 ? (vector4.Y = vector4.Z = 1f) : (vector4.Y = vector4.Z = 1.5f);
      this.mEffect.Color = vector4;
      this.mEffect.TextureOffset = allPacks[index1].ThumbOffset;
      this.mEffect.Transform = matrix;
      this.mEffect.CommitChanges();
      this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 56, 2);
      matrix.M41 += 64f;
      index1 = (index1 + 1) % allPacks.Length;
    }
label_22:
    this.mEffect.TextureScale = Vector2.One;
    this.mEffect.TextureOffset = Vector2.Zero;
  }

  private void DrawSlotBackground(
    int iIndex,
    ref Vector2 iPos,
    bool iLocked,
    bool iInUse,
    bool iSelected)
  {
    this.mEffect.Texture = (Texture) (this.mEffect.OverlayTexture = SubMenu.sPagesTexture);
    this.mEffect.TextureEnabled = this.mEffect.OverlayTextureEnabled = true;
    Matrix matrix = new Matrix();
    matrix.M11 = matrix.M22 = matrix.M44 = 1f;
    matrix.M41 = iPos.X;
    matrix.M42 = iPos.Y;
    this.mEffect.Transform = matrix;
    this.mEffect.GraphicsDevice.RenderState.CullMode = CullMode.None;
    if (iLocked)
      this.mEffect.Saturation = 0.0f;
    else if (iSelected)
      this.mEffect.Saturation = 1.5f;
    else
      this.mEffect.Saturation = 1f;
    Vector4 vector4 = new Vector4();
    vector4.X = vector4.Y = vector4.Z = 1f;
    vector4.W = 1f;
    if (!iInUse)
      vector4.W = 0.66f;
    ref Vector4 local1 = ref vector4;
    ref Vector4 local2 = ref vector4;
    float num1;
    vector4.Z = num1 = iSelected ? 1.5f : 1f;
    double num2;
    float num3 = (float) (num2 = (double) num1);
    local2.Y = (float) num2;
    double num4 = (double) num3;
    local1.X = (float) num4;
    this.mEffect.Color = vector4;
    if (this.mGameSettings.GameType == GameType.Versus && this.mVersusSettings.TeamsEnabled)
    {
      Magicka.GameLogic.Player player = Magicka.Game.Instance.Players[iIndex];
      if (player.Playing && (player.Team & Factions.TEAM_RED) != Factions.NONE)
      {
        vector4.X = Defines.PLAYERCOLORS[0].X;
        vector4.Y = Defines.PLAYERCOLORS[0].Y;
        vector4.Z = Defines.PLAYERCOLORS[0].Z;
      }
      else if (player.Playing && (player.Team & Factions.TEAM_BLUE) != Factions.NONE)
      {
        vector4.X = Defines.PLAYERCOLORS[3].X;
        vector4.Y = Defines.PLAYERCOLORS[3].Y;
        vector4.Z = Defines.PLAYERCOLORS[3].Z;
      }
      else
      {
        vector4.X = 0.75f;
        vector4.Y = 0.75f;
        vector4.Z = 0.75f;
      }
    }
    else
    {
      vector4.X = 0.75f;
      vector4.Y = 0.75f;
      vector4.Z = 0.75f;
    }
    vector4.W = 1f;
    this.mEffect.OverlayTint = vector4;
    this.mEffect.CommitChanges();
    this.mEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 4, 0, 16 /*0x10*/, 0, 18);
    this.mEffect.OverlayTextureEnabled = false;
  }

  private void DrawAvatars(int iID, ref Vector2 iPos)
  {
    Magicka.GameLogic.Player player = Magicka.Game.Instance.Players[iID];
    Gamer gamer = player.Gamer;
    SortedList<string, Profile.PlayableAvatar> avatars = Profile.Instance.Avatars;
    Vector2 iPos1 = new Vector2();
    iPos1.X = (float) ((double) iPos.X + 224.0 - 144.0);
    iPos1.Y = iPos.Y + 56f;
    if (this.mValidatingLevels)
    {
      Vector4 color = MenuItem.COLOR;
      color.W *= 1f;
      this.mEffect.Color = color;
      this.mLoadingText.Draw(this.mEffect, iPos.X - 72f, iPos.Y + (float) this.mLoadingText.Font.LineHeight * 0.5f);
    }
    else
    {
      bool flag1 = SubMenuCharacterSelect.mRobeRepresentations.Count > 4;
      if (flag1)
      {
        this.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertexBuffer, 0, 16 /*0x10*/);
        this.mEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
        this.mEffect.Texture = (Texture) SubMenu.sPagesTexture;
        this.mEffect.Color = new Vector4(1.25f, 1.25f, 1.25f, 1f);
        Matrix matrix = new Matrix();
        matrix.M12 = -64f;
        matrix.M21 = 64f;
        matrix.M41 = iPos1.X - 72f;
        matrix.M42 = iPos1.Y;
        matrix.M44 = 1f;
        this.mEffect.Transform = matrix;
        this.mEffect.CommitChanges();
        this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 52, 2);
        matrix.M12 = 64f;
        matrix.M21 = -64f;
        matrix.M41 += 432f;
        this.mEffect.Transform = matrix;
        this.mEffect.CommitChanges();
        this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 52, 2);
      }
      int num = 0;
      if (flag1)
      {
        if (this.mPlayerSlots[iID].ScrollValue >= SubMenuCharacterSelect.mRobeRepresentations.Count)
          this.mPlayerSlots[iID].ScrollValue = SubMenuCharacterSelect.mRobeRepresentations.Count - 1;
        num = this.mPlayerSlots[iID].ScrollValue;
      }
      for (int index = 0; index < Math.Min(SubMenuCharacterSelect.mRobeRepresentations.Count, 4); ++index)
      {
        bool flag2 = this.Robe_CheckIfLocked(num);
        bool flag3 = !flag2 && !this.Robe_CheckIfUsed(num);
        bool flag4 = this.Robe_CheckIfFree(num);
        bool flag5 = this.Robe_CheckIfNew(num);
        float iScale;
        if (this.mPlayerSlots[iID].SelectedItem == num || this.mPlayerSlots[iID].SelectedItem < 0 && gamer != null && avatars.IndexOfKey(gamer.Avatar.Name) == SubMenuCharacterSelect.mRobeRepresentations[num].OriginalIndex)
        {
          iScale = 0.5f;
          this.mEffect.Saturation = flag2 ? 0.25f : 1f;
        }
        else
        {
          iScale = 0.4f;
          this.mEffect.Saturation = 0.25f;
        }
        Vector3 pColor = Defines.PLAYERCOLORS[(int) player.Color] * (flag2 ? 0.75f : 1f);
        Texture2D thumb = avatars.Values[SubMenuCharacterSelect.mRobeRepresentations[num].OriginalIndex].Thumb;
        this.DrawAvatar(thumb, SubMenuCharacterSelect.mRobeRepresentations[num].IsCustom, pColor, ref iPos1, iScale);
        Vector2 iPos2 = iPos1;
        iPos2.X += 3f;
        iPos2.Y += 33f;
        this.mEffect.Saturation = 1f;
        if (flag4)
        {
          if (flag2)
            this.DrawAvatar(this.mRobeFreeAndLockedOverlay, false, Vector3.One, ref iPos2, iScale);
          else if (flag3)
            this.DrawAvatar(this.mRobeFreeAndUnusedOverlay, false, Vector3.One, ref iPos2, iScale);
          else
            this.DrawAvatar(this.mRobeFreeOverlay, false, Vector3.One, ref iPos2, iScale);
        }
        else if (flag2)
          this.DrawAvatar(this.mRobeLockedOverlay, false, Vector3.One, ref iPos2, iScale);
        else if (flag3)
          this.DrawAvatar(this.mRobeUnusedOverlay, false, Vector3.One, ref iPos2, iScale);
        if (flag5)
        {
          iPos2 = iPos1;
          iPos2.X += (float) (thumb.Width - this.mRobeNewOverlay.Width) - 6f;
          iPos2.Y -= 26f;
          this.DrawAvatar(this.mRobeNewOverlay, false, Vector3.One, ref iPos2, iScale);
        }
        iPos1.X += 96f;
        num = (num + 1) % SubMenuCharacterSelect.mRobeRepresentations.Count;
      }
    }
    this.mEffect.Saturation = 1f;
  }

  private void DrawAvatar(
    Texture2D iTexture,
    bool iCustom,
    Vector3 pColor,
    ref Vector2 iPos,
    float iScale)
  {
    this.mEffect.Color = Vector4.One;
    this.mEffect.OverlayTint = new Vector4(pColor.X, pColor.Y, pColor.Z, 1f);
    this.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mAvatarVertices, 0, 24);
    this.mEffect.GraphicsDevice.VertexDeclaration = this.mAvatarVertexDeclaration;
    this.mEffect.Texture = (Texture) (this.mEffect.OverlayTexture = iTexture);
    this.mEffect.TextureEnabled = true;
    this.mEffect.OverlayTextureEnabled = true;
    Matrix matrix = new Matrix();
    matrix.M41 = iPos.X;
    matrix.M42 = iPos.Y;
    matrix.M11 = (float) iTexture.Width * iScale;
    matrix.M22 = (float) iTexture.Height * 0.5f * iScale;
    matrix.M44 = 1f;
    this.mEffect.Transform = matrix;
    this.mEffect.CommitChanges();
    this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
    if (!iCustom)
      return;
    this.mEffect.TextureOffset = Vector2.Zero;
    this.mEffect.TextureScale = Vector2.One;
    this.mEffect.Texture = (Texture) this.mCustomTexture;
    this.mEffect.OverlayTextureEnabled = false;
    this.mEffect.Color = Vector4.One;
    matrix.M42 += (float) (0.25 * (double) iTexture.Height - (double) this.mCustomTexture.Height * 0.5) * iScale;
    matrix.M11 = (float) this.mCustomTexture.Width * iScale;
    matrix.M22 = (float) this.mCustomTexture.Height * iScale;
    this.mEffect.Transform = matrix;
    this.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertexBuffer, 0, 16 /*0x10*/);
    this.mEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
    this.mEffect.GraphicsDevice.SamplerStates[0].AddressU = TextureAddressMode.Clamp;
    this.mEffect.GraphicsDevice.SamplerStates[0].AddressV = TextureAddressMode.Clamp;
    this.mEffect.CommitChanges();
    this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 56, 2);
  }

  private void NewGamerCreated(string iName)
  {
    if (string.IsNullOrEmpty(iName))
      return;
    string key = (string) null;
    if (this.mGameSettings.Level != -1)
    {
      switch (this.mGameSettings.GameType)
      {
        case GameType.Campaign:
          key = LevelManager.Instance.VanillaCampaign[this.mGameSettings.Level].PreferredAvatar;
          break;
        case GameType.Challenge:
          key = LevelManager.Instance.Challenges[this.mGameSettings.Level].PreferredAvatar;
          break;
        case GameType.Versus:
          key = LevelManager.Instance.Versus[this.mGameSettings.Level].PreferredAvatar;
          break;
        case GameType.Mythos:
          key = LevelManager.Instance.MythosCampaign[this.mGameSettings.Level].PreferredAvatar;
          break;
        case GameType.StoryChallange:
          key = LevelManager.Instance.StoryChallanges[this.mGameSettings.Level].PreferredAvatar;
          break;
      }
    }
    Profile.PlayableAvatar playableAvatar = Profile.Instance.DefaultAvatar;
    Profile.PlayableAvatar iAvatar;
    if (!string.IsNullOrEmpty(key) && Profile.Instance.Avatars.TryGetValue(key, out iAvatar))
    {
      switch (HackHelper.CheckLicense(iAvatar))
      {
        case HackHelper.License.Yes:
          playableAvatar = iAvatar;
          break;
        case HackHelper.License.Custom:
          if (!this.AllowCustom)
            break;
          goto case HackHelper.License.Yes;
      }
    }
    Gamer iGamer = new Gamer(iName);
    iGamer.Avatar = playableAvatar;
    iGamer.Color = (byte) (Profile.Instance.Gamers.Count % Defines.PLAYERCOLORS.Length);
    Profile.Instance.Add(iGamer);
    this.GamerSelected(this.mNameInputController, iGamer);
    this.mNameInputController = (Controller) null;
  }

  private void UpdateControllerIcon(Controller controller, int slotIndex)
  {
    int num;
    switch (controller)
    {
      case KeyboardMouseController _:
        num = 0;
        break;
      case XInputController _:
        num = (int) ((controller as XInputController).PlayerIndex + 1);
        break;
      default:
        num = -1;
        break;
    }
    this.mPlayerSlots[slotIndex].ControllerType = num;
  }

  private void DaisyWheelChatInput(string text)
  {
    DaisyWheel.SetActionToCallWhenComplete((Action<string>) null);
    if (text == null)
      return;
    NetworkChat.Instance.AddMessage(text);
  }

  public override void ControllerA(Controller iSender)
  {
    if ((double) this.mOptionsAlpha > 0.0 && (double) this.mOptionsAlpha < 1.0 || RenderManager.Instance.IsTransitionActive)
      return;
    if (iSender.Player == null || !iSender.Player.Playing)
    {
      this.JoinPlayer(iSender, -1, (Gamer) null);
    }
    else
    {
      int id = iSender.Player.ID;
      if (iSender.Player.Gamer != Gamer.INVALID_GAMER && iSender is XInputController && this.mSelectedPosition == -1 && this.mCurrentState == SubMenuCharacterSelect.State.Normal && this.mPlayerSlots[id].State != SubMenuCharacterSelect.GamerState.CustomizingAvatar && this.mPlayerSlots[id].State != SubMenuCharacterSelect.GamerState.CustomizingColor)
      {
        if (DaisyWheel.IsDisplaying)
          return;
        DaisyWheel.SetActionToCallWhenComplete(new Action<string>(this.DaisyWheelChatInput));
        DaisyWheel.TryShow(iSender, LanguageManager.Instance.GetString(SubMenuCharacterSelect.LOC_CHATMESSAGE), false, GamepadTextInputLineMode.GamepadTextInputLineModeMultipleLines, (uint) byte.MaxValue);
      }
      else
      {
        this.UpdateControllerIcon(iSender, iSender.Player.ID);
        if (iSender.Player.Gamer == Gamer.INVALID_GAMER)
        {
          int selectedItem = this.mPlayerSlots[id].SelectedItem;
          if (selectedItem >= Profile.Instance.Gamers.Count)
          {
            this.mNameInputController = iSender;
            string upper = LanguageManager.Instance.GetString(SubMenuCharacterSelect.LOC_ENTER_NAME).ToUpper();
            this.mNameInputBox.Show(new Action<string>(this.NewGamerCreated), iSender, upper);
          }
          else
          {
            if (selectedItem < 0 || Profile.Instance.Gamers.Values[selectedItem].InUse)
              return;
            this.GamerSelected(iSender, Profile.Instance.Gamers.Values[selectedItem]);
            Magicka.GameLogic.Player player = iSender.Player;
            if (player.Gamer == null)
              return;
            this.VerifyAvatar(ref player, false);
          }
        }
        else if (this.mPlayerSlots[id].State == SubMenuCharacterSelect.GamerState.Open && NetworkManager.Instance.State == NetworkState.Client)
          this.SetReady(true, (byte) id);
        else if (this.mPlayerSlots[id].State == SubMenuCharacterSelect.GamerState.CustomizingAvatar)
        {
          int selectedItem = this.mPlayerSlots[id].SelectedItem;
          if (selectedItem < 0 || selectedItem >= SubMenuCharacterSelect.mRobeRepresentations.Count)
            return;
          Profile.PlayableAvatar playableAvatar = Profile.Instance.Avatars.Values[SubMenuCharacterSelect.mRobeRepresentations[selectedItem].OriginalIndex];
          uint appID = 0;
          uint storePageAppID = 0;
          if (DLC_StatusHelper.ValidateRobeLocked(playableAvatar, out appID, out storePageAppID))
          {
            SteamUtils.ActivateGameOverlayToWebPage($"http://store.steampowered.com/app/{(object) storePageAppID}/");
          }
          else
          {
            switch (HackHelper.CheckLicense(playableAvatar))
            {
              case HackHelper.License.Yes:
                if (!iSender.Player.Gamer.Avatar.Name.Equals(playableAvatar.Name))
                  this.mPlayerSlots[id].ConsecutiveColorChanges = -1;
                iSender.Player.Gamer.Avatar = playableAvatar;
                this.mPlayerSlots[id].Custom = SubMenuCharacterSelect.mRobeRepresentations[selectedItem].IsCustom;
                if (NetworkManager.Instance.State != NetworkState.Offline)
                {
                  GamerChangedMessage iMessage = new GamerChangedMessage(iSender.Player);
                  NetworkManager.Instance.Interface.SendMessage<GamerChangedMessage>(ref iMessage);
                }
                ToolTipMan.Instance.Kill(iSender.Player, false);
                this.SetRobeUsed(playableAvatar.Name);
                this.mPlayerSlots[id].State = SubMenuCharacterSelect.GamerState.CustomizingColor;
                this.mPlayerSlots[id].SelectedItem = (int) iSender.Player.Color;
                break;
              case HackHelper.License.Custom:
                if (NetworkManager.Instance.State != NetworkState.Offline && NetworkManager.Instance.Interface.IsVACSecure)
                  break;
                goto case HackHelper.License.Yes;
            }
          }
        }
        else if (this.mPlayerSlots[id].State == SubMenuCharacterSelect.GamerState.CustomizingColor)
        {
          if (this.mPlayerSlots[id].SelectedItem < 0 || this.mPlayerSlots[id].SelectedItem >= Defines.PLAYERCOLORS.Length)
            return;
          if (this.mPlayerSlots[id].SelectedItem != (int) iSender.Player.Color)
          {
            ++this.mPlayerSlots[id].ConsecutiveColorChanges;
            if (this.mPlayerSlots[id].ConsecutiveColorChanges == 2)
              AchievementsManager.Instance.AwardAchievement((PlayState) null, "bluenoyelloooow");
          }
          this.mPlayerSlots[id].State = SubMenuCharacterSelect.GamerState.Open;
          iSender.Player.Color = (byte) this.mPlayerSlots[id].SelectedItem;
        }
        else
        {
          if (NetworkManager.Instance.State == NetworkState.Client)
            return;
          if (this.mGameModeBox.IsDown)
          {
            this.mGameModeBox.SelectedIndex = this.mGameModeBox.NewSelection;
            this.mGameModeBox.IsDown = false;
          }
          else
          {
            for (int index = 0; index < this.mVersusSettings.MenuItems.Count; ++index)
            {
              DropDownBox menuItem = this.mVersusSettings.MenuItems[index];
              if (menuItem.IsDown)
              {
                menuItem.SelectedIndex = menuItem.NewSelection;
                menuItem.IsDown = false;
                return;
              }
            }
            if (this.mCurrentState == SubMenuCharacterSelect.State.ChangingLevel)
            {
              if (this.mSelectedPack < 0 || this.mSelectedPack >= SubMenuCharacterSelect.mLevelRepresentations.Count)
                return;
              int levelOriginalIndex = this.GetLevelOriginalIndex(this.mSelectedPack);
              this.OnLevelChange(iSender, this.mGameSettings.GameType, levelOriginalIndex);
              ToolTipMan.Instance.Kill((Magicka.GameLogic.Player) null, false);
              this.ChangeState(iSender, SubMenuCharacterSelect.State.Normal);
            }
            else if (this.mCurrentState == SubMenuCharacterSelect.State.ChangingPacks)
            {
              if (this.mSelectedPack >= 0 && this.mSelectedPack < PackMan.Instance.AllPacks.Length)
              {
                IPack allPack = PackMan.Instance.AllPacks[this.mSelectedPack];
                allPack.Enabled = !allPack.Enabled;
              }
              else
              {
                this.mSelectedPosition = 5;
                this.ChangeState(iSender, SubMenuCharacterSelect.State.Normal);
              }
            }
            else if (this.mCurrentState == SubMenuCharacterSelect.State.Normal && this.mSelectedPosition == 9 && this.mGameSettings.GameType != GameType.Versus)
            {
              this.mSpecialScrollBar.Height = 840f;
              this.mSpecialScrollBar.SetMaxValue(SubMenuCharacterSelect.mLevelRepresentations.Count - 6);
              ToolTipMan.Instance.Kill((Magicka.GameLogic.Player) null, false);
              if (this.mGameSettings.GameType == GameType.Challenge || this.mGameSettings.GameType == GameType.Versus)
                this.SortLevelRepList();
              this.ChangeState(iSender, SubMenuCharacterSelect.State.ChangingLevel);
            }
            else if (this.mSelectedPosition == 6 && this.mGameSettings.GameType != GameType.Campaign && this.mGameSettings.GameType != GameType.Mythos && NetworkManager.Instance.State != NetworkState.Client)
            {
              if (!this.HasSelectedLevel && (SubMenuCharacterSelect.mLevelRepresentations == null || SubMenuCharacterSelect.mLevelRepresentations.Count == 0 || !this.mSelectLevelButton.Enabled))
                return;
              this.mSpecialScrollBar.Height = this.mSpecialScrollBar.Height = 840f;
              this.mSpecialScrollBar.SetMaxValue(SubMenuCharacterSelect.mLevelRepresentations.Count - 6);
              ToolTipMan.Instance.Kill((Magicka.GameLogic.Player) null, false);
              if (this.mGameSettings.GameType == GameType.Challenge || this.mGameSettings.GameType == GameType.Versus)
                this.SortLevelRepList();
              this.ChangeState(iSender, SubMenuCharacterSelect.State.ChangingLevel);
              this.mCurrentController = iSender;
            }
            else if (this.mSelectedPosition == 5 && this.mGameSettings.GameType != GameType.Campaign && this.mGameSettings.GameType != GameType.Mythos && NetworkManager.Instance.State != NetworkState.Client)
            {
              if (NetworkManager.Instance.State == NetworkState.Client)
                return;
              int length1 = PackMan.Instance.ItemPacks.Length;
              int length2 = PackMan.Instance.MagickPacks.Length;
              int num = 2 + length1 / 5 + length2 / 5;
              if (length1 % 5 != 0)
                ++num;
              if (length2 % 5 != 0)
                ++num;
              this.mSpecialScrollBar.Height = 448f;
              this.mSpecialScrollBar.SetMaxValue(num - 7);
              ToolTipMan.Instance.Kill((Magicka.GameLogic.Player) null, false);
              this.mSelectedPack = 0;
              this.mSpecialScrollBar.Value = 0;
              this.ChangeState(iSender, SubMenuCharacterSelect.State.ChangingPacks);
            }
            else if (this.mSelectedPosition == 4 && this.mStartButton.Enabled && this.CanStart())
            {
              switch (NetworkManager.Instance.State)
              {
                case NetworkState.Offline:
                  this.StartLevel();
                  break;
                case NetworkState.Server:
                  this.Start();
                  break;
              }
            }
            else if (this.mSelectedPosition == 7 && this.mGameSettings.GameType == GameType.Versus && this.mGameModeBox.Enabled)
            {
              this.mGameModeBox.IsDown = true;
            }
            else
            {
              if (this.mSelectedPosition < 8 || this.mGameSettings.GameType != GameType.Versus)
                return;
              DropDownBox menuItem = this.mVersusSettings.MenuItems[this.mSelectedPosition - 8];
              if (!menuItem.Enabled)
                return;
              menuItem.IsDown = true;
            }
          }
        }
      }
    }
  }

  private void ChangeState(Controller iSender, SubMenuCharacterSelect.State iNewState)
  {
    this.mCurrentState = iNewState;
    this.mCurrentController = iSender;
    if (iNewState == SubMenuCharacterSelect.State.Normal)
      this.mCurrentController = (Controller) null;
    if (this.HasSelectedLevel)
      return;
    this.mChapterName.SetText("");
    this.mChapterDescription.SetText("");
    this.mCurrentController = (Controller) null;
    this.mStartButton.Enabled = false;
    this.mSelectLevelButton.Enabled = false;
  }

  public bool AllowCustom
  {
    get
    {
      return NetworkManager.Instance.State == NetworkState.Offline || !NetworkManager.Instance.Interface.IsVACSecure;
    }
  }

  public override void ControllerB(Controller iSender)
  {
    if (this.mGameModeBox.IsDown)
    {
      this.mGameModeBox.IsDown = false;
    }
    else
    {
      for (int index = 0; index < this.mVersusSettings.MenuItems.Count; ++index)
      {
        if (this.mVersusSettings.MenuItems[index].IsDown)
        {
          this.mVersusSettings.MenuItems[index].IsDown = false;
          return;
        }
      }
      bool flag1 = this.mCurrentState != SubMenuCharacterSelect.State.Normal && this.mCurrentController == iSender;
      if (flag1 && this.mCurrentState == SubMenuCharacterSelect.State.ChangingPacks)
      {
        this.mSelectedPosition = 5;
        this.ChangeState(iSender, SubMenuCharacterSelect.State.Normal);
      }
      else if (this.mCurrentController == iSender && this.mCurrentState == SubMenuCharacterSelect.State.ChangingLevel)
      {
        if (this.mGameSettings.GameType != GameType.Campaign && this.mGameSettings.GameType != GameType.Mythos)
        {
          this.HasSelectedLevel = false;
          this.SetNoLevelSelected();
          this.mSelectLevelButton.Enabled = this.mSelectLevelButton.Selected = true;
          this.mSelectedPosition = 6;
        }
        this.ChangeState(iSender, SubMenuCharacterSelect.State.Normal);
      }
      else if (flag1 && this.mCurrentState != SubMenuCharacterSelect.State.Normal)
        this.ChangeState(iSender, SubMenuCharacterSelect.State.Normal);
      else if (iSender.Player == null)
      {
        bool flag2 = false;
        for (int index = 0; index < this.mPlayerSlots.Length; ++index)
        {
          if (this.mPlayerSlots[index].ControllerType != -1)
          {
            flag2 = true;
            break;
          }
        }
        if (flag2)
          return;
        if (NetworkManager.Instance.HasHostSettings && (NetworkManager.Instance.GameType == GameType.Campaign || NetworkManager.Instance.GameType == GameType.Mythos))
          Tome.Instance.PopPreviousMenu();
        NetworkManager.Instance.EndSession();
        Tome.Instance.PopMenu();
      }
      else if (iSender.Player.Playing)
      {
        byte id = (byte) iSender.Player.ID;
        if (this.mPlayerSlots[(int) id].State == SubMenuCharacterSelect.GamerState.Open)
        {
          this.GamerSelected(iSender, (Gamer) null);
          this.mStartButton.Enabled = this.CanStart();
        }
        else if (this.mPlayerSlots[(int) id].State == SubMenuCharacterSelect.GamerState.Ready)
          this.SetReady(false, id);
        else if (this.mPlayerSlots[(int) id].State == SubMenuCharacterSelect.GamerState.CustomizingAvatar)
        {
          ToolTipMan.Instance.Kill(iSender.Player, false);
          this.mPlayerSlots[(int) id].State = SubMenuCharacterSelect.GamerState.Open;
          this.mStartButton.Enabled = this.CanStart();
        }
        else
        {
          if (this.mPlayerSlots[(int) id].State != SubMenuCharacterSelect.GamerState.CustomizingColor)
            return;
          int num1 = Profile.Instance.Avatars.IndexOfKey(iSender.Player.Gamer.Avatar.Name);
          int num2 = 0;
          for (int index = 0; index < SubMenuCharacterSelect.mRobeRepresentations.Count; ++index)
          {
            if (SubMenuCharacterSelect.mRobeRepresentations[index].OriginalIndex == num1)
            {
              num2 = index;
              break;
            }
          }
          this.mPlayerSlots[(int) id].SelectedItem = this.mPlayerSlots[(int) id].ScrollValue = num2;
          if (this.mDefaultAvatars)
            this.mPlayerSlots[(int) id].State = SubMenuCharacterSelect.GamerState.Open;
          else
            this.mPlayerSlots[(int) id].State = SubMenuCharacterSelect.GamerState.CustomizingAvatar;
        }
      }
      else
      {
        if (NetworkManager.Instance.HasHostSettings && (NetworkManager.Instance.GameType == GameType.Campaign || NetworkManager.Instance.GameType == GameType.Mythos))
          Tome.Instance.PopPreviousMenu();
        Tome.Instance.PopMenu();
        NetworkManager.Instance.EndSession();
      }
    }
  }

  public override void ControllerX(Controller iSender)
  {
    base.ControllerX(iSender);
    if (this.mVersusSettings.TeamsEnabled && this.mGameSettings.GameType == GameType.Versus)
    {
      if (iSender.Player == null || !iSender.Player.Playing)
        return;
      iSender.Player.Team = (iSender.Player.Team & Factions.TEAM_RED) == Factions.NONE ? Factions.TEAM_RED : Factions.TEAM_BLUE;
      if (NetworkManager.Instance.State == NetworkState.Offline)
        return;
      NetworkManager.Instance.Interface.SendMessage<MenuSelectMessage>(ref new MenuSelectMessage()
      {
        IntendedMenu = MenuSelectMessage.MenuType.CharacterSelect,
        Option = 2,
        Param0I = iSender.Player.ID,
        Param1I = (int) iSender.Player.Team
      });
    }
    else
    {
      if (iSender.Player == null || !iSender.Player.Playing)
        return;
      iSender.Player.Team = Factions.NONE;
    }
  }

  public override void ControllerY(Controller iSender)
  {
    base.ControllerY(iSender);
    if (iSender.Player == null || !iSender.Player.Playing)
      return;
    int id = iSender.Player.ID;
    if (this.mPlayerSlots[id].State != SubMenuCharacterSelect.GamerState.Open)
      return;
    int num = Profile.Instance.Avatars.IndexOfKey(iSender.Player.Gamer.Avatar.Name);
    for (int index = 0; index < SubMenuCharacterSelect.mRobeRepresentations.Count; ++index)
    {
      if (num == SubMenuCharacterSelect.mRobeRepresentations[index].OriginalIndex)
      {
        num = index;
        break;
      }
    }
    this.mPlayerSlots[id].ConsecutiveColorChanges = Math.Max(0, this.mPlayerSlots[id].ConsecutiveColorChanges);
    this.mPlayerSlots[id].SelectedItem = this.mPlayerSlots[id].ScrollValue = num;
    if (this.mDefaultAvatars)
      this.mPlayerSlots[id].State = SubMenuCharacterSelect.GamerState.CustomizingColor;
    else
      this.mPlayerSlots[id].State = SubMenuCharacterSelect.GamerState.CustomizingAvatar;
  }

  public override void ControllerDown(Controller iSender)
  {
    if (iSender.Player == null || !iSender.Player.Playing)
      return;
    int id = iSender.Player.ID;
    if (iSender.Player.Gamer == Gamer.INVALID_GAMER)
    {
      int selectedItem = this.mPlayerSlots[id].SelectedItem;
      do
      {
        ++selectedItem;
        if (selectedItem >= this.mGamerItems.Count)
          selectedItem -= this.mGamerItems.Count;
      }
      while (selectedItem < this.mGamerItems.Count - 1 && Profile.Instance.Gamers.Values[selectedItem].InUse);
      while (this.mGamerScrollBars[id].Value < this.mGamerScrollBars[id].MaxValue && this.mGamerScrollBars[id].Value < selectedItem - 2)
        ++this.mGamerScrollBars[id].Value;
      while (this.mGamerScrollBars[id].Value > 0 && this.mGamerScrollBars[id].Value >= selectedItem)
        --this.mGamerScrollBars[id].Value;
      this.mPlayerSlots[id].SelectedItem = selectedItem;
    }
    else
    {
      if (this.mPlayerSlots[id].State == SubMenuCharacterSelect.GamerState.CustomizingAvatar)
        return;
      if (this.mPlayerSlots[id].State == SubMenuCharacterSelect.GamerState.CustomizingColor)
      {
        int num = this.mPlayerSlots[id].SelectedItem + Defines.PLAYERCOLORS.Length / 2 + Defines.PLAYERCOLORS.Length % 2;
        if (num >= Defines.PLAYERCOLORS.Length)
          num -= Defines.PLAYERCOLORS.Length;
        this.mPlayerSlots[id].SelectedItem = num;
      }
      else if (this.mGameModeBox.IsDown)
      {
        int num = this.mGameModeBox.NewSelection + 1;
        if (num >= this.mGameModeBox.Count)
          num -= this.mGameModeBox.Count;
        this.mGameModeBox.NewSelection = num;
      }
      else
      {
        for (int index = 0; index < this.mVersusSettings.MenuItems.Count; ++index)
        {
          DropDownBox menuItem = this.mVersusSettings.MenuItems[index];
          if (menuItem.IsDown)
          {
            int num = menuItem.NewSelection + 1;
            if (num >= menuItem.Count)
              num -= menuItem.Count;
            menuItem.NewSelection = num;
            return;
          }
        }
        if (this.mCurrentState != SubMenuCharacterSelect.State.Normal && this.mCurrentController != null && this.mCurrentController != iSender)
          return;
        if (this.mCurrentState == SubMenuCharacterSelect.State.ChangingLevel)
        {
          int num = this.mSelectedPack + 1;
          if (num > SubMenuCharacterSelect.mLevelRepresentations.Count - 1)
            num = 0;
          if (this.mSpecialScrollBar.Value > num)
            this.mSpecialScrollBar.Value = num;
          if (this.mSpecialScrollBar.Value + 6 <= num)
            this.mSpecialScrollBar.Value = num - 6 + 1;
          this.mSelectedPack = num;
        }
        else if (this.mCurrentState == SubMenuCharacterSelect.State.ChangingPacks)
        {
          if (this.mSelectedPack < 0)
          {
            this.mSelectedPosition = 5;
            this.mSelectedPack = 0;
            this.mSpecialScrollBar.Value = 0;
            this.ShowPackToolTip(this.mSelectedPack);
          }
          else
          {
            this.mSelectedPosition = 5;
            int length1 = PackMan.Instance.ItemPacks.Length;
            int length2 = PackMan.Instance.MagickPacks.Length;
            int iIndex = this.mSelectedPack + 5;
            if (iIndex >= length1 + length2)
            {
              if ((iIndex - length1) / 5 * 5 < length2)
              {
                this.mSpecialScrollBar.Value = this.mSpecialScrollBar.MaxValue;
                iIndex = length1 + length2 - 1;
              }
              else
                iIndex = -1;
            }
            else
            {
              if (this.mSelectedPack < length1 && iIndex >= length1)
                iIndex = iIndex / 5 * 5 >= length1 ? length1 + iIndex % 5 : length1 - 1;
              if (iIndex < length1)
              {
                while (this.mSpecialScrollBar.Value < this.mSpecialScrollBar.MaxValue && this.mSpecialScrollBar.Value + 7 <= iIndex / 5 + 1)
                  ++this.mSpecialScrollBar.Value;
              }
              else
              {
                int num = length1 / 5;
                if (length1 % 5 != 0)
                  ++num;
                while (this.mSpecialScrollBar.Value < this.mSpecialScrollBar.MaxValue && this.mSpecialScrollBar.Value + 7 <= (iIndex - length1) / 5 + 2 + num)
                  ++this.mSpecialScrollBar.Value;
              }
            }
            this.mSelectedPack = iIndex;
            if (iIndex >= 0)
              this.ShowPackToolTip(iIndex);
            else
              ToolTipMan.Instance.Kill((Magicka.GameLogic.Player) null, false);
          }
        }
        else
        {
          if (this.mSelectedPosition == 7 + this.mVersusSettings.MenuItems.Count && this.mGameSettings.GameType == GameType.Versus)
          {
            this.mSelectedPosition = 5;
          }
          else
          {
            if (iSender.Player.Gamer != Gamer.INVALID_GAMER && iSender is XInputController)
            {
              if (this.mSelectedPosition == -1)
              {
                this.mSelectedPosition = 4;
                return;
              }
              if (this.mSelectedPosition == 3)
              {
                this.mSelectedPosition = -1;
                return;
              }
            }
            this.mSelectLevelButton.Selected = false;
            switch (this.mSelectedPosition)
            {
              case -1:
                this.mSelectedPosition = 0;
                break;
              case 4:
                this.mSelectedPosition = 0;
                break;
              case 6:
                if (this.mGameSettings.GameType == GameType.Versus)
                {
                  this.mSelectedPosition = 7;
                  break;
                }
                if (this.mGameSettings.GameType == GameType.Challenge)
                {
                  this.mSelectedPosition = 5;
                  break;
                }
                break;
              case 9:
                if (this.mGameSettings.GameType != GameType.Campaign && this.mGameSettings.GameType != GameType.Mythos)
                {
                  ++this.mSelectedPosition;
                  break;
                }
                break;
              default:
                ++this.mSelectedPosition;
                if (this.mSelectedPosition == 6 && !this.HasSelectedLevel)
                {
                  this.mSelectLevelButton.Selected = true;
                  break;
                }
                break;
            }
          }
          if (this.mSelectedPosition != 5)
            return;
          this.mSelectedPack = this.mPackScrollValue;
        }
      }
    }
  }

  public override void ControllerUp(Controller iSender)
  {
    if (iSender.Player == null || !iSender.Player.Playing)
      return;
    int id = iSender.Player.ID;
    if (iSender.Player.Gamer == Gamer.INVALID_GAMER)
    {
      int selectedItem = this.mPlayerSlots[id].SelectedItem;
      do
      {
        --selectedItem;
        if (selectedItem < 0)
          selectedItem += this.mGamerItems.Count;
      }
      while (selectedItem < this.mGamerItems.Count - 1 && Profile.Instance.Gamers.Values[selectedItem].InUse);
      while (this.mGamerScrollBars[id].Value < this.mGamerScrollBars[id].MaxValue && this.mGamerScrollBars[id].Value < selectedItem - 2)
        ++this.mGamerScrollBars[id].Value;
      while (this.mGamerScrollBars[id].Value > 0 && this.mGamerScrollBars[id].Value >= selectedItem)
        --this.mGamerScrollBars[id].Value;
      this.mPlayerSlots[id].SelectedItem = selectedItem;
    }
    else
    {
      if (this.mPlayerSlots[id].State == SubMenuCharacterSelect.GamerState.CustomizingAvatar)
        return;
      if (this.mPlayerSlots[id].State == SubMenuCharacterSelect.GamerState.CustomizingColor)
      {
        int num = this.mPlayerSlots[id].SelectedItem - Defines.PLAYERCOLORS.Length / 2 - Defines.PLAYERCOLORS.Length % 2;
        if (num < 0)
          num += Defines.PLAYERCOLORS.Length;
        this.mPlayerSlots[id].SelectedItem = num;
      }
      else if (this.mGameModeBox.IsDown)
      {
        int num = this.mGameModeBox.NewSelection - 1;
        if (num < 0)
          num += this.mGameModeBox.Count;
        this.mGameModeBox.NewSelection = num;
      }
      else
      {
        for (int index = 0; index < this.mVersusSettings.MenuItems.Count; ++index)
        {
          DropDownBox menuItem = this.mVersusSettings.MenuItems[index];
          if (menuItem.IsDown)
          {
            int num = menuItem.NewSelection - 1;
            if (num < 0)
              num += menuItem.Count;
            menuItem.NewSelection = num;
            return;
          }
        }
        if (this.mCurrentState != SubMenuCharacterSelect.State.Normal && this.mCurrentController != null && this.mCurrentController != iSender)
          return;
        if (this.mCurrentState == SubMenuCharacterSelect.State.ChangingLevel)
        {
          int num = this.mSelectedPack != 0 ? this.mSelectedPack - 1 : SubMenuCharacterSelect.mLevelRepresentations.Count - 1;
          if (num < 0)
            num += SubMenuCharacterSelect.mLevelRepresentations.Count - 1;
          if (this.mSpecialScrollBar.Value > num)
            this.mSpecialScrollBar.Value = num;
          if (this.mSpecialScrollBar.Value + 6 <= num)
            this.mSpecialScrollBar.Value = num - 6 + 1;
          this.mSelectedPack = num;
        }
        else if (this.mCurrentState == SubMenuCharacterSelect.State.ChangingPacks)
        {
          if (this.mSelectedPack < 0)
          {
            this.mSelectedPosition = 5;
            int length1 = PackMan.Instance.ItemPacks.Length;
            int length2 = PackMan.Instance.MagickPacks.Length;
            int num = length2 / 5;
            if (length2 % 5 == 0)
              --num;
            this.mSelectedPack = length1 + num * 5;
            this.mSpecialScrollBar.Value = this.mSpecialScrollBar.MaxValue;
            this.ShowPackToolTip(this.mSelectedPack);
          }
          else
          {
            this.mSelectedPosition = 5;
            int length = PackMan.Instance.ItemPacks.Length;
            int iIndex = this.mSelectedPack - 5;
            if (iIndex < 0)
            {
              iIndex = -1;
            }
            else
            {
              if (this.mSelectedPack >= length && iIndex < length)
              {
                int num = length / 5;
                if (length % 5 == 0)
                  --num;
                iIndex = num * 5 + (this.mSelectedPack - length) % 5;
                if (iIndex >= length)
                  iIndex = length - 1;
              }
              if (iIndex < length)
              {
                while (this.mSpecialScrollBar.Value > 0 && this.mSpecialScrollBar.Value >= iIndex / 5 + 2)
                  --this.mSpecialScrollBar.Value;
              }
              else
              {
                int num = length / 5;
                if (length % 5 != 0)
                  ++num;
                while (this.mSpecialScrollBar.Value > 0 && this.mSpecialScrollBar.Value >= (iIndex - length) / 5 + 3 + num)
                  --this.mSpecialScrollBar.Value;
              }
            }
            this.mSelectedPack = iIndex;
            if (iIndex >= 0)
              this.ShowPackToolTip(iIndex);
            else
              ToolTipMan.Instance.Kill((Magicka.GameLogic.Player) null, false);
          }
        }
        else
        {
          if (iSender.Player.Gamer != Gamer.INVALID_GAMER && iSender is XInputController)
          {
            if (this.mSelectedPosition == -1)
            {
              this.mSelectedPosition = 3;
              return;
            }
            if (this.mSelectedPosition == 4)
            {
              this.mSelectedPosition = -1;
              return;
            }
          }
          this.mSelectLevelButton.Selected = false;
          switch (this.mSelectedPosition)
          {
            case 0:
              this.mSelectedPosition = 4;
              break;
            case 4:
              this.mSelectedPosition = 0;
              break;
            case 5:
              if (this.mGameSettings.GameType == GameType.Versus)
              {
                this.mSelectedPosition = 7 + this.mVersusSettings.MenuItems.Count;
                break;
              }
              if (this.mGameSettings.GameType == GameType.Challenge)
              {
                this.mSelectedPosition = 6;
                break;
              }
              break;
            case 7:
              this.mSelectedPosition = 6;
              break;
            case 9:
              if (this.mGameSettings.GameType != GameType.Campaign && this.mGameSettings.GameType != GameType.Mythos)
              {
                --this.mSelectedPosition;
                break;
              }
              break;
            default:
              --this.mSelectedPosition;
              if (this.mSelectedPosition == 6 && !this.HasSelectedLevel)
              {
                this.mSelectLevelButton.Selected = true;
                break;
              }
              break;
          }
          if (this.mSelectedPosition != 5)
            return;
          this.mSelectedPack = this.mPackScrollValue;
        }
      }
    }
  }

  public override void ControllerRight(Controller iSender)
  {
    if (iSender.Player == null || !iSender.Player.Playing)
      return;
    if (iSender.Player.Gamer != Gamer.INVALID_GAMER)
    {
      int id = iSender.Player.ID;
      if (this.mPlayerSlots[id].State == SubMenuCharacterSelect.GamerState.CustomizingAvatar)
      {
        int count = SubMenuCharacterSelect.mRobeRepresentations.Count;
        int iIndex = this.mPlayerSlots[id].SelectedItem + 1;
        if (iIndex >= count)
          iIndex -= count;
        if ((this.mPlayerSlots[id].ScrollValue + 3) % count == this.mPlayerSlots[id].SelectedItem)
          this.mPlayerSlots[id].ScrollValue = (iIndex - 3 + count) % count;
        this.mPlayerSlots[id].SelectedItem = iIndex;
        this.ShowAvatarToolTip(iSender, iIndex, new MouseState?());
        return;
      }
      if (this.mPlayerSlots[id].State == SubMenuCharacterSelect.GamerState.CustomizingColor)
      {
        int num = this.mPlayerSlots[id].SelectedItem + 1;
        if (num >= Defines.PLAYERCOLORS.Length)
          num -= Defines.PLAYERCOLORS.Length;
        this.mPlayerSlots[id].SelectedItem = num;
        return;
      }
    }
    if (this.mGameModeBox.IsDown)
      return;
    for (int index = 0; index < this.mVersusSettings.MenuItems.Count; ++index)
    {
      if (this.mVersusSettings.MenuItems[index].IsDown)
        return;
    }
    if (this.mCurrentState != SubMenuCharacterSelect.State.Normal && this.mCurrentController != null && this.mCurrentController != iSender)
      return;
    if (this.mCurrentState == SubMenuCharacterSelect.State.Normal && this.mSelectedPosition == 9)
      this.mSelectedPosition = 0;
    else if (this.mCurrentState == SubMenuCharacterSelect.State.ChangingPacks)
    {
      if (this.mSelectedPack < 0 || this.mSelectedPack >= PackMan.Instance.AllPacks.Length)
        return;
      this.mSelectedPosition = 5;
      int length1 = PackMan.Instance.ItemPacks.Length;
      int iIndex;
      if (this.mSelectedPack < length1)
      {
        iIndex = this.mSelectedPack / 5 * 5 + (this.mSelectedPack + 1) % 5;
        if (iIndex >= length1)
          iIndex = this.mSelectedPack / 5 * 5;
      }
      else
      {
        int length2 = PackMan.Instance.MagickPacks.Length;
        iIndex = length1 + (this.mSelectedPack - length1) / 5 * 5 + (this.mSelectedPack - length1 + 1) % 5;
        if (iIndex >= length1 + length2)
          iIndex = length1 + length2 / 5 * 5;
      }
      this.mSelectedPack = iIndex;
      this.ShowPackToolTip(iIndex);
    }
    else if (this.mSelectedPosition == 5)
    {
      this.ScrollPack(1);
    }
    else
    {
      if (this.mGameSettings.GameType == GameType.Campaign || this.mGameSettings.GameType == GameType.Mythos)
        return;
      switch (this.mSelectedPosition)
      {
        case 0:
        case 1:
        case 2:
        case 3:
        case 4:
          if (this.mGameSettings.GameType == GameType.Versus)
          {
            this.mSelectedPosition = 7;
            break;
          }
          if (this.mGameSettings.GameType == GameType.Campaign || this.mGameSettings.GameType == GameType.Mythos)
            break;
          this.mSelectedPosition = 5;
          break;
        default:
          this.mSelectedPosition = 0;
          break;
      }
    }
  }

  private void ShowPackToolTip(int iIndex)
  {
    IPack allPack = PackMan.Instance.AllPacks[iIndex];
    int length = PackMan.Instance.ItemPacks.Length;
    int num1;
    int num2;
    if (iIndex < length)
    {
      num1 = 1 + iIndex / 5;
      num2 = iIndex % 5;
    }
    else
    {
      num1 = 2 + length / 5 + (iIndex - length) / 5;
      if (length % 5 != 0)
        ++num1;
      num2 = (iIndex - length) % 5;
    }
    Vector2 oScreenPos;
    Tome.Instance.PageToScreen(true, ref new Vector2()
    {
      X = (float) (160.0 + 64.0 * (double) num2),
      Y = (float) (64.0 + (double) this.mSpecialScrollBar.Position.Y - (double) this.mSpecialScrollBar.Height * 0.5 + (double) Math.Max(num1 - this.mSpecialScrollBar.Value, 0) * 64.0)
    }, out oScreenPos);
    ToolTipMan.Instance.Set((Magicka.GameLogic.Player) null, $"{LanguageManager.Instance.GetStringWithReferencs(allPack.Name)}\n{LanguageManager.Instance.GetStringWithReferencs(allPack.Descritpion)}", ref oScreenPos);
  }

  public override void ControllerLeft(Controller iSender)
  {
    if (iSender.Player == null || !iSender.Player.Playing)
      return;
    if (iSender.Player.Gamer != Gamer.INVALID_GAMER)
    {
      int id = iSender.Player.ID;
      if (this.mPlayerSlots[id].State == SubMenuCharacterSelect.GamerState.CustomizingAvatar)
      {
        int iIndex = this.mPlayerSlots[id].SelectedItem - 1;
        if (iIndex < 0)
          iIndex += SubMenuCharacterSelect.mRobeRepresentations.Count;
        if (this.mPlayerSlots[id].ScrollValue == this.mPlayerSlots[id].SelectedItem)
          this.mPlayerSlots[id].ScrollValue = iIndex;
        this.mPlayerSlots[id].SelectedItem = iIndex;
        this.ShowAvatarToolTip(iSender, iIndex, new MouseState?());
        return;
      }
      if (this.mPlayerSlots[id].State == SubMenuCharacterSelect.GamerState.CustomizingColor)
      {
        int num = this.mPlayerSlots[id].SelectedItem - 1;
        if (num < 0)
          num += Defines.PLAYERCOLORS.Length;
        this.mPlayerSlots[id].SelectedItem = num;
        return;
      }
    }
    if (this.mGameModeBox.IsDown)
      return;
    for (int index = 0; index < this.mVersusSettings.MenuItems.Count; ++index)
    {
      if (this.mVersusSettings.MenuItems[index].IsDown)
        return;
    }
    if (this.mCurrentState != SubMenuCharacterSelect.State.Normal && this.mCurrentController != null && this.mCurrentController != iSender)
      return;
    if (this.mCurrentState == SubMenuCharacterSelect.State.ChangingPacks)
    {
      if (this.mSelectedPack < 0 || this.mSelectedPack >= PackMan.Instance.AllPacks.Length)
        return;
      this.mSelectedPosition = 5;
      int length1 = PackMan.Instance.ItemPacks.Length;
      int iIndex;
      if (this.mSelectedPack < length1)
      {
        iIndex = this.mSelectedPack / 5 * 5 + (this.mSelectedPack - 1 + 5) % 5;
        if (iIndex >= length1)
          iIndex = length1 - 1;
      }
      else
      {
        int length2 = PackMan.Instance.MagickPacks.Length;
        iIndex = length1 + (this.mSelectedPack - length1) / 5 * 5 + (this.mSelectedPack - length1 - 1 + 5) % 5;
        if (iIndex >= length1 + length2)
          iIndex = length1 + length2 - 1;
      }
      this.mSelectedPack = iIndex;
      this.ShowPackToolTip(iIndex);
    }
    else if (this.mSelectedPosition == 5)
    {
      this.ScrollPack(-1);
    }
    else
    {
      switch (this.mSelectedPosition)
      {
        case 0:
        case 1:
        case 2:
        case 3:
        case 4:
          if (this.mGameSettings.GameType == GameType.Versus)
          {
            this.mSelectedPosition = 7;
            break;
          }
          if (this.mGameSettings.GameType == GameType.Campaign || this.mGameSettings.GameType == GameType.Mythos)
          {
            this.mSelectedPosition = 9;
            break;
          }
          this.mSelectedPosition = 5;
          break;
        default:
          if (this.mGameSettings.GameType != GameType.Campaign && this.mGameSettings.GameType != GameType.Mythos)
            break;
          this.mSelectedPosition = 9;
          break;
      }
    }
  }

  private void ScrollPack(int iDir)
  {
    int mPackScrollValue = this.mPackScrollValue;
    IPack[] allPacks = PackMan.Instance.AllPacks;
    do
    {
      mPackScrollValue += iDir;
      if (mPackScrollValue < 0)
        mPackScrollValue += allPacks.Length;
      if (mPackScrollValue >= allPacks.Length)
        mPackScrollValue -= allPacks.Length;
    }
    while (!allPacks[mPackScrollValue].Enabled && mPackScrollValue != this.mPackScrollValue);
    this.mPackScrollValue = mPackScrollValue;
  }

  public override void ControllerMouseAction(
    Controller iSender,
    Point iScreenSize,
    MouseState iState,
    MouseState iOldState)
  {
    if ((double) this.mOptionsAlpha > 0.0 && (double) this.mOptionsAlpha < 1.0 || RenderManager.Instance.IsTransitionActive)
      return;
    if (iState.ScrollWheelValue > iOldState.ScrollWheelValue)
    {
      if (iSender.Player != null && iSender.Player.Playing && iSender.Player.Gamer == Gamer.INVALID_GAMER)
      {
        --this.mGamerScrollBars[iSender.Player.ID].Value;
        this.ControllerMouseMove(iSender, iScreenSize, iState, iOldState);
      }
      else if (this.mCurrentState == SubMenuCharacterSelect.State.ChangingLevel || this.mCurrentState == SubMenuCharacterSelect.State.ChangingPacks)
      {
        --this.mSpecialScrollBar.Value;
      }
      else
      {
        if (NetworkManager.Instance.State == NetworkState.Offline)
          return;
        --NetworkChat.Instance.ScrollBar.Value;
      }
    }
    else if (iState.ScrollWheelValue < iOldState.ScrollWheelValue)
    {
      if (iSender.Player != null && iSender.Player.Playing && iSender.Player.Gamer == Gamer.INVALID_GAMER)
      {
        ++this.mGamerScrollBars[iSender.Player.ID].Value;
        this.ControllerMouseMove(iSender, iScreenSize, iState, iOldState);
      }
      else if (this.mCurrentState == SubMenuCharacterSelect.State.ChangingLevel || this.mCurrentState == SubMenuCharacterSelect.State.ChangingPacks)
      {
        ++this.mSpecialScrollBar.Value;
      }
      else
      {
        if (NetworkManager.Instance.State == NetworkState.Offline)
          return;
        ++NetworkChat.Instance.ScrollBar.Value;
      }
    }
    else
    {
      Vector2 oHitPosition;
      bool oRightPageHit;
      if (Tome.MousePickTome(iScreenSize, iState.X, iState.Y, out oHitPosition, out oRightPageHit))
      {
        if (oRightPageHit)
        {
          if (iState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Released && iOldState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
          {
            int oIndex;
            this.HitGamer(ref oHitPosition, out oIndex);
            if (iSender.Player != null && iSender.Player.Playing && iSender.Player.ID == oIndex)
            {
              this.UpdateGamerDropDownMenu();
              this.mGamerDropDownMenu.Show(Math.Min((int) oHitPosition.X, 1024 /*0x0400*/ - (int) this.mGamerDropDownMenu.Size.X - 16 /*0x10*/), Math.Min((int) oHitPosition.Y, 1024 /*0x0400*/ - (int) this.mGamerDropDownMenu.Size.Y * this.mGamerDropDownMenu.Count - 16 /*0x10*/));
            }
            else
            {
              this.mGamerDropDownMenu.Hide();
              Magicka.GameLogic.Player[] players = Magicka.Game.Instance.Players;
              if (NetworkManager.Instance.State != NetworkState.Server || oIndex < 0 || !players[oIndex].Playing || !(players[oIndex].Gamer is NetworkGamer))
                return;
              this.mAdminDropDownMenu.Tag = (object) oIndex;
              this.mAdminDropDownMenu.Show(Math.Min((int) oHitPosition.X, 1024 /*0x0400*/ - (int) this.mAdminDropDownMenu.Size.X - 16 /*0x10*/), Math.Min((int) oHitPosition.Y, 1024 /*0x0400*/ - (int) this.mAdminDropDownMenu.Size.Y * this.mAdminDropDownMenu.Count - 16 /*0x10*/));
            }
          }
          else if (iState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released && iOldState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
          {
            if (this.mSpecialScrollBar.Grabbed)
              this.mSpecialScrollBar.Grabbed = false;
            else if (NetworkManager.Instance.State != NetworkState.Offline && NetworkChat.Instance.ScrollBar.Grabbed)
              NetworkChat.Instance.ScrollBar.Grabbed = false;
            else if (this.mSettingsScrollbar.Grabbed)
              this.mSettingsScrollbar.Grabbed = false;
            else if (this.mGamerDropDownMenu.IsVisible)
            {
              if (this.mGamerDropDownMenu.GetHitIndex(ref oHitPosition) == 0 && this.mVersusSettings.TeamsEnabled && iSender.Player != null && iSender.Player.Playing)
              {
                iSender.Player.Team = (iSender.Player.Team & Factions.TEAM_RED) == Factions.NONE ? Factions.TEAM_RED : Factions.TEAM_BLUE;
                if (NetworkManager.Instance.State != NetworkState.Offline)
                  NetworkManager.Instance.Interface.SendMessage<MenuSelectMessage>(ref new MenuSelectMessage()
                  {
                    Option = 2,
                    Param0I = iSender.Player.ID,
                    Param1I = (int) iSender.Player.Team
                  });
              }
              this.mGamerDropDownMenu.Hide();
            }
            else if (this.mAdminDropDownMenu.IsVisible)
            {
              if (this.mAdminDropDownMenu.GetHitIndex(ref oHitPosition) == 0)
                (NetworkManager.Instance.Interface as NetworkServer)?.CloseConnection((Magicka.Game.Instance.Players[(int) this.mAdminDropDownMenu.Tag].Gamer as NetworkGamer).ClientID, ConnectionClosedMessage.CReason.Kicked);
              this.mAdminDropDownMenu.Hide();
            }
            else if (this.mGameModeBox.IsDown)
            {
              this.mGameModeBox.SelectedIndex = this.mGameModeBox.GetHitIndex(ref oHitPosition);
              this.mGameModeBox.IsDown = false;
            }
            else
            {
              for (int index = 0; index < this.mVersusSettings.MenuItems.Count; ++index)
              {
                if (this.mVersusSettings.MenuItems[index].IsDown)
                {
                  this.mVersusSettings.MenuItems[index].SelectedIndex = this.mVersusSettings.MenuItems[index].GetHitIndex(ref oHitPosition);
                  this.mVersusSettings.MenuItems[index].IsDown = false;
                  return;
                }
              }
              float num1 = this.mCurrentState == SubMenuCharacterSelect.State.ChangingLevel ? 904f : 629.3333f;
              bool flag1 = !this.HasSelectedLevel && !this.mSelectLevelButton.InsideBounds(iState) && NetworkManager.Instance.State != NetworkState.Client;
              bool flag2 = this.mCurrentState != SubMenuCharacterSelect.State.Normal && (SubMenuCharacterSelect.mLevelRepresentations == null || (double) oHitPosition.X <= 64.0 || (double) oHitPosition.X >= 512.0 || (double) oHitPosition.Y <= 64.0 || (double) oHitPosition.Y >= (double) num1 || NetworkManager.Instance.State == NetworkState.Client);
              if (flag1 || flag2)
              {
                if (this.mCurrentState == SubMenuCharacterSelect.State.ChangingLevel && this.mGameSettings.GameType != GameType.Campaign && this.mGameSettings.GameType != GameType.Mythos)
                  this.SetNoLevelSelected();
                this.ChangeState(iSender, SubMenuCharacterSelect.State.Normal);
              }
              if (this.mCurrentState == SubMenuCharacterSelect.State.Normal)
              {
                if (this.mGameSettings.GameType == GameType.Versus)
                {
                  if (this.mGameModeBox.InsideBounds(ref oHitPosition))
                  {
                    this.mGameModeBox.IsDown = true;
                    return;
                  }
                  for (int index = this.mSettingsScrollbar.Value; index < Math.Min(this.mSettingsScrollbar.Value + 5, this.mVersusSettings.MenuItems.Count); ++index)
                  {
                    if (this.mVersusSettings.MenuItems[index].InsideBounds(ref oHitPosition))
                    {
                      this.mVersusSettings.MenuItems[index].IsDown = true;
                      return;
                    }
                  }
                }
                bool flag3 = this.mSelectLevelButton.InsideBounds(iState) || (double) oHitPosition.X >= 64.0 && (double) oHitPosition.X <= 512.0 && (double) oHitPosition.Y >= (double) this.mSelectLevelButton.Position.Y - 64.0 && (double) oHitPosition.Y <= (double) this.mSelectLevelButton.Position.Y + 64.0;
                if (this.mGameSettings.GameType == GameType.Mythos || this.mGameSettings.GameType == GameType.Campaign)
                {
                  if (SubMenuCharacterSelect.mLevelRepresentations != null && flag3 && (NetworkManager.Instance.State == NetworkState.Server || NetworkManager.Instance.State == NetworkState.Offline))
                  {
                    this.mSpecialScrollBar.Height = 840f;
                    this.mSpecialScrollBar.SetMaxValue(SubMenuCharacterSelect.mLevelRepresentations.Count - 6);
                    this.mSpecialScrollBar.Value = 0;
                    ToolTipMan.Instance.Kill((Magicka.GameLogic.Player) null, false);
                    this.HasSelectedLevel = true;
                    this.ChangeState(iSender, SubMenuCharacterSelect.State.ChangingLevel);
                  }
                }
                else
                {
                  if (SubMenuCharacterSelect.mLevelRepresentations != null && flag3 && (NetworkManager.Instance.State == NetworkState.Server || NetworkManager.Instance.State == NetworkState.Offline))
                  {
                    this.mSpecialScrollBar.Height = 840f;
                    this.mSpecialScrollBar.SetMaxValue(SubMenuCharacterSelect.mLevelRepresentations.Count - 6);
                    this.mSpecialScrollBar.Value = 0;
                    ToolTipMan.Instance.Kill((Magicka.GameLogic.Player) null, false);
                    this.HasSelectedLevel = true;
                    this.ChangeState(iSender, SubMenuCharacterSelect.State.ChangingLevel);
                  }
                  ControllerDirection oScrollDirection;
                  if ((this.mGameSettings.GameType == GameType.Challenge || this.mGameSettings.GameType == GameType.Versus) && this.HitPackOverview(ref oHitPosition, out int _, out oScrollDirection) && !(this.mVersusSettings is Krietor.Settings))
                  {
                    switch (oScrollDirection)
                    {
                      case ControllerDirection.Right:
                        this.ScrollPack(1);
                        break;
                      case ControllerDirection.Left:
                        this.ScrollPack(-1);
                        break;
                      default:
                        if (NetworkManager.Instance.State != NetworkState.Client)
                        {
                          int length1 = PackMan.Instance.ItemPacks.Length;
                          int length2 = PackMan.Instance.MagickPacks.Length;
                          int num2 = 2 + length1 / 5 + length2 / 5;
                          if (length1 % 5 != 0)
                            ++num2;
                          if (length2 % 5 != 0)
                            ++num2;
                          this.mSpecialScrollBar.Height = 448f;
                          this.mSpecialScrollBar.SetMaxValue(num2 - 7);
                          ToolTipMan.Instance.Kill((Magicka.GameLogic.Player) null, false);
                          this.ChangeState(iSender, SubMenuCharacterSelect.State.ChangingPacks);
                          break;
                        }
                        break;
                    }
                  }
                }
              }
              else if (this.mCurrentState == SubMenuCharacterSelect.State.ChangingLevel)
              {
                if ((double) oHitPosition.X >= 64.0 && (double) oHitPosition.X <= 448.0)
                {
                  float num3 = 64f;
                  float num4 = num3 + 140f;
                  int num5 = Math.Min(this.mSpecialScrollBar.Value + 5, SubMenuCharacterSelect.mLevelRepresentations.Count - 1);
                  for (int sortedIndex = this.mSpecialScrollBar.Value; sortedIndex <= num5; ++sortedIndex)
                  {
                    if ((double) oHitPosition.Y >= (double) num3 && (double) oHitPosition.Y <= (double) num4)
                    {
                      int levelOriginalIndex = this.GetLevelOriginalIndex(sortedIndex);
                      ToolTipMan.Instance.Kill((Magicka.GameLogic.Player) null, false);
                      this.OnLevelChange(iSender, this.mGameSettings.GameType, levelOriginalIndex);
                      this.ChangeState(iSender, SubMenuCharacterSelect.State.Normal);
                      break;
                    }
                    num3 += 140f;
                    num4 += 140f;
                  }
                }
              }
              else if (this.mCurrentState == SubMenuCharacterSelect.State.ChangingPacks)
              {
                if (this.mPackCloseButton.InsideBounds(ref oHitPosition))
                {
                  DLC_StatusHelper.TrySetAllItemsAndMagicsUsed();
                  this.ChangeState(iSender, SubMenuCharacterSelect.State.Normal);
                }
                else if (this.HitPackList(ref oHitPosition, out this.mSelectedPack))
                {
                  if (PackMan.Instance.AllPacks[this.mSelectedPack].License == HackHelper.License.No && PackMan.Instance.AllPacks[this.mSelectedPack].StoreURL != 0U)
                  {
                    SteamUtils.ActivateGameOverlayToStore(PackMan.Instance.AllPacks[this.mSelectedPack].StoreURL, OverlayStoreFlag.None);
                  }
                  else
                  {
                    IPack allPack = PackMan.Instance.AllPacks[this.mSelectedPack];
                    allPack.Enabled = !allPack.Enabled;
                  }
                }
              }
              int oIndex1;
              this.HitGamer(ref oHitPosition, out oIndex1);
              if (iSender.Player == null || !iSender.Player.Playing)
              {
                if (oIndex1 >= 0 && !Magicka.Game.Instance.Players[oIndex1].Playing)
                {
                  this.mSelectedPosition = -1;
                  this.JoinPlayer(iSender, oIndex1, (Gamer) null);
                  this.UpdateControllerIcon(iSender, oIndex1);
                }
              }
              else if (iSender.Player.Gamer == Gamer.INVALID_GAMER)
              {
                MenuScrollBar mGamerScrollBar = this.mGamerScrollBars[iSender.Player.ID];
                if (mGamerScrollBar.Grabbed)
                  mGamerScrollBar.Grabbed = false;
                else if ((double) oHitPosition.X >= 672.0 && (double) oHitPosition.X <= 960.0)
                {
                  float num6 = mGamerScrollBar.Position.Y - 62f;
                  float num7 = mGamerScrollBar.Position.Y - 31f;
                  for (int index = mGamerScrollBar.Value; index < Math.Min(mGamerScrollBar.Value + 4, this.mGamerItems.Count); ++index)
                  {
                    if ((double) oHitPosition.Y >= (double) num6 && (double) oHitPosition.Y <= (double) num7)
                    {
                      if (index >= Profile.Instance.Gamers.Count)
                      {
                        this.mNameInputController = iSender;
                        string upper = LanguageManager.Instance.GetString(SubMenuCharacterSelect.LOC_ENTER_NAME).ToUpper();
                        this.mNameInputBox.Show(new Action<string>(this.NewGamerCreated), iSender, upper);
                        break;
                      }
                      if (!Profile.Instance.Gamers.Values[index].InUse)
                      {
                        this.GamerSelected(iSender, Profile.Instance.Gamers.Values[index]);
                        Magicka.GameLogic.Player player = iSender.Player;
                        if (player.Gamer != null)
                        {
                          this.VerifyAvatar(ref player, false);
                          break;
                        }
                        break;
                      }
                      break;
                    }
                    num6 += 31f;
                    num7 += 31f;
                  }
                }
              }
              else
              {
                byte id = (byte) iSender.Player.ID;
                if (oIndex1 == (int) id)
                {
                  if (NetworkManager.Instance.State == NetworkState.Client && (this.mPlayerSlots[oIndex1].State == SubMenuCharacterSelect.GamerState.Open || this.mPlayerSlots[oIndex1].State == SubMenuCharacterSelect.GamerState.Ready) && (double) oHitPosition.X >= 912.0 && (double) oHitPosition.X <= 1040.0 && (double) oHitPosition.Y >= 89.0 + (double) id * 137.0 + 112.0 - 80.0 && (double) oHitPosition.Y <= 89.0 + (double) id * 137.0 + 112.0)
                    this.SetReady(this.mPlayerSlots[(int) id].State == SubMenuCharacterSelect.GamerState.Open, id);
                  else if (this.mPlayerSlots[oIndex1].State == SubMenuCharacterSelect.GamerState.Open)
                  {
                    this.mPlayerSlots[(int) id].ConsecutiveColorChanges = Math.Max(0, this.mPlayerSlots[(int) id].ConsecutiveColorChanges);
                    this.mPlayerSlots[(int) id].SelectedItem = -1;
                    this.mPlayerSlots[(int) id].State = !this.mDefaultAvatars ? SubMenuCharacterSelect.GamerState.CustomizingAvatar : SubMenuCharacterSelect.GamerState.CustomizingColor;
                  }
                  else if (this.mPlayerSlots[(int) id].State == SubMenuCharacterSelect.GamerState.CustomizingAvatar)
                  {
                    int oIndex2;
                    if (this.HitAvatar((int) id, ref oHitPosition, out oIndex2))
                    {
                      Profile.PlayableAvatar playableAvatar = Profile.Instance.Avatars.Values[SubMenuCharacterSelect.mRobeRepresentations[oIndex2].OriginalIndex];
                      uint appID = 0;
                      uint storePageAppID = 0;
                      if (DLC_StatusHelper.ValidateRobeLocked(playableAvatar, out appID, out storePageAppID))
                      {
                        SteamUtils.ActivateGameOverlayToWebPage($"http://store.steampowered.com/app/{(object) storePageAppID}/");
                        return;
                      }
                      this.SetRobeUsed(playableAvatar.Name);
                      this.SortRobeRepList();
                      switch (HackHelper.CheckLicense(playableAvatar))
                      {
                        case HackHelper.License.Yes:
                          if (!iSender.Player.Gamer.Avatar.Name.Equals(playableAvatar.Name))
                            this.mPlayerSlots[(int) id].ConsecutiveColorChanges = -1;
                          iSender.Player.Gamer.Avatar = playableAvatar;
                          this.mPlayerSlots[(int) id].Custom = SubMenuCharacterSelect.mRobeRepresentations[oIndex2].IsCustom;
                          if (NetworkManager.Instance.State != NetworkState.Offline)
                          {
                            GamerChangedMessage iMessage = new GamerChangedMessage(iSender.Player);
                            NetworkManager.Instance.Interface.SendMessage<GamerChangedMessage>(ref iMessage);
                          }
                          ToolTipMan.Instance.Kill(iSender.Player, false);
                          this.mPlayerSlots[(int) id].SelectedItem = -1;
                          this.mPlayerSlots[(int) id].State = SubMenuCharacterSelect.GamerState.CustomizingColor;
                          break;
                        case HackHelper.License.Custom:
                          if (NetworkManager.Instance.State == NetworkState.Offline || !NetworkManager.Instance.Interface.IsVACSecure)
                            goto case HackHelper.License.Yes;
                          break;
                      }
                    }
                    else if ((double) oHitPosition.Y >= 89.0 + 137.0 * (double) id && (double) oHitPosition.Y <= 89.0 + 137.0 * ((double) id + 1.0))
                    {
                      float num8 = 528f;
                      float num9 = num8 + 48f;
                      if ((double) oHitPosition.X >= (double) num8 && (double) oHitPosition.X <= (double) num9)
                      {
                        int num10 = this.mPlayerSlots[(int) id].ScrollValue - 1;
                        if (num10 < 0)
                          num10 += SubMenuCharacterSelect.mRobeRepresentations.Count;
                        this.mPlayerSlots[(int) id].ScrollValue = num10;
                      }
                      else
                      {
                        float num11 = num8 + 432f;
                        float num12 = num9 + 432f;
                        if ((double) oHitPosition.X >= (double) num11 && (double) oHitPosition.X <= (double) num12)
                        {
                          int num13 = this.mPlayerSlots[(int) id].ScrollValue + 1;
                          if (num13 >= SubMenuCharacterSelect.mRobeRepresentations.Count)
                            num13 -= SubMenuCharacterSelect.mRobeRepresentations.Count;
                          this.mPlayerSlots[(int) id].ScrollValue = num13;
                        }
                      }
                    }
                  }
                  else
                  {
                    int oIndex3;
                    if (this.mPlayerSlots[(int) id].State == SubMenuCharacterSelect.GamerState.CustomizingColor && this.HitColor((int) id, ref oHitPosition, out oIndex3))
                    {
                      if ((int) iSender.Player.Color != oIndex3)
                      {
                        ++this.mPlayerSlots[(int) id].ConsecutiveColorChanges;
                        if (this.mPlayerSlots[(int) id].ConsecutiveColorChanges == 2)
                          AchievementsManager.Instance.AwardAchievement((PlayState) null, "bluenoyelloooow");
                      }
                      iSender.Player.Color = (byte) oIndex3;
                      this.mPlayerSlots[(int) id].SelectedItem = -1;
                      this.mPlayerSlots[(int) id].State = SubMenuCharacterSelect.GamerState.Open;
                    }
                  }
                }
              }
              if (this.mBackButton.InsideBounds(ref oHitPosition))
              {
                this.ControllerB(iSender);
              }
              else
              {
                if (!this.mStartButton.InsideBounds(ref oHitPosition) || !this.mStartButton.Enabled || !this.CanStart())
                  return;
                switch (NetworkManager.Instance.State)
                {
                  case NetworkState.Offline:
                    this.StartLevel();
                    break;
                  case NetworkState.Server:
                    this.Start();
                    break;
                }
              }
            }
          }
          else
          {
            if (iState.LeftButton != Microsoft.Xna.Framework.Input.ButtonState.Pressed || iOldState.LeftButton != Microsoft.Xna.Framework.Input.ButtonState.Released)
              return;
            MenuScrollBar scrollBar = NetworkChat.Instance.ScrollBar;
            if ((this.mCurrentState == SubMenuCharacterSelect.State.ChangingLevel || this.mCurrentState == SubMenuCharacterSelect.State.ChangingPacks) && this.mSpecialScrollBar.InsideBounds(ref oHitPosition))
            {
              if (this.mSpecialScrollBar.InsideDragBounds(oHitPosition))
                this.mSpecialScrollBar.Grabbed = true;
              else if (this.mSpecialScrollBar.InsideDragDownBounds(oHitPosition))
              {
                ++this.mSpecialScrollBar.Value;
              }
              else
              {
                if (!this.mSpecialScrollBar.InsideUpBounds(oHitPosition))
                  return;
                --this.mSpecialScrollBar.Value;
              }
            }
            else if (NetworkManager.Instance.State != NetworkState.Offline && scrollBar.InsideBounds(ref oHitPosition))
            {
              if (scrollBar.InsideDragBounds(oHitPosition))
                scrollBar.Grabbed = true;
              else if (scrollBar.InsideDownBounds(oHitPosition))
              {
                ++scrollBar.Value;
              }
              else
              {
                if (!scrollBar.InsideUpBounds(oHitPosition))
                  return;
                --scrollBar.Value;
              }
            }
            else if (this.mSettingsScrollbar.InsideBounds(ref oHitPosition))
            {
              if (this.mSettingsScrollbar.InsideDragBounds(oHitPosition))
                this.mSettingsScrollbar.Grabbed = true;
              else if (this.mSettingsScrollbar.InsideDragDownBounds(oHitPosition))
              {
                ++this.mSettingsScrollbar.Value;
              }
              else
              {
                if (!this.mSettingsScrollbar.InsideDragUpBounds(oHitPosition))
                  return;
                --this.mSettingsScrollbar.Value;
              }
            }
            else
            {
              if (iSender.Player == null || !iSender.Player.Playing || iSender.Player.Gamer != Gamer.INVALID_GAMER)
                return;
              MenuScrollBar mGamerScrollBar = this.mGamerScrollBars[iSender.Player.ID];
              if (mGamerScrollBar.InsideDragBounds(oHitPosition))
                mGamerScrollBar.Grabbed = true;
              else if (mGamerScrollBar.InsideDownBounds(oHitPosition))
              {
                ++mGamerScrollBar.Value;
              }
              else
              {
                if (!mGamerScrollBar.InsideUpBounds(oHitPosition))
                  return;
                --mGamerScrollBar.Value;
              }
            }
          }
        }
        else
        {
          this.mGamerDropDownMenu.Hide();
          this.mGameModeBox.IsDown = false;
          for (int index = 0; index < this.mVersusSettings.MenuItems.Count; ++index)
            this.mVersusSettings.MenuItems[index].IsDown = false;
        }
      }
      else
      {
        this.mGamerDropDownMenu.Hide();
        this.mGameModeBox.IsDown = false;
        for (int index = 0; index < this.mVersusSettings.MenuItems.Count; ++index)
          this.mVersusSettings.MenuItems[index].IsDown = false;
      }
    }
  }

  private bool CanStart()
  {
    if (this.mValidatingLevels || !this.HasSelectedLevel)
      return false;
    bool flag = false;
    for (int index = 0; index < this.mPlayerSlots.Length; ++index)
    {
      if (this.mPlayerSlots[index].AvatarSelected)
      {
        flag = true;
        break;
      }
    }
    return flag;
  }

  private unsafe void StartLevel()
  {
    if (RenderManager.Instance.IsTransitionActive)
      return;
    int num1 = 0;
    int num2 = 0;
    int num3 = 0;
    Magicka.GameLogic.Player[] players = Magicka.Game.Instance.Players;
    for (int index = 0; index < players.Length; ++index)
    {
      if (players[index].Playing)
      {
        ++num1;
        if (players[index].Team == Factions.TEAM_RED)
          ++num2;
        if (players[index].Team == Factions.TEAM_BLUE)
          ++num3;
      }
    }
    if (this.mGameSettings.GameType == GameType.Versus)
    {
      if (num1 < 2 || this.mVersusSettings.TeamsEnabled && (num2 == 0 || num3 == 0))
        return;
    }
    else if (num1 < 1)
      return;
    if (NetworkManager.Instance.State == NetworkState.Server)
    {
      NetworkServer networkServer = NetworkManager.Instance.Interface as NetworkServer;
      for (int index1 = 0; index1 < networkServer.Connections; ++index1)
      {
        bool flag = true;
        SteamID steamId = networkServer.GetSteamID(index1);
        for (int index2 = 0; index2 < players.Length; ++index2)
        {
          if (players[index2].Gamer is NetworkGamer gamer && gamer.ClientID == steamId)
          {
            flag = false;
            break;
          }
        }
        if (flag)
        {
          networkServer.SendMessage<ConnectionClosedMessage>(ref new ConnectionClosedMessage()
          {
            Reason = ConnectionClosedMessage.CReason.Kicked
          }, index1);
          Thread.Sleep(100);
          networkServer.CloseConnection(index1, ConnectionClosedMessage.CReason.Kicked);
        }
      }
      MenuSelectMessage iMessage = new MenuSelectMessage();
      iMessage.Option = 1;
      iMessage.IntendedMenu = MenuSelectMessage.MenuType.CharacterSelect;
      byte* numPtr = (byte*) &iMessage.Param0I;
      byte[] combinedHash = LevelManager.Instance.GetLevel(this.mGameSettings.GameType, this.mGameSettings.Level).GetCombinedHash();
      for (int index = 0; index < combinedHash.Length; ++index)
        numPtr[index] = combinedHash[index];
      NetworkManager.Instance.Interface.SendMessage<MenuSelectMessage>(ref iMessage);
    }
    if (this.mGameSettings.GameType == GameType.Campaign | this.mGameSettings.GameType == GameType.Mythos)
    {
      if (NetworkManager.Instance.State == NetworkState.Server)
      {
        SaveSlotInfo iMessage = new SaveSlotInfo(SubMenuCampaignSelect_SaveSlotSelect.Instance.CurrentSaveData);
        NetworkManager.Instance.Interface.SendMessage<SaveSlotInfo>(ref iMessage);
      }
      if (this.mGameSettings.Level == 0 && this.mGameSettings.GameType == GameType.Campaign)
      {
        SubMenuIntro.Instance.Play = true;
        Tome.Instance.PushMenu((SubMenu) SubMenuIntro.Instance, 10);
      }
      else
      {
        SubMenuCutscene.Instance.Play = true;
        SubMenuCutscene.Instance.Level = this.mGameSettings.Level;
        Tome.Instance.PushMenu((SubMenu) SubMenuCutscene.Instance, 10);
      }
    }
    else
    {
      int num4 = 0;
      bool flag = false;
      int level = this.mGameSettings.Level;
      if (this.mGameSettings.GameType == GameType.StoryChallange)
      {
        flag = true;
        if (level < 0 || level > LevelManager.Instance.DungeonsCampaign.Length - 1)
          flag = false;
        if (flag)
        {
          LevelNode levelNode = (LevelNode) LevelManager.Instance.DungeonsCampaign[level];
          if (levelNode == null)
            flag = false;
          else if (string.IsNullOrEmpty(levelNode.FileName))
          {
            flag = false;
          }
          else
          {
            flag = false;
            for (int index = 0; index < SubMenuCharacterSelect.dungeonLevelFileNames.Length; ++index)
            {
              flag |= string.Compare(levelNode.FileName, SubMenuCharacterSelect.dungeonLevelFileNames[index]) == 0;
              num4 = (index + 1) * -1;
              if (flag)
                break;
            }
          }
        }
      }
      if (flag)
      {
        SubMenuCutscene.Instance.Play = true;
        SubMenuCutscene.Instance.Level = num4;
        Tome.Instance.PushMenu((SubMenu) SubMenuCutscene.Instance, 20);
      }
      else
      {
        RenderManager.Instance.TransitionEnd += new TransitionEnd(this.OnTransitionEnd);
        RenderManager.Instance.BeginTransition(Transitions.Fade, Color.Black, 0.5f);
      }
    }
    LevelNode level1 = this.mGameSettings.Level < 0 ? (LevelNode) null : LevelManager.Instance.GetLevel(this.mGameSettings.GameType, this.mGameSettings.Level);
    Singleton<ParadoxServices>.Instance.TelemetryEvent(new EventData[2]
    {
      TelemetryUtils.GetGameplayStartedData(this.mGameSettings.GameType.ToString(), level1.Name.Substring(level1.Name.IndexOf('#') + 1), Magicka.Game.Instance.PlayerCount),
      TelemetryUtils.GetControllerChangedData(players)
    });
  }

  private void OnLevelChange(Controller iSender, GameType iGameType, int iLevel)
  {
    if (iLevel == -1)
    {
      this.SetNoLevelSelected();
    }
    else
    {
      LevelNode level = LevelManager.Instance.GetLevel(iGameType, iLevel);
      if (level == null)
        return;
      bool flag = true;
      if (iGameType == GameType.Campaign || iGameType == GameType.Mythos)
      {
        if ((int) SubMenuCampaignSelect_SaveSlotSelect.Instance.CurrentSaveData.Level == iLevel)
          return;
        flag = this.ChooseLevel(iLevel);
      }
      else
      {
        this.SetLevelUsed(level.Name);
        uint appID = 0;
        if (DLC_StatusHelper.ValidateLevelLocked(level, out appID))
        {
          flag = false;
          SteamUtils.ActivateGameOverlayToWebPage($"http://store.steampowered.com/app/{(object) appID}/");
          this.HasSelectedLevel = false;
          this.mStartButton.Enabled = false;
        }
        if (this.mGameSettings.GameType == GameType.Challenge || this.mGameSettings.GameType == GameType.Versus)
          this.SortLevelRepList();
      }
      if (flag)
      {
        this.SetChangedLevel(iGameType, iLevel);
        this.mGameSettings.Level = iLevel;
        this.UpdateAvailableAvatars(new DlcInstalled());
        this.UpdateChapterText(level);
      }
      if (level == null || level.AllowedAvatars == null || level.AllowedAvatars.Count <= 0)
        return;
      for (int index = 0; index < this.mPlayerSlots.Length; ++index)
      {
        if (this.mPlayerSlots[index].State == SubMenuCharacterSelect.GamerState.CustomizingAvatar)
          this.mPlayerSlots[index].State = SubMenuCharacterSelect.GamerState.CustomizingColor;
      }
    }
  }

  private void UpdateChapterText(LevelNode iLevel)
  {
    int iTargetLineWidth1 = 560;
    this.mChapterName.SetText(this.mChapterName.Font.Wrap(LanguageManager.Instance.GetString(iLevel.Name.GetHashCodeCustom()), iTargetLineWidth1, true));
    int iTargetLineWidth2 = this.mGameSettings.GameType == GameType.Campaign || this.mGameSettings.GameType == GameType.Mythos ? 560 : 940;
    string iText1 = LanguageManager.Instance.GetString(iLevel.Description);
    string iText2 = this.mChapterDescription.Font.Wrap(iText1, iTargetLineWidth2, true);
    int num = this.mChapterDescription.Font.LineHeight * 4;
    int length1 = iText1.Length;
    Vector2 vector2 = this.mChapterDescription.Font.MeasureText(iText2.ToCharArray(), true);
    if ((double) vector2.Y > (double) num)
    {
      for (; (double) vector2.Y > (double) num; vector2 = this.mChapterDescription.Font.MeasureText(iText2.ToCharArray(), true))
      {
        int length2 = iText1.LastIndexOf(" ");
        if (length2 == -1)
        {
          length2 = iText1.Length;
          if (length2 > 3)
            length2 -= 3;
        }
        iText1 = iText1.Substring(0, length2).Trim() + "...";
        iText2 = this.mChapterDescription.Font.Wrap(iText1, iTargetLineWidth2, true);
      }
    }
    this.mChapterDescription.SetText(iText2);
  }

  private void SetNoLevelSelected()
  {
    this.HasSelectedLevel = false;
    this.mGameSettings.Level = -1;
    this.mGameSettings.GameName = NetworkManager.Instance.GameName;
    this.mCustomLevel = false;
    if (NetworkManager.Instance.State != NetworkState.Server)
      return;
    NetworkManager.Instance.SetGame(this.mGameSettings.GameType, SubMenuOnline.LOC_NO_LEVEL_SELECTED.ToString());
    NetworkManager.Instance.Interface.SendMessage<GameInfoMessage>(ref this.mGameSettings);
  }

  private void SetChangedLevel(GameType iGameType, int iLevel)
  {
    LevelNode level = LevelManager.Instance.GetLevel(iGameType, iLevel);
    HackHelper.License license = HackHelper.CheckLicense(level);
    if (license == HackHelper.License.No)
      return;
    this.mGameSettings.GameType = iGameType;
    this.mGameSettings.Level = iLevel;
    this.mGameSettings.GameName = NetworkManager.Instance.GameName;
    this.mCustomLevel = license == HackHelper.License.Custom;
    if (NetworkManager.Instance.State == NetworkState.Server)
    {
      string fileName = level.FileName;
      NetworkManager.Instance.SetGame(iGameType, fileName);
      NetworkManager.Instance.Interface.SendMessage<GameInfoMessage>(ref this.mGameSettings);
      this.HasSelectedLevel = true;
    }
    else if (NetworkManager.Instance.State == NetworkState.Client)
    {
      this.HasSelectedLevel = iLevel >= 0;
    }
    else
    {
      if (NetworkManager.Instance.State != NetworkState.Offline)
        return;
      this.HasSelectedLevel = true;
    }
  }

  private bool ChooseLevel(int iLevel)
  {
    SaveData currentSaveData = SubMenuCampaignSelect_SaveSlotSelect.Instance.CurrentSaveData;
    MemoryStream checkpoint = currentSaveData.Checkpoint;
    bool flag = checkpoint != null && checkpoint.Length > 0L;
    if (flag)
    {
      this.mLevelToSet = iLevel;
      this.mCheckPointRUSure.SelectedIndex = 1;
      this.mCheckPointRUSure.Show();
    }
    else
    {
      currentSaveData.Level = (byte) iLevel;
      SaveManager.Instance.SaveCampaign();
    }
    return !flag;
  }

  private void DeleteCheckpointCallback(MessageBox iSender, int iSelection)
  {
    if (iSelection != 0)
      return;
    SaveData currentSaveData = SubMenuCampaignSelect_SaveSlotSelect.Instance.CurrentSaveData;
    currentSaveData.Level = (byte) this.mLevelToSet;
    currentSaveData.Checkpoint = (MemoryStream) null;
    SaveManager.Instance.SaveCampaign();
    this.UpdateChapterText(LevelManager.Instance.GetLevel(this.mGameSettings.GameType, this.mLevelToSet));
    this.SetChangedLevel(this.mGameSettings.GameType, this.mLevelToSet);
  }

  public override void ControllerMouseMove(
    Controller iSender,
    Point iScreenSize,
    MouseState iState,
    MouseState iOldState)
  {
    int index1 = -1;
    Vector2 oHitPosition;
    bool oRightPageHit;
    if (Tome.MousePickTome(iScreenSize, iState.X, iState.Y, out oHitPosition, out oRightPageHit))
    {
      if (oRightPageHit)
      {
        if (this.mGamerDropDownMenu.IsVisible)
          this.mGamerDropDownMenu.SelectedIndex = this.mGamerDropDownMenu.GetHitIndex(ref oHitPosition);
        else if (this.mAdminDropDownMenu.IsVisible)
          this.mAdminDropDownMenu.SelectedIndex = this.mAdminDropDownMenu.GetHitIndex(ref oHitPosition);
        else if (this.mGameModeBox.IsDown)
        {
          this.mGameModeBox.NewSelection = this.mGameModeBox.GetHitIndex(ref oHitPosition);
        }
        else
        {
          for (int index2 = 0; index2 < this.mVersusSettings.MenuItems.Count; ++index2)
          {
            if (this.mVersusSettings.MenuItems[index2].IsDown)
            {
              this.mVersusSettings.MenuItems[index2].NewSelection = this.mVersusSettings.MenuItems[index2].GetHitIndex(ref oHitPosition);
              goto label_95;
            }
          }
          if (this.mCurrentState == SubMenuCharacterSelect.State.Normal)
          {
            if (this.mGameSettings.GameType == GameType.Mythos || this.mGameSettings.GameType == GameType.Campaign)
            {
              if (SubMenuCharacterSelect.mLevelRepresentations != null && (double) oHitPosition.X >= 64.0 && (double) oHitPosition.X <= 512.0 && (double) oHitPosition.Y >= 64.0 && (double) oHitPosition.Y <= 629.33331298828125 && NetworkManager.Instance.State != NetworkState.Client)
                index1 = 9;
            }
            else
            {
              if (this.mGameSettings.GameType == GameType.Versus)
              {
                if (this.mGameModeBox.InsideBounds(ref oHitPosition))
                  index1 = 7;
                for (int index3 = this.mSettingsScrollbar.Value; index3 < Math.Min(this.mSettingsScrollbar.Value + 5, this.mVersusSettings.MenuItems.Count); ++index3)
                {
                  if (this.mVersusSettings.MenuItems[index3].InsideBounds(ref oHitPosition))
                    index1 = index3 + 8;
                }
              }
              if (this.HasSelectedLevel)
              {
                if ((double) oHitPosition.X >= 64.0 && (double) oHitPosition.X <= 512.0 && (double) oHitPosition.Y >= 480.0 && (double) oHitPosition.Y <= 629.33331298828125)
                  index1 = 6;
              }
              else if (this.mSelectLevelButton.InsideBounds(iState))
                index1 = 6;
              if ((this.mGameSettings.GameType == GameType.Challenge || this.mGameSettings.GameType == GameType.Versus) && this.HitPackOverview(ref oHitPosition, out this.mSelectedPack, out this.mSelectedPackScroll) && !(this.mVersusSettings is Krietor.Settings))
                index1 = 5;
            }
          }
          if (this.mSettingsScrollbar.Grabbed)
            this.mSettingsScrollbar.ScrollTo(oHitPosition.Y);
          if (this.mCurrentState == SubMenuCharacterSelect.State.ChangingLevel)
          {
            if (this.mSpecialScrollBar.Grabbed)
              this.mSpecialScrollBar.ScrollTo(oHitPosition.Y);
            int num1 = -1;
            if ((double) oHitPosition.X >= 64.0 && (double) oHitPosition.X <= 448.0)
            {
              float num2 = 64f;
              float num3 = num2 + 140f;
              int num4 = Math.Min(this.mSpecialScrollBar.Value + 5, SubMenuCharacterSelect.mLevelRepresentations.Count);
              for (int index4 = this.mSpecialScrollBar.Value; index4 <= num4; ++index4)
              {
                if ((double) oHitPosition.Y >= (double) num2 && (double) oHitPosition.Y <= (double) num3)
                {
                  num1 = index4;
                  break;
                }
                num2 += 140f;
                num3 += 140f;
              }
            }
            this.mSelectedPack = num1;
          }
          else if (this.mCurrentState == SubMenuCharacterSelect.State.ChangingPacks)
          {
            if (this.mSpecialScrollBar.Grabbed)
            {
              this.mSpecialScrollBar.ScrollTo(oHitPosition.Y);
              this.mSelectedPack = -1;
            }
            else if (this.mPackCloseButton.InsideBounds(ref oHitPosition))
            {
              index1 = 5;
              this.mSelectedPack = -1;
            }
            else if ((this.mGameSettings.GameType == GameType.Challenge || this.mGameSettings.GameType == GameType.Versus) && this.HitPackList(ref oHitPosition, out this.mSelectedPack))
              index1 = 5;
          }
          if (NetworkManager.Instance.State != NetworkState.Offline && NetworkChat.Instance.ScrollBar.Grabbed)
            NetworkChat.Instance.ScrollBar.ScrollTo(oHitPosition.Y);
          this.mBackButton.Selected = this.mBackButton.InsideBounds(ref oHitPosition);
          if (this.mStartButton.InsideBounds(ref oHitPosition))
            index1 = 4;
          if (iSender.Player == null || !iSender.Player.Playing)
          {
            int num5 = -1;
            if ((double) oHitPosition.X >= 544.0 && (double) oHitPosition.X <= 992.0)
            {
              Magicka.GameLogic.Player[] players = Magicka.Game.Instance.Players;
              float num6 = 89f;
              float num7 = 201f;
              for (int index5 = 0; index5 < 4; ++index5)
              {
                if (this.mPlayerSlots[index5].State != SubMenuCharacterSelect.GamerState.Locked && !players[index5].Playing && (double) oHitPosition.Y >= (double) num6 && (double) oHitPosition.Y <= (double) num7)
                {
                  num5 = index5;
                  break;
                }
                num6 += 137f;
                num7 += 137f;
              }
            }
            if (num5 >= 0)
              index1 = num5;
          }
          else
          {
            int id = iSender.Player.ID;
            if (iSender.Player.Gamer != Gamer.INVALID_GAMER)
            {
              if (this.mPlayerSlots[id].State == SubMenuCharacterSelect.GamerState.Open)
              {
                float num8 = (float) (89.0 + (double) iSender.Player.ID * 144.0);
                float num9 = num8 + 112f;
                if (NetworkManager.Instance.State != NetworkState.Offline && (double) oHitPosition.X >= 912.0 && (double) oHitPosition.X <= 1040.0 && (double) oHitPosition.Y >= (double) num9 - 80.0 && (double) oHitPosition.Y <= (double) num9)
                {
                  this.mPlayerSlots[iSender.Player.ID].SelectedItem = 0;
                  index1 = iSender.Player.ID;
                }
                else if ((double) oHitPosition.X >= 544.0 && (double) oHitPosition.X <= 992.0 && (double) oHitPosition.Y >= (double) num8 && (double) oHitPosition.Y <= (double) num9)
                {
                  index1 = iSender.Player.ID;
                  this.mPlayerSlots[iSender.Player.ID].SelectedItem = -1;
                }
                else
                  this.mPlayerSlots[iSender.Player.ID].SelectedItem = -1;
              }
              else if (this.mPlayerSlots[id].State == SubMenuCharacterSelect.GamerState.Ready)
              {
                float num = (float) (89.0 + (double) iSender.Player.ID * 144.0) + 112f;
                if (NetworkManager.Instance.State != NetworkState.Offline && (double) oHitPosition.X >= 912.0 && (double) oHitPosition.X <= 1040.0 && (double) oHitPosition.Y >= (double) num - 80.0 && (double) oHitPosition.Y <= (double) num)
                {
                  this.mPlayerSlots[iSender.Player.ID].SelectedItem = 0;
                  index1 = iSender.Player.ID;
                }
                else
                  this.mPlayerSlots[id].SelectedItem = -1;
              }
              else if (this.mPlayerSlots[id].State == SubMenuCharacterSelect.GamerState.CustomizingAvatar)
              {
                int oIndex;
                this.mPlayerSlots[id].SelectedItem = !this.HitAvatar(id, ref oHitPosition, out oIndex) ? -1 : oIndex;
              }
              else if (this.mPlayerSlots[id].State == SubMenuCharacterSelect.GamerState.CustomizingColor)
              {
                int oIndex;
                this.mPlayerSlots[id].SelectedItem = !this.HitColor(id, ref oHitPosition, out oIndex) ? -1 : oIndex;
              }
              else
                this.mPlayerSlots[iSender.Player.ID].SelectedItem = -1;
            }
            else
              this.mPlayerSlots[iSender.Player.ID].SelectedItem = -1;
          }
          if (iSender.Player != null && iSender.Player.Playing && iSender.Player.Gamer == Gamer.INVALID_GAMER)
          {
            this.mSelectedPosition = -1;
            MenuScrollBar mGamerScrollBar = this.mGamerScrollBars[iSender.Player.ID];
            if (mGamerScrollBar.Grabbed)
              mGamerScrollBar.ScrollTo(oHitPosition.Y);
            else if ((double) oHitPosition.X >= 672.0 && (double) oHitPosition.X <= 960.0)
            {
              float num10 = mGamerScrollBar.Position.Y - 62f;
              float num11 = mGamerScrollBar.Position.Y - 31f;
              int num12 = -1;
              for (int index6 = mGamerScrollBar.Value; index6 < Math.Min(mGamerScrollBar.Value + 4, this.mGamerItems.Count); ++index6)
              {
                if ((double) oHitPosition.Y >= (double) num10 && (double) oHitPosition.Y <= (double) num11)
                {
                  num12 = index6;
                  break;
                }
                num10 += 31f;
                num11 += 31f;
              }
              this.mPlayerSlots[iSender.Player.ID].SelectedItem = num12;
            }
          }
        }
      }
      else
      {
        this.mSelectedPack = -1;
        if (iSender.Player != null && iSender.Player.Playing)
          this.mPlayerSlots[iSender.Player.ID].SelectedItem = -1;
      }
    }
    else
    {
      this.mSelectedPack = -1;
      if (iSender.Player != null && iSender.Player.Playing)
        this.mPlayerSlots[iSender.Player.ID].SelectedItem = -1;
    }
label_95:
    if (this.mCurrentState != SubMenuCharacterSelect.State.Normal && this.mCurrentController != null && this.mCurrentController != iSender)
      return;
    bool flag = false;
    if (iSender.Player != null && iSender.Player.Playing)
    {
      int id = iSender.Player.ID;
      if (this.mPlayerSlots[id].State == SubMenuCharacterSelect.GamerState.CustomizingAvatar)
      {
        flag = true;
        this.ShowAvatarToolTip(iSender, this.mPlayerSlots[id].SelectedItem, new MouseState?(iState));
      }
      else if (this.mPlayerSlots[id].State == SubMenuCharacterSelect.GamerState.CustomizingColor)
      {
        flag = true;
        ToolTipMan.Instance.Kill(iSender.Player, false);
      }
    }
    if (!flag && this.mCurrentState == SubMenuCharacterSelect.State.ChangingLevel && this.mSelectedPack >= 0 && this.mSelectedPack < SubMenuCharacterSelect.mLevelRepresentations.Count)
    {
      LevelNode level = LevelManager.Instance.GetLevel(this.mGameSettings.GameType, SubMenuCharacterSelect.mLevelRepresentations[this.mSelectedPack].HashSum);
      if (level != null)
      {
        flag = true;
        ToolTipMan.Instance.Set((Magicka.GameLogic.Player) null, LanguageManager.Instance.GetString(level.Description), iState);
      }
    }
    if (!flag && this.mGameSettings.GameType == GameType.Versus)
    {
      int index7 = index1 - 8;
      if (index7 >= 0 && index7 < this.mVersusSettings.ToolTips.Count)
      {
        flag = true;
        ToolTipMan.Instance.Set((Magicka.GameLogic.Player) null, LanguageManager.Instance.GetString(this.mVersusSettings.ToolTips[index7]), iState);
      }
    }
    if (!flag)
    {
      switch (index1)
      {
        case 0:
        case 1:
        case 2:
        case 3:
          if (iSender.Player == null || !iSender.Player.Playing)
          {
            if (!Magicka.Game.Instance.Players[index1].Playing)
            {
              ToolTipMan.Instance.Set((Magicka.GameLogic.Player) null, LanguageManager.Instance.GetString(SubMenuCharacterSelect.LOC_JOIN), iState);
              break;
            }
            break;
          }
          if (index1 == iSender.Player.ID)
          {
            ToolTipMan.Instance.Set(iSender.Player, LanguageManager.Instance.GetString(SubMenuCharacterSelect.LOC_TT_CUSTOMIZE), iState);
            break;
          }
          break;
        case 5:
          IPack[] allPacks = PackMan.Instance.AllPacks;
          if (this.mSelectedPack >= 0 && this.mSelectedPack < allPacks.Length)
          {
            ToolTipMan.Instance.Set((Magicka.GameLogic.Player) null, $"{LanguageManager.Instance.GetString(allPacks[this.mSelectedPack].Name)}\n{LanguageManager.Instance.GetStringWithReferencs(allPacks[this.mSelectedPack].Descritpion)}", iState);
            break;
          }
          ToolTipMan.Instance.Kill((Magicka.GameLogic.Player) null, false);
          break;
        case 6:
          if (NetworkManager.Instance.State == NetworkState.Client && this.HasSelectedLevel)
          {
            LevelNode levelNode = (LevelNode) null;
            switch (this.mGameSettings.GameType)
            {
              case GameType.Campaign:
                levelNode = (LevelNode) LevelManager.Instance.VanillaCampaign[this.mGameSettings.Level];
                break;
              case GameType.Challenge:
                levelNode = LevelManager.Instance.Challenges[this.mGameSettings.Level];
                break;
              case GameType.Versus:
                levelNode = LevelManager.Instance.Versus[this.mGameSettings.Level];
                break;
              case GameType.Mythos:
                levelNode = (LevelNode) LevelManager.Instance.MythosCampaign[this.mGameSettings.Level];
                break;
              case GameType.StoryChallange:
                levelNode = LevelManager.Instance.StoryChallanges[this.mGameSettings.Level];
                break;
            }
            if (levelNode != null)
            {
              ToolTipMan.Instance.Set((Magicka.GameLogic.Player) null, LanguageManager.Instance.GetString(levelNode.Name.GetHashCodeCustom()), iState);
              break;
            }
            break;
          }
          ToolTipMan.Instance.Set((Magicka.GameLogic.Player) null, LanguageManager.Instance.GetString(SubMenuCharacterSelect.LOC_TT_CHANGE_LEVEL), iState);
          break;
        case 7:
          ToolTipMan.Instance.Set((Magicka.GameLogic.Player) null, LanguageManager.Instance.GetString(SubMenuCharacterSelect.LOC_TT_GAME_MODE), iState);
          break;
        case 9:
          ToolTipMan.Instance.Set((Magicka.GameLogic.Player) null, LanguageManager.Instance.GetString(SubMenu.LOC_CHANGE_CHAPTER), iState);
          break;
        default:
          ToolTipMan.Instance.Kill((Magicka.GameLogic.Player) null, false);
          ToolTipMan.Instance.Kill(iSender.Player, false);
          break;
      }
    }
    this.mSelectedPosition = index1;
  }

  private void ShowAvatarToolTip(Controller iSender, int iIndex, MouseState? iMouseState)
  {
    IList<Profile.PlayableAvatar> values = Profile.Instance.Avatars.Values;
    if (iIndex >= 0 && iIndex < SubMenuCharacterSelect.mRobeRepresentations.Count)
    {
      int index = iIndex;
      string iString = $"{LanguageManager.Instance.GetStringWithReferencs(SubMenuCharacterSelect.mRobeRepresentations[index].DisplayName)}\n{LanguageManager.Instance.GetStringWithReferencs(SubMenuCharacterSelect.mRobeRepresentations[index].Description)}";
      if (iMouseState.HasValue)
      {
        ToolTipMan.Instance.Set(iSender.Player, iString, iMouseState.Value);
      }
      else
      {
        Vector2 iPagePos = new Vector2()
        {
          X = 544f,
          Y = (float) (89.0 + (double) iSender.Player.ID * 137.0)
        };
        iPagePos.X = (float) ((double) iPagePos.X + 224.0 - 144.0);
        iPagePos.Y += 56f;
        int num = 0;
        if (SubMenuCharacterSelect.mRobeRepresentations.Count > 4)
          num = this.mPlayerSlots[iSender.Player.ID].ScrollValue;
        iPagePos.X += 96f * (float) ((iIndex - num + values.Count) % values.Count);
        iPagePos.Y += 56f;
        Vector2 oScreenPos;
        Tome.Instance.PageToScreen(true, ref iPagePos, out oScreenPos);
        ToolTipMan.Instance.Set(iSender.Player, iString, ref oScreenPos);
      }
    }
    else
      ToolTipMan.Instance.Kill(iSender.Player, false);
  }

  private void GamerSelected(Controller iController, Gamer iGamer)
  {
    if (DaisyWheel.IsDisplaying)
      return;
    this.UpdateGamers();
    if (iGamer == null)
    {
      this.mPlayerSlots[iController.Player.ID].AvatarSelected = false;
      this.mPlayerSlots[iController.Player.ID].ControllerType = -1;
      ToolTipMan.Instance.Kill(iController.Player, false);
      iController.Player.Leave();
      this.UpdateAvailableAvatars(new DlcInstalled());
    }
    else
    {
      switch (HackHelper.CheckLicense(iGamer.Avatar))
      {
        case HackHelper.License.Yes:
          this.mPlayerSlots[iController.Player.ID].Custom = !(iGamer is NetworkGamer) && HackHelper.CheckLicense(iGamer.Avatar) != HackHelper.License.Yes;
          this.mPlayerSlots[iController.Player.ID].Name.SetText(iGamer.GamerTag);
          iController.Player.Gamer = iGamer;
          this.mPlayerSlots[iController.Player.ID].AvatarSelected = true;
          if (!(iGamer is NetworkGamer))
          {
            if (!Profile.Instance.LastGamer.InUse)
            {
              Profile.Instance.LastGamer = iController.Player.Gamer;
              Profile.Instance.Write();
            }
            if (this.mGameSettings.GameType == GameType.Versus && this.mVersusSettings.TeamsEnabled && iController.Player.Team == Factions.NONE)
            {
              int num1 = 0;
              int num2 = 0;
              Magicka.GameLogic.Player[] players = Magicka.Game.Instance.Players;
              for (int index = 0; index < players.Length; ++index)
              {
                if (players[index].Playing)
                {
                  if ((players[index].Team & Factions.TEAM_RED) != Factions.NONE)
                    ++num1;
                  if ((players[index].Team & Factions.TEAM_BLUE) != Factions.NONE)
                    ++num2;
                }
              }
              iController.Player.Team = num1 > num2 ? Factions.TEAM_BLUE : Factions.TEAM_RED;
              if (NetworkManager.Instance.State != NetworkState.Offline)
                NetworkManager.Instance.Interface.SendMessage<MenuSelectMessage>(ref new MenuSelectMessage()
                {
                  Option = 2,
                  Param0I = iController.Player.ID,
                  Param1I = (int) iController.Player.Team
                });
            }
          }
          if (NetworkManager.Instance.State != NetworkState.Offline)
            NetworkManager.Instance.Interface.SendMessage<GamerChangedMessage>(ref new GamerChangedMessage(iController.Player)
            {
              UnlockedMagicks = iController.Player.UnlockedMagicks
            });
          LevelNode levelNode = (LevelNode) null;
          if (this.mGameSettings.Level > -1)
            levelNode = LevelManager.Instance.GetLevel(this.mGameSettings.GameType, this.mGameSettings.Level);
          if (levelNode != null && levelNode.AllowedAvatars.Count > 0)
          {
            this.DefaultAvatar(iController);
            break;
          }
          Magicka.GameLogic.Player player = iController.Player;
          this.VerifyAvatar(ref player);
          break;
        case HackHelper.License.Custom:
          if (this.AllowCustom)
            goto case HackHelper.License.Yes;
          goto default;
        default:
          iGamer.Avatar = Profile.Instance.DefaultAvatar;
          goto case HackHelper.License.Yes;
      }
    }
    if (iGamer is NetworkGamer)
      return;
    this.SetReady(false);
  }

  public void SetVsSettings(
    ref VersusRuleset.Settings.OptionsMessage iVersusSettings)
  {
    VersusRuleset.Settings mVersusSettings = this.mVersusSettings;
    VersusRuleset.Settings.ApplyMessage(ref mVersusSettings, ref iVersusSettings);
    if (mVersusSettings == this.mVersusSettings)
      return;
    this.mVersusSettings.Changed -= new VersusRuleset.Settings.SettingChanged(this.mVersusSettings_Changed);
    this.mVersusSettings = mVersusSettings;
    this.mVersusSettings.Changed += new VersusRuleset.Settings.SettingChanged(this.mVersusSettings_Changed);
    LanguageManager instance = LanguageManager.Instance;
    this.mVersusSettingsTitles = new Text[this.mVersusSettings.MenuTitles.Count];
    for (int index = 0; index < this.mVersusSettings.MenuTitles.Count; ++index)
    {
      this.mVersusSettingsTitles[index] = new Text(48 /*0x30*/, this.mVersusSettings.MenuItems[index].Font, TextAlign.Left, false);
      this.mVersusSettingsTitles[index].SetText(instance.GetString(this.mVersusSettings.MenuTitles[index]));
    }
    this.mGameModeBox.ValueChanged -= new Action<DropDownBox, Rulesets>(this.mGameModeBox_ValueChanged);
    int index1;
    for (index1 = 0; index1 < this.mGameModeBox.Values.Length; ++index1)
    {
      if (this.mGameModeBox.Values[index1] == iVersusSettings.Ruleset)
      {
        this.mGameModeBox.SelectedIndex = index1;
        break;
      }
    }
    if (index1 == this.mGameModeBox.Values.Length)
    {
      Console.WriteLine("Invalid settings!");
      this.mGameModeBox.SelectedIndex = 0;
    }
    this.mGameModeBox.ValueChanged += new Action<DropDownBox, Rulesets>(this.mGameModeBox_ValueChanged);
  }

  public void SetSettings(GameType iGameType, string tryLevelName, bool iCustomLevel)
  {
    this.mLevelNameToFocusWhenLevelComplete = tryLevelName;
    this.SetSettings(iGameType, -1, iCustomLevel);
  }

  public void SetSettings(GameType iGameType, int iLevel, bool iCustomLevel)
  {
    GameType gameType = this.mGameSettings.GameType;
    int num = this.mGameSettings.Level;
    if (gameType != iGameType)
      num = 0;
    if (iLevel == -1)
    {
      this.HasSelectedLevel = false;
      this.mStartButton.Enabled = false;
      this.mGameSettings.Level = num;
      iCustomLevel = false;
    }
    else
    {
      this.HasSelectedLevel = true;
      this.mGameSettings.Level = iLevel;
    }
    if (gameType != iGameType || SubMenuCharacterSelect.mLevelRepresentations == null || gameType == iGameType && !string.IsNullOrEmpty(this.mLevelNameToFocusWhenLevelComplete))
    {
      if (SubMenuCharacterSelect.mLevelRepresentations != null)
        SubMenuCharacterSelect.mLevelRepresentations.Clear();
      this.mSelectLevelButton.Enabled = this.mSelectLevelButton.Selected = false;
      this.mSelectLevelButton.SetText(LanguageManager.Instance.GetString(SubMenuCharacterSelect.LOC_LOADING));
      this.HasSelectedLevel = true;
      this.mGameSettings.Level = iLevel;
      this.mLoadingLevels = true;
      Magicka.Game.Instance.AddLoadTask(new Action(this.LoadLevelPreviews));
      Thread.Sleep(0);
    }
    this.mGameSettings.GameType = iGameType;
    this.mCustomLevel = iCustomLevel;
    this.UpdateAvailableAvatars(new DlcInstalled());
    this.mGameSettings.Level = iLevel;
    LevelNode level = iLevel < 0 ? (LevelNode) null : LevelManager.Instance.GetLevel(iGameType, iLevel);
    if (level != null)
    {
      this.UpdateChapterText(level);
    }
    else
    {
      this.mChapterName.SetText((string) null);
      this.mChapterDescription.SetText((string) null);
    }
    if (NetworkManager.Instance.State == NetworkState.Server)
      NetworkManager.Instance.Interface.SendMessage<GameInfoMessage>(ref new GameInfoMessage()
      {
        NrOfPlayers = (byte) Magicka.Game.Instance.PlayerCount,
        GameName = NetworkManager.Instance.GameName,
        GameType = iGameType,
        Level = iLevel
      });
    else if (NetworkManager.Instance.State == NetworkState.Client)
    {
      if (num != iLevel && level != null && level.AllowedAvatars != null && level.AllowedAvatars.Count > 0)
      {
        for (int index = 0; index < this.mPlayerSlots.Length; ++index)
        {
          if (this.mPlayerSlots[index].State == SubMenuCharacterSelect.GamerState.CustomizingAvatar)
            this.mPlayerSlots[index].State = SubMenuCharacterSelect.GamerState.CustomizingColor;
        }
      }
      GamerReadyMessage iMessage;
      iMessage.Id = (byte) 0;
      iMessage.Ready = false;
      NetworkManager.Instance.Interface.SendMessage<GamerReadyMessage>(ref iMessage);
    }
    this.SetReady(false);
    if (iLevel < -1)
    {
      ErrorMessageBox.Instance.Show(LanguageManager.Instance.GetString(SubMenuOnline.LOC_ERROR_MISMATCH));
      if (NetworkManager.Instance.State != NetworkState.Offline)
        NetworkManager.Instance.EndSession();
      if (!(Tome.Instance.CurrentMenu is SubMenuCharacterSelect))
        return;
      Tome.Instance.PopMenu();
    }
    else
      this.ResetLevelTexts();
  }

  public void GetSettings(
    out GameType oGameType,
    out int oLevel,
    out VersusRuleset.Settings.OptionsMessage oVersusSettings)
  {
    this.mVersusSettings.GetMessage(out oVersusSettings);
    oGameType = this.mGameSettings.GameType;
    oLevel = this.mGameSettings.Level;
  }

  public GameType GameType => this.mGameSettings.GameType;

  public void JoinPlayer(Controller iSender, int iIndex, Gamer iGamer)
  {
    if (iGamer == null)
      iGamer = Gamer.INVALID_GAMER;
    Magicka.GameLogic.Player player = Magicka.GameLogic.Player.Join(iSender, iIndex, iGamer);
    if (player == null)
      return;
    this.mPlayerSlots[player.ID].State = SubMenuCharacterSelect.GamerState.Open;
    this.mPlayerSlots[player.ID].Name.SetText(player.GamerTag);
  }

  public void SetPlayerActive(Controller iSender)
  {
    if (iSender.Player == null || iSender.Player.Gamer == null)
    {
      if (!Profile.Instance.LastGamer.InUse)
      {
        Magicka.GameLogic.Player.Join(iSender, -1, Profile.Instance.LastGamer);
        if (iSender.Player != null && iSender.Player.Gamer != null)
          this.UpdateGamer(iSender.Player, iSender.Player.Gamer);
        Magicka.GameLogic.Player player = iSender.Player;
        this.VerifyAvatar(ref player);
      }
      else
        Magicka.GameLogic.Player.Join(iSender, -1, Gamer.INVALID_GAMER);
    }
    else
    {
      Magicka.GameLogic.Player.Join(iSender, iSender.Player.ID, iSender.Player.Gamer);
      Magicka.GameLogic.Player player = iSender.Player;
      this.VerifyAvatar(ref player);
    }
  }

  private bool VerifyAvatar(ref Magicka.GameLogic.Player p) => this.VerifyAvatar(ref p, true);

  private bool VerifyAvatar(ref Magicka.GameLogic.Player p, bool ignoreIfClient)
  {
    if (ignoreIfClient && NetworkManager.Instance.State == NetworkState.Client)
      return true;
    int index1;
    try
    {
      index1 = SubMenuCharacterSelect.mRobeRepresentations == null ? 0 : SubMenuCharacterSelect.mRobeRepresentations[0].OriginalIndex;
    }
    catch
    {
      index1 = 0;
    }
    if (index1 == -1)
      index1 = 0;
    switch (this.mGameSettings.GameType)
    {
      case GameType.Campaign:
      case GameType.Mythos:
        if (p.Gamer.Avatar.AllowCampaign)
          return true;
        p.Gamer.Avatar = Profile.Instance.Avatars.Values[index1];
        break;
      case GameType.Challenge:
      case GameType.StoryChallange:
        if (p.Gamer.Avatar.AllowChallenge)
          return true;
        p.Gamer.Avatar = Profile.Instance.Avatars.Values[index1];
        if (string.Compare(p.Gamer.Avatar.Name, "wizardalu") == 0)
        {
          Magicka.GameLogic.Player[] players = Magicka.Game.Instance.Players;
          for (int index2 = 0; index2 < players.Length; ++index2)
          {
            if (players[index2] != null && players[index2].Gamer != null)
            {
              Profile.PlayableAvatar avatar = players[index2].Gamer.Avatar;
              if (players[index2] != p && (string.Compare(players[index2].Gamer.Avatar.Name, "wizardalu") == 0 || string.Compare(players[index2].Gamer.Avatar.TypeName, "wizard_alucart") == 0))
              {
                p.Gamer.Avatar = Profile.Instance.Avatars.Values[SubMenuCharacterSelect.mRobeRepresentations[1].OriginalIndex];
                break;
              }
            }
          }
          break;
        }
        break;
      case GameType.Versus:
        if (p.Gamer.Avatar.AllowPVP)
          return true;
        p.Gamer.Avatar = Profile.Instance.Avatars.Values[index1];
        break;
    }
    if (NetworkManager.Instance.State != NetworkState.Offline)
    {
      GamerChangedMessage iMessage = new GamerChangedMessage(p);
      NetworkManager.Instance.Interface.SendMessage<GamerChangedMessage>(ref iMessage);
    }
    return false;
  }

  private void Start()
  {
    if (this.mValidatingLevels)
      return;
    int num1 = 0;
    bool flag = true;
    int num2 = 0;
    int num3 = 0;
    Magicka.GameLogic.Player[] players = Magicka.Game.Instance.Players;
    for (int index = 0; index < players.Length; ++index)
    {
      if (players[index].Playing)
      {
        ++num1;
        if (players[index].Gamer is NetworkGamer && this.mPlayerSlots[index].State != SubMenuCharacterSelect.GamerState.Ready)
          flag = false;
        if (players[index].Team == Factions.TEAM_RED)
          ++num2;
        if (players[index].Team == Factions.TEAM_BLUE)
          ++num3;
      }
    }
    if (this.mGameSettings.GameType == GameType.Versus)
    {
      if (num1 < 2 || this.mVersusSettings.TeamsEnabled && (num2 == 0 || num3 == 0))
        return;
    }
    else if (num1 < 1)
      return;
    this.Start(flag ? 3 : 10);
  }

  private void Start(int iCountDown)
  {
    if (this.mValidatingLevels)
      return;
    if (NetworkManager.Instance.State == NetworkState.Server)
      NetworkManager.Instance.Interface.SendMessage<MenuSelectMessage>(ref new MenuSelectMessage()
      {
        IntendedMenu = MenuSelectMessage.MenuType.CharacterSelect,
        Option = 4,
        Param0I = iCountDown
      });
    NetworkChat.Instance.AddMessage(LanguageManager.Instance.GetString(SubMenuCharacterSelect.LOC_COUNTDOWN).Replace("#1;", iCountDown.ToString()));
    this.mCountDown = (float) iCountDown;
    this.mLastCountDownNr = iCountDown;
  }

  internal void SetReady(bool iReady)
  {
    Magicka.GameLogic.Player[] players = Magicka.Game.Instance.Players;
    for (byte iID = 0; (int) iID < players.Length; ++iID)
    {
      if (!(players[(int) iID].Gamer is NetworkGamer))
        this.SetReady(iReady, iID);
    }
  }

  internal void SetReady(bool iReady, byte iID)
  {
    if (NetworkManager.Instance.State == NetworkState.Offline)
    {
      if (this.mPlayerSlots[(int) iID].State != SubMenuCharacterSelect.GamerState.Ready)
        return;
      this.mPlayerSlots[(int) iID].State = SubMenuCharacterSelect.GamerState.Open;
    }
    else
    {
      if (iReady)
      {
        this.mPlayerSlots[(int) iID].State = SubMenuCharacterSelect.GamerState.Ready;
      }
      else
      {
        if (this.mLastCountDownNr >= 0)
        {
          NetworkChat.Instance.AddMessage(LanguageManager.Instance.GetString(SubMenuCharacterSelect.LOC_COUNTDOWN_ABORTED));
          this.mLastCountDownNr = -1;
          this.mCountDown = 0.0f;
        }
        if (this.mPlayerSlots[(int) iID].State == SubMenuCharacterSelect.GamerState.Ready)
          this.mPlayerSlots[(int) iID].State = SubMenuCharacterSelect.GamerState.Open;
      }
      if (NetworkManager.Instance.State == NetworkState.Offline || !Magicka.Game.Instance.Players[(int) iID].Playing || Magicka.Game.Instance.Players[(int) iID].Gamer is NetworkGamer)
        return;
      NetworkManager.Instance.Interface.SendMessage<GamerReadyMessage>(ref new GamerReadyMessage()
      {
        Id = iID,
        Ready = iReady
      });
    }
  }

  internal bool GetReady(int iID)
  {
    return this.mPlayerSlots[iID].State == SubMenuCharacterSelect.GamerState.Ready;
  }

  internal override void NetworkInput(ref MenuSelectMessage iMessage)
  {
    if (iMessage.Option == 0)
      this.mVersusSettings.MenuItems[iMessage.Param0I].SelectedIndex = iMessage.Param1I;
    else if (iMessage.Option == 1)
      this.StartLevel();
    else if (iMessage.Option == 2)
    {
      Magicka.Game.Instance.Players[iMessage.Param0I].Team = (Factions) iMessage.Param1I;
    }
    else
    {
      if (iMessage.Option != 4)
        throw new InvalidOperationException();
      this.Start(iMessage.Param0I);
    }
  }

  private void OnTransitionEnd(TransitionEffect iDeadTransition)
  {
    RenderManager.Instance.TransitionEnd -= new TransitionEnd(this.OnTransitionEnd);
    SpawnPoint? iSpawnPoint = new SpawnPoint?();
    LevelNode iLevel;
    switch (this.mGameSettings.GameType)
    {
      case GameType.Campaign:
        iLevel = (LevelNode) LevelManager.Instance.VanillaCampaign[this.mGameSettings.Level];
        iSpawnPoint = (iLevel as CampaignNode).SpawnPoint;
        break;
      case GameType.Challenge:
        iLevel = LevelManager.Instance.Challenges[this.mGameSettings.Level];
        break;
      case GameType.Versus:
        iLevel = LevelManager.Instance.Versus[this.mGameSettings.Level];
        break;
      case GameType.Mythos:
        iLevel = (LevelNode) LevelManager.Instance.MythosCampaign[this.mGameSettings.Level];
        iSpawnPoint = (iLevel as CampaignNode).SpawnPoint;
        break;
      case GameType.StoryChallange:
        iLevel = LevelManager.Instance.StoryChallanges[this.mGameSettings.Level];
        break;
      default:
        throw new Exception("Invalid Game Type");
    }
    int state = (int) NetworkManager.Instance.State;
    SaveData iSaveSlot = (SaveData) null;
    if (NetworkManager.Instance.State != NetworkState.Client && this.mGameSettings.GameType == GameType.Campaign | this.mGameSettings.GameType == GameType.Mythos)
      iSaveSlot = SubMenuCampaignSelect_SaveSlotSelect.Instance.CurrentSaveData;
    bool iCustom = HackHelper.LicenseStatus == HackHelper.Status.Hacked || HackHelper.CheckLicense(iLevel) != HackHelper.License.Yes || this.mGameSettings.GameType == GameType.Campaign && LevelManager.Instance.CampaignIsHacked == HackHelper.Status.Hacked;
    Magicka.GameLogic.Player[] players = Magicka.Game.Instance.Players;
    for (int index = 0; index < players.Length; ++index)
    {
      if (players[index].Playing && HackHelper.CheckLicense(players[index].Gamer.Avatar) != HackHelper.License.Yes)
      {
        iCustom = true;
        break;
      }
    }
    Magicka.Game.Instance.AddLoadTask((Action) (() => SaveManager.Instance.SaveSettings()));
    GameStateManager.Instance.PushState((GameState) new PlayState(iCustom, iLevel.FullFileName, this.mGameSettings.GameType, iSpawnPoint, iSaveSlot, this.mVersusSettings));
  }

  internal void ValidateLevels()
  {
    this.mValidatingLevels = true;
    Magicka.Game.Instance.AddLoadTask(new Action(this.LevelValidation));
  }

  private void LevelValidation()
  {
    LevelNode[] levelNodeArray;
    switch (this.mGameSettings.GameType)
    {
      case GameType.Challenge:
        levelNodeArray = LevelManager.Instance.Challenges;
        break;
      case GameType.Versus:
        levelNodeArray = LevelManager.Instance.Versus;
        break;
      case GameType.StoryChallange:
        levelNodeArray = LevelManager.Instance.StoryChallanges;
        break;
      default:
        this.mValidatingLevels = false;
        this.mStartButton.Enabled = true;
        return;
    }
    int index = this.mGameSettings.Level == -1 ? 0 : this.mGameSettings.Level;
    int num = 0;
    while (num < levelNodeArray.Length)
    {
      HackHelper.License license;
      do
      {
        license = HackHelper.CheckLicense(levelNodeArray[index]);
        Thread.Sleep(1);
      }
      while (license == HackHelper.License.Pending);
      if (license == HackHelper.License.Yes || license == HackHelper.License.Custom && this.AllowCustom)
      {
        this.mGameSettings.Level = index;
        break;
      }
      ++num;
      index = (index + 1) % levelNodeArray.Length;
    }
    this.mValidatingLevels = false;
  }

  public override void OnEnter()
  {
    this.mLastCountDownNr = -1;
    this.ChangeState((Controller) null, SubMenuCharacterSelect.State.Normal);
    ToolTipMan.Instance.KillAll(false);
    this.mOptionsAlpha = 1f;
    this.mLevelSelectAlpha = 0.0f;
    this.mPackSelectAlpha = 0.0f;
    Magicka.GameLogic.Player[] players = Magicka.Game.Instance.Players;
    NetworkState state = NetworkManager.Instance.State;
    int num1 = 0;
    int num2 = 0;
    SteamAPI.GameOverlayActivated -= new Action<GameOverlayActivated>(this.SteamOverlayActivated);
    SteamAPI.GameOverlayActivated += new Action<GameOverlayActivated>(this.SteamOverlayActivated);
    for (int slotIndex = 0; slotIndex < players.Length; ++slotIndex)
    {
      this.mPlayerSlots[slotIndex].ConsecutiveColorChanges = 0;
      if (players[slotIndex].Playing && players[slotIndex].Gamer != null)
      {
        if (state != NetworkState.Client)
        {
          this.mPlayerSlots[slotIndex].AvatarSelected = true;
          this.UpdateControllerIcon(players[slotIndex].Controller, slotIndex);
          if (this.mGameSettings.GameType == GameType.Versus && this.mVersusSettings.TeamsEnabled)
          {
            if (players[slotIndex].Team == Factions.NONE)
            {
              if (num1 <= num2)
              {
                players[slotIndex].Team = Factions.TEAM_RED;
                ++num1;
              }
              else
              {
                players[slotIndex].Team = Factions.TEAM_BLUE;
                ++num2;
              }
            }
            else if ((players[slotIndex].Team & Factions.TEAM_RED) != Factions.NONE)
              ++num1;
            else
              ++num2;
          }
          else
            players[slotIndex].Team = Factions.NONE;
        }
        if (!(players[slotIndex].Gamer is NetworkGamer))
        {
          switch (HackHelper.CheckLicense(players[slotIndex].Gamer.Avatar))
          {
            case HackHelper.License.Yes:
              continue;
            case HackHelper.License.Custom:
              if (this.AllowCustom)
                continue;
              break;
          }
          players[slotIndex].Gamer.Avatar = Profile.Instance.DefaultAvatar;
          if (state != NetworkState.Offline)
          {
            GamerChangedMessage iMessage = new GamerChangedMessage(players[slotIndex]);
            NetworkManager.Instance.Interface.SendMessage<GamerChangedMessage>(ref iMessage);
          }
        }
      }
      else
        players[slotIndex].Team = Factions.NONE;
    }
    NetworkManager.Instance.AbortQuery();
    LanguageManager instance = LanguageManager.Instance;
    if (NetworkManager.Instance.State == NetworkState.Client)
    {
      this.mStartButton.Enabled = false;
      GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Bottom, $"{instance.GetString(SubMenuCharacterSelect.LOC_JOIN)}/{instance.GetString(SubMenuCharacterSelect.LOC_READY)}");
      GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Right, GamePadMenuHelp.LOC_BACK);
      GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Left, "#menu_change_team".GetHashCodeCustom());
      GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Top, SubMenuCharacterSelect.LOC_TT_CUSTOMIZE);
    }
    else
    {
      this.mStartButton.Enabled = true;
      GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Bottom, $"{instance.GetString(SubMenuCharacterSelect.LOC_JOIN)}/{instance.GetString(SubMenu.LOC_SELECT)}");
      GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Right, GamePadMenuHelp.LOC_BACK);
      GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Left, "#menu_change_team".GetHashCodeCustom());
      GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Top, SubMenuCharacterSelect.LOC_TT_CUSTOMIZE);
    }
    for (int index = 0; index < this.mPlayerSlots.Length; ++index)
    {
      this.mPlayerSlots[index].SelectedItem = -1;
      if (this.mPlayerSlots[index].State != SubMenuCharacterSelect.GamerState.Locked)
        this.mPlayerSlots[index].State = SubMenuCharacterSelect.GamerState.Open;
      this.mPlayerSlots[index].Custom = players[index].Playing && HackHelper.CheckLicense(players[index].Gamer.Avatar) != HackHelper.License.Yes;
    }
    NetworkChat.Instance.Set(448, 185, SubMenu.sPagesTexture, new Rectangle(), FontManager.Instance.GetFont(MagickaFont.Maiandra18), false, 8, true, float.MaxValue);
    NetworkChat.Instance.Active = true;
    this.UpdateGamerDropDownMenu();
    this.HasSelectedLevel = this.mGameSettings.Level > -1;
    if (this.HasSelectedLevel)
      this.UpdateChapterText(LevelManager.Instance.GetLevel(this.mGameSettings.GameType, this.mGameSettings.Level));
    base.OnEnter();
  }

  public override void OnExit()
  {
    ToolTipMan.Instance.KillAll(false);
    NetworkChat.Instance.Active = false;
    SteamAPI.GameOverlayActivated -= new Action<GameOverlayActivated>(this.SteamOverlayActivated);
    Magicka.Game.Instance.AddLoadTask((Action) (() => SaveManager.Instance.SaveSettings()));
    base.OnExit();
  }

  internal void UpdateGamer(Magicka.GameLogic.Player iPlayer, Gamer iGamer)
  {
    if (iGamer == Gamer.INVALID_GAMER)
    {
      if (!(iPlayer.Controller is KeyboardMouseController))
      {
        int index = 0;
        while (index < Profile.Instance.Gamers.Count && Profile.Instance.Gamers.Values[index].InUse)
          ++index;
        this.mPlayerSlots[iPlayer.ID].SelectedItem = index;
      }
    }
    else if (iGamer != null)
    {
      this.mPlayerSlots[iPlayer.ID].Name.SetText(iGamer.GamerTag);
      this.mPlayerSlots[iPlayer.ID].AvatarSelected = true;
    }
    else
      this.mPlayerSlots[iPlayer.ID].AvatarSelected = false;
    if (iGamer is NetworkGamer)
      return;
    this.SetReady(false);
  }

  public bool[] Ready
  {
    get
    {
      bool[] ready = new bool[4];
      for (int index = 0; index < 4; ++index)
        ready[index] = this.mPlayerSlots[index].State == SubMenuCharacterSelect.GamerState.Ready;
      return ready;
    }
  }

  public bool IsTeamsEnabled
  {
    get => this.mGameSettings.GameType == GameType.Versus && this.mVersusSettings.TeamsEnabled;
  }

  internal static int CreateVertices(
    Vector4[] iVertices,
    int iStartIndex,
    ref Vector2 iSize,
    ref Vector2 iMargin,
    ref Vector2 iUVOffset,
    ref Vector2 iUVSize,
    ref Vector2 iUVMargin)
  {
    iVertices[iStartIndex].X = 0.0f;
    iVertices[iStartIndex].Y = 0.0f;
    iVertices[iStartIndex].Z = iUVOffset.X;
    iVertices[iStartIndex].W = iUVOffset.Y;
    ++iStartIndex;
    iVertices[iStartIndex].X = iMargin.X;
    iVertices[iStartIndex].Y = 0.0f;
    iVertices[iStartIndex].Z = iUVOffset.X + iUVMargin.X;
    iVertices[iStartIndex].W = iUVOffset.Y;
    ++iStartIndex;
    iVertices[iStartIndex].X = iSize.X - iMargin.X;
    iVertices[iStartIndex].Y = 0.0f;
    iVertices[iStartIndex].Z = iUVOffset.X + iUVSize.X - iUVMargin.X;
    iVertices[iStartIndex].W = iUVOffset.Y;
    ++iStartIndex;
    iVertices[iStartIndex].X = iSize.X;
    iVertices[iStartIndex].Y = 0.0f;
    iVertices[iStartIndex].Z = iUVOffset.X + iUVSize.X;
    iVertices[iStartIndex].W = iUVOffset.Y;
    ++iStartIndex;
    iVertices[iStartIndex].X = 0.0f;
    iVertices[iStartIndex].Y = iMargin.Y;
    iVertices[iStartIndex].Z = iUVOffset.X;
    iVertices[iStartIndex].W = iUVOffset.Y + iUVMargin.Y;
    ++iStartIndex;
    iVertices[iStartIndex].X = iMargin.X;
    iVertices[iStartIndex].Y = iMargin.Y;
    iVertices[iStartIndex].Z = iUVOffset.X + iUVMargin.X;
    iVertices[iStartIndex].W = iUVOffset.Y + iUVMargin.Y;
    ++iStartIndex;
    iVertices[iStartIndex].X = iSize.X - iMargin.X;
    iVertices[iStartIndex].Y = iMargin.Y;
    iVertices[iStartIndex].Z = iUVOffset.X + iUVSize.X - iUVMargin.X;
    iVertices[iStartIndex].W = iUVOffset.Y + iUVMargin.Y;
    ++iStartIndex;
    iVertices[iStartIndex].X = iSize.X;
    iVertices[iStartIndex].Y = iMargin.Y;
    iVertices[iStartIndex].Z = iUVOffset.X + iUVSize.X;
    iVertices[iStartIndex].W = iUVOffset.Y + iUVMargin.Y;
    ++iStartIndex;
    iVertices[iStartIndex].X = 0.0f;
    iVertices[iStartIndex].Y = iSize.Y - iMargin.Y;
    iVertices[iStartIndex].Z = iUVOffset.X;
    iVertices[iStartIndex].W = iUVOffset.Y + iUVSize.Y - iUVMargin.Y;
    ++iStartIndex;
    iVertices[iStartIndex].X = iMargin.X;
    iVertices[iStartIndex].Y = iSize.Y - iMargin.Y;
    iVertices[iStartIndex].Z = iUVOffset.X + iUVMargin.X;
    iVertices[iStartIndex].W = iUVOffset.Y + iUVSize.Y - iUVMargin.Y;
    ++iStartIndex;
    iVertices[iStartIndex].X = iSize.X - iMargin.X;
    iVertices[iStartIndex].Y = iSize.Y - iMargin.Y;
    iVertices[iStartIndex].Z = iUVOffset.X + iUVSize.X - iUVMargin.X;
    iVertices[iStartIndex].W = iUVOffset.Y + iUVSize.Y - iUVMargin.Y;
    ++iStartIndex;
    iVertices[iStartIndex].X = iSize.X;
    iVertices[iStartIndex].Y = iSize.Y - iMargin.Y;
    iVertices[iStartIndex].Z = iUVOffset.X + iUVSize.X;
    iVertices[iStartIndex].W = iUVOffset.Y + iUVSize.Y - iUVMargin.Y;
    ++iStartIndex;
    iVertices[iStartIndex].X = 0.0f;
    iVertices[iStartIndex].Y = iSize.Y;
    iVertices[iStartIndex].Z = iUVOffset.X;
    iVertices[iStartIndex].W = iUVOffset.Y + iUVSize.Y;
    ++iStartIndex;
    iVertices[iStartIndex].X = iMargin.X;
    iVertices[iStartIndex].Y = iSize.Y;
    iVertices[iStartIndex].Z = iUVOffset.X + iUVMargin.X;
    iVertices[iStartIndex].W = iUVOffset.Y + iUVSize.Y;
    ++iStartIndex;
    iVertices[iStartIndex].X = iSize.X - iMargin.X;
    iVertices[iStartIndex].Y = iSize.Y;
    iVertices[iStartIndex].Z = iUVOffset.X + iUVSize.X - iUVMargin.X;
    iVertices[iStartIndex].W = iUVOffset.Y + iUVSize.Y;
    ++iStartIndex;
    iVertices[iStartIndex].X = iSize.X;
    iVertices[iStartIndex].Y = iSize.Y;
    iVertices[iStartIndex].Z = iUVOffset.X + iUVSize.X;
    iVertices[iStartIndex].W = iUVOffset.Y + iUVSize.Y;
    ++iStartIndex;
    return iStartIndex;
  }

  internal static int CreateVertices(
    SubMenuCharacterSelect.VertexPositionTextureTexture[] iVertices,
    int iStartIndex,
    ref Vector2 iSize,
    ref Vector2 iMargin,
    ref Vector2 iUVOffset0,
    ref Vector2 iUVOffset1,
    ref Vector2 iUVSize,
    ref Vector2 iUVMargin)
  {
    iVertices[iStartIndex].Position.X = 0.0f;
    iVertices[iStartIndex].Position.Y = 0.0f;
    iVertices[iStartIndex].TexCoord0.X = iUVOffset0.X;
    iVertices[iStartIndex].TexCoord0.Y = iUVOffset0.Y;
    iVertices[iStartIndex].TexCoord1.X = iUVOffset1.X;
    iVertices[iStartIndex].TexCoord1.Y = iUVOffset1.Y;
    ++iStartIndex;
    iVertices[iStartIndex].Position.X = iMargin.X;
    iVertices[iStartIndex].Position.Y = 0.0f;
    iVertices[iStartIndex].TexCoord0.X = iUVOffset0.X + iUVMargin.X;
    iVertices[iStartIndex].TexCoord0.Y = iUVOffset0.Y;
    iVertices[iStartIndex].TexCoord1.X = iUVOffset1.X + iUVMargin.X;
    iVertices[iStartIndex].TexCoord1.Y = iUVOffset1.Y;
    ++iStartIndex;
    iVertices[iStartIndex].Position.X = iSize.X - iMargin.X;
    iVertices[iStartIndex].Position.Y = 0.0f;
    iVertices[iStartIndex].TexCoord0.X = iUVOffset0.X + iUVSize.X - iUVMargin.X;
    iVertices[iStartIndex].TexCoord0.Y = iUVOffset0.Y;
    iVertices[iStartIndex].TexCoord1.X = iUVOffset1.X + iUVSize.X - iUVMargin.X;
    iVertices[iStartIndex].TexCoord1.Y = iUVOffset1.Y;
    ++iStartIndex;
    iVertices[iStartIndex].Position.X = iSize.X;
    iVertices[iStartIndex].Position.Y = 0.0f;
    iVertices[iStartIndex].TexCoord0.X = iUVOffset0.X + iUVSize.X;
    iVertices[iStartIndex].TexCoord0.Y = iUVOffset0.Y;
    iVertices[iStartIndex].TexCoord1.X = iUVOffset1.X + iUVSize.X;
    iVertices[iStartIndex].TexCoord1.Y = iUVOffset1.Y;
    ++iStartIndex;
    iVertices[iStartIndex].Position.X = 0.0f;
    iVertices[iStartIndex].Position.Y = iMargin.Y;
    iVertices[iStartIndex].TexCoord0.X = iUVOffset0.X;
    iVertices[iStartIndex].TexCoord0.Y = iUVOffset0.Y + iUVMargin.Y;
    iVertices[iStartIndex].TexCoord1.X = iUVOffset1.X;
    iVertices[iStartIndex].TexCoord1.Y = iUVOffset1.Y + iUVMargin.Y;
    ++iStartIndex;
    iVertices[iStartIndex].Position.X = iMargin.X;
    iVertices[iStartIndex].Position.Y = iMargin.Y;
    iVertices[iStartIndex].TexCoord0.X = iUVOffset0.X + iUVMargin.X;
    iVertices[iStartIndex].TexCoord0.Y = iUVOffset0.Y + iUVMargin.Y;
    iVertices[iStartIndex].TexCoord1.X = iUVOffset1.X + iUVMargin.X;
    iVertices[iStartIndex].TexCoord1.Y = iUVOffset1.Y + iUVMargin.Y;
    ++iStartIndex;
    iVertices[iStartIndex].Position.X = iSize.X - iMargin.X;
    iVertices[iStartIndex].Position.Y = iMargin.Y;
    iVertices[iStartIndex].TexCoord0.X = iUVOffset0.X + iUVSize.X - iUVMargin.X;
    iVertices[iStartIndex].TexCoord0.Y = iUVOffset0.Y + iUVMargin.Y;
    iVertices[iStartIndex].TexCoord1.X = iUVOffset1.X + iUVSize.X - iUVMargin.X;
    iVertices[iStartIndex].TexCoord1.Y = iUVOffset1.Y + iUVMargin.Y;
    ++iStartIndex;
    iVertices[iStartIndex].Position.X = iSize.X;
    iVertices[iStartIndex].Position.Y = iMargin.Y;
    iVertices[iStartIndex].TexCoord0.X = iUVOffset0.X + iUVSize.X;
    iVertices[iStartIndex].TexCoord0.Y = iUVOffset0.Y + iUVMargin.Y;
    iVertices[iStartIndex].TexCoord1.X = iUVOffset1.X + iUVSize.X;
    iVertices[iStartIndex].TexCoord1.Y = iUVOffset1.Y + iUVMargin.Y;
    ++iStartIndex;
    iVertices[iStartIndex].Position.X = 0.0f;
    iVertices[iStartIndex].Position.Y = iSize.Y - iMargin.Y;
    iVertices[iStartIndex].TexCoord0.X = iUVOffset0.X;
    iVertices[iStartIndex].TexCoord0.Y = iUVOffset0.Y + iUVSize.Y - iUVMargin.Y;
    iVertices[iStartIndex].TexCoord1.X = iUVOffset1.X;
    iVertices[iStartIndex].TexCoord1.Y = iUVOffset1.Y + iUVSize.Y - iUVMargin.Y;
    ++iStartIndex;
    iVertices[iStartIndex].Position.X = iMargin.X;
    iVertices[iStartIndex].Position.Y = iSize.Y - iMargin.Y;
    iVertices[iStartIndex].TexCoord0.X = iUVOffset0.X + iUVMargin.X;
    iVertices[iStartIndex].TexCoord0.Y = iUVOffset0.Y + iUVSize.Y - iUVMargin.Y;
    iVertices[iStartIndex].TexCoord1.X = iUVOffset1.X + iUVMargin.X;
    iVertices[iStartIndex].TexCoord1.Y = iUVOffset1.Y + iUVSize.Y - iUVMargin.Y;
    ++iStartIndex;
    iVertices[iStartIndex].Position.X = iSize.X - iMargin.X;
    iVertices[iStartIndex].Position.Y = iSize.Y - iMargin.Y;
    iVertices[iStartIndex].TexCoord0.X = iUVOffset0.X + iUVSize.X - iUVMargin.X;
    iVertices[iStartIndex].TexCoord0.Y = iUVOffset0.Y + iUVSize.Y - iUVMargin.Y;
    iVertices[iStartIndex].TexCoord1.X = iUVOffset1.X + iUVSize.X - iUVMargin.X;
    iVertices[iStartIndex].TexCoord1.Y = iUVOffset1.Y + iUVSize.Y - iUVMargin.Y;
    ++iStartIndex;
    iVertices[iStartIndex].Position.X = iSize.X;
    iVertices[iStartIndex].Position.Y = iSize.Y - iMargin.Y;
    iVertices[iStartIndex].TexCoord0.X = iUVOffset0.X + iUVSize.X;
    iVertices[iStartIndex].TexCoord0.Y = iUVOffset0.Y + iUVSize.Y - iUVMargin.Y;
    iVertices[iStartIndex].TexCoord1.X = iUVOffset1.X + iUVSize.X;
    iVertices[iStartIndex].TexCoord1.Y = iUVOffset1.Y + iUVSize.Y - iUVMargin.Y;
    ++iStartIndex;
    iVertices[iStartIndex].Position.X = 0.0f;
    iVertices[iStartIndex].Position.Y = iSize.Y;
    iVertices[iStartIndex].TexCoord0.X = iUVOffset0.X;
    iVertices[iStartIndex].TexCoord0.Y = iUVOffset0.Y + iUVSize.Y;
    iVertices[iStartIndex].TexCoord1.X = iUVOffset1.X;
    iVertices[iStartIndex].TexCoord1.Y = iUVOffset1.Y + iUVSize.Y;
    ++iStartIndex;
    iVertices[iStartIndex].Position.X = iMargin.X;
    iVertices[iStartIndex].Position.Y = iSize.Y;
    iVertices[iStartIndex].TexCoord0.X = iUVOffset0.X + iUVMargin.X;
    iVertices[iStartIndex].TexCoord0.Y = iUVOffset0.Y + iUVSize.Y;
    iVertices[iStartIndex].TexCoord1.X = iUVOffset1.X + iUVMargin.X;
    iVertices[iStartIndex].TexCoord1.Y = iUVOffset1.Y + iUVSize.Y;
    ++iStartIndex;
    iVertices[iStartIndex].Position.X = iSize.X - iMargin.X;
    iVertices[iStartIndex].Position.Y = iSize.Y;
    iVertices[iStartIndex].TexCoord0.X = iUVOffset0.X + iUVSize.X - iUVMargin.X;
    iVertices[iStartIndex].TexCoord0.Y = iUVOffset0.Y + iUVSize.Y;
    iVertices[iStartIndex].TexCoord1.X = iUVOffset1.X + iUVSize.X - iUVMargin.X;
    iVertices[iStartIndex].TexCoord1.Y = iUVOffset1.Y + iUVSize.Y;
    ++iStartIndex;
    iVertices[iStartIndex].Position.X = iSize.X;
    iVertices[iStartIndex].Position.Y = iSize.Y;
    iVertices[iStartIndex].TexCoord0.X = iUVOffset0.X + iUVSize.X;
    iVertices[iStartIndex].TexCoord0.Y = iUVOffset0.Y + iUVSize.Y;
    iVertices[iStartIndex].TexCoord1.X = iUVOffset1.X + iUVSize.X;
    iVertices[iStartIndex].TexCoord1.Y = iUVOffset1.Y + iUVSize.Y;
    ++iStartIndex;
    return iStartIndex;
  }

  private bool Level_CheckIfLocked(int idx)
  {
    return SubMenuCharacterSelect.mLevelRepresentations != null && idx >= 0 && idx <= SubMenuCharacterSelect.mLevelRepresentations.Count - 1 && SubMenuCharacterSelect.mLevelRepresentations[idx].IsLocked;
  }

  private bool Level_CheckIfUsed(int idx)
  {
    return SubMenuCharacterSelect.mLevelRepresentations != null && idx >= 0 && idx <= SubMenuCharacterSelect.mLevelRepresentations.Count - 1 && SubMenuCharacterSelect.mLevelRepresentations[idx].IsUsed;
  }

  private bool Level_CheckIfNew(int idx)
  {
    return SubMenuCharacterSelect.mLevelRepresentations != null && idx >= 0 && idx <= SubMenuCharacterSelect.mLevelRepresentations.Count - 1 && SubMenuCharacterSelect.mLevelRepresentations[idx].IsNew;
  }

  private bool Level_CheckIfFree(int idx)
  {
    return SubMenuCharacterSelect.mLevelRepresentations != null && idx >= 0 && idx <= SubMenuCharacterSelect.mLevelRepresentations.Count - 1 && SubMenuCharacterSelect.mLevelRepresentations[idx].IsFree;
  }

  public void SetLevelUsed(string lvlName)
  {
    if (SubMenuCharacterSelect.mLevelRepresentations == null || SubMenuCharacterSelect.mLevelRepresentations.Count == 0 || string.IsNullOrEmpty(lvlName))
      return;
    int index = 0;
    bool flag = false;
    foreach (SubMenuCharacterSelect.LevelRep levelRepresentation in SubMenuCharacterSelect.mLevelRepresentations)
    {
      if (string.Compare(levelRepresentation.Name, lvlName) == 0)
      {
        DLC_StatusHelper.Instance.Item_TrySetUsed("level", levelRepresentation.Name, true);
        flag = true;
        break;
      }
      ++index;
    }
    if (!flag)
      return;
    SubMenuCharacterSelect.mLevelRepresentations[index].IsUsed = true;
  }

  private bool Robe_CheckIfLocked(int idx)
  {
    return SubMenuCharacterSelect.mRobeRepresentations != null && idx >= 0 && idx <= SubMenuCharacterSelect.mRobeRepresentations.Count - 1 && SubMenuCharacterSelect.mRobeRepresentations[idx].IsLocked;
  }

  private bool Robe_CheckIfUsed(int idx)
  {
    return SubMenuCharacterSelect.mRobeRepresentations != null && idx >= 0 && idx <= SubMenuCharacterSelect.mRobeRepresentations.Count - 1 && SubMenuCharacterSelect.mRobeRepresentations[idx].IsUsed;
  }

  private bool Robe_CheckIfFree(int idx)
  {
    return SubMenuCharacterSelect.mRobeRepresentations != null && idx >= 0 && idx <= SubMenuCharacterSelect.mRobeRepresentations.Count - 1 && SubMenuCharacterSelect.mRobeRepresentations[idx].IsFree;
  }

  private bool Robe_CheckIfNew(int idx)
  {
    return SubMenuCharacterSelect.mRobeRepresentations != null && idx >= 0 && idx <= SubMenuCharacterSelect.mRobeRepresentations.Count - 1 && SubMenuCharacterSelect.mRobeRepresentations[idx].IsNew;
  }

  private void SetRobeUsed(string robeName)
  {
    if (SubMenuCharacterSelect.mRobeRepresentations == null || SubMenuCharacterSelect.mRobeRepresentations.Count == 0 || string.IsNullOrEmpty(robeName) || string.Compare(robeName, "wizard") == 0)
      return;
    int index = 0;
    bool flag = false;
    foreach (SubMenuCharacterSelect.RobeRep robeRepresentation in SubMenuCharacterSelect.mRobeRepresentations)
    {
      if (string.Compare(robeRepresentation.Name, robeName) == 0)
      {
        DLC_StatusHelper.Instance.Item_TrySetUsed("robe", robeRepresentation.Name, true);
        flag = true;
        break;
      }
      ++index;
    }
    if (!flag)
      return;
    SubMenuCharacterSelect.mRobeRepresentations[index].IsUsed = true;
  }

  internal struct VertexPositionTextureTexture
  {
    public const int SIZE = 24;
    public Vector2 Position;
    public Vector2 TexCoord0;
    public Vector2 TexCoord1;
    public static readonly VertexElement[] VertexElements = new VertexElement[3]
    {
      new VertexElement((short) 0, (short) 0, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.Position, (byte) 0),
      new VertexElement((short) 0, (short) 8, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, (byte) 0),
      new VertexElement((short) 0, (short) 16 /*0x10*/, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, (byte) 1)
    };
  }

  private enum GamerState
  {
    Locked = -1, // 0xFFFFFFFF
    Open = 0,
    CustomizingAvatar = 1,
    CustomizingColor = 2,
    Ready = 3,
  }

  private struct PlayerState
  {
    public SubMenuCharacterSelect.GamerState State;
    public Text Name;
    public int Latency;
    public Text LatencyText;
    public int Team;
    public int SelectedItem;
    public int ScrollValue;
    public bool Custom;
    public int ConsecutiveColorChanges;
    public bool AvatarSelected;
    public int ControllerType;

    public void SetLatency(int iLatencyMS)
    {
      this.Latency = iLatencyMS;
      string iText = LanguageManager.Instance.GetString(SubMenuCharacterSelect.LOC_LATENCY).Replace("#1;", iLatencyMS.ToString());
      this.LatencyText.DefaultColor = iLatencyMS >= 100 ? (iLatencyMS >= 200 ? new Vector4(1f, 0.0f, 0.0f, 1f) : new Vector4(1f, 1f, 0.0f, 1f)) : new Vector4(0.0f, 1f, 0.0f, 1f);
      this.LatencyText.SetText(iText);
    }
  }

  private enum State
  {
    Normal,
    ChangingLevel,
    ChangingPacks,
    CountDown,
  }

  private class SelectableObjectRep
  {
    public string Name = "UNDEFINED";
    public bool IsLocked = true;
    public bool IsUsed;
    public bool IsCustom;
    public bool IsFree;
    public bool IsNew;
    public uint BelongsToAppID;
    public byte[] HashSum;
    protected int originalIndex = -1;

    public int OriginalIndex
    {
      get => this.originalIndex;
      set => this.originalIndex = value;
    }

    public int SortVal
    {
      get
      {
        if (this.IsCustom)
          return -5;
        if (this.IsNew && this.IsLocked)
          return this.IsFree ? -3 : -4;
        if (this.IsFree && !this.IsUsed || !this.IsLocked && !this.IsUsed)
          return -2;
        if (this.IsFree)
          return -1;
        if (this.IsUsed && !this.IsLocked && !this.IsFree)
          return this.IsNew ? -1 : 0;
        if (this.IsLocked && this.IsFree)
          return 1;
        return this.IsLocked ? 2 : 3;
      }
    }

    public static int CompareRep(
      SubMenuCharacterSelect.SelectableObjectRep A,
      SubMenuCharacterSelect.SelectableObjectRep B)
    {
      if (A == null)
        return B == null ? 0 : -1;
      if (B == null)
        return 1;
      int sortVal1 = A.SortVal;
      int sortVal2 = B.SortVal;
      if (sortVal1 < sortVal2)
        return -1;
      return sortVal1 != sortVal2 ? 1 : 0;
    }

    public override string ToString()
    {
      return string.Format("Index = {0}, Name = \"{1}\", BelongsToAppID = {2}, {3}, {4}, {5}, {6}", (object) this.OriginalIndex, (object) this.Name, (object) this.BelongsToAppID, this.IsFree ? (object) "FREE" : (object) "NOT FREE", this.IsLocked ? (object) "LOCKED" : (object) "UNLOCKED", this.IsUsed ? (object) "USED" : (object) "UNUSED", this.IsCustom ? (object) "CUSTOM" : (object) "NOT CUSTOM", this.IsNew ? (object) "NEW" : (object) "NOT NEW");
    }
  }

  private sealed class LevelRep : SubMenuCharacterSelect.SelectableObjectRep
  {
    public Text Title;
    public Text Descr;
    public Texture2D PreviewImage;
    public string FileName;
  }

  private sealed class RobeRep : SubMenuCharacterSelect.SelectableObjectRep
  {
    public int DisplayName;
    public int Description;
  }
}
