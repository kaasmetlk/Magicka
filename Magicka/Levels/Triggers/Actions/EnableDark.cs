using System;
using Magicka.Graphics;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x02000145 RID: 325
	internal class EnableDark : Action
	{
		// Token: 0x0600092A RID: 2346 RVA: 0x0003986E File Offset: 0x00037A6E
		public EnableDark(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x0600092B RID: 2347 RVA: 0x00039878 File Offset: 0x00037A78
		protected override void Execute()
		{
			TutorialManager.Instance.EnableDark();
		}

		// Token: 0x0600092C RID: 2348 RVA: 0x00039884 File Offset: 0x00037A84
		public override void QuickExecute()
		{
			this.Execute();
		}
	}
}
