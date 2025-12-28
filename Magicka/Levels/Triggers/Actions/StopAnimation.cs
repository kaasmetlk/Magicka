using System;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x0200013F RID: 319
	public class StopAnimation : Action
	{
		// Token: 0x060008FC RID: 2300 RVA: 0x00039435 File Offset: 0x00037635
		public StopAnimation(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x060008FD RID: 2301 RVA: 0x00039448 File Offset: 0x00037648
		protected override void Execute()
		{
			AnimatedLevelPart animatedLevelPart = base.GameScene.LevelModel.GetAnimatedLevelPart(this.mAnimationID[0]);
			for (int i = 1; i < this.mAnimationID.Length; i++)
			{
				animatedLevelPart = animatedLevelPart.GetChild(this.mAnimationID[i]);
			}
			animatedLevelPart.Stop(this.mStopAll);
		}

		// Token: 0x060008FE RID: 2302 RVA: 0x0003949C File Offset: 0x0003769C
		public override void QuickExecute()
		{
		}

		// Token: 0x170001CD RID: 461
		// (get) Token: 0x060008FF RID: 2303 RVA: 0x0003949E File Offset: 0x0003769E
		// (set) Token: 0x06000900 RID: 2304 RVA: 0x000394A8 File Offset: 0x000376A8
		public string Name
		{
			get
			{
				return this.mAnimationName;
			}
			set
			{
				this.mAnimationName = value;
				string[] array = this.mAnimationName.Split(new char[]
				{
					'/'
				});
				this.mAnimationID = new int[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					this.mAnimationID[i] = array[i].GetHashCodeCustom();
				}
			}
		}

		// Token: 0x170001CE RID: 462
		// (get) Token: 0x06000901 RID: 2305 RVA: 0x00039501 File Offset: 0x00037701
		// (set) Token: 0x06000902 RID: 2306 RVA: 0x00039509 File Offset: 0x00037709
		public bool Children
		{
			get
			{
				return this.mStopAll;
			}
			set
			{
				this.mStopAll = value;
			}
		}

		// Token: 0x04000868 RID: 2152
		private string mAnimationName;

		// Token: 0x04000869 RID: 2153
		private bool mStopAll = true;

		// Token: 0x0400086A RID: 2154
		private int[] mAnimationID;
	}
}
