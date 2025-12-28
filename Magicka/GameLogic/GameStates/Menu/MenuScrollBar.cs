using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PolygonHead;
using PolygonHead.Effects;

namespace Magicka.GameLogic.GameStates.Menu
{
	// Token: 0x0200050B RID: 1291
	public class MenuScrollBar : MenuItem
	{
		// Token: 0x06002699 RID: 9881 RVA: 0x00119470 File Offset: 0x00117670
		public MenuScrollBar(Vector2 iPosition, float iHeight, int iMaxValue)
		{
			iMaxValue = Math.Max(0, iMaxValue);
			this.mHeight = iHeight - 128f;
			this.mMaxValue = iMaxValue;
			this.mColor = new Vector4(1f);
			this.mValue = 0;
			this.mScrollLength = this.mHeight / (float)iMaxValue;
			this.mSize.X = 64f;
			this.mSize.Y = iHeight;
			this.mPosition = iPosition;
			this.mScrollUp = default(Vector4);
			this.mScrollDown = default(Vector4);
			this.mScrollDrag = default(Vector4);
			this.UpdateBoundingBox();
			this.mTransform = Matrix.Identity;
			this.mTransform.M41 = this.mPosition.X;
			this.mTransform.M42 = this.mPosition.Y;
			if (MenuScrollBar.sVertices == null)
			{
				MenuScrollBar.sTexture = Game.Instance.Content.Load<Texture2D>("UI/Menu/Pages");
				float num = 64f / (float)MenuScrollBar.sTexture.Width;
				float num2 = 64f / (float)MenuScrollBar.sTexture.Height;
				VertexPositionTexture[] array = new VertexPositionTexture[24];
				float num3 = 1280f / (float)MenuScrollBar.sTexture.Width;
				float num4 = 96f / (float)MenuScrollBar.sTexture.Height;
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
				num4 = 64f / (float)MenuScrollBar.sTexture.Width;
				array[8].Position = new Vector3(-32f, -32f, 0f);
				array[8].TextureCoordinate = new Vector2(num3, num4);
				array[9].Position = new Vector3(32f, -32f, 0f);
				array[9].TextureCoordinate = new Vector2(num3 + num, num4);
				array[10].Position = new Vector3(32f, 32f, 0f);
				array[10].TextureCoordinate = new Vector2(num3 + num, num4 + num2);
				array[11].Position = new Vector3(-32f, 32f, 0f);
				array[11].TextureCoordinate = new Vector2(num3, num4 + num2);
				num3 = 1216f / (float)MenuScrollBar.sTexture.Width;
				num4 = 64f / (float)MenuScrollBar.sTexture.Height;
				array[12].Position = new Vector3(-32f, -32f, 0f);
				array[12].TextureCoordinate = new Vector2(num3, num4);
				array[13].Position = new Vector3(32f, -32f, 0f);
				array[13].TextureCoordinate = new Vector2(num3 + num, num4);
				array[14].Position = new Vector3(32f, 32f, 0f);
				array[14].TextureCoordinate = new Vector2(num3 + num, num4 + num2);
				array[15].Position = new Vector3(-32f, 32f, 0f);
				array[15].TextureCoordinate = new Vector2(num3, num4 + num2);
				num4 = 32f / (float)MenuScrollBar.sTexture.Height;
				num2 = 32f / (float)MenuScrollBar.sTexture.Height;
				array[16].Position = new Vector3(-32f, -16f, 0f);
				array[16].TextureCoordinate = new Vector2(num3, num4);
				array[17].Position = new Vector3(32f, -16f, 0f);
				array[17].TextureCoordinate = new Vector2(num3 + num, num4);
				array[18].Position = new Vector3(32f, 16f, 0f);
				array[18].TextureCoordinate = new Vector2(num3 + num, num4 + num2);
				array[19].Position = new Vector3(-32f, 16f, 0f);
				array[19].TextureCoordinate = new Vector2(num3, num4 + num2);
				num4 = 128f / (float)MenuScrollBar.sTexture.Height;
				array[20].Position = new Vector3(-32f, -16f, 0f);
				array[20].TextureCoordinate = new Vector2(num3, num4);
				array[21].Position = new Vector3(32f, -16f, 0f);
				array[21].TextureCoordinate = new Vector2(num3 + num, num4);
				array[22].Position = new Vector3(32f, 16f, 0f);
				array[22].TextureCoordinate = new Vector2(num3 + num, num4 + num2);
				array[23].Position = new Vector3(-32f, 16f, 0f);
				array[23].TextureCoordinate = new Vector2(num3, num4 + num2);
				MenuScrollBar.sVertexStride = VertexPositionTexture.SizeInBytes;
				GraphicsDevice graphicsDevice = Game.Instance.GraphicsDevice;
				lock (graphicsDevice)
				{
					MenuScrollBar.sVertexDeclaration = RenderManager.Instance.CreateVertexDeclaration(VertexPositionTexture.VertexElements);
					MenuScrollBar.sVertices = new VertexBuffer(graphicsDevice, array.Length * VertexPositionTexture.SizeInBytes, BufferUsage.WriteOnly);
					MenuScrollBar.sVertices.SetData<VertexPositionTexture>(array);
				}
			}
		}

		// Token: 0x17000914 RID: 2324
		// (get) Token: 0x0600269A RID: 9882 RVA: 0x00119BE4 File Offset: 0x00117DE4
		// (set) Token: 0x0600269B RID: 9883 RVA: 0x00119BEC File Offset: 0x00117DEC
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

		// Token: 0x17000915 RID: 2325
		// (get) Token: 0x0600269C RID: 9884 RVA: 0x00119C1F File Offset: 0x00117E1F
		public int MaxValue
		{
			get
			{
				return this.mMaxValue;
			}
		}

		// Token: 0x17000916 RID: 2326
		// (get) Token: 0x0600269D RID: 9885 RVA: 0x00119C27 File Offset: 0x00117E27
		// (set) Token: 0x0600269E RID: 9886 RVA: 0x00119C35 File Offset: 0x00117E35
		public float Height
		{
			get
			{
				return this.mHeight + 128f;
			}
			set
			{
				this.mHeight = value - 128f;
				this.mSize.Y = value;
				this.mScrollLength = this.mHeight / (float)this.mMaxValue;
				this.UpdateBoundingBox();
			}
		}

		// Token: 0x17000917 RID: 2327
		// (get) Token: 0x0600269F RID: 9887 RVA: 0x00119C6C File Offset: 0x00117E6C
		// (set) Token: 0x060026A0 RID: 9888 RVA: 0x00119CBE File Offset: 0x00117EBE
		public Vector2 TextureOffset
		{
			get
			{
				return new Vector2
				{
					X = this.mTextureOffset.X * (float)MenuScrollBar.sTexture.Width,
					Y = this.mTextureOffset.Y * (float)MenuScrollBar.sTexture.Height
				};
			}
			set
			{
				this.mTextureOffset.X = value.X / (float)MenuScrollBar.sTexture.Width;
				this.mTextureOffset.Y = value.Y / (float)MenuScrollBar.sTexture.Height;
			}
		}

		// Token: 0x060026A1 RID: 9889 RVA: 0x00119CFC File Offset: 0x00117EFC
		public void ScrollTo(float iY)
		{
			float num = (iY - this.mTopLeft.Y - 64f * this.mScale) / this.mScale;
			this.Value = (int)(0.5f + num / this.mScrollLength);
		}

		// Token: 0x17000918 RID: 2328
		// (get) Token: 0x060026A2 RID: 9890 RVA: 0x00119D40 File Offset: 0x00117F40
		// (set) Token: 0x060026A3 RID: 9891 RVA: 0x00119D48 File Offset: 0x00117F48
		public bool Grabbed { get; set; }

		// Token: 0x060026A4 RID: 9892 RVA: 0x00119D51 File Offset: 0x00117F51
		public void SetMaxValue(int iMaxValue)
		{
			iMaxValue = Math.Max(0, iMaxValue);
			if (this.mMaxValue == iMaxValue)
			{
				return;
			}
			this.mMaxValue = iMaxValue;
			this.mValue = 0;
			this.mScrollLength = this.mHeight / (float)iMaxValue;
			this.UpdateBoundingBox();
		}

		// Token: 0x060026A5 RID: 9893 RVA: 0x00119D8C File Offset: 0x00117F8C
		public bool InsideDragBounds(Vector2 iPoint)
		{
			return iPoint.X >= this.mScrollDrag.X & iPoint.Y >= this.mScrollDrag.Y & iPoint.X <= this.mScrollDrag.Z & iPoint.Y <= this.mScrollDrag.W;
		}

		// Token: 0x060026A6 RID: 9894 RVA: 0x00119DF8 File Offset: 0x00117FF8
		public bool InsideDragBounds(MouseState iPoint)
		{
			return (float)iPoint.X >= this.mScrollDrag.X & (float)iPoint.Y >= this.mScrollDrag.Y & (float)iPoint.X <= this.mScrollDrag.Z & (float)iPoint.Y <= this.mScrollDrag.W;
		}

		// Token: 0x060026A7 RID: 9895 RVA: 0x00119E68 File Offset: 0x00118068
		public bool InsideDragUpBounds(Vector2 iPoint)
		{
			return iPoint.Y < this.mScrollDragUp;
		}

		// Token: 0x060026A8 RID: 9896 RVA: 0x00119E79 File Offset: 0x00118079
		public bool InsideDragUpBounds(MouseState iPoint)
		{
			return (float)iPoint.Y < this.mScrollDragUp;
		}

		// Token: 0x060026A9 RID: 9897 RVA: 0x00119E8B File Offset: 0x0011808B
		public bool InsideDragDownBounds(Vector2 iPoint)
		{
			return iPoint.Y > this.mScrollDragDown;
		}

		// Token: 0x060026AA RID: 9898 RVA: 0x00119E9C File Offset: 0x0011809C
		public bool InsideDragDownBounds(MouseState iPoint)
		{
			return (float)iPoint.Y > this.mScrollDragDown;
		}

		// Token: 0x060026AB RID: 9899 RVA: 0x00119EB0 File Offset: 0x001180B0
		public bool InsideUpBounds(Vector2 iPoint)
		{
			return iPoint.X >= this.mScrollUp.X & iPoint.Y <= this.mScrollUp.W & iPoint.X <= this.mScrollUp.Z & iPoint.Y >= this.mScrollUp.Y;
		}

		// Token: 0x060026AC RID: 9900 RVA: 0x00119F1C File Offset: 0x0011811C
		public bool InsideUpBounds(MouseState iPoint)
		{
			return (float)iPoint.X >= this.mScrollUp.X & (float)iPoint.Y <= this.mScrollUp.W & (float)iPoint.X <= this.mScrollUp.Z & (float)iPoint.Y >= this.mScrollUp.Y;
		}

		// Token: 0x060026AD RID: 9901 RVA: 0x00119F8C File Offset: 0x0011818C
		public bool InsideDownBounds(MouseState iPoint)
		{
			return (float)iPoint.X >= this.mScrollDown.X & (float)iPoint.Y <= this.mScrollDown.W & (float)iPoint.X <= this.mScrollDown.Z & (float)iPoint.Y >= this.mScrollDown.Y;
		}

		// Token: 0x060026AE RID: 9902 RVA: 0x00119FFC File Offset: 0x001181FC
		public bool InsideDownBounds(Vector2 iPoint)
		{
			return iPoint.X >= this.mScrollDown.X & iPoint.Y <= this.mScrollDown.W & iPoint.X <= this.mScrollDown.Z & iPoint.Y >= this.mScrollDown.Y;
		}

		// Token: 0x060026AF RID: 9903 RVA: 0x0011A068 File Offset: 0x00118268
		private void UpdateScrollDrag()
		{
			this.mScrollDrag.X = this.mTopLeft.X;
			this.mScrollDrag.Y = this.mTopLeft.Y + (32f + (float)this.mValue * this.mScrollLength) * this.mScale;
			this.mScrollDrag.Z = this.mBottomRight.X;
			this.mScrollDrag.W = this.mTopLeft.Y + (32f + (float)this.mValue * this.mScrollLength + 64f) * this.mScale;
			this.mScrollDragUp = this.mScrollDrag.Y + (64f - this.mScrollLength) * 0.5f * this.mScale;
			this.mScrollDragDown = this.mScrollDrag.Y + (64f + this.mScrollLength) * 0.5f * this.mScale;
			this.mScrollUp.X = this.mTopLeft.X;
			this.mScrollUp.Y = this.mTopLeft.Y;
			this.mScrollUp.Z = this.mBottomRight.X;
			this.mScrollUp.W = this.mTopLeft.Y + 64f * this.mScale;
			this.mScrollDown.X = this.mTopLeft.X;
			this.mScrollDown.Y = this.mBottomRight.Y - 64f * this.mScale;
			this.mScrollDown.Z = this.mBottomRight.X;
			this.mScrollDown.W = this.mBottomRight.Y;
		}

		// Token: 0x060026B0 RID: 9904 RVA: 0x0011A22C File Offset: 0x0011842C
		protected override void UpdateBoundingBox()
		{
			this.mTopLeft.X = this.mPosition.X - this.mSize.X * 0.5f * this.mScale;
			this.mTopLeft.Y = this.mPosition.Y - this.mSize.Y * 0.5f * this.mScale;
			this.mBottomRight.X = this.mPosition.X + this.mSize.X * 0.5f * this.mScale;
			this.mBottomRight.Y = this.mPosition.Y + this.mSize.Y * 0.5f * this.mScale;
			this.UpdateScrollDrag();
		}

		// Token: 0x060026B1 RID: 9905 RVA: 0x0011A2FB File Offset: 0x001184FB
		public override void Draw(GUIBasicEffect iEffect)
		{
			this.Draw(iEffect, this.mScale);
		}

		// Token: 0x060026B2 RID: 9906 RVA: 0x0011A30C File Offset: 0x0011850C
		public override void Draw(GUIBasicEffect iEffect, float iScale)
		{
			iEffect.GraphicsDevice.Vertices[0].SetSource(MenuScrollBar.sVertices, 0, MenuScrollBar.sVertexStride);
			iEffect.GraphicsDevice.VertexDeclaration = MenuScrollBar.sVertexDeclaration;
			iEffect.TextureOffset = this.mTextureOffset;
			iEffect.Saturation = 1f;
			iEffect.Color = (this.mSelected ? this.mColorSelected : this.mColor);
			iEffect.Texture = MenuScrollBar.sTexture;
			iEffect.TextureEnabled = true;
			iEffect.TextureScale = new Vector2(1f, 1f);
			Matrix transform = new Matrix
			{
				M44 = 1f,
				M41 = this.mPosition.X,
				M42 = this.mPosition.Y,
				M11 = iScale,
				M22 = iScale * (this.mHeight - this.mSize.X * 0.5f) / this.mSize.X
			};
			iEffect.Transform = transform;
			iEffect.CommitChanges();
			iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 12, 2);
			transform.M41 = this.mPosition.X;
			transform.M42 = this.mTopLeft.Y + this.mSize.X * iScale;
			transform.M11 = iScale;
			transform.M22 = iScale;
			iEffect.Transform = transform;
			iEffect.CommitChanges();
			iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 16, 2);
			transform.M41 = this.mPosition.X;
			transform.M42 = this.mBottomRight.Y - this.mSize.X * iScale;
			iEffect.Transform = transform;
			iEffect.CommitChanges();
			iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 20, 2);
			transform.M41 = this.mPosition.X + (32f - this.mSize.X * 0.5f) * iScale;
			transform.M42 = this.mPosition.Y + (32f - this.mSize.Y * 0.5f) * iScale;
			transform.M11 = iScale;
			transform.M22 = iScale;
			iEffect.Transform = transform;
			iEffect.CommitChanges();
			iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
			transform.M42 = this.mPosition.Y + (this.mSize.Y * 0.5f - 32f) * iScale;
			transform.M11 *= -1f;
			transform.M22 *= -1f;
			iEffect.Transform = transform;
			iEffect.CommitChanges();
			iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 4, 2);
			transform.M42 = this.mPosition.Y - this.mHeight * 0.5f * iScale + (float)this.mValue * this.mScrollLength * iScale;
			iEffect.Transform = transform;
			iEffect.CommitChanges();
			iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 8, 2);
			iEffect.GraphicsDevice.Vertices[0].SetSource(null, 0, 0);
			iEffect.TextureOffset = default(Vector2);
		}

		// Token: 0x060026B3 RID: 9907 RVA: 0x0011A63E File Offset: 0x0011883E
		public override void LanguageChanged()
		{
		}

		// Token: 0x040029F0 RID: 10736
		public const float SIZE = 64f;

		// Token: 0x040029F1 RID: 10737
		private static Texture2D sTexture;

		// Token: 0x040029F2 RID: 10738
		private static VertexBuffer sVertices;

		// Token: 0x040029F3 RID: 10739
		private static VertexDeclaration sVertexDeclaration;

		// Token: 0x040029F4 RID: 10740
		private static int sVertexStride;

		// Token: 0x040029F5 RID: 10741
		private int mMaxValue;

		// Token: 0x040029F6 RID: 10742
		private float mHeight;

		// Token: 0x040029F7 RID: 10743
		private float mScrollLength;

		// Token: 0x040029F8 RID: 10744
		private int mValue;

		// Token: 0x040029F9 RID: 10745
		private Vector2 mTextureOffset;

		// Token: 0x040029FA RID: 10746
		private Vector2 mSize;

		// Token: 0x040029FB RID: 10747
		private Vector4 mScrollUp;

		// Token: 0x040029FC RID: 10748
		private Vector4 mScrollDown;

		// Token: 0x040029FD RID: 10749
		private Vector4 mScrollDrag;

		// Token: 0x040029FE RID: 10750
		private float mScrollDragUp;

		// Token: 0x040029FF RID: 10751
		private float mScrollDragDown;
	}
}
