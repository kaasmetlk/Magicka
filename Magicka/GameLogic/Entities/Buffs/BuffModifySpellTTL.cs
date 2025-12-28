using System;
using Microsoft.Xna.Framework.Content;

namespace Magicka.GameLogic.Entities.Buffs
{
	// Token: 0x02000354 RID: 852
	public struct BuffModifySpellTTL
	{
		// Token: 0x060019E9 RID: 6633 RVA: 0x000AD4F0 File Offset: 0x000AB6F0
		public BuffModifySpellTTL(float iTTLMultiplier, float iTTLModififier)
		{
			this.TTLMultiplier = iTTLMultiplier;
			this.TTLModifier = iTTLModififier;
		}

		// Token: 0x060019EA RID: 6634 RVA: 0x000AD500 File Offset: 0x000AB700
		public BuffModifySpellTTL(ContentReader iInput)
		{
			this.TTLMultiplier = iInput.ReadSingle();
			this.TTLModifier = iInput.ReadSingle();
		}

		// Token: 0x060019EB RID: 6635 RVA: 0x000AD51A File Offset: 0x000AB71A
		public float Execute(Character iOwner)
		{
			return 1f;
		}

		// Token: 0x04001C23 RID: 7203
		public float TTLMultiplier;

		// Token: 0x04001C24 RID: 7204
		public float TTLModifier;
	}
}
