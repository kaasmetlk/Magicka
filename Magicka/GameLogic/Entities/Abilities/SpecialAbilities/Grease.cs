using System;
using System.Collections.Generic;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Physics;
using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Graphics;
using Magicka.Levels;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using PolygonHead;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x02000256 RID: 598
	public class Grease : SpecialAbility, IAbilityEffect
	{
		// Token: 0x06001265 RID: 4709 RVA: 0x0007138C File Offset: 0x0006F58C
		public static Grease GetInstance()
		{
			if (Grease.sCache.Count > 0)
			{
				Grease result = Grease.sCache[Grease.sCache.Count - 1];
				Grease.sCache.RemoveAt(Grease.sCache.Count - 1);
				return result;
			}
			return new Grease();
		}

		// Token: 0x06001266 RID: 4710 RVA: 0x000713DC File Offset: 0x0006F5DC
		public static void InitializeCache(int iNr, PlayState iPlayState)
		{
			Grease.sCache = new List<Grease>(iNr);
			for (int i = 0; i < iNr; i++)
			{
				Grease.sCache.Add(new Grease());
			}
		}

		// Token: 0x06001267 RID: 4711 RVA: 0x0007140F File Offset: 0x0006F60F
		public Grease(Animations iAnimation) : base(iAnimation, "#magick_grease".GetHashCodeCustom())
		{
		}

		// Token: 0x06001268 RID: 4712 RVA: 0x00071422 File Offset: 0x0006F622
		private Grease() : base(Animations.cast_magick_sweep, "#magick_grease".GetHashCodeCustom())
		{
		}

		// Token: 0x06001269 RID: 4713 RVA: 0x00071436 File Offset: 0x0006F636
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			throw new Exception("Grease can not be cast without an owner!");
		}

		// Token: 0x0600126A RID: 4714 RVA: 0x00071444 File Offset: 0x0006F644
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			base.Execute(iOwner, iPlayState);
			this.mOwner = (iOwner as Character);
			if (this.mOwner == null)
			{
				this.OnRemove();
				return false;
			}
			this.mPlayState = iPlayState;
			this.mTTL = 0.5f;
			this.mCue = AudioManager.Instance.PlayCue(Banks.Spells, Grease.SOUNDHASH, this.mOwner.AudioEmitter);
			Vector3 translation = iOwner.CastSource.Translation;
			Vector3 direction = iOwner.Direction;
			EffectManager.Instance.StartEffect(Grease.EFFECT, ref translation, ref direction, out this.mEffect);
			SpellManager.Instance.AddSpellEffect(this);
			return true;
		}

		// Token: 0x170004BA RID: 1210
		// (get) Token: 0x0600126B RID: 4715 RVA: 0x000714E6 File Offset: 0x0006F6E6
		public bool IsDead
		{
			get
			{
				return this.mTTL <= 0f;
			}
		}

		// Token: 0x0600126C RID: 4716 RVA: 0x000714F8 File Offset: 0x0006F6F8
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			this.mOwner.TurnSpeed = 0f;
			float num = this.mTTL;
			this.mTTL -= iDeltaTime;
			Vector3 vector = this.mOwner.Position;
			Vector3 direction = this.mOwner.Direction;
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				float yaw = -(this.mTTL / 0.5f * 6f - 2.5f) / 2.5f * 0.7853982f;
				Quaternion quaternion;
				Quaternion.CreateFromYawPitchRoll(yaw, 0f, 0f, out quaternion);
				float num2 = 6f;
				Segment segment;
				segment.Origin = vector;
				Vector3.Transform(ref direction, ref quaternion, out direction);
				Vector3.Multiply(ref direction, num2, out segment.Delta);
				GameScene currentScene = this.mOwner.PlayState.Level.CurrentScene;
				List<Shield> shields = this.mOwner.PlayState.EntityManager.Shields;
				float num3;
				Vector3 position;
				Vector3 direction2;
				for (int i = 0; i < shields.Count; i++)
				{
					if (shields[i].Body.CollisionSkin.SegmentIntersect(out num3, out position, out direction2, segment))
					{
						num2 *= num3;
						Vector3.Multiply(ref segment.Delta, num3, out segment.Delta);
					}
				}
				if (currentScene.SegmentIntersect(out num3, out position, out direction2, segment))
				{
					num2 *= num3;
					Vector3.Multiply(ref segment.Delta, num3, out segment.Delta);
				}
				List<Entity> entities = this.mPlayState.EntityManager.GetEntities(vector, num2, true);
				entities.Remove(this.mOwner);
				for (int j = 0; j < entities.Count; j++)
				{
					Character character = entities[j] as Character;
					Vector3 vector2;
					if (character != null && !character.HasStatus(StatusEffects.Greased) && character.ArcIntersect(out vector2, segment.Origin, direction, num2, 0.17453292f, 5f))
					{
						character.AddStatusEffect(new StatusEffect(StatusEffects.Greased, 0f, 1f, 1f, 1f));
					}
				}
				vector = this.mOwner.CastSource.Translation;
				EffectManager.Instance.UpdatePositionDirection(ref this.mEffect, ref vector, ref direction);
				this.mPlayState.EntityManager.ReturnEntityList(entities);
				num2 -= 1.5f;
				float num4 = (float)Math.Floor((double)(num / 0.5f * 6f));
				float num5 = (float)Math.Floor((double)(this.mTTL / 0.5f * 6f));
				if (num5 < num4 & num5 >= 0f & num2 > 0f)
				{
					vector = this.mOwner.Position;
					direction = this.mOwner.Direction;
					yaw = -(num5 - 2.5f) / 2.5f * 0.7853982f;
					Quaternion.CreateFromYawPitchRoll(yaw, 0f, 0f, out quaternion);
					Vector3.Transform(ref direction, ref quaternion, out direction);
					Vector3.Multiply(ref direction, num2, out segment.Delta);
					Vector3.Add(ref segment.Origin, ref segment.Delta, out segment.Origin);
					segment.Origin.Y = segment.Origin.Y + 2f;
					segment.Delta.X = 0f;
					segment.Delta.Y = -4f;
					segment.Delta.Z = 0f;
					AnimatedLevelPart animatedLevelPart;
					if (currentScene.SegmentIntersect(out num3, out position, out direction2, out animatedLevelPart, segment))
					{
						Grease.GreaseField instance = Grease.GreaseField.GetInstance(this.mPlayState);
						instance.Initialize(this.mOwner, animatedLevelPart, ref position, ref direction2);
						this.mPlayState.EntityManager.AddEntity(instance);
						if (NetworkManager.Instance.State == NetworkState.Server)
						{
							TriggerActionMessage triggerActionMessage = default(TriggerActionMessage);
							triggerActionMessage.Handle = instance.Handle;
							triggerActionMessage.Position = position;
							triggerActionMessage.Direction = direction2;
							if (animatedLevelPart != null)
							{
								triggerActionMessage.Arg = (int)animatedLevelPart.Handle;
							}
							else
							{
								triggerActionMessage.Arg = 65535;
							}
							triggerActionMessage.Id = (int)this.mOwner.Handle;
							triggerActionMessage.ActionType = TriggerActionType.SpawnGrease;
							NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref triggerActionMessage);
							return;
						}
					}
				}
			}
			else
			{
				vector = this.mOwner.CastSource.Translation;
				EffectManager.Instance.UpdatePositionDirection(ref this.mEffect, ref vector, ref direction);
			}
		}

		// Token: 0x0600126D RID: 4717 RVA: 0x0007194F File Offset: 0x0006FB4F
		public void OnRemove()
		{
			EffectManager.Instance.Stop(ref this.mEffect);
			if (this.mCue.IsPlaying)
			{
				this.mCue.Stop(AudioStopOptions.AsAuthored);
			}
			Grease.sCache.Add(this);
		}

		// Token: 0x04001129 RID: 4393
		public const float GREASESPRAY_TTL = 0.5f;

		// Token: 0x0400112A RID: 4394
		public const int NR_OF_FIELDS = 6;

		// Token: 0x0400112B RID: 4395
		public const float NR_OF_FIELDS_F = 6f;

		// Token: 0x0400112C RID: 4396
		private static List<Grease> sCache;

		// Token: 0x0400112D RID: 4397
		public static readonly int EFFECT = "magick_grease_spray".GetHashCodeCustom();

		// Token: 0x0400112E RID: 4398
		public static readonly int SOUNDHASH = "magick_grease".GetHashCodeCustom();

		// Token: 0x0400112F RID: 4399
		private Character mOwner;

		// Token: 0x04001130 RID: 4400
		private PlayState mPlayState;

		// Token: 0x04001131 RID: 4401
		private VisualEffectReference mEffect;

		// Token: 0x04001132 RID: 4402
		private Cue mCue;

		// Token: 0x04001133 RID: 4403
		private float mTTL;

		// Token: 0x02000257 RID: 599
		public class GreaseField : Entity, IDamageable
		{
			// Token: 0x0600126F RID: 4719 RVA: 0x000719A8 File Offset: 0x0006FBA8
			public static Grease.GreaseField GetInstance(PlayState iPlayState)
			{
				Grease.GreaseField greaseField;
				lock (Grease.GreaseField.sCache)
				{
					greaseField = Grease.GreaseField.sCache[0];
					Grease.GreaseField.sCache.RemoveAt(0);
					Grease.GreaseField.sCache.Add(greaseField);
				}
				return greaseField;
			}

			// Token: 0x06001270 RID: 4720 RVA: 0x00071A00 File Offset: 0x0006FC00
			public static Grease.GreaseField GetSpecificInstance(ushort iHandle)
			{
				Grease.GreaseField greaseField;
				lock (Grease.GreaseField.sCache)
				{
					greaseField = (Entity.GetFromHandle((int)iHandle) as Grease.GreaseField);
					Grease.GreaseField.sCache.Remove(greaseField);
					Grease.GreaseField.sCache.Add(greaseField);
				}
				return greaseField;
			}

			// Token: 0x06001271 RID: 4721 RVA: 0x00071A58 File Offset: 0x0006FC58
			public static void InitializeCache(int iNr, PlayState iPlayState)
			{
				Grease.GreaseField.sCache = new List<Grease.GreaseField>(iNr);
				for (int i = 0; i < iNr; i++)
				{
					Grease.GreaseField.sCache.Add(new Grease.GreaseField(iPlayState));
				}
			}

			// Token: 0x06001272 RID: 4722 RVA: 0x00071A8C File Offset: 0x0006FC8C
			private GreaseField(PlayState iPlayState) : base(iPlayState)
			{
				this.mPlayState = iPlayState;
				this.mBody = new Body();
				this.mCollision = new CollisionSkin(this.mBody);
				this.mCollision.AddPrimitive(new Sphere(default(Vector3), 1.5f), 1, default(MaterialProperties));
				this.mCollision.callbackFn += this.OnCollision;
				this.mBody.CollisionSkin = this.mCollision;
				this.mBody.Immovable = true;
				this.mBody.Tag = this;
				this.mAudioEmitter.Forward = Vector3.Forward;
				this.mAudioEmitter.Up = Vector3.Up;
				this.mBurnDamage = default(Damage);
				this.mBurnDamage.AttackProperty = AttackProperties.Status;
				this.mBurnDamage.Element = Elements.Fire;
				this.mBurnDamage.Amount = Defines.SPELL_DAMAGE_FIRE * 4f;
				this.mBurnDamage.Magnitude = 1f;
				this.mRadius = 1.5f;
			}

			// Token: 0x06001273 RID: 4723 RVA: 0x00071BA4 File Offset: 0x0006FDA4
			public void Initialize(ISpellCaster iOwner, AnimatedLevelPart iAnimation, ref Vector3 iPosition, ref Vector3 iNormal)
			{
				EffectManager.Instance.Stop(ref this.mBurnEffect);
				EffectManager.Instance.Stop(ref this.mParticleEffect);
				float num;
				DecalManager.Instance.GetDecalTTL(ref this.mDecalReference, out num);
				if (num > 1f)
				{
					DecalManager.Instance.SetDecalTTL(ref this.mDecalReference, 1f);
				}
				this.mOwner = iOwner;
				this.mPlayState = iOwner.PlayState;
				this.mTTL = 60f;
				this.mTemperature = 0f;
				this.mBurning = false;
				this.mTimeStamp = iOwner.PlayState.PlayTime;
				this.mDead = false;
				this.mBody.MoveTo(iPosition, Matrix.Identity);
				this.mBody.CollisionSkin.NonCollidables.Add(this.mPlayState.Level.CurrentScene.CollisionSkin);
				if (iAnimation != null)
				{
					iAnimation.AddEntity(this);
				}
				Vector2 vector = default(Vector2);
				vector.X = (vector.Y = 3f);
				DecalManager.Instance.AddAlphaBlendedDecal(Decal.Grease, iAnimation, ref vector, ref iPosition, null, ref iNormal, this.mTTL, 1f, out this.mDecalReference);
				base.Initialize();
				this.mAudioEmitter.Position = iPosition;
				Vector3 forward = Vector3.Forward;
				EffectManager.Instance.StartEffect(Grease.GreaseField.PARTICLE_EFFECT, ref iPosition, ref forward, out this.mParticleEffect);
			}

			// Token: 0x170004BB RID: 1211
			// (get) Token: 0x06001274 RID: 4724 RVA: 0x00071D15 File Offset: 0x0006FF15
			public bool Resting
			{
				get
				{
					return this.mRestingTimer < 0f;
				}
			}

			// Token: 0x06001275 RID: 4725 RVA: 0x00071D24 File Offset: 0x0006FF24
			public override void Deinitialize()
			{
				if (Grease.GreaseField.sHitListOwner == this)
				{
					Grease.GreaseField.sHitListOwner = null;
				}
				EffectManager.Instance.Stop(ref this.mBurnEffect);
				EffectManager.Instance.Stop(ref this.mParticleEffect);
				float num;
				DecalManager.Instance.GetDecalTTL(ref this.mDecalReference, out num);
				if (num > 1f)
				{
					DecalManager.Instance.SetDecalTTL(ref this.mDecalReference, 1f);
				}
				this.mOwner = null;
				base.Deinitialize();
			}

			// Token: 0x06001276 RID: 4726 RVA: 0x00071DA0 File Offset: 0x0006FFA0
			public override void Update(DataChannel iDataChannel, float iDeltaTime)
			{
				if (this.mBurning)
				{
					this.mTemperature = Math.Min(1f, this.mTemperature + iDeltaTime * 0.5f);
					this.mTTL -= iDeltaTime * 3f;
					List<Entity> entities = base.PlayState.EntityManager.GetEntities(this.Position, 1.5f, false);
					entities.Remove(this);
					for (int i = 0; i < entities.Count; i++)
					{
						IDamageable damageable = entities[i] as IDamageable;
						if (damageable != null && !Grease.GreaseField.sHitlist.ContainsKey(damageable.Handle))
						{
							Grease.GreaseField.sHitlist.Add(damageable.Handle);
							damageable.Damage(this.mBurnDamage, this.mOwner as Entity, this.mTimeStamp, this.Position);
						}
					}
					base.PlayState.EntityManager.ReturnEntityList(entities);
				}
				else
				{
					this.mTTL -= iDeltaTime;
				}
				DecalManager.Instance.SetDecalTTL(ref this.mDecalReference, this.mTTL);
				this.mDead = (this.mTTL < 0f);
				if (Grease.GreaseField.sHitListOwner == null)
				{
					Grease.GreaseField.sHitListOwner = this;
				}
				if (Grease.GreaseField.sHitListOwner == this)
				{
					Grease.GreaseField.sHitlist.Update(iDeltaTime);
				}
				base.Update(iDataChannel, iDeltaTime);
				if (this.mBody.Velocity.LengthSquared() > 1E-06f)
				{
					this.mRestingTimer = 1f;
					return;
				}
				this.mRestingTimer -= iDeltaTime;
			}

			// Token: 0x06001277 RID: 4727 RVA: 0x00071F20 File Offset: 0x00070120
			public bool OnCollision(CollisionSkin iSkin0, int iPrim0, CollisionSkin iSkin1, int iPrim1)
			{
				if (iSkin1.Owner != null)
				{
					Character character = iSkin1.Owner.Tag as Character;
					if (character != null && !character.IsNonslippery)
					{
						character.CharacterBody.IsOnGrease = true;
					}
				}
				return false;
			}

			// Token: 0x06001278 RID: 4728 RVA: 0x00071F5E File Offset: 0x0007015E
			public float ResistanceAgainst(Elements iElement)
			{
				return 0f;
			}

			// Token: 0x170004BC RID: 1212
			// (get) Token: 0x06001279 RID: 4729 RVA: 0x00071F65 File Offset: 0x00070165
			public override bool Dead
			{
				get
				{
					return this.mDead;
				}
			}

			// Token: 0x170004BD RID: 1213
			// (get) Token: 0x0600127A RID: 4730 RVA: 0x00071F6D File Offset: 0x0007016D
			public override bool Removable
			{
				get
				{
					return this.mDead;
				}
			}

			// Token: 0x0600127B RID: 4731 RVA: 0x00071F75 File Offset: 0x00070175
			public override void Kill()
			{
				this.mTTL = 0f;
				this.mDead = true;
			}

			// Token: 0x170004BE RID: 1214
			// (get) Token: 0x0600127C RID: 4732 RVA: 0x00071F89 File Offset: 0x00070189
			public float HitPoints
			{
				get
				{
					return this.mTTL;
				}
			}

			// Token: 0x170004BF RID: 1215
			// (get) Token: 0x0600127D RID: 4733 RVA: 0x00071F91 File Offset: 0x00070191
			public float MaxHitPoints
			{
				get
				{
					return 60f;
				}
			}

			// Token: 0x0600127E RID: 4734 RVA: 0x00071F98 File Offset: 0x00070198
			public bool SegmentIntersect(out Vector3 oPosition, Segment iSeg, float iSegmentRadius)
			{
				float num;
				Vector3 vector;
				return this.mCollision.SegmentIntersect(out num, out oPosition, out vector, iSeg);
			}

			// Token: 0x0600127F RID: 4735 RVA: 0x00071FB8 File Offset: 0x000701B8
			public DamageResult InternalDamage(DamageCollection5 iDamages, Entity iAttacker, double iTimeStamp, Vector3 iAttackPosition, Defines.DamageFeatures iFeatures)
			{
				DamageResult damageResult = DamageResult.None;
				damageResult |= this.InternalDamage(iDamages.A, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
				damageResult |= this.InternalDamage(iDamages.B, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
				damageResult |= this.InternalDamage(iDamages.C, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
				damageResult |= this.InternalDamage(iDamages.D, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
				return damageResult | this.InternalDamage(iDamages.E, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
			}

			// Token: 0x06001280 RID: 4736 RVA: 0x00072038 File Offset: 0x00070238
			public DamageResult InternalDamage(Damage iDamage, Entity iAttacker, double iTimeStamp, Vector3 iAttackPosition, Defines.DamageFeatures iFeatures)
			{
				if ((iDamage.Element & Elements.Fire) == Elements.Fire)
				{
					this.mTemperature = Math.Min(1f, this.mTemperature + iDamage.Magnitude);
					if (!this.mBurning & this.mTemperature >= 1f)
					{
						this.mBurning = true;
						Vector3 position = this.Position;
						Vector3 vector = default(Vector3);
						vector.Z = -1f;
						EffectManager.Instance.StartEffect(Grease.GreaseField.BURNING_EFFECT, ref position, ref vector, out this.mBurnEffect);
					}
				}
				else if (SpellManager.InclusiveOpposites(iDamage.Element, Elements.Fire))
				{
					this.mTemperature = Math.Max(0f, this.mTemperature - iDamage.Magnitude);
					if (this.mBurning & this.mTemperature <= 0f)
					{
						this.mBurning = false;
						EffectManager.Instance.Stop(ref this.mBurnEffect);
					}
				}
				return DamageResult.None;
			}

			// Token: 0x06001281 RID: 4737 RVA: 0x0007212C File Offset: 0x0007032C
			internal void Burn(float iTemperature)
			{
				this.mTemperature = Math.Min(1f, this.mTemperature + iTemperature);
				if (!this.mBurning & this.mTemperature >= 1f)
				{
					this.mBurning = true;
					this.mTemperature = iTemperature;
					Vector3 position = this.Position;
					Vector3 vector = default(Vector3);
					vector.Z = -1f;
					EffectManager.Instance.StartEffect(Grease.GreaseField.BURNING_EFFECT, ref position, ref vector, out this.mBurnEffect);
				}
			}

			// Token: 0x06001282 RID: 4738 RVA: 0x000721B0 File Offset: 0x000703B0
			public void OverKill()
			{
				this.mTTL = 0f;
				this.mDead = true;
			}

			// Token: 0x06001283 RID: 4739 RVA: 0x000721C4 File Offset: 0x000703C4
			protected override void INetworkUpdate(ref EntityUpdateMessage iMsg)
			{
				base.INetworkUpdate(ref iMsg);
				bool genericBool = iMsg.GenericBool;
				if (!this.mBurning && genericBool)
				{
					Vector3 position = this.Position;
					Vector3 vector = default(Vector3);
					vector.Z = -1f;
					EffectManager.Instance.StartEffect(Grease.GreaseField.BURNING_EFFECT, ref position, ref vector, out this.mBurnEffect);
				}
				if ((ushort)(iMsg.Features & EntityFeatures.GenericFloat) == 512)
				{
					this.mTemperature = iMsg.GenericFloat;
				}
				else
				{
					this.mTemperature = 0f;
					EffectManager.Instance.Stop(ref this.mBurnEffect);
				}
				this.mBurning = genericBool;
			}

			// Token: 0x06001284 RID: 4740 RVA: 0x00072264 File Offset: 0x00070464
			protected override void IGetNetworkUpdate(out EntityUpdateMessage oMsg, float iPrediction)
			{
				oMsg = default(EntityUpdateMessage);
				if (!this.Resting)
				{
					oMsg.Features |= EntityFeatures.Position;
					oMsg.Position = this.Position;
				}
				oMsg.Features |= EntityFeatures.GenericBool;
				oMsg.GenericBool = this.mBurning;
				if (this.mBurning && this.mTemperature > 0f)
				{
					oMsg.Features |= EntityFeatures.GenericFloat;
					oMsg.GenericFloat = this.mTemperature;
				}
			}

			// Token: 0x06001285 RID: 4741 RVA: 0x000722EE File Offset: 0x000704EE
			public void Electrocute(IDamageable iTarget, float iMultiplyer)
			{
			}

			// Token: 0x06001286 RID: 4742 RVA: 0x000722F0 File Offset: 0x000704F0
			internal override float GetDanger()
			{
				if (!this.mBurning)
				{
					return 0.5f;
				}
				return this.mTemperature * 10f;
			}

			// Token: 0x04001134 RID: 4404
			public const float GREASEFIELD_TTL = 60f;

			// Token: 0x04001135 RID: 4405
			public const float BURN_RATE = 3f;

			// Token: 0x04001136 RID: 4406
			public const float GREASE_RADIUS = 1.5f;

			// Token: 0x04001137 RID: 4407
			private static List<Grease.GreaseField> sCache;

			// Token: 0x04001138 RID: 4408
			public static readonly int BURNING_EFFECT = "magick_grease_burning".GetHashCodeCustom();

			// Token: 0x04001139 RID: 4409
			public static readonly int PARTICLE_EFFECT = "magick_grease_particle".GetHashCodeCustom();

			// Token: 0x0400113A RID: 4410
			private static Grease.GreaseField sHitListOwner;

			// Token: 0x0400113B RID: 4411
			private static HitList sHitlist = new HitList(128);

			// Token: 0x0400113C RID: 4412
			private float mTTL;

			// Token: 0x0400113D RID: 4413
			private float mTemperature;

			// Token: 0x0400113E RID: 4414
			private bool mBurning;

			// Token: 0x0400113F RID: 4415
			private Damage mBurnDamage;

			// Token: 0x04001140 RID: 4416
			private float mRestingTimer;

			// Token: 0x04001141 RID: 4417
			private ISpellCaster mOwner;

			// Token: 0x04001142 RID: 4418
			private VisualEffectReference mBurnEffect;

			// Token: 0x04001143 RID: 4419
			private VisualEffectReference mParticleEffect;

			// Token: 0x04001144 RID: 4420
			private new PlayState mPlayState;

			// Token: 0x04001145 RID: 4421
			private DecalManager.DecalReference mDecalReference;

			// Token: 0x04001146 RID: 4422
			private double mTimeStamp;
		}
	}
}
