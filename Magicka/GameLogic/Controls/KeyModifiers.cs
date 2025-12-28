using System;

namespace Magicka.GameLogic.Controls
{
	// Token: 0x02000187 RID: 391
	[Flags]
	internal enum KeyModifiers
	{
		// Token: 0x04000AEA RID: 2794
		None = 0,
		// Token: 0x04000AEB RID: 2795
		LeftControl = 1,
		// Token: 0x04000AEC RID: 2796
		RightControl = 2,
		// Token: 0x04000AED RID: 2797
		Control = 3,
		// Token: 0x04000AEE RID: 2798
		LeftAlt = 4,
		// Token: 0x04000AEF RID: 2799
		RightAlt = 8,
		// Token: 0x04000AF0 RID: 2800
		Alt = 12,
		// Token: 0x04000AF1 RID: 2801
		LeftShift = 16,
		// Token: 0x04000AF2 RID: 2802
		RightShift = 32,
		// Token: 0x04000AF3 RID: 2803
		Shift = 48
	}
}
