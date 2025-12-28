using System;
using Microsoft.Xna.Framework.Content;

namespace Magicka.GameLogic.Entities.Items
{
	// Token: 0x0200049C RID: 1180
	public struct RemoveEvent
	{
		// Token: 0x060023D1 RID: 9169 RVA: 0x001019F4 File Offset: 0x000FFBF4
		public RemoveEvent(int iNrOfBounces)
		{
			this.Bounce = iNrOfBounces;
		}

		// Token: 0x060023D2 RID: 9170 RVA: 0x001019FD File Offset: 0x000FFBFD
		public RemoveEvent(ContentReader iInput)
		{
			this.Bounce = iInput.ReadInt32();
		}

		// Token: 0x060023D3 RID: 9171 RVA: 0x00101A0B File Offset: 0x000FFC0B
		public void Execute(Entity iItem, Entity iTarget)
		{
			if (iItem is Character)
			{
				(iItem as Character).Terminate(true, false);
				return;
			}
			if (this.Bounce <= 0)
			{
				iItem.Kill();
				return;
			}
			this.Bounce--;
		}

		// Token: 0x040026E9 RID: 9961
		public int Bounce;
	}
}
