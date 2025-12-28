using System;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x02000486 RID: 1158
	public class DisableUI : Action
	{
		// Token: 0x06002308 RID: 8968 RVA: 0x000FBAD0 File Offset: 0x000F9CD0
		public DisableUI(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x06002309 RID: 8969 RVA: 0x000FBADA File Offset: 0x000F9CDA
		protected override void Execute()
		{
			this.mScene.PlayState.UIEnabled = false;
		}

		// Token: 0x0600230A RID: 8970 RVA: 0x000FBAED File Offset: 0x000F9CED
		public override void QuickExecute()
		{
			this.Execute();
		}
	}
}
