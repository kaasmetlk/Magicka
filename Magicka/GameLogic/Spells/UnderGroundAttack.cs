using System;
using System.Collections.Generic;
using JigLibX.Geometry;
using Magicka.Audio;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.GameStates;
using Magicka.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using PolygonHead;

namespace Magicka.GameLogic.Spells
{
	// Token: 0x02000504 RID: 1284
	public class UnderGroundAttack : IAbilityEffect
	{
		// Token: 0x0600261D RID: 9757 RVA: 0x001138C4 File Offset: 0x00111AC4
		public static void InitializeCache(int iNr, PlayState iPlayState)
		{
			UnderGroundAttack.sCache = new List<UnderGroundAttack>(iNr);
			for (int i = 0; i < iNr; i++)
			{
				UnderGroundAttack.sCache.Add(new UnderGroundAttack(iPlayState));
			}
		}

		// Token: 0x0600261E RID: 9758 RVA: 0x001138F8 File Offset: 0x00111AF8
		public static UnderGroundAttack GetFromCache(PlayState iPlayState)
		{
			if (UnderGroundAttack.sCache.Count > 0)
			{
				UnderGroundAttack result = UnderGroundAttack.sCache[UnderGroundAttack.sCache.Count - 1];
				UnderGroundAttack.sCache.RemoveAt(UnderGroundAttack.sCache.Count - 1);
				return result;
			}
			return new UnderGroundAttack(iPlayState);
		}

		// Token: 0x0600261F RID: 9759 RVA: 0x00113948 File Offset: 0x00111B48
		private UnderGroundAttack(PlayState iPlayState)
		{
			this.mPlayState = iPlayState;
			this.mCues = new Cue[11];
			this.mAE = new AudioEmitter();
			this.mAE.Up = Vector3.Up;
		}

		// Token: 0x06002620 RID: 9760 RVA: 0x001139A4 File Offset: 0x00111BA4
		public void Initialize(ref Vector3 iPosition, ref Vector2 iVelocity, ISpellCaster iOwner, double iTimeStamp, float iRange, DamageCollection5 iDamage, bool iPiercing)
		{
			this.mHitList.Clear();
			this.mHitList.Add(iOwner.Handle);
			this.mRange = iRange;
			this.mPosition = iPosition;
			this.mVelocity = iVelocity;
			this.mOwner = iOwner;
			this.mTimeStamp = iTimeStamp;
			this.mDamage = iDamage;
			this.mAE.Velocity = new Vector3(this.mVelocity.X, 0f, this.mVelocity.Y);
			this.mAE.Forward = Vector3.Normalize(this.mAE.Velocity);
			Vector2 vector;
			Vector2.Normalize(ref iVelocity, out vector);
			this.mDirection.X = vector.X;
			this.mDirection.Z = vector.Y;
			Segment iSeg = default(Segment);
			iSeg.Origin = iPosition;
			iSeg.Origin.Y = iSeg.Origin.Y + 1f;
			iSeg.Delta.Y = -4f;
			float num;
			Vector3 vector2;
			if (this.mPlayState.Level.CurrentScene.SegmentIntersect(out num, out this.mPosition, out vector2, iSeg) && vector2.Y >= 0.7f)
			{
				Elements allElements = iDamage.GetAllElements();
				int num2 = 0;
				for (int i = 0; i < 11; i++)
				{
					Elements elements = (Elements)(1 << i);
					if ((elements & allElements) == elements)
					{
						EffectManager.Instance.StartEffect(UnderGroundAttack.EFFECTS[i], ref this.mPosition, ref this.mDirection, out this.mEffect[num2++]);
						this.mCues[i] = AudioManager.Instance.GetCue(Banks.Spells, UnderGroundAttack.SFX[i]);
						this.mCues[i].Apply3D(this.mPlayState.Camera.Listener, this.mAE);
						this.mCues[i].Play();
					}
				}
				SpellManager.Instance.AddSpellEffect(this);
				return;
			}
			UnderGroundAttack.sCache.Add(this);
		}

		// Token: 0x06002621 RID: 9761 RVA: 0x00113BBC File Offset: 0x00111DBC
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			Vector2 vector;
			Vector2.Multiply(ref this.mVelocity, iDeltaTime, out vector);
			Segment iSeg = default(Segment);
			iSeg.Origin.X = this.mPosition.X + vector.X;
			iSeg.Origin.Y = this.mPosition.Y + 1f;
			iSeg.Origin.Z = this.mPosition.Z + vector.Y;
			iSeg.Delta.Y = -2f;
			this.mAE.Position = this.mPosition;
			Vector3 origin = this.mPosition;
			float num;
			Vector3 vector2;
			if (this.mPlayState.Level.CurrentScene.SegmentIntersect(out num, out this.mPosition, out vector2, iSeg) && vector2.Y >= 0.7f)
			{
				Segment iSeg2;
				iSeg2.Origin = origin;
				Vector3.Subtract(ref this.mPosition, ref origin, out iSeg2.Delta);
				List<Shield> shields = this.mPlayState.EntityManager.Shields;
				bool flag = true;
				for (int i = 0; i < shields.Count; i++)
				{
					Vector3 vector3;
					if (shields[i].SegmentIntersect(out vector3, iSeg2, 1f))
					{
						this.mPosition = origin;
						flag = false;
						this.Kill();
						break;
					}
				}
				if (flag)
				{
					iSeg2.Origin.Y = iSeg2.Origin.Y + 1f;
					Vector3 vector4;
					if (this.mPlayState.Level.CurrentScene.SegmentIntersect(out num, out vector4, out vector2, iSeg2))
					{
						this.mPosition = origin;
						flag = false;
						this.Kill();
					}
				}
				if (flag)
				{
					for (int j = 0; j < this.mEffect.Length; j++)
					{
						EffectManager.Instance.UpdatePositionDirection(ref this.mEffect[j], ref this.mPosition, ref this.mDirection);
					}
					this.mRange -= vector.Length();
					List<Entity> entities = this.mPlayState.EntityManager.GetEntities(this.mPosition, 1f, true);
					for (int k = 0; k < entities.Count; k++)
					{
						IDamageable damageable = entities[k] as IDamageable;
						if (damageable != null && !this.mHitList.Contains(damageable.Handle))
						{
							this.mHitList.Add(damageable.Handle);
							damageable.Damage(this.mDamage, this.mOwner as Entity, this.mTimeStamp, this.mPosition);
						}
					}
					this.mPlayState.EntityManager.ReturnEntityList(entities);
					return;
				}
			}
			else
			{
				this.mPosition = origin;
				this.Kill();
			}
		}

		// Token: 0x06002622 RID: 9762 RVA: 0x00113E5F File Offset: 0x0011205F
		public void Kill()
		{
			this.mRange = 0f;
		}

		// Token: 0x170008F6 RID: 2294
		// (get) Token: 0x06002623 RID: 9763 RVA: 0x00113E6C File Offset: 0x0011206C
		public bool IsDead
		{
			get
			{
				return this.mRange <= 0f;
			}
		}

		// Token: 0x06002624 RID: 9764 RVA: 0x00113E80 File Offset: 0x00112080
		public void OnRemove()
		{
			for (int i = 0; i < this.mEffect.Length; i++)
			{
				EffectManager.Instance.Stop(ref this.mEffect[i]);
			}
			for (int j = 0; j < 11; j++)
			{
				if (this.mCues[j] != null)
				{
					if (!this.mCues[j].IsStopped || !this.mCues[j].IsStopping)
					{
						this.mCues[j].Stop(AudioStopOptions.AsAuthored);
					}
					this.mCues[j] = null;
				}
			}
			UnderGroundAttack.sCache.Add(this);
		}

		// Token: 0x04002957 RID: 10583
		public static readonly int[] EFFECTS = new int[]
		{
			"underground_earth".GetHashCodeCustom(),
			"underground_water".GetHashCodeCustom(),
			"underground_cold".GetHashCodeCustom(),
			"underground_fire".GetHashCodeCustom(),
			0,
			"underground_arcane".GetHashCodeCustom(),
			"underground_life".GetHashCodeCustom(),
			0,
			0,
			"underground_steam".GetHashCodeCustom(),
			"underground_steam".GetHashCodeCustom()
		};

		// Token: 0x04002958 RID: 10584
		public static readonly int[] SFX = new int[]
		{
			"spell_earth_ground".GetHashCodeCustom(),
			"spell_water_spray".GetHashCodeCustom(),
			"spell_cold_spray".GetHashCodeCustom(),
			"spell_fire_spray".GetHashCodeCustom(),
			0,
			"spell_arcane_ray_stage2".GetHashCodeCustom(),
			"spell_life_ray_stage2".GetHashCodeCustom(),
			0,
			0,
			"spell_steam_spray".GetHashCodeCustom(),
			"spell_poison_spray".GetHashCodeCustom()
		};

		// Token: 0x04002959 RID: 10585
		private static List<UnderGroundAttack> sCache;

		// Token: 0x0400295A RID: 10586
		private float mRange;

		// Token: 0x0400295B RID: 10587
		private ISpellCaster mOwner;

		// Token: 0x0400295C RID: 10588
		private DamageCollection5 mDamage;

		// Token: 0x0400295D RID: 10589
		private Vector3 mPosition;

		// Token: 0x0400295E RID: 10590
		private Vector3 mDirection;

		// Token: 0x0400295F RID: 10591
		private Vector2 mVelocity;

		// Token: 0x04002960 RID: 10592
		private Cue[] mCues;

		// Token: 0x04002961 RID: 10593
		private AudioEmitter mAE;

		// Token: 0x04002962 RID: 10594
		private PlayState mPlayState;

		// Token: 0x04002963 RID: 10595
		private double mTimeStamp;

		// Token: 0x04002964 RID: 10596
		private VisualEffectReference[] mEffect = new VisualEffectReference[5];

		// Token: 0x04002965 RID: 10597
		private List<ushort> mHitList = new List<ushort>(32);
	}
}
