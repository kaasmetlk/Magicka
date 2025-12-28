using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Magicka.PathFinding
{
	// Token: 0x02000098 RID: 152
	internal struct Triangle
	{
		// Token: 0x0600047B RID: 1147 RVA: 0x00016488 File Offset: 0x00014688
		public Triangle(ContentReader iInput)
		{
			this.VertexA = iInput.ReadUInt16();
			this.VertexB = iInput.ReadUInt16();
			this.VertexC = iInput.ReadUInt16();
			this.NeighbourA = iInput.ReadUInt16();
			this.NeighbourB = iInput.ReadUInt16();
			this.NeighbourC = iInput.ReadUInt16();
			this.CostAB = iInput.ReadSingle();
			this.CostBC = iInput.ReadSingle();
			this.CostCA = iInput.ReadSingle();
			this.Properties = (MovementProperties)iInput.ReadByte();
		}

		// Token: 0x0600047C RID: 1148 RVA: 0x00016510 File Offset: 0x00014710
		public float GetCostFrom(ushort iParent, byte iTargetEdge)
		{
			switch (iTargetEdge)
			{
			case 0:
				if (iParent == this.NeighbourB)
				{
					return this.CostAB;
				}
				if (iParent == this.NeighbourC)
				{
					return this.CostCA;
				}
				break;
			case 1:
				if (iParent == this.NeighbourA)
				{
					return this.CostAB;
				}
				if (iParent == this.NeighbourC)
				{
					return this.CostBC;
				}
				break;
			case 2:
				if (iParent == this.NeighbourA)
				{
					return this.CostCA;
				}
				if (iParent == this.NeighbourB)
				{
					return this.CostBC;
				}
				break;
			default:
				throw new ArgumentException("Invalid edge index! Must be >= 0 and < 3");
			}
			throw new Exception("iParent and iTargetEdge refers to the same edge!");
		}

		// Token: 0x0600047D RID: 1149 RVA: 0x000165A8 File Offset: 0x000147A8
		internal void GetPortalPoints(Vector3[] iVertices, ushort iNeighbour, out Vector3 oLeft, out Vector3 oRight)
		{
			if (iNeighbour == this.NeighbourA)
			{
				oRight = iVertices[(int)this.VertexA];
				oLeft = iVertices[(int)this.VertexB];
				return;
			}
			if (iNeighbour == this.NeighbourB)
			{
				oRight = iVertices[(int)this.VertexB];
				oLeft = iVertices[(int)this.VertexC];
				return;
			}
			if (iNeighbour == this.NeighbourC)
			{
				oRight = iVertices[(int)this.VertexC];
				oLeft = iVertices[(int)this.VertexA];
				return;
			}
			throw new Exception("Invalid neighbour!");
		}

		// Token: 0x0600047E RID: 1150 RVA: 0x0001666A File Offset: 0x0001486A
		public void GetCenter(Vector3[] iVertices, out Vector3 oCenter)
		{
			Vector3.Add(ref iVertices[(int)this.VertexA], ref iVertices[(int)this.VertexB], out oCenter);
			Vector3.Add(ref iVertices[(int)this.VertexC], ref oCenter, out oCenter);
			Vector3.Multiply(ref oCenter, 0.333333f, out oCenter);
		}

		// Token: 0x040002F6 RID: 758
		public ushort NeighbourA;

		// Token: 0x040002F7 RID: 759
		public ushort NeighbourB;

		// Token: 0x040002F8 RID: 760
		public ushort NeighbourC;

		// Token: 0x040002F9 RID: 761
		public ushort VertexA;

		// Token: 0x040002FA RID: 762
		public ushort VertexB;

		// Token: 0x040002FB RID: 763
		public ushort VertexC;

		// Token: 0x040002FC RID: 764
		public float CostAB;

		// Token: 0x040002FD RID: 765
		public float CostBC;

		// Token: 0x040002FE RID: 766
		public float CostCA;

		// Token: 0x040002FF RID: 767
		public MovementProperties Properties;
	}
}
