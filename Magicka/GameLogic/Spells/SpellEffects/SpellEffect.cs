using System;
using System.Collections.Generic;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.GameStates;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace Magicka.GameLogic.Spells.SpellEffects
{
	// Token: 0x020001B0 RID: 432
	public abstract class SpellEffect
	{
		// Token: 0x06000D05 RID: 3333 RVA: 0x0004BF58 File Offset: 0x0004A158
		public static void IntializeCaches(PlayState iState, ContentManager iContent)
		{
			SpellEffect.mPlayState = iState;
			PushSpell.IntializeCache(16);
			SpraySpell.IntializeCache(16);
			ProjectileSpell.InitializeCache(16);
			RailGunSpell.InitializeCache(16);
			LightningSpell.InitializeCache(16);
			ShieldSpell.InitializeCache(16);
		}

		// Token: 0x06000D06 RID: 3334 RVA: 0x0004BF8A File Offset: 0x0004A18A
		public virtual void CastArea(Spell iSpell, ISpellCaster iOwner, bool iFromStaff)
		{
			this.Active = true;
			this.mMinTTL = 0.5f;
			this.mMaxTTL = 1.5f;
			this.mFromStaff = iFromStaff;
			this.mCastType = CastType.Area;
			this.mTimeStamp = iOwner.PlayState.PlayTime;
		}

		// Token: 0x06000D07 RID: 3335 RVA: 0x0004BFC8 File Offset: 0x0004A1C8
		public virtual void CastForce(Spell iSpell, ISpellCaster iOwner, bool iFromStaff)
		{
			this.Active = true;
			this.mMinTTL = 0.5f;
			this.mMaxTTL = 1.5f;
			this.mFromStaff = iFromStaff;
			this.mCastType = CastType.Force;
			this.mTimeStamp = iOwner.PlayState.PlayTime;
		}

		// Token: 0x06000D08 RID: 3336 RVA: 0x0004C006 File Offset: 0x0004A206
		public virtual void CastSelf(Spell iSpell, ISpellCaster iOwner, bool iFromStaff)
		{
			this.Active = true;
			this.mMinTTL = 0.2f;
			this.mMaxTTL = 0.2f;
			this.mFromStaff = iFromStaff;
			this.mCastType = CastType.Self;
			this.mTimeStamp = iOwner.PlayState.PlayTime;
		}

		// Token: 0x06000D09 RID: 3337 RVA: 0x0004C044 File Offset: 0x0004A244
		public virtual void CastWeapon(Spell iSpell, ISpellCaster iOwner, bool iFromStaff)
		{
			this.Active = true;
			this.mMinTTL = 0.5f;
			this.mMaxTTL = 1.5f;
			this.mFromStaff = iFromStaff;
			this.mCastType = CastType.Weapon;
			this.mTimeStamp = iOwner.PlayState.PlayTime;
		}

		// Token: 0x06000D0A RID: 3338 RVA: 0x0004C084 File Offset: 0x0004A284
		public virtual bool CastUpdate(float iDeltaTime, ISpellCaster iOwner, out float oTurnSpeed)
		{
			oTurnSpeed = 0.75f;
			this.mMinTTL -= iDeltaTime;
			this.mMaxTTL -= iDeltaTime;
			if (this.Active && this.mMinTTL <= 0f && iOwner is Avatar && (iOwner as Avatar).CastButton(CastType.None) && !(this is ProjectileSpell))
			{
				this.DeInitialize(iOwner);
				return false;
			}
			if (!this.Active || iOwner.Dead || (iOwner is Avatar && (iOwner as Avatar).CastType == CastType.None && !(this is ProjectileSpell)))
			{
				this.DeInitialize(iOwner);
				return false;
			}
			return true;
		}

		// Token: 0x06000D0B RID: 3339 RVA: 0x0004C127 File Offset: 0x0004A327
		public void Stop(ISpellCaster iOwner)
		{
			this.DeInitialize(iOwner);
			this.Active = false;
		}

		// Token: 0x06000D0C RID: 3340
		public abstract void DeInitialize(ISpellCaster iOwner);

		// Token: 0x1700031F RID: 799
		// (get) Token: 0x06000D0D RID: 3341 RVA: 0x0004C137 File Offset: 0x0004A337
		// (set) Token: 0x06000D0E RID: 3342 RVA: 0x0004C13F File Offset: 0x0004A33F
		public bool Active { get; protected set; }

		// Token: 0x17000320 RID: 800
		// (get) Token: 0x06000D0F RID: 3343 RVA: 0x0004C148 File Offset: 0x0004A348
		public CastType CastType
		{
			get
			{
				return this.mCastType;
			}
		}

		// Token: 0x17000321 RID: 801
		// (get) Token: 0x06000D10 RID: 3344 RVA: 0x0004C150 File Offset: 0x0004A350
		public Spell Spell
		{
			get
			{
				return this.mSpell;
			}
		}

		// Token: 0x06000D11 RID: 3345 RVA: 0x0004C158 File Offset: 0x0004A358
		internal virtual void AnimationEnd(ISpellCaster iOwner)
		{
		}

		// Token: 0x17000322 RID: 802
		// (get) Token: 0x06000D12 RID: 3346 RVA: 0x0004C15A File Offset: 0x0004A35A
		public double TimeStamp
		{
			get
			{
				return this.mTimeStamp;
			}
		}

		// Token: 0x06000D13 RID: 3347 RVA: 0x0004C164 File Offset: 0x0004A364
		public void PlaySound(SpellType iSpellType, CastType iCastType, ISpellCaster iOwner)
		{
			if (this.mSpell.TotalMagnitude() <= 0f)
			{
				throw new Exception("Total magnitude = 0, ERROR!");
			}
			if (this.mSpellCues.Count > 0)
			{
				foreach (Cue cue in this.mSpellCues)
				{
					cue.Stop(AudioStopOptions.AsAuthored);
				}
			}
			this.mSpellCues.Clear();
			AudioListener listener = iOwner.PlayState.Camera.Listener;
			AudioEmitter audioEmitter = iOwner.AudioEmitter;
			foreach (Cue cue2 in this.mSpell.PlaySound(iSpellType, iCastType))
			{
				if (cue2 != null)
				{
					this.mSpellCues.Add(cue2);
					if (cue2 != null && (cue2.IsPrepared || cue2.IsPreparing))
					{
						cue2.Apply3D(listener, audioEmitter);
						cue2.Play();
					}
				}
			}
		}

		// Token: 0x04000BC9 RID: 3017
		public static readonly int[] SelfCastEffectHash = new int[]
		{
			"self_earth".GetHashCodeCustom(),
			"self_water".GetHashCodeCustom(),
			"self_cold".GetHashCodeCustom(),
			"self_fire".GetHashCodeCustom(),
			"self_lightning".GetHashCodeCustom(),
			"self_arcane".GetHashCodeCustom(),
			"self_life".GetHashCodeCustom(),
			"self_fire".GetHashCodeCustom(),
			"self_ice".GetHashCodeCustom(),
			"self_steam".GetHashCodeCustom(),
			"self_steam".GetHashCodeCustom()
		};

		// Token: 0x04000BCA RID: 3018
		protected static readonly Random RANDOM = new Random();

		// Token: 0x04000BCB RID: 3019
		protected static PlayState mPlayState;

		// Token: 0x04000BCC RID: 3020
		protected float mMaxTTL;

		// Token: 0x04000BCD RID: 3021
		protected float mMinTTL;

		// Token: 0x04000BCE RID: 3022
		protected bool mFromStaff;

		// Token: 0x04000BCF RID: 3023
		protected List<Cue> mSpellCues;

		// Token: 0x04000BD0 RID: 3024
		protected Spell mSpell;

		// Token: 0x04000BD1 RID: 3025
		protected CastType mCastType;

		// Token: 0x04000BD2 RID: 3026
		protected double mTimeStamp;
	}
}
