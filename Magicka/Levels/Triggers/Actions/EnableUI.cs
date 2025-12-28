using System;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x0200001C RID: 28
	public class EnableUI : Action
	{
		// Token: 0x060000D8 RID: 216 RVA: 0x00006EC7 File Offset: 0x000050C7
		public EnableUI(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x060000D9 RID: 217 RVA: 0x00006ED1 File Offset: 0x000050D1
		protected override void Execute()
		{
			this.mScene.PlayState.UIEnabled = true;
		}

		// Token: 0x060000DA RID: 218 RVA: 0x00006EE4 File Offset: 0x000050E4
		public override void QuickExecute()
		{
			this.Execute();
		}
	}
}
