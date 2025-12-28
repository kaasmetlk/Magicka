using System;
using Microsoft.Xna.Framework;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x0200023B RID: 571
	public class CameraShake : Action
	{
		// Token: 0x0600117E RID: 4478 RVA: 0x0006BF34 File Offset: 0x0006A134
		public CameraShake(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x0600117F RID: 4479 RVA: 0x0006BF40 File Offset: 0x0006A140
		protected override void Execute()
		{
			if (string.IsNullOrEmpty(this.mArea))
			{
				base.GameScene.PlayState.Camera.CameraShake(this.mMagnitude, this.mTime);
				return;
			}
			Matrix matrix;
			base.GameScene.GetLocator(this.mAreaID, out matrix);
			base.GameScene.PlayState.Camera.CameraShake(matrix.Translation, this.mMagnitude, this.mTime);
		}

		// Token: 0x06001180 RID: 4480 RVA: 0x0006BFB7 File Offset: 0x0006A1B7
		public override void QuickExecute()
		{
		}

		// Token: 0x17000477 RID: 1143
		// (get) Token: 0x06001181 RID: 4481 RVA: 0x0006BFB9 File Offset: 0x0006A1B9
		// (set) Token: 0x06001182 RID: 4482 RVA: 0x0006BFC1 File Offset: 0x0006A1C1
		public string Area
		{
			get
			{
				return this.mArea;
			}
			set
			{
				this.mArea = value;
				this.mAreaID = this.mArea.GetHashCodeCustom();
			}
		}

		// Token: 0x17000478 RID: 1144
		// (get) Token: 0x06001183 RID: 4483 RVA: 0x0006BFDB File Offset: 0x0006A1DB
		// (set) Token: 0x06001184 RID: 4484 RVA: 0x0006BFE3 File Offset: 0x0006A1E3
		public float Magnitude
		{
			get
			{
				return this.mMagnitude;
			}
			set
			{
				this.mMagnitude = value;
			}
		}

		// Token: 0x17000479 RID: 1145
		// (get) Token: 0x06001185 RID: 4485 RVA: 0x0006BFEC File Offset: 0x0006A1EC
		// (set) Token: 0x06001186 RID: 4486 RVA: 0x0006BFF4 File Offset: 0x0006A1F4
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

		// Token: 0x04001066 RID: 4198
		private float mMagnitude;

		// Token: 0x04001067 RID: 4199
		private float mTime;

		// Token: 0x04001068 RID: 4200
		private string mArea;

		// Token: 0x04001069 RID: 4201
		private int mAreaID;
	}
}
