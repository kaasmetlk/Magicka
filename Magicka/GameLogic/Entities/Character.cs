using System;
using System.Collections.Generic;
using JigLibX.Collision;
using JigLibX.Geometry;
using Magicka.Achievements;
using Magicka.Audio;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.Entities.Abilities.SpecialAbilities;
using Magicka.GameLogic.Entities.AnimationActions;
using Magicka.GameLogic.Entities.Buffs;
using Magicka.GameLogic.Entities.CharacterStates;
using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.Spells.SpellEffects;
using Magicka.GameLogic.Statistics;
using Magicka.GameLogic.UI;
using Magicka.Gamers;
using Magicka.Graphics;
using Magicka.Graphics.Effects;
using Magicka.Graphics.Lights;
using Magicka.Levels;
using Magicka.Localization;
using Magicka.Network;
using Magicka.Physics;
using Magicka.WebTools.Paradox.Telemetry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using PolygonHead.Lights;
using XNAnimation;
using XNAnimation.Controllers;
using XNAnimation.Effects;

namespace Magicka.GameLogic.Entities
{
	// Token: 0x020001BB RID: 443
	public abstract class Character : Entity, ISpellCaster, IStatusEffected, IDamageable
	{
		// Token: 0x06000DAB RID: 3499 RVA: 0x00051690 File Offset: 0x0004F890
		public virtual bool HasStatus(StatusEffects iStatus)
		{
			return (this.mCurrentStatusEffects & iStatus) == iStatus;
		}

		// Token: 0x06000DAC RID: 3500 RVA: 0x0005169E File Offset: 0x0004F89E
		public StatusEffect[] GetStatusEffects()
		{
			return this.mStatusEffects;
		}

		// Token: 0x06000DAD RID: 3501 RVA: 0x000516A8 File Offset: 0x0004F8A8
		public virtual float StatusMagnitude(StatusEffects iStatus)
		{
			int num = StatusEffect.StatusIndex(iStatus);
			if (!this.mStatusEffects[num].Dead)
			{
				return this.mStatusEffects[num].Magnitude;
			}
			return 0f;
		}

		// Token: 0x06000DAE RID: 3502 RVA: 0x000516E8 File Offset: 0x0004F8E8
		public virtual void Damage(float iDamage, Elements iElement)
		{
			if (this.IsImmortal)
			{
				return;
			}
			this.mInvisibilityTimer = 0f;
			DamageResult iResult;
			if (this.mSelfShield.Active)
			{
				iResult = this.mSelfShield.Damage(ref iDamage, iElement);
			}
			if ((iElement & Elements.Fire) == Elements.Fire && this.HasStatus(StatusEffects.Greased))
			{
				iDamage *= 2f;
			}
			float num = this.mHitPoints;
			float num2 = this.mHitPoints - iDamage;
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				this.mHitPoints -= iDamage;
			}
			if (num2 <= 0f & num > 0f)
			{
				iResult = DamageResult.Killed;
			}
			else if (iDamage >= 0f)
			{
				iResult = DamageResult.Damaged;
				this.StopHypnotize();
			}
			else
			{
				iResult = DamageResult.Healed;
				if (this is Avatar)
				{
					Profile.Instance.AddHealedAmount(base.PlayState, iDamage);
				}
			}
			if (this.IsGripping && this.mLastAccumulationDamager != null && this.mLastAccumulationDamager.Avatar == this.mGrippedCharacter)
			{
				this.mGripDamageAccumulation += iDamage;
			}
			if (this.mLastAccumulationDamager != null && (iElement & Elements.Life) == Elements.None)
			{
				StatisticsManager.Instance.AddDamageEvent(this.mPlayState, this.mLastAccumulationDamager.Avatar, this, base.PlayState.PlayTime, new Damage(AttackProperties.Damage, iElement, (float)((int)iDamage), 1f), iResult);
			}
			if (iElement <= Elements.Fire)
			{
				if (iElement != Elements.Earth)
				{
					if (iElement == Elements.Fire)
					{
						this.mFireDamageAccumulation += iDamage;
					}
				}
				else
				{
					this.mBleedDamageAccumulation += iDamage;
				}
			}
			else if (iElement != Elements.Life)
			{
				if (iElement == Elements.Poison)
				{
					this.mPoisonDamageAccumulation += iDamage;
				}
			}
			else if (num2 < this.mMaxHitPoints)
			{
				this.mHealingAccumulation += iDamage;
			}
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				if (num2 > this.mMaxHitPoints)
				{
					this.mHitPoints = this.mMaxHitPoints;
				}
				if (this.IsEntangled && (iElement & Elements.Fire) == Elements.Fire)
				{
					this.mEntaglement.DecreaseEntanglement(iDamage, iElement);
				}
			}
		}

		// Token: 0x17000348 RID: 840
		// (get) Token: 0x06000DAF RID: 3503 RVA: 0x000518D1 File Offset: 0x0004FAD1
		public override bool Dead
		{
			get
			{
				return !this.mCannotDieWithoutExplicitKill & this.mDead;
			}
		}

		// Token: 0x17000349 RID: 841
		// (get) Token: 0x06000DB0 RID: 3504 RVA: 0x000518E3 File Offset: 0x0004FAE3
		// (set) Token: 0x06000DB1 RID: 3505 RVA: 0x000518EB File Offset: 0x0004FAEB
		public float HitPoints
		{
			get
			{
				return this.mHitPoints;
			}
			set
			{
				this.mHitPoints = value;
			}
		}

		// Token: 0x1700034A RID: 842
		// (get) Token: 0x06000DB2 RID: 3506 RVA: 0x000518F4 File Offset: 0x0004FAF4
		// (set) Token: 0x06000DB3 RID: 3507 RVA: 0x000518FC File Offset: 0x0004FAFC
		public float MaxHitPoints
		{
			get
			{
				return this.mMaxHitPoints;
			}
			set
			{
				this.mMaxHitPoints = value;
			}
		}

		// Token: 0x06000DB4 RID: 3508 RVA: 0x00051908 File Offset: 0x0004FB08
		public bool SegmentIntersect(out Vector3 oPosition, Segment iSeg, float iSegmentRadius)
		{
			if (this.mEthereal)
			{
				oPosition = default(Vector3);
				return false;
			}
			Segment seg = default(Segment);
			seg.Origin = this.Position;
			seg.Origin.Y = seg.Origin.Y - this.Capsule.Length * 0.5f;
			seg.Delta = this.Capsule.Orientation.Backward;
			Vector3.Multiply(ref seg.Delta, this.Capsule.Length, out seg.Delta);
			float t;
			float t2;
			float num = Distance.SegmentSegmentDistanceSq(out t, out t2, seg, iSeg);
			float num2 = iSegmentRadius + this.Capsule.Radius;
			num2 *= num2;
			if (num > num2)
			{
				oPosition = default(Vector3);
				return false;
			}
			Vector3 vector;
			seg.GetPoint(t, out vector);
			Vector3 vector2;
			iSeg.GetPoint(t2, out vector2);
			Vector3.Subtract(ref vector2, ref vector, out vector2);
			vector2.Normalize();
			Vector3.Multiply(ref vector2, this.Capsule.Radius, out vector2);
			Vector3.Add(ref vector, ref vector2, out oPosition);
			return true;
		}

		// Token: 0x06000DB5 RID: 3509 RVA: 0x00051A10 File Offset: 0x0004FC10
		public virtual DamageResult InternalDamage(DamageCollection5 iDamages, Entity iAttacker, double iTimeStamp, Vector3 iAttackPosition, Defines.DamageFeatures iFeatures)
		{
			DamageResult damageResult = this.InternalDamage(iDamages.A, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
			DamageResult damageResult2 = this.InternalDamage(iDamages.B, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
			DamageResult damageResult3 = this.InternalDamage(iDamages.C, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
			DamageResult damageResult4 = this.InternalDamage(iDamages.D, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
			DamageResult damageResult5 = this.InternalDamage(iDamages.E, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
			return (damageResult | damageResult2 | damageResult3 | damageResult4 | damageResult5) & ~((damageResult & damageResult2 & damageResult3 & damageResult4 & damageResult5) ^ DamageResult.Deflected);
		}

		// Token: 0x06000DB6 RID: 3510 RVA: 0x00051AA0 File Offset: 0x0004FCA0
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
					if (this is Avatar)
					{
						num3 *= this.Equipment[1].Item.Resistance[i].Multiplier;
						num4 += this.Equipment[1].Item.Resistance[i].Modifier;
					}
					for (int j = 0; j < this.mBuffs.Count; j++)
					{
						BuffStorage buffStorage = this.mBuffs[j];
						if (buffStorage.BuffType == BuffType.Resistance && (buffStorage.BuffResistance.Resistance.ResistanceAgainst & elements) == elements)
						{
							num3 *= buffStorage.BuffResistance.Resistance.Multiplier;
							num4 += buffStorage.BuffResistance.Resistance.Modifier;
						}
					}
					num += num4;
					num2 += num3;
				}
			}
			float num5 = MathHelper.Clamp(num / 300f + num2, -1f, 1f);
			return 1f - num5;
		}

		// Token: 0x06000DB7 RID: 3511 RVA: 0x00051C28 File Offset: 0x0004FE28
		public virtual DamageResult InternalDamage(Damage iDamage, Entity iAttacker, double iTimeStamp, Vector3 iAttackPosition, Defines.DamageFeatures iFeatures)
		{
			DamageResult damageResult = DamageResult.None;
			if (this.Dead | this.IsImmortal | this.mResistances == null)
			{
				return damageResult;
			}
			float num = this.mHitPoints;
			this.mInvisibilityTimer = 0f;
			if ((short)(iDamage.AttackProperty & AttackProperties.Entanglement) == 64 && !this.HasStatus(StatusEffects.Burning))
			{
				if (iAttacker != this)
				{
					this.Entangle(iDamage.Magnitude);
					return DamageResult.Hit;
				}
				return DamageResult.None;
			}
			else
			{
				if (this.IsEntangled && ((short)(iDamage.AttackProperty & AttackProperties.Damage) == 1 || ((short)(iDamage.AttackProperty & AttackProperties.Status) == 32 && (iDamage.Element & Elements.Fire) == Elements.Fire)))
				{
					this.mEntaglement.DecreaseEntanglement(iDamage.Magnitude, iDamage.Element);
				}
				if (this.mSelfShield.Active)
				{
					damageResult |= this.mSelfShield.Damage(this, ref iDamage, iAttacker, iAttackPosition, iFeatures);
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
							this.mKnockedDown = true;
							damageResult |= DamageResult.Knockedback;
						}
					}
				}
				else
				{
					if ((short)(iDamage.AttackProperty & AttackProperties.Pushed) == 4)
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
					if ((short)(iDamage.AttackProperty & AttackProperties.Knockdown) == 2)
					{
						this.KnockDown(iDamage.Magnitude);
					}
				}
				if ((short)(iDamage.AttackProperty & AttackProperties.Damage) == 1 && (iDamage.Element & (Elements.Earth | Elements.Lightning)) != Elements.None && this.BlockItem >= 0)
				{
					float num2 = 0f;
					if (this.BlockDamage(iDamage, iAttackPosition, out num2))
					{
						Vector3 position3 = this.Body.Position;
						Vector3.Subtract(ref iAttackPosition, ref position3, out position3);
						VisualEffectReference visualEffectReference;
						EffectManager.Instance.StartEffect("melee_block".GetHashCodeCustom(), ref iAttackPosition, ref position3, out visualEffectReference);
						if (num2 >= iDamage.Amount * iDamage.Magnitude)
						{
							damageResult |= DamageResult.Deflected;
						}
						else
						{
							iDamage.Amount = Math.Max(0f, iDamage.Amount - num2);
						}
					}
				}
				if (Defines.FeatureDamage(iFeatures) && (short)(iDamage.AttackProperty & AttackProperties.Status) == 32)
				{
					if ((iDamage.Element & Elements.Fire) == Elements.Fire)
					{
						damageResult |= this.AddStatusEffect(new StatusEffect(StatusEffects.Burning, iDamage.Amount, iDamage.Magnitude, this.Capsule.Length, this.Radius));
					}
					if ((iDamage.Element & Elements.Cold) == Elements.Cold)
					{
						damageResult |= this.AddStatusEffect(new StatusEffect(StatusEffects.Cold, iDamage.Amount, iDamage.Magnitude, this.Capsule.Length, this.Radius));
					}
					if ((iDamage.Element & Elements.Water) == Elements.Water)
					{
						damageResult |= this.AddStatusEffect(new StatusEffect(StatusEffects.Wet, iDamage.Amount, iDamage.Magnitude, this.Capsule.Length, this.Radius));
					}
					if ((iDamage.Element & Elements.Poison) == Elements.Poison)
					{
						damageResult |= this.AddStatusEffect(new StatusEffect(StatusEffects.Poisoned, iDamage.Amount, iDamage.Magnitude, this.Capsule.Length, this.Radius));
					}
					if ((iDamage.Element & Elements.Life) == Elements.Life)
					{
						damageResult |= this.AddStatusEffect(new StatusEffect(StatusEffects.Healing, iDamage.Amount, iDamage.Magnitude, this.Capsule.Length, this.Radius));
					}
					if ((iDamage.Element & Elements.Steam) == Elements.Steam)
					{
						damageResult |= this.AddStatusEffect(new StatusEffect(StatusEffects.Wet, iDamage.Amount, iDamage.Magnitude, this.Capsule.Length, this.Radius));
					}
				}
				iDamage.ApplyResistances(this.mResistances, (this is Avatar) ? this.Equipment[1].Item.Resistance : null, this.mBuffs, this.mCurrentStatusEffects);
				float num3 = Math.Abs(iDamage.Magnitude);
				if (num3 <= 1E-45f)
				{
					damageResult |= DamageResult.Deflected;
				}
				if ((damageResult & DamageResult.Deflected) == DamageResult.Deflected)
				{
					return damageResult;
				}
				if ((short)(iDamage.AttackProperty & AttackProperties.Stun) == 128 && iDamage.Amount > 0f)
				{
					this.mStunTimer = this.mStunTime;
				}
				if ((short)(iDamage.AttackProperty & AttackProperties.Bleed) == 256 && iDamage.Amount > 0f)
				{
					damageResult |= this.AddStatusEffect(new StatusEffect(StatusEffects.Bleeding, this.mBleedRate, 1f, this.Capsule.Length, this.Radius));
				}
				if ((short)(iDamage.AttackProperty & AttackProperties.Damage) == 1)
				{
					if ((iDamage.Element & Elements.Lightning) == Elements.Lightning)
					{
						if (this.mZapTimer <= 0f)
						{
							this.mZapTimer = 0.1f;
						}
						if (this.HasStatus(StatusEffects.Wet))
						{
							this.mHitByLightning = 0.1f;
							iDamage.Amount *= 2f;
						}
					}
					if ((iDamage.Element & Elements.Life) == Elements.Life)
					{
						StatusEffect[] array = this.mStatusEffects;
						int num4 = StatusEffect.StatusIndex(StatusEffects.Poisoned);
						array[num4].Magnitude = array[num4].Magnitude - iDamage.Magnitude;
						if (this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Poisoned)].Magnitude <= 0f)
						{
							this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Poisoned)].Stop();
						}
					}
					if ((iDamage.Element & (Elements.Earth | Elements.Arcane | Elements.Ice)) != Elements.None)
					{
						if (this.HasStatus(StatusEffects.Frozen))
						{
							iDamage.Amount *= 3f;
						}
						else if (this.mBlood != BloodType.none && GlobalSettings.Instance.BloodAndGore == SettingOptions.On && iDamage.Amount > 0f && Defines.FeatureEffects(iFeatures))
						{
							Vector3 position4 = this.Position;
							Vector3 direction = this.Direction;
							VisualEffectReference visualEffectReference2;
							EffectManager.Instance.StartEffect(Gib.GORE_SPLASH_EFFECTS[(int)this.mBlood], ref position4, ref direction, out visualEffectReference2);
							SpellSoundVariables iVariables = default(SpellSoundVariables);
							iVariables.mMagnitude = iDamage.Amount;
							AudioManager.Instance.PlayCue<SpellSoundVariables>(Banks.Misc, Defines.SOUNDS_GORE[(int)this.mBlood], iVariables, base.AudioEmitter);
							Segment iSeg = default(Segment);
							Vector3 vector5 = new Vector3((float)((MagickaMath.Random.NextDouble() - 0.5) * 2.0 * (double)this.Radius), 0f, (float)((MagickaMath.Random.NextDouble() - 0.5) * 2.0 * (double)this.Radius));
							iSeg.Origin = this.Position;
							Vector3.Add(ref iSeg.Origin, ref vector5, out iSeg.Origin);
							iSeg.Delta = this.Position;
							Vector3.Subtract(ref iSeg.Delta, ref iAttackPosition, out iSeg.Delta);
							iSeg.Delta.Normalize();
							iSeg.Delta.Y = -2f;
							Vector3.Multiply(ref iSeg.Delta, 5f, out iSeg.Delta);
							float num5;
							Vector3 vector6;
							AnimatedLevelPart iAnimation;
							this.mPlayState.Level.CurrentScene.SegmentIntersect(out num5, out position4, out vector6, out iAnimation, iSeg);
							DecalManager.Instance.AddAlphaBlendedDecal((Decal)(this.mBlood * BloodType.insect + (int)Math.Min(iDamage.Amount * iDamage.Magnitude / 400f, 3f)), iAnimation, 4f, ref position4, ref vector6, 60f);
						}
					}
					iDamage.Amount *= iDamage.Magnitude;
					if (Defines.FeatureDamage(iFeatures) && NetworkManager.Instance.State != NetworkState.Client)
					{
						this.mHitPoints -= iDamage.Amount;
					}
					if (iAttacker is Avatar && !((iAttacker as Avatar).Player.Gamer is NetworkGamer))
					{
						if (Defines.FeatureDamage(iFeatures) && iDamage.Element == Elements.Life)
						{
							Profile.Instance.AddHealedAmount(base.PlayState, iDamage.Amount);
						}
						if (iDamage.Amount > 9000f)
						{
							AchievementsManager.Instance.AwardAchievement(base.PlayState, "itsoverninethousand");
						}
					}
					if (this.IsGripping & iAttacker == this.mGrippedCharacter)
					{
						this.mGripDamageAccumulation += iDamage.Amount;
						if (this.CurrentState is GripAttackState)
						{
							this.ChangeState(GrippingState.Instance);
						}
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
					if (iDamage.Amount > this.mHitTolerance || this.mCurrentState is RessurectionState)
					{
						damageResult |= DamageResult.Hit;
						this.mIsHit = true;
					}
					if (Defines.FeatureNotify(iFeatures))
					{
						if (iDamage.Amount != 0f)
						{
							this.mTimeSinceLastDamage = 0f;
						}
						if (this.mLastDamageIndex >= 0)
						{
							DamageNotifyer.Instance.AddToNumber(this.mLastDamageIndex, iDamage.Amount);
						}
						else
						{
							this.mLastDamageAmount = iDamage.Amount;
							this.mLastDamageElement = iDamage.Element;
							Vector3 position5 = this.Position;
							position5.Y += this.Capsule.Length * 0.5f + this.mRadius;
							this.mLastDamageIndex = DamageNotifyer.Instance.AddNumber(iDamage.Amount, ref position5, 0.4f, true);
						}
					}
				}
				if (this.mHitPoints > this.mMaxHitPoints)
				{
					this.mHitPoints = this.mMaxHitPoints;
				}
				float num6 = num;
				if ((short)(iDamage.AttackProperty & AttackProperties.Damage) == 1)
				{
					num6 -= iDamage.Amount;
				}
				if (iDamage.Amount == 0f)
				{
					damageResult |= DamageResult.Deflected;
				}
				else if (iDamage.Amount > 0f)
				{
					this.StopHypnotize();
				}
				if (num6 <= 0f)
				{
					if (this.HasStatus(StatusEffects.Frozen) || this.OverKilled(num6))
					{
						if (Defines.FeatureDamage(iFeatures) && (iDamage.Element & ~Elements.Arcane) != Elements.None)
						{
							this.OverKill();
						}
						damageResult |= DamageResult.OverKilled;
					}
					damageResult |= DamageResult.Killed;
				}
				Avatar avatar = iAttacker as Avatar;
				if (avatar != null && ((damageResult & DamageResult.Statusadded) == DamageResult.Statusadded & (iDamage.Element & Elements.Fire) == Elements.Fire))
				{
					this.mLastAccumulationDamager = avatar.Player;
				}
				DamageResult damageResult2 = damageResult;
				if (num <= 0f)
				{
					damageResult2 &= ~DamageResult.Killed;
				}
				if ((damageResult & DamageResult.Damaged) != DamageResult.None)
				{
					if (this.mOnDamageTrigger != 0)
					{
						this.mPlayState.Level.CurrentScene.ExecuteTrigger(this.mOnDamageTrigger, this, false);
					}
					StatisticsManager.Instance.AddDamageEvent(this.mPlayState, iAttacker as IDamageable, this, iTimeStamp, iDamage, damageResult2);
				}
				if (iAttacker is Avatar && !((iAttacker as Avatar).Player.Gamer is NetworkGamer) && (damageResult & DamageResult.OverKilled) == DamageResult.OverKilled && this is NonPlayerCharacter)
				{
					NonPlayerCharacter nonPlayerCharacter = this as NonPlayerCharacter;
					if (nonPlayerCharacter != null)
					{
						bool flag = false;
						for (int i = 0; i < Character.BEASTMEN_HASHES.Length; i++)
						{
							if (nonPlayerCharacter.Type == Character.BEASTMEN_HASHES[i])
							{
								flag = true;
							}
						}
						if (flag)
						{
							base.PlayState.ItsRainingBeastMen[(iAttacker as Avatar).Player.ID][(this as NonPlayerCharacter).Handle] = 5f;
						}
					}
				}
				if (this is Avatar)
				{
					if (this.mHitPoints <= 0f && Game.Instance.PlayerCount > 1)
					{
						TutorialManager.Instance.SetTip(TutorialManager.Tips.Dying, TutorialManager.Position.Top);
					}
					if (this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Poisoned)].Magnitude > 0f)
					{
						TutorialManager.Instance.SetTip(TutorialManager.Tips.Poison, TutorialManager.Position.Top);
					}
					if (this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Wet)].Magnitude > 0f)
					{
						TutorialManager.Instance.SetTip(TutorialManager.Tips.Wet, TutorialManager.Position.Top);
					}
					if (this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Cold)].Magnitude > 0f)
					{
						TutorialManager.Instance.SetTip(TutorialManager.Tips.Cold, TutorialManager.Position.Top);
					}
					if (this.mHitPoints > 0f && this.mHitPoints < this.mMaxHitPoints)
					{
						TutorialManager.Instance.SetTip(TutorialManager.Tips.Heal, TutorialManager.Position.Top);
					}
				}
				if (num > 0f)
				{
					this.mLastAttacker = iAttacker;
				}
				this.mLastAttackerTimer = 8f;
				this.mLastDamage = damageResult;
				return damageResult;
			}
		}

		// Token: 0x06000DB8 RID: 3512 RVA: 0x00052920 File Offset: 0x00050B20
		public override void Kill()
		{
			if (!this.CannotDieWithoutExplicitKill)
			{
				this.mInvisibilityTimer = 0f;
				if (this.HasStatus(StatusEffects.Frozen))
				{
					this.OverKill();
				}
				else
				{
					this.mHitPoints = 0f;
				}
				if (this.mStatusEffectLight != null)
				{
					this.mStatusEffectLight.Stop(false);
					this.mStatusEffectLight = null;
				}
			}
		}

		// Token: 0x06000DB9 RID: 3513 RVA: 0x00052978 File Offset: 0x00050B78
		public virtual void OverKill()
		{
			if (!this.mOverkilled && !this.CannotDieWithoutExplicitKill)
			{
				this.mDead = true;
				this.mOverkilled = true;
				this.mDeathStatusEffects = this.mCurrentStatusEffects;
				this.mInvisibilityTimer = 0f;
				this.mHitPoints = -this.mMaxHitPoints - 1000f;
				this.mPlayState.Camera.CameraShake(this.Position, 1.5f, 0.5f);
				EffectManager.Instance.Stop(ref this.mFearEffect);
				EffectManager.Instance.Stop(ref this.mWaterSplashEffect);
				this.StopHypnotize();
				this.EndCharm();
				this.StopLevitate();
				for (int i = 0; i < this.mCurrentActions.Length; i++)
				{
					this.mCurrentActions[i].Kill(this);
				}
				if (this.mStatusEffectLight != null)
				{
					this.mStatusEffectLight.Stop(false);
					this.mStatusEffectLight = null;
				}
				EventCondition eventCondition = default(EventCondition);
				eventCondition.EventConditionType = EventConditionType.OverKill;
				this.mEventConditions.ExecuteAll(this, null, ref eventCondition);
				if (this.mOnDeathTrigger != 0)
				{
					this.mPlayState.Level.CurrentScene.ExecuteTrigger(this.mOnDeathTrigger, this, false);
				}
				if (this.mLastAttacker is Avatar && !((this.mLastAttacker as Avatar).Player.Gamer is NetworkGamer))
				{
					Profile.Instance.AddOverKills(this);
				}
				this.mHitList.Clear();
				if (this.mDialog != 0 && DialogManager.Instance.DialogActive(this.mDialog))
				{
					DialogManager.Instance.End(this.mDialog);
					DialogManager.Instance.Dialogs.DialogFinished(this.mDialog);
				}
			}
		}

		// Token: 0x06000DBA RID: 3514 RVA: 0x00052B25 File Offset: 0x00050D25
		public void Electrocute(IDamageable iTarget, float iMultiplyer)
		{
		}

		// Token: 0x06000DBB RID: 3515 RVA: 0x00052B48 File Offset: 0x00050D48
		protected Character(PlayState iPlayState) : base(iPlayState)
		{
			GraphicsDevice graphicsDevice = Game.Instance.GraphicsDevice;
			if (Character.sBarrierSkinModel == null)
			{
				lock (graphicsDevice)
				{
					Character.sBarrierSkinModel = Game.Instance.Content.Load<SkinnedModel>("Models/Effects/combined_armor");
				}
				Helper.SkinnedModelDeferredMaterialFromBasicEffect(Character.sBarrierSkinModel.Model.Meshes[0].Effects[0] as SkinnedModelBasicEffect, out Character.sEarthBarrierSkinMaterial);
				Helper.SkinnedModelDeferredMaterialFromBasicEffect(Character.sBarrierSkinModel.Model.Meshes[0].Effects[1] as SkinnedModelBasicEffect, out Character.sIceBarrierSkinMaterial);
			}
			if (Character.sAuraModel == null)
			{
				lock (graphicsDevice)
				{
					Character.sAuraModel = Game.Instance.Content.Load<Model>("Models/Effects/armor_aura");
				}
			}
			if (Character.sIceCubeMap == null)
			{
				lock (graphicsDevice)
				{
					Character.sIceCubeMap = Game.Instance.Content.Load<TextureCube>("EffectTextures/iceCube");
				}
			}
			if (Character.sIceCubeNormalMap == null)
			{
				lock (graphicsDevice)
				{
					Character.sIceCubeNormalMap = Game.Instance.Content.Load<TextureCube>("EffectTextures/iceCube_NRM");
				}
			}
			this.mRenderData = new Character.RenderData[3];
			this.mNormalDistortionRenderData = new Character.NormalDistortionRenderData[3];
			this.mShieldSkinRenderData = new Character.ShieldSkinRenderData[3];
			this.mHighlightRenderData = new Character.HighlightRenderData[3];
			this.mLightningZapRenderData = new Character.LightningZapRenderData[3];
			this.mBarrierSkinRenderData = new Character.BarrierSkinRenderData[3];
			this.mArmourRenderData = new Character.HaloAuraRenderData[3];
			lock (Game.Instance.GraphicsDevice)
			{
				InvisibilityEffect iEffect = new InvisibilityEffect(Game.Instance.GraphicsDevice, Game.Instance.Content);
				for (int i = 0; i < 3; i++)
				{
					this.mRenderData[i] = new Character.RenderData(this);
					this.mNormalDistortionRenderData[i] = new Character.NormalDistortionRenderData(iEffect);
					this.mNormalDistortionRenderData[i].mSkeleton = this.mRenderData[i].mSkeleton;
					this.mShieldSkinRenderData[i] = new Character.ShieldSkinRenderData();
					this.mShieldSkinRenderData[i].mSkeleton = this.mRenderData[i].mSkeleton;
					this.mHighlightRenderData[i] = new Character.HighlightRenderData();
					this.mHighlightRenderData[i].mSkeleton = this.mRenderData[i].mSkeleton;
					this.mLightningZapRenderData[i] = new Character.LightningZapRenderData();
					this.mLightningZapRenderData[i].mSkeleton = this.mRenderData[i].mSkeleton;
					this.mBarrierSkinRenderData[i] = new Character.BarrierSkinRenderData();
					this.mBarrierSkinRenderData[i].mSkeleton = this.mRenderData[i].mSkeleton;
					this.mBarrierSkinRenderData[i].SetMesh(Character.sBarrierSkinModel.Model.Meshes[0].VertexBuffer, Character.sBarrierSkinModel.Model.Meshes[0].IndexBuffer, Character.sBarrierSkinModel.Model.Meshes[0].MeshParts[0], 0, 3, 4);
					this.mBarrierSkinRenderData[i].SetIceMesh(Character.sBarrierSkinModel.Model.Meshes[0].MeshParts[1]);
					this.mBarrierSkinRenderData[i].mMaterial = Character.sEarthBarrierSkinMaterial;
					this.mBarrierSkinRenderData[i].mIceMaterial = Character.sIceBarrierSkinMaterial;
					this.mBarrierSkinRenderData[i].RenderEarth = false;
					this.mBarrierSkinRenderData[i].RenderIce = false;
					this.mArmourRenderData[i] = new Character.HaloAuraRenderData();
					this.mArmourRenderData[i].SetMesh(Character.sAuraModel.Meshes[0], Character.sAuraModel.Meshes[0].MeshParts[0], 0);
					this.mArmourRenderData[i].AssignParts(Character.sAuraModel.Meshes[0].MeshParts[1], Character.sAuraModel.Meshes[0].MeshParts[0], Character.sAuraModel.Meshes[0].MeshParts[2]);
				}
			}
			for (int j = 0; j < this.mEquipment.Length; j++)
			{
				this.mEquipment[j] = new Attachment(iPlayState, this);
			}
			this.mBody = new CharacterBody(this);
			this.mCollision = new CollisionSkin(this.mBody);
			this.mCollision.AddPrimitive(new Capsule(default(Vector3), Character.sRotateX90, 4.5f, 1f), 1, new MaterialProperties(0f, 0f, 0f));
			this.mCollision.callbackFn += this.OnCollision;
			this.mCollision.postCollisionCallbackFn += this.PostCollision;
			this.mBody.CollisionSkin = this.mCollision;
			this.mBody.Immovable = false;
			this.mBody.Tag = this;
			this.mAnimationController = new AnimationController();
			this.mAnimationController.AnimationLooped += this.OnAnimationLooped;
			this.mAnimationController.CrossfadeFinished += this.OnCrossfadeFinished;
			this.mSpellQueue = new StaticEquatableList<Spell>(5);
			this.mGibs = new List<GibReference>();
			this.mEntaglement = new Entanglement(this);
			this.mStatusEffectCues = new Cue[3];
			this.mDoNotRender = false;
			this.BlockItem = -1;
		}

		// Token: 0x06000DBC RID: 3516 RVA: 0x00053288 File Offset: 0x00051488
		public void AddBuff(ref BuffStorage iBuff)
		{
			for (int i = 0; i < this.mBuffs.Count; i++)
			{
				BuffStorage value = this.mBuffs[i];
				if (value.UniqueID == iBuff.UniqueID)
				{
					value.TTL = iBuff.TTL;
					this.mBuffs[i] = value;
					return;
				}
			}
			if (this.mCurrentState is DeadState)
			{
				return;
			}
			if (iBuff.BuffType == BuffType.Resistance)
			{
				for (int j = 0; j < 11; j++)
				{
					Elements elements = (Elements)(1 << j);
					if ((elements & iBuff.BuffResistance.Resistance.ResistanceAgainst) != Elements.None)
					{
						Elements elements2 = elements;
						if (elements2 <= Elements.Fire)
						{
							switch (elements2)
							{
							case Elements.Water:
								if (Math.Abs(iBuff.BuffResistance.Resistance.Multiplier) < 1E-06f)
								{
									this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Wet)].Magnitude = 0f;
								}
								else
								{
									StatusEffect[] array = this.mStatusEffects;
									int num = StatusEffect.StatusIndex(StatusEffects.Wet);
									array[num].Magnitude = array[num].Magnitude * iBuff.BuffResistance.Resistance.Multiplier;
								}
								break;
							case Elements.Earth | Elements.Water:
								break;
							case Elements.Cold:
								if (Math.Abs(iBuff.BuffResistance.Resistance.Multiplier) < 1E-06f)
								{
									this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Cold)].Magnitude = 0f;
								}
								else
								{
									StatusEffect[] array2 = this.mStatusEffects;
									int num2 = StatusEffect.StatusIndex(StatusEffects.Cold);
									array2[num2].Magnitude = array2[num2].Magnitude * iBuff.BuffResistance.Resistance.Multiplier;
								}
								break;
							default:
								if (elements2 == Elements.Fire)
								{
									if (Math.Abs(iBuff.BuffResistance.Resistance.Multiplier) < 1E-06f)
									{
										this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Burning)].Magnitude = 0f;
									}
									else
									{
										StatusEffect[] array3 = this.mStatusEffects;
										int num3 = StatusEffect.StatusIndex(StatusEffects.Burning);
										array3[num3].Magnitude = array3[num3].Magnitude * iBuff.BuffResistance.Resistance.Multiplier;
									}
								}
								break;
							}
						}
						else if (elements2 != Elements.Life)
						{
							if (elements2 != Elements.Steam)
							{
								if (elements2 == Elements.Poison)
								{
									if (Math.Abs(iBuff.BuffResistance.Resistance.Multiplier) < 1E-06f)
									{
										this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Poisoned)].Magnitude = 0f;
									}
									else
									{
										StatusEffect[] array4 = this.mStatusEffects;
										int num4 = StatusEffect.StatusIndex(StatusEffects.Poisoned);
										array4[num4].Magnitude = array4[num4].Magnitude * iBuff.BuffResistance.Resistance.Multiplier;
									}
								}
							}
							else if (Math.Abs(iBuff.BuffResistance.Resistance.Multiplier) < 1E-06f)
							{
								this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Steamed)].Magnitude = 0f;
							}
							else
							{
								StatusEffect[] array5 = this.mStatusEffects;
								int num5 = StatusEffect.StatusIndex(StatusEffects.Steamed);
								array5[num5].Magnitude = array5[num5].Magnitude * iBuff.BuffResistance.Resistance.Multiplier;
							}
						}
						else if (Math.Abs(iBuff.BuffResistance.Resistance.Multiplier) < 1E-06f)
						{
							this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Healing)].Magnitude = 0f;
						}
						else
						{
							StatusEffect[] array6 = this.mStatusEffects;
							int num6 = StatusEffect.StatusIndex(StatusEffects.Healing);
							array6[num6].Magnitude = array6[num6].Magnitude * iBuff.BuffResistance.Resistance.Multiplier;
						}
					}
				}
			}
			if (iBuff.BuffType == BuffType.ModifyHitPoints && this is NonPlayerCharacter)
			{
				this.mHitPoints *= iBuff.BuffModifyHitPoints.HitPointsMultiplier;
			}
			DecalManager.DecalReference item = default(DecalManager.DecalReference);
			item.Index = -1;
			if (iBuff.VisualCategory != VisualCategory.None)
			{
				Vector2 vector = default(Vector2);
				vector.X = (vector.Y = this.mRadius * 1.5f);
				Vector3 position = this.Position;
				Vector3 vector2 = default(Vector3);
				vector2.Y = 1f;
				Vector4 vector3 = default(Vector4);
				vector3.X = iBuff.Color.X;
				vector3.Y = iBuff.Color.Y;
				vector3.Z = iBuff.Color.Z;
				DecalManager.Instance.AddAlphaBlendedDecal(Decal.BuffOffensive + (int)iBuff.VisualCategory - 1, null, ref vector, ref position, null, ref vector2, 1f, ref vector3, out item);
			}
			this.mBuffs.Add(iBuff);
			this.mBuffDecals.Add(item);
			VisualEffectReference item2 = default(VisualEffectReference);
			if (iBuff.Effect != 0)
			{
				Vector3 position2 = this.Position;
				Vector3 direction = this.Direction;
				EffectManager.Instance.StartEffect(iBuff.Effect, ref position2, ref direction, out item2);
			}
			this.mBuffEffects.Add(item2);
		}

		// Token: 0x06000DBD RID: 3517 RVA: 0x00053768 File Offset: 0x00051968
		public void ClearAura()
		{
			for (int i = 0; i < this.mAuras.Count; i++)
			{
				ActiveAura value = this.mAuras[i];
				if (value.SelfCasted)
				{
					value.Aura.TTL = 0f;
					this.mAuras[i] = value;
				}
			}
		}

		// Token: 0x06000DBE RID: 3518 RVA: 0x000537C0 File Offset: 0x000519C0
		public void AddAura(ref AuraStorage iAura, bool iSelfCasted)
		{
			ActiveAura item = default(ActiveAura);
			if (iAura.AuraType == AuraType.Buff)
			{
				iAura.AuraBuff.Buff.SelfCasted = iSelfCasted;
			}
			item.Aura = iAura;
			item.SelfCasted = iSelfCasted;
			item.StartTTL = iAura.TTL;
			VisualEffectReference mEffect = default(VisualEffectReference);
			if (iAura.Effect != 0)
			{
				Vector3 position = this.Position;
				Vector3 direction = this.Direction;
				EffectManager.Instance.StartEffect(iAura.Effect, ref position, ref direction, out mEffect);
			}
			item.mEffect = mEffect;
			this.mAuras.Add(item);
		}

		// Token: 0x06000DBF RID: 3519 RVA: 0x0005385C File Offset: 0x00051A5C
		public BaseState ChangeState(BaseState iState)
		{
			if (iState == null || this.mCurrentState == iState)
			{
				return this.mCurrentState;
			}
			this.mCurrentState.OnExit(this);
			this.mPreviousState = this.mCurrentState;
			this.mCurrentState = iState;
			this.mCurrentState.OnEnter(this);
			return this.mPreviousState;
		}

		// Token: 0x06000DC0 RID: 3520 RVA: 0x000538AD File Offset: 0x00051AAD
		public void SetState(BaseState iState)
		{
			this.mPreviousState = null;
			this.mCurrentState = iState;
			this.mCurrentState.OnEnter(this);
		}

		// Token: 0x06000DC1 RID: 3521 RVA: 0x000538C9 File Offset: 0x00051AC9
		public void SetStateIgnoreEnterExit(BaseState iState)
		{
			this.mPreviousState = null;
			this.mCurrentState = iState;
		}

		// Token: 0x1700034B RID: 843
		// (get) Token: 0x06000DC2 RID: 3522 RVA: 0x000538D9 File Offset: 0x00051AD9
		public BaseState CurrentState
		{
			get
			{
				return this.mCurrentState;
			}
		}

		// Token: 0x1700034C RID: 844
		// (get) Token: 0x06000DC3 RID: 3523 RVA: 0x000538E1 File Offset: 0x00051AE1
		public BaseState PreviousState
		{
			get
			{
				return this.mPreviousState;
			}
		}

		// Token: 0x06000DC4 RID: 3524 RVA: 0x000538E9 File Offset: 0x00051AE9
		public virtual void Initialize(CharacterTemplate iTemplate, int iRandomOverride, Vector3 iPosition, int iUniqueID, float iDeadTimer)
		{
			this.Initialize(iTemplate, iRandomOverride, iPosition, iUniqueID);
			this.mDeadTimer = iDeadTimer;
			this.mDoNotRender = false;
		}

		// Token: 0x06000DC5 RID: 3525 RVA: 0x00053905 File Offset: 0x00051B05
		public virtual void Initialize(CharacterTemplate iTemplate, Vector3 iPosition, int iUniqueID, float iDeadTimer)
		{
			this.Initialize(iTemplate, iPosition, iUniqueID);
			this.mDeadTimer = iDeadTimer;
			this.mDoNotRender = false;
		}

		// Token: 0x06000DC6 RID: 3526 RVA: 0x0005391F File Offset: 0x00051B1F
		public virtual void Initialize(CharacterTemplate iTemplate, Vector3 iPosition, int iUniqueID)
		{
			this.Initialize(iTemplate, -1, iPosition, iUniqueID);
			this.mDoNotRender = false;
		}

		// Token: 0x06000DC7 RID: 3527 RVA: 0x00053934 File Offset: 0x00051B34
		public virtual void Initialize(CharacterTemplate iTemplate, int iRandomOverride, Vector3 iPosition, int iUniqueID)
		{
			base.Initialize(iUniqueID);
			this.mNotedKilledEvent = false;
			this.mTemplate = iTemplate;
			this.mShadowTimer = 0f;
			this.mDrowning = false;
			this.mDashing = false;
			this.mRegenRate = 0;
			this.mOnDamageTrigger = 0;
			this.mOnDeathTrigger = 0;
			this.mSpecialIdleAnimation = (this.mNetworkAnimation = Animations.None);
			this.mNetworkAnimationBlend = 0.2f;
			this.StopLevitate();
			this.mLevitationFreeFallTimer = 0f;
			this.mCharmTimer = 0f;
			this.mStunTimer = 0f;
			this.mCannotDieWithoutExplicitKill = false;
			this.mCollisionDamageGracePeriod = 1f;
			this.ApplyTemplate(iTemplate, ref iRandomOverride);
			this.mLastAccumulationDamager = null;
			this.mNormalizedHitPoints = 0f;
			this.mTimeSinceLastDamage = float.MaxValue;
			this.mTimeSinceLastStatusDamage = float.MaxValue;
			this.mLastDraw = 0.2f;
			this.mForceAnimationUpdate = false;
			this.mBoundingScale = 1.333f;
			this.mLastDraw = 0f;
			this.mZapTimer = 0f;
			this.mNormalizedHitPoints = 0f;
			this.mRegenAccumulation = 0f;
			this.mPanic = 0f;
			this.mRegenTimer = 1f;
			this.mBloatTimer = 0f;
			this.mBloating = false;
			this.mAttacking = false;
			this.mGripAttack = false;
			this.mBloatKilled = false;
			this.mOverkilled = false;
			this.mBlock = false;
			this.mIsHit = false;
			this.mKnockedDown = false;
			this.mFearTimer = 0f;
			this.mHypnotized = false;
			this.mForceCamera = false;
			this.mForceNavMesh = false;
			this.mDead = false;
			this.mWaterDepth = 0f;
			this.mDeathStatusEffects = StatusEffects.None;
			this.mCastType = CastType.None;
			for (int i = 0; i < this.mBloatDamageAccumulation.Length; i++)
			{
				this.mBloatDamageAccumulation[i] = 0f;
			}
			this.CharacterBody.Velocity = default(Vector3);
			this.CharacterBody.Movement = default(Vector3);
			this.mDeadTimer = 20f + 20f * (float)MagickaMath.Random.NextDouble();
			this.RemoveSelfShield();
			this.mUndying = this.mTemplate.Undying;
			if (this.mUndying)
			{
				this.mUndieTimer = this.mTemplate.UndieTime;
			}
			else
			{
				this.mUndieTimer = float.NaN;
			}
			if (this.mPlayState.Level != null && this.mPlayState.Level.CurrentScene != null)
			{
				Segment iSeg = default(Segment);
				iSeg.Origin = iPosition;
				iSeg.Origin.Y = iSeg.Origin.Y + 1f;
				iSeg.Delta.Y = -10f;
				float num;
				Vector3 vector;
				Vector3 vector2;
				AnimatedLevelPart animatedLevelPart;
				if (this.mPlayState.Level.CurrentScene.SegmentIntersect(out num, out vector, out vector2, out animatedLevelPart, iSeg))
				{
					iPosition = vector;
					iPosition.Y += 0.1f;
				}
			}
			else
			{
				iPosition.Y += (this.Body.CollisionSkin.GetPrimitiveLocal(0) as Capsule).Length * 0.5f + (this.Body.CollisionSkin.GetPrimitiveLocal(0) as Capsule).Radius;
			}
			for (int j = 0; j < this.mStatusEffects.Length; j++)
			{
				this.mStatusEffects[j].Stop();
				this.mStatusEffects[j] = default(StatusEffect);
			}
			this.mCurrentAnimation = Animations.idle;
			this.mCurrentAnimationSet = WeaponClass.Default;
			AnimationClipAction animationClipAction = this.mAnimationClips[(int)this.mCurrentAnimationSet][(int)this.mCurrentAnimation];
			this.mAnimationController.ClipSpeed = animationClipAction.AnimationSpeed;
			this.mAnimationController.ClearCrossfadeQueue();
			this.mAnimationController.StartClip(animationClipAction.Clip, animationClipAction.LoopAnimation);
			this.mCurrentActions = animationClipAction.Actions;
			this.mForceAnimationUpdate = false;
			for (int k = 0; k < this.mCurrentActions.Length; k++)
			{
				this.mExecutedActions.Add(false);
				this.mDeadActions.Add(false);
				if (this.mCurrentActions[k].UsesBones)
				{
					this.mForceAnimationUpdate = true;
				}
			}
			this.mNextAttackAnimation = Animations.None;
			this.mSpawnAnimation = Animations.None;
			this.mAttacking = false;
			ModelMesh modelMesh = this.mModel.Model.Meshes[0];
			this.mDefaultSpecular = (modelMesh.Effects[0] as SkinnedModelBasicEffect).SpecularPower;
			for (int l = 0; l < 3; l++)
			{
				this.mRenderData[l].SetMeshDirty();
				this.mNormalDistortionRenderData[l].SetMeshDirty();
				this.mShieldSkinRenderData[l].SetMeshDirty();
				this.mHighlightRenderData[l].SetMeshDirty();
				this.mLightningZapRenderData[l].SetMeshDirty();
			}
			iPosition.Y -= this.mHeightOffset;
			this.mBody.MoveTo(iPosition, Matrix.Identity);
			this.mBody.EnableBody();
			this.mVolume = this.Capsule.GetVolume();
			this.SetState(IdleState.Instance);
			this.mDoNotRender = false;
			for (int m = 0; m < this.mAttachedSounds.Length; m++)
			{
				this.mAttachedSoundCues[m] = AudioManager.Instance.PlayCue(this.mAttachedSounds[m].Value, this.mAttachedSounds[m].Key, this.mAudioEmitter);
			}
			for (int n = 0; n < iTemplate.Auras.Count; n++)
			{
				AuraStorage auraStorage = iTemplate.Auras[n];
				this.AddAura(ref auraStorage, false);
			}
			this.mTimeDead = 0f;
		}

		// Token: 0x06000DC8 RID: 3528 RVA: 0x00053EE4 File Offset: 0x000520E4
		protected virtual void ApplyTemplate(CharacterTemplate iTemplate, ref int iModel)
		{
			this.mType = iTemplate.ID;
			this.mName = iTemplate.Name;
			this.mDisplayName = iTemplate.DisplayName;
			this.mTemplateIsEthereal = iTemplate.IsEthereal;
			this.mEthereal = iTemplate.IsEthereal;
			this.mEtherealLook = iTemplate.LooksEthereal;
			this.mFearless = iTemplate.IsFearless;
			this.mUncharmable = iTemplate.IsUncharmable;
			this.mNonslippery = iTemplate.IsNonslippery;
			this.mHasFairy = iTemplate.HasFairy;
			this.mFaction = iTemplate.Faction;
			this.mBlood = iTemplate.Blood;
			this.mScoreValue = iTemplate.ScoreValue;
			this.mResistances = iTemplate.Resistances;
			this.mMaxPanic = iTemplate.MaxPanic;
			this.mZapModifier = iTemplate.ZapModifier;
			this.mMaxHitPoints = iTemplate.MaxHitpoints;
			this.mNumberOfHealtBars = iTemplate.NumberOfHealthBars;
			this.mHitPoints = this.mMaxHitPoints;
			this.mHitTolerance = (float)iTemplate.HitTolerance;
			this.mKnockdownTolerance = iTemplate.KnockdownTolerance;
			this.mRadius = iTemplate.Radius;
			this.mRegenRate = iTemplate.Regeneration;
			this.mBleedRate = iTemplate.BleedRate;
			this.mStunTime = iTemplate.StunTime;
			this.mEventConditions = iTemplate.EventConditions;
			this.mCanHasStatusEffect = !this.mName.Contains("elemental");
			this.mMoveAbilities = iTemplate.MoveAbilities;
			this.mMoveAnimations = iTemplate.MoveAnimations;
			this.mBreakFreeStrength = iTemplate.BreakFreeStrength;
			this.CharacterBody.MaxVelocity = iTemplate.Speed;
			this.mTurnSpeedMax = (this.mTurnSpeed = iTemplate.TurnSpeed);
			this.mSummonElementCue = iTemplate.SummonElementCue;
			this.mSummonElementBank = iTemplate.SummonElementBank;
			this.mGibs.Clear();
			for (int i = 0; i < iTemplate.Gibs.Length; i++)
			{
				this.mGibs.Add(iTemplate.Gibs[i]);
			}
			this.mAnimationClips = iTemplate.AnimationClips;
			if (iModel < 0)
			{
				iModel = MagickaMath.Random.Next(0, iTemplate.Models.Length);
			}
			else if (iModel >= iTemplate.Models.Length)
			{
				iModel = 0;
			}
			this.mModelID = iModel;
			this.mModel = iTemplate.Models[iModel].Model;
			this.mMouthJoint = iTemplate.MouthJoint;
			this.mHipJoint = iTemplate.HipJoint;
			this.mLeftHandJoint = iTemplate.LeftHandJoint;
			this.mLeftKneeJoint = iTemplate.LeftKneeJoint;
			this.mRightHandJoint = iTemplate.RightHandJoint;
			this.mRightKneeJoint = iTemplate.RightKneeJoint;
			this.mStaticTransform = iTemplate.Models[iModel].Transform;
			Matrix matrix;
			Matrix.Invert(ref this.mStaticTransform, out matrix);
			Matrix.Multiply(ref matrix, ref this.mMouthJoint.mBindPose, out this.mMouthJoint.mBindPose);
			Matrix.Multiply(ref matrix, ref this.mLeftHandJoint.mBindPose, out this.mLeftHandJoint.mBindPose);
			Matrix.Multiply(ref matrix, ref this.mRightHandJoint.mBindPose, out this.mRightHandJoint.mBindPose);
			this.mAnimationController.Skeleton = iTemplate.Skeleton;
			for (int j = 0; j < this.mEquipment.Length; j++)
			{
				iTemplate.Equipment[j].ReAttach(this);
				iTemplate.Equipment[j].CopyToInstance(this.mEquipment[j]);
				this.mEquipment[j].TransformBindPose(ref matrix);
				this.mEquipment[j].ReAttach(this);
			}
			this.mSourceOfSpellIndex = -1;
			for (int k = 0; k < this.mEquipment.Length; k++)
			{
				if (this.mEquipment[k].Item.WeaponClass == WeaponClass.Staff)
				{
					this.mSourceOfSpellIndex = k;
					break;
				}
			}
			Helper.SkinnedModelDeferredMaterialFromBasicEffect(this.mModel.Model.Meshes[0].MeshParts[0].Effect as SkinnedModelBasicEffect, out this.mMaterial);
			this.mMaterial.TintColor = iTemplate.Models[iModel].Tint;
			this.mResistances = iTemplate.Resistances;
			for (int l = 0; l < this.mAttachedEffectsBoneIndex.Length; l++)
			{
				this.mAttachedEffectsBoneIndex[l] = -1;
			}
			for (int m = 0; m < iTemplate.AttachedEffects.Length; m++)
			{
				this.AttachEffect(iTemplate.AttachedEffects[m].Value, iTemplate.AttachedEffects[m].Key);
			}
			for (int n = 0; n < iTemplate.AttachedSounds.Length; n++)
			{
				if (n >= this.mAttachedEffects.Length)
				{
					this.mAttachedSounds[n] = iTemplate.AttachedSounds[n];
				}
			}
			this.mPointLightHolder = iTemplate.PointLightHolder[0];
			this.mHeightOffset = -iTemplate.Radius - 0.5f * iTemplate.Length;
			(this.mCollision.GetPrimitiveLocal(0) as Capsule).Length = iTemplate.Length;
			(this.mCollision.GetPrimitiveNewWorld(0) as Capsule).Length = iTemplate.Length;
			(this.mCollision.GetPrimitiveOldWorld(0) as Capsule).Length = iTemplate.Length;
			(this.mCollision.GetPrimitiveLocal(0) as Capsule).Radius = iTemplate.Radius;
			(this.mCollision.GetPrimitiveNewWorld(0) as Capsule).Radius = iTemplate.Radius;
			(this.mCollision.GetPrimitiveOldWorld(0) as Capsule).Radius = iTemplate.Radius;
			Vector3 position = (this.mCollision.GetPrimitiveLocal(0) as Capsule).Position;
			position.Y = iTemplate.Length * -0.5f;
			(this.mCollision.GetPrimitiveLocal(0) as Capsule).Position = position;
			this.mBody.Mass = iTemplate.Mass;
			this.mTemplateMass = this.mBody.Mass;
			bool flag = (iTemplate.MoveAbilities & MovementProperties.Fly) == MovementProperties.Fly;
			this.CharacterBody.IsFlying = flag;
			this.mBody.ApplyGravity = !flag;
			this.mAttachedSounds = iTemplate.AttachedSounds;
		}

		// Token: 0x06000DC9 RID: 3529 RVA: 0x00054518 File Offset: 0x00052718
		public int AttachEffect(int iEffect, int iBoneIndex)
		{
			for (int i = 0; i < this.mAttachedEffects.Length; i++)
			{
				if (this.mAttachedEffectsBoneIndex[i] < 0)
				{
					Matrix inverseBindPoseTransform = this.mAnimationController.Skeleton[iBoneIndex].InverseBindPoseTransform;
					Matrix.Invert(ref inverseBindPoseTransform, out inverseBindPoseTransform);
					Matrix.Multiply(ref Character.sRotateY180, ref inverseBindPoseTransform, out this.mAttachedEffectsBindPose[i]);
					Matrix matrix;
					Matrix.Multiply(ref this.mAttachedEffectsBindPose[i], ref this.mAnimationController.SkinnedBoneTransforms[iBoneIndex], out matrix);
					VisualEffectReference visualEffectReference;
					EffectManager.Instance.StartEffect(iEffect, ref matrix, out visualEffectReference);
					this.mAttachedEffects[i] = visualEffectReference;
					this.mAttachedEffectsBoneIndex[i] = iBoneIndex;
					return i;
				}
			}
			return -1;
		}

		// Token: 0x06000DCA RID: 3530 RVA: 0x000545D6 File Offset: 0x000527D6
		public void StopAttachedEffect(int iIndex)
		{
			if (iIndex >= 0)
			{
				this.mAttachedEffectsBoneIndex[iIndex] = -1;
			}
		}

		// Token: 0x1700034D RID: 845
		// (get) Token: 0x06000DCB RID: 3531 RVA: 0x000545E5 File Offset: 0x000527E5
		// (set) Token: 0x06000DCC RID: 3532 RVA: 0x000545ED File Offset: 0x000527ED
		public Animations SpawnAnimation
		{
			get
			{
				return this.mSpawnAnimation;
			}
			set
			{
				this.mSpawnAnimation = value;
			}
		}

		// Token: 0x1700034E RID: 846
		// (get) Token: 0x06000DCD RID: 3533 RVA: 0x000545F6 File Offset: 0x000527F6
		// (set) Token: 0x06000DCE RID: 3534 RVA: 0x000545FE File Offset: 0x000527FE
		public Animations SpecialIdleAnimation
		{
			get
			{
				return this.mSpecialIdleAnimation;
			}
			set
			{
				this.mSpecialIdleAnimation = value;
			}
		}

		// Token: 0x1700034F RID: 847
		// (get) Token: 0x06000DCF RID: 3535 RVA: 0x00054607 File Offset: 0x00052807
		// (set) Token: 0x06000DD0 RID: 3536 RVA: 0x0005460F File Offset: 0x0005280F
		public Animations NetworkAnimation
		{
			get
			{
				return this.mNetworkAnimation;
			}
			set
			{
				this.mNetworkAnimation = value;
			}
		}

		// Token: 0x17000350 RID: 848
		// (get) Token: 0x06000DD1 RID: 3537 RVA: 0x00054618 File Offset: 0x00052818
		public float NetworkAnimationBlend
		{
			get
			{
				return this.mNetworkAnimationBlend;
			}
		}

		// Token: 0x17000351 RID: 849
		// (get) Token: 0x06000DD2 RID: 3538 RVA: 0x00054620 File Offset: 0x00052820
		public virtual bool IsImmortal
		{
			get
			{
				return this.mImmortalTime > 0f;
			}
		}

		// Token: 0x06000DD3 RID: 3539 RVA: 0x0005462F File Offset: 0x0005282F
		public void SetImmortalTime(float iTime)
		{
			this.mImmortalTime = iTime;
		}

		// Token: 0x17000352 RID: 850
		// (get) Token: 0x06000DD4 RID: 3540 RVA: 0x00054638 File Offset: 0x00052838
		// (set) Token: 0x06000DD5 RID: 3541 RVA: 0x00054640 File Offset: 0x00052840
		public float CollisionIgnoreTime
		{
			get
			{
				return this.mCollisionIgnoreTime;
			}
			set
			{
				this.mCollisionIgnoreTime = value;
			}
		}

		// Token: 0x06000DD6 RID: 3542 RVA: 0x00054649 File Offset: 0x00052849
		public void SetInvisible(float iTime)
		{
			this.mInvisibilityTimer = iTime;
			if (iTime > 0f)
			{
				this.RemoveSelfShield();
			}
		}

		// Token: 0x17000353 RID: 851
		// (get) Token: 0x06000DD7 RID: 3543 RVA: 0x00054660 File Offset: 0x00052860
		// (set) Token: 0x06000DD8 RID: 3544 RVA: 0x00054668 File Offset: 0x00052868
		public float WaterDepth
		{
			get
			{
				return this.mWaterDepth;
			}
			set
			{
				this.mWaterDepth = value;
			}
		}

		// Token: 0x06000DD9 RID: 3545 RVA: 0x00054674 File Offset: 0x00052874
		public virtual void Hypnotize(ref Vector3 iDirection, int iEffect)
		{
			iDirection.Y = 0f;
			this.CharacterBody.Movement = iDirection;
			this.mHypnotizeDirection = iDirection;
			this.mHypnotized = true;
			EffectManager.Instance.Stop(ref this.mHypnotizeEffect);
			this.mHypnotizeEffectID = iEffect;
			if (iEffect != 0)
			{
				Vector3 position = this.Position;
				Vector3 direction = this.Direction;
				EffectManager.Instance.StartEffect(iEffect, ref position, ref direction, out this.mHypnotizeEffect);
			}
		}

		// Token: 0x06000DDA RID: 3546 RVA: 0x000546EE File Offset: 0x000528EE
		public virtual void StopHypnotize()
		{
			this.mHypnotized = false;
			if (EffectManager.Instance.IsActive(ref this.mHypnotizeEffect))
			{
				EffectManager.Instance.Stop(ref this.mHypnotizeEffect);
			}
		}

		// Token: 0x06000DDB RID: 3547 RVA: 0x00054719 File Offset: 0x00052919
		public void StopLevitate()
		{
			this.mLevitateTimer = 0f;
			this.CharacterBody.ApplyGravity = true;
			if (EffectManager.Instance.IsActive(ref this.mLevitateEffect))
			{
				EffectManager.Instance.Stop(ref this.mLevitateEffect);
			}
		}

		// Token: 0x06000DDC RID: 3548 RVA: 0x00054754 File Offset: 0x00052954
		public void SetLevitate(float iTTL, int iEffect)
		{
			this.mLevitateTimer = iTTL;
			if (iTTL > 0f)
			{
				if (EffectManager.Instance.IsActive(ref this.mLevitateEffect))
				{
					EffectManager.Instance.Stop(ref this.mLevitateEffect);
				}
				this.CharacterBody.ApplyGravity = false;
				Vector3 position = this.Position;
				Vector3 direction = this.Direction;
				EffectManager.Instance.StartEffect(iEffect, ref position, ref direction, out this.mLevitateEffect);
				return;
			}
			this.StopLevitate();
		}

		// Token: 0x17000354 RID: 852
		// (get) Token: 0x06000DDD RID: 3549 RVA: 0x000547C9 File Offset: 0x000529C9
		public bool IsHypnotized
		{
			get
			{
				return this.mHypnotized;
			}
		}

		// Token: 0x17000355 RID: 853
		// (get) Token: 0x06000DDE RID: 3550 RVA: 0x000547D1 File Offset: 0x000529D1
		public bool IsInvisibile
		{
			get
			{
				return this.mInvisibilityTimer > 0f;
			}
		}

		// Token: 0x06000DDF RID: 3551 RVA: 0x000547E0 File Offset: 0x000529E0
		public void RemoveInvisibility()
		{
			this.mInvisibilityTimer = 0f;
		}

		// Token: 0x17000356 RID: 854
		// (get) Token: 0x06000DE0 RID: 3552 RVA: 0x000547ED File Offset: 0x000529ED
		public List<GibReference> Gibs
		{
			get
			{
				return this.mGibs;
			}
		}

		// Token: 0x17000357 RID: 855
		// (get) Token: 0x06000DE1 RID: 3553 RVA: 0x000547F5 File Offset: 0x000529F5
		public float ZapTimer
		{
			get
			{
				return this.mZapTimer;
			}
		}

		// Token: 0x17000358 RID: 856
		// (get) Token: 0x06000DE2 RID: 3554 RVA: 0x000547FD File Offset: 0x000529FD
		public float ZapModifier
		{
			get
			{
				return this.mZapModifier;
			}
		}

		// Token: 0x17000359 RID: 857
		// (get) Token: 0x06000DE3 RID: 3555 RVA: 0x00054805 File Offset: 0x00052A05
		public int ScoreValue
		{
			get
			{
				return this.mScoreValue;
			}
		}

		// Token: 0x1700035A RID: 858
		// (get) Token: 0x06000DE4 RID: 3556 RVA: 0x0005480D File Offset: 0x00052A0D
		// (set) Token: 0x06000DE5 RID: 3557 RVA: 0x00054815 File Offset: 0x00052A15
		public bool DoNotRender
		{
			get
			{
				return this.mDoNotRender;
			}
			set
			{
				this.mDoNotRender = value;
			}
		}

		// Token: 0x06000DE6 RID: 3558 RVA: 0x0005481E File Offset: 0x00052A1E
		internal void ResetRestingTimers()
		{
			this.mRestingMovementTimer = 1f;
			this.mRestingHealthTimer = 1f;
		}

		// Token: 0x1700035B RID: 859
		// (get) Token: 0x06000DE7 RID: 3559 RVA: 0x00054836 File Offset: 0x00052A36
		protected bool RestingMovement
		{
			get
			{
				return this.mRestingMovementTimer < 0f;
			}
		}

		// Token: 0x1700035C RID: 860
		// (get) Token: 0x06000DE8 RID: 3560 RVA: 0x00054845 File Offset: 0x00052A45
		protected bool RestingHealth
		{
			get
			{
				return this.mRestingHealthTimer < 0f;
			}
		}

		// Token: 0x1700035D RID: 861
		// (get) Token: 0x06000DE9 RID: 3561 RVA: 0x00054854 File Offset: 0x00052A54
		public float BleedRate
		{
			get
			{
				return this.mBleedRate;
			}
		}

		// Token: 0x06000DEA RID: 3562 RVA: 0x0005485C File Offset: 0x00052A5C
		public void Stun(float time)
		{
			this.mStunTimer = time;
		}

		// Token: 0x06000DEB RID: 3563 RVA: 0x00054865 File Offset: 0x00052A65
		public void Unstun()
		{
			this.mStunTimer = 0.001f;
		}

		// Token: 0x1700035E RID: 862
		// (get) Token: 0x06000DEC RID: 3564 RVA: 0x00054872 File Offset: 0x00052A72
		public bool IsStunned
		{
			get
			{
				return this.mStunTimer > 0f && !this.Dead;
			}
		}

		// Token: 0x1700035F RID: 863
		// (get) Token: 0x06000DED RID: 3565 RVA: 0x0005488C File Offset: 0x00052A8C
		public bool IsLevitating
		{
			get
			{
				return this.mLevitateTimer > 0f && !this.Dead;
			}
		}

		// Token: 0x17000360 RID: 864
		// (get) Token: 0x06000DEE RID: 3566 RVA: 0x000548A6 File Offset: 0x00052AA6
		// (set) Token: 0x06000DEF RID: 3567 RVA: 0x000548AE File Offset: 0x00052AAE
		public bool Undying
		{
			get
			{
				return this.mUndying;
			}
			set
			{
				this.mUndying = value;
			}
		}

		// Token: 0x17000361 RID: 865
		// (get) Token: 0x06000DF0 RID: 3568 RVA: 0x000548B7 File Offset: 0x00052AB7
		public float UndyingTimer
		{
			get
			{
				return this.mUndieTimer;
			}
		}

		// Token: 0x17000362 RID: 866
		// (get) Token: 0x06000DF1 RID: 3569 RVA: 0x000548BF File Offset: 0x00052ABF
		// (set) Token: 0x06000DF2 RID: 3570 RVA: 0x000548C7 File Offset: 0x00052AC7
		public bool JustCastInvisible
		{
			get
			{
				return this.mJustCastInvisible;
			}
			set
			{
				this.mJustCastInvisible = value;
			}
		}

		// Token: 0x17000363 RID: 867
		// (get) Token: 0x06000DF3 RID: 3571 RVA: 0x000548D0 File Offset: 0x00052AD0
		public Entity LastAttacker
		{
			get
			{
				return this.mLastAttacker;
			}
		}

		// Token: 0x17000364 RID: 868
		// (get) Token: 0x06000DF4 RID: 3572 RVA: 0x000548D8 File Offset: 0x00052AD8
		public Elements LastDamageElement
		{
			get
			{
				return this.mLastDamageElement;
			}
		}

		// Token: 0x06000DF5 RID: 3573 RVA: 0x000548E0 File Offset: 0x00052AE0
		internal void KillAnimationActions()
		{
			for (int i = 0; i < this.mCurrentActions.Length; i++)
			{
				if (!this.mDeadActions[i])
				{
					this.mCurrentActions[i].Kill(this);
				}
			}
		}

		// Token: 0x06000DF6 RID: 3574 RVA: 0x0005491C File Offset: 0x00052B1C
		public override void Deinitialize()
		{
			if (this.mCurrentSpell != null)
			{
				this.mCurrentSpell.DeInitialize(this);
			}
			this.mHitList.Clear();
			if (this.mLastDamageIndex >= 0)
			{
				DamageNotifyer.Instance.ReleasNumber(this.mLastDamageIndex);
			}
			this.mLastDamageIndex = -1;
			EffectManager.Instance.Stop(ref this.mStunEffect);
			EffectManager.Instance.Stop(ref this.mFearEffect);
			this.EndCharm();
			this.StopHypnotize();
			this.StopLevitate();
			this.mCurrentSpell = null;
			if (this.ChargeCue != null)
			{
				this.ChargeCue.Stop(AudioStopOptions.Immediate);
			}
			this.ReleaseAttachedCharacter();
			for (int i = 0; i < this.mEquipment.Length; i++)
			{
				if (this.mEquipment[i].Item != null && this.mEquipment[i].Item.Attached)
				{
					this.mEquipment[i].Item.Deinitialize();
				}
			}
			for (int j = 0; j < this.mStatusEffectCues.Length; j++)
			{
				if (this.mStatusEffectCues[j] != null)
				{
					this.mStatusEffectCues[j].Stop(AudioStopOptions.AsAuthored);
				}
			}
			this.mDialog = 0;
			for (int k = 0; k < this.mAttachedSoundCues.Length; k++)
			{
				if (this.mAttachedSoundCues[k] != null && !this.mAttachedSoundCues[k].IsStopping)
				{
					this.mAttachedSoundCues[k].Stop(AudioStopOptions.AsAuthored);
				}
			}
			for (int l = 0; l < this.mAttachedEffects.Length; l++)
			{
				if (this.mAttachedEffects[l].Hash != 0)
				{
					EffectManager.Instance.Stop(ref this.mAttachedEffects[l]);
				}
			}
			if (this.mPointLight != null)
			{
				this.mPointLight.Disable();
				this.mPointLight = null;
				this.mPointLightHolder.Enabled = false;
			}
			if (this.mStatusEffectLight != null)
			{
				this.mStatusEffectLight.Stop(false);
				this.mStatusEffectLight = null;
			}
			this.mBloatTimer = 0f;
			for (int m = 0; m < this.mAuras.Count; m++)
			{
				VisualEffectReference mEffect = this.mAuras[m].mEffect;
				EffectManager.Instance.Stop(ref mEffect);
			}
			this.mAuras.Clear();
			this.mBuffs.Clear();
			for (int n = 0; n < this.mBuffEffects.Count; n++)
			{
				VisualEffectReference visualEffectReference = this.mBuffEffects[n];
				EffectManager.Instance.Stop(ref visualEffectReference);
			}
			this.mBuffEffects.Clear();
			this.mBuffDecals.Clear();
			for (int num = 0; num < this.mCurrentActions.Length; num++)
			{
				if (!this.mDeadActions[num])
				{
					this.mCurrentActions[num].Kill(this);
				}
			}
			base.Deinitialize();
		}

		// Token: 0x17000365 RID: 869
		// (get) Token: 0x06000DF7 RID: 3575 RVA: 0x00054BCD File Offset: 0x00052DCD
		internal bool IgnoreCollisionDamage
		{
			get
			{
				return this.mCollisionDamageGracePeriod > 0f;
			}
		}

		// Token: 0x06000DF8 RID: 3576 RVA: 0x00054BDC File Offset: 0x00052DDC
		protected virtual void UpdateAnimationController(float iDeltaTime, ref Matrix iOrientation, bool iUpdateTransforms)
		{
			this.mAnimationController.Update(iDeltaTime, ref iOrientation, iUpdateTransforms);
		}

		// Token: 0x06000DF9 RID: 3577 RVA: 0x00054BEC File Offset: 0x00052DEC
		public override void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			this.mCollidedWithCamera = false;
			Matrix orientation = this.GetOrientation();
			this.mCollision.GetPrimitiveOldWorld(0);
			this.mHaloBuffColor = default(Vector4);
			for (int i = 0; i < this.mAuras.Count; i++)
			{
				ActiveAura value = this.mAuras[i];
				value.Execute(this, iDeltaTime);
				if (value.Aura.TTL <= 0f)
				{
					if (this.mAuraCycleTimer > (float)i)
					{
						this.mAuraCycleTimer -= 1f;
					}
					VisualEffectReference mEffect = this.mAuras[i].mEffect;
					EffectManager.Instance.Stop(ref mEffect);
					this.mAuras.RemoveAt(i);
					i--;
				}
				else
				{
					float amount = Math.Min(Math.Abs((float)i - this.mAuraCycleTimer), Math.Abs((float)i - (this.mAuraCycleTimer - (float)this.mAuras.Count)));
					float scaleFactor = MathHelper.SmoothStep(1f, 0f, amount);
					this.mHaloBuffFade = 1f;
					Vector3 color = this.mAuras[i].Aura.Color;
					Vector3.Multiply(ref color, scaleFactor, out color);
					Vector3 vector = new Vector3(this.mHaloBuffColor.X, this.mHaloBuffColor.Y, this.mHaloBuffColor.Z);
					Vector3.Add(ref color, ref vector, out color);
					this.mHaloBuffColor = new Vector4(color, 1f);
					Vector3 position = this.Position;
					Vector3 direction = this.Direction;
					VisualEffectReference mEffect2 = this.mAuras[i].mEffect;
					EffectManager.Instance.UpdatePositionDirection(ref mEffect2, ref position, ref direction);
					this.mAuras[i] = value;
					if (value.SelfCasted)
					{
						position.Y += this.HeightOffset;
						Vector2 value2 = new Vector2(0f, (float)(6 * this.mNumberOfHealtBars));
						Healthbars.Instance.AddHealthBar(position, this.mAuras[i].Aura.TTL / this.mAuras[i].StartTTL, this.mRadius, 1f, float.MaxValue, false, new Vector4?(new Vector4(Spell.SHIELDCOLOR, 1f)), new Vector2?(value2));
					}
				}
			}
			if (this.mAuras.Count > 1)
			{
				this.mAuraCycleTimer = (this.mAuraCycleTimer + iDeltaTime) % (float)this.mAuras.Count;
			}
			else
			{
				this.mAuraCycleTimer = Math.Max(this.mAuraCycleTimer - iDeltaTime, 0f);
			}
			this.mCollisionDamageGracePeriod = MathHelper.Max(this.mCollisionDamageGracePeriod - iDeltaTime, 0f);
			if (this.mBody.Velocity.LengthSquared() > 1E-06f)
			{
				this.mRestingMovementTimer = 1f;
			}
			else
			{
				this.mRestingMovementTimer -= iDeltaTime;
			}
			if (this.mLastHitpoints != this.mHitPoints || this.mCurrentStatusEffects != StatusEffects.None)
			{
				this.mRestingHealthTimer = 1f;
			}
			else
			{
				this.mRestingHealthTimer -= iDeltaTime;
			}
			if (this.mLevitateTimer > 0f)
			{
				this.mLevitateTimer -= iDeltaTime;
				Segment iSeg = default(Segment);
				iSeg.Origin = this.Position;
				iSeg.Origin.Y = iSeg.Origin.Y + this.HeightOffset * 0.5f;
				iSeg.Delta.Y = -0.75f + this.HeightOffset * 0.5f;
				Vector3 vector2 = default(Vector3);
				float num;
				Vector3 vector3;
				Vector3 vector4;
				AnimatedLevelPart animatedLevelPart;
				int num2;
				if (this.mPlayState.Level.CurrentScene.LiquidSegmentIntersect(out num, out vector3, out vector4, ref iSeg, false, false, false) || this.mPlayState.Level.CurrentScene.SegmentIntersect(out num, out vector3, out vector4, out animatedLevelPart, out num2, iSeg))
				{
					vector2.Y = (1f - num) * 0.75f;
					this.mLevitationFreeFallTimer = 0f;
				}
				else
				{
					vector2.Y = -iDeltaTime * (0.5f + 1.5f * this.mLevitationFreeFallTimer);
					this.mLevitationFreeFallTimer = Math.Min(this.mLevitationFreeFallTimer + iDeltaTime * 0.5f, 1f);
				}
				Vector3 velocity = this.mBody.Velocity;
				Vector3.Add(ref velocity, ref vector2, out velocity);
				this.mBody.Velocity = velocity;
				if (this.mLevitateTimer <= 0f)
				{
					this.StopLevitate();
				}
				else
				{
					Vector3 position2 = this.Position;
					position2.Y += this.HeightOffset;
					Vector3 direction2 = this.Direction;
					if (!EffectManager.Instance.IsActive(ref this.mLevitateEffect))
					{
						EffectManager.Instance.StartEffect(Levitate.MAGICK_EFFECT, ref position2, ref direction2, out this.mLevitateEffect);
					}
					else
					{
						EffectManager.Instance.UpdatePositionDirection(ref this.mLevitateEffect, ref position2, ref direction2);
					}
				}
			}
			this.mHitList.Update(iDeltaTime);
			this.mLastDraw += iDeltaTime;
			this.mUndying = this.mTemplate.Undying;
			this.mMaxHitPoints = this.mTemplate.MaxHitpoints;
			this.mBuffRotation = MathHelper.WrapAngle(this.mBuffRotation + iDeltaTime * -0.2f);
			Matrix matrix;
			Matrix.CreateFromYawPitchRoll(0f, -1.5707964f, this.mBuffRotation, out matrix);
			matrix.M11 *= this.mRadius * 3f;
			matrix.M12 *= this.mRadius * 3f;
			matrix.M13 *= this.mRadius * 3f;
			matrix.M21 *= this.mRadius * 3f;
			matrix.M22 *= this.mRadius * 3f;
			matrix.M23 *= this.mRadius * 3f;
			matrix.M31 *= this.mRadius * 1f;
			matrix.M32 *= this.mRadius * 1f;
			matrix.M33 *= this.mRadius * 1f;
			Vector3 pos = this.Position;
			matrix.M41 = pos.X;
			matrix.M42 = pos.Y + this.mHeightOffset;
			matrix.M43 = pos.Z;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			for (int j = 0; j < this.mBuffs.Count; j++)
			{
				switch (this.mBuffs[j].VisualCategory)
				{
				case VisualCategory.Offensive:
					num3++;
					break;
				case VisualCategory.Defensive:
					num4++;
					break;
				case VisualCategory.Special:
					num5++;
					break;
				}
			}
			int num6 = 0;
			int num7 = 0;
			int num8 = 0;
			for (int k = 0; k < this.mBuffs.Count; k++)
			{
				DecalManager.DecalReference value3 = this.mBuffDecals[k];
				BuffStorage value4 = this.mBuffs[k];
				float num9 = 0f;
				float num10 = 0f;
				float num11 = 0f;
				switch (value4.VisualCategory)
				{
				case VisualCategory.Offensive:
					num9 = this.mBuffCycleTimerOffensive;
					num11 = (float)num3;
					num10 = (float)num6;
					num6++;
					break;
				case VisualCategory.Defensive:
					num9 = this.mBuffCycleTimerDefensive;
					num11 = (float)num4;
					num10 = (float)num7;
					num7++;
					break;
				case VisualCategory.Special:
					num9 = this.mBuffCycleTimerSpecial;
					num11 = (float)num5;
					num10 = (float)num8;
					num8++;
					break;
				}
				float amount2 = Math.Min(Math.Abs(num10 - num9), Math.Abs(num10 - (num9 - num11)));
				float num12 = MathHelper.SmoothStep(1f, 0f, amount2);
				if (value3.Index < 0)
				{
					if (value4.VisualCategory != VisualCategory.None)
					{
						Vector2 vector5 = default(Vector2);
						vector5.X = (vector5.Y = this.mRadius * 1.5f);
						Vector3 vector6 = default(Vector3);
						vector6.Y = 1f;
						Vector4 vector7 = default(Vector4);
						vector7.X = value4.Color.X;
						vector7.Y = value4.Color.Y;
						vector7.Z = value4.Color.Z;
						DecalManager.Instance.AddAlphaBlendedDecal(Decal.BuffOffensive + (int)value4.VisualCategory - 1, null, ref vector5, ref pos, null, ref vector6, 1f, ref vector7, out value3);
					}
				}
				else if (!this.mPlayState.IsInCutscene)
				{
					DecalManager.Instance.SetDecal(ref value3, 1f, ref matrix, num12 * 0.8f);
				}
				else
				{
					DecalManager.Instance.SetDecal(ref value3, 1f, ref matrix, 0f);
				}
				this.mBuffDecals[k] = value3;
				if (this.mBuffs[k].TTL > 0f)
				{
					value4.Execute(this, iDeltaTime);
					this.mBuffs[k] = value4;
					if (this.mBuffs[k].Effect != 0)
					{
						Vector3 position3 = this.Position;
						Vector3 direction3 = this.Direction;
						VisualEffectReference visualEffectReference = this.mBuffEffects[k];
						EffectManager.Instance.UpdatePositionDirection(ref visualEffectReference, ref position3, ref direction3);
					}
				}
				else
				{
					this.mBuffs.RemoveAt(k);
					VisualEffectReference visualEffectReference2 = this.mBuffEffects[k];
					EffectManager.Instance.Stop(ref visualEffectReference2);
					this.mBuffEffects.RemoveAt(k);
					switch (value4.VisualCategory)
					{
					case VisualCategory.Offensive:
						if (this.mBuffCycleTimerOffensive > (float)k)
						{
							this.mBuffCycleTimerOffensive -= 1f;
						}
						break;
					case VisualCategory.Defensive:
						if (this.mBuffCycleTimerDefensive > (float)k)
						{
							this.mBuffCycleTimerDefensive -= 1f;
						}
						break;
					case VisualCategory.Special:
						if (this.mBuffCycleTimerSpecial > (float)k)
						{
							this.mBuffCycleTimerSpecial -= 1f;
						}
						break;
					}
					this.mBuffDecals.RemoveAt(k);
					k--;
				}
			}
			if (num3 > 1)
			{
				this.mBuffCycleTimerOffensive = (this.mBuffCycleTimerOffensive + iDeltaTime) % (float)num3;
			}
			else
			{
				this.mBuffCycleTimerOffensive = Math.Max(this.mBuffCycleTimerOffensive - iDeltaTime, 0f);
			}
			if (num4 > 1)
			{
				this.mBuffCycleTimerDefensive = (this.mBuffCycleTimerDefensive + iDeltaTime) % (float)num4;
			}
			else
			{
				this.mBuffCycleTimerDefensive = Math.Max(this.mBuffCycleTimerDefensive - iDeltaTime, 0f);
			}
			if (num5 > 1)
			{
				this.mBuffCycleTimerSpecial = (this.mBuffCycleTimerSpecial + iDeltaTime) % (float)num5;
			}
			else
			{
				this.mBuffCycleTimerSpecial = Math.Max(this.mBuffCycleTimerSpecial - iDeltaTime, 0f);
			}
			if (this.mHitPoints > this.mMaxHitPoints)
			{
				this.mHitPoints = this.mMaxHitPoints;
			}
			for (int l = 0; l < this.mBloatDamageAccumulation.Length; l++)
			{
				this.mBloatDamageAccumulation[l] = Math.Max(0f, this.mBloatDamageAccumulation[l] - iDeltaTime * 0.75f);
			}
			this.mImmortalTime -= iDeltaTime;
			this.mCollisionIgnoreTime -= iDeltaTime;
			this.mNormalizedHitPoints = MathHelper.Clamp(MathHelper.Lerp(this.mHitPoints / this.mMaxHitPoints, this.mNormalizedHitPoints, (float)Math.Pow(0.002, (double)iDeltaTime)), 0f, 1f);
			this.BlockItem = -1;
			bool flag = this.mLastDraw < 0.2f | this.mForceAnimationUpdate | this.IsInvisibile;
			if (!this.Dead | this.mDeadTimer > -10f | !this.mAnimationController.HasFinished | this.mAnimationController.CrossFadeEnabled)
			{
				this.UpdateAnimationController(iDeltaTime, ref orientation, flag);
				if (flag)
				{
					for (int m = 0; m < this.mAttachedEffects.Length; m++)
					{
						if (this.mAttachedEffectsBoneIndex[m] >= 0)
						{
							Matrix boneOrientation = this.GetBoneOrientation(this.mAttachedEffectsBoneIndex[m], ref this.mAttachedEffectsBindPose[m]);
							EffectManager.Instance.UpdateOrientation(ref this.mAttachedEffects[m], ref boneOrientation);
							if (this.mAttachedEffects[m].ID < 0)
							{
								this.mAttachedEffectsBoneIndex[m] = -1;
							}
						}
					}
				}
				for (int n = 0; n < this.mCurrentActions.Length; n++)
				{
					AnimationAction animationAction = this.mCurrentActions[n];
					bool value5 = this.mExecutedActions[n];
					bool value6 = this.mDeadActions[n];
					animationAction.Execute(this, this.mAnimationController, ref value5, ref value6);
					this.mExecutedActions[n] = value5;
					this.mDeadActions[n] = value6;
				}
			}
			if (!this.Dead)
			{
				if (this.mDrowning || !this.CharacterBody.IsInWater)
				{
					EffectManager.Instance.Stop(ref this.mWaterSplashEffect);
				}
				else if (this.WaterDepth >= Defines.FOOTSTEP_WATER_OFFSET)
				{
					Matrix identity = Matrix.Identity;
					Vector3 position4 = this.Position;
					position4.Y += this.HeightOffset + this.WaterDepth;
					identity.Translation = position4;
					if (EffectManager.Instance.IsActive(ref this.mWaterSplashEffect))
					{
						EffectManager.Instance.UpdateOrientation(ref this.mWaterSplashEffect, ref identity);
					}
					else if (this.CharacterBody.GroundMaterial == CollisionMaterials.Lava)
					{
						EffectManager.Instance.StartEffect(Defines.LAVA_SPLASH_EFFECT, ref identity, out this.mWaterSplashEffect);
					}
					else
					{
						EffectManager.Instance.StartEffect(Defines.WATER_SPLASH_EFFECT, ref identity, out this.mWaterSplashEffect);
					}
				}
				else
				{
					EffectManager.Instance.Stop(ref this.mWaterSplashEffect);
				}
			}
			if (this.mEtherealTimer > 0f)
			{
				this.mEtherealTimer -= iDeltaTime;
				if (this.mEtherealTimer <= 0f)
				{
					this.mEthereal = this.mEtherealTimedState;
					this.mEtherealAlpha = 1f;
					this.Ethereal(this.mEtherealTimedState, 1f, 1f);
				}
			}
			this.mEtherealAlpha += (this.mEtherealAlphaTarget - this.mEtherealAlpha) * this.mEtherealAlphaSpeed * iDeltaTime;
			this.mAnimationController.Speed = 1f;
			this.CharacterBody.SpeedMultiplier = 1f;
			if (this.mSelfShield.Active)
			{
				this.mSelfShield.Update(this, iDeltaTime);
			}
			this.mMeleeBoostTimer -= iDeltaTime;
			if (this.mMeleeBoostTimer <= 0f)
			{
				this.mMeleeBoostAmount = 0f;
			}
			this.ChangeState(this.mCurrentState.Update(this, iDeltaTime));
			if (this.mZapTimer >= 0f)
			{
				if (!float.IsNaN(this.mZapModifier))
				{
					this.CharacterBody.SpeedMultiplier = this.mZapModifier;
					this.mAnimationController.Speed *= this.mZapModifier;
				}
				this.mZapTimer -= iDeltaTime;
			}
			if (this.mGrippedCharacter != null)
			{
				if (this.mHitPoints < 0f || this.mGrippedCharacter.mDead || this.ShouldReleaseGrip)
				{
					if (this.ShouldReleaseGrip && this.mGrippedCharacter is Avatar && !((this.mGrippedCharacter as Avatar).Player.Gamer is NetworkGamer))
					{
						AchievementsManager.Instance.AwardAchievement(base.PlayState, "houdini");
					}
					this.ReleaseAttachedCharacter();
				}
				else
				{
					switch (this.mGripType)
					{
					case Grip.GripType.Pickup:
					{
						Matrix orientation2;
						Matrix.Multiply(ref this.mGripJoint.mBindPose, ref this.mAnimationController.SkinnedBoneTransforms[this.mGripJoint.mIndex], out orientation2);
						pos = orientation2.Translation;
						orientation2.M41 = (orientation2.M42 = (orientation2.M43 = 0f));
						this.mGrippedCharacter.Body.MoveTo(pos, orientation2);
						break;
					}
					case Grip.GripType.Hold:
					{
						this.mGrippedCharacter.mGripper = this;
						this.mGrippedCharacter.CharacterBody.SpeedMultiplier = 0f;
						this.mGrippedCharacter.CharacterBody.AllowRotate = false;
						Vector3 position5 = this.mGrippedCharacter.Position;
						Vector3 position6 = this.Position;
						Vector3 forward;
						Vector3.Subtract(ref position5, ref position6, out forward);
						if (forward.LengthSquared() < 1E-06f)
						{
							forward = Vector3.Forward;
						}
						else
						{
							forward.Normalize();
						}
						pos = this.mGrippedCharacter.Position;
						Vector3 vector8;
						Vector3.Multiply(ref forward, -this.mRadius - this.mGrippedCharacter.Radius, out vector8);
						Vector3.Add(ref pos, ref vector8, out pos);
						Vector3 vector9 = default(Vector3);
						vector9.Y = 1f;
						Matrix orientation3;
						Matrix.CreateWorld(ref pos, ref forward, ref vector9, out orientation3);
						orientation3.M41 = (orientation3.M42 = (orientation3.M43 = 0f));
						this.Body.MoveTo(pos, orientation3);
						this.mGrippedCharacter.CharacterBody.AllowRotate = false;
						this.mGrippedCharacter.CharacterBody.AllowMove = false;
						break;
					}
					}
				}
			}
			if (this.mGripper != null)
			{
				if (this.mDead | this.mGripper.mDead)
				{
					this.mGripper.ReleaseAttachedCharacter();
					this.mGripper = null;
				}
				else
				{
					switch (this.mGripType)
					{
					case Grip.GripType.Pickup:
					case Grip.GripType.Hold:
						this.mGripper.CharacterBody.SpeedMultiplier = 0f;
						break;
					case Grip.GripType.Ride:
					{
						Matrix orientation4 = this.mGripper.Body.Orientation;
						orientation4.Translation = this.mGripper.Body.Position;
						Matrix orientation5;
						Matrix.Multiply(ref this.mGripper.mGripJoint.mBindPose, ref this.mAnimationController.SkinnedBoneTransforms[this.mGripJoint.mIndex], out orientation5);
						Character.sRotateSpines = Matrix.CreateFromYawPitchRoll(1.5707964f, 1.5707964f, 0f);
						Matrix.Multiply(ref Character.sRotateSpines, ref orientation5, out orientation5);
						Vector3 up = orientation5.Up;
						Vector3.Multiply(ref up, -this.mGripper.mHeightOffset, out up);
						pos = orientation5.Translation;
						Vector3.Add(ref pos, ref up, out pos);
						Matrix.Lerp(ref orientation5, ref orientation4, 0.5f, out orientation5);
						orientation5.M41 = (orientation5.M42 = (orientation5.M43 = 0f));
						this.mGripper.Body.MoveTo(pos, orientation5);
						break;
					}
					}
				}
			}
			Vector3 vector10 = this.Position;
			Vector3 direction4 = this.Direction;
			if (this.IsFeared)
			{
				this.mFearTimer -= iDeltaTime;
				if (!EffectManager.Instance.UpdatePositionDirection(ref this.mFearEffect, ref vector10, ref direction4))
				{
					EffectManager.Instance.StartEffect(Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Fear.FEARED_EFFECT, ref vector10, ref direction4, out this.mFearEffect);
				}
				if (!this.IsFeared)
				{
					this.mFearTimer = 0f;
					EffectManager.Instance.Stop(ref this.mFearEffect);
				}
			}
			if (this.IsStunned)
			{
				this.mStunTimer -= iDeltaTime;
				if (this.mStunTimer <= 0f)
				{
					EffectManager.Instance.Stop(ref this.mStunEffect);
				}
				else
				{
					Matrix mouthAttachOrientation = this.GetMouthAttachOrientation();
					if (!EffectManager.Instance.UpdateOrientation(ref this.mStunEffect, ref mouthAttachOrientation))
					{
						EffectManager.Instance.StartEffect(Character.STUN_EFFECT, ref mouthAttachOrientation, out this.mStunEffect);
					}
				}
			}
			if (this.mCurrentSpell != null)
			{
				float num13;
				if (!this.mCurrentSpell.CastUpdate(iDeltaTime, this, out num13) && this.mCurrentSpell.CastType == CastType.Weapon && !this.mCurrentSpell.Active && !(this.mCurrentState is CastState))
				{
					this.mCurrentSpell = null;
				}
				this.mTurnSpeed = this.mTurnSpeedMax * num13;
			}
			else if (!(this.mCurrentState is ChargeState))
			{
				this.mTurnSpeed = this.mTurnSpeedMax;
			}
			if (this.mBloating)
			{
				this.mBloatTimer += iDeltaTime;
				if (this.mBloatTimer >= 0.333f)
				{
					this.BloatBlast();
				}
			}
			if (this.mDead)
			{
				if (!float.IsNaN(this.mUndieTimer) && !this.mOverkilled)
				{
					this.mUndieTimer -= iDeltaTime;
					if (this.mUndieTimer <= 0f)
					{
						this.mDead = false;
						this.mHitPoints = this.mTemplate.UndieHitPoints;
					}
				}
				else
				{
					this.mDeadTimer -= iDeltaTime;
					this.mShadowTimer += iDeltaTime;
					if (this.mDeadTimer < 0f && this.mRemoveAfterDeath)
					{
						this.CharacterBody.Position = new Vector3(this.Position.X, this.Position.Y - iDeltaTime * 0.3f, this.Position.Z);
					}
				}
			}
			else
			{
				for (int num14 = 0; num14 < this.mEquipment.Length; num14++)
				{
					Item item = this.mEquipment[num14].Item;
					if (item.Attached && !item.AnimationDetached)
					{
						Matrix itemAttachOrientation = this.GetItemAttachOrientation(num14);
						item.Transform = itemAttachOrientation;
						item.IsInvisible = this.IsInvisibile;
						item.Update(iDataChannel, iDeltaTime);
						if (num14 == this.mSourceOfSpellIndex)
						{
							this.mStaffOrb = item.AttachAbsoluteTransform;
						}
						else if (num14 == 0)
						{
							this.mWeaponTransform = item.AttachAbsoluteTransform;
						}
					}
				}
				if (this.mPointLightHolder.ContainsLight && !this.mPointLightHolder.Enabled)
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
					this.mPointLight.Position = this.mPointLightHolder.Joint.mBindPose.Translation;
					this.mPointLight.Speed = 1f;
					this.mPointLight.Intensity = 1f;
					this.mPointLight.Enable(this.mPlayState.Scene);
				}
				else if (this.mPointLight != null && this.mPointLight.Enabled)
				{
					Matrix identity2;
					if (this.mPointLightHolder.Joint.mIndex < 0)
					{
						identity2 = Matrix.Identity;
					}
					else
					{
						Matrix.Multiply(ref this.mPointLightHolder.Joint.mBindPose, ref this.mAnimationController.SkinnedBoneTransforms[this.mPointLightHolder.Joint.mIndex], out identity2);
					}
					this.mPointLight.Position = identity2.Translation;
				}
			}
			this.UpdateDamage(iDeltaTime);
			this.UpdateStatusEffects(iDeltaTime);
			Vector3 position7 = this.Position;
			Vector3 direction5 = this.Direction;
			EffectManager.Instance.UpdatePositionDirection(ref this.mDryingEffect, ref position7, ref direction5);
			if (this.mCurrentState is PanicState || this.CurrentState is MoveState)
			{
				if (this.CharacterBody.MaxVelocity > 0f)
				{
					Vector3 velocity2 = this.CharacterBody.Velocity;
					velocity2.Y = 0f;
					float num15 = velocity2.Length() / this.CharacterBody.MaxVelocity;
					if (!float.IsNaN(num15))
					{
						this.mAnimationController.Speed *= num15;
					}
				}
				else
				{
					this.mAnimationController.Speed = 1f;
				}
			}
			else if (this.HasStatus(StatusEffects.Cold))
			{
				this.mAnimationController.Speed *= this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Cold)].GetSlowdown();
			}
			if (this.IsHypnotized)
			{
				this.CharacterBody.SpeedMultiplier *= 0.4f;
				EffectManager.Instance.UpdatePositionDirection(ref this.mHypnotizeEffect, ref vector10, ref direction4);
			}
			if (this.IsCharmed)
			{
				this.mCharmTimer -= iDeltaTime;
				EffectManager.Instance.UpdatePositionDirection(ref this.mCharmEffect, ref vector10, ref direction4);
				if (this.mCharmTimer <= 0f)
				{
					this.EndCharm();
				}
			}
			BoundingSphere boundingSphere = this.mModel.Model.Meshes[0].BoundingSphere;
			boundingSphere.Center = this.Position;
			boundingSphere.Radius *= this.mBoundingScale * this.mStaticTransform.M11;
			this.mEntaglement.Update(iDataChannel, iDeltaTime, ref boundingSphere);
			this.UpdateRenderData(iDataChannel, boundingSphere, iDeltaTime);
			this.mHeightOffset = -0.5f * this.Capsule.Length - this.Capsule.Radius;
			float magnitude = this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Frozen)].Magnitude;
			if (magnitude > 0f || this.mBloating)
			{
				this.mAnimationController.Speed = 0f;
				this.CharacterBody.SpeedMultiplier = 0f;
				this.mTurnSpeed = 0f;
				this.CharacterBody.Movement = Vector3.Zero;
			}
			if (flag && this.mShadowTimer <= 1.1f && !this.IsEthereal)
			{
				vector10 = (Matrix.CreateRotationY(3.1415927f) * Matrix.Invert(this.mModel.SkeletonBones[1].InverseBindPoseTransform)).Translation;
				Vector3.Transform(ref vector10, ref this.mAnimationController.SkinnedBoneTransforms[1], out vector10);
				vector10.Y += this.HeightOffset;
				ShadowBlobs.Instance.AddShadowBlob(ref vector10, this.mRadius, this.mShadowTimer);
				if (this.mHitPoints <= 0f)
				{
					this.mShadowTimer += iDeltaTime;
				}
			}
			for (int num16 = 0; num16 < this.mStatusEffectCues.Length; num16++)
			{
				if (this.mStatusEffectCues[num16] != null)
				{
					this.mStatusEffectCues[num16].Apply3D(this.mPlayState.Camera.Listener, this.mAudioEmitter);
				}
			}
			if (this.mLastAttacker != null && this.mCurrentStatusEffects == StatusEffects.None)
			{
				if (this.mLastAttackerTimer <= 0f)
				{
					this.mLastAttacker = null;
				}
				this.mLastAttackerTimer -= iDeltaTime;
			}
			base.Update(iDataChannel, iDeltaTime);
		}

		// Token: 0x06000DFA RID: 3578 RVA: 0x00056664 File Offset: 0x00054864
		public void OnSpawnedSummon(NonPlayerCharacter npc)
		{
			while (this.mNumCurrentUndeadSummons >= 1)
			{
				this.RemoveUndead();
			}
			while (this.mNumCurrentFlamerSummons >= 4)
			{
				this.RemoveFlamer();
			}
			while (this.mNumCurrentSummons >= 16)
			{
				NonPlayerCharacter nonPlayerCharacter = this.mCurrentSummons[0];
				this.RemoveSummon(0);
				nonPlayerCharacter.OverKill();
			}
			this.mCurrentSummons[this.mNumCurrentSummons] = npc;
			this.mNumCurrentSummons++;
			if (npc.IsUndeadSummon)
			{
				this.mNumCurrentUndeadSummons++;
				return;
			}
			if (npc.IsFlamerSummon)
			{
				this.mNumCurrentFlamerSummons++;
			}
		}

		// Token: 0x06000DFB RID: 3579 RVA: 0x00056700 File Offset: 0x00054900
		private void RemoveUndead()
		{
			for (int i = 0; i < this.mCurrentSummons.Length; i++)
			{
				NonPlayerCharacter nonPlayerCharacter = this.mCurrentSummons[i];
				if (nonPlayerCharacter != null && nonPlayerCharacter.IsUndeadSummon)
				{
					this.RemoveSummon(i);
					nonPlayerCharacter.OverKill();
					this.mNumCurrentUndeadSummons--;
					return;
				}
			}
		}

		// Token: 0x06000DFC RID: 3580 RVA: 0x00056750 File Offset: 0x00054950
		private void RemoveFlamer()
		{
			for (int i = 0; i < this.mCurrentSummons.Length; i++)
			{
				NonPlayerCharacter nonPlayerCharacter = this.mCurrentSummons[i];
				if (nonPlayerCharacter != null && nonPlayerCharacter.IsFlamerSummon)
				{
					this.RemoveSummon(i);
					nonPlayerCharacter.OverKill();
					this.mNumCurrentFlamerSummons--;
					return;
				}
			}
		}

		// Token: 0x06000DFD RID: 3581 RVA: 0x000567A0 File Offset: 0x000549A0
		private void RemoveSummon(int position)
		{
			int num = Math.Min(this.mNumCurrentSummons, 15);
			for (int i = position; i < num; i++)
			{
				this.mCurrentSummons[i] = this.mCurrentSummons[i + 1];
			}
			if (num >= 0)
			{
				this.mCurrentSummons[num] = null;
				this.mNumCurrentSummons--;
			}
		}

		// Token: 0x06000DFE RID: 3582 RVA: 0x000567F4 File Offset: 0x000549F4
		public void OnDespawnedSummon(NonPlayerCharacter npc)
		{
			int num = -1;
			for (int i = 0; i < this.mNumCurrentSummons; i++)
			{
				if (this.mCurrentSummons[i] == npc)
				{
					num = i;
					break;
				}
			}
			if (num >= 0)
			{
				if (this.mCurrentSummons[num].IsUndeadSummon)
				{
					this.RemoveUndead();
					return;
				}
				this.RemoveSummon(num);
			}
		}

		// Token: 0x06000DFF RID: 3583 RVA: 0x00056844 File Offset: 0x00054A44
		protected void UpdateRenderData(DataChannel iDataChannel, BoundingSphere iBoundingSphere, float iDeltaTime)
		{
			if (this.mDoNotRender)
			{
				return;
			}
			int count = this.mModel.SkeletonBones.Count;
			Character.RenderData renderData = this.mRenderData[(int)iDataChannel];
			Array.Copy(this.mAnimationController.SkinnedBoneTransforms, 0, renderData.mSkeleton, 0, count);
			renderData.mBoundingSphere = iBoundingSphere;
			renderData.mMaterial.CubeNormalMapEnabled = false;
			float scaleFactor = MathHelper.Clamp(1f - this.mHitByLightning * 10f, 0f, 1f);
			Vector3.Multiply(ref this.mMaterial.DiffuseColor, scaleFactor, out renderData.mMaterial.DiffuseColor);
			float num = this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Cold)].Magnitude;
			num *= 10f;
			num = Math.Min(num, 1f);
			renderData.mMaterial.Colorize.X = Character.ColdColor.X;
			renderData.mMaterial.Colorize.Y = Character.ColdColor.Y;
			renderData.mMaterial.Colorize.Z = Character.ColdColor.Z;
			renderData.mMaterial.Colorize.W = num;
			renderData.mMaterial.Bloat = this.mBloatTimer / 0.333f * 0.333f;
			Vector3.Multiply(ref renderData.mMaterial.DiffuseColor, 1f + num, out renderData.mMaterial.DiffuseColor);
			if (this.HasStatus(StatusEffects.Frozen))
			{
				renderData.mMaterial.CubeMapRotation = Matrix.Identity;
				renderData.mMaterial.Bloat = (float)((int)(this.StatusMagnitude(StatusEffects.Frozen) * 10f)) * 0.02f;
				renderData.mMaterial.EmissiveAmount = 0.5f;
				renderData.mMaterial.SpecularBias = 1f;
				renderData.mMaterial.SpecularPower = 10f;
				renderData.mMaterial.CubeMapEnabled = true;
				renderData.mMaterial.CubeNormalMapEnabled = true;
				renderData.mMaterial.CubeMap = Character.sIceCubeMap;
				renderData.mMaterial.CubeNormalMap = Character.sIceCubeNormalMap;
				renderData.mMaterial.CubeMapColor.X = (renderData.mMaterial.CubeMapColor.Y = (renderData.mMaterial.CubeMapColor.Z = 1f));
				renderData.mMaterial.CubeMapColor.W = ((this.StatusMagnitude(StatusEffects.Frozen) > 0f) ? 1f : 0f);
			}
			else if (this.HasStatus(StatusEffects.Wet) || this.mDrowning)
			{
				this.mMapRotation += iDeltaTime;
				this.mMapScale += iDeltaTime * 5f;
				renderData.mMaterial.ProjectionMap = Game.Instance.Content.Load<Texture2D>("EffectTextures/wetMap");
				renderData.mMaterial.ProjectionMapEnabled = true;
				renderData.mMaterial.CubeMapEnabled = false;
				Vector3 vector = this.Position + Vector3.Up + Vector3.Forward - this.Position;
				Vector3 direction = this.Direction;
				float angle = (float)Math.Sin((double)this.mMapRotation) - 0.5f;
				Quaternion quaternion;
				Quaternion.CreateFromAxisAngle(ref vector, angle, out quaternion);
				Vector3.Transform(ref direction, ref quaternion, out direction);
				float scale = 1f + (float)Math.Sin((double)this.mMapScale) * 0f;
				renderData.mMaterial.ProjectionMapMatrix = Matrix.CreateLookAt(this.Position + Vector3.Up + new Vector3(0f, 0f, -1f), this.Position, direction) * Matrix.CreateScale(scale);
				renderData.mMaterial.SpecularBias = 0.5f;
				renderData.mMaterial.SpecularPower = this.mMaterial.SpecularPower * 2f;
				Vector3 wetColor = Character.WetColor;
				Vector3.Multiply(ref renderData.mMaterial.DiffuseColor, ref wetColor, out renderData.mMaterial.DiffuseColor);
			}
			else if (this.HasStatus(StatusEffects.Burning))
			{
				this.mMapRotation -= iDeltaTime;
				this.mMapScale += iDeltaTime;
				renderData.mMaterial.ProjectionMap = Game.Instance.Content.Load<Texture2D>("EffectTextures/burnMap");
				renderData.mMaterial.ProjectionMapEnabled = true;
				renderData.mMaterial.CubeMapEnabled = false;
				renderData.mMaterial.ProjectionMapColor = new Vector4(1f);
				renderData.mMaterial.ProjectionMapColor.W = this.StatusMagnitude(StatusEffects.Burning);
				renderData.mMaterial.ProjectionMapAdditive = true;
				Vector3 vector2 = this.Position + Vector3.Up - this.Position;
				Vector3 direction2 = this.Direction;
				Quaternion quaternion2;
				Quaternion.CreateFromAxisAngle(ref vector2, this.mMapRotation, out quaternion2);
				Vector3.Transform(ref direction2, ref quaternion2, out direction2);
				float scale2 = 1f + ((float)Math.Sin((double)this.mMapScale) - 0.25f) * 0.5f;
				renderData.mMaterial.ProjectionMapMatrix = Matrix.CreateLookAt(this.Position + Vector3.Up, this.Position, direction2) * Matrix.CreateScale(scale2);
				renderData.mMaterial.SpecularBias = 0f;
				renderData.mMaterial.SpecularPower = this.mMaterial.SpecularPower;
			}
			else if (this.HasStatus(StatusEffects.Greased))
			{
				this.mMapRotation += iDeltaTime * 0.2f;
				this.mMapScale += iDeltaTime;
				renderData.mMaterial.ProjectionMap = Game.Instance.Content.Load<Texture2D>("EffectTextures/Greased");
				renderData.mMaterial.ProjectionMapEnabled = true;
				renderData.mMaterial.CubeMapEnabled = false;
				Vector3 vector3 = this.Position + Vector3.Down - this.Position;
				Vector3 direction3 = this.Direction;
				Quaternion quaternion3;
				Quaternion.CreateFromAxisAngle(ref vector3, this.mMapRotation, out quaternion3);
				Vector3.Transform(ref direction3, ref quaternion3, out direction3);
				float scale3 = 1f + (float)Math.Sin((double)this.mMapScale) * 0.2f;
				renderData.mMaterial.ProjectionMapMatrix = Matrix.CreateLookAt(this.Position + Vector3.Down, this.Position, direction3) * Matrix.CreateScale(scale3);
				renderData.mMaterial.EmissiveAmount = this.mMaterial.EmissiveAmount;
				renderData.mMaterial.SpecularBias = 0.5f;
				renderData.mMaterial.SpecularPower = this.mMaterial.SpecularPower * 2f;
			}
			else
			{
				renderData.mMaterial.SpecularPower = this.mMaterial.SpecularPower;
				renderData.mMaterial.SpecularAmount = this.mMaterial.SpecularAmount;
				renderData.mMaterial.SpecularBias = 0f;
				renderData.mMaterial.ProjectionMapEnabled = false;
				renderData.mMaterial.CubeMapEnabled = false;
				renderData.mMaterial.EmissiveAmount = this.mMaterial.EmissiveAmount;
			}
			this.mHitByLightning -= iDeltaTime;
			renderData.mMaterial.Damage = 1f - this.mHitPoints / this.mMaxHitPoints;
			ModelMesh modelMesh = this.mModel.Model.Meshes[0];
			ModelMeshPart modelMeshPart = modelMesh.MeshParts[0];
			if (renderData.MeshDirty)
			{
				SkinnedModelDeferredEffect.Technique activeTechnique = (SkinnedModelDeferredEffect.Technique)(modelMeshPart.Effect as SkinnedModelBasicEffect).ActiveTechnique;
				renderData.SetMesh(modelMesh.VertexBuffer, modelMesh.IndexBuffer, modelMeshPart, ref this.mMaterial, activeTechnique);
			}
			if (this.mHighlight)
			{
				Character.HighlightRenderData highlightRenderData = this.mHighlightRenderData[(int)iDataChannel];
				if (highlightRenderData.MeshDirty)
				{
					highlightRenderData.SetMesh(modelMesh.VertexBuffer, modelMesh.IndexBuffer, modelMeshPart);
				}
				highlightRenderData.mBoundingSphere = iBoundingSphere;
				this.mPlayState.Scene.AddRenderableAdditiveObject(iDataChannel, highlightRenderData);
			}
			if (this.IsInvisibile)
			{
				Character.NormalDistortionRenderData normalDistortionRenderData = this.mNormalDistortionRenderData[(int)iDataChannel];
				if (normalDistortionRenderData.MeshDirty)
				{
					normalDistortionRenderData.SetMesh(modelMesh.VertexBuffer, modelMesh.IndexBuffer, modelMeshPart);
				}
				this.mPlayState.Scene.AddPostEffect(iDataChannel, normalDistortionRenderData);
			}
			else if (this.mZapTimer > 0f)
			{
				this.mLastDraw = 0f;
				renderData.mMaterial.DiffuseColor = default(Vector3);
				renderData.mMaterial.SpecularAmount = 0f;
				Character.LightningZapRenderData lightningZapRenderData = this.mLightningZapRenderData[(int)iDataChannel];
				if (lightningZapRenderData.MeshDirty)
				{
					lightningZapRenderData.SetMesh(modelMesh.VertexBuffer, modelMesh.IndexBuffer, modelMeshPart, ref this.mMaterial, this.mTemplate.SkeletonVertices, this.mTemplate.SkeletonVertexDeclaration, this.mTemplate.SkeletonVertexStride, this.mTemplate.SkeletonPrimitiveCount);
				}
				lightningZapRenderData.mBoundingSphere = iBoundingSphere;
				this.mPlayState.Scene.AddPostEffect(iDataChannel, lightningZapRenderData);
			}
			else if (this.mEthereal || this.mEtherealLook)
			{
				renderData.mTechnique = SkinnedModelDeferredEffect.Technique.Additive;
				renderData.mMaterial.Colorize = new Vector4(Character.ColdColor, 1f);
				renderData.mMaterial.Alpha = this.mEtherealAlpha;
				this.mPlayState.Scene.AddRenderableAdditiveObject(iDataChannel, renderData);
			}
			else
			{
				renderData.mTechnique = SkinnedModelDeferredEffect.Technique.Default;
				this.mPlayState.Scene.AddRenderableObject(iDataChannel, renderData);
			}
			if (this.mSelfShield.Active)
			{
				switch (this.mSelfShield.mShieldType)
				{
				case Character.SelfShieldType.Shield:
				{
					Character.ShieldSkinRenderData shieldSkinRenderData = this.mShieldSkinRenderData[(int)iDataChannel];
					shieldSkinRenderData.mBoundingSphere = iBoundingSphere;
					Matrix matrix;
					Matrix.CreateRotationY(2.0943952f, out matrix);
					Vector3 vector4 = default(Vector3);
					vector4.Y = 1f;
					Vector3 position = this.Position;
					Vector3 direction4 = this.Direction;
					Vector3 vector5;
					Vector3.Add(ref position, ref direction4, out vector5);
					Matrix.CreateLookAt(ref position, ref vector5, ref vector4, out shieldSkinRenderData.mProjectionMatrix0);
					Matrix.Multiply(ref shieldSkinRenderData.mProjectionMatrix0, ref matrix, out shieldSkinRenderData.mProjectionMatrix1);
					Matrix.Multiply(ref shieldSkinRenderData.mProjectionMatrix1, ref matrix, out shieldSkinRenderData.mProjectionMatrix2);
					shieldSkinRenderData.mColor.X = Spell.SHIELDCOLOR.X;
					shieldSkinRenderData.mColor.Y = Spell.SHIELDCOLOR.Y;
					shieldSkinRenderData.mColor.Z = Spell.SHIELDCOLOR.Z;
					shieldSkinRenderData.mColor.W = 4f + MathHelper.Max(0.333f - this.mSelfShield.mTimeSinceDamage, 0f) * 60f;
					if (shieldSkinRenderData.MeshDirty)
					{
						shieldSkinRenderData.SetMesh(modelMesh.VertexBuffer, modelMesh.IndexBuffer, modelMeshPart);
					}
					shieldSkinRenderData.mTextureOffset0 = this.mSelfShield.mNoiseOffset0;
					shieldSkinRenderData.mTextureOffset1 = this.mSelfShield.mNoiseOffset1;
					shieldSkinRenderData.mTextureOffset2 = this.mSelfShield.mNoiseOffset2;
					shieldSkinRenderData.mTextureScale.X = (shieldSkinRenderData.mTextureScale.Y = 0.5f);
					this.mPlayState.Scene.AddRenderableAdditiveObject(iDataChannel, shieldSkinRenderData);
					break;
				}
				case Character.SelfShieldType.Earth:
				case Character.SelfShieldType.Ice:
				case Character.SelfShieldType.IcedEarth:
				{
					Character.BarrierSkinRenderData barrierSkinRenderData = this.mBarrierSkinRenderData[(int)iDataChannel];
					barrierSkinRenderData.mBoundingSphere = iBoundingSphere;
					barrierSkinRenderData.RenderIce = false;
					barrierSkinRenderData.RenderEarth = false;
					if (this.mSelfShield.mSpell[Elements.Ice] > 0f)
					{
						barrierSkinRenderData.RenderIce = true;
						barrierSkinRenderData.mIceMaterial.OverlayAlpha = MathHelper.Clamp((this.mSelfShield.mSpell.IceMagnitude - 1f) * 0.333333f, 0f, 1f);
					}
					if (this.mSelfShield.mSpell[Elements.Earth] > 0f)
					{
						barrierSkinRenderData.RenderEarth = true;
						barrierSkinRenderData.mMaterial.OverlayAlpha = MathHelper.Clamp((this.mSelfShield.mSpell.EarthMagnitude - 1f) * 0.333333f, 0f, 1f);
					}
					this.mPlayState.Scene.AddRenderableObject(iDataChannel, barrierSkinRenderData);
					break;
				}
				}
			}
			if (this.mAuras.Count > 0 || this.mHaloBuffFade > 0f)
			{
				Character.HaloAuraRenderData haloAuraRenderData = this.mArmourRenderData[(int)iDataChannel];
				Vector3 translation = this.GetHipAttachOrientation().Translation;
				translation.Y += -this.HeightOffset * 0.5f;
				haloAuraRenderData.Position = translation;
				haloAuraRenderData.mBoundingSphere = iBoundingSphere;
				this.mAuraPulsation = MathHelper.WrapAngle(this.mAuraPulsation + iDeltaTime * 2f);
				float num2 = (float)Math.Sin((double)this.mAuraPulsation) * 0.3f + 0.7f;
				haloAuraRenderData.ColorTint.X = this.mHaloBuffColor.X;
				haloAuraRenderData.ColorTint.Y = this.mHaloBuffColor.Y;
				haloAuraRenderData.ColorTint.Z = this.mHaloBuffColor.Z;
				haloAuraRenderData.ColorTint.W = num2 * this.mHaloBuffFade;
				this.mAuraRays1Rotation = MathHelper.WrapAngle(this.mAuraRays1Rotation + iDeltaTime * 0.666f);
				Matrix.CreateRotationZ(this.mAuraRays1Rotation, out haloAuraRenderData.Ray1Transform);
				this.mAuraRays2Rotation = MathHelper.WrapAngle(this.mAuraRays2Rotation - iDeltaTime * 0.333f);
				Matrix.CreateRotationZ(this.mAuraRays2Rotation, out haloAuraRenderData.Ray2Transform);
				this.mPlayState.Scene.AddRenderableAdditiveObject(iDataChannel, haloAuraRenderData);
				this.mHaloBuffFade -= iDeltaTime;
			}
			this.mHighlight = false;
		}

		// Token: 0x06000E00 RID: 3584 RVA: 0x000575A0 File Offset: 0x000557A0
		protected void UpdateDamage(float iDeltaTime)
		{
			if (this.mHitPoints > 0f && this.mHitPoints <= this.mMaxHitPoints && !this.HasStatus(StatusEffects.Burning) && !this.HasStatus(StatusEffects.Poisoned))
			{
				this.mTotalRegenAccumulation += iDeltaTime * (float)this.mRegenRate;
				this.mHitPoints += iDeltaTime * (float)this.mRegenRate;
				this.mRegenTimer -= iDeltaTime;
				if (this.mRegenTimer <= 0f)
				{
					this.mRegenTimer = 1f;
					this.mTotalRegenAccumulation -= (float)this.mRegenRate;
					Vector3 position = this.Position;
					position.Y += this.Capsule.Length * 0.5f + this.mRadius;
					if (this.mRegenRate != 0 && GameStateManager.Instance.CurrentState is PlayState && !(GameStateManager.Instance.CurrentState as PlayState).IsInCutscene && this.mHitPoints <= this.mMaxHitPoints)
					{
						DamageNotifyer.Instance.AddNumber((float)(-(float)this.mRegenRate), ref position, 0.7f, false);
					}
				}
				if (this.mHitPoints > this.mMaxHitPoints)
				{
					this.mHitPoints = this.mMaxHitPoints;
				}
			}
			Vector3 position2 = this.Position;
			position2.Y += this.Capsule.Length * 0.5f + this.mRadius;
			this.mTimeSinceLastDamage += iDeltaTime;
			this.mTimeSinceLastStatusDamage += iDeltaTime;
			if (this.mLastDamageIndex >= 0)
			{
				if (this.mTimeSinceLastDamage > 0.333f || this.Dead)
				{
					DamageNotifyer.Instance.ReleasNumber(this.mLastDamageIndex);
					this.mLastDamageIndex = -1;
				}
				else
				{
					DamageNotifyer.Instance.UpdateNumberPosition(this.mLastDamageIndex, ref position2);
				}
			}
			this.mHealingAccumulationTimer -= iDeltaTime;
			if (this.mHealingAccumulationTimer <= 0f)
			{
				this.mHealingAccumulationTimer += 1f;
				if (this.mHealingAccumulation != 0f)
				{
					DamageNotifyer.Instance.AddNumber(this.mHealingAccumulation, ref position2, 0.7f, false);
					this.mHealingAccumulation = 0f;
					this.mTimeSinceLastStatusDamage = 0f;
				}
			}
			this.mFireDamageAccumulationTimer -= iDeltaTime;
			if (this.mFireDamageAccumulationTimer <= 0f)
			{
				this.mFireDamageAccumulationTimer += 1f;
				if (this.mFireDamageAccumulation != 0f)
				{
					Vector3 firecolor = Spell.FIRECOLOR;
					DamageNotifyer.Instance.AddNumber(this.mFireDamageAccumulation, ref position2, 0.7f, false, ref firecolor);
					this.mFireDamageAccumulation = 0f;
					this.mTimeSinceLastStatusDamage = 0f;
				}
			}
			this.mPoisonDamageAccumulationTimer -= iDeltaTime;
			if (this.mPoisonDamageAccumulationTimer <= 0f)
			{
				this.mPoisonDamageAccumulationTimer += 1f;
				if (this.mPoisonDamageAccumulation != 0f)
				{
					Vector3 poisoncolor = Spell.POISONCOLOR;
					DamageNotifyer.Instance.AddNumber(this.mPoisonDamageAccumulation, ref position2, 0.7f, false, ref poisoncolor);
					this.mPoisonDamageAccumulation = 0f;
					this.mTimeSinceLastStatusDamage = 0f;
				}
			}
			this.mBleedDamageAccumulationTimer -= iDeltaTime;
			if (this.mBleedDamageAccumulationTimer <= 0f)
			{
				this.mBleedDamageAccumulationTimer += 1f;
				if (this.mBleedDamageAccumulation != 0f)
				{
					Vector3 vector = new Vector3(1.25f, 0.1254902f, 0.0627451f);
					DamageNotifyer.Instance.AddNumber(this.mBleedDamageAccumulation, ref position2, 0.7f, false, ref vector);
					this.mBleedDamageAccumulation = 0f;
					this.mTimeSinceLastStatusDamage = 0f;
				}
			}
		}

		// Token: 0x06000E01 RID: 3585 RVA: 0x00057954 File Offset: 0x00055B54
		protected void UpdateStatusEffects(float iDeltaTime)
		{
			this.mDryTimer -= iDeltaTime;
			StatusEffects statusEffects = StatusEffects.None;
			int num = StatusEffect.StatusIndex(StatusEffects.Burning);
			int num2 = StatusEffect.StatusIndex(StatusEffects.Wet);
			StatusEffect.StatusIndex(StatusEffects.Cold);
			int num3 = StatusEffect.StatusIndex(StatusEffects.Frozen);
			int num4 = StatusEffect.StatusIndex(StatusEffects.Steamed);
			if (this.Dead)
			{
				for (int i = 0; i < this.mStatusEffects.Length; i++)
				{
					switch (this.mStatusEffects[i].DamageType)
					{
					case StatusEffects.Burning:
						if (this.mStatusEffectCues[num] != null && !this.mStatusEffectCues[num].IsStopping)
						{
							this.mStatusEffectCues[num].Stop(AudioStopOptions.AsAuthored);
						}
						break;
					case StatusEffects.Wet:
						if (this.mStatusEffectCues[num2] != null && !this.mStatusEffectCues[num2].IsStopping)
						{
							this.mStatusEffectCues[num2].Stop(AudioStopOptions.AsAuthored);
						}
						break;
					case StatusEffects.Frozen:
						if (this.mStatusEffectCues[num3] != null && !this.mStatusEffectCues[num3].IsStopping)
						{
							this.mStatusEffectCues[num3].Stop(AudioStopOptions.AsAuthored);
						}
						break;
					}
					this.mStatusEffects[i].Stop();
					this.mStatusEffects[i] = default(StatusEffect);
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
						case StatusEffects.Burning:
							if (this.mStatusEffectCues[num] != null && !this.mStatusEffectCues[num].IsStopping)
							{
								this.mStatusEffectCues[num].Stop(AudioStopOptions.AsAuthored);
							}
							break;
						case StatusEffects.Wet:
							if (this.mStatusEffectCues[num2] != null && !this.mStatusEffectCues[num2].IsStopping)
							{
								this.mStatusEffectCues[num2].Stop(AudioStopOptions.AsAuthored);
							}
							break;
						case StatusEffects.Frozen:
							if (this.mStatusEffectCues[num3] != null && !this.mStatusEffectCues[num3].IsStopping)
							{
								this.mStatusEffectCues[num3].Stop(AudioStopOptions.AsAuthored);
							}
							break;
						}
						this.mStatusEffects[j].Stop();
						this.mStatusEffects[j] = default(StatusEffect);
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
			if (this.HasStatus(StatusEffects.Steamed))
			{
				this.mPanic = Math.Max(this.mStatusEffects[num4].Magnitude - this.mMaxPanic, 0f);
			}
			else if (this.HasStatus(StatusEffects.Burning))
			{
				this.mPanic = Math.Max(this.mStatusEffects[num].Magnitude - this.mMaxPanic, 0f);
				if (this.mStatusEffectLight == null)
				{
					this.mStatusEffectLight = DynamicLight.GetCachedLight();
					this.mStatusEffectLight.Initialize(this.Position, new Vector3(1f, 0.4f, 0f), 1f, 5f, 1f, 0.5f);
					this.mStatusEffectLight.VariationType = LightVariationType.Candle;
					this.mStatusEffectLight.VariationSpeed = 4f;
					this.mStatusEffectLight.VariationAmount = 0.2f;
					this.mStatusEffectLight.AmbientColor = new Vector3(0.4f, 0.2f, 0f);
					this.mStatusEffectLight.Enable();
				}
			}
			else if (this.mStatusEffectLight != null)
			{
				this.mPanic = 0f;
				this.mStatusEffectLight.Stop(false);
				this.mStatusEffectLight = null;
			}
			this.mCurrentStatusEffects = statusEffects;
		}

		// Token: 0x06000E02 RID: 3586 RVA: 0x00057D1C File Offset: 0x00055F1C
		public virtual void BreakFree()
		{
			this.mBreakFreeCounter += (int)this.mBreakFreeStrength;
			this.DamageEntanglement(0.5f, Elements.Earth);
		}

		// Token: 0x06000E03 RID: 3587 RVA: 0x00057D3E File Offset: 0x00055F3E
		public void Highlight()
		{
			if (this.mDialog == 0)
			{
				return;
			}
			this.mHighlight = true;
		}

		// Token: 0x06000E04 RID: 3588 RVA: 0x00057D50 File Offset: 0x00055F50
		private void BloatBlast()
		{
			if (this.mBloatElement != Elements.None)
			{
				float iRadius = this.mRadius + 2.5f;
				Damage iDamage;
				iDamage.AttackProperty = AttackProperties.Damage;
				iDamage.Element = this.mBloatElement;
				if (this.mBloatElement == Elements.Life)
				{
					iDamage.Amount = Defines.SPELL_DAMAGE_LIFE;
				}
				else
				{
					iDamage.Amount = Defines.SPELL_DAMAGE_ARCANE;
				}
				iDamage.Magnitude = 1f + this.MaxHitPoints / 5000f;
				Blast.FullBlast(this.mPlayState, this.mBloatKiller, base.PlayState.PlayTime, null, iRadius, this.Position, iDamage);
				AudioManager.Instance.PlayCue(Banks.Spells, Railgun.ARCANESTAGESOUNDSHASH[3], base.AudioEmitter);
			}
			this.mBloating = false;
			if (this.HasGibs())
			{
				this.mBloatKilled = true;
				this.OverKill();
				return;
			}
			this.Kill();
		}

		// Token: 0x17000366 RID: 870
		// (get) Token: 0x06000E05 RID: 3589 RVA: 0x00057E2A File Offset: 0x0005602A
		public Entity CharmOwner
		{
			get
			{
				return this.mCharmOwner;
			}
		}

		// Token: 0x06000E06 RID: 3590 RVA: 0x00057E34 File Offset: 0x00056034
		public virtual void Charm(Entity iCassanova, float iTTL, int iEffect)
		{
			if (this.IsUncharmable)
			{
				return;
			}
			this.mCharmOwner = iCassanova;
			this.mCharmTimer = iTTL;
			EffectManager.Instance.Stop(ref this.mCharmEffect);
			this.mCharmEffectID = iEffect;
			if (iEffect != 0)
			{
				Vector3 position = this.Position;
				Vector3 direction = this.Direction;
				EffectManager.Instance.StartEffect(iEffect, ref position, ref direction, out this.mCharmEffect);
			}
		}

		// Token: 0x17000367 RID: 871
		// (get) Token: 0x06000E07 RID: 3591 RVA: 0x00057E96 File Offset: 0x00056096
		public bool IsCharmed
		{
			get
			{
				return this.mCharmTimer > float.Epsilon;
			}
		}

		// Token: 0x06000E08 RID: 3592 RVA: 0x00057EA5 File Offset: 0x000560A5
		public virtual void EndCharm()
		{
			this.mCharmTimer = 0f;
			EffectManager.Instance.Stop(ref this.mCharmEffect);
		}

		// Token: 0x17000368 RID: 872
		// (get) Token: 0x06000E09 RID: 3593 RVA: 0x00057EC2 File Offset: 0x000560C2
		public Vector3 FearPosition
		{
			get
			{
				if (this.mFearedBy != null)
				{
					return this.mFearedBy.Position;
				}
				return this.mFearPosition;
			}
		}

		// Token: 0x06000E0A RID: 3594 RVA: 0x00057EE0 File Offset: 0x000560E0
		public void Fear(Vector3 iPosition)
		{
			if ((this.mFaction & (Factions.DEMON | Factions.UNDEAD)) != Factions.NONE)
			{
				return;
			}
			if (this.IsFearless)
			{
				return;
			}
			this.mFearTimer = 5f;
			this.mFearedBy = null;
			this.mFearPosition = iPosition;
			if (!EffectManager.Instance.IsActive(ref this.mFearEffect))
			{
				Vector3 position = this.Position;
				Vector3 direction = this.Direction;
				EffectManager.Instance.StartEffect(Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Fear.FEARED_EFFECT, ref position, ref direction, out this.mFearEffect);
			}
		}

		// Token: 0x06000E0B RID: 3595 RVA: 0x00057F58 File Offset: 0x00056158
		public void Fear(Character iFearedBy)
		{
			if ((this.mFaction & (Factions.DEMON | Factions.UNDEAD)) != Factions.NONE)
			{
				return;
			}
			if (this.IsFearless)
			{
				return;
			}
			this.mFearTimer = 5f;
			this.mFearedBy = iFearedBy;
			if (!EffectManager.Instance.IsActive(ref this.mFearEffect))
			{
				Vector3 position = this.Position;
				Vector3 direction = this.Direction;
				EffectManager.Instance.StartEffect(Magicka.GameLogic.Entities.Abilities.SpecialAbilities.Fear.FEARED_EFFECT, ref position, ref direction, out this.mFearEffect);
			}
		}

		// Token: 0x06000E0C RID: 3596 RVA: 0x00057FC6 File Offset: 0x000561C6
		public void RemoveFear()
		{
			this.mFearTimer = 0f;
			this.mFearedBy = null;
			EffectManager.Instance.Stop(ref this.mFearEffect);
		}

		// Token: 0x17000369 RID: 873
		// (get) Token: 0x06000E0D RID: 3597 RVA: 0x00057FEA File Offset: 0x000561EA
		public bool IsFeared
		{
			get
			{
				return this.mFearTimer > 0f && !this.Dead;
			}
		}

		// Token: 0x1700036A RID: 874
		// (get) Token: 0x06000E0E RID: 3598 RVA: 0x00058004 File Offset: 0x00056204
		public BloodType BloodType
		{
			get
			{
				return this.mBlood;
			}
		}

		// Token: 0x1700036B RID: 875
		// (get) Token: 0x06000E0F RID: 3599
		public abstract bool IsInAEvent { get; }

		// Token: 0x06000E10 RID: 3600 RVA: 0x0005800C File Offset: 0x0005620C
		private void PostCollision(ref CollisionInfo iInfo)
		{
			if ((this is Avatar || this.mPlayState.IsInCutscene) && this.IsInAEvent && !(iInfo.SkinInfo.Skin0.Tag is LevelModel | iInfo.SkinInfo.Skin1.Tag is LevelModel | iInfo.SkinInfo.Skin0.Tag is Water | iInfo.SkinInfo.Skin1.Tag is Water))
			{
				if (iInfo.SkinInfo.Skin0 == this.mCollision)
				{
					iInfo.SkinInfo.IgnoreSkin0 = true;
					return;
				}
				iInfo.SkinInfo.IgnoreSkin1 = true;
			}
		}

		// Token: 0x06000E11 RID: 3601 RVA: 0x000580D4 File Offset: 0x000562D4
		protected bool OnCollision(CollisionSkin iSkin0, int iPrim0, CollisionSkin iSkin1, int iPrim1)
		{
			if (iSkin1.Owner != null)
			{
				if (this.Dead)
				{
					return false;
				}
				if (iSkin1.Owner.Tag == this.mGripper)
				{
					return false;
				}
				if (iSkin1.Owner.Tag == this.mGrippedCharacter)
				{
					return false;
				}
				Character character = iSkin1.Owner.Tag as Character;
				if (this.CurrentSelfShieldType == Character.SelfShieldType.Ice || this.CurrentSelfShieldType == Character.SelfShieldType.IcedEarth)
				{
					if (character != null && !this.mHitList.ContainsKey(character.Handle))
					{
						Vector3 velocity = character.Body.Velocity;
						Vector3 velocity2 = this.Body.Velocity;
						float num = velocity.LengthSquared();
						float num2 = velocity2.LengthSquared();
						if (num >= 100f || num2 >= 100f)
						{
							DamageCollection5 iDamage;
							this.mSelfShield.mSpell.CalculateDamage(SpellType.Projectile, CastType.Force, out iDamage);
							character.Damage(iDamage, this, 0.0, default(Vector3));
							this.mHitList.Add(character);
						}
					}
				}
				else if (this.CurrentSelfShieldType == Character.SelfShieldType.Shield)
				{
					Shield shield = iSkin1.Owner.Tag as Shield;
					if (character != null && character.CurrentSelfShieldType == Character.SelfShieldType.Shield)
					{
						this.RemoveSelfShield(Character.SelfShieldType.Shield);
						character.RemoveSelfShield(Character.SelfShieldType.Shield);
						Vector3 position = this.Position;
						Vector3 position2 = character.Position;
						Vector3.Subtract(ref position2, ref position, out position2);
						position2.Normalize();
						Vector3.Multiply(ref position2, this.Radius, out position2);
						Vector3.Add(ref position, ref position2, out position);
						Vector3 right = Vector3.Right;
						VisualEffectReference visualEffectReference;
						EffectManager.Instance.StartEffect(Nullify.EFFECT, ref position, ref right, out visualEffectReference);
						AudioManager.Instance.PlayCue(Banks.Spells, Invisibility.SOUNDHASH, base.AudioEmitter);
					}
					else if (shield != null)
					{
						this.RemoveSelfShield(Character.SelfShieldType.Shield);
						shield.Kill(0.25f);
						Vector3 position3 = this.Position;
						Vector3 position4 = shield.Position;
						if (shield.ShieldType == ShieldType.WALL)
						{
							Vector3 forward = shield.Body.Orientation.Forward;
							Vector3.Multiply(ref forward, shield.Radius, out forward);
							Vector3.Subtract(ref position4, ref forward, out position4);
							Vector3 vector;
							Vector3.Subtract(ref position3, ref position4, out vector);
							forward = shield.Body.Orientation.Forward;
							Vector3.Multiply(ref forward, 2f * shield.Radius, out forward);
							vector.Y = (forward.Y = 0f);
							float num3;
							Vector3.Dot(ref vector, ref forward, out num3);
							num3 /= forward.Length();
							forward = shield.Body.Orientation.Forward;
							Vector3.Multiply(ref forward, num3, out forward);
							Vector3.Add(ref position4, ref forward, out position3);
							position3.Y = this.Position.Y;
						}
						else
						{
							Vector3.Subtract(ref position3, ref position4, out position3);
							position3.Normalize();
							Vector3.Multiply(ref position3, shield.Radius, out position3);
							Vector3.Add(ref position3, ref position4, out position3);
						}
						Vector3 right2 = Vector3.Right;
						VisualEffectReference visualEffectReference2;
						EffectManager.Instance.StartEffect(Nullify.EFFECT, ref position3, ref right2, out visualEffectReference2);
						AudioManager.Instance.PlayCue(Banks.Spells, Invisibility.SOUNDHASH, base.AudioEmitter);
						return false;
					}
				}
			}
			if (iSkin1.Tag is MagickCamera)
			{
				this.mCollidedWithCamera = true;
			}
			if (iSkin1.Tag is LevelModel | iSkin1.Tag is Liquid)
			{
				if (this.mCollisionDamages.A.AttackProperty != (AttackProperties)0)
				{
					this.Damage(this.mCollisionDamages, null, base.PlayState.PlayTime, this.Position);
					this.mCollisionDamages = default(DamageCollection5);
				}
				return true;
			}
			return !this.mEthereal && this.mCollisionIgnoreTime <= 0f;
		}

		// Token: 0x06000E12 RID: 3602 RVA: 0x00058497 File Offset: 0x00056697
		public virtual MissileEntity GetMissileInstance()
		{
			return MissileEntity.GetInstance(this.mPlayState);
		}

		// Token: 0x06000E13 RID: 3603 RVA: 0x000584A4 File Offset: 0x000566A4
		public override Vector3 CalcImpulseVelocity(Vector3 iDirection, float iElevation, float iMassPower, float iDistance)
		{
			if ((this.mImmortalTime > 0f | this.mEthereal) || this.mSelfShield.Solid || this.mBody.Mass > 1000f || (this.IsEntangled | this.Dead))
			{
				return default(Vector3);
			}
			return base.CalcImpulseVelocity(iDirection, iElevation, iMassPower, iDistance);
		}

		// Token: 0x06000E14 RID: 3604 RVA: 0x0005850A File Offset: 0x0005670A
		protected override void AddImpulseVelocity(ref Vector3 iVelocity)
		{
			this.CharacterBody.AddImpulseVelocity(ref iVelocity);
		}

		// Token: 0x06000E15 RID: 3605 RVA: 0x00058518 File Offset: 0x00056718
		public void Jump(Vector3 iDelta, float iElevation)
		{
			if (!this.IsEntangled && !this.Dead)
			{
				if (!this.CharacterBody.IsJumping && NetworkManager.Instance.State == NetworkState.Server)
				{
					CharacterActionMessage characterActionMessage = default(CharacterActionMessage);
					characterActionMessage.Handle = base.Handle;
					characterActionMessage.Action = ActionType.Jump;
					characterActionMessage.Param0F = iDelta.X;
					characterActionMessage.Param1F = iDelta.Y;
					characterActionMessage.Param1F = iDelta.Z;
					characterActionMessage.TimeStamp = (double)iElevation;
					NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref characterActionMessage);
				}
				float y = iDelta.Y;
				iDelta.Y = 0f;
				float num = iDelta.Length();
				Vector3.Divide(ref iDelta, num, out iDelta);
				float num2 = iDelta.Y = (float)Math.Sin((double)iElevation);
				float num3 = (float)Math.Cos((double)iElevation);
				iDelta.X *= num3;
				iDelta.Z *= num3;
				float num4 = (float)Math.Sqrt((double)(PhysicsManager.Instance.Simulator.Gravity.Y * -1f * num * num / (2f * (num * num2 / num3 - y) * num3 * num3)));
				if (float.IsNaN(num4) || float.IsInfinity(num4))
				{
					return;
				}
				Vector3.Multiply(ref iDelta, num4, out iDelta);
				this.CharacterBody.AddJump(iDelta);
			}
		}

		// Token: 0x1700036C RID: 876
		// (get) Token: 0x06000E16 RID: 3606 RVA: 0x00058682 File Offset: 0x00056882
		public Capsule Capsule
		{
			get
			{
				return (Capsule)this.mCollision.GetPrimitiveLocal(0);
			}
		}

		// Token: 0x1700036D RID: 877
		// (get) Token: 0x06000E17 RID: 3607 RVA: 0x00058695 File Offset: 0x00056895
		internal CharacterBody CharacterBody
		{
			get
			{
				return (CharacterBody)this.mBody;
			}
		}

		// Token: 0x1700036E RID: 878
		// (get) Token: 0x06000E18 RID: 3608 RVA: 0x000586A4 File Offset: 0x000568A4
		public override Vector3 Direction
		{
			get
			{
				return this.mBody.Orientation.Forward;
			}
		}

		// Token: 0x1700036F RID: 879
		// (get) Token: 0x06000E19 RID: 3609 RVA: 0x000586C4 File Offset: 0x000568C4
		public virtual float Volume
		{
			get
			{
				return this.mVolume;
			}
		}

		// Token: 0x17000370 RID: 880
		// (get) Token: 0x06000E1A RID: 3610 RVA: 0x000586CC File Offset: 0x000568CC
		// (set) Token: 0x06000E1B RID: 3611 RVA: 0x000586D4 File Offset: 0x000568D4
		public float TurnSpeed
		{
			get
			{
				return this.mTurnSpeed;
			}
			set
			{
				this.mTurnSpeed = value;
			}
		}

		// Token: 0x17000371 RID: 881
		// (get) Token: 0x06000E1C RID: 3612 RVA: 0x000586E0 File Offset: 0x000568E0
		public int ModelIndex
		{
			get
			{
				for (int i = 0; i < this.mTemplate.Models.Length; i++)
				{
					if (this.mTemplate.Models[i].Model == this.mModel)
					{
						return i;
					}
				}
				return -1;
			}
		}

		// Token: 0x17000372 RID: 882
		// (get) Token: 0x06000E1D RID: 3613 RVA: 0x00058726 File Offset: 0x00056926
		public SkinnedModel Model
		{
			get
			{
				return this.mModel;
			}
		}

		// Token: 0x17000373 RID: 883
		// (get) Token: 0x06000E1E RID: 3614 RVA: 0x0005872E File Offset: 0x0005692E
		public AnimationController AnimationController
		{
			get
			{
				return this.mAnimationController;
			}
		}

		// Token: 0x17000374 RID: 884
		// (get) Token: 0x06000E1F RID: 3615 RVA: 0x00058736 File Offset: 0x00056936
		public Animations CurrentAnimation
		{
			get
			{
				return this.mCurrentAnimation % Animations.totalanimations;
			}
		}

		// Token: 0x06000E20 RID: 3616 RVA: 0x00058748 File Offset: 0x00056948
		public bool HasAnimation(Animations iAnimation)
		{
			for (int i = 0; i < this.mEquipment.Length; i++)
			{
				Item item = this.mEquipment[i].Item;
				if (item != null && item.WeaponClass != WeaponClass.Staff && this.mAnimationClips[(int)item.WeaponClass] != null && this.mAnimationClips[(int)item.WeaponClass][(int)iAnimation] != null)
				{
					return true;
				}
			}
			return this.mAnimationClips[0][(int)iAnimation] != null;
		}

		// Token: 0x06000E21 RID: 3617 RVA: 0x000587B8 File Offset: 0x000569B8
		protected void OnAnimationLooped()
		{
			this.mExecutedActions.Clear();
			this.mDeadActions.Clear();
			for (int i = 0; i < this.mCurrentActions.Length; i++)
			{
				this.mExecutedActions.Add(false);
				this.mDeadActions.Add(false);
			}
		}

		// Token: 0x06000E22 RID: 3618 RVA: 0x00058808 File Offset: 0x00056A08
		protected void OnCrossfadeFinished()
		{
			AnimationClipAction[] array = this.mAnimationClips[(int)this.mCurrentAnimationSet];
			AnimationClipAction animationClipAction = array[(int)this.mCurrentAnimation];
			if (this.mCurrentActions != animationClipAction.Actions)
			{
				for (int i = 0; i < this.mCurrentActions.Length; i++)
				{
					if (!this.mDeadActions[i])
					{
						this.mCurrentActions[i].Kill(this);
					}
				}
			}
			this.mCurrentActions = animationClipAction.Actions;
			this.mExecutedActions.Clear();
			this.mDeadActions.Clear();
			this.mForceAnimationUpdate = false;
			for (int j = 0; j < this.mCurrentActions.Length; j++)
			{
				this.mExecutedActions.Add(false);
				this.mDeadActions.Add(false);
				if (this.mCurrentActions[j].UsesBones)
				{
					this.mForceAnimationUpdate = true;
				}
			}
		}

		// Token: 0x06000E23 RID: 3619 RVA: 0x000588D4 File Offset: 0x00056AD4
		public void GoToAnimation(Animations iAnimation, float iTime)
		{
			if (this.mCurrentAnimation != iAnimation | this.mAnimationController.HasFinished)
			{
				this.mCurrentAnimation = iAnimation;
				this.mCurrentAnimationSet = WeaponClass.Default;
				AnimationClipAction animationClipAction = this.mAnimationClips[0][(int)iAnimation];
				for (int i = 0; i < this.mEquipment.Length; i++)
				{
					Item item = this.mEquipment[i].Item;
					if (item != null && item.WeaponClass != WeaponClass.Staff && this.mAnimationClips[(int)item.WeaponClass] != null && this.mAnimationClips[(int)item.WeaponClass][(int)iAnimation] != null)
					{
						this.mCurrentAnimationSet = item.WeaponClass;
						animationClipAction = this.mAnimationClips[(int)this.mCurrentAnimationSet][(int)iAnimation];
						break;
					}
				}
				if (animationClipAction == null)
				{
					this.mCurrentAnimation = Animations.idle;
					this.mCurrentAnimationSet = WeaponClass.Default;
					animationClipAction = this.mAnimationClips[(int)this.mCurrentAnimationSet][(int)this.mCurrentAnimation];
					this.mAnimationController.ClipSpeed = animationClipAction.AnimationSpeed;
					float blendTime = animationClipAction.BlendTime;
					this.mAnimationController.CrossFade(animationClipAction.Clip, (blendTime > 0f) ? blendTime : iTime, false);
					return;
				}
				this.mAnimationController.ClipSpeed = animationClipAction.AnimationSpeed;
				float blendTime2 = animationClipAction.BlendTime;
				this.mAnimationController.CrossFade(animationClipAction.Clip, (blendTime2 > 0f) ? blendTime2 : iTime, animationClipAction.LoopAnimation);
			}
		}

		// Token: 0x06000E24 RID: 3620 RVA: 0x00058A20 File Offset: 0x00056C20
		public void ForceAnimation(Animations iAnimation)
		{
			this.mAnimationController.ClearCrossfadeQueue();
			this.mCurrentAnimation = iAnimation;
			this.mCurrentAnimationSet = WeaponClass.Default;
			AnimationClipAction animationClipAction = this.mAnimationClips[0][(int)iAnimation];
			for (int i = 0; i < this.mEquipment.Length; i++)
			{
				Item item = this.mEquipment[i].Item;
				if (item != null && item.WeaponClass != WeaponClass.Staff && this.mAnimationClips[(int)item.WeaponClass] != null && this.mAnimationClips[(int)item.WeaponClass][(int)iAnimation] != null)
				{
					this.mCurrentAnimationSet = item.WeaponClass;
					animationClipAction = this.mAnimationClips[(int)this.mCurrentAnimationSet][(int)iAnimation];
					break;
				}
			}
			if (animationClipAction == null)
			{
				this.mCurrentAnimation = Animations.idle;
				this.mCurrentAnimationSet = WeaponClass.Default;
			}
			this.mAnimationController.ClipSpeed = animationClipAction.AnimationSpeed;
			this.mAnimationController.StartClip(animationClipAction.Clip, animationClipAction.LoopAnimation);
			this.OnCrossfadeFinished();
		}

		// Token: 0x17000375 RID: 885
		// (get) Token: 0x06000E25 RID: 3621 RVA: 0x00058AFC File Offset: 0x00056CFC
		// (set) Token: 0x06000E26 RID: 3622 RVA: 0x00058AFF File Offset: 0x00056CFF
		internal virtual bool Polymorphed
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		// Token: 0x06000E27 RID: 3623 RVA: 0x00058B04 File Offset: 0x00056D04
		public override Matrix GetOrientation()
		{
			Matrix orientation = this.mBody.Orientation;
			orientation.Translation = this.mBody.Position;
			orientation.M42 += this.mHeightOffset;
			Matrix.Multiply(ref this.mStaticTransform, ref orientation, out orientation);
			return orientation;
		}

		// Token: 0x06000E28 RID: 3624 RVA: 0x00058B54 File Offset: 0x00056D54
		public Matrix GetHipAttachOrientation()
		{
			if (this.mHipJoint.mIndex < 0)
			{
				return Matrix.Identity;
			}
			Matrix result;
			Matrix.Multiply(ref this.mHipJoint.mBindPose, ref this.mAnimationController.SkinnedBoneTransforms[this.mHipJoint.mIndex], out result);
			return result;
		}

		// Token: 0x06000E29 RID: 3625 RVA: 0x00058BA4 File Offset: 0x00056DA4
		public virtual Matrix GetRightAttachOrientation()
		{
			if (this.mRightHandJoint.mIndex < 0)
			{
				return Matrix.Identity;
			}
			Matrix result;
			Matrix.Multiply(ref this.mRightHandJoint.mBindPose, ref this.mAnimationController.SkinnedBoneTransforms[this.mRightHandJoint.mIndex], out result);
			return result;
		}

		// Token: 0x06000E2A RID: 3626 RVA: 0x00058BF4 File Offset: 0x00056DF4
		public virtual Matrix GetLeftAttachOrientation()
		{
			if (this.mLeftHandJoint.mIndex < 0)
			{
				return Matrix.Identity;
			}
			Matrix result;
			Matrix.Multiply(ref this.mLeftHandJoint.mBindPose, ref this.mAnimationController.SkinnedBoneTransforms[this.mLeftHandJoint.mIndex], out result);
			return result;
		}

		// Token: 0x06000E2B RID: 3627 RVA: 0x00058C44 File Offset: 0x00056E44
		public virtual Matrix GetMouthAttachOrientation()
		{
			if (this.mMouthJoint.mIndex < 0)
			{
				return Matrix.Identity;
			}
			Matrix result;
			Matrix.Multiply(ref this.mMouthJoint.mBindPose, ref this.mAnimationController.SkinnedBoneTransforms[this.mMouthJoint.mIndex], out result);
			return result;
		}

		// Token: 0x06000E2C RID: 3628 RVA: 0x00058C94 File Offset: 0x00056E94
		public virtual Matrix GetBoneOrientation(int iBoneIndex, ref Matrix iBoneBindPose)
		{
			Matrix result;
			Matrix.Multiply(ref iBoneBindPose, ref this.mAnimationController.SkinnedBoneTransforms[iBoneIndex], out result);
			return result;
		}

		// Token: 0x06000E2D RID: 3629 RVA: 0x00058CBC File Offset: 0x00056EBC
		public virtual Matrix GetItemAttachOrientation(int iIndex)
		{
			Attachment attachment = this.mEquipment[iIndex];
			if (attachment.AttachIndex < 0)
			{
				return Matrix.Identity;
			}
			Matrix bindBose = attachment.BindBose;
			Matrix.Multiply(ref bindBose, ref this.mAnimationController.SkinnedBoneTransforms[attachment.AttachIndex], out bindBose);
			return bindBose;
		}

		// Token: 0x06000E2E RID: 3630 RVA: 0x00058D08 File Offset: 0x00056F08
		public void KnockDown(float iMagnitude)
		{
			if (iMagnitude >= this.mKnockdownTolerance)
			{
				this.mKnockedDown = true;
				Vector3 velocity = this.mBody.Velocity;
				velocity.X = 0f;
				velocity.Z = 0f;
				this.mBody.Velocity = velocity;
			}
		}

		// Token: 0x06000E2F RID: 3631 RVA: 0x00058D58 File Offset: 0x00056F58
		public void KnockDown()
		{
			this.mKnockedDown = true;
			Vector3 velocity = this.mBody.Velocity;
			velocity.X = 0f;
			velocity.Z = 0f;
			this.mBody.Velocity = velocity;
			this.mGripDamageAccumulation = 0f;
		}

		// Token: 0x17000376 RID: 886
		// (get) Token: 0x06000E30 RID: 3632 RVA: 0x00058DA7 File Offset: 0x00056FA7
		// (set) Token: 0x06000E31 RID: 3633 RVA: 0x00058DAF File Offset: 0x00056FAF
		public virtual bool IsAlerted
		{
			get
			{
				return this.mAlert;
			}
			set
			{
				this.mAlert = value;
			}
		}

		// Token: 0x17000377 RID: 887
		// (get) Token: 0x06000E32 RID: 3634 RVA: 0x00058DB8 File Offset: 0x00056FB8
		// (set) Token: 0x06000E33 RID: 3635 RVA: 0x00058DC0 File Offset: 0x00056FC0
		public virtual bool IsBlocking
		{
			get
			{
				return this.mBlock;
			}
			set
			{
				if (this.mBlock != value & NetworkManager.Instance.State != NetworkState.Offline)
				{
					CharacterActionMessage characterActionMessage = default(CharacterActionMessage);
					characterActionMessage.Handle = base.Handle;
					characterActionMessage.Action = ActionType.Block;
					characterActionMessage.Param0I = (value ? 1 : 0);
					NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref characterActionMessage);
				}
				this.mBlock = value;
			}
		}

		// Token: 0x17000378 RID: 888
		// (get) Token: 0x06000E34 RID: 3636 RVA: 0x00058E2F File Offset: 0x0005702F
		// (set) Token: 0x06000E35 RID: 3637 RVA: 0x00058E37 File Offset: 0x00057037
		public int BlockItem { get; set; }

		// Token: 0x17000379 RID: 889
		// (get) Token: 0x06000E36 RID: 3638 RVA: 0x00058E40 File Offset: 0x00057040
		public bool Chanting
		{
			get
			{
				return this.mSpellQueue.Count > 0;
			}
		}

		// Token: 0x06000E37 RID: 3639 RVA: 0x00058E50 File Offset: 0x00057050
		public virtual void Die()
		{
			if (this.mDead)
			{
				return;
			}
			if (this.mUndying & !this.mDrowning)
			{
				this.mUndieTimer = this.mTemplate.UndieTime;
				if (this.mUndieTimer < 1E-45f)
				{
					this.mUndieTimer = 0.5f;
				}
			}
			else
			{
				this.mUndieTimer = float.NaN;
				Dictionary<int, AnimatedLevelPart> animatedLevelParts = this.mPlayState.Level.CurrentScene.LevelModel.AnimatedLevelParts;
				bool flag = false;
				foreach (AnimatedLevelPart animatedLevelPart in animatedLevelParts.Values)
				{
					if (animatedLevelPart.IsTouchingEntity(base.Handle, true))
					{
						flag = true;
						break;
					}
				}
				if (!flag && this.CharacterBody.IsTouchingGround)
				{
					this.mBody.DisableBody();
				}
			}
			this.StopLevitate();
			this.mHitList.Clear();
			if (!(this.mOverkilled | this.mDrowning))
			{
				EventCondition eventCondition = default(EventCondition);
				eventCondition.EventConditionType = EventConditionType.Death;
				this.mEventConditions.ExecuteAll(this, null, ref eventCondition);
			}
			if (this.mOnDeathTrigger != 0)
			{
				this.mPlayState.Level.CurrentScene.ExecuteTrigger(this.mOnDeathTrigger, this, false);
			}
			this.StopHypnotize();
			this.mDead = true;
			for (int i = 0; i < this.mAttachedEffectsBoneIndex.Length; i++)
			{
				this.mAttachedEffectsBoneIndex[i] = -1;
			}
			if (this.mSpellLight != null)
			{
				this.mSpellLight.Stop(false);
			}
			if (this.mStatusEffectLight != null)
			{
				this.mStatusEffectLight.Stop(false);
				this.mStatusEffectLight = null;
			}
			if (this.mCurrentSpell != null)
			{
				this.mCurrentSpell.DeInitialize(this);
				this.mCurrentSpell = null;
			}
			for (int j = 0; j < this.mAuras.Count; j++)
			{
				VisualEffectReference mEffect = this.mAuras[j].mEffect;
				EffectManager.Instance.Stop(ref mEffect);
			}
			this.mAuras.Clear();
			this.mBuffs.Clear();
			for (int k = 0; k < this.mBuffEffects.Count; k++)
			{
				VisualEffectReference visualEffectReference = this.mBuffEffects[k];
				EffectManager.Instance.Stop(ref visualEffectReference);
			}
			this.mBuffEffects.Clear();
			this.mBuffDecals.Clear();
			EffectManager.Instance.Stop(ref this.mStunEffect);
			EffectManager.Instance.Stop(ref this.mFearEffect);
			EffectManager.Instance.Stop(ref this.mWaterSplashEffect);
			this.EndCharm();
			this.StopHypnotize();
			this.mDeathStatusEffects = this.mCurrentStatusEffects;
			this.mCurrentStatusEffects = StatusEffects.None;
			for (int l = 0; l < 9; l++)
			{
				this.mStatusEffects[l].Stop();
			}
			for (int m = 0; m < this.mStatusEffectCues.Length; m++)
			{
				if (this.mStatusEffectCues[m] != null)
				{
					this.mStatusEffectCues[m].Stop(AudioStopOptions.AsAuthored);
				}
			}
			for (int n = 0; n < this.mAttachedEffects.Length; n++)
			{
				if (this.mAttachedEffects[n].Hash != 0)
				{
					EffectManager.Instance.Stop(ref this.mAttachedEffects[n]);
				}
			}
			if (this.ChargeCue != null)
			{
				this.ChargeCue.Stop(AudioStopOptions.AsAuthored);
			}
			this.mSpellQueue.Clear();
			this.mEntaglement.Release();
			if (this.mDialog != 0 && DialogManager.Instance.DialogActive(this.mDialog))
			{
				DialogManager.Instance.End(this.mDialog);
				DialogManager.Instance.Dialogs.DialogFinished(this.mDialog);
			}
			this.RemoveSelfShield();
			for (int num = 0; num < this.mAttachedSoundCues.Length; num++)
			{
				if (this.mAttachedSoundCues[num] != null && !this.mAttachedSoundCues[num].IsStopping)
				{
					this.mAttachedSoundCues[num].Stop(AudioStopOptions.AsAuthored);
				}
			}
		}

		// Token: 0x06000E38 RID: 3640 RVA: 0x00059250 File Offset: 0x00057450
		public void UpdateDeath(float iDeltaTime)
		{
			if (!this.mDead)
			{
				return;
			}
			this.mTimeDead += iDeltaTime;
			EventCondition eventCondition = default(EventCondition);
			eventCondition.EventConditionType = EventConditionType.Death;
			eventCondition.Time = this.mTimeDead;
			this.mEventConditions.ExecuteAll(this, null, ref eventCondition);
		}

		// Token: 0x06000E39 RID: 3641 RVA: 0x000592A4 File Offset: 0x000574A4
		public void Attack(Animations iAnimation, bool iAllowRotate)
		{
			if (NetworkManager.Instance.State != NetworkState.Offline && (!this.mAttacking || iAnimation != this.mNextAttackAnimation))
			{
				CharacterActionMessage characterActionMessage = default(CharacterActionMessage);
				characterActionMessage.Handle = base.Handle;
				characterActionMessage.Action = ActionType.Attack;
				characterActionMessage.Param0I = (int)iAnimation;
				characterActionMessage.Param1I = (int)this.mCastType;
				characterActionMessage.Param2F = this.mSpellPower;
				if (iAllowRotate)
				{
					characterActionMessage.Param0I |= int.MinValue;
				}
				NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref characterActionMessage);
			}
			this.mNextAttackAnimation = iAnimation;
			this.mAllowAttackRotate = iAllowRotate;
			this.mAttacking = true;
		}

		// Token: 0x06000E3A RID: 3642 RVA: 0x00059349 File Offset: 0x00057549
		public void SpecialAbilityAnimation(Animations iAnimation)
		{
			this.mNextAttackAnimation = iAnimation;
			this.mAllowAttackRotate = false;
			this.mAttacking = true;
		}

		// Token: 0x06000E3B RID: 3643 RVA: 0x00059360 File Offset: 0x00057560
		public void Dash(Animations iAnimation, bool iAllowRotate)
		{
			if (NetworkManager.Instance.State != NetworkState.Offline && (!this.mDashing || iAnimation != this.mNextAttackAnimation))
			{
				CharacterActionMessage characterActionMessage = default(CharacterActionMessage);
				characterActionMessage.Handle = base.Handle;
				characterActionMessage.Action = ActionType.Dash;
				characterActionMessage.Param0I = (int)iAnimation;
				if (iAllowRotate)
				{
					characterActionMessage.Param0I |= int.MinValue;
				}
				NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref characterActionMessage);
			}
			this.mNextAttackAnimation = iAnimation;
			this.mAllowAttackRotate = iAllowRotate;
			this.mDashing = true;
		}

		// Token: 0x06000E3C RID: 3644 RVA: 0x000593EC File Offset: 0x000575EC
		public void DamageGripped(Animations iAnimation)
		{
			if (this.mGripAttack && this.mNextGripAttackAnimation != Animations.None && !(this.mCurrentState is GripAttackState))
			{
				return;
			}
			if (NetworkManager.Instance.State != NetworkState.Offline && (!this.mGripAttack || iAnimation != this.mNextAttackAnimation))
			{
				CharacterActionMessage characterActionMessage = default(CharacterActionMessage);
				characterActionMessage.Handle = base.Handle;
				characterActionMessage.Action = ActionType.GripAttack;
				characterActionMessage.Param0I = (int)iAnimation;
				NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref characterActionMessage);
			}
			this.mNextGripAttackAnimation = iAnimation;
			this.mAllowAttackRotate = false;
			this.mGripAttack = true;
		}

		// Token: 0x06000E3D RID: 3645 RVA: 0x00059480 File Offset: 0x00057680
		protected bool BlockDamage(Damage iDamage, Vector3 iPosition, out float oBlocked)
		{
			float num = 0f;
			oBlocked = 0f;
			for (int i = 0; i < this.mEquipment.Length; i++)
			{
				num += (float)this.mEquipment[i].Item.BlockValue;
			}
			Vector3 forward = this.Body.Orientation.Forward;
			Vector3 position = this.Body.Position;
			Vector3 vector;
			Vector3.Subtract(ref iPosition, ref position, out vector);
			vector.Y = 0f;
			vector.Normalize();
			forward.Y = 0f;
			forward.Normalize();
			float num2 = MagickaMath.Angle(ref forward, ref vector);
			if (num2 <= 1.0995574f)
			{
				oBlocked = num;
				return true;
			}
			return false;
		}

		// Token: 0x06000E3E RID: 3646 RVA: 0x00059530 File Offset: 0x00057730
		public void StopStatusEffects(StatusEffects iEffects)
		{
			for (int i = 0; i < 9; i++)
			{
				if ((short)(iEffects & this.mStatusEffects[i].DamageType) != 0)
				{
					this.mStatusEffects[i].Stop();
					if (i < this.mStatusEffectCues.Length && this.mStatusEffectCues[i] != null)
					{
						this.mStatusEffectCues[i].Stop(AudioStopOptions.AsAuthored);
					}
				}
			}
		}

		// Token: 0x1700037A RID: 890
		// (get) Token: 0x06000E3F RID: 3647 RVA: 0x00059594 File Offset: 0x00057794
		public virtual Spell SpellToCast
		{
			get
			{
				return SpellManager.Instance.Combine(this.SpellQueue);
			}
		}

		// Token: 0x06000E40 RID: 3648 RVA: 0x000595A6 File Offset: 0x000577A6
		public virtual void CombineSpell()
		{
			this.mSpell = SpellManager.Instance.Combine(this.mSpellQueue);
			this.SpellQueue.Clear();
		}

		// Token: 0x06000E41 RID: 3649 RVA: 0x000595CC File Offset: 0x000577CC
		public virtual void CastSpell(bool iFromStaff, string iJoint)
		{
			if (this.mCurrentSpell == null)
			{
				if (this.mSpell.Element == Elements.All)
				{
					if (this.mSpecialAbility == null)
					{
						SpellMagickConverter spellMagickConverter = default(SpellMagickConverter);
						spellMagickConverter.Spell = this.mSpell;
						this.mSpecialAbility = spellMagickConverter.Magick.Effect;
						this.CastType = CastType.None;
					}
					if (this.mSpecialAbility.Execute(this, this.mPlayState) && this is Avatar && (this as Avatar).Player != null && !((this as Avatar).Player.Gamer is NetworkGamer))
					{
						base.PlayState.HasUsedMagick[(this as Avatar).Player.ID] = true;
						AchievementsManager.Instance.AwardAchievement(base.PlayState, "cookingbythebook");
					}
					this.mSpell = default(Spell);
					this.mSpecialAbility = null;
					return;
				}
				if (this is Avatar && (this as Avatar).Player != null && !((this as Avatar).Player.Gamer is NetworkGamer))
				{
					Profile.Instance.UsedElements(base.PlayState, (this as Avatar).Player.GamerTag, this.mSpell.Element);
				}
				this.mSpell.Cast(iFromStaff, this, this.mCastType);
				TelemetryUtils.SendSpellCast(this);
				if (this.IsEntangled && this.mCastType == CastType.Self)
				{
					this.mEntaglement.DecreaseEntanglement(0f, this.mSpell.Element);
				}
				this.mSpell = default(Spell);
			}
		}

		// Token: 0x1700037B RID: 891
		// (get) Token: 0x06000E42 RID: 3650 RVA: 0x0005975D File Offset: 0x0005795D
		// (set) Token: 0x06000E43 RID: 3651 RVA: 0x00059765 File Offset: 0x00057965
		public Magicka.GameLogic.Entities.Abilities.SpecialAbilities.SpecialAbility Magick
		{
			get
			{
				return this.mSpecialAbility;
			}
			set
			{
				this.mSpecialAbility = value;
			}
		}

		// Token: 0x1700037C RID: 892
		// (get) Token: 0x06000E44 RID: 3652 RVA: 0x0005976E File Offset: 0x0005796E
		// (set) Token: 0x06000E45 RID: 3653 RVA: 0x00059776 File Offset: 0x00057976
		public DynamicLight SpellLight
		{
			get
			{
				return this.mSpellLight;
			}
			set
			{
				this.mSpellLight = value;
			}
		}

		// Token: 0x1700037D RID: 893
		// (get) Token: 0x06000E46 RID: 3654 RVA: 0x0005977F File Offset: 0x0005797F
		public int SourceOfSpell
		{
			get
			{
				return this.mSourceOfSpellIndex;
			}
		}

		// Token: 0x1700037E RID: 894
		// (get) Token: 0x06000E47 RID: 3655 RVA: 0x00059787 File Offset: 0x00057987
		public StaticList<Spell> SpellQueue
		{
			get
			{
				return this.mSpellQueue;
			}
		}

		// Token: 0x1700037F RID: 895
		// (get) Token: 0x06000E48 RID: 3656 RVA: 0x0005978F File Offset: 0x0005798F
		// (set) Token: 0x06000E49 RID: 3657 RVA: 0x00059797 File Offset: 0x00057997
		public SpellEffect CurrentSpell
		{
			get
			{
				return this.mCurrentSpell;
			}
			set
			{
				this.mCurrentSpell = value;
				if (value == null)
				{
					this.mCastType = CastType.None;
					this.mTurnSpeed = this.mTurnSpeedMax;
				}
			}
		}

		// Token: 0x17000380 RID: 896
		// (get) Token: 0x06000E4A RID: 3658 RVA: 0x000597B6 File Offset: 0x000579B6
		public Spell Spell
		{
			get
			{
				return this.mSpell;
			}
		}

		// Token: 0x17000381 RID: 897
		// (get) Token: 0x06000E4B RID: 3659 RVA: 0x000597BE File Offset: 0x000579BE
		// (set) Token: 0x06000E4C RID: 3660 RVA: 0x000597C6 File Offset: 0x000579C6
		public virtual CastType CastType
		{
			get
			{
				return this.mCastType;
			}
			set
			{
				if (value != CastType.Weapon & value != CastType.None & value != this.mCastType)
				{
					this.CombineSpell();
				}
				this.mCastType = value;
			}
		}

		// Token: 0x17000382 RID: 898
		// (get) Token: 0x06000E4D RID: 3661
		// (set) Token: 0x06000E4E RID: 3662
		public abstract int Boosts { get; set; }

		// Token: 0x17000383 RID: 899
		// (get) Token: 0x06000E4F RID: 3663
		public abstract float BoostCooldown { get; }

		// Token: 0x17000384 RID: 900
		// (get) Token: 0x06000E50 RID: 3664 RVA: 0x000597F3 File Offset: 0x000579F3
		public Matrix CastSource
		{
			get
			{
				return this.mStaffOrb;
			}
		}

		// Token: 0x17000385 RID: 901
		// (get) Token: 0x06000E51 RID: 3665 RVA: 0x000597FB File Offset: 0x000579FB
		public Matrix WeaponSource
		{
			get
			{
				return this.mWeaponTransform;
			}
		}

		// Token: 0x06000E52 RID: 3666 RVA: 0x00059803 File Offset: 0x00057A03
		public void StopAllActions()
		{
			this.ReleaseAttachedCharacter();
			if (this.mCurrentSpell != null)
			{
				this.mCurrentSpell.Stop(this);
			}
		}

		// Token: 0x06000E53 RID: 3667 RVA: 0x0005981F File Offset: 0x00057A1F
		public void ResetSpell()
		{
			this.mSpell = default(Spell);
		}

		// Token: 0x17000386 RID: 902
		// (get) Token: 0x06000E54 RID: 3668 RVA: 0x0005982D File Offset: 0x00057A2D
		// (set) Token: 0x06000E55 RID: 3669 RVA: 0x00059835 File Offset: 0x00057A35
		public bool NotedKilledEvent
		{
			get
			{
				return this.mNotedKilledEvent;
			}
			set
			{
				this.mNotedKilledEvent = true;
			}
		}

		// Token: 0x06000E56 RID: 3670 RVA: 0x00059840 File Offset: 0x00057A40
		public virtual void Terminate(bool iKillItems, bool iIsKillPlane, bool iNetwork)
		{
			this.mDeadTimer = -100f;
			this.mDead = true;
			this.mHitPoints = 0f;
			if (this.mSpellLight != null)
			{
				this.mSpellLight.Stop(false);
			}
			if (this.mStatusEffectLight != null)
			{
				this.mStatusEffectLight.Stop(false);
				this.mStatusEffectLight = null;
			}
			if (this.mCurrentSpell != null)
			{
				this.mCurrentSpell.DeInitialize(this);
				this.mCurrentSpell = null;
			}
			for (int i = 0; i < this.mAuras.Count; i++)
			{
				VisualEffectReference mEffect = this.mAuras[i].mEffect;
				EffectManager.Instance.Stop(ref mEffect);
			}
			this.mAuras.Clear();
			this.mBuffs.Clear();
			for (int j = 0; j < this.mBuffEffects.Count; j++)
			{
				VisualEffectReference visualEffectReference = this.mBuffEffects[j];
				EffectManager.Instance.Stop(ref visualEffectReference);
			}
			this.mBuffEffects.Clear();
			this.mBuffDecals.Clear();
			EffectManager.Instance.Stop(ref this.mStunEffect);
			EffectManager.Instance.Stop(ref this.mFearEffect);
			EffectManager.Instance.Stop(ref this.mWaterSplashEffect);
			this.EndCharm();
			this.StopHypnotize();
			this.mDeathStatusEffects = this.mCurrentStatusEffects;
			this.mCurrentStatusEffects = StatusEffects.None;
			for (int k = 0; k < 9; k++)
			{
				this.mStatusEffects[k].Stop();
			}
			if (iKillItems)
			{
				for (int l = 0; l < this.mEquipment.Length; l++)
				{
					this.mEquipment[l].Item.Kill();
				}
			}
			if (iIsKillPlane && this.mLastAttacker is Avatar && !((this.mLastAttacker as Avatar).Player.Gamer is NetworkGamer) && (this.mLastDamage & DamageResult.Pushed) == DamageResult.Pushed)
			{
				AchievementsManager.Instance.AwardAchievement(base.PlayState, "wingardiumleviosa");
			}
		}

		// Token: 0x06000E57 RID: 3671 RVA: 0x00059A30 File Offset: 0x00057C30
		public void Terminate(bool iKillItems, bool iNetwork)
		{
			this.Terminate(iKillItems, false, iNetwork);
		}

		// Token: 0x17000387 RID: 903
		// (get) Token: 0x06000E58 RID: 3672 RVA: 0x00059A3B File Offset: 0x00057C3B
		// (set) Token: 0x06000E59 RID: 3673 RVA: 0x00059A48 File Offset: 0x00057C48
		public virtual Vector3 Color
		{
			get
			{
				return this.mMaterial.TintColor;
			}
			set
			{
				this.mMaterial.TintColor = value;
			}
		}

		// Token: 0x17000388 RID: 904
		// (get) Token: 0x06000E5A RID: 3674 RVA: 0x00059A56 File Offset: 0x00057C56
		// (set) Token: 0x06000E5B RID: 3675 RVA: 0x00059A5E File Offset: 0x00057C5E
		public float HeightOffset
		{
			get
			{
				return this.mHeightOffset;
			}
			set
			{
				this.mHeightOffset = value;
			}
		}

		// Token: 0x06000E5C RID: 3676 RVA: 0x00059A68 File Offset: 0x00057C68
		public Vector3 GetRandomPositionOnCollisionSkin()
		{
			Vector3 position = this.Position;
			if (this.mCollision == null)
			{
				return position;
			}
			Vector3 result = default(Vector3);
			float iAngle = MagickaMath.SetBetween(-3.1415927f, 3.1415927f, (float)Character.sRandom.NextDouble());
			float iAngle2 = MagickaMath.SetBetween(-1.5707964f, 1.5707964f, (float)Character.sRandom.NextDouble());
			float num;
			float num2;
			MathApproximation.FastSinCos(iAngle, out num, out num2);
			float num3;
			float num4;
			MathApproximation.FastSinCos(iAngle2, out num3, out num4);
			num2 *= num4;
			num *= num4;
			float radius = (this.mCollision.GetPrimitiveLocal(0) as Capsule).Radius;
			float num5 = (this.mCollision.GetPrimitiveLocal(0) as Capsule).Radius * 2f + (this.mCollision.GetPrimitiveLocal(0) as Capsule).Length;
			result.X = num2 * radius;
			result.Z = num * radius;
			result.Y = num3 * num5 * 0.5f;
			result.X += position.X;
			result.Y += position.Y;
			result.Z += position.Z;
			return result;
		}

		// Token: 0x06000E5D RID: 3677 RVA: 0x00059BA0 File Offset: 0x00057DA0
		public DamageResult AddStatusEffect(StatusEffect iStatusEffect)
		{
			if (this.IsImmortal | this.mEthereal | !this.mCanHasStatusEffect)
			{
				return DamageResult.None;
			}
			DamageResult damageResult = DamageResult.None;
			if (!iStatusEffect.Dead)
			{
				bool flag = false;
				bool flag2 = false;
				StatusEffects damageType = iStatusEffect.DamageType;
				if (damageType <= StatusEffects.Poisoned)
				{
					switch (damageType)
					{
					case StatusEffects.Burning:
					{
						if (!this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Cold)].Dead)
						{
							this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Cold)].Stop();
							flag = true;
						}
						if (!this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Wet)].Dead)
						{
							this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Wet)].Stop();
							if (this.mDryTimer < 0f && this.mStatusEffectCues[StatusEffect.StatusIndex(StatusEffects.Wet)] != null && this.mStatusEffectCues[StatusEffect.StatusIndex(StatusEffects.Wet)].IsPlaying)
							{
								this.mDryTimer = 0.9f;
								Vector3 position = this.Position;
								Vector3 direction = this.Direction;
								EffectManager.Instance.StartEffect(Defines.STATUS_DRYING_EFFECT_HASH, ref position, ref direction, out this.mDryingEffect);
								this.mStatusEffectCues[StatusEffect.StatusIndex(StatusEffects.Wet)].Stop(AudioStopOptions.AsAuthored);
								AudioManager.Instance.PlayCue(Banks.Spells, Defines.STEAM_CUE, base.AudioEmitter);
							}
							flag = true;
						}
						else if (!this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Frozen)].Dead)
						{
							StatusEffect[] array = this.mStatusEffects;
							int num = StatusEffect.StatusIndex(StatusEffects.Frozen);
							array[num].Magnitude = array[num].Magnitude - iStatusEffect.Magnitude;
							Vector3 position2 = this.Position;
							Vector3 direction2 = this.Direction;
							EffectManager.Instance.StartEffect(Defines.STATUS_DRYING_EFFECT_HASH, ref position2, ref direction2, out this.mDryingEffect);
							if (this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Frozen)].Magnitude <= 0f && this.mDryTimer < 0f && this.mStatusEffectCues[StatusEffect.StatusIndex(StatusEffects.Frozen)] != null && this.mStatusEffectCues[StatusEffect.StatusIndex(StatusEffects.Frozen)].IsPlaying)
							{
								this.mStatusEffectCues[StatusEffect.StatusIndex(StatusEffects.Frozen)].Stop(AudioStopOptions.AsAuthored);
								this.mDryTimer = 0.9f;
								EffectManager.Instance.StartEffect(Defines.STATUS_DRYING_EFFECT_HASH, ref position2, ref direction2, out this.mDryingEffect);
								if (this.mStatusEffectCues[StatusEffect.StatusIndex(StatusEffects.Wet)] != null)
								{
									this.mStatusEffectCues[StatusEffect.StatusIndex(StatusEffects.Wet)].Stop(AudioStopOptions.AsAuthored);
								}
								AudioManager.Instance.PlayCue(Banks.Spells, Defines.STEAM_CUE, base.AudioEmitter);
							}
							flag = true;
						}
						if (this.HasStatus(StatusEffects.Greased))
						{
							iStatusEffect.DPS *= 4f;
							int num2 = StatusEffect.StatusIndex(StatusEffects.Greased);
							this.mStatusEffects[num2].Stop();
							this.mStatusEffects[num2] = default(StatusEffect);
						}
						Elements elements = Elements.Fire;
						int num3 = Spell.ElementIndex(elements);
						float num4 = this.mResistances[num3].Multiplier;
						float num5 = this.mResistances[num3].Modifier;
						if (this is Avatar)
						{
							num4 *= this.Equipment[1].Item.Resistance[num3].Multiplier;
							num5 += this.Equipment[1].Item.Resistance[num3].Modifier;
						}
						for (int i = 0; i < this.mBuffs.Count; i++)
						{
							BuffStorage buffStorage = this.mBuffs[i];
							if (buffStorage.BuffType == BuffType.Resistance && (buffStorage.BuffResistance.Resistance.ResistanceAgainst & elements) == elements)
							{
								num4 *= buffStorage.BuffResistance.Resistance.Multiplier;
								num5 += buffStorage.BuffResistance.Resistance.Modifier;
							}
						}
						iStatusEffect.DPS = (iStatusEffect.DPS + num5) * num4;
						if (Math.Abs(iStatusEffect.DPS) < 1E-45f || Math.Abs(iStatusEffect.Magnitude) < 1E-45f)
						{
							flag2 = true;
						}
						if (this.mResistances[num3].StatusResistance)
						{
							flag2 = true;
						}
						break;
					}
					case StatusEffects.Wet:
					{
						if (this.HasStatus(StatusEffects.Burning) | this.mDryTimer > 0f)
						{
							int num6 = StatusEffect.StatusIndex(StatusEffects.Burning);
							if (this.mStatusEffectCues[num6] != null && this.mStatusEffectCues[num6].IsPlaying)
							{
								this.mStatusEffectCues[num6].Stop(AudioStopOptions.AsAuthored);
								this.mDryTimer = 0.9f;
								Vector3 position3 = this.Position;
								Vector3 direction3 = this.Direction;
								EffectManager.Instance.StartEffect(Defines.STATUS_DRYING_EFFECT_HASH, ref position3, ref direction3, out this.mDryingEffect);
								AudioManager.Instance.PlayCue(Banks.Spells, Defines.STEAM_CUE, base.AudioEmitter);
							}
							this.mStatusEffects[num6].Stop();
							this.mStatusEffects[num6] = default(StatusEffect);
							flag = true;
						}
						if (this.HasStatus(StatusEffects.Greased))
						{
							int num7 = StatusEffect.StatusIndex(StatusEffects.Greased);
							this.mStatusEffects[num7].Stop();
							this.mStatusEffects[num7] = default(StatusEffect);
						}
						Elements elements2 = Elements.Water;
						int num8 = Spell.ElementIndex(elements2);
						float num9 = this.mResistances[num8].Multiplier;
						float num10 = this.mResistances[num8].Modifier;
						if (this is Avatar)
						{
							num9 *= this.Equipment[1].Item.Resistance[num8].Multiplier;
							num10 += this.Equipment[1].Item.Resistance[num8].Modifier;
						}
						for (int j = 0; j < this.mBuffs.Count; j++)
						{
							BuffStorage buffStorage2 = this.mBuffs[j];
							if (buffStorage2.BuffType == BuffType.Resistance && (buffStorage2.BuffResistance.Resistance.ResistanceAgainst & elements2) == elements2)
							{
								num9 *= buffStorage2.BuffResistance.Resistance.Multiplier;
								num10 += buffStorage2.BuffResistance.Resistance.Modifier;
							}
						}
						iStatusEffect.DPS += num10;
						iStatusEffect.Magnitude *= num9;
						if (Math.Abs(iStatusEffect.Magnitude) < 1E-45f)
						{
							flag2 = true;
						}
						if (this.mResistances[num8].StatusResistance)
						{
							flag2 = true;
						}
						break;
					}
					default:
						if (damageType != StatusEffects.Cold)
						{
							if (damageType == StatusEffects.Poisoned)
							{
								Elements elements3 = Elements.Poison;
								int num11 = Spell.ElementIndex(elements3);
								float num12 = this.mResistances[num11].Multiplier;
								float num13 = this.mResistances[num11].Modifier;
								if (this is Avatar)
								{
									num12 *= this.Equipment[1].Item.Resistance[num11].Multiplier;
									num13 += this.Equipment[1].Item.Resistance[num11].Modifier;
								}
								for (int k = 0; k < this.mBuffs.Count; k++)
								{
									BuffStorage buffStorage3 = this.mBuffs[k];
									if (buffStorage3.BuffType == BuffType.Resistance && (buffStorage3.BuffResistance.Resistance.ResistanceAgainst & elements3) == elements3)
									{
										num12 *= buffStorage3.BuffResistance.Resistance.Multiplier;
										num13 += buffStorage3.BuffResistance.Resistance.Modifier;
									}
								}
								iStatusEffect.DPS += num13;
								iStatusEffect.Magnitude *= num12;
								if (Math.Abs(iStatusEffect.DPS) < 1E-45f || Math.Abs(iStatusEffect.Magnitude) < 1E-45f)
								{
									flag2 = true;
								}
								if (this.mResistances[num11].StatusResistance)
								{
									flag2 = true;
								}
							}
						}
						else
						{
							float num14 = iStatusEffect.Magnitude;
							if (!this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Burning)].Dead || this.mDryTimer > 0f)
							{
								if (this.mStatusEffectCues[StatusEffect.StatusIndex(StatusEffects.Burning)] != null && this.mStatusEffectCues[StatusEffect.StatusIndex(StatusEffects.Burning)].IsPlaying)
								{
									this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Burning)].Stop();
									this.mDryTimer = 0.9f;
									Vector3 position4 = this.Position;
									Vector3 direction4 = this.Direction;
									EffectManager.Instance.StartEffect(Defines.STATUS_DRYING_EFFECT_HASH, ref position4, ref direction4, out this.mDryingEffect);
									this.mStatusEffectCues[StatusEffect.StatusIndex(StatusEffects.Burning)].Stop(AudioStopOptions.AsAuthored);
									AudioManager.Instance.PlayCue(Banks.Spells, Defines.STEAM_CUE, base.AudioEmitter);
								}
								flag = true;
							}
							num14 = 1f;
							Elements iElement = Elements.Ice;
							int num15 = Spell.ElementIndex(iElement);
							if (this.HasStatus(StatusEffects.Wet) && !this.mResistances[num15].StatusResistance)
							{
								if (this.mCurrentSpell != null)
								{
									this.mCurrentSpell.Stop(this);
								}
								int num16 = StatusEffect.StatusIndex(StatusEffects.Frozen);
								this.mStatusEffects[num16] = new StatusEffect(StatusEffects.Frozen, 0f, num14, this.Capsule.Length, this.Capsule.Radius);
								this.mCurrentStatusEffects |= StatusEffects.Frozen;
								if (this.mStatusEffectCues[num16] == null || (this.mStatusEffectCues[num16] != null && !this.mStatusEffectCues[num16].IsPlaying))
								{
									this.mStatusEffectCues[num16] = AudioManager.Instance.PlayCue(Banks.Spells, "spell_ice_frozen".GetHashCodeCustom(), base.AudioEmitter);
								}
								num16 = StatusEffect.StatusIndex(StatusEffects.Wet);
								if (this.mStatusEffectCues[num16] != null && this.mStatusEffectCues[num16].IsPlaying)
								{
									this.mStatusEffectCues[num16].Stop(AudioStopOptions.AsAuthored);
								}
								this.mStatusEffects[num16].Stop();
								this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Cold)].Stop();
								flag = true;
							}
							else if (this.HasStatus(StatusEffects.Frozen))
							{
								this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Frozen)].Magnitude = num14;
								flag = true;
							}
							else
							{
								Elements elements4 = Elements.Cold;
								int num17 = Spell.ElementIndex(elements4);
								float num18 = this.mResistances[num17].Multiplier;
								float num19 = this.mResistances[num17].Modifier;
								if (this is Avatar)
								{
									num18 *= this.Equipment[1].Item.Resistance[num17].Multiplier;
									num19 += this.Equipment[1].Item.Resistance[num17].Modifier;
								}
								for (int l = 0; l < this.mBuffs.Count; l++)
								{
									BuffStorage buffStorage4 = this.mBuffs[l];
									if (buffStorage4.BuffType == BuffType.Resistance && (buffStorage4.BuffResistance.Resistance.ResistanceAgainst & elements4) == elements4)
									{
										num18 *= buffStorage4.BuffResistance.Resistance.Multiplier;
										num19 += buffStorage4.BuffResistance.Resistance.Modifier;
									}
								}
								iStatusEffect.DPS += num19;
								iStatusEffect.Magnitude *= num18;
								if (Math.Abs(iStatusEffect.Magnitude) < 1E-45f)
								{
									flag2 = true;
								}
								if (this.mResistances[num17].StatusResistance)
								{
									flag2 = true;
								}
							}
						}
						break;
					}
				}
				else if (damageType <= StatusEffects.Greased)
				{
					if (damageType != StatusEffects.Healing)
					{
						if (damageType == StatusEffects.Greased)
						{
							if (this.HasStatus(StatusEffects.Wet))
							{
								int num20 = StatusEffect.StatusIndex(StatusEffects.Wet);
								this.mStatusEffects[num20].Stop();
								this.mStatusEffects[num20] = default(StatusEffect);
							}
							Elements elements5 = Elements.Water;
							int num21 = Spell.ElementIndex(elements5);
							float num22 = this.mResistances[num21].Multiplier;
							float num23 = this.mResistances[num21].Modifier;
							if (this is Avatar)
							{
								num22 *= this.Equipment[1].Item.Resistance[num21].Multiplier;
								num23 += this.Equipment[1].Item.Resistance[num21].Modifier;
							}
							for (int m = 0; m < this.mBuffs.Count; m++)
							{
								BuffStorage buffStorage5 = this.mBuffs[m];
								if (buffStorage5.BuffType == BuffType.Resistance && (buffStorage5.BuffResistance.Resistance.ResistanceAgainst & elements5) == elements5)
								{
									num22 *= buffStorage5.BuffResistance.Resistance.Multiplier;
									num23 += buffStorage5.BuffResistance.Resistance.Modifier;
								}
							}
							iStatusEffect.DPS += num23;
							iStatusEffect.Magnitude *= num22;
							if (Math.Abs(iStatusEffect.DPS) < 1E-45f || Math.Abs(iStatusEffect.Magnitude) < 1E-45f)
							{
								flag2 = true;
							}
						}
					}
					else
					{
						if (this.HasStatus(StatusEffects.Poisoned))
						{
							this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Poisoned)].Stop();
							this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Poisoned)] = default(StatusEffect);
						}
						if (this.HasStatus(StatusEffects.Bleeding))
						{
							this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Bleeding)].Stop();
							this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Bleeding)] = default(StatusEffect);
						}
						Elements elements6 = Elements.Life;
						int num24 = Spell.ElementIndex(elements6);
						float num25 = this.mResistances[num24].Multiplier;
						float num26 = this.mResistances[num24].Modifier;
						if (this is Avatar)
						{
							num25 *= this.Equipment[1].Item.Resistance[num24].Multiplier;
							num26 += this.Equipment[1].Item.Resistance[num24].Modifier;
						}
						for (int n = 0; n < this.mBuffs.Count; n++)
						{
							BuffStorage buffStorage6 = this.mBuffs[n];
							if (buffStorage6.BuffType == BuffType.Resistance && (buffStorage6.BuffResistance.Resistance.ResistanceAgainst & elements6) == elements6)
							{
								num25 *= buffStorage6.BuffResistance.Resistance.Multiplier;
								num26 += buffStorage6.BuffResistance.Resistance.Modifier;
							}
						}
						iStatusEffect.DPS += num26;
						iStatusEffect.Magnitude *= num25;
						if (Math.Abs(iStatusEffect.DPS) < 1E-45f || Math.Abs(iStatusEffect.Magnitude) < 1E-45f)
						{
							flag2 = true;
						}
						if (this.mResistances[num24].StatusResistance)
						{
							flag2 = true;
						}
					}
				}
				else if (damageType != StatusEffects.Steamed)
				{
					if (damageType != StatusEffects.Bleeding)
					{
					}
				}
				else
				{
					if (this.HasStatus(StatusEffects.Cold))
					{
						int num27 = StatusEffect.StatusIndex(StatusEffects.Cold);
						this.mStatusEffects[num27].Stop();
						this.mStatusEffects[num27] = default(StatusEffect);
					}
					Elements elements7 = Elements.Steam;
					int num28 = Spell.ElementIndex(elements7);
					float num29 = this.mResistances[num28].Multiplier;
					float num30 = this.mResistances[num28].Modifier;
					if (this is Avatar)
					{
						num29 *= this.Equipment[1].Item.Resistance[num28].Multiplier;
						num30 += this.Equipment[1].Item.Resistance[num28].Modifier;
					}
					for (int num31 = 0; num31 < this.mBuffs.Count; num31++)
					{
						BuffStorage buffStorage7 = this.mBuffs[num31];
						if (buffStorage7.BuffType == BuffType.Resistance && (buffStorage7.BuffResistance.Resistance.ResistanceAgainst & elements7) == elements7)
						{
							num29 *= buffStorage7.BuffResistance.Resistance.Multiplier;
							num30 += buffStorage7.BuffResistance.Resistance.Modifier;
						}
					}
					iStatusEffect.DPS += num30;
					iStatusEffect.Magnitude *= num29;
					if (Math.Abs(iStatusEffect.DPS) < 1E-45f || Math.Abs(iStatusEffect.Magnitude) < 1E-45f)
					{
						flag2 = true;
					}
					if (this.mResistances[num28].StatusResistance)
					{
						flag2 = true;
					}
				}
				if (!flag && !flag2)
				{
					int num32 = StatusEffect.StatusIndex(iStatusEffect.DamageType);
					if (iStatusEffect.DamageType == StatusEffects.Burning && !this.HasStatus(StatusEffects.Burning))
					{
						int num33 = StatusEffect.StatusIndex(StatusEffects.Burning);
						if (this.mStatusEffectCues[num33] == null || (this.mStatusEffectCues[num33] != null && !this.mStatusEffectCues[num33].IsPlaying))
						{
							this.mStatusEffectCues[num33] = AudioManager.Instance.PlayCue(Banks.Spells, "spell_fire_onfire".GetHashCodeCustom(), this.mAudioEmitter);
						}
					}
					if (iStatusEffect.DamageType == StatusEffects.Wet && !this.HasStatus(StatusEffects.Wet))
					{
						int num34 = StatusEffect.StatusIndex(StatusEffects.Wet);
						if (this.mStatusEffectCues[num34] == null || (this.mStatusEffectCues[num34] != null && !this.mStatusEffectCues[num34].IsPlaying))
						{
							this.mStatusEffectCues[num34] = AudioManager.Instance.PlayCue(Banks.Spells, "spell_water_drip".GetHashCodeCustom(), this.mAudioEmitter);
						}
					}
					if (iStatusEffect.DamageType == StatusEffects.Frozen && !this.HasStatus(StatusEffects.Frozen))
					{
						int num35 = StatusEffect.StatusIndex(StatusEffects.Frozen);
						if (this.mStatusEffectCues[num35] == null || (this.mStatusEffectCues[num35] != null && !this.mStatusEffectCues[num35].IsPlaying))
						{
							this.mStatusEffectCues[num35] = AudioManager.Instance.PlayCue(Banks.Spells, "spell_ice_frozen".GetHashCodeCustom(), this.mAudioEmitter);
						}
					}
					this.mStatusEffects[num32] = this.mStatusEffects[num32] + iStatusEffect;
					damageResult |= DamageResult.Statusadded;
				}
				else if (flag)
				{
					damageResult |= DamageResult.Statusremoved;
				}
				else if (flag2)
				{
					damageResult = damageResult;
				}
			}
			StatusEffects statusEffects = StatusEffects.None;
			int num36 = 1;
			int num37 = 0;
			while (num36 < 9)
			{
				if (!this.mStatusEffects[num37].Dead)
				{
					statusEffects |= (StatusEffects)num36;
				}
				num36 <<= 1;
				num37++;
			}
			this.mCurrentStatusEffects = statusEffects;
			return damageResult;
		}

		// Token: 0x17000389 RID: 905
		// (get) Token: 0x06000E5E RID: 3678 RVA: 0x0005AE05 File Offset: 0x00059005
		public float BreakFreeStrength
		{
			get
			{
				return this.mBreakFreeStrength;
			}
		}

		// Token: 0x1700038A RID: 906
		// (get) Token: 0x06000E5F RID: 3679 RVA: 0x0005AE0D File Offset: 0x0005900D
		// (set) Token: 0x06000E60 RID: 3680 RVA: 0x0005AE15 File Offset: 0x00059015
		public int Dialog
		{
			get
			{
				return this.mDialog;
			}
			set
			{
				this.mDialog = value;
			}
		}

		// Token: 0x06000E61 RID: 3681 RVA: 0x0005AE20 File Offset: 0x00059020
		internal bool Interact(Character iCaller, Controller iSender)
		{
			if (this.mDialog == 0 || this.mCurrentState is DeadState)
			{
				return false;
			}
			Dialog dialog = DialogManager.Instance.Dialogs[this.mDialog];
			Interact interact = dialog.Peek();
			if (interact != null)
			{
				bool turnToTarget = interact[interact.Current].TurnToTarget;
				if (turnToTarget)
				{
					Vector3 position = this.Position;
					Vector3 position2 = iCaller.Position;
					Vector3 desiredDirection;
					Vector3.Subtract(ref position2, ref position, out desiredDirection);
					this.CharacterBody.DesiredDirection = desiredDirection;
				}
			}
			DialogManager.Instance.StartDialog(this.mDialog, this, iSender);
			return true;
		}

		// Token: 0x1700038B RID: 907
		// (get) Token: 0x06000E62 RID: 3682 RVA: 0x0005AEB3 File Offset: 0x000590B3
		public InteractType InteractText
		{
			get
			{
				if (this.mDialog == 0 || this.mCurrentState is DeadState)
				{
					return InteractType.None;
				}
				return DialogManager.Instance.GetDialogIconText(this.mDialog);
			}
		}

		// Token: 0x06000E63 RID: 3683 RVA: 0x0005AEDC File Offset: 0x000590DC
		public virtual void Entangle(float iMagnitude)
		{
			if (this.IsImmortal || this.HasStatus(StatusEffects.Burning))
			{
				return;
			}
			NetworkState state = NetworkManager.Instance.State;
			if (state == NetworkState.Server)
			{
				CharacterActionMessage characterActionMessage = default(CharacterActionMessage);
				characterActionMessage.Action = ActionType.Entangle;
				characterActionMessage.Handle = base.Handle;
				characterActionMessage.Param0I = 0;
				characterActionMessage.Param1F = iMagnitude;
				NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref characterActionMessage);
			}
			if (state != NetworkState.Client)
			{
				if (this.mEntaglement.Magnitude <= 1E-45f)
				{
					this.mEntaglement.Initialize();
				}
				this.mEntaglement.AddEntanglement(iMagnitude);
			}
		}

		// Token: 0x06000E64 RID: 3684 RVA: 0x0005AF76 File Offset: 0x00059176
		public void ReleaseEntanglement()
		{
			if (this.mEntaglement.Magnitude > 0f)
			{
				this.mEntaglement.Release();
			}
		}

		// Token: 0x1700038C RID: 908
		// (get) Token: 0x06000E65 RID: 3685 RVA: 0x0005AF95 File Offset: 0x00059195
		public virtual bool IsEntangled
		{
			get
			{
				return this.mEntaglement.Magnitude > float.Epsilon;
			}
		}

		// Token: 0x06000E66 RID: 3686 RVA: 0x0005AFA9 File Offset: 0x000591A9
		public virtual void DamageEntanglement(float iAmount, Elements iElement)
		{
			if (this.mEntaglement.Magnitude > 0f)
			{
				this.mEntaglement.DecreaseEntanglement(iAmount, iElement);
			}
		}

		// Token: 0x06000E67 RID: 3687 RVA: 0x0005AFCA File Offset: 0x000591CA
		public bool HasGibs()
		{
			return this.mGibs.Count > 0 && GlobalSettings.Instance.BloodAndGore == SettingOptions.On;
		}

		// Token: 0x06000E68 RID: 3688 RVA: 0x0005AFEC File Offset: 0x000591EC
		public void SpawnGibs()
		{
			Vector3 position = this.Position;
			bool flag = (short)(this.mDeathStatusEffects & StatusEffects.Frozen) == 4;
			for (int i = 0; i < this.mGibs.Count; i++)
			{
				Gib fromCache = Gib.GetFromCache();
				if (fromCache == null)
				{
					break;
				}
				Vector3 randomPositionOnCollisionSkin = this.GetRandomPositionOnCollisionSkin();
				randomPositionOnCollisionSkin.Y += ((fromCache.Radius >= 0.1f) ? fromCache.Radius : 0.1f);
				float scaleFactor = 5f;
				float num = (this.Radius >= 0.2f) ? this.Radius : 0.2f;
				float num2 = 6.2831855f * (float)Character.sRandom.NextDouble();
				float num3 = (float)Math.Acos((double)(2f * (float)Character.sRandom.NextDouble() - 1f));
				Vector3 vector = new Vector3(num * (float)Math.Cos((double)num2) * (float)Math.Sin((double)num3), num * (float)Math.Sin((double)num2) * (float)Math.Sin((double)num3), num * (float)Math.Cos((double)num3));
				Vector3.Normalize(ref vector, out vector);
				Vector3 iVelocity;
				Vector3.Multiply(ref vector, scaleFactor, out iVelocity);
				iVelocity.Y = Math.Abs(iVelocity.Y);
				float iTTL = 10f + (float)Character.sRandom.NextDouble() * 10f;
				fromCache.Initialize(this.mGibs[i].mModel, this.mGibs[i].mMass, this.mGibs[i].mScale, randomPositionOnCollisionSkin, iVelocity, iTTL, this, this.mBlood, Gib.GORE_GIB_TRAIL_EFFECTS[(int)this.mBlood], flag);
				Vector3 angularVelocity = default(Vector3);
				Vector3.Negate(ref vector, out angularVelocity);
				Vector3.Multiply(ref angularVelocity, 5f, out angularVelocity);
				fromCache.Body.AngularVelocity = angularVelocity;
				this.mPlayState.EntityManager.AddEntity(fromCache);
			}
			Vector3 direction = this.Direction;
			if (flag)
			{
				VisualEffectReference visualEffectReference;
				EffectManager.Instance.StartEffect(Barrier.Ice_Barrier_Death_Effect_Hash, ref position, ref direction, out visualEffectReference);
			}
			else if (this.mBlood != BloodType.none)
			{
				if (this.mBody.Mass < 250f)
				{
					VisualEffectReference visualEffectReference;
					EffectManager.Instance.StartEffect(Gib.GORE_GIB_SMALL_EFFECTS[(int)this.mBlood], ref position, ref direction, out visualEffectReference);
				}
				int num4;
				if (this.mBody.Mass > 1000f)
				{
					num4 = 6;
					VisualEffectReference visualEffectReference;
					EffectManager.Instance.StartEffect(Gib.GORE_GIB_LARGE_EFFECTS[(int)this.mBlood], ref position, ref direction, out visualEffectReference);
				}
				else
				{
					num4 = 4;
					VisualEffectReference visualEffectReference;
					EffectManager.Instance.StartEffect(Gib.GORE_GIB_MEDIUM_EFFECTS[(int)this.mBlood], ref position, ref direction, out visualEffectReference);
				}
				Segment iSeg = default(Segment);
				for (int j = 0; j < num4; j++)
				{
					iSeg.Origin = this.Position + new Vector3((float)(MagickaMath.Random.NextDouble() * (double)this.Radius * 2.0), 0f, (float)(MagickaMath.Random.NextDouble() * (double)this.Radius * 2.0));
					iSeg.Delta.Y = -8f;
					iSeg.Delta.X = (float)((MagickaMath.Random.NextDouble() - 0.5) * 4.0);
					iSeg.Delta.Z = (float)((MagickaMath.Random.NextDouble() - 0.5) * 4.0);
					float num5;
					Vector3 vector2;
					Vector3 vector3;
					AnimatedLevelPart iAnimation;
					if (this.mPlayState.Level.CurrentScene.SegmentIntersect(out num5, out vector2, out vector3, out iAnimation, iSeg))
					{
						DecalManager.Instance.AddAlphaBlendedDecal((Decal)(this.mBlood * BloodType.insect + MagickaMath.Random.Next(0, 3)), iAnimation, 3f + (float)(MagickaMath.Random.NextDouble() * 3.0), ref vector2, ref vector3, 60f);
					}
				}
			}
			this.mDeadTimer = -100f;
		}

		// Token: 0x1700038D RID: 909
		// (get) Token: 0x06000E69 RID: 3689 RVA: 0x0005B3DE File Offset: 0x000595DE
		public Factions GetOriginalFaction
		{
			get
			{
				return this.mFaction;
			}
		}

		// Token: 0x1700038E RID: 910
		// (get) Token: 0x06000E6A RID: 3690 RVA: 0x0005B3E6 File Offset: 0x000595E6
		// (set) Token: 0x06000E6B RID: 3691 RVA: 0x0005B3EE File Offset: 0x000595EE
		public virtual Factions Faction
		{
			get
			{
				return this.mFaction;
			}
			set
			{
				this.mFaction = value;
			}
		}

		// Token: 0x06000E6C RID: 3692 RVA: 0x0005B3F7 File Offset: 0x000595F7
		public void ForceDraw()
		{
			this.mBoundingScale = 40f;
		}

		// Token: 0x1700038F RID: 911
		// (get) Token: 0x06000E6D RID: 3693 RVA: 0x0005B404 File Offset: 0x00059604
		public float Panic
		{
			get
			{
				return this.mMaxPanic;
			}
		}

		// Token: 0x06000E6E RID: 3694 RVA: 0x0005B40C File Offset: 0x0005960C
		public void OrderPanic()
		{
			this.ChangeState(PanicState.Instance);
			this.mPanic = 10f;
		}

		// Token: 0x17000390 RID: 912
		// (get) Token: 0x06000E6F RID: 3695 RVA: 0x0005B425 File Offset: 0x00059625
		public virtual int Type
		{
			get
			{
				return this.mType;
			}
		}

		// Token: 0x17000391 RID: 913
		// (get) Token: 0x06000E70 RID: 3696 RVA: 0x0005B42D File Offset: 0x0005962D
		public string Name
		{
			get
			{
				return this.mName;
			}
		}

		// Token: 0x17000392 RID: 914
		// (get) Token: 0x06000E71 RID: 3697 RVA: 0x0005B435 File Offset: 0x00059635
		public bool CanSeeInvisible
		{
			get
			{
				return this.mTemplate.CanSeeInvisible;
			}
		}

		// Token: 0x17000393 RID: 915
		// (get) Token: 0x06000E72 RID: 3698 RVA: 0x0005B442 File Offset: 0x00059642
		public int DisplayName
		{
			get
			{
				return this.mDisplayName;
			}
		}

		// Token: 0x17000394 RID: 916
		// (get) Token: 0x06000E73 RID: 3699 RVA: 0x0005B44A File Offset: 0x0005964A
		// (set) Token: 0x06000E74 RID: 3700 RVA: 0x0005B452 File Offset: 0x00059652
		public float SpellCooldown { get; set; }

		// Token: 0x17000395 RID: 917
		// (get) Token: 0x06000E75 RID: 3701 RVA: 0x0005B45B File Offset: 0x0005965B
		// (set) Token: 0x06000E76 RID: 3702 RVA: 0x0005B463 File Offset: 0x00059663
		public float WanderAngle
		{
			get
			{
				return this.mWanderAngle;
			}
			set
			{
				this.mWanderAngle = value;
			}
		}

		// Token: 0x17000396 RID: 918
		// (get) Token: 0x06000E77 RID: 3703 RVA: 0x0005B46C File Offset: 0x0005966C
		public Resistance[] Resistance
		{
			get
			{
				return this.mResistances;
			}
		}

		// Token: 0x17000397 RID: 919
		// (get) Token: 0x06000E78 RID: 3704 RVA: 0x0005B474 File Offset: 0x00059674
		public float NormalizedHitPoints
		{
			get
			{
				return this.mNormalizedHitPoints;
			}
		}

		// Token: 0x17000398 RID: 920
		// (get) Token: 0x06000E79 RID: 3705 RVA: 0x0005B47C File Offset: 0x0005967C
		// (set) Token: 0x06000E7A RID: 3706 RVA: 0x0005B484 File Offset: 0x00059684
		public bool Dashing
		{
			get
			{
				return this.mDashing;
			}
			set
			{
				this.mDashing = value;
			}
		}

		// Token: 0x17000399 RID: 921
		// (get) Token: 0x06000E7B RID: 3707 RVA: 0x0005B48D File Offset: 0x0005968D
		// (set) Token: 0x06000E7C RID: 3708 RVA: 0x0005B495 File Offset: 0x00059695
		public bool Attacking
		{
			get
			{
				return this.mAttacking;
			}
			set
			{
				this.mAttacking = value;
			}
		}

		// Token: 0x1700039A RID: 922
		// (get) Token: 0x06000E7D RID: 3709 RVA: 0x0005B49E File Offset: 0x0005969E
		public Animations SpecialAttackAnimation
		{
			get
			{
				return this.mSpawnAnimation;
			}
		}

		// Token: 0x1700039B RID: 923
		// (get) Token: 0x06000E7E RID: 3710 RVA: 0x0005B4A6 File Offset: 0x000596A6
		// (set) Token: 0x06000E7F RID: 3711 RVA: 0x0005B4AE File Offset: 0x000596AE
		public Animations NextAttackAnimation
		{
			get
			{
				return this.mNextAttackAnimation;
			}
			set
			{
				this.mNextAttackAnimation = value;
			}
		}

		// Token: 0x1700039C RID: 924
		// (get) Token: 0x06000E80 RID: 3712 RVA: 0x0005B4B7 File Offset: 0x000596B7
		// (set) Token: 0x06000E81 RID: 3713 RVA: 0x0005B4BF File Offset: 0x000596BF
		public Animations NextGripAttackAnimation
		{
			get
			{
				return this.mNextGripAttackAnimation;
			}
			set
			{
				this.mNextGripAttackAnimation = value;
			}
		}

		// Token: 0x1700039D RID: 925
		// (get) Token: 0x06000E82 RID: 3714 RVA: 0x0005B4C8 File Offset: 0x000596C8
		// (set) Token: 0x06000E83 RID: 3715 RVA: 0x0005B4D0 File Offset: 0x000596D0
		public Animations NextDashAnimation
		{
			get
			{
				return this.mNextDashAnimation;
			}
			set
			{
				this.mNextDashAnimation = value;
			}
		}

		// Token: 0x1700039E RID: 926
		// (get) Token: 0x06000E84 RID: 3716 RVA: 0x0005B4D9 File Offset: 0x000596D9
		public Attachment[] Equipment
		{
			get
			{
				return this.mEquipment;
			}
		}

		// Token: 0x1700039F RID: 927
		// (get) Token: 0x06000E85 RID: 3717 RVA: 0x0005B4E1 File Offset: 0x000596E1
		public MovementProperties MoveAbilities
		{
			get
			{
				return this.mMoveAbilities;
			}
		}

		// Token: 0x170003A0 RID: 928
		// (get) Token: 0x06000E86 RID: 3718 RVA: 0x0005B4E9 File Offset: 0x000596E9
		public Dictionary<byte, Animations[]> MoveAnimations
		{
			get
			{
				return this.mMoveAnimations;
			}
		}

		// Token: 0x170003A1 RID: 929
		// (get) Token: 0x06000E87 RID: 3719 RVA: 0x0005B4F1 File Offset: 0x000596F1
		public override bool Removable
		{
			get
			{
				return this.Dead && this.mDeadTimer < -10f && this.mRemoveAfterDeath && !this.mCannotDieWithoutExplicitKill;
			}
		}

		// Token: 0x170003A2 RID: 930
		// (get) Token: 0x06000E89 RID: 3721 RVA: 0x0005B524 File Offset: 0x00059724
		// (set) Token: 0x06000E88 RID: 3720 RVA: 0x0005B51B File Offset: 0x0005971B
		public bool RemoveAfterDeath
		{
			get
			{
				return this.mRemoveAfterDeath;
			}
			set
			{
				this.mRemoveAfterDeath = value;
			}
		}

		// Token: 0x170003A3 RID: 931
		// (get) Token: 0x06000E8A RID: 3722 RVA: 0x0005B52C File Offset: 0x0005972C
		public bool BloatKilled
		{
			get
			{
				return this.mBloatKilled && GlobalSettings.Instance.BloodAndGore == SettingOptions.On;
			}
		}

		// Token: 0x170003A4 RID: 932
		// (get) Token: 0x06000E8B RID: 3723 RVA: 0x0005B545 File Offset: 0x00059745
		public bool Overkilled
		{
			get
			{
				return (this.mHitPoints <= this.mMaxHitPoints * -0.5f - 500f || this.mBloatKilled) && GlobalSettings.Instance.BloodAndGore == SettingOptions.On;
			}
		}

		// Token: 0x06000E8C RID: 3724 RVA: 0x0005B578 File Offset: 0x00059778
		public bool OverKilled(float iAmount)
		{
			return (iAmount <= this.mMaxHitPoints * -0.5f - 500f || this.mBloatKilled) && GlobalSettings.Instance.BloodAndGore == SettingOptions.On;
		}

		// Token: 0x170003A5 RID: 933
		// (get) Token: 0x06000E8D RID: 3725 RVA: 0x0005B5A6 File Offset: 0x000597A6
		public bool AllowAttackRotate
		{
			get
			{
				return this.mAllowAttackRotate;
			}
		}

		// Token: 0x170003A6 RID: 934
		// (get) Token: 0x06000E8E RID: 3726 RVA: 0x0005B5AE File Offset: 0x000597AE
		// (set) Token: 0x06000E8F RID: 3727 RVA: 0x0005B5B6 File Offset: 0x000597B6
		public bool IsKnockedDown
		{
			get
			{
				return this.mKnockedDown;
			}
			set
			{
				this.mKnockedDown = value;
			}
		}

		// Token: 0x170003A7 RID: 935
		// (get) Token: 0x06000E90 RID: 3728 RVA: 0x0005B5BF File Offset: 0x000597BF
		// (set) Token: 0x06000E91 RID: 3729 RVA: 0x0005B5C7 File Offset: 0x000597C7
		public bool IsHit
		{
			get
			{
				return this.mIsHit;
			}
			set
			{
				this.mIsHit = value;
			}
		}

		// Token: 0x170003A8 RID: 936
		// (get) Token: 0x06000E92 RID: 3730 RVA: 0x0005B5D0 File Offset: 0x000597D0
		public bool IsPanicing
		{
			get
			{
				return this.mPanic - this.mMaxPanic > float.Epsilon;
			}
		}

		// Token: 0x170003A9 RID: 937
		// (get) Token: 0x06000E93 RID: 3731 RVA: 0x0005B5E6 File Offset: 0x000597E6
		public bool IsStumbling
		{
			get
			{
				return this.mGripper != null && this.mGripType == Grip.GripType.Ride;
			}
		}

		// Token: 0x170003AA RID: 938
		// (get) Token: 0x06000E94 RID: 3732
		public abstract bool IsAggressive { get; }

		// Token: 0x170003AB RID: 939
		// (get) Token: 0x06000E95 RID: 3733 RVA: 0x0005B5FB File Offset: 0x000597FB
		public bool IsGripping
		{
			get
			{
				return this.mGrippedCharacter != null;
			}
		}

		// Token: 0x170003AC RID: 940
		// (get) Token: 0x06000E96 RID: 3734 RVA: 0x0005B609 File Offset: 0x00059809
		public bool IsGripped
		{
			get
			{
				return this.mGripper != null;
			}
		}

		// Token: 0x170003AD RID: 941
		// (get) Token: 0x06000E97 RID: 3735 RVA: 0x0005B617 File Offset: 0x00059817
		public Character GrippedCharacter
		{
			get
			{
				return this.mGrippedCharacter;
			}
		}

		// Token: 0x170003AE RID: 942
		// (get) Token: 0x06000E98 RID: 3736 RVA: 0x0005B61F File Offset: 0x0005981F
		public Character Gripper
		{
			get
			{
				return this.mGripper;
			}
		}

		// Token: 0x170003AF RID: 943
		// (get) Token: 0x06000E99 RID: 3737 RVA: 0x0005B628 File Offset: 0x00059828
		public bool ShouldReleaseGrip
		{
			get
			{
				return this.mGrippedCharacter == null || (this.mGrippedCharacter.HasStatus(StatusEffects.Burning) | this.mGrippedCharacter.IsImmortal | (float)this.mGrippedCharacter.mBreakFreeCounter >= this.mBreakFreeTolerance) || this.mGripDamageAccumulation > this.mHitTolerance;
			}
		}

		// Token: 0x170003B0 RID: 944
		// (get) Token: 0x06000E9A RID: 3738 RVA: 0x0005B681 File Offset: 0x00059881
		public bool AttachedToGripped
		{
			get
			{
				return this.mGripType == Grip.GripType.Ride;
			}
		}

		// Token: 0x170003B1 RID: 945
		// (get) Token: 0x06000E9B RID: 3739 RVA: 0x0005B68C File Offset: 0x0005988C
		public bool GripperAttached
		{
			get
			{
				return this.mGripType == Grip.GripType.Pickup;
			}
		}

		// Token: 0x170003B2 RID: 946
		// (get) Token: 0x06000E9C RID: 3740 RVA: 0x0005B697 File Offset: 0x00059897
		// (set) Token: 0x06000E9D RID: 3741 RVA: 0x0005B69F File Offset: 0x0005989F
		public Grip.GripType GripType
		{
			get
			{
				return this.mGripType;
			}
			set
			{
				this.mGripType = value;
			}
		}

		// Token: 0x06000E9E RID: 3742 RVA: 0x0005B6A8 File Offset: 0x000598A8
		public void GripCharacter(Character iVictim, Grip.GripType iType, int iBoneIndex, int iTolerance)
		{
			if (iVictim.IsGripped || iVictim.Dead || iVictim.IsImmortal || iVictim.IsEthereal || iVictim.Polymorphed || iVictim.CharacterBody.Mass > 500f || this.CurrentState is KnockDownState || (iType == Grip.GripType.Ride && iVictim.Template.AnimationClips[0][140] == null))
			{
				return;
			}
			NetworkState state = NetworkManager.Instance.State;
			if (state == NetworkState.Server)
			{
				CharacterActionMessage characterActionMessage = default(CharacterActionMessage);
				characterActionMessage.Action = ActionType.Grip;
				characterActionMessage.Handle = base.Handle;
				characterActionMessage.TargetHandle = iVictim.Handle;
				characterActionMessage.Param0I = (int)iType;
				characterActionMessage.Param1I = iBoneIndex;
				characterActionMessage.Param2I = iTolerance;
				NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref characterActionMessage);
			}
			if (state != NetworkState.Client)
			{
				this.mGripType = iType;
				this.mBreakFreeTolerance = (float)((iTolerance > 0) ? iTolerance : 3);
				this.mGripDamageAccumulation = 0f;
				if (iVictim.IsGripped && iVictim.GripperAttached)
				{
					iVictim.Gripper.ReleaseAttachedCharacter();
				}
				if (iVictim.IsGripping)
				{
					iVictim.ReleaseAttachedCharacter();
				}
				iVictim.ReleaseEntanglement();
				this.mGripJoint.mIndex = iBoneIndex;
				if (iBoneIndex >= 0)
				{
					SkinnedModelBoneCollection skeleton;
					if (iType == Grip.GripType.Pickup)
					{
						skeleton = this.mAnimationController.Skeleton;
					}
					else
					{
						skeleton = iVictim.mAnimationController.Skeleton;
					}
					this.mGripJoint.mBindPose = skeleton[iBoneIndex].InverseBindPoseTransform;
					Matrix.Multiply(ref this.mGripJoint.mBindPose, ref this.mStaticTransform, out this.mGripJoint.mBindPose);
					Matrix.Invert(ref this.mGripJoint.mBindPose, out this.mGripJoint.mBindPose);
					Matrix.Multiply(ref Character.sRotateY180, ref this.mGripJoint.mBindPose, out this.mGripJoint.mBindPose);
				}
				this.mGrippedCharacter = iVictim;
				iVictim.mGripper = this;
				iVictim.GripType = iType;
				if (iType == Grip.GripType.Pickup)
				{
					iVictim.mBody.Immovable = true;
				}
				this.mBody.Immovable = true;
				this.mBreakFreeCounter = 0;
				iVictim.mBreakFreeCounter = 0;
			}
		}

		// Token: 0x06000E9F RID: 3743 RVA: 0x0005B8B4 File Offset: 0x00059AB4
		public void ReleaseAttachedCharacter()
		{
			this.mGripDamageAccumulation = 0f;
			if (this.mGrippedCharacter != null)
			{
				if (NetworkManager.Instance.State != NetworkState.Offline)
				{
					CharacterActionMessage characterActionMessage = default(CharacterActionMessage);
					characterActionMessage.Action = ActionType.Release;
					characterActionMessage.Handle = base.Handle;
					NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref characterActionMessage);
				}
				if (this.mCurrentState == BusyState.Instance || this.mCurrentState == DeadState.Instance || this.mCurrentState == DropState.Instance || this.mCurrentState == LandState.Instance || this.mCurrentState == IdleState.Instance || this.mCurrentState == JumpState.Instance || this.mCurrentState == MoveState.Instance || this.mCurrentState == EntangledState.Instance || this.mCurrentState == GripAttackState.Instance || this.mCurrentState == RessurectionState.Instance)
				{
					this.mGrippedCharacter.CharacterBody.AllowMove = false;
				}
				else
				{
					this.mGrippedCharacter.CharacterBody.AllowMove = true;
				}
				this.mGrippedCharacter.CharacterBody.AllowRotate = true;
				this.mGrippedCharacter.mBreakFreeCounter = 0;
				this.mGrippedCharacter.mGripper = null;
				this.mGrippedCharacter.mBody.Immovable = false;
				this.mBody.Immovable = false;
				Segment iSeg = new Segment(this.mGrippedCharacter.Position, new Vector3(0f, -6f, 0f));
				iSeg.Origin.Y = iSeg.Origin.Y + 3f;
				float num;
				Vector3 vector;
				Vector3 vector2;
				if (this.mPlayState.Level.CurrentScene.SegmentIntersect(out num, out vector, out vector2, iSeg))
				{
					vector.Y += this.mGrippedCharacter.Capsule.Radius + this.mGrippedCharacter.Capsule.Length * 0.5f + 0.1f;
					Matrix orientation = this.mGrippedCharacter.Body.Orientation;
					this.mGrippedCharacter.Body.MoveTo(ref vector, ref orientation);
				}
				this.mGrippedCharacter = null;
			}
		}

		// Token: 0x170003B3 RID: 947
		// (get) Token: 0x06000EA0 RID: 3744 RVA: 0x0005BAC2 File Offset: 0x00059CC2
		// (set) Token: 0x06000EA1 RID: 3745 RVA: 0x0005BACA File Offset: 0x00059CCA
		public bool ForceCamera
		{
			get
			{
				return this.mForceCamera;
			}
			set
			{
				this.mForceCamera = value;
			}
		}

		// Token: 0x170003B4 RID: 948
		// (get) Token: 0x06000EA2 RID: 3746 RVA: 0x0005BAD3 File Offset: 0x00059CD3
		// (set) Token: 0x06000EA3 RID: 3747 RVA: 0x0005BADB File Offset: 0x00059CDB
		public bool ForceNavMesh
		{
			get
			{
				return this.mForceNavMesh;
			}
			set
			{
				this.mForceNavMesh = value;
			}
		}

		// Token: 0x170003B5 RID: 949
		// (get) Token: 0x06000EA4 RID: 3748 RVA: 0x0005BAE4 File Offset: 0x00059CE4
		public bool CollidedWithCamera
		{
			get
			{
				return this.mCollidedWithCamera;
			}
		}

		// Token: 0x170003B6 RID: 950
		// (get) Token: 0x06000EA5 RID: 3749 RVA: 0x0005BAEC File Offset: 0x00059CEC
		// (set) Token: 0x06000EA6 RID: 3750 RVA: 0x0005BAF4 File Offset: 0x00059CF4
		public int OnDamageTrigger
		{
			get
			{
				return this.mOnDamageTrigger;
			}
			set
			{
				this.mOnDamageTrigger = value;
			}
		}

		// Token: 0x170003B7 RID: 951
		// (get) Token: 0x06000EA7 RID: 3751 RVA: 0x0005BAFD File Offset: 0x00059CFD
		// (set) Token: 0x06000EA8 RID: 3752 RVA: 0x0005BB05 File Offset: 0x00059D05
		public int OnDeathTrigger
		{
			get
			{
				return this.mOnDeathTrigger;
			}
			set
			{
				this.mOnDeathTrigger = value;
			}
		}

		// Token: 0x06000EA9 RID: 3753 RVA: 0x0005BB10 File Offset: 0x00059D10
		public virtual void BloatKill(Elements iElement, Entity iKiller)
		{
			if (!this.HasGibs() && this.mHitPoints <= 0f)
			{
				return;
			}
			if (!this.mCannotDieWithoutExplicitKill)
			{
				this.mInvisibilityTimer = 0f;
				if (this.HasGibs())
				{
					AudioManager.Instance.PlayCue(Banks.Spells, Railgun.ARCANESTAGESOUNDSHASH[2], base.AudioEmitter);
					this.mBloating = true;
					this.mBloatElement = iElement;
					this.mBloatKiller = iKiller;
					this.mHitPoints = 0f;
					return;
				}
				this.mBloatElement = iElement;
				this.mBloatKiller = iKiller;
				this.BloatBlast();
			}
		}

		// Token: 0x170003B8 RID: 952
		// (get) Token: 0x06000EAA RID: 3754 RVA: 0x0005BB9D File Offset: 0x00059D9D
		public bool Bloating
		{
			get
			{
				return this.mBloating;
			}
		}

		// Token: 0x170003B9 RID: 953
		// (get) Token: 0x06000EAB RID: 3755 RVA: 0x0005BBA5 File Offset: 0x00059DA5
		// (set) Token: 0x06000EAC RID: 3756 RVA: 0x0005BBAD File Offset: 0x00059DAD
		public bool CannotDieWithoutExplicitKill
		{
			get
			{
				return this.mCannotDieWithoutExplicitKill;
			}
			set
			{
				this.mCannotDieWithoutExplicitKill = value;
			}
		}

		// Token: 0x06000EAD RID: 3757 RVA: 0x0005BBB8 File Offset: 0x00059DB8
		public virtual void Drown()
		{
			this.mDrowning = true;
			if (this.IsGripped)
			{
				this.mGripper.ReleaseAttachedCharacter();
			}
			this.mInvisibilityTimer = 0f;
			this.mHitPoints = 0f;
			this.mDeadTimer = 10f;
			Vector3 position = this.Position;
			position.Y += this.WaterDepth;
			this.CharacterBody.MoveTo(position, this.mBody.Orientation);
			if (!this.mCannotDieWithoutExplicitKill)
			{
				this.mBody.DisableBody();
			}
			EffectManager.Instance.Stop(ref this.mStunEffect);
			EffectManager.Instance.Stop(ref this.mFearEffect);
			this.StopHypnotize();
			this.EndCharm();
			Vector3 direction = this.Direction;
			CollisionMaterials groundMaterial = this.CharacterBody.GroundMaterial;
			VisualEffectReference visualEffectReference;
			if (groundMaterial == CollisionMaterials.Lava)
			{
				EffectManager.Instance.StartEffect(Defines.LAVA_DROWN_EFFECT, ref position, ref direction, out visualEffectReference);
				return;
			}
			EffectManager.Instance.StartEffect(Defines.WATER_DROWN_EFFECT, ref position, ref direction, out visualEffectReference);
		}

		// Token: 0x170003BA RID: 954
		// (get) Token: 0x06000EAE RID: 3758 RVA: 0x0005BCB6 File Offset: 0x00059EB6
		public bool Drowning
		{
			get
			{
				return this.mDrowning;
			}
		}

		// Token: 0x06000EAF RID: 3759 RVA: 0x0005BCBE File Offset: 0x00059EBE
		public void SetCollisionDamage(ref DamageCollection5 iDamages)
		{
			this.mCollisionDamages = iDamages;
		}

		// Token: 0x170003BB RID: 955
		// (get) Token: 0x06000EB0 RID: 3760 RVA: 0x0005BCCC File Offset: 0x00059ECC
		// (set) Token: 0x06000EB1 RID: 3761 RVA: 0x0005BCD4 File Offset: 0x00059ED4
		public Animations DropAnimation
		{
			get
			{
				return this.mDropAnimation;
			}
			set
			{
				this.mDropAnimation = value;
			}
		}

		// Token: 0x06000EB2 RID: 3762 RVA: 0x0005BCDD File Offset: 0x00059EDD
		public void RemoveSelfShield()
		{
			if (this.mSelfShield.Active)
			{
				this.mSelfShield.Remove(this);
			}
		}

		// Token: 0x06000EB3 RID: 3763 RVA: 0x0005BCF8 File Offset: 0x00059EF8
		public void RemoveSelfShield(Character.SelfShieldType iType)
		{
			if (iType == this.mSelfShield.mShieldType)
			{
				this.mSelfShield.Remove(this);
			}
		}

		// Token: 0x06000EB4 RID: 3764 RVA: 0x0005BD14 File Offset: 0x00059F14
		public void HealSelfShield(float iAmount)
		{
			if (this.mSelfShield.Active)
			{
				this.mSelfShield.Damage(ref iAmount, Elements.Shield);
			}
		}

		// Token: 0x170003BC RID: 956
		// (get) Token: 0x06000EB5 RID: 3765 RVA: 0x0005BD36 File Offset: 0x00059F36
		public Character.SelfShieldType CurrentSelfShieldType
		{
			get
			{
				return this.mSelfShield.mShieldType;
			}
		}

		// Token: 0x170003BD RID: 957
		// (get) Token: 0x06000EB6 RID: 3766 RVA: 0x0005BD43 File Offset: 0x00059F43
		public bool IsSelfShielded
		{
			get
			{
				return this.mSelfShield.Active;
			}
		}

		// Token: 0x06000EB7 RID: 3767 RVA: 0x0005BD50 File Offset: 0x00059F50
		public void AddSelfShield(Spell iSpell)
		{
			for (int i = 0; i < this.mAuras.Count; i++)
			{
				ActiveAura value = this.mAuras[i];
				if (value.SelfCasted)
				{
					value.Aura.TTL = 0f;
					this.mAuras[i] = value;
				}
			}
			if (iSpell[Elements.Earth] == 0f && iSpell[Elements.Ice] == 0f)
			{
				bool flag = !(this.mSelfShield.Active & this.mSelfShield.mShieldType == Character.SelfShieldType.Shield);
				if (this.mSelfShield.Active)
				{
					this.RemoveSelfShield();
				}
				if (flag)
				{
					this.mSelfShield = new Character.SelfShield(this, iSpell);
					return;
				}
			}
			else
			{
				this.mSelfShield = new Character.SelfShield(this, iSpell);
			}
		}

		// Token: 0x06000EB8 RID: 3768 RVA: 0x0005BE18 File Offset: 0x0005A018
		public void AddBubbleShield(AuraStorage aura)
		{
			this.mBubble = true;
			this.AddAura(ref aura, true);
		}

		// Token: 0x06000EB9 RID: 3769 RVA: 0x0005BE2A File Offset: 0x0005A02A
		public void RemoveBubbleShield()
		{
			if (this.mBubble)
			{
				this.ClearAura();
			}
		}

		// Token: 0x06000EBA RID: 3770 RVA: 0x0005BE3A File Offset: 0x0005A03A
		public bool HaveBubbleShield()
		{
			return this.mBubble;
		}

		// Token: 0x170003BE RID: 958
		// (get) Token: 0x06000EBB RID: 3771 RVA: 0x0005BE42 File Offset: 0x0005A042
		public bool IsSolidSelfShielded
		{
			get
			{
				return this.mSelfShield.Active && this.mSelfShield.Solid;
			}
		}

		// Token: 0x06000EBC RID: 3772 RVA: 0x0005BE5E File Offset: 0x0005A05E
		public void MeleeDamageBoost(float iPercentage)
		{
			this.mMeleeBoostAmount = iPercentage;
			this.mMeleeBoostTimer = 5f;
		}

		// Token: 0x170003BF RID: 959
		// (get) Token: 0x06000EBD RID: 3773 RVA: 0x0005BE72 File Offset: 0x0005A072
		public bool MeleeBoosted
		{
			get
			{
				return this.mMeleeBoostTimer > 0f;
			}
		}

		// Token: 0x170003C0 RID: 960
		// (get) Token: 0x06000EBE RID: 3774 RVA: 0x0005BE81 File Offset: 0x0005A081
		public float MeleeBoostAmount
		{
			get
			{
				return this.mMeleeBoostAmount;
			}
		}

		// Token: 0x170003C1 RID: 961
		// (get) Token: 0x06000EBF RID: 3775 RVA: 0x0005BE89 File Offset: 0x0005A089
		// (set) Token: 0x06000EC0 RID: 3776 RVA: 0x0005BE91 File Offset: 0x0005A091
		public bool GripAttacking
		{
			get
			{
				return this.mGripAttack;
			}
			set
			{
				this.mGripAttack = value;
			}
		}

		// Token: 0x170003C2 RID: 962
		// (get) Token: 0x06000EC1 RID: 3777 RVA: 0x0005BE9A File Offset: 0x0005A09A
		// (set) Token: 0x06000EC2 RID: 3778 RVA: 0x0005BEA2 File Offset: 0x0005A0A2
		public float SpellPower
		{
			get
			{
				return this.mSpellPower;
			}
			set
			{
				this.mSpellPower = value;
			}
		}

		// Token: 0x170003C3 RID: 963
		// (get) Token: 0x06000EC3 RID: 3779 RVA: 0x0005BEAB File Offset: 0x0005A0AB
		public float GripDamageAccumulation
		{
			get
			{
				return this.mGripDamageAccumulation;
			}
		}

		// Token: 0x170003C4 RID: 964
		// (get) Token: 0x06000EC4 RID: 3780 RVA: 0x0005BEB3 File Offset: 0x0005A0B3
		public float HitTolerance
		{
			get
			{
				return this.mHitTolerance;
			}
		}

		// Token: 0x170003C5 RID: 965
		// (get) Token: 0x06000EC5 RID: 3781 RVA: 0x0005BEBB File Offset: 0x0005A0BB
		// (set) Token: 0x06000EC6 RID: 3782 RVA: 0x0005BEC3 File Offset: 0x0005A0C3
		public float BreakFreeTolerance
		{
			get
			{
				return this.mBreakFreeTolerance;
			}
			set
			{
				this.mBreakFreeTolerance = value;
			}
		}

		// Token: 0x170003C6 RID: 966
		// (get) Token: 0x06000EC7 RID: 3783 RVA: 0x0005BECC File Offset: 0x0005A0CC
		// (set) Token: 0x06000EC8 RID: 3784 RVA: 0x0005BED4 File Offset: 0x0005A0D4
		public bool IsEthereal
		{
			get
			{
				return this.mEthereal;
			}
			set
			{
				this.mEthereal = value;
				if (this.mEthereal)
				{
					this.StopStatusEffects(StatusEffects.Burning | StatusEffects.Wet | StatusEffects.Frozen | StatusEffects.Cold | StatusEffects.Poisoned | StatusEffects.Healing | StatusEffects.Greased | StatusEffects.Steamed);
				}
			}
		}

		// Token: 0x170003C7 RID: 967
		// (get) Token: 0x06000EC9 RID: 3785 RVA: 0x0005BEF0 File Offset: 0x0005A0F0
		// (set) Token: 0x06000ECA RID: 3786 RVA: 0x0005BEF8 File Offset: 0x0005A0F8
		public float EtherealAlpha
		{
			get
			{
				return this.mEtherealAlpha;
			}
			set
			{
				this.mEtherealAlphaTarget = value;
				this.mEtherealAlpha = value;
			}
		}

		// Token: 0x06000ECB RID: 3787 RVA: 0x0005BF15 File Offset: 0x0005A115
		public void Ethereal(bool iEthereal, float iAlpha, float iSpeed)
		{
			this.mEthereal = iEthereal;
			if (this.mEthereal)
			{
				this.StopStatusEffects(StatusEffects.Burning | StatusEffects.Wet | StatusEffects.Frozen | StatusEffects.Cold | StatusEffects.Poisoned | StatusEffects.Healing | StatusEffects.Greased | StatusEffects.Steamed);
			}
			this.mEtherealAlphaTarget = iAlpha;
			this.mEtherealAlphaSpeed = iSpeed;
		}

		// Token: 0x06000ECC RID: 3788 RVA: 0x0005BF3F File Offset: 0x0005A13F
		internal void TimedEthereal(float iTime, bool iEtherealState)
		{
			if (iTime <= 0f)
			{
				this.mEthereal = iEtherealState;
				this.mEtherealTimer = 0f;
				return;
			}
			this.mEtherealTimer = iTime;
		}

		// Token: 0x170003C8 RID: 968
		// (get) Token: 0x06000ECD RID: 3789 RVA: 0x0005BF63 File Offset: 0x0005A163
		// (set) Token: 0x06000ECE RID: 3790 RVA: 0x0005BF6B File Offset: 0x0005A16B
		public float TimeWarpModifier
		{
			get
			{
				return this.mTimeWarpModifier;
			}
			set
			{
				this.mTimeWarpModifier = value;
			}
		}

		// Token: 0x170003C9 RID: 969
		// (get) Token: 0x06000ECF RID: 3791 RVA: 0x0005BF74 File Offset: 0x0005A174
		// (set) Token: 0x06000ED0 RID: 3792 RVA: 0x0005BF7C File Offset: 0x0005A17C
		public bool IsFearless
		{
			get
			{
				return this.mFearless;
			}
			set
			{
				this.mFearless = value;
				if (this.mFearless)
				{
					this.mFearTimer = 0f;
				}
			}
		}

		// Token: 0x170003CA RID: 970
		// (get) Token: 0x06000ED1 RID: 3793 RVA: 0x0005BF98 File Offset: 0x0005A198
		// (set) Token: 0x06000ED2 RID: 3794 RVA: 0x0005BFA0 File Offset: 0x0005A1A0
		public bool IsUncharmable
		{
			get
			{
				return this.mUncharmable;
			}
			set
			{
				this.mUncharmable = value;
				if (this.mUncharmable)
				{
					this.mCharmTimer = 0f;
				}
			}
		}

		// Token: 0x170003CB RID: 971
		// (get) Token: 0x06000ED3 RID: 3795 RVA: 0x0005BFBC File Offset: 0x0005A1BC
		public bool IsNonslippery
		{
			get
			{
				return this.mNonslippery;
			}
		}

		// Token: 0x170003CC RID: 972
		// (get) Token: 0x06000ED4 RID: 3796 RVA: 0x0005BFC4 File Offset: 0x0005A1C4
		public bool HasFairy
		{
			get
			{
				return this.mHasFairy;
			}
		}

		// Token: 0x06000ED5 RID: 3797 RVA: 0x0005BFCC File Offset: 0x0005A1CC
		internal void ResetStaticTransform()
		{
			this.mStaticTransform = this.mTemplate.Models[this.mModelID].Transform;
		}

		// Token: 0x06000ED6 RID: 3798 RVA: 0x0005BFF0 File Offset: 0x0005A1F0
		internal void SetStaticTransform(float iX, float iY, float iZ)
		{
			Matrix matrix;
			Matrix.CreateScale(iX, iY, iZ, out matrix);
			Matrix.Multiply(ref matrix, ref this.mTemplate.Models[this.mModelID].Transform, out this.mStaticTransform);
		}

		// Token: 0x06000ED7 RID: 3799 RVA: 0x0005C02F File Offset: 0x0005A22F
		internal void ResetCapsuleForm()
		{
			this.SetCapsuleForm(this.mTemplate.Length, this.mTemplate.Radius);
		}

		// Token: 0x06000ED8 RID: 3800 RVA: 0x0005C050 File Offset: 0x0005A250
		internal void SetCapsuleForm(float iLength, float iRadius)
		{
			Capsule capsule = this.Body.CollisionSkin.GetPrimitiveNewWorld(0) as Capsule;
			Capsule capsule2 = this.Body.CollisionSkin.GetPrimitiveOldWorld(0) as Capsule;
			Capsule capsule3 = this.Body.CollisionSkin.GetPrimitiveLocal(0) as Capsule;
			float length = capsule3.Length;
			float radius = capsule3.Radius;
			float length2 = capsule.Length;
			float radius2 = capsule.Radius;
			float length3 = capsule2.Length;
			float radius3 = capsule2.Radius;
			capsule3.Length = iLength;
			capsule3.Radius = iRadius;
			capsule.Length = iLength;
			capsule.Radius = iRadius;
			capsule2.Length = iLength;
			capsule2.Radius = iRadius;
			Vector3 position = capsule3.Position;
			position.Y = iLength * -0.5f;
			capsule3.Position = position;
			position = capsule.Position;
			position.Y -= (length2 - iLength) * 0.5f;
			position.Y -= radius2 - iRadius;
			capsule.Position = position;
			position = capsule2.Position;
			position.Y -= (length3 - iLength) * 0.5f;
			position.Y -= radius3 - iRadius;
			capsule2.Position = position;
			this.HeightOffset = -iRadius - 0.5f * iLength;
			Matrix orientation = this.Body.Orientation;
			position = this.Position;
			position.Y -= (length - iLength) * 0.5f;
			position.Y -= radius - iRadius;
			this.Body.MoveTo(position, orientation);
			this.mRadius = iRadius;
		}

		// Token: 0x170003CD RID: 973
		// (get) Token: 0x06000ED9 RID: 3801 RVA: 0x0005C1F1 File Offset: 0x0005A3F1
		public CharacterTemplate Template
		{
			get
			{
				return this.mTemplate;
			}
		}

		// Token: 0x170003CE RID: 974
		// (get) Token: 0x06000EDA RID: 3802 RVA: 0x0005C1F9 File Offset: 0x0005A3F9
		// (set) Token: 0x06000EDB RID: 3803 RVA: 0x0005C201 File Offset: 0x0005A401
		internal ConditionCollection EventConditions
		{
			get
			{
				return this.mEventConditions;
			}
			set
			{
				this.mEventConditions = value;
			}
		}

		// Token: 0x06000EDC RID: 3804 RVA: 0x0005C20A File Offset: 0x0005A40A
		public override bool ArcIntersect(out Vector3 oPosition, Vector3 iOrigin, Vector3 iDirection, float iRange, float iAngle, float iHeightDifference)
		{
			if (this.mEthereal)
			{
				oPosition = default(Vector3);
				return false;
			}
			return base.ArcIntersect(out oPosition, iOrigin, iDirection, iRange, iAngle, iHeightDifference);
		}

		// Token: 0x06000EDD RID: 3805 RVA: 0x0005C22C File Offset: 0x0005A42C
		internal void AccumulateArcaneDamage(Elements iElement, float iMagnitude)
		{
			int num = MagickaMath.CountTrailingZeroBits((uint)iElement);
			this.mBloatDamageAccumulation[num] += iMagnitude;
		}

		// Token: 0x06000EDE RID: 3806 RVA: 0x0005C25C File Offset: 0x0005A45C
		internal virtual void NetworkAction(ref CharacterActionMessage iMsg)
		{
			switch (iMsg.Action)
			{
			case ActionType.ConjureElement:
			{
				Elements param0I = (Elements)iMsg.Param0I;
				Spell spell;
				Spell.DefaultSpell(param0I, out spell);
				SpellManager.Instance.TryAddToQueue(null, this, this.mSpellQueue, 5, ref spell);
				return;
			}
			case ActionType.Attack:
				this.mNextAttackAnimation = (Animations)(iMsg.Param0I & int.MaxValue);
				this.mAllowAttackRotate = (((long)iMsg.Param0I & (long)((ulong)int.MinValue)) != 0L);
				this.CastType = (CastType)iMsg.Param1I;
				this.mSpellPower = iMsg.Param2F;
				this.mAttacking = true;
				return;
			case ActionType.Block:
				this.mBlock = (iMsg.Param0I != 0);
				return;
			case ActionType.BreakFree:
				if (!(this.IsEntangled | this.IsGripped))
				{
					return;
				}
				this.BreakFree();
				if (!(this.mCurrentState is CastState))
				{
					this.GoToAnimation(Animations.spec_entangled_attack, 0.1f);
					return;
				}
				return;
			case ActionType.Grip:
			{
				Character character = Entity.GetFromHandle((int)iMsg.TargetHandle) as Character;
				Grip.GripType param0I2 = (Grip.GripType)iMsg.Param0I;
				this.mGripType = param0I2;
				this.mBreakFreeTolerance = (float)((iMsg.Param2I > 0) ? iMsg.Param2I : 3);
				this.mGripDamageAccumulation = 0f;
				if (character.IsGripped && character.GripperAttached)
				{
					character.Gripper.ReleaseAttachedCharacter();
				}
				if (character.IsGripping)
				{
					character.ReleaseAttachedCharacter();
				}
				character.ReleaseEntanglement();
				this.mGripJoint.mIndex = iMsg.Param1I;
				if (iMsg.Param1I >= 0)
				{
					SkinnedModelBoneCollection skeleton;
					if (param0I2 == Grip.GripType.Pickup)
					{
						skeleton = this.mAnimationController.Skeleton;
					}
					else
					{
						skeleton = character.mAnimationController.Skeleton;
					}
					this.mGripJoint.mBindPose = skeleton[iMsg.Param1I].InverseBindPoseTransform;
					Matrix.Multiply(ref this.mGripJoint.mBindPose, ref this.mStaticTransform, out this.mGripJoint.mBindPose);
					Matrix.Invert(ref this.mGripJoint.mBindPose, out this.mGripJoint.mBindPose);
					Matrix.Multiply(ref Character.sRotateY180, ref this.mGripJoint.mBindPose, out this.mGripJoint.mBindPose);
				}
				this.mGrippedCharacter = character;
				character.mGripper = this;
				character.GripType = param0I2;
				if (param0I2 == Grip.GripType.Pickup)
				{
					character.mBody.Immovable = true;
				}
				this.mBody.Immovable = true;
				this.mBreakFreeCounter = 0;
				character.mBreakFreeCounter = 0;
				return;
			}
			case ActionType.GripAttack:
				this.mNextGripAttackAnimation = (Animations)iMsg.Param0I;
				this.mAllowAttackRotate = false;
				this.mGripAttack = true;
				this.mAttacking = false;
				return;
			case ActionType.Entangle:
				if (this.mEntaglement.Magnitude <= 1E-45f)
				{
					this.mEntaglement.Initialize();
				}
				this.mEntaglement.AddEntanglement(iMsg.Param1F);
				return;
			case ActionType.Release:
				if (this.IsEntangled)
				{
					this.ReleaseEntanglement();
					return;
				}
				if (this.IsGripped)
				{
					this.mGripDamageAccumulation = 0f;
					this.mGripper.ReleaseAttachedCharacter();
					return;
				}
				if (this.IsGripping)
				{
					this.mGripDamageAccumulation = 0f;
					if (this.mCurrentState == BusyState.Instance || this.mCurrentState == DeadState.Instance || this.mCurrentState == DropState.Instance || this.mCurrentState == LandState.Instance || this.mCurrentState == IdleState.Instance || this.mCurrentState == JumpState.Instance || this.mCurrentState == MoveState.Instance || this.mCurrentState == EntangledState.Instance || this.mCurrentState == GripAttackState.Instance || this.mCurrentState == RessurectionState.Instance)
					{
						this.mGrippedCharacter.CharacterBody.AllowMove = false;
					}
					else
					{
						this.mGrippedCharacter.CharacterBody.AllowMove = true;
					}
					this.mGrippedCharacter.CharacterBody.AllowRotate = true;
					this.mGrippedCharacter.mBreakFreeCounter = 0;
					this.mGrippedCharacter.mGripper = null;
					this.mGrippedCharacter.mBody.Immovable = false;
					this.mBody.Immovable = false;
					Segment iSeg = new Segment(this.mGrippedCharacter.Position, new Vector3(0f, -6f, 0f));
					iSeg.Origin.Y = iSeg.Origin.Y + 3f;
					float num;
					Vector3 vector;
					Vector3 vector2;
					if (this.mPlayState.Level.CurrentScene.SegmentIntersect(out num, out vector, out vector2, iSeg))
					{
						vector.Y += this.mGrippedCharacter.Capsule.Radius + this.mGrippedCharacter.Capsule.Length * 0.5f + 0.1f;
						Matrix orientation = this.mGrippedCharacter.Body.Orientation;
						this.mGrippedCharacter.Body.MoveTo(ref vector, ref orientation);
					}
					this.mGrippedCharacter = null;
					return;
				}
				return;
			case ActionType.Dash:
				this.mNextDashAnimation = (Animations)(iMsg.Param0I & int.MaxValue);
				this.mAllowAttackRotate = (((long)iMsg.Param0I & (long)((ulong)int.MinValue)) != 0L);
				this.mDashing = true;
				return;
			case ActionType.Jump:
			{
				if (this.IsEntangled || this.Dead)
				{
					return;
				}
				Vector3 iImpulse = new Vector3(iMsg.Param0F, iMsg.Param1F, iMsg.Param2F);
				float num2 = (float)iMsg.TimeStamp;
				float y = iImpulse.Y;
				iImpulse.Y = 0f;
				float num3 = iImpulse.Length();
				Vector3.Divide(ref iImpulse, num3, out iImpulse);
				float num4 = iImpulse.Y = (float)Math.Sin((double)num2);
				float num5 = (float)Math.Cos((double)num2);
				iImpulse.X *= num5;
				iImpulse.Z *= num5;
				float num6 = (float)Math.Sqrt((double)(PhysicsManager.Instance.Simulator.Gravity.Y * -1f * num3 * num3 / (2f * (num3 * num4 / num5 - y) * num5 * num5)));
				if (float.IsNaN(num6) || float.IsInfinity(num6))
				{
					return;
				}
				Vector3.Multiply(ref iImpulse, num6, out iImpulse);
				this.CharacterBody.AddJump(iImpulse);
				return;
			}
			case ActionType.Magick:
			{
				MagickType param3I = (MagickType)iMsg.Param3I;
				switch (param3I)
				{
				case MagickType.Teleport:
				{
					Vector3 iPosition = new Vector3(iMsg.Param0F, iMsg.Param1F, iMsg.Param2F);
					Vector3 position = this.Position;
					if (iMsg.TargetHandle != 0)
					{
						Entity fromHandle = Entity.GetFromHandle((int)iMsg.TargetHandle);
						position = fromHandle.Position;
					}
					Vector3.Subtract(ref iPosition, ref position, out position);
					position.Y = 0f;
					position.Normalize();
					Teleport.Instance.DoTeleport(this, iPosition, position, (Teleport.TeleportType)iMsg.Param4I);
					return;
				}
				case MagickType.Fear:
					if (iMsg.TargetHandle != 0)
					{
						Character iFearedBy = Entity.GetFromHandle((int)iMsg.TargetHandle) as Character;
						this.Fear(iFearedBy);
						return;
					}
					this.Fear(new Vector3(iMsg.Param0F, iMsg.Param1F, iMsg.Param2F));
					return;
				default:
					if (param3I != MagickType.CTD)
					{
						if (param3I != MagickType.Levitate)
						{
							throw new Exception(string.Format("{0} failed to handle MagicType {1} by way of networkaction!", this.mName, (MagickType)iMsg.Param3I));
						}
						Levitate.CastLevitate(this);
						return;
					}
					else
					{
						Character character2 = Entity.GetFromHandle((int)iMsg.TargetHandle) as Character;
						Matrix matrix;
						Matrix.CreateScale(character2.Radius, character2.Capsule.Length + character2.Radius * 2f, character2.Radius, out matrix);
						matrix.Translation = character2.Position;
						VisualEffectReference visualEffectReference;
						EffectManager.Instance.StartEffect(CTD.EFFECT, ref matrix, out visualEffectReference);
						AudioManager.Instance.PlayCue(Banks.Additional, CTD.SOUND, character2.AudioEmitter);
						if (base.PlayState.Level.CurrentScene.RuleSet is SurvivalRuleset && character2 is NonPlayerCharacter && character2.DisplayName != 0)
						{
							NetworkChat.Instance.AddMessage(LanguageManager.Instance.GetString(CTD.MESSAGE).Replace("#1;", LanguageManager.Instance.GetString(character2.DisplayName)));
						}
						if (NetworkManager.Instance.State == NetworkState.Server)
						{
							character2.Terminate(true, false);
							return;
						}
						return;
					}
					break;
				}
				break;
			}
			case ActionType.SelfShield:
			{
				Elements param0I3 = (Elements)iMsg.Param0I;
				Spell iSpell = default(Spell);
				iSpell.Element = param0I3;
				iSpell[Elements.Earth] = iMsg.Param1F;
				iSpell[Elements.Ice] = iMsg.Param2F;
				iSpell[Elements.Shield] = 1f;
				this.AddSelfShield(iSpell);
				return;
			}
			case ActionType.EventAnimation:
				if ((ushort)iMsg.Param2I != 0)
				{
					this.SpecialIdleAnimation = (Animations)iMsg.Param2I;
				}
				if ((ushort)iMsg.Param0I == 0)
				{
					return;
				}
				if (this.mCurrentState is IdleState)
				{
					this.GoToAnimation((Animations)iMsg.Param0I, iMsg.Param1F);
					return;
				}
				this.mNetworkAnimation = (Animations)iMsg.Param0I;
				this.mNetworkAnimationBlend = iMsg.Param1F;
				return;
			}
			throw new NotImplementedException();
		}

		// Token: 0x06000EDF RID: 3807 RVA: 0x0005CB54 File Offset: 0x0005AD54
		protected unsafe override void INetworkUpdate(ref EntityUpdateMessage iMsg)
		{
			base.INetworkUpdate(ref iMsg);
			if ((ushort)(iMsg.Features & EntityFeatures.Damageable) != 0)
			{
				this.mHitPoints = iMsg.HitPoints;
			}
			if ((ushort)(iMsg.Features & EntityFeatures.SelfShield) != 0)
			{
				this.mSelfShield.mShieldType = iMsg.SelfShieldType;
				this.mSelfShield.mSelfShieldHealth = iMsg.SelfShieldHealth;
			}
			if ((ushort)(iMsg.Features & EntityFeatures.Etherealized) != 0)
			{
				if (iMsg.EtherealState)
				{
					if (!this.IsEthereal)
					{
						this.Ethereal(true, 1f, 1f);
					}
				}
				else if (this.IsEthereal)
				{
					this.Ethereal(false, 1f, 1f);
				}
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
									int num = StatusEffect.StatusIndex(StatusEffects.Burning);
									int num2 = StatusEffect.StatusIndex(StatusEffects.Wet);
									int num3 = StatusEffect.StatusIndex(StatusEffects.Frozen);
									switch (iStatus)
									{
									case StatusEffects.Burning:
										if (this.mStatusEffectCues[num] != null && !this.mStatusEffectCues[num].IsStopping)
										{
											this.mStatusEffectCues[num].Stop(AudioStopOptions.AsAuthored);
										}
										break;
									case StatusEffects.Wet:
										if (this.mStatusEffectCues[num2] != null && !this.mStatusEffectCues[num2].IsStopping)
										{
											this.mStatusEffectCues[num2].Stop(AudioStopOptions.AsAuthored);
										}
										break;
									case StatusEffects.Frozen:
										if (this.mStatusEffectCues[num3] != null && !this.mStatusEffectCues[num3].IsStopping)
										{
											this.mStatusEffectCues[num3].Stop(AudioStopOptions.AsAuthored);
										}
										break;
									}
									this.mStatusEffects[i].Stop();
									this.mStatusEffects[i] = default(StatusEffect);
								}
							}
							else if (ptr[i] > 0f && this.mResistances != null)
							{
								this.AddStatusEffect(new StatusEffect(iStatus, ptr2[i], ptr[i], this.Capsule.Length, this.Capsule.Radius));
							}
						}
					}
				}
			}
			if ((ushort)(iMsg.Features & EntityFeatures.Direction) != 0)
			{
				if (iMsg.GenericFloat > 1E-45f)
				{
					Vector3 movement = default(Vector3);
					movement.X = (float)Math.Cos((double)iMsg.Direction) * iMsg.GenericFloat;
					movement.Z = (float)Math.Sin((double)iMsg.Direction) * iMsg.GenericFloat;
					this.CharacterBody.Movement = movement;
				}
				else
				{
					this.CharacterBody.Movement = default(Vector3);
					Vector3 desiredDirection = default(Vector3);
					desiredDirection.X = (float)Math.Cos((double)iMsg.Direction);
					desiredDirection.Z = (float)Math.Sin((double)iMsg.Direction);
					this.CharacterBody.DesiredDirection = desiredDirection;
				}
			}
			if ((ushort)(iMsg.Features & EntityFeatures.WanderAngle) != 0)
			{
				this.WanderAngle = iMsg.WanderAngle;
			}
		}

		// Token: 0x06000EE0 RID: 3808 RVA: 0x0005CEA8 File Offset: 0x0005B0A8
		protected unsafe override void IGetNetworkUpdate(out EntityUpdateMessage oMsg, float iPrediction)
		{
			oMsg = default(EntityUpdateMessage);
			if (!this.RestingMovement)
			{
				bool isTouchingGround = this.CharacterBody.IsTouchingGround;
				Vector3 velocity = this.mBody.Velocity;
				Vector3 vector2;
				if (isTouchingGround)
				{
					Vector3 vector = default(Vector3);
					vector2 = default(Vector3);
				}
				else
				{
					Vector3 vector = PhysicsManager.Instance.Simulator.Gravity;
					Vector3.Multiply(ref vector, iPrediction, out vector);
					Vector3.Multiply(ref vector, iPrediction, out vector2);
					oMsg.Features |= EntityFeatures.Velocity;
					Vector3.Add(ref velocity, ref vector, out oMsg.Velocity);
				}
				oMsg.Features |= EntityFeatures.Position;
				oMsg.Position = this.Position;
				Vector3.Multiply(ref velocity, iPrediction, out velocity);
				Vector3.Add(ref velocity, ref oMsg.Position, out oMsg.Position);
				if (!isTouchingGround)
				{
					Vector3.Add(ref vector2, ref oMsg.Position, out oMsg.Position);
				}
			}
			oMsg.Features |= EntityFeatures.Direction;
			Vector3 desiredDirection = this.CharacterBody.DesiredDirection;
			desiredDirection.Y = 0f;
			desiredDirection.Normalize();
			oMsg.Direction = (float)Math.Atan2((double)desiredDirection.Z, (double)desiredDirection.X);
			oMsg.Features |= EntityFeatures.GenericFloat;
			oMsg.GenericFloat = this.CharacterBody.Movement.Length();
			if (this.IsStumbling | this.IsPanicing)
			{
				oMsg.Features |= EntityFeatures.WanderAngle;
				oMsg.WanderAngle = this.WanderAngle;
			}
			oMsg.Features |= EntityFeatures.Etherealized;
			oMsg.EtherealState = this.IsEthereal;
			oMsg.Features |= EntityFeatures.SelfShield;
			if (this.mSelfShield.Active)
			{
				oMsg.SelfShieldHealth = this.mSelfShield.mSelfShieldHealth;
				oMsg.SelfShieldType = this.mSelfShield.mShieldType;
			}
			else
			{
				oMsg.SelfShieldHealth = 0f;
				oMsg.SelfShieldType = Character.SelfShieldType.None;
			}
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

		// Token: 0x06000EE1 RID: 3809 RVA: 0x0005D159 File Offset: 0x0005B359
		internal override float GetDanger()
		{
			return (float)this.mSpellQueue.Count;
		}

		// Token: 0x06000EE2 RID: 3810 RVA: 0x0005D168 File Offset: 0x0005B368
		public void GetDamageModifier(Elements iElement, out float oMultiplyer, out float oBias)
		{
			oMultiplyer = 1f;
			oBias = 0f;
			foreach (BuffStorage buffStorage in this.mBuffs)
			{
				if (buffStorage.BuffType == BuffType.BoostDamage && (buffStorage.BuffBoostDamage.Damage.Element & iElement) != Elements.None)
				{
					oBias += buffStorage.BuffBoostDamage.Damage.Amount;
					oMultiplyer *= buffStorage.BuffBoostDamage.Damage.Magnitude;
				}
			}
		}

		// Token: 0x06000EE3 RID: 3811 RVA: 0x0005D20C File Offset: 0x0005B40C
		public void GetSpellTTLModifier(ref float iTTL)
		{
			float num = 1f;
			float num2 = 0f;
			foreach (BuffStorage buffStorage in this.mBuffs)
			{
				if (buffStorage.BuffType == BuffType.ModifySpellTTL)
				{
					num *= buffStorage.BuffModifySpellTTL.TTLMultiplier;
					num2 += buffStorage.BuffModifySpellTTL.TTLModifier;
				}
			}
			iTTL *= num;
			iTTL += num2;
		}

		// Token: 0x06000EE4 RID: 3812 RVA: 0x0005D298 File Offset: 0x0005B498
		public void GetSpellRangeModifier(ref float iRange)
		{
			float num = 1f;
			float num2 = 0f;
			foreach (BuffStorage buffStorage in this.mBuffs)
			{
				if (buffStorage.BuffType == BuffType.ModifySpellRange)
				{
					num *= buffStorage.BuffModifySpellRange.RangeMultiplier;
					num2 += buffStorage.BuffModifySpellRange.RangeModifier;
				}
			}
			iRange *= num;
			iRange += num2;
		}

		// Token: 0x06000EE5 RID: 3813 RVA: 0x0005D324 File Offset: 0x0005B524
		public float GetAgroMultiplier()
		{
			float num = 1f;
			foreach (BuffStorage buffStorage in this.mBuffs)
			{
				if (buffStorage.BuffType == BuffType.ReduceAgro)
				{
					num *= buffStorage.BuffReduceAgro.Amount;
				}
			}
			return num;
		}

		// Token: 0x06000EE6 RID: 3814 RVA: 0x0005D390 File Offset: 0x0005B590
		internal bool HasAura(AuraType iType)
		{
			for (int i = 0; i < this.mAuras.Count; i++)
			{
				if (this.mAuras[i].Aura.AuraType == iType)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000EE7 RID: 3815 RVA: 0x0005D3D0 File Offset: 0x0005B5D0
		internal float GetResistanceBuffMultiplier(Elements iElement)
		{
			float num = 1f;
			for (int i = 0; i < this.mBuffs.Count; i++)
			{
				if (this.mBuffs[i].BuffType == BuffType.Resistance && (this.mBuffs[i].BuffResistance.Resistance.ResistanceAgainst & iElement) != Elements.None)
				{
					num *= this.mBuffs[i].BuffResistance.Resistance.Multiplier;
				}
			}
			return num;
		}

		// Token: 0x06000EE8 RID: 3816 RVA: 0x0005D44C File Offset: 0x0005B64C
		internal float IsResistantAgainst(Elements iElement)
		{
			float num = 1f;
			num *= this.GetResistanceBuffMultiplier(iElement);
			for (int i = 0; i < this.mResistances.Length; i++)
			{
				if ((this.mResistances[i].ResistanceAgainst & iElement) != Elements.None)
				{
					num *= this.mResistances[i].Multiplier;
				}
			}
			for (int j = 0; j < this.mEquipment.Length; j++)
			{
				if (this.mEquipment[j].Item != null)
				{
					Resistance[] resistance = this.mEquipment[j].Item.Resistance;
					for (int k = 0; k < resistance.Length; k++)
					{
						if ((resistance[k].ResistanceAgainst & iElement) != Elements.None)
						{
							num *= resistance[k].Multiplier;
						}
					}
				}
			}
			return num;
		}

		// Token: 0x06000EE9 RID: 3817 RVA: 0x0005D50E File Offset: 0x0005B70E
		internal void ScaleGraphicModel(float iScale)
		{
			this.mStaticTransform.M11 = iScale;
			this.mStaticTransform.M22 = iScale;
			this.mStaticTransform.M33 = iScale;
		}

		// Token: 0x06000EEA RID: 3818 RVA: 0x0005D534 File Offset: 0x0005B734
		public bool HasPassiveAbility(Item.PassiveAbilities iAbility)
		{
			for (int i = 0; i < this.mEquipment.Length; i++)
			{
				if (this.mEquipment[i].Item.Attached && this.mEquipment[i].Item.PassiveAbility.Ability == iAbility)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x04000C58 RID: 3160
		private static readonly int[] BEASTMEN_HASHES = new int[]
		{
			"beastman_brute".GetHashCodeCustom(),
			"beastman_brute_earth".GetHashCodeCustom(),
			"beastman_brute_fire".GetHashCodeCustom(),
			"beastman_chieftain".GetHashCodeCustom(),
			"beastman_raider".GetHashCodeCustom(),
			"beastman_raider_lightning".GetHashCodeCustom(),
			"beastman_raider_water".GetHashCodeCustom(),
			"beastman_torcher".GetHashCodeCustom()
		};

		// Token: 0x04000C59 RID: 3161
		protected static readonly int STUN_EFFECT = "stunned".GetHashCodeCustom();

		// Token: 0x04000C5A RID: 3162
		protected static readonly Vector3 WetColor = new Vector3(0.75f, 0.8f, 1f);

		// Token: 0x04000C5B RID: 3163
		protected static readonly Vector3 ColdColor = new Vector3(1f, 1.6f, 2f);

		// Token: 0x04000C5C RID: 3164
		protected static Matrix sRotateY180 = Matrix.CreateRotationY(3.1415927f);

		// Token: 0x04000C5D RID: 3165
		protected static Matrix sRotateX90 = Matrix.CreateRotationX(-1.5707964f);

		// Token: 0x04000C5E RID: 3166
		protected static Matrix sRotateSpines = Matrix.CreateFromYawPitchRoll(0f, 1.5707964f, 1.5707964f);

		// Token: 0x04000C5F RID: 3167
		protected static TextureCube sIceCubeMap;

		// Token: 0x04000C60 RID: 3168
		protected static TextureCube sIceCubeNormalMap;

		// Token: 0x04000C61 RID: 3169
		private static SkinnedModel sBarrierSkinModel;

		// Token: 0x04000C62 RID: 3170
		private static SkinnedModelDeferredAdvancedMaterial sEarthBarrierSkinMaterial;

		// Token: 0x04000C63 RID: 3171
		private static SkinnedModelDeferredAdvancedMaterial sIceBarrierSkinMaterial;

		// Token: 0x04000C64 RID: 3172
		private static Model sAuraModel;

		// Token: 0x04000C65 RID: 3173
		protected Character.RenderData[] mRenderData;

		// Token: 0x04000C66 RID: 3174
		protected Character.NormalDistortionRenderData[] mNormalDistortionRenderData;

		// Token: 0x04000C67 RID: 3175
		protected Character.ShieldSkinRenderData[] mShieldSkinRenderData;

		// Token: 0x04000C68 RID: 3176
		protected Character.BarrierSkinRenderData[] mBarrierSkinRenderData;

		// Token: 0x04000C69 RID: 3177
		protected Character.HaloAuraRenderData[] mArmourRenderData;

		// Token: 0x04000C6A RID: 3178
		protected Character.LightningZapRenderData[] mLightningZapRenderData;

		// Token: 0x04000C6B RID: 3179
		protected Character.HighlightRenderData[] mHighlightRenderData;

		// Token: 0x04000C6C RID: 3180
		protected float mAuraPulsation;

		// Token: 0x04000C6D RID: 3181
		protected float mAuraRays1Rotation;

		// Token: 0x04000C6E RID: 3182
		protected float mAuraRays2Rotation;

		// Token: 0x04000C6F RID: 3183
		protected CharacterTemplate mTemplate;

		// Token: 0x04000C70 RID: 3184
		protected ConditionCollection mEventConditions;

		// Token: 0x04000C71 RID: 3185
		private int mDisplayName;

		// Token: 0x04000C72 RID: 3186
		protected string mName;

		// Token: 0x04000C73 RID: 3187
		protected int mType;

		// Token: 0x04000C74 RID: 3188
		protected Factions mFaction;

		// Token: 0x04000C75 RID: 3189
		protected BloodType mBlood;

		// Token: 0x04000C76 RID: 3190
		protected bool mEthereal;

		// Token: 0x04000C77 RID: 3191
		protected bool mEtherealLook;

		// Token: 0x04000C78 RID: 3192
		protected float mVolume;

		// Token: 0x04000C79 RID: 3193
		protected int mScoreValue;

		// Token: 0x04000C7A RID: 3194
		protected float mHitPoints;

		// Token: 0x04000C7B RID: 3195
		protected float mMaxHitPoints;

		// Token: 0x04000C7C RID: 3196
		protected int mNumberOfHealtBars;

		// Token: 0x04000C7D RID: 3197
		protected float mNormalizedHitPoints;

		// Token: 0x04000C7E RID: 3198
		protected float mBreakFreeTolerance;

		// Token: 0x04000C7F RID: 3199
		protected float mHitTolerance;

		// Token: 0x04000C80 RID: 3200
		protected float mTurnSpeed;

		// Token: 0x04000C81 RID: 3201
		protected float mTurnSpeedMax;

		// Token: 0x04000C82 RID: 3202
		protected float mKnockdownTolerance;

		// Token: 0x04000C83 RID: 3203
		protected Resistance[] mResistances;

		// Token: 0x04000C84 RID: 3204
		protected float mHeightOffset;

		// Token: 0x04000C85 RID: 3205
		protected float mMapRotation;

		// Token: 0x04000C86 RID: 3206
		protected float mMapScale;

		// Token: 0x04000C87 RID: 3207
		protected float mZapTimer;

		// Token: 0x04000C88 RID: 3208
		protected float mBloatTimer;

		// Token: 0x04000C89 RID: 3209
		protected float mDryTimer;

		// Token: 0x04000C8A RID: 3210
		protected float mMaxPanic;

		// Token: 0x04000C8B RID: 3211
		protected float mPanic;

		// Token: 0x04000C8C RID: 3212
		protected float mWanderAngle;

		// Token: 0x04000C8D RID: 3213
		protected float mZapModifier;

		// Token: 0x04000C8E RID: 3214
		protected bool mBloatKilled;

		// Token: 0x04000C8F RID: 3215
		protected bool mFearless;

		// Token: 0x04000C90 RID: 3216
		protected bool mUncharmable;

		// Token: 0x04000C91 RID: 3217
		protected bool mNonslippery;

		// Token: 0x04000C92 RID: 3218
		protected bool mHasFairy;

		// Token: 0x04000C93 RID: 3219
		protected MovementProperties mMoveAbilities;

		// Token: 0x04000C94 RID: 3220
		protected Dictionary<byte, Animations[]> mMoveAnimations;

		// Token: 0x04000C95 RID: 3221
		protected float mTotalRegenAccumulation;

		// Token: 0x04000C96 RID: 3222
		protected float mRegenAccumulation;

		// Token: 0x04000C97 RID: 3223
		protected int mRegenRate;

		// Token: 0x04000C98 RID: 3224
		protected float mRegenTimer;

		// Token: 0x04000C99 RID: 3225
		internal Character.SelfShield mSelfShield;

		// Token: 0x04000C9A RID: 3226
		internal bool mBubble;

		// Token: 0x04000C9B RID: 3227
		protected float mTemplateMass;

		// Token: 0x04000C9C RID: 3228
		protected float mUndieTimer;

		// Token: 0x04000C9D RID: 3229
		protected bool mAlert;

		// Token: 0x04000C9E RID: 3230
		protected bool mDashing;

		// Token: 0x04000C9F RID: 3231
		protected bool mAttacking;

		// Token: 0x04000CA0 RID: 3232
		protected bool mKnockedDown;

		// Token: 0x04000CA1 RID: 3233
		protected bool mIsHit;

		// Token: 0x04000CA2 RID: 3234
		private bool mBlock;

		// Token: 0x04000CA3 RID: 3235
		protected bool mDrowning;

		// Token: 0x04000CA4 RID: 3236
		protected bool mBloating;

		// Token: 0x04000CA5 RID: 3237
		protected Elements mBloatElement;

		// Token: 0x04000CA6 RID: 3238
		protected Entity mBloatKiller;

		// Token: 0x04000CA7 RID: 3239
		protected int mDialog;

		// Token: 0x04000CA8 RID: 3240
		protected bool mAllowAttackRotate;

		// Token: 0x04000CA9 RID: 3241
		protected SkinnedModel mModel;

		// Token: 0x04000CAA RID: 3242
		protected AnimationController mAnimationController;

		// Token: 0x04000CAB RID: 3243
		protected AnimationClipAction[][] mAnimationClips;

		// Token: 0x04000CAC RID: 3244
		protected AnimationAction[] mCurrentActions;

		// Token: 0x04000CAD RID: 3245
		protected List<bool> mExecutedActions = new List<bool>(8);

		// Token: 0x04000CAE RID: 3246
		protected List<bool> mDeadActions = new List<bool>(8);

		// Token: 0x04000CAF RID: 3247
		protected Animations mNextDashAnimation;

		// Token: 0x04000CB0 RID: 3248
		protected Animations mNextAttackAnimation;

		// Token: 0x04000CB1 RID: 3249
		protected Animations mNextGripAttackAnimation;

		// Token: 0x04000CB2 RID: 3250
		protected Animations mSpawnAnimation;

		// Token: 0x04000CB3 RID: 3251
		protected Animations mSpecialIdleAnimation;

		// Token: 0x04000CB4 RID: 3252
		protected Animations mNetworkAnimation;

		// Token: 0x04000CB5 RID: 3253
		protected float mNetworkAnimationBlend;

		// Token: 0x04000CB6 RID: 3254
		protected WeaponClass mCurrentAnimationSet;

		// Token: 0x04000CB7 RID: 3255
		protected Animations mCurrentAnimation;

		// Token: 0x04000CB8 RID: 3256
		protected Matrix mStaticTransform;

		// Token: 0x04000CB9 RID: 3257
		protected int mModelID;

		// Token: 0x04000CBA RID: 3258
		protected float mLastDraw;

		// Token: 0x04000CBB RID: 3259
		protected bool mForceAnimationUpdate;

		// Token: 0x04000CBC RID: 3260
		private BindJoint mHipJoint;

		// Token: 0x04000CBD RID: 3261
		private BindJoint mMouthJoint;

		// Token: 0x04000CBE RID: 3262
		private BindJoint mLeftHandJoint;

		// Token: 0x04000CBF RID: 3263
		private BindJoint mLeftKneeJoint;

		// Token: 0x04000CC0 RID: 3264
		private BindJoint mRightHandJoint;

		// Token: 0x04000CC1 RID: 3265
		private BindJoint mRightKneeJoint;

		// Token: 0x04000CC2 RID: 3266
		protected Character mGripper;

		// Token: 0x04000CC3 RID: 3267
		protected bool mGripAttack;

		// Token: 0x04000CC4 RID: 3268
		protected Grip.GripType mGripType;

		// Token: 0x04000CC5 RID: 3269
		protected BindJoint mGripJoint;

		// Token: 0x04000CC6 RID: 3270
		protected Character mGrippedCharacter;

		// Token: 0x04000CC7 RID: 3271
		protected DamageCollection5 mCollisionDamages;

		// Token: 0x04000CC8 RID: 3272
		protected Animations mDropAnimation;

		// Token: 0x04000CC9 RID: 3273
		protected float mGripDamageAccumulation;

		// Token: 0x04000CCA RID: 3274
		protected float mFireDamageAccumulation;

		// Token: 0x04000CCB RID: 3275
		protected float mFireDamageAccumulationTimer;

		// Token: 0x04000CCC RID: 3276
		protected float mPoisonDamageAccumulation;

		// Token: 0x04000CCD RID: 3277
		protected float mPoisonDamageAccumulationTimer = 0.25f;

		// Token: 0x04000CCE RID: 3278
		protected float mHealingAccumulation;

		// Token: 0x04000CCF RID: 3279
		protected float mHealingAccumulationTimer = 0.25f;

		// Token: 0x04000CD0 RID: 3280
		protected float mBleedDamageAccumulation;

		// Token: 0x04000CD1 RID: 3281
		protected float mBleedDamageAccumulationTimer;

		// Token: 0x04000CD2 RID: 3282
		protected int mLastDamageIndex;

		// Token: 0x04000CD3 RID: 3283
		protected float mLastDamageAmount;

		// Token: 0x04000CD4 RID: 3284
		protected Elements mLastDamageElement;

		// Token: 0x04000CD5 RID: 3285
		protected float mTimeSinceLastDamage;

		// Token: 0x04000CD6 RID: 3286
		protected float mTimeSinceLastStatusDamage;

		// Token: 0x04000CD7 RID: 3287
		protected float[] mBloatDamageAccumulation = new float[11];

		// Token: 0x04000CD8 RID: 3288
		protected NonPlayerCharacter[] mCurrentSummons = new NonPlayerCharacter[16];

		// Token: 0x04000CD9 RID: 3289
		protected int mNumCurrentSummons;

		// Token: 0x04000CDA RID: 3290
		protected int mNumCurrentUndeadSummons;

		// Token: 0x04000CDB RID: 3291
		protected int mNumCurrentFlamerSummons;

		// Token: 0x04000CDC RID: 3292
		protected Player mLastAccumulationDamager;

		// Token: 0x04000CDD RID: 3293
		protected Entity mLastAttacker;

		// Token: 0x04000CDE RID: 3294
		protected float mLastAttackerTimer;

		// Token: 0x04000CDF RID: 3295
		protected DamageResult mLastDamage;

		// Token: 0x04000CE0 RID: 3296
		protected DynamicLight mStatusEffectLight;

		// Token: 0x04000CE1 RID: 3297
		protected int mSourceOfSpellIndex;

		// Token: 0x04000CE2 RID: 3298
		protected DynamicLight mSpellLight;

		// Token: 0x04000CE3 RID: 3299
		protected StatusEffects mCurrentStatusEffects;

		// Token: 0x04000CE4 RID: 3300
		protected StatusEffect[] mStatusEffects = new StatusEffect[9];

		// Token: 0x04000CE5 RID: 3301
		protected Cue[] mStatusEffectCues;

		// Token: 0x04000CE6 RID: 3302
		public Cue ChargeCue;

		// Token: 0x04000CE7 RID: 3303
		protected float mSpellPower;

		// Token: 0x04000CE8 RID: 3304
		protected Spell mSpell;

		// Token: 0x04000CE9 RID: 3305
		protected CastType mCastType;

		// Token: 0x04000CEA RID: 3306
		protected float mHitByLightning;

		// Token: 0x04000CEB RID: 3307
		protected Matrix mStaffOrb;

		// Token: 0x04000CEC RID: 3308
		protected Matrix mWeaponTransform;

		// Token: 0x04000CED RID: 3309
		protected StaticList<Spell> mSpellQueue;

		// Token: 0x04000CEE RID: 3310
		protected SpellEffect mCurrentSpell;

		// Token: 0x04000CEF RID: 3311
		public int mSummonElementCue;

		// Token: 0x04000CF0 RID: 3312
		public Banks mSummonElementBank;

		// Token: 0x04000CF1 RID: 3313
		protected Entanglement mEntaglement;

		// Token: 0x04000CF2 RID: 3314
		protected Attachment[] mEquipment = new Attachment[8];

		// Token: 0x04000CF3 RID: 3315
		protected List<GibReference> mGibs;

		// Token: 0x04000CF4 RID: 3316
		protected VisualEffectReference mDryingEffect;

		// Token: 0x04000CF5 RID: 3317
		protected int mOnDamageTrigger;

		// Token: 0x04000CF6 RID: 3318
		protected int mOnDeathTrigger;

		// Token: 0x04000CF7 RID: 3319
		protected BaseState mPreviousState;

		// Token: 0x04000CF8 RID: 3320
		protected BaseState mCurrentState;

		// Token: 0x04000CF9 RID: 3321
		protected SkinnedModelDeferredBasicMaterial mMaterial;

		// Token: 0x04000CFA RID: 3322
		protected float mImmortalTime;

		// Token: 0x04000CFB RID: 3323
		protected float mCollisionIgnoreTime;

		// Token: 0x04000CFC RID: 3324
		protected bool mOverkilled;

		// Token: 0x04000CFD RID: 3325
		protected bool mHighlight;

		// Token: 0x04000CFE RID: 3326
		protected float mInvisibilityTimer;

		// Token: 0x04000CFF RID: 3327
		protected bool mCannotDieWithoutExplicitKill;

		// Token: 0x04000D00 RID: 3328
		protected bool mFeared;

		// Token: 0x04000D01 RID: 3329
		protected float mFearTimer;

		// Token: 0x04000D02 RID: 3330
		protected Character mFearedBy;

		// Token: 0x04000D03 RID: 3331
		protected Vector3 mFearPosition;

		// Token: 0x04000D04 RID: 3332
		protected VisualEffectReference mFearEffect;

		// Token: 0x04000D05 RID: 3333
		protected float mMeleeBoostAmount;

		// Token: 0x04000D06 RID: 3334
		protected float mMeleeBoostTimer;

		// Token: 0x04000D07 RID: 3335
		protected float mBreakFreeStrength;

		// Token: 0x04000D08 RID: 3336
		private float mWaterDepth;

		// Token: 0x04000D09 RID: 3337
		private VisualEffectReference mWaterSplashEffect;

		// Token: 0x04000D0A RID: 3338
		protected float mShadowTimer;

		// Token: 0x04000D0B RID: 3339
		protected float mDeadTimer = 20f;

		// Token: 0x04000D0C RID: 3340
		public new bool mDead;

		// Token: 0x04000D0D RID: 3341
		protected bool mRemoveAfterDeath = true;

		// Token: 0x04000D0E RID: 3342
		protected HitList mHitList = new HitList(32);

		// Token: 0x04000D0F RID: 3343
		private float mDefaultSpecular;

		// Token: 0x04000D10 RID: 3344
		protected StatusEffects mDeathStatusEffects;

		// Token: 0x04000D11 RID: 3345
		protected VisualEffectReference[] mAttachedEffects = new VisualEffectReference[8];

		// Token: 0x04000D12 RID: 3346
		protected Matrix[] mAttachedEffectsBindPose = new Matrix[8];

		// Token: 0x04000D13 RID: 3347
		protected int[] mAttachedEffectsBoneIndex = new int[]
		{
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1
		};

		// Token: 0x04000D14 RID: 3348
		protected Cue[] mAttachedSoundCues = new Cue[4];

		// Token: 0x04000D15 RID: 3349
		protected KeyValuePair<int, Banks>[] mAttachedSounds;

		// Token: 0x04000D16 RID: 3350
		protected DynamicLight mPointLight;

		// Token: 0x04000D17 RID: 3351
		protected Character.PointLightHolder mPointLightHolder;

		// Token: 0x04000D18 RID: 3352
		protected int mBreakFreeCounter;

		// Token: 0x04000D19 RID: 3353
		protected float mBoundingScale;

		// Token: 0x04000D1A RID: 3354
		protected Magicka.GameLogic.Entities.Abilities.SpecialAbilities.SpecialAbility mSpecialAbility;

		// Token: 0x04000D1B RID: 3355
		protected static readonly Random sRandom = new Random();

		// Token: 0x04000D1C RID: 3356
		protected float mEtherealAlpha = 1f;

		// Token: 0x04000D1D RID: 3357
		protected float mEtherealAlphaTarget = 1f;

		// Token: 0x04000D1E RID: 3358
		protected float mEtherealAlphaSpeed = 1f;

		// Token: 0x04000D1F RID: 3359
		protected float mEtherealTimer;

		// Token: 0x04000D20 RID: 3360
		protected bool mEtherealTimedState;

		// Token: 0x04000D21 RID: 3361
		protected bool mTemplateIsEthereal;

		// Token: 0x04000D22 RID: 3362
		protected float mTimeWarpModifier = 1f;

		// Token: 0x04000D23 RID: 3363
		protected bool mDoNotRender;

		// Token: 0x04000D24 RID: 3364
		protected List<ActiveAura> mAuras = new List<ActiveAura>();

		// Token: 0x04000D25 RID: 3365
		protected List<BuffStorage> mBuffs = new List<BuffStorage>();

		// Token: 0x04000D26 RID: 3366
		protected float mAuraCycleTimer;

		// Token: 0x04000D27 RID: 3367
		protected Vector4 mHaloBuffColor = default(Vector4);

		// Token: 0x04000D28 RID: 3368
		protected float mHaloBuffFade;

		// Token: 0x04000D29 RID: 3369
		protected List<DecalManager.DecalReference> mBuffDecals = new List<DecalManager.DecalReference>();

		// Token: 0x04000D2A RID: 3370
		protected List<VisualEffectReference> mBuffEffects = new List<VisualEffectReference>();

		// Token: 0x04000D2B RID: 3371
		protected float mBuffRotation;

		// Token: 0x04000D2C RID: 3372
		protected float mBuffCycleTimerOffensive;

		// Token: 0x04000D2D RID: 3373
		protected float mBuffCycleTimerDefensive;

		// Token: 0x04000D2E RID: 3374
		protected float mBuffCycleTimerSpecial;

		// Token: 0x04000D2F RID: 3375
		public bool mUndying;

		// Token: 0x04000D30 RID: 3376
		protected bool mJustCastInvisible;

		// Token: 0x04000D31 RID: 3377
		protected bool mCanHasStatusEffect = true;

		// Token: 0x04000D32 RID: 3378
		protected float mRestingMovementTimer = 1f;

		// Token: 0x04000D33 RID: 3379
		protected float mLastHitpoints;

		// Token: 0x04000D34 RID: 3380
		protected float mRestingHealthTimer = 1f;

		// Token: 0x04000D35 RID: 3381
		protected VisualEffectReference mLevitateEffect;

		// Token: 0x04000D36 RID: 3382
		protected float mLevitateTimer;

		// Token: 0x04000D37 RID: 3383
		protected float mLevitationFreeFallTimer;

		// Token: 0x04000D38 RID: 3384
		protected int mHypnotizeEffectID;

		// Token: 0x04000D39 RID: 3385
		protected VisualEffectReference mHypnotizeEffect;

		// Token: 0x04000D3A RID: 3386
		protected bool mHypnotized;

		// Token: 0x04000D3B RID: 3387
		protected Vector3 mHypnotizeDirection;

		// Token: 0x04000D3C RID: 3388
		protected Entity mCharmOwner;

		// Token: 0x04000D3D RID: 3389
		protected float mCharmTimer;

		// Token: 0x04000D3E RID: 3390
		protected int mCharmEffectID;

		// Token: 0x04000D3F RID: 3391
		protected VisualEffectReference mCharmEffect;

		// Token: 0x04000D40 RID: 3392
		protected bool mForceCamera;

		// Token: 0x04000D41 RID: 3393
		protected bool mForceNavMesh;

		// Token: 0x04000D42 RID: 3394
		protected bool mCollidedWithCamera;

		// Token: 0x04000D43 RID: 3395
		internal float mStunTime = 2f;

		// Token: 0x04000D44 RID: 3396
		internal float mStunTimer;

		// Token: 0x04000D45 RID: 3397
		internal VisualEffectReference mStunEffect;

		// Token: 0x04000D46 RID: 3398
		protected float mBleedRate;

		// Token: 0x04000D47 RID: 3399
		protected bool mNotedKilledEvent;

		// Token: 0x04000D48 RID: 3400
		protected float mCollisionDamageGracePeriod;

		// Token: 0x04000D49 RID: 3401
		protected float mTimeDead;

		// Token: 0x020001BC RID: 444
		protected class LightningZapRenderData : IPostEffect, IPreRenderRenderer
		{
			// Token: 0x06000EEC RID: 3820 RVA: 0x0005D68C File Offset: 0x0005B88C
			public LightningZapRenderData()
			{
				GraphicsDevice graphicsDevice = Game.Instance.GraphicsDevice;
				if (Character.LightningZapRenderData.sFlashVertexBuffer == null || Character.LightningZapRenderData.sFlashVertexBuffer.IsDisposed)
				{
					VertexPositionTexture[] array = new VertexPositionTexture[4];
					array[0].Position.X = 1f;
					array[0].Position.Y = 1f;
					array[0].TextureCoordinate.X = 0f;
					array[0].TextureCoordinate.Y = 0f;
					array[1].Position.X = -1f;
					array[1].Position.Y = 1f;
					array[1].TextureCoordinate.X = 1f;
					array[1].TextureCoordinate.Y = 0f;
					array[2].Position.X = -1f;
					array[2].Position.Y = -1f;
					array[2].TextureCoordinate.X = 1f;
					array[2].TextureCoordinate.Y = 1f;
					array[3].Position.X = 1f;
					array[3].Position.Y = -1f;
					array[3].TextureCoordinate.X = 0f;
					array[3].TextureCoordinate.Y = 1f;
					lock (graphicsDevice)
					{
						Character.LightningZapRenderData.sFlashVertexBuffer = new VertexBuffer(graphicsDevice, 4 * VertexPositionTexture.SizeInBytes, BufferUsage.WriteOnly);
						Character.LightningZapRenderData.sFlashVertexBuffer.SetData<VertexPositionTexture>(array);
					}
				}
				if (Character.LightningZapRenderData.sFlashVertexDeclaration == null || Character.LightningZapRenderData.sFlashVertexDeclaration.IsDisposed)
				{
					lock (graphicsDevice)
					{
						Character.LightningZapRenderData.sFlashVertexDeclaration = new VertexDeclaration(graphicsDevice, VertexPositionTexture.VertexElements);
					}
				}
				if (Character.LightningZapRenderData.sFlashTexture == null || Character.LightningZapRenderData.sFlashTexture.IsDisposed)
				{
					lock (graphicsDevice)
					{
						Character.LightningZapRenderData.sFlashTexture = Game.Instance.Content.Load<Texture2D>("EffectTextures/LightningHit");
					}
				}
			}

			// Token: 0x06000EED RID: 3821 RVA: 0x0005D8F8 File Offset: 0x0005BAF8
			public void SetMeshDirty()
			{
				this.mMeshDirty = true;
			}

			// Token: 0x170003CF RID: 975
			// (get) Token: 0x06000EEE RID: 3822 RVA: 0x0005D901 File Offset: 0x0005BB01
			public bool MeshDirty
			{
				get
				{
					return this.mMeshDirty;
				}
			}

			// Token: 0x06000EEF RID: 3823 RVA: 0x0005D90C File Offset: 0x0005BB0C
			public void SetMesh(VertexBuffer iVertices, IndexBuffer iIndices, ModelMeshPart iMeshPart, ref SkinnedModelDeferredBasicMaterial iBasicMaterial, VertexBuffer iSkeletonVertices, VertexDeclaration iSkeletonVertexDeclaration, int iSkeletonVertexStride, int iSkeletonPrimitiveCount)
			{
				this.mMeshDirty = false;
				this.mMaterial = iBasicMaterial;
				this.mVertexBuffer = iVertices;
				this.mIndexBuffer = iIndices;
				this.mVertexDeclaration = iMeshPart.VertexDeclaration;
				this.mBaseVertex = iMeshPart.BaseVertex;
				this.mNumVertices = iMeshPart.NumVertices;
				this.mPrimitiveCount = iMeshPart.PrimitiveCount;
				this.mStartIndex = iMeshPart.StartIndex;
				this.mStreamOffset = iMeshPart.StreamOffset;
				this.mVertexStride = iMeshPart.VertexStride;
				this.mSkeletonVertexBuffer = iSkeletonVertices;
				this.mSkeletonVertexDeclaration = iSkeletonVertexDeclaration;
				this.mSkeletonVertexStride = iSkeletonVertexStride;
				this.mSkeletonPrimitiveCount = iSkeletonPrimitiveCount;
			}

			// Token: 0x170003D0 RID: 976
			// (get) Token: 0x06000EF0 RID: 3824 RVA: 0x0005D9AF File Offset: 0x0005BBAF
			public int ZIndex
			{
				get
				{
					return 0;
				}
			}

			// Token: 0x06000EF1 RID: 3825 RVA: 0x0005D9B4 File Offset: 0x0005BBB4
			public void Draw(float iDeltaTime, ref Vector2 iPixelSize, ref Matrix iViewMatrix, ref Matrix iProjectionMatrix, Texture2D iCandidate, Texture2D iDepthMap, Texture2D iNormalMap)
			{
				SkinnedModelSkeletonEffect skinnedModelSkeletonEffect = RenderManager.Instance.GetEffect(SkinnedModelSkeletonEffect.TYPEHASH) as SkinnedModelSkeletonEffect;
				skinnedModelSkeletonEffect.GraphicsDevice.RenderState.StencilEnable = true;
				skinnedModelSkeletonEffect.GraphicsDevice.RenderState.ReferenceStencil = 1;
				skinnedModelSkeletonEffect.GraphicsDevice.RenderState.StencilFunction = CompareFunction.Always;
				skinnedModelSkeletonEffect.GraphicsDevice.RenderState.StencilPass = StencilOperation.Replace;
				skinnedModelSkeletonEffect.GraphicsDevice.RenderState.AlphaBlendEnable = false;
				skinnedModelSkeletonEffect.GraphicsDevice.RenderState.DepthBufferEnable = true;
				skinnedModelSkeletonEffect.GraphicsDevice.RenderState.DepthBufferWriteEnable = false;
				skinnedModelSkeletonEffect.CurrentTechnique = skinnedModelSkeletonEffect.Techniques[0];
				skinnedModelSkeletonEffect.DiffuseMap0 = this.mMaterial.DiffuseMap0;
				skinnedModelSkeletonEffect.DiffuseMap0Enabled = this.mMaterial.DiffuseMap0Enabled;
				skinnedModelSkeletonEffect.DiffuseMap1 = this.mMaterial.DiffuseMap1;
				skinnedModelSkeletonEffect.DiffuseMap1Enabled = this.mMaterial.DiffuseMap1Enabled;
				skinnedModelSkeletonEffect.DepthMap = iDepthMap;
				skinnedModelSkeletonEffect.View = iViewMatrix;
				skinnedModelSkeletonEffect.Projection = iProjectionMatrix;
				skinnedModelSkeletonEffect.Bones = this.mSkeleton;
				skinnedModelSkeletonEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertexBuffer, 0, this.mVertexStride);
				skinnedModelSkeletonEffect.GraphicsDevice.Indices = this.mIndexBuffer;
				skinnedModelSkeletonEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
				skinnedModelSkeletonEffect.Begin();
				skinnedModelSkeletonEffect.CurrentTechnique.Passes[0].Begin();
				skinnedModelSkeletonEffect.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
				skinnedModelSkeletonEffect.CurrentTechnique.Passes[0].End();
				skinnedModelSkeletonEffect.End();
				skinnedModelSkeletonEffect.GraphicsDevice.RenderState.StencilFunction = CompareFunction.Equal;
				skinnedModelSkeletonEffect.GraphicsDevice.RenderState.StencilPass = StencilOperation.Keep;
				skinnedModelSkeletonEffect.GraphicsDevice.RenderState.AlphaBlendEnable = true;
				skinnedModelSkeletonEffect.GraphicsDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
				skinnedModelSkeletonEffect.GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha;
				skinnedModelSkeletonEffect.CurrentTechnique = skinnedModelSkeletonEffect.Techniques[1];
				skinnedModelSkeletonEffect.GraphicsDevice.Vertices[0].SetSource(this.mSkeletonVertexBuffer, 0, this.mSkeletonVertexStride);
				skinnedModelSkeletonEffect.GraphicsDevice.VertexDeclaration = this.mSkeletonVertexDeclaration;
				skinnedModelSkeletonEffect.Begin();
				skinnedModelSkeletonEffect.CurrentTechnique.Passes[0].Begin();
				skinnedModelSkeletonEffect.GraphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.LineStrip, 0, this.mSkeletonPrimitiveCount);
				skinnedModelSkeletonEffect.CurrentTechnique.Passes[0].End();
				skinnedModelSkeletonEffect.End();
				ArcaneEffect arcaneEffect = RenderManager.Instance.GetEffect(ArcaneEffect.TYPEHASH) as ArcaneEffect;
				skinnedModelSkeletonEffect.GraphicsDevice.RenderState.StencilFunction = CompareFunction.NotEqual;
				skinnedModelSkeletonEffect.GraphicsDevice.RenderState.DestinationBlend = Blend.One;
				skinnedModelSkeletonEffect.GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha;
				arcaneEffect.Alpha = 1f;
				arcaneEffect.ColorCenter = Spell.LIGHTNINGCOLOR;
				arcaneEffect.ColorEdge = Spell.LIGHTNINGCOLOR;
				arcaneEffect.Texture = Character.LightningZapRenderData.sFlashTexture;
				arcaneEffect.TextureScale = 1f;
				arcaneEffect.World = this.mFlashTransform;
				arcaneEffect.ViewProjection = iViewMatrix * iProjectionMatrix;
				arcaneEffect.CurrentTechnique = arcaneEffect.Techniques[0];
				arcaneEffect.GraphicsDevice.Vertices[0].SetSource(Character.LightningZapRenderData.sFlashVertexBuffer, 0, VertexPositionTexture.SizeInBytes);
				arcaneEffect.GraphicsDevice.VertexDeclaration = Character.LightningZapRenderData.sFlashVertexDeclaration;
				arcaneEffect.Begin();
				arcaneEffect.CurrentTechnique.Passes[0].Begin();
				arcaneEffect.GraphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleFan, 0, 2);
				arcaneEffect.CurrentTechnique.Passes[0].End();
				arcaneEffect.End();
				skinnedModelSkeletonEffect.GraphicsDevice.RenderState.StencilEnable = false;
			}

			// Token: 0x06000EF2 RID: 3826 RVA: 0x0005DD98 File Offset: 0x0005BF98
			public void PreRenderUpdate(DataChannel iDataChannel, float iDeltaTime, ref Matrix iViewProjectionMatrix, ref Vector3 iCameraPosition, ref Vector3 iCameraDirection)
			{
				Vector3 vector = default(Vector3);
				vector.Y = 1f;
				Matrix.CreateBillboard(ref this.mBoundingSphere.Center, ref iCameraPosition, ref vector, new Vector3?(iCameraDirection), out this.mFlashTransform);
				this.mFlashTransform.M11 = this.mFlashTransform.M11 * (this.mBoundingSphere.Radius * 0.85f);
				this.mFlashTransform.M12 = this.mFlashTransform.M12 * (this.mBoundingSphere.Radius * 0.85f);
				this.mFlashTransform.M13 = this.mFlashTransform.M13 * (this.mBoundingSphere.Radius * 0.85f);
				this.mFlashTransform.M21 = this.mFlashTransform.M21 * (this.mBoundingSphere.Radius * 0.85f);
				this.mFlashTransform.M22 = this.mFlashTransform.M22 * (this.mBoundingSphere.Radius * 0.85f);
				this.mFlashTransform.M23 = this.mFlashTransform.M23 * (this.mBoundingSphere.Radius * 0.85f);
				this.mFlashTransform.M31 = this.mFlashTransform.M31 * (this.mBoundingSphere.Radius * 0.85f);
				this.mFlashTransform.M32 = this.mFlashTransform.M32 * (this.mBoundingSphere.Radius * 0.85f);
				this.mFlashTransform.M33 = this.mFlashTransform.M33 * (this.mBoundingSphere.Radius * 0.85f);
				Matrix matrix;
				Matrix.CreateRotationZ(MagickaMath.RandomBetween(0f, 6.2831855f), out matrix);
				Matrix.Multiply(ref matrix, ref this.mFlashTransform, out this.mFlashTransform);
			}

			// Token: 0x04000D4C RID: 3404
			public BoundingSphere mBoundingSphere;

			// Token: 0x04000D4D RID: 3405
			public Matrix[] mSkeleton;

			// Token: 0x04000D4E RID: 3406
			protected bool mMeshDirty = true;

			// Token: 0x04000D4F RID: 3407
			protected VertexDeclaration mVertexDeclaration;

			// Token: 0x04000D50 RID: 3408
			protected VertexBuffer mVertexBuffer;

			// Token: 0x04000D51 RID: 3409
			protected IndexBuffer mIndexBuffer;

			// Token: 0x04000D52 RID: 3410
			protected int mBaseVertex;

			// Token: 0x04000D53 RID: 3411
			protected int mNumVertices;

			// Token: 0x04000D54 RID: 3412
			protected int mPrimitiveCount;

			// Token: 0x04000D55 RID: 3413
			protected int mStartIndex;

			// Token: 0x04000D56 RID: 3414
			protected int mStreamOffset;

			// Token: 0x04000D57 RID: 3415
			protected int mVertexStride;

			// Token: 0x04000D58 RID: 3416
			public SkinnedModelDeferredBasicMaterial mMaterial;

			// Token: 0x04000D59 RID: 3417
			protected VertexDeclaration mSkeletonVertexDeclaration;

			// Token: 0x04000D5A RID: 3418
			protected int mSkeletonPrimitiveCount;

			// Token: 0x04000D5B RID: 3419
			protected int mSkeletonVertexStride;

			// Token: 0x04000D5C RID: 3420
			protected VertexBuffer mSkeletonVertexBuffer;

			// Token: 0x04000D5D RID: 3421
			protected static VertexDeclaration sFlashVertexDeclaration;

			// Token: 0x04000D5E RID: 3422
			protected static VertexBuffer sFlashVertexBuffer;

			// Token: 0x04000D5F RID: 3423
			protected static Texture2D sFlashTexture;

			// Token: 0x04000D60 RID: 3424
			protected Matrix mFlashTransform;
		}

		// Token: 0x020001BD RID: 445
		protected class ShieldSkinRenderData : IRenderableAdditiveObject
		{
			// Token: 0x06000EF3 RID: 3827 RVA: 0x0005DF44 File Offset: 0x0005C144
			public ShieldSkinRenderData()
			{
				if (Character.ShieldSkinRenderData.sTexture == null || Character.ShieldSkinRenderData.sTexture.IsDisposed)
				{
					lock (Game.Instance.GraphicsDevice)
					{
						Character.ShieldSkinRenderData.sTexture = Game.Instance.Content.Load<Texture2D>("EffectTextures/Shield");
					}
				}
			}

			// Token: 0x170003D1 RID: 977
			// (get) Token: 0x06000EF4 RID: 3828 RVA: 0x0005DFB4 File Offset: 0x0005C1B4
			public bool MeshDirty
			{
				get
				{
					return this.mMeshDirty;
				}
			}

			// Token: 0x170003D2 RID: 978
			// (get) Token: 0x06000EF5 RID: 3829 RVA: 0x0005DFBC File Offset: 0x0005C1BC
			public int Effect
			{
				get
				{
					return SkinnedShieldEffect.TYPEHASH;
				}
			}

			// Token: 0x170003D3 RID: 979
			// (get) Token: 0x06000EF6 RID: 3830 RVA: 0x0005DFC3 File Offset: 0x0005C1C3
			public int Technique
			{
				get
				{
					return 0;
				}
			}

			// Token: 0x170003D4 RID: 980
			// (get) Token: 0x06000EF7 RID: 3831 RVA: 0x0005DFC6 File Offset: 0x0005C1C6
			public VertexBuffer Vertices
			{
				get
				{
					return this.mVertexBuffer;
				}
			}

			// Token: 0x170003D5 RID: 981
			// (get) Token: 0x06000EF8 RID: 3832 RVA: 0x0005DFCE File Offset: 0x0005C1CE
			public IndexBuffer Indices
			{
				get
				{
					return this.mIndexBuffer;
				}
			}

			// Token: 0x170003D6 RID: 982
			// (get) Token: 0x06000EF9 RID: 3833 RVA: 0x0005DFD6 File Offset: 0x0005C1D6
			public VertexDeclaration VertexDeclaration
			{
				get
				{
					return this.mVertexDeclaration;
				}
			}

			// Token: 0x170003D7 RID: 983
			// (get) Token: 0x06000EFA RID: 3834 RVA: 0x0005DFDE File Offset: 0x0005C1DE
			public int VertexStride
			{
				get
				{
					return this.mVertexStride;
				}
			}

			// Token: 0x170003D8 RID: 984
			// (get) Token: 0x06000EFB RID: 3835 RVA: 0x0005DFE6 File Offset: 0x0005C1E6
			public int VerticesHashCode
			{
				get
				{
					return this.mVerticesHash;
				}
			}

			// Token: 0x06000EFC RID: 3836 RVA: 0x0005DFF0 File Offset: 0x0005C1F0
			public bool Cull(BoundingFrustum iViewFrustum)
			{
				BoundingSphere boundingSphere = this.mBoundingSphere;
				return boundingSphere.Contains(iViewFrustum) == ContainmentType.Disjoint;
			}

			// Token: 0x06000EFD RID: 3837 RVA: 0x0005E010 File Offset: 0x0005C210
			public virtual void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				SkinnedShieldEffect skinnedShieldEffect = iEffect as SkinnedShieldEffect;
				skinnedShieldEffect.GraphicsDevice.RenderState.CullMode = CullMode.None;
				skinnedShieldEffect.ProjectionMap = Character.ShieldSkinRenderData.sTexture;
				skinnedShieldEffect.Bones = this.mSkeleton;
				skinnedShieldEffect.Color = this.mColor;
				skinnedShieldEffect.TextureScale = this.mTextureScale;
				skinnedShieldEffect.TextureOffset0 = this.mTextureOffset0;
				skinnedShieldEffect.TextureOffset1 = this.mTextureOffset1;
				skinnedShieldEffect.TextureOffset2 = this.mTextureOffset2;
				skinnedShieldEffect.ProjectionMapMatrix0 = this.mProjectionMatrix0;
				skinnedShieldEffect.ProjectionMapMatrix1 = this.mProjectionMatrix1;
				skinnedShieldEffect.ProjectionMapMatrix2 = this.mProjectionMatrix2;
				skinnedShieldEffect.Bloat = 0.075f;
				skinnedShieldEffect.CommitChanges();
				skinnedShieldEffect.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
				skinnedShieldEffect.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
			}

			// Token: 0x06000EFE RID: 3838 RVA: 0x0005E0F3 File Offset: 0x0005C2F3
			public void SetMeshDirty()
			{
				this.mMeshDirty = true;
			}

			// Token: 0x06000EFF RID: 3839 RVA: 0x0005E0FC File Offset: 0x0005C2FC
			public void SetMesh(VertexBuffer iVertices, IndexBuffer iIndices, ModelMeshPart iMeshPart)
			{
				this.mMeshDirty = false;
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
			}

			// Token: 0x04000D61 RID: 3425
			private static Texture2D sTexture;

			// Token: 0x04000D62 RID: 3426
			public BoundingSphere mBoundingSphere;

			// Token: 0x04000D63 RID: 3427
			protected VertexDeclaration mVertexDeclaration;

			// Token: 0x04000D64 RID: 3428
			protected int mBaseVertex;

			// Token: 0x04000D65 RID: 3429
			protected int mNumVertices;

			// Token: 0x04000D66 RID: 3430
			protected int mPrimitiveCount;

			// Token: 0x04000D67 RID: 3431
			protected int mStartIndex;

			// Token: 0x04000D68 RID: 3432
			protected int mStreamOffset;

			// Token: 0x04000D69 RID: 3433
			protected int mVertexStride;

			// Token: 0x04000D6A RID: 3434
			protected int mVerticesHash;

			// Token: 0x04000D6B RID: 3435
			protected VertexBuffer mVertexBuffer;

			// Token: 0x04000D6C RID: 3436
			protected IndexBuffer mIndexBuffer;

			// Token: 0x04000D6D RID: 3437
			protected bool mMeshDirty = true;

			// Token: 0x04000D6E RID: 3438
			public Matrix[] mSkeleton;

			// Token: 0x04000D6F RID: 3439
			public Vector2 mTextureScale;

			// Token: 0x04000D70 RID: 3440
			public Vector2 mTextureOffset0;

			// Token: 0x04000D71 RID: 3441
			public Vector2 mTextureOffset1;

			// Token: 0x04000D72 RID: 3442
			public Vector2 mTextureOffset2;

			// Token: 0x04000D73 RID: 3443
			public Matrix mProjectionMatrix0;

			// Token: 0x04000D74 RID: 3444
			public Matrix mProjectionMatrix1;

			// Token: 0x04000D75 RID: 3445
			public Matrix mProjectionMatrix2;

			// Token: 0x04000D76 RID: 3446
			public Vector4 mColor;
		}

		// Token: 0x020001BE RID: 446
		protected class BarrierSkinRenderData : RenderableObject<SkinnedModelDeferredEffect, SkinnedModelDeferredAdvancedMaterial>
		{
			// Token: 0x06000F00 RID: 3840 RVA: 0x0005E180 File Offset: 0x0005C380
			public override void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				SkinnedModelDeferredEffect skinnedModelDeferredEffect = iEffect as SkinnedModelDeferredEffect;
				skinnedModelDeferredEffect.Bones = this.mSkeleton;
				if (this.RenderEarth)
				{
					this.mMaterial.AssignToEffect(skinnedModelDeferredEffect);
					skinnedModelDeferredEffect.CommitChanges();
					skinnedModelDeferredEffect.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
				}
				if (this.RenderIce)
				{
					this.mIceMaterial.AssignToEffect(skinnedModelDeferredEffect);
					skinnedModelDeferredEffect.CommitChanges();
					skinnedModelDeferredEffect.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mIceBaseVertex, 0, this.mIceNumVertices, this.mIceStartIndex, this.mIcePrimitiveCount);
				}
			}

			// Token: 0x06000F01 RID: 3841 RVA: 0x0005E220 File Offset: 0x0005C420
			public override void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				SkinnedModelDeferredEffect skinnedModelDeferredEffect = iEffect as SkinnedModelDeferredEffect;
				skinnedModelDeferredEffect.Bones = this.mSkeleton;
				if (this.RenderEarth)
				{
					this.mMaterial.AssignToEffect(skinnedModelDeferredEffect);
					skinnedModelDeferredEffect.CommitChanges();
					skinnedModelDeferredEffect.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
				}
				if (this.RenderIce)
				{
					this.mIceMaterial.AssignToEffect(skinnedModelDeferredEffect);
					skinnedModelDeferredEffect.CommitChanges();
					skinnedModelDeferredEffect.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mIceBaseVertex, 0, this.mIceNumVertices, this.mIceStartIndex, this.mIcePrimitiveCount);
				}
			}

			// Token: 0x06000F02 RID: 3842 RVA: 0x0005E2BE File Offset: 0x0005C4BE
			public void SetIceMesh(ModelMeshPart iIceMeshPart)
			{
				this.mIceBaseVertex = iIceMeshPart.BaseVertex;
				this.mIceNumVertices = iIceMeshPart.NumVertices;
				this.mIceStartIndex = iIceMeshPart.StartIndex;
				this.mIcePrimitiveCount = iIceMeshPart.PrimitiveCount;
			}

			// Token: 0x04000D77 RID: 3447
			public Matrix[] mSkeleton;

			// Token: 0x04000D78 RID: 3448
			public SkinnedModelDeferredAdvancedMaterial mIceMaterial;

			// Token: 0x04000D79 RID: 3449
			public bool RenderEarth;

			// Token: 0x04000D7A RID: 3450
			public bool RenderIce;

			// Token: 0x04000D7B RID: 3451
			protected int mIceBaseVertex;

			// Token: 0x04000D7C RID: 3452
			protected int mIceNumVertices;

			// Token: 0x04000D7D RID: 3453
			protected int mIceStartIndex;

			// Token: 0x04000D7E RID: 3454
			protected int mIcePrimitiveCount;
		}

		// Token: 0x020001BF RID: 447
		protected class HaloAuraRenderData : RenderableAdditiveObject<AdditiveEffect, AdditiveMaterial>, IPreRenderRenderer
		{
			// Token: 0x06000F04 RID: 3844 RVA: 0x0005E2F8 File Offset: 0x0005C4F8
			public HaloAuraRenderData()
			{
				this.Ray1Transform = Matrix.Identity;
				this.Ray2Transform = Matrix.Identity;
			}

			// Token: 0x06000F05 RID: 3845 RVA: 0x0005E318 File Offset: 0x0005C518
			public override void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				AdditiveEffect additiveEffect = iEffect as AdditiveEffect;
				additiveEffect.GraphicsDevice.RenderState.DepthBias = 0.00125f;
				this.mMaterial.ColorTint = this.ColorTint;
				this.mMaterial.ColorTint.W = 0.666f;
				this.mMaterial.AssignToEffect(additiveEffect);
				additiveEffect.World = this.mOrientation;
				additiveEffect.CommitChanges();
				additiveEffect.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mHaloBaseVertex, 0, this.mHaloNumvertices, this.mHaloStartIndex, this.mHaloPrimitiveCount);
				additiveEffect.GraphicsDevice.RenderState.DepthBias = 0f;
			}

			// Token: 0x06000F06 RID: 3846 RVA: 0x0005E3C0 File Offset: 0x0005C5C0
			public void PreRenderUpdate(DataChannel iDataChannel, float iDeltaTime, ref Matrix iViewProjectionMatrix, ref Vector3 iCameraPosition, ref Vector3 iCameraDirection)
			{
				Vector3 up = Vector3.Up;
				Vector3 position = this.Position;
				Vector3 forward;
				Vector3.Subtract(ref iCameraPosition, ref position, out forward);
				forward.Normalize();
				Vector3 right;
				Vector3.Cross(ref up, ref forward, out right);
				right.Normalize();
				Vector3.Cross(ref forward, ref right, out up);
				this.mOrientation.Forward = forward;
				this.mOrientation.Right = right;
				this.mOrientation.Up = up;
				this.mOrientation.M44 = 1f;
				this.mOrientation.Translation = this.Position;
			}

			// Token: 0x06000F07 RID: 3847 RVA: 0x0005E450 File Offset: 0x0005C650
			public void AssignParts(ModelMeshPart iHalo, ModelMeshPart iRays1, ModelMeshPart iRays2)
			{
				this.mHaloBaseVertex = iHalo.BaseVertex;
				this.mHaloNumvertices = iHalo.NumVertices;
				this.mHaloStartIndex = iHalo.StartIndex;
				this.mHaloPrimitiveCount = iHalo.PrimitiveCount;
				this.mRays1BaseVertex = iRays1.BaseVertex;
				this.mRays1Numvertices = iRays1.NumVertices;
				this.mRays1StartIndex = iRays1.StartIndex;
				this.mRays1PrimitiveCount = iRays1.PrimitiveCount;
				this.mRays2BaseVertex = iRays2.BaseVertex;
				this.mRays2Numvertices = iRays2.NumVertices;
				this.mRays2StartIndex = iRays2.StartIndex;
				this.mRays2PrimitiveCount = iRays2.PrimitiveCount;
			}

			// Token: 0x04000D7F RID: 3455
			public Vector3 Position;

			// Token: 0x04000D80 RID: 3456
			public Vector4 ColorTint;

			// Token: 0x04000D81 RID: 3457
			private Matrix mOrientation;

			// Token: 0x04000D82 RID: 3458
			public Matrix Ray1Transform;

			// Token: 0x04000D83 RID: 3459
			public Matrix Ray2Transform;

			// Token: 0x04000D84 RID: 3460
			private int mHaloBaseVertex;

			// Token: 0x04000D85 RID: 3461
			private int mHaloNumvertices;

			// Token: 0x04000D86 RID: 3462
			private int mHaloStartIndex;

			// Token: 0x04000D87 RID: 3463
			private int mHaloPrimitiveCount;

			// Token: 0x04000D88 RID: 3464
			private int mRays1BaseVertex;

			// Token: 0x04000D89 RID: 3465
			private int mRays1Numvertices;

			// Token: 0x04000D8A RID: 3466
			private int mRays1StartIndex;

			// Token: 0x04000D8B RID: 3467
			private int mRays1PrimitiveCount;

			// Token: 0x04000D8C RID: 3468
			private int mRays2BaseVertex;

			// Token: 0x04000D8D RID: 3469
			private int mRays2Numvertices;

			// Token: 0x04000D8E RID: 3470
			private int mRays2StartIndex;

			// Token: 0x04000D8F RID: 3471
			private int mRays2PrimitiveCount;
		}

		// Token: 0x020001C0 RID: 448
		protected class NormalDistortionRenderData : IPostEffect
		{
			// Token: 0x06000F08 RID: 3848 RVA: 0x0005E4ED File Offset: 0x0005C6ED
			public NormalDistortionRenderData(InvisibilityEffect iEffect)
			{
				this.mEffect = iEffect;
			}

			// Token: 0x170003D9 RID: 985
			// (get) Token: 0x06000F09 RID: 3849 RVA: 0x0005E503 File Offset: 0x0005C703
			public bool MeshDirty
			{
				get
				{
					return this.mMeshDirty;
				}
			}

			// Token: 0x170003DA RID: 986
			// (get) Token: 0x06000F0A RID: 3850 RVA: 0x0005E50B File Offset: 0x0005C70B
			public int ZIndex
			{
				get
				{
					return 1500;
				}
			}

			// Token: 0x06000F0B RID: 3851 RVA: 0x0005E514 File Offset: 0x0005C714
			public void Draw(float iDeltaTime, ref Vector2 iPixelSize, ref Matrix iViewMatrix, ref Matrix iProjectionMatrix, Texture2D iCandidate, Texture2D iDepthMap, Texture2D iNormalMap)
			{
				this.mEffect.PixelSize = iPixelSize;
				this.mEffect.Distortion = -0.4f;
				this.mEffect.Bones = this.mSkeleton;
				this.mEffect.SourceTexture = iCandidate;
				this.mEffect.DepthTexture = iDepthMap;
				this.mEffect.Bloat = -0.05f;
				this.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertexBuffer, 0, this.mVertexStride);
				this.mEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
				this.mEffect.GraphicsDevice.Indices = this.mIndexBuffer;
				this.mEffect.Begin();
				this.mEffect.CurrentTechnique.Passes[0].Begin();
				this.mEffect.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
				this.mEffect.CurrentTechnique.Passes[0].End();
				this.mEffect.End();
			}

			// Token: 0x06000F0C RID: 3852 RVA: 0x0005E647 File Offset: 0x0005C847
			public void SetMeshDirty()
			{
				this.mMeshDirty = true;
			}

			// Token: 0x06000F0D RID: 3853 RVA: 0x0005E650 File Offset: 0x0005C850
			public void SetMesh(VertexBuffer iVertices, IndexBuffer iIndices, ModelMeshPart iMeshPart)
			{
				this.mMeshDirty = false;
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
			}

			// Token: 0x04000D90 RID: 3472
			protected InvisibilityEffect mEffect;

			// Token: 0x04000D91 RID: 3473
			protected VertexDeclaration mVertexDeclaration;

			// Token: 0x04000D92 RID: 3474
			protected int mBaseVertex;

			// Token: 0x04000D93 RID: 3475
			protected int mNumVertices;

			// Token: 0x04000D94 RID: 3476
			protected int mPrimitiveCount;

			// Token: 0x04000D95 RID: 3477
			protected int mStartIndex;

			// Token: 0x04000D96 RID: 3478
			protected int mStreamOffset;

			// Token: 0x04000D97 RID: 3479
			protected int mVertexStride;

			// Token: 0x04000D98 RID: 3480
			public float mDamage;

			// Token: 0x04000D99 RID: 3481
			protected VertexBuffer mVertexBuffer;

			// Token: 0x04000D9A RID: 3482
			protected IndexBuffer mIndexBuffer;

			// Token: 0x04000D9B RID: 3483
			public Matrix[] mSkeleton;

			// Token: 0x04000D9C RID: 3484
			protected int mVerticesHash;

			// Token: 0x04000D9D RID: 3485
			public Matrix mCubeMapRotation;

			// Token: 0x04000D9E RID: 3486
			public float mWet;

			// Token: 0x04000D9F RID: 3487
			public bool mBurning;

			// Token: 0x04000DA0 RID: 3488
			public bool mMuddy;

			// Token: 0x04000DA1 RID: 3489
			public float mCold;

			// Token: 0x04000DA2 RID: 3490
			public float mBlack;

			// Token: 0x04000DA3 RID: 3491
			public float mBloated;

			// Token: 0x04000DA4 RID: 3492
			protected bool mMeshDirty = true;

			// Token: 0x04000DA5 RID: 3493
			public bool mIsVisible;

			// Token: 0x04000DA6 RID: 3494
			protected Character mOwner;
		}

		// Token: 0x020001C1 RID: 449
		protected class HighlightRenderData : IRenderableAdditiveObject
		{
			// Token: 0x170003DB RID: 987
			// (get) Token: 0x06000F0E RID: 3854 RVA: 0x0005E6D2 File Offset: 0x0005C8D2
			public int Effect
			{
				get
				{
					return SkinnedModelDeferredEffect.TYPEHASH;
				}
			}

			// Token: 0x170003DC RID: 988
			// (get) Token: 0x06000F0F RID: 3855 RVA: 0x0005E6D9 File Offset: 0x0005C8D9
			public int Technique
			{
				get
				{
					return (int)this.mTechnique;
				}
			}

			// Token: 0x170003DD RID: 989
			// (get) Token: 0x06000F10 RID: 3856 RVA: 0x0005E6E1 File Offset: 0x0005C8E1
			public VertexBuffer Vertices
			{
				get
				{
					return this.mVertexBuffer;
				}
			}

			// Token: 0x170003DE RID: 990
			// (get) Token: 0x06000F11 RID: 3857 RVA: 0x0005E6E9 File Offset: 0x0005C8E9
			public int VerticesHashCode
			{
				get
				{
					return this.mVerticesHash;
				}
			}

			// Token: 0x170003DF RID: 991
			// (get) Token: 0x06000F12 RID: 3858 RVA: 0x0005E6F1 File Offset: 0x0005C8F1
			public int VertexStride
			{
				get
				{
					return this.mVertexStride;
				}
			}

			// Token: 0x170003E0 RID: 992
			// (get) Token: 0x06000F13 RID: 3859 RVA: 0x0005E6F9 File Offset: 0x0005C8F9
			public IndexBuffer Indices
			{
				get
				{
					return this.mIndexBuffer;
				}
			}

			// Token: 0x170003E1 RID: 993
			// (get) Token: 0x06000F14 RID: 3860 RVA: 0x0005E701 File Offset: 0x0005C901
			public VertexDeclaration VertexDeclaration
			{
				get
				{
					return this.mVertexDeclaration;
				}
			}

			// Token: 0x06000F15 RID: 3861 RVA: 0x0005E70C File Offset: 0x0005C90C
			public bool Cull(BoundingFrustum iViewFrustum)
			{
				BoundingSphere boundingSphere = this.mBoundingSphere;
				return boundingSphere.Contains(iViewFrustum) == ContainmentType.Disjoint;
			}

			// Token: 0x06000F16 RID: 3862 RVA: 0x0005E72C File Offset: 0x0005C92C
			public void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				SkinnedModelDeferredEffect skinnedModelDeferredEffect = iEffect as SkinnedModelDeferredEffect;
				this.mMaterial.AssignToEffect(skinnedModelDeferredEffect);
				skinnedModelDeferredEffect.DiffuseColor = new Vector3(1f, 1f, 1f);
				skinnedModelDeferredEffect.FresnelPower = 4f;
				skinnedModelDeferredEffect.Bones = this.mSkeleton;
				skinnedModelDeferredEffect.CommitChanges();
				skinnedModelDeferredEffect.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
				skinnedModelDeferredEffect.Colorize = default(Vector4);
				skinnedModelDeferredEffect.DiffuseColor = new Vector3(1f);
			}

			// Token: 0x170003E2 RID: 994
			// (get) Token: 0x06000F17 RID: 3863 RVA: 0x0005E7C7 File Offset: 0x0005C9C7
			public bool MeshDirty
			{
				get
				{
					return this.mMeshDirty;
				}
			}

			// Token: 0x06000F18 RID: 3864 RVA: 0x0005E7CF File Offset: 0x0005C9CF
			public void SetMeshDirty()
			{
				this.mMeshDirty = true;
			}

			// Token: 0x06000F19 RID: 3865 RVA: 0x0005E7D8 File Offset: 0x0005C9D8
			public void SetMesh(VertexBuffer iVertices, IndexBuffer iIndices, ModelMeshPart iMeshPart)
			{
				this.mMeshDirty = false;
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
				this.mTechnique = SkinnedModelDeferredEffect.Technique.AdditiveFresnel;
			}

			// Token: 0x04000DA7 RID: 3495
			public BoundingSphere mBoundingSphere;

			// Token: 0x04000DA8 RID: 3496
			protected VertexDeclaration mVertexDeclaration;

			// Token: 0x04000DA9 RID: 3497
			protected int mBaseVertex;

			// Token: 0x04000DAA RID: 3498
			protected int mNumVertices;

			// Token: 0x04000DAB RID: 3499
			protected int mPrimitiveCount;

			// Token: 0x04000DAC RID: 3500
			protected int mStartIndex;

			// Token: 0x04000DAD RID: 3501
			protected int mStreamOffset;

			// Token: 0x04000DAE RID: 3502
			protected int mVertexStride;

			// Token: 0x04000DAF RID: 3503
			protected VertexBuffer mVertexBuffer;

			// Token: 0x04000DB0 RID: 3504
			protected IndexBuffer mIndexBuffer;

			// Token: 0x04000DB1 RID: 3505
			public Matrix[] mSkeleton;

			// Token: 0x04000DB2 RID: 3506
			public SkinnedModelDeferredAdvancedMaterial mMaterial;

			// Token: 0x04000DB3 RID: 3507
			public SkinnedModelDeferredEffect.Technique mTechnique;

			// Token: 0x04000DB4 RID: 3508
			protected int mVerticesHash;

			// Token: 0x04000DB5 RID: 3509
			protected bool mMeshDirty = true;
		}

		// Token: 0x020001C2 RID: 450
		protected class RenderData : IRenderableObject, IRenderableAdditiveObject
		{
			// Token: 0x06000F1B RID: 3867 RVA: 0x0005E870 File Offset: 0x0005CA70
			public RenderData(Character iOwner)
			{
				this.mOwner = iOwner;
				this.mSkeleton = new Matrix[80];
			}

			// Token: 0x170003E3 RID: 995
			// (get) Token: 0x06000F1C RID: 3868 RVA: 0x0005E893 File Offset: 0x0005CA93
			public bool MeshDirty
			{
				get
				{
					return this.mMeshDirty;
				}
			}

			// Token: 0x170003E4 RID: 996
			// (get) Token: 0x06000F1D RID: 3869 RVA: 0x0005E89B File Offset: 0x0005CA9B
			public int Effect
			{
				get
				{
					return SkinnedModelDeferredEffect.TYPEHASH;
				}
			}

			// Token: 0x170003E5 RID: 997
			// (get) Token: 0x06000F1E RID: 3870 RVA: 0x0005E8A2 File Offset: 0x0005CAA2
			public int DepthTechnique
			{
				get
				{
					return 3;
				}
			}

			// Token: 0x170003E6 RID: 998
			// (get) Token: 0x06000F1F RID: 3871 RVA: 0x0005E8A5 File Offset: 0x0005CAA5
			public int Technique
			{
				get
				{
					return (int)this.mTechnique;
				}
			}

			// Token: 0x170003E7 RID: 999
			// (get) Token: 0x06000F20 RID: 3872 RVA: 0x0005E8AD File Offset: 0x0005CAAD
			public int ShadowTechnique
			{
				get
				{
					return 4;
				}
			}

			// Token: 0x170003E8 RID: 1000
			// (get) Token: 0x06000F21 RID: 3873 RVA: 0x0005E8B0 File Offset: 0x0005CAB0
			public VertexBuffer Vertices
			{
				get
				{
					return this.mVertexBuffer;
				}
			}

			// Token: 0x170003E9 RID: 1001
			// (get) Token: 0x06000F22 RID: 3874 RVA: 0x0005E8B8 File Offset: 0x0005CAB8
			public IndexBuffer Indices
			{
				get
				{
					return this.mIndexBuffer;
				}
			}

			// Token: 0x170003EA RID: 1002
			// (get) Token: 0x06000F23 RID: 3875 RVA: 0x0005E8C0 File Offset: 0x0005CAC0
			public VertexDeclaration VertexDeclaration
			{
				get
				{
					return this.mVertexDeclaration;
				}
			}

			// Token: 0x170003EB RID: 1003
			// (get) Token: 0x06000F24 RID: 3876 RVA: 0x0005E8C8 File Offset: 0x0005CAC8
			public int VertexStride
			{
				get
				{
					return this.mVertexStride;
				}
			}

			// Token: 0x170003EC RID: 1004
			// (get) Token: 0x06000F25 RID: 3877 RVA: 0x0005E8D0 File Offset: 0x0005CAD0
			public int VerticesHashCode
			{
				get
				{
					return this.mVerticesHash;
				}
			}

			// Token: 0x06000F26 RID: 3878 RVA: 0x0005E8D8 File Offset: 0x0005CAD8
			public bool Cull(BoundingFrustum iViewFrustum)
			{
				BoundingSphere boundingSphere = this.mBoundingSphere;
				return boundingSphere.Contains(iViewFrustum) == ContainmentType.Disjoint;
			}

			// Token: 0x06000F27 RID: 3879 RVA: 0x0005E8F8 File Offset: 0x0005CAF8
			public virtual void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				SkinnedModelDeferredEffect skinnedModelDeferredEffect = iEffect as SkinnedModelDeferredEffect;
				this.mMaterial.AssignToEffect(skinnedModelDeferredEffect);
				skinnedModelDeferredEffect.Bones = this.mSkeleton;
				if (this.mTechnique == SkinnedModelDeferredEffect.Technique.Additive)
				{
					skinnedModelDeferredEffect.Colorize = new Vector4(Character.ColdColor, 1f);
				}
				skinnedModelDeferredEffect.GraphicsDevice.RenderState.ReferenceStencil = 2;
				skinnedModelDeferredEffect.CommitChanges();
				skinnedModelDeferredEffect.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
				skinnedModelDeferredEffect.Bloat = 0f;
				skinnedModelDeferredEffect.GraphicsDevice.RenderState.ReferenceStencil = 0;
				this.mOwner.mLastDraw = 0f;
				skinnedModelDeferredEffect.CubeMapEnabled = false;
				skinnedModelDeferredEffect.Colorize = default(Vector4);
			}

			// Token: 0x06000F28 RID: 3880 RVA: 0x0005E9C0 File Offset: 0x0005CBC0
			public virtual void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				if (this.mTechnique == SkinnedModelDeferredEffect.Technique.Additive)
				{
					return;
				}
				SkinnedModelDeferredEffect skinnedModelDeferredEffect = iEffect as SkinnedModelDeferredEffect;
				this.mMaterial.AssignOpacityToEffect(skinnedModelDeferredEffect);
				skinnedModelDeferredEffect.Bones = this.mSkeleton;
				skinnedModelDeferredEffect.CommitChanges();
				skinnedModelDeferredEffect.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
				skinnedModelDeferredEffect.Bloat = 0f;
				this.mOwner.mLastDraw = 0f;
			}

			// Token: 0x06000F29 RID: 3881 RVA: 0x0005EA3C File Offset: 0x0005CC3C
			public void SetMeshDirty()
			{
				this.mMeshDirty = true;
			}

			// Token: 0x06000F2A RID: 3882 RVA: 0x0005EA48 File Offset: 0x0005CC48
			public void SetMesh(VertexBuffer iVertices, IndexBuffer iIndices, ModelMeshPart iMeshPart, ref SkinnedModelDeferredBasicMaterial iBasicMaterial, SkinnedModelDeferredEffect.Technique iTechnique)
			{
				this.mMeshDirty = false;
				this.mMaterial.CopyFrom(ref iBasicMaterial);
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
				this.mTechnique = iTechnique;
			}

			// Token: 0x04000DB6 RID: 3510
			public BoundingSphere mBoundingSphere;

			// Token: 0x04000DB7 RID: 3511
			protected VertexDeclaration mVertexDeclaration;

			// Token: 0x04000DB8 RID: 3512
			protected int mBaseVertex;

			// Token: 0x04000DB9 RID: 3513
			protected int mNumVertices;

			// Token: 0x04000DBA RID: 3514
			protected int mPrimitiveCount;

			// Token: 0x04000DBB RID: 3515
			protected int mStartIndex;

			// Token: 0x04000DBC RID: 3516
			protected int mStreamOffset;

			// Token: 0x04000DBD RID: 3517
			protected int mVertexStride;

			// Token: 0x04000DBE RID: 3518
			protected VertexBuffer mVertexBuffer;

			// Token: 0x04000DBF RID: 3519
			protected IndexBuffer mIndexBuffer;

			// Token: 0x04000DC0 RID: 3520
			public Matrix[] mSkeleton;

			// Token: 0x04000DC1 RID: 3521
			public SkinnedModelDeferredAdvancedMaterial mMaterial;

			// Token: 0x04000DC2 RID: 3522
			public SkinnedModelDeferredEffect.Technique mTechnique;

			// Token: 0x04000DC3 RID: 3523
			protected int mVerticesHash;

			// Token: 0x04000DC4 RID: 3524
			protected bool mMeshDirty = true;

			// Token: 0x04000DC5 RID: 3525
			protected Character mOwner;
		}

		// Token: 0x020001C3 RID: 451
		public struct PointLightHolder
		{
			// Token: 0x04000DC6 RID: 3526
			public bool Enabled;

			// Token: 0x04000DC7 RID: 3527
			public bool ContainsLight;

			// Token: 0x04000DC8 RID: 3528
			public string JointName;

			// Token: 0x04000DC9 RID: 3529
			public BindJoint Joint;

			// Token: 0x04000DCA RID: 3530
			public float Radius;

			// Token: 0x04000DCB RID: 3531
			public Vector3 DiffuseColor;

			// Token: 0x04000DCC RID: 3532
			public Vector3 AmbientColor;

			// Token: 0x04000DCD RID: 3533
			public float SpecularAmount;

			// Token: 0x04000DCE RID: 3534
			public LightVariationType VariationType;

			// Token: 0x04000DCF RID: 3535
			public float VariationAmount;

			// Token: 0x04000DD0 RID: 3536
			public float VariationSpeed;
		}

		// Token: 0x020001C4 RID: 452
		public enum SelfShieldType : byte
		{
			// Token: 0x04000DD2 RID: 3538
			None,
			// Token: 0x04000DD3 RID: 3539
			Shield,
			// Token: 0x04000DD4 RID: 3540
			Earth,
			// Token: 0x04000DD5 RID: 3541
			Ice,
			// Token: 0x04000DD6 RID: 3542
			IcedEarth
		}

		// Token: 0x020001C5 RID: 453
		public struct SelfShield
		{
			// Token: 0x06000F2B RID: 3883 RVA: 0x0005EAE0 File Offset: 0x0005CCE0
			public SelfShield(Character iOwner, Spell iSpell)
			{
				if (iSpell[Elements.Earth] > 0f)
				{
					if (iSpell[Elements.Ice] > 0f)
					{
						this.mShieldType = Character.SelfShieldType.IcedEarth;
					}
					else
					{
						this.mShieldType = Character.SelfShieldType.Earth;
					}
				}
				else if (iSpell[Elements.Ice] > 0f)
				{
					this.mShieldType = Character.SelfShieldType.Ice;
				}
				else
				{
					this.mShieldType = Character.SelfShieldType.Shield;
				}
				this.mSelfShieldMaxHealth = (this.mSelfShieldHealth = 0f);
				this.mTimeStamp = iOwner.PlayState.PlayTime;
				this.mSpell = iSpell;
				this.mMagnitude = Math.Max(1f, iSpell[Elements.Ice] + iSpell[Elements.Earth]);
				this.mNoiseOffset0 = default(Vector2);
				this.mNoiseOffset1 = default(Vector2);
				this.mNoiseOffset2 = default(Vector2);
				this.mTimeSinceDamage = 1f;
				switch (this.mShieldType)
				{
				case Character.SelfShieldType.Shield:
					this.mSelfShieldMaxHealth = (this.mSelfShieldHealth = 500f);
					AudioManager.Instance.PlayCue(Banks.Spells, Character.SelfShield.SELF_SHIELD, iOwner.AudioEmitter);
					iOwner.mShieldSkinRenderData[0].SetMeshDirty();
					iOwner.mShieldSkinRenderData[1].SetMeshDirty();
					iOwner.mShieldSkinRenderData[2].SetMeshDirty();
					break;
				case Character.SelfShieldType.Earth:
				case Character.SelfShieldType.Ice:
				case Character.SelfShieldType.IcedEarth:
					iOwner.mBody.Mass = 1001f;
					for (int i = 0; i < 3; i++)
					{
						iOwner.mBarrierSkinRenderData[i].RenderEarth = false;
						iOwner.mBarrierSkinRenderData[i].RenderIce = false;
					}
					if (iSpell[Elements.Ice] > 0f)
					{
						Damage damage = new Damage(AttackProperties.Damage | AttackProperties.Piercing, Elements.Earth, Defines.SPELL_DAMAGE_BARRIER_ICE, this.mMagnitude);
						Vector3 position = iOwner.Position;
						Helper.CircleDamage(iOwner.mPlayState, iOwner, this.mTimeStamp, iOwner, ref position, 3f, ref damage);
						AudioManager.Instance.PlayCue(Banks.Spells, Character.SelfShield.SHIELD_SHIELD_ICE, iOwner.AudioEmitter);
						this.mSelfShieldMaxHealth = (this.mSelfShieldHealth = 900f);
					}
					if (iSpell[Elements.Earth] > 0f)
					{
						AudioManager.Instance.PlayCue(Banks.Spells, Character.SelfShield.SELF_SHIELD_EARTH, iOwner.AudioEmitter);
						this.mSelfShieldMaxHealth = (this.mSelfShieldHealth = 1500f);
					}
					break;
				default:
					throw new InvalidOperationException("New shields must be of type Shield, Earth or Ice");
				}
				Vector3 color = iSpell.GetColor();
				this.mHealthColor.X = color.X;
				this.mHealthColor.Y = color.Y;
				this.mHealthColor.Z = color.Z;
				this.mHealthColor.W = 1f;
			}

			// Token: 0x06000F2C RID: 3884 RVA: 0x0005ED98 File Offset: 0x0005CF98
			public void Update(Character iOwner, float iDeltaTime)
			{
				this.mTimeSinceDamage += iDeltaTime;
				if (this.mSelfShieldHealth > this.mSelfShieldMaxHealth)
				{
					this.mSelfShieldHealth = this.mSelfShieldMaxHealth;
				}
				switch (this.mShieldType)
				{
				case Character.SelfShieldType.Shield:
				{
					float num = iDeltaTime * (0.5f * (this.mSelfShieldMaxHealth - this.mSelfShieldHealth) / this.mSelfShieldMaxHealth + 0.05f);
					this.mNoiseOffset2.Y = this.mNoiseOffset2.Y - num;
					this.mNoiseOffset0.Y = this.mNoiseOffset0.Y + 0.3f * num;
					this.mNoiseOffset0.X = this.mNoiseOffset0.X - 0.7f * num;
					this.mNoiseOffset1.Y = this.mNoiseOffset1.Y + 0.7f * num;
					this.mNoiseOffset1.X = this.mNoiseOffset1.X + 0.4f * num;
					this.mSelfShieldHealth -= iDeltaTime * 100f;
					break;
				}
				case Character.SelfShieldType.Earth:
				case Character.SelfShieldType.Ice:
				case Character.SelfShieldType.IcedEarth:
					iOwner.CharacterBody.SpeedMultiplier = 0.9f - (this.mSpell[Elements.Earth] * 0.075f + this.mSpell[Elements.Ice] * 0.025f);
					break;
				}
				Vector3 position = iOwner.Position;
				position.Y -= iOwner.Capsule.Length * 0.5f + iOwner.Capsule.Radius;
				Vector2 value = default(Vector2);
				value.Y = (float)(6 * iOwner.mNumberOfHealtBars);
				Healthbars.Instance.AddHealthBar(position, this.mSelfShieldHealth / this.mSelfShieldMaxHealth, iOwner.mRadius, 1f, this.mTimeSinceDamage, false, new Vector4?(this.mHealthColor), new Vector2?(value));
				if (this.mSelfShieldHealth <= 0f)
				{
					this.Remove(iOwner);
				}
			}

			// Token: 0x06000F2D RID: 3885 RVA: 0x0005EF74 File Offset: 0x0005D174
			internal DamageResult Damage(ref float iDamage, Elements iElement)
			{
				this.mTimeSinceDamage = 0f;
				DamageResult result = DamageResult.None;
				switch (this.mShieldType)
				{
				case Character.SelfShieldType.Shield:
					if (iDamage >= 0f || (iElement & Elements.Shield) == Elements.Shield)
					{
						this.mSelfShieldHealth -= iDamage;
					}
					result = DamageResult.Deflected;
					break;
				case Character.SelfShieldType.Earth:
				case Character.SelfShieldType.Ice:
				case Character.SelfShieldType.IcedEarth:
					if (iDamage < 0f)
					{
						iDamage = 0f;
					}
					else
					{
						if ((iElement & Elements.Fire) == Elements.Fire)
						{
							iDamage *= 1f + this.mSpell[Elements.Ice];
						}
						this.mSelfShieldHealth -= iDamage;
					}
					break;
				}
				return result;
			}

			// Token: 0x06000F2E RID: 3886 RVA: 0x0005F020 File Offset: 0x0005D220
			private void ModifyDamage(ref Damage iDamage)
			{
				if (this.mShieldType == Character.SelfShieldType.Earth)
				{
					iDamage.Amount -= 10f;
				}
				else
				{
					iDamage.Amount -= 70f;
				}
				float num = 1f - (this.mSpell[Elements.Ice] * 0.05f + this.mSpell[Elements.Earth] * 0.15f);
				iDamage.Amount *= num;
			}

			// Token: 0x06000F2F RID: 3887 RVA: 0x0005F09C File Offset: 0x0005D29C
			internal DamageResult Damage(Character iOwner, ref Damage iDamage, Entity iAttacker, Vector3 iAttackPosition, Defines.DamageFeatures iFeatures)
			{
				bool flag = (iDamage.Element & Elements.PhysicalElements) != Elements.None;
				bool flag2 = (short)(iDamage.AttackProperty & AttackProperties.Damage) == 1;
				bool flag3 = (short)(iDamage.AttackProperty & AttackProperties.Status) == 32;
				bool flag4 = (short)(iDamage.AttackProperty & AttackProperties.Knockback) != 0;
				if (this.mShieldType == Character.SelfShieldType.Shield)
				{
					if (flag2 || flag3)
					{
						if (Defines.FeatureDamage(iFeatures) && iDamage.Amount * iDamage.Magnitude >= 0f && NetworkManager.Instance.State != NetworkState.Client)
						{
							this.mSelfShieldHealth -= iDamage.Amount * iDamage.Magnitude;
						}
						this.mTimeSinceDamage = 0f;
						iDamage.AttackProperty = (AttackProperties)0;
						if (flag)
						{
							iDamage.AttackProperty = AttackProperties.Knockback;
							iDamage.Magnitude = 0.6980619f;
						}
						if (flag2)
						{
							Vector3 vector = iAttackPosition;
							Vector3 position = iOwner.Position;
							Vector3.Subtract(ref vector, ref position, out position);
							position.X += 0.1f;
							position.Normalize();
							VisualEffectReference visualEffectReference;
							EffectManager.Instance.StartEffect(Character.SelfShield.PURE_SHIELD_HIT_EFFECT, ref vector, ref position, out visualEffectReference);
						}
					}
					return DamageResult.Deflected;
				}
				if (!flag2)
				{
					if (flag4)
					{
						iDamage.Amount = 0f;
						iDamage.Magnitude = 0f;
					}
					return DamageResult.None;
				}
				if (flag)
				{
					this.ModifyDamage(ref iDamage);
				}
				float num = iDamage.Amount * iDamage.Magnitude - this.mSelfShieldHealth;
				if (Defines.FeatureDamage(iFeatures) && iDamage.Amount * iDamage.Magnitude >= 0f && NetworkManager.Instance.State != NetworkState.Client)
				{
					this.mSelfShieldHealth -= iDamage.Amount * iDamage.Magnitude;
				}
				this.mTimeSinceDamage = 0f;
				if (this.mSpell[Elements.Earth] > 0f)
				{
					Vector3 vector2 = iAttackPosition;
					Vector3 position2 = iOwner.Position;
					Vector3.Subtract(ref vector2, ref position2, out position2);
					position2.X += 0.1f;
					position2.Normalize();
					VisualEffectReference visualEffectReference2;
					EffectManager.Instance.StartEffect(Character.SelfShield.EARTH_SHIELD_HIT_EFFECT, ref vector2, ref position2, out visualEffectReference2);
				}
				if (this.mSpell[Elements.Ice] > 0f)
				{
					Vector3 vector3 = iAttackPosition;
					Vector3 position3 = iOwner.Position;
					Vector3.Subtract(ref vector3, ref position3, out position3);
					position3.X += 0.1f;
					position3.Normalize();
					VisualEffectReference visualEffectReference3;
					EffectManager.Instance.StartEffect(Character.SelfShield.ICE_SHIELD_HIT_EFFECT, ref vector3, ref position3, out visualEffectReference3);
				}
				if (num > 0f)
				{
					iDamage.Amount = num;
					return DamageResult.Hit;
				}
				iDamage.Amount = 0f;
				iDamage.AttackProperty = (AttackProperties)0;
				return DamageResult.Deflected;
			}

			// Token: 0x06000F30 RID: 3888 RVA: 0x0005F32C File Offset: 0x0005D52C
			internal void Remove(Character iOwner)
			{
				iOwner.Body.Mass = iOwner.mTemplateMass;
				this.mShieldType = Character.SelfShieldType.None;
				Matrix identity = Matrix.Identity;
				identity.Translation = iOwner.Position;
				if (this.mSpell.EarthMagnitude > 0f)
				{
					VisualEffectReference visualEffectReference;
					EffectManager.Instance.StartEffect(Barrier.Earth_Barrier_Death_Effect_Hash, ref identity, out visualEffectReference);
				}
				if (this.mSpell.IceMagnitude > 0f)
				{
					VisualEffectReference visualEffectReference;
					EffectManager.Instance.StartEffect(Barrier.Ice_Barrier_Death_Effect_Hash, ref identity, out visualEffectReference);
				}
				iOwner.ClearAura();
			}

			// Token: 0x170003ED RID: 1005
			// (get) Token: 0x06000F31 RID: 3889 RVA: 0x0005F3B6 File Offset: 0x0005D5B6
			public bool Solid
			{
				get
				{
					return this.mShieldType == Character.SelfShieldType.Ice || this.mShieldType == Character.SelfShieldType.Earth || this.mShieldType == Character.SelfShieldType.IcedEarth;
				}
			}

			// Token: 0x170003EE RID: 1006
			// (get) Token: 0x06000F32 RID: 3890 RVA: 0x0005F3D5 File Offset: 0x0005D5D5
			internal bool Active
			{
				get
				{
					return this.mShieldType != Character.SelfShieldType.None;
				}
			}

			// Token: 0x04000DD7 RID: 3543
			public static readonly int SELF_SHIELD = "spell_shield_self".GetHashCodeCustom();

			// Token: 0x04000DD8 RID: 3544
			public static readonly int SELF_SHIELD_EARTH = "spell_shield_self_earth".GetHashCodeCustom();

			// Token: 0x04000DD9 RID: 3545
			public static readonly int SHIELD_SHIELD_ICE = "spell_shield_self_ice".GetHashCodeCustom();

			// Token: 0x04000DDA RID: 3546
			public static readonly int PURE_SHIELD_HIT_EFFECT = "pure_self_shield_hit".GetHashCodeCustom();

			// Token: 0x04000DDB RID: 3547
			public static readonly int EARTH_SHIELD_HIT_EFFECT = "earth_self_shield_hit".GetHashCodeCustom();

			// Token: 0x04000DDC RID: 3548
			public static readonly int ICE_SHIELD_HIT_EFFECT = "ice_self_shield_hit".GetHashCodeCustom();

			// Token: 0x04000DDD RID: 3549
			internal Character.SelfShieldType mShieldType;

			// Token: 0x04000DDE RID: 3550
			internal float mMagnitude;

			// Token: 0x04000DDF RID: 3551
			private readonly float mSelfShieldMaxHealth;

			// Token: 0x04000DE0 RID: 3552
			internal float mSelfShieldHealth;

			// Token: 0x04000DE1 RID: 3553
			internal Spell mSpell;

			// Token: 0x04000DE2 RID: 3554
			private double mTimeStamp;

			// Token: 0x04000DE3 RID: 3555
			private Vector4 mHealthColor;

			// Token: 0x04000DE4 RID: 3556
			public float mTimeSinceDamage;

			// Token: 0x04000DE5 RID: 3557
			public Vector2 mNoiseOffset0;

			// Token: 0x04000DE6 RID: 3558
			public Vector2 mNoiseOffset1;

			// Token: 0x04000DE7 RID: 3559
			public Vector2 mNoiseOffset2;
		}

		// Token: 0x020001C6 RID: 454
		public enum DamageAssistance : byte
		{
			// Token: 0x04000DE9 RID: 3561
			None,
			// Token: 0x04000DEA RID: 3562
			Damaged,
			// Token: 0x04000DEB RID: 3563
			Killed
		}
	}
}
