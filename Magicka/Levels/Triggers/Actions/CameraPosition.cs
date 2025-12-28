using System;
using Magicka.Graphics;
using Microsoft.Xna.Framework;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x020005CF RID: 1487
	internal class CameraPosition : Action
	{
		// Token: 0x06002C71 RID: 11377 RVA: 0x0015DB55 File Offset: 0x0015BD55
		public CameraPosition(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x06002C72 RID: 11378 RVA: 0x0015DB6A File Offset: 0x0015BD6A
		public override void Initialize()
		{
			base.Initialize();
			this.mCamera = base.GameScene.PlayState.Camera;
		}

		// Token: 0x06002C73 RID: 11379 RVA: 0x0015DB88 File Offset: 0x0015BD88
		protected override void Execute()
		{
			Matrix matrix;
			Vector3 vector;
			if (base.GameScene.TryGetLocator(this.mTargetHash, out matrix))
			{
				vector = matrix.Translation;
			}
			else
			{
				vector = this.mCamera.GroundPosition;
			}
			Vector3.Add(ref vector, ref this.mOffset, out vector);
			this.mCamera.SetPosition(ref vector);
			if (this.mMagnification > 0f)
			{
				this.mCamera.Magnification = this.mMagnification;
			}
		}

		// Token: 0x06002C74 RID: 11380 RVA: 0x0015DBFA File Offset: 0x0015BDFA
		public override void QuickExecute()
		{
		}

		// Token: 0x17000A7A RID: 2682
		// (get) Token: 0x06002C75 RID: 11381 RVA: 0x0015DBFC File Offset: 0x0015BDFC
		// (set) Token: 0x06002C76 RID: 11382 RVA: 0x0015DC04 File Offset: 0x0015BE04
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

		// Token: 0x17000A7B RID: 2683
		// (get) Token: 0x06002C77 RID: 11383 RVA: 0x0015DC0D File Offset: 0x0015BE0D
		// (set) Token: 0x06002C78 RID: 11384 RVA: 0x0015DC15 File Offset: 0x0015BE15
		public string Target
		{
			get
			{
				return this.mTarget;
			}
			set
			{
				this.mTarget = value;
				this.mTargetHash = this.mTarget.ToLowerInvariant().GetHashCodeCustom();
			}
		}

		// Token: 0x17000A7C RID: 2684
		// (get) Token: 0x06002C79 RID: 11385 RVA: 0x0015DC34 File Offset: 0x0015BE34
		// (set) Token: 0x06002C7A RID: 11386 RVA: 0x0015DC3C File Offset: 0x0015BE3C
		public Vector3 Offset
		{
			get
			{
				return this.mOffset;
			}
			set
			{
				this.mOffset = value;
			}
		}

		// Token: 0x04003007 RID: 12295
		private string mTarget;

		// Token: 0x04003008 RID: 12296
		private int mTargetHash;

		// Token: 0x04003009 RID: 12297
		private Vector3 mOffset;

		// Token: 0x0400300A RID: 12298
		private float mMagnification = -1f;

		// Token: 0x0400300B RID: 12299
		private MagickCamera mCamera;
	}
}
