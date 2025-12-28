using System;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x0200028C RID: 652
	public class AddCameraEffect : Action
	{
		// Token: 0x06001336 RID: 4918 RVA: 0x000763A2 File Offset: 0x000745A2
		public AddCameraEffect(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x06001337 RID: 4919 RVA: 0x000763AC File Offset: 0x000745AC
		protected override void Execute()
		{
			base.GameScene.PlayState.Camera.StartVisualEffect(this.mCameraEffectHash);
		}

		// Token: 0x06001338 RID: 4920 RVA: 0x000763C9 File Offset: 0x000745C9
		public override void QuickExecute()
		{
			this.Execute();
		}

		// Token: 0x170004DF RID: 1247
		// (get) Token: 0x06001339 RID: 4921 RVA: 0x000763D1 File Offset: 0x000745D1
		// (set) Token: 0x0600133A RID: 4922 RVA: 0x000763D9 File Offset: 0x000745D9
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

		// Token: 0x040014DF RID: 5343
		private string mCameraEffectName;

		// Token: 0x040014E0 RID: 5344
		private int mCameraEffectHash;
	}
}
