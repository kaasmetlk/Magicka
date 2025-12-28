using System;

namespace Magicka.GameLogic.Entities.CharacterStates
{
	// Token: 0x0200064C RID: 1612
	public class DropState : BaseState
	{
		// Token: 0x17000B8D RID: 2957
		// (get) Token: 0x06003121 RID: 12577 RVA: 0x001940EC File Offset: 0x001922EC
		public static DropState Instance
		{
			get
			{
				if (DropState.sSingelton == null)
				{
					lock (DropState.sSingeltonLock)
					{
						if (DropState.sSingelton == null)
						{
							DropState.sSingelton = new DropState();
						}
					}
				}
				return DropState.sSingelton;
			}
		}

		// Token: 0x06003122 RID: 12578 RVA: 0x00194140 File Offset: 0x00192340
		public override void OnEnter(Character iOwner)
		{
			if (iOwner.DropAnimation != Animations.None)
			{
				iOwner.GoToAnimation(iOwner.DropAnimation, 0.1f);
			}
			else
			{
				iOwner.GoToAnimation(Animations.hit, 0.1f);
			}
			iOwner.CharacterBody.AllowMove = false;
			iOwner.CharacterBody.AllowRotate = false;
		}

		// Token: 0x06003123 RID: 12579 RVA: 0x00194190 File Offset: 0x00192390
		public override BaseState Update(Character iOwner, float iDeltaTime)
		{
			iOwner.Attacking = false;
			BaseState baseState = this.UpdateBloatDeath(iOwner, iDeltaTime);
			if (baseState != null)
			{
				return baseState;
			}
			if ((!iOwner.AnimationController.HasFinished || iOwner.AnimationController.CrossFadeEnabled) && !iOwner.AnimationController.IsLooping)
			{
				return null;
			}
			if (iOwner.CharacterBody.Movement.Length() < 1E-45f)
			{
				return IdleState.Instance;
			}
			return MoveState.Instance;
		}

		// Token: 0x04003550 RID: 13648
		private static DropState sSingelton;

		// Token: 0x04003551 RID: 13649
		private static volatile object sSingeltonLock = new object();
	}
}
