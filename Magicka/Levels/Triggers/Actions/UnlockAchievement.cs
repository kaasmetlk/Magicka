using System;
using Magicka.Achievements;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x020003E5 RID: 997
	internal class UnlockAchievement : Action
	{
		// Token: 0x06001E95 RID: 7829 RVA: 0x000D5F3D File Offset: 0x000D413D
		public UnlockAchievement(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x06001E96 RID: 7830 RVA: 0x000D5F47 File Offset: 0x000D4147
		protected override void Execute()
		{
			AchievementsManager.Instance.AwardAchievement(base.GameScene.PlayState, this.mAchievement);
		}

		// Token: 0x06001E97 RID: 7831 RVA: 0x000D5F64 File Offset: 0x000D4164
		public override void QuickExecute()
		{
		}

		// Token: 0x1700077B RID: 1915
		// (get) Token: 0x06001E98 RID: 7832 RVA: 0x000D5F66 File Offset: 0x000D4166
		// (set) Token: 0x06001E99 RID: 7833 RVA: 0x000D5F6E File Offset: 0x000D416E
		public string Achievement
		{
			get
			{
				return this.mAchievement;
			}
			set
			{
				this.mAchievement = value;
			}
		}

		// Token: 0x040020ED RID: 8429
		private string mAchievement;
	}
}
