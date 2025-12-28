using System;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x02000146 RID: 326
	internal class AddTimer : Action
	{
		// Token: 0x0600092D RID: 2349 RVA: 0x0003988C File Offset: 0x00037A8C
		public AddTimer(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x0600092E RID: 2350 RVA: 0x00039896 File Offset: 0x00037A96
		protected override void Execute()
		{
			base.GameScene.Level.AddTimer(this.mID, this.mPlayedTimer, this.mValue);
		}

		// Token: 0x0600092F RID: 2351 RVA: 0x000398BA File Offset: 0x00037ABA
		public override void QuickExecute()
		{
			this.Execute();
		}

		// Token: 0x170001DB RID: 475
		// (get) Token: 0x06000930 RID: 2352 RVA: 0x000398C2 File Offset: 0x00037AC2
		// (set) Token: 0x06000931 RID: 2353 RVA: 0x000398CA File Offset: 0x00037ACA
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

		// Token: 0x170001DC RID: 476
		// (get) Token: 0x06000932 RID: 2354 RVA: 0x000398E4 File Offset: 0x00037AE4
		// (set) Token: 0x06000933 RID: 2355 RVA: 0x000398EC File Offset: 0x00037AEC
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

		// Token: 0x170001DD RID: 477
		// (get) Token: 0x06000934 RID: 2356 RVA: 0x000398F5 File Offset: 0x00037AF5
		// (set) Token: 0x06000935 RID: 2357 RVA: 0x000398FD File Offset: 0x00037AFD
		public bool PlayedTimer
		{
			get
			{
				return this.mPlayedTimer;
			}
			set
			{
				this.mPlayedTimer = value;
			}
		}

		// Token: 0x0400087B RID: 2171
		private string mName;

		// Token: 0x0400087C RID: 2172
		private int mID;

		// Token: 0x0400087D RID: 2173
		private float mValue;

		// Token: 0x0400087E RID: 2174
		private bool mPlayedTimer;
	}
}
