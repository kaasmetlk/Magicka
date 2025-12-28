using System;
using Microsoft.Xna.Framework.Content;

namespace Magicka.GameLogic.Entities.Buffs
{
	// Token: 0x02000352 RID: 850
	public struct BuffReduceAgro
	{
		// Token: 0x060019E3 RID: 6627 RVA: 0x000AD47A File Offset: 0x000AB67A
		public BuffReduceAgro(float iAmount)
		{
			this.Amount = iAmount;
		}

		// Token: 0x060019E4 RID: 6628 RVA: 0x000AD483 File Offset: 0x000AB683
		public BuffReduceAgro(ContentReader iInput)
		{
			this.Amount = iInput.ReadSingle();
		}

		// Token: 0x060019E5 RID: 6629 RVA: 0x000AD491 File Offset: 0x000AB691
		public float Execute(Character iOwner)
		{
			return this.Amount;
		}

		// Token: 0x04001C20 RID: 7200
		public float Amount;
	}
}
