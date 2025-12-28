using System;
using Microsoft.Xna.Framework.Content;

namespace Magicka.GameLogic.Entities.Buffs
{
	// Token: 0x0200034D RID: 845
	public struct BuffBoostDamage
	{
		// Token: 0x060019D5 RID: 6613 RVA: 0x000AD20F File Offset: 0x000AB40F
		public BuffBoostDamage(Damage iDamage)
		{
			this.Damage = iDamage;
			this.mAuraOwnerHandle = ushort.MaxValue;
		}

		// Token: 0x060019D6 RID: 6614 RVA: 0x000AD224 File Offset: 0x000AB424
		public BuffBoostDamage(ContentReader iInput)
		{
			this.Damage = default(Damage);
			this.Damage.AttackProperty = (AttackProperties)iInput.ReadInt32();
			this.Damage.Element = (Elements)iInput.ReadInt32();
			this.Damage.Amount = iInput.ReadSingle();
			this.Damage.Magnitude = iInput.ReadSingle();
			this.mAuraOwnerHandle = ushort.MaxValue;
		}

		// Token: 0x060019D7 RID: 6615 RVA: 0x000AD28D File Offset: 0x000AB48D
		public float Execute(Character iOwner)
		{
			return 1f;
		}

		// Token: 0x04001C17 RID: 7191
		public Damage Damage;

		// Token: 0x04001C18 RID: 7192
		public ushort mAuraOwnerHandle;
	}
}
