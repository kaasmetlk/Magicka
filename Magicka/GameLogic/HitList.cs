using System;
using System.Collections.Generic;
using Magicka.GameLogic.Entities;

namespace Magicka.GameLogic
{
	// Token: 0x0200002E RID: 46
	public class HitList : SortedList<ushort, float>
	{
		// Token: 0x0600017B RID: 379 RVA: 0x0000AC70 File Offset: 0x00008E70
		public HitList(int iCapacity) : base(iCapacity)
		{
		}

		// Token: 0x0600017C RID: 380 RVA: 0x0000AC79 File Offset: 0x00008E79
		public void Add(Entity iEntity)
		{
			base[iEntity.Handle] = 0.25f;
		}

		// Token: 0x0600017D RID: 381 RVA: 0x0000AC8C File Offset: 0x00008E8C
		public void Add(IDamageable iDamageable)
		{
			base[iDamageable.Handle] = 0.25f;
		}

		// Token: 0x0600017E RID: 382 RVA: 0x0000AC9F File Offset: 0x00008E9F
		public void Add(ISpellCaster iCaster)
		{
			base[iCaster.Handle] = 0.25f;
		}

		// Token: 0x0600017F RID: 383 RVA: 0x0000ACB2 File Offset: 0x00008EB2
		public void Add(ushort iHandle)
		{
			base[iHandle] = 0.25f;
		}

		// Token: 0x06000180 RID: 384 RVA: 0x0000ACC0 File Offset: 0x00008EC0
		public bool Contains(IDamageable iDamageable)
		{
			return base.ContainsKey(iDamageable.Handle);
		}

		// Token: 0x06000181 RID: 385 RVA: 0x0000ACD0 File Offset: 0x00008ED0
		public void Update(float iDeltaTime)
		{
			for (int i = 0; i < base.Count; i++)
			{
				float num = base.Values[i] - iDeltaTime;
				if (num <= 0f)
				{
					base.RemoveAt(i--);
				}
				else
				{
					base[base.Keys[i]] = num;
				}
			}
		}
	}
}
