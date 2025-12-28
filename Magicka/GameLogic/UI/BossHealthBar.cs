using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;

namespace Magicka.GameLogic.UI
{
	// Token: 0x02000492 RID: 1170
	public class BossHealthBar
	{
		// Token: 0x06002370 RID: 9072 RVA: 0x000FE460 File Offset: 0x000FC660
		public BossHealthBar(Scene iScene)
		{
			this.mScene = iScene;
			GraphicsDevice graphicsDevice = Game.Instance.GraphicsDevice;
			lock (graphicsDevice)
			{
				this.mEffect = new GUIBasicEffect(graphicsDevice, RenderManager.Instance.GlobalDummyEffect.EffectPool);
				this.mTexture = Game.Instance.Content.Load<Texture2D>("UI/HUD/hud");
				this.mEffect.Texture = this.mTexture;
			}
			this.mEffect.SetScreenSize(RenderManager.Instance.ScreenSize.X, RenderManager.Instance.ScreenSize.Y);
			this.mEffect.TextureEnabled = true;
			this.mRenderData = new BossHealthBar.RenderData[3];
			for (int i = 0; i < 3; i++)
			{
				BossHealthBar.RenderData renderData = new BossHealthBar.RenderData();
				this.mRenderData[i] = renderData;
				renderData.mEffect = this.mEffect;
			}
			this.mVertexBuffer = new VertexBuffer(Game.Instance.GraphicsDevice, VertexPositionTexture.SizeInBytes * this.mVertices.Length, BufferUsage.WriteOnly);
			this.mVertexDeclaration = new VertexDeclaration(Game.Instance.GraphicsDevice, VertexPositionTexture.VertexElements);
			this.CreateVertices(0.8f, (float)this.mTexture.Width, (float)this.mTexture.Height);
			this.mAlpha = 0f;
			this.mPower = 1f;
			this.mDisplayHealth = 0f;
			this.mNormalizedHealth = 1f;
			this.mDestroy = false;
		}

		// Token: 0x17000872 RID: 2162
		// (get) Token: 0x06002371 RID: 9073 RVA: 0x000FE5F8 File Offset: 0x000FC7F8
		// (set) Token: 0x06002372 RID: 9074 RVA: 0x000FE600 File Offset: 0x000FC800
		public Scene Scene
		{
			get
			{
				return this.mScene;
			}
			set
			{
				this.mScene = value;
			}
		}

		// Token: 0x06002373 RID: 9075 RVA: 0x000FE609 File Offset: 0x000FC809
		public void Reset()
		{
			this.mPower = 1f;
			this.mAlpha = 0f;
			this.mDisplayHealth = 0f;
			this.mNormalizedHealth = 1f;
			this.mDestroy = false;
		}

		// Token: 0x06002374 RID: 9076 RVA: 0x000FE640 File Offset: 0x000FC840
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			if (iDataChannel == DataChannel.None)
			{
				return;
			}
			BossHealthBar.RenderData renderData = this.mRenderData[(int)iDataChannel];
			if (this.mDestroy)
			{
				this.mAlpha = Math.Max(this.mAlpha - iDeltaTime * 0.5f, 0f);
			}
			else
			{
				this.mAlpha = Math.Min(this.mAlpha + iDeltaTime * 0.5f, 1f);
			}
			if (this.mAlpha > 0.99f || this.mDestroy)
			{
				this.mDisplayHealth += (this.mNormalizedHealth - this.mDisplayHealth) * 10f * iDeltaTime;
			}
			renderData.mAlpha = this.mAlpha;
			renderData.mNormalizedHealth = this.mDisplayHealth;
			renderData.mHealthColor = new Vector3(this.mPower, 0f, 0f);
			this.mScene.AddRenderableGUIObject(iDataChannel, renderData);
		}

		// Token: 0x06002375 RID: 9077 RVA: 0x000FE718 File Offset: 0x000FC918
		public void SetWidth(float iHealthbarWidth)
		{
			this.CreateVertices(iHealthbarWidth, (float)this.mTexture.Width, (float)this.mTexture.Height);
		}

		// Token: 0x06002376 RID: 9078 RVA: 0x000FE73C File Offset: 0x000FC93C
		protected void CreateVertices(float iHealthbarWidth, float iTextureWidth, float iTextureHeight)
		{
			int x = RenderManager.Instance.ScreenSize.X;
			int num = x / 2;
			int num2 = (int)(iHealthbarWidth * (float)x);
			int num3 = num2 / 2;
			this.mVertices[0].Position.X = (float)(num - num3);
			this.mVertices[0].Position.Y = 32f;
			this.mVertices[0].TextureCoordinate.X = 0f / iTextureWidth;
			this.mVertices[0].TextureCoordinate.Y = 72f / iTextureHeight;
			this.mVertices[1].Position.X = (float)(num - num3);
			this.mVertices[1].Position.Y = 8f;
			this.mVertices[1].TextureCoordinate.X = 0f / iTextureWidth;
			this.mVertices[1].TextureCoordinate.Y = 48f / iTextureHeight;
			this.mVertices[2].Position.X = (float)(num - num3 + 96);
			this.mVertices[2].Position.Y = 32f;
			this.mVertices[2].TextureCoordinate.X = 96f / iTextureWidth;
			this.mVertices[2].TextureCoordinate.Y = 72f / iTextureHeight;
			this.mVertices[3].Position.X = (float)(num - num3 + 96);
			this.mVertices[3].Position.Y = 8f;
			this.mVertices[3].TextureCoordinate.X = 96f / iTextureWidth;
			this.mVertices[3].TextureCoordinate.Y = 48f / iTextureHeight;
			this.mVertices[4].Position.X = (float)(num + num3 - 96);
			this.mVertices[4].Position.Y = 32f;
			this.mVertices[4].TextureCoordinate.X = 160f / iTextureWidth;
			this.mVertices[4].TextureCoordinate.Y = 72f / iTextureHeight;
			this.mVertices[5].Position.X = (float)(num + num3 - 96);
			this.mVertices[5].Position.Y = 8f;
			this.mVertices[5].TextureCoordinate.X = 160f / iTextureWidth;
			this.mVertices[5].TextureCoordinate.Y = 48f / iTextureHeight;
			this.mVertices[6].Position.X = (float)(num + num3);
			this.mVertices[6].Position.Y = 32f;
			this.mVertices[6].TextureCoordinate.X = 256f / iTextureWidth;
			this.mVertices[6].TextureCoordinate.Y = 72f / iTextureHeight;
			this.mVertices[7].Position.X = (float)(num + num3);
			this.mVertices[7].Position.Y = 8f;
			this.mVertices[7].TextureCoordinate.X = 256f / iTextureWidth;
			this.mVertices[7].TextureCoordinate.Y = 48f / iTextureHeight;
			this.mVertices[8].Position.X = 0f;
			this.mVertices[8].Position.Y = 32f;
			this.mVertices[8].TextureCoordinate.X = 76f / iTextureWidth;
			this.mVertices[8].TextureCoordinate.Y = 96f / iTextureHeight;
			this.mVertices[9].Position.X = 0f;
			this.mVertices[9].Position.Y = 8f;
			this.mVertices[9].TextureCoordinate.X = 76f / iTextureWidth;
			this.mVertices[9].TextureCoordinate.Y = 72f / iTextureHeight;
			this.mVertices[10].Position.X = (float)(num2 - 32);
			this.mVertices[10].Position.Y = 32f;
			this.mVertices[10].TextureCoordinate.X = 84f / iTextureWidth;
			this.mVertices[10].TextureCoordinate.Y = 96f / iTextureHeight;
			this.mVertices[11].Position.X = (float)(num2 - 32);
			this.mVertices[11].Position.Y = 8f;
			this.mVertices[11].TextureCoordinate.X = 84f / iTextureWidth;
			this.mVertices[11].TextureCoordinate.Y = 72f / iTextureHeight;
			this.mVertices[12].Position.X = (float)(num - num3);
			this.mVertices[12].Position.Y = 32f;
			this.mVertices[12].TextureCoordinate.X = 0f / iTextureWidth;
			this.mVertices[12].TextureCoordinate.Y = 48f / iTextureHeight;
			this.mVertices[13].Position.X = (float)(num - num3);
			this.mVertices[13].Position.Y = 8f;
			this.mVertices[13].TextureCoordinate.X = 0f / iTextureWidth;
			this.mVertices[13].TextureCoordinate.Y = 24f / iTextureHeight;
			this.mVertices[14].Position.X = (float)(num - num3 + 96);
			this.mVertices[14].Position.Y = 32f;
			this.mVertices[14].TextureCoordinate.X = 96f / iTextureWidth;
			this.mVertices[14].TextureCoordinate.Y = 48f / iTextureHeight;
			this.mVertices[15].Position.X = (float)(num - num3 + 96);
			this.mVertices[15].Position.Y = 8f;
			this.mVertices[15].TextureCoordinate.X = 96f / iTextureWidth;
			this.mVertices[15].TextureCoordinate.Y = 24f / iTextureHeight;
			this.mVertices[16].Position.X = (float)(num + num3 - 96);
			this.mVertices[16].Position.Y = 32f;
			this.mVertices[16].TextureCoordinate.X = 160f / iTextureWidth;
			this.mVertices[16].TextureCoordinate.Y = 48f / iTextureHeight;
			this.mVertices[17].Position.X = (float)(num + num3 - 96);
			this.mVertices[17].Position.Y = 8f;
			this.mVertices[17].TextureCoordinate.X = 160f / iTextureWidth;
			this.mVertices[17].TextureCoordinate.Y = 24f / iTextureHeight;
			this.mVertices[18].Position.X = (float)(num + num3);
			this.mVertices[18].Position.Y = 32f;
			this.mVertices[18].TextureCoordinate.X = 256f / iTextureWidth;
			this.mVertices[18].TextureCoordinate.Y = 48f / iTextureHeight;
			this.mVertices[19].Position.X = (float)(num + num3);
			this.mVertices[19].Position.Y = 8f;
			this.mVertices[19].TextureCoordinate.X = 256f / iTextureWidth;
			this.mVertices[19].TextureCoordinate.Y = 24f / iTextureHeight;
			this.mVertexBuffer.SetData<VertexPositionTexture>(this.mVertices);
			for (int i = 0; i < 3; i++)
			{
				this.mRenderData[i].mVertices = this.mVertexBuffer;
				this.mRenderData[i].mVertexDeclaration = this.mVertexDeclaration;
				this.mRenderData[i].mHealthBarPosition = (float)(num - num3 + 16);
			}
		}

		// Token: 0x17000873 RID: 2163
		// (get) Token: 0x06002377 RID: 9079 RVA: 0x000FF0B9 File Offset: 0x000FD2B9
		// (set) Token: 0x06002378 RID: 9080 RVA: 0x000FF0C1 File Offset: 0x000FD2C1
		public bool Destroy
		{
			get
			{
				return this.mDestroy;
			}
			set
			{
				this.mDestroy = value;
			}
		}

		// Token: 0x17000874 RID: 2164
		// (get) Token: 0x06002379 RID: 9081 RVA: 0x000FF0CA File Offset: 0x000FD2CA
		public float Alpha
		{
			get
			{
				return this.mAlpha;
			}
		}

		// Token: 0x17000875 RID: 2165
		// (get) Token: 0x0600237A RID: 9082 RVA: 0x000FF0D2 File Offset: 0x000FD2D2
		// (set) Token: 0x0600237B RID: 9083 RVA: 0x000FF0DA File Offset: 0x000FD2DA
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

		// Token: 0x0600237C RID: 9084 RVA: 0x000FF0E3 File Offset: 0x000FD2E3
		public void SetNormalizedHealth(float iPercent)
		{
			this.mNormalizedHealth = iPercent;
		}

		// Token: 0x0400267E RID: 9854
		public const int HEALTHBARSIDESIZE = 96;

		// Token: 0x0400267F RID: 9855
		public const int HEALTHOFFSET = 16;

		// Token: 0x04002680 RID: 9856
		private GUIBasicEffect mEffect;

		// Token: 0x04002681 RID: 9857
		private VertexBuffer mVertexBuffer;

		// Token: 0x04002682 RID: 9858
		private VertexDeclaration mVertexDeclaration;

		// Token: 0x04002683 RID: 9859
		private VertexPositionTexture[] mVertices = new VertexPositionTexture[20];

		// Token: 0x04002684 RID: 9860
		private Scene mScene;

		// Token: 0x04002685 RID: 9861
		private BossHealthBar.RenderData[] mRenderData;

		// Token: 0x04002686 RID: 9862
		private bool mDestroy;

		// Token: 0x04002687 RID: 9863
		private float mAlpha;

		// Token: 0x04002688 RID: 9864
		private float mPower;

		// Token: 0x04002689 RID: 9865
		private float mNormalizedHealth;

		// Token: 0x0400268A RID: 9866
		private float mDisplayHealth;

		// Token: 0x0400268B RID: 9867
		private Texture2D mTexture;

		// Token: 0x02000493 RID: 1171
		protected class RenderData : IRenderableGUIObject
		{
			// Token: 0x0600237D RID: 9085 RVA: 0x000FF0EC File Offset: 0x000FD2EC
			public void Draw(float iDeltaTime)
			{
				this.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertices, 0, VertexPositionTexture.SizeInBytes);
				this.mEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
				this.mEffect.SetScreenSize(RenderManager.Instance.ScreenSize.X, RenderManager.Instance.ScreenSize.Y);
				this.mEffect.Begin();
				EffectPassCollection passes = this.mEffect.CurrentTechnique.Passes;
				for (int i = 0; i < passes.Count; i++)
				{
					passes[i].Begin();
					this.mEffect.Transform = Matrix.Identity;
					Vector4 color = default(Vector4);
					color.X = 1f;
					color.Y = 1f;
					color.Z = 1f;
					color.W = this.mAlpha;
					this.mEffect.Color = color;
					this.mEffect.CommitChanges();
					this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 6);
					Matrix transform = default(Matrix);
					transform.M11 = this.mNormalizedHealth;
					transform.M41 = this.mHealthBarPosition;
					transform.M22 = 1f;
					transform.M33 = 1f;
					transform.M44 = 1f;
					this.mEffect.Transform = transform;
					color.X = this.mHealthColor.X;
					color.Y = this.mHealthColor.Y;
					color.Z = this.mHealthColor.Z;
					color.W = this.mAlpha;
					this.mEffect.Color = color;
					this.mEffect.CommitChanges();
					this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 8, 2);
					this.mEffect.Transform = Matrix.Identity;
					color.X = 1f;
					color.Y = 1f;
					color.Z = 1f;
					color.W = this.mAlpha;
					this.mEffect.Color = color;
					this.mEffect.CommitChanges();
					this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 12, 6);
					passes[i].End();
				}
				this.mEffect.End();
			}

			// Token: 0x17000876 RID: 2166
			// (get) Token: 0x0600237E RID: 9086 RVA: 0x000FF353 File Offset: 0x000FD553
			public int ZIndex
			{
				get
				{
					return 200;
				}
			}

			// Token: 0x0400268C RID: 9868
			public GUIBasicEffect mEffect;

			// Token: 0x0400268D RID: 9869
			public VertexBuffer mVertices;

			// Token: 0x0400268E RID: 9870
			public VertexDeclaration mVertexDeclaration;

			// Token: 0x0400268F RID: 9871
			public float mAlpha;

			// Token: 0x04002690 RID: 9872
			public float mNormalizedHealth;

			// Token: 0x04002691 RID: 9873
			public Vector3 mHealthColor;

			// Token: 0x04002692 RID: 9874
			public float mHealthBarPosition;
		}
	}
}
