using System;
using System.IO;
using System.Net;
using System.Xml;
using SteamWrapper;

namespace Magicka.WebTools
{
	// Token: 0x020003D2 RID: 978
	internal class RSSReader
	{
		// Token: 0x06001DF2 RID: 7666 RVA: 0x000D301E File Offset: 0x000D121E
		internal static bool TryGetNewsItem(out RSSReader.RssItem oItem)
		{
			oItem = default(RSSReader.RssItem);
			if (RSSReader.sNewsItem.Valid)
			{
				oItem = RSSReader.sNewsItem;
				return true;
			}
			return false;
		}

		// Token: 0x06001DF3 RID: 7667 RVA: 0x000D3044 File Offset: 0x000D1244
		internal static void Update()
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("http://bitsquid.blogspot.com/feeds/posts/default?alt=rss");
			httpWebRequest.BeginGetResponse(new AsyncCallback(RSSReader.GetResponseCallback), httpWebRequest);
		}

		// Token: 0x06001DF4 RID: 7668 RVA: 0x000D3078 File Offset: 0x000D1278
		private static void GetResponseCallback(IAsyncResult asynchResult)
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)asynchResult.AsyncState;
			HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.EndGetResponse(asynchResult);
			Stream responseStream = httpWebResponse.GetResponseStream();
			RSSReader.ParseStream(responseStream);
			responseStream.Close();
			httpWebResponse.Close();
		}

		// Token: 0x06001DF5 RID: 7669 RVA: 0x000D30B8 File Offset: 0x000D12B8
		private static void ParseStream(Stream iStream)
		{
			XmlTextReader reader = new XmlTextReader(iStream);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(reader);
			for (int i = 0; i < xmlDocument.ChildNodes.Count; i++)
			{
				if (xmlDocument.ChildNodes[i].Name.Equals("rss", StringComparison.InvariantCultureIgnoreCase))
				{
					XmlNode xmlNode = xmlDocument.ChildNodes[i];
					for (int j = 0; j < xmlNode.ChildNodes.Count; j++)
					{
						if (xmlNode.ChildNodes[j].Name.Equals("channel", StringComparison.InvariantCultureIgnoreCase))
						{
							XmlNode xmlNode2 = xmlNode.ChildNodes[j];
							for (int k = 0; k < xmlNode2.ChildNodes.Count; k++)
							{
								XmlNode xmlNode3 = xmlNode2.ChildNodes[k];
								if (xmlNode3.Name.Equals("item", StringComparison.InvariantCultureIgnoreCase))
								{
									string title = null;
									string text = null;
									string link = null;
									for (int l = 0; l < xmlNode3.ChildNodes.Count; l++)
									{
										if (xmlNode3.ChildNodes[l].Name.Equals("description", StringComparison.InvariantCultureIgnoreCase))
										{
											string innerText = xmlNode3.ChildNodes[l].InnerText;
										}
										else if (xmlNode3.ChildNodes[l].Name.Equals("title", StringComparison.InvariantCultureIgnoreCase))
										{
											title = xmlNode3.ChildNodes[l].InnerText;
										}
										else if (xmlNode3.ChildNodes[l].Name.Equals("pubDate", StringComparison.InvariantCultureIgnoreCase))
										{
											text = xmlNode3.ChildNodes[l].InnerText;
										}
										else if (xmlNode3.ChildNodes[l].Name.Equals("link", StringComparison.InvariantCultureIgnoreCase))
										{
											link = xmlNode3.ChildNodes[l].InnerText;
										}
									}
									if (text != null)
									{
										DateTime dateTime = DateTime.Parse(text);
										if (dateTime > RSSReader.sLastNews && dateTime > RSSReader.sNewsItem.Date)
										{
											RSSReader.sNewsItem.Title = title;
											RSSReader.sNewsItem.Link = link;
											RSSReader.sNewsItem.Date = dateTime;
										}
									}
								}
							}
							return;
						}
					}
					return;
				}
			}
		}

		// Token: 0x06001DF6 RID: 7670 RVA: 0x000D3318 File Offset: 0x000D1518
		private static void SaveStorage()
		{
			try
			{
				File.WriteAllText("./SaveData/News.ini", RSSReader.sLastNews.ToString());
			}
			catch
			{
			}
		}

		// Token: 0x06001DF7 RID: 7671 RVA: 0x000D3354 File Offset: 0x000D1554
		internal static void LoadStorage()
		{
			try
			{
				string s = File.ReadAllText("./SaveData/News.ini");
				RSSReader.sLastNews = DateTime.Parse(s);
			}
			catch
			{
			}
		}

		// Token: 0x06001DF8 RID: 7672 RVA: 0x000D338C File Offset: 0x000D158C
		internal static void OpenItem()
		{
			if (!RSSReader.sNewsItem.Valid)
			{
				return;
			}
			if (RSSReader.sNewsItem.Link != null)
			{
				SteamUtils.ActivateGameOverlayToWebPage(RSSReader.sNewsItem.Link);
			}
			else
			{
				SteamUtils.ActivateGameOverlayToStore(42910U, OverlayStoreFlag.None);
			}
			RSSReader.sLastNews = RSSReader.sNewsItem.Date;
			RSSReader.sNewsItem = default(RSSReader.RssItem);
			Game.Instance.AddLoadTask(new Action(RSSReader.SaveStorage));
		}

		// Token: 0x04002076 RID: 8310
		private const string URI = "http://bitsquid.blogspot.com/feeds/posts/default?alt=rss";

		// Token: 0x04002077 RID: 8311
		private const string STORAGEPATH = "./SaveData/News.ini";

		// Token: 0x04002078 RID: 8312
		private static RSSReader.RssItem sNewsItem;

		// Token: 0x04002079 RID: 8313
		private static DateTime sLastNews;

		// Token: 0x020003D3 RID: 979
		public struct RssItem
		{
			// Token: 0x1700075B RID: 1883
			// (get) Token: 0x06001DFA RID: 7674 RVA: 0x000D3406 File Offset: 0x000D1606
			public bool Valid
			{
				get
				{
					return this.Title != null;
				}
			}

			// Token: 0x0400207A RID: 8314
			public string Title;

			// Token: 0x0400207B RID: 8315
			public string Link;

			// Token: 0x0400207C RID: 8316
			public DateTime Date;
		}
	}
}
