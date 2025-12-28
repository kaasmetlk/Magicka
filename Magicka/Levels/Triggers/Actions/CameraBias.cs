using System;
using Microsoft.Xna.Framework;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x020000BB RID: 187
	internal class CameraBias : Action
	{
		// Token: 0x06000571 RID: 1393 RVA: 0x0002045B File Offset: 0x0001E65B
		public CameraBias(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x06000572 RID: 1394 RVA: 0x00020470 File Offset: 0x0001E670
		protected override void Execute()
		{
			base.GameScene.PlayState.Camera.SetBias(ref this.mBias, this.mTime);
		}

		// Token: 0x06000573 RID: 1395 RVA: 0x00020493 File Offset: 0x0001E693
		public override void QuickExecute()
		{
		}

		// Token: 0x170000E8 RID: 232
		// (get) Token: 0x06000574 RID: 1396 RVA: 0x00020495 File Offset: 0x0001E695
		// (set) Token: 0x06000575 RID: 1397 RVA: 0x0002049D File Offset: 0x0001E69D
		public Vector3 Bias
		{
			get
			{
				return this.mBias;
			}
			set
			{
				this.mBias = value;
			}
		}

		// Token: 0x170000E9 RID: 233
		// (get) Token: 0x06000576 RID: 1398 RVA: 0x000204A6 File Offset: 0x0001E6A6
		// (set) Token: 0x06000577 RID: 1399 RVA: 0x000204AE File Offset: 0x0001E6AE
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

		// Token: 0x04000437 RID: 1079
		private Vector3 mBias;

		// Token: 0x04000438 RID: 1080
		private float mTime = 20f;
	}
}
