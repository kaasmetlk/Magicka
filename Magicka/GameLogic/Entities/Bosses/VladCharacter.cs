using System;
using Magicka.GameLogic.GameStates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using XNAnimation.Effects;

namespace Magicka.GameLogic.Entities.Bosses
{
	// Token: 0x0200066C RID: 1644
	internal class VladCharacter : NonPlayerCharacter
	{
		// Token: 0x060031A9 RID: 12713 RVA: 0x001985E0 File Offset: 0x001967E0
		public VladCharacter(PlayState iPlayState) : base(iPlayState)
		{
			this.mAfterImageRenderData = new VladCharacter.AfterImageRenderData[3];
			Matrix[][] array = new Matrix[5][];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new Matrix[80];
			}
			for (int j = 0; j < 3; j++)
			{
				this.mAfterImageRenderData[j] = new VladCharacter.AfterImageRenderData(array);
			}
		}

		// Token: 0x060031AA RID: 12714 RVA: 0x0019863C File Offset: 0x0019683C
		public override void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			base.Update(iDataChannel, iDeltaTime);
			base.CharacterBody.SpeedMultiplier = base.CharacterBody.SpeedMultiplier * 1.5f;
			if (base.CharacterBody.SpeedMultiplier > 1f | this.mAfterImageIntensity > -1f)
			{
				VladCharacter.AfterImageRenderData afterImageRenderData = this.mAfterImageRenderData[(int)iDataChannel];
				if (afterImageRenderData.MeshDirty)
				{
					ModelMesh modelMesh = this.mModel.Model.Meshes[0];
					ModelMeshPart iMeshPart = modelMesh.MeshParts[0];
					afterImageRenderData.SetMesh(modelMesh.VertexBuffer, modelMesh.IndexBuffer, iMeshPart, SkinnedModelBasicEffect.TYPEHASH);
					afterImageRenderData.Color = this.mMaterial.TintColor;
				}
				int count = this.mModel.SkeletonBones.Count;
				this.mAfterImageTimer -= iDeltaTime;
				this.mAfterImageIntensity -= iDeltaTime;
				while (this.mAfterImageTimer <= 0f)
				{
					this.mAfterImageTimer += 0.05f;
					Vector3 velocity = this.mBody.Velocity;
					velocity.Y = 0f;
					if (velocity.LengthSquared() > 0.001f & base.CharacterBody.SpeedMultiplier > 1f)
					{
						while (this.mAfterImageIntensity <= 0f)
						{
							this.mAfterImageIntensity += 0.05f;
						}
					}
					for (int i = afterImageRenderData.mSkeleton.Length - 1; i > 0; i--)
					{
						Array.Copy(afterImageRenderData.mSkeleton[i - 1], afterImageRenderData.mSkeleton[i], count);
					}
					Array.Copy(this.mAnimationController.SkinnedBoneTransforms, afterImageRenderData.mSkeleton[0], count);
				}
				afterImageRenderData.mIntensity = this.mAfterImageIntensity / 0.05f;
				afterImageRenderData.mBoundingSphere = this.mRenderData[(int)iDataChannel].mBoundingSphere;
				this.mPlayState.Scene.AddRenderableAdditiveObject(iDataChannel, afterImageRenderData);
			}
		}

		// Token: 0x060031AB RID: 12715 RVA: 0x00198824 File Offset: 0x00196A24
		public override void OverKill()
		{
			this.mDead = false;
			this.mOverkilled = true;
			this.mCannotDieWithoutExplicitKill = true;
			this.mAnimationController.Stop();
			this.mHitPoints = -1f;
		}

		// Token: 0x04003611 RID: 13841
		private float mAfterImageTimer;

		// Token: 0x04003612 RID: 13842
		private float mAfterImageIntensity;

		// Token: 0x04003613 RID: 13843
		private VladCharacter.AfterImageRenderData[] mAfterImageRenderData;

		// Token: 0x0200066D RID: 1645
		protected class AfterImageRenderData : IRenderableAdditiveObject
		{
			// Token: 0x060031AC RID: 12716 RVA: 0x00198851 File Offset: 0x00196A51
			public AfterImageRenderData(Matrix[][] iSkeleton)
			{
				this.mSkeleton = iSkeleton;
			}

			// Token: 0x17000B98 RID: 2968
			// (get) Token: 0x060031AD RID: 12717 RVA: 0x00198867 File Offset: 0x00196A67
			public bool MeshDirty
			{
				get
				{
					return this.mMeshDirty;
				}
			}

			// Token: 0x17000B99 RID: 2969
			// (get) Token: 0x060031AE RID: 12718 RVA: 0x0019886F File Offset: 0x00196A6F
			public int Effect
			{
				get
				{
					return this.mEffect;
				}
			}

			// Token: 0x17000B9A RID: 2970
			// (get) Token: 0x060031AF RID: 12719 RVA: 0x00198877 File Offset: 0x00196A77
			public int Technique
			{
				get
				{
					return 1;
				}
			}

			// Token: 0x17000B9B RID: 2971
			// (get) Token: 0x060031B0 RID: 12720 RVA: 0x0019887A File Offset: 0x00196A7A
			public VertexBuffer Vertices
			{
				get
				{
					return this.mVertexBuffer;
				}
			}

			// Token: 0x17000B9C RID: 2972
			// (get) Token: 0x060031B1 RID: 12721 RVA: 0x00198882 File Offset: 0x00196A82
			public IndexBuffer Indices
			{
				get
				{
					return this.mIndexBuffer;
				}
			}

			// Token: 0x17000B9D RID: 2973
			// (get) Token: 0x060031B2 RID: 12722 RVA: 0x0019888A File Offset: 0x00196A8A
			public VertexDeclaration VertexDeclaration
			{
				get
				{
					return this.mVertexDeclaration;
				}
			}

			// Token: 0x17000B9E RID: 2974
			// (get) Token: 0x060031B3 RID: 12723 RVA: 0x00198892 File Offset: 0x00196A92
			public int VertexStride
			{
				get
				{
					return this.mVertexStride;
				}
			}

			// Token: 0x17000B9F RID: 2975
			// (get) Token: 0x060031B4 RID: 12724 RVA: 0x0019889A File Offset: 0x00196A9A
			public int VerticesHashCode
			{
				get
				{
					return this.mVerticesHash;
				}
			}

			// Token: 0x060031B5 RID: 12725 RVA: 0x001988A4 File Offset: 0x00196AA4
			public bool Cull(BoundingFrustum iViewFrustum)
			{
				BoundingSphere boundingSphere = this.mBoundingSphere;
				return boundingSphere.Contains(iViewFrustum) == ContainmentType.Disjoint;
			}

			// Token: 0x060031B6 RID: 12726 RVA: 0x001988C4 File Offset: 0x00196AC4
			public virtual void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				if (iEffect.GraphicsDevice.RenderState.ColorWriteChannels == ColorWriteChannels.Alpha)
				{
					iEffect.GraphicsDevice.RenderState.ColorWriteChannels = ColorWriteChannels.All;
					iEffect.GraphicsDevice.RenderState.AlphaBlendEnable = true;
					return;
				}
				SkinnedModelBasicEffect skinnedModelBasicEffect = iEffect as SkinnedModelBasicEffect;
				skinnedModelBasicEffect.GraphicsDevice.RenderState.DestinationBlend = Blend.One;
				skinnedModelBasicEffect.GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha;
				this.mMaterial.AssignToEffect(skinnedModelBasicEffect);
				skinnedModelBasicEffect.Colorize = new Vector4(Character.ColdColor, 1f);
				float num = 0.333f;
				float num2 = 0.333f / ((float)this.mSkeleton.Length + 1f);
				num += this.mIntensity * num2;
				int num3 = 0;
				while (num3 < this.mSkeleton.Length && num > 0f)
				{
					if (num3 != 0)
					{
						skinnedModelBasicEffect.Alpha = num;
						skinnedModelBasicEffect.Bones = this.mSkeleton[num3];
						skinnedModelBasicEffect.CommitChanges();
						skinnedModelBasicEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
						num -= num2;
					}
					num3++;
				}
				skinnedModelBasicEffect.Colorize = default(Vector4);
			}

			// Token: 0x060031B7 RID: 12727 RVA: 0x001989EB File Offset: 0x00196BEB
			public void SetMeshDirty()
			{
				this.mMeshDirty = true;
			}

			// Token: 0x060031B8 RID: 12728 RVA: 0x001989F4 File Offset: 0x00196BF4
			public void SetMesh(VertexBuffer iVertices, IndexBuffer iIndices, ModelMeshPart iMeshPart, int iEffectHash)
			{
				this.mMeshDirty = false;
				SkinnedModelMaterial.CreateFromEffect(iMeshPart.Effect as SkinnedModelBasicEffect, out this.mMaterial);
				this.mVertexBuffer = iVertices;
				this.mVerticesHash = iVertices.GetHashCode();
				this.mIndexBuffer = iIndices;
				this.mEffect = iEffectHash;
				this.mVertexDeclaration = iMeshPart.VertexDeclaration;
				this.mBaseVertex = iMeshPart.BaseVertex;
				this.mNumVertices = iMeshPart.NumVertices;
				this.mPrimitiveCount = iMeshPart.PrimitiveCount;
				this.mStartIndex = iMeshPart.StartIndex;
				this.mStreamOffset = iMeshPart.StreamOffset;
				this.mVertexStride = iMeshPart.VertexStride;
				for (int i = 0; i < this.mSkeleton.Length; i++)
				{
					Matrix[] array = this.mSkeleton[i];
					for (int j = 0; j < array.Length; j++)
					{
						array[j].M11 = (array[j].M22 = (array[j].M33 = (array[j].M44 = float.NaN)));
					}
				}
			}

			// Token: 0x04003614 RID: 13844
			public BoundingSphere mBoundingSphere;

			// Token: 0x04003615 RID: 13845
			protected int mEffect;

			// Token: 0x04003616 RID: 13846
			protected VertexDeclaration mVertexDeclaration;

			// Token: 0x04003617 RID: 13847
			protected int mBaseVertex;

			// Token: 0x04003618 RID: 13848
			protected int mNumVertices;

			// Token: 0x04003619 RID: 13849
			protected int mPrimitiveCount;

			// Token: 0x0400361A RID: 13850
			protected int mStartIndex;

			// Token: 0x0400361B RID: 13851
			protected int mStreamOffset;

			// Token: 0x0400361C RID: 13852
			protected int mVertexStride;

			// Token: 0x0400361D RID: 13853
			protected VertexBuffer mVertexBuffer;

			// Token: 0x0400361E RID: 13854
			protected IndexBuffer mIndexBuffer;

			// Token: 0x0400361F RID: 13855
			public float mIntensity;

			// Token: 0x04003620 RID: 13856
			public Vector3 Color;

			// Token: 0x04003621 RID: 13857
			public Matrix[][] mSkeleton;

			// Token: 0x04003622 RID: 13858
			private SkinnedModelMaterial mMaterial;

			// Token: 0x04003623 RID: 13859
			protected int mVerticesHash;

			// Token: 0x04003624 RID: 13860
			protected bool mMeshDirty = true;
		}
	}
}
