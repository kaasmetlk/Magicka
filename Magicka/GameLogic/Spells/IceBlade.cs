using System;
using System.Collections.Generic;
using JigLibX.Geometry;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.Items;
using Magicka.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;

namespace Magicka.GameLogic.Spells
{
	// Token: 0x020000D1 RID: 209
	public class IceBlade : IAbilityEffect
	{
		// Token: 0x0600064E RID: 1614 RVA: 0x00025558 File Offset: 0x00023758
		public static IceBlade GetInstance()
		{
			if (IceBlade.sCache.Count > 0)
			{
				IceBlade result = IceBlade.sCache[IceBlade.sCache.Count - 1];
				IceBlade.sCache.RemoveAt(IceBlade.sCache.Count - 1);
				return result;
			}
			return new IceBlade();
		}

		// Token: 0x0600064F RID: 1615 RVA: 0x000255A8 File Offset: 0x000237A8
		public static void InitializeCache(int iNr)
		{
			IceBlade.sCache = new List<IceBlade>(iNr);
			for (int i = 0; i < iNr; i++)
			{
				IceBlade.sCache.Add(new IceBlade());
			}
		}

		// Token: 0x06000650 RID: 1616 RVA: 0x000255DC File Offset: 0x000237DC
		private IceBlade()
		{
			if (IceBlade.sModel == null)
			{
				GraphicsDevice graphicsDevice = Game.Instance.GraphicsDevice;
				lock (graphicsDevice)
				{
					IceBlade.sModel = Game.Instance.Content.Load<Model>("Models/Effects/Ice_Blade");
					IceBlade.sTrailTexture = Game.Instance.Content.Load<Texture2D>("EffectTextures/IceBlade");
					IceBlade.sTrailVertexDeclaration = new VertexDeclaration(graphicsDevice, VertexPositionTexture.VertexElements);
				}
				IceBlade.sBoundingSphere = IceBlade.sModel.Meshes[0].BoundingSphere;
				for (int i = 0; i < IceBlade.sModel.Bones.Count; i++)
				{
					if (IceBlade.sModel.Bones[i].Name.Equals("attach0", StringComparison.OrdinalIgnoreCase))
					{
						IceBlade.sAttach = IceBlade.sModel.Bones[i].Transform.Translation;
						break;
					}
				}
			}
			this.mTrailVertices = new VertexPositionTexture[32];
			for (int j = 0; j < this.mTrailVertices.Length; j++)
			{
				this.mTrailVertices[j].TextureCoordinate.X = (float)(j / 2) / 15f;
				this.mTrailVertices[j].TextureCoordinate.Y = (float)(j % 2);
			}
			this.mRenderData = new RenderableObject<RenderDeferredEffect, RenderDeferredMaterial>[3];
			this.mTrailRenderData = new IceBlade.TrailRenderData[3];
			for (int k = 0; k < 3; k++)
			{
				RenderableObject<RenderDeferredEffect, RenderDeferredMaterial> renderableObject = new RenderableObject<RenderDeferredEffect, RenderDeferredMaterial>();
				this.mRenderData[k] = renderableObject;
				renderableObject.SetMesh(IceBlade.sModel.Meshes[0], IceBlade.sModel.Meshes[0].MeshParts[0], 4, 0, 5);
				this.mTrailRenderData[k] = new IceBlade.TrailRenderData(32);
			}
		}

		// Token: 0x06000651 RID: 1617 RVA: 0x000257C4 File Offset: 0x000239C4
		public void Initialize(Item iOwner, ref DamageCollection5 iDamage, float iRange)
		{
			this.mDead = false;
			this.mOwner = iOwner;
			this.mDamage = iDamage;
			this.mTimeStamp = iOwner.PlayState.PlayTime;
			Matrix orientation = this.mOwner.GetOrientation();
			this.mTrailVertices[0].Position = orientation.Translation;
			Vector3.Transform(ref IceBlade.sAttach, ref orientation, out this.mTrailVertices[1].Position);
			for (int i = 2; i < 32; i += 2)
			{
				this.mTrailVertices[i].Position = this.mTrailVertices[i - 2].Position;
				this.mTrailVertices[i + 1].Position = this.mTrailVertices[i - 1].Position;
			}
			this.mRange = iRange;
			this.mScale = iRange / IceBlade.sAttach.Y;
			VisualEffectReference visualEffectReference;
			EffectManager.Instance.StartEffect(IceBlade.SPAWN_EFFECT, ref orientation, out visualEffectReference);
			SpellManager.Instance.AddSpellEffect(this);
		}

		// Token: 0x06000652 RID: 1618 RVA: 0x000258CC File Offset: 0x00023ACC
		public void Kill()
		{
			this.mDead = true;
		}

		// Token: 0x17000134 RID: 308
		// (get) Token: 0x06000653 RID: 1619 RVA: 0x000258D5 File Offset: 0x00023AD5
		public bool IsDead
		{
			get
			{
				return this.mDead;
			}
		}

		// Token: 0x06000654 RID: 1620 RVA: 0x000258E0 File Offset: 0x00023AE0
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			RenderableObject<RenderDeferredEffect, RenderDeferredMaterial> renderableObject = this.mRenderData[(int)iDataChannel];
			renderableObject.mMaterial.WorldTransform = this.mOwner.GetOrientation();
			RenderableObject<RenderDeferredEffect, RenderDeferredMaterial> renderableObject2 = renderableObject;
			renderableObject2.mMaterial.WorldTransform.M11 = renderableObject2.mMaterial.WorldTransform.M11 * this.mScale;
			RenderableObject<RenderDeferredEffect, RenderDeferredMaterial> renderableObject3 = renderableObject;
			renderableObject3.mMaterial.WorldTransform.M12 = renderableObject3.mMaterial.WorldTransform.M12 * this.mScale;
			RenderableObject<RenderDeferredEffect, RenderDeferredMaterial> renderableObject4 = renderableObject;
			renderableObject4.mMaterial.WorldTransform.M13 = renderableObject4.mMaterial.WorldTransform.M13 * this.mScale;
			RenderableObject<RenderDeferredEffect, RenderDeferredMaterial> renderableObject5 = renderableObject;
			renderableObject5.mMaterial.WorldTransform.M21 = renderableObject5.mMaterial.WorldTransform.M21 * this.mScale;
			RenderableObject<RenderDeferredEffect, RenderDeferredMaterial> renderableObject6 = renderableObject;
			renderableObject6.mMaterial.WorldTransform.M22 = renderableObject6.mMaterial.WorldTransform.M22 * this.mScale;
			RenderableObject<RenderDeferredEffect, RenderDeferredMaterial> renderableObject7 = renderableObject;
			renderableObject7.mMaterial.WorldTransform.M23 = renderableObject7.mMaterial.WorldTransform.M23 * this.mScale;
			RenderableObject<RenderDeferredEffect, RenderDeferredMaterial> renderableObject8 = renderableObject;
			renderableObject8.mMaterial.WorldTransform.M31 = renderableObject8.mMaterial.WorldTransform.M31 * this.mScale;
			RenderableObject<RenderDeferredEffect, RenderDeferredMaterial> renderableObject9 = renderableObject;
			renderableObject9.mMaterial.WorldTransform.M32 = renderableObject9.mMaterial.WorldTransform.M32 * this.mScale;
			RenderableObject<RenderDeferredEffect, RenderDeferredMaterial> renderableObject10 = renderableObject;
			renderableObject10.mMaterial.WorldTransform.M33 = renderableObject10.mMaterial.WorldTransform.M33 * this.mScale;
			renderableObject.mBoundingSphere.Radius = IceBlade.sBoundingSphere.Radius;
			Vector3.Transform(ref IceBlade.sBoundingSphere.Center, ref renderableObject.mMaterial.WorldTransform, out renderableObject.mBoundingSphere.Center);
			Vector3 position;
			Vector3.Transform(ref IceBlade.sAttach, ref renderableObject.mMaterial.WorldTransform, out position);
			Segment segment;
			segment.Origin = renderableObject.mMaterial.WorldTransform.Translation;
			Vector3.Subtract(ref position, ref segment.Origin, out segment.Delta);
			this.mTrailVertices[0].Position = segment.Origin;
			this.mTrailVertices[1].Position = position;
			double x = 1.401298464324817E-45;
			float amount = (float)Math.Pow(x, (double)iDeltaTime);
			for (int i = 2; i < 32; i += 2)
			{
				Vector3.Lerp(ref this.mTrailVertices[i - 2].Position, ref this.mTrailVertices[i].Position, amount, out this.mTrailVertices[i].Position);
				Vector3.Lerp(ref this.mTrailVertices[i - 1].Position, ref this.mTrailVertices[i + 1].Position, amount, out this.mTrailVertices[i + 1].Position);
			}
			this.mOwner.PlayState.Scene.AddRenderableObject(iDataChannel, renderableObject);
			IceBlade.TrailRenderData trailRenderData = this.mTrailRenderData[(int)iDataChannel];
			trailRenderData.BoundingSphere = renderableObject.mBoundingSphere;
			this.mTrailVertices.CopyTo(trailRenderData.VertexArray, 0);
			this.mOwner.PlayState.Scene.AddRenderableAdditiveObject(iDataChannel, trailRenderData);
		}

		// Token: 0x06000655 RID: 1621 RVA: 0x00025BC0 File Offset: 0x00023DC0
		public void OnRemove()
		{
			Magicka.GameLogic.Entities.Character owner = this.mOwner.Owner;
			if (owner != null)
			{
				Segment iSeg;
				iSeg.Origin = owner.Position;
				iSeg.Delta = owner.Direction;
				Vector3.Multiply(ref iSeg.Delta, this.mRange, out iSeg.Delta);
				Vector3 iCenter;
				iSeg.GetPoint(0.5f, out iCenter);
				List<Entity> entities = owner.PlayState.EntityManager.GetEntities(iCenter, this.mRange * 0.5f, true);
				for (int i = 0; i < entities.Count; i++)
				{
					IDamageable damageable = entities[i] as IDamageable;
					Vector3 iAttackPosition;
					if ((damageable != owner & damageable != null) && damageable.SegmentIntersect(out iAttackPosition, iSeg, 1f))
					{
						damageable.Damage(this.mDamage, owner, this.mTimeStamp, iAttackPosition);
					}
				}
				owner.PlayState.EntityManager.ReturnEntityList(entities);
			}
			IceBlade.sCache.Add(this);
			Matrix orientation = this.mOwner.GetOrientation();
			VisualEffectReference visualEffectReference;
			EffectManager.Instance.StartEffect(IceBlade.DEATH_EFFECT, ref orientation, out visualEffectReference);
		}

		// Token: 0x04000508 RID: 1288
		public const int TRAIL_VERTEX_COUNT = 32;

		// Token: 0x04000509 RID: 1289
		private static List<IceBlade> sCache;

		// Token: 0x0400050A RID: 1290
		private static readonly int SPAWN_EFFECT = "weapon_ice_spawn".GetHashCodeCustom();

		// Token: 0x0400050B RID: 1291
		private static readonly int DEATH_EFFECT = "weapon_ice_death".GetHashCodeCustom();

		// Token: 0x0400050C RID: 1292
		private static Model sModel;

		// Token: 0x0400050D RID: 1293
		private static BoundingSphere sBoundingSphere;

		// Token: 0x0400050E RID: 1294
		private static Vector3 sAttach;

		// Token: 0x0400050F RID: 1295
		private static VertexDeclaration sTrailVertexDeclaration;

		// Token: 0x04000510 RID: 1296
		private static Texture2D sTrailTexture;

		// Token: 0x04000511 RID: 1297
		private VertexPositionTexture[] mTrailVertices;

		// Token: 0x04000512 RID: 1298
		private IceBlade.TrailRenderData[] mTrailRenderData;

		// Token: 0x04000513 RID: 1299
		private RenderableObject<RenderDeferredEffect, RenderDeferredMaterial>[] mRenderData;

		// Token: 0x04000514 RID: 1300
		private Item mOwner;

		// Token: 0x04000515 RID: 1301
		private bool mDead;

		// Token: 0x04000516 RID: 1302
		private List<ushort> mHitlist = new List<ushort>(32);

		// Token: 0x04000517 RID: 1303
		private DamageCollection5 mDamage;

		// Token: 0x04000518 RID: 1304
		private float mScale;

		// Token: 0x04000519 RID: 1305
		private float mRange;

		// Token: 0x0400051A RID: 1306
		private double mTimeStamp;

		// Token: 0x020000D2 RID: 210
		protected class TrailRenderData : IRenderableAdditiveObject
		{
			// Token: 0x06000657 RID: 1623 RVA: 0x00025CFB File Offset: 0x00023EFB
			public TrailRenderData(int iNrOfVertices)
			{
				this.mVertices = new VertexPositionTexture[iNrOfVertices];
			}

			// Token: 0x17000135 RID: 309
			// (get) Token: 0x06000658 RID: 1624 RVA: 0x00025D0F File Offset: 0x00023F0F
			public VertexPositionTexture[] VertexArray
			{
				get
				{
					return this.mVertices;
				}
			}

			// Token: 0x17000136 RID: 310
			// (get) Token: 0x06000659 RID: 1625 RVA: 0x00025D17 File Offset: 0x00023F17
			public int Effect
			{
				get
				{
					return AdditiveEffect.TYPEHASH;
				}
			}

			// Token: 0x17000137 RID: 311
			// (get) Token: 0x0600065A RID: 1626 RVA: 0x00025D1E File Offset: 0x00023F1E
			public int Technique
			{
				get
				{
					return 0;
				}
			}

			// Token: 0x17000138 RID: 312
			// (get) Token: 0x0600065B RID: 1627 RVA: 0x00025D21 File Offset: 0x00023F21
			public VertexBuffer Vertices
			{
				get
				{
					return null;
				}
			}

			// Token: 0x17000139 RID: 313
			// (get) Token: 0x0600065C RID: 1628 RVA: 0x00025D24 File Offset: 0x00023F24
			public int VerticesHashCode
			{
				get
				{
					return 0;
				}
			}

			// Token: 0x1700013A RID: 314
			// (get) Token: 0x0600065D RID: 1629 RVA: 0x00025D27 File Offset: 0x00023F27
			public int VertexStride
			{
				get
				{
					return VertexPositionTexture.SizeInBytes;
				}
			}

			// Token: 0x1700013B RID: 315
			// (get) Token: 0x0600065E RID: 1630 RVA: 0x00025D2E File Offset: 0x00023F2E
			public IndexBuffer Indices
			{
				get
				{
					return null;
				}
			}

			// Token: 0x1700013C RID: 316
			// (get) Token: 0x0600065F RID: 1631 RVA: 0x00025D31 File Offset: 0x00023F31
			public VertexDeclaration VertexDeclaration
			{
				get
				{
					return IceBlade.sTrailVertexDeclaration;
				}
			}

			// Token: 0x06000660 RID: 1632 RVA: 0x00025D38 File Offset: 0x00023F38
			public bool Cull(BoundingFrustum iViewFrustum)
			{
				return this.BoundingSphere.Contains(iViewFrustum) == ContainmentType.Disjoint;
			}

			// Token: 0x06000661 RID: 1633 RVA: 0x00025D4C File Offset: 0x00023F4C
			public void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				AdditiveEffect additiveEffect = iEffect as AdditiveEffect;
				additiveEffect.ColorTint = new Vector4(1f);
				additiveEffect.TextureEnabled = true;
				additiveEffect.Texture = IceBlade.sTrailTexture;
				additiveEffect.TextureOffset = default(Vector2);
				additiveEffect.TextureScale = new Vector2(1f);
				additiveEffect.VertexColorEnabled = false;
				additiveEffect.World = Matrix.Identity;
				additiveEffect.GraphicsDevice.RenderState.CullMode = CullMode.None;
				iEffect.CommitChanges();
				iEffect.GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleStrip, this.mVertices, 0, 30);
			}

			// Token: 0x0400051B RID: 1307
			public BoundingSphere BoundingSphere;

			// Token: 0x0400051C RID: 1308
			private VertexPositionTexture[] mVertices;
		}
	}
}
