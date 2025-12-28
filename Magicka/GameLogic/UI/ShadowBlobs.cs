using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;

namespace Magicka.GameLogic.UI
{
	// Token: 0x020005D8 RID: 1496
	public class ShadowBlobs
	{
		// Token: 0x17000A97 RID: 2711
		// (get) Token: 0x06002CBD RID: 11453 RVA: 0x0015EE4C File Offset: 0x0015D04C
		public static ShadowBlobs Instance
		{
			get
			{
				if (ShadowBlobs.mSingelton == null)
				{
					lock (ShadowBlobs.mSingeltonLock)
					{
						if (ShadowBlobs.mSingelton == null)
						{
							ShadowBlobs.mSingelton = new ShadowBlobs();
						}
					}
				}
				return ShadowBlobs.mSingelton;
			}
		}

		// Token: 0x06002CBE RID: 11454 RVA: 0x0015EEA0 File Offset: 0x0015D0A0
		private ShadowBlobs()
		{
			this.mTransforms = new Matrix[512];
			this.mInvTransforms = new Matrix[512];
			this.mArgs = new Vector3[512];
			this.mRenderData = new ShadowBlobs.RenderData[3];
			for (int i = 0; i < 3; i++)
			{
				this.mRenderData[i] = new ShadowBlobs.RenderData();
			}
			this.CreateVertexBuffer();
		}

		// Token: 0x06002CBF RID: 11455 RVA: 0x0015EF0E File Offset: 0x0015D10E
		public void Initialize(Scene iScene)
		{
			this.mScene = iScene;
		}

		// Token: 0x06002CC0 RID: 11456 RVA: 0x0015EF18 File Offset: 0x0015D118
		private void CreateVertexBuffer()
		{
			ShadowBlobs.Vertex[] array = new ShadowBlobs.Vertex[240];
			ushort[] array2 = new ushort[1080];
			for (int i = 0; i < 30; i++)
			{
				int num = i * 8;
				for (int j = 0; j < 8; j++)
				{
					array[num + j].Position.X = ((j % 2 < 1) ? -1f : 1f);
					array[num + j].Position.Y = ((j % 4 < 2) ? -1f : 1f);
					array[num + j].Position.Z = ((j % 8 < 4) ? -1f : 1f);
					array[num + j].Instance = (float)i;
				}
				int num2 = i * 36;
				array2[num2++] = (ushort)num;
				array2[num2++] = (ushort)(num + 2);
				array2[num2++] = (ushort)(num + 1);
				array2[num2++] = (ushort)(num + 1);
				array2[num2++] = (ushort)(num + 2);
				array2[num2++] = (ushort)(num + 3);
				array2[num2++] = (ushort)(num + 1);
				array2[num2++] = (ushort)num;
				array2[num2++] = (ushort)(num + 4);
				array2[num2++] = (ushort)(num + 4);
				array2[num2++] = (ushort)(num + 5);
				array2[num2++] = (ushort)(num + 1);
				array2[num2++] = (ushort)num;
				array2[num2++] = (ushort)(num + 2);
				array2[num2++] = (ushort)(num + 4);
				array2[num2++] = (ushort)(num + 4);
				array2[num2++] = (ushort)(num + 2);
				array2[num2++] = (ushort)(num + 6);
				array2[num2++] = (ushort)(num + 4);
				array2[num2++] = (ushort)(num + 6);
				array2[num2++] = (ushort)(num + 5);
				array2[num2++] = (ushort)(num + 5);
				array2[num2++] = (ushort)(num + 6);
				array2[num2++] = (ushort)(num + 7);
				array2[num2++] = (ushort)(num + 1);
				array2[num2++] = (ushort)(num + 5);
				array2[num2++] = (ushort)(num + 3);
				array2[num2++] = (ushort)(num + 3);
				array2[num2++] = (ushort)(num + 5);
				array2[num2++] = (ushort)(num + 7);
				array2[num2++] = (ushort)(num + 2);
				array2[num2++] = (ushort)(num + 3);
				array2[num2++] = (ushort)(num + 6);
				array2[num2++] = (ushort)(num + 6);
				array2[num2++] = (ushort)(num + 3);
				array2[num2++] = (ushort)(num + 7);
			}
			lock (Game.Instance.GraphicsDevice)
			{
				this.mVertices = new VertexBuffer(Game.Instance.GraphicsDevice, 3840, BufferUsage.WriteOnly);
				this.mVertices.SetData<ShadowBlobs.Vertex>(array);
				this.mIndices = new IndexBuffer(Game.Instance.GraphicsDevice, 2160, BufferUsage.WriteOnly, IndexElementSize.SixteenBits);
				this.mIndices.SetData<ushort>(array2);
				this.mVertexDeclaration = new VertexDeclaration(Game.Instance.GraphicsDevice, ShadowBlobs.Vertex.VertexElements);
			}
			for (int k = 0; k < 3; k++)
			{
				ShadowBlobs.RenderData renderData = this.mRenderData[k];
				renderData.mVertices = this.mVertices;
				renderData.mVerticesHash = this.mVertices.GetHashCode();
				renderData.mIndices = this.mIndices;
				renderData.mVertexDeclaration = this.mVertexDeclaration;
			}
		}

		// Token: 0x06002CC1 RID: 11457 RVA: 0x0015F2AC File Offset: 0x0015D4AC
		public void AddShadowBlob(ref Vector3 iPosition, float iRadius, float iDeadTime)
		{
			Vector3 up = Vector3.Up;
			up.Y *= 4f;
			Vector3 unitZ = Vector3.UnitZ;
			unitZ.Z = -unitZ.Z;
			Vector3 unitX = Vector3.UnitX;
			Matrix matrix = default(Matrix);
			matrix.Backward = up;
			matrix.Right = unitX;
			matrix.Up = unitZ;
			matrix.M11 *= iRadius;
			matrix.M12 *= iRadius;
			matrix.M13 *= iRadius;
			matrix.M21 *= iRadius;
			matrix.M22 *= iRadius;
			matrix.M23 *= iRadius;
			matrix.M44 = 1f;
			matrix.Translation = iPosition;
			this.mTransforms[this.mNrOfBlobs] = matrix;
			this.mArgs[this.mNrOfBlobs] = new Vector3(iDeadTime, 0f, 0f);
			Matrix.Invert(ref matrix, out this.mInvTransforms[this.mNrOfBlobs]);
			this.mNrOfBlobs++;
		}

		// Token: 0x06002CC2 RID: 11458 RVA: 0x0015F3E4 File Offset: 0x0015D5E4
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			if (this.mNrOfBlobs > 0)
			{
				ShadowBlobs.RenderData renderData = this.mRenderData[(int)iDataChannel];
				Array.Copy(this.mTransforms, 0, renderData.mTransforms, 0, this.mNrOfBlobs);
				Array.Copy(this.mInvTransforms, 0, renderData.mInvTransforms, 0, this.mNrOfBlobs);
				Array.Copy(this.mArgs, 0, renderData.mArgs, 0, this.mNrOfBlobs);
				renderData.mNrOfBlobs = this.mNrOfBlobs;
				this.mScene.AddProjection(iDataChannel, renderData);
			}
			this.mNrOfBlobs = 0;
		}

		// Token: 0x04003063 RID: 12387
		private static ShadowBlobs mSingelton;

		// Token: 0x04003064 RID: 12388
		private static volatile object mSingeltonLock = new object();

		// Token: 0x04003065 RID: 12389
		private Scene mScene;

		// Token: 0x04003066 RID: 12390
		private Matrix[] mTransforms;

		// Token: 0x04003067 RID: 12391
		private Matrix[] mInvTransforms;

		// Token: 0x04003068 RID: 12392
		private Vector3[] mArgs;

		// Token: 0x04003069 RID: 12393
		private int mNrOfBlobs;

		// Token: 0x0400306A RID: 12394
		private VertexBuffer mVertices;

		// Token: 0x0400306B RID: 12395
		private IndexBuffer mIndices;

		// Token: 0x0400306C RID: 12396
		private VertexDeclaration mVertexDeclaration;

		// Token: 0x0400306D RID: 12397
		private ShadowBlobs.RenderData[] mRenderData;

		// Token: 0x020005D9 RID: 1497
		private struct Vertex
		{
			// Token: 0x0400306E RID: 12398
			public const int SIZEINBYTES = 16;

			// Token: 0x0400306F RID: 12399
			public Vector3 Position;

			// Token: 0x04003070 RID: 12400
			public float Instance;

			// Token: 0x04003071 RID: 12401
			public static readonly VertexElement[] VertexElements = new VertexElement[]
			{
				new VertexElement(0, 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, 0),
				new VertexElement(0, 12, VertexElementFormat.Single, VertexElementMethod.Default, VertexElementUsage.BlendIndices, 0)
			};
		}

		// Token: 0x020005DA RID: 1498
		protected class RenderData : IProjectionObject
		{
			// Token: 0x17000A98 RID: 2712
			// (get) Token: 0x06002CC5 RID: 11461 RVA: 0x0015F4C5 File Offset: 0x0015D6C5
			public int Effect
			{
				get
				{
					return HardwareInstancedProjectionEffect.TYPEHASH;
				}
			}

			// Token: 0x17000A99 RID: 2713
			// (get) Token: 0x06002CC6 RID: 11462 RVA: 0x0015F4CC File Offset: 0x0015D6CC
			public int Technique
			{
				get
				{
					return 1;
				}
			}

			// Token: 0x17000A9A RID: 2714
			// (get) Token: 0x06002CC7 RID: 11463 RVA: 0x0015F4CF File Offset: 0x0015D6CF
			public VertexBuffer Vertices
			{
				get
				{
					return this.mVertices;
				}
			}

			// Token: 0x17000A9B RID: 2715
			// (get) Token: 0x06002CC8 RID: 11464 RVA: 0x0015F4D7 File Offset: 0x0015D6D7
			public int VerticesHashCode
			{
				get
				{
					return this.mVerticesHash;
				}
			}

			// Token: 0x17000A9C RID: 2716
			// (get) Token: 0x06002CC9 RID: 11465 RVA: 0x0015F4DF File Offset: 0x0015D6DF
			public int VertexStride
			{
				get
				{
					return 16;
				}
			}

			// Token: 0x17000A9D RID: 2717
			// (get) Token: 0x06002CCA RID: 11466 RVA: 0x0015F4E3 File Offset: 0x0015D6E3
			public IndexBuffer Indices
			{
				get
				{
					return this.mIndices;
				}
			}

			// Token: 0x17000A9E RID: 2718
			// (get) Token: 0x06002CCB RID: 11467 RVA: 0x0015F4EB File Offset: 0x0015D6EB
			public VertexDeclaration VertexDeclaration
			{
				get
				{
					return this.mVertexDeclaration;
				}
			}

			// Token: 0x06002CCC RID: 11468 RVA: 0x0015F4F3 File Offset: 0x0015D6F3
			public bool Cull(BoundingFrustum iViewFrustum)
			{
				return false;
			}

			// Token: 0x06002CCD RID: 11469 RVA: 0x0015F4F8 File Offset: 0x0015D6F8
			public void Draw(Effect iEffect, Texture2D iDepthMap)
			{
				HardwareInstancedProjectionEffect hardwareInstancedProjectionEffect = iEffect as HardwareInstancedProjectionEffect;
				hardwareInstancedProjectionEffect.ShadowIntensity = 0.55f;
				hardwareInstancedProjectionEffect.DepthMap = iDepthMap;
				hardwareInstancedProjectionEffect.GraphicsDevice.RenderState.DepthBufferWriteEnable = false;
				hardwareInstancedProjectionEffect.GraphicsDevice.RenderState.ReferenceStencil = 1;
				hardwareInstancedProjectionEffect.GraphicsDevice.RenderState.StencilFunction = CompareFunction.Equal;
				hardwareInstancedProjectionEffect.PixelSize = new Vector2(1f / (float)iDepthMap.Width, 1f / (float)iDepthMap.Height);
				hardwareInstancedProjectionEffect.Texture = null;
				hardwareInstancedProjectionEffect.GraphicsDevice.RenderState.ColorWriteChannels1 = ColorWriteChannels.None;
				hardwareInstancedProjectionEffect.GraphicsDevice.RenderState.ColorWriteChannels2 = ColorWriteChannels.None;
				hardwareInstancedProjectionEffect.GraphicsDevice.RenderState.SourceBlend = Blend.Zero;
				for (int i = 0; i < this.mNrOfBlobs; i += 30)
				{
					int num = Math.Min(Math.Min(this.mTransforms.Length - i, 30), this.mNrOfBlobs - i);
					Array.Copy(this.mTransforms, i, this.mBatchTransforms, 0, num);
					Array.Copy(this.mInvTransforms, i, this.mBatchInvTransforms, 0, num);
					Array.Copy(this.mArgs, i, this.mBatchArgs, 0, num);
					hardwareInstancedProjectionEffect.WorldTransforms = this.mBatchTransforms;
					hardwareInstancedProjectionEffect.InvWorldTransforms = this.mBatchInvTransforms;
					hardwareInstancedProjectionEffect.Args = this.mBatchArgs;
					hardwareInstancedProjectionEffect.CommitChanges();
					hardwareInstancedProjectionEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, num * 8, 0, num * 12);
				}
				hardwareInstancedProjectionEffect.GraphicsDevice.RenderState.DepthBufferWriteEnable = true;
				hardwareInstancedProjectionEffect.GraphicsDevice.RenderState.ColorWriteChannels1 = ColorWriteChannels.All;
				hardwareInstancedProjectionEffect.GraphicsDevice.RenderState.ColorWriteChannels2 = ColorWriteChannels.All;
				hardwareInstancedProjectionEffect.GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha;
			}

			// Token: 0x04003072 RID: 12402
			public VertexBuffer mVertices;

			// Token: 0x04003073 RID: 12403
			public IndexBuffer mIndices;

			// Token: 0x04003074 RID: 12404
			public VertexDeclaration mVertexDeclaration;

			// Token: 0x04003075 RID: 12405
			public int mNrOfBlobs;

			// Token: 0x04003076 RID: 12406
			public int mVerticesHash;

			// Token: 0x04003077 RID: 12407
			public Matrix[] mTransforms = new Matrix[512];

			// Token: 0x04003078 RID: 12408
			public Matrix[] mInvTransforms = new Matrix[512];

			// Token: 0x04003079 RID: 12409
			public Vector3[] mArgs = new Vector3[512];

			// Token: 0x0400307A RID: 12410
			private Matrix[] mBatchTransforms = new Matrix[30];

			// Token: 0x0400307B RID: 12411
			private Matrix[] mBatchInvTransforms = new Matrix[30];

			// Token: 0x0400307C RID: 12412
			private Vector3[] mBatchArgs = new Vector3[30];
		}
	}
}
