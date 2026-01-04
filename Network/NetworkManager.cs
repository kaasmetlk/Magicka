// Decompiled with JetBrains decompiler
// Type: Magicka.Network.NetworkManager
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.GameLogic.GameStates.Menu.Main;
using Magicka.GameLogic.UI;
using Magicka.Gamers;
using Magicka.Localization;
using Microsoft.Xna.Framework;
using SteamWrapper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;

#nullable disable
namespace Magicka.Network;

internal class NetworkManager
{
  public const int MTU = 1100;
  private static NetworkManager sSingelton;
  private static volatile object sSingeltonLock = new object();
  private Stopwatch mTimer = new Stopwatch();
  private NetworkInterface mInterface;
  private bool mRefreshPending;
  private ServerListRequest mPendingRefresh;
  private bool mOverlayLobbyRequest;
  private bool mVAC;
  private string mPassword = "";
  private TextInputMessageBox mPasswordMessageBox;
  private SteamID mServer;
  private ConnectionStatus mConnectionStatus;
  private string mGameName = "Magicka Game";
  private GameType mGameType = GameType.Any;
  private string mLevelName;
  private byte[] mTXBuffer = new byte[1024 /*0x0400*/];
  private byte[] mRXBuffer = new byte[1024 /*0x0400*/];
  private List<GameServerItem> mAllServers = new List<GameServerItem>(32 /*0x20*/);
  private ReadOnlyCollection<GameServerItem> mAllServersReadOnly;
  private bool mListen = true;
  private OptionsMessageBox mTimeoutMessage;
  private static readonly int LOC_PASSWORD = "#SETTINGS_P01_PASSWORD".GetHashCodeCustom();

  public static NetworkManager Instance
  {
    get
    {
      if (NetworkManager.sSingelton == null)
      {
        lock (NetworkManager.sSingeltonLock)
        {
          if (NetworkManager.sSingelton == null)
            NetworkManager.sSingelton = new NetworkManager();
        }
      }
      return NetworkManager.sSingelton;
    }
  }

  private NetworkManager()
  {
    this.mAllServersReadOnly = new ReadOnlyCollection<GameServerItem>((IList<GameServerItem>) this.mAllServers);
    SteamAPI.GameOverlayActivated += new Action<GameOverlayActivated>(this.SteamAPI_GameOverlayActivated);
    SteamAPI.GameServerChangeRequested += new Action<GameServerChangeRequested>(this.SteamAPI_GameServerChangeRequested);
    SteamAPI.GameLobbyJoinRequested += new Action<GameLobbyJoinRequested>(this.SteamAPI_GameLobbyJoinRequested);
    SteamAPI.LobbyDataUpdate += new Action<LobbyDataUpdate>(this.SteamAPI_LobbyDataUpdate);
    SteamAPI.LobbyChatUpdate += new Action<LobbyChatUpdate>(this.SteamAPI_LobbyChatUpdate);
    SteamAPI.LobbyChatMsg += new Action<LobbyChatMsg>(this.SteamAPI_LobbyChatMsg);
    this.mPasswordMessageBox = new TextInputMessageBox(SubMenuOnline.LOC_SETTINGS_PASSWORD, 10);
    this.mTimeoutMessage = new OptionsMessageBox(LanguageManager.Instance.GetString("#add_menu_err_connclose".GetHashCodeCustom()), new string[1]
    {
      LanguageManager.Instance.GetString(Defines.LOC_GEN_OK)
    });
  }

  public NetworkInterface Interface => this.mInterface;

  public NetworkState State
  {
    get
    {
      if (this.mInterface is NetworkServer)
        return NetworkState.Server;
      return this.mInterface is NetworkClient ? NetworkState.Client : NetworkState.Offline;
    }
  }

  internal void ClearHost()
  {
    this.mGameType = GameType.Any;
    this.mLevelName = (string) null;
  }

  internal void SetupHost(GameType iGameType, string iName, bool iVAC, string iPassword)
  {
    this.mGameType = iGameType;
    this.mGameName = iName;
    this.mVAC = iVAC;
    this.mPassword = iPassword;
  }

  public void SetGame(GameType iGameType, string iLevelName)
  {
    this.mGameType = iGameType;
    this.mLevelName = iLevelName;
    if (this.State != NetworkState.Server)
      return;
    (this.Interface as NetworkServer).UpdateLevel(iLevelName);
  }

  public GameType GameType => this.mGameType;

  public string LevelName => this.mLevelName;

  public string Password => this.mPassword;

  public bool HasHostSettings => this.mGameType != GameType.Any;

  public void StartHost()
  {
    if (this.State != NetworkState.Offline)
      NetworkManager.Instance.EndSession();
    this.mConnectionStatus = ConnectionStatus.Connected;
    NetworkChat.Instance.Clear();
    try
    {
      this.mInterface = (NetworkInterface) new NetworkServer(this.mGameType, this.mVAC);
    }
    catch (Exception ex)
    {
      this.mConnectionStatus = ConnectionStatus.NotConnected;
      this.mInterface = (NetworkInterface) null;
      ErrorMessageBox.Instance.Show(LanguageManager.Instance.GetString(SubMenuOnline.LOC_ERROR_UNKNOWN));
      return;
    }
    string str1 = LanguageManager.Instance.GetString("#add_menu_not_hosting".GetHashCodeCustom());
    Vector4 vector4 = new Vector4();
    vector4.X = Defines.DIALOGUE_COLOR_DEFAULT.X / 0.7037f;
    vector4.Y = Defines.DIALOGUE_COLOR_DEFAULT.Y / 0.7037f;
    vector4.Z = Defines.DIALOGUE_COLOR_DEFAULT.Z / 0.7037f;
    vector4.W = Defines.DIALOGUE_COLOR_DEFAULT.W;
    IFormatProvider numberFormat = (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat;
    string str2 = $"[c={vector4.X.ToString(numberFormat)},{vector4.Y.ToString(numberFormat)},{vector4.Z.ToString(numberFormat)},{vector4.W.ToString(numberFormat)}]";
    NetworkChat.Instance.SetTitle(str1.Replace("#1;", $"{str2}{this.mGameName}[/c]").Replace("#2;", $"{str2}{27016.ToString()}[/c]"));
  }

  public void AbortQuery()
  {
    if (this.mRefreshPending)
      SteamMatchmakingServers.CancelQuery(this.mPendingRefresh);
    if (!this.mPendingRefresh.Valid)
      return;
    SteamMatchmakingServers.ReleaseRequest(this.mPendingRefresh);
    this.mPendingRefresh = new ServerListRequest();
  }

  private void OnPasswordInput(string iPassword)
  {
    if (string.IsNullOrEmpty(iPassword))
      return;
    WaitingMessageBox.Instance.Show(SubMenuOnline.LOC_CONNECTING);
    WaitingMessageBox.Instance.OnAbort += new Action(SubMenuOnline.Instance.OnAbortConnectToServer);
    this.mPassword = iPassword;
    this.ConnectToServer(this.mServer, iPassword, new Action<ConnectionStatus>(this.OnConnectToServer));
  }

  private void SteamAPI_LobbyDataUpdate(LobbyDataUpdate iData)
  {
    if (this.mOverlayLobbyRequest && this.State == NetworkState.Offline)
    {
      SteamID steamIDLobby = new SteamID(iData.mSteamIDLobby);
      uint punGameServerIP = 0;
      ushort punGameServerPort = 0;
      SteamID psteamIDGameServer = new SteamID();
      if (SteamMatchmaking.GetLobbyGameServer(steamIDLobby, ref punGameServerIP, ref punGameServerPort, ref psteamIDGameServer))
      {
        if (!string.Equals(SteamMatchmaking.GetLobbyData(steamIDLobby, "version"), Application.ProductVersion))
        {
          WaitingMessageBox.Instance.Kill();
          this.mOverlayLobbyRequest = false;
          ErrorMessageBox.Instance.Show(LanguageManager.Instance.GetString(SubMenuOnline.LOC_ERROR_VERSION));
          return;
        }
        string lobbyData = SteamMatchmaking.GetLobbyData(steamIDLobby, "password");
        int result = 0;
        if (!int.TryParse(lobbyData, out result))
          result = 0;
        string startupPassword = GlobalSettings.Instance.StartupPassword;
        if (result == 1)
        {
          if (!string.IsNullOrEmpty(startupPassword))
          {
            this.ConnectToServer(psteamIDGameServer, startupPassword, new Action<ConnectionStatus>(this.OnConnectToServer));
            GlobalSettings.Instance.StartupPassword = (string) null;
          }
          else
          {
            WaitingMessageBox.Instance.Kill();
            this.mServer = psteamIDGameServer;
            this.mPasswordMessageBox.Show(new Action<string>(this.OnPasswordInput), (Controller) null, LanguageManager.Instance.GetString(NetworkManager.LOC_PASSWORD).ToUpper(), true);
          }
        }
        else
          this.ConnectToServer(psteamIDGameServer, (string) null, new Action<ConnectionStatus>(this.OnConnectToServer));
      }
      else
        WaitingMessageBox.Instance.Kill();
      this.mOverlayLobbyRequest = false;
    }
    else
      WaitingMessageBox.Instance.Kill();
  }

  private void SteamAPI_LobbyChatUpdate(LobbyChatUpdate iChat)
  {
  }

  private void SteamAPI_LobbyChatMsg(LobbyChatMsg iMsg)
  {
  }

  private void SteamAPI_GameOverlayActivated(GameOverlayActivated iActivated)
  {
    if (!(GameStateManager.Instance.CurrentState is PlayState currentState))
      return;
    int num = iActivated.mActive == (byte) 1 ? 1 : 0;
    currentState.OverlayPause(num != 0);
  }

  private void SteamAPI_GameServerChangeRequested(GameServerChangeRequested iRequest)
  {
  }

  private void SteamAPI_GameLobbyJoinRequested(GameLobbyJoinRequested iRequest)
  {
    if (this.State != NetworkState.Offline)
      this.EndSession();
    this.mOverlayLobbyRequest = true;
    SteamMatchmaking.RequestLobbyData(iRequest.mSteamIDLobby);
    WaitingMessageBox.Instance.Show(SubMenuOnline.LOC_CONNECTING);
    WaitingMessageBox.Instance.OnAbort += new Action(this.OnAbortedLobbyRequest);
  }

  private void OnConnectToServer(ConnectionStatus iStatus)
  {
    this.AbortQuery();
    WaitingMessageBox.Instance.Kill();
    if (this.State == NetworkState.Offline)
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
        if (Tome.Instance.CurrentState is Tome.ClosedState | Tome.Instance.CurrentState is Tome.ClosedBack)
        {
          Tome.Instance.ChangeState((Tome.TomeState) Tome.OpeningState.Instance);
          Tome.Instance.PushMenu((SubMenu) SubMenuMain.Instance, 1);
        }
        while (!(Tome.Instance.CurrentMenu is SubMenuMain) && Tome.Instance.CurrentMenu != null)
          Tome.Instance.PopMenuInstant();
        Tome.Instance.PushMenuInstant((SubMenu) SubMenuOnline.Instance, 1);
        Tome.Instance.PushMenuInstant((SubMenu) SubMenuCharacterSelect.Instance, 1);
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

  internal void DirectConnect(ulong iLobbyID)
  {
    this.AbortQuery();
    SteamID steamIDLobby = new SteamID(iLobbyID);
    SteamMatchmaking.RequestLobbyData(steamIDLobby);
    this.mOverlayLobbyRequest = false;
    uint punGameServerIP = 0;
    ushort punGameServerPort = 0;
    SteamID psteamIDGameServer = new SteamID();
    bool lobbyGameServer = SteamMatchmaking.GetLobbyGameServer(steamIDLobby, ref punGameServerIP, ref punGameServerPort, ref psteamIDGameServer);
    this.mPassword = GlobalSettings.Instance.StartupPassword;
    if (lobbyGameServer)
    {
      this.ConnectToServer(psteamIDGameServer, this.mPassword, new Action<ConnectionStatus>(this.OnConnectToServer));
      WaitingMessageBox.Instance.Show(SubMenuOnline.LOC_CONNECTING);
      WaitingMessageBox.Instance.OnAbort += new Action(this.OnAbortedDirectConnect);
      GlobalSettings.Instance.StartupPassword = (string) null;
    }
    else
    {
      this.mOverlayLobbyRequest = true;
      WaitingMessageBox.Instance.Show(SubMenuOnline.LOC_CONNECTING);
      WaitingMessageBox.Instance.OnAbort += new Action(this.OnAbortedLobbyRequest);
    }
  }

  private void OnAbortedLobbyRequest()
  {
  }

  private void OnAbortedDirectConnect()
  {
  }

  public void ConnectToServer(uint iIP, string iPassword, Action<ConnectionStatus> iOnComplete)
  {
    this.AbortQuery();
    NetworkManager.PingResponse.Instance.ConnectTo(iIP, iPassword, iOnComplete);
  }

  public void ConnectToServer(
    SteamID iServerID,
    string iPassword,
    Action<ConnectionStatus> iOnComplete)
  {
    this.AbortQuery();
    if (this.State != NetworkState.Offline)
      NetworkManager.Instance.EndSession();
    this.mPassword = iPassword;
    Magicka.Game.Instance.AddLoadTask((Action) (() =>
    {
      try
      {
        this.mConnectionStatus = ConnectionStatus.Connecting;
        NetworkChat.Instance.Clear();
        this.mInterface = (NetworkInterface) new NetworkClient(iServerID, iOnComplete);
      }
      catch (TimeoutException ex)
      {
        this.mTimeoutMessage.Show();
        this.mConnectionStatus = ConnectionStatus.Failed_Unknown;
        iOnComplete(this.mConnectionStatus);
        this.EndSession();
      }
    }));
  }

  public void RequestServers(FilterData iFilter)
  {
    lock (this.mAllServers)
    {
      if (this.mRefreshPending)
        SteamMatchmakingServers.CancelQuery(this.mPendingRefresh);
      if (this.mPendingRefresh.Valid)
      {
        SteamMatchmakingServers.ReleaseRequest(this.mPendingRefresh);
        this.mPendingRefresh = new ServerListRequest();
      }
      this.mRefreshPending = true;
      this.mAllServers.Clear();
      if (iFilter.Scope == Scope.LAN)
      {
        this.mPendingRefresh = SteamMatchmakingServers.RequestLANServerList(SteamUtils.GetAppID(), (SteamMatchmakingServerListResponse) NetworkManager.ListResponse.Instance);
      }
      else
      {
        MatchMakingKeyValuePair[] array = new List<MatchMakingKeyValuePair>()
        {
          new MatchMakingKeyValuePair()
          {
            Key = "gamedir",
            Value = "magicka"
          }
        }.ToArray();
        if (iFilter.Scope == Scope.FriendsOnly)
          this.mPendingRefresh = SteamMatchmakingServers.RequestFriendsServerList(SteamUtils.GetAppID(), array, (SteamMatchmakingServerListResponse) NetworkManager.ListResponse.Instance);
        else
          this.mPendingRefresh = SteamMatchmakingServers.RequestInternetServerList(SteamUtils.GetAppID(), array, (SteamMatchmakingServerListResponse) NetworkManager.ListResponse.Instance);
      }
    }
  }

  public void PingServers()
  {
  }

  public void EndSession()
  {
    this.mConnectionStatus = ConnectionStatus.NotConnected;
    if (this.mInterface == null)
      return;
    this.mInterface.CloseConnection();
    this.mInterface.Dispose();
    this.mInterface = (NetworkInterface) null;
    NetworkChat.Instance.Clear();
    Player[] players = Magicka.Game.Instance.Players;
    for (int index = 0; index < players.Length; ++index)
    {
      if (players[index].Playing && players[index].Gamer is NetworkGamer)
        players[index].Leave();
    }
    while (!(Tome.Instance.CurrentMenu is SubMenuOnline) && !(Tome.Instance.CurrentMenu is SubMenuMain))
      Tome.Instance.PopMenuInstant();
  }

  internal void Dispose()
  {
    this.EndSession();
    this.mListen = false;
    SteamAPI.GameOverlayActivated -= new Action<GameOverlayActivated>(this.SteamAPI_GameOverlayActivated);
    SteamAPI.GameServerChangeRequested -= new Action<GameServerChangeRequested>(this.SteamAPI_GameServerChangeRequested);
    SteamAPI.GameLobbyJoinRequested -= new Action<GameLobbyJoinRequested>(this.SteamAPI_GameLobbyJoinRequested);
    SteamAPI.LobbyDataUpdate -= new Action<LobbyDataUpdate>(this.SteamAPI_LobbyDataUpdate);
    SteamAPI.LobbyChatUpdate -= new Action<LobbyChatUpdate>(this.SteamAPI_LobbyChatUpdate);
    SteamAPI.LobbyChatMsg -= new Action<LobbyChatMsg>(this.SteamAPI_LobbyChatMsg);
  }

  public string GameName
  {
    get => this.mGameName;
    set => this.mGameName = value;
  }

  public ConnectionStatus ConnectionStatus
  {
    get => this.mConnectionStatus;
    set => this.mConnectionStatus = value;
  }

  public void Update()
  {
    if (this.mInterface == null)
      return;
    this.mInterface.Update();
  }

  public class ListResponse : SteamMatchmakingServerListResponse
  {
    private static NetworkManager.ListResponse sSingelton;
    private static volatile object sSingeltonLock = new object();
    public Action<ReadOnlyCollection<GameServerItem>> ServerListChanged;

    public static NetworkManager.ListResponse Instance
    {
      get
      {
        if (NetworkManager.ListResponse.sSingelton == null)
        {
          lock (NetworkManager.ListResponse.sSingeltonLock)
          {
            if (NetworkManager.ListResponse.sSingelton == null)
              NetworkManager.ListResponse.sSingelton = new NetworkManager.ListResponse();
          }
        }
        return NetworkManager.ListResponse.sSingelton;
      }
    }

    private ListResponse()
    {
    }

    public override void RefreshComplete(
      ServerListRequest hRequest,
      MatchMakingServerResponse response)
    {
      NetworkManager.Instance.mRefreshPending = false;
      NetworkManager.Instance.mPendingRefresh = new ServerListRequest();
    }

    public override void ServerFailedToRespond(ServerListRequest hRequest, int iServer)
    {
      SteamMatchmakingServers.GetServerDetails(hRequest, iServer);
    }

    public override void ServerResponded(ServerListRequest hRequest, int iServer)
    {
      NetworkManager.Instance.mAllServers.Add(SteamMatchmakingServers.GetServerDetails(hRequest, iServer));
      if (this.ServerListChanged == null)
        return;
      this.ServerListChanged(NetworkManager.Instance.mAllServersReadOnly);
    }
  }

  private class PingResponse : SteamMatchmakingPingResponse
  {
    private static NetworkManager.PingResponse sSingelton;
    private static volatile object sSingeltonLock = new object();
    private uint mConnectToIP;
    private uint mConnectToPort;
    private string mPassword;
    private Action<ConnectionStatus> mOnComplete;

    public static NetworkManager.PingResponse Instance
    {
      get
      {
        if (NetworkManager.PingResponse.sSingelton == null)
        {
          lock (NetworkManager.PingResponse.sSingeltonLock)
          {
            if (NetworkManager.PingResponse.sSingelton == null)
              NetworkManager.PingResponse.sSingelton = new NetworkManager.PingResponse();
          }
        }
        return NetworkManager.PingResponse.sSingelton;
      }
    }

    private PingResponse()
    {
    }

    public override void ServerFailedToRespond() => WaitingMessageBox.Instance.Kill();

    public override void ServerResponded(GameServerItem iServer)
    {
      if ((int) iServer.m_NetAdr.GetIP() != (int) this.mConnectToIP || (int) iServer.m_NetAdr.GetQueryPort() != (int) this.mConnectToPort)
        return;
      NetworkManager.Instance.ConnectToServer(iServer.ServerID, this.mPassword, this.mOnComplete);
    }

    public void ConnectTo(uint iIP, string iPassword, Action<ConnectionStatus> iOnComplete)
    {
      this.mOnComplete = iOnComplete;
      this.mConnectToIP = iIP;
      this.mConnectToPort = 27016U;
      this.mPassword = iPassword;
      SteamMatchmakingServers.PingServer(iIP, (ushort) this.mConnectToPort, (SteamMatchmakingPingResponse) this);
    }
  }
}
