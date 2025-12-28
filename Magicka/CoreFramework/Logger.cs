using System;
using System.Collections.Generic;

namespace Magicka.CoreFramework
{
	// Token: 0x02000278 RID: 632
	public static class Logger
	{
		// Token: 0x060012AD RID: 4781 RVA: 0x0007495A File Offset: 0x00072B5A
		public static void LogVerbose(string iMessage)
		{
			Logger.LogVerbose(Logger.Source.Global, iMessage);
		}

		// Token: 0x060012AE RID: 4782 RVA: 0x00074963 File Offset: 0x00072B63
		public static void LogVerbose(string iMessage, bool iFileAndLine)
		{
			Logger.LogVerbose(Logger.Source.Global, iMessage, iFileAndLine);
		}

		// Token: 0x060012AF RID: 4783 RVA: 0x0007496D File Offset: 0x00072B6D
		public static void LogVerbose(Logger.Source iSource, string iMessage)
		{
			Logger.sWhiteList.Contains(iSource);
		}

		// Token: 0x060012B0 RID: 4784 RVA: 0x0007497C File Offset: 0x00072B7C
		public static void LogVerbose(Logger.Source iSource, string iMessage, bool iFileAndLine)
		{
			if (Logger.sWhiteList.Contains(iSource))
			{
				string iPrefix = iSource.ToString();
				if (iFileAndLine)
				{
					Logger.PrintFileAndLine(iPrefix);
				}
			}
		}

		// Token: 0x060012B1 RID: 4785 RVA: 0x000749AB File Offset: 0x00072BAB
		public static void LogDebug(string iMessage)
		{
		}

		// Token: 0x060012B2 RID: 4786 RVA: 0x000749AD File Offset: 0x00072BAD
		public static void LogDebug(string iMessage, bool iFileAndLine)
		{
		}

		// Token: 0x060012B3 RID: 4787 RVA: 0x000749AF File Offset: 0x00072BAF
		public static void LogDebug(Logger.Source iSource, string iMessage)
		{
		}

		// Token: 0x060012B4 RID: 4788 RVA: 0x000749B1 File Offset: 0x00072BB1
		public static void LogDebug(Logger.Source iSource, string iMessage, bool iFileAndLine)
		{
		}

		// Token: 0x060012B5 RID: 4789 RVA: 0x000749B3 File Offset: 0x00072BB3
		public static void LogWarning(string iMessage)
		{
		}

		// Token: 0x060012B6 RID: 4790 RVA: 0x000749B5 File Offset: 0x00072BB5
		public static void LogWarning(string iMessage, bool iFileAndLine)
		{
		}

		// Token: 0x060012B7 RID: 4791 RVA: 0x000749B7 File Offset: 0x00072BB7
		public static void LogWarning(Logger.Source iSource, string iMessage)
		{
		}

		// Token: 0x060012B8 RID: 4792 RVA: 0x000749B9 File Offset: 0x00072BB9
		public static void LogWarning(Logger.Source iSource, string iMessage, bool iFileAndLine)
		{
		}

		// Token: 0x060012B9 RID: 4793 RVA: 0x000749BB File Offset: 0x00072BBB
		public static void LogError(string iMessage)
		{
		}

		// Token: 0x060012BA RID: 4794 RVA: 0x000749BD File Offset: 0x00072BBD
		public static void LogError(string iMessage, bool iFileAndLine)
		{
		}

		// Token: 0x060012BB RID: 4795 RVA: 0x000749BF File Offset: 0x00072BBF
		public static void LogError(Logger.Source iSource, string iMessage)
		{
		}

		// Token: 0x060012BC RID: 4796 RVA: 0x000749C1 File Offset: 0x00072BC1
		public static void LogError(Logger.Source iSource, string iMessage, bool iFileAndLine)
		{
		}

		// Token: 0x060012BD RID: 4797 RVA: 0x000749C3 File Offset: 0x00072BC3
		public static void LogCritical(string iMessage)
		{
		}

		// Token: 0x060012BE RID: 4798 RVA: 0x000749C5 File Offset: 0x00072BC5
		public static void LogCritical(string iMessage, bool iFileAndLine)
		{
		}

		// Token: 0x060012BF RID: 4799 RVA: 0x000749C7 File Offset: 0x00072BC7
		public static void LogCritical(Logger.Source iSource, string iMessage)
		{
		}

		// Token: 0x060012C0 RID: 4800 RVA: 0x000749C9 File Offset: 0x00072BC9
		public static void LogCritical(Logger.Source iSource, string iMessage, bool iFileAndLine)
		{
		}

		// Token: 0x060012C1 RID: 4801 RVA: 0x000749CB File Offset: 0x00072BCB
		private static void PrintFileAndLine(string iPrefix)
		{
		}

		// Token: 0x04001481 RID: 5249
		private const string VERBOSE_PREFIXED_FORMAT = "[{0}] {1}";

		// Token: 0x04001482 RID: 5250
		private const string DEBUG_FORMAT = "DEBUG : {0}";

		// Token: 0x04001483 RID: 5251
		private const string DEBUG_PREFIXED_FORMAT = "[{0}] DEBUG : {1}";

		// Token: 0x04001484 RID: 5252
		private const string WARNING_FORMAT = "WARNING : {0}";

		// Token: 0x04001485 RID: 5253
		private const string WARNING_PREFIXED_FORMAT = "[{0}] WARNING : {1}";

		// Token: 0x04001486 RID: 5254
		private const string ERROR_FORMAT = "ERROR : {0}";

		// Token: 0x04001487 RID: 5255
		private const string ERROR_PREFIXED_FORMAT = "[{0}] ERROR : {1}";

		// Token: 0x04001488 RID: 5256
		private const string CRITICAL_FORMAT = "CRITICAL : {0}";

		// Token: 0x04001489 RID: 5257
		private const string CRITICAL_PREFIXED_FORMAT = "[{0}] CRITICAL : {1}";

		// Token: 0x0400148A RID: 5258
		private const string FILE_AND_LINE_FORMAT = "In File {0} ({1})";

		// Token: 0x0400148B RID: 5259
		private static readonly List<Logger.Source> sWhiteList = new List<Logger.Source>
		{
			Logger.Source.Global
		};

		// Token: 0x02000279 RID: 633
		public enum Source
		{
			// Token: 0x0400148D RID: 5261
			Global,
			// Token: 0x0400148E RID: 5262
			EventParameter,
			// Token: 0x0400148F RID: 5263
			GameSparks,
			// Token: 0x04001490 RID: 5264
			GameSparksAccount,
			// Token: 0x04001491 RID: 5265
			GameSparksServices,
			// Token: 0x04001492 RID: 5266
			GameSparksProperties,
			// Token: 0x04001493 RID: 5267
			HardwareInfoManager,
			// Token: 0x04001494 RID: 5268
			ParadoxAccount,
			// Token: 0x04001495 RID: 5269
			ParadoxAccountGameStartup,
			// Token: 0x04001496 RID: 5270
			ParadoxAccountPlayerCreate,
			// Token: 0x04001497 RID: 5271
			ParadoxAccountPlayerLogin,
			// Token: 0x04001498 RID: 5272
			ParadoxAccountPlayerLogout,
			// Token: 0x04001499 RID: 5273
			ParadoxAccountSteamLink,
			// Token: 0x0400149A RID: 5274
			ParadoxAccountSteamUnlink,
			// Token: 0x0400149B RID: 5275
			ParadoxAccountGameSparksAvailable,
			// Token: 0x0400149C RID: 5276
			ParadoxPopupUtils,
			// Token: 0x0400149D RID: 5277
			ParadoxAccountSaveData,
			// Token: 0x0400149E RID: 5278
			ParadoxAccountSequence,
			// Token: 0x0400149F RID: 5279
			ParadoxOPS,
			// Token: 0x040014A0 RID: 5280
			ParadoxServices,
			// Token: 0x040014A1 RID: 5281
			PlayerSegmentManager,
			// Token: 0x040014A2 RID: 5282
			TutorialUtils,
			// Token: 0x040014A3 RID: 5283
			Threads
		}
	}
}
