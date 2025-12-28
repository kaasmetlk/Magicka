using System;
using JigLibX.Collision;
using Magicka.GameLogic.Spells;
using Magicka.Graphics;
using Microsoft.Xna.Framework;
using PolygonHead.ParticleEffects;

namespace Magicka.GameLogic.Entities.Bosses
{
	// Token: 0x02000313 RID: 787
	public struct TentacleStatusEffect : IEquatable<StatusEffect>
	{
		// Token: 0x06001828 RID: 6184 RVA: 0x0009FDFC File Offset: 0x0009DFFC
		public TentacleStatusEffect(StatusEffects iStatus, float iDPS, float iMagnitude, float iLength, float iRadius)
		{
			this = new TentacleStatusEffect(iStatus, iDPS, iMagnitude, iLength, iRadius, 0f);
		}

		// Token: 0x06001829 RID: 6185 RVA: 0x0009FE10 File Offset: 0x0009E010
		public TentacleStatusEffect(StatusEffects iStatus, float iDPS, float iMagnitude, float iLength, float iRadius, float iWetTimer)
		{
			this.mWetTimer = iWetTimer;
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
			int num = 10;
			this.mEmitterA = new VisualEffect[num];
			for (int i = 0; i < this.mEmitterA.Length; i++)
			{
				this.mEmitterA[i] = default(VisualEffect);
			}
			this.mMagnitude = iMagnitude;
			this.mParticleMagntiude = (int)Math.Ceiling((double)this.mMagnitude);
			switch (iStatus)
			{
			case StatusEffects.Wet:
				if (this.mWetTimer > 0f)
				{
					for (int j = 0; j < this.mEmitterA.Length; j++)
					{
						this.mEmitterA[j] = EffectManager.Instance.GetEffect(TentacleStatusEffect.EFFECTS_STATUS[TentacleStatusEffect.StatusIndex(iStatus)]);
						this.mEmitterA[j].Start(ref matrix);
					}
				}
				break;
			case StatusEffects.Burning | StatusEffects.Wet:
				break;
			case StatusEffects.Frozen:
				for (int k = 0; k < this.mEmitterA.Length; k++)
				{
					this.mEmitterA[k] = EffectManager.Instance.GetEffect(TentacleStatusEffect.EFFECTS_STATUS[TentacleStatusEffect.StatusIndex(iStatus)]);
					this.mEmitterA[k].Start(ref matrix);
				}
				break;
			default:
				if (iStatus == StatusEffects.Steamed)
				{
					for (int l = 0; l < this.mEmitterA.Length; l++)
					{
						this.mEmitterA[l] = EffectManager.Instance.GetEffect(Defines.STATUS_DRYING_EFFECT_HASH);
						this.mMagnitude = 1f;
						this.mEmitterA[l].Start(ref matrix);
					}
				}
				break;
			}
			this.mParticleTimer = 0f;
			this.mDamageRest = 0f;
		}

		// Token: 0x0600182A RID: 6186 RVA: 0x000A0035 File Offset: 0x0009E235
		public static int StatusIndex(StatusEffects iStatus)
		{
			return (int)(Math.Log((double)iStatus) * TentacleStatusEffect.ONEOVERLN2 + 0.5);
		}

		// Token: 0x0600182B RID: 6187 RVA: 0x000A0050 File Offset: 0x0009E250
		public static StatusEffects StatusFromIndex(int iIndex)
		{
			int num = (int)(Math.Pow(2.0, (double)iIndex) + 0.5);
			return (StatusEffects)num;
		}

		// Token: 0x0600182C RID: 6188 RVA: 0x000A0080 File Offset: 0x0009E280
		public void Update(float iDeltaTime, IStatusEffected iOwner, CollisionSkin iSkin)
		{
			if (this.mMagnitude <= 0f)
			{
				return;
			}
			float num = 1f;
			this.mParticleTimer -= iDeltaTime;
			Matrix matrix = default(Matrix);
			StatusEffects statusEffects = this.mDamageType;
			if (statusEffects <= StatusEffects.Cold)
			{
				switch (statusEffects)
				{
				case StatusEffects.Wet:
					if (this.mWetTimer < 0f)
					{
						num = 0f;
					}
					else
					{
						num = 1f;
						this.mWetTimer -= iDeltaTime;
					}
					this.mMagnitude = 1f;
					break;
				case StatusEffects.Burning | StatusEffects.Wet:
					break;
				case StatusEffects.Frozen:
					this.mMagnitude = Math.Min(this.mMagnitude, 3f);
					this.mMagnitude -= iDeltaTime * 0.25f;
					break;
				default:
					if (statusEffects == StatusEffects.Cold)
					{
						num = (float)Math.Sqrt((double)(this.mMagnitude * 0.2f)) * 0.2f;
						Character character = iOwner as Character;
						if (character != null)
						{
							float slowdown = this.GetSlowdown();
							character.CharacterBody.SpeedMultiplier = Math.Min(character.CharacterBody.SpeedMultiplier, slowdown);
						}
						this.mMagnitude -= iDeltaTime * 0.1f;
						this.mMagnitude = Math.Min(this.mMagnitude, 3f);
						int num2 = (int)Math.Ceiling((double)this.mMagnitude);
						if (num2 > 0 && (this.mParticleMagntiude != num2 || !this.mEmitterA[0].IsActive))
						{
							this.mParticleMagntiude = num2;
							matrix = default(Matrix);
							matrix.M11 = this.mRadius;
							matrix.M22 = 1f + this.mRadius;
							matrix.M33 = this.mRadius;
							matrix.M44 = this.mLength;
							matrix.M41 = float.MaxValue;
							matrix.M42 = float.MaxValue;
							matrix.M43 = float.MaxValue;
							int num3 = Math.Min(Math.Max(this.mParticleMagntiude - 1, 0), TentacleStatusEffect.EFFECT_COLD.Length - 1);
							for (int i = 0; i < this.mEmitterA.Length; i++)
							{
								this.mEmitterA[i].Stop();
								this.mEmitterA[i] = EffectManager.Instance.GetEffect(TentacleStatusEffect.EFFECT_COLD[num3]);
								this.mEmitterA[i].Start(ref matrix);
							}
						}
					}
					break;
				}
			}
			else if (statusEffects != StatusEffects.Steamed)
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
			int num4 = (int)this.mDamageRest;
			if (num4 != 0)
			{
				this.mDamageRest -= (float)num4;
				Elements iElement = Elements.Earth;
				iOwner.Damage((float)num4, iElement);
			}
			float num5 = iDeltaTime * num;
			if (num5 > 0f)
			{
				for (int j = 0; j < this.mEmitterA.Length; j++)
				{
					Vector3 translation = iSkin.GetPrimitiveNewWorld(j).TransformMatrix.Translation;
					this.mEmitterA[j].Transform.M41 = translation.X;
					this.mEmitterA[j].Transform.M42 = translation.Y;
					this.mEmitterA[j].Transform.M43 = translation.Z;
					this.mEmitterA[j].Update(num5);
				}
			}
		}

		// Token: 0x0600182D RID: 6189 RVA: 0x000A0418 File Offset: 0x0009E618
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

		// Token: 0x17000614 RID: 1556
		// (get) Token: 0x0600182E RID: 6190 RVA: 0x000A0466 File Offset: 0x0009E666
		public StatusEffects DamageType
		{
			get
			{
				return this.mDamageType;
			}
		}

		// Token: 0x17000615 RID: 1557
		// (get) Token: 0x0600182F RID: 6191 RVA: 0x000A046E File Offset: 0x0009E66E
		// (set) Token: 0x06001830 RID: 6192 RVA: 0x000A0476 File Offset: 0x0009E676
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

		// Token: 0x17000616 RID: 1558
		// (get) Token: 0x06001831 RID: 6193 RVA: 0x000A047F File Offset: 0x0009E67F
		// (set) Token: 0x06001832 RID: 6194 RVA: 0x000A0487 File Offset: 0x0009E687
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

		// Token: 0x17000617 RID: 1559
		// (get) Token: 0x06001833 RID: 6195 RVA: 0x000A0490 File Offset: 0x0009E690
		public bool Dead
		{
			get
			{
				return this.mMagnitude <= 0f;
			}
		}

		// Token: 0x06001834 RID: 6196 RVA: 0x000A04A2 File Offset: 0x0009E6A2
		public bool Equals(StatusEffect obj)
		{
			return obj.DamageType == this.mDamageType;
		}

		// Token: 0x06001835 RID: 6197 RVA: 0x000A04B3 File Offset: 0x0009E6B3
		public override bool Equals(object obj)
		{
			if (obj is StatusEffect)
			{
				return this.Equals((StatusEffect)obj);
			}
			return base.Equals(obj);
		}

		// Token: 0x06001836 RID: 6198 RVA: 0x000A04DC File Offset: 0x0009E6DC
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

		// Token: 0x06001837 RID: 6199 RVA: 0x000A0527 File Offset: 0x0009E727
		public override int GetHashCode()
		{
			return this.mDamageType.GetHashCode();
		}

		// Token: 0x06001838 RID: 6200 RVA: 0x000A053C File Offset: 0x0009E73C
		public static TentacleStatusEffect operator +(TentacleStatusEffect A, TentacleStatusEffect B)
		{
			if (A.Dead | A.mDamageType == StatusEffects.None)
			{
				return B;
			}
			if (B.Dead | B.DamageType == StatusEffects.None)
			{
				return A;
			}
			if (A.mDamageType != B.DamageType)
			{
				throw new Exception("Cannot add status effects. Both effects must be of the same type. " + A.ToString() + " and " + B.ToString());
			}
			TentacleStatusEffect result = default(TentacleStatusEffect);
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
			result.mWetTimer = Math.Max(A.mWetTimer, B.mWetTimer);
			float num = 1f / result.mMagnitude;
			result.mDPS = A.mMagnitude * num * A.mDPS + B.Magnitude * num * B.DPS;
			return result;
		}

		// Token: 0x06001839 RID: 6201 RVA: 0x000A06CE File Offset: 0x0009E8CE
		public static bool operator ==(TentacleStatusEffect A, StatusEffect B)
		{
			return A.mDamageType == B.DamageType;
		}

		// Token: 0x0600183A RID: 6202 RVA: 0x000A06E0 File Offset: 0x0009E8E0
		public static bool operator !=(TentacleStatusEffect A, StatusEffect B)
		{
			return A.mDamageType != B.DamageType;
		}

		// Token: 0x0600183B RID: 6203 RVA: 0x000A06F5 File Offset: 0x0009E8F5
		public void Stop()
		{
			this.mMagnitude = 0f;
		}

		// Token: 0x040019EF RID: 6639
		private static readonly double ONEOVERLN2 = 1.0 / Math.Log(2.0);

		// Token: 0x040019F0 RID: 6640
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

		// Token: 0x040019F1 RID: 6641
		public static readonly int[] EFFECT_COLD = new int[]
		{
			"statuseffect_cold".GetHashCodeCustom(),
			"statuseffect_cold_2".GetHashCodeCustom(),
			"statuseffect_cold_3".GetHashCodeCustom()
		};

		// Token: 0x040019F2 RID: 6642
		public static readonly int[] BLEED_EFFECT = new int[]
		{
			"gore_bleed_regular".GetHashCodeCustom(),
			"gore_bleed_green".GetHashCodeCustom(),
			"gore_bleed_black".GetHashCodeCustom(),
			"gore_bleed_wood".GetHashCodeCustom(),
			"gore_bleed_insect".GetHashCodeCustom(),
			"".GetHashCode()
		};

		// Token: 0x040019F3 RID: 6643
		private VisualEffect[] mEmitterA;

		// Token: 0x040019F4 RID: 6644
		private float mDamageRest;

		// Token: 0x040019F5 RID: 6645
		private float mMagnitude;

		// Token: 0x040019F6 RID: 6646
		private int mParticleMagntiude;

		// Token: 0x040019F7 RID: 6647
		private float mDPS;

		// Token: 0x040019F8 RID: 6648
		private float mParticleTimer;

		// Token: 0x040019F9 RID: 6649
		private float mRadius;

		// Token: 0x040019FA RID: 6650
		private float mLength;

		// Token: 0x040019FB RID: 6651
		private float mWetTimer;

		// Token: 0x040019FC RID: 6652
		private StatusEffects mDamageType;
	}
}
