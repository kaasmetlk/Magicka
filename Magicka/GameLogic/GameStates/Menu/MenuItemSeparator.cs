using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead.Effects;

namespace Magicka.GameLogic.GameStates.Menu
{
	// Token: 0x0200050D RID: 1293
	public class MenuItemSeparator : MenuItem
	{
		// Token: 0x060026BA RID: 9914 RVA: 0x0011AACA File Offset: 0x00118CCA
		public MenuItemSeparator(Vector2 iPosition)
		{
			this.mPosition = iPosition;
			this.EnsureInitialized();
		}

		// Token: 0x060026BB RID: 9915 RVA: 0x0011AAE0 File Offset: 0x00118CE0
		private void EnsureInitialized()
		{
			GraphicsDevice graphicsDevice = Game.Instance.GraphicsDevice;
			if (MenuItemSeparator.sTexture == null || MenuItemSeparator.sTexture.IsDisposed)
			{
				lock (graphicsDevice)
				{
					MenuItemSeparator.sTexture = Game.Instance.Content.Load<Texture2D>("UI/Menu/Pages");
				}
			}
			if (MenuItemSeparator.sVertices == null || MenuItemSeparator.sVertices.IsDisposed)
			{
				Vector4[] array = new Vector4[4];
				Vector2 vector = default(Vector2);
				vector.X = 1f / (float)MenuItemSeparator.sTexture.Width;
				vector.Y = 1f / (float)MenuItemSeparator.sTexture.Height;
				array[0].X = -304f;
				array[0].Y = -24f;
				array[0].Z = 448f * vector.X;
				array[0].W = 976f * vector.Y;
				array[1].X = 304f;
				array[1].Y = -24f;
				array[1].Z = 1056f * vector.X;
				array[1].W = 976f * vector.Y;
				array[2].X = 304f;
				array[2].Y = 24f;
				array[2].Z = 1056f * vector.X;
				array[2].W = 1024f * vector.Y;
				array[3].X = -304f;
				array[3].Y = 24f;
				array[3].Z = 448f * vector.X;
				array[3].W = 1024f * vector.Y;
				lock (graphicsDevice)
				{
					MenuItemSeparator.sVertices = new VertexBuffer(graphicsDevice, 16 * array.Length, BufferUsage.WriteOnly);
					MenuItemSeparator.sVertices.SetData<Vector4>(array);
				}
			}
			if (MenuItemSeparator.sVertexDeclaration == null || MenuItemSeparator.sVertexDeclaration.IsDisposed)
			{
				lock (graphicsDevice)
				{
					MenuItemSeparator.sVertexDeclaration = new VertexDeclaration(graphicsDevice, new VertexElement[]
					{
						new VertexElement(0, 0, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.Position, 0),
						new VertexElement(0, 8, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0)
					});
				}
			}
		}

		// Token: 0x17000919 RID: 2329
		// (get) Token: 0x060026BC RID: 9916 RVA: 0x0011ADA0 File Offset: 0x00118FA0
		// (set) Token: 0x060026BD RID: 9917 RVA: 0x0011ADA3 File Offset: 0x00118FA3
		public override bool Enabled
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		// Token: 0x060026BE RID: 9918 RVA: 0x0011ADA8 File Offset: 0x00118FA8
		protected override void UpdateBoundingBox()
		{
			this.mTopLeft.X = this.mPosition.X - 304f;
			this.mTopLeft.Y = this.mPosition.Y - 24f;
			this.mBottomRight.X = this.mPosition.X + 304f;
			this.mBottomRight.Y = this.mPosition.Y + 24f;
		}

		// Token: 0x060026BF RID: 9919 RVA: 0x0011AE25 File Offset: 0x00119025
		public override void Draw(GUIBasicEffect iEffect)
		{
			this.Draw(iEffect, this.mScale);
		}

		// Token: 0x060026C0 RID: 9920 RVA: 0x0011AE34 File Offset: 0x00119034
		public override void Draw(GUIBasicEffect iEffect, float iScale)
		{
			iEffect.Texture = MenuItemSeparator.sTexture;
			iEffect.TextureEnabled = true;
			Matrix transform = default(Matrix);
			transform.M11 = (transform.M22 = 1f);
			transform.M44 = 1f;
			transform.M41 = this.mPosition.X;
			transform.M42 = this.mPosition.Y;
			iEffect.Transform = transform;
			Vector4 color = default(Vector4);
			color.X = (color.Y = (color.Z = 1f));
			color.W = 0.75f;
			iEffect.Color = color;
			iEffect.GraphicsDevice.Vertices[0].SetSource(MenuItemSeparator.sVertices, 0, 16);
			iEffect.GraphicsDevice.VertexDeclaration = MenuItemSeparator.sVertexDeclaration;
			iEffect.CommitChanges();
			iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
		}

		// Token: 0x060026C1 RID: 9921 RVA: 0x0011AF27 File Offset: 0x00119127
		public override void LanguageChanged()
		{
		}

		// Token: 0x04002A09 RID: 10761
		private static Texture2D sTexture;

		// Token: 0x04002A0A RID: 10762
		private static VertexBuffer sVertices;

		// Token: 0x04002A0B RID: 10763
		private static VertexDeclaration sVertexDeclaration;
	}
}
