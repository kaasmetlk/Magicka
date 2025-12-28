using System;
using Microsoft.Xna.Framework.Content;

namespace Magicka.GameLogic.Entities.Buffs
{
	// Token: 0x0200034E RID: 846
	public struct BuffDealDamage
	{
		// Token: 0x060019D8 RID: 6616 RVA: 0x000AD294 File Offset: 0x000AB494
		public BuffDealDamage(Damage iDamage)
		{
			this.Damage = iDamage;
			this.mBuffIntervall = 1f;
			this.mInternalIntervall = 1f;
			this.mAuraOwnerHandle = 65535;
		}

		// Token: 0x060019D9 RID: 6617 RVA: 0x000AD2C0 File Offset: 0x000AB4C0
		public BuffDealDamage(ContentReader iInput)
		{
			this.Damage = default(Damage);
			this.Damage.AttackProperty = (AttackProperties)iInput.ReadInt32();
			this.Damage.Element = (Elements)iInput.ReadInt32();
			this.Damage.Amount = iInput.ReadSingle();
			this.Damage.Magnitude = iInput.ReadSingle();
			this.mBuffIntervall = 1f;
			this.mInternalIntervall = 1f;
			this.mAuraOwnerHandle = 65535;
		}

		// Token: 0x060019DA RID: 6618 RVA: 0x000AD340 File Offset: 0x000AB540
		public float Execute(Character iOwner, float iDeltaTime)
		{
			this.mInternalIntervall -= iDeltaTime;
			if (this.mInternalIntervall <= 0f)
			{
				if (this.mAuraOwnerHandle != 65535)
				{
					Character iAttacker = (Character)Entity.GetFromHandle(this.mAuraOwnerHandle);
					iOwner.Damage(this.Damage, iAttacker, iOwner.PlayState.PlayTime, iOwner.Position);
				}
				else
				{
					iOwner.Damage(this.Damage, iOwner, iOwner.PlayState.PlayTime, iOwner.Position);
				}
				this.mInternalIntervall = this.mBuffIntervall;
			}
			return 1f;
		}

		// Token: 0x04001C19 RID: 7193
		private Damage Damage;

		// Token: 0x04001C1A RID: 7194
		private float mBuffIntervall;

		// Token: 0x04001C1B RID: 7195
		private float mInternalIntervall;

		// Token: 0x04001C1C RID: 7196
		public int mAuraOwnerHandle;
	}
}
