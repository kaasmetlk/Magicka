using System;
using Magicka.GameLogic.Entities.Items;
using Microsoft.Xna.Framework.Content;

namespace Magicka.ContentReaders
{
	// Token: 0x020005A7 RID: 1447
	public class ItemReader : ContentTypeReader<Item>
	{
		// Token: 0x06002B4B RID: 11083 RVA: 0x00155F24 File Offset: 0x00154124
		protected override Item Read(ContentReader iInput, Item existingInstance)
		{
			return Item.Read(iInput);
		}
	}
}
