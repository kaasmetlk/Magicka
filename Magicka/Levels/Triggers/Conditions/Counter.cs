using System;
using Magicka.GameLogic.Entities;

namespace Magicka.Levels.Triggers.Conditions
{
	// Token: 0x020004F4 RID: 1268
	public class Counter : Condition
	{
		// Token: 0x06002582 RID: 9602 RVA: 0x00111243 File Offset: 0x0010F443
		public Counter(GameScene iScene) : base(iScene)
		{
		}

		// Token: 0x06002583 RID: 9603 RVA: 0x0011124C File Offset: 0x0010F44C
		protected override bool InternalMet(Character iSender)
		{
			int counterValue = base.Scene.Level.GetCounterValue(this.mID);
			switch (this.mCompareMethod)
			{
			case CompareMethod.LESS:
				return this.mValue > counterValue;
			case CompareMethod.EQUAL:
				return this.mValue == counterValue;
			case CompareMethod.GREATER:
				return this.mValue < counterValue;
			default:
				return false;
			}
		}

		// Token: 0x170008B8 RID: 2232
		// (get) Token: 0x06002584 RID: 9604 RVA: 0x001112AC File Offset: 0x0010F4AC
		// (set) Token: 0x06002585 RID: 9605 RVA: 0x001112B4 File Offset: 0x0010F4B4
		public string Name
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

		// Token: 0x170008B9 RID: 2233
		// (get) Token: 0x06002586 RID: 9606 RVA: 0x001112CE File Offset: 0x0010F4CE
		// (set) Token: 0x06002587 RID: 9607 RVA: 0x001112D6 File Offset: 0x0010F4D6
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

		// Token: 0x170008BA RID: 2234
		// (get) Token: 0x06002588 RID: 9608 RVA: 0x001112DF File Offset: 0x0010F4DF
		// (set) Token: 0x06002589 RID: 9609 RVA: 0x001112E7 File Offset: 0x0010F4E7
		public int Value
		{
			get
			{
				return this.mValue;
			}
			set
			{
				this.mValue = value;
			}
		}

		// Token: 0x040028F5 RID: 10485
		private string mName;

		// Token: 0x040028F6 RID: 10486
		private int mID;

		// Token: 0x040028F7 RID: 10487
		private CompareMethod mCompareMethod;

		// Token: 0x040028F8 RID: 10488
		private int mValue;
	}
}
