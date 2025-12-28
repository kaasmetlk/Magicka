using System;
using Magicka.WebTools.Paradox.Telemetry;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x02000019 RID: 25
	public class TelemetryTutorialStep : Action
	{
		// Token: 0x1700002E RID: 46
		// (get) Token: 0x060000BC RID: 188 RVA: 0x00006C73 File Offset: 0x00004E73
		// (set) Token: 0x060000BD RID: 189 RVA: 0x00006C7B File Offset: 0x00004E7B
		public string TutorialName
		{
			get
			{
				return this.mTutorialName;
			}
			set
			{
				this.mTutorialName = value;
			}
		}

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x060000BE RID: 190 RVA: 0x00006C84 File Offset: 0x00004E84
		// (set) Token: 0x060000BF RID: 191 RVA: 0x00006C8C File Offset: 0x00004E8C
		public string StepName
		{
			get
			{
				return this.mStepName;
			}
			set
			{
				this.mStepName = value;
			}
		}

		// Token: 0x060000C0 RID: 192 RVA: 0x00006C95 File Offset: 0x00004E95
		public TelemetryTutorialStep(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x00006CB5 File Offset: 0x00004EB5
		protected override void Execute()
		{
			TutorialUtils.Step(this.mTutorialName, this.mStepName);
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x00006CC8 File Offset: 0x00004EC8
		public override void QuickExecute()
		{
			this.Execute();
		}

		// Token: 0x04000092 RID: 146
		private string mTutorialName = string.Empty;

		// Token: 0x04000093 RID: 147
		private string mStepName = string.Empty;
	}
}
