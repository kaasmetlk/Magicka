using System;

namespace Magicka.GameLogic.Entities.CharacterStates
{
	// Token: 0x0200053B RID: 1339
	public class StunState : BaseState
	{
		// Token: 0x17000952 RID: 2386
		// (get) Token: 0x060027D6 RID: 10198 RVA: 0x00123324 File Offset: 0x00121524
		public static StunState Instance
		{
			get
			{
				if (StunState.sSingelton == null)
				{
					lock (StunState.sSingeltonLock)
					{
						if (StunState.sSingelton == null)
						{
							StunState.sSingelton = new StunState();
						}
					}
				}
				return StunState.sSingelton;
			}
		}

		// Token: 0x060027D7 RID: 10199 RVA: 0x00123378 File Offset: 0x00121578
		public override void OnEnter(Character iOwner)
		{
			if (iOwner.HasAnimation(Animations.stunned))
			{
				iOwner.GoToAnimation(Animations.stunned, 0.35f);
			}
			else
			{
				iOwner.GoToAnimation(Animations.idle_wnd, 0.35f);
			}
			iOwner.CharacterBody.AllowMove = false;
			iOwner.CharacterBody.AllowRotate = false;
			base.OnEnter(iOwner);
		}

		// Token: 0x060027D8 RID: 10200 RVA: 0x001233D0 File Offset: 0x001215D0
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
			if (!iOwner.IsStunned)
			{
				return IdleState.Instance;
			}
			return null;
		}

		// Token: 0x060027D9 RID: 10201 RVA: 0x00123406 File Offset: 0x00121606
		public override void OnExit(Character iOwner)
		{
			iOwner.CharacterBody.AllowMove = true;
			iOwner.CharacterBody.AllowRotate = true;
			base.OnExit(iOwner);
		}

		// Token: 0x04002B6F RID: 11119
		private static StunState sSingelton;

		// Token: 0x04002B70 RID: 11120
		private static volatile object sSingeltonLock = new object();
	}
}
