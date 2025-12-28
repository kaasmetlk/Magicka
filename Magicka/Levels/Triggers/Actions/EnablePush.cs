using System;
using Magicka.Graphics;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x02000485 RID: 1157
	public class EnablePush : Action
	{
		// Token: 0x06002305 RID: 8965 RVA: 0x000FBAB2 File Offset: 0x000F9CB2
		public EnablePush(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x06002306 RID: 8966 RVA: 0x000FBABC File Offset: 0x000F9CBC
		protected override void Execute()
		{
			TutorialManager.Instance.EnablePush();
		}

		// Token: 0x06002307 RID: 8967 RVA: 0x000FBAC8 File Offset: 0x000F9CC8
		public override void QuickExecute()
		{
			this.Execute();
		}
	}
}
