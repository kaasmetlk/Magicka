using System;
using System.Collections.Generic;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Math;
using JigLibX.Physics;
using Magicka.GameLogic.GameStates;
using Magicka.Graphics;
using Magicka.Levels;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;

namespace Magicka.GameLogic.Entities
{
	// Token: 0x020002A5 RID: 677
	public class Gib : Entity
	{
		// Token: 0x0600145D RID: 5213 RVA: 0x0007EC50 File Offset: 0x0007CE50
		public static void InitializeCache(int size, PlayState p)
		{
			Gib.GibCache = new List<Gib>(size);
			for (int i = 0; i < size; i++)
			{
				Gib.GibCache.Add(new Gib(p));
			}
		}

		// Token: 0x0600145E RID: 5214 RVA: 0x0007EC84 File Offset: 0x0007CE84
		public static Gib GetFromCache()
		{
			if (Gib.GibCache.Count <= 0)
			{
				return null;
			}
			Gib result = Gib.GibCache[Gib.GibCache.Count - 1];
			Gib.GibCache.RemoveAt(Gib.GibCache.Count - 1);
			return result;
		}

		// Token: 0x0600145F RID: 5215 RVA: 0x0007ECCE File Offset: 0x0007CECE
		private static void ReturnGib(Gib iGib)
		{
			if (Gib.GibCache.Contains(iGib))
			{
				throw new Exception("FAIL");
			}
			Gib.GibCache.Add(iGib);
		}

		// Token: 0x06001460 RID: 5216 RVA: 0x0007ECF4 File Offset: 0x0007CEF4
		public Gib(PlayState iPlayState) : base(iPlayState)
		{
			this.mRenderData = new Gib.RenderData[3];
			for (int i = 0; i < 3; i++)
			{
				this.mRenderData[i] = new Gib.RenderData();
			}
			this.mBody = new Body();
			this.mCollision = new CollisionSkin(this.mBody);
			this.mCollision.AddPrimitive(new Box(new Vector3(0.5f), Matrix.Identity, new Vector3(1f)), 1, new MaterialProperties(0f, 0.8f, 0.8f));
			this.mBody.CollisionSkin = this.mCollision;
			this.mCollision.callbackFn += this.OnCollision;
			this.mBody.Immovable = false;
			this.mBody.Tag = this;
		}

		// Token: 0x06001461 RID: 5217 RVA: 0x0007EDD4 File Offset: 0x0007CFD4
		public void Initialize(Model iModel, float iMass, float iScale, Vector3 iPosition, Vector3 iVelocity, float iTTL, Entity iOwner, BloodType iBloodType, int iTrailEffect, bool iFrozen)
		{
			this.mScale = iScale;
			this.mBloodType = iBloodType;
			this.mFrozen = iFrozen;
			this.Model = iModel;
			this.mBody.Mass = iMass;
			this.mTTL = iTTL;
			this.mRadius = this.mMesh.BoundingSphere.Radius;
			Vector3 sideLengths = new Vector3(this.mMesh.BoundingSphere.Radius);
			Vector3 position = -this.mMesh.BoundingSphere.Center;
			(this.mCollision.GetPrimitiveLocal(0) as Box).SideLengths = sideLengths;
			(this.mCollision.GetPrimitiveLocal(0) as Box).Position = position;
			(this.mCollision.GetPrimitiveNewWorld(0) as Box).SideLengths = sideLengths;
			(this.mCollision.GetPrimitiveNewWorld(0) as Box).Position = position;
			(this.mCollision.GetPrimitiveOldWorld(0) as Box).SideLengths = sideLengths;
			(this.mCollision.GetPrimitiveOldWorld(0) as Box).Position = position;
			Vector3 vector = base.SetMass(iMass);
			Transform transform = default(Transform);
			Vector3.Negate(ref vector, out transform.Position);
			transform.Orientation = Matrix.Identity;
			this.mCollision.ApplyLocalTransform(transform);
			this.mBody.MoveTo(iPosition, Matrix.Identity);
			this.mBody.AllowFreezing = false;
			this.mCollision.NonCollidables.Clear();
			if (iOwner != null)
			{
				this.mCollision.NonCollidables.Add(iOwner.Body.CollisionSkin);
			}
			this.mBody.Velocity = iVelocity;
			this.mBody.EnableBody();
			for (int i = 0; i < 3; i++)
			{
				this.mRenderData[i].mFrozen = this.mFrozen;
			}
			if (!this.mFrozen && iTrailEffect != 0 && iTrailEffect != Gib.GORE_GIB_TRAIL_EFFECTS[5])
			{
				Matrix orientation = this.mBody.Orientation;
				EffectManager.Instance.StartEffect(iTrailEffect, ref orientation, out this.mTrailEffect);
			}
		}

		// Token: 0x17000533 RID: 1331
		// (get) Token: 0x06001462 RID: 5218 RVA: 0x0007EFD9 File Offset: 0x0007D1D9
		// (set) Token: 0x06001463 RID: 5219 RVA: 0x0007EFE4 File Offset: 0x0007D1E4
		public Model Model
		{
			get
			{
				return this.mModel;
			}
			set
			{
				this.mModel = value;
				if (this.mRenderData != null)
				{
					for (int i = 0; i < 3; i++)
					{
						this.mRenderData[i].SetMeshDirty();
					}
				}
				if (this.mModel != null)
				{
					this.mMesh = this.mModel.Meshes[0];
					this.mMeshPart = this.mMesh.MeshParts[0];
					if (this.mModel.Meshes[0].VertexBuffer.IsDisposed)
					{
						throw new Exception("Vertexbuffern is disposed!");
					}
				}
				else
				{
					this.mMesh = null;
					this.mMeshPart = null;
				}
			}
		}

		// Token: 0x17000534 RID: 1332
		// (get) Token: 0x06001464 RID: 5220 RVA: 0x0007F085 File Offset: 0x0007D285
		// (set) Token: 0x06001465 RID: 5221 RVA: 0x0007F092 File Offset: 0x0007D292
		public float Mass
		{
			get
			{
				return this.mBody.Mass;
			}
			set
			{
				this.mBody.Mass = value;
			}
		}

		// Token: 0x17000535 RID: 1333
		// (get) Token: 0x06001466 RID: 5222 RVA: 0x0007F0A0 File Offset: 0x0007D2A0
		// (set) Token: 0x06001467 RID: 5223 RVA: 0x0007F0A8 File Offset: 0x0007D2A8
		public float TTL
		{
			get
			{
				return this.mTTL;
			}
			set
			{
				this.mTTL = value;
			}
		}

		// Token: 0x17000536 RID: 1334
		// (get) Token: 0x06001468 RID: 5224 RVA: 0x0007F0B1 File Offset: 0x0007D2B1
		public override bool Removable
		{
			get
			{
				return this.mTTL <= 0f;
			}
		}

		// Token: 0x06001469 RID: 5225 RVA: 0x0007F0C4 File Offset: 0x0007D2C4
		public override Vector3 CalcImpulseVelocity(Vector3 iDirection, float iElevation, float iMassPower, float iDistance)
		{
			if (!this.mBody.IsBodyEnabled)
			{
				return default(Vector3);
			}
			return base.CalcImpulseVelocity(iDirection, iElevation, iMassPower, iDistance);
		}

		// Token: 0x17000537 RID: 1335
		// (get) Token: 0x0600146A RID: 5226 RVA: 0x0007F0F3 File Offset: 0x0007D2F3
		public override bool Dead
		{
			get
			{
				return this.mTTL < 0f;
			}
		}

		// Token: 0x0600146B RID: 5227 RVA: 0x0007F104 File Offset: 0x0007D304
		private bool OnCollision(CollisionSkin iSkin0, int iPrim0, CollisionSkin iSkin1, int iPrim1)
		{
			if (iSkin1.Owner != null && (iSkin1.Owner.Tag is Character || iSkin1.Owner.Tag is Gib))
			{
				return false;
			}
			if ((!this.mFrozen & this.mBloodType != BloodType.none) && iSkin1.Tag is LevelModel && this.mBody.Velocity.Y < -10f && !EffectManager.Instance.IsActive(ref this.mBloodEffect))
			{
				Vector3 position = this.mBody.Position;
				Vector3 backward = this.mBody.Orientation.Backward;
				EffectManager.Instance.StartEffect(Gib.GORE_GIB_SMALL_EFFECTS[(int)this.mBloodType], ref position, ref backward, out this.mBloodEffect);
			}
			return true;
		}

		// Token: 0x0600146C RID: 5228 RVA: 0x0007F1D0 File Offset: 0x0007D3D0
		public override void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			this.mTTL -= iDeltaTime;
			if (this.mTTL < 1f)
			{
				this.mBody.AllowFreezing = true;
				this.mBody.SetInactive();
				Vector3 position = this.Position;
				position.Y -= iDeltaTime * this.mRadius * 2f;
				this.mBody.Position = position;
				if (this.mTTL <= 0f && this.mBody.IsBodyEnabled && !this.mBody.IsActive)
				{
					this.mBody.DisableBody();
				}
			}
			base.Update(iDataChannel, iDeltaTime);
			if (this.mModel != null)
			{
				Matrix orientation = this.mBody.Orientation;
				orientation.Translation = this.mBody.Position;
				if (!this.mFrozen)
				{
					EffectManager.Instance.UpdateOrientation(ref this.mTrailEffect, ref orientation);
				}
				Gib.RenderData renderData = this.mRenderData[(int)iDataChannel];
				if (renderData.MeshDirty)
				{
					renderData.SetMesh(this.mMesh.VertexBuffer, this.mMesh.IndexBuffer, this.mMeshPart, RenderDeferredEffect.TYPEHASH);
				}
				renderData.mTransform = orientation;
				MagickaMath.UniformMatrixScale(ref renderData.mTransform, this.mScale);
				renderData.mBoundingSphere = this.mMesh.BoundingSphere;
				Vector3.Transform(ref renderData.mBoundingSphere.Center, ref orientation, out renderData.mBoundingSphere.Center);
				this.mPlayState.Scene.AddRenderableObject(iDataChannel, renderData);
			}
		}

		// Token: 0x0600146D RID: 5229 RVA: 0x0007F34C File Offset: 0x0007D54C
		public override void Deinitialize()
		{
			Gib.ReturnGib(this);
			base.Deinitialize();
		}

		// Token: 0x0600146E RID: 5230 RVA: 0x0007F35A File Offset: 0x0007D55A
		public override void Kill()
		{
			this.mTTL = 0f;
		}

		// Token: 0x0600146F RID: 5231 RVA: 0x0007F367 File Offset: 0x0007D567
		protected override void IGetNetworkUpdate(out EntityUpdateMessage oMsg, float iPrediction)
		{
			oMsg = default(EntityUpdateMessage);
		}

		// Token: 0x040015BB RID: 5563
		private static List<Gib> GibCache;

		// Token: 0x040015BC RID: 5564
		public static readonly int[] GORE_GIB_SMALL_EFFECTS = new int[]
		{
			"gore_gib_regular_small".GetHashCodeCustom(),
			"gore_gib_green_small".GetHashCodeCustom(),
			"gore_gib_black_small".GetHashCodeCustom(),
			"gore_gib_wood_small".GetHashCodeCustom(),
			"gore_gib_insect_small".GetHashCodeCustom(),
			"".GetHashCodeCustom()
		};

		// Token: 0x040015BD RID: 5565
		public static readonly int[] GORE_GIB_MEDIUM_EFFECTS = new int[]
		{
			"gore_gib_regular_medium".GetHashCodeCustom(),
			"gore_gib_green_medium".GetHashCodeCustom(),
			"gore_gib_black_medium".GetHashCodeCustom(),
			"gore_gib_wood_medium".GetHashCodeCustom(),
			"gore_gib_insect_medium".GetHashCodeCustom(),
			"".GetHashCodeCustom()
		};

		// Token: 0x040015BE RID: 5566
		public static readonly int[] GORE_GIB_LARGE_EFFECTS = new int[]
		{
			"gore_gib_regular_large".GetHashCodeCustom(),
			"gore_gib_green_large".GetHashCodeCustom(),
			"gore_gib_black_large".GetHashCodeCustom(),
			"gore_gib_wood_large".GetHashCodeCustom(),
			"gore_gib_insect_large".GetHashCodeCustom(),
			"".GetHashCodeCustom()
		};

		// Token: 0x040015BF RID: 5567
		public static readonly int[] GORE_GIB_TRAIL_EFFECTS = new int[]
		{
			"gore_gib_regular_trail".GetHashCodeCustom(),
			"gore_gib_green_trail".GetHashCodeCustom(),
			"gore_gib_black_trail".GetHashCodeCustom(),
			"gore_gib_wood_trail".GetHashCodeCustom(),
			"gore_gib_insect_trail".GetHashCodeCustom(),
			"".GetHashCodeCustom()
		};

		// Token: 0x040015C0 RID: 5568
		public static readonly int[] GORE_SPLASH_EFFECTS = new int[]
		{
			"gore_splash_regular".GetHashCodeCustom(),
			"gore_splash_green".GetHashCodeCustom(),
			"gore_splash_black".GetHashCodeCustom(),
			"gore_splash_wood".GetHashCodeCustom(),
			"gore_splash_insect".GetHashCodeCustom(),
			"".GetHashCodeCustom()
		};

		// Token: 0x040015C1 RID: 5569
		private Gib.RenderData[] mRenderData;

		// Token: 0x040015C2 RID: 5570
		private Model mModel;

		// Token: 0x040015C3 RID: 5571
		private ModelMesh mMesh;

		// Token: 0x040015C4 RID: 5572
		private ModelMeshPart mMeshPart;

		// Token: 0x040015C5 RID: 5573
		private float mTTL;

		// Token: 0x040015C6 RID: 5574
		private float mScale = 1f;

		// Token: 0x040015C7 RID: 5575
		private BloodType mBloodType;

		// Token: 0x040015C8 RID: 5576
		private bool mFrozen;

		// Token: 0x040015C9 RID: 5577
		private VisualEffectReference mBloodEffect;

		// Token: 0x040015CA RID: 5578
		private VisualEffectReference mTrailEffect;

		// Token: 0x020002A6 RID: 678
		protected class RenderData : IRenderableObject
		{
			// Token: 0x06001471 RID: 5233 RVA: 0x0007F54C File Offset: 0x0007D74C
			public RenderData()
			{
				if (Gib.RenderData.sIceGibTexture == null)
				{
					Gib.RenderData.sIceGibTexture = Game.Instance.Content.Load<Texture2D>("EffectTextures/iceGib");
				}
			}

			// Token: 0x17000538 RID: 1336
			// (get) Token: 0x06001472 RID: 5234 RVA: 0x0007F574 File Offset: 0x0007D774
			public int Effect
			{
				get
				{
					return this.mEffect;
				}
			}

			// Token: 0x17000539 RID: 1337
			// (get) Token: 0x06001473 RID: 5235 RVA: 0x0007F57C File Offset: 0x0007D77C
			public int DepthTechnique
			{
				get
				{
					return 4;
				}
			}

			// Token: 0x1700053A RID: 1338
			// (get) Token: 0x06001474 RID: 5236 RVA: 0x0007F57F File Offset: 0x0007D77F
			public int Technique
			{
				get
				{
					return 0;
				}
			}

			// Token: 0x1700053B RID: 1339
			// (get) Token: 0x06001475 RID: 5237 RVA: 0x0007F582 File Offset: 0x0007D782
			public int ShadowTechnique
			{
				get
				{
					return 5;
				}
			}

			// Token: 0x1700053C RID: 1340
			// (get) Token: 0x06001476 RID: 5238 RVA: 0x0007F585 File Offset: 0x0007D785
			public VertexBuffer Vertices
			{
				get
				{
					return this.mVertexBuffer;
				}
			}

			// Token: 0x1700053D RID: 1341
			// (get) Token: 0x06001477 RID: 5239 RVA: 0x0007F58D File Offset: 0x0007D78D
			public IndexBuffer Indices
			{
				get
				{
					return this.mIndexBuffer;
				}
			}

			// Token: 0x1700053E RID: 1342
			// (get) Token: 0x06001478 RID: 5240 RVA: 0x0007F595 File Offset: 0x0007D795
			public VertexDeclaration VertexDeclaration
			{
				get
				{
					return this.mVertexDeclaration;
				}
			}

			// Token: 0x1700053F RID: 1343
			// (get) Token: 0x06001479 RID: 5241 RVA: 0x0007F59D File Offset: 0x0007D79D
			public int VertexStride
			{
				get
				{
					return this.mVertexStride;
				}
			}

			// Token: 0x17000540 RID: 1344
			// (get) Token: 0x0600147A RID: 5242 RVA: 0x0007F5A5 File Offset: 0x0007D7A5
			public int VerticesHashCode
			{
				get
				{
					return this.mVerticesHash;
				}
			}

			// Token: 0x17000541 RID: 1345
			// (get) Token: 0x0600147B RID: 5243 RVA: 0x0007F5AD File Offset: 0x0007D7AD
			public bool MeshDirty
			{
				get
				{
					return this.mMeshDirty;
				}
			}

			// Token: 0x0600147C RID: 5244 RVA: 0x0007F5B8 File Offset: 0x0007D7B8
			public bool Cull(BoundingFrustum iViewFrustum)
			{
				BoundingSphere boundingSphere = this.mBoundingSphere;
				return boundingSphere.Contains(iViewFrustum) == ContainmentType.Disjoint;
			}

			// Token: 0x0600147D RID: 5245 RVA: 0x0007F5D8 File Offset: 0x0007D7D8
			public void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				RenderDeferredEffect renderDeferredEffect = iEffect as RenderDeferredEffect;
				this.mMaterial.AssignToEffect(renderDeferredEffect);
				renderDeferredEffect.GraphicsDevice.RenderState.DepthBias = -0.0001f;
				renderDeferredEffect.Bloat = this.mBloat;
				if (this.mFrozen)
				{
					renderDeferredEffect.DiffuseTexture0 = Gib.RenderData.sIceGibTexture;
					renderDeferredEffect.EmissiveAmount0 += 0.1f;
				}
				renderDeferredEffect.World = this.mTransform;
				renderDeferredEffect.CommitChanges();
				renderDeferredEffect.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
				renderDeferredEffect.Bloat = 0f;
				renderDeferredEffect.GraphicsDevice.RenderState.DepthBias = 0f;
			}

			// Token: 0x0600147E RID: 5246 RVA: 0x0007F698 File Offset: 0x0007D898
			public void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				RenderDeferredEffect renderDeferredEffect = iEffect as RenderDeferredEffect;
				this.mMaterial.AssignOpacityToEffect(renderDeferredEffect);
				renderDeferredEffect.Bloat = this.mBloat;
				renderDeferredEffect.World = this.mTransform;
				renderDeferredEffect.CommitChanges();
				renderDeferredEffect.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
				renderDeferredEffect.Bloat = 0f;
			}

			// Token: 0x0600147F RID: 5247 RVA: 0x0007F706 File Offset: 0x0007D906
			public void SetMeshDirty()
			{
				this.mMeshDirty = true;
			}

			// Token: 0x06001480 RID: 5248 RVA: 0x0007F710 File Offset: 0x0007D910
			public void SetMesh(VertexBuffer iVertices, IndexBuffer iIndices, ModelMeshPart iMeshPart, int iEffectHash)
			{
				this.mMeshDirty = false;
				this.mVertexBuffer = iVertices;
				this.mVerticesHash = iVertices.GetHashCode();
				this.mIndexBuffer = iIndices;
				if (this.mVertexBuffer.IsDisposed)
				{
					throw new Exception("What the hell are we gonna do?");
				}
				this.mEffect = iEffectHash;
				this.mVertexDeclaration = iMeshPart.VertexDeclaration;
				this.mBaseVertex = iMeshPart.BaseVertex;
				this.mNumVertices = iMeshPart.NumVertices;
				this.mPrimitiveCount = iMeshPart.PrimitiveCount;
				this.mStartIndex = iMeshPart.StartIndex;
				this.mStreamOffset = iMeshPart.StreamOffset;
				this.mVertexStride = iMeshPart.VertexStride;
				this.mMaterial.FetchFromEffect(iMeshPart.Effect as RenderDeferredEffect);
			}

			// Token: 0x040015CB RID: 5579
			private int mEffect;

			// Token: 0x040015CC RID: 5580
			private VertexDeclaration mVertexDeclaration;

			// Token: 0x040015CD RID: 5581
			private int mBaseVertex;

			// Token: 0x040015CE RID: 5582
			private int mNumVertices;

			// Token: 0x040015CF RID: 5583
			private int mPrimitiveCount;

			// Token: 0x040015D0 RID: 5584
			private int mStartIndex;

			// Token: 0x040015D1 RID: 5585
			private int mStreamOffset;

			// Token: 0x040015D2 RID: 5586
			private int mVertexStride;

			// Token: 0x040015D3 RID: 5587
			private VertexBuffer mVertexBuffer;

			// Token: 0x040015D4 RID: 5588
			private IndexBuffer mIndexBuffer;

			// Token: 0x040015D5 RID: 5589
			public int mVerticesHash;

			// Token: 0x040015D6 RID: 5590
			private bool mMeshDirty;

			// Token: 0x040015D7 RID: 5591
			private RenderDeferredMaterial mMaterial;

			// Token: 0x040015D8 RID: 5592
			public BoundingSphere mBoundingSphere;

			// Token: 0x040015D9 RID: 5593
			public Matrix mTransform;

			// Token: 0x040015DA RID: 5594
			public bool mFrozen;

			// Token: 0x040015DB RID: 5595
			public float mBloat;

			// Token: 0x040015DC RID: 5596
			private static readonly Vector3 ICE_COLOR = new Vector3(0.5f, 1f, 1.5f);

			// Token: 0x040015DD RID: 5597
			private static Texture2D sIceGibTexture;
		}
	}
}
