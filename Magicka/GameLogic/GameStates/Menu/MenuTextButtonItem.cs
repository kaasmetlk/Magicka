using System;
using Magicka.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;

namespace Magicka.GameLogic.GameStates.Menu
{
	// Token: 0x02000510 RID: 1296
	public class MenuTextButtonItem : MenuItem
	{
		// Token: 0x060026EA RID: 9962 RVA: 0x0011BE0C File Offset: 0x0011A00C
		static MenuTextButtonItem()
		{
			lock (Game.Instance.GraphicsDevice)
			{
				MenuTextButtonItem.sVertices = new VertexBuffer(Game.Instance.GraphicsDevice, Defines.QUAD_TEX_VERTS_C.Length * VertexPositionTexture.SizeInBytes, BufferUsage.WriteOnly);
				MenuTextButtonItem.sVertices.SetData<VertexPositionTexture>(Defines.QUAD_TEX_VERTS_C);
				MenuTextButtonItem.sDeclaration = new VertexDeclaration(Game.Instance.GraphicsDevice, VertexPositionTexture.VertexElements);
			}
		}

		// Token: 0x060026EB RID: 9963 RVA: 0x0011BEB8 File Offset: 0x0011A0B8
		public MenuTextButtonItem(Vector2 iPosition, Texture2D iTexture, Vector2 iUVOffset, Vector2 iUVSize, int iText, BitmapFont iFont, float iMinWidth, TextAlign iButtonAlignment) : this(iPosition, iTexture, iUVOffset, iUVSize, iText, iFont, iMinWidth, float.MaxValue, iButtonAlignment)
		{
		}

		// Token: 0x060026EC RID: 9964 RVA: 0x0011BEE0 File Offset: 0x0011A0E0
		public MenuTextButtonItem(Vector2 iPosition, Texture2D iTexture, Vector2 iUVOffset, Vector2 iUVSize, int iText, BitmapFont iFont, float iMinWidth, float iMaxWidth, TextAlign iButtonAlignment)
		{
			this.mFont = iFont;
			this.mText = iText;
			this.mTexture = iTexture;
			this.mPosition = iPosition;
			this.mUVOffset.X = iUVOffset.X / (float)iTexture.Width;
			this.mUVOffset.Y = iUVOffset.Y / (float)iTexture.Height;
			this.mSize = iUVSize;
			this.mUVScale.X = iUVSize.X / (float)iTexture.Width;
			this.mUVScale.Y = iUVSize.Y / (float)iTexture.Height;
			this.mAlignment = iButtonAlignment;
			this.mTitle = new Text(40, iFont, TextAlign.Center, false);
			string @string = LanguageManager.Instance.GetString(iText);
			this.mTitle.SetText(@string);
			this.mTextSize = this.mFont.MeasureText(@string, true);
			this.mMinWidth = iMinWidth;
			this.mMaxWidth = iMaxWidth;
			this.UpdateBoundingBox();
			this.mTransform = Matrix.Identity;
			this.mTransform.M41 = this.mPosition.X;
			this.mTransform.M42 = this.mPosition.Y;
			this.mColor = Defines.DIALOGUE_COLOR_DEFAULT;
			this.mColorDisabled = this.mColor;
		}

		// Token: 0x060026ED RID: 9965 RVA: 0x0011C02C File Offset: 0x0011A22C
		protected override void UpdateBoundingBox()
		{
			float num = Math.Max(this.mMinWidth, Math.Min(this.mMaxWidth, this.mTextSize.X + this.mSize.X * 0.5f));
			switch (this.mAlignment)
			{
			case TextAlign.Left:
				this.mLeftPosition = 0f;
				this.mMiddlePosition = num * 0.5f;
				this.mRightPosition = num;
				goto IL_B9;
			case TextAlign.Right:
				this.mLeftPosition = -num;
				this.mMiddlePosition = -num * 0.5f;
				this.mRightPosition = 0f;
				goto IL_B9;
			}
			this.mLeftPosition = -num * 0.5f;
			this.mMiddlePosition = 0f;
			this.mRightPosition = num * 0.5f;
			IL_B9:
			this.mTopLeft.X = this.mPosition.X + this.mLeftPosition * this.mScale;
			this.mTopLeft.Y = this.mPosition.Y - this.mSize.Y * 0.5f * this.mScale;
			this.mBottomRight.X = this.mPosition.X + this.mRightPosition * this.mScale;
			this.mBottomRight.Y = this.mPosition.Y + this.mSize.Y * 0.5f * this.mScale;
		}

		// Token: 0x060026EE RID: 9966 RVA: 0x0011C198 File Offset: 0x0011A398
		public void SetText(string iText)
		{
			this.mText = 0;
			this.mTitle.SetText(iText);
			this.mTextSize = this.mFont.MeasureText(iText, true);
			this.UpdateBoundingBox();
		}

		// Token: 0x060026EF RID: 9967 RVA: 0x0011C1C6 File Offset: 0x0011A3C6
		public void SetText(int iText)
		{
			this.mText = iText;
			this.LanguageChanged();
		}

		// Token: 0x17000924 RID: 2340
		// (get) Token: 0x060026F0 RID: 9968 RVA: 0x0011C1D5 File Offset: 0x0011A3D5
		// (set) Token: 0x060026F1 RID: 9969 RVA: 0x0011C1DD File Offset: 0x0011A3DD
		public Vector2 UVOffset
		{
			get
			{
				return this.mUVOffset;
			}
			set
			{
				this.mUVOffset = value;
			}
		}

		// Token: 0x17000925 RID: 2341
		// (get) Token: 0x060026F2 RID: 9970 RVA: 0x0011C1E6 File Offset: 0x0011A3E6
		// (set) Token: 0x060026F3 RID: 9971 RVA: 0x0011C1EE File Offset: 0x0011A3EE
		public Vector2 UVScale
		{
			get
			{
				return this.mUVScale;
			}
			set
			{
				this.mUVScale = value;
			}
		}

		// Token: 0x17000926 RID: 2342
		// (get) Token: 0x060026F4 RID: 9972 RVA: 0x0011C1F7 File Offset: 0x0011A3F7
		public Vector2 Size
		{
			get
			{
				return this.mSize;
			}
		}

		// Token: 0x17000927 RID: 2343
		// (get) Token: 0x060026F5 RID: 9973 RVA: 0x0011C1FF File Offset: 0x0011A3FF
		public float RealWidth
		{
			get
			{
				return Math.Max(this.mMinWidth, Math.Min(this.mMaxWidth, this.mTextSize.X + this.mSize.X * 0.5f));
			}
		}

		// Token: 0x060026F6 RID: 9974 RVA: 0x0011C234 File Offset: 0x0011A434
		public override void Draw(GUIBasicEffect iEffect)
		{
			this.Draw(iEffect, this.mScale, this.mAlpha);
		}

		// Token: 0x060026F7 RID: 9975 RVA: 0x0011C249 File Offset: 0x0011A449
		public override void Draw(GUIBasicEffect iEffect, float iScale)
		{
			this.Draw(iEffect, iScale, this.mAlpha);
		}

		// Token: 0x060026F8 RID: 9976 RVA: 0x0011C25C File Offset: 0x0011A45C
		public void Draw(GUIBasicEffect iEffect, float iScale, float iAlpha)
		{
			float num = Math.Max(this.mMinWidth, Math.Min(this.mMaxWidth, this.mTextSize.X + this.mSize.X * 0.5f));
			Vector2 textureOffset = this.mUVOffset;
			Vector2 textureScale = this.mUVScale;
			iEffect.GraphicsDevice.Vertices[0].SetSource(MenuTextButtonItem.sVertices, 0, VertexPositionTexture.SizeInBytes);
			iEffect.GraphicsDevice.VertexDeclaration = MenuTextButtonItem.sDeclaration;
			Vector4 color = Vector4.One;
			color.W = iAlpha;
			iEffect.Color = color;
			iEffect.Texture = this.mTexture;
			iEffect.TextureEnabled = true;
			iEffect.VertexColorEnabled = false;
			if (!this.mEnabled)
			{
				iEffect.Saturation = 0f;
			}
			else if (this.mSelected)
			{
				iEffect.Saturation = 1.5f;
			}
			else
			{
				iEffect.Saturation = 1f;
			}
			Matrix mTransform = this.mTransform;
			mTransform.M22 = iScale * this.mSize.Y;
			mTransform.M11 = iScale * this.mSize.X * 0.25f;
			mTransform.M41 = this.mPosition.X + (this.mLeftPosition + this.mSize.X * 0.25f * 0.5f) * iScale;
			mTransform.M42 = this.mPosition.Y;
			iEffect.Transform = mTransform;
			iEffect.TextureOffset = textureOffset;
			textureScale.X = this.mUVScale.X * 0.25f;
			iEffect.TextureScale = textureScale;
			iEffect.CommitChanges();
			iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
			mTransform.M11 = iScale * (num - this.mSize.X * 0.5f);
			mTransform.M41 = this.mPosition.X + this.mMiddlePosition * iScale;
			iEffect.Transform = mTransform;
			textureOffset.X = this.mUVOffset.X + this.mUVScale.X * 0.25f;
			iEffect.TextureOffset = textureOffset;
			textureScale.X = this.mUVScale.X * 0.5f;
			iEffect.TextureScale = textureScale;
			iEffect.CommitChanges();
			iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
			mTransform.M11 = iScale * this.mSize.X * 0.25f;
			mTransform.M41 = this.mPosition.X + (this.mRightPosition - this.mSize.X * 0.25f * 0.5f) * iScale;
			iEffect.Transform = mTransform;
			textureOffset.X = this.mUVOffset.X + this.mUVScale.X * 0.75f;
			iEffect.TextureOffset = textureOffset;
			textureScale.X = this.mUVScale.X * 0.25f;
			iEffect.TextureScale = textureScale;
			iEffect.CommitChanges();
			iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
			iEffect.GraphicsDevice.Vertices[0].SetSource(null, 0, 0);
			color = (this.mEnabled ? (this.mSelected ? this.mColorSelected : this.mColor) : this.mColorDisabled);
			color.W *= iAlpha * iAlpha;
			iEffect.Color = color;
			mTransform.M11 = Math.Min((this.mMaxWidth - this.mSize.X * 0.5f) / this.mTextSize.X, 1f) * iScale;
			mTransform.M22 = iScale;
			mTransform.M41 = this.mPosition.X + this.mMiddlePosition * iScale;
			mTransform.M42 = this.mPosition.Y - this.mTextSize.Y * 0.5f * iScale;
			this.mTitle.Draw(iEffect, ref mTransform);
		}

		// Token: 0x060026F9 RID: 9977 RVA: 0x0011C634 File Offset: 0x0011A834
		public override void LanguageChanged()
		{
			if (this.mText == 0)
			{
				return;
			}
			string @string = LanguageManager.Instance.GetString(this.mText);
			this.mTitle.SetText(@string);
			this.mTextSize = this.mFont.MeasureText(@string, true);
			this.UpdateBoundingBox();
		}

		// Token: 0x04002A26 RID: 10790
		public static readonly Vector2 DEFAULT_UV_OFFSET = new Vector2(768f, 384f);

		// Token: 0x04002A27 RID: 10791
		public static readonly Vector2 DEFAULT_SIZE = new Vector2(128f, 64f);

		// Token: 0x04002A28 RID: 10792
		private static VertexBuffer sVertices;

		// Token: 0x04002A29 RID: 10793
		private static VertexDeclaration sDeclaration;

		// Token: 0x04002A2A RID: 10794
		private int mText;

		// Token: 0x04002A2B RID: 10795
		private Text mTitle;

		// Token: 0x04002A2C RID: 10796
		private BitmapFont mFont;

		// Token: 0x04002A2D RID: 10797
		private Texture2D mTexture;

		// Token: 0x04002A2E RID: 10798
		private Vector2 mSize;

		// Token: 0x04002A2F RID: 10799
		private float mMinWidth;

		// Token: 0x04002A30 RID: 10800
		private float mMaxWidth;

		// Token: 0x04002A31 RID: 10801
		private Vector2 mTextSize;

		// Token: 0x04002A32 RID: 10802
		private Vector2 mUVOffset;

		// Token: 0x04002A33 RID: 10803
		private Vector2 mUVScale;

		// Token: 0x04002A34 RID: 10804
		private TextAlign mAlignment;

		// Token: 0x04002A35 RID: 10805
		private float mLeftPosition;

		// Token: 0x04002A36 RID: 10806
		private float mMiddlePosition;

		// Token: 0x04002A37 RID: 10807
		private float mRightPosition;
	}
}
