using System;

namespace Magicka
{
	// Token: 0x02000260 RID: 608
	[Flags]
	public enum MovementProperties
	{
		// Token: 0x04001174 RID: 4468
		Default = 0,
		// Token: 0x04001175 RID: 4469
		Water = 1,
		// Token: 0x04001176 RID: 4470
		Jump = 2,
		// Token: 0x04001177 RID: 4471
		Fly = 4,
		// Token: 0x04001178 RID: 4472
		Dynamic = 128,
		// Token: 0x04001179 RID: 4473
		All = 255
	}
}
