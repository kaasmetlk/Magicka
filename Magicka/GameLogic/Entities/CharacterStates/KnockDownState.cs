using System;

namespace Magicka.GameLogic.Entities.CharacterStates
{
	// Token: 0x020003B2 RID: 946
	public class KnockDownState : BaseState
	{
		// Token: 0x17000731 RID: 1841
		// (get) Token: 0x06001D1A RID: 7450 RVA: 0x000CEC80 File Offset: 0x000CCE80
		public static KnockDownState Instance
		{
			get
			{
				if (KnockDownState.mSingelton == null)
				{
					lock (KnockDownState.mSingeltonLock)
					{
						if (KnockDownState.mSingelton == null)
						{
							KnockDownState.mSingelton = new KnockDownState();
						}
					}
				}
				return KnockDownState.mSingelton;
			}
		}

		// Token: 0x06001D1B RID: 7451 RVA: 0x000CECD4 File Offset: 0x000CCED4
		public override void OnEnter(Character iOwner)
		{
			iOwner.CharacterBody.AllowRotate = false;
			iOwner.GoToAnimation(Animations.hit_stun_begin, 0.05f);
			iOwner.ReleaseAttachedCharacter();
			if (iOwner.PreviousState is ChargeState)
			{
				iOwner.CastSpell(true, "");
			}
		}

		// Token: 0x06001D1C RID: 7452 RVA: 0x000CED10 File Offset: 0x000CCF10
		public override BaseState Update(Character iOwner, float iDeltaTime)
		{
			iOwner.IsHit = false;
			iOwner.IsKnockedDown = false;
			BaseState baseState = this.UpdateBloatDeath(iOwner, iDeltaTime);
			if (baseState == null)
			{
				baseState = this.UpdateHit(iOwner, iDeltaTime);
			}
			if (baseState != null)
			{
				return baseState;
			}
			if ((iOwner.AnimationController.HasFinished && !iOwner.AnimationController.CrossFadeEnabled) || iOwner.HasStatus(StatusEffects.Frozen))
			{
				if (iOwner.CurrentAnimation == Animations.hit_stun_begin)
				{
					iOwner.GoToAnimation(Animations.hit_stun_end, 0.05f);
				}
				else
				{
					if (iOwner.CharacterBody.Movement.Length() < 1E-45f)
					{
						return IdleState.Instance;
					}
					return MoveState.Instance;
				}
			}
			return null;
		}

		// Token: 0x06001D1D RID: 7453 RVA: 0x000CEDA8 File Offset: 0x000CCFA8
		public override void OnExit(Character iOwner)
		{
			iOwner.IsHit = false;
			iOwner.IsKnockedDown = false;
		}

		// Token: 0x04001FB9 RID: 8121
		private static KnockDownState mSingelton;

		// Token: 0x04001FBA RID: 8122
		private static volatile object mSingeltonLock = new object();
	}
}
