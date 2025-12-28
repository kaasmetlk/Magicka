using System;
using System.Collections.Generic;
using Magicka.Audio;
using Magicka.GameLogic.Entities;
using Magicka.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Magicka.GameLogic.Spells.SpellEffects
{
	// Token: 0x020001B1 RID: 433
	public class RailGunSpell : SpellEffect
	{
		// Token: 0x06000D16 RID: 3350 RVA: 0x0004C338 File Offset: 0x0004A538
		public static void InitializeCache(int iSize)
		{
			RailGunSpell.mCache = new List<RailGunSpell>(iSize);
			for (int i = 0; i < iSize; i++)
			{
				RailGunSpell.mCache.Add(new RailGunSpell());
			}
		}

		// Token: 0x06000D17 RID: 3351 RVA: 0x0004C36C File Offset: 0x0004A56C
		public static SpellEffect GetFromCache()
		{
			if (RailGunSpell.mCache.Count <= 0)
			{
				return null;
			}
			RailGunSpell railGunSpell = RailGunSpell.mCache[RailGunSpell.mCache.Count - 1];
			RailGunSpell.mCache.Remove(railGunSpell);
			SpellEffect.mPlayState.SpellEffects.Add(railGunSpell);
			return railGunSpell;
		}

		// Token: 0x06000D18 RID: 3352 RVA: 0x0004C3BC File Offset: 0x0004A5BC
		public static void ReturnToCache(RailGunSpell iEffect)
		{
			SpellEffect.mPlayState.SpellEffects.Remove(iEffect);
			RailGunSpell.mCache.Add(iEffect);
		}

		// Token: 0x06000D19 RID: 3353 RVA: 0x0004C3DC File Offset: 0x0004A5DC
		public override void CastArea(Spell iSpell, ISpellCaster iOwner, bool iFromStaff)
		{
			base.CastArea(iSpell, iOwner, iFromStaff);
			this.mTimeStamp = iOwner.PlayState.PlayTime;
			this.mTTL = 0f;
			DamageCollection5 iDamage;
			iSpell.CalculateDamage(SpellType.Beam, CastType.Area, out iDamage);
			SpellSoundVariables iVariables = default(SpellSoundVariables);
			if (iSpell[Elements.Arcane] > 0f)
			{
				iVariables.mMagnitude = iSpell[Elements.Arcane];
				AudioManager.Instance.PlayCue<SpellSoundVariables>(Banks.Spells, "spell_arcane_area".GetHashCodeCustom(), iVariables);
			}
			else
			{
				iVariables.mMagnitude = iSpell[Elements.Life];
				AudioManager.Instance.PlayCue<SpellSoundVariables>(Banks.Spells, "spell_life_area".GetHashCodeCustom(), iVariables);
			}
			Blast.FullBlast(iOwner.PlayState, iOwner as Entity, this.mTimeStamp, iOwner as Entity, iSpell.BlastSize() * 10f, iOwner.Position, iDamage);
		}

		// Token: 0x06000D1A RID: 3354 RVA: 0x0004C4B8 File Offset: 0x0004A6B8
		public override void CastForce(Spell iSpell, ISpellCaster iOwner, bool iFromStaff)
		{
			base.CastForce(iSpell, iOwner, iFromStaff);
			this.mTimeStamp = iOwner.PlayState.PlayTime;
			this.mTTL = 1f + 2f * (iSpell.ArcaneMagnitude + iSpell.LifeMagnitude);
			if (iOwner is Character)
			{
				(iOwner as Character).GetSpellTTLModifier(ref this.mTTL);
			}
			this.mCaster = iOwner;
			this.mSpell = iSpell;
			this.mFromStaff = iFromStaff;
			DamageCollection5 damageCollection;
			this.mSpell.CalculateDamage(SpellType.Beam, CastType.Force, out damageCollection);
			this.mRailGun = Railgun.GetFromCache();
			this.mRailGun.Initialize(iOwner, iOwner.Position, iOwner.Direction, iSpell.GetColor(), ref damageCollection, ref iSpell);
		}

		// Token: 0x06000D1B RID: 3355 RVA: 0x0004C56C File Offset: 0x0004A76C
		public override void CastSelf(Spell iSpell, ISpellCaster iOwner, bool iFromStaff)
		{
			base.CastSelf(iSpell, iOwner, iFromStaff);
			this.mTimeStamp = iOwner.PlayState.PlayTime;
			this.mTTL = 1f;
			this.mCaster = iOwner;
			this.mSpell = iSpell;
			this.mSpell.CalculateDamage(SpellType.Beam, CastType.Self, out this.mDamage);
			SpellEffect.mPlayState.SpellEffects.Add(this);
			Vector3 position = iOwner.Position;
			Vector3 direction = iOwner.Direction;
			for (int i = 0; i < 11; i++)
			{
				Elements elements = Defines.ElementFromIndex(i);
				if ((elements & iSpell.Element) == elements)
				{
					EffectManager.Instance.StartEffect(SpellEffect.SelfCastEffectHash[Defines.ElementIndex(elements)], ref position, ref direction, out this.mSelfCastEffectReference[i]);
				}
			}
			foreach (Cue cue in this.mSpell.PlaySound(SpellType.Beam, CastType.Self))
			{
				if (cue != null)
				{
					cue.Apply3D(SpellEffect.mPlayState.Camera.Listener, iOwner.AudioEmitter);
					cue.Play();
				}
			}
		}

		// Token: 0x06000D1C RID: 3356 RVA: 0x0004C694 File Offset: 0x0004A894
		public override void CastWeapon(Spell iSpell, ISpellCaster iOwner, bool iFromStaff)
		{
			base.CastWeapon(iSpell, iOwner, iFromStaff);
			this.mTimeStamp = iOwner.PlayState.PlayTime;
			iSpell.CalculateDamage(SpellType.Beam, this.mCastType, out this.mDamage);
			this.mSpell = iSpell;
			foreach (Cue cue in this.mSpell.PlaySound(SpellType.Beam, CastType.Weapon))
			{
				cue.Apply3D(SpellEffect.mPlayState.Camera.Listener, iOwner.AudioEmitter);
				cue.Play();
			}
			this.mBlade = ArcaneBlade.GetInstance();
			float num = iSpell.TotalMagnitude() * 0.5f + 1.5f;
			this.mBlade.Initialize(SpellEffect.mPlayState, (iOwner as Character).Equipment[0].Item, iSpell.Element, num);
			Vector3 position = iOwner.Position;
			Vector3 direction = iOwner.Direction;
			EntityManager entityManager = iOwner.PlayState.EntityManager;
			List<Entity> entities = entityManager.GetEntities(position, num, true);
			for (int i = 0; i < entities.Count; i++)
			{
				IDamageable damageable = entities[i] as IDamageable;
				Vector3 iAttackPosition;
				if (damageable != null && damageable.ArcIntersect(out iAttackPosition, position, direction, num, 1.4137167f, 2f))
				{
					damageable.Damage(this.mDamage, iOwner as Entity, this.mTimeStamp, iAttackPosition);
				}
			}
			entityManager.ReturnEntityList(entities);
		}

		// Token: 0x06000D1D RID: 3357 RVA: 0x0004C814 File Offset: 0x0004AA14
		public override bool CastUpdate(float iDeltaTime, ISpellCaster iOwner, out float oTurnSpeed)
		{
			oTurnSpeed = 0.0325f;
			if (this.mCastType == CastType.Self)
			{
				if (!this.mHasCast)
				{
					this.mCaster.Damage(this.mDamage, this.mCaster as Entity, this.mTimeStamp, this.mCaster.Position);
					this.mHasCast = true;
				}
				Vector3 position = iOwner.Position;
				Vector3 direction = iOwner.Direction;
				for (int i = 0; i < this.mSelfCastEffectReference.Length; i++)
				{
					EffectManager.Instance.UpdatePositionDirection(ref this.mSelfCastEffectReference[i], ref position, ref direction);
				}
				if (iOwner.Dead | (iOwner is Avatar && (iOwner as Avatar).CastType == CastType.None))
				{
					this.mTTL = 0f;
				}
			}
			this.mTTL -= iDeltaTime;
			if (this.mRailGun != null)
			{
				this.mRailGun.Position = iOwner.CastSource.Translation;
				this.mRailGun.Direction = iOwner.Direction;
			}
			if (this.mRailGun != null && this.mRailGun.IsDead)
			{
				this.DeInitialize(iOwner);
				return false;
			}
			if (iOwner.CastType != CastType.Weapon && (this.mTTL <= 0f | iOwner.Dead | iOwner.CastType == CastType.None))
			{
				this.DeInitialize(iOwner);
				return false;
			}
			float num;
			return base.CastUpdate(iDeltaTime, iOwner, out num);
		}

		// Token: 0x06000D1E RID: 3358 RVA: 0x0004C978 File Offset: 0x0004AB78
		private RailGunSpell()
		{
		}

		// Token: 0x06000D1F RID: 3359 RVA: 0x0004C990 File Offset: 0x0004AB90
		public override void DeInitialize(ISpellCaster iOwner)
		{
			if (!base.Active)
			{
				return;
			}
			this.mHasCast = false;
			if (this.mRailGun != null)
			{
				this.mRailGun.Kill();
			}
			this.mRailGun = null;
			if (this.mBlade != null)
			{
				this.mBlade.Kill();
				this.mBlade = null;
			}
			for (int i = 0; i < this.mSelfCastEffectReference.Length; i++)
			{
				EffectManager.Instance.Stop(ref this.mSelfCastEffectReference[i]);
			}
			SpellEffect.mPlayState.SpellEffects.Remove(this);
			RailGunSpell.ReturnToCache(this);
			base.Active = false;
		}

		// Token: 0x04000BD4 RID: 3028
		private static List<RailGunSpell> mCache;

		// Token: 0x04000BD5 RID: 3029
		private ISpellCaster mCaster;

		// Token: 0x04000BD6 RID: 3030
		private static int WeaponEffectHash = "weapon_arcane4".GetHashCodeCustom();

		// Token: 0x04000BD7 RID: 3031
		private static int LifeAreaSoundEffect = "spell_life_area_ray_stage1".GetHashCodeCustom();

		// Token: 0x04000BD8 RID: 3032
		private static int ArcaneAreaSoundEffect = "spell_arcane_area_ray_stage1".GetHashCodeCustom();

		// Token: 0x04000BD9 RID: 3033
		private DamageCollection5 mDamage;

		// Token: 0x04000BDA RID: 3034
		private bool mHasCast;

		// Token: 0x04000BDB RID: 3035
		private Railgun mRailGun;

		// Token: 0x04000BDC RID: 3036
		private ArcaneBlade mBlade;

		// Token: 0x04000BDD RID: 3037
		private float mTTL;

		// Token: 0x04000BDE RID: 3038
		private VisualEffectReference[] mSelfCastEffectReference = new VisualEffectReference[11];

		// Token: 0x04000BDF RID: 3039
		private new double mTimeStamp;
	}
}
