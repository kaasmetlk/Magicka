using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead.Effects;

namespace Magicka.GameLogic.GameStates.Menu
{
	// Token: 0x02000509 RID: 1289
	public class MenuImageItem : MenuItem
	{
		// Token: 0x06002678 RID: 9848 RVA: 0x00117FF4 File Offset: 0x001161F4
		public MenuImageItem(Vector2 iPosition, Texture2D iTexture, VertexBuffer iVertices, VertexDeclaration iDeclaration, int iStride, float iXSize, float iYSize) : this(iPosition, iTexture, iVertices, iDeclaration, 0f, 0, iStride, iXSize, iYSize)
		{
		}

		// Token: 0x06002679 RID: 9849 RVA: 0x00118018 File Offset: 0x00116218
		public MenuImageItem(Vector2 iPosition, Texture2D iTexture, VertexBuffer iVertices, VertexDeclaration iDeclaration, float iRotation, int iOffset, int iStride, float iXSize, float iYSize)
		{
			this.mTexture = iTexture;
			this.mPosition = iPosition;
			this.mVertexOffset = iOffset;
			this.mVertices = iVertices;
			this.mVertexStride = iStride;
			this.mVertexDeclaration = iDeclaration;
			this.mSize.X = iXSize;
			this.mSize.Y = iYSize;
			this.UpdateBoundingBox();
			this.mTransform = Matrix.CreateRotationZ(iRotation);
			this.mTransform.M41 = this.mPosition.X;
			this.mTransform.M42 = this.mPosition.Y;
		}

		// Token: 0x0600267A RID: 9850 RVA: 0x001180B4 File Offset: 0x001162B4
		protected override void UpdateBoundingBox()
		{
			this.mTopLeft.X = this.mPosition.X - this.mSize.X * 0.5f * this.mScale;
			this.mTopLeft.Y = this.mPosition.Y - this.mSize.Y * 0.5f * this.mScale;
			this.mBottomRight.X = this.mPosition.X + this.mSize.X * 0.5f * this.mScale;
			this.mBottomRight.Y = this.mPosition.Y + this.mSize.Y * 0.5f * this.mScale;
		}

		// Token: 0x0600267B RID: 9851 RVA: 0x0011817D File Offset: 0x0011637D
		public override void Draw(GUIBasicEffect iEffect)
		{
			this.Draw(iEffect, this.mScale);
		}

		// Token: 0x0600267C RID: 9852 RVA: 0x0011818C File Offset: 0x0011638C
		public override void Draw(GUIBasicEffect iEffect, float iScale)
		{
			if (!this.mEnabled)
			{
				return;
			}
			if (this.mSelected)
			{
				iEffect.Color = new Vector4(1.5f, 1.5f, 1.5f, 1f);
			}
			else
			{
				iEffect.Color = Vector4.One;
			}
			iEffect.Saturation = (this.mSelected ? 1.3f : 1f);
			iEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertices, 0, this.mVertexStride);
			iEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
			iEffect.GraphicsDevice.RenderState.CullMode = CullMode.None;
			Matrix mTransform = this.mTransform;
			mTransform.M11 *= iScale;
			mTransform.M12 *= iScale;
			mTransform.M13 *= iScale;
			mTransform.M21 *= iScale;
			mTransform.M22 *= iScale;
			mTransform.M23 *= iScale;
			iEffect.Transform = mTransform;
			iEffect.Texture = this.mTexture;
			iEffect.TextureEnabled = true;
			iEffect.VertexColorEnabled = false;
			iEffect.CommitChanges();
			iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, this.mVertexOffset, 2);
			iEffect.GraphicsDevice.Vertices[0].SetSource(null, 0, 0);
			iEffect.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
		}

		// Token: 0x0600267D RID: 9853 RVA: 0x001182FA File Offset: 0x001164FA
		public override void LanguageChanged()
		{
		}

		// Token: 0x040029D9 RID: 10713
		private Texture2D mTexture;

		// Token: 0x040029DA RID: 10714
		private int mVertexStride;

		// Token: 0x040029DB RID: 10715
		private VertexBuffer mVertices;

		// Token: 0x040029DC RID: 10716
		private VertexDeclaration mVertexDeclaration;

		// Token: 0x040029DD RID: 10717
		private Vector2 mSize;

		// Token: 0x040029DE RID: 10718
		private int mVertexOffset;
	}
}
