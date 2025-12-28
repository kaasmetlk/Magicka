using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;

namespace Magicka.Graphics.Effects
{
	// Token: 0x02000500 RID: 1280
	public class RainSplashEffect : Effect, IPostEffect
	{
		// Token: 0x060025E8 RID: 9704 RVA: 0x00112274 File Offset: 0x00110474
		public RainSplashEffect(GraphicsDevice iDevice, ContentManager iContentManager) : base(iDevice, iContentManager.Load<Effect>("Shaders/RainSplashEffect"))
		{
			this.mDefaultTechnique = base.Techniques["Default"];
			this.mInverseViewProjectionParameter = base.Parameters["InverseViewProjection"];
			this.mTextureProjectionParameter = base.Parameters["TextureProjection"];
			this.mCameraPositionParameter = base.Parameters["CameraPosition"];
			this.mColorParameter = base.Parameters["Color"];
			this.mWParameter = base.Parameters["W"];
			this.mYOffsetParameter = base.Parameters["YOffset"];
			this.mTextureParameter = base.Parameters["SourceTexture0"];
			this.mNormalMapParameter = base.Parameters["SourceTexture1"];
			this.mDepthMapParameter = base.Parameters["SourceTexture2"];
			this.mDestinationDimensionsParameter = base.Parameters["DestinationDimensions"];
			Vector2 destinationDimensions = default(Vector2);
			Point screenSize = RenderManager.Instance.ScreenSize;
			destinationDimensions.X = (float)screenSize.X;
			destinationDimensions.Y = (float)screenSize.Y;
			this.DestinationDimensions = destinationDimensions;
			this.UpdateVertices();
		}

		// Token: 0x060025E9 RID: 9705 RVA: 0x001123C4 File Offset: 0x001105C4
		protected void UpdateVertices()
		{
			VertexPositionTexture[] array = new VertexPositionTexture[4];
			array[0].Position = new Vector3(-1f, -1f, 0f);
			array[0].TextureCoordinate = new Vector2(0f, 1f);
			array[1].Position = new Vector3(-1f, 1f, 0f);
			array[1].TextureCoordinate = new Vector2(0f, 0f);
			array[2].Position = new Vector3(1f, 1f, 0f);
			array[2].TextureCoordinate = new Vector2(1f, 0f);
			array[3].Position = new Vector3(1f, -1f, 0f);
			array[3].TextureCoordinate = new Vector2(1f, 1f);
			this.mVertices = new VertexBuffer(base.GraphicsDevice, array.Length * VertexPositionTexture.SizeInBytes, BufferUsage.WriteOnly);
			this.mVertices.SetData<VertexPositionTexture>(array);
			this.mVertexDeclaration = new VertexDeclaration(base.GraphicsDevice, VertexPositionTexture.VertexElements);
		}

		// Token: 0x060025EA RID: 9706 RVA: 0x00112504 File Offset: 0x00110704
		public void SetTechnique(RainSplashEffect.Technique iTechnique)
		{
			if (iTechnique != RainSplashEffect.Technique.Default)
			{
				return;
			}
			base.CurrentTechnique = this.mDefaultTechnique;
		}

		// Token: 0x170008E6 RID: 2278
		// (get) Token: 0x060025EB RID: 9707 RVA: 0x00112524 File Offset: 0x00110724
		// (set) Token: 0x060025EC RID: 9708 RVA: 0x0011252C File Offset: 0x0011072C
		public Matrix InverseViewProjection
		{
			get
			{
				return this.mInverseViewProjection;
			}
			set
			{
				if (this.mInverseViewProjection != value)
				{
					this.mInverseViewProjection = value;
					this.mInverseViewProjectionParameter.SetValue(value);
				}
			}
		}

		// Token: 0x170008E7 RID: 2279
		// (get) Token: 0x060025ED RID: 9709 RVA: 0x0011254F File Offset: 0x0011074F
		// (set) Token: 0x060025EE RID: 9710 RVA: 0x00112557 File Offset: 0x00110757
		public Matrix TextureProjection
		{
			get
			{
				return this.mTextureProjection;
			}
			set
			{
				if (this.mTextureProjection != value)
				{
					this.mTextureProjection = value;
					this.mTextureProjectionParameter.SetValue(value);
				}
			}
		}

		// Token: 0x170008E8 RID: 2280
		// (get) Token: 0x060025EF RID: 9711 RVA: 0x0011257A File Offset: 0x0011077A
		// (set) Token: 0x060025F0 RID: 9712 RVA: 0x00112582 File Offset: 0x00110782
		public Vector3 CameraPosition
		{
			get
			{
				return this.mCameraPosition;
			}
			set
			{
				if (this.mCameraPosition != value)
				{
					this.mCameraPosition = value;
					this.mCameraPositionParameter.SetValue(value);
				}
			}
		}

		// Token: 0x170008E9 RID: 2281
		// (get) Token: 0x060025F1 RID: 9713 RVA: 0x001125A5 File Offset: 0x001107A5
		// (set) Token: 0x060025F2 RID: 9714 RVA: 0x001125AD File Offset: 0x001107AD
		public Vector4 Color
		{
			get
			{
				return this.mColor;
			}
			set
			{
				if (this.mColor != value)
				{
					this.mColor = value;
					this.mColorParameter.SetValue(value);
				}
			}
		}

		// Token: 0x170008EA RID: 2282
		// (get) Token: 0x060025F3 RID: 9715 RVA: 0x001125D0 File Offset: 0x001107D0
		// (set) Token: 0x060025F4 RID: 9716 RVA: 0x001125D8 File Offset: 0x001107D8
		public float W
		{
			get
			{
				return this.mW;
			}
			set
			{
				if (this.mW != value)
				{
					this.mW = value;
					this.mWParameter.SetValue(value);
				}
			}
		}

		// Token: 0x170008EB RID: 2283
		// (get) Token: 0x060025F5 RID: 9717 RVA: 0x001125F6 File Offset: 0x001107F6
		// (set) Token: 0x060025F6 RID: 9718 RVA: 0x001125FE File Offset: 0x001107FE
		public float YOffset
		{
			get
			{
				return this.mYOffset;
			}
			set
			{
				if (this.mYOffset != value)
				{
					this.mYOffset = value;
					this.mYOffsetParameter.SetValue(value);
				}
			}
		}

		// Token: 0x170008EC RID: 2284
		// (get) Token: 0x060025F7 RID: 9719 RVA: 0x0011261C File Offset: 0x0011081C
		// (set) Token: 0x060025F8 RID: 9720 RVA: 0x00112624 File Offset: 0x00110824
		public Texture2D NormalMap
		{
			get
			{
				return this.mNormalMap;
			}
			set
			{
				if (this.mNormalMap != value)
				{
					this.mNormalMap = value;
					this.mNormalMapParameter.SetValue(value);
				}
			}
		}

		// Token: 0x170008ED RID: 2285
		// (get) Token: 0x060025F9 RID: 9721 RVA: 0x00112642 File Offset: 0x00110842
		// (set) Token: 0x060025FA RID: 9722 RVA: 0x0011264A File Offset: 0x0011084A
		public Texture2D DepthMap
		{
			get
			{
				return this.mDepthMap;
			}
			set
			{
				if (this.mDepthMap != value)
				{
					this.mDepthMap = value;
					this.mDepthMapParameter.SetValue(value);
				}
			}
		}

		// Token: 0x170008EE RID: 2286
		// (get) Token: 0x060025FB RID: 9723 RVA: 0x00112668 File Offset: 0x00110868
		// (set) Token: 0x060025FC RID: 9724 RVA: 0x00112670 File Offset: 0x00110870
		public Texture3D Texture
		{
			get
			{
				return this.mTexture;
			}
			set
			{
				if (this.mTexture != value)
				{
					this.mTexture = value;
					this.mTextureParameter.SetValue(value);
				}
			}
		}

		// Token: 0x170008EF RID: 2287
		// (get) Token: 0x060025FD RID: 9725 RVA: 0x0011268E File Offset: 0x0011088E
		// (set) Token: 0x060025FE RID: 9726 RVA: 0x00112696 File Offset: 0x00110896
		public Vector2 DestinationDimensions
		{
			get
			{
				return this.mDestinationDimensions;
			}
			set
			{
				if (this.mDestinationDimensions != value)
				{
					this.mDestinationDimensions = value;
					this.mDestinationDimensionsParameter.SetValue(value);
				}
			}
		}

		// Token: 0x170008F0 RID: 2288
		// (get) Token: 0x060025FF RID: 9727 RVA: 0x001126B9 File Offset: 0x001108B9
		public int ZIndex
		{
			get
			{
				return 5;
			}
		}

		// Token: 0x170008F1 RID: 2289
		// (get) Token: 0x06002600 RID: 9728 RVA: 0x001126BC File Offset: 0x001108BC
		public bool Dead
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06002601 RID: 9729 RVA: 0x001126C0 File Offset: 0x001108C0
		public void Draw(float iDeltaTime, ref Vector2 iPixelSize, ref Matrix iViewMatrix, ref Matrix iProjectionMatrix, Texture2D iCandidate, Texture2D iDepthMap, Texture2D iNormalMap)
		{
			base.GraphicsDevice.Vertices[0].SetSource(this.mVertices, 0, VertexPositionTexture.SizeInBytes);
			base.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
			this.Color = new Vector4(1f);
			this.NormalMap = iNormalMap;
			this.DepthMap = iDepthMap;
			this.W += iDeltaTime * 4f;
			this.YOffset = 1f;
			base.Begin();
			base.CurrentTechnique.Passes[0].Begin();
			base.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
			base.CurrentTechnique.Passes[0].End();
			base.End();
		}

		// Token: 0x0400292C RID: 10540
		public const int MAXBONES = 10;

		// Token: 0x0400292D RID: 10541
		public static readonly int TYPEHASH = typeof(RainSplashEffect).GetHashCode();

		// Token: 0x0400292E RID: 10542
		private EffectTechnique mDefaultTechnique;

		// Token: 0x0400292F RID: 10543
		private EffectParameter mInverseViewProjectionParameter;

		// Token: 0x04002930 RID: 10544
		private Matrix mInverseViewProjection;

		// Token: 0x04002931 RID: 10545
		private EffectParameter mTextureProjectionParameter;

		// Token: 0x04002932 RID: 10546
		private Matrix mTextureProjection;

		// Token: 0x04002933 RID: 10547
		private EffectParameter mCameraPositionParameter;

		// Token: 0x04002934 RID: 10548
		private Vector3 mCameraPosition;

		// Token: 0x04002935 RID: 10549
		private EffectParameter mColorParameter;

		// Token: 0x04002936 RID: 10550
		private Vector4 mColor;

		// Token: 0x04002937 RID: 10551
		private EffectParameter mWParameter;

		// Token: 0x04002938 RID: 10552
		private float mW;

		// Token: 0x04002939 RID: 10553
		private EffectParameter mYOffsetParameter;

		// Token: 0x0400293A RID: 10554
		private float mYOffset;

		// Token: 0x0400293B RID: 10555
		private EffectParameter mNormalMapParameter;

		// Token: 0x0400293C RID: 10556
		private Texture2D mNormalMap;

		// Token: 0x0400293D RID: 10557
		private EffectParameter mDepthMapParameter;

		// Token: 0x0400293E RID: 10558
		private Texture2D mDepthMap;

		// Token: 0x0400293F RID: 10559
		private EffectParameter mTextureParameter;

		// Token: 0x04002940 RID: 10560
		private Texture3D mTexture;

		// Token: 0x04002941 RID: 10561
		private EffectParameter mDestinationDimensionsParameter;

		// Token: 0x04002942 RID: 10562
		private Vector2 mDestinationDimensions;

		// Token: 0x04002943 RID: 10563
		private VertexBuffer mVertices;

		// Token: 0x04002944 RID: 10564
		private VertexDeclaration mVertexDeclaration;

		// Token: 0x02000501 RID: 1281
		public enum Technique
		{
			// Token: 0x04002946 RID: 10566
			Default
		}
	}
}
