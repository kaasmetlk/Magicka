using System;

namespace Magicka
{
	// Token: 0x02000274 RID: 628
	[Flags]
	public enum EntityFeatures : ushort
	{
		// Token: 0x0400137F RID: 4991
		None = 0,
		// Token: 0x04001380 RID: 4992
		Position = 1,
		// Token: 0x04001381 RID: 4993
		Direction = 2,
		// Token: 0x04001382 RID: 4994
		Velocity = 4,
		// Token: 0x04001383 RID: 4995
		Orientation = 8,
		// Token: 0x04001384 RID: 4996
		Character = 16,
		// Token: 0x04001385 RID: 4997
		Damageable = 32,
		// Token: 0x04001386 RID: 4998
		StatusEffected = 64,
		// Token: 0x04001387 RID: 4999
		GenericBool = 128,
		// Token: 0x04001388 RID: 5000
		GenericInt = 256,
		// Token: 0x04001389 RID: 5001
		GenericFloat = 512,
		// Token: 0x0400138A RID: 5002
		WanderAngle = 1024,
		// Token: 0x0400138B RID: 5003
		SelfShield = 2048,
		// Token: 0x0400138C RID: 5004
		Etherealized = 4096,
		// Token: 0x0400138D RID: 5005
		GenericUShort = 8192
	}
}
