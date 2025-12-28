using System;
using Microsoft.Xna.Framework.Content;

namespace Magicka.GameLogic.Entities.Buffs
{
	// Token: 0x02000355 RID: 853
	public struct BuffModifySpellRange
	{
		// Token: 0x060019EC RID: 6636 RVA: 0x000AD521 File Offset: 0x000AB721
		public BuffModifySpellRange(float iRangeMultiplier, float iRangeModififier)
		{
			this.RangeMultiplier = iRangeMultiplier;
			this.RangeModifier = iRangeModififier;
		}

		// Token: 0x060019ED RID: 6637 RVA: 0x000AD531 File Offset: 0x000AB731
		public BuffModifySpellRange(ContentReader iInput)
		{
			this.RangeMultiplier = iInput.ReadSingle();
			this.RangeModifier = iInput.ReadSingle();
		}

		// Token: 0x060019EE RID: 6638 RVA: 0x000AD54B File Offset: 0x000AB74B
		public float Execute(Character iOwner)
		{
			return 1f;
		}

		// Token: 0x04001C25 RID: 7205
		public float RangeMultiplier;

		// Token: 0x04001C26 RID: 7206
		public float RangeModifier;
	}
}
