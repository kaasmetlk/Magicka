using System;
using Magicka.GameLogic.Entities;

namespace Magicka.Levels.Triggers.Conditions
{
	// Token: 0x020000B7 RID: 183
	public class PlayerCount : Condition
	{
		// Token: 0x06000559 RID: 1369 RVA: 0x000200C5 File Offset: 0x0001E2C5
		public PlayerCount(GameScene iScene) : base(iScene)
		{
		}

		// Token: 0x0600055A RID: 1370 RVA: 0x000200D0 File Offset: 0x0001E2D0
		protected override bool InternalMet(Character iSender)
		{
			int playerCount = Game.Instance.PlayerCount;
			switch (this.mCompareMethod)
			{
			case CompareMethod.LESS:
				return this.mNr > playerCount;
			case CompareMethod.EQUAL:
				return this.mNr == playerCount;
			case CompareMethod.GREATER:
				return this.mNr < playerCount;
			default:
				return false;
			}
		}

		// Token: 0x170000E1 RID: 225
		// (get) Token: 0x0600055B RID: 1371 RVA: 0x00020124 File Offset: 0x0001E324
		// (set) Token: 0x0600055C RID: 1372 RVA: 0x0002012C File Offset: 0x0001E32C
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

		// Token: 0x170000E2 RID: 226
		// (get) Token: 0x0600055D RID: 1373 RVA: 0x00020135 File Offset: 0x0001E335
		// (set) Token: 0x0600055E RID: 1374 RVA: 0x0002013D File Offset: 0x0001E33D
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

		// Token: 0x0400042A RID: 1066
		private CompareMethod mCompareMethod;

		// Token: 0x0400042B RID: 1067
		private int mNr;
	}
}
