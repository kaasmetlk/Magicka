using System;
using System.Collections.Generic;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.Buffs;

namespace Magicka.GameLogic
{
	// Token: 0x020004E2 RID: 1250
	public struct Damage
	{
		// Token: 0x0600251B RID: 9499 RVA: 0x0010C532 File Offset: 0x0010A732
		public Damage(AttackProperties iAttackProperty, Elements iElement, float iAmount, float iMagnitude)
		{
			this.AttackProperty = iAttackProperty;
			this.Element = iElement;
			this.Amount = iAmount;
			this.Magnitude = iMagnitude;
		}

		// Token: 0x0600251C RID: 9500 RVA: 0x0010C554 File Offset: 0x0010A754
		public static void Add(ref Damage iA, ref Damage iB, out Damage oResult)
		{
			oResult.AttackProperty = (iA.AttackProperty | iB.AttackProperty);
			oResult.Element = (iA.Element | iB.Element);
			oResult.Amount = (iA.Amount * iA.Magnitude + iB.Amount * iB.Magnitude) / (iA.Magnitude + iB.Magnitude);
			oResult.Magnitude = iA.Magnitude + iB.Magnitude;
		}

		// Token: 0x0600251D RID: 9501 RVA: 0x0010C5CC File Offset: 0x0010A7CC
		public void ApplyResistances(Resistance[] iResistances, Resistance[] iAdditionalResistances, IList<BuffStorage> iBuffs, StatusEffects iStatusEffects)
		{
			bool flag = (short)(this.AttackProperty & AttackProperties.ArmourPiercing) != 0;
			bool flag2 = false;
			bool flag3 = false;
			float num = 1f;
			if (iResistances != null)
			{
				for (int i = 0; i < iResistances.Length; i++)
				{
					Elements elements = (Elements)(1 << i);
					if ((elements & this.Element) != Elements.None)
					{
						float multiplier = iResistances[i].Multiplier;
						if (!flag || multiplier > 1f)
						{
							bool flag4 = multiplier < 0f;
							flag2 |= (flag4 && elements == Elements.Life);
							flag3 = (flag3 || flag4);
							num *= Math.Abs(multiplier);
						}
						if (!flag || iResistances[i].Modifier > 0f)
						{
							if (this.Amount >= 0f)
							{
								this.Amount = Math.Max(0f, this.Amount + iResistances[i].Modifier);
							}
							else
							{
								this.Amount = Math.Min(0f, this.Amount + iResistances[i].Modifier);
							}
						}
						if (iAdditionalResistances != null)
						{
							multiplier = iAdditionalResistances[i].Multiplier;
							if (!flag || multiplier > 1f)
							{
								bool flag4 = multiplier < 0f;
								flag2 |= (flag4 && elements == Elements.Life);
								flag3 = (flag3 || flag4);
								num *= Math.Abs(multiplier);
							}
							if (!flag || iAdditionalResistances[i].Modifier > 0f)
							{
								if (this.Amount >= 0f)
								{
									this.Amount = Math.Max(0f, this.Amount + iAdditionalResistances[i].Modifier);
								}
								else
								{
									this.Amount = Math.Min(0f, this.Amount + iAdditionalResistances[i].Modifier);
								}
							}
						}
					}
				}
			}
			if (iBuffs != null)
			{
				for (int j = 0; j < iBuffs.Count; j++)
				{
					if (iBuffs[j].BuffType == BuffType.Resistance && (iBuffs[j].BuffResistance.Resistance.ResistanceAgainst & this.Element) != Elements.None)
					{
						Resistance resistance = iBuffs[j].BuffResistance.Resistance;
						if (!flag || resistance.Multiplier > 1f)
						{
							bool flag5 = resistance.Multiplier < 0f;
							flag2 |= (flag5 && (resistance.ResistanceAgainst & Elements.Life) != Elements.None);
							flag3 = (flag3 || flag5);
							num *= resistance.Multiplier;
						}
						if (!flag || resistance.Modifier > 0f)
						{
							if (this.Amount >= 0f)
							{
								this.Amount = Math.Max(0f, this.Amount + resistance.Modifier);
							}
							else
							{
								this.Amount = Math.Min(0f, this.Amount + resistance.Modifier);
							}
						}
					}
				}
			}
			if (flag2)
			{
				this.Amount = Math.Abs(this.Amount);
			}
			else if (flag3)
			{
				this.Amount = -this.Amount;
			}
			this.Magnitude *= num;
		}

		// Token: 0x0600251E RID: 9502 RVA: 0x0010C8DC File Offset: 0x0010AADC
		public void ApplyResistancesInclusive(Resistance[] iResistances, Resistance[] iAdditionalResistances, IList<BuffStorage> iBuffs, StatusEffects iStatusEffects)
		{
			float num = float.NegativeInfinity;
			for (int i = 0; i < iResistances.Length; i++)
			{
				Elements elements = (Elements)(1 << i);
				if ((elements & this.Element) != Elements.None)
				{
					float multiplier = iResistances[i].Multiplier;
					num = Math.Max(num, iResistances[i].Multiplier);
					if (this.Amount >= 0f)
					{
						this.Amount = Math.Max(0f, this.Amount + iResistances[i].Modifier);
					}
					else
					{
						this.Amount = Math.Min(0f, this.Amount + iResistances[i].Modifier);
					}
					if (iAdditionalResistances != null)
					{
						float multiplier2 = iAdditionalResistances[i].Multiplier;
						num = Math.Max(num, iAdditionalResistances[i].Multiplier);
						if (this.Amount >= 0f)
						{
							this.Amount = Math.Max(0f, this.Amount + iAdditionalResistances[i].Modifier);
						}
						else
						{
							this.Amount = Math.Min(0f, this.Amount + iAdditionalResistances[i].Modifier);
						}
					}
				}
			}
			if (iBuffs != null)
			{
				for (int j = 0; j < iBuffs.Count; j++)
				{
					if (iBuffs[j].BuffType == BuffType.Resistance && (iBuffs[j].BuffResistance.Resistance.ResistanceAgainst & this.Element) != Elements.None)
					{
						Resistance resistance = iBuffs[j].BuffResistance.Resistance;
						num = Math.Max(num, resistance.Multiplier);
					}
				}
			}
			this.Magnitude *= num;
		}

		// Token: 0x0400287E RID: 10366
		public AttackProperties AttackProperty;

		// Token: 0x0400287F RID: 10367
		public Elements Element;

		// Token: 0x04002880 RID: 10368
		public float Amount;

		// Token: 0x04002881 RID: 10369
		public float Magnitude;
	}
}
