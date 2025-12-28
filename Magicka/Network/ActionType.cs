using System;

namespace Magicka.Network
{
	// Token: 0x0200013C RID: 316
	public enum ActionType : byte
	{
		// Token: 0x04000843 RID: 2115
		ConjureElement,
		// Token: 0x04000844 RID: 2116
		CastSpell,
		// Token: 0x04000845 RID: 2117
		Attack,
		// Token: 0x04000846 RID: 2118
		Block,
		// Token: 0x04000847 RID: 2119
		PickUp,
		// Token: 0x04000848 RID: 2120
		PickUpRequest,
		// Token: 0x04000849 RID: 2121
		Interact,
		// Token: 0x0400084A RID: 2122
		Boost,
		// Token: 0x0400084B RID: 2123
		BreakFree,
		// Token: 0x0400084C RID: 2124
		Grip,
		// Token: 0x0400084D RID: 2125
		GripAttack,
		// Token: 0x0400084E RID: 2126
		Entangle,
		// Token: 0x0400084F RID: 2127
		Release,
		// Token: 0x04000850 RID: 2128
		Dash,
		// Token: 0x04000851 RID: 2129
		Jump,
		// Token: 0x04000852 RID: 2130
		ItemSpecial,
		// Token: 0x04000853 RID: 2131
		Magick,
		// Token: 0x04000854 RID: 2132
		SelfShield,
		// Token: 0x04000855 RID: 2133
		EventAnimation,
		// Token: 0x04000856 RID: 2134
		EventComplete
	}
}
