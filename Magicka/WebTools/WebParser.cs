using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using Magicka.Achievements;

namespace Magicka.WebTools
{
	// Token: 0x0200027C RID: 636
	internal class WebParser
	{
		// Token: 0x060012CF RID: 4815 RVA: 0x00074CB8 File Offset: 0x00072EB8
		public static void CheckSteamDLCs(Action<bool> iCallback)
		{
			try
			{
				AchievementsManager.Instance.Request("http://store.steampowered.com/dlc/42910/", iCallback, new Action<HttpWebResponse, object>(WebParser.GetResponseCallback));
			}
			catch
			{
			}
		}

		// Token: 0x060012D0 RID: 4816 RVA: 0x00074CF8 File Offset: 0x00072EF8
		private static void GetResponseCallback(HttpWebResponse iResponse, object iArguments)
		{
			Action<bool> action = (Action<bool>)iArguments;
			try
			{
				StreamReader streamReader = new StreamReader(iResponse.GetResponseStream());
				string input = streamReader.ReadToEnd();
				streamReader.Close();
				iResponse.Close();
				MatchCollection matchCollection = Regex.Matches(input, "<a href=\"http://store.steampowered.com/app/([0-9]+)/\">.+?<p class=\"dlc_page_release_date\">[A-Za-z]+(.+?)</p>", RegexOptions.Singleline);
				bool obj = false;
				if (matchCollection.Count > 0)
				{
					for (int i = 0; i < matchCollection.Count; i++)
					{
						uint iAppID;
						if (uint.TryParse(matchCollection[i].Groups[1].Value, out iAppID) && !Helper.CheckDLCID(iAppID))
						{
							obj = true;
							break;
						}
					}
				}
				if (action != null)
				{
					action(obj);
				}
			}
			catch
			{
				if (action != null)
				{
					action(false);
				}
			}
		}
	}
}
