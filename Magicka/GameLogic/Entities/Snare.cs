using System;
using System.Collections.Generic;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Physics;
using Magicka.GameLogic.GameStates;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using XNAnimation;
using XNAnimation.Controllers;
using XNAnimation.Effects;

namespace Magicka.GameLogic.Entities
{
	// Token: 0x020003B3 RID: 947
	internal class Snare : Entity
	{
		// Token: 0x06001D20 RID: 7456 RVA: 0x000CEDD0 File Offset: 0x000CCFD0
		public static Snare GetFromCache(PlayState iPlayState)
		{
			if (Snare.sCache.Count > 0)
			{
				Snare result = Snare.sCache[Snare.sCache.Count - 1];
				Snare.sCache.RemoveAt(Snare.sCache.Count - 1);
				return result;
			}
			return new Snare(iPlayState);
		}

		// Token: 0x06001D21 RID: 7457 RVA: 0x000CEE1F File Offset: 0x000CD01F
		public static void ReturnToCache(Snare iEnsnare)
		{
			Snare.sCache.Add(iEnsnare);
		}

		// Token: 0x06001D22 RID: 7458 RVA: 0x000CEE2C File Offset: 0x000CD02C
		public static void InitializeCache(int iNrOfEnsnares, PlayState iPlayState)
		{
			Snare.sCache = new List<Snare>(iNrOfEnsnares);
			for (int i = 0; i < iNrOfEnsnares; i++)
			{
				Snare.sCache.Add(new Snare(iPlayState));
			}
		}

		// Token: 0x06001D23 RID: 7459 RVA: 0x000CEE60 File Offset: 0x000CD060
		public Snare(PlayState iPlaystate) : base(iPlaystate)
		{
			GraphicsDevice graphicsDevice = Game.Instance.GraphicsDevice;
			lock (graphicsDevice)
			{
				this.mModel = Game.Instance.Content.Load<SkinnedModel>("Models/Effects/Frog_Snare");
			}
			this.mClip = this.mModel.AnimationClips["Snare"];
			this.mController = new AnimationController();
			this.mController.Skeleton = this.mModel.SkeletonBones;
			Matrix matrix;
			Matrix.CreateRotationY(3.1415927f, out matrix);
			foreach (SkinnedModelBone skinnedModelBone in this.mModel.SkeletonBones)
			{
				if (skinnedModelBone.Index == 1)
				{
					this.mStartJoint = (int)skinnedModelBone.Index;
					this.mStartBindPose = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Multiply(ref this.mStartBindPose, ref matrix, out this.mStartBindPose);
					Matrix.Invert(ref this.mStartBindPose, out this.mStartBindPose);
				}
				else if (skinnedModelBone.Index == 2)
				{
					this.mEndJoint = (int)skinnedModelBone.Index;
					this.mEndBindPose = skinnedModelBone.InverseBindPoseTransform;
					Matrix.Multiply(ref this.mEndBindPose, ref matrix, out this.mEndBindPose);
					Matrix.Invert(ref this.mEndBindPose, out this.mEndBindPose);
				}
			}
			this.mBody = new Body();
			this.mCollision = new CollisionSkin(this.mBody);
			this.mCollision.AddPrimitive(new Capsule(Vector3.Zero, Matrix.Identity, 0.2f, 2f), 1, new MaterialProperties(0.2f, 0.8f, 0.8f));
			this.mCollision.callbackFn += this.OnCollision;
			this.mBody.CollisionSkin = this.mCollision;
			this.mBody.Immovable = true;
			this.mBody.Tag = this;
			this.mRenderData = new Snare.RenderData[3];
			this.mRenderData[0] = new Snare.RenderData();
			this.mRenderData[1] = new Snare.RenderData();
			this.mRenderData[2] = new Snare.RenderData();
		}

		// Token: 0x06001D24 RID: 7460 RVA: 0x000CF09C File Offset: 0x000CD29C
		public void Initialize(Character iOwner)
		{
			this.mSnared = null;
			this.mOwner = iOwner;
			this.mController.PlaybackMode = PlaybackMode.Forward;
			this.mController.StartClip(this.mClip, false);
		}

		// Token: 0x06001D25 RID: 7461 RVA: 0x000CF0CC File Offset: 0x000CD2CC
		public bool OnCollision(CollisionSkin iSkin0, int iPrim0, CollisionSkin iSkin1, int iPrim1)
		{
			if (iSkin1.Owner != null && iSkin1.Owner.Tag != this.mOwner)
			{
				Entity entity = iSkin1.Owner.Tag as Entity;
				if (this.mSnared != null)
				{
					this.mSnared = entity;
					this.mController.PlaybackMode = PlaybackMode.Backward;
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001D26 RID: 7462 RVA: 0x000CF124 File Offset: 0x000CD324
		public override void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			base.Update(iDataChannel, iDeltaTime);
			Matrix mouthAttachOrientation = this.mOwner.GetMouthAttachOrientation();
			Vector3 position = this.mOwner.Position;
			this.mBody.MoveTo(position, mouthAttachOrientation);
			Matrix matrix = mouthAttachOrientation;
			matrix.Translation = position;
			this.mController.PlaybackMode = PlaybackMode.Forward;
			this.mController.Update(iDeltaTime, ref matrix, true);
			Matrix matrix2;
			Matrix.Multiply(ref this.mStartBindPose, ref this.mController.SkinnedBoneTransforms[this.mStartJoint], out matrix2);
			Matrix matrix3;
			Matrix.Multiply(ref this.mEndBindPose, ref this.mController.SkinnedBoneTransforms[this.mEndJoint], out matrix3);
			Array.Copy(this.mController.SkinnedBoneTransforms, 0, this.mRenderData[(int)iDataChannel].mSkeleton, 0, 2);
			this.mOwner.PlayState.Scene.AddRenderableObject(iDataChannel, this.mRenderData[(int)iDataChannel]);
			Vector3 translation = matrix2.Translation;
			Vector3 translation2 = matrix3.Translation;
			float num;
			Vector3.Distance(ref translation2, ref translation, out num);
			if (num < 1E-45f)
			{
				this.mDead = true;
				(this.mBody.CollisionSkin.GetPrimitiveLocal(0) as Capsule).Orientation = mouthAttachOrientation;
				(this.mBody.CollisionSkin.GetPrimitiveLocal(0) as Capsule).Length = num;
			}
			if (this.mSnared != null)
			{
				Matrix orientation = this.mSnared.Body.Orientation;
				this.mSnared.Body.MoveTo(translation2, orientation);
			}
		}

		// Token: 0x17000732 RID: 1842
		// (get) Token: 0x06001D27 RID: 7463 RVA: 0x000CF29B File Offset: 0x000CD49B
		public override bool Dead
		{
			get
			{
				return this.mDead;
			}
		}

		// Token: 0x17000733 RID: 1843
		// (get) Token: 0x06001D28 RID: 7464 RVA: 0x000CF2A3 File Offset: 0x000CD4A3
		public override bool Removable
		{
			get
			{
				return this.mDead;
			}
		}

		// Token: 0x06001D29 RID: 7465 RVA: 0x000CF2AB File Offset: 0x000CD4AB
		public override void Kill()
		{
			this.mDead = true;
			this.mSnared = null;
		}

		// Token: 0x06001D2A RID: 7466 RVA: 0x000CF2BB File Offset: 0x000CD4BB
		protected override void IGetNetworkUpdate(out EntityUpdateMessage oMsg, float iPrediction)
		{
			oMsg = default(EntityUpdateMessage);
			oMsg.Features |= EntityFeatures.Position;
			oMsg.Position = this.Position;
		}

		// Token: 0x04001FBB RID: 8123
		private static List<Snare> sCache;

		// Token: 0x04001FBC RID: 8124
		private Snare.RenderData[] mRenderData;

		// Token: 0x04001FBD RID: 8125
		private SkinnedModel mModel;

		// Token: 0x04001FBE RID: 8126
		private AnimationClip mClip;

		// Token: 0x04001FBF RID: 8127
		private AnimationController mController;

		// Token: 0x04001FC0 RID: 8128
		private int mStartJoint;

		// Token: 0x04001FC1 RID: 8129
		private Matrix mStartBindPose;

		// Token: 0x04001FC2 RID: 8130
		private int mEndJoint;

		// Token: 0x04001FC3 RID: 8131
		private Matrix mEndBindPose;

		// Token: 0x04001FC4 RID: 8132
		private Entity mSnared;

		// Token: 0x04001FC5 RID: 8133
		private Character mOwner;

		// Token: 0x020003B4 RID: 948
		protected class RenderData : IRenderableObject
		{
			// Token: 0x06001D2B RID: 7467 RVA: 0x000CF2DF File Offset: 0x000CD4DF
			public RenderData()
			{
				this.mSkeleton = new Matrix[80];
			}

			// Token: 0x17000734 RID: 1844
			// (get) Token: 0x06001D2C RID: 7468 RVA: 0x000CF2F4 File Offset: 0x000CD4F4
			public int Effect
			{
				get
				{
					return this.mEffect;
				}
			}

			// Token: 0x17000735 RID: 1845
			// (get) Token: 0x06001D2D RID: 7469 RVA: 0x000CF2FC File Offset: 0x000CD4FC
			public int DepthTechnique
			{
				get
				{
					return 3;
				}
			}

			// Token: 0x17000736 RID: 1846
			// (get) Token: 0x06001D2E RID: 7470 RVA: 0x000CF2FF File Offset: 0x000CD4FF
			public int Technique
			{
				get
				{
					return 0;
				}
			}

			// Token: 0x17000737 RID: 1847
			// (get) Token: 0x06001D2F RID: 7471 RVA: 0x000CF302 File Offset: 0x000CD502
			public int ShadowTechnique
			{
				get
				{
					return 4;
				}
			}

			// Token: 0x17000738 RID: 1848
			// (get) Token: 0x06001D30 RID: 7472 RVA: 0x000CF305 File Offset: 0x000CD505
			public VertexBuffer Vertices
			{
				get
				{
					return this.mVertexBuffer;
				}
			}

			// Token: 0x17000739 RID: 1849
			// (get) Token: 0x06001D31 RID: 7473 RVA: 0x000CF30D File Offset: 0x000CD50D
			public IndexBuffer Indices
			{
				get
				{
					return this.mIndexBuffer;
				}
			}

			// Token: 0x1700073A RID: 1850
			// (get) Token: 0x06001D32 RID: 7474 RVA: 0x000CF315 File Offset: 0x000CD515
			public VertexDeclaration VertexDeclaration
			{
				get
				{
					return this.mVertexDeclaration;
				}
			}

			// Token: 0x1700073B RID: 1851
			// (get) Token: 0x06001D33 RID: 7475 RVA: 0x000CF31D File Offset: 0x000CD51D
			public int VertexStride
			{
				get
				{
					return this.mVertexStride;
				}
			}

			// Token: 0x1700073C RID: 1852
			// (get) Token: 0x06001D34 RID: 7476 RVA: 0x000CF325 File Offset: 0x000CD525
			public Texture2D Texture
			{
				get
				{
					return this.mSkinnedModelMaterial.DiffuseMap0;
				}
			}

			// Token: 0x1700073D RID: 1853
			// (get) Token: 0x06001D35 RID: 7477 RVA: 0x000CF332 File Offset: 0x000CD532
			public int VerticesHashCode
			{
				get
				{
					return this.mVerticesHash;
				}
			}

			// Token: 0x06001D36 RID: 7478 RVA: 0x000CF33C File Offset: 0x000CD53C
			public bool Cull(BoundingFrustum iViewFrustum)
			{
				return default(BoundingSphere).Contains(iViewFrustum) == ContainmentType.Disjoint;
			}

			// Token: 0x06001D37 RID: 7479 RVA: 0x000CF35C File Offset: 0x000CD55C
			public void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				SkinnedModelBasicEffect skinnedModelBasicEffect = iEffect as SkinnedModelBasicEffect;
				this.mSkinnedModelMaterial.AssignToEffect(skinnedModelBasicEffect);
				skinnedModelBasicEffect.Bones = this.mSkeleton;
				skinnedModelBasicEffect.OverlayColor = default(Vector4);
				skinnedModelBasicEffect.SpecularBias = 0f;
				skinnedModelBasicEffect.OverlayMapEnabled = false;
				skinnedModelBasicEffect.OverlayNormalMapEnabled = false;
				skinnedModelBasicEffect.CommitChanges();
				skinnedModelBasicEffect.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
			}

			// Token: 0x06001D38 RID: 7480 RVA: 0x000CF3DC File Offset: 0x000CD5DC
			public void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				SkinnedModelBasicEffect skinnedModelBasicEffect = iEffect as SkinnedModelBasicEffect;
				this.mSkinnedModelMaterial.AssignToEffect(skinnedModelBasicEffect);
				skinnedModelBasicEffect.Bones = this.mSkeleton;
				skinnedModelBasicEffect.CommitChanges();
				skinnedModelBasicEffect.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
			}

			// Token: 0x06001D39 RID: 7481 RVA: 0x000CF434 File Offset: 0x000CD634
			public void SetMesh(VertexBuffer iVertices, IndexBuffer iIndices, ModelMeshPart iMeshPart, int iEffectHash)
			{
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
				SkinnedModelMaterial.CreateFromEffect(iMeshPart.Effect as SkinnedModelBasicEffect, out this.mSkinnedModelMaterial);
			}

			// Token: 0x04001FC6 RID: 8134
			protected int mEffect;

			// Token: 0x04001FC7 RID: 8135
			protected VertexDeclaration mVertexDeclaration;

			// Token: 0x04001FC8 RID: 8136
			protected int mBaseVertex;

			// Token: 0x04001FC9 RID: 8137
			protected int mNumVertices;

			// Token: 0x04001FCA RID: 8138
			protected int mPrimitiveCount;

			// Token: 0x04001FCB RID: 8139
			protected int mStartIndex;

			// Token: 0x04001FCC RID: 8140
			protected int mStreamOffset;

			// Token: 0x04001FCD RID: 8141
			protected int mVertexStride;

			// Token: 0x04001FCE RID: 8142
			protected VertexBuffer mVertexBuffer;

			// Token: 0x04001FCF RID: 8143
			protected IndexBuffer mIndexBuffer;

			// Token: 0x04001FD0 RID: 8144
			public Matrix[] mSkeleton;

			// Token: 0x04001FD1 RID: 8145
			private SkinnedModelMaterial mSkinnedModelMaterial;

			// Token: 0x04001FD2 RID: 8146
			public int mVerticesHash;
		}
	}
}
