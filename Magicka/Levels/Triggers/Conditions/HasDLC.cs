using System;
using Magicka.GameLogic.Entities;

namespace Magicka.Levels.Triggers.Conditions
{
	// Token: 0x0200055A RID: 1370
	internal class HasDLC : Condition
	{
		// Token: 0x060028DA RID: 10458 RVA: 0x001419C1 File Offset: 0x0013FBC1
		public HasDLC(GameScene iScene) : base(iScene)
		{
		}

		// Token: 0x060028DB RID: 10459 RVA: 0x001419CA File Offset: 0x0013FBCA
		protected override bool InternalMet(Character iSender)
		{
			return Helper.CheckDLC(this.mType);
		}

		// Token: 0x1700099C RID: 2460
		// (get) Token: 0x060028DC RID: 10460 RVA: 0x001419D7 File Offset: 0x0013FBD7
		// (set) Token: 0x060028DD RID: 10461 RVA: 0x001419DF File Offset: 0x0013FBDF
		public Helper.DLC Type
		{
			get
			{
				return this.mType;
			}
			set
			{
				this.mType = value;
			}
		}

		// Token: 0x04002C50 RID: 11344
		private Helper.DLC mType;
	}
}
