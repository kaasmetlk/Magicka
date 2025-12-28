using System;
using Magicka.GameLogic.GameStates;
using Magicka.Graphics.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;

namespace Magicka.GameLogic.UI
{
	// Token: 0x02000636 RID: 1590
	public class Healthbars
	{
		// Token: 0x17000B5F RID: 2911
		// (get) Token: 0x0600300B RID: 12299 RVA: 0x0018862C File Offset: 0x0018682C
		public static Healthbars Instance
		{
			get
			{
				if (Healthbars.mSingelton == null)
				{
					lock (Healthbars.mSingeltonLock)
					{
						if (Healthbars.mSingelton == null)
						{
							Healthbars.mSingelton = new Healthbars();
						}
					}
				}
				return Healthbars.mSingelton;
			}
		}

		// Token: 0x0600300C RID: 12300 RVA: 0x00188680 File Offset: 0x00186880
		public Healthbars()
		{
			Texture2D texture2D;
			lock (Game.Instance.GraphicsDevice)
			{
				texture2D = Game.Instance.Content.Load<Texture2D>("UI/Hud/Hud");
			}
			Point screenSize = RenderManager.Instance.ScreenSize;
			this.mGUIHardwareInstancingEffect = new GUIHardwareInstancingEffect(Game.Instance.GraphicsDevice, Game.Instance.Content);
			this.mGUIHardwareInstancingEffect.SetTechnique(GUIHardwareInstancingEffect.Technique.Healthbars);
			this.mGUIHardwareInstancingEffect.ScreenSize = screenSize;
			this.mGUIHardwareInstancingEffect.SetScreenSize(screenSize.X, screenSize.Y);
			this.mGUIHardwareInstancingEffect.Texture = texture2D;
			float num = 1f / (float)texture2D.Height;
			float num2 = 1f / (float)texture2D.Width;
			Vector2 vector = new Vector2(16f * num2, 73f * num);
			Vector2 vector2 = new Vector2(8f * num2, 6f * num);
			Vector2 vector3 = new Vector2(6f * num2, 73f * num);
			Vector2 vector4 = new Vector2(32f * num2, 73f * num);
			Vector2 vector5 = new Vector2(2f * num2, 6f * num);
			Vector2 vector6 = new Vector2(2f * num2, 81f * num);
			Vector2 vector7 = new Vector2(4f * num2, 6f * num);
			Healthbars.Vertex[] array = new Healthbars.Vertex[8192];
			int[] array2 = new int[12288];
			Healthbars.Vertex vertex = default(Healthbars.Vertex);
			for (int i = 0; i < 512; i++)
			{
				int num3 = i * 16;
				vertex.Index = (float)(i % 512);
				vertex.Layer = 1f;
				vertex.Position.X = -4f;
				vertex.Position.Y = Healthbars.BARSIZE.Y;
				vertex.TexCoord.X = vector3.X;
				vertex.TexCoord.Y = vector3.Y + vector5.Y;
				array[num3] = vertex;
				vertex.Layer = 1f;
				vertex.Position.X = -4f;
				vertex.Position.Y = 0f;
				vertex.TexCoord.X = vector3.X;
				vertex.TexCoord.Y = vector3.Y;
				array[1 + num3] = vertex;
				vertex.Layer = 1f;
				vertex.Position.X = 0f;
				vertex.Position.Y = 0f;
				vertex.TexCoord.X = vector3.X + vector5.X;
				vertex.TexCoord.Y = vector3.Y;
				array[2 + num3] = vertex;
				vertex.Layer = 1f;
				vertex.Position.X = 0f;
				vertex.Position.Y = Healthbars.BARSIZE.Y;
				vertex.TexCoord.X = vector3.X + vector5.X;
				vertex.TexCoord.Y = vector3.Y + vector5.Y;
				array[3 + num3] = vertex;
				vertex.Layer = 1f;
				vertex.Position.X = Healthbars.BARSIZE.X;
				vertex.Position.Y = Healthbars.BARSIZE.Y;
				vertex.TexCoord.X = vector4.X;
				vertex.TexCoord.Y = vector4.Y + vector5.Y;
				array[4 + num3] = vertex;
				vertex.Layer = 1f;
				vertex.Position.X = Healthbars.BARSIZE.X;
				vertex.Position.Y = 0f;
				vertex.TexCoord.X = vector4.X;
				vertex.TexCoord.Y = vector4.Y;
				array[5 + num3] = vertex;
				vertex.Layer = 1f;
				vertex.Position.X = Healthbars.BARSIZE.X + 4f;
				vertex.Position.Y = 0f;
				vertex.TexCoord.X = vector4.X + vector5.X;
				vertex.TexCoord.Y = vector4.Y;
				array[6 + num3] = vertex;
				vertex.Layer = 1f;
				vertex.Position.X = Healthbars.BARSIZE.X + 4f;
				vertex.Position.Y = Healthbars.BARSIZE.Y;
				vertex.TexCoord.X = vector4.X + vector5.X;
				vertex.TexCoord.Y = vector4.Y + vector5.Y;
				array[7 + num3] = vertex;
				vertex.Layer = 1f;
				vertex.Position.X = 0f;
				vertex.Position.Y = Healthbars.BARSIZE.Y;
				vertex.TexCoord.X = vector.X;
				vertex.TexCoord.Y = vector.Y + vector2.Y;
				array[8 + num3] = vertex;
				vertex.Layer = 1f;
				vertex.Position.X = 0f;
				vertex.Position.Y = 0f;
				vertex.TexCoord.X = vector.X;
				vertex.TexCoord.Y = vector.Y;
				array[9 + num3] = vertex;
				vertex.Layer = 1f;
				vertex.Position.X = Healthbars.BARSIZE.X;
				vertex.Position.Y = 0f;
				vertex.TexCoord.X = vector.X + vector2.X;
				vertex.TexCoord.Y = vector.Y;
				array[10 + num3] = vertex;
				vertex.Layer = 1f;
				vertex.Position.X = Healthbars.BARSIZE.X;
				vertex.Position.Y = Healthbars.BARSIZE.Y;
				vertex.TexCoord.X = vector.X + vector2.X;
				vertex.TexCoord.Y = vector.Y + vector2.Y;
				array[11 + num3] = vertex;
				vertex.Layer = 0f;
				vertex.Position.X = 0f;
				vertex.Position.Y = Healthbars.BARSIZE.Y;
				vertex.TexCoord.X = vector6.X;
				vertex.TexCoord.Y = vector6.Y + vector7.Y;
				array[12 + num3] = vertex;
				vertex.Layer = 0f;
				vertex.Position.X = 0f;
				vertex.Position.Y = 0f;
				vertex.TexCoord.X = vector6.X;
				vertex.TexCoord.Y = vector6.Y;
				array[13 + num3] = vertex;
				vertex.Layer = 0f;
				vertex.Position.X = Healthbars.BARSIZE.X;
				vertex.Position.Y = 0f;
				vertex.TexCoord.X = vector6.X + vector7.X;
				vertex.TexCoord.Y = vector6.Y;
				array[14 + num3] = vertex;
				vertex.Layer = 0f;
				vertex.Position.X = Healthbars.BARSIZE.X;
				vertex.Position.Y = Healthbars.BARSIZE.Y;
				vertex.TexCoord.X = vector6.X + vector7.X;
				vertex.TexCoord.Y = vector6.Y + vector7.Y;
				array[15 + num3] = vertex;
				int num4 = i * 24;
				array2[num4] = num3;
				array2[1 + num4] = num3 + 1;
				array2[2 + num4] = num3 + 2;
				array2[3 + num4] = num3 + 2;
				array2[4 + num4] = num3 + 3;
				array2[5 + num4] = num3;
				array2[6 + num4] = num3 + 4;
				array2[7 + num4] = num3 + 1 + 4;
				array2[8 + num4] = num3 + 2 + 4;
				array2[9 + num4] = num3 + 2 + 4;
				array2[10 + num4] = num3 + 3 + 4;
				array2[11 + num4] = num3 + 4;
				array2[12 + num4] = num3 + 8;
				array2[13 + num4] = num3 + 1 + 8;
				array2[14 + num4] = num3 + 2 + 8;
				array2[15 + num4] = num3 + 2 + 8;
				array2[16 + num4] = num3 + 3 + 8;
				array2[17 + num4] = num3 + 8;
				array2[18 + num4] = num3 + 12;
				array2[19 + num4] = num3 + 1 + 12;
				array2[20 + num4] = num3 + 2 + 12;
				array2[21 + num4] = num3 + 2 + 12;
				array2[22 + num4] = num3 + 3 + 12;
				array2[23 + num4] = num3 + 12;
			}
			lock (Game.Instance.GraphicsDevice)
			{
				this.mVertexBuffer = new VertexBuffer(Game.Instance.GraphicsDevice, 229376, BufferUsage.WriteOnly);
				this.mVertexBuffer.SetData<Healthbars.Vertex>(array);
				this.mIndexBuffer = new IndexBuffer(Game.Instance.GraphicsDevice, 49152, BufferUsage.WriteOnly, IndexElementSize.ThirtyTwoBits);
				this.mIndexBuffer.SetData<int>(array2);
				this.mVertexDeclaration = new VertexDeclaration(Game.Instance.GraphicsDevice, Healthbars.Vertex.VertexElements);
			}
			for (int j = 0; j < 512; j++)
			{
				this.mPositions[j] = new Vector3(0f);
				this.mColors[j] = default(Vector4);
				this.mScales[j] = new Vector3(1f);
				this.mLength[j] = 72f;
			}
			this.mRenderData = new Healthbars.RenderData[3];
			for (int k = 0; k < 3; k++)
			{
				Healthbars.RenderData renderData = new Healthbars.RenderData();
				this.mRenderData[k] = renderData;
				renderData.mIndexBuffer = this.mIndexBuffer;
				renderData.mVertexBuffer = this.mVertexBuffer;
				renderData.mVertexDeclaration = this.mVertexDeclaration;
				renderData.mEffect = this.mGUIHardwareInstancingEffect;
			}
		}

		// Token: 0x0600300D RID: 12301 RVA: 0x0018926C File Offset: 0x0018746C
		public void AddHealthBar(Vector3 iPosition, float iNormalizedHitPoints, float iWidth, float iHeight, float iTimeSinceLastDamage, bool iFadeWithoutDamage, Vector4? iColor, Vector2? iOffset)
		{
			if (iColor == null)
			{
				iColor = new Vector4?(new Vector4(1f, 0f, 0f, 1f));
			}
			float num = MathHelper.Clamp(0.3f - iTimeSinceLastDamage, 0f, 0.3f) * 4f;
			float num2;
			if (iFadeWithoutDamage)
			{
				num2 = MathHelper.Lerp(0f, 1f, 3f - iTimeSinceLastDamage);
				num2 = MathHelper.Clamp(num2, 0f, 1f);
			}
			else
			{
				num2 = 1f;
			}
			if (this.mHealtbarsToDraw < 512 && num2 > 0f)
			{
				this.mPositions[this.mHealtbarsToDraw] = iPosition;
				this.mColors[this.mHealtbarsToDraw] = iColor.Value + new Vector4(num, num, num, 0f);
				Vector4[] array = this.mColors;
				int num3 = this.mHealtbarsToDraw;
				array[num3].W = array[num3].W * num2;
				this.mScales[this.mHealtbarsToDraw].X = MathHelper.Clamp(iNormalizedHitPoints, 0f, 1f);
				this.mScales[this.mHealtbarsToDraw].Y = MathHelper.Min(iWidth, 1.5f);
				this.mScales[this.mHealtbarsToDraw].Z = iHeight;
				this.mOffsets[this.mHealtbarsToDraw] = iOffset.GetValueOrDefault();
				this.mHealtbarsToDraw++;
			}
		}

		// Token: 0x0600300E RID: 12302 RVA: 0x00189400 File Offset: 0x00187600
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			Healthbars.RenderData renderData = this.mRenderData[(int)iDataChannel];
			this.mPositions.CopyTo(renderData.mWorldPositions, 0);
			this.mColors.CopyTo(renderData.mColors, 0);
			this.mScales.CopyTo(renderData.mScales, 0);
			this.mOffsets.CopyTo(renderData.mOffsets, 0);
			renderData.mHealthbarsToDraw = this.mHealtbarsToDraw;
			this.mHealtbarsToDraw = 0;
			if (this.mUIEnabled)
			{
				GameStateManager.Instance.CurrentState.Scene.AddRenderableGUIObject(iDataChannel, renderData);
			}
		}

		// Token: 0x17000B60 RID: 2912
		// (get) Token: 0x0600300F RID: 12303 RVA: 0x0018948F File Offset: 0x0018768F
		// (set) Token: 0x06003010 RID: 12304 RVA: 0x00189497 File Offset: 0x00187697
		public bool UIEnabled
		{
			get
			{
				return this.mUIEnabled;
			}
			set
			{
				this.mUIEnabled = value;
			}
		}

		// Token: 0x0400341A RID: 13338
		private static Healthbars mSingelton;

		// Token: 0x0400341B RID: 13339
		private static volatile object mSingeltonLock = new object();

		// Token: 0x0400341C RID: 13340
		private Healthbars.RenderData[] mRenderData;

		// Token: 0x0400341D RID: 13341
		private GUIHardwareInstancingEffect mGUIHardwareInstancingEffect;

		// Token: 0x0400341E RID: 13342
		private IndexBuffer mIndexBuffer;

		// Token: 0x0400341F RID: 13343
		private VertexBuffer mVertexBuffer;

		// Token: 0x04003420 RID: 13344
		private VertexDeclaration mVertexDeclaration;

		// Token: 0x04003421 RID: 13345
		private static readonly Vector2 BARSIZE = new Vector2(64f, 8f);

		// Token: 0x04003422 RID: 13346
		private Vector3[] mPositions = new Vector3[512];

		// Token: 0x04003423 RID: 13347
		private Vector3[] mScales = new Vector3[512];

		// Token: 0x04003424 RID: 13348
		private Vector2[] mOffsets = new Vector2[512];

		// Token: 0x04003425 RID: 13349
		private Vector4[] mColors = new Vector4[512];

		// Token: 0x04003426 RID: 13350
		private float[] mLength = new float[512];

		// Token: 0x04003427 RID: 13351
		private int mHealtbarsToDraw;

		// Token: 0x04003428 RID: 13352
		private bool mUIEnabled = true;

		// Token: 0x02000637 RID: 1591
		protected class RenderData : IRenderableGUIObject, IPreRenderRenderer
		{
			// Token: 0x06003012 RID: 12306 RVA: 0x001894C4 File Offset: 0x001876C4
			public void Draw(float iDeltaTime)
			{
				if (this.mHealthbarsToDraw > 0)
				{
					this.mEffect.GraphicsDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
					this.mEffect.GraphicsDevice.Indices = this.mIndexBuffer;
					this.mEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
					this.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertexBuffer, 0, 28);
					this.mEffect.Begin();
					this.mEffect.CurrentTechnique.Passes[0].Begin();
					for (int i = 0; i < this.mHealthbarsToDraw; i += 40)
					{
						int num = this.mWorldPositions.Length - i;
						if (num > 40)
						{
							num = 40;
						}
						if (num > this.mHealthbarsToDraw)
						{
							num = this.mHealthbarsToDraw;
						}
						Array.Copy(this.mColors, i, this.mBatchColors, 0, num);
						Array.Copy(this.mWorldPositions, i, this.mBatchPositions, 0, num);
						Array.Copy(this.mScales, i, this.mBatchScales, 0, num);
						Array.Copy(this.mOffsets, i, this.mBatchOffsets, 0, num);
						this.mEffect.Colors = this.mBatchColors;
						this.mEffect.WorldPositions = this.mBatchPositions;
						this.mEffect.Scales = this.mBatchScales;
						this.mEffect.TextureOffsets = this.mBatchOffsets;
						this.mEffect.CommitChanges();
						this.mEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, i * 16, num * 16, i * 24, num * 8);
					}
					this.mEffect.CurrentTechnique.Passes[0].End();
					this.mEffect.End();
				}
			}

			// Token: 0x17000B61 RID: 2913
			// (get) Token: 0x06003013 RID: 12307 RVA: 0x0018968A File Offset: 0x0018788A
			public int ZIndex
			{
				get
				{
					return 98;
				}
			}

			// Token: 0x06003014 RID: 12308 RVA: 0x00189690 File Offset: 0x00187890
			public void PreRenderUpdate(DataChannel iDataChannel, float iDeltaTime, ref Matrix iViewProjectionMatrix, ref Vector3 iCameraPosition, ref Vector3 iCameraDirection)
			{
				this.mEffect.ScreenSize = RenderManager.Instance.ScreenSize;
				this.mEffect.SetScreenSize(RenderManager.Instance.ScreenSize.X, RenderManager.Instance.ScreenSize.Y);
				this.mEffect.WorldToScreen = iViewProjectionMatrix;
			}

			// Token: 0x04003429 RID: 13353
			public GUIHardwareInstancingEffect mEffect;

			// Token: 0x0400342A RID: 13354
			public VertexDeclaration mVertexDeclaration;

			// Token: 0x0400342B RID: 13355
			public VertexBuffer mVertexBuffer;

			// Token: 0x0400342C RID: 13356
			public IndexBuffer mIndexBuffer;

			// Token: 0x0400342D RID: 13357
			public Vector3[] mWorldPositions = new Vector3[512];

			// Token: 0x0400342E RID: 13358
			public Vector3[] mScales = new Vector3[512];

			// Token: 0x0400342F RID: 13359
			public Vector4[] mColors = new Vector4[512];

			// Token: 0x04003430 RID: 13360
			public Vector2[] mOffsets = new Vector2[512];

			// Token: 0x04003431 RID: 13361
			public int mHealthbarsToDraw;

			// Token: 0x04003432 RID: 13362
			private Vector3[] mBatchPositions = new Vector3[40];

			// Token: 0x04003433 RID: 13363
			private Vector3[] mBatchScales = new Vector3[40];

			// Token: 0x04003434 RID: 13364
			private Vector4[] mBatchColors = new Vector4[40];

			// Token: 0x04003435 RID: 13365
			private Vector2[] mBatchOffsets = new Vector2[40];
		}

		// Token: 0x02000638 RID: 1592
		private struct Vertex
		{
			// Token: 0x04003436 RID: 13366
			public const int SIZEINBYTES = 28;

			// Token: 0x04003437 RID: 13367
			public Vector3 Position;

			// Token: 0x04003438 RID: 13368
			public Vector2 TexCoord;

			// Token: 0x04003439 RID: 13369
			public float Index;

			// Token: 0x0400343A RID: 13370
			public float Layer;

			// Token: 0x0400343B RID: 13371
			public static readonly VertexElement[] VertexElements = new VertexElement[]
			{
				new VertexElement(0, 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, 0),
				new VertexElement(0, 12, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0),
				new VertexElement(0, 20, VertexElementFormat.Single, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 1),
				new VertexElement(0, 24, VertexElementFormat.Single, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 2)
			};
		}
	}
}
