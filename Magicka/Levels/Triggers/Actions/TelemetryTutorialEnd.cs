using System;
using Magicka.WebTools.Paradox.Telemetry;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x020002C8 RID: 712
	public class TelemetryTutorialEnd : Action
	{
		// Token: 0x060015A7 RID: 5543 RVA: 0x0008A943 File Offset: 0x00088B43
		public TelemetryTutorialEnd(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x060015A8 RID: 5544 RVA: 0x0008A94D File Offset: 0x00088B4D
		protected override void Execute()
		{
			TutorialUtils.Complete();
		}

		// Token: 0x060015A9 RID: 5545 RVA: 0x0008A954 File Offset: 0x00088B54
		public override void QuickExecute()
		{
			this.Execute();
		}
	}
}
