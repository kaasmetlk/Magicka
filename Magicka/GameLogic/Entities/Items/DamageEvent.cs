using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Magicka.GameLogic.Entities.Items
{
	// Token: 0x02000499 RID: 1177
	public struct DamageEvent
	{
		// Token: 0x060023C1 RID: 9153 RVA: 0x0010142E File Offset: 0x000FF62E
		public DamageEvent(AttackProperties iAProp, Elements iElements, float iAmount, float iMagnitude)
		{
			this = new DamageEvent(iAProp, iElements, iAmount, iMagnitude, false);
		}

		// Token: 0x060023C2 RID: 9154 RVA: 0x0010143C File Offset: 0x000FF63C
		public DamageEvent(AttackProperties iAProp, Elements iElements, float iAmount, float iMagnitude, bool iVelocityBased)
		{
			this.Damage = new Damage
			{
				AttackProperty = iAProp,
				Element = iElements,
				Amount = iAmount,
				Magnitude = iMagnitude
			};
			this.VelocityBased = iVelocityBased;
		}

		// Token: 0x060023C3 RID: 9155 RVA: 0x00101481 File Offset: 0x000FF681
		public DamageEvent(Damage iDamage)
		{
			this = new DamageEvent(iDamage, false);
		}

		// Token: 0x060023C4 RID: 9156 RVA: 0x0010148B File Offset: 0x000FF68B
		public DamageEvent(Damage iDamage, bool iVelocityBased)
		{
			this.VelocityBased = iVelocityBased;
			this.Damage = iDamage;
		}

		// Token: 0x060023C5 RID: 9157 RVA: 0x0010149C File Offset: 0x000FF69C
		public DamageEvent(ContentReader iInput)
		{
			Damage damage = default(Damage);
			damage.AttackProperty = (AttackProperties)iInput.ReadInt32();
			damage.Element = (Elements)iInput.ReadInt32();
			damage.Amount = iInput.ReadSingle();
			damage.Magnitude = iInput.ReadSingle();
			this.VelocityBased = iInput.ReadBoolean();
			this.Damage = damage;
		}

		// Token: 0x060023C6 RID: 9158 RVA: 0x001014FC File Offset: 0x000FF6FC
		public DamageResult Execute(Entity iItem, Entity iTarget)
		{
			DamageResult damageResult = DamageResult.None;
			if (iTarget is IDamageable)
			{
				Damage damage = this.Damage;
				Vector3 position = iItem.Position;
				Item item = iItem as Item;
				if (item != null && item.Owner != null && item.IsGunClass && iTarget != null)
				{
					position = iTarget.Position;
					Vector3 position2 = iItem.Position;
					Vector3.Subtract(ref position2, ref position, out position2);
					position2.Normalize();
					Vector3.Multiply(ref position2, iTarget.Radius, out position2);
					Vector3.Add(ref position, ref position2, out position);
				}
				Entity entity;
				if (iItem is MissileEntity)
				{
					entity = (iItem as MissileEntity).Owner;
				}
				else if (iItem is Item)
				{
					entity = (iItem as Item).Owner;
					if ((iItem as Item).Owner.MeleeBoosted)
					{
						damage.Amount *= (iItem as Item).Owner.MeleeBoostAmount;
					}
				}
				else
				{
					entity = iItem;
				}
				if (this.VelocityBased)
				{
					float normalizedVelocity = (iItem as MissileEntity).NormalizedVelocity;
					damage.Amount *= normalizedVelocity;
				}
				damageResult |= (iTarget as IDamageable).Damage(damage, entity, entity.PlayState.PlayTime, position);
			}
			return damageResult;
		}

		// Token: 0x040026E3 RID: 9955
		public Damage Damage;

		// Token: 0x040026E4 RID: 9956
		public bool VelocityBased;
	}
}
