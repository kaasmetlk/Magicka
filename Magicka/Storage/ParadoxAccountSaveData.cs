using System;
using System.IO;
using System.Text;
using System.Web.Security;
using Magicka.CoreFramework;
using Magicka.Misc;
using SteamWrapper;

namespace Magicka.Storage
{
	// Token: 0x0200027E RID: 638
	public class ParadoxAccountSaveData : Singleton<ParadoxAccountSaveData>
	{
		// Token: 0x170004C4 RID: 1220
		// (get) Token: 0x060012D4 RID: 4820 RVA: 0x00074DCC File Offset: 0x00072FCC
		public string ShadowUniqueId
		{
			get
			{
				return this.mShadowUniqueId;
			}
		}

		// Token: 0x170004C5 RID: 1221
		// (get) Token: 0x060012D5 RID: 4821 RVA: 0x00074DD4 File Offset: 0x00072FD4
		public bool HasShadowUniqueId
		{
			get
			{
				return !string.IsNullOrEmpty(this.mShadowUniqueId);
			}
		}

		// Token: 0x170004C6 RID: 1222
		// (get) Token: 0x060012D6 RID: 4822 RVA: 0x00074DE4 File Offset: 0x00072FE4
		public string AuthToken
		{
			get
			{
				return this.mAuthToken;
			}
		}

		// Token: 0x170004C7 RID: 1223
		// (get) Token: 0x060012D7 RID: 4823 RVA: 0x00074DEC File Offset: 0x00072FEC
		public bool HasAuthToken
		{
			get
			{
				return !string.IsNullOrEmpty(this.mAuthToken);
			}
		}

		// Token: 0x170004C8 RID: 1224
		// (get) Token: 0x060012D8 RID: 4824 RVA: 0x00074DFC File Offset: 0x00072FFC
		public bool IsShadow
		{
			get
			{
				return this.HasShadowUniqueId && !this.HasAuthToken;
			}
		}

		// Token: 0x060012D9 RID: 4825 RVA: 0x00074E11 File Offset: 0x00073011
		public ParadoxAccountSaveData()
		{
			this.Load();
		}

		// Token: 0x060012DA RID: 4826 RVA: 0x00074E40 File Offset: 0x00073040
		public void SetShadowUniqueId(string iUniqueId)
		{
			this.mShadowUniqueId = iUniqueId;
			this.Save();
		}

		// Token: 0x060012DB RID: 4827 RVA: 0x00074E4F File Offset: 0x0007304F
		public void ClearShadowUniqueId()
		{
			this.mShadowUniqueId = string.Empty;
			this.Save();
		}

		// Token: 0x060012DC RID: 4828 RVA: 0x00074E62 File Offset: 0x00073062
		public void SetAuthToken(string iAuthToken)
		{
			this.mAuthToken = iAuthToken;
			this.Save();
		}

		// Token: 0x060012DD RID: 4829 RVA: 0x00074E71 File Offset: 0x00073071
		public void ClearAuthToken()
		{
			this.mAuthToken = string.Empty;
			this.Save();
		}

		// Token: 0x060012DE RID: 4830 RVA: 0x00074E84 File Offset: 0x00073084
		public void Promote(string iAuthToken)
		{
			this.mShadowUniqueId = string.Empty;
			this.SetAuthToken(iAuthToken);
		}

		// Token: 0x060012DF RID: 4831 RVA: 0x00074E98 File Offset: 0x00073098
		public void Write(BinaryWriter iWriter)
		{
			string[] value = new string[]
			{
				this.mShadowUniqueId,
				this.mAuthToken
			};
			string text = string.Join('|'.ToString(), value);
			string salt = this.GetSalt();
			string value2 = Convert.ToBase64String(Encoding.UTF8.GetBytes(FormsAuthentication.HashPasswordForStoringInConfigFile(text + salt, "MD5")));
			string value3 = Convert.ToBase64String(Encoding.UTF8.GetBytes(Encryption.Vigenere(text, this.GetSalt(), false)));
			iWriter.Write(value2);
			iWriter.Write(value3);
		}

		// Token: 0x060012E0 RID: 4832 RVA: 0x00074F2C File Offset: 0x0007312C
		public void Read(BinaryReader iReader)
		{
			string salt = this.GetSalt();
			string @string = Encoding.UTF8.GetString(Convert.FromBase64String(iReader.ReadString()));
			string text = Encryption.Vigenere(Encoding.UTF8.GetString(Convert.FromBase64String(iReader.ReadString())), salt, true);
			string strB = FormsAuthentication.HashPasswordForStoringInConfigFile(text + salt, "MD5");
			if (@string.CompareTo(strB) == 0)
			{
				string[] array = text.Split(new char[]
				{
					'|'
				});
				try
				{
					int num = 0;
					this.mShadowUniqueId = array[num++];
					this.mAuthToken = array[num++];
					return;
				}
				catch (Exception)
				{
					this.Reset();
					return;
				}
			}
			Logger.LogError(Logger.Source.ParadoxAccountSaveData, "Checksum fail.");
			this.Reset();
		}

		// Token: 0x060012E1 RID: 4833 RVA: 0x00074FF8 File Offset: 0x000731F8
		public string GetSalt()
		{
			return Convert.ToBase64String(Encoding.UTF8.GetBytes(SteamUser.GetSteamID().AsUInt64.ToString() + SteamUtils.GetAppID().ToString()));
		}

		// Token: 0x060012E2 RID: 4834 RVA: 0x0007503C File Offset: 0x0007323C
		public void Load()
		{
			lock (this.mSaveLoadLock)
			{
				SaveManager.Instance.LoadPOPSData(this);
			}
		}

		// Token: 0x060012E3 RID: 4835 RVA: 0x0007507C File Offset: 0x0007327C
		public void Save()
		{
			lock (this.mSaveLoadLock)
			{
				SaveManager.Instance.SavePOPSData(this);
			}
		}

		// Token: 0x060012E4 RID: 4836 RVA: 0x000750BC File Offset: 0x000732BC
		public void Reset()
		{
			this.mShadowUniqueId = string.Empty;
			this.mAuthToken = string.Empty;
			this.mLinkedToSteam = false;
			this.Save();
		}

		// Token: 0x040014AB RID: 5291
		private const Logger.Source LOGGER_SOURCE = Logger.Source.ParadoxAccountSaveData;

		// Token: 0x040014AC RID: 5292
		private const char DATA_SEPARATOR = '|';

		// Token: 0x040014AD RID: 5293
		private string mShadowUniqueId = string.Empty;

		// Token: 0x040014AE RID: 5294
		private string mAuthToken = string.Empty;

		// Token: 0x040014AF RID: 5295
		private bool mLinkedToSteam;

		// Token: 0x040014B0 RID: 5296
		private object mSaveLoadLock = new object();
	}
}
