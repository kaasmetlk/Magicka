using System;

namespace Magicka.GameLogic.Entities.Bosses
{
	// Token: 0x020000F2 RID: 242
	public interface IBossState<T> where T : IBoss
	{
		// Token: 0x0600075D RID: 1885
		void OnEnter(T iOwner);

		// Token: 0x0600075E RID: 1886
		void OnUpdate(float iDeltaTime, T iOwner);

		// Token: 0x0600075F RID: 1887
		void OnExit(T iOwner);
	}
}
