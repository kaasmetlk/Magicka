using System;
using System.Collections.Generic;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.CharacterStates;
using Magicka.GameLogic.Entities.Items;
using Magicka.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Magicka.GameLogic.Spells.SpellEffects
{
	// Token: 0x02000643 RID: 1603
	internal class LightningSpell : SpellEffect
	{
		// Token: 0x060030A1 RID: 12449 RVA: 0x0018E6B0 File Offset: 0x0018C8B0
		public static void InitializeCache(int iSize)
		{
			LightningSpell.mCache = new List<LightningSpell>(iSize);
			for (int i = 0; i < iSize; i++)
			{
				LightningSpell.mCache.Add(new LightningSpell());
			}
		}

		// Token: 0x060030A2 RID: 12450 RVA: 0x0018E6E4 File Offset: 0x0018C8E4
		public static SpellEffect GetFromCache()
		{
			LightningSpell lightningSpell;
			try
			{
				lightningSpell = LightningSpell.mCache[LightningSpell.mCache.Count - 1];
				LightningSpell.mCache.Remove(lightningSpell);
				SpellEffect.mPlayState.SpellEffects.Add(lightningSpell);
			}
			catch
			{
				lightningSpell = new LightningSpell();
				SpellEffect.mPlayState.SpellEffects.Add(lightningSpell);
			}
			return lightningSpell;
		}

		// Token: 0x060030A3 RID: 12451 RVA: 0x0018E750 File Offset: 0x0018C950
		public static void ReturnToCache(LightningSpell iEffect)
		{
			foreach (Cue cue in iEffect.mSpellCues)
			{
				if (!cue.IsStopping || !cue.IsStopped)
				{
					cue.Stop(AudioStopOptions.AsAuthored);
				}
			}
			iEffect.mSpellCues.Clear();
			iEffect.mHitList.Clear();
			LightningSpell.mCache.Add(iEffect);
		}

		// Token: 0x060030A4 RID: 12452 RVA: 0x0018E7D4 File Offset: 0x0018C9D4
		public LightningSpell()
		{
			this.mHitList = new HitList(256);
			this.mSpellCues = new List<Cue>(8);
		}

		// Token: 0x060030A5 RID: 12453 RVA: 0x0018E7F8 File Offset: 0x0018C9F8
		public override void CastArea(Spell iSpell, ISpellCaster iOwner, bool iFromStaff)
		{
			base.CastArea(iSpell, iOwner, iFromStaff);
			this.mTimeStamp = iOwner.PlayState.PlayTime;
			this.mFromStaff = false;
			this.mSpell = iSpell;
			this.mAllAround = true;
			this.mTTL = 1f;
			this.mRange = 2f + this.mSpell[Elements.Lightning] / 5f * 4f * 0.5f;
			if (iOwner is Character)
			{
				(iOwner as Character).GetSpellRangeModifier(ref this.mRange);
			}
			this.mLightningsToCast = 32;
			this.mSpell.CalculateDamage(SpellType.Lightning, CastType.Area, out this.mDamages);
			this.mDamages.MultiplyMagnitude(0.0625f);
			this.mTimeBetweenCasts = this.mTTL / (float)this.mLightningsToCast;
			this.mTTL = 0f;
			this.mScale = 0.8f + this.mSpell.TotalMagnitude() / 5f * 0.4f;
			base.PlaySound(SpellType.Lightning, CastType.Area, iOwner);
		}

		// Token: 0x060030A6 RID: 12454 RVA: 0x0018E8FC File Offset: 0x0018CAFC
		public override void CastForce(Spell iSpell, ISpellCaster iOwner, bool iFromStaff)
		{
			base.CastForce(iSpell, iOwner, iFromStaff);
			if (iOwner is Character && !((iOwner as Character).CurrentState is PanicCastState) && this.mFromStaff)
			{
				this.mFromStaff = false;
			}
			this.mTimeStamp = iOwner.PlayState.PlayTime;
			this.mSpell = iSpell;
			this.mTTL = 1f;
			this.mAllAround = false;
			this.mRange = 4f + this.mSpell[Elements.Lightning] / 5f * 8f * 0.5f;
			if (iOwner is Character)
			{
				(iOwner as Character).GetSpellRangeModifier(ref this.mRange);
			}
			this.mLightningsToCast = 8;
			this.mSpell.CalculateDamage(SpellType.Lightning, CastType.Force, out this.mDamages);
			this.mDamages.MultiplyMagnitude(1f / (float)this.mLightningsToCast);
			this.mTimeBetweenCasts = this.mTTL / (float)this.mLightningsToCast;
			this.mTTL = 0f;
			this.mScale = 0.8f + this.mSpell.TotalMagnitude() / 5f * 0.4f;
			base.PlaySound(SpellType.Lightning, CastType.Force, iOwner);
		}

		// Token: 0x060030A7 RID: 12455 RVA: 0x0018EA28 File Offset: 0x0018CC28
		public override void CastSelf(Spell iSpell, ISpellCaster iOwner, bool iFromStaff)
		{
			base.CastSelf(iSpell, iOwner, iFromStaff);
			this.mSpell = iSpell;
			this.mTimeStamp = iOwner.PlayState.PlayTime;
			this.mTTL = 0f;
			Vector3 position = iOwner.Position;
			Vector3 direction = iOwner.Direction;
			VisualEffectReference visualEffectReference;
			EffectManager.Instance.StartEffect(SpellEffect.SelfCastEffectHash[Defines.ElementIndex(iSpell.Element)], ref position, ref direction, out visualEffectReference);
			this.mSpell.CalculateDamage(SpellType.Lightning, CastType.Self, out this.mDamages);
			this.mLightningsToCast = 1;
			iOwner.Damage(this.mDamages, iOwner as Entity, this.mTimeStamp, iOwner.Position);
		}

		// Token: 0x060030A8 RID: 12456 RVA: 0x0018EACB File Offset: 0x0018CCCB
		public override void CastWeapon(Spell iSpell, ISpellCaster iOwner, bool iFromStaff)
		{
			this.CastForce(iSpell, iOwner, iFromStaff);
			this.mCastType = CastType.Weapon;
		}

		// Token: 0x060030A9 RID: 12457 RVA: 0x0018EAE0 File Offset: 0x0018CCE0
		public override bool CastUpdate(float iDeltaTime, ISpellCaster iOwner, out float oTurnSpeed)
		{
			oTurnSpeed = 0.1f;
			this.mTTL -= iDeltaTime;
			if (this.mTTL <= 0f && this.mLightningsToCast > 0)
			{
				if (base.CastType != CastType.Self)
				{
					this.mHitList.Clear();
					this.mHitList.Add(iOwner);
					this.mTTL = this.mTimeBetweenCasts;
					this.mLightningsToCast--;
					Vector3 iDirection;
					if (this.mFromStaff && iOwner is Character)
					{
						if (base.CastType == CastType.Weapon)
						{
							iDirection = iOwner.WeaponSource.Forward;
						}
						else
						{
							iDirection = iOwner.CastSource.Forward;
						}
					}
					else
					{
						iDirection = iOwner.Direction;
					}
					if (iOwner.HasStatus(StatusEffects.Wet) && !iOwner.HasPassiveAbility(Item.PassiveAbilities.WetLightning))
					{
						this.mRange = 2f;
						this.mAllAround = true;
					}
					Vector3 translation;
					if (this.mAllAround)
					{
						Quaternion quaternion;
						Quaternion.CreateFromYawPitchRoll((float)(this.mLightningsToCast % 8) * 3.1415927f / 4f, 0f, 0f, out quaternion);
						Vector3.Transform(ref iDirection, ref quaternion, out iDirection);
						translation = iOwner.CastSource.Translation;
					}
					else if (base.CastType == CastType.Weapon)
					{
						translation = iOwner.WeaponSource.Translation;
					}
					else
					{
						translation = iOwner.CastSource.Translation;
					}
					LightningBolt lightning = LightningBolt.GetLightning();
					DamageCollection5 damageCollection = this.mDamages;
					lightning.Cast(iOwner, translation, iDirection, this.mHitList, this.mSpell.GetColor(), this.mRange, ref damageCollection, new Spell?(this.mSpell), SpellEffect.mPlayState);
				}
			}
			else if (base.Active && this.mLightningsToCast <= 0)
			{
				this.DeInitialize(iOwner);
				return false;
			}
			float num;
			return base.CastUpdate(iDeltaTime, iOwner, out num);
		}

		// Token: 0x060030AA RID: 12458 RVA: 0x0018ECA0 File Offset: 0x0018CEA0
		public override void DeInitialize(ISpellCaster iOwner)
		{
			if (!base.Active)
			{
				return;
			}
			foreach (Cue cue in this.mSpellCues)
			{
				if (!cue.IsStopping || !cue.IsStopped)
				{
					cue.Stop(AudioStopOptions.AsAuthored);
				}
			}
			this.mSpellCues.Clear();
			base.Active = false;
			LightningSpell.ReturnToCache(this);
		}

		// Token: 0x0400349F RID: 13471
		private static List<LightningSpell> mCache;

		// Token: 0x040034A0 RID: 13472
		private bool mAllAround;

		// Token: 0x040034A1 RID: 13473
		private float mTTL;

		// Token: 0x040034A2 RID: 13474
		private float mTimeBetweenCasts;

		// Token: 0x040034A3 RID: 13475
		private int mLightningsToCast;

		// Token: 0x040034A4 RID: 13476
		private float mRange;

		// Token: 0x040034A5 RID: 13477
		private float mScale;

		// Token: 0x040034A6 RID: 13478
		private HitList mHitList;

		// Token: 0x040034A7 RID: 13479
		private DamageCollection5 mDamages;

		// Token: 0x040034A8 RID: 13480
		private new double mTimeStamp;
	}
}
