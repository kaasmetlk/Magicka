using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml;
using JigLibX.Geometry;
using JigLibX.Math;
using Magicka.Achievements;
using Magicka.AI;
using Magicka.Audio;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.Abilities.SpecialAbilities;
using Magicka.GameLogic.Entities.Bosses;
using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.GameStates.InGameMenus;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.GameLogic.GameStates.Menu.Main;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.Spells.SpellEffects;
using Magicka.GameLogic.Statistics;
using Magicka.GameLogic.UI;
using Magicka.Gamers;
using Magicka.Graphics;
using Magicka.Graphics.Effects;
using Magicka.Graphics.Lights;
using Magicka.Levels;
using Magicka.Levels.Campaign;
using Magicka.Levels.Triggers;
using Magicka.Levels.Triggers.Actions;
using Magicka.Levels.Versus;
using Magicka.Localization;
using Magicka.Misc;
using Magicka.Network;
using Magicka.Physics;
using Magicka.Storage;
using Magicka.WebTools.Paradox.Telemetry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using PolygonHead.Lights;
using PolygonHead.ParticleEffects;
using SteamWrapper;

namespace Magicka.GameLogic.GameStates
{
	// Token: 0x0200040D RID: 1037
	public class PlayState : GameState, IDisposable
	{
		// Token: 0x170007C7 RID: 1991
		// (get) Token: 0x06001FED RID: 8173 RVA: 0x000DFE82 File Offset: 0x000DE082
		public static PlayState RecentPlayState
		{
			get
			{
				return PlayState.sRecentPlayState;
			}
		}

		// Token: 0x170007C8 RID: 1992
		// (get) Token: 0x06001FEE RID: 8174 RVA: 0x000DFE89 File Offset: 0x000DE089
		public bool HasEntered
		{
			get
			{
				return this.mHasEntered;
			}
		}

		// Token: 0x06001FEF RID: 8175 RVA: 0x000DFE94 File Offset: 0x000DE094
		internal PlayState(bool iCustom, string iLevelFileName, GameType iGameType, SpawnPoint? iSpawnPoint, SaveData iSaveSlot, VersusRuleset.Settings iSettings) : base(new MagickCamera(MagickCamera.CAMERAOFFSET, Vector3.Forward, Vector3.Up, MagickCamera.DEFAULTFOV, 1.7777778f, MagickCamera.NEARCLIP, MagickCamera.FARCLIP))
		{
			this.mGameType = iGameType;
			this.mSpawnPoint = iSpawnPoint;
			this.mSettings = iSettings;
			this.mSaveSlot = iSaveSlot;
			SubMenuEndGame.Instance.Set(this.mGameType == GameType.Mythos | this.mGameType == GameType.Campaign, iSaveSlot);
			this.mCamera = (this.mScene.Camera as MagickCamera);
			this.mCamera.SetPlayState(this);
			this.mLevelFileName = iLevelFileName;
			this.mEndGameText = new Text(32, FontManager.Instance.GetFont(MagickaFont.Stonecross50), TextAlign.Center, false);
			GUIBasicEffect iEffect = null;
			GraphicsDevice graphicsDevice = Game.Instance.GraphicsDevice;
			lock (graphicsDevice)
			{
				iEffect = new GUIBasicEffect(graphicsDevice, null);
			}
			this.mRenderData = new PlayState.RenderData[3];
			this.mCutsceneRenderData = new PlayState.CutsceneRenderData[3];
			for (int i = 0; i < 3; i++)
			{
				this.mRenderData[i] = new PlayState.RenderData(iEffect, this.mEndGameText);
				this.mCutsceneRenderData[i] = new PlayState.CutsceneRenderData(iEffect);
			}
			TutorialManager instance = TutorialManager.Instance;
			int num = MagickaMath.Random.Next(22) + 1;
			bool flag = false;
			if (num == 6)
			{
				flag = true;
			}
			if (num < 10)
			{
				num = ("#tip0" + num).GetHashCodeCustom();
			}
			else
			{
				num = ("#tip" + num).GetHashCodeCustom();
			}
			int hashCodeCustom = "#tip".GetHashCodeCustom();
			string text = LanguageManager.Instance.GetString(num);
			if (flag)
			{
				bool flag2 = false;
				Player[] players = Game.Instance.Players;
				for (int j = 0; j < players.Length; j++)
				{
					if (players[j].Playing && !(players[j].Gamer is NetworkGamer))
					{
						flag2 = !(players[j].Controller is KeyboardMouseController);
						break;
					}
				}
				if (flag2)
				{
					text = text.Replace("#KEY_BOOST;", '̥'.ToString());
				}
				else
				{
					text = text.Replace("#KEY_BOOST;", '̪'.ToString());
				}
			}
			this.mLoadingTip = LanguageManager.Instance.GetString(hashCodeCustom) + "\n" + text;
		}

		// Token: 0x170007C9 RID: 1993
		// (get) Token: 0x06001FF0 RID: 8176 RVA: 0x000E0144 File Offset: 0x000DE344
		public bool DiedInLevel
		{
			get
			{
				return this.mDiedInLevel;
			}
		}

		// Token: 0x06001FF1 RID: 8177 RVA: 0x000E014C File Offset: 0x000DE34C
		public void SetDiedInLevel()
		{
			this.mDiedInLevel = true;
		}

		// Token: 0x170007CA RID: 1994
		// (get) Token: 0x06001FF2 RID: 8178 RVA: 0x000E0155 File Offset: 0x000DE355
		public InventoryBox Inventory
		{
			get
			{
				return this.mInventoryBox;
			}
		}

		// Token: 0x06001FF3 RID: 8179 RVA: 0x000E015D File Offset: 0x000DE35D
		public void SetTip(string iTip, bool iShowProgress, Cue iCueToFinish)
		{
			this.mLoadingTip = iTip;
			this.mShowProgress = iShowProgress;
			this.mCueToFinish = iCueToFinish;
		}

		// Token: 0x170007CB RID: 1995
		// (get) Token: 0x06001FF4 RID: 8180 RVA: 0x000E0174 File Offset: 0x000DE374
		public GameType GameType
		{
			get
			{
				return this.mGameType;
			}
		}

		// Token: 0x170007CC RID: 1996
		// (get) Token: 0x06001FF5 RID: 8181 RVA: 0x000E017C File Offset: 0x000DE37C
		public SaveData SaveSlot
		{
			get
			{
				return this.mSaveSlot;
			}
		}

		// Token: 0x170007CD RID: 1997
		// (get) Token: 0x06001FF6 RID: 8182 RVA: 0x000E0184 File Offset: 0x000DE384
		// (set) Token: 0x06001FF7 RID: 8183 RVA: 0x000E018C File Offset: 0x000DE38C
		public MemoryStream CheckpointStream
		{
			get
			{
				return this.mCheckpointStream;
			}
			set
			{
				this.mCheckpointStream = value;
			}
		}

		// Token: 0x170007CE RID: 1998
		// (get) Token: 0x06001FF8 RID: 8184 RVA: 0x000E0195 File Offset: 0x000DE395
		public bool Busy
		{
			get
			{
				return this.mBusy;
			}
		}

		// Token: 0x06001FF9 RID: 8185 RVA: 0x000E01A0 File Offset: 0x000DE3A0
		public void UpdateCheckPoint(Matrix[] iSpawnPoints, List<int> iIgnoredTriggers, bool iSaveToDisk)
		{
			lock (this.mCheckpointState)
			{
				this.mCheckpointState.UpdateState(iSpawnPoints, iIgnoredTriggers);
				Profile.Instance.Write();
				if (iSaveToDisk && this.mGameType != GameType.Versus && NetworkManager.Instance.State != NetworkState.Client)
				{
					MemoryStream memoryStream = new MemoryStream();
					BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
					binaryWriter.Write(this.mLevelHash, 0, 32);
					binaryWriter.Write(this.mDialogHash, 0, 32);
					this.mCheckpointState.Write(binaryWriter);
					if (this.mSaveSlot == null)
					{
						this.mSaveSlot = new SaveData();
					}
					this.mSaveSlot.Checkpoint = memoryStream;
					if (this.mGameType == GameType.Campaign || this.mGameType == GameType.Mythos)
					{
						SaveManager.Instance.SaveCampaign();
					}
				}
			}
		}

		// Token: 0x06001FFA RID: 8186 RVA: 0x000E0278 File Offset: 0x000DE478
		public void Restart(object iSender, RestartType iRestartType)
		{
			if (iSender is NetworkInterface || NetworkManager.Instance.State != NetworkState.Client)
			{
				if (iRestartType == RestartType.StartOfLevel)
				{
					RenderManager.Instance.TransitionEnd += this.RestartLevel;
				}
				else
				{
					if (iRestartType != RestartType.Checkpoint)
					{
						throw new ArgumentException("Invalid RestartType!", "iRestartType");
					}
					RenderManager.Instance.TransitionEnd += this.RestartAtCheckPoint;
				}
				InGameMenu.Hide();
				this.mBlizzardRainCount = 0U;
				this.mDiedInLevel = (iRestartType != RestartType.StartOfLevel);
				RenderManager.Instance.BeginTransition(Transitions.Fade, Color.Black, 0.5f);
				RenderManager.Instance.SkyMapColor = Vector3.One;
			}
		}

		// Token: 0x06001FFB RID: 8187 RVA: 0x000E03C0 File Offset: 0x000DE5C0
		private void RestartAtCheckPoint(TransitionEffect iDeadTransition)
		{
			Game.Instance.DisableRendering();
			RenderManager.Instance.TransitionEnd -= this.RestartAtCheckPoint;
			AudioManager.Instance.StopAll(AudioStopOptions.Immediate);
			this.EndCutscene(true);
			SpellManager.Instance.ClearMagicks();
			Game.Instance.AddLoadTask(delegate
			{
				while (!this.mBusy)
				{
					Thread.Sleep(1);
				}
				PlayState.sWaitingForPlayers = true;
				if (NetworkManager.Instance.Interface is NetworkServer)
				{
					(NetworkManager.Instance.Interface as NetworkServer).SetAllClientsBusy();
					GameRestartMessage gameRestartMessage;
					gameRestartMessage.Type = RestartType.Checkpoint;
					NetworkManager.Instance.Interface.SendMessage<GameRestartMessage>(ref gameRestartMessage);
				}
				this.ApplyState(this.mCheckpointState);
				RenderManager.Instance.EndTransition(Transitions.Fade, Color.Black, 0.5f);
				RenderManager.Instance.SkyMapColor = Vector3.One;
				Game.Instance.EnableRendering();
			});
			this.mDiedInLevel = true;
			for (int i = 0; i < Game.Instance.Players.Length; i++)
			{
				if (Game.Instance.Players[i].Playing && Game.Instance.Players[i].IconRenderer != null)
				{
					Game.Instance.Players[i].IconRenderer.TomeMagick = MagickType.None;
				}
			}
			SubMenuCharacterSelect.Instance.UpdateAvailableAvatars(default(DlcInstalled));
		}

		// Token: 0x06001FFC RID: 8188 RVA: 0x000E052C File Offset: 0x000DE72C
		private void RestartLevel(TransitionEffect iDeadTransition)
		{
			Game.Instance.DisableRendering();
			RenderManager.Instance.TransitionEnd -= this.RestartLevel;
			AudioManager.Instance.StopAll(AudioStopOptions.Immediate);
			SpellManager.Instance.ClearMagicks();
			this.mLevel.ClearDisplayTitles();
			if (this.mLevel.CurrentScene.RuleSet is VersusRuleset)
			{
				for (int i = 0; i < Game.Instance.Players.Length; i++)
				{
					Game.Instance.Players[i].UnlockedMagicks = 0UL;
				}
			}
			Game.Instance.AddLoadTask(delegate
			{
				while (!this.mBusy)
				{
					Thread.Sleep(1);
				}
				PlayState.sWaitingForPlayers = true;
				if (NetworkManager.Instance.Interface is NetworkServer)
				{
					(NetworkManager.Instance.Interface as NetworkServer).SetAllClientsBusy();
					GameRestartMessage gameRestartMessage;
					gameRestartMessage.Type = RestartType.StartOfLevel;
					NetworkManager.Instance.Interface.SendMessage<GameRestartMessage>(ref gameRestartMessage);
				}
				this.ApplyState(this.mLevelStartState);
				RenderManager.Instance.EndTransition(Transitions.Fade, Color.Black, 0.5f);
				RenderManager.Instance.SkyMapColor = Vector3.One;
				Game.Instance.EnableRendering();
			});
			this.mDiedInLevel = false;
			for (int j = 0; j < Game.Instance.Players.Length; j++)
			{
				this.mHasUsedMagick[j] = false;
				this.mTooFancyForFireballs[j] = false;
				if (Game.Instance.Players[j].Playing)
				{
					this.mTooFancyForFireballs[j] = true;
					this.mItsRainingBeastMen[j].Clear();
					this.mItsRainingBeastMen[j] = new HitList(10);
					if (Game.Instance.Players[j].IconRenderer != null)
					{
						Game.Instance.Players[j].IconRenderer.TomeMagick = MagickType.None;
					}
				}
			}
		}

		// Token: 0x06001FFD RID: 8189 RVA: 0x000E0665 File Offset: 0x000DE865
		private void ApplyState(PlayState.State iState)
		{
			this.ApplyState(iState, null);
		}

		// Token: 0x06001FFE RID: 8190 RVA: 0x000E0670 File Offset: 0x000DE870
		private void ApplyState(PlayState.State iState, Action<float> reportProgressBackAction)
		{
			this.EndCutscene(true);
			if (Credits.Instance.IsActive)
			{
				Credits.Instance.Kill();
			}
			this.mCutsceneSkipTipAlpha = 0f;
			this.mShowCutsceneSkipTip = false;
			this.mEndGameActive = false;
			this.mEndGameMusicActive = false;
			DialogManager.Instance.EndAll();
			this.mCamera.Release(0f);
			this.mTimeModifier = 1f;
			this.mTimeMultiplier = 1f;
			TutorialManager.Instance.Reset();
			if (this.mGenericHealtBar != null)
			{
				this.mGenericHealtBar.Reset();
			}
			this.mCamera.RemoveEffects();
			iState.ApplyState(reportProgressBackAction);
		}

		// Token: 0x06001FFF RID: 8191 RVA: 0x000E071C File Offset: 0x000DE91C
		private unsafe void Initialize()
		{
			TutorialManager.Instance.Initialize(this);
			TutorialManager.Instance.Reset();
			Action.ClearInstances();
			this.mInitialized = false;
			this.mBusy = true;
			this.mEndGamePhony = false;
			Game.Instance.DisableRendering();
			GraphicsDevice graphicsDevice = Game.Instance.GraphicsDevice;
			lock (graphicsDevice)
			{
				this.loadingScreen = new LoadingScreen(this.mShowProgress, this.mLoadingTip);
				goto IL_74;
			}
			IL_6E:
			Thread.Sleep(1);
			IL_74:
			if (!Game.Instance.RenderingEnabled)
			{
				this.mDiedInLevel = false;
				this.loadingScreen.Progress = 0f;
				this.loadingScreen.FadeIn(0.5f);
				this.mCutsceneSkipped = false;
				this.mCutscene = false;
				if (PlayState.sRecentPlayState != null)
				{
					PlayState.sRecentPlayState.Dispose();
				}
				PlayState.sRecentPlayState = this;
				this.loadingScreen.Progress = 0.01f;
				this.loadingScreen.Draw();
				this.mTimeModifier = 1f;
				this.mTimeMultiplier = 1f;
				this.mContent = new SharedContentManager(Game.Instance.Content.ServiceProvider);
				this.mInventoryBox = new InventoryBox();
				LanguageManager.Instance.SetPlayerStrings();
				this.mLevelFileName = Path.GetFullPath("content/Levels/" + this.mLevelFileName);
				this.mScene.Camera.Direction = new Vector3(0f, (float)Math.Sin(-0.6981317007977318), -(float)Math.Cos(-0.6981317007977318));
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.Load(this.mLevelFileName);
				this.mLevel = new Level(this.mLevelFileName, xmlDocument, this, this.mSpawnPoint, this.mSettings);
				this.mLevelHash = this.mLevel.ShaHash;
				this.mDialogHash = this.mLevel.DialogHash;
				foreach (GameScene gameScene in this.mLevel.Scenes)
				{
					byte[] shaHash = gameScene.ShaHash;
					for (int i = 0; i < 32; i++)
					{
						byte[] array = this.mLevelHash;
						int num = i;
						array[num] ^= shaHash[i];
					}
				}
				if (this.mLevel.Name.Equals("vs_boat", StringComparison.InvariantCultureIgnoreCase))
				{
					Profile.Instance.PlayingIslandCruise(this);
				}
				this.loadingScreen.Progress = 0.02f;
				this.loadingScreen.Draw();
				InGameMenu.Initialize(this);
				this.mLevel.Initialize();
				this.mPlayerStaffLight = false;
				this.loadingScreen.Progress = 0.03f;
				this.loadingScreen.Draw();
				this.mGenericHealtBar = new GenericHealthBar(this.mScene);
				this.mEntityManager = new EntityManager(this);
				this.loadingScreen.Progress = 0.04f;
				this.loadingScreen.Draw();
				this.mSpellEffects = new List<SpellEffect>(16);
				SpellEffect.IntializeCaches(this, this.mContent);
				this.loadingScreen.Progress = 0.05f;
				this.loadingScreen.Draw();
				Magicka.GameLogic.Spells.Magick.InitializeMagicks(this);
				this.loadingScreen.Progress = 0.06f;
				this.loadingScreen.Draw();
				LightningBolt.InitializeCache(Game.Instance.Content, 128);
				this.loadingScreen.Progress = 0.07f;
				this.loadingScreen.Draw();
				Railgun.InitializeCache(64);
				this.loadingScreen.Progress = 0.08f;
				this.loadingScreen.Draw();
				ArcaneBlast.InitializeCache(64);
				this.loadingScreen.Progress = 0.09f;
				this.loadingScreen.Draw();
				ArcaneBlade.InitializeCache(64);
				this.loadingScreen.Progress = 0.1f;
				this.loadingScreen.Draw();
				IceBlade.InitializeCache(64);
				this.loadingScreen.Progress = 0.11f;
				this.loadingScreen.Draw();
				UnderGroundAttack.InitializeCache(32, this);
				this.loadingScreen.Progress = 0.14f;
				this.loadingScreen.Draw();
				SpellMine.InitializeCache(64, this);
				this.loadingScreen.Progress = 0.17f;
				this.loadingScreen.Draw();
				IceSpikes.InitializeCache(32);
				this.loadingScreen.Progress = 0.2f;
				this.loadingScreen.Draw();
				TeslaField.InitializeCache(32, this);
				this.loadingScreen.Progress = 0.22f;
				this.loadingScreen.Draw();
				Shield.InitializeCache(64, this);
				this.loadingScreen.Progress = 0.25f;
				this.loadingScreen.Draw();
				Barrier.InitializeCache(128, this);
				WaveEntity.InitializeCache(16, this);
				this.loadingScreen.Progress = 0.28f;
				this.loadingScreen.Draw();
				Barrier.HitListWithBarriers.InitializeCache(32);
				this.loadingScreen.Progress = 0.3f;
				this.loadingScreen.Draw();
				SprayEntity.InitializeCache(this, 48);
				this.loadingScreen.Progress = 0.32f;
				this.loadingScreen.Draw();
				RadialBlur.InitializeCache(this.mContent, 16);
				this.loadingScreen.Progress = 0.35f;
				this.loadingScreen.Draw();
				Dispenser.InitializeCache(32, this);
				this.loadingScreen.Progress = 0.39f;
				this.loadingScreen.Draw();
				Gib.InitializeCache(256, this);
				this.loadingScreen.Progress = 0.41f;
				this.loadingScreen.Draw();
				DamageNotifyer.Instance.Scene = this.mScene;
				this.loadingScreen.Progress = 0.45f;
				this.loadingScreen.Draw();
				DynamicLight.Initialize(this.mScene);
				this.loadingScreen.Progress = 0.48f;
				this.loadingScreen.Draw();
				Avatar.InitializeCache(this);
				this.loadingScreen.Progress = 0.5f;
				this.loadingScreen.Draw();
				BookOfMagick.InitializeCache(64, this);
				this.loadingScreen.Progress = 0.52f;
				this.loadingScreen.Draw();
				this.mItsRainingBeastMen = new HitList[4];
				this.mTooFancyForFireballs = new bool[4];
				this.mHasUsedMagick = new bool[4];
				int num2 = Game.Instance.Players.Length;
				this.mInfo = default(SaveSlotInfo);
				if (this.mGameType == GameType.Campaign || this.mGameType == GameType.Mythos)
				{
					if (NetworkManager.Instance.State != NetworkState.Client)
					{
						this.mInfo = new SaveSlotInfo(this.mSaveSlot);
					}
					else
					{
						this.mInfo = (NetworkManager.Instance.Interface as NetworkClient).SaveSlot;
						while (!this.mInfo.IsValid)
						{
							this.mInfo = (NetworkManager.Instance.Interface as NetworkClient).SaveSlot;
							Thread.Sleep(100);
						}
						(NetworkManager.Instance.Interface as NetworkClient).ClearSaveSlot();
					}
				}
				this.loadingScreen.Progress = 0.55f;
				this.loadingScreen.Draw();
				for (int j = 0; j < num2; j++)
				{
					Player player = Game.Instance.Players[j];
					if (player.Playing && player.IconRenderer != null)
					{
						player.IconRenderer.TomeMagick = MagickType.None;
					}
					if (player.Playing)
					{
						this.mItsRainingBeastMen[j] = new HitList(10);
						this.mTooFancyForFireballs[j] = true;
					}
					if (this.mGameType == GameType.Campaign | this.mGameType == GameType.Mythos)
					{
						player.UnlockedMagicks = this.mInfo.UnlockedMagicks;
						if (player.Playing)
						{
							PlayerSaveData playerSaveData;
							if (!this.mInfo.Players.TryGetValue(Game.Instance.Players[j].GamerTag, out playerSaveData))
							{
								playerSaveData = new PlayerSaveData();
								this.mInfo.Players.Add(Game.Instance.Players[j].GamerTag, playerSaveData);
							}
							player.Staff = playerSaveData.Staff;
							player.Weapon = playerSaveData.Weapon;
						}
					}
					else if (this.mGameType == GameType.Challenge || this.mGameType == GameType.StoryChallange)
					{
						player.UnlockedMagicks = 2UL;
						player.Staff = null;
						player.Weapon = null;
					}
					else
					{
						player.UnlockedMagicks = 0UL;
						player.Staff = null;
						player.Weapon = null;
					}
					player.InitializeGame(this);
					if (player.Playing)
					{
						if (!string.IsNullOrEmpty(player.Weapon))
						{
							try
							{
								this.mContent.Load<Item>("data/items/wizard/" + player.Weapon);
							}
							catch
							{
								this.mContent.Load<Item>("data/items/npc/" + player.Weapon);
							}
						}
						if (!string.IsNullOrEmpty(player.Staff))
						{
							try
							{
								this.mContent.Load<Item>("data/items/wizard/" + player.Staff);
							}
							catch
							{
								this.mContent.Load<Item>("data/items/npc/" + player.Staff);
							}
						}
						player.Avatar = Avatar.GetFromCache(player);
						CharacterTemplate iTemplate = this.mContent.Load<CharacterTemplate>("Data/Characters/" + player.Gamer.Avatar.TypeName);
						player.Avatar.Initialize(iTemplate, default(Vector3), Player.UNIQUE_ID[j]);
						if (this.mLevel.SpawnFairy && Game.Instance.PlayerCount == 1 && (this.GameType == GameType.Campaign | this.GameType == GameType.Mythos))
						{
							this.SetSpawnFairies();
						}
					}
				}
				Item.InitializePickableCache(16, this);
				AIManager.Instance.Clear();
				this.loadingScreen.Progress = 0.6f;
				this.loadingScreen.Draw();
				ParticleLightBatcher.Instance.Initialize(Game.Instance.GraphicsDevice);
				if (GlobalSettings.Instance.ParticleLights)
				{
					ParticleLightBatcher.Instance.Enable(this.mScene);
				}
				PointLightBatcher.Instance.Initialize(Game.Instance.GraphicsDevice);
				PointLightBatcher.Instance.Enable(this.mScene);
				DecalManager.Instance.Initialize(this.mScene, Defines.DecalLimit());
				ParticleSystem.SetSpawnModifier(Defines.ParticleMultiplyer());
				this.mEndGameCondition = EndGameCondition.None;
				this.loadingScreen.Progress = 0.65f;
				this.loadingScreen.Draw();
				if (this.mSpawnPoint != null && this.mSpawnPoint != null)
				{
					this.StartupActionsPercentageAtStart = this.loadingScreen.Progress;
					this.StartupActionsPercentageTarget = 0.75f - this.StartupActionsPercentageAtStart;
					this.mLevel.GoToScene(this.mSpawnPoint.Value, Transitions.None, 0f, false, null, new Action<float>(this.StartupActionsReportHandle));
				}
				else
				{
					this.StartupActionsPercentageAtStart = this.loadingScreen.Progress;
					this.StartupActionsPercentageTarget = 0.75f - this.StartupActionsPercentageAtStart;
					this.mLevel.GoToScene(this.mLevel.SpawnPoint, Transitions.None, 0f, false, null, new Action<float>(this.StartupActionsReportHandle));
				}
				this.loadingScreen.Progress = 0.8f;
				this.loadingScreen.Draw();
				this.mLevelStartState = new PlayState.State(this);
				this.mCheckpointState = new PlayState.State(this);
				bool flag = false;
				NetworkState state = NetworkManager.Instance.State;
				if ((this.mGameType == GameType.Campaign || this.mGameType == GameType.Mythos) && ((state != NetworkState.Client && this.mSaveSlot.Checkpoint != null) || state == NetworkState.Client))
				{
					if (state == NetworkState.Client)
					{
						while (this.mCheckpointStream == null)
						{
							Thread.Sleep(100);
						}
					}
					else
					{
						this.mCheckpointStream = this.mSaveSlot.Checkpoint;
					}
					this.mCheckpointStream.Position = 0L;
					BinaryReader binaryReader = new BinaryReader(this.mCheckpointStream);
					byte[] iB = binaryReader.ReadBytes(32);
					byte[] iB2 = binaryReader.ReadBytes(32);
					if (Helper.ArrayEquals(this.mLevelHash, iB) && Helper.ArrayEquals(this.mDialogHash, iB2))
					{
						try
						{
							this.mCheckpointState.Read(binaryReader);
							flag = true;
						}
						catch
						{
							flag = false;
							this.mCheckpointState = new PlayState.State(this);
						}
					}
				}
				if (state == NetworkState.Server)
				{
					if (flag)
					{
						byte[] buffer = this.mCheckpointStream.GetBuffer();
						fixed (byte* ptr = buffer)
						{
							NetworkManager.Instance.Interface.SendRaw(PacketType.Checkpoint, (void*)ptr, (int)this.mCheckpointStream.Length);
						}
					}
					else
					{
						NetworkManager.Instance.Interface.SendRaw(PacketType.Checkpoint, null, 0);
					}
				}
				while (this.mCueToFinish != null)
				{
					if (this.mCueToFinish.IsStopped)
					{
						this.mCueToFinish = null;
					}
					else
					{
						Thread.Sleep(100);
					}
				}
				this.loadingScreen.Progress += 0.01f;
				this.loadingScreen.Draw();
				if (NetworkManager.Instance.State != NetworkState.Offline)
				{
					NetworkManager.Instance.Interface.Sync();
				}
				AudioManager.Instance.StopMusic();
				if (flag)
				{
					this.StartupActionsPercentageAtStart = this.loadingScreen.Progress;
					this.StartupActionsPercentageTarget = 1f - this.StartupActionsPercentageAtStart;
					this.ApplyState(this.mCheckpointState, new Action<float>(this.StartupActionsReportHandle));
				}
				PhysicsManager.Instance.Update(0f);
				this.mLevel.CurrentScene.Update(DataChannel.None, 0f);
				if (NetworkManager.Instance.State != NetworkState.Offline)
				{
					NetworkManager.Instance.Interface.Sync();
				}
				this.loadingScreen.Progress = 1f;
				this.loadingScreen.Draw();
				this.loadingScreen.FadeOut(0.5f);
				GC.Collect();
				GC.WaitForPendingFinalizers();
				if (!RenderManager.Instance.IsTransitionActive && !this.IgnoreInitFade)
				{
					RenderManager.Instance.EndTransition(Transitions.Fade, Color.Black, 0.5f);
				}
				Game.Instance.EnableRendering();
				this.mInitialized = true;
				return;
			}
			goto IL_6E;
		}

		// Token: 0x06002000 RID: 8192 RVA: 0x000E1548 File Offset: 0x000DF748
		private void StartupActionsReportHandle(float f)
		{
			if (this.loadingScreen != null)
			{
				this.loadingScreen.Progress = Math.Min(this.StartupActionsPercentageAtStart + this.StartupActionsPercentageTarget * f, 1f);
				this.loadingScreen.Draw();
			}
		}

		// Token: 0x06002001 RID: 8193 RVA: 0x000E1584 File Offset: 0x000DF784
		public void HookupLoadingScreen(out Action<float> loadingCallBack, out Action whenDoneCallBack, float startPercentage, float targetPercentage)
		{
			loadingCallBack = null;
			whenDoneCallBack = null;
			if (this.loadingScreen == null)
			{
				this.loadingScreen = new LoadingScreen(true, this.mLoadingTip, true);
			}
			else
			{
				this.loadingScreen.Initialize(true, this.mLoadingTip, true);
			}
			this.StartupActionsPercentageAtStart = startPercentage;
			this.StartupActionsPercentageTarget = targetPercentage;
			loadingCallBack = new Action<float>(this.StartupActionsReportHandle);
			whenDoneCallBack = new Action(this.TryDestroyLoadingScreen);
			Game.Instance.DisableRendering();
			this.loadingScreen.Progress = 0f;
			this.loadingScreen.FadeIn(0.5f);
		}

		// Token: 0x06002002 RID: 8194 RVA: 0x000E161C File Offset: 0x000DF81C
		private void TryDestroyLoadingScreen()
		{
			if (this.loadingScreen != null)
			{
				this.loadingScreen.FadeOut(0.5f);
				this.loadingScreen.EndDraw();
			}
			Game.Instance.EnableRendering();
		}

		// Token: 0x170007CF RID: 1999
		// (get) Token: 0x06002003 RID: 8195 RVA: 0x000E164B File Offset: 0x000DF84B
		// (set) Token: 0x06002004 RID: 8196 RVA: 0x000E1653 File Offset: 0x000DF853
		public bool IgnoreInitFade { get; set; }

		// Token: 0x170007D0 RID: 2000
		// (get) Token: 0x06002005 RID: 8197 RVA: 0x000E165C File Offset: 0x000DF85C
		public bool Initialized
		{
			get
			{
				return this.mInitialized;
			}
		}

		// Token: 0x170007D1 RID: 2001
		// (get) Token: 0x06002006 RID: 8198 RVA: 0x000E1664 File Offset: 0x000DF864
		public bool IsGameEnded
		{
			get
			{
				return this.mEndGameCondition != EndGameCondition.None;
			}
		}

		// Token: 0x06002007 RID: 8199 RVA: 0x000E1672 File Offset: 0x000DF872
		public bool IsNotClientAndHasCampaignCheckpoint()
		{
			return (this.mGameType == GameType.Campaign || this.mGameType == GameType.Mythos) && NetworkManager.Instance.State != NetworkState.Client && this.mSaveSlot != null && this.mSaveSlot.Checkpoint != null;
		}

		// Token: 0x06002008 RID: 8200 RVA: 0x000E16B0 File Offset: 0x000DF8B0
		public bool IsNotClientAndHasChallengeCheckpoint()
		{
			return this.mGameType == GameType.Challenge && NetworkManager.Instance.State != NetworkState.Client && this.mSaveSlot != null && this.mSaveSlot.Checkpoint != null;
		}

		// Token: 0x06002009 RID: 8201 RVA: 0x000E16E5 File Offset: 0x000DF8E5
		public void ClearCurrentCampaignCheckPoint()
		{
			if (!this.IsNotClientAndHasCampaignCheckpoint())
			{
				return;
			}
			this.mSaveSlot.Checkpoint = null;
		}

		// Token: 0x0600200A RID: 8202 RVA: 0x000E1730 File Offset: 0x000DF930
		public void ReloadFromCheckpoint()
		{
			if (!this.IsNotClientAndHasCampaignCheckpoint() && !this.IsNotClientAndHasChallengeCheckpoint())
			{
				this.Restart(this, RestartType.StartOfLevel);
				return;
			}
			this.mSaveSlot.Checkpoint.Position = 0L;
			BinaryReader reader = new BinaryReader(this.mSaveSlot.Checkpoint);
			byte[] iB = reader.ReadBytes(32);
			byte[] iB2 = reader.ReadBytes(32);
			if (Helper.ArrayEquals(this.mLevelHash, iB) && Helper.ArrayEquals(this.mDialogHash, iB2))
			{
				bool done = false;
				Game.Instance.AddLoadTask(delegate
				{
					this.mCheckpointState.Read(reader);
					done = true;
				});
				while (!done)
				{
					Thread.Sleep(1);
				}
				this.Restart(null, RestartType.Checkpoint);
				return;
			}
			this.Restart(this, RestartType.StartOfLevel);
		}

		// Token: 0x0600200B RID: 8203 RVA: 0x000E1814 File Offset: 0x000DFA14
		public void Endgame(EndGameCondition iType, bool iFreezeGame, bool iPhony, float iTime)
		{
			this.mEndGamePhony = iPhony;
			this.mEndGameCondition = iType;
			this.mEndGameTimer = Math.Min(iTime, this.mEndGameTimer);
			switch (iType)
			{
			case EndGameCondition.Victory:
				if (this.Level.CurrentScene.RuleSet is SurvivalRuleset)
				{
					AchievementsManager.Instance.AwardAchievement(this, "wearethechampions");
				}
				this.mEndGameText.SetText(LanguageManager.Instance.GetString(Defines.LOC_GAME_VICTORY));
				goto IL_166;
			case EndGameCondition.LevelComplete:
				if (this.mEndGamePhony)
				{
					RenderManager.Instance.TimeModifier = 0f;
				}
				this.mEndGameText.SetText(LanguageManager.Instance.GetString(Defines.LOC_GAME_LEVCOMP));
				Singleton<PlayerSegmentManager>.Instance.NotifyWonAgainstAI();
				goto IL_166;
			case EndGameCondition.Defeat:
				this.mEndGameText.SetText(LanguageManager.Instance.GetString(Defines.LOC_GAME_DEFEATED));
				if (TutorialUtils.IsInProgress)
				{
					TutorialUtils.Fail();
					goto IL_166;
				}
				goto IL_166;
			case EndGameCondition.VersusPlayer:
			case EndGameCondition.VersusTeam:
				this.mEndGameText.SetText(null);
				goto IL_166;
			case EndGameCondition.Disconnected:
				this.mEndGameText.SetText(null);
				goto IL_166;
			case EndGameCondition.ToBeContinued:
				this.mEndGameText.Clear();
				RenderManager.Instance.TransitionEnd += this.OnTransitionEnd;
				RenderManager.Instance.BeginTransition(Transitions.Fade, Color.Black, 8f);
				goto IL_166;
			}
			this.mEndGameText.SetText(null);
			IL_166:
			StatisticsManager.Instance.ClearPlayerMultiKillCounter();
			if (iFreezeGame)
			{
				ControlManager.Instance.LimitInput(this);
			}
		}

		// Token: 0x0600200C RID: 8204 RVA: 0x000E19A0 File Offset: 0x000DFBA0
		public void OnTransitionEnd(TransitionEffect iDeadTransition)
		{
			RenderManager.Instance.TransitionEnd -= this.OnTransitionEnd;
			ControlManager.Instance.UnlimitInput();
			InGameMenu.HideInstant();
			RenderManager.Instance.EndTransition(Transitions.CrossFade, Color.White, 1f);
			Tome.Instance.PlayCameraAnimation(Tome.CameraAnimation.Zoomed_In);
			Tome.Instance.PushMenuInstant(SubMenuEndGame.Instance, 1);
			Tome.Instance.ChangeState(Tome.ClosedState.Instance);
			GameStateManager.Instance.PopState();
		}

		// Token: 0x0600200D RID: 8205 RVA: 0x000E1A1B File Offset: 0x000DFC1B
		public void ChangingScene()
		{
			this.mCutsceneSkipped = false;
		}

		// Token: 0x0600200E RID: 8206 RVA: 0x000E1A24 File Offset: 0x000DFC24
		public void BeginCutscene(int iOnSkipID, bool iSkipBarMove, bool iKillDialogs)
		{
			this.UIEnabled = false;
			this.mCutsceneSkipped = false;
			this.mCutscene = true;
			this.mCutsceneTimer = (iSkipBarMove ? 1f : 0f);
			this.mCutsceneSkipTrigger = iOnSkipID;
			if (iKillDialogs)
			{
				DialogManager.Instance.EndAll();
			}
			for (int i = 0; i < this.mCutsceneRenderData.Length; i++)
			{
				this.mCutsceneRenderData[i].UpdateText();
			}
			this.mInventoryBox.Close(null);
			Vector3 cameraoffset = MagickCamera.CAMERAOFFSET;
			Vector3.Negate(ref cameraoffset, out cameraoffset);
			Vector3 position = this.mCamera.Position;
			Vector3.Add(ref position, ref cameraoffset, out position);
			SpellManager.Instance.ClearMagicks();
			List<Entity> entities = this.EntityManager.GetEntities(position, 40f, false);
			int j = 0;
			while (j < entities.Count)
			{
				IDamageable damageable = entities[j] as IDamageable;
				if (entities[j] is TornadoEntity | entities[j] is Barrier | entities[j] is SpellMine | entities[j] is Shield | entities[j] is Grease.GreaseField | entities[j] is SummonDeath.MagickDeath)
				{
					entities[j].Kill();
					goto IL_13E;
				}
				if (damageable != null)
				{
					goto IL_13E;
				}
				IL_1AD:
				j++;
				continue;
				IL_13E:
				Magicka.GameLogic.Entities.Character character = entities[j] as Magicka.GameLogic.Entities.Character;
				if (character == null)
				{
					goto IL_1AD;
				}
				character.StopAllActions();
				character.StopStatusEffects(StatusEffects.Frozen);
				if (character.IsEntangled)
				{
					character.ReleaseEntanglement();
				}
				if (character.IsFeared)
				{
					character.RemoveFear();
				}
				NonPlayerCharacter nonPlayerCharacter = character as NonPlayerCharacter;
				if (nonPlayerCharacter == null)
				{
					goto IL_1AD;
				}
				if (nonPlayerCharacter.IsCharmed)
				{
					nonPlayerCharacter.EndCharm();
				}
				if (nonPlayerCharacter.IsSummoned)
				{
					nonPlayerCharacter.Kill();
					goto IL_1AD;
				}
				goto IL_1AD;
			}
			this.EntityManager.ReturnEntityList(entities);
		}

		// Token: 0x0600200F RID: 8207 RVA: 0x000E1BFD File Offset: 0x000DFDFD
		public void EndCutscene(bool iSkipBarMove)
		{
			this.UIEnabled = true;
			this.mCutscene = false;
			this.mCutsceneSkipped = false;
			this.mCutsceneSkipRemoveBars = iSkipBarMove;
			DialogManager.Instance.EndAll();
		}

		// Token: 0x06002010 RID: 8208 RVA: 0x000E1C28 File Offset: 0x000DFE28
		public void SkipCutscene()
		{
			if (this.mCutsceneSkipped || NetworkManager.Instance.State == NetworkState.Client)
			{
				return;
			}
			if (!this.mShowCutsceneSkipTip)
			{
				this.mShowCutsceneSkipTip = true;
				return;
			}
			this.mCutsceneSkipped = true;
			if (this.mCutsceneSkipTrigger != 0)
			{
				this.Level.CurrentScene.ExecuteTrigger(this.mCutsceneSkipTrigger, null, false);
			}
			this.mCutsceneSkipTrigger = 0;
		}

		// Token: 0x170007D2 RID: 2002
		// (get) Token: 0x06002011 RID: 8209 RVA: 0x000E1C89 File Offset: 0x000DFE89
		public bool IsInCutscene
		{
			get
			{
				return this.mCutscene | this.mCutsceneTimer >= 1f;
			}
		}

		// Token: 0x06002012 RID: 8210 RVA: 0x000E1CA8 File Offset: 0x000DFEA8
		public override void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			this.mScene.ClearObjects(iDataChannel);
			if (!this.mInitialized || this.mLevel.Busy || !Game.Instance.RenderingEnabled)
			{
				this.mBusy = true;
				return;
			}
			this.mBusy = false;
			if (this.mOverlayIsPaused)
			{
				if (NetworkManager.Instance.State == NetworkState.Offline)
				{
					iDeltaTime = 0f;
				}
			}
			else if (InGameMenu.Visible)
			{
				InGameMenu.Update(iDataChannel, iDeltaTime);
				if (NetworkManager.Instance.State == NetworkState.Offline)
				{
					iDeltaTime = 0f;
				}
			}
			else if (this.mIsPaused)
			{
				AudioManager.Instance.ResumeAll();
				ControlManager.Instance.UnlimitInput(this);
				this.mIsPaused = false;
			}
			StatisticsManager.Instance.UpdatePlayerMultiKillCounter(iDeltaTime);
			this.mPlayTime += (double)iDeltaTime;
			iDeltaTime *= this.mTimeModifier * this.mTimeMultiplier;
			this.UpdatePhysics(iDataChannel, iDeltaTime);
			this.UpdateAI(iDataChannel, iDeltaTime);
			this.mEntityManager.UpdateQuadGrid();
			float iDeltaTime2 = iDeltaTime;
			if (this.mEndGamePhony && this.mEndGameCondition == EndGameCondition.LevelComplete)
			{
				iDeltaTime2 = 0f;
			}
			this.UpdateAnimatedLevelParts(iDataChannel, iDeltaTime2);
			this.UpdateEntitiesA(iDataChannel, iDeltaTime2);
			if (this.mBossFight != null)
			{
				this.mBossFight.Update(iDataChannel, iDeltaTime2);
			}
			if (this.mGenericHealtBar != null)
			{
				this.mGenericHealtBar.Update(iDataChannel, iDeltaTime);
			}
			this.mEntityManager.RemoveDeadEntities();
			this.UpdateMiscA(iDataChannel, iDeltaTime);
			this.UpdateMiscB(iDataChannel, iDeltaTime);
			for (int i = 0; i < Game.Instance.Players.Length; i++)
			{
				if (Game.Instance.Players[i].Playing)
				{
					this.mItsRainingBeastMen[i].Update(iDeltaTime);
					if (this.mItsRainingBeastMen[i].Count >= 5)
					{
						AchievementsManager.Instance.AwardAchievement(this, "itsrainingbeastmen");
					}
				}
			}
			NetworkInterface @interface = NetworkManager.Instance.Interface;
			if (@interface != null)
			{
				this.mNetworkUpdateTimer -= iDeltaTime;
				if (this.mNetworkUpdateTimer <= 0f)
				{
					this.mNetworkUpdateTimer = 0.033333335f;
					for (int j = 0; j < @interface.Connections; j++)
					{
						float iPrediction = @interface.GetLatency(j) * 0.5f;
						this.mEntityManager.UpdateNetwork(j, iPrediction);
					}
				}
			}
			ControlManager.Instance.HandleInput(iDataChannel, iDeltaTime);
		}

		// Token: 0x06002013 RID: 8211 RVA: 0x000E1ECF File Offset: 0x000E00CF
		private void UpdateAI(DataChannel iDataChannel, float iDeltaTime)
		{
			AIManager.Instance.Update(iDeltaTime);
		}

		// Token: 0x170007D3 RID: 2003
		// (get) Token: 0x06002014 RID: 8212 RVA: 0x000E1EDC File Offset: 0x000E00DC
		public double PlayTime
		{
			get
			{
				return this.mPlayTime;
			}
		}

		// Token: 0x170007D4 RID: 2004
		// (get) Token: 0x06002015 RID: 8213 RVA: 0x000E1EE4 File Offset: 0x000E00E4
		// (set) Token: 0x06002016 RID: 8214 RVA: 0x000E1EEC File Offset: 0x000E00EC
		public float TimeModifier
		{
			get
			{
				return this.mTimeModifier;
			}
			set
			{
				this.mTimeModifier = value;
			}
		}

		// Token: 0x170007D5 RID: 2005
		// (get) Token: 0x06002017 RID: 8215 RVA: 0x000E1EF5 File Offset: 0x000E00F5
		// (set) Token: 0x06002018 RID: 8216 RVA: 0x000E1EFD File Offset: 0x000E00FD
		public float TimeMultiplier
		{
			get
			{
				return this.mTimeMultiplier;
			}
			set
			{
				this.mTimeMultiplier = value;
			}
		}

		// Token: 0x06002019 RID: 8217 RVA: 0x000E1F06 File Offset: 0x000E0106
		private void UpdatePhysics(DataChannel iDataChannel, float iDeltaTime)
		{
			PhysicsManager.Instance.Update(iDeltaTime);
		}

		// Token: 0x0600201A RID: 8218 RVA: 0x000E1F13 File Offset: 0x000E0113
		private void UpdateEntitiesA(DataChannel iDataChannel, float iDeltaTime)
		{
			this.mEntityManager.Update(iDataChannel, iDeltaTime);
		}

		// Token: 0x0600201B RID: 8219 RVA: 0x000E1F24 File Offset: 0x000E0124
		private void UpdateMiscA(DataChannel iDataChannel, float iDeltaTime)
		{
			if (this.mEndGameCondition == EndGameCondition.Disconnected)
			{
				this.mEndGameText.SetText("NDFJ");
			}
			float iDeltaTime2 = iDeltaTime;
			if (this.mEndGamePhony && this.mEndGameCondition == EndGameCondition.LevelComplete)
			{
				iDeltaTime2 = 0f;
			}
			DecalManager.Instance.Update(iDataChannel, iDeltaTime2);
			ShadowBlobs.Instance.Update(iDataChannel, iDeltaTime2);
			Credits.Instance.Update(iDataChannel, iDeltaTime2, this);
			TutorialManager.Instance.Update(iDataChannel, iDeltaTime2);
			this.mInventoryBox.Update(iDeltaTime2, iDataChannel);
			if (this.mEndGameTimer > 0f)
			{
				bool flag = true;
				Player[] players = Game.Instance.Players;
				for (int i = 0; i < players.Length; i++)
				{
					Player player = players[i];
					if (player != null)
					{
						player.Update(iDataChannel, iDeltaTime2);
					}
					if (players[i].Playing && players[i].Avatar != null && (!players[i].Avatar.Dead || (Game.Instance.PlayerCount == 1 && players[i].Avatar.RevivalFairy.Active)))
					{
						flag = false;
					}
				}
				if (Credits.Instance.IsActive && (this.mGameType == GameType.Campaign || this.mGameType == GameType.Mythos))
				{
					flag = false;
				}
				if (flag & this.mGameType != GameType.Versus)
				{
					if (this.mEndGameCondition != EndGameCondition.Defeat)
					{
						this.Endgame(EndGameCondition.Defeat, false, false, 3f);
					}
					if (NetworkManager.Instance.State == NetworkState.Server)
					{
						GameEndMessage gameEndMessage;
						gameEndMessage.Condition = this.mEndGameCondition;
						gameEndMessage.DelayTime = 3f;
						gameEndMessage.Argument = 0;
						gameEndMessage.Phony = false;
						NetworkManager.Instance.Interface.SendMessage<GameEndMessage>(ref gameEndMessage);
					}
				}
				else if (this.mEndGameCondition == EndGameCondition.Defeat)
				{
					this.mEndGameCondition = EndGameCondition.None;
				}
			}
			if (this.mEndGameCondition != EndGameCondition.None)
			{
				if (this.mEndGameTimer <= -2f && (!this.mEndGameMusicActive || !AudioManager.Instance.IsMusicPlaying))
				{
					this.mEndGameTextAlpha = Math.Max(this.mEndGameTextAlpha - iDeltaTime, 0f);
				}
				else if (this.mEndGameTimer <= 0f)
				{
					this.mEndGameTextAlpha = Math.Min(this.mEndGameTextAlpha + iDeltaTime, 1f);
				}
				if (!this.mEndGameActive)
				{
					if (this.mEndGameTimer <= 0f && !this.mEndGameMusicActive)
					{
						switch (this.mEndGameCondition)
						{
						case EndGameCondition.Defeat:
							AudioManager.Instance.PlayMusic(Banks.Music, PlayState.MUSIC_DEFEAT, null);
							this.mEndGameMusicActive = true;
							goto IL_28E;
						case EndGameCondition.EightyeightMilesPerHour:
						case EndGameCondition.ChallengeExit:
						case EndGameCondition.EndOffGame:
							goto IL_28E;
						}
						AudioManager.Instance.PlayMusic(Banks.Music, PlayState.MUSIC_VICTORY, null);
						this.mEndGameMusicActive = true;
					}
					IL_28E:
					if (this.mEndGameCondition == EndGameCondition.ChallengeExit)
					{
						this.mEndGameActive = true;
						ControlManager.Instance.UnlimitInput(this);
						Texture2D screenShot = RenderManager.Instance.GetScreenShot(new Action(this.ScreenCaptureCallback));
						SubMenuEndGame.Instance.ScreenShot = screenShot;
						RenderManager.Instance.GetTransitionEffect(Transitions.CrossFade).SourceTexture1 = screenShot;
					}
					else if (this.mEndGameTimer < -3f && this.mEndGameTextAlpha <= 0f)
					{
						if (this.mEndGamePhony)
						{
							this.mEndGameCondition = EndGameCondition.None;
							this.mEndGamePhony = false;
							ControlManager.Instance.UnlimitInput(this);
							RenderManager.Instance.TimeModifier = 1f;
						}
						else
						{
							this.mEndGameActive = true;
							if (this.mGameType != GameType.Campaign && this.mGameType != GameType.Mythos)
							{
								if (this.mLevel.CurrentScene.RuleSet != null)
								{
									InGameMenu.Show(null);
									if (this.mLevel.CurrentScene.RuleSet is SurvivalRuleset)
									{
										InGameMenu.PushMenu(InGameMenuSurvivalStatistics.Instance);
									}
									else if (this.mLevel.CurrentScene.RuleSet is TimedObjectiveRuleset)
									{
										InGameMenu.PushMenu(InGameMenuTimedObjectiveStatistics.Instance);
									}
									else if (this.mLevel.CurrentScene.RuleSet is VersusRuleset)
									{
										InGameMenu.PushMenu(InGameMenuVersusStatistics.Instance);
									}
									this.mIsPaused = true;
								}
								else
								{
									this.Restart(this, RestartType.Checkpoint);
								}
							}
							else if (this.mEndGameCondition == EndGameCondition.LevelComplete)
							{
								ControlManager.Instance.UnlimitInput(this);
								Texture2D screenShot2 = RenderManager.Instance.GetScreenShot(new Action(this.ScreenCaptureCallback));
								SubMenuEndGame.Instance.ScreenShot = screenShot2;
								RenderManager.Instance.GetTransitionEffect(Transitions.CrossFade).SourceTexture1 = screenShot2;
								for (int j = 0; j < Game.Instance.Players.Length; j++)
								{
									if (Game.Instance.Players[j].Playing && !(Game.Instance.Players[j].Gamer is NetworkGamer) && this.mTooFancyForFireballs[j] && this.mHasUsedMagick[j])
									{
										AchievementsManager.Instance.AwardAchievement(this, "toofancyforfireballs");
									}
								}
							}
							else
							{
								this.Restart(this, RestartType.Checkpoint);
							}
							SubMenuEndGame.Instance.Set(this.mGameType == GameType.Mythos | this.mGameType == GameType.Campaign, this.mSaveSlot);
						}
					}
				}
				this.mEndGameTimer -= iDeltaTime;
			}
			else
			{
				this.mEndGameTextAlpha = 0f;
				this.mEndGameTimer = 3f;
			}
			if (this.mCutscene)
			{
				if (this.mCutsceneSkipTrigger != 0 && this.mShowCutsceneSkipTip)
				{
					this.mCutsceneSkipTipAlpha = Math.Min(this.mCutsceneSkipTipAlpha + iDeltaTime * 2f, 1f);
				}
				else
				{
					this.mCutsceneSkipTipAlpha = Math.Max(this.mCutsceneSkipTipAlpha - iDeltaTime * 2f, 0f);
				}
				this.mCutsceneTimer = Math.Min(this.mCutsceneTimer + iDeltaTime, 1f);
				PlayState.CutsceneRenderData cutsceneRenderData = this.mCutsceneRenderData[(int)iDataChannel];
				cutsceneRenderData.Time = this.mCutsceneTimer;
				cutsceneRenderData.mSkipAlpha = this.mCutsceneSkipTipAlpha;
				this.mScene.AddRenderableGUIObject(iDataChannel, cutsceneRenderData);
			}
			else if (this.mCutsceneTimer > 0f)
			{
				this.mCutsceneTimer = Math.Max(this.mCutsceneTimer - iDeltaTime, 0f);
				if (this.mShowCutsceneSkipTip)
				{
					this.mCutsceneSkipTipAlpha = Math.Max(this.mCutsceneSkipTipAlpha - iDeltaTime, 0f);
				}
				PlayState.CutsceneRenderData cutsceneRenderData2 = this.mCutsceneRenderData[(int)iDataChannel];
				cutsceneRenderData2.Time = this.mCutsceneTimer;
				cutsceneRenderData2.mSkipAlpha = this.mCutsceneSkipTipAlpha;
				this.mScene.AddRenderableGUIObject(iDataChannel, cutsceneRenderData2);
			}
			else
			{
				this.mCutsceneSkipTipAlpha = 0f;
				this.mShowCutsceneSkipTip = false;
			}
			EffectManager.Instance.Update(iDeltaTime2);
			SpellManager.Instance.Update(iDataChannel, iDeltaTime2, this);
			Healthbars.Instance.Update(iDataChannel, iDeltaTime2);
			this.mScene.Camera.Update(iDataChannel, iDeltaTime2);
			Matrix matrix;
			this.mScene.Camera.GetViewProjectionMatrix(iDataChannel, out matrix);
			DialogManager.Instance.Update(iDataChannel, iDeltaTime2, ref matrix);
			NetworkChat.Instance.Update(iDeltaTime);
			PlayState.RenderData renderData = this.mRenderData[(int)iDataChannel];
			renderData.PostTextAlpha = this.mEndGameTextAlpha;
			this.mScene.AddRenderableGUIObject(iDataChannel, renderData);
		}

		// Token: 0x0600201C RID: 8220 RVA: 0x000E25EC File Offset: 0x000E07EC
		private void ScreenCaptureCallback()
		{
			InGameMenu.HideInstant();
			RenderManager.Instance.EndTransition(Transitions.CrossFade, Color.White, 1f);
			Tome.Instance.PlayCameraAnimation(Tome.CameraAnimation.Zoomed_In);
			Tome.Instance.PushMenuInstant(SubMenuEndGame.Instance, 1);
			Tome.Instance.ChangeState(Tome.OpenState.Instance);
			GameStateManager.Instance.PopState();
		}

		// Token: 0x0600201D RID: 8221 RVA: 0x000E2648 File Offset: 0x000E0848
		private void UpdateMiscB(DataChannel iDataChannel, float iDeltaTime)
		{
			float iDeltaTime2 = iDeltaTime;
			if (this.mEndGamePhony && this.mEndGameCondition == EndGameCondition.LevelComplete)
			{
				iDeltaTime2 = 0f;
			}
			TracerMan.Instance.Update(iDeltaTime2);
			DamageNotifyer.Instance.Update(iDataChannel, iDeltaTime2);
			this.mLevel.Update(iDataChannel, iDeltaTime2);
			ParticleSystem.Instance.UpdateParticles(iDataChannel, iDeltaTime2);
			ParticleLightBatcher.Instance.Update(iDataChannel, iDeltaTime2);
			PointLightBatcher.Instance.Update(iDataChannel, iDeltaTime2);
		}

		// Token: 0x0600201E RID: 8222 RVA: 0x000E26B6 File Offset: 0x000E08B6
		private void UpdateAnimatedLevelParts(DataChannel iDataChannel, float iDeltaTime)
		{
			this.mLevel.UpdateAnimatedLevelParts(iDataChannel, iDeltaTime);
		}

		// Token: 0x0600201F RID: 8223 RVA: 0x000E26C8 File Offset: 0x000E08C8
		public void Dispose()
		{
			if (!this.mInitialized)
			{
				return;
			}
			this.mHasEntered = false;
			DialogManager.Instance.SetDialogs(null);
			Item.ClearCache();
			Avatar.ClearCache();
			CharacterTemplate.ClearCache();
			BossFight.Instance.Clear();
			RadialBlur.DisposeCache();
			this.mContent.Unload();
			this.mContent.Dispose();
			this.mContent = null;
			Entanglement.DisposeModels();
			SpellManager.Instance.ClearEffects();
			this.mEntityManager.Clear();
			this.mEntityManager = null;
			Entity.ClearHandles();
			this.mLevel.Dispose();
			this.mLevel = null;
			DecalManager.Instance.Initialize(null, 0);
			DialogManager.Instance.EndAll();
			AIManager.Instance.Clear();
			PhysicsManager.Instance.Clear();
			this.mScene = null;
			GC.Collect();
			GC.WaitForPendingFinalizers();
		}

		// Token: 0x06002020 RID: 8224 RVA: 0x000E27A0 File Offset: 0x000E09A0
		public override void OnEnter()
		{
			this.mHasEntered = true;
			if (!this.mInitialized)
			{
				Game.Instance.AddLoadTask(new Action(this.Initialize));
				NetworkChat.Instance.Set(PlayState.CHAT_SIZE.X, PlayState.CHAT_SIZE.Y, null, default(Microsoft.Xna.Framework.Rectangle), FontManager.Instance.GetFont(MagickaFont.Maiandra14), true, 10, false, 10f);
				NetworkChat.Instance.Active = false;
			}
			Microsoft.Xna.Framework.Rectangle rectangle = default(Microsoft.Xna.Framework.Rectangle);
			Point screenSize = RenderManager.Instance.ScreenSize;
			rectangle.Width = screenSize.X;
			rectangle.Height = screenSize.Y;
			AudioManager.Instance.SetListener(this.mCamera.Listener);
			this.mCamera.Init();
			this.mCamera.AspectRation = (float)rectangle.Width / (float)rectangle.Height;
		}

		// Token: 0x06002021 RID: 8225 RVA: 0x000E2884 File Offset: 0x000E0A84
		public override void OnExit()
		{
			if (!this.mInitialized)
			{
				return;
			}
			this.EndCutscene(true);
			Tome.Instance.ChangeState(Tome.OpenState.Instance);
			if (Credits.Instance.IsActive)
			{
				Credits.Instance.Kill();
			}
			ControlManager.Instance.ClearControllers();
			lock (Game.Instance.GraphicsDevice)
			{
				this.mScene.ClearObjects(DataChannel.A);
				this.mScene.ClearObjects(DataChannel.B);
				this.mScene.ClearObjects(DataChannel.C);
				PhysicsManager.Instance.Clear();
			}
			RenderManager.Instance.Fog = default(Fog);
			RenderManager.Instance.Brightness = 1f;
			RenderManager.Instance.Contrast = 1f;
			RenderManager.Instance.Saturation = 1f;
			RenderManager.Instance.SkyMap = null;
			RenderManager.Instance.BloomThreshold = 0.8f;
			RenderManager.Instance.BloomMultiplier = 1f;
			RenderManager.Instance.BlurSigma = 2.5f;
			AudioManager.Instance.StopAll(AudioStopOptions.AsAuthored);
			EffectManager.Instance.Clear();
			NetworkState state = NetworkManager.Instance.State;
			if (this.mGameType == GameType.Campaign | this.mGameType == GameType.Mythos)
			{
				if (state != NetworkState.Client && this.mEndGameCondition != EndGameCondition.Disconnected && this.mSaveSlot != null)
				{
					this.mSaveSlot.TotalPlayTime += (int)this.mPlayTime;
					this.mSaveSlot.CurrentPlayTime += (int)this.mPlayTime;
					SubMenuCampaignSelect_SaveSlotSelect.Instance.UpdateSlots();
				}
				this.mPlayTime = 0.0;
				if ((this.mEndGameCondition == EndGameCondition.LevelComplete | this.mEndGameCondition == EndGameCondition.Victory | this.mEndGameCondition == EndGameCondition.EndOffGame | this.mEndGameCondition == EndGameCondition.ToBeContinued) && state != NetworkState.Client && this.mSaveSlot != null)
				{
					int num = (int)(this.mSaveSlot.Level + 1);
					CampaignNode[] array = (SubMenuCharacterSelect.Instance.GameType == GameType.Mythos) ? LevelManager.Instance.MythosCampaign : LevelManager.Instance.VanillaCampaign;
					if (num >= array.Length)
					{
						num = 0;
						this.mSaveSlot.Looped = true;
					}
					this.mSaveSlot.Level = (byte)num;
					SubMenuCharacterSelect.Instance.SetSettings(this.mGameType, num, false);
					for (int i = 0; i < Game.Instance.Players.Length; i++)
					{
						if (Game.Instance.Players[i].Playing)
						{
							this.mSaveSlot.UnlockedMagicks |= Game.Instance.Players[i].UnlockedMagicks;
							PlayerSaveData playerSaveData = null;
							if (!this.mSaveSlot.Players.TryGetValue(Game.Instance.Players[i].GamerTag, out playerSaveData))
							{
								playerSaveData = new PlayerSaveData();
								this.mSaveSlot.Players.Add(Game.Instance.Players[i].GamerTag, playerSaveData);
							}
							playerSaveData.Staff = Game.Instance.Players[i].Staff;
							playerSaveData.Weapon = Game.Instance.Players[i].Weapon;
							this.mSaveSlot.Players[Game.Instance.Players[i].GamerTag] = playerSaveData;
						}
					}
				}
				this.mHasEntered = false;
				if (state != NetworkState.Client && this.mSaveSlot != null)
				{
					SaveManager.Instance.SaveCampaign();
				}
				if (this.mEndGameCondition == EndGameCondition.ChallengeExit)
				{
					Tome.Instance.PopMenu();
				}
			}
			int num2 = Game.Instance.Players.Length;
			for (int j = 0; j < num2; j++)
			{
				Player player = Game.Instance.Players[j];
				player.DeinitializeGame();
			}
			SaveManager.Instance.SaveLeaderBoards();
			Profile.Instance.Write();
			BossFight.Instance.Clear();
		}

		// Token: 0x170007D6 RID: 2006
		// (get) Token: 0x06002022 RID: 8226 RVA: 0x000E2C60 File Offset: 0x000E0E60
		public Level Level
		{
			get
			{
				return this.mLevel;
			}
		}

		// Token: 0x170007D7 RID: 2007
		// (get) Token: 0x06002023 RID: 8227 RVA: 0x000E2C68 File Offset: 0x000E0E68
		// (set) Token: 0x06002024 RID: 8228 RVA: 0x000E2C70 File Offset: 0x000E0E70
		public bool StaffLight
		{
			get
			{
				return this.mPlayerStaffLight;
			}
			set
			{
				this.mPlayerStaffLight = value;
			}
		}

		// Token: 0x170007D8 RID: 2008
		// (get) Token: 0x06002025 RID: 8229 RVA: 0x000E2C79 File Offset: 0x000E0E79
		public MagickCamera Camera
		{
			get
			{
				return this.mScene.Camera as MagickCamera;
			}
		}

		// Token: 0x170007D9 RID: 2009
		// (get) Token: 0x06002026 RID: 8230 RVA: 0x000E2C8B File Offset: 0x000E0E8B
		public EntityManager EntityManager
		{
			get
			{
				return this.mEntityManager;
			}
		}

		// Token: 0x170007DA RID: 2010
		// (get) Token: 0x06002027 RID: 8231 RVA: 0x000E2C93 File Offset: 0x000E0E93
		public ContentManager Content
		{
			get
			{
				return this.mContent;
			}
		}

		// Token: 0x170007DB RID: 2011
		// (get) Token: 0x06002028 RID: 8232 RVA: 0x000E2C9B File Offset: 0x000E0E9B
		public List<SpellEffect> SpellEffects
		{
			get
			{
				return this.mSpellEffects;
			}
		}

		// Token: 0x170007DC RID: 2012
		// (get) Token: 0x06002029 RID: 8233 RVA: 0x000E2CA3 File Offset: 0x000E0EA3
		// (set) Token: 0x0600202A RID: 8234 RVA: 0x000E2CAB File Offset: 0x000E0EAB
		public BossFight BossFight
		{
			get
			{
				return this.mBossFight;
			}
			set
			{
				this.mBossFight = value;
			}
		}

		// Token: 0x170007DD RID: 2013
		// (get) Token: 0x0600202B RID: 8235 RVA: 0x000E2CB4 File Offset: 0x000E0EB4
		// (set) Token: 0x0600202C RID: 8236 RVA: 0x000E2CBC File Offset: 0x000E0EBC
		public GenericHealthBar GenericHealthBar
		{
			get
			{
				return this.mGenericHealtBar;
			}
			set
			{
				this.mGenericHealtBar = value;
			}
		}

		// Token: 0x0600202D RID: 8237 RVA: 0x000E2CC5 File Offset: 0x000E0EC5
		public void OverlayPause(bool iPause)
		{
			if (iPause)
			{
				this.mOverlayIsPaused = true;
				AudioManager.Instance.PauseAll();
				return;
			}
			this.mOverlayIsPaused = false;
			AudioManager.Instance.PauseAll();
		}

		// Token: 0x0600202E RID: 8238 RVA: 0x000E2CF0 File Offset: 0x000E0EF0
		internal void TogglePause(Controller iController)
		{
			if (InGameMenu.Visible && (InGameMenu.CurrentMenu is InGameMenuSurvivalStatistics || InGameMenu.CurrentMenu is InGameMenuTimedObjectiveStatistics))
			{
				return;
			}
			if (!this.mIsPaused)
			{
				InGameMenu.Show(iController);
				this.mIsPaused = true;
				AudioManager.Instance.PauseAll();
				ControlManager.Instance.LimitInput(this);
				return;
			}
			InGameMenu.ControllerBack(iController);
		}

		// Token: 0x170007DE RID: 2014
		// (get) Token: 0x0600202F RID: 8239 RVA: 0x000E2D4E File Offset: 0x000E0F4E
		public bool IsPaused
		{
			get
			{
				return this.mIsPaused;
			}
		}

		// Token: 0x170007DF RID: 2015
		// (get) Token: 0x06002030 RID: 8240 RVA: 0x000E2D56 File Offset: 0x000E0F56
		// (set) Token: 0x06002031 RID: 8241 RVA: 0x000E2D5E File Offset: 0x000E0F5E
		public bool UIEnabled
		{
			get
			{
				return this.mUIEnabled;
			}
			set
			{
				this.mUIEnabled = value;
				KeyboardHUD.Instance.UIEnabled = this.mUIEnabled;
				Healthbars.Instance.UIEnabled = this.mUIEnabled;
			}
		}

		// Token: 0x170007E0 RID: 2016
		// (get) Token: 0x06002032 RID: 8242 RVA: 0x000E2D87 File Offset: 0x000E0F87
		// (set) Token: 0x06002033 RID: 8243 RVA: 0x000E2D8E File Offset: 0x000E0F8E
		public static bool WaitingForPlayers
		{
			get
			{
				return PlayState.sWaitingForPlayers;
			}
			set
			{
				PlayState.sWaitingForPlayers = value;
			}
		}

		// Token: 0x06002034 RID: 8244 RVA: 0x000E2D98 File Offset: 0x000E0F98
		public void IncrementBlizzardRainCount()
		{
			if (this.mLevel.CurrentScene.RuleSet is VersusRuleset)
			{
				this.mBlizzardRainCount += 1U;
				if (this.mBlizzardRainCount >= 20U)
				{
					AchievementsManager.Instance.AwardAchievement(this, "swedishsummer");
				}
			}
		}

		// Token: 0x170007E1 RID: 2017
		// (get) Token: 0x06002035 RID: 8245 RVA: 0x000E2DE4 File Offset: 0x000E0FE4
		public HitList[] ItsRainingBeastMen
		{
			get
			{
				return this.mItsRainingBeastMen;
			}
		}

		// Token: 0x170007E2 RID: 2018
		// (get) Token: 0x06002036 RID: 8246 RVA: 0x000E2DEC File Offset: 0x000E0FEC
		public bool[] TooFancyForFireballs
		{
			get
			{
				return this.mTooFancyForFireballs;
			}
		}

		// Token: 0x170007E3 RID: 2019
		// (get) Token: 0x06002037 RID: 8247 RVA: 0x000E2DF4 File Offset: 0x000E0FF4
		public bool[] HasUsedMagick
		{
			get
			{
				return this.mHasUsedMagick;
			}
		}

		// Token: 0x06002038 RID: 8248 RVA: 0x000E2DFC File Offset: 0x000E0FFC
		internal void Endgame(ref GameEndMessage iMsg)
		{
			this.mEndGameActive = false;
			this.Endgame(iMsg.Condition, iMsg.Argument != 0, iMsg.Phony, iMsg.DelayTime);
		}

		// Token: 0x170007E4 RID: 2020
		// (get) Token: 0x06002039 RID: 8249 RVA: 0x000E2E29 File Offset: 0x000E1029
		internal SaveSlotInfo Info
		{
			get
			{
				return this.mInfo;
			}
		}

		// Token: 0x0600203A RID: 8250 RVA: 0x000E2E34 File Offset: 0x000E1034
		internal void ToggleChat()
		{
			if (NetworkManager.Instance.State != NetworkState.Offline)
			{
				if (NetworkChat.Instance.Active)
				{
					NetworkChat.Instance.SendMessage();
				}
				NetworkChat.Instance.Active = !NetworkChat.Instance.Active;
				return;
			}
			NetworkChat.Instance.Active = false;
		}

		// Token: 0x0600203B RID: 8251 RVA: 0x000E2E86 File Offset: 0x000E1086
		internal void PlayMusic(Banks iBank, int iCueID, float? iFocusValue)
		{
			this.mMusicBank = iBank;
			this.mMusicCue = iCueID;
			this.mMusicFocusValue = iFocusValue;
			AudioManager.Instance.PlayMusic(iBank, iCueID, iFocusValue);
		}

		// Token: 0x0600203C RID: 8252 RVA: 0x000E2EAC File Offset: 0x000E10AC
		private void SetSpawnFairies()
		{
			if (Game.Instance.PlayerCount != 1 && (this.GameType == GameType.Mythos || this.GameType == GameType.Campaign))
			{
				return;
			}
			Player[] players = Game.Instance.Players;
			for (int i = 0; i < players.Length; i++)
			{
				this.mSpawnFairies[i] = (players[i] != null && players[i].Avatar != null);
			}
		}

		// Token: 0x0600203D RID: 8253 RVA: 0x000E2F10 File Offset: 0x000E1110
		internal void SpawnFairies()
		{
			if (Game.Instance.PlayerCount != 1 && (this.GameType == GameType.Mythos || this.GameType == GameType.Campaign))
			{
				return;
			}
			Player[] players = Game.Instance.Players;
			for (int i = 0; i < players.Length; i++)
			{
				if (players[i] != null && players[i].Avatar != null && this.mSpawnFairies[i])
				{
					players[i].Avatar.RevivalFairy.Initialize(this, false);
				}
			}
		}

		// Token: 0x0600203E RID: 8254 RVA: 0x000E2F84 File Offset: 0x000E1184
		internal void RemoveFairyFrom(Avatar mOwner)
		{
			Player[] players = Game.Instance.Players;
			for (int i = 0; i < players.Length; i++)
			{
				if (players[i] == mOwner.Player)
				{
					this.mSpawnFairies[i] = false;
				}
			}
		}

		// Token: 0x0400223A RID: 8762
		public const float NETWORK_UPDATE_PERIOD = 0.033333335f;

		// Token: 0x0400223B RID: 8763
		public static readonly Point CHAT_SIZE = new Point(600, 200);

		// Token: 0x0400223C RID: 8764
		public static readonly float CUTSCENE_BLACKBAR_SIZE = 0.1f;

		// Token: 0x0400223D RID: 8765
		public static readonly int STARTHASHCODE = "start".GetHashCodeCustom();

		// Token: 0x0400223E RID: 8766
		public static readonly int MUSIC_DEFEAT = "music_defeat".GetHashCodeCustom();

		// Token: 0x0400223F RID: 8767
		public static readonly int MUSIC_VICTORY = "music_fanfare".GetHashCodeCustom();

		// Token: 0x04002240 RID: 8768
		private static PlayState sRecentPlayState;

		// Token: 0x04002241 RID: 8769
		private PlayState.State mLevelStartState;

		// Token: 0x04002242 RID: 8770
		private PlayState.State mCheckpointState;

		// Token: 0x04002243 RID: 8771
		private GameType mGameType;

		// Token: 0x04002244 RID: 8772
		private double mPlayTime;

		// Token: 0x04002245 RID: 8773
		private float mTimeModifier = 1f;

		// Token: 0x04002246 RID: 8774
		private float mTimeMultiplier = 1f;

		// Token: 0x04002247 RID: 8775
		private EntityManager mEntityManager;

		// Token: 0x04002248 RID: 8776
		private string mLevelFileName;

		// Token: 0x04002249 RID: 8777
		private Level mLevel;

		// Token: 0x0400224A RID: 8778
		private ContentManager mContent;

		// Token: 0x0400224B RID: 8779
		private static bool sWaitingForPlayers = true;

		// Token: 0x0400224C RID: 8780
		private List<SpellEffect> mSpellEffects;

		// Token: 0x0400224D RID: 8781
		private Text mEndGameText;

		// Token: 0x0400224E RID: 8782
		private float mEndGameTextAlpha;

		// Token: 0x0400224F RID: 8783
		private bool mInitialized;

		// Token: 0x04002250 RID: 8784
		private bool mHasEntered;

		// Token: 0x04002251 RID: 8785
		private bool mIsPaused;

		// Token: 0x04002252 RID: 8786
		private bool mOverlayIsPaused;

		// Token: 0x04002253 RID: 8787
		private bool mUIEnabled = true;

		// Token: 0x04002254 RID: 8788
		private MagickCamera mCamera;

		// Token: 0x04002255 RID: 8789
		private BossFight mBossFight;

		// Token: 0x04002256 RID: 8790
		private GenericHealthBar mGenericHealtBar = new GenericHealthBar(null);

		// Token: 0x04002257 RID: 8791
		private bool mPlayerStaffLight;

		// Token: 0x04002258 RID: 8792
		private bool mEndGameActive;

		// Token: 0x04002259 RID: 8793
		private bool mEndGameMusicActive;

		// Token: 0x0400225A RID: 8794
		private bool mEndGamePhony;

		// Token: 0x0400225B RID: 8795
		private float mEndGameTimer;

		// Token: 0x0400225C RID: 8796
		private EndGameCondition mEndGameCondition;

		// Token: 0x0400225D RID: 8797
		private bool mBusy = true;

		// Token: 0x0400225E RID: 8798
		private float mNetworkUpdateTimer = 0.033333335f;

		// Token: 0x0400225F RID: 8799
		private PlayState.RenderData[] mRenderData;

		// Token: 0x04002260 RID: 8800
		private bool mCutscene;

		// Token: 0x04002261 RID: 8801
		private bool mCutsceneSkipped;

		// Token: 0x04002262 RID: 8802
		private int mCutsceneSkipTrigger;

		// Token: 0x04002263 RID: 8803
		private bool mShowCutsceneSkipTip;

		// Token: 0x04002264 RID: 8804
		private float mCutsceneSkipTipAlpha;

		// Token: 0x04002265 RID: 8805
		private bool mCutsceneSkipRemoveBars;

		// Token: 0x04002266 RID: 8806
		private float mCutsceneTimer;

		// Token: 0x04002267 RID: 8807
		private PlayState.CutsceneRenderData[] mCutsceneRenderData;

		// Token: 0x04002268 RID: 8808
		private bool[] mTooFancyForFireballs;

		// Token: 0x04002269 RID: 8809
		private bool[] mHasUsedMagick;

		// Token: 0x0400226A RID: 8810
		private HitList[] mItsRainingBeastMen;

		// Token: 0x0400226B RID: 8811
		private uint mBlizzardRainCount;

		// Token: 0x0400226C RID: 8812
		private bool mDiedInLevel;

		// Token: 0x0400226D RID: 8813
		private InventoryBox mInventoryBox;

		// Token: 0x0400226E RID: 8814
		private string mLoadingTip;

		// Token: 0x0400226F RID: 8815
		private bool mShowProgress = true;

		// Token: 0x04002270 RID: 8816
		private Cue mCueToFinish;

		// Token: 0x04002271 RID: 8817
		private bool[] mSpawnFairies = new bool[4];

		// Token: 0x04002272 RID: 8818
		private Banks mMusicBank;

		// Token: 0x04002273 RID: 8819
		private int mMusicCue;

		// Token: 0x04002274 RID: 8820
		private float? mMusicFocusValue;

		// Token: 0x04002275 RID: 8821
		private SpawnPoint? mSpawnPoint;

		// Token: 0x04002276 RID: 8822
		private SaveData mSaveSlot;

		// Token: 0x04002277 RID: 8823
		private SaveSlotInfo mInfo;

		// Token: 0x04002278 RID: 8824
		private MemoryStream mCheckpointStream;

		// Token: 0x04002279 RID: 8825
		private VersusRuleset.Settings mSettings;

		// Token: 0x0400227A RID: 8826
		private byte[] mLevelHash;

		// Token: 0x0400227B RID: 8827
		private byte[] mDialogHash;

		// Token: 0x0400227C RID: 8828
		private LoadingScreen loadingScreen;

		// Token: 0x0400227D RID: 8829
		private float StartupActionsPercentageAtStart;

		// Token: 0x0400227E RID: 8830
		private float StartupActionsPercentageTarget;

		// Token: 0x0200040E RID: 1038
		protected class RenderData : IRenderableGUIObject
		{
			// Token: 0x06002042 RID: 8258 RVA: 0x000E301E File Offset: 0x000E121E
			public RenderData(GUIBasicEffect iEffect, Text iText)
			{
				this.mEffect = iEffect;
				this.mText = iText;
			}

			// Token: 0x06002043 RID: 8259 RVA: 0x000E3034 File Offset: 0x000E1234
			public void Draw(float iDeltaTime)
			{
				Point screenSize = RenderManager.Instance.ScreenSize;
				if (this.PostTextAlpha > 1E-45f)
				{
					this.mEffect.SetScreenSize(screenSize.X, screenSize.Y);
					Vector4 color = default(Vector4);
					color.X = (color.Y = (color.Z = 1f));
					color.W = this.PostTextAlpha;
					this.mEffect.Color = color;
					this.mEffect.Begin();
					this.mEffect.CurrentTechnique.Passes[0].Begin();
					this.mText.Draw(this.mEffect, (float)screenSize.X * 0.5f, (float)(screenSize.Y - this.mText.Font.LineHeight) * 0.5f);
					this.mEffect.CurrentTechnique.Passes[0].End();
					this.mEffect.End();
				}
				if (NetworkManager.Instance.State != NetworkState.Offline)
				{
					Vector2 vector = default(Vector2);
					vector.Y = (float)(screenSize.Y - NetworkChat.Instance.Size.Y) - 200f;
					NetworkChat.Instance.Draw(ref vector);
				}
			}

			// Token: 0x170007E5 RID: 2021
			// (get) Token: 0x06002044 RID: 8260 RVA: 0x000E3187 File Offset: 0x000E1387
			public int ZIndex
			{
				get
				{
					return 999;
				}
			}

			// Token: 0x04002280 RID: 8832
			private Text mText;

			// Token: 0x04002281 RID: 8833
			public float PostTextAlpha;

			// Token: 0x04002282 RID: 8834
			private GUIBasicEffect mEffect;
		}

		// Token: 0x0200040F RID: 1039
		protected class CutsceneRenderData : IRenderableGUIObject
		{
			// Token: 0x06002045 RID: 8261 RVA: 0x000E3190 File Offset: 0x000E1390
			static CutsceneRenderData()
			{
				lock (Game.Instance.GraphicsDevice)
				{
					PlayState.CutsceneRenderData.sVertexBuffer = new VertexBuffer(Game.Instance.GraphicsDevice, Defines.QUAD_COL_VERTS_TL.Length * PlayState.CutsceneRenderData.sVertexStride, BufferUsage.None);
					PlayState.CutsceneRenderData.sVertexBuffer.SetData<VertexPositionColor>(Defines.QUAD_COL_VERTS_TL);
				}
			}

			// Token: 0x06002046 RID: 8262 RVA: 0x000E3228 File Offset: 0x000E1428
			public CutsceneRenderData(GUIBasicEffect iEffect)
			{
				this.mEffect = iEffect;
				this.mTransform = Matrix.Identity;
				this.mText = new Text(100, FontManager.Instance.GetFont(MagickaFont.Maiandra18), TextAlign.Right, false);
				this.mSkipText = null;
			}

			// Token: 0x06002047 RID: 8263 RVA: 0x000E3264 File Offset: 0x000E1464
			public void UpdateText()
			{
				if (this.mSkipText == null)
				{
					if (this.mSkipText == null)
					{
						this.mSkipText = LanguageManager.Instance.GetString(PlayState.CutsceneRenderData.CUTSCENE_SKIP_TIP);
						bool flag = false;
						Player[] players = Game.Instance.Players;
						for (int i = 0; i < players.Length; i++)
						{
							if (players[i].Playing && !(players[i].Gamer is NetworkGamer))
							{
								flag = !(players[i].Controller is KeyboardMouseController);
								break;
							}
						}
						ButtonChar buttonChar;
						if (flag)
						{
							buttonChar = ButtonChar.A;
						}
						else
						{
							buttonChar = ButtonChar.SpaceBar;
						}
						string text = this.mSkipText;
						string oldValue = "#1;";
						char c = (char)buttonChar;
						this.mSkipText = text.Replace(oldValue, c.ToString());
					}
					this.mText.SetText(this.mSkipText);
				}
			}

			// Token: 0x06002048 RID: 8264 RVA: 0x000E3328 File Offset: 0x000E1528
			public void Draw(float iDeltaTime)
			{
				Point screenSize = RenderManager.Instance.ScreenSize;
				this.mEffect.SetScreenSize(screenSize.X, screenSize.Y);
				this.mTransform.M11 = (float)screenSize.X;
				this.mTransform.M22 = (float)screenSize.Y * PlayState.CUTSCENE_BLACKBAR_SIZE;
				this.mTransform.M42 = -this.mTransform.M22 + this.mTransform.M22 * this.Time;
				this.mEffect.TextureEnabled = false;
				this.mEffect.VertexColorEnabled = true;
				this.mEffect.Color = new Vector4(1f);
				this.mEffect.Transform = this.mTransform;
				this.mEffect.GraphicsDevice.Vertices[0].SetSource(PlayState.CutsceneRenderData.sVertexBuffer, 0, PlayState.CutsceneRenderData.sVertexStride);
				this.mEffect.GraphicsDevice.VertexDeclaration = PlayState.CutsceneRenderData.sVertexDeclaration;
				this.mEffect.GraphicsDevice.RenderState.CullMode = CullMode.None;
				this.mEffect.Begin();
				this.mEffect.CurrentTechnique.Passes[0].Begin();
				this.mEffect.GraphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleFan, 0, 2);
				this.mTransform.M42 = (float)screenSize.Y - this.mTransform.M22 * this.Time;
				this.mEffect.Transform = this.mTransform;
				this.mEffect.CommitChanges();
				this.mEffect.GraphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleFan, 0, 2);
				Vector4 vector = new Vector4(1f, 1f, 1f, this.mSkipAlpha);
				this.mEffect.Color = vector;
				this.mText.DefaultColor = vector;
				this.mText.Draw(this.mEffect, (float)screenSize.X - 20f, (float)screenSize.Y - (float)screenSize.Y * PlayState.CUTSCENE_BLACKBAR_SIZE * 0.5f - (float)this.mText.Font.LineHeight * 0.5f);
				this.mEffect.CurrentTechnique.Passes[0].End();
				this.mEffect.End();
				this.mEffect.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
			}

			// Token: 0x170007E6 RID: 2022
			// (get) Token: 0x06002049 RID: 8265 RVA: 0x000E3590 File Offset: 0x000E1790
			public int ZIndex
			{
				get
				{
					return 990;
				}
			}

			// Token: 0x04002283 RID: 8835
			private Matrix mTransform;

			// Token: 0x04002284 RID: 8836
			public float Time;

			// Token: 0x04002285 RID: 8837
			private GUIBasicEffect mEffect;

			// Token: 0x04002286 RID: 8838
			private Text mText;

			// Token: 0x04002287 RID: 8839
			private string mSkipText;

			// Token: 0x04002288 RID: 8840
			public float mSkipAlpha;

			// Token: 0x04002289 RID: 8841
			private static VertexBuffer sVertexBuffer;

			// Token: 0x0400228A RID: 8842
			private static VertexDeclaration sVertexDeclaration = RenderManager.Instance.CreateVertexDeclaration(VertexPositionColor.VertexElements);

			// Token: 0x0400228B RID: 8843
			private static int sVertexStride = VertexPositionColor.SizeInBytes;

			// Token: 0x0400228C RID: 8844
			private static readonly int CUTSCENE_SKIP_TIP = "#cut_press_skip".GetHashCodeCustom();
		}

		// Token: 0x02000410 RID: 1040
		private class State
		{
			// Token: 0x0600204A RID: 8266 RVA: 0x000E3598 File Offset: 0x000E1798
			public State(PlayState iPlayState)
			{
				this.mPlayState = iPlayState;
				this.mCamera = new MagickCamera.State(this.mPlayState.mCamera);
				this.mLevel = new Level.State(this.mPlayState.mLevel);
				if (DialogManager.Instance.Dialogs != null)
				{
					this.mDialogState = new DialogCollection.State(DialogManager.Instance.Dialogs);
				}
				this.UpdateState(null, null);
				this.mFirstStart = true;
			}

			// Token: 0x0600204B RID: 8267 RVA: 0x000E3658 File Offset: 0x000E1858
			public void UpdateState(Matrix[] iSpawnPoints, List<int> iIgnoredTriggers)
			{
				this.mFirstStart = false;
				this.mIgnoredTriggers.Clear();
				if (iIgnoredTriggers != null)
				{
					this.mIgnoredTriggers.AddRange(iIgnoredTriggers);
				}
				this.mSpawnPoints = iSpawnPoints;
				this.mCamera.UpdateState();
				this.mLevel.UpdateState();
				if (this.mDialogState != null)
				{
					this.mDialogState.UpdateState();
				}
				this.mMusicBank = this.mPlayState.mMusicBank;
				this.mMusicCue = this.mPlayState.mMusicCue;
				this.mMusicFocusValue = this.mPlayState.mMusicFocusValue;
				this.mUnlockedMagicks = 0UL;
				Player[] players = Game.Instance.Players;
				for (int i = 0; i < players.Length; i++)
				{
					if (players[i].Playing)
					{
						this.mUnlockedMagicks |= players[i].UnlockedMagicks;
						if (this.mPlayState.GameType == GameType.Campaign | this.mPlayState.GameType == GameType.Mythos)
						{
							this.mWeapons[i] = players[i].Weapon;
							this.mStaves[i] = players[i].Staff;
							this.mHasUsedMagick[i] = this.mPlayState.HasUsedMagick[i];
							this.mTooFancyForFireballs[i] = this.mPlayState.TooFancyForFireballs[i];
						}
						this.mSpawnFairies[i] = (players[i].Avatar != null && players[i].Avatar.RevivalFairy != null && players[i].Avatar.RevivalFairy.Active);
					}
					else
					{
						this.mSpawnFairies[i] = false;
					}
				}
				Array.Copy(this.mSpawnFairies, this.mPlayState.mSpawnFairies, this.mPlayState.mSpawnFairies.Length);
			}

			// Token: 0x0600204C RID: 8268 RVA: 0x000E37FF File Offset: 0x000E19FF
			public void ApplyState()
			{
				this.ApplyState(null);
			}

			// Token: 0x0600204D RID: 8269 RVA: 0x000E3808 File Offset: 0x000E1A08
			public unsafe void ApplyState(Action<float> reportProgressBackAction)
			{
				this.mPlayState.Inventory.Close(null);
				EffectManager.Instance.Clear();
				ControlManager.Instance.UnlimitInput();
				DamageNotifyer.Instance.Clear();
				BossFight.Instance.Reset();
				this.mPlayState.mEntityManager.Clear();
				TutorialManager.Instance.Reset();
				DecalManager.Instance.Clear();
				this.mPlayState.mEndGameCondition = EndGameCondition.None;
				this.mPlayState.mMusicBank = this.mMusicBank;
				this.mPlayState.mMusicCue = this.mMusicCue;
				this.mPlayState.mMusicFocusValue = this.mMusicFocusValue;
				AudioManager.Instance.StopMusic();
				AudioManager.Instance.PlayMusic(this.mMusicBank, this.mMusicCue, this.mMusicFocusValue);
				DialogManager.Instance.Reset();
				if (this.mDialogState != null)
				{
					this.mDialogState.ApplyState();
				}
				this.mLevel.ApplyState(this.mIgnoredTriggers, reportProgressBackAction);
				this.mPlayState.Camera.Release(0.001f);
				this.mCamera.ApplyState();
				this.mPlayState.Camera.ClearPlayers();
				Player[] players = Game.Instance.Players;
				for (int i = 0; i < players.Length; i++)
				{
					Player player = players[i];
					if (player.Playing)
					{
						if ("weapon_unarmed".Equals(this.mWeapons[i], StringComparison.OrdinalIgnoreCase) && (TutorialManager.Instance.EnabledElements & Elements.Basic) != Elements.None)
						{
							PlayerSaveData playerSaveData;
							if (this.mPlayState.mInfo.Players.TryGetValue(player.GamerTag, out playerSaveData))
							{
								this.mWeapons[i] = playerSaveData.Weapon;
							}
							else
							{
								this.mWeapons[i] = "";
							}
						}
						if ("weapon_unarmed".Equals(this.mStaves[i], StringComparison.OrdinalIgnoreCase) && (TutorialManager.Instance.EnabledElements & Elements.Basic) != Elements.None)
						{
							PlayerSaveData playerSaveData2;
							if (this.mPlayState.mInfo.Players.TryGetValue(player.GamerTag, out playerSaveData2))
							{
								this.mStaves[i] = playerSaveData2.Weapon;
							}
							else
							{
								this.mStaves[i] = "";
							}
						}
						player.Weapon = this.mWeapons[i];
						player.Staff = this.mStaves[i];
						player.UnlockedMagicks = this.mUnlockedMagicks;
						this.mPlayState.HasUsedMagick[i] = this.mHasUsedMagick[i];
						this.mPlayState.TooFancyForFireballs[i] = this.mTooFancyForFireballs[i];
						if (player.Avatar != null)
						{
							player.Avatar.Terminate(true, true);
						}
						player.Avatar = Avatar.GetFromCache(player);
						player.Avatar.Initialize(CharacterTemplate.GetCachedTemplate(player.Gamer.Avatar.Type), default(Vector3), Player.UNIQUE_ID[player.ID]);
						Array.Copy(this.mSpawnFairies, this.mPlayState.mSpawnFairies, this.mPlayState.mSpawnFairies.Length);
						if (Game.Instance.PlayerCount == 1 && this.mSpawnFairies[i])
						{
							player.Avatar.RevivalFairy.Initialize(this.mPlayState, false);
						}
						else
						{
							player.Avatar.RevivalFairy.Kill();
						}
						if (player.Controller != null)
						{
							player.Controller.Invert(false);
						}
						if (!this.mPlayState.mLevel.SpawnPoint.SpawnPlayers)
						{
							player.Avatar.Body.DisableBody();
						}
						else
						{
							Matrix orientation = default(Matrix);
							if (this.mSpawnPoints != null)
							{
								orientation = this.mSpawnPoints[i];
							}
							else if (this.mPlayState.Level.CurrentScene.RuleSet is VersusRuleset)
							{
								int teamArea = (this.mPlayState.Level.CurrentScene.RuleSet as VersusRuleset).GetTeamArea(player.Team);
								TriggerArea triggerArea;
								if (!this.mPlayState.mLevel.CurrentScene.TryGetLocator(teamArea, out orientation) && this.mPlayState.mLevel.CurrentScene.TryGetTriggerArea(teamArea, out triggerArea))
								{
									orientation.Translation = triggerArea.GetRandomLocation();
								}
							}
							else
							{
								SpawnPoint spawnPoint = this.mPlayState.mLevel.SpawnPoint;
								int j;
								for (j = i; j >= 0; j--)
								{
									Locator locator;
									if (this.mPlayState.mLevel.CurrentScene.LevelModel.Locators.TryGetValue((&spawnPoint.Locations.FixedElementField)[j], out locator))
									{
										orientation = locator.Transform;
										break;
									}
								}
								orientation.M41 += 2f * (float)(i - j);
								orientation.M42 += 1f;
							}
							Segment iSeg = default(Segment);
							Transform transform = default(Transform);
							transform.Orientation = orientation;
							transform.Orientation.Translation = default(Vector3);
							iSeg.Delta.Y = -5f;
							iSeg.Origin = orientation.Translation;
							float num;
							Vector3 position;
							Vector3 vector;
							if (!this.mPlayState.Level.CurrentScene.SegmentIntersect(out num, out position, out vector, iSeg))
							{
								int num2 = 0;
								while (num2 < this.mPlayState.Level.CurrentScene.Level.CurrentScene.Liquids.Length && !this.mPlayState.Level.CurrentScene.Liquids[num2].SegmentIntersect(out num, out position, out vector, ref iSeg, true, true, false))
								{
									num2++;
								}
							}
							transform.Position = position;
							transform.Position.Y = transform.Position.Y + (player.Avatar.Radius + player.Avatar.Capsule.Length * 0.5f);
							player.Avatar.CharacterBody.MoveTo(transform.Position, transform.Orientation);
							this.mPlayState.mEntityManager.AddEntity(player.Avatar);
						}
					}
				}
				if (this.mFirstStart && this.mPlayState.mLevel.NoItems)
				{
					Item item = this.mPlayState.Content.Load<Item>("Data/Items/Wizard/weapon_unarmed");
					for (int k = 0; k < players.Length; k++)
					{
						if (players[k].Playing && players[k].Avatar != null)
						{
							Avatar avatar = players[k].Avatar;
							item.Copy(avatar.Equipment[1].Item);
							item.Copy(avatar.Equipment[0].Item);
						}
					}
				}
				if (NetworkManager.Instance.State != NetworkState.Offline)
				{
					NetworkManager.Instance.Interface.Sync();
				}
			}

			// Token: 0x0600204E RID: 8270 RVA: 0x000E3EA8 File Offset: 0x000E20A8
			public void Read(BinaryReader iReader)
			{
				this.mFirstStart = false;
				this.mIgnoredTriggers.Clear();
				int num = iReader.ReadInt32();
				for (int i = 0; i < num; i++)
				{
					this.mIgnoredTriggers.Add(iReader.ReadInt32());
				}
				if (iReader.ReadBoolean())
				{
					this.mSpawnPoints = new Matrix[4];
					Vector3 up = Vector3.Up;
					for (int j = 0; j < 4; j++)
					{
						Vector3 vector = default(Vector3);
						vector.X = iReader.ReadSingle();
						vector.Y = iReader.ReadSingle();
						vector.Z = iReader.ReadSingle();
						Vector3 vector2 = default(Vector3);
						vector2.X = iReader.ReadSingle();
						vector2.Y = iReader.ReadSingle();
						vector2.Z = iReader.ReadSingle();
						Matrix.CreateWorld(ref vector, ref vector2, ref up, out this.mSpawnPoints[j]);
					}
				}
				else
				{
					this.mSpawnPoints = null;
				}
				this.mCamera.Read(iReader);
				this.mLevel.Read(iReader);
				this.mDialogState.Read(iReader);
				this.mMusicBank = (Banks)iReader.ReadUInt16();
				this.mMusicCue = iReader.ReadInt32();
				if (iReader.ReadBoolean())
				{
					this.mMusicFocusValue = new float?(iReader.ReadSingle());
				}
				else
				{
					this.mMusicFocusValue = null;
				}
				Player[] players = Game.Instance.Players;
				for (int k = 0; k < players.Length; k++)
				{
					this.mHasUsedMagick[k] = false;
					this.mTooFancyForFireballs[k] = false;
				}
				this.mUnlockedMagicks = iReader.ReadUInt64();
				num = iReader.ReadInt32();
				for (int l = 0; l < num; l++)
				{
					string value = iReader.ReadString();
					string text = iReader.ReadString();
					string text2 = iReader.ReadString();
					bool flag = iReader.ReadBoolean();
					bool flag2 = iReader.ReadBoolean();
					for (int m = 0; m < players.Length; m++)
					{
						if (players[m].Playing && players[m].GamerTag.Equals(value))
						{
							this.mWeapons[l] = text;
							this.mStaves[l] = text2;
							if (!string.IsNullOrEmpty(text))
							{
								try
								{
									this.mPlayState.mContent.Load<Item>("data/items/wizard/" + text);
								}
								catch
								{
									this.mPlayState.mContent.Load<Item>("data/items/npc/" + text);
								}
							}
							if (!string.IsNullOrEmpty(text2))
							{
								try
								{
									this.mPlayState.mContent.Load<Item>("data/items/wizard/" + text2);
								}
								catch
								{
									this.mPlayState.mContent.Load<Item>("data/items/npc/" + text2);
								}
							}
							this.mHasUsedMagick[l] = flag;
							this.mTooFancyForFireballs[l] = flag2;
						}
					}
				}
				bool flag3 = iReader.ReadBoolean();
				for (int n = 0; n < this.mSpawnFairies.Length; n++)
				{
					this.mSpawnFairies[n] = flag3;
				}
			}

			// Token: 0x0600204F RID: 8271 RVA: 0x000E41BC File Offset: 0x000E23BC
			public void Write(BinaryWriter iWriter)
			{
				iWriter.Write(this.mIgnoredTriggers.Count);
				for (int i = 0; i < this.mIgnoredTriggers.Count; i++)
				{
					iWriter.Write(this.mIgnoredTriggers[i]);
				}
				iWriter.Write(this.mSpawnPoints != null);
				if (this.mSpawnPoints != null)
				{
					for (int j = 0; j < 4; j++)
					{
						Vector3 translation = this.mSpawnPoints[j].Translation;
						iWriter.Write(translation.X);
						iWriter.Write(translation.Y);
						iWriter.Write(translation.Z);
						Vector3 forward = this.mSpawnPoints[j].Forward;
						iWriter.Write(forward.X);
						iWriter.Write(forward.Y);
						iWriter.Write(forward.Z);
					}
				}
				this.mCamera.Write(iWriter);
				this.mLevel.Write(iWriter);
				this.mDialogState.Write(iWriter);
				iWriter.Write((ushort)this.mMusicBank);
				iWriter.Write(this.mMusicCue);
				iWriter.Write(this.mMusicFocusValue != null);
				if (this.mMusicFocusValue != null)
				{
					iWriter.Write(this.mMusicFocusValue.Value);
				}
				iWriter.Write(this.mUnlockedMagicks);
				iWriter.Write(Game.Instance.PlayerCount);
				Player[] players = Game.Instance.Players;
				for (int k = 0; k < players.Length; k++)
				{
					if (players[k].Playing)
					{
						iWriter.Write(players[k].GamerTag);
						iWriter.Write((players[k].Weapon != null) ? players[k].Weapon : "");
						iWriter.Write((players[k].Staff != null) ? players[k].Staff : "");
						iWriter.Write(this.mHasUsedMagick[k]);
						iWriter.Write(this.mTooFancyForFireballs[k]);
					}
				}
				bool flag = false;
				for (int l = 0; l < this.mSpawnFairies.Length; l++)
				{
					flag |= this.mSpawnFairies[l];
				}
				iWriter.Write(flag);
			}

			// Token: 0x0400228D RID: 8845
			private PlayState mPlayState;

			// Token: 0x0400228E RID: 8846
			private MagickCamera.State mCamera;

			// Token: 0x0400228F RID: 8847
			private bool[] mSpawnFairies = new bool[4];

			// Token: 0x04002290 RID: 8848
			private Matrix[] mSpawnPoints;

			// Token: 0x04002291 RID: 8849
			private DialogCollection.State mDialogState;

			// Token: 0x04002292 RID: 8850
			private Level.State mLevel;

			// Token: 0x04002293 RID: 8851
			private Banks mMusicBank;

			// Token: 0x04002294 RID: 8852
			private int mMusicCue;

			// Token: 0x04002295 RID: 8853
			private float? mMusicFocusValue;

			// Token: 0x04002296 RID: 8854
			private List<int> mIgnoredTriggers = new List<int>(10);

			// Token: 0x04002297 RID: 8855
			public string[] mWeapons = new string[4];

			// Token: 0x04002298 RID: 8856
			public string[] mStaves = new string[4];

			// Token: 0x04002299 RID: 8857
			public ulong mUnlockedMagicks;

			// Token: 0x0400229A RID: 8858
			public bool[] mTooFancyForFireballs = new bool[4];

			// Token: 0x0400229B RID: 8859
			public bool[] mHasUsedMagick = new bool[4];

			// Token: 0x0400229C RID: 8860
			private bool mFirstStart;
		}
	}
}
