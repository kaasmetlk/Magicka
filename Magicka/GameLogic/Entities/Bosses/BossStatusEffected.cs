using System;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.UI;
using Magicka.Graphics;
using Microsoft.Xna.Framework;

namespace Magicka.GameLogic.Entities.Bosses
{
	// Token: 0x020000E7 RID: 231
	public abstract class BossStatusEffected
	{
		// Token: 0x17000173 RID: 371
		// (get) Token: 0x0600071E RID: 1822 RVA: 0x00029A18 File Offset: 0x00027C18
		internal Resistance[] Resistances
		{
			get
			{
				return this.mResistances;
			}
		}

		// Token: 0x17000174 RID: 372
		// (get) Token: 0x0600071F RID: 1823
		protected abstract BossDamageZone Entity { get; }

		// Token: 0x17000175 RID: 373
		// (get) Token: 0x06000720 RID: 1824
		protected abstract float Radius { get; }

		// Token: 0x17000176 RID: 374
		// (get) Token: 0x06000721 RID: 1825
		protected abstract float Length { get; }

		// Token: 0x17000177 RID: 375
		// (get) Token: 0x06000722 RID: 1826
		public abstract bool Dead { get; }

		// Token: 0x17000178 RID: 376
		// (get) Token: 0x06000723 RID: 1827
		protected abstract int BloodEffect { get; }

		// Token: 0x17000179 RID: 377
		// (get) Token: 0x06000724 RID: 1828
		protected abstract Vector3 NotifierTextPostion { get; }

		// Token: 0x06000725 RID: 1829 RVA: 0x00029A20 File Offset: 0x00027C20
		protected virtual void UpdateDamage(float iDeltaTime)
		{
			Vector3 notifierTextPostion = this.NotifierTextPostion;
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
					DamageNotifyer.Instance.UpdateNumberPosition(this.mLastDamageIndex, ref notifierTextPostion);
				}
			}
			this.mHealingAccumulationTimer -= iDeltaTime;
			if (this.mHealingAccumulationTimer <= 0f)
			{
				this.mHealingAccumulationTimer += 1f;
				if (this.mHealingAccumulation != 0f)
				{
					DamageNotifyer.Instance.AddNumber(this.mHealingAccumulation, ref notifierTextPostion, 0.7f, false);
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
					DamageNotifyer.Instance.AddNumber(this.mFireDamageAccumulation, ref notifierTextPostion, 0.7f, false);
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
					DamageNotifyer.Instance.AddNumber(this.mPoisonDamageAccumulation, ref notifierTextPostion, 0.7f, false);
					this.mPoisonDamageAccumulation = 0f;
					this.mTimeSinceLastStatusDamage = 0f;
				}
			}
		}

		// Token: 0x06000726 RID: 1830 RVA: 0x00029BD4 File Offset: 0x00027DD4
		protected virtual void UpdateStatusEffects(float iDeltaTime)
		{
			this.mDryTimer -= iDeltaTime;
			StatusEffects statusEffects = StatusEffects.None;
			if (this.Dead)
			{
				for (int i = 0; i < this.mStatusEffects.Length; i++)
				{
					this.mStatusEffects[i].Stop();
					this.mStatusEffects[i] = default(StatusEffect);
				}
			}
			else
			{
				for (int j = 0; j < this.mStatusEffects.Length; j++)
				{
					this.mStatusEffects[j].Update(iDeltaTime, this.Entity);
					if (this.mStatusEffects[j].Dead)
					{
						this.mStatusEffects[j].Stop();
						this.mStatusEffects[j] = default(StatusEffect);
					}
					else if (this.mStatusEffects[j].DamageType == StatusEffects.Wet)
					{
						if (this.mStatusEffects[j].Magnitude >= 1f)
						{
							statusEffects |= this.mStatusEffects[j].DamageType;
						}
					}
					else
					{
						statusEffects |= this.mStatusEffects[j].DamageType;
					}
				}
			}
			this.mCurrentStatusEffects = statusEffects;
		}

		// Token: 0x06000727 RID: 1831 RVA: 0x00029CFC File Offset: 0x00027EFC
		public DamageResult AddStatusEffect(StatusEffect iStatusEffect)
		{
			DamageResult damageResult = DamageResult.None;
			if (!iStatusEffect.Dead)
			{
				bool flag = false;
				StatusEffects damageType = iStatusEffect.DamageType;
				if (damageType <= StatusEffects.Poisoned)
				{
					switch (damageType)
					{
					case StatusEffects.Burning:
						if (!this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Cold)].Dead || !this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Wet)].Dead || !this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Frozen)].Dead || this.mDryTimer > 0f)
						{
							this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Cold)].Stop();
							this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Cold)] = default(StatusEffect);
							this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Wet)].Stop();
							this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Wet)] = default(StatusEffect);
							this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Frozen)].Stop();
							this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Frozen)] = default(StatusEffect);
							flag = true;
						}
						break;
					case StatusEffects.Wet:
						if (this.HasStatus(StatusEffects.Burning) || this.mDryTimer > 0f)
						{
							int num = StatusEffect.StatusIndex(StatusEffects.Burning);
							this.mStatusEffects[num].Stop();
							this.mStatusEffects[num] = default(StatusEffect);
							flag = true;
						}
						if (this.HasStatus(StatusEffects.Greased))
						{
							int num2 = StatusEffect.StatusIndex(StatusEffects.Greased);
							this.mStatusEffects[num2].Stop();
							this.mStatusEffects[num2] = default(StatusEffect);
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
							float num3 = iStatusEffect.Magnitude;
							if (!this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Burning)].Dead || this.mDryTimer > 0f)
							{
								this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Burning)].Stop();
								this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Burning)] = default(StatusEffect);
								this.mDryTimer = 0.9f;
								flag = true;
							}
							num3 *= this.mResistances[Defines.ElementIndex(Elements.Cold)].Multiplier;
							if (this.HasStatus(StatusEffects.Wet))
							{
								this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Frozen)] = new StatusEffect(StatusEffects.Frozen, 0f, num3, this.Length, this.Radius);
								this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Wet)] = default(StatusEffect);
							}
							if (this.HasStatus(StatusEffects.Frozen))
							{
								StatusEffect[] array = this.mStatusEffects;
								int num4 = StatusEffect.StatusIndex(StatusEffects.Frozen);
								array[num4].Magnitude = array[num4].Magnitude + num3;
								num3 = 0f;
							}
							iStatusEffect.Magnitude = num3;
						}
						break;
					}
				}
				else if (damageType != StatusEffects.Healing)
				{
					if (damageType != StatusEffects.Greased)
					{
						if (damageType != StatusEffects.Steamed)
						{
						}
					}
					else if (this.HasStatus(StatusEffects.Wet))
					{
						int num5 = StatusEffect.StatusIndex(StatusEffects.Wet);
						this.mStatusEffects[num5].Stop();
						this.mStatusEffects[num5] = default(StatusEffect);
					}
				}
				else if (this.HasStatus(StatusEffects.Poisoned))
				{
					this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Poisoned)].Stop();
					this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Poisoned)] = default(StatusEffect);
				}
				if (!flag)
				{
					int num6 = StatusEffect.StatusIndex(iStatusEffect.DamageType);
					this.mStatusEffects[num6] = this.mStatusEffects[num6] + iStatusEffect;
					damageResult |= DamageResult.Statusadded;
				}
				else
				{
					damageResult |= DamageResult.Statusremoved;
				}
			}
			return damageResult;
		}

		// Token: 0x06000728 RID: 1832 RVA: 0x0002A0C8 File Offset: 0x000282C8
		public bool HasStatus(StatusEffects iStatus)
		{
			return (this.mCurrentStatusEffects & iStatus) == iStatus;
		}

		// Token: 0x06000729 RID: 1833 RVA: 0x0002A0D6 File Offset: 0x000282D6
		public StatusEffect[] GetStatusEffects()
		{
			return this.mStatusEffects;
		}

		// Token: 0x0600072A RID: 1834 RVA: 0x0002A0DE File Offset: 0x000282DE
		public float StatusMagnitude(StatusEffects iStatus)
		{
			return this.mStatusEffects[StatusEffect.StatusIndex(iStatus)].Magnitude;
		}

		// Token: 0x0600072B RID: 1835 RVA: 0x0002A0F8 File Offset: 0x000282F8
		protected void Damage(float iDamage, Elements iElement)
		{
			if ((iElement & Elements.Fire) == Elements.Fire && this.HasStatus(StatusEffects.Greased))
			{
				iDamage *= 2f;
			}
			this.mHitPoints -= iDamage;
			if (iElement != Elements.Fire)
			{
				if (iElement != Elements.Life)
				{
					if (iElement == Elements.Poison)
					{
						this.mPoisonDamageAccumulation += iDamage;
					}
				}
				else if (this.mHitPoints < this.mMaxHitPoints)
				{
					this.mHealingAccumulation += iDamage;
				}
			}
			else
			{
				this.mFireDamageAccumulation += iDamage;
			}
			if (this.mHitPoints > this.mMaxHitPoints)
			{
				this.mHitPoints = this.mMaxHitPoints;
			}
		}

		// Token: 0x0600072C RID: 1836 RVA: 0x0002A198 File Offset: 0x00028398
		protected virtual DamageResult Damage(Damage iDamage, Entity iAttacker, Vector3 iAttackPosition, Defines.DamageFeatures iFeatures)
		{
			if (this.Dead)
			{
				return DamageResult.Deflected;
			}
			Damage damage = iDamage;
			DamageResult damageResult = DamageResult.None;
			float num = 0f;
			float num2 = 0f;
			for (int i = 0; i < this.mResistances.Length; i++)
			{
				Elements elements = Defines.ElementFromIndex(i);
				if ((damage.Element & elements) == elements)
				{
					if (damage.Element == Elements.Earth && this.mResistances[i].Modifier != 0f)
					{
						damage.Amount = (float)((int)Math.Max(damage.Amount + this.mResistances[i].Modifier, 0f));
					}
					else
					{
						damage.Amount += (float)((int)this.mResistances[i].Modifier);
					}
					num += this.mResistances[i].Multiplier;
					num2 += 1f;
				}
			}
			if (num2 != 0f)
			{
				damage.Magnitude *= num / num2;
			}
			if (Math.Abs(damage.Magnitude) <= 1E-45f)
			{
				damageResult |= DamageResult.Deflected;
			}
			if ((damageResult & DamageResult.Deflected) == DamageResult.Deflected)
			{
				return damageResult;
			}
			if ((short)(damage.AttackProperty & AttackProperties.Status) == 32 && Math.Abs(num) > 1E-45f)
			{
				if ((damage.Element & Elements.Fire) == Elements.Fire && this.mResistances[Spell.ElementIndex(Elements.Fire)].Multiplier > 1E-45f)
				{
					damageResult |= this.AddStatusEffect(new StatusEffect(StatusEffects.Burning, damage.Amount, damage.Magnitude, this.Length, this.Radius));
				}
				if ((damage.Element & Elements.Cold) == Elements.Cold && this.mResistances[Spell.ElementIndex(Elements.Cold)].Multiplier > 1E-45f)
				{
					damageResult |= this.AddStatusEffect(new StatusEffect(StatusEffects.Cold, damage.Amount, damage.Magnitude, this.Length, this.Radius));
				}
				if ((damage.Element & Elements.Water) == Elements.Water && this.mResistances[Spell.ElementIndex(Elements.Water)].Multiplier > 1E-45f)
				{
					damageResult |= this.AddStatusEffect(new StatusEffect(StatusEffects.Wet, damage.Amount, damage.Magnitude, this.Length, this.Radius));
				}
				if ((damage.Element & Elements.Poison) == Elements.Poison && this.mResistances[Spell.ElementIndex(Elements.Poison)].Multiplier > 1E-45f)
				{
					damageResult |= this.AddStatusEffect(new StatusEffect(StatusEffects.Poisoned, damage.Amount, damage.Magnitude, this.Length, this.Radius));
				}
				if ((damage.Element & Elements.Life) == Elements.Life && this.mResistances[Spell.ElementIndex(Elements.Life)].Multiplier > 1E-45f)
				{
					damageResult |= this.AddStatusEffect(new StatusEffect(StatusEffects.Healing, damage.Amount, damage.Magnitude, this.Length, this.Radius));
				}
				if ((damage.Element & Elements.Steam) == Elements.Steam && this.mResistances[Spell.ElementIndex(Elements.Steam)].Multiplier > 1E-45f)
				{
					damageResult |= this.AddStatusEffect(new StatusEffect(StatusEffects.Wet, damage.Amount, damage.Magnitude, this.Length, this.Radius));
				}
			}
			if ((short)(damage.AttackProperty & AttackProperties.Damage) == 1)
			{
				if ((damage.Element & Elements.Lightning) == Elements.Lightning && this.HasStatus(StatusEffects.Wet))
				{
					damage.Amount *= 2f;
				}
				if ((damage.Element & Elements.Life) == Elements.Life)
				{
					StatusEffect[] array = this.mStatusEffects;
					int num3 = StatusEffect.StatusIndex(StatusEffects.Poisoned);
					array[num3].Magnitude = array[num3].Magnitude - damage.Magnitude;
					if (this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Poisoned)].Magnitude <= 0f)
					{
						this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Poisoned)].Stop();
					}
				}
				if ((damage.Element & Elements.PhysicalElements) != Elements.None)
				{
					if (this.HasStatus(StatusEffects.Frozen))
					{
						damage.Amount = Math.Max(damage.Amount - 200f, 0f);
						damage.Magnitude = Math.Max(1f, damage.Magnitude);
						damage.Amount *= 3f;
					}
					else if (GlobalSettings.Instance.BloodAndGore == SettingOptions.On)
					{
						Vector3 vector = iAttackPosition;
						Vector3 right = Vector3.Right;
						VisualEffectReference visualEffectReference;
						EffectManager.Instance.StartEffect(this.BloodEffect, ref vector, ref right, out visualEffectReference);
					}
				}
				damage.Amount *= damage.Magnitude;
				this.mHitPoints -= damage.Amount;
				if ((short)(damage.AttackProperty & AttackProperties.Piercing) != 0 && damage.Magnitude > 0f && damage.Amount > 0f)
				{
					damageResult |= DamageResult.Pierced;
				}
				if (damage.Amount > 0f)
				{
					damageResult |= DamageResult.Damaged;
				}
				if (damage.Amount == 0f)
				{
					damageResult |= DamageResult.Deflected;
				}
				if (damage.Amount < 0f)
				{
					damageResult |= DamageResult.Healed;
				}
				damageResult |= DamageResult.Hit;
				if (damage.Amount != 0f)
				{
					this.mTimeSinceLastDamage = 0f;
				}
				if (Defines.FeatureNotify(iFeatures))
				{
					if (this.mLastDamageIndex >= 0)
					{
						DamageNotifyer.Instance.AddToNumber(this.mLastDamageIndex, damage.Amount);
					}
					else
					{
						if (this.mLastDamageIndex >= 0)
						{
							DamageNotifyer.Instance.ReleasNumber(this.mLastDamageIndex);
						}
						this.mLastDamageAmount = damage.Amount;
						this.mLastDamageElement = damage.Element;
						Vector3 notifierTextPostion = this.NotifierTextPostion;
						this.mLastDamageIndex = DamageNotifyer.Instance.AddNumber(damage.Amount, ref notifierTextPostion, 0.4f, true);
					}
				}
			}
			if (this.mHitPoints > this.mMaxHitPoints)
			{
				this.mHitPoints = this.mMaxHitPoints;
			}
			if (damage.Amount == 0f)
			{
				damageResult |= DamageResult.Deflected;
			}
			if (this.mHitPoints <= 0f)
			{
				damageResult |= DamageResult.Killed;
			}
			return damageResult;
		}

		// Token: 0x040005BC RID: 1468
		protected float mHitPoints;

		// Token: 0x040005BD RID: 1469
		protected float mMaxHitPoints;

		// Token: 0x040005BE RID: 1470
		protected StatusEffects mCurrentStatusEffects;

		// Token: 0x040005BF RID: 1471
		protected StatusEffect[] mStatusEffects = new StatusEffect[9];

		// Token: 0x040005C0 RID: 1472
		protected Resistance[] mResistances = new Resistance[11];

		// Token: 0x040005C1 RID: 1473
		protected float mFireDamageAccumulation;

		// Token: 0x040005C2 RID: 1474
		protected float mFireDamageAccumulationTimer = 0.25f;

		// Token: 0x040005C3 RID: 1475
		protected float mPoisonDamageAccumulation;

		// Token: 0x040005C4 RID: 1476
		protected float mPoisonDamageAccumulationTimer = 0.25f;

		// Token: 0x040005C5 RID: 1477
		protected float mHealingAccumulation;

		// Token: 0x040005C6 RID: 1478
		protected float mHealingAccumulationTimer = 0.25f;

		// Token: 0x040005C7 RID: 1479
		protected int mLastDamageIndex;

		// Token: 0x040005C8 RID: 1480
		protected float mLastDamageAmount;

		// Token: 0x040005C9 RID: 1481
		protected Elements mLastDamageElement;

		// Token: 0x040005CA RID: 1482
		protected float mTimeSinceLastDamage;

		// Token: 0x040005CB RID: 1483
		protected float mTimeSinceLastStatusDamage;

		// Token: 0x040005CC RID: 1484
		protected float mDryTimer;

		// Token: 0x040005CD RID: 1485
		protected VisualEffectReference mDryingEffect;
	}
}
