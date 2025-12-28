using System;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Math;
using Magicka.GameLogic.GameStates;
using Magicka.Graphics.Effects;
using Magicka.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;

namespace Magicka.Levels
{
	// Token: 0x02000396 RID: 918
	internal class ForceField
	{
		// Token: 0x06001C2B RID: 7211 RVA: 0x000BFDA8 File Offset: 0x000BDFA8
		internal ForceField(ContentReader iInput, LevelModel iLevel)
		{
			this.mMaterial.Color = iInput.ReadVector3();
			this.mMaterial.Width = iInput.ReadSingle();
			this.mMaterial.AlphaPower = iInput.ReadSingle();
			this.mMaterial.AlphaFalloffPower = iInput.ReadSingle();
			this.mMaterial.MaxRadius = iInput.ReadSingle();
			this.mMaterial.RippleDistortion = iInput.ReadSingle();
			this.mMaterial.MapDistortion = iInput.ReadSingle();
			this.mMaterial.VertexColorEnabled = iInput.ReadBoolean();
			this.mMaterial.DisplacementMap = iInput.ReadExternalReference<Texture2D>();
			this.mTTL = iInput.ReadSingle();
			this.mVertices = iInput.ReadObject<VertexBuffer>();
			this.mIndices = iInput.ReadObject<IndexBuffer>();
			this.mDeclaration = iInput.ReadObject<VertexDeclaration>();
			this.mVertexStride = iInput.ReadInt32();
			this.mNumVertices = iInput.ReadInt32();
			this.mPrimitiveCount = iInput.ReadInt32();
			VertexElement[] vertexElements = this.mDeclaration.GetVertexElements();
			int num = -1;
			int i = 0;
			while (i < vertexElements.Length)
			{
				if (vertexElements[i].VertexElementUsage == VertexElementUsage.Position && vertexElements[i].UsageIndex == 0)
				{
					if (vertexElements[i].VertexElementFormat != VertexElementFormat.Vector3)
					{
						throw new Exception("Unsupported vertex format: \"" + vertexElements[i].VertexElementFormat + "\"");
					}
					num = (int)vertexElements[i].Offset;
					break;
				}
				else
				{
					i++;
				}
			}
			if (num < 0)
			{
				throw new Exception("Vertices contain no position?!");
			}
			Vector3[] array = new Vector3[this.mNumVertices];
			this.mVertices.GetData<Vector3>(num, array, 0, this.mNumVertices, this.mVertexStride);
			TriangleVertexIndices[] array2 = new TriangleVertexIndices[this.mPrimitiveCount];
			if (this.mIndices.IndexElementSize == IndexElementSize.SixteenBits)
			{
				ushort[] array3 = new ushort[this.mIndices.SizeInBytes / 2];
				this.mIndices.GetData<ushort>(array3);
				int j = 0;
				while (j < array3.Length)
				{
					int num2 = j / 3;
					TriangleVertexIndices triangleVertexIndices;
					triangleVertexIndices.I0 = (int)array3[j++];
					triangleVertexIndices.I2 = (int)array3[j++];
					triangleVertexIndices.I1 = (int)array3[j++];
					array2[num2] = triangleVertexIndices;
				}
			}
			else if (this.mIndices.IndexElementSize == IndexElementSize.ThirtyTwoBits)
			{
				int[] array4 = new int[this.mIndices.SizeInBytes / 4];
				this.mIndices.GetData<int>(array4);
				int k = 0;
				while (k < array4.Length)
				{
					int num3 = k / 3;
					TriangleVertexIndices triangleVertexIndices2;
					triangleVertexIndices2.I0 = array4[k++];
					triangleVertexIndices2.I2 = array4[k++];
					triangleVertexIndices2.I1 = array4[k++];
					array2[num3] = triangleVertexIndices2;
				}
			}
			this.mCollision = new CollisionSkin();
			this.mCollision.Tag = iLevel;
			TriangleMesh triangleMesh = new TriangleMesh();
			triangleMesh.CreateMesh(array, array2, 8, 1f);
			this.mCollision.AddPrimitive(triangleMesh, 1, new MaterialProperties(1f, 1f, 1f));
			this.mCollision.ApplyLocalTransform(Transform.Identity);
			this.mCollision.postCollisionCallbackFn += this.mCollision_postCollisionCallbackFn;
			if (!(RenderManager.Instance.GetEffect(ForceFieldEffect.TYPEHASH) is ForceFieldEffect))
			{
				ForceFieldEffect iEffect = new ForceFieldEffect(Game.Instance.GraphicsDevice, Game.Instance.Content);
				RenderManager.Instance.RegisterEffect(iEffect);
			}
			this.mRenderData = new ForceField.RenderData[3];
			for (int l = 0; l < 3; l++)
			{
				this.mRenderData[l] = new ForceField.RenderData(this.mVertices, this.mIndices, this.mDeclaration, this.mVertexStride, this.mNumVertices, this.mPrimitiveCount);
				this.mRenderData[l].Material = this.mMaterial;
			}
		}

		// Token: 0x170006F5 RID: 1781
		// (get) Token: 0x06001C2C RID: 7212 RVA: 0x000C019D File Offset: 0x000BE39D
		internal CollisionSkin CollisionSkin
		{
			get
			{
				return this.mCollision;
			}
		}

		// Token: 0x06001C2D RID: 7213 RVA: 0x000C01A8 File Offset: 0x000BE3A8
		private void mCollision_postCollisionCallbackFn(ref CollisionInfo iInfo)
		{
			if (iInfo.NumCollPts > 0)
			{
				Vector3 vector = default(Vector3);
				Vector3 vector2 = default(Vector3);
				if (iInfo.SkinInfo.Skin0 == this.mCollision)
				{
					for (int i = 0; i < iInfo.NumCollPts; i++)
					{
						Vector3.Add(ref vector, ref iInfo.PointInfo[i].info.R0, out vector);
					}
				}
				else
				{
					for (int j = 0; j < iInfo.NumCollPts; j++)
					{
						Vector3.Add(ref vector, ref iInfo.PointInfo[j].info.R1, out vector);
					}
				}
				Vector3.Divide(ref vector, (float)iInfo.NumCollPts, out vector);
				int num = -1;
				for (int k = 0; k < this.mCollPoints.Length; k++)
				{
					if (num < 0 && this.mCollPoints[k].W >= this.mTTL)
					{
						num = k;
					}
					else if (this.mCollPoints[k].W < this.mTTL * 0.5f)
					{
						vector2.X = this.mCollPoints[k].X;
						vector2.Y = this.mCollPoints[k].Y;
						vector2.Z = this.mCollPoints[k].Z;
						float num2;
						Vector3.Distance(ref vector, ref vector2, out num2);
						if (num2 < this.mMaterial.MaxRadius * 0.5f)
						{
							return;
						}
					}
				}
				if (num >= 0)
				{
					Vector4 vector3 = default(Vector4);
					vector3.X = vector.X;
					vector3.Y = vector.Y;
					vector3.Z = vector.Z;
					this.mCollPoints[num] = vector3;
				}
			}
		}

		// Token: 0x06001C2E RID: 7214 RVA: 0x000C0378 File Offset: 0x000BE578
		public void Initialize(PlayState iPlayState)
		{
			if (!PhysicsManager.Instance.Simulator.CollisionSystem.CollisionSkins.Contains(this.mCollision))
			{
				PhysicsManager.Instance.Simulator.CollisionSystem.AddCollisionSkin(this.mCollision);
			}
			for (int i = 0; i < this.mCollPoints.Length; i++)
			{
				this.mCollPoints[i].W = float.MaxValue;
			}
			this.mPlayState = iPlayState;
		}

		// Token: 0x06001C2F RID: 7215 RVA: 0x000C03F0 File Offset: 0x000BE5F0
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			if (iDataChannel == DataChannel.None)
			{
				return;
			}
			float num = iDeltaTime / this.mTTL;
			for (int i = 0; i < this.mCollPoints.Length; i++)
			{
				Vector4[] array = this.mCollPoints;
				int num2 = i;
				array[num2].W = array[num2].W + num;
			}
			ForceField.RenderData renderData = this.mRenderData[(int)iDataChannel];
			this.mCollPoints.CopyTo(renderData.CollPoints, 0);
			this.mPlayState.Scene.AddPostEffect(iDataChannel, renderData);
		}

		// Token: 0x04001E57 RID: 7767
		private ForceField.RenderData[] mRenderData;

		// Token: 0x04001E58 RID: 7768
		private ForceFieldMaterial mMaterial;

		// Token: 0x04001E59 RID: 7769
		private float mTTL;

		// Token: 0x04001E5A RID: 7770
		private Vector4[] mCollPoints = new Vector4[32];

		// Token: 0x04001E5B RID: 7771
		private VertexBuffer mVertices;

		// Token: 0x04001E5C RID: 7772
		private IndexBuffer mIndices;

		// Token: 0x04001E5D RID: 7773
		private VertexDeclaration mDeclaration;

		// Token: 0x04001E5E RID: 7774
		private int mVertexStride;

		// Token: 0x04001E5F RID: 7775
		private int mNumVertices;

		// Token: 0x04001E60 RID: 7776
		private int mPrimitiveCount;

		// Token: 0x04001E61 RID: 7777
		private CollisionSkin mCollision;

		// Token: 0x04001E62 RID: 7778
		private PlayState mPlayState;

		// Token: 0x02000397 RID: 919
		private class RenderData : IPostEffect
		{
			// Token: 0x06001C30 RID: 7216 RVA: 0x000C0464 File Offset: 0x000BE664
			public RenderData(VertexBuffer iVertices, IndexBuffer iIndices, VertexDeclaration iDeclaration, int iVertexStride, int iNumVertices, int iPrimitiveCount)
			{
				this.mVertices = iVertices;
				this.mIndices = iIndices;
				this.mDeclaration = iDeclaration;
				this.mVertexStride = iVertexStride;
				this.mNumVertices = iNumVertices;
				this.mPrimitiveCount = iPrimitiveCount;
			}

			// Token: 0x170006F6 RID: 1782
			// (get) Token: 0x06001C31 RID: 7217 RVA: 0x000C04B1 File Offset: 0x000BE6B1
			public int ZIndex
			{
				get
				{
					return 0;
				}
			}

			// Token: 0x06001C32 RID: 7218 RVA: 0x000C04B4 File Offset: 0x000BE6B4
			public void Draw(float iDeltaTime, ref Vector2 iPixelSize, ref Matrix iViewMatrix, ref Matrix iProjectionMatrix, Texture2D iCandidate, Texture2D iDepthMap, Texture2D iNormalMap)
			{
				ForceFieldEffect forceFieldEffect = RenderManager.Instance.GetEffect(ForceFieldEffect.TYPEHASH) as ForceFieldEffect;
				this.Material.AssignToEffect(forceFieldEffect);
				forceFieldEffect.CollPoints = this.CollPoints;
				forceFieldEffect.View = iViewMatrix;
				forceFieldEffect.Projection = iProjectionMatrix;
				forceFieldEffect.World = Matrix.Identity;
				RenderState renderState = forceFieldEffect.GraphicsDevice.RenderState;
				renderState.CullMode = CullMode.None;
				renderState.AlphaBlendEnable = false;
				forceFieldEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertices, 0, this.mVertexStride);
				forceFieldEffect.GraphicsDevice.VertexDeclaration = this.mDeclaration;
				forceFieldEffect.GraphicsDevice.Indices = this.mIndices;
				forceFieldEffect.DestinationDimentions = new Point
				{
					X = iDepthMap.Width,
					Y = iDepthMap.Height
				};
				forceFieldEffect.Candidate = iCandidate;
				forceFieldEffect.DepthMap = iDepthMap;
				forceFieldEffect.Begin();
				forceFieldEffect.CurrentTechnique.Passes[0].Begin();
				forceFieldEffect.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, 0, 0, this.mNumVertices, 0, this.mPrimitiveCount);
				forceFieldEffect.CurrentTechnique.Passes[0].End();
				forceFieldEffect.End();
			}

			// Token: 0x04001E63 RID: 7779
			public Vector4[] CollPoints = new Vector4[32];

			// Token: 0x04001E64 RID: 7780
			public ForceFieldMaterial Material;

			// Token: 0x04001E65 RID: 7781
			private VertexBuffer mVertices;

			// Token: 0x04001E66 RID: 7782
			private IndexBuffer mIndices;

			// Token: 0x04001E67 RID: 7783
			private VertexDeclaration mDeclaration;

			// Token: 0x04001E68 RID: 7784
			private int mVertexStride;

			// Token: 0x04001E69 RID: 7785
			private int mNumVertices;

			// Token: 0x04001E6A RID: 7786
			private int mPrimitiveCount;
		}
	}
}
