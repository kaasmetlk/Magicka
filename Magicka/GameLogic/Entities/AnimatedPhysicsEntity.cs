using System;
using JigLibX.Collision;
using JigLibX.Geometry;
using Magicka.GameLogic.Entities.AnimationActions;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Levels;
using Magicka.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using XNAnimation;
using XNAnimation.Controllers;
using XNAnimation.Effects;

namespace Magicka.GameLogic.Entities
{
	// Token: 0x020001CE RID: 462
	internal class AnimatedPhysicsEntity : DamageablePhysicsEntity
	{
		// Token: 0x17000407 RID: 1031
		// (get) Token: 0x06000FA0 RID: 4000 RVA: 0x00060EF7 File Offset: 0x0005F0F7
		// (set) Token: 0x06000FA1 RID: 4001 RVA: 0x00060EFF File Offset: 0x0005F0FF
		public bool IsDamageable
		{
			get
			{
				return this.mIsDamageable;
			}
			set
			{
				this.mIsDamageable = value;
			}
		}

		// Token: 0x17000408 RID: 1032
		// (get) Token: 0x06000FA2 RID: 4002 RVA: 0x00060F08 File Offset: 0x0005F108
		// (set) Token: 0x06000FA3 RID: 4003 RVA: 0x00060F10 File Offset: 0x0005F110
		public bool IsAffectedByGravity
		{
			get
			{
				return this.mIsAffectedByGravity;
			}
			set
			{
				this.mIsAffectedByGravity = value;
				if (this.mBody != null)
				{
					this.mBody.ApplyGravity = this.mIsAffectedByGravity;
				}
			}
		}

		// Token: 0x06000FA4 RID: 4004 RVA: 0x00060F34 File Offset: 0x0005F134
		public AnimatedPhysicsEntity(PlayState iPlayState) : base(iPlayState)
		{
			this.mAnimationController = new AnimationController();
			this.mAnimationController.AnimationLooped += this.OnAnimationLooped;
			this.mAnimationController.CrossfadeFinished += this.OnCrossfadeFinished;
			this.mAnimatedRenderData = new AnimatedPhysicsEntity.AnimatedRenderData[3];
			for (int i = 0; i < 3; i++)
			{
				this.mAnimatedRenderData[i] = new AnimatedPhysicsEntity.AnimatedRenderData(this);
			}
			if (AnimatedPhysicsEntity.sIceCubeMap == null)
			{
				lock (Game.Instance.GraphicsDevice)
				{
					AnimatedPhysicsEntity.sIceCubeMap = Game.Instance.Content.Load<TextureCube>("EffectTextures/iceCube");
				}
			}
			if (AnimatedPhysicsEntity.sIceCubeNormalMap == null)
			{
				lock (Game.Instance.GraphicsDevice)
				{
					AnimatedPhysicsEntity.sIceCubeNormalMap = Game.Instance.Content.Load<TextureCube>("EffectTextures/iceCube_NRM");
				}
			}
		}

		// Token: 0x06000FA5 RID: 4005 RVA: 0x00061054 File Offset: 0x0005F254
		protected override void CreateBody()
		{
			this.mBody = new PhysicsObjectBody();
			this.mCollision = new CollisionSkin(this.mBody);
			this.mCollision.AddPrimitive(new Box(new Vector3(-this.mRadius, 0f, -this.mRadius), Matrix.Identity, Vector3.One * (this.mRadius * 2f)), 1, new MaterialProperties(0f, 1f, 1f));
			this.mCollision.callbackFn -= base.OnCollision;
			this.mCollision.callbackFn += base.OnCollision;
			this.mCollision.postCollisionCallbackFn -= base.PostCollision;
			this.mCollision.postCollisionCallbackFn += base.PostCollision;
			this.mBody.CollisionSkin = this.mCollision;
			this.mBody.Immovable = !this.mPushable;
			this.mBody.AllowFreezing = true;
			this.mBody.ApplyGravity = this.mIsAffectedByGravity;
			this.mBody.Tag = this;
		}

		// Token: 0x06000FA6 RID: 4006 RVA: 0x00061180 File Offset: 0x0005F380
		protected override void CorrectWithTemplateBox()
		{
		}

		// Token: 0x06000FA7 RID: 4007 RVA: 0x00061184 File Offset: 0x0005F384
		public override void Initialize(PhysicsEntityTemplate iTemplate, Matrix iStartTransform, int iUniqueID)
		{
			this.mRadius = iTemplate.mRadius;
			if (this.mRadius <= 0f)
			{
				this.mRadius = 1f;
			}
			base.Initialize(iTemplate, iStartTransform, iUniqueID);
			this.mLastDraw = 0f;
			this.mStatic = false;
			if (this.mBody.Mass > 0f && this.mIsAffectedByGravity)
			{
				Vector3 vector = this.mBody.Position;
				if (this.mPlayState.Level != null && this.mPlayState.Level.CurrentScene != null)
				{
					Segment iSeg = default(Segment);
					iSeg.Origin = vector;
					iSeg.Origin.Y = iSeg.Origin.Y + 1f;
					iSeg.Delta.Y = -10f;
					float num;
					Vector3 vector2;
					Vector3 vector3;
					AnimatedLevelPart animatedLevelPart;
					if (this.mPlayState.Level.CurrentScene.SegmentIntersect(out num, out vector2, out vector3, out animatedLevelPart, iSeg))
					{
						vector = vector2;
						vector.Y += 0.1f;
					}
				}
				else
				{
					vector.Y += this.mRadius * 2f;
				}
				this.mBody.Position = vector;
			}
			this.mAnimationClips = iTemplate.AnimationClips;
			if (iTemplate.Models != null)
			{
				this.mModelID = MagickaMath.Random.Next(0, iTemplate.Models.Length);
				this.mModel = iTemplate.Models[this.mModelID].Model;
				this.mStaticTransform = iTemplate.Models[this.mModelID].Transform;
				Matrix matrix;
				Matrix.Invert(ref this.mStaticTransform, out matrix);
				Helper.SkinnedModelDeferredMaterialFromBasicEffect(this.mModel.Model.Meshes[0].MeshParts[0].Effect as SkinnedModelBasicEffect, out this.mMaterial);
				this.mMaterial.TintColor = iTemplate.Models[this.mModelID].Tint;
			}
			this.mAnimationController.Skeleton = iTemplate.Skeleton;
			this.mCurrentAnimation = Animations.idle;
			this.mCurrentAnimationSet = WeaponClass.Default;
			this.mAnimationController.ClearCrossfadeQueue();
			if (this.mModel != null)
			{
				ModelMesh modelMesh = this.mModel.Model.Meshes[0];
				this.mDefaultSpecular = (modelMesh.Effects[0] as SkinnedModelBasicEffect).SpecularPower;
				if (this.mDefaultSpecular < 0.1f)
				{
					this.mDefaultSpecular = 0.1f;
				}
			}
			for (int i = 0; i < 3; i++)
			{
				AnimatedPhysicsEntity.AnimatedRenderData animatedRenderData = this.mAnimatedRenderData[i];
				animatedRenderData.mNumVertices = iTemplate.VertexCount;
				animatedRenderData.mVertexStride = iTemplate.VertexStride;
				animatedRenderData.mPrimitiveCount = iTemplate.PrimitiveCount;
				animatedRenderData.mVertexBuffer = iTemplate.Vertices;
				if (animatedRenderData.mVertexBuffer != null)
				{
					animatedRenderData.mVerticesHash = animatedRenderData.mVertexBuffer.GetHashCode();
				}
				else
				{
					animatedRenderData.mVerticesHash = 0;
				}
				animatedRenderData.mVertexDeclaration = iTemplate.SkeletonVertexDeclaration;
				animatedRenderData.mIndexBuffer = iTemplate.Indices;
				animatedRenderData.SetMeshDirty();
			}
		}

		// Token: 0x06000FA8 RID: 4008 RVA: 0x0006149C File Offset: 0x0005F69C
		public override void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			base.Update(iDataChannel, iDeltaTime);
			if (!this.Dead && this.mAnimationController != null)
			{
				this.mLastDraw += iDeltaTime;
				if (!this.mAnimationController.HasFinished | this.mAnimationController.CrossFadeEnabled)
				{
					Matrix orientation = this.GetOrientation();
					bool updateBoneTransforms = this.mLastDraw < 0.2f || this.mForceAnimationUpdate;
					this.mAnimationController.Update(iDeltaTime, ref orientation, updateBoneTransforms);
				}
			}
		}

		// Token: 0x06000FA9 RID: 4009 RVA: 0x00061518 File Offset: 0x0005F718
		internal void SetStaticTransform(float iX, float iY, float iZ)
		{
			Matrix matrix;
			Matrix.CreateScale(iX, iY, iZ, out matrix);
			Matrix.Multiply(ref matrix, ref this.mTemplate.Models[this.mModelID].Transform, out this.mStaticTransform);
		}

		// Token: 0x06000FAA RID: 4010 RVA: 0x00061557 File Offset: 0x0005F757
		internal void ScaleGraphicModel(float iScale)
		{
			this.mStaticTransform.M11 = iScale;
			this.mStaticTransform.M22 = iScale;
			this.mStaticTransform.M33 = iScale;
		}

		// Token: 0x06000FAB RID: 4011 RVA: 0x00061580 File Offset: 0x0005F780
		protected override void UpdateRenderData(DataChannel iDataChannel)
		{
			if (this.mModel == null || this.mModel.SkeletonBones == null || this.mModel.SkeletonBones.Count == 0)
			{
				base.UpdateRenderData(iDataChannel);
				return;
			}
			AnimatedPhysicsEntity.AnimatedRenderData animatedRenderData = this.mAnimatedRenderData[(int)iDataChannel];
			int count = this.mModel.SkeletonBones.Count;
			Array.Copy(this.mAnimationController.SkinnedBoneTransforms, 0, animatedRenderData.mSkeleton, 0, count);
			BoundingSphere boundingSphere = this.mModel.Model.Meshes[0].BoundingSphere;
			boundingSphere.Center = this.Position;
			boundingSphere.Radius *= this.mBoundingScale * this.mStaticTransform.M11;
			animatedRenderData.mBoundingSphere = boundingSphere;
			animatedRenderData.mMaterial.CubeNormalMapEnabled = false;
			float num = this.mStatusEffects[StatusEffect.StatusIndex(StatusEffects.Cold)].Magnitude;
			num *= 10f;
			num = Math.Min(num, 1f);
			animatedRenderData.mMaterial.Colorize.X = AnimatedPhysicsEntity.ColdColor.X;
			animatedRenderData.mMaterial.Colorize.Y = AnimatedPhysicsEntity.ColdColor.Y;
			animatedRenderData.mMaterial.Colorize.Z = AnimatedPhysicsEntity.ColdColor.Z;
			animatedRenderData.mMaterial.Colorize.W = num;
			animatedRenderData.mMaterial.Bloat = 0f;
			Vector3.Multiply(ref animatedRenderData.mMaterial.DiffuseColor, 1f + num, out animatedRenderData.mMaterial.DiffuseColor);
			if (base.HasStatus(StatusEffects.Frozen))
			{
				animatedRenderData.mMaterial.CubeMapRotation = Matrix.Identity;
				animatedRenderData.mMaterial.Bloat = (float)((int)(base.StatusMagnitude(StatusEffects.Frozen) * 10f)) * 0.02f;
				animatedRenderData.mMaterial.EmissiveAmount = 0.5f;
				animatedRenderData.mMaterial.SpecularBias = 1f;
				animatedRenderData.mMaterial.SpecularPower = 10f;
				animatedRenderData.mMaterial.CubeMapEnabled = true;
				animatedRenderData.mMaterial.CubeNormalMapEnabled = true;
				animatedRenderData.mMaterial.CubeMap = AnimatedPhysicsEntity.sIceCubeMap;
				animatedRenderData.mMaterial.CubeNormalMap = AnimatedPhysicsEntity.sIceCubeNormalMap;
				animatedRenderData.mMaterial.CubeMapColor.X = (animatedRenderData.mMaterial.CubeMapColor.Y = (animatedRenderData.mMaterial.CubeMapColor.Z = 1f));
				animatedRenderData.mMaterial.CubeMapColor.W = ((base.StatusMagnitude(StatusEffects.Frozen) > 0f) ? 1f : 0f);
			}
			else if (base.HasStatus(StatusEffects.Wet))
			{
				animatedRenderData.mMaterial.ProjectionMap = Game.Instance.Content.Load<Texture2D>("EffectTextures/wetMap");
				animatedRenderData.mMaterial.ProjectionMapEnabled = true;
				animatedRenderData.mMaterial.CubeMapEnabled = false;
				Vector3 vector = this.Position + Vector3.Up + Vector3.Forward - this.Position;
				Vector3 direction = this.Direction;
				float angle = 0.2f;
				Quaternion quaternion;
				Quaternion.CreateFromAxisAngle(ref vector, angle, out quaternion);
				Vector3.Transform(ref direction, ref quaternion, out direction);
				float scale = 1f;
				animatedRenderData.mMaterial.ProjectionMapMatrix = Matrix.CreateLookAt(this.Position + Vector3.Up + new Vector3(0f, 0f, -1f), this.Position, direction) * Matrix.CreateScale(scale);
				animatedRenderData.mMaterial.SpecularBias = 0.5f;
				animatedRenderData.mMaterial.SpecularPower = this.mMaterial.SpecularPower * 2f;
				Vector3 wetColor = AnimatedPhysicsEntity.WetColor;
				Vector3.Multiply(ref animatedRenderData.mMaterial.DiffuseColor, ref wetColor, out animatedRenderData.mMaterial.DiffuseColor);
			}
			else if (base.HasStatus(StatusEffects.Burning))
			{
				animatedRenderData.mMaterial.ProjectionMap = Game.Instance.Content.Load<Texture2D>("EffectTextures/burnMap");
				animatedRenderData.mMaterial.ProjectionMapEnabled = true;
				animatedRenderData.mMaterial.CubeMapEnabled = false;
				animatedRenderData.mMaterial.ProjectionMapColor = new Vector4(1f);
				animatedRenderData.mMaterial.ProjectionMapColor.W = base.StatusMagnitude(StatusEffects.Burning);
				animatedRenderData.mMaterial.ProjectionMapAdditive = true;
				Vector3 vector2 = this.Position + Vector3.Up - this.Position;
				Vector3 direction2 = this.Direction;
				Quaternion quaternion2;
				Quaternion.CreateFromAxisAngle(ref vector2, 0f, out quaternion2);
				Vector3.Transform(ref direction2, ref quaternion2, out direction2);
				float scale2 = 1f;
				animatedRenderData.mMaterial.ProjectionMapMatrix = Matrix.CreateLookAt(this.Position + Vector3.Up, this.Position, direction2) * Matrix.CreateScale(scale2);
				animatedRenderData.mMaterial.EmissiveAmount = base.StatusMagnitude(StatusEffects.Burning);
				animatedRenderData.mMaterial.SpecularBias = 0f;
				animatedRenderData.mMaterial.SpecularPower = this.mMaterial.SpecularPower;
			}
			else if (base.HasStatus(StatusEffects.Greased))
			{
				animatedRenderData.mMaterial.ProjectionMap = Game.Instance.Content.Load<Texture2D>("EffectTextures/Greased");
				animatedRenderData.mMaterial.ProjectionMapEnabled = true;
				animatedRenderData.mMaterial.CubeMapEnabled = false;
				Vector3 vector3 = this.Position + Vector3.Down - this.Position;
				Vector3 direction3 = this.Direction;
				Quaternion quaternion3;
				Quaternion.CreateFromAxisAngle(ref vector3, 1f, out quaternion3);
				Vector3.Transform(ref direction3, ref quaternion3, out direction3);
				float scale3 = 1f;
				animatedRenderData.mMaterial.ProjectionMapMatrix = Matrix.CreateLookAt(this.Position + Vector3.Down, this.Position, direction3) * Matrix.CreateScale(scale3);
				animatedRenderData.mMaterial.EmissiveAmount = this.mMaterial.EmissiveAmount;
				animatedRenderData.mMaterial.SpecularBias = 0.5f;
				animatedRenderData.mMaterial.SpecularPower = this.mMaterial.SpecularPower * 2f;
			}
			else
			{
				animatedRenderData.mMaterial.SpecularPower = this.mMaterial.SpecularPower;
				animatedRenderData.mMaterial.SpecularAmount = this.mMaterial.SpecularAmount;
				animatedRenderData.mMaterial.SpecularBias = 0f;
				animatedRenderData.mMaterial.ProjectionMapEnabled = false;
				animatedRenderData.mMaterial.CubeMapEnabled = false;
				animatedRenderData.mMaterial.EmissiveAmount = this.mMaterial.EmissiveAmount;
			}
			animatedRenderData.mMaterial.Damage = 1f - this.mHitPoints / this.mMaxHitPoints;
			ModelMesh modelMesh = this.mModel.Model.Meshes[0];
			ModelMeshPart modelMeshPart = modelMesh.MeshParts[0];
			if (animatedRenderData.MeshDirty)
			{
				SkinnedModelDeferredEffect.Technique activeTechnique = (SkinnedModelDeferredEffect.Technique)(modelMeshPart.Effect as SkinnedModelBasicEffect).ActiveTechnique;
				animatedRenderData.SetMesh(modelMesh.VertexBuffer, modelMesh.IndexBuffer, modelMeshPart, ref this.mMaterial, activeTechnique);
			}
			animatedRenderData.mTechnique = SkinnedModelDeferredEffect.Technique.Default;
			this.mPlayState.Scene.AddRenderableObject(iDataChannel, animatedRenderData);
		}

		// Token: 0x06000FAC RID: 4012 RVA: 0x00061C84 File Offset: 0x0005FE84
		public void GoToAnimation(Animations iAnimation, float iTime)
		{
			if (this.mCurrentAnimation != iAnimation | this.mAnimationController.HasFinished)
			{
				this.mCurrentAnimation = iAnimation;
				this.mCurrentAnimationSet = WeaponClass.Default;
				AnimationClipAction animationClipAction = this.mAnimationClips[(int)this.mCurrentAnimationSet][(int)iAnimation];
				if (animationClipAction == null)
				{
					this.mCurrentAnimation = Animations.idle;
					this.mCurrentAnimationSet = WeaponClass.Default;
					return;
				}
				this.mAnimationController.ClipSpeed = animationClipAction.AnimationSpeed;
				float blendTime = animationClipAction.BlendTime;
				this.mAnimationController.CrossFade(animationClipAction.Clip, (blendTime > 0f) ? blendTime : iTime, animationClipAction.LoopAnimation);
			}
		}

		// Token: 0x06000FAD RID: 4013 RVA: 0x00061D18 File Offset: 0x0005FF18
		public void ForceAnimation(Animations iAnimation)
		{
			this.mAnimationController.ClearCrossfadeQueue();
			this.mCurrentAnimation = iAnimation;
			this.mCurrentAnimationSet = WeaponClass.Default;
			AnimationClipAction animationClipAction = this.mAnimationClips[(int)this.mCurrentAnimationSet][(int)iAnimation];
			if (animationClipAction == null)
			{
				this.mCurrentAnimation = Animations.idle;
				this.mCurrentAnimationSet = WeaponClass.Default;
				return;
			}
			this.mAnimationController.ClipSpeed = animationClipAction.AnimationSpeed;
			this.mAnimationController.StartClip(animationClipAction.Clip, animationClipAction.LoopAnimation);
			this.OnCrossfadeFinished();
		}

		// Token: 0x06000FAE RID: 4014 RVA: 0x00061D90 File Offset: 0x0005FF90
		protected void OnAnimationLooped()
		{
			for (int i = 0; i < this.mCurrentActions.Length; i++)
			{
			}
		}

		// Token: 0x06000FAF RID: 4015 RVA: 0x00061DB0 File Offset: 0x0005FFB0
		protected void OnCrossfadeFinished()
		{
			AnimationClipAction[] array = this.mAnimationClips[(int)this.mCurrentAnimationSet];
			AnimationClipAction animationClipAction = array[(int)this.mCurrentAnimation];
			this.mCurrentActions = animationClipAction.Actions;
			this.mForceAnimationUpdate = false;
			for (int i = 0; i < this.mCurrentActions.Length; i++)
			{
				if (this.mCurrentActions[i].UsesBones)
				{
					this.mForceAnimationUpdate = true;
				}
			}
		}

		// Token: 0x06000FB0 RID: 4016 RVA: 0x00061E10 File Offset: 0x00060010
		public override DamageResult InternalDamage(DamageCollection5 iDamages, Entity iAttacker, double iTimeStamp, Vector3 iAttackPosition, Defines.DamageFeatures iFeatures)
		{
			if (!this.mIsDamageable)
			{
				return DamageResult.Deflected;
			}
			return base.InternalDamage(iDamages, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
		}

		// Token: 0x06000FB1 RID: 4017 RVA: 0x00061E30 File Offset: 0x00060030
		public override DamageResult InternalDamage(Damage iDamage, Entity iAttacker, double iTimeStamp, Vector3 iAttackPosition, Defines.DamageFeatures iFeatures)
		{
			bool flag = (short)(iDamage.AttackProperty & AttackProperties.Pushed) == 4;
			flag &= ((iDamage.Element & Elements.Earth) == Elements.Earth || 0 == 0);
			flag &= this.mPushable;
			if (!this.mIsDamageable && !flag)
			{
				return DamageResult.Deflected;
			}
			return base.InternalDamage(iDamage, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
		}

		// Token: 0x06000FB2 RID: 4018 RVA: 0x00061E88 File Offset: 0x00060088
		public override DamageResult AddStatusEffect(StatusEffect iStatusEffect)
		{
			if (!this.mIsDamageable)
			{
				return DamageResult.Deflected;
			}
			return base.AddStatusEffect(iStatusEffect);
		}

		// Token: 0x04000E1E RID: 3614
		private static readonly Vector3 WetColor = new Vector3(0.75f, 0.8f, 1f);

		// Token: 0x04000E1F RID: 3615
		private static readonly Vector3 ColdColor = new Vector3(1f, 1.6f, 2f);

		// Token: 0x04000E20 RID: 3616
		protected static Matrix sRotateX90 = Matrix.CreateRotationX(-1.5707964f);

		// Token: 0x04000E21 RID: 3617
		private SkinnedModel mModel;

		// Token: 0x04000E22 RID: 3618
		private AnimationController mAnimationController;

		// Token: 0x04000E23 RID: 3619
		private AnimationClipAction[][] mAnimationClips;

		// Token: 0x04000E24 RID: 3620
		private AnimationAction[] mCurrentActions;

		// Token: 0x04000E25 RID: 3621
		private WeaponClass mCurrentAnimationSet;

		// Token: 0x04000E26 RID: 3622
		private Animations mCurrentAnimation;

		// Token: 0x04000E27 RID: 3623
		private Matrix mStaticTransform;

		// Token: 0x04000E28 RID: 3624
		private SkinnedModelDeferredBasicMaterial mMaterial;

		// Token: 0x04000E29 RID: 3625
		private AnimatedPhysicsEntity.AnimatedRenderData[] mAnimatedRenderData;

		// Token: 0x04000E2A RID: 3626
		protected static TextureCube sIceCubeMap;

		// Token: 0x04000E2B RID: 3627
		protected static TextureCube sIceCubeNormalMap;

		// Token: 0x04000E2C RID: 3628
		private int mModelID;

		// Token: 0x04000E2D RID: 3629
		private float mDefaultSpecular = 0.5f;

		// Token: 0x04000E2E RID: 3630
		private float mLastDraw;

		// Token: 0x04000E2F RID: 3631
		private float mBoundingScale = 1.333f;

		// Token: 0x04000E30 RID: 3632
		private bool mIsDamageable;

		// Token: 0x04000E31 RID: 3633
		private bool mIsAffectedByGravity = true;

		// Token: 0x04000E32 RID: 3634
		private bool mForceAnimationUpdate;

		// Token: 0x020001CF RID: 463
		private class AnimatedRenderData : IRenderableObject, IRenderableAdditiveObject
		{
			// Token: 0x17000409 RID: 1033
			// (get) Token: 0x06000FB4 RID: 4020 RVA: 0x00061EEE File Offset: 0x000600EE
			// (set) Token: 0x06000FB5 RID: 4021 RVA: 0x00061EF6 File Offset: 0x000600F6
			public int PrimitiveCount
			{
				get
				{
					return this.mPrimitiveCount;
				}
				private set
				{
					this.mPrimitiveCount = value;
				}
			}

			// Token: 0x06000FB6 RID: 4022 RVA: 0x00061EFF File Offset: 0x000600FF
			public AnimatedRenderData(AnimatedPhysicsEntity owner)
			{
				this.mOwner = owner;
				this.mSkeleton = new Matrix[80];
			}

			// Token: 0x1700040A RID: 1034
			// (get) Token: 0x06000FB7 RID: 4023 RVA: 0x00061F22 File Offset: 0x00060122
			public bool MeshDirty
			{
				get
				{
					return this.mMeshDirty;
				}
			}

			// Token: 0x1700040B RID: 1035
			// (get) Token: 0x06000FB8 RID: 4024 RVA: 0x00061F2A File Offset: 0x0006012A
			public int Effect
			{
				get
				{
					return SkinnedModelDeferredEffect.TYPEHASH;
				}
			}

			// Token: 0x1700040C RID: 1036
			// (get) Token: 0x06000FB9 RID: 4025 RVA: 0x00061F31 File Offset: 0x00060131
			public int DepthTechnique
			{
				get
				{
					return 3;
				}
			}

			// Token: 0x1700040D RID: 1037
			// (get) Token: 0x06000FBA RID: 4026 RVA: 0x00061F34 File Offset: 0x00060134
			public int Technique
			{
				get
				{
					return (int)this.mTechnique;
				}
			}

			// Token: 0x1700040E RID: 1038
			// (get) Token: 0x06000FBB RID: 4027 RVA: 0x00061F3C File Offset: 0x0006013C
			public int ShadowTechnique
			{
				get
				{
					return 4;
				}
			}

			// Token: 0x1700040F RID: 1039
			// (get) Token: 0x06000FBC RID: 4028 RVA: 0x00061F3F File Offset: 0x0006013F
			public VertexBuffer Vertices
			{
				get
				{
					return this.mVertexBuffer;
				}
			}

			// Token: 0x17000410 RID: 1040
			// (get) Token: 0x06000FBD RID: 4029 RVA: 0x00061F47 File Offset: 0x00060147
			public IndexBuffer Indices
			{
				get
				{
					return this.mIndexBuffer;
				}
			}

			// Token: 0x17000411 RID: 1041
			// (get) Token: 0x06000FBE RID: 4030 RVA: 0x00061F4F File Offset: 0x0006014F
			public VertexDeclaration VertexDeclaration
			{
				get
				{
					return this.mVertexDeclaration;
				}
			}

			// Token: 0x17000412 RID: 1042
			// (get) Token: 0x06000FBF RID: 4031 RVA: 0x00061F57 File Offset: 0x00060157
			public int VertexStride
			{
				get
				{
					return this.mVertexStride;
				}
			}

			// Token: 0x17000413 RID: 1043
			// (get) Token: 0x06000FC0 RID: 4032 RVA: 0x00061F5F File Offset: 0x0006015F
			public int VerticesHashCode
			{
				get
				{
					return this.mVerticesHash;
				}
			}

			// Token: 0x06000FC1 RID: 4033 RVA: 0x00061F68 File Offset: 0x00060168
			public bool Cull(BoundingFrustum iViewFrustum)
			{
				BoundingSphere boundingSphere = this.mBoundingSphere;
				return boundingSphere.Contains(iViewFrustum) == ContainmentType.Disjoint;
			}

			// Token: 0x06000FC2 RID: 4034 RVA: 0x00061F88 File Offset: 0x00060188
			public virtual void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				SkinnedModelDeferredEffect skinnedModelDeferredEffect = iEffect as SkinnedModelDeferredEffect;
				this.mMaterial.AssignToEffect(skinnedModelDeferredEffect);
				skinnedModelDeferredEffect.Bones = this.mSkeleton;
				skinnedModelDeferredEffect.GraphicsDevice.RenderState.ReferenceStencil = 2;
				skinnedModelDeferredEffect.DiffuseColor = Color.White.ToVector3();
				skinnedModelDeferredEffect.CommitChanges();
				skinnedModelDeferredEffect.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
				skinnedModelDeferredEffect.Bloat = 0f;
				skinnedModelDeferredEffect.GraphicsDevice.RenderState.ReferenceStencil = 0;
				skinnedModelDeferredEffect.CubeMapEnabled = false;
				this.mOwner.mLastDraw = 0f;
				skinnedModelDeferredEffect.Colorize = default(Vector4);
			}

			// Token: 0x06000FC3 RID: 4035 RVA: 0x00062048 File Offset: 0x00060248
			public virtual void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				if (this.mTechnique == SkinnedModelDeferredEffect.Technique.Additive)
				{
					return;
				}
				SkinnedModelDeferredEffect skinnedModelDeferredEffect = iEffect as SkinnedModelDeferredEffect;
				this.mMaterial.AssignOpacityToEffect(skinnedModelDeferredEffect);
				skinnedModelDeferredEffect.Bones = this.mSkeleton;
				skinnedModelDeferredEffect.CommitChanges();
				skinnedModelDeferredEffect.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
				skinnedModelDeferredEffect.Bloat = 0f;
			}

			// Token: 0x06000FC4 RID: 4036 RVA: 0x000620B4 File Offset: 0x000602B4
			public void SetMeshDirty()
			{
				this.mMeshDirty = true;
			}

			// Token: 0x06000FC5 RID: 4037 RVA: 0x000620C0 File Offset: 0x000602C0
			public void SetMesh(VertexBuffer iVertices, IndexBuffer iIndices, ModelMeshPart iMeshPart, ref SkinnedModelDeferredBasicMaterial iBasicMaterial, SkinnedModelDeferredEffect.Technique iTechnique)
			{
				this.mMeshDirty = false;
				this.mMaterial.CopyFrom(ref iBasicMaterial);
				this.mVertexBuffer = iVertices;
				this.mVerticesHash = iVertices.GetHashCode();
				this.mIndexBuffer = iIndices;
				this.mVertexDeclaration = iMeshPart.VertexDeclaration;
				this.mBaseVertex = iMeshPart.BaseVertex;
				this.mNumVertices = iMeshPart.NumVertices;
				this.mPrimitiveCount = iMeshPart.PrimitiveCount;
				this.mStartIndex = iMeshPart.StartIndex;
				this.mStreamOffset = iMeshPart.StreamOffset;
				this.mVertexStride = iMeshPart.VertexStride;
				this.mTechnique = iTechnique;
			}

			// Token: 0x04000E33 RID: 3635
			public BoundingSphere mBoundingSphere;

			// Token: 0x04000E34 RID: 3636
			public VertexDeclaration mVertexDeclaration;

			// Token: 0x04000E35 RID: 3637
			public int mBaseVertex;

			// Token: 0x04000E36 RID: 3638
			public int mNumVertices;

			// Token: 0x04000E37 RID: 3639
			public int mPrimitiveCount;

			// Token: 0x04000E38 RID: 3640
			public int mStartIndex;

			// Token: 0x04000E39 RID: 3641
			public int mStreamOffset;

			// Token: 0x04000E3A RID: 3642
			public int mVertexStride;

			// Token: 0x04000E3B RID: 3643
			public VertexBuffer mVertexBuffer;

			// Token: 0x04000E3C RID: 3644
			public IndexBuffer mIndexBuffer;

			// Token: 0x04000E3D RID: 3645
			public Matrix[] mSkeleton;

			// Token: 0x04000E3E RID: 3646
			public SkinnedModelDeferredAdvancedMaterial mMaterial;

			// Token: 0x04000E3F RID: 3647
			public SkinnedModelDeferredEffect.Technique mTechnique;

			// Token: 0x04000E40 RID: 3648
			public int mVerticesHash;

			// Token: 0x04000E41 RID: 3649
			public bool mMeshDirty = true;

			// Token: 0x04000E42 RID: 3650
			private AnimatedPhysicsEntity mOwner;
		}
	}
}
