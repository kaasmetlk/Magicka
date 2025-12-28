using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Magicka.GameLogic.Entities.Items
{
	// Token: 0x0200049A RID: 1178
	public struct DamageOwnerEvent
	{
		// Token: 0x060023C7 RID: 9159 RVA: 0x00101625 File Offset: 0x000FF825
		public DamageOwnerEvent(AttackProperties iAProp, Elements iElements, float iAmount, float iMagnitude)
		{
			this = new DamageOwnerEvent(iAProp, iElements, iAmount, iMagnitude, false);
		}

		// Token: 0x060023C8 RID: 9160 RVA: 0x00101634 File Offset: 0x000FF834
		public DamageOwnerEvent(AttackProperties iAProp, Elements iElements, float iAmount, float iMagnitude, bool iVelocityBased)
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

		// Token: 0x060023C9 RID: 9161 RVA: 0x00101679 File Offset: 0x000FF879
		public DamageOwnerEvent(Damage iDamage)
		{
			this = new DamageOwnerEvent(iDamage, false);
		}

		// Token: 0x060023CA RID: 9162 RVA: 0x00101683 File Offset: 0x000FF883
		public DamageOwnerEvent(Damage iDamage, bool iVelocityBased)
		{
			this.VelocityBased = iVelocityBased;
			this.Damage = iDamage;
		}

		// Token: 0x060023CB RID: 9163 RVA: 0x00101694 File Offset: 0x000FF894
		public DamageOwnerEvent(ContentReader iInput)
		{
			Damage damage = default(Damage);
			damage.AttackProperty = (AttackProperties)iInput.ReadInt32();
			damage.Element = (Elements)iInput.ReadInt32();
			damage.Amount = iInput.ReadSingle();
			damage.Magnitude = iInput.ReadSingle();
			this.VelocityBased = iInput.ReadBoolean();
			this.Damage = damage;
		}

		// Token: 0x060023CC RID: 9164 RVA: 0x001016F4 File Offset: 0x000FF8F4
		public DamageResult Execute(Entity iItem, Entity iTarget)
		{
			DamageResult damageResult = DamageResult.None;
			if (iItem is Item)
			{
				iTarget = (iItem as Item).Owner;
			}
			else if (iItem is MissileEntity)
			{
				iTarget = (iItem as MissileEntity).Owner;
			}
			else
			{
				iTarget = iItem;
			}
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

		// Token: 0x040026E5 RID: 9957
		public Damage Damage;

		// Token: 0x040026E6 RID: 9958
		public bool VelocityBased;
	}
}
