using System;
using Magicka.GameLogic;
using Magicka.GameLogic.Spells;
using Magicka.Graphics.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;

namespace Magicka.Graphics
{
	// Token: 0x020001AE RID: 430
	public class Flash : IAbilityEffect, IRenderableAdditiveObject, IPreRenderRenderer
	{
		// Token: 0x1700030D RID: 781
		// (get) Token: 0x06000CE0 RID: 3296 RVA: 0x0004BAA0 File Offset: 0x00049CA0
		public static Flash Instance
		{
			get
			{
				if (Flash.sSingelton == null)
				{
					lock (Flash.sSingeltonLock)
					{
						if (Flash.sSingelton == null)
						{
							Flash.sSingelton = new Flash();
						}
					}
				}
				return Flash.sSingelton;
			}
		}

		// Token: 0x06000CE1 RID: 3297 RVA: 0x0004BAF4 File Offset: 0x00049CF4
		private Flash()
		{
			this.mEffectHash = RenderManager.Instance.RegisterEffect(new FlashEffect());
			Vector2[] array = new Vector2[4];
			array[0].X = -1f;
			array[0].Y = 1f;
			array[1].X = 1f;
			array[1].Y = 1f;
			array[2].X = 1f;
			array[2].Y = -1f;
			array[3].X = -1f;
			array[3].Y = -1f;
			this.mVertices = new VertexBuffer(Game.Instance.GraphicsDevice, 32, BufferUsage.WriteOnly);
			this.mVertices.SetData<Vector2>(array);
			this.mVertexDeclaration = new VertexDeclaration(Game.Instance.GraphicsDevice, new VertexElement[]
			{
				new VertexElement(0, 0, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.Position, 0)
			});
			this.mVerticesHash = this.mVertices.GetHashCode();
		}

		// Token: 0x06000CE2 RID: 3298 RVA: 0x0004BC20 File Offset: 0x00049E20
		public void Execute(Scene iScene, float iTime)
		{
			this.mTTL = (this.mIntensity = Math.Max(iTime, this.mIntensity));
			this.mScene = iScene;
			SpellManager.Instance.AddSpellEffect(this);
		}

		// Token: 0x1700030E RID: 782
		// (get) Token: 0x06000CE3 RID: 3299 RVA: 0x0004BC5A File Offset: 0x00049E5A
		public int Effect
		{
			get
			{
				return this.mEffectHash;
			}
		}

		// Token: 0x1700030F RID: 783
		// (get) Token: 0x06000CE4 RID: 3300 RVA: 0x0004BC62 File Offset: 0x00049E62
		public int Technique
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x17000310 RID: 784
		// (get) Token: 0x06000CE5 RID: 3301 RVA: 0x0004BC65 File Offset: 0x00049E65
		public VertexBuffer Vertices
		{
			get
			{
				return this.mVertices;
			}
		}

		// Token: 0x17000311 RID: 785
		// (get) Token: 0x06000CE6 RID: 3302 RVA: 0x0004BC6D File Offset: 0x00049E6D
		public int VerticesHashCode
		{
			get
			{
				return this.mVerticesHash;
			}
		}

		// Token: 0x17000312 RID: 786
		// (get) Token: 0x06000CE7 RID: 3303 RVA: 0x0004BC75 File Offset: 0x00049E75
		public int VertexStride
		{
			get
			{
				return 8;
			}
		}

		// Token: 0x17000313 RID: 787
		// (get) Token: 0x06000CE8 RID: 3304 RVA: 0x0004BC78 File Offset: 0x00049E78
		public IndexBuffer Indices
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000314 RID: 788
		// (get) Token: 0x06000CE9 RID: 3305 RVA: 0x0004BC7B File Offset: 0x00049E7B
		public VertexDeclaration VertexDeclaration
		{
			get
			{
				return this.mVertexDeclaration;
			}
		}

		// Token: 0x06000CEA RID: 3306 RVA: 0x0004BC83 File Offset: 0x00049E83
		public bool Cull(BoundingFrustum iViewFrustum)
		{
			return false;
		}

		// Token: 0x06000CEB RID: 3307 RVA: 0x0004BC88 File Offset: 0x00049E88
		public void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
		{
			FlashEffect flashEffect = iEffect as FlashEffect;
			Vector4 color = default(Vector4);
			color.X = (color.Y = (color.Z = this.mIntensities[(int)this.mCurrentDataChannel]));
			color.W = 1f;
			flashEffect.Color = color;
			flashEffect.CommitChanges();
			flashEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
		}

		// Token: 0x06000CEC RID: 3308 RVA: 0x0004BCF3 File Offset: 0x00049EF3
		public void PreRenderUpdate(DataChannel iDataChannel, float iDeltaTime, ref Matrix iViewProjectionMatrix, ref Vector3 iCameraPosition, ref Vector3 iCameraDirection)
		{
			this.mCurrentDataChannel = iDataChannel;
		}

		// Token: 0x17000315 RID: 789
		// (get) Token: 0x06000CED RID: 3309 RVA: 0x0004BCFC File Offset: 0x00049EFC
		public bool IsDead
		{
			get
			{
				return this.mIntensity <= 0f;
			}
		}

		// Token: 0x06000CEE RID: 3310 RVA: 0x0004BD0E File Offset: 0x00049F0E
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			this.mIntensity -= iDeltaTime;
			this.mIntensities[(int)iDataChannel] = this.mIntensity / this.mTTL * 0.75f;
			this.mScene.AddRenderableAdditiveObject(iDataChannel, this);
		}

		// Token: 0x06000CEF RID: 3311 RVA: 0x0004BD46 File Offset: 0x00049F46
		public void OnRemove()
		{
		}

		// Token: 0x04000BB4 RID: 2996
		private static Flash sSingelton;

		// Token: 0x04000BB5 RID: 2997
		private static volatile object sSingeltonLock = new object();

		// Token: 0x04000BB6 RID: 2998
		private float mIntensity;

		// Token: 0x04000BB7 RID: 2999
		private float mTTL;

		// Token: 0x04000BB8 RID: 3000
		private Scene mScene;

		// Token: 0x04000BB9 RID: 3001
		private float[] mIntensities = new float[3];

		// Token: 0x04000BBA RID: 3002
		private int mEffectHash;

		// Token: 0x04000BBB RID: 3003
		private VertexBuffer mVertices;

		// Token: 0x04000BBC RID: 3004
		private VertexDeclaration mVertexDeclaration;

		// Token: 0x04000BBD RID: 3005
		private int mVerticesHash;

		// Token: 0x04000BBE RID: 3006
		private DataChannel mCurrentDataChannel;
	}
}
