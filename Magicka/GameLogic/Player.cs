using System;
using System.Collections.Generic;
using Magicka.DRM;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.GameStates.Menu.Main;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.UI;
using Magicka.Gamers;
using Magicka.Graphics;
using Magicka.Network;
using PolygonHead;
using SteamWrapper;

namespace Magicka.GameLogic
{
	// Token: 0x020002ED RID: 749
	public class Player
	{
		// Token: 0x170005CB RID: 1483
		// (get) Token: 0x060016ED RID: 5869 RVA: 0x00093FD6 File Offset: 0x000921D6
		public string GamerTag
		{
			get
			{
				return this.mGamer.GamerTag;
			}
		}

		// Token: 0x170005CC RID: 1484
		// (get) Token: 0x060016EE RID: 5870 RVA: 0x00093FE3 File Offset: 0x000921E3
		// (set) Token: 0x060016EF RID: 5871 RVA: 0x00093FEB File Offset: 0x000921EB
		public string Weapon { get; set; }

		// Token: 0x170005CD RID: 1485
		// (get) Token: 0x060016F0 RID: 5872 RVA: 0x00093FF4 File Offset: 0x000921F4
		// (set) Token: 0x060016F1 RID: 5873 RVA: 0x00093FFC File Offset: 0x000921FC
		public string Staff { get; set; }

		// Token: 0x170005CE RID: 1486
		// (get) Token: 0x060016F2 RID: 5874 RVA: 0x00094005 File Offset: 0x00092205
		// (set) Token: 0x060016F3 RID: 5875 RVA: 0x0009400D File Offset: 0x0009220D
		public bool Ressing
		{
			get
			{
				return this.mRessing;
			}
			set
			{
				this.mRessing = value;
			}
		}

		// Token: 0x060016F4 RID: 5876 RVA: 0x00094016 File Offset: 0x00092216
		public Player(int iID)
		{
			this.mID = iID;
			this.mInputQueue = new StaticEquatableList<int>(5);
			this.mAvatar = new WeakReference(null);
		}

		// Token: 0x060016F5 RID: 5877 RVA: 0x00094040 File Offset: 0x00092240
		public void InitializeGame(PlayState iPlayState)
		{
			this.mDeadTimer = 0f;
			if (this.mSpellWheel == null)
			{
				this.mSpellWheel = new SpellWheel(this, iPlayState);
			}
			else
			{
				this.mSpellWheel.Initialize(iPlayState);
			}
			if (this.mNotifierButton == null)
			{
				this.mNotifierButton = new NotifierButton();
			}
			if (this.mIconRenderer == null)
			{
				this.mIconRenderer = new IconRenderer(this, iPlayState);
			}
			else
			{
				this.mIconRenderer.Initialize(iPlayState);
			}
			if (this.mObtainedTextBox == null)
			{
				this.mObtainedTextBox = new TextBox();
			}
		}

		// Token: 0x060016F6 RID: 5878 RVA: 0x000940C4 File Offset: 0x000922C4
		public void DeinitializeGame()
		{
		}

		// Token: 0x170005CF RID: 1487
		// (get) Token: 0x060016F7 RID: 5879 RVA: 0x000940C6 File Offset: 0x000922C6
		// (set) Token: 0x060016F8 RID: 5880 RVA: 0x000940CE File Offset: 0x000922CE
		public bool Playing { get; set; }

		// Token: 0x170005D0 RID: 1488
		// (get) Token: 0x060016F9 RID: 5881 RVA: 0x000940D7 File Offset: 0x000922D7
		// (set) Token: 0x060016FA RID: 5882 RVA: 0x0009410C File Offset: 0x0009230C
		public byte Color
		{
			get
			{
				if (this.Team == Factions.TEAM_RED)
				{
					return 0;
				}
				if (this.Team == Factions.TEAM_BLUE)
				{
					return 3;
				}
				if (this.mGamer == null)
				{
					return 0;
				}
				return this.mGamer.Color;
			}
			set
			{
				this.mGamer.Color = value;
				if (!(this.mGamer is NetworkGamer) && NetworkManager.Instance.State != NetworkState.Offline)
				{
					GamerChangedMessage gamerChangedMessage = new GamerChangedMessage(this);
					NetworkManager.Instance.Interface.SendMessage<GamerChangedMessage>(ref gamerChangedMessage);
				}
			}
		}

		// Token: 0x170005D1 RID: 1489
		// (get) Token: 0x060016FB RID: 5883 RVA: 0x0009415C File Offset: 0x0009235C
		public NotifierButton NotifierButton
		{
			get
			{
				return this.mNotifierButton;
			}
		}

		// Token: 0x170005D2 RID: 1490
		// (get) Token: 0x060016FC RID: 5884 RVA: 0x00094164 File Offset: 0x00092364
		public TextBox ObtainedTextBox
		{
			get
			{
				return this.mObtainedTextBox;
			}
		}

		// Token: 0x170005D3 RID: 1491
		// (get) Token: 0x060016FD RID: 5885 RVA: 0x0009416C File Offset: 0x0009236C
		// (set) Token: 0x060016FE RID: 5886 RVA: 0x00094174 File Offset: 0x00092374
		public Gamer Gamer
		{
			get
			{
				return this.mGamer;
			}
			set
			{
				this.mGamer = value;
			}
		}

		// Token: 0x170005D4 RID: 1492
		// (get) Token: 0x060016FF RID: 5887 RVA: 0x0009417D File Offset: 0x0009237D
		// (set) Token: 0x06001700 RID: 5888 RVA: 0x0009418F File Offset: 0x0009238F
		public Avatar Avatar
		{
			get
			{
				return this.mAvatar.Target as Avatar;
			}
			set
			{
				this.mAvatar.Target = value;
			}
		}

		// Token: 0x170005D5 RID: 1493
		// (get) Token: 0x06001701 RID: 5889 RVA: 0x0009419D File Offset: 0x0009239D
		public SpellWheel SpellWheel
		{
			get
			{
				return this.mSpellWheel;
			}
		}

		// Token: 0x170005D6 RID: 1494
		// (get) Token: 0x06001702 RID: 5890 RVA: 0x000941A5 File Offset: 0x000923A5
		public IconRenderer IconRenderer
		{
			get
			{
				return this.mIconRenderer;
			}
		}

		// Token: 0x170005D7 RID: 1495
		// (get) Token: 0x06001703 RID: 5891 RVA: 0x000941AD File Offset: 0x000923AD
		// (set) Token: 0x06001704 RID: 5892 RVA: 0x000941B5 File Offset: 0x000923B5
		public Factions Team { get; set; }

		// Token: 0x170005D8 RID: 1496
		// (get) Token: 0x06001705 RID: 5893 RVA: 0x000941BE File Offset: 0x000923BE
		// (set) Token: 0x06001706 RID: 5894 RVA: 0x000941C6 File Offset: 0x000923C6
		public int ID
		{
			get
			{
				return this.mID;
			}
			set
			{
				this.mID = value;
			}
		}

		// Token: 0x170005D9 RID: 1497
		// (get) Token: 0x06001707 RID: 5895 RVA: 0x000941CF File Offset: 0x000923CF
		public float DeadAge
		{
			get
			{
				return this.mDeadTimer;
			}
		}

		// Token: 0x170005DA RID: 1498
		// (get) Token: 0x06001708 RID: 5896 RVA: 0x000941D7 File Offset: 0x000923D7
		// (set) Token: 0x06001709 RID: 5897 RVA: 0x000941DF File Offset: 0x000923DF
		internal Controller Controller { get; set; }

		// Token: 0x170005DB RID: 1499
		// (get) Token: 0x0600170A RID: 5898 RVA: 0x000941E8 File Offset: 0x000923E8
		public StaticList<int> InputQueue
		{
			get
			{
				return this.mInputQueue;
			}
		}

		// Token: 0x170005DC RID: 1500
		// (get) Token: 0x0600170B RID: 5899 RVA: 0x000941F0 File Offset: 0x000923F0
		public StaticList<Spell> SpellQueue
		{
			get
			{
				if (this.Avatar == null)
				{
					return null;
				}
				return this.Avatar.SpellQueue;
			}
		}

		// Token: 0x0600170C RID: 5900 RVA: 0x00094207 File Offset: 0x00092407
		public void ForceSetNotDead()
		{
			this.Avatar.ForceSetNotDead();
		}

		// Token: 0x0600170D RID: 5901 RVA: 0x00094214 File Offset: 0x00092414
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			if (this.mAvatar.Target != null)
			{
				if (this.Avatar.Dead)
				{
					this.mDeadTimer += iDeltaTime;
					return;
				}
				this.mDeadTimer = 0f;
				if (GlobalSettings.Instance.SpellWheel == SettingOptions.On && this.Avatar.PlayState != null && !this.Avatar.PlayState.IsInCutscene)
				{
					if (this.Controller is DirectInputController | this.Controller is XInputController)
					{
						this.mSpellWheel.Update(iDataChannel, iDeltaTime);
					}
					this.mIconRenderer.Update(iDataChannel, iDeltaTime);
				}
			}
		}

		// Token: 0x0600170E RID: 5902 RVA: 0x000942C0 File Offset: 0x000924C0
		public void Leave()
		{
			if (this.Playing)
			{
				this.Playing = false;
				if (this.Avatar != null && !this.Avatar.Dead)
				{
					this.Avatar.Kill();
				}
				this.Avatar = null;
				if (this.Controller != null)
				{
					this.Controller.Player = null;
				}
				this.Controller = null;
				this.Gamer = null;
				this.Team = Factions.NONE;
				SubMenuCharacterSelect.Instance.UpdateGamer(this, null);
				if (NetworkManager.Instance.State != NetworkState.Offline)
				{
					GamerLeaveMessage gamerLeaveMessage;
					gamerLeaveMessage.Id = (byte)this.mID;
					NetworkManager.Instance.Interface.SendMessage<GamerLeaveMessage>(ref gamerLeaveMessage);
				}
			}
		}

		// Token: 0x0600170F RID: 5903 RVA: 0x00094368 File Offset: 0x00092568
		internal static Player Join(Controller iController, int iIndex, Gamer iGamer)
		{
			if (NetworkManager.Instance.State == NetworkState.Client)
			{
				GamerJoinRequestMessage gamerJoinRequestMessage;
				gamerJoinRequestMessage.Gamer = iGamer.GamerTag;
				gamerJoinRequestMessage.AvatarThumb = iGamer.Avatar.ThumbPath;
				gamerJoinRequestMessage.AvatarPortrait = iGamer.Avatar.PortraitPath;
				gamerJoinRequestMessage.AvatarType = iGamer.Avatar.TypeName;
				gamerJoinRequestMessage.Color = iGamer.Color;
				gamerJoinRequestMessage.Id = (sbyte)iIndex;
				gamerJoinRequestMessage.SteamID = SteamUser.GetSteamID();
				NetworkManager.Instance.Interface.SendMessage<GamerJoinRequestMessage>(ref gamerJoinRequestMessage, 0);
				Player.sWaitingGamers.Add(new Player.QueuedGamer(iController, iGamer));
			}
			else
			{
				Player[] players = Game.Instance.Players;
				if (iIndex < 0)
				{
					if (iGamer != Gamer.INVALID_GAMER)
					{
						for (int i = 0; i < players.Length; i++)
						{
							if (players[i].Gamer == iGamer)
							{
								Player player = players[i];
								Controller controller = player.Controller;
								if (controller != null)
								{
									controller.Player.Controller = null;
									controller.Player = null;
								}
								if (!(iGamer is NetworkGamer) && NetworkManager.Instance.State != NetworkState.Offline && NetworkManager.Instance.Interface.IsVACSecure && HackHelper.CheckLicense(player.Gamer.Avatar) != HackHelper.License.Yes)
								{
									player.Gamer.Avatar = Profile.Instance.DefaultAvatar;
									if (NetworkManager.Instance.State != NetworkState.Offline)
									{
										GamerChangedMessage gamerChangedMessage = new GamerChangedMessage(player);
										NetworkManager.Instance.Interface.SendMessage<GamerChangedMessage>(ref gamerChangedMessage);
									}
								}
								player.Controller = iController;
								iController.Player = player;
								player.Playing = true;
								Player.ServerPlayerJoin(player);
								return player;
							}
						}
					}
					for (int j = 0; j < players.Length; j++)
					{
						if (!players[j].Playing)
						{
							Player player2 = players[j];
							Controller controller2 = player2.Controller;
							if (controller2 != null)
							{
								controller2.Player.Controller = null;
								controller2.Player = null;
							}
							player2.Gamer = iGamer;
							if (!(iGamer is NetworkGamer) && NetworkManager.Instance.State != NetworkState.Offline && NetworkManager.Instance.Interface.IsVACSecure && HackHelper.CheckLicense(player2.Gamer.Avatar) != HackHelper.License.Yes)
							{
								player2.Gamer.Avatar = Profile.Instance.DefaultAvatar;
								if (NetworkManager.Instance.State != NetworkState.Offline)
								{
									GamerChangedMessage gamerChangedMessage2 = new GamerChangedMessage(player2);
									NetworkManager.Instance.Interface.SendMessage<GamerChangedMessage>(ref gamerChangedMessage2);
								}
							}
							player2.Controller = iController;
							if (iController != null)
							{
								iController.Player = player2;
							}
							player2.Playing = true;
							Player.ServerPlayerJoin(player2);
							return player2;
						}
					}
				}
				else
				{
					if (iGamer != Gamer.INVALID_GAMER && players[iIndex].Gamer == iGamer)
					{
						Player player3 = players[iIndex];
						Controller controller3 = player3.Controller;
						if (controller3 != null)
						{
							controller3.Player.Controller = null;
							controller3.Player = null;
						}
						player3.Controller = iController;
						iController.Player = player3;
						if (!(iGamer is NetworkGamer) && NetworkManager.Instance.State != NetworkState.Offline && NetworkManager.Instance.Interface.IsVACSecure && HackHelper.CheckLicense(player3.Gamer.Avatar) != HackHelper.License.Yes)
						{
							player3.Gamer.Avatar = Profile.Instance.DefaultAvatar;
							if (NetworkManager.Instance.State != NetworkState.Offline)
							{
								GamerChangedMessage gamerChangedMessage3 = new GamerChangedMessage(player3);
								NetworkManager.Instance.Interface.SendMessage<GamerChangedMessage>(ref gamerChangedMessage3);
							}
						}
						player3.Playing = true;
						Player.ServerPlayerJoin(player3);
						return player3;
					}
					if (!players[iIndex].Playing)
					{
						Player player4 = players[iIndex];
						Controller controller4 = player4.Controller;
						if (controller4 != null)
						{
							controller4.Player.Controller = null;
							controller4.Player = null;
						}
						player4.Gamer = iGamer;
						if (!(iGamer is NetworkGamer) && NetworkManager.Instance.State != NetworkState.Offline && NetworkManager.Instance.Interface.IsVACSecure && HackHelper.CheckLicense(player4.Gamer.Avatar) != HackHelper.License.Yes)
						{
							player4.Gamer.Avatar = Profile.Instance.DefaultAvatar;
							if (NetworkManager.Instance.State != NetworkState.Offline)
							{
								GamerChangedMessage gamerChangedMessage4 = new GamerChangedMessage(player4);
								NetworkManager.Instance.Interface.SendMessage<GamerChangedMessage>(ref gamerChangedMessage4);
							}
						}
						player4.Controller = iController;
						if (iController != null)
						{
							iController.Player = player4;
						}
						player4.Playing = true;
						Player.ServerPlayerJoin(player4);
						return player4;
					}
				}
			}
			return null;
		}

		// Token: 0x06001710 RID: 5904 RVA: 0x000947BC File Offset: 0x000929BC
		internal static void JoinServerGranted(ref GamerJoinAcceptMessage iMessage)
		{
			Player player = Game.Instance.Players[(int)iMessage.Id];
			long ticks = DateTime.Now.Ticks;
			bool flag = false;
			for (int i = 0; i < Player.sWaitingGamers.Count; i++)
			{
				if (Player.sWaitingGamers[i].Gamer.GamerTag.Equals(iMessage.Gamer))
				{
					player.Gamer = Player.sWaitingGamers[i].Gamer;
					if (player.Controller != null)
					{
						player.Controller.Player = null;
					}
					player.Controller = Player.sWaitingGamers[i].Controller;
					player.Controller.Player = player;
					player.Playing = true;
					if (NetworkManager.Instance.State != NetworkState.Offline && NetworkManager.Instance.Interface.IsVACSecure && HackHelper.CheckLicense(player.Gamer.Avatar) != HackHelper.License.Yes)
					{
						player.Gamer.Avatar = Profile.Instance.DefaultAvatar;
						if (NetworkManager.Instance.State != NetworkState.Offline)
						{
							GamerChangedMessage gamerChangedMessage = new GamerChangedMessage(player);
							NetworkManager.Instance.Interface.SendMessage<GamerChangedMessage>(ref gamerChangedMessage);
						}
					}
					SubMenuCharacterSelect.Instance.UpdateGamer(player, player.Gamer);
					Player.sWaitingGamers.RemoveAt(i);
					flag = true;
					Player.sWaitingGamers.Clear();
					break;
				}
				if (ticks - Player.sWaitingGamers[i].RequestTime > 10000000L)
				{
					Player.sWaitingGamers.RemoveAt(i);
					i--;
				}
			}
			if (!flag)
			{
				GamerLeaveMessage gamerLeaveMessage;
				gamerLeaveMessage.Id = (byte)iMessage.Id;
				NetworkManager.Instance.Interface.SendMessage<GamerLeaveMessage>(ref gamerLeaveMessage);
			}
		}

		// Token: 0x06001711 RID: 5905 RVA: 0x00094964 File Offset: 0x00092B64
		private static void ServerPlayerJoin(Player iPlayer)
		{
			if (NetworkManager.Instance.State == NetworkState.Server)
			{
				GamerJoinRequestMessage gamerJoinRequestMessage;
				gamerJoinRequestMessage.Id = (sbyte)iPlayer.ID;
				gamerJoinRequestMessage.Gamer = iPlayer.mGamer.GamerTag;
				gamerJoinRequestMessage.AvatarThumb = iPlayer.mGamer.Avatar.ThumbPath;
				gamerJoinRequestMessage.AvatarPortrait = iPlayer.mGamer.Avatar.PortraitPath;
				gamerJoinRequestMessage.AvatarType = iPlayer.mGamer.Avatar.TypeName;
				gamerJoinRequestMessage.Color = iPlayer.mGamer.Color;
				if (iPlayer.mGamer is NetworkGamer)
				{
					gamerJoinRequestMessage.SteamID = (iPlayer.mGamer as NetworkGamer).ClientID;
				}
				else
				{
					gamerJoinRequestMessage.SteamID = SteamGameServer.GetSteamID();
				}
				NetworkManager.Instance.Interface.SendMessage<GamerJoinRequestMessage>(ref gamerJoinRequestMessage);
			}
		}

		// Token: 0x170005DD RID: 1501
		// (get) Token: 0x06001712 RID: 5906 RVA: 0x00094A3B File Offset: 0x00092C3B
		// (set) Token: 0x06001713 RID: 5907 RVA: 0x00094A44 File Offset: 0x00092C44
		public ulong UnlockedMagicks
		{
			get
			{
				return this.mUnlockedMagicks;
			}
			set
			{
				this.mUnlockedMagicks = value;
				if (this.mGamer != null && !(this.mGamer is NetworkGamer) && NetworkManager.Instance.State == NetworkState.Server)
				{
					GamerChangedMessage gamerChangedMessage = new GamerChangedMessage(this);
					NetworkManager.Instance.Interface.SendMessage<GamerChangedMessage>(ref gamerChangedMessage);
				}
			}
		}

		// Token: 0x04001888 RID: 6280
		private static List<Player.QueuedGamer> sWaitingGamers = new List<Player.QueuedGamer>(4);

		// Token: 0x04001889 RID: 6281
		public static readonly int[] UNIQUE_ID = new int[]
		{
			"player1".GetHashCodeCustom(),
			"player2".GetHashCodeCustom(),
			"player3".GetHashCodeCustom(),
			"player4".GetHashCodeCustom()
		};

		// Token: 0x0400188A RID: 6282
		public static readonly int ALUCART_UNIQUE_ID = "wizard_alucart".GetHashCodeCustom();

		// Token: 0x0400188B RID: 6283
		private Gamer mGamer;

		// Token: 0x0400188C RID: 6284
		private int mID;

		// Token: 0x0400188D RID: 6285
		private WeakReference mAvatar;

		// Token: 0x0400188E RID: 6286
		private StaticList<int> mInputQueue;

		// Token: 0x0400188F RID: 6287
		private SpellWheel mSpellWheel;

		// Token: 0x04001890 RID: 6288
		private IconRenderer mIconRenderer;

		// Token: 0x04001891 RID: 6289
		private TextBox mObtainedTextBox;

		// Token: 0x04001892 RID: 6290
		private float mDeadTimer;

		// Token: 0x04001893 RID: 6291
		protected ulong mUnlockedMagicks;

		// Token: 0x04001894 RID: 6292
		protected NotifierButton mNotifierButton;

		// Token: 0x04001895 RID: 6293
		private bool mRessing;

		// Token: 0x020002EE RID: 750
		private struct QueuedGamer
		{
			// Token: 0x06001715 RID: 5909 RVA: 0x00094B00 File Offset: 0x00092D00
			public QueuedGamer(Controller iController, Gamer iGamer)
			{
				this.Controller = iController;
				this.Gamer = iGamer;
				this.RequestTime = DateTime.Now.Ticks;
			}

			// Token: 0x0400189B RID: 6299
			public Controller Controller;

			// Token: 0x0400189C RID: 6300
			public Gamer Gamer;

			// Token: 0x0400189D RID: 6301
			public long RequestTime;
		}
	}
}
