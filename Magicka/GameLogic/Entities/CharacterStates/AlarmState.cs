using System;
using Magicka.AI;
using XNAnimation.Controllers;

namespace Magicka.GameLogic.Entities.CharacterStates
{
	// Token: 0x02000349 RID: 841
	internal class AlarmState : BaseState
	{
		// Token: 0x1700066D RID: 1645
		// (get) Token: 0x060019BB RID: 6587 RVA: 0x000ACB9C File Offset: 0x000AAD9C
		public static AlarmState Instance
		{
			get
			{
				if (AlarmState.mSingelton == null)
				{
					lock (AlarmState.mSingeltonLock)
					{
						if (AlarmState.mSingelton == null)
						{
							AlarmState.mSingelton = new AlarmState();
						}
					}
				}
				return AlarmState.mSingelton;
			}
		}

		// Token: 0x060019BC RID: 6588 RVA: 0x000ACBF0 File Offset: 0x000AADF0
		public override void OnEnter(Character iOwner)
		{
			Agent ai = (iOwner as NonPlayerCharacter).AI;
			if (ai.AlertMode == AlertMode.Discover)
			{
				iOwner.GoToAnimation(Animations.spec_alert0, 0.2f);
				return;
			}
			iOwner.GoToAnimation(Animations.spec_alert1, 0.2f);
		}

		// Token: 0x060019BD RID: 6589 RVA: 0x000ACC34 File Offset: 0x000AAE34
		public override BaseState Update(Character iOwner, float iDeltaTime)
		{
			AnimationController animationController = iOwner.AnimationController;
			BaseState baseState = this.UpdateBloatDeath(iOwner, iDeltaTime);
			if (baseState != null)
			{
				return baseState;
			}
			if (iOwner.NextAttackAnimation != Animations.None || (!animationController.HasFinished && (!animationController.IsLooping || animationController.CrossFadeEnabled)))
			{
				return null;
			}
			if (iOwner.CharacterBody.Movement.Length() < 1E-45f)
			{
				return IdleState.Instance;
			}
			return MoveState.Instance;
		}

		// Token: 0x060019BE RID: 6590 RVA: 0x000ACC9D File Offset: 0x000AAE9D
		public override void OnExit(Character iOwner)
		{
			iOwner.IsAlerted = false;
		}

		// Token: 0x04001BF5 RID: 7157
		private static AlarmState mSingelton;

		// Token: 0x04001BF6 RID: 7158
		private static volatile object mSingeltonLock = new object();
	}
}
