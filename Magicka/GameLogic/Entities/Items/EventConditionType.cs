using System;

namespace Magicka.GameLogic.Entities.Items
{
	// Token: 0x0200024D RID: 589
	[Flags]
	public enum EventConditionType : byte
	{
		// Token: 0x040010F4 RID: 4340
		Default = 1,
		// Token: 0x040010F5 RID: 4341
		Hit = 2,
		// Token: 0x040010F6 RID: 4342
		Collision = 4,
		// Token: 0x040010F7 RID: 4343
		Damaged = 8,
		// Token: 0x040010F8 RID: 4344
		Timer = 16,
		// Token: 0x040010F9 RID: 4345
		Death = 32,
		// Token: 0x040010FA RID: 4346
		OverKill = 64
	}
}
