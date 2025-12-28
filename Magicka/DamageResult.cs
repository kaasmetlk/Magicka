using System;

namespace Magicka
{
	// Token: 0x0200026F RID: 623
	[Flags]
	public enum DamageResult
	{
		// Token: 0x04001313 RID: 4883
		None = 0,
		// Token: 0x04001314 RID: 4884
		Damaged = 1,
		// Token: 0x04001315 RID: 4885
		Hit = 2,
		// Token: 0x04001316 RID: 4886
		Knockeddown = 4,
		// Token: 0x04001317 RID: 4887
		Knockedback = 8,
		// Token: 0x04001318 RID: 4888
		Pushed = 16,
		// Token: 0x04001319 RID: 4889
		Statusadded = 32,
		// Token: 0x0400131A RID: 4890
		Statusremoved = 64,
		// Token: 0x0400131B RID: 4891
		Healed = 128,
		// Token: 0x0400131C RID: 4892
		Deflected = 256,
		// Token: 0x0400131D RID: 4893
		Killed = 512,
		// Token: 0x0400131E RID: 4894
		Pierced = 1024,
		// Token: 0x0400131F RID: 4895
		OverKilled = 2048
	}
}
