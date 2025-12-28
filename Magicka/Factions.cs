using System;

namespace Magicka
{
	// Token: 0x02000270 RID: 624
	[Flags]
	public enum Factions
	{
		// Token: 0x04001321 RID: 4897
		NONE = 0,
		// Token: 0x04001322 RID: 4898
		EVIL = 1,
		// Token: 0x04001323 RID: 4899
		WILD = 2,
		// Token: 0x04001324 RID: 4900
		FRIENDLY = 4,
		// Token: 0x04001325 RID: 4901
		DEMON = 8,
		// Token: 0x04001326 RID: 4902
		UNDEAD = 16,
		// Token: 0x04001327 RID: 4903
		HUMAN = 32,
		// Token: 0x04001328 RID: 4904
		WIZARD = 64,
		// Token: 0x04001329 RID: 4905
		NEUTRAL = 255,
		// Token: 0x0400132A RID: 4906
		PLAYER0 = 256,
		// Token: 0x0400132B RID: 4907
		PLAYER1 = 512,
		// Token: 0x0400132C RID: 4908
		PLAYER2 = 1024,
		// Token: 0x0400132D RID: 4909
		PLAYER3 = 2048,
		// Token: 0x0400132E RID: 4910
		TEAM_RED = 4096,
		// Token: 0x0400132F RID: 4911
		TEAM_BLUE = 8192,
		// Token: 0x04001330 RID: 4912
		PLAYERS = 16128,
		// Token: 0x04001331 RID: 4913
		NUMBER_OF_FACTIONS = 15
	}
}
