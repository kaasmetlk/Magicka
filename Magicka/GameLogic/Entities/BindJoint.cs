using System;
using Microsoft.Xna.Framework;

namespace Magicka.GameLogic.Entities
{
	// Token: 0x02000162 RID: 354
	public struct BindJoint
	{
		// Token: 0x06000AA2 RID: 2722 RVA: 0x00040AF9 File Offset: 0x0003ECF9
		public BindJoint(int iIndex, Matrix iBindPose)
		{
			this.mBindPose = iBindPose;
			this.mIndex = iIndex;
		}

		// Token: 0x040009A0 RID: 2464
		public int mIndex;

		// Token: 0x040009A1 RID: 2465
		public Matrix mBindPose;
	}
}
