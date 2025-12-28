using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Magicka.GameLogic.Entities.Items
{
	// Token: 0x0200049B RID: 1179
	public struct SplashEvent
	{
		// Token: 0x060023CD RID: 9165 RVA: 0x00101850 File Offset: 0x000FFA50
		public SplashEvent(AttackProperties iAProp, Elements iElements, int iAmount, float iMagnitude, float iRadius)
		{
			this.Damage = new Damage
			{
				AttackProperty = iAProp,
				Element = iElements,
				Amount = (float)iAmount,
				Magnitude = iMagnitude
			};
			this.Radius = iRadius;
		}

		// Token: 0x060023CE RID: 9166 RVA: 0x00101896 File Offset: 0x000FFA96
		public SplashEvent(Damage iDamage, float iRadius)
		{
			this.Damage = iDamage;
			this.Radius = iRadius;
		}

		// Token: 0x060023CF RID: 9167 RVA: 0x001018A8 File Offset: 0x000FFAA8
		public SplashEvent(ContentReader iInput)
		{
			this.Damage = new Damage
			{
				AttackProperty = (AttackProperties)iInput.ReadInt32(),
				Element = (Elements)iInput.ReadInt32(),
				Amount = (float)iInput.ReadInt32(),
				Magnitude = iInput.ReadSingle()
			};
			this.Radius = iInput.ReadSingle();
			if (this.Radius <= 1E-45f)
			{
				throw new Exception("WTH");
			}
		}

		// Token: 0x060023D0 RID: 9168 RVA: 0x00101920 File Offset: 0x000FFB20
		public DamageResult Execute(Entity iItem, Entity iTarget, ref Vector3? iPosition)
		{
			DamageResult damageResult = DamageResult.None;
			Vector3 vector = iItem.Position;
			if (iPosition != null)
			{
				vector = iPosition.Value;
			}
			if (iItem is MissileEntity)
			{
				Entity owner = (iItem as MissileEntity).Owner;
				damageResult |= Helper.CircleDamage(iItem.PlayState, owner, iItem.PlayState.PlayTime, iItem, ref vector, this.Radius, ref this.Damage);
			}
			else if (iItem is Item)
			{
				Entity owner = (iItem as Item).Owner;
				damageResult |= Helper.CircleDamage(iItem.PlayState, owner, iItem.PlayState.PlayTime, owner, ref vector, this.Radius, ref this.Damage);
			}
			else
			{
				damageResult |= Helper.CircleDamage(iItem.PlayState, iItem as Character, iItem.PlayState.PlayTime, iItem, ref vector, this.Radius, ref this.Damage);
			}
			return damageResult;
		}

		// Token: 0x040026E7 RID: 9959
		public float Radius;

		// Token: 0x040026E8 RID: 9960
		public Damage Damage;
	}
}
