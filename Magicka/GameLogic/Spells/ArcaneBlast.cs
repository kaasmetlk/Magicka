using System;
using System.Collections.Generic;
using Magicka.GameLogic.Entities;
using Magicka.Graphics;
using Magicka.Graphics.Lights;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;

namespace Magicka.GameLogic.Spells
{
	// Token: 0x02000572 RID: 1394
	internal class ArcaneBlast : IAbilityEffect
	{
		// Token: 0x06002997 RID: 10647 RVA: 0x001461B4 File Offset: 0x001443B4
		public static ArcaneBlast GetInstance()
		{
			if (ArcaneBlast.sCache.Count > 0)
			{
				ArcaneBlast result = ArcaneBlast.sCache[ArcaneBlast.sCache.Count - 1];
				ArcaneBlast.sCache.RemoveAt(ArcaneBlast.sCache.Count - 1);
				return result;
			}
			return new ArcaneBlast();
		}

		// Token: 0x06002998 RID: 10648 RVA: 0x00146204 File Offset: 0x00144404
		public static void InitializeCache(int iNr)
		{
			ArcaneBlast.sCache = new List<ArcaneBlast>(iNr);
			for (int i = 0; i < iNr; i++)
			{
				ArcaneBlast.sCache.Add(new ArcaneBlast());
			}
		}

		// Token: 0x06002999 RID: 10649 RVA: 0x00146238 File Offset: 0x00144438
		private ArcaneBlast()
		{
			if (ArcaneBlast.sVertices == null)
			{
				GraphicsDevice graphicsDevice = Game.Instance.GraphicsDevice;
				VertexPositionTexture[] array = new VertexPositionTexture[4];
				array[0].Position.X = -1f;
				array[0].Position.Z = -1f;
				array[0].TextureCoordinate.X = 0f;
				array[0].TextureCoordinate.Y = 0f;
				array[1].Position.X = 1f;
				array[1].Position.Z = -1f;
				array[1].TextureCoordinate.X = 1f;
				array[1].TextureCoordinate.Y = 0f;
				array[2].Position.X = 1f;
				array[2].Position.Z = 1f;
				array[2].TextureCoordinate.X = 1f;
				array[2].TextureCoordinate.Y = 1f;
				array[3].Position.X = -1f;
				array[3].Position.Z = 1f;
				array[3].TextureCoordinate.X = 0f;
				array[3].TextureCoordinate.Y = 1f;
				lock (graphicsDevice)
				{
					ArcaneBlast.sTexture = Game.Instance.Content.Load<Texture2D>("EffectTextures/ArcaneDisc");
					ArcaneBlast.sVertices = new VertexBuffer(graphicsDevice, 4 * VertexPositionTexture.SizeInBytes, BufferUsage.WriteOnly);
					ArcaneBlast.sVertices.SetData<VertexPositionTexture>(array);
					ArcaneBlast.sVertexDeclaration = new VertexDeclaration(graphicsDevice, VertexPositionTexture.VertexElements);
				}
				ArcaneBlast.sVerticesHash = ArcaneBlast.sVertices.GetHashCode();
			}
			this.mRenderData = new ArcaneBlast.RenderData[3];
			for (int i = 0; i < 3; i++)
			{
				ArcaneBlast.RenderData renderData = new ArcaneBlast.RenderData();
				this.mRenderData[i] = renderData;
			}
		}

		// Token: 0x0600299A RID: 10650 RVA: 0x00146468 File Offset: 0x00144668
		public void Initialize(Entity iOwner, Vector3 iPosition, Vector3 iColor, float iRadius, Elements iElements)
		{
			this.mElement = iElements;
			this.mMaxRadius = iRadius;
			this.mTTL = 1f;
			this.mPosition = iPosition;
			this.mColor = iColor;
			this.mOwner = iOwner;
			Vector3 forward = Vector3.Forward;
			for (int i = 0; i < 11; i++)
			{
				if ((this.mElement & Defines.ElementFromIndex(i)) != Elements.None)
				{
					VisualEffectReference visualEffectReference;
					EffectManager.Instance.StartEffect(ArcaneBlast.EFFECT_BEAM[i], ref this.mPosition, ref forward, out visualEffectReference);
				}
			}
			SpellManager.Instance.AddSpellEffect(this);
			DynamicLight cachedLight = DynamicLight.GetCachedLight();
			cachedLight.Initialize(iPosition, iColor, 2f, iRadius * 2f, 2f, 1f, this.mTTL * 0.25f);
			cachedLight.Enable();
		}

		// Token: 0x170009CC RID: 2508
		// (get) Token: 0x0600299B RID: 10651 RVA: 0x00146525 File Offset: 0x00144725
		public bool IsDead
		{
			get
			{
				return this.mTTL <= -1f;
			}
		}

		// Token: 0x0600299C RID: 10652 RVA: 0x00146538 File Offset: 0x00144738
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			this.mTTL -= iDeltaTime * 5f;
			ArcaneBlast.RenderData renderData = this.mRenderData[(int)iDataChannel];
			this.mRotation = MathHelper.WrapAngle(this.mRotation + iDeltaTime * 4f);
			renderData.BoundingSphere.Center = this.mPosition;
			renderData.BoundingSphere.Radius = (1f - this.mTTL) * this.mMaxRadius;
			renderData.ColorEdge = this.mColor;
			Vector3.Multiply(ref this.mColor, 1f, out renderData.ColorCenter);
			renderData.Element = this.mElement;
			Matrix.CreateRotationY(this.mRotation, out renderData.Transform0);
			MagickaMath.UniformMatrixScale(ref renderData.Transform0, renderData.BoundingSphere.Radius);
			renderData.Transform0.Translation = this.mPosition;
			Matrix.CreateRotationY(-this.mRotation, out renderData.Transform1);
			MagickaMath.UniformMatrixScale(ref renderData.Transform1, renderData.BoundingSphere.Radius * 0.9f);
			renderData.Transform1.Translation = this.mPosition;
			renderData.Alpha = Math.Min(1f, this.mTTL * 4f + 1f);
			this.mOwner.PlayState.Scene.AddRenderableAdditiveObject(iDataChannel, renderData);
		}

		// Token: 0x0600299D RID: 10653 RVA: 0x00146688 File Offset: 0x00144888
		public void OnRemove()
		{
			ArcaneBlast.sCache.Add(this);
		}

		// Token: 0x04002CFF RID: 11519
		private static List<ArcaneBlast> sCache;

		// Token: 0x04002D00 RID: 11520
		public static readonly int[] EFFECT_BEAM = new int[]
		{
			0,
			"beam_water_explosion".GetHashCodeCustom(),
			"beam_cold_explosion".GetHashCodeCustom(),
			"beam_fire_explosion".GetHashCodeCustom(),
			"beam_lightning_explosion".GetHashCodeCustom(),
			"beam_arcane_explosion".GetHashCodeCustom(),
			"beam_life_explosion".GetHashCodeCustom(),
			0,
			0,
			"beam_steam_explosion".GetHashCodeCustom(),
			"beam_poison_explosion".GetHashCodeCustom()
		};

		// Token: 0x04002D01 RID: 11521
		private static int sVerticesHash;

		// Token: 0x04002D02 RID: 11522
		private static VertexBuffer sVertices;

		// Token: 0x04002D03 RID: 11523
		private static VertexDeclaration sVertexDeclaration;

		// Token: 0x04002D04 RID: 11524
		private static Texture2D sTexture;

		// Token: 0x04002D05 RID: 11525
		private ArcaneBlast.RenderData[] mRenderData;

		// Token: 0x04002D06 RID: 11526
		private float mRotation;

		// Token: 0x04002D07 RID: 11527
		private float mTTL;

		// Token: 0x04002D08 RID: 11528
		private float mMaxRadius;

		// Token: 0x04002D09 RID: 11529
		private Elements mElement;

		// Token: 0x04002D0A RID: 11530
		private Entity mOwner;

		// Token: 0x04002D0B RID: 11531
		private Vector3 mPosition;

		// Token: 0x04002D0C RID: 11532
		private Vector3 mColor;

		// Token: 0x02000573 RID: 1395
		protected class RenderData : IRenderableAdditiveObject
		{
			// Token: 0x170009CD RID: 2509
			// (get) Token: 0x0600299F RID: 10655 RVA: 0x0014671D File Offset: 0x0014491D
			public int Effect
			{
				get
				{
					return AdditiveEffect.TYPEHASH;
				}
			}

			// Token: 0x170009CE RID: 2510
			// (get) Token: 0x060029A0 RID: 10656 RVA: 0x00146724 File Offset: 0x00144924
			public int Technique
			{
				get
				{
					return 0;
				}
			}

			// Token: 0x170009CF RID: 2511
			// (get) Token: 0x060029A1 RID: 10657 RVA: 0x00146727 File Offset: 0x00144927
			public VertexBuffer Vertices
			{
				get
				{
					return ArcaneBlast.sVertices;
				}
			}

			// Token: 0x170009D0 RID: 2512
			// (get) Token: 0x060029A2 RID: 10658 RVA: 0x0014672E File Offset: 0x0014492E
			public int VerticesHashCode
			{
				get
				{
					return ArcaneBlast.sVerticesHash;
				}
			}

			// Token: 0x170009D1 RID: 2513
			// (get) Token: 0x060029A3 RID: 10659 RVA: 0x00146735 File Offset: 0x00144935
			public int VertexStride
			{
				get
				{
					return VertexPositionTexture.SizeInBytes;
				}
			}

			// Token: 0x170009D2 RID: 2514
			// (get) Token: 0x060029A4 RID: 10660 RVA: 0x0014673C File Offset: 0x0014493C
			public IndexBuffer Indices
			{
				get
				{
					return null;
				}
			}

			// Token: 0x170009D3 RID: 2515
			// (get) Token: 0x060029A5 RID: 10661 RVA: 0x0014673F File Offset: 0x0014493F
			public VertexDeclaration VertexDeclaration
			{
				get
				{
					return ArcaneBlast.sVertexDeclaration;
				}
			}

			// Token: 0x060029A6 RID: 10662 RVA: 0x00146746 File Offset: 0x00144946
			public bool Cull(BoundingFrustum iViewFrustum)
			{
				return this.BoundingSphere.Contains(iViewFrustum) == ContainmentType.Disjoint;
			}

			// Token: 0x060029A7 RID: 10663 RVA: 0x00146758 File Offset: 0x00144958
			public void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				AdditiveEffect additiveEffect = iEffect as AdditiveEffect;
				additiveEffect.Texture = ArcaneBlast.sTexture;
				additiveEffect.TextureEnabled = true;
				additiveEffect.TextureScale = new Vector2(1f, 0.125f);
				additiveEffect.ColorTint = new Vector4(1f, 1f, 1f, this.Alpha * 0.5f);
				additiveEffect.VertexColorEnabled = false;
				for (int i = 0; i < 11; i++)
				{
					Elements elements = this.Element & Defines.ElementFromIndex(i);
					if (elements != Elements.None)
					{
						additiveEffect.TextureOffset = Railgun.ELEMENT_OFFSET_LOOKUP[Defines.ElementIndex(elements)];
						additiveEffect.World = this.Transform0;
						iEffect.CommitChanges();
						iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
						additiveEffect.World = this.Transform1;
						iEffect.CommitChanges();
						iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
					}
				}
			}

			// Token: 0x04002D0D RID: 11533
			public BoundingSphere BoundingSphere;

			// Token: 0x04002D0E RID: 11534
			public Matrix Transform0;

			// Token: 0x04002D0F RID: 11535
			public Matrix Transform1;

			// Token: 0x04002D10 RID: 11536
			public Vector3 ColorCenter;

			// Token: 0x04002D11 RID: 11537
			public Vector3 ColorEdge;

			// Token: 0x04002D12 RID: 11538
			public float Alpha;

			// Token: 0x04002D13 RID: 11539
			public Elements Element;

			// Token: 0x04002D14 RID: 11540
			private static Vector2 sTextureSize = new Vector2(128f, 128f);
		}
	}
}
