using System;
using Microsoft.Xna.Framework;

namespace Magicka.GameLogic.Entities.CharacterStates
{
	// Token: 0x020000E5 RID: 229
	public abstract class BaseState
	{
		// Token: 0x06000711 RID: 1809 RVA: 0x0002963C File Offset: 0x0002783C
		public virtual void OnEnter(Character iOwner)
		{
		}

		// Token: 0x06000712 RID: 1810 RVA: 0x0002963E File Offset: 0x0002783E
		public virtual BaseState UpdateBloatDeath(Character iOwner, float iDeltaTime)
		{
			if (iOwner.Bloating)
			{
				return BloatState.Instance;
			}
			if ((iOwner.Overkilled || iOwner.HitPoints <= 0f) && !iOwner.CannotDieWithoutExplicitKill)
			{
				return DeadState.Instance;
			}
			return null;
		}

		// Token: 0x06000713 RID: 1811 RVA: 0x00029674 File Offset: 0x00027874
		public virtual BaseState UpdateHit(Character iOwner, float iDeltaTime)
		{
			if (!iOwner.CharacterBody.IsJumping && !iOwner.IsGripped && iOwner.CharacterBody.IsPushed && !iOwner.CharacterBody.IsTouchingGround)
			{
				return FlyingState.Instance;
			}
			if ((iOwner.IsEntangled || (iOwner.Gripper != null && !iOwner.GripperAttached && !iOwner.AttachedToGripped)) && !iOwner.Attacking)
			{
				return EntangledState.Instance;
			}
			if (iOwner.CharacterBody.IsPushed)
			{
				return PushedState.Instance;
			}
			if (iOwner.IsKnockedDown)
			{
				return KnockDownState.Instance;
			}
			if (iOwner.IsGripping && (iOwner.ShouldReleaseGrip || iOwner.GripDamageAccumulation > iOwner.HitTolerance))
			{
				if (iOwner.CharacterBody.IsTouchingGround && iOwner.GripperAttached)
				{
					return DropState.Instance;
				}
				Vector3 vector = default(Vector3);
				iOwner.CharacterBody.AddImpulseVelocity(ref vector);
				iOwner.KnockDown();
			}
			if (iOwner.IsHit)
			{
				return HitState.Instance;
			}
			if (iOwner.IsStunned && !iOwner.IsHit && !iOwner.IsKnockedDown && !iOwner.CharacterBody.IsPushed)
			{
				return StunState.Instance;
			}
			return null;
		}

		// Token: 0x06000714 RID: 1812 RVA: 0x00029798 File Offset: 0x00027998
		public virtual BaseState UpdateActions(Character iOwner, float iDeltaTime)
		{
			if (iOwner.CharacterBody.IsJumping && !iOwner.IsGripping)
			{
				return JumpState.Instance;
			}
			if ((iOwner.IsPanicing | iOwner.IsStumbling | iOwner.IsFeared) && !iOwner.IsEntangled)
			{
				return PanicState.Instance;
			}
			if (iOwner.Boosts > 0)
			{
				if (BoostState.Instance.ShieldToBoost(iOwner) != null || iOwner.IsSelfShielded)
				{
					return BoostState.Instance;
				}
				iOwner.Boosts = 0;
			}
			if (iOwner.IsGripping && !iOwner.AttachedToGripped)
			{
				return GrippingState.Instance;
			}
			return null;
		}

		// Token: 0x06000715 RID: 1813 RVA: 0x00029828 File Offset: 0x00027A28
		public virtual BaseState Update(Character iOwner, float iDeltaTime)
		{
			return null;
		}

		// Token: 0x06000716 RID: 1814 RVA: 0x0002982B File Offset: 0x00027A2B
		public virtual void OnExit(Character iOwner)
		{
		}
	}
}
