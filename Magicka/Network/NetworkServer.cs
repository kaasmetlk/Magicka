using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Magicka.GameLogic;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.Abilities.SpecialAbilities;
using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.GameLogic.GameStates.Menu.Main;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.Spells.SpellEffects;
using Magicka.GameLogic.UI;
using Magicka.Gamers;
using Magicka.Levels;
using Magicka.Levels.Triggers;
using Magicka.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SteamWrapper;

namespace Magicka.Network
{
	// Token: 0x02000475 RID: 1141
	internal class NetworkServer : NetworkInterface
	{
		// Token: 0x0600226E RID: 8814 RVA: 0x000F76AC File Offset: 0x000F58AC
		public NetworkServer(GameType iGameType, bool iVAC)
		{
			this.mPlaying = false;
			this.mGameType = iGameType;
			this.mLevelName = NetworkManager.Instance.LevelName;
			this.mPassword = NetworkManager.Instance.Password;
			this.mVAC = iVAC;
			if (!SteamGameServer.Init(0U, 8766, 27015, 0, 27016, iVAC ? ServerMode.AuthenticationAndSecure : ServerMode.Authentication, "magicka", Application.ProductVersion))
			{
				throw new Exception();
			}
			Game.Instance.AddLoadTask(delegate
			{
				try
				{
					this.mUPnPEnabled = NAT.Discover();
					if (this.mUPnPEnabled)
					{
						NAT.ForwardPort(27016, ProtocolType.Udp, "Magicka: " + NetworkManager.Instance.GameName + " (UDP)");
					}
				}
				catch
				{
					this.mUPnPEnabled = false;
				}
			});
			SteamGameServer.SteamServersConnected += this.SteamGameServer_SteamServersConnected;
			SteamGameServer.SteamServersDisconnected += this.SteamGameServer_SteamServersDisconnected;
			SteamGameServer.GSPolicyResponse += this.SteamGameServer_GSPolicyResponse;
			SteamGameServer.GSClientApprove += this.SteamGameServer_GSClientApprove;
			SteamGameServer.GSClientDeny += this.SteamGameServer_GSClientDeny;
			SteamGameServer.GSClientKick += this.SteamGameServer_GSClientKick;
			SteamGameServer.P2PSessionRequest += this.SteamGameServer_P2PSessionRequest;
			SteamGameServer.P2PSessionConnectFail += this.SteamGameServer_P2PSessionConnectFail;
			this.UpdateGameTags();
			this.mRxBuffer = new byte[1048576];
			this.mTxBuffer = new byte[1048576];
			this.mReader = new BinaryReader(new MemoryStream(this.mRxBuffer));
			this.mWriter = new BinaryWriter(new MemoryStream(this.mTxBuffer));
			this.mListening = true;
			this.mLostConnectionMB = new OptionsMessageBox("Connection lost to                                               ", new string[]
			{
				"Ok"
			});
		}

		// Token: 0x17000835 RID: 2101
		// (get) Token: 0x0600226F RID: 8815 RVA: 0x000F7899 File Offset: 0x000F5A99
		public override SteamID ServerID
		{
			get
			{
				return SteamGameServer.GetSteamID();
			}
		}

		// Token: 0x17000836 RID: 2102
		// (get) Token: 0x06002270 RID: 8816 RVA: 0x000F78A0 File Offset: 0x000F5AA0
		public override bool IsVACSecure
		{
			get
			{
				return this.mVAC;
			}
		}

		// Token: 0x17000837 RID: 2103
		// (get) Token: 0x06002271 RID: 8817 RVA: 0x000F78A8 File Offset: 0x000F5AA8
		// (set) Token: 0x06002272 RID: 8818 RVA: 0x000F78B0 File Offset: 0x000F5AB0
		public bool Playing
		{
			get
			{
				return this.mPlaying;
			}
			set
			{
				if (this.mPlaying != value)
				{
					this.mPlaying = value;
					SteamMatchmaking.SetLobbyJoinable(this.mSteamIDLobby, !value);
					SteamMatchmaking.SetLobbyData(this.mSteamIDLobby, "playing", value ? "1" : "0");
					this.UpdateGameTags();
					if (value)
					{
						this.mPendingClients.Clear();
					}
				}
			}
		}

		// Token: 0x06002273 RID: 8819 RVA: 0x000F7911 File Offset: 0x000F5B11
		private void UpdateGameTags()
		{
			SteamGameServer.SetGameType(string.Format("P{0}T{1}", this.mPlaying ? "1" : "0", (int)this.mGameType));
		}

		// Token: 0x06002274 RID: 8820 RVA: 0x000F7944 File Offset: 0x000F5B44
		private void OnCreatedLobby(LobbyCreated iLobby)
		{
			if (iLobby.mResult == Result.OK)
			{
				this.mSteamIDLobby = new SteamID(iLobby.mSteamIDLobby);
				if (!NetworkManager.Instance.HasHostSettings)
				{
					SteamMatchmaking.LeaveLobby(this.mSteamIDLobby);
					SteamMatchmaking.SetLobbyJoinable(this.mSteamIDLobby, false);
					return;
				}
				SteamMatchmaking.SetLobbyData(this.mSteamIDLobby, "slots", (4 - this.mClients.Count).ToString());
				SteamMatchmaking.SetLobbyData(this.mSteamIDLobby, "playing", "0");
				if (string.IsNullOrEmpty(this.mPassword))
				{
					SteamMatchmaking.SetLobbyData(this.mSteamIDLobby, "password", "0");
				}
				else
				{
					SteamMatchmaking.SetLobbyData(this.mSteamIDLobby, "password", "1");
				}
				SteamMatchmaking.SetLobbyData(this.mSteamIDLobby, "version", Application.ProductVersion);
				if (this.mGameType != GameType.Any)
				{
					SteamMatchmaking.SetLobbyData(this.mSteamIDLobby, "gametype", this.mGameType.ToString());
				}
				SteamID steamID = SteamGameServer.GetSteamID();
				uint publicIP = SteamGameServer.GetPublicIP();
				SteamMatchmaking.SetLobbyGameServer(this.mSteamIDLobby, publicIP, 27015, steamID);
			}
		}

		// Token: 0x06002275 RID: 8821 RVA: 0x000F7A6B File Offset: 0x000F5C6B
		private void SteamGameServer_SteamServersConnected(SteamServersConnected iParam)
		{
			if (!this.mSteamIDLobby.IsValid)
			{
				SteamMatchmaking.CreateLobby(LobbyType.Public, 4, new Action<LobbyCreated>(this.OnCreatedLobby));
			}
			this.SendUpdatedGameDataToSteam();
		}

		// Token: 0x06002276 RID: 8822 RVA: 0x000F7A93 File Offset: 0x000F5C93
		private void SteamGameServer_SteamServersDisconnected(SteamServersDisconnected iParam)
		{
		}

		// Token: 0x06002277 RID: 8823 RVA: 0x000F7A95 File Offset: 0x000F5C95
		private void SteamGameServer_GSPolicyResponse(GSPolicyResponse iParam)
		{
		}

		// Token: 0x06002278 RID: 8824 RVA: 0x000F7A98 File Offset: 0x000F5C98
		private void SteamGameServer_GSClientApprove(GSClientApprove iParam)
		{
			for (int i = 0; i < this.mPendingClients.Count; i++)
			{
				NetworkServer.PendingUser pendingUser = this.mPendingClients[i];
				if (pendingUser.ID == iParam.mSteamID)
				{
					AuthenticateReplyMessage authenticateReplyMessage = default(AuthenticateReplyMessage);
					authenticateReplyMessage.Response = AuthenticateReplyMessage.Reply.Ok;
					SubMenuCharacterSelect.Instance.GetSettings(out authenticateReplyMessage.GameInfo.GameType, out authenticateReplyMessage.GameInfo.Level, out authenticateReplyMessage.VersusSettings);
					authenticateReplyMessage.GameInfo.GameName = NetworkManager.Instance.GameName;
					NetworkServer.Connection item;
					item.ID = pendingUser.ID;
					item.Ready = false;
					item.Latency = 0f;
					item.Timer = Stopwatch.StartNew();
					item.TxBuffer = new byte[4400];
					item.Writer = new BinaryWriter(new MemoryStream(item.TxBuffer));
					item.SyncPoints = new List<uint>();
					this.mClients.Add(item);
					this.SendMessage<AuthenticateReplyMessage>(ref authenticateReplyMessage, pendingUser.ID);
					LobbyInfoMessage lobbyInfoMessage = default(LobbyInfoMessage);
					lobbyInfoMessage.SteamID = this.mSteamIDLobby.AsUInt64;
					this.SendMessage<LobbyInfoMessage>(ref lobbyInfoMessage, pendingUser.ID);
					int hashCodeCustom = "#add_menu_not_joined".GetHashCodeCustom();
					string iMessage = string.Format("{0} {1}", SteamFriends.GetFriendPersonaName(iParam.mSteamID), LanguageManager.Instance.GetString(hashCodeCustom));
					NetworkChat.Instance.AddMessage(iMessage);
					ClientConnectedMessage clientConnectedMessage;
					clientConnectedMessage.ID = pendingUser.ID;
					for (int j = 0; j < this.mClients.Count - 1; j++)
					{
						this.SendMessage<ClientConnectedMessage>(ref clientConnectedMessage, j);
					}
					for (int k = 0; k < this.mClients.Count - 1; k++)
					{
						clientConnectedMessage.ID = this.mClients[k].ID;
						this.SendMessage<ClientConnectedMessage>(ref clientConnectedMessage, pendingUser.ID);
					}
					bool[] ready = SubMenuCharacterSelect.Instance.Ready;
					for (int l = 0; l < this.mClients.Count - 1; l++)
					{
						GamerReadyMessage gamerReadyMessage;
						gamerReadyMessage.Id = (byte)l;
						gamerReadyMessage.Ready = ready[l];
						this.SendMessage<GamerReadyMessage>(ref gamerReadyMessage, pendingUser.ID);
					}
					Player[] players = Game.Instance.Players;
					for (int m = 0; m < 4; m++)
					{
						if (players[m].Playing)
						{
							GamerJoinRequestMessage gamerJoinRequestMessage;
							gamerJoinRequestMessage.Id = (sbyte)m;
							gamerJoinRequestMessage.Gamer = players[m].Gamer.GamerTag;
							gamerJoinRequestMessage.AvatarThumb = players[m].Gamer.Avatar.ThumbPath;
							gamerJoinRequestMessage.AvatarPortrait = players[m].Gamer.Avatar.PortraitPath;
							gamerJoinRequestMessage.AvatarType = players[m].Gamer.Avatar.TypeName;
							gamerJoinRequestMessage.Color = players[m].Gamer.Color;
							if (players[m].Gamer is NetworkGamer)
							{
								gamerJoinRequestMessage.SteamID = (players[m].Gamer as NetworkGamer).ClientID;
							}
							else
							{
								gamerJoinRequestMessage.SteamID = this.ServerID;
							}
							this.SendMessage<GamerJoinRequestMessage>(ref gamerJoinRequestMessage, pendingUser.ID);
							GamerReadyMessage gamerReadyMessage2 = default(GamerReadyMessage);
							gamerReadyMessage2.Id = (byte)m;
							gamerReadyMessage2.Ready = SubMenuCharacterSelect.Instance.GetReady(m);
							this.SendMessage<GamerReadyMessage>(ref gamerReadyMessage2, pendingUser.ID);
							MenuSelectMessage menuSelectMessage = default(MenuSelectMessage);
							menuSelectMessage.IntendedMenu = MenuSelectMessage.MenuType.CharacterSelect;
							menuSelectMessage.Option = 2;
							menuSelectMessage.Param0I = m;
							menuSelectMessage.Param1I = (int)players[m].Team;
							this.SendMessage<MenuSelectMessage>(ref menuSelectMessage, pendingUser.ID);
						}
					}
					SteamFriends.GetFriendPersonaName(iParam.mSteamID);
					SteamFriends.GetFriendAvatar(iParam.mSteamID, AvatarSize.AvatarSize32x32);
					SteamFriends.GetFriendAvatar(iParam.mSteamID, AvatarSize.AvatarSize64x64);
					this.mPendingClients.RemoveAt(i);
					break;
				}
			}
			SteamMatchmaking.SetLobbyData(this.mSteamIDLobby, "slots", (4 - this.mClients.Count).ToString());
		}

		// Token: 0x06002279 RID: 8825 RVA: 0x000F7EB4 File Offset: 0x000F60B4
		private void SteamGameServer_GSClientDeny(GSClientDeny iParam)
		{
			for (int i = 0; i < this.mPendingClients.Count; i++)
			{
				NetworkServer.PendingUser pendingUser = this.mPendingClients[i];
				if (pendingUser.ID == iParam.mSteamID)
				{
					AuthenticateReplyMessage authenticateReplyMessage = default(AuthenticateReplyMessage);
					authenticateReplyMessage.Response = AuthenticateReplyMessage.Reply.Error_AuthFailed;
					this.SendMessage<AuthenticateReplyMessage>(ref authenticateReplyMessage, pendingUser.ID);
					this.mPendingClients.RemoveAt(i);
					return;
				}
			}
		}

		// Token: 0x0600227A RID: 8826 RVA: 0x000F7F25 File Offset: 0x000F6125
		private void SteamGameServer_GSClientKick(GSClientKick iParam)
		{
			ErrorMessageBox.Instance.Show(LanguageManager.Instance.GetString(SubMenuOnline.LOC_ERROR_AUTHFAIL));
			NetworkManager.Instance.EndSession();
		}

		// Token: 0x0600227B RID: 8827 RVA: 0x000F7F4A File Offset: 0x000F614A
		private void SteamGameServer_P2PSessionRequest(P2PSessionRequest iParam)
		{
			SteamGameServerNetworking.AcceptP2PSessionWithUser(iParam.mSteamIDRemote);
		}

		// Token: 0x0600227C RID: 8828 RVA: 0x000F7F5C File Offset: 0x000F615C
		private void SteamGameServer_P2PSessionConnectFail(P2PSessionConnectFail iParam)
		{
			for (int i = 0; i < this.mClients.Count; i++)
			{
				if (this.mClients[i].ID == iParam.mSteamIDRemote)
				{
					this.CloseConnection(iParam.mSteamIDRemote, ConnectionClosedMessage.CReason.LostConnection);
					SteamGameServer.SendUserDisconnect(iParam.mSteamIDRemote);
				}
			}
			for (int j = 0; j < this.mPendingClients.Count; j++)
			{
				if (this.mPendingClients[j].ID == iParam.mSteamIDRemote)
				{
					this.mPendingClients.RemoveAt(j);
					SteamGameServer.SendUserDisconnect(iParam.mSteamIDRemote);
				}
			}
		}

		// Token: 0x0600227D RID: 8829 RVA: 0x000F8008 File Offset: 0x000F6208
		internal void UpdateLevel(string iLevelName)
		{
			this.mLevelName = iLevelName;
			string text = string.Format("{0}|{1}", (byte)this.mGameType, this.mLevelName);
			SteamGameServer.UpdateServerStatus(this.mClients.Count + 1, 4, 0, NetworkManager.Instance.GameName, text, text, !string.IsNullOrEmpty(NetworkManager.Instance.Password));
		}

		// Token: 0x0600227E RID: 8830 RVA: 0x000F806C File Offset: 0x000F626C
		private unsafe void SendUpdatedGameDataToSteam()
		{
			SteamMasterServerUpdater.SetBasicServerData(7, true, "255", "Magicka", 4, !string.IsNullOrEmpty(this.mPassword), "Magicka");
			SteamMasterServerUpdater.SetActive(true);
			string text = string.Format("{0}|{1}", (byte)this.mGameType, this.mLevelName);
			SteamGameServer.UpdateServerStatus(this.mClients.Count + 1, 4, 0, NetworkManager.Instance.GameName, text, text, !string.IsNullOrEmpty(NetworkManager.Instance.Password));
			SteamID steamID = SteamUser.GetSteamID();
			P2PSessionState p2PSessionState;
			SteamGameServerNetworking.GetP2PSessionState(steamID, out p2PSessionState);
			SteamID steamID2 = SteamGameServer.GetSteamID();
			P2PSessionState p2PSessionState2;
			SteamGameServerNetworking.GetP2PSessionState(steamID2, out p2PSessionState2);
			AuthenticationToken authenticationToken = default(AuthenticationToken);
			SteamGameServer.GetPublicIP();
			authenticationToken.Length = SteamUser.InitiateGameConnection((void*)(&authenticationToken.Data.FixedElementField), 1024, steamID2, p2PSessionState2.RemoteIP, 27015, true);
			if (!SteamGameServer.SendUserConnectAndAuthenticate(p2PSessionState.RemoteIP, (void*)(&authenticationToken.Data.FixedElementField), (uint)authenticationToken.Length, ref steamID))
			{
				SteamGameServer.SendUserDisconnect(steamID);
			}
		}

		// Token: 0x0600227F RID: 8831 RVA: 0x000F8174 File Offset: 0x000F6374
		public override void Sync()
		{
			EnterSyncMessage enterSyncMessage;
			lock (this)
			{
				this.mSyncPointCounter += 1U;
				enterSyncMessage.ID = this.mSyncPointCounter;
			}
			this.SendMessage<EnterSyncMessage>(ref enterSyncMessage);
			Thread.Sleep(100);
			while (!this.AllClientsReachedSyncPoint(enterSyncMessage.ID))
			{
				Thread.Sleep(10);
			}
			LeaveSyncMessage leaveSyncMessage;
			leaveSyncMessage.ID = enterSyncMessage.ID;
			this.SendMessage<LeaveSyncMessage>(ref leaveSyncMessage);
			lock (this.mClients)
			{
				for (int i = 0; i < this.mClients.Count; i++)
				{
					this.mClients[i].SyncPoints.Remove(enterSyncMessage.ID);
				}
			}
		}

		// Token: 0x06002280 RID: 8832 RVA: 0x000F8254 File Offset: 0x000F6454
		private bool AllClientsReachedSyncPoint(uint iID)
		{
			bool result = true;
			lock (this.mClients)
			{
				for (int i = 0; i < this.mClients.Count; i++)
				{
					if (!this.mClients[i].SyncPoints.Contains(iID))
					{
						result = false;
						break;
					}
				}
			}
			return result;
		}

		// Token: 0x06002281 RID: 8833 RVA: 0x000F82C0 File Offset: 0x000F64C0
		public override void Update()
		{
			long ticks = DateTime.Now.Ticks;
			int num = 0;
			for (int i = 0; i < this.mBytesSent.Count; i++)
			{
				if (ticks - this.mBytesSent.Keys[i] > 10000000L)
				{
					this.mBytesSent.RemoveAt(i);
					i--;
				}
				else
				{
					num += this.mBytesSent.Values[i];
				}
			}
			this.mBytesPerSecond = num;
			if (ticks - this.mLastLatencyCheck > 10000000L)
			{
				this.mLastLatencyCheck = ticks;
				for (int j = 0; j < this.mClients.Count; j++)
				{
					this.mClients[j].Timer.Reset();
					this.mClients[j].Timer.Start();
				}
				PingRequestMessage pingRequestMessage = default(PingRequestMessage);
				pingRequestMessage.Payload = (int)ticks;
				this.SendUdpMessage<PingRequestMessage>(ref pingRequestMessage);
			}
			SteamGameServer.RunCallbacks();
			uint num2;
			while (SteamGameServerNetworking.IsP2PPacketAvailable(out num2))
			{
				SteamID iSender;
				if (!SteamGameServerNetworking.ReadP2PPacket(this.mRxBuffer, out num2, out iSender))
				{
					return;
				}
				this.mReader.BaseStream.Position = 0L;
				while (this.mReader.BaseStream.Position < (long)((ulong)num2))
				{
					this.ReadMessage(this.mReader, iSender);
				}
			}
		}

		// Token: 0x06002282 RID: 8834 RVA: 0x000F8408 File Offset: 0x000F6608
		public override void FlushMessageBuffers()
		{
			for (int i = 0; i < this.mClients.Count; i++)
			{
				NetworkServer.Connection connection = this.mClients[i];
				uint num = (uint)connection.Writer.BaseStream.Position;
				if (num > 0U)
				{
					this.FlushUDPBatch(i, num);
				}
				P2PSessionState p2PSessionState;
				SteamGameServerNetworking.GetP2PSessionState(connection.ID, out p2PSessionState);
				if (p2PSessionState.ConnectionActive && p2PSessionState.PacketsQueuedForSend > 0)
				{
					SteamGameServerNetworking.SendP2PPacket(connection.ID, this.mDummyData, 0U, P2PSend.Reliable);
				}
			}
		}

		// Token: 0x06002283 RID: 8835 RVA: 0x000F8490 File Offset: 0x000F6690
		private int GetClient(SteamID iID)
		{
			for (int i = 0; i < this.mClients.Count; i++)
			{
				if (this.mClients[i].ID == iID)
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x06002284 RID: 8836 RVA: 0x000F84D0 File Offset: 0x000F66D0
		private unsafe void ReadMessage(BinaryReader iReader, SteamID iSender)
		{
			PacketType packetType = PacketType.ChatMessage;
			try
			{
				packetType = (PacketType)iReader.ReadByte();
				PacketType packetType2;
				if ((byte)(packetType & PacketType.Request) == 128)
				{
					packetType &= (PacketType)127;
					packetType2 = packetType;
					if (packetType2 <= PacketType.GamerJoin)
					{
						switch (packetType2)
						{
						case PacketType.Ping:
						{
							PingRequestMessage pingRequestMessage = default(PingRequestMessage);
							pingRequestMessage.Read(iReader);
							PingReplyMessage pingReplyMessage = default(PingReplyMessage);
							pingReplyMessage.Payload = pingRequestMessage.Payload;
							this.SendUdpMessage<PingReplyMessage>(ref pingReplyMessage, iSender);
							goto IL_1424;
						}
						case PacketType.Connect:
						{
							default(ConnectRequestMessage).Read(iReader);
							ConnectReplyMessage connectReplyMessage = default(ConnectReplyMessage);
							connectReplyMessage.ServerID = SteamGameServer.GetSteamID();
							connectReplyMessage.VACSecure = SteamGameServer.BSecure();
							connectReplyMessage.ServerName = NetworkManager.Instance.GameName;
							this.SendMessage<ConnectReplyMessage>(ref connectReplyMessage, iSender);
							goto IL_1424;
						}
						case PacketType.Authenticate:
						{
							AuthenticateRequestMessage authenticateRequestMessage = default(AuthenticateRequestMessage);
							authenticateRequestMessage.Read(iReader);
							if (authenticateRequestMessage.Version != Game.Instance.Version)
							{
								AuthenticateReplyMessage authenticateReplyMessage = default(AuthenticateReplyMessage);
								authenticateReplyMessage.Response = AuthenticateReplyMessage.Reply.Error_Version;
								this.SendMessage<AuthenticateReplyMessage>(ref authenticateReplyMessage, iSender);
								for (int i = 0; i < this.mPendingClients.Count; i++)
								{
									if (this.mPendingClients[i].ID == iSender)
									{
										this.mPendingClients.RemoveAt(i--);
									}
								}
								goto IL_1424;
							}
							if (!string.IsNullOrEmpty(this.mPassword) && !string.Equals(authenticateRequestMessage.Password, this.mPassword))
							{
								AuthenticateReplyMessage authenticateReplyMessage2 = default(AuthenticateReplyMessage);
								authenticateReplyMessage2.Response = AuthenticateReplyMessage.Reply.Error_Password;
								this.SendMessage<AuthenticateReplyMessage>(ref authenticateReplyMessage2, iSender);
								for (int j = 0; j < this.mPendingClients.Count; j++)
								{
									if (this.mPendingClients[j].ID == iSender)
									{
										this.mPendingClients.RemoveAt(j--);
									}
								}
								goto IL_1424;
							}
							bool flag = false;
							for (int k = 0; k < this.mClients.Count; k++)
							{
								if (this.mClients[k].ID == iSender)
								{
									flag = true;
									break;
								}
							}
							if (flag)
							{
								goto IL_1424;
							}
							int num = Math.Max(this.mClients.Count + 1, Game.Instance.PlayerCount) + this.mPendingClients.Count;
							if (num + 1 > 4)
							{
								AuthenticateReplyMessage authenticateReplyMessage3 = default(AuthenticateReplyMessage);
								authenticateReplyMessage3.Response = AuthenticateReplyMessage.Reply.Error_ServerFull;
								this.SendMessage<AuthenticateReplyMessage>(ref authenticateReplyMessage3, iSender);
								for (int l = 0; l < this.mPendingClients.Count; l++)
								{
									if (this.mPendingClients[l].ID == iSender)
									{
										this.mPendingClients.RemoveAt(l--);
									}
								}
								goto IL_1424;
							}
							if (this.mPlaying)
							{
								AuthenticateReplyMessage authenticateReplyMessage4 = default(AuthenticateReplyMessage);
								authenticateReplyMessage4.Response = AuthenticateReplyMessage.Reply.Error_GamePlaying;
								this.SendMessage<AuthenticateReplyMessage>(ref authenticateReplyMessage4, iSender);
								goto IL_1424;
							}
							NetworkServer.PendingUser item;
							item.ID = iSender;
							item.Players = authenticateRequestMessage.NrOfPlayers;
							if (this.mPendingClients.Contains(item))
							{
								goto IL_1424;
							}
							this.mPendingClients.Add(item);
							P2PSessionState p2PSessionState;
							SteamGameServerNetworking.GetP2PSessionState(iSender, out p2PSessionState);
							SteamID steamIDUser = iSender;
							if (!SteamGameServer.SendUserConnectAndAuthenticate(p2PSessionState.RemoteIP, (void*)(&authenticateRequestMessage.Token.Data.FixedElementField), (uint)authenticateRequestMessage.Token.Length, ref steamIDUser))
							{
								AuthenticateReplyMessage authenticateReplyMessage5 = default(AuthenticateReplyMessage);
								authenticateReplyMessage5.Response = AuthenticateReplyMessage.Reply.Error_AuthFailed;
								this.SendMessage<AuthenticateReplyMessage>(ref authenticateReplyMessage5, iSender);
								for (int m = 0; m < this.mPendingClients.Count; m++)
								{
									if (this.mPendingClients[m].ID == iSender)
									{
										this.mPendingClients.RemoveAt(m--);
									}
								}
								SteamGameServer.SendUserDisconnect(steamIDUser);
								goto IL_1424;
							}
							goto IL_1424;
						}
						default:
							if (packetType2 == PacketType.GamerJoin)
							{
								GamerJoinRequestMessage gamerJoinRequestMessage = default(GamerJoinRequestMessage);
								gamerJoinRequestMessage.Read(iReader);
								Player player = Player.Join(null, (int)gamerJoinRequestMessage.Id, new NetworkGamer(gamerJoinRequestMessage.Gamer, gamerJoinRequestMessage.Color, gamerJoinRequestMessage.AvatarThumb, gamerJoinRequestMessage.AvatarType, iSender));
								if (player != null)
								{
									GamerJoinAcceptMessage gamerJoinAcceptMessage;
									gamerJoinAcceptMessage.Gamer = gamerJoinRequestMessage.Gamer;
									gamerJoinAcceptMessage.Id = (sbyte)player.ID;
									this.SendMessage<GamerJoinAcceptMessage>(ref gamerJoinAcceptMessage, iSender);
									gamerJoinRequestMessage.Id = (sbyte)player.ID;
									for (int n = 0; n < this.mClients.Count; n++)
									{
										if (iSender != this.mClients[n].ID)
										{
											this.SendMessage<GamerJoinRequestMessage>(ref gamerJoinRequestMessage, n);
										}
									}
									goto IL_1424;
								}
								goto IL_1424;
							}
							break;
						}
					}
					else
					{
						if (packetType2 == PacketType.TriggerAction)
						{
							TriggerRequestMessage triggerRequestMessage = default(TriggerRequestMessage);
							triggerRequestMessage.Read(iReader);
							Trigger.NetworkAction(ref triggerRequestMessage);
							goto IL_1424;
						}
						switch (packetType2)
						{
						case PacketType.SpawnShield:
						{
							SpawnShieldRequestMessage spawnShieldRequestMessage = default(SpawnShieldRequestMessage);
							spawnShieldRequestMessage.Read(iReader);
							Shield fromCache = Shield.GetFromCache(PlayState.RecentPlayState);
							ISpellCaster iOwner = Entity.GetFromHandle((int)spawnShieldRequestMessage.OwnerHandle) as ISpellCaster;
							fromCache.Initialize(iOwner, spawnShieldRequestMessage.Position, spawnShieldRequestMessage.Radius, spawnShieldRequestMessage.Direction, spawnShieldRequestMessage.ShieldType, spawnShieldRequestMessage.HitPoints, Spell.SHIELDCOLOR);
							fromCache.PlayState.EntityManager.AddEntity(fromCache);
							SpawnShieldMessage spawnShieldMessage;
							spawnShieldMessage.Handle = fromCache.Handle;
							spawnShieldMessage.OwnerHandle = spawnShieldRequestMessage.OwnerHandle;
							spawnShieldMessage.Position = spawnShieldRequestMessage.Position;
							spawnShieldMessage.Radius = spawnShieldRequestMessage.Radius;
							spawnShieldMessage.Direction = spawnShieldRequestMessage.Direction;
							spawnShieldMessage.ShieldType = spawnShieldRequestMessage.ShieldType;
							spawnShieldMessage.HitPoints = spawnShieldRequestMessage.HitPoints;
							this.SendMessage<SpawnShieldMessage>(ref spawnShieldMessage);
							goto IL_1424;
						}
						case PacketType.SpawnBarrier:
						{
							SpawnBarrierRequestMessage spawnBarrierRequestMessage = default(SpawnBarrierRequestMessage);
							spawnBarrierRequestMessage.Read(iReader);
							Barrier fromCache2 = Barrier.GetFromCache(PlayState.RecentPlayState);
							Barrier.HitListWithBarriers fromCache3 = Barrier.HitListWithBarriers.GetFromCache();
							ISpellCaster iOwner2 = Entity.GetFromHandle((int)spawnBarrierRequestMessage.OwnerHandle) as ISpellCaster;
							AnimatedLevelPart iAnimation = null;
							if (spawnBarrierRequestMessage.AnimationHandle < 65535)
							{
								iAnimation = AnimatedLevelPart.GetFromHandle((int)spawnBarrierRequestMessage.AnimationHandle);
							}
							fromCache2.Initialize(iOwner2, spawnBarrierRequestMessage.Position, spawnBarrierRequestMessage.Direction, spawnBarrierRequestMessage.Scale, spawnBarrierRequestMessage.Range, spawnBarrierRequestMessage.NextDir, spawnBarrierRequestMessage.NextRotation, spawnBarrierRequestMessage.Distance, ref spawnBarrierRequestMessage.Spell, ref spawnBarrierRequestMessage.Damage, fromCache3, iAnimation);
							fromCache2.PlayState.EntityManager.AddEntity(fromCache2);
							SpawnBarrierMessage spawnBarrierMessage;
							spawnBarrierMessage.Handle = fromCache2.Handle;
							spawnBarrierMessage.OwnerHandle = spawnBarrierRequestMessage.OwnerHandle;
							spawnBarrierMessage.AnimationHandle = spawnBarrierRequestMessage.AnimationHandle;
							spawnBarrierMessage.Position = spawnBarrierRequestMessage.Position;
							spawnBarrierMessage.Direction = spawnBarrierRequestMessage.Direction;
							spawnBarrierMessage.Scale = spawnBarrierRequestMessage.Scale;
							spawnBarrierMessage.Spell = spawnBarrierRequestMessage.Spell;
							spawnBarrierMessage.Damage = spawnBarrierRequestMessage.Damage;
							spawnBarrierMessage.HitlistHandle = fromCache3.Handle;
							this.SendMessage<SpawnBarrierMessage>(ref spawnBarrierMessage);
							goto IL_1424;
						}
						case PacketType.SpawnWave:
						{
							SpawnWaveRequestMessage spawnWaveRequestMessage = default(SpawnWaveRequestMessage);
							spawnWaveRequestMessage.Read(iReader);
							WaveEntity fromCache4 = WaveEntity.GetFromCache(PlayState.RecentPlayState);
							Barrier.HitListWithBarriers fromCache5 = Barrier.HitListWithBarriers.GetFromCache();
							ISpellCaster iOwner3 = Entity.GetFromHandle((int)spawnWaveRequestMessage.OwnerHandle) as ISpellCaster;
							AnimatedLevelPart iAnimation2 = null;
							if (spawnWaveRequestMessage.AnimationHandle < 65535)
							{
								iAnimation2 = AnimatedLevelPart.GetFromHandle((int)spawnWaveRequestMessage.AnimationHandle);
							}
							Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Wave instance = Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Wave.GetInstance();
							fromCache4.Initialize(iOwner3, spawnWaveRequestMessage.Position, spawnWaveRequestMessage.Direction, spawnWaveRequestMessage.Scale, spawnWaveRequestMessage.Range, spawnWaveRequestMessage.NextDir, spawnWaveRequestMessage.NextRotation, spawnWaveRequestMessage.Distance, ref spawnWaveRequestMessage.Spell, ref spawnWaveRequestMessage.Damage, ref fromCache5, iAnimation2, ref instance);
							fromCache4.PlayState.EntityManager.AddEntity(fromCache4);
							SpawnWaveMessage spawnWaveMessage;
							spawnWaveMessage.Handle = fromCache4.Handle;
							spawnWaveMessage.OwnerHandle = spawnWaveRequestMessage.OwnerHandle;
							spawnWaveMessage.AnimationHandle = spawnWaveRequestMessage.AnimationHandle;
							spawnWaveMessage.ParentHandle = spawnWaveRequestMessage.ParentHandle;
							spawnWaveMessage.Position = spawnWaveRequestMessage.Position;
							spawnWaveMessage.Direction = spawnWaveRequestMessage.Direction;
							spawnWaveMessage.Scale = spawnWaveRequestMessage.Scale;
							spawnWaveMessage.Spell = spawnWaveRequestMessage.Spell;
							spawnWaveMessage.Damage = spawnWaveRequestMessage.Damage;
							spawnWaveMessage.HitlistHandle = fromCache5.Handle;
							this.SendMessage<SpawnWaveMessage>(ref spawnWaveMessage);
							goto IL_1424;
						}
						case PacketType.SpawnMine:
						{
							SpawnMineRequestMessage spawnMineRequestMessage = default(SpawnMineRequestMessage);
							spawnMineRequestMessage.Read(iReader);
							SpellMine instance2 = SpellMine.GetInstance();
							ISpellCaster iOwner4 = Entity.GetFromHandle((int)spawnMineRequestMessage.OwnerHandle) as ISpellCaster;
							AnimatedLevelPart iAnimation3 = null;
							if (spawnMineRequestMessage.AnimationHandle < 65535)
							{
								iAnimation3 = AnimatedLevelPart.GetFromHandle((int)spawnMineRequestMessage.AnimationHandle);
							}
							instance2.Initialize(iOwner4, spawnMineRequestMessage.Position, spawnMineRequestMessage.Direction, spawnMineRequestMessage.Scale, spawnMineRequestMessage.Range, spawnMineRequestMessage.NextDir, spawnMineRequestMessage.NextRotation, spawnMineRequestMessage.Distance, ref spawnMineRequestMessage.Spell, ref spawnMineRequestMessage.Damage, iAnimation3);
							instance2.PlayState.EntityManager.AddEntity(instance2);
							SpawnMineMessage spawnMineMessage;
							spawnMineMessage.Handle = instance2.Handle;
							spawnMineMessage.OwnerHandle = spawnMineRequestMessage.OwnerHandle;
							spawnMineMessage.AnimationHandle = spawnMineRequestMessage.AnimationHandle;
							spawnMineMessage.Position = spawnMineRequestMessage.Position;
							spawnMineMessage.Direction = spawnMineRequestMessage.Direction;
							spawnMineMessage.Scale = spawnMineRequestMessage.Scale;
							spawnMineMessage.Spell = spawnMineRequestMessage.Spell;
							spawnMineMessage.Damage = spawnMineRequestMessage.Damage;
							this.SendMessage<SpawnMineMessage>(ref spawnMineMessage);
							goto IL_1424;
						}
						case PacketType.SpawnVortex:
						{
							SpawnVortexMessage spawnVortexMessage = default(SpawnVortexMessage);
							spawnVortexMessage.Read(iReader);
							VortexEntity specificInstance = VortexEntity.GetSpecificInstance(spawnVortexMessage.Handle);
							Entity fromHandle = Entity.GetFromHandle((int)spawnVortexMessage.OwnerHandle);
							specificInstance.Initialize(fromHandle as ISpellCaster, spawnVortexMessage.Position);
							specificInstance.PlayState.EntityManager.AddEntity(specificInstance);
							goto IL_1424;
						}
						case PacketType.Damage:
						{
							DamageRequestMessage damageRequestMessage = default(DamageRequestMessage);
							damageRequestMessage.Read(iReader);
							Entity fromHandle2 = Entity.GetFromHandle((int)damageRequestMessage.AttackerHandle);
							IDamageable damageable = Entity.GetFromHandle((int)damageRequestMessage.TargetHandle) as IDamageable;
							if (!damageable.Dead || !(damageable is Avatar) || (damageable as Avatar).Player == null)
							{
								Vector3 relativeAttackPosition = damageRequestMessage.RelativeAttackPosition;
								Vector3 position = damageable.Position;
								Vector3.Add(ref position, ref relativeAttackPosition, out relativeAttackPosition);
								Defines.DamageFeatures iFeatures = Defines.DamageFeatures.DNK;
								damageable.InternalDamage(damageRequestMessage.Damage, fromHandle2, damageRequestMessage.TimeStamp, relativeAttackPosition, iFeatures);
								goto IL_1424;
							}
							goto IL_1424;
						}
						}
					}
					throw new Exception("Unhandled request (" + packetType.ToString() + ")!");
				}
				packetType2 = packetType;
				if (packetType2 <= PacketType.TriggerAction)
				{
					if (packetType2 <= PacketType.ConnectionClosed)
					{
						if (packetType2 != PacketType.Ping)
						{
							if (packetType2 != PacketType.ConnectionClosed)
							{
								goto IL_1404;
							}
							ConnectionClosedMessage connectionClosedMessage = default(ConnectionClosedMessage);
							connectionClosedMessage.Read(iReader);
							this.CloseConnection(iSender, connectionClosedMessage.Reason);
							goto IL_1424;
						}
						else
						{
							PingReplyMessage pingReplyMessage2 = default(PingReplyMessage);
							pingReplyMessage2.Read(iReader);
							if (pingReplyMessage2.Payload == (int)this.mLastLatencyCheck && this.mClients != null)
							{
								for (int num2 = 0; num2 < this.mClients.Count; num2++)
								{
									NetworkServer.Connection value = this.mClients[num2];
									if (value.ID == iSender)
									{
										value.Timer.Stop();
										value.Latency = (float)value.Timer.Elapsed.TotalSeconds;
										this.mClients[num2] = value;
										break;
									}
								}
								goto IL_1424;
							}
							goto IL_1424;
						}
					}
					else
					{
						switch (packetType2)
						{
						case PacketType.ChatMessage:
						{
							ChatMessage chatMessage = default(ChatMessage);
							chatMessage.Read(iReader);
							NetworkChat.Instance.AddMessage(chatMessage.ToString());
							goto IL_1424;
						}
						case PacketType.GamerJoin:
						case PacketType.LevelList:
						case PacketType.SaveData:
						case PacketType.GameChanged:
						case PacketType.GameBeginSceneChange:
							goto IL_1404;
						case PacketType.GamerChanged:
						{
							GamerChangedMessage gamerChangedMessage = default(GamerChangedMessage);
							gamerChangedMessage.Read(iReader);
							Player player2 = Game.Instance.Players[(int)gamerChangedMessage.Id];
							player2.UnlockedMagicks = gamerChangedMessage.UnlockedMagicks;
							Gamer gamer = player2.Gamer;
							gamer.Color = gamerChangedMessage.Color;
							gamer.GamerTag = gamerChangedMessage.GamerTag;
							Profile.PlayableAvatar avatar = default(Profile.PlayableAvatar);
							avatar.ThumbPath = gamerChangedMessage.AvatarThumb;
							lock (Game.Instance.GraphicsDevice)
							{
								avatar.Thumb = Game.Instance.Content.Load<Texture2D>(avatar.ThumbPath);
							}
							avatar.PortraitPath = gamerChangedMessage.AvatarPortrait;
							lock (Game.Instance.GraphicsDevice)
							{
								avatar.Portrait = Game.Instance.Content.Load<Texture2D>(avatar.PortraitPath);
							}
							avatar.TypeName = gamerChangedMessage.AvatarType;
							avatar.Type = avatar.TypeName.GetHashCodeCustom();
							avatar.Name = player2.Gamer.Avatar.Name;
							avatar.AllowCampaign = gamerChangedMessage.AvatarAllowCampaign;
							avatar.AllowChallenge = gamerChangedMessage.AvatarAllowChallenge;
							avatar.AllowPVP = gamerChangedMessage.AvatarAllowPVP;
							player2.Gamer.Avatar = avatar;
							SubMenuCharacterSelect.Instance.UpdateGamer(player2, gamer);
							goto IL_1424;
						}
						case PacketType.GamerLeave:
						{
							GamerLeaveMessage gamerLeaveMessage = default(GamerLeaveMessage);
							gamerLeaveMessage.Read(iReader);
							SubMenuCharacterSelect.Instance.SetReady(false);
							Player player3 = Game.Instance.Players[(int)gamerLeaveMessage.Id];
							player3.Playing = false;
							player3.Team = Factions.NONE;
							player3.Gamer = null;
							SubMenuCharacterSelect.Instance.UpdateGamer(player3, null);
							if (SubMenuCharacterSelect.Instance.NeedToUpdateDefaultAvatarsUponClientLeaving())
							{
								SubMenuCharacterSelect.Instance.DefaultAvatars();
								goto IL_1424;
							}
							goto IL_1424;
						}
						case PacketType.GamerReady:
						{
							GamerReadyMessage gamerReadyMessage = default(GamerReadyMessage);
							gamerReadyMessage.Read(iReader);
							SubMenuCharacterSelect.Instance.SetReady(gamerReadyMessage.Ready, gamerReadyMessage.Id);
							goto IL_1424;
						}
						case PacketType.MenuSelection:
						{
							MenuSelectMessage menuSelectMessage = default(MenuSelectMessage);
							menuSelectMessage.Read(iReader);
							MenuState menuState = GameStateManager.Instance.CurrentState as MenuState;
							if (menuState != null)
							{
								menuState.NetworkInput(ref menuSelectMessage);
							}
							for (int num3 = 0; num3 < this.mClients.Count; num3++)
							{
								SteamID id = this.mClients[num3].ID;
								if (id != iSender)
								{
									this.SendMessage<MenuSelectMessage>(ref menuSelectMessage, id);
								}
							}
							goto IL_1424;
						}
						case PacketType.GameEndLoad:
							default(GameEndLoadMessage).Read(iReader);
							for (int num4 = 0; num4 < this.mClients.Count; num4++)
							{
								NetworkServer.Connection value2 = this.mClients[num4];
								if (value2.ID == iSender)
								{
									value2.Ready = true;
									this.mClients[num4] = value2;
									break;
								}
							}
							goto IL_1424;
						default:
							if (packetType2 != PacketType.TriggerAction)
							{
								goto IL_1404;
							}
							break;
						}
					}
				}
				else if (packetType2 <= PacketType.SpawnMissile)
				{
					switch (packetType2)
					{
					case PacketType.DialogAdvance:
					{
						DialogAdvanceMessage dialogAdvanceMessage = default(DialogAdvanceMessage);
						dialogAdvanceMessage.Read(iReader);
						DialogManager.Instance.NetworkAdvance(dialogAdvanceMessage.Interact);
						goto IL_1424;
					}
					case PacketType.GameUpdate:
					case PacketType.PlayerUpdate:
						goto IL_1404;
					case PacketType.EntityUpdate:
					{
						EntityUpdateMessage entityUpdateMessage = default(EntityUpdateMessage);
						entityUpdateMessage.Read(iReader);
						Entity fromHandle3 = Entity.GetFromHandle((int)entityUpdateMessage.Handle);
						if (fromHandle3 != null)
						{
							fromHandle3.NetworkUpdate(iSender, ref entityUpdateMessage);
							goto IL_1424;
						}
						goto IL_1424;
					}
					case PacketType.CharacterAction:
					{
						CharacterActionMessage characterActionMessage = default(CharacterActionMessage);
						characterActionMessage.Read(iReader);
						Entity fromHandle4 = Entity.GetFromHandle((int)characterActionMessage.Handle);
						if (fromHandle4 == null)
						{
							return;
						}
						Character character = fromHandle4 as Character;
						if (character == null)
						{
							return;
						}
						character.NetworkAction(ref characterActionMessage);
						goto IL_1424;
					}
					default:
					{
						if (packetType2 != PacketType.SpawnMissile)
						{
							goto IL_1404;
						}
						SpawnMissileMessage spawnMissileMessage = default(SpawnMissileMessage);
						spawnMissileMessage.Read(iReader);
						MissileEntity missileEntity = Entity.GetFromHandle((int)spawnMissileMessage.Handle) as MissileEntity;
						if (missileEntity == null)
						{
							throw new Exception("Caches out of sync!");
						}
						switch (spawnMissileMessage.Type)
						{
						case SpawnMissileMessage.MissileType.Spell:
							ProjectileSpell.SpawnMissile(ref missileEntity, Entity.GetFromHandle((int)spawnMissileMessage.Owner) as ISpellCaster, spawnMissileMessage.Homing, ref spawnMissileMessage.Position, ref spawnMissileMessage.Velocity, ref spawnMissileMessage.Spell, spawnMissileMessage.Splash, (int)spawnMissileMessage.Item);
							goto IL_1424;
						case SpawnMissileMessage.MissileType.Item:
						{
							Item item2 = Entity.GetFromHandle((int)spawnMissileMessage.Item) as Item;
							Entity fromHandle5 = Entity.GetFromHandle((int)spawnMissileMessage.Owner);
							if (item2.ProjectileModel != null)
							{
								missileEntity.Initialize(fromHandle5, item2.ProjectileModel.Meshes[0].BoundingSphere.Radius, ref spawnMissileMessage.Position, ref spawnMissileMessage.Velocity, item2.ProjectileModel, item2.RangedConditions, false);
							}
							else
							{
								missileEntity.Initialize(fromHandle5, 0.75f, ref spawnMissileMessage.Position, ref spawnMissileMessage.Velocity, null, item2.RangedConditions, false);
							}
							missileEntity.Danger = item2.Danger;
							missileEntity.Homing = item2.Homing;
							missileEntity.FacingVelocity = item2.Facing;
							missileEntity.PlayState.EntityManager.AddEntity(missileEntity);
							goto IL_1424;
						}
						case SpawnMissileMessage.MissileType.HolyGrenade:
							HolyGrenade.SpawnGrenade(ref missileEntity, Entity.GetFromHandle((int)spawnMissileMessage.Owner) as ISpellCaster, ref spawnMissileMessage.Position, ref spawnMissileMessage.Velocity);
							goto IL_1424;
						case SpawnMissileMessage.MissileType.Grenade:
							Grenade.SpawnGrenade(ref missileEntity, Entity.GetFromHandle((int)spawnMissileMessage.Owner) as ISpellCaster, ref spawnMissileMessage.Position, ref spawnMissileMessage.Velocity);
							goto IL_1424;
						case SpawnMissileMessage.MissileType.FireFlask:
							Fireflask.SpawnGrenade(ref missileEntity, Entity.GetFromHandle((int)spawnMissileMessage.Owner) as ISpellCaster, ref spawnMissileMessage.Position, ref spawnMissileMessage.Velocity);
							goto IL_1424;
						case SpawnMissileMessage.MissileType.ProppMagick:
							ProppMagick.Spawn(ref missileEntity, Entity.GetFromHandle((int)spawnMissileMessage.Owner) as ISpellCaster, ref spawnMissileMessage.Position, ref spawnMissileMessage.Velocity, ref spawnMissileMessage.AngularVelocity, ref spawnMissileMessage.Lever);
							goto IL_1424;
						case SpawnMissileMessage.MissileType.JudgementMissile:
						{
							Entity fromHandle6 = Entity.GetFromHandle((int)spawnMissileMessage.Target);
							JudgementSpray.SpawnProjectile(ref missileEntity, Entity.GetFromHandle((int)spawnMissileMessage.Owner) as ISpellCaster, ref fromHandle6, ref spawnMissileMessage.Position, ref spawnMissileMessage.Velocity);
							goto IL_1424;
						}
						case SpawnMissileMessage.MissileType.GreaseLump:
							Entity.GetFromHandle((int)spawnMissileMessage.Target);
							GreaseLump.SpawnLump(ref missileEntity, Entity.GetFromHandle((int)spawnMissileMessage.Owner) as ISpellCaster, ref spawnMissileMessage.Position, ref spawnMissileMessage.Velocity);
							goto IL_1424;
						case SpawnMissileMessage.MissileType.PotionFlask:
							Entity.GetFromHandle((int)spawnMissileMessage.Target);
							Potion.SpawnFlask(ref missileEntity, Entity.GetFromHandle((int)spawnMissileMessage.Owner) as ISpellCaster, ref spawnMissileMessage.Position, ref spawnMissileMessage.Velocity);
							goto IL_1424;
						}
						throw new Exception();
					}
					}
				}
				else
				{
					if (packetType2 == PacketType.SpawnVortex)
					{
						SpawnVortexMessage spawnVortexMessage2 = default(SpawnVortexMessage);
						spawnVortexMessage2.Read(iReader);
						VortexEntity specificInstance2 = VortexEntity.GetSpecificInstance(spawnVortexMessage2.Handle);
						Entity fromHandle7 = Entity.GetFromHandle((int)spawnVortexMessage2.OwnerHandle);
						specificInstance2.Initialize(fromHandle7 as ISpellCaster, spawnVortexMessage2.Position);
						specificInstance2.PlayState.EntityManager.AddEntity(specificInstance2);
						goto IL_1424;
					}
					switch (packetType2)
					{
					case PacketType.MissileEntity:
					{
						MissileEntityEventMessage missileEntityEventMessage = default(MissileEntityEventMessage);
						missileEntityEventMessage.Read(iReader);
						MissileEntity missileEntity2 = Entity.GetFromHandle((int)missileEntityEventMessage.Handle) as MissileEntity;
						missileEntity2.NetworkEventMessage(ref missileEntityEventMessage);
						goto IL_1424;
					}
					case PacketType.Threat:
						goto IL_1404;
					case PacketType.EnterSync:
					{
						EnterSyncMessage enterSyncMessage = default(EnterSyncMessage);
						enterSyncMessage.Read(iReader);
						int client = this.GetClient(iSender);
						lock (this.mClients)
						{
							this.mClients[client].SyncPoints.Add(enterSyncMessage.ID);
							goto IL_1424;
						}
						break;
					}
					default:
					{
						if (packetType2 != PacketType.RequestForcedPlayerStatusSync)
						{
							goto IL_1404;
						}
						RequestForcedPlayerStatusSync requestForcedPlayerStatusSync = default(RequestForcedPlayerStatusSync);
						requestForcedPlayerStatusSync.Read(iReader);
						Entity fromHandle8 = Entity.GetFromHandle((int)requestForcedPlayerStatusSync.Handle);
						Character character2 = fromHandle8 as Character;
						if (character2 == null)
						{
							return;
						}
						Avatar avatar2 = character2 as Avatar;
						if (avatar2 == null)
						{
							return;
						}
						if (avatar2.Player == null)
						{
							return;
						}
						this.SendForcedSyncMessageToClient(iSender, false);
						goto IL_1424;
					}
					}
				}
				TriggerActionMessage triggerActionMessage = default(TriggerActionMessage);
				triggerActionMessage.Read(iReader);
				Trigger.NetworkAction(ref triggerActionMessage);
				goto IL_1424;
				IL_1404:
				throw new Exception("Unhandled message type (" + packetType.ToString() + ")!");
				IL_1424:;
			}
			catch (IOException)
			{
				this.CloseConnection(iSender, ConnectionClosedMessage.CReason.LostConnection);
			}
			catch (NullReferenceException innerException)
			{
				string text = SteamFriends.GetFriendPersonaName(iSender);
				if (string.IsNullOrEmpty(text))
				{
					text = "--";
				}
				throw new Exception(string.Format("System.NullReferenceException when reading message of type: {0} from {1} !", packetType.ToString(), text), innerException);
			}
		}

		// Token: 0x06002285 RID: 8837 RVA: 0x000F99CC File Offset: 0x000F7BCC
		public void SetAllClientsBusy()
		{
			for (int i = 0; i < this.mClients.Count; i++)
			{
				NetworkServer.Connection value = this.mClients[i];
				value.Ready = false;
				this.mClients[i] = value;
			}
		}

		// Token: 0x17000838 RID: 2104
		// (get) Token: 0x06002286 RID: 8838 RVA: 0x000F9A14 File Offset: 0x000F7C14
		public bool AllClientsReady
		{
			get
			{
				for (int i = 0; i < this.mClients.Count; i++)
				{
					if (!this.mClients[i].Ready)
					{
						return false;
					}
				}
				return true;
			}
		}

		// Token: 0x06002287 RID: 8839 RVA: 0x000F9A50 File Offset: 0x000F7C50
		public override void SendMessage<T>(ref T iMessage, P2PSend sendType)
		{
			lock (this.mTxBuffer)
			{
				this.mTxBuffer[0] = (byte)iMessage.PacketType;
				this.mWriter.BaseStream.Position = 1L;
				iMessage.Write(this.mWriter);
				uint num = (uint)this.mWriter.BaseStream.Position;
				for (int i = 0; i < this.mClients.Count; i++)
				{
					SteamGameServerNetworking.SendP2PPacket(this.mClients[i].ID, this.mTxBuffer, num, sendType);
					long ticks = DateTime.Now.Ticks;
					int num2;
					if (!this.mBytesSent.TryGetValue(ticks, out num2))
					{
						num2 = 0;
					}
					this.mBytesSent[ticks] = num2 + (int)num;
				}
			}
		}

		// Token: 0x06002288 RID: 8840 RVA: 0x000F9B34 File Offset: 0x000F7D34
		public override void SendMessage<T>(ref T iMessage, int iClientIndex)
		{
			this.SendMessage<T>(ref iMessage, iClientIndex, P2PSend.ReliableWithBuffering);
		}

		// Token: 0x06002289 RID: 8841 RVA: 0x000F9B3F File Offset: 0x000F7D3F
		public override void SendMessage<T>(ref T iMessage, int iClientIndex, P2PSend sendType)
		{
			this.SendMessage<T>(ref iMessage, this.mClients[iClientIndex].ID, sendType);
		}

		// Token: 0x0600228A RID: 8842 RVA: 0x000F9B5A File Offset: 0x000F7D5A
		public override void SendMessage<T>(ref T iMessage, SteamID iClientID)
		{
			this.SendMessage<T>(ref iMessage, iClientID, P2PSend.ReliableWithBuffering);
		}

		// Token: 0x0600228B RID: 8843 RVA: 0x000F9B68 File Offset: 0x000F7D68
		public override void SendMessage<T>(ref T iMessage, SteamID iClientID, P2PSend sendType)
		{
			lock (this.mTxBuffer)
			{
				this.mTxBuffer[0] = (byte)iMessage.PacketType;
				this.mWriter.BaseStream.Position = 1L;
				iMessage.Write(this.mWriter);
				uint num = (uint)this.mWriter.BaseStream.Position;
				SteamGameServerNetworking.SendP2PPacket(iClientID, this.mTxBuffer, num, sendType);
				long ticks = DateTime.Now.Ticks;
				int num2;
				if (!this.mBytesSent.TryGetValue(ticks, out num2))
				{
					num2 = 0;
				}
				this.mBytesSent[ticks] = num2 + (int)num;
			}
		}

		// Token: 0x0600228C RID: 8844 RVA: 0x000F9C24 File Offset: 0x000F7E24
		public override void SendUdpMessage<T>(ref T iMessage)
		{
			lock (this.mTxBuffer)
			{
				for (int i = 0; i < this.mClients.Count; i++)
				{
					this.QueueUDPMessage<T>(i, ref iMessage);
				}
			}
		}

		// Token: 0x0600228D RID: 8845 RVA: 0x000F9C78 File Offset: 0x000F7E78
		public override void SendUdpMessage<T>(ref T iMessage, int iClientIndex)
		{
			lock (this.mTxBuffer)
			{
				this.QueueUDPMessage<T>(iClientIndex, ref iMessage);
			}
		}

		// Token: 0x0600228E RID: 8846 RVA: 0x000F9CB4 File Offset: 0x000F7EB4
		public override void SendUdpMessage<T>(ref T iMessage, SteamID iClientID)
		{
			int client = this.GetClient(iClientID);
			if (client >= 0)
			{
				this.QueueUDPMessage<T>(client, ref iMessage);
				return;
			}
			this.mTxBuffer[0] = (byte)iMessage.PacketType;
			this.mWriter.BaseStream.Position = 1L;
			iMessage.Write(this.mWriter);
			uint num = (uint)this.mWriter.BaseStream.Position;
			SteamGameServerNetworking.SendP2PPacket(iClientID, this.mTxBuffer, num, P2PSend.UnreliableNoDelay);
			long ticks = DateTime.Now.Ticks;
			int num2;
			if (!this.mBytesSent.TryGetValue(ticks, out num2))
			{
				num2 = 0;
			}
			this.mBytesSent[ticks] = num2 + (int)num;
		}

		// Token: 0x0600228F RID: 8847 RVA: 0x000F9D60 File Offset: 0x000F7F60
		public unsafe override void SendRaw(PacketType iType, void* iPtr, int iLength)
		{
			for (int i = 0; i < this.mClients.Count; i++)
			{
				this.SendRaw(iType, iPtr, iLength, this.mClients[i].ID);
			}
		}

		// Token: 0x06002290 RID: 8848 RVA: 0x000F9D9D File Offset: 0x000F7F9D
		public unsafe override void SendRaw(PacketType iType, void* iPtr, int iLength, int iPeer)
		{
			this.SendRaw(iType, iPtr, iLength, this.mClients[iPeer].ID);
		}

		// Token: 0x06002291 RID: 8849 RVA: 0x000F9DBC File Offset: 0x000F7FBC
		public unsafe override void SendRaw(PacketType iType, void* iPtr, int iLength, SteamID iPeer)
		{
			byte[] array = new byte[iLength + 1 + 4];
			array[0] = (byte)iType;
			array[1] = (byte)iLength;
			array[2] = (byte)(iLength >> 8);
			array[3] = (byte)(iLength >> 16);
			array[4] = (byte)(iLength >> 24);
			if (iLength > 0)
			{
				Marshal.Copy(new IntPtr(iPtr), array, 5, iLength);
			}
			SteamGameServerNetworking.SendP2PPacket(iPeer, array, (uint)array.Length, P2PSend.ReliableWithBuffering);
		}

		// Token: 0x06002292 RID: 8850 RVA: 0x000F9E4C File Offset: 0x000F804C
		public override void Dispose()
		{
			Player[] players = Game.Instance.Players;
			for (int i = 0; i < players.Length; i++)
			{
				if (players[i].Playing)
				{
					GamerLeaveMessage gamerLeaveMessage = default(GamerLeaveMessage);
					gamerLeaveMessage.Id = (byte)i;
					this.SendMessage<GamerLeaveMessage>(ref gamerLeaveMessage);
					if (players[i].Gamer is NetworkGamer)
					{
						players[i].Playing = false;
					}
				}
			}
			NetworkManager.Instance.ClearHost();
			this.mListening = false;
			for (int j = 0; j < this.mClients.Count; j++)
			{
				SteamNetworking.CloseP2PSessionWithUser(this.mClients[j].ID);
			}
			SteamMasterServerUpdater.SetActive(false);
			SteamMatchmaking.LeaveLobby(this.mSteamIDLobby);
			SteamGameServer.SteamServersConnected -= this.SteamGameServer_SteamServersConnected;
			SteamGameServer.SteamServersDisconnected -= this.SteamGameServer_SteamServersDisconnected;
			SteamGameServer.GSPolicyResponse -= this.SteamGameServer_GSPolicyResponse;
			SteamGameServer.GSClientApprove -= this.SteamGameServer_GSClientApprove;
			SteamGameServer.GSClientDeny -= this.SteamGameServer_GSClientDeny;
			SteamGameServer.GSClientKick -= this.SteamGameServer_GSClientKick;
			SteamGameServer.P2PSessionRequest -= this.SteamGameServer_P2PSessionRequest;
			SteamGameServer.P2PSessionConnectFail -= this.SteamGameServer_P2PSessionConnectFail;
			SteamGameServer.LogOff();
			SteamMasterServerUpdater.NotifyShutdown();
			SteamGameServer.Shutdown();
			Game.Instance.AddLoadTask(delegate
			{
				try
				{
					if (this.mUPnPEnabled)
					{
						NAT.DeleteForwardingRule(27016, ProtocolType.Udp);
					}
				}
				catch
				{
				}
			});
		}

		// Token: 0x17000839 RID: 2105
		// (get) Token: 0x06002293 RID: 8851 RVA: 0x000F9FA9 File Offset: 0x000F81A9
		public override int Connections
		{
			get
			{
				return this.mClients.Count;
			}
		}

		// Token: 0x06002294 RID: 8852 RVA: 0x000F9FB6 File Offset: 0x000F81B6
		public override float GetLatency(int iConnection)
		{
			return this.mClients[iConnection].Latency;
		}

		// Token: 0x06002295 RID: 8853 RVA: 0x000F9FC9 File Offset: 0x000F81C9
		public override int GetLatencyMS(int iConnection)
		{
			return (int)(this.mClients[iConnection].Latency * 1000f);
		}

		// Token: 0x06002296 RID: 8854 RVA: 0x000F9FE4 File Offset: 0x000F81E4
		public override float GetLatency(SteamID iConnection)
		{
			for (int i = 0; i < this.mClients.Count; i++)
			{
				NetworkServer.Connection connection = this.mClients[i];
				if (connection.ID == iConnection)
				{
					return connection.Latency;
				}
			}
			return 0f;
		}

		// Token: 0x06002297 RID: 8855 RVA: 0x000FA030 File Offset: 0x000F8230
		public override int GetLatencyMS(SteamID iConnection)
		{
			for (int i = 0; i < this.mClients.Count; i++)
			{
				NetworkServer.Connection connection = this.mClients[i];
				if (connection.ID == iConnection)
				{
					return (int)(connection.Latency * 1000f);
				}
			}
			return 0;
		}

		// Token: 0x06002298 RID: 8856 RVA: 0x000FA080 File Offset: 0x000F8280
		public void CloseConnection(SteamID iClient, ConnectionClosedMessage.CReason iReason)
		{
			for (int i = 0; i < this.mClients.Count; i++)
			{
				if (this.mClients[i].ID == iClient)
				{
					this.CloseConnection(i, iReason);
					return;
				}
			}
		}

		// Token: 0x06002299 RID: 8857 RVA: 0x000FA0C8 File Offset: 0x000F82C8
		public void CloseConnection(int iClient, ConnectionClosedMessage.CReason iReason)
		{
			NetworkServer.Connection connection = this.mClients[iClient];
			switch (iReason)
			{
			case ConnectionClosedMessage.CReason.Kicked:
			{
				string iMessage = string.Format("{0} {1}", SteamFriends.GetFriendPersonaName(this.mClients[iClient].ID), LanguageManager.Instance.GetString("#add_menu_not_kicked".GetHashCodeCustom()));
				NetworkChat.Instance.AddMessage(iMessage);
				ConnectionClosedMessage connectionClosedMessage = default(ConnectionClosedMessage);
				connectionClosedMessage.Reason = ConnectionClosedMessage.CReason.Kicked;
				this.SendMessage<ConnectionClosedMessage>(ref connectionClosedMessage, iClient);
				Thread.Sleep(250);
				break;
			}
			case ConnectionClosedMessage.CReason.Left:
			{
				string iMessage2 = string.Format("{0} {1}", SteamFriends.GetFriendPersonaName(this.mClients[iClient].ID), LanguageManager.Instance.GetString("#add_menu_not_left".GetHashCodeCustom()));
				NetworkChat.Instance.AddMessage(iMessage2);
				break;
			}
			default:
			{
				string text = LanguageManager.Instance.GetString("#add_connectiontolost".GetHashCodeCustom());
				text = text.Replace("#1;", SteamFriends.GetFriendPersonaName(this.mClients[iClient].ID));
				NetworkChat.Instance.AddMessage(text);
				break;
			}
			}
			SteamGameServer.SendUserDisconnect(connection.ID);
			SteamGameServerNetworking.CloseP2PSessionWithUser(connection.ID);
			this.mClients.RemoveAt(iClient);
			foreach (Player player in Game.Instance.Players)
			{
				NetworkGamer networkGamer = player.Gamer as NetworkGamer;
				if (networkGamer != null && networkGamer.ClientID == connection.ID)
				{
					player.Leave();
				}
			}
			SteamMatchmaking.SetLobbyData(this.mSteamIDLobby, "slots", (4 - this.mClients.Count).ToString());
		}

		// Token: 0x0600229A RID: 8858 RVA: 0x000FA28C File Offset: 0x000F848C
		public override void CloseConnection()
		{
			ConnectionClosedMessage connectionClosedMessage = default(ConnectionClosedMessage);
			connectionClosedMessage.Reason = ConnectionClosedMessage.CReason.Left;
			this.SendMessage<ConnectionClosedMessage>(ref connectionClosedMessage);
			Thread.Sleep(250);
		}

		// Token: 0x0600229B RID: 8859 RVA: 0x000FA2BB File Offset: 0x000F84BB
		internal SteamID GetSteamID(int iIndex)
		{
			return this.mClients[iIndex].ID;
		}

		// Token: 0x0600229C RID: 8860 RVA: 0x000FA2D0 File Offset: 0x000F84D0
		private void QueueUDPMessage<T>(int iClient, ref T iMessage) where T : ISendable
		{
			NetworkServer.Connection connection;
			lock (this.mClients)
			{
				connection = this.mClients[iClient];
			}
			long position = connection.Writer.BaseStream.Position;
			connection.Writer.Write((byte)iMessage.PacketType);
			iMessage.Write(connection.Writer);
			if (connection.Writer.BaseStream.Position > 1100L)
			{
				this.FlushUDPBatch(iClient, (uint)position);
				connection.Writer.Write((byte)iMessage.PacketType);
				iMessage.Write(connection.Writer);
			}
		}

		// Token: 0x0600229D RID: 8861 RVA: 0x000FA39C File Offset: 0x000F859C
		private void FlushUDPBatch(int iClient, uint iLength)
		{
			NetworkServer.Connection connection;
			lock (this.mClients)
			{
				connection = this.mClients[iClient];
			}
			SteamGameServerNetworking.SendP2PPacket(connection.ID, connection.TxBuffer, iLength, P2PSend.UnreliableNoDelay);
			long ticks = DateTime.Now.Ticks;
			int num;
			if (!this.mBytesSent.TryGetValue(ticks, out num))
			{
				num = 0;
			}
			lock (this.mBytesSent)
			{
				this.mBytesSent[ticks] = num + (int)iLength;
			}
			connection.Writer.BaseStream.Position = 0L;
		}

		// Token: 0x0600229E RID: 8862 RVA: 0x000FA45C File Offset: 0x000F865C
		private void SendForcedSyncMessageToClient(SteamID iSteamID, bool syncAfterwards)
		{
			this.SendForcedSyncMessageToClient(this.GetClient(iSteamID), syncAfterwards);
		}

		// Token: 0x0600229F RID: 8863 RVA: 0x000FA46C File Offset: 0x000F866C
		private void SendForcedSyncMessageToClient(Avatar iAvatar, bool syncAfterwards)
		{
			if (iAvatar == null)
			{
				return;
			}
			Player player = iAvatar.Player;
			if (player == null)
			{
				return;
			}
			if (player.Gamer == null)
			{
				return;
			}
			NetworkGamer networkGamer = player.Gamer as NetworkGamer;
			if (networkGamer == null)
			{
				return;
			}
			int client = this.GetClient(networkGamer.ClientID);
			this.SendForcedSyncMessageToClient(client, syncAfterwards);
		}

		// Token: 0x060022A0 RID: 8864 RVA: 0x000FA4B8 File Offset: 0x000F86B8
		private void SendForcedSyncMessageToClient(int clientIndex, bool syncAfterwards)
		{
			Player[] players = Game.Instance.Players;
			ForceSyncPlayerStatusesMessage forceSyncPlayerStatusesMessage = default(ForceSyncPlayerStatusesMessage);
			int num = 0;
			for (int i = 0; i < players.Length; i++)
			{
				if (players[i] != null && players[i].Gamer != null && players[i].Gamer is NetworkGamer)
				{
					num++;
				}
			}
			if (num == 0)
			{
				return;
			}
			forceSyncPlayerStatusesMessage.playerUpdateMessages = new EntityUpdateMessage[num];
			forceSyncPlayerStatusesMessage.numPlayers = (short)num;
			EntityUpdateMessage[] playerUpdateMessages = new EntityUpdateMessage[num];
			for (int j = 0; j < num; j++)
			{
				players[j].Avatar.GetNetworkUpdate(out forceSyncPlayerStatusesMessage.playerUpdateMessages[j], NetworkState.Server, 1f);
				forceSyncPlayerStatusesMessage.playerUpdateMessages[j].Handle = (ushort)players[j].ID;
			}
			forceSyncPlayerStatusesMessage.playerUpdateMessages = playerUpdateMessages;
			this.SendMessage<ForceSyncPlayerStatusesMessage>(ref forceSyncPlayerStatusesMessage, clientIndex, P2PSend.Reliable);
			if (syncAfterwards)
			{
				this.Sync();
			}
		}

		// Token: 0x040025C0 RID: 9664
		private const uint ANY_IP = 0U;

		// Token: 0x040025C1 RID: 9665
		public const int GAME_PORT = 27015;

		// Token: 0x040025C2 RID: 9666
		public const int QUERY_PORT = 27016;

		// Token: 0x040025C3 RID: 9667
		public const int AUTHENTICATION_PORT = 8766;

		// Token: 0x040025C4 RID: 9668
		private SteamID mSteamIDLobby;

		// Token: 0x040025C5 RID: 9669
		private byte[] mRxBuffer;

		// Token: 0x040025C6 RID: 9670
		private byte[] mTxBuffer;

		// Token: 0x040025C7 RID: 9671
		private string mPassword;

		// Token: 0x040025C8 RID: 9672
		private BinaryReader mReader;

		// Token: 0x040025C9 RID: 9673
		private BinaryWriter mWriter;

		// Token: 0x040025CA RID: 9674
		private uint mSyncPointCounter;

		// Token: 0x040025CB RID: 9675
		private List<NetworkServer.PendingUser> mPendingClients = new List<NetworkServer.PendingUser>();

		// Token: 0x040025CC RID: 9676
		private byte[] mDummyData = new byte[1];

		// Token: 0x040025CD RID: 9677
		private bool mUPnPEnabled;

		// Token: 0x040025CE RID: 9678
		private bool mPlaying;

		// Token: 0x040025CF RID: 9679
		private long mLastLatencyCheck = DateTime.Now.Ticks;

		// Token: 0x040025D0 RID: 9680
		private bool mListening;

		// Token: 0x040025D1 RID: 9681
		private byte[] mLocalIPNumber = new byte[]
		{
			127,
			0,
			0,
			1
		};

		// Token: 0x040025D2 RID: 9682
		private List<NetworkServer.Connection> mClients = new List<NetworkServer.Connection>(8);

		// Token: 0x040025D3 RID: 9683
		private OptionsMessageBox mLostConnectionMB;

		// Token: 0x040025D4 RID: 9684
		private SortedList<long, int> mBytesSent = new SortedList<long, int>();

		// Token: 0x040025D5 RID: 9685
		private int mBytesPerSecond;

		// Token: 0x040025D6 RID: 9686
		private bool mVAC;

		// Token: 0x040025D7 RID: 9687
		private string mLevelName;

		// Token: 0x02000476 RID: 1142
		private struct Connection
		{
			// Token: 0x040025D8 RID: 9688
			public SteamID ID;

			// Token: 0x040025D9 RID: 9689
			public byte[] TxBuffer;

			// Token: 0x040025DA RID: 9690
			public BinaryWriter Writer;

			// Token: 0x040025DB RID: 9691
			public List<uint> SyncPoints;

			// Token: 0x040025DC RID: 9692
			public Stopwatch Timer;

			// Token: 0x040025DD RID: 9693
			public float Latency;

			// Token: 0x040025DE RID: 9694
			public bool Ready;
		}

		// Token: 0x02000477 RID: 1143
		private struct PendingUser
		{
			// Token: 0x040025DF RID: 9695
			public SteamID ID;

			// Token: 0x040025E0 RID: 9696
			public byte Players;
		}
	}
}
