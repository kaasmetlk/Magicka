using System;
using Microsoft.Xna.Framework.Input;

namespace Magicka.GameLogic.Controls
{
	// Token: 0x02000188 RID: 392
	internal struct KeyData
	{
		// Token: 0x06000BED RID: 3053 RVA: 0x00047F95 File Offset: 0x00046195
		public KeyData(Keys iKey, KeyModifiers iModifier)
		{
			this.Key = iKey;
			this.Modifier = iModifier;
		}

		// Token: 0x04000AF4 RID: 2804
		public Keys Key;

		// Token: 0x04000AF5 RID: 2805
		public KeyModifiers Modifier;
	}
}
