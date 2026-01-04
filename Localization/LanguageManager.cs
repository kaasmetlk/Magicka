// Decompiled with JetBrains decompiler
// Type: Magicka.Localization.LanguageManager
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic;
using Magicka.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

#nullable disable
namespace Magicka.Localization;

public class LanguageManager
{
  private static readonly int PLAYERSHASH = "#players".GetHashCodeCustom();
  private string mPlayersString;
  private static LanguageManager sSingelton;
  private static volatile object sSingeltonLock = new object();
  private Language mCurrentLanguage = (Language) -1;
  private StringCollection mStrings;
  private Language[] mAllLanguages;

  public static LanguageManager Instance
  {
    get
    {
      if (LanguageManager.sSingelton == null)
      {
        lock (LanguageManager.sSingeltonLock)
        {
          if (LanguageManager.sSingelton == null)
            LanguageManager.sSingelton = new LanguageManager();
        }
      }
      return LanguageManager.sSingelton;
    }
  }

  public event Action LanguageChanged;

  private LanguageManager()
  {
    this.mStrings = new StringCollection();
    DirectoryInfo[] directories = new DirectoryInfo("content/Languages/").GetDirectories();
    List<Language> languageList = new List<Language>();
    for (int index = 0; index < directories.Length; ++index)
    {
      try
      {
        Language language = (Language) Enum.Parse(typeof (Language), directories[index].Name, true);
        languageList.Add(language);
      }
      catch
      {
      }
    }
    this.mAllLanguages = languageList.ToArray();
  }

  public string ISO6391() => this.ISO6391(this.CurrentLanguage);

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
      case Language.srp:
        return "sr";
      default:
        return "n/a";
    }
  }

  public void SetPlayerStrings()
  {
    List<string> stringList = new List<string>();
    Player[] players = Magicka.Game.Instance.Players;
    for (int index = 0; index < players.Length; ++index)
    {
      if (players[index].Playing)
        stringList.Add(players[index].GamerTag);
    }
    for (int index = 0; index < stringList.Count; ++index)
      this.mPlayersString = index != 0 ? (index != stringList.Count - 1 ? $"{this.mPlayersString}, {stringList[index]}" : $"{this.mPlayersString} & {stringList[index]}") : stringList[index];
  }

  public string GetString(int iID)
  {
    if (iID == LanguageManager.PLAYERSHASH)
      return this.mPlayersString;
    string str;
    return this.mStrings.TryGetValue(iID, out str) ? str : "NOT FOUND: " + (object) iID;
  }

  internal string GetStringWithReferencs(int iID)
  {
    if (iID == LanguageManager.PLAYERSHASH)
      return this.mPlayersString;
    string stringWithReferencs;
    if (!this.mStrings.TryGetValue(iID, out stringWithReferencs))
      return "NOT FOUND: " + (object) iID;
    for (int index = stringWithReferencs.IndexOf('#'); index >= 0; index = stringWithReferencs.IndexOf('#'))
    {
      int num = stringWithReferencs.IndexOf(';', index + 1);
      if (num > index)
      {
        int hashCodeCustom = stringWithReferencs.Substring(index, num - index).ToLowerInvariant().GetHashCodeCustom();
        stringWithReferencs = stringWithReferencs.Substring(0, index) + this.GetStringWithReferencs(hashCodeCustom) + stringWithReferencs.Substring(num + 1);
      }
      else
        break;
    }
    return stringWithReferencs;
  }

  internal string GetStringWithReferencs(int iID, ref Vector3 iReferenceColor)
  {
    if (iID == LanguageManager.PLAYERSHASH)
      return this.mPlayersString;
    string stringWithReferencs;
    if (!this.mStrings.TryGetValue(iID, out stringWithReferencs))
      return "NOT FOUND: " + (object) iID;
    for (int index = stringWithReferencs.IndexOf('#'); index >= 0; index = stringWithReferencs.IndexOf('#'))
    {
      int num = stringWithReferencs.IndexOf(';', index + 1);
      if (num > index)
      {
        int hashCodeCustom = stringWithReferencs.Substring(index, num - index).ToLowerInvariant().GetHashCodeCustom();
        stringWithReferencs = stringWithReferencs.Substring(0, index) + LanguageManager.ColorCode(ref iReferenceColor) + this.GetStringWithReferencs(hashCodeCustom, ref iReferenceColor) + LanguageManager.ColorCodeEnd() + stringWithReferencs.Substring(num + 1);
      }
      else
        break;
    }
    return stringWithReferencs;
  }

  internal string GetStringWithReferencs(int iID, ref Vector4 iReferenceColor)
  {
    if (iID == LanguageManager.PLAYERSHASH)
      return this.mPlayersString;
    string stringWithReferencs;
    if (!this.mStrings.TryGetValue(iID, out stringWithReferencs))
      return "NOT FOUND: " + (object) iID;
    for (int index = stringWithReferencs.IndexOf('#'); index >= 0; index = stringWithReferencs.IndexOf('#'))
    {
      int num = stringWithReferencs.IndexOf(';', index + 1);
      if (num > index)
      {
        int hashCodeCustom = stringWithReferencs.Substring(index, num - index).ToLowerInvariant().GetHashCodeCustom();
        stringWithReferencs = stringWithReferencs.Substring(0, index) + LanguageManager.ColorCode(ref iReferenceColor) + this.GetStringWithReferencs(hashCodeCustom, ref iReferenceColor) + LanguageManager.ColorCodeEnd() + stringWithReferencs.Substring(num + 1);
      }
      else
        break;
    }
    return stringWithReferencs;
  }

  public static string ColorCode(ref Vector3 iColor)
  {
    return $"[c={iColor.X.ToString((IFormatProvider) CultureInfo.InvariantCulture.NumberFormat)},{iColor.Y.ToString((IFormatProvider) CultureInfo.InvariantCulture.NumberFormat)},{iColor.Z.ToString((IFormatProvider) CultureInfo.InvariantCulture.NumberFormat)}]";
  }

  public static string ColorCode(ref Vector4 iColor)
  {
    return $"[c={iColor.X.ToString((IFormatProvider) CultureInfo.InvariantCulture.NumberFormat)},{iColor.Y.ToString((IFormatProvider) CultureInfo.InvariantCulture.NumberFormat)},{iColor.Z.ToString((IFormatProvider) CultureInfo.InvariantCulture.NumberFormat)},{iColor.W.ToString((IFormatProvider) CultureInfo.InvariantCulture.NumberFormat)}]";
  }

  public static string ColorCodeEnd() => "[/c]";

  public bool TryGetString(int iID, out string oString)
  {
    if (iID != LanguageManager.PLAYERSHASH)
      return this.mStrings.TryGetValue(iID, out oString);
    oString = this.mPlayersString;
    return true;
  }

  public string ParseReferences(string iText)
  {
    for (int index = iText.IndexOf('#'); index >= 0; index = iText.IndexOf('#'))
    {
      int num = iText.IndexOf(';', index + 1);
      if (num > index)
      {
        int hashCodeCustom = iText.Substring(index, num - index).ToLowerInvariant().GetHashCodeCustom();
        iText = $"{iText.Substring(0, index)}[c=1,1,1]{this.GetString(hashCodeCustom)}[/c]{iText.Substring(num + 1)}";
      }
      else
        break;
    }
    return iText;
  }

  public void SetLanguage(Language iLanguage)
  {
    bool flag = false;
    for (int index = 0; index < this.mAllLanguages.Length; ++index)
    {
      if (this.mAllLanguages[index] == iLanguage)
      {
        flag = true;
        break;
      }
    }
    if (!flag)
      iLanguage = Language.eng;
    if (iLanguage == this.mCurrentLanguage)
      return;
    this.mStrings.Clear();
    this.mCurrentLanguage = iLanguage;
    foreach (FileSystemInfo file in new DirectoryInfo("content/Languages/" + (object) this.mCurrentLanguage).GetFiles("*.loctable.xml"))
      this.mStrings.AddFile(file.FullName);
    FontManager.Instance.LoadFonts(iLanguage);
    if (this.LanguageChanged == null)
      return;
    this.LanguageChanged();
  }

  public string GetNativeName(Language iLanguage)
  {
    string nativeName = CultureInfo.GetCultureInfo((int) iLanguage).NativeName;
    return char.ToUpper(nativeName[0]).ToString() + nativeName.Substring(1);
  }

  public Language[] AllLanguages => this.mAllLanguages;

  public Language CurrentLanguage => this.mCurrentLanguage;

  public Language GetLanguage(string iLanguage)
  {
    if (iLanguage.Equals("english", StringComparison.OrdinalIgnoreCase))
      return Language.eng;
    if (iLanguage.Equals("spanish", StringComparison.OrdinalIgnoreCase))
      return Language.spa;
    if (iLanguage.Equals("french", StringComparison.OrdinalIgnoreCase))
      return Language.fra;
    if (iLanguage.Equals("german", StringComparison.OrdinalIgnoreCase))
      return Language.deu;
    return iLanguage.Equals("russian", StringComparison.OrdinalIgnoreCase) ? Language.rus : Language.eng;
  }
}
