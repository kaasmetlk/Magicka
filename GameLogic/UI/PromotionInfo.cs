// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.UI.PromotionInfo
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using System;
using System.Xml;

#nullable disable
namespace Magicka.GameLogic.UI;

public sealed class PromotionInfo
{
  public uint AppID;
  public DateTime ReleasedDate;
  public string Name;
  public bool IsOwndByPlayer;
  public bool IsNewToPlayer = true;
  private bool isDynamicallyLoaded;
  private string nonStoreURL = "";
  private int dynamicTextureID;

  public bool NewButOwnd => this.IsOwndByPlayer && this.IsNewToPlayer;

  public string NoneStoreURL
  {
    get => this.nonStoreURL;
    set => this.nonStoreURL = value;
  }

  public bool IsDynamicallyLoaded
  {
    get => this.isDynamicallyLoaded;
    set => this.isDynamicallyLoaded = value;
  }

  public int DynamicTextureID
  {
    get => this.dynamicTextureID;
    set => this.dynamicTextureID = value;
  }

  public static int CompareByDate(PromotionInfo x, PromotionInfo y)
  {
    return x == null ? (y == null ? 0 : -1) : (y == null ? 1 : y.ReleasedDate.CompareTo(x.ReleasedDate));
  }

  public string ToXML()
  {
    if (this.isDynamicallyLoaded)
      return "";
    return $"<DLC_SplashInfo AppID=\"{this.AppID}\" ReleaseDate=\"{this.ReleasedDate.ToShortDateString()}\" Name=\"{this.Name}\" IsNew=\"{(this.IsNewToPlayer ? (object) "true" : (object) "false")}\" IsOwnd=\"{(this.IsOwndByPlayer ? (object) "true" : (object) "false")}\" />\n";
  }

  public static PromotionInfo FromXml(XmlNode n)
  {
    XmlAttribute attribute1 = n.Attributes["AppID"];
    if (attribute1 == null)
      return (PromotionInfo) null;
    PromotionInfo promotionInfo = new PromotionInfo();
    promotionInfo.AppID = uint.Parse(attribute1.Value);
    XmlAttribute attribute2 = n.Attributes["ReleaseDate"];
    if (attribute2 == null)
      return (PromotionInfo) null;
    promotionInfo.ReleasedDate = DateTime.Parse(attribute2.Value);
    XmlAttribute attribute3 = n.Attributes["Name"];
    if (attribute3 == null)
      return (PromotionInfo) null;
    promotionInfo.Name = attribute3.Value;
    XmlAttribute attribute4 = n.Attributes["IsNew"];
    if (attribute4 == null)
      return (PromotionInfo) null;
    promotionInfo.IsNewToPlayer = string.Compare(attribute4.Value, "true") == 0;
    XmlAttribute attribute5 = n.Attributes["IsOwnd"];
    if (attribute5 == null)
      return (PromotionInfo) null;
    promotionInfo.IsOwndByPlayer = string.Compare(attribute5.Value, "true") == 0;
    return promotionInfo;
  }
}
