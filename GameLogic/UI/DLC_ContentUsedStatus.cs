// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.UI.DLC_ContentUsedStatus
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using System.Xml;

#nullable disable
namespace Magicka.GameLogic.UI;

public class DLC_ContentUsedStatus
{
  public string Name = "";
  public string ItemType = "UNKNOWN";
  public uint StoreAppID;
  public bool IsUsed;
  private uint mAppID;

  public uint AppID
  {
    get => this.mAppID;
    set
    {
      this.mAppID = value;
      if (this.StoreAppID != 0U)
        return;
      this.StoreAppID = value;
    }
  }

  public string ToXML()
  {
    string str = (int) this.StoreAppID == (int) this.AppID || this.StoreAppID == 0U ? "" : $" StoreAppID=\"{this.StoreAppID}\"";
    return $"<DLC_ContentNewStatus Type=\"{this.ItemType}\" Name=\"{this.Name}\" IsUsed=\"{(this.IsUsed ? (object) "true" : (object) "false")}\" AppID=\"{this.AppID}\"{str}/>\n";
  }

  public static DLC_ContentUsedStatus FromXml(XmlNode n)
  {
    DLC_ContentUsedStatus contentUsedStatus = new DLC_ContentUsedStatus();
    contentUsedStatus.StoreAppID = 0U;
    XmlAttribute attribute1 = n.Attributes["AppID"];
    if (attribute1 != null)
      contentUsedStatus.AppID = uint.Parse(attribute1.Value);
    XmlAttribute attribute2 = n.Attributes["Type"];
    if (attribute2 == null)
      return (DLC_ContentUsedStatus) null;
    if (string.IsNullOrEmpty(attribute2.Value))
      return (DLC_ContentUsedStatus) null;
    contentUsedStatus.ItemType = attribute2.Value;
    XmlAttribute attribute3 = n.Attributes["Name"];
    if (attribute3 == null)
      return (DLC_ContentUsedStatus) null;
    contentUsedStatus.Name = attribute3.Value;
    XmlAttribute attribute4 = n.Attributes["IsUsed"];
    if (attribute4 == null)
      return (DLC_ContentUsedStatus) null;
    contentUsedStatus.IsUsed = string.Compare(attribute4.Value, "true") == 0;
    XmlAttribute attribute5 = n.Attributes["StoreAppID"];
    if (attribute5 != null)
      contentUsedStatus.StoreAppID = uint.Parse(attribute5.Value);
    if (contentUsedStatus.StoreAppID == 0U)
      contentUsedStatus.StoreAppID = contentUsedStatus.AppID;
    return contentUsedStatus;
  }

  public static int CompareByType(DLC_ContentUsedStatus x, DLC_ContentUsedStatus y)
  {
    return x == null ? (y == null ? 0 : -1) : (y == null ? 1 : x.ItemType.CompareTo(y.ItemType));
  }
}
