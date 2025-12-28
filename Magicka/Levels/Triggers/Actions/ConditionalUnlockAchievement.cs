using System;
using Magicka.Achievements;
using Magicka.GameLogic.Entities;
using Magicka.Gamers;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x0200041C RID: 1052
	internal class ConditionalUnlockAchievement : Action
	{
		// Token: 0x0600208E RID: 8334 RVA: 0x000E6D93 File Offset: 0x000E4F93
		public ConditionalUnlockAchievement(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x0600208F RID: 8335 RVA: 0x000E6DA0 File Offset: 0x000E4FA0
		protected override void Execute()
		{
			bool flag = false;
			if (this.mAchievement.Equals("thisismagicka", StringComparison.InvariantCultureIgnoreCase))
			{
				NonPlayerCharacter nonPlayerCharacter = Entity.GetByID(this.mID) as NonPlayerCharacter;
				if (nonPlayerCharacter != null)
				{
					Avatar avatar = nonPlayerCharacter.LastAttacker as Avatar;
					if (avatar != null && avatar.Player.Playing && !(avatar.Player.Gamer is NetworkGamer))
					{
						flag = true;
					}
				}
			}
			else if (this.mAchievement.Equals("therecanbeonlyone", StringComparison.InvariantCultureIgnoreCase))
			{
				NonPlayerCharacter nonPlayerCharacter2 = Entity.GetByID(this.mID) as NonPlayerCharacter;
				if (nonPlayerCharacter2 != null)
				{
					Avatar avatar2 = nonPlayerCharacter2.LastAttacker as Avatar;
					if (avatar2 != null && avatar2.Player.Playing && !(avatar2.Player.Gamer is NetworkGamer))
					{
						flag = true;
					}
				}
			}
			else
			{
				if (!this.mAchievement.Equals("drivenmad", StringComparison.InvariantCultureIgnoreCase) && !this.mAchievement.Equals("breezedthrough", StringComparison.InvariantCultureIgnoreCase) && !this.mAchievement.Equals("handlingthefrustration", StringComparison.InvariantCultureIgnoreCase))
				{
					throw new Exception("Not a implemented conditional Achievement");
				}
				if (!this.mScene.PlayState.DiedInLevel)
				{
					flag = true;
				}
			}
			if (flag)
			{
				AchievementsManager.Instance.AwardAchievement(base.GameScene.PlayState, this.mAchievement);
			}
		}

		// Token: 0x06002090 RID: 8336 RVA: 0x000E6EEF File Offset: 0x000E50EF
		public override void QuickExecute()
		{
		}

		// Token: 0x170007F9 RID: 2041
		// (get) Token: 0x06002091 RID: 8337 RVA: 0x000E6EF1 File Offset: 0x000E50F1
		// (set) Token: 0x06002092 RID: 8338 RVA: 0x000E6EF9 File Offset: 0x000E50F9
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

		// Token: 0x170007FA RID: 2042
		// (get) Token: 0x06002093 RID: 8339 RVA: 0x000E6F02 File Offset: 0x000E5102
		// (set) Token: 0x06002094 RID: 8340 RVA: 0x000E6F0A File Offset: 0x000E510A
		public string Victim
		{
			get
			{
				return this.mVictim;
			}
			set
			{
				this.mVictim = value;
				this.mID = this.mVictim.GetHashCodeCustom();
			}
		}

		// Token: 0x04002309 RID: 8969
		private string mAchievement;

		// Token: 0x0400230A RID: 8970
		private string mVictim;

		// Token: 0x0400230B RID: 8971
		private int mID;
	}
}
