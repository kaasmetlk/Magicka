using System;

namespace Magicka.AI.AgentStates
{
	// Token: 0x02000198 RID: 408
	public interface IAIState
	{
		// Token: 0x06000C2F RID: 3119
		void OnEnter(IAI iOwner);

		// Token: 0x06000C30 RID: 3120
		void OnExit(IAI iOwner);

		// Token: 0x06000C31 RID: 3121
		void OnExecute(IAI iOwner, float dTime);

		// Token: 0x06000C32 RID: 3122
		void IncrementEvent(IAI iOwner);
	}
}
