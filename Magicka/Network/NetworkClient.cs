using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using Magicka.Audio;
using Magicka.DRM;
using Magicka.GameLogic;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.Abilities.SpecialAbilities;
using Magicka.GameLogic.Entities.Bosses;
using Magicka.GameLogic.Entities.CharacterStates;
using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.GameLogic.GameStates.Menu.Main;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.Spells.SpellEffects;
using Magicka.GameLogic.Statistics;
using Magicka.GameLogic.UI;
using Magicka.Gamers;
using Magicka.Levels;
using Magicka.Levels.Packs;
using Magicka.Levels.Triggers;
using Magicka.Levels.Versus;
using Magicka.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using SteamWrapper;

namespace Magicka.Network
{
	// Token: 0x02000374 RID: 884
	internal class NetworkClient : NetworkInterface
	{
		// Token: 0x06001AED RID: 6893 RVA: 0x000B65E8 File Offset: 0x000B47E8
		public NetworkClient(SteamID iSteamID, Action<ConnectionStatus> iOnComplete)
		{
			Player[] players = Game.Instance.Players;
			for (int i = 0; i < players.Length; i++)
			{
				if (players[i].Playing)
				{
					players[i].Leave();
				}
			}
			this.mConnections = new List<NetworkClient.Connection>();
			this.mOnComplete = iOnComplete;
			this.mServerID = iSteamID;
			this.mTxBuffer = new byte[1048576];
			this.mRxBuffer = new byte[1048576];
			this.mReader = new BinaryReader(new MemoryStream(this.mRxBuffer));
			this.mWriter = new BinaryWriter(new MemoryStream(this.mTxBuffer));
			NetworkClient.Connection item;
			item.ID = this.mServerID;
			item.Timer = new Stopwatch();
			item.Latency = 0f;
			item.TxBuffer = new byte[4400];
			item.Writer = new BinaryWriter(new MemoryStream(item.TxBuffer));
			this.mConnections.Add(item);
			ConnectRequestMessage connectRequestMessage = default(ConnectRequestMessage);
			this.SendMessage<ConnectRequestMessage>(ref connectRequestMessage);
			this.mLastLatencyCheck = DateTime.Now.Ticks;
			SteamAPI.P2PSessionRequest += this.SteamAPI_P2PSessionRequest;
			SteamAPI.P2PSessionConnectFail += this.SteamAPI_P2PSessionConnectFail;
			this.mErrorMessageBox = new OptionsMessageBox(SubMenuOnline.LOC_ERROR_UNKNOWN, new int[]
			{
				Defines.LOC_GEN_OK
			});
		}

		// Token: 0x1700069E RID: 1694
		// (get) Token: 0x06001AEE RID: 6894 RVA: 0x000B677B File Offset: 0x000B497B
		public override SteamID ServerID
		{
			get
			{
				return this.mServerID;
			}
		}

		// Token: 0x1700069F RID: 1695
		// (get) Token: 0x06001AEF RID: 6895 RVA: 0x000B6783 File Offset: 0x000B4983
		public override bool IsVACSecure
		{
			get
			{
				return this.mVAC;
			}
		}

		// Token: 0x06001AF0 RID: 6896 RVA: 0x000B678B File Offset: 0x000B498B
		private void SteamAPI_P2PSessionRequest(P2PSessionRequest iParam)
		{
			SteamNetworking.AcceptP2PSessionWithUser(iParam.mSteamIDRemote);
		}

		// Token: 0x06001AF1 RID: 6897 RVA: 0x000B679C File Offset: 0x000B499C
		private void SteamAPI_P2PSessionConnectFail(P2PSessionConnectFail iParam)
		{
			lock (this.mConnections)
			{
				for (int i = 0; i < this.mConnections.Count; i++)
				{
					if (this.mConnections[i].ID == iParam.mSteamIDRemote)
					{
						if (i == 0)
						{
							ErrorMessageBox.Instance.Show(LanguageManager.Instance.GetString(SubMenuOnline.LOC_ERROR_AUTHFAIL));
							this.Dispose();
							return;
						}
						this.mConnections.RemoveAt(i--);
					}
				}
			}
			SteamNetworking.CloseP2PSessionWithUser(iParam.mSteamIDRemote);
		}

		// Token: 0x06001AF2 RID: 6898 RVA: 0x000B6848 File Offset: 0x000B4A48
		private void OnJoinLobby(LobbyEnter iLobby)
		{
		}

		// Token: 0x06001AF3 RID: 6899 RVA: 0x000B684C File Offset: 0x000B4A4C
		public override void Sync()
		{
			while (this.mSyncPoints.Count == 0)
			{
				Thread.Sleep(10);
			}
			EnterSyncMessage enterSyncMessage;
			lock (this.mSyncPoints)
			{
				enterSyncMessage.ID = this.mSyncPoints.Dequeue();
			}
			this.SendMessage<EnterSyncMessage>(ref enterSyncMessage, 0);
			while (!this.mFinishedSyncPoints.Contains(enterSyncMessage.ID))
			{
				Thread.Sleep(10);
			}
			lock (this.mFinishedSyncPoints)
			{
				this.mFinishedSyncPoints.Remove(enterSyncMessage.ID);
			}
		}

		// Token: 0x06001AF4 RID: 6900 RVA: 0x000B6904 File Offset: 0x000B4B04
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
			ConnectionStatus connectionStatus = NetworkManager.Instance.ConnectionStatus;
			if (connectionStatus == ConnectionStatus.Connecting)
			{
				if (ticks - this.mLastLatencyCheck > 70000000L)
				{
					this.mLastLatencyCheck = ticks;
					this.mRetryCount++;
					if (this.mRetryCount >= 3)
					{
						NetworkManager.Instance.ConnectionStatus = ConnectionStatus.Failed_Timeout;
						this.mOnComplete(NetworkManager.Instance.ConnectionStatus);
						NetworkManager.Instance.EndSession();
						return;
					}
					ConnectRequestMessage connectRequestMessage = default(ConnectRequestMessage);
					this.SendMessage<ConnectRequestMessage>(ref connectRequestMessage);
				}
			}
			else if (ticks - this.mLastLatencyCheck > 10000000L)
			{
				for (int j = 0; j < this.mConnections.Count; j++)
				{
					NetworkClient.Connection connection = this.mConnections[j];
					connection.Timer.Reset();
					connection.Timer.Start();
					PingRequestMessage pingRequestMessage = default(PingRequestMessage);
					pingRequestMessage.Payload = (int)ticks;
					this.SendUdpMessage<PingRequestMessage>(ref pingRequestMessage, j);
				}
				this.mLastLatencyCheck = ticks;
			}
			uint num2;
			while (SteamNetworking.IsP2PPacketAvailable(out num2))
			{
				SteamID steamID;
				if (SteamNetworking.ReadP2PPacket(this.mRxBuffer, out num2, out steamID) && this.GetConnection(steamID) >= 0)
				{
					this.mReader.BaseStream.Position = 0L;
					while (this.mReader.BaseStream.Position < (long)((ulong)num2))
					{
						this.ReadMessage(this.mReader, steamID);
					}
				}
			}
		}

		// Token: 0x06001AF5 RID: 6901 RVA: 0x000B6AD8 File Offset: 0x000B4CD8
		public override void FlushMessageBuffers()
		{
			for (int i = 0; i < this.mConnections.Count; i++)
			{
				NetworkClient.Connection connection = this.mConnections[i];
				uint num = (uint)connection.Writer.BaseStream.Position;
				if (num > 0U)
				{
					this.FlushUDPBatch(i, num);
				}
				P2PSessionState p2PSessionState;
				SteamNetworking.GetP2PSessionState(connection.ID, out p2PSessionState);
				if (p2PSessionState.ConnectionActive && p2PSessionState.PacketsQueuedForSend > 0)
				{
					SteamNetworking.SendP2PPacket(connection.ID, this.mDummyData, 0U, P2PSend.Reliable);
				}
			}
		}

		// Token: 0x06001AF6 RID: 6902 RVA: 0x000B6B60 File Offset: 0x000B4D60
		private int GetConnection(SteamID iID)
		{
			for (int i = 0; i < this.mConnections.Count; i++)
			{
				if (this.mConnections[i].ID == iID)
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x06001AF7 RID: 6903 RVA: 0x000B6BA0 File Offset: 0x000B4DA0
		private unsafe void ReadMessage(BinaryReader iReader, SteamID iSender)
		{
			PacketType packetType = PacketType.ChatMessage;
			try
			{
				packetType = (PacketType)iReader.ReadByte();
				if ((byte)(packetType & PacketType.Request) != 128)
				{
					switch (packetType)
					{
					case PacketType.Ping:
					{
						PingReplyMessage pingReplyMessage = default(PingReplyMessage);
						pingReplyMessage.Read(iReader);
						if (pingReplyMessage.Payload == (int)this.mLastLatencyCheck)
						{
							int connection = this.GetConnection(iSender);
							NetworkClient.Connection value = this.mConnections[connection];
							value.Timer.Stop();
							value.Latency = (float)value.Timer.Elapsed.TotalSeconds;
							this.mConnections[connection] = value;
							goto IL_17EB;
						}
						goto IL_17EB;
					}
					case PacketType.Connect:
					{
						ConnectReplyMessage connectReplyMessage = default(ConnectReplyMessage);
						connectReplyMessage.Read(iReader);
						this.mVAC = connectReplyMessage.VACSecure;
						if (this.mVAC && HackHelper.LicenseStatus == HackHelper.Status.Hacked)
						{
							return;
						}
						NetworkManager.Instance.ConnectionStatus = ConnectionStatus.Authenticating;
						NetworkManager.Instance.GameName = connectReplyMessage.ServerName;
						string text = LanguageManager.Instance.GetString("#add_menu_not_connected".GetHashCodeCustom());
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
						text = text.Replace("#1;", str + connectReplyMessage.ServerName + "[/c]");
						text = text.Replace("#2;", str + 27016.ToString() + "[/c]");
						NetworkChat.Instance.SetTitle(text);
						P2PSessionState p2PSessionState;
						SteamNetworking.GetP2PSessionState(connectReplyMessage.ServerID, out p2PSessionState);
						AuthenticateRequestMessage authenticateRequestMessage = default(AuthenticateRequestMessage);
						authenticateRequestMessage.NrOfPlayers = (byte)Game.Instance.PlayerCount;
						authenticateRequestMessage.Version = Game.Instance.Version;
						authenticateRequestMessage.Password = NetworkManager.Instance.Password;
						authenticateRequestMessage.Token.Length = SteamUser.InitiateGameConnection((void*)(&authenticateRequestMessage.Token.Data.FixedElementField), 1024, connectReplyMessage.ServerID, p2PSessionState.RemoteIP, p2PSessionState.RemotePort, connectReplyMessage.VACSecure);
						this.SendMessage<AuthenticateRequestMessage>(ref authenticateRequestMessage, 0);
						goto IL_17EB;
					}
					case PacketType.Authenticate:
					{
						AuthenticateReplyMessage authenticateReplyMessage = default(AuthenticateReplyMessage);
						authenticateReplyMessage.Read(iReader);
						if (!(iSender != this.mServerID))
						{
							if (authenticateReplyMessage.Response == AuthenticateReplyMessage.Reply.Ok)
							{
								NetworkManager.Instance.ConnectionStatus = ConnectionStatus.Connected;
								if (authenticateReplyMessage.GameInfo.GameName != null)
								{
									NetworkManager.Instance.GameName = authenticateReplyMessage.GameInfo.GameName;
									SubMenuCharacterSelect.Instance.SetSettings(authenticateReplyMessage.GameInfo.GameType, authenticateReplyMessage.GameInfo.Level, false);
									if (authenticateReplyMessage.GameInfo.GameType == GameType.Versus)
									{
										SubMenuCharacterSelect.Instance.SetVsSettings(ref authenticateReplyMessage.VersusSettings);
									}
								}
							}
							else
							{
								if (authenticateReplyMessage.Response == AuthenticateReplyMessage.Reply.Error_AuthFailed)
								{
									ErrorMessageBox.Instance.Show(LanguageManager.Instance.GetString(SubMenuOnline.LOC_ERROR_AUTHFAIL));
									NetworkManager.Instance.ConnectionStatus = ConnectionStatus.Failed_Authentication;
								}
								else if (authenticateReplyMessage.Response == AuthenticateReplyMessage.Reply.Error_ServerFull)
								{
									ErrorMessageBox.Instance.Show(LanguageManager.Instance.GetString(SubMenuOnline.LOC_ERROR_SERVERFULL));
									NetworkManager.Instance.ConnectionStatus = ConnectionStatus.Failed_GameFull;
								}
								else if (authenticateReplyMessage.Response == AuthenticateReplyMessage.Reply.Error_GamePlaying)
								{
									ErrorMessageBox.Instance.Show(LanguageManager.Instance.GetString(SubMenuOnline.LOC_ERROR_INPROGRESS));
									NetworkManager.Instance.ConnectionStatus = ConnectionStatus.Failed_GamePlaying;
								}
								else if (authenticateReplyMessage.Response == AuthenticateReplyMessage.Reply.Error_Version)
								{
									ErrorMessageBox.Instance.Show(LanguageManager.Instance.GetString(SubMenuOnline.LOC_ERROR_VERSION));
									NetworkManager.Instance.ConnectionStatus = ConnectionStatus.Failed_Version;
								}
								else if (authenticateReplyMessage.Response == AuthenticateReplyMessage.Reply.Error_Password)
								{
									ErrorMessageBox.Instance.Show(LanguageManager.Instance.GetString(SubMenuOnline.LOC_ERROR_PASSWORD));
									NetworkManager.Instance.ConnectionStatus = ConnectionStatus.Failed_Password;
								}
								else
								{
									ErrorMessageBox.Instance.Show(LanguageManager.Instance.GetString(SubMenuOnline.LOC_ERROR_UNKNOWN));
									NetworkManager.Instance.ConnectionStatus = ConnectionStatus.Failed_Unknown;
								}
								NetworkManager.Instance.EndSession();
							}
							this.mOnComplete(NetworkManager.Instance.ConnectionStatus);
							goto IL_17EB;
						}
						goto IL_17EB;
					}
					case PacketType.LobbyInfo:
					{
						LobbyInfoMessage lobbyInfoMessage = default(LobbyInfoMessage);
						lobbyInfoMessage.Read(iReader);
						this.mSteamIDLobby = new SteamID(lobbyInfoMessage.SteamID);
						SteamMatchmaking.JoinLobby(this.mSteamIDLobby, new Action<LobbyEnter>(this.OnJoinLobby));
						goto IL_17EB;
					}
					case PacketType.ClientConnected:
					{
						ClientConnectedMessage clientConnectedMessage = default(ClientConnectedMessage);
						clientConnectedMessage.Read(iReader);
						if (this.GetConnection(iSender) == 0)
						{
							NetworkClient.Connection item;
							item.ID = clientConnectedMessage.ID;
							item.Timer = new Stopwatch();
							item.Latency = 0f;
							item.TxBuffer = new byte[4400];
							item.Writer = new BinaryWriter(new MemoryStream(item.TxBuffer));
							this.mConnections.Add(item);
							this.SendMessage<ClientConnectedMessage>(ref clientConnectedMessage, this.mConnections.Count - 1);
							clientConnectedMessage.ID = SteamUser.GetSteamID();
							goto IL_17EB;
						}
						goto IL_17EB;
					}
					case PacketType.ConnectionClosed:
					{
						ConnectionClosedMessage connectionClosedMessage = default(ConnectionClosedMessage);
						connectionClosedMessage.Read(iReader);
						if (iSender == this.mServerID)
						{
							ConnectionClosedMessage.CReason reason = connectionClosedMessage.Reason;
							if (reason == ConnectionClosedMessage.CReason.Kicked)
							{
								ErrorMessageBox.Instance.Show(LanguageManager.Instance.GetString("#add_connectionlost".GetHashCodeCustom()));
							}
							else
							{
								ErrorMessageBox.Instance.Show(LanguageManager.Instance.GetString("#add_connectionlost".GetHashCodeCustom()));
							}
							NetworkManager.Instance.EndSession();
						}
						else
						{
							this.CloseConnection(iSender, connectionClosedMessage.Reason);
						}
						return;
					}
					case PacketType.GameFull:
					case PacketType.LevelList:
					case PacketType.GameChanged:
					case PacketType.GameBeginSceneChange:
					case PacketType.ScoreAdded:
					case PacketType.GameUpdate:
					case PacketType.PlayerUpdate:
					case PacketType.Damage:
						goto IL_17CB;
					case PacketType.GameInfo:
					{
						GameInfoMessage gameInfoMessage = default(GameInfoMessage);
						gameInfoMessage.Read(iReader);
						SubMenuCharacterSelect.Instance.SetSettings(gameInfoMessage.GameType, gameInfoMessage.Level, false);
						goto IL_17EB;
					}
					case PacketType.VersusOptions:
					{
						VersusRuleset.Settings.OptionsMessage optionsMessage = default(VersusRuleset.Settings.OptionsMessage);
						optionsMessage.Read(iReader);
						SubMenuCharacterSelect.Instance.SetVsSettings(ref optionsMessage);
						goto IL_17EB;
					}
					case PacketType.PackOptions:
						default(PackOptionsMessage).Read(iReader);
						goto IL_17EB;
					case PacketType.ChatMessage:
					{
						ChatMessage chatMessage = default(ChatMessage);
						chatMessage.Read(iReader);
						NetworkChat.Instance.AddMessage(chatMessage.ToString());
						goto IL_17EB;
					}
					case PacketType.GamerJoin:
					{
						GamerJoinAcceptMessage gamerJoinAcceptMessage = default(GamerJoinAcceptMessage);
						gamerJoinAcceptMessage.Read(iReader);
						Player.JoinServerGranted(ref gamerJoinAcceptMessage);
						goto IL_17EB;
					}
					case PacketType.GamerChanged:
					{
						GamerChangedMessage gamerChangedMessage = default(GamerChangedMessage);
						gamerChangedMessage.Read(iReader);
						Player player = Game.Instance.Players[(int)gamerChangedMessage.Id];
						player.UnlockedMagicks = gamerChangedMessage.UnlockedMagicks;
						Gamer gamer = player.Gamer;
						if (gamer != null)
						{
							gamer.Color = gamerChangedMessage.Color;
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
							avatar.Type = gamerChangedMessage.AvatarType.GetHashCodeCustom();
							avatar.Name = player.Gamer.Avatar.Name;
							avatar.AllowCampaign = gamerChangedMessage.AvatarAllowCampaign;
							avatar.AllowChallenge = gamerChangedMessage.AvatarAllowChallenge;
							avatar.AllowPVP = gamerChangedMessage.AvatarAllowPVP;
							gamer.Avatar = avatar;
							gamer.GamerTag = gamerChangedMessage.GamerTag;
							SubMenuCharacterSelect.Instance.UpdateGamer(player, gamer);
							goto IL_17EB;
						}
						goto IL_17EB;
					}
					case PacketType.GamerLeave:
					{
						GamerLeaveMessage gamerLeaveMessage = default(GamerLeaveMessage);
						gamerLeaveMessage.Read(iReader);
						Player player2 = Game.Instance.Players[(int)gamerLeaveMessage.Id];
						player2.Playing = false;
						player2.Team = Factions.NONE;
						player2.Gamer = null;
						SubMenuCharacterSelect.Instance.UpdateGamer(player2, null);
						goto IL_17EB;
					}
					case PacketType.GamerReady:
					{
						GamerReadyMessage gamerReadyMessage = default(GamerReadyMessage);
						gamerReadyMessage.Read(iReader);
						SubMenuCharacterSelect.Instance.SetReady(gamerReadyMessage.Ready, gamerReadyMessage.Id);
						goto IL_17EB;
					}
					case PacketType.SaveData:
						this.mSaveSlot.Read(iReader);
						goto IL_17EB;
					case PacketType.MenuSelection:
					{
						MenuSelectMessage menuSelectMessage = default(MenuSelectMessage);
						menuSelectMessage.Read(iReader);
						MenuState.Instance.NetworkInput(ref menuSelectMessage);
						goto IL_17EB;
					}
					case PacketType.GameEndLoad:
						default(GameEndLoadMessage).Read(iReader);
						PlayState.WaitingForPlayers = false;
						goto IL_17EB;
					case PacketType.GameEnd:
					{
						GameEndMessage gameEndMessage = default(GameEndMessage);
						gameEndMessage.Read(iReader);
						PlayState.RecentPlayState.Endgame(ref gameEndMessage);
						goto IL_17EB;
					}
					case PacketType.GameRestart:
					{
						GameRestartMessage gameRestartMessage = default(GameRestartMessage);
						gameRestartMessage.Read(iReader);
						PlayState.RecentPlayState.Restart(this, gameRestartMessage.Type);
						goto IL_17EB;
					}
					case PacketType.TriggerAction:
					{
						TriggerActionMessage triggerActionMessage = default(TriggerActionMessage);
						triggerActionMessage.Read(iReader);
						Trigger.NetworkAction(ref triggerActionMessage);
						goto IL_17EB;
					}
					case PacketType.StatisticsUpdate:
					{
						StatisticsMessage statisticsMessage = default(StatisticsMessage);
						statisticsMessage.Read(iReader);
						StatisticsManager.Instance.NetworkUpdate(ref statisticsMessage);
						goto IL_17EB;
					}
					case PacketType.LeaderboardEntry:
					{
						LeaderboardMessage leaderboardMessage = default(LeaderboardMessage);
						leaderboardMessage.Read(iReader);
						SteamUserStats.UploadLeaderboardScore(leaderboardMessage.SteamLeaderboard, leaderboardMessage.ScoreMethod, leaderboardMessage.Score, new int[]
						{
							leaderboardMessage.Data
						}, new Action<LeaderboardScoreUploaded>(StatisticsManager.Instance.OnlineScoreUploaded));
						goto IL_17EB;
					}
					case PacketType.RulesetUpdate:
					{
						RulesetMessage rulesetMessage = default(RulesetMessage);
						rulesetMessage.Read(iReader);
						PlayState recentPlayState = PlayState.RecentPlayState;
						if (recentPlayState.Level.CurrentScene.RuleSet != null)
						{
							recentPlayState.Level.CurrentScene.RuleSet.NetworkUpdate(ref rulesetMessage);
							goto IL_17EB;
						}
						goto IL_17EB;
					}
					case PacketType.DialogAdvance:
					{
						DialogAdvanceMessage dialogAdvanceMessage = default(DialogAdvanceMessage);
						dialogAdvanceMessage.Read(iReader);
						DialogManager.Instance.NetworkAdvance(dialogAdvanceMessage.Interact);
						goto IL_17EB;
					}
					case PacketType.EntityUpdate:
					{
						EntityUpdateMessage entityUpdateMessage = default(EntityUpdateMessage);
						entityUpdateMessage.Read(iReader);
						Entity fromHandle = Entity.GetFromHandle((int)entityUpdateMessage.Handle);
						if (fromHandle != null)
						{
							fromHandle.NetworkUpdate(iSender, ref entityUpdateMessage);
							goto IL_17EB;
						}
						goto IL_17EB;
					}
					case PacketType.CharacterAction:
					{
						CharacterActionMessage characterActionMessage = default(CharacterActionMessage);
						characterActionMessage.Read(iReader);
						Magicka.GameLogic.Entities.Character character = Entity.GetFromHandle((int)characterActionMessage.Handle) as Magicka.GameLogic.Entities.Character;
						if (character != null)
						{
							character.NetworkAction(ref characterActionMessage);
							goto IL_17EB;
						}
						goto IL_17EB;
					}
					case PacketType.AnimatedLevelPartUpdate:
					{
						AnimatedLevelPartUpdateMessage animatedLevelPartUpdateMessage = default(AnimatedLevelPartUpdateMessage);
						animatedLevelPartUpdateMessage.Read(iReader);
						AnimatedLevelPart fromHandle2 = AnimatedLevelPart.GetFromHandle((int)animatedLevelPartUpdateMessage.Handle);
						if (fromHandle2 != null)
						{
							fromHandle2.NetworkUpdate(ref animatedLevelPartUpdateMessage);
							goto IL_17EB;
						}
						goto IL_17EB;
					}
					case PacketType.BossUpdate:
					{
						BossUpdateMessage bossUpdateMessage = default(BossUpdateMessage);
						bossUpdateMessage.Read(iReader);
						BossFight.Instance.NetworkUpdate(ref bossUpdateMessage);
						goto IL_17EB;
					}
					case PacketType.BossInitialize:
					{
						BossInitializeMessage bossInitializeMessage = default(BossInitializeMessage);
						bossInitializeMessage.Read(iReader);
						BossFight.Instance.NetworkInitialize(ref bossInitializeMessage);
						goto IL_17EB;
					}
					case PacketType.SpawnPlayer:
					{
						SpawnPlayerMessage spawnPlayerMessage = default(SpawnPlayerMessage);
						spawnPlayerMessage.Read(iReader);
						Player player3 = Game.Instance.Players[(int)spawnPlayerMessage.Id];
						if (player3 == null)
						{
							goto IL_17EB;
						}
						lock (player3)
						{
							Avatar fromCache = Avatar.GetFromCache(player3, spawnPlayerMessage.Handle);
							if (fromCache != null)
							{
								player3.Weapon = null;
								player3.Staff = null;
								fromCache.Initialize(CharacterTemplate.GetCachedTemplate(player3.Gamer.Avatar.Type), spawnPlayerMessage.Position, Player.UNIQUE_ID[(int)spawnPlayerMessage.Id]);
								fromCache.SpawnAnimation = Animations.revive;
								fromCache.ChangeState(RessurectionState.Instance);
								fromCache.CharacterBody.DesiredDirection = spawnPlayerMessage.Direction;
								player3.Avatar = fromCache;
								if (spawnPlayerMessage.MagickRevive && player3.Controller is XInputController)
								{
									(player3.Controller as XInputController).Rumble(2f, 2f);
								}
								fromCache.PlayState.EntityManager.AddEntity(fromCache);
								AudioManager.Instance.PlayCue(Banks.Spells, Revive.SOUNDHASH, fromCache.AudioEmitter);
							}
							goto IL_17EB;
						}
						break;
					}
					case PacketType.SpawnMissile:
						break;
					case PacketType.SpawnShield:
					{
						SpawnShieldMessage spawnShieldMessage = default(SpawnShieldMessage);
						spawnShieldMessage.Read(iReader);
						Shield shield = Entity.GetFromHandle((int)spawnShieldMessage.Handle) as Shield;
						ISpellCaster iOwner = Entity.GetFromHandle((int)spawnShieldMessage.OwnerHandle) as ISpellCaster;
						shield.Initialize(iOwner, spawnShieldMessage.Position, spawnShieldMessage.Radius, spawnShieldMessage.Direction, spawnShieldMessage.ShieldType, spawnShieldMessage.HitPoints, Spell.SHIELDCOLOR);
						shield.PlayState.EntityManager.AddEntity(shield);
						goto IL_17EB;
					}
					case PacketType.SpawnBarrier:
					{
						SpawnBarrierMessage spawnBarrierMessage = default(SpawnBarrierMessage);
						spawnBarrierMessage.Read(iReader);
						Barrier barrier = Entity.GetFromHandle((int)spawnBarrierMessage.Handle) as Barrier;
						Barrier.HitListWithBarriers byHandle = Barrier.HitListWithBarriers.GetByHandle(spawnBarrierMessage.HitlistHandle);
						ISpellCaster iOwner2 = Entity.GetFromHandle((int)spawnBarrierMessage.OwnerHandle) as ISpellCaster;
						AnimatedLevelPart iAnimation = null;
						if (spawnBarrierMessage.AnimationHandle < 65535)
						{
							iAnimation = AnimatedLevelPart.GetFromHandle((int)spawnBarrierMessage.AnimationHandle);
						}
						barrier.Initialize(iOwner2, spawnBarrierMessage.Position, spawnBarrierMessage.Direction, spawnBarrierMessage.Scale, 0f, default(Vector3), Quaternion.Identity, 0f, ref spawnBarrierMessage.Spell, ref spawnBarrierMessage.Damage, byHandle, iAnimation);
						barrier.PlayState.EntityManager.AddEntity(barrier);
						goto IL_17EB;
					}
					case PacketType.SpawnWave:
					{
						SpawnWaveMessage spawnWaveMessage = default(SpawnWaveMessage);
						spawnWaveMessage.Read(iReader);
						WaveEntity waveEntity = Entity.GetFromHandle((int)spawnWaveMessage.Handle) as WaveEntity;
						Barrier.HitListWithBarriers byHandle2 = Barrier.HitListWithBarriers.GetByHandle(spawnWaveMessage.HitlistHandle);
						ISpellCaster iOwner3 = Entity.GetFromHandle((int)spawnWaveMessage.OwnerHandle) as ISpellCaster;
						AnimatedLevelPart iAnimation2 = null;
						if (spawnWaveMessage.AnimationHandle < 65535)
						{
							iAnimation2 = AnimatedLevelPart.GetFromHandle((int)spawnWaveMessage.AnimationHandle);
						}
						Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Wave instance = Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Wave.GetInstance();
						waveEntity.Initialize(iOwner3, spawnWaveMessage.Position, spawnWaveMessage.Direction, spawnWaveMessage.Scale, 0f, default(Vector3), Quaternion.Identity, 0f, ref spawnWaveMessage.Spell, ref spawnWaveMessage.Damage, ref byHandle2, iAnimation2, ref instance);
						waveEntity.PlayState.EntityManager.AddEntity(waveEntity);
						goto IL_17EB;
					}
					case PacketType.SpawnMine:
					{
						SpawnMineMessage spawnMineMessage = default(SpawnMineMessage);
						spawnMineMessage.Read(iReader);
						SpellMine spellMine = Entity.GetFromHandle((int)spawnMineMessage.Handle) as SpellMine;
						ISpellCaster iOwner4 = Entity.GetFromHandle((int)spawnMineMessage.OwnerHandle) as ISpellCaster;
						AnimatedLevelPart iAnimation3 = null;
						if (spawnMineMessage.AnimationHandle < 65535)
						{
							iAnimation3 = AnimatedLevelPart.GetFromHandle((int)spawnMineMessage.AnimationHandle);
						}
						spellMine.Initialize(iOwner4, spawnMineMessage.Position, spawnMineMessage.Direction, spawnMineMessage.Scale, 0f, default(Vector3), Quaternion.Identity, 0f, ref spawnMineMessage.Spell, ref spawnMineMessage.Damage, iAnimation3);
						spellMine.PlayState.EntityManager.AddEntity(spellMine);
						goto IL_17EB;
					}
					case PacketType.SpawnVortex:
					{
						SpawnVortexMessage spawnVortexMessage = default(SpawnVortexMessage);
						spawnVortexMessage.Read(iReader);
						VortexEntity specificInstance = VortexEntity.GetSpecificInstance(spawnVortexMessage.Handle);
						Entity fromHandle3 = Entity.GetFromHandle((int)spawnVortexMessage.OwnerHandle);
						specificInstance.Initialize(fromHandle3 as ISpellCaster, spawnVortexMessage.Position);
						specificInstance.PlayState.EntityManager.AddEntity(specificInstance);
						goto IL_17EB;
					}
					case PacketType.SpawnPortal:
					{
						SpawnPortalMessage spawnPortalMessage = default(SpawnPortalMessage);
						spawnPortalMessage.Read(iReader);
						Portal.Instance.SpawnPortal(ref spawnPortalMessage);
						goto IL_17EB;
					}
					case PacketType.EntityRemove:
					{
						EntityRemoveMessage entityRemoveMessage = default(EntityRemoveMessage);
						entityRemoveMessage.Read(iReader);
						Entity fromHandle4 = Entity.GetFromHandle((int)entityRemoveMessage.Handle);
						if (fromHandle4 is Magicka.GameLogic.Entities.Character)
						{
							(fromHandle4 as Magicka.GameLogic.Entities.Character).Terminate(false, true);
							goto IL_17EB;
						}
						if (!fromHandle4.Dead)
						{
							fromHandle4.Kill();
							goto IL_17EB;
						}
						goto IL_17EB;
					}
					case PacketType.CharacterDie:
					{
						CharacterDieMessage characterDieMessage = default(CharacterDieMessage);
						characterDieMessage.Read(iReader);
						Magicka.GameLogic.Entities.Character character2 = Entity.GetFromHandle((int)characterDieMessage.Handle) as Magicka.GameLogic.Entities.Character;
						if (!character2.NotedKilledEvent)
						{
							StatisticsManager.Instance.AddKillEvent(character2.PlayState, character2, character2.LastAttacker);
							character2.NotedKilledEvent = true;
						}
						if (!characterDieMessage.Overkill)
						{
							character2.Die();
							goto IL_17EB;
						}
						if (!character2.mDead)
						{
							character2.Die();
						}
						character2.RemoveAfterDeath = true;
						if (character2.HasGibs())
						{
							character2.SpawnGibs();
							AudioManager.Instance.PlayCue(Banks.Misc, DeadState.SOUND_GIB, character2.AudioEmitter);
							goto IL_17EB;
						}
						if (character2.BloatKilled)
						{
							character2.Terminate(false, true);
							goto IL_17EB;
						}
						goto IL_17EB;
					}
					case PacketType.MissileEntity:
					{
						MissileEntityEventMessage missileEntityEventMessage = default(MissileEntityEventMessage);
						missileEntityEventMessage.Read(iReader);
						MissileEntity missileEntity = Entity.GetFromHandle((int)missileEntityEventMessage.Handle) as MissileEntity;
						missileEntity.NetworkEventMessage(ref missileEntityEventMessage);
						goto IL_17EB;
					}
					case PacketType.Threat:
					{
						ThreatMessage threatMessage = default(ThreatMessage);
						threatMessage.Read(iReader);
						AudioManager.Instance.Threat = threatMessage.Threat;
						goto IL_17EB;
					}
					case PacketType.EnterSync:
					{
						EnterSyncMessage enterSyncMessage = default(EnterSyncMessage);
						enterSyncMessage.Read(iReader);
						lock (this.mSyncPoints)
						{
							this.mSyncPoints.Enqueue(enterSyncMessage.ID);
							goto IL_17EB;
						}
						goto IL_1791;
					}
					case PacketType.LeaveSync:
						goto IL_1791;
					case PacketType.Checkpoint:
					{
						int num = iReader.ReadInt32();
						byte[] buffer = new byte[num];
						iReader.Read(buffer, 0, num);
						MemoryStream checkpointStream = new MemoryStream(buffer);
						PlayState.RecentPlayState.CheckpointStream = checkpointStream;
						goto IL_17EB;
					}
					case PacketType.ForceSyncPlayersMessage:
					{
						ForceSyncPlayerStatusesMessage forceSyncPlayerStatusesMessage = default(ForceSyncPlayerStatusesMessage);
						forceSyncPlayerStatusesMessage.Read(iReader);
						for (int i = 0; i < (int)forceSyncPlayerStatusesMessage.numPlayers; i++)
						{
							EntityUpdateMessage entityUpdateMessage2 = forceSyncPlayerStatusesMessage.playerUpdateMessages[i];
							int handle = (int)entityUpdateMessage2.Handle;
							Player player4 = null;
							foreach (Player player4 in Game.Instance.Players)
							{
								if (player4 != null && player4.ID == handle)
								{
									break;
								}
							}
							if (player4 != null)
							{
								player4.Avatar.ForcedNetworkUpdate(iSender, ref entityUpdateMessage2);
							}
						}
						goto IL_17EB;
					}
					default:
						goto IL_17CB;
					}
					SpawnMissileMessage spawnMissileMessage = default(SpawnMissileMessage);
					spawnMissileMessage.Read(iReader);
					MissileEntity missileEntity2 = Entity.GetFromHandle((int)spawnMissileMessage.Handle) as MissileEntity;
					if (missileEntity2 == null)
					{
						throw new Exception("Caches out of sync!");
					}
					switch (spawnMissileMessage.Type)
					{
					case SpawnMissileMessage.MissileType.Spell:
						ProjectileSpell.SpawnMissile(ref missileEntity2, Entity.GetFromHandle((int)spawnMissileMessage.Owner) as ISpellCaster, spawnMissileMessage.Homing, ref spawnMissileMessage.Position, ref spawnMissileMessage.Velocity, ref spawnMissileMessage.Spell, spawnMissileMessage.Splash, (int)spawnMissileMessage.Item);
						goto IL_17EB;
					case SpawnMissileMessage.MissileType.Item:
					{
						Item item2 = Entity.GetFromHandle((int)spawnMissileMessage.Item) as Item;
						Entity fromHandle5 = Entity.GetFromHandle((int)spawnMissileMessage.Owner);
						if (item2.ProjectileModel != null)
						{
							missileEntity2.Initialize(fromHandle5, item2.ProjectileModel.Meshes[0].BoundingSphere.Radius, ref spawnMissileMessage.Position, ref spawnMissileMessage.Velocity, item2.ProjectileModel, item2.RangedConditions, false);
						}
						else
						{
							missileEntity2.Initialize(fromHandle5, 0.75f, ref spawnMissileMessage.Position, ref spawnMissileMessage.Velocity, null, item2.RangedConditions, false);
						}
						missileEntity2.Danger = item2.Danger;
						missileEntity2.Homing = item2.Homing;
						missileEntity2.FacingVelocity = item2.Facing;
						missileEntity2.PlayState.EntityManager.AddEntity(missileEntity2);
						goto IL_17EB;
					}
					case SpawnMissileMessage.MissileType.HolyGrenade:
						HolyGrenade.SpawnGrenade(ref missileEntity2, Entity.GetFromHandle((int)spawnMissileMessage.Owner) as ISpellCaster, ref spawnMissileMessage.Position, ref spawnMissileMessage.Velocity);
						missileEntity2.FacingVelocity = false;
						goto IL_17EB;
					case SpawnMissileMessage.MissileType.Grenade:
						Grenade.SpawnGrenade(ref missileEntity2, Entity.GetFromHandle((int)spawnMissileMessage.Owner) as ISpellCaster, ref spawnMissileMessage.Position, ref spawnMissileMessage.Velocity);
						missileEntity2.FacingVelocity = false;
						goto IL_17EB;
					case SpawnMissileMessage.MissileType.FireFlask:
						Fireflask.SpawnGrenade(ref missileEntity2, Entity.GetFromHandle((int)spawnMissileMessage.Owner) as ISpellCaster, ref spawnMissileMessage.Position, ref spawnMissileMessage.Velocity);
						missileEntity2.FacingVelocity = false;
						goto IL_17EB;
					case SpawnMissileMessage.MissileType.ProppMagick:
						ProppMagick.Spawn(ref missileEntity2, Entity.GetFromHandle((int)spawnMissileMessage.Owner) as ISpellCaster, ref spawnMissileMessage.Position, ref spawnMissileMessage.Velocity, ref spawnMissileMessage.AngularVelocity, ref spawnMissileMessage.Lever);
						goto IL_17EB;
					case SpawnMissileMessage.MissileType.JudgementMissile:
					{
						Entity fromHandle6 = Entity.GetFromHandle((int)spawnMissileMessage.Target);
						JudgementSpray.SpawnProjectile(ref missileEntity2, Entity.GetFromHandle((int)spawnMissileMessage.Owner) as ISpellCaster, ref fromHandle6, ref spawnMissileMessage.Position, ref spawnMissileMessage.Velocity);
						missileEntity2.FacingVelocity = false;
						goto IL_17EB;
					}
					case SpawnMissileMessage.MissileType.GreaseLump:
						Entity.GetFromHandle((int)spawnMissileMessage.Target);
						GreaseLump.SpawnLump(ref missileEntity2, Entity.GetFromHandle((int)spawnMissileMessage.Owner) as ISpellCaster, ref spawnMissileMessage.Position, ref spawnMissileMessage.Velocity);
						missileEntity2.FacingVelocity = false;
						goto IL_17EB;
					case SpawnMissileMessage.MissileType.PotionFlask:
						Potion.SpawnFlask(ref missileEntity2, Entity.GetFromHandle((int)spawnMissileMessage.Owner) as ISpellCaster, ref spawnMissileMessage.Position, ref spawnMissileMessage.Velocity);
						missileEntity2.FacingVelocity = false;
						goto IL_17EB;
					}
					throw new Exception();
					IL_1791:
					LeaveSyncMessage leaveSyncMessage = default(LeaveSyncMessage);
					leaveSyncMessage.Read(iReader);
					lock (this.mFinishedSyncPoints)
					{
						this.mFinishedSyncPoints.Add(leaveSyncMessage.ID);
						goto IL_17EB;
					}
					IL_17CB:
					throw new Exception("Unhandled message type (" + packetType.ToString() + ")!");
				}
				packetType &= (PacketType)127;
				PacketType packetType2 = packetType;
				if (packetType2 != PacketType.Ping)
				{
					if (packetType2 != PacketType.GamerJoin)
					{
						if (packetType2 != PacketType.Damage)
						{
							throw new Exception("Unhandled request (" + packetType.ToString() + ")!");
						}
						DamageRequestMessage damageRequestMessage = default(DamageRequestMessage);
						damageRequestMessage.Read(iReader);
						Entity fromHandle7 = Entity.GetFromHandle((int)damageRequestMessage.AttackerHandle);
						IDamageable damageable = Entity.GetFromHandle((int)damageRequestMessage.TargetHandle) as IDamageable;
						if (damageable != null)
						{
							Vector3 relativeAttackPosition = damageRequestMessage.RelativeAttackPosition;
							Vector3 position = damageable.Position;
							Vector3.Add(ref position, ref relativeAttackPosition, out relativeAttackPosition);
							Defines.DamageFeatures iFeatures = Defines.DamageFeatures.NK;
							damageable.InternalDamage(damageRequestMessage.Damage, fromHandle7, damageRequestMessage.TimeStamp, relativeAttackPosition, iFeatures);
						}
					}
					else
					{
						GamerJoinRequestMessage gamerJoinRequestMessage = default(GamerJoinRequestMessage);
						gamerJoinRequestMessage.Read(iReader);
						if (iSender != this.mServerID)
						{
							if (Debugger.IsAttached)
							{
								Debugger.Break();
							}
						}
						else
						{
							Player player5 = Game.Instance.Players[(int)gamerJoinRequestMessage.Id];
							if (player5.Controller != null)
							{
								player5.Controller.Player = null;
								player5.Controller = null;
							}
							player5.Gamer = new NetworkGamer(gamerJoinRequestMessage.Gamer, gamerJoinRequestMessage.Color, gamerJoinRequestMessage.AvatarThumb, gamerJoinRequestMessage.AvatarType, gamerJoinRequestMessage.SteamID);
							player5.Playing = true;
							SubMenuCharacterSelect.Instance.UpdateGamer(player5, player5.Gamer);
						}
					}
				}
				else
				{
					PingRequestMessage pingRequestMessage = default(PingRequestMessage);
					pingRequestMessage.Read(iReader);
					PingReplyMessage pingReplyMessage2 = default(PingReplyMessage);
					pingReplyMessage2.Payload = pingRequestMessage.Payload;
					this.SendUdpMessage<PingReplyMessage>(ref pingReplyMessage2);
				}
				IL_17EB:;
			}
			catch (NullReferenceException innerException)
			{
				string text2 = SteamFriends.GetFriendPersonaName(iSender);
				if (string.IsNullOrEmpty(text2))
				{
					text2 = "( server ? )";
				}
				throw new Exception(string.Format("System.NullReferenceException when reading message of type: {0} from {1} !", packetType.ToString(), text2), innerException);
			}
		}

		// Token: 0x06001AF8 RID: 6904 RVA: 0x000B8470 File Offset: 0x000B6670
		public override void SendMessage<T>(ref T iMessage, P2PSend sendType)
		{
			lock (this.mTxBuffer)
			{
				this.mTxBuffer[0] = (byte)iMessage.PacketType;
				this.mWriter.BaseStream.Position = 1L;
				iMessage.Write(this.mWriter);
				for (int i = 0; i < this.mConnections.Count; i++)
				{
					if (!SteamNetworking.SendP2PPacket(this.mConnections[i].ID, this.mTxBuffer, (uint)this.mWriter.BaseStream.Position, sendType))
					{
					}
					long ticks = DateTime.Now.Ticks;
					int num;
					if (!this.mBytesSent.TryGetValue(ticks, out num))
					{
						num = 0;
					}
					this.mBytesSent[ticks] = num + (int)this.mWriter.BaseStream.Position;
				}
			}
		}

		// Token: 0x06001AF9 RID: 6905 RVA: 0x000B8568 File Offset: 0x000B6768
		public override void SendMessage<T>(ref T iMessage, int iPeer, P2PSend sendType)
		{
			this.SendMessage<T>(ref iMessage, this.mConnections[iPeer].ID, sendType);
		}

		// Token: 0x06001AFA RID: 6906 RVA: 0x000B8584 File Offset: 0x000B6784
		public override void SendMessage<T>(ref T iMessage, SteamID iPeer, P2PSend sendType)
		{
			lock (this.mTxBuffer)
			{
				this.mTxBuffer[0] = (byte)iMessage.PacketType;
				this.mWriter.BaseStream.Position = 1L;
				iMessage.Write(this.mWriter);
				if (!SteamNetworking.SendP2PPacket(iPeer, this.mTxBuffer, (uint)this.mWriter.BaseStream.Position, sendType))
				{
					iPeer == this.mServerID;
				}
				long ticks = DateTime.Now.Ticks;
				int num;
				if (!this.mBytesSent.TryGetValue(ticks, out num))
				{
					num = 0;
				}
				this.mBytesSent[ticks] = num + (int)this.mWriter.BaseStream.Position;
			}
		}

		// Token: 0x06001AFB RID: 6907 RVA: 0x000B865C File Offset: 0x000B685C
		public override void SendUdpMessage<T>(ref T iMessage)
		{
			lock (this.mTxBuffer)
			{
				for (int i = 0; i < this.mConnections.Count; i++)
				{
					this.QueueUDPMessage<T>(i, ref iMessage);
				}
			}
		}

		// Token: 0x06001AFC RID: 6908 RVA: 0x000B86B0 File Offset: 0x000B68B0
		public override void SendUdpMessage<T>(ref T iMessage, int iPeer)
		{
			this.QueueUDPMessage<T>(iPeer, ref iMessage);
		}

		// Token: 0x06001AFD RID: 6909 RVA: 0x000B86BC File Offset: 0x000B68BC
		public override void SendUdpMessage<T>(ref T iMessage, SteamID iPeer)
		{
			int connection = this.GetConnection(iPeer);
			if (connection >= 0)
			{
				this.QueueUDPMessage<T>(connection, ref iMessage);
				return;
			}
			this.mTxBuffer[0] = (byte)iMessage.PacketType;
			this.mWriter.BaseStream.Position = 1L;
			iMessage.Write(this.mWriter);
			if (!SteamNetworking.SendP2PPacket(iPeer, this.mTxBuffer, (uint)this.mWriter.BaseStream.Position, P2PSend.UnreliableNoDelay))
			{
				iPeer == this.mServerID;
			}
			long ticks = DateTime.Now.Ticks;
			int num;
			if (!this.mBytesSent.TryGetValue(ticks, out num))
			{
				num = 0;
			}
			this.mBytesSent[ticks] = num + (int)this.mWriter.BaseStream.Position;
		}

		// Token: 0x06001AFE RID: 6910 RVA: 0x000B8782 File Offset: 0x000B6982
		public unsafe override void SendRaw(PacketType iType, void* iPtr, int iLength)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001AFF RID: 6911 RVA: 0x000B8789 File Offset: 0x000B6989
		public unsafe override void SendRaw(PacketType iType, void* iPtr, int iLength, int iPeer)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001B00 RID: 6912 RVA: 0x000B8790 File Offset: 0x000B6990
		public unsafe override void SendRaw(PacketType iType, void* iPtr, int iLength, SteamID iPeer)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001B01 RID: 6913 RVA: 0x000B8798 File Offset: 0x000B6998
		public override void Dispose()
		{
			Player[] players = Game.Instance.Players;
			for (int i = 0; i < players.Length; i++)
			{
				if (players[i].Playing && !(players[i].Gamer is NetworkGamer))
				{
					players[i].Leave();
				}
			}
			for (int j = 0; j < this.mConnections.Count; j++)
			{
				SteamNetworking.CloseP2PSessionWithUser(this.mConnections[j].ID);
			}
			uint num = 0U;
			ushort num2 = 0;
			SteamID steamID = default(SteamID);
			if (SteamMatchmaking.GetLobbyGameServer(this.mSteamIDLobby, ref num, ref num2, ref steamID) && num != 0U && num2 != 0)
			{
				SteamUser.TerminateGameConnection(num, num2);
			}
			SteamMatchmaking.LeaveLobby(this.mSteamIDLobby);
			SteamAPI.P2PSessionRequest -= this.SteamAPI_P2PSessionRequest;
			SteamAPI.P2PSessionConnectFail -= this.SteamAPI_P2PSessionConnectFail;
			LanguageManager.Instance.LanguageChanged -= new Action(this.mErrorMessageBox.LanguageChanged);
			if (GameStateManager.Instance.CurrentState is PlayState)
			{
				RenderManager.Instance.TransitionEnd += this.TransitionFinish;
				RenderManager.Instance.BeginTransition(Transitions.Fade, Color.Black, 0.5f);
			}
			Game.Instance.AddLoadTask(new Action(PackMan.Instance.UpdatePackLicense));
		}

		// Token: 0x06001B02 RID: 6914 RVA: 0x000B88DC File Offset: 0x000B6ADC
		private void TransitionFinish(TransitionEffect iOldEffect)
		{
			RenderManager.Instance.TransitionEnd -= this.TransitionFinish;
			while (!(Tome.Instance.CurrentMenu is SubMenuOnline) && !(Tome.Instance.CurrentMenu is SubMenuMain))
			{
				Tome.Instance.PopMenuInstant();
			}
			if (GameStateManager.Instance.CurrentState is PlayState)
			{
				GameStateManager.Instance.PopState();
			}
			RenderManager.Instance.EndTransition(Transitions.Fade, Color.Black, 0.5f);
		}

		// Token: 0x06001B03 RID: 6915 RVA: 0x000B895D File Offset: 0x000B6B5D
		public override string ToString()
		{
			return SteamFriends.GetFriendPersonaName(this.mServerID);
		}

		// Token: 0x170006A0 RID: 1696
		// (get) Token: 0x06001B04 RID: 6916 RVA: 0x000B896A File Offset: 0x000B6B6A
		public SaveSlotInfo SaveSlot
		{
			get
			{
				return this.mSaveSlot;
			}
		}

		// Token: 0x06001B05 RID: 6917 RVA: 0x000B8972 File Offset: 0x000B6B72
		public void ClearSaveSlot()
		{
			this.mSaveSlot = default(SaveSlotInfo);
		}

		// Token: 0x170006A1 RID: 1697
		// (get) Token: 0x06001B06 RID: 6918 RVA: 0x000B8980 File Offset: 0x000B6B80
		public override int Connections
		{
			get
			{
				return this.mConnections.Count;
			}
		}

		// Token: 0x06001B07 RID: 6919 RVA: 0x000B898D File Offset: 0x000B6B8D
		public override float GetLatency(int iConnection)
		{
			return this.mConnections[iConnection].Latency;
		}

		// Token: 0x06001B08 RID: 6920 RVA: 0x000B89A0 File Offset: 0x000B6BA0
		public override int GetLatencyMS(int iConnection)
		{
			return (int)(this.mConnections[iConnection].Latency * 1000f);
		}

		// Token: 0x06001B09 RID: 6921 RVA: 0x000B89BC File Offset: 0x000B6BBC
		public override float GetLatency(SteamID iConnection)
		{
			for (int i = 0; i < this.mConnections.Count; i++)
			{
				NetworkClient.Connection connection = this.mConnections[i];
				if (connection.ID == iConnection)
				{
					return connection.Latency;
				}
			}
			return 0f;
		}

		// Token: 0x06001B0A RID: 6922 RVA: 0x000B8A08 File Offset: 0x000B6C08
		public override int GetLatencyMS(SteamID iConnection)
		{
			for (int i = 0; i < this.mConnections.Count; i++)
			{
				NetworkClient.Connection connection = this.mConnections[i];
				if (connection.ID == iConnection)
				{
					return (int)(connection.Latency * 1000f);
				}
			}
			return 0;
		}

		// Token: 0x06001B0B RID: 6923 RVA: 0x000B8A58 File Offset: 0x000B6C58
		private void CloseConnection(SteamID iID, ConnectionClosedMessage.CReason iReason)
		{
			switch (iReason)
			{
			case ConnectionClosedMessage.CReason.Kicked:
			{
				string iMessage = string.Format("{0} {1}", SteamFriends.GetFriendPersonaName(iID), LanguageManager.Instance.GetString("#add_menu_not_kicked".GetHashCodeCustom()));
				NetworkChat.Instance.AddMessage(iMessage);
				goto IL_B2;
			}
			case ConnectionClosedMessage.CReason.Left:
			{
				string iMessage2 = string.Format("{0} {1}", SteamFriends.GetFriendPersonaName(iID), LanguageManager.Instance.GetString("#add_menu_not_left".GetHashCodeCustom()));
				NetworkChat.Instance.AddMessage(iMessage2);
				goto IL_B2;
			}
			}
			string text = LanguageManager.Instance.GetString("#add_connectiontolost".GetHashCodeCustom());
			text = text.Replace("#1;", SteamFriends.GetFriendPersonaName(iID));
			NetworkChat.Instance.AddMessage(text);
			IL_B2:
			lock (this.mConnections)
			{
				for (int i = 0; i < this.mConnections.Count; i++)
				{
					if (this.mConnections[i].ID == iID)
					{
						this.mConnections.RemoveAt(i--);
						break;
					}
				}
			}
			SteamNetworking.CloseP2PSessionWithUser(iID);
		}

		// Token: 0x06001B0C RID: 6924 RVA: 0x000B8B88 File Offset: 0x000B6D88
		public override void CloseConnection()
		{
			SteamMatchmaking.LeaveLobby(this.mSteamIDLobby);
			ConnectionClosedMessage connectionClosedMessage = default(ConnectionClosedMessage);
			connectionClosedMessage.Reason = ConnectionClosedMessage.CReason.Left;
			this.SendMessage<ConnectionClosedMessage>(ref connectionClosedMessage);
			Thread.Sleep(250);
		}

		// Token: 0x06001B0D RID: 6925 RVA: 0x000B8BC4 File Offset: 0x000B6DC4
		private void QueueUDPMessage<T>(int iClient, ref T iMessage) where T : ISendable
		{
			lock (this.mConnections)
			{
				NetworkClient.Connection connection = this.mConnections[iClient];
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
		}

		// Token: 0x06001B0E RID: 6926 RVA: 0x000B8C90 File Offset: 0x000B6E90
		private void FlushUDPBatch(int iClient, uint iLength)
		{
			lock (this.mConnections)
			{
				NetworkClient.Connection connection = this.mConnections[iClient];
				SteamNetworking.SendP2PPacket(connection.ID, connection.TxBuffer, iLength, P2PSend.UnreliableNoDelay);
				long ticks = DateTime.Now.Ticks;
				int num;
				if (!this.mBytesSent.TryGetValue(ticks, out num))
				{
					num = 0;
				}
				this.mBytesSent[ticks] = num + (int)iLength;
				connection.Writer.BaseStream.Position = 0L;
			}
		}

		// Token: 0x04001D01 RID: 7425
		private byte[] mRxBuffer;

		// Token: 0x04001D02 RID: 7426
		private byte[] mTxBuffer;

		// Token: 0x04001D03 RID: 7427
		public BinaryReader mReader;

		// Token: 0x04001D04 RID: 7428
		public BinaryWriter mWriter;

		// Token: 0x04001D05 RID: 7429
		private Queue<uint> mSyncPoints = new Queue<uint>();

		// Token: 0x04001D06 RID: 7430
		private List<uint> mFinishedSyncPoints = new List<uint>();

		// Token: 0x04001D07 RID: 7431
		private SteamID mServerID;

		// Token: 0x04001D08 RID: 7432
		private int mRetryCount;

		// Token: 0x04001D09 RID: 7433
		private List<NetworkClient.Connection> mConnections;

		// Token: 0x04001D0A RID: 7434
		private OptionsMessageBox mErrorMessageBox;

		// Token: 0x04001D0B RID: 7435
		private long mLastLatencyCheck;

		// Token: 0x04001D0C RID: 7436
		private SteamID mSteamIDLobby;

		// Token: 0x04001D0D RID: 7437
		private Action<ConnectionStatus> mOnComplete;

		// Token: 0x04001D0E RID: 7438
		private byte[] mDummyData = new byte[1];

		// Token: 0x04001D0F RID: 7439
		private bool mVAC;

		// Token: 0x04001D10 RID: 7440
		private SaveSlotInfo mSaveSlot;

		// Token: 0x04001D11 RID: 7441
		private SortedList<long, int> mBytesSent = new SortedList<long, int>();

		// Token: 0x04001D12 RID: 7442
		private int mBytesPerSecond;

		// Token: 0x02000375 RID: 885
		private struct Connection
		{
			// Token: 0x04001D13 RID: 7443
			public SteamID ID;

			// Token: 0x04001D14 RID: 7444
			public byte[] TxBuffer;

			// Token: 0x04001D15 RID: 7445
			public BinaryWriter Writer;

			// Token: 0x04001D16 RID: 7446
			public Stopwatch Timer;

			// Token: 0x04001D17 RID: 7447
			public float Latency;
		}
	}
}
