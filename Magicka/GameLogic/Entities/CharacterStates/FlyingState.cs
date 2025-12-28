using System;

namespace Magicka.GameLogic.Entities.CharacterStates
{
	// Token: 0x02000439 RID: 1081
	public class FlyingState : BaseState
	{
		// Token: 0x17000821 RID: 2081
		// (get) Token: 0x0600217D RID: 8573 RVA: 0x000EF1C0 File Offset: 0x000ED3C0
		public static FlyingState Instance
		{
			get
			{
				if (FlyingState.mSingelton == null)
				{
					lock (FlyingState.mSingeltonLock)
					{
						if (FlyingState.mSingelton == null)
						{
							FlyingState.mSingelton = new FlyingState();
						}
					}
				}
				return FlyingState.mSingelton;
			}
		}

		// Token: 0x0600217E RID: 8574 RVA: 0x000EF214 File Offset: 0x000ED414
		public override void OnEnter(Character iOwner)
		{
			iOwner.CharacterBody.AllowRotate = false;
			iOwner.GoToAnimation(Animations.hit_fly, 0.03f);
			iOwner.ReleaseAttachedCharacter();
		}

		// Token: 0x0600217F RID: 8575 RVA: 0x000EF238 File Offset: 0x000ED438
		public override BaseState Update(Character iOwner, float iDeltaTime)
		{
			iOwner.IsHit = false;
			BaseState baseState = this.UpdateBloatDeath(iOwner, iDeltaTime);
			if (baseState != null)
			{
				return baseState;
			}
			if (iOwner.IsGripping | iOwner.IsGripped)
			{
				if (iOwner.CharacterBody.Movement.Length() < 1E-45f)
				{
					return IdleState.Instance;
				}
				return MoveState.Instance;
			}
			else
			{
				if (!iOwner.CharacterBody.IsTouchingGround)
				{
					return null;
				}
				if (iOwner.IsKnockedDown)
				{
					return KnockDownState.Instance;
				}
				if (iOwner.CharacterBody.Movement.Length() < 1E-45f)
				{
					return IdleState.Instance;
				}
				return MoveState.Instance;
			}
		}

		// Token: 0x06002180 RID: 8576 RVA: 0x000EF2D1 File Offset: 0x000ED4D1
		public override void OnExit(Character iOwner)
		{
			iOwner.IsHit = false;
		}

		// Token: 0x04002443 RID: 9283
		private static FlyingState mSingelton;

		// Token: 0x04002444 RID: 9284
		private static volatile object mSingeltonLock = new object();
	}
}
