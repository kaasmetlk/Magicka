using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PolygonHead;
using PolygonHead.Effects;

namespace Magicka.GameLogic.GameStates.Menu
{
	// Token: 0x0200050A RID: 1290
	public class MenuScrollSlider : MenuItem
	{
		// Token: 0x0600267E RID: 9854 RVA: 0x001182FC File Offset: 0x001164FC
		public MenuScrollSlider(Vector2 iPosition, float iWidth, int iMaxValue)
		{
			iMaxValue = Math.Max(1, iMaxValue);
			this.mWidth = iWidth - 128f;
			this.mMaxValue = iMaxValue;
			this.mColor = new Vector4(1f);
			this.mValue = 0;
			this.mScrollLength = this.mWidth / (float)iMaxValue;
			this.mSize.X = iWidth;
			this.mSize.Y = 64f;
			this.mPosition = iPosition;
			this.mScrollLeft = default(Vector4);
			this.mScrollRight = default(Vector4);
			this.mScrollDrag = default(Vector4);
			this.UpdateBoundingBox();
			Matrix.CreateRotationZ(1.5707964f, out this.mTransform);
			this.mTransform.M41 = this.mPosition.X;
			this.mTransform.M42 = this.mPosition.Y;
			if (MenuScrollSlider.sVertices == null)
			{
				MenuScrollSlider.sTexture = Game.Instance.Content.Load<Texture2D>("UI/Menu/Pages");
				float num = 64f / (float)MenuScrollSlider.sTexture.Width;
				float num2 = 64f / (float)MenuScrollSlider.sTexture.Height;
				VertexPositionTexture[] array = new VertexPositionTexture[24];
				float num3 = 1280f / (float)MenuScrollSlider.sTexture.Width;
				float num4 = 96f / (float)MenuScrollSlider.sTexture.Height;
				array[0].Position = new Vector3(-32f, -32f, 0f);
				array[0].TextureCoordinate = new Vector2(num3, num4);
				array[1].Position = new Vector3(32f, -32f, 0f);
				array[1].TextureCoordinate = new Vector2(num3 + num, num4);
				array[2].Position = new Vector3(32f, 32f, 0f);
				array[2].TextureCoordinate = new Vector2(num3 + num, num4 + num2);
				array[3].Position = new Vector3(-32f, 32f, 0f);
				array[3].TextureCoordinate = new Vector2(num3, num4 + num2);
				array[4].Position = new Vector3(-32f, -32f, 0f);
				array[4].TextureCoordinate = new Vector2(num3, num4);
				array[5].Position = new Vector3(32f, -32f, 0f);
				array[5].TextureCoordinate = new Vector2(num3 + num, num4);
				array[6].Position = new Vector3(32f, 32f, 0f);
				array[6].TextureCoordinate = new Vector2(num3 + num, num4 + num2);
				array[7].Position = new Vector3(-32f, 32f, 0f);
				array[7].TextureCoordinate = new Vector2(num3, num4 + num2);
				num4 = 64f / (float)MenuScrollSlider.sTexture.Width;
				array[8].Position = new Vector3(-32f, -32f, 0f);
				array[8].TextureCoordinate = new Vector2(num3, num4);
				array[9].Position = new Vector3(32f, -32f, 0f);
				array[9].TextureCoordinate = new Vector2(num3 + num, num4);
				array[10].Position = new Vector3(32f, 32f, 0f);
				array[10].TextureCoordinate = new Vector2(num3 + num, num4 + num2);
				array[11].Position = new Vector3(-32f, 32f, 0f);
				array[11].TextureCoordinate = new Vector2(num3, num4 + num2);
				num3 = 1216f / (float)MenuScrollSlider.sTexture.Width;
				num4 = 64f / (float)MenuScrollSlider.sTexture.Height;
				array[12].Position = new Vector3(-32f, -32f, 0f);
				array[12].TextureCoordinate = new Vector2(num3, num4);
				array[13].Position = new Vector3(32f, -32f, 0f);
				array[13].TextureCoordinate = new Vector2(num3 + num, num4);
				array[14].Position = new Vector3(32f, 32f, 0f);
				array[14].TextureCoordinate = new Vector2(num3 + num, num4 + num2);
				array[15].Position = new Vector3(-32f, 32f, 0f);
				array[15].TextureCoordinate = new Vector2(num3, num4 + num2);
				num4 = 32f / (float)MenuScrollSlider.sTexture.Height;
				num2 = 32f / (float)MenuScrollSlider.sTexture.Height;
				array[16].Position = new Vector3(-32f, -16f, 0f);
				array[16].TextureCoordinate = new Vector2(num3, num4);
				array[17].Position = new Vector3(32f, -16f, 0f);
				array[17].TextureCoordinate = new Vector2(num3 + num, num4);
				array[18].Position = new Vector3(32f, 16f, 0f);
				array[18].TextureCoordinate = new Vector2(num3 + num, num4 + num2);
				array[19].Position = new Vector3(-32f, 16f, 0f);
				array[19].TextureCoordinate = new Vector2(num3, num4 + num2);
				num4 = 128f / (float)MenuScrollSlider.sTexture.Height;
				array[20].Position = new Vector3(-32f, -16f, 0f);
				array[20].TextureCoordinate = new Vector2(num3, num4);
				array[21].Position = new Vector3(32f, -16f, 0f);
				array[21].TextureCoordinate = new Vector2(num3 + num, num4);
				array[22].Position = new Vector3(32f, 16f, 0f);
				array[22].TextureCoordinate = new Vector2(num3 + num, num4 + num2);
				array[23].Position = new Vector3(-32f, 16f, 0f);
				array[23].TextureCoordinate = new Vector2(num3, num4 + num2);
				MenuScrollSlider.sVertexStride = VertexPositionTexture.SizeInBytes;
				GraphicsDevice graphicsDevice = Game.Instance.GraphicsDevice;
				lock (graphicsDevice)
				{
					MenuScrollSlider.sVertexDeclaration = RenderManager.Instance.CreateVertexDeclaration(VertexPositionTexture.VertexElements);
					MenuScrollSlider.sVertices = new VertexBuffer(graphicsDevice, array.Length * VertexPositionTexture.SizeInBytes, BufferUsage.WriteOnly);
					MenuScrollSlider.sVertices.SetData<VertexPositionTexture>(array);
				}
			}
		}

		// Token: 0x1700090F RID: 2319
		// (get) Token: 0x0600267F RID: 9855 RVA: 0x00118A74 File Offset: 0x00116C74
		// (set) Token: 0x06002680 RID: 9856 RVA: 0x00118A7C File Offset: 0x00116C7C
		public int Value
		{
			get
			{
				return this.mValue;
			}
			set
			{
				if (value < 0)
				{
					this.mValue = 0;
				}
				else if (value > this.mMaxValue)
				{
					this.mValue = this.mMaxValue;
				}
				else
				{
					this.mValue = value;
				}
				this.UpdateScrollDrag();
			}
		}

		// Token: 0x17000910 RID: 2320
		// (get) Token: 0x06002681 RID: 9857 RVA: 0x00118AAF File Offset: 0x00116CAF
		public int MaxValue
		{
			get
			{
				return this.mMaxValue;
			}
		}

		// Token: 0x17000911 RID: 2321
		// (get) Token: 0x06002682 RID: 9858 RVA: 0x00118AB7 File Offset: 0x00116CB7
		// (set) Token: 0x06002683 RID: 9859 RVA: 0x00118AC5 File Offset: 0x00116CC5
		public float Width
		{
			get
			{
				return this.mWidth + 128f;
			}
			set
			{
				this.mWidth = value - 128f;
				this.mSize.X = value;
				this.mScrollLength = this.mWidth / (float)this.mMaxValue;
				this.UpdateBoundingBox();
			}
		}

		// Token: 0x17000912 RID: 2322
		// (get) Token: 0x06002684 RID: 9860 RVA: 0x00118AFC File Offset: 0x00116CFC
		// (set) Token: 0x06002685 RID: 9861 RVA: 0x00118B4E File Offset: 0x00116D4E
		public Vector2 TextureOffset
		{
			get
			{
				return new Vector2
				{
					X = this.mTextureOffset.X * (float)MenuScrollSlider.sTexture.Width,
					Y = this.mTextureOffset.Y * (float)MenuScrollSlider.sTexture.Height
				};
			}
			set
			{
				this.mTextureOffset.X = value.X / (float)MenuScrollSlider.sTexture.Width;
				this.mTextureOffset.Y = value.Y / (float)MenuScrollSlider.sTexture.Height;
			}
		}

		// Token: 0x06002686 RID: 9862 RVA: 0x00118B8C File Offset: 0x00116D8C
		public void ScrollTo(float iX)
		{
			float num = (iX - this.mTopLeft.X - 64f * this.mScale) / this.mScale;
			this.Value = (int)(0.5f + num / this.mScrollLength);
		}

		// Token: 0x17000913 RID: 2323
		// (get) Token: 0x06002687 RID: 9863 RVA: 0x00118BD0 File Offset: 0x00116DD0
		// (set) Token: 0x06002688 RID: 9864 RVA: 0x00118BD8 File Offset: 0x00116DD8
		public bool Grabbed { get; set; }

		// Token: 0x06002689 RID: 9865 RVA: 0x00118BE1 File Offset: 0x00116DE1
		public void SetMaxValue(int iMaxValue)
		{
			iMaxValue = Math.Max(0, iMaxValue);
			if (this.mMaxValue == iMaxValue)
			{
				return;
			}
			this.mMaxValue = iMaxValue;
			this.mValue = 0;
			this.mScrollLength = this.mWidth / (float)iMaxValue;
			this.UpdateBoundingBox();
		}

		// Token: 0x0600268A RID: 9866 RVA: 0x00118C1C File Offset: 0x00116E1C
		public bool InsideDragBounds(Vector2 iPoint)
		{
			return iPoint.X >= this.mScrollDrag.X & iPoint.Y >= this.mScrollDrag.Y & iPoint.X <= this.mScrollDrag.Z & iPoint.Y <= this.mScrollDrag.W;
		}

		// Token: 0x0600268B RID: 9867 RVA: 0x00118C88 File Offset: 0x00116E88
		public bool InsideDragBounds(MouseState iPoint)
		{
			return (float)iPoint.X >= this.mScrollDrag.X & (float)iPoint.Y >= this.mScrollDrag.Y & (float)iPoint.X <= this.mScrollDrag.Z & (float)iPoint.Y <= this.mScrollDrag.W;
		}

		// Token: 0x0600268C RID: 9868 RVA: 0x00118CF8 File Offset: 0x00116EF8
		public bool InsideDragLeftBounds(Vector2 iPoint)
		{
			return iPoint.X < this.mScrollDragLeft;
		}

		// Token: 0x0600268D RID: 9869 RVA: 0x00118D09 File Offset: 0x00116F09
		public bool InsideDragLeftBounds(MouseState iPoint)
		{
			return (float)iPoint.X < this.mScrollDragLeft;
		}

		// Token: 0x0600268E RID: 9870 RVA: 0x00118D1B File Offset: 0x00116F1B
		public bool InsideDragRightBounds(Vector2 iPoint)
		{
			return iPoint.X > this.mScrollDragRight;
		}

		// Token: 0x0600268F RID: 9871 RVA: 0x00118D2C File Offset: 0x00116F2C
		public bool InsideDragRightBounds(MouseState iPoint)
		{
			return (float)iPoint.X > this.mScrollDragRight;
		}

		// Token: 0x06002690 RID: 9872 RVA: 0x00118D40 File Offset: 0x00116F40
		public bool InsideLeftBounds(Vector2 iPoint)
		{
			return iPoint.X >= this.mScrollLeft.X & iPoint.Y >= this.mScrollLeft.Y & iPoint.X <= this.mScrollLeft.Z & iPoint.Y <= this.mScrollLeft.W;
		}

		// Token: 0x06002691 RID: 9873 RVA: 0x00118DAC File Offset: 0x00116FAC
		public bool InsideLeftBounds(MouseState iPoint)
		{
			return (float)iPoint.X >= this.mScrollLeft.X & (float)iPoint.Y <= this.mScrollLeft.W & (float)iPoint.X <= this.mScrollLeft.Z & (float)iPoint.Y >= this.mScrollLeft.Y;
		}

		// Token: 0x06002692 RID: 9874 RVA: 0x00118E1C File Offset: 0x0011701C
		public bool InsideRightBounds(Vector2 iPoint)
		{
			return iPoint.X >= this.mScrollRight.X & iPoint.Y <= this.mScrollRight.W & iPoint.X <= this.mScrollRight.Z & iPoint.Y >= this.mScrollRight.Y;
		}

		// Token: 0x06002693 RID: 9875 RVA: 0x00118E88 File Offset: 0x00117088
		public bool InsideRightBounds(MouseState iPoint)
		{
			return (float)iPoint.X >= this.mScrollRight.X & (float)iPoint.Y <= this.mScrollRight.W & (float)iPoint.X <= this.mScrollRight.Z & (float)iPoint.Y >= this.mScrollRight.Y;
		}

		// Token: 0x06002694 RID: 9876 RVA: 0x00118EF8 File Offset: 0x001170F8
		private void UpdateScrollDrag()
		{
			this.mScrollDrag.X = this.mTopLeft.X + (48f + (float)this.mValue * this.mScrollLength) * this.mScale;
			this.mScrollDrag.Y = this.mTopLeft.Y;
			this.mScrollDrag.Z = this.mTopLeft.X + (48f + (float)this.mValue * this.mScrollLength + 32f) * this.mScale;
			this.mScrollDrag.W = this.mBottomRight.Y;
			this.mScrollDragLeft = this.mScrollDrag.X;
			this.mScrollDragRight = this.mScrollDrag.Z;
			this.mScrollLeft.X = this.mTopLeft.X;
			this.mScrollLeft.Y = this.mTopLeft.Y;
			this.mScrollLeft.Z = this.mTopLeft.X + 64f * this.mScale;
			this.mScrollLeft.W = this.mBottomRight.Y;
			this.mScrollRight.X = this.mBottomRight.X - 64f * this.mScale;
			this.mScrollRight.Y = this.mTopLeft.Y;
			this.mScrollRight.Z = this.mBottomRight.X;
			this.mScrollRight.W = this.mBottomRight.Y;
		}

		// Token: 0x06002695 RID: 9877 RVA: 0x00119088 File Offset: 0x00117288
		protected override void UpdateBoundingBox()
		{
			this.mTopLeft.X = this.mPosition.X - this.mSize.X * 0.5f * this.mScale;
			this.mTopLeft.Y = this.mPosition.Y - this.mSize.Y * 0.5f * this.mScale;
			this.mBottomRight.X = this.mPosition.X + this.mSize.X * 0.5f * this.mScale;
			this.mBottomRight.Y = this.mPosition.Y + this.mSize.Y * 0.5f * this.mScale;
			this.UpdateScrollDrag();
		}

		// Token: 0x06002696 RID: 9878 RVA: 0x00119157 File Offset: 0x00117357
		public override void Draw(GUIBasicEffect iEffect)
		{
			this.Draw(iEffect, this.mScale);
		}

		// Token: 0x06002697 RID: 9879 RVA: 0x00119168 File Offset: 0x00117368
		public override void Draw(GUIBasicEffect iEffect, float iScale)
		{
			iEffect.GraphicsDevice.Vertices[0].SetSource(MenuScrollSlider.sVertices, 0, MenuScrollSlider.sVertexStride);
			iEffect.GraphicsDevice.VertexDeclaration = MenuScrollSlider.sVertexDeclaration;
			iEffect.TextureOffset = this.mTextureOffset;
			iEffect.Saturation = 1f;
			iEffect.Color = this.mColor;
			iEffect.Texture = MenuScrollSlider.sTexture;
			iEffect.TextureEnabled = true;
			iEffect.TextureScale = Vector2.One;
			Matrix transform = new Matrix
			{
				M44 = 1f,
				M41 = this.mPosition.X,
				M42 = this.mPosition.Y,
				M12 = -iScale,
				M21 = iScale * (this.mWidth - 32f) / this.mSize.Y
			};
			iEffect.Transform = transform;
			iEffect.CommitChanges();
			iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 12, 2);
			transform.M41 = this.mTopLeft.X + 64f * iScale;
			transform.M42 = this.mPosition.Y;
			transform.M12 = -iScale;
			transform.M21 = iScale;
			iEffect.Transform = transform;
			iEffect.CommitChanges();
			iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 16, 2);
			transform.M41 = base.BottomRight.X - 64f * iScale;
			transform.M42 = this.mPosition.Y;
			transform.M12 = -iScale;
			transform.M21 = iScale;
			iEffect.Transform = transform;
			iEffect.CommitChanges();
			iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 20, 2);
			transform.M41 = this.mPosition.X + (this.mSize.Y * 0.5f + this.mWidth * 0.5f) * iScale;
			transform.M12 = iScale;
			transform.M21 = -iScale;
			iEffect.Transform = transform;
			iEffect.CommitChanges();
			iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
			transform.M41 = this.mPosition.X - (this.mSize.Y * 0.5f + this.mWidth * 0.5f) * iScale;
			transform.M12 *= -1f;
			transform.M21 *= -1f;
			iEffect.Transform = transform;
			iEffect.CommitChanges();
			iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 4, 2);
			transform.M12 = iScale;
			transform.M21 = -1f;
			transform.M11 = (transform.M22 = 0f);
			transform.M41 = this.mTopLeft.X + (this.mSize.Y + (float)this.mValue * this.mScrollLength) * iScale;
			iEffect.Transform = transform;
			iEffect.CommitChanges();
			iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 8, 2);
			iEffect.TextureOffset = default(Vector2);
		}

		// Token: 0x06002698 RID: 9880 RVA: 0x0011946C File Offset: 0x0011766C
		public override void LanguageChanged()
		{
		}

		// Token: 0x040029DF RID: 10719
		private const float SIZE = 64f;

		// Token: 0x040029E0 RID: 10720
		private static Texture2D sTexture;

		// Token: 0x040029E1 RID: 10721
		private static VertexBuffer sVertices;

		// Token: 0x040029E2 RID: 10722
		private static VertexDeclaration sVertexDeclaration;

		// Token: 0x040029E3 RID: 10723
		private static int sVertexStride;

		// Token: 0x040029E4 RID: 10724
		private int mMaxValue;

		// Token: 0x040029E5 RID: 10725
		private float mWidth;

		// Token: 0x040029E6 RID: 10726
		private float mScrollLength;

		// Token: 0x040029E7 RID: 10727
		private int mValue;

		// Token: 0x040029E8 RID: 10728
		private Vector2 mTextureOffset;

		// Token: 0x040029E9 RID: 10729
		private Vector2 mSize;

		// Token: 0x040029EA RID: 10730
		private Vector4 mScrollLeft;

		// Token: 0x040029EB RID: 10731
		private Vector4 mScrollRight;

		// Token: 0x040029EC RID: 10732
		private Vector4 mScrollDrag;

		// Token: 0x040029ED RID: 10733
		private float mScrollDragLeft;

		// Token: 0x040029EE RID: 10734
		private float mScrollDragRight;
	}
}
