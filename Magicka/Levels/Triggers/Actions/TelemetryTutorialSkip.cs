using System;
using Magicka.WebTools.Paradox.Telemetry;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x0200055C RID: 1372
	public class TelemetryTutorialSkip : Action
	{
		// Token: 0x060028E6 RID: 10470 RVA: 0x00141AAC File Offset: 0x0013FCAC
		public TelemetryTutorialSkip(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x060028E7 RID: 10471 RVA: 0x00141AB6 File Offset: 0x0013FCB6
		protected override void Execute()
		{
			TutorialUtils.Skip();
		}

		// Token: 0x060028E8 RID: 10472 RVA: 0x00141ABD File Offset: 0x0013FCBD
		public override void QuickExecute()
		{
			this.Execute();
		}
	}
}
