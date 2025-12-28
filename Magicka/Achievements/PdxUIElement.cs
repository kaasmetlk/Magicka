using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead.Effects;

namespace Magicka.Achievements
{
	// Token: 0x02000135 RID: 309
	internal abstract class PdxUIElement
	{
		// Token: 0x060008C0 RID: 2240 RVA: 0x00037FA0 File Offset: 0x000361A0
		public PdxUIElement()
		{
			if (PdxUIElement.sVertexDeclaration == null)
			{
				GraphicsDevice graphicsDevice = Game.Instance.GraphicsDevice;
				lock (graphicsDevice)
				{
					PdxUIElement.sVertexDeclaration = new VertexDeclaration(graphicsDevice, new VertexElement[]
					{
						new VertexElement(0, 0, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.Position, 0),
						new VertexElement(0, 8, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0)
					});
				}
			}
		}

		// Token: 0x060008C1 RID: 2241
		public abstract void Draw(GUIBasicEffect iEffect, float iAlpha);

		// Token: 0x060008C2 RID: 2242 RVA: 0x00038028 File Offset: 0x00036228
		public virtual void OnLanguageChanged()
		{
		}

		// Token: 0x060008C3 RID: 2243 RVA: 0x0003802C File Offset: 0x0003622C
		public bool InsideBounds(ref Vector2 iPoint)
		{
			return iPoint.X >= this.mPosition.X & iPoint.Y >= this.mPosition.Y & iPoint.X <= this.mPosition.X + this.mSize.X & iPoint.Y <= this.mPosition.Y + this.mSize.Y;
		}

		// Token: 0x170001BD RID: 445
		// (get) Token: 0x060008C4 RID: 2244 RVA: 0x000380AC File Offset: 0x000362AC
		// (set) Token: 0x060008C5 RID: 2245 RVA: 0x000380B4 File Offset: 0x000362B4
		public Vector2 Position
		{
			get
			{
				return this.mPosition;
			}
			set
			{
				this.mPosition = value;
			}
		}

		// Token: 0x0400081F RID: 2079
		protected Vector2 mPosition;

		// Token: 0x04000820 RID: 2080
		protected Vector2 mSize;

		// Token: 0x04000821 RID: 2081
		protected static VertexDeclaration sVertexDeclaration;
	}
}
