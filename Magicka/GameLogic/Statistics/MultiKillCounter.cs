using System;
using System.Collections.Generic;
using Magicka.Achievements;

namespace Magicka.GameLogic.Statistics
{
	// Token: 0x0200024A RID: 586
	public class MultiKillCounter : SortedList<double, KillCount>
	{
		// Token: 0x06001224 RID: 4644 RVA: 0x0006E83D File Offset: 0x0006CA3D
		public MultiKillCounter(int iCapacity) : base(iCapacity)
		{
		}

		// Token: 0x06001225 RID: 4645 RVA: 0x0006E846 File Offset: 0x0006CA46
		public void Add(double iTimeStamp)
		{
			this.Check(iTimeStamp, 1, 120f);
		}

		// Token: 0x06001226 RID: 4646 RVA: 0x0006E855 File Offset: 0x0006CA55
		public void Add(double iTimeStamp, float iTTL)
		{
			this.Check(iTimeStamp, 1, iTTL);
		}

		// Token: 0x06001227 RID: 4647 RVA: 0x0006E860 File Offset: 0x0006CA60
		public void Add(double iTimeStamp, int iCount)
		{
			this.Check(iTimeStamp, iCount, 120f);
		}

		// Token: 0x06001228 RID: 4648 RVA: 0x0006E86F File Offset: 0x0006CA6F
		public void Add(double iTimeStamp, int iCount, float iTTL)
		{
			this.Check(iTimeStamp, iCount, iTTL);
		}

		// Token: 0x06001229 RID: 4649 RVA: 0x0006E87C File Offset: 0x0006CA7C
		internal void Check(double iTimeStamp, int iCount, float iTTL)
		{
			KillCount value;
			if (base.TryGetValue(iTimeStamp, out value))
			{
				value.Count += iCount;
				value.TTL = iTTL;
			}
			else
			{
				value = new KillCount(iCount, iTTL);
			}
			base[iTimeStamp] = value;
		}

		// Token: 0x0600122A RID: 4650 RVA: 0x0006E8C4 File Offset: 0x0006CAC4
		public void Update(Player iOwner, float iDeltaTime)
		{
			for (int i = 0; i < base.Count; i++)
			{
				KillCount value = base.Values[i];
				value.TTL -= iDeltaTime;
				if (value.TTL <= 0f)
				{
					base.RemoveAt(i--);
				}
				else
				{
					base[base.Keys[i]] = value;
					if (value.Count >= 20 && iOwner.Avatar != null)
					{
						AchievementsManager.Instance.AwardAchievement(iOwner.Avatar.PlayState, "mumumumultikill");
					}
				}
			}
		}
	}
}
