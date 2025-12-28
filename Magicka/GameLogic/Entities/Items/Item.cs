using System;
using System.Collections.Generic;
using System.Globalization;
using JigLibX.Geometry;
using JigLibX.Math;
using Magicka.Achievements;
using Magicka.Audio;
using Magicka.GameLogic.Entities.Abilities;
using Magicka.GameLogic.Entities.Abilities.SpecialAbilities;
using Magicka.GameLogic.Entities.AnimationActions;
using Magicka.GameLogic.Entities.Bosses;
using Magicka.GameLogic.Entities.Buffs;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Gamers;
using Magicka.Graphics;
using Magicka.Graphics.Lights;
using Magicka.Levels;
using Magicka.Levels.Versus;
using Magicka.Localization;
using Magicka.Network;
using Magicka.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using PolygonHead.Lights;

namespace Magicka.GameLogic.Entities.Items
{
	// Token: 0x02000359 RID: 857
	public class Item : Pickable
	{
		// Token: 0x06001A01 RID: 6657 RVA: 0x000AD8B8 File Offset: 0x000ABAB8
		public static Item GetPickableIntstance()
		{
			Item item = Item.sPickableCache.Dequeue();
			Item.sPickableCache.Enqueue(item);
			return item;
		}

		// Token: 0x06001A02 RID: 6658 RVA: 0x000AD8DC File Offset: 0x000ABADC
		public static void InitializePickableCache(int iNr, PlayState iPlayState)
		{
			Item.sPickableCache = new Queue<Item>(iNr);
			for (int i = 0; i < iNr; i++)
			{
				Item.sPickableCache.Enqueue(new Item(iPlayState, null));
			}
		}

		// Token: 0x06001A03 RID: 6659 RVA: 0x000AD911 File Offset: 0x000ABB11
		public static void GetCachedWeapon(int iType, Item iTarget)
		{
			if (Item.CachedWeapons.ContainsKey(iType))
			{
				Item.CachedWeapons[iType].Copy(iTarget);
				return;
			}
			throw new Exception("Weapon not cached");
		}

		// Token: 0x06001A04 RID: 6660 RVA: 0x000AD93C File Offset: 0x000ABB3C
		public static Item GetCachedWeapon(int iType)
		{
			if (Item.CachedWeapons.ContainsKey(iType))
			{
				return Item.CachedWeapons[iType];
			}
			throw new Exception("Weapon not cached");
		}

		// Token: 0x06001A05 RID: 6661 RVA: 0x000AD961 File Offset: 0x000ABB61
		public static void CacheWeapon(int iType, Item iItem)
		{
			Item.CachedWeapons[iType] = iItem;
		}

		// Token: 0x06001A06 RID: 6662 RVA: 0x000AD96F File Offset: 0x000ABB6F
		public static void ClearCache()
		{
			Item.CachedWeapons.Clear();
		}

		// Token: 0x06001A07 RID: 6663 RVA: 0x000AD97C File Offset: 0x000ABB7C
		public Item(PlayState iPlayState, Character iOwner) : base(iPlayState)
		{
			if (iPlayState != null)
			{
				this.mElementRenderData = new Item.ElementRenderData[3];
				for (int i = 0; i < 3; i++)
				{
					Item.RenderData renderData = new Item.RenderData();
					this.mRenderData[i] = renderData;
					this.mElementRenderData[i] = new Item.ElementRenderData();
				}
				this.mOwner = iOwner;
				this.mHitlist = new List<IDamageable>(64);
			}
			this.mMeleeConditions = new ConditionCollection();
			this.mRangedConditions = new ConditionCollection();
			this.mGunConditions = new ConditionCollection();
			this.mSpellQueue = new StaticEquatableList<Spell>(5);
			this.mSpellEffects = new VisualEffectReference[11];
			for (int j = 0; j < this.mSpellEffects.Length; j++)
			{
				this.mSpellEffects[j].ID = -1;
			}
			this.mAttached = true;
			this.mBound = false;
			this.mAnimationDetached = false;
		}

		// Token: 0x06001A08 RID: 6664 RVA: 0x000ADAC4 File Offset: 0x000ABCC4
		public override void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			if (!this.mAttached)
			{
				this.mVisible = true;
				if (!this.mPickable)
				{
					this.mTimeToRemove -= iDeltaTime;
				}
			}
			if (this.mType == Item.TYRFING_TYPE)
			{
				Math.Max(0f, this.mTeleportCooldown -= iDeltaTime);
			}
			if (this.mTimeToRemove < 0f)
			{
				this.Deinitialize();
			}
			this.mShieldDeflectCoolDown -= iDeltaTime;
			this.mCooldownTimer -= iDeltaTime;
			this.mWorldCollisionTimer -= iDeltaTime;
			if (this.mPassiveAbility.Ability == Item.PassiveAbilities.Gungner)
			{
				Vector3 position = this.Position;
				Vector3 up = this.mBody.Orientation.Up;
				if (this.mGungnirMissile != null)
				{
					if (this.mGungnirMissile.Dead)
					{
						EffectManager.Instance.UpdatePositionDirection(ref this.mPassiveAbilityEffect, ref position, ref up);
						this.mGungnirMissile = null;
						this.mVisible = true;
					}
					else
					{
						if (!EffectManager.Instance.UpdatePositionDirection(ref this.mPassiveAbilityEffect, ref position, ref up))
						{
							EffectManager.Instance.StartEffect(Item.GUNGNER_EFFECT, ref position, ref up, out this.mPassiveAbilityEffect);
						}
						this.mVisible = false;
					}
				}
				else
				{
					EffectManager.Instance.UpdatePositionDirection(ref this.mPassiveAbilityEffect, ref position, ref up);
					this.mVisible = true;
				}
			}
			base.Update(iDataChannel, iDeltaTime);
			Matrix orientation = this.mBody.Orientation;
			orientation.Translation = this.mBody.Position;
			if (!this.mAttached && this.mPickable)
			{
				orientation.M11 *= 1.5f;
				orientation.M12 *= 1.5f;
				orientation.M13 *= 1.5f;
				orientation.M21 *= 1.5f;
				orientation.M22 *= 1.5f;
				orientation.M23 *= 1.5f;
				orientation.M31 *= 1.5f;
				orientation.M32 *= 1.5f;
				orientation.M33 *= 1.5f;
			}
			this.mRenderData[(int)iDataChannel].mTransform = orientation;
			this.mHighlightRenderData[(int)iDataChannel].mTransform = orientation;
			this.mLastAttachAbsolutePosition = this.mAttach0AbsoluteTransform.Translation;
			Matrix.Multiply(ref this.mAttach0, ref orientation, out this.mAttach0AbsoluteTransform);
			Matrix.Multiply(ref this.mAttach1, ref orientation, out this.mAttach1AbsoluteTransform);
			Vector3.Transform(ref this.mEffect0Pos, ref orientation, out this.mEffect0AbsolutePos);
			Vector3.Transform(ref this.mEffect1Pos, ref orientation, out this.mEffect1AbsolutePos);
			bool flag = this.mSpecialAbilityCooldown > 0f;
			this.mSpecialAbilityCooldown -= iDeltaTime;
			if (flag && this.mSpecialAbilityCooldown <= 0f)
			{
				VisualEffectReference visualEffectReference;
				EffectManager.Instance.StartEffect(Item.SPECIAL_READY_EFFECT, ref this.mAttach0AbsoluteTransform, out visualEffectReference);
				AudioManager.Instance.PlayCue(Banks.Weapons, Item.RECHARGE_SOUND, base.AudioEmitter);
			}
			Vector3 forward = this.mAttach0AbsoluteTransform.Forward;
			Vector3 translation = this.mAttach0AbsoluteTransform.Translation;
			Elements elements = Elements.None;
			for (int i = 0; i < this.mSpellQueue.Count; i++)
			{
				elements |= this.mSpellQueue[i].Element;
			}
			Matrix matrix = default(Matrix);
			Vector3 vector;
			Vector3.Subtract(ref this.mEffect1AbsolutePos, ref this.mEffect0AbsolutePos, out vector);
			if (vector.LengthSquared() <= 1E-06f)
			{
				vector = forward;
			}
			else
			{
				vector.Normalize();
			}
			Vector3 vector2 = default(Vector3);
			vector2.Y = 1f;
			Vector3 vector3;
			Vector3.Cross(ref vector2, ref vector, out vector3);
			Vector3.Cross(ref vector, ref vector3, out vector2);
			vector3.Normalize();
			vector2.Normalize();
			matrix.M11 = vector3.X;
			matrix.M12 = vector3.Y;
			matrix.M13 = vector3.Z;
			matrix.M21 = vector2.X;
			matrix.M22 = vector2.Y;
			matrix.M23 = vector2.Z;
			matrix.M31 = vector.X;
			matrix.M32 = vector.Y;
			matrix.M33 = vector.Z;
			matrix.M41 = this.mEffect0AbsolutePos.X;
			matrix.M42 = this.mEffect0AbsolutePos.Y;
			matrix.M43 = this.mEffect0AbsolutePos.Z;
			matrix.M44 = 1f;
			if (this.mAttached)
			{
				for (int j = 0; j < this.mSpellEffects.Length; j++)
				{
					Elements elements2 = Spell.ElementFromIndex(j);
					if ((elements & elements2) == elements2)
					{
						if (this.mSpellEffects[j].ID < 0)
						{
							EffectManager.Instance.StartEffect(Item.ChantEffects[j], ref matrix, out this.mSpellEffects[j]);
						}
						else
						{
							EffectManager.Instance.UpdateOrientation(ref this.mSpellEffects[j], ref matrix);
						}
					}
					else if (this.mSpellEffects[j].ID >= 0)
					{
						EffectManager.Instance.Stop(ref this.mSpellEffects[j]);
					}
				}
				if (this.mEffects != null)
				{
					for (int k = 0; k < this.mEffects.Length; k++)
					{
						if (this.mEffects[k] != 0 && k < this.mVisualEffects.Length)
						{
							if (this.mVisible && !this.mIsInvisible && !this.HideEffects)
							{
								if (!EffectManager.Instance.UpdatePositionDirection(ref this.mVisualEffects[k], ref this.mEffect0AbsolutePos, ref vector))
								{
									EffectManager.Instance.StartEffect(this.mEffects[k], ref translation, ref forward, out this.mVisualEffects[k]);
								}
							}
							else
							{
								EffectManager.Instance.Stop(ref this.mVisualEffects[k]);
							}
						}
					}
				}
				if (this.mFollowEffects != null)
				{
					for (int l = 0; l < this.mFollowEffects.Length; l++)
					{
						if (EffectManager.Instance.IsActive(ref this.mFollowEffects[l]))
						{
							if (this.mVisible && !this.mIsInvisible && !this.HideEffects)
							{
								if (!EffectManager.Instance.UpdatePositionDirection(ref this.mFollowEffects[l], ref this.mEffect0AbsolutePos, ref vector))
								{
									EffectManager.Instance.StartEffect(this.mFollowEffects[l].ID, ref this.mEffect0AbsolutePos, ref vector, out this.mFollowEffects[l]);
								}
							}
							else
							{
								EffectManager.Instance.Stop(ref this.mFollowEffects[l]);
							}
						}
					}
				}
				if (this.mSounds != null)
				{
					for (int m = 0; m < this.mSounds.Length; m++)
					{
						if (this.mSounds[m].Key != 0 && m < this.mSoundCues.Length)
						{
							if (this.mPauseSounds)
							{
								if (this.mSoundCues[m] != null && !this.mSoundCues[m].IsPaused)
								{
									this.mSoundCues[m].Pause();
								}
							}
							else if (this.mSoundCues[m] == null || this.mSoundCues[m].IsStopped || this.mSoundCues[m].IsPaused)
							{
								this.mSoundCues[m] = AudioManager.Instance.PlayCue(this.mSounds[m].Value, this.mSounds[m].Key, this.mAudioEmitter);
							}
						}
					}
				}
				if (this.mPointLightHolder.ContainsLight && !this.mPointLightHolder.Enabled && this.mOwner != null)
				{
					if (this.mVisible && !this.mIsInvisible && !this.mHideModel)
					{
						this.mPointLightHolder.Enabled = true;
						this.mPointLight = DynamicLight.GetCachedLight();
						this.mPointLight.AmbientColor = this.mPointLightHolder.AmbientColor;
						this.mPointLight.DiffuseColor = this.mPointLightHolder.DiffuseColor;
						this.mPointLight.Radius = this.mPointLightHolder.Radius;
						this.mPointLight.SpecularAmount = this.mPointLightHolder.SpecularAmount;
						this.mPointLight.VariationAmount = this.mPointLightHolder.VariationAmount;
						this.mPointLight.VariationSpeed = this.mPointLightHolder.VariationSpeed;
						this.mPointLight.VariationType = this.mPointLightHolder.VariationType;
						this.mPointLight.Position = this.mAttach0AbsoluteTransform.Translation;
						this.mPointLight.Speed = 1f;
						this.mPointLight.Intensity = 1f;
						this.mPointLight.Enable(this.mOwner.PlayState.Scene);
					}
				}
				else if (this.mPointLight != null && this.mPointLight.Enabled)
				{
					if (this.mVisible && !this.mIsInvisible && !this.mHideModel)
					{
						this.mPointLight.Position = this.mAttach0AbsoluteTransform.Translation;
					}
					else
					{
						this.mPointLight.Disable();
						this.mPointLight = null;
						this.mPointLightHolder.Enabled = false;
					}
				}
				if (this.mOwner != null)
				{
					for (int n = 0; n < this.mAuras.Count; n++)
					{
						ActiveAura value = this.mAuras[n];
						value.Execute(this.mOwner, iDeltaTime);
						this.mAuras[n] = value;
						Vector3 position2 = this.mOwner.Position;
						Vector3 direction = this.mOwner.Direction;
						VisualEffectReference mEffect = this.mAuras[n].mEffect;
						EffectManager.Instance.UpdatePositionDirection(ref mEffect, ref position2, ref direction);
					}
				}
			}
			else
			{
				for (int num = 0; num < this.mSpellEffects.Length; num++)
				{
					if (this.mSpellEffects[num].ID >= 0)
					{
						EffectManager.Instance.Stop(ref this.mSpellEffects[num]);
					}
				}
				if (this.mEffects != null)
				{
					for (int num2 = 0; num2 < this.mEffects.Length; num2++)
					{
						if (num2 < this.mVisualEffects.Length && this.mVisualEffects[num2].ID != -1)
						{
							EffectManager.Instance.Stop(ref this.mVisualEffects[num2]);
						}
					}
				}
				if (this.mSounds != null)
				{
					for (int num3 = 0; num3 < this.mSoundCues.Length; num3++)
					{
						if (this.mSoundCues[num3] != null && !this.mSoundCues[num3].IsStopping)
						{
							this.mSoundCues[num3].Stop(AudioStopOptions.AsAuthored);
						}
					}
				}
				if (this.mPointLightHolder.ContainsLight && this.mPointLightHolder.Enabled && this.mOwner != null)
				{
					this.mPointLightHolder.Enabled = false;
					if (this.mPointLight != null)
					{
						this.mPointLight.Disable();
					}
					this.mPointLight = null;
				}
			}
			if (this.mSpecialAbilityCooldown > 0f)
			{
				(this.mRenderData[(int)iDataChannel] as Item.RenderData).EmissiveMultiplyer = Math.Min((float)Math.Pow(0.05, (double)this.mSpecialAbilityCooldown), 1f);
			}
			if (this.mOwner != null)
			{
				switch (this.mPassiveAbility.Ability)
				{
				case Item.PassiveAbilities.AreaLifeDrain:
					this.mPassiveAbilityTimer -= iDeltaTime;
					if (this.mPassiveAbilityTimer <= 0f && this.mOwner != null)
					{
						this.mPassiveAbilityTimer += 0.25f;
						List<Entity> entities = this.mPlayState.EntityManager.GetEntities(translation, 7f, true);
						entities.Remove(this.mOwner);
						if (this.mPassiveAbility.Ability == Item.PassiveAbilities.AreaLifeDrain)
						{
							foreach (Entity entity in entities)
							{
								Character character = entity as Character;
								if (character != null && !character.Dead)
								{
									character.Damage(4f, Elements.Arcane);
									this.mOwner.Damage(-4f, Elements.Life);
								}
							}
						}
						this.mPlayState.EntityManager.ReturnEntityList(entities);
					}
					break;
				case Item.PassiveAbilities.EnhanceAllyMelee:
				{
					List<Entity> entities2 = this.mPlayState.EntityManager.GetEntities(translation, 7f, true);
					foreach (Entity entity2 in entities2)
					{
						Character character2 = entity2 as Character;
						if (character2 != null && (character2.Faction & this.mOwner.Faction) != Factions.NONE)
						{
							character2.MeleeDamageBoost(1.5f);
						}
					}
					this.mPlayState.EntityManager.ReturnEntityList(entities2);
					break;
				}
				case Item.PassiveAbilities.AreaRegeneration:
					this.mPassiveAbilityTimer -= iDeltaTime;
					if (this.mPassiveAbilityTimer <= 0f && this.mOwner != null)
					{
						this.mPassiveAbilityTimer = 0.25f;
						List<Entity> entities3 = this.mPlayState.EntityManager.GetEntities(translation, 7f, true);
						foreach (Entity entity3 in entities3)
						{
							Character character3 = entity3 as Character;
							if (character3 != null && !character3.Dead && (character3.Faction & this.mOwner.Faction) != Factions.NONE)
							{
								character3.Damage(-2f, Elements.Life);
							}
						}
						this.mPlayState.EntityManager.ReturnEntityList(entities3);
					}
					break;
				case Item.PassiveAbilities.Zap:
					if (this.mAttached)
					{
						if (this.mTeslaField == null)
						{
							Spell iSpell = default(Spell);
							iSpell.Element = Elements.Lightning;
							iSpell[Elements.Lightning] = 1f;
							this.mTeslaField = TeslaField.GetFromCache(this.mPlayState);
							this.mTeslaField.Initialize(this.mOwner, iSpell);
							this.mTeslaField.ItemAbility = true;
						}
						this.mTeslaField.Update(iDataChannel, iDeltaTime);
					}
					else if (this.mTeslaField != null)
					{
						this.mTeslaField.Deinitialize();
					}
					break;
				case Item.PassiveAbilities.BirchSteam:
					if (this.mAttached)
					{
						Vector3 right = Vector3.Right;
						if (EffectManager.Instance.IsActive(ref this.mPassiveAbilityEffect))
						{
							EffectManager.Instance.UpdatePositionDirection(ref this.mPassiveAbilityEffect, ref translation, ref right);
						}
						else
						{
							EffectManager.Instance.StartEffect(Item.STEAM_EFFECT, ref translation, ref right, out this.mPassiveAbilityEffect);
						}
					}
					else
					{
						EffectManager.Instance.Stop(ref this.mPassiveAbilityEffect);
					}
					break;
				case Item.PassiveAbilities.MoveSpeed:
					this.mOwner.CharacterBody.SpeedMultiplier *= this.mPassiveAbility.Variable;
					break;
				case Item.PassiveAbilities.Glow:
				{
					List<Entity> entities4 = this.mPlayState.EntityManager.GetEntities(translation, this.mPassiveAbility.Variable, true);
					entities4.Remove(this.mOwner);
					bool flag2 = false;
					foreach (Entity entity4 in entities4)
					{
						Character character4 = entity4 as Character;
						if (character4 != null && (character4.Faction & this.Owner.Faction) == Factions.NONE)
						{
							flag2 = true;
							break;
						}
					}
					this.mPlayState.EntityManager.ReturnEntityList(entities4);
					this.mGlowTarget = (flag2 ? 1f : 0f);
					this.mGlow += (this.mGlowTarget - this.mGlow) * iDeltaTime * 0.5f;
					(this.mRenderData[(int)iDataChannel] as Item.RenderData).EmissiveMultiplyer = Math.Max(this.mGlow, 0f);
					break;
				}
				}
			}
			if (this.IsGunClass && this.mGunRateTimer > 0f && this.mOwner != null && this.mGunCurrentClip > 0)
			{
				this.mGunRateTimer -= iDeltaTime;
				if (this.mGunSound != null)
				{
					this.mGunSound.Apply3D(this.mPlayState.Camera.Listener, this.mAudioEmitter);
				}
				EffectManager.Instance.UpdateOrientation(ref this.mGunShellsEffect, ref this.mAttach1AbsoluteTransform);
				if (this.mGunRateTimer <= 0f)
				{
					if (this.mFiring)
					{
						this.mGunRateTimer += 1f / this.mGunRate;
						this.mGunCurrentClip--;
					}
					VisualEffectReference visualEffectReference2;
					EffectManager.Instance.StartEffect(this.mGunMuzzleEffectID, ref this.mAttach0AbsoluteTransform, out visualEffectReference2);
					if (this.mOwner.CurrentAnimation != Animations.pickup_weapon)
					{
						Vector3 translation2 = this.mAttach0AbsoluteTransform.Translation;
						Vector3 translation3 = this.mAttach1AbsoluteTransform.Translation;
						Vector3.Subtract(ref translation2, ref translation3, out translation3);
						translation3.Normalize();
						float num4 = (1f - this.mGunCurrentAccuracy) * 0.2f;
						float num5 = (float)Math.Atan2((double)translation3.Z, (double)translation3.X);
						float num6 = MagickaMath.RandomBetween(Item.sRandom, -num4 * 0.5f, num4 * 0.5f);
						float z = (float)Math.Sin((double)(num5 + num6));
						float x = (float)Math.Cos((double)(num5 + num6));
						translation3.X = x;
						translation3.Z = z;
						translation3.Y += MagickaMath.RandomBetween(Item.sRandom, -num4 * 2f, num4 * 2f);
						translation3.Normalize();
						this.FireShot(ref translation2, ref translation3, 0f, this.mGunRange, this.mOwner);
						this.mTracerCount += 1U;
					}
				}
			}
			if (this.mSpellQueue.Count > 0 && base.Model != null)
			{
				this.mSpellTime += iDeltaTime;
				Item.ElementRenderData elementRenderData = this.mElementRenderData[(int)iDataChannel];
				elementRenderData.mBoundingSphere = this.mRenderData[(int)iDataChannel].mBoundingSphere;
				elementRenderData.Pos0 = this.mEffect0AbsolutePos;
				elementRenderData.Pos1 = this.mEffect1AbsolutePos;
				elementRenderData.Alpha = ((float)Math.Sin((double)(this.mSpellTime * 2f)) * 0.25f + 0.75f) * Math.Min(this.mSpellTime, 1f);
				elementRenderData.Color = this.mSpellColor;
				this.mPlayState.Scene.AddRenderableAdditiveObject(iDataChannel, elementRenderData);
			}
			else
			{
				this.mSpellTime = 0f;
			}
			if (this.mDespawnTime > 0f && base.Model != null)
			{
				this.mDespawnTime -= iDeltaTime;
				Vector3 position3 = this.Position;
				Vector3 right2 = Vector3.Right;
				bool flag3 = EffectManager.Instance.UpdatePositionDirection(ref this.mDespawnEffect, ref position3, ref right2);
				if (this.mDespawnTime <= 0f)
				{
					VisualEffectReference visualEffectReference3;
					EffectManager.Instance.StartEffect(BookOfMagick.DISAPPEAR_EFFECT, ref position3, ref right2, out visualEffectReference3);
					if (NetworkManager.Instance.State != NetworkState.Client)
					{
						this.Kill();
					}
				}
				else if (!flag3 && this.mDespawnTime <= 4f)
				{
					EffectManager.Instance.StartEffect(BookOfMagick.TIMEOUT_EFFECT, ref position3, ref right2, out this.mDespawnEffect);
				}
			}
			if (this.mAttached)
			{
				this.mDespawnTime = 0f;
			}
		}

		// Token: 0x06001A09 RID: 6665 RVA: 0x000AEE80 File Offset: 0x000AD080
		private void FireShot(ref Vector3 iOrigin, ref Vector3 iDir, float iDelay, float iRange, Entity iIgnored)
		{
			Segment segment;
			segment.Origin = iOrigin;
			Vector3.Multiply(ref iDir, iRange, out segment.Delta);
			Vector3 iCenter;
			segment.GetPoint(0.5f, out iCenter);
			float scaleFactor;
			Vector3 vector;
			Vector3 vector2;
			AnimatedLevelPart animatedLevelPart;
			int num;
			bool flag = this.mPlayState.Level.CurrentScene.SegmentIntersect(out scaleFactor, out vector, out vector2, out animatedLevelPart, out num, segment);
			if (flag)
			{
				Vector3.Multiply(ref segment.Delta, scaleFactor, out segment.Delta);
			}
			List<Entity> entities = this.mPlayState.EntityManager.GetEntities(iCenter, iRange * 0.5f, true);
			Entity entity = null;
			float num2 = float.MaxValue;
			Vector3 value = default(Vector3);
			for (int i = 0; i < entities.Count; i++)
			{
				Entity entity2 = entities[i];
				if (entity2 != iIgnored)
				{
					IDamageable damageable = entities[i] as IDamageable;
					Portal.PortalEntity portalEntity = entities[i] as Portal.PortalEntity;
					float num4;
					Vector3 vector4;
					Vector3 vector5;
					if (damageable != null && !this.mHitlist.Contains(damageable) && !(entity2 is Grease.GreaseField) && !(entity2 is TornadoEntity))
					{
						Vector3 vector3;
						if (damageable.SegmentIntersect(out vector3, segment, 0.05f))
						{
							float num3;
							Vector3.DistanceSquared(ref segment.Origin, ref vector3, out num3);
							if (num3 < num2)
							{
								value = vector3;
								num2 = num3;
								entity = (damageable as Entity);
							}
						}
					}
					else if (portalEntity != null && Portal.Instance.Connected && portalEntity.Body.CollisionSkin.SegmentIntersect(out num4, out vector4, out vector5, segment))
					{
						float num5;
						Vector3.DistanceSquared(ref segment.Origin, ref vector4, out num5);
						if (num5 < num2)
						{
							value = vector4;
							num2 = num5;
							entity = portalEntity;
						}
					}
				}
			}
			this.mPlayState.EntityManager.ReturnEntityList(entities);
			if (entity != null)
			{
				Character character = entity as Character;
				IDamageable damageable2 = entity as IDamageable;
				Portal.PortalEntity portalEntity2 = entity as Portal.PortalEntity;
				if (portalEntity2 != null)
				{
					float num6 = (float)Math.Sqrt((double)num2);
					float iDelay2 = (num6 - 2f) / this.mTracerVelocity;
					Vector3 vector6;
					portalEntity2.GetOutPos(ref value, out vector6);
					this.FireShot(ref vector6, ref iDir, iDelay2, iRange - num6, Portal.OtherPortal(portalEntity2));
				}
				else if (character != null && character.HasAura(AuraType.Deflect))
				{
					float num7 = (float)Math.Sqrt((double)num2);
					float iDelay3 = (num7 - 2f) / this.mTracerVelocity;
					float num8;
					Vector3 vector7;
					Vector3 position;
					if (!character.Body.CollisionSkin.SegmentIntersect(out num8, out vector7, out position, segment))
					{
						position = character.Position;
						Vector3.Subtract(ref value, ref position, out position);
					}
					position.Y = 0f;
					Vector3 vector8;
					if (position.LengthSquared() < 1E-06f)
					{
						vector8 = iDir;
					}
					else
					{
						position.Normalize();
						Vector3.Reflect(ref iDir, ref position, out vector8);
					}
					this.FireShot(ref value, ref vector8, iDelay3, iRange - num7, entity);
				}
				else if (damageable2 != null)
				{
					bool flag2 = damageable2.HitPoints <= 0f;
					EventCondition eventCondition = default(EventCondition);
					eventCondition.Position = new Vector3?(value);
					eventCondition.EventConditionType = EventConditionType.Hit;
					DamageResult damageResult;
					this.mGunConditions.ExecuteAll(this, entity, ref eventCondition, out damageResult);
					if (this.mOwner is Avatar && !((this.mOwner as Avatar).Player.Gamer is NetworkGamer) && !flag2 && (damageResult & (DamageResult.Killed | DamageResult.OverKilled)) != DamageResult.None)
					{
						Profile.Instance.AddLedKill(base.PlayState);
						AchievementsManager.Instance.AwardAchievement(base.PlayState, "firstblood");
					}
					if (entity is PhysicsEntity)
					{
						Vector3 vector9;
						Vector3.Negate(ref iDir, out vector9);
						VisualEffectReference visualEffectReference;
						EffectManager.Instance.StartEffect((entity as PhysicsEntity).HitEffect, ref value, ref vector9, out visualEffectReference);
					}
				}
			}
			else if (flag)
			{
				value = vector;
				EventCondition eventCondition2 = default(EventCondition);
				eventCondition2.Position = new Vector3?(vector);
				eventCondition2.EventConditionType = EventConditionType.Collision;
				DamageResult damageResult2;
				this.mGunConditions.ExecuteAll(this, null, ref eventCondition2, out damageResult2);
				if (num >= 0)
				{
					VisualEffectReference visualEffectReference2;
					EffectManager.Instance.StartEffect(Item.GUN_HIT_EFFECTS[num], ref vector, ref vector2, out visualEffectReference2);
				}
			}
			else
			{
				segment.GetEnd(out value);
			}
			if (this.mTracerCount % 5U == 0U && this.mTracerSprite >= 0f)
			{
				TracerMan.Instance.AddTracer(ref segment.Origin, ref value, this.mTracerVelocity, 1f, (byte)this.mTracerSprite, iDelay);
				return;
			}
			if (this.mNonTracerSprite >= 0f)
			{
				TracerMan.Instance.AddTracer(ref segment.Origin, ref value, this.mTracerVelocity, 1f, (byte)this.mNonTracerSprite, iDelay);
			}
		}

		// Token: 0x06001A0A RID: 6666 RVA: 0x000AF2EC File Offset: 0x000AD4EC
		public static Item Read(ContentReader iInput)
		{
			Item item = new Item(null, null);
			item.mName = iInput.ReadString();
			item.mType = item.mName.GetHashCodeCustom();
			item.mAnimatedLevelPartID = 0;
			string text = iInput.ReadString();
			item.mDisplayName = text.GetHashCodeCustom();
			item.mPickUpString = LanguageManager.Instance.GetString(Item.ITEM_PICKUP_LOC);
			if (!string.IsNullOrEmpty(text))
			{
				item.mPickUpString = item.mPickUpString.Replace("#1;", "[c=1,1,1]" + LanguageManager.Instance.GetString(item.mDisplayName) + "[/c]");
			}
			item.mDescription = iInput.ReadString().GetHashCodeCustom();
			int num = iInput.ReadInt32();
			item.mSounds = new KeyValuePair<int, Banks>[num];
			for (int i = 0; i < num; i++)
			{
				string text2 = iInput.ReadString();
				Banks value = (Banks)iInput.ReadInt32();
				int hashCodeCustom = text2.ToLowerInvariant().GetHashCodeCustom();
				item.mSounds[i] = new KeyValuePair<int, Banks>(hashCodeCustom, value);
			}
			item.mPickable = iInput.ReadBoolean();
			item.mBound = iInput.ReadBoolean();
			item.mBlockValue = iInput.ReadInt32();
			item.mWeaponClass = (WeaponClass)iInput.ReadByte();
			item.mCooldownTimer = 0f;
			item.mCooldownTime = iInput.ReadSingle();
			item.mHideModel = iInput.ReadBoolean();
			item.mHideEffect = iInput.ReadBoolean();
			item.mPauseSounds = iInput.ReadBoolean();
			for (int j = 0; j < 11; j++)
			{
				Resistance resistance;
				resistance.ResistanceAgainst = Spell.ElementFromIndex(j);
				resistance.Multiplier = 1f;
				resistance.Modifier = 0f;
				resistance.StatusResistance = false;
				item.mResistances[j] = resistance;
			}
			int num2 = iInput.ReadInt32();
			for (int k = 0; k < num2; k++)
			{
				Resistance resistance2;
				resistance2.ResistanceAgainst = (Elements)iInput.ReadInt32();
				resistance2.Multiplier = iInput.ReadSingle();
				resistance2.Modifier = iInput.ReadSingle();
				resistance2.StatusResistance = iInput.ReadBoolean();
				item.mResistances[Spell.ElementIndex(resistance2.ResistanceAgainst)] = resistance2;
			}
			item.mPassiveAbility = new Item.PassiveAbilityStruct((Item.PassiveAbilities)iInput.ReadByte(), iInput.ReadSingle());
			num2 = iInput.ReadInt32();
			item.mEffects = new int[num2];
			for (int l = 0; l < num2; l++)
			{
				string text3 = iInput.ReadString();
				item.mEffects[l] = text3.ToLowerInvariant().GetHashCodeCustom();
			}
			num2 = iInput.ReadInt32();
			for (int m = 0; m < num2; m++)
			{
				if (m != 0)
				{
					throw new Exception("Items may Only have One Light!");
				}
				item.mPointLightHolder.ContainsLight = true;
				item.mPointLightHolder.Radius = iInput.ReadSingle();
				item.mPointLightHolder.DiffuseColor = iInput.ReadVector3();
				item.mPointLightHolder.AmbientColor = iInput.ReadVector3();
				item.mPointLightHolder.SpecularAmount = iInput.ReadSingle();
				item.mPointLightHolder.VariationType = (LightVariationType)iInput.ReadByte();
				item.mPointLightHolder.VariationAmount = iInput.ReadSingle();
				item.mPointLightHolder.VariationSpeed = iInput.ReadSingle();
			}
			if (iInput.ReadBoolean())
			{
				item.mSpecialAbilityRechargeTime = iInput.ReadSingle();
				item.mSpecialAbility = Magicka.GameLogic.Entities.Abilities.SpecialAbilities.SpecialAbility.Read(iInput);
			}
			item.mMeleeRange = iInput.ReadSingle();
			item.mMeleeMultiHit = iInput.ReadBoolean();
			item.mMeleeConditions = new ConditionCollection(iInput);
			item.mRangedRange = iInput.ReadSingle();
			item.mFacing = iInput.ReadBoolean();
			item.mHoming = iInput.ReadSingle();
			item.mRangedElevation = iInput.ReadSingle();
			item.mRangedElevation = MathHelper.ToRadians(item.mRangedElevation);
			item.mRangedDanger = iInput.ReadSingle();
			item.mGunRange = iInput.ReadSingle();
			item.mGunClip = iInput.ReadInt32();
			item.mGunRate = (float)iInput.ReadInt32();
			item.mGunAccuracy = iInput.ReadSingle();
			string text4 = iInput.ReadString();
			item.mGunSoundBank = Banks.Weapons;
			if (string.IsNullOrEmpty(text4))
			{
				item.mGunSoundID = 0;
			}
			else
			{
				string[] array = text4.Split(new char[]
				{
					'/'
				});
				if (array != null && array.Length > 1)
				{
					item.mGunSoundBank = (Banks)Enum.Parse(typeof(Banks), array[0], true);
					item.mGunSoundID = array[1].ToLower().GetHashCodeCustom();
				}
				else
				{
					item.mGunSoundBank = Banks.Weapons;
					item.mGunSoundID = text4.ToLower().GetHashCodeCustom();
				}
			}
			text4 = iInput.ReadString();
			if (string.IsNullOrEmpty(text4))
			{
				item.mGunMuzzleEffectID = 0;
			}
			else
			{
				item.mGunMuzzleEffectID = text4.GetHashCodeCustom();
			}
			text4 = iInput.ReadString();
			if (string.IsNullOrEmpty(text4))
			{
				item.mGunShellsEffectID = 0;
			}
			else
			{
				item.mGunShellsEffectID = text4.GetHashCodeCustom();
			}
			item.mTracerVelocity = iInput.ReadSingle();
			text4 = iInput.ReadString();
			if (string.IsNullOrEmpty(text4))
			{
				item.mNonTracerSprite = -1f;
			}
			else
			{
				string[] array2 = text4.Split(new char[]
				{
					'/'
				});
				if (array2.Length > 1)
				{
					item.mNonTracerSprite = float.Parse(array2[1], CultureInfo.InvariantCulture.NumberFormat);
					if (array2[0].Equals("B", StringComparison.InvariantCultureIgnoreCase))
					{
						item.mNonTracerSprite += 64f;
					}
					if (array2[0].Equals("C", StringComparison.InvariantCultureIgnoreCase))
					{
						item.mNonTracerSprite += 128f;
					}
					if (array2[0].Equals("D", StringComparison.InvariantCultureIgnoreCase))
					{
						item.mNonTracerSprite += 192f;
					}
					if (array2[0].Equals("E", StringComparison.InvariantCultureIgnoreCase))
					{
						item.mNonTracerSprite += 256f;
					}
				}
				else
				{
					item.mNonTracerSprite = float.Parse(array2[0], CultureInfo.InvariantCulture.NumberFormat);
				}
			}
			text4 = iInput.ReadString();
			if (string.IsNullOrEmpty(text4))
			{
				item.mTracerSprite = -1f;
			}
			else
			{
				string[] array2 = text4.Split(new char[]
				{
					'/'
				});
				if (array2.Length > 1)
				{
					item.mTracerSprite = float.Parse(array2[1], CultureInfo.InvariantCulture.NumberFormat);
					if (array2[0].Equals("B", StringComparison.InvariantCultureIgnoreCase))
					{
						item.mTracerSprite += 64f;
					}
					if (array2[0].Equals("C", StringComparison.InvariantCultureIgnoreCase))
					{
						item.mTracerSprite += 128f;
					}
					if (array2[0].Equals("D", StringComparison.InvariantCultureIgnoreCase))
					{
						item.mTracerSprite += 192f;
					}
					if (array2[0].Equals("E", StringComparison.InvariantCultureIgnoreCase))
					{
						item.mTracerSprite += 256f;
					}
				}
				else
				{
					item.mTracerSprite = float.Parse(array2[0], CultureInfo.InvariantCulture.NumberFormat);
				}
			}
			item.mGunConditions = new ConditionCollection(iInput);
			lock (Game.Instance.GraphicsDevice)
			{
				item.ProjectileModel = iInput.ReadExternalReference<Model>();
			}
			item.mRangedConditions = new ConditionCollection(iInput);
			item.mScale = iInput.ReadSingle();
			lock (Game.Instance.GraphicsDevice)
			{
				item.Model = iInput.ReadExternalReference<Model>();
			}
			item.mAttach0 = Matrix.Identity;
			item.mGlow = 1f;
			item.mGlowTarget = 1f;
			if (item.Model != null)
			{
				VertexElement[] vertexElements;
				lock (Game.Instance.GraphicsDevice)
				{
					vertexElements = item.Model.Meshes[0].MeshParts[0].VertexDeclaration.GetVertexElements();
				}
				int num3 = -1;
				for (int n = 0; n < vertexElements.Length; n++)
				{
					if (vertexElements[n].VertexElementUsage == VertexElementUsage.Position)
					{
						num3 = (int)vertexElements[n].Offset;
						break;
					}
				}
				if (num3 < 0)
				{
					throw new Exception("No positions found");
				}
				Vector3[] array3 = new Vector3[item.Model.Meshes[0].MeshParts[0].NumVertices];
				item.Model.Meshes[0].VertexBuffer.GetData<Vector3>(num3, array3, item.Model.Meshes[0].MeshParts[0].StartIndex, array3.Length, item.Model.Meshes[0].MeshParts[0].VertexStride);
				item.mBoundingBox = BoundingBox.CreateFromPoints(array3);
				for (int num4 = 0; num4 < item.Model.Bones.Count; num4++)
				{
					if (item.Model.Bones[num4].Name.Equals("attach0", StringComparison.OrdinalIgnoreCase))
					{
						ModelBone modelBone = item.Model.Bones[num4];
						Matrix transform = modelBone.Transform;
						while (modelBone.Parent != null)
						{
							modelBone = modelBone.Parent;
							Matrix transform2 = modelBone.Transform;
							Matrix.Multiply(ref transform2, ref transform, out transform);
						}
						item.mAttach0 = transform;
					}
					else if (item.Model.Bones[num4].Name.Equals("attach1", StringComparison.OrdinalIgnoreCase))
					{
						ModelBone modelBone2 = item.Model.Bones[num4];
						Matrix transform3 = modelBone2.Transform;
						while (modelBone2.Parent != null)
						{
							modelBone2 = modelBone2.Parent;
							Matrix transform4 = modelBone2.Transform;
							Matrix.Multiply(ref transform4, ref transform3, out transform3);
						}
						item.mAttach1 = transform3;
					}
					else if (item.Model.Bones[num4].Name.Equals("effect0", StringComparison.OrdinalIgnoreCase))
					{
						ModelBone modelBone3 = item.Model.Bones[num4];
						Matrix transform5 = modelBone3.Transform;
						while (modelBone3.Parent != null)
						{
							modelBone3 = modelBone3.Parent;
							Matrix transform6 = modelBone3.Transform;
							Matrix.Multiply(ref transform6, ref transform5, out transform5);
						}
						item.mEffect0Pos = transform5.Translation;
					}
					else if (item.Model.Bones[num4].Name.Equals("effect1", StringComparison.OrdinalIgnoreCase))
					{
						ModelBone modelBone4 = item.Model.Bones[num4];
						Matrix transform7 = modelBone4.Transform;
						while (modelBone4.Parent != null)
						{
							modelBone4 = modelBone4.Parent;
							Matrix transform8 = modelBone4.Transform;
							Matrix.Multiply(ref transform8, ref transform7, out transform7);
						}
						item.mEffect1Pos = transform7.Translation;
					}
				}
			}
			item.mIgnoreTractorPull = false;
			num2 = iInput.ReadInt32();
			item.mAuras = new List<ActiveAura>(num2);
			for (int num5 = 0; num5 < num2; num5++)
			{
				ActiveAura item2 = default(ActiveAura);
				item2.Aura = new AuraStorage(iInput);
				item.mAuras.Add(item2);
			}
			Item.CacheWeapon(item.mType, item);
			return item;
		}

		// Token: 0x06001A0B RID: 6667 RVA: 0x000AFE38 File Offset: 0x000AE038
		public static void Swap(Item iAItem, Item iBItem)
		{
			Helper.Swap<string>(ref iAItem.mName, ref iBItem.mName);
			Helper.Swap<int>(ref iAItem.mType, ref iBItem.mType);
			Helper.Swap<int>(ref iAItem.mDisplayName, ref iBItem.mDisplayName);
			Helper.Swap<int>(ref iAItem.mDescription, ref iBItem.mDescription);
			Helper.Swap<string>(ref iAItem.mPickUpString, ref iBItem.mPickUpString);
			Helper.Swap<bool>(ref iAItem.mPickable, ref iBItem.mPickable);
			Helper.Swap<bool>(ref iAItem.mBound, ref iBItem.mBound);
			Helper.Swap<WeaponClass>(ref iAItem.mWeaponClass, ref iBItem.mWeaponClass);
			Helper.Swap<int[]>(ref iAItem.mEffects, ref iBItem.mEffects);
			Helper.Swap<bool>(ref iAItem.mFacing, ref iBItem.mFacing);
			Helper.Swap<float>(ref iAItem.mDespawnTime, ref iBItem.mDespawnTime);
			Helper.Swap<float>(ref iAItem.mCooldownTimer, ref iBItem.mCooldownTimer);
			Helper.Swap<float>(ref iAItem.mCooldownTime, ref iBItem.mCooldownTime);
			Helper.Swap<bool>(ref iAItem.mHideModel, ref iBItem.mHideModel);
			Helper.Swap<bool>(ref iAItem.mHideEffect, ref iBItem.mHideEffect);
			Helper.Swap<bool>(ref iAItem.mPauseSounds, ref iBItem.mPauseSounds);
			Helper.Swap<bool>(ref iAItem.mIgnoreTractorPull, ref iBItem.mIgnoreTractorPull);
			AnimatedLevelPart animatedLevelPart;
			if (iAItem.mAnimatedLevelPartID != 0 && iAItem.mPlayState.Level.CurrentScene.LevelModel.AnimatedLevelParts.TryGetValue(iAItem.mAnimatedLevelPartID, out animatedLevelPart))
			{
				animatedLevelPart.RemoveEntity(iAItem);
				iAItem.AnimatedLevelPartID = 0;
			}
			AnimatedLevelPart animatedLevelPart2;
			if (iBItem.mAnimatedLevelPartID != 0 && iBItem.mPlayState.Level.CurrentScene.LevelModel.AnimatedLevelParts.TryGetValue(iBItem.mAnimatedLevelPartID, out animatedLevelPart2))
			{
				animatedLevelPart2.RemoveEntity(iBItem);
				iBItem.AnimatedLevelPartID = 0;
			}
			EffectManager.Instance.Stop(ref iAItem.mDespawnEffect);
			EffectManager.Instance.Stop(ref iBItem.mDespawnEffect);
			for (int i = 0; i < iAItem.mVisualEffects.Length; i++)
			{
				if (iAItem.mVisualEffects[i].Hash != 0 && iAItem.mVisualEffects[i].ID >= 0)
				{
					EffectManager.Instance.Stop(ref iAItem.mVisualEffects[i]);
				}
			}
			for (int j = 0; j < iBItem.mVisualEffects.Length; j++)
			{
				if (iBItem.mVisualEffects[j].Hash != 0 && iBItem.mVisualEffects[j].ID >= 0)
				{
					EffectManager.Instance.Stop(ref iBItem.mVisualEffects[j]);
				}
			}
			Helper.Swap<KeyValuePair<int, Banks>[]>(ref iAItem.mSounds, ref iBItem.mSounds);
			for (int k = 0; k < iAItem.mSoundCues.Length; k++)
			{
				if (iAItem.mSoundCues[k] != null && !iAItem.mSoundCues[k].IsStopping)
				{
					iAItem.mSoundCues[k].Stop(AudioStopOptions.AsAuthored);
				}
			}
			for (int l = 0; l < iBItem.mSoundCues.Length; l++)
			{
				if (iBItem.mSoundCues[l] != null && !iBItem.mSoundCues[l].IsStopping)
				{
					iBItem.mSoundCues[l].Stop(AudioStopOptions.AsAuthored);
				}
			}
			if (iAItem.mGunSound != null && !iAItem.mGunSound.IsStopping)
			{
				iAItem.mGunSound.Stop(AudioStopOptions.AsAuthored);
				iAItem.mGunSound = null;
			}
			EffectManager.Instance.Stop(ref iAItem.mGunShellsEffect);
			if (iBItem.mGunSound != null && !iBItem.mGunSound.IsStopping)
			{
				iBItem.mGunSound.Stop(AudioStopOptions.AsAuthored);
				iBItem.mGunSound = null;
			}
			EffectManager.Instance.Stop(ref iBItem.mGunShellsEffect);
			Helper.Swap<Item.PointLightHolder>(ref iAItem.mPointLightHolder, ref iBItem.mPointLightHolder);
			if (iAItem.mPointLight != null)
			{
				iAItem.mPointLight.Disable();
				iAItem.mPointLight = null;
			}
			if (iBItem.mPointLight != null)
			{
				iBItem.mPointLight.Disable();
				iBItem.mPointLight = null;
			}
			iAItem.mPointLightHolder.Enabled = false;
			iBItem.mPointLightHolder.Enabled = false;
			Model model = iBItem.Model;
			iBItem.Model = iAItem.Model;
			iAItem.Model = model;
			Helper.Swap<Model>(ref iAItem.mProjectileModel, ref iBItem.mProjectileModel);
			Helper.Swap<float>(ref iAItem.mMeleeRange, ref iBItem.mMeleeRange);
			Helper.Swap<ConditionCollection>(ref iAItem.mMeleeConditions, ref iBItem.mMeleeConditions);
			Helper.Swap<bool>(ref iAItem.mMeleeMultiHit, ref iBItem.mMeleeMultiHit);
			Helper.Swap<ConditionCollection>(ref iAItem.mGunConditions, ref iBItem.mGunConditions);
			Helper.Swap<float>(ref iAItem.mRangedRange, ref iBItem.mRangedRange);
			Helper.Swap<ConditionCollection>(ref iAItem.mRangedConditions, ref iBItem.mRangedConditions);
			iAItem.mSpellQueue.Clear();
			iBItem.mSpellQueue.Clear();
			Helper.Swap<VisualEffectReference[]>(ref iAItem.mSpellEffects, ref iBItem.mSpellEffects);
			Helper.Swap<VisualEffectReference[]>(ref iAItem.mVisualEffects, ref iBItem.mVisualEffects);
			Helper.Swap<Matrix>(ref iAItem.mAttach0, ref iBItem.mAttach0);
			Helper.Swap<Matrix>(ref iAItem.mAttach1, ref iBItem.mAttach1);
			Helper.Swap<Vector3>(ref iAItem.mEffect0Pos, ref iBItem.mEffect0Pos);
			Helper.Swap<Vector3>(ref iAItem.mEffect1Pos, ref iBItem.mEffect1Pos);
			Helper.Swap<BoundingBox>(ref iAItem.mBoundingBox, ref iBItem.mBoundingBox);
			Helper.Swap<int>(ref iAItem.mBlockValue, ref iBItem.mBlockValue);
			Helper.Swap<float>(ref iAItem.mRangedDanger, ref iBItem.mRangedDanger);
			Helper.Swap<float>(ref iAItem.mRangedElevation, ref iBItem.mRangedElevation);
			Helper.Swap<float>(ref iAItem.mHoming, ref iBItem.mHoming);
			Helper.Swap<float>(ref iAItem.mGunRange, ref iBItem.mGunRange);
			Helper.Swap<int>(ref iAItem.mGunClip, ref iBItem.mGunClip);
			Helper.Swap<float>(ref iAItem.mGunRate, ref iBItem.mGunRate);
			Helper.Swap<float>(ref iAItem.mGunAccuracy, ref iBItem.mGunAccuracy);
			Helper.Swap<int>(ref iAItem.mGunSoundID, ref iBItem.mGunSoundID);
			Helper.Swap<Banks>(ref iAItem.mGunSoundBank, ref iBItem.mGunSoundBank);
			Helper.Swap<int>(ref iAItem.mGunMuzzleEffectID, ref iBItem.mGunMuzzleEffectID);
			Helper.Swap<int>(ref iAItem.mGunShellsEffectID, ref iBItem.mGunShellsEffectID);
			Helper.Swap<uint>(ref iAItem.mTracerCount, ref iBItem.mTracerCount);
			Helper.Swap<float>(ref iAItem.mTracerSprite, ref iBItem.mTracerSprite);
			Helper.Swap<float>(ref iAItem.mNonTracerSprite, ref iBItem.mNonTracerSprite);
			Helper.Swap<float>(ref iAItem.mTracerVelocity, ref iBItem.mTracerVelocity);
			Helper.Swap<float>(ref iAItem.mSpecialAbilityCooldown, ref iBItem.mSpecialAbilityCooldown);
			Helper.Swap<float>(ref iAItem.mSpecialAbilityRechargeTime, ref iBItem.mSpecialAbilityRechargeTime);
			Helper.Swap<Magicka.GameLogic.Entities.Abilities.SpecialAbilities.SpecialAbility>(ref iAItem.mSpecialAbility, ref iBItem.mSpecialAbility);
			Helper.Swap<Item.PassiveAbilityStruct>(ref iAItem.mPassiveAbility, ref iBItem.mPassiveAbility);
			Helper.Swap<TeslaField>(ref iAItem.mTeslaField, ref iBItem.mTeslaField);
			Helper.Swap<float>(ref iAItem.mGlow, ref iBItem.mGlow);
			Helper.Swap<float>(ref iAItem.mGlowTarget, ref iBItem.mGlowTarget);
			Helper.Swap<MissileEntity>(ref iAItem.mGungnirMissile, ref iBItem.mGungnirMissile);
			Helper.Swap<Resistance[]>(ref iAItem.mResistances, ref iBItem.mResistances);
			for (int m = 0; m < iAItem.mAuras.Count; m++)
			{
				VisualEffectReference mEffect = iAItem.mAuras[m].mEffect;
				EffectManager.Instance.Stop(ref mEffect);
			}
			for (int n = 0; n < iBItem.mAuras.Count; n++)
			{
				VisualEffectReference mEffect2 = iBItem.mAuras[n].mEffect;
				EffectManager.Instance.Stop(ref mEffect2);
			}
			Helper.Swap<List<ActiveAura>>(ref iAItem.mAuras, ref iBItem.mAuras);
			if (iAItem.mOwner != null)
			{
				for (int num = 0; num < iAItem.mAuras.Count; num++)
				{
					VisualEffectReference mEffect3 = default(VisualEffectReference);
					ActiveAura value = iAItem.mAuras[num];
					if (iAItem.mAuras[num].Aura.Effect != 0)
					{
						int effect = iAItem.mAuras[num].Aura.Effect;
						Vector3 position = iAItem.mOwner.Position;
						Vector3 direction = iAItem.mOwner.Direction;
						EffectManager.Instance.StartEffect(effect, ref position, ref direction, out mEffect3);
					}
					value.mEffect = mEffect3;
					iAItem.mAuras[num] = value;
				}
			}
		}

		// Token: 0x06001A0C RID: 6668 RVA: 0x000B060C File Offset: 0x000AE80C
		public void Copy(Item iCopy)
		{
			iCopy.mName = this.mName;
			iCopy.mType = this.mType;
			iCopy.mDescription = this.mDescription;
			iCopy.mDisplayName = this.mDisplayName;
			iCopy.mPickUpString = this.mPickUpString;
			iCopy.mPickable = this.mPickable;
			iCopy.mBound = this.mBound;
			iCopy.mWeaponClass = this.mWeaponClass;
			iCopy.mEffects = this.mEffects;
			iCopy.mGunConditions = this.mGunConditions;
			iCopy.Model = base.Model;
			iCopy.ProjectileModel = this.mProjectileModel;
			iCopy.mMeleeRange = this.mMeleeRange;
			iCopy.mMeleeConditions = this.mMeleeConditions;
			iCopy.mMeleeMultiHit = this.mMeleeMultiHit;
			iCopy.mRangedRange = this.mRangedRange;
			iCopy.mRangedConditions = this.mRangedConditions;
			iCopy.mHoming = this.mHoming;
			iCopy.mFacing = this.mFacing;
			iCopy.mRangedElevation = this.mRangedElevation;
			iCopy.mRangedDanger = this.mRangedDanger;
			iCopy.PreviousOwner = this.mPreviousOwner;
			iCopy.mIgnoreTractorPull = this.mIgnoreTractorPull;
			iCopy.mAnimatedLevelPartID = this.mAnimatedLevelPartID;
			iCopy.mGunRange = this.mGunRange;
			iCopy.mGunClip = this.mGunClip;
			iCopy.mGunRate = this.mGunRate;
			iCopy.mGunAccuracy = this.mGunAccuracy;
			iCopy.mGunSoundBank = this.mGunSoundBank;
			iCopy.mGunSoundID = this.mGunSoundID;
			iCopy.mGunMuzzleEffectID = this.mGunMuzzleEffectID;
			iCopy.mGunShellsEffectID = this.mGunShellsEffectID;
			iCopy.mNonTracerSprite = this.mNonTracerSprite;
			iCopy.mTracerSprite = this.mTracerSprite;
			iCopy.mTracerVelocity = this.mTracerVelocity;
			iCopy.mTracerCount = this.mTracerCount;
			iCopy.mCooldownTimer = (this.mCooldownTimer = 0f);
			iCopy.mCooldownTime = this.mCooldownTime;
			iCopy.mHideModel = this.mHideModel;
			iCopy.mHideEffect = this.mHideEffect;
			iCopy.mPauseSounds = this.mPauseSounds;
			iCopy.mSpellQueue.Clear();
			for (int i = 0; i < this.mSpellQueue.Count; i++)
			{
				iCopy.mSpellQueue.Add(this.mSpellQueue[i]);
			}
			this.mSpellEffects.CopyTo(iCopy.mSpellEffects, 0);
			iCopy.mSounds = this.mSounds;
			iCopy.mSoundCues = this.mSoundCues;
			if (this.mPointLight != null)
			{
				this.mPointLight.Disable();
				this.mPointLight = null;
				this.mPointLightHolder.Enabled = false;
			}
			if (iCopy.mPointLight != null)
			{
				iCopy.mPointLight.Disable();
				iCopy.mPointLight = null;
				iCopy.mPointLightHolder.Enabled = false;
			}
			iCopy.mPointLightHolder = this.mPointLightHolder;
			iCopy.mAttach0 = this.mAttach0;
			iCopy.mAttach1 = this.mAttach1;
			iCopy.mEffect0Pos = this.mEffect0Pos;
			iCopy.mEffect1Pos = this.mEffect1Pos;
			iCopy.mBoundingBox = this.mBoundingBox;
			iCopy.mBlockValue = this.mBlockValue;
			this.mResistances.CopyTo(iCopy.mResistances, 0);
			iCopy.mSpecialAbilityCooldown = this.mSpecialAbilityCooldown;
			iCopy.mSpecialAbilityRechargeTime = this.mSpecialAbilityRechargeTime;
			iCopy.mSpecialAbility = this.mSpecialAbility;
			iCopy.mPassiveAbility = this.mPassiveAbility;
			iCopy.mTeslaField = this.mTeslaField;
			iCopy.mGlow = this.mGlow;
			iCopy.mGlowTarget = this.mGlowTarget;
			iCopy.mGungnirMissile = this.mGungnirMissile;
			if (iCopy.mBody != null && base.Model != null)
			{
				Vector3 sideLengths;
				Vector3.Subtract(ref this.mBoundingBox.Max, ref this.mBoundingBox.Min, out sideLengths);
				(iCopy.mCollision.GetPrimitiveLocal(0) as Box).SideLengths = sideLengths;
				(iCopy.mCollision.GetPrimitiveOldWorld(0) as Box).SideLengths = sideLengths;
				(iCopy.mCollision.GetPrimitiveNewWorld(0) as Box).SideLengths = sideLengths;
				Vector3 vector = (this.mBoundingBox.Min + this.mBoundingBox.Max) * 0.5f;
				Vector3 vector2 = iCopy.SetMass(50f);
				Transform transform = default(Transform);
				Vector3.Negate(ref vector2, out transform.Position);
				Vector3.Add(ref transform.Position, ref vector, out transform.Position);
				transform.Orientation = Matrix.Identity;
				iCopy.mCollision.ApplyLocalTransform(transform);
				iCopy.mBody.SetOrientation(base.Transform);
			}
			iCopy.mAuras.Clear();
			for (int j = 0; j < this.mAuras.Count; j++)
			{
				iCopy.mAuras.Add(this.mAuras[j]);
			}
		}

		// Token: 0x1700066E RID: 1646
		// (get) Token: 0x06001A0D RID: 6669 RVA: 0x000B0ABF File Offset: 0x000AECBF
		// (set) Token: 0x06001A0E RID: 6670 RVA: 0x000B0AC7 File Offset: 0x000AECC7
		internal bool IgnoreTractorPull
		{
			get
			{
				return this.mIgnoreTractorPull;
			}
			set
			{
				this.mIgnoreTractorPull = value;
			}
		}

		// Token: 0x1700066F RID: 1647
		// (get) Token: 0x06001A0F RID: 6671 RVA: 0x000B0AD0 File Offset: 0x000AECD0
		// (set) Token: 0x06001A10 RID: 6672 RVA: 0x000B0AD8 File Offset: 0x000AECD8
		internal int AnimatedLevelPartID
		{
			get
			{
				return this.mAnimatedLevelPartID;
			}
			set
			{
				this.mAnimatedLevelPartID = value;
			}
		}

		// Token: 0x06001A11 RID: 6673 RVA: 0x000B0AE1 File Offset: 0x000AECE1
		public void Despawn(float iTime)
		{
			this.mDespawnTime = iTime;
			EffectManager.Instance.Stop(ref this.mDespawnEffect);
		}

		// Token: 0x17000670 RID: 1648
		// (get) Token: 0x06001A12 RID: 6674 RVA: 0x000B0AFA File Offset: 0x000AECFA
		public float DespawnTime
		{
			get
			{
				return this.mDespawnTime;
			}
		}

		// Token: 0x17000671 RID: 1649
		// (get) Token: 0x06001A13 RID: 6675 RVA: 0x000B0B02 File Offset: 0x000AED02
		public string Name
		{
			get
			{
				return this.mName;
			}
		}

		// Token: 0x17000672 RID: 1650
		// (get) Token: 0x06001A14 RID: 6676 RVA: 0x000B0B0A File Offset: 0x000AED0A
		public string PickUpString
		{
			get
			{
				return this.mPickUpString;
			}
		}

		// Token: 0x17000673 RID: 1651
		// (get) Token: 0x06001A15 RID: 6677 RVA: 0x000B0B12 File Offset: 0x000AED12
		public bool AnimationDetached
		{
			get
			{
				return this.mAnimationDetached;
			}
		}

		// Token: 0x06001A16 RID: 6678 RVA: 0x000B0B1A File Offset: 0x000AED1A
		public void AnimationDetach()
		{
			this.mAnimationDetached = true;
		}

		// Token: 0x17000674 RID: 1652
		// (get) Token: 0x06001A17 RID: 6679 RVA: 0x000B0B23 File Offset: 0x000AED23
		public bool Attached
		{
			get
			{
				return this.mAttached;
			}
		}

		// Token: 0x17000675 RID: 1653
		// (get) Token: 0x06001A18 RID: 6680 RVA: 0x000B0B2B File Offset: 0x000AED2B
		public bool SpecialAbilityReady
		{
			get
			{
				return this.mSpecialAbilityCooldown <= 0f && this.mSpecialAbility != null;
			}
		}

		// Token: 0x17000676 RID: 1654
		// (get) Token: 0x06001A19 RID: 6681 RVA: 0x000B0B48 File Offset: 0x000AED48
		public bool CooldownHintTime
		{
			get
			{
				return this.mSpecialAbilityCooldown < 0f && this.mSpecialAbilityCooldown > -2f;
			}
		}

		// Token: 0x17000677 RID: 1655
		// (get) Token: 0x06001A1A RID: 6682 RVA: 0x000B0B66 File Offset: 0x000AED66
		public bool IsCoolingdown
		{
			get
			{
				return this.mCooldownTimer > 0f;
			}
		}

		// Token: 0x06001A1B RID: 6683 RVA: 0x000B0B75 File Offset: 0x000AED75
		public Spell PeekSpell()
		{
			return SpellManager.Instance.Combine(this.mSpellQueue);
		}

		// Token: 0x06001A1C RID: 6684 RVA: 0x000B0B88 File Offset: 0x000AED88
		public Spell RetrieveSpell()
		{
			Spell result = SpellManager.Instance.Combine(this.mSpellQueue);
			this.mSpellQueue.Clear();
			for (int i = 0; i < this.mSpellEffects.Length; i++)
			{
				EffectManager.Instance.Stop(ref this.mSpellEffects[i]);
			}
			return result;
		}

		// Token: 0x17000678 RID: 1656
		// (get) Token: 0x06001A1D RID: 6685 RVA: 0x000B0BDB File Offset: 0x000AEDDB
		public Item.PassiveAbilityStruct PassiveAbility
		{
			get
			{
				return this.mPassiveAbility;
			}
		}

		// Token: 0x17000679 RID: 1657
		// (get) Token: 0x06001A1E RID: 6686 RVA: 0x000B0BE3 File Offset: 0x000AEDE3
		public bool IsBound
		{
			get
			{
				return this.mBound;
			}
		}

		// Token: 0x06001A1F RID: 6687 RVA: 0x000B0BEC File Offset: 0x000AEDEC
		public bool TryAddToQueue(ref Spell iSpell, bool iLightningPrecedence)
		{
			int num = this.mSpellQueue.Count + 1;
			if (this.mSpellQueue.Count < 5)
			{
				this.mSpellQueue.Add(iSpell.Normalize());
				SpellManager.FindOppositesAndCombinables(null, this.mOwner, this.mSpellQueue);
			}
			else
			{
				int num2 = SpellManager.FindOpposites(this.mSpellQueue, 4, iSpell.Element);
				if (num2 >= 0)
				{
					this.mSpellQueue.RemoveAt(num2);
				}
				else
				{
					num2 = SpellManager.FindRevertables(this.mSpellQueue, 4, iSpell.Element);
					if (num2 >= 0)
					{
						Spell value = iSpell + this.mSpellQueue[num2];
						this.mSpellQueue[num2] = value;
					}
					else
					{
						num2 = SpellManager.FindCombines(this.mSpellQueue, 4, iSpell.Element);
						if (num2 >= 0)
						{
							Spell value2 = iSpell + this.mSpellQueue[num2];
							this.mSpellQueue[num2] = value2;
						}
					}
				}
				if (iLightningPrecedence && num2 == -1)
				{
					num2 = SpellManager.FindDifferentElement(this.mSpellQueue, 4, Elements.Lightning);
					if (num2 >= 0)
					{
						Spell value3;
						Spell.DefaultSpell(Elements.Lightning, out value3);
						this.mSpellQueue[num2] = value3;
					}
				}
			}
			if (num == this.mSpellQueue.Count)
			{
				for (int i = 0; i < 11; i++)
				{
					Elements elements = Defines.ElementFromIndex(i);
					if ((iSpell.Element & elements) == elements)
					{
						EffectManager.Instance.Stop(ref this.mSpellEffects[i]);
						Vector3 position = this.Position;
						Vector3 forward = this.mBody.Orientation.Forward;
						EffectManager.Instance.StartEffect(Item.ChantEffects[i], ref position, ref forward, out this.mSpellEffects[i]);
					}
				}
				this.mSpellColor = this.PeekSpell().GetColor();
				return true;
			}
			for (int j = 0; j < 11; j++)
			{
				Elements elements2 = Defines.ElementFromIndex(j);
				if ((iSpell.Element & elements2) == elements2)
				{
					EffectManager.Instance.Stop(ref this.mSpellEffects[j]);
				}
			}
			this.mSpellColor = this.PeekSpell().GetColor();
			return false;
		}

		// Token: 0x06001A20 RID: 6688 RVA: 0x000B0E0C File Offset: 0x000AF00C
		public void StopEffects()
		{
			for (int i = 0; i < this.mSpellEffects.Length; i++)
			{
				EffectManager.Instance.Stop(ref this.mSpellEffects[i]);
			}
			EffectManager.Instance.Stop(ref this.mPassiveAbilityEffect);
			for (int j = 0; j < this.mAuras.Count; j++)
			{
				VisualEffectReference mEffect = this.mAuras[j].mEffect;
				EffectManager.Instance.Stop(ref mEffect);
			}
			for (int k = 0; k < this.mFollowEffects.Length; k++)
			{
				EffectManager.Instance.Stop(ref this.mFollowEffects[k]);
			}
		}

		// Token: 0x1700067A RID: 1658
		// (get) Token: 0x06001A21 RID: 6689 RVA: 0x000B0EAF File Offset: 0x000AF0AF
		public bool SpellCharged
		{
			get
			{
				return this.mSpellQueue.Count > 0;
			}
		}

		// Token: 0x1700067B RID: 1659
		// (get) Token: 0x06001A22 RID: 6690 RVA: 0x000B0EC2 File Offset: 0x000AF0C2
		public StaticList<Spell> SpellList
		{
			get
			{
				return this.mSpellQueue;
			}
		}

		// Token: 0x06001A23 RID: 6691 RVA: 0x000B0ECA File Offset: 0x000AF0CA
		public void ClearHitlist()
		{
			this.mHitlist.Clear();
			this.mNextToDamage = 0;
		}

		// Token: 0x1700067C RID: 1660
		// (get) Token: 0x06001A24 RID: 6692 RVA: 0x000B0EDE File Offset: 0x000AF0DE
		// (set) Token: 0x06001A25 RID: 6693 RVA: 0x000B0EE6 File Offset: 0x000AF0E6
		public Model ProjectileModel
		{
			get
			{
				return this.mProjectileModel;
			}
			set
			{
				if (value != null)
				{
					this.mProjectileModel = value;
				}
			}
		}

		// Token: 0x06001A26 RID: 6694 RVA: 0x000B0EF4 File Offset: 0x000AF0F4
		public override Vector3 CalcImpulseVelocity(Vector3 iDirection, float iElevation, float iMassPower, float iDistance)
		{
			if (!this.mBody.IsBodyEnabled)
			{
				return default(Vector3);
			}
			return base.CalcImpulseVelocity(iDirection, iElevation, iMassPower, iDistance);
		}

		// Token: 0x1700067D RID: 1661
		// (get) Token: 0x06001A27 RID: 6695 RVA: 0x000B0F23 File Offset: 0x000AF123
		public int BlockValue
		{
			get
			{
				return this.mBlockValue;
			}
		}

		// Token: 0x1700067E RID: 1662
		// (get) Token: 0x06001A28 RID: 6696 RVA: 0x000B0F2B File Offset: 0x000AF12B
		public override bool Dead
		{
			get
			{
				return base.Model == null || this.mAttached;
			}
		}

		// Token: 0x1700067F RID: 1663
		// (get) Token: 0x06001A29 RID: 6697 RVA: 0x000B0F3D File Offset: 0x000AF13D
		public override bool Removable
		{
			get
			{
				return this.Dead;
			}
		}

		// Token: 0x06001A2A RID: 6698 RVA: 0x000B0F48 File Offset: 0x000AF148
		public override void Deinitialize()
		{
			if (this.mBody != null)
			{
				this.mBody.SetActive();
			}
			if (!this.mAnimationDetached)
			{
				this.mTimeToRemove = 15f;
			}
			this.mAttached = true;
			this.mSpellQueue.Clear();
			this.StopEffects();
			for (int i = 0; i < this.mSoundCues.Length; i++)
			{
				if (this.mSoundCues[i] != null && !this.mSoundCues[i].IsStopping)
				{
					this.mSoundCues[i].Stop(AudioStopOptions.AsAuthored);
				}
			}
			if (this.mEffects != null)
			{
				for (int j = 0; j < this.mEffects.Length; j++)
				{
					if (j < this.mVisualEffects.Length)
					{
						EffectManager.Instance.Stop(ref this.mVisualEffects[j]);
						this.mVisualEffects[j].ID = -1;
					}
				}
			}
			for (int k = 0; k < this.mFollowEffects.Length; k++)
			{
				EffectManager.Instance.Stop(ref this.mFollowEffects[k]);
			}
			EffectManager.Instance.Stop(ref this.mGunShellsEffect);
			if (this.mPointLight != null)
			{
				this.mPointLight.Disable();
				this.mPointLight = null;
				this.mPointLightHolder.Enabled = false;
			}
			base.Deinitialize();
		}

		// Token: 0x17000680 RID: 1664
		// (get) Token: 0x06001A2B RID: 6699 RVA: 0x000B1080 File Offset: 0x000AF280
		public Matrix AttachAbsoluteTransform
		{
			get
			{
				return this.mAttach0AbsoluteTransform;
			}
		}

		// Token: 0x17000681 RID: 1665
		// (get) Token: 0x06001A2C RID: 6700 RVA: 0x000B1088 File Offset: 0x000AF288
		// (set) Token: 0x06001A2D RID: 6701 RVA: 0x000B1090 File Offset: 0x000AF290
		public Character Owner
		{
			get
			{
				return this.mOwner;
			}
			set
			{
				this.mOwner = value;
			}
		}

		// Token: 0x17000682 RID: 1666
		// (get) Token: 0x06001A2E RID: 6702 RVA: 0x000B1099 File Offset: 0x000AF299
		public bool IsGunClass
		{
			get
			{
				return this.mWeaponClass == WeaponClass.Handgun | this.mWeaponClass == WeaponClass.Rifle | this.mWeaponClass == WeaponClass.Machinegun | this.mWeaponClass == WeaponClass.Heavy;
			}
		}

		// Token: 0x17000683 RID: 1667
		// (get) Token: 0x06001A2F RID: 6703 RVA: 0x000B10C6 File Offset: 0x000AF2C6
		public WeaponClass WeaponClass
		{
			get
			{
				return this.mWeaponClass;
			}
		}

		// Token: 0x17000684 RID: 1668
		// (get) Token: 0x06001A30 RID: 6704 RVA: 0x000B10CE File Offset: 0x000AF2CE
		public List<ActiveAura> Auras
		{
			get
			{
				return this.mAuras;
			}
		}

		// Token: 0x06001A31 RID: 6705 RVA: 0x000B10D6 File Offset: 0x000AF2D6
		public void PrepareToExecute(Ability iAb)
		{
			this.mSourceAbility = iAb;
		}

		// Token: 0x17000685 RID: 1669
		// (get) Token: 0x06001A32 RID: 6706 RVA: 0x000B10DF File Offset: 0x000AF2DF
		public int SpecialAbilityName
		{
			get
			{
				return this.mSpecialAbility.DisplayName;
			}
		}

		// Token: 0x17000686 RID: 1670
		// (get) Token: 0x06001A33 RID: 6707 RVA: 0x000B10EC File Offset: 0x000AF2EC
		public bool HasSpecialAbility
		{
			get
			{
				return this.mSpecialAbility != null;
			}
		}

		// Token: 0x06001A34 RID: 6708 RVA: 0x000B10FC File Offset: 0x000AF2FC
		public void ExecuteSpecialAbility()
		{
			if (this.mSpecialAbilityCooldown <= 0f && this.mSpecialAbility != null)
			{
				this.mOwner.SpecialAbilityAnimation(this.mSpecialAbility.Animation);
				this.mSpecialAbility.Execute(this.mOwner, this.mPlayState);
				if (this.mSpecialAbility is Invisibility)
				{
					this.mOwner.JustCastInvisible = true;
				}
				this.mSpecialAbilityCooldown = this.mSpecialAbilityRechargeTime;
			}
		}

		// Token: 0x06001A35 RID: 6709 RVA: 0x000B1174 File Offset: 0x000AF374
		public void AddEffectReference(int iEffectHash, VisualEffectReference iEffect)
		{
			for (int i = 0; i < this.mFollowEffects.Length; i++)
			{
				if (!EffectManager.Instance.IsActive(ref this.mFollowEffects[i]))
				{
					this.mFollowEffects[i] = iEffect;
					return;
				}
			}
			EffectManager.Instance.Stop(ref iEffect);
		}

		// Token: 0x06001A36 RID: 6710 RVA: 0x000B11CC File Offset: 0x000AF3CC
		public void StopExecute()
		{
			for (int i = 0; i < this.mFollowEffects.Length; i++)
			{
				if (this.mFollowEffects[i].ID != -1)
				{
					EffectManager.Instance.Stop(ref this.mFollowEffects[i]);
				}
			}
		}

		// Token: 0x06001A37 RID: 6711 RVA: 0x000B1218 File Offset: 0x000AF418
		public void StopGunfire()
		{
			if (this.mFiring && (!(this.mOwner is Avatar) || (this.mOwner is Avatar && ((!this.mOwner.Attacking && this.mWeaponClass == WeaponClass.Machinegun) || (this.mWeaponClass == WeaponClass.Rifle | this.mWeaponClass == WeaponClass.Handgun)))))
			{
				this.mGunCurrentClip = 0;
				this.mGunRateTimer = 0f;
				this.mFiring = false;
				if (this.mGunSound != null && !this.mGunSound.IsStopping)
				{
					this.mGunSound.Stop(AudioStopOptions.AsAuthored);
				}
				EffectManager.Instance.Stop(ref this.mGunShellsEffect);
			}
		}

		// Token: 0x06001A38 RID: 6712 RVA: 0x000B12C4 File Offset: 0x000AF4C4
		public void ExecuteGun(float iAccuracy)
		{
			if (this.mGunConditions[0] == null && this.mGunConditions[1] == null && this.mGunConditions[2] == null && this.mGunConditions[3] == null && this.mGunConditions[4] == null)
			{
				return;
			}
			this.mGunCurrentAccuracy = this.mGunAccuracy * iAccuracy;
			this.mGunCurrentClip = this.mGunClip;
			if (this.mGunRateTimer <= 0f)
			{
				this.mGunRateTimer = float.Epsilon;
				this.mGunSound = AudioManager.Instance.PlayCue(this.mGunSoundBank, this.mGunSoundID, this.mAudioEmitter);
				EffectManager.Instance.StartEffect(this.mGunShellsEffectID, ref this.mAttach1AbsoluteTransform, out this.mGunShellsEffect);
			}
			this.mFiring = true;
			this.mCooldownTimer = this.mCooldownTime;
		}

		// Token: 0x06001A39 RID: 6713 RVA: 0x000B139C File Offset: 0x000AF59C
		public virtual void Execute(DealDamage.Targets iTargets)
		{
			if (this.mMeleeConditions[0] == null && this.mMeleeConditions[1] == null && this.mMeleeConditions[2] == null && this.mMeleeConditions[3] == null && this.mMeleeConditions[4] == null)
			{
				return;
			}
			IDamageable damageable = null;
			if (this.mOwner is NonPlayerCharacter)
			{
				damageable = (this.mOwner as NonPlayerCharacter).AI.CurrentTarget;
			}
			if (this.mType == Item.TYRFING_TYPE & this.mTeleportCooldown <= 0f)
			{
				this.mTeleportCooldown = 0.5f;
				List<Entity> entities = this.mOwner.PlayState.EntityManager.GetEntities(this.mOwner.Position, 20f, false);
				float num = 1.5707964f;
				float num2 = float.MaxValue;
				Character character = null;
				Vector3 direction = this.mOwner.Direction;
				for (int i = 0; i < entities.Count; i++)
				{
					if (entities[i] is Character && entities[i] != this.mOwner)
					{
						Vector3 vector = entities[i].Position - this.mOwner.Position;
						float num3 = vector.Length();
						vector.Normalize();
						float num4 = MagickaMath.Angle(ref vector, ref direction);
						if (num4 < num && num4 < 0.3926991f && num3 < num2)
						{
							num = num4;
							num2 = num3;
							character = (entities[i] as Character);
						}
					}
				}
				if (character != null && num2 > this.mOwner.Radius + 3f + character.Radius && NetworkManager.Instance.State != NetworkState.Client)
				{
					Vector3 vector2 = this.mOwner.Position - character.Position;
					vector2.Y = 0f;
					vector2.Normalize();
					vector2 *= character.Radius + this.mOwner.Radius + 0.2f;
					vector2 += character.Position;
					Vector3 iDirection = character.Position - vector2;
					iDirection.Y = 0f;
					iDirection.Normalize();
					if (NetworkManager.Instance.State == NetworkState.Server)
					{
						CharacterActionMessage characterActionMessage = default(CharacterActionMessage);
						characterActionMessage.Handle = this.mOwner.Handle;
						characterActionMessage.Param0F = vector2.X;
						characterActionMessage.Param1F = vector2.Y;
						characterActionMessage.Param2F = vector2.Z;
						characterActionMessage.Param4I = 1;
						characterActionMessage.TargetHandle = character.Handle;
						characterActionMessage.Action = ActionType.Magick;
						characterActionMessage.Param3I = 5;
						NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref characterActionMessage);
					}
					Teleport.Instance.DoTeleport(this.mOwner, vector2, iDirection, Teleport.TeleportType.Regular);
				}
				this.mOwner.PlayState.EntityManager.ReturnEntityList(entities);
			}
			this.mContinueHitting = (this.mHitlist.Count == 0);
			if ((!this.mMeleeMultiHit && !this.mContinueHitting) || this.mShieldDeflectCoolDown >= 0f)
			{
				return;
			}
			this.mCooldownTimer = this.mCooldownTime;
			Segment iSeg = default(Segment);
			iSeg.Origin = this.mAttach0AbsoluteTransform.Translation;
			Vector3.Subtract(ref this.mLastAttachAbsolutePosition, ref iSeg.Origin, out iSeg.Delta);
			List<Shield> shields = this.mPlayState.EntityManager.Shields;
			Segment iSeg2 = default(Segment);
			iSeg2.Origin = this.mOwner.Position;
			Vector3.Subtract(ref iSeg.Origin, ref iSeg2.Origin, out iSeg2.Delta);
			for (int j = 0; j < shields.Count; j++)
			{
				Shield shield = shields[j];
				Vector3 vector3;
				if (shield.SegmentIntersect(out vector3, iSeg, this.mMeleeRange) || shield.SegmentIntersect(out vector3, iSeg2, this.mMeleeRange))
				{
					EventCondition eventCondition = default(EventCondition);
					eventCondition.EventConditionType = EventConditionType.Hit;
					DamageResult damageResult;
					this.mMeleeConditions.ExecuteAll(this.mOwner, shield, ref eventCondition, out damageResult);
					DamageResult damageResult2 = DamageResult.Damaged | DamageResult.Hit | DamageResult.Killed;
					if (this.mPassiveAbility.Ability == Item.PassiveAbilities.Mjolnr && (damageResult & damageResult2) != DamageResult.None)
					{
						Magick magick = default(Magick);
						magick.MagickType = MagickType.ThunderB;
						magick.Effect.Execute(this.mOwner, this.mPlayState);
					}
					this.mShieldDeflectCoolDown = 0.25f;
					this.mOwner.GoToAnimation(Animations.attack_recoil, 0.1f);
					return;
				}
			}
			List<Entity> entities2 = this.mPlayState.EntityManager.GetEntities(this.mAttach0AbsoluteTransform.Translation, this.mMeleeRange + 5f, true);
			for (int k = 0; k < entities2.Count; k++)
			{
				IDamageable damageable2 = entities2[k] as IDamageable;
				if (damageable2 != this.mOwner && damageable2 != null && !this.mHitlist.Contains(damageable2))
				{
					if (damageable2 != damageable)
					{
						if (damageable2 is Character)
						{
							bool flag = ((damageable2 as Character).Faction & this.mOwner.Faction) != Factions.NONE;
							if (flag && (byte)(iTargets & DealDamage.Targets.Friendly) == 0)
							{
								goto IL_54F;
							}
							if (!flag && (byte)(iTargets & DealDamage.Targets.Enemy) == 0)
							{
								goto IL_54F;
							}
						}
						else if ((byte)(iTargets & DealDamage.Targets.NonCharacters) == 0)
						{
							goto IL_54F;
						}
					}
					Vector3 vector4;
					if (damageable2.SegmentIntersect(out vector4, iSeg, this.mMeleeRange))
					{
						this.mHitlist.Add(damageable2);
						if (!this.mMeleeMultiHit)
						{
							break;
						}
					}
				}
				IL_54F:;
			}
			this.mPlayState.EntityManager.ReturnEntityList(entities2);
			if (this.mWorldCollisionTimer <= 0f)
			{
				int l = 0;
				while (l < this.mMeleeConditions.Count)
				{
					if (this.mMeleeConditions[l] != null && this.mMeleeConditions[l].Condition.EventConditionType == EventConditionType.Collision)
					{
						float num5;
						Vector3 vector5;
						Vector3 vector6;
						AnimatedLevelPart animatedLevelPart;
						if (this.mPlayState.Level.CurrentScene.SegmentIntersect(out num5, out vector5, out vector6, out animatedLevelPart, iSeg))
						{
							EventCondition eventCondition2 = default(EventCondition);
							eventCondition2.EventConditionType = EventConditionType.Collision;
							DamageResult damageResult3;
							this.mMeleeConditions.ExecuteAll(this, null, ref eventCondition2, out damageResult3);
							this.mOwner.GoToAnimation(Animations.attack_recoil, 0.1f);
							this.mWorldCollisionTimer = 0.25f;
							break;
						}
						break;
					}
					else
					{
						l++;
					}
				}
			}
			for (int m = this.mNextToDamage; m < this.mHitlist.Count; m++)
			{
				IDamageable damageable3 = this.mHitlist[m];
				EventCondition eventCondition3 = default(EventCondition);
				eventCondition3.EventConditionType = EventConditionType.Hit;
				DamageResult damageResult4;
				this.mMeleeConditions.ExecuteAll(this, (Entity)damageable3, ref eventCondition3, out damageResult4);
				if (this.mPassiveAbility.Ability == Item.PassiveAbilities.DragonSlayer && !((this.mOwner as Avatar).Player.Gamer is NetworkGamer) && damageable3 is BossDamageZone && (damageable3 as BossDamageZone).Owner is Fafnir)
				{
					AchievementsManager.Instance.AwardAchievement(base.PlayState, "stuffoflegends");
					damageable3.OverKill();
					damageResult4 = DamageResult.Hit;
				}
				DamageResult damageResult5 = DamageResult.Damaged | DamageResult.Hit | DamageResult.Killed;
				if (this.mPassiveAbility.Ability == Item.PassiveAbilities.Mjolnr && (damageResult4 & damageResult5) != DamageResult.None)
				{
					this.MjolnirStrike(damageable3);
				}
				if (this.mType == Item.SONIC_SCREWDRIVER && (damageResult4 & DamageResult.Healed) == DamageResult.Healed)
				{
					if (damageable3 is ISpellCaster)
					{
						Vector3 direction2 = this.mOwner.Direction;
						Vector3 position = damageable3.Position;
						VisualEffectReference visualEffectReference;
						EffectManager.Instance.StartEffect(Item.SONIC_SCREWDRIVER_HITEFFECT, ref position, ref direction2, out visualEffectReference);
						Haste.GetInstance().Execute((ISpellCaster)damageable3, base.PlayState, true);
					}
					else if (damageable3 is DamageablePhysicsEntity)
					{
						(damageable3 as DamageablePhysicsEntity).Damage(100f, Elements.None);
					}
				}
				if ((damageResult4 & (DamageResult.Killed | DamageResult.OverKilled)) != DamageResult.None && damageable3 is Entity)
				{
					AudioManager.Instance.PlayCue(Banks.Weapons, Item.DEATH_SOUND, (damageable3 as Entity).AudioEmitter);
				}
				this.mContinueHitting = (this.mContinueHitting && (damageResult4 & (DamageResult.Knockeddown | DamageResult.Knockedback | DamageResult.Pushed | DamageResult.Killed)) != DamageResult.None);
				if (!this.mContinueHitting && (damageResult4 & DamageResult.Deflected) == DamageResult.Deflected)
				{
					this.mOwner.GoToAnimation(Animations.attack_recoil, 0.1f);
				}
				this.mNextToDamage = m + 1;
				if (!this.mMeleeMultiHit)
				{
					return;
				}
			}
		}

		// Token: 0x06001A3A RID: 6714 RVA: 0x000B1BD8 File Offset: 0x000AFDD8
		public void ExecuteRanged(ref MissileEntity iMissile, Vector3? iVelocity, bool iItemAligned)
		{
			if (this.mRangedConditions[0] == null && this.mRangedConditions[1] == null && this.mRangedConditions[2] == null && this.mRangedConditions[3] == null && this.mRangedConditions[4] == null)
			{
				return;
			}
			NetworkState state = NetworkManager.Instance.State;
			if (iMissile == null && (state == NetworkState.Client || (this.mOwner is Avatar && (this.mOwner as Avatar).Player.Gamer is NetworkGamer)) && (state != NetworkState.Client || !(this.mOwner is Avatar) || (this.mOwner as Avatar).Player.Gamer is NetworkGamer))
			{
				return;
			}
			if (this.mPassiveAbility.Ability == Item.PassiveAbilities.Gungner && this.mGungnirMissile != null)
			{
				return;
			}
			if (this.mPassiveAbility.Ability == Item.PassiveAbilities.MasterSword && this.mOwner.HitPoints < this.mOwner.MaxHitPoints)
			{
				return;
			}
			this.mCooldownTimer = this.mCooldownTime;
			if (iMissile == null)
			{
				iMissile = this.mOwner.GetMissileInstance();
			}
			iMissile.Danger = this.mRangedDanger;
			iMissile.FacingVelocity = this.mFacing;
			Entity entity = null;
			Vector3 value = this.mOwner.CharacterBody.Direction;
			Vector3 translation = this.mAttach0AbsoluteTransform.Translation;
			if (iVelocity != null)
			{
				Matrix orientation;
				if (iItemAligned)
				{
					orientation = this.mBody.Orientation;
				}
				else
				{
					orientation = this.mOwner.Body.Orientation;
				}
				value = iVelocity.Value;
				Vector3.TransformNormal(ref value, ref orientation, out value);
				if (this.mSourceAbility != null && this.mSourceAbility is Ranged && (this.mOwner as NonPlayerCharacter).AI.CurrentTarget != null)
				{
					entity = ((this.mOwner as NonPlayerCharacter).AI.CurrentTarget as Entity);
				}
			}
			else if (this.mSourceAbility != null && this.mSourceAbility is Ranged && (this.mOwner as NonPlayerCharacter).AI.CurrentTarget != null)
			{
				Vector3 position = (this.mOwner as NonPlayerCharacter).AI.CurrentTarget.Position;
				entity = ((this.mOwner as NonPlayerCharacter).AI.CurrentTarget as Entity);
				Vector3 position2 = this.Position;
				float num = position.Y - position2.Y;
				position.Y = (position2.Y = 0f);
				float num2;
				Vector3.Distance(ref position, ref position2, out num2);
				float num3 = (this.mSourceAbility as Ranged).GetElevation() + this.mRangedElevation;
				float num4 = value.Y = (float)Math.Sin((double)num3);
				float num5 = (float)Math.Cos((double)num3);
				value.X *= num5;
				value.Z *= num5;
				float num6 = (this.mSourceAbility as Ranged).GetAccuracy();
				num6 *= MagickaMath.RandomBetween(-1f, 1f) * Math.Max(1f - num6, 0f);
				float num7 = (float)Math.Sqrt((double)(PhysicsManager.Instance.Simulator.Gravity.Y * -1f * num2 * num2 / (2f * (num2 * num4 / num5 - num) * num5 * num5)));
				if (float.IsNaN(num7) || float.IsInfinity(num7))
				{
					num7 = this.RangedRange * 2f;
				}
				num7 *= num6 + 1f;
				num6 *= 0.7853982f;
				Quaternion quaternion;
				Quaternion.CreateFromYawPitchRoll(num6, 0f, 0f, out quaternion);
				Vector3.Transform(ref value, ref quaternion, out value);
				Vector3.Multiply(ref value, num7, out value);
			}
			else
			{
				if (this.mRangedElevation > 0f)
				{
					float y = (float)Math.Sin((double)this.mRangedElevation);
					float num8 = (float)Math.Cos((double)this.mRangedElevation);
					value.Y = y;
					value.X *= num8;
					value.Z *= num8;
				}
				else
				{
					value.Y = 0.05f;
				}
				value *= this.mRangedRange * 2f;
			}
			if (this.mPassiveAbility.Ability == Item.PassiveAbilities.Gungner)
			{
				iMissile.Initialize(this.mOwner, 0.5f, ref translation, ref value, this.mProjectileModel, this.mRangedConditions, false);
				this.mGungnirMissile = iMissile;
				iMissile.FacingVelocity = true;
			}
			else if (entity != null)
			{
				if (this.mProjectileModel != null && this.mWeaponClass != WeaponClass.Heavy)
				{
					iMissile.Initialize(this.mOwner, entity, this.mHoming, this.mProjectileModel.Meshes[0].BoundingSphere.Radius, ref translation, ref value, this.mProjectileModel, this.mRangedConditions, false);
				}
				else
				{
					iMissile.Initialize(this.mOwner, entity, this.mHoming, 0.25f, ref translation, ref value, this.mProjectileModel, this.mRangedConditions, false);
				}
			}
			else if (this.mProjectileModel != null && this.mWeaponClass != WeaponClass.Heavy)
			{
				iMissile.Initialize(this.mOwner, this.mProjectileModel.Meshes[0].BoundingSphere.Radius, ref translation, ref value, this.mProjectileModel, this.mRangedConditions, false);
			}
			else
			{
				iMissile.Initialize(this.mOwner, 0.25f, ref translation, ref value, this.mProjectileModel, this.mRangedConditions, false);
			}
			if (NetworkManager.Instance.State != NetworkState.Offline)
			{
				SpawnMissileMessage spawnMissileMessage = default(SpawnMissileMessage);
				spawnMissileMessage.Type = SpawnMissileMessage.MissileType.Item;
				spawnMissileMessage.Owner = this.mOwner.Handle;
				spawnMissileMessage.Item = base.Handle;
				spawnMissileMessage.Handle = iMissile.Handle;
				spawnMissileMessage.Position = iMissile.Position;
				spawnMissileMessage.Velocity = iMissile.Body.Velocity;
				spawnMissileMessage.Homing = this.mHoming;
				NetworkManager.Instance.Interface.SendMessage<SpawnMissileMessage>(ref spawnMissileMessage);
			}
			this.mPlayState.EntityManager.AddEntity(iMissile);
		}

		// Token: 0x06001A3B RID: 6715 RVA: 0x000B21F4 File Offset: 0x000B03F4
		private void MjolnirStrike(IDamageable iTarget)
		{
			Damage mjolnir_DAMAGE = Defines.MJOLNIR_DAMAGE;
			if (this.mPlayState.Level.CurrentScene.Indoors)
			{
				return;
			}
			Flash.Instance.Execute(this.mPlayState.Scene, 0.125f);
			Vector3 vector = iTarget.Position;
			Segment iSeg = default(Segment);
			iSeg.Origin = vector;
			iSeg.Origin.Y = iSeg.Origin.Y + 25f;
			iSeg.Delta.Y = iSeg.Delta.Y - 35f;
			List<Shield> shields = this.mPlayState.EntityManager.Shields;
			for (int i = 0; i < shields.Count; i++)
			{
				if (shields[i].ShieldType == ShieldType.SPHERE && shields[i].SegmentIntersect(out vector, iSeg, 0.2f))
				{
					iTarget = shields[i];
					vector.Y += iTarget.Body.CollisionSkin.WorldBoundingBox.Max.Y * 0.5f;
					break;
				}
			}
			if (!(iTarget is Shield))
			{
				iSeg.Origin = vector;
				iSeg.Delta.Y = iSeg.Delta.Y - 10f;
				float num;
				Vector3 vector2;
				Vector3 vector3;
				AnimatedLevelPart iAnimation;
				if (this.mPlayState.Level.CurrentScene.SegmentIntersect(out num, out vector2, out vector3, out iAnimation, iSeg))
				{
					vector = vector2;
					DecalManager.Instance.AddAlphaBlendedDecal(Decal.Scorched, iAnimation, 4f, ref vector, ref vector3, 60f);
				}
			}
			LightningBolt lightning = LightningBolt.GetLightning();
			Vector3 vector4 = vector;
			vector4.Y += 40f;
			Vector3 vector5 = new Vector3(0f, -1f, 0f);
			Vector3 lightningcolor = Spell.LIGHTNINGCOLOR;
			Vector3 position = this.mPlayState.Scene.Camera.Position;
			float iScale = 1f;
			lightning.InitializeEffect(ref vector4, ref vector5, ref vector, ref position, ref lightningcolor, false, iScale, 1f, this.mPlayState);
			iTarget.Damage(mjolnir_DAMAGE, this.mOwner, this.mOwner.PlayState.PlayTime, this.mOwner.Position);
			vector5 = Vector3.Right;
			VisualEffectReference visualEffectReference;
			EffectManager.Instance.StartEffect(Thunderbolt.EFFECT, ref vector, ref vector5, out visualEffectReference);
			AudioManager.Instance.PlayCue(Banks.Spells, Thunderbolt.SOUND, (iTarget as Entity).AudioEmitter);
			this.mPlayState.Camera.CameraShake(vector, 1.5f, 0.333f);
		}

		// Token: 0x06001A3C RID: 6716 RVA: 0x000B2468 File Offset: 0x000B0668
		protected override void AddImpulseVelocity(ref Vector3 iVelocity)
		{
			AnimatedLevelPart animatedLevelPart;
			if (this.mAnimatedLevelPartID != 0 && !this.mIgnoreTractorPull && this.mPlayState.Level.CurrentScene.LevelModel.AnimatedLevelParts.TryGetValue(this.mAnimatedLevelPartID, out animatedLevelPart))
			{
				animatedLevelPart.RemoveEntity(this);
				this.AnimatedLevelPartID = 0;
			}
			base.AddImpulseVelocity(ref iVelocity);
		}

		// Token: 0x17000687 RID: 1671
		// (get) Token: 0x06001A3D RID: 6717 RVA: 0x000B24C3 File Offset: 0x000B06C3
		public Resistance[] Resistance
		{
			get
			{
				return this.mResistances;
			}
		}

		// Token: 0x17000688 RID: 1672
		// (get) Token: 0x06001A3E RID: 6718 RVA: 0x000B24CB File Offset: 0x000B06CB
		public float RangedRange
		{
			get
			{
				return this.mRangedRange;
			}
		}

		// Token: 0x17000689 RID: 1673
		// (get) Token: 0x06001A3F RID: 6719 RVA: 0x000B24D3 File Offset: 0x000B06D3
		public float Homing
		{
			get
			{
				return this.mHoming;
			}
		}

		// Token: 0x1700068A RID: 1674
		// (get) Token: 0x06001A40 RID: 6720 RVA: 0x000B24DB File Offset: 0x000B06DB
		public bool Facing
		{
			get
			{
				return this.mFacing;
			}
		}

		// Token: 0x1700068B RID: 1675
		// (get) Token: 0x06001A41 RID: 6721 RVA: 0x000B24E3 File Offset: 0x000B06E3
		public float Danger
		{
			get
			{
				return this.mRangedDanger;
			}
		}

		// Token: 0x1700068C RID: 1676
		// (get) Token: 0x06001A42 RID: 6722 RVA: 0x000B24EB File Offset: 0x000B06EB
		// (set) Token: 0x06001A43 RID: 6723 RVA: 0x000B24F3 File Offset: 0x000B06F3
		public ConditionCollection MeleeConditions
		{
			get
			{
				return this.mMeleeConditions;
			}
			set
			{
				this.mMeleeConditions = value;
			}
		}

		// Token: 0x1700068D RID: 1677
		// (get) Token: 0x06001A44 RID: 6724 RVA: 0x000B24FC File Offset: 0x000B06FC
		// (set) Token: 0x06001A45 RID: 6725 RVA: 0x000B2504 File Offset: 0x000B0704
		public ConditionCollection RangedConditions
		{
			get
			{
				return this.mRangedConditions;
			}
			set
			{
				this.mRangedConditions = value;
			}
		}

		// Token: 0x06001A46 RID: 6726 RVA: 0x000B2510 File Offset: 0x000B0710
		public void Detach()
		{
			this.mAttached = false;
			this.mBody.Velocity = default(Vector3);
			this.StopEffects();
			for (int i = 0; i < this.mSoundCues.Length; i++)
			{
				if (this.mSoundCues[i] != null && !this.mSoundCues[i].IsStopping)
				{
					this.mSoundCues[i].Stop(AudioStopOptions.AsAuthored);
				}
			}
			if (this.mEffects != null)
			{
				for (int j = 0; j < this.mVisualEffects.Length; j++)
				{
					EffectManager.Instance.Stop(ref this.mVisualEffects[j]);
				}
			}
			if (this.mPointLight != null)
			{
				this.mPointLight.Disable();
				this.mPointLight = null;
				this.mPointLightHolder.Enabled = false;
			}
			if (this.mGunSound != null && !this.mGunSound.IsStopping)
			{
				this.mGunSound.Stop(AudioStopOptions.AsAuthored);
				this.mGunSound = null;
			}
			EffectManager.Instance.Stop(ref this.mGunShellsEffect);
			for (int k = 0; k < this.mAuras.Count; k++)
			{
				VisualEffectReference mEffect = this.mAuras[k].mEffect;
				EffectManager.Instance.Stop(ref mEffect);
			}
			if (this.mPlayState.Level.CurrentScene.RuleSet is VersusRuleset)
			{
				this.Despawn(20f);
			}
		}

		// Token: 0x1700068E RID: 1678
		// (get) Token: 0x06001A47 RID: 6727 RVA: 0x000B2663 File Offset: 0x000B0863
		public int Type
		{
			get
			{
				return this.mType;
			}
		}

		// Token: 0x1700068F RID: 1679
		// (get) Token: 0x06001A48 RID: 6728 RVA: 0x000B266B File Offset: 0x000B086B
		protected override bool HideModel
		{
			get
			{
				return (this.mWeaponClass != WeaponClass.Staff && this.mHideModel && this.IsCoolingdown) || (this.mWeaponClass == WeaponClass.Staff && this.mHideModel && !this.SpecialAbilityReady);
			}
		}

		// Token: 0x17000690 RID: 1680
		// (get) Token: 0x06001A49 RID: 6729 RVA: 0x000B26A6 File Offset: 0x000B08A6
		private bool HideEffects
		{
			get
			{
				return (this.mWeaponClass != WeaponClass.Staff && this.mHideEffect && this.IsCoolingdown) || (this.mWeaponClass == WeaponClass.Staff && this.mHideEffect && !this.SpecialAbilityReady);
			}
		}

		// Token: 0x17000691 RID: 1681
		// (get) Token: 0x06001A4A RID: 6730 RVA: 0x000B26E1 File Offset: 0x000B08E1
		private bool PauseSounds
		{
			get
			{
				return (this.mWeaponClass != WeaponClass.Staff && this.mPauseSounds && this.IsCoolingdown) || (this.mWeaponClass == WeaponClass.Staff && this.mPauseSounds && !this.SpecialAbilityReady);
			}
		}

		// Token: 0x06001A4B RID: 6731 RVA: 0x000B271C File Offset: 0x000B091C
		public override void Kill()
		{
			base.Model = null;
			this.mDespawnTime = 0f;
			EffectManager.Instance.Stop(ref this.mDespawnEffect);
		}

		// Token: 0x06001A4C RID: 6732 RVA: 0x000B2740 File Offset: 0x000B0940
		protected override void IGetNetworkUpdate(out EntityUpdateMessage oMsg, float iPrediction)
		{
			oMsg = default(EntityUpdateMessage);
			if (this.mPickable & this.mBody.IsActive)
			{
				Transform transform = this.mBody.Transform;
				TransformRate transformRate = this.mBody.TransformRate;
				transform.ApplyTransformRate(ref transformRate, iPrediction);
				oMsg.Features |= EntityFeatures.Position;
				oMsg.Position = transform.Position;
				oMsg.Features |= EntityFeatures.Orientation;
				Quaternion.CreateFromRotationMatrix(ref transform.Orientation, out oMsg.Orientation);
				oMsg.Features |= EntityFeatures.Velocity;
				oMsg.Velocity = this.mBody.Velocity;
			}
		}

		// Token: 0x17000692 RID: 1682
		// (get) Token: 0x06001A4D RID: 6733 RVA: 0x000B27E7 File Offset: 0x000B09E7
		public override bool Permanent
		{
			get
			{
				return this.mPickable && !this.Dead && this.mDespawnTime <= 0f;
			}
		}

		// Token: 0x06001A4E RID: 6734 RVA: 0x000B280B File Offset: 0x000B0A0B
		internal new void SetUniqueID(int iID)
		{
			base.SetUniqueID(iID);
		}

		// Token: 0x04001C2D RID: 7213
		private static Queue<Item> sPickableCache;

		// Token: 0x04001C2E RID: 7214
		public static readonly int[] ChantEffects = new int[]
		{
			"swordelementeffect_earth".GetHashCodeCustom(),
			"swordelementeffect_water".GetHashCodeCustom(),
			"swordelementeffect_cold".GetHashCodeCustom(),
			"swordelementeffect_fire".GetHashCodeCustom(),
			"swordelementeffect_lightning".GetHashCodeCustom(),
			"swordelementeffect_arcane".GetHashCodeCustom(),
			"swordelementeffect_life".GetHashCodeCustom(),
			"swordelementeffect_shield".GetHashCodeCustom(),
			"swordelementeffect_ice".GetHashCodeCustom(),
			"swordelementeffect_steam".GetHashCodeCustom(),
			"swordelementeffect_cold".GetHashCodeCustom(),
			"swordelementeffect_cold".GetHashCodeCustom(),
			"swordelementeffect_cold".GetHashCodeCustom()
		};

		// Token: 0x04001C2F RID: 7215
		public static readonly int SPECIAL_READY_EFFECT = "special_ready".GetHashCodeCustom();

		// Token: 0x04001C30 RID: 7216
		public static readonly int RECHARGE_SOUND = "staff_recharge01".GetHashCodeCustom();

		// Token: 0x04001C31 RID: 7217
		public static readonly int TYRFING_TYPE = "weapon_tyrfing".GetHashCodeCustom();

		// Token: 0x04001C32 RID: 7218
		public static readonly int SONIC_SCREWDRIVER = "weapon_sonicscrewdriver".GetHashCodeCustom();

		// Token: 0x04001C33 RID: 7219
		public static readonly int SONIC_SCREWDRIVER_HITEFFECT = "magick_generic_2".GetHashCodeCustom();

		// Token: 0x04001C34 RID: 7220
		private static Dictionary<int, Item> CachedWeapons = new Dictionary<int, Item>();

		// Token: 0x04001C35 RID: 7221
		private static Random sRandom = new Random();

		// Token: 0x04001C36 RID: 7222
		private Character mOwner;

		// Token: 0x04001C37 RID: 7223
		private Matrix mAttach0;

		// Token: 0x04001C38 RID: 7224
		private Matrix mAttach1;

		// Token: 0x04001C39 RID: 7225
		private Vector3 mEffect0Pos;

		// Token: 0x04001C3A RID: 7226
		private Vector3 mEffect1Pos;

		// Token: 0x04001C3B RID: 7227
		protected Matrix mAttach0AbsoluteTransform;

		// Token: 0x04001C3C RID: 7228
		protected Matrix mAttach1AbsoluteTransform;

		// Token: 0x04001C3D RID: 7229
		protected Vector3 mEffect0AbsolutePos;

		// Token: 0x04001C3E RID: 7230
		protected Vector3 mEffect1AbsolutePos;

		// Token: 0x04001C3F RID: 7231
		protected Vector3 mLastAttachAbsolutePosition;

		// Token: 0x04001C40 RID: 7232
		private Item.ElementRenderData[] mElementRenderData;

		// Token: 0x04001C41 RID: 7233
		private static readonly int DEATH_SOUND = "wep_deathblow".GetHashCodeCustom();

		// Token: 0x04001C42 RID: 7234
		private static readonly int[] GUN_HIT_EFFECTS = new int[]
		{
			"gunhit_generic".GetHashCodeCustom(),
			"gunhit_gravel".GetHashCodeCustom(),
			"gunhit_grass".GetHashCodeCustom(),
			"gunhit_wood".GetHashCodeCustom(),
			"gunhit_snow".GetHashCodeCustom(),
			"gunhit_stone".GetHashCodeCustom(),
			"gunhit_mud".GetHashCodeCustom(),
			"gunhit_generic".GetHashCodeCustom(),
			"gunhit_water".GetHashCodeCustom(),
			"gunhit_lava".GetHashCodeCustom()
		};

		// Token: 0x04001C43 RID: 7235
		private static readonly int GUN_WATER_HIT_EFFECT = "footstep_water".GetHashCodeCustom();

		// Token: 0x04001C44 RID: 7236
		private static readonly int GUN_ICE_HIT_EFFECT = "footstep_snow".GetHashCodeCustom();

		// Token: 0x04001C45 RID: 7237
		private static readonly int GUN_LAVA_HIT_EFFECT = "footstep_lava".GetHashCodeCustom();

		// Token: 0x04001C46 RID: 7238
		private static readonly int GUN_CRUST_HIT_EFFECT = "footstep_gravel".GetHashCodeCustom();

		// Token: 0x04001C47 RID: 7239
		private Model mProjectileModel;

		// Token: 0x04001C48 RID: 7240
		protected bool mMeleeMultiHit;

		// Token: 0x04001C49 RID: 7241
		protected float mHoming;

		// Token: 0x04001C4A RID: 7242
		protected float mRangedDanger;

		// Token: 0x04001C4B RID: 7243
		protected float mRangedElevation;

		// Token: 0x04001C4C RID: 7244
		protected bool mContinueHitting;

		// Token: 0x04001C4D RID: 7245
		private float mTeleportCooldown;

		// Token: 0x04001C4E RID: 7246
		private float mShieldDeflectCoolDown;

		// Token: 0x04001C4F RID: 7247
		private float mScale = 1f;

		// Token: 0x04001C50 RID: 7248
		private float mTimeToRemove = 15f;

		// Token: 0x04001C51 RID: 7249
		protected float mMeleeRange;

		// Token: 0x04001C52 RID: 7250
		protected ConditionCollection mMeleeConditions;

		// Token: 0x04001C53 RID: 7251
		private float mRangedRange;

		// Token: 0x04001C54 RID: 7252
		private bool mFacing;

		// Token: 0x04001C55 RID: 7253
		private ConditionCollection mRangedConditions;

		// Token: 0x04001C56 RID: 7254
		private ConditionCollection mGunConditions;

		// Token: 0x04001C57 RID: 7255
		private Banks mGunSoundBank;

		// Token: 0x04001C58 RID: 7256
		private int mGunSoundID;

		// Token: 0x04001C59 RID: 7257
		private int mGunMuzzleEffectID;

		// Token: 0x04001C5A RID: 7258
		private int mGunShellsEffectID;

		// Token: 0x04001C5B RID: 7259
		private int mGunClip;

		// Token: 0x04001C5C RID: 7260
		private int mGunCurrentClip;

		// Token: 0x04001C5D RID: 7261
		private float mGunAccuracy;

		// Token: 0x04001C5E RID: 7262
		private float mGunCurrentAccuracy;

		// Token: 0x04001C5F RID: 7263
		private float mGunRate;

		// Token: 0x04001C60 RID: 7264
		private float mGunRateTimer;

		// Token: 0x04001C61 RID: 7265
		private float mDespawnTime;

		// Token: 0x04001C62 RID: 7266
		private VisualEffectReference mDespawnEffect;

		// Token: 0x04001C63 RID: 7267
		private float mGunRange;

		// Token: 0x04001C64 RID: 7268
		private bool mFiring;

		// Token: 0x04001C65 RID: 7269
		private float mTracerVelocity = 100f;

		// Token: 0x04001C66 RID: 7270
		private float mTracerSprite = 35f;

		// Token: 0x04001C67 RID: 7271
		private float mNonTracerSprite = -1f;

		// Token: 0x04001C68 RID: 7272
		private uint mTracerCount;

		// Token: 0x04001C69 RID: 7273
		private Cue mGunSound;

		// Token: 0x04001C6A RID: 7274
		private VisualEffectReference mGunShellsEffect;

		// Token: 0x04001C6B RID: 7275
		private StaticList<Spell> mSpellQueue;

		// Token: 0x04001C6C RID: 7276
		private VisualEffectReference[] mSpellEffects;

		// Token: 0x04001C6D RID: 7277
		private int[] mEffects;

		// Token: 0x04001C6E RID: 7278
		private VisualEffectReference[] mFollowEffects = new VisualEffectReference[4];

		// Token: 0x04001C6F RID: 7279
		private VisualEffectReference[] mVisualEffects = new VisualEffectReference[4];

		// Token: 0x04001C70 RID: 7280
		private KeyValuePair<int, Banks>[] mSounds;

		// Token: 0x04001C71 RID: 7281
		private Cue[] mSoundCues = new Cue[4];

		// Token: 0x04001C72 RID: 7282
		private Item.PointLightHolder mPointLightHolder;

		// Token: 0x04001C73 RID: 7283
		private DynamicLight mPointLight;

		// Token: 0x04001C74 RID: 7284
		private int mBlockValue;

		// Token: 0x04001C75 RID: 7285
		private WeaponClass mWeaponClass;

		// Token: 0x04001C76 RID: 7286
		private List<ActiveAura> mAuras = new List<ActiveAura>();

		// Token: 0x04001C77 RID: 7287
		private float mCooldownTime;

		// Token: 0x04001C78 RID: 7288
		private bool mHideModel;

		// Token: 0x04001C79 RID: 7289
		private bool mHideEffect;

		// Token: 0x04001C7A RID: 7290
		private bool mPauseSounds;

		// Token: 0x04001C7B RID: 7291
		private float mCooldownTimer;

		// Token: 0x04001C7C RID: 7292
		private float mWorldCollisionTimer;

		// Token: 0x04001C7D RID: 7293
		private bool mAnimationDetached;

		// Token: 0x04001C7E RID: 7294
		private bool mAttached;

		// Token: 0x04001C7F RID: 7295
		private Ability mSourceAbility;

		// Token: 0x04001C80 RID: 7296
		private Magicka.GameLogic.Entities.Abilities.SpecialAbilities.SpecialAbility mSpecialAbility;

		// Token: 0x04001C81 RID: 7297
		private float mSpecialAbilityRechargeTime;

		// Token: 0x04001C82 RID: 7298
		private float mSpecialAbilityCooldown;

		// Token: 0x04001C83 RID: 7299
		protected Resistance[] mResistances = new Resistance[11];

		// Token: 0x04001C84 RID: 7300
		private static readonly int STEAM_EFFECT = "birch_steam".GetHashCodeCustom();

		// Token: 0x04001C85 RID: 7301
		private static readonly int GUNGNER_EFFECT = "gungner".GetHashCodeCustom();

		// Token: 0x04001C86 RID: 7302
		protected Item.PassiveAbilityStruct mPassiveAbility;

		// Token: 0x04001C87 RID: 7303
		protected VisualEffectReference mPassiveAbilityEffect;

		// Token: 0x04001C88 RID: 7304
		protected float mPassiveAbilityTimer;

		// Token: 0x04001C89 RID: 7305
		protected TeslaField mTeslaField;

		// Token: 0x04001C8A RID: 7306
		protected float mGlow;

		// Token: 0x04001C8B RID: 7307
		protected float mGlowTarget;

		// Token: 0x04001C8C RID: 7308
		protected MissileEntity mGungnirMissile;

		// Token: 0x04001C8D RID: 7309
		protected List<IDamageable> mHitlist;

		// Token: 0x04001C8E RID: 7310
		protected int mNextToDamage;

		// Token: 0x04001C8F RID: 7311
		protected Vector3 mSpellColor;

		// Token: 0x04001C90 RID: 7312
		protected float mSpellTime;

		// Token: 0x04001C91 RID: 7313
		private static readonly int ITEM_PICKUP_LOC = "#item_pick_up".GetHashCodeCustom();

		// Token: 0x04001C92 RID: 7314
		protected string mPickUpString;

		// Token: 0x04001C93 RID: 7315
		protected bool mIgnoreTractorPull;

		// Token: 0x04001C94 RID: 7316
		protected int mAnimatedLevelPartID;

		// Token: 0x04001C95 RID: 7317
		protected bool mBound;

		// Token: 0x0200035A RID: 858
		public struct PointLightHolder
		{
			// Token: 0x04001C96 RID: 7318
			public bool Enabled;

			// Token: 0x04001C97 RID: 7319
			public bool ContainsLight;

			// Token: 0x04001C98 RID: 7320
			public float Radius;

			// Token: 0x04001C99 RID: 7321
			public Vector3 DiffuseColor;

			// Token: 0x04001C9A RID: 7322
			public Vector3 AmbientColor;

			// Token: 0x04001C9B RID: 7323
			public float SpecularAmount;

			// Token: 0x04001C9C RID: 7324
			public LightVariationType VariationType;

			// Token: 0x04001C9D RID: 7325
			public float VariationAmount;

			// Token: 0x04001C9E RID: 7326
			public float VariationSpeed;
		}

		// Token: 0x0200035B RID: 859
		protected class ElementRenderData : IRenderableAdditiveObject, IPreRenderRenderer
		{
			// Token: 0x06001A50 RID: 6736 RVA: 0x000B2A44 File Offset: 0x000B0C44
			public ElementRenderData()
			{
				if (Item.ElementRenderData.sVertices == null || Item.ElementRenderData.sVertices.IsDisposed)
				{
					Item.ElementRenderData.VertexPositionTextureIndex[] array = new Item.ElementRenderData.VertexPositionTextureIndex[8];
					array[0].Position.X = -1f;
					array[0].Position.Y = -1f;
					array[0].TexCoord.X = 0f;
					array[0].TexCoord.Y = 1f;
					array[0].BlendIndices.X = 0f;
					array[0].BlendWeights.X = 1f;
					array[1].Position.X = -1f;
					array[1].Position.Y = 1f;
					array[1].TexCoord.X = 0f;
					array[1].TexCoord.Y = 0f;
					array[1].BlendIndices.X = 0f;
					array[1].BlendWeights.X = 1f;
					array[2].Position.X = 0f;
					array[2].Position.Y = -1f;
					array[2].TexCoord.X = 0.25f;
					array[2].TexCoord.Y = 1f;
					array[2].BlendIndices.X = 0f;
					array[2].BlendWeights.X = 1f;
					array[3].Position.X = 0f;
					array[3].Position.Y = 1f;
					array[3].TexCoord.X = 0.25f;
					array[3].TexCoord.Y = 0f;
					array[3].BlendIndices.X = 0f;
					array[3].BlendWeights.X = 1f;
					array[4].Position.X = 0f;
					array[4].Position.Y = -1f;
					array[4].TexCoord.X = 0.75f;
					array[4].TexCoord.Y = 1f;
					array[4].BlendIndices.X = 1f;
					array[4].BlendWeights.X = 1f;
					array[5].Position.X = 0f;
					array[5].Position.Y = 1f;
					array[5].TexCoord.X = 0.75f;
					array[5].TexCoord.Y = 0f;
					array[5].BlendIndices.X = 1f;
					array[5].BlendWeights.X = 1f;
					array[6].Position.X = 1f;
					array[6].Position.Y = -1f;
					array[6].TexCoord.X = 1f;
					array[6].TexCoord.Y = 1f;
					array[6].BlendIndices.X = 1f;
					array[6].BlendWeights.X = 1f;
					array[7].Position.X = 1f;
					array[7].Position.Y = 1f;
					array[7].TexCoord.X = 1f;
					array[7].TexCoord.Y = 0f;
					array[7].BlendIndices.X = 1f;
					array[7].BlendWeights.X = 1f;
					GraphicsDevice graphicsDevice = Game.Instance.GraphicsDevice;
					lock (graphicsDevice)
					{
						Item.ElementRenderData.sVertices = new VertexBuffer(graphicsDevice, 52 * array.Length, BufferUsage.WriteOnly);
						Item.ElementRenderData.sVertices.SetData<Item.ElementRenderData.VertexPositionTextureIndex>(array);
						Item.ElementRenderData.sVertexDeclaration = new VertexDeclaration(graphicsDevice, Item.ElementRenderData.VertexPositionTextureIndex.VertexElements);
						Item.ElementRenderData.sTexture = Game.Instance.Content.Load<Texture2D>("effectTextures/bladeElements");
					}
					Item.ElementRenderData.sVerticesHash = Item.ElementRenderData.sVertices.GetHashCode();
				}
			}

			// Token: 0x17000693 RID: 1683
			// (get) Token: 0x06001A51 RID: 6737 RVA: 0x000B2F24 File Offset: 0x000B1124
			public int Effect
			{
				get
				{
					return SkinnedModelDeferredEffect.TYPEHASH;
				}
			}

			// Token: 0x17000694 RID: 1684
			// (get) Token: 0x06001A52 RID: 6738 RVA: 0x000B2F2B File Offset: 0x000B112B
			public int Technique
			{
				get
				{
					return 2;
				}
			}

			// Token: 0x17000695 RID: 1685
			// (get) Token: 0x06001A53 RID: 6739 RVA: 0x000B2F2E File Offset: 0x000B112E
			public VertexBuffer Vertices
			{
				get
				{
					return Item.ElementRenderData.sVertices;
				}
			}

			// Token: 0x17000696 RID: 1686
			// (get) Token: 0x06001A54 RID: 6740 RVA: 0x000B2F35 File Offset: 0x000B1135
			public int VerticesHashCode
			{
				get
				{
					return Item.ElementRenderData.sVerticesHash;
				}
			}

			// Token: 0x17000697 RID: 1687
			// (get) Token: 0x06001A55 RID: 6741 RVA: 0x000B2F3C File Offset: 0x000B113C
			public int VertexStride
			{
				get
				{
					return 52;
				}
			}

			// Token: 0x17000698 RID: 1688
			// (get) Token: 0x06001A56 RID: 6742 RVA: 0x000B2F40 File Offset: 0x000B1140
			public IndexBuffer Indices
			{
				get
				{
					return null;
				}
			}

			// Token: 0x17000699 RID: 1689
			// (get) Token: 0x06001A57 RID: 6743 RVA: 0x000B2F43 File Offset: 0x000B1143
			public VertexDeclaration VertexDeclaration
			{
				get
				{
					return Item.ElementRenderData.sVertexDeclaration;
				}
			}

			// Token: 0x06001A58 RID: 6744 RVA: 0x000B2F4C File Offset: 0x000B114C
			public bool Cull(BoundingFrustum iViewFrustum)
			{
				BoundingSphere boundingSphere = this.mBoundingSphere;
				return boundingSphere.Contains(iViewFrustum) == ContainmentType.Disjoint;
			}

			// Token: 0x06001A59 RID: 6745 RVA: 0x000B2F6C File Offset: 0x000B116C
			public void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				SkinnedModelDeferredEffect skinnedModelDeferredEffect = iEffect as SkinnedModelDeferredEffect;
				SkinnedModelDeferredAdvancedMaterial skinnedModelDeferredAdvancedMaterial = default(SkinnedModelDeferredAdvancedMaterial);
				skinnedModelDeferredAdvancedMaterial.DiffuseColor = this.Color;
				skinnedModelDeferredAdvancedMaterial.DiffuseMap0 = Item.ElementRenderData.sTexture;
				skinnedModelDeferredAdvancedMaterial.DiffuseMap0Enabled = true;
				skinnedModelDeferredAdvancedMaterial.Alpha = this.Alpha;
				skinnedModelDeferredAdvancedMaterial.AssignToEffect(skinnedModelDeferredEffect);
				skinnedModelDeferredEffect.Bones = this.mBones;
				skinnedModelDeferredEffect.GraphicsDevice.RenderState.DepthBias = -0.002f;
				skinnedModelDeferredEffect.CommitChanges();
				skinnedModelDeferredEffect.GraphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleStrip, 0, 6);
				skinnedModelDeferredEffect.GraphicsDevice.RenderState.DepthBias = 0f;
			}

			// Token: 0x06001A5A RID: 6746 RVA: 0x000B3008 File Offset: 0x000B1208
			public void PreRenderUpdate(DataChannel iDataChannel, float iDeltaTime, ref Matrix iViewProjectionMatrix, ref Vector3 iCameraPosition, ref Vector3 iCameraDirection)
			{
				Vector3 vector;
				Vector3.Subtract(ref this.Pos1, ref this.Pos0, out vector);
				float value;
				Vector3.Dot(ref vector, ref iCameraDirection, out value);
				float num = vector.LengthSquared();
				if (num > 1E-06f & Math.Abs(value) < 0.999999f)
				{
					Vector3.Divide(ref vector, (float)Math.Sqrt((double)num), out vector);
				}
				else
				{
					vector.X = 1f;
					vector.Y = 0f;
					vector.Z = 0f;
				}
				Vector3 vector2 = default(Vector3);
				vector2.Y = 1f;
				Vector3 vector3;
				Vector3.Cross(ref iCameraDirection, ref vector2, out vector3);
				vector3.Normalize();
				Vector3 vector4;
				Vector3.Cross(ref vector3, ref iCameraDirection, out vector4);
				Vector2 vector5 = default(Vector2);
				Vector3.Dot(ref vector, ref vector3, out vector5.X);
				Vector3.Dot(ref vector, ref vector4, out vector5.Y);
				vector5.Normalize();
				vector.X = vector5.X * vector3.X + vector5.Y * vector4.X;
				vector.Y = vector5.X * vector3.Y + vector5.Y * vector4.Y;
				vector.Z = vector5.X * vector3.Z + vector5.Y * vector4.Z;
				Vector3.Cross(ref vector, ref iCameraDirection, out vector2);
				Vector3 vector6;
				Vector3.Negate(ref iCameraDirection, out vector6);
				this.mBones[0].M11 = vector.X * 0.333f;
				this.mBones[0].M12 = vector.Y * 0.333f;
				this.mBones[0].M13 = vector.Z * 0.333f;
				this.mBones[0].M21 = vector2.X * 0.333f;
				this.mBones[0].M22 = vector2.Y * 0.333f;
				this.mBones[0].M23 = vector2.Z * 0.333f;
				this.mBones[0].M31 = vector6.X * 0.333f;
				this.mBones[0].M32 = vector6.Y * 0.333f;
				this.mBones[0].M33 = vector6.Z * 0.333f;
				this.mBones[0].M41 = this.Pos0.X;
				this.mBones[0].M42 = this.Pos0.Y;
				this.mBones[0].M43 = this.Pos0.Z;
				this.mBones[0].M44 = 1f;
				this.mBones[1].M11 = vector.X * 0.333f;
				this.mBones[1].M12 = vector.Y * 0.333f;
				this.mBones[1].M13 = vector.Z * 0.333f;
				this.mBones[1].M21 = vector2.X * 0.333f;
				this.mBones[1].M22 = vector2.Y * 0.333f;
				this.mBones[1].M23 = vector2.Z * 0.333f;
				this.mBones[1].M31 = vector6.X * 0.333f;
				this.mBones[1].M32 = vector6.Y * 0.333f;
				this.mBones[1].M33 = vector6.Z * 0.333f;
				this.mBones[1].M41 = this.Pos1.X;
				this.mBones[1].M42 = this.Pos1.Y;
				this.mBones[1].M43 = this.Pos1.Z;
				this.mBones[1].M44 = 1f;
			}

			// Token: 0x04001C9F RID: 7327
			private static int sVerticesHash;

			// Token: 0x04001CA0 RID: 7328
			private static VertexBuffer sVertices;

			// Token: 0x04001CA1 RID: 7329
			private static VertexDeclaration sVertexDeclaration;

			// Token: 0x04001CA2 RID: 7330
			private static Texture2D sTexture;

			// Token: 0x04001CA3 RID: 7331
			public Vector3 Pos0;

			// Token: 0x04001CA4 RID: 7332
			public Vector3 Pos1;

			// Token: 0x04001CA5 RID: 7333
			private Matrix[] mBones = new Matrix[2];

			// Token: 0x04001CA6 RID: 7334
			public float Alpha;

			// Token: 0x04001CA7 RID: 7335
			public Vector3 Color;

			// Token: 0x04001CA8 RID: 7336
			public BoundingSphere mBoundingSphere;

			// Token: 0x0200035C RID: 860
			private struct VertexPositionTextureIndex
			{
				// Token: 0x04001CA9 RID: 7337
				public const int SIZEINBYTES = 52;

				// Token: 0x04001CAA RID: 7338
				public Vector3 Position;

				// Token: 0x04001CAB RID: 7339
				public Vector2 TexCoord;

				// Token: 0x04001CAC RID: 7340
				public Vector4 BlendIndices;

				// Token: 0x04001CAD RID: 7341
				public Vector4 BlendWeights;

				// Token: 0x04001CAE RID: 7342
				public static readonly VertexElement[] VertexElements = new VertexElement[]
				{
					new VertexElement(0, 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, 0),
					new VertexElement(0, 12, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0),
					new VertexElement(0, 20, VertexElementFormat.Vector4, VertexElementMethod.Default, VertexElementUsage.BlendIndices, 0),
					new VertexElement(0, 36, VertexElementFormat.Vector4, VertexElementMethod.Default, VertexElementUsage.BlendWeight, 0)
				};
			}
		}

		// Token: 0x0200035D RID: 861
		protected new class RenderData : Pickable.RenderData
		{
			// Token: 0x06001A5C RID: 6748 RVA: 0x000B34DC File Offset: 0x000B16DC
			public override void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				RenderDeferredEffect renderDeferredEffect = iEffect as RenderDeferredEffect;
				this.mMaterial.AssignToEffect(renderDeferredEffect);
				renderDeferredEffect.EmissiveAmount0 *= this.EmissiveMultiplyer;
				renderDeferredEffect.EmissiveAmount1 *= this.EmissiveMultiplyer;
				renderDeferredEffect.World = this.mTransform;
				renderDeferredEffect.GraphicsDevice.RenderState.DepthBias = -5E-06f;
				renderDeferredEffect.CommitChanges();
				renderDeferredEffect.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
				renderDeferredEffect.GraphicsDevice.RenderState.DepthBufferFunction = CompareFunction.LessEqual;
				renderDeferredEffect.GraphicsDevice.RenderState.DepthBias = 0f;
			}

			// Token: 0x04001CAF RID: 7343
			public float EmissiveMultiplyer = 1f;
		}

		// Token: 0x0200035E RID: 862
		public enum PassiveAbilities : byte
		{
			// Token: 0x04001CB1 RID: 7345
			None,
			// Token: 0x04001CB2 RID: 7346
			ShieldBoost,
			// Token: 0x04001CB3 RID: 7347
			AreaLifeDrain,
			// Token: 0x04001CB4 RID: 7348
			ZombieDeterrent,
			// Token: 0x04001CB5 RID: 7349
			ReduceAggro,
			// Token: 0x04001CB6 RID: 7350
			EnhanceAllyMelee,
			// Token: 0x04001CB7 RID: 7351
			AreaRegeneration,
			// Token: 0x04001CB8 RID: 7352
			InverseArcaneLife,
			// Token: 0x04001CB9 RID: 7353
			Zap,
			// Token: 0x04001CBA RID: 7354
			BirchSteam,
			// Token: 0x04001CBB RID: 7355
			WetLightning,
			// Token: 0x04001CBC RID: 7356
			MoveSpeed,
			// Token: 0x04001CBD RID: 7357
			Glow,
			// Token: 0x04001CBE RID: 7358
			Mjolnr,
			// Token: 0x04001CBF RID: 7359
			Gungner,
			// Token: 0x04001CC0 RID: 7360
			MasterSword,
			// Token: 0x04001CC1 RID: 7361
			DragonSlayer
		}

		// Token: 0x0200035F RID: 863
		public struct PassiveAbilityStruct
		{
			// Token: 0x06001A5E RID: 6750 RVA: 0x000B35A7 File Offset: 0x000B17A7
			public PassiveAbilityStruct(Item.PassiveAbilities iAbility, float iVar)
			{
				this.Ability = iAbility;
				this.Variable = iVar;
			}

			// Token: 0x04001CC2 RID: 7362
			public Item.PassiveAbilities Ability;

			// Token: 0x04001CC3 RID: 7363
			public float Variable;
		}
	}
}
