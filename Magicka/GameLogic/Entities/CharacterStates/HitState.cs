using System;

namespace Magicka.GameLogic.Entities.CharacterStates
{
	// Token: 0x020002F3 RID: 755
	public class HitState : BaseState
	{
		// Token: 0x170005E7 RID: 1511
		// (get) Token: 0x06001747 RID: 5959 RVA: 0x0009720C File Offset: 0x0009540C
		public static HitState Instance
		{
			get
			{
				if (HitState.mSingelton == null)
				{
					lock (HitState.mSingeltonLock)
					{
						if (HitState.mSingelton == null)
						{
							HitState.mSingelton = new HitState();
						}
					}
				}
				return HitState.mSingelton;
			}
		}

		// Token: 0x06001748 RID: 5960 RVA: 0x00097260 File Offset: 0x00095460
		public override void OnEnter(Character iOwner)
		{
			iOwner.CharacterBody.AllowRotate = false;
			iOwner.GoToAnimation(Animations.hit, 0.1f);
		}

		// Token: 0x06001749 RID: 5961 RVA: 0x0009727C File Offset: 0x0009547C
		public override BaseState Update(Character iOwner, float iDeltaTime)
		{
			BaseState baseState = base.UpdateBloatDeath(iOwner, iDeltaTime);
			if (baseState != null)
			{
				return baseState;
			}
			if (!(iOwner.AnimationController.HasFinished | iOwner.AnimationController.IsLooping | iOwner.HasStatus(StatusEffects.Frozen)))
			{
				return null;
			}
			if (iOwner.PreviousState is ChargeState)
			{
				iOwner.CastSpell(true, "");
			}
			if (iOwner.CharacterBody.Movement.Length() < 1E-45f)
			{
				return IdleState.Instance;
			}
			return MoveState.Instance;
		}

		// Token: 0x0600174A RID: 5962 RVA: 0x000972F8 File Offset: 0x000954F8
		public override void OnExit(Character iOwner)
		{
			iOwner.IsHit = false;
		}

		// Token: 0x040018C9 RID: 6345
		private static HitState mSingelton;

		// Token: 0x040018CA RID: 6346
		private static volatile object mSingeltonLock = new object();
	}
}
