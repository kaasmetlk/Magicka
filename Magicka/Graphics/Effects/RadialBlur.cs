using System;
using System.Collections.Generic;
using Magicka.GameLogic;
using Magicka.GameLogic.Spells;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;

namespace Magicka.Graphics.Effects
{
	// Token: 0x020000C0 RID: 192
	public class RadialBlur : Effect, IPostEffect, IAbilityEffect
	{
		// Token: 0x06000593 RID: 1427 RVA: 0x00020BEC File Offset: 0x0001EDEC
		private RadialBlur(GraphicsDevice iDevice, ContentManager iContentManager) : base(iDevice, iContentManager.Load<Effect>("Shaders/RadialBlur"))
		{
			this.mWorldParameter = base.Parameters["World"];
			this.mViewParameter = base.Parameters["View"];
			this.mProjectionParameter = base.Parameters["Projection"];
			this.mFarDistanceParameter = base.Parameters["FarDistance"];
			this.mStartRadiusParameter = base.Parameters["StartRadius"];
			this.mMidRadiusParameter = base.Parameters["MidRadius"];
			this.mEndRadiusParameter = base.Parameters["EndRadius"];
			this.mSpreadParameter = base.Parameters["Spread"];
			this.mAlphaParameter = base.Parameters["Alpha"];
			this.mPixelSizeParameter = base.Parameters["PixelSize"];
			this.mSourceTextureParameter = base.Parameters["SourceTexture"];
			this.mDepthTextureParameter = base.Parameters["DepthTexture"];
			RadialBlur.RadialBlurVertex[] array = new RadialBlur.RadialBlurVertex[51];
			for (int i = 0; i <= 16; i++)
			{
				RadialBlur.RadialBlurVertex radialBlurVertex = default(RadialBlur.RadialBlurVertex);
				radialBlurVertex.RotationOffset = 1f - (float)i * 2f / 16f;
				radialBlurVertex.Amount = 0f;
				radialBlurVertex.StartOffset = 1f;
				array[i * 3] = radialBlurVertex;
				radialBlurVertex.Amount = 1f;
				radialBlurVertex.StartOffset = 0f;
				radialBlurVertex.MidOffset = 1f;
				array[i * 3 + 1] = radialBlurVertex;
				radialBlurVertex.Amount = 0f;
				radialBlurVertex.MidOffset = 0f;
				radialBlurVertex.EndOffset = 1f;
				array[i * 3 + 2] = radialBlurVertex;
			}
			ushort[] array2 = new ushort[192];
			for (int j = 0; j < 16; j++)
			{
				array2[j * 12] = (ushort)(j * 3 + 3);
				array2[j * 12 + 1] = (ushort)(j * 3 + 1);
				array2[j * 12 + 2] = (ushort)(j * 3);
				array2[j * 12 + 3] = (ushort)(j * 3 + 3);
				array2[j * 12 + 4] = (ushort)(j * 3 + 4);
				array2[j * 12 + 5] = (ushort)(j * 3 + 1);
				array2[j * 12 + 6] = (ushort)(j * 3 + 4);
				array2[j * 12 + 7] = (ushort)(j * 3 + 2);
				array2[j * 12 + 8] = (ushort)(j * 3 + 1);
				array2[j * 12 + 9] = (ushort)(j * 3 + 4);
				array2[j * 12 + 10] = (ushort)(j * 3 + 5);
				array2[j * 12 + 11] = (ushort)(j * 3 + 2);
			}
			this.mVertices = new VertexBuffer(iDevice, 20 * array.Length, BufferUsage.WriteOnly);
			this.mVertices.SetData<RadialBlur.RadialBlurVertex>(array);
			this.mIndices = new IndexBuffer(iDevice, 2 * array2.Length, BufferUsage.WriteOnly, IndexElementSize.SixteenBits);
			this.mIndices.SetData<ushort>(array2);
			this.mDeclaration = new VertexDeclaration(iDevice, RadialBlur.RadialBlurVertex.VertexElements);
			this.mNumVertices = array.Length;
			this.mPrimitiveCount = array2.Length / 3;
		}

		// Token: 0x06000594 RID: 1428 RVA: 0x00020F28 File Offset: 0x0001F128
		public static void InitializeCache(ContentManager iContent, int iSize)
		{
			RadialBlur.mContent = iContent;
			RadialBlur.mCache = new List<RadialBlur>(iSize);
			GraphicsDevice graphicsDevice = Game.Instance.GraphicsDevice;
			for (int i = 0; i < iSize; i++)
			{
				lock (graphicsDevice)
				{
					RadialBlur.mCache.Add(new RadialBlur(graphicsDevice, RadialBlur.mContent));
				}
			}
		}

		// Token: 0x06000595 RID: 1429 RVA: 0x00020F94 File Offset: 0x0001F194
		public static RadialBlur GetRadialBlur()
		{
			if (RadialBlur.mCache.Count > 0)
			{
				RadialBlur result = RadialBlur.mCache[RadialBlur.mCache.Count - 1];
				RadialBlur.mCache.RemoveAt(RadialBlur.mCache.Count - 1);
				return result;
			}
			return new RadialBlur(Game.Instance.GraphicsDevice, Game.Instance.Content);
		}

		// Token: 0x170000F1 RID: 241
		// (get) Token: 0x06000596 RID: 1430 RVA: 0x00020FF6 File Offset: 0x0001F1F6
		// (set) Token: 0x06000597 RID: 1431 RVA: 0x00021003 File Offset: 0x0001F203
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

		// Token: 0x170000F2 RID: 242
		// (get) Token: 0x06000598 RID: 1432 RVA: 0x00021011 File Offset: 0x0001F211
		// (set) Token: 0x06000599 RID: 1433 RVA: 0x0002101E File Offset: 0x0001F21E
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

		// Token: 0x170000F3 RID: 243
		// (get) Token: 0x0600059A RID: 1434 RVA: 0x0002102C File Offset: 0x0001F22C
		// (set) Token: 0x0600059B RID: 1435 RVA: 0x00021039 File Offset: 0x0001F239
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

		// Token: 0x170000F4 RID: 244
		// (get) Token: 0x0600059C RID: 1436 RVA: 0x00021047 File Offset: 0x0001F247
		// (set) Token: 0x0600059D RID: 1437 RVA: 0x00021054 File Offset: 0x0001F254
		public float Angle
		{
			get
			{
				return this.mSpreadParameter.GetValueSingle();
			}
			set
			{
				this.mSpreadParameter.SetValue(MathHelper.Clamp(value, 0f, 3.1415927f));
			}
		}

		// Token: 0x170000F5 RID: 245
		// (get) Token: 0x0600059E RID: 1438 RVA: 0x00021071 File Offset: 0x0001F271
		// (set) Token: 0x0600059F RID: 1439 RVA: 0x0002107E File Offset: 0x0001F27E
		public float FarDistance
		{
			get
			{
				return this.mFarDistanceParameter.GetValueSingle();
			}
			set
			{
				this.mFarDistanceParameter.SetValue(value);
			}
		}

		// Token: 0x170000F6 RID: 246
		// (get) Token: 0x060005A0 RID: 1440 RVA: 0x0002108C File Offset: 0x0001F28C
		// (set) Token: 0x060005A1 RID: 1441 RVA: 0x00021099 File Offset: 0x0001F299
		public float StartRadius
		{
			get
			{
				return this.mStartRadiusParameter.GetValueSingle();
			}
			set
			{
				this.mStartRadiusParameter.SetValue(value);
			}
		}

		// Token: 0x170000F7 RID: 247
		// (get) Token: 0x060005A2 RID: 1442 RVA: 0x000210A7 File Offset: 0x0001F2A7
		// (set) Token: 0x060005A3 RID: 1443 RVA: 0x000210B4 File Offset: 0x0001F2B4
		public float MidRadius
		{
			get
			{
				return this.mMidRadiusParameter.GetValueSingle();
			}
			set
			{
				this.mMidRadiusParameter.SetValue(value);
			}
		}

		// Token: 0x170000F8 RID: 248
		// (get) Token: 0x060005A4 RID: 1444 RVA: 0x000210C2 File Offset: 0x0001F2C2
		// (set) Token: 0x060005A5 RID: 1445 RVA: 0x000210CF File Offset: 0x0001F2CF
		public float EndRadius
		{
			get
			{
				return this.mEndRadiusParameter.GetValueSingle();
			}
			set
			{
				this.mEndRadiusParameter.SetValue(value);
			}
		}

		// Token: 0x170000F9 RID: 249
		// (get) Token: 0x060005A6 RID: 1446 RVA: 0x000210DD File Offset: 0x0001F2DD
		// (set) Token: 0x060005A7 RID: 1447 RVA: 0x000210EA File Offset: 0x0001F2EA
		public float Alpha
		{
			get
			{
				return this.mAlphaParameter.GetValueSingle();
			}
			set
			{
				this.mAlphaParameter.SetValue(value);
			}
		}

		// Token: 0x170000FA RID: 250
		// (get) Token: 0x060005A8 RID: 1448 RVA: 0x000210F8 File Offset: 0x0001F2F8
		// (set) Token: 0x060005A9 RID: 1449 RVA: 0x00021105 File Offset: 0x0001F305
		public Vector2 PixelSize
		{
			get
			{
				return this.mPixelSizeParameter.GetValueVector2();
			}
			set
			{
				this.mPixelSizeParameter.SetValue(value);
			}
		}

		// Token: 0x170000FB RID: 251
		// (get) Token: 0x060005AA RID: 1450 RVA: 0x00021113 File Offset: 0x0001F313
		// (set) Token: 0x060005AB RID: 1451 RVA: 0x00021120 File Offset: 0x0001F320
		public Texture2D SourceTexture
		{
			get
			{
				return this.mSourceTextureParameter.GetValueTexture2D();
			}
			set
			{
				this.mSourceTextureParameter.SetValue(value);
			}
		}

		// Token: 0x170000FC RID: 252
		// (get) Token: 0x060005AC RID: 1452 RVA: 0x0002112E File Offset: 0x0001F32E
		// (set) Token: 0x060005AD RID: 1453 RVA: 0x0002113B File Offset: 0x0001F33B
		public Texture2D DepthTexture
		{
			get
			{
				return this.mDepthTextureParameter.GetValueTexture2D();
			}
			set
			{
				this.mDepthTextureParameter.SetValue(value);
			}
		}

		// Token: 0x170000FD RID: 253
		// (get) Token: 0x060005AE RID: 1454 RVA: 0x00021149 File Offset: 0x0001F349
		public bool Dead
		{
			get
			{
				return this.mTTL <= 0f;
			}
		}

		// Token: 0x060005AF RID: 1455 RVA: 0x0002115C File Offset: 0x0001F35C
		public void Initialize(ref Vector3 iPosition, float iMaxRadius, float iTTL, Scene iScene)
		{
			Vector3 forward = Vector3.Forward;
			this.Initialize(ref iPosition, ref forward, 3.1415927f, iMaxRadius, iTTL, iScene);
		}

		// Token: 0x060005B0 RID: 1456 RVA: 0x00021184 File Offset: 0x0001F384
		public void Initialize(ref Vector3 iPosition, ref Vector3 iDirection, float iAngle, float iRadius, float iTTL, Scene iScene)
		{
			Matrix identity = Matrix.Identity;
			Vector3 up = Vector3.Up;
			Vector3 right;
			Vector3.Cross(ref iDirection, ref up, out right);
			Vector3.Cross(ref right, ref iDirection, out up);
			up.Normalize();
			right.Normalize();
			identity.Up = up;
			identity.Right = right;
			identity.Forward = iDirection;
			identity.Translation = iPosition;
			this.World = identity;
			this.Angle = iAngle;
			this.mTTL = iTTL;
			this.mLifeTime = iTTL;
			this.mMaxRadius = iRadius * 1.5f;
			this.mScene = iScene;
			SpellManager.Instance.AddSpellEffect(this);
		}

		// Token: 0x060005B1 RID: 1457 RVA: 0x00021228 File Offset: 0x0001F428
		public void Draw(float iDeltaTime, ref Vector2 iPixelSize, ref Matrix iViewMatrix, ref Matrix iProjectionMatrix, Texture2D iCandidate, Texture2D iDepthMap, Texture2D iNormalMap)
		{
			this.mTTL -= iDeltaTime;
			float num = this.mTTL / this.mLifeTime;
			float num2 = (float)Math.Pow((double)(1f - num), 0.10000000149011612) * this.mMaxRadius;
			this.Alpha = (float)Math.Sqrt((double)(num * (1f - num) * 4f));
			this.StartRadius = num2 * 0f;
			this.MidRadius = num2 * 0.666f;
			this.EndRadius = num2;
			this.PixelSize = iPixelSize;
			this.View = iViewMatrix;
			this.Projection = iProjectionMatrix;
			this.SourceTexture = iCandidate;
			this.DepthTexture = iDepthMap;
			base.GraphicsDevice.VertexDeclaration = this.mDeclaration;
			base.GraphicsDevice.Vertices[0].SetSource(this.mVertices, 0, 20);
			base.GraphicsDevice.Indices = this.mIndices;
			base.Begin();
			for (int i = 0; i < base.CurrentTechnique.Passes.Count; i++)
			{
				EffectPass effectPass = base.CurrentTechnique.Passes[i];
				effectPass.Begin();
				base.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, this.mNumVertices, 0, this.mPrimitiveCount);
				effectPass.End();
			}
			base.End();
		}

		// Token: 0x060005B2 RID: 1458 RVA: 0x00021383 File Offset: 0x0001F583
		public void Kill()
		{
			this.mTTL = 0f;
		}

		// Token: 0x170000FE RID: 254
		// (get) Token: 0x060005B3 RID: 1459 RVA: 0x00021390 File Offset: 0x0001F590
		public int ZIndex
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x060005B4 RID: 1460 RVA: 0x00021394 File Offset: 0x0001F594
		internal static void DisposeCache()
		{
			for (int i = 0; i < RadialBlur.mCache.Count; i++)
			{
				RadialBlur.mCache[i].Dispose();
			}
		}

		// Token: 0x170000FF RID: 255
		// (get) Token: 0x060005B5 RID: 1461 RVA: 0x000213C6 File Offset: 0x0001F5C6
		public bool IsDead
		{
			get
			{
				return this.mTTL <= 0f;
			}
		}

		// Token: 0x060005B6 RID: 1462 RVA: 0x000213D8 File Offset: 0x0001F5D8
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			this.mScene.AddPostEffect(iDataChannel, this);
		}

		// Token: 0x060005B7 RID: 1463 RVA: 0x000213E7 File Offset: 0x0001F5E7
		public void OnRemove()
		{
			RadialBlur.mCache.Add(this);
		}

		// Token: 0x04000455 RID: 1109
		private static List<RadialBlur> mCache;

		// Token: 0x04000456 RID: 1110
		private static ContentManager mContent;

		// Token: 0x04000457 RID: 1111
		private EffectParameter mWorldParameter;

		// Token: 0x04000458 RID: 1112
		private EffectParameter mViewParameter;

		// Token: 0x04000459 RID: 1113
		private EffectParameter mProjectionParameter;

		// Token: 0x0400045A RID: 1114
		private EffectParameter mFarDistanceParameter;

		// Token: 0x0400045B RID: 1115
		private EffectParameter mStartRadiusParameter;

		// Token: 0x0400045C RID: 1116
		private EffectParameter mMidRadiusParameter;

		// Token: 0x0400045D RID: 1117
		private EffectParameter mEndRadiusParameter;

		// Token: 0x0400045E RID: 1118
		private EffectParameter mSpreadParameter;

		// Token: 0x0400045F RID: 1119
		private EffectParameter mAlphaParameter;

		// Token: 0x04000460 RID: 1120
		private EffectParameter mPixelSizeParameter;

		// Token: 0x04000461 RID: 1121
		private EffectParameter mSourceTextureParameter;

		// Token: 0x04000462 RID: 1122
		private EffectParameter mDepthTextureParameter;

		// Token: 0x04000463 RID: 1123
		private VertexDeclaration mDeclaration;

		// Token: 0x04000464 RID: 1124
		private VertexBuffer mVertices;

		// Token: 0x04000465 RID: 1125
		private IndexBuffer mIndices;

		// Token: 0x04000466 RID: 1126
		private int mPrimitiveCount;

		// Token: 0x04000467 RID: 1127
		private int mNumVertices;

		// Token: 0x04000468 RID: 1128
		private float mTTL;

		// Token: 0x04000469 RID: 1129
		private float mLifeTime;

		// Token: 0x0400046A RID: 1130
		private float mMaxRadius;

		// Token: 0x0400046B RID: 1131
		private Scene mScene;

		// Token: 0x020000C1 RID: 193
		protected struct RadialBlurVertex
		{
			// Token: 0x0400046C RID: 1132
			public const int SizeInBytes = 20;

			// Token: 0x0400046D RID: 1133
			public float StartOffset;

			// Token: 0x0400046E RID: 1134
			public float MidOffset;

			// Token: 0x0400046F RID: 1135
			public float EndOffset;

			// Token: 0x04000470 RID: 1136
			public float RotationOffset;

			// Token: 0x04000471 RID: 1137
			public float Amount;

			// Token: 0x04000472 RID: 1138
			public static readonly VertexElement[] VertexElements = new VertexElement[]
			{
				new VertexElement(0, 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Normal, 0),
				new VertexElement(0, 12, VertexElementFormat.Single, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0),
				new VertexElement(0, 16, VertexElementFormat.Single, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 1)
			};
		}
	}
}
