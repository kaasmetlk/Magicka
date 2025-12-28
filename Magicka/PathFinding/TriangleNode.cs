using System;

namespace Magicka.PathFinding
{
	// Token: 0x0200013A RID: 314
	public struct TriangleNode
	{
		// Token: 0x170001C6 RID: 454
		// (get) Token: 0x060008E7 RID: 2279 RVA: 0x00038F4A File Offset: 0x0003714A
		public uint LongId
		{
			get
			{
				return (uint)((int)this.Mesh << 16 | (int)this.Id);
			}
		}

		// Token: 0x0400083B RID: 2107
		public float Cost;

		// Token: 0x0400083C RID: 2108
		public float TotalCost;

		// Token: 0x0400083D RID: 2109
		public ushort Mesh;

		// Token: 0x0400083E RID: 2110
		public ushort Id;

		// Token: 0x0400083F RID: 2111
		public uint ParentId;
	}
}
