using System;

namespace Magicka.GameLogic.Entities.Buffs
{
	// Token: 0x020002FA RID: 762
	public enum AuraTarget : byte
	{
		// Token: 0x0400193B RID: 6459
		Friendly,
		// Token: 0x0400193C RID: 6460
		FriendlyButSelf,
		// Token: 0x0400193D RID: 6461
		Enemy,
		// Token: 0x0400193E RID: 6462
		All,
		// Token: 0x0400193F RID: 6463
		AllButSelf,
		// Token: 0x04001940 RID: 6464
		Self,
		// Token: 0x04001941 RID: 6465
		Type,
		// Token: 0x04001942 RID: 6466
		TypeButSelf,
		// Token: 0x04001943 RID: 6467
		Faction,
		// Token: 0x04001944 RID: 6468
		FactionButSelf
	}
}
