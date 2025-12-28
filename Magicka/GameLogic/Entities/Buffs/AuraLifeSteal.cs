using System;
using Microsoft.Xna.Framework.Content;

namespace Magicka.GameLogic.Entities.Buffs
{
	// Token: 0x02000303 RID: 771
	public struct AuraLifeSteal
	{
		// Token: 0x060017AC RID: 6060 RVA: 0x0009BF90 File Offset: 0x0009A190
		public AuraLifeSteal(float iAmount)
		{
			this.Amount = iAmount;
		}

		// Token: 0x060017AD RID: 6061 RVA: 0x0009BF99 File Offset: 0x0009A199
		public AuraLifeSteal(ContentReader iInput)
		{
			this.Amount = iInput.ReadSingle();
		}

		// Token: 0x060017AE RID: 6062 RVA: 0x0009BFA7 File Offset: 0x0009A1A7
		public float Execute(Character iOwner, AuraTarget iAuraTarget, int iEffect, float iRadius)
		{
			return 666f;
		}

		// Token: 0x04001969 RID: 6505
		public float Amount;
	}
}
