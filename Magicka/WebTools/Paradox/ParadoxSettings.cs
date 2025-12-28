using System;
using System.IO;

namespace Magicka.WebTools.Paradox
{
	// Token: 0x02000621 RID: 1569
	public static class ParadoxSettings
	{
		// Token: 0x04003329 RID: 13097
		public const string ROOT_PATH = ".";

		// Token: 0x0400332A RID: 13098
		public const string PARADOX_CACHE_FOLDER_NAME = "cache";

		// Token: 0x0400332B RID: 13099
		public const string SHADOW_ACCOUNT_ID_TYPE = "generated";

		// Token: 0x0400332C RID: 13100
		public const string MAGICKA_NEWSLETTER = "red_wizard";

		// Token: 0x0400332D RID: 13101
		public const int MERGE_SHADOW_WAIT_DELAY = 2000;

		// Token: 0x0400332E RID: 13102
		public static readonly string PARADOX_CACHE_PATH = Path.Combine(".", "cache");
	}
}
