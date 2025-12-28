using System;
using System.Collections.Generic;
using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.GameStates;
using Magicka.Graphics;
using Magicka.Graphics.Effects;
using Magicka.Graphics.Lights;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;

namespace Magicka.GameLogic.Spells
{
	// Token: 0x02000574 RID: 1396
	public class ArcaneBlade : IAbilityEffect
	{
		// Token: 0x060029AA RID: 10666 RVA: 0x00146858 File Offset: 0x00144A58
		public static ArcaneBlade GetInstance()
		{
			if (ArcaneBlade.sCache.Count > 0)
			{
				ArcaneBlade result = ArcaneBlade.sCache[ArcaneBlade.sCache.Count - 1];
				ArcaneBlade.sCache.RemoveAt(ArcaneBlade.sCache.Count - 1);
				return result;
			}
			return new ArcaneBlade();
		}

		// Token: 0x060029AB RID: 10667 RVA: 0x001468A8 File Offset: 0x00144AA8
		public static void InitializeCache(int iNr)
		{
			ArcaneBlade.sCache = new List<ArcaneBlade>(iNr);
			for (int i = 0; i < iNr; i++)
			{
				ArcaneBlade.sCache.Add(new ArcaneBlade());
			}
		}

		// Token: 0x060029AC RID: 10668 RVA: 0x001468DC File Offset: 0x00144ADC
		private ArcaneBlade()
		{
			if (ArcaneBlade.sVertexDeclaration == null)
			{
				GraphicsDevice graphicsDevice = Game.Instance.GraphicsDevice;
				lock (graphicsDevice)
				{
					ArcaneBlade.sTexture = Game.Instance.Content.Load<Texture2D>("EffectTextures/ArcaneBlade");
					ArcaneBlade.sVertexDeclaration = new VertexDeclaration(graphicsDevice, VertexPositionTexture.VertexElements);
				}
			}
			this.mVertices = new VertexPositionTexture[32];
			for (int i = 0; i < this.mVertices.Length; i++)
			{
				this.mVertices[i].TextureCoordinate.X = (float)(i / 2) / 15f;
				this.mVertices[i].TextureCoordinate.Y = (float)(i % 2);
			}
			this.mRenderData = new ArcaneBlade.RenderData[3];
			for (int j = 0; j < 3; j++)
			{
				ArcaneBlade.RenderData renderData = new ArcaneBlade.RenderData(32);
				this.mRenderData[j] = renderData;
			}
			this.mLight = new CapsuleLight(Game.Instance.Content);
		}

		// Token: 0x060029AD RID: 10669 RVA: 0x001469F4 File Offset: 0x00144BF4
		public void Initialize(PlayState iPlayState, Item iItem, Elements iElements, float iRange)
		{
			this.mOwner = iItem;
			this.mPlayState = iPlayState;
			this.mDead = false;
			this.mAlpha = 1f;
			this.mRange = iRange;
			this.mMaxRange = iRange;
			Spell spell;
			Spell.DefaultSpell(iElements, out spell);
			this.mColor = spell.GetColor();
			if (this.mOwner != null)
			{
				this.mVertices[0].Position = this.mOwner.Position;
				Vector3 up = this.mOwner.Body.Orientation.Up;
				Vector3.Multiply(ref up, this.mRange, out up);
				Vector3.Add(ref this.mVertices[0].Position, ref up, out this.mVertices[1].Position);
				for (int i = 2; i < this.mVertices.Length; i += 2)
				{
					this.mVertices[i].Position = this.mVertices[i - 2].Position;
					this.mVertices[i + 1].Position = this.mVertices[i - 1].Position;
				}
			}
			this.mLight.DiffuseColor = this.mColor;
			this.mLight.Start = this.mVertices[0].Position;
			this.mLight.End = this.mVertices[1].Position;
			this.mLight.Radius = 4f;
			this.mLight.Intensity = 0f;
			this.mLight.Enable(this.mPlayState.Scene);
			SpellManager.Instance.AddSpellEffect(this);
			if (this.mOwner != null)
			{
				Matrix orientation = this.mOwner.Body.Orientation;
				orientation.Translation = this.mOwner.Body.Position;
				orientation.M21 *= this.mRange;
				orientation.M22 *= this.mRange;
				orientation.M23 *= this.mRange;
				Elements elements = iElements & ~(Elements.Arcane | Elements.Life);
				int num = 0;
				for (int j = 0; j < 11; j++)
				{
					Elements elements2 = (Elements)(1 << j);
					if ((elements2 & elements) == elements2)
					{
						EffectManager.Instance.StartEffect(ArcaneBlade.ELEMENT_EFFECTS[j], ref orientation, out this.mEffects[num++]);
					}
				}
				return;
			}
			Matrix identity = Matrix.Identity;
			Elements elements3 = iElements & ~(Elements.Arcane | Elements.Life);
			int num2 = 0;
			for (int k = 0; k < 11; k++)
			{
				Elements elements4 = (Elements)(1 << k);
				if ((elements4 & elements3) == elements4)
				{
					EffectManager.Instance.StartEffect(ArcaneBlade.ELEMENT_EFFECTS[k], ref identity, out this.mEffects[num2++]);
				}
			}
		}

		// Token: 0x060029AE RID: 10670 RVA: 0x00146CC3 File Offset: 0x00144EC3
		public void Kill()
		{
			this.mDead = true;
		}

		// Token: 0x170009D4 RID: 2516
		// (get) Token: 0x060029AF RID: 10671 RVA: 0x00146CCC File Offset: 0x00144ECC
		public bool IsDead
		{
			get
			{
				float num;
				Vector3.DistanceSquared(ref this.mVertices[30].Position, ref this.mVertices[31].Position, out num);
				return this.mDead & num <= 0.1f;
			}
		}

		// Token: 0x060029B0 RID: 10672 RVA: 0x00146D18 File Offset: 0x00144F18
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			if (this.mDead)
			{
				this.mAlpha -= iDeltaTime * 5f;
				this.mRange = this.mMaxRange * (float)Math.Sqrt((double)Math.Max(this.mAlpha, 0f));
				this.mLight.Intensity = this.mAlpha;
			}
			else
			{
				this.mLight.Intensity = Math.Min(this.mLight.Intensity + iDeltaTime * 10f, 1f);
			}
			Vector3 up;
			if (this.mOwner == null)
			{
				this.mVertices[0].Position = this.mOrientation.Translation;
				up = this.mOrientation.Up;
			}
			else
			{
				this.mVertices[0].Position = this.mOwner.Position;
				up = this.mOwner.Body.Orientation.Up;
				Matrix orientation = this.mOwner.Body.Orientation;
				orientation.Translation = this.mOwner.Body.Position;
				orientation.M21 *= this.mRange;
				orientation.M22 *= this.mRange;
				orientation.M23 *= this.mRange;
				for (int i = 0; i < this.mEffects.Length; i++)
				{
					EffectManager.Instance.UpdateOrientation(ref this.mEffects[i], ref orientation);
				}
			}
			Vector3.Multiply(ref up, this.mRange, out up);
			Vector3.Add(ref this.mVertices[0].Position, ref up, out this.mVertices[1].Position);
			double x = 1.401298464324817E-45;
			float amount = (float)Math.Pow(x, (double)iDeltaTime);
			for (int j = 2; j < this.mVertices.Length; j += 2)
			{
				Vector3.Lerp(ref this.mVertices[j - 2].Position, ref this.mVertices[j].Position, amount, out this.mVertices[j].Position);
				Vector3.Lerp(ref this.mVertices[j - 1].Position, ref this.mVertices[j + 1].Position, amount, out this.mVertices[j + 1].Position);
			}
			ArcaneBlade.RenderData renderData = this.mRenderData[(int)iDataChannel];
			Vector3 one = Vector3.One;
			Vector3.Add(ref this.mColor, ref one, out renderData.ColorCenter);
			renderData.ColorEdge = this.mColor;
			renderData.BoundingSphere.Center = this.mVertices[0].Position;
			renderData.BoundingSphere.Radius = 5f;
			renderData.Alpha = 1f;
			this.mVertices.CopyTo(renderData.VertexArray, 0);
			this.mPlayState.Scene.AddRenderableAdditiveObject(iDataChannel, renderData);
			this.mLight.Start = this.mVertices[0].Position;
			this.mLight.End = this.mVertices[1].Position;
		}

		// Token: 0x060029B1 RID: 10673 RVA: 0x00147054 File Offset: 0x00145254
		public void OnRemove()
		{
			ArcaneBlade.sCache.Add(this);
			this.mLight.Disable();
			for (int i = 0; i < this.mEffects.Length; i++)
			{
				EffectManager.Instance.Stop(ref this.mEffects[i]);
			}
		}

		// Token: 0x170009D5 RID: 2517
		// (get) Token: 0x060029B2 RID: 10674 RVA: 0x001470A0 File Offset: 0x001452A0
		// (set) Token: 0x060029B3 RID: 10675 RVA: 0x001470A8 File Offset: 0x001452A8
		public Matrix Orientation
		{
			get
			{
				return this.mOrientation;
			}
			set
			{
				this.mOrientation = value;
			}
		}

		// Token: 0x170009D6 RID: 2518
		// (get) Token: 0x060029B4 RID: 10676 RVA: 0x001470B1 File Offset: 0x001452B1
		public float Range
		{
			get
			{
				return this.mRange;
			}
		}

		// Token: 0x04002D15 RID: 11541
		public const int VERTEXCOUNT = 32;

		// Token: 0x04002D16 RID: 11542
		private static List<ArcaneBlade> sCache;

		// Token: 0x04002D17 RID: 11543
		private static readonly int[] ELEMENT_EFFECTS = new int[]
		{
			0,
			"weapon_arcane_water".GetHashCodeCustom(),
			"weapon_arcane_cold".GetHashCodeCustom(),
			"weapon_arcane_fire".GetHashCodeCustom(),
			"weapon_arcane_lightning".GetHashCodeCustom(),
			0,
			0,
			0,
			0,
			"weapon_arcane_steam".GetHashCodeCustom(),
			"weapon_arcane_poison".GetHashCodeCustom()
		};

		// Token: 0x04002D18 RID: 11544
		private bool mDead;

		// Token: 0x04002D19 RID: 11545
		private Item mOwner;

		// Token: 0x04002D1A RID: 11546
		private static VertexDeclaration sVertexDeclaration;

		// Token: 0x04002D1B RID: 11547
		private static Texture2D sTexture;

		// Token: 0x04002D1C RID: 11548
		private VertexPositionTexture[] mVertices;

		// Token: 0x04002D1D RID: 11549
		private ArcaneBlade.RenderData[] mRenderData;

		// Token: 0x04002D1E RID: 11550
		private PlayState mPlayState;

		// Token: 0x04002D1F RID: 11551
		private float mAlpha;

		// Token: 0x04002D20 RID: 11552
		private Vector3 mColor;

		// Token: 0x04002D21 RID: 11553
		private float mRange;

		// Token: 0x04002D22 RID: 11554
		private float mMaxRange;

		// Token: 0x04002D23 RID: 11555
		private Matrix mOrientation;

		// Token: 0x04002D24 RID: 11556
		private CapsuleLight mLight;

		// Token: 0x04002D25 RID: 11557
		private VisualEffectReference[] mEffects = new VisualEffectReference[4];

		// Token: 0x02000575 RID: 1397
		protected class RenderData : IRenderableAdditiveObject
		{
			// Token: 0x060029B6 RID: 10678 RVA: 0x00147127 File Offset: 0x00145327
			public RenderData(int iNrOfVertices)
			{
				this.mVertices = new VertexPositionTexture[iNrOfVertices];
			}

			// Token: 0x170009D7 RID: 2519
			// (get) Token: 0x060029B7 RID: 10679 RVA: 0x0014713B File Offset: 0x0014533B
			public VertexPositionTexture[] VertexArray
			{
				get
				{
					return this.mVertices;
				}
			}

			// Token: 0x170009D8 RID: 2520
			// (get) Token: 0x060029B8 RID: 10680 RVA: 0x00147143 File Offset: 0x00145343
			public int Effect
			{
				get
				{
					return ArcaneEffect.TYPEHASH;
				}
			}

			// Token: 0x170009D9 RID: 2521
			// (get) Token: 0x060029B9 RID: 10681 RVA: 0x0014714A File Offset: 0x0014534A
			public int Technique
			{
				get
				{
					return 0;
				}
			}

			// Token: 0x170009DA RID: 2522
			// (get) Token: 0x060029BA RID: 10682 RVA: 0x0014714D File Offset: 0x0014534D
			public VertexBuffer Vertices
			{
				get
				{
					return null;
				}
			}

			// Token: 0x170009DB RID: 2523
			// (get) Token: 0x060029BB RID: 10683 RVA: 0x00147150 File Offset: 0x00145350
			public int VerticesHashCode
			{
				get
				{
					return 0;
				}
			}

			// Token: 0x170009DC RID: 2524
			// (get) Token: 0x060029BC RID: 10684 RVA: 0x00147153 File Offset: 0x00145353
			public int VertexStride
			{
				get
				{
					return VertexPositionTexture.SizeInBytes;
				}
			}

			// Token: 0x170009DD RID: 2525
			// (get) Token: 0x060029BD RID: 10685 RVA: 0x0014715A File Offset: 0x0014535A
			public IndexBuffer Indices
			{
				get
				{
					return null;
				}
			}

			// Token: 0x170009DE RID: 2526
			// (get) Token: 0x060029BE RID: 10686 RVA: 0x0014715D File Offset: 0x0014535D
			public VertexDeclaration VertexDeclaration
			{
				get
				{
					return ArcaneBlade.sVertexDeclaration;
				}
			}

			// Token: 0x060029BF RID: 10687 RVA: 0x00147164 File Offset: 0x00145364
			public bool Cull(BoundingFrustum iViewFrustum)
			{
				return this.BoundingSphere.Contains(iViewFrustum) == ContainmentType.Disjoint;
			}

			// Token: 0x060029C0 RID: 10688 RVA: 0x00147178 File Offset: 0x00145378
			public void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
			{
				ArcaneEffect arcaneEffect = iEffect as ArcaneEffect;
				arcaneEffect.ColorCenter = this.ColorCenter;
				arcaneEffect.ColorEdge = this.ColorEdge;
				arcaneEffect.World = Matrix.Identity;
				arcaneEffect.Texture = ArcaneBlade.sTexture;
				arcaneEffect.Alpha = this.Alpha;
				arcaneEffect.GraphicsDevice.RenderState.CullMode = CullMode.None;
				iEffect.CommitChanges();
				iEffect.GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleStrip, this.mVertices, 0, this.mVertices.Length - 2);
			}

			// Token: 0x04002D26 RID: 11558
			public BoundingSphere BoundingSphere;

			// Token: 0x04002D27 RID: 11559
			public Vector3 ColorCenter;

			// Token: 0x04002D28 RID: 11560
			public Vector3 ColorEdge;

			// Token: 0x04002D29 RID: 11561
			public float Alpha;

			// Token: 0x04002D2A RID: 11562
			private VertexPositionTexture[] mVertices;
		}
	}
}
