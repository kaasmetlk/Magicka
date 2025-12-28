using System;
using System.Collections.Generic;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Math;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.GameStates;
using Magicka.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;

namespace Magicka.Levels
{
	// Token: 0x020003E3 RID: 995
	public class Water : Liquid
	{
		// Token: 0x06001E7E RID: 7806 RVA: 0x000D52D4 File Offset: 0x000D34D4
		public Water(RenderDeferredLiquidEffect iEffect, ContentReader iInput, LevelModel iLevel, AnimatedLevelPart iParent) : base(iParent)
		{
			this.mMaterial.FetchFromEffect(iEffect);
			this.mWaterVertices = iInput.ReadObject<VertexBuffer>();
			this.mWaterIndices = iInput.ReadObject<IndexBuffer>();
			VertexDeclaration vertexDeclaration = iInput.ReadObject<VertexDeclaration>();
			VertexElement[] vertexElements = vertexDeclaration.GetVertexElements();
			VertexElement[] array = new VertexElement[vertexElements.Length + 1];
			vertexElements.CopyTo(array, 0);
			array[array.Length - 1] = new VertexElement(1, 0, VertexElementFormat.Single, VertexElementMethod.Default, VertexElementUsage.Color, 1);
			GraphicsDevice graphicsDevice = (iInput.ContentManager.ServiceProvider.GetService(typeof(IGraphicsDeviceManager)) as GraphicsDeviceManager).GraphicsDevice;
			lock (graphicsDevice)
			{
				this.mVertexDeclaration = new VertexDeclaration(graphicsDevice, array);
			}
			this.mVertexStride = iInput.ReadInt32();
			this.mNumVertices = iInput.ReadInt32();
			this.mPrimitiveCount = iInput.ReadInt32();
			int offsetInBytes = 0;
			int offsetInBytes2 = 0;
			for (int i = 0; i < vertexElements.Length; i++)
			{
				if (vertexElements[i].VertexElementUsage == VertexElementUsage.Position)
				{
					offsetInBytes = (int)vertexElements[i].Offset;
				}
				else if (vertexElements[i].VertexElementUsage == VertexElementUsage.Color)
				{
					offsetInBytes2 = (int)vertexElements[i].Offset;
				}
			}
			Vector3[] array2 = new Vector3[this.mNumVertices];
			this.mWaterVertices.GetData<Vector3>(offsetInBytes, array2, 0, this.mNumVertices, this.mVertexStride);
			float[] data = new float[this.mNumVertices];
			this.mWaterVertices.GetData<float>(offsetInBytes2, data, 0, this.mNumVertices, this.mVertexStride);
			List<TriangleVertexIndices> list = new List<TriangleVertexIndices>();
			if (this.mWaterIndices.IndexElementSize == IndexElementSize.SixteenBits)
			{
				short[] array3 = new short[this.mPrimitiveCount * 3];
				this.mWaterIndices.GetData<short>(array3);
				int j = 0;
				while (j < array3.Length)
				{
					TriangleVertexIndices item;
					item.I0 = (int)array3[j++];
					item.I2 = (int)array3[j++];
					item.I1 = (int)array3[j++];
					list.Add(item);
				}
			}
			else
			{
				int[] array4 = new int[this.mPrimitiveCount * 3];
				this.mWaterIndices.GetData<int>(array4);
				int k = 0;
				while (k < array4.Length)
				{
					TriangleVertexIndices item2;
					item2.I0 = array4[k++];
					item2.I2 = array4[k++];
					item2.I1 = array4[k++];
					list.Add(item2);
				}
			}
			this.mWaterFreezeVertices = new float[this.mNumVertices];
			lock (graphicsDevice)
			{
				this.mWaterFreezeVertexBuffer = new DynamicVertexBuffer((iInput.ContentManager.ServiceProvider.GetService(typeof(IGraphicsDeviceManager)) as GraphicsDeviceManager).GraphicsDevice, 4 * this.mNumVertices, BufferUsage.WriteOnly);
			}
			Transform identity = Transform.Identity;
			bool flag = iInput.ReadBoolean();
			if (flag)
			{
				this.mIceCollisionMesh = new IceMesh();
				this.mIceCollisionMesh.CreateMesh(array2, this.mWaterFreezeVertices, list.ToArray(), 10, 1f);
				this.mIceCollisionMesh.SetTransform(ref identity);
				this.mWaterCollisionMesh = new WaterMesh();
				this.mWaterCollisionMesh.CreateMesh(array2, this.mWaterFreezeVertices, list.ToArray(), 10, 1f);
				this.mWaterCollisionMesh.SetTransform(ref identity);
				this.mCollisionSkin = new CollisionSkin(null);
				this.mCollisionSkin.callbackFn += this.OnCollision;
				this.mCollisionSkin.AddPrimitive(this.mIceCollisionMesh, 1, new MaterialProperties(0f, 10f, 10f));
				this.mCollisionSkin.AddPrimitive(this.mWaterCollisionMesh, 1, new MaterialProperties(0f, 10f, 10f));
				this.mCollisionSkin.ApplyLocalTransform(identity);
				this.mCollisionSkin.Tag = this;
			}
			this.mRenderData = new Water.RenderData[3];
			int mTechnique = (this.mMaterial.ReflectionMap != null) ? 1 : 0;
			for (int l = 0; l < 3; l++)
			{
				Water.RenderData renderData = new Water.RenderData(this.mNumVertices);
				this.mRenderData[l] = renderData;
				renderData.mPrimitiveCount = this.mPrimitiveCount;
				renderData.mNumVertices = this.mNumVertices;
				renderData.mVertexStride = this.mVertexStride;
				renderData.mVertices = this.mWaterVertices;
				renderData.mVerticesHash = this.mWaterVertices.GetHashCode();
				renderData.mFrozenVertexBuffer = this.mWaterFreezeVertexBuffer;
				renderData.mIndices = this.mWaterIndices;
				renderData.mVertexDeclaration = this.mVertexDeclaration;
				renderData.mTechnique = mTechnique;
				renderData.mMaterial = this.mMaterial;
			}
			this.mFreezable = iInput.ReadBoolean();
			this.mAutoFreeze = iInput.ReadBoolean();
			if (this.mAutoFreeze)
			{
				for (int m = 0; m < this.mWaterFreezeVertices.Length; m++)
				{
					this.mWaterFreezeVertices[m] = 1f;
				}
			}
		}

		// Token: 0x06001E7F RID: 7807 RVA: 0x000D57DC File Offset: 0x000D39DC
		public override void Initialize()
		{
			if (this.mCollisionSkin != null && !PhysicsManager.Instance.Simulator.CollisionSystem.CollisionSkins.Contains(this.mCollisionSkin))
			{
				PhysicsManager.Instance.Simulator.CollisionSystem.AddCollisionSkin(this.mCollisionSkin);
			}
		}

		// Token: 0x06001E80 RID: 7808 RVA: 0x000D582C File Offset: 0x000D3A2C
		private bool OnCollision(CollisionSkin skin0, int prim0, CollisionSkin skin1, int prim1)
		{
			return prim0 == 0 || (skin1 != null && skin1.Owner != null && skin1.Owner.Tag is Magicka.GameLogic.Entities.Character);
		}

		// Token: 0x17000770 RID: 1904
		// (get) Token: 0x06001E81 RID: 7809 RVA: 0x000D5853 File Offset: 0x000D3A53
		public override CollisionSkin CollisionSkin
		{
			get
			{
				return this.mCollisionSkin;
			}
		}

		// Token: 0x17000771 RID: 1905
		// (get) Token: 0x06001E82 RID: 7810 RVA: 0x000D585B File Offset: 0x000D3A5B
		internal override bool AutoFreeze
		{
			get
			{
				return this.mAutoFreeze;
			}
		}

		// Token: 0x06001E83 RID: 7811 RVA: 0x000D5864 File Offset: 0x000D3A64
		public override void Update(DataChannel iDataChannel, float iDeltaTime, Scene scene, ref Matrix iTransform, ref Matrix iInvTransform)
		{
			this.mTransform = iTransform;
			this.mInvTransform = iInvTransform;
			if (this.mCollisionSkin != null)
			{
				Transform newTransform = this.mCollisionSkin.NewTransform;
				Transform transform;
				transform.Position = iTransform.Translation;
				transform.Orientation = iTransform;
				transform.Orientation.Translation = default(Vector3);
				this.mCollisionSkin.SetTransform(ref newTransform, ref transform);
			}
			if (iDataChannel == DataChannel.None)
			{
				return;
			}
			Water.RenderData renderData = this.mRenderData[(int)iDataChannel];
			float num = iDeltaTime * 0.1f;
			for (int i = 0; i < this.mWaterFreezeVertices.Length; i++)
			{
				if (this.mAutoFreeze)
				{
					this.mWaterFreezeVertices[i] = MathHelper.Clamp(this.mWaterFreezeVertices[i] + num, -1f, 1f);
				}
				else
				{
					this.mWaterFreezeVertices[i] = MathHelper.Clamp(this.mWaterFreezeVertices[i] - num, 0f, 2f);
				}
			}
			this.mTime += iDeltaTime;
			renderData.mMaterial.WorldTransform = iTransform;
			renderData.mTime = this.mTime;
			renderData.SetFrozenVertices(this.mWaterFreezeVertices);
			scene.AddRenderableObject(iDataChannel, renderData);
		}

		// Token: 0x06001E84 RID: 7812 RVA: 0x000D59A0 File Offset: 0x000D3BA0
		public unsafe override bool SegmentIntersect(out float frac, out Vector3 pos, out Vector3 normal, ref Segment seg, bool ignoreBackfaces, bool ignoreWater, bool ignoreIce)
		{
			if (this.mCollisionSkin == null || (ignoreIce && ignoreWater))
			{
				frac = 0f;
				pos = default(Vector3);
				normal = default(Vector3);
				return false;
			}
			Segment seg2;
			Vector3.Transform(ref seg.Origin, ref this.mInvTransform, out seg2.Origin);
			Vector3.TransformNormal(ref seg.Delta, ref this.mInvTransform, out seg2.Delta);
			BoundingBox initialBox = BoundingBoxHelper.InitialBox;
			BoundingBoxHelper.AddSegment(seg2, ref initialBox);
			int[] array = DetectFunctor.IntStackAlloc();
			fixed (int* ptr = array)
			{
				int allTrianglesIntersectingtAABox = this.mIceCollisionMesh.GetAllTrianglesIntersectingtAABox(ptr, 2048, ref initialBox);
				pos = Vector3.Zero;
				normal = Vector3.Zero;
				float num = float.MaxValue;
				bool flag = false;
				IndexedTriangle indexedTriangle = default(IndexedTriangle);
				int i = 0;
				while (i < allTrianglesIntersectingtAABox)
				{
					int iTriangle = ptr[i];
					IndexedTriangle triangle = this.mIceCollisionMesh.GetTriangle(iTriangle);
					int num2;
					int num3;
					int num4;
					triangle.GetVertexIndices(out num2, out num3, out num4);
					if (ignoreWater)
					{
						if (this.mWaterFreezeVertices[num2] >= 0.5f || this.mWaterFreezeVertices[num3] >= 0.5f || this.mWaterFreezeVertices[num4] >= 0.5f)
						{
							goto IL_167;
						}
					}
					else if (!ignoreIce || (this.mWaterFreezeVertices[num2] < 0.5f && this.mWaterFreezeVertices[num3] < 0.5f && this.mWaterFreezeVertices[num4] < 0.5f))
					{
						goto IL_167;
					}
					IL_1DF:
					i++;
					continue;
					IL_167:
					Vector3 vector;
					this.mIceCollisionMesh.GetVertex(num2, out vector);
					Vector3 vector2;
					this.mIceCollisionMesh.GetVertex(num3, out vector2);
					Vector3 vector3;
					this.mIceCollisionMesh.GetVertex(num4, out vector3);
					Triangle triangle2 = new Triangle(ref vector, ref vector2, ref vector3);
					float num5;
					float num6;
					float num7;
					if (Intersection.SegmentTriangleIntersection(out num5, out num6, out num7, ref seg2, ref triangle2, ignoreBackfaces) && num5 < num)
					{
						flag = true;
						num = num5;
						indexedTriangle = triangle;
						Vector3.Multiply(ref seg2.Delta, num5, out seg2.Delta);
						goto IL_1DF;
					}
					goto IL_1DF;
				}
				frac = num;
				if (flag)
				{
					seg2.GetEnd(out pos);
					normal = indexedTriangle.Plane.Normal;
				}
				DetectFunctor.FreeStackAlloc(array);
				return flag;
			}
		}

		// Token: 0x06001E85 RID: 7813 RVA: 0x000D5BCC File Offset: 0x000D3DCC
		protected unsafe override void Freeze(ref Vector3 iOrigin, ref Vector3 iDirection, float iSpread, float iMagnitude)
		{
			if (!this.mFreezable)
			{
				return;
			}
			Vector3 origin;
			Vector3.Transform(ref iOrigin, ref this.mInvTransform, out origin);
			Vector3 vector;
			Vector3.TransformNormal(ref iDirection, ref this.mInvTransform, out vector);
			int[] array = DetectFunctor.IntStackAlloc();
			fixed (int* ptr = array)
			{
				int num = 0;
				int num2 = (int)(iSpread / 0.7853982f);
				float num3 = iSpread / (float)(num2 * 2 + 1);
				Segment seg;
				seg.Origin = origin;
				for (int i = -num2; i <= num2; i++)
				{
					float yaw = (float)i * num3;
					Quaternion quaternion;
					Quaternion.CreateFromYawPitchRoll(yaw, 0f, 0f, out quaternion);
					Vector3.Transform(ref vector, ref quaternion, out seg.Delta);
					PlayState playState = GameStateManager.Instance.CurrentState as PlayState;
					if (playState != null)
					{
						List<Shield> shields = playState.EntityManager.Shields;
						for (int j = 0; j < shields.Count; j++)
						{
							float scaleFactor;
							Vector3 vector2;
							Vector3 vector3;
							if (shields[j].Body.CollisionSkin.SegmentIntersect(out scaleFactor, out vector2, out vector3, seg))
							{
								Vector3.Multiply(ref seg.Delta, scaleFactor, out seg.Delta);
							}
						}
					}
					int num4 = num;
					num += this.mIceCollisionMesh.GetVerticesIntersectingArc(ptr + num, 2048 - num, ref origin, ref seg.Delta, num3);
					for (int k = 0; k < num4; k++)
					{
						for (int l = num4 + 1; l < num; l++)
						{
							if (ptr[k] == ptr[l])
							{
								ptr[l] = -1;
							}
						}
					}
				}
				for (int m = 0; m < num; m++)
				{
					if (ptr[m] >= 0)
					{
						this.mWaterFreezeVertices[ptr[m]] += iMagnitude;
					}
				}
			}
			DetectFunctor.FreeStackAlloc(array);
		}

		// Token: 0x06001E86 RID: 7814 RVA: 0x000D5DAC File Offset: 0x000D3FAC
		public override void FreezeAll(float iMagnitude)
		{
			if (!this.mFreezable)
			{
				return;
			}
			for (int i = 0; i < this.mWaterFreezeVertices.Length; i++)
			{
				this.mWaterFreezeVertices[i] += iMagnitude;
			}
		}

		// Token: 0x040020CF RID: 8399
		private CollisionSkin mCollisionSkin;

		// Token: 0x040020D0 RID: 8400
		private IceMesh mIceCollisionMesh;

		// Token: 0x040020D1 RID: 8401
		private WaterMesh mWaterCollisionMesh;

		// Token: 0x040020D2 RID: 8402
		private RenderDeferredLiquidMaterial mMaterial;

		// Token: 0x040020D3 RID: 8403
		private VertexBuffer mWaterVertices;

		// Token: 0x040020D4 RID: 8404
		private DynamicVertexBuffer mWaterFreezeVertexBuffer;

		// Token: 0x040020D5 RID: 8405
		private float[] mWaterFreezeVertices;

		// Token: 0x040020D6 RID: 8406
		private IndexBuffer mWaterIndices;

		// Token: 0x040020D7 RID: 8407
		private VertexDeclaration mVertexDeclaration;

		// Token: 0x040020D8 RID: 8408
		private int mVertexStride;

		// Token: 0x040020D9 RID: 8409
		private int mNumVertices;

		// Token: 0x040020DA RID: 8410
		private int mPrimitiveCount;

		// Token: 0x040020DB RID: 8411
		private float mTime;

		// Token: 0x040020DC RID: 8412
		private Matrix mTransform;

		// Token: 0x040020DD RID: 8413
		private Matrix mInvTransform;

		// Token: 0x040020DE RID: 8414
		private bool mFreezable;

		// Token: 0x040020DF RID: 8415
		private bool mAutoFreeze;

		// Token: 0x040020E0 RID: 8416
		private Water.RenderData[] mRenderData;

		// Token: 0x020003E4 RID: 996
		protected class RenderData : IRenderableObject
		{
			// Token: 0x06001E87 RID: 7815 RVA: 0x000D5DEE File Offset: 0x000D3FEE
			public RenderData(int iVertexCount)
			{
				this.mFrozenVertices = new float[iVertexCount];
			}

			// Token: 0x06001E88 RID: 7816 RVA: 0x000D5E02 File Offset: 0x000D4002
			public void SetFrozenVertices(float[] iFrozenVertices)
			{
				iFrozenVertices.CopyTo(this.mFrozenVertices, 0);
			}

			// Token: 0x17000772 RID: 1906
			// (get) Token: 0x06001E89 RID: 7817 RVA: 0x000D5E11 File Offset: 0x000D4011
			public int Effect
			{
				get
				{
					return RenderDeferredLiquidEffect.TYPEHASH;
				}
			}

			// Token: 0x17000773 RID: 1907
			// (get) Token: 0x06001E8A RID: 7818 RVA: 0x000D5E18 File Offset: 0x000D4018
			public int DepthTechnique
			{
				get
				{
					return 2;
				}
			}

			// Token: 0x17000774 RID: 1908
			// (get) Token: 0x06001E8B RID: 7819 RVA: 0x000D5E1B File Offset: 0x000D401B
			public int Technique
			{
				get
				{
					return this.mTechnique;
				}
			}

			// Token: 0x17000775 RID: 1909
			// (get) Token: 0x06001E8C RID: 7820 RVA: 0x000D5E23 File Offset: 0x000D4023
			public int ShadowTechnique
			{
				get
				{
					return 3;
				}
			}

			// Token: 0x17000776 RID: 1910
			// (get) Token: 0x06001E8D RID: 7821 RVA: 0x000D5E26 File Offset: 0x000D4026
			public VertexBuffer Vertices
			{
				get
				{
					return this.mVertices;
				}
			}

			// Token: 0x17000777 RID: 1911
			// (get) Token: 0x06001E8E RID: 7822 RVA: 0x000D5E2E File Offset: 0x000D402E
			public int VertexStride
			{
				get
				{
					return this.mVertexStride;
				}
			}

			// Token: 0x17000778 RID: 1912
			// (get) Token: 0x06001E8F RID: 7823 RVA: 0x000D5E36 File Offset: 0x000D4036
			public IndexBuffer Indices
			{
				get
				{
					return this.mIndices;
				}
			}

			// Token: 0x17000779 RID: 1913
			// (get) Token: 0x06001E90 RID: 7824 RVA: 0x000D5E3E File Offset: 0x000D403E
			public VertexDeclaration VertexDeclaration
			{
				get
				{
					return this.mVertexDeclaration;
				}
			}

			// Token: 0x06001E91 RID: 7825 RVA: 0x000D5E46 File Offset: 0x000D4046
			public bool Cull(BoundingFrustum iViewFrustum)
			{
				return false;
			}

			// Token: 0x1700077A RID: 1914
			// (get) Token: 0x06001E92 RID: 7826 RVA: 0x000D5E49 File Offset: 0x000D4049
			public int VerticesHashCode
			{
				get
				{
					return this.mVerticesHash;
				}
			}

			// Token: 0x06001E93 RID: 7827 RVA: 0x000D5E54 File Offset: 0x000D4054
			public void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				RenderDeferredLiquidEffect renderDeferredLiquidEffect = iEffect as RenderDeferredLiquidEffect;
				this.mFrozenVertexBuffer.SetData<float>(this.mFrozenVertices);
				renderDeferredLiquidEffect.GraphicsDevice.Vertices[1].SetSource(this.mFrozenVertexBuffer, 0, 4);
				renderDeferredLiquidEffect.GraphicsDevice.RenderState.ReferenceStencil = 3;
				this.mMaterial.AssignToEffect(renderDeferredLiquidEffect);
				renderDeferredLiquidEffect.Time = this.mTime;
				renderDeferredLiquidEffect.CommitChanges();
				renderDeferredLiquidEffect.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, 0, 0, this.mNumVertices, 0, this.mPrimitiveCount);
				renderDeferredLiquidEffect.GraphicsDevice.Vertices[1].SetSource(null, 0, 0);
			}

			// Token: 0x06001E94 RID: 7828 RVA: 0x000D5EFC File Offset: 0x000D40FC
			public void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				RenderDeferredLiquidEffect renderDeferredLiquidEffect = iEffect as RenderDeferredLiquidEffect;
				this.mMaterial.AssignOpacityToEffect(renderDeferredLiquidEffect);
				renderDeferredLiquidEffect.CommitChanges();
				renderDeferredLiquidEffect.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, 0, 0, this.mNumVertices, 0, this.mPrimitiveCount);
			}

			// Token: 0x040020E1 RID: 8417
			public float mTime;

			// Token: 0x040020E2 RID: 8418
			public int mTechnique;

			// Token: 0x040020E3 RID: 8419
			public RenderDeferredLiquidMaterial mMaterial;

			// Token: 0x040020E4 RID: 8420
			public VertexBuffer mVertices;

			// Token: 0x040020E5 RID: 8421
			public VertexBuffer mFrozenVertexBuffer;

			// Token: 0x040020E6 RID: 8422
			public int mVerticesHash;

			// Token: 0x040020E7 RID: 8423
			public IndexBuffer mIndices;

			// Token: 0x040020E8 RID: 8424
			public VertexDeclaration mVertexDeclaration;

			// Token: 0x040020E9 RID: 8425
			public int mVertexStride;

			// Token: 0x040020EA RID: 8426
			public int mNumVertices;

			// Token: 0x040020EB RID: 8427
			public int mPrimitiveCount;

			// Token: 0x040020EC RID: 8428
			private float[] mFrozenVertices;
		}
	}
}
