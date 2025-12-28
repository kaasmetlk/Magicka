using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows.Forms;
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

namespace Magicka.GameLogic.GameStates.Menu.Main
{
	// Token: 0x020003A6 RID: 934
	internal class SubMenuOnline : SubMenu
	{
		// Token: 0x1700070E RID: 1806
		// (get) Token: 0x06001C9D RID: 7325 RVA: 0x000C69C4 File Offset: 0x000C4BC4
		public static SubMenuOnline Instance
		{
			get
			{
				if (SubMenuOnline.sSingelton == null)
				{
					lock (SubMenuOnline.sSingeltonLock)
					{
						if (SubMenuOnline.sSingelton == null)
						{
							SubMenuOnline.sSingelton = new SubMenuOnline();
						}
					}
				}
				return SubMenuOnline.sSingelton;
			}
		}

		// Token: 0x06001C9E RID: 7326 RVA: 0x000C6A18 File Offset: 0x000C4C18
		private static string GetModeString(GameType iMode)
		{
			switch (iMode)
			{
			case GameType.Campaign:
				return LanguageManager.Instance.GetString(SubMenu.LOC_ADVENTURE);
			case GameType.Challenge:
				return LanguageManager.Instance.GetString(SubMenuOnline.LOC_CHALLENGE);
			case (GameType)3:
				break;
			case GameType.Versus:
				return LanguageManager.Instance.GetString(SubMenu.LOC_VERSUS);
			default:
				if (iMode == GameType.Mythos)
				{
					return LanguageManager.Instance.GetString(SubMenu.LOC_MYTHOS);
				}
				if (iMode == GameType.StoryChallange)
				{
					return LanguageManager.Instance.GetString(SubMenuOnline.LOC_STORYCHALLANGE);
				}
				break;
			}
			return "Incorrect gametype!";
		}

		// Token: 0x06001C9F RID: 7327 RVA: 0x000C6AA0 File Offset: 0x000C4CA0
		public static string GetLevelString(string iLevel)
		{
			string[] array = iLevel.Split(new char[]
			{
				'|'
			});
			if (array == null || array.Length <= 1)
			{
				return iLevel;
			}
			GameType iMode = (GameType)byte.Parse(array[0]);
			string text = array[1];
			int num = 0;
			string text2;
			if (int.TryParse(text, out num))
			{
				if (num == SubMenuOnline.LOC_NO_LEVEL_SELECTED)
				{
					text2 = LanguageManager.Instance.GetString(SubMenuOnline.LOC_NO_LEVEL_SELECTED);
				}
				else
				{
					text2 = LevelManager.GetLocalizedName(text);
				}
			}
			else
			{
				text2 = LevelManager.GetLocalizedName(text);
			}
			if (text2 != null)
			{
				return text2;
			}
			return SubMenuOnline.GetModeString(iMode);
		}

		// Token: 0x06001CA0 RID: 7328 RVA: 0x000C6B20 File Offset: 0x000C4D20
		private SubMenuOnline()
		{
			VertexPositionColor[] array = new VertexPositionColor[Defines.QUAD_COL_VERTS_TL.Length];
			Defines.QUAD_COL_VERTS_TL.CopyTo(array, 0);
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Color.A = 127;
			}
			lock (Game.Instance.GraphicsDevice)
			{
				SubMenuOnline.sVertexBuffer = new VertexBuffer(Game.Instance.GraphicsDevice, Defines.QUAD_TEX_VERTS_TL.Length * VertexPositionTexture.SizeInBytes, BufferUsage.WriteOnly);
				SubMenuOnline.sVertexBuffer.SetData<VertexPositionTexture>(Defines.QUAD_TEX_VERTS_TL);
				SubMenuOnline.sVertexDeclaration = RenderManager.Instance.CreateVertexDeclaration(VertexPositionTexture.VertexElements);
				SubMenuOnline.sBackgroundVertexBuffer = new VertexBuffer(Game.Instance.GraphicsDevice, array.Length * VertexPositionColor.SizeInBytes, BufferUsage.WriteOnly);
				SubMenuOnline.sBackgroundVertexBuffer.SetData<VertexPositionColor>(array);
				SubMenuOnline.sBackgroundVertexDeclaration = RenderManager.Instance.CreateVertexDeclaration(VertexPositionColor.VertexElements);
			}
			this.mPasswordMessageBox = new TextInputMessageBox(SubMenuOnline.LOC_SETTINGS_PASSWORD, 10);
			this.mMenuTitle = new Text(30, FontManager.Instance.GetFont(MagickaFont.MenuTitle), TextAlign.Center, false);
			this.mMenuTitle.SetText(LanguageManager.Instance.GetString(SubMenu.LOC_ONLINE_PLAY));
			this.mMenuItems = new List<MenuItem>();
			Vector2 iPosition = new Vector2(160f, 752f);
			BitmapFont font = FontManager.Instance.GetFont(MagickaFont.Maiandra18);
			this.mTitleItems.Add(new MenuTextItem(SubMenuOnline.LOC_NAME, new Vector2(SubMenuOnline.LIST_HEADER_POSITION.X, SubMenuOnline.LIST_HEADER_POSITION.Y + 14f), font, TextAlign.Left));
			this.mTitleItems.Add(new MenuTextItem(SubMenuOnline.LOC_LEVEL, new Vector2(SubMenuOnline.LIST_HEADER_POSITION.X + 330f, SubMenuOnline.LIST_HEADER_POSITION.Y + 14f), font, TextAlign.Left));
			this.mTitleItems.Add(new MenuTextItem("###", new Vector2(SubMenuOnline.LIST_HEADER_POSITION.X + 630f, SubMenuOnline.LIST_HEADER_POSITION.Y + 14f), font, TextAlign.Left));
			this.mTitleItems.Add(new MenuTextItem(SubMenuOnline.LOC_PING, new Vector2(SubMenuOnline.LIST_HEADER_POSITION.X + 690f, SubMenuOnline.LIST_HEADER_POSITION.Y + 14f), font, TextAlign.Left));
			this.mSelectedTab = MagickaMath.CountTrailingZeroBits((uint)GlobalSettings.Instance.Filter.GameType);
			this.mTabTexts = new Text[3];
			for (int j = 0; j < this.mTabTexts.Length; j++)
			{
				this.mTabTexts[j] = new Text(64, font, TextAlign.Center, false);
			}
			this.mTabTexts[0].SetText(LanguageManager.Instance.GetString("#menu_main_01".GetHashCodeCustom()));
			this.mTabTexts[1].SetText(LanguageManager.Instance.GetString("#menu_main_02".GetHashCodeCustom()));
			this.mTabTexts[2].SetText(LanguageManager.Instance.GetString("#menu_main_03".GetHashCodeCustom()));
			RowItem[] array2 = new RowItem[4];
			array2[0].Alignment = TextAlign.Left;
			array2[0].RelativePosition = 0f;
			array2[0].Text = "First";
			array2[1].Alignment = TextAlign.Left;
			array2[1].RelativePosition = 0.38f;
			array2[1].Text = "Second";
			array2[2].Alignment = TextAlign.Right;
			array2[2].RelativePosition = 0.86f;
			array2[2].Text = "Third";
			array2[3].Alignment = TextAlign.Right;
			array2[3].RelativePosition = 0.96f;
			array2[3].Text = "Fourth";
			Vector2 iSize = new Vector2(724f, 35f);
			for (int k = 0; k < 12; k++)
			{
				this.mMenuItems.Add(new MenuTextRowItem(new Vector2(216f, 288f + iSize.Y * (float)k), iSize, font, array2));
				this.mMenuItems[k].Enabled = false;
			}
			this.mServerScroll = new MenuScrollBar(new Vector2(928f, SubMenuOnline.LIST_BG_POSITION.Y + SubMenuOnline.LIST_BG_SIZE.Y * 0.5f), SubMenuOnline.LIST_BG_SIZE.Y, 0);
			font = FontManager.Instance.GetFont(MagickaFont.MenuOption);
			iPosition.Y += 64f;
			iPosition.X = SubMenuOnline.LIST_BG_POSITION.X + SubMenuOnline.LIST_BG_SIZE.X * 0.5f;
			this.mMenuItems.Add(new MenuTextButtonItem(iPosition, SubMenu.sPagesTexture, MenuTextButtonItem.DEFAULT_UV_OFFSET, MenuTextButtonItem.DEFAULT_SIZE, SubMenuOnline.LOC_QUICK_START, font, 260f, 260f, TextAlign.Center));
			iPosition.X = SubMenuOnline.LIST_BG_POSITION.X;
			this.mMenuItems.Add(new MenuTextButtonItem(iPosition, SubMenu.sPagesTexture, MenuTextButtonItem.DEFAULT_UV_OFFSET, MenuTextButtonItem.DEFAULT_SIZE, SubMenuOnline.LOC_HOST, font, 260f, 260f, TextAlign.Left));
			iPosition.X = SubMenuOnline.LIST_BG_POSITION.X + SubMenuOnline.LIST_BG_SIZE.X;
			this.mMenuItems.Add(new MenuTextButtonItem(iPosition, SubMenu.sPagesTexture, MenuTextButtonItem.DEFAULT_UV_OFFSET, MenuTextButtonItem.DEFAULT_SIZE, SubMenuOnline.LOC_MANUAL, font, 260f, 260f, TextAlign.Right));
			iPosition.Y = SubMenu.BACK_POSITION.Y + 40f;
			this.mMenuItems.Add(new MenuTextButtonItem(iPosition, SubMenu.sPagesTexture, MenuTextButtonItem.DEFAULT_UV_OFFSET, MenuTextButtonItem.DEFAULT_SIZE, SubMenuOnline.LOC_JOIN, font, 260f, 260f, TextAlign.Right));
			this.mMenuItems.Add(new MenuImageTextItem(SubMenu.BACK_POSITION, SubMenu.sPagesTexture, SubMenu.BACK_UVOFFSET, SubMenu.BACK_UVSCALE, 0, default(Vector2), SubMenu.BACK_TEXT_ALIGN, FontManager.Instance.GetFont(SubMenu.BACK_FONT), SubMenu.BACK_SIZE));
			this.mFilteredServerList = new FilteredServerList();
			this.mSelectedServer = -1;
			this.mServers = this.mFilteredServerList.GetVisibleServers(GlobalSettings.Instance.Filter);
			ManualConnectMessageBox.Instance.Complete += new Action<IPAddress, string>(this.ManualConnectCallback);
			this.mFilterPanel = new FilterPanel();
			this.mFilterPanel.CreateFilterPanel();
			this.mFilterPanel.DoLayout(SubMenuOnline.LIST_BG_POSITION, SubMenuOnline.LIST_BG_SIZE);
			FilterPanel filterPanel = this.mFilterPanel;
			filterPanel.FilterDataChanged = (Action<FilterData>)Delegate.Combine(filterPanel.FilterDataChanged, new Action<FilterData>(this.OnFilterDataChanged));
			FilterPanel filterPanel2 = this.mFilterPanel;
			filterPanel2.RefreshRequest = (Action<FilterData>)Delegate.Combine(filterPanel2.RefreshRequest, new Action<FilterData>(this.OnRefreshRequest));
			this.mMenuItems.Add(this.mFilterPanel);
			this.mInputHelper = new DirectionalKeyboardHelper();
			DirectionalKeyboardHelper directionalKeyboardHelper = this.mInputHelper;
			directionalKeyboardHelper.DownPressed = (Action<Controller>)Delegate.Combine(directionalKeyboardHelper.DownPressed, new Action<Controller>(this.ControllerDown));
			DirectionalKeyboardHelper directionalKeyboardHelper2 = this.mInputHelper;
			directionalKeyboardHelper2.UpPressed = (Action<Controller>)Delegate.Combine(directionalKeyboardHelper2.UpPressed, new Action<Controller>(this.ControllerUp));
			DirectionalKeyboardHelper directionalKeyboardHelper3 = this.mInputHelper;
			directionalKeyboardHelper3.RightPressed = (Action<Controller>)Delegate.Combine(directionalKeyboardHelper3.RightPressed, new Action<Controller>(this.ControllerRight));
			DirectionalKeyboardHelper directionalKeyboardHelper4 = this.mInputHelper;
			directionalKeyboardHelper4.LeftPressed = (Action<Controller>)Delegate.Combine(directionalKeyboardHelper4.LeftPressed, new Action<Controller>(this.ControllerLeft));
			for (int l = 0; l < this.mTitleItems.Count; l++)
			{
				this.mMenuItems.Add(this.mTitleItems[l]);
			}
			this.mSelectedPosition = 12;
		}

		// Token: 0x06001CA1 RID: 7329 RVA: 0x000C72EC File Offset: 0x000C54EC
		private void OnFilterDataChanged(FilterData iFilter)
		{
			GlobalSettings.Instance.Filter = iFilter;
			this.mFilteredServerList.FilterList(iFilter);
		}

		// Token: 0x06001CA2 RID: 7330 RVA: 0x000C7305 File Offset: 0x000C5505
		private void OnRefreshRequest(FilterData iFilter)
		{
			GlobalSettings.Instance.Filter = iFilter;
			this.RefreshServerList();
		}

		// Token: 0x06001CA3 RID: 7331 RVA: 0x000C7318 File Offset: 0x000C5518
		internal void SortServerList(FilteredServerList.SortType iType)
		{
			this.mFilteredServerList.SortList(iType);
		}

		// Token: 0x06001CA4 RID: 7332 RVA: 0x000C7328 File Offset: 0x000C5528
		internal void RefreshServerList()
		{
			this.mSelectedServer = -1;
			this.mServerScroll.Value = 0;
			this.mServerTop = 0;
			for (int i = 0; i < 12; i++)
			{
				this.mMenuItems[i].Enabled = false;
			}
			lock (this.mServers)
			{
				this.mServers = this.mFilteredServerList.GetVisibleServers(GlobalSettings.Instance.Filter);
			}
		}

		// Token: 0x06001CA5 RID: 7333 RVA: 0x000C73B0 File Offset: 0x000C55B0
		public override void LanguageChanged()
		{
			this.mTabTexts[0].SetText(LanguageManager.Instance.GetString("#menu_main_01".GetHashCodeCustom()));
			this.mTabTexts[1].SetText(LanguageManager.Instance.GetString("#menu_main_02".GetHashCodeCustom()));
			this.mTabTexts[2].SetText(LanguageManager.Instance.GetString("#menu_main_03".GetHashCodeCustom()));
			for (int i = 12; i < this.mMenuItems.Count; i++)
			{
				this.mMenuItems[i].LanguageChanged();
			}
			this.mMenuTitle.SetText(LanguageManager.Instance.GetString(SubMenu.LOC_ONLINE_PLAY));
			this.UpdateServerList();
		}

		// Token: 0x06001CA6 RID: 7334 RVA: 0x000C7468 File Offset: 0x000C5668
		internal void OnAbortConnectToServer()
		{
			NetworkManager.Instance.EndSession();
		}

		// Token: 0x06001CA7 RID: 7335 RVA: 0x000C7474 File Offset: 0x000C5674
		internal void OnAbortedManualConnect()
		{
			NetworkManager.Instance.EndSession();
		}

		// Token: 0x06001CA8 RID: 7336 RVA: 0x000C7480 File Offset: 0x000C5680
		internal void OnAbortedQuickStart()
		{
			NetworkManager.Instance.EndSession();
		}

		// Token: 0x06001CA9 RID: 7337 RVA: 0x000C748C File Offset: 0x000C568C
		internal void OnRequestedLobbyList(LobbyMatchList iList)
		{
			if (NetworkManager.Instance.ConnectionStatus != ConnectionStatus.NotConnected)
			{
				return;
			}
			if (iList.mLobbiesMatching == 0)
			{
				WaitingMessageBox.Instance.Kill();
				NetworkHostMessageBox.Instance.Show(new NetworkHostMessageBox.Complete(this.OnHostGame));
				return;
			}
			SteamID lobbyByIndex = SteamMatchmaking.GetLobbyByIndex(1);
			uint num = 0U;
			ushort num2 = 0;
			SteamID iServerID = default(SteamID);
			bool lobbyGameServer = SteamMatchmaking.GetLobbyGameServer(lobbyByIndex, ref num, ref num2, ref iServerID);
			if (lobbyGameServer)
			{
				WaitingMessageBox.Instance.Show(SubMenuOnline.LOC_CONNECTING);
				WaitingMessageBox instance = WaitingMessageBox.Instance;
				instance.OnAbort = (Action)Delegate.Combine(instance.OnAbort, new Action(this.OnAbortConnectToServer));
				NetworkManager.Instance.ConnectToServer(iServerID, null, new Action<ConnectionStatus>(this.OnConnectToServer));
				return;
			}
			WaitingMessageBox.Instance.Kill();
			NetworkHostMessageBox.Instance.Show(new NetworkHostMessageBox.Complete(this.OnHostGame));
		}

		// Token: 0x06001CAA RID: 7338 RVA: 0x000C7564 File Offset: 0x000C5764
		internal void OnRequestedLobbyList_QuickJoin(LobbyMatchList iList)
		{
			if (NetworkManager.Instance.ConnectionStatus != ConnectionStatus.NotConnected)
			{
				return;
			}
			if (iList.mLobbiesMatching == 0)
			{
				if (SubMenuOnline._quickJoinSearchDistance == LobbyDistanceFilter.Worldwide)
				{
					WaitingMessageBox.Instance.Kill();
					SubMenuOnline._quickJoinSearchDistance = LobbyDistanceFilter.Default;
					SteamMatchmaking.AddRequestLobbyListDistanceFilter(LobbyDistanceFilter.Default);
					NetworkHostMessageBox.Instance.Show(new NetworkHostMessageBox.Complete(this.OnHostGame));
					return;
				}
				SubMenuOnline._quickJoinSearchDistance++;
				SteamMatchmaking.AddRequestLobbyListDistanceFilter(SubMenuOnline._quickJoinSearchDistance);
				SteamMatchmaking.AddRequestLobbyListNumericalFilter("slots", 1, LobbyComparison.EqualToOrGreaterThan);
				SteamMatchmaking.AddRequestLobbyListNumericalFilter("playing", 0, LobbyComparison.Equal);
				SteamMatchmaking.AddRequestLobbyListNumericalFilter("password", 0, LobbyComparison.Equal);
				SteamMatchmaking.AddRequestLobbyListStringFilter("version", Application.ProductVersion, LobbyComparison.Equal);
				SteamMatchmaking.AddRequestLobbyListResultCountFilter(10);
				SteamMatchmaking.RequestLobbyList(new Action<LobbyMatchList>(this.OnRequestedLobbyList_QuickJoin));
				return;
			}
			else
			{
				SteamID lobbyByIndex = SteamMatchmaking.GetLobbyByIndex(1);
				uint num = 0U;
				ushort num2 = 0;
				SteamID iServerID = default(SteamID);
				bool lobbyGameServer = SteamMatchmaking.GetLobbyGameServer(lobbyByIndex, ref num, ref num2, ref iServerID);
				if (lobbyGameServer)
				{
					WaitingMessageBox.Instance.Show(SubMenuOnline.LOC_CONNECTING);
					WaitingMessageBox instance = WaitingMessageBox.Instance;
					instance.OnAbort = (Action)Delegate.Combine(instance.OnAbort, new Action(this.OnAbortConnectToServer));
					NetworkManager.Instance.ConnectToServer(iServerID, null, new Action<ConnectionStatus>(this.OnConnectToServer));
					return;
				}
				WaitingMessageBox.Instance.Kill();
				NetworkHostMessageBox.Instance.Show(new NetworkHostMessageBox.Complete(this.OnHostGame));
				return;
			}
		}

		// Token: 0x06001CAB RID: 7339 RVA: 0x000C76B8 File Offset: 0x000C58B8
		private void OnHostGame(GameType iGameType, NetworkHostMessageBox.StoryChallengeType iStoryChallangeType, string iName, bool iVAC, string iPassword)
		{
			NetworkManager.Instance.SetupHost(iGameType, iName, iVAC, iPassword);
			switch (iGameType)
			{
			case GameType.Campaign:
			{
				SubMenuCampaignSelect_SaveSlotSelect instance = SubMenuCampaignSelect_SaveSlotSelect.Instance;
				instance.mComplete = (Action)Delegate.Combine(instance.mComplete, new Action(this.OnCampaignSelect));
				SubMenuCampaignSelect_SaveSlotSelect.Instance.GameType = GameType.Campaign;
				Tome.Instance.PushMenu(SubMenuCampaignSelect_SaveSlotSelect.Instance, 1);
				return;
			}
			case GameType.Challenge:
				this.QuickStartCharSelect_Challange();
				return;
			case (GameType)3:
				break;
			case GameType.Versus:
				this.QuickStartCharSelect_Versus();
				return;
			default:
				if (iGameType == GameType.Mythos)
				{
					SubMenuCampaignSelect_SaveSlotSelect instance2 = SubMenuCampaignSelect_SaveSlotSelect.Instance;
					instance2.mComplete = (Action)Delegate.Combine(instance2.mComplete, new Action(this.OnCampaignSelect));
					SubMenuCampaignSelect_SaveSlotSelect.Instance.GameType = GameType.Mythos;
					Tome.Instance.PushMenu(SubMenuCampaignSelect_SaveSlotSelect.Instance, 1);
					return;
				}
				if (iGameType == GameType.StoryChallange)
				{
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
					}
					this.QuickStartCharSelect_StoryChallange(null);
					return;
				}
				break;
			}
			throw new Exception("Invalid GameType!");
		}

		// Token: 0x06001CAC RID: 7340 RVA: 0x000C77F4 File Offset: 0x000C59F4
		private void ManualConnectCallback(IPAddress iAddress, string iPassword)
		{
			byte[] addressBytes = iAddress.GetAddressBytes();
			uint iIP = (uint)((int)addressBytes[0] << 24 | (int)addressBytes[1] << 16 | (int)addressBytes[2] << 8 | (int)addressBytes[3]);
			NetworkManager.Instance.ConnectToServer(iIP, iPassword, new Action<ConnectionStatus>(this.OnConnectToServer));
			WaitingMessageBox.Instance.Show(SubMenuOnline.LOC_CONNECTING);
			WaitingMessageBox instance = WaitingMessageBox.Instance;
			instance.OnAbort = (Action)Delegate.Combine(instance.OnAbort, new Action(this.OnAbortedManualConnect));
		}

		// Token: 0x06001CAD RID: 7341 RVA: 0x000C7870 File Offset: 0x000C5A70
		private void OnConnectToServer(ConnectionStatus iStatus)
		{
			WaitingMessageBox.Instance.Kill();
			if (NetworkManager.Instance.State == NetworkState.Offline)
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
				Tome.Instance.PushMenu(SubMenuCharacterSelect.Instance, 1);
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

		// Token: 0x06001CAE RID: 7342 RVA: 0x000C7994 File Offset: 0x000C5B94
		private void OnCampaignSelect()
		{
			if (NetworkManager.Instance.HasHostSettings)
			{
				string fileName = LevelManager.Instance.GetLevel(SubMenuCampaignSelect_SaveSlotSelect.Instance.GameType, (int)SubMenuCampaignSelect_SaveSlotSelect.Instance.CurrentSaveData.Level).FileName;
				NetworkManager.Instance.SetGame(GameType.Campaign, fileName);
				NetworkManager.Instance.StartHost();
				NetworkManager.Instance.SetGame(GameType.Campaign, fileName);
			}
			SubMenuCampaignSelect_SaveSlotSelect instance = SubMenuCampaignSelect_SaveSlotSelect.Instance;
			instance.mComplete = (Action)Delegate.Remove(instance.mComplete, new Action(this.OnCampaignSelect));
		}

		// Token: 0x06001CAF RID: 7343 RVA: 0x000C7A20 File Offset: 0x000C5C20
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
			Tome.Instance.PushMenu(SubMenuCharacterSelect.Instance, 1);
		}

		// Token: 0x06001CB0 RID: 7344 RVA: 0x000C7A88 File Offset: 0x000C5C88
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
			Tome.Instance.PushMenu(SubMenuCharacterSelect.Instance, 1);
		}

		// Token: 0x06001CB1 RID: 7345 RVA: 0x000C7AF0 File Offset: 0x000C5CF0
		private void QuickStartCharSelect_StoryChallange(string specificLevel)
		{
			if (NetworkManager.Instance.HasHostSettings)
			{
				string iLevelName;
				if (string.IsNullOrEmpty(specificLevel))
				{
					iLevelName = SubMenuOnline.LOC_NO_LEVEL_SELECTED.ToString();
				}
				else
				{
					iLevelName = LevelManager.GetLocalizedName(specificLevel);
				}
				NetworkManager.Instance.SetGame(GameType.Campaign, iLevelName);
				NetworkManager.Instance.StartHost();
				NetworkManager.Instance.SetGame(GameType.Campaign, iLevelName);
			}
			if (string.IsNullOrEmpty(specificLevel))
			{
				SubMenuCharacterSelect.Instance.SetSettings(GameType.StoryChallange, -1, false);
			}
			else
			{
				SubMenuCharacterSelect.Instance.SetSettings(GameType.StoryChallange, specificLevel, false);
			}
			Tome.Instance.PushMenu(SubMenuCharacterSelect.Instance, 1);
		}

		// Token: 0x06001CB2 RID: 7346 RVA: 0x000C7B80 File Offset: 0x000C5D80
		public override void Draw(Viewport iLeftSide, Viewport iRightSide)
		{
			this.mEffect.GraphicsDevice.Viewport = iRightSide;
			this.mEffect.Begin();
			this.mEffect.CurrentTechnique.Passes[0].Begin();
			this.mEffect.VertexColorEnabled = false;
			this.mEffect.Color = Defines.MESSAGEBOX_COLOR_DEFAULT;
			if (this.mMenuTitle != null)
			{
				this.mMenuTitle.Draw(this.mEffect, 512f, 96f);
			}
			this.mEffect.GraphicsDevice.Vertices[0].SetSource(SubMenuOnline.sBackgroundVertexBuffer, 0, VertexPositionColor.SizeInBytes);
			this.mEffect.GraphicsDevice.VertexDeclaration = SubMenuOnline.sBackgroundVertexDeclaration;
			this.mEffect.TextureEnabled = false;
			this.mEffect.VertexColorEnabled = true;
			this.mEffect.Saturation = 1f;
			this.mEffect.Color = new Vector4(1f, 1f, 1f, 0.5f);
			Matrix identity = Matrix.Identity;
			identity.M11 = SubMenuOnline.LIST_BG_SIZE.X;
			identity.M22 = SubMenuOnline.LIST_BG_SIZE.Y;
			identity.M41 = SubMenuOnline.LIST_BG_POSITION.X;
			identity.M42 = SubMenuOnline.LIST_BG_POSITION.Y;
			this.mEffect.Transform = identity;
			this.mEffect.CommitChanges();
			this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
			this.mEffect.GraphicsDevice.Vertices[0].SetSource(SubMenuOnline.sVertexBuffer, 0, VertexPositionTexture.SizeInBytes);
			this.mEffect.GraphicsDevice.VertexDeclaration = SubMenuOnline.sVertexDeclaration;
			this.mEffect.TextureEnabled = true;
			this.mEffect.Texture = SubMenu.sPagesTexture;
			this.mEffect.Color = Vector4.One;
			this.mEffect.Saturation = 1f;
			this.mEffect.VertexColorEnabled = false;
			Vector2 textureOffset = default(Vector2);
			textureOffset.X = 0f / (float)SubMenu.sPagesTexture.Width;
			textureOffset.Y = 384f / (float)SubMenu.sPagesTexture.Height;
			Vector2 textureScale = default(Vector2);
			textureScale.X = 320f / (float)SubMenu.sPagesTexture.Width;
			textureScale.Y = 80f / (float)SubMenu.sPagesTexture.Height;
			this.mEffect.TextureOffset = textureOffset;
			this.mEffect.TextureScale = textureScale;
			identity.M11 = 320f;
			identity.M22 = 80f;
			identity.M41 = SubMenuOnline.LIST_TOP_POSITION.X;
			identity.M42 = SubMenuOnline.LIST_TOP_POSITION.Y + 4f;
			for (int i = 0; i < 3; i++)
			{
				if (i != this.mSelectedTab)
				{
					if (i == this.mHighlitTab)
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
			this.mEffect.Color = MenuItem.COLOR;
			Vector2 vector = default(Vector2);
			vector.X = SubMenuOnline.LIST_TOP_POSITION.X + 128f + 32f;
			vector.Y = SubMenuOnline.LIST_TOP_POSITION.Y + 22f;
			for (int j = 0; j < 3; j++)
			{
				if (j != this.mSelectedTab)
				{
					this.mEffect.Color = ((this.mHighlitTab == j) ? MenuItem.COLOR_SELECTED : MenuItem.COLOR);
					this.mTabTexts[j].Draw(this.mEffect, vector.X, vector.Y + 4f);
				}
				vector.X += 256f;
			}
			this.mEffect.GraphicsDevice.Vertices[0].SetSource(SubMenuOnline.sVertexBuffer, 0, VertexPositionTexture.SizeInBytes);
			this.mEffect.GraphicsDevice.VertexDeclaration = SubMenuOnline.sVertexDeclaration;
			this.mEffect.Texture = SubMenu.sPagesTexture;
			this.mEffect.Color = Vector4.One;
			this.mEffect.TextureOffset = new Vector2(0f / (float)SubMenu.sPagesTexture.Width, 240f / (float)SubMenu.sPagesTexture.Height);
			this.mEffect.TextureScale = new Vector2(SubMenuOnline.LIST_TOP_SIZE.X / (float)SubMenu.sPagesTexture.Width, SubMenuOnline.LIST_TOP_SIZE.Y / (float)SubMenu.sPagesTexture.Height);
			identity.M11 = SubMenuOnline.LIST_TOP_SIZE.X;
			identity.M22 = SubMenuOnline.LIST_TOP_SIZE.Y;
			identity.M41 = SubMenuOnline.LIST_TOP_POSITION.X;
			identity.M42 = SubMenuOnline.LIST_TOP_POSITION.Y;
			this.mEffect.Transform = identity;
			this.mEffect.CommitChanges();
			this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
			this.mEffect.TextureOffset = new Vector2(0f / (float)SubMenu.sPagesTexture.Width, 384f / (float)SubMenu.sPagesTexture.Height);
			this.mEffect.TextureScale = new Vector2(320f / (float)SubMenu.sPagesTexture.Width, 80f / (float)SubMenu.sPagesTexture.Height);
			identity.M11 = 320f;
			identity.M22 = 80f;
			identity.M41 = SubMenuOnline.LIST_TOP_POSITION.X + 256f * (float)this.mSelectedTab;
			identity.M42 = SubMenuOnline.LIST_TOP_POSITION.Y;
			this.mEffect.Transform = identity;
			this.mEffect.CommitChanges();
			this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
			this.mEffect.Color = ((this.mHighlitTab == this.mSelectedTab && this.mSelectedPosition < 0) ? MenuItem.COLOR_SELECTED : MenuItem.COLOR);
			this.mTabTexts[this.mSelectedTab].Draw(this.mEffect, SubMenuOnline.LIST_TOP_POSITION.X + 128f + 32f + 256f * (float)this.mSelectedTab, vector.Y);
			this.mEffect.GraphicsDevice.Vertices[0].SetSource(null, 0, 0);
			for (int k = 12; k < this.mMenuItems.Count; k++)
			{
				this.mMenuItems[k].Draw(this.mEffect);
			}
			lock (this.mServers)
			{
				this.UpdateServerList();
				for (int l = 0; l < 12; l++)
				{
					if (this.mMenuItems[l].Enabled && this.mServerTop + l < this.mServers.Count)
					{
						if (this.mServersSelected)
						{
							this.mMenuItems[l].Selected = true;
						}
						GameServerItem gameServerItem = this.mServers[this.mServerTop + l];
						this.mMenuItems[l].Enabled = !gameServerItem.Playing();
						this.mMenuItems[l].Draw(this.mEffect);
						if (gameServerItem.m_Password)
						{
							this.mEffect.GraphicsDevice.Vertices[0].SetSource(SubMenuOnline.sVertexBuffer, 0, VertexPositionTexture.SizeInBytes);
							this.mEffect.GraphicsDevice.VertexDeclaration = SubMenuOnline.sVertexDeclaration;
							this.mEffect.TextureEnabled = true;
							this.mEffect.Texture = SubMenu.sPagesTexture;
							this.mEffect.Color = Vector4.One;
							this.mEffect.Saturation = 1f;
							this.mEffect.VertexColorEnabled = false;
							this.mEffect.TextureOffset = new Vector2(1056f / (float)SubMenu.sPagesTexture.Width, 128f / (float)SubMenu.sPagesTexture.Height);
							this.mEffect.TextureScale = new Vector2(32f / (float)SubMenu.sPagesTexture.Width, 32f / (float)SubMenu.sPagesTexture.Height);
							identity.M11 = 32f;
							identity.M22 = 32f;
							identity.M41 = this.mMenuItems[l].Position.X - 32f;
							identity.M42 = this.mMenuItems[l].Position.Y;
							this.mEffect.Transform = identity;
							this.mEffect.CommitChanges();
							this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
							this.mEffect.TextureScale = new Vector2(1f, 1f);
							this.mEffect.TextureOffset = Vector2.Zero;
						}
						if (gameServerItem.m_Secure)
						{
							this.mEffect.GraphicsDevice.Vertices[0].SetSource(SubMenuOnline.sVertexBuffer, 0, VertexPositionTexture.SizeInBytes);
							this.mEffect.GraphicsDevice.VertexDeclaration = SubMenuOnline.sVertexDeclaration;
							this.mEffect.TextureEnabled = true;
							this.mEffect.Texture = SubMenu.sPagesTexture;
							this.mEffect.Color = Vector4.One;
							this.mEffect.Saturation = 1f;
							this.mEffect.VertexColorEnabled = false;
							this.mEffect.TextureOffset = new Vector2(1024f / (float)SubMenu.sPagesTexture.Width, 128f / (float)SubMenu.sPagesTexture.Height);
							this.mEffect.TextureScale = new Vector2(32f / (float)SubMenu.sPagesTexture.Width, 32f / (float)SubMenu.sPagesTexture.Height);
							identity.M11 = 32f;
							identity.M22 = 32f;
							identity.M41 = this.mMenuItems[l].Position.X - 64f;
							identity.M42 = this.mMenuItems[l].Position.Y;
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

		// Token: 0x06001CB3 RID: 7347 RVA: 0x000C8704 File Offset: 0x000C6904
		private void OnPasswordInput(string iPassword)
		{
			if (this.mSelectedServer >= 0)
			{
				if (string.IsNullOrEmpty(iPassword))
				{
					return;
				}
				WaitingMessageBox.Instance.Show(SubMenuOnline.LOC_CONNECTING);
				WaitingMessageBox instance = WaitingMessageBox.Instance;
				instance.OnAbort = (Action)Delegate.Combine(instance.OnAbort, new Action(this.OnAbortConnectToServer));
				NetworkManager.Instance.ConnectToServer(this.mServers[this.mServerTop + this.mSelectedServer].ServerID, iPassword, new Action<ConnectionStatus>(this.OnConnectToServer));
			}
		}

		// Token: 0x06001CB4 RID: 7348 RVA: 0x000C878C File Offset: 0x000C698C
		public override void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			base.Update(iDataChannel, iDeltaTime);
			if (this.mServerTop != this.mServerScroll.Value)
			{
				this.UpdateServerScroll();
			}
			for (int i = 0; i < 12; i++)
			{
				this.mMenuItems[i].Selected = (this.mSelectedServer == i | this.mSelectedPosition == i);
			}
			this.mMenuItems[15].Enabled = (this.mSelectedServer >= 0 & this.mSelectedServer < 12);
			this.mFilterPanel.Update(iDeltaTime);
		}

		// Token: 0x06001CB5 RID: 7349 RVA: 0x000C8820 File Offset: 0x000C6A20
		private void UpdateServerScroll()
		{
			lock (this.mServers)
			{
				int num = this.mServerScroll.Value;
				int num2 = 0;
				while (num2 < 12 && num < this.mServers.Count)
				{
					this.SetServerItem(num2, num);
					num++;
					num2++;
				}
			}
			this.mSelectedServer = this.mSelectedPosition;
			this.mServerTop = this.mServerScroll.Value;
		}

		// Token: 0x06001CB6 RID: 7350 RVA: 0x000C88A4 File Offset: 0x000C6AA4
		private void UpdateServerList()
		{
			int num = this.mServerScroll.Value;
			int num2 = 0;
			while (num2 < 12 && num < this.mServers.Count)
			{
				if (num < this.mServers.Count)
				{
					this.SetServerItem(num2, num);
					this.mMenuItems[num2].Enabled = true;
				}
				else if (num >= this.mServers.Count)
				{
					this.mMenuItems[num2].Enabled = false;
				}
				num++;
				num2++;
			}
			int value = this.mServerScroll.Value;
			this.mServerScroll.SetMaxValue(Math.Max(0, this.mServers.Count - 12));
			this.mServerScroll.Value = value;
		}

		// Token: 0x06001CB7 RID: 7351 RVA: 0x000C8960 File Offset: 0x000C6B60
		private void SetServerItem(int iMenuItem, int iServer)
		{
			if (string.IsNullOrEmpty(this.mServers[iServer].m_Name))
			{
				(this.mMenuItems[iMenuItem] as MenuTextRowItem).SetItemText(0, "");
			}
			else
			{
				string text = this.mServers[iServer].m_Name;
				BitmapFont font = FontManager.Instance.GetFont(MagickaFont.Maiandra18);
				for (float x = font.MeasureText(text, true).X; x > 265f; x = font.MeasureText(text, true).X)
				{
					text = text.Substring(0, text.Length - 1);
				}
				(this.mMenuItems[iMenuItem] as MenuTextRowItem).SetItemText(0, text);
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
			(this.mMenuItems[iMenuItem] as MenuTextRowItem).SetItemText(2, string.Format("{0}/{1}", this.mServers[iServer].m_Players, this.mServers[iServer].m_MaxPlayers));
			(this.mMenuItems[iMenuItem] as MenuTextRowItem).SetItemText(3, this.mServers[iServer].m_Ping.ToString());
		}

		// Token: 0x06001CB8 RID: 7352 RVA: 0x000C8AF0 File Offset: 0x000C6CF0
		public override void ControllerMouseMove(Controller iSender, Point iScreenSize, MouseState iState, MouseState iOldState)
		{
			Vector2 vector;
			bool flag;
			if (Tome.MousePickTome(iScreenSize, iState.X, iState.Y, out vector, out flag))
			{
				if (flag)
				{
					string iToolTip = null;
					if (this.mFilterPanel.CheckMouseMove(ref vector, ref iToolTip))
					{
						this.mSelectedPosition = 17;
						this.SetToolTip(iToolTip, iState);
						return;
					}
					if (this.mServerScroll.Grabbed)
					{
						this.mServerScroll.ScrollTo(vector.Y);
					}
					else
					{
						int num = -1;
						if (vector.Y >= SubMenuOnline.LIST_TOP_POSITION.Y && vector.Y <= SubMenuOnline.LIST_TOP_POSITION.Y + 48f)
						{
							int i = 0;
							while (i < 3)
							{
								if (vector.X >= SubMenuOnline.LIST_TOP_POSITION.X + 16f + (float)i * (SubMenuOnline.LIST_TOP_SIZE.X - 32f) / 3f && vector.X <= SubMenuOnline.LIST_TOP_POSITION.X + 16f + ((float)i + 1f) * (SubMenuOnline.LIST_TOP_SIZE.X - 32f) / 3f)
								{
									if (i != this.mSelectedTab)
									{
										num = i;
										break;
									}
									break;
								}
								else
								{
									i++;
								}
							}
						}
						this.mHighlitTab = num;
						bool flag2 = false;
						for (int j = 12; j < this.mMenuItems.Count; j++)
						{
							MenuItem menuItem = this.mMenuItems[j];
							if (menuItem.Enabled && menuItem.InsideBounds(ref vector))
							{
								if (this.mSelectedPosition != j)
								{
									AudioManager.Instance.PlayCue(Banks.UI, "ui_menu_mouseover".GetHashCodeCustom());
								}
								this.mSelectedPosition = j;
								for (int k = 0; k < this.mMenuItems.Count; k++)
								{
									this.mMenuItems[k].Selected = false;
								}
								this.mServersSelected = false;
								menuItem.Selected = true;
								flag2 = true;
								break;
							}
						}
						if (!flag2 && this.mSelectedPosition >= 12)
						{
							for (int l = 0; l < this.mMenuItems.Count; l++)
							{
								this.mMenuItems[l].Selected = false;
							}
							this.mSelectedPosition = -1;
							this.mServersSelected = false;
						}
					}
				}
				this.SetToolTip(null, iState);
				return;
			}
			ToolTipMan.Instance.Kill(null, false);
		}

		// Token: 0x06001CB9 RID: 7353 RVA: 0x000C8D46 File Offset: 0x000C6F46
		private void SetToolTip(string iToolTip, MouseState iState)
		{
			this.SetToolTip(iToolTip, new Vector2((float)iState.X, (float)iState.Y + 32f));
		}

		// Token: 0x06001CBA RID: 7354 RVA: 0x000C8D6C File Offset: 0x000C6F6C
		private void SetToolTip(string iToolTip, Vector2 iPos)
		{
			if (!string.IsNullOrEmpty(iToolTip))
			{
				ToolTipMan.Instance.Set(null, iToolTip, ref iPos);
				return;
			}
			switch (this.mSelectedPosition)
			{
			case 12:
				ToolTipMan.Instance.Set(null, LanguageManager.Instance.GetString(SubMenuOnline.LOC_TT_QUICK), ref iPos);
				return;
			case 13:
				ToolTipMan.Instance.Set(null, LanguageManager.Instance.GetString(SubMenuOnline.LOC_TT_HOST), ref iPos);
				return;
			case 14:
				ToolTipMan.Instance.Set(null, LanguageManager.Instance.GetString(SubMenuOnline.LOC_TT_IP), ref iPos);
				return;
			case 15:
				ToolTipMan.Instance.Set(null, LanguageManager.Instance.GetString(SubMenuOnline.LOC_TT_JOIN), ref iPos);
				return;
			case 17:
			{
				string toolTip = this.mFilterPanel.GetToolTip(out iPos);
				Vector2 vector;
				Tome.Instance.PageToScreen(true, ref iPos, out vector);
				vector.Y += 32f;
				if (toolTip != null)
				{
					ToolTipMan.Instance.Set(null, toolTip, ref vector);
					return;
				}
				return;
			}
			case 18:
				ToolTipMan.Instance.Set(null, LanguageManager.Instance.GetString(SubMenuOnline.LOC_TT_NAME), ref iPos);
				return;
			case 19:
				ToolTipMan.Instance.Set(null, LanguageManager.Instance.GetString(SubMenuOnline.LOC_TT_LEVEL), ref iPos);
				return;
			case 20:
				ToolTipMan.Instance.Set(null, LanguageManager.Instance.GetString(SubMenuOnline.LOC_TT_PLAYERS), ref iPos);
				return;
			case 21:
				ToolTipMan.Instance.Set(null, LanguageManager.Instance.GetString(SubMenuOnline.LOC_TT_PING), ref iPos);
				return;
			}
			ToolTipMan.Instance.Kill(null, false);
		}

		// Token: 0x06001CBB RID: 7355 RVA: 0x000C8F08 File Offset: 0x000C7108
		public override void ControllerEvent(Controller iSender, KeyboardState iOldState, KeyboardState iNewState)
		{
			this.mInputHelper.Update(iSender, iOldState, iNewState);
		}

		// Token: 0x06001CBC RID: 7356 RVA: 0x000C8F18 File Offset: 0x000C7118
		private void SetControllerToolTip()
		{
			if (this.mSelectedPosition >= 12 && this.mSelectedPosition < this.mMenuItems.Count)
			{
				Vector2 topLeft = this.mMenuItems[this.mSelectedPosition].TopLeft;
				Vector2 iPos;
				Tome.Instance.PageToScreen(true, ref topLeft, out iPos);
				iPos.Y += 32f;
				this.SetToolTip(null, iPos);
			}
		}

		// Token: 0x06001CBD RID: 7357 RVA: 0x000C8F84 File Offset: 0x000C7184
		public override void ControllerUp(Controller iSender)
		{
			ToolTipMan.Instance.Kill(null, false);
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
				case 16:
					this.mSelectedPosition = 13;
					break;
				case 17:
					if (this.mFilterPanel.ControllUpOver())
					{
						if (this.mServers.Count > 0)
						{
							this.mServersSelected = true;
							this.mSelectedPosition = -1;
						}
						else
						{
							this.mSelectedPosition = 18;
							this.mHighlitTab = this.mSelectedTab;
						}
					}
					break;
				default:
					if (this.mHighlitTab >= 0)
					{
						int num = this.mSelectedPosition - 18;
						this.mSelectedPosition = -1;
						if (num != this.mSelectedTab)
						{
							this.mHighlitTab = (int)MathHelper.Clamp((float)num, 0f, 2f);
						}
					}
					else if (this.mSelectedPosition == 0 && this.mServerScroll.Value > 0)
					{
						int num2 = Math.Max(6, 6 - this.mServerScroll.Value);
						this.mSelectedPosition += num2;
						this.mServerScroll.Value -= num2;
					}
					else if (this.mSelectedPosition != 0 || this.mServerScroll.Value != 0)
					{
						base.ControllerUp(iSender);
						this.mSelectedServer = this.mSelectedPosition;
					}
					break;
				}
			}
			this.SetControllerToolTip();
		}

		// Token: 0x06001CBE RID: 7358 RVA: 0x000C915C File Offset: 0x000C735C
		public override void ControllerDown(Controller iSender)
		{
			ToolTipMan.Instance.Kill(null, false);
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
					}
					else
					{
						this.mSelectedPosition = 16;
					}
					break;
				case 13:
					this.mSelectedPosition = 16;
					break;
				case 14:
					if (this.mMenuItems[15].Enabled)
					{
						this.mSelectedPosition = 15;
					}
					else
					{
						this.mSelectedPosition = 16;
					}
					break;
				case 15:
				case 16:
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
					}
					else if (this.mSelectedPosition >= 11)
					{
						if (this.mServers.Count - (this.mServerScroll.Value + this.mSelectedPosition + 1) != 0)
						{
							int val = this.mServers.Count - (this.mServerScroll.Value + this.mSelectedPosition + 1);
							int num = Math.Min(6, val);
							this.mSelectedPosition -= num;
							this.mSelectedServer = this.mSelectedPosition;
							this.mServerScroll.Value += num;
						}
					}
					else if (this.mSelectedPosition < this.mServers.Count - 1)
					{
						base.ControllerDown(iSender);
						this.mSelectedServer = this.mSelectedPosition;
					}
					break;
				}
			}
			this.SetControllerToolTip();
		}

		// Token: 0x06001CBF RID: 7359 RVA: 0x000C93C0 File Offset: 0x000C75C0
		public override void ControllerLeft(Controller iSender)
		{
			ToolTipMan.Instance.Kill(null, false);
			switch (this.mSelectedPosition)
			{
			case 12:
				this.mSelectedPosition = 13;
				break;
			case 13:
			case 16:
			case 18:
				break;
			case 14:
				this.mSelectedPosition = 12;
				break;
			case 15:
				this.mSelectedPosition = 16;
				break;
			case 17:
				this.mFilterPanel.ControllLeftOver();
				break;
			case 19:
			case 20:
			case 21:
				this.mSelectedPosition--;
				break;
			default:
				if (this.mServersSelected)
				{
					if (this.mSelectedTab > 0)
					{
						this.SwitchToTab(this.mSelectedTab - 1);
					}
				}
				else if (this.mSelectedPosition == -1)
				{
					if (this.mHighlitTab > 0)
					{
						this.mHighlitTab--;
					}
				}
				else if (this.mSelectedTab > 0)
				{
					this.SwitchToTab(this.mSelectedTab - 1);
					this.mSelectedServer = (this.mSelectedPosition = 0);
				}
				break;
			}
			this.SetControllerToolTip();
		}

		// Token: 0x06001CC0 RID: 7360 RVA: 0x000C94C8 File Offset: 0x000C76C8
		public override void ControllerRight(Controller iSender)
		{
			ToolTipMan.Instance.Kill(null, false);
			switch (this.mSelectedPosition)
			{
			case 12:
				this.mSelectedPosition = 14;
				break;
			case 13:
				this.mSelectedPosition = 12;
				break;
			case 14:
			case 15:
			case 21:
				break;
			case 16:
				if (this.mMenuItems[15].Enabled)
				{
					this.mSelectedPosition = 15;
				}
				break;
			case 17:
				this.mFilterPanel.ControllRightOver();
				break;
			case 18:
			case 19:
			case 20:
				this.mSelectedPosition++;
				break;
			default:
				if (this.mServersSelected)
				{
					if (this.mSelectedTab < 2)
					{
						this.SwitchToTab(this.mSelectedTab + 1);
					}
				}
				else if (this.mSelectedPosition == -1)
				{
					if (this.mHighlitTab < 2)
					{
						this.mHighlitTab++;
					}
				}
				else if (this.mSelectedTab < 2)
				{
					this.SwitchToTab(this.mSelectedTab + 1);
					this.mSelectedServer = (this.mSelectedPosition = 0);
				}
				break;
			}
			this.SetControllerToolTip();
		}

		// Token: 0x06001CC1 RID: 7361 RVA: 0x000C95E8 File Offset: 0x000C77E8
		public override void ControllerMouseAction(Controller iSender, Point iScreenSize, MouseState iState, MouseState iOldState)
		{
			Vector2 iPoint;
			bool flag;
			if (!Tome.MousePickTome(iScreenSize, iState.X, iState.Y, out iPoint, out flag))
			{
				return;
			}
			if (!flag)
			{
				return;
			}
			ToolTipMan.Instance.Kill(null, false);
			this.mServerScroll.Grabbed = false;
			if (iState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
			{
				if (this.mServerScroll.InsideBounds(iPoint.X, iPoint.Y))
				{
					if (this.mServerScroll.InsideDragBounds(iPoint))
					{
						this.mServerScroll.Grabbed = true;
						return;
					}
					if (this.mServerScroll.InsideDragUpBounds(iPoint))
					{
						this.mServerScroll.Value--;
						return;
					}
					if (this.mServerScroll.InsideDragDownBounds(iPoint))
					{
						this.mServerScroll.Value++;
						return;
					}
					this.mServerScroll.ScrollTo(iPoint.Y);
					return;
				}
			}
			else
			{
				if (iState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released && iOldState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed && this.mFilterPanel.CheckMouseAction(ref iPoint))
				{
					return;
				}
				for (int i = 0; i < this.mTitleItems.Count; i++)
				{
					if (this.mTitleItems[i].InsideBounds(iPoint.X, iPoint.Y))
					{
						this.SortServers(6 + i);
						return;
					}
				}
				if (this.mServerScroll.InsideBounds(iPoint.X, iPoint.Y))
				{
					if (iState.ScrollWheelValue != iOldState.ScrollWheelValue)
					{
						if (this.mServerScroll.Value > 0 && iState.ScrollWheelValue > iOldState.ScrollWheelValue)
						{
							this.mServerScroll.Value--;
							return;
						}
						if (this.mServerScroll.Value < this.mServers.Count && iState.ScrollWheelValue < iOldState.ScrollWheelValue)
						{
							this.mServerScroll.Value++;
							return;
						}
					}
				}
				else
				{
					if (iPoint.Y >= SubMenuOnline.LIST_TOP_POSITION.Y && iPoint.Y <= SubMenuOnline.LIST_TOP_POSITION.Y + 48f)
					{
						for (int j = 0; j < 3; j++)
						{
							if (iPoint.X >= SubMenuOnline.LIST_TOP_POSITION.X + 16f + (float)j * (SubMenuOnline.LIST_TOP_SIZE.X - 32f) / 3f && iPoint.X <= SubMenuOnline.LIST_TOP_POSITION.X + 16f + ((float)j + 1f) * (SubMenuOnline.LIST_TOP_SIZE.X - 32f) / 3f)
							{
								this.SwitchToTab(j);
								return;
							}
						}
						return;
					}
					bool flag2 = false;
					int k = 0;
					while (k < this.mMenuItems.Count)
					{
						MenuItem menuItem = this.mMenuItems[k];
						if (menuItem.Enabled && menuItem.InsideBounds(ref iPoint))
						{
							if (k < 12 && iState.ScrollWheelValue != iOldState.ScrollWheelValue)
							{
								if (this.mServerScroll.Value > 0 && iState.ScrollWheelValue > iOldState.ScrollWheelValue)
								{
									this.mServerScroll.Value--;
									break;
								}
								if (this.mServerScroll.Value < this.mServers.Count && iState.ScrollWheelValue < iOldState.ScrollWheelValue)
								{
									this.mServerScroll.Value++;
									break;
								}
								break;
							}
							else
							{
								if ((iState.LeftButton != Microsoft.Xna.Framework.Input.ButtonState.Released || iOldState.LeftButton != Microsoft.Xna.Framework.Input.ButtonState.Pressed) && (iState.RightButton != Microsoft.Xna.Framework.Input.ButtonState.Released || iOldState.RightButton != Microsoft.Xna.Framework.Input.ButtonState.Pressed))
								{
									break;
								}
								for (int l = 0; l < this.mMenuItems.Count; l++)
								{
									this.mMenuItems[l].Selected = false;
								}
								flag2 = true;
								if (k < 12)
								{
									if (this.mSelectedPosition == k)
									{
										this.ControllerMouseClicked(iSender);
									}
									else
									{
										this.mSelectedPosition = k;
									}
									this.mSelectedServer = k;
									break;
								}
								this.mSelectedPosition = k;
								this.ControllerMouseClicked(iSender);
								break;
							}
						}
						else
						{
							k++;
						}
					}
					if (!flag2)
					{
						this.mSelectedPosition = -1;
						this.mSelectedServer = -1;
					}
				}
			}
		}

		// Token: 0x06001CC2 RID: 7362 RVA: 0x000C9A10 File Offset: 0x000C7C10
		private void SortServers(int iSelectedIndex)
		{
			switch (iSelectedIndex)
			{
			case 6:
				this.SortServerList(FilteredServerList.SortType.Name);
				return;
			case 7:
				this.SortServerList(FilteredServerList.SortType.Level);
				return;
			case 8:
				this.SortServerList(FilteredServerList.SortType.Players);
				return;
			case 9:
				this.SortServerList(FilteredServerList.SortType.Ping);
				return;
			default:
				return;
			}
		}

		// Token: 0x06001CC3 RID: 7363 RVA: 0x000C9A58 File Offset: 0x000C7C58
		private void SwitchToTab(int i)
		{
			this.mSelectedTab = i;
			FilterData filter = GlobalSettings.Instance.Filter;
			filter.GameType = (GameType)(1 << i);
			GlobalSettings.Instance.Filter = filter;
			this.mSelectedPosition = -1;
			this.RefreshServerList();
		}

		// Token: 0x06001CC4 RID: 7364 RVA: 0x000C9AA0 File Offset: 0x000C7CA0
		public override void ControllerB(Controller iSender)
		{
			if (DaisyWheel.IsDisplaying)
			{
				return;
			}
			ToolTipMan.Instance.Kill(null, false);
			if (this.mSelectedPosition == 17)
			{
				if (!this.mFilterPanel.ControllB())
				{
					Tome.Instance.PopMenu();
					return;
				}
			}
			else
			{
				if (this.mSelectedPosition >= 0 && this.mSelectedPosition < 12)
				{
					this.mServersSelected = true;
					this.mSelectedServer = (this.mSelectedPosition = -1);
					GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Bottom, SubMenu.LOC_SELECT);
					return;
				}
				Tome.Instance.PopMenu();
			}
		}

		// Token: 0x06001CC5 RID: 7365 RVA: 0x000C9B2C File Offset: 0x000C7D2C
		public override void ControllerA(Controller iSender)
		{
			ToolTipMan.Instance.Kill(null, false);
			if (this.mServersSelected)
			{
				GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Bottom, SubMenuOnline.LOC_JOIN);
				this.mServersSelected = false;
				this.mSelectedPosition = 0;
				for (int i = 1; i < 12; i++)
				{
					if (this.mMenuItems[i].Enabled && this.mServerTop + i < this.mServers.Count)
					{
						this.mMenuItems[i].Selected = false;
					}
				}
				return;
			}
			if (this.mHighlitTab < 0)
			{
				switch (this.mSelectedPosition)
				{
				case 12:
				{
					SubMenuOnline._quickJoinSearchDistance = LobbyDistanceFilter.Close;
					SteamMatchmaking.AddRequestLobbyListDistanceFilter(LobbyDistanceFilter.Close);
					SteamMatchmaking.AddRequestLobbyListNumericalFilter("slots", 1, LobbyComparison.EqualToOrGreaterThan);
					SteamMatchmaking.AddRequestLobbyListNumericalFilter("playing", 0, LobbyComparison.Equal);
					SteamMatchmaking.AddRequestLobbyListNumericalFilter("password", 0, LobbyComparison.Equal);
					SteamMatchmaking.AddRequestLobbyListStringFilter("version", Application.ProductVersion, LobbyComparison.Equal);
					SteamMatchmaking.AddRequestLobbyListResultCountFilter(10);
					SteamMatchmaking.RequestLobbyList(new Action<LobbyMatchList>(this.OnRequestedLobbyList_QuickJoin));
					WaitingMessageBox.Instance.Show(SubMenuOnline.LOC_SEARCHING);
					WaitingMessageBox instance = WaitingMessageBox.Instance;
					instance.OnAbort = (Action)Delegate.Combine(instance.OnAbort, new Action(this.OnAbortedQuickStart));
					return;
				}
				case 13:
					NetworkHostMessageBox.Instance.Show(new NetworkHostMessageBox.Complete(this.OnHostGame));
					return;
				case 14:
					ManualConnectMessageBox.Instance.Show();
					return;
				case 15:
					if (this.mSelectedServer >= 0 && this.mSelectedServer < 12)
					{
						if (this.mServers[this.mServerTop + this.mSelectedServer].m_Password)
						{
							string iDescr = LanguageManager.Instance.GetString(SubMenuOnline.LOC_PASSWORD).ToUpper();
							this.mPasswordMessageBox.Show(new Action<string>(this.OnPasswordInput), iSender, iDescr, true);
							return;
						}
						WaitingMessageBox.Instance.Show(SubMenuOnline.LOC_CONNECTING);
						WaitingMessageBox instance2 = WaitingMessageBox.Instance;
						instance2.OnAbort = (Action)Delegate.Combine(instance2.OnAbort, new Action(this.OnAbortConnectToServer));
						NetworkManager.Instance.ConnectToServer(this.mServers[this.mServerTop + this.mSelectedServer].ServerID, null, new Action<ConnectionStatus>(this.OnConnectToServer));
						return;
					}
					break;
				case 16:
					Tome.Instance.PopMenu();
					return;
				case 17:
					this.mFilterPanel.ControllA();
					return;
				default:
					if (this.mSelectedPosition < 12)
					{
						if (this.mSelectedServer == -1 || this.mSelectedPosition != this.mSelectedServer)
						{
							this.mSelectedServer = this.mSelectedPosition;
							this.mSelectedPosition = 15;
							return;
						}
						if (this.mSelectedPosition > this.mServers.Count)
						{
							return;
						}
						if (this.mServers[this.mServerTop + this.mSelectedServer].m_Password)
						{
							string iDescr2 = LanguageManager.Instance.GetString(SubMenuOnline.LOC_PASSWORD).ToUpper();
							this.mPasswordMessageBox.Show(new Action<string>(this.OnPasswordInput), iSender, iDescr2, true);
							return;
						}
						WaitingMessageBox.Instance.Show(SubMenuOnline.LOC_CONNECTING);
						WaitingMessageBox instance3 = WaitingMessageBox.Instance;
						instance3.OnAbort = (Action)Delegate.Combine(instance3.OnAbort, new Action(this.OnAbortConnectToServer));
						NetworkManager.Instance.ConnectToServer(this.mServers[this.mServerTop + this.mSelectedServer].ServerID, null, new Action<ConnectionStatus>(this.OnConnectToServer));
					}
					break;
				}
				return;
			}
			if (this.mHighlitTab != this.mSelectedTab)
			{
				this.SwitchToTab(this.mHighlitTab);
				return;
			}
			if (this.mSelectedPosition < 0)
			{
				this.mHighlitTab = -1;
				this.mSelectedPosition = 0;
				return;
			}
			this.SortServers(this.mSelectedPosition - 12);
		}

		// Token: 0x06001CC6 RID: 7366 RVA: 0x000C9ECC File Offset: 0x000C80CC
		public override void OnEnter()
		{
			GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Bottom, SubMenu.LOC_SELECT);
			GamePadMenuHelp.Instance.ActivateButton(GamePadMenuHelp.Button.Right, SubMenu.LOC_BACK);
			GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Left);
			GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Top);
			if (this.mFilterPanel != null)
			{
				this.mFilterPanel.InitializeFilters();
			}
			base.OnEnter();
			for (int i = 0; i < this.mMenuItems.Count; i++)
			{
				this.mMenuItems[i].Selected = false;
			}
			this.mSelectedTab = MagickaMath.CountTrailingZeroBits((uint)GlobalSettings.Instance.Filter.GameType);
			this.RefreshServerList();
		}

		// Token: 0x06001CC7 RID: 7367 RVA: 0x000C9F70 File Offset: 0x000C8170
		public override void OnExit()
		{
			ToolTipMan.Instance.KillAll(false);
			NetworkManager.Instance.AbortQuery();
			SaveManager.Instance.SaveSettings();
			base.OnExit();
		}

		// Token: 0x04001EEE RID: 7918
		private const int MAX_VISIBLE_SERVERS = 12;

		// Token: 0x04001EEF RID: 7919
		private const int QUICK = 0;

		// Token: 0x04001EF0 RID: 7920
		private const int HOST = 1;

		// Token: 0x04001EF1 RID: 7921
		private const int MANUAL = 2;

		// Token: 0x04001EF2 RID: 7922
		private const int JOIN = 3;

		// Token: 0x04001EF3 RID: 7923
		private const int BACK = 4;

		// Token: 0x04001EF4 RID: 7924
		private const int FILTER_PANEL = 5;

		// Token: 0x04001EF5 RID: 7925
		private const int NAME = 6;

		// Token: 0x04001EF6 RID: 7926
		private const int LEVEL = 7;

		// Token: 0x04001EF7 RID: 7927
		private const int NR_PLAYERS = 8;

		// Token: 0x04001EF8 RID: 7928
		private const int PING = 9;

		// Token: 0x04001EF9 RID: 7929
		private static SubMenuOnline sSingelton;

		// Token: 0x04001EFA RID: 7930
		private static volatile object sSingeltonLock = new object();

		// Token: 0x04001EFB RID: 7931
		public static readonly int LOC_ONLINE = "#network_02".GetHashCodeCustom();

		// Token: 0x04001EFC RID: 7932
		public static readonly int LOC_IP = "#network_03".GetHashCodeCustom();

		// Token: 0x04001EFD RID: 7933
		public static readonly int LOC_SEARCH = "#network_04".GetHashCodeCustom();

		// Token: 0x04001EFE RID: 7934
		public static readonly int LOC_REFRESH = "#network_05".GetHashCodeCustom();

		// Token: 0x04001EFF RID: 7935
		public static readonly int LOC_KICK = "#network_06".GetHashCodeCustom();

		// Token: 0x04001F00 RID: 7936
		public static readonly int LOC_BAN = "#network_07".GetHashCodeCustom();

		// Token: 0x04001F01 RID: 7937
		public static readonly int LOC_RETRY = "#network_08".GetHashCodeCustom();

		// Token: 0x04001F02 RID: 7938
		public static readonly int LOC_DISCONNECTED = "#network_09".GetHashCodeCustom();

		// Token: 0x04001F03 RID: 7939
		public static readonly int LOC_DISCONNECT = "#network_10".GetHashCodeCustom();

		// Token: 0x04001F04 RID: 7940
		public static readonly int LOC_CONNECT = "#network_11".GetHashCodeCustom();

		// Token: 0x04001F05 RID: 7941
		public static readonly int LOC_CONNECTED = "#network_12".GetHashCodeCustom();

		// Token: 0x04001F06 RID: 7942
		public static readonly int LOC_PING = "#network_13".GetHashCodeCustom();

		// Token: 0x04001F07 RID: 7943
		public static readonly int LOC_INVITE_FRIEND = "#network_14".GetHashCodeCustom();

		// Token: 0x04001F08 RID: 7944
		public static readonly int LOC_FRIENDS = "#network_15".GetHashCodeCustom();

		// Token: 0x04001F09 RID: 7945
		public static readonly int LOC_LATENCY = "#network_16".GetHashCodeCustom();

		// Token: 0x04001F0A RID: 7946
		public static readonly int LOC_INFO = "#network_17".GetHashCodeCustom();

		// Token: 0x04001F0B RID: 7947
		public static readonly int LOC_DETAILS = "#network_18".GetHashCodeCustom();

		// Token: 0x04001F0C RID: 7948
		public static readonly int LOC_SIGN_IN = "#network_19".GetHashCodeCustom();

		// Token: 0x04001F0D RID: 7949
		public static readonly int LOC_UPDATE = "#network_20".GetHashCodeCustom();

		// Token: 0x04001F0E RID: 7950
		public static readonly int LOC_NO_GAMES_FOUND = "#network_21".GetHashCodeCustom();

		// Token: 0x04001F0F RID: 7951
		public static readonly int LOC_GAMES_FOUND = "#network_22".GetHashCodeCustom();

		// Token: 0x04001F10 RID: 7952
		public static readonly int LOC_LOADING = "#network_23".GetHashCodeCustom();

		// Token: 0x04001F11 RID: 7953
		public static readonly int LOC_WAITING_FOR_OTHER_PLAYERS = "#network_24".GetHashCodeCustom();

		// Token: 0x04001F12 RID: 7954
		public static readonly int LOC_SERVERS = "#network_25".GetHashCodeCustom();

		// Token: 0x04001F13 RID: 7955
		public static readonly int LOC_QUICK_START = "#network_26".GetHashCodeCustom();

		// Token: 0x04001F14 RID: 7956
		public static readonly int LOC_VAC = "#network_27".GetHashCodeCustom();

		// Token: 0x04001F15 RID: 7957
		public static readonly int LOC_FILTER = "#network_28".GetHashCodeCustom();

		// Token: 0x04001F16 RID: 7958
		public static readonly int LOC_ALLOW_MIDGAME_JOIN = "#network_29".GetHashCodeCustom();

		// Token: 0x04001F17 RID: 7959
		public static readonly int LOC_PRIVATE = "#network_30".GetHashCodeCustom();

		// Token: 0x04001F18 RID: 7960
		public static readonly int LOC_LOCAL = "#network_31".GetHashCodeCustom();

		// Token: 0x04001F19 RID: 7961
		public static readonly int LOC_ALL = "#network_33".GetHashCodeCustom();

		// Token: 0x04001F1A RID: 7962
		public static readonly int LOC_HOST = "#network_34".GetHashCodeCustom();

		// Token: 0x04001F1B RID: 7963
		public static readonly int LOC_FREESLOTS = "#network_35".GetHashCodeCustom();

		// Token: 0x04001F1C RID: 7964
		public static readonly int LOC_FRIENDS_ONLY = "#network_36".GetHashCodeCustom();

		// Token: 0x04001F1D RID: 7965
		public static readonly int LOC_LAN_ABBR = "#network_37".GetHashCodeCustom();

		// Token: 0x04001F1E RID: 7966
		public static readonly int LOC_CONNECTING = "#network_40".GetHashCodeCustom();

		// Token: 0x04001F1F RID: 7967
		public static readonly int LOC_CHAT = "#network_41".GetHashCodeCustom();

		// Token: 0x04001F20 RID: 7968
		public static readonly int LOC_SHOW_CHAT = "#network_42".GetHashCodeCustom();

		// Token: 0x04001F21 RID: 7969
		public static readonly int LOC_HIDE_CHAT = "#network_43".GetHashCodeCustom();

		// Token: 0x04001F22 RID: 7970
		public static readonly int LOC_NO_LEVEL_SELECTED = "#network_no_level_selected".GetHashCodeCustom();

		// Token: 0x04001F23 RID: 7971
		public static readonly int LOC_HOST_ONLINE = "#menu_opt_online_01".GetHashCodeCustom();

		// Token: 0x04001F24 RID: 7972
		public static readonly int LOC_STOP_HOST = "#menu_opt_online_02".GetHashCodeCustom();

		// Token: 0x04001F25 RID: 7973
		public static readonly int LOC_JOIN = "#add_menu_join".GetHashCodeCustom();

		// Token: 0x04001F26 RID: 7974
		public static readonly int LOC_JOIN_ONLINE = "#menu_opt_online_03".GetHashCodeCustom();

		// Token: 0x04001F27 RID: 7975
		public static readonly int LOC_MANUAL = "#add_menu_ip".GetHashCodeCustom();

		// Token: 0x04001F28 RID: 7976
		public static readonly int LOC_NAME = "#add_menu_gamename".GetHashCodeCustom();

		// Token: 0x04001F29 RID: 7977
		public static readonly int LOC_MODE = "#menu_vs_02".GetHashCodeCustom();

		// Token: 0x04001F2A RID: 7978
		public static readonly int LOC_LEVEL = "#menu_vs_13".GetHashCodeCustom();

		// Token: 0x04001F2B RID: 7979
		public static readonly int LOC_PLAYERS = "#network_39".GetHashCodeCustom();

		// Token: 0x04001F2C RID: 7980
		public static readonly int LOC_PORT = "#add_menu_port".GetHashCodeCustom();

		// Token: 0x04001F2D RID: 7981
		public static readonly int LOC_SEARCHING = "#network_searching".GetHashCodeCustom();

		// Token: 0x04001F2E RID: 7982
		public static readonly int LOC_CHALLENGE = "#network_challenge".GetHashCodeCustom();

		// Token: 0x04001F2F RID: 7983
		public static readonly int LOC_STORYCHALLANGE = "#network_storychallenge".GetHashCodeCustom();

		// Token: 0x04001F30 RID: 7984
		public static readonly int LOC_STATUS_INITIALIZING = "#status_p01_initializing".GetHashCodeCustom();

		// Token: 0x04001F31 RID: 7985
		public static readonly int LOC_STATUS_CONNECTING = "#status_p01_connecting".GetHashCodeCustom();

		// Token: 0x04001F32 RID: 7986
		public static readonly int LOC_STATUS_AUTHENTICATING = "#status_p01_authenticating".GetHashCodeCustom();

		// Token: 0x04001F33 RID: 7987
		public static readonly int LOC_STATUS_CONNECTED = "#status_p01_connected".GetHashCodeCustom();

		// Token: 0x04001F34 RID: 7988
		public static readonly int LOC_SETTINGS_PASSWORD = "#settings_p01_password".GetHashCodeCustom();

		// Token: 0x04001F35 RID: 7989
		public static readonly int LOC_SETTINGS_VISIBILITY = "#settings_p01_visibility".GetHashCodeCustom();

		// Token: 0x04001F36 RID: 7990
		public static readonly int LOC_SETTINGS_PRIVATE = "#settings_p01_private".GetHashCodeCustom();

		// Token: 0x04001F37 RID: 7991
		public static readonly int LOC_SETTINGS_PUBLIC = "#settings_p01_public".GetHashCodeCustom();

		// Token: 0x04001F38 RID: 7992
		public static readonly int LOC_SETTINGS_SHOW = "#settings_p01_show".GetHashCodeCustom();

		// Token: 0x04001F39 RID: 7993
		public static readonly int LOC_SETTINGS_HIDE = "#settings_p01_hide".GetHashCodeCustom();

		// Token: 0x04001F3A RID: 7994
		public static readonly int LOC_ERROR_MISMATCH = "#error_p01_mismatch".GetHashCodeCustom();

		// Token: 0x04001F3B RID: 7995
		public static readonly int LOC_ERROR_UNKNOWN = "#error_p01_unknown".GetHashCodeCustom();

		// Token: 0x04001F3C RID: 7996
		public static readonly int LOC_ERROR_SERVERFULL = "#error_p01_serverfull".GetHashCodeCustom();

		// Token: 0x04001F3D RID: 7997
		public static readonly int LOC_ERROR_AUTHFAIL = "#error_p01_authfail".GetHashCodeCustom();

		// Token: 0x04001F3E RID: 7998
		public static readonly int LOC_ERROR_INPROGRESS = "#error_p01_inprogress".GetHashCodeCustom();

		// Token: 0x04001F3F RID: 7999
		public static readonly int LOC_ERROR_VERSION = "#error_p01_version".GetHashCodeCustom();

		// Token: 0x04001F40 RID: 8000
		public static readonly int LOC_ERROR_PASSWORD = "#error_p01_password".GetHashCodeCustom();

		// Token: 0x04001F41 RID: 8001
		public static readonly int LOC_ERROR_NO_GAMER = "#error_p01_no_gamer".GetHashCodeCustom();

		// Token: 0x04001F42 RID: 8002
		public static readonly int LOC_ERROR_TIMEOUT = "#error_p01_timeout".GetHashCodeCustom();

		// Token: 0x04001F43 RID: 8003
		public static readonly int LOC_SEARCH_SCOPE = "#search_scope".GetHashCodeCustom();

		// Token: 0x04001F44 RID: 8004
		public static readonly int LOC_LAN = "#lan".GetHashCodeCustom();

		// Token: 0x04001F45 RID: 8005
		public static readonly int LOC_VAC_ONLY = "#vac_only".GetHashCodeCustom();

		// Token: 0x04001F46 RID: 8006
		public static readonly int LOC_PLAYING = "#playing".GetHashCodeCustom();

		// Token: 0x04001F47 RID: 8007
		public static readonly int LOC_TT_HIDE_PRIVATE_GAMES = "#hide_private_games".GetHashCodeCustom();

		// Token: 0x04001F48 RID: 8008
		public static readonly int LOC_TT_HIDE_GAMES_IN_PROGRESS = "#hide_games_in_progress".GetHashCodeCustom();

		// Token: 0x04001F49 RID: 8009
		public static readonly int LOC_TT_SHOW_ONLY_GAMES_PROTECTED_BY_VAC = "#show_vac_only".GetHashCodeCustom();

		// Token: 0x04001F4A RID: 8010
		private static readonly int LOC_TT_NAME = "#tooltip_sb_name".GetHashCodeCustom();

		// Token: 0x04001F4B RID: 8011
		private static readonly int LOC_TT_LEVEL = "#tooltip_sb_level".GetHashCodeCustom();

		// Token: 0x04001F4C RID: 8012
		private static readonly int LOC_TT_PLAYERS = "#tooltip_sb_players".GetHashCodeCustom();

		// Token: 0x04001F4D RID: 8013
		private static readonly int LOC_TT_PING = "#tooltip_sb_ping".GetHashCodeCustom();

		// Token: 0x04001F4E RID: 8014
		private static readonly int LOC_TT_FILTER = "#tooltip_sb_filter".GetHashCodeCustom();

		// Token: 0x04001F4F RID: 8015
		private static readonly int LOC_TT_HOST = "#tooltip_sb_host".GetHashCodeCustom();

		// Token: 0x04001F50 RID: 8016
		private static readonly int LOC_TT_QUICK = "#tooltip_sb_quick".GetHashCodeCustom();

		// Token: 0x04001F51 RID: 8017
		private static readonly int LOC_TT_REFRESH = "#tooltip_sb_refresh".GetHashCodeCustom();

		// Token: 0x04001F52 RID: 8018
		private static readonly int LOC_TT_IP = "#tooltip_sb_ip".GetHashCodeCustom();

		// Token: 0x04001F53 RID: 8019
		private static readonly int LOC_TT_JOIN = "#tooltip_sb_join".GetHashCodeCustom();

		// Token: 0x04001F54 RID: 8020
		private new static readonly int LOC_PASSWORD = "#SETTINGS_P01_PASSWORD".GetHashCodeCustom();

		// Token: 0x04001F55 RID: 8021
		private static VertexBuffer sBackgroundVertexBuffer;

		// Token: 0x04001F56 RID: 8022
		private static VertexDeclaration sBackgroundVertexDeclaration;

		// Token: 0x04001F57 RID: 8023
		private static VertexBuffer sVertexBuffer;

		// Token: 0x04001F58 RID: 8024
		private static VertexDeclaration sVertexDeclaration;

		// Token: 0x04001F59 RID: 8025
		private static LobbyDistanceFilter _quickJoinSearchDistance;

		// Token: 0x04001F5A RID: 8026
		private ReadOnlyCollection<GameServerItem> mServers;

		// Token: 0x04001F5B RID: 8027
		private int mServerTop;

		// Token: 0x04001F5C RID: 8028
		private int mSelectedServer;

		// Token: 0x04001F5D RID: 8029
		private MenuScrollBar mServerScroll;

		// Token: 0x04001F5E RID: 8030
		private TextInputMessageBox mPasswordMessageBox;

		// Token: 0x04001F5F RID: 8031
		private List<MenuTextItem> mTitleItems = new List<MenuTextItem>();

		// Token: 0x04001F60 RID: 8032
		private static readonly Vector2 LIST_BG_POSITION = new Vector2(144f, 288f);

		// Token: 0x04001F61 RID: 8033
		private static readonly Vector2 LIST_BG_SIZE = new Vector2(800f, 416f);

		// Token: 0x04001F62 RID: 8034
		private static readonly Vector2 LIST_TOP_POSITION = new Vector2(128f, SubMenuOnline.LIST_BG_POSITION.Y - 128f);

		// Token: 0x04001F63 RID: 8035
		private static readonly Vector2 LIST_TOP_SIZE = new Vector2(832f, 144f);

		// Token: 0x04001F64 RID: 8036
		private static readonly Vector2 LIST_HEADER_POSITION = new Vector2(SubMenuOnline.LIST_TOP_POSITION.X + 20f + 16f, SubMenuOnline.LIST_TOP_POSITION.Y + SubMenuOnline.LIST_TOP_SIZE.Y * 0.5f);

		// Token: 0x04001F65 RID: 8037
		private int mHighlitTab = -1;

		// Token: 0x04001F66 RID: 8038
		private int mSelectedTab;

		// Token: 0x04001F67 RID: 8039
		private Text[] mTabTexts;

		// Token: 0x04001F68 RID: 8040
		private FilterPanel mFilterPanel;

		// Token: 0x04001F69 RID: 8041
		private FilteredServerList mFilteredServerList;

		// Token: 0x04001F6A RID: 8042
		private DirectionalKeyboardHelper mInputHelper;

		// Token: 0x04001F6B RID: 8043
		private bool mServersSelected;
	}
}
