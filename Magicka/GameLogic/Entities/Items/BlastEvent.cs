using System;
using Magicka.GameLogic.Spells;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Magicka.GameLogic.Entities.Items
{
	// Token: 0x020004A3 RID: 1187
	public struct BlastEvent
	{
		// Token: 0x060023F5 RID: 9205 RVA: 0x001021D4 File Offset: 0x001003D4
		public BlastEvent(float iRadius, DamageCollection5 iDamage)
		{
			this.Radius = iRadius;
			this.Damage = iDamage;
		}

		// Token: 0x060023F6 RID: 9206 RVA: 0x001021E4 File Offset: 0x001003E4
		public BlastEvent(float iRadius, Damage iDamage)
		{
			this.Radius = iRadius;
			this.Damage = default(DamageCollection5);
			this.Damage.A = iDamage;
		}

		// Token: 0x060023F7 RID: 9207 RVA: 0x00102205 File Offset: 0x00100405
		public BlastEvent(ContentReader iInput)
		{
			throw new NotImplementedException();
		}

		// Token: 0x060023F8 RID: 9208 RVA: 0x0010220C File Offset: 0x0010040C
		public DamageResult Execute(Entity iItem, Entity iTarget, ref Vector3? iPosition)
		{
			Vector3 iPosition2 = iItem.Body.OldPosition;
			if (iPosition != null)
			{
				iPosition2 = iPosition.Value;
			}
			DamageResult damageResult = DamageResult.None;
			Entity entity;
			if (iItem is MissileEntity)
			{
				entity = (iItem as MissileEntity).Owner;
			}
			else if (iItem is Item)
			{
				entity = (iItem as Item).Owner;
			}
			else
			{
				entity = iItem;
			}
			return damageResult | Blast.FullBlast(iItem.PlayState, entity, entity.PlayState.PlayTime, iItem, this.Radius, iPosition2, this.Damage);
		}

		// Token: 0x040026FD RID: 9981
		public float Radius;

		// Token: 0x040026FE RID: 9982
		public DamageCollection5 Damage;
	}
}
