using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Magicka.GameLogic;
using Magicka.Graphics;
using Microsoft.Xna.Framework;

namespace Magicka.Localization
{
	// Token: 0x02000377 RID: 887
	public class LanguageManager
	{
		// Token: 0x170006A2 RID: 1698
		// (get) Token: 0x06001B0F RID: 6927 RVA: 0x000B8D2C File Offset: 0x000B6F2C
		public static LanguageManager Instance
		{
			get
			{
				if (LanguageManager.sSingelton == null)
				{
					lock (LanguageManager.sSingeltonLock)
					{
						if (LanguageManager.sSingelton == null)
						{
							LanguageManager.sSingelton = new LanguageManager();
						}
					}
				}
				return LanguageManager.sSingelton;
			}
		}

		// Token: 0x14000010 RID: 16
		// (add) Token: 0x06001B10 RID: 6928 RVA: 0x000B8D80 File Offset: 0x000B6F80
		// (remove) Token: 0x06001B11 RID: 6929 RVA: 0x000B8D99 File Offset: 0x000B6F99
		public event Action LanguageChanged;

		// Token: 0x06001B12 RID: 6930 RVA: 0x000B8DB4 File Offset: 0x000B6FB4
		private LanguageManager()
		{
			this.mStrings = new StringCollection();
			DirectoryInfo[] directories = new DirectoryInfo("content/Languages/").GetDirectories();
			List<Language> list = new List<Language>();
			for (int i = 0; i < directories.Length; i++)
			{
				try
				{
					Language item = (Language)Enum.Parse(typeof(Language), directories[i].Name, true);
					list.Add(item);
				}
				catch
				{
				}
			}
			this.mAllLanguages = list.ToArray();
		}

		// Token: 0x06001B13 RID: 6931 RVA: 0x000B8E44 File Offset: 0x000B7044
		public string ISO6391()
		{
			return this.ISO6391(this.CurrentLanguage);
		}

		// Token: 0x06001B14 RID: 6932 RVA: 0x000B8E54 File Offset: 0x000B7054
		public string ISO6391(Language lang)
		{
			switch (lang)
			{
			case Language.ara:
				return "ar";
			case Language.bul:
				return "bg";
			case Language.cat:
				return "ca";
			case Language.zho:
				return "zh";
			case Language.ces:
				return "cs";
			case Language.dan:
				return "da";
			case Language.deu:
				return "de";
			case Language.ell:
				return "el";
			case Language.eng:
				return "en";
			case Language.spa:
				return "es";
			case Language.fin:
				return "fi";
			case Language.fra:
				return "fr";
			case Language.heb:
				return "he";
			case Language.hun:
				return "hu";
			case Language.isl:
				return "is";
			case Language.ita:
				return "it";
			case Language.jpn:
				return "ja";
			case Language.kor:
				return "ko";
			case Language.nld:
				return "nl";
			case Language.nor:
				return "no";
			case Language.pol:
				return "pl";
			case Language.por:
				return "pt";
			case (Language)23:
			case (Language)40:
			case (Language)46:
			case (Language)48:
			case (Language)49:
			case (Language)50:
			case (Language)51:
			case (Language)52:
			case (Language)53:
			case (Language)58:
			case (Language)59:
			case (Language)60:
			case (Language)61:
			case (Language)66:
			case (Language)69:
			case (Language)72:
			case (Language)76:
			case (Language)77:
			case (Language)81:
			case (Language)82:
			case (Language)83:
			case (Language)84:
			case (Language)85:
			case (Language)88:
			case (Language)89:
			case (Language)91:
			case (Language)92:
			case (Language)93:
			case (Language)94:
			case (Language)95:
			case (Language)96:
			case (Language)97:
			case (Language)98:
			case (Language)99:
			case (Language)100:
				break;
			case Language.ron:
				return "ro";
			case Language.rus:
				return "ru";
			case Language.hrv:
				return "hr";
			case Language.slk:
				return "sk";
			case Language.sqi:
				return "sq";
			case Language.swe:
				return "sv";
			case Language.tha:
				return "th";
			case Language.tur:
				return "tr";
			case Language.urd:
				return "ur";
			case Language.ind:
				return "id";
			case Language.ukr:
				return "uk";
			case Language.bel:
				return "be";
			case Language.slv:
				return "sl";
			case Language.est:
				return "et";
			case Language.lav:
				return "lv";
			case Language.lit:
				return "lt";
			case Language.fas:
				return "fa";
			case Language.vie:
				return "vi";
			case Language.hye:
				return "hy";
			case Language.aze:
				return "az";
			case Language.eus:
				return "eu";
			case Language.mkd:
				return "mk";
			case Language.afr:
				return "af";
			case Language.kat:
				return "ka";
			case Language.fao:
				return "fo";
			case Language.hin:
				return "hi";
			case Language.msa:
				return "ms";
			case Language.kaz:
				return "kk";
			case Language.kir:
				return "ky";
			case Language.swa:
				return "sw";
			case Language.uzb:
				return "uz";
			case Language.tat:
				return "tt";
			case Language.pan:
				return "pa";
			case Language.guj:
				return "gu";
			case Language.tam:
				return "ta";
			case Language.tel:
				return "te";
			case Language.kan:
				return "kn";
			case Language.mar:
				return "mr";
			case Language.san:
				return "sa";
			case Language.mon:
				return "mn";
			case Language.glg:
				return "gl";
			case Language.kok:
				return "n/a";
			case Language.syr:
				return "n/a";
			case Language.div:
				return "dv";
			default:
				if (lang == Language.srp)
				{
					return "sr";
				}
				break;
			}
			return "n/a";
		}

		// Token: 0x06001B15 RID: 6933 RVA: 0x000B91A8 File Offset: 0x000B73A8
		public void SetPlayerStrings()
		{
			List<string> list = new List<string>();
			Player[] players = Game.Instance.Players;
			for (int i = 0; i < players.Length; i++)
			{
				if (players[i].Playing)
				{
					list.Add(players[i].GamerTag);
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				if (j == 0)
				{
					this.mPlayersString = list[j];
				}
				else if (j == list.Count - 1)
				{
					this.mPlayersString = this.mPlayersString + " & " + list[j];
				}
				else
				{
					this.mPlayersString = this.mPlayersString + ", " + list[j];
				}
			}
		}

		// Token: 0x06001B16 RID: 6934 RVA: 0x000B9258 File Offset: 0x000B7458
		public string GetString(int iID)
		{
			if (iID == LanguageManager.PLAYERSHASH)
			{
				return this.mPlayersString;
			}
			string result;
			if (this.mStrings.TryGetValue(iID, out result))
			{
				return result;
			}
			return "NOT FOUND: " + iID;
		}

		// Token: 0x06001B17 RID: 6935 RVA: 0x000B9298 File Offset: 0x000B7498
		internal string GetStringWithReferencs(int iID)
		{
			if (iID == LanguageManager.PLAYERSHASH)
			{
				return this.mPlayersString;
			}
			string text;
			if (this.mStrings.TryGetValue(iID, out text))
			{
				for (int i = text.IndexOf('#'); i >= 0; i = text.IndexOf('#'))
				{
					int num = text.IndexOf(';', i + 1);
					if (num <= i)
					{
						break;
					}
					string text2 = text.Substring(i, num - i);
					int hashCodeCustom = text2.ToLowerInvariant().GetHashCodeCustom();
					text = text.Substring(0, i) + this.GetStringWithReferencs(hashCodeCustom) + text.Substring(num + 1);
				}
				return text;
			}
			return "NOT FOUND: " + iID;
		}

		// Token: 0x06001B18 RID: 6936 RVA: 0x000B9338 File Offset: 0x000B7538
		internal string GetStringWithReferencs(int iID, ref Vector3 iReferenceColor)
		{
			if (iID == LanguageManager.PLAYERSHASH)
			{
				return this.mPlayersString;
			}
			string text;
			if (this.mStrings.TryGetValue(iID, out text))
			{
				for (int i = text.IndexOf('#'); i >= 0; i = text.IndexOf('#'))
				{
					int num = text.IndexOf(';', i + 1);
					if (num <= i)
					{
						break;
					}
					string text2 = text.Substring(i, num - i);
					int hashCodeCustom = text2.ToLowerInvariant().GetHashCodeCustom();
					text = string.Concat(new string[]
					{
						text.Substring(0, i),
						LanguageManager.ColorCode(ref iReferenceColor),
						this.GetStringWithReferencs(hashCodeCustom, ref iReferenceColor),
						LanguageManager.ColorCodeEnd(),
						text.Substring(num + 1)
					});
				}
				return text;
			}
			return "NOT FOUND: " + iID;
		}

		// Token: 0x06001B19 RID: 6937 RVA: 0x000B9404 File Offset: 0x000B7604
		internal string GetStringWithReferencs(int iID, ref Vector4 iReferenceColor)
		{
			if (iID == LanguageManager.PLAYERSHASH)
			{
				return this.mPlayersString;
			}
			string text;
			if (this.mStrings.TryGetValue(iID, out text))
			{
				for (int i = text.IndexOf('#'); i >= 0; i = text.IndexOf('#'))
				{
					int num = text.IndexOf(';', i + 1);
					if (num <= i)
					{
						break;
					}
					string text2 = text.Substring(i, num - i);
					int hashCodeCustom = text2.ToLowerInvariant().GetHashCodeCustom();
					text = string.Concat(new string[]
					{
						text.Substring(0, i),
						LanguageManager.ColorCode(ref iReferenceColor),
						this.GetStringWithReferencs(hashCodeCustom, ref iReferenceColor),
						LanguageManager.ColorCodeEnd(),
						text.Substring(num + 1)
					});
				}
				return text;
			}
			return "NOT FOUND: " + iID;
		}

		// Token: 0x06001B1A RID: 6938 RVA: 0x000B94D0 File Offset: 0x000B76D0
		public static string ColorCode(ref Vector3 iColor)
		{
			return string.Concat(new string[]
			{
				"[c=",
				iColor.X.ToString(CultureInfo.InvariantCulture.NumberFormat),
				",",
				iColor.Y.ToString(CultureInfo.InvariantCulture.NumberFormat),
				",",
				iColor.Z.ToString(CultureInfo.InvariantCulture.NumberFormat),
				"]"
			});
		}

		// Token: 0x06001B1B RID: 6939 RVA: 0x000B9554 File Offset: 0x000B7754
		public static string ColorCode(ref Vector4 iColor)
		{
			return string.Concat(new string[]
			{
				"[c=",
				iColor.X.ToString(CultureInfo.InvariantCulture.NumberFormat),
				",",
				iColor.Y.ToString(CultureInfo.InvariantCulture.NumberFormat),
				",",
				iColor.Z.ToString(CultureInfo.InvariantCulture.NumberFormat),
				",",
				iColor.W.ToString(CultureInfo.InvariantCulture.NumberFormat),
				"]"
			});
		}

		// Token: 0x06001B1C RID: 6940 RVA: 0x000B95F7 File Offset: 0x000B77F7
		public static string ColorCodeEnd()
		{
			return "[/c]";
		}

		// Token: 0x06001B1D RID: 6941 RVA: 0x000B95FE File Offset: 0x000B77FE
		public bool TryGetString(int iID, out string oString)
		{
			if (iID == LanguageManager.PLAYERSHASH)
			{
				oString = this.mPlayersString;
				return true;
			}
			return this.mStrings.TryGetValue(iID, out oString);
		}

		// Token: 0x06001B1E RID: 6942 RVA: 0x000B9620 File Offset: 0x000B7820
		public string ParseReferences(string iText)
		{
			for (int i = iText.IndexOf('#'); i >= 0; i = iText.IndexOf('#'))
			{
				int num = iText.IndexOf(';', i + 1);
				if (num <= i)
				{
					break;
				}
				string text = iText.Substring(i, num - i);
				int hashCodeCustom = text.ToLowerInvariant().GetHashCodeCustom();
				iText = string.Concat(new string[]
				{
					iText.Substring(0, i),
					"[c=1,1,1]",
					this.GetString(hashCodeCustom),
					"[/c]",
					iText.Substring(num + 1)
				});
			}
			return iText;
		}

		// Token: 0x06001B1F RID: 6943 RVA: 0x000B96B4 File Offset: 0x000B78B4
		public void SetLanguage(Language iLanguage)
		{
			bool flag = false;
			for (int i = 0; i < this.mAllLanguages.Length; i++)
			{
				if (this.mAllLanguages[i] == iLanguage)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				iLanguage = Language.eng;
			}
			if (iLanguage != this.mCurrentLanguage)
			{
				this.mStrings.Clear();
				this.mCurrentLanguage = iLanguage;
				FileInfo[] files = new DirectoryInfo("content/Languages/" + this.mCurrentLanguage).GetFiles("*.loctable.xml");
				for (int j = 0; j < files.Length; j++)
				{
					this.mStrings.AddFile(files[j].FullName);
				}
				FontManager.Instance.LoadFonts(iLanguage);
				if (this.LanguageChanged != null)
				{
					this.LanguageChanged.Invoke();
				}
			}
		}

		// Token: 0x06001B20 RID: 6944 RVA: 0x000B976C File Offset: 0x000B796C
		public string GetNativeName(Language iLanguage)
		{
			string nativeName = CultureInfo.GetCultureInfo((int)iLanguage).NativeName;
			return char.ToUpper(nativeName[0]).ToString() + nativeName.Substring(1);
		}

		// Token: 0x170006A3 RID: 1699
		// (get) Token: 0x06001B21 RID: 6945 RVA: 0x000B97A7 File Offset: 0x000B79A7
		public Language[] AllLanguages
		{
			get
			{
				return this.mAllLanguages;
			}
		}

		// Token: 0x170006A4 RID: 1700
		// (get) Token: 0x06001B22 RID: 6946 RVA: 0x000B97AF File Offset: 0x000B79AF
		public Language CurrentLanguage
		{
			get
			{
				return this.mCurrentLanguage;
			}
		}

		// Token: 0x06001B23 RID: 6947 RVA: 0x000B97B8 File Offset: 0x000B79B8
		public Language GetLanguage(string iLanguage)
		{
			if (iLanguage.Equals("english", StringComparison.OrdinalIgnoreCase))
			{
				return Language.eng;
			}
			if (iLanguage.Equals("spanish", StringComparison.OrdinalIgnoreCase))
			{
				return Language.spa;
			}
			if (iLanguage.Equals("french", StringComparison.OrdinalIgnoreCase))
			{
				return Language.fra;
			}
			if (iLanguage.Equals("german", StringComparison.OrdinalIgnoreCase))
			{
				return Language.deu;
			}
			if (iLanguage.Equals("russian", StringComparison.OrdinalIgnoreCase))
			{
				return Language.rus;
			}
			return Language.eng;
		}

		// Token: 0x04001D5C RID: 7516
		private static readonly int PLAYERSHASH = "#players".GetHashCodeCustom();

		// Token: 0x04001D5D RID: 7517
		private string mPlayersString;

		// Token: 0x04001D5E RID: 7518
		private static LanguageManager sSingelton;

		// Token: 0x04001D5F RID: 7519
		private static volatile object sSingeltonLock = new object();

		// Token: 0x04001D60 RID: 7520
		private Language mCurrentLanguage = (Language)(-1);

		// Token: 0x04001D61 RID: 7521
		private StringCollection mStrings;

		// Token: 0x04001D62 RID: 7522
		private Language[] mAllLanguages;
	}
}
