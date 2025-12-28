using System;
using XNAnimation.Controllers;

namespace Magicka.GameLogic.Entities.CharacterStates
{
	// Token: 0x020002A7 RID: 679
	internal class DashState : BaseState
	{
		// Token: 0x17000542 RID: 1346
		// (get) Token: 0x06001482 RID: 5250 RVA: 0x0007F7E4 File Offset: 0x0007D9E4
		public static DashState Instance
		{
			get
			{
				if (DashState.sSingelton == null)
				{
					lock (DashState.sSingeltonLock)
					{
						if (DashState.sSingelton == null)
						{
							DashState.sSingelton = new DashState();
						}
					}
				}
				return DashState.sSingelton;
			}
		}

		// Token: 0x06001483 RID: 5251 RVA: 0x0007F838 File Offset: 0x0007DA38
		public override void OnEnter(Character iOwner)
		{
			iOwner.GoToAnimation(iOwner.NextDashAnimation, 0.2f);
		}

		// Token: 0x06001484 RID: 5252 RVA: 0x0007F84C File Offset: 0x0007DA4C
		public override BaseState Update(Character iOwner, float iDeltaTime)
		{
			BaseState baseState = this.UpdateBloatDeath(iOwner, iDeltaTime);
			if (baseState == null)
			{
				baseState = this.UpdateHit(iOwner, iDeltaTime);
			}
			if (baseState != null)
			{
				return baseState;
			}
			if (iOwner.IsGripping)
			{
				return GrippingState.Instance;
			}
			AnimationController animationController = iOwner.AnimationController;
			if (animationController.CrossFadeEnabled || !animationController.HasFinished)
			{
				return null;
			}
			if (iOwner.CharacterBody.Movement.Length() < 1E-45f)
			{
				return IdleState.Instance;
			}
			return MoveState.Instance;
		}

		// Token: 0x06001485 RID: 5253 RVA: 0x0007F8BF File Offset: 0x0007DABF
		public override void OnExit(Character iOwner)
		{
			iOwner.Dashing = false;
		}

		// Token: 0x040015DE RID: 5598
		private static DashState sSingelton;

		// Token: 0x040015DF RID: 5599
		private static volatile object sSingeltonLock = new object();
	}
}
