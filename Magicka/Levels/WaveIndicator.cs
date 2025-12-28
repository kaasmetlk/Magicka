using System;
using Magicka.Graphics;
using Magicka.Localization;
using Microsoft.Xna.Framework;
using PolygonHead;
using PolygonHead.Effects;

namespace Magicka.Levels
{
	// Token: 0x020000B4 RID: 180
	public class WaveIndicator
	{
		// Token: 0x0600054B RID: 1355 RVA: 0x0001FD68 File Offset: 0x0001DF68
		public WaveIndicator()
		{
			this.mRenderData = new WaveIndicator.RenderData[3];
			GUIBasicEffect guibasicEffect = new GUIBasicEffect(Game.Instance.GraphicsDevice, null);
			guibasicEffect.SetScreenSize(RenderManager.Instance.ScreenSize.X, RenderManager.Instance.ScreenSize.Y);
			guibasicEffect.Color = new Vector4(1f, 1f, 1f, 1f);
			this.mText = new Text(48, FontManager.Instance.GetFont(MagickaFont.Maiandra16), TextAlign.Center, false);
			this.mText.SetText(LanguageManager.Instance.GetString(WaveIndicator.LOC_WAVE));
			Vector2 mPosition = new Vector2((float)RenderManager.Instance.ScreenSize.X * 0.5f - this.mText.Font.MeasureText(this.mText.Characters, true).X * 0.5f, 8f);
			for (int i = 0; i < this.mRenderData.Length; i++)
			{
				this.mRenderData[i] = new WaveIndicator.RenderData();
				WaveIndicator.RenderData renderData = this.mRenderData[i];
				renderData.mGuiEffect = guibasicEffect;
				renderData.mRenderableText = this.mText;
				renderData.mPosition = mPosition;
			}
			this.mWaveNum = 0;
		}

		// Token: 0x0600054C RID: 1356 RVA: 0x0001FEA3 File Offset: 0x0001E0A3
		public void Update(float iDeltaTime, DataChannel iDataChan, Scene iScene)
		{
			if (iDataChan != DataChannel.None)
			{
				iScene.AddRenderableGUIObject(iDataChan, this.mRenderData[(int)iDataChan]);
			}
		}

		// Token: 0x170000DD RID: 221
		// (get) Token: 0x0600054D RID: 1357 RVA: 0x0001FEB8 File Offset: 0x0001E0B8
		public int WaveNum
		{
			get
			{
				return this.mWaveNum;
			}
		}

		// Token: 0x0600054E RID: 1358 RVA: 0x0001FEC0 File Offset: 0x0001E0C0
		public void SetWave(int iWaveNum)
		{
			this.mWaveNum = iWaveNum;
			string text = LanguageManager.Instance.GetString(WaveIndicator.LOC_WAVE).Replace("#1;", iWaveNum.ToString());
			this.mText.SetText(text);
		}

		// Token: 0x04000420 RID: 1056
		private static readonly int LOC_WAVE = "#challenge_wave".GetHashCodeCustom();

		// Token: 0x04000421 RID: 1057
		private WaveIndicator.RenderData[] mRenderData;

		// Token: 0x04000422 RID: 1058
		private Text mText;

		// Token: 0x04000423 RID: 1059
		private int mWaveNum;

		// Token: 0x020000B5 RID: 181
		protected class RenderData : IRenderableGUIObject
		{
			// Token: 0x06000550 RID: 1360 RVA: 0x0001FF14 File Offset: 0x0001E114
			public void Draw(float iDeltaTime)
			{
				this.mGuiEffect.SetScreenSize(RenderManager.Instance.ScreenSize.X, RenderManager.Instance.ScreenSize.Y);
				this.mGuiEffect.Begin();
				this.mGuiEffect.CurrentTechnique.Passes[0].Begin();
				this.mRenderableText.Draw(this.mGuiEffect, this.mPosition.X, this.mPosition.Y);
				this.mGuiEffect.CurrentTechnique.Passes[0].End();
				this.mGuiEffect.End();
			}

			// Token: 0x170000DE RID: 222
			// (get) Token: 0x06000551 RID: 1361 RVA: 0x0001FFBD File Offset: 0x0001E1BD
			public int ZIndex
			{
				get
				{
					return 1226;
				}
			}

			// Token: 0x04000424 RID: 1060
			public Text mRenderableText;

			// Token: 0x04000425 RID: 1061
			public GUIBasicEffect mGuiEffect;

			// Token: 0x04000426 RID: 1062
			public Vector2 mPosition;
		}
	}
}
