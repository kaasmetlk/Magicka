using System;

namespace Magicka.GameLogic
{
	// Token: 0x020004E1 RID: 1249
	public struct DamageCollection5
	{
		// Token: 0x06002516 RID: 9494 RVA: 0x0010C200 File Offset: 0x0010A400
		public void MultiplyMagnitude(float iMultiplier)
		{
			this.A.Magnitude = this.A.Magnitude * iMultiplier;
			this.B.Magnitude = this.B.Magnitude * iMultiplier;
			this.C.Magnitude = this.C.Magnitude * iMultiplier;
			this.D.Magnitude = this.D.Magnitude * iMultiplier;
			this.E.Magnitude = this.E.Magnitude * iMultiplier;
		}

		// Token: 0x06002517 RID: 9495 RVA: 0x0010C26C File Offset: 0x0010A46C
		public void AddDamage(Damage iDamage)
		{
			switch (this.count)
			{
			case 0:
				this.A = iDamage;
				break;
			case 1:
				this.B = iDamage;
				break;
			case 2:
				this.C = iDamage;
				break;
			case 3:
				this.D = iDamage;
				break;
			case 4:
				this.E = iDamage;
				break;
			default:
				throw new Exception("DamageCollection struct is full, pull up!");
			}
			this.count++;
		}

		// Token: 0x06002518 RID: 9496 RVA: 0x0010C2E2 File Offset: 0x0010A4E2
		public Elements GetAllElements()
		{
			return this.A.Element | this.B.Element | this.C.Element | this.D.Element | this.E.Element;
		}

		// Token: 0x06002519 RID: 9497 RVA: 0x0010C31F File Offset: 0x0010A51F
		public float GetTotalMagnitude()
		{
			return this.A.Magnitude + this.B.Magnitude + this.C.Magnitude + this.D.Magnitude + this.E.Magnitude;
		}

		// Token: 0x0600251A RID: 9498 RVA: 0x0010C35C File Offset: 0x0010A55C
		public unsafe static DamageCollection5 Combine(DamageCollection5 iA, DamageCollection5 iB)
		{
			DamageCollection5 result = default(DamageCollection5);
			Damage* ptr = &iA.A;
			Damage* ptr2 = &iB.A;
			Damage* ptr3 = &result.A;
			int num = 0;
			for (int i = 0; i < 5; i++)
			{
				Damage damage = ptr[i];
				if (damage.AttackProperty != (AttackProperties)0)
				{
					bool flag = false;
					for (int j = 0; j < 5; j++)
					{
						Damage damage2 = ptr2[j];
						if (damage.AttackProperty == damage2.AttackProperty & damage.Element == damage2.Element)
						{
							flag = true;
							ptr3[num].AttackProperty = damage.AttackProperty;
							ptr3[num].Element = damage.Element;
							ptr3[num].Amount = (damage.Amount * damage.Magnitude + damage2.Amount * damage2.Magnitude) / (damage.Magnitude + damage2.Magnitude);
							ptr3[num].Magnitude = damage.Magnitude + damage2.Magnitude;
							num++;
							ptr2[j].AttackProperty = (AttackProperties)0;
							break;
						}
					}
					if (!flag)
					{
						ptr3[(IntPtr)(num++) * (IntPtr)sizeof(Damage)] = damage;
					}
				}
			}
			int num2 = 0;
			while (num2 < 5 & num < 5)
			{
				if (ptr2[num2].AttackProperty != (AttackProperties)0)
				{
					ptr3[(IntPtr)(num++) * (IntPtr)sizeof(Damage)] = ptr2[num2];
				}
				num2++;
			}
			return result;
		}

		// Token: 0x04002877 RID: 10359
		public const int LENGTH = 5;

		// Token: 0x04002878 RID: 10360
		public Damage A;

		// Token: 0x04002879 RID: 10361
		public Damage B;

		// Token: 0x0400287A RID: 10362
		public Damage C;

		// Token: 0x0400287B RID: 10363
		public Damage D;

		// Token: 0x0400287C RID: 10364
		public Damage E;

		// Token: 0x0400287D RID: 10365
		private int count;
	}
}
