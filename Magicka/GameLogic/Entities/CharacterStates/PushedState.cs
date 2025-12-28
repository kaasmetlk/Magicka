using System;

namespace Magicka.GameLogic.Entities.CharacterStates
{
	// Token: 0x020004AC RID: 1196
	public class PushedState : BaseState
	{
		// Token: 0x17000883 RID: 2179
		// (get) Token: 0x0600241B RID: 9243 RVA: 0x0010303C File Offset: 0x0010123C
		public static PushedState Instance
		{
			get
			{
				if (PushedState.mSingelton == null)
				{
					lock (PushedState.mSingeltonLock)
					{
						if (PushedState.mSingelton == null)
						{
							PushedState.mSingelton = new PushedState();
						}
					}
				}
				return PushedState.mSingelton;
			}
		}

		// Token: 0x0600241C RID: 9244 RVA: 0x00103090 File Offset: 0x00101290
		public override void OnEnter(Character iOwner)
		{
			iOwner.GoToAnimation(Animations.hit_slide, 0.05f);
			iOwner.ReleaseAttachedCharacter();
		}

		// Token: 0x0600241D RID: 9245 RVA: 0x001030A8 File Offset: 0x001012A8
		public override BaseState Update(Character iOwner, float iDeltaTime)
		{
			BaseState baseState = this.UpdateBloatDeath(iOwner, iDeltaTime);
			if (baseState == null)
			{
				baseState = this.UpdateHit(iOwner, iDeltaTime);
			}
			if (baseState != null && !(baseState is FlyingState))
			{
				return baseState;
			}
			if (iOwner.CharacterBody.IsPushed)
			{
				return null;
			}
			if (iOwner.CharacterBody.Movement.Length() < 1E-45f)
			{
				return IdleState.Instance;
			}
			return MoveState.Instance;
		}

		// Token: 0x0400271A RID: 10010
		private static PushedState mSingelton;

		// Token: 0x0400271B RID: 10011
		private static volatile object mSingeltonLock = new object();
	}
}
