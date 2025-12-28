using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Magicka.Network
{
	// Token: 0x02000207 RID: 519
	internal struct AuthenticationToken
	{
		// Token: 0x04000FAD RID: 4013
		public const int MAX_SIZE = 1024;

		// Token: 0x04000FAE RID: 4014
		[FixedBuffer(typeof(byte), 1024)]
		public AuthenticationToken.<Data>e__FixedBuffer2 Data;

		// Token: 0x04000FAF RID: 4015
		public int Length;

		// Token: 0x02000208 RID: 520
		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 1024)]
		public struct <Data>e__FixedBuffer2
		{
			// Token: 0x04000FB0 RID: 4016
			public byte FixedElementField;
		}
	}
}
