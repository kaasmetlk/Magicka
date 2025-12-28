using System;

namespace Magicka
{
	// Token: 0x0200026E RID: 622
	[Flags]
	public enum StatusEffects : short
	{
		// Token: 0x04001306 RID: 4870
		None = 0,
		// Token: 0x04001307 RID: 4871
		Burning = 1,
		// Token: 0x04001308 RID: 4872
		Wet = 2,
		// Token: 0x04001309 RID: 4873
		Frozen = 4,
		// Token: 0x0400130A RID: 4874
		Cold = 8,
		// Token: 0x0400130B RID: 4875
		Poisoned = 16,
		// Token: 0x0400130C RID: 4876
		Healing = 32,
		// Token: 0x0400130D RID: 4877
		Life = 32,
		// Token: 0x0400130E RID: 4878
		Greased = 64,
		// Token: 0x0400130F RID: 4879
		Steamed = 128,
		// Token: 0x04001310 RID: 4880
		Bleeding = 256,
		// Token: 0x04001311 RID: 4881
		NumberOfTypes = 512
	}
}
