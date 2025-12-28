using System;
using Microsoft.Xna.Framework.Content;

namespace Magicka.GameLogic.Entities.Buffs
{
	// Token: 0x02000353 RID: 851
	public struct BuffModifyHitPoints
	{
		// Token: 0x060019E6 RID: 6630 RVA: 0x000AD499 File Offset: 0x000AB699
		public BuffModifyHitPoints(float iHitPointsMultiplier, float iHitPointsModififier)
		{
			this.HitPointsMultiplier = iHitPointsMultiplier;
			this.HitPointsModifier = iHitPointsModififier;
		}

		// Token: 0x060019E7 RID: 6631 RVA: 0x000AD4A9 File Offset: 0x000AB6A9
		public BuffModifyHitPoints(ContentReader iInput)
		{
			this.HitPointsMultiplier = iInput.ReadSingle();
			this.HitPointsModifier = iInput.ReadSingle();
		}

		// Token: 0x060019E8 RID: 6632 RVA: 0x000AD4C3 File Offset: 0x000AB6C3
		public float Execute(Character iOwner)
		{
			iOwner.MaxHitPoints *= this.HitPointsMultiplier;
			iOwner.MaxHitPoints += this.HitPointsModifier;
			return 1f;
		}

		// Token: 0x04001C21 RID: 7201
		public float HitPointsMultiplier;

		// Token: 0x04001C22 RID: 7202
		public float HitPointsModifier;
	}
}
