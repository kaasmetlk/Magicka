using System;
using System.Collections.Generic;
using System.IO;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Math;
using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.GameStates;
using Magicka.Graphics;
using Magicka.Levels;
using Magicka.Network;
using Magicka.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;

namespace Magicka.GameLogic.Entities
{
	// Token: 0x0200015B RID: 347
	public class PhysicsEntity : Entity
	{
		// Token: 0x06000A4E RID: 2638 RVA: 0x0003E6DC File Offset: 0x0003C8DC
		public PhysicsEntity(PlayState iPlayState) : base(iPlayState)
		{
			this.mRenderData = new PhysicsEntity.RenderData[3];
			this.mHighlightRenderData = new PhysicsEntity.HighlightRenderData[3];
			for (int i = 0; i < 3; i++)
			{
				this.mRenderData[i] = new PhysicsEntity.RenderData();
				this.mHighlightRenderData[i] = new PhysicsEntity.HighlightRenderData();
			}
		}

		// Token: 0x06000A4F RID: 2639 RVA: 0x0003E754 File Offset: 0x0003C954
		protected virtual void CreateBody()
		{
			this.mBody = new PhysicsObjectBody();
			this.mCollision = new CollisionSkin(this.mBody);
			this.mCollision.AddPrimitive(new Box(new Vector3(0.5f), Matrix.Identity, Vector3.One), 1, new MaterialProperties(0f, 1f, 1f));
			this.mBody.CollisionSkin = this.mCollision;
			Vector3 vector = base.SetMass(100f);
			Transform transform = default(Transform);
			Vector3.Negate(ref vector, out transform.Position);
			transform.Orientation = Matrix.Identity;
			this.mCollision.ApplyLocalTransform(transform);
			this.mCollision.callbackFn += this.OnCollision;
			this.mCollision.postCollisionCallbackFn += this.PostCollision;
			this.mBody.Immovable = false;
			this.mBody.AllowFreezing = true;
			this.mBody.ApplyGravity = true;
			this.mBody.Tag = this;
		}

		// Token: 0x06000A50 RID: 2640 RVA: 0x0003E860 File Offset: 0x0003CA60
		protected void PostCollision(ref CollisionInfo iInfo)
		{
			if (this.mStatic)
			{
				if (iInfo.SkinInfo.Skin0 == this.mCollision)
				{
					iInfo.SkinInfo.IgnoreSkin0 = true;
					return;
				}
				iInfo.SkinInfo.IgnoreSkin1 = true;
			}
		}

		// Token: 0x06000A51 RID: 2641 RVA: 0x0003E89C File Offset: 0x0003CA9C
		public bool OnCollision(CollisionSkin iSkin0, int iPrim0, CollisionSkin iSkin1, int iPrim1)
		{
			if (iSkin1.Owner != null)
			{
				Entity entity = iSkin1.Owner.Tag as Entity;
				if (entity != null)
				{
					if (!this.mHitList.ContainsKey(entity.Handle))
					{
						this.mHitList.Add(entity.Handle);
						EventCondition eventCondition = default(EventCondition);
						eventCondition.EventConditionType = (EventConditionType.Hit | EventConditionType.Collision);
						DamageResult damageResult;
						this.mConditions.ExecuteAll(this, entity, ref eventCondition, out damageResult);
					}
					return this.mSolid;
				}
			}
			return true;
		}

		// Token: 0x06000A52 RID: 2642 RVA: 0x0003E913 File Offset: 0x0003CB13
		protected override void AddImpulseVelocity(ref Vector3 iVelocity)
		{
			if (!this.mStatic)
			{
				base.AddImpulseVelocity(ref iVelocity);
			}
		}

		// Token: 0x06000A53 RID: 2643 RVA: 0x0003E924 File Offset: 0x0003CB24
		public virtual void Initialize(PhysicsEntityTemplate iTemplate, Matrix iStartTransform, int iUniqueID)
		{
			this.mTemplate = iTemplate;
			this.CreateBody();
			for (int i = 0; i < 3; i++)
			{
				PhysicsEntity.RenderData renderData = this.mRenderData[i];
				renderData.mVertexCount = iTemplate.VertexCount;
				renderData.mVertexStride = iTemplate.VertexStride;
				renderData.mPrimitiveCount = iTemplate.PrimitiveCount;
				renderData.mVertices = iTemplate.Vertices;
				if (renderData.mVertices != null)
				{
					renderData.mVerticesHash = renderData.mVertices.GetHashCode();
				}
				else
				{
					renderData.mVerticesHash = 0;
				}
				renderData.mVertexDeclaration = iTemplate.VertexDeclaration;
				renderData.mIndices = iTemplate.Indices;
				renderData.mEffect = RenderDeferredEffect.TYPEHASH;
				renderData.mMaterial = iTemplate.Material;
				if (iTemplate.Material.ReflectionMap != null)
				{
					if (iTemplate.Material.DiffuseTexture1 != null)
					{
						renderData.mTechnique = RenderDeferredEffect.Technique.DualLayerReflection;
					}
					else
					{
						renderData.mTechnique = RenderDeferredEffect.Technique.SingleLayerReflection;
					}
				}
				else if (iTemplate.Material.DiffuseTexture1 != null)
				{
					renderData.mTechnique = RenderDeferredEffect.Technique.DualLayer;
				}
				else
				{
					renderData.mTechnique = RenderDeferredEffect.Technique.SingleLayer;
				}
				PhysicsEntity.HighlightRenderData highlightRenderData = this.mHighlightRenderData[i];
				highlightRenderData.SetMesh(iTemplate.Vertices, iTemplate.Indices, iTemplate.VertexDeclaration, iTemplate.VertexCount, iTemplate.PrimitiveCount, iTemplate.VertexStride, RenderDeferredEffect.TYPEHASH, iTemplate.Material);
			}
			this.mHighlighted = -1f;
			this.mConditions = iTemplate.Conditions;
			this.mHitEffect = iTemplate.HitEffect;
			this.mHitSound = iTemplate.HitSound;
			this.mGibTrailEffect = iTemplate.GibTrailEffect;
			this.mEffects = iTemplate.Effects;
			this.mLiveEffects.Clear();
			EffectManager instance = EffectManager.Instance;
			for (int j = 0; j < this.mEffects.Length; j++)
			{
				Matrix matrix;
				Matrix.Multiply(ref iStartTransform, ref this.mEffects[j].Transform, out matrix);
				VisualEffectReference item;
				instance.StartEffect(this.mEffects[j].EffectHash, ref matrix, out item);
				this.mLiveEffects.Add(item);
			}
			this.mPushable = iTemplate.Pushable;
			this.mBody.ApplyGravity = iTemplate.Movable;
			this.mStatic = !iTemplate.Movable;
			this.mSolid = iTemplate.Solid;
			this.CorrectWithTemplateBox();
			Vector3 translation = iStartTransform.Translation;
			iStartTransform.Translation = default(Vector3);
			base.SetMass(iTemplate.Mass);
			this.mBody.MoveTo(translation, iStartTransform);
			this.mBody.EnableBody();
			this.mBody.SetInactive();
			base.Initialize(iUniqueID);
		}

		// Token: 0x06000A54 RID: 2644 RVA: 0x0003EBA8 File Offset: 0x0003CDA8
		protected virtual void CorrectWithTemplateBox()
		{
			PhysicsEntityTemplate.BoxInfo box = this.mTemplate.Box;
			Matrix orientation;
			Matrix.CreateFromQuaternion(ref box.Orientation, out orientation);
			(this.mBody as PhysicsObjectBody).ReactToCharacters = this.mTemplate.Pushable;
			(this.mCollision.GetPrimitiveLocal(0) as Box).Position = box.Positon;
			(this.mCollision.GetPrimitiveNewWorld(0) as Box).Position = box.Positon;
			(this.mCollision.GetPrimitiveOldWorld(0) as Box).Position = box.Positon;
			(this.mCollision.GetPrimitiveLocal(0) as Box).Orientation = orientation;
			(this.mCollision.GetPrimitiveNewWorld(0) as Box).Orientation = orientation;
			(this.mCollision.GetPrimitiveOldWorld(0) as Box).Orientation = orientation;
			(this.mCollision.GetPrimitiveLocal(0) as Box).SideLengths = box.Sides;
			(this.mCollision.GetPrimitiveNewWorld(0) as Box).SideLengths = box.Sides;
			(this.mCollision.GetPrimitiveOldWorld(0) as Box).SideLengths = box.Sides;
			this.mRadius = box.Sides.Length() * 0.5f;
		}

		// Token: 0x06000A55 RID: 2645 RVA: 0x0003ECF8 File Offset: 0x0003CEF8
		internal void OnSpawn()
		{
			if (!this.mStatic)
			{
				this.mBody.AllowFreezing = true;
				return;
			}
			Segment iSeg = default(Segment);
			iSeg.Origin = this.mBody.Position;
			iSeg.Delta.Y = -4f;
			float num;
			Vector3 vector;
			Vector3 vector2;
			AnimatedLevelPart animatedLevelPart;
			if (base.PlayState.Level.CurrentScene.SegmentIntersect(out num, out vector, out vector2, out animatedLevelPart, iSeg) && animatedLevelPart != null)
			{
				animatedLevelPart.AddEntity(this);
				this.mBody.AllowFreezing = false;
				return;
			}
			this.mBody.AllowFreezing = true;
		}

		// Token: 0x06000A56 RID: 2646 RVA: 0x0003ED88 File Offset: 0x0003CF88
		public override void Deinitialize()
		{
			base.Deinitialize();
			EffectManager instance = EffectManager.Instance;
			for (int i = 0; i < this.mLiveEffects.Count; i++)
			{
				VisualEffectReference visualEffectReference = this.mLiveEffects[i];
				instance.Stop(ref visualEffectReference);
			}
			this.mLiveEffects.Clear();
		}

		// Token: 0x06000A57 RID: 2647 RVA: 0x0003EDD8 File Offset: 0x0003CFD8
		public override Matrix GetOrientation()
		{
			Matrix orientation = this.mBody.Orientation;
			orientation.Translation = this.mBody.Position;
			return orientation;
		}

		// Token: 0x06000A58 RID: 2648 RVA: 0x0003EE04 File Offset: 0x0003D004
		public override void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			this.mHitList.Update(iDeltaTime);
			this.UpdateRenderData(iDataChannel);
			if (this.mBody.Velocity.LengthSquared() > 1E-06f)
			{
				this.mRestingTimer = 1f;
			}
			else
			{
				this.mRestingTimer -= iDeltaTime;
			}
			Matrix transformMatrix = this.mBody.TransformMatrix;
			EffectManager instance = EffectManager.Instance;
			for (int i = 0; i < this.mLiveEffects.Count; i++)
			{
				Matrix matrix;
				Matrix.Multiply(ref transformMatrix, ref this.mEffects[i].Transform, out matrix);
				VisualEffectReference value = this.mLiveEffects[i];
				instance.UpdateOrientation(ref value, ref matrix);
				if (value.ID < 0)
				{
					instance.StartEffect(this.mEffects[i].EffectHash, ref matrix, out value);
				}
				this.mLiveEffects[i] = value;
			}
			base.Update(iDataChannel, iDeltaTime);
			this.mHighlighted -= iDeltaTime;
		}

		// Token: 0x06000A59 RID: 2649 RVA: 0x0003EF00 File Offset: 0x0003D100
		protected virtual void UpdateRenderData(DataChannel iDataChannel)
		{
			PhysicsEntity.RenderData renderData = this.mRenderData[(int)iDataChannel];
			if (renderData.mPrimitiveCount > 0)
			{
				renderData.mTransform = this.GetOrientation();
				renderData.mBoundingSphere.Center = renderData.mTransform.Translation;
				renderData.mBoundingSphere.Radius = this.mRadius;
				this.mPlayState.Scene.AddRenderableObject(iDataChannel, renderData);
				if (this.mHighlighted >= 0f)
				{
					PhysicsEntity.HighlightRenderData highlightRenderData = this.mHighlightRenderData[(int)iDataChannel];
					highlightRenderData.mTransform = renderData.mTransform;
					highlightRenderData.mBoundingSphere = renderData.mBoundingSphere;
					this.mPlayState.Scene.AddRenderableAdditiveObject(iDataChannel, highlightRenderData);
				}
			}
		}

		// Token: 0x1700022F RID: 559
		// (get) Token: 0x06000A5A RID: 2650 RVA: 0x0003EFA7 File Offset: 0x0003D1A7
		public int HitEffect
		{
			get
			{
				return this.mHitEffect;
			}
		}

		// Token: 0x17000230 RID: 560
		// (get) Token: 0x06000A5B RID: 2651 RVA: 0x0003EFAF File Offset: 0x0003D1AF
		public bool Resting
		{
			get
			{
				return this.mRestingTimer < 0f;
			}
		}

		// Token: 0x17000231 RID: 561
		// (get) Token: 0x06000A5C RID: 2652 RVA: 0x0003EFBE File Offset: 0x0003D1BE
		public override bool Dead
		{
			get
			{
				return this.mDead;
			}
		}

		// Token: 0x17000232 RID: 562
		// (get) Token: 0x06000A5D RID: 2653 RVA: 0x0003EFC6 File Offset: 0x0003D1C6
		public override bool Removable
		{
			get
			{
				return this.Dead;
			}
		}

		// Token: 0x06000A5E RID: 2654 RVA: 0x0003EFCE File Offset: 0x0003D1CE
		public void Highlight(float iTTL)
		{
			this.mHighlighted = iTTL;
		}

		// Token: 0x17000233 RID: 563
		// (get) Token: 0x06000A5F RID: 2655 RVA: 0x0003EFD7 File Offset: 0x0003D1D7
		public PhysicsObjectBody PhysicsObjectBody
		{
			get
			{
				return this.mBody as PhysicsObjectBody;
			}
		}

		// Token: 0x06000A60 RID: 2656 RVA: 0x0003EFE4 File Offset: 0x0003D1E4
		public override void Kill()
		{
			this.mDead = true;
		}

		// Token: 0x06000A61 RID: 2657 RVA: 0x0003EFF0 File Offset: 0x0003D1F0
		protected Vector3 GetRandomPositionOnCollisionSkin()
		{
			Vector3 position = this.Position;
			if (this.mCollision == null)
			{
				return position;
			}
			float num = (float)MagickaMath.Random.NextDouble() - 0.5f;
			float num2 = (float)MagickaMath.Random.NextDouble() - 0.5f;
			float num3 = (float)MagickaMath.Random.NextDouble() - 0.5f;
			Vector3 sideLengths = (this.mCollision.GetPrimitiveLocal(0) as Box).SideLengths;
			if (num > 0f)
			{
				position.X = position.X + sideLengths.X * 0.25f + sideLengths.X * 0.25f * num;
			}
			else
			{
				position.X = position.X - sideLengths.X * 0.25f - sideLengths.X * 0.25f * num;
			}
			if (num2 > 0f)
			{
				position.Y = position.Y + sideLengths.Y * 0.25f + sideLengths.Y * 0.25f * num2;
			}
			else
			{
				position.Y = position.Y - sideLengths.Y * 0.25f - sideLengths.Y * 0.25f * num2;
			}
			if (num3 > 0f)
			{
				position.Z = position.Z + sideLengths.Z * 0.25f + sideLengths.Z * 0.25f * num3;
			}
			else
			{
				position.Z = position.Z - sideLengths.Z * 0.25f - sideLengths.Z * 0.25f * num3;
			}
			return position;
		}

		// Token: 0x06000A62 RID: 2658 RVA: 0x0003F184 File Offset: 0x0003D384
		protected override void IGetNetworkUpdate(out EntityUpdateMessage oMsg, float iPrediction)
		{
			oMsg = default(EntityUpdateMessage);
			if (!this.Resting & !this.mBody.Immovable)
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
		}

		// Token: 0x0400095B RID: 2395
		protected PhysicsEntity.RenderData[] mRenderData;

		// Token: 0x0400095C RID: 2396
		protected PhysicsEntity.HighlightRenderData[] mHighlightRenderData;

		// Token: 0x0400095D RID: 2397
		protected float mHighlighted;

		// Token: 0x0400095E RID: 2398
		protected bool mPushable;

		// Token: 0x0400095F RID: 2399
		protected bool mSolid;

		// Token: 0x04000960 RID: 2400
		protected ConditionCollection mConditions;

		// Token: 0x04000961 RID: 2401
		protected int mHitEffect;

		// Token: 0x04000962 RID: 2402
		protected int mHitSound;

		// Token: 0x04000963 RID: 2403
		protected int mGibTrailEffect;

		// Token: 0x04000964 RID: 2404
		protected HitList mHitList = new HitList(32);

		// Token: 0x04000965 RID: 2405
		protected PhysicsEntity.VisualEffectStorage[] mEffects;

		// Token: 0x04000966 RID: 2406
		protected List<VisualEffectReference> mLiveEffects = new List<VisualEffectReference>(4);

		// Token: 0x04000967 RID: 2407
		protected float mRestingTimer = 1f;

		// Token: 0x04000968 RID: 2408
		protected PhysicsEntityTemplate mTemplate;

		// Token: 0x04000969 RID: 2409
		protected bool mStatic;

		// Token: 0x0200015C RID: 348
		public struct State
		{
			// Token: 0x06000A63 RID: 2659 RVA: 0x0003F23C File Offset: 0x0003D43C
			public State(BinaryReader iReader)
			{
				this.mEntity = null;
				this.mTemplate = iReader.ReadString();
				this.mPosition.X = iReader.ReadSingle();
				this.mPosition.Y = iReader.ReadSingle();
				this.mPosition.Z = iReader.ReadSingle();
				this.mOrientation.X = iReader.ReadSingle();
				this.mOrientation.Y = iReader.ReadSingle();
				this.mOrientation.Z = iReader.ReadSingle();
				this.mOrientation.W = iReader.ReadSingle();
				this.mUniqueID = iReader.ReadInt32();
				this.mOnDamage = iReader.ReadInt32();
				this.mOnDeath = iReader.ReadInt32();
			}

			// Token: 0x06000A64 RID: 2660 RVA: 0x0003F2F8 File Offset: 0x0003D4F8
			public State(PhysicsEntity iPhysicsEntity)
			{
				this.mEntity = iPhysicsEntity;
				this.mTemplate = iPhysicsEntity.mTemplate.Path;
				Vector3 vector;
				iPhysicsEntity.Body.TransformMatrix.Decompose(out vector, out this.mOrientation, out this.mPosition);
				this.mUniqueID = iPhysicsEntity.UniqueID;
				DamageablePhysicsEntity damageablePhysicsEntity = iPhysicsEntity as DamageablePhysicsEntity;
				if (damageablePhysicsEntity != null)
				{
					this.mOnDamage = damageablePhysicsEntity.OnDamage;
					this.mOnDeath = damageablePhysicsEntity.OnDeath;
					return;
				}
				this.mOnDamage = 0;
				this.mOnDeath = 0;
			}

			// Token: 0x06000A65 RID: 2661 RVA: 0x0003F37C File Offset: 0x0003D57C
			public PhysicsEntity ApplyTo(PlayState iPlayState)
			{
				PhysicsEntityTemplate physicsEntityTemplate = iPlayState.Content.Load<PhysicsEntityTemplate>(this.mTemplate);
				if (this.mEntity == null)
				{
					if ((float)physicsEntityTemplate.MaxHitpoints > 0f)
					{
						this.mEntity = new DamageablePhysicsEntity(iPlayState)
						{
							OnDamage = this.mOnDamage,
							OnDeath = this.mOnDeath
						};
					}
					else
					{
						this.mEntity = new PhysicsEntity(iPlayState);
					}
				}
				Matrix iStartTransform;
				Matrix.CreateFromQuaternion(ref this.mOrientation, out iStartTransform);
				iStartTransform.Translation = this.mPosition;
				this.mEntity.Initialize(physicsEntityTemplate, iStartTransform, this.mUniqueID);
				return this.mEntity;
			}

			// Token: 0x06000A66 RID: 2662 RVA: 0x0003F418 File Offset: 0x0003D618
			public void Write(BinaryWriter iWriter)
			{
				iWriter.Write(this.mTemplate);
				iWriter.Write(this.mPosition.X);
				iWriter.Write(this.mPosition.Y);
				iWriter.Write(this.mPosition.Z);
				iWriter.Write(this.mOrientation.X);
				iWriter.Write(this.mOrientation.Y);
				iWriter.Write(this.mOrientation.Z);
				iWriter.Write(this.mOrientation.W);
				iWriter.Write(this.mUniqueID);
				iWriter.Write(this.mOnDamage);
				iWriter.Write(this.mOnDeath);
			}

			// Token: 0x0400096A RID: 2410
			private PhysicsEntity mEntity;

			// Token: 0x0400096B RID: 2411
			private string mTemplate;

			// Token: 0x0400096C RID: 2412
			private Vector3 mPosition;

			// Token: 0x0400096D RID: 2413
			private Quaternion mOrientation;

			// Token: 0x0400096E RID: 2414
			private int mUniqueID;

			// Token: 0x0400096F RID: 2415
			private int mOnDeath;

			// Token: 0x04000970 RID: 2416
			private int mOnDamage;
		}

		// Token: 0x0200015D RID: 349
		public struct VisualEffectStorage
		{
			// Token: 0x04000971 RID: 2417
			public Matrix Transform;

			// Token: 0x04000972 RID: 2418
			public int EffectHash;
		}

		// Token: 0x0200015E RID: 350
		protected class RenderData : IRenderableObject
		{
			// Token: 0x17000234 RID: 564
			// (get) Token: 0x06000A67 RID: 2663 RVA: 0x0003F4CC File Offset: 0x0003D6CC
			public int Effect
			{
				get
				{
					return this.mEffect;
				}
			}

			// Token: 0x17000235 RID: 565
			// (get) Token: 0x06000A68 RID: 2664 RVA: 0x0003F4D4 File Offset: 0x0003D6D4
			public int DepthTechnique
			{
				get
				{
					return 4;
				}
			}

			// Token: 0x17000236 RID: 566
			// (get) Token: 0x06000A69 RID: 2665 RVA: 0x0003F4D7 File Offset: 0x0003D6D7
			public int Technique
			{
				get
				{
					return (int)this.mTechnique;
				}
			}

			// Token: 0x17000237 RID: 567
			// (get) Token: 0x06000A6A RID: 2666 RVA: 0x0003F4DF File Offset: 0x0003D6DF
			public int ShadowTechnique
			{
				get
				{
					return 5;
				}
			}

			// Token: 0x17000238 RID: 568
			// (get) Token: 0x06000A6B RID: 2667 RVA: 0x0003F4E2 File Offset: 0x0003D6E2
			public VertexBuffer Vertices
			{
				get
				{
					return this.mVertices;
				}
			}

			// Token: 0x17000239 RID: 569
			// (get) Token: 0x06000A6C RID: 2668 RVA: 0x0003F4EA File Offset: 0x0003D6EA
			public IndexBuffer Indices
			{
				get
				{
					return this.mIndices;
				}
			}

			// Token: 0x1700023A RID: 570
			// (get) Token: 0x06000A6D RID: 2669 RVA: 0x0003F4F2 File Offset: 0x0003D6F2
			public VertexDeclaration VertexDeclaration
			{
				get
				{
					return this.mVertexDeclaration;
				}
			}

			// Token: 0x1700023B RID: 571
			// (get) Token: 0x06000A6E RID: 2670 RVA: 0x0003F4FA File Offset: 0x0003D6FA
			public int VertexStride
			{
				get
				{
					return this.mVertexStride;
				}
			}

			// Token: 0x1700023C RID: 572
			// (get) Token: 0x06000A6F RID: 2671 RVA: 0x0003F502 File Offset: 0x0003D702
			public Texture2D Texture
			{
				get
				{
					return this.mMaterial.DiffuseTexture0;
				}
			}

			// Token: 0x1700023D RID: 573
			// (get) Token: 0x06000A70 RID: 2672 RVA: 0x0003F50F File Offset: 0x0003D70F
			public int VerticesHashCode
			{
				get
				{
					return this.mVerticesHash;
				}
			}

			// Token: 0x06000A71 RID: 2673 RVA: 0x0003F518 File Offset: 0x0003D718
			public bool Cull(BoundingFrustum iViewFrustum)
			{
				BoundingSphere boundingSphere = this.mBoundingSphere;
				return boundingSphere.Contains(iViewFrustum) == ContainmentType.Disjoint;
			}

			// Token: 0x06000A72 RID: 2674 RVA: 0x0003F538 File Offset: 0x0003D738
			public void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				RenderDeferredEffect renderDeferredEffect = iEffect as RenderDeferredEffect;
				this.mMaterial.AssignToEffect(renderDeferredEffect);
				renderDeferredEffect.World = this.mTransform;
				renderDeferredEffect.CommitChanges();
				renderDeferredEffect.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, 0, 0, this.mVertexCount, 0, this.mPrimitiveCount);
			}

			// Token: 0x06000A73 RID: 2675 RVA: 0x0003F588 File Offset: 0x0003D788
			public void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				RenderDeferredEffect renderDeferredEffect = iEffect as RenderDeferredEffect;
				this.mMaterial.AssignOpacityToEffect(renderDeferredEffect);
				renderDeferredEffect.World = this.mTransform;
				renderDeferredEffect.CommitChanges();
				renderDeferredEffect.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, 0, 0, this.mVertexCount, 0, this.mPrimitiveCount);
			}

			// Token: 0x04000973 RID: 2419
			public RenderDeferredMaterial mMaterial;

			// Token: 0x04000974 RID: 2420
			public int mEffect;

			// Token: 0x04000975 RID: 2421
			public BoundingSphere mBoundingSphere;

			// Token: 0x04000976 RID: 2422
			public Matrix mTransform;

			// Token: 0x04000977 RID: 2423
			public int mVertexCount;

			// Token: 0x04000978 RID: 2424
			public int mVertexStride;

			// Token: 0x04000979 RID: 2425
			public int mPrimitiveCount;

			// Token: 0x0400097A RID: 2426
			public VertexBuffer mVertices;

			// Token: 0x0400097B RID: 2427
			public VertexDeclaration mVertexDeclaration;

			// Token: 0x0400097C RID: 2428
			public IndexBuffer mIndices;

			// Token: 0x0400097D RID: 2429
			public RenderDeferredEffect.Technique mTechnique;

			// Token: 0x0400097E RID: 2430
			public int mVerticesHash;
		}

		// Token: 0x0200015F RID: 351
		protected class HighlightRenderData : IRenderableAdditiveObject
		{
			// Token: 0x1700023E RID: 574
			// (get) Token: 0x06000A75 RID: 2677 RVA: 0x0003F5DD File Offset: 0x0003D7DD
			public int Effect
			{
				get
				{
					return this.mEffect;
				}
			}

			// Token: 0x1700023F RID: 575
			// (get) Token: 0x06000A76 RID: 2678 RVA: 0x0003F5E5 File Offset: 0x0003D7E5
			public int Technique
			{
				get
				{
					return 6;
				}
			}

			// Token: 0x17000240 RID: 576
			// (get) Token: 0x06000A77 RID: 2679 RVA: 0x0003F5E8 File Offset: 0x0003D7E8
			public VertexBuffer Vertices
			{
				get
				{
					return this.mVertexBuffer;
				}
			}

			// Token: 0x17000241 RID: 577
			// (get) Token: 0x06000A78 RID: 2680 RVA: 0x0003F5F0 File Offset: 0x0003D7F0
			public int VerticesHashCode
			{
				get
				{
					return this.mVerticesHash;
				}
			}

			// Token: 0x17000242 RID: 578
			// (get) Token: 0x06000A79 RID: 2681 RVA: 0x0003F5F8 File Offset: 0x0003D7F8
			public int VertexStride
			{
				get
				{
					return this.mVertexStride;
				}
			}

			// Token: 0x17000243 RID: 579
			// (get) Token: 0x06000A7A RID: 2682 RVA: 0x0003F600 File Offset: 0x0003D800
			public IndexBuffer Indices
			{
				get
				{
					return this.mIndexBuffer;
				}
			}

			// Token: 0x17000244 RID: 580
			// (get) Token: 0x06000A7B RID: 2683 RVA: 0x0003F608 File Offset: 0x0003D808
			public VertexDeclaration VertexDeclaration
			{
				get
				{
					return this.mVertexDeclaration;
				}
			}

			// Token: 0x06000A7C RID: 2684 RVA: 0x0003F610 File Offset: 0x0003D810
			public bool Cull(BoundingFrustum iViewFrustum)
			{
				BoundingSphere boundingSphere = this.mBoundingSphere;
				return boundingSphere.Contains(iViewFrustum) == ContainmentType.Disjoint;
			}

			// Token: 0x06000A7D RID: 2685 RVA: 0x0003F630 File Offset: 0x0003D830
			public void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				RenderDeferredEffect renderDeferredEffect = iEffect as RenderDeferredEffect;
				this.mMaterial.AssignToEffect(renderDeferredEffect);
				renderDeferredEffect.DiffuseColor0 = new Vector3(1f);
				renderDeferredEffect.FresnelPower = 2f;
				renderDeferredEffect.World = this.mTransform;
				renderDeferredEffect.CommitChanges();
				renderDeferredEffect.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, 0, 0, this.mNumVertices, 0, this.mPrimitiveCount);
				renderDeferredEffect.DiffuseColor0 = Vector3.One;
			}

			// Token: 0x17000245 RID: 581
			// (get) Token: 0x06000A7E RID: 2686 RVA: 0x0003F6A3 File Offset: 0x0003D8A3
			public bool MeshDirty
			{
				get
				{
					return this.mMeshDirty;
				}
			}

			// Token: 0x06000A7F RID: 2687 RVA: 0x0003F6AB File Offset: 0x0003D8AB
			public void SetMeshDirty()
			{
				this.mMeshDirty = true;
			}

			// Token: 0x06000A80 RID: 2688 RVA: 0x0003F6B4 File Offset: 0x0003D8B4
			public void SetMesh(VertexBuffer iVertices, IndexBuffer iIndices, VertexDeclaration iVertexDeclaration, int iNumVertices, int iPrimitiveCount, int iVertexStride, int iEffectHash, RenderDeferredMaterial iMaterial)
			{
				this.mMeshDirty = false;
				this.mVertexBuffer = iVertices;
				if (iVertices != null)
				{
					this.mVerticesHash = iVertices.GetHashCode();
				}
				else
				{
					this.mVerticesHash = 0;
				}
				this.mIndexBuffer = iIndices;
				this.mEffect = iEffectHash;
				this.mVertexDeclaration = iVertexDeclaration;
				this.mNumVertices = iNumVertices;
				this.mPrimitiveCount = iPrimitiveCount;
				this.mVertexStride = iVertexStride;
				this.mMaterial = iMaterial;
			}

			// Token: 0x0400097F RID: 2431
			protected VertexDeclaration mVertexDeclaration;

			// Token: 0x04000980 RID: 2432
			protected int mNumVertices;

			// Token: 0x04000981 RID: 2433
			protected int mPrimitiveCount;

			// Token: 0x04000982 RID: 2434
			protected int mVertexStride;

			// Token: 0x04000983 RID: 2435
			protected VertexBuffer mVertexBuffer;

			// Token: 0x04000984 RID: 2436
			protected int mVerticesHash;

			// Token: 0x04000985 RID: 2437
			protected IndexBuffer mIndexBuffer;

			// Token: 0x04000986 RID: 2438
			protected int mEffect;

			// Token: 0x04000987 RID: 2439
			protected RenderDeferredMaterial mMaterial;

			// Token: 0x04000988 RID: 2440
			public BoundingSphere mBoundingSphere;

			// Token: 0x04000989 RID: 2441
			public Matrix mTransform;

			// Token: 0x0400098A RID: 2442
			protected bool mMeshDirty = true;
		}
	}
}
