using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;

namespace Magicka.GameLogic.GameStates.Menu
{
	// Token: 0x0200050C RID: 1292
	public class MenuItemList : MenuItem
	{
		// Token: 0x060026B4 RID: 9908 RVA: 0x0011A640 File Offset: 0x00118840
		static MenuItemList()
		{
			VertexPositionColor[] array = new VertexPositionColor[]
			{
				new VertexPositionColor(new Vector3(0f, 0f, 0f), Microsoft.Xna.Framework.Graphics.Color.Black),
				new VertexPositionColor(new Vector3(1f, 0f, 0f), Microsoft.Xna.Framework.Graphics.Color.Black),
				new VertexPositionColor(new Vector3(1f, 1f, 0f), Microsoft.Xna.Framework.Graphics.Color.Black),
				new VertexPositionColor(new Vector3(0f, 1f, 0f), Microsoft.Xna.Framework.Graphics.Color.Black)
			};
			lock (Game.Instance.GraphicsDevice)
			{
				MenuItemList.sVertexBuffer = new VertexBuffer(Game.Instance.GraphicsDevice, array.Length * VertexPositionColor.SizeInBytes, BufferUsage.WriteOnly);
				MenuItemList.sVertexBuffer.SetData<VertexPositionColor>(array);
				MenuItemList.sVertexDeclaration = new VertexDeclaration(Game.Instance.GraphicsDevice, VertexPositionColor.VertexElements);
			}
		}

		// Token: 0x060026B5 RID: 9909 RVA: 0x0011A76C File Offset: 0x0011896C
		public MenuItemList(Vector2 iPosition, BitmapFont iFont, Vector2 iSize)
		{
			this.mFont = iFont;
			this.mLineHeight = (float)iFont.LineHeight;
			this.mItems = new List<Text>(8);
			for (int i = 0; i < 6; i++)
			{
				this.mItems.Add(new Text(100, iFont, TextAlign.Left, false));
				if (i == 0)
				{
					this.mItems[i].SetText("Create new profile(noloc)");
				}
				else
				{
					this.mItems[i].SetText("Dummmy");
				}
				this.mItems[i].DefaultColor = new Vector4(1f, 1f, 1f, 1f);
			}
			this.mPosition = iPosition;
			this.mSize = iSize;
			this.mTransform = Matrix.Identity;
			this.mTransform.M41 = iPosition.X;
			this.mTransform.M42 = iPosition.Y;
			this.mTransform.M11 = this.mSize.X;
			this.mTransform.M22 = this.mSize.Y;
		}

		// Token: 0x060026B6 RID: 9910 RVA: 0x0011A884 File Offset: 0x00118A84
		protected override void UpdateBoundingBox()
		{
			this.mTopLeft.X = this.mPosition.X - this.mSize.X * 0.5f * this.mScale;
			this.mTopLeft.Y = this.mPosition.Y - this.mSize.Y * 0.5f * this.mScale;
			this.mBottomRight.X = this.mPosition.X + this.mSize.X * 0.5f * this.mScale;
			this.mBottomRight.Y = this.mPosition.Y + this.mSize.Y * 0.5f * this.mScale;
		}

		// Token: 0x060026B7 RID: 9911 RVA: 0x0011A94D File Offset: 0x00118B4D
		public override void LanguageChanged()
		{
		}

		// Token: 0x060026B8 RID: 9912 RVA: 0x0011A94F File Offset: 0x00118B4F
		public override void Draw(GUIBasicEffect iEffect)
		{
			this.Draw(iEffect, 1f);
		}

		// Token: 0x060026B9 RID: 9913 RVA: 0x0011A960 File Offset: 0x00118B60
		public override void Draw(GUIBasicEffect iEffect, float iScale)
		{
			if (!this.mEnabled)
			{
				return;
			}
			iEffect.GraphicsDevice.Vertices[0].SetSource(MenuItemList.sVertexBuffer, 0, VertexPositionColor.SizeInBytes);
			iEffect.GraphicsDevice.VertexDeclaration = MenuItemList.sVertexDeclaration;
			this.mTransform.M41 = this.mPosition.X;
			this.mTransform.M42 = this.mPosition.Y;
			this.mTransform.M11 = this.mSize.X;
			this.mTransform.M22 = this.mSize.Y;
			iEffect.Transform = this.mTransform;
			iEffect.Color = new Vector4(1f);
			iEffect.TextureEnabled = false;
			iEffect.OverlayTextureEnabled = false;
			iEffect.VertexColorEnabled = true;
			iEffect.Saturation = 1f;
			iEffect.CommitChanges();
			iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
			this.mTransform.M11 = iScale;
			this.mTransform.M22 = iScale;
			this.mTransform.M42 = this.mPosition.Y;
			iEffect.VertexColorEnabled = false;
			for (int i = 0; i < this.mItems.Count; i++)
			{
				this.mItems[i].Draw(iEffect, ref this.mTransform);
				this.mTransform.M42 = this.mTransform.M42 + this.mLineHeight;
			}
		}

		// Token: 0x04002A01 RID: 10753
		private const float MIN_WIDTH = 128f;

		// Token: 0x04002A02 RID: 10754
		private const float MIN_HEIGHT = 32f;

		// Token: 0x04002A03 RID: 10755
		private static VertexBuffer sVertexBuffer;

		// Token: 0x04002A04 RID: 10756
		private static VertexDeclaration sVertexDeclaration;

		// Token: 0x04002A05 RID: 10757
		private List<Text> mItems;

		// Token: 0x04002A06 RID: 10758
		private Vector2 mSize;

		// Token: 0x04002A07 RID: 10759
		private BitmapFont mFont;

		// Token: 0x04002A08 RID: 10760
		private float mLineHeight;
	}
}
