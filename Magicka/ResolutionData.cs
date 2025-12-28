using System;
using Microsoft.Xna.Framework.Graphics;

namespace Magicka
{
	// Token: 0x0200025D RID: 605
	internal struct ResolutionData : IComparable<ResolutionData>
	{
		// Token: 0x0600129D RID: 4765 RVA: 0x00073252 File Offset: 0x00071452
		public ResolutionData(DisplayMode iMode)
		{
			this.RefreshRate = iMode.RefreshRate;
			this.Width = iMode.Width;
			this.Height = iMode.Height;
		}

		// Token: 0x0600129E RID: 4766 RVA: 0x0007327C File Offset: 0x0007147C
		public int CompareTo(ResolutionData other)
		{
			if (other.RefreshRate != this.RefreshRate)
			{
				return this.RefreshRate - other.RefreshRate;
			}
			if (other.Width != this.Width)
			{
				return this.Width - other.Width;
			}
			return this.Height - other.Height;
		}

		// Token: 0x04001162 RID: 4450
		public int Width;

		// Token: 0x04001163 RID: 4451
		public int Height;

		// Token: 0x04001164 RID: 4452
		public int RefreshRate;
	}
}
