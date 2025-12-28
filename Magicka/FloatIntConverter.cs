using System;
using System.Runtime.InteropServices;

namespace Magicka
{
	// Token: 0x0200025E RID: 606
	[StructLayout(LayoutKind.Explicit)]
	public struct FloatIntConverter
	{
		// Token: 0x0600129F RID: 4767 RVA: 0x000732D3 File Offset: 0x000714D3
		public FloatIntConverter(float _float)
		{
			this.Int = 0;
			this.Float = _float;
		}

		// Token: 0x060012A0 RID: 4768 RVA: 0x000732E3 File Offset: 0x000714E3
		public FloatIntConverter(int _int)
		{
			this.Float = 0f;
			this.Int = _int;
		}

		// Token: 0x04001165 RID: 4453
		[FieldOffset(0)]
		public float Float;

		// Token: 0x04001166 RID: 4454
		[FieldOffset(0)]
		public int Int;
	}
}
