using System;
using System.Collections.Generic;
using Magicka.Graphics;
using Magicka.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;

namespace Magicka.GameLogic.GameStates.Menu
{
	// Token: 0x02000515 RID: 1301
	public class MenuItemMultiLineText : MenuItem
	{
		// Token: 0x0600271C RID: 10012 RVA: 0x0011CEDA File Offset: 0x0011B0DA
		public MenuItemMultiLineText(Vector2 iPosition, MagickaFont iFont) : this(iPosition, iFont, 0f)
		{
		}

		// Token: 0x0600271D RID: 10013 RVA: 0x0011CEEC File Offset: 0x0011B0EC
		public MenuItemMultiLineText(Vector2 iPosition, MagickaFont iFont, float iSpacing)
		{
			this.mText = new List<Text>();
			this.mLocIds = new List<int>();
			this.mPosition = iPosition;
			this.mFont = FontManager.Instance.GetFont(iFont);
			this.mLineSpacing = iSpacing;
		}

		// Token: 0x17000930 RID: 2352
		// (get) Token: 0x0600271E RID: 10014 RVA: 0x0011CF3F File Offset: 0x0011B13F
		public Vector2 Size
		{
			get
			{
				return this.mSize;
			}
		}

		// Token: 0x0600271F RID: 10015 RVA: 0x0011CF48 File Offset: 0x0011B148
		public override void LanguageChanged()
		{
			for (int i = 0; i < this.mText.Count; i++)
			{
				if (this.mLocIds[i] != 0)
				{
					this.mText[i].SetText(LanguageManager.Instance.GetString(this.mLocIds[i]));
				}
			}
		}

		// Token: 0x06002720 RID: 10016 RVA: 0x0011CFA0 File Offset: 0x0011B1A0
		public void AddLines(int iLoc, TextAlign iAlign, Vector4 iColour, int iMaxWidth)
		{
			string text = LanguageManager.Instance.GetString(iLoc);
			text = this.mFont.Wrap(text, iMaxWidth, true);
			string[] array = text.Split(new char[]
			{
				'\n'
			});
			foreach (string iLoc2 in array)
			{
				this.AddNewLine(iLoc2, iAlign, iColour);
			}
			this.UpdateBoundingBox();
		}

		// Token: 0x06002721 RID: 10017 RVA: 0x0011D008 File Offset: 0x0011B208
		public void AddNewLine(int iLoc, TextAlign iAlign, Vector4 iColour)
		{
			Text text = new Text(128, this.mFont, TextAlign.Center, true);
			text.SetText(LanguageManager.Instance.GetString(iLoc));
			text.DefaultColor = iColour;
			this.mText.Add(text);
			this.mLocIds.Add(iLoc);
			this.UpdateBoundingBox();
		}

		// Token: 0x06002722 RID: 10018 RVA: 0x0011D060 File Offset: 0x0011B260
		public void AddNewLine(string iLoc, TextAlign iAlign, Vector4 iColour)
		{
			Text text = new Text(100, this.mFont, iAlign, true);
			text.SetText(iLoc);
			text.DefaultColor = iColour;
			text.Position = new Vector2(0f, (float)this.mText.Count * ((float)this.mFont.LineHeight + this.mLineSpacing));
			this.mText.Add(text);
			this.mLocIds.Add(0);
			this.UpdateBoundingBox();
		}

		// Token: 0x06002723 RID: 10019 RVA: 0x0011D0D9 File Offset: 0x0011B2D9
		public void SetText(int iIndex, int iLoc)
		{
			if (iIndex >= 0 && iIndex < this.mText.Count)
			{
				this.SetText(iIndex, LanguageManager.Instance.GetString(iLoc));
				this.mLocIds[iIndex] = iLoc;
			}
		}

		// Token: 0x06002724 RID: 10020 RVA: 0x0011D10C File Offset: 0x0011B30C
		public void SetText(int iIndex, string iLoc)
		{
			if (iIndex >= 0 && iIndex < this.mText.Count)
			{
				this.mText[iIndex].SetText(iLoc);
				this.UpdateBoundingBox();
			}
		}

		// Token: 0x06002725 RID: 10021 RVA: 0x0011D138 File Offset: 0x0011B338
		public void SetColour(int iIndex, Vector4 iColour)
		{
			if (iIndex >= 0 && iIndex < this.mText.Count)
			{
				this.mText[iIndex].DefaultColor = iColour;
			}
		}

		// Token: 0x06002726 RID: 10022 RVA: 0x0011D15E File Offset: 0x0011B35E
		public void SetColour(int iIndex, Color iColour)
		{
			this.SetColour(iIndex, iColour.ToVector4());
		}

		// Token: 0x06002727 RID: 10023 RVA: 0x0011D170 File Offset: 0x0011B370
		public void MarkAsDirty()
		{
			foreach (Text text in this.mText)
			{
				text.MarkAsDirty();
			}
		}

		// Token: 0x06002728 RID: 10024 RVA: 0x0011D1C4 File Offset: 0x0011B3C4
		protected override void UpdateBoundingBox()
		{
			this.mSize = Vector2.Zero;
			float num = this.mPosition.Y - (float)this.mFont.LineHeight * 0.5f;
			for (int i = 0; i < this.mText.Count; i++)
			{
				float x = this.mFont.MeasureText(this.mText[i].Characters, true).X;
				if (x > this.mSize.X)
				{
					this.mSize.X = x;
				}
				this.mText[i].Position = new Vector2(this.mPosition.X, num);
				this.mSize.Y = this.mSize.Y + ((float)this.mFont.LineHeight + this.mLineSpacing);
				num += (float)this.mFont.LineHeight + this.mLineSpacing;
			}
			this.mSize.Y = this.mSize.Y - this.mLineSpacing;
			this.mTopLeft.X = this.mPosition.X - this.mSize.X * 0.5f * this.mScale;
			this.mTopLeft.Y = this.mPosition.Y;
			this.mBottomRight.X = this.mPosition.X + this.mSize.X * 0.5f * this.mScale;
			this.mBottomRight.Y = this.mPosition.Y + this.mSize.Y * this.mScale;
			this.mTopLeft.Y = this.mTopLeft.Y - (float)this.mFont.LineHeight * 0.5f * this.mScale;
			this.mBottomRight.Y = this.mBottomRight.Y - (float)this.mFont.LineHeight * 0.5f * this.mScale;
		}

		// Token: 0x06002729 RID: 10025 RVA: 0x0011D3BC File Offset: 0x0011B5BC
		public override void Draw(GUIBasicEffect iEffect)
		{
			this.Draw(iEffect, this.mScale);
		}

		// Token: 0x0600272A RID: 10026 RVA: 0x0011D3CC File Offset: 0x0011B5CC
		public override void Draw(GUIBasicEffect iEffect, float iScale)
		{
			for (int i = 0; i < this.mText.Count; i++)
			{
				Vector4 defaultColor = this.mText[i].DefaultColor;
				Vector4 vector = this.mEnabled ? (this.mSelected ? this.mColorSelected : defaultColor) : this.mColorDisabled;
				this.mText[i].DefaultColor = new Vector4(vector.X, vector.Y, vector.Z, vector.W * this.mAlpha);
				this.mText[i].Draw(iEffect, iScale);
				this.mText[i].DefaultColor = defaultColor;
			}
		}

		// Token: 0x04002A4F RID: 10831
		private const int MAX_TEXT_LENGTH = 128;

		// Token: 0x04002A50 RID: 10832
		private Vector2 mSize = Vector2.Zero;

		// Token: 0x04002A51 RID: 10833
		private BitmapFont mFont;

		// Token: 0x04002A52 RID: 10834
		private List<Text> mText;

		// Token: 0x04002A53 RID: 10835
		private List<int> mLocIds;

		// Token: 0x04002A54 RID: 10836
		private float mLineSpacing;
	}
}
