// Decompiled with JetBrains decompiler
// Type: Magicka.WebTools.RSSReader
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using SteamWrapper;
using System;
using System.IO;
using System.Net;
using System.Xml;

#nullable disable
namespace Magicka.WebTools;

internal class RSSReader
{
  private const string URI = "http://bitsquid.blogspot.com/feeds/posts/default?alt=rss";
  private const string STORAGEPATH = "./SaveData/News.ini";
  private static RSSReader.RssItem sNewsItem;
  private static DateTime sLastNews;

  internal static bool TryGetNewsItem(out RSSReader.RssItem oItem)
  {
    oItem = new RSSReader.RssItem();
    if (!RSSReader.sNewsItem.Valid)
      return false;
    oItem = RSSReader.sNewsItem;
    return true;
  }

  internal static void Update()
  {
    HttpWebRequest state = (HttpWebRequest) WebRequest.Create("http://bitsquid.blogspot.com/feeds/posts/default?alt=rss");
    state.BeginGetResponse(new AsyncCallback(RSSReader.GetResponseCallback), (object) state);
  }

  private static void GetResponseCallback(IAsyncResult asynchResult)
  {
    HttpWebResponse response = (HttpWebResponse) ((WebRequest) asynchResult.AsyncState).EndGetResponse(asynchResult);
    Stream responseStream = response.GetResponseStream();
    RSSReader.ParseStream(responseStream);
    responseStream.Close();
    response.Close();
  }

  private static void ParseStream(Stream iStream)
  {
    XmlTextReader reader = new XmlTextReader(iStream);
    XmlDocument xmlDocument = new XmlDocument();
    xmlDocument.Load((XmlReader) reader);
    for (int i1 = 0; i1 < xmlDocument.ChildNodes.Count; ++i1)
    {
      if (xmlDocument.ChildNodes[i1].Name.Equals("rss", StringComparison.InvariantCultureIgnoreCase))
      {
        XmlNode childNode1 = xmlDocument.ChildNodes[i1];
        for (int i2 = 0; i2 < childNode1.ChildNodes.Count; ++i2)
        {
          if (childNode1.ChildNodes[i2].Name.Equals("channel", StringComparison.InvariantCultureIgnoreCase))
          {
            XmlNode childNode2 = childNode1.ChildNodes[i2];
            for (int i3 = 0; i3 < childNode2.ChildNodes.Count; ++i3)
            {
              XmlNode childNode3 = childNode2.ChildNodes[i3];
              if (childNode3.Name.Equals("item", StringComparison.InvariantCultureIgnoreCase))
              {
                string str1 = (string) null;
                string s = (string) null;
                string str2 = (string) null;
                for (int i4 = 0; i4 < childNode3.ChildNodes.Count; ++i4)
                {
                  if (childNode3.ChildNodes[i4].Name.Equals("description", StringComparison.InvariantCultureIgnoreCase))
                  {
                    string innerText = childNode3.ChildNodes[i4].InnerText;
                  }
                  else if (childNode3.ChildNodes[i4].Name.Equals("title", StringComparison.InvariantCultureIgnoreCase))
                    str1 = childNode3.ChildNodes[i4].InnerText;
                  else if (childNode3.ChildNodes[i4].Name.Equals("pubDate", StringComparison.InvariantCultureIgnoreCase))
                    s = childNode3.ChildNodes[i4].InnerText;
                  else if (childNode3.ChildNodes[i4].Name.Equals("link", StringComparison.InvariantCultureIgnoreCase))
                    str2 = childNode3.ChildNodes[i4].InnerText;
                }
                if (s != null)
                {
                  DateTime dateTime = DateTime.Parse(s);
                  if (dateTime > RSSReader.sLastNews && dateTime > RSSReader.sNewsItem.Date)
                  {
                    RSSReader.sNewsItem.Title = str1;
                    RSSReader.sNewsItem.Link = str2;
                    RSSReader.sNewsItem.Date = dateTime;
                  }
                }
              }
            }
            break;
          }
        }
        break;
      }
    }
  }

  private static void SaveStorage()
  {
    try
    {
      System.IO.File.WriteAllText("./SaveData/News.ini", RSSReader.sLastNews.ToString());
    }
    catch
    {
    }
  }

  internal static void LoadStorage()
  {
    try
    {
      RSSReader.sLastNews = DateTime.Parse(System.IO.File.ReadAllText("./SaveData/News.ini"));
    }
    catch
    {
    }
  }

  internal static void OpenItem()
  {
    if (!RSSReader.sNewsItem.Valid)
      return;
    if (RSSReader.sNewsItem.Link != null)
      SteamUtils.ActivateGameOverlayToWebPage(RSSReader.sNewsItem.Link);
    else
      SteamUtils.ActivateGameOverlayToStore(42910U, OverlayStoreFlag.None);
    RSSReader.sLastNews = RSSReader.sNewsItem.Date;
    RSSReader.sNewsItem = new RSSReader.RssItem();
    Game.Instance.AddLoadTask(new Action(RSSReader.SaveStorage));
  }

  public struct RssItem
  {
    public string Title;
    public string Link;
    public DateTime Date;

    public bool Valid => this.Title != null;
  }
}
