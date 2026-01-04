// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.GameStates.Menu.Main.SubMenuOnline
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.UI;
using Magicka.Graphics;
using Magicka.Levels.Campaign;
using Magicka.Localization;
using Magicka.Network;
using Magicka.Storage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PolygonHead;
using SteamWrapper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows.Forms;

#nullable disable
namespace Magicka.GameLogic.GameStates.Menu.Main;

internal class SubMenuOnline : SubMenu
{
  private const int MAX_VISIBLE_SERVERS = 12;
  private const int QUICK = 0;
  private const int HOST = 1;
  private const int MANUAL = 2;
  private const int JOIN = 3;
  private const int BACK = 4;
  private const int FILTER_PANEL = 5;
  private const int NAME = 6;
  private const int LEVEL = 7;
  private const int NR_PLAYERS = 8;
  private const int PING = 9;
  private static SubMenuOnline sSingelton;
  private static volatile object sSingeltonLock = new object();
  public static readonly int LOC_ONLINE = "#network_02".GetHashCodeCustom();
  public static readonly int LOC_IP = "#network_03".GetHashCodeCustom();
  public static readonly int LOC_SEARCH = "#network_04".GetHashCodeCustom();
  public static readonly int LOC_REFRESH = "#network_05".GetHashCodeCustom();
  public static readonly int LOC_KICK = "#network_06".GetHashCodeCustom();
  public static readonly int LOC_BAN = "#network_07".GetHashCodeCustom();
  public static readonly int LOC_RETRY = "#network_08".GetHashCodeCustom();
  public static readonly int LOC_DISCONNECTED = "#network_09".GetHashCodeCustom();
  public static readonly int LOC_DISCONNECT = "#network_10".GetHashCodeCustom();
  public static readonly int LOC_CONNECT = "#network_11".GetHashCodeCustom();
  public static readonly int LOC_CONNECTED = "#network_12".GetHashCodeCustom();
  public static readonly int LOC_PING = "#network_13".GetHashCodeCustom();
  public static readonly int LOC_INVITE_FRIEND = "#network_14".GetHashCodeCustom();
  public static readonly int LOC_FRIENDS = "#network_15".GetHashCodeCustom();
  public static readonly int LOC_LATENCY = "#network_16".GetHashCodeCustom();
  public static readonly int LOC_INFO = "#network_17".GetHashCodeCustom();
  public static readonly int LOC_DETAILS = "#network_18".GetHashCodeCustom();
  public static readonly int LOC_SIGN_IN = "#network_19".GetHashCodeCustom();
  public static readonly int LOC_UPDATE = "#network_20".GetHashCodeCustom();
  public static readonly int LOC_NO_GAMES_FOUND = "#network_21".GetHashCodeCustom();
  public static readonly int LOC_GAMES_FOUND = "#network_22".GetHashCodeCustom();
  public static readonly int LOC_LOADING = "#network_23".GetHashCodeCustom();
  public static readonly int LOC_WAITING_FOR_OTHER_PLAYERS = "#network_24".GetHashCodeCustom();
  public static readonly int LOC_SERVERS = "#network_25".GetHashCodeCustom();
  public static readonly int LOC_QUICK_START = "#network_26".GetHashCodeCustom();
  public static readonly int LOC_VAC = "#network_27".GetHashCodeCustom();
  public static readonly int LOC_FILTER = "#network_28".GetHashCodeCustom();
  public static readonly int LOC_ALLOW_MIDGAME_JOIN = "#network_29".GetHashCodeCustom();
  public static readonly int LOC_PRIVATE = "#network_30".GetHashCodeCustom();
  public static readonly int LOC_LOCAL = "#network_31".GetHashCodeCustom();
  public static readonly int LOC_ALL = "#network_33".GetHashCodeCustom();
  public static readonly int LOC_HOST = "#network_34".GetHashCodeCustom();
  public static readonly int LOC_FREESLOTS = "#network_35".GetHashCodeCustom();
  public static readonly int LOC_FRIENDS_ONLY = "#network_36".GetHashCodeCustom();
  public static readonly int LOC_LAN_ABBR = "#network_37".GetHashCodeCustom();
  public static readonly int LOC_CONNECTING = "#network_40".GetHashCodeCustom();
  public static readonly int LOC_CHAT = "#network_41".GetHashCodeCustom();
  public static readonly int LOC_SHOW_CHAT = "#network_42".GetHashCodeCustom();
  public static readonly int LOC_HIDE_CHAT = "#network_43".GetHashCodeCustom();
  public static readonly int LOC_NO_LEVEL_SELECTED = "#network_no_level_selected".GetHashCodeCustom();
  public static readonly int LOC_HOST_ONLINE = "#menu_opt_online_01".GetHashCodeCustom();
  public static readonly int LOC_STOP_HOST = "#menu_opt_online_02".GetHashCodeCustom();
  public static readonly int LOC_JOIN = "#add_menu_join".GetHashCodeCustom();
  public static readonly int LOC_JOIN_ONLINE = "#menu_opt_online_03".GetHashCodeCustom();
  public static readonly int LOC_MANUAL = "#add_menu_ip".GetHashCodeCustom();
  public static readonly int LOC_NAME = "#add_menu_gamename".GetHashCodeCustom();
  public static readonly int LOC_MODE = "#menu_vs_02".GetHashCodeCustom();
  public static readonly int LOC_LEVEL = "#menu_vs_13".GetHashCodeCustom();
  public static readonly int LOC_PLAYERS = "#network_39".GetHashCodeCustom();
  public static readonly int LOC_PORT = "#add_menu_port".GetHashCodeCustom();
  public static readonly int LOC_SEARCHING = "#network_searching".GetHashCodeCustom();
  public static readonly int LOC_CHALLENGE = "#network_challenge".GetHashCodeCustom();
  public static readonly int LOC_STORYCHALLANGE = "#network_storychallenge".GetHashCodeCustom();
  public static readonly int LOC_STATUS_INITIALIZING = "#status_p01_initializing".GetHashCodeCustom();
  public static readonly int LOC_STATUS_CONNECTING = "#status_p01_connecting".GetHashCodeCustom();
  public static readonly int LOC_STATUS_AUTHENTICATING = "#status_p01_authenticating".GetHashCodeCustom();
  public static readonly int LOC_STATUS_CONNECTED = "#status_p01_connected".GetHashCodeCustom();
  public static readonly int LOC_SETTINGS_PASSWORD = "#settings_p01_password".GetHashCodeCustom();
  public static readonly int LOC_SETTINGS_VISIBILITY = "#settings_p01_visibility".GetHashCodeCustom();
  public static readonly int LOC_SETTINGS_PRIVATE = "#settings_p01_private".GetHashCodeCustom();
  public static readonly int LOC_SETTINGS_PUBLIC = "#settings_p01_public".GetHashCodeCustom();
  public static readonly int LOC_SETTINGS_SHOW = "#settings_p01_show".GetHashCodeCustom();
  public static readonly int LOC_SETTINGS_HIDE = "#settings_p01_hide".GetHashCodeCustom();
  public static readonly int LOC_ERROR_MISMATCH = "#error_p01_mismatch".GetHashCodeCustom();
  public static readonly int LOC_ERROR_UNKNOWN = "#error_p01_unknown".GetHashCodeCustom();
  public static readonly int LOC_ERROR_SERVERFULL = "#error_p01_serverfull".GetHashCodeCustom();
  public static readonly int LOC_ERROR_AUTHFAIL = "#error_p01_authfail".GetHashCodeCustom();
  public static readonly int LOC_ERROR_INPROGRESS = "#error_p01_inprogress".GetHashCodeCustom();
  public static readonly int LOC_ERROR_VERSION = "#error_p01_version".GetHashCodeCustom();
  public static readonly int LOC_ERROR_PASSWORD = "#error_p01_password".GetHashCodeCustom();
  public static readonly int LOC_ERROR_NO_GAMER = "#error_p01_no_gamer".GetHashCodeCustom();
  public static readonly int LOC_ERROR_TIMEOUT = "#error_p01_timeout".GetHashCodeCustom();
  public static readonly int LOC_SEARCH_SCOPE = "#search_scope".GetHashCodeCustom();
  public static readonly int LOC_LAN = "#lan".GetHashCodeCustom();
  public static readonly int LOC_VAC_ONLY = "#vac_only".GetHashCodeCustom();
  public static readonly int LOC_PLAYING = "#playing".GetHashCodeCustom();
  public static readonly int LOC_TT_HIDE_PRIVATE_GAMES = "#hide_private_games".GetHashCodeCustom();
  public static readonly int LOC_TT_HIDE_GAMES_IN_PROGRESS = "#hide_games_in_progress".GetHashCodeCustom();
  public static readonly int LOC_TT_SHOW_ONLY_GAMES_PROTECTED_BY_VAC = "#show_vac_only".GetHashCodeCustom();
  private static readonly int LOC_TT_NAME = "#tooltip_sb_name".GetHashCodeCustom();
  private static readonly int LOC_TT_LEVEL = "#tooltip_sb_level".GetHashCodeCustom();
  private static readonly int LOC_TT_PLAYERS = "#tooltip_sb_players".GetHashCodeCustom();
  private static readonly int LOC_TT_PING = "#tooltip_sb_ping".GetHashCodeCustom();
  private static readonly int LOC_TT_FILTER = "#tooltip_sb_filter".GetHashCodeCustom();
  private static readonly int LOC_TT_HOST = "#tooltip_sb_host".GetHashCodeCustom();
  private static readonly int LOC_TT_QUICK = "#tooltip_sb_quick".GetHashCodeCustom();
  private static readonly int LOC_TT_REFRESH = "#tooltip_sb_refresh".GetHashCodeCustom();
  private static readonly int LOC_TT_IP = "#tooltip_sb_ip".GetHashCodeCustom();
  private static readonly int LOC_TT_JOIN = "#tooltip_sb_join".GetHashCodeCustom();
  private new static readonly int LOC_PASSWORD = "#SETTINGS_P01_PASSWORD".GetHashCodeCustom();
  private static VertexBuffer sBackgroundVertexBuffer;
  private static VertexDeclaration sBackgroundVertexDeclaration;
  private static VertexBuffer sVertexBuffer;
  private static VertexDeclaration sVertexDeclaration;
  private static LobbyDistanceFilter _quickJoinSearchDistance;
  private ReadOnlyCollection<GameServerItem> mServers;
  private int mServerTop;
  private int mSelectedServer;
  private MenuScrollBar mServerScroll;
  private TextInputMessageBox mPasswordMessageBox;
  private List<MenuTextItem> mTitleItems = new List<MenuTextItem>();
  private static readonly Vector2 LIST_BG_POSITION = new Vector2(144f, 288f);
  private static readonly Vector2 LIST_BG_SIZE = new Vector2(800f, 416f);
  private static readonly Vector2 LIST_TOP_POSITION = new Vector2(128f, SubMenuOnline.LIST_BG_POSITION.Y - 128f);
  private static readonly Vector2 LIST_TOP_SIZE = new Vector2(832f, 144f);
  private static readonly Vector2 LIST_HEADER_POSITION = new Vector2((float) ((double) SubMenuOnline.LIST_TOP_POSITION.X + 20.0 + 16.0), SubMenuOnline.LIST_TOP_POSITION.Y + SubMenuOnline.LIST_TOP_SIZE.Y * 0.5f);
  private int mHighlitTab = -1;
  private int mSelectedTab;
  private Text[] mTabTexts;
  private FilterPanel mFilterPanel;
  private FilteredServerList mFilteredServerList;
  private DirectionalKeyboardHelper mInputHelper;
  private bool mServersSelected;

  public static SubMenuOnline Instance
  {
    get
    {
      if (SubMenuOnline.sSingelton == null)
      {
        lock (SubMenuOnline.sSingeltonLock)
        {
          if (SubMenuOnline.sSingelton == null)
            SubMenuOnline.sSingelton = new SubMenuOnline();
        }
      }
      return SubMenuOnline.sSingelton;
    }
  }

  private static string GetModeString(GameType iMode)
  {
    switch (iMode)
    {
      case GameType.Campaign:
        return LanguageManager.Instance.GetString(SubMenu.LOC_ADVENTURE);
      case GameType.Challenge:
        return LanguageManager.Instance.GetString(SubMenuOnline.LOC_CHALLENGE);
      case GameType.Versus:
        return LanguageManager.Instance.GetString(SubMenu.LOC_VERSUS);
      case GameType.Mythos:
        return LanguageManager.Instance.GetString(SubMenu.LOC_MYTHOS);
      case GameType.StoryChallange:
        return LanguageManager.Instance.GetString(SubMenuOnline.LOC_STORYCHALLANGE);
      default:
        return "Incorrect gametype!";
    }
  }

  public static string GetLevelString(string iLevel)
  {
    string[] strArray = iLevel.Split('|');
    if (strArray == null || strArray.Length <= 1)
      return iLevel;
    GameType iMode = (GameType) byte.Parse(strArray[0]);
    string str = strArray[1];
    int result = 0;
    return (!int.TryParse(str, out result) ? LevelManager.GetLocalizedName(str) : (result != SubMenuOnline.LOC_NO_LEVEL_SELECTED ? LevelManager.GetLocalizedName(str) : LanguageManager.Instance.GetString(SubMenuOnline.LOC_NO_LEVEL_SELECTED))) ?? SubMenuOnline.GetModeString(iMode);
  }

  private SubMenuOnline()
  {
    VertexPositionColor[] data = new VertexPositionColor[Defines.QUAD_COL_VERTS_TL.Length];
    Defines.QUAD_COL_VERTS_TL.CopyTo((Array) data, 0);
    for (int index = 0; index < data.Length; ++index)
      data[index].Color.A = (byte) 127 /*0x7F*/;
    lock (Magicka.Game.Instance.GraphicsDevice)
    {
      SubMenuOnline.sVertexBuffer = new VertexBuffer(Magicka.Game.Instance.GraphicsDevice, Defines.QUAD_TEX_VERTS_TL.Length * VertexPositionTexture.SizeInBytes, BufferUsage.WriteOnly);
      SubMenuOnline.sVertexBuffer.SetData<VertexPositionTexture>(Defines.QUAD_TEX_VERTS_TL);
      SubMenuOnline.sVertexDeclaration = RenderManager.Instance.CreateVertexDeclaration(VertexPositionTexture.VertexElements);
      SubMenuOnline.sBackgroundVertexBuffer = new VertexBuffer(Magicka.Game.Instance.GraphicsDevice, data.Length * VertexPositionColor.SizeInBytes, BufferUsage.WriteOnly);
      SubMenuOnline.sBackgroundVertexBuffer.SetData<VertexPositionColor>(data);
      SubMenuOnline.sBackgroundVertexDeclaration = RenderManager.Instance.CreateVertexDeclaration(VertexPositionColor.VertexElements);
    }
    this.mPasswordMessageBox = new TextInputMessageBox(SubMenuOnline.LOC_SETTINGS_PASSWORD, 10);
    this.mMenuTitle = new Text(30, FontManager.Instance.GetFont(MagickaFont.MenuTitle), TextAlign.Center, false);
    this.mMenuTitle.SetText(LanguageManager.Instance.GetString(SubMenu.LOC_ONLINE_PLAY));
    this.mMenuItems = new List<Magicka.GameLogic.GameStates.Menu.MenuItem>();
    Vector2 iPosition = new Vector2(160f, 752f);
    BitmapFont font1 = FontManager.Instance.GetFont(MagickaFont.Maiandra18);
    this.mTitleItems.Add(new MenuTextItem(SubMenuOnline.LOC_NAME, new Vector2(SubMenuOnline.LIST_HEADER_POSITION.X, SubMenuOnline.LIST_HEADER_POSITION.Y + 14f), font1, TextAlign.Left));
    this.mTitleItems.Add(new MenuTextItem(SubMenuOnline.LOC_LEVEL, new Vector2(SubMenuOnline.LIST_HEADER_POSITION.X + 330f, SubMenuOnline.LIST_HEADER_POSITION.Y + 14f), font1, TextAlign.Left));
    this.mTitleItems.Add(new MenuTextItem("###", new Vector2(SubMenuOnline.LIST_HEADER_POSITION.X + 630f, SubMenuOnline.LIST_HEADER_POSITION.Y + 14f), font1, TextAlign.Left));
    this.mTitleItems.Add(new MenuTextItem(SubMenuOnline.LOC_PING, new Vector2(SubMenuOnline.LIST_HEADER_POSITION.X + 690f, SubMenuOnline.LIST_HEADER_POSITION.Y + 14f), font1, TextAlign.Left));
    this.mSelectedTab = MagickaMath.CountTrailingZeroBits((uint) GlobalSettings.Instance.Filter.GameType);
    this.mTabTexts = new Text[3];
    for (int index = 0; index < this.mTabTexts.Length; ++index)
      this.mTabTexts[index] = new Text(64 /*0x40*/, font1, TextAlign.Center, false);
    this.mTabTexts[0].SetText(LanguageManager.Instance.GetString("#menu_main_01".GetHashCodeCustom()));
    this.mTabTexts[1].SetText(LanguageManager.Instance.GetString("#menu_main_02".GetHashCodeCustom()));
    this.mTabTexts[2].SetText(LanguageManager.Instance.GetString("#menu_main_03".GetHashCodeCustom()));
    RowItem[] rowItemArray = new RowItem[4];
    rowItemArray[0].Alignment = TextAlign.Left;
    rowItemArray[0].RelativePosition = 0.0f;
    rowItemArray[0].Text = "First";
    rowItemArray[1].Alignment = TextAlign.Left;
    rowItemArray[1].RelativePosition = 0.38f;
    rowItemArray[1].Text = "Second";
    rowItemArray[2].Alignment = TextAlign.Right;
    rowItemArray[2].RelativePosition = 0.86f;
    rowItemArray[2].Text = "Third";
    rowItemArray[3].Alignment = TextAlign.Right;
    rowItemArray[3].RelativePosition = 0.96f;
    rowItemArray[3].Text = "Fourth";
    Vector2 iSize = new Vector2(724f, 35f);
    for (int index = 0; index < 12; ++index)
    {
      this.mMenuItems.Add((Magicka.GameLogic.GameStates.Menu.MenuItem) new MenuTextRowItem(new Vector2(216f, (float) (288.0 + (double) iSize.Y * (double) index)), iSize, font1, rowItemArray));
      this.mMenuItems[index].Enabled = false;
    }
    this.mServerScroll = new MenuScrollBar(new Vector2(928f, SubMenuOnline.LIST_BG_POSITION.Y + SubMenuOnline.LIST_BG_SIZE.Y * 0.5f), SubMenuOnline.LIST_BG_SIZE.Y, 0);
    BitmapFont font2 = FontManager.Instance.GetFont(MagickaFont.MenuOption);
    iPosition.Y += 64f;
    iPosition.X = SubMenuOnline.LIST_BG_POSITION.X + SubMenuOnline.LIST_BG_SIZE.X * 0.5f;
    this.mMenuItems.Add((Magicka.GameLogic.GameStates.Menu.MenuItem) new MenuTextButtonItem(iPosition, SubMenu.sPagesTexture, MenuTextButtonItem.DEFAULT_UV_OFFSET, MenuTextButtonItem.DEFAULT_SIZE, SubMenuOnline.LOC_QUICK_START, font2, 260f, 260f, TextAlign.Center));
    iPosition.X = SubMenuOnline.LIST_BG_POSITION.X;
    this.mMenuItems.Add((Magicka.GameLogic.GameStates.Menu.MenuItem) new MenuTextButtonItem(iPosition, SubMenu.sPagesTexture, MenuTextButtonItem.DEFAULT_UV_OFFSET, MenuTextButtonItem.DEFAULT_SIZE, SubMenuOnline.LOC_HOST, font2, 260f, 260f, TextAlign.Left));
    iPosition.X = SubMenuOnline.LIST_BG_POSITION.X + SubMenuOnline.LIST_BG_SIZE.X;
    this.mMenuItems.Add((Magicka.GameLogic.GameStates.Menu.MenuItem) new MenuTextButtonItem(iPosition, SubMenu.sPagesTexture, MenuTextButtonItem.DEFAULT_UV_OFFSET, MenuTextButtonItem.DEFAULT_SIZE, SubMenuOnline.LOC_MANUAL, font2, 260f, 260f, TextAlign.Right));
    iPosition.Y = SubMenu.BACK_POSITION.Y + 40f;
    this.mMenuItems.Add((Magicka.GameLogic.GameStates.Menu.MenuItem) new MenuTextButtonItem(iPosition, SubMenu.sPagesTexture, MenuTextButtonItem.DEFAULT_UV_OFFSET, MenuTextButtonItem.DEFAULT_SIZE, SubMenuOnline.LOC_JOIN, font2, 260f, 260f, TextAlign.Right));
    this.mMenuItems.Add((Magicka.GameLogic.GameStates.Menu.MenuItem) new MenuImageTextItem(SubMenu.BACK_POSITION, SubMenu.sPagesTexture, SubMenu.BACK_UVOFFSET, SubMenu.BACK_UVSCALE, 0, new Vector2(), SubMenu.BACK_TEXT_ALIGN, FontManager.Instance.GetFont(SubMenu.BACK_FONT), SubMenu.BACK_SIZE));
    this.mFilteredServerList = new FilteredServerList();
    this.mSelectedServer = -1;
    this.mServers = this.mFilteredServerList.GetVisibleServers(GlobalSettings.Instance.Filter);
    ManualConnectMessageBox.Instance.Complete += new Action<IPAddress, string>(this.ManualConnectCallback);
    this.mFilterPanel = new FilterPanel();
    this.mFilterPanel.CreateFilterPanel();
    this.mFilterPanel.DoLayout(SubMenuOnline.LIST_BG_POSITION, SubMenuOnline.LIST_BG_SIZE);
    this.mFilterPanel.FilterDataChanged += new Action<FilterData>(this.OnFilterDataChanged);
    this.mFilterPanel.RefreshRequest += new Action<FilterData>(this.OnRefreshRequest);
    this.mMenuItems.Add((Magicka.GameLogic.GameStates.Menu.MenuItem) this.mFilterPanel);
    this.mInputHelper = new DirectionalKeyboardHelper();
    this.mInputHelper.DownPressed += new Action<Controller>(((SubMenu) this).ControllerDown);
    this.mInputHelper.UpPressed += new Action<Controller>(((SubMenu) this).ControllerUp);
    this.mInputHelper.RightPressed += new Action<Controller>(((SubMenu) this).ControllerRight);
    this.mInputHelper.LeftPressed += new Action<Controller>(((SubMenu) this).ControllerLeft);
    for (int index = 0; index < this.mTitleItems.Count; ++index)
      this.mMenuItems.Add((Magicka.GameLogic.GameStates.Menu.MenuItem) this.mTitleItems[index]);
    this.mSelectedPosition = 12;
  }

  private void OnFilterDataChanged(FilterData iFilter)
  {
    GlobalSettings.Instance.Filter = iFilter;
    this.mFilteredServerList.FilterList(iFilter);
  }

  private void OnRefreshRequest(FilterData iFilter)
  {
    GlobalSettings.Instance.Filter = iFilter;
    this.RefreshServerList();
  }

  internal void SortServerList(FilteredServerList.SortType iType)
  {
    this.mFilteredServerList.SortList(iType);
  }

  internal void RefreshServerList()
  {
    this.mSelectedServer = -1;
    this.mServerScroll.Value = 0;
    this.mServerTop = 0;
    for (int index = 0; index < 12; ++index)
      this.mMenuItems[index].Enabled = false;
    lock (this.mServers)
      this.mServers = this.mFilteredServerList.GetVisibleServers(GlobalSettings.Instance.Filter);
  }

  public override void LanguageChanged()
  {
    this.mTabTexts[0].SetText(LanguageManager.Instance.GetString("#menu_main_01".GetHashCodeCustom()));
    this.mTabTexts[1].SetText(LanguageManager.Instance.GetString("#menu_main_02".GetHashCodeCustom()));
    this.mTabTexts[2].SetText(LanguageManager.Instance.GetString("#menu_main_03".GetHashCodeCustom()));
    for (int index = 12; index < this.mMenuItems.Count; ++index)
      this.mMenuItems[index].LanguageChanged();
    this.mMenuTitle.SetText(LanguageManager.Instance.GetString(SubMenu.LOC_ONLINE_PLAY));
    this.UpdateServerList();
  }

  internal void OnAbortConnectToServer() => NetworkManager.Instance.EndSession();

  internal void OnAbortedManualConnect() => NetworkManager.Instance.EndSession();

  internal void OnAbortedQuickStart() => NetworkManager.Instance.EndSession();

  internal void OnRequestedLobbyList(LobbyMatchList iList)
  {
    if (NetworkManager.Instance.ConnectionStatus != ConnectionStatus.NotConnected)
      return;
    if (iList.mLobbiesMatching == 0)
    {
      WaitingMessageBox.Instance.Kill();
      NetworkHostMessageBox.Instance.Show(new NetworkHostMessageBox.Complete(this.OnHostGame));
    }
    else
    {
      SteamID lobbyByIndex = SteamMatchmaking.GetLobbyByIndex(1);
      uint punGameServerIP = 0;
      ushort punGameServerPort = 0;
      SteamID psteamIDGameServer = new SteamID();
      if (SteamMatchmaking.GetLobbyGameServer(lobbyByIndex, ref punGameServerIP, ref punGameServerPort, ref psteamIDGameServer))
      {
        WaitingMessageBox.Instance.Show(SubMenuOnline.LOC_CONNECTING);
        WaitingMessageBox.Instance.OnAbort += new Action(this.OnAbortConnectToServer);
        NetworkManager.Instance.ConnectToServer(psteamIDGameServer, (string) null, new Action<ConnectionStatus>(this.OnConnectToServer));
      }
      else
      {
        WaitingMessageBox.Instance.Kill();
        NetworkHostMessageBox.Instance.Show(new NetworkHostMessageBox.Complete(this.OnHostGame));
      }
    }
  }

  internal void OnRequestedLobbyList_QuickJoin(LobbyMatchList iList)
  {
    if (NetworkManager.Instance.ConnectionStatus != ConnectionStatus.NotConnected)
      return;
    if (iList.mLobbiesMatching == 0)
    {
      if (SubMenuOnline._quickJoinSearchDistance == LobbyDistanceFilter.Worldwide)
      {
        WaitingMessageBox.Instance.Kill();
        SubMenuOnline._quickJoinSearchDistance = LobbyDistanceFilter.Default;
        SteamMatchmaking.AddRequestLobbyListDistanceFilter(LobbyDistanceFilter.Default);
        NetworkHostMessageBox.Instance.Show(new NetworkHostMessageBox.Complete(this.OnHostGame));
      }
      else
      {
        ++SubMenuOnline._quickJoinSearchDistance;
        SteamMatchmaking.AddRequestLobbyListDistanceFilter(SubMenuOnline._quickJoinSearchDistance);
        SteamMatchmaking.AddRequestLobbyListNumericalFilter("slots", 1, LobbyComparison.EqualToOrGreaterThan);
        SteamMatchmaking.AddRequestLobbyListNumericalFilter("playing", 0, LobbyComparison.Equal);
        SteamMatchmaking.AddRequestLobbyListNumericalFilter("password", 0, LobbyComparison.Equal);
        SteamMatchmaking.AddRequestLobbyListStringFilter("version", Application.ProductVersion, LobbyComparison.Equal);
        SteamMatchmaking.AddRequestLobbyListResultCountFilter(10);
        SteamMatchmaking.RequestLobbyList(new Action<LobbyMatchList>(this.OnRequestedLobbyList_QuickJoin));
      }
    }
    else
    {
      SteamID lobbyByIndex = SteamMatchmaking.GetLobbyByIndex(1);
      uint punGameServerIP = 0;
      ushort punGameServerPort = 0;
      SteamID psteamIDGameServer = new SteamID();
      if (SteamMatchmaking.GetLobbyGameServer(lobbyByIndex, ref punGameServerIP, ref punGameServerPort, ref psteamIDGameServer))
      {
        WaitingMessageBox.Instance.Show(SubMenuOnline.LOC_CONNECTING);
        WaitingMessageBox.Instance.OnAbort += new Action(this.OnAbortConnectToServer);
        NetworkManager.Instance.ConnectToServer(psteamIDGameServer, (string) null, new Action<ConnectionStatus>(this.OnConnectToServer));
      }
      else
      {
        WaitingMessageBox.Instance.Kill();
        NetworkHostMessageBox.Instance.Show(new NetworkHostMessageBox.Complete(this.OnHostGame));
      }
    }
  }

  private void OnHostGame(
    GameType iGameType,
    NetworkHostMessageBox.StoryChallengeType iStoryChallangeType,
    string iName,
    bool iVAC,
    string iPassword)
  {
    NetworkManager.Instance.SetupHost(iGameType, iName, iVAC, iPassword);
    switch (iGameType)
    {
      case GameType.Campaign:
        SubMenuCampaignSelect_SaveSlotSelect.Instance.mComplete += new Action(this.OnCampaignSelect);
        SubMenuCampaignSelect_SaveSlotSelect.Instance.GameType = GameType.Campaign;
        Tome.Instance.PushMenu((SubMenu) SubMenuCampaignSelect_SaveSlotSelect.Instance, 1);
        break;
      case GameType.Challenge:
        this.QuickStartCharSelect_Challange();
        break;
      case GameType.Versus:
        this.QuickStartCharSelect_Versus();
        break;
      case GameType.Mythos:
        SubMenuCampaignSelect_SaveSlotSelect.Instance.mComplete += new Action(this.OnCampaignSelect);
        SubMenuCampaignSelect_SaveSlotSelect.Instance.GameType = GameType.Mythos;
        Tome.Instance.PushMenu((SubMenu) SubMenuCampaignSelect_SaveSlotSelect.Instance, 1);
        break;
      case GameType.StoryChallange:
        switch (iStoryChallangeType)
        {
          case NetworkHostMessageBox.StoryChallengeType.Vietnam:
            this.QuickStartCharSelect_StoryChallange("ch_vietnam");
            return;
          case NetworkHostMessageBox.StoryChallengeType.OSOTC:
            this.QuickStartCharSelect_StoryChallange("ch_osotc");
            return;
          case NetworkHostMessageBox.StoryChallengeType.DUNG1:
            this.QuickStartCharSelect_StoryChallange("ch_dungeons_ch1");
            return;
          case NetworkHostMessageBox.StoryChallengeType.DUNG2:
            this.QuickStartCharSelect_StoryChallange("ch_dungeons_ch2");
            return;
          default:
            this.QuickStartCharSelect_StoryChallange((string) null);
            return;
        }
      default:
        throw new Exception("Invalid GameType!");
    }
  }

  private void ManualConnectCallback(IPAddress iAddress, string iPassword)
  {
    byte[] addressBytes = iAddress.GetAddressBytes();
    NetworkManager.Instance.ConnectToServer((uint) ((int) addressBytes[0] << 24 | (int) addressBytes[1] << 16 /*0x10*/ | (int) addressBytes[2] << 8) | (uint) addressBytes[3], iPassword, new Action<ConnectionStatus>(this.OnConnectToServer));
    WaitingMessageBox.Instance.Show(SubMenuOnline.LOC_CONNECTING);
    WaitingMessageBox.Instance.OnAbort += new Action(this.OnAbortedManualConnect);
  }

  private void OnConnectToServer(ConnectionStatus iStatus)
  {
    WaitingMessageBox.Instance.Kill();
    if (NetworkManager.Instance.State == NetworkState.Offline)
      return;
    switch (iStatus)
    {
      case ConnectionStatus.NotConnected:
        break;
      case ConnectionStatus.Connecting:
        break;
      case ConnectionStatus.Authenticating:
        break;
      case ConnectionStatus.Connected:
        Tome.Instance.PushMenu((SubMenu) SubMenuCharacterSelect.Instance, 1);
        break;
      case ConnectionStatus.Failed_GameFull:
        ErrorMessageBox.Instance.Show(LanguageManager.Instance.GetString(SubMenuOnline.LOC_ERROR_SERVERFULL));
        break;
      case ConnectionStatus.Failed_Authentication:
        ErrorMessageBox.Instance.Show(LanguageManager.Instance.GetString(SubMenuOnline.LOC_ERROR_AUTHFAIL));
        break;
      case ConnectionStatus.Failed_GamePlaying:
        ErrorMessageBox.Instance.Show(LanguageManager.Instance.GetString(SubMenuOnline.LOC_ERROR_INPROGRESS));
        break;
      case ConnectionStatus.Failed_Version:
        ErrorMessageBox.Instance.Show(LanguageManager.Instance.GetString(SubMenuOnline.LOC_ERROR_VERSION));
        break;
      case ConnectionStatus.Failed_Password:
        ErrorMessageBox.Instance.Show(LanguageManager.Instance.GetString(SubMenuOnline.LOC_ERROR_PASSWORD));
        break;
      case ConnectionStatus.Failed_Timeout:
        ErrorMessageBox.Instance.Show(LanguageManager.Instance.GetString(SubMenuOnline.LOC_ERROR_TIMEOUT));
        break;
      default:
        ErrorMessageBox.Instance.Show(LanguageManager.Instance.GetString(SubMenuOnline.LOC_ERROR_UNKNOWN));
        break;
    }
  }

  private void OnCampaignSelect()
  {
    if (NetworkManager.Instance.HasHostSettings)
    {
      string fileName = LevelManager.Instance.GetLevel(SubMenuCampaignSelect_SaveSlotSelect.Instance.GameType, (int) SubMenuCampaignSelect_SaveSlotSelect.Instance.CurrentSaveData.Level).FileName;
      NetworkManager.Instance.SetGame(GameType.Campaign, fileName);
      NetworkManager.Instance.StartHost();
      NetworkManager.Instance.SetGame(GameType.Campaign, fileName);
    }
    SubMenuCampaignSelect_SaveSlotSelect.Instance.mComplete -= new Action(this.OnCampaignSelect);
  }

  private void QuickStartCharSelect_Versus()
  {
    if (NetworkManager.Instance.HasHostSettings)
    {
      string iLevelName = SubMenuOnline.LOC_NO_LEVEL_SELECTED.ToString();
      NetworkManager.Instance.SetGame(GameType.Versus, iLevelName);
      NetworkManager.Instance.StartHost();
      NetworkManager.Instance.SetGame(GameType.Versus, iLevelName);
    }
    SubMenuCharacterSelect.Instance.SetSettings(GameType.Versus, -1, false);
    Tome.Instance.PushMenu((SubMenu) SubMenuCharacterSelect.Instance, 1);
  }

  private void QuickStartCharSelect_Challange()
  {
    if (NetworkManager.Instance.HasHostSettings)
    {
      string iLevelName = SubMenuOnline.LOC_NO_LEVEL_SELECTED.ToString();
      NetworkManager.Instance.SetGame(GameType.Challenge, iLevelName);
      NetworkManager.Instance.StartHost();
      NetworkManager.Instance.SetGame(GameType.Challenge, iLevelName);
    }
    SubMenuCharacterSelect.Instance.SetSettings(GameType.Challenge, -1, false);
    Tome.Instance.PushMenu((SubMenu) SubMenuCharacterSelect.Instance, 1);
  }

  private void QuickStartCharSelect_StoryChallange(string specificLevel)
  {
    if (NetworkManager.Instance.HasHostSettings)
    {
      string iLevelName = !string.IsNullOrEmpty(specificLevel) ? LevelManager.GetLocalizedName(specificLevel) : SubMenuOnline.LOC_NO_LEVEL_SELECTED.ToString();
      NetworkManager.Instance.SetGame(GameType.Campaign, iLevelName);
      NetworkManager.Instance.StartHost();
      NetworkManager.Instance.SetGame(GameType.Campaign, iLevelName);
    }
    if (string.IsNullOrEmpty(specificLevel))
      SubMenuCharacterSelect.Instance.SetSettings(GameType.StoryChallange, -1, false);
    else
      SubMenuCharacterSelect.Instance.SetSettings(GameType.StoryChallange, specificLevel, false);
    Tome.Instance.PushMenu((SubMenu) SubMenuCharacterSelect.Instance, 1);
  }

  public override void Draw(Viewport iLeftSide, Viewport iRightSide)
  {
    this.mEffect.GraphicsDevice.Viewport = iRightSide;
    this.mEffect.Begin();
    this.mEffect.CurrentTechnique.Passes[0].Begin();
    this.mEffect.VertexColorEnabled = false;
    this.mEffect.Color = Defines.MESSAGEBOX_COLOR_DEFAULT;
    if (this.mMenuTitle != null)
      this.mMenuTitle.Draw(this.mEffect, 512f, 96f);
    this.mEffect.GraphicsDevice.Vertices[0].SetSource(SubMenuOnline.sBackgroundVertexBuffer, 0, VertexPositionColor.SizeInBytes);
    this.mEffect.GraphicsDevice.VertexDeclaration = SubMenuOnline.sBackgroundVertexDeclaration;
    this.mEffect.TextureEnabled = false;
    this.mEffect.VertexColorEnabled = true;
    this.mEffect.Saturation = 1f;
    this.mEffect.Color = new Vector4(1f, 1f, 1f, 0.5f);
    Matrix identity = Matrix.Identity with
    {
      M11 = SubMenuOnline.LIST_BG_SIZE.X,
      M22 = SubMenuOnline.LIST_BG_SIZE.Y,
      M41 = SubMenuOnline.LIST_BG_POSITION.X,
      M42 = SubMenuOnline.LIST_BG_POSITION.Y
    };
    this.mEffect.Transform = identity;
    this.mEffect.CommitChanges();
    this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
    this.mEffect.GraphicsDevice.Vertices[0].SetSource(SubMenuOnline.sVertexBuffer, 0, VertexPositionTexture.SizeInBytes);
    this.mEffect.GraphicsDevice.VertexDeclaration = SubMenuOnline.sVertexDeclaration;
    this.mEffect.TextureEnabled = true;
    this.mEffect.Texture = (Texture) SubMenu.sPagesTexture;
    this.mEffect.Color = Vector4.One;
    this.mEffect.Saturation = 1f;
    this.mEffect.VertexColorEnabled = false;
    Vector2 vector2_1 = new Vector2();
    vector2_1.X = 0.0f / (float) SubMenu.sPagesTexture.Width;
    vector2_1.Y = 384f / (float) SubMenu.sPagesTexture.Height;
    Vector2 vector2_2 = new Vector2();
    vector2_2.X = 320f / (float) SubMenu.sPagesTexture.Width;
    vector2_2.Y = 80f / (float) SubMenu.sPagesTexture.Height;
    this.mEffect.TextureOffset = vector2_1;
    this.mEffect.TextureScale = vector2_2;
    identity.M11 = 320f;
    identity.M22 = 80f;
    identity.M41 = SubMenuOnline.LIST_TOP_POSITION.X;
    identity.M42 = SubMenuOnline.LIST_TOP_POSITION.Y + 4f;
    for (int index = 0; index < 3; ++index)
    {
      if (index != this.mSelectedTab)
      {
        if (index == this.mHighlitTab)
        {
          this.mEffect.Saturation = 1.5f;
          this.mEffect.Color = new Vector4(1.25f, 1.25f, 1.25f, 1f);
        }
        else
        {
          this.mEffect.Saturation = 1f;
          this.mEffect.Color = Vector4.One;
        }
        this.mEffect.Transform = identity;
        this.mEffect.CommitChanges();
        this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
      }
      identity.M41 += 256f;
    }
    this.mEffect.Saturation = 1f;
    this.mEffect.Color = Magicka.GameLogic.GameStates.Menu.MenuItem.COLOR;
    Vector2 vector2_3 = new Vector2();
    vector2_3.X = (float) ((double) SubMenuOnline.LIST_TOP_POSITION.X + 128.0 + 32.0);
    vector2_3.Y = SubMenuOnline.LIST_TOP_POSITION.Y + 22f;
    for (int index = 0; index < 3; ++index)
    {
      if (index != this.mSelectedTab)
      {
        this.mEffect.Color = this.mHighlitTab == index ? Magicka.GameLogic.GameStates.Menu.MenuItem.COLOR_SELECTED : Magicka.GameLogic.GameStates.Menu.MenuItem.COLOR;
        this.mTabTexts[index].Draw(this.mEffect, vector2_3.X, vector2_3.Y + 4f);
      }
      vector2_3.X += 256f;
    }
    this.mEffect.GraphicsDevice.Vertices[0].SetSource(SubMenuOnline.sVertexBuffer, 0, VertexPositionTexture.SizeInBytes);
    this.mEffect.GraphicsDevice.VertexDeclaration = SubMenuOnline.sVertexDeclaration;
    this.mEffect.Texture = (Texture) SubMenu.sPagesTexture;
    this.mEffect.Color = Vector4.One;
    this.mEffect.TextureOffset = new Vector2(0.0f / (float) SubMenu.sPagesTexture.Width, 240f / (float) SubMenu.sPagesTexture.Height);
    this.mEffect.TextureScale = new Vector2(SubMenuOnline.LIST_TOP_SIZE.X / (float) SubMenu.sPagesTexture.Width, SubMenuOnline.LIST_TOP_SIZE.Y / (float) SubMenu.sPagesTexture.Height);
    identity.M11 = SubMenuOnline.LIST_TOP_SIZE.X;
    identity.M22 = SubMenuOnline.LIST_TOP_SIZE.Y;
    identity.M41 = SubMenuOnline.LIST_TOP_POSITION.X;
    identity.M42 = SubMenuOnline.LIST_TOP_POSITION.Y;
    this.mEffect.Transform = identity;
    this.mEffect.CommitChanges();
    this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
    this.mEffect.TextureOffset = new Vector2(0.0f / (float) SubMenu.sPagesTexture.Width, 384f / (float) SubMenu.sPagesTexture.Height);
    this.mEffect.TextureScale = new Vector2(320f / (float) SubMenu.sPagesTexture.Width, 80f / (float) SubMenu.sPagesTexture.Height);
    identity.M11 = 320f;
    identity.M22 = 80f;
    identity.M41 = SubMenuOnline.LIST_TOP_POSITION.X + 256f * (float) this.mSelectedTab;
    identity.M42 = SubMenuOnline.LIST_TOP_POSITION.Y;
    this.mEffect.Transform = identity;
    this.mEffect.CommitChanges();
    this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
    this.mEffect.Color = this.mHighlitTab != this.mSelectedTab || this.mSelectedPosition >= 0 ? Magicka.GameLogic.GameStates.Menu.MenuItem.COLOR : Magicka.GameLogic.GameStates.Menu.MenuItem.COLOR_SELECTED;
    this.mTabTexts[this.mSelectedTab].Draw(this.mEffect, (float) ((double) SubMenuOnline.LIST_TOP_POSITION.X + 128.0 + 32.0 + 256.0 * (double) this.mSelectedTab), vector2_3.Y);
    this.mEffect.GraphicsDevice.Vertices[0].SetSource((VertexBuffer) null, 0, 0);
    for (int index = 12; index < this.mMenuItems.Count; ++index)
      this.mMenuItems[index].Draw(this.mEffect);
    lock (this.mServers)
    {
      this.UpdateServerList();
      for (int index = 0; index < 12; ++index)
      {
        if (this.mMenuItems[index].Enabled && this.mServerTop + index < this.mServers.Count)
        {
          if (this.mServersSelected)
            this.mMenuItems[index].Selected = true;
          GameServerItem mServer = this.mServers[this.mServerTop + index];
          this.mMenuItems[index].Enabled = !mServer.Playing();
          this.mMenuItems[index].Draw(this.mEffect);
          if (mServer.m_Password)
          {
            this.mEffect.GraphicsDevice.Vertices[0].SetSource(SubMenuOnline.sVertexBuffer, 0, VertexPositionTexture.SizeInBytes);
            this.mEffect.GraphicsDevice.VertexDeclaration = SubMenuOnline.sVertexDeclaration;
            this.mEffect.TextureEnabled = true;
            this.mEffect.Texture = (Texture) SubMenu.sPagesTexture;
            this.mEffect.Color = Vector4.One;
            this.mEffect.Saturation = 1f;
            this.mEffect.VertexColorEnabled = false;
            this.mEffect.TextureOffset = new Vector2(1056f / (float) SubMenu.sPagesTexture.Width, 128f / (float) SubMenu.sPagesTexture.Height);
            this.mEffect.TextureScale = new Vector2(32f / (float) SubMenu.sPagesTexture.Width, 32f / (float) SubMenu.sPagesTexture.Height);
            identity.M11 = 32f;
            identity.M22 = 32f;
            identity.M41 = this.mMenuItems[index].Position.X - 32f;
            identity.M42 = this.mMenuItems[index].Position.Y;
            this.mEffect.Transform = identity;
            this.mEffect.CommitChanges();
            this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
            this.mEffect.TextureScale = new Vector2(1f, 1f);
            this.mEffect.TextureOffset = Vector2.Zero;
          }
          if (mServer.m_Secure)
          {
            this.mEffect.GraphicsDevice.Vertices[0].SetSource(SubMenuOnline.sVertexBuffer, 0, VertexPositionTexture.SizeInBytes);
            this.mEffect.GraphicsDevice.VertexDeclaration = SubMenuOnline.sVertexDeclaration;
            this.mEffect.TextureEnabled = true;
            this.mEffect.Texture = (Texture) SubMenu.sPagesTexture;
            this.mEffect.Color = Vector4.One;
            this.mEffect.Saturation = 1f;
            this.mEffect.VertexColorEnabled = false;
            this.mEffect.TextureOffset = new Vector2(1024f / (float) SubMenu.sPagesTexture.Width, 128f / (float) SubMenu.sPagesTexture.Height);
            this.mEffect.TextureScale = new Vector2(32f / (float) SubMenu.sPagesTexture.Width, 32f / (float) SubMenu.sPagesTexture.Height);
            identity.M11 = 32f;
            identity.M22 = 32f;
            identity.M41 = this.mMenuItems[index].Position.X - 64f;
            identity.M42 = this.mMenuItems[index].Position.Y;
            this.mEffect.Transform = identity;
            this.mEffect.CommitChanges();
            this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
            this.mEffect.TextureScale = new Vector2(1f, 1f);
            this.mEffect.TextureOffset = Vector2.Zero;
          }
        }
      }
    }
    this.mServerScroll.Draw(this.mEffect);
    this.mEffect.CurrentTechnique.Passes[0].End();
    this.mEffect.End();
  }

  private void OnPasswordInput(string iPassword)
  {
    if (this.mSelectedServer < 0 || string.IsNullOrEmpty(iPassword))
      return;
    WaitingMessageBox.Instance.Show(SubMenuOnline.LOC_CONNECTING);
    WaitingMessageBox.Instance.OnAbort += new Action(this.OnAbortConnectToServer);
    NetworkManager.Instance.ConnectToServer(this.mServers[this.mServerTop + this.mSelectedServer].ServerID, iPassword, new Action<ConnectionStatus>(this.OnConnectToServer));
  }

  public override void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    base.Update(iDataChannel, iDeltaTime);
    if (this.mServerTop != this.mServerScroll.Value)
      this.UpdateServerScroll();
    for (int index = 0; index < 12; ++index)
      this.mMenuItems[index].Selected = this.mSelectedServer == index | this.mSelectedPosition == index;
    this.mMenuItems[15].Enabled = this.mSelectedServer >= 0 & this.mSelectedServer < 12;
    this.mFilterPanel.Update(iDeltaTime);
  }

  private void UpdateServerScroll()
  {
    lock (this.mServers)
    {
      int iServer = this.mServerScroll.Value;
      for (int iMenuItem = 0; iMenuItem < 12; ++iMenuItem)
      {
        if (iServer < this.mServers.Count)
        {
          this.SetServerItem(iMenuItem, iServer);
          ++iServer;
        }
        else
          break;
      }
    }
    this.mSelectedServer = this.mSelectedPosition;
    this.mServerTop = this.mServerScroll.Value;
  }

  private void UpdateServerList()
  {
    int iServer = this.mServerScroll.Value;
    for (int index = 0; index < 12 && iServer < this.mServers.Count; ++index)
    {
      if (iServer < this.mServers.Count)
      {
        this.SetServerItem(index, iServer);
        this.mMenuItems[index].Enabled = true;
      }
      else if (iServer >= this.mServers.Count)
        this.mMenuItems[index].Enabled = false;
      ++iServer;
    }
    int num = this.mServerScroll.Value;
    this.mServerScroll.SetMaxValue(Math.Max(0, this.mServers.Count - 12));
    this.mServerScroll.Value = num;
  }

  private void SetServerItem(int iMenuItem, int iServer)
  {
    if (string.IsNullOrEmpty(this.mServers[iServer].m_Name))
    {
      (this.mMenuItems[iMenuItem] as MenuTextRowItem).SetItemText(0, "");
    }
    else
    {
      string iText = this.mServers[iServer].m_Name;
      BitmapFont font = FontManager.Instance.GetFont(MagickaFont.Maiandra18);
      for (float x = font.MeasureText(iText, true).X; (double) x > 265.0; x = font.MeasureText(iText, true).X)
        iText = iText.Substring(0, iText.Length - 1);
      (this.mMenuItems[iMenuItem] as MenuTextRowItem).SetItemText(0, iText);
    }
    if (string.IsNullOrEmpty(this.mServers[iServer].m_Map))
    {
      (this.mMenuItems[iMenuItem] as MenuTextRowItem).SetItemText(1, "");
    }
    else
    {
      string levelString = SubMenuOnline.GetLevelString(this.mServers[iServer].m_Map);
      (this.mMenuItems[iMenuItem] as MenuTextRowItem).SetItemText(1, levelString);
    }
    (this.mMenuItems[iMenuItem] as MenuTextRowItem).SetItemText(2, $"{this.mServers[iServer].m_Players}/{this.mServers[iServer].m_MaxPlayers}");
    (this.mMenuItems[iMenuItem] as MenuTextRowItem).SetItemText(3, this.mServers[iServer].m_Ping.ToString());
  }

  public override void ControllerMouseMove(
    Controller iSender,
    Point iScreenSize,
    MouseState iState,
    MouseState iOldState)
  {
    Vector2 oHitPosition;
    bool oRightPageHit;
    if (Tome.MousePickTome(iScreenSize, iState.X, iState.Y, out oHitPosition, out oRightPageHit))
    {
      if (oRightPageHit)
      {
        string iToolTip = (string) null;
        if (this.mFilterPanel.CheckMouseMove(ref oHitPosition, ref iToolTip))
        {
          this.mSelectedPosition = 17;
          this.SetToolTip(iToolTip, iState);
          return;
        }
        if (this.mServerScroll.Grabbed)
        {
          this.mServerScroll.ScrollTo(oHitPosition.Y);
        }
        else
        {
          int num = -1;
          if ((double) oHitPosition.Y >= (double) SubMenuOnline.LIST_TOP_POSITION.Y && (double) oHitPosition.Y <= (double) SubMenuOnline.LIST_TOP_POSITION.Y + 48.0)
          {
            for (int index = 0; index < 3; ++index)
            {
              if ((double) oHitPosition.X >= (double) SubMenuOnline.LIST_TOP_POSITION.X + 16.0 + (double) index * ((double) SubMenuOnline.LIST_TOP_SIZE.X - 32.0) / 3.0 && (double) oHitPosition.X <= (double) SubMenuOnline.LIST_TOP_POSITION.X + 16.0 + ((double) index + 1.0) * ((double) SubMenuOnline.LIST_TOP_SIZE.X - 32.0) / 3.0)
              {
                if (index != this.mSelectedTab)
                {
                  num = index;
                  break;
                }
                break;
              }
            }
          }
          this.mHighlitTab = num;
          bool flag = false;
          for (int index1 = 12; index1 < this.mMenuItems.Count; ++index1)
          {
            Magicka.GameLogic.GameStates.Menu.MenuItem mMenuItem = this.mMenuItems[index1];
            if (mMenuItem.Enabled && mMenuItem.InsideBounds(ref oHitPosition))
            {
              if (this.mSelectedPosition != index1)
                AudioManager.Instance.PlayCue(Banks.UI, "ui_menu_mouseover".GetHashCodeCustom());
              this.mSelectedPosition = index1;
              for (int index2 = 0; index2 < this.mMenuItems.Count; ++index2)
                this.mMenuItems[index2].Selected = false;
              this.mServersSelected = false;
              mMenuItem.Selected = true;
              flag = true;
              break;
            }
          }
          if (!flag && this.mSelectedPosition >= 12)
          {
            for (int index = 0; index < this.mMenuItems.Count; ++index)
              this.mMenuItems[index].Selected = false;
            this.mSelectedPosition = -1;
            this.mServersSelected = false;
          }
        }
      }
      this.SetToolTip((string) null, iState);
    }
    else
      ToolTipMan.Instance.Kill((Player) null, false);
  }

  private void SetToolTip(string iToolTip, MouseState iState)
  {
    this.SetToolTip(iToolTip, new Vector2((float) iState.X, (float) iState.Y + 32f));
  }

  private void SetToolTip(string iToolTip, Vector2 iPos)
  {
    if (!string.IsNullOrEmpty(iToolTip))
    {
      ToolTipMan.Instance.Set((Player) null, iToolTip, ref iPos);
    }
    else
    {
      switch (this.mSelectedPosition)
      {
        case 12:
          ToolTipMan.Instance.Set((Player) null, LanguageManager.Instance.GetString(SubMenuOnline.LOC_TT_QUICK), ref iPos);
          break;
        case 13:
          ToolTipMan.Instance.Set((Player) null, LanguageManager.Instance.GetString(SubMenuOnline.LOC_TT_HOST), ref iPos);
          break;
        case 14:
          ToolTipMan.Instance.Set((Player) null, LanguageManager.Instance.GetString(SubMenuOnline.LOC_TT_IP), ref iPos);
          break;
        case 15:
          ToolTipMan.Instance.Set((Player) null, LanguageManager.Instance.GetString(SubMenuOnline.LOC_TT_JOIN), ref iPos);
          break;
        case 17:
          string toolTip = this.mFilterPanel.GetToolTip(out iPos);
          Vector2 oScreenPos;
          Tome.Instance.PageToScreen(true, ref iPos, out oScreenPos);
          oScreenPos.Y += 32f;
          if (toolTip == null)
            break;
          ToolTipMan.Instance.Set((Player) null, toolTip, ref oScreenPos);
          break;
        case 18:
          ToolTipMan.Instance.Set((Player) null, LanguageManager.Instance.GetString(SubMenuOnline.LOC_TT_NAME), ref iPos);
          break;
        case 19:
          ToolTipMan.Instance.Set((Player) null, LanguageManager.Instance.GetString(SubMenuOnline.LOC_TT_LEVEL), ref iPos);
          break;
        case 20:
          ToolTipMan.Instance.Set((Player) null, LanguageManager.Instance.GetString(SubMenuOnline.LOC_TT_PLAYERS), ref iPos);
          break;
        case 21:
          ToolTipMan.Instance.Set((Player) null, LanguageManager.Instance.GetString(SubMenuOnline.LOC_TT_PING), ref iPos);
          break;
        default:
          ToolTipMan.Instance.Kill((Player) null, false);
          break;
      }
    }
  }

  public override void ControllerEvent(
    Controller iSender,
    KeyboardState iOldState,
    KeyboardState iNewState)
  {
    this.mInputHelper.Update(iSender, iOldState, iNewState);
  }

  private void SetControllerToolTip()
  {
    if (this.mSelectedPosition < 12 || this.mSelectedPosition >= this.mMenuItems.Count)
      return;
    Vector2 topLeft = this.mMenuItems[this.mSelectedPosition].TopLeft;
    Vector2 oScreenPos;
    Tome.Instance.PageToScreen(true, ref topLeft, out oScreenPos);
    oScreenPos.Y += 32f;
    this.SetToolTip((string) null, oScreenPos);
  }

  public override void ControllerUp(Controller iSender)
  {
    ToolTipMan.Instance.Kill((Player) null, false);
    if (this.mServersSelected)
    {
      this.mServersSelected = false;
      this.mSelectedPosition = 18;
      this.mHighlitTab = this.mSelectedTab;
    }
    else
    {
      switch (this.mSelectedPosition)
      {
        case 12:
          this.mSelectedPosition = 17;
          this.mFilterPanel.ControllEnter(3);
          break;
        case 13:
          this.mSelectedPosition = 17;
          this.mFilterPanel.ControllEnter(1);
          break;
        case 14:
          this.mSelectedPosition = 17;
          this.mFilterPanel.ControllEnter(5);
          break;
        case 15:
          this.mSelectedPosition = 14;
          break;
        case 16 /*0x10*/:
          this.mSelectedPosition = 13;
          break;
        case 17:
          if (this.mFilterPanel.ControllUpOver())
          {
            if (this.mServers.Count > 0)
            {
              this.mServersSelected = true;
              this.mSelectedPosition = -1;
              break;
            }
            this.mSelectedPosition = 18;
            this.mHighlitTab = this.mSelectedTab;
            break;
          }
          break;
        default:
          if (this.mHighlitTab >= 0)
          {
            int num = this.mSelectedPosition - 18;
            this.mSelectedPosition = -1;
            if (num != this.mSelectedTab)
            {
              this.mHighlitTab = (int) MathHelper.Clamp((float) num, 0.0f, 2f);
              break;
            }
            break;
          }
          if (this.mSelectedPosition == 0 && this.mServerScroll.Value > 0)
          {
            int num = Math.Max(6, 6 - this.mServerScroll.Value);
            this.mSelectedPosition += num;
            this.mServerScroll.Value -= num;
            break;
          }
          if (this.mSelectedPosition != 0 || this.mServerScroll.Value != 0)
          {
            base.ControllerUp(iSender);
            this.mSelectedServer = this.mSelectedPosition;
            break;
          }
          break;
      }
    }
    this.SetControllerToolTip();
  }

  public override void ControllerDown(Controller iSender)
  {
    ToolTipMan.Instance.Kill((Player) null, false);
    if (this.mServersSelected)
    {
      this.mServersSelected = false;
      this.mSelectedPosition = 17;
      this.mFilterPanel.ControllEnter(0);
      this.mHighlitTab = -1;
    }
    else
    {
      switch (this.mSelectedPosition)
      {
        case 12:
          if (this.mMenuItems[15].Enabled)
          {
            this.mSelectedPosition = 15;
            break;
          }
          this.mSelectedPosition = 16 /*0x10*/;
          break;
        case 13:
          this.mSelectedPosition = 16 /*0x10*/;
          break;
        case 14:
          if (this.mMenuItems[15].Enabled)
          {
            this.mSelectedPosition = 15;
            break;
          }
          this.mSelectedPosition = 16 /*0x10*/;
          break;
        case 15:
        case 16 /*0x10*/:
          break;
        case 17:
          switch (this.mFilterPanel.ControllDownOver())
          {
            case 1:
              this.mSelectedPosition = 13;
              break;
            case 3:
              this.mSelectedPosition = 12;
              break;
            case 4:
            case 5:
              this.mSelectedPosition = 14;
              break;
          }
          break;
        case 18:
        case 19:
        case 20:
        case 21:
          if (this.mServers.Count > 0)
          {
            this.mServersSelected = true;
            this.mSelectedPosition = -1;
          }
          else
          {
            this.mSelectedPosition = 17;
            this.mFilterPanel.ControllEnter(0);
          }
          this.mHighlitTab = -1;
          break;
        default:
          if (this.mHighlitTab >= 0)
          {
            this.mSelectedPosition = 18 + this.mHighlitTab;
            this.mHighlitTab = this.mSelectedTab;
            break;
          }
          if (this.mSelectedPosition >= 11)
          {
            if (this.mServers.Count - (this.mServerScroll.Value + this.mSelectedPosition + 1) != 0)
            {
              int num = Math.Min(6, this.mServers.Count - (this.mServerScroll.Value + this.mSelectedPosition + 1));
              this.mSelectedPosition -= num;
              this.mSelectedServer = this.mSelectedPosition;
              this.mServerScroll.Value += num;
              break;
            }
            break;
          }
          if (this.mSelectedPosition < this.mServers.Count - 1)
          {
            base.ControllerDown(iSender);
            this.mSelectedServer = this.mSelectedPosition;
            break;
          }
          break;
      }
    }
    this.SetControllerToolTip();
  }

  public override void ControllerLeft(Controller iSender)
  {
    ToolTipMan.Instance.Kill((Player) null, false);
    switch (this.mSelectedPosition)
    {
      case 12:
        this.mSelectedPosition = 13;
        goto case 13;
      case 13:
      case 16 /*0x10*/:
      case 18:
        this.SetControllerToolTip();
        break;
      case 14:
        this.mSelectedPosition = 12;
        goto case 13;
      case 15:
        this.mSelectedPosition = 16 /*0x10*/;
        goto case 13;
      case 17:
        this.mFilterPanel.ControllLeftOver();
        goto case 13;
      case 19:
      case 20:
      case 21:
        --this.mSelectedPosition;
        goto case 13;
      default:
        if (this.mServersSelected)
        {
          if (this.mSelectedTab > 0)
          {
            this.SwitchToTab(this.mSelectedTab - 1);
            goto case 13;
          }
          goto case 13;
        }
        if (this.mSelectedPosition == -1)
        {
          if (this.mHighlitTab > 0)
          {
            --this.mHighlitTab;
            goto case 13;
          }
          goto case 13;
        }
        if (this.mSelectedTab > 0)
        {
          this.SwitchToTab(this.mSelectedTab - 1);
          this.mSelectedServer = this.mSelectedPosition = 0;
          goto case 13;
        }
        goto case 13;
    }
  }

  public override void ControllerRight(Controller iSender)
  {
    ToolTipMan.Instance.Kill((Player) null, false);
    switch (this.mSelectedPosition)
    {
      case 12:
        this.mSelectedPosition = 14;
        goto case 14;
      case 13:
        this.mSelectedPosition = 12;
        goto case 14;
      case 14:
      case 15:
      case 21:
        this.SetControllerToolTip();
        break;
      case 16 /*0x10*/:
        if (this.mMenuItems[15].Enabled)
        {
          this.mSelectedPosition = 15;
          goto case 14;
        }
        goto case 14;
      case 17:
        this.mFilterPanel.ControllRightOver();
        goto case 14;
      case 18:
      case 19:
      case 20:
        ++this.mSelectedPosition;
        goto case 14;
      default:
        if (this.mServersSelected)
        {
          if (this.mSelectedTab < 2)
          {
            this.SwitchToTab(this.mSelectedTab + 1);
            goto case 14;
          }
          goto case 14;
        }
        if (this.mSelectedPosition == -1)
        {
          if (this.mHighlitTab < 2)
          {
            ++this.mHighlitTab;
            goto case 14;
          }
          goto case 14;
        }
        if (this.mSelectedTab < 2)
        {
          this.SwitchToTab(this.mSelectedTab + 1);
          this.mSelectedServer = this.mSelectedPosition = 0;
          goto case 14;
        }
        goto case 14;
    }
  }

  public override void ControllerMouseAction(
    Controller iSender,
    Point iScreenSize,
    MouseState iState,
    MouseState iOldState)
  {
    Vector2 oHitPosition;
    bool oRightPageHit;
    if (!Tome.MousePickTome(iScreenSize, iState.X, iState.Y, out oHitPosition, out oRightPageHit) || !oRightPageHit)
      return;
    ToolTipMan.Instance.Kill((Player) null, false);
    this.mServerScroll.Grabbed = false;
    if (iState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
    {
      if (!this.mServerScroll.InsideBounds(oHitPosition.X, oHitPosition.Y))
        return;
      if (this.mServerScroll.InsideDragBounds(oHitPosition))
        this.mServerScroll.Grabbed = true;
      else if (this.mServerScroll.InsideDragUpBounds(oHitPosition))
        --this.mServerScroll.Value;
      else if (this.mServerScroll.InsideDragDownBounds(oHitPosition))
        ++this.mServerScroll.Value;
      else
        this.mServerScroll.ScrollTo(oHitPosition.Y);
    }
    else
    {
      if (iState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released && iOldState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed && this.mFilterPanel.CheckMouseAction(ref oHitPosition))
        return;
      for (int index = 0; index < this.mTitleItems.Count; ++index)
      {
        if (this.mTitleItems[index].InsideBounds(oHitPosition.X, oHitPosition.Y))
        {
          this.SortServers(6 + index);
          return;
        }
      }
      if (this.mServerScroll.InsideBounds(oHitPosition.X, oHitPosition.Y))
      {
        if (iState.ScrollWheelValue == iOldState.ScrollWheelValue)
          return;
        if (this.mServerScroll.Value > 0 && iState.ScrollWheelValue > iOldState.ScrollWheelValue)
        {
          --this.mServerScroll.Value;
        }
        else
        {
          if (this.mServerScroll.Value >= this.mServers.Count || iState.ScrollWheelValue >= iOldState.ScrollWheelValue)
            return;
          ++this.mServerScroll.Value;
        }
      }
      else if ((double) oHitPosition.Y >= (double) SubMenuOnline.LIST_TOP_POSITION.Y && (double) oHitPosition.Y <= (double) SubMenuOnline.LIST_TOP_POSITION.Y + 48.0)
      {
        for (int i = 0; i < 3; ++i)
        {
          if ((double) oHitPosition.X >= (double) SubMenuOnline.LIST_TOP_POSITION.X + 16.0 + (double) i * ((double) SubMenuOnline.LIST_TOP_SIZE.X - 32.0) / 3.0 && (double) oHitPosition.X <= (double) SubMenuOnline.LIST_TOP_POSITION.X + 16.0 + ((double) i + 1.0) * ((double) SubMenuOnline.LIST_TOP_SIZE.X - 32.0) / 3.0)
          {
            this.SwitchToTab(i);
            break;
          }
        }
      }
      else
      {
        bool flag = false;
        for (int index1 = 0; index1 < this.mMenuItems.Count; ++index1)
        {
          Magicka.GameLogic.GameStates.Menu.MenuItem mMenuItem = this.mMenuItems[index1];
          if (mMenuItem.Enabled && mMenuItem.InsideBounds(ref oHitPosition))
          {
            if (index1 < 12 && iState.ScrollWheelValue != iOldState.ScrollWheelValue)
            {
              if (this.mServerScroll.Value > 0 && iState.ScrollWheelValue > iOldState.ScrollWheelValue)
              {
                --this.mServerScroll.Value;
                break;
              }
              if (this.mServerScroll.Value < this.mServers.Count && iState.ScrollWheelValue < iOldState.ScrollWheelValue)
              {
                ++this.mServerScroll.Value;
                break;
              }
              break;
            }
            if (iState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released && iOldState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed || iState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Released && iOldState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
            {
              for (int index2 = 0; index2 < this.mMenuItems.Count; ++index2)
                this.mMenuItems[index2].Selected = false;
              flag = true;
              if (index1 < 12)
              {
                if (this.mSelectedPosition == index1)
                  this.ControllerMouseClicked(iSender);
                else
                  this.mSelectedPosition = index1;
                this.mSelectedServer = index1;
                break;
              }
              this.mSelectedPosition = index1;
              this.ControllerMouseClicked(iSender);
              break;
            }
            break;
          }
        }
        if (flag)
          return;
        this.mSelectedPosition = -1;
        this.mSelectedServer = -1;
      }
    }
  }

  private void SortServers(int iSelectedIndex)
  {
    switch (iSelectedIndex)
    {
      case 6:
        this.SortServerList(FilteredServerList.SortType.Name);
        break;
      case 7:
        this.SortServerList(FilteredServerList.SortType.Level);
        break;
      case 8:
        this.SortServerList(FilteredServerList.SortType.Players);
        break;
      case 9:
        this.SortServerList(FilteredServerList.SortType.Ping);
        break;
    }
  }

  private void SwitchToTab(int i)
  {
    this.mSelectedTab = i;
    GlobalSettings.Instance.Filter = GlobalSettings.Instance.Filter with
    {
      GameType = (GameType) (1 << i)
    };
    this.mSelectedPosition = -1;
    this.RefreshServerList();
  }

  public override void ControllerB(Controller iSender)
  {
    if (DaisyWheel.IsDisplaying)
      return;
    ToolTipMan.Instance.Kill((Player) null, false);
    if (this.mSelectedPosition == 17)
    {
      if (this.mFilterPanel.ControllB())
        return;
      Tome.Instance.PopMenu();
    }
    else if (this.mSelectedPosition >= 0 && this.mSelectedPosition < 12)
    {
      this.mServersSelected = true;
      this.mSelectedServer = this.mSelectedPosition = -1;
      GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Bottom, SubMenu.LOC_SELECT);
    }
    else
      Tome.Instance.PopMenu();
  }

  public override void ControllerA(Controller iSender)
  {
    ToolTipMan.Instance.Kill((Player) null, false);
    if (this.mServersSelected)
    {
      GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Bottom, SubMenuOnline.LOC_JOIN);
      this.mServersSelected = false;
      this.mSelectedPosition = 0;
      for (int index = 1; index < 12; ++index)
      {
        if (this.mMenuItems[index].Enabled && this.mServerTop + index < this.mServers.Count)
          this.mMenuItems[index].Selected = false;
      }
    }
    else if (this.mHighlitTab >= 0)
    {
      if (this.mHighlitTab == this.mSelectedTab)
      {
        if (this.mSelectedPosition < 0)
        {
          this.mHighlitTab = -1;
          this.mSelectedPosition = 0;
        }
        else
          this.SortServers(this.mSelectedPosition - 12);
      }
      else
        this.SwitchToTab(this.mHighlitTab);
    }
    else
    {
      switch (this.mSelectedPosition)
      {
        case 12:
          SubMenuOnline._quickJoinSearchDistance = LobbyDistanceFilter.Close;
          SteamMatchmaking.AddRequestLobbyListDistanceFilter(LobbyDistanceFilter.Close);
          SteamMatchmaking.AddRequestLobbyListNumericalFilter("slots", 1, LobbyComparison.EqualToOrGreaterThan);
          SteamMatchmaking.AddRequestLobbyListNumericalFilter("playing", 0, LobbyComparison.Equal);
          SteamMatchmaking.AddRequestLobbyListNumericalFilter("password", 0, LobbyComparison.Equal);
          SteamMatchmaking.AddRequestLobbyListStringFilter("version", Application.ProductVersion, LobbyComparison.Equal);
          SteamMatchmaking.AddRequestLobbyListResultCountFilter(10);
          SteamMatchmaking.RequestLobbyList(new Action<LobbyMatchList>(this.OnRequestedLobbyList_QuickJoin));
          WaitingMessageBox.Instance.Show(SubMenuOnline.LOC_SEARCHING);
          WaitingMessageBox.Instance.OnAbort += new Action(this.OnAbortedQuickStart);
          break;
        case 13:
          NetworkHostMessageBox.Instance.Show(new NetworkHostMessageBox.Complete(this.OnHostGame));
          break;
        case 14:
          ManualConnectMessageBox.Instance.Show();
          break;
        case 15:
          if (this.mSelectedServer < 0 || this.mSelectedServer >= 12)
            break;
          if (this.mServers[this.mServerTop + this.mSelectedServer].m_Password)
          {
            string upper = LanguageManager.Instance.GetString(SubMenuOnline.LOC_PASSWORD).ToUpper();
            this.mPasswordMessageBox.Show(new Action<string>(this.OnPasswordInput), iSender, upper, true);
            break;
          }
          WaitingMessageBox.Instance.Show(SubMenuOnline.LOC_CONNECTING);
          WaitingMessageBox.Instance.OnAbort += new Action(this.OnAbortConnectToServer);
          NetworkManager.Instance.ConnectToServer(this.mServers[this.mServerTop + this.mSelectedServer].ServerID, (string) null, new Action<ConnectionStatus>(this.OnConnectToServer));
          break;
        case 16 /*0x10*/:
          Tome.Instance.PopMenu();
          break;
        case 17:
          this.mFilterPanel.ControllA();
          break;
        default:
          if (this.mSelectedPosition >= 12)
            break;
          if (this.mSelectedServer == -1 || this.mSelectedPosition != this.mSelectedServer)
          {
            this.mSelectedServer = this.mSelectedPosition;
            this.mSelectedPosition = 15;
            break;
          }
          if (this.mSelectedPosition > this.mServers.Count)
            break;
          if (this.mServers[this.mServerTop + this.mSelectedServer].m_Password)
          {
            string upper = LanguageManager.Instance.GetString(SubMenuOnline.LOC_PASSWORD).ToUpper();
            this.mPasswordMessageBox.Show(new Action<string>(this.OnPasswordInput), iSender, upper, true);
            break;
          }
          WaitingMessageBox.Instance.Show(SubMenuOnline.LOC_CONNECTING);
          WaitingMessageBox.Instance.OnAbort += new Action(this.OnAbortConnectToServer);
          NetworkManager.Instance.ConnectToServer(this.mServers[this.mServerTop + this.mSelectedServer].ServerID, (string) null, new Action<ConnectionStatus>(this.OnConnectToServer));
          break;
      }
    }
  }

  public override void OnEnter()
  {
    GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Bottom, SubMenu.LOC_SELECT);
    GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Right, SubMenu.LOC_BACK);
    GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Left);
    GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Top);
    if (this.mFilterPanel != null)
      this.mFilterPanel.InitializeFilters();
    base.OnEnter();
    for (int index = 0; index < this.mMenuItems.Count; ++index)
      this.mMenuItems[index].Selected = false;
    this.mSelectedTab = MagickaMath.CountTrailingZeroBits((uint) GlobalSettings.Instance.Filter.GameType);
    this.RefreshServerList();
  }

  public override void OnExit()
  {
    ToolTipMan.Instance.KillAll(false);
    NetworkManager.Instance.AbortQuery();
    SaveManager.Instance.SaveSettings();
    base.OnExit();
  }
}
