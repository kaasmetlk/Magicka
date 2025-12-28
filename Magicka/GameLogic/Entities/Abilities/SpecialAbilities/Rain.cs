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
	// Token: 0x020005A3 RID: 1443
	public class Rain : SpecialAbility, IAbilityEffect
	{
		// Token: 0x17000A22 RID: 2594
		// (get) Token: 0x06002B22 RID: 11042 RVA: 0x00153308 File Offset: 0x00151508
		public static Rain Instance
		{
			get
			{
				if (Rain.mSingelton == null)
				{
					lock (Rain.mSingeltonLock)
					{
						if (Rain.mSingelton == null)
						{
							Rain.mSingelton = new Rain();
						}
					}
				}
				return Rain.mSingelton;
			}
		}

		// Token: 0x06002B23 RID: 11043 RVA: 0x0015335C File Offset: 0x0015155C
		private Rain() : base(Animations.cast_magick_global, "#magick_rain".GetHashCodeCustom())
		{
			this.mDamage.AttackProperty = AttackProperties.Status;
			this.mDamage.Element = Elements.Water;
		}

		// Token: 0x06002B24 RID: 11044 RVA: 0x00153389 File Offset: 0x00151589
		public Rain(Animations iAnimation) : base(iAnimation, "#magick_rain".GetHashCodeCustom())
		{
		}

		// Token: 0x06002B25 RID: 11045 RVA: 0x0015339C File Offset: 0x0015159C
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			this.mCaster = null;
			this.mTimeStamp = 0.0;
			this.mPlayState = iPlayState;
			this.mDoDamage = (NetworkManager.Instance.State != NetworkState.Client);
			return this.Execute();
		}

		// Token: 0x06002B26 RID: 11046 RVA: 0x001533D8 File Offset: 0x001515D8
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

		// Token: 0x06002B27 RID: 11047 RVA: 0x00153494 File Offset: 0x00151694
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
				EffectManager.Instance.StartEffect(Rain.EFFECT, ref position, ref vector, out this.mEffect);
			}
			this.mScene = this.mPlayState.Level.CurrentScene;
			if (this.mAmbience == null || !this.mAmbience.IsPlaying)
			{
				this.mAmbience = AudioManager.Instance.PlayCue(Banks.Spells, Rain.AMBIENCE);
			}
			this.mTTL = 17.5f;
			this.mScene.LightTargetIntensity = 0.333f;
			SpellManager.Instance.AddSpellEffect(this);
			if (this.mCaster is Avatar)
			{
				this.mPlayState.IncrementBlizzardRainCount();
			}
			return true;
		}

		// Token: 0x17000A23 RID: 2595
		// (get) Token: 0x06002B28 RID: 11048 RVA: 0x0015359D File Offset: 0x0015179D
		public bool IsDead
		{
			get
			{
				return this.mTTL <= 0f;
			}
		}

		// Token: 0x06002B29 RID: 11049 RVA: 0x001535B0 File Offset: 0x001517B0
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
				if (this.mWetTimer <= 0f)
				{
					EntityManager entityManager = this.mPlayState.EntityManager;
					StaticList<Entity> entities = entityManager.Entities;
					foreach (Entity entity in entities)
					{
						IDamageable damageable = entity as IDamageable;
						Shield shield = null;
						if (!(damageable == null | entity is MissileEntity) && damageable.ResistanceAgainst(Elements.Water) != 1f && !entityManager.IsProtectedByShield(entity, out shield))
						{
							damageable.Damage(this.mDamage, this.mCaster as Entity, this.mTimeStamp, default(Vector3));
						}
					}
					this.mWetTimer = 0.25f;
				}
			}
		}

		// Token: 0x06002B2A RID: 11050 RVA: 0x0015370C File Offset: 0x0015190C
		public void OnRemove()
		{
			if (this.mAmbience != null && !this.mAmbience.IsStopping)
			{
				this.mAmbience.Stop(AudioStopOptions.AsAuthored);
			}
			EffectManager.Instance.Stop(ref this.mEffect);
			this.mScene.LightTargetIntensity = 1f;
		}

		// Token: 0x04002E59 RID: 11865
		private const float MAGICK_TTL = 17.5f;

		// Token: 0x04002E5A RID: 11866
		private static Rain mSingelton;

		// Token: 0x04002E5B RID: 11867
		private static volatile object mSingeltonLock = new object();

		// Token: 0x04002E5C RID: 11868
		private float mTTL;

		// Token: 0x04002E5D RID: 11869
		private float mWetTimer;

		// Token: 0x04002E5E RID: 11870
		private VisualEffectReference mEffect;

		// Token: 0x04002E5F RID: 11871
		public static readonly int AMBIENCE = "magick_rain".GetHashCodeCustom();

		// Token: 0x04002E60 RID: 11872
		public static readonly int EFFECT = "magick_rain".GetHashCodeCustom();

		// Token: 0x04002E61 RID: 11873
		private PlayState mPlayState;

		// Token: 0x04002E62 RID: 11874
		private GameScene mScene;

		// Token: 0x04002E63 RID: 11875
		private Damage mDamage;

		// Token: 0x04002E64 RID: 11876
		private Cue mAmbience;

		// Token: 0x04002E65 RID: 11877
		private ISpellCaster mCaster;

		// Token: 0x04002E66 RID: 11878
		private bool mDoDamage;
	}
}
