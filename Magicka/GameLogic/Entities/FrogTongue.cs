using System;
using System.Collections.Generic;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Math;
using JigLibX.Physics;
using Magicka.GameLogic.Entities.Bosses;
using Magicka.GameLogic.GameStates;
using Magicka.Graphics;
using Magicka.Levels;
using Magicka.Network;
using Magicka.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using XNAnimation;
using XNAnimation.Effects;

namespace Magicka.GameLogic.Entities
{
	// Token: 0x02000173 RID: 371
	internal class FrogTongue : Entity, IDamageable
	{
		// Token: 0x06000B39 RID: 2873 RVA: 0x0004359C File Offset: 0x0004179C
		public static FrogTongue GetInstance(PlayState iPlayState)
		{
			if (FrogTongue.sCache.Count > 0)
			{
				FrogTongue result = FrogTongue.sCache[FrogTongue.sCache.Count - 1];
				FrogTongue.sCache.RemoveAt(FrogTongue.sCache.Count - 1);
				return result;
			}
			return new FrogTongue(iPlayState);
		}

		// Token: 0x06000B3A RID: 2874 RVA: 0x000435EC File Offset: 0x000417EC
		public static void InitializeCache(int iNr, PlayState iPlayState)
		{
			FrogTongue.sCache = new List<FrogTongue>(iNr);
			for (int i = 0; i < iNr; i++)
			{
				FrogTongue.sCache.Add(new FrogTongue(iPlayState));
			}
		}

		// Token: 0x06000B3B RID: 2875 RVA: 0x00043620 File Offset: 0x00041820
		public FrogTongue(PlayState iPlayState) : base(iPlayState)
		{
			SkinnedModel skinnedModel;
			lock (Game.Instance.GraphicsDevice)
			{
				skinnedModel = Game.Instance.Content.Load<SkinnedModel>("Models/Missiles/Tongue");
			}
			this.mBody = new Body();
			this.mCollision = new CollisionSkin(this.mBody);
			this.mCollision.AddPrimitive(new Sphere(Vector3.Zero, 0.5f), 1, new MaterialProperties(0.333f, 0.8f, 0.8f));
			this.mBody.CollisionSkin = this.mCollision;
			Vector3 vector = base.SetMass(4f);
			Transform transform = default(Transform);
			Vector3.Negate(ref vector, out transform.Position);
			transform.Orientation = Matrix.Identity;
			this.mCollision.ApplyLocalTransform(transform);
			this.mCollision.callbackFn += this.OnCollision;
			this.mBody.Tag = this;
			this.mBody.Immovable = false;
			this.mBody.AllowFreezing = false;
			this.mOwner = null;
			Matrix matrix;
			Matrix.CreateRotationY(3.1415927f, out matrix);
			for (int i = 0; i < skinnedModel.SkeletonBones.Count; i++)
			{
				SkinnedModelBone skinnedModelBone = skinnedModel.SkeletonBones[i];
				if (skinnedModelBone.Name.Equals("joint1", StringComparison.OrdinalIgnoreCase))
				{
					this.mStartBindPose = skinnedModelBone.InverseBindPoseTransform;
				}
				else if (skinnedModelBone.Name.Equals("joint2", StringComparison.OrdinalIgnoreCase))
				{
					this.mEndBindPose = skinnedModelBone.InverseBindPoseTransform;
				}
			}
			this.mDead = true;
			ModelMesh modelMesh = skinnedModel.Model.Meshes[0];
			ModelMeshPart iMeshPart = modelMesh.MeshParts[0];
			this.mSkeleton = new Matrix[skinnedModel.SkeletonBones.Count];
			this.mRenderData = new FrogTongue.RenderData[3];
			for (int j = 0; j < this.mRenderData.Length; j++)
			{
				this.mRenderData[j] = new FrogTongue.RenderData();
				this.mRenderData[j].mSkeleton = new Matrix[skinnedModel.SkeletonBones.Count];
				this.mRenderData[j].SetMesh(modelMesh.VertexBuffer, modelMesh.IndexBuffer, iMeshPart, SkinnedModelBasicEffect.TYPEHASH);
			}
		}

		// Token: 0x06000B3C RID: 2876 RVA: 0x00043878 File Offset: 0x00041A78
		protected bool OnCollision(CollisionSkin iSkin0, int iPrim0, CollisionSkin iSkin1, int iPrim1)
		{
			if (this.Dead && iSkin1.Owner != null)
			{
				return false;
			}
			if (iSkin1.Owner is CharacterBody)
			{
				if (this.mAttachedCharacter != null)
				{
					return false;
				}
				if (iSkin1.Owner.Tag is Character && iSkin1.Owner.Tag as Character != this.mOwner)
				{
					this.mPullingBack = true;
					this.mAttachedCharacter = (iSkin1.Owner.Tag as Character);
					this.mVelocity = 30f;
				}
			}
			else if (iSkin1.Tag is LevelModel)
			{
				this.mPullingBack = true;
				this.mVelocity = 80f;
				Vector3 position = this.mBody.Position;
				Vector3 forward = this.mBody.Orientation.Forward;
				VisualEffectReference visualEffectReference;
				EffectManager.Instance.StartEffect(FrogTongue.HIT_GROUND_SPLASH_EFFECT, ref position, ref forward, out visualEffectReference);
			}
			else if (iSkin1.Owner is IBoss)
			{
				return false;
			}
			return true;
		}

		// Token: 0x06000B3D RID: 2877 RVA: 0x00043970 File Offset: 0x00041B70
		public void Initialize(Character iOwner, Vector3 iDirection, float iMaxLengthSquared)
		{
			base.Initialize();
			this.mPullingBack = false;
			this.mOwner = iOwner;
			this.mAttachedCharacter = null;
			this.mMaxLengthSquared = iMaxLengthSquared;
			Vector3 up = Vector3.Up;
			Vector3 translation = iOwner.GetMouthAttachOrientation().Translation;
			iDirection.Y *= -0.5f;
			Matrix.CreateWorld(ref translation, ref iDirection, ref up, out this.mOrientation);
			this.mBody.MoveTo(translation, Matrix.Identity);
			Vector3.Multiply(ref iDirection, 60f, out iDirection);
			this.mBody.Velocity = iDirection;
			this.mBody.EnableBody();
			this.mAudioEmitter.Position = translation;
			this.mAudioEmitter.Forward = Vector3.Forward;
			this.mAudioEmitter.Up = Vector3.Up;
			this.mSkeleton[0] = Matrix.Identity;
			this.mSkeleton[1] = Matrix.Identity;
			this.mPlayState.EntityManager.AddEntity(this);
		}

		// Token: 0x06000B3E RID: 2878 RVA: 0x00043A77 File Offset: 0x00041C77
		public float ResistanceAgainst(Elements iElement)
		{
			return 0f;
		}

		// Token: 0x06000B3F RID: 2879 RVA: 0x00043A80 File Offset: 0x00041C80
		public override void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			this.mStartPoint = this.mOwner.GetMouthAttachOrientation().Translation;
			this.mEndPoint = this.mBody.Position;
			Matrix matrix = this.mOrientation;
			matrix.Translation = this.mStartPoint;
			Matrix.Multiply(ref this.mStartBindPose, ref matrix, out matrix);
			this.mSkeleton[0] = matrix;
			matrix = this.mOrientation;
			matrix.Translation = this.mEndPoint;
			Matrix.Multiply(ref this.mEndBindPose, ref matrix, out matrix);
			this.mSkeleton[1] = matrix;
			this.mBody.AngularVelocity = default(Vector3);
			if (this.mPullingBack)
			{
				Vector3 velocity;
				Vector3.Subtract(ref this.mStartPoint, ref this.mEndPoint, out velocity);
				float num = velocity.LengthSquared();
				if (this.mAttachedCharacter != null)
				{
					float num2 = Math.Abs(velocity.X) + Math.Abs(velocity.Z);
					if (num2 > 2f)
					{
						velocity.Y = 0f;
					}
				}
				velocity.Normalize();
				Vector3.Multiply(ref velocity, this.mVelocity, out velocity);
				if (num > 1f)
				{
					this.mBody.Velocity = velocity;
				}
				else
				{
					this.mDead = true;
				}
				if (!this.mDead && this.mAttachedCharacter != null)
				{
					if (this.mAttachedCharacter.Dead || this.mAttachedCharacter.IsGripped || this.mAttachedCharacter.HasStatus(StatusEffects.Burning | StatusEffects.Greased))
					{
						this.mAttachedCharacter = null;
					}
					else
					{
						Vector3 position = this.mBody.Position;
						position.Y -= (this.mBody.CollisionSkin.GetPrimitiveLocal(0) as Sphere).Radius;
						position.Y += -this.mAttachedCharacter.HeightOffset;
						this.mAttachedCharacter.Body.MoveTo(position, this.mAttachedCharacter.Body.Orientation);
					}
				}
			}
			else
			{
				float num;
				Vector3.DistanceSquared(ref this.mStartPoint, ref this.mEndPoint, out num);
				if (num >= this.mMaxLengthSquared)
				{
					this.mPullingBack = true;
					this.mVelocity = ((this.mAttachedCharacter == null) ? 80f : 30f);
				}
			}
			Array.Copy(this.mSkeleton, this.mRenderData[(int)iDataChannel].mSkeleton, this.mSkeleton.Length);
			GameStateManager.Instance.CurrentState.Scene.AddRenderableObject(iDataChannel, this.mRenderData[(int)iDataChannel]);
			base.Update(iDataChannel, iDeltaTime);
		}

		// Token: 0x170002A4 RID: 676
		// (get) Token: 0x06000B40 RID: 2880 RVA: 0x00043D07 File Offset: 0x00041F07
		public Character AttachedCharacter
		{
			get
			{
				return this.mAttachedCharacter;
			}
		}

		// Token: 0x170002A5 RID: 677
		// (get) Token: 0x06000B41 RID: 2881 RVA: 0x00043D0F File Offset: 0x00041F0F
		public bool PullingBack
		{
			get
			{
				return this.mPullingBack;
			}
		}

		// Token: 0x170002A6 RID: 678
		// (get) Token: 0x06000B42 RID: 2882 RVA: 0x00043D17 File Offset: 0x00041F17
		public Vector3 Start
		{
			get
			{
				return this.mStartPoint;
			}
		}

		// Token: 0x170002A7 RID: 679
		// (get) Token: 0x06000B43 RID: 2883 RVA: 0x00043D1F File Offset: 0x00041F1F
		public Vector3 End
		{
			get
			{
				return this.mEndPoint;
			}
		}

		// Token: 0x06000B44 RID: 2884 RVA: 0x00043D27 File Offset: 0x00041F27
		public override void Deinitialize()
		{
			base.Deinitialize();
			this.mOwner = null;
			this.mDead = true;
		}

		// Token: 0x170002A8 RID: 680
		// (get) Token: 0x06000B45 RID: 2885 RVA: 0x00043D3D File Offset: 0x00041F3D
		public override bool Dead
		{
			get
			{
				return this.mDead;
			}
		}

		// Token: 0x170002A9 RID: 681
		// (get) Token: 0x06000B46 RID: 2886 RVA: 0x00043D45 File Offset: 0x00041F45
		public override bool Removable
		{
			get
			{
				return this.mDead;
			}
		}

		// Token: 0x06000B47 RID: 2887 RVA: 0x00043D4D File Offset: 0x00041F4D
		public override void Kill()
		{
			this.mDead = true;
		}

		// Token: 0x06000B48 RID: 2888 RVA: 0x00043D58 File Offset: 0x00041F58
		protected override void IGetNetworkUpdate(out EntityUpdateMessage oMsg, float iPrediction)
		{
			Transform transform = this.mBody.Transform;
			TransformRate transformRate = this.mBody.TransformRate;
			transform.ApplyTransformRate(ref transformRate, iPrediction);
			oMsg = default(EntityUpdateMessage);
			oMsg.Features |= EntityFeatures.Position;
			oMsg.Position = transform.Position;
			oMsg.Features |= EntityFeatures.Orientation;
			Quaternion.CreateFromRotationMatrix(ref transform.Orientation, out oMsg.Orientation);
			oMsg.Features |= EntityFeatures.Velocity;
			oMsg.Velocity = this.mBody.Velocity;
		}

		// Token: 0x170002AA RID: 682
		// (get) Token: 0x06000B49 RID: 2889 RVA: 0x00043DEB File Offset: 0x00041FEB
		public float HitPoints
		{
			get
			{
				return 1f;
			}
		}

		// Token: 0x170002AB RID: 683
		// (get) Token: 0x06000B4A RID: 2890 RVA: 0x00043DF2 File Offset: 0x00041FF2
		public float MaxHitPoints
		{
			get
			{
				return 1f;
			}
		}

		// Token: 0x06000B4B RID: 2891 RVA: 0x00043DFC File Offset: 0x00041FFC
		public bool SegmentIntersect(out Vector3 oPosition, Segment iSeg, float iSegmentRadius)
		{
			float num;
			Vector3 vector;
			return this.mBody.CollisionSkin.SegmentIntersect(out num, out oPosition, out vector, iSeg);
		}

		// Token: 0x06000B4C RID: 2892 RVA: 0x00043E20 File Offset: 0x00042020
		public DamageResult InternalDamage(DamageCollection5 iDamages, Entity iAttacker, double iTimeStamp, Vector3 iAttackPosition, Defines.DamageFeatures iFeatures)
		{
			DamageResult damageResult = DamageResult.None;
			damageResult |= this.InternalDamage(iDamages.A, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
			damageResult |= this.InternalDamage(iDamages.B, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
			damageResult |= this.InternalDamage(iDamages.C, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
			damageResult |= this.InternalDamage(iDamages.D, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
			return damageResult | this.InternalDamage(iDamages.E, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
		}

		// Token: 0x06000B4D RID: 2893 RVA: 0x00043E9E File Offset: 0x0004209E
		public DamageResult InternalDamage(Damage iDamage, Entity iAttacker, double iTimeStamp, Vector3 iAttackPosition, Defines.DamageFeatures iFeatures)
		{
			if ((short)(iDamage.AttackProperty & AttackProperties.Damage) == 1 && iDamage.Amount > 0f)
			{
				this.mPullingBack = true;
				this.mVelocity = 80f;
				this.mAttachedCharacter = null;
			}
			return DamageResult.None;
		}

		// Token: 0x06000B4E RID: 2894 RVA: 0x00043ED5 File Offset: 0x000420D5
		public void Electrocute(IDamageable iTarget, float iMultiplyer)
		{
		}

		// Token: 0x06000B4F RID: 2895 RVA: 0x00043ED7 File Offset: 0x000420D7
		public void OverKill()
		{
			throw new NotImplementedException();
		}

		// Token: 0x04000A37 RID: 2615
		private const float SHOOT_VELOCITY = 60f;

		// Token: 0x04000A38 RID: 2616
		private const float DRAG_VELOCITY = 30f;

		// Token: 0x04000A39 RID: 2617
		private const float PULL_VELOCITY = 80f;

		// Token: 0x04000A3A RID: 2618
		private static List<FrogTongue> sCache;

		// Token: 0x04000A3B RID: 2619
		private static readonly int HIT_GROUND_SPLASH_EFFECT = "generic_dust".GetHashCodeCustom();

		// Token: 0x04000A3C RID: 2620
		private static readonly int HIT_GREEN_SPLASH_EFFECT = Gib.GORE_GIB_MEDIUM_EFFECTS[1];

		// Token: 0x04000A3D RID: 2621
		private Character mAttachedCharacter;

		// Token: 0x04000A3E RID: 2622
		private Character mOwner;

		// Token: 0x04000A3F RID: 2623
		private Matrix[] mSkeleton;

		// Token: 0x04000A40 RID: 2624
		private FrogTongue.RenderData[] mRenderData;

		// Token: 0x04000A41 RID: 2625
		private bool mPullingBack;

		// Token: 0x04000A42 RID: 2626
		private float mMaxLengthSquared;

		// Token: 0x04000A43 RID: 2627
		private Matrix mStartBindPose;

		// Token: 0x04000A44 RID: 2628
		private Matrix mEndBindPose;

		// Token: 0x04000A45 RID: 2629
		private Matrix mOrientation;

		// Token: 0x04000A46 RID: 2630
		private Vector3 mStartPoint;

		// Token: 0x04000A47 RID: 2631
		private Vector3 mEndPoint;

		// Token: 0x04000A48 RID: 2632
		private float mVelocity;

		// Token: 0x02000174 RID: 372
		protected class RenderData : IRenderableObject
		{
			// Token: 0x170002AC RID: 684
			// (get) Token: 0x06000B51 RID: 2897 RVA: 0x00043EFB File Offset: 0x000420FB
			public bool MeshDirty
			{
				get
				{
					return this.mMeshDirty;
				}
			}

			// Token: 0x170002AD RID: 685
			// (get) Token: 0x06000B52 RID: 2898 RVA: 0x00043F03 File Offset: 0x00042103
			public int Effect
			{
				get
				{
					return this.mEffect;
				}
			}

			// Token: 0x170002AE RID: 686
			// (get) Token: 0x06000B53 RID: 2899 RVA: 0x00043F0B File Offset: 0x0004210B
			public int DepthTechnique
			{
				get
				{
					return 3;
				}
			}

			// Token: 0x170002AF RID: 687
			// (get) Token: 0x06000B54 RID: 2900 RVA: 0x00043F10 File Offset: 0x00042110
			public int Technique
			{
				get
				{
					switch (this.mMaterial.Technique)
					{
					default:
						return 0;
					case SkinnedModelBasicEffect.Technique.Additive:
						return 2;
					}
				}
			}

			// Token: 0x170002B0 RID: 688
			// (get) Token: 0x06000B55 RID: 2901 RVA: 0x00043F3E File Offset: 0x0004213E
			public int ShadowTechnique
			{
				get
				{
					return 4;
				}
			}

			// Token: 0x170002B1 RID: 689
			// (get) Token: 0x06000B56 RID: 2902 RVA: 0x00043F41 File Offset: 0x00042141
			public VertexBuffer Vertices
			{
				get
				{
					return this.mVertexBuffer;
				}
			}

			// Token: 0x170002B2 RID: 690
			// (get) Token: 0x06000B57 RID: 2903 RVA: 0x00043F49 File Offset: 0x00042149
			public IndexBuffer Indices
			{
				get
				{
					return this.mIndexBuffer;
				}
			}

			// Token: 0x170002B3 RID: 691
			// (get) Token: 0x06000B58 RID: 2904 RVA: 0x00043F51 File Offset: 0x00042151
			public VertexDeclaration VertexDeclaration
			{
				get
				{
					return this.mVertexDeclaration;
				}
			}

			// Token: 0x170002B4 RID: 692
			// (get) Token: 0x06000B59 RID: 2905 RVA: 0x00043F59 File Offset: 0x00042159
			public int VertexStride
			{
				get
				{
					return this.mVertexStride;
				}
			}

			// Token: 0x170002B5 RID: 693
			// (get) Token: 0x06000B5A RID: 2906 RVA: 0x00043F61 File Offset: 0x00042161
			public int VerticesHashCode
			{
				get
				{
					return this.mVerticesHash;
				}
			}

			// Token: 0x170002B6 RID: 694
			// (get) Token: 0x06000B5B RID: 2907 RVA: 0x00043F69 File Offset: 0x00042169
			public Texture2D Texture
			{
				get
				{
					return this.mMaterial.DiffuseMap0;
				}
			}

			// Token: 0x06000B5C RID: 2908 RVA: 0x00043F76 File Offset: 0x00042176
			public bool Cull(BoundingFrustum iViewFrustum)
			{
				return false;
			}

			// Token: 0x06000B5D RID: 2909 RVA: 0x00043F7C File Offset: 0x0004217C
			public virtual void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				SkinnedModelBasicEffect skinnedModelBasicEffect = iEffect as SkinnedModelBasicEffect;
				skinnedModelBasicEffect.Damage = 0f;
				this.mMaterial.AssignToEffect(skinnedModelBasicEffect);
				skinnedModelBasicEffect.OverlayColor = default(Vector4);
				skinnedModelBasicEffect.SpecularBias = 0f;
				skinnedModelBasicEffect.OverlayMapEnabled = false;
				skinnedModelBasicEffect.OverlayNormalMapEnabled = false;
				skinnedModelBasicEffect.Bones = this.mSkeleton;
				skinnedModelBasicEffect.CommitChanges();
				skinnedModelBasicEffect.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
			}

			// Token: 0x06000B5E RID: 2910 RVA: 0x00044008 File Offset: 0x00042208
			public virtual void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				SkinnedModelBasicEffect skinnedModelBasicEffect = iEffect as SkinnedModelBasicEffect;
				this.mMaterial.AssignToEffect(skinnedModelBasicEffect);
				skinnedModelBasicEffect.Bones = this.mSkeleton;
				skinnedModelBasicEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertexBuffer, 0, this.mVertexStride);
				skinnedModelBasicEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
				skinnedModelBasicEffect.GraphicsDevice.Indices = this.mIndexBuffer;
				skinnedModelBasicEffect.CommitChanges();
				skinnedModelBasicEffect.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
			}

			// Token: 0x06000B5F RID: 2911 RVA: 0x000440A4 File Offset: 0x000422A4
			public void SetMeshDirty()
			{
				this.mMeshDirty = true;
			}

			// Token: 0x06000B60 RID: 2912 RVA: 0x000440B0 File Offset: 0x000422B0
			public void SetMesh(VertexBuffer iVertices, IndexBuffer iIndices, ModelMeshPart iMeshPart, int iEffectHash)
			{
				this.mMeshDirty = false;
				lock (iMeshPart.Effect.GraphicsDevice)
				{
					SkinnedModelMaterial.CreateFromEffect(iMeshPart.Effect as SkinnedModelBasicEffect, out this.mMaterial);
				}
				this.mVertexBuffer = iVertices;
				this.mVerticesHash = iVertices.GetHashCode();
				this.mIndexBuffer = iIndices;
				this.mEffect = iEffectHash;
				this.mVertexDeclaration = iMeshPart.VertexDeclaration;
				this.mBaseVertex = iMeshPart.BaseVertex;
				this.mVerticesHash = iVertices.GetHashCode();
				this.mNumVertices = iMeshPart.NumVertices;
				this.mPrimitiveCount = iMeshPart.PrimitiveCount;
				this.mStartIndex = iMeshPart.StartIndex;
				this.mStreamOffset = iMeshPart.StreamOffset;
				this.mVertexStride = iMeshPart.VertexStride;
			}

			// Token: 0x04000A49 RID: 2633
			protected int mEffect;

			// Token: 0x04000A4A RID: 2634
			protected VertexDeclaration mVertexDeclaration;

			// Token: 0x04000A4B RID: 2635
			protected int mBaseVertex;

			// Token: 0x04000A4C RID: 2636
			protected int mNumVertices;

			// Token: 0x04000A4D RID: 2637
			protected int mPrimitiveCount;

			// Token: 0x04000A4E RID: 2638
			protected int mStartIndex;

			// Token: 0x04000A4F RID: 2639
			protected int mStreamOffset;

			// Token: 0x04000A50 RID: 2640
			protected int mVertexStride;

			// Token: 0x04000A51 RID: 2641
			protected VertexBuffer mVertexBuffer;

			// Token: 0x04000A52 RID: 2642
			protected int mVerticesHash;

			// Token: 0x04000A53 RID: 2643
			protected IndexBuffer mIndexBuffer;

			// Token: 0x04000A54 RID: 2644
			public Matrix[] mSkeleton;

			// Token: 0x04000A55 RID: 2645
			public SkinnedModelMaterial mMaterial;

			// Token: 0x04000A56 RID: 2646
			protected bool mMeshDirty = true;
		}
	}
}
