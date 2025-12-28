using System;

namespace Magicka.Audio
{
	// Token: 0x02000043 RID: 67
	[Flags]
	public enum Banks : ushort
	{
		// Token: 0x04000223 RID: 547
		WaveBank = 1,
		// Token: 0x04000224 RID: 548
		Music = 2,
		// Token: 0x04000225 RID: 549
		Ambience = 4,
		// Token: 0x04000226 RID: 550
		UI = 8,
		// Token: 0x04000227 RID: 551
		Spells = 16,
		// Token: 0x04000228 RID: 552
		Characters = 32,
		// Token: 0x04000229 RID: 553
		Footsteps = 64,
		// Token: 0x0400022A RID: 554
		Weapons = 128,
		// Token: 0x0400022B RID: 555
		Misc = 256,
		// Token: 0x0400022C RID: 556
		Additional = 512,
		// Token: 0x0400022D RID: 557
		AdditionalMusic = 1024
	}
}
