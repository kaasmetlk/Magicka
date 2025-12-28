using System;
using System.Collections.Generic;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using PolygonHead;

namespace Magicka.GameLogic.Entities
{
	// Token: 0x02000342 RID: 834
	public class TeslaField
	{
		// Token: 0x06001974 RID: 6516 RVA: 0x000ABCA8 File Offset: 0x000A9EA8
		public static void InitializeCache(int iNrOfItems, PlayState iPlayState)
		{
			TeslaField.sCache = new List<TeslaField>(iNrOfItems);
			for (int i = 0; i < iNrOfItems; i++)
			{
				TeslaField.sCache.Add(new TeslaField(iPlayState));
			}
		}

		// Token: 0x06001975 RID: 6517 RVA: 0x000ABCDC File Offset: 0x000A9EDC
		public static TeslaField GetFromCache(PlayState iPlayState)
		{
			if (TeslaField.sCache.Count > 0)
			{
				TeslaField result = TeslaField.sCache[TeslaField.sCache.Count - 1];
				TeslaField.sCache.RemoveAt(TeslaField.sCache.Count - 1);
				return result;
			}
			return new TeslaField(iPlayState);
		}

		// Token: 0x06001976 RID: 6518 RVA: 0x000ABD2B File Offset: 0x000A9F2B
		private TeslaField(PlayState iPlayState)
		{
			this.mPlaystate = iPlayState;
		}

		// Token: 0x06001977 RID: 6519 RVA: 0x000ABD48 File Offset: 0x000A9F48
		public void Initialize(Character iOwner, Spell iSpell)
		{
			this.mItemAbility = false;
			this.mOwner = iOwner;
			this.mRadius = 0f;
			this.mSpell = iSpell;
			this.mSpell.CalculateDamage(SpellType.Shield, CastType.None, out this.mDamages);
			this.mDamages.MultiplyMagnitude(0.25f);
			this.mTimeAlive = 0f;
			this.mLifeTime = 5f;
		}

		// Token: 0x06001978 RID: 6520 RVA: 0x000ABDAE File Offset: 0x000A9FAE
		public void Kill()
		{
			this.mTimeAlive = this.mLifeTime;
		}

		// Token: 0x1700064E RID: 1614
		// (get) Token: 0x06001979 RID: 6521 RVA: 0x000ABDBC File Offset: 0x000A9FBC
		// (set) Token: 0x0600197A RID: 6522 RVA: 0x000ABDC4 File Offset: 0x000A9FC4
		public bool ItemAbility
		{
			get
			{
				return this.mItemAbility;
			}
			set
			{
				this.mItemAbility = value;
			}
		}

		// Token: 0x0600197B RID: 6523 RVA: 0x000ABDD0 File Offset: 0x000A9FD0
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			this.mTimeAlive += iDeltaTime;
			this.mRadius += iDeltaTime * 5f;
			this.mRadius = Math.Min(this.mRadius, 5f);
			IDamageable closestIDamageable = this.mOwner.PlayState.EntityManager.GetClosestIDamageable(this.mOwner, this.mOwner.Position, this.mRadius, true);
			if (closestIDamageable != null && this.mRadius > 0.5f)
			{
				this.mLastZap = this.mTimeAlive;
				Vector3 direction = this.mOwner.Direction;
				LightningBolt lightning = LightningBolt.GetLightning();
				this.mHitList.Clear();
				if (!this.mOwner.HasStatus(StatusEffects.Wet))
				{
					this.mHitList.Add(this.mOwner);
				}
				lightning.Cast(this.mOwner, this.mOwner.Position, direction, this.mHitList, this.mSpell.GetColor(), this.mRadius, ref this.mDamages, new Spell?(this.mSpell), this.mOwner.PlayState);
				this.mRadius = 0f;
			}
			if (this.mTimeAlive > this.mLifeTime)
			{
				this.Deinitialize();
			}
		}

		// Token: 0x0600197C RID: 6524 RVA: 0x000ABF0B File Offset: 0x000AA10B
		public void Deinitialize()
		{
			TeslaField.sCache.Add(this);
		}

		// Token: 0x04001BA8 RID: 7080
		private static List<TeslaField> sCache;

		// Token: 0x04001BA9 RID: 7081
		private static readonly int EFFECT_HASH = "ground_arcane".GetHashCodeCustom();

		// Token: 0x04001BAA RID: 7082
		private static readonly int STATIC_SOUND_HASH = "spell_shield".GetHashCodeCustom();

		// Token: 0x04001BAB RID: 7083
		private static readonly int SPARK_SOUND_HASH = "spell_shield".GetHashCodeCustom();

		// Token: 0x04001BAC RID: 7084
		protected VisualEffectReference mEffect;

		// Token: 0x04001BAD RID: 7085
		protected Matrix mScaleMatrix;

		// Token: 0x04001BAE RID: 7086
		protected Character mOwner;

		// Token: 0x04001BAF RID: 7087
		protected HitList mHitList = new HitList(32);

		// Token: 0x04001BB0 RID: 7088
		protected Cue mCue;

		// Token: 0x04001BB1 RID: 7089
		private float mTimeAlive;

		// Token: 0x04001BB2 RID: 7090
		private float mLifeTime;

		// Token: 0x04001BB3 RID: 7091
		private float mLastZap;

		// Token: 0x04001BB4 RID: 7092
		private float mRadius;

		// Token: 0x04001BB5 RID: 7093
		private Spell mSpell;

		// Token: 0x04001BB6 RID: 7094
		private DamageCollection5 mDamages;

		// Token: 0x04001BB7 RID: 7095
		private PlayState mPlaystate;

		// Token: 0x04001BB8 RID: 7096
		private bool mItemAbility;
	}
}
