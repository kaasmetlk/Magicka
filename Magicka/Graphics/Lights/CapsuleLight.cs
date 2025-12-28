using System;
using Magicka.Graphics.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Lights;

namespace Magicka.Graphics.Lights
{
	// Token: 0x020005A0 RID: 1440
	public class CapsuleLight : Light
	{
		// Token: 0x06002AF7 RID: 10999 RVA: 0x00152080 File Offset: 0x00150280
		public CapsuleLight(ContentManager iContent)
		{
			if (CapsuleLight.sMesh == null)
			{
				lock (Game.Instance.GraphicsDevice)
				{
					CapsuleLight.sMesh = iContent.Load<Model>("models/effects/Capsule").Meshes[0];
				}
			}
			CapsuleLightEffect capsuleLightEffect = RenderManager.Instance.GetEffect(CapsuleLightEffect.TYPEHASH) as CapsuleLightEffect;
			if (capsuleLightEffect == null)
			{
				lock (Game.Instance.GraphicsDevice)
				{
					capsuleLightEffect = new CapsuleLightEffect(Game.Instance.GraphicsDevice, iContent);
				}
				RenderManager.Instance.RegisterEffect(capsuleLightEffect);
			}
			this.mDevice = capsuleLightEffect.GraphicsDevice;
		}

		// Token: 0x06002AF8 RID: 11000 RVA: 0x00152150 File Offset: 0x00150350
		protected void UpdateMatrices()
		{
			this.mMatrixDirty = false;
			Vector3.Distance(ref this.mStart, ref this.mEnd, out this.mLength);
			Vector3 vector;
			Vector3.Add(ref this.mStart, ref this.mEnd, out vector);
			Vector3.Multiply(ref vector, 0.5f, out vector);
			Vector3 vector2;
			if (this.mEnd.X == this.mStart.X && this.mEnd.Z == this.mStart.Z)
			{
				vector2 = Vector3.Backward;
			}
			else
			{
				vector2 = Vector3.Up;
			}
			Matrix.CreateLookAt(ref vector, ref this.mEnd, ref vector2, out this.mLightOrientation);
			Matrix.Invert(ref this.mLightOrientation, out this.mLightOrientation);
		}

		// Token: 0x17000A10 RID: 2576
		// (get) Token: 0x06002AF9 RID: 11001 RVA: 0x00152200 File Offset: 0x00150400
		// (set) Token: 0x06002AFA RID: 11002 RVA: 0x00152208 File Offset: 0x00150408
		public Vector3 Start
		{
			get
			{
				return this.mStart;
			}
			set
			{
				this.mStart = value;
				this.mMatrixDirty = true;
			}
		}

		// Token: 0x17000A11 RID: 2577
		// (get) Token: 0x06002AFB RID: 11003 RVA: 0x00152218 File Offset: 0x00150418
		// (set) Token: 0x06002AFC RID: 11004 RVA: 0x00152220 File Offset: 0x00150420
		public Vector3 End
		{
			get
			{
				return this.mEnd;
			}
			set
			{
				this.mEnd = value;
				this.mMatrixDirty = true;
			}
		}

		// Token: 0x17000A12 RID: 2578
		// (get) Token: 0x06002AFD RID: 11005 RVA: 0x00152230 File Offset: 0x00150430
		// (set) Token: 0x06002AFE RID: 11006 RVA: 0x00152238 File Offset: 0x00150438
		public float Radius
		{
			get
			{
				return this.mRadius;
			}
			set
			{
				this.mRadius = value;
			}
		}

		// Token: 0x06002AFF RID: 11007 RVA: 0x00152241 File Offset: 0x00150441
		protected override void Update(DataChannel iDataChannel, float iDeltaTime, ref Vector3 iCameraPosition, ref Vector3 iCameraDirection)
		{
		}

		// Token: 0x06002B00 RID: 11008 RVA: 0x00152244 File Offset: 0x00150444
		public override void Draw(Effect iEffect, DataChannel iDataChannel, float iDeltaTime, Texture2D iNormalMap, Texture2D iDepthMap)
		{
			if (this.mMatrixDirty)
			{
				this.UpdateMatrices();
			}
			CapsuleLightEffect capsuleLightEffect = iEffect as CapsuleLightEffect;
			capsuleLightEffect.Length = this.mLength;
			capsuleLightEffect.World = this.mLightOrientation;
			capsuleLightEffect.Start = this.mStart;
			capsuleLightEffect.End = this.mEnd;
			capsuleLightEffect.Radius = this.mRadius;
			Vector3 diffuseColor;
			Vector3.Multiply(ref this.mDiffuseColor, this.mIntensity, out diffuseColor);
			Vector3 ambientColor;
			Vector3.Multiply(ref this.mAmbientColor, this.mIntensity, out ambientColor);
			capsuleLightEffect.DiffuseColor = diffuseColor;
			capsuleLightEffect.AmbientColor = ambientColor;
			capsuleLightEffect.NormalMap = iNormalMap;
			capsuleLightEffect.DepthMap = iDepthMap;
			Vector2 halfPixel = default(Vector2);
			Point screenSize = RenderManager.Instance.ScreenSize;
			halfPixel.X = 0.5f / (float)screenSize.X;
			halfPixel.Y = 0.5f / (float)screenSize.Y;
			capsuleLightEffect.HalfPixel = halfPixel;
			capsuleLightEffect.CommitChanges();
			ModelMeshPart modelMeshPart = CapsuleLight.sMesh.MeshParts[0];
			this.mDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, modelMeshPart.BaseVertex, 0, modelMeshPart.NumVertices, modelMeshPart.StartIndex, modelMeshPart.PrimitiveCount);
		}

		// Token: 0x17000A13 RID: 2579
		// (get) Token: 0x06002B01 RID: 11009 RVA: 0x0015236C File Offset: 0x0015056C
		// (set) Token: 0x06002B02 RID: 11010 RVA: 0x00152374 File Offset: 0x00150574
		public override Vector3 DiffuseColor
		{
			get
			{
				return this.mDiffuseColor;
			}
			set
			{
				this.mDiffuseColor = value;
			}
		}

		// Token: 0x17000A14 RID: 2580
		// (get) Token: 0x06002B03 RID: 11011 RVA: 0x0015237D File Offset: 0x0015057D
		// (set) Token: 0x06002B04 RID: 11012 RVA: 0x00152385 File Offset: 0x00150585
		public override Vector3 AmbientColor
		{
			get
			{
				return this.mAmbientColor;
			}
			set
			{
				this.mAmbientColor = value;
			}
		}

		// Token: 0x17000A15 RID: 2581
		// (get) Token: 0x06002B05 RID: 11013 RVA: 0x0015238E File Offset: 0x0015058E
		// (set) Token: 0x06002B06 RID: 11014 RVA: 0x00152396 File Offset: 0x00150596
		public float Intensity
		{
			get
			{
				return this.mIntensity;
			}
			set
			{
				this.mIntensity = value;
			}
		}

		// Token: 0x17000A16 RID: 2582
		// (get) Token: 0x06002B07 RID: 11015 RVA: 0x0015239F File Offset: 0x0015059F
		public override int Effect
		{
			get
			{
				return CapsuleLightEffect.TYPEHASH;
			}
		}

		// Token: 0x17000A17 RID: 2583
		// (get) Token: 0x06002B08 RID: 11016 RVA: 0x001523A6 File Offset: 0x001505A6
		public override int Technique
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x17000A18 RID: 2584
		// (get) Token: 0x06002B09 RID: 11017 RVA: 0x001523A9 File Offset: 0x001505A9
		public override int VertexStride
		{
			get
			{
				return CapsuleLight.sMesh.MeshParts[0].VertexStride;
			}
		}

		// Token: 0x17000A19 RID: 2585
		// (get) Token: 0x06002B0A RID: 11018 RVA: 0x001523C0 File Offset: 0x001505C0
		public override VertexBuffer VertexBuffer
		{
			get
			{
				return CapsuleLight.sMesh.VertexBuffer;
			}
		}

		// Token: 0x17000A1A RID: 2586
		// (get) Token: 0x06002B0B RID: 11019 RVA: 0x001523CC File Offset: 0x001505CC
		public override IndexBuffer IndexBuffer
		{
			get
			{
				return CapsuleLight.sMesh.IndexBuffer;
			}
		}

		// Token: 0x17000A1B RID: 2587
		// (get) Token: 0x06002B0C RID: 11020 RVA: 0x001523D8 File Offset: 0x001505D8
		public override VertexDeclaration VertexDeclaration
		{
			get
			{
				return CapsuleLight.sMesh.MeshParts[0].VertexDeclaration;
			}
		}

		// Token: 0x06002B0D RID: 11021 RVA: 0x001523EF File Offset: 0x001505EF
		public override bool ShouldDraw(BoundingFrustum iViewFrustum)
		{
			return true;
		}

		// Token: 0x17000A1C RID: 2588
		// (get) Token: 0x06002B0E RID: 11022 RVA: 0x001523F2 File Offset: 0x001505F2
		// (set) Token: 0x06002B0F RID: 11023 RVA: 0x001523F9 File Offset: 0x001505F9
		public override float SpecularAmount
		{
			get
			{
				return 0f;
			}
			set
			{
			}
		}

		// Token: 0x17000A1D RID: 2589
		// (get) Token: 0x06002B10 RID: 11024 RVA: 0x001523FB File Offset: 0x001505FB
		// (set) Token: 0x06002B11 RID: 11025 RVA: 0x001523FE File Offset: 0x001505FE
		public override bool CastShadows
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		// Token: 0x17000A1E RID: 2590
		// (get) Token: 0x06002B12 RID: 11026 RVA: 0x00152400 File Offset: 0x00150600
		// (set) Token: 0x06002B13 RID: 11027 RVA: 0x00152403 File Offset: 0x00150603
		public override int ShadowMapSize
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		// Token: 0x06002B14 RID: 11028 RVA: 0x00152405 File Offset: 0x00150605
		public override void DisposeShadowMap()
		{
		}

		// Token: 0x06002B15 RID: 11029 RVA: 0x00152407 File Offset: 0x00150607
		public override void CreateShadowMap()
		{
		}

		// Token: 0x06002B16 RID: 11030 RVA: 0x00152409 File Offset: 0x00150609
		public override void DrawShadows(DataChannel iDataChannel, float iDeltaTime, Scene iScene)
		{
		}

		// Token: 0x04002E32 RID: 11826
		private static ModelMesh sMesh;

		// Token: 0x04002E33 RID: 11827
		private float mRadius;

		// Token: 0x04002E34 RID: 11828
		private float mLength;

		// Token: 0x04002E35 RID: 11829
		private Vector3 mStart;

		// Token: 0x04002E36 RID: 11830
		private Vector3 mEnd;

		// Token: 0x04002E37 RID: 11831
		private Matrix mLightOrientation;

		// Token: 0x04002E38 RID: 11832
		private bool mMatrixDirty = true;

		// Token: 0x04002E39 RID: 11833
		private Vector3 mDiffuseColor;

		// Token: 0x04002E3A RID: 11834
		private Vector3 mAmbientColor;

		// Token: 0x04002E3B RID: 11835
		private new float mIntensity;

		// Token: 0x04002E3C RID: 11836
		private GraphicsDevice mDevice;
	}
}
