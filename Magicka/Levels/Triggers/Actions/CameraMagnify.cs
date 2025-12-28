using System;
using Magicka.Graphics;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x02000482 RID: 1154
	internal class CameraMagnify : Action
	{
		// Token: 0x060022ED RID: 8941 RVA: 0x000FB858 File Offset: 0x000F9A58
		public CameraMagnify(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x060022EE RID: 8942 RVA: 0x000FB878 File Offset: 0x000F9A78
		public override void Initialize()
		{
			base.Initialize();
			this.mCamera = base.GameScene.PlayState.Camera;
		}

		// Token: 0x060022EF RID: 8943 RVA: 0x000FB896 File Offset: 0x000F9A96
		protected override void Execute()
		{
			this.mCamera.Magnification = this.mMagnification;
			this.mCamera.Time = this.mTime;
		}

		// Token: 0x060022F0 RID: 8944 RVA: 0x000FB8BA File Offset: 0x000F9ABA
		public override void QuickExecute()
		{
		}

		// Token: 0x1700084B RID: 2123
		// (get) Token: 0x060022F1 RID: 8945 RVA: 0x000FB8BC File Offset: 0x000F9ABC
		// (set) Token: 0x060022F2 RID: 8946 RVA: 0x000FB8C4 File Offset: 0x000F9AC4
		public float Magnification
		{
			get
			{
				return this.mMagnification;
			}
			set
			{
				this.mMagnification = value;
			}
		}

		// Token: 0x1700084C RID: 2124
		// (get) Token: 0x060022F3 RID: 8947 RVA: 0x000FB8CD File Offset: 0x000F9ACD
		// (set) Token: 0x060022F4 RID: 8948 RVA: 0x000FB8D5 File Offset: 0x000F9AD5
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

		// Token: 0x04002617 RID: 9751
		private float mMagnification = 1f;

		// Token: 0x04002618 RID: 9752
		private float mTime = 1f;

		// Token: 0x04002619 RID: 9753
		private MagickCamera mCamera;
	}
}
