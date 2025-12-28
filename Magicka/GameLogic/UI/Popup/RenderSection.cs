using System;
using Microsoft.Xna.Framework;

namespace Magicka.GameLogic.UI.Popup
{
	// Token: 0x020005DD RID: 1501
	internal struct RenderSection
	{
		// Token: 0x06002CD0 RID: 11472 RVA: 0x0015F726 File Offset: 0x0015D926
		public RenderSection(Vector2 iPosition, Vector2 iSize, Vector2 iTextureOffset, Vector2 iTextureSize)
		{
			this.Position = iPosition;
			this.Size = iSize;
			this.TextureOffset = iTextureOffset;
			this.TextureSize = iTextureSize;
		}

		// Token: 0x04003083 RID: 12419
		public readonly Vector2 Position;

		// Token: 0x04003084 RID: 12420
		public readonly Vector2 Size;

		// Token: 0x04003085 RID: 12421
		public readonly Vector2 TextureOffset;

		// Token: 0x04003086 RID: 12422
		public readonly Vector2 TextureSize;
	}
}
