using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Xml;
using JigLibX.Geometry;
using Magicka.Audio;
using Magicka.GameLogic;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.Abilities.SpecialAbilities;
using Magicka.GameLogic.Entities.CharacterStates;
using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.Statistics;
using Magicka.GameLogic.UI;
using Magicka.Gamers;
using Magicka.Graphics;
using Magicka.Graphics.Effects;
using Magicka.Levels.Packs;
using Magicka.Levels.Triggers;
using Magicka.Localization;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;

namespace Magicka.Levels.Versus
{
	// Token: 0x02000378 RID: 888
	public abstract class VersusRuleset : IRuleset
	{
		// Token: 0x06001B25 RID: 6949 RVA: 0x000B9838 File Offset: 0x000B7A38
		public VersusRuleset(GameScene iScene, XmlNode iNode)
		{
			this.mScene = iScene;
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			List<string> list3 = new List<string>();
			for (int i = 0; i < iNode.ChildNodes.Count; i++)
			{
				XmlNode xmlNode = iNode.ChildNodes[i];
				if (xmlNode.Name.Equals("Areas", StringComparison.OrdinalIgnoreCase))
				{
					for (int j = 0; j < xmlNode.ChildNodes.Count; j++)
					{
						XmlNode xmlNode2 = xmlNode.ChildNodes[j];
						if (xmlNode2.Name.Equals("Area", StringComparison.OrdinalIgnoreCase))
						{
							for (int k = 0; k < xmlNode2.Attributes.Count; k++)
							{
								XmlAttribute xmlAttribute = xmlNode2.Attributes[k];
								if (xmlAttribute.Name.Equals("team", StringComparison.OrdinalIgnoreCase))
								{
									if (xmlAttribute.Value.Equals("B", StringComparison.OrdinalIgnoreCase) || xmlAttribute.Value.Equals("blue", StringComparison.OrdinalIgnoreCase))
									{
										list3.Add(xmlNode2.InnerText);
									}
									else if (xmlAttribute.Value.Equals("a", StringComparison.OrdinalIgnoreCase) || xmlAttribute.Value.Equals("red", StringComparison.OrdinalIgnoreCase))
									{
										list2.Add(xmlNode2.InnerText);
									}
								}
							}
							list.Add(xmlNode2.InnerText);
						}
					}
				}
			}
			this.mAreas_All = new int[list.Count];
			for (int l = 0; l < list.Count; l++)
			{
				this.mAreas_All[l] = list[l].ToLowerInvariant().GetHashCodeCustom();
			}
			this.mAreas_TeamA = new int[list2.Count];
			for (int m = 0; m < list2.Count; m++)
			{
				this.mAreas_TeamA[m] = list2[m].ToLowerInvariant().GetHashCodeCustom();
			}
			this.mAreas_TeamB = new int[list3.Count];
			for (int n = 0; n < list3.Count; n++)
			{
				this.mAreas_TeamB[n] = list3[n].ToLowerInvariant().GetHashCodeCustom();
			}
			this.mItemTemplate = this.mScene.PlayState.Content.Load<CharacterTemplate>("Data/Characters/Luggage_item");
			this.mMagickTemplate = this.mScene.PlayState.Content.Load<CharacterTemplate>("Data/Characters/Luggage_magick");
			this.mItemConditions = new ConditionCollection();
			this.mMagickConditions = new ConditionCollection();
			ItemPack[] itemPacks = PackMan.Instance.ItemPacks;
			for (int num = 0; num < itemPacks.Length; num++)
			{
				if (itemPacks[num].Enabled)
				{
					string[] items = itemPacks[num].Items;
					for (int num2 = 0; num2 < items.Length; num2++)
					{
						this.mScene.PlayState.Content.Load<Item>(items[num2]);
					}
				}
			}
			this.mLuggageItems = new List<int>(8);
			for (int num3 = 0; num3 < itemPacks.Length; num3++)
			{
				if (itemPacks[num3].Enabled)
				{
					this.mLuggageItems.AddRange(itemPacks[num3].ItemIDs);
				}
			}
			MagickPack[] magickPacks = PackMan.Instance.MagickPacks;
			this.mLuggageMagicks = new List<MagickType>(8);
			for (int num4 = 0; num4 < magickPacks.Length; num4++)
			{
				if (magickPacks[num4].Enabled)
				{
					this.mLuggageMagicks.AddRange(magickPacks[num4].Magicks);
				}
			}
			this.mScoreUIs = new List<VersusRuleset.Score>();
			GUIBasicEffect iEffect = null;
			PieEffect pieEffect = null;
			Texture2D iPieTexture = null;
			Texture2D iCountdownTexture = null;
			lock (Game.Instance.GraphicsDevice)
			{
				iEffect = new GUIBasicEffect(RenderManager.Instance.GraphicsDevice, null);
				pieEffect = new PieEffect(RenderManager.Instance.GraphicsDevice, Game.Instance.Content);
				iCountdownTexture = this.mScene.PlayState.Content.Load<Texture2D>("UI/HUD/versus_countdown");
				iPieTexture = Game.Instance.Content.Load<Texture2D>("UI/HUD/hud");
			}
			pieEffect.Radius = 32f;
			pieEffect.MaxAngle = 6.2831855f;
			pieEffect.SetTechnique(PieEffect.Technique.Technique1);
			Point screenSize = RenderManager.Instance.ScreenSize;
			pieEffect.SetScreenSize(screenSize.X, screenSize.Y);
			this.mRenderData = new VersusRuleset.RenderData[3];
			for (int num5 = 0; num5 < 3; num5++)
			{
				this.mRenderData[num5] = new VersusRuleset.RenderData(iEffect, pieEffect, iPieTexture, iCountdownTexture, this.mScoreUIs);
			}
			this.mTemporarySpawns = new List<int>(this.mAreas_All.Length);
			KeyboardHUD.Instance.Reset();
		}

		// Token: 0x06001B26 RID: 6950 RVA: 0x000B9CFC File Offset: 0x000B7EFC
		protected void CountDown(float iTime)
		{
			this.mCountDownTimer = iTime;
			ControlManager.Instance.LimitInput(this.mScene.Level);
		}

		// Token: 0x06001B27 RID: 6951 RVA: 0x000B9D1A File Offset: 0x000B7F1A
		public virtual void OnPlayerKill(Avatar atkAvatar, Avatar tarAvatar)
		{
		}

		// Token: 0x06001B28 RID: 6952 RVA: 0x000B9D1C File Offset: 0x000B7F1C
		public virtual void OnPlayerDeath(Player iPlayer)
		{
		}

		// Token: 0x06001B29 RID: 6953 RVA: 0x000B9D20 File Offset: 0x000B7F20
		protected void EndGame()
		{
			if (this.mGameOver)
			{
				return;
			}
			if (this.mGameClockCue != null && !this.mGameClockCue.IsStopping)
			{
				this.mGameClockCue.Stop(AudioStopOptions.AsAuthored);
			}
			AudioManager.Instance.PlayCue(Banks.Additional, VersusRuleset.SOUND_GAME_END);
			this.mScene.PlayState.Endgame(EndGameCondition.EndOffGame, true, false, 0f);
			if (NetworkManager.Instance.State == NetworkState.Server)
			{
				RulesetMessage rulesetMessage = default(RulesetMessage);
				rulesetMessage.Type = this.RulesetType;
				rulesetMessage.Byte01 = 1;
				NetworkManager.Instance.Interface.SendMessage<RulesetMessage>(ref rulesetMessage);
			}
			this.OnEndGame();
		}

		// Token: 0x06001B2A RID: 6954 RVA: 0x000B9DC5 File Offset: 0x000B7FC5
		public virtual void OnEndGame()
		{
		}

		// Token: 0x06001B2B RID: 6955 RVA: 0x000B9DC8 File Offset: 0x000B7FC8
		public virtual int GetAnyArea()
		{
			int num = VersusRuleset.RANDOM.Next(this.mAreas_All.Length);
			return this.mAreas_All[num];
		}

		// Token: 0x06001B2C RID: 6956 RVA: 0x000B9DF0 File Offset: 0x000B7FF0
		public virtual void Update(float iDeltaTime, DataChannel iDataChannel)
		{
			if (this.mIntroCueTimer < 0.25f)
			{
				this.mIntroCueTimer += iDeltaTime;
				if (this.mIntroCueTimer >= 0.25f)
				{
					AudioManager.Instance.PlayCue(Banks.Additional, VersusRuleset.SOUND_GAME_START);
				}
			}
			for (int i = 0; i < this.mPlayers.Length; i++)
			{
				int num = this.mIDToScoreUILookUp[i];
				if (num != -1 && !this.mPlayers[i].Playing)
				{
					this.mScoreUIs[num].RemovePlayer(i);
					this.mIDToScoreUILookUp[i] = -1;
				}
			}
			if (Game.Instance.PlayerCount <= 1 && !this.mGameOver)
			{
				this.EndGame();
			}
			VersusRuleset.RenderData renderData = this.mRenderData[(int)iDataChannel];
			if (this.mScene.PlayState.IsGameEnded)
			{
				renderData.GameOver = true;
			}
			else
			{
				renderData.GameOver = false;
			}
			if (this.mCountDownTimer > -1f)
			{
				this.mCountDownTimer -= iDeltaTime;
				if (this.mCountDownTimer <= 0f)
				{
					ControlManager.Instance.UnlimitInput(this.mScene.Level);
				}
			}
			renderData.SetCountDown(this.mCountDownTimer);
			this.mScene.Scene.AddRenderableGUIObject(iDataChannel, renderData);
		}

		// Token: 0x06001B2D RID: 6957 RVA: 0x000B9F30 File Offset: 0x000B8130
		public virtual void LocalUpdate(float iDeltaTime, DataChannel iDataChannel)
		{
			if (this.mIntroCueTimer < 0.25f)
			{
				this.mIntroCueTimer += iDeltaTime;
				if (this.mIntroCueTimer >= 0.25f)
				{
					AudioManager.Instance.PlayCue(Banks.Additional, VersusRuleset.SOUND_GAME_START);
				}
			}
			VersusRuleset.RenderData renderData = this.mRenderData[(int)iDataChannel];
			if (this.mCountDownTimer > -1f)
			{
				this.mCountDownTimer -= iDeltaTime;
				if (this.mCountDownTimer <= 0f)
				{
					ControlManager.Instance.UnlimitInput(this.mScene.Level);
				}
			}
			renderData.SetCountDown(this.mCountDownTimer);
			this.mScene.Scene.AddRenderableGUIObject(iDataChannel, renderData);
		}

		// Token: 0x06001B2E RID: 6958 RVA: 0x000B9FE0 File Offset: 0x000B81E0
		public virtual void NetworkUpdate(ref RulesetMessage iMsg)
		{
			switch (iMsg.Byte01)
			{
			case 1:
				this.EndGame();
				break;
			case 2:
				break;
			case 3:
				if (this.mPlayers[(int)iMsg.Byte02].Playing)
				{
					Matrix matrix;
					this.GetMatrix(iMsg.Integer01, out matrix);
					if (this.mPlayers[(int)iMsg.Byte02].Avatar == null || this.mPlayers[(int)iMsg.Byte02].Avatar.Dead)
					{
						this.RevivePlayer((int)iMsg.Byte02, iMsg.Integer01, ref matrix, new ushort?(iMsg.UShort01));
						return;
					}
					this.MovePlayer((int)iMsg.Byte02, ref matrix);
					this.mPlayers[(int)iMsg.Byte02].Avatar.HitPoints = this.mPlayers[(int)iMsg.Byte02].Avatar.MaxHitPoints;
					this.mPlayers[(int)iMsg.Byte02].Avatar.StopStatusEffects(StatusEffects.Burning | StatusEffects.Wet | StatusEffects.Frozen | StatusEffects.Cold | StatusEffects.Poisoned | StatusEffects.Healing | StatusEffects.Greased | StatusEffects.Steamed);
					return;
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x06001B2F RID: 6959 RVA: 0x000BA0DC File Offset: 0x000B82DC
		public virtual void Initialize()
		{
			this.mScoreUIs.Clear();
			this.mPlayers = Game.Instance.Players;
			for (int i = 0; i < this.mPlayers.Length; i++)
			{
				this.mPlayers[i].UnlockedMagicks = 0UL;
			}
			StatisticsManager.Instance.SurvivalReset();
			for (int j = 0; j < this.mPlayers.Length; j++)
			{
				if (this.mPlayers[j].Playing)
				{
					this.mPlayers[j].Avatar.Faction &= ~Factions.FRIENDLY;
				}
				this.mIDToScoreUILookUp[j] = -1;
			}
			if (this.mScene.Level.CurrentScene.Indoors)
			{
				for (int k = 0; k < this.mLuggageMagicks.Count; k++)
				{
					if (this.mLuggageMagicks[k] == MagickType.Blizzard | this.mLuggageMagicks[k] == MagickType.MeteorS | this.mLuggageMagicks[k] == MagickType.Napalm | this.mLuggageMagicks[k] == MagickType.Rain | this.mLuggageMagicks[k] == MagickType.SPhoenix | this.mLuggageMagicks[k] == MagickType.ThunderB | this.mLuggageMagicks[k] == MagickType.ThunderS)
					{
						this.mLuggageMagicks.RemoveAt(k--);
					}
				}
			}
			this.mGameOver = false;
			this.CountDown(4f);
			if (this.mGameClockCue != null && !this.mGameClockCue.IsStopping)
			{
				this.mGameClockCue.Stop(AudioStopOptions.AsAuthored);
			}
			this.mIntroCueTimer = 0f;
		}

		// Token: 0x06001B30 RID: 6960
		public abstract void DeInitialize();

		// Token: 0x170006A5 RID: 1701
		// (get) Token: 0x06001B31 RID: 6961
		public abstract Rulesets RulesetType { get; }

		// Token: 0x06001B32 RID: 6962 RVA: 0x000BA275 File Offset: 0x000B8475
		public int GetTeamArea(Factions iFaction)
		{
			if ((iFaction & Factions.TEAM_RED) == Factions.TEAM_RED)
			{
				return this.GetTeamArea(0);
			}
			if ((iFaction & Factions.TEAM_BLUE) == Factions.TEAM_BLUE)
			{
				return this.GetTeamArea(1);
			}
			return this.GetAnyArea();
		}

		// Token: 0x06001B33 RID: 6963 RVA: 0x000BA2AC File Offset: 0x000B84AC
		protected virtual int GetTeamArea(int iTeam)
		{
			if (iTeam == 0)
			{
				return this.mAreas_TeamA[VersusRuleset.RANDOM.Next(this.mAreas_TeamA.Length)];
			}
			if (1 == iTeam)
			{
				return this.mAreas_TeamB[VersusRuleset.RANDOM.Next(this.mAreas_TeamB.Length)];
			}
			return this.GetAnyArea();
		}

		// Token: 0x06001B34 RID: 6964 RVA: 0x000BA2FC File Offset: 0x000B84FC
		protected bool GetMatrix(int iArea, out Matrix oOrientation)
		{
			oOrientation = Matrix.Identity;
			Matrix identity;
			if (!this.mScene.TryGetLocator(iArea, out identity))
			{
				identity = Matrix.Identity;
				TriggerArea triggerArea;
				if (!this.mScene.TryGetTriggerArea(iArea, out triggerArea))
				{
					return false;
				}
				identity.Translation = triggerArea.GetRandomLocation();
			}
			Segment iSeg = default(Segment);
			iSeg.Origin = identity.Translation;
			iSeg.Origin.Y = iSeg.Origin.Y + 1f;
			iSeg.Delta.Y = -3f;
			float num;
			Vector3 translation;
			Vector3 vector;
			if (!this.mScene.Level.CurrentScene.SegmentIntersect(out num, out translation, out vector, iSeg))
			{
				bool flag = false;
				for (int i = 0; i < this.mScene.Level.CurrentScene.Liquids.Length; i++)
				{
					if (this.mScene.Level.CurrentScene.Liquids[i].SegmentIntersect(out num, out translation, out vector, ref iSeg, true, true, false))
					{
						identity.Translation = translation;
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			else
			{
				identity.Translation = translation;
			}
			oOrientation = identity;
			return true;
		}

		// Token: 0x06001B35 RID: 6965 RVA: 0x000BA420 File Offset: 0x000B8620
		protected void SetupPlayer(int iID, int iArea)
		{
			Matrix matrix;
			this.GetMatrix(iArea, out matrix);
			if (this.mPlayers[iID].Avatar != null && !this.mPlayers[iID].Avatar.Dead)
			{
				if (!(this.mPlayers[iID].Gamer is NetworkGamer))
				{
					this.MovePlayer(iID, ref matrix);
					this.mPlayers[iID].Avatar.HitPoints = this.mPlayers[iID].Avatar.MaxHitPoints;
					this.mPlayers[iID].Avatar.StopStatusEffects(StatusEffects.Burning | StatusEffects.Wet | StatusEffects.Frozen | StatusEffects.Cold | StatusEffects.Poisoned | StatusEffects.Healing | StatusEffects.Greased | StatusEffects.Steamed);
					return;
				}
				if (NetworkManager.Instance.State == NetworkState.Server)
				{
					RulesetMessage rulesetMessage = default(RulesetMessage);
					rulesetMessage.Type = this.RulesetType;
					rulesetMessage.Byte01 = 3;
					rulesetMessage.Byte02 = (byte)iID;
					rulesetMessage.Integer01 = iArea;
					rulesetMessage.UShort01 = this.mPlayers[iID].Avatar.Handle;
					NetworkManager.Instance.Interface.SendMessage<RulesetMessage>(ref rulesetMessage);
					return;
				}
			}
			else if (NetworkManager.Instance.State != NetworkState.Client)
			{
				ushort @ushort = this.RevivePlayer(iID, iArea, ref matrix, null);
				if (NetworkManager.Instance.State == NetworkState.Server)
				{
					RulesetMessage rulesetMessage2 = default(RulesetMessage);
					rulesetMessage2.Type = this.RulesetType;
					rulesetMessage2.Byte01 = 3;
					rulesetMessage2.Byte02 = (byte)iID;
					rulesetMessage2.Integer01 = iArea;
					rulesetMessage2.UShort01 = @ushort;
					NetworkManager.Instance.Interface.SendMessage<RulesetMessage>(ref rulesetMessage2);
				}
			}
		}

		// Token: 0x06001B36 RID: 6966 RVA: 0x000BA598 File Offset: 0x000B8798
		protected void MovePlayer(int iID, ref Matrix iTransform)
		{
			Avatar avatar = this.mPlayers[iID].Avatar;
			Vector3 translation = iTransform.Translation;
			iTransform.Translation = default(Vector3);
			translation.Y += avatar.Capsule.Length * 0.5f + avatar.Radius + 0.15f;
			avatar.Body.MoveTo(translation, iTransform);
			avatar.Body.SetActive();
			avatar.StopAllActions();
			avatar.ResetSpell();
			if (avatar.Player != null)
			{
				avatar.Player.IconRenderer.Clear();
			}
			avatar.SpellQueue.Clear();
			avatar.CastType = CastType.None;
			avatar.ChangeState(IdleState.Instance);
			avatar.ResetRestingTimers();
		}

		// Token: 0x06001B37 RID: 6967 RVA: 0x000BA65C File Offset: 0x000B885C
		protected ushort RevivePlayer(int iID, int iArea, ref Matrix iTransform, ushort? iHandle)
		{
			Vector3 translation = iTransform.Translation;
			iTransform.Translation = default(Vector3);
			Vector3 vector = new Vector3(2f, 0f, 0f);
			Damage damage = new Damage(AttackProperties.Status, Elements.Cold, 100f, 4f);
			Liquid.Freeze(this.mScene.Level.CurrentScene, ref translation, ref vector, 6.2831855f, 2f, ref damage);
			CharacterTemplate cachedTemplate = CharacterTemplate.GetCachedTemplate(this.mPlayers[iID].Gamer.Avatar.Type);
			translation.Y += cachedTemplate.Length * 0.5f + cachedTemplate.Radius + 0.15f;
			this.mPlayers[iID].Weapon = null;
			this.mPlayers[iID].Staff = null;
			Avatar fromCache;
			if (iHandle != null)
			{
				fromCache = Avatar.GetFromCache(this.mPlayers[iID], iHandle.Value);
			}
			else
			{
				fromCache = Avatar.GetFromCache(this.mPlayers[iID]);
			}
			fromCache.Initialize(cachedTemplate, translation, Player.UNIQUE_ID[iID]);
			fromCache.TimedEthereal(0f, true);
			fromCache.TimedEthereal(2.25f, false);
			fromCache.Body.MoveTo(translation, iTransform);
			fromCache.Faction &= ~Factions.FRIENDLY;
			fromCache.HitPoints = fromCache.MaxHitPoints;
			fromCache.SpawnAnimation = Animations.revive;
			fromCache.ChangeState(RessurectionState.Instance);
			this.mScene.PlayState.EntityManager.AddEntity(fromCache);
			AudioManager.Instance.PlayCue(Banks.Spells, Revive.SOUNDHASH, fromCache.AudioEmitter);
			this.mPlayers[iID].Avatar = fromCache;
			this.mPlayers[iID].Ressing = false;
			if (this.mPlayers[iID].Controller is XInputController)
			{
				(this.mPlayers[iID].Controller as XInputController).Rumble(2f, 2f);
			}
			return fromCache.Handle;
		}

		// Token: 0x06001B38 RID: 6968 RVA: 0x000BA86C File Offset: 0x000B8A6C
		protected void SpawnLuggage(bool iSpawnItem)
		{
			CharacterTemplate characterTemplate;
			ConditionCollection conditionCollection;
			if (iSpawnItem && this.mLuggageItems.Count > 0)
			{
				characterTemplate = this.mItemTemplate;
				conditionCollection = this.mItemConditions;
				int index = VersusRuleset.RANDOM.Next(this.mLuggageItems.Count);
				int iType = this.mLuggageItems[index];
				conditionCollection[0].Clear();
				conditionCollection[0].Add(new EventStorage(new SpawnItemEvent(iType, 12f)));
				conditionCollection[1].Clear();
				conditionCollection[1].Add(new EventStorage(new SpawnItemEvent(iType, 12f)));
			}
			else
			{
				if (this.mLuggageMagicks.Count <= 0)
				{
					return;
				}
				characterTemplate = this.mMagickTemplate;
				conditionCollection = this.mMagickConditions;
				MagickType iType2 = this.mLuggageMagicks[VersusRuleset.RANDOM.Next(this.mLuggageMagicks.Count)];
				conditionCollection[0].Clear();
				conditionCollection[0].Add(new EventStorage(new SpawnMagickEvent(iType2, 12f)));
				conditionCollection[1].Clear();
				conditionCollection[1].Add(new EventStorage(new SpawnMagickEvent(iType2, 12f)));
			}
			conditionCollection[0].Condition.Repeat = false;
			conditionCollection[0].Condition.Count = 1;
			conditionCollection[0].Condition.Activated = false;
			conditionCollection[0].Condition.EventConditionType = EventConditionType.Death;
			conditionCollection[1].Condition.Repeat = false;
			conditionCollection[1].Condition.Count = 1;
			conditionCollection[1].Condition.Activated = false;
			conditionCollection[1].Condition.EventConditionType = EventConditionType.OverKill;
			int anyArea = this.GetAnyArea();
			Matrix identity;
			if (!this.mScene.TryGetLocator(anyArea, out identity))
			{
				identity = Matrix.Identity;
				TriggerArea triggerArea;
				if (this.mScene.TryGetTriggerArea(anyArea, out triggerArea))
				{
					identity.Translation = triggerArea.GetRandomLocation();
				}
			}
			Vector3 translation = identity.Translation;
			translation.Y += characterTemplate.Length * 0.5f + characterTemplate.Radius;
			identity.Translation = default(Vector3);
			NonPlayerCharacter instance = NonPlayerCharacter.GetInstance(this.mScene.PlayState);
			instance.Initialize(characterTemplate, translation, 0);
			instance.Body.Orientation = identity;
			instance.CharacterBody.DesiredDirection = identity.Forward;
			instance.SpawnAnimation = Animations.spawn;
			instance.ChangeState(RessurectionState.Instance);
			instance.Faction = Factions.EVIL;
			this.mScene.PlayState.EntityManager.AddEntity(instance);
			instance.EventConditions = conditionCollection;
			identity.Translation = translation;
			VisualEffectReference visualEffectReference;
			EffectManager.Instance.StartEffect("luggage_spawn".GetHashCodeCustom(), ref identity, out visualEffectReference);
			if (NetworkManager.Instance.State == NetworkState.Server)
			{
				TriggerActionMessage triggerActionMessage = default(TriggerActionMessage);
				triggerActionMessage.ActionType = TriggerActionType.SpawnLuggage;
				triggerActionMessage.Handle = instance.Handle;
				triggerActionMessage.Template = instance.Template.ID;
				triggerActionMessage.Position = instance.Position;
				triggerActionMessage.Direction = instance.Direction;
				triggerActionMessage.Point0 = 170;
				NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref triggerActionMessage);
			}
		}

		// Token: 0x170006A6 RID: 1702
		// (get) Token: 0x06001B39 RID: 6969 RVA: 0x000BABD3 File Offset: 0x000B8DD3
		public virtual bool DropMagicks
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06001B3A RID: 6970
		public abstract bool CanRevive(Player iReviver, Player iRevivee);

		// Token: 0x06001B3B RID: 6971
		internal abstract short[] GetTeamScores();

		// Token: 0x06001B3C RID: 6972
		internal abstract short[] GetScores();

		// Token: 0x170006A7 RID: 1703
		// (get) Token: 0x06001B3D RID: 6973
		internal abstract bool Teams { get; }

		// Token: 0x04001D64 RID: 7524
		internal const int TEAM_A_RED = 0;

		// Token: 0x04001D65 RID: 7525
		internal const int TEAM_B_BLUE = 1;

		// Token: 0x04001D66 RID: 7526
		protected const float COUNTDOWN = 4f;

		// Token: 0x04001D67 RID: 7527
		internal static readonly int LOC_TEAMRED = "#add_team_red".GetHashCodeCustom();

		// Token: 0x04001D68 RID: 7528
		internal static readonly int LOC_TEAMBLUE = "#add_team_blue".GetHashCodeCustom();

		// Token: 0x04001D69 RID: 7529
		internal static readonly int LOC_GAME_OVER = "#game_over".GetHashCodeCustom();

		// Token: 0x04001D6A RID: 7530
		internal static readonly int SOUND_GAME_END = "game_end".GetHashCodeCustom();

		// Token: 0x04001D6B RID: 7531
		internal static readonly int SOUND_GAME_START = "game_start".GetHashCodeCustom();

		// Token: 0x04001D6C RID: 7532
		internal static readonly int SOUND_GAME_CLOCK = "game_clock".GetHashCodeCustom();

		// Token: 0x04001D6D RID: 7533
		protected static Random RANDOM = new Random();

		// Token: 0x04001D6E RID: 7534
		protected GameScene mScene;

		// Token: 0x04001D6F RID: 7535
		protected int[] mAreas_All;

		// Token: 0x04001D70 RID: 7536
		protected int[] mAreas_TeamA;

		// Token: 0x04001D71 RID: 7537
		protected int[] mAreas_TeamB;

		// Token: 0x04001D72 RID: 7538
		protected VersusRuleset.RenderData[] mRenderData;

		// Token: 0x04001D73 RID: 7539
		protected List<VersusRuleset.Score> mScoreUIs;

		// Token: 0x04001D74 RID: 7540
		protected Player[] mPlayers;

		// Token: 0x04001D75 RID: 7541
		protected List<int> mTemporarySpawns;

		// Token: 0x04001D76 RID: 7542
		protected Dictionary<int, int> mIDToScoreUILookUp = new Dictionary<int, int>(4);

		// Token: 0x04001D77 RID: 7543
		protected float mCountDownTimer;

		// Token: 0x04001D78 RID: 7544
		protected CharacterTemplate mItemTemplate;

		// Token: 0x04001D79 RID: 7545
		protected CharacterTemplate mMagickTemplate;

		// Token: 0x04001D7A RID: 7546
		private ConditionCollection mItemConditions;

		// Token: 0x04001D7B RID: 7547
		private ConditionCollection mMagickConditions;

		// Token: 0x04001D7C RID: 7548
		private List<int> mLuggageItems;

		// Token: 0x04001D7D RID: 7549
		private List<MagickType> mLuggageMagicks;

		// Token: 0x04001D7E RID: 7550
		protected Cue mGameClockCue;

		// Token: 0x04001D7F RID: 7551
		protected float mIntroCueTimer;

		// Token: 0x04001D80 RID: 7552
		protected bool mGameOver;

		// Token: 0x02000379 RID: 889
		internal abstract class Settings
		{
			// Token: 0x14000011 RID: 17
			// (add) Token: 0x06001B3F RID: 6975 RVA: 0x000BAC49 File Offset: 0x000B8E49
			// (remove) Token: 0x06001B40 RID: 6976 RVA: 0x000BAC62 File Offset: 0x000B8E62
			public event VersusRuleset.Settings.SettingChanged Changed;

			// Token: 0x06001B41 RID: 6977 RVA: 0x000BAC7C File Offset: 0x000B8E7C
			protected Settings()
			{
				this.mMenuItems = new List<DropDownBox>();
				this.mMenuTitles = new List<int>();
				this.mToolTips = new List<int>();
			}

			// Token: 0x06001B42 RID: 6978 RVA: 0x000BADE0 File Offset: 0x000B8FE0
			public DropDownBox<T> AddOption<T>(int iTitle, int iToolTip, T[] iValues, int?[] iLocalization)
			{
				DropDownBox<T> dropDownBox = new DropDownBox<T>(FontManager.Instance.GetFont(MagickaFont.MenuOption), iValues, iLocalization, 200);
				dropDownBox.SelectedIndexChanged += new Action<DropDownBox, int>(this.OnChanged);
				this.mMenuItems.Add(dropDownBox);
				this.mMenuTitles.Add(iTitle);
				this.mToolTips.Add(iToolTip);
				return dropDownBox;
			}

			// Token: 0x170006A8 RID: 1704
			// (get) Token: 0x06001B43 RID: 6979 RVA: 0x000BAE3D File Offset: 0x000B903D
			public IList<DropDownBox> MenuItems
			{
				get
				{
					return this.mMenuItems;
				}
			}

			// Token: 0x170006A9 RID: 1705
			// (get) Token: 0x06001B44 RID: 6980 RVA: 0x000BAE45 File Offset: 0x000B9045
			public IList<int> MenuTitles
			{
				get
				{
					return this.mMenuTitles;
				}
			}

			// Token: 0x170006AA RID: 1706
			// (get) Token: 0x06001B45 RID: 6981 RVA: 0x000BAE4D File Offset: 0x000B904D
			public IList<int> ToolTips
			{
				get
				{
					return this.mToolTips;
				}
			}

			// Token: 0x170006AB RID: 1707
			// (get) Token: 0x06001B46 RID: 6982
			public abstract bool TeamsEnabled { get; }

			// Token: 0x06001B47 RID: 6983 RVA: 0x000BAE55 File Offset: 0x000B9055
			protected void OnChanged(DropDownBox iBox, int iNewIndex)
			{
				if (this.Changed != null)
				{
					this.Changed(this.mMenuItems.IndexOf(iBox), iNewIndex);
				}
			}

			// Token: 0x06001B48 RID: 6984 RVA: 0x000BAE78 File Offset: 0x000B9078
			public unsafe void GetMessage(out VersusRuleset.Settings.OptionsMessage oMessage)
			{
				if (this is DeathMatch.Settings)
				{
					oMessage.Ruleset = Rulesets.DeathMatch;
				}
				else if (this is Brawl.Settings)
				{
					oMessage.Ruleset = Rulesets.Brawl;
				}
				else if (this is Krietor.Settings)
				{
					oMessage.Ruleset = Rulesets.Kreitor;
				}
				else if (this is King.Settings)
				{
					oMessage.Ruleset = Rulesets.King;
				}
				else
				{
					if (!(this is Pyrite.Settings))
					{
						throw new NotImplementedException();
					}
					oMessage.Ruleset = Rulesets.Pyrite;
				}
				oMessage.NrOfSettings = (byte)this.mMenuItems.Count;
				fixed (byte* ptr = &oMessage.Settings.FixedElementField)
				{
					for (int i = 0; i < this.mMenuItems.Count; i++)
					{
						ptr[i] = (byte)this.mMenuItems[i].SelectedIndex;
					}
				}
			}

			// Token: 0x06001B49 RID: 6985 RVA: 0x000BAF30 File Offset: 0x000B9130
			public unsafe static void ApplyMessage(ref VersusRuleset.Settings iSettings, ref VersusRuleset.Settings.OptionsMessage iMessage)
			{
				switch (iMessage.Ruleset)
				{
				case Rulesets.DeathMatch:
					if (!(iSettings is DeathMatch.Settings))
					{
						iSettings = new DeathMatch.Settings();
					}
					break;
				case Rulesets.Brawl:
					if (!(iSettings is Brawl.Settings))
					{
						iSettings = new Brawl.Settings();
					}
					break;
				case Rulesets.Pyrite:
					if (!(iSettings is Pyrite.Settings))
					{
						iSettings = new Pyrite.Settings();
					}
					break;
				case Rulesets.Kreitor:
					if (!(iSettings is Krietor.Settings))
					{
						iSettings = new Krietor.Settings();
					}
					break;
				case Rulesets.King:
					if (!(iSettings is King.Settings))
					{
						iSettings = new King.Settings();
					}
					break;
				default:
					iSettings = null;
					Console.WriteLine("Invalid Ruleset type: " + iMessage.Ruleset);
					return;
				}
				fixed (byte* ptr = &iMessage.Settings.FixedElementField)
				{
					for (int i = 0; i < iSettings.mMenuItems.Count; i++)
					{
						iSettings.mMenuItems[i].SelectedIndex = (int)ptr[i];
					}
				}
			}

			// Token: 0x04001D81 RID: 7553
			public const int DROPDOWNBOX_WIDTH = 200;

			// Token: 0x04001D83 RID: 7555
			protected int LOC_TIME_LIMIT = "#menu_vs_03".GetHashCodeCustom();

			// Token: 0x04001D84 RID: 7556
			protected int LOC_SCORE_LIMIT = "#menu_vs_04".GetHashCodeCustom();

			// Token: 0x04001D85 RID: 7557
			protected int LOC_UNLIMITED = "#menu_opt_alt_unlimited".GetHashCodeCustom();

			// Token: 0x04001D86 RID: 7558
			protected int LOC_NEVER = "#menu_opt_alt_never".GetHashCodeCustom();

			// Token: 0x04001D87 RID: 7559
			protected int LOC_TEAMS = "#menu_vs_09".GetHashCodeCustom();

			// Token: 0x04001D88 RID: 7560
			protected int LOC_OFF = "#menu_opt_alt_02".GetHashCodeCustom();

			// Token: 0x04001D89 RID: 7561
			protected int LOC_NO = "#add_menu_no".GetHashCodeCustom();

			// Token: 0x04001D8A RID: 7562
			protected int LOC_YES = "#add_menu_yes".GetHashCodeCustom();

			// Token: 0x04001D8B RID: 7563
			protected int LOC_LIVES = "#opt_vs_lives".GetHashCodeCustom();

			// Token: 0x04001D8C RID: 7564
			protected int LOC_LUGGAGE_INTERVAL = "#opt_vs_luggageinterval".GetHashCodeCustom();

			// Token: 0x04001D8D RID: 7565
			protected int LOC_LOW = "#menu_opt_alt_04".GetHashCodeCustom();

			// Token: 0x04001D8E RID: 7566
			protected int LOC_MEDIUM = "#menu_opt_alt_05".GetHashCodeCustom();

			// Token: 0x04001D8F RID: 7567
			protected int LOC_HIGH = "#menu_opt_alt_06".GetHashCodeCustom();

			// Token: 0x04001D90 RID: 7568
			protected int LOC_NONE = "#menu_opt_alt_09".GetHashCodeCustom();

			// Token: 0x04001D91 RID: 7569
			protected int LOC_TT_TIME = "#tooltip_vs_time".GetHashCodeCustom();

			// Token: 0x04001D92 RID: 7570
			protected int LOC_TT_SCORE = "#tooltip_vs_score".GetHashCodeCustom();

			// Token: 0x04001D93 RID: 7571
			protected int LOC_TT_LIVES = "#tooltip_vs_lives".GetHashCodeCustom();

			// Token: 0x04001D94 RID: 7572
			protected int LOC_TT_TEAMS = "#tooltip_vs_teams".GetHashCodeCustom();

			// Token: 0x04001D95 RID: 7573
			protected int LOC_TT_LUGGAGE = "#tooltip_vs_luggage".GetHashCodeCustom();

			// Token: 0x04001D96 RID: 7574
			private List<DropDownBox> mMenuItems;

			// Token: 0x04001D97 RID: 7575
			private List<int> mMenuTitles;

			// Token: 0x04001D98 RID: 7576
			private List<int> mToolTips;

			// Token: 0x0200037A RID: 890
			public struct OptionsMessage : ISendable
			{
				// Token: 0x170006AC RID: 1708
				// (get) Token: 0x06001B4A RID: 6986 RVA: 0x000BB017 File Offset: 0x000B9217
				public PacketType PacketType
				{
					get
					{
						return PacketType.VersusOptions;
					}
				}

				// Token: 0x06001B4B RID: 6987 RVA: 0x000BB01C File Offset: 0x000B921C
				public unsafe void Write(BinaryWriter iWriter)
				{
					iWriter.Write((byte)this.Ruleset);
					iWriter.Write(this.NrOfSettings);
					fixed (byte* ptr = &this.Settings.FixedElementField)
					{
						for (int i = 0; i < (int)this.NrOfSettings; i++)
						{
							iWriter.Write(ptr[i]);
						}
					}
				}

				// Token: 0x06001B4C RID: 6988 RVA: 0x000BB06C File Offset: 0x000B926C
				public unsafe void Read(BinaryReader iReader)
				{
					this.Ruleset = (Rulesets)iReader.ReadByte();
					this.NrOfSettings = iReader.ReadByte();
					fixed (byte* ptr = &this.Settings.FixedElementField)
					{
						for (int i = 0; i < (int)this.NrOfSettings; i++)
						{
							ptr[i] = iReader.ReadByte();
						}
					}
				}

				// Token: 0x04001D99 RID: 7577
				public Rulesets Ruleset;

				// Token: 0x04001D9A RID: 7578
				public byte NrOfSettings;

				// Token: 0x04001D9B RID: 7579
				[FixedBuffer(typeof(byte), 32)]
				public VersusRuleset.Settings.OptionsMessage.<Settings>e__FixedBufferd Settings;

				// Token: 0x0200037B RID: 891
				[UnsafeValueType]
				[CompilerGenerated]
				[StructLayout(LayoutKind.Sequential, Size = 32)]
				public struct <Settings>e__FixedBufferd
				{
					// Token: 0x04001D9C RID: 7580
					public byte FixedElementField;
				}
			}

			// Token: 0x0200037C RID: 892
			// (Invoke) Token: 0x06001B4E RID: 6990
			public delegate void SettingChanged(int iOption, int iNewSelection);
		}

		// Token: 0x0200037D RID: 893
		protected struct DirtyItem<T>
		{
			// Token: 0x04001D9D RID: 7581
			public T Value;

			// Token: 0x04001D9E RID: 7582
			public T NewValue;

			// Token: 0x04001D9F RID: 7583
			public bool Dirty;
		}

		// Token: 0x0200037E RID: 894
		protected class Score
		{
			// Token: 0x06001B51 RID: 6993 RVA: 0x000BB0BC File Offset: 0x000B92BC
			public Score(bool iLeftAligned)
			{
				if (VersusRuleset.Score.sVertexBuffer == null)
				{
					lock (Game.Instance.GraphicsDevice)
					{
						VersusRuleset.Score.sVertexBuffer = new VertexBuffer(Game.Instance.GraphicsDevice, Defines.QUAD_TEX_VERTS_TL.Length * VertexPositionTexture.SizeInBytes, BufferUsage.WriteOnly);
						VersusRuleset.Score.sVertexBuffer.SetData<VertexPositionTexture>(Defines.QUAD_TEX_VERTS_TL);
						VersusRuleset.Score.sVertexDeclaration = RenderManager.Instance.CreateVertexDeclaration(VertexPositionTexture.VertexElements);
						VersusRuleset.Score.sFont = FontManager.Instance.GetFont(MagickaFont.Maiandra16);
						VersusRuleset.Score.sTexture = Game.Instance.Content.Load<Texture2D>("UI/HUD/hud");
					}
				}
				this.mPlayers = new List<VersusRuleset.Score.Player>();
				this.mLeftAligned.Dirty = false;
				this.mLeftAligned.Value = iLeftAligned;
				this.mScore.Dirty = false;
				this.mScore.Value = 0;
				this.mScoreFont = FontManager.Instance.GetFont(MagickaFont.VersusText);
				this.mScoreFontLineHeight = (float)this.mScoreFont.LineHeight;
				this.mScoreText = new Text(32, this.mScoreFont, TextAlign.Center, false);
				this.mScoreText.SetText(this.mScore.Value.ToString());
				this.mTimers = new List<VersusRuleset.DirtyItem<int>>();
				this.mRespawnTexts = new List<Text>();
				this.mRespawnFont = FontManager.Instance.GetFont(MagickaFont.VersusText);
				this.mTimerFontLineHeight = (float)this.mRespawnFont.LineHeight;
			}

			// Token: 0x06001B52 RID: 6994 RVA: 0x000BB23C File Offset: 0x000B943C
			public void AddPlayer(string iName, int iID, Texture2D iTexture, Vector3 iColor)
			{
				VersusRuleset.Score.Player item = default(VersusRuleset.Score.Player);
				item.ID = iID;
				item.Texture = iTexture;
				item.Color = new Vector4(iColor, 1f);
				VersusRuleset.DirtyItem<int> item2 = default(VersusRuleset.DirtyItem<int>);
				item2.Dirty = false;
				item2.Value = 0;
				this.mTimers.Add(item2);
				Text text = new Text(32, this.mRespawnFont, TextAlign.Center, false);
				text.SetText("");
				this.mRespawnTexts.Add(text);
				this.mPlayers.Add(item);
			}

			// Token: 0x06001B53 RID: 6995 RVA: 0x000BB2CB File Offset: 0x000B94CB
			public void SetScore(int iScore)
			{
				if (iScore != this.mScore.Value)
				{
					this.mScore.Dirty = true;
					this.mScore.NewValue = iScore;
				}
			}

			// Token: 0x06001B54 RID: 6996 RVA: 0x000BB2F4 File Offset: 0x000B94F4
			public void SetTimer(int iID, int iTimer)
			{
				for (int i = 0; i < this.mPlayers.Count; i++)
				{
					if (this.mPlayers[i].ID == iID && this.mTimers[i].Value != iTimer)
					{
						VersusRuleset.DirtyItem<int> value = this.mTimers[i];
						value.NewValue = iTimer;
						value.Dirty = true;
						this.mTimers[i] = value;
					}
				}
			}

			// Token: 0x06001B55 RID: 6997 RVA: 0x000BB36C File Offset: 0x000B956C
			public void RemovePlayer(int iID)
			{
				for (int i = 0; i < this.mPlayers.Count; i++)
				{
					if (this.mPlayers[i].ID == iID)
					{
						this.mPlayers.RemoveAt(i);
						this.mRespawnTexts.RemoveAt(i);
						this.mTimers.RemoveAt(i);
						return;
					}
				}
			}

			// Token: 0x170006AD RID: 1709
			// (get) Token: 0x06001B56 RID: 6998 RVA: 0x000BB3C8 File Offset: 0x000B95C8
			// (set) Token: 0x06001B57 RID: 6999 RVA: 0x000BB3D5 File Offset: 0x000B95D5
			public bool LeftAligned
			{
				get
				{
					return this.mLeftAligned.Value;
				}
				set
				{
					if (value != this.mLeftAligned.Value)
					{
						this.mLeftAligned.NewValue = value;
						this.mLeftAligned.Dirty = true;
					}
				}
			}

			// Token: 0x170006AE RID: 1710
			// (get) Token: 0x06001B58 RID: 7000 RVA: 0x000BB3FD File Offset: 0x000B95FD
			public float Width
			{
				get
				{
					return (float)this.mPlayers.Count * VersusRuleset.Score.sSize.X + 80f;
				}
			}

			// Token: 0x06001B59 RID: 7001 RVA: 0x000BB41C File Offset: 0x000B961C
			public void Draw(GUIBasicEffect iEffect, float iX, float iY)
			{
				if (this.mPlayers.Count == 0)
				{
					return;
				}
				if (this.mLeftAligned.Dirty)
				{
					this.mLeftAligned.Value = this.mLeftAligned.NewValue;
					this.mLeftAligned.Dirty = false;
					BitmapFont font = FontManager.Instance.GetFont(MagickaFont.MenuTitle);
					if (this.mLeftAligned.Value)
					{
						this.mScoreText = new Text(32, font, TextAlign.Right, false);
					}
					else
					{
						this.mScoreText = new Text(32, font, TextAlign.Left, false);
					}
				}
				if (this.mScore.Dirty)
				{
					this.mScore.Value = this.mScore.NewValue;
					this.mScore.Dirty = false;
					this.mScoreText.SetText(this.mScore.Value.ToString());
				}
				for (int i = 0; i < this.mTimers.Count; i++)
				{
					VersusRuleset.DirtyItem<int> value = this.mTimers[i];
					if (value.Dirty)
					{
						value.Value = this.mTimers[i].NewValue;
						value.Dirty = false;
						this.mTimers[i] = value;
						this.mRespawnTexts[i].SetText(value.Value.ToString());
					}
				}
				iEffect.GraphicsDevice.Vertices[0].SetSource(VersusRuleset.Score.sVertexBuffer, 0, VertexPositionTexture.SizeInBytes);
				iEffect.GraphicsDevice.VertexDeclaration = VersusRuleset.Score.sVertexDeclaration;
				iEffect.VertexColorEnabled = false;
				iEffect.GraphicsDevice.RenderState.CullMode = CullMode.None;
				Point screenSize = RenderManager.Instance.ScreenSize;
				iEffect.SetScreenSize(screenSize.X, screenSize.Y);
				float num = this.mLeftAligned.Value ? (iX - this.Width) : (iX + 80f);
				Matrix transform;
				if (this.mLeftAligned.Value)
				{
					transform = new Matrix(23f, 0f, 0f, 0f, 0f, 106f, 0f, 0f, 0f, 0f, 1f, 0f, num - 23f, iY, 0f, 1f);
				}
				else
				{
					transform = new Matrix(-23f, 0f, 0f, 0f, 0f, 106f, 0f, 0f, 0f, 0f, 1f, 0f, num + (float)this.mPlayers.Count * VersusRuleset.Score.sSize.X + 23f, iY, 0f, 1f);
				}
				iEffect.Transform = transform;
				iEffect.Texture = VersusRuleset.Score.sTexture;
				iEffect.TextureOffset = new Vector2(0.5f, 0f);
				iEffect.TextureScale = new Vector2(23f / (float)VersusRuleset.Score.sTexture.Width, 106f / (float)VersusRuleset.Score.sTexture.Height);
				iEffect.TextureEnabled = true;
				iEffect.CommitChanges();
				iEffect.GraphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleFan, 0, 2);
				for (int j = 0; j < this.mPlayers.Count; j++)
				{
					transform = new Matrix(74f, 0f, 0f, 0f, 0f, 32f, 0f, 0f, 0f, 0f, 1f, 0f, num + (float)j * VersusRuleset.Score.sSize.X, iY, 0f, 1f);
					iEffect.Transform = transform;
					iEffect.Texture = VersusRuleset.Score.sTexture;
					iEffect.TextureOffset = new Vector2(0.5f + 23f / (float)VersusRuleset.Score.sTexture.Width, 0f);
					iEffect.TextureScale = new Vector2(74f / (float)VersusRuleset.Score.sTexture.Width, 32f / (float)VersusRuleset.Score.sTexture.Height);
					iEffect.TextureEnabled = true;
					iEffect.CommitChanges();
					iEffect.GraphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleFan, 0, 2);
					float num2 = iY + 32f;
					this.DrawPlayer(iEffect, j, num + (float)j * VersusRuleset.Score.sSize.X, num2);
					iEffect.Texture = VersusRuleset.Score.sTexture;
					iEffect.TextureOffset = new Vector2(0.5f + 23f / (float)VersusRuleset.Score.sTexture.Width, 32f / (float)VersusRuleset.Score.sTexture.Height);
					iEffect.TextureScale = new Vector2(74f / (float)VersusRuleset.Score.sTexture.Width, 74f / (float)VersusRuleset.Score.sTexture.Height);
					transform = new Matrix(74f, 0f, 0f, 0f, 0f, 74f, 0f, 0f, 0f, 0f, 1f, 0f, num + (float)j * VersusRuleset.Score.sSize.X, num2, 0f, 1f);
					iEffect.Transform = transform;
					iEffect.CommitChanges();
					iEffect.GraphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleFan, 0, 2);
				}
				if (this.mLeftAligned.Value)
				{
					transform = new Matrix(64f, 0f, 0f, 0f, 0f, 90f, 0f, 0f, 0f, 0f, 1f, 0f, num + (float)this.mPlayers.Count * VersusRuleset.Score.sSize.X, iY, 0f, 1f);
				}
				else
				{
					transform = new Matrix(-64f, 0f, 0f, 0f, 0f, 90f, 0f, 0f, 0f, 0f, 1f, 0f, num, iY, 0f, 1f);
				}
				iEffect.Transform = transform;
				iEffect.Texture = VersusRuleset.Score.sTexture;
				iEffect.TextureOffset = new Vector2(0.5f + 97f / (float)VersusRuleset.Score.sTexture.Width, 0f);
				iEffect.TextureScale = new Vector2(64f / (float)VersusRuleset.Score.sTexture.Width, 90f / (float)VersusRuleset.Score.sTexture.Height);
				iEffect.TextureEnabled = true;
				iEffect.CommitChanges();
				iEffect.GraphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleFan, 0, 2);
				iEffect.Color = new Vector4(1f);
				iEffect.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
				if (!this.HideNegativeScore || this.mScore.Value >= 0)
				{
					if (this.mLeftAligned.Value)
					{
						this.mScoreText.Draw(iEffect, iX - 40f - 10f, iY + 24f + VersusRuleset.Score.sSize.X * 0.5f - this.mScoreFontLineHeight * 0.5f + 8f, 0.8f);
						return;
					}
					this.mScoreText.Draw(iEffect, iX + 40f + 11f, iY + 24f + VersusRuleset.Score.sSize.X * 0.5f - this.mScoreFontLineHeight * 0.5f + 8f, 0.8f);
				}
			}

			// Token: 0x06001B5A RID: 7002 RVA: 0x000BBB5C File Offset: 0x000B9D5C
			private void DrawPlayer(GUIBasicEffect iEffect, int iIndex, float iX, float iY)
			{
				Matrix transform = new Matrix(64f, 0f, 0f, 0f, 0f, 64f, 0f, 0f, 0f, 0f, 1f, 0f, iX + 5f, iY + 5f, 0f, 1f);
				iEffect.Transform = transform;
				iEffect.Color = new Vector4(1f);
				iEffect.TextureEnabled = true;
				iEffect.Texture = VersusRuleset.Score.sTexture;
				iEffect.TextureOffset = new Vector2(448f / (float)VersusRuleset.Score.sTexture.Width, 0f);
				iEffect.TextureScale = new Vector2(64f / (float)VersusRuleset.Score.sTexture.Width, 64f / (float)VersusRuleset.Score.sTexture.Height);
				iEffect.CommitChanges();
				iEffect.GraphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleFan, 0, 2);
				iEffect.TextureOffset = new Vector2(0f, 0.5f);
				iEffect.TextureScale = new Vector2(1f, 0.5f);
				iEffect.Color = this.mPlayers[iIndex].Color;
				iEffect.Texture = this.mPlayers[iIndex].Texture;
				iEffect.CommitChanges();
				iEffect.GraphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleFan, 0, 2);
				iEffect.TextureOffset = new Vector2(0f, 0f);
				iEffect.Color = new Vector4(1f);
				iEffect.CommitChanges();
				iEffect.GraphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleFan, 0, 2);
				if ((float)this.mTimers[iIndex].Value > 0f)
				{
					iEffect.Color = new Vector4(1f);
					this.mRespawnTexts[iIndex].Draw(iEffect, iX + VersusRuleset.Score.sSize.X * 0.5f + 4f, iY + VersusRuleset.Score.sSize.Y * 0.5f - this.mTimerFontLineHeight * 0.5f);
				}
				iEffect.GraphicsDevice.Vertices[0].SetSource(VersusRuleset.Score.sVertexBuffer, 0, VertexPositionTexture.SizeInBytes);
				iEffect.GraphicsDevice.VertexDeclaration = VersusRuleset.Score.sVertexDeclaration;
			}

			// Token: 0x04001DA0 RID: 7584
			private const float SCORE_TEXT_WIDTH = 80f;

			// Token: 0x04001DA1 RID: 7585
			private static VertexBuffer sVertexBuffer;

			// Token: 0x04001DA2 RID: 7586
			private static VertexDeclaration sVertexDeclaration;

			// Token: 0x04001DA3 RID: 7587
			private static BitmapFont sFont;

			// Token: 0x04001DA4 RID: 7588
			private static Texture2D sTexture;

			// Token: 0x04001DA5 RID: 7589
			private List<VersusRuleset.Score.Player> mPlayers;

			// Token: 0x04001DA6 RID: 7590
			private List<VersusRuleset.DirtyItem<int>> mTimers;

			// Token: 0x04001DA7 RID: 7591
			private List<Text> mRespawnTexts;

			// Token: 0x04001DA8 RID: 7592
			private static readonly Vector2 sSize = new Vector2(74f, 74f);

			// Token: 0x04001DA9 RID: 7593
			private VersusRuleset.DirtyItem<bool> mLeftAligned;

			// Token: 0x04001DAA RID: 7594
			private VersusRuleset.DirtyItem<int> mScore;

			// Token: 0x04001DAB RID: 7595
			private Text mScoreText;

			// Token: 0x04001DAC RID: 7596
			private float mScoreFontLineHeight;

			// Token: 0x04001DAD RID: 7597
			private float mTimerFontLineHeight;

			// Token: 0x04001DAE RID: 7598
			public bool HideNegativeScore;

			// Token: 0x04001DAF RID: 7599
			private BitmapFont mScoreFont;

			// Token: 0x04001DB0 RID: 7600
			private BitmapFont mRespawnFont;

			// Token: 0x0200037F RID: 895
			private struct Player
			{
				// Token: 0x04001DB1 RID: 7601
				public Texture2D Texture;

				// Token: 0x04001DB2 RID: 7602
				public Vector4 Color;

				// Token: 0x04001DB3 RID: 7603
				public int ID;
			}
		}

		// Token: 0x02000380 RID: 896
		protected class RenderData : IRenderableGUIObject
		{
			// Token: 0x06001B5C RID: 7004 RVA: 0x000BBDB0 File Offset: 0x000B9FB0
			public RenderData(GUIBasicEffect iEffect, PieEffect iPieEffect, Texture2D iPieTexture, Texture2D iCountdownTexture, List<VersusRuleset.Score> iScores)
			{
				this.mEffect = iEffect;
				this.mPieEffect = iPieEffect;
				this.mPieEffect.Texture = iPieTexture;
				this.mScores = iScores;
				this.mMagickType.Value = MagickType.None;
				this.mMagickType.Dirty = false;
				this.mMagickTime.Value = 0;
				this.mMagickTime.Dirty = false;
				BitmapFont font = FontManager.Instance.GetFont(MagickaFont.VersusText);
				this.mTimeFontHeight = (float)font.LineHeight;
				this.mTimeText = new Text(10, font, TextAlign.Center, false);
				this.mTimeText.SetText("");
				this.mTimeText.DrawShadows = true;
				this.mTimeText.ShadowsOffset = new Vector2(2f, 2f);
				this.mMagickTypeFont = FontManager.Instance.GetFont(MagickaFont.VersusText);
				this.mMagickWidth = 0f;
				this.mMagickFontHeight = (float)font.LineHeight;
				this.mMagickText = new Text(50, font, TextAlign.Center, false);
				this.mMagickText.SetText("");
				this.mMagickText.DrawShadows = true;
				this.mMagickText.ShadowsOffset = new Vector2(2f, 2f);
				this.mMagickTimeText = new Text(20, font, TextAlign.Left, false);
				this.mMagickTimeText.SetText("");
				this.mMagickTimeText.DrawShadows = true;
				this.mMagickTimeText.ShadowsOffset = new Vector2(2f, 2f);
				this.mCountDown.Dirty = false;
				this.mCountDown.Value = (this.mCountDown.NewValue = 0);
				this.mSuddenDeath.Dirty = false;
				this.mSuddenDeath.Value = false;
				font = FontManager.Instance.GetFont(MagickaFont.VersusText);
				this.mGameOverText = new Text(64, font, TextAlign.Center, false);
				this.mGameOverText.SetText(LanguageManager.Instance.GetString(VersusRuleset.LOC_GAME_OVER));
				this.mGameOver.Dirty = false;
				this.mGameOver.Value = false;
				this.mCountdownTexture = iCountdownTexture;
				if (VersusRuleset.RenderData.sPieVertexBuffer == null)
				{
					VertexPositionTexture[] array = new VertexPositionTexture[33];
					float num = 32f;
					Vector2 vector = new Vector2(0f, 0f);
					array[0].Position.X = 0f;
					array[0].Position.Y = 0f;
					array[0].TextureCoordinate.X = vector.X;
					array[0].TextureCoordinate.Y = vector.Y;
					for (int i = 1; i < 33; i++)
					{
						float num2 = (float)(i - 1) / 31f;
						float num3 = (float)Math.Cos((double)((num2 - 0.25f) * 6.2831855f));
						float num4 = (float)Math.Sin((double)((num2 - 0.25f) * 6.2831855f));
						array[i].Position.X = 1f;
						array[i].Position.Y = num2;
						array[i].TextureCoordinate.X = vector.X + -num3 * num / (float)iPieTexture.Width;
						array[i].TextureCoordinate.Y = vector.Y + num4 * num / (float)iPieTexture.Height;
					}
					lock (Game.Instance.GraphicsDevice)
					{
						VersusRuleset.RenderData.sPieVertexBuffer = new VertexBuffer(Game.Instance.GraphicsDevice, array.Length * VertexPositionTexture.SizeInBytes, BufferUsage.WriteOnly);
						VersusRuleset.RenderData.sPieVertexBuffer.SetData<VertexPositionTexture>(array);
						VersusRuleset.RenderData.sPieVertexDeclaration = new VertexDeclaration(Game.Instance.GraphicsDevice, VertexPositionTexture.VertexElements);
					}
				}
				if (VersusRuleset.RenderData.sVertexBuffer == null)
				{
					lock (Game.Instance.GraphicsDevice)
					{
						VersusRuleset.RenderData.sVertexBuffer = new VertexBuffer(Game.Instance.GraphicsDevice, Defines.QUAD_TEX_VERTS_TL.Length * VertexPositionTexture.SizeInBytes, BufferUsage.WriteOnly);
						VersusRuleset.RenderData.sVertexBuffer.SetData<VertexPositionTexture>(Defines.QUAD_TEX_VERTS_TL);
						VersusRuleset.RenderData.sVertexDeclaration = RenderManager.Instance.CreateVertexDeclaration(VertexPositionTexture.VertexElements);
						VersusRuleset.RenderData.sTexture = Game.Instance.Content.Load<Texture2D>("UI/HUD/hud");
					}
				}
			}

			// Token: 0x06001B5D RID: 7005 RVA: 0x000BC20C File Offset: 0x000BA40C
			public void SetUnlockMagick(MagickType iMagick)
			{
				if (this.mMagickType.Value != iMagick)
				{
					this.mMagickType.NewValue = iMagick;
					this.mMagickType.Dirty = true;
				}
			}

			// Token: 0x06001B5E RID: 7006 RVA: 0x000BC234 File Offset: 0x000BA434
			public void SetUnlockMagickTime(float iTime)
			{
				if (this.mMagickTime.Value != (int)iTime)
				{
					this.mMagickTime.NewValue = (int)iTime;
					this.mMagickTime.Dirty = true;
				}
			}

			// Token: 0x06001B5F RID: 7007 RVA: 0x000BC25E File Offset: 0x000BA45E
			public void SetCountDown(float iTime)
			{
				if (this.mCountDown.Value != (int)iTime)
				{
					this.mCountDown.NewValue = (int)iTime;
					this.mCountDown.Dirty = true;
				}
				this.mCountDownFloat = iTime - (float)this.mCountDown.NewValue;
			}

			// Token: 0x06001B60 RID: 7008 RVA: 0x000BC29C File Offset: 0x000BA49C
			public void SetTimeText(int iTime)
			{
				if (this.mTime.Value != iTime)
				{
					this.mTime.NewValue = iTime;
					this.mTime.Dirty = true;
				}
			}

			// Token: 0x06001B61 RID: 7009 RVA: 0x000BC2C4 File Offset: 0x000BA4C4
			public void SuddenDeath(bool iBool)
			{
				if (this.mSuddenDeath.Value != iBool)
				{
					this.mSuddenDeath.NewValue = iBool;
					this.mSuddenDeath.Dirty = true;
				}
			}

			// Token: 0x170006AF RID: 1711
			// (get) Token: 0x06001B62 RID: 7010 RVA: 0x000BC2EC File Offset: 0x000BA4EC
			// (set) Token: 0x06001B63 RID: 7011 RVA: 0x000BC2F9 File Offset: 0x000BA4F9
			public bool GameOver
			{
				get
				{
					return this.mGameOver.Value;
				}
				set
				{
					if (this.mGameOver.Value != value)
					{
						this.mGameOver.NewValue = value;
						this.mGameOver.Dirty = true;
					}
				}
			}

			// Token: 0x06001B64 RID: 7012 RVA: 0x000BC324 File Offset: 0x000BA524
			public void Draw(float iDeltaTime)
			{
				if (this.mCountDown.Dirty)
				{
					this.mCountDown.Value = this.mCountDown.NewValue;
					this.mCountDown.Dirty = false;
				}
				if (this.mTime.Dirty)
				{
					this.mTime.Value = this.mTime.NewValue;
					this.mTime.Dirty = false;
					this.mTimeText.SetText(this.mTime.Value.ToString());
				}
				if (this.mSuddenDeath.Dirty)
				{
					this.mSuddenDeath.Value = this.mSuddenDeath.NewValue;
					this.mSuddenDeath.Dirty = false;
					if (this.mSuddenDeath.Value)
					{
						this.mMagickText.SetText(LanguageManager.Instance.GetString(VersusRuleset.RenderData.LOC_SUDDEN_DEATH));
						this.mMagickType.Value = MagickType.None;
					}
				}
				if (this.mMagickType.Dirty)
				{
					this.mMagickType.Value = this.mMagickType.NewValue;
					this.mMagickType.Dirty = false;
					this.mMagickText.SetText(LanguageManager.Instance.GetString(Magick.NAME_LOCALIZATION[(int)this.mMagickType.Value]));
				}
				if (this.mMagickTime.Dirty)
				{
					this.mMagickTime.Value = this.mMagickTime.NewValue;
					this.mMagickTime.Dirty = false;
					this.mMagickTimeText.SetText(this.mMagickTime.Value.ToString());
					this.mMagickWidth = this.mMagickTypeFont.MeasureText(this.mMagickText.Characters, true).X;
				}
				if (this.mGameOver.Dirty)
				{
					this.mGameOver.Dirty = false;
					this.mGameOver.Value = this.mGameOver.NewValue;
				}
				Point screenSize = RenderManager.Instance.ScreenSize;
				if (this.DrawTime)
				{
					this.mEffect.GraphicsDevice.Vertices[0].SetSource(VersusRuleset.RenderData.sVertexBuffer, 0, VertexPositionTexture.SizeInBytes);
					this.mEffect.GraphicsDevice.VertexDeclaration = VersusRuleset.RenderData.sVertexDeclaration;
					this.mEffect.VertexColorEnabled = false;
					this.mEffect.SetScreenSize(screenSize.X, screenSize.Y);
					Matrix identity = new Matrix(94f, 0f, 0f, 0f, 0f, 94f, 0f, 0f, 0f, 0f, 1f, 0f, (float)screenSize.X * 0.5f - 47f, 6f, 0f, 1f);
					this.mEffect.Transform = identity;
					this.mEffect.Texture = VersusRuleset.RenderData.sTexture;
					this.mEffect.TextureOffset = new Vector2(384f / (float)VersusRuleset.RenderData.sTexture.Width, 128f / (float)VersusRuleset.RenderData.sTexture.Height);
					this.mEffect.TextureScale = new Vector2(94f / (float)VersusRuleset.RenderData.sTexture.Width, 94f / (float)VersusRuleset.RenderData.sTexture.Height);
					this.mEffect.TextureEnabled = true;
					this.mEffect.Begin();
					this.mEffect.CurrentTechnique.Passes[0].Begin();
					this.mEffect.GraphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleFan, 0, 2);
					this.mEffect.CurrentTechnique.Passes[0].End();
					this.mEffect.End();
					this.mPieEffect.GraphicsDevice.RenderState.CullMode = CullMode.None;
					this.mPieEffect.GraphicsDevice.Vertices[0].SetSource(VersusRuleset.RenderData.sPieVertexBuffer, 0, VertexPositionTexture.SizeInBytes);
					this.mPieEffect.GraphicsDevice.VertexDeclaration = VersusRuleset.RenderData.sPieVertexDeclaration;
					this.mPieEffect.TextureOffset = VersusRuleset.RenderData.sPieOffset;
					identity = Matrix.Identity;
					identity.M41 = (float)screenSize.X * 0.5f;
					identity.M42 = 54f;
					identity.M11 = 0f;
					identity.M12 = -1.5f;
					identity.M21 = -1.5f;
					identity.M22 = 0f;
					identity.M33 = -1.5f;
					this.mPieEffect.Transform = identity;
					this.mPieEffect.MaxAngle = 6.2831855f * (this.Time / this.TimeLimit);
					this.mPieEffect.Begin();
					this.mPieEffect.CurrentTechnique.Passes[0].Begin();
					this.mPieEffect.GraphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleFan, 0, 31);
					this.mPieEffect.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
					this.mPieEffect.CurrentTechnique.Passes[0].End();
					this.mPieEffect.End();
					this.mEffect.GraphicsDevice.Vertices[0].SetSource(VersusRuleset.RenderData.sVertexBuffer, 0, VertexPositionTexture.SizeInBytes);
					this.mEffect.GraphicsDevice.VertexDeclaration = VersusRuleset.RenderData.sVertexDeclaration;
					this.mEffect.VertexColorEnabled = false;
					this.mEffect.SetScreenSize(screenSize.X, screenSize.Y);
					identity = new Matrix(106f, 0f, 0f, 0f, 0f, 106f, 0f, 0f, 0f, 0f, 1f, 0f, (float)screenSize.X * 0.5f - 53f, 0f, 0f, 1f);
					this.mEffect.Transform = identity;
					this.mEffect.Texture = VersusRuleset.RenderData.sTexture;
					this.mEffect.TextureOffset = new Vector2(0.5f, 128f / (float)VersusRuleset.RenderData.sTexture.Height);
					this.mEffect.TextureScale = new Vector2(106f / (float)VersusRuleset.RenderData.sTexture.Width, 106f / (float)VersusRuleset.RenderData.sTexture.Height);
					this.mEffect.TextureEnabled = true;
					this.mEffect.Begin();
					this.mEffect.CurrentTechnique.Passes[0].Begin();
					this.mEffect.GraphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleFan, 0, 2);
					this.mEffect.CurrentTechnique.Passes[0].End();
					this.mEffect.End();
				}
				this.mEffect.SetScreenSize(screenSize.X, screenSize.Y);
				this.mEffect.Begin();
				this.mEffect.CurrentTechnique.Passes[0].Begin();
				float num = (float)screenSize.X * 0.5f;
				if (this.mCountDown.Value >= 0)
				{
					this.mEffect.GraphicsDevice.Vertices[0].SetSource(VersusRuleset.RenderData.sVertexBuffer, 0, VertexPositionTexture.SizeInBytes);
					this.mEffect.GraphicsDevice.VertexDeclaration = VersusRuleset.RenderData.sVertexDeclaration;
					this.mEffect.VertexColorEnabled = false;
					this.mEffect.Texture = this.mCountdownTexture;
					this.mEffect.TextureEnabled = true;
					this.mEffect.TextureScale = new Vector2(0.25f, 1f);
					this.mEffect.TextureOffset = new Vector2((float)this.mCountDown.Value / 4f, 0f);
					float num2 = 256f;
					float num3 = 0f;
					float num4 = 1f - this.mCountDownFloat;
					this.mCountDownScale = (2f * (float)Math.Pow(0.0010000000474974513, (double)num4) + 1f) * (((1f - (float)Math.Pow(9.999999747378752E-05, (double)num4)) * 1f + 0f) / 2f);
					if (this.mCountDown.Value <= 0)
					{
						num2 *= this.mCountDownScale * 1.5f;
						num3 = 16f;
					}
					else
					{
						num2 *= this.mCountDownScale;
					}
					Matrix identity = new Matrix(num2, 0f, 0f, 0f, 0f, num2, 0f, 0f, 0f, 0f, 1f, 0f, (float)screenSize.X * 0.5f - num2 * 0.5f + num3, (float)screenSize.Y * 0.5f - num2 * 0.5f, 0f, 1f);
					this.mEffect.Transform = identity;
					this.mEffect.CommitChanges();
					this.mEffect.GraphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleFan, 0, 2);
				}
				if (this.mGameOver.Value)
				{
					this.mGameOverText.Draw(this.mEffect, num, (float)screenSize.Y * 0.35f);
				}
				if (this.mTime.Value > 0)
				{
					this.mTimeText.Draw(this.mEffect, num + 1f, 54f - this.mTimeFontHeight * 0.5f);
				}
				float num5 = 0f;
				float num6 = 0f;
				for (int i = 0; i < this.mScores.Count; i++)
				{
					if (this.mScores[i].LeftAligned)
					{
						this.mScores[i].Draw(this.mEffect, num - num5 - 48f, 0f);
						num5 += this.mScores[i].Width + 16f;
					}
					else
					{
						this.mScores[i].Draw(this.mEffect, num + num6 + 48f, 0f);
						num6 += this.mScores[i].Width + 16f;
					}
				}
				if (this.mMagickType.Value != MagickType.None && (float)this.mMagickTime.Value > 0f)
				{
					this.mMagickText.Draw(this.mEffect, num, 96f);
					this.mMagickTimeText.Draw(this.mEffect, num + this.mMagickWidth * 0.5f + 8f, 96f);
				}
				if (this.mSuddenDeath.Value)
				{
					this.mMagickText.Draw(this.mEffect, num, 96f);
				}
				this.mEffect.CurrentTechnique.Passes[0].End();
				this.mEffect.End();
			}

			// Token: 0x170006B0 RID: 1712
			// (get) Token: 0x06001B65 RID: 7013 RVA: 0x000BCDE3 File Offset: 0x000BAFE3
			public int ZIndex
			{
				get
				{
					return 100;
				}
			}

			// Token: 0x04001DB4 RID: 7604
			private const float PADDING = 4f;

			// Token: 0x04001DB5 RID: 7605
			public const float CLOCK_RADIUS = 32f;

			// Token: 0x04001DB6 RID: 7606
			private const int CLOCK_VERTS = 33;

			// Token: 0x04001DB7 RID: 7607
			private static VertexBuffer sVertexBuffer;

			// Token: 0x04001DB8 RID: 7608
			private static VertexDeclaration sVertexDeclaration;

			// Token: 0x04001DB9 RID: 7609
			private static Texture2D sTexture;

			// Token: 0x04001DBA RID: 7610
			private static VertexBuffer sPieVertexBuffer;

			// Token: 0x04001DBB RID: 7611
			private static VertexDeclaration sPieVertexDeclaration;

			// Token: 0x04001DBC RID: 7612
			private static readonly Vector2 sPieOffset = new Vector2(0.9375f, 0.1875f);

			// Token: 0x04001DBD RID: 7613
			private static int LOC_SUDDEN_DEATH = "#opt_vs_sudden_death".GetHashCodeCustom();

			// Token: 0x04001DBE RID: 7614
			private Texture2D mCountdownTexture;

			// Token: 0x04001DBF RID: 7615
			private List<VersusRuleset.Score> mScores;

			// Token: 0x04001DC0 RID: 7616
			private GUIBasicEffect mEffect;

			// Token: 0x04001DC1 RID: 7617
			private PieEffect mPieEffect;

			// Token: 0x04001DC2 RID: 7618
			public bool DrawTime;

			// Token: 0x04001DC3 RID: 7619
			public float Time;

			// Token: 0x04001DC4 RID: 7620
			public float TimeLimit;

			// Token: 0x04001DC5 RID: 7621
			private VersusRuleset.DirtyItem<int> mTime;

			// Token: 0x04001DC6 RID: 7622
			private Text mTimeText;

			// Token: 0x04001DC7 RID: 7623
			private float mTimeFontHeight;

			// Token: 0x04001DC8 RID: 7624
			private VersusRuleset.DirtyItem<int> mCountDown;

			// Token: 0x04001DC9 RID: 7625
			private float mCountDownScale;

			// Token: 0x04001DCA RID: 7626
			private float mCountDownFloat;

			// Token: 0x04001DCB RID: 7627
			private VersusRuleset.DirtyItem<bool> mGameOver;

			// Token: 0x04001DCC RID: 7628
			private Text mGameOverText;

			// Token: 0x04001DCD RID: 7629
			private VersusRuleset.DirtyItem<bool> mSuddenDeath;

			// Token: 0x04001DCE RID: 7630
			private VersusRuleset.DirtyItem<int> mMagickTime;

			// Token: 0x04001DCF RID: 7631
			private Text mMagickTimeText;

			// Token: 0x04001DD0 RID: 7632
			private VersusRuleset.DirtyItem<MagickType> mMagickType;

			// Token: 0x04001DD1 RID: 7633
			private Text mMagickText;

			// Token: 0x04001DD2 RID: 7634
			private float mMagickWidth;

			// Token: 0x04001DD3 RID: 7635
			private float mMagickFontHeight;

			// Token: 0x04001DD4 RID: 7636
			private BitmapFont mMagickTypeFont;
		}

		// Token: 0x02000381 RID: 897
		public enum MessageTypes : byte
		{
			// Token: 0x04001DD6 RID: 7638
			Time,
			// Token: 0x04001DD7 RID: 7639
			End,
			// Token: 0x04001DD8 RID: 7640
			SuddenDeath,
			// Token: 0x04001DD9 RID: 7641
			Setup,
			// Token: 0x04001DDA RID: 7642
			Score
		}
	}
}
