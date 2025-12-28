using System;

namespace Magicka.GameLogic.Entities.CharacterStates
{
	// Token: 0x020001D0 RID: 464
	public class BlockState : BaseState
	{
		// Token: 0x17000414 RID: 1044
		// (get) Token: 0x06000FC6 RID: 4038 RVA: 0x00062158 File Offset: 0x00060358
		public static BlockState Instance
		{
			get
			{
				if (BlockState.mSingelton == null)
				{
					lock (BlockState.mSingeltonLock)
					{
						if (BlockState.mSingelton == null)
						{
							BlockState.mSingelton = new BlockState();
						}
					}
				}
				return BlockState.mSingelton;
			}
		}

		// Token: 0x06000FC7 RID: 4039 RVA: 0x000621AC File Offset: 0x000603AC
		public override void OnEnter(Character iOwner)
		{
			iOwner.GoToAnimation(Animations.block, 0.1f);
		}

		// Token: 0x06000FC8 RID: 4040 RVA: 0x000621BC File Offset: 0x000603BC
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
			if (iOwner.Attacking)
			{
				return AttackState.Instance;
			}
			if (iOwner.IsBlocking)
			{
				return null;
			}
			if (iOwner.CharacterBody.Movement.Length() < 1E-45f)
			{
				return IdleState.Instance;
			}
			return MoveState.Instance;
		}

		// Token: 0x04000E43 RID: 3651
		private static BlockState mSingelton;

		// Token: 0x04000E44 RID: 3652
		private static volatile object mSingeltonLock = new object();
	}
}
