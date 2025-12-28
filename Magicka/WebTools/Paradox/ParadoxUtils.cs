using System;
using System.Globalization;
using System.IO;
using System.Net.Mail;
using Magicka.Misc;

namespace Magicka.WebTools.Paradox
{
	// Token: 0x02000593 RID: 1427
	public static class ParadoxUtils
	{
		// Token: 0x06002A9D RID: 10909 RVA: 0x001504E1 File Offset: 0x0014E6E1
		public static void EnsureParadoxFolder()
		{
			if (!Directory.Exists(ParadoxSettings.PARADOX_CACHE_PATH))
			{
				Directory.CreateDirectory(ParadoxSettings.PARADOX_CACHE_PATH);
			}
		}

		// Token: 0x06002A9E RID: 10910 RVA: 0x001504FC File Offset: 0x0014E6FC
		public static bool IsValidEmail(string iEmail)
		{
			bool result;
			try
			{
				MailAddress mailAddress = new MailAddress(iEmail);
				result = (mailAddress.Address == iEmail);
			}
			catch
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06002A9F RID: 10911 RVA: 0x00150538 File Offset: 0x0014E738
		public static bool IsValidPassword(string iPassword)
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = iPassword.Length >= 5 && iPassword.Length <= 128;
			for (int i = 0; i < iPassword.Length; i++)
			{
				char c = iPassword[i];
				if (char.IsUpper(iPassword, i))
				{
					flag = true;
				}
				if (char.IsNumber(iPassword, i))
				{
					flag2 = true;
				}
			}
			return flag && flag2 && flag3;
		}

		// Token: 0x06002AA0 RID: 10912 RVA: 0x001505A0 File Offset: 0x0014E7A0
		public static bool IsValidDoB(string iDateOfBirth)
		{
			DateTime dateTime;
			return DateTime.TryParse(iDateOfBirth, new CultureInfo("en-US"), DateTimeStyles.AdjustToUniversal, out dateTime) && dateTime.Year >= 1900 && dateTime.Year <= DateTime.Now.Year - 5;
		}

		// Token: 0x06002AA1 RID: 10913 RVA: 0x001505F0 File Offset: 0x0014E7F0
		public static void AuthenticateWithGameSparks(GameSparksAccount.OperationCompleteDelegate iCallback)
		{
			string iGuid = Singleton<ParadoxServices>.Instance.RetrieveAccountGuid();
			string iAuthToken = Singleton<ParadoxServices>.Instance.RetrieveAuthToken();
			Singleton<GameSparksAccount>.Instance.Authenticate(iGuid, iAuthToken, iCallback);
		}

		// Token: 0x06002AA2 RID: 10914 RVA: 0x00150620 File Offset: 0x0014E820
		public static void RegisterWithGameSparks(GameSparksAccount.OperationCompleteDelegate iCallback)
		{
			string iGuid = Singleton<ParadoxServices>.Instance.RetrieveAccountGuid();
			string iAuthToken = Singleton<ParadoxServices>.Instance.RetrieveAuthToken();
			Singleton<GameSparksAccount>.Instance.Register(iGuid, iAuthToken, iCallback);
		}

		// Token: 0x04002DFA RID: 11770
		public const int EMAIL_MAX_LEN = 128;

		// Token: 0x04002DFB RID: 11771
		private const int PASSWORD_MIN_LEN = 5;

		// Token: 0x04002DFC RID: 11772
		public const int PASSWORD_MAX_LEN = 128;

		// Token: 0x04002DFD RID: 11773
		public const int DATEOFBIRTH_MAX_LEN = 10;

		// Token: 0x04002DFE RID: 11774
		private const int MINIMUM_AGE = 5;

		// Token: 0x04002DFF RID: 11775
		private const int MIN_YEAR = 1900;
	}
}
