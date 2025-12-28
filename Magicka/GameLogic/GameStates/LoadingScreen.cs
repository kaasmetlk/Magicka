using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Magicka.Graphics;
using Magicka.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;

namespace Magicka.GameLogic.GameStates
{
	// Token: 0x02000578 RID: 1400
	internal sealed class LoadingScreen
	{
		// Token: 0x060029E2 RID: 10722 RVA: 0x00147DD4 File Offset: 0x00145FD4
		public LoadingScreen(bool iShowProgress, string iTipText) : this(iShowProgress, iTipText, false)
		{
		}

		// Token: 0x060029E3 RID: 10723 RVA: 0x00147DE0 File Offset: 0x00145FE0
		public LoadingScreen(bool iShowProgress, string iTipText, bool managedMode)
		{
			if (this.mDevice == null)
			{
				this.mDevice = Game.Instance.GraphicsDevice;
			}
			if (this.mContent == null)
			{
				this.mContent = new ContentManager(Game.Instance.Content.ServiceProvider, Game.Instance.Content.RootDirectory);
			}
			this.mHudTexture = this.mContent.Load<Texture2D>("UI/HUD/hud");
			this.CreateVertices((float)this.mHudTexture.Width, (float)this.mHudTexture.Height);
			this.Initialize(iShowProgress, iTipText, managedMode);
		}

		// Token: 0x060029E4 RID: 10724 RVA: 0x00147E94 File Offset: 0x00146094
		public void Initialize(bool iShowProgress, string iTipText, bool managedMode)
		{
			this.mManagedMode = managedMode;
			this.mShowProgress = iShowProgress;
			Point screenSize = RenderManager.Instance.ScreenSize;
			if (this.mHudTexture == null)
			{
				this.mHudTexture = this.mContent.Load<Texture2D>("UI/HUD/hud");
			}
			if (this.mBackgroundTexture == null)
			{
				this.mBackgroundTexture = this.mContent.Load<Texture2D>("UI/Loading/background");
			}
			DirectoryInfo directoryInfo = new DirectoryInfo("content/UI/Loading/images");
			FileInfo[] files = directoryInfo.GetFiles("*.xnb");
			string fullName = files[MagickaMath.Random.Next(files.Length)].FullName;
			this.mImage = this.mContent.Load<Texture2D>(fullName.Substring(0, fullName.Length - 4));
			if (this.mLoadingText == null)
			{
				this.mLoadingText = new Text(32, FontManager.Instance.GetFont(MagickaFont.MenuDefault), TextAlign.Center, false);
			}
			if (this.mTipText == null)
			{
				this.mTipText = new Text(512, FontManager.Instance.GetFont(MagickaFont.Maiandra14), TextAlign.Center, false);
			}
			iTipText = this.mTipText.Font.Wrap(iTipText, 700, true);
			this.mTipText.SetText(iTipText);
			this.mScreenSize.X = (float)screenSize.X;
			this.mScreenSize.Y = (float)screenSize.Y;
			if (managedMode)
			{
				this.alphaBlend_Saved = this.mDevice.RenderState.AlphaBlendEnable;
				this.destBlend_Saved = this.mDevice.RenderState.DestinationBlend;
				this.sourceBlend_Saved = this.mDevice.RenderState.SourceBlend;
				this.depthBuff_Saved = this.mDevice.RenderState.DepthBufferEnable;
				this.stencil_Saved = this.mDevice.RenderState.StencilEnable;
				this.renderTarget_Saved = this.mDevice.GetRenderTarget(0);
				this.depthStencilBuffer_Saved = this.mDevice.DepthStencilBuffer;
				new RenderTarget2D(this.mDevice, (int)this.mScreenSize.X, (int)this.mScreenSize.Y, 0, SurfaceFormat.Color);
				this.mDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer | ClearOptions.Stencil, Color.Black, 1f, 7);
				this.mDevice.SetRenderTarget(0, null);
				this.mDevice.DepthStencilBuffer = null;
			}
			this.mDevice.RenderState.DepthBufferEnable = false;
			this.mDevice.RenderState.StencilEnable = false;
			this.mDevice.RenderState.AlphaTestEnable = false;
			this.mDevice.RenderState.SeparateAlphaBlendEnabled = false;
			this.mDevice.RenderState.AlphaBlendEnable = true;
			this.mDevice.RenderState.SourceBlend = Blend.SourceAlpha;
			this.mDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
			this.mEffect = new GUIBasicEffect(this.mDevice, null);
			this.mEffect.SetScreenSize(screenSize.X, screenSize.Y);
			this.mEffect.ScaleToHDR = false;
		}

		// Token: 0x060029E5 RID: 10725 RVA: 0x00148170 File Offset: 0x00146370
		private void CreateVertices(float iTextureWidth, float iTextureHeight)
		{
			VertexPositionTexture[] array = new VertexPositionTexture[24];
			array[0].Position.X = -0.5f;
			array[0].Position.Y = -0.5f;
			array[0].TextureCoordinate.X = 0f;
			array[0].TextureCoordinate.Y = 0f;
			array[1].Position.X = 0.5f;
			array[1].Position.Y = -0.5f;
			array[1].TextureCoordinate.X = 1f;
			array[1].TextureCoordinate.Y = 0f;
			array[2].Position.X = 0.5f;
			array[2].Position.Y = 0.5f;
			array[2].TextureCoordinate.X = 1f;
			array[2].TextureCoordinate.Y = 1f;
			array[3].Position.X = -0.5f;
			array[3].Position.Y = 0.5f;
			array[3].TextureCoordinate.X = 0f;
			array[3].TextureCoordinate.Y = 1f;
			array[4].Position.X = -320f;
			array[4].Position.Y = 12f;
			array[4].TextureCoordinate.X = 0f / iTextureWidth;
			array[4].TextureCoordinate.Y = 72f / iTextureHeight;
			array[5].Position.X = -320f;
			array[5].Position.Y = -12f;
			array[5].TextureCoordinate.X = 0f / iTextureWidth;
			array[5].TextureCoordinate.Y = 48f / iTextureHeight;
			array[6].Position.X = -224f;
			array[6].Position.Y = 12f;
			array[6].TextureCoordinate.X = 96f / iTextureWidth;
			array[6].TextureCoordinate.Y = 72f / iTextureHeight;
			array[7].Position.X = -224f;
			array[7].Position.Y = -12f;
			array[7].TextureCoordinate.X = 96f / iTextureWidth;
			array[7].TextureCoordinate.Y = 48f / iTextureHeight;
			array[8].Position.X = 224f;
			array[8].Position.Y = 12f;
			array[8].TextureCoordinate.X = 160f / iTextureWidth;
			array[8].TextureCoordinate.Y = 72f / iTextureHeight;
			array[9].Position.X = 224f;
			array[9].Position.Y = -12f;
			array[9].TextureCoordinate.X = 160f / iTextureWidth;
			array[9].TextureCoordinate.Y = 48f / iTextureHeight;
			array[10].Position.X = 320f;
			array[10].Position.Y = 12f;
			array[10].TextureCoordinate.X = 256f / iTextureWidth;
			array[10].TextureCoordinate.Y = 72f / iTextureHeight;
			array[11].Position.X = 320f;
			array[11].Position.Y = -12f;
			array[11].TextureCoordinate.X = 256f / iTextureWidth;
			array[11].TextureCoordinate.Y = 48f / iTextureHeight;
			array[12].Position.X = 0f;
			array[12].Position.Y = 12f;
			array[12].TextureCoordinate.X = 76f / iTextureWidth;
			array[12].TextureCoordinate.Y = 96f / iTextureHeight;
			array[13].Position.X = 0f;
			array[13].Position.Y = -12f;
			array[13].TextureCoordinate.X = 76f / iTextureWidth;
			array[13].TextureCoordinate.Y = 72f / iTextureHeight;
			array[14].Position.X = 608f;
			array[14].Position.Y = 12f;
			array[14].TextureCoordinate.X = 84f / iTextureWidth;
			array[14].TextureCoordinate.Y = 96f / iTextureHeight;
			array[15].Position.X = 608f;
			array[15].Position.Y = -12f;
			array[15].TextureCoordinate.X = 84f / iTextureWidth;
			array[15].TextureCoordinate.Y = 72f / iTextureHeight;
			array[16].Position.X = -320f;
			array[16].Position.Y = 12f;
			array[16].TextureCoordinate.X = 0f / iTextureWidth;
			array[16].TextureCoordinate.Y = 48f / iTextureHeight;
			array[17].Position.X = -320f;
			array[17].Position.Y = -12f;
			array[17].TextureCoordinate.X = 0f / iTextureWidth;
			array[17].TextureCoordinate.Y = 24f / iTextureHeight;
			array[18].Position.X = -224f;
			array[18].Position.Y = 12f;
			array[18].TextureCoordinate.X = 96f / iTextureWidth;
			array[18].TextureCoordinate.Y = 48f / iTextureHeight;
			array[19].Position.X = -224f;
			array[19].Position.Y = -12f;
			array[19].TextureCoordinate.X = 96f / iTextureWidth;
			array[19].TextureCoordinate.Y = 24f / iTextureHeight;
			array[20].Position.X = 224f;
			array[20].Position.Y = 12f;
			array[20].TextureCoordinate.X = 160f / iTextureWidth;
			array[20].TextureCoordinate.Y = 48f / iTextureHeight;
			array[21].Position.X = 224f;
			array[21].Position.Y = -12f;
			array[21].TextureCoordinate.X = 160f / iTextureWidth;
			array[21].TextureCoordinate.Y = 24f / iTextureHeight;
			array[22].Position.X = 320f;
			array[22].Position.Y = 12f;
			array[22].TextureCoordinate.X = 256f / iTextureWidth;
			array[22].TextureCoordinate.Y = 48f / iTextureHeight;
			array[23].Position.X = 320f;
			array[23].Position.Y = -12f;
			array[23].TextureCoordinate.X = 256f / iTextureWidth;
			array[23].TextureCoordinate.Y = 24f / iTextureHeight;
			this.mVertexBuffer = new VertexBuffer(this.mDevice, VertexPositionTexture.SizeInBytes * array.Length, BufferUsage.WriteOnly);
			this.mVertexBuffer.SetData<VertexPositionTexture>(array);
			this.VertexDeclaration = RenderManager.Instance.CreateVertexDeclaration(VertexPositionTexture.VertexElements);
		}

		// Token: 0x060029E6 RID: 10726 RVA: 0x00148A8D File Offset: 0x00146C8D
		public void DisplayWaiting()
		{
			this.mLoadingText.SetText(LanguageManager.Instance.GetString("#network_24".GetHashCodeCustom()));
			this.mWaiting = true;
		}

		// Token: 0x170009E2 RID: 2530
		// (get) Token: 0x060029E7 RID: 10727 RVA: 0x00148AB5 File Offset: 0x00146CB5
		// (set) Token: 0x060029E8 RID: 10728 RVA: 0x00148ABD File Offset: 0x00146CBD
		public float Progress
		{
			get
			{
				return this.mProgress;
			}
			set
			{
				this.mProgress = value;
			}
		}

		// Token: 0x060029E9 RID: 10729 RVA: 0x00148AC8 File Offset: 0x00146CC8
		public void FadeIn(float iTime)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			do
			{
				this.mAlpha = Math.Min((float)stopwatch.Elapsed.TotalSeconds / iTime, 1f);
				this.Draw();
			}
			while (this.mAlpha < 1f);
		}

		// Token: 0x060029EA RID: 10730 RVA: 0x00148B10 File Offset: 0x00146D10
		public void FadeOut(float iTime)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			do
			{
				this.mAlpha = 1f - Math.Min((float)stopwatch.Elapsed.TotalSeconds / iTime, 1f);
				this.Draw();
			}
			while (this.mAlpha > 0f);
		}

		// Token: 0x060029EB RID: 10731 RVA: 0x00148B60 File Offset: 0x00146D60
		public void EndDraw()
		{
			Thread.Sleep(0);
			if (!this.mManagedMode)
			{
				return;
			}
			lock (this.mDevice)
			{
				this.mDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer | ClearOptions.Stencil, Color.Black, 1f, 7);
				this.mDevice.RenderState.AlphaBlendEnable = this.alphaBlend_Saved;
				this.mDevice.RenderState.DestinationBlend = this.destBlend_Saved;
				this.mDevice.RenderState.SourceBlend = this.sourceBlend_Saved;
				this.mDevice.RenderState.DepthBufferEnable = this.depthBuff_Saved;
				this.mDevice.RenderState.StencilEnable = this.stencil_Saved;
				this.mDevice.SetRenderTarget(0, this.renderTarget_Saved as RenderTarget2D);
				this.mDevice.DepthStencilBuffer = this.depthStencilBuffer_Saved;
			}
			Thread.Sleep(0);
		}

		// Token: 0x060029EC RID: 10732 RVA: 0x00148C58 File Offset: 0x00146E58
		public void Draw()
		{
			this.mDevice.Clear(Color.Black);
			this.mDevice.RenderState.AlphaBlendEnable = true;
			this.mDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
			this.mDevice.RenderState.SourceBlend = Blend.SourceAlpha;
			this.mDevice.RenderState.DepthBufferEnable = false;
			this.mDevice.RenderState.StencilEnable = false;
			this.mEffect.Begin();
			this.mEffect.CurrentTechnique.Passes[0].Begin();
			this.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertexBuffer, 0, VertexPositionTexture.SizeInBytes);
			this.mEffect.GraphicsDevice.VertexDeclaration = this.VertexDeclaration;
			Vector4 color = default(Vector4);
			color.X = 1f;
			color.Y = 1f;
			color.Z = 1f;
			color.W = this.mAlpha;
			this.mEffect.Color = color;
			this.mEffect.Texture = this.mBackgroundTexture;
			Matrix transform = default(Matrix);
			transform.M44 = 1f;
			transform.M11 = this.mScreenSize.Y;
			transform.M22 = this.mScreenSize.Y;
			transform.M41 = this.mScreenSize.X * 0.5f;
			transform.M42 = this.mScreenSize.Y * 0.5f;
			this.mEffect.Transform = transform;
			this.mEffect.CommitChanges();
			this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
			if (this.mShowProgress)
			{
				this.mEffect.Texture = this.mImage;
				transform.M42 = this.mScreenSize.Y * 0.5f - 100f;
				transform.M11 = (float)this.mImage.Width;
				transform.M22 = (float)this.mImage.Height;
				this.mEffect.Transform = transform;
				this.mEffect.CommitChanges();
				this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
			}
			if (this.mWaiting)
			{
				this.mLoadingText.Draw(this.mEffect, this.mScreenSize.X * 0.5f, this.mScreenSize.Y * 0.5f + 100f);
			}
			else if (this.mShowProgress)
			{
				this.mEffect.Texture = this.mHudTexture;
				transform.M11 = 1f;
				transform.M22 = 1f;
				transform.M41 = this.mScreenSize.X * 0.5f;
				transform.M42 = this.mScreenSize.Y * 0.5f + 112f;
				this.mEffect.Transform = transform;
				color.X = 1f;
				color.Y = 1f;
				color.Z = 1f;
				color.W = this.mAlpha * this.mAlpha;
				this.mEffect.Color = color;
				this.mEffect.CommitChanges();
				this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 4, 6);
				transform.M11 = this.mProgress;
				transform.M41 = this.mScreenSize.X * 0.5f - 320f + 16f;
				this.mEffect.Transform = transform;
				color.X = this.mHealthColor.X;
				color.Y = this.mHealthColor.Y;
				color.Z = this.mHealthColor.Z;
				this.mEffect.Color = color;
				this.mEffect.CommitChanges();
				this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 12, 2);
				transform.M11 = 1f;
				transform.M22 = 1f;
				transform.M41 = this.mScreenSize.X * 0.5f;
				this.mEffect.Transform = transform;
				color.X = 1f;
				color.Y = 1f;
				color.Z = 1f;
				this.mEffect.Color = color;
				this.mEffect.CommitChanges();
				this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 16, 6);
			}
			Vector2 vector = this.mTipText.Font.MeasureText(this.mTipText.Characters, true);
			try
			{
				this.mTipText.Draw(this.mEffect, this.mScreenSize.X * 0.5f, this.mScreenSize.Y - (vector.Y + (float)this.mTipText.Font.LineHeight * 2f));
			}
			catch
			{
			}
			this.mEffect.CurrentTechnique.Passes[0].End();
			this.mEffect.End();
			this.mDevice.Present();
		}

		// Token: 0x060029ED RID: 10733 RVA: 0x00149194 File Offset: 0x00147394
		public void Dispose()
		{
			this.mEffect.Dispose();
			this.mContent.Dispose();
		}

		// Token: 0x04002D39 RID: 11577
		private const int HEALTHBARSIDESIZE = 96;

		// Token: 0x04002D3A RID: 11578
		private const int HEALTHOFFSET = 16;

		// Token: 0x04002D3B RID: 11579
		private const int HEALTHBARSIZE = 640;

		// Token: 0x04002D3C RID: 11580
		private const int HEALTHBARHALFSIZE = 320;

		// Token: 0x04002D3D RID: 11581
		private GraphicsDevice mDevice;

		// Token: 0x04002D3E RID: 11582
		private ContentManager mContent;

		// Token: 0x04002D3F RID: 11583
		private GUIBasicEffect mEffect;

		// Token: 0x04002D40 RID: 11584
		private VertexBuffer mVertexBuffer;

		// Token: 0x04002D41 RID: 11585
		private VertexDeclaration VertexDeclaration;

		// Token: 0x04002D42 RID: 11586
		private float mProgress;

		// Token: 0x04002D43 RID: 11587
		private float mAlpha;

		// Token: 0x04002D44 RID: 11588
		private Vector3 mHealthColor = new Vector3(1f, 0f, 0f);

		// Token: 0x04002D45 RID: 11589
		private Texture2D mHudTexture;

		// Token: 0x04002D46 RID: 11590
		private Texture2D mBackgroundTexture;

		// Token: 0x04002D47 RID: 11591
		private Texture2D mImage;

		// Token: 0x04002D48 RID: 11592
		private bool mShowProgress;

		// Token: 0x04002D49 RID: 11593
		private bool mWaiting;

		// Token: 0x04002D4A RID: 11594
		private bool mManagedMode;

		// Token: 0x04002D4B RID: 11595
		private bool alphaBlend_Saved;

		// Token: 0x04002D4C RID: 11596
		private Blend destBlend_Saved;

		// Token: 0x04002D4D RID: 11597
		private Blend sourceBlend_Saved;

		// Token: 0x04002D4E RID: 11598
		private bool depthBuff_Saved;

		// Token: 0x04002D4F RID: 11599
		private bool stencil_Saved;

		// Token: 0x04002D50 RID: 11600
		private RenderTarget renderTarget_Saved;

		// Token: 0x04002D51 RID: 11601
		private DepthStencilBuffer depthStencilBuffer_Saved;

		// Token: 0x04002D52 RID: 11602
		private Text mLoadingText;

		// Token: 0x04002D53 RID: 11603
		private Text mTipText;

		// Token: 0x04002D54 RID: 11604
		private Vector2 mScreenSize;
	}
}
