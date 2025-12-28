using System;
using Magicka.Graphics.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;

namespace Magicka.GameLogic.UI
{
	// Token: 0x02000639 RID: 1593
	public sealed class DamageNotifyer
	{
		// Token: 0x17000B62 RID: 2914
		// (get) Token: 0x06003017 RID: 12311 RVA: 0x001897F0 File Offset: 0x001879F0
		public static DamageNotifyer Instance
		{
			get
			{
				if (DamageNotifyer.mSingelton == null)
				{
					lock (DamageNotifyer.mSingeltonLock)
					{
						if (DamageNotifyer.mSingelton == null)
						{
							DamageNotifyer.mSingelton = new DamageNotifyer();
						}
					}
				}
				return DamageNotifyer.mSingelton;
			}
		}

		// Token: 0x06003018 RID: 12312 RVA: 0x00189844 File Offset: 0x00187A44
		private DamageNotifyer()
		{
			this.mNumbers = new DamageNotifyer.Number[1024];
			this.mFreeInstances = new Heap<int>(1024);
			for (int i = 0; i < 1024; i++)
			{
				this.mFreeInstances.Push(i);
			}
			GraphicsDevice graphicsDevice = Game.Instance.GraphicsDevice;
			Texture2D texture2D;
			lock (graphicsDevice)
			{
				this.mEffect = new GUIHardwareInstancingEffect(Game.Instance.GraphicsDevice, Game.Instance.Content);
				texture2D = Game.Instance.Content.Load<Texture2D>("UI/HUD/Digits");
			}
			this.mEffect.Texture = texture2D;
			this.DIGITSIZEUV = DamageNotifyer.DIGITSIZE / new Vector2((float)texture2D.Width, (float)texture2D.Height);
			Point point;
			point.X = GlobalSettings.Instance.Resolution.Width;
			point.Y = GlobalSettings.Instance.Resolution.Height;
			this.mEffect.SetScreenSize(point.X, point.Y);
			this.mEffect.SetTechnique(GUIHardwareInstancingEffect.Technique.Numbers);
			this.mEffect.DigitWidth = this.DIGITSIZEUV.X;
			this.mRenderData = new DamageNotifyer.RenderData[3];
			for (int j = 0; j < 3; j++)
			{
				DamageNotifyer.RenderData renderData = new DamageNotifyer.RenderData();
				this.mRenderData[j] = renderData;
				renderData.mEffect = this.mEffect;
			}
			this.CreateVertices();
		}

		// Token: 0x06003019 RID: 12313 RVA: 0x001899CC File Offset: 0x00187BCC
		private void CreateVertices()
		{
			DamageNotifyer.VertexPositionTextureIndex[] array = new DamageNotifyer.VertexPositionTextureIndex[240];
			for (int i = 0; i < 40; i++)
			{
				DamageNotifyer.VertexPositionTextureIndex vertexPositionTextureIndex;
				vertexPositionTextureIndex.Index = (float)i;
				vertexPositionTextureIndex.Position = new Vector3(DamageNotifyer.DIGITSIZE.X * -0.5f, DamageNotifyer.DIGITSIZE.Y * -0.5f, 0f);
				vertexPositionTextureIndex.TexCoord = new Vector2(0f, 0f);
				array[i * 6] = vertexPositionTextureIndex;
				vertexPositionTextureIndex.Position = new Vector3(DamageNotifyer.DIGITSIZE.X * 0.5f, DamageNotifyer.DIGITSIZE.Y * -0.5f, 0f);
				vertexPositionTextureIndex.TexCoord = new Vector2(1f, 0f);
				array[1 + i * 6] = vertexPositionTextureIndex;
				vertexPositionTextureIndex.Position = new Vector3(DamageNotifyer.DIGITSIZE.X * -0.5f, DamageNotifyer.DIGITSIZE.Y * 0.5f, 0f);
				vertexPositionTextureIndex.TexCoord = new Vector2(0f, 1f);
				array[2 + i * 6] = vertexPositionTextureIndex;
				vertexPositionTextureIndex.Position = new Vector3(DamageNotifyer.DIGITSIZE.X * 0.5f, DamageNotifyer.DIGITSIZE.Y * -0.5f, 0f);
				vertexPositionTextureIndex.TexCoord = new Vector2(1f, 0f);
				array[3 + i * 6] = vertexPositionTextureIndex;
				vertexPositionTextureIndex.Position = new Vector3(DamageNotifyer.DIGITSIZE.X * 0.5f, DamageNotifyer.DIGITSIZE.Y * 0.5f, 0f);
				vertexPositionTextureIndex.TexCoord = new Vector2(1f, 1f);
				array[4 + i * 6] = vertexPositionTextureIndex;
				vertexPositionTextureIndex.Position = new Vector3(DamageNotifyer.DIGITSIZE.X * -0.5f, DamageNotifyer.DIGITSIZE.Y * 0.5f, 0f);
				vertexPositionTextureIndex.TexCoord = new Vector2(0f, 1f);
				array[5 + i * 6] = vertexPositionTextureIndex;
			}
			if (this.mVertices == null || this.mVertices.IsDisposed)
			{
				this.mVertices = new VertexBuffer(Game.Instance.GraphicsDevice, 24 * array.Length, BufferUsage.WriteOnly);
			}
			this.mVertices.SetData<DamageNotifyer.VertexPositionTextureIndex>(array);
			if (this.mVertexDeclaration == null || this.mVertexDeclaration.IsDisposed)
			{
				this.mVertexDeclaration = new VertexDeclaration(Game.Instance.GraphicsDevice, DamageNotifyer.VertexPositionTextureIndex.VertexElements);
			}
			for (int j = 0; j < 3; j++)
			{
				this.mRenderData[j].mVertices = this.mVertices;
				this.mRenderData[j].mVertexDeclaration = this.mVertexDeclaration;
			}
		}

		// Token: 0x0600301A RID: 12314 RVA: 0x00189CAC File Offset: 0x00187EAC
		public void Clear()
		{
			this.mFreeInstances.Clear();
			for (int i = 0; i < 1024; i++)
			{
				this.mFreeInstances.Push(i);
			}
			this.mLastNumber = -1;
		}

		// Token: 0x0600301B RID: 12315 RVA: 0x00189CE8 File Offset: 0x00187EE8
		public int AddNumber(float iValue, ref Vector3 iPosition, float iTTL, bool iLocked)
		{
			if (GlobalSettings.Instance.DamageNumbers == SettingOptions.Off)
			{
				return 0;
			}
			if (this.mFreeInstances.IsEmpty)
			{
				return -1;
			}
			int num = this.mFreeInstances.Pop();
			DamageNotifyer.Number number = default(DamageNotifyer.Number);
			number.Locked = iLocked;
			number.Position = iPosition;
			number.Scale.Y = 1f;
			number.Scale.X = 1f;
			number.Value = iValue;
			Vector3 color;
			DamageNotifyer.GetDefaultColors(iValue, out color);
			number.Color = color;
			number.TTL.X = iTTL;
			number.TTL.Y = iTTL;
			number.Velocity = 0.02f;
			this.mNumbers[num] = number;
			this.mLastNumber = Math.Max(this.mLastNumber, num);
			return num;
		}

		// Token: 0x0600301C RID: 12316 RVA: 0x00189DC4 File Offset: 0x00187FC4
		public int AddNumber(float iValue, ref Vector3 iPosition, float iTTL, bool iLocked, ref Vector3 iColor)
		{
			if (GlobalSettings.Instance.DamageNumbers == SettingOptions.Off)
			{
				return 0;
			}
			if (this.mFreeInstances.IsEmpty)
			{
				return -1;
			}
			int num = this.mFreeInstances.Pop();
			DamageNotifyer.Number number = default(DamageNotifyer.Number);
			number.Locked = iLocked;
			number.Position = iPosition;
			number.Scale.Y = 1f;
			number.Scale.X = 1f;
			number.Value = iValue;
			number.Color = iColor;
			number.TTL.X = iTTL;
			number.TTL.Y = iTTL;
			number.Velocity = 0.02f;
			this.mNumbers[num] = number;
			this.mLastNumber = Math.Max(this.mLastNumber, num);
			return num;
		}

		// Token: 0x0600301D RID: 12317 RVA: 0x00189E9B File Offset: 0x0018809B
		public void AddToNumber(int iIndex, float iValue)
		{
			DamageNotifyer.Number[] array = this.mNumbers;
			array[iIndex].Value = array[iIndex].Value + iValue;
		}

		// Token: 0x0600301E RID: 12318 RVA: 0x00189EB6 File Offset: 0x001880B6
		public void UpdateNumberPosition(int iIndex, ref Vector3 iPosition)
		{
			this.mNumbers[iIndex].Position = iPosition;
		}

		// Token: 0x0600301F RID: 12319 RVA: 0x00189ECF File Offset: 0x001880CF
		public void ReleasNumber(int iIndex)
		{
			this.mNumbers[iIndex].Locked = false;
		}

		// Token: 0x06003020 RID: 12320 RVA: 0x00189EE4 File Offset: 0x001880E4
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			DamageNotifyer.RenderData renderData = this.mRenderData[(int)iDataChannel];
			renderData.mNrOfNumbers = this.mLastNumber + 1;
			DamageNotifyer.Number.CopyToArrays(this.mNumbers, 0, 0, this.mLastNumber + 1, renderData.mWorldPositions, renderData.mScales, renderData.mValues, renderData.mColors);
			for (int i = 0; i <= this.mLastNumber; i++)
			{
				DamageNotifyer.Number number = this.mNumbers[i];
				number.Scale.Y = 1f + Math.Min(2f, Math.Abs(number.Value / 1000f));
				number.Scale.X = 1f + Math.Min(2f, Math.Abs(number.Value / 1000f));
				if (number.Locked)
				{
					this.mNumbers[i] = number;
				}
				else
				{
					float y = number.TTL.Y;
					number.TTL.Y = y - iDeltaTime;
					number.Velocity -= iDeltaTime * 0.03f;
					number.Position.Y = number.Position.Y + number.Velocity;
					this.mNumbers[i] = number;
					if (number.TTL.Y < 1E-45f && y >= 1E-45f)
					{
						this.mFreeInstances.Push(i);
					}
				}
			}
			while (this.mLastNumber >= 0 && this.mNumbers[this.mLastNumber].TTL.Y < 0f)
			{
				this.mLastNumber--;
			}
			this.mScene.AddRenderableGUIObject(iDataChannel, renderData);
		}

		// Token: 0x17000B63 RID: 2915
		// (get) Token: 0x06003021 RID: 12321 RVA: 0x0018A0A6 File Offset: 0x001882A6
		// (set) Token: 0x06003022 RID: 12322 RVA: 0x0018A0B0 File Offset: 0x001882B0
		public Scene Scene
		{
			get
			{
				return this.mScene;
			}
			set
			{
				this.mScene = value;
				this.mLastNumber = -1;
				this.mFreeInstances.Clear();
				for (int i = 0; i < 1024; i++)
				{
					this.mFreeInstances.Push(i);
				}
				this.UpdateResolution();
			}
		}

		// Token: 0x06003023 RID: 12323 RVA: 0x0018A0F8 File Offset: 0x001882F8
		public static void GetDefaultColors(float amount, out Vector3 oColor)
		{
			oColor = default(Vector3);
			if (amount < 0f)
			{
				oColor.X = 0f;
				oColor.Y = 1f;
				oColor.Z = 0f;
				return;
			}
			if (amount > 0f)
			{
				oColor.X = 1f;
				oColor.Y = 0.9f;
				oColor.Z = 0f;
				return;
			}
			oColor.X = 0.5f;
			oColor.Y = 0.5f;
			oColor.Z = 0.5f;
		}

		// Token: 0x06003024 RID: 12324 RVA: 0x0018A184 File Offset: 0x00188384
		public void UpdateResolution()
		{
			Point point;
			point.X = GlobalSettings.Instance.Resolution.Width;
			point.Y = GlobalSettings.Instance.Resolution.Height;
			this.mEffect.SetScreenSize(point.X, point.Y);
		}

		// Token: 0x0400343C RID: 13372
		public const int MAXNUMBERS = 1024;

		// Token: 0x0400343D RID: 13373
		private static DamageNotifyer mSingelton;

		// Token: 0x0400343E RID: 13374
		private static volatile object mSingeltonLock = new object();

		// Token: 0x0400343F RID: 13375
		public static readonly Vector2 DIGITSIZE = new Vector2(11f, 16f);

		// Token: 0x04003440 RID: 13376
		public readonly Vector2 DIGITSIZEUV;

		// Token: 0x04003441 RID: 13377
		private VertexBuffer mVertices;

		// Token: 0x04003442 RID: 13378
		private VertexDeclaration mVertexDeclaration;

		// Token: 0x04003443 RID: 13379
		private GUIHardwareInstancingEffect mEffect;

		// Token: 0x04003444 RID: 13380
		private Heap<int> mFreeInstances;

		// Token: 0x04003445 RID: 13381
		private int mLastNumber;

		// Token: 0x04003446 RID: 13382
		private DamageNotifyer.Number[] mNumbers;

		// Token: 0x04003447 RID: 13383
		private Scene mScene;

		// Token: 0x04003448 RID: 13384
		private DamageNotifyer.RenderData[] mRenderData;

		// Token: 0x0200063A RID: 1594
		private class RenderData : IRenderableGUIObject, IPreRenderRenderer
		{
			// Token: 0x06003026 RID: 12326 RVA: 0x0018A1F8 File Offset: 0x001883F8
			public void Draw(float iDeltaTime)
			{
				if (this.mNrOfNumbers <= 0)
				{
					return;
				}
				this.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertices, 0, 24);
				this.mEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
				this.mEffect.GraphicsDevice.RenderState.CullMode = CullMode.None;
				for (int i = 0; i < this.mNrOfNumbers; i += 40)
				{
					int num = this.mPositions.Length - i;
					if (num > 40)
					{
						num = 40;
					}
					if (num > this.mNrOfNumbers)
					{
						num = this.mNrOfNumbers;
					}
					Array.Copy(this.mPositions, i, this.mBatchPositions, 0, num);
					Array.Copy(this.mScales, i, this.mBatchScales, 0, num);
					Array.Copy(this.mValues, i, this.mBatchValues, 0, num);
					Array.Copy(this.mColors, i, this.mBatchColors, 0, num);
					this.mEffect.Positions = this.mBatchPositions;
					this.mEffect.Scales = this.mBatchScales;
					this.mEffect.Values = this.mBatchValues;
					this.mEffect.Colors = this.mBatchColors;
					this.mEffect.Begin();
					for (int j = 0; j < this.mEffect.CurrentTechnique.Passes.Count; j++)
					{
						this.mEffect.CurrentTechnique.Passes[j].Begin();
						this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, num * 2);
						this.mEffect.CurrentTechnique.Passes[j].End();
					}
					this.mEffect.End();
				}
			}

			// Token: 0x17000B64 RID: 2916
			// (get) Token: 0x06003027 RID: 12327 RVA: 0x0018A3AF File Offset: 0x001885AF
			public int ZIndex
			{
				get
				{
					return 20;
				}
			}

			// Token: 0x06003028 RID: 12328 RVA: 0x0018A3B4 File Offset: 0x001885B4
			public void PreRenderUpdate(DataChannel iDataChannel, float iDeltaTime, ref Matrix iViewProjectionMatrix, ref Vector3 iCameraPosition, ref Vector3 iCameraDirection)
			{
				for (int i = 0; i < this.mNrOfNumbers; i++)
				{
					this.mPositions[i] = MagickaMath.WorldToScreenPosition(ref this.mWorldPositions[i], ref iViewProjectionMatrix);
				}
			}

			// Token: 0x04003449 RID: 13385
			public GUIHardwareInstancingEffect mEffect;

			// Token: 0x0400344A RID: 13386
			public VertexBuffer mVertices;

			// Token: 0x0400344B RID: 13387
			public VertexDeclaration mVertexDeclaration;

			// Token: 0x0400344C RID: 13388
			public int mNrOfNumbers;

			// Token: 0x0400344D RID: 13389
			public Vector3[] mWorldPositions = new Vector3[1024];

			// Token: 0x0400344E RID: 13390
			private Vector2[] mPositions = new Vector2[1024];

			// Token: 0x0400344F RID: 13391
			public Vector3[] mScales = new Vector3[1024];

			// Token: 0x04003450 RID: 13392
			public float[] mValues = new float[1024];

			// Token: 0x04003451 RID: 13393
			public Vector4[] mColors = new Vector4[1024];

			// Token: 0x04003452 RID: 13394
			private Vector2[] mBatchPositions = new Vector2[40];

			// Token: 0x04003453 RID: 13395
			private Vector3[] mBatchScales = new Vector3[40];

			// Token: 0x04003454 RID: 13396
			private float[] mBatchValues = new float[40];

			// Token: 0x04003455 RID: 13397
			private Vector4[] mBatchColors = new Vector4[40];
		}

		// Token: 0x0200063B RID: 1595
		private struct VertexPositionTextureIndex
		{
			// Token: 0x04003456 RID: 13398
			public const int SIZEINBYTES = 24;

			// Token: 0x04003457 RID: 13399
			public Vector3 Position;

			// Token: 0x04003458 RID: 13400
			public Vector2 TexCoord;

			// Token: 0x04003459 RID: 13401
			public float Index;

			// Token: 0x0400345A RID: 13402
			public static readonly VertexElement[] VertexElements = new VertexElement[]
			{
				new VertexElement(0, 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, 0),
				new VertexElement(0, 12, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0),
				new VertexElement(0, 20, VertexElementFormat.Single, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 1)
			};
		}

		// Token: 0x0200063C RID: 1596
		private struct Number
		{
			// Token: 0x0600302B RID: 12331 RVA: 0x0018A4F4 File Offset: 0x001886F4
			public static void CopyToArrays(DamageNotifyer.Number[] iSource, int iSourceStartIndex, int iTargetStartIndex, int iCount, Vector3[] iTargetPositions, Vector3[] iTargetScales, float[] iTargetValues, Vector4[] iTargetColors)
			{
				Vector4 vector = default(Vector4);
				for (int i = 0; i < iCount; i++)
				{
					DamageNotifyer.Number number = iSource[i + iSourceStartIndex];
					vector.X = number.Color.X;
					vector.Y = number.Color.Y;
					vector.Z = number.Color.Z;
					vector.W = Math.Max(1f - (float)Math.Pow((double)(1f - number.TTL.Y / number.TTL.X), 5.0), 0f);
					int num = i + iTargetStartIndex;
					iTargetPositions[num] = number.Position;
					iTargetScales[num] = new Vector3(number.Scale, 1f);
					iTargetValues[num] = number.Value + 0.5f;
					iTargetColors[num] = vector;
				}
			}

			// Token: 0x0400345B RID: 13403
			public bool Locked;

			// Token: 0x0400345C RID: 13404
			public float Velocity;

			// Token: 0x0400345D RID: 13405
			public Vector3 Position;

			// Token: 0x0400345E RID: 13406
			public float Value;

			// Token: 0x0400345F RID: 13407
			public Vector3 Color;

			// Token: 0x04003460 RID: 13408
			public Vector2 Scale;

			// Token: 0x04003461 RID: 13409
			public Vector2 TTL;
		}
	}
}
