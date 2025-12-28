using System;
using Microsoft.Xna.Framework;

namespace Magicka.GameLogic.UI.Popup
{
	// Token: 0x020005DC RID: 1500
	internal struct RenderModeSettings
	{
		// Token: 0x06002CCF RID: 11471 RVA: 0x0015F716 File Offset: 0x0015D916
		public RenderModeSettings(Vector4 iInsets, Vector4 iProportions)
		{
			this.Insets = iInsets;
			this.Proportions = iProportions;
		}

		// Token: 0x04003081 RID: 12417
		public readonly Vector4 Insets;

		// Token: 0x04003082 RID: 12418
		public readonly Vector4 Proportions;
	}
}
