using System;
using Magicka.GameLogic.UI;
using Magicka.Localization;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x020002CC RID: 716
	public class HealthBar : Action
	{
		// Token: 0x060015CD RID: 5581 RVA: 0x0008AFB7 File Offset: 0x000891B7
		public HealthBar(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x060015CE RID: 5582 RVA: 0x0008AFE2 File Offset: 0x000891E2
		public override void Initialize()
		{
			base.Initialize();
			if (this.mDisplayNameHash != 0)
			{
				this.mDisplayName = LanguageManager.Instance.GetString(this.mDisplayNameHash);
			}
		}

		// Token: 0x060015CF RID: 5583 RVA: 0x0008B008 File Offset: 0x00089208
		protected override void Execute()
		{
			GenericHealthBar genericHealthBar = base.GameScene.PlayState.GenericHealthBar;
			genericHealthBar.Power = this.mPower;
			genericHealthBar.Alpha = this.mAlpha;
			genericHealthBar.Type = this.mType;
			genericHealthBar.NormalizedHealth = this.mNormalizedHealth;
			genericHealthBar.DisplayHealth = this.mDisplayHealth;
			genericHealthBar.InitialTimerDelay = this.mInitialTimerDelay;
			genericHealthBar.IsColoredRed = this.mIsColoredRed;
			genericHealthBar.IsScaled = this.mIsScaled;
			genericHealthBar.ShowDisplayName = this.mShowDisplayName;
			genericHealthBar.DisplayName = this.mDisplayName;
			if (this.mType == GenericHealthBarTypes.TimerDecreasing || this.mType == GenericHealthBarTypes.TimerIncreasing)
			{
				genericHealthBar.SetupTimer(this.mTime);
			}
			else
			{
				genericHealthBar.SetupCounter(this.mCount);
			}
			if (this.mDisplayNameHash != 0)
			{
				genericHealthBar.DisplayName = this.mDisplayName;
			}
			genericHealthBar.GraphicsType = this.mGraphicsType;
			genericHealthBar.HasAnimatedSprite = this.mHasAnimatedSprite;
			genericHealthBar.AnimationSpriteOffsetY = this.mAnimationSpriteOffsetY;
			genericHealthBar.FadeTime = this.mFadeTime;
			genericHealthBar.OnEndTriggerID = this.mOnEndTriggerID;
		}

		// Token: 0x060015D0 RID: 5584 RVA: 0x0008B11A File Offset: 0x0008931A
		public override void QuickExecute()
		{
			this.Execute();
		}

		// Token: 0x17000583 RID: 1411
		// (get) Token: 0x060015D1 RID: 5585 RVA: 0x0008B122 File Offset: 0x00089322
		// (set) Token: 0x060015D2 RID: 5586 RVA: 0x0008B12A File Offset: 0x0008932A
		public string DisplayName
		{
			get
			{
				return this.mDisplayName;
			}
			set
			{
				this.mDisplayName = value;
				this.mDisplayNameHash = this.mDisplayName.GetHashCodeCustom();
			}
		}

		// Token: 0x17000584 RID: 1412
		// (get) Token: 0x060015D3 RID: 5587 RVA: 0x0008B144 File Offset: 0x00089344
		// (set) Token: 0x060015D4 RID: 5588 RVA: 0x0008B14C File Offset: 0x0008934C
		public float NormalizedHealth
		{
			get
			{
				return this.mNormalizedHealth;
			}
			set
			{
				this.mNormalizedHealth = value;
			}
		}

		// Token: 0x17000585 RID: 1413
		// (get) Token: 0x060015D5 RID: 5589 RVA: 0x0008B155 File Offset: 0x00089355
		// (set) Token: 0x060015D6 RID: 5590 RVA: 0x0008B15D File Offset: 0x0008935D
		public float DisplayHealth
		{
			get
			{
				return this.mDisplayHealth;
			}
			set
			{
				this.mDisplayHealth = value;
			}
		}

		// Token: 0x17000586 RID: 1414
		// (get) Token: 0x060015D7 RID: 5591 RVA: 0x0008B166 File Offset: 0x00089366
		// (set) Token: 0x060015D8 RID: 5592 RVA: 0x0008B16E File Offset: 0x0008936E
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

		// Token: 0x17000587 RID: 1415
		// (get) Token: 0x060015D9 RID: 5593 RVA: 0x0008B177 File Offset: 0x00089377
		// (set) Token: 0x060015DA RID: 5594 RVA: 0x0008B17F File Offset: 0x0008937F
		public GenericHealthBarTypes Type
		{
			get
			{
				return this.mType;
			}
			set
			{
				this.mType = value;
			}
		}

		// Token: 0x17000588 RID: 1416
		// (get) Token: 0x060015DB RID: 5595 RVA: 0x0008B188 File Offset: 0x00089388
		// (set) Token: 0x060015DC RID: 5596 RVA: 0x0008B190 File Offset: 0x00089390
		public float Power
		{
			get
			{
				return this.mPower;
			}
			set
			{
				this.mPower = value;
			}
		}

		// Token: 0x17000589 RID: 1417
		// (get) Token: 0x060015DD RID: 5597 RVA: 0x0008B199 File Offset: 0x00089399
		// (set) Token: 0x060015DE RID: 5598 RVA: 0x0008B1A1 File Offset: 0x000893A1
		public float Alpha
		{
			get
			{
				return this.mAlpha;
			}
			set
			{
				this.mAlpha = value;
			}
		}

		// Token: 0x1700058A RID: 1418
		// (get) Token: 0x060015DF RID: 5599 RVA: 0x0008B1AA File Offset: 0x000893AA
		// (set) Token: 0x060015E0 RID: 5600 RVA: 0x0008B1B2 File Offset: 0x000893B2
		public float Count
		{
			get
			{
				return this.mCount;
			}
			set
			{
				this.mCount = value;
			}
		}

		// Token: 0x1700058B RID: 1419
		// (get) Token: 0x060015E1 RID: 5601 RVA: 0x0008B1BB File Offset: 0x000893BB
		// (set) Token: 0x060015E2 RID: 5602 RVA: 0x0008B1C3 File Offset: 0x000893C3
		public GenericHealthBarGraphics GraphicsType
		{
			get
			{
				return this.mGraphicsType;
			}
			set
			{
				this.mGraphicsType = value;
			}
		}

		// Token: 0x1700058C RID: 1420
		// (get) Token: 0x060015E3 RID: 5603 RVA: 0x0008B1CC File Offset: 0x000893CC
		// (set) Token: 0x060015E4 RID: 5604 RVA: 0x0008B1D4 File Offset: 0x000893D4
		public float InitialTimerDelay
		{
			get
			{
				return this.mInitialTimerDelay;
			}
			set
			{
				this.mInitialTimerDelay = value;
			}
		}

		// Token: 0x1700058D RID: 1421
		// (get) Token: 0x060015E5 RID: 5605 RVA: 0x0008B1DD File Offset: 0x000893DD
		// (set) Token: 0x060015E6 RID: 5606 RVA: 0x0008B1E5 File Offset: 0x000893E5
		public bool IsScaled
		{
			get
			{
				return this.mIsScaled;
			}
			set
			{
				this.mIsScaled = value;
			}
		}

		// Token: 0x1700058E RID: 1422
		// (get) Token: 0x060015E7 RID: 5607 RVA: 0x0008B1EE File Offset: 0x000893EE
		// (set) Token: 0x060015E8 RID: 5608 RVA: 0x0008B1F6 File Offset: 0x000893F6
		public bool IsColoredRed
		{
			get
			{
				return this.mIsColoredRed;
			}
			set
			{
				this.mIsColoredRed = value;
			}
		}

		// Token: 0x1700058F RID: 1423
		// (get) Token: 0x060015E9 RID: 5609 RVA: 0x0008B1FF File Offset: 0x000893FF
		// (set) Token: 0x060015EA RID: 5610 RVA: 0x0008B207 File Offset: 0x00089407
		public bool ShowDisplayName
		{
			get
			{
				return this.mShowDisplayName;
			}
			set
			{
				this.mShowDisplayName = value;
			}
		}

		// Token: 0x17000590 RID: 1424
		// (get) Token: 0x060015EB RID: 5611 RVA: 0x0008B210 File Offset: 0x00089410
		// (set) Token: 0x060015EC RID: 5612 RVA: 0x0008B218 File Offset: 0x00089418
		public bool HasAnimatedSprite
		{
			get
			{
				return this.mHasAnimatedSprite;
			}
			set
			{
				this.mHasAnimatedSprite = value;
			}
		}

		// Token: 0x17000591 RID: 1425
		// (get) Token: 0x060015ED RID: 5613 RVA: 0x0008B221 File Offset: 0x00089421
		// (set) Token: 0x060015EE RID: 5614 RVA: 0x0008B229 File Offset: 0x00089429
		public float AnimationOffsetPositionY
		{
			get
			{
				return this.mAnimationSpriteOffsetY;
			}
			set
			{
				this.mAnimationSpriteOffsetY = value;
			}
		}

		// Token: 0x17000592 RID: 1426
		// (get) Token: 0x060015EF RID: 5615 RVA: 0x0008B232 File Offset: 0x00089432
		// (set) Token: 0x060015F0 RID: 5616 RVA: 0x0008B23A File Offset: 0x0008943A
		public float FadeTime
		{
			get
			{
				return this.mFadeTime;
			}
			set
			{
				this.mFadeTime = value;
			}
		}

		// Token: 0x17000593 RID: 1427
		// (get) Token: 0x060015F1 RID: 5617 RVA: 0x0008B243 File Offset: 0x00089443
		// (set) Token: 0x060015F2 RID: 5618 RVA: 0x0008B24B File Offset: 0x0008944B
		public string OnEndTrigger
		{
			get
			{
				return this.mOnEndTrigger;
			}
			set
			{
				this.mOnEndTrigger = value;
				this.mOnEndTriggerID = this.mOnEndTrigger.GetHashCodeCustom();
			}
		}

		// Token: 0x04001717 RID: 5911
		private int mDisplayNameHash;

		// Token: 0x04001718 RID: 5912
		private string mDisplayName;

		// Token: 0x04001719 RID: 5913
		private bool mShowDisplayName;

		// Token: 0x0400171A RID: 5914
		private float mNormalizedHealth = 1f;

		// Token: 0x0400171B RID: 5915
		private float mPower = 1.1f;

		// Token: 0x0400171C RID: 5916
		private float mAlpha;

		// Token: 0x0400171D RID: 5917
		private float mTime;

		// Token: 0x0400171E RID: 5918
		private float mDisplayHealth;

		// Token: 0x0400171F RID: 5919
		private float mCount;

		// Token: 0x04001720 RID: 5920
		private float mInitialTimerDelay;

		// Token: 0x04001721 RID: 5921
		private GenericHealthBarTypes mType;

		// Token: 0x04001722 RID: 5922
		private GenericHealthBarGraphics mGraphicsType;

		// Token: 0x04001723 RID: 5923
		private bool mIsScaled;

		// Token: 0x04001724 RID: 5924
		private bool mIsColoredRed;

		// Token: 0x04001725 RID: 5925
		private bool mHasAnimatedSprite;

		// Token: 0x04001726 RID: 5926
		private float mAnimationSpriteOffsetY = 16f;

		// Token: 0x04001727 RID: 5927
		private float mFadeTime;

		// Token: 0x04001728 RID: 5928
		private string mOnEndTrigger;

		// Token: 0x04001729 RID: 5929
		private int mOnEndTriggerID;
	}
}
