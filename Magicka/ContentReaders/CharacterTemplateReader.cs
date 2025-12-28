using System;
using Magicka.GameLogic.Entities;
using Microsoft.Xna.Framework.Content;

namespace Magicka.ContentReaders
{
	// Token: 0x020005A9 RID: 1449
	public class CharacterTemplateReader : ContentTypeReader<CharacterTemplate>
	{
		// Token: 0x06002B5C RID: 11100 RVA: 0x0015671C File Offset: 0x0015491C
		protected override CharacterTemplate Read(ContentReader input, CharacterTemplate existingInstance)
		{
			return CharacterTemplate.Read(input);
		}
	}
}
