using System;
using Magicka.GameLogic.Entities.Bosses;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x020000AD RID: 173
	public class EndBossFight : Action
	{
		// Token: 0x0600050F RID: 1295 RVA: 0x0001C6A1 File Offset: 0x0001A8A1
		public EndBossFight(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x06000510 RID: 1296 RVA: 0x0001C6AC File Offset: 0x0001A8AC
		protected override void Execute()
		{
			BossFight bossFight = base.GameScene.PlayState.BossFight;
			if (bossFight != null)
			{
				bossFight.Reset();
			}
		}

		// Token: 0x06000511 RID: 1297 RVA: 0x0001C6D3 File Offset: 0x0001A8D3
		public override void QuickExecute()
		{
			this.Execute();
		}
	}
}
