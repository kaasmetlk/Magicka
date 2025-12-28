using System;
using Magicka.Graphics;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x02000394 RID: 916
	public class DisablePush : Action
	{
		// Token: 0x06001C10 RID: 7184 RVA: 0x000BF90C File Offset: 0x000BDB0C
		public DisablePush(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x06001C11 RID: 7185 RVA: 0x000BF916 File Offset: 0x000BDB16
		protected override void Execute()
		{
			TutorialManager.Instance.DisablePush();
		}

		// Token: 0x06001C12 RID: 7186 RVA: 0x000BF922 File Offset: 0x000BDB22
		public override void QuickExecute()
		{
			this.Execute();
		}
	}
}
