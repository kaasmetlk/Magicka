using System;
using Microsoft.Xna.Framework.Content;

namespace Magicka.GameLogic.Entities.Items
{
	// Token: 0x0200049D RID: 1181
	public struct OverKillEvent
	{
		// Token: 0x060023D4 RID: 9172 RVA: 0x00101A41 File Offset: 0x000FFC41
		public OverKillEvent(ContentReader iInput)
		{
			this.mDummy = 0;
		}

		// Token: 0x060023D5 RID: 9173 RVA: 0x00101A4A File Offset: 0x000FFC4A
		public void Execute(Entity iItem, Entity iTarget)
		{
			if (iItem is Character)
			{
				(iItem as Character).OverKill();
			}
		}

		// Token: 0x040026EA RID: 9962
		private byte mDummy;
	}
}
