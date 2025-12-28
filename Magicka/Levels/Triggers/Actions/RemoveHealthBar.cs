using System;
using Magicka.GameLogic.UI;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x020001AC RID: 428
	public class RemoveHealthBar : Action
	{
		// Token: 0x06000CCE RID: 3278 RVA: 0x0004B880 File Offset: 0x00049A80
		public RemoveHealthBar(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x06000CCF RID: 3279 RVA: 0x0004B88C File Offset: 0x00049A8C
		protected override void Execute()
		{
			GenericHealthBar genericHealthBar = base.GameScene.PlayState.GenericHealthBar;
			genericHealthBar.Remove();
		}

		// Token: 0x06000CD0 RID: 3280 RVA: 0x0004B8B0 File Offset: 0x00049AB0
		public override void QuickExecute()
		{
			this.Execute();
		}
	}
}
