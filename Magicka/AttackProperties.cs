using System;

namespace Magicka
{
	// Token: 0x0200026D RID: 621
	[Flags]
	public enum AttackProperties : short
	{
		// Token: 0x040012FA RID: 4858
		Damage = 1,
		// Token: 0x040012FB RID: 4859
		Knockdown = 2,
		// Token: 0x040012FC RID: 4860
		Pushed = 4,
		// Token: 0x040012FD RID: 4861
		Knockback = 6,
		// Token: 0x040012FE RID: 4862
		Piercing = 8,
		// Token: 0x040012FF RID: 4863
		ArmourPiercing = 16,
		// Token: 0x04001300 RID: 4864
		Status = 32,
		// Token: 0x04001301 RID: 4865
		Entanglement = 64,
		// Token: 0x04001302 RID: 4866
		Stun = 128,
		// Token: 0x04001303 RID: 4867
		Bleed = 256,
		// Token: 0x04001304 RID: 4868
		NumberOfTypes = 512
	}
}
