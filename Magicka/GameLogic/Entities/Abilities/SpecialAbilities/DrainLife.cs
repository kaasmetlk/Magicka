using System;
using System.Collections.Generic;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Graphics;
using Microsoft.Xna.Framework;
using PolygonHead;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x02000457 RID: 1111
	public class DrainLife : SpecialAbility, IAbilityEffect
	{
		// Token: 0x060021F5 RID: 8693 RVA: 0x000F3453 File Offset: 0x000F1653
		public DrainLife(Animations iAnimation) : base(iAnimation, "#specab_drain".GetHashCodeCustom())
		{
		}

		// Token: 0x060021F6 RID: 8694 RVA: 0x000F3468 File Offset: 0x000F1668
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			base.Execute(iOwner, iPlayState);
			this.mOwner = iOwner;
			List<Entity> entities = this.mOwner.PlayState.EntityManager.GetEntities(this.mOwner.Position, 8f, false, true);
			Entity entity = null;
			float num = float.MaxValue;
			Vector3 position = this.mOwner.Position;
			foreach (Entity entity2 in entities)
			{
				if (entity2 is Character && entity2.Position != this.mOwner.Position)
				{
					Vector3 position2 = entity2.Position;
					float num2;
					Vector3.DistanceSquared(ref position2, ref position, out num2);
					float num3 = 8f + entity2.Radius;
					num3 *= num3;
					if (num2 <= num3 && num2 < num)
					{
						entity = entity2;
						num = num2;
					}
				}
			}
			Character character = entity as Character;
			if (character == null)
			{
				return false;
			}
			this.LifeStealAmount = this.mOwner.MaxHitPoints - this.mOwner.HitPoints;
			if (this.LifeStealAmount > character.HitPoints)
			{
				this.LifeStealAmount = character.HitPoints;
			}
			character.Damage(this.LifeStealAmount, Elements.Arcane);
			this.mOwner.PlayState.EntityManager.ReturnEntityList(entities);
			this.mPlayState = iPlayState;
			if (this.IsDead)
			{
				this.targetInitialPos = character.Position;
				Vector3 vector = this.mOwner.Position - character.Position;
				vector.Normalize();
				EffectManager.Instance.StartEffect(DrainLife.EFFECT, ref this.targetInitialPos, ref vector, out this.mEffect);
				SpellManager.Instance.AddSpellEffect(this);
			}
			this.mTTL = 1f;
			return true;
		}

		// Token: 0x060021F7 RID: 8695 RVA: 0x000F3638 File Offset: 0x000F1838
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			throw new NotImplementedException();
		}

		// Token: 0x1700082B RID: 2091
		// (get) Token: 0x060021F8 RID: 8696 RVA: 0x000F363F File Offset: 0x000F183F
		public bool IsDead
		{
			get
			{
				return this.mTTL <= 0f;
			}
		}

		// Token: 0x060021F9 RID: 8697 RVA: 0x000F3654 File Offset: 0x000F1854
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			this.mTTL -= iDeltaTime;
			this.mHitTimer -= iDeltaTime;
			Vector3 vector = this.mOwner.Position - this.targetInitialPos;
			vector.Normalize();
			this.effectPos = Vector3.Lerp(this.targetInitialPos, this.mOwner.Position, 1f - this.mTTL);
			EffectManager.Instance.UpdatePositionDirection(ref this.mEffect, ref this.effectPos, ref vector);
			if (this.mHitTimer <= 0f)
			{
				this.mHitTimer = 0.25f;
			}
		}

		// Token: 0x060021FA RID: 8698 RVA: 0x000F36F4 File Offset: 0x000F18F4
		public void OnRemove()
		{
			this.mOwner.Damage(-this.LifeStealAmount, Elements.Life);
			EffectManager.Instance.Stop(ref this.mEffect);
		}

		// Token: 0x040024F6 RID: 9462
		private const float TTL = 1f;

		// Token: 0x040024F7 RID: 9463
		private const float RANGE = 8f;

		// Token: 0x040024F8 RID: 9464
		private const float HIT_TIME = 0.25f;

		// Token: 0x040024F9 RID: 9465
		public static readonly int EFFECT = "drainlife".GetHashCodeCustom();

		// Token: 0x040024FA RID: 9466
		private ISpellCaster mOwner;

		// Token: 0x040024FB RID: 9467
		private PlayState mPlayState;

		// Token: 0x040024FC RID: 9468
		private VisualEffectReference mEffect;

		// Token: 0x040024FD RID: 9469
		private float mHitTimer;

		// Token: 0x040024FE RID: 9470
		private float mTTL;

		// Token: 0x040024FF RID: 9471
		private float LifeStealAmount;

		// Token: 0x04002500 RID: 9472
		private Vector3 effectPos;

		// Token: 0x04002501 RID: 9473
		private Vector3 targetInitialPos;
	}
}
