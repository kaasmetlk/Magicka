// Decompiled with JetBrains decompiler
// Type: Magicka.WebTools.WebParser
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Achievements;
using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

#nullable disable
namespace Magicka.WebTools;

internal class WebParser
{
  public static void CheckSteamDLCs(Action<bool> iCallback)
  {
    try
    {
      AchievementsManager.Instance.Request("http://store.steampowered.com/dlc/42910/", (object) iCallback, new Action<HttpWebResponse, object>(WebParser.GetResponseCallback));
    }
    catch
    {
    }
  }

  private static void GetResponseCallback(HttpWebResponse iResponse, object iArguments)
  {
    Action<bool> action = (Action<bool>) iArguments;
    try
    {
      StreamReader streamReader = new StreamReader(iResponse.GetResponseStream());
      string end = streamReader.ReadToEnd();
      streamReader.Close();
      iResponse.Close();
      MatchCollection matchCollection = Regex.Matches(end, "<a href=\"http://store.steampowered.com/app/([0-9]+)/\">.+?<p class=\"dlc_page_release_date\">[A-Za-z]+(.+?)</p>", RegexOptions.Singleline);
      bool flag = false;
      if (matchCollection.Count > 0)
      {
        for (int i = 0; i < matchCollection.Count; ++i)
        {
          uint result;
          if (uint.TryParse(matchCollection[i].Groups[1].Value, out result) && !Helper.CheckDLCID(result))
          {
            flag = true;
            break;
          }
        }
      }
      if (action == null)
        return;
      action(flag);
    }
    catch
    {
      if (action == null)
        return;
      action(false);
    }
  }
}
