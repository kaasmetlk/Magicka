using System;
using System.Collections.Generic;
using JigLibX.Geometry;
using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.Statistics;
using Magicka.Graphics.Lights;
using Magicka.Network;
using Microsoft.Xna.Framework;
using PolygonHead;
using PolygonHead.Lights;

namespace Magicka.GameLogic.Entities
{
	// Token: 0x02000160 RID: 352
	internal class DamageablePhysicsEntity : PhysicsEntity, IStatusEffected, IDamageable
	{
		// Token: 0x06000A82 RID: 2690 RVA: 0x0003F72C File Offset: 0x0003D92C
		public DamageablePhysicsEntity(PlayState iPlayState) : base(iPlayState)
		{
		}

		// Token: 0x06000A83 RID: 2691 RVA: 0x0003F75C File Offset: 0x0003D95C
		public override void Initialize(PhysicsEntityTemplate iTemplate, Matrix iStartTransform, int iUniqueID)
		{
			base.Initialize(iTemplate, iStartTransform, iUniqueID);
			this.mMaxHitPoints = (this.mOldHitPoints = (this.mHitPoints = (float)iTemplate.MaxHitpoints));
			this.mOldPercentage = 1f;
			this.mGibs.Clear();
			for (int i = 0; i < iTemplate.Gibs.Length; i++)
			{
				this.mGibs.Add(iTemplate.Gibs[i]);
			}
			for (int j = 0; j < this.mStatusEffects.Length; j++)
			{
				this.mStatusEffects[j] = default(StatusEffect);
			}
			this.mResistances = iTemplate.Resistances;
			this.mCanHasStatus = iTemplate.CanHaveStatus;
			Vector3 sides = iTemplate.Box.Sides;
			this.mVolume = sides.X * sides.Y * sides.Z;
		}

		// Token: 0x06000A84 RID: 2692 RVA: 0x0003F840 File Offset: 0x0003DA40
		public float ResistanceAgainst(Elements iElement)
		{
			float num = 0f;
			float num2 = 0f;
			for (int i = 0; i < this.mResistances.Length; i++)
			{
				Elements elements = Defines.ElementFromIndex(i);
				if ((iElement & elements) != Elements.None)
				{
					float num3 = this.mResistances[i].Multiplier;
					float num4 = this.mResistances[i].Modifier;
					if (this.HasStatus(StatusEffects.Frozen) && (iElement & Elements.Earth) != Elements.None)
					{
						num4 -= 350f;
					}
					if (this.HasStatus(StatusEffects.Greased) && (iElement & Elements.Fire) != Elements.None)
					{
						num3 *= 2f;
					}
					num += num4;
					num2 += num3;
				}
			}
			float num5 = MathHelper.Clamp(num / 300f + num2, -1f, 1f);
			return 1f - num5;
		}

		// Token: 0x06000A85 RID: 2693 RVA: 0x0003F8FC File Offset: 0x0003DAFC
		public override void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			if (this.mAnimations.Count > 0 && Math.Abs(this.mOldHitPoints - this.mHitPoints) > 1E-45f)
			{
				float num = 1f - this.mHitPoints / this.mMaxHitPoints;
				foreach (PhysAnimationControl physAnimationControl in this.mAnimations)
				{
					float num2 = physAnimationControl.Start + (physAnimationControl.End - physAnimationControl.Start) * this.mOldPercentage;
					float num3 = physAnimationControl.Start + (physAnimationControl.End - physAnimationControl.Start) * num;
					if (num2 > num3)
					{
						physAnimationControl.AnimatedLevelPart.Play(physAnimationControl.Children, num3, num2, -physAnimationControl.Speed, false, false);
					}
					else
					{
						physAnimationControl.AnimatedLevelPart.Play(physAnimationControl.Children, num2, num3, physAnimationControl.Speed, false, false);
					}
				}
				this.mOldHitPoints = this.mHitPoints;
				this.mOldPercentage = num;
			}
			this.UpdateStatusEffects(iDeltaTime);
			if (this.mLastHitpoints != this.mHitPoints || this.mCurrentStatusEffects != StatusEffects.None)
			{
				this.mRestingHealthTimer = 1f;
			}
			else
			{
				this.mRestingHealthTimer -= iDeltaTime;
			}
			base.Update(iDataChannel, iDeltaTime);
		}

		// Token: 0x06000A86 RID: 2694 RVA: 0x0003FA60 File Offset: 0x0003DC60
		private void UpdateStatusEffects(float iDeltaTime)
		{
			StatusEffects statusEffects = StatusEffects.None;
			if (this.Dead)
			{
				int i = 0;
				while (i < this.mStatusEffects.Length)
				{
					switch (this.mStatusEffects[i].DamageType)
					{
					default:
						this.mStatusEffects[i].Stop();
						this.mStatusEffects[i] = default(StatusEffect);
						i++;
						break;
					}
				}
			}
			else
			{
				for (int j = 0; j < this.mStatusEffects.Length; j++)
				{
					this.mStatusEffects[j].Update(iDeltaTime, this);
					if (this.mStatusEffects[j].Dead)
					{
						switch (this.mStatusEffects[j].DamageType)
						{
						default:
							this.mStatusEffects[j].Stop();
							this.mStatusEffects[j] = default(StatusEffect);
							break;
						}
					}
					else
					{
						statusEffects |= this.mStatusEffects[j].DamageType;
					}
				}
			}
			if (this.mStatusEffectLight != null)
			{
				this.mStatusEffectLight.Position = this.Position;
			}
			if (this.HasStatus(StatusEffects.Burning))
			{
				if (this.mStatusEffectLight == null)
				{
					this.mStatusEffectLight = DynamicLight.GetCachedLight();
					this.mStatusEffectLight.Initialize(Vector3.Zero, new Vector3(1f, 0.4f, 0f), 2.5f, 5f, 1f, 0.5f);
					this.mStatusEffectLight.VariationType = LightVariationType.Candle;
					this.mStatusEffectLight.VariationSpeed = 4f;
					this.mStatusEffectLight.VariationAmount = 0.2f;
					this.mStatusEffectLight.Enable();
				}
				this.mStatusEffectLight.Intensity = 3f + (float)Math.Sqrt((double)this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Burning)].Magnitude);
			}
			else if (this.mStatusEffectLight != null)
			{
				this.mStatusEffectLight.Stop(false);
				this.mStatusEffectLight = null;
			}
			this.mCurrentStatusEffects = statusEffects;
		}

		// Token: 0x06000A87 RID: 2695 RVA: 0x0003FC83 File Offset: 0x0003DE83
		public override void Deinitialize()
		{
			base.Deinitialize();
			if (this.mStatusEffectLight != null)
			{
				this.mStatusEffectLight.Disable();
				this.mStatusEffectLight = null;
			}
		}

		// Token: 0x17000246 RID: 582
		// (get) Token: 0x06000A88 RID: 2696 RVA: 0x0003FCA5 File Offset: 0x0003DEA5
		// (set) Token: 0x06000A89 RID: 2697 RVA: 0x0003FCAD File Offset: 0x0003DEAD
		public int OnDeath
		{
			get
			{
				return this.mOnDeath;
			}
			set
			{
				this.mOnDeath = value;
			}
		}

		// Token: 0x17000247 RID: 583
		// (get) Token: 0x06000A8A RID: 2698 RVA: 0x0003FCB6 File Offset: 0x0003DEB6
		// (set) Token: 0x06000A8B RID: 2699 RVA: 0x0003FCBE File Offset: 0x0003DEBE
		public int OnDamage
		{
			get
			{
				return this.mOnDamage;
			}
			set
			{
				this.mOnDamage = value;
			}
		}

		// Token: 0x06000A8C RID: 2700 RVA: 0x0003FCC7 File Offset: 0x0003DEC7
		public bool HasStatus(StatusEffects iStatus)
		{
			return (this.mCurrentStatusEffects & iStatus) == iStatus;
		}

		// Token: 0x06000A8D RID: 2701 RVA: 0x0003FCD5 File Offset: 0x0003DED5
		public StatusEffect[] GetStatusEffects()
		{
			return this.mStatusEffects;
		}

		// Token: 0x06000A8E RID: 2702 RVA: 0x0003FCE0 File Offset: 0x0003DEE0
		public float StatusMagnitude(StatusEffects iStatus)
		{
			int num = StatusEffect.StatusIndex(iStatus);
			if (!this.mStatusEffects[num].Dead)
			{
				return this.mStatusEffects[num].Magnitude;
			}
			return 0f;
		}

		// Token: 0x17000248 RID: 584
		// (get) Token: 0x06000A8F RID: 2703 RVA: 0x0003FD1E File Offset: 0x0003DF1E
		public override bool Dead
		{
			get
			{
				return this.mHitPoints <= 0f;
			}
		}

		// Token: 0x06000A90 RID: 2704 RVA: 0x0003FD30 File Offset: 0x0003DF30
		public void SetSlow()
		{
		}

		// Token: 0x06000A91 RID: 2705 RVA: 0x0003FD32 File Offset: 0x0003DF32
		public void Damage(float iDamage, Elements iElement)
		{
			if (NetworkManager.Instance.State == NetworkState.Client)
			{
				return;
			}
			this.mHitPoints -= iDamage;
			if (this.mHitPoints <= 0f)
			{
				this.SpawnGibs();
			}
		}

		// Token: 0x17000249 RID: 585
		// (get) Token: 0x06000A92 RID: 2706 RVA: 0x0003FD63 File Offset: 0x0003DF63
		public float Volume
		{
			get
			{
				return this.mVolume;
			}
		}

		// Token: 0x06000A93 RID: 2707 RVA: 0x0003FD6C File Offset: 0x0003DF6C
		public bool SegmentIntersect(out Vector3 oPosition, Segment iSeg, float iSegmentRadius)
		{
			Vector3 value = default(Vector3);
			Box box = this.mCollision.GetPrimitiveNewWorld(0) as Box;
			float t;
			float num = Distance.SegmentBoxDistanceSq(out t, out value.X, out value.Y, out value.Z, iSeg, box);
			value += box.Position;
			iSeg.GetPoint(t, out oPosition);
			return num <= iSegmentRadius * iSegmentRadius;
		}

		// Token: 0x06000A94 RID: 2708 RVA: 0x0003FDD4 File Offset: 0x0003DFD4
		public virtual DamageResult InternalDamage(DamageCollection5 iDamages, Entity iAttacker, double iTimeStamp, Vector3 iAttackPosition, Defines.DamageFeatures iFeatures)
		{
			DamageResult damageResult = this.InternalDamage(iDamages.A, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
			DamageResult damageResult2 = this.InternalDamage(iDamages.B, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
			DamageResult damageResult3 = this.InternalDamage(iDamages.C, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
			DamageResult damageResult4 = this.InternalDamage(iDamages.D, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
			DamageResult damageResult5 = this.InternalDamage(iDamages.E, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
			return (damageResult | damageResult2 | damageResult3 | damageResult4 | damageResult5) & ~((damageResult & damageResult2 & damageResult3 & damageResult4 & damageResult5) ^ DamageResult.Deflected);
		}

		// Token: 0x06000A95 RID: 2709 RVA: 0x0003FE64 File Offset: 0x0003E064
		public virtual DamageResult InternalDamage(Damage iDamage, Entity iAttacker, double iTimeStamp, Vector3 iAttackPosition, Defines.DamageFeatures iFeatures)
		{
			DamageResult damageResult = DamageResult.None;
			if (this.Dead)
			{
				return damageResult;
			}
			if ((short)(iDamage.AttackProperty & AttackProperties.Knockback) == 6)
			{
				Vector3 vector = iAttackPosition;
				Vector3 position = this.Position;
				Vector3.Subtract(ref position, ref vector, out position);
				position.Y = 0f;
				if (position.LengthSquared() > 1E-45f)
				{
					position.Normalize();
					Vector3 vector2 = this.CalcImpulseVelocity(position, 0.6980619f, iDamage.Amount, iDamage.Magnitude);
					if (vector2.LengthSquared() > 1E-06f)
					{
						if (Defines.FeatureKnockback(iFeatures))
						{
							this.AddImpulseVelocity(ref vector2);
						}
						damageResult |= DamageResult.Knockedback;
					}
				}
			}
			else if ((short)(iDamage.AttackProperty & AttackProperties.Pushed) == 4)
			{
				Vector3 vector3 = iAttackPosition;
				Vector3 position2 = this.Position;
				Vector3.Subtract(ref position2, ref vector3, out position2);
				position2.Y = 0f;
				position2.Normalize();
				Vector3 vector4 = this.CalcImpulseVelocity(position2, 0.17453292f, iDamage.Amount, iDamage.Magnitude);
				if (vector4.LengthSquared() > 1E-06f)
				{
					if (Defines.FeatureKnockback(iFeatures))
					{
						this.AddImpulseVelocity(ref vector4);
					}
					damageResult |= DamageResult.Pushed;
				}
			}
			iDamage.ApplyResistancesInclusive(this.mResistances, null, null, this.mCurrentStatusEffects);
			if ((short)(iDamage.AttackProperty & AttackProperties.Status) == 32)
			{
				if ((iDamage.Element & Elements.Fire) == Elements.Fire)
				{
					damageResult |= this.AddStatusEffect(new StatusEffect(StatusEffects.Burning, iDamage.Amount, iDamage.Magnitude, 0f, this.Radius));
				}
				if ((iDamage.Element & Elements.Cold) == Elements.Cold)
				{
					damageResult |= this.AddStatusEffect(new StatusEffect(StatusEffects.Cold, iDamage.Amount, iDamage.Magnitude, 0f, this.Radius));
				}
				if ((iDamage.Element & Elements.Water) == Elements.Water)
				{
					damageResult |= this.AddStatusEffect(new StatusEffect(StatusEffects.Wet, iDamage.Amount, iDamage.Magnitude, 0f, this.Radius));
				}
				if ((iDamage.Element & Elements.Poison) == Elements.Poison)
				{
					damageResult |= this.AddStatusEffect(new StatusEffect(StatusEffects.Poisoned, iDamage.Amount, iDamage.Magnitude, 0f, this.Radius));
				}
			}
			if ((short)(iDamage.AttackProperty & AttackProperties.Damage) == 1)
			{
				if (iDamage.Element == Elements.Lightning && this.HasStatus(StatusEffects.Wet))
				{
					iDamage.Amount *= 3f;
				}
				if ((iDamage.Element & Elements.Life) == Elements.Life)
				{
					StatusEffect[] array = this.mStatusEffects;
					int num = StatusEffect.StatusIndex(StatusEffects.Poisoned);
					array[num].Magnitude = array[num].Magnitude - iDamage.Magnitude;
					if (this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Poisoned)].Magnitude <= 0f)
					{
						this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Poisoned)].Stop();
					}
				}
				if ((iDamage.Element & Elements.PhysicalElements) != Elements.None && this.HasStatus(StatusEffects.Frozen))
				{
					iDamage.Amount = Math.Max(iDamage.Amount - 200f, 0f);
					iDamage.Magnitude = Math.Max(1f, iDamage.Magnitude);
					iDamage.Amount *= 3f;
				}
				iDamage.Amount = (float)((int)(iDamage.Amount * iDamage.Magnitude));
				if (Defines.FeatureDamage(iFeatures))
				{
					this.mHitPoints -= iDamage.Amount;
				}
				if ((short)(iDamage.AttackProperty & AttackProperties.Piercing) != 0 && iDamage.Magnitude > 0f && iDamage.Amount > 0f)
				{
					damageResult |= DamageResult.Pierced;
				}
				if (iDamage.Amount > 0f)
				{
					damageResult |= DamageResult.Damaged;
				}
				if (iDamage.Amount == 0f)
				{
					damageResult |= DamageResult.Deflected;
				}
				if (iDamage.Amount < 0f)
				{
					damageResult |= DamageResult.Healed;
				}
			}
			if (this.mHitPoints > this.mMaxHitPoints)
			{
				this.mHitPoints = this.mMaxHitPoints;
			}
			if (iDamage.Amount == 0f)
			{
				damageResult |= DamageResult.Deflected;
			}
			if (this.mHitPoints <= 0f)
			{
				damageResult |= DamageResult.Killed;
				this.SpawnGibs();
			}
			if ((damageResult & DamageResult.Damaged) != DamageResult.None && this.mOnDamage != 0 && NetworkManager.Instance.State != NetworkState.Client)
			{
				this.mPlayState.Level.CurrentScene.ExecuteTrigger(this.mOnDamage, null, false);
			}
			StatisticsManager.Instance.AddDamageEvent(this.mPlayState, iAttacker as IDamageable, this, iTimeStamp, iDamage, damageResult);
			return damageResult;
		}

		// Token: 0x06000A96 RID: 2710 RVA: 0x000402C0 File Offset: 0x0003E4C0
		public virtual DamageResult AddStatusEffect(StatusEffect iStatusEffect)
		{
			DamageResult damageResult = DamageResult.None;
			if (!iStatusEffect.Dead)
			{
				bool flag = false;
				StatusEffects damageType = iStatusEffect.DamageType;
				switch (damageType)
				{
				case StatusEffects.Burning:
					if (!this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Cold)].Dead || !this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Wet)].Dead || !this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Frozen)].Dead)
					{
						this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Cold)].Stop();
						this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Cold)] = default(StatusEffect);
						this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Wet)].Stop();
						this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Wet)] = default(StatusEffect);
						StatusEffect[] array = this.mStatusEffects;
						int num = StatusEffect.StatusIndex(StatusEffects.Frozen);
						array[num].Magnitude = array[num].Magnitude - iStatusEffect.Magnitude;
						if (this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Frozen)].Magnitude <= 0f)
						{
							this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Frozen)].Stop();
							this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Frozen)] = default(StatusEffect);
						}
						flag = true;
					}
					break;
				case StatusEffects.Wet:
					if (!this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Burning)].Dead)
					{
						this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Burning)].Stop();
						this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Burning)] = default(StatusEffect);
						flag = true;
					}
					break;
				default:
					if (damageType != StatusEffects.Cold)
					{
						if (damageType == StatusEffects.Poisoned)
						{
							iStatusEffect.Magnitude *= this.mResistances[Defines.ElementIndex(Elements.Poison)].Multiplier;
						}
					}
					else
					{
						float num2 = iStatusEffect.Magnitude;
						if (!this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Burning)].Dead)
						{
							this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Burning)].Stop();
							this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Burning)] = default(StatusEffect);
							flag = true;
						}
						num2 *= this.mResistances[Defines.ElementIndex(Elements.Cold)].Multiplier * (0.5f + 0.5f * (500f / this.Body.Mass));
						if (this.HasStatus(StatusEffects.Wet))
						{
							this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Frozen)] = new StatusEffect(StatusEffects.Frozen, 0f, 0.5f, 0f, this.mRadius);
							this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Wet)].Stop();
							this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Wet)] = default(StatusEffect);
						}
						if (this.HasStatus(StatusEffects.Frozen))
						{
							StatusEffect[] array2 = this.mStatusEffects;
							int num3 = StatusEffect.StatusIndex(StatusEffects.Frozen);
							array2[num3].Magnitude = array2[num3].Magnitude + num2;
							num2 = 0f;
						}
						iStatusEffect.Magnitude = num2;
					}
					break;
				}
				if (!flag)
				{
					int num4 = StatusEffect.StatusIndex(iStatusEffect.DamageType);
					if (iStatusEffect.DamageType == StatusEffects.Burning)
					{
						this.HasStatus(StatusEffects.Burning);
					}
					if (iStatusEffect.DamageType == StatusEffects.Wet)
					{
						this.HasStatus(StatusEffects.Wet);
					}
					if (iStatusEffect.DamageType == StatusEffects.Cold)
					{
						this.HasStatus(StatusEffects.Cold);
					}
					if (iStatusEffect.DamageType == StatusEffects.Frozen)
					{
						this.HasStatus(StatusEffects.Frozen);
					}
					this.mStatusEffects[num4] = this.mStatusEffects[num4] + iStatusEffect;
					damageResult |= DamageResult.Statusadded;
				}
				else
				{
					damageResult |= DamageResult.Statusremoved;
				}
			}
			return damageResult;
		}

		// Token: 0x06000A97 RID: 2711 RVA: 0x0004064C File Offset: 0x0003E84C
		public override void Kill()
		{
			this.mHitPoints = 0f;
			this.SpawnGibs();
			if (this.mStatusEffectLight != null)
			{
				this.mStatusEffectLight.Stop(false);
			}
		}

		// Token: 0x06000A98 RID: 2712 RVA: 0x00040673 File Offset: 0x0003E873
		public void OverKill()
		{
			this.mHitPoints = 0f;
			this.SpawnGibs();
			if (this.mStatusEffectLight != null)
			{
				this.mStatusEffectLight.Stop(false);
			}
		}

		// Token: 0x1700024A RID: 586
		// (get) Token: 0x06000A99 RID: 2713 RVA: 0x0004069A File Offset: 0x0003E89A
		public float HitPoints
		{
			get
			{
				return this.mHitPoints;
			}
		}

		// Token: 0x1700024B RID: 587
		// (get) Token: 0x06000A9A RID: 2714 RVA: 0x000406A2 File Offset: 0x0003E8A2
		public float MaxHitPoints
		{
			get
			{
				return this.mMaxHitPoints;
			}
		}

		// Token: 0x1700024C RID: 588
		// (get) Token: 0x06000A9B RID: 2715 RVA: 0x000406AA File Offset: 0x0003E8AA
		protected bool RestingHealth
		{
			get
			{
				return this.mRestingHealthTimer < 0f;
			}
		}

		// Token: 0x06000A9C RID: 2716 RVA: 0x000406BC File Offset: 0x0003E8BC
		public void SpawnGibs()
		{
			if (this.mOnDeath != 0 && NetworkManager.Instance.State != NetworkState.Client)
			{
				this.mPlayState.Level.CurrentScene.ExecuteTrigger(this.mOnDeath, null, false);
			}
			if (this.mConditions != null)
			{
				EventCondition eventCondition = default(EventCondition);
				eventCondition.EventConditionType = EventConditionType.Death;
				DamageResult damageResult;
				this.mConditions.ExecuteAll(this, null, ref eventCondition, out damageResult);
			}
			if (this.mGibs.Count > 0)
			{
				Vector3 position = this.Position;
				for (int i = 0; i < this.mGibs.Count; i++)
				{
					Gib fromCache = Gib.GetFromCache();
					if (fromCache == null)
					{
						break;
					}
					float scaleFactor = fromCache.Mass * 1.25f;
					float num = (this.Radius >= 0.2f) ? this.Radius : 0.2f;
					float num2 = 6.2831855f * (float)DamageablePhysicsEntity.sRandom.NextDouble();
					float num3 = (float)Math.Acos((double)(2f * (float)DamageablePhysicsEntity.sRandom.NextDouble() - 1f));
					Vector3 vector = new Vector3(num * (float)Math.Cos((double)num2) * (float)Math.Sin((double)num3), 3f * num * (float)Math.Sin((double)num2) * (float)Math.Sin((double)num3), num * (float)Math.Cos((double)num3));
					Vector3.Normalize(ref vector, out vector);
					Vector3 iVelocity;
					Vector3.Multiply(ref vector, scaleFactor, out iVelocity);
					iVelocity.Y = Math.Abs(iVelocity.Y);
					float iTTL = 10f + (float)DamageablePhysicsEntity.sRandom.NextDouble() * 10f;
					fromCache.Initialize(this.mGibs[i].mModel, this.mGibs[i].mMass, this.mGibs[i].mScale, position, iVelocity, iTTL, this, BloodType.none, this.mGibTrailEffect, this.HasStatus(StatusEffects.Frozen));
					Vector3 angularVelocity = default(Vector3);
					Vector3.Negate(ref vector, out angularVelocity);
					Vector3.Multiply(ref angularVelocity, 5f, out angularVelocity);
					fromCache.Body.AngularVelocity = angularVelocity;
					this.mPlayState.EntityManager.AddEntity(fromCache);
				}
				this.mGibs.Clear();
			}
		}

		// Token: 0x06000A9D RID: 2717 RVA: 0x000408F0 File Offset: 0x0003EAF0
		protected unsafe override void INetworkUpdate(ref EntityUpdateMessage iMsg)
		{
			base.INetworkUpdate(ref iMsg);
			if ((ushort)(iMsg.Features & EntityFeatures.Damageable) != 0)
			{
				this.mHitPoints = iMsg.HitPoints;
			}
			if ((ushort)(iMsg.Features & EntityFeatures.StatusEffected) != 0)
			{
				this.mCurrentStatusEffects = iMsg.StatusEffects;
				fixed (float* ptr = &iMsg.StatusEffectMagnitude.FixedElementField)
				{
					fixed (float* ptr2 = &iMsg.StatusEffectDPS.FixedElementField)
					{
						for (int i = 0; i < 9; i++)
						{
							StatusEffects iStatus = StatusEffect.StatusFromIndex(i);
							if (this.StatusMagnitude(iStatus) > 0f)
							{
								if (ptr[i] > 0f)
								{
									this.mStatusEffects[i].Magnitude = ptr[i];
									this.mStatusEffects[i].DPS = ptr2[i];
								}
								else
								{
									this.mStatusEffects[i].Stop();
									this.mStatusEffects[i] = default(StatusEffect);
								}
							}
							else if (ptr[i] > 0f)
							{
								this.AddStatusEffect(new StatusEffect(iStatus, ptr2[i], ptr[i], 0f, this.Radius));
							}
						}
					}
				}
			}
		}

		// Token: 0x06000A9E RID: 2718 RVA: 0x00040A1C File Offset: 0x0003EC1C
		protected unsafe override void IGetNetworkUpdate(out EntityUpdateMessage oMsg, float iPrediction)
		{
			base.IGetNetworkUpdate(out oMsg, iPrediction);
			if (!this.RestingHealth)
			{
				oMsg.Features |= EntityFeatures.Damageable;
				oMsg.HitPoints = this.mHitPoints;
				this.mLastHitpoints = this.mHitPoints;
				oMsg.Features |= EntityFeatures.StatusEffected;
				oMsg.StatusEffects = this.mCurrentStatusEffects;
				fixed (float* ptr = &oMsg.StatusEffectMagnitude.FixedElementField)
				{
					fixed (float* ptr2 = &oMsg.StatusEffectDPS.FixedElementField)
					{
						for (int i = 0; i < 9; i++)
						{
							ptr[i] = this.mStatusEffects[i].Magnitude;
							ptr2[i] = this.mStatusEffects[i].DPS;
						}
					}
				}
			}
		}

		// Token: 0x06000A9F RID: 2719 RVA: 0x00040ADD File Offset: 0x0003ECDD
		public void Electrocute(IDamageable iTarget, float iMultiplyer)
		{
		}

		// Token: 0x06000AA0 RID: 2720 RVA: 0x00040ADF File Offset: 0x0003ECDF
		internal void AddAnimation(PhysAnimationControl physAnimCont)
		{
			this.mAnimations.Add(physAnimCont);
		}

		// Token: 0x0400098B RID: 2443
		protected float mMaxHitPoints;

		// Token: 0x0400098C RID: 2444
		protected float mHitPoints;

		// Token: 0x0400098D RID: 2445
		private List<GibReference> mGibs = new List<GibReference>();

		// Token: 0x0400098E RID: 2446
		private List<PhysAnimationControl> mAnimations = new List<PhysAnimationControl>(10);

		// Token: 0x0400098F RID: 2447
		private float mOldHitPoints;

		// Token: 0x04000990 RID: 2448
		private float mOldPercentage;

		// Token: 0x04000991 RID: 2449
		private Resistance[] mResistances;

		// Token: 0x04000992 RID: 2450
		private bool mCanHasStatus;

		// Token: 0x04000993 RID: 2451
		private float mVolume;

		// Token: 0x04000994 RID: 2452
		private int mOnDeath;

		// Token: 0x04000995 RID: 2453
		private int mOnDamage;

		// Token: 0x04000996 RID: 2454
		protected DynamicLight mStatusEffectLight;

		// Token: 0x04000997 RID: 2455
		protected StatusEffects mCurrentStatusEffects;

		// Token: 0x04000998 RID: 2456
		protected StatusEffect[] mStatusEffects = new StatusEffect[9];

		// Token: 0x04000999 RID: 2457
		protected float mRestingHealthTimer;

		// Token: 0x0400099A RID: 2458
		protected float mLastHitpoints;

		// Token: 0x0400099B RID: 2459
		protected static readonly Random sRandom = new Random();
	}
}
