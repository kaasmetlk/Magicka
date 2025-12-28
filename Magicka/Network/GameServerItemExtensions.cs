using System;
using Magicka.GameLogic.GameStates;
using SteamWrapper;

namespace Magicka.Network
{
	// Token: 0x02000597 RID: 1431
	internal static class GameServerItemExtensions
	{
		// Token: 0x06002AB0 RID: 10928 RVA: 0x0015134C File Offset: 0x0014F54C
		public static bool Playing(this GameServerItem self)
		{
			int num = self.GameTags.IndexOf('P');
			if (num >= 0)
			{
				char c = self.GameTags[num + 1];
				return c == '1';
			}
			return false;
		}

		// Token: 0x06002AB1 RID: 10929 RVA: 0x00151384 File Offset: 0x0014F584
		public static GameType GameType(this GameServerItem self)
		{
			int num = self.GameTags.IndexOf('T');
			if (num >= 0)
			{
				char c = self.GameTags[num + 1];
				return (GameType)(c - '0');
			}
			return (GameType)0;
		}
	}
}
