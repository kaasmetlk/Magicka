using System;
using Microsoft.Xna.Framework;

namespace Magicka.PathFinding
{
	// Token: 0x0200027F RID: 639
	internal struct NeighbourInfo
	{
		// Token: 0x040014B1 RID: 5297
		public ushort Mesh;

		// Token: 0x040014B2 RID: 5298
		public ushort Triangle;

		// Token: 0x040014B3 RID: 5299
		public float Cost;

		// Token: 0x040014B4 RID: 5300
		public Vector3 EdgePos;
	}
}
