using System;
using Microsoft.Xna.Framework.Content;

namespace Magicka.GameLogic.Entities.Buffs
{
	// Token: 0x0200034F RID: 847
	public struct BuffResistance
	{
		// Token: 0x060019DB RID: 6619 RVA: 0x000AD3D7 File Offset: 0x000AB5D7
		public BuffResistance(Resistance iResistance)
		{
			this.Resistance = iResistance;
		}

		// Token: 0x060019DC RID: 6620 RVA: 0x000AD3E0 File Offset: 0x000AB5E0
		public BuffResistance(ContentReader iInput)
		{
			this.Resistance = default(Resistance);
			this.Resistance.ResistanceAgainst = (Elements)iInput.ReadInt32();
			this.Resistance.Modifier = iInput.ReadSingle();
			this.Resistance.Multiplier = iInput.ReadSingle();
			this.Resistance.StatusResistance = iInput.ReadBoolean();
		}

		// Token: 0x060019DD RID: 6621 RVA: 0x000AD43D File Offset: 0x000AB63D
		public float Execute(Character iOwner)
		{
			return 1f;
		}

		// Token: 0x04001C1D RID: 7197
		public Resistance Resistance;
	}
}
