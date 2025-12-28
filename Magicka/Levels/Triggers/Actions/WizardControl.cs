using System;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x02000237 RID: 567
	internal class WizardControl : Action
	{
		// Token: 0x06001163 RID: 4451 RVA: 0x0006BA21 File Offset: 0x00069C21
		public WizardControl(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x06001164 RID: 4452 RVA: 0x0006BA2C File Offset: 0x00069C2C
		protected override void Execute()
		{
			switch (this.mAction)
			{
			case WizardAction.StaffLightOn:
				base.GameScene.PlayState.StaffLight = true;
				return;
			case WizardAction.StaffLightOff:
				base.GameScene.PlayState.StaffLight = false;
				return;
			default:
				return;
			}
		}

		// Token: 0x06001165 RID: 4453 RVA: 0x0006BA72 File Offset: 0x00069C72
		public override void QuickExecute()
		{
			this.Execute();
		}

		// Token: 0x17000470 RID: 1136
		// (get) Token: 0x06001166 RID: 4454 RVA: 0x0006BA7A File Offset: 0x00069C7A
		// (set) Token: 0x06001167 RID: 4455 RVA: 0x0006BA82 File Offset: 0x00069C82
		public WizardAction Action
		{
			get
			{
				return this.mAction;
			}
			set
			{
				this.mAction = value;
			}
		}

		// Token: 0x04001059 RID: 4185
		private WizardAction mAction;
	}
}
