using System;
using Magicka.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;

namespace Magicka.GameLogic.GameStates.Menu
{
	// Token: 0x0200050F RID: 1295
	public class MenuImageTextItem : MenuItem
	{
		// Token: 0x060026D9 RID: 9945 RVA: 0x0011B6EC File Offset: 0x001198EC
		static MenuImageTextItem()
		{
			VertexPositionTexture[] array = new VertexPositionTexture[]
			{
				new VertexPositionTexture(new Vector3(0f, 0f, 0f), new Vector2(0f, 0f)),
				new VertexPositionTexture(new Vector3(1f, 0f, 0f), new Vector2(1f, 0f)),
				new VertexPositionTexture(new Vector3(1f, 1f, 0f), new Vector2(1f, 1f)),
				new VertexPositionTexture(new Vector3(0f, 1f, 0f), new Vector2(0f, 1f))
			};
			lock (Game.Instance.GraphicsDevice)
			{
				MenuImageTextItem.sVertices = new VertexBuffer(Game.Instance.GraphicsDevice, array.Length * VertexPositionTexture.SizeInBytes, BufferUsage.WriteOnly);
				MenuImageTextItem.sVertices.SetData<VertexPositionTexture>(array);
				MenuImageTextItem.sDeclaration = new VertexDeclaration(Game.Instance.GraphicsDevice, VertexPositionTexture.VertexElements);
			}
		}

		// Token: 0x17000920 RID: 2336
		// (get) Token: 0x060026DA RID: 9946 RVA: 0x0011B840 File Offset: 0x00119A40
		// (set) Token: 0x060026DB RID: 9947 RVA: 0x0011B848 File Offset: 0x00119A48
		public Vector2 TextPosition
		{
			get
			{
				return this.mTextPosition;
			}
			set
			{
				this.mTextPosition = value;
			}
		}

		// Token: 0x060026DC RID: 9948 RVA: 0x0011B854 File Offset: 0x00119A54
		public void SetTitle(int iText)
		{
			this.mText = iText;
			if (this.mText != 0)
			{
				this.mTitleString = LanguageManager.Instance.GetString(this.mText);
			}
			else
			{
				this.mTitleString = " ";
			}
			this.mTitle.SetText(this.mTitleString);
		}

		// Token: 0x060026DD RID: 9949 RVA: 0x0011B8A4 File Offset: 0x00119AA4
		public void SetTitle(string iText)
		{
			this.mTitleString = iText;
			this.mTitle.SetText(this.mTitleString);
		}

		// Token: 0x060026DE RID: 9950 RVA: 0x0011B8C0 File Offset: 0x00119AC0
		public MenuImageTextItem(Vector2 iPosition, Texture2D iTexture, Vector2 iTextureOffset, Vector2 iTextureScale, int iText, Vector2 iTextPosition, TextAlign iAlignment, BitmapFont iFont, Vector2 iSize)
		{
			this.mNormalSaturation = 1f;
			this.mSelectedSaturation = 1.3f;
			this.mDisabledSaturation = 0f;
			this.mTexture = iTexture;
			this.mPosition = iPosition;
			this.mTextPosition = iTextPosition;
			this.mTextureOffset = iTextureOffset;
			this.mTextureScale = iTextureScale;
			this.mLineHeight = (float)iFont.LineHeight;
			this.mHitBox.Width = (int)iSize.X;
			this.mHitBox.Height = (int)iSize.Y;
			this.mSize = iSize;
			this.mFont = iFont;
			this.mText = iText;
			this.mAlignment = iAlignment;
			this.mTitle = new Text(80, iFont, iAlignment, false);
			if (iText != 0)
			{
				this.mTitleString = LanguageManager.Instance.GetString(iText);
			}
			else
			{
				this.mTitleString = " ";
			}
			this.mTitle.SetText(this.mTitleString);
			this.UpdateBoundingBox();
			this.mTransform = Matrix.Identity;
			this.mTransform.M11 = this.mSize.X;
			this.mTransform.M22 = this.mSize.Y;
			this.mTransform.M41 = this.mPosition.X;
			this.mTransform.M42 = this.mPosition.Y;
		}

		// Token: 0x17000921 RID: 2337
		// (get) Token: 0x060026DF RID: 9951 RVA: 0x0011BA27 File Offset: 0x00119C27
		// (set) Token: 0x060026E0 RID: 9952 RVA: 0x0011BA2F File Offset: 0x00119C2F
		public float SelectedPower
		{
			get
			{
				return this.mSelectedPower;
			}
			set
			{
				this.mSelectedPower = value;
			}
		}

		// Token: 0x17000922 RID: 2338
		// (get) Token: 0x060026E1 RID: 9953 RVA: 0x0011BA38 File Offset: 0x00119C38
		public Text Text
		{
			get
			{
				return this.mTitle;
			}
		}

		// Token: 0x060026E2 RID: 9954 RVA: 0x0011BA40 File Offset: 0x00119C40
		public void SetText(string iText)
		{
			this.mText = 0;
			this.mTitleString = iText;
			this.mTitle.SetText(iText);
			this.UpdateBoundingBox();
		}

		// Token: 0x060026E3 RID: 9955 RVA: 0x0011BA64 File Offset: 0x00119C64
		protected override void UpdateBoundingBox()
		{
			this.mTopLeft.X = this.mPosition.X + (float)this.mHitBox.X * this.mScale;
			this.mTopLeft.Y = this.mPosition.Y + (float)this.mHitBox.Y * this.mScale;
			this.mBottomRight.X = this.mPosition.X + (float)(this.mHitBox.X + this.mHitBox.Width) * this.mScale;
			this.mBottomRight.Y = this.mPosition.Y + (float)(this.mHitBox.Y + this.mHitBox.Height) * this.mScale;
		}

		// Token: 0x060026E4 RID: 9956 RVA: 0x0011BB34 File Offset: 0x00119D34
		public void ResetHitArea()
		{
			this.mHitBox.X = 0;
			this.mHitBox.Y = 0;
			this.mHitBox.Width = (int)this.mSize.X;
			this.mHitBox.Height = (int)this.mSize.Y;
			this.UpdateBoundingBox();
		}

		// Token: 0x060026E5 RID: 9957 RVA: 0x0011BB8D File Offset: 0x00119D8D
		public void SetHitArea(int iX, int iY, int iWidth, int iHeight)
		{
			this.mHitBox.X = iX;
			this.mHitBox.Y = iY;
			this.mHitBox.Width = iWidth;
			this.mHitBox.Height = iHeight;
			this.UpdateBoundingBox();
		}

		// Token: 0x17000923 RID: 2339
		// (get) Token: 0x060026E6 RID: 9958 RVA: 0x0011BBC6 File Offset: 0x00119DC6
		public Vector2 Size
		{
			get
			{
				return this.mSize;
			}
		}

		// Token: 0x060026E7 RID: 9959 RVA: 0x0011BBCE File Offset: 0x00119DCE
		public override void Draw(GUIBasicEffect iEffect)
		{
			this.Draw(iEffect, this.mScale);
		}

		// Token: 0x060026E8 RID: 9960 RVA: 0x0011BBE0 File Offset: 0x00119DE0
		public override void Draw(GUIBasicEffect iEffect, float iScale)
		{
			iEffect.GraphicsDevice.Vertices[0].SetSource(MenuImageTextItem.sVertices, 0, VertexPositionTexture.SizeInBytes);
			iEffect.GraphicsDevice.VertexDeclaration = MenuImageTextItem.sDeclaration;
			iEffect.VertexColorEnabled = false;
			Vector4 color = Vector4.One;
			if (this.mSelected)
			{
				color = new Vector4(this.mSelectedPower, this.mSelectedPower, this.mSelectedPower, Math.Min(this.mSelectedPower, 1f));
			}
			color.W *= this.mAlpha;
			iEffect.Color = color;
			this.mTransform.M11 = this.mSize.X * iScale;
			this.mTransform.M22 = this.mSize.Y * iScale;
			iEffect.Texture = this.mTexture;
			iEffect.TextureEnabled = true;
			iEffect.TextureOffset = this.mTextureOffset;
			iEffect.TextureScale = this.mTextureScale;
			iEffect.Transform = this.mTransform;
			iEffect.Saturation = (this.mEnabled ? (this.mSelected ? this.mSelectedSaturation : this.mNormalSaturation) : this.mDisabledSaturation);
			iEffect.CommitChanges();
			iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
			iEffect.GraphicsDevice.Vertices[0].SetSource(null, 0, 0);
			color = (this.mEnabled ? (this.mSelected ? this.mColorSelected : this.mColor) : this.mColorDisabled);
			color.W *= this.mAlpha;
			iEffect.Color = color;
			iEffect.TextureOffset = Vector2.Zero;
			iEffect.TextureScale = Vector2.One;
			this.mTitle.Draw(iEffect, this.mPosition.X + this.mTextPosition.X + 4f, this.mPosition.Y + this.mTextPosition.Y - this.mLineHeight + 4f, iScale);
		}

		// Token: 0x060026E9 RID: 9961 RVA: 0x0011BDDD File Offset: 0x00119FDD
		public override void LanguageChanged()
		{
			if (this.mText == 0)
			{
				return;
			}
			this.mTitle.SetText(LanguageManager.Instance.GetString(this.mText));
			this.UpdateBoundingBox();
		}

		// Token: 0x04002A14 RID: 10772
		private int mText;

		// Token: 0x04002A15 RID: 10773
		private Text mTitle;

		// Token: 0x04002A16 RID: 10774
		private BitmapFont mFont;

		// Token: 0x04002A17 RID: 10775
		private string mTitleString;

		// Token: 0x04002A18 RID: 10776
		private Texture2D mTexture;

		// Token: 0x04002A19 RID: 10777
		private Vector2 mSize;

		// Token: 0x04002A1A RID: 10778
		private TextAlign mAlignment;

		// Token: 0x04002A1B RID: 10779
		private Vector2 mTextPosition;

		// Token: 0x04002A1C RID: 10780
		private Vector2 mTextureOffset;

		// Token: 0x04002A1D RID: 10781
		private Vector2 mTextureScale;

		// Token: 0x04002A1E RID: 10782
		private float mLineHeight;

		// Token: 0x04002A1F RID: 10783
		private Rectangle mHitBox;

		// Token: 0x04002A20 RID: 10784
		private float mSelectedPower = 1.5f;

		// Token: 0x04002A21 RID: 10785
		private static VertexBuffer sVertices;

		// Token: 0x04002A22 RID: 10786
		private static VertexDeclaration sDeclaration;

		// Token: 0x04002A23 RID: 10787
		private float mSelectedSaturation;

		// Token: 0x04002A24 RID: 10788
		private float mNormalSaturation;

		// Token: 0x04002A25 RID: 10789
		private float mDisabledSaturation;
	}
}
