using System;

namespace Magicka
{
	// Token: 0x02000261 RID: 609
	[Flags]
	internal enum ControllerDirection : byte
	{
		// Token: 0x0400117B RID: 4475
		Dead = 255,
		// Token: 0x0400117C RID: 4476
		Center = 0,
		// Token: 0x0400117D RID: 4477
		Right = 1,
		// Token: 0x0400117E RID: 4478
		Up = 2,
		// Token: 0x0400117F RID: 4479
		Left = 4,
		// Token: 0x04001180 RID: 4480
		Down = 8,
		// Token: 0x04001181 RID: 4481
		UpRight = 3,
		// Token: 0x04001182 RID: 4482
		UpLeft = 6,
		// Token: 0x04001183 RID: 4483
		DownRight = 9,
		// Token: 0x04001184 RID: 4484
		DownLeft = 12
	}
}
