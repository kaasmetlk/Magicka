using System;
using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Gamers;
using Magicka.Graphics;
using Magicka.Levels;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using PolygonHead;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x0200003F RID: 63
	public class HealingRain : SpecialAbility, IAbilityEffect
	{
		// Token: 0x17000087 RID: 135
		// (get) Token: 0x06000283 RID: 643 RVA: 0x000101F0 File Offset: 0x0000E3F0
		public static HealingRain Instance
		{
			get
			{
				if (HealingRain.mSingelton == null)
				{
					lock (HealingRain.mSingeltonLock)
					{
						if (HealingRain.mSingelton == null)
						{
							HealingRain.mSingelton = new HealingRain();
						}
					}
				}
				return HealingRain.mSingelton;
			}
		}

		// Token: 0x06000284 RID: 644 RVA: 0x00010244 File Offset: 0x0000E444
		private HealingRain() : base(Animations.cast_magick_global, "#magick_rain".GetHashCodeCustom())
		{
			this.mDamage.AttackProperty = AttackProperties.Damage;
			this.mStatus.AttackProperty = AttackProperties.Status;
			this.mDamage.Element = Elements.Life;
			this.mStatus.Element = Elements.Water;
		}

		// Token: 0x06000285 RID: 645 RVA: 0x000102A0 File Offset: 0x0000E4A0
		public HealingRain(Animations iAnimation) : base(iAnimation, "#magick_rain".GetHashCodeCustom())
		{
			this.mDamage.AttackProperty = AttackProperties.Damage;
			this.mStatus.AttackProperty = AttackProperties.Status;
			this.mDamage.Element = Elements.Life;
			this.mStatus.Element = Elements.Water;
		}

		// Token: 0x06000286 RID: 646 RVA: 0x000102FB File Offset: 0x0000E4FB
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			this.mCaster = null;
			this.mTimeStamp = 0.0;
			this.mPlayState = iPlayState;
			this.mDoDamage = (NetworkManager.Instance.State != NetworkState.Client);
			return this.Execute();
		}

		// Token: 0x06000287 RID: 647 RVA: 0x00010338 File Offset: 0x0000E538
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			base.Execute(iOwner, iPlayState);
			this.mCaster = iOwner;
			this.mTimeStamp = iOwner.PlayState.PlayTime;
			this.mPlayState = iPlayState;
			Avatar avatar = iOwner as Avatar;
			if (NetworkManager.Instance.State == NetworkState.Client)
			{
				if (avatar != null && !(avatar.Player.Gamer is NetworkGamer))
				{
					this.mDoDamage = true;
				}
				else
				{
					this.mDoDamage = false;
				}
			}
			else if (NetworkManager.Instance.State == NetworkState.Server)
			{
				if (avatar == null)
				{
					this.mDoDamage = true;
				}
				else if (avatar.Player.Gamer is NetworkGamer)
				{
					this.mDoDamage = false;
				}
				else
				{
					this.mDoDamage = true;
				}
			}
			else
			{
				this.mDoDamage = true;
			}
			return this.Execute();
		}

		// Token: 0x06000288 RID: 648 RVA: 0x000103F4 File Offset: 0x0000E5F4
		private bool Execute()
		{
			if (this.mPlayState.Level.CurrentScene.Indoors)
			{
				return false;
			}
			Vector3 position = this.mPlayState.Camera.Position;
			Vector3 cameraoffset = MagickCamera.CAMERAOFFSET;
			Vector3 vector = default(Vector3);
			vector.Z = -1f;
			Vector3.Subtract(ref position, ref cameraoffset, out position);
			if (!EffectManager.Instance.IsActive(ref this.mEffect))
			{
				EffectManager.Instance.StartEffect(HealingRain.EFFECT, ref position, ref vector, out this.mEffect);
			}
			this.mScene = this.mPlayState.Level.CurrentScene;
			if (this.mAmbience == null || !this.mAmbience.IsPlaying)
			{
				this.mAmbience = AudioManager.Instance.PlayCue(Banks.Spells, HealingRain.AMBIENCE);
			}
			this.mTTL = 8f;
			this.mScene.LightTargetIntensity = 0.333f;
			SpellManager.Instance.AddSpellEffect(this);
			if (this.mCaster is Avatar)
			{
				this.mPlayState.IncrementBlizzardRainCount();
			}
			return true;
		}

		// Token: 0x17000088 RID: 136
		// (get) Token: 0x06000289 RID: 649 RVA: 0x000104FD File Offset: 0x0000E6FD
		public bool IsDead
		{
			get
			{
				return this.mTTL <= 0f;
			}
		}

		// Token: 0x0600028A RID: 650 RVA: 0x00010510 File Offset: 0x0000E710
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			this.mTTL -= iDeltaTime;
			this.mWetTimer -= iDeltaTime;
			Vector3 position = this.mPlayState.Camera.Position;
			Vector3 cameraoffset = MagickCamera.CAMERAOFFSET;
			Vector3 vector = default(Vector3);
			vector.Z = -1f;
			Vector3.Subtract(ref position, ref cameraoffset, out position);
			EffectManager.Instance.UpdatePositionDirection(ref this.mEffect, ref position, ref vector);
			if (this.mDoDamage)
			{
				this.mDamage.Magnitude = 0.25f;
				this.mStatus.Magnitude = 0.25f;
				this.mDamage.Amount = this.HEALING_AMOUNT;
				if (this.mWetTimer <= 0f)
				{
					EntityManager entityManager = this.mPlayState.EntityManager;
					StaticList<Entity> entities = entityManager.Entities;
					foreach (Entity entity in entities)
					{
						IDamageable damageable = entity as IDamageable;
						Shield shield = null;
						if (!(damageable == null | entity is MissileEntity) && damageable.ResistanceAgainst(Elements.Life) != 1f && damageable.ResistanceAgainst(Elements.Water) != 1f && !entityManager.IsProtectedByShield(entity, out shield))
						{
							damageable.Damage(this.mDamage, this.mCaster as Entity, this.mTimeStamp, default(Vector3));
							damageable.Damage(this.mStatus, this.mCaster as Entity, this.mTimeStamp, default(Vector3));
						}
					}
					this.mWetTimer = 0.25f;
				}
			}
		}

		// Token: 0x0600028B RID: 651 RVA: 0x000106CC File Offset: 0x0000E8CC
		public void OnRemove()
		{
			if (this.mAmbience != null && !this.mAmbience.IsStopping)
			{
				this.mAmbience.Stop(AudioStopOptions.AsAuthored);
			}
			EffectManager.Instance.Stop(ref this.mEffect);
			this.mScene.LightTargetIntensity = 1f;
		}

		// Token: 0x0400020A RID: 522
		private const float MAGICK_TTL = 8f;

		// Token: 0x0400020B RID: 523
		private static HealingRain mSingelton;

		// Token: 0x0400020C RID: 524
		private static volatile object mSingeltonLock = new object();

		// Token: 0x0400020D RID: 525
		private float mTTL;

		// Token: 0x0400020E RID: 526
		private float HEALING_AMOUNT = -20f;

		// Token: 0x0400020F RID: 527
		private float mWetTimer;

		// Token: 0x04000210 RID: 528
		private VisualEffectReference mEffect;

		// Token: 0x04000211 RID: 529
		public static readonly int AMBIENCE = "magick_rain".GetHashCodeCustom();

		// Token: 0x04000212 RID: 530
		public static readonly int EFFECT = "magick_healingrain".GetHashCodeCustom();

		// Token: 0x04000213 RID: 531
		private PlayState mPlayState;

		// Token: 0x04000214 RID: 532
		private GameScene mScene;

		// Token: 0x04000215 RID: 533
		private Damage mDamage;

		// Token: 0x04000216 RID: 534
		private Damage mStatus;

		// Token: 0x04000217 RID: 535
		private Cue mAmbience;

		// Token: 0x04000218 RID: 536
		private ISpellCaster mCaster;

		// Token: 0x04000219 RID: 537
		private bool mDoDamage;
	}
}
