using System;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x0200023D RID: 573
	internal class Timer : Action
	{
		// Token: 0x06001197 RID: 4503 RVA: 0x0006C156 File Offset: 0x0006A356
		public Timer(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x06001198 RID: 4504 RVA: 0x0006C160 File Offset: 0x0006A360
		protected override void Execute()
		{
			if (this.mValue != null)
			{
				base.GameScene.Level.SetTimer(this.mID, this.mValue.Value);
			}
			if (this.mPause != null)
			{
				base.GameScene.Level.PauseTimer(this.mID, this.mPause.Value);
			}
		}

		// Token: 0x06001199 RID: 4505 RVA: 0x0006C1C9 File Offset: 0x0006A3C9
		public override void QuickExecute()
		{
			this.Execute();
		}

		// Token: 0x17000480 RID: 1152
		// (get) Token: 0x0600119A RID: 4506 RVA: 0x0006C1D1 File Offset: 0x0006A3D1
		// (set) Token: 0x0600119B RID: 4507 RVA: 0x0006C1D9 File Offset: 0x0006A3D9
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

		// Token: 0x17000481 RID: 1153
		// (get) Token: 0x0600119C RID: 4508 RVA: 0x0006C1F3 File Offset: 0x0006A3F3
		// (set) Token: 0x0600119D RID: 4509 RVA: 0x0006C200 File Offset: 0x0006A400
		public float Value
		{
			get
			{
				return this.mValue.Value;
			}
			set
			{
				this.mValue = new float?(value);
			}
		}

		// Token: 0x17000482 RID: 1154
		// (get) Token: 0x0600119E RID: 4510 RVA: 0x0006C20E File Offset: 0x0006A40E
		// (set) Token: 0x0600119F RID: 4511 RVA: 0x0006C21B File Offset: 0x0006A41B
		public bool Paused
		{
			get
			{
				return this.mPause.Value;
			}
			set
			{
				this.mPause = new bool?(value);
			}
		}

		// Token: 0x04001072 RID: 4210
		private string mName;

		// Token: 0x04001073 RID: 4211
		private int mID;

		// Token: 0x04001074 RID: 4212
		private float? mValue;

		// Token: 0x04001075 RID: 4213
		private bool? mPause;
	}
}
