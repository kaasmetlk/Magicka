using System;
using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Magicka.Graphics;
using Magicka.Graphics.Effects;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using XNAnimation;
using XNAnimation.Effects;

namespace Magicka.GameLogic.Entities
{
	// Token: 0x0200064A RID: 1610
	public class Entanglement
	{
		// Token: 0x0600310A RID: 12554 RVA: 0x001937D0 File Offset: 0x001919D0
		public static void DisposeModels()
		{
			if (Entanglement.mModels != null)
			{
				Entanglement.mModels = null;
			}
		}

		// Token: 0x0600310B RID: 12555 RVA: 0x001937E0 File Offset: 0x001919E0
		public Entanglement(Character iOwner)
		{
			this.mOwner = iOwner;
			PlayState playState = iOwner.PlayState;
			GraphicsDevice graphicsDevice = Game.Instance.GraphicsDevice;
			if (Entanglement.mModels == null)
			{
				lock (graphicsDevice)
				{
					Entanglement.mModels = playState.Content.Load<SkinnedModel>("Models/Effects/Entangle_web");
				}
				for (int i = 0; i < Entanglement.mModels.SkeletonBones.Count; i++)
				{
					SkinnedModelBone skinnedModelBone = Entanglement.mModels.SkeletonBones[i];
					if (skinnedModelBone.Index == 0)
					{
						Entanglement.mRootJoint = (int)skinnedModelBone.Index;
					}
					else if (skinnedModelBone.Index == 1)
					{
						Entanglement.mHipJoint = (int)skinnedModelBone.Index;
						Entanglement.mHipBindPose = skinnedModelBone.InverseBindPoseTransform;
					}
				}
				Entanglement.mWeakness = Elements.Fire;
			}
			ModelMesh modelMesh = Entanglement.mModels.Model.Meshes[0];
			ModelMeshPart iMeshPart = modelMesh.MeshParts[0];
			lock (graphicsDevice)
			{
				this.mEffect = new EntangleEffect(graphicsDevice, Game.Instance.Content);
			}
			this.mRenderData = new Entanglement.RenderData[3];
			for (int j = 0; j < 3; j++)
			{
				this.mRenderData[j] = new Entanglement.RenderData();
				this.mRenderData[j].SetMesh(modelMesh.VertexBuffer, modelMesh.IndexBuffer, iMeshPart, EntangleEffect.TYPEHASH);
			}
			this.mFadeTimer = 0f;
			float length = iOwner.Capsule.Length;
			float radius = iOwner.Capsule.Radius;
			this.mSkinScaleMatrix = Matrix.Identity;
			this.mSkinScaleMatrix.M11 = this.mSkinScaleMatrix.M11 * radius;
			this.mSkinScaleMatrix.M12 = this.mSkinScaleMatrix.M12 * radius;
			this.mSkinScaleMatrix.M13 = this.mSkinScaleMatrix.M13 * radius;
			this.mSkinScaleMatrix.M21 = this.mSkinScaleMatrix.M21 * length;
			this.mSkinScaleMatrix.M22 = this.mSkinScaleMatrix.M22 * length;
			this.mSkinScaleMatrix.M23 = this.mSkinScaleMatrix.M23 * length;
			this.mSkinScaleMatrix.M31 = this.mSkinScaleMatrix.M31 * radius;
			this.mSkinScaleMatrix.M32 = this.mSkinScaleMatrix.M32 * radius;
			this.mSkinScaleMatrix.M33 = this.mSkinScaleMatrix.M33 * radius;
		}

		// Token: 0x0600310C RID: 12556 RVA: 0x00193A40 File Offset: 0x00191C40
		public void Initialize()
		{
			this.mFadeTimer = 0.5f;
			ModelMesh modelMesh = Entanglement.mModels.Model.Meshes[0];
			ModelMeshPart modelMeshPart = modelMesh.MeshParts[0];
			this.mEffect.DiffuseMap = (modelMeshPart.Effect as SkinnedModelBasicEffect).DiffuseMap0;
			for (int i = 0; i < 3; i++)
			{
				this.mRenderData[i].SetMesh(modelMesh.VertexBuffer, modelMesh.IndexBuffer, modelMeshPart, EntangleEffect.TYPEHASH);
			}
			this.mOwnerLength = this.mOwner.Capsule.Length;
			this.mOwnerRadius = this.mOwner.Capsule.Radius;
			float num = (this.mOwnerRadius + this.mOwnerLength) * 0.8f;
			float num2 = this.mOwnerRadius * 1.3333334f;
			this.mSkinScaleMatrix = Matrix.Identity;
			this.mSkinScaleMatrix.M11 = this.mSkinScaleMatrix.M11 * num2;
			this.mSkinScaleMatrix.M12 = this.mSkinScaleMatrix.M12 * num2;
			this.mSkinScaleMatrix.M13 = this.mSkinScaleMatrix.M13 * num2;
			this.mSkinScaleMatrix.M21 = this.mSkinScaleMatrix.M21 * num;
			this.mSkinScaleMatrix.M22 = this.mSkinScaleMatrix.M22 * num;
			this.mSkinScaleMatrix.M23 = this.mSkinScaleMatrix.M23 * num;
			this.mSkinScaleMatrix.M31 = this.mSkinScaleMatrix.M31 * num2;
			this.mSkinScaleMatrix.M32 = this.mSkinScaleMatrix.M32 * num2;
			this.mSkinScaleMatrix.M33 = this.mSkinScaleMatrix.M33 * num2;
		}

		// Token: 0x0600310D RID: 12557 RVA: 0x00193BCC File Offset: 0x00191DCC
		public void Update(DataChannel iDataChannel, float iDeltaTime, ref BoundingSphere iBoundingSphere)
		{
			if (this.mMagnitude > 1E-45f || this.mFadeTimer > 1E-45f)
			{
				Entanglement.RenderData renderData = this.mRenderData[(int)iDataChannel];
				renderData.mBoundingSphere = iBoundingSphere;
				Matrix orientation = this.mOwner.Body.Orientation;
				Matrix hipAttachOrientation = this.mOwner.GetHipAttachOrientation();
				Matrix.Multiply(ref this.mSkinScaleMatrix, ref orientation, out orientation);
				Vector3 position = this.mOwner.Position;
				position.Y -= this.mOwner.Capsule.Radius + this.mOwner.Capsule.Length * 0.5f;
				orientation.Translation = position;
				renderData.mBones[Entanglement.mRootJoint] = orientation;
				Matrix.Multiply(ref hipAttachOrientation, ref Entanglement.mHipBindPose, out hipAttachOrientation);
				orientation.Translation = hipAttachOrientation.Translation;
				renderData.mBones[Entanglement.mHipJoint] = orientation;
				if (this.mMagnitude <= 0f)
				{
					this.mFadeTimer -= iDeltaTime;
				}
				renderData.mVisibility = 0.5f * (this.mMagnitude / 20f) + this.mFadeTimer;
				this.mOwner.PlayState.Scene.AddRenderableObject(iDataChannel, renderData);
			}
		}

		// Token: 0x0600310E RID: 12558 RVA: 0x00193D19 File Offset: 0x00191F19
		public void AddEntanglement(float iMagnitude)
		{
			this.mMagnitude += iMagnitude;
			if (this.mMagnitude > 20f)
			{
				this.mMagnitude = 20f;
			}
		}

		// Token: 0x0600310F RID: 12559 RVA: 0x00193D44 File Offset: 0x00191F44
		public void DecreaseEntanglement(float iMagnitude, Elements iElement)
		{
			if (this.mMagnitude <= 0f)
			{
				return;
			}
			Vector3 position = this.mOwner.Position;
			Vector3 direction = this.mOwner.Direction;
			VisualEffectReference visualEffectReference;
			EffectManager.Instance.StartEffect(Entanglement.HIT_EFFECT, ref position, ref direction, out visualEffectReference);
			float num = this.mMagnitude - iMagnitude;
			if ((iElement & Entanglement.mWeakness) != Elements.None || num < 0f)
			{
				this.mFadeTimer = 0.5f;
				this.Release();
				return;
			}
			this.mMagnitude = num;
		}

		// Token: 0x06003110 RID: 12560 RVA: 0x00193DC0 File Offset: 0x00191FC0
		public void Release()
		{
			if (NetworkManager.Instance.State != NetworkState.Offline)
			{
				CharacterActionMessage characterActionMessage = default(CharacterActionMessage);
				characterActionMessage.Action = ActionType.Release;
				characterActionMessage.Handle = this.mOwner.Handle;
				NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref characterActionMessage);
			}
			if (this.mMagnitude > 0f)
			{
				AudioManager.Instance.PlayCue(Banks.Misc, Entanglement.BREAK_SOUND, this.mOwner.AudioEmitter);
				this.mFadeTimer = 0.5f;
			}
			this.mMagnitude = 0f;
		}

		// Token: 0x17000B83 RID: 2947
		// (get) Token: 0x06003111 RID: 12561 RVA: 0x00193E50 File Offset: 0x00192050
		public float Magnitude
		{
			get
			{
				return this.mMagnitude;
			}
		}

		// Token: 0x0400352D RID: 13613
		public const float MAX_MAGNITUDE = 20f;

		// Token: 0x0400352E RID: 13614
		private static readonly int BREAK_SOUND = "misc_spider_web_cut".GetHashCodeCustom();

		// Token: 0x0400352F RID: 13615
		private static readonly int BREAK_EFFECT = "web_hit".GetHashCodeCustom();

		// Token: 0x04003530 RID: 13616
		private static readonly int HIT_EFFECT = "web_hit".GetHashCodeCustom();

		// Token: 0x04003531 RID: 13617
		private static Elements mWeakness;

		// Token: 0x04003532 RID: 13618
		private static SkinnedModel mModels;

		// Token: 0x04003533 RID: 13619
		private static int mRootJoint;

		// Token: 0x04003534 RID: 13620
		private static int mHipJoint;

		// Token: 0x04003535 RID: 13621
		private static Matrix mHipBindPose;

		// Token: 0x04003536 RID: 13622
		private Entanglement.RenderData[] mRenderData;

		// Token: 0x04003537 RID: 13623
		private float mMagnitude;

		// Token: 0x04003538 RID: 13624
		private float mFadeTimer;

		// Token: 0x04003539 RID: 13625
		private Character mOwner;

		// Token: 0x0400353A RID: 13626
		private Matrix mSkinScaleMatrix;

		// Token: 0x0400353B RID: 13627
		private float mOwnerRadius;

		// Token: 0x0400353C RID: 13628
		private float mOwnerLength;

		// Token: 0x0400353D RID: 13629
		private EntangleEffect mEffect;

		// Token: 0x0200064B RID: 1611
		protected class RenderData : IRenderableObject, IRenderableAdditiveObject
		{
			// Token: 0x17000B84 RID: 2948
			// (get) Token: 0x06003113 RID: 12563 RVA: 0x00193E87 File Offset: 0x00192087
			public int Effect
			{
				get
				{
					return this.mEffect;
				}
			}

			// Token: 0x17000B85 RID: 2949
			// (get) Token: 0x06003114 RID: 12564 RVA: 0x00193E8F File Offset: 0x0019208F
			public int DepthTechnique
			{
				get
				{
					return this.mDepthTechnique;
				}
			}

			// Token: 0x17000B86 RID: 2950
			// (get) Token: 0x06003115 RID: 12565 RVA: 0x00193E97 File Offset: 0x00192097
			public int Technique
			{
				get
				{
					return this.mTechnique;
				}
			}

			// Token: 0x17000B87 RID: 2951
			// (get) Token: 0x06003116 RID: 12566 RVA: 0x00193E9F File Offset: 0x0019209F
			public int ShadowTechnique
			{
				get
				{
					return this.mShadowTechnique;
				}
			}

			// Token: 0x17000B88 RID: 2952
			// (get) Token: 0x06003117 RID: 12567 RVA: 0x00193EA7 File Offset: 0x001920A7
			public VertexBuffer Vertices
			{
				get
				{
					return this.mVertices;
				}
			}

			// Token: 0x17000B89 RID: 2953
			// (get) Token: 0x06003118 RID: 12568 RVA: 0x00193EAF File Offset: 0x001920AF
			public IndexBuffer Indices
			{
				get
				{
					return this.mIndices;
				}
			}

			// Token: 0x17000B8A RID: 2954
			// (get) Token: 0x06003119 RID: 12569 RVA: 0x00193EB7 File Offset: 0x001920B7
			public VertexDeclaration VertexDeclaration
			{
				get
				{
					return this.mVertexDeclaration;
				}
			}

			// Token: 0x17000B8B RID: 2955
			// (get) Token: 0x0600311A RID: 12570 RVA: 0x00193EBF File Offset: 0x001920BF
			public int VertexStride
			{
				get
				{
					return this.mVertexStride;
				}
			}

			// Token: 0x17000B8C RID: 2956
			// (get) Token: 0x0600311B RID: 12571 RVA: 0x00193EC7 File Offset: 0x001920C7
			public int VerticesHashCode
			{
				get
				{
					return this.mVerticesHash;
				}
			}

			// Token: 0x0600311C RID: 12572 RVA: 0x00193ED0 File Offset: 0x001920D0
			public bool Cull(BoundingFrustum iViewFrustum)
			{
				BoundingSphere boundingSphere = this.mBoundingSphere;
				return boundingSphere.Contains(iViewFrustum) == ContainmentType.Disjoint;
			}

			// Token: 0x0600311D RID: 12573 RVA: 0x00193EF0 File Offset: 0x001920F0
			public void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				EntangleEffect entangleEffect = iEffect as EntangleEffect;
				entangleEffect.MatBones = this.mBones;
				entangleEffect.SpecularPower = 20f;
				entangleEffect.SpecularAmount = 0.5f;
				entangleEffect.AlphaBias = this.mVisibility * 0.5f - 1f;
				entangleEffect.ColorTint = Vector4.One;
				entangleEffect.DiffuseMapEnabled = true;
				entangleEffect.DiffuseMap = this.mTexture;
				entangleEffect.DiffuseColor = new Vector3(1f);
				entangleEffect.CommitChanges();
				entangleEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
				entangleEffect.DiffuseMapEnabled = false;
			}

			// Token: 0x0600311E RID: 12574 RVA: 0x00193FA0 File Offset: 0x001921A0
			public void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				EntangleEffect entangleEffect = iEffect as EntangleEffect;
				entangleEffect.MatBones = this.mBones;
				entangleEffect.AlphaBias = this.mVisibility * 0.5f - 1f;
				entangleEffect.ColorTint = Vector4.One;
				entangleEffect.DiffuseMapEnabled = true;
				entangleEffect.DiffuseMap = this.mTexture;
				entangleEffect.CommitChanges();
				entangleEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
				entangleEffect.DiffuseMapEnabled = false;
			}

			// Token: 0x0600311F RID: 12575 RVA: 0x00194028 File Offset: 0x00192228
			public void SetMesh(VertexBuffer iVertices, IndexBuffer iIndices, ModelMeshPart iMeshPart, int iEffectHash)
			{
				this.mVertices = iVertices;
				this.mVerticesHash = iVertices.GetHashCode();
				this.mIndices = iIndices;
				this.mTechnique = 0;
				this.mDepthTechnique = 2;
				this.mShadowTechnique = 3;
				this.mEffect = iEffectHash;
				this.mVertexDeclaration = iMeshPart.VertexDeclaration;
				this.mBaseVertex = iMeshPart.BaseVertex;
				this.mNumVertices = iMeshPart.NumVertices;
				this.mPrimitiveCount = iMeshPart.PrimitiveCount;
				this.mStartIndex = iMeshPart.StartIndex;
				this.mStreamOffset = iMeshPart.StreamOffset;
				this.mVertexStride = iMeshPart.VertexStride;
				this.mTexture = (iMeshPart.Effect as SkinnedModelBasicEffect).DiffuseMap0;
			}

			// Token: 0x0400353E RID: 13630
			protected VertexBuffer mVertices;

			// Token: 0x0400353F RID: 13631
			protected IndexBuffer mIndices;

			// Token: 0x04003540 RID: 13632
			public BoundingSphere mBoundingSphere;

			// Token: 0x04003541 RID: 13633
			protected int mEffect;

			// Token: 0x04003542 RID: 13634
			protected VertexDeclaration mVertexDeclaration;

			// Token: 0x04003543 RID: 13635
			protected int mBaseVertex;

			// Token: 0x04003544 RID: 13636
			protected int mNumVertices;

			// Token: 0x04003545 RID: 13637
			protected int mPrimitiveCount;

			// Token: 0x04003546 RID: 13638
			protected int mStartIndex;

			// Token: 0x04003547 RID: 13639
			protected int mStreamOffset;

			// Token: 0x04003548 RID: 13640
			protected int mVertexStride;

			// Token: 0x04003549 RID: 13641
			private Texture2D mTexture;

			// Token: 0x0400354A RID: 13642
			public Matrix[] mBones = new Matrix[10];

			// Token: 0x0400354B RID: 13643
			public float mVisibility;

			// Token: 0x0400354C RID: 13644
			public int mVerticesHash;

			// Token: 0x0400354D RID: 13645
			protected int mTechnique;

			// Token: 0x0400354E RID: 13646
			protected int mDepthTechnique;

			// Token: 0x0400354F RID: 13647
			protected int mShadowTechnique;
		}
	}
}
