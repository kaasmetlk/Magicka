using System;
using System.Collections.Generic;
using JigLibX.Geometry;
using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Graphics;
using Magicka.Levels;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using PolygonHead;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x02000458 RID: 1112
	public class PoisonSpray : SpecialAbility, IAbilityEffect
	{
		// Token: 0x060021FC RID: 8700 RVA: 0x000F372B File Offset: 0x000F192B
		public PoisonSpray(Animations iAnimation) : base(iAnimation, "#specab_poison".GetHashCodeCustom())
		{
		}

		// Token: 0x060021FD RID: 8701 RVA: 0x000F3740 File Offset: 0x000F1940
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			base.Execute(iOwner, iPlayState);
			this.mOwner = iOwner;
			if (this.mOwner == null)
			{
				throw new Exception("Grease can not be cast without an owner!");
			}
			this.mPlayState = iPlayState;
			this.mTTL = 0.5f;
			this.mCue = AudioManager.Instance.PlayCue(Banks.Spells, PoisonSpray.SOUNDHASH, this.mOwner.AudioEmitter);
			Vector3 translation = iOwner.CastSource.Translation;
			Vector3 direction = iOwner.Direction;
			EffectManager.Instance.StartEffect(PoisonSpray.EFFECT, ref translation, ref direction, out this.mEffect);
			SpellManager.Instance.AddSpellEffect(this);
			return true;
		}

		// Token: 0x060021FE RID: 8702 RVA: 0x000F37E0 File Offset: 0x000F19E0
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			return false;
		}

		// Token: 0x1700082C RID: 2092
		// (get) Token: 0x060021FF RID: 8703 RVA: 0x000F37E3 File Offset: 0x000F19E3
		public bool IsDead
		{
			get
			{
				return this.mTTL <= 0f;
			}
		}

		// Token: 0x06002200 RID: 8704 RVA: 0x000F37F8 File Offset: 0x000F19F8
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			this.mTTL -= iDeltaTime;
			Vector3 vector = this.mOwner.Position;
			Vector3 direction = this.mOwner.Direction;
			float yaw = -(this.mTTL / 0.5f * 6f - 2.5f) / 2.5f * 0.7853982f;
			Quaternion quaternion;
			Quaternion.CreateFromYawPitchRoll(yaw, 0f, 0f, out quaternion);
			float num = 6f;
			Segment segment;
			segment.Origin = vector;
			Vector3.Transform(ref direction, ref quaternion, out direction);
			Vector3.Multiply(ref direction, num, out segment.Delta);
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				GameScene currentScene = this.mOwner.PlayState.Level.CurrentScene;
				List<Shield> shields = this.mOwner.PlayState.EntityManager.Shields;
				float num2;
				Vector3 vector2;
				Vector3 vector3;
				for (int i = 0; i < shields.Count; i++)
				{
					if (shields[i].Body.CollisionSkin.SegmentIntersect(out num2, out vector2, out vector3, segment))
					{
						num *= num2;
						Vector3.Multiply(ref segment.Delta, num2, out segment.Delta);
					}
				}
				if (currentScene.SegmentIntersect(out num2, out vector2, out vector3, segment))
				{
					num *= num2;
					Vector3.Multiply(ref segment.Delta, num2, out segment.Delta);
				}
				List<Entity> entities = this.mPlayState.EntityManager.GetEntities(vector, num, true);
				entities.Remove(this.mOwner as Entity);
				for (int j = 0; j < entities.Count; j++)
				{
					Character character = entities[j] as Character;
					Vector3 iAttackPosition;
					if (character != null && !character.HasStatus(StatusEffects.Greased) && character.ArcIntersect(out iAttackPosition, segment.Origin, direction, num, 0.17453292f, 5f))
					{
						character.Damage(new Damage(AttackProperties.Status, Elements.Poison, 75f, 1f), this.mOwner as Entity, this.mTimeStamp, iAttackPosition);
					}
				}
				this.mPlayState.EntityManager.ReturnEntityList(entities);
			}
			vector = this.mOwner.CastSource.Translation;
			EffectManager.Instance.UpdatePositionDirection(ref this.mEffect, ref vector, ref direction);
		}

		// Token: 0x06002201 RID: 8705 RVA: 0x000F3A34 File Offset: 0x000F1C34
		public void OnRemove()
		{
			EffectManager.Instance.Stop(ref this.mEffect);
			if (this.mCue.IsPlaying)
			{
				this.mCue.Stop(AudioStopOptions.AsAuthored);
			}
		}

		// Token: 0x04002502 RID: 9474
		public const float GREASESPRAY_TTL = 0.5f;

		// Token: 0x04002503 RID: 9475
		public const int NR_OF_FIELDS = 6;

		// Token: 0x04002504 RID: 9476
		public const float NR_OF_FIELDS_F = 6f;

		// Token: 0x04002505 RID: 9477
		public static readonly int EFFECT = "scythe_spray".GetHashCodeCustom();

		// Token: 0x04002506 RID: 9478
		public static readonly int SOUNDHASH = "spell_poison_spray".GetHashCodeCustom();

		// Token: 0x04002507 RID: 9479
		private ISpellCaster mOwner;

		// Token: 0x04002508 RID: 9480
		private PlayState mPlayState;

		// Token: 0x04002509 RID: 9481
		private VisualEffectReference mEffect;

		// Token: 0x0400250A RID: 9482
		private Cue mCue;

		// Token: 0x0400250B RID: 9483
		private float mTTL;
	}
}
