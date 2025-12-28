using System;
using Microsoft.Xna.Framework.Content;

namespace Magicka.GameLogic.Entities.Buffs
{
	// Token: 0x02000351 RID: 849
	public struct BuffBoost
	{
		// Token: 0x060019E0 RID: 6624 RVA: 0x000AD45B File Offset: 0x000AB65B
		public BuffBoost(float iAmount)
		{
			this.Amount = iAmount;
		}

		// Token: 0x060019E1 RID: 6625 RVA: 0x000AD464 File Offset: 0x000AB664
		public BuffBoost(ContentReader iInput)
		{
			this.Amount = iInput.ReadSingle();
		}

		// Token: 0x060019E2 RID: 6626 RVA: 0x000AD472 File Offset: 0x000AB672
		public float Execute(Character iOwner)
		{
			return this.Amount;
		}

		// Token: 0x04001C1F RID: 7199
		public float Amount;
	}
}
