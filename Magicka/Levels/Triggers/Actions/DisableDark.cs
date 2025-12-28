using System;
using Magicka.Graphics;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x0200062A RID: 1578
	internal class DisableDark : Action
	{
		// Token: 0x06002F93 RID: 12179 RVA: 0x0018120D File Offset: 0x0017F40D
		public DisableDark(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x06002F94 RID: 12180 RVA: 0x00181217 File Offset: 0x0017F417
		protected override void Execute()
		{
			TutorialManager.Instance.DisableDark();
		}

		// Token: 0x06002F95 RID: 12181 RVA: 0x00181223 File Offset: 0x0017F423
		public override void QuickExecute()
		{
			this.Execute();
		}
	}
}
