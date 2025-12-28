using System;
using Magicka.GameLogic.UI;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x0200059E RID: 1438
	public class KillDialogs : Action
	{
		// Token: 0x06002AE5 RID: 10981 RVA: 0x00151936 File Offset: 0x0014FB36
		public KillDialogs(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x06002AE6 RID: 10982 RVA: 0x00151940 File Offset: 0x0014FB40
		protected override void Execute()
		{
			DialogManager.Instance.EndAll();
		}

		// Token: 0x06002AE7 RID: 10983 RVA: 0x0015194C File Offset: 0x0014FB4C
		public override void QuickExecute()
		{
			this.Execute();
		}
	}
}
