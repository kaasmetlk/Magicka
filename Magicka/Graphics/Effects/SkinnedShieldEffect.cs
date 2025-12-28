using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Magicka.Graphics.Effects
{
	// Token: 0x020001B2 RID: 434
	internal class SkinnedShieldEffect : Effect
	{
		// Token: 0x06000D21 RID: 3361 RVA: 0x0004CA58 File Offset: 0x0004AC58
		public SkinnedShieldEffect(GraphicsDevice iDevice, ContentManager iContent) : base(iDevice, iContent.Load<Effect>("shaders/skinnedShieldEffect"))
		{
			this.mCameraPositionParameter = base.Parameters["CameraPosition"];
			this.mColorParameter = base.Parameters["Color"];
			this.mBloatParameter = base.Parameters["Bloat"];
			this.mTextureScaleParameter = base.Parameters["TextureScale"];
			this.mTextureOffset0Parameter = base.Parameters["TextureOffset0"];
			this.mTextureOffset1Parameter = base.Parameters["TextureOffset1"];
			this.mTextureOffset2Parameter = base.Parameters["TextureOffset2"];
			this.mViewParameter = base.Parameters["View"];
			this.mProjectionParameter = base.Parameters["Projection"];
			this.mViewProjectionParameter = base.Parameters["ViewProjection"];
			this.mProjectionMapMatrix0Parameter = base.Parameters["ProjectionMapMatrix0"];
			this.mProjectionMapMatrix1Parameter = base.Parameters["ProjectionMapMatrix1"];
			this.mProjectionMapMatrix2Parameter = base.Parameters["ProjectionMapMatrix2"];
			this.mBonesParameter = base.Parameters["Bones"];
			this.mProjectionMapParameter = base.Parameters["ProjectionMap"];
		}

		// Token: 0x17000323 RID: 803
		// (get) Token: 0x06000D22 RID: 3362 RVA: 0x0004CBC1 File Offset: 0x0004ADC1
		// (set) Token: 0x06000D23 RID: 3363 RVA: 0x0004CBCE File Offset: 0x0004ADCE
		public Vector3 CameraPosition
		{
			get
			{
				return this.mCameraPositionParameter.GetValueVector3();
			}
			set
			{
				this.mCameraPositionParameter.SetValue(value);
			}
		}

		// Token: 0x17000324 RID: 804
		// (get) Token: 0x06000D24 RID: 3364 RVA: 0x0004CBDC File Offset: 0x0004ADDC
		// (set) Token: 0x06000D25 RID: 3365 RVA: 0x0004CBE9 File Offset: 0x0004ADE9
		public Vector4 Color
		{
			get
			{
				return this.mColorParameter.GetValueVector4();
			}
			set
			{
				this.mColorParameter.SetValue(value);
			}
		}

		// Token: 0x17000325 RID: 805
		// (get) Token: 0x06000D26 RID: 3366 RVA: 0x0004CBF7 File Offset: 0x0004ADF7
		// (set) Token: 0x06000D27 RID: 3367 RVA: 0x0004CC04 File Offset: 0x0004AE04
		public float Bloat
		{
			get
			{
				return this.mBloatParameter.GetValueSingle();
			}
			set
			{
				this.mBloatParameter.SetValue(value);
			}
		}

		// Token: 0x17000326 RID: 806
		// (get) Token: 0x06000D28 RID: 3368 RVA: 0x0004CC12 File Offset: 0x0004AE12
		// (set) Token: 0x06000D29 RID: 3369 RVA: 0x0004CC1F File Offset: 0x0004AE1F
		public Vector2 TextureScale
		{
			get
			{
				return this.mTextureScaleParameter.GetValueVector2();
			}
			set
			{
				this.mTextureScaleParameter.SetValue(value);
			}
		}

		// Token: 0x17000327 RID: 807
		// (get) Token: 0x06000D2A RID: 3370 RVA: 0x0004CC2D File Offset: 0x0004AE2D
		// (set) Token: 0x06000D2B RID: 3371 RVA: 0x0004CC3A File Offset: 0x0004AE3A
		public Vector2 TextureOffset0
		{
			get
			{
				return this.mTextureOffset0Parameter.GetValueVector2();
			}
			set
			{
				this.mTextureOffset0Parameter.SetValue(value);
			}
		}

		// Token: 0x17000328 RID: 808
		// (get) Token: 0x06000D2C RID: 3372 RVA: 0x0004CC48 File Offset: 0x0004AE48
		// (set) Token: 0x06000D2D RID: 3373 RVA: 0x0004CC55 File Offset: 0x0004AE55
		public Vector2 TextureOffset1
		{
			get
			{
				return this.mTextureOffset1Parameter.GetValueVector2();
			}
			set
			{
				this.mTextureOffset1Parameter.SetValue(value);
			}
		}

		// Token: 0x17000329 RID: 809
		// (get) Token: 0x06000D2E RID: 3374 RVA: 0x0004CC63 File Offset: 0x0004AE63
		// (set) Token: 0x06000D2F RID: 3375 RVA: 0x0004CC70 File Offset: 0x0004AE70
		public Vector2 TextureOffset2
		{
			get
			{
				return this.mTextureOffset2Parameter.GetValueVector2();
			}
			set
			{
				this.mTextureOffset2Parameter.SetValue(value);
			}
		}

		// Token: 0x1700032A RID: 810
		// (get) Token: 0x06000D30 RID: 3376 RVA: 0x0004CC7E File Offset: 0x0004AE7E
		// (set) Token: 0x06000D31 RID: 3377 RVA: 0x0004CC8B File Offset: 0x0004AE8B
		public Matrix View
		{
			get
			{
				return this.mViewParameter.GetValueMatrix();
			}
			set
			{
				this.mViewParameter.SetValue(value);
			}
		}

		// Token: 0x1700032B RID: 811
		// (get) Token: 0x06000D32 RID: 3378 RVA: 0x0004CC99 File Offset: 0x0004AE99
		// (set) Token: 0x06000D33 RID: 3379 RVA: 0x0004CCA6 File Offset: 0x0004AEA6
		public Matrix Projection
		{
			get
			{
				return this.mProjectionParameter.GetValueMatrix();
			}
			set
			{
				this.mProjectionParameter.SetValue(value);
			}
		}

		// Token: 0x1700032C RID: 812
		// (get) Token: 0x06000D34 RID: 3380 RVA: 0x0004CCB4 File Offset: 0x0004AEB4
		// (set) Token: 0x06000D35 RID: 3381 RVA: 0x0004CCC1 File Offset: 0x0004AEC1
		public Matrix ViewProjection
		{
			get
			{
				return this.mViewProjectionParameter.GetValueMatrix();
			}
			set
			{
				this.mViewProjectionParameter.SetValue(value);
			}
		}

		// Token: 0x1700032D RID: 813
		// (get) Token: 0x06000D36 RID: 3382 RVA: 0x0004CCCF File Offset: 0x0004AECF
		// (set) Token: 0x06000D37 RID: 3383 RVA: 0x0004CCDC File Offset: 0x0004AEDC
		public Matrix ProjectionMapMatrix0
		{
			get
			{
				return this.mProjectionMapMatrix0Parameter.GetValueMatrix();
			}
			set
			{
				this.mProjectionMapMatrix0Parameter.SetValue(value);
			}
		}

		// Token: 0x1700032E RID: 814
		// (get) Token: 0x06000D38 RID: 3384 RVA: 0x0004CCEA File Offset: 0x0004AEEA
		// (set) Token: 0x06000D39 RID: 3385 RVA: 0x0004CCF7 File Offset: 0x0004AEF7
		public Matrix ProjectionMapMatrix1
		{
			get
			{
				return this.mProjectionMapMatrix1Parameter.GetValueMatrix();
			}
			set
			{
				this.mProjectionMapMatrix1Parameter.SetValue(value);
			}
		}

		// Token: 0x1700032F RID: 815
		// (get) Token: 0x06000D3A RID: 3386 RVA: 0x0004CD05 File Offset: 0x0004AF05
		// (set) Token: 0x06000D3B RID: 3387 RVA: 0x0004CD12 File Offset: 0x0004AF12
		public Matrix ProjectionMapMatrix2
		{
			get
			{
				return this.mProjectionMapMatrix2Parameter.GetValueMatrix();
			}
			set
			{
				this.mProjectionMapMatrix2Parameter.SetValue(value);
			}
		}

		// Token: 0x17000330 RID: 816
		// (get) Token: 0x06000D3C RID: 3388 RVA: 0x0004CD20 File Offset: 0x0004AF20
		// (set) Token: 0x06000D3D RID: 3389 RVA: 0x0004CD2F File Offset: 0x0004AF2F
		public Matrix[] Bones
		{
			get
			{
				return this.mBonesParameter.GetValueMatrixArray(80);
			}
			set
			{
				this.mBonesParameter.SetValue(value);
			}
		}

		// Token: 0x17000331 RID: 817
		// (get) Token: 0x06000D3E RID: 3390 RVA: 0x0004CD3D File Offset: 0x0004AF3D
		// (set) Token: 0x06000D3F RID: 3391 RVA: 0x0004CD4A File Offset: 0x0004AF4A
		public Texture2D ProjectionMap
		{
			get
			{
				return this.mProjectionMapParameter.GetValueTexture2D();
			}
			set
			{
				this.mProjectionMapParameter.SetValue(value);
			}
		}

		// Token: 0x04000BE0 RID: 3040
		public static readonly int TYPEHASH = typeof(SkinnedShieldEffect).GetHashCode();

		// Token: 0x04000BE1 RID: 3041
		private EffectParameter mCameraPositionParameter;

		// Token: 0x04000BE2 RID: 3042
		private EffectParameter mColorParameter;

		// Token: 0x04000BE3 RID: 3043
		private EffectParameter mBloatParameter;

		// Token: 0x04000BE4 RID: 3044
		private EffectParameter mTextureScaleParameter;

		// Token: 0x04000BE5 RID: 3045
		private EffectParameter mTextureOffset0Parameter;

		// Token: 0x04000BE6 RID: 3046
		private EffectParameter mTextureOffset1Parameter;

		// Token: 0x04000BE7 RID: 3047
		private EffectParameter mTextureOffset2Parameter;

		// Token: 0x04000BE8 RID: 3048
		private EffectParameter mViewParameter;

		// Token: 0x04000BE9 RID: 3049
		private EffectParameter mProjectionParameter;

		// Token: 0x04000BEA RID: 3050
		private EffectParameter mViewProjectionParameter;

		// Token: 0x04000BEB RID: 3051
		private EffectParameter mProjectionMapMatrix0Parameter;

		// Token: 0x04000BEC RID: 3052
		private EffectParameter mProjectionMapMatrix1Parameter;

		// Token: 0x04000BED RID: 3053
		private EffectParameter mProjectionMapMatrix2Parameter;

		// Token: 0x04000BEE RID: 3054
		private EffectParameter mBonesParameter;

		// Token: 0x04000BEF RID: 3055
		private EffectParameter mProjectionMapParameter;
	}
}
