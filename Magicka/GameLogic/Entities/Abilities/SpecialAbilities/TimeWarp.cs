using System;
using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using PolygonHead;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x0200003C RID: 60
	internal class TimeWarp : SpecialAbility, IAbilityEffect
	{
		// Token: 0x17000080 RID: 128
		// (get) Token: 0x06000267 RID: 615 RVA: 0x0000F3E0 File Offset: 0x0000D5E0
		public static TimeWarp Instance
		{
			get
			{
				if (TimeWarp.mSingelton == null)
				{
					lock (TimeWarp.mSingeltonLock)
					{
						if (TimeWarp.mSingelton == null)
						{
							TimeWarp.mSingelton = new TimeWarp();
						}
					}
				}
				return TimeWarp.mSingelton;
			}
		}

		// Token: 0x17000081 RID: 129
		// (get) Token: 0x06000268 RID: 616 RVA: 0x0000F434 File Offset: 0x0000D634
		public ISpellCaster Owner
		{
			get
			{
				return this.mOwner;
			}
		}

		// Token: 0x06000269 RID: 617 RVA: 0x0000F43C File Offset: 0x0000D63C
		public TimeWarp(Animations iAnimation) : base(iAnimation, "#magick_timewarp".GetHashCodeCustom())
		{
		}

		// Token: 0x0600026A RID: 618 RVA: 0x0000F45A File Offset: 0x0000D65A
		private TimeWarp() : base(Animations.cast_magick_self, "#magick_timewarp".GetHashCodeCustom())
		{
		}

		// Token: 0x0600026B RID: 619 RVA: 0x0000F47C File Offset: 0x0000D67C
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			if (SpellManager.Instance.IsEffectActive(base.GetType()) || SpellManager.Instance.IsEffectActive(typeof(TimeWarpStaff)))
			{
				return false;
			}
			if (iOwner == null)
			{
				throw new Exception("TimeWarp can not be cast without a valid owner!");
			}
			this.mTTL = 15f;
			this.mOwner = iOwner;
			this.mPlayState = iPlayState;
			this.mTimeMultiplier = 1f;
			this.mSaturation = this.mPlayState.Level.CurrentScene.Saturation;
			this.mTimeMultiplierTarget = 0.5f;
			this.mSaturationTarget = 0.1f;
			SpellManager.Instance.AddSpellEffect(this);
			TimeWarp.sCue = AudioManager.Instance.GetCue(Banks.Spells, TimeWarp.SOUND_HASH);
			TimeWarp.sCue.Play();
			return true;
		}

		// Token: 0x0600026C RID: 620 RVA: 0x0000F544 File Offset: 0x0000D744
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			if (SpellManager.Instance.IsEffectActive(base.GetType()) || SpellManager.Instance.IsEffectActive(typeof(TimeWarpStaff)))
			{
				return false;
			}
			this.mTTL = 15f;
			this.mOwner = null;
			this.mPlayState = iPlayState;
			this.mTimeMultiplier = 1f;
			this.mSaturation = this.mPlayState.Level.CurrentScene.Saturation;
			this.mTimeMultiplierTarget = 0.5f;
			this.mSaturationTarget = 0.1f;
			SpellManager.Instance.AddSpellEffect(this);
			TimeWarp.sCue = AudioManager.Instance.GetCue(Banks.Spells, TimeWarp.SOUND_HASH);
			TimeWarp.sCue.Play();
			return true;
		}

		// Token: 0x17000082 RID: 130
		// (get) Token: 0x0600026D RID: 621 RVA: 0x0000F5FC File Offset: 0x0000D7FC
		public bool IsDead
		{
			get
			{
				return this.mTTL <= 0f;
			}
		}

		// Token: 0x0600026E RID: 622 RVA: 0x0000F610 File Offset: 0x0000D810
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

		// Token: 0x0600026F RID: 623 RVA: 0x0000F6D4 File Offset: 0x0000D8D4
		public void OnRemove()
		{
			if (TimeWarp.sCue != null)
			{
				TimeWarp.sCue.Stop(AudioStopOptions.AsAuthored);
			}
			TimeWarp.sCue = null;
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

		// Token: 0x040001D8 RID: 472
		private const float DURATION_TIME = 15f;

		// Token: 0x040001D9 RID: 473
		private const float TIME_TARGET = 0.5f;

		// Token: 0x040001DA RID: 474
		private const float SATURATION_TARGET = 0.1f;

		// Token: 0x040001DB RID: 475
		private static TimeWarp mSingelton;

		// Token: 0x040001DC RID: 476
		private static volatile object mSingeltonLock = new object();

		// Token: 0x040001DD RID: 477
		private float mTTL;

		// Token: 0x040001DE RID: 478
		private float mTimeMultiplierTarget;

		// Token: 0x040001DF RID: 479
		private float mTimeMultiplier;

		// Token: 0x040001E0 RID: 480
		private float mSaturationTarget;

		// Token: 0x040001E1 RID: 481
		private float mSaturation;

		// Token: 0x040001E2 RID: 482
		private float mFadeTime = 1f;

		// Token: 0x040001E3 RID: 483
		private static Cue sCue;

		// Token: 0x040001E4 RID: 484
		private PlayState mPlayState;

		// Token: 0x040001E5 RID: 485
		private ISpellCaster mOwner;

		// Token: 0x040001E6 RID: 486
		public static readonly int SOUND_HASH = "magick_timewarp".GetHashCodeCustom();
	}
}
