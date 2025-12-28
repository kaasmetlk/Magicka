using System;
using Magicka.Graphics;
using Microsoft.Xna.Framework;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x0200023C RID: 572
	internal class CameraMove : Action
	{
		// Token: 0x06001187 RID: 4487 RVA: 0x0006BFFD File Offset: 0x0006A1FD
		public CameraMove(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x06001188 RID: 4488 RVA: 0x0006C024 File Offset: 0x0006A224
		public override void Initialize()
		{
			base.Initialize();
			this.mCamera = base.GameScene.PlayState.Camera;
		}

		// Token: 0x06001189 RID: 4489 RVA: 0x0006C044 File Offset: 0x0006A244
		protected override void Execute()
		{
			Matrix matrix;
			Vector3 iTarget;
			if (base.GameScene.TryGetLocator(this.mTargetHash, out matrix))
			{
				iTarget = matrix.Translation;
			}
			else
			{
				iTarget = this.mCamera.GroundPosition;
			}
			Vector3.Add(ref iTarget, ref this.mOffset, out iTarget);
			this.mCamera.MoveTo(iTarget, this.mTime);
			this.mCamera.Interpolation = this.mInterpolation;
			if (this.mMagnification > 0f)
			{
				this.mCamera.Magnification = this.mMagnification;
			}
			this.mCamera.LockInput = this.mLockInput;
		}

		// Token: 0x0600118A RID: 4490 RVA: 0x0006C0DD File Offset: 0x0006A2DD
		public override void QuickExecute()
		{
		}

		// Token: 0x1700047A RID: 1146
		// (get) Token: 0x0600118B RID: 4491 RVA: 0x0006C0DF File Offset: 0x0006A2DF
		// (set) Token: 0x0600118C RID: 4492 RVA: 0x0006C0E7 File Offset: 0x0006A2E7
		public bool LockInput
		{
			get
			{
				return this.mLockInput;
			}
			set
			{
				this.mLockInput = value;
			}
		}

		// Token: 0x1700047B RID: 1147
		// (get) Token: 0x0600118D RID: 4493 RVA: 0x0006C0F0 File Offset: 0x0006A2F0
		// (set) Token: 0x0600118E RID: 4494 RVA: 0x0006C0F8 File Offset: 0x0006A2F8
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

		// Token: 0x1700047C RID: 1148
		// (get) Token: 0x0600118F RID: 4495 RVA: 0x0006C101 File Offset: 0x0006A301
		// (set) Token: 0x06001190 RID: 4496 RVA: 0x0006C109 File Offset: 0x0006A309
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

		// Token: 0x1700047D RID: 1149
		// (get) Token: 0x06001191 RID: 4497 RVA: 0x0006C112 File Offset: 0x0006A312
		// (set) Token: 0x06001192 RID: 4498 RVA: 0x0006C11A File Offset: 0x0006A31A
		public string Target
		{
			get
			{
				return this.mTarget;
			}
			set
			{
				this.mTarget = value;
				this.mTargetHash = this.mTarget.GetHashCodeCustom();
			}
		}

		// Token: 0x1700047E RID: 1150
		// (get) Token: 0x06001193 RID: 4499 RVA: 0x0006C134 File Offset: 0x0006A334
		// (set) Token: 0x06001194 RID: 4500 RVA: 0x0006C13C File Offset: 0x0006A33C
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

		// Token: 0x1700047F RID: 1151
		// (get) Token: 0x06001195 RID: 4501 RVA: 0x0006C145 File Offset: 0x0006A345
		// (set) Token: 0x06001196 RID: 4502 RVA: 0x0006C14D File Offset: 0x0006A34D
		public CameraInterpolation Interpolation
		{
			get
			{
				return this.mInterpolation;
			}
			set
			{
				this.mInterpolation = value;
			}
		}

		// Token: 0x0400106A RID: 4202
		private bool mLockInput;

		// Token: 0x0400106B RID: 4203
		private float mTime = 1f;

		// Token: 0x0400106C RID: 4204
		private string mTarget;

		// Token: 0x0400106D RID: 4205
		private int mTargetHash;

		// Token: 0x0400106E RID: 4206
		private Vector3 mOffset;

		// Token: 0x0400106F RID: 4207
		private float mMagnification = -1f;

		// Token: 0x04001070 RID: 4208
		private CameraInterpolation mInterpolation = CameraInterpolation.Interpolated;

		// Token: 0x04001071 RID: 4209
		private MagickCamera mCamera;
	}
}
