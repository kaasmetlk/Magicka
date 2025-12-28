using System;
using Microsoft.Xna.Framework;

namespace Magicka.GameLogic.Entities.CharacterStates
{
	// Token: 0x020004D6 RID: 1238
	internal class GrippingState : BaseState
	{
		// Token: 0x170008A0 RID: 2208
		// (get) Token: 0x060024D8 RID: 9432 RVA: 0x0010A4C4 File Offset: 0x001086C4
		public static GrippingState Instance
		{
			get
			{
				if (GrippingState.mSingelton == null)
				{
					lock (GrippingState.mSingeltonLock)
					{
						if (GrippingState.mSingelton == null)
						{
							GrippingState.mSingelton = new GrippingState();
						}
					}
				}
				return GrippingState.mSingelton;
			}
		}

		// Token: 0x060024D9 RID: 9433 RVA: 0x0010A518 File Offset: 0x00108718
		public override void OnEnter(Character iOwner)
		{
			if (iOwner.HasAnimation(Animations.idle_grip))
			{
				iOwner.GoToAnimation(Animations.idle_grip, 0.1f);
			}
			else
			{
				iOwner.GoToAnimation(Animations.idle, 0.1f);
			}
			iOwner.CharacterBody.AllowRotate = false;
			NonPlayerCharacter nonPlayerCharacter = iOwner as NonPlayerCharacter;
			if (nonPlayerCharacter != null)
			{
				nonPlayerCharacter.AI.BusyAbility = null;
			}
		}

		// Token: 0x060024DA RID: 9434 RVA: 0x0010A56C File Offset: 0x0010876C
		public override BaseState Update(Character iOwner, float iDeltaTime)
		{
			BaseState baseState = base.UpdateHit(iOwner, iDeltaTime);
			if (baseState != null)
			{
				return baseState;
			}
			if (iOwner.GripAttacking)
			{
				return GripAttackState.Instance;
			}
			if (iOwner.Attacking)
			{
				return AttackState.Instance;
			}
			if (iOwner.IsGripping && (iOwner.ShouldReleaseGrip || iOwner.GripDamageAccumulation > iOwner.HitTolerance))
			{
				if (!iOwner.CharacterBody.IsTouchingGround || !iOwner.GripperAttached)
				{
					Vector3 vector = default(Vector3);
					iOwner.CharacterBody.AddImpulseVelocity(ref vector);
					iOwner.KnockDown();
					return KnockDownState.Instance;
				}
				return DropState.Instance;
			}
			else
			{
				if (iOwner.IsGripping)
				{
					return null;
				}
				if (iOwner.CharacterBody.Movement.Length() < 1E-45f)
				{
					return IdleState.Instance;
				}
				return MoveState.Instance;
			}
		}

		// Token: 0x060024DB RID: 9435 RVA: 0x0010A62C File Offset: 0x0010882C
		public override void OnExit(Character iOwner)
		{
			base.OnExit(iOwner);
			iOwner.CharacterBody.AllowRotate = true;
		}

		// Token: 0x0400283F RID: 10303
		private static GrippingState mSingelton;

		// Token: 0x04002840 RID: 10304
		private static volatile object mSingeltonLock = new object();
	}
}
