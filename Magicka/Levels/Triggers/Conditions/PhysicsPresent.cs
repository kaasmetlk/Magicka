using System;
using Magicka.GameLogic.Entities;

namespace Magicka.Levels.Triggers.Conditions
{
	// Token: 0x0200032E RID: 814
	internal class PhysicsPresent : Condition
	{
		// Token: 0x060018D5 RID: 6357 RVA: 0x000A3DA2 File Offset: 0x000A1FA2
		public PhysicsPresent(GameScene iScene) : base(iScene)
		{
		}

		// Token: 0x060018D6 RID: 6358 RVA: 0x000A3DC4 File Offset: 0x000A1FC4
		protected override bool InternalMet(Character iSender)
		{
			int num = 0;
			TriggerArea triggerArea = base.Scene.GetTriggerArea(this.mAreaID);
			for (int i = 0; i < triggerArea.PresentEntities.Count; i++)
			{
				if (this.mOnlyDamagable)
				{
					if (triggerArea.PresentEntities[i] is DamageablePhysicsEntity)
					{
						num++;
					}
				}
				else if (triggerArea.PresentEntities[i] is PhysicsEntity)
				{
					num++;
				}
			}
			switch (this.mCompareMethod)
			{
			case CompareMethod.LESS:
				if (num < this.mNr)
				{
					return true;
				}
				break;
			case CompareMethod.EQUAL:
				if (num == this.mNr)
				{
					return true;
				}
				break;
			case CompareMethod.GREATER:
				if (num > this.mNr)
				{
					return true;
				}
				break;
			}
			return false;
		}

		// Token: 0x1700062D RID: 1581
		// (get) Token: 0x060018D7 RID: 6359 RVA: 0x000A3E72 File Offset: 0x000A2072
		// (set) Token: 0x060018D8 RID: 6360 RVA: 0x000A3E7A File Offset: 0x000A207A
		public bool OnlyDamagable
		{
			get
			{
				return this.mOnlyDamagable;
			}
			set
			{
				this.mOnlyDamagable = value;
			}
		}

		// Token: 0x1700062E RID: 1582
		// (get) Token: 0x060018D9 RID: 6361 RVA: 0x000A3E83 File Offset: 0x000A2083
		// (set) Token: 0x060018DA RID: 6362 RVA: 0x000A3E8B File Offset: 0x000A208B
		public string Area
		{
			get
			{
				return this.mArea;
			}
			set
			{
				this.mArea = value;
				this.mAreaID = this.mArea.GetHashCodeCustom();
			}
		}

		// Token: 0x1700062F RID: 1583
		// (get) Token: 0x060018DB RID: 6363 RVA: 0x000A3EA5 File Offset: 0x000A20A5
		// (set) Token: 0x060018DC RID: 6364 RVA: 0x000A3EAD File Offset: 0x000A20AD
		public int Nr
		{
			get
			{
				return this.mNr;
			}
			set
			{
				this.mNr = value;
			}
		}

		// Token: 0x17000630 RID: 1584
		// (get) Token: 0x060018DD RID: 6365 RVA: 0x000A3EB6 File Offset: 0x000A20B6
		// (set) Token: 0x060018DE RID: 6366 RVA: 0x000A3EBE File Offset: 0x000A20BE
		public CompareMethod CompareMethod
		{
			get
			{
				return this.mCompareMethod;
			}
			set
			{
				this.mCompareMethod = value;
			}
		}

		// Token: 0x04001AAD RID: 6829
		private string mArea = "any";

		// Token: 0x04001AAE RID: 6830
		private int mAreaID = Condition.ANYID;

		// Token: 0x04001AAF RID: 6831
		private int mNr;

		// Token: 0x04001AB0 RID: 6832
		private CompareMethod mCompareMethod;

		// Token: 0x04001AB1 RID: 6833
		private bool mOnlyDamagable;
	}
}
