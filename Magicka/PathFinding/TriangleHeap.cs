using System;
using PolygonHead;

namespace Magicka.PathFinding
{
	// Token: 0x0200013B RID: 315
	public class TriangleHeap : IHeap<TriangleNode>
	{
		// Token: 0x170001C7 RID: 455
		// (get) Token: 0x060008E8 RID: 2280 RVA: 0x00038F5C File Offset: 0x0003715C
		public bool IsEmpty
		{
			get
			{
				return this.mLastIndex == 0;
			}
		}

		// Token: 0x060008E9 RID: 2281 RVA: 0x00038F67 File Offset: 0x00037167
		public TriangleHeap(int iSize)
		{
			this.mHeap = new TriangleNode[iSize + 1];
		}

		// Token: 0x060008EA RID: 2282 RVA: 0x00038F80 File Offset: 0x00037180
		protected void IncreasSize()
		{
			TriangleNode[] array = new TriangleNode[(this.mHeap.Length - 1) * 2 + 1];
			for (int i = 0; i < this.mHeap.Length; i++)
			{
				array[i] = this.mHeap[i];
			}
			this.mHeap = array;
		}

		// Token: 0x060008EB RID: 2283 RVA: 0x00038FD8 File Offset: 0x000371D8
		public void Push(TriangleNode iValue)
		{
			this.mLastIndex++;
			int num = this.mLastIndex;
			int num2 = num / 2;
			TriangleNode triangleNode = this.mHeap[num2];
			while (num > 1 && iValue.TotalCost < triangleNode.TotalCost)
			{
				this.mHeap[num] = triangleNode;
				num = num2;
				num2 = num / 2;
				triangleNode = this.mHeap[num2];
			}
			this.mHeap[num] = iValue;
			if (num == 0)
			{
				throw new Exception();
			}
		}

		// Token: 0x060008EC RID: 2284 RVA: 0x0003906C File Offset: 0x0003726C
		public TriangleNode Pop()
		{
			if (this.mLastIndex < 1)
			{
				throw new Exception("Heap is empty!");
			}
			TriangleNode result = this.mHeap[1];
			TriangleNode triangleNode = this.mHeap[this.mLastIndex];
			this.mLastIndex--;
			int num = 1;
			int num2 = 2;
			int num3 = 3;
			for (;;)
			{
				int num4 = 0;
				if (num2 <= this.mLastIndex)
				{
					float totalCost = this.mHeap[num2].TotalCost;
					if (totalCost < triangleNode.TotalCost)
					{
						num4 = num2;
					}
					float totalCost2 = this.mHeap[num3].TotalCost;
					if (num3 <= this.mLastIndex && totalCost2 < triangleNode.TotalCost && totalCost2 < totalCost)
					{
						num4 = num3;
					}
				}
				if (num4 < 1)
				{
					break;
				}
				this.mHeap[num] = this.mHeap[num4];
				num = num4;
				num2 = num * 2;
				num3 = num2 + 1;
			}
			this.mHeap[num] = triangleNode;
			return result;
		}

		// Token: 0x170001C8 RID: 456
		// (get) Token: 0x060008ED RID: 2285 RVA: 0x00039179 File Offset: 0x00037379
		public bool IsMinHeap
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060008EE RID: 2286 RVA: 0x0003917C File Offset: 0x0003737C
		public TriangleNode Peek()
		{
			return this.mHeap[1];
		}

		// Token: 0x060008EF RID: 2287 RVA: 0x0003918F File Offset: 0x0003738F
		public void Clear()
		{
			this.mLastIndex = 0;
		}

		// Token: 0x170001C9 RID: 457
		// (get) Token: 0x060008F0 RID: 2288 RVA: 0x00039198 File Offset: 0x00037398
		public int Count
		{
			get
			{
				return this.mLastIndex;
			}
		}

		// Token: 0x060008F1 RID: 2289 RVA: 0x000391A0 File Offset: 0x000373A0
		public bool Contains(TriangleNode iValue)
		{
			return this.Contains(iValue, 1);
		}

		// Token: 0x060008F2 RID: 2290 RVA: 0x000391AC File Offset: 0x000373AC
		private bool Contains(TriangleNode iValue, int iNode)
		{
			if (iNode > this.mLastIndex)
			{
				return false;
			}
			TriangleNode triangleNode = this.mHeap[iNode];
			return (iValue.TotalCost == triangleNode.TotalCost && iValue.Id == triangleNode.Id) || (iValue.TotalCost >= triangleNode.TotalCost && (this.Contains(iValue, iNode * 2) || this.Contains(iValue, iNode * 2 + 1)));
		}

		// Token: 0x04000840 RID: 2112
		protected TriangleNode[] mHeap;

		// Token: 0x04000841 RID: 2113
		protected int mLastIndex;
	}
}
