using System;
using Magicka.GameLogic.Controls;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x020001A8 RID: 424
	public class EnableInput : Action
	{
		// Token: 0x06000CB3 RID: 3251 RVA: 0x0004B5CC File Offset: 0x000497CC
		public EnableInput(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x06000CB4 RID: 3252 RVA: 0x0004B5D6 File Offset: 0x000497D6
		protected override void Execute()
		{
			ControlManager.Instance.UnlimitInput(base.GameScene.Level);
		}

		// Token: 0x06000CB5 RID: 3253 RVA: 0x0004B5ED File Offset: 0x000497ED
		public override void QuickExecute()
		{
			this.Execute();
		}
	}
}
