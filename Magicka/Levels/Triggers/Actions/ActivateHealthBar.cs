using System;
using Magicka.GameLogic.UI;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x020000BE RID: 190
	public class ActivateHealthBar : Action
	{
		// Token: 0x0600058D RID: 1421 RVA: 0x000209AF File Offset: 0x0001EBAF
		public ActivateHealthBar(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x0600058E RID: 1422 RVA: 0x000209BC File Offset: 0x0001EBBC
		protected override void Execute()
		{
			GenericHealthBar genericHealthBar = base.GameScene.PlayState.GenericHealthBar;
			genericHealthBar.Activate();
		}

		// Token: 0x0600058F RID: 1423 RVA: 0x000209E0 File Offset: 0x0001EBE0
		public override void QuickExecute()
		{
			this.Execute();
		}
	}
}
