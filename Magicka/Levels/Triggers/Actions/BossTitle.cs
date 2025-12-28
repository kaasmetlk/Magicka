using System;
using Magicka.GameLogic.Entities.Bosses;
using Magicka.Localization;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x02000487 RID: 1159
	public class BossTitle : Action
	{
		// Token: 0x0600230B RID: 8971 RVA: 0x000FBAF5 File Offset: 0x000F9CF5
		public BossTitle(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x0600230C RID: 8972 RVA: 0x000FBB20 File Offset: 0x000F9D20
		public override void Initialize()
		{
			base.Initialize();
			if (this.mBossTitleHash != 0)
			{
				this.mBossTitle = LanguageManager.Instance.GetString(this.mBossTitleHash).ToUpper();
			}
			if (this.mBossSubTitleHash != 0)
			{
				this.mBossSubTitle = LanguageManager.Instance.GetString(this.mBossSubTitleHash).ToUpper();
			}
		}

		// Token: 0x0600230D RID: 8973 RVA: 0x000FBB7C File Offset: 0x000F9D7C
		protected override void Execute()
		{
			if (this.mBossTitleHash != 0 || this.mBossSubTitleHash != 0)
			{
				if (this.mTitleDisplayTime < 0f)
				{
					this.mTitleDisplayTime = 2f;
				}
				BossFight.Instance.SetTitles((this.mBossTitleHash == 0) ? "" : this.mBossTitle, (this.mBossSubTitleHash == 0) ? "" : this.mBossSubTitle, this.mTitleDisplayTime, this.mFadeIn, this.mFadeOut);
			}
		}

		// Token: 0x0600230E RID: 8974 RVA: 0x000FBBF7 File Offset: 0x000F9DF7
		public override void QuickExecute()
		{
		}

		// Token: 0x17000852 RID: 2130
		// (get) Token: 0x0600230F RID: 8975 RVA: 0x000FBBF9 File Offset: 0x000F9DF9
		// (set) Token: 0x06002310 RID: 8976 RVA: 0x000FBC01 File Offset: 0x000F9E01
		public string Title
		{
			get
			{
				return this.mBossTitle;
			}
			set
			{
				this.mBossTitle = value;
				this.mBossTitleHash = this.mBossTitle.GetHashCodeCustom();
			}
		}

		// Token: 0x17000853 RID: 2131
		// (get) Token: 0x06002311 RID: 8977 RVA: 0x000FBC1B File Offset: 0x000F9E1B
		// (set) Token: 0x06002312 RID: 8978 RVA: 0x000FBC23 File Offset: 0x000F9E23
		public string SubTitle
		{
			get
			{
				return this.mBossSubTitle;
			}
			set
			{
				this.mBossSubTitle = value;
				this.mBossSubTitleHash = this.mBossSubTitle.GetHashCodeCustom();
			}
		}

		// Token: 0x17000854 RID: 2132
		// (get) Token: 0x06002313 RID: 8979 RVA: 0x000FBC3D File Offset: 0x000F9E3D
		// (set) Token: 0x06002314 RID: 8980 RVA: 0x000FBC45 File Offset: 0x000F9E45
		public float DisplayTime
		{
			get
			{
				return this.mTitleDisplayTime;
			}
			set
			{
				this.mTitleDisplayTime = value;
			}
		}

		// Token: 0x17000855 RID: 2133
		// (get) Token: 0x06002315 RID: 8981 RVA: 0x000FBC4E File Offset: 0x000F9E4E
		// (set) Token: 0x06002316 RID: 8982 RVA: 0x000FBC56 File Offset: 0x000F9E56
		public float FadeIn
		{
			get
			{
				return this.mFadeIn;
			}
			set
			{
				this.mFadeIn = value;
			}
		}

		// Token: 0x17000856 RID: 2134
		// (get) Token: 0x06002317 RID: 8983 RVA: 0x000FBC5F File Offset: 0x000F9E5F
		// (set) Token: 0x06002318 RID: 8984 RVA: 0x000FBC67 File Offset: 0x000F9E67
		public float FadeOut
		{
			get
			{
				return this.mFadeOut;
			}
			set
			{
				this.mFadeOut = value;
			}
		}

		// Token: 0x04002621 RID: 9761
		private string mBossTitle;

		// Token: 0x04002622 RID: 9762
		private int mBossTitleHash;

		// Token: 0x04002623 RID: 9763
		private string mBossSubTitle;

		// Token: 0x04002624 RID: 9764
		private int mBossSubTitleHash;

		// Token: 0x04002625 RID: 9765
		private float mTitleDisplayTime = -1f;

		// Token: 0x04002626 RID: 9766
		private float mFadeOut = 1f;

		// Token: 0x04002627 RID: 9767
		private float mFadeIn = 1f;
	}
}
