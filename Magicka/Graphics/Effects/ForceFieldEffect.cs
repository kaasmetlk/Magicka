using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Magicka.Graphics.Effects
{
	// Token: 0x0200014D RID: 333
	internal class ForceFieldEffect : Effect
	{
		// Token: 0x060009A3 RID: 2467 RVA: 0x0003B50C File Offset: 0x0003970C
		public ForceFieldEffect(GraphicsDevice iDevice, ContentManager iContent) : base(iDevice, iContent.Load<Effect>("Shaders/ForceFieldEffect"))
		{
			this.mWorldParameter = base.Parameters["World"];
			this.mViewParameter = base.Parameters["View"];
			this.mProjectionParameter = base.Parameters["Projection"];
			this.mCameraPositionParameter = base.Parameters["CameraPosition"];
			this.mEyeOfTheBeholderParameter = base.Parameters["EyeOfTheBeholder"];
			this.mVertexColorEnabledParameter = base.Parameters["VertexColorEnabled"];
			this.mColorParameter = base.Parameters["Color"];
			this.mWidthParameter = base.Parameters["Width"];
			this.mAlphaPowerParameter = base.Parameters["AlphaPower"];
			this.mAlphaFalloffPowerParameter = base.Parameters["AlphaFalloffPower"];
			this.mMaxRadiusParameter = base.Parameters["MaxRadius"];
			this.mRippleDistortionParameter = base.Parameters["RippleDistortion"];
			this.mMapDistortionParameter = base.Parameters["MapDistortion"];
			this.mDisplacementMapParameter = base.Parameters["DisplacementMap"];
			this.mCollPointsParameter = base.Parameters["CollPoints"];
			this.mCandidateParameter = base.Parameters["Candidate"];
			this.mDepthMapParameter = base.Parameters["DepthMap"];
			this.mHalfPixelParameter = base.Parameters["HalfPixel"];
		}

		// Token: 0x17000205 RID: 517
		// (get) Token: 0x060009A4 RID: 2468 RVA: 0x0003B6B7 File Offset: 0x000398B7
		// (set) Token: 0x060009A5 RID: 2469 RVA: 0x0003B6C4 File Offset: 0x000398C4
		public Matrix World
		{
			get
			{
				return this.mWorldParameter.GetValueMatrix();
			}
			set
			{
				this.mWorldParameter.SetValue(value);
			}
		}

		// Token: 0x17000206 RID: 518
		// (get) Token: 0x060009A6 RID: 2470 RVA: 0x0003B6D2 File Offset: 0x000398D2
		// (set) Token: 0x060009A7 RID: 2471 RVA: 0x0003B6DF File Offset: 0x000398DF
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

		// Token: 0x17000207 RID: 519
		// (get) Token: 0x060009A8 RID: 2472 RVA: 0x0003B6ED File Offset: 0x000398ED
		// (set) Token: 0x060009A9 RID: 2473 RVA: 0x0003B6FA File Offset: 0x000398FA
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

		// Token: 0x17000208 RID: 520
		// (get) Token: 0x060009AA RID: 2474 RVA: 0x0003B708 File Offset: 0x00039908
		// (set) Token: 0x060009AB RID: 2475 RVA: 0x0003B715 File Offset: 0x00039915
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

		// Token: 0x17000209 RID: 521
		// (get) Token: 0x060009AC RID: 2476 RVA: 0x0003B723 File Offset: 0x00039923
		// (set) Token: 0x060009AD RID: 2477 RVA: 0x0003B730 File Offset: 0x00039930
		public Vector3 EyeOfTheBeholder
		{
			get
			{
				return this.mEyeOfTheBeholderParameter.GetValueVector3();
			}
			set
			{
				this.mEyeOfTheBeholderParameter.SetValue(value);
			}
		}

		// Token: 0x1700020A RID: 522
		// (get) Token: 0x060009AE RID: 2478 RVA: 0x0003B73E File Offset: 0x0003993E
		// (set) Token: 0x060009AF RID: 2479 RVA: 0x0003B74B File Offset: 0x0003994B
		public bool VertexColorEnabled
		{
			get
			{
				return this.mVertexColorEnabledParameter.GetValueBoolean();
			}
			set
			{
				this.mVertexColorEnabledParameter.SetValue(value);
			}
		}

		// Token: 0x1700020B RID: 523
		// (get) Token: 0x060009B0 RID: 2480 RVA: 0x0003B759 File Offset: 0x00039959
		// (set) Token: 0x060009B1 RID: 2481 RVA: 0x0003B766 File Offset: 0x00039966
		public Vector3 Color
		{
			get
			{
				return this.mColorParameter.GetValueVector3();
			}
			set
			{
				this.mColorParameter.SetValue(value);
			}
		}

		// Token: 0x1700020C RID: 524
		// (get) Token: 0x060009B2 RID: 2482 RVA: 0x0003B774 File Offset: 0x00039974
		// (set) Token: 0x060009B3 RID: 2483 RVA: 0x0003B781 File Offset: 0x00039981
		public float Width
		{
			get
			{
				return this.mWidthParameter.GetValueSingle();
			}
			set
			{
				this.mWidthParameter.SetValue(value);
			}
		}

		// Token: 0x1700020D RID: 525
		// (get) Token: 0x060009B4 RID: 2484 RVA: 0x0003B78F File Offset: 0x0003998F
		// (set) Token: 0x060009B5 RID: 2485 RVA: 0x0003B79C File Offset: 0x0003999C
		public float AlphaPower
		{
			get
			{
				return this.mAlphaPowerParameter.GetValueSingle();
			}
			set
			{
				this.mAlphaPowerParameter.SetValue(value);
			}
		}

		// Token: 0x1700020E RID: 526
		// (get) Token: 0x060009B6 RID: 2486 RVA: 0x0003B7AA File Offset: 0x000399AA
		// (set) Token: 0x060009B7 RID: 2487 RVA: 0x0003B7B7 File Offset: 0x000399B7
		public float AlphaFalloffPower
		{
			get
			{
				return this.mAlphaFalloffPowerParameter.GetValueSingle();
			}
			set
			{
				this.mAlphaFalloffPowerParameter.SetValue(value);
			}
		}

		// Token: 0x1700020F RID: 527
		// (get) Token: 0x060009B8 RID: 2488 RVA: 0x0003B7C5 File Offset: 0x000399C5
		// (set) Token: 0x060009B9 RID: 2489 RVA: 0x0003B7D2 File Offset: 0x000399D2
		public float MaxRadius
		{
			get
			{
				return this.mMaxRadiusParameter.GetValueSingle();
			}
			set
			{
				this.mMaxRadiusParameter.SetValue(value);
			}
		}

		// Token: 0x17000210 RID: 528
		// (get) Token: 0x060009BA RID: 2490 RVA: 0x0003B7E0 File Offset: 0x000399E0
		// (set) Token: 0x060009BB RID: 2491 RVA: 0x0003B7ED File Offset: 0x000399ED
		public float RippleDistortion
		{
			get
			{
				return this.mRippleDistortionParameter.GetValueSingle();
			}
			set
			{
				this.mRippleDistortionParameter.SetValue(value);
			}
		}

		// Token: 0x17000211 RID: 529
		// (get) Token: 0x060009BC RID: 2492 RVA: 0x0003B7FB File Offset: 0x000399FB
		// (set) Token: 0x060009BD RID: 2493 RVA: 0x0003B808 File Offset: 0x00039A08
		public float MapDistortion
		{
			get
			{
				return this.mMapDistortionParameter.GetValueSingle();
			}
			set
			{
				this.mMapDistortionParameter.SetValue(value);
			}
		}

		// Token: 0x17000212 RID: 530
		// (get) Token: 0x060009BE RID: 2494 RVA: 0x0003B816 File Offset: 0x00039A16
		// (set) Token: 0x060009BF RID: 2495 RVA: 0x0003B823 File Offset: 0x00039A23
		public Texture2D DisplacementMap
		{
			get
			{
				return this.mDisplacementMapParameter.GetValueTexture2D();
			}
			set
			{
				this.mDisplacementMapParameter.SetValue(value);
			}
		}

		// Token: 0x17000213 RID: 531
		// (get) Token: 0x060009C0 RID: 2496 RVA: 0x0003B831 File Offset: 0x00039A31
		// (set) Token: 0x060009C1 RID: 2497 RVA: 0x0003B840 File Offset: 0x00039A40
		public Vector4[] CollPoints
		{
			get
			{
				return this.mCollPointsParameter.GetValueVector4Array(32);
			}
			set
			{
				this.mCollPointsParameter.SetValue(value);
			}
		}

		// Token: 0x17000214 RID: 532
		// (get) Token: 0x060009C2 RID: 2498 RVA: 0x0003B84E File Offset: 0x00039A4E
		// (set) Token: 0x060009C3 RID: 2499 RVA: 0x0003B85B File Offset: 0x00039A5B
		public Texture2D Candidate
		{
			get
			{
				return this.mCandidateParameter.GetValueTexture2D();
			}
			set
			{
				this.mCandidateParameter.SetValue(value);
			}
		}

		// Token: 0x17000215 RID: 533
		// (get) Token: 0x060009C4 RID: 2500 RVA: 0x0003B869 File Offset: 0x00039A69
		// (set) Token: 0x060009C5 RID: 2501 RVA: 0x0003B876 File Offset: 0x00039A76
		public Texture2D DepthMap
		{
			get
			{
				return this.mDepthMapParameter.GetValueTexture2D();
			}
			set
			{
				this.mDepthMapParameter.SetValue(value);
			}
		}

		// Token: 0x17000216 RID: 534
		// (get) Token: 0x060009C6 RID: 2502 RVA: 0x0003B884 File Offset: 0x00039A84
		// (set) Token: 0x060009C7 RID: 2503 RVA: 0x0003B8D0 File Offset: 0x00039AD0
		public Point DestinationDimentions
		{
			get
			{
				Point result = default(Point);
				Vector2 valueVector = this.mHalfPixelParameter.GetValueVector2();
				result.X = (int)(0.5f / valueVector.X);
				result.Y = (int)(0.5f / valueVector.Y);
				return result;
			}
			set
			{
				Vector2 value2 = default(Vector2);
				value2.X = 0.5f / (float)value.X;
				value2.Y = 0.5f / (float)value.Y;
				this.mHalfPixelParameter.SetValue(value2);
			}
		}

		// Token: 0x040008DF RID: 2271
		public const int NR_COLL_PTS = 32;

		// Token: 0x040008E0 RID: 2272
		public static readonly int TYPEHASH = typeof(ForceFieldEffect).GetHashCode();

		// Token: 0x040008E1 RID: 2273
		private EffectParameter mWorldParameter;

		// Token: 0x040008E2 RID: 2274
		private EffectParameter mViewParameter;

		// Token: 0x040008E3 RID: 2275
		private EffectParameter mProjectionParameter;

		// Token: 0x040008E4 RID: 2276
		private EffectParameter mCameraPositionParameter;

		// Token: 0x040008E5 RID: 2277
		private EffectParameter mEyeOfTheBeholderParameter;

		// Token: 0x040008E6 RID: 2278
		private EffectParameter mVertexColorEnabledParameter;

		// Token: 0x040008E7 RID: 2279
		private EffectParameter mColorParameter;

		// Token: 0x040008E8 RID: 2280
		private EffectParameter mWidthParameter;

		// Token: 0x040008E9 RID: 2281
		private EffectParameter mAlphaPowerParameter;

		// Token: 0x040008EA RID: 2282
		private EffectParameter mAlphaFalloffPowerParameter;

		// Token: 0x040008EB RID: 2283
		private EffectParameter mMaxRadiusParameter;

		// Token: 0x040008EC RID: 2284
		private EffectParameter mRippleDistortionParameter;

		// Token: 0x040008ED RID: 2285
		private EffectParameter mMapDistortionParameter;

		// Token: 0x040008EE RID: 2286
		private EffectParameter mDisplacementMapParameter;

		// Token: 0x040008EF RID: 2287
		private EffectParameter mCollPointsParameter;

		// Token: 0x040008F0 RID: 2288
		private EffectParameter mCandidateParameter;

		// Token: 0x040008F1 RID: 2289
		private EffectParameter mDepthMapParameter;

		// Token: 0x040008F2 RID: 2290
		private EffectParameter mHalfPixelParameter;
	}
}
