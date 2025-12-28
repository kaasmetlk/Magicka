using System;
using System.Threading;
using Magicka.Graphics;
using PolygonHead;
using PolygonHead.Effects;

namespace Magicka.GameLogic
{
	// Token: 0x0200056F RID: 1391
	public class TitleRenderer
	{
		// Token: 0x06002973 RID: 10611 RVA: 0x00144FE6 File Offset: 0x001431E6
		public TitleRenderer() : this(MagickaFont.Stonecross36, MagickaFont.Stonecross28)
		{
		}

		// Token: 0x06002974 RID: 10612 RVA: 0x00144FF0 File Offset: 0x001431F0
		public TitleRenderer(TextAlign iTextAlignment) : this(MagickaFont.Stonecross36, MagickaFont.Stonecross28, 3, iTextAlignment)
		{
		}

		// Token: 0x06002975 RID: 10613 RVA: 0x00144FFC File Offset: 0x001431FC
		public TitleRenderer(MagickaFont iTitleFont, MagickaFont iSubtitleFont) : this(iTitleFont, iSubtitleFont, 3)
		{
		}

		// Token: 0x06002976 RID: 10614 RVA: 0x00145007 File Offset: 0x00143207
		public TitleRenderer(MagickaFont iTitleFont, MagickaFont iSubtitleFont, int iBufferDepth) : this(iTitleFont, iSubtitleFont, iBufferDepth, TextAlign.Center)
		{
		}

		// Token: 0x06002977 RID: 10615 RVA: 0x00145014 File Offset: 0x00143214
		public TitleRenderer(MagickaFont iTitleFont, MagickaFont iSubtitleFont, int iBufferDepth, TextAlign iTextAlignment)
		{
			this.mTitleFont = iTitleFont;
			this.mSubtitleFont = iSubtitleFont;
			this.mTextAlignment = iTextAlignment;
			this.mBufferDepth = iBufferDepth;
			lock (Game.Instance.GraphicsDevice)
			{
				this.mEffect = new GUIBasicEffect(Game.Instance.GraphicsDevice, null);
			}
			this.Initialize();
		}

		// Token: 0x06002978 RID: 10616 RVA: 0x00145098 File Offset: 0x00143298
		public void Clear()
		{
			if (this.mRenderData == null)
			{
				return;
			}
			for (int i = 0; i < this.mRenderData.Length; i++)
			{
				this.mRenderData[i] = null;
			}
			GC.Collect();
			Thread.Sleep(0);
		}

		// Token: 0x06002979 RID: 10617 RVA: 0x001450D8 File Offset: 0x001432D8
		public void Initialize()
		{
			this.mRenderData = new TitleRenderData[this.mBufferDepth];
			for (int i = 0; i < this.mBufferDepth; i++)
			{
				this.mRenderData[i] = new TitleRenderData(this.mEffect, this.mTitleFont, this.mSubtitleFont, this.mTextAlignment);
			}
			this.mTimer = 0f;
			this.mStarted = false;
		}

		// Token: 0x0600297A RID: 10618 RVA: 0x00145140 File Offset: 0x00143340
		public void SetTitles(string iTitle, string iSubtitle, float iDisplayTime, float iFadeIn, float iFadeOut)
		{
			this.mFadeInTime = Math.Max(iFadeIn, 0.01f);
			this.mFadeOutTime = Math.Max(iFadeOut, 0.01f);
			this.mFadeInTimeDivisor = 1f / this.mFadeInTime;
			this.mFadeOutTimeDivisor = 1f / this.mFadeOutTime;
			this.mTitleDisplayTime = iDisplayTime;
			for (int i = 0; i < this.mBufferDepth; i++)
			{
				this.mRenderData[i].SetText(iTitle, iSubtitle, this.mRenderData[i].TitleFont, this.mRenderData[i].SubtitleFont);
			}
			this.mFadeIn = true;
		}

		// Token: 0x0600297B RID: 10619 RVA: 0x001451DD File Offset: 0x001433DD
		public void Start()
		{
			this.mFadeIn = true;
			this.mStarted = true;
			this.mStopped = false;
			this.mTimer = 0f;
		}

		// Token: 0x0600297C RID: 10620 RVA: 0x001451FF File Offset: 0x001433FF
		public void Stop()
		{
			this.mFadeIn = true;
			this.mStarted = false;
			this.mStopped = true;
			this.mTimer = 0f;
		}

		// Token: 0x0600297D RID: 10621 RVA: 0x00145221 File Offset: 0x00143421
		public void ResetTimer()
		{
			this.mFadeIn = true;
			this.mTimer = 0f;
		}

		// Token: 0x0600297E RID: 10622 RVA: 0x00145238 File Offset: 0x00143438
		public TitleRenderData Update(int depth, float iDeltaTime)
		{
			if (this.mStopped)
			{
				return null;
			}
			if (this.mStarted)
			{
				if (this.mFadeIn)
				{
					this.mTimer += iDeltaTime * this.mFadeInTimeDivisor;
					if (this.mTimer > this.mTitleDisplayTime + this.mFadeInTime)
					{
						this.mFadeIn = false;
						this.mTimer = 1f;
					}
				}
				else
				{
					this.mTimer -= iDeltaTime * this.mFadeOutTimeDivisor;
					if (this.mTimer <= 0f)
					{
						this.mStopped = true;
					}
				}
				int num = (depth < 0) ? 0 : ((depth > this.mRenderData.Length - 1) ? (this.mRenderData.Length - 1) : depth);
				TitleRenderData titleRenderData = this.mRenderData[num];
				titleRenderData.Alpha = Math.Min(this.mTimer, 1f);
				return titleRenderData;
			}
			return null;
		}

		// Token: 0x04002CCD RID: 11469
		private TitleRenderData[] mRenderData;

		// Token: 0x04002CCE RID: 11470
		private GUIBasicEffect mEffect;

		// Token: 0x04002CCF RID: 11471
		private float mFadeInTime;

		// Token: 0x04002CD0 RID: 11472
		private float mFadeOutTime;

		// Token: 0x04002CD1 RID: 11473
		private float mFadeInTimeDivisor;

		// Token: 0x04002CD2 RID: 11474
		private float mFadeOutTimeDivisor;

		// Token: 0x04002CD3 RID: 11475
		private float mTitleDisplayTime;

		// Token: 0x04002CD4 RID: 11476
		private float mTimer;

		// Token: 0x04002CD5 RID: 11477
		private int mBufferDepth = 3;

		// Token: 0x04002CD6 RID: 11478
		private MagickaFont mTitleFont;

		// Token: 0x04002CD7 RID: 11479
		private MagickaFont mSubtitleFont;

		// Token: 0x04002CD8 RID: 11480
		private TextAlign mTextAlignment = TextAlign.Center;

		// Token: 0x04002CD9 RID: 11481
		private bool mFadeIn;

		// Token: 0x04002CDA RID: 11482
		private bool mStarted;

		// Token: 0x04002CDB RID: 11483
		private bool mStopped;
	}
}
