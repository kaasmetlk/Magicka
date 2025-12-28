using System;

namespace Magicka.GameLogic.Entities.CharacterStates
{
	// Token: 0x02000583 RID: 1411
	internal class BusyState : BaseState
	{
		// Token: 0x170009EC RID: 2540
		// (get) Token: 0x06002A2A RID: 10794 RVA: 0x0014B9F0 File Offset: 0x00149BF0
		public static BusyState Instance
		{
			get
			{
				if (BusyState.sSingelton == null)
				{
					lock (BusyState.sSingeltonLock)
					{
						if (BusyState.sSingelton == null)
						{
							BusyState.sSingelton = new BusyState();
						}
					}
				}
				return BusyState.sSingelton;
			}
		}

		// Token: 0x06002A2B RID: 10795 RVA: 0x0014BA44 File Offset: 0x00149C44
		private BusyState()
		{
		}

		// Token: 0x06002A2C RID: 10796 RVA: 0x0014BA4C File Offset: 0x00149C4C
		public override void OnEnter(Character iOwner)
		{
			iOwner.CharacterBody.AllowMove = false;
			iOwner.CharacterBody.AllowRotate = false;
			iOwner.GoToAnimation(Animations.emote_confused0, 0.2f);
		}

		// Token: 0x06002A2D RID: 10797 RVA: 0x0014BA78 File Offset: 0x00149C78
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
			if (!iOwner.PlayState.Inventory.Active)
			{
				return IdleState.Instance;
			}
			return null;
		}

		// Token: 0x06002A2E RID: 10798 RVA: 0x0014BAB8 File Offset: 0x00149CB8
		public override void OnExit(Character iOwner)
		{
			iOwner.PlayState.Inventory.Close(iOwner);
			iOwner.CharacterBody.AllowMove = true;
			iOwner.CharacterBody.AllowRotate = true;
		}

		// Token: 0x04002D95 RID: 11669
		private static BusyState sSingelton;

		// Token: 0x04002D96 RID: 11670
		private static volatile object sSingeltonLock = new object();
	}
}
