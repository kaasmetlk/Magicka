using System;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x02000140 RID: 320
	internal class CameraRelease : Action
	{
		// Token: 0x06000903 RID: 2307 RVA: 0x00039512 File Offset: 0x00037712
		public CameraRelease(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x06000904 RID: 2308 RVA: 0x00039527 File Offset: 0x00037727
		protected override void Execute()
		{
			base.GameScene.PlayState.Camera.Release(this.mTime);
		}

		// Token: 0x06000905 RID: 2309 RVA: 0x00039544 File Offset: 0x00037744
		public override void QuickExecute()
		{
		}

		// Token: 0x170001CF RID: 463
		// (get) Token: 0x06000906 RID: 2310 RVA: 0x00039546 File Offset: 0x00037746
		// (set) Token: 0x06000907 RID: 2311 RVA: 0x0003954E File Offset: 0x0003774E
		public float Time
		{
			get
			{
				return this.mTime;
			}
			set
			{
				this.mTime = value;
			}
		}

		// Token: 0x0400086B RID: 2155
		private float mTime = 1f;
	}
}
