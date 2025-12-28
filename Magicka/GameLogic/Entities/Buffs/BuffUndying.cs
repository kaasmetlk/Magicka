using System;
using Microsoft.Xna.Framework.Content;

namespace Magicka.GameLogic.Entities.Buffs
{
	// Token: 0x02000350 RID: 848
	public struct BuffUndying
	{
		// Token: 0x060019DE RID: 6622 RVA: 0x000AD444 File Offset: 0x000AB644
		public BuffUndying(ContentReader iInput)
		{
			this.Nada = 0;
		}

		// Token: 0x060019DF RID: 6623 RVA: 0x000AD44D File Offset: 0x000AB64D
		public float Execute(Character iOwner)
		{
			iOwner.Undying = true;
			return 1f;
		}

		// Token: 0x04001C1E RID: 7198
		public byte Nada;
	}
}
