using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Magicka.Graphics.Effects
{
	// Token: 0x020005D2 RID: 1490
	public class CapsuleLightEffect : Effect
	{
		// Token: 0x06002C94 RID: 11412 RVA: 0x0015DFC4 File Offset: 0x0015C1C4
		public CapsuleLightEffect(GraphicsDevice iDevice, ContentManager iContentManager) : base(iDevice, iContentManager.Load<Effect>("Shaders/CapsuleLight"))
		{
			this.mWorldParameter = base.Parameters["World"];
			this.mViewProjectionParameter = base.Parameters["ViewProjection"];
			this.mInverseViewProjectionParameter = base.Parameters["InverseViewProjection"];
			this.mLengthParameter = base.Parameters["Length"];
			this.mRadiusParameter = base.Parameters["Radius"];
			this.mStartParameter = base.Parameters["Start"];
			this.mEndParameter = base.Parameters["End"];
			this.mDiffuseColorParameter = base.Parameters["DiffuseColor"];
			this.mAmbientColorParameter = base.Parameters["AmbientColor"];
			this.mCameraPositionParameter = base.Parameters["CameraPosition"];
			this.mHalfPixelParameter = base.Parameters["HalfPixel"];
			this.mNormalMapParameter = base.Parameters["NormalMap"];
			this.mDepthMapParameter = base.Parameters["DepthMap"];
		}

		// Token: 0x17000A88 RID: 2696
		// (get) Token: 0x06002C95 RID: 11413 RVA: 0x0015E101 File Offset: 0x0015C301
		// (set) Token: 0x06002C96 RID: 11414 RVA: 0x0015E109 File Offset: 0x0015C309
		public Matrix World
		{
			get
			{
				return this.mWorld;
			}
			set
			{
				this.mWorld = value;
				this.mWorldParameter.SetValue(value);
			}
		}

		// Token: 0x17000A89 RID: 2697
		// (get) Token: 0x06002C97 RID: 11415 RVA: 0x0015E11E File Offset: 0x0015C31E
		// (set) Token: 0x06002C98 RID: 11416 RVA: 0x0015E126 File Offset: 0x0015C326
		public Matrix ViewProjection
		{
			get
			{
				return this.mViewProjection;
			}
			set
			{
				this.mViewProjection = value;
				this.mViewProjectionParameter.SetValue(value);
			}
		}

		// Token: 0x17000A8A RID: 2698
		// (get) Token: 0x06002C99 RID: 11417 RVA: 0x0015E13B File Offset: 0x0015C33B
		// (set) Token: 0x06002C9A RID: 11418 RVA: 0x0015E143 File Offset: 0x0015C343
		public Matrix InverseViewProjection
		{
			get
			{
				return this.mInverseViewProjection;
			}
			set
			{
				this.mInverseViewProjection = value;
				this.mInverseViewProjectionParameter.SetValue(value);
			}
		}

		// Token: 0x17000A8B RID: 2699
		// (get) Token: 0x06002C9B RID: 11419 RVA: 0x0015E158 File Offset: 0x0015C358
		// (set) Token: 0x06002C9C RID: 11420 RVA: 0x0015E160 File Offset: 0x0015C360
		public float Length
		{
			get
			{
				return this.mLength;
			}
			set
			{
				this.mLength = value;
				this.mLengthParameter.SetValue(value);
			}
		}

		// Token: 0x17000A8C RID: 2700
		// (get) Token: 0x06002C9D RID: 11421 RVA: 0x0015E175 File Offset: 0x0015C375
		// (set) Token: 0x06002C9E RID: 11422 RVA: 0x0015E17D File Offset: 0x0015C37D
		public float Radius
		{
			get
			{
				return this.mRadius;
			}
			set
			{
				this.mRadius = value;
				this.mRadiusParameter.SetValue(value);
			}
		}

		// Token: 0x17000A8D RID: 2701
		// (get) Token: 0x06002C9F RID: 11423 RVA: 0x0015E192 File Offset: 0x0015C392
		// (set) Token: 0x06002CA0 RID: 11424 RVA: 0x0015E19A File Offset: 0x0015C39A
		public Vector3 Start
		{
			get
			{
				return this.mStart;
			}
			set
			{
				this.mStart = value;
				this.mStartParameter.SetValue(value);
			}
		}

		// Token: 0x17000A8E RID: 2702
		// (get) Token: 0x06002CA1 RID: 11425 RVA: 0x0015E1AF File Offset: 0x0015C3AF
		// (set) Token: 0x06002CA2 RID: 11426 RVA: 0x0015E1B7 File Offset: 0x0015C3B7
		public new Vector3 End
		{
			get
			{
				return this.mEnd;
			}
			set
			{
				this.mEnd = value;
				this.mEndParameter.SetValue(value);
			}
		}

		// Token: 0x17000A8F RID: 2703
		// (get) Token: 0x06002CA3 RID: 11427 RVA: 0x0015E1CC File Offset: 0x0015C3CC
		// (set) Token: 0x06002CA4 RID: 11428 RVA: 0x0015E1D4 File Offset: 0x0015C3D4
		public Vector3 DiffuseColor
		{
			get
			{
				return this.mDiffuseColor;
			}
			set
			{
				this.mDiffuseColor = value;
				this.mDiffuseColorParameter.SetValue(value);
			}
		}

		// Token: 0x17000A90 RID: 2704
		// (get) Token: 0x06002CA5 RID: 11429 RVA: 0x0015E1E9 File Offset: 0x0015C3E9
		// (set) Token: 0x06002CA6 RID: 11430 RVA: 0x0015E1F1 File Offset: 0x0015C3F1
		public Vector3 AmbientColor
		{
			get
			{
				return this.mAmbientColor;
			}
			set
			{
				this.mAmbientColor = value;
				this.mAmbientColorParameter.SetValue(value);
			}
		}

		// Token: 0x17000A91 RID: 2705
		// (get) Token: 0x06002CA7 RID: 11431 RVA: 0x0015E206 File Offset: 0x0015C406
		// (set) Token: 0x06002CA8 RID: 11432 RVA: 0x0015E20E File Offset: 0x0015C40E
		public Vector2 HalfPixel
		{
			get
			{
				return this.mHalfPixel;
			}
			set
			{
				this.mHalfPixel = value;
				this.mHalfPixelParameter.SetValue(value);
			}
		}

		// Token: 0x17000A92 RID: 2706
		// (get) Token: 0x06002CA9 RID: 11433 RVA: 0x0015E223 File Offset: 0x0015C423
		// (set) Token: 0x06002CAA RID: 11434 RVA: 0x0015E22B File Offset: 0x0015C42B
		public Vector3 CameraPosition
		{
			get
			{
				return this.mCameraPosition;
			}
			set
			{
				this.mCameraPosition = value;
				this.mCameraPositionParameter.SetValue(value);
			}
		}

		// Token: 0x17000A93 RID: 2707
		// (get) Token: 0x06002CAB RID: 11435 RVA: 0x0015E240 File Offset: 0x0015C440
		// (set) Token: 0x06002CAC RID: 11436 RVA: 0x0015E248 File Offset: 0x0015C448
		public Texture NormalMap
		{
			get
			{
				return this.mNormalMap;
			}
			set
			{
				this.mNormalMap = value;
				this.mNormalMapParameter.SetValue(value);
			}
		}

		// Token: 0x17000A94 RID: 2708
		// (get) Token: 0x06002CAD RID: 11437 RVA: 0x0015E25D File Offset: 0x0015C45D
		// (set) Token: 0x06002CAE RID: 11438 RVA: 0x0015E265 File Offset: 0x0015C465
		public Texture DepthMap
		{
			get
			{
				return this.mDepthMap;
			}
			set
			{
				this.mDepthMap = value;
				this.mDepthMapParameter.SetValue(value);
			}
		}

		// Token: 0x04003020 RID: 12320
		public static readonly int TYPEHASH = typeof(CapsuleLightEffect).GetHashCode();

		// Token: 0x04003021 RID: 12321
		private EffectParameter mWorldParameter;

		// Token: 0x04003022 RID: 12322
		private Matrix mWorld;

		// Token: 0x04003023 RID: 12323
		private EffectParameter mViewProjectionParameter;

		// Token: 0x04003024 RID: 12324
		private Matrix mViewProjection;

		// Token: 0x04003025 RID: 12325
		private EffectParameter mInverseViewProjectionParameter;

		// Token: 0x04003026 RID: 12326
		private Matrix mInverseViewProjection;

		// Token: 0x04003027 RID: 12327
		private EffectParameter mLengthParameter;

		// Token: 0x04003028 RID: 12328
		private float mLength;

		// Token: 0x04003029 RID: 12329
		private EffectParameter mRadiusParameter;

		// Token: 0x0400302A RID: 12330
		private float mRadius;

		// Token: 0x0400302B RID: 12331
		private EffectParameter mStartParameter;

		// Token: 0x0400302C RID: 12332
		private Vector3 mStart;

		// Token: 0x0400302D RID: 12333
		private EffectParameter mEndParameter;

		// Token: 0x0400302E RID: 12334
		private Vector3 mEnd;

		// Token: 0x0400302F RID: 12335
		private EffectParameter mDiffuseColorParameter;

		// Token: 0x04003030 RID: 12336
		private Vector3 mDiffuseColor;

		// Token: 0x04003031 RID: 12337
		private EffectParameter mAmbientColorParameter;

		// Token: 0x04003032 RID: 12338
		private Vector3 mAmbientColor;

		// Token: 0x04003033 RID: 12339
		private EffectParameter mHalfPixelParameter;

		// Token: 0x04003034 RID: 12340
		private Vector3 mCameraPosition;

		// Token: 0x04003035 RID: 12341
		private EffectParameter mCameraPositionParameter;

		// Token: 0x04003036 RID: 12342
		private Vector2 mHalfPixel;

		// Token: 0x04003037 RID: 12343
		private EffectParameter mNormalMapParameter;

		// Token: 0x04003038 RID: 12344
		private Texture mNormalMap;

		// Token: 0x04003039 RID: 12345
		private EffectParameter mDepthMapParameter;

		// Token: 0x0400303A RID: 12346
		private Texture mDepthMap;
	}
}
