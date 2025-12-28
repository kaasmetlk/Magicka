using System;

namespace Magicka.GameLogic.Entities.CharacterStates
{
	// Token: 0x02000582 RID: 1410
	public class BloatState : BaseState
	{
		// Token: 0x170009EB RID: 2539
		// (get) Token: 0x06002A26 RID: 10790 RVA: 0x0014B974 File Offset: 0x00149B74
		public static BloatState Instance
		{
			get
			{
				if (BloatState.mSingelton == null)
				{
					lock (BloatState.mSingeltonLock)
					{
						if (BloatState.mSingelton == null)
						{
							BloatState.mSingelton = new BloatState();
						}
					}
				}
				return BloatState.mSingelton;
			}
		}

		// Token: 0x06002A27 RID: 10791 RVA: 0x0014B9C8 File Offset: 0x00149BC8
		public override BaseState Update(Character iOwner, float iDeltaTime)
		{
			if (iOwner.BloatKilled)
			{
				return DeadState.Instance;
			}
			return null;
		}

		// Token: 0x04002D93 RID: 11667
		private static BloatState mSingelton;

		// Token: 0x04002D94 RID: 11668
		private static volatile object mSingeltonLock = new object();
	}
}
