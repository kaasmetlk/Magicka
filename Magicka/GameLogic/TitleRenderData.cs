using System;
using Magicka.Graphics;
using Microsoft.Xna.Framework;
using PolygonHead;
using PolygonHead.Effects;

namespace Magicka.GameLogic
{
	// Token: 0x02000151 RID: 337
	public sealed class TitleRenderData : IRenderableGUIObject
	{
		// Token: 0x17000220 RID: 544
		// (get) Token: 0x06000A04 RID: 2564 RVA: 0x0003C3F6 File Offset: 0x0003A5F6
		public BitmapFont TitleFont
		{
			get
			{
				return this.mTitleFont;
			}
		}

		// Token: 0x17000221 RID: 545
		// (get) Token: 0x06000A05 RID: 2565 RVA: 0x0003C3FE File Offset: 0x0003A5FE
		public BitmapFont SubtitleFont
		{
			get
			{
				return this.mSubtitleFont;
			}
		}

		// Token: 0x17000222 RID: 546
		// (set) Token: 0x06000A06 RID: 2566 RVA: 0x0003C406 File Offset: 0x0003A606
		public bool DrawShadows
		{
			set
			{
				if (this.mTitleText != null)
				{
					this.mTitleText.DrawShadows = value;
				}
				if (this.mSubtitleText != null)
				{
					this.mSubtitleText.DrawShadows = value;
				}
			}
		}

		// Token: 0x17000223 RID: 547
		// (set) Token: 0x06000A07 RID: 2567 RVA: 0x0003C430 File Offset: 0x0003A630
		public float ShadowAlpha
		{
			set
			{
				if (this.mTitleText != null)
				{
					this.mTitleText.ShadowAlpha = value;
				}
				if (this.mSubtitleText != null)
				{
					this.mSubtitleText.ShadowAlpha = value;
				}
			}
		}

		// Token: 0x17000224 RID: 548
		// (set) Token: 0x06000A08 RID: 2568 RVA: 0x0003C45A File Offset: 0x0003A65A
		public Vector2 TitleShadowOffset
		{
			set
			{
				if (this.mTitleText != null)
				{
					this.mTitleText.ShadowsOffset = value;
				}
			}
		}

		// Token: 0x17000225 RID: 549
		// (set) Token: 0x06000A09 RID: 2569 RVA: 0x0003C470 File Offset: 0x0003A670
		public Vector2 SubtitleShadowOffset
		{
			set
			{
				if (this.mSubtitleText != null)
				{
					this.mSubtitleText.ShadowsOffset = value;
				}
			}
		}

		// Token: 0x17000226 RID: 550
		// (set) Token: 0x06000A0A RID: 2570 RVA: 0x0003C488 File Offset: 0x0003A688
		public Vector2 ShadowOffset
		{
			set
			{
				this.SubtitleShadowOffset = value;
				this.TitleShadowOffset = value;
			}
		}

		// Token: 0x06000A0B RID: 2571 RVA: 0x0003C4A5 File Offset: 0x0003A6A5
		public TitleRenderData(GUIBasicEffect iGUIBasicEffect, MagickaFont iTitleFont, MagickaFont iSubtitleFont) : this(iGUIBasicEffect, iTitleFont, iSubtitleFont, TextAlign.Center)
		{
		}

		// Token: 0x06000A0C RID: 2572 RVA: 0x0003C4B4 File Offset: 0x0003A6B4
		public TitleRenderData(GUIBasicEffect iGUIBasicEffect, MagickaFont iTitleFont, MagickaFont iSubtitleFont, TextAlign iTextAlignment)
		{
			this.mGUIBasicEffect = iGUIBasicEffect;
			this.mTitleFont = FontManager.Instance.GetFont(iTitleFont);
			this.mSubtitleFont = FontManager.Instance.GetFont(iSubtitleFont);
			this.mTitleText = new Text(100, this.mTitleFont, iTextAlignment, false);
			this.mTitleText.DrawShadows = true;
			this.mTitleText.ShadowsOffset = new Vector2(2f, 2f);
			this.mTitleText.ShadowAlpha = 1f;
			this.mSubtitleText = new Text(100, this.mSubtitleFont, iTextAlignment, false);
			this.mSubtitleText.DrawShadows = true;
			this.mSubtitleText.ShadowsOffset = new Vector2(2f, 2f);
			this.mSubtitleText.ShadowAlpha = 1f;
		}

		// Token: 0x06000A0D RID: 2573 RVA: 0x0003C588 File Offset: 0x0003A788
		public void SetText(string iTitle, string iSubtitle, BitmapFont iTitleFont, BitmapFont iSubtitleFont)
		{
			if (iTitleFont != null)
			{
				this.mTitleFont = iTitleFont;
				this.mTitleText.Font = iTitleFont;
			}
			if (iSubtitleFont != null)
			{
				this.mSubtitleFont = iSubtitleFont;
				this.mSubtitleText.Font = iSubtitleFont;
			}
			this.mTitleHeight = this.mTitleFont.MeasureText(iTitle, true).Y;
			this.mTitle = iTitle;
			this.mSubtitle = iSubtitle;
			this.mIsDirty = true;
		}

		// Token: 0x06000A0E RID: 2574 RVA: 0x0003C5F4 File Offset: 0x0003A7F4
		public void Draw(float iDeltaTime)
		{
			if (this.mIsDirty)
			{
				this.mTitleText.SetText(this.mTitle);
				this.mSubtitleText.SetText(this.mSubtitle);
				this.mIsDirty = false;
			}
			Point screenSize = RenderManager.Instance.ScreenSize;
			this.mGUIBasicEffect.Color = new Vector4(1f, 1f, 1f, this.Alpha);
			this.mGUIBasicEffect.SetScreenSize(screenSize.X, screenSize.Y);
			this.mGUIBasicEffect.TextureEnabled = true;
			this.mGUIBasicEffect.Begin();
			this.mGUIBasicEffect.CurrentTechnique.Passes[0].Begin();
			if (!string.IsNullOrEmpty(this.mTitle))
			{
				this.mTitleText.Draw(this.mGUIBasicEffect, (float)screenSize.X * 0.5f + 0.5f, (float)screenSize.Y * 0.3f + 0.5f);
			}
			if (!string.IsNullOrEmpty(this.mSubtitle))
			{
				this.mSubtitleText.Draw(this.mGUIBasicEffect, (float)screenSize.X * 0.5f + 0.5f, (float)screenSize.Y * 0.3f + 0.5f + this.mTitleHeight);
			}
			this.mGUIBasicEffect.CurrentTechnique.Passes[0].End();
			this.mGUIBasicEffect.End();
		}

		// Token: 0x17000227 RID: 551
		// (get) Token: 0x06000A0F RID: 2575 RVA: 0x0003C766 File Offset: 0x0003A966
		public int ZIndex
		{
			get
			{
				return 205;
			}
		}

		// Token: 0x0400090F RID: 2319
		private GUIBasicEffect mGUIBasicEffect;

		// Token: 0x04000910 RID: 2320
		private string mTitle;

		// Token: 0x04000911 RID: 2321
		private string mSubtitle;

		// Token: 0x04000912 RID: 2322
		private Text mTitleText;

		// Token: 0x04000913 RID: 2323
		private Text mSubtitleText;

		// Token: 0x04000914 RID: 2324
		private BitmapFont mTitleFont;

		// Token: 0x04000915 RID: 2325
		private BitmapFont mSubtitleFont;

		// Token: 0x04000916 RID: 2326
		private bool mIsDirty;

		// Token: 0x04000917 RID: 2327
		private float mTitleHeight;

		// Token: 0x04000918 RID: 2328
		public float Alpha;
	}
}
