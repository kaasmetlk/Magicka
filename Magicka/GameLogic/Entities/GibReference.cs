using System;
using Microsoft.Xna.Framework.Graphics;

namespace Magicka.GameLogic.Entities
{
	// Token: 0x02000165 RID: 357
	public struct GibReference
	{
		// Token: 0x06000AA9 RID: 2729 RVA: 0x00040BB8 File Offset: 0x0003EDB8
		public GibReference(Model iModel, float iMass, float iScale)
		{
			this.mModel = iModel;
			this.mMass = iMass;
			this.mScale = iScale;
		}

		// Token: 0x040009AB RID: 2475
		public Model mModel;

		// Token: 0x040009AC RID: 2476
		public float mMass;

		// Token: 0x040009AD RID: 2477
		public float mScale;
	}
}
