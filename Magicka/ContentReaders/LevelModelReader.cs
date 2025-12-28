using System;
using Magicka.Levels;
using Microsoft.Xna.Framework.Content;

namespace Magicka.ContentReaders
{
	// Token: 0x02000553 RID: 1363
	public class LevelModelReader : ContentTypeReader<LevelModel>
	{
		// Token: 0x06002883 RID: 10371 RVA: 0x0013DA47 File Offset: 0x0013BC47
		protected override LevelModel Read(ContentReader input, LevelModel existingInstance)
		{
			return new LevelModel(input);
		}
	}
}
