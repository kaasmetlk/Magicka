using System;

namespace Magicka
{
	// Token: 0x0200026C RID: 620
	[Flags]
	public enum Elements
	{
		// Token: 0x040012E0 RID: 4832
		None = 0,
		// Token: 0x040012E1 RID: 4833
		Earth = 1,
		// Token: 0x040012E2 RID: 4834
		Physical = 1,
		// Token: 0x040012E3 RID: 4835
		Water = 2,
		// Token: 0x040012E4 RID: 4836
		Cold = 4,
		// Token: 0x040012E5 RID: 4837
		Fire = 8,
		// Token: 0x040012E6 RID: 4838
		Lightning = 16,
		// Token: 0x040012E7 RID: 4839
		Arcane = 32,
		// Token: 0x040012E8 RID: 4840
		Life = 64,
		// Token: 0x040012E9 RID: 4841
		Shield = 128,
		// Token: 0x040012EA RID: 4842
		Ice = 256,
		// Token: 0x040012EB RID: 4843
		Steam = 512,
		// Token: 0x040012EC RID: 4844
		Poison = 1024,
		// Token: 0x040012ED RID: 4845
		Offensive = 65343,
		// Token: 0x040012EE RID: 4846
		Defensive = 176,
		// Token: 0x040012EF RID: 4847
		All = 65535,
		// Token: 0x040012F0 RID: 4848
		Magick = 65535,
		// Token: 0x040012F1 RID: 4849
		Basic = 255,
		// Token: 0x040012F2 RID: 4850
		Instant = 881,
		// Token: 0x040012F3 RID: 4851
		InstantPhysical = 369,
		// Token: 0x040012F4 RID: 4852
		InstantNonPhysical = 624,
		// Token: 0x040012F5 RID: 4853
		StatusEffects = 1614,
		// Token: 0x040012F6 RID: 4854
		ShieldElements = 224,
		// Token: 0x040012F7 RID: 4855
		PhysicalElements = 257,
		// Token: 0x040012F8 RID: 4856
		Beams = 96
	}
}
