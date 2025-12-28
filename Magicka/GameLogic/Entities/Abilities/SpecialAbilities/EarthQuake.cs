using System;
using System.Collections.Generic;
using JigLibX.Geometry;
using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Graphics;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using PolygonHead;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x0200003D RID: 61
	internal class EarthQuake : SpecialAbility, IAbilityEffect
	{
		// Token: 0x17000083 RID: 131
		// (get) Token: 0x06000271 RID: 625 RVA: 0x0000F7A4 File Offset: 0x0000D9A4
		public static EarthQuake Instance
		{
			get
			{
				if (EarthQuake.sSingelton == null)
				{
					lock (EarthQuake.sSingeltonLock)
					{
						if (EarthQuake.sSingelton == null)
						{
							EarthQuake.sSingelton = new EarthQuake();
						}
					}
				}
				return EarthQuake.sSingelton;
			}
		}

		// Token: 0x06000272 RID: 626 RVA: 0x0000F7F8 File Offset: 0x0000D9F8
		private EarthQuake() : base(Animations.cast_magick_global, "#magick_earthquake".GetHashCodeCustom())
		{
			this.mDamage = new Damage(AttackProperties.Knockdown, Elements.Earth, 1000f, 1f);
		}

		// Token: 0x06000273 RID: 627 RVA: 0x0000F849 File Offset: 0x0000DA49
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			if (this.Execute(iOwner.Position, iPlayState))
			{
				this.mOwner = iOwner;
				return true;
			}
			AudioManager.Instance.PlayCue(Banks.Spells, SpecialAbility.SOUND_MAGICK_FAIL, iOwner.AudioEmitter);
			return false;
		}

		// Token: 0x06000274 RID: 628 RVA: 0x0000F87C File Offset: 0x0000DA7C
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			this.mOwner = null;
			this.mPlayState = iPlayState;
			this.mTTL = 16f;
			this.mHitTimer = 0.5f;
			Segment iSeg = default(Segment);
			iSeg.Origin = iPosition;
			iSeg.Delta.Y = -20f;
			float num;
			Vector3 vector;
			Vector3 vector2;
			if (this.mPlayState.Level.CurrentScene.SegmentIntersect(out num, out vector, out vector2, iSeg))
			{
				this.mEpicenter = vector;
				if (!SpellManager.Instance.IsEffectActive(typeof(EarthQuake)))
				{
					SpellManager.Instance.AddSpellEffect(this);
				}
				return base.Execute(iPosition, iPlayState);
			}
			return false;
		}

		// Token: 0x17000084 RID: 132
		// (get) Token: 0x06000275 RID: 629 RVA: 0x0000F91E File Offset: 0x0000DB1E
		public bool IsDead
		{
			get
			{
				return this.mTTL <= 0f;
			}
		}

		// Token: 0x06000276 RID: 630 RVA: 0x0000F930 File Offset: 0x0000DB30
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			this.mTTL -= iDeltaTime;
			this.mHitTimer -= iDeltaTime;
			this.mQuakeTTL -= iDeltaTime;
			this.mHitList.Update(iDeltaTime);
			if (NetworkManager.Instance.State != NetworkState.Client && this.mQuakeTTL <= 0f)
			{
				float num = (float)Math.Sqrt(SpecialAbility.RANDOM.NextDouble());
				float num2 = (float)SpecialAbility.RANDOM.NextDouble() * 6.2831855f;
				float num3 = (float)((double)num * Math.Cos((double)num2));
				float num4 = (float)((double)num * Math.Sin((double)num2));
				Vector3 vector;
				vector.X = this.mEpicenter.X + num3 * 15f;
				vector.Z = this.mEpicenter.Z + num4 * 15f;
				vector.Y = this.mEpicenter.Y + 0.5f;
				float num5 = (float)SpecialAbility.RANDOM.NextDouble() + 1f;
				this.NewQuake(ref vector, num5);
				if (NetworkManager.Instance.State == NetworkState.Server)
				{
					TriggerActionMessage triggerActionMessage = default(TriggerActionMessage);
					triggerActionMessage.Handle = this.mOwner.Handle;
					triggerActionMessage.Position = this.mQuakePosition;
					triggerActionMessage.ActionType = TriggerActionType.EarthQuake;
					triggerActionMessage.Time = num5;
					NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref triggerActionMessage);
				}
			}
			if (this.mHitTimer <= 0f)
			{
				this.Quake(ref this.mQuakePosition, 10f);
				this.mHitTimer = 0.5f;
			}
		}

		// Token: 0x06000277 RID: 631 RVA: 0x0000FAB8 File Offset: 0x0000DCB8
		public void OnRemove()
		{
			if (this.mQuakeCue != null && this.mQuakeCue.IsPlaying)
			{
				this.mQuakeCue.Stop(AudioStopOptions.AsAuthored);
			}
			if (EffectManager.Instance.IsActive(ref this.mQuakeEffect))
			{
				EffectManager.Instance.Stop(ref this.mQuakeEffect);
			}
		}

		// Token: 0x06000278 RID: 632 RVA: 0x0000FB08 File Offset: 0x0000DD08
		public void NewQuake(ref Vector3 iPosition, float iTTL)
		{
			this.mQuakePosition = iPosition;
			this.mQuakeTTL = iTTL;
			this.mPlayState.Camera.CameraShake(this.mQuakePosition, 0.5f, this.mQuakeTTL);
		}

		// Token: 0x06000279 RID: 633 RVA: 0x0000FB40 File Offset: 0x0000DD40
		private void Quake(ref Vector3 iPosition, float iRadius)
		{
			List<Entity> entities = this.mPlayState.EntityManager.GetEntities(iPosition, iRadius, false, false);
			Vector3 vector = iPosition;
			vector.Y -= 5f;
			for (int i = 0; i < entities.Count; i++)
			{
				if (!this.mHitList.ContainsKey(entities[i].Handle))
				{
					if (entities[i] is IDamageable)
					{
						IDamageable damageable = entities[i] as IDamageable;
						if (entities[i] is Character)
						{
							Character character = entities[i] as Character;
							if (character.IsLevitating || character.CharacterBody.Mass >= 3000f || character.IsImmortal || character.IsEthereal || character.IsInAEvent || !character.CharacterBody.IsTouchingGround)
							{
								goto IL_21A;
							}
							Vector3 vector2 = new Vector3(0f, 6f, 0f);
							character.CharacterBody.AddImpulseVelocity(ref vector2);
						}
						this.mDamage.Amount = damageable.Body.Mass;
						Vector3 position = damageable.Position;
						position.X += (float)SpecialAbility.RANDOM.NextDouble() - 0.5f;
						position.Z += (float)SpecialAbility.RANDOM.NextDouble() - 0.5f;
						damageable.Damage(this.mDamage, this.mOwner as Entity, this.mTimeStamp, position);
					}
					else
					{
						Entity entity = entities[i];
						Vector3 velocity = entity.Body.Velocity;
						velocity.Y += 5f;
						velocity.X += (float)SpecialAbility.RANDOM.NextDouble() - 0.5f;
						velocity.Z += (float)SpecialAbility.RANDOM.NextDouble() - 0.5f;
						entity.Body.Velocity = velocity;
					}
					this.mHitList.Add(entities[i]);
				}
				IL_21A:;
			}
			this.mPlayState.EntityManager.ReturnEntityList(entities);
			if (this.mQuakeCue != null && this.mQuakeCue.IsPlaying)
			{
				this.mQuakeCue.Stop(AudioStopOptions.AsAuthored);
			}
			if (EffectManager.Instance.IsActive(ref this.mQuakeEffect))
			{
				EffectManager.Instance.Stop(ref this.mQuakeEffect);
			}
			this.mQuakeCue = AudioManager.Instance.PlayCue(Banks.Additional, EarthQuake.MAGICK_SOUND, this.mQuakeEmitter);
			this.mQuakeEmitter.Position = iPosition;
			this.mQuakeEmitter.Up = Vector3.Up;
			this.mQuakeEmitter.Forward = Vector3.Right;
			Matrix matrix = default(Matrix);
			matrix.M11 = iRadius / 5f;
			matrix.M22 = 1f;
			matrix.M33 = iRadius / 5f;
			matrix.M44 = 1f;
			matrix.Translation = iPosition;
			EffectManager.Instance.StartEffect(EarthQuake.MAGICK_EFFECT, ref matrix, out this.mQuakeEffect);
		}

		// Token: 0x040001E7 RID: 487
		private const float TTL = 16f;

		// Token: 0x040001E8 RID: 488
		private const float HIT_TTL = 0.5f;

		// Token: 0x040001E9 RID: 489
		private static EarthQuake sSingelton;

		// Token: 0x040001EA RID: 490
		private static volatile object sSingeltonLock = new object();

		// Token: 0x040001EB RID: 491
		private static readonly int MAGICK_EFFECT = "magick_earthquake".GetHashCodeCustom();

		// Token: 0x040001EC RID: 492
		private static readonly int MAGICK_EFFECT_HIT = "magick_earthquake_hit".GetHashCodeCustom();

		// Token: 0x040001ED RID: 493
		private static readonly int MAGICK_SOUND = "magick_earthquake".GetHashCodeCustom();

		// Token: 0x040001EE RID: 494
		private float mTTL;

		// Token: 0x040001EF RID: 495
		private Vector3 mEpicenter;

		// Token: 0x040001F0 RID: 496
		private ISpellCaster mOwner;

		// Token: 0x040001F1 RID: 497
		private PlayState mPlayState;

		// Token: 0x040001F2 RID: 498
		private Cue mQuakeCue;

		// Token: 0x040001F3 RID: 499
		private AudioEmitter mQuakeEmitter = new AudioEmitter();

		// Token: 0x040001F4 RID: 500
		private VisualEffectReference mQuakeEffect;

		// Token: 0x040001F5 RID: 501
		private float mQuakeTTL;

		// Token: 0x040001F6 RID: 502
		private Vector3 mQuakePosition;

		// Token: 0x040001F7 RID: 503
		private Damage mDamage;

		// Token: 0x040001F8 RID: 504
		private float mHitTimer;

		// Token: 0x040001F9 RID: 505
		private HitList mHitList = new HitList(128);
	}
}
