using System;
using Magicka.GameLogic.Entities;
using Magicka.Graphics;
using Microsoft.Xna.Framework;
using PolygonHead.ParticleEffects;

namespace Magicka.GameLogic.Spells
{
	// Token: 0x02000503 RID: 1283
	public struct StatusEffect : IEquatable<StatusEffect>
	{
		// Token: 0x06002607 RID: 9735 RVA: 0x00112C1C File Offset: 0x00110E1C
		public StatusEffect(StatusEffects iStatus, float iDPS, float iMagnitude, float iLength, float iRadius)
		{
			this.mRadius = iRadius;
			this.mLength = iLength;
			this.mDamageType = iStatus;
			this.mDPS = iDPS;
			Matrix matrix = default(Matrix);
			matrix.M11 = iRadius;
			matrix.M22 = 0.5f * iLength + iRadius;
			matrix.M33 = iRadius;
			matrix.M44 = 1f;
			matrix.M41 = float.MaxValue;
			matrix.M42 = float.MaxValue;
			matrix.M43 = float.MaxValue;
			this.mEmitterA = default(VisualEffect);
			this.mMagnitude = iMagnitude;
			this.mParticleMagntiude = (int)Math.Ceiling((double)this.mMagnitude);
			if (iStatus <= StatusEffects.Healing)
			{
				switch (iStatus)
				{
				case StatusEffects.Wet:
				case StatusEffects.Frozen:
					this.mEmitterA = EffectManager.Instance.GetEffect(StatusEffect.EFFECTS_STATUS[StatusEffect.StatusIndex(iStatus)]);
					this.mEmitterA.Start(ref matrix);
					break;
				case StatusEffects.Burning | StatusEffects.Wet:
					break;
				default:
					if (iStatus == StatusEffects.Healing)
					{
						this.mMagnitude = 0.5f + iMagnitude / 2f;
						this.mEmitterA = EffectManager.Instance.GetEffect(StatusEffect.EFFECTS_STATUS[StatusEffect.StatusIndex(iStatus)]);
						this.mEmitterA.Start(ref matrix);
					}
					break;
				}
			}
			else if (iStatus != StatusEffects.Steamed)
			{
				if (iStatus == StatusEffects.Bleeding)
				{
					this.mMagnitude = 1f;
					this.mEmitterA = EffectManager.Instance.GetEffect(StatusEffect.BLEED_EFFECT[0]);
					this.mEmitterA.Start(ref matrix);
				}
			}
			else
			{
				this.mEmitterA = EffectManager.Instance.GetEffect(Defines.STATUS_DRYING_EFFECT_HASH);
				this.mMagnitude = 1f;
				this.mEmitterA.Start(ref matrix);
			}
			this.mVomitEffect = default(VisualEffectReference);
			this.mParticleTimer = 0f;
			this.mDamageRest = 0f;
		}

		// Token: 0x06002608 RID: 9736 RVA: 0x00112DF2 File Offset: 0x00110FF2
		public static int StatusIndex(StatusEffects iStatus)
		{
			return (int)(Math.Log((double)iStatus) * StatusEffect.ONEOVERLN2 + 0.5);
		}

		// Token: 0x06002609 RID: 9737 RVA: 0x00112E0C File Offset: 0x0011100C
		public static StatusEffects StatusFromIndex(int iIndex)
		{
			int num = (int)(Math.Pow(2.0, (double)iIndex) + 0.5);
			return (StatusEffects)num;
		}

		// Token: 0x0600260A RID: 9738 RVA: 0x00112E3C File Offset: 0x0011103C
		public void Update(float iDeltaTime, IStatusEffected iOwner)
		{
			this.Update(iDeltaTime, iOwner, null);
		}

		// Token: 0x0600260B RID: 9739 RVA: 0x00112E5C File Offset: 0x0011105C
		public void Update(float iDeltaTime, IStatusEffected iOwner, Vector3? iPosition)
		{
			if (this.mMagnitude <= 0f)
			{
				return;
			}
			float num = 1f;
			Vector3 vector = iPosition.GetValueOrDefault(iOwner.Position);
			this.mParticleTimer -= iDeltaTime;
			Matrix matrix = default(Matrix);
			StatusEffects statusEffects = this.mDamageType;
			if (statusEffects <= StatusEffects.Poisoned)
			{
				switch (statusEffects)
				{
				case StatusEffects.Burning:
				{
					num = this.mEmitterA.Transform.M22 * 1.2f;
					this.mMagnitude = Math.Min(this.mMagnitude, 3f);
					this.mMagnitude -= iDeltaTime * 0.2f;
					this.mDPS = Math.Min(this.mDPS + iDeltaTime * this.mDPS * 0.25f, 500f * this.mMagnitude);
					int num2 = (int)Math.Ceiling((double)this.mMagnitude);
					this.mDamageRest += this.mDPS * iDeltaTime;
					int num3 = (int)MathHelper.Clamp(this.mDPS / 100f, 0f, 2f);
					if (this.mParticleMagntiude != num3 || !this.mEmitterA.IsActive)
					{
						this.mParticleMagntiude = num3;
						this.mEmitterA.Stop();
						matrix = default(Matrix);
						matrix.M11 = this.mRadius;
						matrix.M22 = 1f + this.mRadius;
						matrix.M33 = this.mRadius;
						matrix.M44 = this.mLength;
						matrix.M41 = float.MaxValue;
						matrix.M42 = float.MaxValue;
						matrix.M43 = float.MaxValue;
						this.mEmitterA = EffectManager.Instance.GetEffect(StatusEffect.EFFECT_BURNING[num3]);
						this.mEmitterA.Start(ref matrix);
					}
					break;
				}
				case StatusEffects.Wet:
					num = 1f;
					this.mMagnitude = Math.Min(this.mMagnitude, 1f);
					break;
				case StatusEffects.Burning | StatusEffects.Wet:
					break;
				case StatusEffects.Frozen:
					this.mMagnitude = Math.Min(this.mMagnitude, 3f);
					this.mMagnitude -= iDeltaTime * 0.25f;
					break;
				default:
					if (statusEffects != StatusEffects.Cold)
					{
						if (statusEffects == StatusEffects.Poisoned)
						{
							if (this.mMagnitude > 1f)
							{
								this.mMagnitude = 1f;
							}
							this.mDamageRest += iDeltaTime * this.mDPS * this.mMagnitude;
							Character character = iOwner as Character;
							if (character != null)
							{
								vector = character.GetMouthAttachOrientation().Translation;
								if (!character.PlayState.IsInCutscene)
								{
									character.CharacterBody.SpeedMultiplier = Math.Min(character.CharacterBody.SpeedMultiplier, this.GetSlowdown());
								}
								Matrix mouthAttachOrientation = character.GetMouthAttachOrientation();
								if (this.mParticleTimer < 0f)
								{
									EffectManager.Instance.StartEffect(StatusEffect.POISON_VOMIT, ref mouthAttachOrientation, out this.mVomitEffect);
									this.mParticleTimer += MagickaMath.RandomBetween(3f, 7f) / this.mMagnitude;
								}
								EffectManager.Instance.UpdateOrientation(ref this.mVomitEffect, ref mouthAttachOrientation);
							}
						}
					}
					else
					{
						num = (float)Math.Sqrt((double)(this.mMagnitude * 0.2f)) * 0.2f;
						Character character2 = iOwner as Character;
						if (character2 != null)
						{
							float slowdown = this.GetSlowdown();
							character2.CharacterBody.SpeedMultiplier = Math.Min(character2.CharacterBody.SpeedMultiplier, slowdown);
						}
						this.mMagnitude -= iDeltaTime * 0.1f;
						this.mMagnitude = Math.Min(this.mMagnitude, 3f);
						int num2 = (int)Math.Ceiling((double)this.mMagnitude);
						if (num2 > 0 && (this.mParticleMagntiude != num2 || !this.mEmitterA.IsActive))
						{
							this.mEmitterA.Stop();
							this.mParticleMagntiude = num2;
							matrix = default(Matrix);
							matrix.M11 = this.mRadius;
							matrix.M22 = 1f + this.mRadius;
							matrix.M33 = this.mRadius;
							matrix.M44 = this.mLength;
							matrix.M41 = float.MaxValue;
							matrix.M42 = float.MaxValue;
							matrix.M43 = float.MaxValue;
							int num4 = Math.Min(Math.Max(this.mParticleMagntiude - 1, 0), StatusEffect.EFFECT_COLD.Length - 1);
							this.mEmitterA = EffectManager.Instance.GetEffect(StatusEffect.EFFECT_COLD[num4]);
							this.mEmitterA.Start(ref matrix);
						}
					}
					break;
				}
			}
			else if (statusEffects != StatusEffects.Healing)
			{
				if (statusEffects != StatusEffects.Steamed)
				{
					if (statusEffects == StatusEffects.Bleeding)
					{
						this.mDamageRest += iDeltaTime * this.mDPS;
					}
				}
				else
				{
					this.mMagnitude = Math.Min(1f, this.mMagnitude);
					this.mMagnitude -= iDeltaTime;
				}
			}
			else
			{
				this.mDamageRest += iDeltaTime * this.mDPS;
				if (this.mMagnitude > 3f)
				{
					this.mMagnitude = 3f;
				}
				if (iOwner.HitPoints >= iOwner.MaxHitPoints)
				{
					this.mMagnitude = iDeltaTime;
				}
				this.mMagnitude -= iDeltaTime;
				num = 1f;
			}
			int num5 = (int)this.mDamageRest;
			if (num5 != 0)
			{
				this.mDamageRest -= (float)num5;
				StatusEffects statusEffects2 = this.mDamageType;
				Elements iElement;
				if (statusEffects2 != StatusEffects.Burning)
				{
					if (statusEffects2 != StatusEffects.Poisoned)
					{
						if (statusEffects2 != StatusEffects.Healing)
						{
							iElement = Elements.Earth;
						}
						else
						{
							iElement = Elements.Life;
						}
					}
					else
					{
						iElement = Elements.Poison;
					}
				}
				else
				{
					iElement = Elements.Fire;
				}
				iOwner.Damage((float)num5, iElement);
			}
			float iDeltaTime2 = iDeltaTime * num;
			this.mEmitterA.Transform.M41 = vector.X;
			this.mEmitterA.Transform.M42 = vector.Y;
			this.mEmitterA.Transform.M43 = vector.Z;
			this.mEmitterA.Update(iDeltaTime2);
		}

		// Token: 0x0600260C RID: 9740 RVA: 0x00113458 File Offset: 0x00111658
		internal void StopEffect()
		{
			if (this.mEmitterA.IsActive)
			{
				this.mEmitterA.Stop();
			}
		}

		// Token: 0x0600260D RID: 9741 RVA: 0x00113474 File Offset: 0x00111674
		internal float GetSlowdown()
		{
			if (this.mDamageType == StatusEffects.Cold)
			{
				return (float)Math.Pow(0.001, (double)this.mMagnitude) * 0.667f + 0.333f;
			}
			if (this.mDamageType == StatusEffects.Poisoned)
			{
				return 0.5f;
			}
			return 1f;
		}

		// Token: 0x170008F2 RID: 2290
		// (get) Token: 0x0600260E RID: 9742 RVA: 0x001134C2 File Offset: 0x001116C2
		public StatusEffects DamageType
		{
			get
			{
				return this.mDamageType;
			}
		}

		// Token: 0x170008F3 RID: 2291
		// (get) Token: 0x0600260F RID: 9743 RVA: 0x001134CA File Offset: 0x001116CA
		// (set) Token: 0x06002610 RID: 9744 RVA: 0x001134D2 File Offset: 0x001116D2
		public float Magnitude
		{
			get
			{
				return this.mMagnitude;
			}
			set
			{
				this.mMagnitude = value;
			}
		}

		// Token: 0x170008F4 RID: 2292
		// (get) Token: 0x06002611 RID: 9745 RVA: 0x001134DB File Offset: 0x001116DB
		// (set) Token: 0x06002612 RID: 9746 RVA: 0x001134E3 File Offset: 0x001116E3
		public float DPS
		{
			get
			{
				return this.mDPS;
			}
			set
			{
				this.mDPS = value;
			}
		}

		// Token: 0x170008F5 RID: 2293
		// (get) Token: 0x06002613 RID: 9747 RVA: 0x001134EC File Offset: 0x001116EC
		public bool Dead
		{
			get
			{
				return this.mMagnitude <= 0f;
			}
		}

		// Token: 0x06002614 RID: 9748 RVA: 0x001134FE File Offset: 0x001116FE
		public bool Equals(StatusEffect obj)
		{
			return obj.mDamageType == this.mDamageType;
		}

		// Token: 0x06002615 RID: 9749 RVA: 0x0011350F File Offset: 0x0011170F
		public override bool Equals(object obj)
		{
			if (obj is StatusEffect)
			{
				return this.Equals((StatusEffect)obj);
			}
			return base.Equals(obj);
		}

		// Token: 0x06002616 RID: 9750 RVA: 0x00113538 File Offset: 0x00111738
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"Effect: ",
				this.mDamageType.ToString(),
				" Magnitude: ",
				this.mMagnitude
			});
		}

		// Token: 0x06002617 RID: 9751 RVA: 0x00113583 File Offset: 0x00111783
		public override int GetHashCode()
		{
			return this.mDamageType.GetHashCode();
		}

		// Token: 0x06002618 RID: 9752 RVA: 0x00113598 File Offset: 0x00111798
		public static StatusEffect operator +(StatusEffect A, StatusEffect B)
		{
			if (A.Dead | A.mDamageType == StatusEffects.None)
			{
				return B;
			}
			if (B.Dead | B.mDamageType == StatusEffects.None)
			{
				return A;
			}
			if (A.mDamageType != B.mDamageType)
			{
				throw new Exception("Cannot add status effects. Both effects must be of the same type. " + A.ToString() + " and " + B.ToString());
			}
			StatusEffect result = default(StatusEffect);
			result.mParticleTimer = Math.Max(A.mParticleTimer, B.mParticleTimer);
			result.mDamageRest = A.mDamageRest + B.mDamageRest;
			result.mEmitterA = A.mEmitterA;
			if (A.DamageType == StatusEffects.Healing)
			{
				result.mMagnitude = Math.Max(A.mMagnitude, B.mMagnitude);
			}
			else
			{
				result.mMagnitude = A.mMagnitude + B.mMagnitude;
			}
			result.mDamageType = A.mDamageType;
			result.mParticleMagntiude = Math.Max(A.mParticleMagntiude, B.mParticleMagntiude);
			result.mLength = A.mLength;
			result.mRadius = A.mRadius;
			float num = 1f / result.mMagnitude;
			if (result.mDamageType == StatusEffects.Burning)
			{
				result.mDPS = Math.Max(A.mDPS, B.mDPS);
			}
			else
			{
				result.mDPS = A.mMagnitude * num * A.mDPS + B.mMagnitude * num * B.mDPS;
			}
			return result;
		}

		// Token: 0x06002619 RID: 9753 RVA: 0x00113736 File Offset: 0x00111936
		public static bool operator ==(StatusEffect A, StatusEffect B)
		{
			return A.mDamageType == B.mDamageType;
		}

		// Token: 0x0600261A RID: 9754 RVA: 0x00113748 File Offset: 0x00111948
		public static bool operator !=(StatusEffect A, StatusEffect B)
		{
			return A.mDamageType != B.mDamageType;
		}

		// Token: 0x0600261B RID: 9755 RVA: 0x0011375D File Offset: 0x0011195D
		public void Stop()
		{
			this.mMagnitude = 0f;
		}

		// Token: 0x04002947 RID: 10567
		private static readonly double ONEOVERLN2 = 1.0 / Math.Log(2.0);

		// Token: 0x04002948 RID: 10568
		private static readonly int POISON_VOMIT = "statuseffect_poison".GetHashCodeCustom();

		// Token: 0x04002949 RID: 10569
		public static readonly int[] EFFECTS_STATUS = new int[]
		{
			0,
			"statuseffect_wet".GetHashCodeCustom(),
			"statuseffect_frozen".GetHashCodeCustom(),
			0,
			"statuseffect_burning_arcane".GetHashCodeCustom(),
			"statuseffect_healing".GetHashCodeCustom(),
			"statuseffect_wet".GetHashCodeCustom(),
			"drying_steam".GetHashCodeCustom()
		};

		// Token: 0x0400294A RID: 10570
		public static readonly int[] EFFECT_BURNING = new int[]
		{
			"statuseffect_burning".GetHashCodeCustom(),
			"statuseffect_burning_2".GetHashCodeCustom(),
			"statuseffect_burning_3".GetHashCodeCustom()
		};

		// Token: 0x0400294B RID: 10571
		public static readonly int[] EFFECT_COLD = new int[]
		{
			"statuseffect_cold".GetHashCodeCustom(),
			"statuseffect_cold_2".GetHashCodeCustom(),
			"statuseffect_cold_3".GetHashCodeCustom()
		};

		// Token: 0x0400294C RID: 10572
		public static readonly int[] BLEED_EFFECT = new int[]
		{
			"gore_bleed_regular".GetHashCodeCustom(),
			"gore_bleed_green".GetHashCodeCustom(),
			"gore_bleed_black".GetHashCodeCustom(),
			"gore_bleed_wood".GetHashCodeCustom(),
			"gore_bleed_insect".GetHashCodeCustom(),
			"".GetHashCode()
		};

		// Token: 0x0400294D RID: 10573
		private VisualEffect mEmitterA;

		// Token: 0x0400294E RID: 10574
		private VisualEffectReference mVomitEffect;

		// Token: 0x0400294F RID: 10575
		private float mDamageRest;

		// Token: 0x04002950 RID: 10576
		private float mMagnitude;

		// Token: 0x04002951 RID: 10577
		private int mParticleMagntiude;

		// Token: 0x04002952 RID: 10578
		private float mDPS;

		// Token: 0x04002953 RID: 10579
		private float mParticleTimer;

		// Token: 0x04002954 RID: 10580
		private float mRadius;

		// Token: 0x04002955 RID: 10581
		private float mLength;

		// Token: 0x04002956 RID: 10582
		private StatusEffects mDamageType;
	}
}
