using System;
using Magicka.GameLogic.Spells;

namespace Magicka.GameLogic.Entities.CharacterStates
{
	// Token: 0x02000348 RID: 840
	public class EntangledState : BaseState
	{
		// Token: 0x1700066C RID: 1644
		// (get) Token: 0x060019B5 RID: 6581 RVA: 0x000ACA58 File Offset: 0x000AAC58
		public static EntangledState Instance
		{
			get
			{
				if (EntangledState.mSingelton == null)
				{
					lock (EntangledState.mSingeltonLock)
					{
						if (EntangledState.mSingelton == null)
						{
							EntangledState.mSingelton = new EntangledState();
						}
					}
				}
				return EntangledState.mSingelton;
			}
		}

		// Token: 0x060019B6 RID: 6582 RVA: 0x000ACAAC File Offset: 0x000AACAC
		public override void OnEnter(Character iOwner)
		{
			iOwner.CharacterBody.AllowMove = false;
			iOwner.CharacterBody.AllowRotate = false;
			iOwner.GoToAnimation(Animations.spec_entangled, 0.2f);
			iOwner.SpecialIdleAnimation = Animations.None;
		}

		// Token: 0x060019B7 RID: 6583 RVA: 0x000ACAE0 File Offset: 0x000AACE0
		public override BaseState Update(Character iOwner, float iDeltaTime)
		{
			BaseState baseState = this.UpdateBloatDeath(iOwner, iDeltaTime);
			if (baseState != null)
			{
				return baseState;
			}
			if (iOwner is Avatar && iOwner.CastType != CastType.None)
			{
				return CastState.Instance;
			}
			if (iOwner.Attacking)
			{
				return AttackState.Instance;
			}
			if (iOwner.IsEntangled || iOwner.Gripper != null)
			{
				if (iOwner.CurrentAnimation != Animations.spec_entangled & iOwner.AnimationController.HasFinished)
				{
					iOwner.GoToAnimation(Animations.spec_entangled, 0.1f);
				}
				return null;
			}
			if (iOwner.CharacterBody.Movement.Length() < 1E-45f)
			{
				return IdleState.Instance;
			}
			return MoveState.Instance;
		}

		// Token: 0x060019B8 RID: 6584 RVA: 0x000ACB84 File Offset: 0x000AAD84
		public override void OnExit(Character iOwner)
		{
		}

		// Token: 0x04001BF3 RID: 7155
		private static EntangledState mSingelton;

		// Token: 0x04001BF4 RID: 7156
		private static volatile object mSingeltonLock = new object();
	}
}
