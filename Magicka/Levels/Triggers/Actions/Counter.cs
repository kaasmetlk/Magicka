using System;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x0200023A RID: 570
	internal class Counter : Action
	{
		// Token: 0x06001175 RID: 4469 RVA: 0x0006BE64 File Offset: 0x0006A064
		public Counter(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x06001176 RID: 4470 RVA: 0x0006BE70 File Offset: 0x0006A070
		protected override void Execute()
		{
			switch (this.mAction)
			{
			case CounterAction.INC:
				base.GameScene.Level.AddToCounter(this.mID, this.mValue);
				return;
			case CounterAction.DEC:
				base.GameScene.Level.AddToCounter(this.mID, -this.mValue);
				return;
			case CounterAction.SET:
				base.GameScene.Level.SetCounterValue(this.mID, this.mValue);
				return;
			default:
				return;
			}
		}

		// Token: 0x06001177 RID: 4471 RVA: 0x0006BEEE File Offset: 0x0006A0EE
		public override void QuickExecute()
		{
		}

		// Token: 0x17000474 RID: 1140
		// (get) Token: 0x06001178 RID: 4472 RVA: 0x0006BEF0 File Offset: 0x0006A0F0
		// (set) Token: 0x06001179 RID: 4473 RVA: 0x0006BEF8 File Offset: 0x0006A0F8
		public string Name
		{
			get
			{
				return this.mName;
			}
			set
			{
				this.mName = value;
				this.mID = this.mName.GetHashCodeCustom();
			}
		}

		// Token: 0x17000475 RID: 1141
		// (get) Token: 0x0600117A RID: 4474 RVA: 0x0006BF12 File Offset: 0x0006A112
		// (set) Token: 0x0600117B RID: 4475 RVA: 0x0006BF1A File Offset: 0x0006A11A
		public CounterAction Action
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

		// Token: 0x17000476 RID: 1142
		// (get) Token: 0x0600117C RID: 4476 RVA: 0x0006BF23 File Offset: 0x0006A123
		// (set) Token: 0x0600117D RID: 4477 RVA: 0x0006BF2B File Offset: 0x0006A12B
		public int Value
		{
			get
			{
				return this.mValue;
			}
			set
			{
				this.mValue = value;
			}
		}

		// Token: 0x04001062 RID: 4194
		private string mName;

		// Token: 0x04001063 RID: 4195
		private int mID;

		// Token: 0x04001064 RID: 4196
		private CounterAction mAction;

		// Token: 0x04001065 RID: 4197
		private int mValue;
	}
}
