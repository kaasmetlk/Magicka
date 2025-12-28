using System;

namespace Magicka.GameLogic.Statistics
{
	// Token: 0x02000249 RID: 585
	public struct KillCount
	{
		// Token: 0x06001221 RID: 4641 RVA: 0x0006E809 File Offset: 0x0006CA09
		public KillCount(int iCount, float iTTL)
		{
			this.Count = iCount;
			this.TTL = iTTL;
		}

		// Token: 0x06001222 RID: 4642 RVA: 0x0006E819 File Offset: 0x0006CA19
		public KillCount(int iCount)
		{
			this.Count = iCount;
			this.TTL = 60f;
		}

		// Token: 0x06001223 RID: 4643 RVA: 0x0006E82D File Offset: 0x0006CA2D
		public KillCount(float iTTL)
		{
			this.Count = 1;
			this.TTL = iTTL;
		}

		// Token: 0x040010DF RID: 4319
		public float TTL;

		// Token: 0x040010E0 RID: 4320
		public int Count;
	}
}
