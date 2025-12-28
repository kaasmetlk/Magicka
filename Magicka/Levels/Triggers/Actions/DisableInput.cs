using System;
using Magicka.GameLogic.Controls;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x02000414 RID: 1044
	public class DisableInput : Action
	{
		// Token: 0x06002056 RID: 8278 RVA: 0x000E58E9 File Offset: 0x000E3AE9
		public DisableInput(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x06002057 RID: 8279 RVA: 0x000E58F3 File Offset: 0x000E3AF3
		protected override void Execute()
		{
			ControlManager.Instance.LimitInput(base.GameScene.Level);
		}

		// Token: 0x06002058 RID: 8280 RVA: 0x000E590A File Offset: 0x000E3B0A
		public override void QuickExecute()
		{
			this.Execute();
		}
	}
}
