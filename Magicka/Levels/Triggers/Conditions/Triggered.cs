using System;
using Magicka.GameLogic.Entities;

namespace Magicka.Levels.Triggers.Conditions
{
	// Token: 0x02000480 RID: 1152
	public class Triggered : Condition
	{
		// Token: 0x060022DF RID: 8927 RVA: 0x000FB6CD File Offset: 0x000F98CD
		public Triggered(GameScene iScene) : base(iScene)
		{
		}

		// Token: 0x060022E0 RID: 8928 RVA: 0x000FB6D6 File Offset: 0x000F98D6
		protected override bool InternalMet(Character iSender)
		{
			return base.Scene.Triggers[this.mTriggerId].HasTriggered;
		}

		// Token: 0x17000848 RID: 2120
		// (get) Token: 0x060022E1 RID: 8929 RVA: 0x000FB6F3 File Offset: 0x000F98F3
		// (set) Token: 0x060022E2 RID: 8930 RVA: 0x000FB6FB File Offset: 0x000F98FB
		public string Trigger
		{
			get
			{
				return this.mTrigger;
			}
			set
			{
				this.mTrigger = value;
				this.mTriggerId = this.mTrigger.GetHashCodeCustom();
			}
		}

		// Token: 0x04002612 RID: 9746
		private string mTrigger;

		// Token: 0x04002613 RID: 9747
		private int mTriggerId;
	}
}
