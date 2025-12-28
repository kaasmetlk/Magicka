using System;
using Microsoft.Xna.Framework;
using PolygonHead;
using PolygonHead.Effects;

namespace Magicka.GameLogic.GameStates.Menu
{
	// Token: 0x02000512 RID: 1298
	public class MenuTextRowItem : MenuItem
	{
		// Token: 0x060026FA RID: 9978 RVA: 0x0011C680 File Offset: 0x0011A880
		public MenuTextRowItem(Vector2 iPosition, Vector2 iSize, BitmapFont iFont, params RowItem[] iItems)
		{
			this.mScale = 1f;
			this.mFont = iFont;
			this.mSize = iSize;
			this.mItems = iItems;
			this.mPosition = iPosition;
			this.mTexts = new Text[this.mItems.Length];
			for (int i = 0; i < this.mTexts.Length; i++)
			{
				this.mTexts[i] = new Text(64, this.mFont, iItems[i].Alignment, false);
			}
			this.LanguageChanged();
			this.UpdateBoundingBox();
		}

		// Token: 0x060026FB RID: 9979 RVA: 0x0011C710 File Offset: 0x0011A910
		protected override void UpdateBoundingBox()
		{
			this.mTopLeft.X = this.mPosition.X * this.mScale;
			this.mTopLeft.Y = this.mPosition.Y * this.mScale;
			this.mBottomRight.X = this.mPosition.X + this.mSize.X * this.mScale;
			this.mBottomRight.Y = this.mPosition.Y + this.mSize.Y * this.mScale;
		}

		// Token: 0x060026FC RID: 9980 RVA: 0x0011C7A9 File Offset: 0x0011A9A9
		public override void Draw(GUIBasicEffect iEffect)
		{
			this.Draw(iEffect, this.mScale);
		}

		// Token: 0x060026FD RID: 9981 RVA: 0x0011C7B8 File Offset: 0x0011A9B8
		public override void Draw(GUIBasicEffect iEffect, float iScale)
		{
			iEffect.Color = (this.mEnabled ? (this.mSelected ? this.mColorSelected : this.mColor) : this.mColorDisabled);
			for (int i = 0; i < this.mTexts.Length; i++)
			{
				this.mTexts[i].Draw(iEffect, this.mPosition.X + this.mItems[i].RelativePosition * this.mSize.X * iScale, this.mPosition.Y);
			}
		}

		// Token: 0x060026FE RID: 9982 RVA: 0x0011C848 File Offset: 0x0011AA48
		public void SetItemText(int iIndex, string iText)
		{
			if (iIndex < 0 | iIndex >= this.mItems.Length)
			{
				return;
			}
			this.mItems[iIndex].Text = iText;
			this.LanguageChanged();
		}

		// Token: 0x060026FF RID: 9983 RVA: 0x0011C878 File Offset: 0x0011AA78
		public void SetItemPosition(int iIndex, float iRelativePosition)
		{
			if (iIndex < 0 | iIndex >= this.mItems.Length)
			{
				return;
			}
			this.mItems[iIndex].RelativePosition = iRelativePosition;
		}

		// Token: 0x06002700 RID: 9984 RVA: 0x0011C8A4 File Offset: 0x0011AAA4
		public override void LanguageChanged()
		{
			for (int i = 0; i < this.mTexts.Length; i++)
			{
				this.mTexts[i].SetText(this.mItems[i].Text);
			}
		}

		// Token: 0x04002A3B RID: 10811
		private RowItem[] mItems;

		// Token: 0x04002A3C RID: 10812
		private Text[] mTexts;

		// Token: 0x04002A3D RID: 10813
		private Vector2 mSize;

		// Token: 0x04002A3E RID: 10814
		private BitmapFont mFont;
	}
}
