using System;
using Magicka.Localization;
using PolygonHead;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x020000BD RID: 189
	public class DisplayTitleInLevel : Action
	{
		// Token: 0x170000EA RID: 234
		// (get) Token: 0x0600057B RID: 1403 RVA: 0x000205B3 File Offset: 0x0001E7B3
		// (set) Token: 0x0600057C RID: 1404 RVA: 0x000205BB File Offset: 0x0001E7BB
		public bool NoTranslation
		{
			get
			{
				return this.mNoTranslation;
			}
			set
			{
				this.mNoTranslation = value;
			}
		}

		// Token: 0x170000EB RID: 235
		// (get) Token: 0x0600057D RID: 1405 RVA: 0x000205C4 File Offset: 0x0001E7C4
		// (set) Token: 0x0600057E RID: 1406 RVA: 0x000205CC File Offset: 0x0001E7CC
		public string Title
		{
			get
			{
				return this.mTitle;
			}
			set
			{
				this.mTitle = value;
				if (this.mTitle != null)
				{
					this.mTitle = this.mTitle.Trim();
				}
				if (string.IsNullOrEmpty(this.mTitle))
				{
					this.mTitle = "";
					this.mTitleHash = 0;
					return;
				}
				this.mTitleHash = this.mTitle.GetHashCodeCustom();
			}
		}

		// Token: 0x170000EC RID: 236
		// (get) Token: 0x0600057F RID: 1407 RVA: 0x0002062A File Offset: 0x0001E82A
		// (set) Token: 0x06000580 RID: 1408 RVA: 0x00020634 File Offset: 0x0001E834
		public string SubTitle
		{
			get
			{
				return this.mSubtitle;
			}
			set
			{
				this.mSubtitle = value;
				if (this.mSubtitle != null)
				{
					this.mSubtitle = this.mSubtitle.Trim();
				}
				if (string.IsNullOrEmpty(this.mSubtitle))
				{
					this.mSubtitle = "";
					this.mSubtitleHash = 0;
					return;
				}
				this.mSubtitleHash = this.mSubtitle.GetHashCodeCustom();
			}
		}

		// Token: 0x170000ED RID: 237
		// (get) Token: 0x06000581 RID: 1409 RVA: 0x00020692 File Offset: 0x0001E892
		// (set) Token: 0x06000582 RID: 1410 RVA: 0x0002069A File Offset: 0x0001E89A
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

		// Token: 0x170000EE RID: 238
		// (get) Token: 0x06000583 RID: 1411 RVA: 0x000206A3 File Offset: 0x0001E8A3
		// (set) Token: 0x06000584 RID: 1412 RVA: 0x000206AB File Offset: 0x0001E8AB
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

		// Token: 0x170000EF RID: 239
		// (get) Token: 0x06000585 RID: 1413 RVA: 0x000206B4 File Offset: 0x0001E8B4
		// (set) Token: 0x06000586 RID: 1414 RVA: 0x000206BC File Offset: 0x0001E8BC
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

		// Token: 0x170000F0 RID: 240
		// (get) Token: 0x06000587 RID: 1415 RVA: 0x000206C5 File Offset: 0x0001E8C5
		// (set) Token: 0x06000588 RID: 1416 RVA: 0x000206D0 File Offset: 0x0001E8D0
		public string TextAlignment
		{
			get
			{
				return this.mTextAlignmentValue;
			}
			set
			{
				this.mTextAlignmentValue = value.Trim().ToUpper();
				if (string.Compare(this.mTextAlignmentValue, "L") == 0 || string.Compare(this.mTextAlignmentValue, "LEFT") == 0)
				{
					this.mTextAlignmentValue = "L";
					this.mTextAlignment = TextAlign.Left;
					return;
				}
				if (string.Compare(this.mTextAlignmentValue, "R") == 0 || string.Compare(this.mTextAlignmentValue, "RIGHT") == 0)
				{
					this.mTextAlignmentValue = "R";
					this.mTextAlignment = TextAlign.Right;
					return;
				}
				if (string.Compare(this.mTextAlignmentValue, "C") == 0 || string.Compare(this.mTextAlignmentValue, "CENTER") == 0)
				{
					this.mTextAlignmentValue = "C";
					this.mTextAlignment = TextAlign.Center;
					return;
				}
				this.mTextAlignmentValue = "C";
				this.mTextAlignment = TextAlign.Center;
			}
		}

		// Token: 0x06000589 RID: 1417 RVA: 0x000207A5 File Offset: 0x0001E9A5
		public DisplayTitleInLevel(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x0600058A RID: 1418 RVA: 0x000207D0 File Offset: 0x0001E9D0
		public override void Initialize()
		{
			base.Initialize();
			if (!this.mNoTranslation && this.mTitleHash != 0)
			{
				this.mTitle = LanguageManager.Instance.GetString(this.mTitleHash).ToUpper();
				if (string.IsNullOrEmpty(this.mTitle))
				{
					this.mTitle = "";
				}
				else if (this.mTitle.Length >= 10 && string.Compare(this.mTitle.Substring(0, 10), "NOT FOUND:") == 0)
				{
					this.mTitle = "";
				}
			}
			else
			{
				this.mTitle = this.mTitle.ToUpper();
			}
			if (!this.mNoTranslation && this.mSubtitleHash != 0)
			{
				this.mSubtitle = LanguageManager.Instance.GetString(this.mSubtitleHash).ToUpper();
				if (string.IsNullOrEmpty(this.mSubtitle))
				{
					this.mSubtitle = "";
				}
				else if (this.mSubtitle.Length >= 10 && string.Compare(this.mSubtitle.Substring(0, 10), "NOT FOUND:") == 0)
				{
					this.mSubtitle = "";
				}
			}
			else if (this.mSubtitle != null)
			{
				this.mSubtitle = this.mSubtitle.ToUpper();
			}
			if (string.IsNullOrEmpty(this.mTextAlignmentValue))
			{
				this.mTextAlignmentValue = "C";
				this.mTextAlignment = TextAlign.Center;
			}
		}

		// Token: 0x0600058B RID: 1419 RVA: 0x00020922 File Offset: 0x0001EB22
		public override void QuickExecute()
		{
		}

		// Token: 0x0600058C RID: 1420 RVA: 0x00020924 File Offset: 0x0001EB24
		protected override void Execute()
		{
			if ((!this.mNoTranslation && (this.mTitleHash != 0 || this.mSubtitleHash != 0)) || (!string.IsNullOrEmpty(this.mTitle) && !string.IsNullOrEmpty(this.mSubtitle)))
			{
				if (this.mTitleDisplayTime < 0f)
				{
					this.mTitleDisplayTime = 2f;
				}
				base.GameScene.Level.DisplayTitle(this.mTitle, this.mSubtitle, this.mTitleDisplayTime, this.mFadeIn, this.mFadeOut, this.mTextAlignment);
			}
		}

		// Token: 0x04000439 RID: 1081
		private string mTitle;

		// Token: 0x0400043A RID: 1082
		private int mTitleHash;

		// Token: 0x0400043B RID: 1083
		private string mSubtitle;

		// Token: 0x0400043C RID: 1084
		private string mTextAlignmentValue;

		// Token: 0x0400043D RID: 1085
		private TextAlign mTextAlignment;

		// Token: 0x0400043E RID: 1086
		private int mSubtitleHash;

		// Token: 0x0400043F RID: 1087
		private float mTitleDisplayTime = -1f;

		// Token: 0x04000440 RID: 1088
		private float mFadeOut = 1f;

		// Token: 0x04000441 RID: 1089
		private float mFadeIn = 1f;

		// Token: 0x04000442 RID: 1090
		private bool mNoTranslation;
	}
}
