using System;
using Magicka.Graphics;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x0200028A RID: 650
	internal class RemoveDialogHint : Action
	{
		// Token: 0x0600132E RID: 4910 RVA: 0x0007634A File Offset: 0x0007454A
		public RemoveDialogHint(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x0600132F RID: 4911 RVA: 0x00076354 File Offset: 0x00074554
		protected override void Execute()
		{
			TutorialManager.Instance.RemoveDialogHint();
		}

		// Token: 0x06001330 RID: 4912 RVA: 0x00076360 File Offset: 0x00074560
		public override void QuickExecute()
		{
			this.Execute();
		}
	}
}
