using System;
using Magicka.Achievements;
using Magicka.GameLogic.Entities;

namespace Magicka.Levels.Triggers.Conditions
{
	// Token: 0x02000018 RID: 24
	internal class HasAchievement : Condition
	{
		// Token: 0x060000B8 RID: 184 RVA: 0x00006C47 File Offset: 0x00004E47
		public HasAchievement(GameScene iScene) : base(iScene)
		{
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x00006C50 File Offset: 0x00004E50
		protected override bool InternalMet(Character iSender)
		{
			return AchievementsManager.Instance.HasAchievement(this.mName);
		}

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x060000BA RID: 186 RVA: 0x00006C62 File Offset: 0x00004E62
		// (set) Token: 0x060000BB RID: 187 RVA: 0x00006C6A File Offset: 0x00004E6A
		public string Name
		{
			get
			{
				return this.mName;
			}
			set
			{
				this.mName = value;
			}
		}

		// Token: 0x04000091 RID: 145
		private string mName;
	}
}
