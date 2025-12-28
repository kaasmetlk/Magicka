using System;
using Magicka.GameLogic.Entities.Bosses;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x0200055E RID: 1374
	internal class ActivateBoss : Action
	{
		// Token: 0x060028F9 RID: 10489 RVA: 0x00141FAA File Offset: 0x001401AA
		public ActivateBoss(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x060028FA RID: 10490 RVA: 0x00141FB4 File Offset: 0x001401B4
		protected override void Execute()
		{
			BossFight.Instance.Start();
		}

		// Token: 0x060028FB RID: 10491 RVA: 0x00141FC0 File Offset: 0x001401C0
		public override void QuickExecute()
		{
			this.Execute();
		}
	}
}
