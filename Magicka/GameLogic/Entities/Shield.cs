using System;
using System.Collections.Generic;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Math;
using JigLibX.Physics;
using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.UI;
using Magicka.Graphics.Effects;
using Magicka.Network;
using Magicka.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;

namespace Magicka.GameLogic.Entities
{
	// Token: 0x0200062E RID: 1582
	public class Shield : Entity, IDamageable
	{
		// Token: 0x06002FB2 RID: 12210 RVA: 0x001817E8 File Offset: 0x0017F9E8
		public static void InitializeCache(int iNrOfShields, PlayState iPlayState)
		{
			Shield.mCache = new List<Shield>(iNrOfShields);
			for (int i = 0; i < iNrOfShields; i++)
			{
				Shield.mCache.Add(new Shield(iPlayState));
			}
		}

		// Token: 0x06002FB3 RID: 12211 RVA: 0x0018181C File Offset: 0x0017FA1C
		public static Shield GetFromCache(PlayState iPlayState)
		{
			if (Shield.mCache.Count > 0)
			{
				Shield result = Shield.mCache[Shield.mCache.Count - 1];
				Shield.mCache.RemoveAt(Shield.mCache.Count - 1);
				return result;
			}
			return new Shield(iPlayState);
		}

		// Token: 0x06002FB4 RID: 12212 RVA: 0x0018186C File Offset: 0x0017FA6C
		protected Shield(PlayState iPlayState) : base(iPlayState)
		{
			GraphicsDevice graphicsDevice = Game.Instance.GraphicsDevice;
			if (Shield.mEffect == null || Shield.mEffect.IsDisposed)
			{
				lock (graphicsDevice)
				{
					Shield.mEffect = new ShieldEffect(graphicsDevice, iPlayState.Content);
					Shield.mEffect.Texture = iPlayState.Content.Load<Texture2D>("EffectTextures/shield");
				}
			}
			if (Shield.sVertexDeclaration == null || Shield.sVertexDeclaration.IsDisposed)
			{
				lock (graphicsDevice)
				{
					Shield.sVertexDeclaration = new VertexDeclaration(Game.Instance.GraphicsDevice, VertexPositionNormalTexture.VertexElements);
				}
			}
			if (Shield.sSphereVertices == null || Shield.sSphereVertices.IsDisposed)
			{
				VertexPositionNormalTexture[] array = new VertexPositionNormalTexture[Math.Max(31, 0) * 65];
				VertexPositionNormalTexture vertexPositionNormalTexture = default(VertexPositionNormalTexture);
				for (int i = 0; i <= 64; i++)
				{
					float num = 6.2831855f * (float)i / 64f;
					float num2 = (float)Math.Cos((double)num);
					float num3 = (float)Math.Sin((double)num);
					int num4 = i * 31;
					vertexPositionNormalTexture.TextureCoordinate.X = 1f * (float)i / 64f;
					for (int j = 0; j < 31; j++)
					{
						float num5 = -1.5707964f + 3.1415927f * ((float)j + 1.9f) / 32f;
						vertexPositionNormalTexture.Position.Y = (float)Math.Sin((double)num5);
						float num6 = (float)Math.Cos((double)num5);
						vertexPositionNormalTexture.Position.X = num2 * num6;
						vertexPositionNormalTexture.Position.Z = num3 * num6;
						vertexPositionNormalTexture.TextureCoordinate.Y = 1f * (float)j / 32f;
						vertexPositionNormalTexture.Normal = vertexPositionNormalTexture.Position;
						array[num4 + j] = vertexPositionNormalTexture;
					}
				}
				ushort[] array2 = new ushort[11520];
				int num7 = 31;
				int num8 = 0;
				for (int k = 0; k < 64; k++)
				{
					int num9 = k * num7;
					for (int l = 0; l < num7 - 1; l++)
					{
						array2[num8++] = (ushort)(num9 + num7);
						array2[num8++] = (ushort)(num9 + 1);
						array2[num8++] = (ushort)num9;
						array2[num8++] = (ushort)(num9 + num7);
						array2[num8++] = (ushort)(num9 + num7 + 1);
						array2[num8++] = (ushort)(num9 + 1);
						num9++;
					}
				}
				lock (graphicsDevice)
				{
					Shield.sSphereVertices = new VertexBuffer(graphicsDevice, array.Length * VertexPositionNormalTexture.SizeInBytes, BufferUsage.WriteOnly);
					Shield.sSphereVertices.SetData<VertexPositionNormalTexture>(array);
					Shield.sSphereIndices = new IndexBuffer(graphicsDevice, array2.Length * 2, BufferUsage.WriteOnly, IndexElementSize.SixteenBits);
					Shield.sSphereIndices.SetData<ushort>(array2);
				}
				Shield.sSphereNumVertices = array.Length;
				Shield.sSphereNumPrimitives = array2.Length / 3;
			}
			if (Shield.sWallVertices == null || Shield.sWallVertices.IsDisposed)
			{
				VertexPositionNormalTexture[] array = new VertexPositionNormalTexture[242];
				ushort[] array2 = new ushort[1200];
				array[120].Position.X = 0.2f;
				array[120].Normal.X = 1f;
				array[241].Position.X = -0.2f;
				array[241].Normal.X = -1f;
				float num10 = 0.1f;
				for (int m = 0; m < 20; m++)
				{
					float num11 = (float)m / 20f;
					num11 = -(float)Math.Cos((double)(num11 * 3.1415927f));
					array[m].Position.Z = num11 * 6f;
					array[m].Position.Y = (float)Math.Pow(1.0 - (double)(num11 * num11), 0.15) * 3f;
					array[m].Normal.X = 1f;
					array[m].TextureCoordinate.X = num11;
					array[m].TextureCoordinate.Y = (float)Math.Pow(1.0 - (double)(num11 * num11), 0.15);
					array[m].TextureCoordinate.Normalize();
					array[m + 20].Position.Z = -array[m].Position.Z;
					array[m + 20].Position.Y = -array[m].Position.Y;
					array[m + 20].Normal.X = 1f;
					array[m + 20].TextureCoordinate.X = -array[m].TextureCoordinate.X;
					array[m + 20].TextureCoordinate.Y = -array[m].TextureCoordinate.Y;
					array[m + 40].Position.Z = array[m].Position.Z * 0.666f;
					array[m + 40].Position.Y = array[m].Position.Y * 0.666f;
					array[m + 40].Position.X = 0.1332f;
					array[m + 40].Normal.X = 1f;
					array[m + 40].TextureCoordinate.X = array[m].TextureCoordinate.X * 0.666f;
					array[m + 40].TextureCoordinate.Y = array[m].TextureCoordinate.Y * 0.666f;
					array[m + 60].Position.Z = -array[m + 40].Position.Z;
					array[m + 60].Position.Y = -array[m + 40].Position.Y;
					array[m + 60].Position.X = 0.1332f;
					array[m + 60].Normal.X = 1f;
					array[m + 60].TextureCoordinate.X = -array[m + 40].TextureCoordinate.X;
					array[m + 60].TextureCoordinate.Y = -array[m + 40].TextureCoordinate.Y;
					array[m + 80].Position.Z = array[m].Position.Z * 0.333f;
					array[m + 80].Position.Y = array[m].Position.Y * 0.333f;
					array[m + 80].Position.X = 0.17999999f;
					array[m + 80].Normal.X = 1f;
					array[m + 80].TextureCoordinate.X = array[m].TextureCoordinate.X * 0.333f;
					array[m + 80].TextureCoordinate.Y = array[m].TextureCoordinate.Y * 0.333f;
					array[m + 100].Position.Z = -array[m + 80].Position.Z;
					array[m + 100].Position.Y = -array[m + 80].Position.Y;
					array[m + 100].Position.X = 0.17999999f;
					array[m + 100].Normal.X = 1f;
					array[m + 100].TextureCoordinate.X = -array[m + 80].TextureCoordinate.X;
					array[m + 100].TextureCoordinate.Y = -array[m + 80].TextureCoordinate.Y;
					array[m + 1 + 120].Position.Z = array[m].Position.Z;
					array[m + 1 + 120].Position.Y = array[m].Position.Y;
					array[m + 1 + 120].Normal.X = -1f;
					array[m + 1 + 120].TextureCoordinate.X = array[m].TextureCoordinate.X;
					array[m + 1 + 120].TextureCoordinate.Y = array[m].TextureCoordinate.Y;
					array[m + 1 + 140].Position.Z = -array[m].Position.Z;
					array[m + 1 + 140].Position.Y = -array[m].Position.Y;
					array[m + 1 + 140].Normal.X = -1f;
					array[m + 1 + 140].TextureCoordinate.X = -array[m].TextureCoordinate.X;
					array[m + 1 + 140].TextureCoordinate.Y = -array[m].TextureCoordinate.Y;
					array[m + 1 + 160].Position.Z = array[m].Position.Z * 0.666f;
					array[m + 1 + 160].Position.Y = array[m].Position.Y * 0.666f;
					array[m + 1 + 160].Position.X = -0.1332f;
					array[m + 1 + 160].Normal.X = -1f;
					array[m + 1 + 160].TextureCoordinate.X = array[m + 1 + 120].TextureCoordinate.X * 0.666f;
					array[m + 1 + 160].TextureCoordinate.Y = array[m + 1 + 120].TextureCoordinate.Y * 0.666f;
					array[m + 1 + 180].Position.Z = -array[m + 40].Position.Z;
					array[m + 1 + 180].Position.Y = -array[m + 40].Position.Y;
					array[m + 1 + 180].Position.X = -0.1332f;
					array[m + 1 + 180].Normal.X = -1f;
					array[m + 1 + 180].TextureCoordinate.X = -array[m + 40].TextureCoordinate.X;
					array[m + 1 + 180].TextureCoordinate.Y = -array[m + 40].TextureCoordinate.Y;
					array[m + 1 + 200].Position.Z = array[m].Position.Z * 0.333f;
					array[m + 1 + 200].Position.Y = array[m].Position.Y * 0.333f;
					array[m + 1 + 200].Position.X = -0.17999999f;
					array[m + 1 + 200].Normal.X = -1f;
					array[m + 1 + 200].TextureCoordinate.X = array[m].TextureCoordinate.X * 0.333f;
					array[m + 1 + 200].TextureCoordinate.Y = array[m].TextureCoordinate.Y * 0.333f;
					array[m + 1 + 220].Position.Z = -array[m + 80].Position.Z;
					array[m + 1 + 220].Position.Y = -array[m + 80].Position.Y;
					array[m + 1 + 220].Position.X = -0.17999999f;
					array[m + 1 + 220].Normal.X = -1f;
					array[m + 1 + 220].TextureCoordinate.X = -array[m + 80].TextureCoordinate.X;
					array[m + 1 + 220].TextureCoordinate.Y = -array[m + 80].TextureCoordinate.Y;
					num11 += num10;
				}
				for (int n = 0; n < 40; n++)
				{
					array2[n * 15] = (ushort)n;
					array2[n * 15 + 1] = (ushort)(n + 40);
					array2[n * 15 + 2] = (ushort)((n + 1) % 40);
					array2[n * 15 + 3] = (ushort)(n + 40);
					array2[n * 15 + 4] = (ushort)((n + 1) % 40 + 40);
					array2[n * 15 + 5] = (ushort)((n + 1) % 40);
					array2[n * 15 + 6] = (ushort)(n + 40);
					array2[n * 15 + 7] = (ushort)(n + 80);
					array2[n * 15 + 8] = (ushort)((n + 1) % 40 + 40);
					array2[n * 15 + 9] = (ushort)(n + 80);
					array2[n * 15 + 10] = (ushort)((n + 1) % 40 + 80);
					array2[n * 15 + 11] = (ushort)((n + 1) % 40 + 40);
					array2[n * 15 + 12] = (ushort)(n + 80);
					array2[n * 15 + 13] = 120;
					array2[n * 15 + 14] = (ushort)((n + 1) % 40 + 80);
					array2[600 + n * 15] = (ushort)(n + 120 + 1);
					array2[600 + n * 15 + 1] = (ushort)((n + 1) % 40 + 120 + 1);
					array2[600 + n * 15 + 2] = (ushort)(n + 160 + 1);
					array2[600 + n * 15 + 3] = (ushort)(n + 160 + 1);
					array2[600 + n * 15 + 4] = (ushort)((n + 1) % 40 + 120 + 1);
					array2[600 + n * 15 + 5] = (ushort)((n + 1) % 40 + 160 + 1);
					array2[600 + n * 15 + 6] = (ushort)(n + 160 + 1);
					array2[600 + n * 15 + 7] = (ushort)((n + 1) % 40 + 160 + 1);
					array2[600 + n * 15 + 8] = (ushort)(n + 200 + 1);
					array2[600 + n * 15 + 9] = (ushort)(n + 200 + 1);
					array2[600 + n * 15 + 10] = (ushort)((n + 1) % 40 + 160 + 1);
					array2[600 + n * 15 + 11] = (ushort)((n + 1) % 40 + 200 + 1);
					array2[600 + n * 15 + 12] = (ushort)(n + 200 + 1);
					array2[600 + n * 15 + 13] = (ushort)((n + 1) % 40 + 200 + 1);
					array2[600 + n * 15 + 14] = 241;
				}
				lock (graphicsDevice)
				{
					Shield.sWallVertices = new VertexBuffer(graphicsDevice, array.Length * VertexPositionNormalTexture.SizeInBytes, BufferUsage.WriteOnly);
					Shield.sWallVertices.SetData<VertexPositionNormalTexture>(array);
					Shield.sWallIndices = new IndexBuffer(graphicsDevice, array2.Length * 2, BufferUsage.WriteOnly, IndexElementSize.SixteenBits);
					Shield.sWallIndices.SetData<ushort>(array2);
				}
				Shield.sWallNumVertices = array.Length;
				Shield.sWallNumPrimitives = array2.Length / 3;
			}
			this.mBody = new Body();
			this.mBody.Immovable = false;
			this.mBody.ApplyGravity = false;
			this.mBody.Tag = this;
			this.mBody.AllowFreezing = false;
			this.mCollision = new CollisionSkin(this.mBody);
			this.mBody.CollisionSkin = this.mCollision;
			this.mCollision.AddPrimitive(new HollowSphere(Vector3.Zero, 1f), 1, new MaterialProperties(1f, 0.8f, 0.8f));
			this.mTargetTint = this.mTint;
			this.mCollision.AddPrimitive(new Box(new Vector3(-0.2f, -3f, -6f), Matrix.Identity, new Vector3(0.4f, 6f, 12f)), 1, new MaterialProperties(1f, 0.8f, 0.8f));
			this.mCollision.ApplyLocalTransform(Transform.Identity);
			this.mCollision.callbackFn += this.OnCollision;
			this.mCollision.postCollisionCallbackFn += this.PostCollision;
			this.mRenderData = new Shield.RenderData[3];
			for (int num12 = 0; num12 < 3; num12++)
			{
				Shield.RenderData renderData = new Shield.RenderData();
				this.mRenderData[num12] = renderData;
			}
			this.mDamagePoints = new Vector4[16];
		}

		// Token: 0x06002FB5 RID: 12213 RVA: 0x00182BA8 File Offset: 0x00180DA8
		private bool OnCollision(CollisionSkin iSkin0, int iPrim0, CollisionSkin iSkin1, int iPrim1)
		{
			return !(this.mIntersectKillTimer > 0f | this.mHitPoints <= 0f) || !(iSkin1.Owner is CharacterBody);
		}

		// Token: 0x06002FB6 RID: 12214 RVA: 0x00182BDC File Offset: 0x00180DDC
		private void PostCollision(ref CollisionInfo iCollisionInfo)
		{
			if (iCollisionInfo.SkinInfo.Skin0 == this.mBody.CollisionSkin)
			{
				iCollisionInfo.SkinInfo.IgnoreSkin0 = true;
			}
			else
			{
				if (iCollisionInfo.SkinInfo.Skin1 != this.mBody.CollisionSkin)
				{
					throw new Exception();
				}
				iCollisionInfo.SkinInfo.IgnoreSkin1 = true;
			}
			int num = 0;
			if (iCollisionInfo.SkinInfo.Skin0.GetPrimitiveNewWorld(iCollisionInfo.SkinInfo.IndexPrim0).Type == JigLibX.Geometry.PrimitiveType.TriangleMesh || iCollisionInfo.SkinInfo.Skin1.GetPrimitiveNewWorld(iCollisionInfo.SkinInfo.IndexPrim1).Type == JigLibX.Geometry.PrimitiveType.TriangleMesh)
			{
				num++;
			}
			for (int i = 0; i < this.mCollision.Collisions.Count; i++)
			{
				CollDetectInfo skinInfo = this.mCollision.Collisions[i].SkinInfo;
				if (skinInfo.Skin0.GetPrimitiveNewWorld(skinInfo.IndexPrim0).Type == JigLibX.Geometry.PrimitiveType.TriangleMesh || skinInfo.Skin1.GetPrimitiveNewWorld(skinInfo.IndexPrim1).Type == JigLibX.Geometry.PrimitiveType.TriangleMesh)
				{
					num++;
				}
			}
			if (num >= 2)
			{
				Vector3 position = this.mBody.Position;
				float num2;
				Vector3.DistanceSquared(ref this.mLastPosition, ref position, out num2);
				if (num2 > 1E-06f)
				{
					this.Kill();
				}
			}
		}

		// Token: 0x17000B54 RID: 2900
		// (get) Token: 0x06002FB7 RID: 12215 RVA: 0x00182D29 File Offset: 0x00180F29
		public bool Resting
		{
			get
			{
				return this.mRestingTimer < 0f;
			}
		}

		// Token: 0x06002FB8 RID: 12216 RVA: 0x00182D38 File Offset: 0x00180F38
		public override Vector3 CalcImpulseVelocity(Vector3 iDirection, float iElevation, float iMassPower, float iDistance)
		{
			return default(Vector3);
		}

		// Token: 0x06002FB9 RID: 12217 RVA: 0x00182D50 File Offset: 0x00180F50
		public virtual void Initialize(ISpellCaster iOwner, Vector3 iPosition, float iRadius, Vector3 iDirection, ShieldType iShieldType, float iHitpoints, Vector3 iColor)
		{
			this.mMaxHitPoints = 5000f;
			this.mHitPoints = iHitpoints;
			this.mOwner = iOwner;
			Matrix identity = Matrix.Identity;
			identity.Forward = iDirection;
			identity.Up = Vector3.Up;
			identity.Right = Vector3.Cross(identity.Forward, identity.Up);
			identity.Up = Vector3.Cross(identity.Right, identity.Forward);
			identity.Up = Vector3.Normalize(identity.Up);
			identity.Right = Vector3.Normalize(identity.Right);
			this.mBody.MoveTo(iPosition, identity);
			this.mLastPosition = iPosition;
			this.mRadius = iRadius;
			this.mShieldType = iShieldType;
			float num = 3.1415927f;
			if (this.mShieldType == ShieldType.DISC)
			{
				num = 0.7853982f;
			}
			(this.mCollision.GetPrimitiveLocal(0) as HollowSphere).MaxAngle = num;
			(this.mCollision.GetPrimitiveNewWorld(0) as HollowSphere).MaxAngle = num;
			(this.mCollision.GetPrimitiveOldWorld(0) as HollowSphere).MaxAngle = num;
			(this.mCollision.GetPrimitiveLocal(0) as HollowSphere).Radius = iRadius;
			(this.mCollision.GetPrimitiveNewWorld(0) as HollowSphere).Radius = iRadius;
			(this.mCollision.GetPrimitiveOldWorld(0) as HollowSphere).Radius = iRadius;
			this.mCollision.UpdateWorldBoundingBox();
			base.Initialize();
			base.AudioEmitter.Position = iPosition;
			base.AudioEmitter.Forward = iDirection;
			base.AudioEmitter.Up = Vector3.Up;
			for (int i = 0; i < this.mDamagePoints.Length; i++)
			{
				this.mDamagePoints[i] = default(Vector4);
			}
			this.mNoiseOffset2 = default(Vector2);
			this.mTextureScale = new Vector2((float)Math.Ceiling((double)(iRadius * 0.666f)));
			this.mTint.X = iColor.X;
			this.mTint.Y = iColor.Y;
			this.mTint.Z = iColor.Z;
			this.mTint.W = 4f;
			this.mTargetTint = this.mTint;
			this.mTint.W = 0f;
			Matrix mRotation;
			Matrix.CreateRotationX(MathHelper.ToRadians(-40f), out mRotation);
			this.mCue = AudioManager.Instance.GetCue(Banks.Spells, Shield.SOUNDHASH);
			this.mCue.SetVariable(Shield.HEALTH_VAR_NAME, this.mHitPoints / this.mMaxHitPoints);
			this.mCue.Apply3D(this.mPlayState.Camera.Listener, base.AudioEmitter);
			this.mCue.Play();
			this.mIntersectKillTimer = 0f;
			for (int j = 0; j < 3; j++)
			{
				Shield.RenderData renderData = this.mRenderData[j];
				renderData.mBoundingSphere.Center = iPosition;
				renderData.mBoundingSphere.Radius = iRadius;
				renderData.mMinDotProduct = (float)Math.Cos((double)num);
				renderData.mDirection = iDirection;
				renderData.mDrawBack = (iShieldType != ShieldType.WALL);
				switch (this.mShieldType)
				{
				default:
					renderData.mVertices = Shield.sSphereVertices;
					renderData.mIndices = Shield.sSphereIndices;
					renderData.mNumVertices = Shield.sSphereNumVertices;
					renderData.mNumPrimitives = Shield.sSphereNumPrimitives;
					renderData.mRotation = mRotation;
					renderData.mTechnique = ShieldEffect.Technique.Sphere;
					break;
				case ShieldType.WALL:
					renderData.mVertices = Shield.sWallVertices;
					renderData.mIndices = Shield.sWallIndices;
					renderData.mNumVertices = Shield.sWallNumVertices;
					renderData.mNumPrimitives = Shield.sWallNumPrimitives;
					renderData.mRotation = Matrix.Identity;
					renderData.mTechnique = ShieldEffect.Technique.Wall;
					break;
				}
			}
			switch (this.mShieldType)
			{
			default:
				this.mCollision.EnablePrimitive(0);
				this.mCollision.DisablePrimitive(1);
				break;
			case ShieldType.WALL:
				this.mCollision.DisablePrimitive(0);
				this.mCollision.EnablePrimitive(1);
				break;
			}
			List<Shield> shields = this.mPlayState.EntityManager.Shields;
			Vector2 vector = new Vector2(iPosition.X, iPosition.Z);
			Vector2 value = new Vector2(iDirection.X, iDirection.Z);
			value.Normalize();
			for (int k = 0; k < shields.Count; k++)
			{
				Shield shield = shields[k];
				if (shield != this)
				{
					Vector2 vector2 = default(Vector2);
					vector2.X = shield.Position.X;
					vector2.Y = shield.Position.Z;
					Vector2 value2 = default(Vector2);
					value2.X = shield.Body.Orientation.Forward.X;
					value2.Y = shield.Body.Orientation.Forward.Z;
					if ((this.mShieldType == ShieldType.SPHERE | this.mShieldType == ShieldType.DISC) & (shield.mShieldType == ShieldType.SPHERE | shield.mShieldType == ShieldType.DISC))
					{
						float iMaxAngleB = (shields[k].mShieldType == ShieldType.SPHERE) ? 3.1415927f : 0.7853982f;
						if (Shield.CircleCircleIntersect(ref vector, ref value, this.mRadius, num, ref vector2, ref value2, shield.mRadius, iMaxAngleB))
						{
							shield.Kill();
							this.mIntersectKillTimer = 0.25f;
						}
					}
					else if (this.mShieldType == ShieldType.WALL && shield.mShieldType == ShieldType.WALL)
					{
						Vector2 value3 = new Vector2(iPosition.X, iPosition.Z);
						Vector2 vector3 = value3 + value * 6f;
						value3 -= value * 6f;
						Vector2 value4 = new Vector2(shield.Position.X, shield.Position.Z);
						Vector2 vector4 = value4 + value2 * 6f;
						value4 -= value2 * 6f;
						if (Shield.SegmentSegmentIntersect(ref value3, ref vector3, ref value4, ref vector4))
						{
							shield.Kill();
							this.mIntersectKillTimer = 0.25f;
						}
					}
					else
					{
						Segment segment;
						if (this.mShieldType == ShieldType.WALL)
						{
							segment.Origin = iPosition - iDirection * 6f;
							segment.Delta = iDirection * 6f * 2f;
						}
						else
						{
							if (shield.mShieldType != ShieldType.WALL)
							{
								throw new Exception("Invalid ShieldType!");
							}
							segment.Origin = shield.mBody.Position - shield.mBody.Orientation.Forward * 6f;
							segment.Delta = shield.mBody.Orientation.Forward * 6f * 2f;
						}
						Vector3 vector5;
						Vector3 vector6;
						float mRadius;
						float iMaxAngle;
						if (this.mShieldType == ShieldType.DISC || this.mShieldType == ShieldType.SPHERE)
						{
							vector5 = iPosition;
							vector6 = iDirection;
							mRadius = this.mRadius;
							iMaxAngle = num;
						}
						else
						{
							if (shield.mShieldType != ShieldType.DISC && shield.mShieldType != ShieldType.SPHERE)
							{
								throw new Exception("Invalid ShieldType!");
							}
							vector5 = shield.Body.Position;
							vector6 = shield.Body.Orientation.Forward;
							mRadius = shield.mRadius;
							iMaxAngle = ((shield.mShieldType == ShieldType.SPHERE) ? 3.1415927f : 0.7853982f);
						}
						if (Shield.SegmentCircleIntersect(ref segment, ref vector5, ref vector6, mRadius, iMaxAngle))
						{
							shield.Kill();
							this.mIntersectKillTimer = 0.25f;
						}
					}
				}
			}
		}

		// Token: 0x06002FBA RID: 12218 RVA: 0x00183518 File Offset: 0x00181718
		private static bool CircleCircleIntersect(ref Vector2 iCenterA, ref Vector2 iDirA, float iRadiusA, float iMaxAngleA, ref Vector2 iCenterB, ref Vector2 iDirB, float iRadiusB, float iMaxAngleB)
		{
			float num;
			Vector2.Distance(ref iCenterA, ref iCenterB, out num);
			if (num + Math.Abs(iRadiusA - iRadiusB) <= 1E-45f)
			{
				if (MagickaMath.Angle(ref iDirA, ref iDirB) < iMaxAngleA + iMaxAngleB)
				{
					return true;
				}
			}
			else if (num < iRadiusB + iRadiusA)
			{
				float num2 = (iRadiusA * iRadiusA - iRadiusB * iRadiusB + num * num) / (2f * num);
				float num3 = (float)Math.Sqrt((double)(iRadiusA * iRadiusA - num2 * num2));
				Vector2 vector;
				Vector2.Subtract(ref iCenterB, ref iCenterA, out vector);
				Vector2.Multiply(ref vector, num2, out vector);
				Vector2.Divide(ref vector, num, out vector);
				Vector2.Add(ref iCenterA, ref vector, out vector);
				Vector2 vector2 = new Vector2(vector.X + num3 * (iCenterB.Y - iCenterA.Y) / num, vector.Y - num3 * (iCenterB.X - iCenterA.X) / num);
				Vector2 vector3 = new Vector2(vector.X - num3 * (iCenterB.Y - iCenterA.Y) / num, vector.Y + num3 * (iCenterB.X - iCenterA.X) / num);
				Vector2 vector4;
				Vector2.Subtract(ref vector2, ref iCenterB, out vector4);
				vector4.Normalize();
				Vector2 vector5;
				Vector2.Subtract(ref vector3, ref iCenterB, out vector5);
				vector5.Normalize();
				Vector2 vector6;
				Vector2.Subtract(ref vector2, ref iCenterA, out vector6);
				vector6.Normalize();
				Vector2 vector7;
				Vector2.Subtract(ref vector3, ref iCenterA, out vector7);
				vector7.Normalize();
				float num4 = MagickaMath.Angle(ref iDirB, ref vector4);
				float num5 = MagickaMath.Angle(ref iDirB, ref vector5);
				float num6 = MagickaMath.Angle(ref iDirA, ref vector6);
				float num7 = MagickaMath.Angle(ref iDirA, ref vector7);
				if ((num4 <= iMaxAngleB | num5 <= iMaxAngleB) && (num6 <= iMaxAngleA | num7 <= iMaxAngleA))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06002FBB RID: 12219 RVA: 0x001836C8 File Offset: 0x001818C8
		private static bool SegmentSegmentIntersect(ref Vector2 iStartA, ref Vector2 iEndA, ref Vector2 iStartB, ref Vector2 iEndB)
		{
			Segment seg = default(Segment);
			seg.Origin.X = iStartA.X;
			seg.Origin.Y = iStartA.Y;
			seg.Delta.X = iEndA.X - iStartA.X;
			seg.Delta.Y = iEndA.Y - iStartA.Y;
			Segment seg2 = default(Segment);
			seg2.Origin.X = iStartB.X;
			seg2.Origin.Y = iStartB.Y;
			seg2.Delta.X = iEndB.X - iStartB.X;
			seg2.Delta.Y = iEndB.Y - iStartB.Y;
			float num;
			float num2;
			return Distance.SegmentSegmentDistanceSq(out num, out num2, seg, seg2) <= 0.25f;
		}

		// Token: 0x06002FBC RID: 12220 RVA: 0x001837A8 File Offset: 0x001819A8
		private static bool SegmentCircleIntersect(ref Segment iSeg, ref Vector3 iCenter, ref Vector3 iDirection, float iRadius, float iMaxAngle)
		{
			Vector3 vector = iCenter;
			Vector3.Subtract(ref vector, ref iSeg.Origin, out vector);
			float num;
			Vector3.Dot(ref iSeg.Delta, ref iSeg.Delta, out num);
			float num2;
			Vector3.Dot(ref iSeg.Delta, ref vector, out num2);
			float num3;
			Vector3.Dot(ref vector, ref vector, out num3);
			num3 -= iRadius * iRadius;
			float num4 = (float)Math.Sqrt((double)(num2 * num2 - num * num3));
			float num5 = 1f / num;
			float num6 = (num2 + num4) * num5;
			float num7 = (num2 - num4) * num5;
			bool flag = num6 > 0f & num6 < 1f & !float.IsNaN(num6);
			bool flag2 = num7 > 0f & num7 < 1f & !float.IsNaN(num7);
			if (!flag & !flag2)
			{
				return false;
			}
			Vector3 vector2;
			iSeg.GetPoint(num6, out vector2);
			Vector3.Subtract(ref vector2, ref iCenter, out vector2);
			vector2.Normalize();
			float num8;
			Vector3.Dot(ref vector2, ref iDirection, out num8);
			num8 = (float)Math.Acos((double)num8);
			Vector3 vector3;
			iSeg.GetPoint(num7, out vector3);
			Vector3.Subtract(ref vector3, ref iCenter, out vector3);
			vector3.Normalize();
			float num9;
			Vector3.Dot(ref vector3, ref iDirection, out num9);
			num9 = (float)Math.Acos((double)num9);
			if (num8 > iMaxAngle)
			{
				flag = false;
			}
			if (num9 > iMaxAngle)
			{
				flag2 = false;
			}
			return (flag & (num6 < num7 | !flag2)) || (flag2 & (num7 < num6 | !flag));
		}

		// Token: 0x06002FBD RID: 12221 RVA: 0x00183914 File Offset: 0x00181B14
		public override Matrix GetOrientation()
		{
			Matrix orientation = this.mBody.Orientation;
			if (this.mShieldType != ShieldType.WALL)
			{
				orientation.M11 *= this.mRadius;
				orientation.M12 *= this.mRadius;
				orientation.M13 *= this.mRadius;
				orientation.M21 *= this.mRadius;
				orientation.M22 *= this.mRadius;
				orientation.M23 *= this.mRadius;
				orientation.M31 *= this.mRadius;
				orientation.M32 *= this.mRadius;
				orientation.M33 *= this.mRadius;
			}
			orientation.Translation = this.mBody.Position;
			return orientation;
		}

		// Token: 0x06002FBE RID: 12222 RVA: 0x00183A00 File Offset: 0x00181C00
		public float ResistanceAgainst(Elements iElement)
		{
			float num = 0f;
			float num2 = 0f;
			float num3 = MathHelper.Clamp(num / 300f + num2, -1f, 1f);
			return 1f - num3;
		}

		// Token: 0x06002FBF RID: 12223 RVA: 0x00183A3C File Offset: 0x00181C3C
		public override void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			base.Update(iDataChannel, iDeltaTime);
			this.mDamageTimer -= iDeltaTime;
			float num = 25f;
			if (this.mIntersectKillTimer > 0f)
			{
				this.mIntersectKillTimer -= iDeltaTime;
				if (this.mIntersectKillTimer < 0f)
				{
					this.mHitPoints = 0f;
				}
			}
			while (this.mDamageTimer < 0f)
			{
				this.mDamageTimer += 0.1f;
				this.mHitPoints -= 10f;
			}
			if (this.mHitPoints <= 0f)
			{
				num = 10f;
				this.mTargetTint.W = 0f;
				if (this.mTint.W <= 0.01f)
				{
					this.mDead = true;
				}
			}
			else if (this.mTint.W > 3.8f)
			{
				this.mTargetTint.W = 1f;
			}
			else
			{
				this.mTargetTint.W = 1f;
			}
			if (this.mHitPoints > this.mMaxHitPoints)
			{
				this.mHitPoints = this.mMaxHitPoints;
			}
			if (this.mBody.Velocity.LengthSquared() > 1E-06f)
			{
				this.mRestingTimer = 1f;
			}
			else
			{
				this.mRestingTimer -= iDeltaTime;
			}
			this.mLastPosition = this.mBody.Position;
			this.mCue.SetVariable(Shield.HEALTH_VAR_NAME, this.mHitPoints / this.mMaxHitPoints);
			this.mCue.Apply3D(this.mPlayState.Camera.Listener, base.AudioEmitter);
			this.mTint.W = this.mTint.W + (this.mTargetTint.W - this.mTint.W) * iDeltaTime * num;
			Vector3 position = this.Position;
			Vector2 value = default(Vector2);
			if (this.mShieldType == ShieldType.SPHERE)
			{
				position.Z += this.mRadius;
				value.Y = 12f;
			}
			else if (this.mShieldType == ShieldType.DISC)
			{
				Vector3 forward = this.mBody.Orientation.Forward;
				Vector3 right = Vector3.Right;
				float num2;
				Vector3.Dot(ref forward, ref right, out num2);
				num2 = Math.Abs(num2);
				num2 = (float)Math.Pow((double)num2, 2.0);
				position.X += forward.X * this.mRadius;
				position.Z += forward.Z * this.mRadius + num2 * this.mRadius * 0.666f + 0.5f;
				value.Y = 16f;
			}
			else if (this.mShieldType == ShieldType.WALL)
			{
				Vector3 forward2 = this.mBody.Orientation.Forward;
				Vector2 vector = new Vector2(forward2.X, forward2.Z);
				Vector2 vector2 = new Vector2(1f, 0f);
				Vector2 vector3 = new Vector2(0f, 1f);
				float num3;
				Vector2.Dot(ref vector, ref vector3, out num3);
				float num4;
				Vector2.Dot(ref vector, ref vector2, out num4);
				Vector2.Multiply(ref vector, 6f * num3, out vector);
				position.X += vector.X;
				position.Z += vector.Y;
				value.Y = 24f;
			}
			Healthbars.Instance.AddHealthBar(position, this.mHitPoints / this.mMaxHitPoints, this.mRadius, 1f, 1f, false, new Vector4?(this.mTint), new Vector2?(value));
			float num5 = 0.5f * (this.mMaxHitPoints - this.mHitPoints) / this.mMaxHitPoints + 0.05f;
			this.mNoiseOffset2.Y = this.mNoiseOffset2.Y - iDeltaTime * num5;
			this.mNoiseOffset0.Y = this.mNoiseOffset0.Y + iDeltaTime * 0.3f * num5;
			this.mNoiseOffset0.X = this.mNoiseOffset0.X - iDeltaTime * 0.7f * num5;
			this.mNoiseOffset1.Y = this.mNoiseOffset1.Y + iDeltaTime * 0.7f * num5;
			this.mNoiseOffset1.X = this.mNoiseOffset1.X + iDeltaTime * 0.4f * num5;
			Shield.RenderData renderData = this.mRenderData[(int)iDataChannel];
			renderData.mNoise0Offset = this.mNoiseOffset0;
			renderData.mNoise1Offset = this.mNoiseOffset1;
			renderData.mNoise2Offset = this.mNoiseOffset2;
			renderData.mTextureScale = this.mTextureScale;
			renderData.mTint = this.mTint;
			renderData.mTransform = this.GetOrientation();
			this.mDamagePoints.CopyTo(renderData.mDamagePoints, 0);
			this.mPlayState.Scene.AddPostEffect(iDataChannel, renderData);
			for (int i = 0; i < this.mDamagePoints.Length; i++)
			{
				Vector4[] array = this.mDamagePoints;
				int num6 = i;
				array[num6].W = array[num6].W - iDeltaTime * 50f;
			}
		}

		// Token: 0x17000B55 RID: 2901
		// (get) Token: 0x06002FC0 RID: 12224 RVA: 0x00183F50 File Offset: 0x00182150
		// (set) Token: 0x06002FC1 RID: 12225 RVA: 0x00183F58 File Offset: 0x00182158
		public new float Radius
		{
			get
			{
				return base.Radius;
			}
			set
			{
				this.mRadius = value;
				(this.mBody.CollisionSkin.GetPrimitiveLocal(0) as HollowSphere).Radius = this.mRadius;
				(this.mBody.CollisionSkin.GetPrimitiveNewWorld(0) as HollowSphere).Radius = this.mRadius;
				(this.mBody.CollisionSkin.GetPrimitiveOldWorld(0) as HollowSphere).Radius = this.mRadius;
				this.mBody.CollisionSkin.UpdateWorldBoundingBox();
			}
		}

		// Token: 0x06002FC2 RID: 12226 RVA: 0x00183FDF File Offset: 0x001821DF
		public override void Deinitialize()
		{
			base.Deinitialize();
			this.mCue.Stop(AudioStopOptions.AsAuthored);
			PhysicsManager.Instance.Simulator.CollisionSystem.RemoveCollisionSkin(this.mCollision);
			Shield.mCache.Add(this);
		}

		// Token: 0x06002FC3 RID: 12227 RVA: 0x00184019 File Offset: 0x00182219
		public virtual void Damage(float iDamage)
		{
			this.mHitPoints -= iDamage;
		}

		// Token: 0x17000B56 RID: 2902
		// (get) Token: 0x06002FC4 RID: 12228 RVA: 0x00184029 File Offset: 0x00182229
		public override bool Dead
		{
			get
			{
				return this.mDead;
			}
		}

		// Token: 0x17000B57 RID: 2903
		// (get) Token: 0x06002FC5 RID: 12229 RVA: 0x00184031 File Offset: 0x00182231
		public override bool Removable
		{
			get
			{
				return this.Dead;
			}
		}

		// Token: 0x17000B58 RID: 2904
		// (get) Token: 0x06002FC6 RID: 12230 RVA: 0x00184039 File Offset: 0x00182239
		public ShieldType ShieldType
		{
			get
			{
				return this.mShieldType;
			}
		}

		// Token: 0x06002FC7 RID: 12231 RVA: 0x00184044 File Offset: 0x00182244
		public Vector3 GetNearestPosition(Vector3 iPosition)
		{
			Vector3 position = this.mBody.Position;
			Vector3.Subtract(ref iPosition, ref position, out iPosition);
			Vector3 forward;
			if (iPosition.LengthSquared() > 1E-45f)
			{
				Vector3.Normalize(ref iPosition, out forward);
				Vector3.Multiply(ref forward, this.mRadius, out forward);
			}
			else
			{
				forward = this.mBody.Orientation.Forward;
				Vector3.Multiply(ref forward, this.mRadius, out forward);
			}
			Vector3.Add(ref forward, ref position, out forward);
			return forward;
		}

		// Token: 0x06002FC8 RID: 12232 RVA: 0x001840C0 File Offset: 0x001822C0
		public bool SegmentIntersect(out Vector3 oPosition, Segment iSeg, float iSegmentRadius)
		{
			float num;
			return this.SegmentIntersect(out num, out oPosition, iSeg, iSegmentRadius);
		}

		// Token: 0x06002FC9 RID: 12233 RVA: 0x001840D8 File Offset: 0x001822D8
		public bool SegmentIntersect(out float oFrac, out Vector3 oPosition, Segment iSeg, float iSegmentRadius)
		{
			Vector3 vector;
			if (this.mShieldType == ShieldType.WALL)
			{
				return this.mCollision.GetPrimitiveNewWorld(1).SegmentIntersect(out oFrac, out oPosition, out vector, ref iSeg);
			}
			return this.mCollision.GetPrimitiveNewWorld(0).SegmentIntersect(out oFrac, out oPosition, out vector, ref iSeg);
		}

		// Token: 0x17000B59 RID: 2905
		// (get) Token: 0x06002FCA RID: 12234 RVA: 0x0018411D File Offset: 0x0018231D
		// (set) Token: 0x06002FCB RID: 12235 RVA: 0x00184125 File Offset: 0x00182325
		public float HitPoints
		{
			get
			{
				return this.mHitPoints;
			}
			set
			{
				this.mHitPoints = value;
			}
		}

		// Token: 0x17000B5A RID: 2906
		// (get) Token: 0x06002FCC RID: 12236 RVA: 0x0018412E File Offset: 0x0018232E
		public float MaxHitPoints
		{
			get
			{
				return this.mMaxHitPoints;
			}
		}

		// Token: 0x17000B5B RID: 2907
		// (get) Token: 0x06002FCD RID: 12237 RVA: 0x00184136 File Offset: 0x00182336
		public ISpellCaster Owner
		{
			get
			{
				return this.mOwner;
			}
		}

		// Token: 0x06002FCE RID: 12238 RVA: 0x00184140 File Offset: 0x00182340
		private void AddDamagePoint(Vector3 iPos, int iDamage)
		{
			for (int i = 0; i < this.mDamagePoints.Length; i++)
			{
				Vector4 vector = this.mDamagePoints[i];
				if (this.mDamagePoints[i].W <= 0f)
				{
					vector.X = iPos.X;
					vector.Y = iPos.Y;
					vector.Z = iPos.Z;
					vector.W = Math.Min(10f, (float)iDamage * 0.025f);
					this.mDamagePoints[i] = vector;
					return;
				}
				Vector3 vector2 = default(Vector3);
				vector2.X = vector.X;
				vector2.Y = vector.Y;
				vector2.Z = vector.Z;
				float num;
				Vector3.DistanceSquared(ref vector2, ref iPos, out num);
				if (num < 0.2f)
				{
					vector.W = Math.Min(10f, vector.W + (float)iDamage * 0.025f);
					this.mDamagePoints[i] = vector;
					return;
				}
			}
		}

		// Token: 0x06002FCF RID: 12239 RVA: 0x00184260 File Offset: 0x00182460
		public DamageResult InternalDamage(DamageCollection5 iDamages, Entity iAttacker, double iTimeStamp, Vector3 iAttackPosition, Defines.DamageFeatures iFeatures)
		{
			DamageResult damageResult = DamageResult.None;
			damageResult |= this.InternalDamage(iDamages.A, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
			damageResult |= this.InternalDamage(iDamages.B, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
			damageResult |= this.InternalDamage(iDamages.C, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
			damageResult |= this.InternalDamage(iDamages.D, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
			return damageResult | this.InternalDamage(iDamages.E, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
		}

		// Token: 0x06002FD0 RID: 12240 RVA: 0x001842E0 File Offset: 0x001824E0
		public DamageResult InternalDamage(Damage iDamage, Entity iAttacker, double iTimeStamp, Vector3 iAttackPosition, Defines.DamageFeatures iFeatures)
		{
			if ((short)(iDamage.AttackProperty & (AttackProperties.Damage | AttackProperties.Status)) == 0)
			{
				return DamageResult.Deflected;
			}
			if (Defines.FeatureDamage(iFeatures) && iDamage.Amount * iDamage.Magnitude >= 0f)
			{
				this.mHitPoints -= iDamage.Amount * iDamage.Magnitude;
			}
			this.AddDamagePoint(iAttackPosition, (int)iDamage.Amount);
			return DamageResult.Hit;
		}

		// Token: 0x06002FD1 RID: 12241 RVA: 0x0018434C File Offset: 0x0018254C
		public override void Kill()
		{
			this.mHitPoints = 0f;
		}

		// Token: 0x06002FD2 RID: 12242 RVA: 0x00184359 File Offset: 0x00182559
		public void Kill(float iIntersectTimer)
		{
			this.mIntersectKillTimer = iIntersectTimer;
		}

		// Token: 0x06002FD3 RID: 12243 RVA: 0x00184364 File Offset: 0x00182564
		public override bool ArcIntersect(out Vector3 oPosition, Vector3 iOrigin, Vector3 iDirection, float iRange, float iAngle, float iHeightDifference)
		{
			Segment iSeg;
			iSeg.Origin = iOrigin;
			Vector3.Multiply(ref iDirection, iRange, out iSeg.Delta);
			return this.SegmentIntersect(out oPosition, iSeg, 0.5f);
		}

		// Token: 0x06002FD4 RID: 12244 RVA: 0x00184396 File Offset: 0x00182596
		public void OverKill()
		{
			this.mHitPoints = -this.mMaxHitPoints;
		}

		// Token: 0x06002FD5 RID: 12245 RVA: 0x001843A5 File Offset: 0x001825A5
		protected override void INetworkUpdate(ref EntityUpdateMessage iMsg)
		{
			this.mHitPoints = iMsg.HitPoints;
			base.INetworkUpdate(ref iMsg);
		}

		// Token: 0x06002FD6 RID: 12246 RVA: 0x001843BC File Offset: 0x001825BC
		protected override void IGetNetworkUpdate(out EntityUpdateMessage oMsg, float iPrediction)
		{
			oMsg = default(EntityUpdateMessage);
			if (!this.Resting)
			{
				oMsg.Features |= EntityFeatures.Position;
				oMsg.Position = this.Position;
				oMsg.Features |= EntityFeatures.Orientation;
				Matrix orientation = this.mBody.Orientation;
				Quaternion.CreateFromRotationMatrix(ref orientation, out oMsg.Orientation);
			}
			oMsg.Features |= EntityFeatures.Damageable;
			oMsg.HitPoints = this.mHitPoints;
		}

		// Token: 0x06002FD7 RID: 12247 RVA: 0x00184437 File Offset: 0x00182637
		public void Electrocute(IDamageable iTarget, float iMultiplyer)
		{
		}

		// Token: 0x040033A2 RID: 13218
		private const int HEIGHTDIVISIONS = 32;

		// Token: 0x040033A3 RID: 13219
		private const int AXISDIVISIONS = 64;

		// Token: 0x040033A4 RID: 13220
		private const float WALL_LENGTH = 6f;

		// Token: 0x040033A5 RID: 13221
		private const float WALL_HEIGHT = 3f;

		// Token: 0x040033A6 RID: 13222
		private const float WALL_WIDTH = 0.2f;

		// Token: 0x040033A7 RID: 13223
		private static readonly int SOUNDHASH = "spell_shield".GetHashCodeCustom();

		// Token: 0x040033A8 RID: 13224
		private static readonly string HEALTH_VAR_NAME = "Health";

		// Token: 0x040033A9 RID: 13225
		private static List<Shield> mCache;

		// Token: 0x040033AA RID: 13226
		protected float mHitPoints;

		// Token: 0x040033AB RID: 13227
		protected float mMaxHitPoints;

		// Token: 0x040033AC RID: 13228
		protected ISpellCaster mOwner;

		// Token: 0x040033AD RID: 13229
		protected float mDamageTimer;

		// Token: 0x040033AE RID: 13230
		protected ShieldType mShieldType;

		// Token: 0x040033AF RID: 13231
		private Vector2 mNoiseOffset0;

		// Token: 0x040033B0 RID: 13232
		private Vector2 mNoiseOffset1;

		// Token: 0x040033B1 RID: 13233
		private Vector2 mNoiseOffset2;

		// Token: 0x040033B2 RID: 13234
		private Vector2 mTextureScale;

		// Token: 0x040033B3 RID: 13235
		private Vector4 mTint = Vector4.One;

		// Token: 0x040033B4 RID: 13236
		private Vector4 mTargetTint = Vector4.One;

		// Token: 0x040033B5 RID: 13237
		private static ShieldEffect mEffect;

		// Token: 0x040033B6 RID: 13238
		private static VertexDeclaration sVertexDeclaration;

		// Token: 0x040033B7 RID: 13239
		private static VertexBuffer sSphereVertices;

		// Token: 0x040033B8 RID: 13240
		private static IndexBuffer sSphereIndices;

		// Token: 0x040033B9 RID: 13241
		private static int sSphereNumVertices;

		// Token: 0x040033BA RID: 13242
		private static int sSphereNumPrimitives;

		// Token: 0x040033BB RID: 13243
		private static VertexBuffer sWallVertices;

		// Token: 0x040033BC RID: 13244
		private static IndexBuffer sWallIndices;

		// Token: 0x040033BD RID: 13245
		private static int sWallNumVertices;

		// Token: 0x040033BE RID: 13246
		private static int sWallNumPrimitives;

		// Token: 0x040033BF RID: 13247
		private Shield.RenderData[] mRenderData;

		// Token: 0x040033C0 RID: 13248
		private Vector4[] mDamagePoints;

		// Token: 0x040033C1 RID: 13249
		private Cue mCue;

		// Token: 0x040033C2 RID: 13250
		protected float mRestingTimer = 1f;

		// Token: 0x040033C3 RID: 13251
		private float mIntersectKillTimer;

		// Token: 0x040033C4 RID: 13252
		private Vector3 mLastPosition;

		// Token: 0x0200062F RID: 1583
		private class RenderData : IPostEffect
		{
			// Token: 0x06002FD9 RID: 12249 RVA: 0x00184454 File Offset: 0x00182654
			public RenderData()
			{
				this.mDamagePoints = new Vector4[16];
			}

			// Token: 0x17000B5C RID: 2908
			// (get) Token: 0x06002FDA RID: 12250 RVA: 0x00184469 File Offset: 0x00182669
			public int ZIndex
			{
				get
				{
					return 90;
				}
			}

			// Token: 0x06002FDB RID: 12251 RVA: 0x00184470 File Offset: 0x00182670
			public void Draw(float iDeltaTime, ref Vector2 iPixelSize, ref Matrix iViewMatrix, ref Matrix iProjectionMatrix, Texture2D iCandidate, Texture2D iDepthMap, Texture2D iNormalMap)
			{
				Shield.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertices, 0, VertexPositionNormalTexture.SizeInBytes);
				Shield.mEffect.GraphicsDevice.Indices = this.mIndices;
				Shield.mEffect.GraphicsDevice.VertexDeclaration = Shield.sVertexDeclaration;
				Shield.mEffect.View = iViewMatrix;
				Shield.mEffect.Projection = iProjectionMatrix;
				Shield.mEffect.Thickness = 0.2f;
				Shield.mEffect.ColorTint = this.mTint;
				Shield.mEffect.DamagePoints = this.mDamagePoints;
				Shield.mEffect.TextureScale = this.mTextureScale;
				Shield.mEffect.Noise0Offset = this.mNoise0Offset;
				Shield.mEffect.Noise1Offset = this.mNoise1Offset;
				Shield.mEffect.Noise2Offset = this.mNoise2Offset;
				Shield.mEffect.MinDotProduct = this.mMinDotProduct;
				Shield.mEffect.Direction = this.mDirection;
				Shield.mEffect.DepthMap = iDepthMap;
				Matrix world;
				Matrix.Multiply(ref this.mTransform, ref this.mRotation, out world);
				world.Translation = this.mTransform.Translation;
				Shield.mEffect.World = world;
				Shield.mEffect.SetTechnique(this.mTechnique);
				Shield.mEffect.Begin();
				Shield.mEffect.CurrentTechnique.Passes[0].Begin();
				Shield.mEffect.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, 0, 0, this.mNumVertices, 0, this.mNumPrimitives);
				Shield.mEffect.CurrentTechnique.Passes[0].End();
				Shield.mEffect.End();
			}

			// Token: 0x040033C5 RID: 13253
			public bool mDrawBack;

			// Token: 0x040033C6 RID: 13254
			public Matrix mTransform;

			// Token: 0x040033C7 RID: 13255
			public Vector2 mNoise0Offset;

			// Token: 0x040033C8 RID: 13256
			public Vector2 mNoise1Offset;

			// Token: 0x040033C9 RID: 13257
			public Vector2 mNoise2Offset;

			// Token: 0x040033CA RID: 13258
			public Vector2 mTextureScale;

			// Token: 0x040033CB RID: 13259
			public Vector4 mTint;

			// Token: 0x040033CC RID: 13260
			public float mMinDotProduct;

			// Token: 0x040033CD RID: 13261
			public ShieldEffect.Technique mTechnique;

			// Token: 0x040033CE RID: 13262
			public Vector3 mDirection;

			// Token: 0x040033CF RID: 13263
			public Matrix mRotation;

			// Token: 0x040033D0 RID: 13264
			public int mNumVertices;

			// Token: 0x040033D1 RID: 13265
			public int mNumPrimitives;

			// Token: 0x040033D2 RID: 13266
			public IndexBuffer mIndices;

			// Token: 0x040033D3 RID: 13267
			public VertexBuffer mVertices;

			// Token: 0x040033D4 RID: 13268
			public Vector4[] mDamagePoints;

			// Token: 0x040033D5 RID: 13269
			public BoundingSphere mBoundingSphere;
		}
	}
}
