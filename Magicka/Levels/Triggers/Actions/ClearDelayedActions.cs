using System;
using System.Collections.Generic;
using System.Xml;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x020004FA RID: 1274
	internal class ClearDelayedActions : Action
	{
		// Token: 0x060025AD RID: 9645 RVA: 0x001115E6 File Offset: 0x0010F7E6
		public ClearDelayedActions(Trigger iTrigger, GameScene iScene, XmlNode iNode) : base(iTrigger, iScene)
		{
		}

		// Token: 0x060025AE RID: 9646 RVA: 0x001115F0 File Offset: 0x0010F7F0
		protected override void Execute()
		{
			if (this.mTriggerID == ClearDelayedActions.ALL || this.mTriggerID == 0)
			{
				using (IEnumerator<Trigger> enumerator = this.mScene.Triggers.Values.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Trigger trigger = enumerator.Current;
						trigger.ClearDelayedActions();
					}
					return;
				}
			}
			if (this.mTriggerID == ClearDelayedActions.ALLBUTTHIS)
			{
				using (IEnumerator<Trigger> enumerator2 = this.mScene.Triggers.Values.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						Trigger trigger2 = enumerator2.Current;
						if (trigger2 != this.mTrigger)
						{
							trigger2.ClearDelayedActions();
						}
					}
					return;
				}
			}
			this.mScene.Triggers[this.mTriggerID].ClearDelayedActions();
		}

		// Token: 0x060025AF RID: 9647 RVA: 0x001116D4 File Offset: 0x0010F8D4
		public override void QuickExecute()
		{
		}

		// Token: 0x170008C5 RID: 2245
		// (get) Token: 0x060025B0 RID: 9648 RVA: 0x001116D6 File Offset: 0x0010F8D6
		// (set) Token: 0x060025B1 RID: 9649 RVA: 0x001116DE File Offset: 0x0010F8DE
		public new string Trigger
		{
			get
			{
				return this.mTriggerName;
			}
			set
			{
				this.mTriggerName = value;
				this.mTriggerID = this.mTriggerName.ToLowerInvariant().GetHashCodeCustom();
			}
		}

		// Token: 0x04002909 RID: 10505
		private static readonly int ALL = "all".GetHashCodeCustom();

		// Token: 0x0400290A RID: 10506
		private static readonly int ALLBUTTHIS = "allbutthis".GetHashCodeCustom();

		// Token: 0x0400290B RID: 10507
		private string mTriggerName;

		// Token: 0x0400290C RID: 10508
		private int mTriggerID;
	}
}
