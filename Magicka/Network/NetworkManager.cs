using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;
using Magicka.GameLogic;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.GameLogic.GameStates.Menu.Main;
using Magicka.GameLogic.UI;
using Magicka.Gamers;
using Magicka.Localization;
using Microsoft.Xna.Framework;
using SteamWrapper;

namespace Magicka.Network
{
	// Token: 0x0200047A RID: 1146
	internal class NetworkManager
	{
		// Token: 0x1700083A RID: 2106
		// (get) Token: 0x060022A3 RID: 8867 RVA: 0x000FA598 File Offset: 0x000F8798
		public static NetworkManager Instance
		{
			get
			{
				if (NetworkManager.sSingelton == null)
				{
					lock (NetworkManager.sSingeltonLock)
					{
						if (NetworkManager.sSingelton == null)
						{
							NetworkManager.sSingelton = new NetworkManager();
						}
					}
				}
				return NetworkManager.sSingelton;
			}
		}

		// Token: 0x060022A4 RID: 8868 RVA: 0x000FA5EC File Offset: 0x000F87EC
		private NetworkManager()
		{
			this.mAllServersReadOnly = new ReadOnlyCollection<GameServerItem>(this.mAllServers);
			SteamAPI.GameOverlayActivated += this.SteamAPI_GameOverlayActivated;
			SteamAPI.GameServerChangeRequested += this.SteamAPI_GameServerChangeRequested;
			SteamAPI.GameLobbyJoinRequested += this.SteamAPI_GameLobbyJoinRequested;
			SteamAPI.LobbyDataUpdate += this.SteamAPI_LobbyDataUpdate;
			SteamAPI.LobbyChatUpdate += this.SteamAPI_LobbyChatUpdate;
			SteamAPI.LobbyChatMsg += this.SteamAPI_LobbyChatMsg;
			this.mPasswordMessageBox = new TextInputMessageBox(SubMenuOnline.LOC_SETTINGS_PASSWORD, 10);
			this.mTimeoutMessage = new OptionsMessageBox(LanguageManager.Instance.GetString("#add_menu_err_connclose".GetHashCodeCustom()), new string[]
			{
				LanguageManager.Instance.GetString(Defines.LOC_GEN_OK)
			});
		}

		// Token: 0x1700083B RID: 2107
		// (get) Token: 0x060022A5 RID: 8869 RVA: 0x000FA71E File Offset: 0x000F891E
		public NetworkInterface Interface
		{
			get
			{
				return this.mInterface;
			}
		}

		// Token: 0x1700083C RID: 2108
		// (get) Token: 0x060022A6 RID: 8870 RVA: 0x000FA726 File Offset: 0x000F8926
		public NetworkState State
		{
			get
			{
				if (this.mInterface is NetworkServer)
				{
					return NetworkState.Server;
				}
				if (this.mInterface is NetworkClient)
				{
					return NetworkState.Client;
				}
				return NetworkState.Offline;
			}
		}

		// Token: 0x060022A7 RID: 8871 RVA: 0x000FA747 File Offset: 0x000F8947
		internal void ClearHost()
		{
			this.mGameType = GameType.Any;
			this.mLevelName = null;
		}

		// Token: 0x060022A8 RID: 8872 RVA: 0x000FA758 File Offset: 0x000F8958
		internal void SetupHost(GameType iGameType, string iName, bool iVAC, string iPassword)
		{
			this.mGameType = iGameType;
			this.mGameName = iName;
			this.mVAC = iVAC;
			this.mPassword = iPassword;
		}

		// Token: 0x060022A9 RID: 8873 RVA: 0x000FA777 File Offset: 0x000F8977
		public void SetGame(GameType iGameType, string iLevelName)
		{
			this.mGameType = iGameType;
			this.mLevelName = iLevelName;
			if (this.State == NetworkState.Server)
			{
				(this.Interface as NetworkServer).UpdateLevel(iLevelName);
			}
		}

		// Token: 0x1700083D RID: 2109
		// (get) Token: 0x060022AA RID: 8874 RVA: 0x000FA7A1 File Offset: 0x000F89A1
		public GameType GameType
		{
			get
			{
				return this.mGameType;
			}
		}

		// Token: 0x1700083E RID: 2110
		// (get) Token: 0x060022AB RID: 8875 RVA: 0x000FA7A9 File Offset: 0x000F89A9
		public string LevelName
		{
			get
			{
				return this.mLevelName;
			}
		}

		// Token: 0x1700083F RID: 2111
		// (get) Token: 0x060022AC RID: 8876 RVA: 0x000FA7B1 File Offset: 0x000F89B1
		public string Password
		{
			get
			{
				return this.mPassword;
			}
		}

		// Token: 0x17000840 RID: 2112
		// (get) Token: 0x060022AD RID: 8877 RVA: 0x000FA7B9 File Offset: 0x000F89B9
		public bool HasHostSettings
		{
			get
			{
				return this.mGameType != GameType.Any;
			}
		}

		// Token: 0x060022AE RID: 8878 RVA: 0x000FA7C8 File Offset: 0x000F89C8
		public void StartHost()
		{
			if (this.State != NetworkState.Offline)
			{
				NetworkManager.Instance.EndSession();
			}
			this.mConnectionStatus = ConnectionStatus.Connected;
			NetworkChat.Instance.Clear();
			try
			{
				this.mInterface = new NetworkServer(this.mGameType, this.mVAC);
			}
			catch (Exception)
			{
				this.mConnectionStatus = ConnectionStatus.NotConnected;
				this.mInterface = null;
				ErrorMessageBox.Instance.Show(LanguageManager.Instance.GetString(SubMenuOnline.LOC_ERROR_UNKNOWN));
				return;
			}
			string text = LanguageManager.Instance.GetString("#add_menu_not_hosting".GetHashCodeCustom());
			Vector4 vector = default(Vector4);
			vector.X = Defines.DIALOGUE_COLOR_DEFAULT.X / 0.7037f;
			vector.Y = Defines.DIALOGUE_COLOR_DEFAULT.Y / 0.7037f;
			vector.Z = Defines.DIALOGUE_COLOR_DEFAULT.Z / 0.7037f;
			vector.W = Defines.DIALOGUE_COLOR_DEFAULT.W;
			IFormatProvider numberFormat = CultureInfo.InvariantCulture.NumberFormat;
			string str = string.Format("[c={0},{1},{2},{3}]", new object[]
			{
				vector.X.ToString(numberFormat),
				vector.Y.ToString(numberFormat),
				vector.Z.ToString(numberFormat),
				vector.W.ToString(numberFormat)
			});
			text = text.Replace("#1;", str + this.mGameName + "[/c]");
			text = text.Replace("#2;", str + 27016.ToString() + "[/c]");
			NetworkChat.Instance.SetTitle(text);
		}

		// Token: 0x060022AF RID: 8879 RVA: 0x000FA974 File Offset: 0x000F8B74
		public void AbortQuery()
		{
			if (this.mRefreshPending)
			{
				SteamMatchmakingServers.CancelQuery(this.mPendingRefresh);
			}
			if (this.mPendingRefresh.Valid)
			{
				SteamMatchmakingServers.ReleaseRequest(this.mPendingRefresh);
				this.mPendingRefresh = default(ServerListRequest);
			}
		}

		// Token: 0x060022B0 RID: 8880 RVA: 0x000FA9B0 File Offset: 0x000F8BB0
		private void OnPasswordInput(string iPassword)
		{
			if (string.IsNullOrEmpty(iPassword))
			{
				return;
			}
			WaitingMessageBox.Instance.Show(SubMenuOnline.LOC_CONNECTING);
			WaitingMessageBox instance = WaitingMessageBox.Instance;
			instance.OnAbort = (Action)Delegate.Combine(instance.OnAbort, new Action(SubMenuOnline.Instance.OnAbortConnectToServer));
			this.mPassword = iPassword;
			this.ConnectToServer(this.mServer, iPassword, new Action<ConnectionStatus>(this.OnConnectToServer));
		}

		// Token: 0x060022B1 RID: 8881 RVA: 0x000FAA20 File Offset: 0x000F8C20
		private void SteamAPI_LobbyDataUpdate(LobbyDataUpdate iData)
		{
			if (this.mOverlayLobbyRequest && this.State == NetworkState.Offline)
			{
				SteamID steamIDLobby = new SteamID(iData.mSteamIDLobby);
				uint num = 0U;
				ushort num2 = 0;
				SteamID iServerID = default(SteamID);
				bool lobbyGameServer = SteamMatchmaking.GetLobbyGameServer(steamIDLobby, ref num, ref num2, ref iServerID);
				if (lobbyGameServer)
				{
					string lobbyData = SteamMatchmaking.GetLobbyData(steamIDLobby, "version");
					if (!string.Equals(lobbyData, Application.ProductVersion))
					{
						WaitingMessageBox.Instance.Kill();
						this.mOverlayLobbyRequest = false;
						ErrorMessageBox.Instance.Show(LanguageManager.Instance.GetString(SubMenuOnline.LOC_ERROR_VERSION));
						return;
					}
					string lobbyData2 = SteamMatchmaking.GetLobbyData(steamIDLobby, "password");
					int num3 = 0;
					if (!int.TryParse(lobbyData2, out num3))
					{
						num3 = 0;
					}
					string startupPassword = GlobalSettings.Instance.StartupPassword;
					if (num3 == 1)
					{
						if (!string.IsNullOrEmpty(startupPassword))
						{
							this.ConnectToServer(iServerID, startupPassword, new Action<ConnectionStatus>(this.OnConnectToServer));
							GlobalSettings.Instance.StartupPassword = null;
						}
						else
						{
							WaitingMessageBox.Instance.Kill();
							this.mServer = iServerID;
							string iDescr = LanguageManager.Instance.GetString(NetworkManager.LOC_PASSWORD).ToUpper();
							this.mPasswordMessageBox.Show(new Action<string>(this.OnPasswordInput), null, iDescr, true);
						}
					}
					else
					{
						this.ConnectToServer(iServerID, null, new Action<ConnectionStatus>(this.OnConnectToServer));
					}
				}
				else
				{
					WaitingMessageBox.Instance.Kill();
				}
				this.mOverlayLobbyRequest = false;
				return;
			}
			WaitingMessageBox.Instance.Kill();
		}

		// Token: 0x060022B2 RID: 8882 RVA: 0x000FAB88 File Offset: 0x000F8D88
		private void SteamAPI_LobbyChatUpdate(LobbyChatUpdate iChat)
		{
		}

		// Token: 0x060022B3 RID: 8883 RVA: 0x000FAB8A File Offset: 0x000F8D8A
		private void SteamAPI_LobbyChatMsg(LobbyChatMsg iMsg)
		{
		}

		// Token: 0x060022B4 RID: 8884 RVA: 0x000FAB8C File Offset: 0x000F8D8C
		private void SteamAPI_GameOverlayActivated(GameOverlayActivated iActivated)
		{
			PlayState playState = GameStateManager.Instance.CurrentState as PlayState;
			if (playState != null)
			{
				playState.OverlayPause(iActivated.mActive == 1);
			}
		}

		// Token: 0x060022B5 RID: 8885 RVA: 0x000FABC0 File Offset: 0x000F8DC0
		private void SteamAPI_GameServerChangeRequested(GameServerChangeRequested iRequest)
		{
		}

		// Token: 0x060022B6 RID: 8886 RVA: 0x000FABC4 File Offset: 0x000F8DC4
		private void SteamAPI_GameLobbyJoinRequested(GameLobbyJoinRequested iRequest)
		{
			if (this.State != NetworkState.Offline)
			{
				this.EndSession();
			}
			this.mOverlayLobbyRequest = true;
			SteamMatchmaking.RequestLobbyData(iRequest.mSteamIDLobby);
			WaitingMessageBox.Instance.Show(SubMenuOnline.LOC_CONNECTING);
			WaitingMessageBox instance = WaitingMessageBox.Instance;
			instance.OnAbort = (Action)Delegate.Combine(instance.OnAbort, new Action(this.OnAbortedLobbyRequest));
		}

		// Token: 0x060022B7 RID: 8887 RVA: 0x000FAC28 File Offset: 0x000F8E28
		private void OnConnectToServer(ConnectionStatus iStatus)
		{
			this.AbortQuery();
			WaitingMessageBox.Instance.Kill();
			if (this.State == NetworkState.Offline)
			{
				return;
			}
			switch (iStatus)
			{
			case ConnectionStatus.NotConnected:
			case ConnectionStatus.Connecting:
			case ConnectionStatus.Authenticating:
				return;
			case ConnectionStatus.Connected:
				if (Tome.Instance.CurrentState is Tome.ClosedState | Tome.Instance.CurrentState is Tome.ClosedBack)
				{
					Tome.Instance.ChangeState(Tome.OpeningState.Instance);
					Tome.Instance.PushMenu(SubMenuMain.Instance, 1);
				}
				while (!(Tome.Instance.CurrentMenu is SubMenuMain) && Tome.Instance.CurrentMenu != null)
				{
					Tome.Instance.PopMenuInstant();
				}
				Tome.Instance.PushMenuInstant(SubMenuOnline.Instance, 1);
				Tome.Instance.PushMenuInstant(SubMenuCharacterSelect.Instance, 1);
				return;
			case ConnectionStatus.Failed_GameFull:
				ErrorMessageBox.Instance.Show(LanguageManager.Instance.GetString(SubMenuOnline.LOC_ERROR_SERVERFULL));
				return;
			case ConnectionStatus.Failed_Authentication:
				ErrorMessageBox.Instance.Show(LanguageManager.Instance.GetString(SubMenuOnline.LOC_ERROR_AUTHFAIL));
				return;
			case ConnectionStatus.Failed_GamePlaying:
				ErrorMessageBox.Instance.Show(LanguageManager.Instance.GetString(SubMenuOnline.LOC_ERROR_INPROGRESS));
				return;
			case ConnectionStatus.Failed_Version:
				ErrorMessageBox.Instance.Show(LanguageManager.Instance.GetString(SubMenuOnline.LOC_ERROR_VERSION));
				return;
			case ConnectionStatus.Failed_Password:
				ErrorMessageBox.Instance.Show(LanguageManager.Instance.GetString(SubMenuOnline.LOC_ERROR_PASSWORD));
				return;
			case ConnectionStatus.Failed_Timeout:
				ErrorMessageBox.Instance.Show(LanguageManager.Instance.GetString(SubMenuOnline.LOC_ERROR_TIMEOUT));
				return;
			}
			ErrorMessageBox.Instance.Show(LanguageManager.Instance.GetString(SubMenuOnline.LOC_ERROR_UNKNOWN));
		}

		// Token: 0x060022B8 RID: 8888 RVA: 0x000FADCC File Offset: 0x000F8FCC
		internal void DirectConnect(ulong iLobbyID)
		{
			this.AbortQuery();
			SteamID steamIDLobby = new SteamID(iLobbyID);
			bool flag = SteamMatchmaking.RequestLobbyData(steamIDLobby);
			this.mOverlayLobbyRequest = false;
			uint num = 0U;
			ushort num2 = 0;
			SteamID iServerID = default(SteamID);
			flag = SteamMatchmaking.GetLobbyGameServer(steamIDLobby, ref num, ref num2, ref iServerID);
			this.mPassword = GlobalSettings.Instance.StartupPassword;
			if (flag)
			{
				this.ConnectToServer(iServerID, this.mPassword, new Action<ConnectionStatus>(this.OnConnectToServer));
				WaitingMessageBox.Instance.Show(SubMenuOnline.LOC_CONNECTING);
				WaitingMessageBox instance = WaitingMessageBox.Instance;
				instance.OnAbort = (Action)Delegate.Combine(instance.OnAbort, new Action(this.OnAbortedDirectConnect));
				GlobalSettings.Instance.StartupPassword = null;
				return;
			}
			this.mOverlayLobbyRequest = true;
			WaitingMessageBox.Instance.Show(SubMenuOnline.LOC_CONNECTING);
			WaitingMessageBox instance2 = WaitingMessageBox.Instance;
			instance2.OnAbort = (Action)Delegate.Combine(instance2.OnAbort, new Action(this.OnAbortedLobbyRequest));
		}

		// Token: 0x060022B9 RID: 8889 RVA: 0x000FAEB8 File Offset: 0x000F90B8
		private void OnAbortedLobbyRequest()
		{
		}

		// Token: 0x060022BA RID: 8890 RVA: 0x000FAEBA File Offset: 0x000F90BA
		private void OnAbortedDirectConnect()
		{
		}

		// Token: 0x060022BB RID: 8891 RVA: 0x000FAEBC File Offset: 0x000F90BC
		public void ConnectToServer(uint iIP, string iPassword, Action<ConnectionStatus> iOnComplete)
		{
			this.AbortQuery();
			NetworkManager.PingResponse.Instance.ConnectTo(iIP, iPassword, iOnComplete);
		}

		// Token: 0x060022BC RID: 8892 RVA: 0x000FAF70 File Offset: 0x000F9170
		public void ConnectToServer(SteamID iServerID, string iPassword, Action<ConnectionStatus> iOnComplete)
		{
			this.AbortQuery();
			if (this.State != NetworkState.Offline)
			{
				NetworkManager.Instance.EndSession();
			}
			this.mPassword = iPassword;
			Game.Instance.AddLoadTask(delegate
			{
				try
				{
					this.mConnectionStatus = ConnectionStatus.Connecting;
					NetworkChat.Instance.Clear();
					this.mInterface = new NetworkClient(iServerID, iOnComplete);
				}
				catch (TimeoutException)
				{
					this.mTimeoutMessage.Show();
					this.mConnectionStatus = ConnectionStatus.Failed_Unknown;
					iOnComplete(this.mConnectionStatus);
					this.EndSession();
				}
			});
		}

		// Token: 0x060022BD RID: 8893 RVA: 0x000FAFD0 File Offset: 0x000F91D0
		public void RequestServers(FilterData iFilter)
		{
			lock (this.mAllServers)
			{
				if (this.mRefreshPending)
				{
					SteamMatchmakingServers.CancelQuery(this.mPendingRefresh);
				}
				if (this.mPendingRefresh.Valid)
				{
					SteamMatchmakingServers.ReleaseRequest(this.mPendingRefresh);
					this.mPendingRefresh = default(ServerListRequest);
				}
				this.mRefreshPending = true;
				this.mAllServers.Clear();
				if (iFilter.Scope == Scope.LAN)
				{
					this.mPendingRefresh = SteamMatchmakingServers.RequestLANServerList(SteamUtils.GetAppID(), NetworkManager.ListResponse.Instance);
				}
				else
				{
					MatchMakingKeyValuePair[] ppchFilters = new List<MatchMakingKeyValuePair>
					{
						new MatchMakingKeyValuePair
						{
							Key = "gamedir",
							Value = "magicka"
						}
					}.ToArray();
					if (iFilter.Scope == Scope.FriendsOnly)
					{
						this.mPendingRefresh = SteamMatchmakingServers.RequestFriendsServerList(SteamUtils.GetAppID(), ppchFilters, NetworkManager.ListResponse.Instance);
					}
					else
					{
						this.mPendingRefresh = SteamMatchmakingServers.RequestInternetServerList(SteamUtils.GetAppID(), ppchFilters, NetworkManager.ListResponse.Instance);
					}
				}
			}
		}

		// Token: 0x060022BE RID: 8894 RVA: 0x000FB0DC File Offset: 0x000F92DC
		public void PingServers()
		{
		}

		// Token: 0x060022BF RID: 8895 RVA: 0x000FB0E0 File Offset: 0x000F92E0
		public void EndSession()
		{
			this.mConnectionStatus = ConnectionStatus.NotConnected;
			if (this.mInterface != null)
			{
				this.mInterface.CloseConnection();
				this.mInterface.Dispose();
				this.mInterface = null;
				NetworkChat.Instance.Clear();
				Player[] players = Game.Instance.Players;
				for (int i = 0; i < players.Length; i++)
				{
					if (players[i].Playing && players[i].Gamer is NetworkGamer)
					{
						players[i].Leave();
					}
				}
				while (!(Tome.Instance.CurrentMenu is SubMenuOnline) && !(Tome.Instance.CurrentMenu is SubMenuMain))
				{
					Tome.Instance.PopMenuInstant();
				}
			}
		}

		// Token: 0x060022C0 RID: 8896 RVA: 0x000FB190 File Offset: 0x000F9390
		internal void Dispose()
		{
			this.EndSession();
			this.mListen = false;
			SteamAPI.GameOverlayActivated -= this.SteamAPI_GameOverlayActivated;
			SteamAPI.GameServerChangeRequested -= this.SteamAPI_GameServerChangeRequested;
			SteamAPI.GameLobbyJoinRequested -= this.SteamAPI_GameLobbyJoinRequested;
			SteamAPI.LobbyDataUpdate -= this.SteamAPI_LobbyDataUpdate;
			SteamAPI.LobbyChatUpdate -= this.SteamAPI_LobbyChatUpdate;
			SteamAPI.LobbyChatMsg -= this.SteamAPI_LobbyChatMsg;
		}

		// Token: 0x17000841 RID: 2113
		// (get) Token: 0x060022C1 RID: 8897 RVA: 0x000FB210 File Offset: 0x000F9410
		// (set) Token: 0x060022C2 RID: 8898 RVA: 0x000FB218 File Offset: 0x000F9418
		public string GameName
		{
			get
			{
				return this.mGameName;
			}
			set
			{
				this.mGameName = value;
			}
		}

		// Token: 0x17000842 RID: 2114
		// (get) Token: 0x060022C3 RID: 8899 RVA: 0x000FB221 File Offset: 0x000F9421
		// (set) Token: 0x060022C4 RID: 8900 RVA: 0x000FB229 File Offset: 0x000F9429
		public ConnectionStatus ConnectionStatus
		{
			get
			{
				return this.mConnectionStatus;
			}
			set
			{
				this.mConnectionStatus = value;
			}
		}

		// Token: 0x060022C5 RID: 8901 RVA: 0x000FB232 File Offset: 0x000F9432
		public void Update()
		{
			if (this.mInterface != null)
			{
				this.mInterface.Update();
			}
		}

		// Token: 0x040025F1 RID: 9713
		public const int MTU = 1100;

		// Token: 0x040025F2 RID: 9714
		private static NetworkManager sSingelton;

		// Token: 0x040025F3 RID: 9715
		private static volatile object sSingeltonLock = new object();

		// Token: 0x040025F4 RID: 9716
		private Stopwatch mTimer = new Stopwatch();

		// Token: 0x040025F5 RID: 9717
		private NetworkInterface mInterface;

		// Token: 0x040025F6 RID: 9718
		private bool mRefreshPending;

		// Token: 0x040025F7 RID: 9719
		private ServerListRequest mPendingRefresh;

		// Token: 0x040025F8 RID: 9720
		private bool mOverlayLobbyRequest;

		// Token: 0x040025F9 RID: 9721
		private bool mVAC;

		// Token: 0x040025FA RID: 9722
		private string mPassword = "";

		// Token: 0x040025FB RID: 9723
		private TextInputMessageBox mPasswordMessageBox;

		// Token: 0x040025FC RID: 9724
		private SteamID mServer;

		// Token: 0x040025FD RID: 9725
		private ConnectionStatus mConnectionStatus;

		// Token: 0x040025FE RID: 9726
		private string mGameName = "Magicka Game";

		// Token: 0x040025FF RID: 9727
		private GameType mGameType = GameType.Any;

		// Token: 0x04002600 RID: 9728
		private string mLevelName;

		// Token: 0x04002601 RID: 9729
		private byte[] mTXBuffer = new byte[1024];

		// Token: 0x04002602 RID: 9730
		private byte[] mRXBuffer = new byte[1024];

		// Token: 0x04002603 RID: 9731
		private List<GameServerItem> mAllServers = new List<GameServerItem>(32);

		// Token: 0x04002604 RID: 9732
		private ReadOnlyCollection<GameServerItem> mAllServersReadOnly;

		// Token: 0x04002605 RID: 9733
		private bool mListen = true;

		// Token: 0x04002606 RID: 9734
		private OptionsMessageBox mTimeoutMessage;

		// Token: 0x04002607 RID: 9735
		private static readonly int LOC_PASSWORD = "#SETTINGS_P01_PASSWORD".GetHashCodeCustom();

		// Token: 0x0200047B RID: 1147
		public class ListResponse : SteamMatchmakingServerListResponse
		{
			// Token: 0x17000843 RID: 2115
			// (get) Token: 0x060022C7 RID: 8903 RVA: 0x000FB264 File Offset: 0x000F9464
			public static NetworkManager.ListResponse Instance
			{
				get
				{
					if (NetworkManager.ListResponse.sSingelton == null)
					{
						lock (NetworkManager.ListResponse.sSingeltonLock)
						{
							if (NetworkManager.ListResponse.sSingelton == null)
							{
								NetworkManager.ListResponse.sSingelton = new NetworkManager.ListResponse();
							}
						}
					}
					return NetworkManager.ListResponse.sSingelton;
				}
			}

			// Token: 0x060022C8 RID: 8904 RVA: 0x000FB2B8 File Offset: 0x000F94B8
			private ListResponse()
			{
			}

			// Token: 0x060022C9 RID: 8905 RVA: 0x000FB2C0 File Offset: 0x000F94C0
			public override void RefreshComplete(ServerListRequest hRequest, MatchMakingServerResponse response)
			{
				NetworkManager.Instance.mRefreshPending = false;
				NetworkManager.Instance.mPendingRefresh = default(ServerListRequest);
			}

			// Token: 0x060022CA RID: 8906 RVA: 0x000FB2DD File Offset: 0x000F94DD
			public override void ServerFailedToRespond(ServerListRequest hRequest, int iServer)
			{
				SteamMatchmakingServers.GetServerDetails(hRequest, iServer);
			}

			// Token: 0x060022CB RID: 8907 RVA: 0x000FB2E8 File Offset: 0x000F94E8
			public override void ServerResponded(ServerListRequest hRequest, int iServer)
			{
				GameServerItem serverDetails = SteamMatchmakingServers.GetServerDetails(hRequest, iServer);
				NetworkManager.Instance.mAllServers.Add(serverDetails);
				if (this.ServerListChanged != null)
				{
					this.ServerListChanged(NetworkManager.Instance.mAllServersReadOnly);
				}
			}

			// Token: 0x04002608 RID: 9736
			private static NetworkManager.ListResponse sSingelton;

			// Token: 0x04002609 RID: 9737
			private static volatile object sSingeltonLock = new object();

			// Token: 0x0400260A RID: 9738
			public Action<ReadOnlyCollection<GameServerItem>> ServerListChanged;
		}

		// Token: 0x0200047C RID: 1148
		private class PingResponse : SteamMatchmakingPingResponse
		{
			// Token: 0x17000844 RID: 2116
			// (get) Token: 0x060022CD RID: 8909 RVA: 0x000FB338 File Offset: 0x000F9538
			public static NetworkManager.PingResponse Instance
			{
				get
				{
					if (NetworkManager.PingResponse.sSingelton == null)
					{
						lock (NetworkManager.PingResponse.sSingeltonLock)
						{
							if (NetworkManager.PingResponse.sSingelton == null)
							{
								NetworkManager.PingResponse.sSingelton = new NetworkManager.PingResponse();
							}
						}
					}
					return NetworkManager.PingResponse.sSingelton;
				}
			}

			// Token: 0x060022CE RID: 8910 RVA: 0x000FB38C File Offset: 0x000F958C
			private PingResponse()
			{
			}

			// Token: 0x060022CF RID: 8911 RVA: 0x000FB394 File Offset: 0x000F9594
			public override void ServerFailedToRespond()
			{
				WaitingMessageBox.Instance.Kill();
			}

			// Token: 0x060022D0 RID: 8912 RVA: 0x000FB3A0 File Offset: 0x000F95A0
			public override void ServerResponded(GameServerItem iServer)
			{
				if (iServer.m_NetAdr.GetIP() == this.mConnectToIP && (uint)iServer.m_NetAdr.GetQueryPort() == this.mConnectToPort)
				{
					NetworkManager.Instance.ConnectToServer(iServer.ServerID, this.mPassword, this.mOnComplete);
				}
			}

			// Token: 0x060022D1 RID: 8913 RVA: 0x000FB3EF File Offset: 0x000F95EF
			public void ConnectTo(uint iIP, string iPassword, Action<ConnectionStatus> iOnComplete)
			{
				this.mOnComplete = iOnComplete;
				this.mConnectToIP = iIP;
				this.mConnectToPort = 27016U;
				this.mPassword = iPassword;
				SteamMatchmakingServers.PingServer(iIP, (ushort)this.mConnectToPort, this);
			}

			// Token: 0x0400260B RID: 9739
			private static NetworkManager.PingResponse sSingelton;

			// Token: 0x0400260C RID: 9740
			private static volatile object sSingeltonLock = new object();

			// Token: 0x0400260D RID: 9741
			private uint mConnectToIP;

			// Token: 0x0400260E RID: 9742
			private uint mConnectToPort;

			// Token: 0x0400260F RID: 9743
			private string mPassword;

			// Token: 0x04002610 RID: 9744
			private Action<ConnectionStatus> mOnComplete;
		}
	}
}
