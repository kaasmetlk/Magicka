using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using Magicka.Achievements;
using Magicka.GameLogic;
using Magicka.GameLogic.Spells;
using Magicka.Levels.Campaign;
using Magicka.Levels.Packs;
using SteamWrapper;

namespace Magicka.DRM
{
	// Token: 0x02000398 RID: 920
	internal static class HackHelper
	{
		// Token: 0x06001C33 RID: 7219 RVA: 0x000C0600 File Offset: 0x000BE800
		static HackHelper()
		{
			HackHelper.sRobeAchievementDict = new Dictionary<int, string[]>(1);
			HackHelper.sRobeAchievementDict.Add("wizard_cultist".GetHashCodeCustom(), new string[]
			{
				"fhtagnoncemore"
			});
			HackHelper.sMagickAppIDs = new uint[35];
			for (int i = 0; i < HackHelper.sMagickAppIDs.Length; i++)
			{
				HackHelper.sMagickAppIDs[i] = SteamUtils.GetAppID();
			}
			HackHelper.sMagickAppIDs[12] = 73030U;
			HackHelper.sMagickAppIDs[24] = 42918U;
		}

		// Token: 0x06001C34 RID: 7220 RVA: 0x000C0690 File Offset: 0x000BE890
		public static uint GetAppIDForMagick(MagickType iMagick)
		{
			if (iMagick < MagickType.None || iMagick >= (MagickType)HackHelper.sMagickAppIDs.Length)
			{
				return SteamUtils.GetAppID();
			}
			return HackHelper.sMagickAppIDs[(int)iMagick];
		}

		// Token: 0x170006F7 RID: 1783
		// (get) Token: 0x06001C35 RID: 7221 RVA: 0x000C06AD File Offset: 0x000BE8AD
		public static HackHelper.Status LicenseStatus
		{
			get
			{
				return HackHelper.sStatus;
			}
		}

		// Token: 0x06001C36 RID: 7222 RVA: 0x000C06B4 File Offset: 0x000BE8B4
		public static HackHelper.License CheckLicense(string iFilename)
		{
			Stream stream = null;
			try
			{
				stream = File.OpenRead(iFilename);
				byte[] iHash = HackHelper.sHasher.ComputeHash(stream);
				stream.Close();
				uint appID;
				if (!HashTable.GetAppID(iHash, out appID))
				{
					return HackHelper.License.Custom;
				}
				if (!SteamApps.BIsSubscribedApp(appID))
				{
					return HackHelper.License.No;
				}
				return HackHelper.License.Yes;
			}
			catch
			{
				if (stream != null)
				{
					stream.Close();
				}
			}
			return HackHelper.License.No;
		}

		// Token: 0x06001C37 RID: 7223 RVA: 0x000C071C File Offset: 0x000BE91C
		public static HackHelper.License CheckLicense(LevelNode iLevel)
		{
			if (iLevel.HashSum == null)
			{
				return HackHelper.License.Pending;
			}
			HackHelper.License result = HackHelper.License.Yes;
			uint appID;
			if (HashTable.GetAppID(iLevel.HashSum, out appID))
			{
				if (!SteamApps.BIsSubscribedApp(appID))
				{
					return HackHelper.License.No;
				}
			}
			else
			{
				result = HackHelper.License.Custom;
			}
			for (int i = 0; i < iLevel.Scenes.Length; i++)
			{
				if (HashTable.GetAppID(iLevel.Scenes[i].ScriptHashSum, out appID))
				{
					if (!SteamApps.BIsSubscribedApp(appID))
					{
						return HackHelper.License.No;
					}
				}
				else
				{
					result = HackHelper.License.Custom;
				}
				if (HashTable.GetAppID(iLevel.Scenes[i].ModelHashSum, out appID))
				{
					if (!SteamApps.BIsSubscribedApp(appID))
					{
						return HackHelper.License.No;
					}
				}
				else
				{
					result = HackHelper.License.Custom;
				}
			}
			return result;
		}

		// Token: 0x06001C38 RID: 7224 RVA: 0x000C07A8 File Offset: 0x000BE9A8
		public static HackHelper.License CheckLicense(Profile.PlayableAvatar iAvatar)
		{
			if (iAvatar.HashSum == null)
			{
				return HackHelper.License.Yes;
			}
			HackHelper.License result = HackHelper.License.Yes;
			uint appID;
			if (HashTable.GetAppID(iAvatar.HashSum, out appID))
			{
				if (!SteamApps.BIsSubscribedApp(appID))
				{
					return HackHelper.License.No;
				}
			}
			else
			{
				result = HackHelper.License.Custom;
			}
			HackHelper.License license = HackHelper.CheckLicense("content/data/characters/" + iAvatar.TypeName + ".xnb");
			if (license == HackHelper.License.Custom)
			{
				result = HackHelper.License.Custom;
			}
			else if (license == HackHelper.License.No)
			{
				return HackHelper.License.No;
			}
			if (HackHelper.sRobeAchievementDict.ContainsKey(iAvatar.Type))
			{
				string[] array = HackHelper.sRobeAchievementDict[iAvatar.Type];
				for (int i = 0; i < array.Length; i++)
				{
					if (!AchievementsManager.Instance.HasAchievement(array[i]))
					{
						return HackHelper.License.No;
					}
				}
				return HackHelper.License.Yes;
			}
			return result;
		}

		// Token: 0x06001C39 RID: 7225 RVA: 0x000C0854 File Offset: 0x000BEA54
		internal static HackHelper.License CheckLicense(ItemPack iPack)
		{
			SHA256 sha = SHA256.Create();
			HackHelper.License result = HackHelper.License.Yes;
			for (int i = 0; i < iPack.Items.Length; i++)
			{
				FileStream fileStream = null;
				try
				{
					fileStream = File.OpenRead(Path.Combine("content", iPack.Items[i] + ".xnb"));
					byte[] iHash = sha.ComputeHash(fileStream);
					fileStream.Close();
					uint appID;
					if (!HashTable.GetAppID(iHash, out appID))
					{
						result = HackHelper.License.Custom;
					}
					else if (!SteamApps.BIsSubscribedApp(appID))
					{
						return HackHelper.License.No;
					}
				}
				catch
				{
					if (fileStream != null)
					{
						fileStream.Close();
					}
					return HackHelper.License.No;
				}
			}
			return result;
		}

		// Token: 0x06001C3A RID: 7226 RVA: 0x000C08F4 File Offset: 0x000BEAF4
		internal static HackHelper.License CheckLicense(MagickPack iPack)
		{
			for (int i = 0; i < iPack.Magicks.Length; i++)
			{
				uint appIDForMagick = HackHelper.GetAppIDForMagick(iPack.Magicks[i]);
				if (!SteamApps.BIsSubscribedApp(appIDForMagick))
				{
					return HackHelper.License.No;
				}
			}
			return HackHelper.License.Yes;
		}

		// Token: 0x06001C3B RID: 7227 RVA: 0x000C0930 File Offset: 0x000BEB30
		public static void BeginCoreCheck()
		{
			new Thread(new ThreadStart(HackHelper.CoreCheck))
			{
				Name = "Hash Checker"
			}.Start();
		}

		// Token: 0x06001C3C RID: 7228 RVA: 0x000C0960 File Offset: 0x000BEB60
		private static void CoreCheck()
		{
			HackHelper.sStatus = HackHelper.Status.Pending;
			for (int i = 0; i < HashTable.CoreFiles.Length; i++)
			{
				Stream stream = null;
				try
				{
					string path = HashTable.CoreFiles[i];
					stream = File.OpenRead(path);
					byte[] iHash = HackHelper.sHasher.ComputeHash(stream);
					stream.Close();
					string value;
					if (!HashTable.GetName(iHash, out value) || !Path.GetFileName(HashTable.CoreFiles[i]).Equals(value, StringComparison.OrdinalIgnoreCase))
					{
						HackHelper.sStatus = HackHelper.Status.Hacked;
						return;
					}
				}
				catch (Exception)
				{
					if (stream != null)
					{
						stream.Close();
					}
					HackHelper.sStatus = HackHelper.Status.Hacked;
					return;
				}
			}
			HackHelper.sStatus = HackHelper.Status.Valid;
		}

		// Token: 0x06001C3D RID: 7229 RVA: 0x000C09FC File Offset: 0x000BEBFC
		internal static bool CheckLicenseMythos()
		{
			return SteamApps.BIsSubscribedApp(73058U);
		}

		// Token: 0x06001C3E RID: 7230 RVA: 0x000C0A08 File Offset: 0x000BEC08
		internal static bool CheckLicenseVietnam()
		{
			return SteamApps.BIsSubscribedApp(42918U);
		}

		// Token: 0x06001C3F RID: 7231 RVA: 0x000C0A14 File Offset: 0x000BEC14
		internal static bool CheckLicenseOSOTC()
		{
			return SteamApps.BIsSubscribedApp(73093U);
		}

		// Token: 0x06001C40 RID: 7232 RVA: 0x000C0A20 File Offset: 0x000BEC20
		internal static bool CheckLicenseDungeons1()
		{
			return SteamApps.BIsSubscribedApp(73115U);
		}

		// Token: 0x06001C41 RID: 7233 RVA: 0x000C0A2C File Offset: 0x000BEC2C
		internal static bool CheckLicenseDungeons2()
		{
			return SteamApps.BIsSubscribedApp(255980U);
		}

		// Token: 0x06001C42 RID: 7234 RVA: 0x000C0A38 File Offset: 0x000BEC38
		internal static bool CheckLicenseDungeons3()
		{
			return false;
		}

		// Token: 0x04001E6B RID: 7787
		private static Dictionary<int, string[]> sRobeAchievementDict;

		// Token: 0x04001E6C RID: 7788
		private static uint[] sMagickAppIDs;

		// Token: 0x04001E6D RID: 7789
		private static SHA256 sHasher = SHA256.Create();

		// Token: 0x04001E6E RID: 7790
		private static HackHelper.Status sStatus = HackHelper.Status.Pending;

		// Token: 0x02000399 RID: 921
		public enum Status
		{
			// Token: 0x04001E70 RID: 7792
			Pending,
			// Token: 0x04001E71 RID: 7793
			Valid,
			// Token: 0x04001E72 RID: 7794
			Hacked
		}

		// Token: 0x0200039A RID: 922
		public enum License
		{
			// Token: 0x04001E74 RID: 7796
			Pending = -1,
			// Token: 0x04001E75 RID: 7797
			No,
			// Token: 0x04001E76 RID: 7798
			Yes,
			// Token: 0x04001E77 RID: 7799
			Custom
		}
	}
}
