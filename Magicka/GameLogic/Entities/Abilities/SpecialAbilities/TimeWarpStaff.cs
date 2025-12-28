using System;
using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using PolygonHead;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x0200003E RID: 62
	internal class TimeWarpStaff : SpecialAbility, IAbilityEffect
	{
		// Token: 0x17000085 RID: 133
		// (get) Token: 0x0600027B RID: 635 RVA: 0x0000FEB8 File Offset: 0x0000E0B8
		public static TimeWarpStaff Instance
		{
			get
			{
				if (TimeWarpStaff.mSingelton == null)
				{
					lock (TimeWarpStaff.mSingeltonLock)
					{
						if (TimeWarpStaff.mSingelton == null)
						{
							TimeWarpStaff.mSingelton = new TimeWarpStaff();
						}
					}
				}
				return TimeWarpStaff.mSingelton;
			}
		}

		// Token: 0x0600027C RID: 636 RVA: 0x0000FF0C File Offset: 0x0000E10C
		public TimeWarpStaff(Animations iAnimation) : base(iAnimation, "#magick_timewarp".GetHashCodeCustom())
		{
		}

		// Token: 0x0600027D RID: 637 RVA: 0x0000FF2A File Offset: 0x0000E12A
		private TimeWarpStaff() : base(Animations.cast_magick_self, "#magick_timewarp".GetHashCodeCustom())
		{
		}

		// Token: 0x0600027E RID: 638 RVA: 0x0000FF4C File Offset: 0x0000E14C
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			if (iOwner == null)
			{
				throw new Exception("TimeWarpStaff can not be cast without a valid owner!");
			}
			if (SpellManager.Instance.IsEffectActive(base.GetType()) || SpellManager.Instance.IsEffectActive(typeof(TimeWarp)))
			{
				return false;
			}
			Vector3 translation = iOwner.CastSource.Translation;
			Vector3 direction = iOwner.Direction;
			VisualEffectReference visualEffectReference;
			EffectManager.Instance.StartEffect(TimeWarpStaff.EFFECT, ref translation, ref direction, out visualEffectReference);
			this.mTTL = 15f;
			this.mOwner = iOwner;
			this.mPlayState = iPlayState;
			this.mTimeMultiplier = 1f;
			this.mSaturation = this.mPlayState.Level.CurrentScene.Saturation;
			this.mTimeMultiplierTarget = 0.5f;
			this.mSaturationTarget = 0.1f;
			SpellManager.Instance.AddSpellEffect(this);
			TimeWarpStaff.sCue = AudioManager.Instance.GetCue(Banks.Spells, TimeWarpStaff.SOUND_HASH);
			TimeWarpStaff.sCue.Play();
			return true;
		}

		// Token: 0x17000086 RID: 134
		// (get) Token: 0x0600027F RID: 639 RVA: 0x0001003E File Offset: 0x0000E23E
		public bool IsDead
		{
			get
			{
				return this.mTTL <= 0f;
			}
		}

		// Token: 0x06000280 RID: 640 RVA: 0x00010050 File Offset: 0x0000E250
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			if (this.mTTL <= this.mFadeTime)
			{
				this.mTimeMultiplierTarget = 1f;
				this.mSaturationTarget = this.mPlayState.Level.CurrentScene.Saturation;
			}
			iDeltaTime /= this.mPlayState.TimeModifier * this.mPlayState.TimeMultiplier;
			this.mTTL -= iDeltaTime;
			this.mTimeMultiplier += (this.mTimeMultiplierTarget - this.mTimeMultiplier) * iDeltaTime;
			this.mPlayState.TimeMultiplier = this.mTimeMultiplier;
			this.mSaturation += (this.mSaturationTarget - this.mSaturation) * iDeltaTime;
			RenderManager.Instance.Saturation = this.mSaturation;
		}

		// Token: 0x06000281 RID: 641 RVA: 0x00010114 File Offset: 0x0000E314
		public void OnRemove()
		{
			if (TimeWarpStaff.sCue != null)
			{
				TimeWarpStaff.sCue.Stop(AudioStopOptions.AsAuthored);
			}
			TimeWarpStaff.sCue = null;
			if (this.mOwner is Character)
			{
				(this.mOwner as Character).TimeWarpModifier = 1f;
			}
			this.mPlayState.TimeMultiplier = 1f;
			if (this.mOwner is Avatar)
			{
				(this.mOwner as Avatar).ResetAfterImages();
			}
			if (this.mPlayState.Level != null)
			{
				RenderManager.Instance.Saturation = this.mPlayState.Level.CurrentScene.Saturation;
				return;
			}
			RenderManager.Instance.Saturation = 1f;
		}

		// Token: 0x040001FA RID: 506
		private const float DURATION_TIME = 15f;

		// Token: 0x040001FB RID: 507
		private const float TIME_TARGET = 0.5f;

		// Token: 0x040001FC RID: 508
		private const float SATURATION_TARGET = 0.1f;

		// Token: 0x040001FD RID: 509
		private static TimeWarpStaff mSingelton;

		// Token: 0x040001FE RID: 510
		private static volatile object mSingeltonLock = new object();

		// Token: 0x040001FF RID: 511
		private float mTTL;

		// Token: 0x04000200 RID: 512
		private float mTimeMultiplierTarget;

		// Token: 0x04000201 RID: 513
		private float mTimeMultiplier;

		// Token: 0x04000202 RID: 514
		private float mSaturationTarget;

		// Token: 0x04000203 RID: 515
		private float mSaturation;

		// Token: 0x04000204 RID: 516
		private float mFadeTime = 1f;

		// Token: 0x04000205 RID: 517
		private static Cue sCue;

		// Token: 0x04000206 RID: 518
		private PlayState mPlayState;

		// Token: 0x04000207 RID: 519
		private ISpellCaster mOwner;

		// Token: 0x04000208 RID: 520
		public static readonly int SOUND_HASH = "magick_timewarp".GetHashCodeCustom();

		// Token: 0x04000209 RID: 521
		private static readonly int EFFECT = "timewarp_effect".GetHashCodeCustom();
	}
}
