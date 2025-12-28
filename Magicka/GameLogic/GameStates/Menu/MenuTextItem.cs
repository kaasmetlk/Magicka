using System;
using Magicka.Localization;
using Microsoft.Xna.Framework;
using PolygonHead;
using PolygonHead.Effects;

namespace Magicka.GameLogic.GameStates.Menu
{
	// Token: 0x0200050E RID: 1294
	public class MenuTextItem : MenuItem
	{
		// Token: 0x1700091A RID: 2330
		// (get) Token: 0x060026C2 RID: 9922 RVA: 0x0011AF29 File Offset: 0x00119129
		// (set) Token: 0x060026C3 RID: 9923 RVA: 0x0011AF31 File Offset: 0x00119131
		public new float Alpha
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

		// Token: 0x060026C4 RID: 9924 RVA: 0x0011AF3C File Offset: 0x0011913C
		public MenuTextItem(int iTitle, Vector2 iPosition, BitmapFont iFont, TextAlign iAlign)
		{
			this.mAlign = iAlign;
			this.mFont = iFont;
			this.mValue = iTitle;
			this.mTitle = new Text(40, iFont, iAlign, false);
			this.mTitleString = LanguageManager.Instance.GetString(iTitle);
			this.mTitle.SetText(this.mTitleString);
			this.mPosition = iPosition;
			this.mSize = this.mFont.MeasureText(this.mTitle.Characters, true);
			this.UpdateBoundingBox();
		}

		// Token: 0x060026C5 RID: 9925 RVA: 0x0011AFD0 File Offset: 0x001191D0
		public MenuTextItem(string iTitle, Vector2 iPosition, BitmapFont iFont, TextAlign iAlign)
		{
			this.mAlign = iAlign;
			this.mFont = iFont;
			this.mValue = 0;
			this.mTitleString = iTitle;
			this.mTitle = new Text(64, iFont, iAlign, false);
			this.mTitle.SetText(iTitle);
			this.mPosition = iPosition;
			this.mSize = this.mFont.MeasureText(this.mTitle.Characters, true);
			this.UpdateBoundingBox();
		}

		// Token: 0x060026C6 RID: 9926 RVA: 0x0011B054 File Offset: 0x00119254
		public MenuTextItem(int iTitle, Vector2 iPosition, BitmapFont iFont, TextAlign iAlign, bool iUseFormatting)
		{
			this.mAlign = iAlign;
			this.mFont = iFont;
			this.mValue = iTitle;
			this.mTitle = new Text(40, iFont, iAlign, false, iUseFormatting);
			this.mTitleString = LanguageManager.Instance.GetString(iTitle);
			this.mTitle.SetText(this.mTitleString);
			this.mPosition = iPosition;
			this.mSize = this.mFont.MeasureText(this.mTitle.Characters, true);
			this.UpdateBoundingBox();
		}

		// Token: 0x060026C7 RID: 9927 RVA: 0x0011B0E8 File Offset: 0x001192E8
		public MenuTextItem(int iTitle, int iCharSize, Vector2 iPosition, BitmapFont iFont, TextAlign iAlign, bool iUseFormatting)
		{
			this.mAlign = iAlign;
			this.mFont = iFont;
			this.mValue = iTitle;
			this.mTitle = new Text(iCharSize, iFont, iAlign, false, iUseFormatting);
			this.mTitleString = LanguageManager.Instance.GetString(iTitle);
			this.mTitle.SetText(this.mTitleString);
			this.mPosition = iPosition;
			this.mSize = this.mFont.MeasureText(this.mTitle.Characters, true);
			this.UpdateBoundingBox();
		}

		// Token: 0x060026C8 RID: 9928 RVA: 0x0011B17C File Offset: 0x0011937C
		public MenuTextItem(string iTitle, Vector2 iPosition, BitmapFont iFont, TextAlign iAlign, bool iUseFormatting)
		{
			this.mAlign = iAlign;
			this.mFont = iFont;
			this.mValue = 0;
			this.mTitleString = iTitle;
			this.mTitle = new Text(40, iFont, iAlign, false, iUseFormatting);
			this.mTitle.SetText(iTitle);
			this.mPosition = iPosition;
			this.mSize = this.mFont.MeasureText(this.mTitle.Characters, true);
			this.UpdateBoundingBox();
		}

		// Token: 0x1700091B RID: 2331
		// (get) Token: 0x060026C9 RID: 9929 RVA: 0x0011B200 File Offset: 0x00119400
		// (set) Token: 0x060026CA RID: 9930 RVA: 0x0011B208 File Offset: 0x00119408
		public int Title
		{
			get
			{
				return this.mValue;
			}
			set
			{
				this.mValue = value;
				this.mTitleString = LanguageManager.Instance.GetString(value);
				this.mTitle.SetText(this.mTitleString);
				this.mTitle.MarkAsDirty();
				this.mSize = this.mFont.MeasureText(this.mTitle.Characters, true);
				this.UpdateBoundingBox();
			}
		}

		// Token: 0x1700091C RID: 2332
		// (get) Token: 0x060026CB RID: 9931 RVA: 0x0011B26C File Offset: 0x0011946C
		// (set) Token: 0x060026CC RID: 9932 RVA: 0x0011B274 File Offset: 0x00119474
		public float MaxWidth
		{
			get
			{
				return this.mMaxWidth;
			}
			set
			{
				this.mMaxWidth = value;
				this.UpdateBoundingBox();
			}
		}

		// Token: 0x1700091D RID: 2333
		// (get) Token: 0x060026CD RID: 9933 RVA: 0x0011B283 File Offset: 0x00119483
		public Text Text
		{
			get
			{
				return this.mTitle;
			}
		}

		// Token: 0x1700091E RID: 2334
		// (get) Token: 0x060026CE RID: 9934 RVA: 0x0011B28B File Offset: 0x0011948B
		public string Name
		{
			get
			{
				return this.mTitleString;
			}
		}

		// Token: 0x1700091F RID: 2335
		// (get) Token: 0x060026CF RID: 9935 RVA: 0x0011B293 File Offset: 0x00119493
		// (set) Token: 0x060026D0 RID: 9936 RVA: 0x0011B29B File Offset: 0x0011949B
		public int Hash
		{
			get
			{
				return this.mValue;
			}
			set
			{
				this.mValue = value;
			}
		}

		// Token: 0x060026D1 RID: 9937 RVA: 0x0011B2A4 File Offset: 0x001194A4
		public void SetText(string iText)
		{
			this.mTitle.SetText(iText);
			this.mTitleString = iText;
			this.mSize = this.mFont.MeasureText(this.mTitle.Characters, true);
			this.UpdateBoundingBox();
		}

		// Token: 0x060026D2 RID: 9938 RVA: 0x0011B2DC File Offset: 0x001194DC
		public void SetText(int iTextHash)
		{
			this.mValue = iTextHash;
			this.mTitle.SetText(LanguageManager.Instance.GetString(iTextHash));
			this.mTitleString = LanguageManager.Instance.GetString(iTextHash);
			this.mSize = this.mFont.MeasureText(this.mTitle.Characters, true);
			this.UpdateBoundingBox();
		}

		// Token: 0x060026D3 RID: 9939 RVA: 0x0011B33C File Offset: 0x0011953C
		public void WrapText(int iLength)
		{
			string text = LanguageManager.Instance.GetString(this.mValue);
			text = this.mFont.Wrap(text, iLength, true);
			this.SetText(text);
		}

		// Token: 0x060026D4 RID: 9940 RVA: 0x0011B370 File Offset: 0x00119570
		protected override void UpdateBoundingBox()
		{
			this.mSize = this.mFont.MeasureText(this.mTitle.Characters, true);
			this.mSize.X = Math.Max(this.mSize.X, 20f);
			this.mSize.Y = Math.Max(this.mSize.Y, 20f);
			if (this.mMaxWidth > 1E-45f)
			{
				this.mSize.X = Math.Min(this.mSize.X, this.mMaxWidth);
			}
			switch (this.mAlign)
			{
			case TextAlign.Left:
				this.mBottomRight.X = this.mPosition.X + this.mSize.X * this.mScale;
				this.mTopLeft.X = this.mPosition.X;
				break;
			case TextAlign.Center:
				this.mBottomRight.X = this.mPosition.X + this.mSize.X * 0.5f * this.mScale;
				this.mTopLeft.X = this.mPosition.X - this.mSize.X * 0.5f * this.mScale;
				break;
			case TextAlign.Right:
				this.mBottomRight.X = this.mPosition.X;
				this.mTopLeft.X = this.mPosition.X - this.mSize.X * this.mScale;
				break;
			}
			this.mTopLeft.Y = this.mPosition.Y - this.mSize.Y * 0.5f * this.mScale;
			this.mBottomRight.Y = this.mPosition.Y + this.mSize.Y * 0.5f * this.mScale;
		}

		// Token: 0x060026D5 RID: 9941 RVA: 0x0011B567 File Offset: 0x00119767
		public override void Draw(GUIBasicEffect iEffect)
		{
			this.Draw(iEffect, this.mScale);
		}

		// Token: 0x060026D6 RID: 9942 RVA: 0x0011B576 File Offset: 0x00119776
		public Vector4 GetCurrentColor()
		{
			if (!this.mEnabled)
			{
				return this.mColorDisabled;
			}
			if (!this.mSelected)
			{
				return this.mColor;
			}
			return this.mColorSelected;
		}

		// Token: 0x060026D7 RID: 9943 RVA: 0x0011B59C File Offset: 0x0011979C
		public override void Draw(GUIBasicEffect iEffect, float iScale)
		{
			Vector4 vector = this.mEnabled ? (this.mSelected ? this.mColorSelected : this.mColor) : this.mColorDisabled;
			iEffect.Color = new Vector4(vector.X, vector.Y, vector.Z, vector.W * this.mAlpha);
			Matrix matrix = default(Matrix);
			if (this.mMaxWidth > 1E-45f)
			{
				float x = this.mTitle.Font.MeasureText(this.mTitle.Characters, true).X;
				matrix.M11 = Math.Min(this.mMaxWidth / x, 1f) * iScale;
			}
			else
			{
				matrix.M11 = iScale;
			}
			matrix.M22 = iScale;
			matrix.M41 = this.mPosition.X;
			matrix.M42 = this.mPosition.Y - (float)this.mTitle.Font.LineHeight * 0.5f * iScale + 0.5f;
			matrix.M44 = 1f;
			this.mTitle.Draw(iEffect, ref matrix);
		}

		// Token: 0x060026D8 RID: 9944 RVA: 0x0011B6C0 File Offset: 0x001198C0
		public override void LanguageChanged()
		{
			if (this.mValue == 0)
			{
				return;
			}
			this.mTitle.SetText(LanguageManager.Instance.GetString(this.mValue));
			this.UpdateBoundingBox();
		}

		// Token: 0x04002A0C RID: 10764
		private Text mTitle;

		// Token: 0x04002A0D RID: 10765
		private int mValue;

		// Token: 0x04002A0E RID: 10766
		private Vector2 mSize;

		// Token: 0x04002A0F RID: 10767
		private TextAlign mAlign;

		// Token: 0x04002A10 RID: 10768
		private BitmapFont mFont;

		// Token: 0x04002A11 RID: 10769
		private string mTitleString;

		// Token: 0x04002A12 RID: 10770
		private float mMaxWidth;

		// Token: 0x04002A13 RID: 10771
		private new float mAlpha = 1f;
	}
}
