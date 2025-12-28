using System;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x020003E6 RID: 998
	public class RemoveCameraEffect : Action
	{
		// Token: 0x06001E9A RID: 7834 RVA: 0x000D5F77 File Offset: 0x000D4177
		public RemoveCameraEffect(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x06001E9B RID: 7835 RVA: 0x000D5F81 File Offset: 0x000D4181
		protected override void Execute()
		{
			if (this.mCameraEffectHash == 0)
			{
				base.GameScene.PlayState.Camera.RemoveEffects();
				return;
			}
			base.GameScene.PlayState.Camera.RemoveEffect(this.mCameraEffectHash);
		}

		// Token: 0x06001E9C RID: 7836 RVA: 0x000D5FBC File Offset: 0x000D41BC
		public override void QuickExecute()
		{
			this.Execute();
		}

		// Token: 0x1700077C RID: 1916
		// (get) Token: 0x06001E9D RID: 7837 RVA: 0x000D5FC4 File Offset: 0x000D41C4
		// (set) Token: 0x06001E9E RID: 7838 RVA: 0x000D5FCC File Offset: 0x000D41CC
		public string Effect
		{
			get
			{
				return this.mCameraEffectName;
			}
			set
			{
				this.mCameraEffectName = value;
				this.mCameraEffectHash = this.mCameraEffectName.GetHashCodeCustom();
			}
		}

		// Token: 0x040020EE RID: 8430
		private string mCameraEffectName;

		// Token: 0x040020EF RID: 8431
		private int mCameraEffectHash;
	}
}
