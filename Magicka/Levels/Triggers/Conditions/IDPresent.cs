using System;
using Magicka.GameLogic.Entities;

namespace Magicka.Levels.Triggers.Conditions
{
	// Token: 0x02000141 RID: 321
	public class IDPresent : Condition
	{
		// Token: 0x06000908 RID: 2312 RVA: 0x00039557 File Offset: 0x00037757
		public IDPresent(GameScene iScene) : base(iScene)
		{
		}

		// Token: 0x06000909 RID: 2313 RVA: 0x00039578 File Offset: 0x00037778
		protected override bool InternalMet(Character iSender)
		{
			TriggerArea triggerArea = base.Scene.GetTriggerArea(this.mAreaID);
			for (int i = 0; i < triggerArea.PresentEntities.Count; i++)
			{
				if (triggerArea.PresentEntities[i].UniqueID == this.mID)
				{
					return !triggerArea.PresentEntities[i].Dead;
				}
			}
			return false;
		}

		// Token: 0x170001D0 RID: 464
		// (get) Token: 0x0600090A RID: 2314 RVA: 0x000395DC File Offset: 0x000377DC
		// (set) Token: 0x0600090B RID: 2315 RVA: 0x000395E4 File Offset: 0x000377E4
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

		// Token: 0x170001D1 RID: 465
		// (get) Token: 0x0600090C RID: 2316 RVA: 0x000395FE File Offset: 0x000377FE
		// (set) Token: 0x0600090D RID: 2317 RVA: 0x00039606 File Offset: 0x00037806
		public string ID
		{
			get
			{
				return this.mName;
			}
			set
			{
				this.mName = value;
				this.mID = this.mName.GetHashCodeCustom();
			}
		}

		// Token: 0x0400086C RID: 2156
		private string mArea = "any";

		// Token: 0x0400086D RID: 2157
		private int mAreaID = Condition.ANYID;

		// Token: 0x0400086E RID: 2158
		private string mName;

		// Token: 0x0400086F RID: 2159
		private int mID;
	}
}
