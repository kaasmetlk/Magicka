using System;
using System.Collections.Generic;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Math;
using JigLibX.Physics;
using Magicka.GameLogic.Entities.Bosses;
using Magicka.GameLogic.GameStates;
using Magicka.Levels;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;

namespace Magicka.GameLogic.Entities
{
	// Token: 0x020000D9 RID: 217
	public class SprayEntity : Entity
	{
		// Token: 0x0600067B RID: 1659 RVA: 0x000267E0 File Offset: 0x000249E0
		public static SprayEntity GetInstance()
		{
			SprayEntity sprayEntity = SprayEntity.sCache[0];
			SprayEntity.sCache.RemoveAt(0);
			SprayEntity.sCache.Add(sprayEntity);
			return sprayEntity;
		}

		// Token: 0x0600067C RID: 1660 RVA: 0x00026810 File Offset: 0x00024A10
		public static void InitializeCache(PlayState iPlayState, int iNr)
		{
			SprayEntity.sCache = new List<SprayEntity>(iNr);
			for (int i = 0; i < iNr; i++)
			{
				SprayEntity.sCache.Add(new SprayEntity(iPlayState));
			}
		}

		// Token: 0x0600067D RID: 1661 RVA: 0x00026844 File Offset: 0x00024A44
		public static SprayEntity GetSpecificInstance(ushort iHandle)
		{
			SprayEntity sprayEntity = Entity.GetFromHandle((int)iHandle) as SprayEntity;
			SprayEntity.sCache.Remove(sprayEntity);
			SprayEntity.sCache.Add(sprayEntity);
			return sprayEntity;
		}

		// Token: 0x0600067E RID: 1662 RVA: 0x00026878 File Offset: 0x00024A78
		public SprayEntity(PlayState iPlayState) : base(iPlayState)
		{
			this.mBody = new Body();
			this.mCollision = new CollisionSkin(this.mBody);
			this.mCollision.AddPrimitive(new Capsule(Vector3.Zero, Matrix.Identity, 0.5f, 0f), 1, new MaterialProperties(0.333f, 0.8f, 0.8f));
			this.mBody.CollisionSkin = this.mCollision;
			this.mBody.Tag = this;
			this.mBody.Immovable = false;
			this.mCollision.callbackFn += this.OnCollision;
			VertexPositionNormalTexture[] array = new VertexPositionNormalTexture[4];
			array[0].TextureCoordinate = new Vector2(1f, 1f);
			array[0].Position = new Vector3(0.5f, 0f, 0f);
			array[0].Normal = new Vector3(0f, 1f, 0f);
			array[1].TextureCoordinate = new Vector2(0f, 1f);
			array[1].Position = new Vector3(-0.5f, 0f, 0f);
			array[1].Normal = new Vector3(0f, 1f, 0f);
			array[2].TextureCoordinate = new Vector2(0f, 0f);
			array[2].Position = new Vector3(-0.5f, 0f, 1f);
			array[2].Normal = new Vector3(0f, 1f, 0f);
			array[3].TextureCoordinate = new Vector2(1f, 0f);
			array[3].Position = new Vector3(0.5f, 0f, 1f);
			array[3].Normal = new Vector3(0f, 1f, 0f);
			VertexBuffer vertexBuffer;
			VertexDeclaration iDeclaration;
			lock (Game.Instance.GraphicsDevice)
			{
				vertexBuffer = new VertexBuffer(Game.Instance.GraphicsDevice, array.Length * VertexPositionNormalTexture.SizeInBytes, BufferUsage.WriteOnly);
				vertexBuffer.SetData<VertexPositionNormalTexture>(array);
				iDeclaration = new VertexDeclaration(Game.Instance.GraphicsDevice, VertexPositionNormalTexture.VertexElements);
				vertexBuffer.Name = "SprayEntityBuffer";
			}
			RenderDeferredMaterial mMaterial = default(RenderDeferredMaterial);
			lock (Game.Instance.GraphicsDevice)
			{
				mMaterial.DiffuseTexture0 = this.mPlayState.Content.Load<Texture2D>("EffectTextures/Spray_web");
			}
			mMaterial.VertexColorEnabled = false;
			this.mRenderData = new SprayEntity.RenderData[3];
			for (int i = 0; i < this.mRenderData.Length; i++)
			{
				this.mRenderData[i] = new SprayEntity.RenderData(vertexBuffer, iDeclaration);
				this.mRenderData[i].mMaterial = mMaterial;
			}
		}

		// Token: 0x0600067F RID: 1663 RVA: 0x00026B98 File Offset: 0x00024D98
		protected bool OnCollision(CollisionSkin iSkin0, int iPrim0, CollisionSkin iSkin1, int iPrim1)
		{
			if (iSkin1.Owner != null)
			{
				if (iSkin1.Owner.Tag is BossCollisionZone || iSkin1.Owner.Tag is SprayEntity)
				{
					return false;
				}
				if (iSkin1.Owner.Tag is Shield)
				{
					this.mBody.Immovable = true;
					return true;
				}
				if (iSkin1.Owner != null && iSkin1.Owner.Tag is Character && !(iSkin1.Owner.Tag as Character).IsEntangled && iSkin1.Owner.Tag is Avatar)
				{
					if (!this.mBody.Immovable)
					{
						(iSkin1.Owner.Tag as Character).Entangle(12f);
						this.mBody.Immovable = true;
						this.mBody.Velocity = default(Vector3);
					}
					return false;
				}
				return false;
			}
			else
			{
				if (iSkin1.Tag is LevelModel)
				{
					this.mBody.Immovable = true;
					return true;
				}
				return true;
			}
		}

		// Token: 0x17000140 RID: 320
		// (get) Token: 0x06000680 RID: 1664 RVA: 0x00026CA3 File Offset: 0x00024EA3
		// (set) Token: 0x06000681 RID: 1665 RVA: 0x00026CAB File Offset: 0x00024EAB
		public SprayEntity Child
		{
			get
			{
				return this.mChild;
			}
			set
			{
				this.mChild = ((value == this) ? null : value);
			}
		}

		// Token: 0x06000682 RID: 1666 RVA: 0x00026CBB File Offset: 0x00024EBB
		public void Initialize(Entity iOwner, Vector3 iPosition, Vector3 iDirection, float iVelocity)
		{
			this.Initialize(iOwner, null, iPosition, iDirection, iVelocity);
		}

		// Token: 0x06000683 RID: 1667 RVA: 0x00026CCC File Offset: 0x00024ECC
		public void Initialize(Entity iOwner, SprayEntity iChild, Vector3 iPosition, Vector3 iDirection, float iVelocity)
		{
			base.Initialize();
			Vector3 zero = Vector3.Zero;
			Vector3 up = Vector3.Up;
			Matrix orientation;
			Matrix.CreateWorld(ref zero, ref iDirection, ref up, out orientation);
			Vector3.Add(ref iPosition, ref iDirection, out iPosition);
			this.mBody.MoveTo(iPosition, orientation);
			Vector3.Multiply(ref iDirection, iVelocity, out iDirection);
			this.mBody.Velocity = iDirection;
			this.mBody.Immovable = false;
			this.mBody.ApplyGravity = false;
			this.mOwner = iOwner;
			this.mChild = iChild;
			this.mDead = false;
			this.mDecayTimer = 0f;
			this.mDragStrength = 0f;
			(this.mCollision.GetPrimitiveLocal(0) as Capsule).Length = 0f;
			(this.mCollision.GetPrimitiveNewWorld(0) as Capsule).Length = 0f;
			(this.mCollision.GetPrimitiveOldWorld(0) as Capsule).Length = 0f;
			if (iOwner != null)
			{
				this.mBody.CollisionSkin.NonCollidables.Add(iOwner.Body.CollisionSkin);
			}
		}

		// Token: 0x06000684 RID: 1668 RVA: 0x00026DE0 File Offset: 0x00024FE0
		public override void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			base.Update(iDataChannel, iDeltaTime);
			this.mDragStrength += iDeltaTime;
			Vector3 velocity = this.mBody.Velocity;
			if (velocity.LengthSquared() > 1E-45f)
			{
				velocity.Y -= iDeltaTime * 10f;
				this.mBody.Velocity = velocity;
			}
			this.mBody.AngularVelocity = Vector3.Zero;
			if (this.mChild != null && this.mChild.Dead)
			{
				if (this.mChild.Child != null)
				{
					this.mChild = this.mChild.Child;
				}
				else
				{
					this.mChild = null;
				}
			}
			if (this.mBody.Immovable)
			{
				this.mDecayTimer += iDeltaTime;
				if (this.mDecayTimer >= 1f)
				{
					this.mDead = true;
				}
			}
			Transform identity = Transform.Identity;
			if (this.mChild != null || this.mOwner != null)
			{
				Matrix identity2 = Matrix.Identity;
				Vector3 position = this.Position;
				Vector3 vector = Vector3.Zero;
				if (this.mChild != null)
				{
					vector = this.mChild.Position;
				}
				else if (this.mOwner != null)
				{
					vector = this.mOwner.Position;
				}
				Vector3 zero = Vector3.Zero;
				Vector3 up = Vector3.Up;
				Vector3 zero2 = Vector3.Zero;
				Vector3 right = Vector3.Right;
				Vector3.Multiply(ref right, 0.01f, out right);
				Vector3.Add(ref vector, ref right, out vector);
				Vector3.Subtract(ref position, ref vector, out zero);
				float num = zero.Length();
				zero.Normalize();
				Matrix.CreateWorld(ref zero2, ref zero, ref up, out identity.Orientation);
				(this.mCollision.GetPrimitiveLocal(0) as Capsule).Length = num;
				(this.mCollision.GetPrimitiveNewWorld(0) as Capsule).Length = num;
				(this.mCollision.GetPrimitiveOldWorld(0) as Capsule).Length = num;
				this.mCollision.UpdateWorldBoundingBox();
				Vector3 position2 = this.mPlayState.Camera.Position;
				Vector3 vector2;
				Vector3.Subtract(ref position2, ref position, out vector2);
				Vector3 right2;
				Vector3.Cross(ref zero, ref vector2, out right2);
				Vector3.Normalize(ref right2, out right2);
				Vector3.Cross(ref right2, ref zero, out up);
				Vector3.Normalize(ref up, out up);
				Matrix matrix;
				Matrix.CreateScale(1f, 1f, num, out matrix);
				identity2.Forward = zero;
				identity2.Up = up;
				identity2.Right = right2;
				Matrix.Multiply(ref matrix, ref identity2, out identity2);
				identity2.Translation = position;
				this.mRenderData[(int)iDataChannel].mMaterial.Alpha = (float)Math.Pow((double)(1f - this.mDecayTimer), 0.2);
				this.mRenderData[(int)iDataChannel].mSize = num;
				this.mRenderData[(int)iDataChannel].mTransform = identity2;
				this.mPlayState.Scene.AddRenderableObject(iDataChannel, this.mRenderData[(int)iDataChannel]);
			}
			this.mBody.SetOrientation(identity.Orientation);
		}

		// Token: 0x06000685 RID: 1669 RVA: 0x000270C5 File Offset: 0x000252C5
		public override void Deinitialize()
		{
			base.Deinitialize();
			this.mBody.CollisionSkin.NonCollidables.Clear();
		}

		// Token: 0x17000141 RID: 321
		// (get) Token: 0x06000686 RID: 1670 RVA: 0x000270E2 File Offset: 0x000252E2
		public override bool Dead
		{
			get
			{
				return this.mDead;
			}
		}

		// Token: 0x17000142 RID: 322
		// (get) Token: 0x06000687 RID: 1671 RVA: 0x000270EA File Offset: 0x000252EA
		public override bool Removable
		{
			get
			{
				return this.mDead;
			}
		}

		// Token: 0x06000688 RID: 1672 RVA: 0x000270F2 File Offset: 0x000252F2
		public override void Kill()
		{
			this.mDead = true;
		}

		// Token: 0x06000689 RID: 1673 RVA: 0x000270FB File Offset: 0x000252FB
		protected override void INetworkUpdate(ref EntityUpdateMessage iMsg)
		{
			base.INetworkUpdate(ref iMsg);
			this.mBody.Velocity = iMsg.Velocity;
		}

		// Token: 0x0600068A RID: 1674 RVA: 0x00027118 File Offset: 0x00025318
		protected override void IGetNetworkUpdate(out EntityUpdateMessage oMsg, float iPrediction)
		{
			oMsg = default(EntityUpdateMessage);
			oMsg.Features |= EntityFeatures.Position;
			oMsg.Position = this.Position;
			Vector3 velocity = this.mBody.Velocity;
			Vector3.Multiply(ref velocity, iPrediction, out velocity);
			Vector3.Add(ref velocity, ref oMsg.Position, out oMsg.Position);
			oMsg.Features |= EntityFeatures.Velocity;
			oMsg.Velocity = this.mBody.Velocity;
		}

		// Token: 0x0400054B RID: 1355
		private const float DECAYTIME = 1f;

		// Token: 0x0400054C RID: 1356
		private static List<SprayEntity> sCache;

		// Token: 0x0400054D RID: 1357
		private static readonly int SplashEffect = "web_hit".GetHashCodeCustom();

		// Token: 0x0400054E RID: 1358
		private SprayEntity.RenderData[] mRenderData;

		// Token: 0x0400054F RID: 1359
		private Entity mOwner;

		// Token: 0x04000550 RID: 1360
		private float mDecayTimer;

		// Token: 0x04000551 RID: 1361
		private SprayEntity mChild;

		// Token: 0x04000552 RID: 1362
		private float mDragStrength;

		// Token: 0x020000DA RID: 218
		protected class RenderData : IRenderableObject
		{
			// Token: 0x0600068C RID: 1676 RVA: 0x000271A1 File Offset: 0x000253A1
			public RenderData(VertexBuffer iVertexBuffer, VertexDeclaration iDeclaration)
			{
				this.mVerticesHash = iVertexBuffer.GetHashCode();
				this.mVertexBuffer = iVertexBuffer;
				this.mVertexDeclaration = iDeclaration;
			}

			// Token: 0x17000143 RID: 323
			// (get) Token: 0x0600068D RID: 1677 RVA: 0x000271C3 File Offset: 0x000253C3
			public int Effect
			{
				get
				{
					return RenderDeferredEffect.TYPEHASH;
				}
			}

			// Token: 0x17000144 RID: 324
			// (get) Token: 0x0600068E RID: 1678 RVA: 0x000271CA File Offset: 0x000253CA
			public int Technique
			{
				get
				{
					return 0;
				}
			}

			// Token: 0x17000145 RID: 325
			// (get) Token: 0x0600068F RID: 1679 RVA: 0x000271CD File Offset: 0x000253CD
			public VertexBuffer Vertices
			{
				get
				{
					return this.mVertexBuffer;
				}
			}

			// Token: 0x17000146 RID: 326
			// (get) Token: 0x06000690 RID: 1680 RVA: 0x000271D5 File Offset: 0x000253D5
			public int VerticesHashCode
			{
				get
				{
					return this.mVerticesHash;
				}
			}

			// Token: 0x17000147 RID: 327
			// (get) Token: 0x06000691 RID: 1681 RVA: 0x000271DD File Offset: 0x000253DD
			public int VertexStride
			{
				get
				{
					return VertexPositionNormalTexture.SizeInBytes;
				}
			}

			// Token: 0x17000148 RID: 328
			// (get) Token: 0x06000692 RID: 1682 RVA: 0x000271E4 File Offset: 0x000253E4
			public IndexBuffer Indices
			{
				get
				{
					return null;
				}
			}

			// Token: 0x17000149 RID: 329
			// (get) Token: 0x06000693 RID: 1683 RVA: 0x000271E7 File Offset: 0x000253E7
			public VertexDeclaration VertexDeclaration
			{
				get
				{
					return this.mVertexDeclaration;
				}
			}

			// Token: 0x06000694 RID: 1684 RVA: 0x000271EF File Offset: 0x000253EF
			public bool Cull(BoundingFrustum iViewFrustum)
			{
				return false;
			}

			// Token: 0x06000695 RID: 1685 RVA: 0x000271F4 File Offset: 0x000253F4
			public void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				RenderDeferredEffect renderDeferredEffect = iEffect as RenderDeferredEffect;
				this.mMaterial.AssignToEffect(renderDeferredEffect);
				renderDeferredEffect.EmissiveAmount0 = 1f;
				renderDeferredEffect.World = this.mTransform;
				renderDeferredEffect.GraphicsDevice.RenderState.CullMode = CullMode.None;
				renderDeferredEffect.DiffuseColor0 = Vector3.One;
				renderDeferredEffect.CommitChanges();
				renderDeferredEffect.GraphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleFan, 0, 2);
				renderDeferredEffect.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
			}

			// Token: 0x1700014A RID: 330
			// (get) Token: 0x06000696 RID: 1686 RVA: 0x0002726C File Offset: 0x0002546C
			public int DepthTechnique
			{
				get
				{
					return 4;
				}
			}

			// Token: 0x1700014B RID: 331
			// (get) Token: 0x06000697 RID: 1687 RVA: 0x0002726F File Offset: 0x0002546F
			public int ShadowTechnique
			{
				get
				{
					return 5;
				}
			}

			// Token: 0x06000698 RID: 1688 RVA: 0x00027274 File Offset: 0x00025474
			public void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				RenderDeferredEffect renderDeferredEffect = iEffect as RenderDeferredEffect;
				this.mMaterial.AssignToEffect(renderDeferredEffect);
				renderDeferredEffect.World = this.mTransform;
				renderDeferredEffect.GraphicsDevice.RenderState.CullMode = CullMode.None;
				renderDeferredEffect.CommitChanges();
				renderDeferredEffect.GraphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleFan, 0, 2);
				renderDeferredEffect.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
			}

			// Token: 0x04000553 RID: 1363
			private int mVerticesHash;

			// Token: 0x04000554 RID: 1364
			private VertexBuffer mVertexBuffer;

			// Token: 0x04000555 RID: 1365
			private VertexDeclaration mVertexDeclaration;

			// Token: 0x04000556 RID: 1366
			public Matrix mTransform;

			// Token: 0x04000557 RID: 1367
			public RenderDeferredMaterial mMaterial;

			// Token: 0x04000558 RID: 1368
			public float mScroll;

			// Token: 0x04000559 RID: 1369
			public float mSize;
		}
	}
}
