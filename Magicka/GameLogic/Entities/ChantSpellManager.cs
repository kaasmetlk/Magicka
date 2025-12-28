using System;
using PolygonHead;

namespace Magicka.GameLogic.Entities
{
	// Token: 0x02000169 RID: 361
	public static class ChantSpellManager
	{
		// Token: 0x06000B00 RID: 2816 RVA: 0x00042350 File Offset: 0x00040550
		static ChantSpellManager()
		{
			ChantSpellManager.sChantSpellHeap = new IntHeap(ChantSpellManager.MAXELEMENTS);
			for (int i = 1; i < ChantSpellManager.MAXELEMENTS; i++)
			{
				ChantSpellManager.sChantSpellHeap.Push(i);
			}
			ChantSpellManager.sChantSpells = new ChantSpells[ChantSpellManager.MAXELEMENTS];
		}

		// Token: 0x06000B01 RID: 2817 RVA: 0x000423A8 File Offset: 0x000405A8
		public static void Add(ref ChantSpells iChantSpell)
		{
			if (ChantSpellManager.sChantSpellHeap.IsEmpty)
			{
				return;
			}
			iChantSpell.Active = true;
			iChantSpell.Index = ChantSpellManager.sChantSpellHeap.Pop();
			ChantSpellManager.sChantSpells[iChantSpell.Index] = iChantSpell;
			ChantSpellManager.sLastActiveChantSpell = Math.Max(ChantSpellManager.sLastActiveChantSpell, iChantSpell.Index);
		}

		// Token: 0x06000B02 RID: 2818 RVA: 0x00042409 File Offset: 0x00040609
		public static void Remove(ref ChantSpells iChantSpell)
		{
			iChantSpell.Active = false;
			ChantSpellManager.sChantSpellHeap.Push(iChantSpell.Index);
			ChantSpellManager.sChantSpells[iChantSpell.Index] = default(ChantSpells);
		}

		// Token: 0x06000B03 RID: 2819 RVA: 0x00042438 File Offset: 0x00040638
		public static void Set(ChantSpells iChantSpell)
		{
			ChantSpellManager.sChantSpells[iChantSpell.Index] = iChantSpell;
		}

		// Token: 0x06000B04 RID: 2820 RVA: 0x00042451 File Offset: 0x00040651
		public static ChantSpells GetChantSpell(int iIndex)
		{
			return ChantSpellManager.sChantSpells[iIndex];
		}

		// Token: 0x06000B05 RID: 2821 RVA: 0x00042464 File Offset: 0x00040664
		internal static void Update(float iDeltaTime)
		{
			for (int i = 1; i <= ChantSpellManager.sLastActiveChantSpell; i++)
			{
				if (ChantSpellManager.sChantSpells[i].Active)
				{
					ChantSpellManager.sChantSpells[i].Update(iDeltaTime);
				}
			}
		}

		// Token: 0x06000B06 RID: 2822 RVA: 0x000424A4 File Offset: 0x000406A4
		internal static void Merge(Character iOwner, Elements iElement1, Elements iElement2, Elements iElementOut)
		{
			int num = -1;
			int num2 = -1;
			for (int i = 1; i < ChantSpellManager.sChantSpells.Length; i++)
			{
				if (!ChantSpellManager.sChantSpellHeap.Contains(i) && ChantSpellManager.sChantSpells[i].Owner == iOwner && ChantSpellManager.sChantSpells[i].State != ChantSpellState.Merging)
				{
					if (ChantSpellManager.sChantSpells[i].Element == iElement1 && num == -1)
					{
						num = i;
					}
					else if (ChantSpellManager.sChantSpells[i].Element == iElement2 && num2 == -1)
					{
						num2 = i;
					}
				}
			}
			if (num >= 1 && num2 >= 1)
			{
				ChantSpellManager.sChantSpells[num].MergeWith(ChantSpellManager.GetChantSpell(num2), iElementOut);
			}
		}

		// Token: 0x04000A03 RID: 2563
		private static readonly int MAXELEMENTS = 128;

		// Token: 0x04000A04 RID: 2564
		private static IntHeap sChantSpellHeap;

		// Token: 0x04000A05 RID: 2565
		private static ChantSpells[] sChantSpells;

		// Token: 0x04000A06 RID: 2566
		private static int sLastActiveChantSpell = -1;
	}
}
