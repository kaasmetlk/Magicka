using System;
using JigLibX.Geometry;
using Magicka.Graphics;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x020004F5 RID: 1269
	internal class CameraLock : Action
	{
		// Token: 0x0600258A RID: 9610 RVA: 0x001112F0 File Offset: 0x0010F4F0
		public CameraLock(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
			this.mScene = iScene;
		}

		// Token: 0x0600258B RID: 9611 RVA: 0x0011130C File Offset: 0x0010F50C
		public override void Initialize()
		{
			this.mCamera = base.GameScene.PlayState.Camera;
		}

		// Token: 0x0600258C RID: 9612 RVA: 0x00111324 File Offset: 0x0010F524
		protected override void Execute()
		{
			this.mCamera.LockOn(this.mScene.GetTriggerArea(this.mTargetID).CollisionSkin.GetPrimitiveLocal(0) as Box, this.mTime);
		}

		// Token: 0x0600258D RID: 9613 RVA: 0x00111358 File Offset: 0x0010F558
		public override void QuickExecute()
		{
		}

		// Token: 0x170008BB RID: 2235
		// (get) Token: 0x0600258E RID: 9614 RVA: 0x0011135A File Offset: 0x0010F55A
		// (set) Token: 0x0600258F RID: 9615 RVA: 0x00111362 File Offset: 0x0010F562
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

		// Token: 0x170008BC RID: 2236
		// (get) Token: 0x06002590 RID: 9616 RVA: 0x0011136B File Offset: 0x0010F56B
		// (set) Token: 0x06002591 RID: 9617 RVA: 0x00111373 File Offset: 0x0010F573
		public string Area
		{
			get
			{
				return this.mTarget;
			}
			set
			{
				this.mTarget = value;
				this.mTargetID = this.mTarget.GetHashCodeCustom();
				if (string.IsNullOrEmpty(this.mTarget))
				{
					throw new Exception("Must input area");
				}
			}
		}

		// Token: 0x040028F9 RID: 10489
		private float mTime = 1f;

		// Token: 0x040028FA RID: 10490
		private string mTarget;

		// Token: 0x040028FB RID: 10491
		private int mTargetID;

		// Token: 0x040028FC RID: 10492
		private new GameScene mScene;

		// Token: 0x040028FD RID: 10493
		private MagickCamera mCamera;
	}
}
