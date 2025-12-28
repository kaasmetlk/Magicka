using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Xml;
using Magicka.Audio;
using Magicka.GameLogic;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.UI;
using Magicka.Levels.Versus;
using Magicka.Network;
using Magicka.Physics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using PolygonHead.Lights;
using PolygonHead.ParticleEffects;

namespace Magicka.Levels
{
	// Token: 0x0200000B RID: 11
	public class Level : IDisposable
	{
		// Token: 0x0600000E RID: 14 RVA: 0x000022D8 File Offset: 0x000004D8
		internal unsafe Level(string iFileName, XmlDocument iInput, PlayState iPlayState, SpawnPoint? iSpawnPoint, VersusRuleset.Settings iSettings)
		{
			this.mSettings = iSettings;
			SHA256 sha = SHA256.Create();
			try
			{
				using (FileStream fileStream = File.OpenRead(iFileName))
				{
					this.mShaHash = sha.ComputeHash(fileStream);
				}
			}
			catch (Exception)
			{
				this.mShaHash = new byte[32];
			}
			this.mPlayState = iPlayState;
			this.mSpawnPoint.SpawnPlayers = true;
			if (iPlayState.GameType == GameType.Campaign | iPlayState.GameType == GameType.Mythos)
			{
				this.mForceCamera = true;
			}
			string directoryName = Path.GetDirectoryName(iFileName);
			this.mName = Path.GetFileNameWithoutExtension(iFileName);
			XmlNode xmlNode = null;
			for (int i = 0; i < iInput.ChildNodes.Count; i++)
			{
				if (iInput.ChildNodes[i].Name.Equals("Level", StringComparison.OrdinalIgnoreCase))
				{
					xmlNode = iInput.ChildNodes[i];
					break;
				}
			}
			if (xmlNode == null)
			{
				throw new Exception("No Level node found in level XML!");
			}
			List<Level.AvatarItem> list = new List<Level.AvatarItem>();
			if (iSpawnPoint != null)
			{
				this.mSpawnPoint = iSpawnPoint.Value;
			}
			for (int j = 0; j < xmlNode.ChildNodes.Count; j++)
			{
				XmlNode xmlNode2 = xmlNode.ChildNodes[j];
				if (xmlNode2.Name.Equals("Name", StringComparison.OrdinalIgnoreCase))
				{
					this.mDisplayName = xmlNode2.InnerText.ToLowerInvariant().GetHashCodeCustom();
				}
				else if (xmlNode2.Name.Equals("Description", StringComparison.OrdinalIgnoreCase))
				{
					this.mDescription = xmlNode2.InnerText.ToLowerInvariant().GetHashCodeCustom();
				}
				else if (xmlNode2.Name.Equals("Start", StringComparison.OrdinalIgnoreCase) & iSpawnPoint == null)
				{
					for (int k = 0; k < xmlNode2.Attributes.Count; k++)
					{
						XmlAttribute xmlAttribute = xmlNode2.Attributes[k];
						if (xmlAttribute.Name.Equals("Scene", StringComparison.OrdinalIgnoreCase))
						{
							this.mSpawnPoint.Scene = xmlAttribute.Value.ToLowerInvariant().GetHashCodeCustom();
						}
						else if (xmlAttribute.Name.Equals("Area", StringComparison.OrdinalIgnoreCase))
						{
							fixed (int* ptr = &this.mSpawnPoint.Locations.FixedElementField)
							{
								for (int l = 0; l < 4; l++)
								{
									ptr[l] = (xmlAttribute.Value + l).GetHashCodeCustom();
								}
							}
						}
						else if (xmlAttribute.Name.Equals("SpawnPlayers", StringComparison.OrdinalIgnoreCase))
						{
							this.mSpawnPoint.SpawnPlayers = bool.Parse(xmlAttribute.Value);
						}
						else if (xmlAttribute.Name.Equals("NoItems", StringComparison.OrdinalIgnoreCase))
						{
							this.mNoItems = bool.Parse(xmlAttribute.Value);
						}
						else if (xmlAttribute.Name.Equals("ForceCamera", StringComparison.OrdinalIgnoreCase))
						{
							this.mForceCamera = bool.Parse(xmlAttribute.Value);
						}
						else if (xmlAttribute.Name.Equals("SpawnFairy", StringComparison.OrdinalIgnoreCase))
						{
							this.mSpawnFairy = bool.Parse(xmlAttribute.Value);
						}
					}
				}
				else
				{
					if (xmlNode2.Name.Equals("Dialogs", StringComparison.OrdinalIgnoreCase))
					{
						string text = Path.Combine(directoryName, xmlNode2.InnerText);
						this.mDialogs = new DialogCollection(text);
						try
						{
							using (FileStream fileStream2 = File.OpenRead(text))
							{
								this.mDialogHash = sha.ComputeHash(fileStream2);
							}
							goto IL_505;
						}
						catch (Exception)
						{
							this.mDialogHash = new byte[32];
							goto IL_505;
						}
					}
					if (xmlNode2.Name.Equals("Scenes", StringComparison.OrdinalIgnoreCase))
					{
						this.ReadScenes(xmlNode2, directoryName, iPlayState.Content);
					}
					else if (xmlNode2.Name.Equals("AdditionalItems", StringComparison.OrdinalIgnoreCase))
					{
						foreach (object obj in xmlNode2.ChildNodes)
						{
							XmlNode xmlNode3 = (XmlNode)obj;
							if (!(xmlNode3 is XmlComment) && xmlNode3.Name.Equals("Item", StringComparison.OrdinalIgnoreCase))
							{
								Level.AvatarItem item = default(Level.AvatarItem);
								foreach (object obj2 in xmlNode3.Attributes)
								{
									XmlAttribute xmlAttribute2 = (XmlAttribute)obj2;
									if (xmlAttribute2.Name.Equals("item", StringComparison.OrdinalIgnoreCase))
									{
										item.Item = iPlayState.Content.Load<Item>(xmlAttribute2.Value);
									}
									else if (xmlAttribute2.Name.Equals("Bone", StringComparison.OrdinalIgnoreCase))
									{
										item.Bone = xmlAttribute2.Value;
									}
								}
								if (item.Item != null && !string.IsNullOrEmpty(item.Bone))
								{
									list.Add(item);
								}
							}
						}
					}
				}
				IL_505:;
			}
			if (this.mDialogs != null)
			{
				this.mDialogs.Initialize(iPlayState.Scene);
			}
			else
			{
				this.mDialogHash = new byte[32];
			}
			this.mAdditionalAvatarItems = list.ToArray();
			this.mHasTitleDisplaying = false;
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600000F RID: 15 RVA: 0x0000288C File Offset: 0x00000A8C
		public byte[] ShaHash
		{
			get
			{
				return this.mShaHash;
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000010 RID: 16 RVA: 0x00002894 File Offset: 0x00000A94
		public byte[] DialogHash
		{
			get
			{
				return this.mDialogHash;
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000011 RID: 17 RVA: 0x0000289C File Offset: 0x00000A9C
		internal Level.AvatarItem[] AdditionalAvatarItems
		{
			get
			{
				return this.mAdditionalAvatarItems;
			}
		}

		// Token: 0x06000012 RID: 18 RVA: 0x000028A4 File Offset: 0x00000AA4
		private void ReadScenes(XmlNode iNode, string iRelativePath, ContentManager iContent)
		{
			for (int i = 0; i < iNode.ChildNodes.Count; i++)
			{
				XmlNode xmlNode = iNode.ChildNodes[i];
				if (!(xmlNode is XmlComment))
				{
					if (!xmlNode.Name.Equals("Scene", StringComparison.OrdinalIgnoreCase))
					{
						throw new Exception("Invalid XML node in level file! Node name: " + iNode.Name);
					}
					XmlDocument xmlDocument = new XmlDocument();
					string text = Path.Combine(iRelativePath, xmlNode.InnerText);
					xmlDocument.Load(text);
					GameScene gameScene = new GameScene(this, text, xmlDocument, iContent, this.mSettings);
					this.mScenes.Add(gameScene.ID, gameScene);
				}
			}
		}

		// Token: 0x06000013 RID: 19 RVA: 0x0000294A File Offset: 0x00000B4A
		public void Initialize()
		{
			DialogManager.Instance.SetDialogs(this.mDialogs);
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000014 RID: 20 RVA: 0x0000295C File Offset: 0x00000B5C
		public GameScene CurrentScene
		{
			get
			{
				return this.mCurrentScene;
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000015 RID: 21 RVA: 0x00002964 File Offset: 0x00000B64
		public string Name
		{
			get
			{
				return this.mName;
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000016 RID: 22 RVA: 0x0000296C File Offset: 0x00000B6C
		public bool ForceCamera
		{
			get
			{
				return this.mForceCamera;
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000017 RID: 23 RVA: 0x00002974 File Offset: 0x00000B74
		public bool SpawnFairy
		{
			get
			{
				return this.mSpawnFairy;
			}
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000018 RID: 24 RVA: 0x0000297C File Offset: 0x00000B7C
		public IEnumerable<GameScene> Scenes
		{
			get
			{
				return this.mScenes.Values;
			}
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00002989 File Offset: 0x00000B89
		public void UpdateAnimatedLevelParts(DataChannel iDataChannel, float iDeltaTime)
		{
			this.mCurrentScene.UpdateAnimatedLevelParts(iDataChannel, iDeltaTime);
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00002998 File Offset: 0x00000B98
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			if (!this.mPlayState.IsPaused && !this.PlayState.IsGameEnded)
			{
				if (this.mHasTitleDisplaying)
				{
					TitleRenderData titleRenderData = this.mTitleRenderer.Update((int)iDataChannel, iDeltaTime);
					if (titleRenderData == null)
					{
						this.mHasTitleDisplaying = false;
					}
					else
					{
						this.mPlayState.Scene.AddRenderableGUIObject(iDataChannel, titleRenderData);
					}
				}
				if (!this.PlayState.IsInCutscene)
				{
					for (int i = 0; i < this.mPlayedTimers.Count; i++)
					{
						Level.TimerBody value = this.mPlayedTimers[i];
						if (!value.Paused)
						{
							value.Time += iDeltaTime;
							this.mPlayedTimers[i] = value;
						}
					}
				}
			}
			for (int j = 0; j < this.mTimers.Count; j++)
			{
				Level.TimerBody value2 = this.mTimers[j];
				if (!value2.Paused)
				{
					value2.Time += iDeltaTime;
					this.mTimers[j] = value2;
				}
			}
			this.mCurrentScene.Update(iDataChannel, iDeltaTime);
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600001B RID: 27 RVA: 0x00002AA5 File Offset: 0x00000CA5
		public PlayState PlayState
		{
			get
			{
				return this.mPlayState;
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600001C RID: 28 RVA: 0x00002AAD File Offset: 0x00000CAD
		public SpawnPoint SpawnPoint
		{
			get
			{
				return this.mSpawnPoint;
			}
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600001D RID: 29 RVA: 0x00002AB5 File Offset: 0x00000CB5
		public SpawnPoint NextSceneSpawnPoint
		{
			get
			{
				return this.mNextSceneSpawnPoint;
			}
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00002AC0 File Offset: 0x00000CC0
		public int GetCounterValue(int iID)
		{
			int result;
			if (this.mCounters.TryGetValue(iID, out result))
			{
				return result;
			}
			return 0;
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00002AE0 File Offset: 0x00000CE0
		public void SetCounterValue(int iID, int iValue)
		{
			this.mCounters[iID] = iValue;
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00002AF0 File Offset: 0x00000CF0
		public void AddToCounter(int iID, int iValue)
		{
			int num;
			if (!this.mCounters.TryGetValue(iID, out num))
			{
				num = 0;
			}
			this.mCounters[iID] = num + iValue;
		}

		// Token: 0x06000021 RID: 33 RVA: 0x00002B20 File Offset: 0x00000D20
		public void AddTimer(int iID, bool iRealTime, float iValue)
		{
			if (iRealTime)
			{
				for (int i = 0; i < this.mPlayedTimers.Count; i++)
				{
					if (this.mPlayedTimers[i].ID == iID)
					{
						return;
					}
				}
				this.mPlayedTimers.Add(new Level.TimerBody(iID, iValue, false));
				return;
			}
			for (int j = 0; j < this.mTimers.Count; j++)
			{
				if (this.mTimers[j].ID == iID)
				{
					return;
				}
			}
			this.mTimers.Add(new Level.TimerBody(iID, iValue, false));
		}

		// Token: 0x06000022 RID: 34 RVA: 0x00002BB0 File Offset: 0x00000DB0
		public void PauseTimer(int iID, bool iPause)
		{
			for (int i = 0; i < this.mPlayedTimers.Count; i++)
			{
				if (this.mPlayedTimers[i].ID == iID)
				{
					Level.TimerBody value = this.mPlayedTimers[i];
					value.Paused = iPause;
					this.mPlayedTimers[i] = value;
					return;
				}
			}
			for (int j = 0; j < this.mTimers.Count; j++)
			{
				if (this.mTimers[j].ID == iID)
				{
					Level.TimerBody value2 = this.mTimers[j];
					value2.Paused = iPause;
					this.mTimers[j] = value2;
					return;
				}
			}
		}

		// Token: 0x06000023 RID: 35 RVA: 0x00002C58 File Offset: 0x00000E58
		public void SetTimer(int iID, float iValue)
		{
			for (int i = 0; i < this.mPlayedTimers.Count; i++)
			{
				if (this.mPlayedTimers[i].ID == iID)
				{
					Level.TimerBody value = this.mPlayedTimers[i];
					value.Time = iValue;
					this.mPlayedTimers[i] = value;
					return;
				}
			}
			for (int j = 0; j < this.mTimers.Count; j++)
			{
				if (this.mTimers[j].ID == iID)
				{
					Level.TimerBody value2 = this.mTimers[j];
					value2.Time = iValue;
					this.mTimers[j] = value2;
					return;
				}
			}
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00002D00 File Offset: 0x00000F00
		public float GetTimerValue(int iID)
		{
			for (int i = 0; i < this.mPlayedTimers.Count; i++)
			{
				if (this.mPlayedTimers[i].ID == iID)
				{
					return this.mPlayedTimers[i].Time;
				}
			}
			for (int j = 0; j < this.mTimers.Count; j++)
			{
				if (this.mTimers[j].ID == iID)
				{
					return this.mTimers[j].Time;
				}
			}
			return 0f;
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00002D8A File Offset: 0x00000F8A
		public void GoToScene(SpawnPoint iSpawnPoint, Transitions iTransition, float iTransitionTime, bool iSaveNPCs, Action iOnComplete)
		{
			this.GoToScene(iSpawnPoint, iTransition, iTransitionTime, iSaveNPCs, iOnComplete, null);
		}

		// Token: 0x06000026 RID: 38 RVA: 0x00002D9C File Offset: 0x00000F9C
		public void GoToScene(SpawnPoint iSpawnPoint, Transitions iTransition, float iTransitionTime, bool iSaveNPCs, Action iOnComplete, Action<float> reportProgressAction)
		{
			this.gotoSceneLoadingDelegate = reportProgressAction;
			this.ClearDisplayTitles();
			this.mNextScene = this.mScenes[iSpawnPoint.Scene];
			this.mNextSceneTransition = iTransition;
			this.mNextSceneTransitionTime = iTransitionTime;
			this.mNextSceneSaveNPCs = iSaveNPCs;
			this.mNextSceneDoneCallback = null;
			this.mOnComplete = iOnComplete;
			this.mNextSceneSpawnPoint = iSpawnPoint;
			if (this.mCurrentScene == null)
			{
				this.mBusy = true;
				if (this.mNoItems)
				{
					Player[] players = Game.Instance.Players;
					Item item = this.PlayState.Content.Load<Item>("Data/Items/Wizard/weapon_unarmed");
					for (int i = 0; i < players.Length; i++)
					{
						if (players[i].Playing && players[i].Avatar != null)
						{
							Avatar avatar = players[i].Avatar;
							item.Copy(avatar.Equipment[1].Item);
							item.Copy(avatar.Equipment[0].Item);
							players[i].Weapon = "weapon_unarmed";
							players[i].Staff = "weapon_unarmed";
						}
					}
				}
				this.ChangeScene();
				return;
			}
			if (iTransition == Transitions.None || iTransitionTime < 1E-45f)
			{
				this.mBusy = true;
				Game.Instance.AddLoadTask(new Action(this.ChangeScene));
			}
			else
			{
				RenderManager.Instance.BeginTransition(iTransition, Color.Black, iTransitionTime);
				RenderManager.Instance.TransitionEnd += this.TransitionFinish;
			}
			AudioManager.Instance.TargetReverbMix = 0f;
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00002F0B File Offset: 0x0000110B
		private void TransitionFinish(TransitionEffect iOldEffect)
		{
			RenderManager.Instance.TransitionEnd -= this.TransitionFinish;
			this.mBusy = true;
			Game.Instance.AddLoadTask(new Action(this.ChangeScene));
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00002F40 File Offset: 0x00001140
		private void ChangeScene()
		{
			this.mBusy = true;
			while (!this.mPlayState.Busy)
			{
				Thread.Sleep(100);
			}
			if (this.gotoSceneLoadingDelegate == null && this.mNextScene != null && this.mNextScene.GetNumStartupActions() > 300)
			{
				this.mPlayState.HookupLoadingScreen(out this.gotoSceneLoadingDelegate, out this.mNextSceneDoneCallback, 0f, 1f);
			}
			this.mNextSceneDoneCallback = (Action)Delegate.Combine(this.mNextSceneDoneCallback, this.mOnComplete);
			if (this.PlayState.BossFight != null)
			{
				this.PlayState.BossFight.Reset();
			}
			if (this.mPlayState.GenericHealthBar != null)
			{
				this.mPlayState.GenericHealthBar.Reset();
			}
			GameScene gameScene = this.mCurrentScene;
			if (gameScene != null)
			{
				gameScene.Destroy(this.mNextSceneSaveNPCs);
			}
			this.mCurrentScene = this.mNextScene;
			this.mNextScene.LoadLevel();
			if (this.mCurrentScene != gameScene && gameScene != null)
			{
				gameScene.UnloadContent();
			}
			this.mNextScene.Initialize(this.mNextSceneSpawnPoint, false, this.gotoSceneLoadingDelegate);
			AudioManager instance = AudioManager.Instance;
			instance.TargetReverbMix = this.mNextScene.ReverbMix;
			this.PlayState.ChangingScene();
			instance.SetRoomType(this.mNextScene.RoomType);
			Thread.Sleep(0);
			if (this.mPlayState.Initialized)
			{
				NetworkServer networkServer = NetworkManager.Instance.Interface as NetworkServer;
				NetworkClient networkClient = NetworkManager.Instance.Interface as NetworkClient;
				if (networkServer != null)
				{
					while (!networkServer.AllClientsReady)
					{
						Thread.Sleep(100);
					}
					GameEndLoadMessage gameEndLoadMessage = default(GameEndLoadMessage);
					networkServer.SendMessage<GameEndLoadMessage>(ref gameEndLoadMessage);
					networkServer.SetAllClientsBusy();
				}
				else if (networkClient != null)
				{
					GameEndLoadMessage gameEndLoadMessage2 = default(GameEndLoadMessage);
					networkClient.SendMessage<GameEndLoadMessage>(ref gameEndLoadMessage2, 0);
					while (PlayState.WaitingForPlayers)
					{
						Thread.Sleep(100);
					}
					PlayState.WaitingForPlayers = true;
				}
				if (this.mNextSceneDoneCallback != null)
				{
					this.mNextSceneDoneCallback.Invoke();
					this.mNextSceneDoneCallback = null;
				}
				PhysicsManager.Instance.Update(0f);
				this.mCurrentScene.Update(DataChannel.None, 0f);
				Thread.Sleep(0);
				if (networkServer != null)
				{
					while (!networkServer.AllClientsReady)
					{
						Thread.Sleep(100);
					}
					GameEndLoadMessage gameEndLoadMessage3 = default(GameEndLoadMessage);
					networkServer.SendMessage<GameEndLoadMessage>(ref gameEndLoadMessage3);
					networkServer.SetAllClientsBusy();
				}
				else if (networkClient != null)
				{
					GameEndLoadMessage gameEndLoadMessage4 = default(GameEndLoadMessage);
					networkClient.SendMessage<GameEndLoadMessage>(ref gameEndLoadMessage4, 0);
					while (PlayState.WaitingForPlayers)
					{
						Thread.Sleep(100);
					}
					PlayState.WaitingForPlayers = true;
				}
			}
			if (this.mNextSceneTransition != Transitions.Invalid)
			{
				RenderManager.Instance.EndTransition(this.mNextSceneTransition, Color.Black, this.mNextSceneTransitionTime);
			}
			if (this.mNextSceneTransition != Transitions.Invalid)
			{
				StaticList<Entity> entities = this.PlayState.EntityManager.Entities;
				if (entities != null && entities.Count > 0)
				{
					for (int i = 0; i < entities.Count; i++)
					{
						Magicka.GameLogic.Entities.Character character = entities[i] as Magicka.GameLogic.Entities.Character;
						if (character != null && character is Avatar)
						{
							Avatar avatar = character as Avatar;
							if (avatar != null && avatar.Player != null)
							{
								StaticList<Spell> spellQueue = avatar.Player.SpellQueue;
								if (spellQueue != null && spellQueue.Count > 0)
								{
									foreach (Spell spell in spellQueue)
									{
										ChantSpells chantSpells = new ChantSpells(spell.Element, character);
										ChantSpellManager.Add(ref chantSpells);
									}
								}
							}
						}
					}
				}
			}
			this.mBusy = false;
		}

		// Token: 0x06000029 RID: 41 RVA: 0x000032D4 File Offset: 0x000014D4
		public void Dispose()
		{
			foreach (GameScene gameScene in this.mScenes.Values)
			{
				gameScene.Dispose();
			}
			this.mHasTitleDisplaying = false;
			if (this.mTitleRenderer != null)
			{
				this.mTitleRenderer.Clear();
			}
			this.mTitleRenderer = null;
			this.mScenes = null;
			this.mCurrentScene = null;
			this.mNextScene = null;
			this.mDialogs = null;
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600002A RID: 42 RVA: 0x00003368 File Offset: 0x00001568
		public bool Busy
		{
			get
			{
				return this.mBusy;
			}
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00003370 File Offset: 0x00001570
		internal GameScene GetScene(int iScene)
		{
			if (this.mScenes == null || !this.mScenes.ContainsKey(iScene))
			{
				return null;
			}
			return this.mScenes[iScene];
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x0600002C RID: 44 RVA: 0x00003396 File Offset: 0x00001596
		public bool NoItems
		{
			get
			{
				return this.mNoItems;
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x0600002D RID: 45 RVA: 0x0000339E File Offset: 0x0000159E
		public int DisplayName
		{
			get
			{
				return this.mDisplayName;
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x0600002E RID: 46 RVA: 0x000033A6 File Offset: 0x000015A6
		public int Description
		{
			get
			{
				return this.mDescription;
			}
		}

		// Token: 0x0600002F RID: 47 RVA: 0x000033AE File Offset: 0x000015AE
		internal void ClearTransition()
		{
			this.mPlayState.IgnoreInitFade = true;
			this.mNextSceneTransition = Transitions.Invalid;
		}

		// Token: 0x06000030 RID: 48 RVA: 0x000033C4 File Offset: 0x000015C4
		public void DisplayTitle(string iTitle, string iSubTitle, float iDisplayTime, float iFadeIn, float iFadeOut, TextAlign iTextAlignment)
		{
			if (this.mHasTitleDisplaying)
			{
				return;
			}
			if (this.mTitleRenderer != null)
			{
				this.mTitleRenderer.Clear();
			}
			this.mTitleRenderer = new TitleRenderer(iTextAlignment);
			this.mTitleRenderer.SetTitles(iTitle, iSubTitle, iDisplayTime, iFadeIn, iFadeOut);
			this.mTitleRenderer.Start();
			this.mHasTitleDisplaying = true;
		}

		// Token: 0x06000031 RID: 49 RVA: 0x0000341E File Offset: 0x0000161E
		public void ClearDisplayTitles()
		{
			this.mHasTitleDisplaying = false;
			if (this.mTitleRenderer != null)
			{
				this.mTitleRenderer.Clear();
			}
		}

		// Token: 0x04000030 RID: 48
		private string mName;

		// Token: 0x04000031 RID: 49
		private Dictionary<int, int> mCounters = new Dictionary<int, int>(32);

		// Token: 0x04000032 RID: 50
		private List<Level.TimerBody> mTimers = new List<Level.TimerBody>();

		// Token: 0x04000033 RID: 51
		private List<Level.TimerBody> mPlayedTimers = new List<Level.TimerBody>();

		// Token: 0x04000034 RID: 52
		private PlayState mPlayState;

		// Token: 0x04000035 RID: 53
		private Dictionary<int, GameScene> mScenes = new Dictionary<int, GameScene>();

		// Token: 0x04000036 RID: 54
		private GameScene mCurrentScene;

		// Token: 0x04000037 RID: 55
		private SpawnPoint mSpawnPoint;

		// Token: 0x04000038 RID: 56
		private GameScene mNextScene;

		// Token: 0x04000039 RID: 57
		private Transitions mNextSceneTransition;

		// Token: 0x0400003A RID: 58
		private float mNextSceneTransitionTime;

		// Token: 0x0400003B RID: 59
		private bool mNextSceneSaveNPCs;

		// Token: 0x0400003C RID: 60
		private Action mNextSceneDoneCallback;

		// Token: 0x0400003D RID: 61
		private Action mOnComplete;

		// Token: 0x0400003E RID: 62
		private SpawnPoint mNextSceneSpawnPoint;

		// Token: 0x0400003F RID: 63
		private bool mNoItems;

		// Token: 0x04000040 RID: 64
		private bool mForceCamera;

		// Token: 0x04000041 RID: 65
		private bool mSpawnFairy = true;

		// Token: 0x04000042 RID: 66
		private bool mBusy;

		// Token: 0x04000043 RID: 67
		private Level.AvatarItem[] mAdditionalAvatarItems;

		// Token: 0x04000044 RID: 68
		private DialogCollection mDialogs;

		// Token: 0x04000045 RID: 69
		private int mDescription;

		// Token: 0x04000046 RID: 70
		private int mDisplayName;

		// Token: 0x04000047 RID: 71
		private byte[] mShaHash;

		// Token: 0x04000048 RID: 72
		private byte[] mDialogHash;

		// Token: 0x04000049 RID: 73
		private TitleRenderer mTitleRenderer;

		// Token: 0x0400004A RID: 74
		private bool mHasTitleDisplaying;

		// Token: 0x0400004B RID: 75
		private VersusRuleset.Settings mSettings;

		// Token: 0x0400004C RID: 76
		private Action<float> gotoSceneLoadingDelegate;

		// Token: 0x0200000C RID: 12
		public class State
		{
			// Token: 0x06000032 RID: 50 RVA: 0x0000343C File Offset: 0x0000163C
			public State(Level iLevel)
			{
				this.mLevel = iLevel;
				this.mCounters = new Dictionary<int, int>(10);
				this.mTimers = new List<Level.TimerBody>();
				this.mPlayedTimers = new List<Level.TimerBody>();
				this.mInactiveScenes = new GameScene.State[this.mLevel.mScenes.Count];
				int num = 0;
				foreach (GameScene iScene in this.mLevel.mScenes.Values)
				{
					this.mInactiveScenes[num++] = new GameScene.State(iScene);
				}
				this.UpdateState();
			}

			// Token: 0x06000033 RID: 51 RVA: 0x000034F8 File Offset: 0x000016F8
			public void UpdateState()
			{
				this.mCurrentScene = this.mLevel.mCurrentScene;
				this.mNextScene = this.mLevel.mNextScene;
				this.mTimers.Clear();
				for (int i = 0; i < this.mLevel.mTimers.Count; i++)
				{
					this.mTimers.Add(this.mLevel.mTimers[i]);
				}
				this.mPlayedTimers.Clear();
				for (int j = 0; j < this.mLevel.mPlayedTimers.Count; j++)
				{
					this.mPlayedTimers.Add(this.mLevel.mPlayedTimers[j]);
				}
				this.mCounters.Clear();
				foreach (KeyValuePair<int, int> keyValuePair in this.mLevel.mCounters)
				{
					this.mCounters[keyValuePair.Key] = keyValuePair.Value;
				}
				for (int k = 0; k < this.mInactiveScenes.Length; k++)
				{
					this.mInactiveScenes[k].UpdateState();
				}
			}

			// Token: 0x06000034 RID: 52 RVA: 0x00003634 File Offset: 0x00001834
			public void ApplyState(List<int> iIgnoredTriggers)
			{
				this.ApplyState(iIgnoredTriggers, null);
			}

			// Token: 0x06000035 RID: 53 RVA: 0x00003640 File Offset: 0x00001840
			public void ApplyState(List<int> iIgnoredTriggers, Action<float> reportBack)
			{
				this.mLevel.mBusy = true;
				while (!this.mLevel.mPlayState.Busy)
				{
					Thread.Sleep(1);
				}
				GameScene gameScene = this.mLevel.mCurrentScene;
				if (this.mLevel.mCurrentScene != this.mCurrentScene && gameScene != null)
				{
					gameScene.Destroy(false);
				}
				foreach (GameScene gameScene2 in this.mLevel.mScenes.Values)
				{
					if (gameScene2.RuleSet != null)
					{
						gameScene2.RuleSet.Initialize();
					}
				}
				for (int i = 0; i < this.mInactiveScenes.Length; i++)
				{
					if (this.mInactiveScenes[i].Scene == this.mCurrentScene)
					{
						this.mInactiveScenes[i].ApplyState(iIgnoredTriggers);
					}
					else
					{
						this.mInactiveScenes[i].ApplyState(null);
					}
				}
				if (this.mLevel.mCurrentScene != this.mCurrentScene)
				{
					this.mLevel.mCurrentScene = this.mCurrentScene;
					if (this.mLevel.mCurrentScene != null)
					{
						this.mLevel.mCurrentScene.LoadLevel();
					}
					if (gameScene != null)
					{
						gameScene.UnloadContent();
					}
					SpawnPoint iSpawnPoint = default(SpawnPoint);
					if (this.mLevel.mCurrentScene != null)
					{
						this.mLevel.mCurrentScene.Initialize(iSpawnPoint, true, reportBack);
					}
				}
				else
				{
					ParticleSystem.Instance.Clear();
					ParticleLightBatcher.Instance.Clear();
					PointLightBatcher.Instance.Clear();
					this.mLevel.mCurrentScene.RestoreSavedAnimations();
					this.mLevel.mCurrentScene.RestoreDynamicLights();
					this.mLevel.mCurrentScene.AddSavedEntities();
					this.mLevel.mCurrentScene.RunStartupActions(reportBack);
				}
				reportBack = null;
				this.mLevel.mNextScene = this.mNextScene;
				this.mLevel.mTimers.Clear();
				for (int j = 0; j < this.mTimers.Count; j++)
				{
					this.mLevel.mTimers.Add(this.mTimers[j]);
				}
				this.mLevel.mPlayedTimers.Clear();
				for (int k = 0; k < this.mPlayedTimers.Count; k++)
				{
					this.mLevel.mPlayedTimers.Add(this.mPlayedTimers[k]);
				}
				this.mLevel.mCounters.Clear();
				foreach (KeyValuePair<int, int> keyValuePair in this.mCounters)
				{
					this.mLevel.mCounters[keyValuePair.Key] = keyValuePair.Value;
				}
				if (NetworkManager.Instance.State != NetworkState.Offline)
				{
					NetworkManager.Instance.Interface.Sync();
				}
				PhysicsManager.Instance.Update(0f);
				this.mLevel.mBusy = false;
			}

			// Token: 0x06000036 RID: 54 RVA: 0x00003958 File Offset: 0x00001B58
			internal void Write(BinaryWriter iWriter)
			{
				iWriter.Write((this.mCurrentScene != null) ? this.mCurrentScene.ID : 0);
				iWriter.Write((this.mNextScene != null) ? this.mNextScene.ID : 0);
				iWriter.Write(this.mTimers.Count);
				foreach (Level.TimerBody timerBody in this.mTimers)
				{
					timerBody.Write(iWriter);
				}
				iWriter.Write(this.mPlayedTimers.Count);
				foreach (Level.TimerBody timerBody2 in this.mPlayedTimers)
				{
					timerBody2.Write(iWriter);
				}
				iWriter.Write(this.mCounters.Count);
				foreach (KeyValuePair<int, int> keyValuePair in this.mCounters)
				{
					iWriter.Write(keyValuePair.Key);
					iWriter.Write(keyValuePair.Value);
				}
				iWriter.Write(this.mInactiveScenes.Length);
				for (int i = 0; i < this.mInactiveScenes.Length; i++)
				{
					iWriter.Write(this.mInactiveScenes[i].Scene.ID);
					this.mInactiveScenes[i].Write(iWriter);
				}
			}

			// Token: 0x06000037 RID: 55 RVA: 0x00003AFC File Offset: 0x00001CFC
			internal void Read(BinaryReader iReader)
			{
				int num = iReader.ReadInt32();
				if (num != 0)
				{
					this.mCurrentScene = this.mLevel.GetScene(num);
				}
				else
				{
					this.mCurrentScene = null;
				}
				int num2 = iReader.ReadInt32();
				if (num2 != 0)
				{
					this.mNextScene = this.mLevel.GetScene(num2);
				}
				else
				{
					this.mNextScene = null;
				}
				this.mTimers.Clear();
				int num3 = iReader.ReadInt32();
				for (int i = 0; i < num3; i++)
				{
					this.mTimers.Add(new Level.TimerBody(iReader));
				}
				this.mPlayedTimers.Clear();
				num3 = iReader.ReadInt32();
				for (int j = 0; j < num3; j++)
				{
					this.mPlayedTimers.Add(new Level.TimerBody(iReader));
				}
				this.mCounters.Clear();
				num3 = iReader.ReadInt32();
				for (int k = 0; k < num3; k++)
				{
					int key = iReader.ReadInt32();
					int value = iReader.ReadInt32();
					this.mCounters[key] = value;
				}
				num3 = iReader.ReadInt32();
				for (int l = 0; l < num3; l++)
				{
					int num4 = iReader.ReadInt32();
					if (num4 != this.mInactiveScenes[l].Scene.ID)
					{
						throw new InvalidOperationException();
					}
					this.mInactiveScenes[l].Read(iReader);
				}
			}

			// Token: 0x0400004D RID: 77
			private Level mLevel;

			// Token: 0x0400004E RID: 78
			private Dictionary<int, int> mCounters;

			// Token: 0x0400004F RID: 79
			private List<Level.TimerBody> mTimers;

			// Token: 0x04000050 RID: 80
			private List<Level.TimerBody> mPlayedTimers;

			// Token: 0x04000051 RID: 81
			private GameScene.State[] mInactiveScenes;

			// Token: 0x04000052 RID: 82
			private GameScene mCurrentScene;

			// Token: 0x04000053 RID: 83
			private GameScene mNextScene;

			// Token: 0x04000054 RID: 84
			public Action<float> ProgressReportBackAction;
		}

		// Token: 0x0200000D RID: 13
		private struct TimerBody
		{
			// Token: 0x06000038 RID: 56 RVA: 0x00003C41 File Offset: 0x00001E41
			public TimerBody(int iID, float iTime, bool iPaused)
			{
				this.ID = iID;
				this.Time = iTime;
				this.Paused = iPaused;
			}

			// Token: 0x06000039 RID: 57 RVA: 0x00003C58 File Offset: 0x00001E58
			public TimerBody(BinaryReader iReader)
			{
				this.ID = iReader.ReadInt32();
				this.Time = iReader.ReadSingle();
				this.Paused = iReader.ReadBoolean();
			}

			// Token: 0x0600003A RID: 58 RVA: 0x00003C7E File Offset: 0x00001E7E
			public void Write(BinaryWriter iWriter)
			{
				iWriter.Write(this.ID);
				iWriter.Write(this.Time);
				iWriter.Write(this.Paused);
			}

			// Token: 0x04000055 RID: 85
			public int ID;

			// Token: 0x04000056 RID: 86
			public float Time;

			// Token: 0x04000057 RID: 87
			public bool Paused;
		}

		// Token: 0x0200000E RID: 14
		internal struct AvatarItem
		{
			// Token: 0x04000058 RID: 88
			public Item Item;

			// Token: 0x04000059 RID: 89
			public string Bone;
		}
	}
}
