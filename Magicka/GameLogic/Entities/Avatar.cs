using System;
using System.Collections.Generic;
using Magicka.Achievements;
using Magicka.AI;
using Magicka.AI.AgentStates;
using Magicka.Audio;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.Entities.Abilities.SpecialAbilities;
using Magicka.GameLogic.Entities.AnimationActions;
using Magicka.GameLogic.Entities.Buffs;
using Magicka.GameLogic.Entities.CharacterStates;
using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.UI;
using Magicka.Gamers;
using Magicka.Graphics;
using Magicka.Levels;
using Magicka.Levels.Triggers;
using Magicka.Levels.Versus;
using Magicka.Localization;
using Magicka.Network;
using Magicka.PathFinding;
using Magicka.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using SteamWrapper;
using XNAnimation;
using XNAnimation.Effects;

namespace Magicka.GameLogic.Entities
{
	// Token: 0x020005EA RID: 1514
	public class Avatar : Character, IAI
	{
		// Token: 0x06002D6D RID: 11629 RVA: 0x00170BF6 File Offset: 0x0016EDF6
		public static void ClearCache()
		{
			Avatar.mCache.Clear();
		}

		// Token: 0x06002D6E RID: 11630 RVA: 0x00170C04 File Offset: 0x0016EE04
		public static void InitializeCache(PlayState iPlayState)
		{
			Avatar.mCache.Clear();
			for (int i = 0; i < 16; i++)
			{
				Avatar.mCache.Add(new Avatar(iPlayState));
			}
		}

		// Token: 0x06002D6F RID: 11631 RVA: 0x00170C38 File Offset: 0x0016EE38
		public static Avatar GetFromCache(Player iPlayer)
		{
			if (iPlayer == null)
			{
				throw new ArgumentException("iPlayer cannot be null", "iPlayer");
			}
			Avatar avatar = Avatar.mCache[0];
			Avatar.mCache.RemoveAt(0);
			avatar.mPlayer = iPlayer;
			return avatar;
		}

		// Token: 0x06002D70 RID: 11632 RVA: 0x00170C78 File Offset: 0x0016EE78
		public static Avatar GetFromCache(Player iPlayer, ushort iHandle)
		{
			if (iPlayer == null)
			{
				throw new ArgumentException("iPlayer cannot be null", "iPlayer");
			}
			Avatar avatar = Entity.GetFromHandle((int)iHandle) as Avatar;
			Avatar.mCache.Remove(avatar);
			avatar.mPlayer = iPlayer;
			return avatar;
		}

		// Token: 0x17000AAC RID: 2732
		// (get) Token: 0x06002D71 RID: 11633 RVA: 0x00170CB8 File Offset: 0x0016EEB8
		// (set) Token: 0x06002D72 RID: 11634 RVA: 0x00170CC0 File Offset: 0x0016EEC0
		internal Fairy RevivalFairy
		{
			get
			{
				return this.mFairy;
			}
			set
			{
				this.mFairy = value;
			}
		}

		// Token: 0x06002D73 RID: 11635 RVA: 0x00170CCC File Offset: 0x0016EECC
		private Avatar(PlayState iPlayState) : base(iPlayState)
		{
			this.mPlayer = null;
			for (int i = 0; i < 32; i++)
			{
				this.mMissileCache.Enqueue(new MissileEntity(iPlayState));
			}
			this.mOutlineRenderData = new Avatar.OutlineRenderData[3];
			for (int j = 0; j < this.mOutlineRenderData.Length; j++)
			{
				this.mOutlineRenderData[j] = new Avatar.OutlineRenderData();
				this.mOutlineRenderData[j].mSkeleton = this.mRenderData[j].mSkeleton;
			}
			this.mAfterImageRenderData = new Avatar.AfterImageRenderData[3];
			Matrix[][] array = new Matrix[5][];
			for (int k = 0; k < array.Length; k++)
			{
				array[k] = new Matrix[80];
			}
			for (int l = 0; l < 3; l++)
			{
				this.mAfterImageRenderData[l] = new Avatar.AfterImageRenderData(array);
			}
			this.mTargettingRenderData = new Avatar.TargettingRenderData[3];
			for (int m = 0; m < 3; m++)
			{
				this.mTargettingRenderData[m] = new Avatar.TargettingRenderData(Game.Instance.GraphicsDevice, Game.Instance.Content.Load<Texture2D>("UI/HUD/AimAid"), null);
			}
			this.mFairy = Fairy.MakeFairy(iPlayState, this);
		}

		// Token: 0x17000AAD RID: 2733
		// (get) Token: 0x06002D74 RID: 11636 RVA: 0x00170E28 File Offset: 0x0016F028
		private int PlayerIndex
		{
			get
			{
				for (int i = 0; i < Game.Instance.Players.Length; i++)
				{
					if (Game.Instance.Players[i] == this.mPlayer)
					{
						return i;
					}
				}
				return -1;
			}
		}

		// Token: 0x06002D75 RID: 11637 RVA: 0x00170E64 File Offset: 0x0016F064
		public void CheckInventory()
		{
			if (this.mPlayState.Inventory.Owner != null && this.mPlayState.Inventory.Owner != this)
			{
				return;
			}
			if (this.mPolymorphed)
			{
				return;
			}
			if (base.Equipment[0].Item.Type == Avatar.UNARMED | base.Equipment[1].Item.Type == Avatar.UNARMED)
			{
				return;
			}
			if (this.mPlayState.Inventory.Active)
			{
				this.mPlayState.Inventory.Close(this);
				return;
			}
			this.mPlayState.Inventory.ShowInventory(base.Equipment[0].Item, base.Equipment[1].Item, this);
		}

		// Token: 0x17000AAE RID: 2734
		// (get) Token: 0x06002D76 RID: 11638 RVA: 0x00170F25 File Offset: 0x0016F125
		internal Vector3 DesiredInputDirection
		{
			get
			{
				return this.mDesiredInputDirection;
			}
		}

		// Token: 0x06002D77 RID: 11639 RVA: 0x00170F2D File Offset: 0x0016F12D
		internal void GenerateForceFeedback(float iLeft, float iRight)
		{
			if (this.Player.Controller != null)
			{
				this.Player.Controller.Rumble(iLeft, iRight);
			}
		}

		// Token: 0x06002D78 RID: 11640 RVA: 0x00170F4E File Offset: 0x0016F14E
		public override void Initialize(CharacterTemplate iTemplate, Vector3 iPosition, int iUniqueID)
		{
			this.Initialize(iTemplate, this.Player.ID, iPosition, iUniqueID);
		}

		// Token: 0x06002D79 RID: 11641 RVA: 0x00170F64 File Offset: 0x0016F164
		public override void Initialize(CharacterTemplate iTemplate, int iRandomOverride, Vector3 iPosition, int iUniqueID)
		{
			this.mStates.Clear();
			this.mStates.Push(AIStateIdle.Instance);
			this.mHeavyWeaponReload = false;
			this.mEvents = null;
			if (iTemplate.ID == Player.ALUCART_UNIQUE_ID)
			{
				base.SetUniqueID(Player.ALUCART_UNIQUE_ID);
			}
			if (this.mPlayState.Level.ForceCamera)
			{
				this.mPlayState.Camera.AttachPlayers(this);
			}
			else if (!(this.Player.Gamer is NetworkGamer))
			{
				this.mPlayState.Camera.AttachPlayers(this);
			}
			else if (this.Player.Gamer is NetworkGamer)
			{
				this.mPlayState.Camera.AttachNetworkPlayers(this);
			}
			base.Initialize(iTemplate, iRandomOverride, iPosition, iUniqueID);
			Level.AvatarItem[] additionalAvatarItems = base.PlayState.Level.AdditionalAvatarItems;
			for (int i = 0; i < additionalAvatarItems.Length; i++)
			{
				SkinnedModelBone skinnedModelBone = null;
				for (int j = 0; j < this.mModel.SkeletonBones.Count; j++)
				{
					if (this.mModel.SkeletonBones[j].Name.Equals(additionalAvatarItems[i].Bone, StringComparison.OrdinalIgnoreCase))
					{
						skinnedModelBone = this.mModel.SkeletonBones[j];
						break;
					}
				}
				if (skinnedModelBone != null && additionalAvatarItems[i].Item != null)
				{
					for (int k = 0; k < this.mEquipment.Length; k++)
					{
						if (this.mEquipment[k].AttachIndex < 0)
						{
							this.mEquipment[k].Set(additionalAvatarItems[i].Item, skinnedModelBone, null);
							break;
						}
					}
				}
			}
			if (!this.mPolymorphed)
			{
				if (!string.IsNullOrEmpty(this.Player.Weapon))
				{
					Item.GetCachedWeapon(this.Player.Weapon.GetHashCodeCustom(), this.mEquipment[0].Item);
				}
				if (!string.IsNullOrEmpty(this.Player.Staff))
				{
					Item.GetCachedWeapon(this.Player.Staff.GetHashCodeCustom(), this.mEquipment[1].Item);
				}
			}
			for (int l = 0; l < this.mOutlineRenderData.Length; l++)
			{
				this.mOutlineRenderData[l].SetMeshDirty();
			}
			this.mTurnSpeedMax = (this.mTurnSpeed = 12f);
			this.mMove = false;
			this.mPolymorphed = false;
			Player player = this.Player;
			this.mBoosts = 0;
			this.mBoostCooldown = 0f;
			this.mCanChant = true;
			this.mLeftTriggerActive = false;
			this.mRightTriggerActive = false;
			this.mShowStaffSpecialAbilityNotifierTimer = 0f;
			this.mDeadAge = 0f;
			this.mChantedSpells = 0;
			if (player.Controller != null)
			{
				player.Controller.Invert(false);
			}
			this.ResetAfterImages();
			this.mCastButtonPressed = CastType.None;
			player.IconRenderer.SetCapacity(5);
			player.IconRenderer.Clear();
			player.InputQueue.Clear();
			if (base.Equipment[1].Item.HasSpecialAbility)
			{
				this.mSpecialAbilityName = LanguageManager.Instance.GetString(base.Equipment[1].Item.SpecialAbilityName);
				this.mShowStaffSpecialAbilityNotifierTimer = 3f;
			}
			this.mFaction |= (Factions)(256 << player.ID);
			this.mFaction |= player.Team;
			if (this.mPlayState.Level.CurrentScene != null && this.mPlayState.Level.CurrentScene.RuleSet is VersusRuleset)
			{
				this.mFaction &= ~Factions.FRIENDLY;
			}
			Factions team = player.Team;
			if (team != Factions.NONE)
			{
				if (team == Factions.TEAM_RED)
				{
					this.mMaterial.TintColor = Defines.TEAMCOLORS[0];
					goto IL_410;
				}
				if (team == Factions.TEAM_BLUE)
				{
					this.mMaterial.TintColor = Defines.TEAMCOLORS[1];
					goto IL_410;
				}
			}
			this.mMaterial.TintColor = Defines.PLAYERCOLORS[(int)player.Color];
			IL_410:
			base.SetImmortalTime(0f);
			this.Player.Ressing = false;
			this.mDoNotRender = false;
		}

		// Token: 0x06002D7A RID: 11642 RVA: 0x0017139F File Offset: 0x0016F59F
		public override void Deinitialize()
		{
			base.Deinitialize();
			if (!Avatar.mCache.Contains(this))
			{
				Avatar.mCache.Add(this);
			}
			this.mPlayState.Inventory.Close(this);
		}

		// Token: 0x06002D7B RID: 11643 RVA: 0x001713D0 File Offset: 0x0016F5D0
		public bool FindPickUp(bool iCheckDirection, Pickable iItem)
		{
			List<Entity> entities = this.mPlayState.EntityManager.GetEntities(this.Position, 2.5f, false);
			for (int i = 0; i < entities.Count; i++)
			{
				Pickable pickable = entities[i] as Pickable;
				if (pickable != null && pickable.IsPickable)
				{
					Vector3 vector = this.Position;
					vector.Y = 0f;
					Vector3 position = pickable.Position;
					position.Y = 0f;
					Vector3.Subtract(ref position, ref vector, out position);
					float num = position.Length();
					if (num <= this.Radius && iItem == pickable)
					{
						this.mPlayState.EntityManager.ReturnEntityList(entities);
						return true;
					}
					if (iCheckDirection)
					{
						Vector3.Divide(ref position, num, out position);
						vector = base.CharacterBody.Direction;
						float num2;
						Vector3.Dot(ref vector, ref position, out num2);
						if (num2 > 0f && iItem == pickable)
						{
							this.mPlayState.EntityManager.ReturnEntityList(entities);
							return true;
						}
					}
					else if (iItem == pickable)
					{
						this.mPlayState.EntityManager.ReturnEntityList(entities);
						return true;
					}
				}
			}
			this.mPlayState.EntityManager.ReturnEntityList(entities);
			return false;
		}

		// Token: 0x06002D7C RID: 11644 RVA: 0x001714FC File Offset: 0x0016F6FC
		public Pickable FindPickUp(bool iCheckDirection)
		{
			List<Entity> entities = this.mPlayState.EntityManager.GetEntities(this.Position, 2.5f, false);
			for (int i = 0; i < entities.Count; i++)
			{
				Pickable pickable = entities[i] as Pickable;
				if (pickable != null && pickable.IsPickable)
				{
					Vector3 vector = this.Position;
					vector.Y = 0f;
					Vector3 position = pickable.Position;
					position.Y = 0f;
					Vector3.Subtract(ref position, ref vector, out position);
					float num = position.Length();
					if (num <= this.Radius)
					{
						this.mPlayState.EntityManager.ReturnEntityList(entities);
						return pickable;
					}
					if (!iCheckDirection)
					{
						this.mPlayState.EntityManager.ReturnEntityList(entities);
						return pickable;
					}
					Vector3.Divide(ref position, num, out position);
					vector = base.CharacterBody.Direction;
					float num2;
					Vector3.Dot(ref vector, ref position, out num2);
					if (num2 > 0f)
					{
						this.mPlayState.EntityManager.ReturnEntityList(entities);
						return pickable;
					}
				}
			}
			this.mPlayState.EntityManager.ReturnEntityList(entities);
			return null;
		}

		// Token: 0x06002D7D RID: 11645 RVA: 0x0017161C File Offset: 0x0016F81C
		public Interactable FindInteractable(bool iCheckDirection)
		{
			SortedList<int, Trigger> triggers = this.mPlayState.Level.CurrentScene.Triggers;
			for (int i = 0; i < triggers.Count; i++)
			{
				Interactable interactable = triggers.Values[i] as Interactable;
				if (interactable != null && interactable.Enabled)
				{
					Vector3 position = this.Position;
					position.Y = 0f;
					Locator locator = interactable.Locator;
					Vector3 translation = locator.Transform.Translation;
					translation.Y = 0f;
					float num;
					Vector3.DistanceSquared(ref translation, ref position, out num);
					float radius = base.Capsule.Radius;
					if (num <= radius * radius)
					{
						return interactable;
					}
					if (num <= locator.Radius * locator.Radius)
					{
						if (!iCheckDirection)
						{
							return interactable;
						}
						Vector3 vector;
						Vector3.Subtract(ref translation, ref position, out vector);
						Vector3 direction = base.CharacterBody.Direction;
						vector.Normalize();
						float num2;
						Vector3.Dot(ref direction, ref vector, out num2);
						if (num2 > 0.7f)
						{
							return interactable;
						}
					}
				}
			}
			return null;
		}

		// Token: 0x06002D7E RID: 11646 RVA: 0x00171720 File Offset: 0x0016F920
		public Character FindCharacter(bool iCheckDirection)
		{
			float num = float.MaxValue;
			Character result = null;
			List<Entity> entities = this.mPlayState.EntityManager.GetEntities(this.Position, 3f, false);
			entities.Remove(this);
			for (int i = 0; i < entities.Count; i++)
			{
				Character character = entities[i] as Character;
				if (character != null && character.InteractText != InteractType.None)
				{
					Vector3 vector = this.Position;
					vector.Y = 0f;
					Vector3 position = character.Position;
					position.Y = 0f;
					Vector3.Subtract(ref position, ref vector, out position);
					if (iCheckDirection)
					{
						position.Normalize();
						vector = base.CharacterBody.Direction;
						float num2;
						Vector3.Dot(ref vector, ref position, out num2);
						if (num2 >= 0.7f)
						{
							this.mPlayState.EntityManager.ReturnEntityList(entities);
							return character;
						}
					}
					else
					{
						float num3 = position.LengthSquared();
						if (num3 < num)
						{
							num = num3;
							result = character;
						}
					}
				}
			}
			this.mPlayState.EntityManager.ReturnEntityList(entities);
			return result;
		}

		// Token: 0x06002D7F RID: 11647 RVA: 0x0017182C File Offset: 0x0016FA2C
		public override MissileEntity GetMissileInstance()
		{
			MissileEntity missileEntity = this.mMissileCache.Dequeue();
			this.mMissileCache.Enqueue(missileEntity);
			return missileEntity;
		}

		// Token: 0x17000AAF RID: 2735
		// (set) Token: 0x06002D80 RID: 11648 RVA: 0x00171852 File Offset: 0x0016FA52
		public override bool IsBlocking
		{
			set
			{
				if (this.mPolymorphed)
				{
					return;
				}
				base.IsBlocking = value;
			}
		}

		// Token: 0x17000AB0 RID: 2736
		// (get) Token: 0x06002D81 RID: 11649 RVA: 0x00171864 File Offset: 0x0016FA64
		public string SpecialAbilityName
		{
			get
			{
				return this.mSpecialAbilityName;
			}
		}

		// Token: 0x17000AB1 RID: 2737
		// (get) Token: 0x06002D82 RID: 11650 RVA: 0x0017186C File Offset: 0x0016FA6C
		public bool NotifySpecialAbility
		{
			get
			{
				return !this.mPolymorphed && ((this.mShowStaffSpecialAbilityNotifierTimer > 0f && this.mEquipment[1].Item.HasSpecialAbility) || (this.mEquipment[1].Item.CooldownHintTime && !string.IsNullOrEmpty(this.mSpecialAbilityName)));
			}
		}

		// Token: 0x17000AB2 RID: 2738
		// (get) Token: 0x06002D83 RID: 11651 RVA: 0x001718CA File Offset: 0x0016FACA
		// (set) Token: 0x06002D84 RID: 11652 RVA: 0x001718D2 File Offset: 0x0016FAD2
		internal override bool Polymorphed
		{
			get
			{
				return this.mPolymorphed;
			}
			set
			{
				this.mPolymorphed = value;
			}
		}

		// Token: 0x06002D85 RID: 11653 RVA: 0x001718DB File Offset: 0x0016FADB
		public void ResetAfterImages()
		{
			this.mAfterImageIntensity = -1f;
			this.mAfterImageTimer = 0f;
		}

		// Token: 0x17000AB3 RID: 2739
		// (get) Token: 0x06002D86 RID: 11654 RVA: 0x001718F3 File Offset: 0x0016FAF3
		public bool ChantingMagick
		{
			get
			{
				return this.mChantingMagick && this.MagickType != MagickType.None;
			}
		}

		// Token: 0x17000AB4 RID: 2740
		// (get) Token: 0x06002D87 RID: 11655 RVA: 0x0017190B File Offset: 0x0016FB0B
		public MagickType MagickType
		{
			get
			{
				return SpellManager.Instance.GetMagickType(this.Player, this.mPlayState, base.SpellQueue);
			}
		}

		// Token: 0x06002D88 RID: 11656 RVA: 0x0017192C File Offset: 0x0016FB2C
		public override void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			if (this.mQueuedNetActions.Count > 0)
			{
				CharacterActionMessage characterActionMessage = this.mQueuedNetActions.Pop();
				this.NetworkAction(ref characterActionMessage);
			}
			if (this.mEvents != null)
			{
				if (this.CurrentEvent < this.mEvents.Length | this.CurrentEventDelay >= 1E-45f | float.IsNaN(this.CurrentEventDelay))
				{
					IAIState iaistate = this.mStates.Peek();
					iaistate.OnExecute(this, iDeltaTime);
					this.CurrentStateAge += iDeltaTime;
				}
				else
				{
					this.mIgnoreTriggers = false;
				}
			}
			else
			{
				this.mIgnoreTriggers = false;
			}
			if ((this.mCurrentSpell == null && this.CastButton(CastType.Weapon) && this.CastType == CastType.None) || (this.mCurrentSpell == null && this.CastButton(CastType.Self) && this.CastType == CastType.None))
			{
				this.mCastButtonPressed = CastType.None;
				if (NetworkManager.Instance.State != NetworkState.Offline)
				{
					CharacterActionMessage characterActionMessage2 = default(CharacterActionMessage);
					characterActionMessage2.Handle = base.Handle;
					characterActionMessage2.Action = ActionType.CastSpell;
					characterActionMessage2.Param0I = 0;
					NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref characterActionMessage2);
				}
			}
			if (!this.Dead)
			{
				if (GlobalSettings.Instance.HealthBars != SettingOptions.Off & (this.mInvisibilityTimer <= 0f | this.mPlayState.GameType != GameType.Versus))
				{
					Vector4 value = new Vector4(1f, 0f, 0f, 1f);
					if (this.HasStatus(StatusEffects.Poisoned))
					{
						value.X = 0f;
						value.Y = 1f;
					}
					float iTimeSinceLastDamage = Math.Min(this.mTimeSinceLastDamage, this.mTimeSinceLastStatusDamage);
					Vector3 position = this.Position;
					position.Y -= base.Capsule.Length * 0.5f + base.Capsule.Radius;
					Healthbars.Instance.AddHealthBar(position, base.NormalizedHitPoints, this.mTemplate.Radius, 1.25f, iTimeSinceLastDamage, false, new Vector4?(value), null);
				}
				if (this.mShowStaffSpecialAbilityNotifierTimer > 0f)
				{
					this.mShowStaffSpecialAbilityNotifierTimer -= iDeltaTime;
				}
				if (SpellManager.Instance.GetMagickType(this.Player, base.PlayState, base.SpellQueue) == MagickType.None)
				{
					this.mChantingMagick = false;
				}
			}
			if (!base.IsInvisibile & !this.mDoNotRender & (base.CharacterBody.SpeedMultiplier > base.TimeWarpModifier | this.mAfterImageIntensity > -1f))
			{
				Avatar.AfterImageRenderData afterImageRenderData = this.mAfterImageRenderData[(int)iDataChannel];
				if (afterImageRenderData.MeshDirty)
				{
					ModelMesh modelMesh = this.mModel.Model.Meshes[0];
					ModelMeshPart iMeshPart = modelMesh.MeshParts[0];
					afterImageRenderData.SetMesh(modelMesh.VertexBuffer, modelMesh.IndexBuffer, iMeshPart);
					afterImageRenderData.Color = this.mMaterial.TintColor;
				}
				int count = this.mModel.SkeletonBones.Count;
				this.mAfterImageTimer -= iDeltaTime;
				this.mAfterImageIntensity -= iDeltaTime;
				while (this.mAfterImageTimer <= 0f)
				{
					this.mAfterImageTimer += 0.05f;
					Vector3 velocity = this.mBody.Velocity;
					velocity.Y = 0f;
					if (velocity.LengthSquared() > 0.001f & base.CharacterBody.SpeedMultiplier > 1f)
					{
						while (this.mAfterImageIntensity <= 0f)
						{
							this.mAfterImageIntensity += 0.05f;
						}
					}
					for (int i = afterImageRenderData.mSkeleton.Length - 1; i > 0; i--)
					{
						Array.Copy(afterImageRenderData.mSkeleton[i - 1], afterImageRenderData.mSkeleton[i], count);
					}
					Array.Copy(this.mAnimationController.SkinnedBoneTransforms, afterImageRenderData.mSkeleton[0], count);
				}
				afterImageRenderData.mIntensity = this.mAfterImageIntensity / 0.05f;
				afterImageRenderData.mBoundingSphere = this.mRenderData[(int)iDataChannel].mBoundingSphere;
				this.mPlayState.Scene.AddRenderableAdditiveObject(iDataChannel, afterImageRenderData);
			}
			else
			{
				this.mAfterImageTimer = 0f;
				this.mAfterImageIntensity = -1f;
			}
			base.Update(iDataChannel, iDeltaTime);
			if (this.mBoostCooldown > 0f)
			{
				this.mBoostCooldown -= iDeltaTime;
			}
			if (this.Dead)
			{
				this.mDeadAge += iDeltaTime;
			}
			if (this.mCurrentState == IdleState.Instance || this.mCurrentState == MoveState.Instance || this.mCurrentState == CastState.Instance || (this.mCurrentState == ChargeState.Instance && this.CastType == CastType.None))
			{
				this.mCanChant = true;
			}
			else
			{
				this.mCanChant = false;
			}
			if (base.SpellQueue.Count > 0)
			{
				base.CharacterBody.SpeedMultiplier *= 1f - (float)base.SpellQueue.Count / (float)base.SpellQueue.Capacity * 0.6f;
			}
			if ((!this.Dead & !this.mDoNotRender & !base.PlayState.IsInCutscene) && (this.mInvisibilityTimer <= 0f | this.mPlayState.GameType != GameType.Versus))
			{
				Avatar.OutlineRenderData outlineRenderData = this.mOutlineRenderData[(int)iDataChannel];
				outlineRenderData.Forced = base.IsInvisibile;
				if (outlineRenderData.MeshDirty)
				{
					ModelMesh modelMesh2 = this.mModel.Model.Meshes[0];
					ModelMeshPart iMeshPart2 = modelMesh2.MeshParts[0];
					outlineRenderData.SetMesh(modelMesh2.VertexBuffer, modelMesh2.IndexBuffer, iMeshPart2, SkinnedModelDeferredEffect.TYPEHASH);
					outlineRenderData.Color = this.mMaterial.TintColor;
				}
				outlineRenderData.mBoundingSphere = this.mRenderData[(int)iDataChannel].mBoundingSphere;
				this.mPlayState.Scene.AddProjection(iDataChannel, outlineRenderData);
			}
			if (!(this.Player.Gamer is NetworkGamer) && this.mCurrentState is ChargeState && !(this.Player.Controller is KeyboardMouseController))
			{
				this.mTargettingAlpha = Math.Min(this.mTargettingAlpha + iDeltaTime * 4f, 1f);
			}
			else
			{
				this.mTargettingAlpha = Math.Max(this.mTargettingAlpha - iDeltaTime * 4f, 0f);
			}
			if (this.mTargettingAlpha > 0f)
			{
				Avatar.TargettingRenderData targettingRenderData = this.mTargettingRenderData[(int)iDataChannel];
				Matrix matrix = default(Matrix);
				Vector3 direction = this.Direction;
				Vector3 up = Vector3.Up;
				Vector3 right;
				Vector3.Cross(ref direction, ref up, out right);
				right.Normalize();
				Vector3.Cross(ref up, ref right, out direction);
				matrix.Backward = up;
				matrix.Right = right;
				matrix.Up = direction;
				Vector3 position2 = this.Position;
				position2.Y += base.HeightOffset;
				matrix.Translation = position2;
				targettingRenderData.Glow = 1f;
				targettingRenderData.Alpha = this.mTargettingAlpha;
				matrix.M11 *= 1.25f;
				matrix.M12 *= 1.25f;
				matrix.M13 *= 1.25f;
				matrix.M21 *= 2.5f;
				matrix.M22 *= 2.5f;
				matrix.M23 *= 2.5f;
				targettingRenderData.TextureScale = new Vector2(0.25f, 1f);
				targettingRenderData.TextureOffset = new Vector2(0.6125f, 0f);
				matrix.M31 *= 2f;
				matrix.M32 *= 2f;
				matrix.M33 *= 2f;
				matrix.M44 = 1f;
				targettingRenderData.SetPosition(ref matrix);
				this.mPlayState.Scene.AddProjection(iDataChannel, targettingRenderData);
			}
			if (base.IsCharmed && (this.mCurrentState is CastState || this.mCurrentState is ChargeState))
			{
				Vector3 movement = base.CharacterBody.Movement;
				Vector3 movement2;
				if (movement.LengthSquared() > 1E-06f && this.InCharmDirection(ref movement, out movement2))
				{
					base.CharacterBody.Movement = movement2;
				}
			}
		}

		// Token: 0x06002D89 RID: 11657 RVA: 0x00172194 File Offset: 0x00170394
		public void UpdateMouseDirection(Vector2 iValue, bool iInverted)
		{
			if ((this.mEvents != null && this.CurrentEvent < this.mEvents.Length) || this.mHypnotized)
			{
				return;
			}
			if (float.IsNaN(iValue.X) || float.IsNaN(iValue.Y))
			{
				iValue = default(Vector2);
			}
			if (ControlManager.Instance.IsInputLimited || ControlManager.Instance.IsPlayerInputLocked(this.Player.ID))
			{
				base.CharacterBody.Movement = default(Vector3);
				return;
			}
			float num = iValue.LengthSquared();
			if (num < 1E-06f && !base.IsPanicing && !base.IsFeared && !base.IsStumbling)
			{
				base.CharacterBody.Movement = default(Vector3);
				CharacterBody characterBody = base.CharacterBody;
				Matrix orientation = base.CharacterBody.Transform.Orientation;
				characterBody.DesiredDirection = orientation.Forward;
				return;
			}
			Vector3 vector;
			if (iInverted)
			{
				vector = new Vector3(-iValue.X, 0f, -iValue.Y);
			}
			else
			{
				vector = new Vector3(iValue.X, 0f, iValue.Y);
			}
			if (num > 1E-06f)
			{
				float value = (float)Math.Sqrt((double)num);
				Vector3.Divide(ref vector, value, out vector);
			}
			this.mDesiredInputDirection = vector;
			Vector3 vector2;
			if (base.IsCharmed && (this.mCurrentState is CastState || this.mCurrentState is ChargeState) && this.InCharmDirection(ref vector, out vector2))
			{
				vector = vector2;
			}
			if (!this.mMove)
			{
				if (!base.IsPanicing && !base.IsFeared && num > 0.1f)
				{
					base.CharacterBody.Movement = vector;
				}
				vector = default(Vector3);
			}
			if (base.IsStumbling)
			{
				Vector3 movement = base.CharacterBody.Movement;
				Vector3.Multiply(ref vector, -1f, out vector);
				Vector3.Add(ref movement, ref vector, out vector);
				base.CharacterBody.Movement = vector;
			}
			if (base.IsPanicing && num > 0f)
			{
				Vector3 movement2 = base.CharacterBody.Movement;
				vector *= 1f - this.mPanic;
				Vector3.Add(ref movement2, ref vector, out vector);
				base.CharacterBody.Movement = vector;
			}
			if (!base.IsFeared)
			{
				base.CharacterBody.Movement = vector;
			}
		}

		// Token: 0x06002D8A RID: 11658 RVA: 0x001723E4 File Offset: 0x001705E4
		public void UpdatePadDirection(Vector2 iValue, bool iInverted)
		{
			if ((this.mEvents != null && this.CurrentEvent < this.mEvents.Length) || this.mHypnotized)
			{
				return;
			}
			if (ControlManager.Instance.IsInputLimited || ControlManager.Instance.IsPlayerInputLocked(this.Player.ID))
			{
				base.CharacterBody.Movement = default(Vector3);
				return;
			}
			float num = iValue.LengthSquared();
			Vector3 vector;
			if (iInverted)
			{
				vector = new Vector3(-iValue.X, 0f, iValue.Y);
			}
			else
			{
				vector = new Vector3(iValue.X, 0f, -iValue.Y);
			}
			this.mDesiredInputDirection = vector;
			if (base.IsCharmed && (this.mCurrentState is CastState || this.mCurrentState is ChargeState))
			{
				if (num <= 1E-06f)
				{
					vector = this.Direction;
				}
				Vector3 vector2;
				if (this.InCharmDirection(ref vector, out vector2))
				{
					vector = vector2;
				}
			}
			else if (num < 1E-06f && !base.IsPanicing && !base.IsFeared && !base.IsStumbling)
			{
				base.CharacterBody.Movement = default(Vector3);
				CharacterBody characterBody = base.CharacterBody;
				Matrix orientation = base.CharacterBody.Transform.Orientation;
				characterBody.DesiredDirection = orientation.Forward;
				return;
			}
			if (base.IsStumbling)
			{
				Vector3 movement = base.CharacterBody.Movement;
				Vector3.Multiply(ref vector, -1f, out vector);
				Vector3.Add(ref movement, ref vector, out vector);
				base.CharacterBody.Movement = vector;
			}
			if (base.IsPanicing && num > 0f)
			{
				Vector3 movement2 = base.CharacterBody.Movement;
				vector *= 1f - this.mPanic;
				Vector3.Add(ref movement2, ref vector, out vector);
				base.CharacterBody.Movement = vector;
			}
			if (!base.IsFeared)
			{
				base.CharacterBody.Movement = vector;
			}
		}

		// Token: 0x06002D8B RID: 11659 RVA: 0x001725D0 File Offset: 0x001707D0
		private bool InCharmDirection(ref Vector3 iDirection, out Vector3 oVector)
		{
			Vector3 position = base.CharmOwner.Position;
			Vector3 position2 = this.Position;
			Vector3 direction = this.Direction;
			Vector3 vector;
			Vector3.Subtract(ref position, ref position2, out vector);
			Vector2 vector2 = new Vector2(vector.X, vector.Z);
			vector2.Normalize();
			float num = MagickaMath.Angle(vector2);
			Vector2 vector3 = new Vector2(iDirection.X, iDirection.Z);
			vector3.Normalize();
			float num2 = MagickaMath.Angle(vector3);
			Vector2 vector4 = new Vector2(direction.X, direction.Z);
			vector4.Normalize();
			float num3 = MagickaMath.Angle(vector4);
			float num4 = num2 - num;
			float value = num3 - num;
			float num5 = 0.3926991f;
			if (Math.Abs(value) < num5 && Math.Abs(num4) < num5)
			{
				Matrix matrix;
				if (num4 >= 0f)
				{
					Matrix.CreateRotationY(-1.5707964f, out matrix);
				}
				else
				{
					Matrix.CreateRotationY(1.5707964f, out matrix);
				}
				Vector3.Transform(ref iDirection, ref matrix, out oVector);
				return true;
			}
			oVector = default(Vector3);
			return false;
		}

		// Token: 0x17000AB5 RID: 2741
		// (get) Token: 0x06002D8C RID: 11660 RVA: 0x001726D0 File Offset: 0x001708D0
		// (set) Token: 0x06002D8D RID: 11661 RVA: 0x001726D8 File Offset: 0x001708D8
		public bool ChargeUnlocked
		{
			get
			{
				return this.mChargeUnlocked;
			}
			set
			{
				this.mChargeUnlocked = value;
			}
		}

		// Token: 0x06002D8E RID: 11662 RVA: 0x001726E4 File Offset: 0x001708E4
		public void ForcePressed()
		{
			if (base.Equipment[1].Item.WeaponClass != WeaponClass.Staff || !TutorialManager.Instance.IsCastTypeEnabled(CastType.Force) || (this.mSpellQueue.Count == 0 && !TutorialManager.Instance.IsPushEnabled()))
			{
				return;
			}
			if (this.mCastButtonPressed == CastType.None & this.CastType == CastType.None)
			{
				this.CastType = CastType.Force;
				this.mCastButtonPressed = CastType.Force;
				if (NetworkManager.Instance.State != NetworkState.Offline)
				{
					CharacterActionMessage characterActionMessage = default(CharacterActionMessage);
					characterActionMessage.Handle = base.Handle;
					characterActionMessage.Action = ActionType.CastSpell;
					characterActionMessage.Param0I = (int)this.mCastButtonPressed;
					NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref characterActionMessage);
				}
			}
		}

		// Token: 0x06002D8F RID: 11663 RVA: 0x00172798 File Offset: 0x00170998
		public void ForceReleased()
		{
			this.mChargeUnlocked = true;
			if (this.mCastButtonPressed == CastType.Force)
			{
				this.mCastButtonPressed = CastType.None;
				if (NetworkManager.Instance.State != NetworkState.Offline)
				{
					CharacterActionMessage characterActionMessage = default(CharacterActionMessage);
					characterActionMessage.Handle = base.Handle;
					characterActionMessage.Action = ActionType.CastSpell;
					characterActionMessage.Param0I = 0;
					NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref characterActionMessage);
				}
			}
		}

		// Token: 0x06002D90 RID: 11664 RVA: 0x00172800 File Offset: 0x00170A00
		public void AreaPressed()
		{
			if (base.Equipment[1].Item.WeaponClass != WeaponClass.Staff || !TutorialManager.Instance.IsCastTypeEnabled(CastType.Area) || (this.mSpellQueue.Count == 0 && !TutorialManager.Instance.IsPushEnabled()))
			{
				return;
			}
			if (this.mCastButtonPressed == CastType.None & this.CastType == CastType.None)
			{
				this.CastType = CastType.Area;
				this.mCastButtonPressed = CastType.Area;
				if (NetworkManager.Instance.State != NetworkState.Offline)
				{
					CharacterActionMessage characterActionMessage = default(CharacterActionMessage);
					characterActionMessage.Handle = base.Handle;
					characterActionMessage.Action = ActionType.CastSpell;
					characterActionMessage.Param0I = (int)this.mCastButtonPressed;
					NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref characterActionMessage);
				}
			}
		}

		// Token: 0x06002D91 RID: 11665 RVA: 0x001728B4 File Offset: 0x00170AB4
		public void AreaReleased()
		{
			if (this.mCastButtonPressed == CastType.Area)
			{
				this.mCastButtonPressed = CastType.None;
				if (NetworkManager.Instance.State != NetworkState.Offline)
				{
					CharacterActionMessage characterActionMessage = default(CharacterActionMessage);
					characterActionMessage.Handle = base.Handle;
					characterActionMessage.Action = ActionType.CastSpell;
					characterActionMessage.Param0I = 0;
					NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref characterActionMessage);
				}
			}
		}

		// Token: 0x06002D92 RID: 11666 RVA: 0x00172913 File Offset: 0x00170B13
		public virtual void MouseMove()
		{
			this.mMove = true;
		}

		// Token: 0x06002D93 RID: 11667 RVA: 0x0017291C File Offset: 0x00170B1C
		public virtual void MouseMoveStop()
		{
			this.mMove = false;
		}

		// Token: 0x17000AB6 RID: 2742
		// (get) Token: 0x06002D94 RID: 11668 RVA: 0x00172925 File Offset: 0x00170B25
		public override int Type
		{
			get
			{
				return Avatar.WIZARDHASH;
			}
		}

		// Token: 0x17000AB7 RID: 2743
		// (get) Token: 0x06002D95 RID: 11669 RVA: 0x0017292C File Offset: 0x00170B2C
		public override bool IsInAEvent
		{
			get
			{
				return this.mEvents != null && this.CurrentEvent < this.mEvents.Length;
			}
		}

		// Token: 0x17000AB8 RID: 2744
		// (get) Token: 0x06002D96 RID: 11670 RVA: 0x00172948 File Offset: 0x00170B48
		public override bool IsImmortal
		{
			get
			{
				return this.IsInAEvent | this.mPlayState.IsInCutscene | base.IsImmortal;
			}
		}

		// Token: 0x17000AB9 RID: 2745
		// (set) Token: 0x06002D97 RID: 11671 RVA: 0x00172963 File Offset: 0x00170B63
		public override CastType CastType
		{
			set
			{
				if (!this.mPolymorphed)
				{
					base.CastType = value;
				}
			}
		}

		// Token: 0x06002D98 RID: 11672 RVA: 0x00172974 File Offset: 0x00170B74
		public bool CastButton(CastType iType)
		{
			return this.mCastButtonPressed == iType;
		}

		// Token: 0x06002D99 RID: 11673 RVA: 0x0017297F File Offset: 0x00170B7F
		internal void ConjureEarth()
		{
			this.HandleCombo(ControllerDirection.Down);
			this.HandleCombo(ControllerDirection.Left);
		}

		// Token: 0x06002D9A RID: 11674 RVA: 0x0017298F File Offset: 0x00170B8F
		internal void ConjureWater()
		{
			this.HandleCombo(ControllerDirection.Left);
			this.HandleCombo(ControllerDirection.Up);
		}

		// Token: 0x06002D9B RID: 11675 RVA: 0x0017299F File Offset: 0x00170B9F
		internal void ConjureCold()
		{
			this.HandleCombo(ControllerDirection.Right);
			this.HandleCombo(ControllerDirection.Up);
		}

		// Token: 0x06002D9C RID: 11676 RVA: 0x001729AF File Offset: 0x00170BAF
		internal void ConjureFire()
		{
			this.HandleCombo(ControllerDirection.Right);
			this.HandleCombo(ControllerDirection.Down);
		}

		// Token: 0x06002D9D RID: 11677 RVA: 0x001729BF File Offset: 0x00170BBF
		internal void ConjureLightning()
		{
			this.HandleCombo(ControllerDirection.Left);
			this.HandleCombo(ControllerDirection.Down);
		}

		// Token: 0x06002D9E RID: 11678 RVA: 0x001729CF File Offset: 0x00170BCF
		internal void ConjureArcane()
		{
			this.HandleCombo(ControllerDirection.Up);
			this.HandleCombo(ControllerDirection.Right);
		}

		// Token: 0x06002D9F RID: 11679 RVA: 0x001729DF File Offset: 0x00170BDF
		internal void ConjureLife()
		{
			this.HandleCombo(ControllerDirection.Up);
			this.HandleCombo(ControllerDirection.Left);
		}

		// Token: 0x06002DA0 RID: 11680 RVA: 0x001729EF File Offset: 0x00170BEF
		internal void ConjureShield()
		{
			this.HandleCombo(ControllerDirection.Down);
			this.HandleCombo(ControllerDirection.Right);
		}

		// Token: 0x06002DA1 RID: 11681 RVA: 0x00172A00 File Offset: 0x00170C00
		internal void HandleCombo(ControllerDirection iDirection)
		{
			if (this.IsInAEvent || this.Polymorphed || this.mCurrentState is ChargeState)
			{
				return;
			}
			Player player = this.Player;
			this.mChantDirection = iDirection;
			player.InputQueue.Add((int)iDirection);
			Spell spell = SpellManager.Instance.HandleCombo(player);
			if (spell.Element != Elements.None)
			{
				if (NetworkManager.Instance.State != NetworkState.Offline)
				{
					CharacterActionMessage characterActionMessage = default(CharacterActionMessage);
					characterActionMessage.Action = ActionType.ConjureElement;
					characterActionMessage.Handle = base.Handle;
					characterActionMessage.Param0I = (int)spell.Element;
					NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref characterActionMessage);
				}
				if (((spell.Element & Elements.Lightning) == Elements.Lightning && this.HasStatus(StatusEffects.Wet) && !base.HasPassiveAbility(Item.PassiveAbilities.WetLightning)) | ((spell.Element & Elements.Fire) == Elements.Fire && this.HasStatus(StatusEffects.Greased)))
				{
					if (this.HasStatus(StatusEffects.Wet))
					{
						TutorialManager.Instance.SetTip(TutorialManager.Tips.WetLightning, TutorialManager.Position.Top);
					}
					DamageCollection5 iDamage;
					spell.CalculateDamage(SpellType.Lightning, CastType.Self, out iDamage);
					iDamage.MultiplyMagnitude(0.5f);
					this.Damage(iDamage, this, base.PlayState.PlayTime, this.Position);
					player.InputQueue.Clear();
					return;
				}
				bool flag = SpellManager.Instance.TryAddToQueue(this.Player, this, this.mSpellQueue, 5, ref spell);
				if (flag)
				{
					this.mChantedSpells++;
				}
				int iSoundIndex = Defines.SOUNDS_UI_ELEMENT[Spell.ElementIndex(spell.Element)];
				AudioManager.Instance.PlayCue(Banks.UI, iSoundIndex);
				player.InputQueue.Clear();
			}
			if (!this.mCanChant)
			{
				this.mChantDirection = ControllerDirection.Center;
			}
			this.mChantingMagick = SpellManager.Instance.IsMagick(this.Player, this.mPlayState.GameType, base.SpellQueue);
			if (this.mChantingMagick)
			{
				TutorialManager.Instance.SetTip(TutorialManager.Tips.MagicksSpell, TutorialManager.Position.Top);
			}
		}

		// Token: 0x06002DA2 RID: 11682 RVA: 0x00172BDC File Offset: 0x00170DDC
		public void Action()
		{
			if (this.mCurrentState is CastState || this.mCurrentState is ChargeState || this.mPolymorphed)
			{
				return;
			}
			if (!DialogManager.Instance.AwaitingInput && !DialogManager.Instance.HoldoffInput)
			{
				if (!AudioManager.Instance.Threat)
				{
					Character character = this.FindCharacter(!(this.Player.Controller is KeyboardMouseController));
					if (character != null && character.Interact(this, this.Player.Controller))
					{
						if (!(this.Player.Gamer is NetworkGamer) & NetworkManager.Instance.State != NetworkState.Offline)
						{
							CharacterActionMessage characterActionMessage = default(CharacterActionMessage);
							characterActionMessage.Action = ActionType.Interact;
							characterActionMessage.Handle = base.Handle;
							characterActionMessage.TargetHandle = character.Handle;
							NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref characterActionMessage);
						}
						return;
					}
				}
				Pickable pickable = this.FindPickUp(!(this.Player.Controller is KeyboardMouseController));
				if (pickable != null)
				{
					this.PickUp(pickable);
				}
				Interactable interactable = this.FindInteractable(!(this.Player.Controller is KeyboardMouseController));
				if (interactable != null)
				{
					if (AudioManager.Instance.Threat && interactable.InteractType == InteractType.Talk)
					{
						return;
					}
					interactable.Interact(this);
				}
			}
		}

		// Token: 0x06002DA3 RID: 11683 RVA: 0x00172D38 File Offset: 0x00170F38
		public void Interact()
		{
			if (this.mCurrentState is CastState | this.mCurrentState is ChargeState)
			{
				return;
			}
			if (!DialogManager.Instance.AwaitingInput && !DialogManager.Instance.HoldoffInput)
			{
				Interactable interactable = this.FindInteractable(!(this.Player.Controller is KeyboardMouseController));
				if (interactable != null)
				{
					interactable.Interact(this);
				}
			}
		}

		// Token: 0x06002DA4 RID: 11684 RVA: 0x00172DA4 File Offset: 0x00170FA4
		public void Boost()
		{
			if (this.mCurrentState is ChargeState)
			{
				return;
			}
			if (this.mPolymorphed)
			{
				return;
			}
			if (TutorialManager.Instance.IsCastTypeEnabled(CastType.Magick) && (SpellManager.Instance.CombineMagick(this.Player, this.mPlayState.GameType, this.mSpellQueue) || (this.mSpellQueue.Count > 0 && this.mSpellQueue[0].Element == Elements.All)))
			{
				this.CastType = CastType.Magick;
				if (NetworkManager.Instance.State != NetworkState.Offline)
				{
					CharacterActionMessage characterActionMessage = default(CharacterActionMessage);
					characterActionMessage.Handle = base.Handle;
					characterActionMessage.Action = ActionType.CastSpell;
					characterActionMessage.Param0I = 5;
					NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref characterActionMessage);
				}
				SpellMagickConverter spellMagickConverter = default(SpellMagickConverter);
				spellMagickConverter.Spell = this.mSpell;
				if (Magicka.GameLogic.Spells.Magick.IsInstant(spellMagickConverter.Magick.MagickType) && (this.HasStatus(StatusEffects.Frozen) | this.mCurrentState is FlyingState | this.mCurrentState is KnockDownState))
				{
					this.CastSpell(true, "");
					return;
				}
			}
			else if (this.IsEntangled | base.IsGripped)
			{
				if (NetworkManager.Instance.State != NetworkState.Offline)
				{
					CharacterActionMessage characterActionMessage2 = default(CharacterActionMessage);
					characterActionMessage2.Handle = base.Handle;
					characterActionMessage2.Action = ActionType.BreakFree;
					NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref characterActionMessage2);
				}
				this.BreakFree();
				if (!(base.CurrentState is CastState))
				{
					base.GoToAnimation(Animations.spec_entangled_attack, 0.1f);
					return;
				}
			}
			else if (BoostState.Instance.ShieldToBoost(this) != null || (base.IsSelfShielded & !base.IsSolidSelfShielded))
			{
				if (NetworkManager.Instance.State != NetworkState.Offline)
				{
					CharacterActionMessage characterActionMessage3 = default(CharacterActionMessage);
					characterActionMessage3.Handle = base.Handle;
					characterActionMessage3.Action = ActionType.Boost;
					NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref characterActionMessage3);
				}
				this.mBoosts++;
				if (base.Equipment[1].Item.PassiveAbility.Ability == Item.PassiveAbilities.ShieldBoost)
				{
					this.mBoosts++;
				}
				this.mBoostCooldown = 0.25f;
				if (base.CurrentState != BoostState.Instance)
				{
					this.mBoosts = 1;
				}
				for (int i = 0; i < this.mAuras.Count; i++)
				{
					AuraStorage aura = this.mAuras[i].Aura;
					if (aura.AuraType == AuraType.Boost)
					{
						this.mBoosts *= (int)aura.AuraBoost.Magnitude;
					}
				}
				for (int j = 0; j < base.Equipment[1].Item.Auras.Count; j++)
				{
					AuraStorage aura2 = base.Equipment[1].Item.Auras[j].Aura;
					if (aura2.AuraType == AuraType.Boost)
					{
						this.mBoosts *= (int)aura2.AuraBoost.Magnitude;
					}
				}
				for (int k = 0; k < this.mBuffs.Count; k++)
				{
					if (this.mBuffs[k].BuffType == BuffType.Boost)
					{
						this.mBoosts *= (int)this.mBuffs[k].BuffBoost.Amount;
					}
				}
			}
		}

		// Token: 0x06002DA5 RID: 11685 RVA: 0x00173104 File Offset: 0x00171304
		public void Attack()
		{
			if (this.mCurrentSpell != null || this.mCurrentState is CastState || this.mCurrentState is ChargeState || ControlManager.Instance.IsInputLimited || base.Equipment[0].Item.IsCoolingdown)
			{
				return;
			}
			if ((base.SpellQueue.Count > 0 && base.SpellQueue[0].Element != Elements.All) || base.Equipment[0].Item.SpellCharged)
			{
				if (this.CastType == CastType.None && !base.IsGripped && TutorialManager.Instance.IsCastTypeEnabled(CastType.Weapon))
				{
					this.mCastButtonPressed = CastType.Weapon;
					this.CastType = CastType.Weapon;
					if (NetworkManager.Instance.State != NetworkState.Offline)
					{
						CharacterActionMessage characterActionMessage = default(CharacterActionMessage);
						characterActionMessage.Handle = base.Handle;
						characterActionMessage.Action = ActionType.CastSpell;
						characterActionMessage.Param0I = (int)this.mCastButtonPressed;
						NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref characterActionMessage);
					}
					base.PlayState.TooFancyForFireballs[this.Player.ID] = false;
				}
				return;
			}
			base.PlayState.TooFancyForFireballs[this.Player.ID] = false;
			if (this.mGripType == Grip.GripType.Pickup && base.IsGripped)
			{
				this.mBreakFreeCounter += (int)this.mBreakFreeStrength;
			}
			if (!this.mAttacking || this.mAnimationController.Time > this.mAnimationController.AnimationClip.Duration - 0.1f)
			{
				if (this.mEquipment[0].Item.WeaponClass == WeaponClass.Heavy)
				{
					if (!this.mAttacking)
					{
						if (this.mHeavyWeaponReload)
						{
							base.Attack(Animations.attack_melee1, this.WieldingGun);
						}
						else
						{
							base.Attack(Animations.attack_melee0, this.WieldingGun);
						}
						this.mHeavyWeaponReload = !this.mHeavyWeaponReload;
						return;
					}
				}
				else
				{
					if (MagickaMath.Random.NextDouble() < 0.5)
					{
						base.Attack(Animations.attack_melee1, this.WieldingGun);
						return;
					}
					base.Attack(Animations.attack_melee0, this.WieldingGun);
				}
			}
		}

		// Token: 0x06002DA6 RID: 11686 RVA: 0x00173310 File Offset: 0x00171510
		public void Special()
		{
			if (ControlManager.Instance.IsInputLimited & this.mCurrentSpell == null)
			{
				return;
			}
			if (base.SpellQueue.Count > 0)
			{
				if (TutorialManager.Instance.IsCastTypeEnabled(CastType.Self) && this.CastType == CastType.None)
				{
					this.mCastButtonPressed = CastType.Self;
					this.CastType = CastType.Self;
					if (this.HasStatus(StatusEffects.Frozen) || this.mCurrentState is FlyingState || this.mCurrentState is PushedState || this.mCurrentState is KnockDownState)
					{
						this.CastSpell(true, "");
					}
					if (NetworkManager.Instance.State != NetworkState.Offline)
					{
						CharacterActionMessage characterActionMessage = default(CharacterActionMessage);
						characterActionMessage.Handle = base.Handle;
						characterActionMessage.Action = ActionType.CastSpell;
						characterActionMessage.Param0I = (int)this.mCastButtonPressed;
						NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref characterActionMessage);
						return;
					}
				}
			}
			else if (!(this.mCurrentState is CastState | this.mCurrentState is ChargeState) && this.mEquipment[this.mSourceOfSpellIndex].Item.HasSpecialAbility)
			{
				if (NetworkManager.Instance.State != NetworkState.Offline)
				{
					CharacterActionMessage characterActionMessage2 = default(CharacterActionMessage);
					characterActionMessage2.Handle = base.Handle;
					characterActionMessage2.Param0I = this.mSourceOfSpellIndex;
					characterActionMessage2.Action = ActionType.ItemSpecial;
					NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref characterActionMessage2);
				}
				this.mEquipment[this.mSourceOfSpellIndex].Item.ExecuteSpecialAbility();
			}
		}

		// Token: 0x06002DA7 RID: 11687 RVA: 0x0017348C File Offset: 0x0017168C
		public override void Entangle(float iMagnitude)
		{
			base.Entangle(iMagnitude);
			this.mBoosts = 0;
		}

		// Token: 0x06002DA8 RID: 11688 RVA: 0x0017349C File Offset: 0x0017169C
		public void SpecialRelease()
		{
			if (this.mCastButtonPressed == CastType.Self)
			{
				this.mCastButtonPressed = CastType.None;
			}
		}

		// Token: 0x06002DA9 RID: 11689 RVA: 0x001734AE File Offset: 0x001716AE
		public void AttackRelease()
		{
			if (this.CastType == CastType.None & this.mCastButtonPressed == CastType.Weapon)
			{
				this.mCastButtonPressed = CastType.None;
			}
			if (this.WieldingGun)
			{
				this.mNextAttackAnimation = Animations.None;
			}
		}

		// Token: 0x17000ABA RID: 2746
		// (get) Token: 0x06002DAA RID: 11690 RVA: 0x001734DB File Offset: 0x001716DB
		public bool WieldingGun
		{
			get
			{
				return this.mEquipment[0].Item.IsGunClass;
			}
		}

		// Token: 0x17000ABB RID: 2747
		// (get) Token: 0x06002DAB RID: 11691 RVA: 0x001734EF File Offset: 0x001716EF
		internal ControllerDirection ChantDirection
		{
			get
			{
				return this.mChantDirection;
			}
		}

		// Token: 0x17000ABC RID: 2748
		// (get) Token: 0x06002DAC RID: 11692 RVA: 0x001734F7 File Offset: 0x001716F7
		// (set) Token: 0x06002DAD RID: 11693 RVA: 0x001734FF File Offset: 0x001716FF
		public override int Boosts
		{
			get
			{
				return this.mBoosts;
			}
			set
			{
				this.mBoosts = value;
			}
		}

		// Token: 0x17000ABD RID: 2749
		// (get) Token: 0x06002DAE RID: 11694 RVA: 0x00173508 File Offset: 0x00171708
		public override float BoostCooldown
		{
			get
			{
				return this.mBoostCooldown;
			}
		}

		// Token: 0x17000ABE RID: 2750
		// (get) Token: 0x06002DAF RID: 11695 RVA: 0x00173510 File Offset: 0x00171710
		public override bool IsAggressive
		{
			get
			{
				return AudioManager.Instance.Threat;
			}
		}

		// Token: 0x17000ABF RID: 2751
		// (get) Token: 0x06002DB0 RID: 11696 RVA: 0x0017351C File Offset: 0x0017171C
		public bool LeftTrigger
		{
			get
			{
				return this.mLeftTriggerActive;
			}
		}

		// Token: 0x17000AC0 RID: 2752
		// (get) Token: 0x06002DB1 RID: 11697 RVA: 0x00173524 File Offset: 0x00171724
		public bool RightTrigger
		{
			get
			{
				return this.mRightTriggerActive;
			}
		}

		// Token: 0x17000AC1 RID: 2753
		// (get) Token: 0x06002DB2 RID: 11698 RVA: 0x0017352C File Offset: 0x0017172C
		// (set) Token: 0x06002DB3 RID: 11699 RVA: 0x00173534 File Offset: 0x00171734
		public Player Player
		{
			get
			{
				return this.mPlayer;
			}
			set
			{
				this.mPlayer = value;
			}
		}

		// Token: 0x06002DB4 RID: 11700 RVA: 0x0017353D File Offset: 0x0017173D
		public override void BloatKill(Elements iElement, Entity iKiller)
		{
			base.BloatKill(iElement, iKiller);
			if (this.Player.Controller != null)
			{
				this.Player.Controller.Rumble(1f, 1f);
			}
		}

		// Token: 0x06002DB5 RID: 11701 RVA: 0x00173570 File Offset: 0x00171770
		public override void CastSpell(bool iFromStaff, string iJoint)
		{
			if (this.mSpell.Element != Elements.All)
			{
				base.PlayState.TooFancyForFireballs[this.Player.ID] = false;
			}
			if (this.mSpell.Element != Elements.All && this.CastType == CastType.Weapon)
			{
				Item item = base.Equipment[0].Item;
				if (item.SpellCharged && base.SpellQueue.Count == 0)
				{
					item.RetrieveSpell().Cast(iFromStaff, this, this.CastType);
					return;
				}
				for (int i = 0; i < base.SpellQueue.Count; i++)
				{
					Spell spell = base.SpellQueue[i];
					item.TryAddToQueue(ref spell, false);
				}
				this.Player.IconRenderer.Clear();
				base.SpellQueue.Clear();
				this.mChantedSpells = 0;
				if (!(this.Player.Gamer is NetworkGamer))
				{
					AchievementsManager.Instance.AwardAchievement(base.PlayState, "theenchanter");
					return;
				}
			}
			else
			{
				base.CastSpell(iFromStaff, iJoint);
			}
		}

		// Token: 0x06002DB6 RID: 11702 RVA: 0x00173684 File Offset: 0x00171884
		public override void CombineSpell()
		{
			this.Player.IconRenderer.Clear();
			this.mSpell = SpellManager.Instance.Combine(this.Player.SpellQueue);
			this.mChantedSpells = 0;
			this.Player.SpellQueue.Clear();
		}

		// Token: 0x06002DB7 RID: 11703 RVA: 0x001736D4 File Offset: 0x001718D4
		private void DropMagicks()
		{
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				int num = 34;
				for (int i = 0; i < num; i++)
				{
					ulong num2 = 1UL << i;
					if ((this.Player.UnlockedMagicks & num2) != 0UL && i != 1)
					{
						BookOfMagick instance = BookOfMagick.GetInstance(this.mPlayState);
						Vector3 vector = new Vector3((float)(Character.sRandom.NextDouble() - 0.5) * 3f, (float)Character.sRandom.NextDouble() * 7f, (float)(Character.sRandom.NextDouble() - 0.5) * 3f);
						instance.Initialize(this.Position, this.Body.Orientation, (MagickType)i, false, vector, 20f, 0);
						this.mPlayState.EntityManager.AddEntity(instance);
						if (NetworkManager.Instance.State == NetworkState.Server)
						{
							TriggerActionMessage triggerActionMessage = default(TriggerActionMessage);
							triggerActionMessage.ActionType = TriggerActionType.SpawnMagick;
							triggerActionMessage.Handle = instance.Handle;
							triggerActionMessage.Template = i;
							triggerActionMessage.Position = instance.Position;
							triggerActionMessage.Direction = vector;
							triggerActionMessage.Time = 20f;
							triggerActionMessage.Point0 = 0;
							triggerActionMessage.Bool0 = false;
							Matrix orientation = instance.Body.Orientation;
							Quaternion.CreateFromRotationMatrix(ref orientation, out triggerActionMessage.Orientation);
							NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref triggerActionMessage);
						}
					}
				}
			}
		}

		// Token: 0x06002DB8 RID: 11704 RVA: 0x00173848 File Offset: 0x00171A48
		public override void Terminate(bool iKillItems, bool iIsKillPlane, bool iNetwork)
		{
			base.Terminate(iKillItems, iIsKillPlane, iNetwork);
			if (iNetwork)
			{
				return;
			}
			if (this.mPlayState.Level.CurrentScene.RuleSet is VersusRuleset)
			{
				(this.mPlayState.Level.CurrentScene.RuleSet as VersusRuleset).OnPlayerDeath(this.Player);
				if ((this.mPlayState.Level.CurrentScene.RuleSet as VersusRuleset).DropMagicks)
				{
					this.DropMagicks();
					this.Player.UnlockedMagicks = 0UL;
					this.Player.IconRenderer.TomeMagick = MagickType.None;
				}
			}
			this.Die();
		}

		// Token: 0x06002DB9 RID: 11705 RVA: 0x001738EE File Offset: 0x00171AEE
		public override void Drown()
		{
			base.Drown();
			this.mPlayState.SetDiedInLevel();
			this.Die();
		}

		// Token: 0x06002DBA RID: 11706 RVA: 0x00173908 File Offset: 0x00171B08
		public override void OverKill()
		{
			base.OverKill();
			this.mPlayState.SetDiedInLevel();
			if (this.mPlayState.Level.CurrentScene.RuleSet is VersusRuleset)
			{
				(this.mPlayState.Level.CurrentScene.RuleSet as VersusRuleset).OnPlayerDeath(this.Player);
				if ((this.mPlayState.Level.CurrentScene.RuleSet as VersusRuleset).DropMagicks)
				{
					this.DropMagicks();
					this.Player.UnlockedMagicks = 0UL;
					this.Player.IconRenderer.TomeMagick = MagickType.None;
				}
			}
			this.Die();
		}

		// Token: 0x06002DBB RID: 11707 RVA: 0x001739B4 File Offset: 0x00171BB4
		public override void Die()
		{
			base.Die();
			this.mPlayState.SetDiedInLevel();
			if (this.Player.Color == 7 && !(this.Player.Gamer is NetworkGamer))
			{
				AchievementsManager.Instance.AwardAchievement(base.PlayState, "omgtheykilledyellow");
			}
			for (int i = 0; i < Game.Instance.Players.Length; i++)
			{
				Player player = Game.Instance.Players[i];
				Avatar avatar = player.Avatar;
				if (player.Playing && !avatar.Dead && avatar.UniqueID != base.UniqueID && avatar.IsInvisibile && !(avatar.Player.Gamer is NetworkGamer))
				{
					AchievementsManager.Instance.AwardAchievement(base.PlayState, "betteryouthanme");
				}
			}
			this.Player.Weapon = null;
			this.Player.Staff = null;
			this.mPlayState.Inventory.Close(this);
			if (this.mPlayState.Level.CurrentScene.RuleSet is VersusRuleset)
			{
				(this.mPlayState.Level.CurrentScene.RuleSet as VersusRuleset).OnPlayerDeath(this.Player);
				if ((this.mPlayState.Level.CurrentScene.RuleSet as VersusRuleset).DropMagicks)
				{
					this.DropMagicks();
					this.Player.UnlockedMagicks = 0UL;
					this.Player.IconRenderer.TomeMagick = MagickType.None;
				}
			}
			if (!Avatar.mCache.Contains(this))
			{
				Avatar.mCache.Add(this);
			}
			if (base.Name == "wizard_alucart")
			{
				this.KillEverybody();
			}
		}

		// Token: 0x06002DBC RID: 11708 RVA: 0x00173B5E File Offset: 0x00171D5E
		public void Slay()
		{
		}

		// Token: 0x06002DBD RID: 11709 RVA: 0x00173B60 File Offset: 0x00171D60
		public void KillEverybody()
		{
			if (NetworkManager.Instance.State == NetworkState.Client)
			{
				return;
			}
			Player[] players = Game.Instance.Players;
			foreach (Player player in players)
			{
				if (player.Avatar != null && !player.Avatar.mDead)
				{
					player.Avatar.Kill();
				}
			}
		}

		// Token: 0x06002DBE RID: 11710 RVA: 0x00173BBC File Offset: 0x00171DBC
		public void SetTransform(Matrix iTransform)
		{
			this.mBody.MoveTo(iTransform.Translation + new Vector3(0f, base.Capsule.Length * 0.5f + base.Capsule.Radius, 0f), iTransform);
		}

		// Token: 0x06002DBF RID: 11711 RVA: 0x00173C10 File Offset: 0x00171E10
		internal override void NetworkAction(ref CharacterActionMessage iMsg)
		{
			if (this.mPlayer == null)
			{
				this.mQueuedNetActions.Push(iMsg);
				return;
			}
			if (this.mQueuedNetActions.Count > 0)
			{
				CharacterActionMessage characterActionMessage = this.mQueuedNetActions.Pop();
				this.NetworkAction(ref characterActionMessage);
			}
			ActionType action = iMsg.Action;
			switch (action)
			{
			case ActionType.ConjureElement:
			{
				Elements param0I = (Elements)iMsg.Param0I;
				Spell spell;
				Spell.DefaultSpell(param0I, out spell);
				if ((param0I == Elements.Lightning & this.HasStatus(StatusEffects.Wet)) | (param0I == Elements.Fire & this.HasStatus(StatusEffects.Greased)))
				{
					DamageCollection5 iDamage;
					spell.CalculateDamage(SpellType.Shield, CastType.Self, out iDamage);
					this.Damage(iDamage, this, iMsg.TimeStamp, this.Position);
					return;
				}
				bool flag = SpellManager.Instance.TryAddToQueue(null, this, this.mSpellQueue, 5, ref spell);
				if (flag)
				{
					this.mChantedSpells++;
				}
				int iSoundIndex = Defines.SOUNDS_UI_ELEMENT[Spell.ElementIndex(spell.Element)];
				AudioManager.Instance.PlayCue(Banks.UI, iSoundIndex);
				return;
			}
			case ActionType.CastSpell:
			{
				CastType param0I2 = (CastType)iMsg.Param0I;
				if (param0I2 == CastType.Magick)
				{
					SpellManager.Instance.CombineMagick(this.Player, this.mPlayState.GameType, this.mSpellQueue);
					if (this.HasStatus(StatusEffects.Frozen) || this.mCurrentState is FlyingState)
					{
						this.CastSpell(true, "");
					}
				}
				else
				{
					this.mCastButtonPressed = param0I2;
				}
				if (param0I2 == CastType.None)
				{
					this.mChargeUnlocked = true;
					return;
				}
				this.CastType = param0I2;
				if (this.HasStatus(StatusEffects.Frozen) || this.mCurrentState is FlyingState || this.mCurrentState is PushedState || this.mCurrentState is KnockDownState)
				{
					this.CastSpell(true, "");
					return;
				}
				return;
			}
			case ActionType.Attack:
			case ActionType.Block:
				break;
			case ActionType.PickUp:
			{
				Pickable iPickable = Entity.GetFromHandle((int)iMsg.TargetHandle) as Pickable;
				this.InternalPickUp(iPickable);
				return;
			}
			case ActionType.PickUpRequest:
			{
				Pickable iPickable2 = Entity.GetFromHandle((int)iMsg.TargetHandle) as Pickable;
				this.PickUp(iPickable2);
				return;
			}
			case ActionType.Interact:
			{
				Character character = Entity.GetFromHandle((int)iMsg.TargetHandle) as Character;
				character.Interact(this, this.Player.Controller);
				return;
			}
			case ActionType.Boost:
				this.mBoosts++;
				if (base.Equipment[1].Item.PassiveAbility.Ability == Item.PassiveAbilities.ShieldBoost)
				{
					this.mBoosts++;
				}
				this.mBoostCooldown = 0.25f;
				if (base.CurrentState != BoostState.Instance)
				{
					this.mBoosts = 1;
				}
				for (int i = 0; i < this.mAuras.Count; i++)
				{
					AuraStorage aura = this.mAuras[i].Aura;
					if (aura.AuraType == AuraType.Boost)
					{
						this.mBoosts *= (int)aura.AuraBoost.Magnitude;
					}
				}
				for (int j = 0; j < base.Equipment[1].Item.Auras.Count; j++)
				{
					AuraStorage aura2 = base.Equipment[1].Item.Auras[j].Aura;
					if (aura2.AuraType == AuraType.Boost)
					{
						this.mBoosts *= (int)aura2.AuraBoost.Magnitude;
					}
				}
				for (int k = 0; k < this.mBuffs.Count; k++)
				{
					if (this.mBuffs[k].BuffType == BuffType.Boost)
					{
						this.mBoosts *= (int)this.mBuffs[k].BuffBoost.Amount;
					}
				}
				return;
			default:
				switch (action)
				{
				case ActionType.ItemSpecial:
					this.mEquipment[iMsg.Param0I].Item.ExecuteSpecialAbility();
					return;
				case ActionType.Magick:
				{
					MagickType param3I = (MagickType)iMsg.Param3I;
					if (param3I == MagickType.Polymorph)
					{
						Polymorph.PolymorphAvatar(this, iMsg.Param0I, ref Polymorph.sTemporaryDataHolder);
						return;
					}
					base.NetworkAction(ref iMsg);
					return;
				}
				case ActionType.EventComplete:
				{
					if (this.mEvents == null || this.CurrentEvent >= this.mEvents.Length)
					{
						return;
					}
					int num = iMsg.Param0I << 16;
					AIEventType aieventType = (AIEventType)(iMsg.Param0I >> 16);
					if (num == this.CurrentEvent && aieventType == this.mEvents[num].EventType)
					{
						switch (aieventType)
						{
						case AIEventType.Move:
							this.CurrentEventDelay = this.mEvents[num].MoveEvent.Delay;
							break;
						case AIEventType.Animation:
							this.CurrentEventDelay = this.mEvents[num].AnimationEvent.Delay;
							break;
						case AIEventType.Face:
							this.CurrentEventDelay = this.mEvents[num].FaceEvent.Delay;
							break;
						case AIEventType.Kill:
							this.CurrentEventDelay = this.mEvents[num].KillEvent.Delay;
							break;
						case AIEventType.Loop:
							this.CurrentEventDelay = this.mEvents[num].LoopEvent.Delay;
							break;
						}
						this.CurrentEvent++;
						return;
					}
					return;
				}
				}
				break;
			}
			base.NetworkAction(ref iMsg);
		}

		// Token: 0x17000AC2 RID: 2754
		// (get) Token: 0x06002DC0 RID: 11712 RVA: 0x00174123 File Offset: 0x00172323
		public Character Owner
		{
			get
			{
				return this;
			}
		}

		// Token: 0x17000AC3 RID: 2755
		// (get) Token: 0x06002DC1 RID: 11713 RVA: 0x00174126 File Offset: 0x00172326
		// (set) Token: 0x06002DC2 RID: 11714 RVA: 0x0017412E File Offset: 0x0017232E
		public bool IgnoreTriggers
		{
			get
			{
				return this.mIgnoreTriggers;
			}
			set
			{
				this.mIgnoreTriggers = value;
			}
		}

		// Token: 0x17000AC4 RID: 2756
		// (get) Token: 0x06002DC3 RID: 11715 RVA: 0x00174137 File Offset: 0x00172337
		// (set) Token: 0x06002DC4 RID: 11716 RVA: 0x00174140 File Offset: 0x00172340
		public AIEvent[] Events
		{
			get
			{
				return this.mEvents;
			}
			set
			{
				this.mEvents = value;
				if (value == null || value.Length == 0)
				{
					base.ChangeState(IdleState.Instance);
					base.GoToAnimation(Animations.idle, 0.05f);
				}
				this.CurrentEvent = 0;
				this.CurrentEventDelay = 0f;
				DialogManager.Instance.End(this);
			}
		}

		// Token: 0x17000AC5 RID: 2757
		// (get) Token: 0x06002DC5 RID: 11717 RVA: 0x00174191 File Offset: 0x00172391
		// (set) Token: 0x06002DC6 RID: 11718 RVA: 0x00174199 File Offset: 0x00172399
		public int CurrentEvent { get; set; }

		// Token: 0x17000AC6 RID: 2758
		// (get) Token: 0x06002DC7 RID: 11719 RVA: 0x001741A2 File Offset: 0x001723A2
		public bool LoopEvents
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000AC7 RID: 2759
		// (get) Token: 0x06002DC8 RID: 11720 RVA: 0x001741A5 File Offset: 0x001723A5
		// (set) Token: 0x06002DC9 RID: 11721 RVA: 0x001741AD File Offset: 0x001723AD
		public Vector3 WayPoint { get; set; }

		// Token: 0x17000AC8 RID: 2760
		// (get) Token: 0x06002DCA RID: 11722 RVA: 0x001741B6 File Offset: 0x001723B6
		// (set) Token: 0x06002DCB RID: 11723 RVA: 0x001741BE File Offset: 0x001723BE
		public float CurrentEventDelay { get; set; }

		// Token: 0x17000AC9 RID: 2761
		// (get) Token: 0x06002DCC RID: 11724 RVA: 0x001741C7 File Offset: 0x001723C7
		public List<PathNode> Path
		{
			get
			{
				return this.mPath;
			}
		}

		// Token: 0x17000ACA RID: 2762
		// (get) Token: 0x06002DCD RID: 11725 RVA: 0x001741CF File Offset: 0x001723CF
		// (set) Token: 0x06002DCE RID: 11726 RVA: 0x001741D7 File Offset: 0x001723D7
		public float CurrentStateAge { get; set; }

		// Token: 0x06002DCF RID: 11727 RVA: 0x001741E0 File Offset: 0x001723E0
		public void PushState(IAIState iNewState)
		{
			this.CurrentStateAge = 0f;
			IAIState iaistate = this.mStates.Peek();
			if (iNewState != iaistate)
			{
				iaistate.OnExit(this);
				this.mStates.Push(iNewState);
				iNewState.OnEnter(this);
			}
		}

		// Token: 0x06002DD0 RID: 11728 RVA: 0x00174224 File Offset: 0x00172424
		public void PopState()
		{
			this.CurrentStateAge = 0f;
			IAIState iaistate = this.mStates.Pop();
			iaistate.OnExit(this);
			this.mStates.Peek().OnEnter(this);
		}

		// Token: 0x06002DD1 RID: 11729 RVA: 0x00174260 File Offset: 0x00172460
		public void PickUp(Pickable iPickable)
		{
			if (this.Polymorphed)
			{
				return;
			}
			NetworkState state = NetworkManager.Instance.State;
			if (state == NetworkState.Client && !(this.Player.Gamer is NetworkGamer))
			{
				CharacterActionMessage characterActionMessage = default(CharacterActionMessage);
				characterActionMessage.Handle = base.Handle;
				characterActionMessage.Action = ActionType.PickUpRequest;
				characterActionMessage.TargetHandle = iPickable.Handle;
				NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref characterActionMessage, 0);
				return;
			}
			if (iPickable is Item && iPickable.Removable)
			{
				return;
			}
			if (state == NetworkState.Server)
			{
				CharacterActionMessage characterActionMessage2 = default(CharacterActionMessage);
				characterActionMessage2.Handle = base.Handle;
				characterActionMessage2.Action = ActionType.PickUp;
				characterActionMessage2.TargetHandle = iPickable.Handle;
				NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref characterActionMessage2);
			}
			this.InternalPickUp(iPickable);
		}

		// Token: 0x06002DD2 RID: 11730 RVA: 0x0017432C File Offset: 0x0017252C
		private void InternalPickUp(Pickable iPickable)
		{
			Item item = iPickable as Item;
			BookOfMagick bookOfMagick = iPickable as BookOfMagick;
			if (item != null)
			{
				this.mAllowAttackRotate = false;
				this.mAttacking = true;
				if (this.mPlayer != null && this.mPlayer.Controller != null && !(this.mPlayer.Controller is KeyboardMouseController))
				{
					TutorialManager.Instance.SetTip(TutorialManager.Tips.ItemPad, TutorialManager.Position.Top);
				}
				else
				{
					TutorialManager.Instance.SetTip(TutorialManager.Tips.ItemKey, TutorialManager.Position.Top);
				}
				if (item.OnPickup != 0)
				{
					this.mPlayState.Level.CurrentScene.ExecuteTrigger(item.OnPickup, this, false);
				}
				this.Player.ObtainedTextBox.Initialize(this.mPlayState.Scene, MagickaFont.Maiandra14, item.PickUpString, default(Vector2), true, this, 2f);
				if (item.WeaponClass == WeaponClass.Staff)
				{
					Item item2 = this.mEquipment[1].Item;
					if (item2.IsBound)
					{
						return;
					}
					Item.Swap(item2, item);
					this.mNextAttackAnimation = Animations.pickup_staff;
					if (item2.DespawnTime > 0f)
					{
						item.Despawn(20f);
					}
					item2.Despawn(0f);
					Vector3 vector;
					Quaternion quaternion;
					Vector3 vector2;
					item2.Transform.Decompose(out vector, out quaternion, out vector2);
					Matrix matrix;
					Matrix.CreateFromQuaternion(ref quaternion, out matrix);
					item.Body.MoveTo(vector2, matrix);
					item.Body.EnableBody();
					matrix.Translation = vector2;
					item.Transform = matrix;
					this.Player.Staff = this.mEquipment[1].Item.Name;
					if (this.mEquipment[1].Item.HasSpecialAbility)
					{
						this.mSpecialAbilityName = LanguageManager.Instance.GetString(this.mEquipment[1].Item.SpecialAbilityName);
						this.mShowStaffSpecialAbilityNotifierTimer = 3f;
					}
				}
				else
				{
					Item item3 = this.mEquipment[0].Item;
					if (item3.IsBound)
					{
						return;
					}
					Item.Swap(item3, item);
					this.mNextAttackAnimation = Animations.pickup_weapon;
					this.mHeavyWeaponReload = false;
					if (item3.DespawnTime > 0f)
					{
						item.Despawn(20f);
					}
					item3.Despawn(0f);
					Vector3 vector3;
					Quaternion quaternion2;
					Vector3 vector4;
					item3.Transform.Decompose(out vector3, out quaternion2, out vector4);
					Matrix matrix2;
					Matrix.CreateFromQuaternion(ref quaternion2, out matrix2);
					item.Body.MoveTo(vector4, matrix2);
					item.Body.EnableBody();
					matrix2.Translation = vector4;
					item.Transform = matrix2;
					this.Player.Weapon = this.mEquipment[0].Item.Name;
				}
				DialogManager.Instance.AddTextBox(this.Player.ObtainedTextBox);
				if (item.PreviousOwner is Avatar && (item.PreviousOwner as Avatar).UniqueID != this.mUniqueID && !(this.Player.Gamer is NetworkGamer))
				{
					AchievementsManager.Instance.AwardAchievement(base.PlayState, "finderskeepers");
				}
				item.PreviousOwner = null;
				item.Body.Immovable = false;
				return;
			}
			if (bookOfMagick != null)
			{
				bookOfMagick.Unlock(this.Player);
				this.Player.IconRenderer.TomeMagick = bookOfMagick.Magick;
				this.Player.ObtainedTextBox.Initialize(this.mPlayState.Scene, MagickaFont.Maiandra14, bookOfMagick.PickUpString, default(Vector2), true, this, 2f);
				if (bookOfMagick.OnPickup != 0)
				{
					this.mPlayState.Level.CurrentScene.ExecuteTrigger(bookOfMagick.OnPickup, this, false);
				}
				DialogManager.Instance.AddTextBox(this.Player.ObtainedTextBox);
				this.mAllowAttackRotate = false;
				this.mAttacking = true;
				this.mNextAttackAnimation = Animations.pickup_magick;
			}
		}

		// Token: 0x06002DD3 RID: 11731 RVA: 0x001746E0 File Offset: 0x001728E0
		public override DamageResult InternalDamage(Damage iDamage, Entity iAttacker, double iTimeStamp, Vector3 iAttackPosition, Defines.DamageFeatures iFeatures)
		{
			DamageResult damageResult = base.InternalDamage(iDamage, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
			if (this.mPlayer != null && !(this.mPlayer.Gamer is NetworkGamer) && (damageResult & DamageResult.OverKilled) == DamageResult.None && ((damageResult & DamageResult.Damaged) == DamageResult.Damaged || (damageResult & DamageResult.Hit) == DamageResult.Hit) && this.mPlayer.Controller != null)
			{
				this.mPlayer.Controller.Rumble(0.5f, 0.5f);
			}
			return damageResult;
		}

		// Token: 0x06002DD4 RID: 11732 RVA: 0x00174752 File Offset: 0x00172952
		internal override float GetDanger()
		{
			if (!(this.mCurrentState is ChargeState))
			{
				return base.GetDanger();
			}
			return 10f;
		}

		// Token: 0x06002DD5 RID: 11733 RVA: 0x00174770 File Offset: 0x00172970
		internal virtual void GetPolymorphValues(out Polymorph.AvatarPolymorphData iData)
		{
			iData = default(Polymorph.AvatarPolymorphData);
			iData.Type = this.mType;
			iData.Faction = this.Faction;
			iData.NormalizedHP = this.mHitPoints / this.mMaxHitPoints;
			iData.TimeSinceLastDamage = this.mTimeSinceLastDamage;
			iData.TimeSinceLastStatusDamage = this.mTimeSinceLastStatusDamage;
			iData.DesiredDirection = base.CharacterBody.DesiredDirection;
			iData.IsEthereal = base.IsEthereal;
			iData.FearedBy = this.mFearedBy;
			iData.FearPosition = this.mFearPosition;
			iData.FearTimer = this.mFearTimer;
			iData.CharmOwner = this.mCharmOwner;
			iData.CharmEffect = this.mCharmEffect;
			iData.CharmTimer = this.mCharmTimer;
			iData.Hypnotized = this.mHypnotized;
			iData.HypnotizeEffect = this.mHypnotizeEffect;
			iData.HypnotizeDirection = this.mHypnotizeDirection;
			StatusEffect[] sTempStatusEffects = Polymorph.AvatarPolymorphData.sTempStatusEffects;
			Array.Copy(this.mStatusEffects, sTempStatusEffects, this.mStatusEffects.Length);
		}

		// Token: 0x06002DD6 RID: 11734 RVA: 0x0017486C File Offset: 0x00172A6C
		internal void ApplyPolymorphValues(ref Polymorph.AvatarPolymorphData iData)
		{
			this.mType = iData.Type;
			this.Faction = iData.Faction;
			this.mHitPoints = iData.NormalizedHP * this.mMaxHitPoints;
			this.mTimeSinceLastDamage = iData.TimeSinceLastDamage;
			this.mTimeSinceLastStatusDamage = iData.TimeSinceLastStatusDamage;
			base.CharacterBody.DesiredDirection = iData.DesiredDirection;
			if (iData.IsEthereal)
			{
				base.Ethereal(true, 1f, 1f);
			}
			this.mFearedBy = iData.FearedBy;
			this.mFearPosition = iData.FearPosition;
			this.mFearTimer = iData.FearTimer;
			this.mCharmOwner = iData.CharmOwner;
			this.mCharmEffect = iData.CharmEffect;
			this.mCharmTimer = iData.CharmTimer;
			this.mHypnotized = iData.Hypnotized;
			this.mHypnotizeEffect = iData.HypnotizeEffect;
			this.mHypnotizeDirection = iData.HypnotizeDirection;
			StatusEffect[] sTempStatusEffects = Polymorph.AvatarPolymorphData.sTempStatusEffects;
			for (int i = 0; i < sTempStatusEffects.Length; i++)
			{
				base.AddStatusEffect(sTempStatusEffects[i]);
			}
		}

		// Token: 0x06002DD7 RID: 11735 RVA: 0x00174979 File Offset: 0x00172B79
		internal override bool SendsNetworkUpdate(NetworkState iState)
		{
			return (this.Player != null && !(this.Player.Gamer is NetworkGamer)) || iState == NetworkState.Server;
		}

		// Token: 0x06002DD8 RID: 11736 RVA: 0x0017499B File Offset: 0x00172B9B
		protected override void INetworkUpdate(ref EntityUpdateMessage iMsg)
		{
			if (this.Player == null)
			{
				return;
			}
			base.INetworkUpdate(ref iMsg);
		}

		// Token: 0x06002DD9 RID: 11737 RVA: 0x001749B0 File Offset: 0x00172BB0
		protected override void IGetNetworkUpdate(out EntityUpdateMessage oMsg, float iPrediction)
		{
			base.IGetNetworkUpdate(out oMsg, iPrediction);
			if (this.Player.Gamer is NetworkGamer)
			{
				oMsg.Features &= ~EntityFeatures.Position;
				oMsg.Features &= ~EntityFeatures.Direction;
				oMsg.Features &= ~EntityFeatures.GenericFloat;
				oMsg.Features &= ~EntityFeatures.Etherealized;
			}
			if (NetworkManager.Instance.State != NetworkState.Server)
			{
				oMsg.Features &= ~EntityFeatures.Damageable;
				oMsg.Features &= ~EntityFeatures.StatusEffected;
				oMsg.Features &= ~EntityFeatures.SelfShield;
			}
		}

		// Token: 0x06002DDA RID: 11738 RVA: 0x00174A69 File Offset: 0x00172C69
		public void ForceSetNotDead()
		{
			this.mDead = false;
		}

		// Token: 0x06002DDB RID: 11739 RVA: 0x00174A74 File Offset: 0x00172C74
		internal void RequestForcedSyncingOfPlayers()
		{
			if (this.mPlayer == null)
			{
				return;
			}
			if (NetworkManager.Instance.State == NetworkState.Server)
			{
				return;
			}
			RequestForcedPlayerStatusSync requestForcedPlayerStatusSync = default(RequestForcedPlayerStatusSync);
			requestForcedPlayerStatusSync.Handle = (ushort)this.mPlayer.ID;
			NetworkManager.Instance.Interface.SendMessage<RequestForcedPlayerStatusSync>(ref requestForcedPlayerStatusSync, 0, P2PSend.Reliable);
		}

		// Token: 0x04003175 RID: 12661
		private static List<Avatar> mCache = new List<Avatar>(16);

		// Token: 0x04003176 RID: 12662
		public static readonly int WIZARDHASH = "wizard".GetHashCodeCustom();

		// Token: 0x04003177 RID: 12663
		private static readonly int UNARMED = "weapon_unarmed".GetHashCodeCustom();

		// Token: 0x04003178 RID: 12664
		private Avatar.OutlineRenderData[] mOutlineRenderData;

		// Token: 0x04003179 RID: 12665
		private Avatar.AfterImageRenderData[] mAfterImageRenderData;

		// Token: 0x0400317A RID: 12666
		private Avatar.TargettingRenderData[] mTargettingRenderData;

		// Token: 0x0400317B RID: 12667
		private float mDeadAge;

		// Token: 0x0400317C RID: 12668
		private Player mPlayer;

		// Token: 0x0400317D RID: 12669
		private Stack<CharacterActionMessage> mQueuedNetActions = new Stack<CharacterActionMessage>(16);

		// Token: 0x0400317E RID: 12670
		private int mChantedSpells;

		// Token: 0x0400317F RID: 12671
		private CastType mCastButtonPressed;

		// Token: 0x04003180 RID: 12672
		private bool mLeftTriggerActive;

		// Token: 0x04003181 RID: 12673
		private bool mRightTriggerActive;

		// Token: 0x04003182 RID: 12674
		private bool mCanChant;

		// Token: 0x04003183 RID: 12675
		private ControllerDirection mChantDirection;

		// Token: 0x04003184 RID: 12676
		private bool mMove;

		// Token: 0x04003185 RID: 12677
		private bool mPolymorphed;

		// Token: 0x04003186 RID: 12678
		private int mBoosts;

		// Token: 0x04003187 RID: 12679
		private float mBoostCooldown;

		// Token: 0x04003188 RID: 12680
		private bool mChantingMagick;

		// Token: 0x04003189 RID: 12681
		private float mShowStaffSpecialAbilityNotifierTimer;

		// Token: 0x0400318A RID: 12682
		private string mSpecialAbilityName;

		// Token: 0x0400318B RID: 12683
		private Queue<MissileEntity> mMissileCache = new Queue<MissileEntity>(32);

		// Token: 0x0400318C RID: 12684
		private Vector3 mDesiredInputDirection;

		// Token: 0x0400318D RID: 12685
		private bool mChargeUnlocked = true;

		// Token: 0x0400318E RID: 12686
		private float mAfterImageTimer;

		// Token: 0x0400318F RID: 12687
		private float mAfterImageIntensity;

		// Token: 0x04003190 RID: 12688
		private float mTargettingAlpha;

		// Token: 0x04003191 RID: 12689
		private bool mHeavyWeaponReload;

		// Token: 0x04003192 RID: 12690
		private Fairy mFairy;

		// Token: 0x04003193 RID: 12691
		private bool mIgnoreTriggers;

		// Token: 0x04003194 RID: 12692
		private Stack<IAIState> mStates = new Stack<IAIState>(5);

		// Token: 0x04003195 RID: 12693
		private AIEvent[] mEvents;

		// Token: 0x04003196 RID: 12694
		private List<PathNode> mPath = new List<PathNode>(512);

		// Token: 0x020005EB RID: 1515
		protected class OutlineRenderData : IProjectionObject
		{
			// Token: 0x17000ACB RID: 2763
			// (get) Token: 0x06002DDD RID: 11741 RVA: 0x00174AF2 File Offset: 0x00172CF2
			public int Effect
			{
				get
				{
					return this.mEffect;
				}
			}

			// Token: 0x17000ACC RID: 2764
			// (get) Token: 0x06002DDE RID: 11742 RVA: 0x00174AFA File Offset: 0x00172CFA
			public int Technique
			{
				get
				{
					return 0;
				}
			}

			// Token: 0x17000ACD RID: 2765
			// (get) Token: 0x06002DDF RID: 11743 RVA: 0x00174AFD File Offset: 0x00172CFD
			public VertexBuffer Vertices
			{
				get
				{
					return this.mVertexBuffer;
				}
			}

			// Token: 0x17000ACE RID: 2766
			// (get) Token: 0x06002DE0 RID: 11744 RVA: 0x00174B05 File Offset: 0x00172D05
			public IndexBuffer Indices
			{
				get
				{
					return this.mIndexBuffer;
				}
			}

			// Token: 0x17000ACF RID: 2767
			// (get) Token: 0x06002DE1 RID: 11745 RVA: 0x00174B0D File Offset: 0x00172D0D
			public VertexDeclaration VertexDeclaration
			{
				get
				{
					return this.mVertexDeclaration;
				}
			}

			// Token: 0x17000AD0 RID: 2768
			// (get) Token: 0x06002DE2 RID: 11746 RVA: 0x00174B15 File Offset: 0x00172D15
			public int VertexStride
			{
				get
				{
					return this.mVertexStride;
				}
			}

			// Token: 0x17000AD1 RID: 2769
			// (get) Token: 0x06002DE3 RID: 11747 RVA: 0x00174B1D File Offset: 0x00172D1D
			public int VerticesHashCode
			{
				get
				{
					return this.mVerticesHash;
				}
			}

			// Token: 0x06002DE4 RID: 11748 RVA: 0x00174B28 File Offset: 0x00172D28
			public bool Cull(BoundingFrustum iViewFrustum)
			{
				BoundingSphere boundingSphere = this.mBoundingSphere;
				return boundingSphere.Contains(iViewFrustum) == ContainmentType.Disjoint;
			}

			// Token: 0x06002DE5 RID: 11749 RVA: 0x00174B48 File Offset: 0x00172D48
			public void Draw(Effect iEffect, Texture2D iDepthMap)
			{
				SkinnedModelDeferredEffect skinnedModelDeferredEffect = iEffect as SkinnedModelDeferredEffect;
				float rimLightBias = skinnedModelDeferredEffect.RimLightBias;
				float rimLightGlow = skinnedModelDeferredEffect.RimLightGlow;
				skinnedModelDeferredEffect.RimLightBias = 0f;
				skinnedModelDeferredEffect.RimLightGlow = 0f;
				skinnedModelDeferredEffect.DiffuseMap0Enabled = false;
				skinnedModelDeferredEffect.DiffuseMap1Enabled = false;
				skinnedModelDeferredEffect.SpecularMapEnabled = false;
				skinnedModelDeferredEffect.Bloat = -0.0333f;
				skinnedModelDeferredEffect.Bones = this.mSkeleton;
				skinnedModelDeferredEffect.SpecularAmount = 0f;
				skinnedModelDeferredEffect.GraphicsDevice.RenderState.AlphaBlendEnable = false;
				skinnedModelDeferredEffect.GraphicsDevice.RenderState.ColorWriteChannels = ColorWriteChannels.None;
				skinnedModelDeferredEffect.GraphicsDevice.RenderState.ColorWriteChannels1 = ColorWriteChannels.None;
				if (this.Forced)
				{
					skinnedModelDeferredEffect.GraphicsDevice.RenderState.DepthBufferFunction = CompareFunction.Always;
					skinnedModelDeferredEffect.GraphicsDevice.RenderState.DepthBufferWriteEnable = true;
					skinnedModelDeferredEffect.GraphicsDevice.RenderState.DepthBias = -0.1f;
				}
				else
				{
					skinnedModelDeferredEffect.GraphicsDevice.RenderState.DepthBufferFunction = CompareFunction.Greater;
					skinnedModelDeferredEffect.GraphicsDevice.RenderState.DepthBufferWriteEnable = false;
					skinnedModelDeferredEffect.GraphicsDevice.RenderState.StencilFunction = CompareFunction.Equal;
					skinnedModelDeferredEffect.GraphicsDevice.RenderState.StencilPass = StencilOperation.Increment;
					skinnedModelDeferredEffect.GraphicsDevice.RenderState.ReferenceStencil = 1;
				}
				skinnedModelDeferredEffect.CommitChanges();
				skinnedModelDeferredEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
				skinnedModelDeferredEffect.GraphicsDevice.RenderState.ColorWriteChannels = ColorWriteChannels.All;
				skinnedModelDeferredEffect.GraphicsDevice.RenderState.ColorWriteChannels1 = ColorWriteChannels.All;
				skinnedModelDeferredEffect.DiffuseColor = this.Color;
				skinnedModelDeferredEffect.EmissiveAmount = 1f;
				skinnedModelDeferredEffect.Bloat = 0f;
				if (this.Forced)
				{
					skinnedModelDeferredEffect.GraphicsDevice.RenderState.DepthBufferFunction = CompareFunction.LessEqual;
					skinnedModelDeferredEffect.GraphicsDevice.RenderState.DepthBias = 0f;
				}
				else
				{
					skinnedModelDeferredEffect.GraphicsDevice.RenderState.StencilFunction = CompareFunction.Equal;
					skinnedModelDeferredEffect.GraphicsDevice.RenderState.StencilPass = StencilOperation.Keep;
					skinnedModelDeferredEffect.GraphicsDevice.RenderState.ReferenceStencil = 1;
					skinnedModelDeferredEffect.GraphicsDevice.RenderState.DepthBias = -0.0025f;
				}
				skinnedModelDeferredEffect.CommitChanges();
				skinnedModelDeferredEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
				skinnedModelDeferredEffect.GraphicsDevice.RenderState.DepthBias = 0f;
				skinnedModelDeferredEffect.Bloat = 0f;
				skinnedModelDeferredEffect.RimLightBias = rimLightBias;
				skinnedModelDeferredEffect.RimLightGlow = rimLightGlow;
				skinnedModelDeferredEffect.GraphicsDevice.RenderState.DepthBufferWriteEnable = false;
				skinnedModelDeferredEffect.GraphicsDevice.RenderState.DepthBufferFunction = CompareFunction.LessEqual;
				skinnedModelDeferredEffect.GraphicsDevice.RenderState.StencilPass = StencilOperation.Keep;
				skinnedModelDeferredEffect.GraphicsDevice.RenderState.StencilFunction = CompareFunction.Always;
				skinnedModelDeferredEffect.GraphicsDevice.RenderState.ReferenceStencil = 0;
				skinnedModelDeferredEffect.GraphicsDevice.RenderState.ColorWriteChannels = (ColorWriteChannels.Red | ColorWriteChannels.Green | ColorWriteChannels.Blue);
				skinnedModelDeferredEffect.GraphicsDevice.RenderState.ColorWriteChannels1 = (ColorWriteChannels.Red | ColorWriteChannels.Green | ColorWriteChannels.Blue);
				skinnedModelDeferredEffect.GraphicsDevice.RenderState.AlphaBlendEnable = true;
			}

			// Token: 0x17000AD2 RID: 2770
			// (get) Token: 0x06002DE6 RID: 11750 RVA: 0x00174E4A File Offset: 0x0017304A
			public bool MeshDirty
			{
				get
				{
					return this.mMeshDirty;
				}
			}

			// Token: 0x06002DE7 RID: 11751 RVA: 0x00174E52 File Offset: 0x00173052
			public void SetMeshDirty()
			{
				this.mMeshDirty = true;
			}

			// Token: 0x06002DE8 RID: 11752 RVA: 0x00174E5C File Offset: 0x0017305C
			public void SetMesh(VertexBuffer iVertices, IndexBuffer iIndices, ModelMeshPart iMeshPart, int iEffectHash)
			{
				this.mMeshDirty = false;
				this.mVertexBuffer = iVertices;
				this.mVerticesHash = iVertices.GetHashCode();
				this.mIndexBuffer = iIndices;
				this.mEffect = iEffectHash;
				this.mVertexDeclaration = iMeshPart.VertexDeclaration;
				this.mBaseVertex = iMeshPart.BaseVertex;
				this.mNumVertices = iMeshPart.NumVertices;
				this.mPrimitiveCount = iMeshPart.PrimitiveCount;
				this.mStartIndex = iMeshPart.StartIndex;
				this.mStreamOffset = iMeshPart.StreamOffset;
				this.mVertexStride = iMeshPart.VertexStride;
			}

			// Token: 0x0400319B RID: 12699
			public BoundingSphere mBoundingSphere;

			// Token: 0x0400319C RID: 12700
			protected int mEffect;

			// Token: 0x0400319D RID: 12701
			protected VertexDeclaration mVertexDeclaration;

			// Token: 0x0400319E RID: 12702
			protected int mBaseVertex;

			// Token: 0x0400319F RID: 12703
			protected int mNumVertices;

			// Token: 0x040031A0 RID: 12704
			protected int mPrimitiveCount;

			// Token: 0x040031A1 RID: 12705
			protected int mStartIndex;

			// Token: 0x040031A2 RID: 12706
			protected int mStreamOffset;

			// Token: 0x040031A3 RID: 12707
			protected int mVertexStride;

			// Token: 0x040031A4 RID: 12708
			protected VertexBuffer mVertexBuffer;

			// Token: 0x040031A5 RID: 12709
			protected IndexBuffer mIndexBuffer;

			// Token: 0x040031A6 RID: 12710
			public Vector3 Color;

			// Token: 0x040031A7 RID: 12711
			public Matrix[] mSkeleton;

			// Token: 0x040031A8 RID: 12712
			public bool Forced;

			// Token: 0x040031A9 RID: 12713
			protected int mVerticesHash;

			// Token: 0x040031AA RID: 12714
			protected bool mMeshDirty = true;
		}

		// Token: 0x020005EC RID: 1516
		protected class AfterImageRenderData : IRenderableAdditiveObject
		{
			// Token: 0x06002DEA RID: 11754 RVA: 0x00174EF5 File Offset: 0x001730F5
			public AfterImageRenderData(Matrix[][] iSkeleton)
			{
				this.mSkeleton = iSkeleton;
			}

			// Token: 0x17000AD3 RID: 2771
			// (get) Token: 0x06002DEB RID: 11755 RVA: 0x00174F0B File Offset: 0x0017310B
			public bool MeshDirty
			{
				get
				{
					return this.mMeshDirty;
				}
			}

			// Token: 0x17000AD4 RID: 2772
			// (get) Token: 0x06002DEC RID: 11756 RVA: 0x00174F13 File Offset: 0x00173113
			public int Effect
			{
				get
				{
					return SkinnedModelDeferredEffect.TYPEHASH;
				}
			}

			// Token: 0x17000AD5 RID: 2773
			// (get) Token: 0x06002DED RID: 11757 RVA: 0x00174F1A File Offset: 0x0017311A
			public int Technique
			{
				get
				{
					return 2;
				}
			}

			// Token: 0x17000AD6 RID: 2774
			// (get) Token: 0x06002DEE RID: 11758 RVA: 0x00174F1D File Offset: 0x0017311D
			public VertexBuffer Vertices
			{
				get
				{
					return this.mVertexBuffer;
				}
			}

			// Token: 0x17000AD7 RID: 2775
			// (get) Token: 0x06002DEF RID: 11759 RVA: 0x00174F25 File Offset: 0x00173125
			public IndexBuffer Indices
			{
				get
				{
					return this.mIndexBuffer;
				}
			}

			// Token: 0x17000AD8 RID: 2776
			// (get) Token: 0x06002DF0 RID: 11760 RVA: 0x00174F2D File Offset: 0x0017312D
			public VertexDeclaration VertexDeclaration
			{
				get
				{
					return this.mVertexDeclaration;
				}
			}

			// Token: 0x17000AD9 RID: 2777
			// (get) Token: 0x06002DF1 RID: 11761 RVA: 0x00174F35 File Offset: 0x00173135
			public int VertexStride
			{
				get
				{
					return this.mVertexStride;
				}
			}

			// Token: 0x17000ADA RID: 2778
			// (get) Token: 0x06002DF2 RID: 11762 RVA: 0x00174F3D File Offset: 0x0017313D
			public int VerticesHashCode
			{
				get
				{
					return this.mVerticesHash;
				}
			}

			// Token: 0x06002DF3 RID: 11763 RVA: 0x00174F48 File Offset: 0x00173148
			public bool Cull(BoundingFrustum iViewFrustum)
			{
				BoundingSphere boundingSphere = this.mBoundingSphere;
				return boundingSphere.Contains(iViewFrustum) == ContainmentType.Disjoint;
			}

			// Token: 0x06002DF4 RID: 11764 RVA: 0x00174F68 File Offset: 0x00173168
			public void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				if (iEffect.GraphicsDevice.RenderState.ColorWriteChannels == ColorWriteChannels.Alpha)
				{
					iEffect.GraphicsDevice.RenderState.ColorWriteChannels = ColorWriteChannels.All;
					iEffect.GraphicsDevice.RenderState.AlphaBlendEnable = true;
					return;
				}
				SkinnedModelDeferredEffect skinnedModelDeferredEffect = iEffect as SkinnedModelDeferredEffect;
				skinnedModelDeferredEffect.GraphicsDevice.RenderState.DestinationBlend = Blend.One;
				skinnedModelDeferredEffect.GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha;
				this.mMaterial.AssignToEffect(skinnedModelDeferredEffect);
				skinnedModelDeferredEffect.Colorize = new Vector4(Character.ColdColor, 1f);
				float num = 0.333f;
				float num2 = 0.333f / ((float)this.mSkeleton.Length + 1f);
				num += this.mIntensity * num2;
				int num3 = 0;
				while (num3 < this.mSkeleton.Length && num > 0f)
				{
					if (num3 != 0)
					{
						skinnedModelDeferredEffect.Alpha = num;
						skinnedModelDeferredEffect.Bones = this.mSkeleton[num3];
						skinnedModelDeferredEffect.CommitChanges();
						skinnedModelDeferredEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
						num -= num2;
					}
					num3++;
				}
				skinnedModelDeferredEffect.Colorize = default(Vector4);
			}

			// Token: 0x06002DF5 RID: 11765 RVA: 0x0017508F File Offset: 0x0017328F
			public void SetMeshDirty()
			{
				this.mMeshDirty = true;
			}

			// Token: 0x06002DF6 RID: 11766 RVA: 0x00175098 File Offset: 0x00173298
			public void SetMesh(VertexBuffer iVertices, IndexBuffer iIndices, ModelMeshPart iMeshPart)
			{
				this.mMeshDirty = false;
				Helper.SkinnedModelDeferredMaterialFromBasicEffect(iMeshPart.Effect as SkinnedModelBasicEffect, out this.mMaterial);
				this.mVertexBuffer = iVertices;
				this.mVerticesHash = iVertices.GetHashCode();
				this.mIndexBuffer = iIndices;
				this.mVertexDeclaration = iMeshPart.VertexDeclaration;
				this.mBaseVertex = iMeshPart.BaseVertex;
				this.mNumVertices = iMeshPart.NumVertices;
				this.mPrimitiveCount = iMeshPart.PrimitiveCount;
				this.mStartIndex = iMeshPart.StartIndex;
				this.mStreamOffset = iMeshPart.StreamOffset;
				this.mVertexStride = iMeshPart.VertexStride;
				for (int i = 0; i < this.mSkeleton.Length; i++)
				{
					Matrix[] array = this.mSkeleton[i];
					for (int j = 0; j < array.Length; j++)
					{
						array[j].M11 = (array[j].M22 = (array[j].M33 = (array[j].M44 = float.NaN)));
					}
				}
			}

			// Token: 0x040031AB RID: 12715
			public BoundingSphere mBoundingSphere;

			// Token: 0x040031AC RID: 12716
			protected VertexDeclaration mVertexDeclaration;

			// Token: 0x040031AD RID: 12717
			protected int mBaseVertex;

			// Token: 0x040031AE RID: 12718
			protected int mNumVertices;

			// Token: 0x040031AF RID: 12719
			protected int mPrimitiveCount;

			// Token: 0x040031B0 RID: 12720
			protected int mStartIndex;

			// Token: 0x040031B1 RID: 12721
			protected int mStreamOffset;

			// Token: 0x040031B2 RID: 12722
			protected int mVertexStride;

			// Token: 0x040031B3 RID: 12723
			protected VertexBuffer mVertexBuffer;

			// Token: 0x040031B4 RID: 12724
			protected IndexBuffer mIndexBuffer;

			// Token: 0x040031B5 RID: 12725
			public float mIntensity;

			// Token: 0x040031B6 RID: 12726
			public Vector3 Color;

			// Token: 0x040031B7 RID: 12727
			public Matrix[][] mSkeleton;

			// Token: 0x040031B8 RID: 12728
			private SkinnedModelDeferredBasicMaterial mMaterial;

			// Token: 0x040031B9 RID: 12729
			protected int mVerticesHash;

			// Token: 0x040031BA RID: 12730
			protected bool mMeshDirty = true;
		}

		// Token: 0x020005ED RID: 1517
		protected class TargettingRenderData : ProjectionObject
		{
			// Token: 0x06002DF7 RID: 11767 RVA: 0x0017519C File Offset: 0x0017339C
			public TargettingRenderData(GraphicsDevice iDevice, Texture2D iTexture, Texture2D iNormalMap) : base(iDevice, iTexture, iNormalMap)
			{
			}

			// Token: 0x06002DF8 RID: 11768 RVA: 0x001751A8 File Offset: 0x001733A8
			public override void Draw(Effect iEffect, Texture2D iDepthMap)
			{
				ProjectionEffect projectionEffect = iEffect as ProjectionEffect;
				projectionEffect.DepthMap = iDepthMap;
				projectionEffect.PixelSize = new Vector2(1f / (float)iDepthMap.Width, 1f / (float)iDepthMap.Height);
				projectionEffect.Alpha = this.Alpha;
				projectionEffect.GraphicsDevice.RenderState.DepthBufferWriteEnable = false;
				projectionEffect.GraphicsDevice.RenderState.ReferenceStencil = 1;
				projectionEffect.GraphicsDevice.RenderState.StencilFunction = CompareFunction.Equal;
				base.Draw(iEffect, iDepthMap);
				projectionEffect.GraphicsDevice.RenderState.ReferenceStencil = 3;
				base.Draw(iEffect, iDepthMap);
				projectionEffect.GraphicsDevice.RenderState.DepthBufferWriteEnable = true;
				projectionEffect.GraphicsDevice.RenderState.ReferenceStencil = 1;
			}

			// Token: 0x040031BB RID: 12731
			public float Alpha;
		}
	}
}
