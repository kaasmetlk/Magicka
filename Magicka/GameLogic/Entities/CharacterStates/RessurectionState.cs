using System;

namespace Magicka.GameLogic.Entities.CharacterStates
{
	// Token: 0x0200057C RID: 1404
	internal class RessurectionState : BaseState
	{
		// Token: 0x170009E6 RID: 2534
		// (get) Token: 0x06002A0D RID: 10765 RVA: 0x0014B12C File Offset: 0x0014932C
		public static RessurectionState Instance
		{
			get
			{
				if (RessurectionState.mSingelton == null)
				{
					lock (RessurectionState.mSingeltonLock)
					{
						if (RessurectionState.mSingelton == null)
						{
							RessurectionState.mSingelton = new RessurectionState();
						}
					}
				}
				return RessurectionState.mSingelton;
			}
		}

		// Token: 0x06002A0E RID: 10766 RVA: 0x0014B180 File Offset: 0x00149380
		public override void OnEnter(Character iOwner)
		{
			iOwner.CharacterBody.AllowMove = false;
			iOwner.CharacterBody.AllowRotate = false;
			iOwner.ForceAnimation(iOwner.SpawnAnimation);
		}

		// Token: 0x06002A0F RID: 10767 RVA: 0x0014B1A8 File Offset: 0x001493A8
		public override BaseState Update(Character iOwner, float iDeltaTime)
		{
			if (iOwner.Bloating)
			{
				return BloatState.Instance;
			}
			if (iOwner.Overkilled && !iOwner.CannotDieWithoutExplicitKill)
			{
				return DeadState.Instance;
			}
			if ((!iOwner.AnimationController.HasFinished || iOwner.AnimationController.CrossFadeEnabled) && !iOwner.AnimationController.IsLooping)
			{
				return null;
			}
			if (iOwner.HitPoints <= 0f)
			{
				return DeadState.Instance;
			}
			if (iOwner.CharacterBody.Movement.Length() < 1E-45f)
			{
				return IdleState.Instance;
			}
			return MoveState.Instance;
		}

		// Token: 0x06002A10 RID: 10768 RVA: 0x0014B23A File Offset: 0x0014943A
		public override void OnExit(Character iOwner)
		{
			iOwner.CharacterBody.AllowRotate = true;
		}

		// Token: 0x04002D7F RID: 11647
		private static RessurectionState mSingelton;

		// Token: 0x04002D80 RID: 11648
		private static volatile object mSingeltonLock = new object();
	}
}
