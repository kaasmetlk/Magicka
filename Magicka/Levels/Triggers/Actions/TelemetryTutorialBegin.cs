using System;
using Magicka.WebTools.Paradox.Telemetry;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x0200038D RID: 909
	public class TelemetryTutorialBegin : Action
	{
		// Token: 0x170006D6 RID: 1750
		// (get) Token: 0x06001BD0 RID: 7120 RVA: 0x000BF1EA File Offset: 0x000BD3EA
		// (set) Token: 0x06001BD1 RID: 7121 RVA: 0x000BF1F2 File Offset: 0x000BD3F2
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

		// Token: 0x170006D7 RID: 1751
		// (get) Token: 0x06001BD2 RID: 7122 RVA: 0x000BF1FB File Offset: 0x000BD3FB
		// (set) Token: 0x06001BD3 RID: 7123 RVA: 0x000BF203 File Offset: 0x000BD403
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

		// Token: 0x06001BD4 RID: 7124 RVA: 0x000BF20C File Offset: 0x000BD40C
		public TelemetryTutorialBegin(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x06001BD5 RID: 7125 RVA: 0x000BF22C File Offset: 0x000BD42C
		protected override void Execute()
		{
			TutorialUtils.Start(this.mTutorialName, this.mStepName);
		}

		// Token: 0x06001BD6 RID: 7126 RVA: 0x000BF23F File Offset: 0x000BD43F
		public override void QuickExecute()
		{
			this.Execute();
		}

		// Token: 0x04001E27 RID: 7719
		private string mTutorialName = string.Empty;

		// Token: 0x04001E28 RID: 7720
		private string mStepName = string.Empty;
	}
}
