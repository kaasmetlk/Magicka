using System;
using Magicka.GameLogic.Entities;

namespace Magicka.Levels.Triggers.Conditions
{
	// Token: 0x020001A9 RID: 425
	internal class Timer : Condition
	{
		// Token: 0x06000CB6 RID: 3254 RVA: 0x0004B5F5 File Offset: 0x000497F5
		public Timer(GameScene iScene) : base(iScene)
		{
		}

		// Token: 0x06000CB7 RID: 3255 RVA: 0x0004B600 File Offset: 0x00049800
		protected override bool InternalMet(Character iSender)
		{
			float timerValue = base.Scene.Level.GetTimerValue(this.mID);
			switch (this.mCompareMethod)
			{
			case CompareMethod.LESS:
				return this.mValue > timerValue;
			case CompareMethod.EQUAL:
				return this.mValue == timerValue;
			case CompareMethod.GREATER:
				return this.mValue < timerValue;
			default:
				return false;
			}
		}

		// Token: 0x17000300 RID: 768
		// (get) Token: 0x06000CB8 RID: 3256 RVA: 0x0004B660 File Offset: 0x00049860
		// (set) Token: 0x06000CB9 RID: 3257 RVA: 0x0004B668 File Offset: 0x00049868
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

		// Token: 0x17000301 RID: 769
		// (get) Token: 0x06000CBA RID: 3258 RVA: 0x0004B682 File Offset: 0x00049882
		// (set) Token: 0x06000CBB RID: 3259 RVA: 0x0004B68A File Offset: 0x0004988A
		public CompareMethod CompareMethod
		{
			get
			{
				return this.mCompareMethod;
			}
			set
			{
				this.mCompareMethod = value;
			}
		}

		// Token: 0x17000302 RID: 770
		// (get) Token: 0x06000CBC RID: 3260 RVA: 0x0004B693 File Offset: 0x00049893
		// (set) Token: 0x06000CBD RID: 3261 RVA: 0x0004B69B File Offset: 0x0004989B
		public float Value
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

		// Token: 0x04000B9A RID: 2970
		private string mName;

		// Token: 0x04000B9B RID: 2971
		private int mID;

		// Token: 0x04000B9C RID: 2972
		private CompareMethod mCompareMethod;

		// Token: 0x04000B9D RID: 2973
		private float mValue;
	}
}
