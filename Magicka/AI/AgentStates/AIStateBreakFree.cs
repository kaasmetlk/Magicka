using System;

namespace Magicka.AI.AgentStates
{
	// Token: 0x02000201 RID: 513
	internal class AIStateBreakFree : IAIState
	{
		// Token: 0x17000434 RID: 1076
		// (get) Token: 0x060010BD RID: 4285 RVA: 0x00069711 File Offset: 0x00067911
		public static AIStateBreakFree Instance
		{
			get
			{
				if (AIStateBreakFree.sSingelton == null)
				{
					AIStateBreakFree.sSingelton = new AIStateBreakFree();
				}
				return AIStateBreakFree.sSingelton;
			}
		}

		// Token: 0x060010BE RID: 4286 RVA: 0x00069729 File Offset: 0x00067929
		public void OnEnter(IAI iOwner)
		{
		}

		// Token: 0x060010BF RID: 4287 RVA: 0x0006972B File Offset: 0x0006792B
		public void OnExit(IAI iOwner)
		{
		}

		// Token: 0x060010C0 RID: 4288 RVA: 0x0006972D File Offset: 0x0006792D
		public void IncrementEvent(IAI iOwner)
		{
		}

		// Token: 0x060010C1 RID: 4289 RVA: 0x00069730 File Offset: 0x00067930
		public void OnExecute(IAI iOwner, float dTime)
		{
			Agent agent = iOwner as Agent;
			if (agent == null)
			{
				throw new NotImplementedException();
			}
			iOwner.Owner.BreakFree();
			if (!iOwner.Owner.IsGripped & !iOwner.Owner.IsEntangled)
			{
				agent.PopState();
			}
		}

		// Token: 0x04000F60 RID: 3936
		private static AIStateBreakFree sSingelton;
	}
}
